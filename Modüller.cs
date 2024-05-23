using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Gis.SpatialReferencing;
using Aspose.Gis;
using AxMapWinGIS;
using EO.WebBrowser;
using MapWinGIS;
using Microsoft.Web.WebView2.WinForms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using Avalonia.Media;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        public bool is_panning = false;
        private double startX = 0, startY = 0;

        // form objeleri
        public GirişFormu gir1;

        // axMap objesine ait int return objesi
        public int layer_1;
         
        // halihazırda import edilmiş olan katman sayısı
        public int eklenmiş_katman_sayısı = 0;

        // stokastik haritasına eklenecek olan shapefile'lar ile alakalı arrayler
        public Shapefile[] shapefile_array = new Shapefile[10]; // shapefile array to hold .shp files/data
        public string[] shapefile_names = new string[10]; // names of the .shp files
        public int[] layerHandles = new int[10]; // array to hold int values to control the layers

        // declare an instance of the Tablo_Formu
        public Tablo_Formu tablo_formu;

        // default extents of the axMap object in the stokastik_haritası module
        public Extents stokastik_default_extents;

        public ModülFormu()
        {
            
            // modül initialization constructor
            InitializeComponent();

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            ConfigureTileCaching();

            // Girdi Modülü'nde default olarak "Abone Verileri" seçeneğini göster
            veri_listesi_seçimi.SelectedIndex = 0;    

            // stokastik haritasına ait initialization parametreleri
            /*stokastik_haritası.Latitude = 38.27f;
            stokastik_haritası.Longitude = 27.0f;
            stokastik_haritası.CurrentZoom = 13;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrArrow;
            stokastik_haritası_checkboxes_init();*/

            InitializeContextMenu_Nokta();

            // initialize the previously declared tablo_formu instance.
            tablo_formu = new Tablo_Formu();

        }

        private void ConfigureTileCaching()
        {
            /*stokastik_haritası.Tiles.DiskCacheFilename = "C:\\Users\\Zekiye\\source\\repos\\emrehmrc\\SLF\\Tiles\\tiles.txt";
            stokastik_haritası.Tiles.UseCache[tkCacheType.Disk] = true;
            stokastik_haritası.Tiles.MaxCacheSize[tkCacheType.Disk] = 2000000;
            stokastik_haritası.Tiles.DoCaching[tkCacheType.Disk] = true;
            stokastik_haritası.Tiles.UseServer = true;*/
        }

        private void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {
            // Find the first available slot
            int index = Array.FindIndex(shapefile_array, s => s == null);

            if (index == -1)
            {
                MessageBox.Show("En fazla 10 adet katman seçilebilmektedir.");
                return;
            }

            OpenFileDialog vektorel_veri_seçimi = new OpenFileDialog();

            vektorel_veri_seçimi.Filter = "Shapefile |*.shp|MapInfo File|*.tab|Google Earth File|*.kml";
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
                    shapefile_array[checkbox_index].SaveAsEx(filepath, true, false);
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

            if (shapefile_array[checkbox_index] != null)
            {
                tablo_formu.Text = "Veri Tablosu - " + shapefile_names[checkbox_index];
                LoadAttributeTable(shapefile_array[checkbox_index], tablo_formu.dataGridView_objesi);
            }
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
        private List<Shape> pointsList = new List<Shape>();

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

        private void stokastik_haritası_MouseDownEvent(object sender, _DMapEvents_MouseDownEvent e)
        {
            /*if (e.button == 1 && adding_points == true) // Left mouse button
            {
                double x = 0, y = 0;
                stokastik_haritası.PixelToProj(e.x, e.y, ref x, ref y);

                Shape pointShape = new Shape();
                pointShape.Create(ShpfileType.SHP_POINT);

                MapWinGIS.Point point = new MapWinGIS.Point();
                point.x = x;
                point.y = y;

                pointShape.InsertPoint(point, ref point_index);
                pointsList.Add(pointShape);
                point_index++;
                    
                MessageBox.Show(my_shp.NumFields.ToString());
                MessageBox.Show(my_shp.NumShapes.ToString());

                if (my_shp == null)
                {
                    my_shp = new Shapefile();
                    my_shp.CreateNewWithShapeID("", ShpfileType.SHP_POINT);
                    stokastik_haritası.AddLayer(my_shp, true);
                }

                int shapeIndex = my_shp.NumShapes;
                MessageBox.Show(shapeIndex.ToString());
                my_shp.EditInsertShape(pointShape, ref shapeIndex);
                my_shp.RefreshExtents();
            
            }*/
        }



        private void InitializeContextMenu_Nokta()
        {
            // Initialize the ContextMenuStrip
            nokta_menüsü = new ContextMenuStrip();

            // Add items to the ContextMenuStrip
            nokta_menüsü.Items.Add("Nokta Oluştur", null, nokta_ekle_click);
            nokta_menüsü.Items.Add("Nokta Sil", null, nokta_sil_click);
            nokta_menüsü.Items.Add("Tümünü Temizle", null, noktaları_sil_click);
            nokta_menüsü.Items.Add("Kaydet", null, noktaları_kaydet_click);
        }

        private void nokta_ekle_click(object sender, EventArgs e){

            adding_points = true;
        }

        private void nokta_sil_click(object sender, EventArgs e)
        {



        }

        private void noktaları_sil_click(object sender, EventArgs e)
        {



        }

        private void noktaları_kaydet_click(object sender, EventArgs e)
        {


        }


        private void button2_Click(object sender, EventArgs e)
        {
            gir1 = (GirişFormu)Tag;
            gir1.Show();
            this.Hide();                     
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

            /*if (result == DialogResult.Yes)
            {
                stokastik_haritası.RemoveAllLayers();
                System.Windows.Forms.Application.Exit();
            }*/
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            is_panning = false;
            /*stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmSelection;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrArrow;*/
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            is_panning = true;
            /*stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmPan;
            stokastik_haritası.MapCursor = MapWinGIS.tkCursor.crsrHand;*/

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            /*stokastik_haritası.Measuring.AreaUnits = tkAreaDisplayMode.admMetric;
            stokastik_haritası.CursorMode = MapWinGIS.tkCursorMode.cmMeasure;
            stokastik_haritası.Measuring.MeasuringType = MapWinGIS.tkMeasuringType.MeasureDistance;*/
            
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


        /// <summary>
        /// CUSTOM METHODS
        /// </summary>
        /// 


        private void LoadAttributeTable(Shapefile shapefile, DataGridView dataGridView)
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
                    
                    row[j+1] = shapefile.get_CellValue(j, i);
                }
                dataTable.Rows.Add(row);
                row_cnt++;
            }

            // Bind the DataTable to the DataGridView
            dataGridView.DataSource = dataTable;
        }

        private void toolStripButton8_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                nokta_menüsü.Show(Cursor.Position);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Google_Earth ge_formu = new Google_Earth();
            ge_formu.Show();
        }
    }
}
