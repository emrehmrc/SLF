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
            this.SelectKmlButton = new System.Windows.Forms.Button();
            this.imarMethodSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.imarCitySelectionPanel = new System.Windows.Forms.Panel();
            this.imarEskisehirRadioButton = new System.Windows.Forms.RadioButton();
            this.imarizmirRadioButton = new System.Windows.Forms.RadioButton();
            this.OkButton = new SLF.CustomButton();
            this.imarFileSelectionPanel.SuspendLayout();
            this.imarCitySelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imarFileSelectionPanel
            // 
            this.imarFileSelectionPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.imarFileSelectionPanel.Controls.Add(this.label1);
            this.imarFileSelectionPanel.Controls.Add(this.SelectCsvButton);
            this.imarFileSelectionPanel.Controls.Add(this.SelectKmlButton);
            this.imarFileSelectionPanel.Location = new System.Drawing.Point(38, 227);
            this.imarFileSelectionPanel.Name = "imarFileSelectionPanel";
            this.imarFileSelectionPanel.Size = new System.Drawing.Size(296, 118);
            this.imarFileSelectionPanel.TabIndex = 5;
            this.imarFileSelectionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.imarFileSelectionPanel_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Yüklenecek Dosyalar Seçimi:";
            // 
            // SelectCsvButton
            // 
            this.SelectCsvButton.BackgroundImage = global::SLF.Properties.Resources.CSV21;
            this.SelectCsvButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectCsvButton.Location = new System.Drawing.Point(75, 44);
            this.SelectCsvButton.Name = "SelectCsvButton";
            this.SelectCsvButton.Size = new System.Drawing.Size(33, 38);
            this.SelectCsvButton.TabIndex = 0;
            this.SelectCsvButton.UseVisualStyleBackColor = true;
            this.SelectCsvButton.Click += new System.EventHandler(this.SelectCsvButton_Click);
            // 
            // SelectKmlButton
            // 
            this.SelectKmlButton.BackgroundImage = global::SLF.Properties.Resources.KML21;
            this.SelectKmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SelectKmlButton.Location = new System.Drawing.Point(167, 44);
            this.SelectKmlButton.Name = "SelectKmlButton";
            this.SelectKmlButton.Size = new System.Drawing.Size(33, 38);
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
            this.imarMethodSelectionComboBox.Location = new System.Drawing.Point(75, 40);
            this.imarMethodSelectionComboBox.Name = "imarMethodSelectionComboBox";
            this.imarMethodSelectionComboBox.Size = new System.Drawing.Size(222, 25);
            this.imarMethodSelectionComboBox.TabIndex = 6;
            this.imarMethodSelectionComboBox.Text = "Yapmak istediğiniz işlemi seçiniz.";
           // this.imarMethodSelectionComboBox.SelectedIndexChanged += new System.EventHandler(this.imarMethodSelectionComboBox_SelectedIndexChanged);
            // 
            // imarCitySelectionPanel
            // 
            this.imarCitySelectionPanel.Controls.Add(this.imarEskisehirRadioButton);
            this.imarCitySelectionPanel.Controls.Add(this.imarizmirRadioButton);
            this.imarCitySelectionPanel.Location = new System.Drawing.Point(38, 89);
            this.imarCitySelectionPanel.Name = "imarCitySelectionPanel";
            this.imarCitySelectionPanel.Size = new System.Drawing.Size(296, 100);
            this.imarCitySelectionPanel.TabIndex = 7;
            // 
            // imarEskisehirRadioButton
            // 
            this.imarEskisehirRadioButton.AutoSize = true;
            this.imarEskisehirRadioButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.imarEskisehirRadioButton.ForeColor = System.Drawing.Color.DarkOrange;
            this.imarEskisehirRadioButton.Location = new System.Drawing.Point(88, 36);
            this.imarEskisehirRadioButton.Name = "imarEskisehirRadioButton";
            this.imarEskisehirRadioButton.Size = new System.Drawing.Size(80, 21);
            this.imarEskisehirRadioButton.TabIndex = 1;
            this.imarEskisehirRadioButton.TabStop = true;
            this.imarEskisehirRadioButton.Text = "Eskişehir";
            this.imarEskisehirRadioButton.UseVisualStyleBackColor = true;
            // 
            // imarizmirRadioButton
            // 
            this.imarizmirRadioButton.AutoSize = true;
            this.imarizmirRadioButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.imarizmirRadioButton.ForeColor = System.Drawing.Color.DarkOrange;
            this.imarizmirRadioButton.Location = new System.Drawing.Point(88, 13);
            this.imarizmirRadioButton.Name = "imarizmirRadioButton";
            this.imarizmirRadioButton.Size = new System.Drawing.Size(57, 21);
            this.imarizmirRadioButton.TabIndex = 0;
            this.imarizmirRadioButton.TabStop = true;
            this.imarizmirRadioButton.Text = "İzmir";
            this.imarizmirRadioButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OkButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OkButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.OkButton.BorderRadius = 0;
            this.OkButton.BorderSize = 0;
            this.OkButton.FlatAppearance.BorderSize = 0;
            this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OkButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.OkButton.ForeColor = System.Drawing.Color.White;
            this.OkButton.Location = new System.Drawing.Point(113, 381);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(150, 40);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "TAMAM";
            this.OkButton.TextColor = System.Drawing.Color.White;
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // imarFileSelectionPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.imarCitySelectionPanel);
            this.Controls.Add(this.imarMethodSelectionComboBox);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.imarFileSelectionPanel);
            this.MaximumSize = new System.Drawing.Size(400, 500);
            this.MinimumSize = new System.Drawing.Size(400, 500);
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
        private CustomButton OkButton;
        private System.Windows.Forms.ComboBox imarMethodSelectionComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel imarCitySelectionPanel;
        private System.Windows.Forms.RadioButton imarEskisehirRadioButton;
        private System.Windows.Forms.RadioButton imarizmirRadioButton;
    }
}