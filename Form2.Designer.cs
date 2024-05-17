using System;

namespace SLF
{
    partial class formOznitelik
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
            this.oznitelik = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.oznitelik)).BeginInit();
            this.SuspendLayout();
            // 
            // oznitelik
            // 
            this.oznitelik.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.oznitelik.Location = new System.Drawing.Point(7, 0);
            this.oznitelik.Name = "oznitelik";
            this.oznitelik.RowHeadersWidth = 51;
            this.oznitelik.RowTemplate.Height = 24;
            this.oznitelik.Size = new System.Drawing.Size(790, 450);
            this.oznitelik.TabIndex = 0;
            // 
            // formOznitelik
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.oznitelik);
            this.Name = "formOznitelik";
            this.Text = "Form2";
           // this.Load += new System.EventHandler(this.formOznitelik_Load);
            ((System.ComponentModel.ISupportInitialize)(this.oznitelik)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.DataGridView oznitelik;
    }
}