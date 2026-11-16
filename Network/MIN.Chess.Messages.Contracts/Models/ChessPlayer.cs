using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Enums;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Messages.Contracts.Models;

/// <summary>
/// Игрок в шахматы
/// </summary>
public record ChessPlayer : Participant
{
    /// <summary>
    /// Роль участника
    /// </summary>
    public Player Role { get; set; }

    /// <summary>
    /// Игрок - ИИ
    /// </summary>
    public bool IsAi { get; init; }

    /// <summary>
    /// Тип агента (если это агента
    /// </summary>
    public AgentType? AgentType { get; init; } = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChessPlayer"/>
    /// </summary>
    public ChessPlayer(Participant participant) : base(participant) { }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChessPlayer"/>
    /// </summary>
    public ChessPlayer(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChessPlayer"/>
    /// </summary>
    public ChessPlayer() { }
}
