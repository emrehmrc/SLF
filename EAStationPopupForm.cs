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
                typeComboBoxColumn.DataSource = new List<string> { "AC (Home)", "AC (Work)", "AC (Public)", "DC-Fast" };
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
        /*        private void InitializeDataGridView(NoktaVeri veri)
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
                    if (ChargingStationDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearColumn)
                    {
                        List<int> years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
                        startYearColumn.DataSource = years;

                        // Optionally set the default value (here, the first year 2024)
                        ChargingStationDataGridView.Rows[rowIndex].Cells["StartYear"].Value = years.First();
                    }

                    // Set ISTASYON_TIPI options to AC types and DC
                    if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
                    {
                        typeComboBoxColumn.DataSource = new List<string> { "AC (Home)", "AC (Work)", "AC (Public)", "DC-Fast" };
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
                }
        */
        private void EATamamButton_Click(object sender, EventArgs e)
        {
            // Step 1: Ensure all necessary fields are filled
            foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Step 2: Extract coordinates and validate
            if (double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam))
            {
                // Update NoktaVeri with the new values
                noktaVeri.Enlem = enlem;
                noktaVeri.Boylam = boylam;
                noktaVeri.CellId = ChargingStationDataGridView.Rows[0].Cells["ID"].Value?.ToString();

                // Step 3: Add new row to the DataTable
                DataRow newRow = dataTable.NewRow();
      //        newRow["ID"] = ChargingStationDataGridView.Rows[0].Cells["ID"].Value.ToString();
                newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                newRow["ISTASYON_GUCU"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString();
                newRow["EA_X_KOORDINAT"] = enlem;
                newRow["EA_Y_KOORDINAT"] = boylam;
     //         newRow["StartYear"] = ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value.ToString();
                dataTable.Rows.Add(newRow);

                // Step 4: Save updated DataTable to file
                SaveUpdatedInputFile(dataTable);  // Save to input file

                // Step 5: Inform the user about the successful update
                MessageBox.Show("Şarj istasyonu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 6: Proceed with the new simulation (update and run Python script)
                // This will be triggered in the main form when the user clicks the button
                isOperationCancelled = false;

                // Close the form after saving
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lütfen geçerli değerler girin.");
            }
        }

/*    public partial class EAStationPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        private readonly List<string> acPowers = new List<string> {"11 kW", "22 kW" };
        //private readonly List<string> dcPowers = new List<string> {"150 kW"};

        public EAStationPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }
        private void InitializeDataGridView(NoktaVeri veri)
        {
            int rowIndex = ChargingStationDataGridView.Rows.Add();

            // Fill initial coordinates from the provided NoktaVeri instance
            ChargingStationDataGridView.Rows[rowIndex].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
            ChargingStationDataGridView.Rows[rowIndex].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;

            // Use the SelectedCellId property (if set) to populate the "ID" cell.
            // If no cell was selected yet, you could leave it blank or assign a default value.
*//*            ChargingStationDataGridView.Rows[rowIndex].Cells["ID"].Value =
                !string.IsNullOrEmpty(this.SelectedCellId) ? this.SelectedCellId : "Not Selected";*//*

            // Populate the StartYear combobox column with years 2024 to 2035.
            if (ChargingStationDataGridView.Columns["StartYear"] is DataGridViewComboBoxColumn startYearColumn)
            {
                List<int> years = Enumerable.Range(2024, 2035 - 2024 + 1).ToList();
                startYearColumn.DataSource = years;
                // Optionally, set the default value (here, the first year 2024)
                ChargingStationDataGridView.Rows[rowIndex].Cells["StartYear"].Value = years.First();
            }

            // Set ISTASYON_TIPI options to AC types and DC
            if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "AC (Home)", "AC (Work)", "AC (Public)", "DC-Fast" };
            }

            // Set default ISTASYON_GUCU options for AC (Home), AC (Work), AC (Public), and DC-Fast
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
        }
*/

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
                        if (selectedType == "AC (Home)" || selectedType == "AC (Work)")
                        {
                            powerComboBoxColumn.DataSource = new List<string> { "11 kW" };  // Set 11 kW for AC Home and AC Work
                        }
                        else if (selectedType == "AC (Public)")
                        {
                            powerComboBoxColumn.DataSource = new List<string> { "22 kW" };  // Set 22 kW for AC Public
                        }
                        else if (selectedType == "DC-Fast")
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
        /*        private void EATamamButton_Click(object sender, EventArgs e)
                {
                    // Step 1: Ensure all necessary fields are filled
                    foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
                    {
                        if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                        {
                            MessageBox.Show("Lütfen tüm alanları doldurun.");
                            return;
                        }
                    }

                    // Step 2: Extract coordinates and validate
                    if (double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                        double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam))
                    {
                        // Step 3: Add new row to the DataTable
                        DataRow newRow = dataTable.NewRow();
                        newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                        newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                        newRow["ISTASYON_GUCU"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString();
                        newRow["EA_X_KOORDINAT"] = enlem;
                        newRow["EA_Y_KOORDINAT"] = boylam;

                        dataTable.Rows.Add(newRow);

                        // Step 4: Save updated DataTable to file
                        SaveUpdatedInputFile(dataTable);  // Save to input file

                        // Step 5: Inform the user about the successful update
                        MessageBox.Show("Şarj istasyonu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Step 6: Proceed with the new simulation (update and run Python script)
                        // This will be triggered in the main form when the user clicks the button
                        isOperationCancelled = false;

                        // Close the form after saving
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Lütfen geçerli değerler girin.");
                    }
                }*/

        // Save the DataTable to an input file
        private void SaveUpdatedInputFile(DataTable updatedData)
        {
            try
            {
                // Retrieve the start year from the DataGridView
                string startYear = (string)ChargingStationDataGridView.Rows[0].Cells["StartYear"].Value;

                string existingFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\ea\yedek\DELTA_EA_DENEME_IMAR - Copy.xlsx"; // Path to the existing file

                // Load the existing Excel file using EPPlus
                using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(existingFilePath)))
                {
                    // Access the worksheet corresponding to the selected startYear
                    var worksheet = package.Workbook.Worksheets[startYear.ToString()]; // Sheet name is the year

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Worksheet for year {startYear} not found.");
                        return;
                    }

                    // Find the last row with data
                    int lastRow = worksheet.Dimension.End.Row;

                    // Iterate through each row of the DataGridView and update the corresponding cells in the Excel sheet
                    foreach (DataGridViewRow dgvRow in ChargingStationDataGridView.Rows)
                    {
                        // Skip new rows (empty rows)
                        if (dgvRow.IsNewRow) continue;

                        // Get the values from the DataGridView for each cell
                        string cellId = dgvRow.Cells["ID"].Value?.ToString();
                        double? eaXKoordinat = dgvRow.Cells["EA_X_KOORDINAT"].Value as double?;
                        double? eaYKoordinat = dgvRow.Cells["EA_Y_KOORDINAT"].Value as double?;

                        // Ensure the values are not null or invalid
                        if (string.IsNullOrEmpty(cellId) || !eaXKoordinat.HasValue || !eaYKoordinat.HasValue)
                        {
                            MessageBox.Show("Please ensure that all necessary fields are filled.");
                            return;
                        }

                        // Find the corresponding row in the worksheet by matching the "ID" column
                        bool rowUpdated = false;
                        for (int i = 2; i <= lastRow; i++)  // Assuming the data starts from row 2
                        {
                            string existingId = worksheet.Cells[i, 1].Text; // Assuming "ID" is in the first column

                            if (existingId == cellId)
                            {
                                // Update the "EA_X_KOORDINAT" and "EA_Y_KOORDINAT" columns in the existing row
                                worksheet.Cells[i, 2].Value = eaXKoordinat;  // Assuming EA_X_KOORDINAT is in the 2nd column
                                worksheet.Cells[i, 3].Value = eaYKoordinat;  // Assuming EA_Y_KOORDINAT is in the 3rd column

                                rowUpdated = true;
                                break;
                            }
                        }

                        // If no matching row found (i.e., new row), add a new row with the new values
                        if (!rowUpdated)
                        {
                            worksheet.Cells[lastRow + 1, 1].Value = cellId;
                            worksheet.Cells[lastRow + 1, 2].Value = eaXKoordinat;
                            worksheet.Cells[lastRow + 1, 3].Value = eaYKoordinat;

                            lastRow++;  // Increment last row count for the next insertion
                        }
                    }

                    // Save the updated Excel file
                    package.Save();

                    // Inform the user that the file was saved
                    MessageBox.Show("Data saved to the existing Excel file successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }



        /*        private void SaveUpdatedInputFile(DataTable updatedData)
                {
                    try
                    {
                        // Open SaveFileDialog to get the file path for saving the Excel file
                        SaveFileDialog saveFileDialog = new SaveFileDialog
                        {
                            Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                            FileName = "updated_input_file.xlsx" // Default file name
                        };

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filePath = saveFileDialog.FileName;

                            // Using EPPlus to create the Excel file
                            using (var package = new OfficeOpenXml.ExcelPackage())
                            {
                                // Create a worksheet
                                var worksheet = package.Workbook.Worksheets.Add("EA Data");

                                // Add headers (column names)
                                for (int col = 1; col <= updatedData.Columns.Count; col++)
                                {
                                    worksheet.Cells[1, col].Value = updatedData.Columns[col - 1].ColumnName;
                                }

                                // Add the rows from the DataTable
                                for (int row = 0; row < updatedData.Rows.Count; row++)
                                {
                                    for (int col = 0; col < updatedData.Columns.Count; col++)
                                    {
                                        worksheet.Cells[row + 2, col + 1].Value = updatedData.Rows[row][col];
                                    }
                                }

                                // Save the file
                                package.SaveAs(new System.IO.FileInfo(filePath));
                            }

                            MessageBox.Show("Data saved to the Excel file successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving data: {ex.Message}");
                    }
                }*/

        /*        private void EATamamButton_Click(object sender, EventArgs e)
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
                        DataRow newRow = dataTable.NewRow();
                        newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                        newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                        newRow["ISTASYON_GUCU"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString();
                        // newRow["EA_TRAFO_KODU"] = ChargingStationDataGridView.Rows[0].Cells["EA_TRAFO_KODU"].Value.ToString();
                        newRow["EA_X_KOORDINAT"] = enlem;
                        newRow["EA_Y_KOORDINAT"] = boylam;

                        dataTable.Rows.Add(newRow);
                        // Show success message
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
        */
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
