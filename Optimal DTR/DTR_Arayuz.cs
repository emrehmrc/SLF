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
using static Trafo;
using SLF.Services;
using System.Diagnostics;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Control = System.Windows.Forms.Control;
using Microsoft.Win32;


namespace SLF.Optimal_DTR
{
    public partial class DTR_Arayuz : Form
    {

        public static string PythonPath;

        public static string PythonFilePath;

        string TuketimDosyaAdi;
        string TrafoDosyaAdi;
        string TrafoAlanDosyaAdi;

        string tuketim_path;

        GMapOverlay overlay;

        private bool isSelecting = false;

        private int highResFactor = 3; // Yüksek çözünürlük katsayısı

        public DTR_Arayuz()
        {
            InitializeComponent();
            InitializeMap();
            ToolTipKismi();
           

            MessageBox.Show("Veriler Sisteme Yüklesin mi?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (DialogResult == DialogResult.Yes)
            {
                VerilerSisteme();
            }
            else
            {
                MessageBox.Show("Veriler sisteme yüklenmedi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private bool CalismaYoluKontrol()
        {
            TuketimDosyaAdi = "Tuketim.xlsx";
            TrafoDosyaAdi = "Trafo.xlsx";
            TrafoAlanDosyaAdi = "TrafoAlan.xlsx";

            //string path = PathService.CurrentWorkingFolder;
            //string imar_path = PathService.ImarAnaliziPath;

            string imar_path = @"C:\Users\vural.bayrakli\source\repos\SLF\bin\Debug\il_ilce_kırılımları\İzmir\Aliağa\proje\imar";

            tuketim_path = Path.Combine(imar_path, TuketimDosyaAdi);
            string trafo_path = Path.Combine(imar_path, TrafoDosyaAdi);
            string trafo_alan_path = Path.Combine(imar_path, TrafoAlanDosyaAdi);

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

            Console.WriteLine(systemPathVariable);

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
                        return pythonPath+@"\python.exe"; 
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

        // Tüketim verileri var mı yok mu sorgulama
        private async void TuketimVeriSorgulama()
        {

            await PythonScriptCalistir(TuketimDosyaAdi);
   
        }

        private async Task VerilerSisteme()
        {       
            progressBar1.Location = new Point((gMapControl1.Width - gMapControl1.Width) / 2, 50);  // 50px uzaklıkta yerleştiriyoruz
            //progressBar1.Show();

            ReadDB.Hucreverioku("veriler.db", "Hucre");
            ReadDB.Maindatabase("veriler.db", "Trafo2603");           

            await Task.Run(() => DrawMap(ReadDB.trafoData, ReadDB.HucreData));

            MessageBox.Show("Veriler sisteme yüklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private async Task PythonScriptCalistir(string inputpath)
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
                    Arguments = $"{PythonFilePath} \"{inputpath}\"",
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

            MessageBox.Show("Veriler Sisteme Yüklesin mi?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (DialogResult == DialogResult.Yes)
            {
                VerilerSisteme();
            }

            else
            {

                MessageBox.Show("Veriler sisteme yüklenmedi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void InitializeMap()
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

        }

        private void DrawMap(List<Trafo> trafoData, List<Hucre> hucreData)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            foreach (var hucre in hucreData)
            {
                // O hücredeki tüm trafoları al
                var trafolar = trafoData.Where(t => t.HucreID == hucre.HucreId).ToList();

                if (trafolar.Count == 0)
                    continue;

                // Hücre sınırları
                double top = hucre.Top;
                double bottom = hucre.Bottom;
                double left = hucre.Left;
                double right = hucre.Right;

                // Trafo sayısına bağlı olarak farklı yerleşimler yap
                List<PointLatLng> markerKonumlari = new List<PointLatLng>();

                if (trafolar.Count == 1)
                {
                    // Tek trafo varsa merkeze koy
                    markerKonumlari.Add(new PointLatLng((top + bottom) / 2, (left + right) / 2));
                }
                else if (trafolar.Count <= 4)
                {
                    // 2-4 trafo varsa köşeleri kullan
                    markerKonumlari.Add(new PointLatLng(top, left));    // Sol üst köşe
                    markerKonumlari.Add(new PointLatLng(top, right));   // Sağ üst köşe
                    markerKonumlari.Add(new PointLatLng(bottom, left)); // Sol alt köşe
                    markerKonumlari.Add(new PointLatLng(bottom, right));// Sağ alt köşe
                }
                else
                {
                    // 4'ten fazla trafo varsa, içeri doğru grid oluştur (3x3 gibi)
                    int satirSayisi = (int)Math.Ceiling(Math.Sqrt(trafolar.Count)); // Kaç satır olmalı
                    int sutunSayisi = (int)Math.Ceiling((double)trafolar.Count / satirSayisi); // Kaç sütun olmalı

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

                // Trafoları belirlenen konumlara ekle
                for (int i = 0; i < trafolar.Count; i++)
                {
                    var trafo = trafolar[i];
                    PointLatLng konum = markerKonumlari[i % markerKonumlari.Count]; // Fazla olursa döngüye girer

                    GMarkerGoogleType markerType = trafo.Owner == "Özel" ? GMarkerGoogleType.red_dot : GMarkerGoogleType.blue_dot;

                    var marker = new GMarkerGoogle(konum, markerType)
                    {
                        ToolTipText = $"TrafoID: {trafo.TrafoID}\nOwner: {trafo.Owner}\nYear: {trafo.Year}\nTrafo Durumu: {trafo.Durumu}",
                        Tag = trafo.TrafoID,
                    };

                    overlay.Markers.Add(marker);
                }
            }
            //gMapControl1.MouseClick += GMapControl1_MouseClick;

            gMapControl1.OnMarkerClick += (s, e) =>
            {

            };
            gMapControl1.Overlays.Add(overlay);
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


            

            DrawMap(filtrelenmisTrafolar, ReadDB.HucreData);

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

        private async void button2_Click(object sender, EventArgs e)
        {
            //İslem();
            PythonFilePath = Path.Combine(@"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\SuperHucreAlgoritma12.py");

            string pythonPath = GetPythonPath();

            PythonPath = Path.Combine(pythonPath);

            bool islemeDevam = CalismaYoluKontrol();

            if (PythonPath == null)
            {
                MessageBox.Show("Python yolu bulunamadı. Lütfen Python yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                if(islemeDevam)
                {
                    await PythonScriptCalistir(tuketim_path);
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
