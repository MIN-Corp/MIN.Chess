using MIN.Chess.Client.Services;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Client.Forms;

/// <summary>
/// Главная форма
/// </summary>
public partial class MainForm : Form
{
    private readonly SynchronizationContext uiContext;
    private readonly NetworkService network;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainForm"/>
    /// </summary>
    public MainForm(IClientProcessBridge bridge, CancellationTokenSource appCts, string connectionString)
    {
        network = new(bridge, appCts.Token);
        network.CloseRequested += Close;

        uiContext = SynchronizationContext.Current
            ?? new SynchronizationContext();

        InitializeComponent();
        ApplyStylings();
        InitializeTimers();
        InitializeNetworking(connectionString);
    }

    private void ApplyStylings()
    {
        resignButton.BackColor = Color.Black;
        drawButton.BackColor = Color.Black;
        pauseButton.BackColor = Color.Black;

        UpdateTimerChoosingDisplay();
        UpdateBonusChoosingDisplay();
        CalculateCellRectangles();
    }
}
