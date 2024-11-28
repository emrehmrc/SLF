namespace SLF
{
    partial class EAChargingStationPopupForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EAChargingStationPopupForm));
            this.ChargingStationDataGridView = new Guna.UI2.WinForms.Guna2DataGridView();
            this.ISTASYON_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISTASYON_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ISTASYON_GUCU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EA_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EA_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChargingStationpanel = new System.Windows.Forms.Panel();
            this.ChargingStationpanel2 = new System.Windows.Forms.Panel();
            this.CancelButton = new Guna.UI2.WinForms.Guna2Button();
            this.TamamButton = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).BeginInit();
            this.ChargingStationpanel.SuspendLayout();
            this.ChargingStationpanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChargingStationDataGridView
            // 
            this.ChargingStationDataGridView.AllowUserToAddRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(218)))));
            this.ChargingStationDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.ChargingStationDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChargingStationDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.ChargingStationDataGridView.ColumnHeadersHeight = 25;
            this.ChargingStationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.ChargingStationDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ISTASYON_ADI,
            this.ISTASYON_TIPI,
            this.ISTASYON_GUCU,
            this.EA_X_KOORDINAT,
            this.EA_Y_KOORDINAT});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(191)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChargingStationDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.ChargingStationDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(224)))), ((int)(((byte)(216)))));
            this.ChargingStationDataGridView.Location = new System.Drawing.Point(36, 24);
            this.ChargingStationDataGridView.Name = "ChargingStationDataGridView";
            this.ChargingStationDataGridView.RowHeadersVisible = false;
            this.ChargingStationDataGridView.RowHeadersWidth = 18;
            this.ChargingStationDataGridView.Size = new System.Drawing.Size(727, 363);
            this.ChargingStationDataGridView.TabIndex = 0;
            this.ChargingStationDataGridView.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.GreenSea;
            this.ChargingStationDataGridView.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(218)))));
            this.ChargingStationDataGridView.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.ChargingStationDataGridView.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.ChargingStationDataGridView.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.ChargingStationDataGridView.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.ChargingStationDataGridView.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.ChargingStationDataGridView.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(224)))), ((int)(((byte)(216)))));
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.Height = 25;
            this.ChargingStationDataGridView.ThemeStyle.ReadOnly = false;
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.Height = 22;
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(191)))), ((int)(((byte)(173)))));
            this.ChargingStationDataGridView.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            // 
            // ISTASYON_ADI
            // 
            this.ISTASYON_ADI.HeaderText = "ISTASYON_ADI";
            this.ISTASYON_ADI.MinimumWidth = 6;
            this.ISTASYON_ADI.Name = "ISTASYON_ADI";
            this.ISTASYON_ADI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ISTASYON_ADI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ISTASYON_TIPI
            // 
            this.ISTASYON_TIPI.HeaderText = "ISTASYON_TIPI";
            this.ISTASYON_TIPI.Items.AddRange(new object[] {
            "AC",
            "DC"});
            this.ISTASYON_TIPI.MinimumWidth = 6;
            this.ISTASYON_TIPI.Name = "ISTASYON_TIPI";
            // 
            // ISTASYON_GUCU
            // 
            this.ISTASYON_GUCU.HeaderText = "ISTASYON_GUCU";
            this.ISTASYON_GUCU.Items.AddRange(new object[] {
            "4",
            "5",
            "4"});
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
            // ChargingStationpanel
            // 
            this.ChargingStationpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(223)))), ((int)(((byte)(156)))));
            this.ChargingStationpanel.Controls.Add(this.ChargingStationDataGridView);
            this.ChargingStationpanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChargingStationpanel.Location = new System.Drawing.Point(0, 0);
            this.ChargingStationpanel.Name = "ChargingStationpanel";
            this.ChargingStationpanel.Size = new System.Drawing.Size(800, 450);
            this.ChargingStationpanel.TabIndex = 1;
            // 
            // ChargingStationpanel2
            // 
            this.ChargingStationpanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChargingStationpanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(223)))), ((int)(((byte)(156)))));
            this.ChargingStationpanel2.Controls.Add(this.CancelButton);
            this.ChargingStationpanel2.Controls.Add(this.TamamButton);
            this.ChargingStationpanel2.Location = new System.Drawing.Point(0, 393);
            this.ChargingStationpanel2.Name = "ChargingStationpanel2";
            this.ChargingStationpanel2.Size = new System.Drawing.Size(800, 57);
            this.ChargingStationpanel2.TabIndex = 2;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.CancelButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.CancelButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.CancelButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.CancelButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.CancelButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.CancelButton.ForeColor = System.Drawing.Color.White;
            this.CancelButton.Location = new System.Drawing.Point(431, 5);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(180, 45);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "İPTAL";
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // TamamButton
            // 
            this.TamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TamamButton.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.TamamButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.TamamButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.TamamButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.TamamButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.TamamButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.TamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.TamamButton.ForeColor = System.Drawing.Color.White;
            this.TamamButton.Location = new System.Drawing.Point(617, 5);
            this.TamamButton.Name = "TamamButton";
            this.TamamButton.Size = new System.Drawing.Size(180, 45);
            this.TamamButton.TabIndex = 2;
            this.TamamButton.Text = "TAMAM";
            this.TamamButton.Click += new System.EventHandler(this.TamamButton_Click);
            // 
            // EAChargingStationPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ChargingStationpanel2);
            this.Controls.Add(this.ChargingStationpanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EAChargingStationPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Şarj İstasyonu Bilgileri";
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).EndInit();
            this.ChargingStationpanel.ResumeLayout(false);
            this.ChargingStationpanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView ChargingStationDataGridView;
        private System.Windows.Forms.Panel ChargingStationpanel;
        private System.Windows.Forms.Panel ChargingStationpanel2;
        private Guna.UI2.WinForms.Guna2Button CancelButton;
        private Guna.UI2.WinForms.Guna2Button TamamButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ISTASYON_ADI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_TIPI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_GUCU;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_Y_KOORDINAT;
    }
}