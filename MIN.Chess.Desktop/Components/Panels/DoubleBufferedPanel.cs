namespace MIN.Chess.Desktop.Components.Panels;

/// <summary>
/// Панель с двойный буфером
/// </summary>
public class DoubleBufferedPanel : Panel
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="DoubleBufferedPanel"/>
    /// </summary>
    public DoubleBufferedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}
