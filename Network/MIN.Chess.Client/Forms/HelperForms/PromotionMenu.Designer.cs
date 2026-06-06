namespace MIN.Chess.Client.Forms.HelperForms
{
    partial class PromotionMenu
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
            KnightImg = new PictureBox();
            BishopImg = new PictureBox();
            RookImg = new PictureBox();
            title = new MIN.Chess.Client.Components.Labels.Heading1Label();
            QueenImg = new PictureBox();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)KnightImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BishopImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RookImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)QueenImg).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 4;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel.Controls.Add(KnightImg, 3, 1);
            tableLayoutPanel.Controls.Add(BishopImg, 2, 1);
            tableLayoutPanel.Controls.Add(RookImg, 1, 1);
            tableLayoutPanel.Controls.Add(title, 0, 0);
            tableLayoutPanel.Controls.Add(QueenImg, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 36.91275F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 63.08725F));
            tableLayoutPanel.Size = new Size(379, 149);
            tableLayoutPanel.TabIndex = 0;
            // 
            // KnightImg
            // 
            KnightImg.Dock = DockStyle.Fill;
            KnightImg.Location = new Point(282, 55);
            KnightImg.Margin = new Padding(0);
            KnightImg.Name = "KnightImg";
            KnightImg.Size = new Size(97, 94);
            KnightImg.TabIndex = 5;
            KnightImg.TabStop = false;
            KnightImg.Click += KnightImg_Click;
            // 
            // BishopImg
            // 
            BishopImg.Dock = DockStyle.Fill;
            BishopImg.Location = new Point(188, 55);
            BishopImg.Margin = new Padding(0);
            BishopImg.Name = "BishopImg";
            BishopImg.Size = new Size(94, 94);
            BishopImg.TabIndex = 4;
            BishopImg.TabStop = false;
            BishopImg.Click += BishopImg_Click;
            // 
            // RookImg
            // 
            RookImg.Dock = DockStyle.Fill;
            RookImg.Location = new Point(94, 55);
            RookImg.Margin = new Padding(0);
            RookImg.Name = "RookImg";
            RookImg.Size = new Size(94, 94);
            RookImg.TabIndex = 3;
            RookImg.TabStop = false;
            RookImg.Click += RookImg_Click;
            // 
            // title
            // 
            title.Anchor = AnchorStyles.None;
            title.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(title, 4);
            title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            title.ForeColor = Color.Black;
            title.Location = new Point(98, 12);
            title.Name = "title";
            title.Size = new Size(182, 30);
            title.TabIndex = 1;
            title.Text = "Выбери фигуру";
            // 
            // QueenImg
            // 
            QueenImg.Dock = DockStyle.Fill;
            QueenImg.Location = new Point(0, 55);
            QueenImg.Margin = new Padding(0);
            QueenImg.Name = "QueenImg";
            QueenImg.Size = new Size(94, 94);
            QueenImg.TabIndex = 2;
            QueenImg.TabStop = false;
            QueenImg.Click += QueenImg_Click;
            // 
            // PromotionMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(379, 149);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PromotionMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Выбор фигуры";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)KnightImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)BishopImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)RookImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)QueenImg).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Components.Labels.Heading1Label title;
        private PictureBox KnightImg;
        private PictureBox BishopImg;
        private PictureBox RookImg;
        private PictureBox QueenImg;
    }
}
