using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Xml;
using NetTopologySuite.Operation.Overlay;
using System.Reflection;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        private double startX = 0, startY = 0;

        // form objeleri
        public GirişFormu gir1;
         
        // halihazırda import edilmiş olan katman sayısı
        public int eklenmiş_katman_sayısı = 0;

        // declare an instance of the Tablo_Formu to be used to see the Attribute Table of the vector layers
        public Tablo_Formu tablo_formu;

        // variables to be used to create "ruler" in Stochastic/EA modules
        private List<PointLatLng> rulerPoints_stokastik = new List<PointLatLng>();
        private List<PointLatLng> rulerPoints_ea = new List<PointLatLng>();
        private GMapOverlay rulerOverlay_stokastik = new GMapOverlay("rulerOverlay_stokastik");
        private GMapOverlay rulerOverlay_ea = new GMapOverlay("rulerOverlay_ea");
        private GMapRoute rulerRoute_stokastik;
        private GMapRoute rulerRoute_ea;
        private bool isRulerEnabled = false; // enable the drawing of a ruler while pushing mouse down   
        private bool isRulerActive = false; // enable the drawing of a ruler
        private GMapOverlay markerOverlay_stokastik = new GMapOverlay("markerOverlay_stokastik");
        private GMapOverlay markerOverlay_ea = new GMapOverlay("markerOverlay_ea");
        
        // variables to be used to create polygons
        private GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        private GMapOverlay polygonOverlay_stokastik = new GMapOverlay("polygonOverlay_stokastik");
        private List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        private List<PoligonVeri> poligonlar_ea = new List<PoligonVeri>();
        private List<PointLatLng> polygonPoints_stokastik = new List<PointLatLng>();
        private List<PoligonVeri> poligonlar_stokastik = new List<PoligonVeri>();

        // variables to be used to create a grid
        private GMapOverlay gridOverlay = new GMapOverlay("grid");
        public int grid_size = 100;


        // boolean variable to control the grid selection by mouse down event
        private bool isSelecting_grid = false;

        // boolean variable to control the polygon selection by mouse down event
        private bool isSelecting_polygon = false;

        // boolean variable to control the marker/point selection by mouse down event
        private bool isSelecting_marker = false;


        private List<YüklenenDosya> loadedFiles = new List<YüklenenDosya>();


        // Nokta veri yapısı
        public class NoktaVeri
        {
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public double Bina_Demandi { get; set; }
            public int Abone_Sayısı { get; set; }
        }

        public enum FileType
        {
            CSV,
            Poligon
        }

        public struct YüklenenDosya
        {
            public string file_name { get; set; }
            public FileType file_type { get; set; }
        }

        public class PoligonVeri
        {
            public string polygon_name { get; set; }
            public List<NoktaVeri> Noktalar { get; set; }
        }

        private void InitializeGMap(GMap.NET.WindowsForms.GMapControl gmap)
        {
            gmap.MapProvider = GMapProviders.GoogleSatelliteMap;
            gmap.ShowCenter = false;
            gmap.Position = new PointLatLng(38.4237, 27.1428);
            gmap.MinZoom = 8;
            gmap.MaxZoom = 20;
            gmap.Zoom = 12;
            gmap.DragButton = MouseButtons.Left;
        }

        private void AddPolygonToOverlay(Polygon polygon, GMapOverlay overlay)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var coord in polygon.Coordinates)
            {
                points.Add(new PointLatLng(coord.Y, coord.X));
            }

            GMapPolygon gMapPolygon = new GMapPolygon(points, "gridPolygon")
            {
                Stroke = new Pen(Color.Green, 3),
                Fill = new SolidBrush(Color.FromArgb(50, Color.Blue))
            };

            overlay.Polygons.Add(gMapPolygon);
        }



        public ModülFormu() {
            
            InitializeComponent();

            // about Stokastik_Yuk_Haritası module
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);
            buton_stokastik_harita_katmanlar.BringToFront();
            buton_ea_harita_katmanlar.BringToFront();

            // stokastik haritası cetvel, nokta, poligon üst katmanları
            gMapControl_stokastik.Overlays.Add(rulerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(markerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(polygonOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(gridOverlay);

            // EA haritası cetvel, nokta, poligon üst katmanları
            gMapControl_EA.Overlays.Add(rulerOverlay_ea);
            gMapControl_EA.Overlays.Add(markerOverlay_ea);
            gMapControl_EA.Overlays.Add(polygonOverlay_ea);

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // Girdi Modülü'nde default olarak "Abone Verileri" seçeneğini göster
            veri_listesi_seçimi.SelectedIndex = 0;    

            // initialize the previously declared tablo_formu instance.
            tablo_formu = new Tablo_Formu();

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
                        Bina_Demandi = binaDem,
                        Abone_Sayısı = aboneSayisi
                    };

                    PointLatLng nokta = new PointLatLng(enlem, boylam);
                    GMapMarker marker = new GMarkerGoogle(nokta, GMarkerGoogleType.orange_dot);
                    marker.ToolTipText = Path.GetFileName(dosyaYolu); // Dosya adını ToolTipText olarak ayarla
                    marker.Tag = noktaVeri;
                    markerOverlay_stokastik.Markers.Add(marker);
                }
            }

            if (ilkNoktaBelirlendi)
            {
                gMapControl_stokastik.Position = new PointLatLng(ilkNoktaEnlem, ilkNoktaBoylam);
                gMapControl_stokastik.Zoom = 15;
            }

            gMapControl_stokastik.Refresh();
        }

        private void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {
            // Find the first available slot
            //int index = Array.FindIndex(shapefile_array, s => s == null);

           /* if (index == -1)
            {
                MessageBox.Show("En fazla 10 adet katman seçilebilmektedir.");
                return;
            }*/

            OpenFileDialog vektorel_veri_seçimi = new OpenFileDialog();

            vektorel_veri_seçimi.Filter = "Shapefile|*.shp|MapInfo File|*.tab|Google Earth File|*.kml|CSV File|*.csv";
            vektorel_veri_seçimi.InitialDirectory = "C:\\Users\\Zekiye\\Desktop\\CBS";

            DialogResult result = vektorel_veri_seçimi.ShowDialog();

            /*
            if (result == DialogResult.OK)
            {
                string filepath = vektorel_veri_seçimi.FileName;
                string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                string extension = filename.Substring(filename.Length - 3);
                
                if (extension == "shp")
                {
                    Shapefile added_shapefile = new Shapefile();

                    if (added_shapefile.Open(filepath, null))
                    {
                        shapefile_array[index] = added_shapefile;
                        shapefile_names[index] = filename;
                        layerHandles[index] = stokastik_haritası.AddLayer(added_shapefile, true);
                        stokastik_haritası.ZoomToLayer(layerHandles[index]);

                        System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(index);
                        if (associatedCheckBox != null)
                        {
                            associatedCheckBox.Checked = true;
                            associatedCheckBox.Visible = true;
                            associatedCheckBox.Text = shapefile_names[index];
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Dosya yüklenemedi: {filepath}");
                    }
                }
                else if (extension == "kml")
                {

                    // Create ConversionOptions if required
                    ConversionOptions options = new ConversionOptions();

                    // This options assigns Wgs84 to the destination layer.
                    // Conversion may throw error If destination layer does not support the Wgs84 spatial reference. So need to check.
                    // 
                    if (Drivers.Shapefile.SupportsSpatialReferenceSystem(SpatialReferenceSystem.Wgs84))
                        options.DestinationSpatialReferenceSystem = SpatialReferenceSystem.Wgs84;

                    // Convert file format from KML to Shapefile.
                    VectorLayer.Convert(filepath, Drivers.Kml, "C:\\Users\\Zekiye\\Desktop\\trial.shp", Drivers.Shapefile, options);

                    Shapefile added_shapefile = new Shapefile();

                    if (added_shapefile.Open("C:\\Users\\Zekiye\\Desktop\\trial.shp", null))
                    {
                        shapefile_array[index] = added_shapefile;
                        shapefile_names[index] = filename;
                        layerHandles[index] = stokastik_haritası.AddLayer(added_shapefile, true);
                        stokastik_haritası.ZoomToLayer(layerHandles[index]);

                        System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(index);
                        if (associatedCheckBox != null)
                        {
                            associatedCheckBox.Checked = true;
                            associatedCheckBox.Visible = true;
                            associatedCheckBox.Text = shapefile_names[index];
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Dosya yüklenemedi: {filepath}");
                    }
                    MessageBox.Show(".kml uzantılı dosya .shp uzantısına çevrildi.");
                }

                /*OgrDatasource ds = new OgrDatasource();*/

            // Open the .map file
            /*ds.Open(filename);
            stokastik_haritası.AddLayer(ds, true);
            ds.*/

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
            }

        }
        else
        {
            vektorel_veri_seçimi.Dispose();
        }*/
        }

        private void rengiDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem rengini_degistir_menu_item = sender as ToolStripMenuItem;
            /*
            if (rengini_degistir_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = rengini_degistir_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                ColorDialog stokastik_color = new ColorDialog();
                stokastik_color.AnyColor = true;
                stokastik_color.AllowFullOpen = true;
                stokastik_color.FullOpen = true;
                stokastik_color.Color = stokastik_haritası.get_ShapeLayerFillColor(checkbox_index);

                if (stokastik_color.ShowDialog() == DialogResult.OK)
                {
                    // Get the color components
                    byte a = stokastik_color.Color.A;
                    byte r = stokastik_color.Color.R;
                    byte g = stokastik_color.Color.G;
                    byte b = stokastik_color.Color.B;

                    // Combine them into a single uint in the order expected by MapWinGIS (ABGR)
                    uint abgr = (uint)(a << 24 | b << 16 | g << 8 | r);

                    stokastik_haritası.set_ShapeLayerFillColor(checkbox_index, abgr);
                    stokastik_haritası.Redraw(); // Redraw the map to reflect the changes
                }
            }*/
        }

        private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem kaydet_menu_item = sender as ToolStripMenuItem;

            if (kaydet_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = kaydet_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                SaveFileDialog kaydet_file_dialog = new SaveFileDialog();
                kaydet_file_dialog.Filter = "Shapefile |*.shp|MapInfo File|*.tab|Google Earth File|*.kml";
                kaydet_file_dialog.InitialDirectory = "C:\\Users\\Zekiye\\Desktop\\CBS";

                DialogResult kaydet_result = kaydet_file_dialog.ShowDialog();

                if (kaydet_result == DialogResult.OK)
                {
                    string filepath = kaydet_file_dialog.FileName;
                    //shapefile_array[checkbox_index].SaveAsEx(filepath, true, false);
                    MessageBox.Show("Dosya başarıyla kaydedildi.");

                }

            }

        }

        private void temizleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem delete_menu_item = sender as ToolStripMenuItem;
            /*
            if (delete_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = delete_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (shapefile_array[checkbox_index] != null)
                {
                    int layerHandle = layerHandles[checkbox_index];
                    string katman_ismi = shapefile_names[checkbox_index];

                    DialogResult temizle_result = MessageBox.Show(katman_ismi + " isimli katman " +
                        "silinecektir. Emin misiniz?", "", MessageBoxButtons.YesNo);

                    if (layerHandle != -1 & temizle_result == DialogResult.Yes)
                    {
                        shapefile_array[checkbox_index].RemoveSpatialIndex();
                        shapefile_array[checkbox_index].Close();
                        shapefile_array[checkbox_index] = null;
                        checkBox.Checked = false;
                        checkBox.Visible = false;
                        stokastik_haritası.Redraw();

                    }
                }
            }*/
        }

        // mouse down event of the checkboxes which displays the related data table with the corresponding
        // checkbox/layer
        private void stokastik_checkBox_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.CheckBox sender_checkbox = (System.Windows.Forms.CheckBox)sender;
            int checkbox_index = int.Parse(sender_checkbox.Tag.ToString()) - 1;
            temizleToolStripMenuItem.Tag = sender_checkbox;
            rengiDeğiştirToolStripMenuItem.Tag = sender_checkbox;
            kaydetToolStripMenuItem.Tag = sender_checkbox;

           /* if (shapefile_array[checkbox_index] != null)
            {
                tablo_formu.Text = "Veri Tablosu - " + shapefile_names[checkbox_index];
                LoadAttributeTable(shapefile_array[checkbox_index], tablo_formu.dataGridView_objesi);
            }*/
        }

        // display or hide the layers by checkboxes of the stokastik_yuk_tahmini form
        private void stokastik_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox) sender;
            int index = int.Parse(checkBox.Tag.ToString()) - 1;
            /*
            if (shapefile_array[index] != null)
            {
                int layerHandle = layerHandles[index];

                if (layerHandle != -1)
                {   
                    stokastik_haritası.set_LayerVisible(layerHandle, checkBox.Checked);
                    stokastik_haritası.Redraw();
                }
            }*/
        }

        private System.Windows.Forms.CheckBox GetCheckBoxByIndex(int index)
        {
            switch (index)
            {
                case 0: return checkBox9;
                case 1: return checkBox10;
                case 2: return checkBox11;
                case 3: return checkBox12;
                case 4: return checkBox13;
                case 5: return checkBox14;
                case 6: return checkBox15;
                case 7: return checkBox16;
                case 8: return checkBox17;
                case 9: return checkBox18;
                default: return null;
            }
        }

        // nokta ekleme/çıkarma gibi opsiyonların olduğu menü
        public ContextMenuStrip nokta_menüsü;

        // noktaların eklenip çıkarılacağı liste
        //private List<Shape> pointsList = new List<Shape>();

        // sol tıkla nokta ekleyebilme kontrolü
        public bool adding_points = false;
        int point_index = 0;

        private void stokastik_haritası_checkboxes_init()
        {
            // shapefile_array de kullanılmak üzere oluşturulan checkbox etiketleri
            checkBox9.Tag = 1;
            checkBox10.Tag = 2;
            checkBox11.Tag = 3;
            checkBox12.Tag = 4;
            checkBox13.Tag = 5;
            checkBox14.Tag = 6;
            checkBox15.Tag = 7;
            checkBox16.Tag = 8;
            checkBox17.Tag = 9;
            checkBox18.Tag = 10;

            // stokastik haritası checkboxlarına event atama
            checkBox9.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox10.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox11.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox12.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox13.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox14.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox15.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox16.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox17.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox18.CheckedChanged += stokastik_checkBox_CheckedChanged;

            checkBox9.MouseDown += stokastik_checkBox_MouseDown;
            checkBox10.MouseDown += stokastik_checkBox_MouseDown;
            checkBox11.MouseDown += stokastik_checkBox_MouseDown;
            checkBox12.MouseDown += stokastik_checkBox_MouseDown;
            checkBox13.MouseDown += stokastik_checkBox_MouseDown;
            checkBox14.MouseDown += stokastik_checkBox_MouseDown;
            checkBox15.MouseDown += stokastik_checkBox_MouseDown;
            checkBox16.MouseDown += stokastik_checkBox_MouseDown;
            checkBox17.MouseDown += stokastik_checkBox_MouseDown;
            checkBox18.MouseDown += stokastik_checkBox_MouseDown;
        }

        private void Poligon_Çiz_Click(object sender, EventArgs e)
        {
            isSelecting_polygon = true;

            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                polygonPoints_ea.Clear();
            } 
            else if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                polygonPoints_stokastik.Clear();
            }
        }

        private void EA_toolStrip_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void Stokastik_toolStrip_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void gMapControl_EA_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            // boolean control for marker selection when clicking on the map
            if(isSelecting_marker)
            {
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                markerOverlay_ea.Markers.Add(marker);

                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem =  Math.Round(pointClick.Lat, 4),
                    Boylam = Math.Round(pointClick.Lng, 4)
                };

                marker.Tag = noktaVeri_marker;

            }

            // boolean control for polygon selection when clicking on the map
            if (isSelecting_polygon)
            {
                polygonPoints_ea.Add(pointClick);
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                markerOverlay_ea.Markers.Add(marker);
                gMapControl_EA.Refresh();
            }

            // eğer gmapControl_OnMapClick event'i ile 2 den fazla nokta seçilirse,
            // bu noktalar arasında bir poligon çiz
            if (polygonPoints_ea.Count >= 3)
            {
                Draw_Polygon(polygonPoints_ea, polygonOverlay_ea,
                    poligonlar_ea, gMapControl_EA);
            }
        }

        private void gMapControl_stokastik_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {

            // boolean control for marker selection when clicking on the map
            if (isSelecting_marker)
            {
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                markerOverlay_stokastik.Markers.Add(marker);

                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem = Math.Round(pointClick.Lat, 4),
                    Boylam = Math.Round(pointClick.Lng, 4)
                };

                marker.Tag = noktaVeri_marker;

            }

            // boolean control for polygon selection when clicking on the map
            if (isSelecting_polygon == true)
            {
                polygonPoints_stokastik.Add(pointClick);
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                markerOverlay_stokastik.Markers.Add(marker);
                gMapControl_stokastik.Refresh();
            }

            // eğer gmapControl_OnMapClick event'i ile 2 den fazla nokta seçilirse,
            // bu noktalar arasında bir poligon çiz
            if (polygonPoints_stokastik.Count >= 3)
            {
                Draw_Polygon(polygonPoints_stokastik, polygonOverlay_stokastik,
                    poligonlar_stokastik, gMapControl_stokastik);
            }
        }

        private void EA_toolStrip_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void Stokastik_toolStrip_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Nokta.Show(Cursor.Position);
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
            if (checkBox6.Checked == true)
            {
                panel1.Visible = true;
                label5.Text = "kW:";
                label6.Text = "kW/m" + "\u00B2" + ":";
                label7.Text = "kWh:";
                label8.Text = "Abone Sayısı:";
                textBox1.CausesValidation= true;
            } 
            else
            {
                panel1.Visible = false;
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
                System.Windows.Forms.Application.Exit();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            Modül_Tabları.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
            veri_listesi_seçimi.Enabled = false;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Modül_Tabları.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "EA Şarj Verileri";
            veri_listesi_seçimi.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Modül_Tabları.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "DEK Verileri";
            veri_listesi_seçimi.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Modül_Tabları.SelectTab(tab_girdi);
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // cetveli ve cetvele ait noktaları/markerları sil
            if (markerOverlay_stokastik != null)
            {
                markerOverlay_stokastik.Markers.Clear();
            }

            if (rulerRoute_stokastik != null)
            {
                rulerRoute_stokastik.Dispose();
            }

            this.gMapControl_stokastik.CanDragMap = false;
            isRulerEnabled = false;
            gMapControl_stokastik.Cursor = Cursors.Arrow;
            mesafe_metre_stokastik.Visible = false;
            mesafe_metre_stokastik.Text = "";
            mesafe_metre_stokastik.SendToBack();
            Mesafe_stokastik.Visible = false;
            Mesafe_stokastik.SendToBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // cetveli ve cetvele ait noktaları/markerları sil
            if (markerOverlay_stokastik != null)
            {
                markerOverlay_stokastik.Markers.Clear();
            }

            if(rulerRoute_stokastik != null)
            {
                rulerRoute_stokastik.Dispose();
            }
            
            this.gMapControl_stokastik.CanDragMap = true;
            isRulerEnabled = false;
            gMapControl_stokastik.Cursor = Cursors.Hand;
            mesafe_metre_stokastik.Visible = false;
            mesafe_metre_stokastik.SendToBack();
            mesafe_metre_stokastik.Text = "";
            Mesafe_stokastik.Visible = false;
            Mesafe_stokastik.SendToBack();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            isRulerEnabled = true;
            Mesafe_stokastik.Visible = true;
            Mesafe_stokastik.BringToFront();
            mesafe_metre_stokastik.Visible = true;
            mesafe_metre_stokastik.BringToFront();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {

            // cetveli ve cetvele ait noktaları/markerları sil
            if (markerOverlay_ea != null)
            {
                markerOverlay_ea.Markers.Clear();
            }

            if (rulerRoute_ea != null)
            {
                rulerRoute_ea.Dispose();
            }

            this.gMapControl_EA.CanDragMap = false;
            isSelecting_marker = false;
            isSelecting_polygon = false;
            isRulerEnabled = false;
            gMapControl_EA.Cursor = Cursors.Arrow;
            mesafe_metre_ea.Visible = false;
            mesafe_metre_ea.Text = "";
            mesafe_metre_ea.SendToBack();
            Mesafe_ea.Visible = false;
            Mesafe_ea.SendToBack();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            // cetveli ve cetvele ait noktaları/markerları sil
            if (markerOverlay_ea != null)
            {
                markerOverlay_ea.Markers.Clear();
            }

            if (rulerRoute_ea != null)
            {
                rulerRoute_ea.Dispose();
            }

            this.gMapControl_EA.CanDragMap = true;
            isRulerEnabled = false;
            gMapControl_EA.Cursor = Cursors.Hand;
            mesafe_metre_ea.Visible = false;
            mesafe_metre_ea.Text = "";
            mesafe_metre_ea.SendToBack();
            Mesafe_ea.Visible = false;
            Mesafe_ea.SendToBack();

        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            isRulerEnabled = true;
            Mesafe_ea.Visible = true;
            Mesafe_ea.BringToFront();
            mesafe_metre_ea.Visible = true;
            mesafe_metre_ea.BringToFront();
        }

        private void Arazi_Click(object sender, EventArgs e)
        {
            gMapControl_stokastik.MapProvider = GMapProviders.GoogleTerrainMap;
            gMapControl_EA.MapProvider = GMapProviders.GoogleTerrainMap;
        }

        private void Harita_Click(object sender, EventArgs e)
        {
            gMapControl_stokastik.MapProvider = GMapProviders.GoogleMap;
            gMapControl_EA.MapProvider = GMapProviders.GoogleMap;
        }

        private void Uydu_Click(object sender, EventArgs e)
        {
            gMapControl_stokastik.MapProvider = GMapProviders.GoogleSatelliteMap;
            gMapControl_EA.MapProvider = GMapProviders.GoogleSatelliteMap;
        }
        private void OSM_Click(object sender, EventArgs e)
        {
            gMapControl_stokastik.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl_EA.MapProvider = GMapProviders.OpenStreetMap;
        }

        private void Google_Earth_Click(object sender, EventArgs e)
        {
            Google_Earth google_earth_form = new Google_Earth();
            google_earth_form.Owner = this;
            google_earth_form.Show();
            google_earth_form.BringToFront();
            google_earth_form.Focus();
        }

        private void gMapControl_EA_MouseDown(object sender, MouseEventArgs e)
        {

            // cetvel eventi tanımlaması
            if (e.Button == MouseButtons.Left && isRulerEnabled == true)
            {

                // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
                isRulerActive = true;

                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl_EA.FromLocalToLatLng(e.X, e.Y);

                // seçilen noktaları bir listeye koy
                rulerPoints_ea.Add(point);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker_ea = new GMarkerGoogle(point, GMarkerGoogleType.orange_dot);
                markerOverlay_ea.Markers.Add(marker_ea);

                // 2 adet nokta seçildiği anda aralarındaki mesafeyi hesapla, göster, sonrasında
                // ise noktaların tutulduğu listeyi temizle
                if (rulerPoints_ea.Count == 2)
                {
                    markerOverlay_ea.Markers.Clear();

                    foreach (var rulerPoint in rulerPoints_ea)
                    {
                        GMapMarker marker_1 = new GMarkerGoogle(rulerPoint, GMarkerGoogleType.orange_dot);
                        markerOverlay_ea.Markers.Add(marker_1);
                    }

                    rulerRoute_ea.Dispose();
                    DrawRuler_ea(rulerOverlay_ea, rulerPoints_ea);
                    CalculateDistance(gMapControl_EA, mesafe_metre_ea, rulerPoints_ea);
                    rulerPoints_ea.Clear();
                    isRulerActive = false;
                }
            }
        }

        private void DrawRuler_stokastik(GMapOverlay rulerOverlay, List<PointLatLng> rulerPoints)
        {
            if (rulerRoute_stokastik != null)
            {
                rulerOverlay.Routes.Remove(rulerRoute_stokastik);
            }
            rulerRoute_stokastik = new GMapRoute(rulerPoints, "rulerRoute");
            rulerRoute_stokastik.Stroke = new Pen(Color.Red, 3);
            rulerOverlay.Routes.Add(rulerRoute_stokastik);
            gMapControl_EA.Refresh();
        }

        private void DrawRuler_ea(GMapOverlay rulerOverlay, List<PointLatLng> rulerPoints)
        {
            if (rulerRoute_ea != null)
            {
                rulerOverlay.Routes.Remove(rulerRoute_ea);
            }
            rulerRoute_ea = new GMapRoute(rulerPoints, "rulerRoute");
            rulerRoute_ea.Stroke = new Pen(Color.Red, 3);
            rulerOverlay.Routes.Add(rulerRoute_ea);
            gMapControl_EA.Refresh();
        }

        private void CalculateDistance(GMapControl gmap, Label mesafe_metre, List<PointLatLng> rulerPoints)
        {
            if (rulerPoints.Count == 2)
            {
                double meter_distance = Math.Round(gmap.MapProvider.Projection.GetDistance(rulerPoints[0], rulerPoints[1]) * 1000, 3);
                mesafe_metre.Text = meter_distance.ToString() + " metre";
            }
        }

        private void gMapControl_EA_MouseMove(object sender, MouseEventArgs e)
        {
            // eğer sadece 1 adet nokta seçilmişse, ve ikinci nokta dinamik olarak farklı yerlere
            // tıklanarak seçiliyorsa, mesafeyi de buna göre güncelle.
            if (isRulerActive && rulerPoints_ea.Count == 1 && isRulerEnabled == true)
            {             
                var point = gMapControl_EA.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_ea != null)
                {
                    rulerOverlay_ea.Routes.Remove(rulerRoute_ea);
                }
                rulerRoute_ea = new GMapRoute(new List<PointLatLng> { rulerPoints_ea[0], point }, "rulerRoute_ea");
                rulerRoute_ea.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_ea.Routes.Add(rulerRoute_ea);
                gMapControl_EA.Refresh();
            }
        }

        private void gMapControl_stokastik_MouseDown(object sender, MouseEventArgs e)
        {
            // cetvel eventi tanımlaması
            if (e.Button == MouseButtons.Left && isRulerEnabled == true)
            {

                // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
                isRulerActive = true;

                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker_stokastik = new GMarkerGoogle(point, GMarkerGoogleType.orange_dot);
                markerOverlay_stokastik.Markers.Add(marker_stokastik);

                // seçilen noktaları bir listeye koy
                rulerPoints_stokastik.Add(point);
                
                // 2 adet nokta seçildiği anda aralarındaki mesafeyi hesapla ve noktaların
                // tutulduğu listeyi temizle
                if (rulerPoints_stokastik.Count == 2)
                {
                    markerOverlay_stokastik.Markers.Clear();

                    foreach (var rulerPoint in rulerPoints_stokastik)
                    {
                        GMapMarker marker_1 = new GMarkerGoogle(rulerPoint, GMarkerGoogleType.orange_dot);
                        markerOverlay_stokastik.Markers.Add(marker_1);
                    }

                    rulerRoute_stokastik.Dispose();
                    DrawRuler_stokastik(rulerOverlay_stokastik, rulerPoints_stokastik);
                    CalculateDistance(gMapControl_stokastik, mesafe_metre_stokastik, rulerPoints_stokastik);
                    rulerPoints_stokastik.Clear();
                    isRulerActive = false;
                }
            }
        }

        private void gMapControl_stokastik_MouseMove(object sender, MouseEventArgs e)
        {
            
            // eğer sadece 1 adet nokta seçilmişse, ve ikinci nokta dinamik olarak farklı yerlere
            // tıklanarak seçiliyorsa, mesafeyi de buna göre güncelle.
            if (isRulerActive && rulerPoints_stokastik.Count == 1 && isRulerEnabled == true)
            {
                
                var point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_stokastik != null)
                {
                    rulerOverlay_stokastik.Routes.Remove(rulerRoute_stokastik);
                }
                rulerRoute_stokastik = new GMapRoute(new List<PointLatLng> { rulerPoints_stokastik[0], point }, "rulerRoute_stokastik");
                rulerRoute_stokastik.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_stokastik.Routes.Add(rulerRoute_stokastik);
                gMapControl_stokastik.Refresh();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            /*stokastik_haritası.Measuring.AreaUnits = tkAreaDisplayMode.admMetric;
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
            stokastik_haritası.Measuring.MeasuringType = MapWinGIS.tkMeasuringType.MeasureArea;*/
            
        }

        private void katmanlar_right_click_Opening(object sender, CancelEventArgs e)
        {
            // Get the context menu strip that is being opened
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;

            // Get the checkbox associated with the context menu strip and set its font to bold
            System.Windows.Forms.CheckBox clickedCheckBox = (System.Windows.Forms.CheckBox)contextMenuStrip.SourceControl; 
            clickedCheckBox.Font = new Font(clickedCheckBox.Font, System.Drawing.FontStyle.Bold);

        }

        private void katmanlar_right_click_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            // Get the context menu strip that is being opened
            ContextMenuStrip contextMenuStrip = sender as ContextMenuStrip;

            // Get the checkbox associated with the context menu strip and set its font to regular
            System.Windows.Forms.CheckBox clickedCheckBox = contextMenuStrip.SourceControl as System.Windows.Forms.CheckBox;
            clickedCheckBox.Font = new Font(clickedCheckBox.Font, System.Drawing.FontStyle.Regular);

        }

        private void tabloyuGörToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tablo_formu.Show();
        }

        /*private void UpdateListBox()
        {
            EA_list_box.Items.Clear();
            foreach (var file in loadedFiles)
            {
                EA_list_box.Items.Add(file.file_name);
            }
        }*/

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
                            gMapControl_EA.Overlays.Add(overlay);
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
                                gMapControl_EA.Overlays.Add(overlay);
                            }
                        }
                    }
                }

                gMapControl_EA.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("KML dosyası yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


       /* private void oznitelikAc_Click(object sender, EventArgs e)
        {
            if (EA_list_box.SelectedIndex != -1)
            {
                int selectedIndex = EA_list_box.SelectedIndex;
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
        }*/


        /*private void button6_Click(object sender, EventArgs e)
        {
            Temizle();

            if (listBox1.SelectedIndex != -1)
            {
                loadedFiles.RemoveAt(listBox1.SelectedIndex);
                UpdateListBox();
            }
        }*/













        /// <summary>
        /// CUSTOM METHODS
        /// </summary>
        /// 









        /*private void LoadAttributeTable(Shapefile shapefile, DataGridView dataGridView)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Row_No");

            // Add columns to the DataTable
            for (int j = 0; j < shapefile.NumFields; j++)
            {
                Field field = shapefile.get_Field(j);
                dataTable.Columns.Add(field.Name);
            }

            int row_cnt = 1;

            // Add rows to the DataTable
            for (int i = 0; i < shapefile.NumShapes; i++)
            {
                DataRow row = dataTable.NewRow();
                row["Row_No"] = row_cnt;

                for (int j = 0; j < shapefile.NumFields; j++)
                {

                    row[j + 1] = shapefile.get_CellValue(j, i);
                }
                dataTable.Rows.Add(row);
                row_cnt++;
            }

            // Bind the DataTable to the DataGridView
            dataGridView.DataSource = dataTable;
        }*/




        private void NoktaBilgileriniGoster(NoktaVeri nokta)
        {
            MessageBox.Show($"Enlem: {nokta.Enlem}\nBoylam: {nokta.Boylam}\nBina Dem: " +
                $"{nokta.Bina_Demandi}\nAbone Sayısı: {nokta.Abone_Sayısı}");
        }

        private void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_ea)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }

        private void Nokta_Ekle_Click(object sender, EventArgs e)
        {
            isSelecting_marker = true;
        }

        private void gMapControl_stokastik_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_stokastik)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }

        public class GridGenerator
        {
            public static List<Polygon> CreateGrid(double xMin, double yMin, double xMax, double yMax, double cellSize)
            {
                var polygons = new List<Polygon>();
                var geomFactory = new GeometryFactory();

                for (double x = xMin; x < xMax; x += cellSize)
                {
                    for (double y = yMin; y < yMax; y += cellSize)
                    {
                        var coordinates = new Coordinate[]
                        {
                    new Coordinate(x, y),
                    new Coordinate(x + cellSize, y),
                    new Coordinate(x + cellSize, y + cellSize),
                    new Coordinate(x, y + cellSize),
                    new Coordinate(x, y)
                        };

                        var polygon = geomFactory.CreatePolygon(coordinates);
                        polygons.Add(polygon);
                    }
                }

                return polygons;
            }
        }

        private void Stokastik_toolStrip_Grid_Click(object sender, EventArgs e)
        {
            isSelecting_grid = true;

            Grid_Seçenekler grid_formu = new Grid_Seçenekler();
            grid_formu.Tag = this;
            grid_formu.Owner = this;
            grid_formu.Show();
            grid_formu.BringToFront();
            grid_formu.Focus();
            grid_formu.StartPosition = FormStartPosition.CenterScreen;

        }

        private void Draw_Polygon(List<PointLatLng> polygonPoints,GMapOverlay polygonOverlay,
                                  List<PoligonVeri> poligonlar, GMapControl gmap)
        {
            // bu noktalar arasında poligon çiz, kırmızı ile işaretle, ve de 
            // polygonOverlay katmanına ekle.
            string poligonIsim = $"Poligon_{poligonlar.Count + 1}";
            GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim);
            polygon.Stroke = new Pen(Color.DarkBlue, 3);
            polygonOverlay.Polygons.Add(polygon);

            // polygon_data isminde, PoligonVeri sınıfına ait yeni bir obje oluştur, bu sınıfın propertyleri
            // olan "polygon_name" ve "Noktalar" özelliklerini, ilgili objelerle doldur.
            PoligonVeri polygon_data = new PoligonVeri
            {
                polygon_name = poligonIsim,

                // LINQ sorgusu ve lambda expressionu (=>) kullanarak polygonPoints listesinin içindeki
                // her bir point için Enlem, Boylam, Bina_Demandi ve Abone_Sayısı property'lerini set et.
                Noktalar = polygonPoints.Select(p => new NoktaVeri
                {
                    Enlem = p.Lat,
                    Boylam = p.Lng,
                    Bina_Demandi = 0,
                    Abone_Sayısı = 0
                }).ToList()
            };

            // List<PoligonVeri> olan "poligonlar" instance'ını, ilgili "polygon_data" objesi ile doldur. 
            poligonlar.Add(polygon_data);

            EA_list_box.Items.Add(polygon_data.polygon_name);
            gmap.Refresh();
             
            //loadedFiles.Add(new YüklenenDosya { DosyaAdi = poligonIsim, DosyaTuru = DosyaTuru.Poligon });
            //UpdateListBox();

        }

        /*private void Temizle()
        {
            if (EA_list_box.SelectedIndex != -1)
            {
                int selectedIndex = EA_list_box.SelectedIndex;
                if (selectedIndex < loadedFiles.Count)
                {
                    // Seçilen öğe bir CSV dosyası ise
                    string selectedFile = loadedFiles[selectedIndex].file_name;

                    // Haritadaki işaretçileri de kaldır
                    foreach (var overlay in gMapControl_EA.Overlays)
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
                            var markerToRemove = markerOverlay_stokastik.Markers.FirstOrDefault(marker => marker.Position.Lat == nokta.Enlem && marker.Position.Lng == nokta.Boylam);
                            if (markerToRemove != null)
                            {
                                markerOverlay_stokastik.Markers.Remove(markerToRemove);
                            }
                        }

                        // Poligonu sil
                        var poligonOverlayToRemove = polygonOverlay.Polygons.FirstOrDefault(polygon => polygon.Name == poligonlar[poligonIndex].polygon_name);
                        if (poligonOverlayToRemove != null)
                        {
                            polygonOverlay.Polygons.Remove(poligonOverlayToRemove);
                        }

                        poligonlar.RemoveAt(poligonIndex);
                    }
                }
                EA_list_box.Items.RemoveAt(selectedIndex); // ListBox'tan ilgili öğeyi sil
            }

            // Haritayı yeniden çiz
            gMapControl_EA.Refresh();
        }*/

        public void CreateAndAddGridToMap()
        {
            // Define the bounding box and cell size
            double xMin = 27.04;
            double yMin = 38.4;
            double xMax = 27.15;
            double yMax = 38.5;
            double cellSizeMeters = grid_size; // Degree size of grid cells

            double centralLatitude = (yMin + yMax) / 2.0;
            double cellSizeDegrees = MetersToDegrees(cellSizeMeters, centralLatitude);

            // Create grid
            var grid = GridGenerator.CreateGrid(xMin, yMin, xMax, yMax, cellSizeDegrees);

            // Add grid polygons to the overlay
            foreach (var polygon in grid)
            {
                AddPolygonToOverlay(polygon, gridOverlay);
            }

            gMapControl_stokastik.Refresh();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            
        }

        private double MetersToDegrees(double meters, double latitude)
        {
            double degreesToRadians = Math.PI / 180.0;

            // Convert latitude from degrees to radians
            double latRad = latitude * degreesToRadians;

            // One degree of latitude in meters
            double metersPerDegreeLat = 111132.954 - 559.822 * Math.Cos(2 * latRad) + 1.175 * Math.Cos(4 * latRad);

            // One degree of longitude in meters, varies with latitude
            double metersPerDegreeLon = Math.Abs(111132.954 * Math.Cos(latRad));

            double degreesLat = meters / metersPerDegreeLat;
            double degreesLon = meters / metersPerDegreeLon;

            return (degreesLat + degreesLon) / 2.0; // Average for simplicity
        }
    }
}


