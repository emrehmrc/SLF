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
        private int veerUniqID = 1; // Class-level field

        private void ImputeMustakilOlmayanTrafoID(List<int> missingCoordinatesRows, List<int> imputedTrafoRows, List<int> noNearestTrafoRows, List<int> newTrafoCreatedRows)
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];

            // Filter out rows with DBNull.Value in coordinates directly in the LINQ query
            var trafoList = trafoDataTable.AsEnumerable()
                .Where(row => row["TRAFO_X_KOORDINAT"] != DBNull.Value && row["TRAFO_Y_KOORDINAT"] != DBNull.Value)
                .Select(row => new
                {
                    TrafoKodu = row["TRAFO_KODU"].ToString(),
                    TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                    TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                })
                .ToList();

            // Filter out rows with DBNull.Value in coordinates or ID directly in the LINQ query
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

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];

                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                {
                    if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                        row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                    {
                        string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();

                        if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                        {
                            // Check if coordinates can be imputed from DTR Verileri
                            if (!IsNullLike(connectedTrafo))
                            {
                                var matchingTrafo = trafoDataTable.AsEnumerable()
                                    .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == connectedTrafo &&
                                                         !IsNullLike(t["TRAFO_X_KOORDINAT"]) &&
                                                         !IsNullLike(t["TRAFO_Y_KOORDINAT"]));
                                if (matchingTrafo != null)
                                {
                                    // Coordinates can be imputed later, so skip marking as #N/A
                                    continue;
                                }
                            }
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                            missingCoordinatesRows.Add(i);
                        }
                        else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                        {
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
                                row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                                imputedTrafoRows.Add(i);
                            }
                            else
                            {
                                row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                noNearestTrafoRows.Add(i);
                            }
                        }
                    }
                }

                if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                    row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "1")
                {
                    string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                    // Skip if the row already has a VEER-uniq-ID
                    if (!IsNullLike(connectedTrafo) && connectedTrafo.StartsWith("VEER-uniq-"))
                    {
                        continue;
                    }
                    if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo))
                    {
                        Console.WriteLine($"Row {i}: Creating new transformer ID for independent transformer. Current veerUniqID: {veerUniqID}");

                        string uniqueId = $"VEER-uniq-{veerUniqID++}";
                        double yeniTrafoKapasitesi = IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"])
                            ? 0.0
                            : Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                        int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                        row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = uniqueId;
                        yeniProjelendirilmisTrafoDataTable.Rows.Add(
                            uniqueId,
                            "",
                            "",
                            "",
                            roundedYeniTrafoKapasitesi,
                            roundedYeniTrafoKapasitesi,
                            lastYear
                        );
                        newTrafoCreatedRows.Add(i);
                    }
                }
            }
        }

        /*        private void ImputeMustakilOlmayanTrafoID(List<int> missingCoordinatesRows, List<int> imputedTrafoRows, List<int> noNearestTrafoRows, List<int> newTrafoCreatedRows)
                {
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];

                    // Filter out rows with DBNull.Value in coordinates directly in the LINQ query
                    var trafoList = trafoDataTable.AsEnumerable()
                        .Where(row => row["TRAFO_X_KOORDINAT"] != DBNull.Value && row["TRAFO_Y_KOORDINAT"] != DBNull.Value)
                        .Select(row => new
                        {
                            TrafoKodu = row["TRAFO_KODU"].ToString(),
                            TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                            TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                        })
                        .ToList();

                    // Filter out rows with DBNull.Value in coordinates or ID directly in the LINQ query
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

                    for (int i = 0; i < currentDataTable.Rows.Count; i++)
                    {
                        DataRow row = currentDataTable.Rows[i];

                        if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                        {
                            if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                                row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                            {
                                string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();

                                if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                                {
                                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                    missingCoordinatesRows.Add(i);
                                }
                                else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                                {
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
                                        row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                                        imputedTrafoRows.Add(i);
                                    }
                                    else
                                    {
                                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                        noNearestTrafoRows.Add(i);
                                    }
                                }
                            }
                        }

                        if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                            row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "1")
                        {
                            string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                            // Skip if the row already has a VEER-uniq-ID
                            if (!IsNullLike(connectedTrafo) && connectedTrafo.StartsWith("VEER-uniq-"))
                            {
                                continue;
                            }
                            if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo))
                            {
                                Console.WriteLine($"Row {i}: Creating new transformer ID for independent transformer. Current veerUniqID: {veerUniqID}");

                                string uniqueId = $"VEER-uniq-{veerUniqID++}";
                                // Add a null check for ENERJI_MUSAADE_BAGLANTI_GUCU to avoid InvalidCastException
                                double yeniTrafoKapasitesi = IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"])
                                    ? 0.0 // Default value if null; adjust as needed
                                    : Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                                int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                                row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = uniqueId;
                                yeniProjelendirilmisTrafoDataTable.Rows.Add(
                                    uniqueId,
                                    "",
                                    "",
                                    "",
                                    roundedYeniTrafoKapasitesi,
                                    roundedYeniTrafoKapasitesi,
                                    lastYear
                                );
                                newTrafoCreatedRows.Add(i);
                            }
                        }
                    }
                }*/

        private void ReportMustakilOlmayanTrafo()
        {
            List<int> missingCoordinatesRows = new List<int>();
            List<int> imputedTrafoRows = new List<int>();
            List<int> noNearestTrafoRows = new List<int>();
            List<int> newTrafoCreatedRows = new List<int>();

            // Perform imputation and collect row indices
            ImputeMustakilOlmayanTrafoID(missingCoordinatesRows, imputedTrafoRows, noNearestTrafoRows, newTrafoCreatedRows);

            // Log summaries for non-independent transformer scenarios
            if (missingCoordinatesRows.Count > 0)
            {
                string message = $"Eksik koordinatlar nedeniyle {missingCoordinatesRows.Count} satır için gerilim seviyesi '#N/A' olarak ayarlanacaktır. (Satır: {string.Join(", ", missingCoordinatesRows)})";
                warningDataTable.Rows.Add(
                    "ENERJI_MUSAADE_X_KOORDINAT, ENERJI_MUSAADE_Y_KOORDINAT",
                    "Koordinat Validasyonu",
                    $"Toplam: {missingCoordinatesRows.Count} satır",
                    message
                );
            }

            if (imputedTrafoRows.Count > 0)
            {
                string message = $"Eksik ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID'ler en yakın TRAFO_ID ile Toplam: {imputedTrafoRows.Count} satır güncellenecektir. (Satır: {string.Join(", ", imputedTrafoRows)})";
                warningDataTable.Rows.Add(
                    "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
                    "Koordinat Bazlı TrafoID Güncelleme",
                    $"Toplam: {imputedTrafoRows.Count} satır",
                    message
                );
            }

            if (noNearestTrafoRows.Count > 0)
            {
                string message = $"En yakın trafo bulunamadı, {noNearestTrafoRows.Count} satır için gerilim seviyesi '#N/A' olarak ayarlandı. Bu satırlar geçersiz olduğu için silinecektir. (Satır: {string.Join(", ", noNearestTrafoRows)})";
                infoDataTable.Rows.Add(
                    "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
                    "Koordinat Bazlı TrafoID Validasyonu",
                    $"Toplam: {noNearestTrafoRows.Count} satır",
                    message
                );
            }

            // Log summary for independent transformer scenario
            if (newTrafoCreatedRows.Count > 0)
            {
                string message = $"Unique ENERJİ_MUSAADE_BAGLANACAGI_TRAFO_ID (VEER-uniq-{veerUniqID - newTrafoCreatedRows.Count} to VEER-uniq-{veerUniqID - 1}) oluşturulacak ve Yeni Projelendirme DTR listesine eklenecektir. Toplam: {newTrafoCreatedRows.Count} satır. (Satır: {string.Join(", ", newTrafoCreatedRows)})";
                warningDataTable.Rows.Add(
                    "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
                    "Müstakil TrafoID Oluşturma",
                    $"Toplam: {newTrafoCreatedRows.Count} satır",
                    message
                );
            }
            else
            {
                Console.WriteLine("No rows required new transformer IDs for independent transformers.");
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

            // Step 5: Log to infoDataTable if duplicates were found and removed
            if (rowsToRemove.Count > 0)
            {
                float duplicatePercentage = (float)rowsToRemove.Count / initialRowCount;
                string message = $"Tekrarlayan ENERJI_MUSAADE_NO değerleri nedeniyle {rowsToRemove.Count} satır silindi. Toplam: {rowsToRemove.Count} satır, Percentage: {duplicatePercentage:P1}. (Satır: {string.Join(", ", rowsToRemove)})";
                infoDataTable.Rows.Add(
                    "ENERJI_MUSAADE_NO",
                    "Tekrarlayan Satır Validasyonu",
                    $"Toplam: {rowsToRemove.Count} satır",
                    message
                );
            }
        }

        private List<(DataRow row, int index)> ConvertAndValidateBaglantiGucu()
        {
            var removedRowsWithIndices = new List<(DataRow row, int index)>();

            // Step 1: Convert ENERJI_MUSAADE_BAGLANTI_GUCU from watts to kilowatts
            foreach (DataRow row in currentDataTable.Rows)
            {
                if (!IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                {
                    try
                    {
                        double baglantiGucuWatt = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                        row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt;
                    }
                    catch (FormatException)
                    {
                        // If the value can't be converted to double, mark the row for removal
                        int index = currentDataTable.Rows.IndexOf(row);
                        removedRowsWithIndices.Add((row, index));
                    }
                }
            }

            // Step 2: Validate connection power against transformer capacity for AG rows
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                if (row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]?.ToString() != "AG") continue;

                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                if (IsNullLike(trafoID)) continue;

                var matchingTrafo = trafoDataTable.AsEnumerable()
                    .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);
                if (matchingTrafo == null) continue;

                // Skip if ENERJI_MUSAADE_BAGLANTI_GUCU is NULL (will be handled by ReportNullCounts)
                if (IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"])) continue;

                try
                {
                    double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                    double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                    if (baglantiGucuKW > trafoKapasitesi * 0.6)
                    {
                        removedRowsWithIndices.Add((row, i));
                    }
                }
                catch (FormatException)
                {
                    // If conversion fails, mark the row for removal
                    removedRowsWithIndices.Add((row, i));
                }
            }

            return removedRowsWithIndices;
        }

        private void ReportRemovedRows(List<(DataRow row, int index)> removedRowsWithIndices)
        {
            if (removedRowsWithIndices.Count == 0) return;

            // Get trafo data ONCE (optimization)
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

            // Collect indices and categorize by reason
            List<int> rowIndices = new List<int>();
            int missingTrafoIdCount = 0;
            int invalidTrafoCount = 0;
            int exceededCapacityCount = 0;
            int dataCorruptionCount = 0;

            foreach (var (row, index) in removedRowsWithIndices)
            {
                rowIndices.Add(index);
                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();

                // Case 1: Missing or empty trafoID
                if (string.IsNullOrEmpty(trafoID))
                {
                    missingTrafoIdCount++;
                    continue;
                }

                // Case 2: Invalid transformer
                var matchingTrafo = trafoDataTable.AsEnumerable()
                    .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID);
                if (matchingTrafo == null)
                {
                    invalidTrafoCount++;
                    continue;
                }

                // Case 3: Check for exceeded capacity or data corruption
                if (IsNullLike(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]))
                {
                    dataCorruptionCount++; // Should be handled by ReportNullCounts, but log here for completeness
                    continue;
                }

                try
                {
                    double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                    double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);

                    if (baglantiGucuKW > trafoKapasitesi * 0.6)
                    {
                        exceededCapacityCount++;
                    }
                    else
                    {
                        dataCorruptionCount++; // Row was removed for another reason (e.g., invalid format in first loop)
                    }
                }
                catch (FormatException)
                {
                    dataCorruptionCount++;
                }
            }

            // Create a summary message
            List<string> reasons = new List<string>();
            if (missingTrafoIdCount > 0) reasons.Add($"{missingTrafoIdCount} satır eksik TRAFO_ID nedeniyle");
            if (invalidTrafoCount > 0) reasons.Add($"{invalidTrafoCount} satır geçersiz trafo nedeniyle");
            if (exceededCapacityCount > 0) reasons.Add($"{exceededCapacityCount} satır trafo kapasitesinin %60'ını aştığı için");
            if (dataCorruptionCount > 0) reasons.Add($"{dataCorruptionCount} satır veri bozulması nedeniyle");

            string summaryMessage = $"Silinen Satır Özeti: {string.Join(", ", reasons)}. (Satır: {string.Join(", ", rowIndices)})";

            // Log the summary
            infoDataTable.Rows.Add(
                "ENERJI_MUSAADE_BAGLANTI_GUCU,TRAFO_KAPASITESI",
                "Kapasite Aşım Validasyonu",
                $"Toplam: {removedRowsWithIndices.Count} satır",
                summaryMessage
            );
        }
        private void ReportOGBaglanacagiTrafo()
        {
            int totalRows = currentDataTable.Rows.Count;
            List<int> invalidRows = new List<int>();

            // Process all rows to collect invalid ones
            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "OG")
                {
                    if (IsNullLike(row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]))
                    {
                        invalidRows.Add(i);
                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                    }
                }
            }

            // Log a single summary if there are invalid rows
            if (invalidRows.Count > 0)
            {
                float invalidPercentage = (float)invalidRows.Count / totalRows;
                string message = $"OG seviyesinde enerji müsaadesi bağlanacağı trafo id boş olan veriler bulunmaktadır bunlar geçersiz olarak işaretlenecektir! Toplam Geçersiz Satır: {invalidRows.Count}, Percentage: {invalidPercentage:P1}. (Satır: {string.Join(", ", invalidRows)})";

                warningDataTable.Rows.Add(
                    "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",                          // Related Column Info
                    "OG-TrafoID Validasyonu", // ValidationType          
                    $"Toplam: {invalidRows.Count} satır", // Details
                    message                      // Action message with row indices
                );
            }
        }

