using MIN.Chess.Agents.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.AgentVsAgentRequest" />
public record AgentVsAgentRequestMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.AgentVsAgentRequest;

    /// <summary>
    /// Выбранный агент на своей стороне
    /// </summary>
    public AgentType SelfAgentType { get; init; }

    /// <summary>
    /// Выбранный агент на стороне противника
    /// </summary>
    public AgentType OpponentAgentType { get; init; }
}
