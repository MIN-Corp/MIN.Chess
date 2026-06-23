namespace MIN.Ipc.Transport.Contracts.Enums;

/// <summary>
/// Аргументы событие получения данных от MIN
/// </summary>
public sealed class ProcessClientTransportMessageEventArgs : EventArgs
{
    /// <summary>
    /// Полученные данные
    /// </summary>
    public byte[] Data { get; init; } = null!;
}
