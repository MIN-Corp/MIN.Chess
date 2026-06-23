namespace MIN.Ipc.Services.Contracts.Interfaces;

/// <summary>
/// Сообщение, полученное по сети
/// </summary>
public interface INetworkMessage
{
    /// <summary>
    /// Тип сообщения
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Идентификатор отправителя сообщения
    /// </summary>
    string SenderId { get; set; }
}
