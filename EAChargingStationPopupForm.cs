using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OSGeo.OGR;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class EAChargingStationPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        private readonly List<string> acPowers = new List<string> {"11 kW", "22 kW" };
        private readonly List<string> dcPowers = new List<string> {"150 kW"};

        public EAChargingStationPopupForm(DataTable existingDataTable, NoktaVeri veri)
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

        /*        private void InitializeDataGridView(NoktaVeri veri)
                {
                    // Fill initial coordinates
                    ChargingStationDataGridView.Rows.Add();
                    ChargingStationDataGridView.Rows[0].Cells["EA_X_KOORDINAT"].Value = veri.Enlem;
                    ChargingStationDataGridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam;

                    // Set ISTASYON_TIPI options to AC types and DC
                    if (ChargingStationDataGridView.Columns["ISTASYON_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
                    {
                        typeComboBoxColumn.DataSource = new List<string> { "AC (Home)", "AC (Work)", "AC (Public)", "Fast DC" };
                    }

                    // Default ISTASYON_GUCU to show AC power options
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


        private void EATamamButton_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewCell cell in ChargingStationDataGridView.Rows[0].Cells)
            {
                Console.WriteLine("Button clicked1"); // Log the click event
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
                Console.WriteLine("Button clickedy"); // Log the click event
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
