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
            this.label1 = new System.Windows.Forms.Label();
            this.SelectCsvButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.SelectKmlButton = new System.Windows.Forms.Button();
            this.imarMethodSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.imarCitySelectionPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.imarFileSelectionPanel.SuspendLayout();
            this.imarCitySelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imarFileSelectionPanel
            // 
            this.imarFileSelectionPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.imarFileSelectionPanel.Controls.Add(this.label2);
            this.imarFileSelectionPanel.Controls.Add(this.OkButton);
            this.imarFileSelectionPanel.Controls.Add(this.SelectKmlButton);
            this.imarFileSelectionPanel.Location = new System.Drawing.Point(51, 279);
            this.imarFileSelectionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.imarFileSelectionPanel.Name = "imarFileSelectionPanel";
            this.imarFileSelectionPanel.Size = new System.Drawing.Size(395, 159);
            this.imarFileSelectionPanel.TabIndex = 5;
            this.imarFileSelectionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.imarFileSelectionPanel_Paint);
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
            // imarMethodSelectionComboBox
            // 
            this.imarMethodSelectionComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.imarMethodSelectionComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.imarMethodSelectionComboBox.FormattingEnabled = true;
            this.imarMethodSelectionComboBox.Items.AddRange(new object[] {
            "Verileri Güncelle",
            "Varolan Verileri Kullan"});
            this.imarMethodSelectionComboBox.Location = new System.Drawing.Point(100, 49);
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
            this.imarCitySelectionPanel.Location = new System.Drawing.Point(51, 110);
            this.imarCitySelectionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.imarCitySelectionPanel.Name = "imarCitySelectionPanel";
            this.imarCitySelectionPanel.Size = new System.Drawing.Size(395, 123);
            this.imarCitySelectionPanel.TabIndex = 7;
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
            // imarFileSelectionPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.ClientSize = new System.Drawing.Size(509, 558);
            this.Controls.Add(this.imarCitySelectionPanel);
            this.Controls.Add(this.imarMethodSelectionComboBox);
            this.Controls.Add(this.imarFileSelectionPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(527, 605);
            this.MinimumSize = new System.Drawing.Size(527, 605);
            this.Name = "imarFileSelectionPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İmar Girdileri Seçimi";
            this.imarFileSelectionPanel.ResumeLayout(false);
            this.imarFileSelectionPanel.PerformLayout();
            this.imarCitySelectionPanel.ResumeLayout(false);
            this.imarCitySelectionPanel.PerformLayout();
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
    }
}