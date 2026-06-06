namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.Error" />
public record ErrorMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.Error;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; } = "Произошла ошибка";
}
