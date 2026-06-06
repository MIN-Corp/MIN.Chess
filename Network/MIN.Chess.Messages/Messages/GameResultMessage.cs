using MIN.Chess.Logic.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="MessageType.GameResult" />
public record GameResultMessage : BaseMessage
{
    /// <inheritdoc />
    public override MessageType MessageType => MessageType.GameResult;

    /// <summary>
    /// Результат игры
    /// </summary>
    public Result Result { get; }
}
