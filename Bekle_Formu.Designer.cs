namespace SLF
{
    partial class BekleForm
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
            this.label_wait = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_wait
            // 
            this.label_wait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_wait.AutoSize = true;
            this.label_wait.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label_wait.Font = new System.Drawing.Font("Malgun Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_wait.ForeColor = System.Drawing.Color.Navy;
            this.label_wait.Location = new System.Drawing.Point(12, 9);
            this.label_wait.Name = "label_wait";
            this.label_wait.Size = new System.Drawing.Size(157, 23);
            this.label_wait.TabIndex = 0;
            this.label_wait.Text = "Lütfen Bekleyiniz ...";
            this.label_wait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_wait.UseWaitCursor = true;
            // 
            // BekleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(164, 29);
            this.ControlBox = false;
            this.Controls.Add(this.label_wait);
            this.Name = "BekleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_wait;
    }
}