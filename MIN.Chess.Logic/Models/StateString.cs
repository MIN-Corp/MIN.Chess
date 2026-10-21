using System.Text;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Состояние, представленное в виде строки
/// </summary>
public class StateString
{
    private readonly StringBuilder sb = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="StateString"/>
    /// </summary>
    public StateString(Player player, Board board)
    {
        AddPiecePlacement(board);
        sb.Append(' ');
        AddCurrentPlayer(player);
        sb.Append(' ');
        AddCastlingRights(board);
        sb.Append(' ');
        AddEnPassant(board, player);
    }

    /// <inheritdoc cref="StringBuilder.ToString()"/>
    public override string ToString()
        => sb.ToString();

    private static char PieceChar(Piece piece)
    {
        var c = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Rook => 'r',
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '\0'
        };

        if (piece.Color == Player.White)
        {
            return char.ToUpper(c);
        }

        return c;
    }

    private void AddRowData(Board board, int row)
    {
        var empty = 0;

        for (var c = 0; c < 8; c++)
        {
            if (board[row, c] == null)
            {
                empty++;
                continue;
            }

            if (empty > 0)
            {
                sb.Append(empty);
                empty = 0;
            }

            sb.Append(PieceChar(board[row, c]!));
        }
    }

    private void AddPiecePlacement(Board board)
    {
        for (var r = 0; r < 8; r++)
        {
            if (r != 0)
            {
                sb.Append('/');
            }
            AddRowData(board, r);
        }
    }

    private void AddCurrentPlayer(Player player)
    {
        if (player == Player.White)
        {
            sb.Append('w');
        }
        else if (player == Player.Black)
        {
            sb.Append('b');
        }
        else
        {
            sb.Append('n');
        }
    }

    private void AddCastlingRights(Board board)
    {
        var castleWks = board.CastleRightKs(Player.White);
        var castleWqs = board.CastleRightQs(Player.White);
        var castleBks = board.CastleRightKs(Player.Black);
        var castleBqs = board.CastleRightQs(Player.Black);

        if (!(castleWks || castleWqs || castleBks || castleBqs))
        {
            sb.Append('-');
            return;
        }

        if (castleWks)
        {
            sb.Append('K');
        }
        if (castleWqs)
        {
            sb.Append('Q');
        }
        if (castleBks)
        {
            sb.Append('k');
        }
        if (castleBqs)
        {
            sb.Append('q');
        }
    }

    private void AddEnPassant(Board board, Player player)
    {
        if (!board.CanCaptureEnPassant(player))
        {
            sb.Append('-');
            return;
        }

        var pos = board.GetPawnSkipPosition(player.Opponent());
        if (pos != null)
        {
            var file = (char)('a' + pos.Value.Column);
            var rank = 8 - pos.Value.Row;
            sb.Append(file);
            sb.Append(rank);
        }
    }
}
