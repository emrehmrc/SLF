namespace SLF
{
    partial class DatabaseListForm
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
            this.listBoxTables = new System.Windows.Forms.ListBox();
            this.dataGridViewTableData = new System.Windows.Forms.DataGridView();
            this.dtrVeriTabloOlustur = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dekTablosuOlustur = new System.Windows.Forms.Button();
            this.EaSarjTablosuOlustur = new System.Windows.Forms.Button();
            this.aboneVeriTablosuOlustur = new System.Windows.Forms.Button();
            this.fiderVerileriTablosuOlustur = new System.Windows.Forms.Button();
            this.enerjiMüsaadeleriTablosuOlustur = new System.Windows.Forms.Button();
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur = new System.Windows.Forms.Button();
            this.imarVerileriTablosuOlustur = new System.Windows.Forms.Button();
            this.listBoxCbsFiles = new System.Windows.Forms.ListBox();
            this.mevcut_Dtr = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tablo_label = new System.Windows.Forms.Label();
            this.Database_tablo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableData)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxTables
            // 
            this.listBoxTables.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.listBoxTables.Font = new System.Drawing.Font("Segoe Fluent Icons", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxTables.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.listBoxTables.FormattingEnabled = true;
            this.listBoxTables.ItemHeight = 24;
            this.listBoxTables.Location = new System.Drawing.Point(28, 47);
            this.listBoxTables.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxTables.Name = "listBoxTables";
            this.listBoxTables.ScrollAlwaysVisible = true;
            this.listBoxTables.Size = new System.Drawing.Size(289, 532);
            this.listBoxTables.TabIndex = 0;
            this.listBoxTables.SelectedIndexChanged += new System.EventHandler(this.listBoxTables_SelectedIndexChanged);
            // 
            // dataGridViewTableData
            // 
            this.dataGridViewTableData.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTableData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTableData.Location = new System.Drawing.Point(780, 47);
            this.dataGridViewTableData.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewTableData.Name = "dataGridViewTableData";
            this.dataGridViewTableData.RowHeadersWidth = 51;
            this.dataGridViewTableData.Size = new System.Drawing.Size(372, 530);
            this.dataGridViewTableData.TabIndex = 1;
            // 
            // dtrVeriTabloOlustur
            // 
            this.dtrVeriTabloOlustur.BackColor = System.Drawing.Color.Cornsilk;
            this.dtrVeriTabloOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtrVeriTabloOlustur.ForeColor = System.Drawing.Color.DarkOrange;
            this.dtrVeriTabloOlustur.Location = new System.Drawing.Point(1219, 106);
            this.dtrVeriTabloOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.dtrVeriTabloOlustur.Name = "dtrVeriTabloOlustur";
            this.dtrVeriTabloOlustur.Size = new System.Drawing.Size(217, 59);
            this.dtrVeriTabloOlustur.TabIndex = 2;
            this.dtrVeriTabloOlustur.Text = "DTR Tablosu Oluştur";
            this.dtrVeriTabloOlustur.UseVisualStyleBackColor = false;
            this.dtrVeriTabloOlustur.Click += new System.EventHandler(this.dtrVerileriTabloOlustur);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1141, 231);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(11, 10);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dekTablosuOlustur
            // 
            this.dekTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dekTablosuOlustur.Location = new System.Drawing.Point(1465, 279);
            this.dekTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.dekTablosuOlustur.Name = "dekTablosuOlustur";
            this.dekTablosuOlustur.Size = new System.Drawing.Size(217, 59);
            this.dekTablosuOlustur.TabIndex = 4;
            this.dekTablosuOlustur.Text = "DEK  Verilerini İçe Aktar";
            this.dekTablosuOlustur.UseVisualStyleBackColor = true;
            this.dekTablosuOlustur.Click += new System.EventHandler(this.dekTablosuOlustur_Click);
            // 
            // EaSarjTablosuOlustur
            // 
            this.EaSarjTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EaSarjTablosuOlustur.Location = new System.Drawing.Point(1465, 449);
            this.EaSarjTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.EaSarjTablosuOlustur.Name = "EaSarjTablosuOlustur";
            this.EaSarjTablosuOlustur.Size = new System.Drawing.Size(217, 59);
            this.EaSarjTablosuOlustur.TabIndex = 5;
            this.EaSarjTablosuOlustur.Text = "EA Şarj Veri Tablosu Oluştur";
            this.EaSarjTablosuOlustur.UseVisualStyleBackColor = true;
            this.EaSarjTablosuOlustur.Click += new System.EventHandler(this.EaSarjTablosuOlustur_Click);
            // 
            // aboneVeriTablosuOlustur
            // 
            this.aboneVeriTablosuOlustur.BackColor = System.Drawing.Color.Cornsilk;
            this.aboneVeriTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboneVeriTablosuOlustur.Location = new System.Drawing.Point(1465, 106);
            this.aboneVeriTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.aboneVeriTablosuOlustur.Name = "aboneVeriTablosuOlustur";
            this.aboneVeriTablosuOlustur.Size = new System.Drawing.Size(217, 59);
            this.aboneVeriTablosuOlustur.TabIndex = 6;
            this.aboneVeriTablosuOlustur.Text = "Abone Tablosu Oluştur";
            this.aboneVeriTablosuOlustur.UseVisualStyleBackColor = false;
            this.aboneVeriTablosuOlustur.Click += new System.EventHandler(this.aboneVeriTablosuOlustur_Click);
            // 
            // fiderVerileriTablosuOlustur
            // 
            this.fiderVerileriTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fiderVerileriTablosuOlustur.Location = new System.Drawing.Point(1465, 366);
            this.fiderVerileriTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.fiderVerileriTablosuOlustur.Name = "fiderVerileriTablosuOlustur";
            this.fiderVerileriTablosuOlustur.Size = new System.Drawing.Size(217, 59);
            this.fiderVerileriTablosuOlustur.TabIndex = 7;
            this.fiderVerileriTablosuOlustur.Text = "Fider Verileri  Verilerini İçe Aktar";
            this.fiderVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.fiderVerileriTablosuOlustur.Click += new System.EventHandler(this.fiderVerileriTablosuOlustur_Click);
            // 
            // enerjiMüsaadeleriTablosuOlustur
            // 
            this.enerjiMüsaadeleriTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enerjiMüsaadeleriTablosuOlustur.Location = new System.Drawing.Point(1219, 449);
            this.enerjiMüsaadeleriTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.enerjiMüsaadeleriTablosuOlustur.Name = "enerjiMüsaadeleriTablosuOlustur";
            this.enerjiMüsaadeleriTablosuOlustur.Size = new System.Drawing.Size(217, 54);
            this.enerjiMüsaadeleriTablosuOlustur.TabIndex = 8;
            this.enerjiMüsaadeleriTablosuOlustur.Text = "Enerji Müsaadeleri Verilerini İçe Aktar";
            this.enerjiMüsaadeleriTablosuOlustur.UseVisualStyleBackColor = true;
            this.enerjiMüsaadeleriTablosuOlustur.Click += new System.EventHandler(this.enerjiMüsaadeleriTablosuOlustur_Click);
            // 
            // YeniProjelendirilmisDTRVerileriTablosuOlustur
            // 
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Location = new System.Drawing.Point(1219, 533);
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Name = "YeniProjelendirilmisDTRVerileriTablosuOlustur";
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Size = new System.Drawing.Size(217, 54);
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.TabIndex = 9;
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Text = "Yeni Projelendirilmiş Verilerini İçe Aktar";
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Click += new System.EventHandler(this.YeniProjelendirilmisDTRVerileriTablosuOlustur_Click);
            // 
            // imarVerileriTablosuOlustur
            // 
            this.imarVerileriTablosuOlustur.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imarVerileriTablosuOlustur.Location = new System.Drawing.Point(1465, 533);
            this.imarVerileriTablosuOlustur.Margin = new System.Windows.Forms.Padding(4);
            this.imarVerileriTablosuOlustur.Name = "imarVerileriTablosuOlustur";
            this.imarVerileriTablosuOlustur.Size = new System.Drawing.Size(217, 54);
            this.imarVerileriTablosuOlustur.TabIndex = 10;
            this.imarVerileriTablosuOlustur.Text = "İmar Verileri Tablosu Oluştur";
            this.imarVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.imarVerileriTablosuOlustur.Click += new System.EventHandler(this.imarVerileriTablosuOlustur_Click);
            // 
            // listBoxCbsFiles
            // 
            this.listBoxCbsFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxCbsFiles.Font = new System.Drawing.Font("Segoe Fluent Icons", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxCbsFiles.ForeColor = System.Drawing.Color.Black;
            this.listBoxCbsFiles.FormattingEnabled = true;
            this.listBoxCbsFiles.ItemHeight = 24;
            this.listBoxCbsFiles.Location = new System.Drawing.Point(364, 47);
            this.listBoxCbsFiles.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxCbsFiles.Name = "listBoxCbsFiles";
            this.listBoxCbsFiles.ScrollAlwaysVisible = true;
            this.listBoxCbsFiles.Size = new System.Drawing.Size(363, 530);
            this.listBoxCbsFiles.TabIndex = 11;
            this.listBoxCbsFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxCbsFiles_SelectedIndexChanged);
            // 
            // mevcut_Dtr
            // 
            this.mevcut_Dtr.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mevcut_Dtr.Location = new System.Drawing.Point(1219, 366);
            this.mevcut_Dtr.Margin = new System.Windows.Forms.Padding(4);
            this.mevcut_Dtr.Name = "mevcut_Dtr";
            this.mevcut_Dtr.Size = new System.Drawing.Size(217, 59);
            this.mevcut_Dtr.TabIndex = 12;
            this.mevcut_Dtr.Text = "Abone Verilerini İçe Aktar";
            this.mevcut_Dtr.UseVisualStyleBackColor = true;
            this.mevcut_Dtr.Click += new System.EventHandler(this.mevcut_Dtr_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(1219, 279);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(217, 59);
            this.button1.TabIndex = 13;
            this.button1.Text = "DTR Verilerini İçe Aktar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tablo_label
            // 
            this.tablo_label.AutoSize = true;
            this.tablo_label.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tablo_label.Location = new System.Drawing.Point(1329, 23);
            this.tablo_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tablo_label.Name = "tablo_label";
            this.tablo_label.Size = new System.Drawing.Size(246, 48);
            this.tablo_label.TabIndex = 14;
            this.tablo_label.Text = "Tablo Oluştur";
            // 
            // Database_tablo
            // 
            this.Database_tablo.AutoSize = true;
            this.Database_tablo.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Database_tablo.Location = new System.Drawing.Point(1329, 206);
            this.Database_tablo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Database_tablo.Name = "Database_tablo";
            this.Database_tablo.Size = new System.Drawing.Size(265, 48);
            this.Database_tablo.TabIndex = 15;
            this.Database_tablo.Text = "Database Giriş";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(61, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 25);
            this.label1.TabIndex = 16;
            this.label1.Text = "Database Tabloları";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(477, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 25);
            this.label2.TabIndex = 17;
            this.label2.Text = "CBS Tabloları";
            // 
            // DatabaseListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1791, 688);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Database_tablo);
            this.Controls.Add(this.tablo_label);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mevcut_Dtr);
            this.Controls.Add(this.listBoxCbsFiles);
            this.Controls.Add(this.imarVerileriTablosuOlustur);
            this.Controls.Add(this.YeniProjelendirilmisDTRVerileriTablosuOlustur);
            this.Controls.Add(this.enerjiMüsaadeleriTablosuOlustur);
            this.Controls.Add(this.fiderVerileriTablosuOlustur);
            this.Controls.Add(this.aboneVeriTablosuOlustur);
            this.Controls.Add(this.EaSarjTablosuOlustur);
            this.Controls.Add(this.dekTablosuOlustur);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dtrVeriTabloOlustur);
            this.Controls.Add(this.dataGridViewTableData);
            this.Controls.Add(this.listBoxTables);
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DatabaseListForm";
            this.Text = "DatabaseListForm";
            this.Load += new System.EventHandler(this.DatabaseListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTables;
        private System.Windows.Forms.DataGridView dataGridViewTableData;
        private System.Windows.Forms.Button dtrVeriTabloOlustur;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button dekTablosuOlustur;
        private System.Windows.Forms.Button EaSarjTablosuOlustur;
        private System.Windows.Forms.Button aboneVeriTablosuOlustur;
        private System.Windows.Forms.Button fiderVerileriTablosuOlustur;
        private System.Windows.Forms.Button enerjiMüsaadeleriTablosuOlustur;
        private System.Windows.Forms.Button YeniProjelendirilmisDTRVerileriTablosuOlustur;
        private System.Windows.Forms.Button imarVerileriTablosuOlustur;
        private System.Windows.Forms.ListBox listBoxCbsFiles;
        private System.Windows.Forms.Button mevcut_Dtr;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label tablo_label;
        private System.Windows.Forms.Label Database_tablo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}