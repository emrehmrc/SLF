using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using SLF.Optimal_DTR;
using SLF.RaporlamaDosyası.FiltrelemeArayuz;
using SLF.Services;
using GMap.NET.WindowsForms.Markers;
using System.Data.SQLite;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Common.Logging;
using SharpKml.Dom;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Xml;
using System.Diagnostics;
using Point = System.Drawing.Point;
using System.Data.Entity.Infrastructure;
using Encoding = System.Text.Encoding;
using SLF.RaporlamaDosyası.Tablo;

namespace SLF.RaporlamaDosyası
{
    public partial class Rapor_Arayuz : Form
    {

        EA eA;
        DTR dTR;
        DEK dek;
        Alansal yuk;


        GMapOverlay overlay;

        ExcelImporter excelImporter = new ExcelImporter();

        Tablo_Formu tablo_Formu = new Tablo_Formu2();

        public DataTable DataTableDTR { get; set; } = new DataTable();
        public DataTable DataTableEA { get; set; } = new DataTable();
        public DataTable DataTableYuk { get; set; } = new DataTable();
        public DataTable DataTableDEK { get; set; } = new DataTable(); 

        public DataTable currentDt { get; set; }

        string KullanilanModul = string.Empty;

        string İlİlceYol;

        string ProjeYolu;

        string ODTRSonucYolu;

        string ELFSonucYolu;

        
        string İlYol;

        string YükVeriYolu;

        string İmarVeriYolu;

        string SonucYolu;

        string PythonFilePath;

        string ArsivVeriYolu;

        string PythonPath;

