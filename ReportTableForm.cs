using Irony;
using SLF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SLF
{
    public partial class ReportTableForm : Form
    {
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;


        public HomePageForm ana_menu_form_objesi;

        public int slfStartYear = 0, slfEndYear = 0;
        private readonly string dataSource; // "EA" or "DEK"


        public ReportTableForm(string dataSource, HomePageForm anaMenuForm)
        {
            InitializeComponent();
            this.dataSource = dataSource; // Store the data source ("EA" or "DEK")
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            ana_menu_form_objesi = new HomePageForm();
            this.ana_menu_form_objesi = anaMenuForm;


            var yearService = YearService.GetInstance();
            if (this.slfStartYear > 0 && this.slfEndYear > 0)
            {
                yearService.SetYears(this.slfStartYear, this.slfEndYear);
            }
            else
            {
                this.slfStartYear = yearService.slfStartYear;
                this.slfEndYear = yearService.slfEndYear;
            }

            // Adjust year range based on data source
            if (dataSource == "EA")
            {
                // EA uses years 2024–2035
                slfStartYear = Math.Max(slfStartYear, 2024);
                slfEndYear = Math.Min(slfEndYear, 2035);
            }
            else if (dataSource == "DEK")
            {
                // DEK uses years 2024–2030
                slfStartYear = Math.Max(slfStartYear, 2024);
                slfEndYear = Math.Min(slfEndYear, 2035);
            }

            var yearList = new List<int>();
            for (int year = slfStartYear; year <= slfEndYear; year++)
            {
                yearList.Add(year);
            }
            comboBox_report_yıl_secimi.DataSource = yearList; // Rapor yılı seçimi için ComboBox
            comboBox_report_yıl_secimi.SelectedIndexChanged += ComboBox_report_yıl_secimi_SelectedIndexChanged;

            // Load data for the initial selected year
            if (yearList.Any())
            {
                LoadExcelData(yearList.First().ToString());
            }
        }

        private void ComboBox_report_yıl_secimi_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedYear = comboBox_report_yıl_secimi.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedYear))
            {
                LoadExcelData(selectedYear);
            }
        }

        private void LoadExcelData(string year)
        {
            try
            {
                string eaExcelFilePath = Path.Combine(ana_menu_form_objesi.userRootPath,
         (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
         (string)ana_menu_form_objesi.config.İl,
         (string)ana_menu_form_objesi.config.İlçe,
         (string)ana_menu_form_objesi.config.EA.ea_klasörü,
        (string)ana_menu_form_objesi.config.EA.cikti_dosyasi).Replace('/', '\\');

                string dekExcelFilePath = Path.Combine(ana_menu_form_objesi.userRootPath,
                         (string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                         (string)ana_menu_form_objesi.config.İl,
                         (string)ana_menu_form_objesi.config.İlçe,
                         (string)ana_menu_form_objesi.config.DEK.dek_klasörü,
                        (string)ana_menu_form_objesi.config.DEK.cikti_dosyasi).Replace('/', '\\');
                // Clear existing data in DataGridView
                ReportsTableDataGridView.DataSource = null;
                ReportsTableDataGridView.Rows.Clear();
                ReportsTableDataGridView.Columns.Clear();

                // Determine the Excel file path based on data source
                string excelFilePath = dataSource == "EA" ? eaExcelFilePath : dekExcelFilePath;

                // Check if the file exists
                if (!File.Exists(excelFilePath))
                {
                    MessageBox.Show($"Excel dosyası bulunamadı: {excelFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load the Excel file
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[year];
                    if (worksheet == null)
                    {
                        MessageBox.Show($"Excel'de {year} yılına ait bir sayfa bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Create a DataTable to hold the worksheet data
                    DataTable dt = new DataTable();

                    // Add columns based on the first row (headers)
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        string columnName = worksheet.Cells[1, col].Text;
                        if (string.IsNullOrEmpty(columnName))
                        {
                            columnName = $"Column{col}";
                        }
                        dt.Columns.Add(columnName);
                    }

                    // Add rows from the worksheet (starting from row 2 to skip headers)
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var dataRow = dt.NewRow();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            dataRow[col - 1] = worksheet.Cells[row, col].Text;
                        }
                        dt.Rows.Add(dataRow);
                    }

                    // Bind the DataTable to the DataGridView
                    ReportsTableDataGridView.DataSource = dt;

                    // Optional: Adjust column headers and formatting
                    ReportsTableDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReportExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there is data to export
                if (ReportsTableDataGridView.DataSource == null || ReportsTableDataGridView.Rows.Count == 0)
                {
                    MessageBox.Show(" Kaydededilecek veri bulunamadı. Lütfen önce bir yıl seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected year
                string selectedYear = comboBox_report_yıl_secimi.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedYear))
                {
                    MessageBox.Show("Lütfen bir yıl seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Open SaveFileDialog to let the user choose the export location
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    saveFileDialog.Title = "Raporu Kaydet";
                    saveFileDialog.FileName = $"{dataSource}_Report_{selectedYear}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Create a new Excel package
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add(selectedYear);

                            // Get the DataTable from DataGridView
                            DataTable dt = (DataTable)ReportsTableDataGridView.DataSource;

                            // Write headers
                            for (int col = 0; col < dt.Columns.Count; col++)
                            {
                                worksheet.Cells[1, col + 1].Value = dt.Columns[col].ColumnName;
                                worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                            }

                            // Write data rows
                            for (int row = 0; row < dt.Rows.Count; row++)
                            {
                                for (int col = 0; col < dt.Columns.Count; col++)
                                {
                                    worksheet.Cells[row + 2, col + 1].Value = dt.Rows[row][col]?.ToString();
                                }
                            }

                            // Auto-fit columns
                            worksheet.Cells.AutoFitColumns();

                            // Save the file
                            File.WriteAllBytes(saveFileDialog.FileName, package.GetAsByteArray());
                            MessageBox.Show($"Rapor başarıyla {saveFileDialog.FileName} konumuna ihraç edildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rapor kaydedilirken edilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReportCancelButton_Click(object sender, EventArgs e)
        {
            isOperationCancelled = true;
            this.Close();
        }
    }
}
/*using Irony;
using SLF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class ReportTableForm : Form
    {
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        public int slfStartYear = 0, slfEndYear = 0;

        public ReportTableForm()
        {
            InitializeComponent();
            var yearService = YearService.GetInstance();
            if (this.slfStartYear > 0 && this.slfEndYear > 0)
            {
                // ModülFormu'na dışarıdan atanan değerleri YearService'e aktarma
                yearService.SetYears(this.slfStartYear, this.slfEndYear);
            }
            else
            {
                // YearService'ten değerleri alma
                this.slfStartYear = yearService.slfStartYear;
                this.slfEndYear = yearService.slfEndYear;
            }

            var yearList = new List<int>();
            for (int year = slfStartYear; year <= slfEndYear; year++)
            {
                yearList.Add(year);
            }
            comboBox_report_yıl_secimi.DataSource = yearList; // Rapor yılı seçimi için ComboBox2

        }

        private void ReportCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}*/