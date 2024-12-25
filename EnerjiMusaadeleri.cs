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
        private void ImputeMustakilOlmayanTrafoID()
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


        /*        private void ConvertAndValidateBaglantiGucu()
                {
                    // Step 1: Convert ENERJI_MUSAADE_BAGLANTI_GUCU from Watt to Kilowatt
                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                        {
                            double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                            row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000; // Convert to kW
                        }
                    }

                    // Step 2: Cross-check with "DTR Verileri" TRAFO_KAPASITESI
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    List<int> rowsToRemove = new List<int>();

                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        // Co-condition: Check if ENERJI_MUSAADE_GERILIM_SEVIYESI is "AG"
                        if (IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) || row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() != "AG")
                            continue;

                        // Get the transformer ID
                        string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();

                        // Skip if transformer ID is null or invalid
                        if (IsNullLike(trafoID))
                            continue;

                        // Find the corresponding transformer in "DTR Verileri"
                        var matchingTrafo = trafoDataTable.AsEnumerable()
                                                          .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);

                        if (matchingTrafo != null)
                        {
                            // Get TRAFO_KAPASITESI and ENERJI_MUSAADE_BAGLANTI_GUCU
                            double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                            double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                            // Check if ENERJI_MUSAADE_BAGLANTI_GUCU exceeds 60% of TRAFO_KAPASITESI
                            if (baglantiGucuKW > trafoKapasitesi * 0.6)
                            {
                                rowsToRemove.Add(currentDataTable.Rows.IndexOf(row)); // Mark row for removal
                            }
                        }
                    }

                    // Step 3: Remove invalid rows
                    RemoveCombinedRows(rowsToRemove);
                }
        */

        /*        private void ConvertAndValidateBaglantiGucu()
                {
                    // Step 1: Convert ENERJI_MUSAADE_BAGLANTI_GUCU from Watt to Kilowatt
                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                        {
                            double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                            row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000; // Convert to kW
                        }
                    }

                    // Step 2: Cross-check with "DTR Verileri" TRAFO_KAPASITESI
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    List<int> rowsToRemove = new List<int>();
                    List<string> removedRowsDetails = new List<string>();

                    foreach (DataRow row in currentDataTable.Rows)
                    {
                        // Co-condition: Check if ENERJI_MUSAADE_GERILIM_SEVIYESI is "AG"
                        if (IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) || row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() != "AG")
                            continue;

                        // Check if ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID exists and is valid
                        string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                        string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();

                        if (IsNullLike(trafoID))
                        {
                            removedRowsDetails.Add($"Row Skipped -> ENERJI_MUSAADE_NO: {enerjiMusaadeNo}, ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID is missing");
                            continue;
                        }

                        // Find the corresponding transformer in "DTR Verileri"
                        var matchingTrafo = trafoDataTable.AsEnumerable()
                                                          .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);

                        if (matchingTrafo != null)
                        {
                            // Get TRAFO_KAPASITESI and ENERJI_MUSAADE_BAGLANTI_GUCU
                            double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                            double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                            // Check if ENERJI_MUSAADE_BAGLANTI_GUCU exceeds 60% of TRAFO_KAPASITESI
                            if (baglantiGucuKW > trafoKapasitesi * 0.6)
                            {
                                rowsToRemove.Add(currentDataTable.Rows.IndexOf(row));

                                // Calculate percentage and add details to removed rows list
                                double percentage = (baglantiGucuKW / trafoKapasitesi) * 100;
                                removedRowsDetails.Add($"Removed Row -> ENERJI_MUSAADE_NO: {enerjiMusaadeNo}, ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID: {trafoID}, BaglantiGucuKW: {baglantiGucuKW:F2}, Percentage of Capacity: {percentage:F2}%");
                            }
                        }
                        else
                        {
                           // removedRowsDetails.Add($"Row Skipped -> ENERJI_MUSAADE_NO: {enerjiMusaadeNo}, ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID: {trafoID} not found in DTR Verileri");
                        }
                    }

                    // Log removed rows details
                    Console.WriteLine($"Rows Removed or Skipped Due to Condition (baglantiGucuKW > 60% of TRAFO_KAPASITESI):");
                    foreach (var detail in removedRowsDetails)
                    {
                        Console.WriteLine(detail);
                    }

                    // Step 3: Remove invalid rows
                    RemoveCombinedRows(rowsToRemove);

                    // Log summary
                    Console.WriteLine($"Total Rows Removed: {rowsToRemove.Count}");
                    Console.WriteLine($"Remaining Rows After Removal: {currentDataTable.Rows.Count}");
                }*/

        private void ConvertAndValidateBaglantiGucu()
        {
            // Step 1: Convert ENERJI_MUSAADE_BAGLANTI_GUCU from Watt to Kilowatt
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                {
                    double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                    row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000; // Convert to kW
                }
            }

            // Step 2: Cross-check with "DTR Verileri" TRAFO_KAPASITESI
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            List<int> rowsToRemove = new List<int>();
            List<string> removedRowsDetails = new List<string>();

            foreach (DataRow row in currentDataTable.Rows)
            {
                // Co-condition: Check if ENERJI_MUSAADE_GERILIM_SEVIYESI is "AG"
                if (IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) || row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() != "AG")
                    continue;

                // Check if ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID exists and is valid
                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();

                if (IsNullLike(trafoID))
                    continue;

                // Find the corresponding transformer in "DTR Verileri"
                var matchingTrafo = trafoDataTable.AsEnumerable()
                                                  .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);

                if (matchingTrafo != null)
                {
                    // Get TRAFO_KAPASITESI and ENERJI_MUSAADE_BAGLANTI_GUCU
                    double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                    double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                    // Check if ENERJI_MUSAADE_BAGLANTI_GUCU exceeds 60% of TRAFO_KAPASITESI
                    if (baglantiGucuKW > trafoKapasitesi * 0.6)
                    {
                        rowsToRemove.Add(currentDataTable.Rows.IndexOf(row));

                        // Calculate percentage and add details to removed rows list
                        double percentage = (baglantiGucuKW / trafoKapasitesi) * 100;
                        string detail = $"Removed Row -> ENERJI_MUSAADE_NO: {enerjiMusaadeNo}, ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID: {trafoID}, BaglantiGucuKW: {baglantiGucuKW:F2}, Percentage of Capacity: {percentage:F2}%";

                        // Add to log and print to output window
                        removedRowsDetails.Add(detail);
                        Console.WriteLine(detail); // Print to Visual Studio Output Window
                    }
                }
            }

            // Step 3: Log removed rows details to the specified static path
            string logFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\veriler deneme\RemovedRowsLog.txt";
            File.WriteAllLines(logFilePath, removedRowsDetails);

            // Output log location to console
            Console.WriteLine($"Removed rows have been logged to: {logFilePath}");

            // Step 4: Remove invalid rows
            RemoveCombinedRows(rowsToRemove);

            // Log summary
            Console.WriteLine($"Total Rows Removed: {rowsToRemove.Count}");
            Console.WriteLine($"Remaining Rows After Removal: {currentDataTable.Rows.Count}");
        }
        private void ReportRemovedRows()
        {
            // Step 1: Convert ENERJI_MUSAADE_BAGLANTI_GUCU from Watt to Kilowatt
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                {
                    double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                    row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000; // Convert to kW
                }
            }

            // Step 2: Cross-check with "DTR Verileri" TRAFO_KAPASITESI
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            List<string> removedRowsDetails = new List<string>();

            foreach (DataRow row in currentDataTable.Rows)
            {
                // Co-condition: Check if ENERJI_MUSAADE_GERILIM_SEVIYESI is "AG"
                if (IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) || row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() != "AG")
                    continue;

                // Check if ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID exists and is valid
                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                string enerjiMusaadeNo = row["ENERJI_MUSAADE_NO"]?.ToString();

                if (IsNullLike(trafoID))
                    continue;

                // Find the corresponding transformer in "DTR Verileri"
                var matchingTrafo = trafoDataTable.AsEnumerable()
                                                  .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);

                if (matchingTrafo != null)
                {
                    // Get TRAFO_KAPASITESI and ENERJI_MUSAADE_BAGLANTI_GUCU
                    double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                    double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                    // Check if ENERJI_MUSAADE_BAGLANTI_GUCU exceeds 60% of TRAFO_KAPASITESI
                    if (baglantiGucuKW > trafoKapasitesi * 0.6)
                    {
                        // Calculate percentage and add details to removed rows list
                        double percentage = (baglantiGucuKW / trafoKapasitesi) * 100;
                        string detail = $"ENERJI_MUSAADE_NO: {enerjiMusaadeNo}, TRAFO_ID: {trafoID}, BaglantiGucuKW: {baglantiGucuKW:F2}, Percentage of Capacity: {percentage:F2}%";

                        // Add removed row details to infoDataTable
                        infoDataTable.Rows.Add(new object[]
                        {
                    enerjiMusaadeNo,
                    "Removed Rows Report",
                    detail,
                    "BaglantiGucuKW exceeds 60% of TRAFO_KAPASITESI"
                        });

                        // Add to log and print to console
                        removedRowsDetails.Add(detail);
                        Console.WriteLine($"Row removed: {detail}");
                    }
                }
            }
            Console.WriteLine($"Total Rows Removed Due to Percentage Validation: {removedRowsDetails.Count}");
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

            RemoveDuplicateRows();

            ReportRemovedRows();

            ReportNullCounts();

            ReportOGBaglanacagiTrafo();

            ConvertAndValidateBaglantiGucu();
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


