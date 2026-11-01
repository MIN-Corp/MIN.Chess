using MIN.Chess.Client.Components.Panels;

namespace MIN.Chess.Client.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            settingsTitle = new MIN.Chess.Client.Components.Labels.Heading3Label();
            mainPanel = new MIN.Chess.Client.Components.Panels.DoubleBufferedPanel();
            tableLayoutPanelSpectating = new System.Windows.Forms.TableLayoutPanel();
            spectatorsCounter = new MIN.Chess.Client.Components.Labels.Heading1Label();
            spectatorLogo = new System.Windows.Forms.PictureBox();
            tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            updateStateButton = new System.Windows.Forms.Button();
            drawButton = new System.Windows.Forms.Button();
            pauseButton = new System.Windows.Forms.Button();
            resignButton = new System.Windows.Forms.Button();
            tableLayoutPanelOpponent = new System.Windows.Forms.TableLayoutPanel();
            opponentNameWithTimer = new System.Windows.Forms.Panel();
            opponentTimerLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            opponentName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            opponentCapturesPanel = new System.Windows.Forms.Panel();
            tableLayoutPanelSelf = new System.Windows.Forms.TableLayoutPanel();
            selfNameAndTimer = new System.Windows.Forms.Panel();
            selfTimerLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            selfRoleName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            selfCapturesPanel = new System.Windows.Forms.Panel();
            changeRoleButton = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            flowLayoutPanelVotes = new System.Windows.Forms.FlowLayoutPanel();
            restartQuote = new MIN.Chess.Client.Components.Labels.Heading3Label();
            drawQuote = new MIN.Chess.Client.Components.Labels.Heading3Label();
            flowLayoutPanelSettings = new System.Windows.Forms.FlowLayoutPanel();
            panelAi = new System.Windows.Forms.Panel();
            buttonChooseAi = new MIN.Chess.Client.Components.Buttons.CommonButton();
            panelTimer = new System.Windows.Forms.Panel();
            timerTrackBar = new System.Windows.Forms.TrackBar();
            timerValueLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            buttonTimerPrevious = new System.Windows.Forms.Button();
            buttonTimerNext = new System.Windows.Forms.Button();
            timerTitle = new MIN.Chess.Client.Components.Labels.Heading3Label();
            panel1 = new System.Windows.Forms.Panel();
            bonusTrackBar = new System.Windows.Forms.TrackBar();
            timerBonusValueLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            buttonBonusPrevious = new System.Windows.Forms.Button();
            buttonBonusNext = new System.Windows.Forms.Button();
            timerBonusLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            buttonSaveSettings = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanelSpectating.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spectatorLogo).BeginInit();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanelOpponent.SuspendLayout();
            opponentNameWithTimer.SuspendLayout();
            tableLayoutPanelSelf.SuspendLayout();
            selfNameAndTimer.SuspendLayout();
            flowLayoutPanelVotes.SuspendLayout();
            flowLayoutPanelSettings.SuspendLayout();
            panelAi.SuspendLayout();
            panelTimer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)timerTrackBar).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bonusTrackBar).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            tableLayoutPanel.Controls.Add(settingsTitle, 0, 1);
            tableLayoutPanel.Controls.Add(mainPanel, 1, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelSpectating, 2, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelButtons, 2, 4);
            tableLayoutPanel.Controls.Add(tableLayoutPanelOpponent, 0, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelSelf, 0, 4);
            tableLayoutPanel.Controls.Add(changeRoleButton, 2, 1);
            tableLayoutPanel.Controls.Add(flowLayoutPanelVotes, 2, 3);
            tableLayoutPanel.Controls.Add(flowLayoutPanelSettings, 0, 2);
            tableLayoutPanel.Controls.Add(buttonSaveSettings, 0, 3);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 5;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new System.Drawing.Size(984, 579);
            tableLayoutPanel.TabIndex = 0;
            // 
            // settingsTitle
            // 
            settingsTitle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            settingsTitle.AutoSize = true;
            settingsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            settingsTitle.ForeColor = System.Drawing.Color.White;
            settingsTitle.Location = new System.Drawing.Point(20, 75);
            settingsTitle.Name = "settingsTitle";
            settingsTitle.Size = new System.Drawing.Size(139, 21);
            settingsTitle.TabIndex = 10;
            settingsTitle.Text = "Настройки игры";
            // 
            // mainPanel
            // 
            mainPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mainPanel.Location = new System.Drawing.Point(180, 0);
            mainPanel.Margin = new System.Windows.Forms.Padding(0);
            mainPanel.Name = "mainPanel";
            tableLayoutPanel.SetRowSpan(mainPanel, 5);
            mainPanel.Size = new System.Drawing.Size(624, 579);
            mainPanel.TabIndex = 0;
            // 
            // tableLayoutPanelSpectating
            // 
            tableLayoutPanelSpectating.ColumnCount = 2;
            tableLayoutPanelSpectating.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanelSpectating.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelSpectating.Controls.Add(spectatorsCounter, 0, 0);
            tableLayoutPanelSpectating.Controls.Add(spectatorLogo, 1, 0);
            tableLayoutPanelSpectating.Location = new System.Drawing.Point(807, 3);
            tableLayoutPanelSpectating.Name = "tableLayoutPanelSpectating";
            tableLayoutPanelSpectating.RowCount = 1;
            tableLayoutPanelSpectating.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelSpectating.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanelSpectating.Size = new System.Drawing.Size(174, 42);
            tableLayoutPanelSpectating.TabIndex = 5;
            // 
            // spectatorsCounter
            // 
            spectatorsCounter.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            spectatorsCounter.ForeColor = System.Drawing.Color.White;
            spectatorsCounter.Location = new System.Drawing.Point(3, 0);
            spectatorsCounter.Name = "spectatorsCounter";
            spectatorsCounter.Size = new System.Drawing.Size(26, 42);
            spectatorsCounter.TabIndex = 4;
            spectatorsCounter.Text = "0";
            spectatorsCounter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spectatorLogo
            // 
            spectatorLogo.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.eye;
            spectatorLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            spectatorLogo.Dock = System.Windows.Forms.DockStyle.Left;
            spectatorLogo.Location = new System.Drawing.Point(35, 3);
            spectatorLogo.Name = "spectatorLogo";
            spectatorLogo.Size = new System.Drawing.Size(36, 36);
            spectatorLogo.TabIndex = 5;
            spectatorLogo.TabStop = false;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 4;
            tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            tableLayoutPanelButtons.Controls.Add(updateStateButton, 2, 0);
            tableLayoutPanelButtons.Controls.Add(drawButton, 1, 0);
            tableLayoutPanelButtons.Controls.Add(pauseButton, 3, 0);
            tableLayoutPanelButtons.Controls.Add(resignButton, 0, 0);
            tableLayoutPanelButtons.Location = new System.Drawing.Point(807, 534);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new System.Drawing.Size(174, 42);
            tableLayoutPanelButtons.TabIndex = 6;
            // 
            // updateStateButton
            // 
            updateStateButton.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.download;
            updateStateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            updateStateButton.ForeColor = System.Drawing.Color.Black;
            updateStateButton.Location = new System.Drawing.Point(87, 3);
            updateStateButton.Name = "updateStateButton";
            updateStateButton.Size = new System.Drawing.Size(36, 36);
            updateStateButton.TabIndex = 12;
            updateStateButton.UseVisualStyleBackColor = true;
            updateStateButton.Click += updateStateButton_Click;
            // 
            // drawButton
            // 
            drawButton.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.draw;
            drawButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            drawButton.Dock = System.Windows.Forms.DockStyle.Fill;
            drawButton.ForeColor = System.Drawing.Color.Black;
            drawButton.Location = new System.Drawing.Point(45, 3);
            drawButton.Name = "drawButton";
            drawButton.Size = new System.Drawing.Size(36, 36);
            drawButton.TabIndex = 11;
            drawButton.UseVisualStyleBackColor = true;
            drawButton.Click += drawButton_Click;
            // 
            // pauseButton
            // 
            pauseButton.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.pause;
            pauseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pauseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            pauseButton.ForeColor = System.Drawing.Color.Black;
            pauseButton.Location = new System.Drawing.Point(135, 3);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new System.Drawing.Size(36, 36);
            pauseButton.TabIndex = 10;
            pauseButton.UseVisualStyleBackColor = true;
            pauseButton.Click += pauseButton_Click;
            // 
            // resignButton
            // 
            resignButton.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.resign;
            resignButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            resignButton.Dock = System.Windows.Forms.DockStyle.Fill;
            resignButton.ForeColor = System.Drawing.Color.Black;
            resignButton.Location = new System.Drawing.Point(3, 3);
            resignButton.Name = "resignButton";
            resignButton.Size = new System.Drawing.Size(36, 36);
            resignButton.TabIndex = 9;
            resignButton.UseVisualStyleBackColor = true;
            resignButton.Click += resignButton_Click;
            // 
            // tableLayoutPanelOpponent
            // 
            tableLayoutPanelOpponent.ColumnCount = 1;
            tableLayoutPanelOpponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelOpponent.Controls.Add(opponentNameWithTimer, 0, 0);
            tableLayoutPanelOpponent.Controls.Add(opponentCapturesPanel, 0, 1);
            tableLayoutPanelOpponent.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanelOpponent.Name = "tableLayoutPanelOpponent";
            tableLayoutPanelOpponent.RowCount = 2;
            tableLayoutPanelOpponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanelOpponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelOpponent.Size = new System.Drawing.Size(174, 42);
            tableLayoutPanelOpponent.TabIndex = 7;
            // 
            // opponentNameWithTimer
            // 
            opponentNameWithTimer.Controls.Add(opponentTimerLabel);
            opponentNameWithTimer.Controls.Add(opponentName);
            opponentNameWithTimer.Location = new System.Drawing.Point(0, 0);
            opponentNameWithTimer.Margin = new System.Windows.Forms.Padding(0);
            opponentNameWithTimer.Name = "opponentNameWithTimer";
            opponentNameWithTimer.Size = new System.Drawing.Size(174, 20);
            opponentNameWithTimer.TabIndex = 15;
            // 
            // opponentTimerLabel
            // 
            opponentTimerLabel.AutoSize = true;
            opponentTimerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            opponentTimerLabel.ForeColor = System.Drawing.Color.White;
            opponentTimerLabel.Location = new System.Drawing.Point(130, -1);
            opponentTimerLabel.Name = "opponentTimerLabel";
            opponentTimerLabel.Size = new System.Drawing.Size(38, 21);
            opponentTimerLabel.TabIndex = 3;
            opponentTimerLabel.Text = "--:--";
            // 
            // opponentName
            // 
            opponentName.AutoSize = true;
            opponentName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            opponentName.ForeColor = System.Drawing.Color.White;
            opponentName.Location = new System.Drawing.Point(0, 0);
            opponentName.Name = "opponentName";
            opponentName.Size = new System.Drawing.Size(104, 21);
            opponentName.TabIndex = 2;
            opponentName.Text = "Загрузка . . .";
            // 
            // opponentCapturesPanel
            // 
            opponentCapturesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            opponentCapturesPanel.Location = new System.Drawing.Point(0, 20);
            opponentCapturesPanel.Margin = new System.Windows.Forms.Padding(0);
            opponentCapturesPanel.Name = "opponentCapturesPanel";
            opponentCapturesPanel.Size = new System.Drawing.Size(174, 22);
            opponentCapturesPanel.TabIndex = 16;
            opponentCapturesPanel.Paint += PanelCaptures_Paint;
            // 
            // tableLayoutPanelSelf
            // 
            tableLayoutPanelSelf.ColumnCount = 1;
            tableLayoutPanelSelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelSelf.Controls.Add(selfNameAndTimer, 0, 0);
            tableLayoutPanelSelf.Controls.Add(selfCapturesPanel, 0, 1);
            tableLayoutPanelSelf.Location = new System.Drawing.Point(3, 534);
            tableLayoutPanelSelf.Name = "tableLayoutPanelSelf";
            tableLayoutPanelSelf.RowCount = 2;
            tableLayoutPanelSelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanelSelf.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanelSelf.Size = new System.Drawing.Size(174, 42);
            tableLayoutPanelSelf.TabIndex = 8;
            // 
            // selfNameAndTimer
            // 
            selfNameAndTimer.Controls.Add(selfTimerLabel);
            selfNameAndTimer.Controls.Add(selfRoleName);
            selfNameAndTimer.Location = new System.Drawing.Point(0, 0);
            selfNameAndTimer.Margin = new System.Windows.Forms.Padding(0);
            selfNameAndTimer.Name = "selfNameAndTimer";
            selfNameAndTimer.Size = new System.Drawing.Size(174, 20);
            selfNameAndTimer.TabIndex = 16;
            // 
            // selfTimerLabel
            // 
            selfTimerLabel.AutoSize = true;
            selfTimerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            selfTimerLabel.ForeColor = System.Drawing.Color.White;
            selfTimerLabel.Location = new System.Drawing.Point(130, -3);
            selfTimerLabel.Name = "selfTimerLabel";
            selfTimerLabel.Size = new System.Drawing.Size(38, 21);
            selfTimerLabel.TabIndex = 3;
            selfTimerLabel.Text = "--:--";
            // 
            // selfRoleName
            // 
            selfRoleName.AutoSize = true;
            selfRoleName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            selfRoleName.ForeColor = System.Drawing.Color.White;
            selfRoleName.Location = new System.Drawing.Point(0, -1);
            selfRoleName.Name = "selfRoleName";
            selfRoleName.Size = new System.Drawing.Size(104, 21);
            selfRoleName.TabIndex = 1;
            selfRoleName.Text = "Загрузка . . .";
            // 
            // selfCapturesPanel
            // 
            selfCapturesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            selfCapturesPanel.Location = new System.Drawing.Point(0, 20);
            selfCapturesPanel.Margin = new System.Windows.Forms.Padding(0);
            selfCapturesPanel.Name = "selfCapturesPanel";
            selfCapturesPanel.Size = new System.Drawing.Size(174, 22);
            selfCapturesPanel.TabIndex = 17;
            selfCapturesPanel.Paint += PanelCaptures_Paint;
            // 
            // changeRoleButton
            // 
            changeRoleButton.BackColor = System.Drawing.Color.FromArgb(((int)((byte)248)), ((int)((byte)249)), ((int)((byte)255)));
            changeRoleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            changeRoleButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            changeRoleButton.FlatAppearance.BorderSize = 2;
            changeRoleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            changeRoleButton.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            changeRoleButton.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            changeRoleButton.Location = new System.Drawing.Point(807, 51);
            changeRoleButton.Name = "changeRoleButton";
            changeRoleButton.Size = new System.Drawing.Size(174, 42);
            changeRoleButton.TabIndex = 11;
            changeRoleButton.Text = "Войти в игру";
            changeRoleButton.UseVisualStyleBackColor = false;
            changeRoleButton.Visible = false;
            changeRoleButton.Click += changeRoleButton_Click;
            // 
            // flowLayoutPanelVotes
            // 
            flowLayoutPanelVotes.Controls.Add(restartQuote);
            flowLayoutPanelVotes.Controls.Add(drawQuote);
            flowLayoutPanelVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanelVotes.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            flowLayoutPanelVotes.Location = new System.Drawing.Point(804, 483);
            flowLayoutPanelVotes.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanelVotes.Name = "flowLayoutPanelVotes";
            flowLayoutPanelVotes.Size = new System.Drawing.Size(180, 48);
            flowLayoutPanelVotes.TabIndex = 12;
            // 
            // restartQuote
            // 
            restartQuote.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            restartQuote.AutoSize = true;
            restartQuote.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            restartQuote.ForeColor = System.Drawing.Color.White;
            restartQuote.Location = new System.Drawing.Point(3, 27);
            restartQuote.Name = "restartQuote";
            restartQuote.Size = new System.Drawing.Size(118, 21);
            restartQuote.TabIndex = 9;
            restartQuote.Text = "Рестарт? (1/2)";
            restartQuote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            restartQuote.Visible = false;
            restartQuote.Click += restartQuote_Click;
            // 
            // drawQuote
            // 
            drawQuote.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            drawQuote.AutoSize = true;
            drawQuote.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            drawQuote.ForeColor = System.Drawing.Color.White;
            drawQuote.Location = new System.Drawing.Point(8, 6);
            drawQuote.Name = "drawQuote";
            drawQuote.Size = new System.Drawing.Size(108, 21);
            drawQuote.TabIndex = 10;
            drawQuote.Text = "Ничья? (1/2)";
            drawQuote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            drawQuote.Visible = false;
            // 
            // flowLayoutPanelSettings
            // 
            flowLayoutPanelSettings.Controls.Add(panelAi);
            flowLayoutPanelSettings.Controls.Add(panelTimer);
            flowLayoutPanelSettings.Controls.Add(panel1);
            flowLayoutPanelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanelSettings.Location = new System.Drawing.Point(3, 99);
            flowLayoutPanelSettings.Name = "flowLayoutPanelSettings";
            flowLayoutPanelSettings.Size = new System.Drawing.Size(174, 381);
            flowLayoutPanelSettings.TabIndex = 13;
            // 
            // panelAi
            // 
            panelAi.Controls.Add(buttonChooseAi);
            panelAi.Dock = System.Windows.Forms.DockStyle.Top;
            panelAi.Location = new System.Drawing.Point(3, 3);
            panelAi.Name = "panelAi";
            panelAi.Size = new System.Drawing.Size(171, 55);
            panelAi.TabIndex = 2;
            // 
            // buttonChooseAi
            // 
            buttonChooseAi.BackColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            buttonChooseAi.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)228)), ((int)((byte)230)), ((int)((byte)240)));
            buttonChooseAi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonChooseAi.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            buttonChooseAi.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)248)), ((int)((byte)249)), ((int)((byte)255)));
            buttonChooseAi.Location = new System.Drawing.Point(6, 12);
            buttonChooseAi.Name = "buttonChooseAi";
            buttonChooseAi.Size = new System.Drawing.Size(159, 32);
            buttonChooseAi.TabIndex = 0;
            buttonChooseAi.Text = "Сыграть с ИИ";
            buttonChooseAi.UseVisualStyleBackColor = false;
            buttonChooseAi.Click += buttonChooseAi_Click;
            // 
            // panelTimer
            // 
            panelTimer.Controls.Add(timerTrackBar);
            panelTimer.Controls.Add(timerValueLabel);
            panelTimer.Controls.Add(buttonTimerPrevious);
            panelTimer.Controls.Add(buttonTimerNext);
            panelTimer.Controls.Add(timerTitle);
            panelTimer.Dock = System.Windows.Forms.DockStyle.Top;
            panelTimer.Location = new System.Drawing.Point(3, 64);
            panelTimer.Name = "panelTimer";
            panelTimer.Size = new System.Drawing.Size(171, 98);
            panelTimer.TabIndex = 0;
            // 
            // timerTrackBar
            // 
            timerTrackBar.Location = new System.Drawing.Point(0, 55);
            timerTrackBar.Name = "timerTrackBar";
            timerTrackBar.Size = new System.Drawing.Size(168, 45);
            timerTrackBar.TabIndex = 15;
            timerTrackBar.ValueChanged += timerTrackBar_ValueChanged;
            // 
            // timerValueLabel
            // 
            timerValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            timerValueLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)204));
            timerValueLabel.ForeColor = System.Drawing.Color.White;
            timerValueLabel.Location = new System.Drawing.Point(42, 30);
            timerValueLabel.Name = "timerValueLabel";
            timerValueLabel.Size = new System.Drawing.Size(87, 21);
            timerValueLabel.TabIndex = 14;
            timerValueLabel.Text = "-";
            timerValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonTimerPrevious
            // 
            buttonTimerPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            buttonTimerPrevious.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            buttonTimerPrevious.ForeColor = System.Drawing.Color.Black;
            buttonTimerPrevious.Location = new System.Drawing.Point(0, 22);
            buttonTimerPrevious.Name = "buttonTimerPrevious";
            buttonTimerPrevious.Size = new System.Drawing.Size(36, 36);
            buttonTimerPrevious.TabIndex = 13;
            buttonTimerPrevious.Text = "<";
            buttonTimerPrevious.UseVisualStyleBackColor = true;
            buttonTimerPrevious.Click += buttonTimerPrevious_Click;
            // 
            // buttonTimerNext
            // 
            buttonTimerNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            buttonTimerNext.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            buttonTimerNext.ForeColor = System.Drawing.Color.Black;
            buttonTimerNext.Location = new System.Drawing.Point(135, 22);
            buttonTimerNext.Name = "buttonTimerNext";
            buttonTimerNext.Size = new System.Drawing.Size(36, 36);
            buttonTimerNext.TabIndex = 12;
            buttonTimerNext.Text = ">";
            buttonTimerNext.UseVisualStyleBackColor = true;
            buttonTimerNext.Click += buttonTimerNext_Click;
            // 
            // timerTitle
            // 
            timerTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            timerTitle.AutoSize = true;
            timerTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            timerTitle.ForeColor = System.Drawing.Color.White;
            timerTitle.Location = new System.Drawing.Point(48, 0);
            timerTitle.Name = "timerTitle";
            timerTitle.Size = new System.Drawing.Size(68, 21);
            timerTitle.TabIndex = 11;
            timerTitle.Text = "Таймер";
            // 
            // panel1
            // 
            panel1.Controls.Add(bonusTrackBar);
            panel1.Controls.Add(timerBonusValueLabel);
            panel1.Controls.Add(buttonBonusPrevious);
            panel1.Controls.Add(buttonBonusNext);
            panel1.Controls.Add(timerBonusLabel);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(3, 168);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(171, 100);
            panel1.TabIndex = 1;
            // 
            // bonusTrackBar
            // 
            bonusTrackBar.Location = new System.Drawing.Point(0, 64);
            bonusTrackBar.Name = "bonusTrackBar";
            bonusTrackBar.Size = new System.Drawing.Size(168, 45);
            bonusTrackBar.TabIndex = 16;
            bonusTrackBar.ValueChanged += bonusTrackBar_ValueChanged;
            // 
            // timerBonusValueLabel
            // 
            timerBonusValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            timerBonusValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            timerBonusValueLabel.ForeColor = System.Drawing.Color.White;
            timerBonusValueLabel.Location = new System.Drawing.Point(42, 31);
            timerBonusValueLabel.Name = "timerBonusValueLabel";
            timerBonusValueLabel.Size = new System.Drawing.Size(87, 21);
            timerBonusValueLabel.TabIndex = 14;
            timerBonusValueLabel.Text = "0";
            timerBonusValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonBonusPrevious
            // 
            buttonBonusPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            buttonBonusPrevious.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            buttonBonusPrevious.ForeColor = System.Drawing.Color.Black;
            buttonBonusPrevious.Location = new System.Drawing.Point(0, 22);
            buttonBonusPrevious.Name = "buttonBonusPrevious";
            buttonBonusPrevious.Size = new System.Drawing.Size(36, 36);
            buttonBonusPrevious.TabIndex = 13;
            buttonBonusPrevious.Text = "<";
            buttonBonusPrevious.UseVisualStyleBackColor = true;
            buttonBonusPrevious.Click += buttonBonusPrevious_Click;
            // 
            // buttonBonusNext
            // 
            buttonBonusNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            buttonBonusNext.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            buttonBonusNext.ForeColor = System.Drawing.Color.Black;
            buttonBonusNext.Location = new System.Drawing.Point(135, 22);
            buttonBonusNext.Name = "buttonBonusNext";
            buttonBonusNext.Size = new System.Drawing.Size(36, 36);
            buttonBonusNext.TabIndex = 12;
            buttonBonusNext.Text = ">";
            buttonBonusNext.UseVisualStyleBackColor = true;
            buttonBonusNext.Click += buttonBonusNext_Click;
            // 
            // timerBonusLabel
            // 
            timerBonusLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            timerBonusLabel.AutoSize = true;
            timerBonusLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            timerBonusLabel.ForeColor = System.Drawing.Color.White;
            timerBonusLabel.Location = new System.Drawing.Point(-3, 0);
            timerBonusLabel.Name = "timerBonusLabel";
            timerBonusLabel.Size = new System.Drawing.Size(181, 21);
            timerBonusLabel.TabIndex = 11;
            timerBonusLabel.Text = "Добавление времени";
            // 
            // buttonSaveSettings
            // 
            buttonSaveSettings.BackColor = System.Drawing.Color.FromArgb(((int)((byte)248)), ((int)((byte)249)), ((int)((byte)255)));
            buttonSaveSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonSaveSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            buttonSaveSettings.FlatAppearance.BorderSize = 2;
            buttonSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSaveSettings.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            buttonSaveSettings.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            buttonSaveSettings.Location = new System.Drawing.Point(3, 486);
            buttonSaveSettings.Name = "buttonSaveSettings";
            buttonSaveSettings.Size = new System.Drawing.Size(174, 42);
            buttonSaveSettings.TabIndex = 14;
            buttonSaveSettings.Text = "Сохранить настройки";
            buttonSaveSettings.UseVisualStyleBackColor = false;
            buttonSaveSettings.Click += buttonSaveSettings_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            ClientSize = new System.Drawing.Size(984, 579);
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
            MinimumSize = new System.Drawing.Size(540, 200);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MIN - Шахматы";
            KeyDown += MainForm_KeyDown;
            Resize += MainForm_Resize;
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            tableLayoutPanelSpectating.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spectatorLogo).EndInit();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelOpponent.ResumeLayout(false);
            opponentNameWithTimer.ResumeLayout(false);
            opponentNameWithTimer.PerformLayout();
            tableLayoutPanelSelf.ResumeLayout(false);
            selfNameAndTimer.ResumeLayout(false);
            selfNameAndTimer.PerformLayout();
            flowLayoutPanelVotes.ResumeLayout(false);
            flowLayoutPanelVotes.PerformLayout();
            flowLayoutPanelSettings.ResumeLayout(false);
            panelAi.ResumeLayout(false);
            panelTimer.ResumeLayout(false);
            panelTimer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)timerTrackBar).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bonusTrackBar).EndInit();
            ResumeLayout(false);
        }

        private MIN.Chess.Client.Components.Buttons.CommonButton buttonChooseAi;
        private Panel panelAi;

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private DoubleBufferedPanel mainPanel;
        private Components.Labels.Heading3Label opponentName;
        private Components.Labels.Heading3Label selfRoleName;
        private Components.Labels.Heading1Label spectatorsCounter;
        private PictureBox spectatorLogo;
        private TableLayoutPanel tableLayoutPanelButtons;
        private TableLayoutPanel tableLayoutPanelSpectating;
        private TableLayoutPanel tableLayoutPanelOpponent;
        private TableLayoutPanel tableLayoutPanelSelf;
        private Button resignButton;
        private Button pauseButton;
        private Components.Labels.Heading3Label restartQuote;
        private Components.Labels.Heading3Label settingsTitle;
        private Components.Buttons.InvertedButton changeRoleButton;
        private Button drawButton;
        private FlowLayoutPanel flowLayoutPanelVotes;
        private Components.Labels.Heading3Label drawQuote;
        private FlowLayoutPanel flowLayoutPanelSettings;
        private System.Windows.Forms.Panel panelTimer;
        private Components.Labels.Heading3Label timerTitle;
        private Button buttonTimerNext;
        private Button buttonTimerPrevious;
        private Components.Labels.Heading3Label timerValueLabel;
        private System.Windows.Forms.Panel panel1;
        private Components.Labels.Heading3Label timerBonusValueLabel;
        private Button buttonBonusPrevious;
        private Button buttonBonusNext;
        private Components.Labels.Heading3Label timerBonusLabel;
        private TrackBar timerTrackBar;
        private TrackBar bonusTrackBar;
        private Components.Buttons.InvertedButton buttonSaveSettings;
        private Panel opponentNameWithTimer;
        private Components.Labels.Heading3Label opponentTimerLabel;
        private Panel selfNameAndTimer;
        private Components.Labels.Heading3Label selfTimerLabel;
        private Panel selfCapturesPanel;
        private Panel opponentCapturesPanel;
        private Button updateStateButton;
    }
}
