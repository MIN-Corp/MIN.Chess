using MIN.Ipc.Transport.Contracts.Enums;

namespace MIN.Ipc.Transport.Contracts.Interfaces;

/// <summary>
/// Транспорт для междупроцессорного общения
/// </summary>
public interface IProcessClientTransport : IAsyncDisposable
{
    /// <summary>
    /// Подключиться к MIN по connection string
    /// </summary>
    Task ConnectAsync(string connectionString, CancellationToken cancellationToken = default);

    /// <summary>
    /// Событие: получены сырые данные от подключения
    /// </summary>
    event EventHandler<ProcessClientTransportMessageEventArgs>? MessageReceived;

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    Task SendAsync(byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться
    /// </summary>
    Task DisconnectAsync();
}
