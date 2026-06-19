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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tableLayoutPanel = new TableLayoutPanel();
            settingsTitle = new MIN.Chess.Client.Components.Labels.Heading3Label();
            mainPanel = new DoubleBufferedPanel();
            tableLayoutPanelSpectating = new TableLayoutPanel();
            spectatorsCounter = new MIN.Chess.Client.Components.Labels.Heading1Label();
            spectatorLogo = new PictureBox();
            tableLayoutPanelButtons = new TableLayoutPanel();
            drawButton = new Button();
            pauseButton = new Button();
            resignButton = new Button();
            tableLayoutPanelOpponent = new TableLayoutPanel();
            opponentNameWithTimer = new Panel();
            opponentTimerLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            opponentName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            opponentCapturesPanel = new Panel();
            tableLayoutPanelSelf = new TableLayoutPanel();
            selfNameAndTimer = new Panel();
            selfTimerLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            selfRoleName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            selfCapturesPanel = new Panel();
            changeRoleButton = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            flowLayoutPanelVotes = new FlowLayoutPanel();
            restartQuote = new MIN.Chess.Client.Components.Labels.Heading3Label();
            drawQuote = new MIN.Chess.Client.Components.Labels.Heading3Label();
            flowLayoutPanelSettings = new FlowLayoutPanel();
            panelTimer = new Panel();
            timerTrackBar = new TrackBar();
            timerValueLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            buttonTimerPrevious = new Button();
            buttonTimerNext = new Button();
            timerTitle = new MIN.Chess.Client.Components.Labels.Heading3Label();
            panel1 = new Panel();
            bonusTrackBar = new TrackBar();
            timerBonusValueLabel = new MIN.Chess.Client.Components.Labels.Heading3Label();
            buttonBonusPrevious = new Button();
            buttonBonusNext = new Button();
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
            panelTimer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)timerTrackBar).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bonusTrackBar).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
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
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 5;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new Size(984, 579);
            tableLayoutPanel.TabIndex = 0;
            // 
            // settingsTitle
            // 
            settingsTitle.Anchor = AnchorStyles.Bottom;
            settingsTitle.AutoSize = true;
            settingsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            settingsTitle.ForeColor = Color.White;
            settingsTitle.Location = new Point(20, 75);
            settingsTitle.Name = "settingsTitle";
            settingsTitle.Size = new Size(139, 21);
            settingsTitle.TabIndex = 10;
            settingsTitle.Text = "Настройки игры";
            // 
            // mainPanel
            // 
            mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(180, 0);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            tableLayoutPanel.SetRowSpan(mainPanel, 5);
            mainPanel.Size = new Size(624, 579);
            mainPanel.TabIndex = 0;
            // 
            // tableLayoutPanelSpectating
            // 
            tableLayoutPanelSpectating.ColumnCount = 2;
            tableLayoutPanelSpectating.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanelSpectating.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSpectating.Controls.Add(spectatorsCounter, 0, 0);
            tableLayoutPanelSpectating.Controls.Add(spectatorLogo, 1, 0);
            tableLayoutPanelSpectating.Location = new Point(807, 3);
            tableLayoutPanelSpectating.Name = "tableLayoutPanelSpectating";
            tableLayoutPanelSpectating.RowCount = 1;
            tableLayoutPanelSpectating.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSpectating.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelSpectating.Size = new Size(174, 42);
            tableLayoutPanelSpectating.TabIndex = 5;
            // 
            // spectatorsCounter
            // 
            spectatorsCounter.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            spectatorsCounter.ForeColor = Color.White;
            spectatorsCounter.Location = new Point(3, 0);
            spectatorsCounter.Name = "spectatorsCounter";
            spectatorsCounter.Size = new Size(26, 42);
            spectatorsCounter.TabIndex = 4;
            spectatorsCounter.Text = "0";
            spectatorsCounter.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // spectatorLogo
            // 
            spectatorLogo.BackgroundImage = Properties.Resources.eye;
            spectatorLogo.BackgroundImageLayout = ImageLayout.Zoom;
            spectatorLogo.Dock = DockStyle.Left;
            spectatorLogo.Location = new Point(35, 3);
            spectatorLogo.Name = "spectatorLogo";
            spectatorLogo.Size = new Size(36, 36);
            spectatorLogo.TabIndex = 5;
            spectatorLogo.TabStop = false;
            // 
            // tableLayoutPanelButtons
            // 
            tableLayoutPanelButtons.ColumnCount = 4;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tableLayoutPanelButtons.Controls.Add(drawButton, 1, 0);
            tableLayoutPanelButtons.Controls.Add(pauseButton, 3, 0);
            tableLayoutPanelButtons.Controls.Add(resignButton, 0, 0);
            tableLayoutPanelButtons.Location = new Point(807, 534);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new Size(174, 42);
            tableLayoutPanelButtons.TabIndex = 6;
            // 
            // drawButton
            // 
            drawButton.BackgroundImage = Properties.Resources.draw;
            drawButton.BackgroundImageLayout = ImageLayout.Zoom;
            drawButton.Dock = DockStyle.Fill;
            drawButton.ForeColor = Color.Black;
            drawButton.Location = new Point(45, 3);
            drawButton.Name = "drawButton";
            drawButton.Size = new Size(36, 36);
            drawButton.TabIndex = 11;
            drawButton.UseVisualStyleBackColor = true;
            drawButton.Click += drawButton_Click;
            // 
            // pauseButton
            // 
            pauseButton.BackgroundImage = Properties.Resources.pause;
            pauseButton.BackgroundImageLayout = ImageLayout.Zoom;
            pauseButton.Dock = DockStyle.Fill;
            pauseButton.ForeColor = Color.Black;
            pauseButton.Location = new Point(135, 3);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(36, 36);
            pauseButton.TabIndex = 10;
            pauseButton.UseVisualStyleBackColor = true;
            pauseButton.Click += pauseButton_Click;
            // 
            // resignButton
            // 
            resignButton.BackgroundImage = Properties.Resources.resign;
            resignButton.BackgroundImageLayout = ImageLayout.Zoom;
            resignButton.Dock = DockStyle.Fill;
            resignButton.ForeColor = Color.Black;
            resignButton.Location = new Point(3, 3);
            resignButton.Name = "resignButton";
            resignButton.Size = new Size(36, 36);
            resignButton.TabIndex = 9;
            resignButton.UseVisualStyleBackColor = true;
            resignButton.Click += resignButton_Click;
            // 
            // tableLayoutPanelOpponent
            // 
            tableLayoutPanelOpponent.ColumnCount = 1;
            tableLayoutPanelOpponent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelOpponent.Controls.Add(opponentNameWithTimer, 0, 0);
            tableLayoutPanelOpponent.Controls.Add(opponentCapturesPanel, 0, 1);
            tableLayoutPanelOpponent.Location = new Point(3, 3);
            tableLayoutPanelOpponent.Name = "tableLayoutPanelOpponent";
            tableLayoutPanelOpponent.RowCount = 2;
            tableLayoutPanelOpponent.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelOpponent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelOpponent.Size = new Size(174, 42);
            tableLayoutPanelOpponent.TabIndex = 7;
            // 
            // opponentNameWithTimer
            // 
            opponentNameWithTimer.Controls.Add(opponentTimerLabel);
            opponentNameWithTimer.Controls.Add(opponentName);
            opponentNameWithTimer.Location = new Point(0, 0);
            opponentNameWithTimer.Margin = new Padding(0);
            opponentNameWithTimer.Name = "opponentNameWithTimer";
            opponentNameWithTimer.Size = new Size(174, 20);
            opponentNameWithTimer.TabIndex = 15;
            // 
            // opponentTimerLabel
            // 
            opponentTimerLabel.AutoSize = true;
            opponentTimerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            opponentTimerLabel.ForeColor = Color.White;
            opponentTimerLabel.Location = new Point(130, -1);
            opponentTimerLabel.Name = "opponentTimerLabel";
            opponentTimerLabel.Size = new Size(38, 21);
            opponentTimerLabel.TabIndex = 3;
            opponentTimerLabel.Text = "--:--";
            // 
            // opponentName
            // 
            opponentName.AutoSize = true;
            opponentName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            opponentName.ForeColor = Color.White;
            opponentName.Location = new Point(0, 0);
            opponentName.Name = "opponentName";
            opponentName.Size = new Size(104, 21);
            opponentName.TabIndex = 2;
            opponentName.Text = "Загрузка . . .";
            // 
            // opponentCapturesPanel
            // 
            opponentCapturesPanel.Dock = DockStyle.Fill;
            opponentCapturesPanel.Location = new Point(0, 20);
            opponentCapturesPanel.Margin = new Padding(0);
            opponentCapturesPanel.Name = "opponentCapturesPanel";
            opponentCapturesPanel.Size = new Size(174, 22);
            opponentCapturesPanel.TabIndex = 16;
            opponentCapturesPanel.Paint += PanelCaptures_Paint;
            // 
            // tableLayoutPanelSelf
            // 
            tableLayoutPanelSelf.ColumnCount = 1;
            tableLayoutPanelSelf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSelf.Controls.Add(selfNameAndTimer, 0, 0);
            tableLayoutPanelSelf.Controls.Add(selfCapturesPanel, 0, 1);
            tableLayoutPanelSelf.Location = new Point(3, 534);
            tableLayoutPanelSelf.Name = "tableLayoutPanelSelf";
            tableLayoutPanelSelf.RowCount = 2;
            tableLayoutPanelSelf.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelSelf.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSelf.Size = new Size(174, 42);
            tableLayoutPanelSelf.TabIndex = 8;
            // 
            // selfNameAndTimer
            // 
            selfNameAndTimer.Controls.Add(selfTimerLabel);
            selfNameAndTimer.Controls.Add(selfRoleName);
            selfNameAndTimer.Location = new Point(0, 0);
            selfNameAndTimer.Margin = new Padding(0);
            selfNameAndTimer.Name = "selfNameAndTimer";
            selfNameAndTimer.Size = new Size(174, 20);
            selfNameAndTimer.TabIndex = 16;
            // 
            // selfTimerLabel
            // 
            selfTimerLabel.AutoSize = true;
            selfTimerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            selfTimerLabel.ForeColor = Color.White;
            selfTimerLabel.Location = new Point(130, -3);
            selfTimerLabel.Name = "selfTimerLabel";
            selfTimerLabel.Size = new Size(38, 21);
            selfTimerLabel.TabIndex = 3;
            selfTimerLabel.Text = "--:--";
            // 
            // selfRoleName
            // 
            selfRoleName.AutoSize = true;
            selfRoleName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            selfRoleName.ForeColor = Color.White;
            selfRoleName.Location = new Point(0, -1);
            selfRoleName.Name = "selfRoleName";
            selfRoleName.Size = new Size(104, 21);
            selfRoleName.TabIndex = 1;
            selfRoleName.Text = "Загрузка . . .";
            // 
            // selfCapturesPanel
            // 
            selfCapturesPanel.Dock = DockStyle.Fill;
            selfCapturesPanel.Location = new Point(0, 20);
            selfCapturesPanel.Margin = new Padding(0);
            selfCapturesPanel.Name = "selfCapturesPanel";
            selfCapturesPanel.Size = new Size(174, 22);
            selfCapturesPanel.TabIndex = 17;
            selfCapturesPanel.Paint += PanelCaptures_Paint;
            // 
            // changeRoleButton
            // 
            changeRoleButton.BackColor = Color.FromArgb(248, 249, 255);
            changeRoleButton.Dock = DockStyle.Fill;
            changeRoleButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            changeRoleButton.FlatAppearance.BorderSize = 2;
            changeRoleButton.FlatStyle = FlatStyle.Flat;
            changeRoleButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            changeRoleButton.ForeColor = Color.FromArgb(167, 157, 255);
            changeRoleButton.Location = new Point(807, 51);
            changeRoleButton.Name = "changeRoleButton";
            changeRoleButton.Size = new Size(174, 42);
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
            flowLayoutPanelVotes.Dock = DockStyle.Fill;
            flowLayoutPanelVotes.FlowDirection = FlowDirection.BottomUp;
            flowLayoutPanelVotes.Location = new Point(804, 483);
            flowLayoutPanelVotes.Margin = new Padding(0);
            flowLayoutPanelVotes.Name = "flowLayoutPanelVotes";
            flowLayoutPanelVotes.Size = new Size(180, 48);
            flowLayoutPanelVotes.TabIndex = 12;
            // 
            // restartQuote
            // 
            restartQuote.Anchor = AnchorStyles.Bottom;
            restartQuote.AutoSize = true;
            restartQuote.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            restartQuote.ForeColor = Color.White;
            restartQuote.Location = new Point(3, 27);
            restartQuote.Name = "restartQuote";
            restartQuote.Size = new Size(118, 21);
            restartQuote.TabIndex = 9;
            restartQuote.Text = "Рестарт? (1/2)";
            restartQuote.TextAlign = ContentAlignment.MiddleLeft;
            restartQuote.Visible = false;
            restartQuote.Click += restartQuote_Click;
            // 
            // drawQuote
            // 
            drawQuote.Anchor = AnchorStyles.Bottom;
            drawQuote.AutoSize = true;
            drawQuote.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            drawQuote.ForeColor = Color.White;
            drawQuote.Location = new Point(8, 6);
            drawQuote.Name = "drawQuote";
            drawQuote.Size = new Size(108, 21);
            drawQuote.TabIndex = 10;
            drawQuote.Text = "Ничья? (1/2)";
            drawQuote.TextAlign = ContentAlignment.MiddleLeft;
            drawQuote.Visible = false;
            // 
            // flowLayoutPanelSettings
            // 
            flowLayoutPanelSettings.Controls.Add(panelTimer);
            flowLayoutPanelSettings.Controls.Add(panel1);
            flowLayoutPanelSettings.Dock = DockStyle.Fill;
            flowLayoutPanelSettings.Location = new Point(3, 99);
            flowLayoutPanelSettings.Name = "flowLayoutPanelSettings";
            flowLayoutPanelSettings.Size = new Size(174, 381);
            flowLayoutPanelSettings.TabIndex = 13;
            // 
            // panelTimer
            // 
            panelTimer.Controls.Add(timerTrackBar);
            panelTimer.Controls.Add(timerValueLabel);
            panelTimer.Controls.Add(buttonTimerPrevious);
            panelTimer.Controls.Add(buttonTimerNext);
            panelTimer.Controls.Add(timerTitle);
            panelTimer.Dock = DockStyle.Top;
            panelTimer.Location = new Point(3, 3);
            panelTimer.Name = "panelTimer";
            panelTimer.Size = new Size(171, 98);
            panelTimer.TabIndex = 0;
            // 
            // timerTrackBar
            // 
            timerTrackBar.Location = new Point(0, 55);
            timerTrackBar.Name = "timerTrackBar";
            timerTrackBar.Size = new Size(168, 45);
            timerTrackBar.TabIndex = 15;
            timerTrackBar.ValueChanged += timerTrackBar_ValueChanged;
            // 
            // timerValueLabel
            // 
            timerValueLabel.Anchor = AnchorStyles.Top;
            timerValueLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            timerValueLabel.ForeColor = Color.White;
            timerValueLabel.Location = new Point(42, 30);
            timerValueLabel.Name = "timerValueLabel";
            timerValueLabel.Size = new Size(87, 21);
            timerValueLabel.TabIndex = 14;
            timerValueLabel.Text = "-";
            timerValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonTimerPrevious
            // 
            buttonTimerPrevious.BackgroundImageLayout = ImageLayout.Zoom;
            buttonTimerPrevious.Font = new Font("Segoe UI Emoji", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonTimerPrevious.ForeColor = Color.Black;
            buttonTimerPrevious.Location = new Point(0, 22);
            buttonTimerPrevious.Name = "buttonTimerPrevious";
            buttonTimerPrevious.Size = new Size(36, 36);
            buttonTimerPrevious.TabIndex = 13;
            buttonTimerPrevious.Text = "<";
            buttonTimerPrevious.UseVisualStyleBackColor = true;
            buttonTimerPrevious.Click += buttonTimerPrevious_Click;
            // 
            // buttonTimerNext
            // 
            buttonTimerNext.BackgroundImageLayout = ImageLayout.Zoom;
            buttonTimerNext.Font = new Font("Segoe UI Emoji", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonTimerNext.ForeColor = Color.Black;
            buttonTimerNext.Location = new Point(135, 22);
            buttonTimerNext.Name = "buttonTimerNext";
            buttonTimerNext.Size = new Size(36, 36);
            buttonTimerNext.TabIndex = 12;
            buttonTimerNext.Text = ">";
            buttonTimerNext.UseVisualStyleBackColor = true;
            buttonTimerNext.Click += buttonTimerNext_Click;
            // 
            // timerTitle
            // 
            timerTitle.Anchor = AnchorStyles.Top;
            timerTitle.AutoSize = true;
            timerTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            timerTitle.ForeColor = Color.White;
            timerTitle.Location = new Point(48, 0);
            timerTitle.Name = "timerTitle";
            timerTitle.Size = new Size(68, 21);
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
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(3, 107);
            panel1.Name = "panel1";
            panel1.Size = new Size(171, 100);
            panel1.TabIndex = 1;
            // 
            // bonusTrackBar
            // 
            bonusTrackBar.Location = new Point(0, 64);
            bonusTrackBar.Name = "bonusTrackBar";
            bonusTrackBar.Size = new Size(168, 45);
            bonusTrackBar.TabIndex = 16;
            bonusTrackBar.ValueChanged += bonusTrackBar_ValueChanged;
            // 
            // timerBonusValueLabel
            // 
            timerBonusValueLabel.Anchor = AnchorStyles.Top;
            timerBonusValueLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            timerBonusValueLabel.ForeColor = Color.White;
            timerBonusValueLabel.Location = new Point(42, 31);
            timerBonusValueLabel.Name = "timerBonusValueLabel";
            timerBonusValueLabel.Size = new Size(87, 21);
            timerBonusValueLabel.TabIndex = 14;
            timerBonusValueLabel.Text = "0";
            timerBonusValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonBonusPrevious
            // 
            buttonBonusPrevious.BackgroundImageLayout = ImageLayout.Zoom;
            buttonBonusPrevious.Font = new Font("Segoe UI Emoji", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonBonusPrevious.ForeColor = Color.Black;
            buttonBonusPrevious.Location = new Point(0, 22);
            buttonBonusPrevious.Name = "buttonBonusPrevious";
            buttonBonusPrevious.Size = new Size(36, 36);
            buttonBonusPrevious.TabIndex = 13;
            buttonBonusPrevious.Text = "<";
            buttonBonusPrevious.UseVisualStyleBackColor = true;
            buttonBonusPrevious.Click += buttonBonusPrevious_Click;
            // 
            // buttonBonusNext
            // 
            buttonBonusNext.BackgroundImageLayout = ImageLayout.Zoom;
            buttonBonusNext.Font = new Font("Segoe UI Emoji", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonBonusNext.ForeColor = Color.Black;
            buttonBonusNext.Location = new Point(135, 22);
            buttonBonusNext.Name = "buttonBonusNext";
            buttonBonusNext.Size = new Size(36, 36);
            buttonBonusNext.TabIndex = 12;
            buttonBonusNext.Text = ">";
            buttonBonusNext.UseVisualStyleBackColor = true;
            buttonBonusNext.Click += buttonBonusNext_Click;
            // 
            // timerBonusLabel
            // 
            timerBonusLabel.Anchor = AnchorStyles.Top;
            timerBonusLabel.AutoSize = true;
            timerBonusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            timerBonusLabel.ForeColor = Color.White;
            timerBonusLabel.Location = new Point(-3, 0);
            timerBonusLabel.Name = "timerBonusLabel";
            timerBonusLabel.Size = new Size(181, 21);
            timerBonusLabel.TabIndex = 11;
            timerBonusLabel.Text = "Добавление времени";
            // 
            // buttonSaveSettings
            // 
            buttonSaveSettings.BackColor = Color.FromArgb(248, 249, 255);
            buttonSaveSettings.Dock = DockStyle.Fill;
            buttonSaveSettings.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            buttonSaveSettings.FlatAppearance.BorderSize = 2;
            buttonSaveSettings.FlatStyle = FlatStyle.Flat;
            buttonSaveSettings.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            buttonSaveSettings.ForeColor = Color.FromArgb(167, 157, 255);
            buttonSaveSettings.Location = new Point(3, 486);
            buttonSaveSettings.Name = "buttonSaveSettings";
            buttonSaveSettings.Size = new Size(174, 42);
            buttonSaveSettings.TabIndex = 14;
            buttonSaveSettings.Text = "Сохранить настройки";
            buttonSaveSettings.UseVisualStyleBackColor = false;
            buttonSaveSettings.Click += buttonSaveSettings_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(984, 579);
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 200);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
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
            panelTimer.ResumeLayout(false);
            panelTimer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)timerTrackBar).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bonusTrackBar).EndInit();
            ResumeLayout(false);
        }

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
        private Panel panelTimer;
        private Components.Labels.Heading3Label timerTitle;
        private Button buttonTimerNext;
        private Button buttonTimerPrevious;
        private Components.Labels.Heading3Label timerValueLabel;
        private Panel panel1;
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
    }
}
