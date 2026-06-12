using MIN.Chess.Logic.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.PlayerConnected" />
public record PlayerConnectedMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.PlayerConnected;

    /// <summary>
    /// Роль игрока
    /// </summary>
    public Player Role { get; init; }

    /// <summary>
    /// Данные об участнике
    /// </summary>
    public Participant Participant { get; init; } = null!;
}
