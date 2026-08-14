using MIN.Chess.Messages.Contracts.Models;
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
        where T : BaseNetworkMessage
        => SendAsync(message, broadcastExcludeIds: RoomService.GetBroadcastExcludeList());

    /// <summary>
    /// Разослать сообщение всем участникам, исключая одного
    /// </summary>
    public Task BroadcastExcludingAsync<T>(T message, Guid? excludeId)
        where T : BaseNetworkMessage
        => SendAsync(message, broadcastExcludeIds: RoomService.GetBroadcastExcludeList(excludeId));

    /// <summary>
    /// Отправить одному участнику
    /// </summary>
    public Task SendToAsync<T>(T message, Guid recipientId)
        where T : BaseNetworkMessage
        => SendAsync(message, recipientId: recipientId);

    /// <summary>
    /// Отправить сообщение об ошибке одному участнику
    /// </summary>
    public Task SendErrorAsync(Guid senderId, string text)
        => SendToAsync(new ErrorMessage { Message = text }, senderId);

    private Task SendAsync(BaseNetworkMessage message, Guid? recipientId = null, IEnumerable<Guid>? broadcastExcludeIds = null)
        => bridge.SendMessage(message, route: message.Route, channel: message.Channel, recipientId: recipientId, broadcastExcludeIds: broadcastExcludeIds, cancellationToken: cts.Token);
}
