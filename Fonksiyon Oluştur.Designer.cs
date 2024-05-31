namespace SLF
{
    partial class Fonksiyon_Oluştur
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fonksiyon_Oluştur));
            this.comboBox_fonksiyonlar_1 = new System.Windows.Forms.ComboBox();
            this.comboBox_fonksiyonlar_2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buton_jabl = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_fonksiyonlar_1
            // 
            this.comboBox_fonksiyonlar_1.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_fonksiyonlar_1.FormattingEnabled = true;
            this.comboBox_fonksiyonlar_1.Location = new System.Drawing.Point(260, 21);
            this.comboBox_fonksiyonlar_1.Name = "comboBox_fonksiyonlar_1";
            this.comboBox_fonksiyonlar_1.Size = new System.Drawing.Size(248, 32);
            this.comboBox_fonksiyonlar_1.TabIndex = 0;
            // 
            // comboBox_fonksiyonlar_2
            // 
            this.comboBox_fonksiyonlar_2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_fonksiyonlar_2.FormattingEnabled = true;
            this.comboBox_fonksiyonlar_2.Location = new System.Drawing.Point(260, 89);
            this.comboBox_fonksiyonlar_2.Name = "comboBox_fonksiyonlar_2";
            this.comboBox_fonksiyonlar_2.Size = new System.Drawing.Size(248, 32);
            this.comboBox_fonksiyonlar_2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(44, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Birincil Katman:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(44, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "İkincil Katman:";
            // 
            // buton_jabl
            // 
            this.buton_jabl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_jabl.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buton_jabl.Location = new System.Drawing.Point(643, 488);
            this.buton_jabl.Name = "buton_jabl";
            this.buton_jabl.Size = new System.Drawing.Size(151, 40);
            this.buton_jabl.TabIndex = 4;
            this.buton_jabl.Text = "Birleştir";
            this.buton_jabl.UseVisualStyleBackColor = true;
            this.buton_jabl.Click += new System.EventHandler(this.buton_jabl_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(819, 488);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 40);
            this.button2.TabIndex = 5;
            this.button2.Text = "İptal";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Fonksiyon_Oluştur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buton_jabl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_fonksiyonlar_2);
            this.Controls.Add(this.comboBox_fonksiyonlar_1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "Fonksiyon_Oluştur";
            this.Text = "Fonksiyon Oluştur";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBox_fonksiyonlar_1;
        public System.Windows.Forms.ComboBox comboBox_fonksiyonlar_2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buton_jabl;
        private System.Windows.Forms.Button button2;
    }
}