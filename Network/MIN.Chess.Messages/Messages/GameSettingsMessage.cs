using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.GameSettings" />
public record GameSettingsMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.GameSettings;

    /// <summary>
    /// Время партии
    /// </summary>
    /// <remarks>
    /// null - если без таймера
    /// </remarks>
    public int? TimerInitialMinutes { get; init; }

    /// <summary>
    /// Добавка времени
    /// </summary>
    public int TimerBonusSeconds { get; init; }
}
