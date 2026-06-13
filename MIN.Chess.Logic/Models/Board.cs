using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;
using MIN.Chess.Logic.Models.Pieces;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Доска
/// </summary>
public class Board
{
    private readonly Piece?[,] pieces = new Piece[8, 8];
    private readonly Dictionary<Player, Position?> pawnSkipPositions = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    /// <inheritdoc cref="this[int, int]"/>
    public Piece? this[int row, int column]
    {
        get => pieces[row, column];
        set => pieces[row, column] = value;
    }

    /// <inheritdoc cref="this[Position]"/>
    public Piece? this[Position position]
    {
        get => pieces[position.Row, position.Column];
        set => pieces[position.Row, position.Column] = value;
    }

    /// <summary>
    /// Получить клетку взятия пешки
    /// </summary>
    public Position? GetPawnSkipPosition(Player player) => pawnSkipPositions[player];

    /// <summary>
    /// Установить клетку взятия пешки
    /// </summary>
    public void SetPawnSkipPosition(Player player, Position? position)
    {
        pawnSkipPositions[player] = position;
    }

    /// <summary>
    /// Начальная позициая на доске
    /// </summary>
    public static Board Initial()
    {
        var board = new Board();
        board.AddStartPieces();
        return board;
    }

    /// <summary>
    /// Начальная позициая на доске
    /// </summary>
    /// <param name="player">Для кого инициализировать доску</param>
    public static Board Initial(Player player)
    {
        var board = new Board();
        board.AddStartPieces(player);
        return board;
    }

    /// <summary>
    /// Посчитать фигуры
    /// </summary>
    public int CountOf(Player player, PieceType type)
        => PiecePositionsFor(player).Count(x => this[x]?.Type == type);

    /// <summary>
    /// Получить доску от состояния в виде строки
    /// </summary>
    public static Board FromStateString(string stateString)
    {
        var parts = stateString.Split(' ');
        var placement = parts[0];
        var activeColor = parts[1];       // "w" or "b"
        var castling = parts.Length > 2 ? parts[2] : "-";
        var enPassant = parts.Length > 3 ? parts[3] : "-";

        var board = new Board();

        // 1. Расстановка фигур
        var rows = placement.Split('/');
        for (var r = 0; r < 8; r++)
        {
            var c = 0;
            foreach (var ch in rows[r])
            {
                if (char.IsDigit(ch))
                {
                    c += ch - '0';
                    continue;
                }

                var color = char.IsUpper(ch) ? Player.White : Player.Black;
                var pieceType = char.ToLower(ch) switch
                {
                    'p' => PieceType.Pawn,
                    'n' => PieceType.Knight,
                    'b' => PieceType.Bishop,
                    'r' => PieceType.Rook,
                    'q' => PieceType.Queen,
                    'k' => PieceType.King,
                    _ => throw new ArgumentException($"Unknown piece: {ch}"),
                };

                var pos = new Position(r, c);
                board[pos] = CreatePiece(pieceType, color);
                c++;
            }
        }

        // 2. HasMoved для короля и ладей — по флагам рокировки
        var canCastleK = castling.Contains('K');
        var canCastleQ = castling.Contains('Q');
        var canCastlek = castling.Contains('k');
        var canCastleq = castling.Contains('q');

        SetHasMovedForKingAndRook(board, Player.White, canCastleK, canCastleQ);
        SetHasMovedForKingAndRook(board, Player.Black, canCastlek, canCastleq);

        // 3. En passant
        if (enPassant != "-")
        {
            var file = enPassant[0] - 'a';
            var rank = 8 - (enPassant[1] - '0');
            var targetPos = new Position(rank, file);
            var currentPlayer = activeColor == "w" ? Player.White : Player.Black;
            board.SetPawnSkipPosition(currentPlayer.Opponent(), targetPos);
        }

        return board;
    }

    private static Piece CreatePiece(PieceType type, Player color) => type switch
    {
        PieceType.Pawn => new Pawn(color),
        PieceType.Knight => new Knight(color),
        PieceType.Bishop => new Bishop(color),
        PieceType.Rook => new Rook(color),
        PieceType.Queen => new Queen(color),
        PieceType.King => new King(color),
        _ => throw new ArgumentException($"Unknown type: {type}"),
    };

    private static void SetHasMovedForKingAndRook(Board board, Player color, bool canCastleK, bool canCastleQ)
    {
        var backRank = color == Player.White ? 7 : 0;

        var kingPos = new Position(backRank, 4);
        var king = board[kingPos] as King;
        king?.HasMoved = !canCastleK && !canCastleQ;

        var rookHPos = new Position(backRank, 7);
        if (board[rookHPos] is Rook rookH && !canCastleK)
        {
            rookH.HasMoved = true;
        }

        var rookAPos = new Position(backRank, 0);
        if (board[rookAPos] is Rook rookA && !canCastleQ)
        {
            rookA.HasMoved = true;
        }
    }

    private void AddStartPieces()
    {
        AddStartPieces(Player.White);
        AddStartPieces(Player.Black);
    }

