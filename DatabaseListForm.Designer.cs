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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonAboneVerisiOlustur
            // 
            this.buttonAboneVerisiOlustur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.buttonAboneVerisiOlustur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonAboneVerisiOlustur.ForeColor = System.Drawing.Color.DarkOrange;
            this.buttonAboneVerisiOlustur.Location = new System.Drawing.Point(71, 96);
            this.buttonAboneVerisiOlustur.Name = "buttonAboneVerisiOlustur";
            this.buttonAboneVerisiOlustur.Size = new System.Drawing.Size(200, 76);
            this.buttonAboneVerisiOlustur.TabIndex = 1;
            this.buttonAboneVerisiOlustur.Text = "Abone Verisi Oluştur";
            this.buttonAboneVerisiOlustur.UseVisualStyleBackColor = false;
            this.buttonAboneVerisiOlustur.Click += new System.EventHandler(this.buttonAboneVerisiOlustur_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(26, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 48);
            this.label1.TabIndex = 2;
            this.label1.Text = "Database Modülü";
            // 
            // DatabaseListForm
            // 
            this.ClientSize = new System.Drawing.Size(378, 198);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAboneVerisiOlustur);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Veritabanı Veri İşlemleri";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button dtrVeriTabloOlustur;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button aboneVeriTablosuOlustur;
        private System.Windows.Forms.Label tablo_label;
        private System.Windows.Forms.Label label1;
    }
}