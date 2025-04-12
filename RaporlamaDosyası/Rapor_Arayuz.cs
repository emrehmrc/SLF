using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using SLF.Optimal_DTR;
using SLF.RaporlamaDosyası.FiltrelemeArayuz;
using SLF.Services;
using GMap.NET.WindowsForms.Markers;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace SLF.RaporlamaDosyası
{
    public partial class Rapor_Arayuz : Form
    {
        string SonucYolu;

        EA eA;
        DTR dTR;
        DEK dek;
        Alansal yuk;


        GMapOverlay overlay;

        ExcelImporter excelImporter = new ExcelImporter();

        Tablo_Formu tablo_Formu = new Tablo_Formu();

        public DataTable DataTableDTR { get; set; } = new DataTable();
        public DataTable DataTableEA { get; set; } = new DataTable();
        public DataTable DataTableYuk { get; set; } = new DataTable();
        public DataTable DataTableDEK { get; set; } = new DataTable();

        string KullanilanModul = string.Empty;


        public Rapor_Arayuz()
        {
            InitializeComponent();
            InitializeMap();



            //DosyaYolu = PathService.ImarAnaliziPath;
            //SonucYolu = PathService.SonuclarPath;
            SonucYolu = @"C:\Users\vural.bayrakli\source\repos\SLF\bin\Debug\il_ilce_kırılımları\İzmir\Aliağa\proje\sonuçlar";
        }

        private void InitializeMap()
        {

            overlay = new GMapOverlay("map");

            gMapControl1.Dock = DockStyle.Fill;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 12;
            gMapControl1.Position = new PointLatLng(38.5, 27.0); // Başlangıç konumu
            gMapControl1.DragButton = MouseButtons.Left;

        }

        

        public bool DosyaMevcutMu(string yol, string dosya)
        {
    
            if (!Directory.Exists(yol))
            {
                MessageBox.Show("Klasör Yok");
                return false;
            }

            if((!File.Exists(dosya)) | (dosya == null))
            {
                MessageBox.Show("Dosya Yok");
                return false;
            }

            return true;

        }

        public void FiltrelemeKismi(Form a, string modul)
        {
            this.panel3.Controls.Clear();

            a.Name = modul;
            a.Text = modul;
            a.TopLevel = false;
            a.FormBorderStyle = FormBorderStyle.None;
            a.Dock = DockStyle.Fill;
            a.Parent = this.panel3;
            a.Show();

        }

        public void VeriYazdir(string yol)
        {
            this.panel4.Controls.Clear();

            DataTable dt = excelImporter.ImportExcelFile(yol, null, false);

            /*tablo_Formu.vektörel_attribute_table.DataSource = dt;
            tablo_Formu.FormBorderStyle = FormBorderStyle.None;
            tablo_Formu.vektörel_attribute_table.Dock = DockStyle.Fill;
            tablo_Formu.TopLevel = false;
            tablo_Formu.Show();


            tablo_Formu.vektörel_attribute_table.Parent = this.panel5;*/


            var sonuc = from row in dt.AsEnumerable()
                       where row.Field<string>("Durum") == "Mevcut"
                       select row;

            DataTable filteredTable = sonuc.CopyToDataTable();

            

            
        }

    



        public string DosyaSeciciGoster(string klasorYolu)
        {
            Form form = new Form();
            form.Text = "Dosya Seç";
            form.Width = 500;
            form.Height = 400;
            form.StartPosition = FormStartPosition.CenterParent;

            ListBox listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Segoe UI", 12); // Yazı boyutu büyütüldü

            Button btnSec = new Button();
            btnSec.Text = "Seç";
            btnSec.Dock = DockStyle.Bottom;
            btnSec.Font = new Font("Segoe UI", 12); // Buton yazısı büyük

            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;

            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Segoe UI", 12);
            contentPanel.Controls.Add(listBox);

            btnSec.Height = 50;
            btnSec.Dock = DockStyle.Bottom;
            btnSec.Font = new Font("Segoe UI", 12);

            // Form’a ekle
            form.Controls.Add(contentPanel);
            form.Controls.Add(btnSec);

            string secilenDosya = null;

            if (Directory.Exists(klasorYolu))
            {
                string[] dosyaYollari = Directory.GetFiles(klasorYolu);
                foreach (var yol in dosyaYollari)
                {
                    listBox.Items.Add(Path.GetFileName(yol)); // sadece dosya adı
                }
            }
            else
            {
                listBox.Items.Add("Dosya bulunamadı.");
                btnSec.Enabled = false;
            }

            btnSec.Click += (s, e) =>
            {
                if (listBox.SelectedItem != null)
                {
                    secilenDosya = listBox.SelectedItem.ToString(); // sadece dosya adı döner
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen bir dosya seçin.");
                }
            };


            form.ShowDialog();

            return secilenDosya;
        }





        private void button1_Click(object sender, EventArgs e)
        {
            KullanilanModul = "DTR";

            string isim = "DTR";

            string yolDir = Path.Combine(SonucYolu, isim);

            string d = DosyaSeciciGoster(yolDir);
            string dosya = Path.Combine(yolDir, d);


            MessageBox.Show(dosya);


            if (DosyaMevcutMu(yolDir, dosya))
            {
                DataTableDTR = excelImporter.ImportExcelFile(dosya, null, false);

                if (dTR == null)
                {
                    dTR = new DTR(DataTableDTR);
                    dTR.FiltrelemeYapildi += Form_FiltrelemeYapildi;


                }
                FiltrelemeKismi(dTR, "DTR");

            }            

        }

        private void Form_FiltrelemeYapildi(object sender, FiltreEventArgs e)
        {
            // Parametreli fonksiyon çağır
            
            
            UygulaFiltre(e.dt);
            

        }

        private void UygulaFiltre(DataTable dt)
        {

            if (KullanilanModul == "DTR")
            {
                dTR.DrawMap(dTR.FiltrelenmisSonuc, overlay, gMapControl1);
            }
            else if (KullanilanModul == "EA")
            {
                
            }

            else if (KullanilanModul == "DEK")
            {
                
            }

            else
            {
                
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            KullanilanModul = "EA";

            string isim = "EA";

            string yolDir = Path.Combine(SonucYolu, isim);

            string d = DosyaSeciciGoster(yolDir);

            string dosya = Path.Combine(yolDir, d);

            if (!DosyaMevcutMu(yolDir, dosya))
            {
                DataTableEA = excelImporter.ImportExcelFile(dosya, null, false);

                if (eA == null)
                {
                    eA = new EA(DataTableEA);
                    eA.FiltrelemeYapildi += Form_FiltrelemeYapildi;

                }

                FiltrelemeKismi(eA, "EA");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string isim = "DEK";

            string yolDir = Path.Combine(SonucYolu, isim);

            string d = DosyaSeciciGoster(yolDir);

            string dosya = Path.Combine(yolDir, d);

            if (!DosyaMevcutMu(yolDir, dosya))
            {
                DataTableDEK = excelImporter.ImportExcelFile(dosya, null, false);

                if (dek == null)
                {
                    dek = new DEK(DataTableDEK);
                    eA.FiltrelemeYapildi += Form_FiltrelemeYapildi;
                }

                FiltrelemeKismi(dek, "DEK");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string isim = "Yuk";

            string yolDir = Path.Combine(SonucYolu, isim);

            try
            {
                string d = DosyaSeciciGoster(yolDir);

                string dosya = Path.Combine(yolDir, d);

                if (!DosyaMevcutMu(yolDir, dosya))
                {
                    DataTableYuk = excelImporter.ImportExcelFile(dosya, null, false);

                    if (yuk == null)
                    {
                        yuk = new Alansal(DataTableYuk);
                        eA.FiltrelemeYapildi += Form_FiltrelemeYapildi;
                    }

                    FiltrelemeKismi(dek, "DEK");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Herhangi dosya seçilmedi.");
            }

        }



        public async Task<DataTable> GetDataTableFromSQLite(string dbPath, string sqlQuery)
            {
                DataTable dt = new DataTable();

                string connectionString = $"Data Source={dbPath};Version=3;";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, conn))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    conn.Close();
                }

                return dt;
            }

        
    }
}

public class FiltreEventArgs : EventArgs
{
    public DataTable dt { get; set; }
    
}

public interface IDrawable
{
    void DrawMap(); // veya Task DrawAsync(); da olabilir
}
