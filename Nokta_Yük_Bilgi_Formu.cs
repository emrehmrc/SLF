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
        public bool is_musaade_changed = false;

        public bool yuk_select;
        public bool musaade_select;

        public Nokta_Yuk_Bilgi_Formu(string filePath, bool yukSelect, bool musaadeSelect)
        {
            InitializeComponent();

            excelFilePath = filePath;
            yuk_select = yukSelect;
            musaade_select = musaadeSelect;

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
            }
            else if (musaade_select)
            {
                // Columns for the second sheet (Musaade)
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ABONE_GRUBU", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));
            }
            else
            {
                // Handle invalid case (neither YUK nor Musaade selected)
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
                        }
                        else if (musaade_select)
                        {
                            // Read the second sheet (index 1)
                            if (result.Tables.Count < 2)
                            {
                                throw new Exception("Excel dosyasında 'Musaade' için gerekli olan ikinci sayfa bulunamadı.");
                            }
                            excelTable = result.Tables[1];
                        }
                        else
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
                            else if (musaade_select)
                            {
                                newRow["Tipi"] = row["Tipi"]?.ToString() ?? string.Empty;
                                newRow["ENERJI_MUSAADE_ABONE_GRUBU"] = row["ENERJI_MUSAADE_ABONE_GRUBU"]?.ToString() ?? string.Empty;
                                newRow["ENERJI_MUSAADE_ENERJILENDIRME_YILI"] = row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"]?.ToString() ?? string.Empty;
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
                    new[] { "Anaokulu", "2,800", "Ticarethane","2027", "30", "0.8", "24" },
                    new[] { "AVM", "10,000", "Ticarethane", "2027", "1,000", "0.6", "600" },
                    new[] { "Banka", "500", "Ticarethane", "2027", "100", "0.7", "70" },
                    new[] { "Akaryakıt İstasyonu", "1,400", "Ticarethane", "2027", "50", "0.6", "30" },
                    new[] { "Cami", "300", "Ticarethane", "2027", "50", "0.7", "35" },
                    new[] { "Fırın", "500", "Ticarethane", "2027", "30", "0.7", "21" },
                    new[] { "Halk Sağlığı Merkezi", "400", "Ticarethane", "2027", "150", "0.8", "120" },
                    new[] { "Hastane", "30,000", "Ticarethane", "2027", "1,000", "0.9", "900" },
                    new[] { "İtfaiye", "1,100", "Ticarethane", "2027", "100", "0.6", "60" },
                    new[] { "Kamu Binası", "1,200", "Ticarethane", "2027", "200", "0.6", "120" },
                    new[] { "Konser Alanı", "2,000", "Ticarethane", "2027", "400", "0.7", "280" },
                    new[] { "Okul", "7,500", "Ticarethane", "2027", "300", "0.8", "240" },
                    new[] { "Oto Tamirci", "700", "Ticarethane", "2027", "250", "0.7", "175" },
                    new[] { "Otogar", "5,800", "Ticarethane", "2027", "350", "0.7", "245" },
                    new[] { "Otopark", "2,500", "Ticarethane", "2027", "200", "0.8", "160" },
                    new[] { "Pazar Alanı", "5,400", "Ticarethane", "2027", "200", "0.8", "160" },
                    new[] { "PTT", "500", "Ticarethane", "2027", "50", "0.7", "35" },
                    new[] { "Restoran", "800", "Ticarethane", "2027", "120", "0.6", "72" },
                    new[] { "Sanat Alanı", "3,000", "Ticarethane", "2027", "100", "0.6", "60" },
                    new[] { "Sosyal Yaşam Merkezi", "1,500", "Ticarethane", "2027", "300", "0.7", "210" },
                    new[] { "Süpermarket", "3,500", "Ticarethane", "2027", "250", "0.7", "175" },
                    new[] { "Tarımsal Alan", "10,000", "Tarımsal Sulama", "2027", "20", "0.7", "14" },
                    new[] { "Üniversite Kampüsü", "200,000", "Ticarethane", "2027", "1,000", "0.6", "600" }
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
            }
            else if (musaade_select)
            {
                // Define columns for the second sheet (Musaade)
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ABONE_GRUBU", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));

                // Default data for Musaade (based on the provided sample)
                var defaultData = new List<string[]>
                {
                    new[] { "ACİL SERVİS", "Ticarethane", "2026", "167", "0.6", "100" },
                    new[] { "ANAOKULU", "Ticarethane", "2026", "86", "0.7", "60" },
                    new[] { "ATM", "Ticarethane", "2026", "10", "0.3", "3" },
                    new[] { "AVM", "Ticarethane", "2026", "1800", "0.5", "900" },
                    new[] { "BAZ İSTASYONU", "Ticarethane", "2026", "1800", "0.5", "900" },
                    new[] { "BİNA", "Mesken", "2026", "36", "0.7", "25" },
                    new[] { "BÜFE", "Ticarethane", "2026", "7", "0.7", "5" },
                    new[] { "DERNEK", "Ticarethane", "2026", "17", "0.7", "12" },
                    new[] { "GÜÇ ARTIRIMI - ATM", "Ticarethane", "2026", "4", "1", "4" },
                    new[] { "GÜÇ ARTIRIMI - İŞYERİ", "Ticarethane", "2026", "25", "1", "25" },
                    new[] { "GÜÇ ARTIRIMI - TİCARETHANE", "Ticarethane", "2026", "30", "1", "30" },
                    new[] { "GÜÇ ARTIRIMI - TARIMSAL SULAMA", "Tarımsal Sulama", "2026", "20", "1", "20" },
                    new[] { "GEÇİCİ - DEPO", "Ticarethane", "2026", "50", "0.4", "20" },
                    new[] { "GEÇİCİ - HOBİ BAHÇESİ", "Mesken", "2026", "8", "0.4", "3" },
                    new[] { "GEÇİCİ - PARK AYDINLATMASI", "Ticarethane", "2026", "83", "0.6", "50" },
                    new[] { "GEÇİCİ - BÜFE", "Ticarethane", "2026", "17", "0.3", "5" },
                    new[] { "İBADETHANE", "Ticarethane", "2026", "42", "0.6", "25" },
                    new[] { "İÇME SUYU", "Ticarethane", "2026", "67", "0.6", "40" },
                    new[] { "İSTASYON", "Ticarethane", "2026", "67", "0.6", "40" },
                    new[] { "MESKEN", "Mesken", "2026", "9", "0.7", "6" },
                    new[] { "MOBESE KAMERASI", "Ticarethane", "2026", "8", "0.6", "5" },
                    new[] { "OKUL", "Ticarethane", "2026", "150", "0.6", "90" },
                    new[] { "OTOPARK", "Ticarethane", "2026", "80", "0.5", "40" },
                    new[] { "ÖĞRENCİ YURDU", "Ticarethane", "2026", "167", "0.6", "100" },
                    new[] { "RESMİ KURUM", "Ticarethane", "2026", "133", "0.6", "80" },
                    new[] { "SANAYİ", "Ticarethane", "2026", "500", "0.5", "250" },
                    new[] { "SİNYALİZASYON", "Ticarethane", "2026", "17", "0.6", "10" },
                    new[] { "SONDAJ KUYUSU", "Tarımsal Sulama", "2026", "60", "0.5", "30" },
                    new[] { "SPOR KOMPLEKSİ", "Ticarethane", "2026", "200", "0.6", "120" },
                    new[] { "ŞARJ İSTASYONU", "Ticarethane", "2026", "167", "0.6", "100" },
                    new[] { "ŞANTİYE", "Ticarethane", "2026", "250", "0.6", "150" },
                    new[] { "TARIMSAL SULAMA", "Tarımsal Sulama", "2026", "133", "0.6", "80" },
                    new[] { "TERFİ İSTASYONU", "Ticarethane", "2026", "91", "0.7", "64" },
                    new[] { "TİCARETHANE", "Ticarethane", "2026", "100", "0.7", "70" },
                    new[] { "YURT", "Ticarethane", "2026", "150", "0.7", "105" }
                };

                foreach (var rowData in defaultData)
                {
                    var row = dataTable.NewRow();
                    row["Tipi"] = rowData[0];
                    row["ENERJI_MUSAADE_ABONE_GRUBU"] = rowData[1];
                    row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"] = rowData[2];
                    row["Kurulu Güç (kW)"] = rowData[3];
                    row["Pik Yüklenme (%)"] = rowData[4];
                    row["Pik Demant (kW)"] = rowData[5];
                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                // Handle invalid case (neither YUK nor Musaade selected)
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
                Console.WriteLine("ValidatePolygonData: Method started.");

                // Check if all required columns exist in the DataGridView
                if (!NoktaYukDataGridView.Columns.Contains(powerColumn) ||
                    !NoktaYukDataGridView.Columns.Contains(peakLoadColumn) ||
                    !NoktaYukDataGridView.Columns.Contains(peakDemandColumn))
                {
                    string missingColumns = string.Join(", ", new[] { powerColumn, peakLoadColumn, peakDemandColumn }
                        .Where(col => !NoktaYukDataGridView.Columns.Contains(col)));
                    MessageBox.Show($"Hata: Şu sütun(lar) bulunamadı: {missingColumns}.",
                        "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine($"ValidatePolygonData: Missing columns: {missingColumns}");
                    return false;
                }

                // Get column indices
                int powerColumnIndex = NoktaYukDataGridView.Columns[powerColumn].Index;
                int peakLoadColumnIndex = NoktaYukDataGridView.Columns[peakLoadColumn].Index;
                int peakDemandColumnIndex = NoktaYukDataGridView.Columns[peakDemandColumn].Index;

                Console.WriteLine($"ValidatePolygonData: Columns found. PowerIndex={powerColumnIndex}, PeakLoadIndex={peakLoadColumnIndex}, PeakDemandIndex={peakDemandColumnIndex}");
                Console.WriteLine($"ValidatePolygonData: Total rows={NoktaYukDataGridView.Rows.Count}");

                // Iterate over each row in the DataGridView
                for (int rowIndex = 0; rowIndex < NoktaYukDataGridView.Rows.Count; rowIndex++)
                {
                    var row = NoktaYukDataGridView.Rows[rowIndex];

                    // Skip the new row placeholder if it exists
                    if (row.IsNewRow)
                    {
                        Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} skipped (new row placeholder).");
                        continue;
                    }

                    // Get the cell values
                    string powerCellValue = row.Cells[powerColumnIndex].Value?.ToString();
                    string peakLoadCellValue = row.Cells[peakLoadColumnIndex].Value?.ToString();
                    string peakDemandCellValue = row.Cells[peakDemandColumnIndex].Value?.ToString();

                    Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Power='{powerCellValue}', PeakLoad='{peakLoadCellValue}', PeakDemand='{peakDemandCellValue}'");

                    // Validate "Kurulu Güç (kW)" if present
                    if (!string.IsNullOrWhiteSpace(powerCellValue))
                    {
                        if (!double.TryParse(powerCellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double powerValue))
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}' sütununda geçersiz bir değer: {powerCellValue}",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Invalid power value: {powerCellValue}");
                            return false;
                        }

                        // Check if the value exceeds 10,000
                        if (powerValue > 10000)
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}' değeri 10,000'i aşamaz. Değer: {powerValue}",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Power value {powerValue} exceeds 10,000.");
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
                            double expectedPeakDemand = powerValue * (peakLoadValue / 100); // Percentage correction
                            Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Power={powerValue}, PeakLoad={peakLoadValue}, ExpectedPeakDemand={expectedPeakDemand}, ActualPeakDemand={peakDemandValue}");

                            // Use a small tolerance for floating-point precision
                            const double tolerance = 0.001; // Increased slightly for robustness
                            if (Math.Abs(expectedPeakDemand - peakDemandValue) > tolerance)
                            {
                                MessageBox.Show(
                                    $"Satır {rowIndex + 1}: '{powerColumn}' * '{peakLoadColumn}' / 100 = '{peakDemandColumn}' eşitliği sağlanmıyor.\n" +
                                    $"Hesaplanan: {expectedPeakDemand:F2} kW, Girilen: {peakDemandValue:F2} kW",
                                    "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Equality failed. Expected: {expectedPeakDemand:F2}, Actual: {peakDemandValue:F2}");
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Satır {rowIndex + 1}: '{powerColumn}', '{peakLoadColumn}' veya '{peakDemandColumn}' sütununda geçersiz bir değer var.\n" +
                                $"Değerler: Power='{powerCellValue}', PeakLoad='{peakLoadCellValue}', PeakDemand='{peakDemandCellValue}'",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.WriteLine($"ValidatePolygonData: Row {rowIndex + 1} - Invalid values: Power='{powerCellValue}', PeakLoad='{peakLoadCellValue}', PeakDemand='{peakDemandCellValue}'");
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
                Console.WriteLine($"ValidatePolygonData: Exception occurred: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
                    }
                    else if (musaade_select)
                    {
                        sheetName = workbook.Worksheets.Count > 1 ? workbook.Worksheet(2).Name : "Musaade";
                        sheetIndex = 2; // Second sheet (index 2 in ClosedXML)
                    }
                    else
                    {
                        throw new Exception("Geçersiz seçim: Yuk veya Musaade seçilmelidir.");
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
                else if (musaade_select)
                    is_musaade_changed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikler kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}