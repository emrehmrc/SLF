using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using System.IO;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        // Nokta veri yapısı
        public class NoktaVeri
        {
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public double BinaDem { get; set; }
            public int AboneSayisi { get; set; }
        }

        private GMapOverlay markerOverlay;
        private List<string> loadedFiles = new List<string>(); // Yüklenen dosyaların adlarını saklamak için liste

        public Form1()
        {
            InitializeComponent();

            // GMapControl özelliklerini ayarlayın
            mapControl.MapProvider = GMapProviders.GoogleMap;
            mapControl.Position = new PointLatLng(0, 0);
            mapControl.MinZoom = 5;
            mapControl.MaxZoom = 100;
            mapControl.Zoom = 10;
            mapControl.DragButton = MouseButtons.Left;

            // Yeni bir overlay oluşturun
            markerOverlay = new GMapOverlay("markers");
            mapControl.Overlays.Add(markerOverlay);

            // Marker click event'ini bağlayın
            mapControl.OnMarkerClick += HaritaMarkerClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mapControl.ShowCenter = false;
        }

        private void HaritaMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri)
            {
                NoktaVeri nokta = (NoktaVeri)item.Tag;
                NoktaBilgileriniGoster(nokta);
            }
        }

        private void Temizle()
        {
            // markerOverlay üzerindeki tüm işaretçileri temizle
            markerOverlay.Markers.Clear();

            // Haritayı yeniden çiz
            mapControl.Refresh();
        }

        private void NoktaBilgileriniGoster(NoktaVeri nokta)
        {
            MessageBox.Show($"Enlem: {nokta.Enlem}\nBoylam: {nokta.Boylam}\nBina Dem: {nokta.BinaDem}\nAbone Sayısı: {nokta.AboneSayisi}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Kullanıcıya bir dosya seçme iletişim kutusu gösterin
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
            openFileDialog.Title = "CSV Dosyasını Seç";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dosyaYolu = openFileDialog.FileName;

                // CSV dosyasını okuyun ve haritaya ekleyin
                CSVYukle(dosyaYolu);
                loadedFiles.Add(Path.GetFileName(dosyaYolu)); // Dosya adını listeye ekle
                UpdateListBox(); // Listbox'ı güncelle
            }
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (string file in loadedFiles)
            {
                listBox1.Items.Add(file);
            }
        }

        private void CSVYukle(string dosyaYolu)
        {
            string[] satirlar = File.ReadAllLines(dosyaYolu);

            // İlk noktayı varsayılan olarak belirleyin
            double ilkNoktaEnlem = 0;
            double ilkNoktaBoylam = 0;
            bool ilkNoktaBelirlendi = false;

            // CSV dosyasındaki her bir satır için
            foreach (string satir in satirlar)
            {
                string[] parcalar = satir.Split(','); // Satırı virgülle ayırarak parçalara ayırın

                // Satırda en azından bir enlem ve bir boylam değeri olmalıdır
                if (parcalar.Length >= 4 && double.TryParse(parcalar[0], out double enlem) && double.TryParse(parcalar[1], out double boylam)
                    && double.TryParse(parcalar[2], out double binaDem) && int.TryParse(parcalar[3], out int aboneSayisi))
                {
                    // Eğer ilk nokta henüz belirlenmediyse, ilk noktayı belirleyin
                    if (!ilkNoktaBelirlendi)
                    {
                        ilkNoktaEnlem = enlem;
                        ilkNoktaBoylam = boylam;
                        ilkNoktaBelirlendi = true;
                    }

                    // Yeni bir nokta veri nesnesi oluşturun
                    NoktaVeri noktaVeri = new NoktaVeri
                    {
                        Enlem = enlem,
                        Boylam = boylam,
                        BinaDem = binaDem,
                        AboneSayisi = aboneSayisi
                    };

                    // Yeni bir nokta oluşturun ve haritaya ekleyin
                    PointLatLng nokta = new PointLatLng(enlem, boylam);
                    GMapMarker marker = new GMarkerGoogle(nokta, GMarkerGoogleType.orange_dot);
                    marker.Tag = noktaVeri; // NoktaVeri nesnesini marker'ın Tag özelliğine atayın
                    markerOverlay.Markers.Add(marker);
                }

            }

            // İlk noktayı haritada göstermek için zoom yapın
            if (ilkNoktaBelirlendi)
            {
                mapControl.Position = new PointLatLng(ilkNoktaEnlem, ilkNoktaBoylam);
                mapControl.Zoom = 15; // Zoom seviyesini istediğiniz seviyeye ayarlayabilirsiniz
            }

            // Haritayı yeniden çiz
            mapControl.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Temizle();

            if (listBox1.SelectedIndex != -1)
            {
                loadedFiles.RemoveAt(listBox1.SelectedIndex); // Seçilen dosyayı listeden sil
                UpdateListBox(); // Listbox'ı güncelle
            }

        }
    }
}
