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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportTableForm));
            this.ExportPngButton = new System.Windows.Forms.Button();
            this.ExportCsvButton = new System.Windows.Forms.Button();
            this.ExportKmlButton = new System.Windows.Forms.Button();
            this.ReportTableBottomPanel = new System.Windows.Forms.Panel();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ReportCancelButton = new System.Windows.Forms.Button();
            this.ReportTablePanel = new System.Windows.Forms.Panel();
            this.ReportMapSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.ReportSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.ReportsTableDataGridView = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ReportTableBottomPanel.SuspendLayout();
            this.ReportTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReportsTableDataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExportPngButton
            // 
            this.ExportPngButton.BackgroundImage = global::SLF.Properties.Resources.PNG2;
            this.ExportPngButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExportPngButton.Location = new System.Drawing.Point(79, 52);
            this.ExportPngButton.Name = "ExportPngButton";
            this.ExportPngButton.Size = new System.Drawing.Size(40, 38);
            this.ExportPngButton.TabIndex = 4;
            this.ExportPngButton.UseVisualStyleBackColor = true;
            // 
            // ExportCsvButton
            // 
            this.ExportCsvButton.BackgroundImage = global::SLF.Properties.Resources.CSV21;
            this.ExportCsvButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ExportCsvButton.Location = new System.Drawing.Point(125, 52);
            this.ExportCsvButton.Name = "ExportCsvButton";
            this.ExportCsvButton.Size = new System.Drawing.Size(33, 38);
            this.ExportCsvButton.TabIndex = 2;
            this.ExportCsvButton.UseVisualStyleBackColor = true;
            // 
            // ExportKmlButton
            // 
            this.ExportKmlButton.BackgroundImage = global::SLF.Properties.Resources.KML21;
            this.ExportKmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ExportKmlButton.Location = new System.Drawing.Point(40, 52);
            this.ExportKmlButton.Name = "ExportKmlButton";
            this.ExportKmlButton.Size = new System.Drawing.Size(33, 38);
            this.ExportKmlButton.TabIndex = 3;
            this.ExportKmlButton.UseVisualStyleBackColor = true;
            // 
            // ReportTableBottomPanel
            // 
            this.ReportTableBottomPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(227)))), ((int)(((byte)(249)))));
            this.ReportTableBottomPanel.Controls.Add(this.SaveButton);
            this.ReportTableBottomPanel.Controls.Add(this.ReportCancelButton);
            this.ReportTableBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ReportTableBottomPanel.Location = new System.Drawing.Point(0, 476);
            this.ReportTableBottomPanel.Name = "ReportTableBottomPanel";
            this.ReportTableBottomPanel.Size = new System.Drawing.Size(786, 51);
            this.ReportTableBottomPanel.TabIndex = 7;
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SaveButton.ForeColor = System.Drawing.Color.Snow;
            this.SaveButton.Location = new System.Drawing.Point(595, 4);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(180, 45);
            this.SaveButton.TabIndex = 41;
            this.SaveButton.Text = "DIŞA AKTAR";
            this.SaveButton.UseVisualStyleBackColor = false;
            // 
            // ReportCancelButton
            // 
            this.ReportCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportCancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.ReportCancelButton.FlatAppearance.BorderSize = 0;
            this.ReportCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReportCancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ReportCancelButton.ForeColor = System.Drawing.Color.Snow;
            this.ReportCancelButton.Location = new System.Drawing.Point(411, 4);
            this.ReportCancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.ReportCancelButton.Name = "ReportCancelButton";
            this.ReportCancelButton.Size = new System.Drawing.Size(180, 45);
            this.ReportCancelButton.TabIndex = 40;
            this.ReportCancelButton.Text = "İPTAL";
            this.ReportCancelButton.UseVisualStyleBackColor = false;
            this.ReportCancelButton.Click += new System.EventHandler(this.ReportCancelButton_Click);
            // 
            // ReportTablePanel
            // 
            this.ReportTablePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(227)))), ((int)(((byte)(249)))));
            this.ReportTablePanel.Controls.Add(this.ReportMapSelectionComboBox);
            this.ReportTablePanel.Controls.Add(this.ReportSelectionComboBox);
            this.ReportTablePanel.Controls.Add(this.ReportsTableDataGridView);
            this.ReportTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportTablePanel.Location = new System.Drawing.Point(0, 0);
            this.ReportTablePanel.Name = "ReportTablePanel";
            this.ReportTablePanel.Size = new System.Drawing.Size(982, 527);
            this.ReportTablePanel.TabIndex = 6;
            // 
            // ReportMapSelectionComboBox
            // 
            this.ReportMapSelectionComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ReportMapSelectionComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.ReportMapSelectionComboBox.FormattingEnabled = true;
            this.ReportMapSelectionComboBox.Items.AddRange(new object[] {
            "EA Haritası",
            "DEK Haritası",
            "YGA Haritası",
            "Imar Haritası",
            "DTR Haritası",
            "Stokastik Harita"});
            this.ReportMapSelectionComboBox.Location = new System.Drawing.Point(282, 12);
            this.ReportMapSelectionComboBox.Name = "ReportMapSelectionComboBox";
            this.ReportMapSelectionComboBox.Size = new System.Drawing.Size(233, 25);
            this.ReportMapSelectionComboBox.TabIndex = 46;
            this.ReportMapSelectionComboBox.Text = "Dışa aktarmak için harita seçimi yapınız.";
            // 
            // ReportSelectionComboBox
            // 
            this.ReportSelectionComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ReportSelectionComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.ReportSelectionComboBox.FormattingEnabled = true;
            this.ReportSelectionComboBox.Items.AddRange(new object[] {
            "EA Sonuçları Tablosu",
            "DEK Sonuçları Tablosu",
            "YGA Sonuçları Tablosu",
            "Imar Sonuçları Tablosu",
            "DTR Sonuçları Tablosu",
            "Stokastik Sonuçlar Tablosu"});
            this.ReportSelectionComboBox.Location = new System.Drawing.Point(12, 12);
            this.ReportSelectionComboBox.Name = "ReportSelectionComboBox";
            this.ReportSelectionComboBox.Size = new System.Drawing.Size(233, 25);
            this.ReportSelectionComboBox.TabIndex = 45;
            this.ReportSelectionComboBox.Text = "Raporlama oluşturmak için modül seçimi yapınız.";
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
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(30)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(77)))), ((int)(((byte)(121)))));
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
            this.ReportsTableDataGridView.Location = new System.Drawing.Point(12, 70);
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
            this.ReportsTableDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ReportsTableDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ReportsTableDataGridView.Size = new System.Drawing.Size(763, 376);
            this.ReportsTableDataGridView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(227)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.ExportKmlButton);
            this.panel1.Controls.Add(this.ExportPngButton);
            this.panel1.Controls.Add(this.ExportCsvButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(786, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(196, 527);
            this.panel1.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button3.ForeColor = System.Drawing.Color.Snow;
            this.button3.Location = new System.Drawing.Point(14, 111);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(180, 45);
            this.button3.TabIndex = 44;
            this.button3.Text = "HARİTALAR";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button2.ForeColor = System.Drawing.Color.Snow;
            this.button2.Location = new System.Drawing.Point(14, 160);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 45);
            this.button2.TabIndex = 43;
            this.button2.Text = "SENARYO SONUÇLARI";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(47)))), ((int)(((byte)(159)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.ForeColor = System.Drawing.Color.Snow;
            this.button1.Location = new System.Drawing.Point(14, 209);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 45);
            this.button1.TabIndex = 42;
            this.button1.Text = "MODÜL TABLOLARI";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // ReportTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 527);
            this.Controls.Add(this.ReportTableBottomPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ReportTablePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportTableForm";
            this.Text = "Raporlama Penceresi";
            this.ReportTableBottomPanel.ResumeLayout(false);
            this.ReportTablePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReportsTableDataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ExportCsvButton;
        private System.Windows.Forms.Button ExportKmlButton;
        private System.Windows.Forms.Button ExportPngButton;
        private System.Windows.Forms.Panel ReportTableBottomPanel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button ReportCancelButton;
        private System.Windows.Forms.Panel ReportTablePanel;
        private System.Windows.Forms.DataGridView ReportsTableDataGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox ReportSelectionComboBox;
        private System.Windows.Forms.ComboBox ReportMapSelectionComboBox;
    }
}