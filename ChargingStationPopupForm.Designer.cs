namespace SLF
{
    partial class ChargingStationPopupForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChargingStationPopupForm));
            this.ChargingStationDataGridView = new Guna.UI2.WinForms.Guna2DataGridView();
            this.ISTASYON_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISTASYON_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ISTASYON_GUCU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EA_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EA_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChargingStationpanel1 = new System.Windows.Forms.Panel();
            this.ChargingStationpanel2 = new System.Windows.Forms.Panel();
            this.CancelButton = new Guna.UI2.WinForms.Guna2Button();
            this.TamamButton = new Guna.UI2.WinForms.Guna2Button();
            this.guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).BeginInit();
            this.ChargingStationpanel1.SuspendLayout();
            this.ChargingStationpanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChargingStationDataGridView
            // 
            this.ChargingStationDataGridView.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(218)))));
            this.ChargingStationDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.ChargingStationDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(197)))), ((int)(((byte)(84)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChargingStationDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.ChargingStationDataGridView.ColumnHeadersHeight = 25;
            this.ChargingStationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.ChargingStationDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ISTASYON_ADI,
            this.ISTASYON_TIPI,
            this.ISTASYON_GUCU,
            this.EA_X_KOORDINAT,
            this.EA_Y_KOORDINAT});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(191)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChargingStationDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
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
            this.ChargingStationDataGridView.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(197)))), ((int)(((byte)(84)))));
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
            this.ISTASYON_TIPI.Name = "ISTASYON_TIPI";
            // 
            // ISTASYON_GUCU
            // 
            this.ISTASYON_GUCU.HeaderText = "ISTASYON_GUCU";
            this.ISTASYON_GUCU.Items.AddRange(new object[] {
            "4",
            "5",
            "4"});
            this.ISTASYON_GUCU.Name = "ISTASYON_GUCU";
            // 
            // EA_X_KOORDINAT
            // 
            this.EA_X_KOORDINAT.HeaderText = "EA_X_KOORDINAT";
            this.EA_X_KOORDINAT.Name = "EA_X_KOORDINAT";
            this.EA_X_KOORDINAT.ReadOnly = true;
            // 
            // EA_Y_KOORDINAT
            // 
            this.EA_Y_KOORDINAT.HeaderText = "EA_Y_KOORDINAT";
            this.EA_Y_KOORDINAT.Name = "EA_Y_KOORDINAT";
            this.EA_Y_KOORDINAT.ReadOnly = true;
            // 
            // ChargingStationpanel1
            // 
            this.ChargingStationpanel1.BackColor = System.Drawing.Color.LightGreen;
            this.ChargingStationpanel1.Controls.Add(this.ChargingStationDataGridView);
            this.ChargingStationpanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChargingStationpanel1.Location = new System.Drawing.Point(0, 0);
            this.ChargingStationpanel1.Name = "ChargingStationpanel1";
            this.ChargingStationpanel1.Size = new System.Drawing.Size(800, 450);
            this.ChargingStationpanel1.TabIndex = 1;
            // 
            // ChargingStationpanel2
            // 
            this.ChargingStationpanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChargingStationpanel2.BackColor = System.Drawing.Color.LightGreen;
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
            this.CancelButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(197)))), ((int)(((byte)(84)))));
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.CancelButton.ForeColor = System.Drawing.Color.White;
            this.CancelButton.Location = new System.Drawing.Point(431, 5);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(180, 45);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "İPTAL";
            // 
            // TamamButton
            // 
            this.TamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TamamButton.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.TamamButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.TamamButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.TamamButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.TamamButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.TamamButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(197)))), ((int)(((byte)(84)))));
            this.TamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.TamamButton.ForeColor = System.Drawing.Color.White;
            this.TamamButton.Location = new System.Drawing.Point(617, 5);
            this.TamamButton.Name = "TamamButton";
            this.TamamButton.Size = new System.Drawing.Size(180, 45);
            this.TamamButton.TabIndex = 2;
            this.TamamButton.Text = "TAMAM";
            // 
            // ChargingStationPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ChargingStationpanel2);
            this.Controls.Add(this.ChargingStationpanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChargingStationPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Şarj İstasyonu Bilgileri";
            ((System.ComponentModel.ISupportInitialize)(this.ChargingStationDataGridView)).EndInit();
            this.ChargingStationpanel1.ResumeLayout(false);
            this.ChargingStationpanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView ChargingStationDataGridView;
        private System.Windows.Forms.Panel ChargingStationpanel1;
        private System.Windows.Forms.Panel ChargingStationpanel2;
        private Guna.UI2.WinForms.Guna2Button CancelButton;
        private Guna.UI2.WinForms.Guna2Button TamamButton;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ISTASYON_ADI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_TIPI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_GUCU;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_Y_KOORDINAT;
    }
}