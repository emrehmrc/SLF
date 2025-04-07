namespace SLF
{
    partial class DEKCenterPopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DEKCenterPopupForm));
            this.DEKCenterpanel2_Dek_Popup = new System.Windows.Forms.Panel();
            this.DEKTamamButton = new System.Windows.Forms.Button();
            this.DEKCancelButton = new System.Windows.Forms.Button();
            this.DEKCenterpanel1_Dek_Popup = new System.Windows.Forms.Panel();
            this.DEKCenterDataGridView = new System.Windows.Forms.DataGridView();
            this.KAYNAK_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DEK_KURULU_GUCU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ILCE_ADI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DEK_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_TM_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_KURULUM_YERI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEKCenterpanel2_Dek_Popup.SuspendLayout();
            this.DEKCenterpanel1_Dek_Popup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DEKCenterDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DEKCenterpanel2_Dek_Popup
            // 
            this.DEKCenterpanel2_Dek_Popup.BackColor = System.Drawing.Color.NavajoWhite;
            this.DEKCenterpanel2_Dek_Popup.Controls.Add(this.DEKTamamButton);
            this.DEKCenterpanel2_Dek_Popup.Controls.Add(this.DEKCancelButton);
            this.DEKCenterpanel2_Dek_Popup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DEKCenterpanel2_Dek_Popup.Location = new System.Drawing.Point(0, 393);
            this.DEKCenterpanel2_Dek_Popup.Name = "DEKCenterpanel2_Dek_Popup";
            this.DEKCenterpanel2_Dek_Popup.Size = new System.Drawing.Size(1029, 62);
            this.DEKCenterpanel2_Dek_Popup.TabIndex = 4;
            // 
            // DEKTamamButton
            // 
            this.DEKTamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKTamamButton.BackColor = System.Drawing.Color.DarkOrange;
            this.DEKTamamButton.FlatAppearance.BorderSize = 0;
            this.DEKTamamButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKTamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKTamamButton.ForeColor = System.Drawing.Color.White;
            this.DEKTamamButton.Location = new System.Drawing.Point(846, 10);
            this.DEKTamamButton.Name = "DEKTamamButton";
            this.DEKTamamButton.Size = new System.Drawing.Size(180, 45);
            this.DEKTamamButton.TabIndex = 4;
            this.DEKTamamButton.Text = "TAMAM";
            this.DEKTamamButton.UseVisualStyleBackColor = false;
            this.DEKTamamButton.Click += new System.EventHandler(this.DEKTamamButton_Click);
            // 
            // DEKCancelButton
            // 
            this.DEKCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKCancelButton.BackColor = System.Drawing.Color.DarkOrange;
            this.DEKCancelButton.FlatAppearance.BorderSize = 0;
            this.DEKCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKCancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCancelButton.ForeColor = System.Drawing.Color.White;
            this.DEKCancelButton.Location = new System.Drawing.Point(660, 10);
            this.DEKCancelButton.Name = "DEKCancelButton";
            this.DEKCancelButton.Size = new System.Drawing.Size(180, 45);
            this.DEKCancelButton.TabIndex = 3;
            this.DEKCancelButton.Text = "İPTAL";
            this.DEKCancelButton.UseVisualStyleBackColor = false;
            this.DEKCancelButton.Click += new System.EventHandler(this.DEKCancelButton_Click);
            // 
            // DEKCenterpanel1_Dek_Popup
            // 
            this.DEKCenterpanel1_Dek_Popup.BackColor = System.Drawing.Color.NavajoWhite;
            this.DEKCenterpanel1_Dek_Popup.Controls.Add(this.DEKCenterDataGridView);
            this.DEKCenterpanel1_Dek_Popup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DEKCenterpanel1_Dek_Popup.Location = new System.Drawing.Point(0, 0);
            this.DEKCenterpanel1_Dek_Popup.Name = "DEKCenterpanel1_Dek_Popup";
            this.DEKCenterpanel1_Dek_Popup.Size = new System.Drawing.Size(1029, 455);
            this.DEKCenterpanel1_Dek_Popup.TabIndex = 3;
            // 
            // DEKCenterDataGridView
            // 
            this.DEKCenterDataGridView.AllowUserToAddRows = false;
            this.DEKCenterDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKCenterDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DEKCenterDataGridView.BackgroundColor = System.Drawing.Color.Snow;
            this.DEKCenterDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DEKCenterDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DEKCenterDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DEKCenterDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DEKCenterDataGridView.ColumnHeadersHeight = 25;
            this.DEKCenterDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KAYNAK_TIPI,
            this.DEK_KURULU_GUCU,
            this.ILCE_ADI,
            this.DEK_X_KOORDINAT,
            this.DEK_Y_KOORDINAT,
            this.DEK_TM_ADI,
            this.DEK_KURULUM_YERI});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DEKCenterDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.DEKCenterDataGridView.EnableHeadersVisualStyles = false;
            this.DEKCenterDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.DEKCenterDataGridView.Location = new System.Drawing.Point(28, 31);
            this.DEKCenterDataGridView.Name = "DEKCenterDataGridView";
            this.DEKCenterDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DEKCenterDataGridView.RowHeadersVisible = false;
            this.DEKCenterDataGridView.RowHeadersWidth = 18;
            this.DEKCenterDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DEKCenterDataGridView.Size = new System.Drawing.Size(972, 356);
            this.DEKCenterDataGridView.TabIndex = 7;
            // 
            // KAYNAK_TIPI
            // 
            this.KAYNAK_TIPI.HeaderText = "KAYNAK_TIPI";
            this.KAYNAK_TIPI.Name = "KAYNAK_TIPI";
            this.KAYNAK_TIPI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.KAYNAK_TIPI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // DEK_KURULU_GUCU
            // 
            this.DEK_KURULU_GUCU.HeaderText = "DEK_KURULU_GUCU";
            this.DEK_KURULU_GUCU.Name = "DEK_KURULU_GUCU";
            this.DEK_KURULU_GUCU.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DEK_KURULU_GUCU.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ILCE_ADI
            // 
            this.ILCE_ADI.HeaderText = "ILCE_ADI";
            this.ILCE_ADI.Name = "ILCE_ADI";
            // 
            // DEK_X_KOORDINAT
            // 
            this.DEK_X_KOORDINAT.HeaderText = "DEK_X_KOORDINAT";
            this.DEK_X_KOORDINAT.Name = "DEK_X_KOORDINAT";
            this.DEK_X_KOORDINAT.ReadOnly = true;
            // 
            // DEK_Y_KOORDINAT
            // 
            this.DEK_Y_KOORDINAT.HeaderText = "DEK_Y_KOORDINAT";
            this.DEK_Y_KOORDINAT.Name = "DEK_Y_KOORDINAT";
            this.DEK_Y_KOORDINAT.ReadOnly = true;
            // 
            // DEK_TM_ADI
            // 
            this.DEK_TM_ADI.HeaderText = "DEK_TM_ADI";
            this.DEK_TM_ADI.Name = "DEK_TM_ADI";
            // 
            // DEK_KURULUM_YERI
            // 
            this.DEK_KURULUM_YERI.HeaderText = "DEK_KURULUM_YERI";
            this.DEK_KURULUM_YERI.Name = "DEK_KURULUM_YERI";
            // 
            // DEKCenterPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 455);
            this.Controls.Add(this.DEKCenterpanel2_Dek_Popup);
            this.Controls.Add(this.DEKCenterpanel1_Dek_Popup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DEKCenterPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DEK Merkezi Bilgileri";
            this.DEKCenterpanel2_Dek_Popup.ResumeLayout(false);
            this.DEKCenterpanel1_Dek_Popup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DEKCenterDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel DEKCenterpanel2_Dek_Popup;
        private System.Windows.Forms.Panel DEKCenterpanel1_Dek_Popup;
        private System.Windows.Forms.Button DEKCancelButton;
        private System.Windows.Forms.Button DEKTamamButton;
        private System.Windows.Forms.DataGridView DEKCenterDataGridView;
        private System.Windows.Forms.DataGridViewComboBoxColumn KAYNAK_TIPI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_KURULU_GUCU;
        private System.Windows.Forms.DataGridViewComboBoxColumn ILCE_ADI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_Y_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_TM_ADI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_KURULUM_YERI;
    }
}