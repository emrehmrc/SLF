using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using System;
using System.Data;
using DT = System.Data;

namespace SLF
{
    internal class ExcelImporter
    {
        public DT.DataTable ImportExcelFile(string filePath)
        {
            // Create a new instance of Excel Application
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            DT.DataTable dataTable = new DT.DataTable();

            try
            {
                // Open the Excel file
                workbook = excel.Workbooks.Open(filePath);

                // Assuming data is in the first worksheet
                worksheet = (Excel.Worksheet)workbook.Worksheets[1];

                // Get the used range of cells
                Excel.Range usedRange = worksheet.UsedRange;

                // Get the number of rows and columns
                int rowCount = usedRange.Rows.Count;
                int colCount = usedRange.Columns.Count;
                int previewCount = 100;  // Preview first 100 rows
                int minCount = Math.Min(previewCount, rowCount);

                // Create columns in DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    DataColumn column = new DataColumn();
                    column.ColumnName = $"Column{col}";
                    dataTable.Columns.Add(column);
                }

                // Populate DataTable with Excel data
                for (int row = 1; row <= minCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = (usedRange.Cells[row, col] as Excel.Range).Value2;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            finally
            {
                // Close the workbook and Excel application
                workbook?.Close(false);
                excel.Quit();

                // Release COM objects to avoid memory leaks
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(excel);
            }
            return dataTable;
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
