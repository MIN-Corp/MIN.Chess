using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Logic.Models.Coordination;

/// <summary>
/// Позиция
/// </summary>
public readonly record struct Position
{
    /// <summary>
    /// Строка
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Стоблец
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Position"/>
    /// </summary>
    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Цвет клетки
    /// </summary>
    public Player SquareColor() => (Row + Column) % 2 == 0 ? Player.White : Player.Black;

    /// <inheritdoc cref="object.GetHashCode()"/>
    public override int GetHashCode() => HashCode.Combine(Row, Column);

    /// <inheritdoc />
    public static Position operator +(Position pos, Direction dir)
        => new(pos.Row + dir.RowDelta, pos.Column + dir.ColumnDelta);
}
