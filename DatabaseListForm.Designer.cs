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
        /// private System.Windows.Forms.Button buttonDtrVerisiOlustur;
        private System.Windows.Forms.Button buttonAboneVerisiOlustur;
        private System.Windows.Forms.Button buttonDtrVerisiOlustur;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseListForm));
            this.buttonAboneVerisiOlustur = new System.Windows.Forms.Button();
            this.buttonDTRVerileriniOlustur = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAboneVerisiOlustur
            // 
            this.buttonAboneVerisiOlustur.BackColor = System.Drawing.Color.Green;
            this.buttonAboneVerisiOlustur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonAboneVerisiOlustur.ForeColor = System.Drawing.Color.White;
            this.buttonAboneVerisiOlustur.Location = new System.Drawing.Point(40, 43);
            this.buttonAboneVerisiOlustur.Name = "buttonAboneVerisiOlustur";
            this.buttonAboneVerisiOlustur.Size = new System.Drawing.Size(345, 81);
            this.buttonAboneVerisiOlustur.TabIndex = 1;
            this.buttonAboneVerisiOlustur.Text = "Abone Verilerini Oluştur";
            this.buttonAboneVerisiOlustur.UseVisualStyleBackColor = false;
            this.buttonAboneVerisiOlustur.Click += new System.EventHandler(this.buttonAboneVerisiOlustur_Click);
            // 
            // buttonDTRVerileriniOlustur
            // 
            this.buttonDTRVerileriniOlustur.BackColor = System.Drawing.Color.DarkSlateGray;
            this.buttonDTRVerileriniOlustur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonDTRVerileriniOlustur.ForeColor = System.Drawing.Color.White;
            this.buttonDTRVerileriniOlustur.Location = new System.Drawing.Point(40, 189);
            this.buttonDTRVerileriniOlustur.Name = "buttonDTRVerileriniOlustur";
            this.buttonDTRVerileriniOlustur.Size = new System.Drawing.Size(345, 81);
            this.buttonDTRVerileriniOlustur.TabIndex = 2;
            this.buttonDTRVerileriniOlustur.Text = "DTR Verilerini Oluştur";
            this.buttonDTRVerileriniOlustur.UseVisualStyleBackColor = false;
            // 
            // DatabaseListForm
            // 
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(438, 346);
            this.Controls.Add(this.buttonDTRVerileriniOlustur);
            this.Controls.Add(this.buttonAboneVerisiOlustur);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Veritabanı İşlemleri";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button dtrVeriTabloOlustur;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button aboneVeriTablosuOlustur;
        private System.Windows.Forms.Label tablo_label;
        private System.Windows.Forms.Button buttonDTRVerileriniOlustur;
    }
}