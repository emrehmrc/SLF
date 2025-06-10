using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using ClosedXML.Excel;
using System.Globalization;
using System.Linq;

namespace SLF
{
    public partial class Nokta_Yuk_Bilgi_Formu : Form
    {
        private DataTable dataTable;
        private string excelFilePath;
        public ModülFormu modül_formu;
        public bool is_yukler_changed = false;

        public bool yuk_select;

        public Nokta_Yuk_Bilgi_Formu(string filePath, bool yukSelect)
        {
            InitializeComponent();

            excelFilePath = filePath;
            yuk_select = yukSelect;

            LoadDataFromExcel();
            modül_formu = new ModülFormu();
        }

        private void LoadDataFromExcel()
        {
            dataTable = new DataTable();

            // Define columns based on the selected condition
            if (yuk_select)
            {
                // Columns for the first sheet (YUK)
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("Ortalama Kapladığı Alan (m2)", typeof(string));
                dataTable.Columns.Add("Tüketim Sınıfı", typeof(string));
                dataTable.Columns.Add("ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));
            } else
            {
                // Handle invalid case 
                MessageBox.Show("Geçersiz seçim", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PopulateDefaultData();
                return;
            }

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

                        // Select the appropriate sheet
                        DataTable excelTable;
                        if (yuk_select)
                        {
                            // Read the first sheet (index 0)
                            if (result.Tables.Count < 1)
                            {
                                throw new Exception("Excel dosyasında 'YUK' için gerekli olan ilk sayfa bulunamadı.");
                            }
                            excelTable = result.Tables[0];
                        } else
                        {
                            throw new Exception("Geçersiz seçim");
                        }

                        // Copy data from Excel table to our DataTable
                        foreach (DataRow row in excelTable.Rows)
                        {
                            var newRow = dataTable.NewRow();
                            if (yuk_select)
                            {
                                newRow["Tipi"] = row["Tipi"]?.ToString() ?? string.Empty;
                                newRow["Ortalama Kapladığı Alan (m2)"] = row["Ortalama Kapladığı Alan (m2)"]?.ToString() ?? string.Empty;
                                newRow["Tüketim Sınıfı"] = row["Tüketim Sınıfı"]?.ToString() ?? string.Empty;
                                newRow["ENERJILENDIRME_YILI"] = row["ENERJILENDIRME_YILI"]?.ToString() ?? string.Empty;
                                newRow["Kurulu Güç (kW)"] = row["Kurulu Güç (kW)"]?.ToString() ?? string.Empty;
                                newRow["Pik Yüklenme (%)"] = row["Pik Yüklenme (%)"]?.ToString() ?? string.Empty;
                                newRow["Pik Demant (kW)"] = row["Pik Demant (kW)"]?.ToString() ?? string.Empty;
                            }

                            dataTable.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel dosyasını okurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PopulateDefaultData();
            }

            // Bind the DataTable to the YGADataGridView
            NoktaYukDataGridView.DataSource = dataTable;
            NoktaYukDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            NoktaYukDataGridView.AllowUserToAddRows = true;
            NoktaYukDataGridView.AllowUserToDeleteRows = true;
        }


        private void PopulateDefaultData()
        {
            // Ensure dataTable is initialized
            if (dataTable == null)
            {
                dataTable = new DataTable();
            }
            else
            {
                dataTable.Clear();
                dataTable.Columns.Clear();
            }


            // Define columns and default data based on the selected condition
            if (yuk_select)
            {
                // Define columns for the first sheet (YUK)
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("Ortalama Kapladığı Alan (m2)", typeof(string));
                dataTable.Columns.Add("Tüketim Sınıfı", typeof(string));
                dataTable.Columns.Add("ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));

                // Default data for YUK
                var defaultData = new List<string[]>
                {
                    new[] { "Anaokulu", "2,800", "TICARETHANE","2027", "30", "0.8", "24" },
                    new[] { "AVM", "10,000", "TICARETHANE", "2027", "1,000", "0.6", "600" },
                    new[] { "Banka", "500", "TICARETHANE", "2027", "100", "0.7", "70" },
                    new[] { "Akaryakıt İstasyonu", "1,400", "TICARETHANE", "2027", "50", "0.6", "30" },
                    new[] { "Cami", "300", "TICARETHANE", "2027", "50", "0.7", "35" },
                    new[] { "Fırın", "500", "TICARETHANE", "2027", "30", "0.7", "21" },
                    new[] { "Halk Sağlığı Merkezi", "400", "TICARETHANE", "2027", "150", "0.8", "120" },
                    new[] { "Hastane", "30,000", "TICARETHANE", "2027", "1,000", "0.9", "900" },
                    new[] { "İtfaiye", "1,100", "TICARETHANE", "2027", "100", "0.6", "60" },
                    new[] { "Kamu Binası", "1,200", "TICARETHANE", "2027", "200", "0.6", "120" },
                    new[] { "Konser Alanı", "2,000", "TICARETHANE", "2027", "400", "0.7", "280" },
                    new[] { "Okul", "7,500", "TICARETHANE", "2027", "300", "0.8", "240" },
                    new[] { "Oto Tamirci", "700", "TICARETHANE", "2027", "250", "0.7", "175" },
                    new[] { "Otogar", "5,800", "TICARETHANE", "2027", "350", "0.7", "245" },
                    new[] { "Otopark", "2,500", "TICARETHANE", "2027", "200", "0.8", "160" },
                    new[] { "Pazar Alanı", "5,400", "TICARETHANE", "2027", "200", "0.8", "160" },
                    new[] { "PTT", "500", "TICARETHANE", "2027", "50", "0.7", "35" },
                    new[] { "Restoran", "800", "TICARETHANE", "2027", "120", "0.6", "72" },
                    new[] { "Sanat Alanı", "3,000", "TICARETHANE", "2027", "100", "0.6", "60" },
                    new[] { "Sosyal Yaşam Merkezi", "1,500", "TICARETHANE", "2027", "300", "0.7", "210" },
                    new[] { "Süpermarket", "3,500", "TICARETHANE", "2027", "250", "0.7", "175" },
                    new[] { "Tarımsal Alan", "10,000", "TARIMSAL_SULAMA", "2027", "20", "0.7", "14" },
                    new[] { "Üniversite Kampüsü", "200,000", "TICARETHANE", "2027", "1,000", "0.6", "600" }
                };

                foreach (var rowData in defaultData)
                {
                    var row = dataTable.NewRow();
                    row["Tipi"] = rowData[0];
                    row["Ortalama Kapladığı Alan (m2)"] = rowData[1];
                    row["Tüketim Sınıfı"] = rowData[2];
                    row["ENERJILENDIRME_YILI"] = rowData[2]; 
                    row["Kurulu Güç (kW)"] = rowData[3];
                    row["Pik Yüklenme (%)"] = rowData[4];
                    row["Pik Demant (kW)"] = rowData[5];
                    dataTable.Rows.Add(row);
                }
            } else
            {
                // Handle invalid case 
                dataTable.Columns.Add("Tipi", typeof(string)); // Minimal column to avoid empty table
                var row = dataTable.NewRow();
                row["Tipi"] = "Hata: Geçersiz seçim";
                dataTable.Rows.Add(row);
            }
        }


        private bool ValidatePolygonData()
        {
            // Columns to validate
            string powerColumn = "Kurulu Güç (kW)";
            string peakLoadColumn = "Pik Yüklenme (%)";
            string peakDemandColumn = "Pik Demant (kW)";

            try
            {

                // Check if all required columns exist in the DataGridView
                if (!NoktaYukDataGridView.Columns.Contains(powerColumn) ||
                    !NoktaYukDataGridView.Columns.Contains(peakLoadColumn) ||
                    !NoktaYukDataGridView.Columns.Contains(peakDemandColumn))
                {
                    string missingColumns = string.Join(", ", new[] { powerColumn, peakLoadColumn, peakDemandColumn }
                        .Where(col => !NoktaYukDataGridView.Columns.Contains(col)));
                    MessageBox.Show($"Hata: Şu sütun(lar) bulunamadı: {missingColumns}.",
                        "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Get column indices
                int powerColumnIndex = NoktaYukDataGridView.Columns[powerColumn].Index;
                int peakLoadColumnIndex = NoktaYukDataGridView.Columns[peakLoadColumn].Index;
                int peakDemandColumnIndex = NoktaYukDataGridView.Columns[peakDemandColumn].Index;

                // Iterate over each row in the DataGridView
                for (int rowIndex = 0; rowIndex < NoktaYukDataGridView.Rows.Count; rowIndex++)
                {
                    var row = NoktaYukDataGridView.Rows[rowIndex];

                    // Skip the new row placeholder if it exists
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    // Get the cell values
                    string powerCellValue = row.Cells[powerColumnIndex].Value?.ToString();
                    string peakLoadCellValue = row.Cells[peakLoadColumnIndex].Value?.ToString();
                    string peakDemandCellValue = row.Cells[peakDemandColumnIndex].Value?.ToString();

                    // Validate "Kurulu Güç (kW)" if present
                    if (!string.IsNullOrWhiteSpace(powerCellValue))
                    {
                        if (!double.TryParse(powerCellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double powerValue))
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}' sütununda geçersiz bir değer: {powerCellValue}",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Check if the value exceeds 10,000
                        if (powerValue > 10000)
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}' değeri 10,000'i aşamaz. Değer: {powerValue}",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    // Validate equality only if all three values are present
                    if (!string.IsNullOrWhiteSpace(powerCellValue) &&
                        !string.IsNullOrWhiteSpace(peakLoadCellValue) &&
                        !string.IsNullOrWhiteSpace(peakDemandCellValue))
                    {
                        if (double.TryParse(powerCellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double powerValue) &&
                            double.TryParse(peakLoadCellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double peakLoadValue) &&
                            double.TryParse(peakDemandCellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double peakDemandValue))
                        {
                            // Calculate expected peak demand
                            double expectedPeakDemand = powerValue * (peakLoadValue); // Percentage correction

                            // Use a small tolerance for floating-point precision
                            const double tolerance = 1; // Increased slightly for robustness
                            if (Math.Abs(expectedPeakDemand - peakDemandValue) > tolerance)
                            {
                                MessageBox.Show(
                                    $"Satır {rowIndex + 1}: '{powerColumn}' * '{peakLoadColumn}' = '{peakDemandColumn}' eşitliği sağlanmıyor.\n" +
                                    $"Hesaplanan: {expectedPeakDemand:F2} kW, Girilen: {peakDemandValue:F2} kW",
                                    "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}', '{peakLoadColumn}' veya '{peakDemandColumn}' sütununda geçersiz bir değer var.\n" +
                                $"Değerler: Power='{powerCellValue}', PeakLoad='{peakLoadCellValue}', PeakDemand='{peakDemandCellValue}'",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Skipped equality validation (missing values).");
                    }
                }

                Console.WriteLine("ValidatePolygonData: Validation passed for all rows.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Doğrulama sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void NoktaYukTableSaveButton_Click(object sender, EventArgs e)
        {

            // Validate the data before saving
            if (!ValidatePolygonData())
            {
                return; // Stop saving if validation fails
            }


            try
            {
                // Load the existing Excel file
                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    // Determine which sheet to update
                    IXLWorksheet worksheet;
                    string sheetName;
                    int sheetIndex;

                    if (yuk_select)
                    {
                        sheetName = workbook.Worksheets.Count > 0 ? workbook.Worksheet(1).Name : "YUK";
                        sheetIndex = 1; // First sheet (index 1 in ClosedXML)
                    } else
                    {
                        throw new Exception("Geçersiz seçim: Yuk seçilmelidir.");
                    }

                    // Delete the existing sheet and recreate it to ensure clean data
                    if (workbook.Worksheets.Contains(sheetName))
                    {
                        workbook.Worksheets.Delete(sheetName);
                    }
                    worksheet = workbook.Worksheets.Add(sheetName, sheetIndex);

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

                    // Save the workbook
                    workbook.Save();
                }

                MessageBox.Show("Değişiklikler kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();

                if (yuk_select)
                    is_yukler_changed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikler kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}