using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SLF
{
    public partial class RaporlamaForm : Form
    {
        Tablo_Formu dt;
        public RaporlamaForm()
        {
            InitializeComponent();
            dt = new Tablo_Formu();
            dt.TopLevel = false;    
            dt.FormBorderStyle = FormBorderStyle.None;
            dt.Dock = DockStyle.Fill;
            
        }

        private void aboneSayisiButton_Click(object sender, EventArgs e)
        {
            string excelPath = @"C:\Users\vural.bayrakli\Downloads\1.2_B_Yeni_TR'ler.xlsx";
            Tablo_Formu tf = Fonk(excelPath, aboneSayisiButton.Text);
            DataGridView dgv = tf.vektörel_attribute_table;

            tf.Location = new Point(this.label1.Bottom + 20, tf.Location.Y);


            this.panel1.Controls.Add(tf);
            tf.Show();
            
        }

        private void meskenSayisiButton_Click(object sender, EventArgs e)
        {

        }

        private void DtrButton1_Click(object sender, EventArgs e)
        {
            string excelPath = @"C:\Users\vural.bayrakli\Downloads\1.2_B_Yeni_TR'ler.xlsx";
            Tablo_Formu tf = Fonk(excelPath, aboneSayisiButton.Text);
            DataGridView dgv = tf.vektörel_attribute_table;

            tf.Location = new Point(this.label1.Bottom + 20, tf.Location.Y);


            this.panel1.Controls.Add(tf);
            tf.Show();
        }

        private void slfDemandButton2_Click(object sender, EventArgs e)
        {

        }

        private Tablo_Formu Fonk(string excelPath, string lableText)
        {
            DataTable dataTable = ImportExcelFile(excelPath, "Abone Sayısı");
            MessageBox.Show("Excel dosyası başarıyla yüklendi.");
            dt.vektörel_attribute_table.DataSource = dataTable;

            this.label1.Text = lableText;
            //this.label1.Visible = true;

            return dt;
        }
        public DataTable ImportExcelFile(string filePath, string seçilenVeriTipi)
        {
            DataTable dataTable = new DataTable();

            // Example of measuring import time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

                // Validate column headers. In case it fails, it throws an exception.

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Create columns in DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    DataColumn column = new DataColumn();
                    column.ColumnName = worksheet.Cells[1, col].Text;
                    dataTable.Columns.Add(column);
                }

                // Populate DataTable with Excel data
                // Row starts from 2 because 1st row is column headers
                for (int row = 2; row <= rowCount; row++)
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

        private void ExcelDownloadButton_Click(object sender, EventArgs e)
        {
            string fp = @"C:\Users\vural.bayrakli\Downloads\test.xlsx";
            DataTable dttest = dt.vektörel_attribute_table.DataSource as DataTable;
            ExportExcelFile(dttest, "test");
        }

        public void ExportExcelFile(DataTable dt, string seçilenVeriTipi)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Excel Dosyasını Kaydet";
                saveFileDialog.FileName = seçilenVeriTipi + ".xlsx"; // Default file name

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Create a new Excel package
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        try
                        {
                            // Create a worksheet for the DataTable
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

                            // Ensure minimum column width
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

                            MessageBox.Show("Dosya başarıyla kaydedildi:\n" + filePath, "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show("Halihazırda böyle bir dosya açık ve kullanımda. Dosyayı kapatıp yeniden deneyin.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        catch (OutOfMemoryException)
                        {
                            MessageBox.Show("Bu işlemi gerçekleştirmek için bellek yetersiz. Kaydetmek istediğiniz dosya çok büyük olabilir.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }
        public void SaveGMapAsTiff(GMapControl gmap)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "TIFF Image|*.tiff;*.tif";
                saveFileDialog.Title = "Save Map as TIFF";
                saveFileDialog.FileName = "GMap_Image.tiff";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // Create a bitmap to store the GMap image
                        using (Bitmap bmp = new Bitmap(gmap.Width, gmap.Height))
                        {
                            // Render GMap content onto the bitmap
                            gmap.DrawToBitmap(bmp, new Rectangle(0, 0, gmap.Width, gmap.Height));

                            // Save the bitmap as a TIFF file
                            bmp.Save(filePath, ImageFormat.Tiff);
                        }

                        MessageBox.Show("Map successfully saved as TIFF:\n" + filePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving TIFF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
    }
