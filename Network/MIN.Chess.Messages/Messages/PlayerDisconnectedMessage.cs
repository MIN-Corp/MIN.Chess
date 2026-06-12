using MIN.Chess.Logic.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.PlayerDisconnected" />
public record PlayerDisconnectedMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.PlayerDisconnected;

    /// <summary>
    /// Роль игрока
    /// </summary>
    public Player Role { get; init; }
}
