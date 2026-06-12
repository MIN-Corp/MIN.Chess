using System.Text.Json;
using MIN.Chess.Logic.Enums;
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
            case StateUpdateRequestMessage _:
                Console.WriteLine("StateUpdateRequestMessage received");
                Console.WriteLine($"sending gamestate {gameState.ToString()}");
                await bridge.SendMessage(new StateUpdateMessage()
                {
                    StateString = gameState.ToString(),
                    CurrentPlayer = gameState.CurrentPlayer,
                }, RecipientId: Guid.Parse(message.SenderId), cancellationToken: appCts.Token);
                break;

            case MoveRequestMessage moveRequestMessage:
                Console.WriteLine($"moveRequestMessage received {moveRequestMessage.MoveUciNotation}");

                var move = MoveParser.Parse(moveRequestMessage.MoveUciNotation, gameState.Board);

                Console.WriteLine("checking legalability . . .");

                if (move.IsLegal(gameState.Board))
                {
                    gameState.MakeMove(move);
                }
                else
                {
                    await bridge.SendMessage(new ErrorMessage()
                    {
                        Message = $"Нелегальный ход",
                    }, RecipientId: Guid.Parse(message.SenderId), cancellationToken: appCts.Token);
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
                break;
        }
    }

    private static async Task HandleParticipantJoining(Participant participant)
    {
        if (IsBothPlayersJoined())
        {
            Console.WriteLine($"{participant.Name} joined as Spectator");
            spectators[participant.Id] = participant;
            return;
        }

        if (players[Player.White] == null)
        {
            Console.WriteLine($"{participant.Name} joined as White");
            players[Player.White] = participant;

            gameState.Board = Board.Initial(Player.White);
        }
        else if (players[Player.Black] == null)
        {
            Console.WriteLine($"{participant.Name} joined as Black");
            players[Player.Black] = participant;

            gameState.Board = Board.Initial();
            gameState.CurrentPlayer = Player.White;

            await SendStateUpdate();
        }
    }

    private static async Task HandleParticipantDisconnecting(Guid participantId)
    {
        if (spectators.ContainsKey(participantId))
        {
            spectators.Remove(participantId);
            return;
        }

        gameState.CurrentPlayer = Player.None;

        if (players[Player.White]?.Id == participantId)
        {
            Console.WriteLine($"White disconnected");
            players[Player.White] = null;
        }
        else if (players[Player.Black]?.Id == participantId)
        {
            Console.WriteLine($"Black disconnected");
            players[Player.Black] = null;
        }

        await SendStateUpdate();
    }

    private static async Task SendStateUpdate()
    {
        await bridge.SendMessage(new StateUpdateMessage()
        {
            StateString = gameState.ToString(),
            CurrentPlayer = gameState.CurrentPlayer,
        }, cancellationToken: appCts.Token);
    }

    private static bool IsBothPlayersJoined()
        => players[Player.White] != null && players[Player.Black] != null;
}
