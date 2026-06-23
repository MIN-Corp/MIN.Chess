using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Logic.Services;

/// <summary>
/// Парсер ходов
/// </summary>
public static class MoveParser
{
    /// <summary>
    /// Распарсить нотацию
    /// </summary>
    public static Move Parse(string uci, Board board)
    {
        var from = Position.FromUci(uci[..2]);
        var to = Position.FromUci(uci[2..4]);
        var piece = board[from];

        if (uci.Length == 5 && piece?.Type == PieceType.Pawn)
        {
            var newType = uci[4] switch
            {
                'q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'n' => PieceType.Knight,
                _ => throw new ArgumentException($"Uknown piece type: {uci[4]}")
            };
            return new PawnPromotion(from, to, newType);
        }

        if (piece?.Type == PieceType.King && Math.Abs(from.Column - to.Column) == 2)
        {
            var type = to.Column > from.Column ? MoveType.CastleKS : MoveType.CastleQS;
            return new Castle(type, from);
        }

        if (piece?.Type == PieceType.Pawn && from.Column != to.Column && board.IsEmpty(to))
        {
            return new EnPassant(from, to);
        }

        if (piece?.Type == PieceType.Pawn && Math.Abs(from.Row - to.Row) == 2)
        {
            return new DoublePawn(from, to);
        }

        return new NormalMove(from, to);
    }
}
