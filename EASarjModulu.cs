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
        // burada EA Şarj koordinatı bulunmuyorsa trafo koordinatını alıyor.

        //private void UpdateKoordinatlar(DataTable eaSektorTable, DataTable trafoTable)
        //{
        //    foreach (DataRow eaRow in eaSektorTable.Rows)
        //    {
        //        if (eaRow["X_KOORDINAT"] == DBNull.Value || eaRow["Y_KOORDINAT"] == DBNull.Value)
        //        {
        //            string eaTrafoKodu = eaRow["EA_TRAFO_KODU"].ToString();
        //            foreach (DataRow trafoRow in trafoTable.Rows)
        //            {
        //                if (trafoRow["TRAFO_KODU"].ToString() == eaTrafoKodu)
        //                {
        //                    eaRow["X_KOORDINAT"] = trafoRow["X_KOORDINAT"];
        //                    eaRow["Y_KOORDINAT"] = trafoRow["Y_KOORDINAT"];
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

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


        //EA Şarj istasyonları verisindeki trafo kodları ile trafo verilerindeki trafo kodlarını karşılaştıracak fonksiyon.
        //Burada bu tablolar ismi ile ilgili nasıl doğru kodu bulacak falan hiç bilmiyorum sor.

        //private void CheckTrafoKodlari(DataTable eaSektorTable, DataTable trafoTable)
        //{
        //    foreach (DataRow eaRow in eaSektorTable.Rows)
        //    {
        //        string eaTrafoKodu = eaRow["EA_TRAFO_KODU"].ToString();
        //        bool matchFound = false;

        //        foreach (DataRow trafoRow in trafoTable.Rows)
        //        {
        //            if (trafoRow["TRAFO_KODU"].ToString() == eaTrafoKodu)
        //            {
        //                matchFound = true;
        //                break;
        //            }
        //        }

        //        if (!matchFound)
        //        {
        //            Console.WriteLine($"Şarj istasyonu ile trafo eşlemesi yapılamamıştır: {eaTrafoKodu}");
        //        }
        //    }
        //}



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

        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "EA_X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "EA_Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
            
        };

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "EA_X_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "EA_Y_KOORDINAT", WarningErrorBoundary(0.2f) },
            { "ISTASYON_ADI", WARNING_ONLY},
            { "ISTASYON_GUCU", InfoErrorBoundary(0.2f) },
            { "EA_TRAFO_KODU", InfoErrorBoundary(0.2f) },
            { "ISTASYON_TIPI", WARNING_ONLY},
        };
        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["EA_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["EA_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["EA_X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["EA_Y_KOORDINAT"]?.ToString(), out float valueY))
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
                    "EA_X_KOORDINAT & EA_Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
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
