using MIN.Chess.Messages.Contracts.Enums;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Messages.Contracts.Models;

/// <summary>
/// Тип сообщения
/// </summary>
public abstract record BaseNetworkMessage : INetworkMessage
{
    /// <summary>
    /// Тип сообщение
    /// </summary>
    public abstract NetworkMessageType MessageType { get; }

    /// <inheritdoc />
    public string Type => MessageType.ToString();
}
