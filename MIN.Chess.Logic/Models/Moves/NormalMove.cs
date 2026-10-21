using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models.Moves;

/// <inheritdoc cref="MoveType.Normal"/>
public class NormalMove : Move
{
    /// <inheritdoc />
    public override MoveType Type => MoveType.Normal;

    /// <inheritdoc />
    public override Position FromPos { get; }

    /// <inheritdoc />
    public override Position ToPos { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NormalMove"/>
    /// </summary>
    public NormalMove(Position from, Position to)
    {
        FromPos = from;
        ToPos = to;
    }

    /// <inheritdoc />
    public override bool Execute(Board board)
    {
        var piece = board[FromPos];
        var capture = !board.IsEmpty(ToPos);
        board[ToPos] = piece;
        board[FromPos] = null!;
        if (piece != null)
        {
            piece.HasMoved = true;
        }

        return capture || piece?.Type == PieceType.Pawn;
    }
}
