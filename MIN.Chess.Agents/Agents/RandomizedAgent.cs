using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Agents.Agents;

/// <summary>
/// Рандомный агент
/// </summary>
public class RandomizedAgent : IChessAgent
{
    AgentType IChessAgent.Type => AgentType.Random;

    Move IChessAgent.GetMove(GameState state)
        => throw new NotImplementedException();
}
