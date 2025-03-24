using Avalonia.Controls;
using SLF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            // Sadece burada, kullanıcı onayladığında PathService'i güncelle ve klasör oluştur
            PathService.UpdatePath(selectedCity, selectedDistrict);

            // Debug bilgisi
            Console.WriteLine($"İlerleme öncesi seçilen path: {PathService.FullPath}");

            // Seçilen metoda göre modül formunu aç
            OpenModülFormuBasedOnSelection(selectedMethod);
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

            {
                if (IlComboBox.SelectedItem.ToString() == "İzmir")
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

                    // Geriye dönük uyumluluk için eski SelectedPath özelliğini de güncelle
                    //SelectedPath = PathService.FullPath;
                }
                else if (IlComboBox.SelectedItem.ToString() == "Eskişehir")
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

                    // Update the global path
                    SelectedPath = PathService.FullPath;
                }

                // Pre-select the first item if needed
                if (IlceComboBox.Items.Count > 0)
                {
                    IlceComboBox.SelectedIndex = 0;
                }
            }
        }

        private void IlceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer "Lütfen seçin" seçenekleri seçiliyse işlem yapma
            if (IlComboBox.SelectedIndex == 0 || IlceComboBox.SelectedIndex == 0)
            {
                return;
            }

            // İlçe seçimini geçici olarak sakla
            tempSelectedDistrict = IlceComboBox.SelectedItem.ToString();

            // İl değişkeninin null olup olmadığını kontrol et
            if (tempSelectedCity != null && tempSelectedDistrict != null)
            {
                // Yolu göstermek için güncelle ama klasör oluşturma
                SelectedPath = System.IO.Path.Combine(tempSelectedCity, tempSelectedDistrict);
            }

            // NOT: Burada PathService.UpdatePath() çağrılmıyor
        }
    }
}