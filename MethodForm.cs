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
        private void InitializeComboBoxes()
        {
            // Add cities to the first combo box
            IlComboBox.Items.Add("İzmir");
            IlComboBox.Items.Add("Eskişehir");

            // Pre-select the first item if needed
            if (IlComboBox.Items.Count > 0)
            {
                IlComboBox.SelectedIndex = 0;
            }
            
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
            if (MethodComboBox.SelectedItem != null)
            {
                // Seçilen metodu kaydet
                selectedMethod = MethodComboBox.SelectedItem.ToString();

                // İl ve ilçe seçimlerini PathService'e kaydet (eğer seçilmişse)
                if (IlComboBox.SelectedItem != null && IlceComboBox.SelectedItem != null)
                {
                    string selectedCity = IlComboBox.SelectedItem.ToString();
                    string selectedDistrict = IlceComboBox.SelectedItem.ToString();

                    // PathService'i güncelle
                    PathService.UpdatePath(selectedCity, selectedDistrict);

                    // Debug bilgisi
                    Console.WriteLine($"İlerleme öncesi seçilen path: {PathService.FullPath}");
                }

                // Seçilen metoda göre modül formunu aç
                OpenModülFormuBasedOnSelection(selectedMethod);
            }
            else
            {
                MessageBox.Show("İlerlemek için bir metot seçiniz");
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
            // Clear the districts combo box
            IlceComboBox.Items.Clear();

            // Depending on the selected city, fill the districts
            if (IlComboBox.SelectedItem != null)
                
            {
                if (IlComboBox.SelectedItem.ToString() == "İzmir")
                {
                    // Add İzmir districts in alphabetical order
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
                    SelectedPath = PathService.FullPath;
                }
                else if (IlComboBox.SelectedItem.ToString() == "Eskişehir")
                {
                    // Add Eskişehir districts in alphabetical order
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
            if (IlComboBox.SelectedItem != null && IlceComboBox.SelectedItem != null)
            {
                string selectedCity = IlComboBox.SelectedItem.ToString();
                string selectedDistrict = IlceComboBox.SelectedItem.ToString();

                // Servis metodunu kullanarak path bilgisini güncelle
                PathService.UpdatePath(selectedCity, selectedDistrict);

                // Geriye dönük uyumluluk için eski SelectedPath özelliğini de güncelle
                SelectedPath = PathService.FullPath;

                //Console.WriteLine("Selected path: " + PathService.FullPath);

                // Örnek: Klasörün var olup olmadığını kontrol etme
                if (PathService.DirectoryExists())
                {
                    //Console.WriteLine("Bu il/ilçe için veri klasörü mevcut.");
                }
                else
                {
                    Console.WriteLine("Bu il/ilçe için veri klasörü henüz oluşturulmamış.");
                }
            }
        }

    }
}