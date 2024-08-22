//using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using OfficeOpenXml; // Import the EPPlus library
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
                try
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

                    FileInfo file = new FileInfo(filePath);
                    package.SaveAs(file);
                    MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Halihazırda böyle bir dosya açık ve kullanımda. Dosyayı kapatıp yeniden deneyin.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method after showing the message
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("Bu işlemi gerçekleştirmek için bellek yetersiz. Kaydetmek istediğiniz dosya çok büyük olabilir.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method after showing the message
                }
            }
        }

        public void ExportCsvFile(string filePath, DataTable dt)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header line
                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));

                    // Write the data rows
                    foreach (DataRow row in dt.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field =>
                        {
                            string fieldString = field.ToString();
                            // Enclose the field in quotes if it contains a comma
                            return fieldString.Contains(",") ? $"\"{fieldString}\"" : fieldString;
                        });
                        writer.WriteLine(string.Join(",", fields));
                    }
                }
                MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Dosya kaydedilirken bir izin hatası oluştu. Dosyanın kaydedileceği klasörün yazma iznine sahip olduğundan emin olun.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Dosya kaydedilirken bir giriş/çıkış hatası oluştu: {ex.Message}", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilinmeyen bir hata oluştu: {ex.Message}", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
