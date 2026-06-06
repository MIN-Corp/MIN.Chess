namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.ResignRequest" />
public record ResignRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.ResignRequest;
}
