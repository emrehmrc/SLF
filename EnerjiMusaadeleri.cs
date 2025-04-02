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
        /*        private void ImputeMustakilOlmayanTrafoID()
                {
                   // int veerUniqID = 1;
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];

                    // Load existing transformer data into a list for distance calculations
                    var trafoList = trafoDataTable.AsEnumerable()
                        .Select(row => new
                        {
                            TrafoKodu = row["TRAFO_KODU"].ToString(),
                            TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                            TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                        })
                        .ToList();

                    // Load newly projected transformer data into a list
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

                    // Lists to track row indices for each scenario (non-independent transformers)
                    List<int> missingCoordinatesRows = new List<int>();  // Rows with missing coordinates
                    List<int> imputedTrafoRows = new List<int>();        // Rows where TRAFO_ID was imputed
                    List<int> noNearestTrafoRows = new List<int>();      // Rows where no nearest transformer was found

                    // Lists to track row indices for independent transformer scenario
                    List<int> newTrafoCreatedRows = new List<int>();     // Rows where a new transformer was created

                    // Process all rows
                    for (int i = 0; i < currentDataTable.Rows.Count; i++)
                    {
                        DataRow row = currentDataTable.Rows[i];

                        // For non-independent transformer rows (flag 0) at AG level
                        if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                        {
                            if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                                row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                            {
                                string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();

                                // Case 1: Missing coordinates
                                if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                                {
                                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                    missingCoordinatesRows.Add(i);
                                }
                                // Case 2: Missing or invalid TRAFO_ID
                                else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                                {
                                    // Find the nearest transformer
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
                                        // Case 2a: Successfully imputed TRAFO_ID
                                        row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                                        imputedTrafoRows.Add(i);
                                    }
                                    else
                                    {
                                        // Case 2b: No nearest transformer found
                                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                        noNearestTrafoRows.Add(i);
                                    }
                                }
                            }
                        }

                        // For independent transformer rows (flag 1) - moved outside the AG condition
                        if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                            row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "1")
                        {
                            string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();
                            if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo))
                            {
                                // Debug log to confirm this block is executed
                                Console.WriteLine($"Row {i}: Creating new transformer ID for independent transformer. Current veerUniqID: {veerUniqID}");

                                // Create a new transformer
                                string uniqueId = $"VEER-uniq-{veerUniqID++}";
                                double yeniTrafoKapasitesi = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                                int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                                row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = uniqueId; // Update the row with the new transformer ID
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

                    // Log summaries for non-independent transformer scenarios
                    if (missingCoordinatesRows.Count > 0)
                    {
                        string message = $"Eksik koordinatlar nedeniyle {missingCoordinatesRows.Count} satır için gerilim seviyesi '#N/A' olarak ayarlandı. (Satır: {string.Join(", ", missingCoordinatesRows)})";
                        infoDataTable.Rows.Add(
                            "ENERJI_MUSAADE_X_KOORDINAT, ENERJI_MUSAADE_Y_KOORDINAT", // Related Column Info
                            "Koordinat Validasyonu",            // ReportType
                            $"Toplam: {missingCoordinatesRows.Count} satır", // Details
                            message                             // Action message with row indices
                        );
                    }

                    if (imputedTrafoRows.Count > 0)
                    {
                        string message = $"En yakın TRAFO_ID ile {imputedTrafoRows.Count} satır güncellendi. (Satır: {string.Join(", ", imputedTrafoRows)})";
                        warningDataTable.Rows.Add(
                            "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID", // Related Column Info
                            "TrafoID Güncelleme",            // ReportType
                            $"Toplam: {imputedTrafoRows.Count} satır", // Details
                            message                          // Action message with row indices
                        );
                    }

                    if (noNearestTrafoRows.Count > 0)
                    {
                        string message = $"En yakın trafo bulunamadı, {noNearestTrafoRows.Count} satır için gerilim seviyesi '#N/A' olarak ayarlandı. (Satır: {string.Join(", ", noNearestTrafoRows)})";
                        infoDataTable.Rows.Add(
                            "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID", // Related Column Info
                            "TrafoID Validasyonu",            // ReportType
                            $"Toplam: {noNearestTrafoRows.Count} satır", // Details
                            message                           // Action message with row indices
                        );
                    }

                    // Log summary for independent transformer scenario
                    if (newTrafoCreatedRows.Count > 0)
                    {
                        string message = $"Unique ENERJİ_MUSAADE_BAGLANACAGI_TRAFO_ID (VEER-uniq-{veerUniqID - newTrafoCreatedRows.Count} to VEER-uniq-{veerUniqID - 1}) oluşturuldu ve Yeni Projelendirme DTR listesine eklendi. Toplam: {newTrafoCreatedRows.Count} satır. (Satır: {string.Join(", ", newTrafoCreatedRows)})";

                        warningDataTable.Rows.Add(
                            "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID", // Related Column Info
                            "Müstakil TrafoID Oluşturma",          // ValidationType
                            $"Toplam: {newTrafoCreatedRows.Count} satır", // Details
                            message                                // Action message with row indices
                        );
                    }
                    else
                    {
                        // Debug log to indicate no independent transformers were processed
                        Console.WriteLine("No rows required new transformer IDs for independent transformers.");
                    }
                }*/
        private void ImputeMustakilOlmayanTrafoID(List<int> missingCoordinatesRows, List<int> imputedTrafoRows, List<int> noNearestTrafoRows, List<int> newTrafoCreatedRows)
        {
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

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                DataRow row = currentDataTable.Rows[i];

                if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                {
                    if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                        row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                    {
                        string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();

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
                    string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();
                    // Skip if the row already has a VEER-uniq- ID
                    if (!IsNullLike(connectedTrafo) && connectedTrafo.StartsWith("VEER-uniq-"))
                    {
                        continue;
                    }
                    if (IsNullLike(connectedTrafo) || !yeniTrafoList.Any(t => t.TrafoKodu == connectedTrafo))
                    {
                        Console.WriteLine($"Row {i}: Creating new transformer ID for independent transformer. Current veerUniqID: {veerUniqID}");

                        string uniqueId = $"VEER-uniq-{veerUniqID++}";
                        double yeniTrafoKapasitesi = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
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
        /*        private void ImputeMustakilOlmayanTrafoID()
                {
                    int veerUniqID = 1;
                    DataTable trafoDataTable = dataTablesByType["DTR Verileri"];
                    DataTable yeniProjelendirilmisTrafoDataTable = dataTablesByType["Yeni Projelendirilmiş DTR Verileri"];

                    // Load existing transformer data into a list for distance calculations
                    var trafoList = trafoDataTable.AsEnumerable()
                        .Select(row => new
                        {
                            TrafoKodu = row["TRAFO_KODU"].ToString(),
                            TrafoXKoordinat = Convert.ToDouble(row["TRAFO_X_KOORDINAT"]),
                            TrafoYKoordinat = Convert.ToDouble(row["TRAFO_Y_KOORDINAT"])
                        })
                        .ToList();

                    // Load newly projected transformer data into a list
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

                    // Lists to track row indices for each scenario (non-independent transformers)
                    List<int> missingCoordinatesRows = new List<int>();  // Rows with missing coordinates
                    List<int> imputedTrafoRows = new List<int>();        // Rows where TRAFO_ID was imputed
                    List<int> noNearestTrafoRows = new List<int>();      // Rows where no nearest transformer was found

                    // Lists to track row indices for independent transformer scenario
                    List<int> newTrafoCreatedRows = new List<int>();     // Rows where a new transformer was created

                    // Process all rows
                    for (int i = 0; i < currentDataTable.Rows.Count; i++)
                    {
                        DataRow row = currentDataTable.Rows[i];
                        if (!IsNullLike(row["ENERJI_MUSAADE_GERILIM_SEVIYESI"]) &&
                            row["ENERJI_MUSAADE_GERILIM_SEVIYESI"].ToString() == "AG")
                        {
                            // For non-independent transformer rows (flag 0)
                            if (!IsNullLike(row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"]) &&
                                row["ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL"].ToString() == "0")
                            {
                                string connectedTrafo = row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"].ToString();

                                // Case 1: Missing coordinates
                                if (IsNullLike(row["ENERJI_MUSAADE_X_KOORDINAT"]) || IsNullLike(row["ENERJI_MUSAADE_Y_KOORDINAT"]))
                                {
                                    row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                    missingCoordinatesRows.Add(i);
                                }
                                // Case 2: Missing or invalid TRAFO_ID
                                else if (IsNullLike(connectedTrafo) || !trafoList.Any(t => t.TrafoKodu == connectedTrafo))
                                {
                                    // Find the nearest transformer
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
                                        // Case 2a: Successfully imputed TRAFO_ID
                                        row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = enYakinTrafoKodu;
                                        imputedTrafoRows.Add(i);
                                    }
                                    else
                                    {
                                        // Case 2b: No nearest transformer found
                                        row["ENERJI_MUSAADE_GERILIM_SEVIYESI"] = "#N/A";
                                        noNearestTrafoRows.Add(i);
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
                                    // Create a new transformer
                                    string uniqueId = $"VEER-uniq-{veerUniqID++}";
                                    double yeniTrafoKapasitesi = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]) * 10;
                                    int roundedYeniTrafoKapasitesi = RoundUpTrafoKapasitesi(yeniTrafoKapasitesi);
                                    row["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"] = uniqueId; // Update the row with the new transformer ID
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

                    // Log summaries for non-independent transformer scenarios
                    if (missingCoordinatesRows.Count > 0)
                    {
                        string message = $"Missing coordinate(s) for {missingCoordinatesRows.Count} rows. Voltage level set to '#N/A'. (Satır: {string.Join(", ", missingCoordinatesRows)})";
                        infoDataTable.Rows.Add(
                            "",                          // No specific enerjiMusaadeNo since this is a summary
                            "Impute TrafoID",            // ReportType
                            $"Total: {missingCoordinatesRows.Count} rows", // Details
                            message                      // Action message with row indices
                        );
                    }

                    if (imputedTrafoRows.Count > 0)
                    {
                        string message = $"Updated to nearest TRAFO_ID for {imputedTrafoRows.Count} rows. (Satır: {string.Join(", ", imputedTrafoRows)})";
                        infoDataTable.Rows.Add(
                            "",                          // No specific enerjiMusaadeNo since this is a summary
                            "Impute TrafoID",            // ReportType
                            $"Total: {imputedTrafoRows.Count} rows", // Details
                            message                      // Action message with row indices
                        );
                    }

                    if (noNearestTrafoRows.Count > 0)
                    {
                        string message = $"No nearest transformer found for {noNearestTrafoRows.Count} rows. Voltage level set to '#N/A'. (Satır: {string.Join(", ", noNearestTrafoRows)})";
                        infoDataTable.Rows.Add(
                            "",                          // No specific enerjiMusaadeNo since this is a summary
                            "Impute TrafoID",            // ReportType
                            $"Total: {noNearestTrafoRows.Count} rows", // Details
                            message                      // Action message with row indices
                        );
                    }

                    // Log summary for independent transformer scenario
                    if (newTrafoCreatedRows.Count > 0)
                    {
                        string message = $"Unique ENERJİ_MUSAADE_BAGLANACAGI_TRAFO_ID oluşturuldu ve Yeni Projelendirme DTR listesine eklendi. Toplam: {newTrafoCreatedRows.Count} satır. (Satır: {string.Join(", ", newTrafoCreatedRows)})";

                        warningDataTable.Rows.Add(
                            "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID", // Related Column Info
                            "Müstakil TrafoID Oluşturma",          // ValidationType
                            $"Toplam: {newTrafoCreatedRows.Count} satır", // Details
                            message                                // Action message with row indices
                        );
                    }
                }*/
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
        /*        private void RemoveDuplicateRows()
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
                }*/
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
                        row["ENERJI_MUSAADE_BAGLANTI_GUCU"] = baglantiGucuWatt / 1000;
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
        /*        private List<(DataRow row, int index)> ConvertAndValidateBaglantiGucu()
                {
                    var removedRowsWithIndices = new List<(DataRow row, int index)>();

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
                    for (int i = 0; i < currentDataTable.Rows.Count; i++)
                    {
                        DataRow row = currentDataTable.Rows[i];
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
                            removedRowsWithIndices.Add((row, i));
                        }
                    }
                    return removedRowsWithIndices;
                }*/

        // Modified ConvertAndValidateBaglantiGucu
        /*        private List<DataRow> ConvertAndValidateBaglantiGucu()
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
        */
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
        /*        private void ReportRemovedRows(List<(DataRow row, int index)> removedRowsWithIndices)
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

                        // Case 3: Exceeded capacity or data corruption
                        try
                        {
                            double baglantiGucuKW = Convert.ToDouble(row["ENERJI_MUSAADE_BAGLANTI_GUCU"]);
                            double trafoKapasitesi = Convert.ToDouble(matchingTrafo["TRAFO_KAPASITESI"]);
                            exceededCapacityCount++;
                        }
                        catch (FormatException)
                        {
                            dataCorruptionCount++;
                        }
                    }

                    // Create a summary message
                    List<string> reasons = new List<string>();
                    if (missingTrafoIdCount > 0) reasons.Add($"{missingTrafoIdCount} rows with missing TRAFO_ID");
                    if (invalidTrafoCount > 0) reasons.Add($"{invalidTrafoCount} rows with invalid transformer");
                    if (exceededCapacityCount > 0) reasons.Add($"{exceededCapacityCount} rows exceeded capacity");
                    if (dataCorruptionCount > 0) reasons.Add($"{dataCorruptionCount} rows with data corruption");

                    string summaryMessage = $"Removed Rows Summary: {string.Join(", ", reasons)}. (Satır: {string.Join(", ", rowIndices)})";

                    // Log the summary
                    infoDataTable.Rows.Add(
                        "ENERJI_MUSAADE_BAGLANTI_GUCU,TRAFO_KAPASITESI",                          // No specific enerjiMusaadeNo since this is a summary
                        "Kapasite Aşım Validasyonu",       // ReportType
                        $"Toplam: {removedRowsWithIndices.Count} satır", // Details
                        summaryMessage               // Action message with row indices and reasons
                    );
                }*/
        /*        private void ReportRemovedRows(List<DataRow> removedRows)
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
        }*/
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
        /*        private void ReportOGBaglanacagiTrafo()
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
                }*/

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
            { "ENERJI_MUSAADE_TALEP_DURUMU", WARNING_ONLY},
            { "ENERJI_MUSAADE_GERILIM_SEVIYESI", INFO_ONLY},
            { "ENERJI_MUSAADE_BAGLANTI_GUCU", INFO_ONLY},
            { "ENERJI_MUSAADE_ENERJILENDIRME_YILI", WARNING_ONLY},
        };
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
        }
        /*        private void ReportNullCounts()
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
                                    "Talep Durumu Validasyonu",                          // No specific enerjiMusaadeNo since this is a summary
                                    column.ColumnName,           // ReportType
                                    $"{nullPercentage:P1}",      // Details (percentage of NULLs)
                                    warningMessage               // Action message with row indices
                                );
                            }
                            // Special case for ENERJI_MUSAADE_TALEP_DURUMU
                            if (column.ColumnName == "ENERJI_MUSAADE_GERILIM_SEVIYESI")
                            {
                                string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlar silinecektir. (Satır: {string.Join(", ", nullRows)})";

                                infoDataTable.Rows.Add(
                                    "Gerilim Seviyesi Validasyonu",                          // No specific enerjiMusaadeNo since this is a summary
                                    column.ColumnName,           // ReportType
                                    $"{nullPercentage:P1}",      // Details (percentage of NULLs)
                                    warningMessage               // Action message with row indices
                                );
                            }

                            else
                            {
                                // Default behavior for other columns
                                string warningMessage = $"Silinecekler Mesajı: {column.ColumnName} için NULL veya geçersiz olan satırlara silinecektir. (Satır: {string.Join(", ", nullRows)})";

                                infoDataTable.Rows.Add(
                                    "NULL Değer Validasyonu",                          // No specific enerjiMusaadeNo since this is a summary
                                    column.ColumnName,           // ReportType
                                    $"{nullPercentage:P1}",      // Details (percentage of NULLs)
                                    warningMessage               // Action message with row indices
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
        */

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
        /*        public override void Validate()
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
                    ReportNullCounts();
                    ReportOGBaglanacagiTrafo();  
                }*/
        /*        public override void Validate()
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
                }*/
        public override void Impute()
        {
            ImputeOnay();

            ImputeEnerjilendirmeYılı();

            //ImputeMustakilOlmayanTrafoID();
        }
        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_GERILIM_SEVIYESI"]);
           // combinedRowsToRemoveList.AddRange(columnNullRowsMap["ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID"]);
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


