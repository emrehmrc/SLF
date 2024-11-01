/*using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class ChargingStationPopupForm : Form
    {
        private bool isOperationCancelled = true;  // Tracks if the operation was canceled
        private DataTable dataTable; // Declare dataTable
        private readonly List<string> acPowers = new List<string> { "3,7 kW", "7,4 kW", "11 kW", "22 kW" };
        private readonly List<string> dcPowers = new List<string> { "50 kW", "100 kW", "150 kW", "350 kW" };
        public ChargingStationPopupForm(NoktaVeri veri)
        {
            InitializeComponent();
            InitializeDataGridView(veri); // Pass veri to initialize method
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Initialize the DataTable

            // Set the DataSource for the DataGridView
            ChargingStationDataGridView.DataSource = dataTable;

            // Check if the key exists in the dictionary
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                // Get distinct transformer codes
                List<string> trafoKoduListesi = trafoDataTable
                                                .AsEnumerable()
                                                .Select(row => row["TRAFO_KODU"].ToString())
                                                .Distinct()
                                                .ToList();

                // Set ComboBox column's DataSource
                if (ChargingStationDataGridView.Columns["EA_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                // Inform the user if the key does not exist
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method if the key does not exist
            }
            // Set ISTASYON_TIPI to a ComboBox column with AC and DC as options
            if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "AC", "DC" };
            }

            // Set ISTASYON_GUCU to a ComboBox column
            if (ChargingStationDataGridView.Columns["ISTASYON_GUCU"] is DataGridViewComboBoxColumn powerComboBoxColumn)
            {
                powerComboBoxColumn.DataSource = acPowers; // Default to AC list
            }

            // Add an event handler for ISTASYON_TIPI column changes
            ChargingStationDataGridView.CellValueChanged += ChargingStationDataGridView_CellValueChanged;
            // Populate initial coordinates
            ChargingStationDataGridView.Rows.Add();
            ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
            ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;
        }
        private void ChargingStationDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the changed cell is in ISTASYON_TIPI column
            if (e.ColumnIndex == ChargingStationDataGridView.Columns["ISTASYON_TIPI"].Index)
            {
                string selectedType = ChargingStationDataGridView.Rows[e.RowIndex].Cells["ISTASYON_TIPI"].Value?.ToString();

                if (ChargingStationDataGridView.Columns["ISTASYON_GUCU"] is DataGridViewComboBoxColumn powerComboBoxColumn)
                {
                    // Update the ISTASYON_GUCU options based on the selected ISTASYON_TIPI
                    powerComboBoxColumn.DataSource = selectedType == "AC" ? acPowers : dcPowers;
                }
            }
        }
        private void SetupEventHandlers()
        {
            TamamButton.Click += TamamButton_Click;
            CancelButton.Click += CancelButton_Click;
            this.FormClosing += ChargingStationPopupForm_FormClosing;
        }

        private void TamamButton_Click(object sender, EventArgs e)
        {
            // Check for empty cells
            foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Validate and parse data
            if (double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam))
            {
                DataRow newRow = dataTable.NewRow();
                newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                newRow["ISTASYON_GUCU"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString();
                newRow["EA_TRAFO_KODU"] = ChargingStationDataGridView.Rows[0].Cells["EA_TRAFO_KODU"].Value.ToString();
                newRow["EA_X_KOORDINAT"] = enlem;
                newRow["EA_Y_KOORDINAT"] = boylam;

                dataTable.Rows.Add(newRow);
                isOperationCancelled = false;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lütfen geçerli değerler girin.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
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
}*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class ChargingStationPopupForm : Form
    {
        private bool isOperationCancelled = true;  // Tracks if the operation was canceled
        private DataTable dataTable; // Declare dataTable
        private GirdiModülü girdiModülü;
        private DataTable tableTrial;
        public ChargingStationPopupForm(NoktaVeri veri)
        {
            InitializeComponent();
            InitializeDataGridView(veri); // Pass veri to initialize method
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Initialize the DataTable

            // Set the DataSource for the DataGridView
            ChargingStationDataGridView.DataSource = dataTable;
            tableTrial = GirdiModülü.CurrentDataTable;
            
            // Check if the key exists in the dictionary
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                // Get distinct transformer codes
                List<string> trafoKoduListesi = trafoDataTable
                                                .AsEnumerable()
                                                .Select(row => row["TRAFO_KODU"].ToString())
                                                .Distinct()
                                                .ToList();

                // Set ComboBox column's DataSource
                if (ChargingStationDataGridView.Columns["EA_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                // Inform the user if the key does not exist
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method if the key does not exist
            }

            // Populate initial coordinates
            ChargingStationDataGridView.Rows.Add();
            ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
            ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;
        }

        private void SetupEventHandlers()
        {
            TamamButton.Click += TamamButton_Click;
            CancelButton.Click += CancelButton_Click;
            this.FormClosing += ChargingStationPopupForm_FormClosing;
        }

        private void TamamButton_Click(object sender, EventArgs e)
        {
            // Eğer DataGridView1'in DataSource'u DataTable değilse, yeni bir DataTable oluştur
            //DataTable dataTable = tableTrial.DataSource as DataTable;
            // Check for empty cells
            foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Validate and parse data
            if (double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                double.TryParse(ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam) &&
                int.TryParse(ChargingStationDataGridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString(), out int istasyonGucu))
            {
                DataRow newRow = dataTable.NewRow();
                newRow["ISTASYON_ADI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                newRow["ISTASYON_TIPI"] = ChargingStationDataGridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                newRow["ISTASYON_GUCU"] = istasyonGucu;
                newRow["EA_TRAFO_KODU"] = ChargingStationDataGridView.Rows[0].Cells["EA_TRAFO_KODU"].Value.ToString();
                newRow["EA_X_KOORDINAT"] = enlem;
                newRow["EA_Y_KOORDINAT"] = boylam;

                dataTable.Rows.Add(newRow);
                isOperationCancelled = false;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lütfen geçerli değerler girin.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
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