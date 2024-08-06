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
    public class FiderVerileri : GirdiModülü

    {
        private readonly (float warningThreshold, float errorThreshold) DEMAND_MAX_THRESHOLD = ERROR_ONLY;
        protected override List<string> Prerequisites => new List<string> { "DTR Verileri" };

        // doğru hesaplamıyor gibi bakmak lazım bir de error veriyor imputasyona geçtiğinde
        private void ReportInvalidPeakDemand()
        {
            // Sabit değerler
            float threshold = 50;

            int countInvalidPeakDemand = 0;

            // DataTable'daki her bir satırı kontrol et
            foreach (DataRow row in currentDataTable.Rows)
            {
                // FIDER_DEMANT sütunundaki değeri float'a dönüştürmeye çalış
                if (float.TryParse(row["FIDER_DEMANT"]?.ToString(), out float demandValue))
                {
                    // Eğer değer 50 veya daha büyükse ya da -50 veya daha küçükse, sayacı artır
                    if (demandValue >= threshold || demandValue <= -threshold)
                    {
                        countInvalidPeakDemand++;
                    }
                }
            }

            // Geçersiz peak demand değerlerinin yüzdesini hesapla
            float invalidPeakDemandPercentage = (float)countInvalidPeakDemand / currentDataTable.Rows.Count;

            // Eğer yüzdelik değer sıfırdan büyükse, tabloya ekleme yap
            if (invalidPeakDemandPercentage > 0)
            {

                var thresholds = DEMAND_MAX_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(invalidPeakDemandPercentage, thresholds.warningThreshold, thresholds.errorThreshold); 
                // Geçersiz peak demand bilgilerini tabloya ekle
                datatableLevel.Rows.Add(new object[] {
            "FIDER_DEMANT", // Hangi parametreyle ilgili olduğu
            "Puant Değer Sınırları", // Sorunun açıklaması
            $"{invalidPeakDemandPercentage:P1}", // Yüzdelik değer
            $"Saatlik maksimum puant değerlerinde 50MW değerinden büyük ya da -50MW değerinden küçük {countInvalidPeakDemand} kadar değer vardır." // Ayrıntılı açıklama
        });
            }
        }

        private void CalculateAnnualPeakDemand()
        {
            annualPeakDemand = currentDataTable.AsEnumerable()
                .Where(row => row.Field<string>("FIDER_TARIH") != null
                      && row.Field<string>("FIDER_ID") != null
                      && row.Field<string>("FIDER_TARIH").Substring(0, 4) == lastYear.ToString())
                .GroupBy(row => row.Field<string>("FIDER_ID"))
                .Select(g => new
                {
                    FiderName = g.Key,
                    Top3PercentAverage = g.Select(row => double.TryParse(row["FIDER_DEMANT"].ToString(), out double value) ? value : (double?)null)
                                          .Where(value => value.HasValue)
                                          .OrderByDescending(value => value.Value)
                                          .Take((int)Math.Max(1, g.Count() * 0.03))
                                          .Average(value => value.Value)
                })
                .ToDictionary(x => x.FiderName, x => x.Top3PercentAverage);
        }

            private void PreprocessMismatchedTMAdi()
        {
            DataTable TMDataTable = dataTablesByType["TM Verileri"];
            var validTrafos = new HashSet<string>(TMDataTable.AsEnumerable()
                                      .Select(row => row["EDW_TM_ID"].ToString())
                                      .Distinct()
            );

            // Loop through currentDataTable to find invalid trafos and their indexes
            foreach (DataRow row in currentDataTable.Rows)
            {
                string connectedTM = row["FIDER_TM_ADI"].ToString();
                if (IsNullLike(connectedTM))
                {
                }
                else if (!validTrafos.Contains(connectedTM))
                {
                    row["FIDER_TM_ADI"] = "#N/A";
                }
            }
        }
        
        private readonly string DATE_FORMAT = "yyyy-MM-dd";
        
        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> dateFormatCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "FIDER_TARIH", INFO_ONLY },
        };
        private void ReportDateFormatErrors()
        {
            float invalidPercentage = 0.0f;
            int totalRows = currentDataTable.Rows.Count;

            foreach (DataColumn column in currentDataTable.Columns)
            {
                if (dateFormatCheckWithLevel.ContainsKey(column.ColumnName))
                {
                    var thresholds = dateFormatCheckWithLevel[column.ColumnName];
                    int invalidCount = 0;
                    var invalidRows = new List<int>();

                    foreach (DataRow row in currentDataTable.Rows) {
                        var value = row[column]?.ToString();
                        if (!DateTime.TryParseExact(value, DATE_FORMAT, null, DateTimeStyles.None, out _))
                        {
                            invalidCount++;
                            invalidRows.Add(row.Table.Rows.IndexOf(row));
                        }
                    }
                    columnNullRowsMap[column.ColumnName] = invalidRows;

                    invalidPercentage = (float)invalidCount / totalRows;

                    if (invalidPercentage > 0)
                    {
                        var datatableLevel = GetDataTableBasedOnThreshold(invalidPercentage, thresholds.warningThreshold, thresholds.errorThreshold);
                        datatableLevel.Rows.Add(new object[]
                        {
                            column.ColumnName, "Geçersiz tarih formatı", $"{invalidPercentage:P1}", $"Tarihler { DATE_FORMAT } biçiminde olmalıdır. Lütfen düzeltiniz."
                        });
                    }
                }
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "FIDER_TM_ADI", ERROR_ONLY},
            { "FIDER_ADI", ERROR_ONLY},
            { "FIDER_DEMAND", WARNING_ONLY},
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
                    if (IsNullLike(row[column], true))
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
        public override void Preprocess()
        {
            //PreprocessMismatchedTMAdi();
            CalculateAnnualPeakDemand();
        }
        public override void Validate()
        {
            base.Validate();

            ReportNullCounts();

            ReportDateFormatErrors();

            ReportInvalidPeakDemand();
        }
        public override void Remove()
        {
            List<int> combinedRowsToRemoveList = new List<int>();

            // Add row indices from different columns to the combined list
            combinedRowsToRemoveList.AddRange(columnNullRowsMap["FIDER_TARIH"]);

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

