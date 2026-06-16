using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния таймера
/// </summary>
internal class TimerService
{
    private readonly MessageService messageService;
    private readonly RoomService roomService;
    private readonly ILoggerProvider logger;
    private readonly CancellationTokenSource appCts;
    private readonly List<int> avaibleTimerValues = [1, 3, 5, 10, 30];
    private readonly List<int> avaibleBonusTimerValues = [0, 1, 2, 3, 5, 10, 20];

    private long whiteRemainingMs;
    private long blackRemainingMs;
    private Player? activeTimer;
    private DateTime turnStartTime;
    private int timerInitialMs;
    private int timerBonusMs;
    private Task? timerTask;
    private CancellationTokenSource? timerCts;
    private readonly object timerLock = new();

    /// <summary>
    /// Включён ли таймер в игре
    /// </summary>
    public bool TimerEnabled;

    /// <summary>
    /// Событие по смене настроек
    /// </summary>
    public Action? OnSettingsChanged;

    /// <summary>
    /// Событие при истечении времени игрока
    /// </summary>
    public Action<Player>? OnTimeOut;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TimerService"/>
    /// </summary>
    public TimerService(MessageService messageService,
        RoomService roomService,
        ILoggerProvider logger,
        CancellationTokenSource appCts)
    {
        this.messageService = messageService;
        this.roomService = roomService;
        this.logger = logger;
        this.appCts = appCts;
    }

    /// <summary>
    /// Получить добавку по времени
    /// </summary>
    public int TimerBonusMs => timerBonusMs;

    /// <summary>
    /// Обработать настройки партии
    /// </summary>
    public async Task HandleGameSettings(GameSettingsMessage gameSettingsMessage)
    {
        var senderId = Guid.Parse(gameSettingsMessage.SenderId);
        var role = roomService.ResolveRole(senderId);

        logger.Log($"Settings received from {role}");

        if (role != Player.White)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (GameService.GameStarted)
        {
            await messageService.SendErrorAsync(senderId, "Настройки можно изменить только до начала игры");
            return;
        }

        if (gameSettingsMessage.TimerInitialMinutes != null
            && !avaibleTimerValues.Contains(gameSettingsMessage.TimerInitialMinutes.Value))
        {
            await messageService.SendErrorAsync(senderId, "Время должно находиться в правильном диапазоне");
            return;
        }

        if (!avaibleBonusTimerValues.Contains(gameSettingsMessage.TimerBonusSeconds))
        {
            await messageService.SendErrorAsync(senderId, "Добавочное время должно находиться в правильном диапазоне");
            return;
        }

        TimerEnabled = gameSettingsMessage.TimerInitialMinutes.HasValue;
        timerInitialMs = (gameSettingsMessage.TimerInitialMinutes ?? 0) * 60 * 1000;
        timerBonusMs = gameSettingsMessage.TimerBonusSeconds * 1000;
        whiteRemainingMs = timerInitialMs;
        blackRemainingMs = timerInitialMs;

        logger.Log($"Timer settings stored: enabled={TimerEnabled}, initial={timerInitialMs}ms, bonus={timerBonusMs}ms");

        if (roomService.IsBothPlayersJoined())
        {
            OnSettingsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Получить оставшееся время для игроков
    /// </summary>
    public (long whiteMs, long blackMs) GetCurrentRemainingMs()
    {
        if (!TimerEnabled)
        {
            return (0, 0);
        }

        lock (timerLock)
        {
            var white = whiteRemainingMs;
            var black = blackRemainingMs;
            var elapsed = (long)(DateTime.UtcNow - turnStartTime).TotalMilliseconds;

            if (activeTimer == Player.White)
            {
                white = Math.Max(0, white - elapsed);
            }
            else if (activeTimer == Player.Black)
            {
                black = Math.Max(0, black - elapsed);
            }

            return (white, black);
        }
    }

    /// <summary>
    /// Обработать время после хода
    /// </summary>
    public bool ManageTimeAfterMove(Player role, bool stateGameStarted)
    {
        var gameStarted = false;

        if (TimerEnabled)
        {
            lock (timerLock)
            {
                var now = DateTime.UtcNow;

                if (!stateGameStarted)
                {
                    gameStarted = true;

                    if (role == Player.White)
                    {
                        whiteRemainingMs = timerInitialMs + timerBonusMs;
                        blackRemainingMs = timerInitialMs;
                    }
                    else
                    {
                        blackRemainingMs = timerInitialMs + timerBonusMs;
                        whiteRemainingMs = timerInitialMs;
                    }

                    activeTimer = role.Opponent();
                    turnStartTime = now;
                    StartTimerTask();
                }
                else
                {
                    var elapsed = (long)(now - turnStartTime).TotalMilliseconds;

                    if (role == Player.White)
                    {
                        whiteRemainingMs = Math.Max(0, whiteRemainingMs - elapsed);
                        whiteRemainingMs += timerBonusMs;
                    }
                    else
                    {
                        blackRemainingMs = Math.Max(0, blackRemainingMs - elapsed);
                        blackRemainingMs += timerBonusMs;
                    }

                    activeTimer = role.Opponent();
                    turnStartTime = now;
                }
            }
        }

        return gameStarted;
    }

    /// <summary>
    /// Остановить все таймеры
    /// </summary>
    public void StopTimer()
    {
        timerCts?.Cancel();
        timerCts?.Dispose();
        timerCts = null;
        timerTask = null;
        activeTimer = null;
        whiteRemainingMs = 0;
        blackRemainingMs = 0;
    }

    private void StartTimerTask()
    {
        if (timerTask != null)
        {
            return;
        }
        timerCts = CancellationTokenSource.CreateLinkedTokenSource(appCts.Token);
        timerTask = TimerCheckLoop(timerCts.Token);
    }

    private async Task TimerCheckLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(500, ct);

                Player? timeoutLoser = null;

                lock (timerLock)
                {
                    if (!TimerEnabled || activeTimer == null || GameService.GameState.IsGameOver())
                    {
                        continue;
                    }

                    var elapsed = (long)(DateTime.UtcNow - turnStartTime).TotalMilliseconds;

                    if (activeTimer == Player.White && whiteRemainingMs - elapsed <= 0)
                    {
                        timeoutLoser = Player.White;
                    }
                    else if (activeTimer == Player.Black && blackRemainingMs - elapsed <= 0)
                    {
                        timeoutLoser = Player.Black;
                    }
                }

                if (timeoutLoser != null)
                {
                    logger.Log($"{timeoutLoser} timed out");
                    OnTimeOut?.Invoke(timeoutLoser.Value);
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
