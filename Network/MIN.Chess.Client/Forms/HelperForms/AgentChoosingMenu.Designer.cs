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
            tableLayoutPanel = new TableLayoutPanel();
            buttonRemoveCurrent = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            commonButton1 = new MIN.Chess.Client.Components.Buttons.CommonButton();
            cancelButton = new MIN.Chess.Client.Components.Buttons.InvertedButton();
            randomAgentButton = new MIN.Chess.Client.Components.Buttons.CommonButton();
            primaryLabel1 = new MIN.Chess.Client.Components.Labels.PrimaryLabel();
            checkBoxAsSelf = new CheckBox();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 4;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0006237F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0006275F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0006275F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.9981289F));
            tableLayoutPanel.Controls.Add(buttonRemoveCurrent, 0, 1);
            tableLayoutPanel.Controls.Add(randomAgentButton, 0, 0);
            tableLayoutPanel.Controls.Add(cancelButton, 3, 1);
            tableLayoutPanel.Controls.Add(commonButton1, 2, 0);
            tableLayoutPanel.Controls.Add(primaryLabel1, 1, 1);
            tableLayoutPanel.Controls.Add(checkBoxAsSelf, 2, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayoutPanel.Size = new Size(585, 346);
            tableLayoutPanel.TabIndex = 0;
            // 
            // buttonRemoveCurrent
            // 
            buttonRemoveCurrent.Anchor = AnchorStyles.Top;
            buttonRemoveCurrent.BackColor = Color.FromArgb(248, 249, 255);
            buttonRemoveCurrent.DialogResult = DialogResult.Cancel;
            buttonRemoveCurrent.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            buttonRemoveCurrent.FlatAppearance.BorderSize = 2;
            buttonRemoveCurrent.FlatStyle = FlatStyle.Flat;
            buttonRemoveCurrent.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            buttonRemoveCurrent.ForeColor = Color.FromArgb(255, 128, 128);
            buttonRemoveCurrent.Location = new Point(3, 301);
            buttonRemoveCurrent.Name = "buttonRemoveCurrent";
            buttonRemoveCurrent.Size = new Size(140, 42);
            buttonRemoveCurrent.TabIndex = 5;
            buttonRemoveCurrent.Text = "Убрать текущего";
            buttonRemoveCurrent.UseVisualStyleBackColor = false;
            buttonRemoveCurrent.Click += buttonRemoveCurrent_Click;
            // 
            // commonButton1
            // 
            commonButton1.BackColor = Color.White;
            commonButton1.BackgroundImage = Properties.Resources.maguns;
            commonButton1.BackgroundImageLayout = ImageLayout.Zoom;
            tableLayoutPanel.SetColumnSpan(commonButton1, 2);
            commonButton1.Dock = DockStyle.Fill;
            commonButton1.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            commonButton1.FlatStyle = FlatStyle.Flat;
            commonButton1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            commonButton1.ForeColor = Color.White;
            commonButton1.Location = new Point(295, 3);
            commonButton1.Name = "commonButton1";
            commonButton1.Size = new Size(287, 292);
            commonButton1.TabIndex = 4;
            commonButton1.Text = "Магнус Карлсен";
            commonButton1.TextAlign = ContentAlignment.TopLeft;
            commonButton1.UseVisualStyleBackColor = false;
            commonButton1.Click += commonButton1_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Top;
            cancelButton.BackColor = Color.FromArgb(248, 249, 255);
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(167, 157, 255);
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            cancelButton.ForeColor = Color.FromArgb(167, 157, 255);
            cancelButton.Location = new Point(441, 301);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(141, 42);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = false;
            // 
            // randomAgentButton
            // 
            randomAgentButton.BackColor = Color.White;
            randomAgentButton.BackgroundImage = Properties.Resources.random;
            randomAgentButton.BackgroundImageLayout = ImageLayout.Zoom;
            tableLayoutPanel.SetColumnSpan(randomAgentButton, 2);
            randomAgentButton.Dock = DockStyle.Fill;
            randomAgentButton.FlatAppearance.BorderColor = Color.FromArgb(228, 230, 240);
            randomAgentButton.FlatStyle = FlatStyle.Flat;
            randomAgentButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            randomAgentButton.ForeColor = Color.Black;
            randomAgentButton.Location = new Point(3, 3);
            randomAgentButton.Name = "randomAgentButton";
            randomAgentButton.Size = new Size(286, 292);
            randomAgentButton.TabIndex = 3;
            randomAgentButton.Text = "Рандом";
            randomAgentButton.TextAlign = ContentAlignment.BottomCenter;
            randomAgentButton.UseVisualStyleBackColor = false;
            randomAgentButton.Click += randomAgentButton_Click;
            // 
            // primaryLabel1
            // 
            primaryLabel1.Anchor = AnchorStyles.Right;
            primaryLabel1.AutoSize = true;
            primaryLabel1.Font = new Font("Segoe UI", 9.75F);
            primaryLabel1.ForeColor = Color.FromArgb(45, 43, 58);
            primaryLabel1.Location = new Point(152, 313);
            primaryLabel1.Name = "primaryLabel1";
            primaryLabel1.Size = new Size(137, 17);
            primaryLabel1.TabIndex = 6;
            primaryLabel1.Text = "выбрать вместо себя";
            primaryLabel1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // checkBoxAsSelf
            // 
            checkBoxAsSelf.Anchor = AnchorStyles.Left;
            checkBoxAsSelf.AutoSize = true;
            checkBoxAsSelf.Location = new Point(295, 315);
            checkBoxAsSelf.Name = "checkBoxAsSelf";
            checkBoxAsSelf.Size = new Size(15, 14);
            checkBoxAsSelf.TabIndex = 7;
            checkBoxAsSelf.UseVisualStyleBackColor = true;
            // 
            // AgentChoosingMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(585, 346);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "AgentChoosingMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Выбор агента";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        private MIN.Chess.Client.Components.Buttons.CommonButton commonButton1;

        private MIN.Chess.Client.Components.Buttons.CommonButton randomAgentButton;

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private MIN.Chess.Client.Components.Buttons.InvertedButton cancelButton;
        private Components.Buttons.InvertedButton buttonRemoveCurrent;
        private Components.Labels.PrimaryLabel primaryLabel1;
        private CheckBox checkBoxAsSelf;
    }
}
