using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SLF.services;
using SLF.Services;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SLF
{
    public partial class DatabaseListForm : Form
    {
        public string csv_file_path_1;
        public string csv_file_path_2;

        public HomePageForm anaMenuObjesi;

        public DatabaseListForm()
        {
            InitializeComponent();
            anaMenuObjesi = new HomePageForm();

            // Load event'ini manuel olarak bağla
            this.Load += DatabaseListForm_Load;
        }

        private void DatabaseListForm_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde veri doğrulaması yap
            //CheckDataValidation();
        }

        /// <summary>
        /// Veritabanı verilerinin güncelliğini kontrol eder
        /// </summary>
        private void CheckDataValidation()
        {
            try
            {
                // Veritabanı bağlantısı var mı kontrol et
                var dbManager = DatabaseManager.GetInstance();
                if (!dbManager.HasConnectionString() || !dbManager.IsConnected())
                {
                    MessageBox.Show("Veritabanı bağlantısı bulunamadı. Lütfen önce veritabanına bağlanın.",
                                  "Bağlantı Hatası",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Config dosyası yolunu al
                string configPath = PathService._configKonum;

                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    MessageBox.Show("Config dosyası bulunamadı.",
                                  "Config Hatası",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Veri doğrulaması yap
                var validationResult = DataValidationService.ValidateAboneData(configPath);

                // Sonucu kullanıcıya göster
                MessageBoxIcon icon = validationResult.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
                string title = validationResult.IsValid ? "Veri Durumu - Güncel" : "Veri Durumu - Güncelleme Gerekli";

                MessageBox.Show(validationResult.Message, title, MessageBoxButtons.OK, icon);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri doğrulaması sırasında hata oluştu: {ex.Message}",
                              "Hata",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void buttonAboneVerisiOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Config yolunu al
                string configPath = PathService._configKonum;

                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    MessageBox.Show("Config dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // CSV dosya yollarını ayarla
                csv_file_path_1 = Path.Combine(anaMenuObjesi.userRootPath,
                    (string)anaMenuObjesi.config.Ana_Klasör_Yolu,
                    (string)anaMenuObjesi.config.depo,
                    "DWH_MRC_SLFPROJE_TUKETIM.csv");

                csv_file_path_2 = Path.Combine(anaMenuObjesi.userRootPath,
                    (string)anaMenuObjesi.config.Ana_Klasör_Yolu,
                    (string)anaMenuObjesi.config.depo,
                    "DWH_MRC_SLFPROJE_ABN_BLG.csv");

                // İlerleme formu göster (opsiyonel)
                var progressForm = new Form()
                {
                    Text = "Abone Verisi Oluşturuluyor...",
                    Size = new Size(400, 100),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                var progressLabel = new Label()
                {
                    Text = "Veri işleniyor...",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                progressForm.Controls.Add(progressLabel);

                // Async olarak işlemi çalıştır
                Task.Run(() =>
                {
                    try
                    {
                        buttonAboneVerisiOlustur.ForeColor = Color.Silver;
                        System.Threading.Thread.Sleep(250);
                        buttonAboneVerisiOlustur.ForeColor = Color.White;

                        string processingMode;
                        if (File.Exists(csv_file_path_1) && File.Exists(csv_file_path_2))
                        {
                            // CSV dosyaları varsa, doğrudan ProcessAboneDataFromCsv'yi çalıştır
                            Console.WriteLine("CSV dosyaları mevcut. CSV'lerden abone verisi oluşturuluyor...");
                            PythonHelper.ProcessAboneDataFromCsv(configPath, Path.GetDirectoryName(csv_file_path_1));
                            processingMode = "Hızlı (CSV)";
                        }
                        else
                        {
                            // CSV dosyaları yoksa, veritabanından çek
                            Console.WriteLine("CSV dosyaları yok. Veritabanından yeni veri çekiliyor...");
                            PythonHelper.RunPythonScriptForAboneVerisi(configPath); // Fallback to database mode
                            processingMode = "Tam güncelleme";
                        }

                        // UI thread'de sonucu göster
                        this.BeginInvoke(new Action(() =>
                        {
                            progressForm.Close();
                            MessageBox.Show(
                                $"✅ Abone verisi başarıyla oluşturuldu!\n\nİşlem türü: {processingMode}",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }));
                    }
                    catch (Exception ex)
                    {
                        // UI thread'de hatayı göster
                        this.BeginInvoke(new Action(() =>
                        {
                            progressForm.Close();
                            MessageBox.Show(
                                $"❌ Abone verisi oluşturulurken hata oluştu:\n\n{ex.Message}",
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }));
                    }
                });

                // İlerleme formunu göster
                progressForm.ShowDialog();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tablo seçim formu (Değiştirilmedi, aynı kalabilir)
        public class SelectTableForm : Form
        {
            private ComboBox comboBoxTables;
            private Button buttonSelect;
            private Button buttonCancel;

            public string SelectedTable { get; private set; }

            public SelectTableForm(DataTable tables)
            {
                InitializeComponent();

                // Tabloları combo box'a ekle
                foreach (DataRow row in tables.Rows)
                {
                    comboBoxTables.Items.Add(row[0].ToString());
                }

                if (comboBoxTables.Items.Count > 0)
                {
                    comboBoxTables.SelectedIndex = 0;
                }
            }

            private void InitializeComponent()
            {
                this.comboBoxTables = new System.Windows.Forms.ComboBox();
                this.buttonSelect = new System.Windows.Forms.Button();
                this.buttonCancel = new System.Windows.Forms.Button();
                this.SuspendLayout();
                // 
                // comboBoxTables
                // 
                this.comboBoxTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.comboBoxTables.FormattingEnabled = true;
                this.comboBoxTables.Location = new System.Drawing.Point(12, 20);
                this.comboBoxTables.Name = "comboBoxTables";
                this.comboBoxTables.Size = new System.Drawing.Size(360, 21);
                this.comboBoxTables.TabIndex = 0;
                // 
                // buttonSelect
                // 
                this.buttonSelect.Location = new System.Drawing.Point(216, 60);
                this.buttonSelect.Name = "buttonSelect";
                this.buttonSelect.Size = new System.Drawing.Size(75, 23);
                this.buttonSelect.TabIndex = 1;
                this.buttonSelect.Text = "Seç";
                this.buttonSelect.UseVisualStyleBackColor = true;
                this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
                // 
                // buttonCancel
                // 
                this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.buttonCancel.Location = new System.Drawing.Point(297, 60);
                this.buttonCancel.Name = "buttonCancel";
                this.buttonCancel.Size = new System.Drawing.Size(75, 23);
                this.buttonCancel.TabIndex = 2;
                this.buttonCancel.Text = "İptal";
                this.buttonCancel.UseVisualStyleBackColor = true;
                // 
                // SelectTableForm
                // 
                this.AcceptButton = this.buttonSelect;
                this.CancelButton = this.buttonCancel;
                this.ClientSize = new System.Drawing.Size(384, 91);
                this.Controls.Add(this.buttonCancel);
                this.Controls.Add(this.buttonSelect);
                this.Controls.Add(this.comboBoxTables);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "SelectTableForm";
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                this.Text = "Tablo Seçin";
                this.ResumeLayout(false);
            }

            private void buttonSelect_Click(object sender, EventArgs e)
            {
                if (comboBoxTables.SelectedItem != null)
                {
                    SelectedTable = comboBoxTables.SelectedItem.ToString();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Lütfen bir tablo seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonDTRVerileriniOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // Config yolunu al
                string configPath = PathService._configKonum;

                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    MessageBox.Show("Config dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Construct the path to the python script
                string dtr_kodu_path = Path.Combine(anaMenuObjesi.userRootPath,
                    (string)anaMenuObjesi.config.Ana_Klasör_Yolu,
                    (string)anaMenuObjesi.config.program_dosyaları_path,
                    "database/kod/dtr_kodu.py").Replace('/', '\\');

                // Async olarak Python script'ini çalıştır
                Task.Run(() =>
                {
                    buttonDTRVerileriniOlustur.ForeColor = Color.Silver;
                    System.Threading.Thread.Sleep(250);
                    buttonDTRVerileriniOlustur.ForeColor = Color.White;

                    // Run the Python script with output and error capturing
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C python \"{dtr_kodu_path}\" \"{anaMenuObjesi.config_path}\"",
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            UseShellExecute = false,
                            CreateNoWindow = false
                        }
                    };

                    process.Start();
                    process.WaitForExit();

                    // UI thread'de sonucu göster
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(
                            $"✅ DTR verileri, OSOS tüketim verisini ve CBS OGAGTRF katmanını kullanarak başarıyla oluşturuldu!\n",
                            "Başarılı",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                    }));

                });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}