using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.StateUpdate" />
public record StateUpdateMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.StateUpdate;

    /// <summary>
    /// Доска с фигурами
    /// </summary>
    public Board Board { get; } = null!;

    /// <summary>
    /// Текущий ход
    /// </summary>
    public Player CurrentPlayer { get; }
}
