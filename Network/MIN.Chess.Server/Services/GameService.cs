using System.Text.Json;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
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
    private readonly MessageService messageService;
    private readonly RoomService roomService;
    private readonly TimerService timerService;

    public readonly static GameState GameState = new(new Board(), Player.None);
    public static bool GameStarted;

    private bool gameOverResultSent;
    private Move? lastMove;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameService"/>
    /// </summary>
    public GameService(MessageService messageService,
        RoomService roomService,
        VoteService voteService,
        TimerService timerService,
        ILoggerProvider logger)
    {
        this.logger = logger;
        this.messageService = messageService;
        this.roomService = roomService;
        this.timerService = timerService;

        timerService.OnSettingsChanged += async () => await SendStateUpdate();
        timerService.OnTimeOut += async (player) =>
        {
            var result = new Result(player.Opponent(), EndReason.Timeout);
            GameState.Result = result;
            await SendGameResult(result);
        };

        roomService.OnInitRequestReceived += async (sender) => await SendInitResponse(sender);
        roomService.OnPlayerJoined += async (role) =>
        {
            if (role != Player.None)
            {
                await ManangeStartGameStateOutOfPlayerPresence();
            }
        };
        roomService.OnPlayerLeft += async (player) =>
        {
            if (player != Player.None)
            {
                timerService.StopTimer();
                GameState.CurrentPlayer = Player.None;
            }
        };

        voteService.OnRestart += async () => await Restart();
        voteService.OnResign += async (winner) =>
        {
            var result = new Result(winner, EndReason.Resign);
            GameState.Result = result;
            await SendGameResult(result);
        };
        voteService.OnDraw += async () =>
        {
            var result = Result.Draw(EndReason.Draw);
            GameState.Result = result;
            await SendGameResult(result);
        };
    }

    /// <summary>
    /// Обновление состояние по запросу
    /// </summary>
    public async Task HandleStateUpdate(INetworkMessage message)
    {
        logger.Log("StateUpdateRequestMessage received");
        logger.Log($"sending gamestate {GameState.ToString()}");
        await messageService.BroadcastExcludingAsync(new StateUpdateMessage()
        {
            StateString = GameState.ToString(),
            CurrentTurn = GameState.CurrentPlayer,
            LastMove = lastMove?.ToUci(),
        }, Guid.Parse(message.SenderId));
    }

    /// <summary>
    /// Обработать ход
    /// </summary>
    public async Task HandleMove(MoveRequestMessage moveRequestMessage)
    {
        var senderId = Guid.Parse(moveRequestMessage.SenderId);
        var senderRole = roomService.ResolveRole(senderId);

        if (GameState.IsGameOver())
        {
            await messageService.SendErrorAsync(senderId, "Игра уже окончена");
            return;
        }

        logger.Log($"moveRequestMessage received {moveRequestMessage.MoveUciNotation}");

        var move = MoveParser.Parse(moveRequestMessage.MoveUciNotation, GameState.Board);

        logger.Log("checking legalability . . .");


        if (move.IsLegal(GameState.Board) && senderRole == GameState.CurrentPlayer)
        {
            GameState.MakeMove(move);
            lastMove = move;
        }
        else
        {
            await messageService.SendErrorAsync(senderId, "Нелегальный ход");
            return;
        }

        GameStarted = timerService.ManageTimeAfterMove(senderRole, GameStarted);

        var (whiteMs, blackMs) = timerService.GetCurrentRemainingMs();

        logger.Log("move is legal, sending move update");

        await messageService.BroadcastExcludingAsync(new MoveUpdateMessage()
        {
            MoveUci = moveRequestMessage.MoveUciNotation,
            CurrentPlayer = GameState.CurrentPlayer,
            WhiteMs = timerService.TimerEnabled ? whiteMs : null,
            BlackMs = timerService.TimerEnabled ? blackMs : null,
        }, senderId);

        GameState.CheckForGameOver();

        if (GameState.IsGameOver())
        {
            logger.Log($"game over, sending result: {JsonSerializer.Serialize(GameState.Result!.Value)}");
            await messageService.BroadcastAsync(new GameResultMessage()
            {
                Result = GameState.Result!.Value,
            });
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
        timerService.StopTimer();
        await messageService.BroadcastAsync(new GameResultMessage()
        {
            Result = result,
        });
    }

    /// <summary>
    /// Перезапустить игру
    /// </summary>
    public async Task Restart()
    {
        GameState.Result = null;
        lastMove = null;
        GameStarted = false;
        gameOverResultSent = false;
        timerService.StopTimer();

        await ManangeStartGameStateOutOfPlayerPresence();
    }

    private async Task ManangeStartGameStateOutOfPlayerPresence()
    {
        if (roomService.IsBothPlayersJoined())
        {
            GameState.Board = Board.Initial();
            GameState.CurrentPlayer = Player.White;

            await SendStateUpdate();
        }

        if (roomService.IsWhitePlayersJoined())
        {
            GameState.Board = !roomService.IsBlackPlayersJoined()
                ? Board.Initial(Player.White)
                : Board.Initial();
        }
        else if (roomService.IsBlackPlayersJoined())
        {
            GameState.Board = !roomService.IsWhitePlayersJoined()
                ? Board.Initial(Player.Black)
                : Board.Initial();
        }
    }

    /// <summary>
    /// Отправить информацию об игре и учатснике
    /// </summary>
    public async Task SendInitResponse(Participant participant)
    {
        var role = roomService.ResolveRole(participant.Id);
        var (whiteMs, blackMs) = timerService.GetCurrentRemainingMs();
        var initResponse = new InitResponseMessage
        {
            StateString = new StateString(GameState.CurrentPlayer, GameState.Board).ToString(),
            CurrentTurn = GameState.CurrentPlayer,
            RequesterRole = role,
            SelfParticipant = participant,
            LastMove = lastMove?.ToUci(),
            ActivePlayers = roomService.GetPlayers(),
            SpectorsCount = roomService.SpectatorCount,
            WhiteMs = timerService.TimerEnabled ? whiteMs : null,
            BlackMs = timerService.TimerEnabled ? blackMs : null,
            TimerBonusMs = timerService.TimerBonusMs / 1000
        };

        await messageService.SendToAsync(initResponse, participant.Id);
    }

    /// <summary>
    /// Отправить обновление состояния всем участникам
    /// </summary>
    public async Task SendStateUpdate()
    {
        var (whiteMs, blackMs) = timerService.GetCurrentRemainingMs();
        logger.Log($"Sending state update: whiteMs={whiteMs}ms, blackMs={blackMs}ms");

        await messageService.BroadcastAsync(new StateUpdateMessage()
        {
            StateString = GameState.ToString(),
            CurrentTurn = GameState.CurrentPlayer,
            LastMove = null,
            WhiteMs = timerService.TimerEnabled ? whiteMs : null,
            BlackMs = timerService.TimerEnabled ? blackMs : null,
            TimerBonusSeconds = timerService.TimerBonusMs / 1000
        });
    }
}
