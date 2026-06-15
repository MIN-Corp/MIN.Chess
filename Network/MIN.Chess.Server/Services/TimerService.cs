using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния таймера
/// </summary>
internal class TimerService
{
    private readonly ILoggerProvider logger;
    private readonly MessageService messageService;
    private readonly List<int> avaibleTimerValues = [1, 3, 5, 10, 30];
    private readonly List<int> avaibleBonusTimerValues = [0, 1, 2, 3, 5, 10, 20];

    private long whiteRemainingMs;
    private long blackRemainingMs;
    private Player? activeTimer;
    private DateTime turnStartTime;
    private int timerInitialMs;
    private int timerBonusMs;
    private bool timerEnabled;
    private bool gameStarted;
    private Task? timerTask;
    private CancellationTokenSource? timerCts;
    private readonly object timerLock = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TimerService"/>
    /// </summary>
    public TimerService(ILoggerProvider logger, MessageService messageService)
    {
        this.logger = logger;
        this.messageService = messageService;
    }

    /// <summary>
    /// Обработать настройки партии
    /// </summary>
    public async Task HandleGameSettings(GameSettingsMessage gameSettingsMessage)
    {
        var senderId = Guid.Parse(gameSettingsMessage.SenderId);
        var role = ResolveRole(senderId);

        logger.Log($"Settings received from {role}");

        if (role != Player.White)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (gameStarted)
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

        timerEnabled = gameSettingsMessage.TimerInitialMinutes.HasValue;
        timerInitialMs = (gameSettingsMessage.TimerInitialMinutes ?? 0) * 60 * 1000;
        timerBonusMs = gameSettingsMessage.TimerBonusSeconds * 1000;
        whiteRemainingMs = timerInitialMs;
        blackRemainingMs = timerInitialMs;

        logger.Log($"Timer settings stored: enabled={timerEnabled}, initial={timerInitialMs}ms, bonus={timerBonusMs}ms");

        if (IsBothPlayersJoined())
        {
            await SendStateUpdate();
        }
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

    private void StopTimer()
    {
        timerCts?.Cancel();
        timerCts?.Dispose();
        timerCts = null;
        timerTask = null;
        activeTimer = null;
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
                    if (!timerEnabled || activeTimer == null || gameState.IsGameOver())
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
                    var result = new Result(timeoutLoser.Value.Opponent(), EndReason.Timeout);
                    gameState.Result = result;
                    await SendGameResult(result);
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private (long whiteMs, long blackMs) GetCurrentRemainingMs()
    {
        if (!timerEnabled)
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
}
