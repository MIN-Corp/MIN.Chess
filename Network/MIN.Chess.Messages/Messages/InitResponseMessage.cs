using MIN.Chess.Logic.Enums;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.InitResponse" />
public record InitResponseMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.InitResponse;

    /// <summary>
    /// Состояние игры в виде строки
    /// </summary>
    public string StateString { get; init; } = string.Empty;

    /// <summary>
    /// Текущий ход игрока
    /// </summary>
    public Player CurrentTurn { get; init; }

    /// <summary>
    /// Роль игрока, отправившего запрос
    /// </summary>
    public Player RequesterRole { get; init; }

    /// <summary>
    /// Присвоенный участник
    /// </summary>
    public Participant SelfParticipant { get; init; } = null!;

    /// <summary>
    /// Играющие игроки
    /// </summary>
    public required IReadOnlyDictionary<Player, Participant?> ActivePlayers { get; init; }

    /// <summary>
    /// Количество наблюдающих
    /// </summary>
    public int SpectorsCount { get; init; }

    /// <summary>
    /// Последний ход (если был)
    /// </summary>
    public string? LastMove { get; init; }
}
