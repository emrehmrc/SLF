using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public class EASarjModulu : GirdiModülü

    {
        // burada şarj istasyonu ad bilgisi yok ise bunu otomatik olarak doldurma yapmasının bir fonksiyonunu yazdım.
        // eğer bunu yaparsa null count yapacak mı bilmiyorum yapmasına gerek var mı onu da bilmiyorum.
        // data table'ın hangisini okuduğunu falan nasıl seçecek onu da bilmiyorum.

        //private void UpdateAdi(DataTable table)
        //{
        //    int counter = 1;

        //    foreach (DataRow row in table.Rows)
        //    {
        //        if (row["ADI"] == DBNull.Value || row["ADI"].ToString() == "#NA" || string.IsNullOrWhiteSpace(row["ADI"].ToString()))
        //        {
        //            row["ADI"] = $"EA_Şarj_{counter}";
        //            counter++;
        //        }
        //    }
        //}
    

        //burada şarj istasyonunun tipi olmaması durumunda istasyon gücüne bakıp tipi belirleyecek olan kodun fonksiyonunu yazdım.
        // data table'ın hangisini okuduğunu falan nasıl seçecek onu da bilmiyorum.

        //private void UpdateIstasyonTipi(DataTable table)
        //{
        //    foreach (DataRow row in table.Rows)
        //    {
        //        if (row["ISTASYON_TIPI"] == DBNull.Value)
        //        {
        //            int istasyonGucu = Convert.ToInt32(row["ISTASYON_GUCU"]);
        //            if (istasyonGucu <= 22)
        //            {
        //                row["ISTASYON_TIPI"] = 0;
        //            }
        //            else
        //            {
        //                row["ISTASYON_TIPI"] = 1;
        //            }
        //        }
        //    }
        //}


        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportCoordinatesOutOfLimits();

        }

        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.1f);

        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
        };

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "X_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "Y_KOORDINAT", WarningErrorBoundary(0.2f) },
        };
        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["Y_KOORDINAT"]?.ToString(), out float valueY))
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
                    "X_KOORDINAT & Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
                });
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
    }
}
