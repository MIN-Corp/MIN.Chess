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
            mainPanel = new DoubleBufferedPanel();
            tableLayoutPanelSpectating = new TableLayoutPanel();
            spectatorsCounter = new MIN.Chess.Client.Components.Labels.Heading1Label();
            spectatorLogo = new PictureBox();
            tableLayoutPanelButtons = new TableLayoutPanel();
            resignButton = new Button();
            tableLayoutPanelOpponent = new TableLayoutPanel();
            opponentName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            flowLayoutPanelOpponentCaptures = new FlowLayoutPanel();
            tableLayoutPanelSelf = new TableLayoutPanel();
            flowLayoutPanelSelfCaptures = new FlowLayoutPanel();
            selfRoleName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            pauseButton = new Button();
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanelSpectating.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spectatorLogo).BeginInit();
            tableLayoutPanelButtons.SuspendLayout();
            tableLayoutPanelOpponent.SuspendLayout();
            tableLayoutPanelSelf.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.Controls.Add(mainPanel, 1, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelSpectating, 2, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelButtons, 2, 3);
            tableLayoutPanel.Controls.Add(tableLayoutPanelOpponent, 0, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanelSelf, 0, 3);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new Size(984, 579);
            tableLayoutPanel.TabIndex = 0;
            // 
            // mainPanel
            // 
            mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(200, 0);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            tableLayoutPanel.SetRowSpan(mainPanel, 4);
            mainPanel.Size = new Size(584, 579);
            mainPanel.TabIndex = 0;
            // 
            // tableLayoutPanelSpectating
            // 
            tableLayoutPanelSpectating.ColumnCount = 2;
            tableLayoutPanelSpectating.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanelSpectating.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSpectating.Controls.Add(spectatorsCounter, 0, 0);
            tableLayoutPanelSpectating.Controls.Add(spectatorLogo, 1, 0);
            tableLayoutPanelSpectating.Location = new Point(787, 3);
            tableLayoutPanelSpectating.Name = "tableLayoutPanelSpectating";
            tableLayoutPanelSpectating.RowCount = 1;
            tableLayoutPanelSpectating.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSpectating.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelSpectating.Size = new Size(194, 42);
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
            tableLayoutPanelButtons.ColumnCount = 3;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tableLayoutPanelButtons.Controls.Add(pauseButton, 2, 0);
            tableLayoutPanelButtons.Controls.Add(resignButton, 0, 0);
            tableLayoutPanelButtons.Location = new Point(787, 534);
            tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            tableLayoutPanelButtons.RowCount = 1;
            tableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtons.Size = new Size(194, 42);
            tableLayoutPanelButtons.TabIndex = 6;
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
            tableLayoutPanelOpponent.Controls.Add(opponentName, 0, 0);
            tableLayoutPanelOpponent.Controls.Add(flowLayoutPanelOpponentCaptures, 0, 1);
            tableLayoutPanelOpponent.Location = new Point(3, 3);
            tableLayoutPanelOpponent.Name = "tableLayoutPanelOpponent";
            tableLayoutPanelOpponent.RowCount = 2;
            tableLayoutPanelOpponent.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanelOpponent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelOpponent.Size = new Size(194, 42);
            tableLayoutPanelOpponent.TabIndex = 7;
            // 
            // opponentName
            // 
            opponentName.AutoSize = true;
            opponentName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            opponentName.ForeColor = Color.White;
            opponentName.Location = new Point(3, 0);
            opponentName.Name = "opponentName";
            opponentName.Size = new Size(104, 21);
            opponentName.TabIndex = 2;
            opponentName.Text = "Загрузка . . .";
            // 
            // flowLayoutPanelOpponentCaptures
            // 
            flowLayoutPanelOpponentCaptures.Dock = DockStyle.Fill;
            flowLayoutPanelOpponentCaptures.Location = new Point(0, 30);
            flowLayoutPanelOpponentCaptures.Margin = new Padding(0);
            flowLayoutPanelOpponentCaptures.Name = "flowLayoutPanelOpponentCaptures";
            flowLayoutPanelOpponentCaptures.Size = new Size(194, 12);
            flowLayoutPanelOpponentCaptures.TabIndex = 3;
            flowLayoutPanelOpponentCaptures.WrapContents = false;
            // 
            // tableLayoutPanelSelf
            // 
            tableLayoutPanelSelf.ColumnCount = 1;
            tableLayoutPanelSelf.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSelf.Controls.Add(flowLayoutPanelSelfCaptures, 0, 1);
            tableLayoutPanelSelf.Controls.Add(selfRoleName, 0, 0);
            tableLayoutPanelSelf.Location = new Point(3, 534);
            tableLayoutPanelSelf.Name = "tableLayoutPanelSelf";
            tableLayoutPanelSelf.RowCount = 2;
            tableLayoutPanelSelf.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanelSelf.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSelf.Size = new Size(194, 42);
            tableLayoutPanelSelf.TabIndex = 8;
            // 
            // flowLayoutPanelSelfCaptures
            // 
            flowLayoutPanelSelfCaptures.Dock = DockStyle.Fill;
            flowLayoutPanelSelfCaptures.Location = new Point(0, 30);
            flowLayoutPanelSelfCaptures.Margin = new Padding(0);
            flowLayoutPanelSelfCaptures.Name = "flowLayoutPanelSelfCaptures";
            flowLayoutPanelSelfCaptures.Size = new Size(194, 12);
            flowLayoutPanelSelfCaptures.TabIndex = 4;
            flowLayoutPanelSelfCaptures.WrapContents = false;
            // 
            // selfRoleName
            // 
            selfRoleName.AutoSize = true;
            selfRoleName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            selfRoleName.ForeColor = Color.White;
            selfRoleName.Location = new Point(3, 0);
            selfRoleName.Name = "selfRoleName";
            selfRoleName.Size = new Size(104, 21);
            selfRoleName.TabIndex = 1;
            selfRoleName.Text = "Загрузка . . .";
            // 
            // pauseButton
            // 
            pauseButton.BackgroundImage = Properties.Resources.pause;
            pauseButton.BackgroundImageLayout = ImageLayout.Zoom;
            pauseButton.Dock = DockStyle.Fill;
            pauseButton.ForeColor = Color.Black;
            pauseButton.Location = new Point(155, 3);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(36, 36);
            pauseButton.TabIndex = 10;
            pauseButton.UseVisualStyleBackColor = true;
            pauseButton.Click += pauseButton_Click;
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
            tableLayoutPanelSpectating.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spectatorLogo).EndInit();
            tableLayoutPanelButtons.ResumeLayout(false);
            tableLayoutPanelOpponent.ResumeLayout(false);
            tableLayoutPanelOpponent.PerformLayout();
            tableLayoutPanelSelf.ResumeLayout(false);
            tableLayoutPanelSelf.PerformLayout();
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
        private FlowLayoutPanel flowLayoutPanelOpponentCaptures;
        private TableLayoutPanel tableLayoutPanelSelf;
        private FlowLayoutPanel flowLayoutPanelSelfCaptures;
        private Button resignButton;
        private Button pauseButton;
    }
}
