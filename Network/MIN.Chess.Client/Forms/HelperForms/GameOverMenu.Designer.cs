namespace MIN.Chess.Client.Forms.HelperForms
{
    partial class GameOverMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            tableLayoutPanel = new TableLayoutPanel();
            reason = new MIN.Chess.Client.Components.Labels.Heading3Label();
            outcomeTitle = new MIN.Chess.Client.Components.Labels.Heading1Label();
            exitButton = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            restartButton = new MIN.Chess.Client.Components.Buttons.CommonButton();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(reason, 0, 1);
            tableLayoutPanel.Controls.Add(outcomeTitle, 0, 0);
            tableLayoutPanel.Controls.Add(exitButton, 1, 2);
            tableLayoutPanel.Controls.Add(restartButton, 0, 2);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.Size = new Size(379, 149);
            tableLayoutPanel.TabIndex = 0;
            // 
            // reason
            // 
            reason.Anchor = AnchorStyles.None;
            reason.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(reason, 2);
            reason.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            reason.ForeColor = Color.FromArgb(45, 43, 58);
            reason.Location = new Point(149, 63);
            reason.Name = "reason";
            reason.Size = new Size(81, 21);
            reason.TabIndex = 0;
            reason.Text = "Причина";
            // 
            // outcomeTitle
            // 
            outcomeTitle.Anchor = AnchorStyles.None;
            outcomeTitle.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(outcomeTitle, 2);
            outcomeTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            outcomeTitle.ForeColor = Color.Black;
            outcomeTitle.Location = new Point(149, 9);
            outcomeTitle.Name = "outcomeTitle";
            outcomeTitle.Size = new Size(80, 30);
            outcomeTitle.TabIndex = 1;
            outcomeTitle.Text = "Исход";
            // 
            // exitButton
            // 
            exitButton.BackColor = Color.FromArgb(248, 249, 255);
            exitButton.Dock = DockStyle.Fill;
            exitButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            exitButton.FlatAppearance.BorderSize = 2;
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            exitButton.ForeColor = Color.FromArgb(167, 157, 255);
            exitButton.Location = new Point(192, 101);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(184, 45);
            exitButton.TabIndex = 2;
            exitButton.Text = "Выйти из активности";
            exitButton.UseVisualStyleBackColor = false;
            exitButton.Click += exitButton_Click;
            // 
            // restartButton
            // 
            restartButton.BackColor = Color.FromArgb(167, 157, 255);
            restartButton.Dock = DockStyle.Fill;
            restartButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            restartButton.FlatStyle = FlatStyle.Flat;
            restartButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            restartButton.ForeColor = Color.FromArgb(248, 249, 255);
            restartButton.Location = new Point(3, 101);
            restartButton.Name = "restartButton";
            restartButton.Size = new Size(183, 45);
            restartButton.TabIndex = 3;
            restartButton.Text = "Рестарт (0/1)";
            restartButton.UseVisualStyleBackColor = false;
            restartButton.Click += restartButton_Click;
            // 
            // GameOverMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(379, 149);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "GameOverMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Конец игры";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Components.Labels.Heading3Label reason;
        private Components.Labels.Heading1Label outcomeTitle;
        private Components.Buttons.InvertedButton exitButton;
        private Components.Buttons.CommonButton restartButton;
    }
}
