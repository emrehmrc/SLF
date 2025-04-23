namespace SLF
{
    partial class MethodForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MethodForm));
            this.MethodPanel = new System.Windows.Forms.Panel();
            this.IlceComboBox = new System.Windows.Forms.ComboBox();
            this.IlComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ForwardButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MethodComboBox = new System.Windows.Forms.ComboBox();
            this.MethodPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MethodPanel
            // 
            this.MethodPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MethodPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.MethodPanel.Controls.Add(this.IlceComboBox);
            this.MethodPanel.Controls.Add(this.IlComboBox);
            this.MethodPanel.Controls.Add(this.label3);
            this.MethodPanel.Controls.Add(this.label2);
            this.MethodPanel.Controls.Add(this.ForwardButton);
            this.MethodPanel.Controls.Add(this.label1);
            this.MethodPanel.Controls.Add(this.MethodComboBox);
            this.MethodPanel.Location = new System.Drawing.Point(44, 53);
            this.MethodPanel.Name = "MethodPanel";
            this.MethodPanel.Size = new System.Drawing.Size(295, 348);
            this.MethodPanel.TabIndex = 0;
            this.MethodPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MethodPanel_Paint);
            // 
            // IlceComboBox
            // 
            this.IlceComboBox.BackColor = System.Drawing.Color.Snow;
            this.IlceComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.IlceComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.IlceComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.IlceComboBox.FormattingEnabled = true;
            this.IlceComboBox.ItemHeight = 21;
            this.IlceComboBox.Items.AddRange(new object[] {
            "SLF (Jeo-Uzamsal)",
            "ELF (Ekonometrik)"});
            this.IlceComboBox.Location = new System.Drawing.Point(8, 159);
            this.IlceComboBox.Name = "IlceComboBox";
            this.IlceComboBox.Size = new System.Drawing.Size(253, 29);
            this.IlceComboBox.TabIndex = 7;
            this.IlceComboBox.Text = "Lütfen ilçe seçiniz";
            this.IlceComboBox.SelectedIndexChanged += new System.EventHandler(this.IlceComboBox_SelectedIndexChanged);
            // 
            // IlComboBox
            // 
            this.IlComboBox.BackColor = System.Drawing.Color.Snow;
            this.IlComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.IlComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.IlComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.IlComboBox.FormattingEnabled = true;
            this.IlComboBox.ItemHeight = 21;
            this.IlComboBox.Location = new System.Drawing.Point(8, 76);
            this.IlComboBox.Name = "IlComboBox";
            this.IlComboBox.Size = new System.Drawing.Size(253, 29);
            this.IlComboBox.TabIndex = 6;
            this.IlComboBox.Text = "Lütfen il seçiniz";
            this.IlComboBox.SelectedIndexChanged += new System.EventHandler(this.IlComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.DarkOrange;
            this.label3.Location = new System.Drawing.Point(3, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 28);
            this.label3.TabIndex = 5;
            this.label3.Text = "İlçe";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.DarkOrange;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 28);
            this.label2.TabIndex = 4;
            this.label2.Text = "İl";
            // 
            // ForwardButton
            // 
            this.ForwardButton.BackColor = System.Drawing.Color.DarkOrange;
            this.ForwardButton.FlatAppearance.BorderSize = 0;
            this.ForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ForwardButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ForwardButton.ForeColor = System.Drawing.Color.White;
            this.ForwardButton.Location = new System.Drawing.Point(77, 293);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(150, 40);
            this.ForwardButton.TabIndex = 3;
            this.ForwardButton.Text = "İLERLE";
            this.ForwardButton.UseVisualStyleBackColor = false;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(3, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 28);
            this.label1.TabIndex = 2;
            this.label1.Text = "Metot Seçimi:";
            // 
            // MethodComboBox
            // 
            this.MethodComboBox.BackColor = System.Drawing.Color.Snow;
            this.MethodComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MethodComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MethodComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.MethodComboBox.FormattingEnabled = true;
            this.MethodComboBox.ItemHeight = 21;
            this.MethodComboBox.Items.AddRange(new object[] {
            "SLF (Jeo-Uzamsal)",
            "ELF (Ekonometrik)"});
            this.MethodComboBox.Location = new System.Drawing.Point(8, 247);
            this.MethodComboBox.Name = "MethodComboBox";
            this.MethodComboBox.Size = new System.Drawing.Size(253, 29);
            this.MethodComboBox.TabIndex = 1;
            this.MethodComboBox.Text = "Lütfen ilerlemek için metot seçiniz.";
            // 
            // MethodForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(382, 453);
            this.Controls.Add(this.MethodPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 500);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "MethodForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "METOT SEÇİM EKRANI";
            this.MethodPanel.ResumeLayout(false);
            this.MethodPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MethodPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MethodComboBox;
        private System.Windows.Forms.Button ForwardButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox IlceComboBox;
        private System.Windows.Forms.ComboBox IlComboBox;
    }
}