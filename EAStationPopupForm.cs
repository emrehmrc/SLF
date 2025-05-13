using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static SLF.ModülFormu;
using OfficeOpenXml;


namespace SLF
{
    public partial class EAStationPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;

        // Private field to store NoktaVeri
        private NoktaVeri noktaVeri;

        public HomePageForm ana_menu_form_objesi;

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

            public static readonly Dictionary<string, double> StationTypeToLoad = new Dictionary<string, double>
            {
                { "AC (Home)_count", 11 },
                { "AC (Work)_count", 11 },
                { "AC (Public)_count", 22 },
                { "Fast DC_count", 150 }
            };

            public static readonly List<int> Years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
        }

        private string GetCountColumnName(string stationType)
        {
            return Constants.StationTypeToCountColumn.TryGetValue(stationType, out string columnName) ? columnName : null;
        }

        private double GetLoadValue(string stationType)
        {
            return Constants.StationTypeToLoad.TryGetValue(stationType, out double load) ? load : 0;
        }

        private readonly int slfEndYear;

        public EAStationPopupForm(DataTable existingDataTable, NoktaVeri veri, int slfEndYear)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            // Initialize NoktaVeri
            noktaVeri = veri;

            this.slfEndYear = slfEndYear;

            ana_menu_form_objesi = new HomePageForm();
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

            // Initialize StartYear as a ComboBox with valid years
            if (ChargingStationDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearComboBox)
            {
                // Configure the ComboBox column
                startYearComboBox.DataSource = Constants.Years; // List<int> [2024, 2025, ..., 2030]
                startYearComboBox.ValueType = typeof(int); // Ensure the value type is int

                ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value = Constants.Years.Min(); // Set to 2024 initially

                // Log the ComboBox items for debugging
                Console.WriteLine("StartYear ComboBox items: " + string.Join(", ", startYearComboBox.Items.Cast<int>()));
                Console.WriteLine($"StartYear cell value after setting: {ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value}");
            }
            else if (ChargingStationDataGridView.Columns.Contains("StartYear"))
            {
                // Fallback for non-ComboBox column (shouldn't execute in your case)
                ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value = Constants.Years.Min().ToString();
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
        /*        private void SaveUpdatedInputFile(DataTable updatedData)
                {
                    try
                    {
                        // Retrieve values from the DataGridView
                        var row = ChargingStationDataGridView.Rows[0];
                        string startYear = row.Cells["StartYear"].Value?.ToString();
                        string cellId = row.Cells["ID"].Value?.ToString();
                        string stationType = row.Cells["ISTASYON_TIPI"].Value?.ToString();
                        double enlem = Convert.ToDouble(row.Cells["EA_X_KOORDINAT"].Value);
                        double boylam = Convert.ToDouble(row.Cells["EA_Y_KOORDINAT"].Value);

                        if (string.IsNullOrEmpty(startYear) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(stationType))
                        {
                            MessageBox.Show("Please ensure StartYear, ID, and ISTASYON_TIPI are filled.");
                            return;
                        }

                        string countColumnName = GetCountColumnName(stationType);
                        double loadToAdd = GetLoadValue(stationType);
                        if (countColumnName == null || loadToAdd == 0)
                        {
                            MessageBox.Show("Invalid ISTASYON_TIPI selected.");
                            return;
                        }

                        string existingFilePath = Path.Combine(ana_menu_form_objesi.userRootPath,
                            (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                            (string)ana_menu_form_objesi.config.İl,
                            (string)ana_menu_form_objesi.config.İlçe,
                            (string)ana_menu_form_objesi.config.EA.ea_klasörü,
                            (string)ana_menu_form_objesi.config.EA.cikti_dosyasi).Replace('/', '\\');

                        string utilizationPath = Path.Combine(ana_menu_form_objesi.userRootPath,
                            (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                            (string)ana_menu_form_objesi.config.İl,
                            (string)ana_menu_form_objesi.config.İlçe,
                            (string)ana_menu_form_objesi.config.EA.ea_klasörü,
                            (string)ana_menu_form_objesi.config.EA.utilization_path);

                        // Load utilization factors from UtilizasyonFaktoru.xlsx
                        Dictionary<int, double> utilizationFactors = new Dictionary<int, double>();
                        using (var utilizationPackage = new OfficeOpenXml.ExcelPackage(new FileInfo(utilizationPath)))
                        {
                            var utilizationWorksheet = utilizationPackage.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                            if (utilizationWorksheet == null)
                            {
                                MessageBox.Show("Utilization factor file is empty or invalid.");
                                return;
                            }

                            int rowCount = utilizationWorksheet.Dimension?.End.Row ?? 0;
                            for (int i = 2; i <= rowCount; i++) // Start from 2 to skip header
                            {
                                int minSocket = Convert.ToInt32(utilizationWorksheet.Cells[i, 1].Text);
                                int maxSocket = Convert.ToInt32(utilizationWorksheet.Cells[i, 2].Text);
                                double factor = Convert.ToDouble(utilizationWorksheet.Cells[i, 3].Text);
                                for (int socket = minSocket; socket <= maxSocket; socket++)
                                {
                                    utilizationFactors[socket] = factor;
                                }
                            }
                        }

                        using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(existingFilePath)))
                        {
                            int startYearInt = int.Parse(startYear);
                            foreach (int year in Enumerable.Range(startYearInt, slfEndYear - startYearInt + 1))
                            {
                                var worksheet = package.Workbook.Worksheets[year.ToString()];
                                if (worksheet == null)
                                {
                                    // Create a new sheet if it doesn’t exist
                                    worksheet = package.Workbook.Worksheets.Add(year.ToString());
                                    worksheet.Cells[1, 1].Value = "ID";
                                    worksheet.Cells[1, 12].Value = "EA_X_KOORDINAT";
                                    worksheet.Cells[1, 11].Value = "EA_Y_KOORDINAT";
                                    worksheet.Cells[1, 7].Value = "AC (Home)_count";
                                    worksheet.Cells[1, 8].Value = "AC (Work)_count";
                                    worksheet.Cells[1, 9].Value = "AC (Public)_count";
                                    worksheet.Cells[1, 10].Value = "Fast DC_count";
                                    worksheet.Cells[1, 18].Value = "toplam_kapasite";
                                    // Add new yuk columns
                                    worksheet.Cells[1, 14].Value = "AC (Home)_yuk";
                                    worksheet.Cells[1, 15].Value = "AC (Work)_yuk";
                                    worksheet.Cells[1, 16].Value = "AC (Public)_yuk";
                                    worksheet.Cells[1, 17].Value = "Fast DC_yuk";
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

                                        // Update all yuk values based on new counts and utilization factors
                                        UpdateYukValues(worksheet, i, utilizationFactors);

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

                                    // Update all yuk values based on initial counts and utilization factors
                                    UpdateYukValues(worksheet, newRowIndex, utilizationFactors);
                                }
                            }

                            package.Save();
                            MessageBox.Show("Data, counts, toplam_kapasite, and specific yuk columns updated successfully in the Excel file!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving data: {ex.Message}");
                    }
                }

                private void UpdateYukValues(OfficeOpenXml.ExcelWorksheet worksheet, int rowIndex, Dictionary<int, double> utilizationFactors)
                {
                    // Define station types and their column indices
                    var stationTypes = new Dictionary<string, int>
            {
                { "AC (Home)_count", 7 },
                { "AC (Work)_count", 8 },
                { "AC (Public)_count", 9 },
                { "Fast DC_count", 10 }
            };
                    var yukColumns = new Dictionary<string, int>
            {
                { "AC (Home)_yuk", 14 },
                { "AC (Work)_yuk", 15 },
                { "AC (Public)_yuk", 16 },
                { "Fast DC_yuk", 17 }
            };
                    var loadValues = new Dictionary<string, double>
            {
                { "AC (Home)_count", 11 },
                { "AC (Work)_count", 11 },
                { "AC (Public)_count", 22 },
                { "Fast DC_count", 150 }
            };

                    double totalYuk = 0;
                    foreach (var stationType in stationTypes)
                    {
                        int countColumnIndex = stationType.Value;
                        string yukColumnName = stationType.Key.Replace("_count", "_yuk");
                        int yukColumnIndex = yukColumns[yukColumnName];
                        int currentCount = worksheet.Cells[rowIndex, countColumnIndex].Value != null ? Convert.ToInt32(worksheet.Cells[rowIndex, countColumnIndex].Value) : 0;
                        double loadValue = loadValues[stationType.Key];
                        double factor = utilizationFactors.ContainsKey(currentCount) ? utilizationFactors[currentCount] : 1.0; // Default to 1.0 if count is out of range
                        double newYuk = currentCount * loadValue * factor;
                        worksheet.Cells[rowIndex, yukColumnIndex].Value = newYuk;
                        totalYuk += newYuk;
                    }

                    // Update toplam_yuk
                    int totalYukColumnIndex = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                        .FirstOrDefault(c => c.Text == "toplam_kapasite")?.Start.Column ?? 18;
                    worksheet.Cells[rowIndex, totalYukColumnIndex].Value = totalYuk;
                }
        */
        private void SaveUpdatedInputFile(DataTable updatedData)
        {
            try
            {
                // Retrieve values from the DataGridView
                var row = ChargingStationDataGridView.Rows[0];
                string startYear = row.Cells["StartYear"].Value?.ToString();
                string cellId = row.Cells["ID"].Value?.ToString();
                string stationType = row.Cells["ISTASYON_TIPI"].Value?.ToString();
                double enlem = Convert.ToDouble(row.Cells["EA_X_KOORDINAT"].Value);
                double boylam = Convert.ToDouble(row.Cells["EA_Y_KOORDINAT"].Value);

                if (string.IsNullOrEmpty(startYear) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(stationType))
                {
                    MessageBox.Show("Please ensure StartYear, ID, and ISTASYON_TIPI are filled.");
                    return;
                }

                string countColumnName = GetCountColumnName(stationType);
                double loadToAdd = GetLoadValue(stationType);
                if (countColumnName == null || loadToAdd == 0)
                {
                    MessageBox.Show("Invalid ISTASYON_TIPI selected.");
                    return;
                }

                string existingFilePath = Path.Combine(ana_menu_form_objesi.userRootPath,
                    (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)ana_menu_form_objesi.config.İl,
                    (string)ana_menu_form_objesi.config.İlçe,
                    (string)ana_menu_form_objesi.config.EA.ea_klasörü,
                    (string)ana_menu_form_objesi.config.EA.cikti_dosyasi);

                string utilizationPath = Path.Combine(ana_menu_form_objesi.userRootPath,
                    (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)ana_menu_form_objesi.config.İl,
                    (string)ana_menu_form_objesi.config.İlçe,
                    (string)ana_menu_form_objesi.config.EA.ea_klasörü,
                    (string)ana_menu_form_objesi.config.EA.utilization_path);

                // Load utilization factors from UtilizasyonFaktoru.xlsx
                Dictionary<int, double> utilizationFactors = new Dictionary<int, double>();
                using (var utilizationPackage = new OfficeOpenXml.ExcelPackage(new FileInfo(utilizationPath)))
                {
                    var utilizationWorksheet = utilizationPackage.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                    if (utilizationWorksheet == null)
                    {
                        MessageBox.Show("Utilization factor file is empty or invalid.");
                        return;
                    }

                    int rowCount = utilizationWorksheet.Dimension?.End.Row ?? 0;
                    for (int i = 2; i <= rowCount; i++) // Start from 2 to skip header
                    {
                        int minSocket = Convert.ToInt32(utilizationWorksheet.Cells[i, 1].Text);
                        int maxSocket = Convert.ToInt32(utilizationWorksheet.Cells[i, 2].Text);
                        double factor = Convert.ToDouble(utilizationWorksheet.Cells[i, 3].Text);
                        for (int socket = minSocket; socket <= maxSocket; socket++)
                        {
                            utilizationFactors[socket] = factor;
                        }
                    }
                }

                using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(existingFilePath)))
                {
                    int startYearInt = int.Parse(startYear);
                    foreach (int year in Enumerable.Range(startYearInt, slfEndYear - startYearInt + 1))
                    {
                        var worksheet = package.Workbook.Worksheets[year.ToString()];
                        if (worksheet == null)
                        {
                            // Create a new sheet if it doesn’t exist
                            worksheet = package.Workbook.Worksheets.Add(year.ToString());
                            worksheet.Cells[1, 1].Value = "ID";
                            worksheet.Cells[1, 12].Value = "EA_X_KOORDINAT";
                            worksheet.Cells[1, 11].Value = "EA_Y_KOORDINAT";
                            worksheet.Cells[1, 7].Value = "AC (Home)_count";
                            worksheet.Cells[1, 8].Value = "AC (Work)_count";
                            worksheet.Cells[1, 9].Value = "AC (Public)_count";
                            worksheet.Cells[1, 10].Value = "Fast DC_count";
                            worksheet.Cells[1, 18].Value = "toplam_yuk";
                            worksheet.Cells[1, 13].Value = "toplam_kapasite"; // New column for original calculation
                                                                              // Add new yuk columns
                            worksheet.Cells[1, 14].Value = "AC (Home)_yuk";
                            worksheet.Cells[1, 15].Value = "AC (Work)_yuk";
                            worksheet.Cells[1, 16].Value = "AC (Public)_yuk";
                            worksheet.Cells[1, 17].Value = "Fast DC_yuk";
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

                                // Update all yuk values based on new counts and utilization factors
                                UpdateYukValues(worksheet, i, utilizationFactors);

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

                            // Update all yuk values based on initial counts and utilization factors
                            UpdateYukValues(worksheet, newRowIndex, utilizationFactors);
                        }
                    }

                    package.Save();
                    MessageBox.Show("Data, counts, toplam_kapasite, toplam_yuk, and specific yuk columns updated successfully in the Excel file!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void UpdateYukValues(OfficeOpenXml.ExcelWorksheet worksheet, int rowIndex, Dictionary<int, double> utilizationFactors)
        {
            // Define station types and their column indices
            var stationTypes = new Dictionary<string, int>
    {
        { "AC (Home)_count", 7 },
        { "AC (Work)_count", 8 },
        { "AC (Public)_count", 9 },
        { "Fast DC_count", 10 }
    };
            var yukColumns = new Dictionary<string, int>
    {
        { "AC (Home)_yuk", 14 },
        { "AC (Work)_yuk", 15 },
        { "AC (Public)_yuk", 16 },
        { "Fast DC_yuk", 17 }
    };
            var loadValues = new Dictionary<string, double>
    {
        { "AC (Home)_count", 11 },
        { "AC (Work)_count", 11 },
        { "AC (Public)_count", 22 },
        { "Fast DC_count", 150 }
    };

            double totalYuk = 0; // Will be used for toplam_yuk (sum of count * power)
            double totalKapasite = 0; // Will be used for toplam_kapasite (sum of yuk with factors)

            foreach (var stationType in stationTypes)
            {
                int countColumnIndex = stationType.Value;
                string yukColumnName = stationType.Key.Replace("_count", "_yuk");
                int yukColumnIndex = yukColumns[yukColumnName];
                int currentCount = worksheet.Cells[rowIndex, countColumnIndex].Value != null ? Convert.ToInt32(worksheet.Cells[rowIndex, countColumnIndex].Value) : 0;
                double loadValue = loadValues[stationType.Key];
                double factor = utilizationFactors.ContainsKey(currentCount) ? utilizationFactors[currentCount] : 1.0; // Default to 1.0 if count is out of range
                double newYuk = currentCount * loadValue * factor;
                worksheet.Cells[rowIndex, yukColumnIndex].Value = newYuk;
                totalKapasite += newYuk; // Sum for toplam_kapasite
                totalYuk += currentCount * loadValue; // Sum for toplam_yuk (without factor)
            }

            // Update toplam_kapasite
            int toplamKapasiteColumnIndex = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .FirstOrDefault(c => c.Text == "toplam_yuk")?.Start.Column ?? 18;
            worksheet.Cells[rowIndex, toplamKapasiteColumnIndex].Value = totalKapasite;

            // Update toplam_yuk
            int toplamYukColumnIndex = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .FirstOrDefault(c => c.Text == "toplam_kapasite")?.Start.Column ?? 13;
            worksheet.Cells[rowIndex, toplamYukColumnIndex].Value = totalYuk;
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