using MIN.Chess.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Enums;
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

    /// <summary>
    /// Канал, по которому пойдёт сообщение
    /// </summary>
    /// <remarks>
    /// По умолчанию Secure
    /// </remarks>
    public virtual MessageChannel Channel { get; } = MessageChannel.Secure;

    /// <summary>
    /// Направление сообщения сессии
    /// </summary>
    /// <remarks>
    /// По умолчанию через сервер
    /// </remarks>
    public virtual SessionMessageRoute Route { get; } = SessionMessageRoute.ViaServer;

    /// <inheritdoc />
    public string Type => MessageType.ToString();

    /// <inheritdoc />
    public string SenderId { get; set; } = string.Empty;
}
