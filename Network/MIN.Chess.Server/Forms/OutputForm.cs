using MIN.Chess.Server.Helpers;
using MIN.Chess.Server.Services;

namespace MIN.Chess.Server.Forms;

/// <summary>
/// Форма для вывода данных из сервера
/// </summary>
public partial class OutputForm : Form, ILoggerProvider
{
    private readonly string connectionString;
    private GameServerService gameServerService = null!;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="OutputForm"/>
    /// </summary>
    public OutputForm(string connectionString)
    {
        this.connectionString = connectionString;
        InitializeComponent();
    }

    void ILoggerProvider.Log(string message) => log.Items.Add(message);

    private async void OutputForm_Load(object sender, EventArgs e)
    {
        gameServerService = new(this);
        await gameServerService.InitializeNetworking(connectionString);
    }
}
