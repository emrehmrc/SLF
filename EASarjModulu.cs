using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SLF
{
    public class EASarjModulu : GirdiModülü

    {
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri" };
        private void PreprocessMismatchedTrafoKodu()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];    // datatablebytype ile birbirne baglı olana modullerin check ve imputasyonlar buradan başlıyor 
            var validTrafos = new HashSet<string>(trafoDataTable.AsEnumerable()
                                      .Select(row => row["TRAFO_KODU"].ToString())
                                      .Distinct()
            );

            // Loop through currentDataTable to find invalid trafos and their indexes
            foreach (DataRow row in currentDataTable.Rows)
            {
                string connectedTrafo = row["EA_TRAFO_KODU"].ToString();
                if (IsNullLike(connectedTrafo))
                {
                }
                else if (!validTrafos.Contains(connectedTrafo))
                {
                    row["EA_TRAFO_KODU"] = "#N/A";
                }
            }
        }
        private void ImputeCoordinate()
        {
            // "DTR Verileri" tablosunu al
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

            // Her satırı dolaş
            foreach (DataRow row in currentDataTable.Rows)
            {
                // "EA_X_Koordinat" değeri null ise
                if (IsNullLike(row["EA_X_KOORDINAT"]) || IsNullLike(row["EA_Y_KOORDINAT"]))
                {
                    string eaTrafoKodu = row["EA_TRAFO_KODU"].ToString();

                    // "DTR Verileri" tablosunda TRAFO_KODU'nu eşle
                    foreach (DataRow dtrRow in trafoDataTable.Rows)
                    {
                        if (dtrRow["TRAFO_KODU"].ToString() == eaTrafoKodu)
                        {
                            // "TRAFO_X_KOORDINAT" değerini al ve güncelle
                            row["EA_X_KOORDINAT"] = dtrRow["TRAFO_X_KOORDINAT"];
                            row["EA_Y_KOORDINAT"] = dtrRow["TRAFO_Y_KOORDINAT"];
                            break;
                        }
                    }
                }
            }
        }


        private const int AC_DC_THRESHOLD = 22;
        private const int AC_CONSTANT = 0;
        private const int DC_CONSTANT = 1;
        // burada şarj istasyonu ad bilgisi yok ise bunu otomatik olarak doldurma yapmasının bir fonksiyonunu yazdım.

        private void ImputeIstasyonAdı()
        {
            int counter = 1;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (IsNullLike(row["ISTASYON_ADI"]))
                {
                    row["ISTASYON_ADI"] = $"EA_Şarj_{counter}";
                    counter++;
                }
            }
        }


        //burada şarj istasyonunun tipi olmaması durumunda istasyon gücüne bakıp tipi belirleyecek olan kodun fonksiyonunu yazdım.

        private void ImputeIstasyonTipi()
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (IsNullLike(row["ISTASYON_TIPI"]))
                {
                    int istasyonGucu = Convert.ToInt32(row["ISTASYON_GUCU"]);
                    if (istasyonGucu <= AC_DC_THRESHOLD)
                    {
                        row["ISTASYON_TIPI"] = AC_CONSTANT;
                    }
                    else
                    {
                        row["ISTASYON_TIPI"] = DC_CONSTANT;
                    }
                }
            }
        }

        public override void Preprocess()
        {
            PreprocessMismatchedTrafoKodu();
        }
        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimits();

        }

        public override void Impute()
        {
            ImputeIstasyonAdı();

            ImputeIstasyonTipi();

            ImputeCoordinate();

            ImputeOutOfBoundsCoordinates(); // Add the new imputation step
        }
        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ISTASYON_GUCU"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["EA_TRAFO_KODU"]);


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

        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.1f);

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "EA_X_KOORDINAT", WARNING_ONLY },
            { "EA_Y_KOORDINAT", WARNING_ONLY },
            { "ISTASYON_ADI", WARNING_ONLY},
            { "ISTASYON_GUCU", InfoErrorBoundary(0.2f) },
            { "EA_TRAFO_KODU", InfoErrorBoundary(0.2f) },
            { "ISTASYON_TIPI", WARNING_ONLY},
        };
        private const float COORDINATE_BUFFER_PERCENTAGE = 10f; // 10% buffer for dynamic bounds

        private Dictionary<string, (float Min, float Max)> minMaxCheckMap;

        // New field to store out-of-bounds rows and their EA_TRAFO_KODU
        private Dictionary<int, string> outOfBoundsRowsToTrafoKodu;

        private void InitializeCoordinateBounds()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            if (trafoDataTable == null || trafoDataTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("Transformer data ('DTR Verileri') is missing or empty. Cannot compute coordinate bounds.");
            }

            // Extract all valid X and Y coordinates directly as double, then convert to float
            var xCoords = trafoDataTable.AsEnumerable()
                .Select(row => Convert.ToSingle(row.Field<double>("TRAFO_X_KOORDINAT")))
                .ToList();

            var yCoords = trafoDataTable.AsEnumerable()
                .Select(row => Convert.ToSingle(row.Field<double>("TRAFO_Y_KOORDINAT")))
                .ToList();

            if (xCoords.Count == 0 || yCoords.Count == 0)
            {
                throw new InvalidOperationException("No valid transformer coordinates found in 'DTR Verileri'. Cannot compute coordinate bounds.");
            }

            // Calculate min/max with buffer
            float xMin = xCoords.Min();
            float xMax = xCoords.Max();
            float yMin = yCoords.Min();
            float yMax = yCoords.Max();

            float xRange = xMax - xMin;
            float yRange = yMax - yMin;

            minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
            {
                { "EA_X_KOORDINAT", (xMin - xRange * COORDINATE_BUFFER_PERCENTAGE, xMax + xRange * COORDINATE_BUFFER_PERCENTAGE) },
                { "EA_Y_KOORDINAT", (yMin - yRange * COORDINATE_BUFFER_PERCENTAGE, yMax + yRange * COORDINATE_BUFFER_PERCENTAGE) }
            };
        }
        private void ReportCoordinatesOutOfLimits()
        {
            // Initialize the dictionary to store out-of-bounds rows
            outOfBoundsRowsToTrafoKodu = new Dictionary<int, string>();

            // Step 1: Initialize dynamic bounds for range check
            try
            {
                if (minMaxCheckMap == null)
                {
                    InitializeCoordinateBounds();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Log the error if bounds cannot be computed
                var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f); // Treat as error
                datatableLevel.Rows.Add(new object[]
                {
                "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                "Koordinat Sınırları",
                "N/A",
                ex.Message
                });
                return; // Cannot proceed with range check
            }

            // Step 2: Check if coordinates are within transformer bounds
            var (minXValue, maxXValue) = minMaxCheckMap["EA_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["EA_Y_KOORDINAT"];
            int outOfBoundsCount = 0;

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                bool isXValid = float.TryParse(row["EA_X_KOORDINAT"]?.ToString(), out float xValue);
                bool isYValid = float.TryParse(row["EA_Y_KOORDINAT"]?.ToString(), out float yValue);

                // Skip if coordinates are not parseable (null check is handled by ReportNullCounts)
                if (!isXValid || !isYValid)
                {
                    continue;
                }

                if (xValue < minXValue || xValue > maxXValue || yValue < minYValue || yValue > maxYValue)
                {
                    outOfBoundsCount++;
                    string eaTrafoKodu = row.Field<string>("EA_TRAFO_KODU");
                    if (!string.IsNullOrEmpty(eaTrafoKodu))
                    {
                        outOfBoundsRowsToTrafoKodu[i] = eaTrafoKodu;
                    }
                }
            }

            if (outOfBoundsCount > 0)
            {
                float outOfBoundsPercentage = (float)outOfBoundsCount / currentDataTable.Rows.Count;
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfBoundsPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                datatableLevel.Rows.Add(new object[]
                {
                "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                "Koordinat Sınırları",
                $"{outOfBoundsPercentage:P1}",
                $"EA_X_KOORDINAT ve/veya EA_Y_KOORDINAT parametresi ilgili trafo koordinat aralığında değil. Trafo koordinat aralığı dışına çıkılamaz. (Sınırlar: X [{minXValue}, {maxXValue}], Y [{minYValue}, {maxYValue}]). Hata oranı %10 üzeri değilse bu değerler imputasyon aşamasında düzeltilecektir."
                });
            }
        }

        // New method to impute out-of-bounds coordinates
        private void ImputeOutOfBoundsCoordinates()
        {
            if (outOfBoundsRowsToTrafoKodu == null || outOfBoundsRowsToTrafoKodu.Count == 0)
            {
                return; // Nothing to impute
            }

            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            if (trafoDataTable == null || trafoDataTable.Rows.Count == 0)
            {
                // Log a warning if transformer data is missing during imputation
                var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f); // Treat as error
                datatableLevel.Rows.Add(new object[]
                {
                "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                "Koordinat Imputasyonu",
                "N/A",
                "Transformer data ('DTR Verileri') is missing or empty. Cannot impute out-of-bounds coordinates."
                });
                return;
            }

            // Create a lookup for transformer coordinates to avoid nested loops
            var trafoLookup = trafoDataTable.AsEnumerable()
                .ToDictionary(
                    row => row.Field<string>("TRAFO_KODU"),
                    row => (X: row.Field<string>("TRAFO_X_KOORDINAT"), Y: row.Field<string>("TRAFO_Y_KOORDINAT")));

            // Impute out-of-bounds coordinates
            foreach (var kvp in outOfBoundsRowsToTrafoKodu)
            {
                int rowIndex = kvp.Key;
                string eaTrafoKodu = kvp.Value;

                if (rowIndex >= currentDataTable.Rows.Count)
                {
                    continue; // Skip if row index is out of bounds (e.g., after removals)
                }

                DataRow row = currentDataTable.Rows[rowIndex];
                if (trafoLookup.TryGetValue(eaTrafoKodu, out var coords))
                {
                    row["EA_X_KOORDINAT"] = coords.X;
                    row["EA_Y_KOORDINAT"] = coords.Y;
                }
                else
                {
                    // Log a warning if the transformer code is not found
                    var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f); // Treat as error
                    datatableLevel.Rows.Add(new object[]
                    {
                    "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                    "Koordinat Imputasyonu",
                    "N/A",
                    $"Transformer code '{eaTrafoKodu}' not found in 'DTR Verileri'. Cannot impute coordinates for row {rowIndex}."
                    });
                }
            }

            // Clear the dictionary after imputation
            outOfBoundsRowsToTrafoKodu.Clear();
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
    }
}