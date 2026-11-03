using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Agents.Contracts;

/// <summary>
/// ИИ-агент (движок) для шахмат
/// </summary>
public interface IChessAgent
{
    /// <summary>
    /// Тип агента
    /// </summary>
    AgentType Type { get; }

    /// <summary>
    /// Имя агента
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Получить ход агента от состояния
    /// </summary>
    Task<Move> GetMove(GameState state);
}
