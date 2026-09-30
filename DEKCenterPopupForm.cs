using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class DEKCenterPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        private NoktaVeri veri; // Store the 'veri' object in the class field

        public HomePageForm ana_menu_form_objesi;

        public bool OperationCancelled => isOperationCancelled;

        private static class Constants
        {
            public static readonly Dictionary<string, string> DekValueToCountColumn = new Dictionary<string, string>
        {
            { "DEK_KURULU_GUCU", "DEK_distributed" }
        };

            public static readonly List<int> Years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
        }

        private readonly int slfEndYear;

        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri, int slfEndYear)
        {
            InitializeComponent();
            dataTable = existingDataTable;
            this.veri = veri;
            this.slfEndYear = slfEndYear;

            ana_menu_form_objesi = new HomePageForm();

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Fill initial coordinates from veri object
            DEKCenterDataGridView.Rows.Add();
            // Konvansiyon: X = boylam (longitude), Y = enlem (latitude) - Abone/DTR/EA modulleriyle tutarli.
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Boylam;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Enlem;

            // Set the cell (grid) ID
            DEKCenterDataGridView.Rows[0].Cells["ID"].Value =
                !string.IsNullOrEmpty(veri.CellId) ? veri.CellId : "Not Selected";

            // Initialize StartYear as a ComboBox with valid years
            if (DEKCenterDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearComboBox)
            {
                // Configure the ComboBox column
                startYearComboBox.DataSource = Constants.Years; // List<int> [2024, 2025, ..., 2030]
                startYearComboBox.ValueType = typeof(int); // Ensure the value type is int

                DEKCenterDataGridView.Rows[0].Cells["StartYear"].Value = Constants.Years.Min(); // Set to 2024 initially

            }
            else if (DEKCenterDataGridView.Columns.Contains("StartYear"))
            {
                // Fallback for non-ComboBox column (shouldn't execute in your case)
                DEKCenterDataGridView.Rows[0].Cells["StartYear"].Value = Constants.Years.Min().ToString();
            }

            // Populate transformer codes if available
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                    .Select(row => row["TRAFO_KODU"].ToString())
                    .Distinct()
                    .ToList();

                if (DEKCenterDataGridView.Columns["DEK_BAGLANDIGI_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void DEKCenterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && (DEKCenterDataGridView.Columns[e.ColumnIndex].Name == "DEK_X_KOORDINAT" ||
                DEKCenterDataGridView.Columns[e.ColumnIndex].Name == "DEK_Y_KOORDINAT"))
            {
                object xValue = DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value;
                object yValue = DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value;

                if (double.TryParse(xValue?.ToString(), out double selectedX) &&
                    double.TryParse(yValue?.ToString(), out double selectedY))
                {
                    Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");
                }
            }
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the earth in km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c; // Distance in km
            return distance;
        }

        private void SetupEventHandlers()
        {
            this.FormClosing += DEKCenterPopupForm_FormClosing;
            DEKCenterDataGridView.CellValueChanged += DEKCenterDataGridView_CellValueChanged;
            DEKCenterDataGridView.DataError += DEKCenterDataGridView_DataError;
        }

        private void DEKCenterDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is ArgumentException && DEKCenterDataGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                e.ThrowException = false;
                var comboBoxCell = DEKCenterDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                var comboBoxColumn = DEKCenterDataGridView.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;
                if (comboBoxCell != null && comboBoxColumn != null)
                {
                    comboBoxCell.Value = comboBoxColumn.Items.Count > 0 ? comboBoxColumn.Items[0] : null;
                }
            }
        }

        private void DEKTamamButton_Click(object sender, EventArgs e)
        {
            if (DEKCenterDataGridView == null || DEKCenterDataGridView.Rows.Count == 0)
            {
                MessageBox.Show("DEK Bilgileri Tablosu boş veya başlatılmadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = DEKCenterDataGridView.Rows[0];
            string[] requiredColumns = { "KAYNAK_TIPI", "DEK_KURULU_GUCU", "DEK_X_KOORDINAT", "DEK_Y_KOORDINAT", /*"DEK_DTR_ADI"*/ "DEK_KURULUM_YERI" };
            foreach (string col in requiredColumns)
            {
                if (!DEKCenterDataGridView.Columns.Contains(col) || row.Cells[col].Value == null ||
                    string.IsNullOrWhiteSpace(row.Cells[col].Value.ToString()))
                {
                    MessageBox.Show($"Lütfen tüm alanların doldurulduğundan emin olun, '{col}' dahil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!double.TryParse(row.Cells["DEK_KURULU_GUCU"].Value.ToString(), out double dekKuruluGucu))
            {
                MessageBox.Show("DEK Bilgileri Tablosu'nda DEK Kurulum Gücü (DEK_KURULU_GUCU) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(row.Cells["DEK_X_KOORDINAT"].Value.ToString(), out double dekXKoordinat))
            {
                MessageBox.Show("DEK Bilgileri Tablosu'nda DEK X Koordinatı(DEK_X_KOORDINAT) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(row.Cells["DEK_Y_KOORDINAT"].Value.ToString(), out double dekYKoordinat))
            {
                MessageBox.Show("DEK Bilgileri Tablosu'nda DEK Y Koordinatı(DEK_Y_KOORDINAT) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            veri.CellId = row.Cells["ID"].Value?.ToString();

            DataRow newRow = dataTable.NewRow();
            newRow["KAYNAK_TIPI"] = row.Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = dekKuruluGucu;
            newRow["DEK_X_KOORDINAT"] = dekXKoordinat;
            newRow["DEK_Y_KOORDINAT"] = dekYKoordinat;
            //newRow["DEK_DTR_ADI"] = row.Cells["DEK_DTR_ADI"].Value.ToString();
            newRow["DEK_KURULUM_YERI"] = row.Cells["DEK_KURULUM_YERI"].Value.ToString();

            dataTable.Rows.Add(newRow);

            SaveUpdatedInputFile(dataTable);

            MessageBox.Show("DEK merkezi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isOperationCancelled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveUpdatedInputFile(DataTable updatedData)
        {
            try
            {
                if (DEKCenterDataGridView == null || DEKCenterDataGridView.Rows.Count == 0)
                {
                    MessageBox.Show("DEK Bilgileri Tablosu boş veya başlatılmadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] requiredColumns = { "StartYear", "ID", "DEK_KURULU_GUCU", "DEK_X_KOORDINAT", "DEK_Y_KOORDINAT" };
                foreach (string col in requiredColumns)
                {
                    if (!DEKCenterDataGridView.Columns.Contains(col))
                    {
                        MessageBox.Show($"DEK Bilgileri Tablosu'nda gerekli sütun '{col}' eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                var row = DEKCenterDataGridView.Rows[0];
                string startYearStr = row.Cells["StartYear"]?.Value?.ToString();
                string cellId = row.Cells["ID"]?.Value?.ToString();
                string dekValueStr = row.Cells["DEK_KURULU_GUCU"]?.Value?.ToString();
                // Konvansiyon: X = boylam, Y = enlem (bkz. InitializeDataGridView)
                object boylamValue = row.Cells["DEK_X_KOORDINAT"]?.Value;
                object enlemValue = row.Cells["DEK_Y_KOORDINAT"]?.Value;

                Console.WriteLine($"StartYear: {startYearStr ?? "null"}, ID: {cellId ?? "null"}, DEK_KURULU_GUCU: {dekValueStr ?? "null"}, " +
                                  $"DEK_X_KOORDINAT: {enlemValue?.ToString() ?? "null"}, DEK_Y_KOORDINAT: {boylamValue?.ToString() ?? "null"}");

                if (string.IsNullOrEmpty(startYearStr) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(dekValueStr))
                {
                    MessageBox.Show("Lütfen Başlangıç Yılı(BASLANGIC_YILI), ID ve DEK Kurulum Gücü(DEK_KURULU_GUCU) alanlarının doldurulduğundan emin olun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(startYearStr, out int startYearInt))
                {
                    MessageBox.Show("Başlangıç Yılı geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(dekValueStr, out double dekValue))
                {
                    MessageBox.Show("DEK Bilgileri Tablosu'nda DEK Kurulum Gücü(DEK_KURULU_GUCU) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (boylamValue == null || !double.TryParse(boylamValue.ToString(), out double boylam))
                {
                    MessageBox.Show("DEK Bilgileri Tablosu'nda DEK X Koordinatı(DEK_X_KOORDINAT) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (enlemValue == null || !double.TryParse(enlemValue.ToString(), out double enlem))
                {
                    MessageBox.Show("DEK Bilgileri Tablosu'nda DEK Y Koordinatı(DEK_Y_KOORDINAT) geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (startYearInt > slfEndYear)
                {
                    MessageBox.Show($"Başlangıç Yılı(BASLANGIC_YILI), {slfEndYear} veya daha küçük olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate ana_menu_form_objesi and its properties
                if (ana_menu_form_objesi == null || ana_menu_form_objesi.config == null ||
                    ana_menu_form_objesi.config.Ana_Klasör_Yolu == null ||
                    ana_menu_form_objesi.config.İl == null ||
                    ana_menu_form_objesi.config.İlçe == null ||
                    ana_menu_form_objesi.config.DEK?.dek_klasörü == null ||
                    ana_menu_form_objesi.config.DEK?.cikti_dosyasi == null)
                {
                    MessageBox.Show("Yapılandırma eksik. Lütfen tüm yapılandırma ayarlarının sağlandığından emin olun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string existingFilePath = Path.Combine(ana_menu_form_objesi.userRootPath,
                     (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                     (string)ana_menu_form_objesi.config.İl,
                     (string)ana_menu_form_objesi.config.İlçe,
                     (string)ana_menu_form_objesi.config.DEK.dek_klasörü,
                    (string)ana_menu_form_objesi.config.DEK.cikti_dosyasi).Replace('/', '\\');

                if (!File.Exists(existingFilePath))
                {
                    MessageBox.Show($"Çıktı Excel dosyası şu adreste bulunamadı: {existingFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var workbook = new XLWorkbook(existingFilePath))
                {
                    foreach (int year in Enumerable.Range(startYearInt, slfEndYear - startYearInt + 1))
                    {
                        var worksheet = workbook.Worksheet(year.ToString());
                        if (worksheet == null)
                        {
                            worksheet = workbook.Worksheets.Add(year.ToString());
                            worksheet.Cell("A1").Value = "ID";
                            worksheet.Cell("H1").Value = "DEK_X_KOORDINAT";
                            worksheet.Cell("I1").Value = "DEK_Y_KOORDINAT";
                            worksheet.Cell("G1").Value = "DEK_distributed";
                        }

                        var rows = worksheet.RowsUsed();
                        bool rowUpdated = false;

                        foreach (var excelRow in rows.Skip(1))
                        {
                            string existingId = excelRow.Cell("A").GetString();
                            if (existingId == cellId)
                            {
                                excelRow.Cell("H").Value = boylam;
                                excelRow.Cell("I").Value = enlem;

                                double currentDekValue = excelRow.Cell("G").TryGetValue<double>(out double value) ? value : 0;
                                excelRow.Cell("G").Value = currentDekValue + dekValue;
                                Console.WriteLine($"Updated DEK_distributed to {currentDekValue + dekValue} for ID {cellId}");

                                rowUpdated = true;
                                break;
                            }
                        }

                        if (!rowUpdated)
                        {
                            var lastRow = worksheet.LastRowUsed() ?? worksheet.Row(1);
                            var newRow = worksheet.Row(lastRow.RowNumber() + 1);
                            newRow.Cell("A").Value = cellId;
                            newRow.Cell("H").Value = boylam;
                            newRow.Cell("I").Value = enlem;
                            newRow.Cell("G").Value = dekValue;
                        }
                    }

                    workbook.Save();
                    MessageBox.Show("Veriler ve DEK Dağıtım Değerleri(DEK_distributed) Excel dosyasında başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DEKCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DEKCenterPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOperationCancelled)
            {
                MessageBox.Show("İşlem iptal edildi.");
            }
        }
    }
}