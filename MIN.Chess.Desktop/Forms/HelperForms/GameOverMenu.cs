using MIN.Chess.Desktop.Contracts.Enums;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Desktop.Forms.HelperForms;

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
    public GameOverMenu(GameState gameState)
    {
        InitializeComponent();

        var result = gameState.Result!.Value;

        outcomeTitle.Text = GetWinnerText(result.Winner);
        reason.Text = GetReasonText(result.Reason, gameState.CurrentPlayer);
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

    private static string GetReasonText(EndReason reason, Player currentPlayer)
    => reason switch
    {
        EndReason.CheckMate => $"Мат - {GetPlayerString(currentPlayer)} не может ходить",
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
        OnOptionSelected?.Invoke(Option.Exit);
    }
}
