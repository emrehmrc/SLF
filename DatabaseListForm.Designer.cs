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
            this.buttonDtrVerisiOlustur = new System.Windows.Forms.Button();
            this.buttonAboneVerisiOlustur = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonDtrVerisiOlustur
            // 
            this.buttonDtrVerisiOlustur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.buttonDtrVerisiOlustur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonDtrVerisiOlustur.Location = new System.Drawing.Point(150, 74);
            this.buttonDtrVerisiOlustur.Name = "buttonDtrVerisiOlustur";
            this.buttonDtrVerisiOlustur.Size = new System.Drawing.Size(200, 76);
            this.buttonDtrVerisiOlustur.TabIndex = 0;
            this.buttonDtrVerisiOlustur.Text = "DTR Verisi Oluştur";
            this.buttonDtrVerisiOlustur.UseVisualStyleBackColor = false;
            this.buttonDtrVerisiOlustur.Click += new System.EventHandler(this.buttonDtrVerisiOlustur_Click);
            // 
            // buttonAboneVerisiOlustur
            // 
            this.buttonAboneVerisiOlustur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.buttonAboneVerisiOlustur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonAboneVerisiOlustur.Location = new System.Drawing.Point(401, 74);
            this.buttonAboneVerisiOlustur.Name = "buttonAboneVerisiOlustur";
            this.buttonAboneVerisiOlustur.Size = new System.Drawing.Size(200, 76);
            this.buttonAboneVerisiOlustur.TabIndex = 1;
            this.buttonAboneVerisiOlustur.Text = "Abone Verisi Oluştur";
            this.buttonAboneVerisiOlustur.UseVisualStyleBackColor = false;
            this.buttonAboneVerisiOlustur.Click += new System.EventHandler(this.buttonAboneVerisiOlustur_Click);
            // 
            // DatabaseListForm
            // 
            this.ClientSize = new System.Drawing.Size(783, 350);
            this.Controls.Add(this.buttonDtrVerisiOlustur);
            this.Controls.Add(this.buttonAboneVerisiOlustur);
            this.Name = "DatabaseListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Veritabanı Veri İşlemleri";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button dtrVeriTabloOlustur;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button aboneVeriTablosuOlustur;
        private System.Windows.Forms.Label tablo_label;
    }
}