namespace SLF
{
    partial class YGAPopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YGAPopupForm));
            this.YGADataGridView = new System.Windows.Forms.DataGridView();
            this.YGATablePanel = new System.Windows.Forms.Panel();
            this.YGATBottomPanel = new System.Windows.Forms.Panel();
            this.YGASaveButton = new System.Windows.Forms.Button();
            this.YGACancelButton = new System.Windows.Forms.Button();
            this.yasakli_alan_percentage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.left = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.top = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bottom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDevelopmentArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imar_tipi = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ilce = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agirlik_hiz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.baslangic_yili = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.YGADataGridView)).BeginInit();
            this.YGATablePanel.SuspendLayout();
            this.YGATBottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // YGADataGridView
            // 
            this.YGADataGridView.AllowUserToAddRows = false;
            this.YGADataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.YGADataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.YGADataGridView.BackgroundColor = System.Drawing.Color.Snow;
            this.YGADataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.YGADataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.YGADataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(13)))), ((int)(((byte)(34)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(19)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.YGADataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.YGADataGridView.ColumnHeadersHeight = 25;
            this.YGADataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yasakli_alan_percentage,
            this.id,
            this.left,
            this.right,
            this.top,
            this.bottom,
            this.IsDevelopmentArea,
            this.lat,
            this.lon,
            this.imar_tipi,
            this.ilce,
            this.taks,
            this.agirlik_hiz,
            this.baslangic_yili});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SeaShell;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.YGADataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.YGADataGridView.EnableHeadersVisualStyles = false;
            this.YGADataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(204)))), ((int)(((byte)(213)))));
            this.YGADataGridView.Location = new System.Drawing.Point(46, 24);
            this.YGADataGridView.Name = "YGADataGridView";
            this.YGADataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.YGADataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.YGADataGridView.RowHeadersWidth = 18;
            this.YGADataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.YGADataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.YGADataGridView.Size = new System.Drawing.Size(990, 387);
            this.YGADataGridView.TabIndex = 0;
            // 
            // YGATablePanel
            // 
            this.YGATablePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(181)))), ((int)(((byte)(152)))));
            this.YGATablePanel.Controls.Add(this.YGADataGridView);
            this.YGATablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.YGATablePanel.Location = new System.Drawing.Point(0, 0);
            this.YGATablePanel.Name = "YGATablePanel";
            this.YGATablePanel.Size = new System.Drawing.Size(1078, 465);
            this.YGATablePanel.TabIndex = 2;
            // 
            // YGATBottomPanel
            // 
            this.YGATBottomPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(181)))), ((int)(((byte)(152)))));
            this.YGATBottomPanel.Controls.Add(this.YGASaveButton);
            this.YGATBottomPanel.Controls.Add(this.YGACancelButton);
            this.YGATBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.YGATBottomPanel.Location = new System.Drawing.Point(0, 414);
            this.YGATBottomPanel.Name = "YGATBottomPanel";
            this.YGATBottomPanel.Size = new System.Drawing.Size(1078, 51);
            this.YGATBottomPanel.TabIndex = 3;
            // 
            // YGASaveButton
            // 
            this.YGASaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YGASaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(13)))), ((int)(((byte)(34)))));
            this.YGASaveButton.FlatAppearance.BorderSize = 0;
            this.YGASaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.YGASaveButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.YGASaveButton.ForeColor = System.Drawing.Color.Snow;
            this.YGASaveButton.Location = new System.Drawing.Point(896, 2);
            this.YGASaveButton.Margin = new System.Windows.Forms.Padding(2);
            this.YGASaveButton.Name = "YGASaveButton";
            this.YGASaveButton.Size = new System.Drawing.Size(180, 45);
            this.YGASaveButton.TabIndex = 41;
            this.YGASaveButton.Text = "KAYDET";
            this.YGASaveButton.UseVisualStyleBackColor = false;
            this.YGASaveButton.Click += new System.EventHandler(this.YGASaveButton_Click);
            // 
            // YGACancelButton
            // 
            this.YGACancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YGACancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(13)))), ((int)(((byte)(34)))));
            this.YGACancelButton.FlatAppearance.BorderSize = 0;
            this.YGACancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.YGACancelButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.YGACancelButton.ForeColor = System.Drawing.Color.Snow;
            this.YGACancelButton.Location = new System.Drawing.Point(712, 2);
            this.YGACancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.YGACancelButton.Name = "YGACancelButton";
            this.YGACancelButton.Size = new System.Drawing.Size(180, 45);
            this.YGACancelButton.TabIndex = 40;
            this.YGACancelButton.Text = "İPTAL";
            this.YGACancelButton.UseVisualStyleBackColor = false;
            //this.YGACancelButton.Click += new System.EventHandler(this.YGACancelButton_Click);
            // 
            // yasakli_alan_percentage
            // 
            this.yasakli_alan_percentage.HeaderText = "yasakli_alan_percentage";
            this.yasakli_alan_percentage.Name = "yasakli_alan_percentage";
            this.yasakli_alan_percentage.Width = 156;
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.Width = 40;
            // 
            // left
            // 
            this.left.HeaderText = "left";
            this.left.Name = "left";
            this.left.Width = 47;
            // 
            // right
            // 
            this.right.HeaderText = "right";
            this.right.Name = "right";
            this.right.Width = 55;
            // 
            // top
            // 
            this.top.HeaderText = "top";
            this.top.Name = "top";
            this.top.Width = 48;
            // 
            // bottom
            // 
            this.bottom.HeaderText = "bottom";
            this.bottom.Name = "bottom";
            this.bottom.Width = 70;
            // 
            // IsDevelopmentArea
            // 
            this.IsDevelopmentArea.HeaderText = "IsDevelopmentArea";
            this.IsDevelopmentArea.Name = "IsDevelopmentArea";
            this.IsDevelopmentArea.Width = 135;
            // 
            // lat
            // 
            this.lat.HeaderText = "lat";
            this.lat.Name = "lat";
            this.lat.Width = 43;
            // 
            // lon
            // 
            this.lon.HeaderText = "lon";
            this.lon.Name = "lon";
            this.lon.Width = 47;
            // 
            // imar_tipi
            // 
            this.imar_tipi.HeaderText = "imar_tipi";
            this.imar_tipi.Name = "imar_tipi";
            this.imar_tipi.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.imar_tipi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.imar_tipi.Width = 76;
            // 
            // ilce
            // 
            this.ilce.HeaderText = "ilce";
            this.ilce.Name = "ilce";
            this.ilce.Width = 48;
            // 
            // taks
            // 
            this.taks.HeaderText = "taks";
            this.taks.Name = "taks";
            this.taks.Width = 51;
            // 
            // agirlik_hiz
            // 
            this.agirlik_hiz.HeaderText = "agirlik_hiz";
            this.agirlik_hiz.Name = "agirlik_hiz";
            this.agirlik_hiz.Width = 83;
            // 
            // baslangic_yili
            // 
            this.baslangic_yili.HeaderText = "baslangic_yili";
            this.baslangic_yili.Name = "baslangic_yili";
            // 
            // YGAPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 465);
            this.Controls.Add(this.YGATBottomPanel);
            this.Controls.Add(this.YGATablePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "YGAPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yeni Genişleme Alanı Bilgileri";
            ((System.ComponentModel.ISupportInitialize)(this.YGADataGridView)).EndInit();
            this.YGATablePanel.ResumeLayout(false);
            this.YGATBottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView YGADataGridView;
        private System.Windows.Forms.Panel YGATablePanel;
        private System.Windows.Forms.Panel YGATBottomPanel;
        private System.Windows.Forms.Button YGACancelButton;
        private System.Windows.Forms.Button YGASaveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn yasakli_alan_percentage;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn left;
        private System.Windows.Forms.DataGridViewTextBoxColumn right;
        private System.Windows.Forms.DataGridViewTextBoxColumn top;
        private System.Windows.Forms.DataGridViewTextBoxColumn bottom;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsDevelopmentArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn lat;
        private System.Windows.Forms.DataGridViewTextBoxColumn lon;
        private System.Windows.Forms.DataGridViewComboBoxColumn imar_tipi;
        private System.Windows.Forms.DataGridViewTextBoxColumn ilce;
        private System.Windows.Forms.DataGridViewTextBoxColumn taks;
        private System.Windows.Forms.DataGridViewTextBoxColumn agirlik_hiz;
        private System.Windows.Forms.DataGridViewTextBoxColumn baslangic_yili;
    }
}