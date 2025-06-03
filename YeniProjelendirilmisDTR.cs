using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SLF
{
    public class YeniProjelendirilmisDTR : GirdiModülü

    {
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri" };
        // private readonly (float warningThreshold, float errorThreshold) INVESTMENT_YEAR_IMPUTATION_THRESHOLD = (0.01f, 0.5f); // 10% warning, 50% error
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = INFO_ONLY;

        public YeniProjelendirilmisDTR()
        {
            minMaxCheckMap = CalculateCoordinateBounds();
        }
        private bool IsNullLike(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        private void MevcutDTRKapasiteCheck()
        {
            int invalidNewCapacityCount = 0;
            int totalRows = currentDataTable.Rows.Count;
            var oldTrafoCapacity = currentDataTable.Columns["PROJELENDIRILMIS_TRAFO_KAPASITE"];
            var newTrafoCapacity = currentDataTable.Columns["PROJELENDIRILMIS_TRAFO_YENI_KAPASITE"];
            var invalidRows = new List<int>();

            if (oldTrafoCapacity == null || newTrafoCapacity == null)
            {
                Console.WriteLine("Error: Capacity columns not found in currentDataTable.");
                return;
            }

            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);

                // Check if PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI is "0" (yeni), and skip capacity check if true
                string yatırımSınıfı = row["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]?.ToString();
                if (yatırımSınıfı == "0")
                {
                    Console.WriteLine($"Row {rowIndex} skipped: PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI is 0 (yeni).");
                    continue;
                }

                bool isOldCapacityValid = float.TryParse(row[oldTrafoCapacity]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float oldCapacity);

                if (!isOldCapacityValid || oldCapacity <= 0)
                {
                    invalidNewCapacityCount++;
                    invalidRows.Add(rowIndex);
                    Console.WriteLine($"Row {rowIndex} flagged: Invalid old capacity (old={row[oldTrafoCapacity]})");
                }
                else
                {
                    if (float.TryParse(row[newTrafoCapacity]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float newCapacity) && newCapacity > 0)
                    {
                        if (newCapacity < oldCapacity)
                        {
                            invalidNewCapacityCount++;
                            invalidRows.Add(rowIndex);
                            Console.WriteLine($"Row {rowIndex} flagged: newCapacity={newCapacity}, oldCapacity={oldCapacity}");
                        }
                    }
                }
            }

            Console.WriteLine($"MevcutDTRKapasiteCheck: Found {invalidNewCapacityCount} invalid rows out of {totalRows}.");

            columnNullRowsMap["KAPASITE"] = invalidRows;
            float invalidNewCapacityPercentage = (float)invalidNewCapacityCount / totalRows;

            if (invalidNewCapacityPercentage > 0)
            {
                infoDataTable.Rows.Add(new object[]
                {
            "PROJELENDIRILMIS_TRAFO_YENI_KAPASITE", "Projelendirilmiş yeni DTR kapasitesi", $"{invalidNewCapacityPercentage:P1}",
            "Projelendirilmiş yeni DTR kapasitesi mevcut DTR kapasitesinden küçük olamaz. Bu şart sağlamayan DTR'lar silinecektir."
                });
            }
        }

        private void ImputeFlagInvestmentYear()
        {
            Console.WriteLine("Starting ImputeFlagInvestmentYear...");
            int horizonYearValue = horizonYear;
            Console.WriteLine($"horizonYearValue: {horizonYearValue}");

            if (!currentDataTable.Columns.Contains("PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"))
            {
                Console.WriteLine("Error: Column PROJELENDIRILMIS_TRAFO_YATIRIM_YILI not found in currentDataTable.");
                return;
            }

            int imputedCount = 0;
            int totalRows = currentDataTable.Rows.Count;
            Console.WriteLine($"Total rows to process: {totalRows}");

            foreach (DataRow row in currentDataTable.Rows)
            {
                int rowIndex = currentDataTable.Rows.IndexOf(row);
                var investmentYearValue = row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"]?.ToString();
                Console.WriteLine($"Row {rowIndex}: PROJELENDIRILMIS_TRAFO_YATIRIM_YILI = '{investmentYearValue}'");

                if (IsNullLike(investmentYearValue))
                {
                    row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"] = "OPTIMIZE";
                    imputedCount++;
                    Console.WriteLine($"Row {rowIndex} imputed: Investment year was null-like.");
                }
                else if (!int.TryParse(investmentYearValue, out _))
                {
                    row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"] = "OPTIMIZE";
                    imputedCount++;
                    Console.WriteLine($"Row {rowIndex} imputed: Investment year {investmentYearValue} is not a valid integer.");
                }
            }

            float imputedPercentage = totalRows > 0 ? (float)imputedCount / totalRows : 0;
            Console.WriteLine($"ImputeFlagInvestmentYear: Imputed {imputedCount} rows out of {totalRows} ({imputedPercentage:P1}).");

            if (imputedPercentage > 0)
            {
                warningDataTable.Rows.Add(new object[]
                {
            "PROJELENDIRILMIS_TRAFO_YATIRIM_YILI", "Projelendirilmiş Trafo Yatırım Yılı", $"{imputedPercentage:P1}",
            $"Yatırım yılı NULL veya geçersiz olan {imputedCount} satır OPTIMIZE olarak güncellendi."
                });
                Console.WriteLine($"Added post-imputation message to warningDataTable. Total rows in warningDataTable: {warningDataTable.Rows.Count}");
            }

            Console.WriteLine("Finished ImputeFlagInvestmentYear.");
        }

        private void PreprocessMismatchedTrafoKoduByCondition()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            var validTrafos = new HashSet<string>(trafoDataTable.AsEnumerable()
                                          .Select(row => row["TRAFO_KODU"].ToString())
                                          .Distinct()
            );

            // Loop through currentDataTable to find invalid trafos and their indexes
            foreach (DataRow row in currentDataTable.Rows)
            {
                // "PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI" değerinin 1, 2 ya da 3 olup olmadığını kontrol et
                string yatırımSınıfı = row["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]?.ToString();
                if (yatırımSınıfı == "1" || yatırımSınıfı == "2" || yatırımSınıfı == "3")
                {
                    string connectedTrafo = row["PROJELENDIRILMIS_TRAFO_ID"].ToString();
                    if (IsNullLike(connectedTrafo))
                    {
                        // Değer null ya da boşsa işlem yapma
                    }
                    else if (!validTrafos.Contains(connectedTrafo))
                    {
                        row["PROJELENDIRILMIS_TRAFO_ID"] = "#N/A";
                    }
                }
            }
        }



        private Dictionary<string, (float Min, float Max)> minMaxCheckMap; // No longer readonly

        private Dictionary<string, (float Min, float Max)> CalculateCoordinateBounds()
        {
            DataTable trafoDataTable = dataTablesByType.ContainsKey("DTR Verileri") ? dataTablesByType["DTR Verileri"] : null;
            if (trafoDataTable == null || trafoDataTable.Rows.Count == 0 ||
                !trafoDataTable.Columns.Contains("TRAFO_X_KOORDINAT") ||
                !trafoDataTable.Columns.Contains("TRAFO_Y_KOORDINAT"))
            {
                return new Dictionary<string, (float Min, float Max)>
            {
                { "PROJELENDIRILMIS_TRAFO_X_KOORDINAT", (float.MinValue, float.MaxValue) },
                { "PROJELENDIRILMIS_TRAFO_Y_KOORDINAT", (float.MinValue, float.MaxValue) }
            };
            }

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (DataRow row in trafoDataTable.Rows)
            {
                if (float.TryParse(row["TRAFO_X_KOORDINAT"]?.ToString(), out float x))
                {
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                }
                if (float.TryParse(row["TRAFO_Y_KOORDINAT"]?.ToString(), out float y))
                {
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }
            }

            if (minX == float.MaxValue) minX = float.MinValue;
            if (maxX == float.MinValue) maxX = float.MaxValue;
            if (minY == float.MaxValue) minY = float.MinValue;
            if (maxY == float.MinValue) maxY = float.MaxValue;

            // Add a fixed buffer of ±0.001 to the bounds
            float buffer = 10f;
            minX -= buffer;
            maxX += buffer;
            minY -= buffer;
            maxY += buffer;

            return new Dictionary<string, (float Min, float Max)>
        {
            { "PROJELENDIRILMIS_TRAFO_X_KOORDINAT", (minX, maxX) },
            { "PROJELENDIRILMIS_TRAFO_Y_KOORDINAT", (minY, maxY) }
        };
        }


        private void ReportCoordinatesOutOfLimitsByCondition()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;
            List<int> outOfBoundsRows = new List<int>();

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (row["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]?.ToString() == "0")
                {
                    if (float.TryParse(row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"]?.ToString(), out float valueX) &&
                        float.TryParse(row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"]?.ToString(), out float valueY))
                    {
                        if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                        {
                            countOutOfThresholdCoordinates++;
                            outOfBoundsRows.Add(currentDataTable.Rows.IndexOf(row));
                        }
                    }
                }
            }

            columnNullRowsMap["COORDINATES_OUT_OF_BOUNDS"] = outOfBoundsRows;

            float outOfThresholdPercentage = (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count;

            if (outOfThresholdPercentage > 0)
            {
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                datatableLevel.Rows.Add(new object[] {
                "PROJELENDIRILMIS_TRAFO_X_KOORDINAT & PROJELENDIRILMIS_TRAFO_Y_KOORDINAT", "Koordinat Sınırları",
                $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
            });
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "PROJELENDIRILMIS_TRAFO_ID", INFO_ONLY},
            { "PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI", INFO_ONLY},
            { "PROJELENDIRILMIS_TRAFO_KAPASITE", INFO_ONLY},
            { "PROJELENDIRILMIS_TRAFO_YATIRIM_YILI", WARNING_ONLY},
        };
        private void ReportNullCounts()
        {
            float nullPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                    continue;

                // Special handling for PROJELENDIRILMIS_TRAFO_ID
                if (column.ColumnName == "PROJELENDIRILMIS_TRAFO_ID")
                {
                    int invalidCount = 0;
                    var invalidRows = new List<int>();

                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        int rowIndex = currentDataTable.Rows.IndexOf(row);
                        var trafoIdValue = row["PROJELENDIRILMIS_TRAFO_ID"]?.ToString();

                        if (IsNullLike(trafoIdValue) || trafoIdValue == "#N/A")
                        {
                            invalidCount++;
                            invalidRows.Add(rowIndex);
                            Console.WriteLine($"Row {rowIndex} flagged for deletion: PROJELENDIRILMIS_TRAFO_ID is {(IsNullLike(trafoIdValue) ? "null" : "#N/A")}.");
                        }
                    }

                    float invalidPercentage = totalRows > 0 ? (float)invalidCount / totalRows : 0;
                    Console.WriteLine($"ReportNullCounts (PROJELENDIRILMIS_TRAFO_ID): Found {invalidCount} invalid rows out of {totalRows} (Invalid: {invalidPercentage:P1}).");

                    if (invalidCount > 0)
                    {
                        columnNullRowsMap["PROJELENDIRILMIS_TRAFO_ID"] = invalidRows;
                        infoDataTable.Rows.Add(new object[]
                        {
                    column.ColumnName,
                            "Projelendirilmiş Trafo ID",
                            $"{invalidPercentage:P1}",
                    $"PROJELENDIRILMIS_TRAFO_ID sütununda {invalidCount} satır NULL veya #N/A değer içeriyor ve silinecek."
                        });
                        Console.WriteLine($"Added deletion message to infoDataTable for PROJELENDIRILMIS_TRAFO_ID. Total rows in infoDataTable: {infoDataTable.Rows.Count}");
                    }

                    continue;
                }

                // Special handling for PROJELENDIRILMIS_TRAFO_YATIRIM_YILI
                if (column.ColumnName == "PROJELENDIRILMIS_TRAFO_YATIRIM_YILI")
                {
                    int horizonYearValue = horizonYear; // Accesses slfStartYear from GirdiModülü

                    int nullOrInvalidCount = 0;
                    int rowsToDeleteCount = 0;
                    var rowsToDelete = new List<int>();

                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        int rowIndex = currentDataTable.Rows.IndexOf(row);
                        var investmentYearValue = row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"]?.ToString();

                        // Skip rows that have already been imputed to "OPTIMIZE"
                        if (investmentYearValue == "OPTIMIZE")
                        {
                            continue;
                        }

                        if (IsNullLike(investmentYearValue))
                        {
                            nullOrInvalidCount++;
                            Console.WriteLine($"Row {rowIndex} has null investment year.");
                        }
                        else if (int.TryParse(investmentYearValue, out int investmentYear))
                        {
                            if (investmentYear < horizonYearValue)
                            {
                                rowsToDeleteCount++;
                                rowsToDelete.Add(rowIndex);
                                Console.WriteLine($"Row {rowIndex} flagged for deletion: Investment year {investmentYear} is less than horizon year {horizonYearValue}.");
                            }
                        }
                        else
                        {
                            nullOrInvalidCount++;
                            Console.WriteLine($"Row {rowIndex} has invalid investment year: {investmentYearValue}.");
                        }
                    }

                    float nullOrInvalidPercentage = totalRows > 0 ? (float)nullOrInvalidCount / totalRows : 0;
                    float deletedPercentage = totalRows > 0 ? (float)rowsToDeleteCount / totalRows : 0;
                    Console.WriteLine($"ReportNullCounts (PROJELENDIRILMIS_TRAFO_YATIRIM_YILI): Found {nullOrInvalidCount} null/invalid rows, flagged {rowsToDeleteCount} rows for deletion out of {totalRows} (Null/Invalid: {nullOrInvalidPercentage:P1}, Deleted: {deletedPercentage:P1}).");

                    if (rowsToDeleteCount > 0)
                    {
                        columnNullRowsMap["YATIRIM_YILI_EARLY"] = rowsToDelete;
                        infoDataTable.Rows.Add(new object[]
                        {
                    column.ColumnName, "Projelendirilmiş Trafo Yatırım Yılı", $"{deletedPercentage:P1}",
                    $"{horizonYearValue} horizon periyodundan önceki {rowsToDeleteCount} satır silinecek."
                        });
                        Console.WriteLine($"Added deletion message to infoDataTable. Total rows in infoDataTable: {infoDataTable.Rows.Count}");
                    }

                    if (nullOrInvalidPercentage > 0)
                    {
                        warningDataTable.Rows.Add(new object[]
                        {
                    column.ColumnName, "Projelendirilmiş Trafo Yatırım Yılı", $"{nullOrInvalidPercentage:P1}",
                    $"Yatırım yılı NULL veya geçersiz olan {nullOrInvalidCount} satır OPTIMIZE olarak belirlenecek."
                        });
                        Console.WriteLine($"Added imputation message to warningDataTable. Total rows in warningDataTable: {warningDataTable.Rows.Count}");
                    }

                    continue;
                }

                // Special handling for PROJELENDIRILMIS_TRAFO_KAPASITE
                if (column.ColumnName == "PROJELENDIRILMIS_TRAFO_KAPASITE")
                {
                    List<int> nullRows = new List<int>();
                    int nullCount = 0;

                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        int rowIndex = currentDataTable.Rows.IndexOf(row);
                        string yatırımSınıfı = row["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]?.ToString();

                        // Skip null check if PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI is "0" (yeni)
                        if (yatırımSınıfı == "0")
                        {
                            Console.WriteLine($"Row {rowIndex} skipped: PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI is 0 (yeni), ignoring null check for PROJELENDIRILMIS_TRAFO_KAPASITE.");
                            continue;
                        }

                        var value = row[column]?.ToString();
                        if (IsNullLike(value) || value == "#N/A")
                        {
                            nullCount++;
                            nullRows.Add(rowIndex);
                            Console.WriteLine($"Row {rowIndex} flagged for deletion: {column.ColumnName} is {(IsNullLike(value) ? "null" : "#N/A")}.");
                        }
                    }

                    columnNullRowsMap[column.ColumnName] = nullRows;
                    nullPercentage = (float)nullCount / totalRows;

                    if (nullPercentage > 0)
                    {
                        var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                        var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        datatableLevel.Rows.Add(new object[]
                        {
                    column.ColumnName, "Null değer", $"{nullPercentage:P1}"
                        });
                        string tableName = string.IsNullOrEmpty(datatableLevel.TableName) ? "UnknownTable" : datatableLevel.TableName;
                        Console.WriteLine($"Added null message to {tableName} for {column.ColumnName}. Total rows in {tableName}: {datatableLevel.Rows.Count}");
                    }

                    continue;
                }

                // Default null check for other columns
                List<int> nullRowsDefault = new List<int>();
                int nullCountDefault = 0;

                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    var value = row[column]?.ToString();
                    if (IsNullLike(value) || value == "#N/A")
                    {
                        nullCountDefault++;
                        nullRowsDefault.Add(i);
                        Console.WriteLine($"Row {i} flagged for deletion: {column.ColumnName} is {(IsNullLike(value) ? "null" : "#N/A")}.");
                    }
                }

                columnNullRowsMap[column.ColumnName] = nullRowsDefault;
                nullPercentage = (float)nullCountDefault / totalRows;

                if (nullPercentage > 0)
                {
                    var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[]
                    {
                column.ColumnName, "Null değer", $"{nullPercentage:P1}"
                    });
                    string tableName = string.IsNullOrEmpty(datatableLevel.TableName) ? "UnknownTable" : datatableLevel.TableName;
                    Console.WriteLine($"Added null message to {tableName} for {column.ColumnName}. Total rows in {tableName}: {datatableLevel.Rows.Count}");
                }
            }
        }

        public override void Preprocess()
        {
            PreprocessMismatchedTrafoKoduByCondition();
        }

        public override void Validate()
        {
            Console.WriteLine("Starting Validate...");
            Console.WriteLine($"Before Validate: columnNullRowsMap has {columnNullRowsMap.Count} entries.");
            columnNullRowsMap.Clear();
            Console.WriteLine($"After Clear: columnNullRowsMap has {columnNullRowsMap.Count} entries.");
            base.Validate();
            minMaxCheckMap = CalculateCoordinateBounds();
            ReportNullCounts();
            ReportCoordinatesOutOfLimitsByCondition();
            MevcutDTRKapasiteCheck();
            Console.WriteLine("Finished Validate.");
        }

        public override void Impute()
        {
            Console.WriteLine("Starting Impute...");
            ImputeFlagInvestmentYear();
            Console.WriteLine("Finished Impute.");
        }

        public override void Remove()
        {
            Console.WriteLine("Starting Remove...");
            List<int> combinedRowsToRemoveList = new List<int>();

            if (columnNullRowsMap.ContainsKey("PROJELENDIRILMIS_TRAFO_ID"))
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["PROJELENDIRILMIS_TRAFO_ID"]);
            }
            if (columnNullRowsMap.ContainsKey("PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"))
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]);
            }
            if (columnNullRowsMap.ContainsKey("KAPASITE"))
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["KAPASITE"]);
            }
            if (columnNullRowsMap.ContainsKey("COORDINATES_OUT_OF_BOUNDS"))
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["COORDINATES_OUT_OF_BOUNDS"]);
            }
            if (columnNullRowsMap.ContainsKey("YATIRIM_YILI_EARLY"))
            {
                combinedRowsToRemoveList.AddRange(columnNullRowsMap["YATIRIM_YILI_EARLY"]);
            }

            var distinctRowsToRemove = combinedRowsToRemoveList.Distinct().ToList();
            Console.WriteLine($"Before Remove: currentDataTable has {currentDataTable.Rows.Count} rows.");
            Console.WriteLine($"Total rows to remove: {distinctRowsToRemove.Count}, including KAPASITE: {(columnNullRowsMap.ContainsKey("KAPASITE") ? columnNullRowsMap["KAPASITE"].Count : 0)}, YATIRIM_YILI_EARLY: {(columnNullRowsMap.ContainsKey("YATIRIM_YILI_EARLY") ? columnNullRowsMap["YATIRIM_YILI_EARLY"].Count : 0)}, PROJELENDIRILMIS_TRAFO_ID: {(columnNullRowsMap.ContainsKey("PROJELENDIRILMIS_TRAFO_ID") ? columnNullRowsMap["PROJELENDIRILMIS_TRAFO_ID"].Count : 0)}");

            RemoveCombinedRows(distinctRowsToRemove);
            Console.WriteLine($"After Remove: currentDataTable has {currentDataTable.Rows.Count} rows.");
            Console.WriteLine("Finished Remove.");
        }

        private void RemoveCombinedRows(List<int> rowsToRemoveList)
        {
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