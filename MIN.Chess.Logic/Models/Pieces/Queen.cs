using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Logic.Models.Pieces;

/// <summary>
/// <inheritdoc cref="PieceType.Queen"/>
/// </summary>
public class Queen : Piece
{
    private readonly Direction[] dirs =
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West,
        Direction.NorthWest,
        Direction.NorthEast,
        Direction.SouthWest,
        Direction.SouthEast,
    ];

    /// <inheritdoc />
    public override PieceType Type => PieceType.Queen;

    /// <inheritdoc />
    public override Player Color { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Queen"/>
    /// </summary>
    public Queen(Player color)
    {
        Color = color;
    }

    /// <inheritdoc />
    public override Piece Copy()
    {
        var copy = new Queen(Color)
        {
            HasMoved = HasMoved
        };
        return copy;
    }

    /// <inheritdoc />
    public override IEnumerable<Move> GetMoves(Position from, Board board)
        => MovePositionsInDirs(from, board, dirs).Select(to => new NormalMove(from, to));

    /// <inheritdoc />
    public override IEnumerable<Position> GetPremovePositions(Position from, Board board)
        => PremovePositionsInDirs(from, board, dirs);

    /// <inheritdoc />
    public override bool CanCaptureOpponentKing(Position from, Board board)
    {
        foreach (var dir in dirs)
        {
            for (var pos = from + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    continue;
                }
                if (board[pos]?.Type == PieceType.King && board[pos]?.Color != Color)
                {
                    return true;
                }
                break;
            }
        }
        return false;
    }
}