/*        private void ImputeOnay()
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
        }*/


        private void ImputeEnerjilendirmeYılı()
        {
            List<int> imputedRows = new List<int>();
            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                if (IsNullLike(row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"]))
                {
                    row["ENERJI_MUSAADE_ENERJILENDIRME_YILI"] = lastYear;
                    imputedRows.Add(i);
                }
            }

            if (imputedRows.Count > 0)
            {
                string message = $"ENERJI_MUSAADE_ENERJILENDIRME_YILI NULL olan satırlar Planlama Horizonundaki ilk yıl ({lastYear}) olarak varsayılanmıştır. (Satır: {string.Join(", ", imputedRows)})";
                infoDataTable.Rows.Add(
                    "ENERJI_MUSAADE_ENERJILENDIRME_YILI",
                    "Enerjilendirme Yılı Imputation",
                    $"Toplam: {imputedRows.Count} satır",
                    message
                );
            }
        }
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            //{ "ENERJI_MUSAADE_TALEP_DURUMU", WARNING_ONLY},
            { "ENERJI_MUSAADE_GERILIM_SEVIYESI", INFO_ONLY},
            { "ENERJI_MUSAADE_BAGLANTI_GUCU", INFO_ONLY},
            { "ENERJI_MUSAADE_ENERJILENDIRME_YILI", WARNING_ONLY},
            { "ENERJI_MUSAADE_ABONE_GRUBU", WARNING_ONLY} 
        };

        private void ReportNullCounts()
        {
            int totalRows = currentDataTable.Rows.Count;
            // Step 1: Check for NULL coordinates (special case for X and Y together)
/*            List<int> missingCoordinateRows = new List<int>();
            List<int> imputedCoordinateRows = new List<int>();
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

            if (currentDataTable.Columns.Contains("ENERJI_MUSAADE_X_KOORDINAT") &&
                currentDataTable.Columns.Contains("ENERJI_MUSAADE_Y_KOORDINAT"))
            {
                for (int i = 0; i < totalRows; i++)
                {
                    DataRow row = currentDataTable.Rows[i];
                    bool isXNull = IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]);
                    bool isYNull = IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]);

                    if (isXNull || isYNull)
                    {
                        string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                        if (!IsNullLike(trafoID))
                        {
                            var matchingTrafo = trafoDataTable.AsEnumerable()
                                .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID &&
                                                     !IsNullLike(t["TRAFO_X_KOORDINAT"]) &&
                                                     !IsNullLike(t["TRAFO_Y_KOORDINAT"]));
                            if (matchingTrafo != null)
                            {
                                try
                                {
                                    row["ENERJI_MUSAADE_X_KOORDINAT"] = matchingTrafo["TRAFO_X_KOORDINAT"];
                                    row["ENERJI_MUSAADE_Y_KOORDINAT"] = matchingTrafo["TRAFO_Y_KOORDINAT"];
                                    imputedCoordinateRows.Add(i);
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Row {i}: Failed to impute coordinates in ReportNullCounts. Error: {ex.Message}");
                                }
                            }
                        }
                        missingCoordinateRows.Add(i);
                    }
                }

                // Log imputed coordinates
                if (imputedCoordinateRows.Count > 0)
                {
                    string message = $"ENERJI_MUSAADE_X_KOORDINAT veya ENERJI_MUSAADE_Y_KOORDINAT NULL olan satırlar, bağlı trafo koordinatları ile doldurulmuştur. Toplam: {imputedCoordinateRows.Count} satır. (Satır: {string.Join(", ", imputedCoordinateRows)})";
                    infoDataTable.Rows.Add(
                        "ENERJI_MUSAADE_X_KOORDINAT,ENERJI_MUSAADE_Y_KOORDINAT",
                        "Koordinat Imputation",
                        $"Toplam: {imputedCoordinateRows.Count} satır",
                        message
                    );
                }

                // Log error for remaining missing coordinates
                if (missingCoordinateRows.Count > 0)
                {
                    float missingPercentage = (float)missingCoordinateRows.Count / totalRows;
                    string errorMessage = $"Hata: ENERJI_MUSAADE_X_KOORDINAT veya ENERJI_MUSAADE_Y_KOORDINAT NULL olduğu için {missingCoordinateRows.Count} satır geçersiz. Bu satırlar silinecektir. Toplam: {missingCoordinateRows.Count} satır, Percentage: {missingPercentage:P1}. (Satır: {string.Join(", ", missingCoordinateRows)})";
                    errorDataTable.Rows.Add(
                        "ENERJI_MUSAADE_X_KOORDINAT,ENERJI_MUSAADE_Y_KOORDINAT",
                        "Koordinat Eksik Validasyonu",
                        $"Toplam: {missingCoordinateRows.Count} satır",
                        errorMessage
                    );
                }
            }*/

            foreach (DataColumn column in currentDataTable.Columns)
            {
                // Only process monitored columns.
                if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }

                List<int> nullRows = new List<int>();
                int nullCount = 0;

                // Process each row for the current column to collect NULL rows.
                for (int i = 0; i < totalRows; i++)
                {
                    DataRow row = currentDataTable.Rows[i];
                    if (IsNullLike(row[column]))
                    {
                        nullCount++;
                        nullRows.Add(i);
                    }
                }

                // Log a single summary entry for the column if there are NULLs.
                if (nullCount > 0)
                {
                    float nullPercentage = (float)nullCount / totalRows;

                    // Special case for ENERJI_MUSAADE_ABONE_GRUBU
                    if (column.ColumnName == "ENERJI_MUSAADE_ABONE_GRUBU")
                    {
                        string errorMessage = $"Silinecek: ENERJI_MUSAADE_ABONE_GRUBU NULL veya boş olduğu için {nullCount} satır geçersiz. Bu satırlar silinecektir. Toplam: {nullCount} satır, Percentage: {nullPercentage:P1}. (Satır: {string.Join(", ", nullRows)})";
                        infoDataTable.Rows.Add(
                            "ENERJI_MUSAADE_ABONE_GRUBU",
                            "Abone Grubu Validasyonu",
                            $"Toplam: {nullCount} satır",
                            errorMessage
                        );
                    }
/*                    // Special case for ENERJI_MUSAADE_TALEP_DURUMU
                    else if (column.ColumnName == "ENERJI_MUSAADE_TALEP_DURUMU")
                    {
                        string warningMessage = $"NULL değerler Onaylandı/Tamamlandı(0) olarak kabul edilerek devam edilecektir. (Satır: {string.Join(", ", nullRows)})";

                        warningDataTable.Rows.Add(
                            "Talep Durumu Validasyonu",
                            column.ColumnName,
                            $"{nullPercentage:P1}",
                            warningMessage
                        );
                    }*/
                    // Special case for ENERJI_MUSAADE_GERILIM_SEVIYESI
                    else if (column.ColumnName == "ENERJI_MUSAADE_GERILIM_SEVIYESI")
                    {
                        string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlar silinecektir. (Satır: {string.Join(", ", nullRows)})";

                        infoDataTable.Rows.Add(
                            "Gerilim Seviyesi Validasyonu",
                            column.ColumnName,
                            $"{nullPercentage:P1}",
                            warningMessage
                        );
                    }
                    // Special case for ENERJI_MUSAADE_ENERJILENDIRME_YILI
                    else if (column.ColumnName == "ENERJI_MUSAADE_ENERJILENDIRME_YILI")
                    {
                        string infoMessage = $"Enerji Musaadeleri Enerjilendirme Yılı doldurulmalıdır. Aksi takdirde Planlama Horizonundaki ilk yıl olarak varsayılanacaktır. Toplam: {nullCount} satır, Percentage: {nullPercentage:P1}. (Satır: {string.Join(", ", nullRows)})";
                        warningDataTable.Rows.Add(
                            "Enerjilendirme Yılı Validasyonu",
                            column.ColumnName,
                            $"Toplam: {nullCount} satır",
                            infoMessage
                        );

                        // Still log the generic message for removal
                        string genericMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlar silinecektir. (Satır: {string.Join(", ", nullRows)})";
                        infoDataTable.Rows.Add(
                            "NULL Değer Validasyonu",
                            column.ColumnName,
                            $"{nullPercentage:P1}",
                            genericMessage
                        );
                    }
                    else
                    {
                        // Default behavior for other columns
                        string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlara silinecektir. (Satır: {string.Join(", ", nullRows)})";

                        infoDataTable.Rows.Add(
                            "NULL Değer Validasyonu",
                            column.ColumnName,
                            $"{nullPercentage:P1}",
                            warningMessage
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
                    int totalRows = currentDataTable.Rows.Count;
                    // Step 1: Check for NULL coordinates (special case for X and Y together)
        *//*            List<int> missingCoordinateRows = new List<int>();
                    List<int> imputedCoordinateRows = new List<int>();
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];

                    if (currentDataTable.Columns.Contains("ENERJI_MUSAADE_X_KOORDINAT") &&
                        currentDataTable.Columns.Contains("ENERJI_MUSAADE_Y_KOORDINAT"))
                    {
                        for (int i = 0; i < totalRows; i++)
                        {
                            DataRow row = currentDataTable.Rows[i];
                            bool isXNull = IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]);
                            bool isYNull = IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]);

                            if (isXNull || isYNull)
                            {
                                string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                                if (!IsNullLike(trafoID))
                                {
                                    var matchingTrafo = trafoDataTable.AsEnumerable()
                                        .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID &&
                                                             !IsNullLike(t["TRAFO_X_KOORDINAT"]) &&
                                                             !IsNullLike(t["TRAFO_Y_KOORDINAT"]));
                                    if (matchingTrafo != null)
                                    {
                                        try
                                        {
                                            row["ENERJI_MUSAADE_X_KOORDINAT"] = matchingTrafo["TRAFO_X_KOORDINAT"];
                                            row["ENERJI_MUSAADE_Y_KOORDINAT"] = matchingTrafo["TRAFO_Y_KOORDINAT"];
                                            imputedCoordinateRows.Add(i);
                                            continue;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Row {i}: Failed to impute coordinates in ReportNullCounts. Error: {ex.Message}");
                                        }
                                    }
                                }
                                missingCoordinateRows.Add(i);
                            }
                        }

                        // Log imputed coordinates
                        if (imputedCoordinateRows.Count > 0)
                        {
                            string message = $"ENERJI_MUSAADE_X_KOORDINAT veya ENERJI_MUSAADE_Y_KOORDINAT NULL olan satırlar, bağlı trafo koordinatları ile doldurulmuştur. Toplam: {imputedCoordinateRows.Count} satır. (Satır: {string.Join(", ", imputedCoordinateRows)})";
                            infoDataTable.Rows.Add(
                                "ENERJI_MUSAADE_X_KOORDINAT, ENERJI_MUSAADE_Y_KOORDINAT",
                                "Koordinat Imputation",
                                $"Toplam: {imputedCoordinateRows.Count} satır",
                                message
                            );
                        }

                        // Log error for remaining missing coordinates
                        if (missingCoordinateRows.Count > 0)
                        {
                            float missingPercentage = (float)missingCoordinateRows.Count / totalRows;
                            string errorMessage = $"Hata: ENERJI_MUSAADE_X_KOORDINAT veya ENERJI_MUSAADE_Y_KOORDINAT NULL olduğu için {missingCoordinateRows.Count} satır geçersiz. Bu satırlar silinecektir. Toplam: {missingCoordinateRows.Count} satır, Percentage: {missingPercentage:P1}. (Satır: {string.Join(", ", missingCoordinateRows)})";
                            errorDataTable.Rows.Add(
                                "ENERJI_MUSAADE_X_KOORDINAT,ENERJI_MUSAADE_Y_KOORDINAT",
                                "Koordinat Eksik Validasyonu",
                                $"Toplam: {missingCoordinateRows.Count} satır",
                                errorMessage
                            );
                        }
                    }*//*

                    foreach (DataColumn column in currentDataTable.Columns)
                    {
                        // Only process monitored columns.
                        if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
                        {
                            continue;
                        }

                        List<int> nullRows = new List<int>();
                        int nullCount = 0;

                        // Process each row for the current column to collect NULL rows.
                        for (int i = 0; i < totalRows; i++)
                        {
                            DataRow row = currentDataTable.Rows[i];
                            if (IsNullLike(row[column]))
                            {
                                nullCount++;
                                nullRows.Add(i);
                            }
                        }

                        // Log a single summary entry for the column if there are NULLs.
                        if (nullCount > 0)
                        {
                            float nullPercentage = (float)nullCount / totalRows;

                            // Special case for ENERJI_MUSAADE_TALEP_DURUMU
                            if (column.ColumnName == "ENERJI_MUSAADE_TALEP_DURUMU")
                            {
                                string warningMessage = $"NULL değerler Onaylandı/Tamamlandı(0) olarak kabul edilerek devam edilecektir. (Satır: {string.Join(", ", nullRows)})";

                                warningDataTable.Rows.Add(
                                    "Talep Durumu Validasyonu",
                                    column.ColumnName,
                                    $"{nullPercentage:P1}",
                                    warningMessage
                                );
                            }
                            // Special case for ENERJI_MUSAADE_GERILIM_SEVIYESI
                            else if (column.ColumnName == "ENERJI_MUSAADE_GERILIM_SEVIYESI")
                            {
                                string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlar silinecektir. (Satır: {string.Join(", ", nullRows)})";

                                infoDataTable.Rows.Add(
                                    "Gerilim Seviyesi Validasyonu",
                                    column.ColumnName,
                                    $"{nullPercentage:P1}",
                                    warningMessage
                                );
                            }
                            // Special case for ENERJI_MUSAADE_ENERJILENDIRME_YILI
                            else if (column.ColumnName == "ENERJI_MUSAADE_ENERJILENDIRME_YILI")
                            {
                                string infoMessage = $"Enerji Musaadeleri Enerjilendirme Yılı doldurulmalıdır. Aksi takdirde Planlama Horizonundaki ilk yıl olarak varsayılanacaktır. Toplam: {nullCount} satır, Percentage: {nullPercentage:P1}. (Satır: {string.Join(", ", nullRows)})";
                                warningDataTable.Rows.Add(
                                    "Enerjilendirme Yılı Validasyonu",
                                    column.ColumnName,
                                    $"Toplam: {nullCount} satır",
                                    infoMessage
                                );

                                // Still log the generic message for removal
                                string genericMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlar silinecektir. (Satır: {string.Join(", ", nullRows)})";
                                infoDataTable.Rows.Add(
                                    "NULL Değer Validasyonu",
                                    column.ColumnName,
                                    $"{nullPercentage:P1}",
                                    genericMessage
                                );
                            }
                            else
                            {
                                // Default behavior for other columns
                                string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlara silinecektir. (Satır: {string.Join(", ", nullRows)})";

                                infoDataTable.Rows.Add(
                                    "NULL Değer Validasyonu",
                                    column.ColumnName,
                                    $"{nullPercentage:P1}",
                                    warningMessage
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
                }*/


        public override void Validate()
        {
            base.Validate();
            RemoveDuplicateRows();

            // Single source of truth
            List<(DataRow row, int index)> capacityViolations = ConvertAndValidateBaglantiGucu();
            ReportRemovedRows(capacityViolations);

            // Actually remove rows
            foreach (var (row, _) in capacityViolations)
            {
                currentDataTable.Rows.Remove(row);
            }

            // Run ReportNullCounts first to catch original NULL values
            ReportNullCounts();
            // Then run ReportOGBaglanacagiTrafo to mark OG rows with NULL TRAFO_ID
            ReportMustakilOlmayanTrafo(); // Call the new reporting method
            ReportOGBaglanacagiTrafo();
            // Run ImputeMustakilOlmayanTrafoID
            //ImputeMustakilOlmayanTrafoID();

            // Debug: Output warningDataTable contents
            Console.WriteLine("Contents of warningDataTable:");
            foreach (DataRow row in warningDataTable.Rows)
            {
                Console.WriteLine($"Column: {row[0]}, ValidationType: {row[1]}, Details: {row[2]}, Message: {row[3]}");
            }
        }

        public override void Impute()
        {
           // ImputeOnay();

            ImputeEnerjilendirmeYılı();
            ImputeCoordinates();
            //ImputeMustakilOlmayanTrafoID();
        }

        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_GERILIM_SEVIYESI"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_ABONE_GRUBU"]);

            RemoveCombinedRows(combinedRowsToRemoveList);
        }
        /*        public override void Remove()
                {
                    List<int> combinedRowsToRemoveList = new List<int>();

                    // Add row indices from different columns to the combined list
                    combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_GERILIM_SEVIYESI"]);
                    // combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]);
                    combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANTI_GUCU"]);

                    RemoveCombinedRows(combinedRowsToRemoveList);
                }*/
        private void ImputeCoordinates()
        {
            DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
            List<int> imputedRows = new List<int>();

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];
                bool isXNull = IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]);
                bool isYNull = IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]);

                // Only impute if at least one coordinate is NULL
                if (isXNull || isYNull)
                {
                    string trafoID = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]?.ToString();
                    if (!IsNullLike(trafoID))
                    {
                        var matchingTrafo = trafoDataTable.AsEnumerable()
                            .FirstOrDefault(t => t["TRAFO_KODU"].ToString() == trafoID &&
                                                 !IsNullLike(t["TRAFO_X_KOORDINAT"]) &&
                                                 !IsNullLike(t["TRAFO_Y_KOORDINAT"]));
                        if (matchingTrafo != null)
                        {
                            try
                            {
                                // Skip if coordinates are already non-NULL (likely imputed in ReportNullCounts)
                                if (!IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) &&
                                    !IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                                {
                                    continue;
                                }
                                row["ENERJI_MUSAADE_X_KOORDINAT"] = matchingTrafo["TRAFO_X_KOORDINAT"];
                                row["ENERJI_MUSAADE_Y_KOORDINAT"] = matchingTrafo["TRAFO_Y_KOORDINAT"];
                                imputedRows.Add(i);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Row {i}: Failed to impute coordinates. Error: {ex.Message}");
                            }
                        }
                    }
                }
            }

            if (imputedRows.Count > 0)
            {
                string message = $"ENERJI_MUSAADE_X_KOORDINAT veya ENERJI_MUSAADE_Y_KOORDINAT NULL olan satırlar, bağlı trafo koordinatları ile doldurulmuştur. Toplam: {imputedRows.Count} satır. (Satır: {string.Join(", ", imputedRows)})";
                infoDataTable.Rows.Add(
                    "ENERJI_MUSAADE_X_KOORDINAT, ENERJI_MUSAADE_Y_KOORDINAT",
                    "Koordinat Imputation",
                    $"Toplam: {imputedRows.Count} satır",
                    message
                );
            }
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

