using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server;

static internal class Program
{
    private readonly static IClientProcessBridge bridge = new ClientProcessBridge();
    private readonly static Dictionary<Guid, Participant> spectators = [];
    private readonly static CancellationTokenSource appCts = new();
    private static Participant? whitePlayer;
    private static Participant? blackPlayer;

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

    private static void ParticipantConnected(object? sender, Participant participant)
    {
        HandleParticipantJoining(participant);
    }

    private static void ParticipantDisconnected(object? sender, Guid participantId)
    {
        HandleParticipantDisconnecting(participantId);
    }

    private static void HandleParticipantJoining(Participant participant)
    {
        if (whitePlayer == null)
        {
            Console.WriteLine($"{participant.Name} joined as White");
            whitePlayer = participant;
        }
        else if (blackPlayer == null)
        {
            Console.WriteLine($"{participant.Name} joined as Black");
            blackPlayer = participant;
        }
        else
        {
            Console.WriteLine($"{participant.Name} joined as Spectator");
            spectators[participant.Id] = participant;
        }
    }

    private static void HandleParticipantDisconnecting(Guid participantId)
    {
        if (whitePlayer?.Id == participantId)
        {
            Console.WriteLine($"White disconnected");
            whitePlayer = null;
        }
        else if (blackPlayer?.Id == participantId)
        {
            Console.WriteLine($"Black disconnected");
            blackPlayer = null;
        }
        else
        {
            spectators.Remove(participantId);
        }
    }

    private static async Task HandleMessage(INetworkMessage message)
    {
        switch (message)
        {
            case MoveRequestMessage moveRequestMessage:
                Console.WriteLine("moveRequestMessage received");
                break;
        }
    }

    private static bool IsBothPlayersJoined()
        => whitePlayer != null && blackPlayer != null;
}
