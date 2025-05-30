using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using Newtonsoft.Json;
using System.IO;
using SLF.Services;
using System.Diagnostics;
using Control = System.Windows.Forms.Control;
using Font = System.Drawing.Font;
using Microsoft.Extensions.Configuration;
using Task = System.Threading.Tasks.Task;
using OfficeOpenXml;


namespace SLF.Optimal_DTR
{
    public partial class DTR_Arayuz : Form
    {
        CBS cbs;

        public static string PythonPath;
        public static string PythonFilePath;

        public string centerX;
        public string centerY;

        string TuketimDosyaAdi;
        string TrafoDosyaAdi;
        string TrafoAlanDosyaAdi;

        string tuketim_path;

        GMapOverlay overlay;

        public string userRootPath;
        string İlİlceYol;
        string SonucYolu;
        string İlYol;
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

            cbs = new CBS();

            HomePageForm anaMenu = new HomePageForm();

            var configPath = anaMenu.config_path;

            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();

            userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string Ana_Klasör_Yolu = Path.Combine(userRootPath, config["Ana_Klasör_Yolu"]);


            İlYol = Path.Combine(Ana_Klasör_Yolu, config["İl"]);
            İlİlceYol = Path.Combine(İlYol, config["İlçe"]);
            SonucYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:Sonuçlar_klasör"]);
            PythonFilePath = Path.Combine(Ana_Klasör_Yolu, config["program_dosyaları_path"], config["ODTR:PYTHON_klasör"]);
            YükVeriYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:INPUT_Yük_klasör"]);
            İmarVeriYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:INPUT_Trafo_klasör"]);
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

        }

        public GMapControl GetActiveGMapControl()
        {
            foreach (System.Windows.Forms.Control control in this.Controls)
            {
                // If the control is a GMapControl, return it
                if (control is GMapControl gmapControl)
                {
                    return gmapControl;
                }

                // If it's a container, recursively search for a GMapControl inside it
                if (control is Panel panel)
                {
                    GMapControl nestedControl = FindGMapControlInContainer(panel);
                    if (nestedControl != null)
                    {
                        return nestedControl;
                    }
                }
            }

            return null;
        }

        // Helper method to recursively search for GMapControl in nested containers
        private GMapControl FindGMapControlInContainer(System.Windows.Forms.Control container)
        {
            foreach (System.Windows.Forms.Control control in container.Controls)
            {
                if (control is GMapControl gmapControl)
                {
                    return gmapControl;
                }

                // Recursively check if the control is a container (e.g., Panel)
                if (control is Panel panel)
                {
                    GMapControl nestedControl = FindGMapControlInContainer(panel);
                    if (nestedControl != null)
                    {
                        return nestedControl;
                    }
                }
            }
            return null;
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
            TrafoDosyaAdi = $"trafo_merkez_hucre_{ilce}.xlsx";
            TrafoAlanDosyaAdi = $"trafo_rezerv_alanlar_{ilce.ToLower()}.xlsx";

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

            form.Close();
            MessageBox.Show("İşlem tamamlandı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void Arazi_Click(object sender, EventArgs e)
        {
            GetActiveGMapControl().Visible = true;
            GetActiveGMapControl().MapProvider = GMapProviders.GoogleTerrainMap;
        }

        private void Google_Earth_Click(object sender, EventArgs e)
        {
            GetActiveGMapControl().Visible = true;

            Google_Earth google_earth_form = new Google_Earth();
            google_earth_form.Owner = this;
            google_earth_form.Show();
            google_earth_form.BringToFront();
            google_earth_form.Focus();
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl1.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();
        }

        private void Harita_Click(object sender, EventArgs e)
        {
            GetActiveGMapControl().Visible = true;
            GetActiveGMapControl().MapProvider = GMapProviders.GoogleMap;
        }

        private void OSM_Click(object sender, EventArgs e)
        {
            GetActiveGMapControl().Visible = true;
            GetActiveGMapControl().MapProvider = GMapProviders.OpenStreetMap;
        }

        private void Uydu_Click(object sender, EventArgs e)
        {
            GetActiveGMapControl().Visible = true;
            GetActiveGMapControl().MapProvider = GMapProviders.GoogleSatelliteMap;
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
