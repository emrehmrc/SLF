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
            { "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID", WARNING_ONLY},
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
        }
        public override void Impute()
        {
            ImputeOnay();

            ImputeEnerjilendirmeYılı();
        }
    }
}


