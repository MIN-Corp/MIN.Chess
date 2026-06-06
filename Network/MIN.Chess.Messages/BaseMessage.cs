namespace MIN.Chess.Messages;

/// <summary>
/// Тип сообщения
/// </summary>
public abstract record BaseMessage
{
    /// <summary>
    /// Тип сообщение
    /// </summary>
    public abstract MessageType MessageType { get; }
}
