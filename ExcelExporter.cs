//using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using OfficeOpenXml; // Import the EPPlus library
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
                    DataTable dt = dataTables[i];
                    // Create a worksheet for each DataTable
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetNames[i]);

                    // Load the DataTable into the worksheet, starting from cell A1
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    // Format the header row
                    using (ExcelRange range = worksheet.Cells[1, 1, 1, dt.Columns.Count]) // assuming your table won't have more than 26 columns
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    // AutoFit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Optionally, set the column width to a minimum value if AutoFit makes it too small
                    for (int col = 1; col <= dataTables[i].Columns.Count; col++)
                    {
                        if (worksheet.Column(col).Width < 15)
                        {
                            worksheet.Column(col).Width = 15;
                        }
                    }
                }

                try
                {
                    FileInfo file = new FileInfo(filePath);
                    package.SaveAs(file);
                    MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Halihazırda böyle bir dosya açık ve kullanımda. Dosyayı kapatıp yeniden deneyin.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method after showing the message
                }
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
