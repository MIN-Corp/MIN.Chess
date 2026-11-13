using MIN.Chess.Agents.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.AgentChoose" />
public record AgentChooseMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.AgentChoose;

    /// <summary>
    /// Выбранный агент
    /// </summary>
    public AgentType AgentType { get; init; }
}
