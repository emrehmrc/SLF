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

        public NoktaVeri NoktaVeri => veri;

        public HomePageForm ana_menu_form_objesi;
        private MethodForm methodFormObjesi;
        private BekleForm bekleForm;

        public string userRootPath;
        public string configPath;
        public bool OperationCancelled => isOperationCancelled;

        private static class Constants
        {
            public static readonly Dictionary<string, string> DekValueToCountColumn = new Dictionary<string, string>
        {
            { "DEK_KURULU_GUCU", "DEK_distributed" }
        };

            public static readonly List<int> Years = Enumerable.Range(2024, 2030 - 2024 + 1).ToList();
        }

        private string GetCountColumnName(string dekValue)
        {
            return Constants.DekValueToCountColumn.TryGetValue(dekValue, out string columnName) ? columnName : null;
        }

        private readonly int slfEndYear;
        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri, int slfEndYear, HomePageForm anaMenuForm)
        {
            InitializeComponent();
            dataTable = existingDataTable;
            this.veri = veri;
            this.slfEndYear = slfEndYear;
            
            ana_menu_form_objesi = new HomePageForm();
            methodFormObjesi = new MethodForm(ana_menu_form_objesi);
            this.ana_menu_form_objesi = anaMenuForm;
            bekleForm = new BekleForm();

            userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            configPath = Path.Combine(((string)ana_menu_form_objesi.projectRoot).Replace('/', '\\'), "config.json");

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }
        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Log veri for debugging
            Console.WriteLine($"veri.Enlem: {veri.Enlem}, veri.Boylam: {veri.Boylam}, veri.CellId: {veri.CellId ?? "null"}");

            // Fill initial coordinates from veri object
            DEKCenterDataGridView.Rows.Add();
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

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

                // Log the ComboBox items for debugging
                Console.WriteLine("StartYear ComboBox items: " + string.Join(", ", startYearComboBox.Items.Cast<int>()));
                Console.WriteLine($"StartYear cell value after setting: {DEKCenterDataGridView.Rows[0].Cells["StartYear"].Value}");
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

            // Log DataGridView columns
            Console.WriteLine("DataGridView columns: " + string.Join(", ", DEKCenterDataGridView.Columns.Cast<DataGridViewColumn>().Select(c => c.Name)));
        }
        /*        private void InitializeDataGridView(NoktaVeri veri)
                {
                    // Log veri for debugging
                    Console.WriteLine($"veri.Enlem: {veri.Enlem}, veri.Boylam: {veri.Boylam}, veri.CellId: {veri.CellId ?? "null"}");

                    // Fill initial coordinates from veri object
                    DEKCenterDataGridView.Rows.Add();
                    DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
                    DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

                    // Set the cell (grid) ID
                    DEKCenterDataGridView.Rows[0].Cells["ID"].Value =
                        !string.IsNullOrEmpty(veri.CellId) ? veri.CellId : "Not Selected";

                    // Initialize StartYear as a ComboBox with valid years
                    if (DEKCenterDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearComboBox)
                    {
                        startYearComboBox.DataSource = Constants.Years;
                        DEKCenterDataGridView.Rows[0].Cells["StartYear"].Value = Constants.Years.Min();
                    }
                    else if (DEKCenterDataGridView.Columns.Contains("StartYear"))
                    {
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

                    // Log DataGridView columns
                    Console.WriteLine("DataGridView columns: " + string.Join(", ", DEKCenterDataGridView.Columns.Cast<DataGridViewColumn>().Select(c => c.Name)));
                }*/

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
            Console.WriteLine($"DataGridView DataError: Column={e.ColumnIndex}, Row={e.RowIndex}, Exception={e.Exception.Message}");
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
                MessageBox.Show("DataGridView is empty or not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = DEKCenterDataGridView.Rows[0];
            string[] requiredColumns = { "KAYNAK_TIPI", "DEK_KURULU_GUCU", "DEK_X_KOORDINAT", "DEK_Y_KOORDINAT", "DEK_DTR_ADI", "DEK_KURULUM_YERI" };
            foreach (string col in requiredColumns)
            {
                if (!DEKCenterDataGridView.Columns.Contains(col) || row.Cells[col].Value == null ||
                    string.IsNullOrWhiteSpace(row.Cells[col].Value.ToString()))
                {
                    MessageBox.Show($"Please ensure all fields are filled, including '{col}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!double.TryParse(row.Cells["DEK_KURULU_GUCU"].Value.ToString(), out double dekKuruluGucu))
            {
                MessageBox.Show("DEK_KURULU_GUCU must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(row.Cells["DEK_X_KOORDINAT"].Value.ToString(), out double dekXKoordinat))
            {
                MessageBox.Show("DEK_X_KOORDINAT must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(row.Cells["DEK_Y_KOORDINAT"].Value.ToString(), out double dekYKoordinat))
            {
                MessageBox.Show("DEK_Y_KOORDINAT must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            veri.CellId = row.Cells["ID"].Value?.ToString();

            DataRow newRow = dataTable.NewRow();
            newRow["KAYNAK_TIPI"] = row.Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = dekKuruluGucu;
            newRow["DEK_X_KOORDINAT"] = dekXKoordinat;
            newRow["DEK_Y_KOORDINAT"] = dekYKoordinat;
            newRow["DEK_DTR_ADI"] = row.Cells["DEK_DTR_ADI"].Value.ToString();
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
                    MessageBox.Show("DataGridView is empty or not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] requiredColumns = { "StartYear", "ID", "DEK_KURULU_GUCU", "DEK_X_KOORDINAT", "DEK_Y_KOORDINAT" };
                foreach (string col in requiredColumns)
                {
                    if (!DEKCenterDataGridView.Columns.Contains(col))
                    {
                        MessageBox.Show($"Required column '{col}' is missing in DataGridView.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                var row = DEKCenterDataGridView.Rows[0];
                string startYearStr = row.Cells["StartYear"]?.Value?.ToString();
                string cellId = row.Cells["ID"]?.Value?.ToString();
                string dekValueStr = row.Cells["DEK_KURULU_GUCU"]?.Value?.ToString();
                object enlemValue = row.Cells["DEK_X_KOORDINAT"]?.Value;
                object boylamValue = row.Cells["DEK_Y_KOORDINAT"]?.Value;

                Console.WriteLine($"StartYear: {startYearStr ?? "null"}, ID: {cellId ?? "null"}, DEK_KURULU_GUCU: {dekValueStr ?? "null"}, " +
                                  $"DEK_X_KOORDINAT: {enlemValue?.ToString() ?? "null"}, DEK_Y_KOORDINAT: {boylamValue?.ToString() ?? "null"}");

                if (string.IsNullOrEmpty(startYearStr) || string.IsNullOrEmpty(cellId) || string.IsNullOrEmpty(dekValueStr))
                {
                    MessageBox.Show("Please ensure StartYear, ID, and DEK_KURULU_GUCU are filled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(startYearStr, out int startYearInt))
                {
                    MessageBox.Show("StartYear must be a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(dekValueStr, out double dekValue))
                {
                    MessageBox.Show("DEK_KURULU_GUCU must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (enlemValue == null || !double.TryParse(enlemValue.ToString(), out double enlem))
                {
                    MessageBox.Show("DEK_X_KOORDINAT must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (boylamValue == null || !double.TryParse(boylamValue.ToString(), out double boylam))
                {
                    MessageBox.Show("DEK_Y_KOORDINAT must be a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (startYearInt > slfEndYear)
                {
                    MessageBox.Show($"StartYear must be less than or equal to {slfEndYear}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string countColumnName = "DEK_distributed";

                // Validate ana_menu_form_objesi and its properties
                if (ana_menu_form_objesi == null || ana_menu_form_objesi.config == null ||
                    ana_menu_form_objesi.config.Ana_Klasör_Yolu == null ||
                    ana_menu_form_objesi.config.İl == null ||
                    ana_menu_form_objesi.config.İlçe == null ||
                    ana_menu_form_objesi.config.DEK?.dek_klasörü == null ||
                    ana_menu_form_objesi.config.DEK?.cikti_dosyasi == null)
                {
                    MessageBox.Show("Configuration is incomplete. Please ensure all configuration settings are provided.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string existingFilePath = Path.Combine(userRootPath,
                     (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                     (string)ana_menu_form_objesi.config.İl,
                     (string)ana_menu_form_objesi.config.İlçe,
                     (string)ana_menu_form_objesi.config.DEK.dek_klasörü,
                    (string)ana_menu_form_objesi.config.DEK.cikti_dosyasi).Replace('/', '\\');

                if (!File.Exists(existingFilePath))
                {
                    MessageBox.Show($"Output Excel file not found at: {existingFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var workbook = new XLWorkbook(existingFilePath))
                {
                    foreach (int year in Enumerable.Range(startYearInt, slfEndYear - startYearInt + 1))
                    {
                        Console.WriteLine($"Processing year: {year}");
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
                            Console.WriteLine($"Checking row {excelRow.RowNumber()}, ID: {existingId}");
                            if (existingId == cellId)
                            {
                                excelRow.Cell("H").Value = enlem;
                                excelRow.Cell("I").Value = boylam;

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
                            newRow.Cell("H").Value = enlem;
                            newRow.Cell("I").Value = boylam;
                            newRow.Cell("G").Value = dekValue;
                            Console.WriteLine($"Added new row for ID {cellId} with DEK_distributed {dekValue}");
                        }
                    }

                    workbook.Save();
                    Console.WriteLine("Excel file saved successfully.");
                    MessageBox.Show("Data and DEK_distributed values updated successfully in the Excel file!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    /*       public NoktaVeri NoktaVeri => veri;

           public HomePageForm ana_menu_form_objesi;
           private MethodForm methodFormObjesi;
           private BekleForm bekleForm;

           public string userRootPath;
           public string configPath;
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
               //PopulateCityComboBox();
               //  DEKCenterDataGridView.CellValueChanged += DEKCenterDataGridView_CellValueChanged;
           }



           private void DEKCenterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
           {
               double selectedX = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
               double selectedY = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);

               Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");

            //   FilterCountiesBasedOnCoordinates(selectedX, selectedY);
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
   *//*                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                   {
                       MessageBox.Show("Lütfen tüm alanları doldurun.");
                       return;
                   }*//*
               }

               // Update veri.CellId
               veri.CellId = DEKCenterDataGridView.Rows[0].Cells["ID"].Value?.ToString();

               // Add new row to the existing DataTable
               DataRow newRow = dataTable.NewRow();
             //  newRow["ILCE_ADI"] = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
               newRow["KAYNAK_TIPI"] = DEKCenterDataGridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
               newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
               newRow["DEK_X_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
               newRow["DEK_Y_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);
               newRow["DEK_DTR_ADI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_DTR_ADI"].Value.ToString();
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

                   //string existingFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\cıktı\dek_distribution_cumulative_0704.xlsx";

                   string existingFilePath = Path.Combine(userRootPath,
       (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
       (string)ana_menu_form_objesi.config.İl,
       (string)ana_menu_form_objesi.config.İlçe,
       (string)ana_menu_form_objesi.config.DEK.dek_klasörü,
       (string)ana_menu_form_objesi.config.DEK.cikti_dosyasi);

                   using (var workbook = new XLWorkbook(existingFilePath))
                   {
                       foreach (int year in Enumerable.Range(2024, 2035 - 2024 + 1).Where(y => y >= startYearInt))
                       {
                           Console.WriteLine($"Processing year: {year}");
                           var worksheet = workbook.Worksheet(year.ToString());
                           if (worksheet == null)
                           {
                               worksheet = workbook.Worksheets.Add(year.ToString());
                               worksheet.Cell("A1").Value = "ID";
                               worksheet.Cell("H1").Value = "DEK_X_KOORDINAT";
                               //worksheet.Cell("H1").Value = "x_koordinat";
                               worksheet.Cell("I1").Value = "DEK_Y_KOORDINAT";
                              // worksheet.Cell("I1").Value = "y_koordinat";
                               worksheet.Cell("G1").Value = "DEK_distributed";
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
                                   row.Cell("H").Value = enlem;
                                   row.Cell("I").Value = boylam;

                                   double currentDekValue = row.Cell("G").TryGetValue<double>(out double value) ? value : 0;
                                   row.Cell("G").Value = currentDekValue + dekValue;
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
                               newRow.Cell("H").Value = enlem;
                               newRow.Cell("I").Value = boylam;
                               newRow.Cell("G").Value = dekValue;
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
       }*/
}