//using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using OfficeOpenXml; // Import the EPPlus library
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SLF
{

    internal class ExcelExporter
    {
        public void ExportExcelFile(string filePath, List<DataTable> dataTables, List<string> sheetNames)
        {
            if (dataTables == null || sheetNames == null || dataTables.Count != sheetNames.Count)
            {
                throw new ArgumentException("The number of DataTables must match the number of sheet names.");
            }

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                for (int i = 0; i < dataTables.Count; i++)
                {
                    // Create a worksheet for each DataTable
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetNames[i]);

                    // Load the DataTable into the worksheet, starting from cell A1
                    worksheet.Cells["A1"].LoadFromDataTable(dataTables[i], true);
                }

                // Save the package to the specified file path
                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }
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
