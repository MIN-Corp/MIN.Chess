using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Результат игры
/// </summary>
public record struct Result
{
    /// <summary>
    /// Победивший игрок
    /// </summary>
    public Player Winner { get; set; }

    /// <summary>
    /// Причина окончания игры
    /// </summary>
    public EndReason Reason { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="Result"/>
    /// </summary>
    public Result(Player winner, EndReason reason)
    {
        Winner = winner;
        Reason = reason;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public static Result Win(Player winner)
        => new(winner, EndReason.CheckMate);

    /// <summary>
    /// Ничья
    /// </summary>
    public static Result Draw(EndReason endReason)
        => new(Player.None, endReason);
}
