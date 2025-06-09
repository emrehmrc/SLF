using OfficeOpenXml; 
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

        public void ExportExcelFileWithMultipleSheets(string filePath, List<DataTable> dataTables, List<string> sheetNames)
        {
            if (dataTables == null || sheetNames == null || dataTables.Count != sheetNames.Count)
            {
                throw new ArgumentException("Veri Tablolarının sayısı, sayfa adlarının sayısıyla eşleşmelidir.");
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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line

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

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                }
            }
        }



        public void ExportExcelFile_poligons(string filePath, DataTable dt, string seçilenVeriTipi, bool poligon_aktarma = false)
        {
            // Create a FileInfo object for the file
            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage())
            {
                try
                {
                    ExcelWorksheet worksheet;
                    int startRow = 1;

                    // Check if the file already exists
                    if (file.Exists)
                    {
                        // Load the existing file
                        using (ExcelPackage existingPackage = new ExcelPackage(file))
                        {
                            worksheet = existingPackage.Workbook.Worksheets[seçilenVeriTipi] ??
                                        existingPackage.Workbook.Worksheets.Add(seçilenVeriTipi);

                            // Find the last used row (skip header row)
                            startRow = worksheet.Dimension?.Rows + 1 ?? 2;

                            // If there's no data yet, start from row 2 (after header)
                            if (startRow == 1) startRow = 2;

                            // Load the new DataTable into the worksheet, starting from the next available row
                            worksheet.Cells[startRow, 1].LoadFromDataTable(dt, false); // false to skip headers

                            // Copy the package content to the new package to avoid file access issues
                            package.Workbook.Worksheets.Add(seçilenVeriTipi, worksheet);
                            worksheet = package.Workbook.Worksheets[seçilenVeriTipi];
                        }
                    }
                    else
                    {
                        // Create a new worksheet
                        worksheet = package.Workbook.Worksheets.Add(seçilenVeriTipi);

                        // Load the DataTable into the worksheet, including headers
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
                    }

                    // AutoFit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Set minimum column width
                    for (int col = 1; col <= dt.Columns.Count; col++)
                    {
                        if (worksheet.Column(col).Width < 15)
                        {
                            worksheet.Column(col).Width = 15;
                        }
                    }

                    // Save the file
                    package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                    package.SaveAs(file);

                    if (!poligon_aktarma)
                    {
                        MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Halihazırda böyle bir dosya açık ve kullanımda. Dosyayı kapatıp yeniden deneyin.",
                        "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("Bu işlemi gerçekleştirmek için bellek yetersiz. Kaydetmek istediğiniz dosya çok büyük olabilir.",
                        "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya kaydedilirken hata oluştu: {ex.Message}",
                        "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        public void ExportExcelFile(string filePath, DataTable dt, string seçilenVeriTipi, bool poligon_aktarma = false)
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

                    if(poligon_aktarma == false)
                    {
                        MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Halihazırda böyle bir dosya açık ve kullanımda. Dosyayı kapatıp yeniden deneyin.", 
                        "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method after showing the message
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("Bu işlemi gerçekleştirmek için bellek yetersiz. Kaydetmek istediğiniz dosya çok büyük olabilir.", 
                        "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method after showing the message
                }
            }
        }

    }
}