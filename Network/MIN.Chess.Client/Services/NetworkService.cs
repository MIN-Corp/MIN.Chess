using MIN.Chess.Messages;
using MIN.Chess.Messages.Contracts.Models;
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
    /// Поступила просьба закрыть приложение
    /// </summary>
    public event Action? CloseRequested;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NetworkService"/>
    /// </summary>
    public NetworkService(IClientProcessBridge bridge, CancellationToken cancellationToken)
    {
        this.bridge = bridge;
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Подключиться к MIN и отправить сообщение о готовности
    /// </summary>
    public async Task ConnectAsync(string connectionString)
    {
        try
        {
            bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
            await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, cancellationToken);
            bridge.NetworkMessageReceived += (_, m) => MessageReceived?.Invoke(m);
            bridge.CloseRequested += (_, _) => CloseRequested?.Invoke();
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
    public Task SendMoveAsync(string uci) => SendMessage(new MoveRequestMessage { MoveUciNotation = uci });

    /// <summary>
    /// Отправить запрос на сдачу
    /// </summary>
    public Task SendResignAsync() => SendMessage(new ResignRequestMessage());

    /// <summary>
    /// Отправить запрос на ничью
    /// </summary>
    public Task SendDrawAsync() => SendMessage(new DrawRequestMessage());

    /// <summary>
    /// Отправить запрос на рестарт
    /// </summary>
    public Task SendRestartAsync() => SendMessage(new RestartRequestMessage());

    /// <summary>
    /// Отправить запрос на смену роли
    /// </summary>
    public Task SendChangeRoleAsync() => SendMessage(new ChangeRoleRequestMessage());

    /// <summary>
    /// Отправить оповещение о загрузке
    /// </summary>
    public Task SendPlayerLoadedAsync() => SendMessage(new PlayerLoadedMessage());

    /// <summary>
    /// Отправить настройки
    /// </summary>
    public Task SendGameSettingsAsync(int? timerMin, int bonusSec) => bridge.SendMessage(
        new GameSettingsMessage { TimerInitialMinutes = timerMin, TimerBonusSeconds = bonusSec },
        cancellationToken: cancellationToken);

    private async Task SendMessage(BaseNetworkMessage message)
    {
        try
        {
            await bridge.SendMessage(message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка при отправке сообщения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
