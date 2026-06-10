using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Server;

static internal class Program
{
    private readonly static IClientProcessBridge bridge = new ClientProcessBridge();
    private const int ConnectionTimeoutMs = 5_000;

    /// <summary>
    /// Главный метод программы
    /// </summary>
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Chess Server requires args");
            return;
        }

        var appCts = new CancellationTokenSource();

        bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
        await bridge.ConnectAsync(args[0], ConnectionTimeoutMs, appCts.Token);

        bridge.NetworkMessageReceived += Bridge_NetworkMessageReceived;

        await bridge.SendReadyMessage(appCts.Token);
    }

    private static async void Bridge_NetworkMessageReceived(object? sender, INetworkMessage e)
    {
        await HandleMessage(e);
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
}
