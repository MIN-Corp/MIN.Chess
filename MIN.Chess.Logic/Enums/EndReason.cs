namespace MIN.Chess.Logic.Enums;

/// <summary>
/// Причина окончания игры
/// </summary>
public enum EndReason
{
    /// <summary>
    /// Мат
    /// </summary>
    CheckMate,

    /// <summary>
    /// Сдача
    /// </summary>
    Resign,

    /// <summary>
    /// Ничья
    /// </summary>
    Draw,

    /// <summary>
    /// Нету доступных ходов
    /// </summary>
    StaleMate,

    /// <summary>
    /// Правило 50 ходов
    /// </summary>
    FiftyMoveRule,

    /// <summary>
    /// Недостаточно материала
    /// </summary>
    InsufficientMaterial,

    /// <summary>
    /// Трёхкратное повторение
    /// </summary>
    ThreefoldRepetition,
}
