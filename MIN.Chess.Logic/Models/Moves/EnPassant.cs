using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models.Moves;

/// <inheritdoc cref="MoveType.EnPassant"/>
public class EnPassant : Move
{
    private readonly Position capturePos;

    /// <inheritdoc />
    public override MoveType Type => MoveType.EnPassant;

    /// <inheritdoc />
    public override Position FromPos { get; }

    /// <inheritdoc />
    public override Position ToPos { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="EnPassant"/>
    /// </summary>
    public EnPassant(Position from, Position to)
    {
        FromPos = from;
        ToPos = to;
        capturePos = new Position(from.Row, to.Column);
    }

    /// <inheritdoc />
    public override bool Execute(Board board)
    {
        new NormalMove(FromPos, ToPos).Execute(board);
        board[capturePos] = null!;

        return true;
    }
}
