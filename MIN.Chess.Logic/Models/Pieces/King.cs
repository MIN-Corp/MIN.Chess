using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Logic.Models.Pieces;

/// <summary>
/// <inheritdoc cref="PieceType.King"/>
/// </summary>
public class King : Piece
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
    public override PieceType Type => PieceType.King;

    /// <inheritdoc />
    public override Player Color { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="King"/>
    /// </summary>
    public King(Player color)
    {
        Color = color;
    }

    private static bool IsUnmovedRook(Position pos, Board board)
    {
        if (board.IsEmpty(pos))
        {
            return false;
        }

        var piece = board[pos];
        return piece?.Type == PieceType.Rook && !piece.HasMoved;
    }

    private static bool AllEmpty(IEnumerable<Position> positions, Board board)
        => positions.All(x => board.IsEmpty(x));

    private bool CanCastleKingSide(Position from, Board board)
    {
        if (HasMoved)
        {
            return false;
        }

        var rookPos = new Position(from.Row, 7);
        var betweenPos = new Position[] { new(from.Row, 5), new(from.Row, 6) };

        return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPos, board);
    }

    private bool CanCastleQueenSide(Position from, Board board)
    {
        if (HasMoved)
        {
            return false;
        }

        var rookPos = new Position(from.Row, 0);
        var betweenPos = new Position[] { new(from.Row, 1), new(from.Row, 2), new(from.Row, 3) };

        return IsUnmovedRook(rookPos, board) && AllEmpty(betweenPos, board);
    }

    /// <inheritdoc />
    public override Piece Copy()
    {
        var copy = new King(Color)
        {
            HasMoved = HasMoved
        };
        return copy;
    }

    private IEnumerable<Position> MovePositions(Position from, Board board)
    {
        foreach (var dir in dirs)
        {
            var to = from + dir;

            if (!Board.IsInside(to))
            {
                continue;
            }

            if (board.IsEmpty(to) || board[to]?.Color != Color)
            {
                yield return to;
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Position> GetPremovePositions(Position from, Board board)
    {
        foreach (var dir in dirs)
        {
            var to = from + dir;
            if (Board.IsInside(to))
            {
                yield return to;
            }
        }
        if (!HasMoved)
        {
            yield return new Position(from.Row, from.Column + 2); // KS
            yield return new Position(from.Row, from.Column - 2); // QS
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        foreach (var to in MovePositions(from, board))
        {
            yield return new NormalMove(from, to);
        }

        if (CanCastleKingSide(from, board))
        {
            yield return new Castle(MoveType.CastleKs, from);
        }

        if (CanCastleQueenSide(from, board))
        {
            yield return new Castle(MoveType.CastleQs, from);
        }
    }

    /// <inheritdoc />
    public override bool CanCaptureOpponentKing(Position from, Board board)
        => MovePositions(from, board).Any(to =>
        {
            var piece = board[to];
            return piece != null && piece.Type == PieceType.King;
        });
}
