using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.MoveRequest" />
public record MoveRequestMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.MoveRequest;

    /// <summary>
    /// Ход в виде нотации
    /// </summary>
    public string MoveUciNotation { get; init; } = string.Empty;
}
