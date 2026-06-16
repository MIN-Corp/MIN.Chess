using MIN.Chess.Client.Contracts.Enums;
using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Client.Services;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Client.Forms;

/// <summary>
/// Главная форма
/// </summary>
public partial class MainForm : Form
{
    private const int ConnectionTimeoutMs = 5_000;

    private readonly SynchronizationContext uiContext;
    private readonly IClientProcessBridge bridge;
    private readonly CancellationTokenSource appCts;
    private readonly Dictionary<Position, Move> moveCache = [];
    private readonly Dictionary<PieceType, int> startPieces = new()
    {
        [PieceType.Pawn] = 8,
        [PieceType.Knight] = 2,
        [PieceType.Bishop] = 2,
        [PieceType.Rook] = 2,
        [PieceType.Queen] = 1,
        [PieceType.King] = 1,
    };

    private Dictionary<Player, Participant?> players = [];
    private Player? role;
    private Participant? participant;
    private GameOverMenu? gameOverMenu;
    private PauseMenu? pauseMenu;
    private GameState? gameState;
    private Position? selectedPos;
    private Rectangle[,] cellRectangles = null!;
    private Move? pendingPromotionMove;
    private Move? lastMove;
    private int spectatorsCount;
    private int restartVote;

    // Timer
    private readonly static int?[] timerMinutes = [null, 1, 3, 5, 10, 30];
    private readonly static int[] bonusSeconds = [0, 1, 2, 3, 5, 10, 20];

    private int timerIndex;
    private int bonusIndex;

    private long whiteMs;
    private long blackMs;
    private int timerBonusSeconds;
    private bool timerEnabled;
    private DateTime lastTimerSync;
    private Player? activeTimer;
    private System.Windows.Forms.Timer? countdownTimer;

    private bool IsGameOn => lastMove != null;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MainForm"/>
    /// </summary>
    public MainForm(IClientProcessBridge bridge, CancellationTokenSource appCts, string connectionString)
    {
        uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
        this.bridge = bridge;
        this.appCts = appCts;
        InitializeComponent();

        resignButton.BackColor = Color.Black;
        drawButton.BackColor = Color.Black;
        pauseButton.BackColor = Color.Black;

        timerTrackBar.Maximum = timerMinutes.Length - 1;
        bonusTrackBar.Maximum = bonusSeconds.Length - 1;

        UpdateTimerChoosingDisplay();
        UpdateBonusChoosingDisplay();

        CalculateCellRectangles();
        InitializeNetworking(connectionString);
    }

