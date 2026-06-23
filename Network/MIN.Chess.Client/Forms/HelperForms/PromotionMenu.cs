using MIN.Chess.Client.Services;
using MIN.Chess.Logic.Enums;

namespace MIN.Chess.Client.Forms.HelperForms;

/// <summary>
/// Меню продвижения пешки
/// </summary>
public partial class PromotionMenu : Form
{
    /// <summary>
    /// Событие по нажатию на выбранную фигуру
    /// </summary>
    public Action<PieceType>? PieceSelected { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="PromotionMenu"/>
    /// </summary>
    public PromotionMenu(Player player)
    {
        InitializeComponent();

        QueenImg.Image = ImageHelper.GetImage(player, PieceType.Queen);
        RookImg.Image = ImageHelper.GetImage(player, PieceType.Rook);
        BishopImg.Image = ImageHelper.GetImage(player, PieceType.Bishop);
        KnightImg.Image = ImageHelper.GetImage(player, PieceType.Knight);
    }

    private void QueenImg_Click(object sender, EventArgs e)
    {
        PieceSelected?.Invoke(PieceType.Queen);
    }

    private void RookImg_Click(object sender, EventArgs e)
    {
        PieceSelected?.Invoke(PieceType.Rook);
    }

    private void BishopImg_Click(object sender, EventArgs e)
    {
        PieceSelected?.Invoke(PieceType.Bishop);
    }

    private void KnightImg_Click(object sender, EventArgs e)
    {
        PieceSelected?.Invoke(PieceType.Knight);
    }
}
