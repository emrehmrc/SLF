using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public class DTRModulu : GirdiModülü

    {
        private readonly (float warningThreshold, float errorThreshold) TUKETIM_ERROR_THRESHOLD = WarningErrorBoundary(0.2f);
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = ERROR_ONLY;
        private readonly string DATE_FORMAT = "dd.MM.yyyy";
        private static readonly List<int> TRAFO_KAPASITE_LISTESI = new List<int>
        {
            15, 25, 40, 50, 63, 100, 160, 200, 250, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500
        };
        private static readonly List<int> PRIMER_GERILIM_LISTESI = new List<int>
        {
            6300, 10500, 15800, 31500, 33000, 34500
        };
        private static readonly List<int> SEKONDER_GERILIM_LISTESI = new List<int>
        {
            400
        };

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dateFormatCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "TRAFO_KURULUM_TARIHI", WarningErrorBoundary(0.2f) },
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
                    int invalidCount = 0;
                    List<int> invalidRows = new List<int>();

                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        var value = row[column]?.ToString();
                        if (!DateTime.TryParseExact(value, DATE_FORMAT, null, DateTimeStyles.None, out _))
                        {
                            invalidCount++;
                            // Add the row index to the invalidRows list
                            invalidRows.Add(i);
                        }
                    }

                    invalidPercentage = (float)invalidCount / totalRows;

                    if (invalidPercentage > 0)
                    {
                        // Add the column and its invalid rows to the dictionary
                        columnNullRowsMap[column.ColumnName] = invalidRows;

                        var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        datatableLevel.Rows.Add(new object[]
                        {
                            column.ColumnName, "Geçersiz tarih formatı", $"{invalidPercentage:P1}", $"Tarihler { DATE_FORMAT } biçiminde olmalıdır. Lütfen düzeltiniz."
                        });
                    }
                }
            }
        }

        private void ReportTrafoLoad()
        {
            double loadThreshold = 1.0;
            int overLoadCount = 0;
            int totalRows = currentDataTable.Rows.Count;
            var lastYearDemand = currentDataTable.Columns[$"YIL_DEMANT_{lastYear}"];
            var nullRows = new List<int>();
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row[lastYearDemand]?.ToString(), out float demand) && demand > 0)
                {
                    var trafoKapasitesi = row["TRAFO_KAPASITESI"];
                    if (float.TryParse(trafoKapasitesi?.ToString(), out float kapasite) && kapasite > 0)
                    {
                        float load = demand / kapasite;
                        if (load > loadThreshold )
                        {
                            overLoadCount++;
                            nullRows.Add(currentDataTable.Rows.IndexOf(row));
                        }
                    }
                }
            }

            columnNullRowsMap["TRAFO_LOAD"] = nullRows;
            float overCapacityPercentage = (float)overLoadCount / totalRows;

            if (overCapacityPercentage > 0)
            {
                infoDataTable.Rows.Add(new object[]
                {
                 "", "Abone kapasitesi", $"{overCapacityPercentage:P1}",
                 $"Abone kapasitesi {loadThreshold:P1}'den büyük olan abonelerin tüketim verileri silinecek."
                });
            }

        }

        private readonly List<string> duplicateFieldsGivingError = new List<string>
        {
            "TRAFO_KODU",
        };
        private void ReportCompositeDuplicateCounts()
        {
            float duplicatePercentage;
            float partialDuplicatePercentage;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!duplicateFieldsGivingError.Contains(column.ColumnName))
                {
                    continue;
                }
                // HashSet to store unique composite keys
                var uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var duplicateRowIndices = new List<int>();

                // Dictionary to track partial duplicates by TRAFO_KODU
                var trafoIdToCoordinatesMap = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                var partialDuplicateRowIndices = new List<int>();

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value1 = row["TRAFO_KODU"]?.ToString() ?? string.Empty;
                    var value2 = row["TRAFO_X_KOORDINAT"]?.ToString() ?? string.Empty;
                    var value3 = row["TRAFO_Y_KOORDINAT"]?.ToString() ?? string.Empty;
                    var compositeKey = $"{value1}|{value2}|{value3}";

                    if (uniqueValues.Contains(compositeKey))
                    {
                        duplicateRowIndices.Add(i);
                    }

                    uniqueValues.Add(compositeKey);
                    // Check for partial duplicates
                    if (trafoIdToCoordinatesMap.ContainsKey(value1))
                    {
                        var coordinatesSet = trafoIdToCoordinatesMap[value1];
                        var coordinatePair = $"{value2}|{value3}";

                        if (!coordinatesSet.Contains(coordinatePair))
                        {
                            partialDuplicateRowIndices.Add(i);
                        }

                        // Add the coordinate pair to the set for this TRAFO_KODU
                        coordinatesSet.Add(coordinatePair);
                    }
                    else
                    {
                        // Initialize the set for this TRAFO_KODU
                        trafoIdToCoordinatesMap[value1] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { $"{value2}|{value3}" };
                    }
                }

                columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"] = duplicateRowIndices;
                columnNullRowsMap["PARTIAL_DUPLICATE_TRAFO_KODU"] = partialDuplicateRowIndices;

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

                // Calculate and store the duplicate percentage for partial duplicates
                int partialDuplicateCount = partialDuplicateRowIndices.Count;
                partialDuplicatePercentage = (float)partialDuplicateCount / totalCount;

                if (partialDuplicateCount > 0)
                {
                    // Append the column name and duplicate percentage to the report message
                    errorDataTable.Rows.Add(new object[] {
                    column.ColumnName, "Kısmi mükerrer hücre değerleri", $"{partialDuplicatePercentage:P1}"
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
            { "TRAFO_KODU", ERROR_ONLY},
            { "TRAFO_X_KOORDINAT", ERROR_ONLY},
            { "TRAFO_Y_KOORDINAT", ERROR_ONLY},
            { "TM_FIDER_ID", WarningErrorBoundary(0.1f)},
            { "TRAFO_KURULUM_TARIHI", WarningErrorBoundary(0.2f) },
            { "TRAFO_KAPASITESI", WarningErrorBoundary(0.2f) },
            { "TRAFO_MULKIYET", WarningErrorBoundary(0.2f) },
            //{ "YIL_TUKETIM_2023", WarningErrorBoundary(0.2f) },
            //{ "YIL_DEMANT_2023", WarningErrorBoundary(0.2f) },
            { "PRIMER_GERILIM", WARNING_ONLY },
            { "SEKONDER_GERILIM", WARNING_ONLY },
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

                var isZero = !column.ColumnName.Contains("MULKIYET");
                List<int> nullRows = new List<int>();

                int nullCount = 0;

                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    if (IsNullLike(row[column], isZero))
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

        private void ReportPrimerGerilim()
        {
            float invalidPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;
            string column = "PRIMER_GERILIM";

                List<int> invalidRows = new List<int>();

                int invalidCount = 0;

                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var cellValue = row[column]?.ToString();
                    if (!IsNullLike(cellValue))
                    {
                        if (int.TryParse(cellValue, out int value))
                        {
                            if (!PRIMER_GERILIM_LISTESI.Contains(value))
                            {
                                invalidCount++;
                                // Add the row number to the invalidRows
                                invalidRows.Add(i);
                            }
                        }
                        else
                        {
                            invalidCount++;
                            // Add the row number to the invalidRows if the value cannot be parsed
                            invalidRows.Add(i);
                        }
                    }
                }

                //columnInvalidRowsMap[column.ColumnName] = invalidRows;

                invalidPercentage = (float)invalidCount / totalRows;

                if (invalidPercentage > 0)
                {
                    //var thresholds = invalidFieldsCheckWithLevel[column.ColumnName];
                    //var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    infoDataTable.Rows.Add(new object[] {
                    "PRIMER_GERILIM", "Geçersiz değer", $"{invalidPercentage:P1}"
                });
                }
        }

        private void ReportSekonderGerilim()
        {
            float invalidPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;
            string column = "SEKONDER_GERILIM";

            List<int> invalidRows = new List<int>();

            int invalidCount = 0;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                var cellValue = row[column]?.ToString();
                if (!IsNullLike(cellValue))
                {
                    if (int.TryParse(cellValue, out int value))
                    {
                        if (!SEKONDER_GERILIM_LISTESI.Contains(value))
                        {
                            invalidCount++;
                            // Add the row number to the invalidRows
                            invalidRows.Add(i);
                        }
                    }
                    else
                    {
                        invalidCount++;
                        // Add the row number to the invalidRows if the value cannot be parsed
                        invalidRows.Add(i);
                    }
                }
            }

            //columnInvalidRowsMap[column.ColumnName] = invalidRows;

            invalidPercentage = (float)invalidCount / totalRows;

            if (invalidPercentage > 0)
            {
                //var thresholds = invalidFieldsCheckWithLevel[column.ColumnName];
                //var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                infoDataTable.Rows.Add(new object[] {
                    "SEKONDER_GERILIM", "Geçersiz değer", $"{invalidPercentage:P1}"
                });
            }
        }


        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimits();

            ReportCompositeDuplicateCounts();

            ReportDateFormatErrors();
            ReportErrorLessThanZero($"YIL_TUKETIM_{lastYear}");
            ReportErrorLessThanZero($"YIL_DEMANT_{lastYear}");
            ReportTrafoLoad();

            ReportPrimerGerilim();
            ReportSekonderGerilim();
        }

        public override void Impute() { 
            ImputeTMFiderID();
            ImputeTuketim();
            ImputeDemand();
            ImputeAverageDate();
            ImputeTrafoMulkiyet();
            ImputeTrafoKapasitesi();
        }

        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap[$"YIL_TUKETIM_{lastYear}"]);



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

        private void ImputeTrafoMulkiyet()
        {
            var column = "TRAFO_MULKIYET";
            foreach (int missingIndex in columnNullRowsMap[column])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                missingRow[column] = 0;

            }
        }
        private void ImputeTrafoKapasitesi()
        {
            var column = "TRAFO_KAPASITESI";
            var refColumn = $"YIL_DEMANT_{lastYear}";
            double demandFactor = 2.5;
            double tentativeKapasite;
            foreach (int missingIndex in columnNullRowsMap[column])
            {
                var missingRow = currentDataTable.Rows[missingIndex];

                var refValue = missingRow[refColumn];

                if (!IsNullLike(refValue))
                {
                    if (double.TryParse(refValue?.ToString(), out double demand))
                    {
                        // Calculate the tentative kapasite
                        tentativeKapasite = demand * demandFactor;

                        // Find the closest kapasite value in the list
                        double kapasite = TRAFO_KAPASITE_LISTESI.OrderBy(x => Math.Abs(x - tentativeKapasite)).First();
                        // Assign the calculated kapasite to the missing row
                        missingRow[column] = kapasite;
                    }
                    else
                    {
                        throw new ArgumentException($"'{refColumn}' column has invalid data format at row index {missingIndex}.");
                    }
                }
            }
        }

        private void ImputeTuketim()
        {
            // TODO: DEEP LEARNING METODU ILE DEGISTIRILECEK
            var tuketimColumn = $"YIL_TUKETIM_{lastYear}";
            foreach (int missingIndex in imputableRowsMap[tuketimColumn])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                double imputedValue = 99999;
                missingRow[tuketimColumn] = imputedValue;

            }
        }

        private void ImputeDemand()
        {
            var demandColumn = $"YIL_DEMANT_{lastYear}";
            var tuketimColumn = $"YIL_TUKETIM_{lastYear}";
            foreach (int missingIndex in imputableRowsMap[demandColumn])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                var tuketim_value = missingRow[tuketimColumn];
                double imputedValue;
                if (double.TryParse(tuketim_value.ToString(), out double tuketimDouble))
                {
                    imputedValue = 2 * tuketimDouble / 8760;
                    missingRow[demandColumn] = imputedValue;
                }
                else
                {
                    throw new ArgumentException($"Tüketim verisi geçersiz: {tuketim_value}");
                }
            }
        }

        private void ImputeTMFiderID()
        {
            // 0.01 is the 2d distance of the delta of x and y coordinates. Roughly equal to 1 km.
            const double maxDistance = 0.01;

            foreach (int missingIndex in columnNullRowsMap["TM_FIDER_ID"])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                double missingX = Convert.ToDouble(missingRow["TRAFO_X_KOORDINAT"]);
                double missingY = Convert.ToDouble(missingRow["TRAFO_Y_KOORDINAT"]);

                double closestDistance = double.MaxValue;
                DataRow closestRow = null;

                foreach (DataRow row in currentDataTable.Rows)
                {
                    if (row == missingRow || IsNullLike(row["TM_FIDER_ID"], true))
                    {
                        continue;
                    }

                    double x = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]);
                    double y = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"]);
                    double distance = Math.Sqrt(Math.Pow(missingX - x, 2) + Math.Pow(missingY - y, 2));

                    if (distance < closestDistance && distance < maxDistance)
                    {
                        closestDistance = distance;
                        closestRow = row;
                    }
                }

                if (closestRow != null)
                {
                    missingRow["TM_FIDER_ID"] = closestRow["TM_FIDER_ID"];
                    missingRow["FIDER_ADI"] = closestRow["FIDER_ADI"];
                }
                else
                {
                    missingRow["TM_FIDER_ID"] = "Fider Bulunamadı";
                }
            }
        }
        private void ReportErrorLessThanZero(string columnName)
        {
            float negativePercentage, zeroPercentage;
            int totalRows = currentDataTable.Rows.Count;
            var column = currentDataTable.Columns[columnName];
            var nullRows = new List<int>();
            var imputableRows = new List<int>();

            int negativeCount = 0;
            int zeroCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (
                    row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(row[column]?.ToString(), StringComparer.OrdinalIgnoreCase) ||
                    float.TryParse(row[column]?.ToString(), out float value) && value < 0)
                {
                    // Son yıl tüketimi 0'dan az ise
                    negativeCount++;
                    imputableRows.Add(currentDataTable.Rows.IndexOf(row));
                }
                else if (float.TryParse(row[column]?.ToString(), out float value2) && value2 == 0) 
                { 
                    zeroCount++;
                    nullRows.Add(currentDataTable.Rows.IndexOf(row));
                }
            }

            columnNullRowsMap[column.ColumnName] = nullRows;
            imputableRowsMap[column.ColumnName] = imputableRows;
            negativePercentage = (float)negativeCount / totalRows;
            zeroPercentage = (float)zeroCount / totalRows;

            if (negativePercentage > 0)
            {
                var thresholds = TUKETIM_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(negativePercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                // Append the column name and null count to the report message
                datatableLevel.Rows.Add(new object[] {
                    column.ColumnName, "Son yıl verisi", $"{negativePercentage:P1} abonenin tüketim verisi yok",
                    "Bu abonelerin tüketim verileri silinecek."
                });
            }
            if (zeroPercentage > 0)
            {
                // Append the column name and null count to the report message
                infoDataTable.Rows.Add(new object[] {
                    column.ColumnName, "Son yıl verisi", $"{zeroPercentage:P1}", "Bu trafolarda son yıl verisi yok. Tüketim verileri silinecek."
                });
            }
        }
        void ImputeAverageDate()
        {
            string dateColumn = "TRAFO_KURULUM_TARIHI";
            var dateStrings = currentDataTable.AsEnumerable()
                                       .Where(row => !string.IsNullOrEmpty(row[dateColumn]?.ToString()))
                                       .Select(row => row[dateColumn].ToString())
                                       .ToList();

            var dateTimes = dateStrings.Select(date => DateTime.ParseExact(date, DATE_FORMAT, CultureInfo.InvariantCulture))
                                       .ToList();

            if (dateTimes.Count == 0)
            {
                throw new ArgumentException("No valid dates found in the column.");
            }

            BigInteger totalTicks = dateTimes.Aggregate(BigInteger.Zero, (sum, date) => sum + date.Ticks);
            long averageTicks = (long)(totalTicks / dateTimes.Count);
            DateTime averageDate = new DateTime(averageTicks);

            string averageDateString = averageDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

            foreach (int index in columnNullRowsMap[dateColumn])
            {
                if (index >= 0 && index < currentDataTable.Rows.Count)
                {
                    currentDataTable.Rows[index][dateColumn] = averageDateString;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Index {index} is out of the valid range.");
                }
            }
        }
    }
}

