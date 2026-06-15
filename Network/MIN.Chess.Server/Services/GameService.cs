using System.Text.Json;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния игры и обработка событий
/// </summary>
internal class GameService
{
    private readonly ILoggerProvider logger;
    private readonly IClientProcessBridge bridge;
    private readonly GameState gameState = new(new Board(), Player.None);

    private bool gameOverResultSent;
    private Move? lastMove;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameService"/>
    /// </summary>
    public GameService(ILoggerProvider logger, IClientProcessBridge bridge)
    {
        this.logger = logger;
        this.bridge = bridge;
    }

    /// <summary>
    /// Обновление состояние по запросу
    /// </summary>
    public async Task HandleStateUpdate(INetworkMessage message)
    {
        logger.Log("StateUpdateRequestMessage received");
        logger.Log($"sending gamestate {gameState.ToString()}");
        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            LastMove = lastMove?.ToUci(),
        }, RecipientId: Guid.Parse(message.SenderId), cancellationToken: appCts.Token);
    }

    /// <summary>
    /// Обработать ход
    /// </summary>
    public async Task HandleMove(MoveRequestMessage moveRequestMessage)
    {
        if (gameState.IsGameOver())
        {
            await SendErrorMessageFor(moveRequestMessage.SenderId, "Игра уже окончена");
            return;
        }

        logger.Log($"moveRequestMessage received {moveRequestMessage.MoveUciNotation}");

        var move = MoveParser.Parse(moveRequestMessage.MoveUciNotation, gameState.Board);

        logger.Log("checking legalability . . .");

        var senderId = Guid.Parse(moveRequestMessage.SenderId);
        var senderRole = ResolveRole(senderId);

        if (move.IsLegal(gameState.Board) && senderRole == gameState.CurrentPlayer)
        {
            gameState.MakeMove(move);
            lastMove = move;
        }
        else
        {
            await SendErrorMessageFor(moveRequestMessage.SenderId, "Нелегальный ход");
            return;
        }

        if (timerEnabled)
        {
            lock (timerLock)
            {
                var now = DateTime.UtcNow;

                if (!gameStarted)
                {
                    gameStarted = true;

                    if (senderRole == Player.White)
                    {
                        whiteRemainingMs = timerInitialMs + timerBonusMs;
                        blackRemainingMs = timerInitialMs;
                    }
                    else
                    {
                        blackRemainingMs = timerInitialMs + timerBonusMs;
                        whiteRemainingMs = timerInitialMs;
                    }

                    activeTimer = senderRole.Opponent();
                    turnStartTime = now;
                    StartTimerTask();
                }
                else
                {
                    var elapsed = (long)(now - turnStartTime).TotalMilliseconds;

                    if (senderRole == Player.White)
                    {
                        whiteRemainingMs = Math.Max(0, whiteRemainingMs - elapsed);
                        whiteRemainingMs += timerBonusMs;
                    }
                    else
                    {
                        blackRemainingMs = Math.Max(0, blackRemainingMs - elapsed);
                        blackRemainingMs += timerBonusMs;
                    }

                    activeTimer = senderRole.Opponent();
                    turnStartTime = now;
                }
            }
        }

        var (whiteMs, blackMs) = GetCurrentRemainingMs();

        logger.Log("move is legal, sending move update");

        await bridge.SendMessage(new MoveUpdateMessage()
        {
            MoveUci = moveRequestMessage.MoveUciNotation,
            CurrentPlayer = gameState.CurrentPlayer,
            WhiteMs = timerEnabled ? whiteMs : null,
            BlackMs = timerEnabled ? blackMs : null,
        },
        BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(senderId),
        cancellationToken: appCts.Token);

        gameState.CheckForGameOver();

        if (gameState.IsGameOver())
        {
            logger.Log($"game over, sending result: {JsonSerializer.Serialize(gameState.Result!.Value)}");
            await bridge.SendMessage(new GameResultMessage()
            {
                Result = gameState.Result!.Value,
            }, cancellationToken: appCts.Token);
            return;
        }
    }

    /// <summary>
    /// Отправить результат игры
    /// </summary>
    public async Task SendGameResult(Result result)
    {
        if (gameOverResultSent)
        {
            return;
        }
        gameOverResultSent = true;
        StopTimer();
        await bridge.SendMessage(new GameResultMessage() { Result = result }, cancellationToken: appCts.Token);
    }

    /// <summary>
    /// Перезапустить игру
    /// </summary>
    public async Task Restart()
    {
        gameState.Result = null;
        lastMove = null;
        gameStarted = false;
        gameOverResultSent = false;

        if (timerEnabled)
        {
            StopTimer();
            whiteRemainingMs = 0;
            blackRemainingMs = 0;
        }

        if (players[Player.White] == null)
        {
            gameState.Board = players[Player.Black] == null
                ? Board.Initial(Player.White)
                : Board.Initial();
        }
        else if (players[Player.Black] == null)
        {
            gameState.Board = players[Player.White] == null
                ? Board.Initial(Player.Black)
                : Board.Initial();
        }

        if (IsBothPlayersJoined())
        {
            gameState.Board = Board.Initial();
            gameState.CurrentPlayer = Player.White;
            timerEnabled = false;

            await SendStateUpdate();
        }
    }

    /// <summary>
    /// Отправить информацию об игре и учатснике
    /// </summary>
    public async Task SendInitResponse(Participant participant)
    {
        var role = ResolveRole(participant.Id);
        var (whiteMs, blackMs) = GetCurrentRemainingMs();
        var initResponse = new InitResponseMessage
        {
            StateString = new StateString(gameState.CurrentPlayer, gameState.Board).ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            RequesterRole = role,
            SelfParticipant = participant,
            LastMove = lastMove?.ToUci(),
            ActivePlayers = players,
            SpectorsCount = spectators.Count,
            WhiteMs = timerEnabled ? whiteMs : null,
            BlackMs = timerEnabled ? blackMs : null,
            TimerBonusSeconds = timerBonusMs / 1000
        };

        await bridge.SendMessage(initResponse, RecipientId: participant.Id, cancellationToken: appCts.Token);
    }

    /// <summary>
    /// Отправить обновление состояния всем участникам
    /// </summary>
    public async Task SendStateUpdate(Guid? broadcastExcludeId = null)
    {
        var (whiteMs, blackMs) = GetCurrentRemainingMs();
        logger.Log($"Sending state update: whiteMs={whiteMs}ms, blackMs={blackMs}ms");

        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            LastMove = null,
            WhiteMs = timerEnabled ? whiteMs : null,
            BlackMs = timerEnabled ? blackMs : null,
            TimerBonusSeconds = timerBonusMs / 1000
        }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(broadcastExcludeId), cancellationToken: appCts.Token);
    }

    /// <summary>
    /// Отправить сообщение об ошибке участнику
    /// </summary>
    public async Task SendErrorMessageFor(string senderId, string message)
    {
        await bridge.SendMessage(new ErrorMessage()
        {
            Message = message,
        }, RecipientId: Guid.Parse(senderId), cancellationToken: appCts.Token);
    }
}
