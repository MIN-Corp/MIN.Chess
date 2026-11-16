using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.StartGameRequest" />
public record StartGameRequestMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.StartGameRequest;
}
