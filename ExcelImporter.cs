//using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using OfficeOpenXml; // Import the EPPlus library
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using DT = System.Data;

namespace SLF
{
    internal class ExcelImporter
    {
        public DT.DataTable ImportExcelFile(string filePath)
        {
            DT.DataTable dataTable = new DT.DataTable();

            // Example of measuring import time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                int previewCount = rowCount; // 100;  // Preview first 100 rows
                int minCount = Math.Min(previewCount, rowCount);

                // Create columns in DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    string columnHeader = worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}";
                    DataColumn column = new DataColumn();
                    column.ColumnName = columnHeader;
                    dataTable.Columns.Add(column);
                }

                // Populate DataTable with Excel data
                for (int row = 2; row <= minCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            stopwatch.Stop();

            Console.WriteLine($"Excel file import took: {stopwatch.ElapsedMilliseconds} ms");
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
