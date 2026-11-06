using System.Diagnostics;
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
    private const int ParallelDepth = 2;
    private const float PieceAdvantageWeight = 20f;
    private const float PositionalAdvantageWeight = 0.1f;
    private const float AttackingAdvantageWeight = 0.01f;

    string IChessAgent.DisplayName => "Магнус Карлсен";

    private readonly struct MinMaxMove
    {
        /// <summary>
        /// Состояние
        /// </summary>
        public readonly GameState State;

        /// <summary>
        /// Выигрыш
        /// </summary>
        public readonly float Score = 0.0f;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MinMaxMove"/>
        /// </summary>
        public MinMaxMove(float score, GameState state)
        {
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

    async Task<Move> IChessAgent.GetMove(GameState state)
    {
        if (state.MoveHistory.Count <= 1)
        {
            return GetRandomMove(state);
        }

        var piecesCount = state.Board.PiecePositionsFor(state.CurrentPlayer).Count()
            + state.Board.PiecePositionsFor(state.CurrentPlayer.Opponent()).Count();
        var appropriateDepth = (int)(24 / Math.Sqrt(piecesCount + 20));

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var compute = await MinMax(state, appropriateDepth, appropriateDepth % 2 == 0 ? 1 : -1, ParallelDepth);
        var firstLine = compute.FirstOrDefault();

        stopWatch.Stop();

        Console.WriteLine($"THOUGHT FOR: {stopWatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"FORESAW {appropriateDepth} MOVES AHEAD");
        Console.WriteLine($"I THINK I AM {firstLine.Score}");

        for (var line = 0; line < compute.Length; line++)
        {
            Console.WriteLine($"HERE's my {line + 1} line ({compute[line].Score}): {string.Join(',', compute[line].State.MoveHistory.Skip(state.MoveHistory.Count).Select(x => x.ToUci()))}");
        }

        var desired = firstLine.GetFirst(state.MoveHistory.Count) ?? GetRandomMove(state);

        var stopWatch2 = new Stopwatch();
        stopWatch2.Start();

        var compute2 = await MinMax(state, 4, 1, ParallelDepth, compute.Select(x => x.GetFirst(state.MoveHistory.Count) ?? GetRandomMove(state)).Take(5).ToArray());
        var firstLine2 = compute.FirstOrDefault();

        stopWatch2.Stop();

        Console.WriteLine($"THOUGHT FOR: {stopWatch2.ElapsedMilliseconds}ms");
        Console.WriteLine($"FORESAW {appropriateDepth} MOVES AHEAD");
        Console.WriteLine($"I THINK I AM {firstLine2.Score}");

        for (var line = 0; line < compute2.Length; line++)
        {
            Console.WriteLine($"HERE's my {line + 1} line ({compute2[line].Score}): {string.Join(',', compute2[line].State.MoveHistory.Skip(state.MoveHistory.Count).Select(x => x.ToUci()))}");
        }

        var desired2 = firstLine2.GetFirst(state.MoveHistory.Count) ?? GetRandomMove(state);

        return desired2;
    }

    private Move GetRandomMove(GameState state)
    {
        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        return moves.ElementAt(random.Next(0, moves.Length));
    }

    private static async Task<MinMaxMove[]> MinMax(GameState state, int depth,
        int minOrMax, int parallelDepth, Move[]? selectedMoves = null)
    {
        var currentScore = state.GetAdvantageScore(PieceAdvantageWeight, PositionalAdvantageWeight, AttackingAdvantageWeight);
        if (float.IsNegativeInfinity(currentScore) && minOrMax == -1)
        {
            currentScore = float.PositiveInfinity;
        }

        if (state.Result.HasValue || depth == 0)
        {
            return [new MinMaxMove(currentScore, state)];
        }

        var moves = selectedMoves ?? state.AllLegalMovesFor(state.CurrentPlayer).ToArray();

        var childDepth = depth - 1;
        var childMinOrMax = -minOrMax;

        var results = new MinMaxMove[moves.Length];

        if (parallelDepth > 0)
        {
            await Parallel.ForEachAsync(
                Enumerable.Range(0, moves.Length),
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                async (i, _) =>
                {
                    var neighbor = state.Copy();
                    neighbor.MakeMove(moves[i]);
                    var bestMoves = await MinMax(
                        neighbor, childDepth, childMinOrMax, parallelDepth > 0 ? parallelDepth - 1 : 0);
                    results[i] = bestMoves[0];
                });
        }
        else
        {
            for (var i = 0; i < moves.Length; i++)
            {
                var neighbor = state.Copy();
                neighbor.MakeMove(moves[i]);
                var bestMoves = await MinMax(
                    neighbor, childDepth, childMinOrMax, 0);
                results[i] = bestMoves[0];
            }
        }

        var best = results;

        if (minOrMax == 1)
        {
            best = results.OrderByDescending(x => x.Score).ToArray();
        }
        else if (minOrMax == -1)
        {
            best = results.OrderBy(x => x.Score).ToArray();
        }

        return best;
    }
}
