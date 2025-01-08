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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableData)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxTables
            // 
            this.listBoxTables.FormattingEnabled = true;
            this.listBoxTables.Location = new System.Drawing.Point(12, 22);
            this.listBoxTables.Name = "listBoxTables";
            this.listBoxTables.Size = new System.Drawing.Size(181, 420);
            this.listBoxTables.TabIndex = 0;
            this.listBoxTables.SelectedIndexChanged += new System.EventHandler(this.listBoxTables_SelectedIndexChanged);
            // 
            // dataGridViewTableData
            // 
            this.dataGridViewTableData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTableData.Location = new System.Drawing.Point(541, 26);
            this.dataGridViewTableData.Name = "dataGridViewTableData";
            this.dataGridViewTableData.RowHeadersWidth = 51;
            this.dataGridViewTableData.Size = new System.Drawing.Size(220, 416);
            this.dataGridViewTableData.TabIndex = 1;
            // 
            // dtrVeriTabloOlustur
            // 
            this.dtrVeriTabloOlustur.Location = new System.Drawing.Point(856, 37);
            this.dtrVeriTabloOlustur.Name = "dtrVeriTabloOlustur";
            this.dtrVeriTabloOlustur.Size = new System.Drawing.Size(163, 48);
            this.dtrVeriTabloOlustur.TabIndex = 2;
            this.dtrVeriTabloOlustur.Text = "DTR Tablolarını Oluştur";
            this.dtrVeriTabloOlustur.UseVisualStyleBackColor = true;
            this.dtrVeriTabloOlustur.Click += new System.EventHandler(this.dtrVerileriTabloOlustur);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(856, 188);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(8, 8);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dekTablosuOlustur
            // 
            this.dekTablosuOlustur.Location = new System.Drawing.Point(856, 113);
            this.dekTablosuOlustur.Name = "dekTablosuOlustur";
            this.dekTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.dekTablosuOlustur.TabIndex = 4;
            this.dekTablosuOlustur.Text = "DEK Tablosu Oluştur";
            this.dekTablosuOlustur.UseVisualStyleBackColor = true;
            this.dekTablosuOlustur.Click += new System.EventHandler(this.dekTablosuOlustur_Click);
            // 
            // EaSarjTablosuOlustur
            // 
            this.EaSarjTablosuOlustur.Location = new System.Drawing.Point(1063, 37);
            this.EaSarjTablosuOlustur.Name = "EaSarjTablosuOlustur";
            this.EaSarjTablosuOlustur.Size = new System.Drawing.Size(163, 48);
            this.EaSarjTablosuOlustur.TabIndex = 5;
            this.EaSarjTablosuOlustur.Text = "EA Şarj Tablosu Oluştur";
            this.EaSarjTablosuOlustur.UseVisualStyleBackColor = true;
            this.EaSarjTablosuOlustur.Click += new System.EventHandler(this.EaSarjTablosuOlustur_Click);
            // 
            // aboneVeriTablosuOlustur
            // 
            this.aboneVeriTablosuOlustur.Location = new System.Drawing.Point(1063, 113);
            this.aboneVeriTablosuOlustur.Name = "aboneVeriTablosuOlustur";
            this.aboneVeriTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.aboneVeriTablosuOlustur.TabIndex = 6;
            this.aboneVeriTablosuOlustur.Text = "Abone Veri Tablosu Oluştur";
            this.aboneVeriTablosuOlustur.UseVisualStyleBackColor = true;
            this.aboneVeriTablosuOlustur.Click += new System.EventHandler(this.aboneVeriTablosuOlustur_Click);
            // 
            // fiderVerileriTablosuOlustur
            // 
            this.fiderVerileriTablosuOlustur.Location = new System.Drawing.Point(856, 188);
            this.fiderVerileriTablosuOlustur.Name = "fiderVerileriTablosuOlustur";
            this.fiderVerileriTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.fiderVerileriTablosuOlustur.TabIndex = 7;
            this.fiderVerileriTablosuOlustur.Text = "Fider Verileri Tablosu Oluştur";
            this.fiderVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.fiderVerileriTablosuOlustur.Click += new System.EventHandler(this.fiderVerileriTablosuOlustur_Click);
            // 
            // enerjiMüsaadeleriTablosuOlustur
            // 
            this.enerjiMüsaadeleriTablosuOlustur.Location = new System.Drawing.Point(1063, 188);
            this.enerjiMüsaadeleriTablosuOlustur.Name = "enerjiMüsaadeleriTablosuOlustur";
            this.enerjiMüsaadeleriTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.enerjiMüsaadeleriTablosuOlustur.TabIndex = 8;
            this.enerjiMüsaadeleriTablosuOlustur.Text = "Enerji Müsaadeleri Tablosu Oluştur";
            this.enerjiMüsaadeleriTablosuOlustur.UseVisualStyleBackColor = true;
            this.enerjiMüsaadeleriTablosuOlustur.Click += new System.EventHandler(this.enerjiMüsaadeleriTablosuOlustur_Click);
            // 
            // YeniProjelendirilmisDTRVerileriTablosuOlustur
            // 
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Location = new System.Drawing.Point(856, 258);
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Name = "YeniProjelendirilmisDTRVerileriTablosuOlustur";
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.TabIndex = 9;
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Text = "Yeni Projelendirilmiş DTR Verileri Tablosu Oluştur";
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.YeniProjelendirilmisDTRVerileriTablosuOlustur.Click += new System.EventHandler(this.YeniProjelendirilmisDTRVerileriTablosuOlustur_Click);
            // 
            // imarVerileriTablosuOlustur
            // 
            this.imarVerileriTablosuOlustur.Location = new System.Drawing.Point(1063, 258);
            this.imarVerileriTablosuOlustur.Name = "imarVerileriTablosuOlustur";
            this.imarVerileriTablosuOlustur.Size = new System.Drawing.Size(163, 44);
            this.imarVerileriTablosuOlustur.TabIndex = 10;
            this.imarVerileriTablosuOlustur.Text = "İmar Verileri Tablosu Oluştur";
            this.imarVerileriTablosuOlustur.UseVisualStyleBackColor = true;
            this.imarVerileriTablosuOlustur.Click += new System.EventHandler(this.imarVerileriTablosuOlustur_Click);
            // 
            // listBoxCbsFiles
            // 
            this.listBoxCbsFiles.FormattingEnabled = true;
            this.listBoxCbsFiles.Location = new System.Drawing.Point(241, 23);
            this.listBoxCbsFiles.Name = "listBoxCbsFiles";
            this.listBoxCbsFiles.Size = new System.Drawing.Size(254, 420);
            this.listBoxCbsFiles.TabIndex = 11;
            this.listBoxCbsFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxCbsFiles_SelectedIndexChanged);
            // 
            // DatabaseListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1343, 559);
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
            this.Name = "DatabaseListForm";
            this.Text = "DatabaseListForm";
            this.Load += new System.EventHandler(this.DatabaseListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableData)).EndInit();
            this.ResumeLayout(false);

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
    }
}