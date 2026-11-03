using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Agents.Agents;

/// <summary>
/// Умный агент на основе alpha beta алгоритма
/// </summary>
public class SmartAgent : IChessAgent
{
    private readonly Random random = new();
    private const int Depth = 3;

    private readonly struct MinMaxMove
    {
        /// <summary>
        /// Ход
        /// </summary>
        public readonly GameState State;

        /// <summary>
        /// Ход
        /// </summary>
        public readonly Move? BestMove;

        /// <summary>
        /// Выигрыш
        /// </summary>
        public readonly float Score = 0.0f;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MinMaxMove"/>
        /// </summary>
        public MinMaxMove(Move? move, float score, GameState state)
        {
            BestMove = move;
            State = state;
            Score = score;
        }

        /// <summary>
        /// Получить первый ход
        /// </summary>
        public Move? GetFirst(int index)
        {
            try
            {
                return State.MoveHistory[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }

    AgentType IChessAgent.Type => AgentType.Smart;

    Task<Move> IChessAgent.GetMove(GameState state)
    {
        var piecesCount = state.Board.PiecePositionsFor(state.CurrentPlayer).Count()
            + state.Board.PiecePositionsFor(state.CurrentPlayer.Opponent()).Count();
        var appropriateDepth = (int)(1 / (0.1 * piecesCount + 0.5) + Depth);

        var compute = MinMax(state, appropriateDepth, -1,
                state.GetLastMove() ?? GetRandomMove(state));

        Console.WriteLine($"HERE's my line: {string.Join(',', compute.State.MoveHistory.Select(x => x.ToUci()))}");
        var desired = compute.GetFirst(state.MoveHistory.Count) ?? GetRandomMove(state);
        Console.WriteLine($"I WANT TO WALK LIKE THIS: {desired.ToUci()}");

        return Task.FromResult(desired);
    }

    private Move GetRandomMove(GameState state)
    {
        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        return moves.ElementAt(random.Next(0, moves.Length));
    }

    private MinMaxMove MinMax(GameState state, int depth, int minOrMax, Move lastMove)
    {
        var currentScore = state.GetAdvantageScore();
        if (state.Result.HasValue || !state.AllLegalMovesFor(state.CurrentPlayer).Any() || depth == 0)
        {
            Console.WriteLine($"NO MOVE: {lastMove.ToUci()} SCORE: {currentScore}, depth: {depth}");
            return new MinMaxMove(lastMove, currentScore, state);
        }

        var bestScore = minOrMax * float.MinValue;
        Move? bestMove = null;
        GameState? bestState = null;

        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        foreach (var move in moves)
        {
            var neighbor = state.Copy();
            neighbor.MakeMove(move);
            var bestClosestNeighbor = MinMax(neighbor, depth - 1, minOrMax * -1, move);

            if ((bestClosestNeighbor.Score > bestScore && minOrMax == 1)
                || (bestClosestNeighbor.Score < bestScore && minOrMax == -1))
            {
                bestMove = bestClosestNeighbor.BestMove;
                bestScore = bestClosestNeighbor.Score;
                bestState = bestClosestNeighbor.State;
            }
        }

        Console.WriteLine($"BEST: {bestMove?.ToUci()}, SCORE: {bestScore}, depth: {depth}");
        return new MinMaxMove(bestMove ?? GetRandomMove(state), bestScore, bestState ?? state);
    }
}
