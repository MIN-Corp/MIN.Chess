using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.SwitchSidesVote" />
public record SwitchSidesVoteMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.SwitchSidesVote;

    /// <summary>
    /// Текущее количество голосов
    /// </summary>
    public int CurrentVotes { get; init; }
}
