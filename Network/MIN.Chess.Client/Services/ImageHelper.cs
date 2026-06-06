using MIN.Chess.Client.Properties;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Client.Services;

/// <summary>
/// Помощник в представлении картинок для шахматных фигур
/// </summary>
public static class ImageHelper
{
    private readonly static Dictionary<PieceType, Bitmap> whitePieces = new()
    {
        { PieceType.Pawn, Resources.wPawn },
        { PieceType.Knight, Resources.wKnight },
        { PieceType.Bishop, Resources.wBishop },
        { PieceType.Rook, Resources.wRook },
        { PieceType.Queen, Resources.wQueen },
        { PieceType.King, Resources.wKing },
    };

    private readonly static Dictionary<PieceType, Bitmap> blackPieces = new()
    {
        { PieceType.Pawn, Resources.bPawn },
        { PieceType.Knight, Resources.bKnight },
        { PieceType.Bishop, Resources.bBishop },
        { PieceType.Rook, Resources.bRook },
        { PieceType.Queen, Resources.bQueen },
        { PieceType.King, Resources.bKing },
    };

    /// <summary>
    /// Получить изображение фигуры
    /// </summary>
    public static Bitmap? GetImage(Player color, PieceType pieceType)
        => color switch
        {
            Player.White => whitePieces[pieceType],
            Player.Black => blackPieces[pieceType],
            _ => null
        };

    /// <summary>
    /// Получить изображение фигуры
    /// </summary>
    public static Bitmap? GetImage(Piece piece)
    {
        if (piece == null)
        {
            return null;
        }

        return GetImage(piece.Color, piece.Type);
    }
}
