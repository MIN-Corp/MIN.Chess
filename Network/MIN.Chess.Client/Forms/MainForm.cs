using MIN.Chess.Client.Contracts.Enums;
using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Client.Services;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Client.Forms;

/// <summary>
/// Главная форма
/// </summary>
public partial class MainForm : Form
{
    private const int ConnectionTimeoutMs = 5_000;

    private readonly IClientProcessBridge bridge;
    private readonly CancellationTokenSource appCts;
    private readonly Dictionary<Position, Move> moveCache = [];

    private GameState? gameState;
    private Position? selectedPos;
    private Rectangle[,] cellRectangles = null!;
    private Move? pendingPromotionMove;
    private Move? lastMove;
    private string? lastMoveUci;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainForm"/>
    /// </summary>
    public MainForm(IClientProcessBridge bridge, CancellationTokenSource appCts, string connectionString)
    {
        this.bridge = bridge;
        this.appCts = appCts;
        InitializeComponent();

        mainPanel.BackgroundImage = Properties.Resources.Board;
        mainPanel.BackgroundImageLayout = ImageLayout.Zoom;

        CalculateCellRectangles();
        InitializeNetworking(connectionString);
    }

    private async void InitializeNetworking(string connectionString)
    {
        bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
        await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, appCts.Token);

        bridge.NetworkMessageReceived += NetworkMessageReceived;

        await bridge.StartListeningAsync(appCts.Token);

        await bridge.SendReadyMessage(appCts.Token);

        await bridge.SendMessage(new StateUpdateRequestMessage(), cancellationToken: appCts.Token);
    }

    private async void NetworkMessageReceived(object? sender, INetworkMessage message)
    {
        await HandleMessage(message);
    }

    private async Task HandleMessage(INetworkMessage message)
    {
        switch (message)
        {
            case StateUpdateMessage stateUpdateMessage:
                var firstUpdate = gameState == null;

                gameState = new GameState(Board.FromStateString(stateUpdateMessage.StateString), stateUpdateMessage.CurrentPlayer);

                if (firstUpdate)
                {
                    mainPanel.Paint += MainPanel_Paint;
                    mainPanel.MouseClick += MainPanel_MouseClick;
                    mainPanel.Resize += (_, _) => CalculateCellRectangles();
                }

                moveCache.Clear();
                mainPanel.Invalidate();
                break;

            case MoveUpdateMessage moveUpdateMessage:
                if (gameState == null)
                {
                    await bridge.SendMessage(new StateUpdateRequestMessage(), cancellationToken: appCts.Token);
                    return;
                }

                if (lastMoveUci == moveUpdateMessage.MoveUci)
                {
                    return;
                }

                var move = MoveParser.Parse(moveUpdateMessage.MoveUci, gameState.Board);

                gameState.MakeMove(move);
                lastMove = move;
                moveCache.Clear();
                mainPanel.Invalidate();
                break;

            case GameResultMessage gameResultMessage:
                if (gameState == null)
                {
                    await bridge.SendMessage(new StateUpdateRequestMessage(), cancellationToken: appCts.Token);
                    return;
                }

                ShowGameOver(gameResultMessage.Result, gameState.CurrentPlayer);
                break;
        }
    }

    /// <summary>
    /// Пересчёт прямоугольников клеток на основе текущего размера панели
    /// </summary>
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
                cellRectangles[row, col] = new Rectangle(x, y, cellSize, cellSize);
            }
        }
    }

    /// <summary>
    /// Отрисовка всего: доска, фигуры, подсветка
    /// </summary>
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

        var moves = gameState.LegalMovesForPiece(position);
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

    private async void HandleMove(Move move)
    {
        if (gameState == null)
        {
            return;
        }

        gameState.MakeMove(move);

        lastMove = move;

        moveCache.Clear();
        mainPanel.Invalidate();

        lastMoveUci = move.ToUci();

        var message = new MoveRequestMessage()
        {
            MoveUciNotation = lastMoveUci,
        };

        await bridge.SendMessage(message, cancellationToken: appCts.Token);
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

    private void ShowGameOver(Result result, Player currentPlayer)
    {
        if (gameState == null)
        {
            return;
        }

        var menu = new GameOverMenu(result, currentPlayer);
        menu.OnOptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                menu.Close();
                RestartGame();
            }
            else
            {
                Close();
            }
        };

        menu.ShowDialog();
    }

    private void RestartGame()
    {
        moveCache.Clear();
        lastMove = null;
        gameState = new GameState(Board.Initial(), Player.White);
        mainPanel.Invalidate();
    }

    private void MainForm_Resize(object sender, EventArgs e)
    {
        CalculateCellRectangles();
        mainPanel.Invalidate();
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        var menu = new PauseMenu();
        menu.OnOptionSelected += option =>
        {
            menu.Close();
            if (option == Option.Restart)
            {
                RestartGame();
            }
        };
        menu.ShowDialog();
    }
}
