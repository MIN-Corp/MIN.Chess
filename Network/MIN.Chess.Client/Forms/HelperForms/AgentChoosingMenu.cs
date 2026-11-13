using MIN.Chess.Agents.Enums;

namespace MIN.Chess.Client.Forms.HelperForms;

/// <summary>
/// Меню выбора агента для игры
/// </summary>
public partial class AgentChoosingMenu : Form
{
    /// <summary>
    /// Событие по нажатию на кнопку выбора агента
    /// </summary>
    public Action<AgentType?, bool>? OnOptionSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AgentChoosingMenu"/>
    /// </summary>
    public AgentChoosingMenu(AgentType? existing = null)
    {
        InitializeComponent();
        buttonRemoveCurrent.Visible = existing != null;
        checkBoxAsSelf.Visible = existing != null;
        primaryLabel1.Visible = existing != null;
    }

    private void randomAgentButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(AgentType.Random, checkBoxAsSelf.Checked);
        Close();
    }

    private void commonButton1_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(AgentType.Smart, checkBoxAsSelf.Checked);
        Close();
    }

    private void buttonRemoveCurrent_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(null, false);
        Close();
    }
}
