using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;


namespace SLF
{
    public class DTRModulu : GirdiModülü

    {
        private readonly (float warningThreshold, float errorThreshold) TUKETIM_ERROR_THRESHOLD = WarningErrorBoundary(0.7f);
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
                    int nullCount = 0;
                    int formatErrorCount = 0;
                    List<int> invalidRows = new List<int>();

                    for (int i = 0; i < totalRows; i++)
                    {
                        var row = currentDataTable.Rows[i];
                        var value = row[column]?.ToString();
                        bool isInvalid = false;

                        if (string.IsNullOrEmpty(value) || row.IsNull(column))
                        {
                            nullCount++;
                            isInvalid = true;
                        }
                        else if (!DateTime.TryParseExact(value, DATE_FORMAT, null, DateTimeStyles.None, out _))
                        {
                            formatErrorCount++;
                            isInvalid = true;
                        }

                        if (isInvalid)
                        {
                            invalidRows.Add(i);
                        }
                    }

                    invalidPercentage = (float)(nullCount + formatErrorCount) / totalRows;

                    if (invalidPercentage > 0)
                    {
                        // Add the column and its invalid rows to the dictionary, deduplicating with existing entries
                        if (columnNullRowsMap.ContainsKey(column.ColumnName))
                        {
                            columnNullRowsMap[column.ColumnName] = columnNullRowsMap[column.ColumnName].Union(invalidRows).Distinct().ToList();
                        }
                        else
                        {
                            columnNullRowsMap[column.ColumnName] = invalidRows;
                        }

                        var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        string message;

                        if (column.ColumnName == "TRAFO_KURULUM_TARIHI")
                        {
                            if (invalidPercentage <= thresholds.warningThreshold)
                            {
                                message = $"Uyarı: {invalidPercentage:P1} oranında {column.ColumnName} değerleri eksik veya geçersiz tarih formatında (NULL: {nullCount}, Format Hatası: {formatErrorCount}, Satır: {string.Join(", ", invalidRows)}). Tarihler {DATE_FORMAT} biçiminde olmalıdır. Bu değerler ortalama tarihle otomatik imputation ile doldurulacaktır.";
                            }
                            else
                            {
                                message = $"Hata: {invalidPercentage:P1} oranında {column.ColumnName} değerleri eksik veya geçersiz tarih formatında (NULL: {nullCount}, Format Hatası: {formatErrorCount}, Satır: {string.Join(", ", invalidRows)}). Tarihler {DATE_FORMAT} biçiminde olmalıdır. Ortalama tarihle imputation uygulanacak; ancak bu oran analizleri etkileyebilir, lütfen verileri kontrol edin.";
                            }
                        }
                        else
                        {
                            message = $"Hata: {invalidPercentage:P1} oranında {column.ColumnName} değerleri eksik veya geçersiz tarih formatında (NULL: {nullCount}, Format Hatası: {formatErrorCount}, Satır: {string.Join(", ", invalidRows)}). Tarihler {DATE_FORMAT} biçiminde olmalıdır. Lütfen düzeltiniz.";
                        }

                        datatableLevel.Rows.Add(new object[]
                        {
                    column.ColumnName,
                    "Geçersiz tarih formatı",
                    $"{invalidPercentage:P1}",
                    message
                        });
                    }
                }
            }
        }
        private void ReportTrafoLoad()
        {
            double loadThreshold = 1.0;
            int totalRows = currentDataTable.Rows.Count;
            var lastYearDemand = currentDataTable.Columns[$"YIL_DEMANT_{lastYear}"];
            var trafoKapasiteColumn = currentDataTable.Columns["TRAFO_KAPASITESI"];

            List<int> nullDemandRows = new List<int>();
            List<int> nonNumericDemandRows = new List<int>();
            List<int> zeroNegativeDemandRows = new List<int>();
            List<int> nullKapasiteRows = new List<int>();
            List<int> nonNumericKapasiteRows = new List<int>();
            List<int> zeroNegativeKapasiteRows = new List<int>();
            List<int> overLoadRows = new List<int>();

            int nullDemandCount = 0;
            int nonNumericDemandCount = 0;
            int zeroNegativeDemandCount = 0;
            int nullKapasiteCount = 0;
            int nonNumericKapasiteCount = 0;
            int zeroNegativeKapasiteCount = 0;
            int overLoadCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                float demand = 0; // Declare demand at the loop scope
                float kapasite = 0; // Declare kapasite at the loop scope

                // Check YIL_DEMANT_{lastYear}
                var demandValue = row[lastYearDemand]?.ToString();
                if (IsNullLike(demandValue))
                {
                    nullDemandCount++;
                    nullDemandRows.Add(rowIndex);
                    row[lastYearDemand] = 0; // Impute null demand to 0
                }
                else if (!float.TryParse(demandValue, out demand))
                {
                    nonNumericDemandCount++;
                    nonNumericDemandRows.Add(rowIndex);
                    row[lastYearDemand] = 0; // Impute non-numeric demand to 0
                }
                else if (demand <= 0)
                {
                    zeroNegativeDemandCount++;
                    zeroNegativeDemandRows.Add(rowIndex);
                    row[lastYearDemand] = 0; // Impute zero/negative demand to 0
                }

                // Check TRAFO_KAPASITESI
                var kapasiteValue = row[trafoKapasiteColumn]?.ToString();
                if (IsNullLike(kapasiteValue))
                {
                    nullKapasiteCount++;
                    nullKapasiteRows.Add(rowIndex);
                    row[trafoKapasiteColumn] = 0; // Impute null kapasite to 0
                }
                else if (!float.TryParse(kapasiteValue, out kapasite))
                {
                    nonNumericKapasiteCount++;
                    nonNumericKapasiteRows.Add(rowIndex);
                    row[trafoKapasiteColumn] = 0; // Impute non-numeric kapasite to 0
                }
                else if (kapasite <= 0)
                {
                    zeroNegativeKapasiteCount++;
                    zeroNegativeKapasiteRows.Add(rowIndex);
                    row[trafoKapasiteColumn] = 0; // Impute zero/negative kapasite to 0
                }

                // Check for overload after imputation
                if (kapasite > 0)
                {
                    demand = float.Parse(row[lastYearDemand]?.ToString() ?? "0"); // Safely parse after imputation
                    if (demand > 0)
                    {
                        float load = demand / kapasite;
                        if (load > loadThreshold)
                        {
                            overLoadCount++;
                            overLoadRows.Add(rowIndex);
                            row[lastYearDemand] = kapasite; // Impute demand to kapasite
                        }
                    }
                }
            }

            // Calculate total invalid percentage
            int totalInvalidCount = nullDemandCount + nonNumericDemandCount + zeroNegativeDemandCount +
                                    nullKapasiteCount + nonNumericKapasiteCount + zeroNegativeKapasiteCount + overLoadCount;
            float invalidPercentage = (float)totalInvalidCount / totalRows;

            // Combine all invalid rows for tracking
            var invalidRows = nullDemandRows.Concat(nonNumericDemandRows).Concat(zeroNegativeDemandRows)
                                            .Concat(nullKapasiteRows).Concat(nonNumericKapasiteRows)
                                            .Concat(zeroNegativeKapasiteRows).Concat(overLoadRows).ToList();
            columnNullRowsMap["TRAFO_LOAD"] = invalidRows; // Store invalid rows for reference

            if (invalidPercentage > 0)
            {
                string message = "Geçersiz veya aşırı yük değerleri: ";
                var issues = new List<string>();
                if (nullDemandCount > 0) issues.Add($"{(float)nullDemandCount / totalRows:P1} NULL/boş demand (imputed with 0) (Satır: {string.Join(", ", nullDemandRows)})");
                if (nonNumericDemandCount > 0) issues.Add($"{(float)nonNumericDemandCount / totalRows:P1} geçersiz format demand (imputed with 0) (Satır: {string.Join(", ", nonNumericDemandRows)})");
                if (zeroNegativeDemandCount > 0) issues.Add($"{(float)zeroNegativeDemandCount / totalRows:P1} sıfır/negatif demand (imputed with 0) (Satır: {string.Join(", ", zeroNegativeDemandRows)})");
                if (nullKapasiteCount > 0) issues.Add($"{(float)nullKapasiteCount / totalRows:P1} NULL/boş kapasite (imputed with 0) (Satır: {string.Join(", ", nullKapasiteRows)})");
                if (nonNumericKapasiteCount > 0) issues.Add($"{(float)nonNumericKapasiteCount / totalRows:P1} geçersiz format kapasite (imputed with 0) (Satır: {string.Join(", ", nonNumericKapasiteRows)})");
                if (zeroNegativeKapasiteCount > 0) issues.Add($"{(float)zeroNegativeKapasiteCount / totalRows:P1} sıfır/negatif kapasite (imputed with 0) (Satır: {string.Join(", ", zeroNegativeKapasiteRows)})");
                if (overLoadCount > 0) issues.Add($"{(float)overLoadCount / totalRows:P1} aşırı yük (demand kapasiteye eşitlendi) (Satır: {string.Join(", ", overLoadRows)})");
                message += string.Join("; ", issues) + ".";

                warningDataTable.Rows.Add(new object[]
                {
            "TRAFO_LOAD",
            "Trafo Yük Kontrolü",
            $"{invalidPercentage:P1}",
            message
                });
            }
        }
        private readonly List<string> duplicateFieldsGivingError = new List<string>
{
    "TRAFO_KODU"
};

        private void ReportCompositeDuplicateCounts()
        {
            float duplicatePercentage;
            float partialDuplicatePercentage;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!duplicateFieldsGivingError.Contains(column.ColumnName))
                    continue;

                // HashSet to store unique composite keys
                var uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var duplicateRowIndices = new List<int>();

                // Dictionary to track partial duplicates by TRAFO_KODU
                var trafoIdToCoordinatesMap = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                var partialDuplicateRowIndices = new List<int>();
                var nullTrafoKoduRows = new List<int>(); // Track rows with NULL or empty TRAFO_KODU

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value1 = row["TRAFO_KODU"]?.ToString() ?? string.Empty;
                    var value2 = row["TRAFO_X_KOORDINAT"]?.ToString() ?? string.Empty;
                    var value3 = row["TRAFO_Y_KOORDINAT"]?.ToString() ?? string.Empty;
                    var compositeKey = $"{value1}|{value2}|{value3}";

                    // Check for NULL or empty TRAFO_KODU
                    if (string.IsNullOrEmpty(value1) || row.IsNull("TRAFO_KODU"))
                    {
                        nullTrafoKoduRows.Add(i);
                        continue; // Skip further duplicate checks for invalid TRAFO_KODU
                    }

                    // Check for full duplicates
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
                        coordinatesSet.Add(coordinatePair);
                    }
                    else
                    {
                        trafoIdToCoordinatesMap[value1] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { $"{value2}|{value3}" };
                    }
                }

                // Store invalid row indices
                columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"] = duplicateRowIndices;
                columnNullRowsMap["PARTIAL_DUPLICATE_TRAFO_KODU"] = partialDuplicateRowIndices;
                columnNullRowsMap["NULL_TRAFO_KODU"] = nullTrafoKoduRows;

                // Calculate percentages
                int totalCount = currentDataTable.Rows.Count;
                int uniqueCount = uniqueValues.Count;
                int duplicateCount = totalCount - uniqueCount;
                duplicatePercentage = (float)duplicateCount / totalCount;

                int partialDuplicateCount = partialDuplicateRowIndices.Count;
                partialDuplicatePercentage = (float)partialDuplicateCount / totalCount;

                // Report full duplicates
                if (duplicateCount > 0)
                {
                    infoDataTable.Rows.Add(new object[]
                    {
                column.ColumnName, "Mükerrer hücre değerleri", $"{duplicatePercentage:P1}",
                "Aynı TRAFO_KODU, X ve Y koordinat kombinasyonu birden fazla kez kullanılmış. Lütfen verileri inceleyin ve düzeltin. Düzeltilmezse bu satırlar silinecektir."
                    });
                }

                // Report partial duplicates
                if (partialDuplicateCount > 0)
                {
                    errorDataTable.Rows.Add(new object[]
                    {
                column.ColumnName, "Kısmi mükerrer hücre değerleri", $"{partialDuplicatePercentage:P1}",
                "Aynı TRAFO_KODU farklı koordinatlarla tanımlanmış. Bu, veri giriş hatası olabilir. Lütfen bu kayıtları inceleyin, birleştirin veya silin."
                    });
                }

                // Report NULL TRAFO_KODU
                int nullTrafoKoduCount = nullTrafoKoduRows.Count;
                if (nullTrafoKoduCount > 0)
                {
                    float nullPercentage = (float)nullTrafoKoduCount / totalCount;
                    infoDataTable.Rows.Add(new object[]
                    {
                column.ColumnName, "NULL veya boş TRAFO_KODU", $"{nullPercentage:P1}",
                "Bazı TRAFO_KODU değerleri NULL veya boş. Bu satırlar analiz için geçersizdir ve silinecektir."
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
            { "TRAFO_KAPASITESI", ERROR_ONLY }, // WarningErrorBoundary(0.2f) },
            { "TRAFO_MULKIYET", WarningErrorBoundary(0.2f) },
            //{ "YIL_TUKETIM_2023", WarningErrorBoundary(0.2f) },
            //{ "YIL_DEMANT_2023", WarningErrorBoundary(0.2f) },
            { "PRIMER_GERILIM", WARNING_ONLY },
            //{ "SEKONDER_GERILIM", WARNING_ONLY },
        };

        private void ReportNullCounts()
        {
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                    continue;

                // Delegate to specialized functions for specific columns
                if (column.ColumnName == "TRAFO_MULKIYET")
                {
                    ImputeTrafoMulkiyet(); // Handle TRAFO_MULKIYET separately
                    continue;
                }
                else if (column.ColumnName == "TRAFO_KAPASITESI" || column.ColumnName.StartsWith("YIL_DEMANT") ||
                         column.ColumnName.StartsWith("YIL_TUKETIM"))
                {
                    // Skip, handled by ReportTrafoLoad and CheckDemandLimits
                    continue;
                }
                else if (column.ColumnName == "TRAFO_X_KOORDINAT" || column.ColumnName == "TRAFO_Y_KOORDINAT")
                {
                    // Skip if handled elsewhere, or add specific logic if needed
                    continue;
                }

                List<int> nullRows = new List<int>();
                int nullCount = 0;
                bool isZeroCheck = !column.ColumnName.Contains("MULKIYET"); // Zero check for non-MULKIYET columns

                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value = row[column]?.ToString();
                    bool isInvalid = false;

                    if (isInvalid)
                    {
                        nullCount++;
                        nullRows.Add(i);
                    }
                }

                columnNullRowsMap[column.ColumnName] = nullRows;
                float nullPercentage = (float)nullCount / totalRows;
                var thresholds = nullFieldsCheckWithLevel[column.ColumnName];

                if (nullPercentage > 0)
                {
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    string message;

                    // Determine message based on threshold levels
                    if (column.ColumnName == "TRAFO_KURULUM_TARIHI")
                    {
                        if (nullPercentage <= thresholds.warningThreshold)
                        {
                            message = $"Uyarı: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Tarihler 'dd.MM.yyyy' biçiminde olmalıdır. Bu değerler ortalama tarihle otomatik imputation ile doldurulacaktır.";
                        }
                        else if (nullPercentage <= thresholds.errorThreshold)
                        {
                            message = $"Dikkat: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Tarihler 'dd.MM.yyyy' biçiminde olmalıdır. Lütfen ortalama tarihle otomatik imputation onaylayın veya verileri kontrol edin.";
                        }
                        else
                        {
                            message = $"Hata: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Tarihler 'dd.MM.yyyy' biçiminde olmalıdır. Ortalama tarihle imputation uygulanacak; bu oran analizleri etkileyebilir, lütfen kontrol edin.";
                        }
                    }
                    else if (column.ColumnName == "TM_FIDER_ID")
                    {
                        if (nullPercentage <= thresholds.warningThreshold)
                        {
                            message = $"Uyarı: {nullPercentage:P1} oranında TM_FIDER_ID değerleri eksik. Bu değerler, koordinat bazlı yakınlık ile otomatik imputation ile doldurulacaktır.";
                        }
                        else if (nullPercentage <= thresholds.errorThreshold)
                        {
                            message = $"Dikkat: {nullPercentage:P1} oranında TM_FIDER_ID değerleri eksik. Lütfen koordinat bazlı otomatik imputation onaylayın veya verileri kontrol edin.";
                        }
                        else
                        {
                            message = $"Hata: {nullPercentage:P1} oranında TM_FIDER_ID değerleri eksik. Bu, analizleri etkileyebilir. Verilerin kontrol edilmesi önerilir.";
                        }
                    }
                    else
                    {
                        if (nullPercentage <= thresholds.warningThreshold)
                        {
                            message = $"Uyarı: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Bu değerler, uygun bir imputation yöntemiyle doldurulabilir.";
                        }
                        else if (nullPercentage <= thresholds.errorThreshold)
                        {
                            message = $"Dikkat: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Lütfen uygun imputation yöntemini onaylayın veya verileri kontrol edin.";
                        }
                        else
                        {
                            message = $"Hata: {nullPercentage:P1} oranında {column.ColumnName} değerleri eksik (Satır: {string.Join(", ", nullRows)}). Bu, analizleri etkileyebilir. Imputation uygulanacak; ek doğrulama gerekebilir.";
                        }
                    }

                    datatableLevel.Rows.Add(new object[]
                    {
                column.ColumnName,
                "Null veya geçersiz değer",
                $"{nullPercentage:P1}",
                message
                    });
                }
            }
        }

        private void ReportPrimerGerilim()
        {
            int totalRows = currentDataTable.Rows.Count;
            string column = "PRIMER_GERILIM";
            const int standardValue = 34500; // Standard imputation value

            List<int> nullRows = new List<int>();
            List<int> nonNumericRows = new List<int>();
            List<int> notInListRows = new List<int>();

            int nullCount = 0;
            int nonNumericCount = 0;
            int notInListCount = 0;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                var cellValue = row[column]?.ToString();
                if (IsNullLike(cellValue))
                {
                    nullCount++;
                    nullRows.Add(i);
                    row[column] = standardValue; // Impute null-like values
                }
                else if (!int.TryParse(cellValue, out int value))
                {
                    nonNumericCount++;
                    nonNumericRows.Add(i);
                    row[column] = standardValue; // Impute non-numeric values
                }
                else if (!PRIMER_GERILIM_LISTESI.Contains(value))
                {
                    notInListCount++;
                    notInListRows.Add(i);
                    row[column] = standardValue; // Impute values not in the list
                }
            }

            // Calculate total invalid percentage
            int totalInvalidCount = nullCount + nonNumericCount + notInListCount;
            float invalidPercentage = (float)totalInvalidCount / totalRows;

            // Combine all invalid rows for tracking
            var invalidRows = nullRows.Concat(nonNumericRows).Concat(notInListRows).ToList();
            columnNullRowsMap[column] = invalidRows; // Store invalid rows for potential future reference

            if (invalidPercentage > 0)
            {
                // Report as warning (no threshold, just warning)
                string message = "Geçersiz değer (imputed with 34500): ";
                var issues = new List<string>();
                if (nullCount > 0) issues.Add($"{(float)nullCount / totalRows:P1} NULL/boş (Satır: {string.Join(", ", nullRows)})");
                if (nonNumericCount > 0) issues.Add($"{(float)nonNumericCount / totalRows:P1} geçersiz format (Satır: {string.Join(", ", nonNumericRows)})");
                if (notInListCount > 0) issues.Add($"{(float)notInListCount / totalRows:P1} listeden değil (Satır: {string.Join(", ", notInListRows)})");
                message += string.Join("; ", issues) + ".";

                warningDataTable.Rows.Add(new object[]
                {
            column,
            "Primer Gerilim Kontrolü",
            $"{invalidPercentage:P1}",
            message
                });
            }
        }
        private void ReportSekonderGerilim()
        {
            int totalRows = currentDataTable.Rows.Count;
            string column = "SEKONDER_GERILIM";
            const int standardValue = 400; // Standard imputation value

            List<int> nullRows = new List<int>();
            List<int> nonNumericRows = new List<int>();
            List<int> greaterThan400Rows = new List<int>();

            int nullCount = 0;
            int nonNumericCount = 0;
            int greaterThan400Count = 0;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                var cellValue = row[column]?.ToString();
                if (IsNullLike(cellValue))
                {
                    nullCount++;
                    nullRows.Add(i);
                    row[column] = standardValue; // Impute null-like values with 400
                }
                else if (!int.TryParse(cellValue, out int value))
                {
                    nonNumericCount++;
                    nonNumericRows.Add(i);
                    row[column] = standardValue; // Impute non-numeric values with 400
                }
                else if (value > 400)
                {
                    greaterThan400Count++;
                    greaterThan400Rows.Add(i);
                    currentDataTable.Rows.RemoveAt(i); // Delete row if value > 400
                    i--; // Adjust index after deletion
                    totalRows--; // Update totalRows after deletion
                }
            }

            // Calculate total invalid percentage based on remaining rows
            int totalInvalidCount = nullCount + nonNumericCount + greaterThan400Count;
            float invalidPercentage = (float)totalInvalidCount / currentDataTable.Rows.Count;

            // Combine all invalid rows for tracking (adjust indices for deleted rows if needed)
            var invalidRows = nullRows.Concat(nonNumericRows).Concat(greaterThan400Rows).ToList();
            columnNullRowsMap[column] = invalidRows; // Store invalid rows for potential future reference

            if (invalidPercentage > 0)
            {
                // Report as warning (no threshold, just warning)
                string message = "Geçersiz değer: ";
                var issues = new List<string>();
                if (nullCount > 0) issues.Add($"{(float)nullCount / currentDataTable.Rows.Count:P1} NULL/boş (imputed with 400) (Satır: {string.Join(", ", nullRows)})");
                if (nonNumericCount > 0) issues.Add($"{(float)nonNumericCount / currentDataTable.Rows.Count:P1} geçersiz format (imputed with 400) (Satır: {string.Join(", ", nonNumericRows)})");
                if (greaterThan400Count > 0) issues.Add($"{(float)greaterThan400Count / currentDataTable.Rows.Count:P1} > 400 (silinecek) (Satır: {string.Join(", ", greaterThan400Rows)})");
                message += string.Join("; ", issues) + ".";

                warningDataTable.Rows.Add(new object[]
                {
            column,
            "Sekonder Gerilim Kontrolü",
            $"{invalidPercentage:P1}",
            message
                });
            }
        }
        /// <summary>
        /// Trafo demand değerlerini kontrol eder ve kapasite limitlerini aşan değerleri kapasite değerine eşitler.
        /// </summary>
        /// 
        private void CheckDemandLimits()
        {
            Console.WriteLine("\n[INFO] Demand limitleri kontrol ediliyor...");

            // Düzeltme yapılacak kayıt sayılarını tut
            Dictionary<int, Dictionary<string, List<int>>> issueRows = new Dictionary<int, Dictionary<string, List<int>>>();
            Dictionary<int, int> fixedCounts = new Dictionary<int, int>();

            // Dynamic year range (e.g., last 3 years up to lastYear)
            int startYear = Math.Max(2021, lastYear - 2); // Ensure at least 2021
            for (int year = startYear; year <= lastYear; year++)
            {
                string demandCol = $"YIL_DEMANT_{year}";
                if (!currentDataTable.Columns.Contains(demandCol)) continue; // Skip if column doesn't exist

                issueRows[year] = new Dictionary<string, List<int>>
        {
            { "nullDemand", new List<int>() },
            { "nonNumericDemand", new List<int>() },
            { "zeroNegativeDemand", new List<int>() },
            { "nullKapasite", new List<int>() },
            { "nonNumericKapasite", new List<int>() },
            { "zeroNegativeKapasite", new List<int>() },
            { "overloaded", new List<int>() }
        };

                int nullDemandCount = 0;
                int nonNumericDemandCount = 0;
                int zeroNegativeDemandCount = 0;
                int nullKapasiteCount = 0;
                int nonNumericKapasiteCount = 0;
                int zeroNegativeKapasiteCount = 0;
                int overloadedCount = 0;

                for (int i = 0; i < currentDataTable.Rows.Count; i++)
                {
                    DataRow row = currentDataTable.Rows[i];
                    double demand = 0; // Declare demand at the loop scope
                    double capacity = 0; // Declare capacity at the loop scope

                    // Check TRAFO_KAPASITESI
                    var kapasiteValue = row["TRAFO_KAPASITESI"]?.ToString();
                    if (IsNullLike(kapasiteValue))
                    {
                        nullKapasiteCount++;
                        issueRows[year]["nullKapasite"].Add(i);
                        row["TRAFO_KAPASITESI"] = 0; // Impute null kapasite to 0
                    }
                    else if (!double.TryParse(kapasiteValue, out capacity))
                    {
                        nonNumericKapasiteCount++;
                        issueRows[year]["nonNumericKapasite"].Add(i);
                        row["TRAFO_KAPASITESI"] = 0; // Impute non-numeric kapasite to 0
                    }
                    else if (capacity <= 0)
                    {
                        zeroNegativeKapasiteCount++;
                        issueRows[year]["zeroNegativeKapasite"].Add(i);
                        row["TRAFO_KAPASITESI"] = 0; // Impute zero/negative kapasite to 0
                    }

                    // Check YIL_DEMANT_{year}
                    var demandValue = row[demandCol]?.ToString();
                    if (IsNullLike(demandValue))
                    {
                        nullDemandCount++;
                        issueRows[year]["nullDemand"].Add(i);
                        row[demandCol] = 0; // Impute null demand to 0
                    }
                    else if (!double.TryParse(demandValue, out demand))
                    {
                        nonNumericDemandCount++;
                        issueRows[year]["nonNumericDemand"].Add(i);
                        row[demandCol] = 0; // Impute non-numeric demand to 0
                    }
                    else if (demand <= 0)
                    {
                        zeroNegativeDemandCount++;
                        issueRows[year]["zeroNegativeDemand"].Add(i);
                        row[demandCol] = 0; // Impute zero/negative demand to 0
                    }
                    else if (capacity > 0 && demand > capacity)
                    {
                        row[demandCol] = capacity; // Correct demand to capacity
                        overloadedCount++;
                        issueRows[year]["overloaded"].Add(i);
                    }
                }

                fixedCounts[year] = overloadedCount;

                // Report for this year
                int totalInvalidCount = nullDemandCount + nonNumericDemandCount + zeroNegativeDemandCount +
                                        nullKapasiteCount + nonNumericKapasiteCount + zeroNegativeKapasiteCount + overloadedCount;
                float invalidPercentage = (float)totalInvalidCount / currentDataTable.Rows.Count;

                if (invalidPercentage > 0)
                {
                    string message = "Geçersiz veya aşırı yük değerleri: ";
                    var issues = new List<string>();
                    if (nullDemandCount > 0) issues.Add($"{(float)nullDemandCount / currentDataTable.Rows.Count:P1} NULL/boş demand (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["nullDemand"])})");
                    if (nonNumericDemandCount > 0) issues.Add($"{(float)nonNumericDemandCount / currentDataTable.Rows.Count:P1} geçersiz format demand (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["nonNumericDemand"])})");
                    if (zeroNegativeDemandCount > 0) issues.Add($"{(float)zeroNegativeDemandCount / currentDataTable.Rows.Count:P1} sıfır/negatif demand (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["zeroNegativeDemand"])})");
                    if (nullKapasiteCount > 0) issues.Add($"{(float)nullKapasiteCount / currentDataTable.Rows.Count:P1} NULL/boş kapasite (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["nullKapasite"])})");
                    if (nonNumericKapasiteCount > 0) issues.Add($"{(float)nonNumericKapasiteCount / currentDataTable.Rows.Count:P1} geçersiz format kapasite (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["nonNumericKapasite"])})");
                    if (zeroNegativeKapasiteCount > 0) issues.Add($"{(float)zeroNegativeKapasiteCount / currentDataTable.Rows.Count:P1} sıfır/negatif kapasite (imputed with 0) (Satır: {string.Join(", ", issueRows[year]["zeroNegativeKapasite"])})");
                    if (overloadedCount > 0) issues.Add($"{(float)overloadedCount / currentDataTable.Rows.Count:P1} aşırı yük (demand kapasiteye eşitlendi) (Satır: {string.Join(", ", issueRows[year]["overloaded"])})");
                    message += string.Join("; ", issues) + ".";

                    warningDataTable.Rows.Add(new object[]
                    {
                $"YIL_DEMANT_{year}",
                "Demand Limit Kontrolü",
                $"{invalidPercentage:P1}",
                message
                    });
                }
            }

            // Console reporting
            Console.WriteLine("\n[INFO] Demand değeri düzeltilen trafo sayıları:");
            foreach (var pair in fixedCounts)
            {
                Console.WriteLine($"  {pair.Key} yılı: {pair.Value} adet trafo");
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

        public override void Impute()
        {
            ImputeTMFiderID();
            ImputeAverageDate();
            ImputeTrafoMulkiyet();
            //ImputeTrafoKapasitesi();
            ImputeTuketim();
            ImputeDemand();

            // Add the new demand limits check
            CheckDemandLimits();
        }

        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["NONUNIQUE_TRAFO_X_Y"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap[$"YIL_TUKETIM_{lastYear}"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["NULL_TRAFO_KODU"]);



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
            const int defaultValue = 0; // Configurable default value for imputation
            int totalRows = currentDataTable.Rows.Count; // Current number of rows
            int originalTotalRows = totalRows; // Assume original count (adjust if tracked elsewhere)

            List<int> nullRows = new List<int>();
            List<int> invalidFormatRows = new List<int>();
            List<int> ozelRows = new List<int>(); // Track rows converted from ÖZEL
            List<int> kurumRows = new List<int>(); // Track rows converted from KURUM
            var imputableRows = new List<int>();

            // Declare message once at the method scope
            string message = "";

            // Scan all rows for null, invalid format, and ÖZEL/KURUM values
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                var cellValue = row[column]?.ToString();
                if (IsNullLike(cellValue))
                {
                    nullRows.Add(i);
                }
                else if (string.Equals(cellValue, "ÖZEL", StringComparison.OrdinalIgnoreCase))
                {
                    row[column] = 1; // Convert ÖZEL to 1
                    ozelRows.Add(i);
                }
                else if (string.Equals(cellValue, "KURUM", StringComparison.OrdinalIgnoreCase))
                {
                    row[column] = 0; // Convert KURUM to 0
                    kurumRows.Add(i);
                }
                else if (!int.TryParse(cellValue, out int value) || value < 0) // Invalid format or negative value
                {
                    invalidFormatRows.Add(i);
                }
            }

            // Calculate total invalid percentage (excluding converted rows)
            int totalInvalidCount = nullRows.Count + invalidFormatRows.Count;
            float invalidPercentage = (float)totalInvalidCount / originalTotalRows;

            // Enforce threshold
            if (invalidPercentage > 0.2f) // 20% threshold from WarningErrorBoundary(0.2f)
            {
                message = $"Hata: {invalidPercentage:P1} oranında {column} değerleri eksik veya geçersiz " +
                          $"(NULL: {string.Join(", ", nullRows)}; Geçersiz format: {string.Join(", ", invalidFormatRows)}). " +
                          "%20 eşiği aşıldı; imputation uygulanamaz, lütfen verileri manuel olarak düzeltin.";
                warningDataTable.Rows.Add(new object[]
                {
            column,
            "Trafo Mülkiyet Kontrolü",
            $"{invalidPercentage:P1}",
            message
                });
                return; // Skip imputation
            }

            // Proceed with imputation if within threshold
            imputableRows.AddRange(nullRows);
            imputableRows.AddRange(invalidFormatRows);

            foreach (int missingIndex in imputableRows)
            {
                if (missingIndex >= 0 && missingIndex < currentDataTable.Rows.Count)
                {
                    var missingRow = currentDataTable.Rows[missingIndex];
                    missingRow[column] = defaultValue; // Impute with default value
                }
                else
                {
                    errorDataTable.Rows.Add(new object[]
                    {
                column,
                "Geçersiz İndeks",
                "0%",
                $"İndeks {missingIndex} geçerli aralıkta değil; {column} için imputation uygulanamadı."
                    });
                }
            }

            // Report conversions and imputation
            message = ""; // Reset message for reporting
            if (ozelRows.Count > 0)
            {
                message += $"Converted ÖZEL to 1 (Satır: {string.Join(", ", ozelRows)}); ";
            }
            if (kurumRows.Count > 0)
            {
                message += $"Converted KURUM to 0 (Satır: {string.Join(", ", kurumRows)}); ";
            }
            if (imputableRows.Count > 0)
            {
                message += $"Geçersiz değer (imputed with {defaultValue}): " +
                           $"{(float)nullRows.Count / originalTotalRows:P1} NULL/boş " +
                           $"(imputed with {defaultValue}) (Satır: {string.Join(", ", nullRows)}) " +
                           $"{(float)invalidFormatRows.Count / originalTotalRows:P1} geçersiz format " +
                           $"(imputed with {defaultValue}) (Satır: {string.Join(", ", invalidFormatRows)}).";
            }

            if (!string.IsNullOrEmpty(message))
            {
                warningDataTable.Rows.Add(new object[]
                {
            column,
            "Trafo Mülkiyet Kontrolü",
            $"{invalidPercentage:P1}",
            message
                });
            }

            // Update map after imputation (optional, if still used by other functions)
            columnNullRowsMap[column] = new List<int>(); // Clear after successful imputation
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
        private void ImputeDemand()
        {
            var demandColumn = $"YIL_DEMANT_{lastYear}";
            var tuketimColumn = $"YIL_TUKETIM_{lastYear}";
            foreach (int missingIndex in imputableRowsMap[demandColumn])
            {
                if (missingIndex >= 0 && missingIndex < currentDataTable.Rows.Count)
                {
                    var missingRow = currentDataTable.Rows[missingIndex];
                    var tuketim_value = missingRow[tuketimColumn];
                    double imputedValue;

                    // Handle NULL, zero, negative, or invalid format values (all imputed to 0)
                    if (tuketim_value == DBNull.Value || tuketim_value == null || string.IsNullOrEmpty(tuketim_value?.ToString()) || !double.TryParse(tuketim_value.ToString(), out double tuketimDouble))
                    {
                        imputedValue = 0; // Fallback for NULL, zero, invalid format, or negative
                        string errorDetail = tuketim_value == DBNull.Value || tuketim_value == null || string.IsNullOrEmpty(tuketim_value?.ToString())
                            ? "NULL veya boş"
                            : $"geçersiz format (non-numeric: '{tuketim_value}')";
                        errorDataTable.Rows.Add(new object[]
                        {
                    demandColumn,
                    "Imputation Uyarısı",
                    "0%",
                    $"Satır {missingIndex} için {tuketimColumn} {errorDetail}; {demandColumn} sıfıra ayarlandı."
                        });
                    }
                    else
                    {
                        imputedValue = K_FACTOR * tuketimDouble / HoursInYear;
                        if (imputedValue < 0)
                        {
                            imputedValue = 0; // Ensure final value is non-negative
                            errorDataTable.Rows.Add(new object[]
                            {
                        demandColumn,
                        "Imputation Uyarısı",
                        "0%",
                        $"Satır {missingIndex} için hesaplanan {demandColumn} değeri negatif ({imputedValue}); sıfıra ayarlandı."
                            });
                        }
                    }
                    missingRow[demandColumn] = imputedValue;
                }
                else
                {
                    errorDataTable.Rows.Add(new object[]
                    {
                demandColumn,
                "Geçersiz İndeks",
                "0%",
                $"İndeks {missingIndex} geçerli aralıkta değil; {demandColumn} için imputation uygulanamadı."
                    });
                }
            }
        }

        private void ImputeTuketim()
        {
            const double maxDistance = 0.005;
            var demandColumn = $"YIL_DEMANT_{lastYear}";
            var kapasiteColumn = "TRAFO_KAPASITESI";
            var tuketimColumn = $"YIL_TUKETIM_{lastYear}";
            foreach (int missingIndex in imputableRowsMap[tuketimColumn])
            {
                if (missingIndex >= 0 && missingIndex < currentDataTable.Rows.Count)
                {
                    var missingRow = currentDataTable.Rows[missingIndex];
                    var demand_value = missingRow[demandColumn];
                    double imputedValue;

                    // Handle NULL, negative, or invalid format values (all imputed the same way)
                    if (demand_value == DBNull.Value || demand_value == null || string.IsNullOrEmpty(demand_value?.ToString()) || !double.TryParse(demand_value.ToString(), out double demandDouble))
                    {
                        imputedValue = 0; // Fallback for NULL, invalid format, or if demandDouble parsing fails
                        string errorDetail = demand_value == DBNull.Value || demand_value == null || string.IsNullOrEmpty(demand_value?.ToString())
                            ? "NULL veya boş"
                            : $"geçersiz format (non-numeric: '{demand_value}')";
                        errorDataTable.Rows.Add(new object[]
                        {
                            tuketimColumn,
                            "Imputation Uyarısı",
                            "0%",
                            $"Satır {missingIndex} için {demandColumn} {errorDetail}; {tuketimColumn} sıfıra ayarlandı."
                        });
                    }
                    else
                    {
                        imputedValue = demandDouble * HoursInYear / K_FACTOR;
                        if (imputedValue < 0)
                        {
                            imputedValue = 0; // Ensure final value is non-negative
                            errorDataTable.Rows.Add(new object[]
                            {
                                tuketimColumn,
                                "Imputation Uyarısı",
                                "0%",
                                $"Satır {missingIndex} için hesaplanan {tuketimColumn} değeri negatif ({imputedValue}); sıfıra ayarlandı."
                            });
                        }
                    }
                    missingRow[tuketimColumn] = imputedValue;
                }
                else
                {
                    errorDataTable.Rows.Add(new object[]
                    {
                        tuketimColumn,
                        "Geçersiz İndeks",
                        "0%",
                        $"İndeks {missingIndex} geçerli aralıkta değil; {tuketimColumn} için imputation uygulanamadı."
                    });
                }
            }
        }
        private void ImputeTMFiderID()
        {
            // 0.01 is the 2d distance of the delta of x and y coordinates, approximately 1 km (assuming degree-based coordinates).
            const double maxDistance = 0.01;

            foreach (int missingIndex in columnNullRowsMap["TM_FIDER_ID"])
            {
                var missingRow = currentDataTable.Rows[missingIndex];

                // Validate and convert coordinates for the missing row
                if (!double.TryParse(missingRow["TRAFO_X_KOORDINAT"]?.ToString(), out double missingX) ||
                    !double.TryParse(missingRow["TRAFO_Y_KOORDINAT"]?.ToString(), out double missingY))
                {
                    missingRow["TM_FIDER_ID"] = "Geçersiz koordinatlar"; // Placeholder value for failed imputation
                                                                         // Log the failure to errorDataTable for user visibility
                    errorDataTable.Rows.Add(new object[]
                    {
                "TM_FIDER_ID", "Imputasyon başarısız", "Geçersiz koordinat",
                $"Satır {missingIndex}: TM_FIDER_ID imputasyonu başarısız: Geçersiz koordinatlar (TRAFO_X_KOORDINAT veya TRAFO_Y_KOORDINAT). Lütfen koordinat verilerini kontrol edin ve düzeltin."
                    });
                    continue;
                }

                double closestDistance = double.MaxValue;
                DataRow closestRow = null;

                foreach (DataRow row in currentDataTable.Rows)
                {
                    if (row == missingRow || IsNullLike(row["TM_FIDER_ID"], true))
                        continue;

                    // Validate and convert coordinates for the candidate row
                    if (!double.TryParse(row["TRAFO_X_KOORDINAT"]?.ToString(), out double x) ||
                        !double.TryParse(row["TRAFO_Y_KOORDINAT"]?.ToString(), out double y))
                        continue;

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
                    // Only impute FIDER_ADI if it exists and is not NULL
                    if (!IsNullLike(closestRow["FIDER_ADI"], true))
                        missingRow["FIDER_ADI"] = closestRow["FIDER_ADI"];
                }
                else
                {
                    missingRow["TM_FIDER_ID"] = "Uygun fider yok"; // Placeholder value for failed imputation
                                                                   // Log the failure to errorDataTable for user visibility
                    errorDataTable.Rows.Add(new object[]
                    {
                "TM_FIDER_ID", "Imputasyon başarısız", "Uygun fider bulunamadı",
                $"Satır {missingIndex}: TM_FIDER_ID imputasyonu başarısız: 1 km içinde uygun bir fider bulunamadı. Lütfen koordinatları doğrulayın veya TM_FIDER_ID değerini manuel olarak atayın."
                    });
                }
            }
        }
        private void ReportErrorLessThanZero(string columnName)
        {
            float nullPercentage, negativePercentage, zeroPercentage, formatPercentage;
            int totalRows = currentDataTable.Rows.Count;
            var column = currentDataTable.Columns[columnName];
            var nullRows = new List<int>();
            var negativeRows = new List<int>();
            var zeroRows = new List<int>();
            var formatErrorRows = new List<int>();
            var imputableRows = new List<int>(); // Combined rows for imputation (below 20%)

            int nullCount = 0;
            int negativeCount = 0;
            int zeroCount = 0;
            int formatErrorCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                var value = row[column]?.ToString();
                if (row.IsNull(column) || row[column] == DBNull.Value || string.IsNullOrEmpty(value) || nullLikeStrings.Contains(value, StringComparer.OrdinalIgnoreCase))
                {
                    nullCount++;
                    nullRows.Add(rowIndex);
                }
                else if (float.TryParse(value, out float parsedValue))
                {
                    if (parsedValue < 0)
                    {
                        negativeCount++;
                        negativeRows.Add(rowIndex);
                    }
                    else if (parsedValue == 0)
                    {
                        zeroCount++;
                        zeroRows.Add(rowIndex);
                    }
                }
                else
                {
                    formatErrorCount++;
                    formatErrorRows.Add(rowIndex);
                }
            }

            // Calculate combined percentage based on column type
            float combinedPercentage;
            if (columnName.StartsWith("YIL_DEMANT"))
            {
                combinedPercentage = (float)(nullCount + negativeCount + zeroCount + formatErrorCount) / totalRows; // Include zeros for DEMANT
            }
            else // YIL_TUKETIM
            {
                combinedPercentage = (float)(nullCount + negativeCount + formatErrorCount) / totalRows; // Exclude zeros for TUKETIM
            }

            nullPercentage = (float)nullCount / totalRows;
            negativePercentage = (float)negativeCount / totalRows;
            zeroPercentage = (float)zeroCount / totalRows;
            formatPercentage = (float)formatErrorCount / totalRows;

            var thresholds = TUKETIM_ERROR_THRESHOLD; // (0.2f, 0.2f)
            var datatableLevel = GetDataTableBasedOnThreshold(combinedPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

            // Handle zero rows based on column type
            if (columnName.StartsWith("YIL_TUKETIM") && zeroPercentage > 0)
            {
                foreach (int zeroIndex in zeroRows.OrderByDescending(i => i))
                {
                    currentDataTable.Rows.RemoveAt(zeroIndex);
                }
            }

            if (combinedPercentage > 0)
            {
                string message = "Son yıl verisi sorunları: ";
                var issues = new List<string>();
                if (nullPercentage > 0) issues.Add($"{nullPercentage:P1} NULL/boş (Satır: {string.Join(", ", nullRows)})");
                if (negativePercentage > 0) issues.Add($"{negativePercentage:P1} negatif (Satır: {string.Join(", ", negativeRows)})");
                if (formatPercentage > 0) issues.Add($"{formatPercentage:P1} geçersiz format (Satır: {string.Join(", ", formatErrorRows)})");
                if (zeroPercentage > 0 && columnName.StartsWith("YIL_DEMANT")) issues.Add($"{zeroPercentage:P1} sıfır (Satır: {string.Join(", ", zeroRows)})");
                message += string.Join("; ", issues);

                if (combinedPercentage <= thresholds.errorThreshold) // Below or equal to 20%
                {
                    imputableRows.AddRange(nullRows);
                    imputableRows.AddRange(negativeRows);
                    if (columnName.StartsWith("YIL_DEMANT")) imputableRows.AddRange(zeroRows); // Impute zeros for DEMANT
                    imputableRows.AddRange(formatErrorRows);
                    message += ". Bu veriler için imputation uygulanacak.";
                }
                else // Above 20%
                {
                    columnNullRowsMap[column.ColumnName] = nullRows.Concat(negativeRows).Concat(zeroRows).Concat(formatErrorRows).ToList();
                    imputableRowsMap[column.ColumnName] = new List<int>(); // Clear for no imputation
                    message += ". %20 eşiği aşıldı; imputation uygulanamaz, lütfen verileri manuel olarak düzeltin.";
                }

                message += " (%20 eşiği aşılırsa ek doğrulama gerekebilir.)";
                datatableLevel.Rows.Add(new object[]
                {
            column.ColumnName,
            "Son yıl verisi",
            $"{combinedPercentage:P1}",
            message
                });
            }

            if (zeroPercentage > 0 && columnName.StartsWith("YIL_TUKETIM"))
            {
                infoDataTable.Rows.Add(new object[]
                {
            column.ColumnName,
            "Son yıl verisi",
            $"{zeroPercentage:P1}",
            $"Satır: {string.Join(", ", zeroRows)}. Bu trafolarda son yıl tüketim verisi sıfır; bu satırlar silindi."
                });
            }

            // Update maps
            columnNullRowsMap[column.ColumnName] = new List<int>(); // Zeros are handled separately
            imputableRowsMap[column.ColumnName] = imputableRows; // Only populated if below 20%
        }

        void ImputeAverageDate()
        {
            string dateColumn = "TRAFO_KURULUM_TARIHI";
            var dateStrings = currentDataTable.AsEnumerable()
                                             .Where(row => !string.IsNullOrEmpty(row[dateColumn]?.ToString()))
                                             .Select(row => row[dateColumn].ToString())
                                             .ToList();

            // Filter only valid dates in "dd.MM.yyyy" format
            var dateTimes = new List<DateTime>();
            var invalidDates = new List<(string date, int rowIndex)>();

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                var dateString = currentDataTable.Rows[i][dateColumn]?.ToString();
                if (!string.IsNullOrEmpty(dateString))
                {
                    if (DateTime.TryParseExact(dateString, DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        dateTimes.Add(parsedDate);
                    }
                    else
                    {
                        invalidDates.Add((dateString, i));
                    }
                }
            }

            // Log invalid dates if any
            if (invalidDates.Any())
            {
                errorDataTable.Rows.Add(new object[]
                {
                    dateColumn,
                    "Geçersiz Tarih Formatı",
                    "0%",
                    $"Imputation sırasında {invalidDates.Count} geçersiz tarih bulundu (Satır ve Değer: {string.Join(", ", invalidDates.Select(x => $"[{x.rowIndex}]: {x.date}"))}). Tarihler 'dd.MM.yyyy' biçiminde olmalıdır."
                });
            }

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