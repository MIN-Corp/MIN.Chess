using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Ipc.Services.Contracts.Interfaces;

/// <summary>
/// Мост для общение с MIN
/// </summary>
public interface IClientProcessBridge
{
    /// <summary>
    /// Начать прослушивание сообщений
    /// </summary>
    Task StartListeningAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Подключиться к MIN по connection string
    /// </summary>
    Task ConnectAsync(string connectionString, int timeOutMs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Зарегистрировать одно сообщение по его типу
    /// </summary>
    void RegisterMessage<T>(string? typeTag = null)
        where T : class, INetworkMessage;

    /// <summary>
    /// Зарегистрировать все сообщения из сборки
    /// </summary>
    void RegisterAllFromAnchor<TAnchor>();

    /// <summary>
    /// Получено междупроцессорное сообщение
    /// </summary>
    event EventHandler<INetworkMessage>? NetworkMessageReceived;

    /// <summary>
    /// Присоединился участник
    /// </summary>
    event EventHandler<Participant>? ParticipantConnected;

    /// <summary>
    /// Отсоединился участник
    /// </summary>
    event EventHandler<Guid>? ParticipantDisconnected;

    /// <summary>
    /// Поступила просьба закрыть приложение
    /// </summary>
    event EventHandler? CloseRequested;

    /// <summary>
    /// Отправить сообщение по сети
    /// </summary>
    /// <typeparam name="T">Какой тип сообщения</typeparam>
    /// <param name="message">само сообщение</param>
    /// <param name="msgType">тип сообщения</param>
    /// <param name="route">По какому пути пойдёт сообщение (сразу всем или через сервер приложения)</param>
    /// <param name="channel">По какому каналу пойдёт сообщение (Secure - tcp, Fast - udp)</param>
    /// <param name="recipientId">Кто получатель (null - если broadcats)</param>
    /// <param name="broadcastExcludeIds">Кого исключить из широковещательного сообщения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendMessage<T>(T message, string? msgType = null, SessionMessageRoute route = SessionMessageRoute.ViaServer,
        MessageChannel channel = MessageChannel.Secure, Guid? recipientId = null,
        IEnumerable<Guid>? broadcastExcludeIds = null, CancellationToken cancellationToken = default)
        where T : class, INetworkMessage;

    /// <summary>
    /// Отправить сообщение о готовности приложения
    /// </summary>
    Task SendReadyMessage(CancellationToken cancellationToken = default);

    /// <summary>
    /// Остоновить прослушивание сообщений
    /// </summary>
    Task StopListeningAsync(CancellationToken cancellationToken);
}
