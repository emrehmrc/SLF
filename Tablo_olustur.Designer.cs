namespace SLF
{
    partial class Tablo_olustur
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
            this.dtrVeriTabloOlustur = new System.Windows.Forms.Button();
            this.aboneVeriOlustur = new System.Windows.Forms.Button();
            this.tablo_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dtrVeriTabloOlustur
            // 
            this.dtrVeriTabloOlustur.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtrVeriTabloOlustur.ForeColor = System.Drawing.Color.DarkOrange;
            this.dtrVeriTabloOlustur.Location = new System.Drawing.Point(69, 115);
            this.dtrVeriTabloOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.dtrVeriTabloOlustur.Name = "dtrVeriTabloOlustur";
            this.dtrVeriTabloOlustur.Size = new System.Drawing.Size(163, 53);
            this.dtrVeriTabloOlustur.TabIndex = 13;
            this.dtrVeriTabloOlustur.Text = "DTR Veri Tablosu Oluştur";
            this.dtrVeriTabloOlustur.UseVisualStyleBackColor = true;
            this.dtrVeriTabloOlustur.Click += new System.EventHandler(this.dtrVeriTabloOlustur_Click);
            // 
            // aboneVeriOlustur
            // 
            this.aboneVeriOlustur.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboneVeriOlustur.ForeColor = System.Drawing.Color.DarkOrange;
            this.aboneVeriOlustur.Location = new System.Drawing.Point(69, 220);
            this.aboneVeriOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.aboneVeriOlustur.Name = "aboneVeriOlustur";
            this.aboneVeriOlustur.Size = new System.Drawing.Size(163, 53);
            this.aboneVeriOlustur.TabIndex = 16;
            this.aboneVeriOlustur.Text = "Abone Veri Tablosu Oluştur";
            this.aboneVeriOlustur.UseVisualStyleBackColor = true;
            this.aboneVeriOlustur.Click += new System.EventHandler(this.aboneVeriOlustur_Click);
            // 
            // tablo_label
            // 
            this.tablo_label.AutoSize = true;
            this.tablo_label.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tablo_label.ForeColor = System.Drawing.Color.DarkOrange;
            this.tablo_label.Location = new System.Drawing.Point(29, 23);
            this.tablo_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tablo_label.Name = "tablo_label";
            this.tablo_label.Size = new System.Drawing.Size(246, 48);
            this.tablo_label.TabIndex = 21;
            this.tablo_label.Text = "Tablo Oluştur";
            // 
            // Tablo_olustur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 293);
            this.Controls.Add(this.tablo_label);
            this.Controls.Add(this.aboneVeriOlustur);
            this.Controls.Add(this.dtrVeriTabloOlustur);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Tablo_olustur";
            this.Text = "Tablo_olustur";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button dtrVeriTabloOlustur;
        private System.Windows.Forms.Button aboneVeriOlustur;
        private System.Windows.Forms.Label tablo_label;
    }
}