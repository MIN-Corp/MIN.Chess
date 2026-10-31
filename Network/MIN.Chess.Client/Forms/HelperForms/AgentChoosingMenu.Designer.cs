namespace MIN.Chess.Client.Forms.HelperForms
{
    partial class AgentChoosingMenu
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
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            cancelButton = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            randomAgentButton = new MIN.Chess.Client.Components.Buttons.CommonButton();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            //
            // tableLayoutPanel
            //
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(cancelButton, 0, 1);
            tableLayoutPanel.Controls.Add(randomAgentButton, 0, 0);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.333332F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new System.Drawing.Size(502, 324);
            tableLayoutPanel.TabIndex = 0;
            //
            // cancelButton
            //
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)((byte)248)), ((int)((byte)249)), ((int)((byte)255)));
            tableLayoutPanel.SetColumnSpan(cancelButton, 2);
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cancelButton.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)167)), ((int)((byte)157)), ((int)((byte)255)));
            cancelButton.Location = new System.Drawing.Point(128, 279);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(245, 42);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = false;
            //
            // randomAgentButton
            //
            randomAgentButton.BackColor = System.Drawing.Color.White;
            randomAgentButton.BackgroundImage = global::MIN.Chess.Client.Properties.Resources.random;
            randomAgentButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            randomAgentButton.Dock = System.Windows.Forms.DockStyle.Fill;
            randomAgentButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)((byte)228)), ((int)((byte)230)), ((int)((byte)240)));
            randomAgentButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            randomAgentButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)204));
            randomAgentButton.ForeColor = System.Drawing.Color.Black;
            randomAgentButton.Location = new System.Drawing.Point(3, 3);
            randomAgentButton.Name = "randomAgentButton";
            randomAgentButton.Size = new System.Drawing.Size(245, 270);
            randomAgentButton.TabIndex = 3;
            randomAgentButton.Text = "РАНДОМ";
            randomAgentButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            randomAgentButton.UseVisualStyleBackColor = false;
            randomAgentButton.Click += randomAgentButton_Click;
            //
            // AgentChoosingMenu
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(502, 324);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Выбор агента";
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private MIN.Chess.Client.Components.Buttons.CommonButton randomAgentButton;

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private MIN.Chess.Client.Components.Buttons.InvertedButton cancelButton;
    }
}
