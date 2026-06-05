using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Подсчёт фигур
/// </summary>
public record struct Counting
{
    private readonly int[] whiteCount = [];
    private readonly int[] blackCount = [];

    /// <summary>
    /// Всего фигур
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Counting"/>
    /// </summary>
    public Counting()
    {
        var piecesLength = Enum.GetValues(typeof(PieceType)).Length;
        whiteCount = new int[piecesLength];
        blackCount = new int[piecesLength];
    }

    /// <summary>
    /// Инкрементировать количество фигур
    /// </summary>
    public void Increment(Player player, PieceType type)
    {
        if (player == Player.White)
        {
            whiteCount[(int)type]++;
        }
        else if (player == Player.Black)
        {
            blackCount[(int)type]++;
        }

        TotalCount++;
    }

    /// <summary>
    /// Кол-во белых фигур
    /// </summary>
    public readonly int White(PieceType type) => whiteCount[(int)type];

    /// <summary>
    /// Кол-во чёрных фигур
    /// </summary>
    public readonly int Black(PieceType type) => blackCount[(int)type];
}