    private void AddStartPieces(Player player)
    {
        if (player == Player.Black)
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            for (var i = 0; i < 8; i++)
            {
                this[1, i] = new Pawn(Player.Black);
            }
        }
        else
        {
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            for (var i = 0; i < 8; i++)
            {
                this[6, i] = new Pawn(Player.White);
            }
        }
    }

    /// <summary>
    /// Находиться ли данная позиция внутри доски
    /// </summary>
    public static bool IsInside(Position position)
        => position.Row >= 0 && position.Column >= 0 && position.Row < 8 && position.Column < 8;

    /// <summary>
    /// Пуста ли клетка на этой позиции
    /// </summary>
    public bool IsEmpty(Position position)
        => this[position] == null;

    /// <summary>
    /// Получить позиции, где находятся фигуры
    /// </summary>
    public IEnumerable<Position> PiecePositions()
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var position = new Position(r, c);

                if (!IsEmpty(position))
                {
                    yield return position;
                }
            }
        }
    }

    /// <summary>
    /// Получить позиции, где находятся фигуры для указанного игрока
    /// </summary>
    public IEnumerable<Position> PiecePositionsFor(Player player)
        => PiecePositions().Where(x => this[x]?.Color == player);

    /// <summary>
    /// Находиться ли сейчас король под угрозой
    /// </summary>
    public bool IsInCheck(Player player)
        => PiecePositionsFor(player.Opponent()).Any(pos =>
        {
            var piece = this[pos];
            return piece?.CanCaptureOpponentKing(pos, this) == true;
        });

    /// <summary>
    /// Сделать копию доски с фигурами
    /// </summary>
    public Board Copy()
    {
        var copy = new Board();

        var piecePositions = PiecePositions();

        foreach (var pos in piecePositions)
        {
            copy[pos] = this[pos]?.Copy();
        }

        return copy;
    }

    /// <summary>
    /// Посчитать фигуры
    /// </summary>
    public Counting CountPieces()
    {
        var counting = new Counting();
        var piecePositions = PiecePositions();
        foreach (var position in piecePositions)
        {
            var piece = this[position];
            counting.Increment(piece!.Color, piece.Type);
        }

        return counting;
    }

    /// <summary>
    /// Достаточно ли материала у двух игроков
    /// </summary>
    public bool InsufficientMaterial()
    {
        var counting = CountPieces();
        return IsKingVKing(counting) || IsKingBishopVKing(counting)
            || IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
    }

    private static bool IsKingVKing(Counting counting) => counting.TotalCount == 2;

    private static bool IsKingBishopVKing(Counting counting)
        => counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);

    private static bool IsKingKnightVKing(Counting counting)
        => counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);

    private bool IsKingBishopVKingBishop(Counting counting)
    {
        if (counting.TotalCount != 4)
        {
            return false;
        }

        if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)
        {
            return false;
        }

        var wBishopPos = FindPiece(Player.White, PieceType.Bishop);
        var bBishopPos = FindPiece(Player.Black, PieceType.Bishop);

        return wBishopPos.SquareColor() == bBishopPos.SquareColor();
    }

    private Position FindPiece(Player player, PieceType type)
        => PiecePositionsFor(player).First(x => this[x]?.Type == type);

    private bool IsUnmobedKingAndRook(Position kingPos, Position rookPos)
    {
        if (IsEmpty(kingPos) || IsEmpty(rookPos))
        {
            return false;
        }

        var king = this[kingPos];
        var rook = this[rookPos];

        return king?.Type == PieceType.King && rook?.Type == PieceType.Rook
            && !king.HasMoved && !rook.HasMoved;
    }

    /// <summary>
    /// Права короля на рокировку в сторону короля
    /// </summary>
    public bool CastleRightKS(Player player)
        => player switch
        {
            Player.White => IsUnmobedKingAndRook(new Position(7, 4), new Position(7, 7)),
            Player.Black => IsUnmobedKingAndRook(new Position(0, 4), new Position(0, 7)),
            _ => false
        };

    /// <summary>
    /// Права короля на рокировку в сторону ферзя
    /// </summary>
    public bool CastleRightQS(Player player)
        => player switch
        {
            Player.White => IsUnmobedKingAndRook(new Position(7, 4), new Position(7, 0)),
            Player.Black => IsUnmobedKingAndRook(new Position(0, 4), new Position(0, 0)),
            _ => false
        };

    private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPosition)
    {
        foreach (var pos in pawnPositions.Where(IsInside))
        {
            var piece = this[pos];
            if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn)
            {
                continue;
            }

            var move = new EnPassant(pos, skipPosition);
            if (move.IsLegal(this))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Может в данной позиции взять пешкой на проходе
    /// </summary>
    public bool CanCaptureEnPassant(Player player)
    {
        if (player == Player.None)
        {
            return false;
        }

        var skipPos = GetPawnSkipPosition(player.Opponent());

        if (skipPos == null)
        {
            return false;
        }

        var pawnPositions = player switch
        {
            Player.White => new Position[] { skipPos.Value + Direction.SouthWest, skipPos.Value + Direction.SouthEast },
            Player.Black => new Position[] { skipPos.Value + Direction.NorthWest, skipPos.Value + Direction.NorthEast },
            _ => []
        };

        return HasPawnInPosition(player, pawnPositions, skipPos.Value);
    }
}
