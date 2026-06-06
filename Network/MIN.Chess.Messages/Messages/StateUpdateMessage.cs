using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.StateUpdate" />
public record StateUpdateMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.StateUpdate;

    /// <summary>
    /// Доска с фигурами
    /// </summary>
    public Board Board { get; } = null!;

    /// <summary>
    /// Текущий ход
    /// </summary>
    public Player CurrentPlayer { get; }
}
