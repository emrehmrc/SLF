using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMapWinGIS;
using MapWinGIS;
using Microsoft.Web.WebView2.WinForms;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        public GirişFormu gir1;

        public ModülFormu()
        {
            
            InitializeComponent();
            veri_listesi_seçimi.SelectedIndex = 0;
            stokastik_haritası.Latitude = 38.5f;
            stokastik_haritası.Longitude = 27.2f;
            stokastik_haritası.CurrentZoom = 12;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrArrow;
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
            fileDialog1.ShowDialog();

            
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
                stokastik_haritası.RemoveAllLayers();
                Application.Exit();
            }
        }

        private void ModülFormu_Load(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

            tabControl1.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
            veri_listesi_seçimi.Enabled = false;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "EA Şarj Verileri";
            veri_listesi_seçimi.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "DEK Verileri";
            veri_listesi_seçimi.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "İmar Verileri";
            veri_listesi_seçimi.Enabled = false;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                panel2.Visible = true;
            }
            else 
            { 
                panel2.Visible= false; 
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmSelection;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrArrow;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmPan;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrHand;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
            stokastik_haritası.Measuring.MeasuringType = MapWinGIS.tkMeasuringType.MeasureDistance;
            stokastik_haritası.Measuring.AreaUnits = tkAreaDisplayMode.admMetric;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
            stokastik_haritası.Measuring.MeasuringType = MapWinGIS.tkMeasuringType.MeasureArea;
            stokastik_haritası.Measuring.AreaUnits = tkAreaDisplayMode.admMetric;
        }

        private void button9_Click(object sender, EventArgs e)
        {

            OpenFileDialog vektorel_veri_sec = new OpenFileDialog();

            vektorel_veri_sec.Filter = "Shapefile |*.shp|MapInfo File|*.tab|Google Earth File|*.kml";
            vektorel_veri_sec.InitialDirectory = "C:\\Users\\emre.hangul\\MRC\\MRC - 1.1.3_T&SI\\" +
                "MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\\09_Alinan Veriler\\GDZ\\CBS";

            DialogResult result = vektorel_veri_sec.ShowDialog();
            Console.WriteLine(result.ToString());

            if(result == DialogResult.OK)
            {
                string filename = vektorel_veri_sec.FileName;

                OgrDatasource ds = new OgrDatasource();

                // Open the KML file
                ds.Open(filename);
                stokastik_haritası.AddLayer(ds, true);

                /*
                var sf = new Shapefile();
                var ds = new OgrDatasource();

                ds.Open(filename);

                if (sf.Open(filename, null))
                {
                    int layerHandle = stokastik_haritası.AddLayer(sf, true);
                }
                else
                {
                    Debug.WriteLine("Failed to open shapefile: " + sf.get_ErrorMsg(sf.LastErrorCode));
                }*/

            }
            else
            {
                vektorel_veri_sec.Dispose();
            }


        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox9_Click(object sender, EventArgs e)
        {
            katmanlar_right_click.Show(0, 0);
        }
    }
}
