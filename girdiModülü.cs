using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLF
{
    public class GirdiModülü
    {
        private List<string> veri_listesi_requires_xlsx;
        private List<string> veri_listesi_requires_csv;
        private List<string> veri_listesi_requires_tabular;

        private readonly List<string> nullLikeStrings = new List<string>
        {
            "null",
            "N/A",
            "#N/A"
        };

        private const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        private const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        private const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        private const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        private const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        private readonly string combinedExcelFilter;
        private readonly string combinedCsvFilter;
        private readonly string combinedTabularFilter;

        private DataTable currentDataTable = new DataTable();

        // Public read-only property
        public DataTable CurrentDataTable
        {
            get { return currentDataTable; }
        }

        public GirdiModülü()
        {
            veri_listesi_requires_xlsx = new List<string> {
                "Ekonometrik Yük Tahmini Verileri",
                "Abone Verileri"
            };
            veri_listesi_requires_csv = new List<string> {};
            veri_listesi_requires_tabular = new List<string> {};

            combinedExcelFilter = $"{FilterExcelFiles}|{FilterAllFiles}";
            combinedCsvFilter = $"{FilterCsvFiles}|{FilterAllFiles}";
            combinedTabularFilter = $"{FilterTabularFiles}|{FilterAllFiles}";
        }

        public void ProcessFileSelection(string seçilenVeriTipi)
        {
            OpenFileDialog fileDialog1 = new OpenFileDialog();
            fileDialog1.Title = FileDialogTitle;

            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedExcelFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessExcelFile(selectedFileName, seçilenVeriTipi);
                }
            }
            else if (veri_listesi_requires_csv.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedCsvFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessCsvFile(selectedFileName);
                }
            }
            else if (veri_listesi_requires_tabular.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedTabularFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessTabularFile(selectedFileName);
                }
            }
        }

        private DataTable ProcessExcelFile(string fileName, string seçilenVeriTipi)
        {
            ExcelImporter importer = new ExcelImporter();
            DataTable dataTable = importer.ImportExcelFile(fileName, seçilenVeriTipi);



            return dataTable;
        }

        private DataTable ProcessCsvFile(string fileName)
        {
            // TODO: Implement CSV file processing
            return new DataTable();
        }

        private DataTable ProcessTabularFile(string fileName)
        {
            // TODO: Implement tabular file processing
            return new DataTable();
        }
        public void ReportNullCounts(DataTable dataTable)
        {
            // StringBuilder to build the report message
            StringBuilder reportMessage = new StringBuilder();

            foreach (DataColumn column in dataTable.Columns)
            {
                // Count the number of null, DBNull, "null", and "N/A" values in the current column

                int nullCount = dataTable.AsEnumerable().Count(row =>
                    row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(row[column]?.ToString(), StringComparer.OrdinalIgnoreCase)
                );

                // Append the column name and null count to the report message
                reportMessage.AppendLine($"{column.ColumnName}: {nullCount} null(s)");
            }

            // Display the report message in a MessageBox
            MessageBox.Show(reportMessage.ToString(), "Null Counts Per Column", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

