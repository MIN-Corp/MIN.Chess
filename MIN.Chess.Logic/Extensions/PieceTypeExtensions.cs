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
}
