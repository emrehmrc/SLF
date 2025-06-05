namespace SLF
{
    partial class EAStationPopupForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EAStationPopupForm));
            this.ChargingStationpanel = new System.Windows.Forms.Panel();
            this.ChargingStationDataGridView = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISTASYON_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISTASYON_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ISTASYON_GUCU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EA_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EA_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartYear = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ChargingStationpanel2 = new System.Windows.Forms.Panel();
            this.EATamamButton = new System.Windows.Forms.Button();
            this.EACancelButton = new System.Windows.Forms.Button();
            this.ChargingStationpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).BeginInit();
            this.ChargingStationpanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChargingStationpanel
            // 
            this.ChargingStationpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(223)))), ((int)(((byte)(156)))));
            this.ChargingStationpanel.Controls.Add(this.ChargingStationDataGridView);
            this.ChargingStationpanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChargingStationpanel.Location = new System.Drawing.Point(0, 0);
            this.ChargingStationpanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChargingStationpanel.Name = "ChargingStationpanel";
            this.ChargingStationpanel.Size = new System.Drawing.Size(1067, 554);
            this.ChargingStationpanel.TabIndex = 1;
            // 
            // ChargingStationDataGridView
            // 
            this.ChargingStationDataGridView.AllowUserToAddRows = false;
            this.ChargingStationDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChargingStationDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ChargingStationDataGridView.BackgroundColor = System.Drawing.Color.Snow;
            this.ChargingStationDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChargingStationDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ChargingStationDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChargingStationDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ChargingStationDataGridView.ColumnHeadersHeight = 25;
            this.ChargingStationDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.ISTASYON_ADI,
            this.ISTASYON_TIPI,
            this.ISTASYON_GUCU,
            this.EA_X_KOORDINAT,
            this.EA_Y_KOORDINAT,
            this.StartYear});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChargingStationDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.ChargingStationDataGridView.EnableHeadersVisualStyles = false;
            this.ChargingStationDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(224)))), ((int)(((byte)(216)))));
            this.ChargingStationDataGridView.Location = new System.Drawing.Point(16, 15);
            this.ChargingStationDataGridView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChargingStationDataGridView.Name = "ChargingStationDataGridView";
            this.ChargingStationDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ChargingStationDataGridView.RowHeadersVisible = false;
            this.ChargingStationDataGridView.RowHeadersWidth = 18;
            this.ChargingStationDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChargingStationDataGridView.Size = new System.Drawing.Size(1035, 462);
            this.ChargingStationDataGridView.TabIndex = 3;
            // 
            // ID
            // 
            this.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ID.HeaderText = "ID";
            this.ID.MinimumWidth = 6;
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ID.Width = 54;
            // 
            // ISTASYON_ADI
            // 
            this.ISTASYON_ADI.HeaderText = "ISTASYON_ADI";
            this.ISTASYON_ADI.MinimumWidth = 6;
            this.ISTASYON_ADI.Name = "ISTASYON_ADI";
            // 
            // ISTASYON_TIPI
            // 
            this.ISTASYON_TIPI.HeaderText = "ISTASYON_TIPI";
            this.ISTASYON_TIPI.MinimumWidth = 6;
            this.ISTASYON_TIPI.Name = "ISTASYON_TIPI";
            // 
            // ISTASYON_GUCU
            // 
            this.ISTASYON_GUCU.HeaderText = "ISTASYON_GUCU";
            this.ISTASYON_GUCU.MinimumWidth = 6;
            this.ISTASYON_GUCU.Name = "ISTASYON_GUCU";
            // 
            // EA_X_KOORDINAT
            // 
            this.EA_X_KOORDINAT.HeaderText = "EA_X_KOORDINAT";
            this.EA_X_KOORDINAT.MinimumWidth = 6;
            this.EA_X_KOORDINAT.Name = "EA_X_KOORDINAT";
            this.EA_X_KOORDINAT.ReadOnly = true;
            // 
            // EA_Y_KOORDINAT
            // 
            this.EA_Y_KOORDINAT.HeaderText = "EA_Y_KOORDINAT";
            this.EA_Y_KOORDINAT.MinimumWidth = 6;
            this.EA_Y_KOORDINAT.Name = "EA_Y_KOORDINAT";
            this.EA_Y_KOORDINAT.ReadOnly = true;
            // 
            // StartYear
            // 
            this.StartYear.HeaderText = "BASLANGIC_YILI";
            this.StartYear.Items.AddRange(new object[] {
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
            this.StartYear.MinimumWidth = 6;
            this.StartYear.Name = "StartYear";
            // 
            // ChargingStationpanel2
            // 
            this.ChargingStationpanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(223)))), ((int)(((byte)(156)))));
            this.ChargingStationpanel2.Controls.Add(this.EATamamButton);
            this.ChargingStationpanel2.Controls.Add(this.EACancelButton);
            this.ChargingStationpanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ChargingStationpanel2.Location = new System.Drawing.Point(0, 484);
            this.ChargingStationpanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChargingStationpanel2.Name = "ChargingStationpanel2";
            this.ChargingStationpanel2.Size = new System.Drawing.Size(1067, 70);
            this.ChargingStationpanel2.TabIndex = 2;
            this.ChargingStationpanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.ChargingStationpanel2_Paint);
            // 
            // EATamamButton
            // 
            this.EATamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EATamamButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.EATamamButton.FlatAppearance.BorderSize = 0;
            this.EATamamButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EATamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EATamamButton.ForeColor = System.Drawing.Color.White;
            this.EATamamButton.Location = new System.Drawing.Point(823, 6);
            this.EATamamButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.EATamamButton.Name = "EATamamButton";
            this.EATamamButton.Size = new System.Drawing.Size(240, 55);
            this.EATamamButton.TabIndex = 4;
            this.EATamamButton.Text = "TAMAM";
            this.EATamamButton.UseVisualStyleBackColor = false;
            this.EATamamButton.Click += new System.EventHandler(this.EATamamButton_Click);
            // 
            // EACancelButton
            // 
            this.EACancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EACancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.EACancelButton.FlatAppearance.BorderSize = 0;
            this.EACancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EACancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EACancelButton.ForeColor = System.Drawing.Color.White;
            this.EACancelButton.Location = new System.Drawing.Point(575, 6);
            this.EACancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.EACancelButton.Name = "EACancelButton";
            this.EACancelButton.Size = new System.Drawing.Size(240, 55);
            this.EACancelButton.TabIndex = 3;
            this.EACancelButton.Text = "İPTAL";
            this.EACancelButton.UseVisualStyleBackColor = false;
            this.EACancelButton.Click += new System.EventHandler(this.EACancelButton_Click);
            // 
            // EAStationPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.ChargingStationpanel2);
            this.Controls.Add(this.ChargingStationpanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EAStationPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Şarj İstasyonu Bilgileri";
            this.ChargingStationpanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).EndInit();
            this.ChargingStationpanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel ChargingStationpanel;
        private System.Windows.Forms.Panel ChargingStationpanel2;
        private System.Windows.Forms.Button EACancelButton;
        private System.Windows.Forms.Button EATamamButton;
        private System.Windows.Forms.DataGridView ChargingStationDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ISTASYON_ADI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_TIPI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_GUCU;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_Y_KOORDINAT;
        private System.Windows.Forms.DataGridViewComboBoxColumn StartYear;
    }
}