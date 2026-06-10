using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.MoveRequest" />
public record MoveRequestMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.MoveRequest;

    /// <summary>
    /// Тип хода
    /// </summary>
    public MoveType MoveType { get; }

    /// <summary>
    /// С какой позиции
    /// </summary>
    public Position From { get; }

    /// <summary>
    /// На какую позицию
    /// </summary>
    public Position To { get; }

    /// <summary>
    /// Фигура продвижения
    /// </summary>
    /// <remarks>
    /// null, если это не продвижение пешки
    /// </remarks>
    public PieceType? PromotionPiece { get; }
}
