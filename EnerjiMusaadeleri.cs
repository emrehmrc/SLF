using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SLF
{
    public class EnerjiMusaadeleri : GirdiModülü

    {
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri", "Yeni Projelendirilmiş DTR Verileri" };
        /*        private void ImputeMustakilOlmayanTrafoID()
                {
                    int veerUniqID = 1;
                    // "DTR Verileri" tablosundan trafo bilgilerini al
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];
                    var trafoList = trafoDataTable.AsEnumerable()
                                                  .Select(row => new
                                                  {
                                                      TrafoKodu = row["TRAFO_KODU"].ToString(),
                                                      TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                                                      TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                                                  })
                                                  .ToList();
                    var yeniTrafoList = yeniProjelendirilmisTrafoDataTable.AsEnumerable()
                            .Where(row => row["PROJELENDIRILMIS_TRAFO_ID"] != DBNull.Value &&
                            row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"] != DBNull.Value &&
                            row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"] != DBNull.Value)

                          .Select(row => new
                          {
                              TrafoKodu = row["PROJELENDIRILMIS_TRAFO_ID"].ToString(),
                              TrafoXKoordinat = Convert.ToDouble(row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"]),
                              TrafoYKoordinat = Convert.ToDouble(row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"])
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
                                }

                                // "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID" değeri IsNullLike ise veya geçerli trafo kodları arasında değilse kontrol eder
                                else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
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
                            else if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) && row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "1")
                            {
                                string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();
                                // "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID" değeri IsNullLike ise veya geçerli trafo kodları arasında değilse kontrol eder
                                if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo)) // TODO
                                {
                                    // Yeni Trafo listesine satır ekle Unique ID ile VEER-uniq-0001
                                    string uniqueId = $"VEER-uniq-{veerUniqID++}";
                                    // Yeni Trafo listesine ayni satida kapasiteyi doldur (enerji musaadesindeki baglanti gucunun 10 katini al)
                                    double yeniTrafoKapasitesi = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                                    int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                                    // TRAFO_KAPASITE_LISTESI listesinde bi ustundekine yuvarla bu degerin
                                    yeniProjelendirilmisTrafoDataTable.Rows.Add(
                                        uniqueId,
                                        "",
                                        "",
                                        "",
                                        roundedYeniTrafoKapasitesi,
                                        roundedYeniTrafoKapasitesi,
                                        lastYear
                                    );

                                }
                            }
                        }
                    }
                }*/
        private void ImputeMustakilOlmayanTrafoID()
        {
            int veerUniqID = 1;
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];

            var trafoList = trafoDataTable.AsEnumerable()
                .Select(row => new
                {
                    TrafoKodu = row["TRAFO_KODU"].ToString(),
                    TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                    TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                })
                .ToList();

            var yeniTrafoList = yeniProjelendirilmisTrafoDataTable.AsEnumerable()
                .Where(row => row["PROJELENDIRILMIS_TRAFO_ID"] != DBNull.Value &&
                              row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"] != DBNull.Value &&
                              row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"] != DBNull.Value)
                .Select(row => new
                {
                    TrafoKodu = row["PROJELENDIRILMIS_TRAFO_ID"].ToString(),
                    TrafoXKoordinat = Convert.ToDouble(row["PROJELENDIRILMIS_TRAFO_X_KOORDINAT"]),
                    TrafoYKoordinat = Convert.ToDouble(row["PROJELENDIRILMIS_TRAFO_Y_KOORDINAT"])
                })
                .ToList();

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                {
                    // For non-independent transformer rows (flag 0)
                    if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                        row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                    {
                        string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();

                        if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                        {
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                            string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                            infoDataTable.Rows.Add(
                                enerjiMusaadeNo,
                                "Impute TrafoID",
                                "Missing coordinate(s).",
                                "Voltage level set to '#N/A'."
                            );
                        }
                        else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                        {
                            // Find the nearest transformer.
                            double enYakinMesafe = double.MaxValue;
                            string enYakinTrafoKodu = null;
                            double musadeXKoordinat = Convert.ToDouble(row["ENERJI_MUSAADE_X_KOORDINAT"]);
                            double musadeYKoordinat = Convert.ToDouble(row["ENERJI_MUSAADE_Y_KOORDINAT"]);

                            foreach (var trafo in trafoList)
                            {
                                double mesafe = Math.Sqrt(
                                    Math.Pow(trafo.TrafoXKoordinat - musadeXKoordinat, 2) +
                                    Math.Pow(trafo.TrafoYKoordinat - musadeYKoordinat, 2)
                                );
                                if (mesafe < enYakinMesafe)
                                {
                                    enYakinMesafe = mesafe;
                                    enYakinTrafoKodu = trafo.TrafoKodu;
                                }
                            }

                            if (enYakinTrafoKodu != null)
                            {
                                string oldTrafoID = connectedTrafo;
                                row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                                infoDataTable.Rows.Add(
                                    enerjiMusaadeNo,
                                    "Impute TrafoID",
                                    $"Old TRAFO_ID: {oldTrafoID ?? "null"}.",
                                    $"Updated to nearest TRAFO_ID: {enYakinTrafoKodu}."
                                );
                            }
                            else
                            {
                                row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                                infoDataTable.Rows.Add(
                                    enerjiMusaadeNo,
                                    "Impute TrafoID",
                                    "No nearest transformer found.",
                                    "Voltage level set to '#N/A'."
                                );
                            }
                        }
                    }
                    // For independent transformer rows (flag 1)
                    else if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                             row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "1")
                    {
                        string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();
                        if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo))
                        {
                            string uniqueId = $"VEER-uniq-{veerUniqID++}";
                            double yeniTrafoKapasitesi = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                            int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                            yeniProjelendirilmisTrafoDataTable.Rows.Add(
                                uniqueId,
                                "",
                                "",
                                "",
                                roundedYeniTrafoKapasitesi,
                                roundedYeniTrafoKapasitesi,
                                lastYear
                            );
                            string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                            infoDataTable.Rows.Add(
                                enerjiMusaadeNo,
                                "Impute TrafoID",
                                "Missing or invalid transformer for independent row.",
                                $"New transformer created with ID: {uniqueId}."
                            );
                        }
                    }
                }
            }
        }

        private void RemoveDuplicateRows()
        {
            // Step 1: Create a HashSet to track unique values of ENERJI_MUSAADE_NO
            HashSet<string> uniqueEnerjiMusaadeNos = new HashSet<string>();

            // Step 2: Prepare a list to track rows to remove
            List<int> rowsToRemove = new List<int>();
            int duplicateCount = 0;

            // Step 3: Iterate through each row in currentDataTable
            foreach (DataRow row in currentDataTable.Rows)
            {
                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();

                if (!string.IsNullOrEmpty(enerjiMusaadeNo))
                {
                    // If the ENERJI_MUSAADE_NO is already in the HashSet, mark the row for removal
                    if (!uniqueEnerjiMusaadeNos.Add(enerjiMusaadeNo))
                    {
                        rowsToRemove.Add(currentDataTable.Rows.IndexOf(row));
                        duplicateCount++;
                    }
                }
            }

            // Log the counts of duplicates
            Console.WriteLine($"Total Duplicate Rows Found: {duplicateCount}");

            // Step 4: Remove duplicate rows
            int initialRowCount = currentDataTable.Rows.Count;
            RemoveCombinedRows(rowsToRemove);
            int remainingRowCount = currentDataTable.Rows.Count;

            // Log the removal results
            Console.WriteLine($"Total Rows Removed: {rowsToRemove.Count}");
            Console.WriteLine($"Remaining Rows After Removal: {remainingRowCount}");
        }

        // Modified ConvertAndValidateBaglantiGucu
        private List<DataRow> ConvertAndValidateBaglantiGucu()
        {
            var removedRows = new List<DataRow>();

            // Convert to kW first
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                {
                    double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                    row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000;
                }
            }

            // Validate capacity and collect rows to remove
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]?.ToString() != "AG") continue;

                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                if (IsNullLike(trafoID)) continue;

                var matchingTrafo = trafoDataTable.AsEnumerable()
                    .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);
                if (matchingTrafo == null) continue;

                double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                if (baglantiGucuKW > trafoKapasitesi * 0.6)
                {
                    removedRows.Add(row);
                }
            }
            return removedRows;
        }

        // Modified ReportRemovedRows (now read-only)
