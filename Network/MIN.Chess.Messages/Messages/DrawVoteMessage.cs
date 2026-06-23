using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.DrawVote" />
public record DrawVoteMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.DrawVote;

    /// <summary>
    /// Текущее количество голосов
    /// </summary>
    public int CurrentVotes { get; init; }
}
