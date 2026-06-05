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

    private void AddStartPieces()
    {
        this[0, 0] = new Rook(Player.Black);
        this[0, 1] = new Knight(Player.Black);
        this[0, 2] = new Bishop(Player.Black);
        this[0, 3] = new Queen(Player.Black);
        this[0, 4] = new King(Player.Black);
        this[0, 5] = new Bishop(Player.Black);
        this[0, 6] = new Knight(Player.Black);
        this[0, 7] = new Rook(Player.Black);

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
            this[1, i] = new Pawn(Player.Black);
            this[6, i] = new Pawn(Player.White);
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
        var skipPos = GetPawnSkipPosition(player);

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
