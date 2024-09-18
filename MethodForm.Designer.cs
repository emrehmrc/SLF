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
            this.ForwardButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MethodComboBox = new System.Windows.Forms.ComboBox();
            this.MethodPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MethodPanel
            // 
            this.MethodPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.MethodPanel.Controls.Add(this.ForwardButton);
            this.MethodPanel.Controls.Add(this.label1);
            this.MethodPanel.Controls.Add(this.MethodComboBox);
            this.MethodPanel.Location = new System.Drawing.Point(44, 63);
            this.MethodPanel.Name = "MethodPanel";
            this.MethodPanel.Size = new System.Drawing.Size(266, 259);
            this.MethodPanel.TabIndex = 0;
            this.MethodPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MethodPanel_Paint);
            // 
            // ForwardButton
            // 
            this.ForwardButton.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ForwardButton.ForeColor = System.Drawing.Color.Orange;
            this.ForwardButton.Location = new System.Drawing.Point(57, 154);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(140, 28);
            this.ForwardButton.TabIndex = 1;
            this.ForwardButton.Text = "İLERLE";
            this.ForwardButton.UseVisualStyleBackColor = true;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.Coral;
            this.label1.Location = new System.Drawing.Point(24, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Metotlar:";
            // 
            // MethodComboBox
            // 
            this.MethodComboBox.ForeColor = System.Drawing.Color.Orange;
            this.MethodComboBox.FormattingEnabled = true;
            this.MethodComboBox.Items.AddRange(new object[] {
            "SLF",
            "ELF"});
            this.MethodComboBox.Location = new System.Drawing.Point(57, 76);
            this.MethodComboBox.Name = "MethodComboBox";
            this.MethodComboBox.Size = new System.Drawing.Size(140, 25);
            this.MethodComboBox.TabIndex = 1;
            this.MethodComboBox.Text = "Metot seçiniz";
            // 
            // MethodForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(351, 404);
            this.Controls.Add(this.MethodPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(367, 443);
            this.MinimumSize = new System.Drawing.Size(367, 443);
            this.Name = "MethodForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modül Seçimi";
            this.MethodPanel.ResumeLayout(false);
            this.MethodPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MethodPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MethodComboBox;
        private System.Windows.Forms.Button ForwardButton;
    }
}