        public Rapor_Arayuz()
        {
            InitializeComponent();

            /*var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("config 1.json")
           .Build();

            İlİlceYol = Path.Combine(config["Ana_Klasör_Yolu"], config["İl"], config["İlçe"]);

            ELFSonucYolu = Path.Combine(İlİlceYol, config["ELF:SONUÇLAR_klasör"]);

            İlYol = Path.Combine(config["Ana_Klasör_Yolu"], config["İl"]);

            SonucYolu = Path.Combine(İlİlceYol, config["ODTR:Klasör"]);

            YükVeriYolu = Path.Combine(İlİlceYol, config["ODTR:INPUT_klasör2"]);

            İmarVeriYolu = Path.Combine(İlİlceYol, config["ODTR:INPUT_klasör1"]);

            currentDt = new DataTable();*/

            var configPath = @"C:\Users\vural.bayrakli\OneDrive - MRC\İletişim sitesi - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Program Dosyaları\configVural.json";
            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();

            İlYol = Path.Combine(config["Ana_Klasör_Yolu"], config["İl"]);

            İlİlceYol = Path.Combine(config["Ana_Klasör_Yolu"], config["İl"], config["İlçe"]);

            SonucYolu = Path.Combine(İlİlceYol, config["proje_dosyası"], config["ODTR:Sonuçlar_klasör"]);

            PythonFilePath = Path.Combine(İlYol, config["program_dosyaları_path"], config["ODTR:PYTHON_klasör"]);

            YükVeriYolu = Path.Combine(İlİlceYol, config["proje_dosyası"], config["ODTR:INPUT_Yük_klasör"]);

            İmarVeriYolu = Path.Combine(İlİlceYol, config["proje_dosyası"], config["ODTR:INPUT_Trafo_klasör"]);
            
            PythonPath = GetPythonPath();

            if (PythonPath == null)
            {
                MessageBox.Show("Python yolu bulunamadı. Lütfen Python yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ArsivVeriYolu = Path.Combine(SonucYolu, "Arşiv");

            if (!Directory.Exists(ArsivVeriYolu))
            {
                Directory.CreateDirectory(ArsivVeriYolu);
            }

        }

        /*private void InitializeMap()
        {

            overlay = new GMapOverlay("map");

            gMapControl1.Dock = DockStyle.Fill;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 12;
            gMapControl1.Position = new PointLatLng(38.5, 27.0); // Başlangıç konumu
            gMapControl1.DragButton = MouseButtons.Left;

        }*/


        

        

        public bool DosyaMevcutMu(string yol, string dosya)
        {
    
            if (!Directory.Exists(yol))
            {
                MessageBox.Show("Klasör Yok");
                return false;
            }

            if((!File.Exists(dosya)) | (dosya == null))
            {
                MessageBox.Show("Dosya Yok");
                return false;
            }

            return true;

        }

        public void FiltrelemeKismi(Form a, string modul)
        {
            /*this.panel3.Controls.Clear();

            a.Name = modul;
            a.Text = modul;
            a.TopLevel = false;
            a.FormBorderStyle = FormBorderStyle.None;
            a.Dock = DockStyle.Fill;
            a.Parent = this.panel3;
            a.Show();*/

        }

        public void VeriYazdir(string yol)
        {
            this.panel4.Controls.Clear();

            DataTable dt = ImportExcelFile(yol);

            /*tablo_Formu.vektörel_attribute_table.DataSource = dt;
            tablo_Formu.FormBorderStyle = FormBorderStyle.None;
            tablo_Formu.vektörel_attribute_table.Dock = DockStyle.Fill;
            tablo_Formu.TopLevel = false;
            tablo_Formu.Show();


            tablo_Formu.vektörel_attribute_table.Parent = this.panel5;*/


            var sonuc = from row in dt.AsEnumerable()
                       where row.Field<string>("Durum") == "Mevcut"
                       select row;

            DataTable filteredTable = sonuc.CopyToDataTable();

            

            
        }

        public void VeriYazdir2(DataTable dt)
        {
            this.panel5.Controls.Clear();

            tablo_Formu.vektörel_attribute_table.DataSource = dt;
            tablo_Formu.FormBorderStyle = FormBorderStyle.None;
            tablo_Formu.vektörel_attribute_table.Dock = DockStyle.Fill;
            tablo_Formu.TopLevel = false;
            tablo_Formu.Show();


            this.panel5.Controls.Add(tablo_Formu.vektörel_attribute_table);
            

        }





        public string DosyaSeciciGoster(string klasorYolu)
        {
            Form form = new Form();
            form.Text = "Dosya Seç";
            form.Width = 500;
            form.Height = 400;
            form.StartPosition = FormStartPosition.CenterParent;

            ListBox listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Segoe UI", 12); // Yazı boyutu büyütüldü

            Button btnSec = new Button();
            btnSec.Text = "Seç";
            btnSec.Dock = DockStyle.Bottom;
            btnSec.Font = new Font("Segoe UI", 12); // Buton yazısı büyük

            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;

            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Segoe UI", 12);
            contentPanel.Controls.Add(listBox);

            btnSec.Height = 50;
            btnSec.Dock = DockStyle.Bottom;
            btnSec.Font = new Font("Segoe UI", 12);

            // Form’a ekle
            form.Controls.Add(contentPanel);
            form.Controls.Add(btnSec);

            string secilenDosya = null;

            if (Directory.Exists(klasorYolu))
            {
                string[] dosyaYollari = Directory.GetFiles(klasorYolu);
                foreach (var yol in dosyaYollari)
                {
                    listBox.Items.Add(Path.GetFileName(yol)); // sadece dosya adı
                }
            }
            else
            {
                listBox.Items.Add("Dosya bulunamadı.");
                btnSec.Enabled = false;
            }

            btnSec.Click += (s, e) =>
            {
                if (listBox.SelectedItem != null)
                {
                    secilenDosya = listBox.SelectedItem.ToString(); // sadece dosya adı döner
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen bir dosya seçin.");
                }
            };


            form.ShowDialog();

            return secilenDosya;
        }





        private void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            button2.Visible = true;
            label7.Visible = true;
            button5.Visible = true;

            KullanilanModul = "DTR";

            this.panel3.Controls.Clear();

            string dtyol = DosyaSeciciGoster(SonucYolu);

            try
            {
                dtyol = Path.Combine(SonucYolu, dtyol);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            try
            {
                

                /*if (DosyaMevcutMu(ProjeYolu, ODTRSonucYolu))
                {
                    DataTableDTR = excelImporter.ImportExcelFile(ODTRSonucYolu, null, false);

                    VeriYazdir2(DataTableDTR);

                }*/

                if (DosyaMevcutMu(SonucYolu, dtyol))
                {
                    DataTableDTR = ImportExcelFile(dtyol);

                    currentDt = DataTableDTR.Copy();

                    // Şimdi, DataTableYuk'un satırlarını currentDt'ye ekliyoruz
                    

                    if (dTR == null)
                    {
                        dTR = new DTR(DataTableDTR);
                        dTR.TopLevel = false;
                        dTR.FormBorderStyle = FormBorderStyle.None;

                        dTR.FiltrelemeYapildi += Form_FiltrelemeYapildi;

                    }

                    this.panel3.Controls.Add(dTR.panel3);

                    FiltrelemeKismi(dTR, "DTR");

                    VeriYazdir2(DataTableDTR);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Form_FiltrelemeYapildi(object sender, FiltreEventArgs e)
        {
            // Parametreli fonksiyon çağır
            
            
            UygulaFiltre(e.dt);
            

        }

        private void UygulaFiltre(DataTable dt)
        {

            if (KullanilanModul == "DTR")
            {
                //dTR.DrawMap(dTR.FiltrelenmisSonuc, overlay, gMapControl1);
                currentDt = dt.Copy();
                VeriYazdir2(dt);
            }
            else if (KullanilanModul == "YUK")
            {
                currentDt = dt.Copy();
                VeriYazdir2(dt);
            }

            else if (KullanilanModul == "DEK")
            {
                
            }

            else
            {
                
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            KullanilanModul = "EA";

            string isim = "EA";

            string yolDir = Path.Combine(SonucYolu, isim);

            string d = DosyaSeciciGoster(yolDir);

            string dosya = Path.Combine(yolDir, d);

            if (!DosyaMevcutMu(yolDir, dosya))
            {
                DataTableEA = ImportExcelFile(dosya);

                if (eA == null)
                {
                    eA = new EA(DataTableEA);
                    eA.FiltrelemeYapildi += Form_FiltrelemeYapildi;

                }

                FiltrelemeKismi(eA, "EA");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string isim = "DEK";

            string yolDir = Path.Combine(SonucYolu, isim);

            string d = DosyaSeciciGoster(yolDir);

            string dosya = Path.Combine(yolDir, d);

            if (!DosyaMevcutMu(yolDir, dosya))
            {
                DataTableDEK = ImportExcelFile(dosya);

                if (dek == null)
                {
                    dek = new DEK(DataTableDEK);
                    eA.FiltrelemeYapildi += Form_FiltrelemeYapildi;
                }

                FiltrelemeKismi(dek, "DEK");
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            button2.Visible = false;
            label7.Visible = false;
            button5.Visible = false;

            KullanilanModul = "YUK";


            this.panel3.Controls.Clear();
            this.panel5.Controls.Clear();



            string veritabaniYolu = Path.Combine(PythonFilePath, "veriler.db");

            // LOADING GIF EKLE
            PictureBox loadingGif = new PictureBox();
            loadingGif.SizeMode = PictureBoxSizeMode.AutoSize;
            string gifPath = Path.Combine(PythonFilePath, "l1.gif");
            loadingGif.Image = Image.FromFile(gifPath);
            loadingGif.Location = new Point(
                (panel5.Width - loadingGif.Width) / 2,
                (panel5.Height - loadingGif.Height) / 2
            );
            panel5.Controls.Add(loadingGif);
            panel5.Refresh();

            // ✨ UI thread'e nefes ver
            await Task.Delay(1);
            Application.DoEvents();


            try
            {
                if (DosyaMevcutMu(SonucYolu, veritabaniYolu))
                {
                    string sqlQuery = "SELECT * FROM dfYukButun";
                    //DataTableYuk = await GetDataTableFromSQLite(veritabaniYolu, sqlQuery).ConfigureAwait(false);

                    DataTableYuk = await Task.Run(() =>
                    {
                        return GetDataTableFromSQLite(veritabaniYolu, sqlQuery).Result;
                    });

                    currentDt = DataTableYuk.Copy();

                    if (yuk == null)
                    {
                        yuk = new Alansal(DataTableYuk);
                        yuk.FiltrelemeYapildi += Form_FiltrelemeYapildi;
                    }

                    this.panel3.Controls.Add(yuk.panel3);
                    VeriYazdir2(DataTableYuk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // ✅ LOADING ANİMASYONUNU KALDIR
                panel5.Controls.Remove(loadingGif);
                loadingGif.Dispose();
            }
        }




        public async Task<DataTable> GetDataTableFromSQLite(string dbPath, string sqlQuery)
            {
                DataTable dt = new DataTable();

                string connectionString = $"Data Source={dbPath};Version=3;";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, conn))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    conn.Close();
                }

                return dt;
            }

        private void ExcelDownloadButton_Click(object sender, EventArgs e)
        {

        }

        private async void ExcelDownloadButton_Click_1(object sender, EventArgs e)
        {
            // Örnek DataTable oluşturma
            string gifPath = Path.Combine(PythonFilePath, "l1.gif");

            string filePath = null;

            PictureBox aktifGif = ShowLoadingGifNextToButton(ExcelDownloadButton, panel7, gifPath);

            await Task.Delay(1);         // animasyonun başlama şansı olsun
            Application.DoEvents();      // UI thread'e nefes ver       

            // Excel dosyasını oluşturma
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save an Excel File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog.FileName;

            }
            try
            {
                // EPPlus kullanarak DataTable'ı Excel dosyasına kaydet
                //ExportDataTableToExcel(currentDt, filePath);
                await Task.Run(() =>
                {
                    ExportExcelFile(filePath, currentDt, KullanilanModul);
                });
                   
                MessageBox.Show("Excel dosyası başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            catch 
            {
                MessageBox.Show(e.ToString());
            }

            finally
            {
                // ✅ LOADING ANİMASYONUNU KALDIR
                panel7.Controls.Remove(aktifGif);
                aktifGif.Dispose();
            }



            }

            
        

        private void ExportDataTableToExcel(DataTable dt, string filePath)
        {
            MessageBox.Show(dt.Rows.Count.ToString());
            using (ExcelPackage package = new ExcelPackage())
            {
                // Yeni bir çalışma sayfası oluştur
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // DataTable'ı Excel'e yazma
                worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);

                // Excel dosyasını kaydet
                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }

            MessageBox.Show("Excel dosyası başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ExportExcelFile(string filePath, DataTable dt, string seçilenVeriTipi="excel")
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
                    
                    //MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public void ExportDataTableToKML(DataTable dt, string filePath)
        {                     
                // KML belgesi oluşturma
                XmlDocument xmlDoc = new XmlDocument();

                // KML kök elemanını oluştur
                XmlElement kmlElement = xmlDoc.CreateElement("kml");
                kmlElement.SetAttribute("xmlns", "http://www.opengis.net/kml/2.2");
                xmlDoc.AppendChild(kmlElement);

                // KML Document elemanı ekleyelim
                XmlElement documentElement = xmlDoc.CreateElement("Document");
                kmlElement.AppendChild(documentElement);

                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // Placemark elemanı
                        XmlElement placemarkElement = xmlDoc.CreateElement("Placemark");
                        documentElement.AppendChild(placemarkElement);

                        // Name elemanı
                        XmlElement nameElement = xmlDoc.CreateElement("name");
                        nameElement.InnerText = row["trafo_id"].ToString();  // "trafo_id" kolonunu kullan
                        placemarkElement.AppendChild(nameElement);

                        // Description elemanı
                        XmlElement descriptionElement = xmlDoc.CreateElement("description");
                        descriptionElement.InnerText = $"Trafo Yaşı: {row["trafo_yasi"]}, Kapasite: {row["kapasite"]}, Aksiyon: {row["Trafo Aksiyon"]}";
                        placemarkElement.AppendChild(descriptionElement);

                        // Point elemanı (koordinatlar)
                        XmlElement pointElement = xmlDoc.CreateElement("Point");
                        placemarkElement.AppendChild(pointElement);

                        // Koordinatlar elemanı (Koord_x ve Koord_y'yi kullanıyoruz)
                        XmlElement coordinatesElement = xmlDoc.CreateElement("coordinates");
                        string x = row["Koord_x"].ToString(); // "Koord_x" kolonunu kullan
                        string y = row["Koord_y"].ToString();  // "Koord_y" kolonunu kullan
                        coordinatesElement.InnerText = $"{x},{y},0"; // X, Y, 0 (yükseklik)
                        pointElement.AppendChild(coordinatesElement);
                    }
                    xmlDoc.Save(filePath);

                    MessageBox.Show("KML dosyası başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"KML dosyası kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        private async void button5_Click(object sender, EventArgs e)
        {

            // Örnek DataTable oluşturma
            string gifPath = Path.Combine(PythonFilePath, "l1.gif");

            PictureBox aktifGif = ShowLoadingGifNextToButton(button5, panel7, gifPath);

            await Task.Delay(1);         // animasyonun başlama şansı olsun
            Application.DoEvents();

            // SaveFileDialog ile kullanıcıdan dosya yolu alalım
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "KML Files|*.kml";
            saveFileDialog.Title = "Save KML File";

            string filePath = null;

            // Kullanıcı bir dosya yolu seçerse
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog.FileName; // Seçilen dosya yolu
            }

                await Task.Run(() =>
            {
                ExportDataTableToKML(currentDt, filePath);
            });

            // ✅ LOADING ANİMASYONUNU KALDIR
            panel7.Controls.Remove(aktifGif);
            aktifGif.Dispose();


        }

        public DataTable ImportExcelFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            // Example of measuring import time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet               

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

        private async void button2_Click_1(object sender, EventArgs e)
        {
            string gifPath = Path.Combine(PythonFilePath, "l1.gif");

            PictureBox aktifGif = ShowLoadingGifNextToButton(button2, panel7, gifPath);


            await Task.Delay(1);         // animasyonun başlama şansı olsun
            Application.DoEvents();      // UI thread'e nefes ver

            string filePath = Path.Combine(ArsivVeriYolu, "Sonuç.xlsx");

            await Task.Run(() =>
            {
                ExportExcelFile(filePath, currentDt, KullanilanModul);
            });       

            string python_path = Path.Combine(PythonFilePath, "PydeckRun.py");

            MessageBox.Show("Python dosyası çalıştırılıyor...");         

            try
            {
                
                await PythonScriptCalistir(python_path, filePath);

                MessageBox.Show("İşlem başarıyla tamamlandı.");
                
            }


            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // ✅ LOADING ANİMASYONUNU KALDIR
                panel7.Controls.Remove(aktifGif);
                aktifGif.Dispose();
            }


        }

        public async Task PythonScriptCalistir(string python_path, string inputpath)
        {

            Form form = new Form
            {
                // Formun başlangıç pozisyonunu ekranın merkezine ayarlıyoruz
                StartPosition = FormStartPosition.CenterScreen,
                // Form boyutunu belirliyoruz
                Size = new Size(300, 400),
                // Form başlığını ayarlıyoruz
                Text = "İşlem devam ediyor...",

                TopMost = true,
                AutoScroll = true,

            };

            form.Show();

            // Yeni bir ProgressBar oluşturuluyor
            ProgressBar progressBar1 = new ProgressBar
            {
                // ProgressBar stilini Marquee olarak ayarlıyoruz
                Style = ProgressBarStyle.Marquee,
                // ProgressBar boyutunu ayarlıyoruz
                Size = new Size(200, 20),

                AutoSize = true,
                // Yükseklik ve genişlik için formun merkezine yerleştirilecek
            };

            progressBar1.Location = new Point((form.Width - progressBar1.Width) / 2, 50);  // 50px uzaklıkta yerleştiriyoruz

            // Yeni bir Label oluşturuluyor
            Label label = new Label
            {
                // Label metnini boş bırakıyoruz, istediğiniz metni buraya ekleyebilirsiniz
                Text = "",

                AutoSize = true, // Label'ın boyutunu otomatik olarak ayarlıyoruz
                // Label'ın yerini ayarlıyoruz (alt tarafta ve ortada)
            };

            label.Location = new Point(progressBar1.Left, progressBar1.Bottom + 10); // ProgressBar'ın altında

            label.Show();

            // Form'a ProgressBar'ı ve Label'ı ekliyoruz
            form.Controls.Add(progressBar1);

            form.Controls.Add(label);

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = $"{PythonPath}", // Python'ın yüklü olduğu path
                    //Arguments = $"{PythonFilePath} \"{inputpath}\"",
                    Arguments = $"\"{python_path}\" \"{inputpath}\"  \"{SonucYolu}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8 // Çıktıyı UTF-8 olarak al

                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    Console.WriteLine("Python script çalıştırıldı");
                    // Python script'inin çıktısını UTF-8 ile yakalıyoruz ve gerçek zamanlı olarak okuyoruz

                    var outputTask = Task.Run(() =>
                    {
                        using (StreamReader reader = new StreamReader(process.StandardOutput.BaseStream, System.Text.Encoding.UTF8))
                        {
                            while (!reader.EndOfStream)
                            {
                                string output = reader.ReadLine();
                                Invoke(new Action(() =>
                                {
                                    label.Text += output + "\n\n";
                                    Console.WriteLine(output);  // Konsola yazdırma
                                                                // Burada isterseniz progress bar'ı veya başka bir UI elementini güncelleyebilirsiniz
                                }));
                            }
                        }
                    });

                    // Hata çıktılarını asenkron olarak okuyalım
                    var errorTask = Task.Run(() =>
                    {
                        using (StreamReader reader = new StreamReader(process.StandardError.BaseStream, Encoding.UTF8))
                        {
                            while (!reader.EndOfStream)
                            {
                                string error = reader.ReadLine();
                                Invoke(new Action(() =>
                                {
                                    Console.WriteLine($"Hata: {error}");
                                }));
                            }
                        }
                    });

                    // Python script'inin tamamlanmasını bekleyelim
                    await Task.WhenAll(outputTask, errorTask);  // Her iki görevi de bekliyoruz

                    process.WaitForExit();

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }

            //progressBar1.Visible = false;

            form.Close();

            MessageBox.Show("İşlem tamamlandı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
             
        }

        public async Task PythonScriptCalistir2(string pythonPath, string inputPath)
        {
            // Geçici Form oluştur
            Form form = new Form
            {
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(400, 300),
                Text = "İşlem devam ediyor...",
                TopMost = true
            };

            // ProgressBar oluştur
            ProgressBar progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                Size = new Size(200, 20),
                Location = new Point((form.ClientSize.Width - 200) / 2, 20)
            };

            // RichTextBox: Canlı çıktı görüntüleme
            RichTextBox rtb = new RichTextBox
            {
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Location = new Point(10, progressBar.Bottom + 10),
                Size = new Size(form.ClientSize.Width - 20, form.ClientSize.Height - 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Form'a ekle
            form.Controls.Add(progressBar);
            form.Controls.Add(rtb);
            form.Show();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python", // python.exe tam yolu gerekiyorsa: @"C:\Python39\python.exe"
                    Arguments = $"\"{pythonPath}\" \"{inputPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

                process.Start();

                var outputTask = Task.Run(async () =>
                {
                    var reader = process.StandardOutput;
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        rtb.Invoke(new MethodInvoker(() =>
                        {
                            rtb.AppendText(line + Environment.NewLine);
                        }));
                    }
                });

                var errorTask = Task.Run(async () =>
                {
                    var reader = process.StandardError;
                    while (!reader.EndOfStream)
                    {
                        string error = await reader.ReadLineAsync();
                        rtb.Invoke(new MethodInvoker(() =>
                        {
                            rtb.AppendText("[HATA] " + error + Environment.NewLine);
                        }));

                    }
                });

                await Task.WhenAll(outputTask, errorTask);

                await Task.Run(() =>
                {
                    process.WaitForExit();
                });


                form.Close(); // İşlem formunu kapat
                MessageBox.Show("Python scripti başarıyla tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var sonuc = MessageBox.Show("Veriler sisteme yüklensin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (sonuc == DialogResult.Yes)
                {
                    MessageBox.Show("Veriler sisteme yüklenecek.", "Bilgi");
                }
            }
            catch (Exception ex)
            {
                form.Close(); // Hata olsa bile formu kapat
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetPythonPath()
        {
            string systemPathVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            

            // Sistem genelindeki PATH çevresel değişkenini alıyoruz
            // Eğer çevresel değişken mevcutsa
            if (!string.IsNullOrEmpty(systemPathVariable))
            {
                // PATH değişkenini ';' ile böldük
                var entries = systemPathVariable.Split(';');

                // Her bir PATH girişi ekrana yazdırılır
                foreach (string path in entries)
                {
                    // Eğer yol 'AppData\Local\Programs\Python' içeriğine sahipse, bu doğru Python yolu olmalı
                    if (path.Contains(@"AppData\Local\Programs\Python"))
                    {
                        string pythonPath = path;
                        return pythonPath + @"\python.exe";
                    }
                }
            }
            else
            {
                Console.WriteLine("Sistem PATH çevresel değişkeni bulunamadı.");
            }

            return null;
        }

        public PictureBox ShowLoadingGifNextToButton(Button button, Panel targetPanel, string gifPath)
        {
            PictureBox loadingGif = new PictureBox();

            int targetHeight = button.Height;
            int targetWidth = targetHeight;

            loadingGif.Size = new Size(targetWidth, targetHeight);
            loadingGif.SizeMode = PictureBoxSizeMode.Zoom;

            loadingGif.Image = Image.FromFile(gifPath);

            loadingGif.Location = new Point(
                button.Right + 5,
                button.Top + (button.Height - loadingGif.Height) / 2
            );

            targetPanel.Controls.Add(loadingGif);
            loadingGif.BringToFront();
            targetPanel.Refresh();

            return loadingGif; // sonradan kaldırmak için referans döndür
        }

        static void Arsivleme(string arananDesen)
        {
            string kaynakKlasor = @"C:\Dosyalar\Gelen";
            string hedefKlasor = @"C:\Dosyalar\Yedek";
            //string arananDesen = "rapor"; // Dosya adında aranacak ifade

            if (!Directory.Exists(hedefKlasor))
            {
                Directory.CreateDirectory(hedefKlasor);
            }

            string[] dosyalar = Directory.GetFiles(kaynakKlasor);

            foreach (string dosya in dosyalar)
            {
                string dosyaAdi = Path.GetFileName(dosya);

                if (dosyaAdi.IndexOf(arananDesen, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string hedefYol = Path.Combine(hedefKlasor, dosyaAdi);
                    File.Copy(dosya, hedefYol, overwrite: true);
                    Console.WriteLine($"Kopyalandı: {dosyaAdi}");
                }
            }

            Console.WriteLine("İşlem tamamlandı.");
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

public class FiltreEventArgs : EventArgs
{
    public DataTable dt { get; set; }
    
}

public interface IDrawable
{
    void DrawMap(); // veya Task DrawAsync(); da olabilir
}
