using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SLF
{
    public class EkonometrikYukTahminiModulu : GirdiModülü

    {
        private int startYear;

        //protected override List<string> Prerequisites => new List<string> { "DTR Verileri", "Abone Verileri" };
        //private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        //{
        //    { "MESKEN_FATURALANAN", ERROR_ONLY},
        //    { "SANAYI_FATURALANAN", ERROR_ONLY},
        //    { "TICARETHANE_FATURALANAN", ERROR_ONLY},
        //    { "AYDINLATMA_FATURALANAN", ERROR_ONLY},
        //    {"TARIMSAL_SULAMA_FATURALANAN",ERROR_ONLY },
        //    {"KKO",WARNING_ONLY },
        //    {"KKM",WARNING_ONLY },
        //    { "PUANT_YAZ", ERROR_ONLY},
        //    { "PUANT_KIŞ", ERROR_ONLY},
        //    {"TOPLAM_DAGITILAN", WARNING_ONLY};
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> negativeOrZeroLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {

            {"YIL", ERROR_ONLY},
            {"MESKEN_FATURALANAN", ERROR_ONLY},
            {"MESKEN_ABONE_SAYISI", ERROR_ONLY},
            {"SANAYI_ABONE_SAYISI", ERROR_ONLY},
            {"TICARETHANE_ABONE_SAYISI", ERROR_ONLY},
            {"TARIMSAL_SULAMA_ABONE_SAYISI", ERROR_ONLY},
            {"AYDINLATMA_ABONE_SAYISI", ERROR_ONLY},
            {"TOPLAM_ABONE_SAYISI", ERROR_ONLY},
            {"SANAYI_FATURALANAN", ERROR_ONLY},
            {"TICARETHANE_FATURALANAN", ERROR_ONLY},
            {"AYDINLATMA_FATURALANAN", ERROR_ONLY},
            {"TARIMSAL_SULAMA_FATURALANAN", ERROR_ONLY},
            {"MESKEN_DAGITILAN", ERROR_ONLY},
            {"SANAYI_DAGITILAN", ERROR_ONLY},
            {"TICARETHANE_DAGITILAN", ERROR_ONLY},
            {"TARIMSAL_SULAMA_DAGITILAN", ERROR_ONLY},
            {"AYDINLATMA_DAGITILAN", ERROR_ONLY},
            {"TOPLAM_DAGITILAN", ERROR_ONLY},
            {"KKO",ERROR_ONLY },
            {"KKM",ERROR_ONLY },
        };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> fiveYearsDataCheck = new Dictionary<string, (float warningThreshold, float errorThreshold)>
{
            {"YIL", ERROR_ONLY},
            {"GDP_GROWTH", ERROR_ONLY},
            {"ULKE_NUFUS", ERROR_ONLY},
            {"BOLGE_NUFUS", ERROR_ONLY},
            //{"KKO", ERROR_ONLY},
            //{"KKM", ERROR_ONLY},
            //{"MESKEN_DAGITILAN", ERROR_ONLY},
            //{"SANAYI_DAGITILAN", ERROR_ONLY},
            //{"TICARETHANE_DAGITILAN", ERROR_ONLY},
            //{"TARIMSAL_SULAMA_DAGITILAN", ERROR_ONLY},
            //{"AYDINLATMA_DAGITILAN", ERROR_ONLY},
            //{"TOPLAM_DAGITILAN", WARNING_ONLY},
            {"MESKEN_FATURALANAN", ERROR_ONLY},
            {"SANAYI_FATURALANAN", ERROR_ONLY},
            {"TICARETHANE_FATURALANAN", ERROR_ONLY},
            {"TARIMSAL_SULAMA_FATURALANAN", ERROR_ONLY},
            {"AYDINLATMA_FATURALANAN", ERROR_ONLY},
            {"TOPLAM_FATURALANAN", ERROR_ONLY},
            {"MESKEN_ABONE_SAYISI", ERROR_ONLY},
            {"SANAYI_ABONE_SAYISI", ERROR_ONLY},
            {"TICARETHANE_ABONE_SAYISI", ERROR_ONLY},
            {"TARIMSAL_SULAMA_ABONE_SAYISI", ERROR_ONLY},
            {"AYDINLATMA_ABONE_SAYISI", ERROR_ONLY},
            {"TOPLAM_ABONE_SAYISI", ERROR_ONLY},
            //{"BOLGE_YAZ_PUANT", ERROR_ONLY},
            //{"BOLGE_KIS_PUANT", ERROR_ONLY},
            {"GRP", ERROR_ONLY},
            {"GRP_TARIMSAL_URETIM", ERROR_ONLY},
            {"GRP_SANAYI_URETIM", ERROR_ONLY},
            {"GRP_HIZMET_URETIM", ERROR_ONLY},
            {"GRP_INSAAT_URETIM", ERROR_ONLY},
            {"GRP_TARIMSAL_URETIM_%", ERROR_ONLY},
            {"GRP_SANAYI_URETIM_%", ERROR_ONLY},
            {"GRP_HIZMET_URETIM_%", ERROR_ONLY},
            {"GRP_INSAAT_URETIM_%", ERROR_ONLY},
            {"GDP", ERROR_ONLY},
            {"GDP_TARIMSAL_URETIM", ERROR_ONLY},
            {"GDP_SANAYI_URETIM", ERROR_ONLY},
            {"GDP_HIZMET_URETIM", ERROR_ONLY},
            {"GDP_INSAAT_URETIM", ERROR_ONLY},
            {"GDP_TARIMSAL_URETIM_%", ERROR_ONLY},
            {"GDP_SANAYI_URETIM_%", ERROR_ONLY},
            {"GDP_HIZMET_URETIM_%", ERROR_ONLY},
            {"GDP_INSAAT_URETIM_%", ERROR_ONLY},
            {"CDD", ERROR_ONLY},
            {"HDD", ERROR_ONLY},
            {"ULKE_NUFUS_%", ERROR_ONLY},
            {"BOLGE_NUFUS_%", ERROR_ONLY},
            {"EA_Talep", ERROR_ONLY},
            {"DEK_Uretim", ERROR_ONLY},
            {"Other", ERROR_ONLY}
};
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dagıtılanCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {

            { "MESKEN_DAGITILAN", WARNING_ONLY},
            { "SANAYI_DAGITILAN", WARNING_ONLY},
            { "TICARETHANE_DAGITILAN", WARNING_ONLY},
            { "AYDINLATMA_DAGITILAN", WARNING_ONLY},
            {"TARIMSAL_SULAMA_DAGITILAN",WARNING_ONLY },
        };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> yearDetect = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "YIL", ERROR_ONLY},
        };
        public readonly Dictionary<string, bool> setConvertPercentage = new Dictionary<string, bool>
    {
        { "GDP_GROWTH", true },
        { "KKO", true },
        { "GRP_TARIMSAL_URETIM_%", true },
        { "GRP_SANAYI_URETIM_%", true },
        { "GRP_HIZMET_URETIM_%", true },
        { "GRP_INSAAT_URETIM_%", true },
        { "GDP_TARIMSAL_URETIM_%", true },
        { "GDP_SANAYI_URETIM_%", true },
        { "GDP_HIZMET_URETIM_%", true },
        { "GDP_INSAAT_URETIM_%", true },
        { "ULKE_NUFUS_%", true },
        { "BOLGE_NUFUS_%", true }
    };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> loadPercentageIncreaseDetect = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {

            {"MESKEN_FATURALANAN", WARNING_ONLY},
            {"SANAYI_FATURALANAN", WARNING_ONLY},
            {"TICARETHANE_FATURALANAN", WARNING_ONLY},
            {"KKO",WARNING_ONLY },
            {"KKM",WARNING_ONLY },
        };

        private bool isDagıtılanİmputed;
        public override void Preprocess()
        {
            isDagıtılanİmputed = false;

        }
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> kkokkmDagCheck = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {

            {"TOPLAM_DAGITILAN", ERROR_ONLY},
            {"KKO",WARNING_ONLY },
            {"KKM",WARNING_ONLY },
        };
        //    private Dictionary<string, bool> GetPercentageColumns()
        //    {
        //        // Buraya yüzdelik gösterilmesi gereken sütunlarınızı ekleyin
        //        return new Dictionary<string, bool>
        //{
        //    { "KKO", true },
        //    { "GDP_GROWTH", true },
        //    { "GRP_TARIMSAL_URETIM_%", true },
        //    { "GRP_SANAYI_URETIM_%", true },
        //    { "GRP_HIZMET_URETIM_%", true },
        //    { "GRP_INSAAT_URETIM_%", true },
        //    { "GDP_TARIMSAL_URETIM_%", true },
        //    { "GDP_SANAYI_URETIM_%", true },
        //    { "GDP_HIZMET_URETIM_%", true },
        //    { "GDP_INSAAT_URETIM_%", true },
        //    { "ULKE_NUFUS_%", true },
        //    { "BOLGE_NUFUS_%n%", true },
        //    // Diğer sütunlarınızı da buraya ekleyebilirsiniz
        //};
        //    }
        //private void ReportNullCounts()
        //{
        //    float nullPercentage = 0.0f;
        //    int totalRows = currentDataTable.Rows.Count;
        //    foreach (DataColumn column in currentDataTable.Columns)
        //    {
        //        if (!nullFieldsCheckWithLevel.ContainsKey(column.ColumnName))
        //        {
        //            continue;
        //        }
        //        List<int> nullRows = new List<int>();
        //        int nullCount = 0;
        //        for (int i = 0; i < totalRows; i++)
        //        {
        //            var row = currentDataTable.Rows[i];
        //            if (IsNullLike(row[column]))
        //            {
        //                nullCount++;
        //            }
        //        }
        //        columnNullRowsMap[column.ColumnName] = nullRows;
        //        nullPercentage = (float)nullCount / totalRows;
        //        if (nullPercentage > 0)
        //        {
        //            var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
        //            var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
        //            datatableLevel.Rows.Add(new object[] {
        //                column.ColumnName, "boş olan veriler doldurulmalı !", $"{nullPercentage:P1}"
        //            });
        //        }
        //    }
        //}

        private void ReportDagıtılanCounts()
        {
            float nullPercentage = 1.0f;
            int totalRows = currentDataTable.Rows.Count;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!dagıtılanCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                List<int> nullRows = new List<int>();
                int nullCount = 0;
                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    if (!isDagıtılanİmputed)
                    {
                        nullCount++;
                    }
                }
                columnNullRowsMap[column.ColumnName] = nullRows;
                nullPercentage = (float)nullCount / totalRows;
                if (nullPercentage > 0)
                {
                    var thresholds = dagıtılanCheckWithLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                        column.ColumnName, "bu datalar faturalanan güç değelerine göre düzenlecektir!", $"{nullPercentage:P1}"
                    });
                }
            }
        }

        private void NegativeOrZeroDetect()
        {
            float negativeOrZeroPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!negativeOrZeroLevel.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                List<int> negativeOrZeroRows = new List<int>();
                int negativeOrZeroCount = 0;
                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    if (IsNegativeOrZero(row[column]))
                    {
                        negativeOrZeroCount++;
                        negativeOrZeroRows.Add(i);
                    }
                }
                columnNullRowsMap[column.ColumnName] = negativeOrZeroRows;
                negativeOrZeroPercentage = (float)negativeOrZeroCount / totalRows;
                if (negativeOrZeroPercentage > 0)
                {
                    var thresholds = negativeOrZeroLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(negativeOrZeroPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                column.ColumnName, "negatif veya sıfır olan veriler pozitif olmalı !", $"{negativeOrZeroPercentage:P1}"
            });
                }
            }
        }
        //private string ConvertToPercentageIfNeeded(string columnName, double value, Dictionary<string, bool> percentageColumns)
        //{
        //    // Eğer bu sütun yüzde gösterilmesi gerekenler arasında varsa yüzde formatına çeviriyoruz
        //    if (percentageColumns.ContainsKey(columnName) && percentageColumns[columnName])
        //    {
        //        return (value * 100).ToString("0.00") + " %"; // Yüzde formatına çevir ve % işareti ekle
        //    }
        //    return value.ToString("0.00"); // Yüzde değilse sadece sayısal formatta göster
        //}
        private bool IsNegativeOrZero(object value) // girdimödülüne eklenebilir private degistirilip string to number etc 
        {
            if (value is DBNull || value == null)
            {
                return false;
            }
            if (value is int intValue)
            {
                return intValue <= 0;
            }
            if (value is float floatValue)
            {
                return floatValue <= 0;
            }
            if (value is double doubleValue)
            {
                return doubleValue <= 0;
            }
            if (value is string stringValue)
            {
                if (int.TryParse(stringValue, out int intParsedValue))
                {
                    return intParsedValue <= 0;
                }
                else if (double.TryParse(stringValue, out double doubleParsedValue))
                {
                    return doubleParsedValue <= 0;
                }
            }
            return false;
        }
        //private void CheckConsecutiveYears() // yılların ardısıklık kontrolu
        //{

        //    List<int> years = new List<int>();
        //    int totalRows = currentDataTable.Rows.Count;
        //    string yearColumnName = "YIL";
        //    for (int i = 0; i < totalRows; i++)
        //    {
        //        var row = currentDataTable.Rows[i];
        //        if (int.TryParse(row[yearColumnName].ToString(), out int year))
        //        {
        //            years.Add(year);
        //        }
        //    }
        //    years.Sort(); 
        //    if (years.Count < 5)
        //    {     
        //        return;
        //    }
        //    bool areConsecutive = true;
        //    for (int i = 1; i < years.Count; i++)
        //    {
        //        if (years[i] != years[i - 1] + 1)
        //        {
        //            areConsecutive = false;
        //            break;
        //        }
        //    }
        //    if (areConsecutive)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        var thresholds = yearDetect[yearColumnName];
        //        float nullDataPercentage = 0 / totalRows; //  tartısılabilir ? ama zaten ustteki nulcheckte bakılıyor ve bosluk oranı soyleniyor
        //        var datatableLevel = GetDataTableBasedOnThreshold(nullDataPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
        //        datatableLevel.Rows.Add(new object[] {
        //        yearColumnName, "Yıllar ardışık değil veya en az 5 yıl değil.","","doldurulmalı" });
        //    }
        //}
        private void CheckDataCompleteness(int slfStartYear)
        {
            int startYear = slfStartYear - 1;  // slfStartYear 2024 ise başlangıç yılı 2023 olacak
            List<int> yearsToCheck = Enumerable.Range(startYear - 4, 5).ToList();  // [2023, 2022, 2021, 2020, 2019]

            // Eksik veri ve hatalı veri olan yılları ve hata mesajlarını tutacak yapı
            Dictionary<int, List<string>> missingDataErrors = new Dictionary<int, List<string>>();

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!fiveYearsDataCheck.ContainsKey(column.ColumnName))
                {
                    continue; // Eğer kontrol edilmesi gereken sütunlar arasında değilse geç
                }

                List<int> nullRows = new List<int>();
                int totalRows = currentDataTable.Rows.Count;
                int nullCount = 0;

                // Yıllar aralığında olan satırları kontrol et
                for (int i = 0; i < totalRows; i++)
                {
                    var row = currentDataTable.Rows[i];
                    int year = Convert.ToInt32(row["YIL"]);

                    // Eğer yıl, slfStartYear ve 5 yıllık aralık içinde değilse kontrol etme
                    if (!yearsToCheck.Contains(year))
                    {
                        continue;
                    }

                    // Eğer sütun değeri null veya boş ise null satırlar listesine ekle
                    if (IsNullLike(row[column]))  // Bu kontrol sıfır ve negatif yerine sadece null ve boş string için çalışacak
                    {
                        nullCount++;
                        nullRows.Add(i);
                    }
                }

                // Boş satırları map'e ekle
                columnNullRowsMap[column.ColumnName] = nullRows;

                // Boş veri yüzdesini hesapla
                float nullPercentage = (float)nullCount / yearsToCheck.Count;

                // Eğer yüzde sıfırdan büyükse, eşikleri kontrol et ve uygun mesajı ekle
                if (nullPercentage > 0)
                {
                    var thresholds = fiveYearsDataCheck[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                column.ColumnName, "Başlangıç yılından itibaren geriye dönük 5 yıl datası dolu olmalı!", $"{nullPercentage:P1}"
            });
                }
            }
        }

        public void CheckPercentageIncreaseLoadSanayiTicarethaneFaturalanan()
        {
            int totalRows = currentDataTable.Rows.Count;
            string yil = "YIL";

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!loadPercentageIncreaseDetect.ContainsKey(column.ColumnName))
                {
                    continue; // Kontrol edilecek kolonlar değilse atla
                }

                List<int> increaseNullRows = new List<int>();
                List<int> decreaseNullRows = new List<int>();
                List<string> increaseYearRows = new List<string>();
                List<string> decreaseYearRows = new List<string>();
                List<string> kkoLowYearRows = new List<string>(); // KKO düşük yıllar için yeni liste
                float overIncreaseCount = 0.0f;
                float overDecreaseCount = 0.0f;
                float kkoImpute = 0.0f;

                        for (int i = 1; i < totalRows; i++)
                        {
                            var previousRow = currentDataTable.Rows[i - 1];
                            var row = currentDataTable.Rows[i];

                    // Sütunun değeri DBNull değilse işlemi devam ettir
                    if (!IsNullLike(row[column]) && !IsNullLike(previousRow[column]))
                    {
                        double currentValue = Convert.ToDouble(row[column] == DBNull.Value ? 0 : row[column]);
                        double previousValue = Convert.ToDouble(previousRow[column] == DBNull.Value ? 0 : previousRow[column]);

                        // Yüzdelik fark hesaplama
                        var percentageDiff = currentValue / previousValue;

                        // %10'dan fazla artış kontrolü
                        if (percentageDiff > 1.10)
                        {
                            if (!increaseYearRows.Contains(row[yil].ToString()))
                            {
                                increaseNullRows.Add(i);
                                increaseYearRows.Add(row[yil].ToString());
                                overIncreaseCount++;
                            }
                        }

                        // %50'den fazla azalma kontrolü (KKO ve KKM için)
                        else if (percentageDiff < 0.50 && (column.ColumnName == "KKO" || column.ColumnName == "KKM"))
                        {
                            if (!decreaseYearRows.Contains(row[yil].ToString()))
                            {
                                decreaseNullRows.Add(i);
                                decreaseYearRows.Add(row[yil].ToString());
                                overDecreaseCount++;
                            }
                        }

                        // KKO için ek kontrol: 0.04'ten küçükse
                        if (column.ColumnName == "KKO" && currentValue < 0.04)
                        {
                            if (!kkoLowYearRows.Contains(row[yil].ToString()))
                            {
                                kkoLowYearRows.Add(row[yil].ToString());
                                kkoImpute++;
                            }
                        }

                        // KKO için ek kontrol: 0.04'ten küçükse
                        if (column.ColumnName == "KKO" && currentValue < 0.04)
                        {
                            if (!kkoLowYearRows.Contains(row[yil].ToString()))
                            {
                                kkoLowYearRows.Add(row[yil].ToString());
                                kkoImpute++;
                            }
                        }
                    }
                }

                // %10'dan fazla artış uyarısı
                if (overIncreaseCount > 0)
                {
                    columnNullRowsMap[column.ColumnName] = increaseNullRows;

                    float nullDataPercentage = (float)increaseNullRows.Count / totalRows;
                    string increaseYilValues = string.Join(", ", increaseYearRows);
                    statDataTable.Rows.Add(new object[]
                    {
                column.ColumnName, "%10 artış gözlemlendi UYARI", increaseYilValues, "VERİNİN DOĞRULUĞUNDAN EMİN OLUNMALI!!"
                    });
                }

                // %50'den fazla azalma uyarısı (KKO ve KKM için)
                if (overDecreaseCount > 0)
                {
                    if (column.ColumnName == "KKO" || column.ColumnName == "KKM")
                    {
                        float nullDataPercentage = (float)decreaseNullRows.Count / totalRows;
                        string decreaseYilValues = string.Join(", ", decreaseYearRows);
                        statDataTable.Rows.Add(new object[]
                        {
                    column.ColumnName, "Yıldan yıla kayıp kaçak oranı %50'den fazla UYARI!!", decreaseYilValues, "VERİNİN DOĞRULUĞUNDAN EMİN OLUNMALI!!"
                        });
                    }
                }

                // KKO oranı %0.05'ten küçükse uyarı
                if (kkoImpute > 0)
                {
                    if (column.ColumnName == "KKO")
                    {
                        string kkoYilValues = string.Join(", ", kkoLowYearRows);
                        WarningDataTable.Rows.Add(new object[]
                        {
                    column.ColumnName, "KKO oranı %5'ten küçük, %5 referans alınarak devam edilecektir", kkoYilValues, "VERİNİN DOĞRULUĞUNDAN EMİN OLUNMALI!!"
                        });
                    }
                }
            }
        }


        //private void CheckAndReportMissingFields()
        //{
        //    int totalRows = currentDataTable.Rows.Count;
        //    List<int> problematicRows = new List<int>();

        //    for (int i = 0; i < totalRows; i++)
        //    {
        //        var row = currentDataTable.Rows[i];
        //        bool hasValidField = false;

        //        foreach (var field in kkokkmDagCheck.Keys)
        //        {
        //            if (!IsNullLike(row[field]))
        //            {
        //                hasValidField = true;
        //                break;
        //            }
        //        }

        //        if (!hasValidField)
        //        {
        //            problematicRows.Add(i);
        //        }
        //    }

        //    if (problematicRows.Count > 0)
        //    {
        //        float missingPercentage = (float)problematicRows.Count / totalRows;

        //        foreach (var kvp in kkokkmDagCheck)
        //        {
        //            string fieldName = kvp.Key;
        //            var thresholds = kvp.Value;

        //            DataTable reportTable = GetDataTableBasedOnThreshold(missingPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

        //            if (reportTable != null)
        //            {
        //                reportTable.Rows.Add(new object[] {
        //            fieldName,
        //            $"{fieldName} alanı veya diğer ilgili alanlar (TICARETHANE_DAGITILAN, KKO, KKM) eksik!",
        //            $"{missingPercentage:P1}"
        //        });
        //            }
        //        }
        //    }
        //}

        //private DataTable GetDataTableBasedOnThreshold(float percentage, float warningThreshold, float errorThreshold)
        //{
        //    if (percentage >= errorThreshold)
        //    {
        //        return errorDataTable;
        //    }
        //    else if (percentage >= warningThreshold)
        //    {
        //        return warningDataTable;
        //    }
        //    return null;
        //}
        private void CheckAndReportKkoKkmDag(int slfStartYear)
        {
            List<int> problematicRows = new List<int>();
            int totalRows = 0;


            int startYear = slfStartYear - 1;  // slfStartYear 2024 ise başlangıç yılı 2023 olacak
            int endYear = slfStartYear - 5;
            List<int> yearsToCheck = Enumerable.Range(endYear, (startYear - endYear) + 1).ToList();  // [2023, 2022, 2021, 2020, 2019]

            for (int i = 0; i < currentDataTable.Rows.Count; i++)
            {
                var row = currentDataTable.Rows[i];

                // Yıl kontrolü
                if (!int.TryParse(row["YIL"].ToString(), out int rowYear) || rowYear < endYear || rowYear > startYear)
                {
                    continue; // Geçersiz yıl veya kontrol aralığı dışında, bu satırı atla
                }

                totalRows++; // Geçerli yıl aralığındaki toplam satır sayısını artır

                // KKO, KKM ve TOPLAM_DAGITILAN sütunlarından her üçünün de boş olup olmadığını kontrol ediyoruz
                if (IsNullLike(row["KKO"]) && IsNullLike(row["KKM"]) && IsNullLike(row["TOPLAM_DAGITILAN"]))
                {
                    problematicRows.Add(i); // Eğer her üçü de boşsa bu satır "problematicRows" listesine eklenir
                }
            }

            if (problematicRows.Count > 0)
            {
                float missingPercentage = (float)problematicRows.Count / totalRows;
                string message = $"{startYear} ile {endYear} KKO, KKM ve TOPLAM_DAGITILAN alanlarından en az biri dolu olmalı!";

                if (missingPercentage >= 0.01f) // %1 ve üzeri hata olarak raporlanır
                {
                    errorDataTable.Rows.Add(new object[] {
                "KKO/KKM/TOPLAM_DAGITILAN",
                message,
                $"{missingPercentage:P1}"
            });
                }
                else if (missingPercentage > 0) // %1'den az ise uyarı olarak raporlanır
                {
                    warningDataTable.Rows.Add(new object[] {
                "KKO/KKM/TOPLAM_DAGITILAN",
                message,
                $"{missingPercentage:P1}"
            });
                }
            }
        }

        public void ImputeKkmKkoDag()
        {
            string toplamDagitilan = "TOPLAM_DAGITILAN";
            string kko = "KKO";
            string kkm = "KKM";
            string toplamFaturalanan = "TOPLAM_FATURALANAN";
            double minThreshold = 0.05; // %5 minimum eşik değeri
            int totalRows = currentDataTable.Rows.Count;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                double toplamFaturalananValue;

                // TOPLAM_FATURALANAN değeri al
                if (!TryParseToDouble(row[toplamFaturalanan], out toplamFaturalananValue))
                {
                    continue; // TOPLAM_FATURALANAN değeri geçerli değilse, bu satırı atla
                }

                // KKO hesaplama
                if (IsNullLike(row[kko]))
                {
                    double kkoValue;
                    if (IsNullLike(row[kkm]))
                    {
                        double toplamDagitilantValue;
                        if (TryParseToDouble(row[toplamDagitilan], out toplamDagitilantValue))
                        {
                            kkoValue = 1 - (toplamFaturalananValue / toplamDagitilantValue);
                            kkoValue = Math.Max(kkoValue, minThreshold); // Minimum %5 kontrolü
                            row[kko] = kkoValue;
                        }
                    }
                    else
                    {
                        double kkmValue;
                        if (TryParseToDouble(row[kkm], out kkmValue))
                        {
                            kkoValue = kkmValue / toplamFaturalananValue;
                            kkoValue = Math.Max(kkoValue, minThreshold); // Minimum %5 kontrolü
                            row[kko] = kkoValue;
                        }
                    }
                }
                else
                {
                    // KKO değeri zaten varsa, minimum %5 kontrolü
                    double kkoValue;
                    if (TryParseToDouble(row[kko], out kkoValue))
                    {
                        kkoValue = Math.Max(kkoValue, minThreshold);
                        row[kko] = kkoValue;
                    }
                }

                // TOPLAM_DAGITILAN hesaplama
                if (IsNullLike(row[toplamDagitilan]))
                {
                    double kkoValue;
                    if (TryParseToDouble(row[kko], out kkoValue))
                    {
                        double toplamDagitilantValue = kkoValue * toplamFaturalananValue + toplamFaturalananValue;
                        row[toplamDagitilan] = toplamDagitilantValue;
                    }
                }

                // KKM hesaplama
                if (IsNullLike(row[kkm]))
                {
                    double kkoValue;
                    if (TryParseToDouble(row[kko], out kkoValue))
                    {
                        double kkmValue = kkoValue * toplamFaturalananValue;
                        kkmValue = Math.Max(kkmValue, minThreshold * toplamFaturalananValue); // Minimum %5 kontrolü
                        row[kkm] = kkmValue;
                    }
                }
                else
                {
                    // KKM değeri zaten varsa, minimum %5 kontrolü
                    double kkmValue;
                    if (TryParseToDouble(row[kkm], out kkmValue))
                    {
                        kkmValue = Math.Max(kkmValue, minThreshold * toplamFaturalananValue);
                        row[kkm] = kkmValue;
                    }


                    // KKO'yu yüzde formatında göstermek
                    //if (!IsNullLike(row[kko]))
                    //{
                    //    double kkoValue;
                    //    if (TryParseToDouble(row[kko], out kkoValue))
                    //    {
                    //        row[kko] = ConvertToPercentage(kkoValue); // Yüzdeye çeviriyoruz
                    //    }
                }
            }
        }
        //private double ConvertToPercentage(double value)
        //{
        //    return value * 100; // Sayısal değeri yüzdelik değere çevirmek için 100 ile çarpıyoruz
        //}
        private bool TryParseToDouble(object value, out double result)
        {
            result = 0;
            if (value == null || value == DBNull.Value)
            {
                return false;
            }
            return double.TryParse(value.ToString(), out result);
        }
        public void ImputeDagıtılan() // Dağıtılan kısımlarının imputasyonu
        {
            int totalRows = currentDataTable.Rows.Count;

            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];

                // Mesken-Dagıtılan hesaplama
                if (!IsNullLike(row["KKM"]) && !IsNullLike(row["MESKEN_FATURALANAN"]))
                {
                    double meskenFaturalanan = Convert.ToDouble(row["MESKEN_FATURALANAN"] == DBNull.Value ? 0 : row["MESKEN_FATURALANAN"]);
                    double kkm = Convert.ToDouble(row["KKM"] == DBNull.Value ? 0 : row["KKM"]);
                    row["MESKEN_DAGITILAN"] = kkm + meskenFaturalanan;
                }

                // Sanayi-Dagıtılan hesaplama
                if (!IsNullLike(row["KKM"]) && !IsNullLike(row["SANAYI_FATURALANAN"]))
                {
                    double sanayiFaturalanan = Convert.ToDouble(row["SANAYI_FATURALANAN"] == DBNull.Value ? 0 : row["SANAYI_FATURALANAN"]);
                    double kkm = Convert.ToDouble(row["KKM"] == DBNull.Value ? 0 : row["KKM"]);
                    row["SANAYI_DAGITILAN"] = kkm + sanayiFaturalanan;
                }

                // Ticarethane-Dagıtılan hesaplama
                if (!IsNullLike(row["KKM"]) && !IsNullLike(row["TICARETHANE_FATURALANAN"]))
                {
                    double ticarethaneFaturalanan = Convert.ToDouble(row["TICARETHANE_FATURALANAN"] == DBNull.Value ? 0 : row["TICARETHANE_FATURALANAN"]);
                    double kkm = Convert.ToDouble(row["KKM"] == DBNull.Value ? 0 : row["KKM"]);
                    row["TICARETHANE_DAGITILAN"] = kkm + ticarethaneFaturalanan;
                }

                // Tarımsal Sulama-Dagıtılan hesaplama
                if (!IsNullLike(row["KKM"]) && !IsNullLike(row["TARIMSAL_SULAMA_FATURALANAN"]))
                {
                    double tarimsalSulamaFaturalanan = Convert.ToDouble(row["TARIMSAL_SULAMA_FATURALANAN"] == DBNull.Value ? 0 : row["TARIMSAL_SULAMA_FATURALANAN"]);
                    double kkm = Convert.ToDouble(row["KKM"] == DBNull.Value ? 0 : row["KKM"]);
                    row["TARIMSAL_SULAMA_DAGITILAN"] = kkm + tarimsalSulamaFaturalanan;
                }

                // Aydınlatma-Dagıtılan hesaplama
                if (!IsNullLike(row["KKM"]) && !IsNullLike(row["AYDINLATMA_FATURALANAN"]))
                {
                    double aydinlatmaFaturalanan = Convert.ToDouble(row["AYDINLATMA_FATURALANAN"] == DBNull.Value ? 0 : row["AYDINLATMA_FATURALANAN"]);
                    double kkm = Convert.ToDouble(row["KKM"] == DBNull.Value ? 0 : row["KKM"]);
                    row["AYDINLATMA_DAGITILAN"] = kkm + aydinlatmaFaturalanan;
                }
            }

            isDagıtılanİmputed = true;
        }
        //public void FormatDataTablePercentages()
        //{
        //    // DataTable'ın her bir satırını dolaş
        //    foreach (DataRow row in currentDataTable.Rows)
        //    {
        //        // Sütunlar arasında gezin
        //        foreach (DataColumn column in currentDataTable.Columns)
        //        {
        //            var columnName = column.ColumnName;

        //            // Eğer sütun yüzde formatında gösterilmesi gereken sütunlardansa
        //            if (setConvertPercentage.ContainsKey(columnName))
        //            {
        //                // Hücre değerini kontrol et
        //                if (double.TryParse(row[columnName]?.ToString(), out double value))
        //                {
        //                    // Yüzdelik formatına çeviriyoruz (% işareti ile)
        //                    row[columnName] = (value * 100).ToString("0.00") + " %";
        //                }
        //            }
        //        }
        //    }
        //}

        //excelde kaldım
        public override void Validate()
        {
            base.Validate();
            //ReportNullCounts();
            NegativeOrZeroDetect();
            //CheckConsecutiveYears();
            //CheckBetweenDagıtılanAndKayip();

            CheckAndReportKkoKkmDag(slfStartYear);
            CheckDataCompleteness(slfStartYear);
            CheckPercentageIncreaseLoadSanayiTicarethaneFaturalanan();
            ReportDagıtılanCounts();
        }
        public override void Impute()
        {
            ImputeKkmKkoDag();
            ImputeDagıtılan();
            //FormatDataTablePercentages();
            string filePath = @"C:\Users\begum.orhan\MRC\MRC - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx"; // Excel dosyasının tam yolu
            ExcelExporter exporter = new ExcelExporter();
            exporter.UpdateExcelFileFirstSheet(filePath, currentDataTable);
        }
    }
}

