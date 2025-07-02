using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data;
using SLF.Services;
using SLF.services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;


namespace SLF
{
    public partial class imarFileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }

        private DataGridView _dataGridViewGirdi;
        private Label cityInfoLabel;
        private Label csvFilePathLabel; // Label değişkenini ekleyin
        private Label kmlFilePathLabel; // Label değişkenini ekleyin

        private HomePageForm anaMenuObjesi;

        public imarFileSelectionPopup(DataGridView dataGridViewGirdi)
        {
            InitializeComponent();
            anaMenuObjesi = new HomePageForm();
            _dataGridViewGirdi = dataGridViewGirdi;

        }



        private async void SelectKmlButton_Click(object sender, EventArgs e)
        {
            await Task.Delay(250);
            this.Cursor = Cursors.WaitCursor;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "KML Files (*.kml)|*.kml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    KmlFilePath = openFileDialog.FileName;
                    string filename = KmlFilePath.Substring(KmlFilePath.LastIndexOf("\\") + 1);

                    label_imar_path.Text = filename;
                    label_imar_path.Visible= true;
                }
            }

            this.Cursor= Cursors.Default;
        }

        // Updated RunPythonScriptAsync to match your paths
        private async Task RunPythonSEgrisiScriptAsync()
        {

            try
            {
                string pythonScriptPath = Path.Combine(anaMenuObjesi.userRootPath,
                    (string)anaMenuObjesi.config.Ana_Klasör_Yolu,
                    (string)anaMenuObjesi.config.program_dosyaları_path,
                    "SLF\\S_Eğrisi_çıkarım.py");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // Specify cmd.exe as the executable
                    Arguments = $"/C python \"{pythonScriptPath}\" \"{anaMenuObjesi.config_path}\"", // Pass arguments correctly
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = true, // Use true to show the window
                    CreateNoWindow = false // Ensure the command window is visible
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    await Task.Run(() => process.WaitForExit());


                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python scripti {process.ExitCode} çıkış koduyla başarısız oldu.");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Python kodu çalıştırma hatası: {ex.Message}");
            }
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                OkButton.ForeColor = Color.LimeGreen;
                OkButton.Refresh(); // Force UI update
                await Task.Delay(300); // Non-blocking delay for 1 second
                OkButton.ForeColor = Color.White;
                OkButton.Refresh(); // Force UI update

                Cursor.Current = Cursors.WaitCursor;

                // PathService'te il ve ilçe bilgileri olup olmadığını kontrol et
                if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
                {
                    MessageBox.Show("Lütfen önce il ve ilçe seçimi yapın.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Sadece KML dosyası gerekli
                if (string.IsNullOrEmpty(KmlFilePath))
                {
                    MessageBox.Show("Lütfen üstteki 'İmar Verisi Seç' bölümünden bir İmar dosyası (.kml) dosyası " +
                        "seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // PythonHelper.RunImarPlanModel'i çağır
                RunImarPlanPython(KmlFilePath);

                await RunPythonSEgrisiScriptAsync();


                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool RunImarPlanPython(string kmlFilePath, string csvFilePath = null)
        {
            try
            {
                // İşlem başlıyor bildirimi
                string message = csvFilePath == null
                    ? "KML dosyası işleniyor ve Overpass verileri çekiliyor..."
                    : "KML ve CSV dosyaları işleniyor...";

                MessageBox.Show(message, "İşlem Başlıyor", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Python betiğini çalıştır
                PythonHelper.RunImarPlanModel(kmlFilePath, csvFilePath);

                // Çıktı klasörünü kontrol et
                string outputDir = PathService.GetImarAnaliziPathForType("imar_planlari");
                string csvOutputDir = Path.Combine(outputDir, "csv");
                string selectedCity = PathService.SelectedCity;
                string selectedDistrict = PathService.SelectedDistrict;

                // Herhangi bir çıktı dosyasını kontrol et
                bool outputExists = Directory.Exists(outputDir) &&
                   (Directory.Exists(csvOutputDir) && Directory.GetFiles(csvOutputDir, "*.csv").Length > 0 ||
                    Directory.GetFiles(outputDir, "*.csv").Length > 0 ||
                    Directory.GetFiles(outputDir, "*.kml").Length > 0 ||
                    Directory.GetDirectories(outputDir).Length > 0);

                if (outputExists)
                {
                    // Proje durumunu güncelle (project_state.json)
                    if (PathService.CurrentMode == PathService.WorkingMode.Project &&
                        !string.IsNullOrEmpty(PathService.CurrentWorkingFolder))
                    {
                        try
                        {
                            // Proje dosyasının yolu
                            string statePath = Path.Combine(
                                PathService.BaseDirectory,
                                PathService.FullPath,
                                PathService.CurrentWorkingFolder,
                                "project_state.json");

                            // JSON dosyası için veri oluştur
                            Dictionary<string, object> projectState;

                            // Eğer dosya varsa, mevcut içeriği oku
                            if (File.Exists(statePath))
                            {
                                string json = File.ReadAllText(statePath);
                                projectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ??
                                               new Dictionary<string, object>();
                            }
                            else
                            {
                                projectState = new Dictionary<string, object>();
                            }

                            // Tamamlanan modüller listesini al veya oluştur
                            List<string> completedModules;
                            if (projectState.TryGetValue("CompletedModules", out object modulesObj))
                            {
                                // Mevcut liste varsa dönüştür
                                try
                                {
                                    completedModules = JsonConvert.DeserializeObject<List<string>>(modulesObj.ToString()) ??
                                                      new List<string>();
                                }
                                catch
                                {
                                    completedModules = new List<string>();
                                }
                            }
                            else
                            {
                                completedModules = new List<string>();
                            }

                            // "İmar Verileri" modülünü ekle (eğer zaten yoksa)
                            if (!completedModules.Contains("İmar Verileri"))
                            {
                                completedModules.Add("İmar Verileri");
                            }

                            // Saturasyon klasörünü kontrol et
                            string saturasyonPath = Path.Combine(outputDir, "saturasyon");
                            if (Directory.Exists(saturasyonPath) && !completedModules.Contains("Saturasyon Analizi"))
                            {
                                completedModules.Add("Saturasyon Analizi");
                            }

                            // Güncellenmiş listeyi dictionary'ye ekle
                            projectState["CompletedModules"] = completedModules;

                            // Diğer bilgileri güncelle
                            projectState["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            // Proje adı yoksa ekle
                            if (!projectState.ContainsKey("ProjectName") &&
                                PathService.CurrentWorkingFolder.StartsWith("proje_"))
                            {
                                projectState["ProjectName"] = PathService.CurrentWorkingFolder.Substring(6);
                            }

                            // YearService'ten yıl bilgilerini al
                            var yearService = YearService.GetInstance();
                            projectState["SLFStartYear"] = yearService.slfStartYear;
                            projectState["SLFEndYear"] = yearService.slfEndYear;

                            // Güncellenen json'ı dosyaya yaz
                            string updatedJson = JsonConvert.SerializeObject(projectState, Formatting.Indented);
                            using (StreamWriter writer = new StreamWriter(statePath, false, new UTF8Encoding(false)))
                            {
                                writer.Write(updatedJson);
                            }

                            Console.WriteLine($"Proje durumu güncellendi: {statePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Proje durumu güncellenirken hata: {ex.Message}");
                            // Hata durumunda işleme devam et
                        }
                    }

                    // Başarılı çalıştırma mesajı
                    MessageBox.Show(
                        "İmar planı analizi başarıyla tamamlandı." +
                        (PathService.CurrentMode == PathService.WorkingMode.Project ?
                            "\nProje durumu güncellendi." : ""),
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return true;
                }
                else
                {
                    // Çıktı dosyası bulunamadıysa uyarı mesajı göster
                    MessageBox.Show(
                        "İmar planı analizi tamamlandı ancak çıktı dosyası bulunamadı.",
                        "Uyarı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İmar planı analizi çalıştırılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;  // Hata durumunda false döndür
            }
        }

        private async void TestCalıstır_Click(object sender, EventArgs e)
        {
            try
            {
                TestCalıstır.ForeColor = Color.LimeGreen;
                TestCalıstır.Refresh(); // Force UI update
                await Task.Delay(300); // Non-blocking delay for 1 second
                TestCalıstır.ForeColor = Color.White;
                TestCalıstır.Refresh(); // Force UI update


                Cursor.Current = Cursors.WaitCursor;

                // Check if KML file is selected
                if (string.IsNullOrEmpty(KmlFilePath))
                {
                    MessageBox.Show("Lütfen bir KML dosyası seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // For CSV, use the katman_eslesme.csv
                string csvFilePath = PathService.KatmanEslestirmePath;
                if (!File.Exists(csvFilePath))
                {
                    MessageBox.Show("Katman eşleştirme CSV dosyası bulunamadı.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Run the Katman Deneme script
                string outputFile = PythonHelper.RunKatmanDeneme(KmlFilePath, csvFilePath);

                // Display success message
                MessageBox.Show("Katman analizi başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Display the results in a new form
                if (File.Exists(outputFile))
                {
                    ShowAnalysisResults(outputFile);
                }
                else
                {
                    MessageBox.Show("Analiz sonuç dosyası bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Katman analizi çalıştırılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ShowAnalysisResults(string csvFilePath)
        {
            try
            {
                // Load the CSV into a DataTable
                DataTable dt = LoadCsvToDataTable(csvFilePath);
                if (dt == null || dt.Columns.Count == 0)
                {
                    MessageBox.Show("Analiz sonuçları okunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ana sonuçlar formu
                Form resultsForm = new Form();
                resultsForm.Text = "Katman Analizi Sonuçları";
                resultsForm.Size = new System.Drawing.Size(800, 600);
                resultsForm.StartPosition = FormStartPosition.Manual;
                // Ana formu sol tarafa yerleştir
                resultsForm.Location = new Point(20, 20);

                // DataGridView oluştur
                DataGridView dataGridView = new DataGridView();
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.ReadOnly = true;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.DataSource = dt;

                // Kontrolleri forma ekle
                resultsForm.Controls.Add(dataGridView);

                // Katman eşleştirme tablonuzu yükleyin
                string layerMappingPath = PathService.KatmanEslestirmePath;
                if (!string.IsNullOrEmpty(layerMappingPath) && File.Exists(layerMappingPath))
                {
                    // Katman eşleştirme dosyasını yükle
                    DataTable mappingDt = LoadCsvToDataTable(layerMappingPath);

                    if (mappingDt != null && mappingDt.Columns.Count > 0)
                    {
                        // Katman eşleştirme formu
                        Form mappingForm = new Form();
                        mappingForm.Text = "Katman Eşleştirme Tablosu";
                        mappingForm.Size = new System.Drawing.Size(700, 500);
                        mappingForm.StartPosition = FormStartPosition.Manual;
                        // Katman formunu sağ tarafa yerleştir
                        mappingForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 720, 20);

                        // Panel oluştur (düğmeler için)
                        Panel buttonPanel = new Panel();
                        buttonPanel.Dock = DockStyle.Bottom;
                        buttonPanel.Height = 50;

                        // Kaydet butonu
                        Button saveButton = new Button();
                        saveButton.Text = "Kaydet";
                        saveButton.Size = new System.Drawing.Size(100, 30);
                        saveButton.Location = new Point(buttonPanel.Width - 120, 10);
                        saveButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                        saveButton.Click += (s, args) => {
                            try
                            {
                                // Değişiklikleri CSV dosyasına kaydet
                                SaveDataTableToCsv(mappingDt, layerMappingPath);
                                MessageBox.Show("Katman eşleştirme tablosu başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Katman eşleştirme tablosu kaydedilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        };

                        // Yeni Satır Ekle butonu
                        Button addButton = new Button();
                        addButton.Text = "Yeni Satır Ekle";
                        addButton.Size = new System.Drawing.Size(120, 30);
                        addButton.Location = new Point(10, 10);
                        addButton.Click += (s, args) => {
                            // Yeni satır ekle
                            DataRow newRow = mappingDt.NewRow();
                            // Varsayılan değerler ata (sütunlara göre)
                            if (mappingDt.Columns.Contains("Imar Tipi"))
                                newRow["Imar Tipi"] = "Yeni Tip";
                            if (mappingDt.Columns.Contains("Katman Adlandirma"))
                                newRow["Katman Adlandirma"] = "Yeni Katman";
                            if (mappingDt.Columns.Contains("Poligon Tip"))
                                newRow["Poligon Tip"] = "PL";
                            if (mappingDt.Columns.Contains("Anahtar Kelimeler"))
                                newRow["Anahtar Kelimeler"] = "anahtar1,anahtar2";

                            // Satırı tabloya ekle
                            mappingDt.Rows.Add(newRow);
                        };

                        // Butonları panele ekle
                        buttonPanel.Controls.Add(saveButton);
                        buttonPanel.Controls.Add(addButton);

                        // Katman eşleştirme DataGridView'i
                        DataGridView mappingGridView = new DataGridView();
                        mappingGridView.Dock = DockStyle.Fill;
                        mappingGridView.AllowUserToAddRows = true;
                        mappingGridView.AllowUserToDeleteRows = true;
                        mappingGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        mappingGridView.DataSource = mappingDt;

                        // Formları oluştur
                        mappingForm.Controls.Add(mappingGridView);
                        mappingForm.Controls.Add(buttonPanel);

                        // Ana formun kapanışını takip et ve diğer formu da kapat
                        resultsForm.FormClosed += (s, args) => {
                            if (!mappingForm.IsDisposed && mappingForm.Visible)
                                mappingForm.Close();
                        };

                        // Katman eşleştirme formunu göster (modalsız olarak)
                        mappingForm.Show();
                    }
                }

                // Ana formu göster (modalsız olarak)
                resultsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analiz sonuçları gösterilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void katman_tablosu_Click(object sender, EventArgs e)
        {

        }

        private DataTable LoadCsvToDataTable(string csvPath)
        {
            DataTable dt = new DataTable();

            try
            {
                Debug.WriteLine($"Dosya yolu: {csvPath}");

                // Dosya var mı kontrol et
                if (!File.Exists(csvPath))
                {
                    Debug.WriteLine($"HATA: Dosya bulunamadı: {csvPath}");
                    MessageBox.Show($"CSV dosyası bulunamadı: {csvPath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return dt;
                }

                // Dosyayı oku - UTF8 ile dene
                string[] lines;
                try
                {
                    lines = File.ReadAllLines(csvPath, Encoding.UTF8);
                    Debug.WriteLine($"Dosya UTF8 ile okundu. Satır sayısı: {lines.Length}");
                }
                catch
                {
                    // UTF8 başarısız olursa varsayılan kodlama ile dene
                    lines = File.ReadAllLines(csvPath, Encoding.Default);
                    Debug.WriteLine($"Dosya varsayılan kodlama ile okundu. Satır sayısı: {lines.Length}");
                }

                // Dosya boş mu kontrol et
                if (lines.Length == 0)
                {
                    Debug.WriteLine("Dosya boş.");
                    return dt;
                }

                // Başlık satırını işle
                string headerLine = lines[0];
                string[] headers = headerLine.Split(',');

                // DataTable'a sütunları ekle
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Trim());
                }

                // Veri satırlarını ekle
                for (int i = 1; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                    {
                        string[] values = lines[i].Split(',');
                        DataRow row = dt.NewRow();

                        for (int j = 0; j < values.Length && j < dt.Columns.Count; j++)
                        {
                            row[j] = values[j].Trim();
                        }

                        dt.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV okuma hatası: {ex.Message}, {ex.StackTrace}");
                MessageBox.Show($"CSV okuma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void SaveDataTableToCsv(DataTable dt, string csvPath)
        {
            try
            {
                Debug.WriteLine($"CSV kaydediliyor: {csvPath}");

                // Klasör yolunun varlığını kontrol et
                string directory = Path.GetDirectoryName(csvPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // CSV dosyasını kaydet
                using (StreamWriter sw = new StreamWriter(csvPath, false, Encoding.UTF8))
                {
                    // Başlıkları yaz
                    List<string> headers = new List<string>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        headers.Add(column.ColumnName);
                    }
                    sw.WriteLine(string.Join(",", headers));

                    // Satırları yaz
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            List<string> fields = new List<string>();
                            foreach (var item in row.ItemArray)
                            {
                                string field = item?.ToString() ?? "";

                                // Virgül, tırnak veya yeni satır içeren alanları tırnak içine al
                                if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                                {
                                    field = $"\"{field.Replace("\"", "\"\"")}\"";
                                }

                                fields.Add(field);
                            }

                            sw.WriteLine(string.Join(",", fields));
                        }
                    }
                }

                Debug.WriteLine($"CSV başarıyla kaydedildi: {csvPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV kaydetme hatası: {ex.Message}, {ex.StackTrace}");
                throw; // Hatayı yukarıya fırlat
            }
        }

        private async void KmlTestButton_Click(object sender, EventArgs e)
        {
            await Task.Delay(250);
            this.Cursor = Cursors.WaitCursor;
            this.Refresh();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "KML Files (*.kml)|*.kml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    KmlFilePath = openFileDialog.FileName;
                    string filename = KmlFilePath.Substring(KmlFilePath.LastIndexOf("\\") + 1);

                    label_imar_test.Text = filename;
                    label_imar_test.Visible = true;

                }
            }
            this.Cursor = Cursors.Default;
            this.Refresh();
        }

        private async void label_imar_katman_listeleri_Click(object sender, EventArgs e)
        {
            try
            {
                label_imar_katman_listeleri.ForeColor = Color.LimeGreen;
                label_imar_katman_listeleri.Refresh(); // Force UI update
                await Task.Delay(250); // Non-blocking delay for 1 second
                label_imar_katman_listeleri.ForeColor = Color.Green;
                label_imar_katman_listeleri.Refresh(); // Force UI update

                // Config'den katman_eslesme.csv yolunu al
                string layerMappingPath = PathService.KatmanEslestirmePath;
                Debug.WriteLine($"Config'den alınan yol: {layerMappingPath}");

                // Tüm slash karakterlerini normalize et (önce hepsini \ yap)
                layerMappingPath = layerMappingPath.Replace('/', '\\');

                // Windows tam yoluna dönüştür
                layerMappingPath = Path.GetFullPath(layerMappingPath);
                Debug.WriteLine($"Normalize edilmiş yol: {layerMappingPath}");

                // Dosya yolu doğru mu kontrol et
                if (string.IsNullOrEmpty(layerMappingPath))
                {
                    MessageBox.Show("Katman eşleştirme dosyası yolu bulunamadı.",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Dosyanın var olup olmadığını kontrol et
                if (!File.Exists(layerMappingPath))
                {
                    // Dosya yoksa hata ver ve geri dön
                    MessageBox.Show($"Katman eşleştirme dosyası bulunamadı: {layerMappingPath}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Dosyayı DataTable'a yükle
                DataTable dt = LoadCsvToDataTable(layerMappingPath);

                // DataTable kontrol et
                if (dt == null || dt.Columns.Count == 0)
                {
                    MessageBox.Show("Katman eşleştirme verileri okunamadı.",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Katman listesi formunu oluştur
                Form layerMappingForm = new Form();
                layerMappingForm.Text = "Katman Listesi";
                layerMappingForm.Size = new System.Drawing.Size(800, 500);
                layerMappingForm.StartPosition = FormStartPosition.CenterParent;

                // DataGridView oluştur
                DataGridView dataGridView = new DataGridView();
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.AllowUserToAddRows = true;
                dataGridView.AllowUserToDeleteRows = true;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.DataSource = dt;

                // Kaydet butonu
                Button saveButton = new Button();
                saveButton.Text = "Kaydet";
                saveButton.Size = new System.Drawing.Size(100, 30);
                saveButton.Dock = DockStyle.Bottom;
                saveButton.Click += (s, args) =>
                {
                    try
                    {
                        SaveDataTableToCsv((DataTable)dataGridView.DataSource, layerMappingPath);
                        MessageBox.Show("Katman listesi başarıyla kaydedildi.",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        layerMappingForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaydetme hatası: {ex.Message}",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // Panel ekle
                Panel buttonPanel = new Panel();
                buttonPanel.Height = 40;
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Controls.Add(saveButton);

                // Kontrolleri forma ekle
                layerMappingForm.Controls.Add(dataGridView);
                layerMappingForm.Controls.Add(buttonPanel);

                // Formu göster
                layerMappingForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Katman listesi hatası: {ex.Message}, {ex.StackTrace}");
                MessageBox.Show($"Katman listesi açılırken hata: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
    
