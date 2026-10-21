using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Client.Services;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private readonly Dictionary<Position, Move> moveCache = [];
    private List<(Player Color, PieceType Type)>? selfCaptures;
    private List<(Player Color, PieceType Type)>? opponentCaptures;
    private readonly List<(Position From, Position To, PieceType? PromotionPiece)> premoves = [];
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

    private Position? dragFromPos;
    private Piece? draggedPiece;
    private Point? dragCurrentCursor;
    private bool isDragging;

    private Board? displayBoard;

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

        var board = displayBoard ?? gameState.Board;
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                if (isDragging && dragFromPos != null &&
                    row == dragFromPos.Value.Row && col == dragFromPos.Value.Column)
                {
                    continue;
                }

                if (premoves.Any(p => p.From == new Position(row, col)))
                {
                    continue;
                }

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
            var movingPiece = board[pendingPromotionMove.FromPos.Row, pendingPromotionMove.FromPos.Column];
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

        foreach (var (from, to, promotionType) in premoves)
        {
            Piece? movingPiece = null;
            Position? drawPosition = null;

            movingPiece = board[from.Row, from.Column];

            if (movingPiece != null)
            {
                drawPosition = from;
            }
            else
            {
                movingPiece = board[to.Row, to.Column];
                if (movingPiece != null)
                {
                    drawPosition = to;
                }
            }

            if (movingPiece != null && drawPosition.HasValue)
            {
                Bitmap? pieceImage;
                if (promotionType != null)
                {
                    pieceImage = ImageHelper.GetImage(movingPiece.Color, promotionType.Value);
                }
                else
                {
                    pieceImage = ImageHelper.GetImage(movingPiece);
                }

                if (pieceImage != null)
                {
                    g.DrawImage(pieceImage, cellRectangles[drawPosition.Value.Row, drawPosition.Value.Column]);
                }
            }
        }

        if (isDragging && draggedPiece != null && dragCurrentCursor != null)
        {
            var pieceImage = ImageHelper.GetImage(draggedPiece);
            if (pieceImage != null)
            {
                var cellSize = cellRectangles[0, 0].Width;
                var x = dragCurrentCursor.Value.X - cellSize / 2;
                var y = dragCurrentCursor.Value.Y - cellSize / 2;
                var rect = new Rectangle(x, y, cellSize, cellSize);

                g.DrawImage(pieceImage, rect);
            }
        }
    }

    private void DrawHighlights(Graphics g)
    {
        if (selectedPos != null)
        {
            using var selectedBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 0));
            g.FillRectangle(selectedBrush, cellRectangles[selectedPos.Value.Row, selectedPos.Value.Column]);

            using var moveBrush = new SolidBrush(Color.FromArgb(50, 80, 80, 80));
            foreach (var pos in moveCache.Keys)
            {
                var r = cellRectangles[pos.Row, pos.Column];
                var targetPiece = gameState!.Board[pos.Row, pos.Column];

                if (targetPiece != null)
                {
                    using var pen = new Pen(Color.FromArgb(150, 80, 80, 80), 5);
                    g.DrawEllipse(pen, r.X + 3, r.Y + 3, r.Width - 6, r.Height - 6);
                }
                else
                {
                    using var brush = new SolidBrush(Color.FromArgb(50, 80, 80, 80));
                    var dotSize = r.Width / 3;
                    g.FillEllipse(brush,
                        r.X + (r.Width - dotSize) / 2,
                        r.Y + (r.Height - dotSize) / 2,
                        dotSize, dotSize);
                }
            }
        }

        if (lastMove != null)
        {
            using var brush = new SolidBrush(Color.FromArgb(150, 233, 215, 0));
            g.FillRectangle(brush, cellRectangles[lastMove.FromPos.Row, lastMove.FromPos.Column]);
            g.FillRectangle(brush, cellRectangles[lastMove.ToPos.Row, lastMove.ToPos.Column]);
        }

        foreach (var (from, to, _) in premoves)
        {
            using var premoveBrush = new SolidBrush(Color.FromArgb(100, 200, 20, 20));
            g.FillRectangle(premoveBrush, cellRectangles[from.Row, from.Column]);
            g.FillRectangle(premoveBrush, cellRectangles[to.Row, to.Column]);
        }
    }

    private void RebuildDisplayBoard()
    {
        if (gameState == null || premoves.Count == 0)
        {
            displayBoard = null;
            return;
        }

        displayBoard = gameState.Board.Copy();
        foreach (var (from, to, _) in premoves)
        {
            displayBoard[to.Row, to.Column] = displayBoard[from.Row, from.Column];
            displayBoard[from.Row, from.Column] = null;
        }
    }

    private void MainPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || gameState == null || isDragging)
        {
            return;
        }

        var pos = GetPositionFromPoint(e.Location);
        if (pos == null)
        {
            return;
        }

        if (TryingToPremove())
        {
            HandlePremove(pos.Value, e);
            return;
        }

        var moves = gameState.LegalMovesForPiece(pos.Value, role);
        if (!moves.Any())
        {
            return;
        }

        selectedPos = pos.Value;
        moveCache.Clear();
        foreach (var move in moves)
        {
            moveCache[move.ToPos] = move;
        }

        var piece = gameState.Board[selectedPos.Value.Row, selectedPos.Value.Column];
        if (piece == null)
        {
            ClearSelection();
            return;
        }

        dragFromPos = selectedPos;
        draggedPiece = piece;
        dragCurrentCursor = e.Location;
        isDragging = true;

        gameState.Board[selectedPos.Value.Row, selectedPos.Value.Column] = null;
        mainPanel.Invalidate();
    }

    private void HandlePremove(Position pos, MouseEventArgs e)
    {
        if (gameState == null)
        {
            return;
        }

        displayBoard ??= gameState.Board.Copy();
        var premovePiece = displayBoard[pos.Row, pos.Column];
        var isNotAllowedToMove = premovePiece == null || (premovePiece != null && premovePiece.Color != role);

        if ((selectedPos != null || moveCache.ContainsKey(pos)) && isNotAllowedToMove)
        {
            isDragging = false;
            return;
        }

        selectedPos = pos;

        if (isNotAllowedToMove)
        {
            ClearSelection();
            return;
        }

        var premovePositions = gameState.PremovePositionsForPiece(selectedPos.Value, role, displayBoard);
        moveCache.Clear();
        foreach (var to in premovePositions)
        {
            moveCache[to] = null!;
        }

        dragFromPos = selectedPos;
        draggedPiece = premovePiece;
        dragCurrentCursor = e.Location;
        isDragging = true;

        displayBoard[selectedPos.Value.Row, selectedPos.Value.Column] = null;
        mainPanel.Invalidate();
    }

    private void MainPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        if (isDragging && draggedPiece != null)
        {
            dragCurrentCursor = e.Location;
            mainPanel.Invalidate();
            return;
        }
    }

    private void MainPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!isDragging)
        {
            HandleClick(e);
            return;
        }

        var dropPos = GetPositionFromPoint(e.Location);

        if (displayBoard == null)
        {
            gameState!.Board[dragFromPos!.Value.Row, dragFromPos!.Value.Column] = draggedPiece;
        }

        if (TryingToPremove())
        {
            if (dropPos == null || (dropPos != null && dropPos.Value == dragFromPos!.Value))
            {
                if (premoves.Count == 0)
                {
                    displayBoard = null;
                }

                if (displayBoard != null)
                {
                    displayBoard[dragFromPos!.Value.Row, dragFromPos!.Value.Column] = draggedPiece;
                }

                ClearDrag();
                mainPanel.Invalidate();
                return;
            }

            var piece = draggedPiece;
            var from = dragFromPos!.Value;
            var to = dropPos!.Value;

            if (moveCache.ContainsKey(dropPos.Value))
            {
                if (displayBoard != null)
                {
                    displayBoard[dropPos.Value.Row, dropPos.Value.Column] = draggedPiece;
                }

                if (piece?.Type == PieceType.Pawn && IsPromotionRank(to, piece.Color))
                {
                    ShowPromotionDialogAndQueue(from, to, piece.Color);
                }
                else
                {
                    premoves.Add((from, to, null));
                }
                RebuildDisplayBoard();
            }
            else
            {
                if (displayBoard != null)
                {
                    displayBoard[dragFromPos!.Value.Row, dragFromPos!.Value.Column] = draggedPiece;
                }
            }

            ClearSelection();
            ClearDrag();
            mainPanel.Invalidate();
            return;
        }

        var handlePromotion = false;
        var handleMoveDirect = false;
        Move? moveToMake = null;

        if (dropPos != null && moveCache.TryGetValue(dropPos.Value, out var move))
        {
            if (move.Type == MoveType.PawnPromotion)
            {
                handlePromotion = true;
            }
            else
            {
                handleMoveDirect = true;
            }
            moveToMake = move;
        }

        ClearDrag();

        if (handlePromotion)
        {
            HandlePromotion(moveToMake!.FromPos, moveToMake.ToPos);
        }
        else if (handleMoveDirect && moveToMake != null)
        {
            HandleMove(moveToMake);
        }
        else
        {
            mainPanel.Invalidate();
        }
    }

    private void HandleClick(MouseEventArgs e)
    {
        if (gameState == null)
        {
            return;
        }

        var clickPos = GetPositionFromPoint(e.Location);
        if (clickPos == null)
        {
            return;
        }

        if (selectedPos == null)
        {
            OnFromPositionSelected(clickPos.Value);
        }
        else if (TryingToPremove())
        {
            var piece = displayBoard?[selectedPos.Value.Row, selectedPos.Value.Column]
                ?? gameState.Board[selectedPos.Value.Row, selectedPos.Value.Column];
            var from = selectedPos.Value;
            var to = clickPos.Value;

            if (moveCache.ContainsKey(to))
            {
                if (piece?.Type == PieceType.Pawn && IsPromotionRank(to, piece.Color))
                {
                    ShowPromotionDialogAndQueue(from, to, piece.Color);
                }
                else
                {
                    premoves.Add((from, to, null));
                }
                RebuildDisplayBoard();
            }

            ClearSelection();
            mainPanel.Invalidate();
        }
        else
        {
            OnToPositionSelected(clickPos.Value);
        }
    }

    private void ClearDrag()
    {
        isDragging = false;
        dragFromPos = null;
        draggedPiece = null;
        dragCurrentCursor = null;
    }

    private void ClearSelection()
    {
        selectedPos = null;
        moveCache.Clear();
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

    private static bool IsPromotionRank(Position to, Player color)
        => color == Player.White ? to.Row == 0 : to.Row == 7;

    private bool TryingToPremove()
        => role!.Value != gameState?.CurrentPlayer
        && role!.Value != Player.None && gameState?.CurrentPlayer != Player.None;

    private void ShowPromotionDialogAndQueue(Position from, Position to, Player color)
    {
        var menu = new PromotionMenu(color);
        menu.PieceSelected += type =>
        {
            menu.Close();
            premoves.Add((from, to, type));
            mainPanel.Invalidate();
        };
        menu.ShowDialog(this);
    }

    private void OnFromPositionSelected(Position position)
    {
        if (gameState == null)
        {
            return;
        }

        var moves = gameState.LegalMovesForPiece(position, role);
        if (!moves.Any())
        {
            return;
        }

        selectedPos = position;
        moveCache.Clear();
        foreach (var move in moves)
        {
            moveCache[move.ToPos] = move;
        }

        mainPanel.Invalidate();
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

    private void CountPiecesCapturesForSelfAndOpponent()
    {
        var isFlipped = role == Player.Black;

        var capturedBlack = startPieces.ToDictionary(
            x => x.Key, x => x.Value - gameState!.Board.CountOf(Player.Black, x.Key));

        var capturedWhite = startPieces.ToDictionary(
            x => x.Key, x => x.Value - gameState!.Board.CountOf(Player.White, x.Key));

        if (!isFlipped)
        {
            selfCaptures = FlattenCaptures(Player.Black, capturedBlack);
            opponentCaptures = FlattenCaptures(Player.White, capturedWhite);
        }
        else
        {
            selfCaptures = FlattenCaptures(Player.White, capturedWhite);
            opponentCaptures = FlattenCaptures(Player.Black, capturedBlack);
        }

        selfCapturesPanel.Invalidate();
        opponentCapturesPanel.Invalidate();
    }

    private static List<(Player Color, PieceType Type)> FlattenCaptures(
    Player capturedColor, Dictionary<PieceType, int> capturedCounts)
    {
        var result = new List<(Player, PieceType)>();
        foreach (var pieceType in capturedCounts.Keys)
        {
            for (var i = 0; i < capturedCounts[pieceType]; i++)
            {
                result.Add((capturedColor, pieceType));
            }
        }
        return result;
    }


    private void PanelCaptures_Paint(object sender, PaintEventArgs e)
    {
        var list = sender == selfCapturesPanel ? selfCaptures : opponentCaptures;
        if (list == null || list.Count == 0)
        {
            return;
        }

        var panel = (Panel)sender;
        var cellSize = panel.Height;
        var x = 0;
        const int tightGap = -12;
        const int normalGap = 0;

        for (var i = 0; i < list.Count; i++)
        {
            var (color, type) = list[i];
            var img = ImageHelper.GetImage(color, type);
            if (img == null)
            {
                continue;
            }

            var gap = (i > 0 && list[i - 1].Type == type) ? tightGap : normalGap;
            x += gap;

            e.Graphics.DrawImage(img, x, 0, cellSize, cellSize);
            x += cellSize;
        }
    }
}
