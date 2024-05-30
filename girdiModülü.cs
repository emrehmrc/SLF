using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLF
{
    public class NoFileSelectedException : Exception
    {
        public NoFileSelectedException(string message) : base(message)
        {
        }
    }

    public class GirdiModülü
    {
        private readonly List<string> veri_listesi_requires_xlsx = new List<string> {
            "Ekonometrik Yük Tahmini Verileri",
            "Abone Verileri"
        };
        private readonly List<string> veri_listesi_requires_csv = new List<string> { };
        private readonly List<string> veri_listesi_requires_tabular = new List<string> { };

        private readonly List<string> nullLikeStrings = new List<string>
        {
            "null",
            "N/A",
            "#N/A"
        };

        private string seçilenVeriTipi;

        private const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        private const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        private const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        private const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        private const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        private readonly string combinedExcelFilter;
        private readonly string combinedCsvFilter;
        private readonly string combinedTabularFilter;

        private DataTable currentDataTable = new DataTable();

        private readonly Dictionary<string, Dictionary<string, (float Min, float Max)>> minMaxCheckMap = new Dictionary<string, Dictionary<string, (float Min, float Max)>>
        {
            {
                "Abone Verileri",
                new Dictionary<string, (float Min, float Max)>
                {
                    { "X_KOORDINAT", (27.0f, 27.15f) },
                    { "Y_KOORDINAT", (38.46f, 38.53f) } 
                }
            }
        };

        // Public read-only property
        public DataTable CurrentDataTable
        {
            get { return currentDataTable; }
        }

        public GirdiModülü()
        {
            combinedExcelFilter = $"{FilterExcelFiles}|{FilterAllFiles}";
            combinedCsvFilter = $"{FilterCsvFiles}|{FilterAllFiles}";
            combinedTabularFilter = $"{FilterTabularFiles}|{FilterAllFiles}";
        }

        public void ProcessFileSelection(string seçilenVeriTipi)
        {
            this.seçilenVeriTipi = seçilenVeriTipi;
            var fileDialog1 = new OpenFileDialog();
            fileDialog1.Title = FileDialogTitle;

            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedExcelFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessExcelFile(selectedFileName, seçilenVeriTipi);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else if (veri_listesi_requires_csv.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedCsvFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessCsvFile(selectedFileName);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else if (veri_listesi_requires_tabular.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedTabularFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessTabularFile(selectedFileName);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else throw new NoFileSelectedException("Bu veri tipi için atanmış bir dosya seçimi prosedürü henüz yok.");
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

        public void Validate()
        {
            ReportNullCounts();
            ReportUniqueRowCounts();
            ReportUniqueCounts();
            ReportCoordinatesOutOfLimits();
        }
        private void ReportNullCounts()
        {
            // StringBuilder to build the report message
            StringBuilder reportMessage = new StringBuilder();

            foreach (DataColumn column in currentDataTable.Columns)
            {
                // Count the number of null, DBNull, "null", and "N/A" values in the current column

                int nullCount = currentDataTable.AsEnumerable().Count(row =>
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
        private void ReportUniqueRowCounts()
        {
            // StringBuilder to build the report message
            StringBuilder reportMessage = new StringBuilder();

            // HashSet to store unique rows
            HashSet<string> uniqueRows = new HashSet<string>();

            // Iterate through each row in the DataTable
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Serialize the row into a string representation
                string rowString = string.Join("|", row.ItemArray.Select(item => item?.ToString() ?? string.Empty));

                // Add the string representation to the HashSet
                uniqueRows.Add(rowString);
            }

            // Calculate the number of duplicate rows
            int totalRows = currentDataTable.Rows.Count;
            int uniqueRowCount = uniqueRows.Count;
            int duplicateRowCount = totalRows - uniqueRowCount;

            // Append the unique and duplicate row counts to the report message
            reportMessage.AppendLine($"Total Rows: {totalRows}");
            reportMessage.AppendLine($"Unique Rows: {uniqueRowCount}");
            reportMessage.AppendLine($"Duplicate Rows: {duplicateRowCount}");

            // Display the report message in a MessageBox
            MessageBox.Show(reportMessage.ToString(), "Unique Row Counts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ReportUniqueCounts()
        {
            // StringBuilder to build the report message
            StringBuilder reportMessage = new StringBuilder();

            foreach (DataColumn column in currentDataTable.Columns)
            {
                // HashSet to store unique values in the current column
                HashSet<string> uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (DataRow row in currentDataTable.Rows)
                {
                    // Get the value in the current column and row
                    var value = row[column]?.ToString();

                    // Add the value to the HashSet
                    uniqueValues.Add(value ?? string.Empty);
                }

                // Calculate the number of unique values and duplicates
                int totalCount = currentDataTable.Rows.Count;
                int uniqueCount = uniqueValues.Count;
                int duplicateCount = totalCount - uniqueCount;

                // Append the column name and unique count to the report message
                reportMessage.AppendLine($"{column.ColumnName}: {uniqueCount} unique value(s), {duplicateCount} duplicate(s)");
            }

            // Display the report message in a MessageBox
            MessageBox.Show(reportMessage.ToString(), "Unique Counts Per Column", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap[seçilenVeriTipi]["X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap[seçilenVeriTipi]["Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["Y_KOORDINAT"]?.ToString(), out float valueY))
                {
                    if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                    {
                        countOutOfThresholdCoordinates++;
                    }
                }
            }
            string reportMessage = $"KOORDINAT sütunlarında {countOutOfThresholdCoordinates} değer belirlenen koordinatların dışarısında.";

            // Display the report message in a MessageBox
            MessageBox.Show(reportMessage, "Koordinat Sınırları", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

