using System.Text.Json;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server.Services
{
    /// <summary>
    /// Оркестратор сервера, маршрутизация сообщений
    /// </summary>
    internal class GameServerService
    {
        private readonly IClientProcessBridge bridge = new ClientProcessBridge();
        private readonly ILoggerProvider logger;
        private readonly GameState gameState = new(new Board(), Player.None);
        private readonly Dictionary<Guid, InitRequestMessage> pendingInits = [];
        private readonly Dictionary<Guid, Participant> spectators = [];
        private readonly CancellationTokenSource appCts = new();
        private readonly List<int> avaibleTimerValues = [1, 3, 5, 10, 30];
        private readonly List<int> avaibleBonusTimerValues = [0, 1, 2, 3, 5, 10, 20];
        private readonly Dictionary<Player, Participant?> players = new()
        {
            { Player.White, null },
            { Player.Black, null },
        };

        private readonly Dictionary<Player, bool> restartVote = new()
        {
            { Player.White, false },
            { Player.Black, false },
        };

        private readonly Dictionary<Player, bool> drawVote = new()
        {
            { Player.White, false },
            { Player.Black, false },
        };

        private const int ConnectionTimeoutMs = 5_000;

        // Timers
        private long whiteRemainingMs;
        private long blackRemainingMs;
        private Player? activeTimer;
        private DateTime turnStartTime;
        private int timerInitialMs;
        private int timerBonusMs;
        private bool timerEnabled;
        private bool gameStarted;
        private Task? timerTask;
        private CancellationTokenSource? timerCts;
        private readonly object timerLock = new();
        private bool gameOverResultSent;

        private Move? lastMove;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GameServerService"/>
        /// </summary>
        public GameServerService(ILoggerProvider logger)
        {
            this.logger = logger;
            bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
        }

        /// <summary>
        /// Инициализировать сервис с подключением к MIN
        /// </summary>
        public async Task InitializeNetworking(string connectionString)
        {
            try
            {
                logger.Log("Connecting to MIN . . .");
                await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, appCts.Token);

                bridge.NetworkMessageReceived += NetworkMessageReceived;
                bridge.ParticipantConnected += ParticipantConnected;
                bridge.ParticipantDisconnected += ParticipantDisconnected;

                await bridge.StartListeningAsync(appCts.Token);

                logger.Log("Connection succeded, sending ready message");
                await bridge.SendReadyMessage(appCts.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Failed: {ex.Message}");
            }
        }

        private async void NetworkMessageReceived(object? sender, INetworkMessage message)
        {
            await HandleMessage(message);
        }

        private async void ParticipantConnected(object? sender, Participant participant)
        {
            await HandleParticipantJoining(participant);
        }

        private async void ParticipantDisconnected(object? sender, Guid participantId)
        {
            await HandleParticipantDisconnecting(participantId);
        }

        private async Task HandleMessage(INetworkMessage message)
        {
            switch (message)
            {
                case InitRequestMessage initRequestMessage:
                    await HandleInit(initRequestMessage);
                    break;

                case StateUpdateRequestMessage _:
                    await StateUpdate(message);
                    break;

                case MoveRequestMessage moveRequestMessage:
                    await HandleMove(moveRequestMessage);
                    break;

                case ResignRequestMessage _:
                    await HandleResign(message);
                    break;

                case DrawRequestMessage _:
                    await HandleDraw(message);
                    break;

                case RestartRequestMessage _:
                    await HandleRestart(message);
                    break;

                case ChangeRoleRequestMessage _:
                    await HandleChangeRole(message);
                    break;

                case GameSettingsMessage gameSettingsMessage:
                    await HandleGameSettings(gameSettingsMessage);
                    break;
            }
        }

        private async Task HandleInit(InitRequestMessage message)
        {
            logger.Log("InitRequestMessage received");
            logger.Log($"sending init {gameState.ToString()}");

            var senderId = Guid.Parse(message.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"ROLE {role}");

            players.TryGetValue(role, out var sender);

            if (sender == null)
            {
                spectators.TryGetValue(senderId, out sender);
            }

            if (sender != null)
            {
                await SendInitResponse(sender);
                return;
            }

            pendingInits[senderId] = message;
        }

        private async Task StateUpdate(INetworkMessage message)
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

        private async Task HandleMove(MoveRequestMessage moveRequestMessage)
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

            logger.Log("move is legal, sending move update");

            var (whiteMs, blackMs) = GetCurrentRemainingMs();

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

        private async Task HandleResign(INetworkMessage message)
        {
            var senderId = Guid.Parse(message.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"Resign received from {role}");

            if (role == Player.None)
            {
                await SendErrorMessageFor(message.SenderId, "У вас нету права на это");
                return;
            }

            await bridge.SendMessage(new GameResultMessage()
            {
                Result = new Result(role.Opponent(), EndReason.Resign),
            }, cancellationToken: appCts.Token);
        }

        private async Task HandleDraw(INetworkMessage message)
        {
            var senderId = Guid.Parse(message.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"Draw request received from {role}");

            if (role == Player.None)
            {
                await SendErrorMessageFor(message.SenderId, "У вас нету права на это");
                return;
            }

            if (drawVote[role] == true)
            {
                return;
            }

            if (!IsBothPlayersJoined())
            {
                return;
            }
            else
            {
                var votes = 1;
                drawVote[role] = true;
                if (drawVote[Player.White] && drawVote[Player.Black])
                {
                    votes = 0;
                    drawVote[Player.White] = false;
                    drawVote[Player.Black] = false;
                    await SendGameResult(Result.Draw(EndReason.Draw));
                }

                await bridge.SendMessage(new DrawVoteMessage()
                {
                    CurrentVotes = votes
                }, cancellationToken: appCts.Token);
            }
        }

        private async Task HandleRestart(INetworkMessage message)
        {
            var senderId = Guid.Parse(message.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"Restart received from {role}");

            if (role == Player.None)
            {
                await SendErrorMessageFor(message.SenderId, "У вас нету права на это");
                return;
            }

            if (restartVote[role] == true)
            {
                return;
            }

            if (!IsBothPlayersJoined())
            {
                return;
            }
            else
            {
                var votes = 1;
                restartVote[role] = true;
                if (restartVote[Player.White] && restartVote[Player.Black])
                {
                    await Restart();
                    votes = 0;
                    restartVote[Player.White] = false;
                    restartVote[Player.Black] = false;
                }

                await bridge.SendMessage(new RestartVoteMessage()
                {
                    CurrentVotes = votes
                }, cancellationToken: appCts.Token);
            }
        }

        private async Task HandleChangeRole(INetworkMessage message)
        {
            var senderId = Guid.Parse(message.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"ChangeRole received from {role}");

            if (IsBothPlayersJoined())
            {
                await SendErrorMessageFor(message.SenderId, "Не кого заменять");
                return;
            }

            spectators.TryGetValue(senderId, out var participant);

            if (participant == null)
            {
                logger.Log($"Didnt found spectator {senderId}");
                return;
            }

            if (players[Player.White] == null)
            {
                logger.Log($"{participant.Name} joined as White");
                players[Player.White] = participant;

                gameState.Board = players[Player.Black] == null
                    ? Board.Initial(Player.White)
                    : Board.Initial();

                if (players[Player.Black] != null)
                {
                    gameState.CurrentPlayer = Player.White;
                }

                await bridge.SendMessage(new PlayerConnectedMessage()
                {
                    Role = Player.White,
                    Participant = participant,
                }, cancellationToken: appCts.Token);
            }
            else if (players[Player.Black] == null)
            {
                logger.Log($"{participant.Name} joined as Black");
                players[Player.Black] = participant;

                gameState.Board = Board.Initial();
                gameState.CurrentPlayer = Player.White;

                await bridge.SendMessage(new PlayerConnectedMessage()
                {
                    Role = Player.Black,
                    Participant = participant,
                }, cancellationToken: appCts.Token);
            }

            spectators.Remove(senderId);

            await bridge.SendMessage(new PlayerDisconnectedMessage()
            {
                Role = Player.None,
            }, cancellationToken: appCts.Token);

            if (IsBothPlayersJoined())
            {
                await SendStateUpdate();
            }
        }

        private async Task HandleGameSettings(GameSettingsMessage gameSettingsMessage)
        {
            var senderId = Guid.Parse(gameSettingsMessage.SenderId);
            var role = ResolveRole(senderId);

            logger.Log($"Settings received from {role}");

            if (role != Player.White)
            {
                await SendErrorMessageFor(gameSettingsMessage.SenderId, "У вас нету права на это");
                return;
            }

            if (gameStarted)
            {
                await SendErrorMessageFor(gameSettingsMessage.SenderId, "Настройки можно изменить только до начала игры");
                return;
            }

            if (gameSettingsMessage.TimerInitialMinutes != null
                && !avaibleTimerValues.Contains(gameSettingsMessage.TimerInitialMinutes.Value))
            {
                await SendErrorMessageFor(gameSettingsMessage.SenderId, "Время должно находиться в правильном диапазоне");
                return;
            }

            if (!avaibleBonusTimerValues.Contains(gameSettingsMessage.TimerBonusSeconds))
            {
                await SendErrorMessageFor(gameSettingsMessage.SenderId, "Добавочное время должно находиться в правильном диапазоне");
                return;
            }

            timerEnabled = gameSettingsMessage.TimerInitialMinutes.HasValue;
            timerInitialMs = (gameSettingsMessage.TimerInitialMinutes ?? 0) * 60 * 1000;
            timerBonusMs = gameSettingsMessage.TimerBonusSeconds * 1000;
            whiteRemainingMs = timerInitialMs;
            blackRemainingMs = timerInitialMs;

            logger.Log($"Timer settings stored: enabled={timerEnabled}, initial={timerInitialMs}ms, bonus={timerBonusMs}ms");

            if (IsBothPlayersJoined())
            {
                await SendStateUpdate();
            }
        }

        private void StartTimerTask()
        {
            if (timerTask != null)
            {
                return;
            }
            timerCts = CancellationTokenSource.CreateLinkedTokenSource(appCts.Token);
            timerTask = TimerCheckLoop(timerCts.Token);
        }

        private void StopTimer()
        {
            timerCts?.Cancel();
            timerCts?.Dispose();
            timerCts = null;
            timerTask = null;
            activeTimer = null;
        }

        private async Task TimerCheckLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, ct);

                    Player? timeoutLoser = null;

                    lock (timerLock)
                    {
                        if (!timerEnabled || activeTimer == null || gameState.IsGameOver())
                        {
                            continue;
                        }

                        var elapsed = (long)(DateTime.UtcNow - turnStartTime).TotalMilliseconds;

                        if (activeTimer == Player.White && whiteRemainingMs - elapsed <= 0)
                        {
                            timeoutLoser = Player.White;
                        }
                        else if (activeTimer == Player.Black && blackRemainingMs - elapsed <= 0)
                        {
                            timeoutLoser = Player.Black;
                        }
                    }

                    if (timeoutLoser != null)
                    {
                        logger.Log($"{timeoutLoser} timed out");
                        var result = new Result(timeoutLoser.Value.Opponent(), EndReason.Timeout);
                        gameState.Result = result;
                        await SendGameResult(result);
                        return;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private (long whiteMs, long blackMs) GetCurrentRemainingMs()
        {
            if (!timerEnabled)
            {
                return (0, 0);
            }

            lock (timerLock)
            {
                var white = whiteRemainingMs;
                var black = blackRemainingMs;
                var elapsed = (long)(DateTime.UtcNow - turnStartTime).TotalMilliseconds;

                if (activeTimer == Player.White)
                {
                    white = Math.Max(0, white - elapsed);
                }
                else if (activeTimer == Player.Black)
                {
                    black = Math.Max(0, black - elapsed);
                }

                return (white, black);
            }
        }

        private async Task SendGameResult(Result result)
        {
            if (gameOverResultSent)
            {
                return;
            }
            gameOverResultSent = true;
            StopTimer();
            await bridge.SendMessage(new GameResultMessage() { Result = result }, cancellationToken: appCts.Token);
        }

        private IEnumerable<Guid>? GetDefaultExcludeBroadcastListWith(Guid? excludeId)
        {
            if (excludeId != null)
            {
                return pendingInits.Keys.Append(excludeId.Value);
            }
            else
            {
                return pendingInits.Keys;
            }
        }

        private async Task Restart()
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

        private Player ResolveRole(Guid senderId)
        {
            if (players[Player.White]?.Id == senderId || players[Player.White] == null)
            {
                return Player.White;
            }
            else if (players[Player.Black]?.Id == senderId || players[Player.Black] == null)
            {
                return Player.Black;
            }
            else
            {
                return Player.None;
            }
        }

        private async Task HandleParticipantJoining(Participant participant)
        {
            if (IsBothPlayersJoined())
            {
                logger.Log($"{participant.Id} joined as Spectator");
                spectators[participant.Id] = participant;

                await bridge.SendMessage(new PlayerConnectedMessage()
                {
                    Role = Player.None,
                    Participant = participant,
                }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(participant.Id),
                cancellationToken: appCts.Token);

                if (pendingInits.Remove(participant.Id, out _))
                {
                    await SendInitResponse(participant);
                }

                return;
            }

            lastMove = null;

            if (players[Player.White] == null)
            {
                logger.Log($"{participant.Name} joined as White");
                players[Player.White] = participant;

                gameState.Board = players[Player.Black] == null
                    ? Board.Initial(Player.White)
                    : Board.Initial();

                if (players[Player.Black] != null)
                {
                    gameState.CurrentPlayer = Player.White;
                }

                await bridge.SendMessage(new PlayerConnectedMessage()
                {
                    Role = Player.White,
                    Participant = participant,
                }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(participant.Id),
                cancellationToken: appCts.Token);
            }
            else if (players[Player.Black] == null)
            {
                logger.Log($"{participant.Name} joined as Black");
                players[Player.Black] = participant;

                gameState.Board = Board.Initial();
                gameState.CurrentPlayer = Player.White;

                await bridge.SendMessage(new PlayerConnectedMessage()
                {
                    Role = Player.Black,
                    Participant = participant,
                }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(participant.Id),
                cancellationToken: appCts.Token);
            }

            if (pendingInits.Remove(participant.Id, out _))
            {
                await SendInitResponse(participant);
            }

            if (IsBothPlayersJoined())
            {
                await SendStateUpdate(participant.Id);
            }
        }

        private async Task SendInitResponse(Participant participant)
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

        private async Task SendStateUpdate(Guid? broadcastExcludeId = null)
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

        private async Task HandleParticipantDisconnecting(Guid participantId)
        {
            if (spectators.ContainsKey(participantId))
            {
                spectators.Remove(participantId);

                await bridge.SendMessage(new PlayerDisconnectedMessage()
                {
                    Role = Player.None,
                }, cancellationToken: appCts.Token);
                return;
            }

            StopTimer();
            gameState.CurrentPlayer = Player.None;

            if (players[Player.White]?.Id == participantId)
            {
                logger.Log($"White disconnected");
                players[Player.White] = null;

                await bridge.SendMessage(new PlayerDisconnectedMessage()
                {
                    Role = Player.White,
                }, cancellationToken: appCts.Token);
            }
            else if (players[Player.Black]?.Id == participantId)
            {
                logger.Log($"Black disconnected");
                players[Player.Black] = null;

                await bridge.SendMessage(new PlayerDisconnectedMessage()
                {
                    Role = Player.Black,
                }, cancellationToken: appCts.Token);
            }
        }

        private async Task SendErrorMessageFor(string senderId, string message)
        {
            await bridge.SendMessage(new ErrorMessage()
            {
                Message = message,
            }, RecipientId: Guid.Parse(senderId), cancellationToken: appCts.Token);
        }

        private bool IsBothPlayersJoined()
            => players[Player.White] != null && players[Player.Black] != null;
    }
}
