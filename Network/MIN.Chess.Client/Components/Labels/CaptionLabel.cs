using MIN.Chess.Client.Contracts.Schemes;

namespace MIN.Chess.Client.Components.Labels;

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
