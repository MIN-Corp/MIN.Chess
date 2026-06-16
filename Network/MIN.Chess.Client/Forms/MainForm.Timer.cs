using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private readonly static int?[] timerMinutes = [null, 1, 3, 5, 10, 30];
    private readonly static int[] bonusSeconds = [0, 1, 2, 3, 5, 10, 20];

    private int timerIndex;
    private int bonusIndex;

    private long whiteMs;
    private long blackMs;
    private int timerBonusSeconds;
    private bool timerEnabled;
    private DateTime lastTimerSync;
    private Player? activeTimer;
    private System.Windows.Forms.Timer? countdownTimer;

    private void InitializeTimers()
    {
        timerTrackBar.Maximum = timerMinutes.Length - 1;
        bonusTrackBar.Maximum = bonusSeconds.Length - 1;
    }

    private void UpdateTimerChoosingDisplay()
    {
        var val = timerMinutes[timerIndex];
        timerValueLabel.Text = val.HasValue ? $"{val} мин" : "Без таймера";
    }

    private void UpdateBonusChoosingDisplay()
    {
        timerBonusValueLabel.Text = $"+{bonusSeconds[bonusIndex]} сек";
    }

    private void SyncTimerValues(long? white, long? black, Player currentTurn)
    {
        if (!white.HasValue || !IsGameOn)
        {
            countdownTimer?.Stop();
            if (white.HasValue)
            {
                whiteMs = white.Value;
                blackMs = black == null ? 0 : black.Value;
            }
            UpdateTimersToInitial(white, black);
            return;
        }

        whiteMs = white.Value;
        blackMs = black == null ? 0 : black.Value;
        lastTimerSync = DateTime.UtcNow;
        activeTimer = currentTurn;

        if (countdownTimer == null)
        {
            countdownTimer = new System.Windows.Forms.Timer { Interval = 250 };
            countdownTimer.Tick += (_, _) => UpdateTimerDisplay();
            countdownTimer.Start();
        }
        else if (!countdownTimer.Enabled)
        {
            countdownTimer.Start();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimersToInitial(long? white, long? black)
    {
        if (white.HasValue && black.HasValue)
        {
            selfTimerLabel.Text = FormatTime(white.Value);
            opponentTimerLabel.Text = FormatTime(black.Value);
        }
        else
        {
            selfTimerLabel.Text = string.Empty;
            opponentTimerLabel.Text = string.Empty;
        }
    }

    private void UpdateTimerDisplay()
    {
        var currentWhiteMs = whiteMs;
        var currentBlackMs = blackMs;
        var elapsed = (long)(DateTime.UtcNow - lastTimerSync).TotalMilliseconds;

        if (activeTimer == Player.White && timerEnabled)
        {
            currentWhiteMs = Math.Max(0, currentWhiteMs - elapsed);
        }
        else if (activeTimer == Player.Black && timerEnabled)
        {
            currentBlackMs = Math.Max(0, currentBlackMs - elapsed);
        }

        selfTimerLabel.Text = role == Player.Black ? FormatTime(currentBlackMs) : FormatTime(currentWhiteMs);
        opponentTimerLabel.Text = role == Player.Black ? FormatTime(currentWhiteMs) : FormatTime(currentBlackMs);
    }

    private static string FormatTime(long ms)
    {
        var t = TimeSpan.FromMilliseconds(ms);
        return t.TotalHours >= 1
            ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}"
            : $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    private void timerTrackBar_ValueChanged(object sender, EventArgs e)
    {
        timerIndex = timerTrackBar.Value;
        UpdateTimerChoosingDisplay();
    }

    private void bonusTrackBar_ValueChanged(object sender, EventArgs e)
    {
        bonusIndex = bonusTrackBar.Value;
        UpdateBonusChoosingDisplay();
    }

    private void buttonTimerPrevious_Click(object sender, EventArgs e)
    {
        timerTrackBar.Value = Math.Max(0, timerTrackBar.Value - 1);
    }

    private void buttonTimerNext_Click(object sender, EventArgs e)
    {
        timerTrackBar.Value = Math.Min(timerMinutes.Length - 1, timerTrackBar.Value + 1);
    }

    private void buttonBonusPrevious_Click(object sender, EventArgs e)
    {
        bonusTrackBar.Value = Math.Max(0, bonusTrackBar.Value - 1);
    }

    private void buttonBonusNext_Click(object sender, EventArgs e)
    {
        bonusTrackBar.Value = Math.Min(bonusSeconds.Length - 1, bonusTrackBar.Value + 1);
    }
}
