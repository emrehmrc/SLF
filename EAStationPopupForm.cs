using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            // Fill initial coordinates
            ChargingStationDataGridView.Rows.Add();
            ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
            ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;

            // Set ISTASYON_TIPI options to AC types and DC
            if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "AC (Home)", "AC (Work)", "AC (Public)", "DC-Fast" };
            }

            // Set default ISTASYON_GUCU options for AC (Home), AC (Work), AC (Public), and DC-Fast
            if (ChargingStationDataGridView.Columns["ISTASYON_GUCU"] is DataGridViewComboBoxColumn powerComboBoxColumn)
            {
                // Initialize with AC power options
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
        }

        // Save the DataTable to an input file
        private void SaveUpdatedInputFile(DataTable updatedData)
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
        }

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
