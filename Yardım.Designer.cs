namespace SLF
{
    partial class Yardım
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
            this.trial_combobox = new System.Windows.Forms.ComboBox();
            this.cf = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // trial_combobox
            // 
            this.trial_combobox.FormattingEnabled = true;
            this.trial_combobox.Location = new System.Drawing.Point(234, 188);
            this.trial_combobox.Name = "trial_combobox";
            this.trial_combobox.Size = new System.Drawing.Size(121, 24);
            this.trial_combobox.TabIndex = 0;
            this.trial_combobox.Text = "Ümit";
            // 
            // cf
            // 
            this.cf.AutoSize = true;
            this.cf.Location = new System.Drawing.Point(602, 172);
            this.cf.Name = "cf";
            this.cf.Size = new System.Drawing.Size(95, 20);
            this.cf.TabIndex = 1;
            this.cf.Text = "checkBox1";
            this.cf.UseVisualStyleBackColor = true;
            this.cf.CheckedChanged += new System.EventHandler(this.aslkşfjlkasf_CheckedChanged);
            // 
            // Yardım
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 550);
            this.Controls.Add(this.cf);
            this.Controls.Add(this.trial_combobox);
            this.Name = "Yardım";
            this.Text = "Yardım";
            this.Load += new System.EventHandler(this.Yardım_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox trial_combobox;
        private System.Windows.Forms.CheckBox cf;
    }
}