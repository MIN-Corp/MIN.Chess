namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.RestartRequest" />
public record RestartRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.RestartRequest;
}
