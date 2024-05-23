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
using System.Xml;
using System.Data.SqlTypes;


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
        public enum DosyaTuru
        {
            CSV,
            Poligon
        }

        public struct YüklenenDosya
        {
            public string DosyaAdi { get; set; }
            public DosyaTuru DosyaTuru { get; set; }
        }

        public class PoligonVeri
        {
            public string Isim { get; set; }
            public List<NoktaVeri> Noktalar { get; set; }
        }

        private GMapOverlay markerOverlay;
        private GMapOverlay polygonOverlay;
        private List<PointLatLng> polygonPoints = new List<PointLatLng>();
        private List<YüklenenDosya> loadedFiles = new List<YüklenenDosya>();
        private List<PoligonVeri> poligonlar = new List<PoligonVeri>();

        private PointLatLng selectionStart;
        private PointLatLng selectionEnd;
        private bool isSelecting = false;
        private GMapPolygon selectionPolygon;

        public ModülFormu()
        {
            InitializeComponent();

            // GMapControl özelliklerini ayarlayın
            mapControl.MapProvider = GMapProviders.GoogleSatelliteMap;
            mapControl.Zoom = 10;
            mapControl.Position = new PointLatLng(38.4237, 27.1428);
            mapControl.MinZoom = 5;
            mapControl.MaxZoom = 100;
            mapControl.DragButton = MouseButtons.Left;

            // Yeni bir overlay oluşturun
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
            tbar1.Minimum = 5;
            tbar1.Maximum = 20;
            tbar1.SmallChange = 1;
            tbar1.LargeChange = 3;
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Zoom Bar'ın değeri değiştiğinde harita zoom seviyesini güncelleyin
            mapControl.Zoom = tbar1.Value;
        }

        private void saToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void button2_Click(object sender, EventArgs e)
        {
            gir1 = (GirişFormu)Tag;
            gir1.Show();
            this.Hide();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) { }

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
                mapControl.Refresh();
            }
        }

        private void at_closed(object sender, FormClosedEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Programı kapatmak istediğinize emin misiniz? Kaydedilmeyen veriler kaybolacaktır!",
                "Çıkış",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void ModülFormu_Load(object sender, EventArgs e) { }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*";
            openFileDialog.Title = "CSV Dosyasını Seç";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dosyaYolu = openFileDialog.FileName;
                CSVYukle(dosyaYolu);
                loadedFiles.Add(new YüklenenDosya { DosyaAdi = Path.GetFileName(dosyaYolu), DosyaTuru = DosyaTuru.CSV });
                UpdateListBox();
            }
        }

        private void CSVYukle(string dosyaYolu)
        {
            string[] satirlar = File.ReadAllLines(dosyaYolu);
            double ilkNoktaEnlem = 0;
            double ilkNoktaBoylam = 0;
            bool ilkNoktaBelirlendi = false;


            foreach (string satir in satirlar)
            {
                string[] parcalar = satir.Split(',');
                if (parcalar.Length >= 4 && double.TryParse(parcalar[0], out double enlem) && double.TryParse(parcalar[1], out double boylam)
                    && double.TryParse(parcalar[2], out double binaDem) && int.TryParse(parcalar[3], out int aboneSayisi))
                {
                    if (!ilkNoktaBelirlendi)
                    {
                        ilkNoktaEnlem = enlem;
                        ilkNoktaBoylam = boylam;
                        ilkNoktaBelirlendi = true;
                    }

                    NoktaVeri noktaVeri = new NoktaVeri
                    {
                        Enlem = enlem,
                        Boylam = boylam,
                        BinaDem = binaDem,
                        AboneSayisi = aboneSayisi
                    };

                    PointLatLng nokta = new PointLatLng(enlem, boylam);
                    GMapMarker marker = new GMarkerGoogle(nokta, GMarkerGoogleType.orange_dot);
                    marker.ToolTipText = Path.GetFileName(dosyaYolu); // Dosya adını ToolTipText olarak ayarla
                    marker.Tag = noktaVeri;
                    markerOverlay.Markers.Add(marker);
                }
            }

            if (ilkNoktaBelirlendi)
            {
                mapControl.Position = new PointLatLng(ilkNoktaEnlem, ilkNoktaBoylam);
                mapControl.Zoom = 15;
            }

            mapControl.Refresh();
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (var file in loadedFiles)
            {
                listBox1.Items.Add(file.DosyaAdi);
            }
        }


        private void btnplgn_Click_1(object sender, EventArgs e)
        {
            isSelecting = true;
            mapControl.OnMapClick += MapControl_OnMapClick;
        }

        private void btnTamamla_Click_1(object sender, EventArgs e)
        {
            if (polygonPoints.Count > 2)
            {
                string poligonIsim = $"poligon_{poligonlar.Count + 1}";
                GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim);
                polygon.Stroke = new Pen(Color.Red, 2);
                polygonOverlay.Polygons.Add(polygon);

                PoligonVeri poligonVeri = new PoligonVeri
                {
                    Isim = poligonIsim,
                    Noktalar = polygonPoints.Select(p => new NoktaVeri
                    {
                        Enlem = p.Lat,
                        Boylam = p.Lng,
                        BinaDem = 0,
                        AboneSayisi = 0
                    }).ToList()
                };
                poligonlar.Add(poligonVeri);
                listBox1.Items.Add(poligonVeri.Isim);

                loadedFiles.Add(new YüklenenDosya { DosyaAdi = poligonIsim, DosyaTuru = DosyaTuru.Poligon });
                UpdateListBox();

                mapControl.Refresh();
                isSelecting = false;
                polygonPoints.Clear();
            }
            else
            {
                MessageBox.Show("Poligon oluşturmak için en az 3 nokta seçmelisiniz.");
            }
        }


        private void ButtonKml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "KML Files (*.kml)|*.kml|All files (*.*)|*.*";
            openFileDialog.Title = "Select a KML File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string kmlFilePath = openFileDialog.FileName;
                KMLYukle(kmlFilePath);
            }
        }

        private void KMLYukle(string kmlFilePath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(kmlFilePath);

                XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
                ns.AddNamespace("kml", "http://www.opengis.net/kml/2.2");
                ns.AddNamespace("gx", "http://www.google.com/kml/ext/2.2");

                XmlNodeList placemarkNodes = xmlDoc.SelectNodes("//kml:Placemark", ns);
                foreach (XmlNode placemarkNode in placemarkNodes)
                {
                    XmlNode nameNode = placemarkNode.SelectSingleNode("kml:name", ns);
                    string name = nameNode != null ? nameNode.InnerText : "Untitled Placemark";

                    XmlNode styleUrlNode = placemarkNode.SelectSingleNode("kml:styleUrl", ns);
                    string styleUrl = styleUrlNode != null ? styleUrlNode.InnerText : "";

                    XmlNode lineStringNode = placemarkNode.SelectSingleNode("kml:LineString", ns);
                    if (lineStringNode != null)
                    {
                        XmlNode coordinatesNode = lineStringNode.SelectSingleNode("kml:coordinates", ns);
                        if (coordinatesNode != null)
                        {
                            string coordinates = coordinatesNode.InnerText.Trim();
                            string[] coordParts = coordinates.Split(' ');

                            List<PointLatLng> points = new List<PointLatLng>();
                            foreach (string coordPart in coordParts)
                            {
                                string[] coord = coordPart.Split(',');
                                if (coord.Length == 3)
                                {
                                    double lon = double.Parse(coord[0]);
                                    double lat = double.Parse(coord[1]);
                                    points.Add(new PointLatLng(lat, lon));
                                }
                            }

                            GMapOverlay overlay = new GMapOverlay();
                            GMapPolygon polygon = new GMapPolygon(points, name);
                            overlay.Polygons.Add(polygon);
                            mapControl.Overlays.Add(overlay);
                        }
                    }

                    XmlNode pointNode = placemarkNode.SelectSingleNode("kml:Point", ns);
                    if (pointNode != null)
                    {
                        XmlNode coordNode = pointNode.SelectSingleNode("kml:coordinates", ns);
                        if (coordNode != null)
                        {
                            string coordinates = coordNode.InnerText.Trim();
                            string[] coord = coordinates.Split(',');
                            if (coord.Length == 3)
                            {
                                double lon = double.Parse(coord[0]);
                                double lat = double.Parse(coord[1]);
                                PointLatLng point = new PointLatLng(lat, lon);

                                GMapOverlay overlay = new GMapOverlay();
                                GMarkerGoogle marker = new GMarkerGoogle(point, GMarkerGoogleType.red);
                                overlay.Markers.Add(marker);
                                mapControl.Overlays.Add(overlay);
                            }
                        }
                    }
                }

                mapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("KML dosyası yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void oznitelikAc_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                int selectedIndex = listBox1.SelectedIndex;
                if (selectedIndex >= loadedFiles.Count)
                {
                    int poligonIndex = selectedIndex - loadedFiles.Count;
                    if (poligonIndex >= 0 && poligonIndex < poligonlar.Count)
                    {
                        PoligonVeri selectedPoligon = poligonlar[poligonIndex];
                        formOznitelik oznitelikForm = new formOznitelik();
                        oznitelikForm.SetOznitelikler(selectedPoligon.Noktalar);
                        oznitelikForm.Show();
                    }
                }
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            Temizle();

            if (listBox1.SelectedIndex != -1)
            {
                loadedFiles.RemoveAt(listBox1.SelectedIndex);
                UpdateListBox();
            }
        }

        private void Temizle()
        {
            if (listBox1.SelectedIndex != -1)
            {
                int selectedIndex = listBox1.SelectedIndex;
                if (selectedIndex < loadedFiles.Count)
                {
                    // Seçilen öğe bir CSV dosyası ise
                    string selectedFile = loadedFiles[selectedIndex].DosyaAdi;

                    // Haritadaki işaretçileri de kaldır
                    foreach (var overlay in mapControl.Overlays)
                    {
                        // overlay.Markers koleksiyonunda işaretçileri bul
                        var markersToRemove = overlay.Markers.Where(marker => marker.ToolTipText == selectedFile).ToList();
                        foreach (var marker in markersToRemove)
                        {
                            overlay.Markers.Remove(marker); // Bulunan işaretçileri kaldır
                        }
                    }

                    loadedFiles.RemoveAt(selectedIndex);
                }
                else
                {
                    // Seçilen öğe bir poligon ise
                    int poligonIndex = selectedIndex - loadedFiles.Count;
                    if (poligonIndex >= 0 && poligonIndex < poligonlar.Count)
                    {
                        // Poligonun işaretçilerini sil
                        foreach (var nokta in poligonlar[poligonIndex].Noktalar)
                        {
                            var markerToRemove = markerOverlay.Markers.FirstOrDefault(marker => marker.Position.Lat == nokta.Enlem && marker.Position.Lng == nokta.Boylam);
                            if (markerToRemove != null)
                            {
                                markerOverlay.Markers.Remove(markerToRemove);
                            }
                        }

                        // Poligonu sil
                        var poligonOverlayToRemove = polygonOverlay.Polygons.FirstOrDefault(polygon => polygon.Name == poligonlar[poligonIndex].Isim);
                        if (poligonOverlayToRemove != null)
                        {
                            polygonOverlay.Polygons.Remove(poligonOverlayToRemove);
                        }

                        poligonlar.RemoveAt(poligonIndex);
                    }
                }
                listBox1.Items.RemoveAt(selectedIndex); // ListBox'tan ilgili öğeyi sil
            }

            // Haritayı yeniden çiz
            mapControl.Refresh();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}


