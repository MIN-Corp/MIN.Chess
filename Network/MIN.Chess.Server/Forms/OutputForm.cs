using MIN.Chess.Server.Helpers;
using MIN.Chess.Server.Properties;
using MIN.Chess.Server.Services;

namespace MIN.Chess.Server.Forms;

/// <summary>
/// Форма для вывода данных из сервера
/// </summary>
public partial class OutputForm : Form, ILoggerProvider
{
    private readonly string connectionString;
    private NotifyIcon? trayIcon;
    private GameServerService gameServerService = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="OutputForm"/>
    /// </summary>
    public OutputForm(string connectionString)
    {
        this.connectionString = connectionString;
        InitializeComponent();
    }

    private void SetupTrayIcon()
    {
        trayIcon = new NotifyIcon
        {
            Icon = Resources.chesslogo,
            Text = "Сервер шахмат",
            Visible = true
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Показать логи", null, (s, e) => WindowState = FormWindowState.Normal);
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Закрыть", null, (s, e) => Close());

        trayIcon.ContextMenuStrip = contextMenu;
        trayIcon.DoubleClick += (s, e) => WindowState = FormWindowState.Normal;
    }

    void ILoggerProvider.Log(string message) => log.Items.Add(message);

    private async void OutputForm_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        SetupTrayIcon();

        gameServerService = new(this);
        gameServerService.CloseRequested += Close;
        await gameServerService.InitializeNetworking(connectionString);
    }

    private void OutputForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        gameServerService.StopService();
        trayIcon?.Visible = false;
        trayIcon?.Dispose();
    }
}
