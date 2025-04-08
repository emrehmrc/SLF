using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data;
using SLF.Services;
using SLF.services;

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
                UploadOutputToGridView();
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

        private void RunImarPlanPython(string kmlFilePath, string csvFilePath = null)
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

                // Başarılı çalıştırma mesajı
                MessageBox.Show("İmar planı analizi başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İmar planı analizi çalıştırılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        private void UploadOutputToGridView()
        {
            try
            {
                // Çıktı klasörünü belirle
                string outputDir = PathService.GetImarAnaliziPathForType("imar_planlari");
                string selectedCity = PathService.SelectedCity;
                string selectedDistrict = PathService.SelectedDistrict;
                string outputPrefix = $"imar_plan_{selectedCity}_{selectedDistrict}";
                string outputCsvPath = Path.Combine(outputDir, $"{outputPrefix}.csv");

                if (!File.Exists(outputCsvPath))
                {
                    MessageBox.Show("Çıktı CSV dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // CSV dosyasını DataTable'a yükle
                var dataTable = new DataTable();
                using (var reader = new StreamReader(outputCsvPath))
                {
                    string headerLine = reader.ReadLine();
                    if (headerLine != null)
                    {
                        string[] headers = headerLine.Split(',');
                        foreach (string header in headers)
                        {
                            dataTable.Columns.Add(header.Trim('\"'));
                        }

                        while (!reader.EndOfStream)
                        {
                            string dataLine = reader.ReadLine();
                            if (dataLine != null)
                            {
                                string[] dataValues = dataLine.Split(',');
                                for (int i = 0; i < dataValues.Length; i++)
                                {
                                    dataValues[i] = dataValues[i].Trim('\"');
                                }
                                dataTable.Rows.Add(dataValues);
                            }
                        }
                    }
                }

                _dataGridViewGirdi.DataSource = dataTable;

                // Sütun genişliklerini ayarla
                _dataGridViewGirdi.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                // Başlık renklendirme
                for (int i = 0; i < _dataGridViewGirdi.Columns.Count; i++)
                {
                    _dataGridViewGirdi.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.LightBlue;
                    _dataGridViewGirdi.Columns[i].HeaderCell.Style.ForeColor = System.Drawing.Color.Navy;
                    _dataGridViewGirdi.Columns[i].HeaderCell.Style.Font = new System.Drawing.Font(_dataGridViewGirdi.Font, System.Drawing.FontStyle.Bold);
                }

                // Satır sayısı bilgisi
                MessageBox.Show($"Toplam {dataTable.Rows.Count} adet kayıt yüklendi.", "Veri Yüklendi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSV verileri yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
       