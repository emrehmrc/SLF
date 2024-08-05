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
            {"TOPLAM_DAGITILAN", WARNING_ONLY},
            {"KKO",WARNING_ONLY },
            {"KKM",WARNING_ONLY },
        };
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
            {"TOPLAM_DAGITILAN", ERROR_ONLY}

        };
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> yearDetect = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "YIL", ERROR_ONLY},
        };
        
        private readonly  Dictionary<string, (float warningThreshold, float errorThreshold)> nullCheckBetweenDagıtılanAndKayip = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
             
            {"TOPLAM_DAGITILAN", ERROR_ONLY},
            {"KKO",ERROR_ONLY },
            {"KKM",ERROR_ONLY },
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
                        column.ColumnName, "boş olan veriler doldurulmalı !", $"{nullPercentage:P1}"
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

            // Tüm yılları topla
            for (int i = 0; i < totalRows; i++)
            {
                var row = currentDataTable.Rows[i];
                if (int.TryParse(row[yearColumnName].ToString(), out int year))
                {
                    years.Add(year);
                }
            }

            
            years.Sort();

            // Ardışıklığı ve en az 5 yıl kontrol et
            if (years.Count < 5)
            {
                Console.WriteLine("Yıllar en az 5 giriş olmalıdır!");
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
                return ;
                

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

            List<int> nullRows = new List<int>();

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
                    // Add the row number and the null-like value to the nullRows
                }
            }

            nullPercentage = (float)nullCount / totalRows;

            if (nullPercentage > 0)
            {
                errorDataTable.Rows.Add(new object[] {
                    "", "boş olan veriler doldurulmalı !", $"{nullPercentage:P1}"
                });
            }
            
        }

        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();
            NegativeOrZeroDetect();
            CheckConsecutiveYears();
            CheckBetweenDagıtılanAndKayip();
        }
    }
}

