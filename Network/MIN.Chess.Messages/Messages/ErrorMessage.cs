using MIN.Chess.Messages.Contracts.Enums;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Messages.Messages;

/// <inheritdoc cref="NetworkMessageType.Error" />
public record ErrorMessage : BaseNetworkMessage
{
    /// <inheritdoc />
    public override NetworkMessageType MessageType => NetworkMessageType.Error;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; init; } = "Произошла ошибка";
}
