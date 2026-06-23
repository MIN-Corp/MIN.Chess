using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.RestartVote" />
public record RestartVoteMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.RestartVote;

    /// <summary>
    /// Текущее количество голосов
    /// </summary>
    public int CurrentVotes { get; init; }
}
