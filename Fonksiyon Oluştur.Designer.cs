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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.fonksiyon_listesi = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox_fonksiyonlar_1
            // 
            this.comboBox_fonksiyonlar_1.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_fonksiyonlar_1.FormattingEnabled = true;
            this.comboBox_fonksiyonlar_1.Location = new System.Drawing.Point(285, 135);
            this.comboBox_fonksiyonlar_1.Name = "comboBox_fonksiyonlar_1";
            this.comboBox_fonksiyonlar_1.Size = new System.Drawing.Size(248, 32);
            this.comboBox_fonksiyonlar_1.TabIndex = 0;
            // 
            // comboBox_fonksiyonlar_2
            // 
            this.comboBox_fonksiyonlar_2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_fonksiyonlar_2.FormattingEnabled = true;
            this.comboBox_fonksiyonlar_2.Location = new System.Drawing.Point(285, 203);
            this.comboBox_fonksiyonlar_2.Name = "comboBox_fonksiyonlar_2";
            this.comboBox_fonksiyonlar_2.Size = new System.Drawing.Size(248, 32);
            this.comboBox_fonksiyonlar_2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(69, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Birincil Katman:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(69, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "İkincil Katman:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(567, 333);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 40);
            this.button1.TabIndex = 4;
            this.button1.Text = "Birleştir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(743, 333);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 40);
            this.button2.TabIndex = 5;
            this.button2.Text = "İptal";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // fonksiyon_listesi
            // 
            this.fonksiyon_listesi.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fonksiyon_listesi.FormattingEnabled = true;
            this.fonksiyon_listesi.Items.AddRange(new object[] {
            "Katmanları Birleştir",
            "Overlap Analizi"});
            this.fonksiyon_listesi.Location = new System.Drawing.Point(254, 17);
            this.fonksiyon_listesi.Name = "fonksiyon_listesi";
            this.fonksiyon_listesi.Size = new System.Drawing.Size(364, 32);
            this.fonksiyon_listesi.TabIndex = 6;
            this.fonksiyon_listesi.Text = "Katmanları Birleştir";
            this.fonksiyon_listesi.SelectedIndexChanged += new System.EventHandler(this.fonksiyon_listesi_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(61, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "Fonksiyon Seç:";
            // 
            // Fonksiyon_Oluştur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(906, 398);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fonksiyon_listesi);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_fonksiyonlar_2);
            this.Controls.Add(this.comboBox_fonksiyonlar_1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Fonksiyon_Oluştur";
            this.Text = "Fonksiyon Oluştur";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_fonksiyonlar_1;
        private System.Windows.Forms.ComboBox comboBox_fonksiyonlar_2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox fonksiyon_listesi;
        private System.Windows.Forms.Label label3;
    }
}