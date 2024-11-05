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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DEKCenterDataGridView = new Guna.UI2.WinForms.Guna2DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DEKCancelButton = new Guna.UI2.WinForms.Guna2Button();
            this.DEKTamamButton = new Guna.UI2.WinForms.Guna2Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            this.ILCE_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KAYNAK_TIPI = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DEK_KURULU_GUCU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DEK_X_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_Y_KOORDINAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_TM_ADI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_KURULUM_YERI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEK_BAGLANDIGI_TRAFO_KODU = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DEKCenterDataGridView)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DEKCenterDataGridView
            // 
            this.DEKCenterDataGridView.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(223)))), ((int)(((byte)(251)))));
            this.DEKCenterDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DEKCenterDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(242)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DEKCenterDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DEKCenterDataGridView.ColumnHeadersHeight = 25;
            this.DEKCenterDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.DEKCenterDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ILCE_ADI,
            this.KAYNAK_TIPI,
            this.DEK_KURULU_GUCU,
            this.DEK_X_KOORDINAT,
            this.DEK_Y_KOORDINAT,
            this.DEK_TM_ADI,
            this.DEK_KURULUM_YERI,
            this.DEK_BAGLANDIGI_TRAFO_KODU});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(233)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(185)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DEKCenterDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.DEKCenterDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(251)))));
            this.DEKCenterDataGridView.Location = new System.Drawing.Point(12, 12);
            this.DEKCenterDataGridView.Name = "DEKCenterDataGridView";
            this.DEKCenterDataGridView.RowHeadersVisible = false;
            this.DEKCenterDataGridView.RowHeadersWidth = 18;
            this.DEKCenterDataGridView.Size = new System.Drawing.Size(1005, 368);
            this.DEKCenterDataGridView.TabIndex = 0;
            this.DEKCenterDataGridView.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Blue;
            this.DEKCenterDataGridView.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(223)))), ((int)(((byte)(251)))));
            this.DEKCenterDataGridView.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.DEKCenterDataGridView.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.DEKCenterDataGridView.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.DEKCenterDataGridView.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.DEKCenterDataGridView.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.DEKCenterDataGridView.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(251)))));
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(242)))));
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.DEKCenterDataGridView.ThemeStyle.HeaderStyle.Height = 25;
            this.DEKCenterDataGridView.ThemeStyle.ReadOnly = false;
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(233)))), ((int)(((byte)(252)))));
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.Height = 22;
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(185)))), ((int)(((byte)(246)))));
            this.DEKCenterDataGridView.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.SkyBlue;
            this.panel2.Controls.Add(this.DEKCancelButton);
            this.panel2.Controls.Add(this.DEKTamamButton);
            this.panel2.Location = new System.Drawing.Point(0, 393);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1029, 62);
            this.panel2.TabIndex = 4;
            // 
            // DEKCancelButton
            // 
            this.DEKCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKCancelButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.DEKCancelButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.DEKCancelButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.DEKCancelButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.DEKCancelButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(242)))));
            this.DEKCancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCancelButton.ForeColor = System.Drawing.Color.White;
            this.DEKCancelButton.Location = new System.Drawing.Point(660, 10);
            this.DEKCancelButton.Name = "DEKCancelButton";
            this.DEKCancelButton.Size = new System.Drawing.Size(180, 45);
            this.DEKCancelButton.TabIndex = 1;
            this.DEKCancelButton.Text = "İPTAL";
            // 
            // DEKTamamButton
            // 
            this.DEKTamamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKTamamButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.DEKTamamButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.DEKTamamButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.DEKTamamButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.DEKTamamButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(242)))));
            this.DEKTamamButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKTamamButton.ForeColor = System.Drawing.Color.White;
            this.DEKTamamButton.Location = new System.Drawing.Point(846, 10);
            this.DEKTamamButton.Name = "DEKTamamButton";
            this.DEKTamamButton.Size = new System.Drawing.Size(180, 45);
            this.DEKTamamButton.TabIndex = 2;
            this.DEKTamamButton.Text = "TAMAM";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SkyBlue;
            this.panel1.Controls.Add(this.DEKCenterDataGridView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1029, 455);
            this.panel1.TabIndex = 3;
            // 
            // ILCE_ADI
            // 
            this.ILCE_ADI.FillWeight = 57.73196F;
            this.ILCE_ADI.HeaderText = "ILCE_ADI";
            this.ILCE_ADI.Name = "ILCE_ADI";
            this.ILCE_ADI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ILCE_ADI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // KAYNAK_TIPI
            // 
            this.KAYNAK_TIPI.FillWeight = 84.90324F;
            this.KAYNAK_TIPI.HeaderText = "KAYNAK_TIPI";
            this.KAYNAK_TIPI.Name = "KAYNAK_TIPI";
            this.KAYNAK_TIPI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // DEK_KURULU_GUCU
            // 
            this.DEK_KURULU_GUCU.FillWeight = 120.7607F;
            this.DEK_KURULU_GUCU.HeaderText = "DEK_KURULU_GUCU";
            this.DEK_KURULU_GUCU.Items.AddRange(new object[] {
            "4",
            "5",
            "4"});
            this.DEK_KURULU_GUCU.Name = "DEK_KURULU_GUCU";
            // 
            // DEK_X_KOORDINAT
            // 
            this.DEK_X_KOORDINAT.FillWeight = 129.1611F;
            this.DEK_X_KOORDINAT.HeaderText = "DEK_X_KOORDINAT";
            this.DEK_X_KOORDINAT.Name = "DEK_X_KOORDINAT";
            this.DEK_X_KOORDINAT.ReadOnly = true;
            // 
            // DEK_Y_KOORDINAT
            // 
            this.DEK_Y_KOORDINAT.FillWeight = 120.1783F;
            this.DEK_Y_KOORDINAT.HeaderText = "DEK_Y_KOORDINAT";
            this.DEK_Y_KOORDINAT.Name = "DEK_Y_KOORDINAT";
            this.DEK_Y_KOORDINAT.ReadOnly = true;
            // 
            // DEK_TM_ADI
            // 
            this.DEK_TM_ADI.FillWeight = 66.81672F;
            this.DEK_TM_ADI.HeaderText = "DEK_TM_ADI";
            this.DEK_TM_ADI.Name = "DEK_TM_ADI";
            this.DEK_TM_ADI.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DEK_TM_ADI.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DEK_KURULUM_YERI
            // 
            this.DEK_KURULUM_YERI.FillWeight = 120.448F;
            this.DEK_KURULUM_YERI.HeaderText = "DEK_KURULUM_YERI";
            this.DEK_KURULUM_YERI.Name = "DEK_KURULUM_YERI";
            // 
            // DEK_BAGLANDIGI_TRAFO_KODU
            // 
            this.DEK_BAGLANDIGI_TRAFO_KODU.HeaderText = "DEK_BAGLANDIGI_TRAFO_KODU";
            this.DEK_BAGLANDIGI_TRAFO_KODU.Name = "DEK_BAGLANDIGI_TRAFO_KODU";
            // 
            // DEKCenterPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 455);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DEKCenterPopupForm";
            this.Text = "DEK Merkezi Bilgileri";
            ((System.ComponentModel.ISupportInitialize)(this.DEKCenterDataGridView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView DEKCenterDataGridView;
        private System.Windows.Forms.Panel panel2;
        private Guna.UI2.WinForms.Guna2Button DEKCancelButton;
        private Guna.UI2.WinForms.Guna2Button DEKTamamButton;
        private System.Windows.Forms.Panel panel1;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ILCE_ADI;
        private System.Windows.Forms.DataGridViewComboBoxColumn KAYNAK_TIPI;
        private System.Windows.Forms.DataGridViewComboBoxColumn DEK_KURULU_GUCU;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_X_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_Y_KOORDINAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_TM_ADI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEK_KURULUM_YERI;
        private System.Windows.Forms.DataGridViewComboBoxColumn DEK_BAGLANDIGI_TRAFO_KODU;
    }
}