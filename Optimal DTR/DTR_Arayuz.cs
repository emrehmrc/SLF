using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using Newtonsoft.Json;
using System.IO;
using SLF.Services;
using System.Diagnostics;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Control = System.Windows.Forms.Control;
using Microsoft.Win32;
using Font = System.Drawing.Font;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Task = System.Threading.Tasks.Task;
using OfficeOpenXml;
using System.Web;


namespace SLF.Optimal_DTR
{
    public partial class DTR_Arayuz : Form
    {
        ExcelImporter excelImporter = new ExcelImporter();

        public static string PythonPath;

        public static string PythonFilePath;

        string TuketimDosyaAdi;
        string TrafoDosyaAdi;
        string TrafoAlanDosyaAdi;

        string tuketim_path;

        GMapOverlay overlay;

        private bool isSelecting = false;

        private int highResFactor = 3; // Yüksek çözünürlük katsayısı

        string İlİlceYol;

        string ODTRAlgoritmaYolu;

        string İmarYolu;

        string YükTahminVeriYolu;

        string TrafoVeriYolu;

        string TrafoAlanlarıVeriYolu;

        string SonucYolu;

        string ODTRSonucYolu;


        string İlYol;
        string ProgramDosyalarıYolu;

        string ProjeYolu;

        string YükVeriYolu;

        string İmarVeriYolu;
        string ODTRJson;

        DataTable trafodt;
        YearService yearService;

        string il;

        string ilce;

        string İlkYıl;

        string SonYıl;

        public DTR_Arayuz()
        {
            InitializeComponent();
            InitializeMap();
            ToolTipKismi();

            // Config dosyası için ayarları tanımla



            /*var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("config.json")
               .Build();*/

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
         
            il = config["İl"];

            ilce = config["İlçe"];

            İlkYıl = config["DEK:baslangıc_yılı"];

            SonYıl = config["DEK:bitis_yılı"];          

            Dictionary<string, PointLatLng> cityCoordinates = new Dictionary<string, PointLatLng>
                {
                    { "İzmir", new PointLatLng(38.4192, 27.1287) }, // Example coordinates for İzmir
                    { "Eskişehir", new PointLatLng(39.7768, 30.5206) } // Example coordinates for Eskişehir
                    // Add more cities and their coordinates as needed
                };
                  
            //VerilerSisteme2();
            /*İlİlceYol = Path.Combine(config["Ana_Klasör_Yolu"], config["İl"], config["İlçe"]);

            ODTRAlgoritmaYolu = Path.Combine(config["SLF_Yolu"], config["Python Kodları:DTR_Algoritması"]);

            ProjeYolu = Path.Combine(İlİlceYol, config["Proje_Yolu"]);

            İmarYolu = Path.Combine(ProjeYolu, "imar");

            YükTahminVeriYolu = Path.Combine(İmarYolu, "Tuketim.xlsx");

            TrafoVeriYolu = Path.Combine(İmarYolu, "Trafo.xlsx");

            TrafoAlanlarıVeriYolu = Path.Combine(İmarYolu, "TrafoAlan.xlsx");

            sonucYolu = Path.Combine(ProjeYolu, "sonuçlar");

            ODTRSonucYolu = Path.Combine(sonucYolu, "ODTR");*/

            //VerilerSisteme();

            /*
            PythonPath = Path.Combine(@"C:\Users\vural.bayrakli\AppData\Local\Programs\Python\Python311\python.exe");
            * ReadDB.Hucreverioku();
            //ReadDB.Maindatabase();

            //DrawMap(ReadDB.trafoData, ReadDB.HucreData);


            // Sadece burada, kullanıcı onayladığında PathService'i güncelle ve klasör oluştur
            //PathService.UpdatePath(selectedCity, selectedDistrict);

            */

        }

