using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.PlayerConnected" />
public record PlayerConnectedMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.PlayerConnected;

    /// <summary>
    /// Данные об участнике
    /// </summary>
    public ChessPlayer Player { get; init; } = null!;
}
