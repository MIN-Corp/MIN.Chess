using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models.Moves;

/// <inheritdoc cref="MoveType.Normal"/>
public class DoublePawn : Move
{
    private readonly Position skippedPos;

    /// <inheritdoc />
    public override MoveType Type => MoveType.DoublePawn;

    /// <inheritdoc />
    public override Position FromPos { get; }

    /// <inheritdoc />
    public override Position ToPos { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DoublePawn"/>
    /// </summary>
    public DoublePawn(Position from, Position to)
    {
        FromPos = from;
        ToPos = to;
        skippedPos = new Position((from.Row + to.Row) / 2, from.Column);
    }

    /// <inheritdoc />
    public override bool Execute(Board board)
    {
        var player = board[FromPos]?.Color;
        board.SetPawnSkipPosition(player!.Value, skippedPos);
        new NormalMove(FromPos, ToPos).Execute(board);

        return true;
    }
}
