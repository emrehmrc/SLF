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
            this.imarFileSelectionPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.SelectKmlButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectCsvButton = new System.Windows.Forms.Button();
            this.imarMethodSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.imarCitySelectionPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.katman_tablosu = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TestCalıstır = new System.Windows.Forms.Button();
            this.KmlTestButton = new System.Windows.Forms.Button();
            this.imarFileSelectionPanel.SuspendLayout();
            this.imarCitySelectionPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imarFileSelectionPanel
            // 
            this.imarFileSelectionPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.imarFileSelectionPanel.Controls.Add(this.label2);
            this.imarFileSelectionPanel.Controls.Add(this.OkButton);
            this.imarFileSelectionPanel.Controls.Add(this.SelectKmlButton);
            this.imarFileSelectionPanel.Location = new System.Drawing.Point(35, 441);
            this.imarFileSelectionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.imarFileSelectionPanel.Name = "imarFileSelectionPanel";
            this.imarFileSelectionPanel.Size = new System.Drawing.Size(395, 159);
            this.imarFileSelectionPanel.TabIndex = 5;
            this.imarFileSelectionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.imarFileSelectionPanel_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.DarkOrange;
            this.label2.Location = new System.Drawing.Point(122, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "KML Veri Seçimi;";
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OkButton.ForeColor = System.Drawing.Color.White;
            this.OkButton.Location = new System.Drawing.Point(126, 109);
            this.OkButton.Margin = new System.Windows.Forms.Padding(4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(100, 28);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "TAMAM";
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // SelectKmlButton
            // 
            this.SelectKmlButton.BackgroundImage = global::SLF.Properties.Resources.KML21;
            this.SelectKmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectKmlButton.Location = new System.Drawing.Point(163, 54);
            this.SelectKmlButton.Margin = new System.Windows.Forms.Padding(4);
            this.SelectKmlButton.Name = "SelectKmlButton";
            this.SelectKmlButton.Size = new System.Drawing.Size(44, 47);
            this.SelectKmlButton.TabIndex = 1;
            this.SelectKmlButton.UseVisualStyleBackColor = true;
            this.SelectKmlButton.Click += new System.EventHandler(this.SelectKmlButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(102, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Overpass Veri Seçimi";
            // 
            // SelectCsvButton
            // 
            this.SelectCsvButton.BackgroundImage = global::SLF.Properties.Resources.CSV21;
            this.SelectCsvButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectCsvButton.Location = new System.Drawing.Point(163, 54);
            this.SelectCsvButton.Margin = new System.Windows.Forms.Padding(4);
            this.SelectCsvButton.Name = "SelectCsvButton";
            this.SelectCsvButton.Size = new System.Drawing.Size(44, 47);
            this.SelectCsvButton.TabIndex = 0;
            this.SelectCsvButton.UseVisualStyleBackColor = true;
            this.SelectCsvButton.Click += new System.EventHandler(this.SelectCsvButton_Click);
            // 
            // imarMethodSelectionComboBox
            // 
            this.imarMethodSelectionComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.imarMethodSelectionComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.imarMethodSelectionComboBox.FormattingEnabled = true;
            this.imarMethodSelectionComboBox.Items.AddRange(new object[] {
            "Verileri Güncelle",
            "Varolan Verileri Kullan"});
            this.imarMethodSelectionComboBox.Location = new System.Drawing.Point(66, 13);
            this.imarMethodSelectionComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.imarMethodSelectionComboBox.Name = "imarMethodSelectionComboBox";
            this.imarMethodSelectionComboBox.Size = new System.Drawing.Size(295, 29);
            this.imarMethodSelectionComboBox.TabIndex = 6;
            this.imarMethodSelectionComboBox.Text = "Yapmak istediğiniz işlemi seçiniz.";
            // 
            // imarCitySelectionPanel
            // 
            this.imarCitySelectionPanel.Controls.Add(this.label1);
            this.imarCitySelectionPanel.Controls.Add(this.SelectCsvButton);
            this.imarCitySelectionPanel.Location = new System.Drawing.Point(35, 265);
            this.imarCitySelectionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.imarCitySelectionPanel.Name = "imarCitySelectionPanel";
            this.imarCitySelectionPanel.Size = new System.Drawing.Size(395, 123);
            this.imarCitySelectionPanel.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.katman_tablosu);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.TestCalıstır);
            this.panel1.Controls.Add(this.KmlTestButton);
            this.panel1.Location = new System.Drawing.Point(35, 62);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 170);
            this.panel1.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.ForeColor = System.Drawing.Color.DarkOrange;
            this.label5.Location = new System.Drawing.Point(94, 13);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 23);
            this.label5.TabIndex = 6;
            this.label5.Text = "İmar Verileri Test Bölümü";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.ForeColor = System.Drawing.Color.DarkOrange;
            this.label4.Location = new System.Drawing.Point(6, 54);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(201, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "Katman Listesi Görüntüle";
            // 
            // katman_tablosu
            // 
            this.katman_tablosu.BackgroundImage = global::SLF.Properties.Resources.CSV21;
            this.katman_tablosu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.katman_tablosu.Location = new System.Drawing.Point(37, 90);
            this.katman_tablosu.Margin = new System.Windows.Forms.Padding(4);
            this.katman_tablosu.Name = "katman_tablosu";
            this.katman_tablosu.Size = new System.Drawing.Size(44, 47);
            this.katman_tablosu.TabIndex = 3;
            this.katman_tablosu.UseVisualStyleBackColor = true;
            this.katman_tablosu.Click += new System.EventHandler(this.katman_tablosu_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.DarkOrange;
            this.label3.Location = new System.Drawing.Point(251, 54);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "KML Veri Seçimi ;";
            // 
            // TestCalıstır
            // 
            this.TestCalıstır.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TestCalıstır.ForeColor = System.Drawing.Color.White;
            this.TestCalıstır.Location = new System.Drawing.Point(126, 109);
            this.TestCalıstır.Margin = new System.Windows.Forms.Padding(4);
            this.TestCalıstır.Name = "TestCalıstır";
            this.TestCalıstır.Size = new System.Drawing.Size(100, 28);
            this.TestCalıstır.TabIndex = 5;
            this.TestCalıstır.Text = "Test Çalıştır";
            this.TestCalıstır.UseVisualStyleBackColor = false;
            this.TestCalıstır.Click += new System.EventHandler(this.TestCalıstır_Click);
            // 
            // KmlTestButton
            // 
            this.KmlTestButton.BackgroundImage = global::SLF.Properties.Resources.KML21;
            this.KmlTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.KmlTestButton.Location = new System.Drawing.Point(282, 90);
            this.KmlTestButton.Margin = new System.Windows.Forms.Padding(4);
            this.KmlTestButton.Name = "KmlTestButton";
            this.KmlTestButton.Size = new System.Drawing.Size(44, 47);
            this.KmlTestButton.TabIndex = 1;
            this.KmlTestButton.UseVisualStyleBackColor = true;
            this.KmlTestButton.Click += new System.EventHandler(this.KmlTestButton_Click);
            // 
            // imarFileSelectionPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.ClientSize = new System.Drawing.Size(509, 622);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.imarCitySelectionPanel);
            this.Controls.Add(this.imarMethodSelectionComboBox);
            this.Controls.Add(this.imarFileSelectionPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(527, 605);
            this.Name = "imarFileSelectionPopup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İmar Girdileri Seçimi";
            this.imarFileSelectionPanel.ResumeLayout(false);
            this.imarFileSelectionPanel.PerformLayout();
            this.imarCitySelectionPanel.ResumeLayout(false);
            this.imarCitySelectionPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SelectCsvButton;
        private System.Windows.Forms.Button SelectKmlButton;
        private System.Windows.Forms.Panel imarFileSelectionPanel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ComboBox imarMethodSelectionComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel imarCitySelectionPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button TestCalıstır;
        private System.Windows.Forms.Button KmlTestButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button katman_tablosu;
        private System.Windows.Forms.Label label5;
    }
}