/*        private void ReportRemovedRows(List<DataRow> removedRows)
        {
            foreach (DataRow row in removedRows)
            {

                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                // Step 2: Cross-check with "DTR Verileri" TRAFO_KAPASITESI
                DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                // Find the corresponding transformer in "DTR Verileri"
                var matchingTrafo = trafoDataTable.AsEnumerable()
                                                  .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);
                double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);

                infoDataTable.Rows.Add(new object[] {
            enerjiMusaadeNo,
            "Removed Rows Report",
            $"TRAFO_ID: {trafoID}, BaglantiGucuKW: {baglantiGucuKW:F2}, Percentage: {(baglantiGucuKW/trafoKapasitesi):P1}",
            "Exceeded capacity"
        });
            }
        }*/

        private void ReportRemovedRows(List<DataRow> removedRows)
{
    // Get trafo data ONCE (optimization)
    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
    
    foreach (DataRow row in removedRows)
    {
        string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
        string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
        
        // Fix 1: Handle null/empty trafoID
        if (string.IsNullOrEmpty(trafoID))
        {
            infoDataTable.Rows.Add(enerjiMusaadeNo, "Removed Rows Report", 
                                  "Missing TRAFO_ID", "Invalid transformer ID");
            continue;
        }

        // Fix 2: Safe trafo lookup
        var matchingTrafo = trafoDataTable.AsEnumerable()
            .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);
        
        if (matchingTrafo == null)
        {
            infoDataTable.Rows.Add(enerjiMusaadeNo, "Removed Rows Report", 
                                  $"TRAFO_ID {trafoID} not found", "Invalid transformer");
            continue;
        }

        // Fix 3: Safe value conversions
        try 
        {
            double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
            double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
            
            infoDataTable.Rows.Add(
                "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
                enerjiMusaadeNo,
              //  "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID - ENERJI_MUSAADE_BAGLANTI_GUCU - TRAFO_KAPASITESI",
               // "Removed Rows Report",
                $"TRAFO_ID: {trafoID}, BaglantiGucuKW: {baglantiGucuKW:F2}, Percentage: {(baglantiGucuKW/trafoKapasitesi):P1}",
                "Exceeded capacity"
            );
        }
        catch (FormatException ex)
        {
            infoDataTable.Rows.Add(
                enerjiMusaadeNo,
                "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
               // "Removed Rows Report", 
                $"Invalid numeric value: {ex.Message}", 
                "Data corruption"
            );
        }
    }
}
        private void ReportOGBaglanacagiTrafo()
        {
            int invalidCount = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                // Check if the row's voltage level is "OG"
                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "OG")
                {
                    if (IsNullLike(row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]))
                    {
                        string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                        invalidCount++;
                        infoDataTable.Rows.Add(
                            enerjiMusaadeNo,
                           // "OG Transformer Report",
                            "Missing transformer ID.",
                            "Voltage level set to '#N/A'."
                        );
                    }
                }
            }

            if (invalidCount > 0)
            {
                float invalidPercentage = (float)invalidCount / currentDataTable.Rows.Count;
                infoDataTable.Rows.Add(new object[] {
            "",
            "OG Transformer Report",
            $"{invalidCount} rows missing transformer ID. Percentage: {invalidPercentage:P1}",
            "Affected rows updated."
        });
            }
        }

        /*        private void ReportOGBaglanacagiTrafo()
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
            }*/

        /*        private void ImputeOnay()
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
                }*/
        private void ImputeOnay()
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (IsNullLike(row["ENERJI_MUSAADE_TALEP_DURUMU"]))
                {
                    string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                    row["ENERJI_MUSAADE_TALEP_DURUMU"] = 0;
                    infoDataTable.Rows.Add(
                        enerjiMusaadeNo,
                     //   "Impute Onay",
                        "Request status was null.",
                        "Defaulted to 0."
                    );
                }
            }
        }

        /*        private void ImputeEnerjilendirmeYılı()
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
                }*/
        private void ImputeEnerjilendirmeYılı()
        {
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (IsNullLike(row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"]))
                {
                    string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                    row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"] = lastYear;
                    infoDataTable.Rows.Add(
                        enerjiMusaadeNo,

                        "Impute Enerjilendirme Yılı",
                        "Energization year was null.",
                        $"Defaulted to {lastYear}."
                    );
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
        /*        private void ReportNullCounts()
                {
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
                            DataRow row = currentDataTable.Rows[i];
                            if (IsNullLike(row[column]))
                            {
                                nullCount++;
                                nullRows.Add(i);
                                // Save the row indices for later removal if needed.
                                columnNullRowsMap[column.ColumnName] = nullRows;

                                float nullPercentage = (float)nullCount / totalRows;
                                if (nullPercentage > 0)
                                {
                                    var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                                   // datatableLevel.Rows.Add(new object[] { column.ColumnName, "Null Value", $"{nullPercentage:P1}" });
                                }
                                // Log detail for each row with a null value.
                                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                                infoDataTable.Rows.Add(
                                    enerjiMusaadeNo,
                                     $"Column '{column.ColumnName}' {nullPercentage:P1}",
                                    "Null Value Report",
                                    $"Column '{column.ColumnName}' is null.",
                                    "This row will be flagged for removal or imputation."
                                );
                            }
                        }

                        // Save the row indices for later removal if needed.
        *//*                columnNullRowsMap[column.ColumnName] = nullRows;

                        float nullPercentage = (float)nullCount / totalRows;
                        if (nullPercentage > 0)
                        {
                            var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                            var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                            datatableLevel.Rows.Add(new object[] { column.ColumnName, "Null Value", $"{nullPercentage:P1}" });
                        }*//*
                    }
                }*/
        private void ReportNullCounts()
        {
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                // Only process monitored columns.
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }

                List<int> nullRows = new List<int>();
                int nullCount = 0;

                // Process each row for the current column.
                for (int i = 0; i < totalRows; i++)
                {
                    DataRow row = currentDataTable.Rows[i];
                    if (IsNullLike(row[column]))
                    {
                        nullCount++;
                        nullRows.Add(i);

                        // Calculate the current null percentage.
                        float nullPercentage = (float)nullCount / totalRows;

                        // Create the additional summary string that was previously added as a row.
                        string additionalSummary = $"{column.ColumnName}, Null Value, {nullPercentage:P1}";

                        // Log detail for each row with a null value.
                        string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();
                        infoDataTable.Rows.Add(
                            enerjiMusaadeNo,                      // Identifier
                            column.ColumnName,                         // ReportType
                            $"{nullPercentage:P1}",               // Details (or you could merge with the summary if desired)
                            "This row will be flagged for removal or imputation." // Action
                           // additionalSummary                     // Additional summary column
                        );
                    }
                }

                // Ensure an entry exists in columnNullRowsMap for later removal.
                if (columnNullRowsMap.ContainsKey(column.ColumnName))
                {
                    columnNullRowsMap[column.ColumnName] = nullRows;
                }
                else
                {
                    columnNullRowsMap.Add(column.ColumnName, nullRows);
                }
            }
        }

        /*        private void ReportNullCounts()
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
                }*/

        public override void Validate()
        {
            base.Validate();
            RemoveDuplicateRows();

            // Single source of truth
            List<DataRow> capacityViolations = ConvertAndValidateBaglantiGucu();
            ReportRemovedRows(capacityViolations);

            // Actually remove rows
            foreach (DataRow row in capacityViolations)
            {
                currentDataTable.Rows.Remove(row);
            }

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


