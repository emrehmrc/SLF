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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EAStationPopupForm));
            this.ChargingStationpanel = new System.Windows.Forms.Panel();
            this.ChargingStationDataGridView = new System.Windows.Forms.DataGridView();
            this.ISTASYON_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ISTASYON_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ISTASYON_GUCU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EA_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EA_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChargingStationpanel2 = new System.Windows.Forms.Panel();
            this.guna2AnimateWindow1_Charging_Popup = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
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
            this.ChargingStationpanel.Name = "ChargingStationpanel";
            this.ChargingStationpanel.Size = new System.Drawing.Size(800, 450);
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
            this.ISTASYON_ADI,
            this.ISTASYON_TIPI,
            this.ISTASYON_GUCU,
            this.EA_X_KOORDINAT,
            this.EA_Y_KOORDINAT});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChargingStationDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.ChargingStationDataGridView.EnableHeadersVisualStyles = false;
            this.ChargingStationDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(224)))), ((int)(((byte)(216)))));
            this.ChargingStationDataGridView.Location = new System.Drawing.Point(37, 12);
            this.ChargingStationDataGridView.Name = "ChargingStationDataGridView";
            this.ChargingStationDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ChargingStationDataGridView.RowHeadersVisible = false;
            this.ChargingStationDataGridView.RowHeadersWidth = 18;
            this.ChargingStationDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChargingStationDataGridView.Size = new System.Drawing.Size(727, 363);
            this.ChargingStationDataGridView.TabIndex = 3;
            // 
            // ISTASYON_ADI
            // 
            this.ISTASYON_ADI.HeaderText = "ISTASYON_ADI";
            this.ISTASYON_ADI.Name = "ISTASYON_ADI";
            // 
            // ISTASYON_TIPI
            // 
            this.ISTASYON_TIPI.HeaderText = "ISTASYON_TIPI";
            this.ISTASYON_TIPI.Name = "ISTASYON_TIPI";
            // 
            // ISTASYON_GUCU
            // 
            this.ISTASYON_GUCU.HeaderText = "ISTASYON_GUCU";
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
            // ChargingStationpanel2
            // 
            this.ChargingStationpanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(223)))), ((int)(((byte)(156)))));
            this.ChargingStationpanel2.Controls.Add(this.EATamamButton);
            this.ChargingStationpanel2.Controls.Add(this.EACancelButton);
            this.ChargingStationpanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ChargingStationpanel2.Location = new System.Drawing.Point(0, 393);
            this.ChargingStationpanel2.Name = "ChargingStationpanel2";
            this.ChargingStationpanel2.Size = new System.Drawing.Size(800, 57);
            this.ChargingStationpanel2.TabIndex = 2;
            // 
            // EATamamButton
            // 
            this.EATamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EATamamButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.EATamamButton.FlatAppearance.BorderSize = 0;
            this.EATamamButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EATamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EATamamButton.ForeColor = System.Drawing.Color.White;
            this.EATamamButton.Location = new System.Drawing.Point(617, 5);
            this.EATamamButton.Name = "EATamamButton";
            this.EATamamButton.Size = new System.Drawing.Size(180, 45);
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
            this.EACancelButton.Location = new System.Drawing.Point(431, 5);
            this.EACancelButton.Name = "EACancelButton";
            this.EACancelButton.Size = new System.Drawing.Size(180, 45);
            this.EACancelButton.TabIndex = 3;
            this.EACancelButton.Text = "İPTAL";
            this.EACancelButton.UseVisualStyleBackColor = false;
            this.EACancelButton.Click += new System.EventHandler(this.EACancelButton_Click);
            // 
            // EAStationPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ChargingStationpanel2);
            this.Controls.Add(this.ChargingStationpanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1_Charging_Popup;
        private System.Windows.Forms.Button EACancelButton;
        private System.Windows.Forms.Button EATamamButton;
        private System.Windows.Forms.DataGridView ChargingStationDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ISTASYON_ADI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_TIPI;
        private System.Windows.Forms.DataGridViewComboBoxColumn ISTASYON_GUCU;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn EA_Y_KOORDINAT;
    }
}