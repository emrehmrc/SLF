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
            this.label_birincil_katman_fonksiyonForm = new System.Windows.Forms.Label();
            this.label_ikincil_katman_fonksiyonForm = new System.Windows.Forms.Label();
            this.buton_jabl = new System.Windows.Forms.Button();
            this.buton_iptal_fonksiyonForm = new System.Windows.Forms.Button();
            this.checkBox_cell_statistics = new System.Windows.Forms.CheckBox();
            this.label_agregasyon_fonksiyonForm = new System.Windows.Forms.Label();
            this.tum_sutunlar_fonksiyonForm = new System.Windows.Forms.ListBox();
            this.secilen_sutunlar_fonksiyonForm = new System.Windows.Forms.ListBox();
            this.label_sütun_fonksiyonForm = new System.Windows.Forms.Label();
            this.pictureBox1_fonksiyonForm = new System.Windows.Forms.PictureBox();
            this.pictureBox2_fonksiyonForm = new System.Windows.Forms.PictureBox();
            this.label_fonksiyonlar_fonksiyonForm = new System.Windows.Forms.Label();
            this.pictureBox3_fonksiyonForm = new System.Windows.Forms.PictureBox();
            this.checkBoxCount = new System.Windows.Forms.CheckBox();
            this.checkBoxMaks = new System.Windows.Forms.CheckBox();
            this.checkBoxMin = new System.Windows.Forms.CheckBox();
            this.checkBoxSum = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_fonksiyonForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2_fonksiyonForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3_fonksiyonForm)).BeginInit();
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
            this.comboBox_fonksiyonlar_2.TextChanged += new System.EventHandler(this.comboBox_fonksiyonlar_2_TextChanged);
            // 
            // label_birincil_katman_fonksiyonForm
            // 
            this.label_birincil_katman_fonksiyonForm.AutoSize = true;
            this.label_birincil_katman_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_birincil_katman_fonksiyonForm.Location = new System.Drawing.Point(44, 29);
            this.label_birincil_katman_fonksiyonForm.Name = "label_birincil_katman_fonksiyonForm";
            this.label_birincil_katman_fonksiyonForm.Size = new System.Drawing.Size(147, 24);
            this.label_birincil_katman_fonksiyonForm.TabIndex = 2;
            this.label_birincil_katman_fonksiyonForm.Text = "Birincil Katman:";
            // 
            // label_ikincil_katman_fonksiyonForm
            // 
            this.label_ikincil_katman_fonksiyonForm.AutoSize = true;
            this.label_ikincil_katman_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ikincil_katman_fonksiyonForm.Location = new System.Drawing.Point(44, 92);
            this.label_ikincil_katman_fonksiyonForm.Name = "label_ikincil_katman_fonksiyonForm";
            this.label_ikincil_katman_fonksiyonForm.Size = new System.Drawing.Size(139, 24);
            this.label_ikincil_katman_fonksiyonForm.TabIndex = 3;
            this.label_ikincil_katman_fonksiyonForm.Text = "İkincil Katman:";
            // 
            // buton_jabl
            // 
            this.buton_jabl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_jabl.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buton_jabl.Location = new System.Drawing.Point(783, 584);
            this.buton_jabl.Name = "buton_jabl";
            this.buton_jabl.Size = new System.Drawing.Size(151, 40);
            this.buton_jabl.TabIndex = 4;
            this.buton_jabl.Text = "Birleştir";
            this.buton_jabl.UseVisualStyleBackColor = true;
            this.buton_jabl.Click += new System.EventHandler(this.buton_jabl_Click);
            // 
            // buton_iptal_fonksiyonForm
            // 
            this.buton_iptal_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_iptal_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buton_iptal_fonksiyonForm.Location = new System.Drawing.Point(959, 584);
            this.buton_iptal_fonksiyonForm.Name = "buton_iptal_fonksiyonForm";
            this.buton_iptal_fonksiyonForm.Size = new System.Drawing.Size(151, 40);
            this.buton_iptal_fonksiyonForm.TabIndex = 5;
            this.buton_iptal_fonksiyonForm.Text = "İptal";
            this.buton_iptal_fonksiyonForm.UseVisualStyleBackColor = true;
            this.buton_iptal_fonksiyonForm.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox_cell_statistics
            // 
            this.checkBox_cell_statistics.AutoSize = true;
            this.checkBox_cell_statistics.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_cell_statistics.Location = new System.Drawing.Point(21, 154);
            this.checkBox_cell_statistics.Name = "checkBox_cell_statistics";
            this.checkBox_cell_statistics.Size = new System.Drawing.Size(281, 28);
            this.checkBox_cell_statistics.TabIndex = 10;
            this.checkBox_cell_statistics.Text = "Hücresel İstatistikleri Oluştur";
            this.checkBox_cell_statistics.UseVisualStyleBackColor = true;
            this.checkBox_cell_statistics.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label_agregasyon_fonksiyonForm
            // 
            this.label_agregasyon_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_agregasyon_fonksiyonForm.AutoSize = true;
            this.label_agregasyon_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_agregasyon_fonksiyonForm.Location = new System.Drawing.Point(393, 237);
            this.label_agregasyon_fonksiyonForm.Name = "label_agregasyon_fonksiyonForm";
            this.label_agregasyon_fonksiyonForm.Size = new System.Drawing.Size(331, 27);
            this.label_agregasyon_fonksiyonForm.TabIndex = 8;
            this.label_agregasyon_fonksiyonForm.Text = "Agregasyonu Yapılacak Sütünlar";
            // 
            // tum_sutunlar_fonksiyonForm
            // 
            this.tum_sutunlar_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tum_sutunlar_fonksiyonForm.BackColor = System.Drawing.Color.FloralWhite;
            this.tum_sutunlar_fonksiyonForm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tum_sutunlar_fonksiyonForm.Font = new System.Drawing.Font("Microsoft Tai Le", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tum_sutunlar_fonksiyonForm.FormattingEnabled = true;
            this.tum_sutunlar_fonksiyonForm.ItemHeight = 23;
            this.tum_sutunlar_fonksiyonForm.Location = new System.Drawing.Point(24, 274);
            this.tum_sutunlar_fonksiyonForm.Name = "tum_sutunlar_fonksiyonForm";
            this.tum_sutunlar_fonksiyonForm.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.tum_sutunlar_fonksiyonForm.Size = new System.Drawing.Size(282, 303);
            this.tum_sutunlar_fonksiyonForm.TabIndex = 9;
            // 
            // secilen_sutunlar_fonksiyonForm
            // 
            this.secilen_sutunlar_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.secilen_sutunlar_fonksiyonForm.BackColor = System.Drawing.Color.FloralWhite;
            this.secilen_sutunlar_fonksiyonForm.Font = new System.Drawing.Font("Microsoft Tai Le", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secilen_sutunlar_fonksiyonForm.FormattingEnabled = true;
            this.secilen_sutunlar_fonksiyonForm.ItemHeight = 23;
            this.secilen_sutunlar_fonksiyonForm.Location = new System.Drawing.Point(398, 274);
            this.secilen_sutunlar_fonksiyonForm.Name = "secilen_sutunlar_fonksiyonForm";
            this.secilen_sutunlar_fonksiyonForm.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.secilen_sutunlar_fonksiyonForm.Size = new System.Drawing.Size(282, 303);
            this.secilen_sutunlar_fonksiyonForm.TabIndex = 10;
            // 
            // label_sütun_fonksiyonForm
            // 
            this.label_sütun_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_sütun_fonksiyonForm.AutoSize = true;
            this.label_sütun_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_sütun_fonksiyonForm.Location = new System.Drawing.Point(23, 237);
            this.label_sütun_fonksiyonForm.Name = "label_sütun_fonksiyonForm";
            this.label_sütun_fonksiyonForm.Size = new System.Drawing.Size(134, 27);
            this.label_sütun_fonksiyonForm.TabIndex = 11;
            this.label_sütun_fonksiyonForm.Text = "Sütun Listesi";
            // 
            // pictureBox1_fonksiyonForm
            // 
            this.pictureBox1_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1_fonksiyonForm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1_fonksiyonForm.BackgroundImage")));
            this.pictureBox1_fonksiyonForm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1_fonksiyonForm.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1_fonksiyonForm.Image")));
            this.pictureBox1_fonksiyonForm.Location = new System.Drawing.Point(329, 373);
            this.pictureBox1_fonksiyonForm.Name = "pictureBox1_fonksiyonForm";
            this.pictureBox1_fonksiyonForm.Size = new System.Drawing.Size(43, 38);
            this.pictureBox1_fonksiyonForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1_fonksiyonForm.TabIndex = 12;
            this.pictureBox1_fonksiyonForm.TabStop = false;
            this.pictureBox1_fonksiyonForm.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2_fonksiyonForm
            // 
            this.pictureBox2_fonksiyonForm.BackColor = System.Drawing.Color.PaleTurquoise;
            this.pictureBox2_fonksiyonForm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2_fonksiyonForm.Location = new System.Drawing.Point(750, 274);
            this.pictureBox2_fonksiyonForm.Name = "pictureBox2_fonksiyonForm";
            this.pictureBox2_fonksiyonForm.Size = new System.Drawing.Size(16, 322);
            this.pictureBox2_fonksiyonForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2_fonksiyonForm.TabIndex = 13;
            this.pictureBox2_fonksiyonForm.TabStop = false;
            // 
            // label_fonksiyonlar_fonksiyonForm
            // 
            this.label_fonksiyonlar_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_fonksiyonlar_fonksiyonForm.AutoSize = true;
            this.label_fonksiyonlar_fonksiyonForm.Font = new System.Drawing.Font("Maiandra GD", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_fonksiyonlar_fonksiyonForm.Location = new System.Drawing.Point(804, 240);
            this.label_fonksiyonlar_fonksiyonForm.Name = "label_fonksiyonlar_fonksiyonForm";
            this.label_fonksiyonlar_fonksiyonForm.Size = new System.Drawing.Size(267, 27);
            this.label_fonksiyonlar_fonksiyonForm.TabIndex = 15;
            this.label_fonksiyonlar_fonksiyonForm.Text = "Kullanılacak Fonksiyonlar";
            // 
            // pictureBox3_fonksiyonForm
            // 
            this.pictureBox3_fonksiyonForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3_fonksiyonForm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox3_fonksiyonForm.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3_fonksiyonForm.Image")));
            this.pictureBox3_fonksiyonForm.Location = new System.Drawing.Point(329, 436);
            this.pictureBox3_fonksiyonForm.Name = "pictureBox3_fonksiyonForm";
            this.pictureBox3_fonksiyonForm.Size = new System.Drawing.Size(43, 38);
            this.pictureBox3_fonksiyonForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3_fonksiyonForm.TabIndex = 17;
            this.pictureBox3_fonksiyonForm.TabStop = false;
            this.pictureBox3_fonksiyonForm.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // checkBoxCount
            // 
            this.checkBoxCount.AutoSize = true;
            this.checkBoxCount.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCount.Location = new System.Drawing.Point(809, 285);
            this.checkBoxCount.Name = "checkBoxCount";
            this.checkBoxCount.Size = new System.Drawing.Size(65, 29);
            this.checkBoxCount.TabIndex = 18;
            this.checkBoxCount.Text = "Say";
            this.checkBoxCount.UseVisualStyleBackColor = true;
            // 
            // checkBoxMaks
            // 
            this.checkBoxMaks.AutoSize = true;
            this.checkBoxMaks.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxMaks.Location = new System.Drawing.Point(809, 390);
            this.checkBoxMaks.Name = "checkBoxMaks";
            this.checkBoxMaks.Size = new System.Drawing.Size(80, 29);
            this.checkBoxMaks.TabIndex = 19;
            this.checkBoxMaks.Text = "Maks";
            this.checkBoxMaks.UseVisualStyleBackColor = true;
            // 
            // checkBoxMin
            // 
            this.checkBoxMin.AutoSize = true;
            this.checkBoxMin.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxMin.Location = new System.Drawing.Point(809, 355);
            this.checkBoxMin.Name = "checkBoxMin";
            this.checkBoxMin.Size = new System.Drawing.Size(68, 29);
            this.checkBoxMin.TabIndex = 20;
            this.checkBoxMin.Text = "Min";
            this.checkBoxMin.UseVisualStyleBackColor = true;
            // 
            // checkBoxSum
            // 
            this.checkBoxSum.AutoSize = true;
            this.checkBoxSum.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSum.Location = new System.Drawing.Point(809, 320);
            this.checkBoxSum.Name = "checkBoxSum";
            this.checkBoxSum.Size = new System.Drawing.Size(83, 29);
            this.checkBoxSum.TabIndex = 21;
            this.checkBoxSum.Text = "Topla";
            this.checkBoxSum.UseVisualStyleBackColor = true;
            // 
            // Fonksiyon_Oluştur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1122, 649);
            this.Controls.Add(this.checkBoxSum);
            this.Controls.Add(this.checkBoxMin);
            this.Controls.Add(this.checkBoxMaks);
            this.Controls.Add(this.checkBoxCount);
            this.Controls.Add(this.pictureBox3_fonksiyonForm);
            this.Controls.Add(this.checkBox_cell_statistics);
            this.Controls.Add(this.buton_iptal_fonksiyonForm);
            this.Controls.Add(this.label_fonksiyonlar_fonksiyonForm);
            this.Controls.Add(this.buton_jabl);
            this.Controls.Add(this.pictureBox2_fonksiyonForm);
            this.Controls.Add(this.label_ikincil_katman_fonksiyonForm);
            this.Controls.Add(this.pictureBox1_fonksiyonForm);
            this.Controls.Add(this.label_birincil_katman_fonksiyonForm);
            this.Controls.Add(this.label_sütun_fonksiyonForm);
            this.Controls.Add(this.comboBox_fonksiyonlar_2);
            this.Controls.Add(this.secilen_sutunlar_fonksiyonForm);
            this.Controls.Add(this.comboBox_fonksiyonlar_1);
            this.Controls.Add(this.tum_sutunlar_fonksiyonForm);
            this.Controls.Add(this.label_agregasyon_fonksiyonForm);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Fonksiyon_Oluştur";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Fonksiyon Oluştur";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_fonksiyonForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2_fonksiyonForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3_fonksiyonForm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBox_fonksiyonlar_1;
        public System.Windows.Forms.ComboBox comboBox_fonksiyonlar_2;
        private System.Windows.Forms.Label label_birincil_katman_fonksiyonForm;
        private System.Windows.Forms.Label label_ikincil_katman_fonksiyonForm;
        private System.Windows.Forms.Button buton_jabl;
        private System.Windows.Forms.Button buton_iptal_fonksiyonForm;
        private System.Windows.Forms.CheckBox checkBox_cell_statistics;
        private System.Windows.Forms.Label label_agregasyon_fonksiyonForm;
        public System.Windows.Forms.ListBox tum_sutunlar_fonksiyonForm;
        public System.Windows.Forms.ListBox secilen_sutunlar_fonksiyonForm;
        private System.Windows.Forms.Label label_sütun_fonksiyonForm;
        private System.Windows.Forms.PictureBox pictureBox1_fonksiyonForm;
        private System.Windows.Forms.PictureBox pictureBox2_fonksiyonForm;
        private System.Windows.Forms.Label label_fonksiyonlar_fonksiyonForm;
        private System.Windows.Forms.PictureBox pictureBox3_fonksiyonForm;
        public System.Windows.Forms.CheckBox checkBoxCount;
        public System.Windows.Forms.CheckBox checkBoxMaks;
        public System.Windows.Forms.CheckBox checkBoxMin;
        public System.Windows.Forms.CheckBox checkBoxSum;
    }
}