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
    /// Перевести позицию в нотацию
    /// </summary>
    public readonly string ToUci() => $"{(char)('a' + Column)}{8 - Row}";

    /// <summary>
    /// Получить позицию от нотации
    /// </summary>
    public static Position FromUci(string uci) => new(8 - (uci[1] - '0'), uci[0] - 'a');

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
