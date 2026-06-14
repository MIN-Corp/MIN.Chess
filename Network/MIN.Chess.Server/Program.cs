using System.Text.Json;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server;

static internal class Program
{
    private readonly static IClientProcessBridge bridge = new ClientProcessBridge();
    private readonly static GameState gameState = new(new Board(), Player.None);
    private readonly static Dictionary<Guid, InitRequestMessage> pendingInits = [];
    private readonly static Dictionary<Guid, Participant> spectators = [];
    private readonly static CancellationTokenSource appCts = new();
    private readonly static Dictionary<Player, Participant?> players = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    private readonly static Dictionary<Player, bool> restartVote = new()
    {
        { Player.White, false },
        { Player.Black, false },
    };

    private readonly static Dictionary<Player, bool> drawVote = new()
    {
        { Player.White, false },
        { Player.Black, false },
    };


    private const int ConnectionTimeoutMs = 5_000;

    private static Move? lastMove;

    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Server requires MIN connection args");
            return;
        }

        bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();

        await InitializeNetworking(args[0]);
        await Task.Delay(-1, appCts.Token);
    }

    private static async Task InitializeNetworking(string connectionString)
    {
        try
        {
            Console.WriteLine("Connecting to MIN . . .");
            await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, appCts.Token);

            bridge.NetworkMessageReceived += NetworkMessageReceived;
            bridge.ParticipantConnected += ParticipantConnected;
            bridge.ParticipantDisconnected += ParticipantDisconnected;

            await bridge.StartListeningAsync(appCts.Token);

            Console.WriteLine("Connection succeded, sending ready message");
            await bridge.SendReadyMessage(appCts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
    }

    private static async void NetworkMessageReceived(object? sender, INetworkMessage message)
    {
        await HandleMessage(message);
    }

    private static async void ParticipantConnected(object? sender, Participant participant)
    {
        await HandleParticipantJoining(participant);
    }

    private static async void ParticipantDisconnected(object? sender, Guid participantId)
    {
        await HandleParticipantDisconnecting(participantId);
    }

    private static async Task HandleMessage(INetworkMessage message)
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
        }
    }

    private static async Task HandleInit(InitRequestMessage message)
    {
        Console.WriteLine("InitRequestMessage received");
        Console.WriteLine($"sending init {gameState.ToString()}");

        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"ROLE {role}");

        Participant? sender;

        players.TryGetValue(role, out sender);

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

    private static async Task StateUpdate(INetworkMessage message)
    {
        Console.WriteLine("StateUpdateRequestMessage received");
        Console.WriteLine($"sending gamestate {gameState.ToString()}");
        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            LastMove = lastMove?.ToUci(),
        }, RecipientId: Guid.Parse(message.SenderId), cancellationToken: appCts.Token);
    }

    private static async Task HandleMove(MoveRequestMessage moveRequestMessage)
    {
        Console.WriteLine($"moveRequestMessage received {moveRequestMessage.MoveUciNotation}");

        var move = MoveParser.Parse(moveRequestMessage.MoveUciNotation, gameState.Board);

        Console.WriteLine("checking legalability . . .");

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

        Console.WriteLine("move is legal, sending move update");

        await bridge.SendMessage(new MoveUpdateMessage()
        {
            MoveUci = moveRequestMessage.MoveUciNotation,
            CurrentPlayer = gameState.CurrentPlayer,
        },
        BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(senderId),
        cancellationToken: appCts.Token);

        gameState.CheckForGameOver();

        if (gameState.IsGameOver())
        {
            Console.WriteLine($"game over, sending result: {JsonSerializer.Serialize(gameState.Result!.Value)}");
            await bridge.SendMessage(new GameResultMessage()
            {
                Result = gameState.Result!.Value,
            }, cancellationToken: appCts.Token);
            return;
        }
    }

    private static async Task HandleResign(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"Resign received from {role}");

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

    private static async Task HandleDraw(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"Draw request received from {role}");

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
                await bridge.SendMessage(new GameResultMessage()
                {
                    Result = Result.Draw(EndReason.Draw)
                }, cancellationToken: appCts.Token);
            }

            await bridge.SendMessage(new DrawVoteMessage()
            {
                CurrentVotes = votes
            }, cancellationToken: appCts.Token);
        }
    }

    private static async Task HandleRestart(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"Restart received from {role}");

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

    private static async Task HandleChangeRole(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"ChangeRole received from {role}");

        if (IsBothPlayersJoined())
        {
            await SendErrorMessageFor(message.SenderId, "Не кого заменять");
            return;
        }

        spectators.TryGetValue(senderId, out var participant);

        if (participant == null)
        {
            Console.WriteLine($"Didnt found spectator {senderId}");
            return;
        }

        if (players[Player.White] == null)
        {
            Console.WriteLine($"{participant.Name} joined as White");
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
            Console.WriteLine($"{participant.Name} joined as Black");
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

    private static IEnumerable<Guid>? GetDefaultExcludeBroadcastListWith(Guid? excludeId)
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

    private static async Task Restart()
    {
        gameState.Result = null;
        lastMove = null;

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

            await SendStateUpdate();
        }
    }

    private static Player ResolveRole(Guid senderId)
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

    private static async Task HandleParticipantJoining(Participant participant)
    {
        if (IsBothPlayersJoined())
        {
            Console.WriteLine($"{participant.Id} joined as Spectator");
            spectators[participant.Id] = participant;

            await bridge.SendMessage(new PlayerConnectedMessage()
            {
                Role = Player.None,
                Participant = participant,
            }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(participant.Id),
            cancellationToken: appCts.Token);

            if (pendingInits.Remove(participant.Id, out _))
            {
                Console.WriteLine($"SENDING INIT RESPONSE TO {participant.Id}");
                await SendInitResponse(participant);
            }
            else
            {
                Console.WriteLine($"DIDNT FOUND {participant.Id} IN PENDING, MUST HAVE ALREADY RECEIVED");
            }

            return;
        }

        lastMove = null;

        if (players[Player.White] == null)
        {
            Console.WriteLine($"{participant.Name} joined as White");
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
            Console.WriteLine($"{participant.Name} joined as Black");
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
            Console.WriteLine($"SENDING INIT RESPONSE TO {participant.Id}");
            await SendInitResponse(participant);
        }
        else
        {
            Console.WriteLine($"DIDNT FOUND {participant.Id} IN PENDING, MUST HAVE ALREADY RECEIVED");
        }

        if (IsBothPlayersJoined())
        {
            await SendStateUpdate(participant.Id);
        }
    }

    private static async Task SendInitResponse(Participant participant)
    {
        var role = ResolveRole(participant.Id);
        var initResponse = new InitResponseMessage
        {
            StateString = new StateString(gameState.CurrentPlayer, gameState.Board).ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            RequesterRole = role,
            SelfParticipant = participant,
            LastMove = lastMove?.ToUci(),
            ActivePlayers = players,
            SpectorsCount = spectators.Count,
        };

        await bridge.SendMessage(initResponse, RecipientId: participant.Id, cancellationToken: appCts.Token);
    }

    private static async Task SendStateUpdate(Guid? broadcastExcludeId = null)
    {
        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            LastMove = null,
        }, BroadcastExcludeIds: GetDefaultExcludeBroadcastListWith(broadcastExcludeId), cancellationToken: appCts.Token);
    }

    private static async Task HandleParticipantDisconnecting(Guid participantId)
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

        gameState.CurrentPlayer = Player.None;

        if (players[Player.White]?.Id == participantId)
        {
            Console.WriteLine($"White disconnected");
            players[Player.White] = null;

            await bridge.SendMessage(new PlayerDisconnectedMessage()
            {
                Role = Player.White,
            }, cancellationToken: appCts.Token);
        }
        else if (players[Player.Black]?.Id == participantId)
        {
            Console.WriteLine($"Black disconnected");
            players[Player.Black] = null;

            await bridge.SendMessage(new PlayerDisconnectedMessage()
            {
                Role = Player.Black,
            }, cancellationToken: appCts.Token);
        }
    }

    private static async Task SendErrorMessageFor(string senderId, string message)
    {
        await bridge.SendMessage(new ErrorMessage()
        {
            Message = message,
        }, RecipientId: Guid.Parse(senderId), cancellationToken: appCts.Token);
    }

    private static bool IsBothPlayersJoined()
        => players[Player.White] != null && players[Player.Black] != null;
}
