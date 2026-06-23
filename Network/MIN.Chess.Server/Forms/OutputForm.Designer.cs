namespace MIN.Chess.Server.Forms
{
    partial class OutputForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputForm));
            log = new ListBox();
            SuspendLayout();
            // 
            // log
            // 
            log.Dock = DockStyle.Fill;
            log.FormattingEnabled = true;
            log.ItemHeight = 15;
            log.Location = new Point(0, 0);
            log.Name = "log";
            log.Size = new Size(800, 450);
            log.TabIndex = 0;
            // 
            // OutputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(log);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "OutputForm";
            Text = "Сервер шахмат";
            FormClosing += OutputForm_FormClosing;
            Load += OutputForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox log;
    }
}
