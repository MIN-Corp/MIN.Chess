using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.GameResult" />
public record GameResultMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.GameResult;

    /// <summary>
    /// Результат игры
    /// </summary>
    public Result Result { get; }
}
