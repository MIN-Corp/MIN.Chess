using System.Diagnostics;
using MIN.Chess.Server.Forms;

namespace MIN.Chess.Server
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Debug.WriteLine("Client requires MIN connection args");
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new OutputForm(args[0]));
        }
    }
}
