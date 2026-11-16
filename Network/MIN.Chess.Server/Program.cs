using MIN.Chess.Server.Helpers;
using MIN.Chess.Server.Services;

namespace MIN.Chess.Server;

static internal class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Server requires MIN connection args");
            return;
        }

        var logger = new ConsoleLogger();
        var gameServerService = new GameServerService(logger);
        var shutdown = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        gameServerService.CloseRequested += () => shutdown.TrySetResult();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; shutdown.TrySetResult(); };

        await gameServerService.InitializeNetworking(args[0]);
        await shutdown.Task;
        gameServerService.StopService();
    }
}
