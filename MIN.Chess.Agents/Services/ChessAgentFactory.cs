using MIN.Chess.Agents.Agents;
using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;

namespace MIN.Chess.Agents.Services;

/// <summary>
/// <inheritdoc cref="IChessAgentFactory"/>
/// </summary>
public class ChessAgentFactory : IChessAgentFactory
{
    IChessAgent IChessAgentFactory.GetAgent(AgentType agentType)
        => agentType switch
        {
            AgentType.Random => new RandomizedAgent(),
            AgentType.Smart => new SmartAgent(),
            _ => throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null)
        };
}
