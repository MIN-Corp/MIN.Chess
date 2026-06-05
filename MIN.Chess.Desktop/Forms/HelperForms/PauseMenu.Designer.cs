namespace MIN.Chess.Desktop.Forms.HelperForms
{
    partial class PauseMenu
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
            title = new MIN.Chess.Desktop.Components.Labels.Heading1Label();
            restartButton = new MIN.Chess.Desktop.Components.Buttons.InvertedButton();
            continueButton = new MIN.Chess.Desktop.Components.Buttons.CommonButton();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(title, 0, 0);
            tableLayoutPanel.Controls.Add(restartButton, 1, 1);
            tableLayoutPanel.Controls.Add(continueButton, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel.Size = new Size(379, 149);
            tableLayoutPanel.TabIndex = 0;
            // 
            // title
            // 
            title.Anchor = AnchorStyles.None;
            title.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(title, 2);
            title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            title.ForeColor = Color.Black;
            title.Location = new Point(151, 22);
            title.Name = "title";
            title.Size = new Size(77, 30);
            title.TabIndex = 1;
            title.Text = "Пауза";
            // 
            // restartButton
            // 
            restartButton.BackColor = Color.FromArgb(248, 249, 255);
            restartButton.Dock = DockStyle.Fill;
            restartButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            restartButton.FlatAppearance.BorderSize = 2;
            restartButton.FlatStyle = FlatStyle.Flat;
            restartButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            restartButton.ForeColor = Color.FromArgb(167, 157, 255);
            restartButton.Location = new Point(192, 77);
            restartButton.Name = "restartButton";
            restartButton.Size = new Size(184, 69);
            restartButton.TabIndex = 2;
            restartButton.Text = "Рестарт (0/1)";
            restartButton.UseVisualStyleBackColor = false;
            restartButton.Click += restartButton_Click;
            // 
            // continueButton
            // 
            continueButton.BackColor = Color.FromArgb(167, 157, 255);
            continueButton.Dock = DockStyle.Fill;
            continueButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            continueButton.FlatStyle = FlatStyle.Flat;
            continueButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            continueButton.ForeColor = Color.FromArgb(248, 249, 255);
            continueButton.Location = new Point(3, 77);
            continueButton.Name = "continueButton";
            continueButton.Size = new Size(183, 69);
            continueButton.TabIndex = 3;
            continueButton.Text = "Продолжить";
            continueButton.UseVisualStyleBackColor = false;
            continueButton.Click += continueButton_Click;
            // 
            // PauseMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(379, 149);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PauseMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Пауза";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Components.Labels.Heading1Label title;
        private Components.Buttons.InvertedButton restartButton;
        private Components.Buttons.CommonButton continueButton;
    }
}
