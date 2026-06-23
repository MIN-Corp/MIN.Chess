using MIN.Chess.Client.Contracts.Enums;
using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private GameOverMenu? gameOverMenu;
    private PauseMenu? pauseMenu;

    private void ShowGameOver(Result result, Player currentPlayer)
    {
        if (gameState == null)
        {
            return;
        }

        gameOverMenu = new GameOverMenu(role!.Value, result, currentPlayer, CountPlayers(), restartVote);
        gameOverMenu.OnOptionSelected += async option =>
        {
            if (option == Option.Restart)
            {
                await network.SendRestartAsync();
            }
            else
            {
                gameOverMenu.Close();
            }
        };

        gameOverMenu.ShowDialog();
        gameOverMenu = null;
    }


    private void ShowPauseMenu()
    {
        pauseMenu = new PauseMenu(CountPlayers(), restartVote);
        pauseMenu.OnOptionSelected += async option =>
        {
            if (option == Option.Restart)
            {
                await network.SendRestartAsync();
            }
            else
            {
                pauseMenu.Close();
            }
        };
        pauseMenu.ShowDialog();
        pauseMenu = null;
    }

    private void UpdateVoteAndPlayersForMenu(bool wasAboveZero)
    {
        restartQuote.Visible = restartVote >= 1;

        if (wasAboveZero)
        {
            gameOverMenu?.Close();
            pauseMenu?.Close();
            return;
        }

        gameOverMenu?.UpdateVoteDisplay(restartVote, CountPlayers());
        pauseMenu?.UpdateVoteDisplay(restartVote, CountPlayers());
    }

    private void UpdatePlayersInfo()
    {
        players.TryGetValue(role!.Value == Player.None ? Player.White : role!.Value, out var player1);

        selfRoleName.Text = player1 != null
            ? player1.Name
            : "Ожидаем оппонента...";

        players.TryGetValue((role!.Value == Player.None ? Player.White : role!.Value).Opponent(), out var player2);

        opponentName.Text = player2 != null
            ? player2.Name
            : "Ожидаем оппонента...";

        spectatorsCounter.Visible = spectatorsCount > 0;
        spectatorLogo.Visible = spectatorsCount > 0;

        tableLayoutPanelButtons.Visible = (role!.Value != Player.None && IsGameOn);

        if (spectatorsCount > 0)
        {
            spectatorsCounter.Text = spectatorsCount.ToString();
        }
    }

    private int CountPlayers()
    {
        var count = 0;

        if (players.TryGetValue(Player.White, out var white) && white != null)
        {
            count++;
        }

        if (players.TryGetValue(Player.Black, out var black) && black != null)
        {
            count++;
        }

        return count;
    }
}
