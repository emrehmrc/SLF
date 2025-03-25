using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
namespace SLF
{
    public class AboneVerileri : GirdiModülü
    {
        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "ABONE_X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "ABONE_Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
        };

        protected override List<string> Prerequisites => new List<string> { "DTR Verileri" };
        private readonly (float warningThreshold, float errorThreshold) TUKETIM_ERROR_THRESHOLD = InfoErrorBoundary(0.2f);
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.1f);
        private const float ABONE_KAPASITE_LIMIT = 0.6f;
        private const double BAGLANTI_GUCU_THRESHOLD = 30.0;

        private const double MAX_DISTANCE_IN_DEGREES = 0.001;

        private readonly string DATE_FORMAT = "yyyyMMdd";
        private readonly string SOZ_DVM = "SÃ¶z.Dvm";
        private readonly string SOZ_IPT = "SÃ¶z.Ipt";

        private bool trafoKoduRemoveFlag;

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "TESISAT_NO", InfoErrorBoundary(0.2f) },
            { "BAGLANDIGI_TRAFO_KODU", WarningErrorBoundary(0.1f) },
            { "BAGLANTI_GUCU", WarningErrorBoundary(0.4f) },
            { "ABONE_GRUBU",WarningErrorBoundary(0.2f) },
            { "ABONE_X_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "ABONE_Y_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "BINA_ID", WarningErrorBoundary(0.2f) },
            //{ "BINA_TURU", WarningErrorBoundary(0.2f) },
            //{ "SOZLESME_DURUMU", INFO_ONLY },
            //{ "GERILIM_SEVIYESI", INFO_ONLY },
            //{ "ABONE_BASLANGIC_TARIHI", INFO_ONLY },
            //{ "ABONE_BITIS_TARIHI", INFO_ONLY },
        };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dateFormatCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            //{ "ABONE_BASLANGIC_TARIHI", INFO_ONLY },
            //{ "ABONE_BITIS_TARIHI", INFO_ONLY },
        };
        private readonly List<string> duplicateFieldsGivingError = new List<string>
        {
            "TESISAT_NO",
        };
        public override void Preprocess()
        {
            trafoKoduRemoveFlag = false;
            PreprocessMismatchedTrafoKodu();
            CheckConnectivity();
        }
        public override void Postprocess()
        {
            DeferredImputeTrafoTuketimDemand();
            MessageBox.Show("DTR verilerinde eksik kalan tüketimler, abone verilerinin yüklenmesiyle birlikte dolduruldu.");
            nullFieldsCheckWithLevel["BAGLANDIGI_TRAFO_KODU"] = WarningErrorBoundary(0.1f);  // Return to the original value as you could reupload the data all over again
        }
        public override void Validate()
        {
            base.Validate();

            ReportErrorLessThanZero();
            BinaKoordinatMatchCheck();
            ReportNullCounts();
            ReportDuplicateRowCounts();
            ReportDuplicateCounts();
            ReportCoordinatesOutOfLimits();
            AboneKapasiteCheck();
            ReportDateFormatErrors();
            ReportSanalCounts();
        }

        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["TESISAT_NO"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["TESISAT_DUPLICATE"]);
            if (trafoKoduRemoveFlag)
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"]);
            }
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["BINA_TURU"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["KAPASITE"]);
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

        private void PreprocessMismatchedTrafoKodu()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            var validTrafos = new HashSet<string>(trafoDataTable.AsEnumerable()
                                      .Select(row => row["TRAFO_KODU"].ToString())
                                      .Distinct()
            );

            // Loop through currentDataTable to find invalid trafos and their indexes
            foreach (DataRow row in currentDataTable.Rows)
            {
                string connectedTrafo = row["BAGLANDIGI_TRAFO_KODU"].ToString();
                if (IsNullLike(connectedTrafo))
                {
                }
                else if (!validTrafos.Contains(connectedTrafo))
                {
                    row["BAGLANDIGI_TRAFO_KODU"] = "#N/A";
                }
            }
        }

        public override void Impute()
        {
            if (!trafoKoduRemoveFlag)
            {
                TrafoKoduImpute();
            }
            ImputeCoordinates();
            ImputeOutOfLimitCoordinates("COORDINATE_LIMITS");
            ImputeOutOfLimitCoordinates("ABONE_X_KOORDINAT");
            ImputeOutOfLimitCoordinates("ABONE_Y_KOORDINAT");
            AboneGrubuImpute();
            BaglantiGucuImpute();
            ImputeLastYearTuketim();
        }

        private void ImputeLastYearTuketim()
        {
            var column = $"YIL_TUKETIM_{lastYear}";
            var fallbackColumn = $"YIL_TUKETIM_{penultimateYear}";

            foreach (int missingIndex in imputableRowsMap[column])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                var imputedValue = missingRow[fallbackColumn];
                missingRow[column] = imputedValue;
            }
        }
        private void ImputeOutOfLimitCoordinates(string column)
        {
            //var column = "COORDINATE_LIMITS";

            foreach (int missingIndex in columnNullRowsMap[column])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                if (aboneTrafoConnectivityPass)
                {
                    var trafoKodu = missingRow["BAGLANDIGI_TRAFO_KODU"].ToString();
                    if (!IsNullLike(trafoKodu) && trafoKodu != "TO_BE_IMPUTED")
                    {
                        var trafoRow = dataTablesByType["DTR Verileri"].AsEnumerable().FirstOrDefault(r => r["TRAFO_KODU"].ToString() == trafoKodu);
                        if (trafoRow != null)
                        {
                            missingRow["ABONE_X_KOORDINAT"] = trafoRow["TRAFO_X_KOORDINAT"];
                            missingRow["ABONE_Y_KOORDINAT"] = trafoRow["TRAFO_Y_KOORDINAT"];
                        }
                        else
                        {
                            throw new ArgumentException($"Abone verileri için koordinatlar impute edilirken hata oluştu. Trafo kodu: {trafoKodu}");
                        }
                    }
                }
                else
                {
                    missingRow["ABONE_X_KOORDINAT"] = "KOORDINATI_YOK";
                    missingRow["ABONE_Y_KOORDINAT"] = "KOORDINATI_YOK";
                }
            }
        }

        private void ImputeCoordinates()
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                string adrBinaId = row["BINA_ID"].ToString();
                if (binaIdToMostFrequentCoordinates.ContainsKey(adrBinaId))
                {
                    var coordinates = binaIdToMostFrequentCoordinates[adrBinaId];
                    row["ABONE_X_KOORDINAT"] = coordinates.X;
                    row["ABONE_Y_KOORDINAT"] = coordinates.Y;
                }
            }
        }

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
        private void ReportSanalCounts()
        {
            float nullPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;
            string column = "BINA_TURU";

            var nullRows = new List<int>();

            int nullCount = 0;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (row[column].ToString() == "SANAL")
                {
                    nullCount++;
                    // Add the row number and the null-like value to the nullRows
                    nullRows.Add(i);
                }
            }

            columnNullRowsMap[column] = nullRows;

            nullPercentage = (float)nullCount / totalRows;

            if (nullPercentage > 0)
            {
                infoDataTable.Rows.Add(new object[] {
                    column, "Sanal bina", $"{nullPercentage:P1}"
                });
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
                var uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var duplicateRowIndices = new List<int>();

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value = row[column]?.ToString() ?? string.Empty;

                    if (uniqueValues.Contains(value))
                    {
                        duplicateRowIndices.Add(i);
                    }

                    uniqueValues.Add(value);
                }

                columnNullRowsMap["TESISAT_DUPLICATE"] = duplicateRowIndices;

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
            var (minXValue, maxXValue) = minMaxCheckMap["ABONE_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["ABONE_Y_KOORDINAT"];

            var nullRows = new List<int>();

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["ABONE_X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["ABONE_Y_KOORDINAT"]?.ToString(), out float valueY))
                {
                    if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                    {
                        countOutOfThresholdCoordinates++;
                        nullRows.Add(currentDataTable.Rows.IndexOf(row));
                    }
                }
            }

            columnNullRowsMap["COORDINATE_LIMITS"] = nullRows;

            float outOfThresholdPercentage = (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count;

            if (outOfThresholdPercentage > 0)
            {
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                // Add the warning to the DataTable
                datatableLevel.Rows.Add(new object[] {
                    "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
                });
            }
        }
        private void ReportErrorLessThanZero()
        {
            float nonPositivePercentage, nonLastYearPercentage;
            int totalRows = currentDataTable.Rows.Count;
            var column = currentDataTable.Columns[$"YIL_TUKETIM_{lastYear}"];
            var fallbackColumn = currentDataTable.Columns[$"YIL_TUKETIM_{penultimateYear}"];
            var nullRows = new List<int>();
            var imputableRows = new List<int>();

            int nonPositiveCount = 0;
            int nonLastYearCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (
                    row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(row[column]?.ToString(), StringComparer.OrdinalIgnoreCase) ||
                    float.TryParse(row[column]?.ToString(), out float value) && value < 0)
                {
                    // If the last year's consumption data is missing or less than or equal to zero, check the previous year's data
                    if (row.IsNull(fallbackColumn) ||
                        row[fallbackColumn] == DBNull.Value ||
                        nullLikeStrings.Contains(row[fallbackColumn]?.ToString(), StringComparer.OrdinalIgnoreCase) ||
                        float.TryParse(row[fallbackColumn]?.ToString(), out float fallbackValue) && fallbackValue < 0
                    )
                    {
                        nonPositiveCount++;
                        nullRows.Add(currentDataTable.Rows.IndexOf(row));
                    }
                    else if (row["SOZLESME_DURUMU"].ToString() == SOZ_DVM)
                    {
                        nonLastYearCount++;
                        imputableRows.Add(currentDataTable.Rows.IndexOf(row));
                    }
                }
            }

            columnNullRowsMap[column.ColumnName] = nullRows;
            imputableRowsMap[column.ColumnName] = imputableRows;
            nonPositivePercentage = (float)nonPositiveCount / totalRows;
            nonLastYearPercentage = (float)nonLastYearCount / totalRows;

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
            if (nonLastYearPercentage > 0)
            {
                // Append the column name and null count to the report message
                warningDataTable.Rows.Add(new object[] {
                    column.ColumnName, "Son yıl tüketim verisi", $"{nonLastYearPercentage:P1}", "Bu abonelerin son yıl tüketim verisi yok. Tüketim verileri geçmiş veriler ile doldurulacak."
                });
            }
        }
        private void AboneKapasiteCheck()
        {
            // yillik tuketim / 8760 / baglanti gucu
            int overCapacityCount = 0;
            int totalRows = currentDataTable.Rows.Count;
            var lastYearTuketim = currentDataTable.Columns[$"YIL_TUKETIM_{lastYear}"];
            var nullRows = new List<int>();
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
                            nullRows.Add(currentDataTable.Rows.IndexOf(row));
                        }
                    }
                }
            }

            columnNullRowsMap["KAPASITE"] = nullRows;
            float overCapacityPercentage = (float)overCapacityCount / totalRows;

            if (overCapacityPercentage > 0)
            {
                infoDataTable.Rows.Add(new object[]
                {
                 "", "Abone kapasitesi", $"{overCapacityPercentage:P1}",
                 $"Abone kapasitesi {ABONE_KAPASITE_LIMIT:P1}'den büyük olan abonelerin tüketim verileri silinecek."
                });
            }

        }
        private void BinaKoordinatMatchCheck()
        {
            var grouped = currentDataTable.AsEnumerable().GroupBy(row => row["BINA_ID"]);

            int nonUniqueCount = 0;

            binaIdToMostFrequentCoordinates.Clear();

            foreach (var group in grouped)
            {
                // Group by original coordinates to determine the most frequent coordinate
                var coordinateGroups = group
                    .Select(row => new
                    {
                        X = double.TryParse(row["ABONE_X_KOORDINAT"].ToString(), out double x) ? (double?)x : null,
                        Y = double.TryParse(row["ABONE_Y_KOORDINAT"].ToString(), out double y) ? (double?)y : null
                    })
                    .Where(coord => coord.X.HasValue && coord.Y.HasValue)
                    .GroupBy(coord => new { coord.X, coord.Y })
                    .OrderByDescending(g => g.Count())
                    .ToList();

                if (coordinateGroups.Count == 0)
                {
                    continue;
                }

                var mostFrequentGroup = coordinateGroups.First();
                var mostFrequentPair = mostFrequentGroup.Key;

                // Check for distinct coordinates based on precision
                var distinctCoordinates = coordinateGroups
                    .Select(g => new
                    {
                        X = Math.Round(g.Key.X.Value, COORDINATE_ROUNDING_PRECISION),
                        Y = Math.Round(g.Key.Y.Value, COORDINATE_ROUNDING_PRECISION)
                    })
                    .Distinct()
                    .ToList();

                if (distinctCoordinates.Count > 1)
                {
                    nonUniqueCount++;
                    binaIdToMostFrequentCoordinates[(string)group.Key] = (mostFrequentPair.X.Value, mostFrequentPair.Y.Value);
                }
            }


            float nonUniquePercentage = (float)nonUniqueCount / grouped.Count();
            if (nonUniquePercentage > 0)
            {
                warningDataTable.Rows.Add(new object[]
                {
                 "X & Y KOORDINAT", "Bina koordinatları", $"{nonUniquePercentage:P1}",
                 "Bazı bina koordinatları farklıdır. Bu durumda o bina için en çok tekrar eden koordinatlar kullanılacaktır."
                });
            }
        }
        private void AboneGrubuImpute()
        {
            var grouped = currentDataTable.AsEnumerable().GroupBy(row => row["BINA_ID"]);

            aboneGrubuMostFrequent.Clear();

            foreach (var group in grouped)
            {
                // Group by original coordinates to determine the most frequent coordinate
                var mostFrequentAboneGrubu = group
                    .GroupBy(row => row["ABONE_GRUBU"].ToString())
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (mostFrequentAboneGrubu != null)
                {
                    aboneGrubuMostFrequent[(string)group.Key] = mostFrequentAboneGrubu.Key;
                }
            }

            foreach (DataRow row in currentDataTable.Rows)
            {
                string aboneGrubu = row["ABONE_GRUBU"].ToString();
                string binaId = row["BINA_ID"].ToString();
                if (IsNullLike(aboneGrubu))
                {
                    var imputedGrup = aboneGrubuMostFrequent[binaId];
                    row["ABONE_GRUBU"] = imputedGrup;
                }
            }
        }
        private void BaglantiGucuImpute()
        {
            // Dictionary to hold the most frequent BAGLANTI_GUCU for each combination of ADR_BINA_ID and ABONE_GRUBU
            var baglantiGucuMostFrequent = new Dictionary<string, string>();

            // Group by ADR_BINA_ID and ABONE_GRUBU
            var grouped = currentDataTable.AsEnumerable()
                .GroupBy(row => new
                {
                    AdrBinaId = row["BINA_ID"].ToString(),
                    AboneGrubu = row["ABONE_GRUBU"].ToString()
                });

            // Find the most frequent BAGLANTI_GUCU for each combination of ADR_BINA_ID and ABONE_GRUBU
            foreach (var group in grouped)
            {
                var mostFrequentBaglantiGucu = group
                    .GroupBy(row => row["BAGLANTI_GUCU"].ToString())
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (mostFrequentBaglantiGucu != null)
                {
                    var key = $"{group.Key.AdrBinaId}_{group.Key.AboneGrubu}";
                    baglantiGucuMostFrequent[key] = mostFrequentBaglantiGucu.Key;
                }
            }

            // Impute the missing BAGLANTI_GUCU values in the DataTable
            foreach (DataRow row in currentDataTable.Rows)
            {
                string aboneGrubu = row["ABONE_GRUBU"].ToString();
                string binaId = row["BINA_ID"].ToString();
                string baglantiGucu = row["BAGLANTI_GUCU"].ToString();
                var key = $"{binaId}_{aboneGrubu}";

                if (IsNullLike(baglantiGucu) && baglantiGucuMostFrequent.ContainsKey(key))
                {
                    row["BAGLANTI_GUCU"] = baglantiGucuMostFrequent[key];
                }
            }
        }
        private void TrafoKoduImpute()
        {
            nullFieldsCheckWithLevel["BAGLANDIGI_TRAFO_KODU"] = InfoErrorBoundary(0.01f);  // Stricter threshold for TrafoKodu
            trafoKoduRemoveFlag = true;
            // 0.001 is the 2d distance of the delta of x and y coordinates. Roughly equal to 100m.

            // Create a list of non-null rows with their coordinates
            var nonNullRows = currentDataTable.AsEnumerable()
                                              .Where(row => !IsNullLike(row["BAGLANDIGI_TRAFO_KODU"], true))
                                              .Select(row => new
                                              {
                                                  Row = row,
                                                  X = TryParseDouble(row["ABONE_X_KOORDINAT"].ToString(), 0),
                                                  Y = TryParseDouble(row["ABONE_Y_KOORDINAT"].ToString(), 0)
                                              })
                                              .ToList();

            // Sort non-null rows by X coordinate
            nonNullRows.Sort((a, b) => a.X.CompareTo(b.X));

            foreach (int missingIndex in columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"])
            {
                var missingRow = currentDataTable.Rows[missingIndex];
                double missingX = TryParseDouble(missingRow["ABONE_X_KOORDINAT"].ToString(), 0);
                double missingY = TryParseDouble(missingRow["ABONE_Y_KOORDINAT"].ToString(), 0);

                double closestDistance = double.MaxValue;
                var closestRow = default(dynamic);

                // Use binary search to find the position of the missing row by X coordinate
                int position = nonNullRows.BinarySearch(new { Row = (DataRow)null, X = missingX, Y = 0.0 },
                                                        Comparer<dynamic>.Create((a, b) => a.X.CompareTo(b.X)));

                // Rest of the function remains the same
                // ...
            }
        }

        // Helper method to safely parse double values
        private double TryParseDouble(string value, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value) || IsNullLike(value))
                return defaultValue;

            if (double.TryParse(value, System.Globalization.NumberStyles.Any,
                               System.Globalization.CultureInfo.InvariantCulture, out double result))
                return result;

            if (double.TryParse(value, System.Globalization.NumberStyles.Any,
                               System.Globalization.CultureInfo.CurrentCulture, out result))
                return result;

            return defaultValue;
        }
        private void CheckConnectivity()
        {
            var trafoDataTable = dataTablesByType["DTR Verileri"];
            // Construct the column name for the last year consumption
            string consumptionColumn = $"YIL_TUKETIM_{lastYear}";

            // Create a dictionary to hold the grouped and summed results
            Dictionary<string, double> trafoDictionary = new Dictionary<string, double>();
            Dictionary<string, double> aboneDictionary = new Dictionary<string, double>();

            foreach (DataRow row in trafoDataTable.Rows)
            {
                // Get the key value (TRAFO_KODU)
                string key = row["TRAFO_KODU"].ToString();

                // Get the consumption value, ensuring proper type conversion and handling of DBNull
                double consumption = 0;
                if (row[consumptionColumn] != DBNull.Value && row[consumptionColumn].ToString() != TO_BE_IMPUTED_STRING)
                {
                    string consumptionStr = row[consumptionColumn].ToString();
                    if (!double.TryParse(consumptionStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out consumption))
                    {
                        // If parsing with InvariantCulture fails, try with CurrentCulture
                        double.TryParse(consumptionStr, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.CurrentCulture, out consumption);
                        // If both fail, consumption will remain 0
                    }
                }

                // Add the consumption value to the corresponding key in the dictionary
                if (trafoDictionary.ContainsKey(key))
                {
                    trafoDictionary[key] += consumption;
                }
                else
                {
                    trafoDictionary[key] = consumption;
                }
            }

            // Iterate through each row in the DataTable
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Get the key value (BAGLANDIGI_TRAFO_KODU)
                string key = row["BAGLANDIGI_TRAFO_KODU"].ToString();

                // Get the consumption value, ensuring proper type conversion and handling of DBNull
                double consumption = 0;
                if (row[consumptionColumn] != DBNull.Value)
                {
                    string consumptionStr = row[consumptionColumn].ToString();
                    if (!double.TryParse(consumptionStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out consumption))
                    {
                        // If parsing with InvariantCulture fails, try with CurrentCulture
                        double.TryParse(consumptionStr, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.CurrentCulture, out consumption);
                        // If both fail, consumption will remain 0
                    }
                }

                // Add the consumption value to the corresponding key in the dictionary
                if (aboneDictionary.ContainsKey(key))
                {
                    aboneDictionary[key] += consumption;
                }
                else
                {
                    aboneDictionary[key] = consumption;
                }
            }

            int connectivityPassCount = 0;
            foreach (var kvp in trafoDictionary)
            {
                var totalKeyCount = trafoDictionary.Count;
                var trafoToplam = kvp.Value;
                var aboneToplam = aboneDictionary.ContainsKey(kvp.Key) ? aboneDictionary[kvp.Key] : 0;
                if (trafoToplam * 0.9 <= aboneToplam && aboneToplam <= trafoToplam)
                {
                    connectivityPassCount++;
                }
            }
            var connectivityPassPercentage = (float)connectivityPassCount / trafoDictionary.Count;
            aboneTrafoConnectivityPass = connectivityPassPercentage > 0.95;
        }

        public void DeferredImputeTrafoTuketimDemand()
        {
            var trafoDataTable = dataTablesByType["DTR Verileri"];
            const double maxDistance = double.MaxValue;  // 0.005;
            var demandColumn = $"YIL_DEMANT_{lastYear}";
            var kapasiteColumn = "TRAFO_KAPASITESI";
            var tuketimColumn = $"YIL_TUKETIM_{lastYear}";

            var deferredImputableRows = new List<int>();

            foreach (DataRow row in trafoDataTable.Rows)
            {
                if (row[tuketimColumn].ToString() == TO_BE_IMPUTED_STRING && row[demandColumn].ToString() == TO_BE_IMPUTED_STRING)
                {
                    deferredImputableRows.Add(trafoDataTable.Rows.IndexOf(row));
                }
            }

            foreach (int missingIndex in deferredImputableRows)
            {
                var missingRow = trafoDataTable.Rows[missingIndex];
                var trafoKodu = missingRow["TRAFO_KODU"];
                double imputedValue;
                double imputedDemandValue;
                if (aboneTrafoConnectivityPass)
                {
                    double sumOfTrafo = 0;
                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        if (row["BAGLANDIGI_TRAFO_KODU"].ToString() == trafoKodu.ToString())
                        {
                            sumOfTrafo += Convert.ToDouble(row[$"YIL_TUKETIM_{lastYear}"]);
                        }
                    }
                    var aboneGrubu = missingRow["ABONE_GRUBU"].ToString();
                    var kFactorForTheGrup = K_FACTOR; // kFactorByAboneGrubu[aboneGrubu];
                    imputedValue = 1.03 * sumOfTrafo;
                    imputedDemandValue = kFactorForTheGrup * imputedValue / HoursInYear;
                }
                else // if (!aboneTrafoConnectivityPass)
                {
                    double missingX = Convert.ToDouble(missingRow["TRAFO_X_KOORDINAT"]);
                    double missingY = Convert.ToDouble(missingRow["TRAFO_Y_KOORDINAT"]);
                    List<(DataRow row, double distance)> closestRows = new List<(DataRow, double)>();
                    foreach (DataRow row in trafoDataTable.Rows)
                    {
                        if (row == missingRow || IsNullLike(row[demandColumn], true) || IsNullLike(row[kapasiteColumn], true) || row[demandColumn].ToString() == TO_BE_IMPUTED_STRING)
                        {
                            continue;
                        }
                        double x = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]);
                        double y = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"]);
                        double distance = Math.Sqrt(Math.Pow(missingX - x, 2) + Math.Pow(missingY - y, 2));
                        if (distance < maxDistance)
                        {
                            closestRows.Add((row, distance));
                        }
                    }
                    // Sort the list by distance
                    closestRows.Sort((a, b) => a.distance.CompareTo(b.distance));

                    // Take the three closest rows
                    var top3ClosestRows = closestRows.Take(3).ToList();

                    if (top3ClosestRows.Count > 0)
                    {
                        // Here, you can decide how to use these three closest rows to impute the value
                        double totalLoad = 0;

                        foreach (var (row, _) in top3ClosestRows)
                        {
                            double rowDemand = Convert.ToDouble(row[demandColumn]);
                            double rowKapasite = Convert.ToDouble(row[kapasiteColumn]);
                            //double rowTuketim = Convert.ToDouble(row[tuketimColumn]);

                            double load = rowDemand / rowKapasite;

                            totalLoad += load;
                        }

                        double averageLoad = totalLoad / top3ClosestRows.Count;

                        double missingKapasite = Convert.ToDouble(missingRow[kapasiteColumn]);

                        imputedDemandValue = averageLoad * missingKapasite;
                        imputedValue = imputedDemandValue * HoursInYear / K_FACTOR;
                        missingRow[demandColumn] = imputedValue;
                    }
                    else
                    {
                        imputedValue = 99999; // No close enough rows found
                        imputedDemandValue = 99999; // No close enough rows found
                        MessageBox.Show("No close enough rows found");
                    }
                }
                missingRow[tuketimColumn] = imputedValue;
                missingRow[demandColumn] = imputedDemandValue;
            }
        }
    }
}

