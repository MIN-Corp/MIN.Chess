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
    private readonly static Dictionary<Guid, Participant> spectators = [];
    private readonly static CancellationTokenSource appCts = new();
    private readonly static Dictionary<Player, Participant?> players = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    private const int ConnectionTimeoutMs = 5_000;

    private static Move? lastMove = null;

    private static int restartVote = 0;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
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
            case InitRequestMessage _:
                await HandleInit(message);
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

            case RestartRequestMessage _:
                await HandleRestart(message);
                break;
        }
    }

    private static async Task HandleInit(INetworkMessage message)
    {
        Console.WriteLine("InitRequestMessage received");
        Console.WriteLine($"sending init {gameState.ToString()}");

        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        Console.WriteLine($"ROLE {role.ToString()}");

        Participant? sender;

        players.TryGetValue(role, out sender);

        if (sender == null)
        {
            spectators.TryGetValue(senderId, out sender);
        }

        await bridge.SendMessage(new InitResponseMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            ActivePlayers = players,
            SpectorsCount = spectators.Count,
            SelfParticipant = sender!,
            RequesterRole = role,
            LastMove = lastMove?.ToUci(),
        }, RecipientId: senderId, cancellationToken: appCts.Token);
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

        var senderRole = ResolveRole(Guid.Parse(moveRequestMessage.SenderId));

        if (move.IsLegal(gameState.Board) && senderRole == gameState.CurrentPlayer)
        {
            gameState.MakeMove(move);
            lastMove = move;
        }
        else
        {
            await bridge.SendMessage(new ErrorMessage()
            {
                Message = $"Нелегальный ход",
            }, RecipientId: Guid.Parse(moveRequestMessage.SenderId), cancellationToken: appCts.Token);
            return;
        }

        Console.WriteLine("move is legal, sending move update");

        await bridge.SendMessage(new MoveUpdateMessage()
        {
            MoveUci = moveRequestMessage.MoveUciNotation,
            CurrentPlayer = gameState.CurrentPlayer,
        }, cancellationToken: appCts.Token);

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

        await bridge.SendMessage(new GameResultMessage()
        {
            Result = new Result(role.Opponent(), EndReason.Resign),
        }, cancellationToken: appCts.Token);
    }

    private static async Task HandleRestart(INetworkMessage message)
    {
        if (!IsBothPlayersJoined())
        {
            await Restart();
        }
        else
        {
            restartVote++;
            if (restartVote >= 2)
            {
                await Restart();
                restartVote = 0;
            }
        }
    }

    private static async Task Restart()
    {
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
            Console.WriteLine($"{participant.Name} joined as Spectator");
            spectators[participant.Id] = participant;

            await bridge.SendMessage(new PlayerConnectedMessage()
            {
                Role = Player.None,
                Participant = participant,
            }, cancellationToken: appCts.Token);

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

        if (IsBothPlayersJoined())
        {
            await SendStateUpdate();
        }
    }

    private static async Task SendStateUpdate()
    {
        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentTurn = gameState.CurrentPlayer,
            LastMove = null,
        }, cancellationToken: appCts.Token);
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

    private static bool IsBothPlayersJoined()
        => players[Player.White] != null && players[Player.Black] != null;
}
