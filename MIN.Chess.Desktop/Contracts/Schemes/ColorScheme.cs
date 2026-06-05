namespace MIN.Chess.Desktop.Contracts.Schemes;

/// <summary>
/// Цветовая схема приложения
/// </summary>
public static class ColorScheme
{
    #region Main backgrounds

    /// <summary>
    /// Цвет заднего фона приложения (основной фон)
    /// </summary>
    public readonly static Color FormBackground = Color.Black;

    #endregion

    #region Buttons

    /// <summary>
    /// Основной акцентный цвет (фиолетовый — для кнопок, выделений, активных элементов)
    /// </summary>
    public readonly static Color PrimaryAccent = ColorTranslator.FromHtml("#6A5BFF");

    /// <summary>
    /// Вторичный акцентный цвет (светло-фиолетовый — для ховеров, второстепенных действий)
    /// </summary>
    public readonly static Color SecondaryAccent = ColorTranslator.FromHtml("#A79DFF");

    #endregion

    #region Text colors

    /// <summary>
    /// Цвет текста по умолчанию (основной контент)
    /// </summary>
    public readonly static Color TextPrimary = ColorTranslator.FromHtml("#2D2B3A");

    /// <summary>
    /// Цвет второстепенного текста (время, метаданные, подписи)
    /// </summary>
    public readonly static Color TextSecondary = ColorTranslator.FromHtml("#7A778F");

    /// <summary>
    /// Цвет текста на акцентном фоне (например, белый текст на кнопке)
    /// </summary>
    public readonly static Color TextOnAccent = ColorTranslator.FromHtml("#FFFFFF");

    #endregion

    #region Other

    /// <summary>
    /// Цвет разделителей и лёгких границ
    /// </summary>
    public readonly static Color DividerColor = ColorTranslator.FromHtml("#E4E6F0");

    /// <summary>
    /// Цвет фона поля ввода сообщения
    /// </summary>
    public readonly static Color InputFieldBackground = ColorTranslator.FromHtml("#F8F9FF");

    /// <summary>
    /// Цвет ошибки или предупреждения (например, недоставленное сообщение)
    /// </summary>
    public readonly static Color ErrorColor = ColorTranslator.FromHtml("#FF6B6B");

    /// <summary>
    /// Невидимый цвет (цвет родителя)
    /// </summary>
    public readonly static Color Transparent = Color.Transparent;

    #endregion
}
