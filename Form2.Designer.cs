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
            this.components = new System.ComponentModel.Container();
            this.oznitelik = new System.Windows.Forms.DataGridView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
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
            this.oznitelik.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.oznitelik_CellContentClick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // formOznitelik
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.oznitelik);
            this.Name = "formOznitelik";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.oznitelik)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.DataGridView oznitelik;
        private System.Windows.Forms.ImageList imageList1;
    }
}