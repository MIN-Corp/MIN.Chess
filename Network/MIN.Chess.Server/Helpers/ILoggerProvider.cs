namespace MIN.Chess.Server.Helpers;

/// <summary>
/// Сервис для логирования данных
/// </summary>
internal interface ILoggerProvider
{
    /// <summary>
    /// Залогировать данные
    /// </summary>
    void Log(string message);
}
