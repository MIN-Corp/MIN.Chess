using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.GameStart" />
public record GameStartMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.GameStart;

    /// <summary>
    /// Цвет
    /// </summary>
    public Player Color { get; }
}
