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
using Microsoft.Web.WebView2.WinForms;
using SLF.RaporlamaDosyası;
using DocumentFormat.OpenXml.Wordprocessing;
using SLF.Properties;
using Irony;
using DocumentFormat.OpenXml.Drawing.Charts;
using Size = System.Drawing.Size;
using Formatting = Newtonsoft.Json.Formatting;
using DataTable = System.Data.DataTable;
using System.Threading;

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
        public string Arsiv;
        string ODTRJson;

        DataTable trafodt;
        YearService yearService;

        string il;
        public string ilce;
        public string İlkYıl;
        public string SonYıl;

        public Panel panelWebview;
        private WebView2 webView;
        public Rapor_Arayuz Rapor_Arayuz;

        string Dosyalar;
        string Katsayilar;

        string YukTabloAdi;
        string AlansalYukTabloAdi;
        string HTML;
        public string VeriTabanıYolu;
        string PointLoadYolu;
        public string YukVeriTabanıYolu;
        public string EAYukVeriTabanıYolu;

        HomePageForm anaMenu;

        bool veriSeçildi_mi = false;

        IConfigurationRoot config;

        Dictionary<string, PointLatLng> cityCoordinates;

        public bool ODTR_çalıştı_mı = false;

        public DTR_Arayuz()
        {
            InitializeComponent();
            
            ToolTipKismi();                     

            anaMenu = new HomePageForm();

            var configPath = anaMenu.config_path;

            /*var configPath = Path.Combine("C:\\Users\\vural.bayrakli\\",
                @"OneDrive - MRC\İletişim sitesi - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\Program Dosyaları\configVural.json");
            */
            config = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();
            
            userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            //string Ana_Klasör_Yolu = Path.Combine(userRootPath, config["Ana_Klasör_Yolu"]);         

            //ConfigKismi(config);

            
        }

        public void ConfigKismi(IConfigurationRoot config)
        {
            string Ana_Klasör_Yolu = Path.Combine(userRootPath, config["Ana_Klasör_Yolu"]);

            try
            {
                İlYol = Path.Combine(Ana_Klasör_Yolu, config["İl"]);

                İlİlceYol = Path.Combine(İlYol, config["İlçe"]);
                SonucYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:Sonuçlar_klasör"]);
                PythonFilePath = Path.Combine(Ana_Klasör_Yolu, config["program_dosyaları_path"], config["ODTR:PYTHON_klasör"]);
                YükVeriYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:INPUT_Yük_klasör"]);
                İmarVeriYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["ODTR:INPUT_Trafo_klasör"]);
                Arsiv = Path.Combine(SonucYolu, config["ODTR:Arşiv"]);
                Dosyalar = Path.Combine(PythonFilePath, config["ODTR:Dosyalar"]);
                Katsayilar = Path.Combine(Dosyalar, config["ODTR:Katsayilar"]);
                YukTabloAdi = config["ODTR:YukTabloAdi"];
                AlansalYukTabloAdi = config["ODTR:AlansalYukTabloAdi"];
                HTML = config["ODTR:HTML"];
                VeriTabanıYolu = Path.Combine(Dosyalar, config["ODTR:veriTabaniAdi"]);
                PointLoadYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["SLF:YUK_poligonu"]);
                YukVeriTabanıYolu = Path.Combine(İlİlceYol, config["proje_ismi"], config["Yük_Yoğunluğu:sonuclar_db"]);
                EAYukVeriTabanıYolu = Path.Combine(İlİlceYol, config["EA:ea_klasörü"], config["EA:cikti_dosyasi"]);

                il = config["İl"];
                ilce = config["İlçe"];
                string yilStr = config["baslangıc_yılı"];
                İlkYıl = string.IsNullOrEmpty(yilStr) ? "2025" : yilStr;

                yilStr = config["bitis_yılı"];
                SonYıl = string.IsNullOrEmpty(yilStr) ? "2035" : yilStr;

            }


            catch (Exception ex)
            {
                MessageBox.Show($"Konfigürasyon dosyası okunamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cityCoordinates = new Dictionary<string, PointLatLng>
                {
                    { "İzmir", new PointLatLng(38.5, 27.0) }, // 
                    { "Eskişehir", new PointLatLng(39.7768, 30.5206) },// 
                    { "Manisa", new PointLatLng(38.6191, 27.4289) }

                    // Add more cities and their coordinates as needed
                };

            InitializeMap();
            YillariYerlestir();

        }

        private async void InitBrowser(string path)
        {
            this.gMapControl1.Visible = false; // GMapControl'ü gizle

            if(panelWebview != null)
            {
                panelWebview.Dispose();
            }


            panelWebview = new Panel
            {
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                Location = this.gMapControl1.Location,
                Size = this.gMapControl1.Size,
            };

            this.Controls.Add(panelWebview);

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                
            };

            PictureBox loadingGif = new PictureBox();
            loadingGif.SizeMode = PictureBoxSizeMode.Zoom;
            loadingGif.Image = Properties.Resources.l1;
            //string gifPath = Path.Combine(PythonFilePath, "l1.gif");
            //loadingGif.Image = Image.FromFile(gifPath);
            loadingGif.Location = new Point(
                (panelWebview.Width - loadingGif.Width) / 2,
                (panelWebview.Height - loadingGif.Height) / 2
            );

            panelWebview.Controls.Add(loadingGif);
            panelWebview.Refresh();

            await Task.Delay(1);
            Application.DoEvents();

            panelWebview.Controls.Add(webView);
            await webView.EnsureCoreWebView2Async();
            webView.Source = new Uri(path);  // ✅ Yerel dosya da olabilir

            panelWebview.Controls.Remove(loadingGif);
            loadingGif.Dispose();// Örnek: webView.Source = new Uri(Path.Combine(Application.StartupPath, "sayfa.html"));
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

                    // Eğer unique koordinatlar varsa, onları kullan
                    PointLatLng konum = new PointLatLng(unique_y, unique_x);  // Unique X ve Y'yi buraya ekliyoruz

                    string owner = trafo["Trafo Mülkiyeti"].ToString();
                    GMarkerGoogleType markerType = owner == "Özel" ? GMarkerGoogleType.red_dot : GMarkerGoogleType.blue_dot;

                    string tooltip = $"TrafoID: {trafo["trafo_id"]}\n" +
                                        $"Mülkiyet: {trafo["Trafo Mülkiyeti"]}\n" +
                                        $"İşlem Tarihi: {trafo["İşlem Tarihi"]}\n" +
                                        $"Trafo Aksiyon: {trafo["Trafo Aksiyon"]}\n" +
                                        $"Trafo Kapasite: {trafo["kapasite"]}";

                    var marker = new GMarkerGoogle(konum, markerType)
                    {
                        ToolTipText = tooltip,
                        Tag = trafo["trafo_id"]
                    };

                    overlay.Markers.Add(marker);
                }

                catch
                {
                    
                    
                }                              

            }

            gMapControl1.Overlays.Add(overlay);
            gMapControl1.Refresh(); // Haritayı güncelle
        }


        private bool CalismaYoluKontrol()
        {
            TuketimDosyaAdi = $"SONUCLAR.db";
            TrafoDosyaAdi = $"trafo_merkez_hucre_{ilce}.xlsx";
            TrafoAlanDosyaAdi = $"trafo_rezerv_alanlar_{ilce.ToLower()}.xlsx";

            tuketim_path = Path.Combine(YükVeriYolu, "5.Yük Tahmini\\çıktı", TuketimDosyaAdi);        

            string trafo_path = Path.Combine(İmarVeriYolu, TrafoDosyaAdi);

            string trafo_alan_path = Path.Combine(İmarVeriYolu, TrafoAlanDosyaAdi);

            List<string> eksikDosyalar = new List<string>();

            if (!File.Exists(tuketim_path))
                eksikDosyalar.Add(TuketimDosyaAdi);

            if (!File.Exists(trafo_path))
                eksikDosyalar.Add(TrafoDosyaAdi);

            if (!File.Exists(trafo_alan_path))
                eksikDosyalar.Add(TrafoAlanDosyaAdi);

            if (eksikDosyalar.Any())
            {
                string mesaj = "Optimal DTR için aşağıdaki dosyalar bulunamadı:\n\n" + string.Join("\n", eksikDosyalar);
                MessageBox.Show(mesaj, "Eksik Veri Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                MessageBox.Show("Optimal DTR için gerekli tüm veriler bulundu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            form.BringToFront();
            form.Activate();

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
                    FileName = $"python",
                    Arguments = $"\"{python_path}\" \"{inputpath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                form.FormClosed += (s, e) =>
                {
                    if (!cts.IsCancellationRequested)
                        cts.Cancel();

                    this.button2.Enabled = true;
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    var outputTask = Task.Run(() =>
                    {
                        using (StreamReader reader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8))
                        {
                            while (!reader.EndOfStream && !token.IsCancellationRequested)
                            {
                                string output = reader.ReadLine();

                                if (!form.IsDisposed && label.IsHandleCreated)
                                {
                                    form.Invoke(new Action(() =>
                                    {
                                        if (!label.IsDisposed && label.IsHandleCreated)
                                        {
                                            label.Text += output + "\n\n";
                                        }
                                    }));
                                }
                            }
                        }
                    }, token);

                    var errorTask = Task.Run(() =>
                    {
                        using (StreamReader reader = new StreamReader(process.StandardError.BaseStream, Encoding.UTF8))
                        {
                            while (!reader.EndOfStream && !form.IsDisposed)
                            {
                                string error = reader.ReadLine();

                                if (!form.IsDisposed && form.IsHandleCreated)
                                {
                                    form.Invoke(new Action(() =>
                                    {
                                        if (!form.IsDisposed)
                                        {
                                            Console.WriteLine($"Hata: {error}");
                                        }
                                    }));
                                }
                            }
                        }
                    });

                    await Task.WhenAll(outputTask, errorTask);
                    process.WaitForExit();

                    // ✅ Hata kodu kontrolü eklendi
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show(form, "Python scripti hata ile sonlandı. Algoritma tamamlanmadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(form, "Algoritma başarıyla tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show($"Algoritma çalıştırılırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Hata gerçekleşti. Algoritma tamamlanmadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            form.Close();
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
            //gMapControl1.Position = new PointLatLng(38.5, 27.0); // Başlangıç konumu
            
            try
            {
                gMapControl1.Position = cityCoordinates[il];
            }

            catch (KeyNotFoundException)
            {
                MessageBox.Show($"Şehir koordinatları bulunamadı: {il}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gMapControl1.Position = new PointLatLng(38.5, 27.0); // Varsayılan konum
            }



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
                { "Gerilim Donüşümü", "gerilim dönüşümü"},
                { "Deplase", "deplase" },
                { "Güç Artırımı", "güç artırımı" },
                { "Projelendirilmiş Yeni Trafo", "projelendirilmiş yeni trafo" },
                { "Yeni Trafo Tesis EA", "yeni trafo tesis EA" }, // "Yeni Trafo Tesis EA" -> "yeni trafo tesis ea"
                
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

            var filtrelenmisData = new List<DataRow>();

            if (veriSeçildi_mi == false)
            {
                MessageBox.Show("Lütfen önce verileri yükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                filtrelenmisData = trafodt.AsEnumerable()
                .Where(row =>
                    (secilenYillar.Count == 0 ||
                    (int.TryParse(row.Field<string>("year"), out int year) && secilenYillar.Contains(year))) && // Yıla göre filtreleme
                    (radiobuttonvalue == "Hepsi" || row.Field<string>("Trafo Mülkiyeti") == radiobuttonvalue) && // Sahiplik filtreleme
                    (eslesenAksiyonlar.Count == 0 || eslesenAksiyonlar.Contains(row.Field<string>("Trafo Aksiyon"))) // Trafo Aksiyonları filtreleme
                )
                .ToList();
            }

            catch
            {
                MessageBox.Show("Filtreleme işlemi sırasında hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

            DataTable filtrelenmis = filtrelenmisData.Any() ? filtrelenmisData.CopyToDataTable() : trafodt.Clone();

            // Filtrelenmiş verileri haritada göster
            DrawMap3(filtrelenmis);
            //Filtrele(filtrelenmis);

        }

        private async void Filtrele(DataTable currentDt)
        {
            string filePath = Path.Combine(Rapor_Arayuz.ArsivVeriYolu, "Sonuç.xlsx");

            await Task.Run(() =>
            {
                FormManager.RaporInstance.ExportExcelFile(filePath, currentDt, "excel");
            });

            string python_path = Path.Combine(PythonFilePath, "PydeckRun.py");

            if(File.Exists(filePath))
            {
                MessageBox.Show("Excel dosyası başarıyla oluşturuldu.");
            }
           
            try
            {

                await Rapor_Arayuz.PythonScriptCalistir(python_path, filePath);

                MessageBox.Show("İşlem başarıyla tamamlandı.");

            }


            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }

            string path = Path.Combine(SonucYolu, "hucre_trafo_pydeck.html");
            InitBrowser(path);

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

                    PointLoadYolu = PointLoadYolu,

                    VeriTabanıYolu = VeriTabanıYolu,

                    YükVeriYolu = YükVeriYolu,

                    İmarVeriYolu = İmarVeriYolu,

                    Arsiv = Arsiv,

                    YukVeriTabanıYolu = YukVeriTabanıYolu,

                    EAYukVeriTabanıYolu = EAYukVeriTabanıYolu

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
                    veriTabaniAdi = config["ODTR:veriTabaniAdi"],
                    YukTabloAdi = config["ODTR:YukTabloAdi"],
                    AlansalYukTabloAdi = AlansalYukTabloAdi,
                    HTML = HTML



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
            this.button2.Enabled = false; // Butonu devre dışı bırakıyoruz    

            bool islemeDevam = CalismaYoluKontrol();

            string python_path = Path.Combine(PythonFilePath, "algoritmaÇalıştır.py");
            //bool islemeDevam = true;
                     
            if (islemeDevam)
            {
                try
                {

                    await Task.Run(() => ODTRconfig());

                    //await PythonScriptCalistir(tuketim_path);
                    await PythonScriptCalistir(python_path, ODTRJson);

                    ODTR_çalıştı_mı = true; // örneğin bir bool flag set etmek

                }

                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }

            }
            

            this.button2.Enabled = true; // İşlem tamamlandığında butonu tekrar etkinleştiriyoruz
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
                    if(yol.Contains("Optimal Trafo Yıllık"))
                    {
                        listBox.Items.Add(Path.GetFileName(yol)); // sadece dosya adı
                    }
                    
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
            //trafo_path = Path.Combine(SonucYolu, trafo_path);
            //InitBrowser(trafo_path);
            
            try
            {
                if(trafo_path != null)
                {
                    trafo_path = Path.Combine(SonucYolu, trafo_path);
                    trafodt = ImportExcelFile(trafo_path);
                    veriSeçildi_mi = true; // Veri seçildi mi kontrolü için flag
                }
                
                else
                {
                    MessageBox.Show("Herhangi bir dosya seçilmedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Dosya Seçilmedi.");
                return;
            }

            
            // Verileri sisteme yükle
            var filtrelenmisData = new List<DataRow>();
            try
            {
                filtrelenmisData = trafodt.AsEnumerable()
                .Where(row =>                       
                    (int.TryParse(row.Field<string>("year"), out int year) && year == int.Parse(İlkYıl))                    
                )
                .ToList();
            }

            catch
            {
                MessageBox.Show("Filtreleme işlemi sırasında hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            DataTable filtrelenmis = filtrelenmisData.Any() ? filtrelenmisData.CopyToDataTable() : trafodt.Clone();
            DrawMap3(trafodt);
            //string path = Path.Combine(SonucYolu, "hucre_trafo_pydeck.html");
            //InitBrowser(path);
                
            
            MessageBox.Show("Trafo verileri başarıyla yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


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


        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx";


            /*if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                ExcelPackage package = new ExcelPackage(new FileInfo(path));
                ExcelWorksheet ws = package.Workbook.Worksheets[0];

                DataTable dt = new DataTable();
                foreach (var cell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    dt.Columns.Add(cell.Text);

                for (int row = 2; row <= ws.Dimension.End.Row; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 1; col <= dt.Columns.Count; col++)
                        dr[col - 1] = ws.Cells[row, col].Text;
                    dt.Rows.Add(dr);
                }

                Console.WriteLine(string.Join(" ", dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));
                Form2 parametreFormu = new Form2(path, dt)
                {
                    StartPosition = FormStartPosition.CenterParent,
                    TopMost = true
                };
                parametreFormu.ShowDialog(this);  // <-- Modal ve merkezde


            }*/

            DataTable dt = ImportExcelFile(Katsayilar);

            Form2 parametreFormu = new Form2(Katsayilar, dt)
            {
                StartPosition = FormStartPosition.CenterParent,
                TopMost = true
            };
            parametreFormu.ShowDialog(this);  // <-- Modal ve merkezde




        }

        private void YillariYerlestir()
        {
            this.checkedListBox1.Items.Clear();

            this.checkedListBox1.Items.Add("Hepsi");

            int ilkYilInt = int.Parse(İlkYıl);
            int sonYilInt = int.Parse(SonYıl);

            for (int year = ilkYilInt - 1; year <= sonYilInt; year++)
            {
                this.checkedListBox1.Items.Add(year.ToString());
            }



        }
    }

    public class Form2 : Form
    {
        private string _excelPath;
        private DataTable _dt;
        private TableLayoutPanel _layout;
        private Button _btnKaydet;
        private List<TextBox> _textBoxes = new List<TextBox>();

        public Form2(string excelPath, DataTable dt)
        {
            _excelPath = excelPath;
            _dt = dt;

            try
            {
                InitializeUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUI()
        {
            this.Text = "Parametre Düzenleyici";
            this.Size = new Size(800, 600);
            this.AutoScroll = true;

            // Ana panel: tüm içeriği taşır
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            _layout = new TableLayoutPanel
            {
                ColumnCount = 6,
                AutoSize = true,
                Padding = new Padding(10),
            };

            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));

            // Başlıklar
            _layout.Controls.Add(CreateHeaderLabel("Parametre"), 0, 0);
            _layout.Controls.Add(CreateHeaderLabel("Birim"), 1, 0);
            _layout.Controls.Add(CreateHeaderLabel("Açıklama"), 2, 0);
            _layout.Controls.Add(CreateHeaderLabel("Değer"), 3, 0);
            _layout.Controls.Add(CreateHeaderLabel("Min. DEĞER"), 4, 0);
            _layout.Controls.Add(CreateHeaderLabel("Maks. DEĞER"), 5, 0);

            int rowCount = _dt.Rows.Count;

            for (int i = 0; i < rowCount; i++)
            {

                int rowIndex = i + 1;

                string parametre = _dt.Rows[i]["parametre"]?.ToString() ?? "";
                string birim = _dt.Rows[i]["birim"]?.ToString() ?? "";
                string aciklama = _dt.Rows[i]["açıklama"]?.ToString() ?? "";
                string deger = _dt.Rows[i]["değer"]?.ToString() ?? "";
                string minDeger = _dt.Rows[i]["Min. DEĞER"]?.ToString() ?? "";
                string maxDeger = _dt.Rows[i]["Maks. DEĞER"]?.ToString() ?? "";

                _layout.Controls.Add(CreateContentLabel(parametre), 0, rowIndex);
                _layout.Controls.Add(CreateContentLabel(birim), 1, rowIndex);
                _layout.Controls.Add(CreateContentLabel(aciklama), 2, rowIndex);
                _layout.Controls.Add(CreateContentLabel(minDeger), 4, rowIndex);
                _layout.Controls.Add(CreateContentLabel(maxDeger), 5, rowIndex);

                TextBox txt = new TextBox
                {
                    Text = deger,
                    Width = 150,
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(5)
                };

                txt.Tag = i;  // Satırın indeksini tag ile saklıyoruz

                txt.KeyDown += Txt_KeyDown;

                if ((double.TryParse(deger, out double p2Value)))
                {
                    if((i == 1) | i == 0)
                        txt.Text = p2Value.ToString("P1", System.Globalization.CultureInfo.InvariantCulture); // Yüzdelik format
                }


                if (i == 1)
                {                    

                    txt.ReadOnly = true;
                    txt.BackColor = SystemColors.Control;
                }

                if (i == 0)
                {
                    //txt.TextChanged += Txt_TextChanged; // Döngüyü önlemek için geçici çıkar
                    txt.Leave += Txt_Leave;

                }

                _textBoxes.Add(txt);
                _layout.Controls.Add(txt, 3, rowIndex);
            }



            mainPanel.Controls.Add(_layout);

            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Dock = DockStyle.Fill,
                Height = 40,
                Margin = new Padding(10)
            };
            _btnKaydet.Click += BtnKaydet_Click;

            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };
            bottomPanel.Controls.Add(_btnKaydet);

            this.Controls.Add(mainPanel);
            this.Controls.Add(bottomPanel);
        }


        private Label CreateHeaderLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(5)
        };

        private Label CreateContentLabel(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(5)
        };

        private void Txt_Leave(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox == null) return;

            int rowIndex = (int)txtBox.Tag;

            if (rowIndex == 0) // P1
            {
                string input = txtBox.Text.Replace("%", "").Replace(",", ".");
                if (double.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double p1Value))
                {
                    if (p1Value > 1) p1Value /= 100.0;
                    double p2Value = (1.0 - p1Value) / 4.0;

                    TextBox txtBoxP2 = _textBoxes[1];
                   
                    txtBoxP2.Text = p2Value.ToString("P1", System.Globalization.CultureInfo.InvariantCulture);         
                    
                    txtBox.Text = p1Value.ToString("P1", System.Globalization.CultureInfo.InvariantCulture);
          
                }
            }
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Txt_Leave(sender, EventArgs.Empty); // Enter'a basınca Leave olayını tetikle
                e.SuppressKeyPress = true; // Ding sesi çıkmasın diye
            }
        }

        private void Txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox == null) return;

            int rowIndex = (int)txtBox.Tag;

            if (rowIndex == 0)
            {
                if (double.TryParse(txtBox.Text.Replace("%", "").Replace(",", "."), out double p1Value))
                {

                    // Yüzdelik değer girildiyse %50 -> 0.5'e dönüştür
                    if (p1Value > 1) p1Value = p1Value / 100.0;
                    // Formül: P1 + 4*P2 = 1 => P2 = (1 - P1) / 4
                    double p2Value = (1.0 - p1Value) / 4.0;

                    // P2 kutusunu güncelle
                    
                    TextBox txtBoxP2 = _textBoxes[1];
                    txtBoxP2.TextChanged -= Txt_TextChanged; // Döngüyü önlemek için geçici çıkar
                    txtBoxP2.Text = p2Value.ToString("P1", System.Globalization.CultureInfo.InvariantCulture); // Yüzdelik format
                    txtBoxP2.TextChanged += Txt_TextChanged;

                    TextBox txtBoxP1 = _textBoxes[0];
                    txtBoxP1.TextChanged -= Txt_TextChanged; // Döngüyü önlemek için geçici çıkar
                    txtBoxP1.Text = p1Value.ToString("P1", System.Globalization.CultureInfo.InvariantCulture); // Yüzdelik format
                    txtBoxP1.TextChanged += Txt_TextChanged;


                    // Eğer isterseniz burada txtBoxP2.Enabled = false; diyerek tamamen kilitleyebilirsiniz
                }
            }
        }


        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                _dt.Rows[i]["değer"] = _textBoxes[i].Text;  // ✅ SADECE "değer" kolonu güncelleniyor
            }

            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Sheet2");

            for (int col = 0; col < _dt.Columns.Count; col++)
                ws.Cells[1, col + 1].Value = _dt.Columns[col].ColumnName;

            for (int row = 0; row < _dt.Rows.Count; row++)
                for (int col = 0; col < _dt.Columns.Count; col++)
                    ws.Cells[row + 2, col + 1].Value = _dt.Rows[row][col];

            package.SaveAs(new FileInfo(_excelPath));
            MessageBox.Show("Excel dosyası başarıyla kaydedildi ✔️");
            Close();
        }
    }

    public static class FormManager
    {
        public static DTR_Arayuz Form2Instance { get; set; }
        public static Rapor_Arayuz RaporInstance { get; set; }


        public static void InitializeForms()
        {
            Form2Instance = new DTR_Arayuz();
        }

        public static void InitializeRapor()
        {
            RaporInstance = new Rapor_Arayuz();
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

