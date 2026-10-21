namespace MIN.Chess.Logic.Enums;

/// <summary>
/// Тип хода
/// </summary>
public enum MoveType
{
    /// <summary>
    /// Обычный ход
    /// </summary>
    Normal,

    /// <summary>
    /// Рокировка в сторону короля
    /// </summary>
    CastleKs,

    /// <summary>
    /// Рокировка в сторону ферзя
    /// </summary>
    CastleQs,

    /// <summary>
    /// Движение пешки на две клетки
    /// </summary>
    DoublePawn,

    /// <summary>
    /// Взятие на проходе
    /// </summary>
    EnPassant,

    /// <summary>
    /// Продвижение пешки
    /// </summary>
    PawnPromotion,
}
