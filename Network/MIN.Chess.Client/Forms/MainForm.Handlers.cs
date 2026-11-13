using MIN.Chess.Client.Contracts.Enums;
using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Logic.Extensions;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private void MainForm_Resize(object sender, EventArgs e)
    {
        CalculateCellRectangles();
        mainPanel.Invalidate();
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            ShowPauseMenu();
        }
    }

    private void pauseButton_Click(object sender, EventArgs e)
    {
        ShowPauseMenu();
    }

    private async void updateStateButton_Click(object sender, EventArgs e)
    {
        await network.SendStateUpdateRequestAsync();
    }

    private async void restartQuote_Click(object sender, EventArgs e)
    {
        await network.SendRestartAsync();
    }

    private async void switchSidesQuote_Click(object sender, EventArgs e)
    {
        await network.SendSwitchSidesAsync();
    }

    private async void changeRoleButton_Click(object sender, EventArgs e)
    {
        await network.SendChangeRoleAsync();
    }

    private async void switchSidesButton_Click(object sender, EventArgs e)
    {
        await network.SendSwitchSidesAsync();
    }

    private async void resignButton_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Вы точно хотите сдаться?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            await network.SendResignAsync();
        }
    }

    private async void drawButton_Click(object sender, EventArgs e)
    {
        await network.SendDrawAsync();
    }

    private async void buttonSaveSettings_Click(object sender, EventArgs e)
    {
        await network.SendGameSettingsAsync(timerMinutes[timerIndex], bonusSeconds[bonusIndex]);
    }

    private async void buttonChooseAi_Click(object sender, EventArgs e)
    {
        if (playerAiState == PlayerAiState.BothAi)
        {
            await network.SendStartGameAsync();
            return;
        }

        var chose = new AgentChoosingMenu(players[role!.Value.Opponent()]?.AgentType);
        chose.OnOptionSelected += async void (type, asSelf) =>
        {
            if (type == null)
            {
                await network.SendAgentRemoveAsync();
                return;
            }

            var opponent = players[role!.Value.Opponent()];

            if (asSelf)
            {
                await network.SendAgentVsAgentAsync(type.Value, opponent!.AgentType!.Value);
                return;
            }

            if (playerAiState == PlayerAiState.Alone)
            {
                await network.SendAgentChooseAsync(type.Value);
            }
            else if (playerAiState == PlayerAiState.WithAi)
            {
                await network.SendAgentChangeAsync(type.Value);
            }
        };
        chose.ShowDialog();
    }
}
