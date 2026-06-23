using MIN.Chess.Client.Contracts.Schemes;

namespace MIN.Chess.Client.Components.Labels;

/// <summary>
/// Кастомный <see cref="BaseLabel"/> для обычного текста
/// </summary>
public class PrimaryLabel : BaseLabel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PrimaryLabel"/>
    /// </summary>
    public PrimaryLabel()
    {
        ForeColor = ColorScheme.TextPrimary;
        Font = FontScheme.Default;
    }
}
