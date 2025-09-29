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
            // Güvenlik kontrolleri
            if (currentDataTable == null)
            {
                Console.WriteLine("currentDataTable is null");
                return;
            }
            if (!dataTablesByType.ContainsKey("DTR Verileri") || dataTablesByType["DTR Verileri"] == null)
            {
                Console.WriteLine("DTR Verileri table not found");
                return;
            }
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

            // Performans için Dictionary oluştur (O(1) lookup)
            var trafoLookup = new Dictionary<string, (double? x, double? y)>();
            foreach (DataRow dtrRow in trafoDataTable.Rows)
            {
                string trafoKodu = dtrRow.Field<string>("TRAFO_KODU") ?? string.Empty;
                if (!string.IsNullOrEmpty(trafoKodu) && !trafoLookup.ContainsKey(trafoKodu))
                {
                    double? xCoord = dtrRow.Field<double?>("TRAFO_X_KOORDINAT");
                    double? yCoord = dtrRow.Field<double?>("TRAFO_Y_KOORDINAT");
                    trafoLookup[trafoKodu] = (xCoord, yCoord);
                }
            }

            // Ana döngü - şimdi O(n) complexity
            int updatedCount = 0;
            foreach (DataRow row in currentDataTable.Rows)
            {
                try
                {
                    if (IsNullLike(row["EA_X_KOORDINAT"]) || IsNullLike(row["EA_Y_KOORDINAT"]))
                    {
                        string eaTrafoKodu = row.Field<string>("EA_TRAFO_KODU") ?? string.Empty;
                        if (!string.IsNullOrEmpty(eaTrafoKodu) && trafoLookup.TryGetValue(eaTrafoKodu, out var coords))
                        {
                            bool wasUpdated = false;

                            // Sadece null olanları güncelle
                            if (IsNullLike(row["EA_X_KOORDINAT"]) && coords.x.HasValue)
                            {
                                row["EA_X_KOORDINAT"] = coords.x.Value;
                                wasUpdated = true;
                            }
                            if (IsNullLike(row["EA_Y_KOORDINAT"]) && coords.y.HasValue)
                            {
                                row["EA_Y_KOORDINAT"] = coords.y.Value;
                                wasUpdated = true;
                            }

                            // Sadece gerçekten güncelleme yapıldıysa say
                            if (wasUpdated)
                            {
                                updatedCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row: {ex.Message}");
                    // Devam et, bir satırdaki hata tüm işlemi durdurmasın
                }
            }
            Console.WriteLine($"Imputed coordinates for {updatedCount} records");
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

        /*private void ImputeIstasyonTipi()
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
        }*/

        // ✅ DAHA KISA VE GÜVENLİ VERSİYON
        private void ImputeIstasyonTipi()
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (IsNullLike(row["ISTASYON_TIPI"]))
                {
                    // Güvenli int dönüştürme helper metodu
                    int istasyonGucu = GetSafeInt(row, "ISTASYON_GUCU", 0);

                    // Threshold kontrolü ve atama
                    row["ISTASYON_TIPI"] = istasyonGucu <= AC_DC_THRESHOLD ? AC_CONSTANT : DC_CONSTANT;
                }
            }
        }

        // ✅ HELPER METODLAR
        private int GetSafeInt(DataRow row, string columnName, int defaultValue = 0)
        {
            try
            {
                if (row[columnName] == null || row[columnName] == DBNull.Value)
                    return defaultValue;

                if (int.TryParse(row[columnName].ToString(), out int result))
                    return result;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
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

        /*private void InitializeCoordinateBounds()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

            if (trafoDataTable == null || trafoDataTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("Transformer data ('DTR Verileri') is missing or empty. Cannot compute coordinate bounds.");
            }

            var xCoords = trafoDataTable.AsEnumerable()
                .Select(row => row.Field<double?>("TRAFO_X_KOORDINAT"))
                .Where(x => x.HasValue)
                .Select(x => (float)x.Value)
                .ToList();

            var yCoords = trafoDataTable.AsEnumerable()
                .Select(row => row.Field<double?>("TRAFO_Y_KOORDINAT"))
                .Where(y => y.HasValue)
                .Select(y => (float)y.Value)
                .ToList();

            if (xCoords.Count == 0 || yCoords.Count == 0)
            {
                throw new InvalidOperationException("No valid transformer coordinates found in 'DTR Verileri'. Cannot compute coordinate bounds.");
            }

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
        */

        // ✅ DÜZELTİLMİŞ VE GÜVENLİ VERSİYON
        private void InitializeCoordinateBounds()
        {
            try
            {
                // Dictionary kontrolü
                if (!dataTablesByType.ContainsKey("DTR Verileri"))
                {
                    throw new InvalidOperationException("'DTR Verileri' anahtarı dataTablesByType içinde bulunamadı.");
                }

                DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                if (trafoDataTable == null || trafoDataTable.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Transformer data ('DTR Verileri') is missing or empty. Cannot compute coordinate bounds.");
                }

                // Sütunların varlığını kontrol et
                string[] requiredColumns = { "TRAFO_X_KOORDINAT", "TRAFO_Y_KOORDINAT" };
                foreach (string column in requiredColumns)
                {
                    if (!trafoDataTable.Columns.Contains(column))
                    {
                        throw new InvalidOperationException($"Required column '{column}' not found in DTR Verileri table.");
                    }
                }

                // Güvenli koordinat okuma - farklı veri tiplerini handle et
                var xCoords = GetSafeCoordinates(trafoDataTable, "TRAFO_X_KOORDINAT");
                var yCoords = GetSafeCoordinates(trafoDataTable, "TRAFO_Y_KOORDINAT");

                // Koordinat validasyonu
                if (xCoords.Count == 0 || yCoords.Count == 0)
                {
                    throw new InvalidOperationException($"No valid transformer coordinates found in 'DTR Verileri'. X count: {xCoords.Count}, Y count: {yCoords.Count}");
                }

                // Min/Max hesaplama
                double xMin = xCoords.Min();
                double xMax = xCoords.Max();
                double yMin = yCoords.Min();
                double yMax = yCoords.Max();

                // Sıfır range kontrolü
                double xRange = xMax - xMin;
                double yRange = yMax - yMin;

                if (xRange == 0 || yRange == 0)
                {
                    throw new InvalidOperationException($"Invalid coordinate range detected. X range: {xRange}, Y range: {yRange}");
                }

                // Buffer percentage kontrolü
                double bufferPercentage = COORDINATE_BUFFER_PERCENTAGE;
                if (bufferPercentage < 0 || bufferPercentage > 1.0)
                {
                    throw new InvalidOperationException($"Invalid COORDINATE_BUFFER_PERCENTAGE: {bufferPercentage}. Should be between 0 and 1.");
                }

                // Float dönüştürme - precision kontrolü ile
                float xMinFloat = SafeDoubleToFloat(xMin - xRange * bufferPercentage);
                float xMaxFloat = SafeDoubleToFloat(xMax + xRange * bufferPercentage);
                float yMinFloat = SafeDoubleToFloat(yMin - yRange * bufferPercentage);
                float yMaxFloat = SafeDoubleToFloat(yMax + yRange * bufferPercentage);

                // Dictionary oluşturma
                minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "EA_X_KOORDINAT", (xMinFloat, xMaxFloat) },
            { "EA_Y_KOORDINAT", (yMinFloat, yMaxFloat) }
        };

               
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error initializing coordinate bounds: {ex.Message}", ex);
            }
        }

        private float SafeDoubleToFloat(double value)
        {
            // Double'ın float aralığında olup olmadığını kontrol et
            if (value > float.MaxValue)
                return float.MaxValue;
            if (value < float.MinValue)
                return float.MinValue;
            if (double.IsNaN(value))
                return float.NaN;
            if (double.IsPositiveInfinity(value))
                return float.PositiveInfinity;
            if (double.IsNegativeInfinity(value))
                return float.NegativeInfinity;

            return (float)value;
        }


        // ✅ HELPER METODLAR
        private List<double> GetSafeCoordinates(DataTable table, string columnName)
        {
            var coordinates = new List<double>();

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    object value = row[columnName];

                    // Null ve DBNull kontrolü
                    if (value == null || value == DBNull.Value)
                        continue;

                    // Farklı veri tiplerini handle et
                    double coord = 0;

                    if (value is double doubleValue)
                    {
                        coord = doubleValue;
                    }
                    else if (value is float floatValue)
                    {
                        coord = floatValue;
                    }
                    else if (value is decimal decimalValue)
                    {
                        coord = (double)decimalValue;
                    }
                    else if (value is int intValue)
                    {
                        coord = intValue;
                    }
                    else if (value is string stringValue)
                    {
                        if (!double.TryParse(stringValue, out coord))
                            continue; // Geçersiz string değeri atla
                    }
                    else
                    {
                        // Diğer tipler için Convert.ToDouble dene
                        if (!double.TryParse(value.ToString(), out coord))
                            continue;
                    }

                    // Koordinat validasyonu
                    if (!double.IsNaN(coord) && !double.IsInfinity(coord))
                    {
                        coordinates.Add(coord);
                    }
                }
                catch (Exception ex)
                {
                    // Hatalı satırı logla ama işleme devam et
                    Console.WriteLine($"Error processing coordinate in column {columnName}: {ex.Message}");
                }
            }

            return coordinates;
        }

        private void ReportCoordinatesOutOfLimits()
        {
            outOfBoundsRowsToTrafoKodu = new Dictionary<int, string>();

            try
            {
                if (minMaxCheckMap == null)
                {
                    InitializeCoordinateBounds();
                }
            }
            catch (InvalidOperationException ex)
            {
                var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f);
                datatableLevel.Rows.Add(new object[] { "EA_X_KOORDINAT & EA_Y_KOORDINAT", "Koordinat Sınırları", "N/A", ex.Message });
                return;
            }

            var (minXValue, maxXValue) = minMaxCheckMap["EA_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["EA_Y_KOORDINAT"];
            int outOfBoundsCount = 0;

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                double? xValue = row.Field<double?>("EA_X_KOORDINAT");
                double? yValue = row.Field<double?>("EA_Y_KOORDINAT");

                if (!xValue.HasValue || !yValue.HasValue)
                {
                    continue;
                }

                if (xValue.Value < minXValue || xValue.Value > maxXValue || yValue.Value < minYValue || yValue.Value > maxYValue)
                {
                    outOfBoundsCount++;
                    string eaTrafoKodu = row.Field<string>("EA_TRAFO_KODU") ?? string.Empty;
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
                datatableLevel.Rows.Add(new object[] {
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
                var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f);
                datatableLevel.Rows.Add(new object[]
                {
            "EA_X_KOORDINAT & EA_Y_KOORDINAT",
            "Koordinat Imputasyonu",
            "N/A",
            "Transformer data ('DTR Verileri') is missing or empty. Cannot impute out-of-bounds coordinates."
                });
                return;
            }

            // Güvenli lookup oluştur
            var trafoLookup = new Dictionary<string, (double? X, double? Y)>();

            foreach (DataRow trafoRow in trafoDataTable.Rows)
            {
                try
                {
                    string trafoKodu = trafoRow.Field<string>("TRAFO_KODU");
                    if (!string.IsNullOrEmpty(trafoKodu) && !trafoLookup.ContainsKey(trafoKodu))
                    {
                        // Koordinatları uygun tipe çevir
                        double? xCoord = null;
                        double? yCoord = null;

                        var xValue = trafoRow["TRAFO_X_KOORDINAT"];
                        var yValue = trafoRow["TRAFO_Y_KOORDINAT"];

                        if (xValue != null && xValue != DBNull.Value)
                        {
                            if (double.TryParse(xValue.ToString(), out double x))
                            {
                                xCoord = x;
                            }
                        }

                        if (yValue != null && yValue != DBNull.Value)
                        {
                            if (double.TryParse(yValue.ToString(), out double y))
                            {
                                yCoord = y;
                            }
                        }

                        trafoLookup[trafoKodu] = (xCoord, yCoord);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing trafo row: {ex.Message}");
                }
            }

            // Out-of-bounds koordinatları impute et
            int successCount = 0;
            int failCount = 0;

            foreach (var kvp in outOfBoundsRowsToTrafoKodu.ToList()) // ToList() ile safe iteration
            {
                int rowIndex = kvp.Key;
                string eaTrafoKodu = kvp.Value;

                if (rowIndex >= currentDataTable.Rows.Count || rowIndex < 0)
                {
                    failCount++;
                    continue; // Skip if row index is out of bounds
                }

                try
                {
                    DataRow row = currentDataTable.Rows[rowIndex];

                    if (!string.IsNullOrEmpty(eaTrafoKodu) && trafoLookup.TryGetValue(eaTrafoKodu, out var coords))
                    {
                        // Sadece geçerli koordinatları ata
                        if (coords.X.HasValue)
                        {
                            row["EA_X_KOORDINAT"] = coords.X.Value;
                        }
                        if (coords.Y.HasValue)
                        {
                            row["EA_Y_KOORDINAT"] = coords.Y.Value;
                        }

                        if (coords.X.HasValue || coords.Y.HasValue)
                        {
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                            // Log koordinat bulunamadı
                            var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f);
                            datatableLevel.Rows.Add(new object[]
                            {
                        "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                        "Koordinat Imputasyonu",
                        "N/A",
                        $"Transformer '{eaTrafoKodu}' found but coordinates are null/invalid."
                            });
                        }
                    }
                    else
                    {
                        failCount++;
                        // Log trafo bulunamadı
                        var datatableLevel = GetDataTableBasedOnThreshold(1.0f, 0.0f, 0.0f);
                        datatableLevel.Rows.Add(new object[]
                        {
                    "EA_X_KOORDINAT & EA_Y_KOORDINAT",
                    "Koordinat Imputasyonu",
                    "N/A",
                    $"Transformer code '{eaTrafoKodu}' not found in 'DTR Verileri'. Cannot impute coordinates for row {rowIndex}."
                        });
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.WriteLine($"Error imputing coordinates for row {rowIndex}: {ex.Message}");
                }
            }

            Console.WriteLine($"Coordinate imputation completed: {successCount} success, {failCount} failed");

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