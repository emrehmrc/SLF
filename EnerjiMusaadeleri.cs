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
    public class EnerjiMusaadeleri : GirdiModülü

    {
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri"};
        private void ImputeMustakilOlmayanTrafoID()
        {
            // "DTR Verileri" tablosundan trafo bilgilerini al
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            var trafoList = trafoDataTable.AsEnumerable()
                                          .Select(row => new
                                          {
                                              TrafoKodu = row["TRAFO_KODU"].ToString(),
                                              TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                                              TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                                          })
                                          .ToList();

            foreach (DataRow row in currentDataTable.Rows)
            {
                // "ENERJI_MUSAADE_GERILIM_SEVIYESI" değerine bakar ve "AG" ise kontrol eder
                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) && row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                {
                    // "ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL" değeri 0 ise kontrol eder
                    if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) && row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                    {
                        string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();
                        // "ENERJI_MUSAADE_X_KOORDINAT" veya "ENERJI_MUSAADE_Y_KOORDINAT" değeri IsNullLike ise kontrol eder
                        if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                        {
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                            continue;
                        }

                        // "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID" değeri IsNullLike ise veya geçerli trafo kodları arasında değilse kontrol eder
                        if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                        {
                            // En yakın trafoyu bul ve "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID" olarak ayarla
                            double enYakinMesafe = double.MaxValue;
                            string enYakinTrafoKodu = null;
                            double musadeXKoordinat = Convert.ToDouble(row["ENERJI_MUSAADE_X_KOORDINAT"]);
                            double musadeYKoordinat = Convert.ToDouble(row["ENERJI_MUSAADE_Y_KOORDINAT"]);

                            foreach (var trafo in trafoList)
                            {
                                double mesafe = Math.Sqrt(Math.Pow(trafo.TrafoXKoordinat - musadeXKoordinat, 2) + Math.Pow(trafo.TrafoYKoordinat - musadeYKoordinat, 2));

                                if (mesafe < enYakinMesafe)
                                {
                                    enYakinMesafe = mesafe;
                                    enYakinTrafoKodu = trafo.TrafoKodu;
                                }
                            }

                            if (enYakinTrafoKodu != null)
                            {
                                row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                            }
                            else
                            {
                                row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                            }
                        }
                    }
                }
            }
        }
        private void ReportOGBaglanacagiTrafo()
        {
            int invalidCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                // "ENERJI_MUSAADE_GERILIM_SEVIYESI" değerine bakar ve "OG" ise kontrol eder
                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) && row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "OG")
                {
                    // "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID" değeri IsNullLike ise kontrol eder
                    if (IsNullLike(row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]))
                    {
                        // "ENERJI_MUSAADE_GERILIM_SEVIYESI" değerini #N/A yapar
                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                        invalidCount++;
                    }
                }
            }

            if (invalidCount > 0)
            {
                float invalidPercentage = (float)invalidCount / currentDataTable.Rows.Count;
                infoDataTable.Rows.Add(new object[] {
                "", "Orta gerilim seviyesinden bağlı olan", $"{invalidCount} enerji müsaadesinin bağlanacağı trafo verisi bulunmamaktadır. Yüzde: {invalidPercentage:P1}"
            });
            }
        
    }
    private void ImputeOnay()
        {
            // currentDataTable'ın tüm satırlarını dolaş
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Eğer ENERJI_MUSAADE_TALEP_DURUMU kolonu IsNullLike metoduna göre null ise
                if (IsNullLike(row["ENERJI_MUSAADE_TALEP_DURUMU"]))
                {
                    // ENERJI_MUSAADE_TALEP_DURUMU kolonunu 1 olarak güncelle
                    row["ENERJI_MUSAADE_TALEP_DURUMU"] = 0;
                }
            }
        }

        private void ImputeEnerjilendirmeYılı()
        {
            // currentDataTable'ın tüm satırlarını dolaş
            foreach (DataRow row in currentDataTable.Rows)
            {
                // Eğer ENERJI_MUSAADE_ENERJILENDIRME_YILI kolonu IsNullLike metoduna göre null ise
                if (IsNullLike(row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"]))
                {
                    // ENERJI_MUSAADE_ENERJILENDIRME_YILI değerini horizon ilk yıl olarak güncelle
                    row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"] = lastYear;
                }
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "ENERJI_MUSAADE_TALEP_DURUMU", WARNING_ONLY},
            { "ENERJI_MUSAADE_GERILIM_SEVIYESI", INFO_ONLY},
            { "ENERJI_MUSAADE_BAGLANTI_GUCU", INFO_ONLY},
            { "ENERJI_MUSAADE_ENERJILENDIRME_YILI", WARNING_ONLY},
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
        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportOGBaglanacagiTrafo();
        }
        public override void Impute()
        {
            ImputeOnay();

            ImputeEnerjilendirmeYılı();

            ImputeMustakilOlmayanTrafoID();
        }
        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_GERILIM_SEVIYESI"]);
            //combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

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


