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
        private GirdiModülü girdiModülü;

        public ModülFormu()
        {
            
            InitializeComponent();
            girdiModülü = new GirdiModülü();

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
            // Girdi modülündeki dosya yükleme butonuna tıklandığında çalışacak kodlar

            // Veri listesinde seçilen veri tipine göre dosya seçme işlemi yapılacak
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

            try
            {
                // ProcessFileSelection metodu ile dosya seçme işlemi yapılır ve seçilen dosya veri tablosuna yüklenir
                girdiModülü.ProcessFileSelection(seçilenVeriTipi);
                DataTable dataTable = girdiModülü.CurrentDataTable;
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dataTable;
                    girdiModülü.Validate();
                }
                else
                {
                    MessageBox.Show("Dosya seçimi gerçekleştirilemedi.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (NoFileSelectedException ex)
            {
                MessageBox.Show(ex.Message, "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidColumnHeadersException ex)
            {
                MessageBox.Show("Geçersiz sütun biçimi: " + ex.Message, "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
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
