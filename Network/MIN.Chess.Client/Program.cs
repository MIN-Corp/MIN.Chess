using System.IO.Pipes;
using System.Text.Json;
using MIN.Chess.Client.Contracts.Models;
using MIN.Chess.Client.Forms;

namespace MIN.Chess.Client;

static internal class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Chess Client requires args");
            return;
        }

        Console.Write($"GOT ARGS: {args[0]}");

        var connInfo = JsonSerializer.Deserialize<ConnectionInfo>(args[0]);

        if (connInfo?.Type == "pipe")
        {
            using var pipe = new NamedPipeClientStream(".", connInfo.Value, PipeDirection.InOut);
            await pipe.ConnectAsync(5000);

            //// Дальше — length-prefixed JSON
            //await SendAsync(pipe, new { type = "move", from = "e2", to = "e4" });
            //var update = await ReadAsync(pipe); // StateUpdate
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
