using MIN.Chess.Server.Forms;

namespace MIN.Chess.Server;

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
            Console.WriteLine("Client requires MIN connection args");
            return Task.CompletedTask;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new OutputForm(args[0]));
        return Task.CompletedTask;
    }
}
