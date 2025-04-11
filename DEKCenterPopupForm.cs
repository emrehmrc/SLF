using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using ClosedXML.Excel;
using GMap.NET;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class DEKCenterPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        private NoktaVeri veri; // Store the 'veri' object in the class field
                                // Public property to expose NoktaVeri
        public NoktaVeri NoktaVeri => veri;
        public bool OperationCancelled => isOperationCancelled;
        private static class Constants
        {
            public static readonly Dictionary<string, string> DekValueToCountColumn = new Dictionary<string, string>
    {
        { "DEK_KURULU_GUCU", "DEK_distributed" } // Map DEK_KURULU_GUCU to DEK_distributed
    };

            public static readonly List<int> Years = Enumerable.Range(2024, 2030 - 2024 + 1).ToList();
        }

        private string GetCountColumnName(string dekValue)
        {
            return Constants.DekValueToCountColumn.TryGetValue(dekValue, out string columnName) ? columnName : null;
        }
        // Define a dictionary for cities and their coordinates
        private Dictionary<string, PointLatLng> cityCoordinates = new Dictionary<string, PointLatLng>
    {
        { "İzmir", new PointLatLng(38.4192, 27.1287) },
        { "Eskişehir", new PointLatLng(39.7768, 30.5206) },
        // Add more cities and their coordinates as needed
    };

        // Define districts for İzmir and Eskişehir
        private Dictionary<string, List<string>> cityDistricts = new Dictionary<string, List<string>>
    {
        { "İzmir", new List<string> { "Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", "Çiğli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karaburun", "Karşıyaka", "Kemalpaşa", "Kınık", "Kiraz", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı" } },
        { "Eskişehir", new List<string> { "Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "İnönü", "Mahmudiye", "Mihalgazi", "Mihalıççık", "Odunpazarı", "Sarıcakaya", "Seyitgazi", "Sivrihisar", "Tepebaşı" } }
    };

        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;
            this.veri = veri; // Store the 'veri' object in the class field

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }
        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Fill initial coordinates from veri object
            DEKCenterDataGridView.Rows.Add();
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

            // Set the cell (grid) ID using the new CellId property of NoktaVeri.
            // If CellId is not set, default to "Not Selected".
            DEKCenterDataGridView.Rows[0].Cells["ID"].Value =
                !string.IsNullOrEmpty(veri.CellId) ? veri.CellId : "Not Selected";

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

            // Initially populate the city names in the ILCE_ADI ComboBox
            PopulateCityComboBox();
            //  DEKCenterDataGridView.CellValueChanged += DEKCenterDataGridView_CellValueChanged;
        }

        private void PopulateCityComboBox()
        {
            var comboBoxColumn = DEKCenterDataGridView.Columns["ILCE_ADI"] as DataGridViewComboBoxColumn;

            if (comboBoxColumn != null)
            {
                // Clear the existing items in the ComboBox column
                comboBoxColumn.Items.Clear();

                // Add the districts for each city into the ComboBox column
                foreach (var city in cityCoordinates.Keys)
                {
                    if (cityDistricts.ContainsKey(city))
                    {
                        comboBoxColumn.Items.AddRange(cityDistricts[city].ToArray());
                    }
                }

                // Optionally set the first item as the default if needed
                var comboBoxCell = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"] as DataGridViewComboBoxCell;
                if (comboBoxCell != null && comboBoxColumn.Items.Count > 0)
                {
                    // Clear the selection if necessary and update it
                    comboBoxCell.Value = null;
                }
            }
        }
        private void FilterCountiesBasedOnCoordinates(double selectedX, double selectedY)
        {
            var closestCity = cityCoordinates
                              .OrderBy(city => GetDistance(city.Value.Lat, city.Value.Lng, selectedX, selectedY))
                              .FirstOrDefault();

            Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");
            Console.WriteLine($"Closest City: {closestCity.Key}");

            if (closestCity.Key != null)
            {
                var comboBoxColumn = DEKCenterDataGridView.Columns["ILCE_ADI"] as DataGridViewComboBoxColumn;

                if (comboBoxColumn != null)
                {
                    // Get the current cell and its value
                    var comboBoxCell = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"] as DataGridViewComboBoxCell;
                    string currentDistrict = comboBoxCell?.Value?.ToString();

                    // Temporarily disable the CellValueChanged event
                    DEKCenterDataGridView.CellValueChanged -= DEKCenterDataGridView_CellValueChanged;

                    try
                    {
                        // Clear the current value to avoid validation errors
                        if (comboBoxCell != null)
                        {
                            comboBoxCell.Value = null;
                        }

                        // Update the items list
                        comboBoxColumn.Items.Clear();
                        if (cityDistricts.ContainsKey(closestCity.Key))
                        {
                            comboBoxColumn.Items.AddRange(cityDistricts[closestCity.Key].ToArray());
                        }

                        Console.WriteLine($"Added Districts: {string.Join(", ", cityDistricts[closestCity.Key])}");

                        // Restore the current district if it's still valid, otherwise set a default
                        if (comboBoxCell != null)
                        {
                            if (!string.IsNullOrEmpty(currentDistrict) && comboBoxColumn.Items.Contains(currentDistrict))
                            {
                                comboBoxCell.Value = currentDistrict;
                            }
                            else
                            {
                                comboBoxCell.Value = comboBoxColumn.Items.Count > 0 ? comboBoxColumn.Items[0] : null;
                            }
                        }
                    }
                    finally
                    {
                        // Re-enable the CellValueChanged event
                        DEKCenterDataGridView.CellValueChanged += DEKCenterDataGridView_CellValueChanged;
                    }
                }
            }
        }

        private void DEKCenterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            double selectedX = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
            double selectedY = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);

            Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");

            FilterCountiesBasedOnCoordinates(selectedX, selectedY);
        }
        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate the distance between two points on the Earth
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
            DEKCenterDataGridView.DataError += DEKCenterDataGridView_DataError; // Add this line
        }

        private void DEKCenterDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Log the error for debugging
            Console.WriteLine($"DataGridView DataError: Column={e.ColumnIndex}, Row={e.RowIndex}, Exception={e.Exception.Message}");

            // Check if the error is related to a ComboBox cell
            if (e.Exception is ArgumentException && DEKCenterDataGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                // Suppress the default error dialog
                e.ThrowException = false;

                // Optionally, set the cell's value to a valid option
                var comboBoxCell = DEKCenterDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                var comboBoxColumn = DEKCenterDataGridView.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;

                if (comboBoxCell != null && comboBoxColumn != null)
                {
                    // Set the value to the first item in the list, or null if the list is empty
                    comboBoxCell.Value = comboBoxColumn.Items.Count > 0 ? comboBoxColumn.Items[0] : null;
                }
            }
        }
        private void DEKTamamButton_Click(object sender, EventArgs e)
        {
            // Validate the input
            foreach (DataGridViewCell cell in DEKCenterDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Update veri.CellId
            veri.CellId = DEKCenterDataGridView.Rows[0].Cells["ID"].Value?.ToString();

            // Add new row to the existing DataTable
            DataRow newRow = dataTable.NewRow();
            newRow["ILCE_ADI"] = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
            newRow["KAYNAK_TIPI"] = DEKCenterDataGridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
            newRow["DEK_X_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
            newRow["DEK_Y_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);
            newRow["DEK_TM_ADI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_TM_ADI"].Value.ToString();
            newRow["DEK_KURULUM_YERI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_KURULUM_YERI"].Value.ToString();
            // Uncomment if needed: newRow["DEK_BAGLANDIGI_TRAFO_KODU"] = DEKCenterDataGridView.Rows[0].Cells["DEK_BAGLANDIGI_TRAFO_KODU"].Value.ToString();

            dataTable.Rows.Add(newRow);

            // Save to Excel file
            SaveUpdatedInputFile(dataTable);

            // Show success message
            MessageBox.Show("DEK merkezi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isOperationCancelled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void SaveUpdatedInputFile(DataTable updatedData)
        {
            try
            {
                // Retrieve values from the DataGridView
                string startYear = DEKCenterDataGridView.Rows[0].Cells["StartYear"].Value?.ToString();
                string cellId = DEKCenterDataGridView.Rows[0].Cells["ID"].Value?.ToString();
                string dekValueStr = DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value?.ToString();
                double enlem = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
                double boylam = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);

                // Validate input
                if (string.IsNullOrEmpty(startYear) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(dekValueStr))
                {
                    MessageBox.Show("Please ensure StartYear, ID, and DEK_KURULU_GUCU are filled.");
                    return;
                }

                if (!int.TryParse(startYear, out int startYearInt))
                {
                    MessageBox.Show("StartYear must be a valid integer.");
                    return;
                }

                if (!double.TryParse(dekValueStr, out double dekValue))
                {
                    MessageBox.Show("DEK_KURULU_GUCU must be a valid number.");
                    return;
                }

                string countColumnName = "DEK_distributed"; // Hardcoded for now, can be dynamic with Constants

                string existingFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\cıktı\dek_distribution_cumulative_0704.xlsx";

                using (var workbook = new XLWorkbook(existingFilePath))
                {
                    foreach (int year in Enumerable.Range(2024, 2030 - 2024 + 1).Where(y => y >= startYearInt))
                    {
                        Console.WriteLine($"Processing year: {year}");
                        var worksheet = workbook.Worksheet(year.ToString());
                        if (worksheet == null)
                        {
                            worksheet = workbook.Worksheets.Add(year.ToString());
                            worksheet.Cell("A1").Value = "ID";
                            worksheet.Cell("B1").Value = "DEK_X_KOORDINAT";
                            worksheet.Cell("C1").Value = "DEK_Y_KOORDINAT";
                            worksheet.Cell("I1").Value = "DEK_distributed";
                        }

                        var rows = worksheet.RowsUsed();
                        bool rowUpdated = false;

                        // Find the row with the matching CellId
                        foreach (var row in rows.Skip(1)) // Skip header row
                        {
                            string existingId = row.Cell("A").GetString();
                            Console.WriteLine($"Checking row {row.RowNumber()}, ID: {existingId}");
                            if (existingId == cellId)
                            {
                                row.Cell("B").Value = enlem;
                                row.Cell("C").Value = boylam;

                                double currentDekValue = row.Cell("I").TryGetValue<double>(out double value) ? value : 0;
                                row.Cell("I").Value = currentDekValue + dekValue;
                                Console.WriteLine($"Updated DEK_distributed to {currentDekValue + dekValue} for ID {cellId}");

                                rowUpdated = true;
                                break;
                            }
                        }

                        // If no matching row found, add a new row
                        if (!rowUpdated)
                        {
                            var lastRow = worksheet.LastRowUsed() ?? worksheet.Row(1);
                            var newRow = worksheet.Row(lastRow.RowNumber() + 1);
                            newRow.Cell("A").Value = cellId;
                            newRow.Cell("B").Value = enlem;
                            newRow.Cell("C").Value = boylam;
                            newRow.Cell("I").Value = dekValue;
                            Console.WriteLine($"Added new row for ID {cellId} with DEK_distributed {dekValue}");
                        }
                    }

                    workbook.Save();
                    Console.WriteLine("Excel file saved successfully.");
                    MessageBox.Show("Data and DEK_distributed values updated successfully in the Excel file!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                MessageBox.Show($"Error saving data: {ex.Message}");
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