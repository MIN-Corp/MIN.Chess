using MIN.Chess.Desktop.Contracts.Schemes;

namespace MIN.Chess.Desktop.Components.Labels;

/// <summary>
/// Кастомный <see cref="BaseLabel"/> для текста описаний
/// </summary>
public class CaptionLabel : BaseLabel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CaptionLabel"/>
    /// </summary>
    public CaptionLabel()
    {
        ForeColor = ColorScheme.FormBackground;
        Font = FontScheme.Caption;
    }
}
