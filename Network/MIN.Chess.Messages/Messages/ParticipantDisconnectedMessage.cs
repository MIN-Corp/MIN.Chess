using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.ParticipantDisconnected" />
public record ParticipantDisconnectedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.ParticipantDisconnected;

    /// <summary>
    /// Участник
    /// </summary>
    public Participant Participant { get; } = null!;
}
