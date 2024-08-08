using System;
using System.Collections.Generic;
using System.Data;

namespace SLF
{
    public class EkonometrikYukTahminiModulu : GirdiModülü

    {
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "MESKEN_FATURALANAN", ERROR_ONLY},
            { "SANAYI_FATURALANAN", ERROR_ONLY},
            { "TICARETHANE_FATURALANAN", ERROR_ONLY},
            { "AYDINLATMA_FATURALANAN", ERROR_ONLY},
            { "YIL", ERROR_ONLY},
            {"TARIMSAL_SULAMA_FATURALANAN",ERROR_ONLY },
            {"KKO",WARNING_ONLY },
            {"KKM",WARNING_ONLY },
            { "PUANT_YAZ", ERROR_ONLY},
            { "PUANT_KIŞ", ERROR_ONLY},
            {"TOPLAM_DAGITILAN", WARNING_ONLY},        };
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
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> loadPercentageIncreaseDetect = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {

            {"MESKEN_FATURALANAN", WARNING_ONLY},
            {"SANAYI_FATURALANAN", WARNING_ONLY},
            {"TICARETHANE_FATURALANAN", WARNING_ONLY},
        };
        private bool isDagıtılanİmputed;
        public override void Preprocess()
        {
            isDagıtılanİmputed = false;
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
                    }
                }
                columnNullRowsMap[column.ColumnName] = nullRows;
                nullPercentage = (float)nullCount / totalRows;
                if (nullPercentage > 0)
                {
                    var thresholds = nullFieldsCheckWithLevel[column.ColumnName];
                    var datatableLevel = GetDataTableBasedOnThreshold(nullPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                    datatableLevel.Rows.Add(new object[] {
                        column.ColumnName, "boş olan veriler doldurulmalı !", $"{nullPercentage:P1}"
                    });
                }
            }
        }
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
        private void CheckConsecutiveYears()
        {
            List<int> years = new List<int>();
            int totalRows = currentDataTable.Rows.Count;
            string yearColumnName = "YIL";
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (int.TryParse(row[yearColumnName].ToString(), out int year))
                {
                    years.Add(year);
                }
            }
            years.Sort(); 
            if (years.Count < 5)
            {     
                return;
            }
            bool areConsecutive = true;
            for (int i = 1; i < years.Count; i++)
            {
                if (years[i] != years[i - 1] + 1)
                {
                    areConsecutive = false;
                    break;
                }
            }
            if (areConsecutive)
            {
                return;
            }
            else
            {
                var thresholds = yearDetect[yearColumnName];
                float nullDataPercentage = 0 / totalRows; //  tartısılabilir ? ama zaten ustteki nulcheckte bakılıyor ve bosluk oranı soyleniyor
                var datatableLevel = GetDataTableBasedOnThreshold(nullDataPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                datatableLevel.Rows.Add(new object[] {
                yearColumnName, "Yıllar ardışık değil veya en az 5 yıl değil.","","doldurulmalı" });
            }
        }
        private void CheckBetweenDagıtılanAndKayip()
        {
            float nullPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;
            string toplamDagitilan = "TOPLAM_DAGITILAN";
            string kko = "KKO";
            string kkm = "KKM";
            int nullCount = 0;
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (IsNullLike(row[toplamDagitilan]) && IsNullLike(row[kko]) && IsNullLike(row[kkm]))
                {
                    nullCount++;
                }
                nullPercentage = (float)nullCount / totalRows;
                if (nullPercentage > 0)
                {
                    errorDataTable.Rows.Add(new object[] {
                    "UYARI!", "kayıp kaçak verilerinden veya toplam dagıtılan güç verisinden ez birinin tüm datası girilmeli !", $"{nullPercentage:P1}"
                });
                }
            }

        }
        public void CheckPercentageIncreaseLoadSanayiTicarethaneFaturalanan()
        {
            int totalRows = currentDataTable.Rows.Count;
            string ticarethaneFaturalanan = "TICARETHANE_FATURALANAN";
            string meskenFaturalanan = "MESKEN_FATURALANAN";
            string sanayiFaturalanan = "SANAYI_FATURALANAN";
            string yil = "YIL";
            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (!loadPercentageIncreaseDetect.ContainsKey(column.ColumnName))
                {
                    continue;
                }
                List<int> increaseNullRows = new List<int>();
                List<int> decreaseNullRows = new List<int>();
                List<string> increaseYearRows = new List<string>();
                List<string> decreaseYearRows = new List<string>();
                float overIncreaseCount = 0.0f;
                float overDecreaseCount = 0.0f;

                for (int i = 1; i < totalRows; i++)
                {
                    var previousRow = currentDataTable.Rows[i - 1];
                    var row = currentDataTable.Rows[i];

                    if (!IsNullLike(row[column]))
                    {
                        var percentageDiff = Convert.ToDouble(row[column]) / Convert.ToDouble(previousRow[column]);
                        if (percentageDiff > 1.10)
                        {       
                            increaseNullRows.Add(i);
                            increaseYearRows.Add(row[yil].ToString());
                            overIncreaseCount++;
                        }
                        else if (percentageDiff < 0.90)
                        {  
                            decreaseNullRows.Add(i);
                            decreaseYearRows.Add(row[yil].ToString());
                            overDecreaseCount++;
                        }
                    }
                }

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

                if (overDecreaseCount > 0)
                {
                    columnNullRowsMap[column.ColumnName] = decreaseNullRows;            
                    float nullDataPercentage = (float)decreaseNullRows.Count / totalRows;
                    string decreaseYilValues = string.Join(", ", decreaseYearRows);
                    statDataTable.Rows.Add(new object[]
                    {
                column.ColumnName, "%10 düşüş gözlemlendi UYARI", decreaseYilValues, "VERİNİN DOĞRULUĞUNDAN EMİN OLUNMALI!!"
                    });
                }
            }
       }
        public void ImputeKkmKkoDag()
        {
            string toplamDagitilan = "TOPLAM_DAGITILAN";
            string kko = "KKO";
            string kkm = "KKM";      
            int totalRows = currentDataTable.Rows.Count;
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (IsNullLike(row[kko]))
                {       if (IsNullLike(row[kkm])) {
                        row[kko] = Convert.ToDouble(row[toplamDagitilan]) / Convert.ToDouble(row["TOPLAM_FATURALANAN"]);
                    }
                    else
                    {
                        row[kko] = Convert.ToDouble(row[kkm]) / Convert.ToDouble(row["TOPLAM_FATURALANAN"]);
                    }             }
                if (IsNullLike(row[toplamDagitilan]))
                {
                   row[toplamDagitilan] =  Convert.ToDouble(row[kko]) * Convert.ToDouble(row["TOPLAM_FATURALANAN"]) + Convert.ToDouble(row["TOPLAM_FATURALANAN"]) ;
                }
                if (IsNullLike(row[kkm]))
                {
                    row[kkm] = Convert.ToDouble(row[kko]) * Convert.ToDouble(row["TOPLAM_FATURALANAN"]);
               }             
            }
          
        } 
        public void ImputeDagıtılan()
        {  int totalRows = currentDataTable.Rows.Count;
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (true) // mesken-dagıtılan
                {
                    row["MESKEN_DAGITILAN"] = Convert.ToDouble(row["KKM"]) + Convert.ToDouble(row["MESKEN_FATURALANAN"]);
                }
                if (true) // sanayi-dagıtılan
                {
                    row["SANAYI_DAGITILAN"] = Convert.ToDouble(row["KKM"]) + Convert.ToDouble(row["SANAYI_FATURALANAN"]);
                }
                if (true) // ticarethane-dagıtılan
                {
                    row["TICARETHANE_DAGITILAN"] = Convert.ToDouble(row["KKM"]) + Convert.ToDouble(row["TICARETHANE_FATURALANAN"]);
                }
                if (true) // tarımsal-sulama-dagıtılan
                {
                    row["TARIMSAL_SULAMA_DAGITILAN"] = Convert.ToDouble(row["KKM"]) + Convert.ToDouble(row["TARIMSAL_SULAMA_FATURALANAN"]);
                }
                if (true) // AYDINLATMA_DAGITILAN
                {
               row["AYDINLATMA_DAGITILAN"] = Convert.ToDouble(row["KKM"]) + Convert.ToDouble(row["AYDINLATMA_FATURALANAN"]);
                }

          }
           isDagıtılanİmputed = true;
    }
        public override void Validate()
        {
            base.Validate();
            ReportNullCounts();
            NegativeOrZeroDetect();
            CheckConsecutiveYears();
            CheckBetweenDagıtılanAndKayip();
           CheckPercentageIncreaseLoadSanayiTicarethaneFaturalanan(); 
            ReportDagıtılanCounts();        }
        public override void Impute()
       {
            ImputeKkmKkoDag();
            ImputeDagıtılan();    
        }
    }
}

