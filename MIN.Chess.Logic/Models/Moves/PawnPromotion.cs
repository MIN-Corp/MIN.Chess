using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Pieces;

namespace MIN.Chess.Logic.Models.Moves;

/// <inheritdoc cref="MoveType.PawnPromotion"/>
public class PawnPromotion : Move
{
    /// <summary>
    /// Новый тип фигуры
    /// </summary>
    public PieceType NewType;

    /// <inheritdoc />
    public override MoveType Type => MoveType.PawnPromotion;

    /// <inheritdoc />
    public override Position FromPos { get; }

    /// <inheritdoc />
    public override Position ToPos { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NormalMove"/>
    /// </summary>
    public PawnPromotion(Position from, Position to, PieceType newType)
    {
        FromPos = from;
        ToPos = to;
        NewType = newType;
    }

    private Piece CreatePromotionPiece(Player color)
        => NewType switch
        {
            PieceType.Knight => new Knight(color),
            PieceType.Bishop => new Bishop(color),
            PieceType.Rook => new Rook(color),
            _ => new Queen(color)
        };

    /// <inheritdoc />
    public override string ToUci() => FromPos.ToUci() + ToPos.ToUci() + NewType.ToUciChar();

    /// <inheritdoc />
    public override bool Execute(Board board)
    {
        var piece = board[FromPos];
        board[FromPos] = null!;

        var promotionPiece = CreatePromotionPiece(piece!.Color);
        board[ToPos] = promotionPiece;
        piece.HasMoved = true;

        return true;
    }
}
