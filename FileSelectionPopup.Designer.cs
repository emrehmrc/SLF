namespace SLF
{
    partial class FileSelectionPopup
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
            this.SelectCsvButton = new System.Windows.Forms.Button();
            this.SelectKmlButton = new System.Windows.Forms.Button();
            this.kmlFilePathTextBox = new System.Windows.Forms.TextBox();
            this.csvFilePathTextBox = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SelectCsvButton
            // 
            this.SelectCsvButton.Location = new System.Drawing.Point(235, 89);
            this.SelectCsvButton.Name = "SelectCsvButton";
            this.SelectCsvButton.Size = new System.Drawing.Size(75, 23);
            this.SelectCsvButton.TabIndex = 0;
            this.SelectCsvButton.Text = "CSV";
            this.SelectCsvButton.UseVisualStyleBackColor = true;
            this.SelectCsvButton.Click += new System.EventHandler(this.SelectCsvButton_Click);
            // 
            // SelectKmlButton
            // 
            this.SelectKmlButton.Location = new System.Drawing.Point(235, 132);
            this.SelectKmlButton.Name = "SelectKmlButton";
            this.SelectKmlButton.Size = new System.Drawing.Size(75, 23);
            this.SelectKmlButton.TabIndex = 1;
            this.SelectKmlButton.Text = "KML";
            this.SelectKmlButton.UseVisualStyleBackColor = true;
            this.SelectKmlButton.Click += new System.EventHandler(this.SelectKmlButton_Click);
            // 
            // kmlFilePathTextBox
            // 
            this.kmlFilePathTextBox.Location = new System.Drawing.Point(90, 134);
            this.kmlFilePathTextBox.Name = "kmlFilePathTextBox";
            this.kmlFilePathTextBox.Size = new System.Drawing.Size(100, 20);
            this.kmlFilePathTextBox.TabIndex = 2;
            // 
            // csvFilePathTextBox
            // 
            this.csvFilePathTextBox.Location = new System.Drawing.Point(90, 89);
            this.csvFilePathTextBox.Name = "csvFilePathTextBox";
            this.csvFilePathTextBox.Size = new System.Drawing.Size(100, 20);
            this.csvFilePathTextBox.TabIndex = 3;
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(235, 209);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 4;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // FileSelectionPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.csvFilePathTextBox);
            this.Controls.Add(this.kmlFilePathTextBox);
            this.Controls.Add(this.SelectKmlButton);
            this.Controls.Add(this.SelectCsvButton);
            this.Name = "FileSelectionPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FileSelectionPopup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectCsvButton;
        private System.Windows.Forms.Button SelectKmlButton;
        private System.Windows.Forms.TextBox kmlFilePathTextBox;
        private System.Windows.Forms.TextBox csvFilePathTextBox;
        private System.Windows.Forms.Button OkButton;
    }
}