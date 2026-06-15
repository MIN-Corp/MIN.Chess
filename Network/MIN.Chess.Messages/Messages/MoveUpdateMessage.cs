using MIN.Chess.Logic.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.MoveUpdate" />
public record MoveUpdateMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.MoveUpdate;

    /// <summary>
    /// Ход в виде нотации
    /// </summary>
    public string MoveUci { get; init; } = string.Empty;

    /// <summary>
    /// Цвет
    /// </summary>
    public Player CurrentPlayer { get; init; }

    /// <summary>
    /// Время белого игрока
    /// </summary>
    /// <remarks>
    /// null если игра без таймера
    /// </remarks>
    public long? WhiteMs { get; init; }

    /// <summary>
    /// Время чёрного игрока
    /// </summary>
    /// <remarks>
    /// null если игра без таймера
    /// </remarks>
    public long? BlackMs { get; init; }
}
