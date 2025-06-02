using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SLF
{
    public class DEKModulu : GirdiModülü

    {
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri" };

        private void ReportKuruluGuc()
        {
            // 1000'den büyük değerlerin yüzdesi için bir değişken tanımla ve başlangıç değeri olarak 0.0f ata
            float invalidPercentage = 0.0f;

            // currentDataTable'daki toplam satır sayısını al
            int totalRows = currentDataTable.Rows.Count;

            // Hangi kolonun kontrol edileceğini belirle
            string column = "DEK_KURULU_GUCU";

            // Geçersiz satırları tutmak için bir liste tanımla
            List<int> invalidRows = new List<int>();

            // Geçersiz değerlerin sayısını tutmak için bir değişken tanımla ve başlangıç değeri olarak 0 ata
            int invalidCount = 0;

            // Tüm satırlar üzerinden döngü başlat
            for (int i = 0; i < totalRows; i++)
            {
                // Mevcut satırı al
                var row = currentDataTable.Rows[i];

                // Satırın belirtilen kolonundaki değeri al
                var cellValue = row[column]?.ToString();

                // Değer null veya boş değilse
                if (!IsNullLike(cellValue))
                {
                    // Değerin sayıya çevrilebilir olup olmadığını kontrol et
                    if (int.TryParse(cellValue, out int value))
                    {
                        // Eğer değer 1000'den büyükse
                        if (value > 1000)
                        {
                            // Geçersiz değer sayısını artır
                            invalidCount++;
                            // Geçersiz satırların listesine satır numarasını ekle
                            invalidRows.Add(i);
                        }
                    }
                }
            }

            // Geçersiz değer yüzdesini hesapla
            invalidPercentage = (float)invalidCount / totalRows;

            // Eğer geçersiz değer yüzdesi 0'dan büyükse
            if (invalidPercentage > 0)
            {
                // statDataTable'a yeni bir satır ekle. Burada "DEK_KURULU_GUCU", "Kurulu gücü 1000kVA'dan büyük olan {invalidCount} kadar DEK'ler mevcuttur. Eğer düzeltilmezse bu durum doğru kabul edilecektir." bilgisi eklenir
                statDataTable.Rows.Add(new object[] {
            "DEK_KURULU_GUCU", "Hatalı sayı", $"{invalidPercentage:P1}", $"Kurulu gücü 1000kVA'dan büyük olan {invalidCount} kadar DEK'ler mevcuttur. Eğer düzeltilmezse bu durum doğru kabul edilecektir."
        });
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
                if (IsNullLike(row["DEK_X_KOORDINAT"]) || IsNullLike(row["DEK_Y_KOORDINAT"]))
                {
                    string DEKTrafoKodu = row["DEK_BAGLANDIGI_TRAFO_KODU"].ToString();

                    // "DTR Verileri" tablosunda TRAFO_KODU'nu eşle
                    foreach (DataRow dtrRow in trafoDataTable.Rows)
                    {
                        if (dtrRow["TRAFO_KODU"].ToString() == DEKTrafoKodu)
                        {
                            // "TRAFO_X_KOORDINAT" değerini al ve güncelle
                            row["DEK_X_KOORDINAT"] = dtrRow["TRAFO_X_KOORDINAT"];
                            row["DEK_Y_KOORDINAT"] = dtrRow["TRAFO_Y_KOORDINAT"];
                            break;
                        }
                    }
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
                string connectedTrafo = row["DEK_BAGLANDIGI_TRAFO_KODU"].ToString();
                if (IsNullLike(connectedTrafo))
                {
                }
                else if (!validTrafos.Contains(connectedTrafo))
                {
                    row["DEK_BAGLANDIGI_TRAFO_KODU"] = "#N/A";
                }
            }
        }
        private void ImputeKaynakTipi()
        {
            // "KAYNAK_TIPI" kolonundaki null değerleri "GES" ile doldurma
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Eğer KAYNAK_TIPI kolonu null ise
                if (IsNullLike(row["KAYNAK_TIPI"]))
                {
                    // "GES" ile doldur
                    row["KAYNAK_TIPI"] = "GES";
                }
            }
        }
        private void ImputeIlceAdi()
        {
            // "ILCE_ADI" kolonundaki null değerleri sayma ve en çok tekrarlanan değeri bulma
            var mostFrequentValue = currentDataTable.AsEnumerable()
                                                    // ILCE_ADI kolonundaki null olmayan değerleri seç
                                                    .Where(row => !IsNullLike(row["ILCE_ADI"]))
                                                    // Bu değerleri grupla
                                                    .GroupBy(row => row["ILCE_ADI"])
                                                    // Grupları tekrar sayısına göre sırala (azalan)
                                                    .OrderByDescending(g => g.Count())
                                                    // İlk grubu (en çok tekrarlanan değeri) seç
                                                    .FirstOrDefault()?.Key;

            // Eğer en çok tekrarlanan değer null ise fonksiyondan çık
            if (mostFrequentValue == null)
            {
                return;
            }

            // "ILCE_ADI" kolonundaki null değerleri en çok kullanılan değer ile doldurma
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Eğer ILCE_ADI kolonu null ise
                if (IsNullLike(row["ILCE_ADI"]))
                {
                    // En çok kullanılan değer ile doldur
                    row["ILCE_ADI"] = mostFrequentValue;
                }
            }
        }

        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.05f);
        private Dictionary<string, (float Min, float Max)> minMaxCheckMap;

        // Helper method to calculate dynamic bounds (unchanged from previous)
        private void InitializeCoordinateBounds()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            if (trafoDataTable == null || trafoDataTable.Rows.Count == 0)
            {
                minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
                {
                    { "DEK_X_KOORDINAT", (float.MinValue, float.MaxValue) },
                    { "DEK_Y_KOORDINAT", (float.MinValue, float.MaxValue) }
                };
                return;
            }

            var xCoords = trafoDataTable.AsEnumerable()
                .Select(row => float.TryParse(row["TRAFO_X_KOORDINAT"]?.ToString(), out float x) ? x : float.NaN)
                .Where(x => !float.IsNaN(x))
                .ToList();

            var yCoords = trafoDataTable.AsEnumerable()
                .Select(row => float.TryParse(row["TRAFO_Y_KOORDINAT"]?.ToString(), out float y) ? y : float.NaN)
                .Where(y => !float.IsNaN(y))
                .ToList();

            if (xCoords.Count == 0 || yCoords.Count == 0)
            {
                minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
                {
                    { "DEK_X_KOORDINAT", (float.MinValue, float.MaxValue) },
                    { "DEK_Y_KOORDINAT", (float.MinValue, float.MaxValue) }
                };
                return;
            }

            float minX = xCoords.Min();
            float maxX = xCoords.Max();
            float xRange = maxX - minX;
            float xTolerance = xRange * 10f;

            float minY = yCoords.Min();
            float maxY = yCoords.Max();
            float yRange = maxY - minY;
            float yTolerance = yRange * 10f;

            minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
            {
                { "DEK_X_KOORDINAT", (minX - xTolerance, maxX + xTolerance) },
                { "DEK_Y_KOORDINAT", (minY - yTolerance, maxY + yTolerance) }
            };
        }

        private void ReportCoordinatesOutOfLimits()
        {
            // Ensure bounds are initialized
            if (minMaxCheckMap == null)
            {
                InitializeCoordinateBounds();
            }

            if (currentDataTable.Rows.Count == 0) return; // Avoid division by zero

            var (minXValue, maxXValue) = minMaxCheckMap["DEK_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["DEK_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = currentDataTable.AsEnumerable()
                .Count(row =>
                    float.TryParse(row["DEK_X_KOORDINAT"]?.ToString(), out float valueX) &&
                    float.TryParse(row["DEK_Y_KOORDINAT"]?.ToString(), out float valueY) &&
                    (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue));

            float outOfThresholdPercentage = (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count;
            if (outOfThresholdPercentage > 0)
            {
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                datatableLevel.Rows.Add(new object[] {
            "DEK_X_KOORDINAT & DEK_Y_KOORDINAT",
            "Koordinat Sınırları",
            $"{outOfThresholdPercentage:P1}",
            $"DEK_X_KOORDINAT ve/veya DEK_Y_KOORDINAT parametresi ilgili trafo koordinat aralığında değil. (X: {minXValue:F2} to {maxXValue:F2}, Y: {minYValue:F2} to {maxYValue:F2}) for {countOutOfThresholdCoordinates}. Hata oranı %10'dan fazla değilse bu değerler veri doldurma aşamasında düzeltilecektir."
        });
            }
        }
        // New method to impute out-of-bound coordinates
        private void ImputeOutOfBoundCoordinates()
        {
            if (minMaxCheckMap == null)
            {
                InitializeCoordinateBounds();
            }

            if (currentDataTable.Rows.Count == 0) return;

            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            if (trafoDataTable == null) return;

            // Create a lookup for transformer coordinates
            var trafoLookup = trafoDataTable.AsEnumerable()
                .ToDictionary(
                    row => row["TRAFO_KODU"].ToString(),
                    row => (X: row["TRAFO_X_KOORDINAT"], Y: row["TRAFO_Y_KOORDINAT"])
                );

            var (minXValue, maxXValue) = minMaxCheckMap["DEK_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["DEK_Y_KOORDINAT"];

            int correctedCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                bool isXValid = float.TryParse(row["DEK_X_KOORDINAT"]?.ToString(), out float valueX);
                bool isYValid = float.TryParse(row["DEK_Y_KOORDINAT"]?.ToString(), out float valueY);

                bool isOutOfBounds = (isXValid && (valueX < minXValue || valueX > maxXValue)) ||
                                     (isYValid && (valueY < minYValue || valueY > maxYValue));

                if (isOutOfBounds)
                {
                    string trafoCode = row["DEK_BAGLANDIGI_TRAFO_KODU"]?.ToString();
                    if (!string.IsNullOrEmpty(trafoCode) && trafoLookup.TryGetValue(trafoCode, out var coords))
                    {
                        row["DEK_X_KOORDINAT"] = coords.X;
                        row["DEK_Y_KOORDINAT"] = coords.Y;
                        correctedCount++;
                    }
                }
            }

            // Optional reporting
            if (correctedCount > 0)
            {
                float correctedPercentage = (float)correctedCount / currentDataTable.Rows.Count;
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(correctedPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                datatableLevel.Rows.Add(new object[] {
            "DEK_X_KOORDINAT & DEK_Y_KOORDINAT",
            "Koordinat Düzeltme",
            $"{correctedPercentage:P1}",
            $"{correctedCount} sınır dışı koordinat, trafo verileri kullanılarak düzeltildi."
        });
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "ILCE_ADI", WARNING_ONLY},
            { "KAYNAK_TIPI", WARNING_ONLY},
            { "DEK_KURULU_GUCU", ERROR_ONLY},
            { "DEK_BAGLANDIGI_TRAFO_KODU", ERROR_ONLY},
            { "DEK_X_KOORDINAT", WARNING_ONLY},
            { "DEK_Y_KOORDINAT", WARNING_ONLY},
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

        public override void Preprocess()
        {
            PreprocessMismatchedTrafoKodu();
        }
        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimits();

            ReportKuruluGuc();
        }
        public override void Impute()
        {
            ImputeIlceAdi();

            ImputeKaynakTipi();

            ImputeOutOfBoundCoordinates();

            ImputeCoordinate();
        }

    }
}