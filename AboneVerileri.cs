using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLF
{
    public class AboneVerileri:GirdiModülü
    {
        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "X_KOORDINAT", (27.0f, 27.15f) }, // TODO: Update these values from the other data
            { "Y_KOORDINAT", (38.46f, 38.53f) } // TODO: Update these values from the other data
        };

        private readonly (float warningThreshold, float errorThreshold) TUKETIM_ERROR_THRESHOLD = WarningErrorBoundary(0.2f);
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.1f);
        private const float ABONE_KAPASITE_LIMIT = 0.6f;

        private string DATE_FORMAT = "yyyyMMdd";

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "TESISAT_NO", WarningErrorBoundary(0.2f) },
            { "X_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "Y_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "ADR_BINA_ID", WarningErrorBoundary(0.2f) },
            { "bina_turu", WarningErrorBoundary(0.2f) },
            { "BAGLANTI_GUCU", WarningErrorBoundary(0.4f) },
            { "SOZ_DURUM", INFO_ONLY },
            { "ABONE_GRUBU",WarningErrorBoundary(0.2f) },
            { "GERILIM_SEVIYESI", INFO_ONLY },
            //{ "SOZ_BAS_TARIH", INFO_ONLY },
            //{ "SOZ_BIT_TARIH", INFO_ONLY },
            { "ENERJI_TABLO_KAYIT_KODU", WarningErrorBoundary(0.1f) }
        };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dateFormatCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            //{ "SOZ_BAS_TARIH", INFO_ONLY },
            //{ "SOZ_BIT_TARIH", INFO_ONLY },
        };
        private readonly List<string> duplicateFieldsGivingError = new List<string>
        {
            "TESISAT_NO",
        };
        public override void Validate()
        {
            base.Validate();

            ReportErrorLessThanOrEqualToZero();
            ReportNullCounts();
            ReportDuplicateRowCounts();
            ReportDuplicateCounts();
            ReportCoordinatesOutOfLimits();
            AboneKapasiteCheck();
            ReportDateFormatErrors();
        }
        private void ReportNullCounts()
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

                if (nullPercentage > 0 && nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                        column.ColumnName, "Null değer", $"{nullPercentage:P1}"
                    });
                }
            }
        }
        private void ReportDateFormatErrors()
        {
            float invalidPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (dateFormatCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    var thresholds = dateFormatCheckWithLevel[column.ColumnName];
                    int invalidCount = currentDataTable.AsEnumerable().Count(row =>
                    {
                        var value = row[column]?.ToString();
                        return !DateTime.TryParseExact(value, DATE_FORMAT, null, DateTimeStyles.None, out _);
                    });

                    invalidPercentage = (float)invalidCount / totalRows;

                    if (invalidPercentage > 0)
                    {
                        var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        datatableLevel.Rows.Add(new object[]
                        {
                            column.ColumnName, "Geçersiz tarih formatı", $"{invalidPercentage:P1}", $"Tarihler { DATE_FORMAT } biçiminde olmalıdır. Lütfen düzeltiniz."
                        });
                    }
                }   
            }
        }
        private void ReportDuplicateRowCounts()
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
        private void ReportDuplicateCounts()
        {
            float duplicatePercentage;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!duplicateFieldsGivingError.Contains(column.ColumnName))
                {
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

        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["Y_KOORDINAT"];

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
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                // Add the warning to the DataTable
                datatableLevel.Rows.Add(new object[] {
                    "X_KOORDINAT & Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}"
                });
            }
        }
        private void ReportErrorLessThanOrEqualToZero()
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
                var thresholds = TUKETIM_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(nonPositivePercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                // Append the column name and null count to the report message
                datatableLevel.Rows.Add(new object[] {
                    column.ColumnName, "Son yıl tüketim verisi", $"{nonPositivePercentage:P1} abonenin tüketim verisi yok",
                    "Bu abonelerin tüketim verileri silinecek."
                });
            }
        }
        private void AboneKapasiteCheck()
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

