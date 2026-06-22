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

    private async void changeRoleButton_Click(object sender, EventArgs e)
    {
        await network.SendChangeRoleAsync();
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
}
