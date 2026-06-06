using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.MoveRequest" />
public record MoveRequestMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.MoveRequest;

    /// <summary>
    /// Тип хода
    /// </summary>
    public MoveType Type { get; }

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
