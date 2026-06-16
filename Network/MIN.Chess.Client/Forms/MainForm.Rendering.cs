using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Client.Services;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private readonly Dictionary<Position, Move> moveCache = [];
    private readonly static Dictionary<PieceType, int> startPieces = new()
    {
        [PieceType.Pawn] = 8,
        [PieceType.Knight] = 2,
        [PieceType.Bishop] = 2,
        [PieceType.Rook] = 2,
        [PieceType.Queen] = 1,
        [PieceType.King] = 1,
    };

    private Rectangle[,] cellRectangles = null!;
    private Move? pendingPromotionMove;
    private Position? selectedPos;

    private void CalculateCellRectangles()
    {
        if (mainPanel.ClientSize.Width == 0 || mainPanel.ClientSize.Height == 0)
        {
            return;
        }

        var boardImage = mainPanel.BackgroundImage;
        if (boardImage == null)
        {
            return;
        }

        var panelSize = mainPanel.ClientSize;
        var imageSize = boardImage.Size;

        var scaleX = (double)panelSize.Width / imageSize.Width;
        var scaleY = (double)panelSize.Height / imageSize.Height;
        var scale = Math.Min(scaleX, scaleY);

        var drawWidth = (int)(imageSize.Width * scale);
        var drawHeight = (int)(imageSize.Height * scale);
        var drawX = (panelSize.Width - drawWidth) / 2;
        var drawY = (panelSize.Height - drawHeight) / 2;

        var cellSize = Math.Min(drawWidth, drawHeight) / 8;
        var offsetX = drawX + (drawWidth - cellSize * 8) / 2;
        var offsetY = drawY + (drawHeight - cellSize * 8) / 2;

        cellRectangles = new Rectangle[8, 8];
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                var x = offsetX + col * cellSize;
                var y = offsetY + row * cellSize;
                var r = role == Player.Black ? 7 - row : row;
                var c = role == Player.Black ? 7 - col : col;
                cellRectangles[r, c] = new Rectangle(x, y, cellSize, cellSize);
            }
        }
    }

    private void MainPanel_Paint(object? sender, PaintEventArgs e)
    {
        DrawHighlights(e.Graphics);
        DrawPieces(e.Graphics);
    }

    private void DrawPieces(Graphics g)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        if (gameState == null)
        {
            return;
        }

        var board = gameState.Board;
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                var piece = board[row, col];
                if (piece == null)
                {
                    continue;
                }

                if (pendingPromotionMove != null &&
                    (row == pendingPromotionMove.FromPos.Row && col == pendingPromotionMove.FromPos.Column
                    || row == pendingPromotionMove.ToPos.Row && col == pendingPromotionMove.ToPos.Column))
                {
                    continue;
                }

                var pieceImage = ImageHelper.GetImage(piece);
                if (pieceImage == null)
                {
                    continue;
                }

                var rect = cellRectangles[row, col];
                g.DrawImage(pieceImage, rect);
            }
        }

        if (pendingPromotionMove != null)
        {
            var movingPiece = gameState.Board[pendingPromotionMove.FromPos.Row, pendingPromotionMove.FromPos.Column];
            if (movingPiece != null)
            {
                var pieceImage = ImageHelper.GetImage(movingPiece);
                if (pieceImage != null)
                {
                    var rect = cellRectangles[pendingPromotionMove.ToPos.Row, pendingPromotionMove.ToPos.Column];
                    g.DrawImage(pieceImage, rect);
                }
            }
        }
    }

    private void DrawHighlights(Graphics g)
    {
        if (selectedPos != null)
        {
            using var brush = new SolidBrush(Color.FromArgb(100, 0, 255, 0));
            foreach (Position pos in moveCache.Keys)
            {
                g.FillRectangle(brush, cellRectangles[pos.Row, pos.Column]);
            }
        }

        if (lastMove != null)
        {
            using var brush = new SolidBrush(Color.FromArgb(150, 233, 215, 0));
            g.FillRectangle(brush, cellRectangles[lastMove.FromPos.Row, lastMove.FromPos.Column]);
            g.FillRectangle(brush, cellRectangles[lastMove.ToPos.Row, lastMove.ToPos.Column]);
        }
    }

    private void MainPanel_MouseClick(object? sender, MouseEventArgs e)
    {
        var pos = GetPositionFromPoint(e.Location);
        if (pos == null)
        {
            return;
        }

        if (selectedPos == null)
        {
            OnFromPositionSelected(pos.Value);
        }
        else
        {
            OnToPositionSelected(pos.Value);
        }
    }

    private Position? GetPositionFromPoint(Point point)
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                if (cellRectangles[row, col].Contains(point))
                {
                    return new Position(row, col);
                }
            }
        }
        return null;
    }

    private void OnFromPositionSelected(Position position)
    {
        if (gameState == null)
        {
            return;
        }

        var moves = gameState.LegalMovesForPiece(position, role);
        if (moves.Any())
        {
            selectedPos = position;
            moveCache.Clear();
            foreach (var move in moves)
            {
                moveCache[move.ToPos] = move;
            }

            mainPanel.Invalidate();
        }
    }

    private void OnToPositionSelected(Position position)
    {
        selectedPos = null;

        if (moveCache.TryGetValue(position, out var move))
        {
            if (move.Type == MoveType.PawnPromotion)
            {
                HandlePromotion(move.FromPos, move.ToPos);
            }
            else
            {
                HandleMove(move);
            }
        }

        moveCache.Clear();
        mainPanel.Invalidate();
    }

    private void UpdateTurnColors()
    {
        var isMyTurn = gameState!.CurrentPlayer == role;

        tableLayoutPanelOpponent.BackColor = isMyTurn ? Color.Black : Color.White;
        opponentName.ForeColor = isMyTurn ? Color.White : Color.Black;
        tableLayoutPanelSelf.BackColor = isMyTurn ? Color.White : Color.Black;
        selfRoleName.ForeColor = isMyTurn ? Color.Black : Color.White;
        opponentTimerLabel.ForeColor = opponentName.ForeColor;
        selfTimerLabel.ForeColor = selfRoleName.ForeColor;

        var hasRightToChangeSettings = role!.Value == Player.White && !IsGameOn;

        tableLayoutPanelButtons.Visible = role!.Value != Player.None && IsGameOn;
        flowLayoutPanelSettings.Visible = hasRightToChangeSettings;
        settingsTitle.Visible = hasRightToChangeSettings;
        buttonSaveSettings.Visible = hasRightToChangeSettings;
    }

    private void HandlePromotion(Position from, Position to)
    {
        if (gameState == null)
        {
            return;
        }

        pendingPromotionMove = new NormalMove(from, to);
        mainPanel.Invalidate();

        var promotionMenu = new PromotionMenu(gameState.CurrentPlayer);
        promotionMenu.PieceSelected += pieceType =>
        {
            promotionMenu.Close();
            var promMove = new PawnPromotion(from, to, pieceType);
            HandleMove(promMove);
            pendingPromotionMove = null;
        };
        promotionMenu.ShowDialog();
    }

    private void CountPiecesCapturesForSelfAndOpponent()
    {
        foreach (Control image in flowLayoutPanelSelfCaptures.Controls)
        {
            image.Dispose();
        }

        foreach (Control image in flowLayoutPanelOpponentCaptures.Controls)
        {
            image.Dispose();
        }

        var isFlipped = role!.Value == Player.Black;

        var capturedBlackPieces = startPieces
            .ToDictionary(x => x.Key, x => x.Value - gameState!.Board.CountOf(Player.Black, x.Key));

        foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType)))
        {
            for (var i = 0; i < capturedBlackPieces[pieceType]; i++)
            {
                if (!isFlipped)
                {
                    flowLayoutPanelSelfCaptures.Controls.Add(CreatePiecePictureForCapture(Player.Black, pieceType));
                }
                else
                {
                    flowLayoutPanelOpponentCaptures.Controls.Add(CreatePiecePictureForCapture(Player.Black, pieceType));
                }
            }
        }

        var capturedWhitePieces = startPieces
            .ToDictionary(x => x.Key, x => x.Value - gameState!.Board.CountOf(Player.White, x.Key));

        foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType)))
        {
            for (var i = 0; i < capturedWhitePieces[pieceType]; i++)
            {
                if (!isFlipped)
                {
                    flowLayoutPanelOpponentCaptures.Controls.Add(CreatePiecePictureForCapture(Player.White, pieceType));
                }
                else
                {
                    flowLayoutPanelSelfCaptures.Controls.Add(CreatePiecePictureForCapture(Player.White, pieceType));
                }
            }
        }
    }

    private PictureBox CreatePiecePictureForCapture(Player player, PieceType pieceType)
        => new()
        {
            Width = Convert.ToInt32(flowLayoutPanelSelfCaptures.Height / 1.2),
            Height = flowLayoutPanelSelfCaptures.Height,
            Image = ImageHelper.GetImage(player, pieceType),
            SizeMode = PictureBoxSizeMode.Zoom,
            Margin = new Padding(0, 0, 0, 0),
        };
}
