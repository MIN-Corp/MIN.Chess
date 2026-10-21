using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models.Moves;

/// <inheritdoc cref="MoveType.PawnPromotion"/>
public class Castle : Move
{
    private readonly Direction kingMoveDir;
    private readonly Position rookFromPos;
    private readonly Position rookToPos;

    /// <inheritdoc />
    public override MoveType Type { get; }

    /// <inheritdoc />
    public override Position FromPos { get; }

    /// <inheritdoc />
    public override Position ToPos { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Castle"/>
    /// </summary>
    public Castle(MoveType type, Position kingPos)
    {
        Type = type;
        FromPos = kingPos;

        if (type == MoveType.CastleKs)
        {
            kingMoveDir = Direction.East;
            ToPos = new Position(kingPos.Row, 6);
            rookFromPos = new Position(kingPos.Row, 7);
            rookToPos = new Position(kingPos.Row, 5);
        }
        else if (type == MoveType.CastleQs)
        {
            kingMoveDir = Direction.West;
            ToPos = new Position(kingPos.Row, 2);
            rookFromPos = new Position(kingPos.Row, 0);
            rookToPos = new Position(kingPos.Row, 3);
        }
    }

    /// <inheritdoc />
    public override bool Execute(Board board)
    {
        new NormalMove(FromPos, ToPos).Execute(board);
        new NormalMove(rookFromPos, rookToPos).Execute(board);

        return false;
    }

    /// <inheritdoc />
    public override bool IsLegal(Board board)
    {
        var player = board[FromPos]?.Color;

        if (board.IsInCheck(player!.Value))
        {
            return false;
        }

        var copy = board.Copy();
        var kingPosInCopy = FromPos;

        for (var i = 0; i < 2; i++)
        {
            new NormalMove(kingPosInCopy, kingPosInCopy + kingMoveDir).Execute(copy);
            kingPosInCopy += kingMoveDir;

            if (copy.IsInCheck(player!.Value))
            {
                return false;
            }
        }

        return true;
    }
}

