namespace SLF.Optimal_DTR
{
    partial class DTR_Arayuz
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DTR_Arayuz));
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.buton_optimalDTR_katmanlar = new System.Windows.Forms.Button();
            this.harita_katmanları_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Arazi = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth = new System.Windows.Forms.ToolStripMenuItem();
            this.Harita = new System.Windows.Forms.ToolStripMenuItem();
            this.OSM = new System.Windows.Forms.ToolStripMenuItem();
            this.Uydu = new System.Windows.Forms.ToolStripMenuItem();
            this.button4 = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.harita_katmanları_right_click.SuspendLayout();
            this.SuspendLayout();
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(338, 78);
            this.gMapControl1.Margin = new System.Windows.Forms.Padding(20);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.Padding = new System.Windows.Forms.Padding(10);
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(1292, 579);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 2D;
            this.gMapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseMove);
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.Color.OldLace;
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(313, 690);
            this.panel3.TabIndex = 8;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.checkedListBox2);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Location = new System.Drawing.Point(10, 532);
            this.panel7.Margin = new System.Windows.Forms.Padding(1);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(256, 200);
            this.panel7.TabIndex = 13;
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Items.AddRange(new object[] {
            "Hepsi",
            "Mevcut",
            "Gerilim Donüşümü",
            "Deplase",
            "Güç Artırımı",
            "Projelendirilmiş Yeni Trafo",
            "Trafo Yenileme-Yaştan",
            "Trafo Yükseltme-Kapasiteden",
            "Trafo Yükseltme-Yükten",
            "Yeni Trafo Tesis"});
            this.checkedListBox2.Location = new System.Drawing.Point(18, 51);
            this.checkedListBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.ScrollAlwaysVisible = true;
            this.checkedListBox2.Size = new System.Drawing.Size(188, 96);
            this.checkedListBox2.TabIndex = 2;
            this.checkedListBox2.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Trafo Durumu";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.ForestGreen;
            this.button1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button1.Location = new System.Drawing.Point(10, 738);
            this.button1.Margin = new System.Windows.Forms.Padding(1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(223, 58);
            this.button1.TabIndex = 9;
            this.button1.Text = "Filtrele";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.checkedListBox1);
            this.panel4.Location = new System.Drawing.Point(10, 235);
            this.panel4.Margin = new System.Windows.Forms.Padding(1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(256, 260);
            this.panel4.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Yıl";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Hepsi",
            "2024",
            "2025",
            "2026",
            "2027",
            "2028",
            "2029",
            "2030",
            "2031",
            "2032",
            "2033",
            "2034",
            "2035"});
            this.checkedListBox1.Location = new System.Drawing.Point(18, 36);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.ScrollAlwaysVisible = true;
            this.checkedListBox1.Size = new System.Drawing.Size(188, 188);
            this.checkedListBox1.TabIndex = 1;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioButton4);
            this.panel2.Controls.Add(this.radioButton3);
            this.panel2.Controls.Add(this.radioButton2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(10, 36);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(256, 162);
            this.panel2.TabIndex = 2;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Checked = true;
            this.radioButton4.Location = new System.Drawing.Point(14, 122);
            this.radioButton4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(75, 24);
            this.radioButton4.TabIndex = 8;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Hepsi";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(14, 90);
            this.radioButton3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(66, 24);
            this.radioButton3.TabIndex = 7;
            this.radioButton3.Text = "Özel";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(14, 51);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(80, 24);
            this.radioButton2.TabIndex = 6;
            this.radioButton2.Text = "Kurum";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Trafo Mülkiyeti";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.Location = new System.Drawing.Point(722, 11);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(298, 51);
            this.button2.TabIndex = 11;
            this.button2.Text = "Çalıştır";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.button3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button3.Location = new System.Drawing.Point(543, 11);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(137, 51);
            this.button3.TabIndex = 12;
            this.button3.Text = "Dosya Seç";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // buton_optimalDTR_katmanlar
            // 
            this.buton_optimalDTR_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_optimalDTR_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_optimalDTR_katmanlar.BackgroundImage")));
            this.buton_optimalDTR_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_optimalDTR_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_optimalDTR_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_optimalDTR_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_optimalDTR_katmanlar.Location = new System.Drawing.Point(338, 591);
            this.buton_optimalDTR_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_optimalDTR_katmanlar.Name = "buton_optimalDTR_katmanlar";
            this.buton_optimalDTR_katmanlar.Size = new System.Drawing.Size(65, 65);
            this.buton_optimalDTR_katmanlar.TabIndex = 42;
            this.buton_optimalDTR_katmanlar.UseVisualStyleBackColor = true;
            // 
            // harita_katmanları_right_click
            // 
            this.harita_katmanları_right_click.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.harita_katmanları_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Arazi,
            this.Google_Earth,
            this.Harita,
            this.OSM,
            this.Uydu});
            this.harita_katmanları_right_click.Name = "harita_katmanları_right_click";
            this.harita_katmanları_right_click.Size = new System.Drawing.Size(224, 164);
            // 
            // Arazi
            // 
            this.Arazi.Image = ((System.Drawing.Image)(resources.GetObject("Arazi.Image")));
            this.Arazi.Name = "Arazi";
            this.Arazi.Size = new System.Drawing.Size(223, 32);
            this.Arazi.Text = "Arazi";
            this.Arazi.Click += new System.EventHandler(this.Arazi_Click);
            // 
            // Google_Earth
            // 
            this.Google_Earth.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth.Image")));
            this.Google_Earth.Name = "Google_Earth";
            this.Google_Earth.Size = new System.Drawing.Size(223, 32);
            this.Google_Earth.Text = "GE Online";
            this.Google_Earth.Click += new System.EventHandler(this.Google_Earth_Click);
            // 
            // Harita
            // 
            this.Harita.Image = ((System.Drawing.Image)(resources.GetObject("Harita.Image")));
            this.Harita.Name = "Harita";
            this.Harita.Size = new System.Drawing.Size(223, 32);
            this.Harita.Text = "Harita";
            this.Harita.Click += new System.EventHandler(this.Harita_Click);
            // 
            // OSM
            // 
            this.OSM.Image = ((System.Drawing.Image)(resources.GetObject("OSM.Image")));
            this.OSM.Name = "OSM";
            this.OSM.Size = new System.Drawing.Size(223, 32);
            this.OSM.Text = "Open Street Map";
            this.OSM.Click += new System.EventHandler(this.OSM_Click);
            // 
            // Uydu
            // 
            this.Uydu.Image = ((System.Drawing.Image)(resources.GetObject("Uydu.Image")));
            this.Uydu.Name = "Uydu";
            this.Uydu.Size = new System.Drawing.Size(223, 32);
            this.Uydu.Text = "Uydu";
            this.Uydu.Click += new System.EventHandler(this.Uydu_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.button4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button4.Location = new System.Drawing.Point(360, 11);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(137, 51);
            this.button4.TabIndex = 43;
            this.button4.Text = "Parametreler";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // DTR_Arayuz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1660, 690);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.buton_optimalDTR_katmanlar);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.gMapControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "DTR_Arayuz";
            this.Text = "Optimal DTR Konumlandırma";
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.harita_katmanları_right_click.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox checkedListBox2;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>


        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button buton_optimalDTR_katmanlar;
        private System.Windows.Forms.ContextMenuStrip harita_katmanları_right_click;
        private System.Windows.Forms.ToolStripMenuItem Arazi;
        private System.Windows.Forms.ToolStripMenuItem Google_Earth;
        private System.Windows.Forms.ToolStripMenuItem Harita;
        private System.Windows.Forms.ToolStripMenuItem OSM;
        private System.Windows.Forms.ToolStripMenuItem Uydu;
        private System.Windows.Forms.Button button4;
    }
}