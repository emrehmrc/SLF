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

        public void ExportExcelFileWithMultipleSheets(string filePath, List<DataTable> dataTables, List<string> sheetNames)
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





                    package.SaveAs(file); // CHECK THIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!




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

        public void UpdateExcelFileFirstSheet(string filePath, DataTable dataTable) // excel formullu sayfa güncellemeleri yapılabilir .
        {
            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                try
                {
                    // İlk sayfayı alıyoruz
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // İlk sayfa (0. index)

                    // Excel dosyasındaki satır ve sütun sayısı
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // currentDataTable içeriğini Excel'e yazdır
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            var cell = worksheet.Cells[row + 2, col + 1]; // 2. satırdan itibaren yazıyoruz (1. satır başlık için)

                            // Eğer hücrede formül yoksa
                            if (string.IsNullOrEmpty(cell.Formula)) // Formula yoksa
                            {
                                var cellValue = dataTable.Rows[row][col];

                                // Eğer hücre değeri sayısal ise biçimlendirme uygula
                                if (double.TryParse(cellValue.ToString(), out double numericValue))
                                {
                                    // Hücreye sayısal değeri yaz ve formatla (binlik ayırıcı ve iki ondalık basamak ile)
                                    cell.Value = numericValue;
                                    cell.Style.Numberformat.Format = "#,##0.00"; // Binlik ayırıcı ve iki ondalık basamak
                                }
                                else
                                {
                                    // Sayısal değilse normal değer olarak yaz
                                    cell.Value = cellValue;
                                }
                            }
                        }
                    }

                    // Dosyayı kaydet
                    package.Save();
                    Console.WriteLine("Excel dosyası başarıyla güncellendi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                }
            }
        }


        public void ExportExcelFile(string filePath, DataTable dt, string seçilenVeriTipi)
        {
            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                try
                {
                    // Create a worksheet for each DataTable
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(seçilenVeriTipi);

                    // Load the DataTable into the worksheet, starting from cell A1
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    // Format the header row
                    using (ExcelRange range = worksheet.Cells[1, 1, 1, dt.Columns.Count])
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
                    for (int col = 1; col <= dt.Columns.Count; col++)
                    {
                        if (worksheet.Column(col).Width < 15)
                        {
                            worksheet.Column(col).Width = 15;
                        }
                    }
                    FileInfo file = new FileInfo(filePath);
                    package.Workbook.CalcMode = ExcelCalcMode.Automatic;
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


        // Formül olan sütunları güncellemeyerek koruma
        
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
