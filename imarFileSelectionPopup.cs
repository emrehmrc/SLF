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

        public imarFileSelectionPopup(DataGridView dataGridViewGirdi)
        {
            InitializeComponent();
            _dataGridViewGirdi = dataGridViewGirdi;

            // Radio butonları gizleyelim veya kaldıralım çünkü artık kullanılmayacak
            if (imarizmirRadioButton != null) imarizmirRadioButton.Visible = false;
            if (imarEskisehirRadioButton != null) imarEskisehirRadioButton.Visible = false;

            // Dosya yolu etiketlerini oluştur
            CreateFilePathLabels();

            // Seçili il/ilçe bilgilerini gösterelim
            UpdateCityDistrictLabel();
        }

        private void UpdateCityDistrictLabel()
        {
            // Bilgi için bir label ekleyelim ve seçili il/ilçeyi gösterelim
            if (cityInfoLabel == null)
            {
                cityInfoLabel = new Label();
                cityInfoLabel.AutoSize = true;
                cityInfoLabel.Location = new System.Drawing.Point(12, 20);
                cityInfoLabel.Name = "cityInfoLabel";
                this.Controls.Add(cityInfoLabel);
            }

            // Eğer il/ilçe seçilmemişse uyarı göster
            if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
            {
                cityInfoLabel.Text = "Lütfen önce il/ilçe seçin!";
                cityInfoLabel.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                cityInfoLabel.Text = $"Seçili Bölge: {PathService.SelectedCity} / {PathService.SelectedDistrict}";
                cityInfoLabel.ForeColor = System.Drawing.Color.Black;
            }
        }
        private void CreateFilePathLabels()
        {
            // CSV dosya adı etiketi
            csvFilePathLabel = new Label();
            csvFilePathLabel.AutoSize = true;
            csvFilePathLabel.Location = new System.Drawing.Point(12, 60); // SelectCsvButton'un altına
            csvFilePathLabel.Name = "csvFilePathLabel";
            csvFilePathLabel.Text = "CSV dosyası seçilmedi";
            this.Controls.Add(csvFilePathLabel);

            // KML dosya adı etiketi
            kmlFilePathLabel = new Label();
            kmlFilePathLabel.AutoSize = true;
            kmlFilePathLabel.Location = new System.Drawing.Point(12, 100); // SelectKmlButton'un altına
            kmlFilePathLabel.Name = "kmlFilePathLabel";
            kmlFilePathLabel.Text = "KML dosyası seçilmedi";
            this.Controls.Add(kmlFilePathLabel);
        }
        private void imarFileSelectionPanel_Paint(object sender, PaintEventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CsvFilePath = openFileDialog.FileName;
                    csvFilePathLabel.Text = Path.GetFileName(CsvFilePath);
                }
            }
        }

        private void SelectKmlButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "KML Files (*.kml)|*.kml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    KmlFilePath = openFileDialog.FileName;
                    kmlFilePathLabel.Text = Path.GetFileName(KmlFilePath);
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // PathService'te il ve ilçe bilgileri olup olmadığını kontrol et
                if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
                {
                    MessageBox.Show("Lütfen önce il ve ilçe seçimi yapın.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (imarMethodSelectionComboBox.SelectedIndex == 0)
                {
                    // Sadece KML dosyası gerekli
                    if (string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen bir KML dosyası seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Dosyayı imar_plans/data klasörüne kopyala
                    string kmlDestinationPath = PathService.CopyKmlToImarPlansData(KmlFilePath);

                    // PythonHelper.RunImarPlanModel'i çağır
                    RunImarPlanPython(kmlDestinationPath);
                }
                else if (imarMethodSelectionComboBox.SelectedIndex == 1)
                {
                    // KML ve CSV dosyaları birlikte gerekli
                    if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Dosyaları imar_plans/data klasörüne kopyala
                    string kmlDestinationPath = PathService.CopyKmlToImarPlansData(KmlFilePath);
                    string csvDestinationPath = PathService.CopyCsvToImarPlansData(CsvFilePath);

                    // PythonHelper.RunImarPlanModel'i çağır
                    RunImarPlanPython(kmlDestinationPath, csvDestinationPath);
                }
                else
                {
                    MessageBox.Show("Lütfen bir yöntem seçin.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Populate the DataGridView after successful script execution
                //UploadOutputToGridView();
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
                string output = PythonHelper.RunImarPlanModel(kmlFilePath, csvFilePath);

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

                            // "İmar Analizi" modülünü ekle (eğer zaten yoksa)
                            if (!completedModules.Contains("İmar Analizi"))
                            {
                                completedModules.Add("İmar Analizi");
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
        private string GetOutputFileInfo(string outputDir)
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine("Çıktı klasörü içeriği:");

            // Ana klasördeki CSV'leri kontrol et
            string[] csvFiles = Directory.GetFiles(outputDir, "*.csv", SearchOption.TopDirectoryOnly);
            if (csvFiles.Length > 0)
            {
                info.AppendLine("Ana klasördeki CSV dosyaları:");
                foreach (var file in csvFiles)
                {
                    info.AppendLine($"- {Path.GetFileName(file)}");
                }
            }

            // csv alt klasörünü kontrol et
            string csvOutputDir = Path.Combine(outputDir, "csv");
            if (Directory.Exists(csvOutputDir))
            {
                string[] csvSubFiles = Directory.GetFiles(csvOutputDir, "*.csv", SearchOption.TopDirectoryOnly);
                if (csvSubFiles.Length > 0)
                {
                    info.AppendLine("CSV alt klasöründeki dosyalar:");
                    foreach (var file in csvSubFiles)
                    {
                        info.AppendLine($"- {Path.GetFileName(file)}");
                    }
                }
            }

            // Alt klasörleri kontrol et
            string[] subDirs = Directory.GetDirectories(outputDir);
            if (subDirs.Length > 0)
            {
                info.AppendLine("Alt klasörler:");
                foreach (var dir in subDirs)
                {
                    info.AppendLine($"- {Path.GetFileName(dir)}");
                }
            }

            return info.ToString();
        }
       
        }
    }
