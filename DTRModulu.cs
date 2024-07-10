using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public class DTRModulu : GirdiModülü

    {
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = ERROR_ONLY;
        private readonly string DATE_FORMAT = "dd.MM.yyyy";

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dateFormatCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "TRAFO_KURULUM_TARIHI", INFO_ONLY },
        };
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

        private readonly List<string> duplicateFieldsGivingError = new List<string>
        {
            "TRAFO_KODU",
        };
        private void ReportCompositeDuplicateCounts()
        {
            float duplicatePercentage;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!duplicateFieldsGivingError.Contains(column.ColumnName))
                {
                    continue;
                }
                // HashSet to store unique composite keys
                var uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var duplicateRowIndices = new List<int>();

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value1 = row["TRAFO_ID"]?.ToString() ?? string.Empty;
                    var value2 = row["TRAFO_X_KOORDINAT"]?.ToString() ?? string.Empty;
                    var value3 = row["TRAFO_Y_KOORDINAT"]?.ToString() ?? string.Empty;
                    var compositeKey = $"{value1}|{value2}|{value3}";

                    if (uniqueValues.Contains(compositeKey))
                    {
                        duplicateRowIndices.Add(i);
                    }

                    uniqueValues.Add(compositeKey);
                }

                columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"] = duplicateRowIndices;

                // Calculate the number of unique values and duplicates
                int totalCount = currentDataTable.Rows.Count;
                int uniqueCount = uniqueValues.Count;
                int duplicateCount = totalCount - uniqueCount;
                duplicatePercentage = (float)duplicateCount / totalCount;

                if (duplicateCount > 0)
                {
                    // Append the column name and unique count to the report message
                    infoDataTable.Rows.Add(new object[] {
                    column.ColumnName, "Mükerrer hücre değerleri", $"{duplicatePercentage:P1}"
                });
                }
            }
        }

        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "TRAFO_X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "TRAFO_Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
            
        };
        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["TRAFO_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["TRAFO_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["TRAFO_X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["TRAFO_Y_KOORDINAT"]?.ToString(), out float valueY))
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
                    "TRAFO_X_KOORDINAT & TRAFO_Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
                });
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "TRAFO_X_KOORDINAT", ERROR_ONLY},
            { "TRAFO_Y_KOORDINAT", ERROR_ONLY},
            { "TM_FIDER_ID", WarningErrorBoundary(0.05f)},
            { "TRAFO_KURULUM_TARIHI", WarningErrorBoundary(0.2f) },
            { "TRAFO_KAPASITESI", WarningErrorBoundary(0.2f) },
            { "TRAFO_MULKIYET", WarningErrorBoundary(0.2f) },
            { "YIL_TUKETIM_2023", WarningErrorBoundary(0.2f) },
            { "YIL_DEMANT_2023", WarningErrorBoundary(0.2f) },
        };
        private void ReportNullCounts()
        {
            float nullPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }

                List<int> nullRows = new List<int>();

                int nullCount = 0;

                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    if (IsNullLike(row[column]))
                    {
                        nullCount++;
                        // Add the row number and the null-like value to the nullRows
                        nullRows.Add(i);
                    }
                }

                columnNullRowsMap[column.ColumnName] = nullRows;

                nullPercentage = (float)nullCount / totalRows;

                if (nullPercentage > 0)
                {
                    var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                        column.ColumnName, "Null değer", $"{nullPercentage:P1}"
                    });
                }
            }
        }


        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimits();

            ReportCompositeDuplicateCounts();

            ReportDateFormatErrors();
        }

        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"]);



            RemoveCombinedRows(combinedRowsToRemoveList);
        }

        private void RemoveCombinedRows(List<int> rowsToRemoveList)
        {
            // Remove duplicates and sort in descending order
            var rowIndicesToRemove = rowsToRemoveList.Distinct().OrderByDescending(i => i).ToList();

            foreach (int rowIndex in rowIndicesToRemove)
            {
                if (rowIndex < currentDataTable.Rows.Count)
                {
                    currentDataTable.Rows.RemoveAt(rowIndex);
                }
            }
        }

    }
}

