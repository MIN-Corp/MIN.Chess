using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Ход
/// </summary>
public abstract class Move
{
    /// <inheritdoc cref="MoveType"/>
    public abstract MoveType Type { get; }

    /// <summary>
    /// Начальная позиция
    /// </summary>
    public abstract Position FromPos { get; }

    /// <summary>
    /// Конечная позиция
    /// </summary>
    public abstract Position ToPos { get; }

    /// <summary>
    /// Выполнить ход
    /// </summary>
    /// <returns>
    /// true - если было взятие или движение пешкой
    /// false - если нет
    /// </returns>
    public abstract bool Execute(Board board);

    /// <summary>
    /// Доступен ли ход (не будет ли король под угрозой после хода)
    /// </summary>
    public virtual bool IsLegal(Board board)
    {
        var player = board[FromPos]?.Color;
        var boardCopy = board.Copy();
        Execute(boardCopy);
        return !boardCopy.IsInCheck(player!.Value);
    }
}
