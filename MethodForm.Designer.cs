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
            this.ForwardButton = new SLF.CustomButton();
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
            this.MethodPanel.Controls.Add(this.ForwardButton);
            this.MethodPanel.Controls.Add(this.label1);
            this.MethodPanel.Controls.Add(this.MethodComboBox);
            this.MethodPanel.Location = new System.Drawing.Point(45, 58);
            this.MethodPanel.Name = "MethodPanel";
            this.MethodPanel.Size = new System.Drawing.Size(291, 347);
            this.MethodPanel.TabIndex = 0;
            this.MethodPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MethodPanel_Paint);
            // 
            // ForwardButton
            // 
            this.ForwardButton.BackColor = System.Drawing.Color.DarkOrange;
            this.ForwardButton.BackgroundColor = System.Drawing.Color.DarkOrange;
            this.ForwardButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.ForwardButton.BorderRadius = 0;
            this.ForwardButton.BorderSize = 0;
            this.ForwardButton.FlatAppearance.BorderSize = 0;
            this.ForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ForwardButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ForwardButton.ForeColor = System.Drawing.Color.White;
            this.ForwardButton.Location = new System.Drawing.Point(71, 235);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(150, 40);
            this.ForwardButton.TabIndex = 3;
            this.ForwardButton.Text = "İLERLE";
            this.ForwardButton.TextColor = System.Drawing.Color.White;
            this.ForwardButton.UseVisualStyleBackColor = false;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.Coral;
            this.label1.Location = new System.Drawing.Point(96, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Metot Seçimi";
            // 
            // MethodComboBox
            // 
            this.MethodComboBox.BackColor = System.Drawing.Color.Snow;
            this.MethodComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MethodComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MethodComboBox.ForeColor = System.Drawing.Color.Orange;
            this.MethodComboBox.FormattingEnabled = true;
            this.MethodComboBox.Items.AddRange(new object[] {
            "SLF (Jeo-Uzamsal)",
            "ELF (Ekonometrik)"});
            this.MethodComboBox.Location = new System.Drawing.Point(53, 122);
            this.MethodComboBox.Name = "MethodComboBox";
            this.MethodComboBox.Size = new System.Drawing.Size(186, 25);
            this.MethodComboBox.TabIndex = 1;
            this.MethodComboBox.Text = "Başlangıç metodu seçiniz.";
            // 
            // MethodForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SLF.Properties.Resources.location_tech;
            this.ClientSize = new System.Drawing.Size(384, 461);
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
            this.Text = "Metot Seçimi";
            this.MethodPanel.ResumeLayout(false);
            this.MethodPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MethodPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MethodComboBox;
        private CustomButton ForwardButton;
    }
}