using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        protected Önizleme onizleme1 = new Önizleme();
        protected readonly List<string> veri_listesi_requires_xlsx = new List<string> {
            "Ekonometrik Yük Tahmini Verileri",
            "Abone Verileri"
        };
        protected readonly List<string> veri_listesi_requires_csv = new List<string> { };
        protected readonly List<string> veri_listesi_requires_tabular = new List<string> { };

        protected readonly List<string> nullLikeStrings = new List<string>
        {
            "",
            "null",
            "N/A",
            "#N/A"
        };

        protected string seçilenVeriTipi;

        protected const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        protected const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        protected const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        protected const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        protected const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        protected readonly string combinedExcelFilter;
        protected readonly string combinedCsvFilter;
        protected readonly string combinedTabularFilter;

        protected DataTable currentDataTable = new DataTable();
        protected DataTable errorDataTable = new DataTable();
        protected DataTable warningDataTable = new DataTable();
        protected DataTable infoDataTable = new DataTable();

        protected readonly Dictionary<string, Dictionary<string, (float Min, float Max)>> minMaxCheckMap = new Dictionary<string, Dictionary<string, (float Min, float Max)>>
        {
            {
                "Abone Verileri",
                new Dictionary<string, (float Min, float Max)>
                {
                    { "X_KOORDINAT", (27.0f, 27.15f) }, // TODO: Update these values from the other data
                    { "Y_KOORDINAT", (38.46f, 38.53f) } // TODO: Update these values from the other data
                }
            }
        };

        protected const float MAX_THRESHOLD = float.MaxValue;
        protected const float MIN_THRESHOLD = 0.0f;
        protected const float TUKETIM_ERROR_THRESHOLD = 0.2f;
        protected const float COORDINATE_ERROR_THRESHOLD = 0.1f;
        protected const float ABONE_KAPASITE_LIMIT = 0.6f;
        protected static (float Min, float Max) WARNING_ONLY = (MIN_THRESHOLD, MAX_THRESHOLD);
        protected static (float Min, float Max) INFO_ONLY = (MAX_THRESHOLD, MAX_THRESHOLD);

        protected static (float Min, float Max) WarningErrorBoundary(float boundary)
        {
            // Bi verinin "boundary"ye kadar olan kısmı warning, "boundary"den sonrası error
            return (MIN_THRESHOLD, boundary);
        }

        protected readonly Dictionary<string, Dictionary<string, (float warningThreshold, float errorThreshold)>> nullFieldsCheckWithLevel = new Dictionary<string, Dictionary<string, (float warningThreshold, float errorThreshold)>>
        {
            {
                "Abone Verileri",
                    new Dictionary<string, (float warningThreshold, float errorThreshold)> 
                    {
                        { "TESISAT_NO", WarningErrorBoundary(0.2f) },
                        { "X_KOORDINAT", WarningErrorBoundary(0.2f)  }, 
                        { "Y_KOORDINAT", WarningErrorBoundary(0.2f)  },
                        { "ADR_BINA_ID", WarningErrorBoundary(0.2f) },
                        { "bina_turu", WarningErrorBoundary(0.2f) },
                        { "BAGLANTI_GUCU", WarningErrorBoundary(0.4f) },
                        { "SOZ_DURUM", INFO_ONLY },
                        { "ABONE_GRUBU",WarningErrorBoundary(0.2f) },
                        { "GERILIM_SEVIYESI", INFO_ONLY },
                        { "SOZ_BAS_TARIH", WARNING_ONLY },
                        { "SOZ_BIT_TARIH", WARNING_ONLY },
                        { "ENERJI_TABLO_KAYIT_KODU", WarningErrorBoundary(0.1f) }
                    }
            }
        };
        protected readonly Dictionary<string, List<string>> duplicateFieldsGivingError = new Dictionary<string, List<string>>
        {
            {
                "Abone Verileri",
                    new List<string> {
                        "TESISAT_NO",
                    }
            }
        };
        // Public read-only property
        public DataTable CurrentDataTable { get { return currentDataTable; }}
        public Önizleme Onizleme1 { get { return onizleme1; }}

        public GirdiModülü()
        {
            combinedExcelFilter = $"{FilterExcelFiles}|{FilterAllFiles}";
            combinedCsvFilter = $"{FilterCsvFiles}|{FilterAllFiles}";
            combinedTabularFilter = $"{FilterTabularFiles}|{FilterAllFiles}";
            AddColumnsToDataTable(errorDataTable);
            AddColumnsToDataTable(warningDataTable);
            AddColumnsToDataTable(infoDataTable);
            onizleme1.Onizleme_DataGrid2.DataSource = errorDataTable;
            onizleme1.Onizleme_DataGrid3.DataSource = warningDataTable;
            onizleme1.Onizleme_DataGrid4.DataSource = infoDataTable;
        }

        protected void AddColumnsToDataTable(DataTable table)
        {
            table.Columns.Add("Sütun Adı", typeof(string));
            table.Columns.Add("Validasyon Türü", typeof(string));
            table.Columns.Add("Validasyon Bilgisi", typeof(string));
            table.Columns.Add("Ek Açıklamalar", typeof(string));
        }

        public void ProcessFileSelection(string seçilenVeriTipi)
        {
            this.seçilenVeriTipi = seçilenVeriTipi;
            var fileDialog1 = new OpenFileDialog
            {
                Title = FileDialogTitle
            };

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

        protected DataTable ProcessExcelFile(string fileName, string seçilenVeriTipi)
        {
            ExcelImporter importer = new ExcelImporter();
            DataTable dataTable = importer.ImportExcelFile(fileName, seçilenVeriTipi);



            return dataTable;
        }

        protected DataTable ProcessCsvFile(string fileName)
        {
            // TODO: Implement CSV file processing
            return new DataTable();
        }

        protected DataTable ProcessTabularFile(string fileName)
        {
            // TODO: Implement tabular file processing
            return new DataTable();
        }

        public void Validate()
        {
            errorDataTable.Rows.Clear();
            warningDataTable.Rows.Clear();
            infoDataTable.Rows.Clear();
            ReportErrorLessThanOrEqualToZero();
            ReportNullCounts();
            ReportDuplicateRowCounts();
            ReportDuplicateCounts();
            ReportCoordinatesOutOfLimits();
            AboneKapasiteCheck();
        }
        protected void ReportNullCounts()
        {
            float nullPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                // Count the number of null, DBNull, "null", and "N/A" values in the current column
                int nullCount = currentDataTable.AsEnumerable().Count(row =>
                    row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(row[column]?.ToString(), StringComparer.OrdinalIgnoreCase)
                );

                nullPercentage = (float)nullCount / totalRows;

                if (nullPercentage > 0 && nullFieldsCheckWithLevel[seçilenVeriTipi].ContainsKey(column.ColumnName))
                {
                    var datatableLevel = infoDataTable;

                    if (nullPercentage >= nullFieldsCheckWithLevel[seçilenVeriTipi][column.ColumnName].errorThreshold)
                    {
                        datatableLevel = errorDataTable;
                    }
                    else if (nullPercentage >= nullFieldsCheckWithLevel[seçilenVeriTipi][column.ColumnName].warningThreshold)
                    {
                        datatableLevel = warningDataTable;
                    }
                    // Append the column name and null count to the report message
                    datatableLevel.Rows.Add(new object[] {
                        column.ColumnName, "Null değer", $"{nullPercentage:P1}"
                    });
                }
            }
        }
        protected void ReportDuplicateRowCounts()
        {
            // HashSet to store unique rows
            HashSet<string> uniqueRows = new HashSet<string>();

            float duplicatePercentage = 0.0f;

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

            if (duplicateRowCount > 0)
            {
                duplicatePercentage = (float)duplicateRowCount / totalRows;
                errorDataTable.Rows.Add(new object[] {
                    "", "Mükerrer veri", $"{duplicatePercentage:P1}"
                });
            }
        }
        protected void ReportDuplicateCounts()
        {
            float duplicatePercentage;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!duplicateFieldsGivingError[seçilenVeriTipi].Contains(column.ColumnName)) {
                    continue;
                }
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
                duplicatePercentage = (float)duplicateCount / totalCount;

                if (duplicateCount > 0)
                {
                    // Append the column name and unique count to the report message
                    warningDataTable.Rows.Add(new object[] {
                        column.ColumnName, "Mükerrer hücre değerleri", $"{duplicatePercentage:P1}"
                    });
                }
            }
        }

        protected void ReportCoordinatesOutOfLimits()
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

            float outOfThresholdPercentage = (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count;

            if (outOfThresholdPercentage > 0)
            {
                var datatableLevel = warningDataTable;
                if (outOfThresholdPercentage >= COORDINATE_ERROR_THRESHOLD)
                {
                    datatableLevel = errorDataTable;
                }
                // Add the warning to the DataTable
                datatableLevel.Rows.Add(new object[] {
                    "X_KOORDINAT & Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}"
                });
            }
        }
        protected void ReportErrorLessThanOrEqualToZero()
        {
            int currentYear = DateTime.Now.Year;
            float nonPositivePercentage;
            int totalRows = currentDataTable.Rows.Count;
            var column = currentDataTable.Columns[$"{currentYear - 1}_Tuketim"];
            var fallbackColumn = currentDataTable.Columns[$"{currentYear - 2}_Tuketim"];

            int nonPositiveCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (
                    row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(row[column]?.ToString(), StringComparer.OrdinalIgnoreCase) ||
                    float.TryParse(row[column]?.ToString(), out float value) && value <= 0)
                {
                    // If the last year's consumption data is missing or less than or equal to zero, check the previous year's data
                    if (row.IsNull(fallbackColumn) ||
                        row[fallbackColumn] == DBNull.Value ||
                        nullLikeStrings.Contains(row[fallbackColumn]?.ToString(), StringComparer.OrdinalIgnoreCase) ||
                        float.TryParse(row[fallbackColumn]?.ToString(), out float fallbackValue) && fallbackValue <= 0
                    )
                    { 
                        nonPositiveCount++;
                    }
                }
            }

            nonPositivePercentage = (float)nonPositiveCount / totalRows;

            if (nonPositivePercentage > 0)
            {
                var datatableLevel = warningDataTable;
                if (nonPositivePercentage >= TUKETIM_ERROR_THRESHOLD)
                {
                    datatableLevel = errorDataTable;
                }

                // Append the column name and null count to the report message
                datatableLevel.Rows.Add(new object[] {
                    column.ColumnName, "Son yıl tüketim verisi", $"{nonPositivePercentage:P1} abonenin tüketim verisi yok",
                    "Bu abonelerin tüketim verileri silinecek."
                });
            }
        }
        protected void AboneKapasiteCheck()
        {
            // yillik tuketim / 8760 / baglanti gucu
            const int HoursInYear = 8760;
            int overCapacityCount = 0;
            int totalRows = currentDataTable.Rows.Count;
            int lastYear = DateTime.Now.Year - 1;
            var lastYearTuketim = currentDataTable.Columns[$"{lastYear}_Tuketim"];
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row[lastYearTuketim]?.ToString(), out float tuketim) && tuketim > 0)
                {
                    var baglantiGucu = row["BAGLANTI_GUCU"];
                    if (float.TryParse(baglantiGucu?.ToString(), out float guc) && guc > 0)
                    {
                        float kapasite = (tuketim / HoursInYear) / guc;
                        if (kapasite > ABONE_KAPASITE_LIMIT)
                        {
                            overCapacityCount++;
                           
                        }
                    }
                }
            }

            float overCapacityPercentage = (float)overCapacityCount / totalRows;

            warningDataTable.Rows.Add(new object[]
            {
                 "", "Abone kapasitesi", $"{overCapacityPercentage:P1}",
                 $"Abone kapasitesi {ABONE_KAPASITE_LIMIT:P1}'den büyük olan abonelerin tüketim verileri silinecek."
            });

        }
     }
}

