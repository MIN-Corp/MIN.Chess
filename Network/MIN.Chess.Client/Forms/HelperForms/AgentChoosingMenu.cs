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
    public Action<AgentType>? OnOptionSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AgentChoosingMenu"/>
    /// </summary>
    public AgentChoosingMenu()
    {
        InitializeComponent();
    }

    private void randomAgentButton_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(AgentType.Random);
        Close();
    }

    private void commonButton1_Click(object sender, EventArgs e)
    {
        OnOptionSelected?.Invoke(AgentType.Smart);
        Close();
    }
}
