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
            BinaKoordinatMatchCheck(); // Moved here
            CheckConnectivity();
            InitializeColumnNullRowsMap();
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
            // BinaKoordinatMatchCheck(); // Removed from here
            ReportNullCounts();
            ReportDuplicateRowCounts();
            ReportDuplicateCounts();
            // ReportCoordinatesOutOfLimits();
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
            var imputedRows = new List<int>();
            var failedRows = new List<int>();

            if (!columnNullRowsMap.ContainsKey(column))
            {
                System.Diagnostics.Debug.WriteLine($"No null rows found for column: {column}");
                return;
            }

            foreach (int missingIndex in columnNullRowsMap[column])
            {
                // Validate the index before accessing the row
                if (missingIndex < 0 || missingIndex >= currentDataTable.Rows.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"Invalid row index {missingIndex} for column {column}. Current row count: {currentDataTable.Rows.Count}");
                    continue; // Skip invalid indices
                }

                var missingRow = currentDataTable.Rows[missingIndex];
                var trafoKodu = missingRow["BAGLANDIGI_TRAFO_KODU"]?.ToString();
                if (!IsNullLike(trafoKodu) && trafoKodu != "TO_BE_IMPUTED")
                {
                    var trafoRow = dataTablesByType["DTR Verileri"].AsEnumerable().FirstOrDefault(r => r["TRAFO_KODU"].ToString() == trafoKodu);
                    if (trafoRow != null)
                    {
                        if (float.TryParse(trafoRow["TRAFO_X_KOORDINAT"]?.ToString(), out float trafoX) &&
                            float.TryParse(trafoRow["TRAFO_Y_KOORDINAT"]?.ToString(), out float trafoY))
                        {
                            missingRow["ABONE_X_KOORDINAT"] = trafoX;
                            missingRow["ABONE_Y_KOORDINAT"] = trafoY;
                            imputedRows.Add(missingIndex);
                            continue;
                        }
                    }
                }
                missingRow["ABONE_X_KOORDINAT"] = DBNull.Value;
                missingRow["ABONE_Y_KOORDINAT"] = DBNull.Value;
                failedRows.Add(missingIndex);
            }

            if (imputedRows.Any())
            {
                infoDataTable.Rows.Add(new object[] {
            "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT",
            "Koordinat Imputasyonu",
            $"{imputedRows.Count} satır",
            $"DTR koordinatları kullanılarak abone koordinatları dolduruldu. (Satır: {string.Join(", ", imputedRows)})"
        });
            }
            if (failedRows.Any())
            {
                infoDataTable.Rows.Add(new object[] {
            "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT",
            "Koordinat Imputasyonu Başarısız",
            $"{failedRows.Count} satır",
            $"Abone koordinatları doldurulamadı, null olarak işaretlendi. (Satır: {string.Join(", ", failedRows)})"
        });
            }
        }
        private void InitializeColumnNullRowsMap()
        {
            columnNullRowsMap = new Dictionary<string, List<int>>();

            // List of columns that ImputeOutOfLimitCoordinates will check
            var columnsToCheck = new List<string> { "COORDINATE_LIMITS", "ABONE_X_KOORDINAT", "ABONE_Y_KOORDINAT" };

            // Initialize dictionary with empty lists for each column
            foreach (var column in columnsToCheck)
            {
                columnNullRowsMap[column] = new List<int>();
            }

            // Populate the dictionary by scanning the DataTable for null values
            for (int rowIndex = 0; rowIndex < currentDataTable.Rows.Count; rowIndex++)
            {
                var row = currentDataTable.Rows[rowIndex];
                foreach (var column in columnsToCheck)
                {
                    // Skip COORDINATE_LIMITS as it might not be a real column in the DataTable
                    if (column == "COORDINATE_LIMITS") continue;

                    if (row[column] == DBNull.Value || row[column] == null)
                    {
                        columnNullRowsMap[column].Add(rowIndex);
                    }
                }
            }

            // Special handling for COORDINATE_LIMITS if it represents rows with out-of-limit coordinates
            // This depends on your application's logic for COORDINATE_LIMITS
            columnNullRowsMap["COORDINATE_LIMITS"] = new List<int>();
            for (int rowIndex = 0; rowIndex < currentDataTable.Rows.Count; rowIndex++)
            {
                var row = currentDataTable.Rows[rowIndex];
                if (row["ABONE_X_KOORDINAT"] != DBNull.Value && row["ABONE_Y_KOORDINAT"] != DBNull.Value)
                {
                    if (float.TryParse(row["ABONE_X_KOORDINAT"]?.ToString(), out float x) &&
                        float.TryParse(row["ABONE_Y_KOORDINAT"]?.ToString(), out float y))
                    {
                        // Define your coordinate limits (example)
                        if (x < -180 || x > 180 || y < -90 || y > 90) // Adjust limits as needed
                        {
                            columnNullRowsMap["COORDINATE_LIMITS"].Add(rowIndex);
                        }
                    }
                }
            }

            // Debug log to verify initialization
            foreach (var kvp in columnNullRowsMap)
            {
                System.Diagnostics.Debug.WriteLine($"Column {kvp.Key} has {kvp.Value.Count} null/out-of-limit rows.");
            }
        }

        private void ImputeCoordinates()
        {
            var imputedRows = new List<int>();
            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                string adrBinaId = row["BINA_ID"]?.ToString();
                if (!string.IsNullOrEmpty(adrBinaId) && binaIdToMostFrequentCoordinates.ContainsKey(adrBinaId))
                {
                    bool isInvalid = !float.TryParse(row["ABONE_X_KOORDINAT"]?.ToString(), out float valueX) ||
                                     !float.TryParse(row["ABONE_Y_KOORDINAT"]?.ToString(), out float valueY);
                    if (isInvalid)
                    {
                        var coordinates = binaIdToMostFrequentCoordinates[adrBinaId];
                        row["ABONE_X_KOORDINAT"] = coordinates.X;
                        row["ABONE_Y_KOORDINAT"] = coordinates.Y;
                        imputedRows.Add(rowIndex);
                    }
                }
            }
            if (imputedRows.Any())
            {
                infoDataTable.Rows.Add(new object[] {
            "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT",
            "Koordinat Imputasyonu",
            $"{imputedRows.Count} satır",
            $"Bina koordinatları kullanılarak abone koordinatları dolduruldu. (Satır: {string.Join(", ", imputedRows)})"
        });
            }
        }

        private void ReportNullCounts()
        {
            float percentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            // Flag to track if we've already processed the coordinates
            bool coordinatesProcessed = false;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }

                List<int> invalidRows = new List<int>();
                int invalidCount = 0;

                // Delegate to specialized functions for specific columns
                if (column.ColumnName == "BAGLANDIGI_TRAFO_KODU")
                {
                    TrafoKoduImpute(); // Handle BAGLANDIGI_TRAFO_KODU separately
                    continue;
                }
                else if (column.ColumnName == "BAGLANTI_GUCU")
                {
                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        if (IsNullLike(row[column]))
                        {
                            invalidCount++;
                            invalidRows.Add(i);
                        }
                        else
                        {
                            if (double.TryParse(row[column].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                            {
                                if (value <= 0)
                                {
                                    invalidCount++;
                                    invalidRows.Add(i);
                                }
                            }
                            else
                            {
                                invalidCount++;
                                invalidRows.Add(i);
                            }
                        }
                    }

                    columnNullRowsMap[column.ColumnName] = invalidRows;

                    percentage = (float)invalidCount / totalRows;

                    if (percentage > 0)
                    {
                        const float errorThreshold = 0.4f; // 40%

                        if (percentage > errorThreshold)
                        {
                            string errorMessage = $"Hata Mesajı: Kullanıcıya %40’dan fazla oranda Abone Bağlantı Gücü yok veya 0’dan küçük hatası. (Satır: {string.Join(", ", invalidRows)})";
                            errorDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Bağlantı Gücü",
                        $"{percentage:P1}",
                        errorMessage
                            });
                        }
                        else
                        {
                            string warningMessage = $"Düzeltilecekler Mesajı: Kullanıcıya Abone Bağlantı Gücü NULL veya geçersiz olan verilere veri doldurma uygulanacaktır. (Satır: {string.Join(", ", invalidRows)})";
                            warningDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Bağlantı Gücü",
                        $"{percentage:P1}",
                        warningMessage
                            });
                        }
                    }
                }
                else if (column.ColumnName == "TESISAT_NO")
                {
                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        var tesisatNo = Convert.ToString(row[column]);
                        if (IsNullLike(tesisatNo) || tesisatNo == "0")
                        {
                            invalidCount++;
                            invalidRows.Add(i);
                        }
                        else
                        {
                            if (!int.TryParse(tesisatNo, out int value) || value <= 0)
                            {
                                invalidCount++;
                                invalidRows.Add(i);
                            }
                        }
                    }

                    columnNullRowsMap[column.ColumnName] = invalidRows;

                    percentage = (float)invalidCount / totalRows;

                    if (percentage > 0)
                    {
                        const float errorThreshold = 0.2f; // 20%

                        if (percentage >= errorThreshold)
                        {
                            string errorMessage = $"Hata Mesajı: %20’den fazla oranda Abone Bağlantı Grubu verisi yok, hatalı veri. (Satır: {string.Join(", ", invalidRows)})";
                            errorDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Tesisat No",
                        $"{percentage:P1}",
                        errorMessage
                            });
                        }
                        else
                        {
                            string warningMessage = $"Silinecekler Mesajı: Tesisat No 0, NULL veya geçersiz formatta olan veriler silinecektir. (Satır: {string.Join(", ", invalidRows)})";
                            warningDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Geçersiz Tesisat No",
                        $"{percentage:P1}",
                        warningMessage
                            });
                        }
                    }
                }
                else if (column.ColumnName == "ABONE_GRUBU")
                {
                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        if (IsNullLike(row[column]))
                        {
                            invalidCount++;
                            invalidRows.Add(i);
                        }
                    }

                    columnNullRowsMap[column.ColumnName] = invalidRows;

                    percentage = (float)invalidCount / totalRows;

                    if (percentage > 0)
                    {
                        const float errorThreshold = 0.2f; // 20%

                        if (percentage > errorThreshold)
                        {
                            string errorMessage = $"Hata Mesajı: Kullanıcıya %20’den fazla oranda {column.ColumnName} verisi yok. hatalı veri. (Satır: {string.Join(", ", invalidRows)})";
                            errorDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Abone Grubu",
                        $"{percentage:P1}",
                        errorMessage
                            });
                        }
                        else
                        {
                            string warningMessage = $"Düzeltilecekler Mesajı: Kullanıcıya Abone Grubu NULL olan verilerin oranı uygun yöntemlerle doldurulacaktır. (Satır: {string.Join(", ", invalidRows)})";
                            warningDataTable.Rows.Add(new object[]
                            {
                        column.ColumnName,
                        "Abone Grubu",
                        $"{percentage:P1}",
                        warningMessage
                            });
                        }
                    }
                }
                // Handle coordinate columns (ABONE_X_KOORDINAT and ABONE_Y_KOORDINAT) together
                else if (column.ColumnName == "ABONE_X_KOORDINAT" || column.ColumnName == "ABONE_Y_KOORDINAT")
                {
                    // Skip if we've already processed the coordinates
                    if (coordinatesProcessed)
                    {
                        continue;
                    }

                    // Mark as processed to prevent re-processing
                    coordinatesProcessed = true;

                    // Validate coordinates and populate columnNullRowsMap["COORDINATE_LIMITS"]
                    ReportCoordinatesOutOfLimits();

                    // Check the percentage of invalid coordinates
                    invalidCount = columnNullRowsMap["COORDINATE_LIMITS"].Count;
                    percentage = totalRows > 0 ? (float)invalidCount / totalRows : 0.0f;

                    // If the percentage of invalid coordinates is 10% or less, proceed with imputation
                    if (percentage <= COORDINATE_ERROR_THRESHOLD.errorThreshold)
                    {
                        // Impute using building coordinates first
                        ImputeCoordinates();

                        // Recheck for remaining invalid coordinates after building imputation
                        var remainingInvalidRows = new List<int>();
                        foreach (DataRow row in currentDataTable.Rows)
                        {
                            int rowIndex = currentDataTable.Rows.IndexOf(row);
                            if (!columnNullRowsMap["COORDINATE_LIMITS"].Contains(rowIndex) &&
                                (!float.TryParse(row["ABONE_X_KOORDINAT"]?.ToString(), out float valueX) ||
                                 !float.TryParse(row["ABONE_Y_KOORDINAT"]?.ToString(), out float valueY)))
                            {
                                remainingInvalidRows.Add(rowIndex);
                            }
                        }
                        columnNullRowsMap["COORDINATE_LIMITS"].AddRange(remainingInvalidRows);
                        if (remainingInvalidRows.Any())
                        {
                            infoDataTable.Rows.Add(new object[] {
                        "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT",
                        "Koordinat Kontrolü",
                        $"{remainingInvalidRows.Count} satır",
                        $"Bina koordinatları ile doldurulduktan sonra hala geçersiz koordinatlar tespit edildi. (Satır: {string.Join(", ", remainingInvalidRows)})"
                    });
                        }

                        // Impute using DTR coordinates as a fallback
                        ImputeOutOfLimitCoordinates("COORDINATE_LIMITS");
                    }
                }
                else
                {
                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        if (IsNullLike(row[column]))
                        {
                            invalidCount++;
                            invalidRows.Add(i);
                        }
                    }

                    columnNullRowsMap[column.ColumnName] = invalidRows;

                    percentage = (float)invalidCount / totalRows;

                    if (percentage > 0)
                    {
                        var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                        var datatableLevel = GetDataTableBasedOnThreshold(percentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        datatableLevel.Rows.Add(new object[] {
                    column.ColumnName, "Null değer", $"{percentage:P1}"
                });
                    }
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
                int nonNullOrEmptyCount = 0; // Count of rows with non-NULL, non-empty values

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value = row[column]?.ToString();

                    // Skip NULL or empty values
                    if (IsNullLike(value))
                    {
                        continue;
                    }

                    nonNullOrEmptyCount++; // Increment count of valid rows

                    if (uniqueValues.Contains(value))
                    {
                        duplicateRowIndices.Add(i);
                    }

                    uniqueValues.Add(value);
                }

                columnNullRowsMap["TESISAT_DUPLICATE"] = duplicateRowIndices;

                // Calculate the number of unique values and duplicates among non-NULL, non-empty rows
                int uniqueCount = uniqueValues.Count;
                int duplicateCount = nonNullOrEmptyCount - uniqueCount;
                duplicatePercentage = nonNullOrEmptyCount > 0 ? (float)duplicateCount / nonNullOrEmptyCount : 0.0f;

                if (duplicateCount > 0)
                {
                    // Append the column name and unique count to the report message
                    string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} sütununda mükerrer veriler silinecektir. (Satır: {string.Join(", ", duplicateRowIndices)})";
                    warningDataTable.Rows.Add(new object[] {
                column.ColumnName, "Mükerrer hücre değerleri", $"{duplicatePercentage:P1}", warningMessage
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
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                bool isInvalid = false;

                if (!float.TryParse(row["ABONE_X_KOORDINAT"]?.ToString(), out float valueX) ||
                    !float.TryParse(row["ABONE_Y_KOORDINAT"]?.ToString(), out float valueY))
                {
                    isInvalid = true; // Non-numeric or null values
                }
                else if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                {
                    isInvalid = true; // Out of bounds
                }

                if (isInvalid)
                {
                    countOutOfThresholdCoordinates++;
                    nullRows.Add(rowIndex);
                }
            }

            columnNullRowsMap["COORDINATE_LIMITS"] = nullRows;

            float outOfThresholdPercentage = currentDataTable.Rows.Count > 0
                ? (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count
                : 0.0f;

            if (outOfThresholdPercentage > 0)
            {
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                datatableLevel.Rows.Add(new object[] {
            "ABONE_X_KOORDINAT & ABONE_Y_KOORDINAT",
            "Koordinat Sınırları",
            $"{outOfThresholdPercentage:P1}",
            $"Bazı konum bilgileri yanlış veya eksiktir. Bu durumda o bina için en çok tekrar eden koordinatlar kullanılacaktır. (Satır: {string.Join(", ", nullRows)})"
        });
            }
        }

        private void ReportErrorLessThanZero()
        {
            float nonPositivePercentage, nonLastYearPercentage;
            int totalRows = currentDataTable.Rows.Count;
            var column = currentDataTable.Columns[$"YIL_TUKETIM_{lastYear}"];
            var fallbackColumn = currentDataTable.Columns[$"YIL_TUKETIM_{penultimateYear}"];
            var nullRows = new List<int>(); // Rows to be deleted
            var imputableRows = new List<int>(); // Rows to be imputed

            int nonPositiveCount = 0;
            int nonLastYearCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                string valueString = row[column]?.ToString();

                // Check for problematic current year's data
                bool isProblematic = false;
                if (row.IsNull(column) ||
                    row[column] == DBNull.Value ||
                    nullLikeStrings.Contains(valueString, StringComparer.OrdinalIgnoreCase))
                {
                    isProblematic = true;
                }
                else if (!float.TryParse(valueString, out float value))
                {
                    // Invalid format (non-numeric value)
                    isProblematic = true;
                }
                else if (value < 0)
                {
                    // Negative value
                    isProblematic = true;
                }

                if (isProblematic)
                {
                    string fallbackValueString = row[fallbackColumn]?.ToString();

                    // Check fallback data
                    if (row.IsNull(fallbackColumn) ||
                        row[fallbackColumn] == DBNull.Value ||
                        nullLikeStrings.Contains(fallbackValueString, StringComparer.OrdinalIgnoreCase) ||
                        (float.TryParse(fallbackValueString, out float fallbackValue) && fallbackValue < 0))
                    {
                        // Fallback data is invalid, mark for deletion
                        nonPositiveCount++;
                        nullRows.Add(rowIndex);
                    }
                    else if (row["SOZLESME_DURUMU"].ToString() == SOZ_DVM)
                    {
                        // Fallback data is valid and contract allows imputation
                        nonLastYearCount++;
                        imputableRows.Add(rowIndex);
                    }
                    else
                    {
                        // Fallback data is valid but contract doesn't allow imputation, mark for deletion
                        nonPositiveCount++;
                        nullRows.Add(rowIndex);
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

                datatableLevel.Rows.Add(new object[] {
            column.ColumnName, "Son yıl tüketim verisi", $"{nonPositivePercentage:P1} abonenin tüketim ve veri doldurma için verisi yok",
            "Bu abonelerin tüketim verileri silinecek."
        });
            }
            if (nonLastYearPercentage > 0)
            {
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
            var invalidKapasiteRows = new List<int>(); // To track rows with invalid kapasite values

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row[lastYearTuketim]?.ToString(), out float tuketim) && tuketim > 0)
                {
                    var baglantiGucu = row["BAGLANTI_GUCU"];
                    if (float.TryParse(baglantiGucu?.ToString(), out float guc) && guc > 0)
                    {
                        float kapasite = (tuketim / HoursInYear) / guc;

                        // Format check for kapasite
                        if (float.IsNaN(kapasite) || float.IsInfinity(kapasite) || kapasite > 10.0f) // 1000% sanity check
                        {
                            invalidKapasiteRows.Add(currentDataTable.Rows.IndexOf(row));
                            continue;
                        }

                        if (kapasite > ABONE_KAPASITE_LIMIT)
                        {
                            overCapacityCount++;
                            nullRows.Add(currentDataTable.Rows.IndexOf(row));
                        }
                    }
                }
            }

            // Log rows with invalid kapasite values
            if (invalidKapasiteRows.Any())
            {
                infoDataTable.Rows.Add(new object[]
                {
                    "", "Abone kapasitesi", "Geçersiz Değer",
                    $"Abone kapasitesi geçersiz veya aşırı büyük (>{10.0f:P0}) olduğu için bazı satırlar atlandı. (Satır: {string.Join(", ", invalidKapasiteRows)})"
                });
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
            var grouped = currentDataTable.AsEnumerable()
                .Where(row => !IsNullLike(row["BINA_ID"]?.ToString()))
                .GroupBy(row => row["BINA_ID"].ToString());

            int nonUniqueCount = 0;
            var inconsistentBinaIds = new List<string>();
            var binaIdsWithNoValidCoordinates = new List<string>();

            binaIdToMostFrequentCoordinates.Clear();

            foreach (var group in grouped)
            {
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
                    binaIdsWithNoValidCoordinates.Add((string)group.Key);
                    continue;
                }

                var mostFrequentGroup = coordinateGroups.First();
                var mostFrequentPair = mostFrequentGroup.Key;
                binaIdToMostFrequentCoordinates[(string)group.Key] = ((float)mostFrequentPair.X.Value, (float)mostFrequentPair.Y.Value);

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
                    inconsistentBinaIds.Add((string)group.Key);
                }
            }

            float nonUniquePercentage = grouped.Any()
                ? (float)nonUniqueCount / grouped.Count()
                : 0.0f;

            if (nonUniquePercentage > 0)
            {
                warningDataTable.Rows.Add(new object[]
                {
            "X & Y KOORDINAT",
            "Bina koordinatları",
            $"{nonUniquePercentage:P1}",
            $"Bazı bina koordinatları farklıdır. Bu durumda o bina için en çok tekrar eden koordinatlar kullanılacaktır. (Bina ID'leri: {string.Join(", ", inconsistentBinaIds)})"
                });
            }

            if (binaIdsWithNoValidCoordinates.Any())
            {
                infoDataTable.Rows.Add(new object[]
                {
            "BINA_ID",
            "Bina koordinatları",
            $"{binaIdsWithNoValidCoordinates.Count} bina",
            $"Bazı binaların hiçbir abonesinde geçerli koordinat bulunamadı. (Bina ID'leri: {string.Join(", ", binaIdsWithNoValidCoordinates)})"
                });
            }
        }

        private void AboneGrubuImpute()
        {
            string columnName = "ABONE_GRUBU";
            int totalRows = currentDataTable.Rows.Count;
            var nullRows = new List<int>();
            int nullCount = 0;

            // Step 1: Identify NULL rows in ABONE_GRUBU
            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                string aboneGrubu = row[columnName]?.ToString();
                if (IsNullLike(aboneGrubu))
                {
                    nullCount++;
                    nullRows.Add(rowIndex);
                }
            }

            // Step 2: Calculate the percentage of NULL rows
            float nullPercentage = (float)nullCount / totalRows;

            // Step 3: Proceed with imputation if there are NULL values
            if (nullPercentage > 0)
            {
                // Imputation logic
                var grouped = currentDataTable.AsEnumerable().GroupBy(row => row["BINA_ID"]);

                aboneGrubuMostFrequent.Clear();

                foreach (var group in grouped)
                {
                    var mostFrequentAboneGrubu = group
                        .GroupBy(row => row["ABONE_GRUBU"].ToString())
                        .Where(g => !IsNullLike(g.Key))
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
                        if (aboneGrubuMostFrequent.ContainsKey(binaId))
                        {
                            var imputedGrup = aboneGrubuMostFrequent[binaId];
                            row["ABONE_GRUBU"] = imputedGrup;
                        }
                        else
                        {
                            row["ABONE_GRUBU"] = "Unknown";
                        }
                    }
                }
            }
        }

        private void BaglantiGucuImpute()
        {
            var grouped = currentDataTable.AsEnumerable().GroupBy(row => new { binaId = row["BINA_ID"].ToString(), aboneGrubu = row["ABONE_GRUBU"].ToString() });

            var mostFrequentBaglantiGucu = new Dictionary<(string, string), double>();

            // Step 1: Find the most frequent BAGLANTI_GUCU for each (BINA_ID, ABONE_GRUBU) pair
            foreach (var group in grouped)
            {
                var mostFrequent = group
                    .Where(row => row["BAGLANTI_GUCU"] != DBNull.Value)
                    .Select(row =>
                    {
                        if (double.TryParse(row["BAGLANTI_GUCU"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value) && value > 0)
                        {
                            return (Value: value, Valid: true);
                        }
                        return (Value: 0.0, Valid: false);
                    })
                    .Where(x => x.Valid)
                    .GroupBy(x => x.Value)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (mostFrequent != null)
                {
                    mostFrequentBaglantiGucu[(group.Key.binaId, group.Key.aboneGrubu)] = mostFrequent.Key;
                }
            }

            // Step 2: Impute BAGLANTI_GUCU for rows where it's ≤0 or NULL
            foreach (DataRow row in currentDataTable.Rows)
            {
                bool needsImputation = false;

                if (IsNullLike(row["BAGLANTI_GUCU"]))
                {
                    needsImputation = true;
                }
                else if (double.TryParse(row["BAGLANTI_GUCU"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    if (value <= 0)
                    {
                        needsImputation = true;
                    }
                }
                else
                {
                    // If parsing fails, treat as invalid and impute
                    needsImputation = true;
                }

                if (needsImputation)
                {
                    string binaId = row["BINA_ID"].ToString();
                    string aboneGrubu = row["ABONE_GRUBU"].ToString();
                    var key = (binaId, aboneGrubu);

                    if (mostFrequentBaglantiGucu.ContainsKey(key))
                    {
                        row["BAGLANTI_GUCU"] = mostFrequentBaglantiGucu[key];
                    }
                    else
                    {
                        // Fallback: Use a default value (e.g., the BAGLANTI_GUCU_THRESHOLD or a reasonable default)
                        row["BAGLANTI_GUCU"] = BAGLANTI_GUCU_THRESHOLD; // 30.0 as defined in the class
                    }
                }
            }
        }

        private void TrafoKoduImpute()
        {
            nullFieldsCheckWithLevel["BAGLANDIGI_TRAFO_KODU"] = InfoErrorBoundary(0.01f);
            trafoKoduRemoveFlag = true;

            // Step 1: Identify invalid BAGLANDIGI_TRAFO_KODU rows
            List<int> invalidRows = new List<int>();
            int invalidCount = 0;
            int totalRows = currentDataTable.Rows.Count;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                var trafoKodu = Convert.ToString(row["BAGLANDIGI_TRAFO_KODU"]);
                // Debug: Log the value to understand what's being encountered
                Console.WriteLine($"Row {i}: BAGLANDIGI_TRAFO_KODU = '{trafoKodu}'");
                if (IsNullLike(trafoKodu, true) || trafoKodu == "0" || trafoKodu == "#N/A")
                {
                    invalidCount++;
                    invalidRows.Add(i);
                }
            }

            columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"] = invalidRows;

            float percentage = (float)invalidCount / totalRows;

            // Debug: Log the results
            Console.WriteLine($"BAGLANDIGI_TRAFO_KODU: Found {invalidCount} invalid rows out of {totalRows}, percentage = {percentage:P1}, rows = {string.Join(", ", invalidRows)}");

            if (percentage > 0)
            {
                const float errorThreshold = 0.1f; // 10% as per the flowchart

                if (percentage > errorThreshold)
                {
                    string errorMessage = $"Hata Mesajı: %10’dan fazla oranda Enerji Tablo Kodu (BAGLANDIGI_TRAFO_KODU) olmayan abone mevcut. (Satır: {string.Join(", ", invalidRows)})";
                    errorDataTable.Rows.Add(new object[]
                    {
                "BAGLANDIGI_TRAFO_KODU",
                "Enerji Tablo Kodu",
                $"{percentage:P1}",
                errorMessage
                    });
                    // If percentage > 10%, we can return early since imputation won't proceed
                    return;
                }
                else
                {
                    string warningMessage = $"Düzeltilecekler Mesajı: Enerji Tablo Kodu (BAGLANDIGI_TRAFO_KODU) NULL veya 0 olan verilere en yakın trafonun kodu atanacaktır. (Satır: {string.Join(", ", invalidRows)})";
                    // Debug: Log the message to confirm content
                    Console.WriteLine($"Düzeltilecekler Message: {warningMessage}");
                    warningDataTable.Rows.Add(new object[]
                    {
                "BAGLANDIGI_TRAFO_KODU",
                "Enerji Tablo Kodu",
                $"{percentage:P1}",
                warningMessage
                    });
                }
            }
            else
            {
                // If no invalid rows, clear the map and return
                columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"] = new List<int>();
                Console.WriteLine("TrafoKoduImpute: No invalid BAGLANDIGI_TRAFO_KODU rows found, skipping imputation.");
                return;
            }

            // Step 2: Perform imputation on invalid rows
            var nonNullRows = currentDataTable.AsEnumerable()
                                              .Where(row => !IsNullLike(row["BAGLANDIGI_TRAFO_KODU"], true))
                                              .Select(row => new
                                              {
                                                  Row = row,
                                                  X = row["ABONE_X_KOORDINAT"] != DBNull.Value ? Convert.ToDouble(row["ABONE_X_KOORDINAT"]) : double.NaN,
                                                  Y = row["ABONE_Y_KOORDINAT"] != DBNull.Value ? Convert.ToDouble(row["ABONE_Y_KOORDINAT"]) : double.NaN
                                              })
                                              .Where(item => !double.IsNaN(item.X) && !double.IsNaN(item.Y))
                                              .ToList();

            nonNullRows.Sort((a, b) => a.X.CompareTo(b.X));

            // Track rows that fail imputation
            var failedImputationRows = new List<int>();

            // Perform imputation on flagged rows
            foreach (int missingIndex in invalidRows)
            {
                var missingRow = currentDataTable.Rows[missingIndex];

                // Check for NULL values in ABONE_X_KOORDINAT and ABONE_Y_KOORDINAT
                if (missingRow["ABONE_X_KOORDINAT"] == DBNull.Value || missingRow["ABONE_Y_KOORDINAT"] == DBNull.Value)
                {
                    Console.WriteLine($"Row {missingIndex}: Failed to impute BAGLANDIGI_TRAFO_KODU due to NULL ABONE_X_KOORDINAT or ABONE_Y_KOORDINAT");
                    failedImputationRows.Add(missingIndex);
                    continue;
                }

                double missingX, missingY;
                try
                {
                    missingX = Convert.ToDouble(missingRow["ABONE_X_KOORDINAT"]);
                    missingY = Convert.ToDouble(missingRow["ABONE_Y_KOORDINAT"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Row {missingIndex}: Failed to convert ABONE_X_KOORDINAT or ABONE_Y_KOORDINAT to double: {ex.Message}");
                    failedImputationRows.Add(missingIndex);
                    continue;
                }

                double closestDistance = double.MaxValue;
                var closestRow = default(dynamic);

                int position = nonNullRows.BinarySearch(new { Row = (DataRow)null, X = missingX, Y = 0.0 },
                                                        Comparer<dynamic>.Create((a, b) => a.X.CompareTo(b.X)));

                if (position < 0) position = ~position;

                int left = Math.Max(0, position - 100);
                int right = Math.Min(nonNullRows.Count - 1, position + 100);

                for (int i = left; i <= right; i++)
                {
                    var row = nonNullRows[i];
                    double x = row.X;
                    double y = row.Y;
                    double distance = Math.Sqrt(Math.Pow(missingX - x, 2) + Math.Pow(missingY - y, 2));

                    if (distance < closestDistance && distance < MAX_DISTANCE_IN_DEGREES)
                    {
                        closestDistance = distance;
                        closestRow = row.Row;
                    }
                }

                bool imputationSuccessful = false;
                if (closestRow != null)
                {
                    // Check for NULL value in BAGLANTI_GUCU
                    if (missingRow["BAGLANTI_GUCU"] == DBNull.Value)
                    {
                        Console.WriteLine($"Row {missingIndex}: Failed to impute BAGLANDIGI_TRAFO_KODU due to NULL BAGLANTI_GUCU");
                        failedImputationRows.Add(missingIndex);
                        continue;
                    }

                    double baglantiGucu;
                    try
                    {
                        baglantiGucu = Convert.ToDouble(missingRow["BAGLANTI_GUCU"]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Row {missingIndex}: Failed to convert BAGLANTI_GUCU to double: {ex.Message}");
                        failedImputationRows.Add(missingIndex);
                        continue;
                    }

                    if (baglantiGucu < BAGLANTI_GUCU_THRESHOLD)
                    {
                        missingRow["BAGLANDIGI_TRAFO_KODU"] = closestRow["BAGLANDIGI_TRAFO_KODU"];
                        imputationSuccessful = true;
                        // Debug: Log successful imputation
                        Console.WriteLine($"Row {missingIndex}: Successfully imputed BAGLANDIGI_TRAFO_KODU = {missingRow["BAGLANDIGI_TRAFO_KODU"]}");
                    }
                    else
                    {
                        // Debug: Log why imputation failed
                        Console.WriteLine($"Row {missingIndex}: Failed to impute BAGLANDIGI_TRAFO_KODU due to BAGLANTI_GUCU ({baglantiGucu}) >= {BAGLANTI_GUCU_THRESHOLD}");
                    }
                }
                else
                {
                    // Debug: Log why imputation failed
                    Console.WriteLine($"Row {missingIndex}: Failed to impute BAGLANDIGI_TRAFO_KODU, no close row found within {MAX_DISTANCE_IN_DEGREES} degrees");
                }

                if (!imputationSuccessful)
                {
                    failedImputationRows.Add(missingIndex);
                }
            }

            // Step 3: After imputation, check all rows for any that are still empty
            var remainingEmptyRows = new List<int>();
            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                var row = currentDataTable.Rows[i];
                var trafoKodu = Convert.ToString(row["BAGLANDIGI_TRAFO_KODU"]);
                // Debug: Log the value after imputation
                // Console.WriteLine($"Post-Imputation Check - Row {i}: BAGLANDIGI_TRAFO_KODU = '{trafoKodu}'");
                if (IsNullLike(trafoKodu, true) || trafoKodu == "0" || trafoKodu == "#N/A")
                {
                    remainingEmptyRows.Add(i);
                    Console.WriteLine($"Row {i}: Still empty after imputation, will be deleted");
                }
            }

            // Step 4: Combine failed imputation rows with remaining empty rows
            var rowsToDelete = failedImputationRows.Union(remainingEmptyRows).Distinct().ToList();

            // Step 5: Log and flag for deletion
            if (rowsToDelete.Any())
            {
                float failedPercentage = (float)rowsToDelete.Count / currentDataTable.Rows.Count;
                string warningMessage = $"Silinecekler Mesajı: Enerji Tablo Kodu (BAGLANDIGI_TRAFO_KODU) verileri doldurulamadı, silinecek. (Satır: {string.Join(", ", rowsToDelete)})";
                infoDataTable.Rows.Add(new object[]
                {
            "BAGLANDIGI_TRAFO_KODU",
            "Enerji Tablo Kodu",
            $"{failedPercentage:P1}",
            warningMessage
                });
                // Debug: Confirm the message was logged
                Console.WriteLine($"Logged Silinecekler Mesajı: {warningMessage}");

                // Update columnNullRowsMap for deletion by Remove
                columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"] = rowsToDelete;
            }
            else
            {
                // If no rows remain empty, clear the list to prevent deletion
                columnNullRowsMap["BAGLANDIGI_TRAFO_KODU"] = new List<int>();
                Console.WriteLine("TrafoKoduImpute: All rows successfully imputed, no deletions needed.");
            }
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
                double consumption = 0.0;
                if (row[consumptionColumn] != DBNull.Value && row[consumptionColumn].ToString() != TO_BE_IMPUTED_STRING)
                {
                    if (double.TryParse(row[consumptionColumn].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        consumption = value;
                    }
                    else
                    {
                        // Log the invalid value for debugging
                        Console.WriteLine($"Invalid format for {consumptionColumn} in trafo {key}: {row[consumptionColumn]}");
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
                double consumption = 0.0;
                if (row[consumptionColumn] != DBNull.Value)
                {
                    if (double.TryParse(row[consumptionColumn].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        consumption = value;
                    }
                    else
                    {
                        // Log the invalid value for debugging
                        Console.WriteLine($"Invalid format for {consumptionColumn} in abone {key}: {row[consumptionColumn]}");
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
