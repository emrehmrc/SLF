using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using SLF.services;
using SLF.Services;

namespace SLF
{
    public partial class DatabaseListForm : Form
    {
        private readonly Dictionary<string, List<string>> requiredColumns = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "DTR Verileri", new List<string> {
                "TRAFO_ID",
                "TRAFO_KODU",
                "TRAFO_ILCE_ADI",
                "TRAFO_KAPASITESI",
                "TRAFO_X_KOORDINAT",
                "TRAFO_Y_KOORDINAT"
            }},
            { "Abone Verileri", new List<string> {
                "TESISAT_NO",
                "ABONE_X_KOORDINAT",
                "ABONE_Y_KOORDINAT",
                "BAGLANDIGI_TRAFO_KODU",
                "BAGLANTI_GUCU",
                "ABONE_GRUBU"
            }}
        };

        public DatabaseListForm()
        {
            InitializeComponent();
        }

       

        

        private void buttonAboneVerisiOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // Config yolunu al
                string configPath = PathService._configKonum;

                // Python script yolunu al


                PythonHelper.RunPythonScriptForAboneVerisi(configPath);

                // Formu göster


                // İşlem başarılı olduysa dialog'u kapat
                MessageBox.Show("Abone verisi başarıyla oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        // Tablo seçim formu
        public class SelectTableForm : Form
        {
            private ComboBox comboBoxTables;
            private Button buttonSelect;
            private Button buttonCancel;

            public string SelectedTable { get; private set; }

            public SelectTableForm(DataTable tables)
            {
                InitializeComponent();

                // Tabloları combo box'a ekle
                foreach (DataRow row in tables.Rows)
                {
                    comboBoxTables.Items.Add(row[0].ToString());
                }

                if (comboBoxTables.Items.Count > 0)
                {
                    comboBoxTables.SelectedIndex = 0;
                }
            }

            private void InitializeComponent()
            {
                this.comboBoxTables = new System.Windows.Forms.ComboBox();
                this.buttonSelect = new System.Windows.Forms.Button();
                this.buttonCancel = new System.Windows.Forms.Button();
                this.SuspendLayout();
                // 
                // comboBoxTables
                // 
                this.comboBoxTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.comboBoxTables.FormattingEnabled = true;
                this.comboBoxTables.Location = new System.Drawing.Point(12, 20);
                this.comboBoxTables.Name = "comboBoxTables";
                this.comboBoxTables.Size = new System.Drawing.Size(360, 21);
                this.comboBoxTables.TabIndex = 0;
                // 
                // buttonSelect
                // 
                this.buttonSelect.Location = new System.Drawing.Point(216, 60);
                this.buttonSelect.Name = "buttonSelect";
                this.buttonSelect.Size = new System.Drawing.Size(75, 23);
                this.buttonSelect.TabIndex = 1;
                this.buttonSelect.Text = "Seç";
                this.buttonSelect.UseVisualStyleBackColor = true;
                this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
                // 
                // buttonCancel
                // 
                this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.buttonCancel.Location = new System.Drawing.Point(297, 60);
                this.buttonCancel.Name = "buttonCancel";
                this.buttonCancel.Size = new System.Drawing.Size(75, 23);
                this.buttonCancel.TabIndex = 2;
                this.buttonCancel.Text = "İptal";
                this.buttonCancel.UseVisualStyleBackColor = true;
                // 
                // SelectTableForm
                // 
                this.AcceptButton = this.buttonSelect;
                this.CancelButton = this.buttonCancel;
                this.ClientSize = new System.Drawing.Size(384, 91);
                this.Controls.Add(this.buttonCancel);
                this.Controls.Add(this.buttonSelect);
                this.Controls.Add(this.comboBoxTables);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "SelectTableForm";
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                this.Text = "Tablo Seçin";
                this.ResumeLayout(false);
            }

            private void buttonSelect_Click(object sender, EventArgs e)
            {
                if (comboBoxTables.SelectedItem != null)
                {
                    SelectedTable = comboBoxTables.SelectedItem.ToString();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Lütfen bir tablo seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }}