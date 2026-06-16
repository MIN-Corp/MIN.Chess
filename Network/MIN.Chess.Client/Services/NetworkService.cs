using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Client.Services;

/// <summary>
/// Сервис для общения с сетью MIN в качестве клиента
/// </summary>
internal class NetworkService
{
    private const int ConnectionTimeoutMs = 5_000;

    private readonly IClientProcessBridge bridge;
    private readonly CancellationToken cancellationToken;

    public event Action<INetworkMessage>? MessageReceived;
    public event Action<string>? ConnectionFailed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NetworkService"/>
    /// </summary>
    public NetworkService(IClientProcessBridge bridge, CancellationToken cancellationToken)
    {
        this.bridge = bridge;
        this.cancellationToken = cancellationToken;
    }

    public async Task ConnectAsync(string connectionString)
    {
        try
        {
            bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
            await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, cancellationToken);
            bridge.NetworkMessageReceived += (_, m) => MessageReceived?.Invoke(m);
            await bridge.StartListeningAsync(cancellationToken);
            await bridge.SendReadyMessage(cancellationToken);
        }
        catch (Exception ex) { ConnectionFailed?.Invoke(ex.Message); }
    }

    /// <summary>
    /// Отправить запрос на первоначальное сообщение
    /// </summary>
    public Task SendInitAsync() => bridge.SendMessage(new InitRequestMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить запрос на ход
    /// </summary>
    public Task SendMoveAsync(string uci) => bridge.SendMessage(new MoveRequestMessage { MoveUciNotation = uci }, cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить запрос на сдачу
    /// </summary>
    public Task SendResignAsync() => bridge.SendMessage(new ResignRequestMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить запрос на ничью
    /// </summary>
    public Task SendDrawAsync() => bridge.SendMessage(new DrawRequestMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить запрос на рестарт
    /// </summary>
    public Task SendRestartAsync() => bridge.SendMessage(new RestartRequestMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить запрос на смену роли
    /// </summary>
    public Task SendChangeRoleAsync() => bridge.SendMessage(new ChangeRoleRequestMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить оповещение о загрузке
    /// </summary>
    public Task SendPlayerLoadedAsync() => bridge.SendMessage(new PlayerLoadedMessage(), cancellationToken: cancellationToken);

    /// <summary>
    /// Отправить настройки
    /// </summary>
    public Task SendGameSettingsAsync(int? timerMin, int bonusSec) => bridge.SendMessage(
        new GameSettingsMessage { TimerInitialMinutes = timerMin, TimerBonusSeconds = bonusSec },
        cancellationToken: cancellationToken);
}
