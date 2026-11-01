using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Agents.Agents;

/// <summary>
/// Умный агент на основе alpha beta алгоритма
/// </summary>
public class SmartAgent : IChessAgent
{
    private readonly Random random = new();
    private const int Depth = 5;

    private struct MinMaxMove
    {
        /// <summary>
        /// Ход
        /// </summary>
        public readonly Move Move;

        /// <summary>
        /// Выигрыш
        /// </summary>
        public readonly float Score = 0.0f;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MinMaxMove"/>
        /// </summary>
        public MinMaxMove(Move move, float score)
        {
            Move = move;
            Score = score;
        }
    }

    AgentType IChessAgent.Type => AgentType.Smart;

    Task<Move> IChessAgent.GetMove(GameState state)
        => Task.FromResult(MinMax(state, Depth, 1,
            state.GetLastMove() ?? GetRandomMove(state), float.MinValue, float.MaxValue).Move);

    private Move GetRandomMove(GameState state)
    {
        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        return moves.ElementAt(random.Next(0, moves.Length));
    }

    private MinMaxMove MinMax(GameState state, int depth, int minOrMax, Move lastMove, float alpha, float beta)
    {
        var currentScore = state.GetAdvantageScore();
        if (currentScore != 0 || !state.AllLegalMovesFor(state.CurrentPlayer).Any() || depth == 0)
        {
            Console.WriteLine($"NO MOVE: {lastMove.ToUci()} SCORE: {currentScore}");
            return new MinMaxMove(lastMove, currentScore);
        }

        var bestScore = minOrMax * float.MinValue;
        Move? bestMove = null;

        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        foreach (var move in moves)
        {
            var neighbor = state.Copy();
            neighbor.MakeMove(move);
            var bestNeighbor = MinMax(state, depth - 1, minOrMax * -1, move, alpha, beta);
            if ((bestNeighbor.Score > bestScore && minOrMax == 1)
                || bestNeighbor.Score < bestScore && minOrMax == -1)
            {
                bestScore = bestNeighbor.Score;
                bestMove = bestNeighbor.Move;
                if (bestScore >= alpha)
                {
                    alpha = bestScore;
                }

                if (bestScore <= beta)
                {
                    beta = bestScore;
                }
            }

            if (alpha >= beta)
            {
                break;
            }
        }

        Console.WriteLine($"BEST: {bestMove?.ToUci()}");
        return new MinMaxMove(bestMove ?? GetRandomMove(state), bestScore);
    }
}