        public void DrawMap2(DataTable trafoTable)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            foreach (DataRow trafo in trafoTable.Rows)
            {
                int hucreId = Convert.ToInt32(trafo["merkez_hucre"]);

                // Bu hücreye ait trafoları filtrele
                var trafolar = trafoTable.AsEnumerable()
                    .Where(t => Convert.ToInt32(t["merkez_hucre"]) == hucreId)
                    .ToList();

                if (trafolar.Count == 0)
                    continue;

                // Hücre sınırları
                double top = Convert.ToDouble(trafo["Top"]);
                double bottom = Convert.ToDouble(trafo["Bottom"]);
                double left = Convert.ToDouble(trafo["Left"]);
                double right = Convert.ToDouble(trafo["Right"]);

                // Trafo konumlarını tutacak liste
                List<PointLatLng> markerKonumlari = new List<PointLatLng>();

                if (trafolar.Count == 1)
                {
                    markerKonumlari.Add(new PointLatLng((top + bottom) / 2, (left + right) / 2));
                }
                else if (trafolar.Count <= 4)
                {
                    markerKonumlari.Add(new PointLatLng(top, left));
                    markerKonumlari.Add(new PointLatLng(top, right));
                    markerKonumlari.Add(new PointLatLng(bottom, left));
                    markerKonumlari.Add(new PointLatLng(bottom, right));
                }
                else
                {
                    int satirSayisi = (int)Math.Ceiling(Math.Sqrt(trafolar.Count));
                    int sutunSayisi = (int)Math.Ceiling((double)trafolar.Count / satirSayisi);

                    double latStep = (top - bottom) / (satirSayisi + 1);
                    double lngStep = (right - left) / (sutunSayisi + 1);

                    for (int i = 1; i <= satirSayisi; i++)
                    {
                        for (int j = 1; j <= sutunSayisi; j++)
                        {
                            if (markerKonumlari.Count >= trafolar.Count)
                                break;

                            double lat = bottom + (i * latStep);
                            double lng = left + (j * lngStep);
                            markerKonumlari.Add(new PointLatLng(lat, lng));
                        }
                    }
                }

                for (int i = 0; i < trafolar.Count; i++)
                {
                    var t = trafolar[i];
                    PointLatLng konum = markerKonumlari[i % markerKonumlari.Count];

                    string owner = trafo["sahip"].ToString();
                    GMarkerGoogleType markerType = owner == "Özel" ? GMarkerGoogleType.red_dot : GMarkerGoogleType.blue_dot;

                    string tooltip = $"TrafoID: {t["trafo_id"]}\n" +
                                     $"Owner: {t["sahip"]}\n" +
                                     $"Year: {t["year"]}\n" +
                                     $"Trafo Durumu: {t["Durum"]}";

                    var marker = new GMarkerGoogle(konum, markerType)
                    {
                        ToolTipText = tooltip,
                        Tag = trafo["trafo_id"]
                    };

                    overlay.Markers.Add(marker);
                }
            }

            gMapControl1.Overlays.Add(overlay);
        }

