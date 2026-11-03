using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Состояние игры
/// </summary>
public class GameState
{
    private IEnumerable<Move>? cachedLegalMoves;
    private Dictionary<string, int> stateHistory = [];
    private int noCapturesOrPawnMoves;
    private string stateString;

    /// <summary>
    /// Список ходов
    /// </summary>
    public List<Move> MoveHistory { get; private init; } = [];

    /// <summary>
    /// Доска с фигурами
    /// </summary>
    public Board Board { get; set; }

    /// <summary>
    /// Текущий ход
    /// </summary>
    public Player CurrentPlayer { get; set; }

    /// <summary>
    /// Результат игры
    /// </summary>
    public Result? Result { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameState"/>
    /// </summary>
    public GameState(Board board, Player currentPlayer, bool asCopy = false)
    {
        Board = board;
        CurrentPlayer = currentPlayer;

        stateString = new StateString(CurrentPlayer, Board).ToString();

        if (asCopy)
        {
            return;
        }

        stateHistory[stateString] = 1;
    }

    /// <summary>
    /// Превратить состояние игры в строку
    /// </summary>
    public override string ToString() => new StateString(CurrentPlayer, Board).ToString();

    /// <summary>
    /// Получить все доступные позиции для фигуры
    /// </summary>
    public IEnumerable<Move> LegalMovesForPiece(Position position, Player? role = null)
    {
        if (Board.IsEmpty(position) || Board[position]?.Color != CurrentPlayer || role != null && (role != CurrentPlayer || role == Player.None))
        {
            return [];
        }

        var piece = Board[position];
        var movesCandidates = piece?.GetMoves(position, Board) ?? [];
        return movesCandidates.Where(x => x.IsLegal(Board));
    }

    /// <summary>
    /// Получить все кандидаты на ход
    /// </summary>
    public IEnumerable<Position> PremovePositionsForPiece(Position position, Player? role = null, Board? displayBoard = null)
    {
        var board = displayBoard ?? Board;

        if (board.IsEmpty(position))
        {
            return [];
        }

        var piece = board[position];

        if (role == Player.None || piece?.Color != role)
        {
            return [];
        }

        var premovePositions = piece?.GetPremovePositions(position, board) ?? [];
        return premovePositions;
    }

    /// <summary>
    /// Выполнить ход
    /// </summary>
    public void MakeMove(Move move)
    {
        MoveHistory.Add(move);

        Board.SetPawnSkipPosition(CurrentPlayer, null);
        var captureOrPawn = move.Execute(Board);

        if (captureOrPawn)
        {
            noCapturesOrPawnMoves = 0;
            stateHistory.Clear();
        }
        else
        {
            noCapturesOrPawnMoves++;
        }

        CurrentPlayer = CurrentPlayer.Opponent();
        UpdateStateString();
        cachedLegalMoves = GenerateAllLegalMoves(CurrentPlayer);
    }

    /// <summary>
    /// Получить последний сыгранный ход
    /// </summary>
    public Move? GetLastMove()
        => MoveHistory.LastOrDefault();

    private IEnumerable<Move> GenerateAllLegalMoves(Player player)
    {
        var moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
        {
            var piece = Board[pos];
            return piece?.GetMoves(pos, Board) ?? [];
        });

        return moveCandidates.Where(move => move.IsLegal(Board));
    }

    /// <summary>
    /// Получить все легальные ходы для игрока
    /// </summary>
    public IEnumerable<Move> AllLegalMovesFor(Player player)
        => cachedLegalMoves ??= GenerateAllLegalMoves(player);

    /// <summary>
    /// Окончена ли игра
    /// </summary>
    public bool IsGameOver() => Result != null;

    /// <summary>
    /// Очистить кэш
    /// </summary>
    public void ClearCache()
    {
        cachedLegalMoves = null;
    }

    /// <summary>
    /// Сработало ли правило пятидесяти ходов
    /// </summary>
    private bool FiftyMoveRule() => (noCapturesOrPawnMoves / 2) == 50;

