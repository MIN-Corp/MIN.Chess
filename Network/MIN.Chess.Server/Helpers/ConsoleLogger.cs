namespace MIN.Chess.Server.Helpers
{
    /// <summary>
    /// <inheritdoc cref="ILoggerProvider"/>
    /// </summary>
    public class ConsoleLogger : ILoggerProvider
    {
        void ILoggerProvider.Log(string message) => Console.WriteLine(message);
    }
}
