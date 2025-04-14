using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClosedXML.Excel;
using OSGeo.OGR;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class EAStationPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        // Private field to store NoktaVeri
        private NoktaVeri noktaVeri;

        // Public property to expose NoktaVeri
        public NoktaVeri NoktaVeri => noktaVeri;

        private readonly List<string> acPowers = new List<string> { "11 kW", "22 kW" };
        private static class Constants
        {
            public static readonly Dictionary<string, string> StationTypeToCountColumn = new Dictionary<string, string>
    {
        { "AC (Home)_count", "AC (Home)_count" },
        { "AC (Work)_count", "AC (Work)_count" },
        { "AC (Public)_count", "AC (Public)_count" },
        { "Fast DC_count", "Fast DC_count" }
    };

            public static readonly List<int> Years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
        }

        private string GetCountColumnName(string stationType)
        {
            return Constants.StationTypeToCountColumn.TryGetValue(stationType, out string columnName) ? columnName : null;
        }
        public EAStationPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            // Initialize NoktaVeri
            noktaVeri = veri;

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }
        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Add a new row to the DataGridView and capture its index
            int rowIndex = ChargingStationDataGridView.Rows.Add();

            // Fill initial coordinates from the provided NoktaVeri instance
            ChargingStationDataGridView.Rows[rowIndex].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
            ChargingStationDataGridView.Rows[rowIndex].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;

            // Set the cell (grid) ID using the new CellId property of NoktaVeri.
            // If CellId is not set, default to "Not Selected".
            ChargingStationDataGridView.Rows[rowIndex].Cells["ID"].Value =
                !string.IsNullOrEmpty(veri.CellId) ? veri.CellId : "Not Selected";

            // Populate the StartYear combobox column with years 2024 to 2035.
            /*            if (ChargingStationDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearColumn)
                        {
                            List<int> years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
                            startYearColumn.DataSource = years;

                            // Optionally set the default value (here, the first year 2024)
                            ChargingStationDataGridView.Rows[rowIndex].Cells["StartYear"].Value = years.First();
                        }*/

            // Set ISTASYON_TIPI options to AC types and DC
            if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "AC (Home)_count", "AC (Work)_count", "AC (Public)_count", "Fast DC_count" };
            }

            // Set default ISTASYON_GUCU options for the charging station power
            if (ChargingStationDataGridView.Columns["ISTASYON_GUCU"] is DataGridViewComboBoxColumn powerComboBoxColumn)
            {
                powerComboBoxColumn.DataSource = acPowers;
            }

            // Populate transformer codes if available
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                    .Select(row => row["TRAFO_KODU"].ToString())
                    .Distinct()
                    .ToList();

                if (ChargingStationDataGridView.Columns["EA_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Event handler to handle changes in StartYear column
            ChargingStationDataGridView.CellValueChanged += ChargingStationDataGridView_CellValueChanged;
        }
        private void EATamamButton_Click(object sender, EventArgs e)
        {
            // Set the cursor to a wait cursor.
            this.Cursor = Cursors.WaitCursor;

            try
            {
                foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        MessageBox.Show("Lütfen tüm alanları doldurun.");
                        return;
                    }
                }

                if (double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                    double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam))
                {
                    noktaVeri.Enlem = enlem;
                    noktaVeri.Boylam = boylam;
                    noktaVeri.CellId = ChargingStationDataGridView.Rows[0].Cells["ID"].Value?.ToString();

                    DataRow newRow = dataTable.NewRow();
                    newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                    newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                    newRow["ISTASYON_GUCU"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString();
                    newRow["EA_X_KOORDINAT"] = enlem;
                    newRow["EA_Y_KOORDINAT"] = boylam;
                    dataTable.Rows.Add(newRow);

                    SaveUpdatedInputFile(dataTable);

                    MessageBox.Show("Şarj istasyonu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isOperationCancelled = false;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli değerler girin.");
                }
            }
            finally
            {
                // Always reset the cursor to default.
                this.Cursor = Cursors.Default;
            }
        }
        private void SetupEventHandlers()
        {
            this.FormClosing += ChargingStationPopupForm_FormClosing;
            ChargingStationDataGridView.CellValueChanged += ChargingStationDataGridView_CellValueChanged;
        }

        private void ChargingStationDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ChargingStationDataGridView.Columns["ISTASYON_TIPI"].Index)
            {
                string selectedType = ChargingStationDataGridView.Rows[e.RowIndex].Cells["ISTASYON_TIPI"].Value?.ToString();

                // If the type is AC, set ISTASYON_GUCU to AC power options
                if (ChargingStationDataGridView.Columns["ISTASYON_GUCU"] is DataGridViewComboBoxColumn powerComboBoxColumn)
                {
                    if (selectedType != null)
                    {
                        if (selectedType == "AC (Home)_count" || selectedType == "AC (Work)_count")
                        {
                            powerComboBoxColumn.DataSource = new List<string> { "11 kW" };  // Set 11 kW for AC Home and AC Work
                        }
                        else if (selectedType == "AC (Public)_count")
                        {
                            powerComboBoxColumn.DataSource = new List<string> { "22 kW" };  // Set 22 kW for AC Public
                        }
                        else if (selectedType == "Fast DC_count")
                        {
                            powerComboBoxColumn.DataSource = new List<string> { "150 kW" };  // Set 150 kW for Fast DC
                        }
                        else
                        {
                            powerComboBoxColumn.DataSource = new List<string>();  // Clear options if none match
                        }
                    }
                }
            }

        }
        private void SaveUpdatedInputFile(DataTable updatedData)
        {
            try
            {
                // Retrieve values from the DataGridView
                string startYear = ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value?.ToString();
                string cellId = ChargingStationDataGridView.Rows[0].Cells["ID"].Value?.ToString();
                string stationType = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value?.ToString();
                double enlem = Convert.ToDouble(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value);
                double boylam = Convert.ToDouble(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value);

                if (string.IsNullOrEmpty(startYear) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(stationType))
                {
                    MessageBox.Show("Please ensure StartYear, ID, and ISTASYON_TIPI are filled.");
                    return;
                }

                string countColumnName = GetCountColumnName(stationType);
                if (countColumnName == null)
                {
                    MessageBox.Show("Invalid ISTASYON_TIPI selected.");
                    return;
                }

                string existingFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V3\ÇIKTI\evcs_monte_carlo_distribution_kumulatif_0411.xlsx";

                using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(existingFilePath)))
                {
                    int startYearInt = int.Parse(startYear);
                    foreach (int year in Constants.Years.Where(y => y >= startYearInt))
                    {
                        var worksheet = package.Workbook.Worksheets[year.ToString()];
                        if (worksheet == null)
                        {
                            // Optionally create a new sheet if it doesn’t exist
                            worksheet = package.Workbook.Worksheets.Add(year.ToString());
                            worksheet.Cells[1, 1].Value = "ID";
                            worksheet.Cells[1, 12].Value = "EA_X_KOORDINAT";
                            worksheet.Cells[1, 11].Value = "EA_Y_KOORDINAT";
                            worksheet.Cells[1, 7].Value = "AC (Home)_count";
                            worksheet.Cells[1, 8].Value = "AC (Work)_count";
                            worksheet.Cells[1, 9].Value = "AC (Public)_count";
                            worksheet.Cells[1, 10].Value = "Fast DC_count";
                        }

                        int lastRow = worksheet.Dimension?.End.Row ?? 1;
                        bool rowUpdated = false;

                        // Find the row with the matching CellId
                        for (int i = 2; i <= lastRow; i++)
                        {
                            string existingId = worksheet.Cells[i, 1].Text;
                            if (existingId == cellId)
                            {
                                // Update coordinates
                                worksheet.Cells[i, 12].Value = enlem;
                                worksheet.Cells[i, 11].Value = boylam;

                                // Increment the count for the selected station type
                                int columnIndex = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                                    .FirstOrDefault(c => c.Text == countColumnName)?.Start.Column ?? 0;
                                if (columnIndex > 0)
                                {
                                    int currentCount = worksheet.Cells[i, columnIndex].Value != null ? Convert.ToInt32(worksheet.Cells[i, columnIndex].Value) : 0;
                                    worksheet.Cells[i, columnIndex].Value = currentCount + 1;
                                }

                                rowUpdated = true;
                                break;
                            }
                        }

                        // If no matching row found, add a new row
                        if (!rowUpdated)
                        {
                            int newRowIndex = lastRow + 1;
                            worksheet.Cells[newRowIndex, 1].Value = cellId;
                            worksheet.Cells[newRowIndex, 12].Value = enlem;
                            worksheet.Cells[newRowIndex, 11].Value = boylam;

                            // Set initial counts (1 for the selected type, 0 for others)
                            worksheet.Cells[newRowIndex, 7].Value = stationType == "AC (Home)_count" ? 1 : 0;
                            worksheet.Cells[newRowIndex, 8].Value = stationType == "AC (Work)_count" ? 1 : 0;
                            worksheet.Cells[newRowIndex, 9].Value = stationType == "AC (Public)_count" ? 1 : 0;
                            worksheet.Cells[newRowIndex, 10].Value = stationType == "Fast DC_count" ? 1 : 0;
                        }
                    }

                    package.Save();
                    MessageBox.Show("Data and counts updated successfully in the Excel file!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }
        private void EACancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChargingStationPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOperationCancelled)
            {
                MessageBox.Show("İşlem iptal edildi.");
            }
        }
    }
}