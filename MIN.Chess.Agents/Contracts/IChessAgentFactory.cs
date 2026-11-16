using MIN.Chess.Agents.Enums;

namespace MIN.Chess.Agents.Contracts;

/// <summary>
/// Фабрика для получения ИИ агентов для шахмат
/// </summary>
public interface IChessAgentFactory
{
    /// <summary>
    /// Получить агента по типу
    /// </summary>
    IChessAgent GetAgent(AgentType agentType);
}
