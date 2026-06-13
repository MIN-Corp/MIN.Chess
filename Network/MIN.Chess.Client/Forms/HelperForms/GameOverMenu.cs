using MIN.Chess.Client.Contracts.Enums;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Client.Forms.HelperForms;

/// <summary>
/// Меню окончания игры
/// </summary>
public partial class GameOverMenu : Form
{
    /// <summary>
    /// Событие по нажатию на кнопку выбора
    /// </summary>
    public Action<Option>? OnOptionSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameOverMenu"/>
    /// </summary>
    public GameOverMenu(Player role, Result result, Player currentPlayer, int players, int votes)
    {
        InitializeComponent();

        if (role == Player.None)
        {
            restartButton.Visible = false;
            exitButton.Visible = false;
        }

        outcomeTitle.Text = GetWinnerText(result.Winner);
        reason.Text = GetReasonText(result, currentPlayer);
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

    private static string GetWinnerText(Player player)
        => player switch
        {
            Player.White => "БЕЛЫЙ ПОБЕДИЛ!",
            Player.Black => "ЧЁРНЫЙ ПОБЕДИЛ!",
            _ => "НИЧЬЯ!"
        };

    private static string GetPlayerString(Player player)
        => player switch
        {
            Player.White => "Белый игрок",
            Player.Black => "Чёрный игрок",
            _ => ""
        };

    private static string GetReasonText(Result result, Player currentPlayer)
    => result.Reason switch
    {
        EndReason.CheckMate => $"Мат - {GetPlayerString(currentPlayer)} не может ходить",
        EndReason.Resign => $"{GetPlayerString(result.Winner.Opponent())} сдался",
        EndReason.StaleMate => $"Ничья - {GetPlayerString(currentPlayer)} не может ходить",
        EndReason.FiftyMoveRule => "Правило пятидесяти ходов",
        EndReason.InsufficientMaterial => "У обоих сторон не хватает фигур",
        EndReason.ThreefoldRepetition => "Трёхкратное повторение",
        _ => "хз почему"
    };

    private void restartButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(Option.Restart);
    }

    private void exitButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(Option.Close);
    }
}
