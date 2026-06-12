using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Logic.Extensions;

/// <summary>
/// Расширения для <see cref="Player"/>
/// </summary>
public static class PlayerExtensions
{
    /// <summary>
    /// Оппонент
    /// </summary>
    public static Player Opponent(this Player player)
        => player switch
        {
            Player.White => Player.Black,
            Player.Black => Player.White,
            _ => Player.None,
        };
}
