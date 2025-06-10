using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SLF.Services;

namespace SLF
{
    public partial class LoginForm : Form
    {
        private string configPath;
        private JObject configJson;

        public LoginForm()
        {
            InitializeComponent();

            // Şifre alanını maskele
            textBoxPassword.PasswordChar = '*';

            // Config dosyasının yolunu al
            string userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            configPath = Path.Combine(userRootPath,
                "MRC\\MRC - 1.1.3_T&SI\\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\\il_ilce_kır" +
                "ılımları\\Program Dosyaları\\config.json").Replace("/", "\\");

            // Config dosyasını yükle
            LoadConfigFile();

            // Kaydedilmiş kullanıcı adı ve şifreyi yükle
            LoadSavedCredentials();
        }

        private void LoadConfigFile()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    configJson = JObject.Parse(jsonContent);
                }
                else
                {
                    MessageBox.Show("Config dosyası bulunamadı: " + configPath, "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config dosyası yüklenirken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSavedCredentials()
        {
            try
            {
                if (configJson != null)
                {
                    // Config'den kullanıcı adını yükle
                    if (configJson["Veritabanı"] != null && configJson["Veritabanı"]["Username"] != null)
                    {
                        textBoxUsername.Text = configJson["Veritabanı"]["Username"].ToString();
                    }

                    // Beni hatırla durumunu ve şifreyi yükle
                    if (configJson["Veritabanı"] != null && configJson["Veritabanı"]["RememberMe"] != null
                        && configJson["Veritabanı"]["RememberMe"].ToString().ToLower() == "true")
                    {
                        checkBoxRememberMe.Checked = true;

                        // Kaydedilmiş şifreyi yükle
                        if (configJson["Veritabanı"]["Password"] != null)
                        {
                            textBoxPassword.Text = configJson["Veritabanı"]["Password"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Sadece loglama yap, kullanıcıya mesaj gösterme
                Console.WriteLine("Kullanıcı bilgileri yüklenirken hata: " + ex.Message);
            }
        }

        private void SaveCredentials()
        {
            try
            {
                if (configJson != null)
                {
                    // Kullanıcı adını her zaman kaydet
                    configJson["Veritabanı"]["Username"] = textBoxUsername.Text;

                    // RememberMe durumunu kaydet
                    configJson["Veritabanı"]["RememberMe"] = checkBoxRememberMe.Checked.ToString().ToLower();

                    // Eğer "Beni Hatırla" seçili ise şifreyi de kaydet
                    if (checkBoxRememberMe.Checked)
                    {
                        configJson["Veritabanı"]["Password"] = textBoxPassword.Text;
                    }
                    else
                    {
                        // Şifre kaydını temizle (ama diğer ayarları koru)
                        configJson["Veritabanı"]["Password"] = "";
                    }

                    // Değişiklikleri kaydet
                    File.WriteAllText(configPath, configJson.ToString(Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı bilgileri kaydedilirken hata: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            // Gerekli alanların doldurulup doldurulmadığını kontrol et
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre alanlarını doldurun!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (configJson == null)
                {
                    MessageBox.Show("Config dosyası yüklenemedi!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Config'den server, port ve database bilgilerini al
                string host = configJson["Veritabanı"]["Host"].ToString();
                int port = int.Parse(configJson["Veritabanı"]["Port"].ToString());
                string serviceName = configJson["Veritabanı"]["Service_Name"].ToString();

                // Oracle için bağlantı cümlesi oluştur
                string connString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})));User Id={username};Password={password};";

                // DatabaseManager'ı başlat
                DatabaseManager.GetInstance(connString).GetConnection();

                SaveCredentials();

                MessageBox.Show("Bağlantı başarılı!\n\n Lütfen bu pencereyi kapattıktan sonra bekleyiniz," +
                    " veritabanında ilgili SAP Abone tüketimleri tablosu (DWH_MRC_SLFPROJE_TUKETIM) ve " +
                    "Abone bilgi tablosunun olup olmadığı (DWH_MRC_SLFPROJE_ABN_BLG) kontrol edilecek.", 
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Giriş başarılı, formu kapat ve ana uygulamayı devam ettir
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}