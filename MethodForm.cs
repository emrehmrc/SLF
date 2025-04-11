using SLF.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace SLF
{
    public partial class MethodForm : Form
    {
        public ModülFormu mod1;

        public string selectedMethod { get; private set; }
        private HomePageForm homePageForm; // Reference to HomePageForm

        public string SelectedPath { get; private set; }

        // İl ve ilçe seçimlerini geçici olarak saklama
        private string tempSelectedCity;
        private string tempSelectedDistrict;

        private void InitializeComboBoxes()
        {
            // Önce ComboBox'ları temizle
            IlComboBox.Items.Clear();
            IlceComboBox.Items.Clear();
            
            // "Lütfen seçin" varsayılan maddelerini ekle
            IlComboBox.Items.Add("Lütfen il seçin");
            IlceComboBox.Items.Add("Lütfen ilçe seçin");

            // Şehirleri ekle
            IlComboBox.Items.Add("İzmir");
            IlComboBox.Items.Add("Eskişehir");

            // Varsayılan olarak "Lütfen seçin" seçeneklerini seç
            IlComboBox.SelectedIndex = 0;
            IlceComboBox.SelectedIndex = 0;
        }

        public MethodForm(HomePageForm homePageForm)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.homePageForm = homePageForm; // Store the reference

            // Set the Enter key to trigger the ForwardButton click event
            this.AcceptButton = ForwardButton;

            // Ensure ForwardButton has focus when the form is shown
            this.Shown += MethodForm_Shown;
            InitializeComboBoxes();
        }

        private void MethodForm_Shown(object sender, EventArgs e)
        {
            ForwardButton.Focus();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {

            homePageForm.config.İl = IlComboBox.SelectedItem.ToString();
            homePageForm.config.İlçe = IlceComboBox.SelectedItem.ToString();

            SaveConfigToFile();
            
            if (MethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("İlerlemek için bir metot seçiniz");
                return;
            }

            selectedMethod = MethodComboBox.SelectedItem.ToString();
            
            // "Lütfen seçin" seçeneklerinin seçili olup olmadığını kontrol et
            if (IlComboBox.SelectedIndex == 0 || IlceComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Lütfen il ve ilçe seçiniz");
                return;
            }

            string selectedCity = IlComboBox.SelectedItem.ToString();
            string selectedDistrict = IlceComboBox.SelectedItem.ToString();

            // Var olan geçici klasörleri temizle
            CleanupExistingTempFolders(selectedCity, selectedDistrict);

            // Var olan oturum veya proje verilerini temizle
            CleanupExistingSessionData();

            // Sadece burada, kullanıcı onayladığında PathService'i güncelle ve klasör oluştur
            bool pathUpdated = PathService.UpdatePath(selectedCity, selectedDistrict);

            if (!pathUpdated)
            {
                MessageBox.Show("İl/ilçe yolu oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.", 
                    "Yol Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Seçilen metoda göre modül formunu aç
            OpenModülFormuBasedOnSelection(selectedMethod);
        }

        // Method to save the updated config to config.json
        public void SaveConfigToFile()
        {
            try
            {
                // Serialize the dynamic config object back to JSON with indentation
                string updatedJson = JsonConvert.SerializeObject(homePageForm.config, Newtonsoft.Json.Formatting.Indented);

                // Write the updated JSON back to the file
                File.WriteAllText(Path.Combine(homePageForm.projectRoot, "config.json"), updatedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}");
            }
        }

        // Mevcut geçici klasörleri temizleyen metot
        private void CleanupExistingTempFolders(string city, string district)
        {
            try
            {
                // İl/ilçe yolunu oluştur
                string districtPath = Path.Combine(PathService.BaseDirectory, city, district);
                
                // Klasör yoksa işlem yapma
                if (!Directory.Exists(districtPath))
                    return;
                
                // "temp_" ile başlayan tüm klasörleri bul
                string[] tempFolders = Directory.GetDirectories(districtPath, "temp_*");
                
                foreach (string folder in tempFolders)
                {
                    try
                    {
                        // Klasörü sil
                        Directory.Delete(folder, true);
                        Console.WriteLine($"Geçici klasör silindi: {folder}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Geçici klasör silinirken hata: {ex.Message}");
                        // Hatayı yut ve devam et
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geçici klasörleri temizlerken genel hata: {ex.Message}");
                // Hatayı yut ve devam et
            }
        }

        // Mevcut oturum verilerini temizleyen metot
        private void CleanupExistingSessionData()
        {
            try
            {
                // PathService'teki değerleri sıfırla
                PathService.ResetWorkingEnvironment();
                
                // GirdiModülü veri tablolarını temizle
                if (GirdiModülü.dataTablesByType != null)
                {
                    GirdiModülü.dataTablesByType.Clear();
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oturum verileri temizlenirken hata: {ex.Message}");
            }
        }

        private void OpenModülFormuBasedOnSelection(string method)
        {
            mod1 = new ModülFormu(method);  // Pass selectedMethod to ModülFormu

            // Hide both forms
            this.Hide();  // Hide MethodForm
            homePageForm.Hide();  // Hide HomePageForm

            mod1.ShowDialog();  // Show the new form as a dialog

            // Optionally, you can show both forms again if needed
            homePageForm.Show(); // Show HomePageForm again if it needs to be visible
            this.Show();  // Show MethodForm again after ModülFormu is closed
        }

        private void MethodPanel_Paint(object sender, PaintEventArgs e)
        {
            MethodPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
            this.DoubleBuffered = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Only trigger the ForwardButton's click event if MethodForm is the active form
            if (keyData == Keys.Enter && this == Form.ActiveForm)
            {
                // Trigger ForwardButton's Click event
                ForwardButton.PerformClick();
                return true; // Mark the key as handled
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void IlComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Önceki ilçeleri temizle
            IlceComboBox.Items.Clear();

            // Eğer "Lütfen seçin" seçeneği seçiliyse işlem yapma
            if (IlComboBox.SelectedIndex == 0)
            {
                IlceComboBox.Items.Add("Lütfen ilçe seçin");
                IlceComboBox.SelectedIndex = 0;
                tempSelectedCity = null; // Reset the temporary city
                return;
            }
            
            // Şehir seçimini geçici olarak sakla
            tempSelectedCity = IlComboBox.SelectedItem.ToString();

            if (tempSelectedCity == "İzmir")
            {
                // Add İzmir districts in alphabetical order
                IlceComboBox.Items.Add("Lütfen ilçe seçin");
                IlceComboBox.Items.Add("Aliağa");
                IlceComboBox.Items.Add("Balçova");
                IlceComboBox.Items.Add("Bayındır");
                IlceComboBox.Items.Add("Bayraklı");
                IlceComboBox.Items.Add("Bergama");
                IlceComboBox.Items.Add("Beydağ");
                IlceComboBox.Items.Add("Bornova");
                IlceComboBox.Items.Add("Buca");
                IlceComboBox.Items.Add("Çeşme");
                IlceComboBox.Items.Add("Çiğli");
                IlceComboBox.Items.Add("Dikili");
                IlceComboBox.Items.Add("Foça");
                IlceComboBox.Items.Add("Gaziemir");
                IlceComboBox.Items.Add("Güzelbahçe");
                IlceComboBox.Items.Add("Karabağlar");
                IlceComboBox.Items.Add("Karaburun");
                IlceComboBox.Items.Add("Karşıyaka");
                IlceComboBox.Items.Add("Kemalpaşa");
                IlceComboBox.Items.Add("Kınık");
                IlceComboBox.Items.Add("Kiraz");
                IlceComboBox.Items.Add("Konak");
                IlceComboBox.Items.Add("Menderes");
                IlceComboBox.Items.Add("Menemen");
                IlceComboBox.Items.Add("Narlıdere");
                IlceComboBox.Items.Add("Ödemiş");
                IlceComboBox.Items.Add("Seferihisar");
                IlceComboBox.Items.Add("Selçuk");
                IlceComboBox.Items.Add("Tire");
                IlceComboBox.Items.Add("Torbalı");
                IlceComboBox.Items.Add("Urla");
            }
            else if (tempSelectedCity == "Eskişehir")
            {
                // Add Eskişehir districts in alphabetical order
                IlceComboBox.Items.Add("Lütfen ilçe seçin");
                IlceComboBox.Items.Add("Alpu");
                IlceComboBox.Items.Add("Beylikova");
                IlceComboBox.Items.Add("Çifteler");
                IlceComboBox.Items.Add("Günyüzü");
                IlceComboBox.Items.Add("Han");
                IlceComboBox.Items.Add("İnönü");
                IlceComboBox.Items.Add("Mahmudiye");
                IlceComboBox.Items.Add("Mihalgazi");
                IlceComboBox.Items.Add("Mihalıççık");
                IlceComboBox.Items.Add("Odunpazarı");
                IlceComboBox.Items.Add("Sarıcakaya");
                IlceComboBox.Items.Add("Seyitgazi");
                IlceComboBox.Items.Add("Sivrihisar");
                IlceComboBox.Items.Add("Tepebaşı");
            }

            // Pre-select the first item
            if (IlceComboBox.Items.Count > 0)
            {
                IlceComboBox.SelectedIndex = 0;
            }
        }

        private void IlceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer "Lütfen seçin" seçenekleri seçiliyse işlem yapma
            if (IlComboBox.SelectedIndex == 0 || IlceComboBox.SelectedIndex == 0)
            {
                tempSelectedDistrict = null; // Reset the temporary district
                return;
            }

            // İlçe seçimini geçici olarak sakla
            tempSelectedDistrict = IlceComboBox.SelectedItem.ToString();

            // İl değişkeninin null olup olmadığını kontrol et
            if (tempSelectedCity != null && tempSelectedDistrict != null)
            {
                // Yolu göstermek için güncelle ama klasör oluşturma
                SelectedPath = System.IO.Path.Combine(tempSelectedCity, tempSelectedDistrict);
                Console.WriteLine($"Geçici seçim yolu: {SelectedPath}");
            }
        }
    }
}