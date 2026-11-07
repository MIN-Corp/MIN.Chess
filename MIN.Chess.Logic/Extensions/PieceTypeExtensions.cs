using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Logic.Extensions;

/// <summary>
/// Расширения для <see cref="PieceType"/>
/// </summary>
public static class PieceTypeExtensions
{
    /// <summary>
    /// Перевести тип фигуры в нотацию
    /// </summary>
    public static char ToUciChar(this PieceType type) => type switch
    {
        PieceType.Knight => 'n',
        PieceType.Bishop => 'b',
        PieceType.Rook => 'r',
        PieceType.Queen => 'q',
        _ => throw new ArgumentException($"Invalid promotion type: {type}"),
    };

    /// <summary>
    /// Получить цену фигуры
    /// </summary>
    public static float GetScore(this PieceType type) => type switch
    {
        PieceType.Pawn => 1.0f,
        PieceType.Knight => 3.0f,
        PieceType.Bishop => 3.0f,
        PieceType.Rook => 5.0f,
        PieceType.Queen => 10.0f,
        PieceType.King => 0.0f,
        _ => throw new ArgumentException($"Invalid promotion type: {type}"),
    };
}
