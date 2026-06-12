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
            opponentName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            mainPanel = new DoubleBufferedPanel();
            selfRoleName = new MIN.Chess.Client.Components.Labels.Heading3Label();
            spectatorsCounter = new MIN.Chess.Client.Components.Labels.Heading1Label();
            listBox1 = new ListBox();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.Controls.Add(opponentName, 0, 0);
            tableLayoutPanel.Controls.Add(mainPanel, 1, 0);
            tableLayoutPanel.Controls.Add(selfRoleName, 0, 2);
            tableLayoutPanel.Controls.Add(spectatorsCounter, 2, 0);
            tableLayoutPanel.Controls.Add(listBox1, 2, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new Size(984, 579);
            tableLayoutPanel.TabIndex = 0;
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
            // mainPanel
            // 
            mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(200, 0);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            tableLayoutPanel.SetRowSpan(mainPanel, 3);
            mainPanel.Size = new Size(584, 579);
            mainPanel.TabIndex = 0;
            // 
            // selfRoleName
            // 
            selfRoleName.AutoSize = true;
            selfRoleName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            selfRoleName.ForeColor = Color.White;
            selfRoleName.Location = new Point(3, 531);
            selfRoleName.Name = "selfRoleName";
            selfRoleName.Size = new Size(104, 21);
            selfRoleName.TabIndex = 1;
            selfRoleName.Text = "Загрузка . . .";
            // 
            // spectatorsCounter
            // 
            spectatorsCounter.Dock = DockStyle.Left;
            spectatorsCounter.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            spectatorsCounter.ForeColor = Color.White;
            spectatorsCounter.Location = new Point(787, 0);
            spectatorsCounter.Name = "spectatorsCounter";
            spectatorsCounter.Size = new Size(32, 48);
            spectatorsCounter.TabIndex = 4;
            spectatorsCounter.Text = "0";
            spectatorsCounter.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox1
            // 
            listBox1.Dock = DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(787, 51);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(194, 477);
            listBox1.TabIndex = 5;
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
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private DoubleBufferedPanel mainPanel;
        private Components.Labels.Heading3Label opponentName;
        private Components.Labels.Heading3Label selfRoleName;
        private Components.Labels.Heading1Label spectatorsCounter;
        private ListBox listBox1;
    }
}
