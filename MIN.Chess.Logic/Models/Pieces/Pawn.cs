using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Logic.Models.Pieces;

/// <summary>
/// <inheritdoc cref="PieceType.Pawn"/>
/// </summary>
public class Pawn : Piece
{
    private readonly Direction forward;

    /// <inheritdoc />
    public override PieceType Type => PieceType.Pawn;

    /// <inheritdoc />
    public override Player Color { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Pawn"/>
    /// </summary>
    public Pawn(Player color)
    {
        Color = color;

        if (color == Player.White)
        {
            forward = Direction.North;
        }
        else
        {
            forward = Direction.South;
        }
    }

    /// <inheritdoc />
    public override Piece Copy()
    {
        var copy = new Pawn(Color)
        {
            HasMoved = HasMoved
        };
        return copy;
    }

    private static bool CanMoveTo(Position pos, Board board)
        => Board.IsInside(pos) && board.IsEmpty(pos);

    private bool CanCaptureAt(Position pos, Board board)
    {
        if (!Board.IsInside(pos) || board.IsEmpty(pos))
        {
            return false;
        }

        return board[pos]?.Color != Color;
    }

    private static IEnumerable<Move> PromotionMoves(Position from, Position to)
    {
        yield return new PawnPromotion(from, to, PieceType.Knight);
        yield return new PawnPromotion(from, to, PieceType.Bishop);
        yield return new PawnPromotion(from, to, PieceType.Rook);
        yield return new PawnPromotion(from, to, PieceType.Queen);
    }

    private IEnumerable<Move> ForwardMoves(Position from, Board board)
    {
        var oneMovePos = from + forward;

        if (CanMoveTo(oneMovePos, board))
        {
            if (oneMovePos.Row == 0 || oneMovePos.Row == (8 - 1))
            {
                foreach (var move in PromotionMoves(from, oneMovePos))
                {
                    yield return move;
                }
            }
            else
            {
                yield return new NormalMove(from, oneMovePos);
            }

            var twoMovesPos = oneMovePos + forward;

            if (!HasMoved && CanMoveTo(twoMovesPos, board))
            {
                yield return new DoublePawn(from, twoMovesPos);
            }
        }
    }

    private IEnumerable<Move> DiagonalMoves(Position from, Board board)
    {
        foreach (var dir in new Direction[] { Direction.West, Direction.East })
        {
            var to = from + forward + dir;

            if (to == board.GetPawnSkipPosition(Color.Opponent()))
            {
                yield return new EnPassant(from, to);
            }

            if (CanCaptureAt(to, board))
            {
                if (to.Row == 0 || to.Row == (8 - 1))
                {
                    foreach (var move in PromotionMoves(from, to))
                    {
                        yield return move;
                    }
                }
                else
                {
                    yield return new NormalMove(from, to);
                }
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Move> GetMoves(Position from, Board board)
        => ForwardMoves(from, board).Concat(DiagonalMoves(from, board));

    /// <inheritdoc />
    public override IEnumerable<Position> GetPremovePositions(Position from, Board board)
    {
        var oneMovePos = from + forward;

        if (Board.IsInside(oneMovePos))
        {
            yield return oneMovePos;
        }

        var twoMovePos = oneMovePos + forward;

        if (Board.IsInside(twoMovePos) && !HasMoved)
        {
            yield return twoMovePos;
        }

        foreach (var dir in new[] { Direction.West, Direction.East })
        {
            var to = from + forward + dir;
            if (Board.IsInside(to))
            {
                yield return to;
            }
        }
    }

    /// <inheritdoc />
    public override bool CanCaptureOpponentKing(Position from, Board board)
        => DiagonalMoves(from, board).Any(move =>
        {
            var piece = board[move.ToPos];
            return piece != null && piece.Type == PieceType.King;
        });
}
