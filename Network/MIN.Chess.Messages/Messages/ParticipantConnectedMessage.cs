using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.ParticipantConnected" />
public record ParticipantConnectedMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.ParticipantConnected;

    /// <summary>
    /// Участник
    /// </summary>
    public Participant Participant { get; } = null!;
}
