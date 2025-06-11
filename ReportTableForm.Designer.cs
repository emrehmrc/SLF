namespace SLF
{
    partial class ReportTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportTableForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ReportTablePanel = new System.Windows.Forms.Panel();
            this.comboBox_report_yıl_secimi = new System.Windows.Forms.ComboBox();
            this.ReportsTableDataGridView = new System.Windows.Forms.DataGridView();
            this.ReportExportButton = new System.Windows.Forms.Button();
            this.ReportCancelButton = new System.Windows.Forms.Button();
            this.ReportTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReportsTableDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ReportTablePanel
            // 
            this.ReportTablePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(227)))), ((int)(((byte)(249)))));
            this.ReportTablePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ReportTablePanel.BackgroundImage")));
            this.ReportTablePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ReportTablePanel.Controls.Add(this.ReportExportButton);
            this.ReportTablePanel.Controls.Add(this.comboBox_report_yıl_secimi);
            this.ReportTablePanel.Controls.Add(this.ReportCancelButton);
            this.ReportTablePanel.Controls.Add(this.ReportsTableDataGridView);
            this.ReportTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportTablePanel.Location = new System.Drawing.Point(0, 0);
            this.ReportTablePanel.Name = "ReportTablePanel";
            this.ReportTablePanel.Size = new System.Drawing.Size(944, 550);
            this.ReportTablePanel.TabIndex = 0;
            // 
            // comboBox_report_yıl_secimi
            // 
            this.comboBox_report_yıl_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_report_yıl_secimi.FormattingEnabled = true;
            this.comboBox_report_yıl_secimi.Location = new System.Drawing.Point(12, 11);
            this.comboBox_report_yıl_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_report_yıl_secimi.Name = "comboBox_report_yıl_secimi";
            this.comboBox_report_yıl_secimi.Size = new System.Drawing.Size(85, 24);
            this.comboBox_report_yıl_secimi.TabIndex = 3;
            this.comboBox_report_yıl_secimi.Text = "YIL";
            // 
            // ReportsTableDataGridView
            // 
            this.ReportsTableDataGridView.AllowUserToAddRows = false;
            this.ReportsTableDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportsTableDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.ReportsTableDataGridView.BackgroundColor = System.Drawing.Color.Snow;
            this.ReportsTableDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ReportsTableDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ReportsTableDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(196)))), ((int)(((byte)(233)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DarkBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ReportsTableDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ReportsTableDataGridView.ColumnHeadersHeight = 25;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ReportsTableDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.ReportsTableDataGridView.EnableHeadersVisualStyles = false;
            this.ReportsTableDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(213)))));
            this.ReportsTableDataGridView.Location = new System.Drawing.Point(160, 38);
            this.ReportsTableDataGridView.Name = "ReportsTableDataGridView";
            this.ReportsTableDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ReportsTableDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ReportsTableDataGridView.RowHeadersWidth = 18;
            this.ReportsTableDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ReportsTableDataGridView.Size = new System.Drawing.Size(733, 396);
            this.ReportsTableDataGridView.TabIndex = 2;
            // 
            // ReportExportButton
            // 
            this.ReportExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportExportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.ReportExportButton.FlatAppearance.BorderSize = 0;
            this.ReportExportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReportExportButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ReportExportButton.ForeColor = System.Drawing.Color.White;
            this.ReportExportButton.Location = new System.Drawing.Point(678, 482);
            this.ReportExportButton.Margin = new System.Windows.Forms.Padding(4);
            this.ReportExportButton.Name = "ReportExportButton";
            this.ReportExportButton.Size = new System.Drawing.Size(240, 55);
            this.ReportExportButton.TabIndex = 6;
            this.ReportExportButton.Text = "DIŞA AKTAR";
            this.ReportExportButton.UseVisualStyleBackColor = false;
            this.ReportExportButton.Click += new System.EventHandler(this.ReportExportButton_Click);
            // 
            // ReportCancelButton
            // 
            this.ReportCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportCancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.ReportCancelButton.FlatAppearance.BorderSize = 0;
            this.ReportCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReportCancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ReportCancelButton.ForeColor = System.Drawing.Color.White;
            this.ReportCancelButton.Location = new System.Drawing.Point(430, 482);
            this.ReportCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.ReportCancelButton.Name = "ReportCancelButton";
            this.ReportCancelButton.Size = new System.Drawing.Size(240, 55);
            this.ReportCancelButton.TabIndex = 5;
            this.ReportCancelButton.Text = "İPTAL";
            this.ReportCancelButton.UseVisualStyleBackColor = false;
            this.ReportCancelButton.Click += new System.EventHandler(this.ReportCancelButton_Click);
            // 
            // ReportTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 550);
            this.Controls.Add(this.ReportTablePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Raporlama Penceresi";
            this.ReportTablePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReportsTableDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ReportTablePanel;
        private System.Windows.Forms.Button ReportExportButton;
        private System.Windows.Forms.Button ReportCancelButton;
        private System.Windows.Forms.DataGridView ReportsTableDataGridView;
        private System.Windows.Forms.ComboBox comboBox_report_yıl_secimi;
    }
}