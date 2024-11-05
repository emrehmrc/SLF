using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class DEKCenterPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Fill initial coordinates
            DEKCenterDataGridView.Rows.Add();
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

            // Set ISTASYON_TIPI options for DEK types
            if (DEKCenterDataGridView.Columns["KAYNAK_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "GES (Güneş)", "RES (Rüzgar)", "BES (Biokütle)" }; // Adjust types as needed
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

        private void SetupEventHandlers()
        {
            DEKTamamButton.Click += DEKTamamButton_Click;
            DEKCancelButton.Click += DEKCancelButton_Click;
            this.FormClosing += DEKCenterPopupForm_FormClosing;
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

            // Add new row to the existing DataTable
            DataRow newRow = dataTable.NewRow();
            newRow["ILCE_ADI"] = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
            newRow["KAYNAK_TIPI"] = DEKCenterDataGridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
            newRow["DEK_X_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
            newRow["DEK_Y_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);
            newRow["DEK_TM_ADI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_TM_ADI"].Value.ToString();
            newRow["DEK_KURULUM_YERI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_KURULUM_YERI"].Value.ToString();
            newRow["DEK_BAGLANDIGI_TRAFO_KODU"] = DEKCenterDataGridView.Rows[0].Cells["DEK_BAGLANDIGI_TRAFO_KODU"].Value.ToString();

            dataTable.Rows.Add(newRow);

            // Show success message
            MessageBox.Show("DEK merkezi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isOperationCancelled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
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
