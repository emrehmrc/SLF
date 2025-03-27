using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using ClosedXML.Excel;

namespace SLF
{
    public partial class Nokta_Yuk_Bilgi_Formu : Form
    {
        private DataTable dataTable;
        private string excelFilePath;
        public ModülFormu modül_formu;
        public bool is_yukler_changed = false;

        public Nokta_Yuk_Bilgi_Formu(string filePath)
        {
            InitializeComponent();
            excelFilePath = filePath;
            LoadDataFromExcel();
            modül_formu = new ModülFormu();
        }

        private void LoadDataFromExcel()
        {
            dataTable = new DataTable();

            // Define columns based on your table
            dataTable.Columns.Add("Tipi", typeof(string));
            dataTable.Columns.Add("Ortalama Kapladığı Alan (m2)", typeof(string));
            dataTable.Columns.Add("Tüketim Sınıfı", typeof(string));
            dataTable.Columns.Add("Kurulu Güç", typeof(string));
            dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
            dataTable.Columns.Add("Pik Demant", typeof(string));

            try
            {
                // Use ExcelDataReader to read the Excel file
                using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Read the Excel data into a DataSet
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true // Use the first row as column headers
                            }
                        });

                        // Get the first table from the DataSet
                        var excelTable = result.Tables[0];

                        // Copy data from Excel table to our DataTable
                        foreach (DataRow row in excelTable.Rows)
                        {
                            var newRow = dataTable.NewRow();
                            newRow["Tipi"] = row["Tipi"]?.ToString();
                            newRow["Ortalama Kapladığı Alan (m2)"] = row["Ortalama Kapladığı Alan (m2)"]?.ToString();
                            newRow["Tüketim Sınıfı"] = row["Tüketim Sınıfı"]?.ToString();
                            newRow["Kurulu Güç"] = row["Kurulu Güç"]?.ToString();
                            newRow["Pik Yüklenme (%)"] = row["Pik Yüklenme (%)"]?.ToString();
                            newRow["Pik Demant"] = row["Pik Demant"]?.ToString();
                            dataTable.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel dosyasını okurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Populate with default data if the file cannot be read
                PopulateDefaultData();
            }

            // Bind the DataTable to the YGADataGridView
            YGADataGridView.DataSource = dataTable;
            YGADataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            YGADataGridView.AllowUserToAddRows = true;
            YGADataGridView.AllowUserToDeleteRows = true;
        }

        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Use ClosedXML to write the DataTable back to the Excel file
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("PolygonTypes");
                    // Write headers
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cell(1, col + 1).Value = dataTable.Columns[col].ColumnName;
                    }
                    // Write data rows
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            worksheet.Cell(row + 2, col + 1).Value = dataTable.Rows[row][col]?.ToString();
                        }
                    }
                    workbook.SaveAs(excelFilePath);
                }
                MessageBox.Show("Değişiklikler kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();

                is_yukler_changed = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikler kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDefaultData()
        {
            // Default data in case the Excel file cannot be read
            var defaultData = new List<string[]>
        {
            new[] { "Anaokulu", "2,800", "Orta Ticarethane", "30", "0.8", "24" },
            new[] { "AVM", "10,000", "Büyük Ticarethane", "1,000", "0.6", "600" },
            new[] { "Banka", "500", "Orta Ticarethane", "100", "0.7", "70" },
            new[] { "Akaryakıt İstasyonu", "1,400", "Orta Ticarethane", "50", "0.6", "30" },
            new[] { "Cami", "300", "Orta Ticarethane", "50", "0.7", "35" },
            new[] { "Fırın", "500", "Orta Ticarethane", "30", "0.7", "21" },
            new[] { "Halk Sağlığı Merkezi", "400", "Orta Ticarethane", "150", "0.8", "120" },
            new[] { "Hastane", "30,000", "Büyük Ticarethane", "1,000", "0.9", "900" },
            new[] { "İtfaiye", "1,100", "Orta Ticarethane", "100", "0.6", "60" },
            new[] { "Kamu Binası", "1,200", "Orta Ticarethane", "200", "0.6", "120" },
            new[] { "Konser Alanı", "2,000", "Orta Ticarethane", "400", "0.7", "280" },
            new[] { "Okul", "7,500", "Orta Ticarethane", "300", "0.8", "240" },
            new[] { "Oto Tamirci", "700", "Orta Ticarethane", "250", "0.7", "175" },
            new[] { "Otogar", "5,800", "Büyük Ticarethane", "350", "0.7", "245" },
            new[] { "Otopark", "2,500", "Orta Ticarethane", "200", "0.8", "160" },
            new[] { "Pazar Alanı", "5,400", "Orta Ticarethane", "200", "0.8", "160" },
            new[] { "PTT", "500", "Orta Ticarethane", "50", "0.7", "35" },
            new[] { "Restoran", "800", "Orta Ticarethane", "120", "0.6", "72" },
            new[] { "Sanat Alanı", "3,000", "Orta Ticarethane", "100", "0.6", "60" },
            new[] { "Sosyal Yaşam Merkezi", "1,500", "Orta Ticarethane", "300", "0.7", "210" },
            new[] { "Süpermarket", "3,500", "Orta Ticarethane", "250", "0.7", "175" },
            new[] { "Üniversite Kampüsü", "200,000", "Büyük Ticarethane", "1,000", "0.6", "600" }
        };

            foreach (var rowData in defaultData)
            {
                var row = dataTable.NewRow();
                row["Tipi"] = rowData[0];
                row["Ortalama Kapladığı Alan (m2)"] = rowData[1];
                row["Tüketim Sınıfı"] = rowData[2];
                row["Kurulu Güç"] = rowData[3];
                row["Pik Yüklenme (%)"] = rowData[4];
                row["Pik Demant"] = rowData[5];
                dataTable.Rows.Add(row);
            }
        }

    }
}