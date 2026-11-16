using MIN.Chess.Agents.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.AgentChangeRequest" />
public record AgentChangeRequestMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.AgentChangeRequest;

    /// <summary>
    /// Тип ИИ агента
    /// </summary>
    public AgentType AgentType { get; init; }
}
