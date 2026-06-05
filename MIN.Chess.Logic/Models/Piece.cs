using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Фигура
/// </summary>
public abstract class Piece
{
    /// <summary>
    /// Тип фигуры
    /// </summary>
    public abstract PieceType Type { get; }

    /// <summary>
    /// Цвет фигуры
    /// </summary>
    public abstract Player Color { get; }

    /// <summary>
    /// Двигалась ли фигура раньше
    /// </summary>
    public bool HasMoved { get; set; }

    /// <summary>
    /// Скопировать фигуру
    /// </summary>
    public abstract Piece Copy();

    /// <summary>
    /// Получить все доступные ходы
    /// </summary>
    public abstract IEnumerable<Move> GetMoves(Position from, Board board);

    /// <summary>
    /// Получить все доступные ходы в заданном направлении
    /// </summary>
    protected IEnumerable<Position> MovePositionsInDir(Position from, Board board, Direction dir)
    {
        for (Position pos = from + dir; Board.IsInside(pos); pos += dir)
        {
            if (board.IsEmpty(pos))
            {
                yield return pos;
                continue;
            }

            var piece = board[pos];

            if (piece?.Color != Color)
            {
                yield return pos;
            }

            yield break;
        }
    }

    /// <summary>
    /// Получить все доступные ходы в заданных направлениях
    /// </summary>
    protected IEnumerable<Position> MovePositionsInDirs(Position from, Board board, Direction[] dirs)
        => dirs.SelectMany(x => MovePositionsInDir(from, board, x));

    /// <summary>
    /// Атакует ли фигура короля
    /// </summary>
    public virtual bool CanCaptureOpponentKing(Position from, Board board)
        => GetMoves(from, board).Any(move =>
        {
            var piece = board[move.ToPos];
            return piece != null && piece.Type == PieceType.King;
        });
}