        public void DrawMap3(DataTable trafoTable)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            double unique_x;
            double unique_y;
            foreach (DataRow trafo in trafoTable.Rows)
            {

                try
                {
                    unique_x = Convert.ToDouble(trafo["Koord_x"]);
                    unique_y = Convert.ToDouble(trafo["Koord_y"]);
                }

                catch
                {
                    MessageBox.Show("Koordinat bilgisi bulunamadı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Unique X ve Y koordinatları (DataTable'dan alınıyor)
                

                // Eğer unique koordinatlar varsa, onları kullan
                PointLatLng konum = new PointLatLng(unique_y, unique_x);  // Unique X ve Y'yi buraya ekliyoruz

                string owner = trafo["Trafo Mülkiyeti"].ToString();
                GMarkerGoogleType markerType = owner == "Özel" ? GMarkerGoogleType.red_dot : GMarkerGoogleType.blue_dot;

                string tooltip = $"TrafoID: {trafo["trafo_id"]}\n" +
                                    $"Owner: {trafo["Trafo Mülkiyeti"]}\n" +
                                    $"Year: {trafo["year"]}\n" +
                                    $"Trafo Durumu: {trafo["Trafo Aksiyon"]}";

                var marker = new GMarkerGoogle(konum, markerType)
                {
                    ToolTipText = tooltip,
                    Tag = trafo["trafo_id"]
                };

                overlay.Markers.Add(marker);

            }

            gMapControl1.Overlays.Add(overlay);
            gMapControl1.Refresh(); // Haritayı güncelle
        }



        private bool CalismaYoluKontrol()
        {
            TuketimDosyaAdi = "SONUCLAR2.xlsx";
            TrafoDosyaAdi = $"trafo_merkez_hucre_{il}_{ilce}.xlsx";
            TrafoAlanDosyaAdi = $"trafo_rezerv_alanlar_{il.ToLower()}.xlsx";

            //string path = PathService.CurrentWorkingFolder;
            //string imar_path = PathService.ImarAnaliziPath; // 
            //string imar_path = @"C:\Users\vural.bayrakli\source\repos\SLF\bin\Debug\il_ilce_kırılımları\İzmir\Aliağa\proje\imar";
            

            tuketim_path = Path.Combine(YükVeriYolu, TuketimDosyaAdi);
            string trafo_path = Path.Combine(İmarVeriYolu, TrafoDosyaAdi);
            string trafo_alan_path = Path.Combine(İmarVeriYolu, TrafoAlanDosyaAdi);

            if (!File.Exists(tuketim_path) & !File.Exists(trafo_path) & !File.Exists(trafo_alan_path))
            {
                MessageBox.Show("Optimal DTR için gerekli veriler bulunamadı. Lütfen verileri yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                MessageBox.Show("Optimal DTR için gerekli veriler bulundu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }



        }

        private bool DosyaKontrol(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // ToolTip fonksiyonu
        private void SetToolTip(Control control, string message)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(control, message); // Kontrol için mesajı ayarlıyoruz
        }

        private void ToolTipKismi()
        {
            SetToolTip(panel2, "Bu kısımda trafonun kurum ya da özel olma durumuna göre filtreleme yapabilirsiniz.");

            SetToolTip(panel4, "Bu kısımda yıllara göre filtreleme yapabilirsiniz.");

            SetToolTip(panel7, "Bu kısımda trafonun durumuna göre filtreleme yapabilirsiniz.");

            SetToolTip(button1, "Bu butona tıklayarak filtreleme işlemini başlatabilirsiniz");

            SetToolTip(button2, "Bu butona tıklayarak Optimal DTR algoritmasını çalıştırabilirsiniz. İşlem 3-5 dk sürer.");


        }


        public string GetPythonPath()
        {
            string systemPathVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            string python_location = null;

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

        private void İslem()
        {
            // Kaynak dosya yolunu belirleyin (örneğin, var olan bir dosya)
            string sourceFilePath = @"C:\Users\vural.bayrakli\Downloads\output_abone_dagilimi_1303_4.xlsx";

            // Python dosyasının bulunduğu dizini alın
            string PythonFileP = Path.Combine(PathService._baseDirectory);

            // Base dizini bir DirectoryInfo nesnesine dönüştür
            DirectoryInfo baseDirInfo = new DirectoryInfo(PythonFileP);

            // Parent (üst) dizini alıyoruz
            DirectoryInfo parentDirInfo = baseDirInfo.Parent.Parent.Parent;

            // Parent dizini null değilse, Python dosyasının tam yolunu oluşturuyoruz
            if (parentDirInfo != null)
            {
                //PythonFilePath = Path.Combine(parentDirInfo.FullName, @"PythonFiles\SuperHucreAlgoritma12.py");

                PythonFilePath = Path.Combine(@"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\SuperHucreAlgoritma12.py");

                PythonPath = Path.Combine(@"C:\Users\vural.bayrakli\AppData\Local\Programs\Python\Python311\python.exe");

                // PythonFilePath'i yazdırıyoruz
                Console.WriteLine(PythonFilePath);
            }
            else
            {
                Console.WriteLine("Parent directory not found.");
            }

            // Hedef dosya yolunu oluşturun
            string destinationFilePath = Path.Combine(PathService.ImarAnaliziPath, "Tuketim.xlsx");

            try
            {
                // Dosyayı kopyalayın
                File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                Console.WriteLine($"Dosya başarıyla kopyalandı: {destinationFilePath}");
            }
            catch (Exception ex)
            {
                // Hata yakalama
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }

        // Tüketim verileri var mı yok mu sorgulam     

        private async Task VerilerSisteme()
        {


            ReadDB.Hucreverioku("veriler.db", "Hucre");
            ReadDB.Maindatabase("veriler.db", "Trafo2603");

            //await Task.Run(() => DrawMap(ReadDB.trafoData, ReadDB.HucreData));

            MessageBox.Show("Veriler sisteme yüklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private async Task VerilerSisteme2()
        {
            Console.WriteLine("Veriler sisteme yükleniyor...");

            //string dosya = Path.Combine(ODTRSonucYolu, "trafo.xlsx");
            string dosya = @"C:\Users\vural.bayrakli\source\repos\SLF\bin\Debug\il_ilce_kırılımları\İzmir\Program Dosyaları\Optimal DTR\dftrafo_hucre20250420_163943.xlsx";
            trafodt = ImportExcelFile(dosya);

            //MessageBox.Show(trafodt.Rows.Count.ToString());  

            //await Task.Run(() => DrawMap3(trafodt));
            DrawMap3(trafodt);
            //MessageBox.Show("Veriler sisteme yüklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                    Arguments = $"\"{python_path}\" \"{inputpath}\"",
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
                        using (StreamReader reader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8))
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

            /*MessageBox.Show("Veriler Sisteme Yüklesin mi?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);


            MessageBox.Show("Veriler sisteme yüklenecek", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await VerilerSisteme2();*/




        }

        private void InitializeMap()
        {

            overlay = new GMapOverlay("map");

            gMapControl1.Dock = DockStyle.None;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 12;

            gMapControl1.Position = new PointLatLng(38.5, 27.0); // Başlangıç konumu
           
            gMapControl1.DragButton = MouseButtons.Left;

        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Seçili yılları listeye al
            List<int> secilenYillar = new List<int>();
            List<string> secilenDurumlar = new List<string>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                if (item.ToString() != "Hepsi")
                {
                    secilenYillar.Add(int.Parse(item.ToString()));
                }

            }

            foreach (var item in checkedListBox2.CheckedItems)
            {
                secilenDurumlar.Add(item.ToString());
            }

            Console.WriteLine(string.Join(", ", secilenYillar));
            Console.WriteLine(string.Join(", ", secilenDurumlar));

            // Trafo Aksiyonları eşleşmesi için sözlük oluştur
            var aksiyonEslestirme = new Dictionary<string, string>
            {
                { "Mevcut", "mevcut" },
                { "Trafo Yükseltme-Yükten", "trafo yükseltme-yükten" },
                { "Yeni Trafo Tesis", "yeni trafo tesis" }, // "Eklenen" -> "yeni trafo tesis"
                { "Trafo Yenileme-Yaştan", "trafo yenileme-yaştan" },
                { "Trafo Yenileme-Kapasiteden", "trafo yükseltme-kapasiteden" },

            };



            // Seçilen aksiyonu al
            List<string> secilenAksiyonlar = checkedListBox2.CheckedItems.Cast<string>().ToList();

            // Eğer "Hepsi" seçilmediyse, aksiyonları eşleştir
            List<string> eslesenAksiyonlar = new List<string>();

            foreach (var aksiyon in secilenAksiyonlar)
            {
                if (aksiyonEslestirme.ContainsKey(aksiyon))
                {
                    eslesenAksiyonlar.Add(aksiyonEslestirme[aksiyon]);
                }
            }

            // Filtreleme işlemi
            var radiobuttonvalue = GetSelectedRadioButton(panel2) ?? "Hepsi";  // Default to "Hepsi" if null

            var filtrelenmisData = trafodt.AsEnumerable()
                .Where(row =>
                    (secilenYillar.Count == 0 ||
                    (int.TryParse(row.Field<string>("year"), out int year) && secilenYillar.Contains(year))) && // Yıla göre filtreleme
                    (radiobuttonvalue == "Hepsi" || row.Field<string>("Trafo Mülkiyeti") == radiobuttonvalue) && // Sahiplik filtreleme
                    (eslesenAksiyonlar.Count == 0 || eslesenAksiyonlar.Contains(row.Field<string>("Trafo Aksiyon"))) // Trafo Aksiyonları filtreleme
                )
                .ToList();

            DataTable filtrelenmis = filtrelenmisData.Any() ? filtrelenmisData.CopyToDataTable() : trafodt.Clone();

            // Filtrelenmiş verileri haritada göster
            DrawMap3(filtrelenmis);
        }
        private void button1__Click(object sender, EventArgs e)
        {

            // Seçili yılları listeye al
            List<int> secilenYillar = new List<int>();
            List<string> secilenDurumlar = new List<string>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                secilenYillar.Add(int.Parse(item.ToString()));
            }

            foreach (var item in checkedListBox2.CheckedItems)
            {
                secilenDurumlar.Add(item.ToString());
            }

            Console.WriteLine(string.Join(", ", secilenYillar));

            Console.WriteLine(string.Join(", ", secilenDurumlar));


            var radiobuttonvalue = GetSelectedRadioButton(panel2) ?? "Hepsi";  // Default to "Hepsi" if null

            // Filtreleme işlemi
            var filtrelenmisTrafolar = ReadDB.trafoData
                .Where(t => secilenYillar.Contains(t.Year))  // Yıla göre filtreleme
                .Where(t => radiobuttonvalue == "Hepsi" || t.Owner == radiobuttonvalue)  // Sahiplik filtreleme
                .Where(t => secilenDurumlar.Contains("Hepsi") || secilenDurumlar.Contains(t.Durumu)) // Trafo Durumu filtreleme
                .ToList();




            //DrawMap(filtrelenmisTrafolar, ReadDB.HucreData);

        }

        private string GetSelectedRadioButton(Panel panel)
        {
            foreach (Control control in panel.Controls) // panel1 yerine kendi panel adını yaz
            {
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    return radioButton.Text; // Seçili RadioButton'un metnini döndür
                }
            }
            return "Hiçbiri seçili değil";
        }

        private void SaveTransformersAsGeoJSON()
        {
            List<object> features = new List<object>();

            foreach (var marker in overlay.Markers)
            {
                string[] tooltipParts = marker.ToolTipText.Split('\n');

                var properties = new
                {
                    TrafoID = tooltipParts[0].Replace("TrafoID: ", ""),
                    Owner = tooltipParts[1].Replace("Owner: ", ""),
                    Year = tooltipParts[2].Replace("Year: ", ""),
                    Durum = tooltipParts[3].Replace("Trafo Durumu: ", "")
                };

                var point = new
                {
                    type = "Feature",
                    geometry = new
                    {
                        type = "Point",
                        coordinates = new double[] { marker.Position.Lng, marker.Position.Lat }
                    },
                    properties = properties // Tüm trafo bilgileri burada saklanıyor
                };

                features.Add(point);
            }

            var geoJson = new
            {
                type = "FeatureCollection",
                features = features
            };

            string json = JsonConvert.SerializeObject(geoJson, Formatting.Indented);
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Trafos.geojson");

            File.WriteAllText(filePath, json, Encoding.UTF8);

            MessageBox.Show($"GeoJSON dosyası kaydedildi:\n{filePath}\nQGIS'te açabilirsiniz.",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            SaveTransformersAsGeoJSON();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void ODTRconfig()
        {
            var configODTR = new
            {
                FilePaths = new
                {

                    İlYol = İlYol,

                    İlİlceYol = İlİlceYol,

                    SonucYolu = SonucYolu,

                    PythonFilePath = PythonFilePath,

                    YükVeriYolu = YükVeriYolu,

                    İmarVeriYolu = İmarVeriYolu,

                },

                Degiskenler = new
                {
                    İl = il,
                    İlçe = ilce,
                    İlkYıl = İlkYıl,
                    SonYıl = SonYıl,
                    TuketimDosyaAdi = TuketimDosyaAdi,
                    TrafoDosyaAdi = TrafoDosyaAdi,
                    TrafoAlanDosyaAdi = TrafoAlanDosyaAdi,

                }
            };

            ODTRJson = Path.Combine(PythonFilePath, "ODTR.json");
            // JSON formatında serileştirme
            string json = JsonConvert.SerializeObject(configODTR, Formatting.Indented);

            // JSON dosyasını yazma
            File.WriteAllText(ODTRJson, json);
        }

        public async void button2_Click(object sender, EventArgs e)
        {


            //PythonFilePath = Path.Combine(@"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\SuperHucreAlgoritma12.py");

            string pythonPath = GetPythonPath();

            PythonPath = Path.Combine(pythonPath);

            bool islemeDevam = CalismaYoluKontrol();

            string python_path = Path.Combine(PythonFilePath, "algoritmaÇalıştır.py");
            //bool islemeDevam = true;

            if (PythonPath == null)
            {
                MessageBox.Show("Python yolu bulunamadı. Lütfen Python yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                if (islemeDevam)
                {
                    await Task.Run(() => ODTRconfig());

                    //await PythonScriptCalistir(tuketim_path);
                    await PythonScriptCalistir(python_path, ODTRJson);
                }
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await VerilerSisteme();
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }


        private void DTR_Arayuz_Load(object sender, EventArgs e)
        {
            
        }

        private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = sender as CheckedListBox;
            if (checkedListBox == null)
                return;


            // Eğer "Hepsi" (index 0) işaretleniyorsa
            if (e.Index == 0)
            {
                // Hepsi işaretleniyorsa tümünü işaretle
                bool check = (e.NewValue == CheckState.Checked);

                // İşlemi event tamamlandıktan sonra yapmamız gerekiyor
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    for (int i = 1; i < checkedListBox.Items.Count; i++)
                    {
                        checkedListBox.SetItemChecked(i, check);
                    }
                }));
            }
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
        private void button3_Click_1(object sender, EventArgs e)
        {
            string trafo_path = DosyaSeciciGoster(SonucYolu);
            try
            {
                trafo_path = Path.Combine(SonucYolu, trafo_path);
                trafodt = ImportExcelFile(trafo_path);

            }

            catch (Exception ex)
            {
                MessageBox.Show("Dosya Seçilmedi.");
                return;
            }

            if (DialogResult.Yes == MessageBox.Show("Veri sisteme yüklensin mi?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                // Verileri sisteme yükle
                DrawMap3(trafodt);
            }
            else
            {
                MessageBox.Show("Veri yüklenmedi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

           
    }

    }




    public class Trafo
    {
        public string TrafoID { get; set; }
        public int HucreID { get; set; }

        public double BosKapasite { get; set; }
        public string Owner { get; set; }

        public int Year { get; set; }

        public string Durumu { get; set; }



        public Trafo(string trafoID, int hucreID, string owner, int year, string durum)
        {
            TrafoID = trafoID;
            HucreID = hucreID;
            Owner = owner;
            Year = year;
            Durumu = durum;
        }



        public class Hucre
        {
            public int HucreId { get; set; }
            public double Left { get; set; }
            public double Top { get; set; }
            public double Right { get; set; }
            public double Bottom { get; set; }

            public Hucre(int id, double left, double top, double right, double bottom)
            {
                HucreId = id;
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

        }

    }

}
