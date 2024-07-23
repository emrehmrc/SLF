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
    public class YeniProjelendirilmisDTR : GirdiModülü

    {
        int horizonYear = 2035;

        private void ImputeFlagInvestmentYear()  // BU FONKSIYON ŞU AN HORIZON YEAR'I STATIK ALIYOR VE BUNU OPTIMIZE OLARAK FLAG EDIYOR.
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                var investmentYearValue = row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"]?.ToString();

                // Yatırım yılı değeri null ise veya horizonYear'dan büyük ise
                if (IsNullLike(investmentYearValue) || (int.TryParse(investmentYearValue, out int investmentYear) && investmentYear > horizonYear))
                {
                    // Değeri "OPTIMIZE" olarak değiştir
                    row["PROJELENDIRILMIS_TRAFO_YATIRIM_YILI"] = "OPTIMIZE";
                }
            }
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
                    string connectedTrafo = row["PROJELENDIRILMIS_TRAFO_PROJE_KODU"].ToString();
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

        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = InfoErrorBoundary(1f);  // BUNDAN EMİN DEĞİLİM KONTROL ETMEK LAZIM!!!

        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "PROJELENDIRILMIS_TRAFO_X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "PROJELENDIRILMIS_TRAFO_Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
            
        };
        private void ReportCoordinatesOutOfLimitsByCondition()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                // "PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI" 0 değerine eşit mi kontrol et
                if (row["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]?.ToString() == "0")
                {
                    // X ve Y koordinatlarını kontrol et
                    if (float.TryParse(row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"]?.ToString(), out float valueY))
                    {
                        if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                        {
                            countOutOfThresholdCoordinates++;
                        }
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
            "PROJELENDIRILMIS_TRAFO_X_KOORDINAT & PROJELENDIRILMIS_TRAFO_Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
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
            PreprocessMismatchedTrafoKoduByCondition();
        }
        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimitsByCondition();
        }
        public override void Impute()
        {
            ImputeFlagInvestmentYear();
        }
        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["PROJELENDIRILMIS_TRAFO_ID"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["PROJELENDIRILMIS_TRAFO_KAPASITE"]);


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


