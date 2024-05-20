using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SLF;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        public GirişFormu gir1;
        List<string> veri_listesi_requires_xlsx = new List<string> { 
            "Ekonometrik Yük Tahmini Verileri" 
        };

        public ModülFormu()
        {
            
            InitializeComponent();

        }

        private void saToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            gir1 = (GirişFormu)Tag;
            gir1.Show();
            this.Hide();
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog1 = new OpenFileDialog();

            // Modül seçimine göre dosya uzantısını belirle
            string selectedItem = veri_listesi_seçimi.SelectedItem.ToString();
            if (veri_listesi_requires_xlsx.Contains(selectedItem))
            {
                // Constructor'daki veri_listesi_requires_xlsx listesindeki verilerin uzantısını xlsx olarak belirle
                fileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Bu if bloğu dosya seçildiğinde çalışır
                    // Selected file path
                    string selectedFileName = fileDialog1.FileName;
                    ExcelImporter importer = new ExcelImporter();
                    DataTable dataTable = importer.ImportExcelFile(selectedFileName);
                    dataGridView1.DataSource = dataTable;

                    // Process the selected file (e.g., upload it)
                }
            }
            else
            {
                // Değilse şimdilik tüm dosya uzantılarını kabul et
                fileDialog1.Filter = "All files (*.*)|*.*";
                if (fileDialog1.ShowDialog() == DialogResult.OK) { 
                    // TODO
                }
            }

            fileDialog1.Title = "Select a file";

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string url = "https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu";
            webView21.CoreWebView2.Navigate(url);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string url2 = "https://www.openstreetmap.org/#map=15/38.4600/27.1153";
            webView21.CoreWebView2.Navigate(url2);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox6.Checked == true)
            {
                panel1.Visible= true;
                label5.Text = "kW:";
                label6.Text = "kW/m" + "\u00B2" + ":";
                label7.Text = "kWh:";
                label8.Text = "Abone Sayısı:";
                textBox1.CausesValidation= true;
            } 
            else
            {
                panel1.Visible= false;
            }
        }

        private void at_closed(object sender, FormClosedEventArgs e)
        {
            DialogResult result = MessageBox.Show("Programı kapatmak istediğinize emin misiniz? Kaydedilmeyen veriler kaybolacaktır!",
                                      "Çıkış",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning); // Added an icon for better visual indication

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void ModülFormu_Load(object sender, EventArgs e)
        {

        }
    }
}
