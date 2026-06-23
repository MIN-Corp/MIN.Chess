using MIN.Chess.Client.Contracts.Enums;

namespace MIN.Chess.Client.Forms.HelperForms;

/// <summary>
/// Меню окончания игры
/// </summary>
public partial class PauseMenu : Form
{
    /// <summary>
    /// Событие по нажатию на кнопку выбора
    /// </summary>
    public Action<Option>? OnOptionSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PauseMenu"/>
    /// </summary>
    public PauseMenu(int players, int votes)
    {
        InitializeComponent();

        restartButton.Text = $"Рестарт ({votes}/{players})";
    }

    /// <summary>
    /// Обновить значение голосов рестарта
    /// </summary>
    public void UpdateVoteDisplay(int newVotes, int newPlayers)
    {
        restartButton.Text = $"Рестарт ({newVotes}/{newPlayers})";
        if (newVotes == newPlayers)
        {
            Close();
        }
    }

    private void continueButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(Option.Continue);
    }

    private void restartButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(Option.Restart);
    }
}
