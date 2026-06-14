using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Logic.Models;

/// <summary>
/// Состояние игры
/// </summary>
public class GameState
{
    private readonly Dictionary<string, int> stateHistory = [];
    private int noCapturesOrPawnMoves;
    private string stateString;
    private IEnumerable<Move>? cachedLegalMoves;

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
    public GameState(Board board, Player currentPlayer)
    {
        Board = board;
        CurrentPlayer = currentPlayer;

        stateString = new StateString(CurrentPlayer, Board).ToString();
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
        if (Board.IsEmpty(position) || Board[position]?.Color != CurrentPlayer)
        {
            return [];
        }

        if (role != null && role != CurrentPlayer && role == Player.None)
        {
            return [];
        }

        var piece = Board[position];
        var movesCandidates = piece?.GetMoves(position, Board) ?? [];
        return movesCandidates.Where(x => x.IsLegal(Board));
    }

    /// <summary>
    /// Выполнить ход
    /// </summary>
    public void MakeMove(Move move)
    {
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
    /// Сработало ли правило пятидесяти ходов
    /// </summary>
    public bool FiftyMoveRule() => (noCapturesOrPawnMoves / 2) == 50;

    /// <summary>
    /// Проверить на окончание игры
    /// </summary>
    public void CheckForGameOver()
    {
        if (!cachedLegalMoves?.Any() == true)
        {
            if (Board.IsInCheck(CurrentPlayer))
            {
                Result = Models.Result.Win(CurrentPlayer.Opponent());
            }
            else
            {
                Result = Models.Result.Draw(EndReason.StaleMate);
            }
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
