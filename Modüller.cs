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
using NetTopologySuite.IO;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading.Tasks;
using Avalonia;
using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Operation;
using MapWinGIS;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        private double startX = 0, startY = 0;

        // form objeleri
        public GirişFormu gir1;
        private GirdiModülü girdiModülü;

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
        public GMapOverlay bounding_box_overlay;
        private GMapOverlay gridOverlay = new GMapOverlay("grid");
        private GMapPolygon bounding_box_polygon;
        public int grid_size = 250;
        private bool isSelecting_grid = false;
        private PointLatLng starting_point;
        private PointLatLng ending_point;

        // Get the user's profile path
        public string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string targetDirectory;

        // boolean variable to control the polygon selection by mouse down event
        private bool isSelecting_polygon = false;

        // boolean variable to control the marker/point selection by mouse down event
        private bool isSelecting_marker = false;
        
        // create a list of gMapOverlay's that will hold the imported vector files
        public GMapOverlay[] tüm_katmanlar_array = new GMapOverlay[10];
        public MapWinGIS.Shapefile[] shapeFileArray_MapWinGIS = new MapWinGIS.Shapefile[10];
        public DataTable[] tüm_katmanlar_datatable = new DataTable[10];    
        public string[] tüm_katmanlar_array_names = new string[10];

        private GMapPolygon selectedPolygon;

        private List<YüklenenDosya> loadedFiles = new List<YüklenenDosya>();

        // see the attributes of a polygon when clicked on it on the map 
        public Dictionary<GMapPolygon, DataRow> polygonAttributes; // for polygons other than grids
        public Dictionary<NetTopologySuite.Geometries.Polygon, DataRow> polygonAttributes_grid; // for polygons of grids
        public List<NetTopologySuite.Geometries.Polygon> entire_grid;
        public GMapPolygon combinedPolygon;
        public NetTopologySuite.Geometries.MultiPolygon multiPolygon;

        // Find the first available slot in the array that holds shapefile overlay layers
        public int layer_index;

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
            gmap.Position = new PointLatLng(38.472, 27.10);
            gmap.MinZoom = 8;
            gmap.Manager.Mode = AccessMode.ServerAndCache;
            gmap.MaxZoom = 20;
            gmap.Zoom = 13;
            gmap.DragButton = MouseButtons.Left;
        }

        public ModülFormu() {

            InitializeComponent();
            girdiModülü = new GirdiModülü();
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);

            // define the initial directory to be shown when the user opens up the import file dialog
            targetDirectory = System.IO.Path.Combine(userProfilePath, "Desktop");

            // bring the layers buttons that are positioned on the bottom left of the maps to front
            buton_stokastik_harita_katmanlar.BringToFront();
            buton_ea_harita_katmanlar.BringToFront();

            // initialization of the polygonAttributes object that gets to be displayed when double clicking
            // on the map
            polygonAttributes = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();

            // stokastik haritası cetvel, nokta, poligon üst katmanları
            gMapControl_stokastik.Overlays.Add(rulerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(markerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(polygonOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(gridOverlay);
            stokastik_haritası_checkboxes_init();

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


        private DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView, 
                ShapefileDataReader shapefile_reader, DataTable data_table, int row_cnt)
        {

            // populate the new row by using the .GetValue method 
            for (int i = 0; i < shapefile_reader.DbaseHeader.NumFields; i++)
            {
                row["Row_No"] = row_cnt;
                row[i+1] = shapefile_reader.GetValue(i); // get the value of all columns for the i-th row
            }

            // add the resulting row to the datatable
            data_table.Rows.Add(row);

            // Bind the DataTable to the DataGridView
            dataGridView.DataSource = data_table;

            return (data_table);
        }

        private async Task LoadShapefile(string filepath, GMapOverlay shapeFileOverlay, 
                                    DataTable shapefile_datatable)
        {
            
            // eğer dosya bulunamadıysa uyarı ver
            if (!File.Exists(filepath))
            {
                MessageBox.Show("Herhangi bir dosya bulunamadı. Lütfen tekrardan kontrol ediniz.");
                return;
            }

            if (!shapefile_datatable.Columns.Contains("Row_No"))
            {
                // datatable that will hold the atttribute table of the .shp file
                shapefile_datatable.Columns.Add("Row_No");
            }

            // shpReader object to read from the shp file  that is being imported
            var shpReader = new ShapefileDataReader(filepath, new NetTopologySuite.Geometries.GeometryFactory());

            // Initialize the DataTable columns based on the shapefile's attribute fields
            for (int i = 0; i < shpReader.DbaseHeader.NumFields; i++)
            {
                var sütunlar = shpReader.DbaseHeader.Fields[i];

                if (!shapefile_datatable.Columns.Contains(sütunlar.Name))
                {
                    shapefile_datatable.Columns.Add(sütunlar.Name, typeof(string)); // Simplified to string for all fields
                }
            }

            int row_cnt = 1;

            // read the lines of the .shp file one by one until no more line/row is left
            while (shpReader.Read())
            {
                // extract the geometry information of each line in the .shp file
                var geometry = shpReader.Geometry;

                // create a new row for the datatable and then populate it by
                // using the LoadAttributeTable() method
                DataRow row = shapefile_datatable.NewRow();
                shapefile_datatable = LoadAttributeTable(row, tablo_formu.attribute_table ,
                            shpReader, shapefile_datatable, row_cnt);

                row_cnt++;

                // check if the geometry of the shapefile includes one polygon or is a multipolygon,
                // add each of the polygons to the shapeFileOverlay by a for loop if multipolygon.
                if (geometry is NetTopologySuite.Geometries.Polygon polygon)
                {
                    AddPolygonToOverlay(polygon, shapeFileOverlay, "shapeFilePolygon", row);
                }
                else if (geometry is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                {
                    foreach (NetTopologySuite.Geometries.Polygon poly in multiPolygon.Geometries)
                    {
                        AddPolygonToOverlay(poly, shapeFileOverlay, "shapeFilePolygon", row);
                        
                    }
                }
            }

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 10 adet katman seçilebilmektedir.");
                return;
            }

            // Convert GMapOverlay to MapWinGIS.Shapefile
            MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(shapeFileOverlay);
            shapeFileArray_MapWinGIS[layer_index] = myShapefile;

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Refresh();
            }
            else if(Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Refresh();
            }
        }

        public MapWinGIS.Shapefile ConvertOverlayToShapefile(GMapOverlay overlay)
        {
            var shapefile = new MapWinGIS.Shapefile();
            shapefile.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);

            // Ensure attributes are added as fields
            if (polygonAttributes.Count > 0)
            {
                var firstPolygon = polygonAttributes.Keys.First();
                var firstRow = polygonAttributes[firstPolygon];
                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 10, 10);
                }
            }

            foreach (var gMapPolygon in overlay.Polygons)
            {
                var shape = new MapWinGIS.Shape();
                shape.Create(ShpfileType.SHP_POLYGON);

                for (int i = 0; i < gMapPolygon.Points.Count; i++)
                {
                    var point = new MapWinGIS.Point
                    {
                        x = gMapPolygon.Points[i].Lng,
                        y = gMapPolygon.Points[i].Lat
                    };
                    shape.InsertPoint(point, ref i);
                }

                int shapeIndex = shapefile.NumShapes;
                shapefile.EditInsertShape(shape, ref shapeIndex);

                // Add attributes to the shape
                if (polygonAttributes.TryGetValue(gMapPolygon, out DataRow row))
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        shapefile.EditCellValue(i, shapeIndex, row[i].ToString());
                    }
                }
            }
            return shapefile;
        }

        public async Task LoadKmlFile(string filepath, GMapOverlay kmlOverlay, DataTable data_table)
        {

            // eğer dosya bulunamadıysa uyarı ver
            if (!File.Exists(filepath))
            {
                MessageBox.Show("KML dosyası bulunamadı.!");
                return;
            }

            // oluşturulacak data table'a eklenecek olan row_cnt variable'ının initialization'u
            int row_cnt = 1;

            // bir stream yarat ve import edilen kml dosyasını okumaya başla.
            using (var stream = File.OpenRead(filepath))
            {
                // create a KML parser object and start parsing the KML stream
                var parser = new Parser();
                parser.Parse(stream);

                // root node olan <kml> node'una eriş
                var kml = parser.Root as Kml;
                var folder = kml?.Feature as SharpKml.Dom.Folder;
                
                // <kml> node'unun child node'unun <folder> veya <Document> olup olmadığının kontrolü
                if(folder != null)
                {
                    // <folder> node'unun içindeki <Document> node'larını okumaya başla
                    foreach (var documents in folder.Features)
                    {
                        // eğer halihazrıda "Row_No" isminde bir sütun yoksa ekle
                        if (!data_table.Columns.Contains("Row_No"))
                        {
                            data_table.Columns.Add("Row_No");
                        }

                        // eğer halihazrıda "coordinates" isminde bir sütun yoksa ekle
                        if (!data_table.Columns.Contains("coordinates"))
                        {
                            data_table.Columns.Add("coordinates");
                        }

                        // <document> node'unu flatten ile düzelt, sonrasında içindeki <placemark> node'unu iterate et 
                        foreach (var placemark in documents.Flatten().OfType<SharpKml.Dom.Placemark>())
                        {
                            var row = data_table.NewRow();

                            // Handle ExtendedData
                            if (placemark.ExtendedData != null)
                            {
                                foreach (var schemaData in placemark.ExtendedData.SchemaData)
                                {
                                    foreach (var simpleData in schemaData.SimpleData)
                                    {
                                        if (!data_table.Columns.Contains(simpleData.Name))
                                        {
                                            data_table.Columns.Add(simpleData.Name);
                                        }
                                        row["Row_No"] = row_cnt;
                                        row[simpleData.Name] = simpleData.Text;
                                    }
                                }  

                                // <ExtendedData> içindeki dataları Attribute Table'da ilgili sütunlara yaz
                                foreach (var data in placemark.ExtendedData.Data)
                                {
                                    if (!data_table.Columns.Contains(data.Name))
                                    {
                                        data_table.Columns.Add(data.Name);
                                    }
                                    row["Row_No"] = row_cnt;
                                    row[data.Name] = data.Value;
                                }
                            }

                            // add polygon coordinates to "coordinates" column if any polygon exists
                            foreach (SharpKml.Dom.Polygon polygon in placemark.Flatten().OfType<SharpKml.Dom.Polygon>())
                            {
                                foreach (SharpKml.Dom.OuterBoundary outerBoundary in polygon.Flatten().OfType<SharpKml.Dom.OuterBoundary>())
                                {
                                    foreach (SharpKml.Dom.LinearRing linearRing in outerBoundary.Flatten().OfType<SharpKml.Dom.LinearRing>())
                                    {
                                        // Convert the coordinates to a string
                                        string coordinatesString = string.Join(" ; ",
                                            linearRing.Coordinates.Select(coord => $"{Math.Round(coord.Longitude, 6)},{Math.Round(coord.Latitude, 6)}"));
                                        row["Row_No"] = row_cnt;
                                        row["coordinates"] = coordinatesString;
                                    }
                                }
                            }

                            // add point coordinates to "coordinate" column if any point exists
                            foreach (SharpKml.Dom.Point points in placemark.Flatten().OfType<SharpKml.Dom.Point>())
                            {
                                // Convert the coordinates to a string
                                string point_coordinates = Math.Round(points.Coordinate.Longitude,6).ToString() + 
                                    " ; " + Math.Round(points.Coordinate.Latitude,6).ToString();
                                row["Row_No"] = row_cnt;
                                row["coordinates"] = point_coordinates;
                                    
                            }

                            // add linestring coordinates to "coordinates" column if any linestring exists
                            foreach (SharpKml.Dom.LineString lineString in placemark.Flatten().OfType<SharpKml.Dom.LineString>())
                            {
                                // Convert the coordinates to a string
                                string coordinatesString = string.Join(" ; ",
                                    lineString.Coordinates.Select(coord => $"{Math.Round(coord.Longitude, 6)},{Math.Round(coord.Latitude, 6)}"));
                                row["Row_No"] = row_cnt;
                                row["coordinates"] = coordinatesString;
                            }

                            // Handle direct attributes
                            var attributes = placemark.GetType().GetProperties();
                            foreach (var attribute in attributes)
                            {
                                if (!data_table.Columns.Contains(attribute.Name))
                                {
                                    data_table.Columns.Add(attribute.Name);
                                }
                                row["Row_No"] = row_cnt;
                                row[attribute.Name] = attribute.GetValue(placemark)?.ToString();
                            }

                            // oluşturulan satırı tablouya ekle
                            data_table.Rows.Add(row);
                            row_cnt++;

                            // geometry bilgisini polygon olarak ya da multiline string olarak ekle
                            var geometry = placemark.Geometry;

                            if (geometry is SharpKml.Dom.Polygon kmlPolygon)
                            {
                                AddPolygonToOverlay_kml(kmlPolygon, kmlOverlay, row);
                            }
                            else if (geometry is SharpKml.Dom.LineString kmlLineString)
                            {
                                AddLineStringToOverlay_kml(kmlLineString, kmlOverlay);
                            }
                        }
                    }

                }
                else
                {
                    // eğer <kml> root node'unun child/feature'larından biri document ise
                    var document = kml?.Feature as SharpKml.Dom.Document;
                    bool columnsAdded = false;

                    // eğer halihazrıda "Row_No" isminde bir sütun yoksa ekle
                    if (!data_table.Columns.Contains("Row_No"))
                    {
                        data_table.Columns.Add("Row_No");
                    }

                    // eğer halihazrıda "coordinates" isminde bir sütun yoksa ekle
                    if (!data_table.Columns.Contains("coordinates"))
                    {
                        data_table.Columns.Add("coordinates");
                    }

                    // for each row of the flattened document of KML file, fill the row of the datatable
                    foreach (var placemark in document.Flatten().OfType<SharpKml.Dom.Placemark>())
                    {
                        // her satırı table'a eklemek için her satır için yeni bir "row" objesi oluştur.
                        var row = data_table.NewRow();

                        if (!columnsAdded)
                        {
                            // Add columns based on the Schema if available
                            foreach (var schema in document.Schemas)
                            {
                                // extract the schema.Fields info and add the field names as the column names of 
                                // the data_table
                                foreach (var field in schema.Fields)
                                {
                                    // halihazırda sütun ismi eklenmişse pas geç, eklenmediyse ekle
                                    if (!data_table.Columns.Contains(field.Name))
                                    {
                                        data_table.Columns.Add(field.Name);
                                    }
                                }
                            }
                            columnsAdded = true;
                        }

                        // <ExtendedData> isimli node varsa içerindeki data'yı Attribute Table'a ekle
                        if (placemark.ExtendedData != null)
                        {
                            foreach (var schemaData in placemark.ExtendedData.SchemaData)
                            {
                                foreach (var simpleData in schemaData.SimpleData)
                                {
                                    if (!data_table.Columns.Contains(simpleData.Name))
                                    {
                                        data_table.Columns.Add(simpleData.Name);
                                    }
                                    row["Row_No"] = row_cnt;
                                    row[simpleData.Name] = simpleData.Text;
                                }
                            }   

                            foreach (var data in placemark.ExtendedData.Data)
                            {
                                if (!data_table.Columns.Contains(data.Name))
                                {
                                    data_table.Columns.Add(data.Name);
                                }
                                row["Row_No"] = row_cnt;
                                row[data.Name] = data.Value;
                            }
                        }

                        // add polygon coordinates to "coordinates" column if any polygon exists
                        foreach (SharpKml.Dom.Polygon polygon in placemark.Flatten().OfType<SharpKml.Dom.Polygon>())
                        {
                            foreach (SharpKml.Dom.OuterBoundary outerBoundary in polygon.Flatten().OfType<SharpKml.Dom.OuterBoundary>())
                            {
                                foreach (SharpKml.Dom.LinearRing linearRing in outerBoundary.Flatten().OfType<SharpKml.Dom.LinearRing>())
                                {
                                    // Convert the coordinates to a string
                                    string coordinatesString = string.Join(" ; ", 
                                        linearRing.Coordinates.Select(coord => $"{Math.Round(coord.Longitude,6)},{Math.Round(coord.Latitude,6)}"));
                                    row["Row_No"] = row_cnt;
                                    row["coordinates"] = coordinatesString;
                                }
                            }
                        }

                        // add point coordinates to "coordinate" column if any point exists
                        foreach (SharpKml.Dom.Point points in placemark.Flatten().OfType<SharpKml.Dom.Point>())
                        {
                            // Convert the coordinates to a string
                            string point_coordinates = Math.Round(points.Coordinate.Longitude, 6).ToString() +
                                " ; " + Math.Round(points.Coordinate.Latitude, 6).ToString();
                            row["Row_No"] = row_cnt;
                            row["coordinates"] = point_coordinates;

                        }

                        // add linestring coordinates to "coordinates" column if any linestring exists
                        foreach (SharpKml.Dom.LineString lineString in placemark.Flatten().OfType<SharpKml.Dom.LineString>())
                        {
                            // Convert the coordinates to a string
                            string coordinatesString = string.Join(" ; ",
                                lineString.Coordinates.Select(coord => $"{Math.Round(coord.Longitude, 6)},{Math.Round(coord.Latitude, 6)}"));
                            row["Row_No"] = row_cnt;
                            row["coordinates"] = coordinatesString;
                        }

                        // Handle direct attributes
                        var attributes = placemark.GetType().GetProperties();
                        foreach (var attribute in attributes)
                        {
                            if (!data_table.Columns.Contains(attribute.Name))
                            {
                                data_table.Columns.Add(attribute.Name);
                            }
                            row["Row_No"] = row_cnt;
                            row[attribute.Name] = attribute.GetValue(placemark)?.ToString();
                        }

                        data_table.Rows.Add(row);
                        row_cnt++;

                        // geometry bilgisini polygon olarak ya da multiline string olarak ekle
                        var geometry = placemark.Geometry;

                        if (geometry is SharpKml.Dom.Polygon kmlPolygon)
                        {
                            AddPolygonToOverlay_kml(kmlPolygon, kmlOverlay, row);
                        }
                        else if (geometry is SharpKml.Dom.LineString kmlLineString)
                        {
                            AddLineStringToOverlay_kml(kmlLineString, kmlOverlay);
                        }
                    }
                }
            }
            
            if(Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Refresh();
            }

            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Refresh();
            }
        }

        private void AddPolygonToOverlay_kml(SharpKml.Dom.Polygon kmlPolygon, 
                                            GMapOverlay overlay,
                                            DataRow attributes)
        {
            // coordinates node, which has parental noods as Polygon.OuterBoundary.LinearRing.Coordinates
            var coordinates = kmlPolygon.OuterBoundary.LinearRing.Coordinates;
            List<PointLatLng> points = coordinates.Select(coord => new PointLatLng(coord.Latitude, coord.Longitude)).ToList();

            GMapPolygon polygon = new GMapPolygon(points, "KmlPolygon")
            {
                Stroke = new Pen(Color.DarkBlue, 3),
                Fill = new SolidBrush(Color.FromArgb(50, Color.DarkBlue))
            };
            overlay.Polygons.Add(polygon);
            polygonAttributes[polygon] = attributes;
        }

        private void AddLineStringToOverlay_kml(SharpKml.Dom.LineString kmlLineString, GMapOverlay overlay)
        {
            // points node list, which has parental nodes as LineString.Coordinates
            List<PointLatLng> points = kmlLineString.Coordinates.Select(coord => new PointLatLng(coord.Latitude, coord.Longitude)).ToList();

            // create a new route including the points inside the points list.
            var route = new GMapRoute(points, "KmlLineString")
            {
                Stroke = new Pen(Color.DarkRed, 3)
            };
            overlay.Routes.Add(route);
        }

        // stokastik dosya seçimi butonu
        private async void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 10 adet katman seçilebilmektedir.");
                return;
            }

            // file dialog to select a file to import
            OpenFileDialog vektorel_veri_seçimi = new OpenFileDialog();

            string targetDirectory = System.IO.Path.Combine(userProfilePath, "Desktop");
            vektorel_veri_seçimi.Filter = "Shapefile|*.shp|Google Earth File|*.kml|CSV File|*.csv";
            vektorel_veri_seçimi.InitialDirectory = targetDirectory;

            DialogResult result = vektorel_veri_seçimi.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filepath = vektorel_veri_seçimi.FileName; 
                string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                string extension = filename.Substring(filename.Length - 3);

                if (extension == "shp")
                {
                    // create a new layer to be added onto the map
                    GMapOverlay shapeFileOverlay = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}");

                    // add the layer to the specified map
                    if (Modül_Tabları.SelectedTab == tab_stokastik)
                    {
                        gMapControl_stokastik.Overlays.Add(shapeFileOverlay);
                    }
                    else if (Modül_Tabları.SelectedTab == tab_ea)
                    {
                        gMapControl_EA.Overlays.Add(shapeFileOverlay);
                    }
                    
                    // create a new datatable to be added to the tüm_katmanlar_datatable array
                    DataTable shapefile_datatable = new DataTable();

                    // run the import method
                    this.Cursor = Cursors.WaitCursor;
                    await LoadShapefile(filepath, shapeFileOverlay, shapefile_datatable);
                    this.Cursor = Cursors.Default;

                    // add the layer and its name to the specified arrays
                    tüm_katmanlar_array[layer_index] = shapeFileOverlay;
                    tüm_katmanlar_array_names[layer_index] = filename;

                    // add the datatable to the array so that it can be summoned later
                    tüm_katmanlar_datatable[layer_index] = shapefile_datatable;

                    System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
                    if (associatedCheckBox != null)
                    {
                        associatedCheckBox.Checked = true;
                        associatedCheckBox.Visible = true;
                        associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
                    }
                }
                else if (extension == "kml")
                {
                    GMapOverlay kmlOverlay = new GMapOverlay($"kmlOverlay_{layer_index + 1}");
                    gMapControl_stokastik.Overlays.Add(kmlOverlay);

                    DataTable kml_datatable = new DataTable();
                    this.Cursor = Cursors.WaitCursor;
                    await LoadKmlFile(filepath, kmlOverlay, kml_datatable);
                    this.Cursor = Cursors.Default;
                    tüm_katmanlar_array[layer_index] = kmlOverlay;
                    tüm_katmanlar_array_names[layer_index] = filename;
                    tüm_katmanlar_datatable[layer_index] = kml_datatable;

                    System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
                    if (associatedCheckBox != null)
                    {
                        associatedCheckBox.Checked = true;
                        associatedCheckBox.Visible = true;
                        associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
                    }
                }
            }       
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

        private void ShowAttributeTable(DataTable datatable)
        {
            tablo_formu.attribute_table.DataSource = datatable;
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

            if (tüm_katmanlar_array[checkbox_index] != null)
             {
                 tablo_formu.Text = "Veri Tablosu -- " + tüm_katmanlar_array_names[checkbox_index] +
                    " -- " + tüm_katmanlar_datatable[checkbox_index].Rows.Count + " satır -- " +
                    tüm_katmanlar_datatable[checkbox_index].Columns.Count + " sütun";
                 ShowAttributeTable(tüm_katmanlar_datatable[checkbox_index]);
             }
        }

        // display or hide the layers by checkboxes of the stokastik_yuk_tahmini form
        private void stokastik_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox)sender;
            int index = int.Parse(checkBox.Tag.ToString()) - 1;
            
            if (tüm_katmanlar_array[index] != null)
            {
                tüm_katmanlar_array[index].IsVisibile = checkBox.Checked;
                gMapControl_stokastik.Refresh();
            }
        }


        private void rengiDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem rengini_degistir_menu_item = sender as ToolStripMenuItem;

            if (rengini_degistir_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = rengini_degistir_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (checkbox_index < 0 || checkbox_index >= tüm_katmanlar_array.Length)
                {
                    MessageBox.Show("Yanlış katman endeksi!","",
                        MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                GMapOverlay overlay = tüm_katmanlar_array[checkbox_index];
                if (overlay == null)
                {
                    MessageBox.Show("Katmanda herhangi bir data bulunamadı.", 
                        "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ColorDialog colorDialog = new ColorDialog
                {
                    AnyColor = true,
                    AllowFullOpen = true,
                    FullOpen = true
                };

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;
                    byte a = selectedColor.A;
                    byte r = selectedColor.R;
                    byte g = selectedColor.G;
                    byte b = selectedColor.B;

                    // Combine them into a single uint in the order expected by MapWinGIS (ABGR)
                    uint abgr = (uint)(a << 24 | b << 16 | g << 8 | r);

                    // Update the polygons in the overlay
                    foreach (var polygon in overlay.Polygons)
                    {
                        polygon.Stroke = new Pen(Color.FromArgb(a, r, g, b), 3); // Set border color
                        polygon.Fill = new SolidBrush(Color.FromArgb(50, selectedColor)); // Set fill color with transparency
                    }

                    gMapControl_stokastik.Refresh(); // Redraw the map to reflect the changes
                }
            }
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
                kaydet_file_dialog.InitialDirectory = targetDirectory;

                DialogResult kaydet_result = kaydet_file_dialog.ShowDialog();

                if (kaydet_result == DialogResult.OK)
                {
                    string filepath = kaydet_file_dialog.FileName;
                    MapWinGIS.Shapefile shapefile = shapeFileArray_MapWinGIS[checkbox_index];
                    shapefile.EditDeleteField(0);
                    shapefile.EditDeleteField(0);
                    shapefile.SaveAsEx(filepath, false, false);
                    shapefile.Close();
                    shapeFileArray_MapWinGIS[checkbox_index] = null;
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


        // nokta ekleme/çıkarma gibi opsiyonların olduğu menü
        public ContextMenuStrip nokta_menüsü;

        // noktaların eklenip çıkarılacağı liste
        //private List<Shape> pointsList = new List<Shape>();

        // sol tıkla nokta ekleyebilme kontrolü
        public bool adding_points = false;
        int point_index = 0;

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




        /* ------------------------------------------------------------------------------------*/

        //////////////// --------------- BUTTON EVENTS  ------------------------////////////////

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

        private void Arazi_Click(object sender, EventArgs e) // Harita katmanları seçimi - Arazi
        {
            if(Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.MapProvider = GMapProviders.GoogleTerrainMap;
            }

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.MapProvider = GMapProviders.GoogleTerrainMap;
            }   
        }

        private void Harita_Click(object sender, EventArgs e) // Harita katmanları seçimi - Harita
        {
            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.MapProvider = GMapProviders.GoogleMap;
            }

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.MapProvider = GMapProviders.GoogleMap;
            }                 
        }

        private void Uydu_Click(object sender, EventArgs e) // Harita katmanları seçimi - Uydu
        {
            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.MapProvider = GMapProviders.GoogleSatelliteMap;
            }

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.MapProvider = GMapProviders.GoogleSatelliteMap;
            }                 
        }
        private void OSM_Click(object sender, EventArgs e) // Harita katmanları seçimi - OpenStreetMap
        {
            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.MapProvider = GMapProviders.OpenStreetMap;
            }

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.MapProvider = GMapProviders.OpenStreetMap;
            }                     
        }

        private void Google_Earth_Click(object sender, EventArgs e) // Harita katmanları seçimi - Google Earth
        {
            Google_Earth google_earth_form = new Google_Earth();
            google_earth_form.Owner = this;
            google_earth_form.Show();
            google_earth_form.BringToFront();
            google_earth_form.Focus();
        }

        // eğer EA tabı seçilmişe, Stokastik tabındaki polygonları sil, ya da vice versa.
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

        private void Nokta_Ekle_Click(object sender, EventArgs e)
        {
            isSelecting_marker = true;
            isSelecting_polygon = false;
        }

        private void tabloyuGörToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tablo_formu.Show();
            tablo_formu.Focus();
            tablo_formu.BringToFront();
        }

        private void EA_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerEnabled = true;
            Mesafe_ea.Visible = true;
            Mesafe_ea.BringToFront();
            mesafe_metre_ea.Visible = true;
            mesafe_metre_ea.BringToFront();
        }

        private void EA_Kaydır_Click(object sender, EventArgs e)
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

        private void EA_Seç_Click(object sender, EventArgs e)
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

        private void Stokastik_Kaydır_Click(object sender, EventArgs e)
        {
            if (markerOverlay_stokastik != null)
            {
                markerOverlay_stokastik.Markers.Clear();
            }

            if (rulerRoute_stokastik != null)
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

        private void Stokastik_Seç_Click(object sender, EventArgs e)
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

        private void Stokastik_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerEnabled = true;
            Mesafe_stokastik.Visible = true;
            Mesafe_stokastik.BringToFront();
            mesafe_metre_stokastik.Visible = true;
            mesafe_metre_stokastik.BringToFront();
        }

        private void Stokastik_Grid_Oluştur_Click(object sender, EventArgs e)
        {
            isSelecting_grid = true;
            gMapControl_stokastik.Cursor = Cursors.Arrow;
        }


        /* -------------------------------------------------------------------------------------------*/


        //////////////// HARİTA EVENTLERİ - MouseDown, MouseUp, MouseMove, OnMapClick  ////////////////

        private void EA_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void Stokastik_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void EA_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void Stokastik_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void gMapControl_EA_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            // boolean control for marker selection when clicking on the map
            if (isSelecting_marker)
            {
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                markerOverlay_ea.Markers.Add(marker);

                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem = Math.Round(pointClick.Lat, 4),
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
            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if(e.Button == MouseButtons.Left && isSelecting_grid == true)
            {
                gMapControl_stokastik.CanDragMap = false;
                starting_point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);

                bounding_box_overlay = new GMapOverlay("bounding_box_overlay");

                // seçilen alanı kullanıcıya gösterecek olan poligonu oluşturmaya başla
                bounding_box_polygon = new GMapPolygon(new List<PointLatLng>(), "bounding_box_polygon")
                {
                    Stroke = new Pen(Color.White, 3),
                    Fill = new SolidBrush(Color.FromArgb(50, Color.White))
                };

                bounding_box_overlay.Polygons.Add(bounding_box_polygon);
                gMapControl_stokastik.Overlays.Add(bounding_box_overlay);
            }

            // cetvel eventi tanımlaması
            if (e.Button == MouseButtons.Left && isRulerEnabled == true)
            {

                // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
                isRulerActive = true;

                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker_stokastik = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);
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

            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if (e.Button == MouseButtons.Left && isSelecting_grid == true)
            {
                ending_point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);
                UpdateSelectionPolygon();
            }

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

        private void gMapControl_stokastik_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                ending_point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);
                isSelecting_grid = false;
                gMapControl_stokastik.CanDragMap = true;

                // Clear the selection polygon
                //gMapControl_stokastik.Overlays.Remove(bounding_box_overlay);
                gMapControl_stokastik.Refresh();

                Grid_Seçenekler grid_formu = new Grid_Seçenekler();
                grid_formu.Tag = this;
                grid_formu.Owner = this;
                grid_formu.Show();
                grid_formu.BringToFront();
                grid_formu.Focus();
                grid_formu.StartPosition = FormStartPosition.CenterScreen;
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


        /////////////////////////////// ---------------------- /////////////////////////////////

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


        /*---------------------------------- MARKER ADDITION ----------------------------------- */

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


        private void gMapControl_stokastik_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_stokastik)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }




        /*---------------------------------------------------------------------------------------------- */
        /*----------------------------------     CUSTOM METHODS & CLASSES     -------------------------- */

        // grid oluşturma metodu
        public List<NetTopologySuite.Geometries.Polygon> CreateGrid(double xMin, double yMin, double xMax,
                double yMax, double cellSizeLat, double cellSizeLon, out DataTable gridTable,
                Dictionary<NetTopologySuite.Geometries.Polygon, DataRow> polygonAttributes_grid)
        {
            // NTS libraries to create polygons
            var polygons = new List<NetTopologySuite.Geometries.Polygon>();
            var geomFactory = new NetTopologySuite.Geometries.GeometryFactory(); // class that has CreatePolygon() method

            int cell_no = 1;

            // Create a DataTable to hold the grid coordinates
            gridTable = new DataTable();
            gridTable.Columns.Add("Cell_No");
            gridTable.Columns.Add("xMin", typeof(double));
            gridTable.Columns.Add("xMax", typeof(double));
            gridTable.Columns.Add("yMin", typeof(double));
            gridTable.Columns.Add("yMax", typeof(double));

            // create squares of sizes defined by cellSizeLon and cellSizeLat parameters - 100x100 meters etc.
            for (double x = xMin; x < xMax; x += cellSizeLon)
            {
                for (double y = yMin; y < yMax; y += cellSizeLat)
                {
                    var coordinates = new NetTopologySuite.Geometries.Coordinate[]
                    {
                    new NetTopologySuite.Geometries.Coordinate(x, y), // bottom-left corner of the cell
                    new NetTopologySuite.Geometries.Coordinate(x + cellSizeLon, y), // bottom-right corner of the cell
                    new NetTopologySuite.Geometries.Coordinate(x + cellSizeLon, y + cellSizeLat), // top-right corner of the cell
                    new NetTopologySuite.Geometries.Coordinate(x, y + cellSizeLat), // top-left corner of the cell
                    new NetTopologySuite.Geometries.Coordinate(x, y) // closing the loop with the starting point of the cell
                    };

                    // seçilen 5 adet closed loop noktalarından CreatePolygon() metodu ile cell ler oluştur.
                    var polygon = geomFactory.CreatePolygon(coordinates);

                    // her bir oluşturulan hücreyi/poligonu listeye ekle
                    polygons.Add(polygon);
                        
                    // Add the coordinates to the DataTable
                    DataRow row = gridTable.NewRow();
                    row["Cell_No"] = cell_no;
                    row["xMin"] = x;
                    row["xMax"] = x + cellSizeLon;
                    row["yMin"] = y;
                    row["yMax"] = y + cellSizeLat;
                    gridTable.Rows.Add(row);
                    polygonAttributes_grid[polygon] = row;
                    cell_no++;
                }
            }

            // grid e ait oluşturulmuş mxm hücreleri "polygons" listesiyle return et.
            return polygons;
        }

        // creates a grid and adds it onto the map
        public void CreateAndAddGridToMap()
        {
            // Clear previous grid
            gridOverlay.Polygons.Clear();

            entire_grid = new List<NetTopologySuite.Geometries.Polygon>();

            // Define the bounding box and cell size
            double xMin = Math.Min(starting_point.Lng, ending_point.Lng);
            double yMin = Math.Min(starting_point.Lat, ending_point.Lat);
            double xMax = Math.Max(starting_point.Lng, ending_point.Lng);
            double yMax = Math.Max(starting_point.Lat, ending_point.Lat);
            double cellSizeMeters = grid_size; // Degree size of grid cells

            // Convert cell size from meters to degrees
            double cellSizeDegreesLat = MetersToDegreesLatitude(cellSizeMeters);
            double cellSizeDegreesLon = MetersToDegreesLongitude(cellSizeMeters, gMapControl_stokastik.Position.Lat);

            // Create grid and DataTable
            DataTable gridTable;
            var polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();
            var grid = CreateGrid(xMin, yMin, xMax, yMax, cellSizeDegreesLat, cellSizeDegreesLon, 
                out gridTable, polygonAttributes_grid);

            // Add grid polygons to the overlay
            foreach (var polygon in grid)
            {
                AddPolygonToOverlay(polygon, gridOverlay, "gridPolygon", polygonAttributes_grid[polygon]);
            }

            gMapControl_stokastik.Refresh();

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);
            tüm_katmanlar_array[layer_index] = gridOverlay;
            tüm_katmanlar_array_names[layer_index] = layer_index.ToString();
            tüm_katmanlar_datatable[layer_index] = gridTable;

            System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
            if (associatedCheckBox != null)
            {
                associatedCheckBox.Checked = true;
                associatedCheckBox.Visible = true;
                associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
            }

        }

        // oluşturulan poligonları ilgili overlay katmanlarına ekleme kodu
        private void AddPolygonToOverlay(NetTopologySuite.Geometries.Polygon polygon,
            GMapOverlay overlay, string gMapPolygonId, DataRow attributes)
        {
            // oluşturulmuş poligona ait noktaların ekleneceği bir liste oluştur
            List<PointLatLng> points_list = new List<PointLatLng>();

            // poligona ait koordinatları nokta olarak "points" listesine ekle
            foreach (var coord in polygon.Coordinates)
            {
                points_list.Add(new PointLatLng(coord.Y, coord.X));
            }

            GMapPolygon gMapPolygon = new GMapPolygon(points_list, gMapPolygonId)
            {
                Stroke = new Pen(Color.DarkBlue, 3),
                Fill = new SolidBrush(Color.FromArgb(50, Color.DarkBlue))
            };

            overlay.Polygons.Add(gMapPolygon);
            polygonAttributes[gMapPolygon] = attributes;

            if(overlay == gridOverlay)
            {
                entire_grid.Add(polygon);
            }
        }

        private double MetersToDegreesLatitude(double meters)
        {
            const double earthRadius = 6378137; // Earth's radius in meters
            double degreesPerMeter = (1 / ((2 * Math.PI / 360) * earthRadius));
            return meters * degreesPerMeter;
        }

        private double MetersToDegreesLongitude(double meters, double latitude)
        {
            const double earthRadius = 6378137; // Earth's radius in meters
            double degreesPerMeter = (1 / ((2 * Math.PI / 360) * earthRadius)) / Math.Cos(latitude * (Math.PI / 180));
            return meters * degreesPerMeter;
        }

        private void Draw_Polygon(List<PointLatLng> polygonPoints, GMapOverlay polygonOverlay,
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
        /*
        private void CSVYukle(string filepath)
        {
            string[] rows = File.ReadAllLines(filepath);
            double ilkNoktaEnlem = 0;
            double ilkNoktaBoylam = 0;
            bool ilkNoktaBelirlendi = false;
            

            foreach (string satir in rows)
            {
                string[] parcalar = satir.Split(',');
                if (parcalar.Length >= 4 && 
                    double.TryParse(parcalar[0], out double enlem) && 
                    double.TryParse(parcalar[1], out double boylam) &&
                     double.TryParse(parcalar[2], out double binaDem) && 
                     int.TryParse(parcalar[3], out int aboneSayisi))
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
                    marker.ToolTipText = Path.GetFileName(filepath); // Dosya adını ToolTipText olarak ayarla
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
        }*/

        // grid oluşturmak için mouse'u basılı tutup çekerken aynı zamanda seçilen alanı
        // gösteren poligonu da güncelle
        private void UpdateSelectionPolygon()
        {
            if (bounding_box_polygon != null)
            {
                var points = new List<PointLatLng>
                    {
            new PointLatLng(starting_point.Lat, starting_point.Lng), // aşağı çekerken: top-left nokta
            new PointLatLng(starting_point.Lat, ending_point.Lng), // aşağı çekerken: top-right nokta
            new PointLatLng(ending_point.Lat, ending_point.Lng), // aşağı çekerken: bottom-right nokta
            new PointLatLng(ending_point.Lat, starting_point.Lng), // aşağı çekerken: bottom-left nokta
            new PointLatLng(starting_point.Lat, starting_point.Lng) // başlangıç noktasına geri dön
                    };

                bounding_box_polygon.Points.Clear(); // halihazırda oluşturulmuş poligonu sil
                bounding_box_polygon.Points.AddRange(points); // yukarıdaki 5 nokta ile yeni poligonu oluştur
                bounding_box_overlay.Polygons.Add(bounding_box_polygon);
                gMapControl_stokastik.Refresh(); // harita objesini güncelle
            }
        }

        // cetvel ile seçilen2 nokta arasındaki mesafeyi metre cinsinden göster
        private void CalculateDistance(GMapControl gmap, System.Windows.Forms.Label mesafe_metre, List<PointLatLng> rulerPoints)
        {
            if (rulerPoints.Count == 2)
            {
                double meter_distance = Math.Round(gmap.MapProvider.Projection.GetDistance(rulerPoints[0], rulerPoints[1]) * 1000, 3);
                mesafe_metre.Text = meter_distance.ToString() + " metre";
            }
        }

        // stokastik haritası için cetvel route'unu çiz
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

        private void Stokastik_Alan_Ölç_Click(object sender, EventArgs e)
        {
            isSelecting_marker = false;
            isSelecting_polygon = false;
            isSelecting_grid = false;
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

        //highlight the polygon which is double clicked on
        private void HighlightPolygon(GMapPolygon polygon)
        {
            if (gridOverlay.Polygons.Contains(polygon))
            {
                // Reset previous selected polygon
                foreach (var poly in gridOverlay.Polygons)
                {
                    poly.Stroke = new Pen(Color.DarkBlue, 3);
                    poly.Fill = new SolidBrush(Color.FromArgb(50, Color.DarkBlue));
                }

                // Highlight new selected polygon
                polygon.Stroke = new Pen(Color.LawnGreen, 3);
                polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.LawnGreen));

                gMapControl_stokastik.Refresh();
            }
            else
            {
                // Reset previous selected polygon to the previously defined default map colors
                if (selectedPolygon != null)
                {
                    selectedPolygon.Stroke = new Pen(Color.DarkBlue, 3);
                    selectedPolygon.Fill = new SolidBrush(Color.FromArgb(50, Color.DarkBlue));
                }

                // Highlight new selected polygon with a different border and fill color
                selectedPolygon = polygon;
                selectedPolygon.Stroke = new Pen(Color.LawnGreen, 3);
                selectedPolygon.Fill = new SolidBrush(Color.FromArgb(50, Color.LawnGreen));

                gMapControl_stokastik.Refresh();
            }

        }

        // show information about polygons when double-clicking on the map
        private void gMapControl_stokastik_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var polygon in tüm_katmanlar_array[layer_index].Polygons)
                {
                    if (IsPointInPolygon(pointClick, polygon))
                    {
                        HighlightPolygon(polygon);

                        if(polygonAttributes.TryGetValue(polygon, out DataRow row))
                        {
                            ShowAttributeRow(row);
                            tablo_formu.Show();
                        }
                    }
                }
            }
        }

        // show just the single row whenever a polygon is clicked on which corresponds to its row
        private void ShowAttributeRow(DataRow row)
        {
            DataTable singleRowTable = row.Table.Clone(); // Clone the structure of the original table
            singleRowTable.ImportRow(row); // Import the specific row into the new table
            ShowAttributeTable(singleRowTable); 
        }

        // show information about polygons when double-clicking on the map
        private void gMapControl_EA_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var polygon in tüm_katmanlar_array[layer_index].Polygons)
                {
                    if (IsPointInPolygon(pointClick, polygon))
                    {
                        HighlightPolygon(polygon);

                        if (polygonAttributes.TryGetValue(polygon, out DataRow row))
                        {
                            ShowAttributeRow(row);
                            tablo_formu.Show();
                        }
                    }
                }
            }
        }

        //method to check whether the point that is double clicked on the map is in a polygon
        private bool IsPointInPolygon(PointLatLng point, GMapPolygon polygon)
        {
            int i, j = polygon.Points.Count - 1;
            bool oddNodes = false;

            for (i = 0; i < polygon.Points.Count; i++)
            {
                if (polygon.Points[i].Lat < point.Lat && polygon.Points[j].Lat >= point.Lat
                || polygon.Points[j].Lat < point.Lat && polygon.Points[i].Lat >= point.Lat)
                {
                    if (polygon.Points[i].Lng + (point.Lat - polygon.Points[i].Lat) / (polygon.Points[j].Lat - polygon.Points[i].Lat) * (polygon.Points[j].Lng - polygon.Points[i].Lng) < point.Lng)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

    }
}


