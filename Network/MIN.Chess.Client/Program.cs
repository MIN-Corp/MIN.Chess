using System.Diagnostics;
using MIN.Chess.Client.Forms;
using MIN.Ipc.Services;

namespace MIN.Chess.Client;

static internal class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.WriteLine("Client requires MIN connection args");
            return Task.CompletedTask;
        }

        var appCts = new CancellationTokenSource();
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(new ClientProcessBridge(), appCts, args[0]));
        return Task.CompletedTask;
    }
}