    /// <summary>
    /// Проверить на окончание игры
    /// </summary>
    public void CheckForGameOver()
    {
        if (!cachedLegalMoves?.Any() == true)
        {
            Result = Board.IsInCheck(CurrentPlayer)
                ? Models.Result.Win(CurrentPlayer.Opponent())
                : Models.Result.Draw(EndReason.StaleMate);
        }
        else if (Board.InsufficientMaterial())
        {
            Result = Models.Result.Draw(EndReason.InsufficientMaterial);
        }
        else if (FiftyMoveRule())
        {
            Result = Models.Result.Draw(EndReason.FiftyMoveRule);
        }
        else if (ThreefoldRepetition())
        {
            Result = Models.Result.Draw(EndReason.ThreefoldRepetition);
        }
    }

    /// <summary>
    /// Оценить эвристическое состояние, выигрывает ли тот, кого сейчас текущий ход
    /// </summary>
    public float GetAdvantageScore(int pieceAdvantageWeight, int positionalAdvantageWeight, int attackingPiecesAdvantageWeight)
    {
        CheckForGameOver();

        if (Result.HasValue && Result.Value.Winner == CurrentPlayer)
        {
            return float.MaxValue;
        }

        if (Result.HasValue && Result.Value.Winner != CurrentPlayer)
        {
            return float.MinValue;
        }

        if (Result.HasValue)
        {
            return 0.0f;
        }

        var pieceAdvantage = (float)Math.Pow(GetOverallPieceScoreFor(CurrentPlayer)
                                / GetOverallPieceScoreFor(CurrentPlayer.Opponent()), pieceAdvantageWeight);

        var pieceActivityAdvantage = (float)Math.Pow(GetOverallAttackingSquaresFor(CurrentPlayer)
                                            / GetOverallAttackingSquaresFor(CurrentPlayer.Opponent()), positionalAdvantageWeight);

        var attackingPiecesAdvantage = (float)Math.Pow(GetOverallAttackingPiecesFor(CurrentPlayer)
                                                     / GetOverallAttackingPiecesFor(CurrentPlayer.Opponent()), attackingPiecesAdvantageWeight);

        return pieceAdvantage + pieceActivityAdvantage + attackingPiecesAdvantage;
    }

    /// <summary>
    /// Копия состояния игры
    /// </summary>
    public GameState Copy()
        => new(Board.Copy(), CurrentPlayer, asCopy: true)
        {
            MoveHistory = MoveHistory.ToList(),
            stateString = stateString,
            stateHistory = stateHistory.ToDictionary(x => x.Key, x => x.Value),
            noCapturesOrPawnMoves = noCapturesOrPawnMoves,
            Result = Result
        };

    private float GetOverallPieceScoreFor(Player player)
        => Enum.GetValues(typeof(PieceType)).Cast<PieceType>()
            .Sum(type => Board.CountOf(player, type) * type.GetScore());

    private float GetOverallAttackingSquaresFor(Player player)
        => Board.PiecePositionsFor(player)
            .Sum(x => (Board[x]?.GetMoves(x, Board) ?? []).Count());

    private float GetOverallAttackingPiecesFor(Player player)
        => Board.PiecePositionsFor(player).Sum(x => (Board[x]?
            .GetMoves(x, Board) ?? []).Where(y => Board[y.ToPos] != null && Board[y.ToPos]?.Color != Board[y.FromPos]?.Color)
            .Sum(z => Board[z.ToPos]?.Type.GetScore() ?? 0.0f));

    private void UpdateStateString()
    {
        stateString = new StateString(CurrentPlayer, Board).ToString();

        if (!stateHistory.TryGetValue(stateString, out var value))
        {
            stateHistory[stateString] = 1;
        }
        else
        {
            stateHistory[stateString] = ++value;
        }
    }

    private bool ThreefoldRepetition() => stateHistory[stateString] == 3;
}
