using MIN.Chess.Client.Forms.HelperForms;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Logic.Models.Coordination;
using MIN.Chess.Logic.Models.Moves;
using MIN.Chess.Logic.Services;
using MIN.Chess.Messages.Messages;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Client.Forms;

public partial class MainForm
{
    private Dictionary<Player, Participant?> players = [];
    private GameState gameState = null!;
    private Player? role;
    private Participant? participant;

    private int spectatorsCount;
    private int restartVote;
    private Move? lastMove;

    private bool IsGameOn => lastMove != null;

    private async void InitializeNetworking(string connectionString)
    {
        network.ConnectionFailed += (error)
            => MessageBox.Show(error, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
        network.MessageReceived += NetworkMessageReceived;
        await network.ConnectAsync(connectionString);
        await network.SendInitAsync();
    }

    private async void NetworkMessageReceived(INetworkMessage message)
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
                        mainPanel.MouseDown += MainPanel_MouseDown;
                        mainPanel.MouseMove += MainPanel_MouseMove;
                        mainPanel.MouseUp += MainPanel_MouseUp;
                        mainPanel.Resize += (_, _) => CalculateCellRectangles();

                        mainPanel.BackgroundImage = role == Player.Black
                            ? Properties.Resources.BoardReversed
                            : Properties.Resources.Board;
                        mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    }

                    premoves.Clear();

                    SyncTimerValues(initResponseMessage.WhiteMs, initResponseMessage.BlackMs, gameState.CurrentPlayer);
                    UpdateTurnColors();
                    UpdatePlayersInfo();
                    CalculateCellRectangles();

                    moveCache.Clear();
                    mainPanel.Invalidate();
                }, this);

                await network.SendPlayerLoadedAsync();
                break;

            case StateUpdateMessage stateUpdateMessage:
                gameState!.Board = Board.FromStateString(stateUpdateMessage.StateString);
                gameState.CurrentPlayer = stateUpdateMessage.CurrentTurn;
                timerEnabled = stateUpdateMessage.WhiteMs.HasValue;
                timerBonusSeconds = stateUpdateMessage.TimerBonusSeconds * 1000;

                lastMove = stateUpdateMessage.LastMove != null
                    ? MoveParser.Parse(stateUpdateMessage.LastMove, gameState.Board)
                    : null;

                premoves.Clear();

                UpdateTurnColors();
                ClearDrag();
                ClearSelection();
                CountPiecesCapturesForSelfAndOpponent();
                SyncTimerValues(stateUpdateMessage.WhiteMs, stateUpdateMessage.BlackMs, gameState.CurrentPlayer);
                RebuildDisplayBoard();

                moveCache.Clear();
                mainPanel.Invalidate();
                break;

            case MoveUpdateMessage moveUpdateMessage:
                var move = MoveParser.Parse(moveUpdateMessage.MoveUci, gameState!.Board);
                timerEnabled = moveUpdateMessage.WhiteMs.HasValue;

                gameState.MakeMove(move);
                UpdateTurnColors();
                lastMove = move;

                SyncTimerValues(moveUpdateMessage.WhiteMs, moveUpdateMessage.BlackMs, gameState.CurrentPlayer);
                CountPiecesCapturesForSelfAndOpponent();
                moveCache.Clear();
                await TryProcessPremoves();
                mainPanel.Invalidate();
                break;

            case GameResultMessage gameResultMessage:
                var loser = gameState!.CurrentPlayer;
                gameState!.CurrentPlayer = Player.None;
                countdownTimer?.Stop();
                premoves.Clear();
                uiContext.Post(_ =>
                {
                    ClearDrag();
                    ClearSelection();
                    drawQuote.Visible = false;
                    ShowGameOver(gameResultMessage.Result, loser);
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
                        UpdateTurnColors();
                        CalculateCellRectangles();
                        moveCache.Clear();
                        mainPanel.Invalidate();
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
                    gameState!.CurrentPlayer = Player.None;

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
            UpdateTurnColors();
            CountPiecesCapturesForSelfAndOpponent();

            await network.SendMoveAsync(move.ToUci());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

    private async Task TryProcessPremoves()
    {
        if (premoves.Count == 0 || gameState.CurrentPlayer != role)
        {
            return;
        }

        var (from, to, promoPiece) = premoves[0];
        premoves.RemoveAt(0);

        var moves = gameState.LegalMovesForPiece(from, role);
        var match = moves.FirstOrDefault(m => m.ToPos.Row == to.Row && m.ToPos.Column == to.Column);

        if (match != null)
        {
            if (match.Type == MoveType.PawnPromotion)
            {
                HandleMove(new PawnPromotion(from, to, promoPiece ?? PieceType.Queen));
            }
            else
            {
                HandleMove(match);
            }
        }
        else
        {
            premoves.Clear();
            moveCache.Clear();
            selectedPos = null;
        }
        RebuildDisplayBoard();
        mainPanel.Invalidate();
    }
}
