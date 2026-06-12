using MIN.Chess.Logic.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.StateUpdate" />
public record StateUpdateMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.StateUpdate;

    /// <summary>
    /// Состояние игры в виде строки
    /// </summary>
    public string StateString { get; init; } = string.Empty;

    /// <summary>
    /// Текущий ход игрока
    /// </summary>
    public Player CurrentTurn { get; init; }

    /// <summary>
    /// Последний ход (если был)
    /// </summary>
    public string? LastMove { get; init; }
}
