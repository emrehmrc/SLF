using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        public GirişFormu gir1;

        // Nokta veri yapısı
        public class NoktaVeri
        {
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public double BinaDem { get; set; }
            public int AboneSayisi { get; set; }
        }

        private GMapOverlay markerOverlay;
        private GMapOverlay polygonOverlay;
        private List<PointLatLng> polygonPoints = new List<PointLatLng>(); // Poligon noktalarını tutacak liste
        private List<string> loadedFiles = new List<string>(); // Yüklenen dosyaların adlarını saklamak için liste
        private List<PoligonVeri> poligonlar = new List<PoligonVeri>();


        private PointLatLng selectionStart;
        private PointLatLng selectionEnd;
        private bool isSelecting = false;
        private GMapPolygon selectionPolygon;

        public ModülFormu()
        {
            InitializeComponent();

            // GMapControl özelliklerini ayarlayın
            mapControl.MapProvider = GMapProviders.GoogleMap;
            mapControl.Zoom = 10;
            mapControl.Position = new PointLatLng(38.4237, 27.1428); // İzmir'in enlem ve boylamı
            mapControl.MinZoom = 5;
            mapControl.MaxZoom = 100;
            
            mapControl.DragButton = MouseButtons.Left;

            //Yeni bir overlay oluşturun
            markerOverlay = new GMapOverlay("markers");
            mapControl.Overlays.Add(markerOverlay);

            // Yeni bir poligon overlay oluşturun
            polygonOverlay = new GMapOverlay("polygonOverlay");
            mapControl.Overlays.Add(polygonOverlay);

            // Zoom Bar'a olayları bağlayın
            tbar1.ValueChanged += TrackBar1_ValueChanged;

            // Başlangıç zoom seviyesini ayarlayın
            mapControl.Zoom = tbar1.Value;

            // TrackBar'ın minimum ve maksimum değerlerini ayarlayın
            tbar1.Minimum = 5; // Minimum zoom seviyesi
            tbar1.Maximum = 20; // Maximum zoom seviyesi

            // TrackBar'ın adımını ayarlayın
            tbar1.SmallChange = 1; // Küçük adım
            tbar1.LargeChange = 3; // Büyük adım

        }


        public class PoligonVeri
        {
            public string Isim { get; set; }
            public List<NoktaVeri> Noktalar { get; set; }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Zoom Bar'ın değeri değiştiğinde harita zoom seviyesini güncelleyin
            mapControl.Zoom = tbar1.Value;
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
            if (checkBox6.Checked == true)
            {
                panel1.Visible = true;
                label5.Text = "kW:";
                label6.Text = "kW/m" + "\u00B2" + ":";
                label7.Text = "kWh:";
                label8.Text = "Abone Sayısı:";
                textBox1.CausesValidation = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }
        private void MapControl_OnMapClick(PointLatLng point, MouseEventArgs e)
        {
            if (isSelecting)
            {
                polygonPoints.Add(point);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.black_small);
                markerOverlay.Markers.Add(marker);

                // Haritayı yeniden çiz
                mapControl.Refresh();
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

        private void button7_Click(object sender, EventArgs e)
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

        private void button6_Click(object sender, EventArgs e)
        {
            Temizle();

            if (listBox1.SelectedIndex != -1)
            {
                loadedFiles.RemoveAt(listBox1.SelectedIndex); // Seçilen dosyayı listeden sil
                UpdateListBox(); // Listbox'ı güncelle
            }
        }

        private void Temizle()
        {
            // markerOverlay üzerindeki tüm işaretçileri temizle
            markerOverlay.Markers.Clear();

            // Poligon overlay üzerindeki tüm poligonları temizle
            polygonOverlay.Polygons.Clear();

            // Poligon noktalarını temizleyin
            polygonPoints.Clear();

            // Poligon verilerini temizleyin
            poligonlar.Clear();

            // ListBox'ı temizleyin
            listBox1.Items.Clear();

            // Haritayı yeniden çiz
            mapControl.Refresh();
        }

        private void mapControl_OnMarkerClick_edited(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri)
            {
                NoktaVeri nokta = (NoktaVeri)item.Tag;
                NoktaBilgileriniGoster(nokta);
            }
        }

        private void NoktaBilgileriniGoster(NoktaVeri nokta)
        {
            MessageBox.Show($"Enlem: {nokta.Enlem}\nBoylam: {nokta.Boylam}\nBina Dem: {nokta.BinaDem}\nAbone Sayısı: {nokta.AboneSayisi}");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnplgn_Click_1(object sender, EventArgs e)
        {
            isSelecting = true;
            mapControl.OnMapClick += MapControl_OnMapClick;
        }

        private void btnTamamla_Click_1(object sender, EventArgs e)
        {
            if (polygonPoints.Count > 2) // Poligon oluşturmak için en az 3 nokta gereklidir
            {
                string poligonIsim = $"poligon_{poligonlar.Count + 1}";
                GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim);
                polygon.Stroke = new Pen(Color.Red, 2); // Poligonun çizgi rengini ve kalınlığını ayarlayın
                polygonOverlay.Polygons.Add(polygon);

                PoligonVeri poligonVeri = new PoligonVeri
                {
                    Isim = poligonIsim,
                    Noktalar = polygonPoints.Select(p => new NoktaVeri
                    {
                        Enlem = p.Lat,
                        Boylam = p.Lng,
                        BinaDem = 0, // Varsayılan değerler
                        AboneSayisi = 0 // Varsayılan değerler
                    }).ToList()
                };
                poligonlar.Add(poligonVeri);
                listBox1.Items.Add(poligonVeri.Isim);

                // Poligon noktalarını ve overlay'i yeniden çiz
                mapControl.Refresh();

                // Seçim işlemini sonlandırın ve listeyi temizleyin
                isSelecting = false;
                polygonPoints.Clear();
            }
            else
            {
                MessageBox.Show("Poligon oluşturmak için en az 3 nokta seçmelisiniz.");
            }
        }

        private void oznitelikAc_Click(object sender, EventArgs e)
{
    // ListBox'ta bir öğe seçilmişse
    if (listBox1.SelectedIndex != -1)
    {
        // Seçilen öğenin bir CSV dosyası mı yoksa bir poligon mu olduğunu kontrol edin
        if (listBox1.SelectedIndex < loadedFiles.Count)
        {
            // Seçilen öğe bir CSV dosyasıdır
            string selectedFileName = loadedFiles[listBox1.SelectedIndex];
            string filePath = Path.Combine(Environment.CurrentDirectory, selectedFileName);

            // CSV dosyasını oku ve öznitelikleri göstermek için uygun metodu çağır
            CSVYukle(filePath);
        }
        else
        {
            // Seçilen öğe bir poligondur
            int poligonIndex = listBox1.SelectedIndex - loadedFiles.Count;
            if (poligonIndex >= 0 && poligonIndex < poligonlar.Count)
            {
                PoligonVeri selectedPoligon = poligonlar[poligonIndex];

                // Yeni formu oluştur ve öznitelikleri ayarla
                formOznitelik oznitelikForm = new formOznitelik();
                oznitelikForm.SetOznitelikler(selectedPoligon.Noktalar);
                oznitelikForm.Show();
            }
        }
    }
}

    }
}
