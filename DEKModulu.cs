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
    public class DEKModulu : GirdiModülü

    {
        private readonly (float warningThreshold, float errorThreshold) COORDINATE_ERROR_THRESHOLD = WarningErrorBoundary(0.1f);
        
        private readonly Dictionary<string, (float Min, float Max)> minMaxCheckMap = new Dictionary<string, (float Min, float Max)>
        {
            { "DEK_X_KOORDINAT", (float.MinValue, float.MaxValue) }, // TODO: Update these values from the other data
            { "DEK_Y_KOORDINAT", (float.MinValue, float.MaxValue) } // TODO: Update these values from the other data
            
        };
        private void ReportCoordinatesOutOfLimits()
        {
            var (minXValue, maxXValue) = minMaxCheckMap["DEK_X_KOORDINAT"];
            var (minYValue, maxYValue) = minMaxCheckMap["DEK_Y_KOORDINAT"];

            int countOutOfThresholdCoordinates = 0;

            foreach (DataRow row in currentDataTable.Rows)
            {
                if (float.TryParse(row["TRAFO_X_KOORDINAT"]?.ToString(), out float valueX) && float.TryParse(row["TRAFO_Y_KOORDINAT"]?.ToString(), out float valueY))
                {
                    if (valueX < minXValue || valueX > maxXValue || valueY < minYValue || valueY > maxYValue)
                    {
                        countOutOfThresholdCoordinates++;
                    }
                }
            }
            float outOfThresholdPercentage = (float)countOutOfThresholdCoordinates / currentDataTable.Rows.Count;

            if (outOfThresholdPercentage > 0)
            {
                var thresholds = COORDINATE_ERROR_THRESHOLD;
                var datatableLevel = GetDataTableBasedOnThreshold(outOfThresholdPercentage, thresholds.warningThreshold, thresholds.errorThreshold);

                // Add the warning to the DataTable
                datatableLevel.Rows.Add(new object[] {
                    "TRAFO_X_KOORDINAT & TRAFO_Y_KOORDINAT", "Koordinat Sınırları", $"{outOfThresholdPercentage:P1}", "%10'dan fazla abonede konum bilgisi doğru değildir."
                });
            }
        }

        private readonly Dictionary<string, (float warningThreshold, float errorThreshold)> nullFieldsCheckWithLevel = new Dictionary<string, (float warningThreshold, float errorThreshold)>
        {
            { "ILCE_ADI", INFO_ONLY},
            { "KAYNAK_TIPI", INFO_ONLY},
            { "DEK_KURULU_GUCU", ERROR_ONLY},
            { "DEK_BAGLANDIGI_TRAFO_KODU", ERROR_ONLY},
            { "DEK_X_KOORDINAT", INFO_ONLY},
            { "DEK_Y_KOORDINAT", INFO_ONLY},
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

            ReportCoordinatesOutOfLimits();
        }
    }
}


