using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Logic.Models.Pieces;

/// <summary>
/// <inheritdoc cref="PieceType.Knight"/>
/// </summary>
public class Knight : Piece
{
    /// <inheritdoc />
    public override PieceType Type => PieceType.Knight;

    /// <inheritdoc />
    public override Player Color { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Knight"/>
    /// </summary>
    public Knight(Player color)
    {
        Color = color;
    }

    /// <inheritdoc />
    public override Piece Copy()
    {
        var copy = new Knight(Color)
        {
            HasMoved = HasMoved
        };
        return copy;
    }

    private static IEnumerable<Position> PotentionalToPositions(Position from)
    {
        foreach (var vDirections in new Direction[] { Direction.North, Direction.South })
        {
            foreach (var hDirections in new Direction[] { Direction.West, Direction.East })
            {
                yield return from + 2 * vDirections + hDirections;
                yield return from + 2 * hDirections + vDirections;
            }
        }
    }

    private IEnumerable<Position> MovePositions(Position from, Board board)
        => PotentionalToPositions(from).Where(to => Board.IsInside(to) && (board.IsEmpty(to) || board[to]?.Color != Color));

    /// <inheritdoc />
    public override IEnumerable<Position> GetPremovePositions(Position from, Board board)
        => PotentionalToPositions(from).Where(Board.IsInside);

    /// <inheritdoc />
    public override IEnumerable<Move> GetMoves(Position from, Board board)
        => MovePositions(from, board).Select(to => new NormalMove(from, to));
}
