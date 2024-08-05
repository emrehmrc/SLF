namespace SLF
{
    partial class Tablo_Formu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tablo_Formu));
            this.vektörel_attribute_table = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.vektörel_attribute_table)).BeginInit();
            this.SuspendLayout();
            // 
            // vektörel_attribute_table
            // 
            this.vektörel_attribute_table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vektörel_attribute_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.vektörel_attribute_table.Location = new System.Drawing.Point(12, 33);
            this.vektörel_attribute_table.Name = "vektörel_attribute_table";
            this.vektörel_attribute_table.RowHeadersWidth = 51;
            this.vektörel_attribute_table.RowTemplate.Height = 24;
            this.vektörel_attribute_table.Size = new System.Drawing.Size(1108, 528);
            this.vektörel_attribute_table.TabIndex = 0;
            // 
            // Tablo_Formu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1132, 573);
            this.Controls.Add(this.vektörel_attribute_table);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1150, 620);
            this.Name = "Tablo_Formu";
            this.Text = "Tablo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tablo_Formu_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.vektörel_attribute_table)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView vektörel_attribute_table;
    }
}