    private async void InitializeNetworking(string connectionString)
    {
        try
        {
            bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();
            await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, appCts.Token);

            bridge.NetworkMessageReceived += NetworkMessageReceived;

            await bridge.StartListeningAsync(appCts.Token);

            await bridge.SendReadyMessage(appCts.Token);

            await bridge.SendMessage(new InitRequestMessage(), cancellationToken: appCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void NetworkMessageReceived(object? sender, INetworkMessage message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(async () =>
            {
                try
                {
                    await HandleMessage(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            return;
        }
        await HandleMessage(message);
    }

    private async Task HandleMessage(INetworkMessage message)
    {
        if (gameState == null && (message is not InitResponseMessage))
        {
            MessageBox.Show($"Got other message rather then init: {message.GetType()}");
            return;
        }

        switch (message)
        {
            case InitResponseMessage initResponseMessage:
                var firstUpdate = gameState == null;

                gameState = new GameState(Board.FromStateString(initResponseMessage.StateString), initResponseMessage.CurrentTurn);

                lastMove = initResponseMessage.LastMove != null
                    ? MoveParser.Parse(initResponseMessage.LastMove, gameState.Board)
                    : null;

                role = initResponseMessage.RequesterRole;
                participant = initResponseMessage.SelfParticipant;

                players = initResponseMessage.ActivePlayers.ToDictionary();
                spectatorsCount = initResponseMessage.SpectorsCount;

                timerBonusSeconds = initResponseMessage.TimerBonusMs * 1000;
                timerEnabled = initResponseMessage.WhiteMs.HasValue;

                uiContext.Post(_ =>
                {
                    if (firstUpdate)
                    {
                        mainPanel.Paint += MainPanel_Paint;
                        mainPanel.MouseClick += MainPanel_MouseClick;
                        mainPanel.Resize += (_, _) => CalculateCellRectangles();

                        mainPanel.BackgroundImage = role == Player.Black
                            ? Properties.Resources.BoardReversed
                            : Properties.Resources.Board;
                        mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    }

                    SyncTimerValues(initResponseMessage.WhiteMs, initResponseMessage.BlackMs, gameState.CurrentPlayer);
                    ColorCurrentTurn();
                    UpdatePlayersInfo();
                    CalculateCellRectangles();

                    moveCache.Clear();
                    mainPanel.Invalidate();
                }, this);
                break;

            case StateUpdateMessage stateUpdateMessage:
                gameState!.Board = Board.FromStateString(stateUpdateMessage.StateString);
                gameState.CurrentPlayer = stateUpdateMessage.CurrentTurn;
                timerEnabled = stateUpdateMessage.WhiteMs.HasValue;
                timerBonusSeconds = stateUpdateMessage.TimerBonusSeconds * 1000;

                lastMove = stateUpdateMessage.LastMove != null
                    ? MoveParser.Parse(stateUpdateMessage.LastMove, gameState.Board)
                    : null;

                ColorCurrentTurn();
                CountPiecesCapturesForSelfAndOpponent();
                SyncTimerValues(stateUpdateMessage.WhiteMs, stateUpdateMessage.BlackMs, gameState.CurrentPlayer);

                moveCache.Clear();
                mainPanel.Invalidate();
                break;

            case MoveUpdateMessage moveUpdateMessage:
                var move = MoveParser.Parse(moveUpdateMessage.MoveUci, gameState!.Board);
                timerEnabled = moveUpdateMessage.WhiteMs.HasValue;

                gameState.MakeMove(move);
                ColorCurrentTurn();
                lastMove = move;

                SyncTimerValues(moveUpdateMessage.WhiteMs, moveUpdateMessage.BlackMs, gameState.CurrentPlayer);
                CountPiecesCapturesForSelfAndOpponent();

                moveCache.Clear();
                mainPanel.Invalidate();
                break;

            case GameResultMessage gameResultMessage:
                gameState?.CurrentPlayer = Player.None;
                countdownTimer?.Stop();
                uiContext.Post(_ =>
                {
                    drawQuote.Visible = false;
                    ShowGameOver(gameResultMessage.Result, gameState!.CurrentPlayer);
                }, this);
                break;

            case ErrorMessage errorMessage:
                uiContext.Post(_ =>
                {
                    MessageBox.Show(errorMessage.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }, this);
                break;

            case RestartVoteMessage restartVoteMessage:
                var wasAboveZero = restartVote > 0;
                restartVote = restartVoteMessage.CurrentVotes;
                uiContext.Post(_ =>
                {
                    UpdateVoteAndPlayersForMenu(wasAboveZero);
                }, this);
                break;

            case DrawVoteMessage drawVoteMessage:
                drawQuote.Visible = drawVoteMessage.CurrentVotes > 0;
                break;

            case PlayerConnectedMessage playerConnectedMessage:
                if (playerConnectedMessage.Role != Player.None)
                {
                    players[playerConnectedMessage.Role] = playerConnectedMessage.Participant;
                    changeRoleButton.Visible = CountPlayers() == 1 && role!.Value == Player.None;

                    if (participant?.Id == playerConnectedMessage.Participant.Id && role!.Value == Player.None)
                    {
                        role = playerConnectedMessage.Role;
                        CalculateCellRectangles();
                    }
                }
                else
                {
                    spectatorsCount++;
                }
                UpdatePlayersInfo();
                UpdateVoteAndPlayersForMenu(false);
                break;

            case PlayerDisconnectedMessage playerDisconnectedMessage:
                if (playerDisconnectedMessage.Role != Player.None)
                {
                    players.Remove(playerDisconnectedMessage.Role);
                    gameState?.CurrentPlayer = Player.None;

                    if (role!.Value == Player.None)
                    {
                        changeRoleButton.Visible = true;
                    }
                }
                else
                {
                    spectatorsCount--;
                }
                UpdatePlayersInfo();
                UpdateVoteAndPlayersForMenu(false);
                break;
        }
    }

    private void ColorCurrentTurn()
    {
        if (role!.Value == Player.Black)
        {
            if (gameState?.CurrentPlayer == Player.White)
            {
                PaintUpTurn();
            }
            else if (gameState?.CurrentPlayer == Player.Black)
            {
                PaintDownTurn();
            }
            else
            {
                PaintNoneTurn();
            }
        }
        else
        {
            if (gameState?.CurrentPlayer == Player.White)
            {
                PaintDownTurn();
            }
            else if (gameState?.CurrentPlayer == Player.Black)
            {
                PaintUpTurn();
            }
            else
            {
                PaintNoneTurn();
            }
        }

        var hasRightToChangeSettings = role!.Value == Player.White && !IsGameOn;

        tableLayoutPanelButtons.Visible = role!.Value != Player.None && IsGameOn;
        flowLayoutPanelSettings.Visible = hasRightToChangeSettings;
        settingsTitle.Visible = hasRightToChangeSettings;
        buttonSaveSettings.Visible = hasRightToChangeSettings;
    }

    private void PaintDownTurn()
    {
        tableLayoutPanelOpponent.BackColor = Color.Black;
        opponentName.ForeColor = Color.White;
        tableLayoutPanelSelf.BackColor = Color.White;
        selfRoleName.ForeColor = Color.Black;
        opponentTimerLabel.ForeColor = Color.White;
        selfTimerLabel.ForeColor = Color.Black;
    }

    private void PaintUpTurn()
    {
        tableLayoutPanelOpponent.BackColor = Color.White;
        opponentName.ForeColor = Color.Black;
        tableLayoutPanelSelf.BackColor = Color.Black;
        selfRoleName.ForeColor = Color.White;
        opponentTimerLabel.ForeColor = Color.Black;
        selfTimerLabel.ForeColor = Color.White;
    }

    private void PaintNoneTurn()
    {
        tableLayoutPanelOpponent.BackColor = Color.Black;
        opponentName.ForeColor = Color.White;
        tableLayoutPanelSelf.BackColor = Color.Black;
        selfRoleName.ForeColor = Color.White;
        opponentTimerLabel.ForeColor = Color.White;
        selfTimerLabel.ForeColor = Color.White;
    }

    private void UpdateVoteAndPlayersForMenu(bool wasAboveZero)
    {
        restartQuote.Visible = restartVote >= 1;

        if (wasAboveZero)
        {
            gameOverMenu?.Close();
            pauseMenu?.Close();
            return;
        }

        gameOverMenu?.UpdateVoteDisplay(restartVote, CountPlayers());
        pauseMenu?.UpdateVoteDisplay(restartVote, CountPlayers());
    }

    private void UpdatePlayersInfo()
    {
        players.TryGetValue(role!.Value == Player.None ? Player.White : role!.Value, out var player1);

        selfRoleName.Text = player1 != null
            ? player1.Name
            : "Ожидаем оппонента...";

        players.TryGetValue((role!.Value == Player.None ? Player.White : role!.Value).Opponent(), out var player2);

        opponentName.Text = player2 != null
            ? player2.Name
            : "Ожидаем оппонента...";

        spectatorsCounter.Visible = spectatorsCount > 0;
        spectatorLogo.Visible = spectatorsCount > 0;

        tableLayoutPanelButtons.Visible = (role!.Value != Player.None && IsGameOn);

        if (spectatorsCount > 0)
        {
            spectatorsCounter.Text = spectatorsCount.ToString();
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
                var r = role == Player.Black ? 7 - row : row;
                var c = role == Player.Black ? 7 - col : col;
                cellRectangles[r, c] = new Rectangle(x, y, cellSize, cellSize);
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

    private async void HandleMove(Move move)
    {
        if (gameState == null)
        {
            return;
        }

        try
        {
            var elapsed = IsGameOn ? (long)(DateTime.UtcNow - lastTimerSync).TotalMilliseconds : 0;
            gameState.MakeMove(move);

            lastMove = move;

            moveCache.Clear();
            mainPanel.Invalidate();

            if (role == Player.White)
            {
                whiteMs = Math.Max(0, whiteMs - elapsed) + timerBonusSeconds;
            }
            else
            {
                blackMs = Math.Max(0, blackMs - elapsed) + timerBonusSeconds;
            }

            SyncTimerValues(timerEnabled ? whiteMs : null, blackMs, gameState.CurrentPlayer);
            ColorCurrentTurn();
            CountPiecesCapturesForSelfAndOpponent();

            var message = new MoveRequestMessage()
            {
                MoveUciNotation = move.ToUci(),
            };

            await bridge.SendMessage(message, cancellationToken: appCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CountPiecesCapturesForSelfAndOpponent()
    {
        flowLayoutPanelSelfCaptures.Controls.Clear();
        flowLayoutPanelOpponentCaptures.Controls.Clear();

        var isFlipped = role!.Value == Player.Black;

        var capturedBlackPieces = startPieces
            .ToDictionary(x => x.Key, x => x.Value - gameState?.Board.CountOf(Player.Black, x.Key));

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
            .ToDictionary(x => x.Key, x => x.Value - gameState?.Board.CountOf(Player.White, x.Key));

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

    private int CountPlayers()
    {
        var count = 0;

        if (players.TryGetValue(Player.White, out var white) && white != null)
        {
            count++;
        }

        if (players.TryGetValue(Player.Black, out var black) && black != null)
        {
            count++;
        }

        return count;
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

        gameOverMenu = new GameOverMenu(role!.Value, result, currentPlayer, CountPlayers(), restartVote);
        gameOverMenu.OnOptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                SendRestartRequest();
            }
            else
            {
                gameOverMenu.Close();
            }
        };

        gameOverMenu.ShowDialog();
        gameOverMenu = null;
    }

    private async void SendRestartRequest()
    {
        try
        {
            await bridge.SendMessage(new RestartRequestMessage(), cancellationToken: appCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Restart Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
        pauseMenu = new PauseMenu(CountPlayers(), restartVote);
        pauseMenu.OnOptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                SendRestartRequest();
            }
            else
            {
                pauseMenu.Close();
            }
        };
        pauseMenu.ShowDialog();
        pauseMenu = null;
    }

    private void pauseButton_Click(object sender, EventArgs e)
    {
        ShowPauseMenu();
    }

    private async void SendResign()
    {
        if (MessageBox.Show("Вы точно хотите сдаться?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            try
            {
                await bridge.SendMessage(new ResignRequestMessage(), cancellationToken: appCts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Resign Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void UpdateTimerChoosingDisplay()
    {
        var val = timerMinutes[timerIndex];
        timerValueLabel.Text = val.HasValue ? $"{val} мин" : "Без таймера";
    }

    private void UpdateBonusChoosingDisplay()
    {
        timerBonusValueLabel.Text = $"+{bonusSeconds[bonusIndex]} сек";
    }

    private void SyncTimerValues(long? white, long? black, Player currentTurn)
    {
        if (!white.HasValue || !IsGameOn)
        {
            countdownTimer?.Stop();
            if (white.HasValue)
            {
                whiteMs = white.Value;
                blackMs = black == null ? 0 : black.Value;
            }
            UpdateTimersToInitial(white, black);
            return;
        }

        whiteMs = white.Value;
        blackMs = black == null ? 0 : black.Value;
        lastTimerSync = DateTime.UtcNow;
        activeTimer = currentTurn;

        if (countdownTimer == null)
        {
            countdownTimer = new System.Windows.Forms.Timer { Interval = 250 };
            countdownTimer.Tick += (_, _) => UpdateTimerDisplay();
            countdownTimer.Start();
        }
        else if (!countdownTimer.Enabled)
        {
            countdownTimer.Start();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimersToInitial(long? white, long? black)
    {
        if (white.HasValue && black.HasValue)
        {
            selfTimerLabel.Text = FormatTime(white.Value);
            opponentTimerLabel.Text = FormatTime(black.Value);
        }
        else
        {
            selfTimerLabel.Text = string.Empty;
            opponentTimerLabel.Text = string.Empty;
        }
    }

    private void UpdateTimerDisplay()
    {
        var currentWhiteMs = whiteMs;
        var currentBlackMs = blackMs;
        var elapsed = (long)(DateTime.UtcNow - lastTimerSync).TotalMilliseconds;

        if (activeTimer == Player.White && timerEnabled)
        {
            currentWhiteMs = Math.Max(0, currentWhiteMs - elapsed);
        }
        else if (activeTimer == Player.Black && timerEnabled)
        {
            currentBlackMs = Math.Max(0, currentBlackMs - elapsed);
        }

        selfTimerLabel.Text = role == Player.White ? FormatTime(currentWhiteMs) : FormatTime(currentBlackMs);
        opponentTimerLabel.Text = role == Player.White ? FormatTime(currentBlackMs) : FormatTime(currentWhiteMs);
    }

    private static string FormatTime(long ms)
    {
        var t = TimeSpan.FromMilliseconds(ms);
        return t.TotalHours >= 1
            ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}"
            : $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    private void resignButton_Click(object sender, EventArgs e)
    {
        if (!IsGameOn)
        {
            return;
        }
        SendResign();
    }

    private void restartQuote_Click(object sender, EventArgs e)
    {
        SendRestartRequest();
    }

    private async void SendChangeRoleRequest()
    {
        await bridge.SendMessage(new ChangeRoleRequestMessage(), cancellationToken: appCts.Token);
    }

    private void changeRoleButton_Click(object sender, EventArgs e)
    {
        SendChangeRoleRequest();
    }

    private async void SendDrawRequest()
    {
        await bridge.SendMessage(new DrawRequestMessage(), cancellationToken: appCts.Token);
    }

    private void drawButton_Click(object sender, EventArgs e)
    {
        SendDrawRequest();
    }

    private void timerTrackBar_ValueChanged(object sender, EventArgs e)
    {
        timerIndex = timerTrackBar.Value;
        UpdateTimerChoosingDisplay();
    }

    private void bonusTrackBar_ValueChanged(object sender, EventArgs e)
    {
        bonusIndex = bonusTrackBar.Value;
        UpdateBonusChoosingDisplay();
    }

    private void buttonTimerPrevious_Click(object sender, EventArgs e)
    {
        timerTrackBar.Value = Math.Max(0, timerTrackBar.Value - 1);
    }

    private void buttonTimerNext_Click(object sender, EventArgs e)
    {
        timerTrackBar.Value = Math.Min(timerMinutes.Length - 1, timerTrackBar.Value + 1);
    }

    private void buttonBonusPrevious_Click(object sender, EventArgs e)
    {
        bonusTrackBar.Value = Math.Max(0, bonusTrackBar.Value - 1);
    }

    private void buttonBonusNext_Click(object sender, EventArgs e)
    {
        bonusTrackBar.Value = Math.Min(bonusSeconds.Length - 1, bonusTrackBar.Value + 1);
    }

    private async void SendGameSettingsAsync()
    {
        try
        {
            await bridge.SendMessage(new GameSettingsMessage
            {
                TimerInitialMinutes = timerMinutes[timerIndex],
                TimerBonusSeconds = bonusSeconds[bonusIndex],
            }, cancellationToken: appCts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonSaveSettings_Click(object sender, EventArgs e)
    {
        SendGameSettingsAsync();
    }
}
