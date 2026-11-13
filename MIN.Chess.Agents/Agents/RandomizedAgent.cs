using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Agents.Agents;

/// <summary>
/// Рандомный агент
/// </summary>
public class RandomizedAgent : IChessAgent
{
    private readonly Random random = new();
    private const float MinThinkingTime = 0.5f;
    private const float MaximumThinkingTime = 2.0f;

    AgentType IChessAgent.Type => AgentType.Random;

    string IChessAgent.DisplayName => "Рандомич";

    Task<Move> IChessAgent.GetMove(GameState state)
    {
        var allMoves = state.AllLegalMovesFor(state.CurrentPlayer);
        var moves = allMoves as Move[] ?? allMoves.ToArray();

        //var timeToThink = MinThinkingTime + random.NextSingle() * (MaximumThinkingTime - MinThinkingTime);
        //await Task.Delay((int)(timeToThink * 1000.0f));

        return Task.FromResult(moves.ElementAt(random.Next(0, moves.Length)));
    }
}
