using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис сообщений
/// </summary>
internal class MessageService
{
    private readonly IClientProcessBridge bridge;
    private readonly CancellationTokenSource cts;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageService"/>
    /// </summary>
    public MessageService(IClientProcessBridge bridge, CancellationTokenSource cts)
    {
        this.bridge = bridge;
        this.cts = cts;
    }

    /// <summary>
    /// Разослать сообщение всем участникам
    /// </summary>
    public Task BroadcastAsync<T>(T message)
        where T : class, INetworkMessage
        => bridge.SendMessage(message,
            BroadcastExcludeIds: RoomService.GetBroadcastExcludeList(), cancellationToken: cts.Token);

    /// <summary>
    /// Разослать сообщение всем участникам, исключая одного
    /// </summary>
    public Task BroadcastExcludingAsync<T>(T message, Guid? excludeId)
        where T : class, INetworkMessage
        => bridge.SendMessage(message,
            BroadcastExcludeIds: RoomService.GetBroadcastExcludeList(excludeId), cancellationToken: cts.Token);

    /// <summary>
    /// Отправить одному участнику
    /// </summary>
    public Task SendToAsync<T>(T message, Guid recipientId)
        where T : class, INetworkMessage
        => bridge.SendMessage(message, RecipientId: recipientId, cancellationToken: cts.Token);

    /// <summary>
    /// Отправить сообщение об ошибке одному участнику
    /// </summary>
    public Task SendErrorAsync(Guid senderId, string text)
        => SendToAsync(new ErrorMessage { Message = text }, senderId);
}
