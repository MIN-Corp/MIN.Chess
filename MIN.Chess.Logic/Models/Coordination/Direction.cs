namespace MIN.Chess.Logic.Models.Coordination;

/// <summary>
/// Направление
/// </summary>
public readonly record struct Direction
{
    /// <summary>
    /// Север
    /// </summary>
    public readonly static Direction North = new(-1, 0);

    /// <summary>
    /// Юг
    /// </summary>
    public readonly static Direction South = new(1, 0);

    /// <summary>
    /// Восток
    /// </summary>
    public readonly static Direction East = new(0, 1);

    /// <summary>
    /// Запад
    /// </summary>
    public readonly static Direction West = new(0, -1);

    /// <summary>
    /// Северо - восток
    /// </summary>
    public readonly static Direction NorthEast = North + East;

    /// <summary>
    /// Северо - запад
    /// </summary>
    public readonly static Direction NorthWest = North + West;

    /// <summary>
    /// Юго - восток
    /// </summary>
    public readonly static Direction SouthEast = South + East;

    /// <summary>
    /// Юго - запад
    /// </summary>
    public readonly static Direction SouthWest = South + West;

    /// <summary>
    /// Отступ по строкам
    /// </summary>
    public int RowDelta { get; }

    /// <summary>
    /// Отступ по столбцам
    /// </summary>
    public int ColumnDelta { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Direction"/>
    /// </summary>
    public Direction(int rowDelta, int columnDelta)
    {
        RowDelta = rowDelta;
        ColumnDelta = columnDelta;
    }

    /// <inheritdoc />
    public static Direction operator +(Direction dir1, Direction dir2)
        => new(dir1.RowDelta + dir2.RowDelta, dir1.ColumnDelta + dir2.ColumnDelta);

    /// <inheritdoc />
    public static Direction operator *(int scalar, Direction dir)
        => new(scalar * dir.RowDelta, scalar * dir.ColumnDelta);
}
