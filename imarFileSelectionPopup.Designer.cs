using System.Web.UI.WebControls;

namespace SLF
{
    partial class imarFileSelectionPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(imarFileSelectionPopup));
            this.label2 = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.SelectKmlButton = new System.Windows.Forms.Button();
            this.imarCitySelectionPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label_imar_path = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_imar_test = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_imar_katman_listeleri = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TestCalıstır = new System.Windows.Forms.Button();
            this.KmlTestButton = new System.Windows.Forms.Button();
            this.imarCitySelectionPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label2.Location = new System.Drawing.Point(25, 77);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.MaximumSize = new System.Drawing.Size(150, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 28);
            this.label2.TabIndex = 3;
            this.label2.Text = "İmar Verisi Seç";
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.OkButton.ForeColor = System.Drawing.Color.White;
            this.OkButton.Location = new System.Drawing.Point(183, 144);
            this.OkButton.Margin = new System.Windows.Forms.Padding(4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(131, 47);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "Çalıştır";
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // SelectKmlButton
            // 
            this.SelectKmlButton.BackgroundImage = global::SLF.Properties.Resources.KML21;
            this.SelectKmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectKmlButton.Location = new System.Drawing.Point(183, 72);
            this.SelectKmlButton.Margin = new System.Windows.Forms.Padding(4);
            this.SelectKmlButton.Name = "SelectKmlButton";
            this.SelectKmlButton.Size = new System.Drawing.Size(44, 47);
            this.SelectKmlButton.TabIndex = 1;
            this.SelectKmlButton.UseVisualStyleBackColor = true;
            this.SelectKmlButton.Click += new System.EventHandler(this.SelectKmlButton_Click);
            // 
            // imarCitySelectionPanel
            // 
            this.imarCitySelectionPanel.BackColor = System.Drawing.Color.White;
            this.imarCitySelectionPanel.Controls.Add(this.label4);
            this.imarCitySelectionPanel.Controls.Add(this.label_imar_path);
            this.imarCitySelectionPanel.Controls.Add(this.OkButton);
            this.imarCitySelectionPanel.Controls.Add(this.label2);
            this.imarCitySelectionPanel.Controls.Add(this.SelectKmlButton);
            this.imarCitySelectionPanel.Location = new System.Drawing.Point(13, 262);
            this.imarCitySelectionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.imarCitySelectionPanel.Name = "imarCitySelectionPanel";
            this.imarCitySelectionPanel.Size = new System.Drawing.Size(521, 219);
            this.imarCitySelectionPanel.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift SemiBold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label4.Location = new System.Drawing.Point(143, 15);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 28);
            this.label4.TabIndex = 8;
            this.label4.Text = "İmar Analizleri Yap";
            // 
            // label_imar_path
            // 
            this.label_imar_path.AutoSize = true;
            this.label_imar_path.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_imar_path.Location = new System.Drawing.Point(234, 82);
            this.label_imar_path.MaximumSize = new System.Drawing.Size(200, 100);
            this.label_imar_path.Name = "label_imar_path";
            this.label_imar_path.Size = new System.Drawing.Size(55, 23);
            this.label_imar_path.TabIndex = 6;
            this.label_imar_path.Text = "label4";
            this.label_imar_path.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label_imar_test);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label_imar_katman_listeleri);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.TestCalıstır);
            this.panel1.Controls.Add(this.KmlTestButton);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 218);
            this.panel1.TabIndex = 6;
            // 
            // label_imar_test
            // 
            this.label_imar_test.AutoSize = true;
            this.label_imar_test.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_imar_test.Location = new System.Drawing.Point(259, 94);
            this.label_imar_test.MaximumSize = new System.Drawing.Size(200, 100);
            this.label_imar_test.Name = "label_imar_test";
            this.label_imar_test.Size = new System.Drawing.Size(55, 23);
            this.label_imar_test.TabIndex = 8;
            this.label_imar_test.Text = "label4";
            this.label_imar_test.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Bahnschrift SemiBold", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label5.Location = new System.Drawing.Point(126, 13);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(300, 28);
            this.label5.TabIndex = 6;
            this.label5.Text = "İmar Katmanları Eşleştirme";
            // 
            // label_imar_katman_listeleri
            // 
            this.label_imar_katman_listeleri.AutoSize = true;
            this.label_imar_katman_listeleri.BackColor = System.Drawing.Color.SeaShell;
            this.label_imar_katman_listeleri.Font = new System.Drawing.Font("Segoe UI", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_imar_katman_listeleri.ForeColor = System.Drawing.Color.Green;
            this.label_imar_katman_listeleri.Location = new System.Drawing.Point(144, 179);
            this.label_imar_katman_listeleri.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_imar_katman_listeleri.Name = "label_imar_katman_listeleri";
            this.label_imar_katman_listeleri.Size = new System.Drawing.Size(226, 23);
            this.label_imar_katman_listeleri.TabIndex = 3;
            this.label_imar_katman_listeleri.Text = "Katman Listesini Görüntüle";
            this.label_imar_katman_listeleri.Click += new System.EventHandler(this.label_imar_katman_listeleri_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.DarkOrange;
            this.label3.Location = new System.Drawing.Point(67, 69);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "Girdi Verisi Seç";
            // 
            // TestCalıstır
            // 
            this.TestCalıstır.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TestCalıstır.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.TestCalıstır.ForeColor = System.Drawing.Color.White;
            this.TestCalıstır.Location = new System.Drawing.Point(131, 98);
            this.TestCalıstır.Margin = new System.Windows.Forms.Padding(4);
            this.TestCalıstır.Name = "TestCalıstır";
            this.TestCalıstır.Size = new System.Drawing.Size(74, 38);
            this.TestCalıstır.TabIndex = 5;
            this.TestCalıstır.Text = "Çalıştır";
            this.TestCalıstır.UseVisualStyleBackColor = false;
            this.TestCalıstır.Click += new System.EventHandler(this.TestCalıstır_Click);
            // 
            // KmlTestButton
            // 
            this.KmlTestButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("KmlTestButton.BackgroundImage")));
            this.KmlTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.KmlTestButton.Location = new System.Drawing.Point(54, 94);
            this.KmlTestButton.Margin = new System.Windows.Forms.Padding(4);
            this.KmlTestButton.Name = "KmlTestButton";
            this.KmlTestButton.Size = new System.Drawing.Size(54, 47);
            this.KmlTestButton.TabIndex = 1;
            this.KmlTestButton.UseVisualStyleBackColor = true;
            this.KmlTestButton.Click += new System.EventHandler(this.KmlTestButton_Click);
            // 
            // imarFileSelectionPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(562, 498);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.imarCitySelectionPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(580, 675);
            this.MinimizeBox = false;
            this.Name = "imarFileSelectionPopup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İmar Girdileri Seçimi";
            this.imarCitySelectionPanel.ResumeLayout(false);
            this.imarCitySelectionPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button SelectKmlButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Panel imarCitySelectionPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button TestCalıstır;
        private System.Windows.Forms.Label label_imar_katman_listeleri;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button KmlTestButton;
        private System.Windows.Forms.Label label_imar_path;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_imar_test;
    }
}