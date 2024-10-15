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
using SharpMap.Data.Providers;
using System.Drawing.Drawing2D;
using System.Threading;

namespace SLF
{
    public partial class ModülFormu : Form
    {

        private double startX = 0, startY = 0;
        public int slfStartYear = 0, slfEndYear = 0;

        // form objeleri
        public GirişFormu gir1;
        private GirdiModülü girdiModülü;
        private Dictionary<string, GirdiModülü> girdiModülleri = new Dictionary<string, GirdiModülü> {
            {"Abone Verileri", new AboneVerileri()},
            {"DEK Verileri", new DEKModulu()},
            {"DTR Verileri", new DTRModulu()},
            {"EA Şarj Verileri", new EASarjModulu()},
            {"Ekonometrik Yük Tahmini Verileri", new EkonometrikYukTahminiModulu()},
            {"Fider Verileri", new FiderVerileri()},
            {"İmar Verileri", new GirdiModülü()},
            {"Enerji Müsaadeleri Verileri", new EnerjiMusaadeleri()},
            {"Yeni Projelendirilmiş DTR Verileri", new YeniProjelendirilmisDTR()},
        };
        public Fonksiyon_Oluştur fonksiyonFormu;

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

        // variables to be used to create polygonspolygonOverlay_stokastik
        private GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        public GMapOverlay polygonOverlay_stokastik;
        private List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        //private List<PoligonVeri> poligonlar_ea = new List<PoligonVeri>();
        private List<PointLatLng> polygonPoints_stokastik = new List<PointLatLng>();
        //private List<PoligonVeri> poligonlar_stokastik = new List<PoligonVeri>();

        // variables to be used to create a grid
        public GMapOverlay bounding_box_overlay;
        private GMapOverlay gridOverlay = new GMapOverlay("grid");
        private GMapPolygon bounding_box_polygon;
        public int grid_size = 250;
        public bool isSelecting_grid = false;
        private PointLatLng starting_point;
        private PointLatLng ending_point;

        // Get the user's profile path
        public string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string targetDirectory;

        // boolean variable to control the polygon selection by mouse down event
        private bool isSelecting_polygon = false;

        // boolean variable to control the marker/point selection by mouse down event
        private bool isSelecting_marker = false;

        // X and Y coordinates of the center location of the gMapControl object
        public string centerX;
        public string centerY;

        // create a list of gMapOverlay's that will hold the imported vector files
        public GMapOverlay[] tüm_katmanlar_array;
        public MapWinGIS.Shapefile[] shapeFileArray_MapWinGIS;
        public DataTable[] tüm_katmanlar_datatable;
        public string[] tüm_katmanlar_array_names;

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

        // variables to be used in the "join attributes by location" functionality
        public int firstLayerToJoin;
        public int secondLayerToJoin;
        public string firstLayerName;
        public string secondLayerName;

        // variables that are to be used to export .kml files
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_kml;
        public Dictionary<GMapRoute, DataRow> routeAttributes_kml;

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

        public ModülFormu()
        {

            InitializeComponent();
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);
            

            SortTabPagesAlphabetically(Modül_Tabları, true);
            ModuleTabPanel.Paint += new PaintEventHandler(ModuleTabPanel_Paint);
            //HeaderPanel.Paint += new PaintEventHandler(HeaderPanel_Paint);

            Modül_Tabları.SelectedTab = tab_girdi;

            tüm_katmanlar_array_names = new string[13];
            tüm_katmanlar_array = new GMapOverlay[13];
            shapeFileArray_MapWinGIS = new MapWinGIS.Shapefile[13];
            tüm_katmanlar_datatable = new DataTable[13];

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

            //
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
            for (int i = 1; i < shapefile_reader.DbaseHeader.NumFields; i++)
            {
                row["Row_No"] = row_cnt;
                row[i] = shapefile_reader.GetValue(i); // get the value of all columns for the i-th row
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
                shapefile_datatable = LoadAttributeTable(row, tablo_formu.attribute_table,
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
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            // Convert GMapOverlay to MapWinGIS.Shapefile
            MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(shapeFileOverlay);
            shapeFileArray_MapWinGIS[layer_index] = myShapefile;

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Refresh();
            }
            else if (Modül_Tabları.SelectedTab == tab_ea)
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
                if (folder != null)
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

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Refresh();
            }

            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Refresh();
            }
        }

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
                Stroke = new Pen(overlayColors[layer_index].BorderColor, 3),
                Fill = new SolidBrush(overlayColors[layer_index].FillColor)
            };

            overlay.Polygons.Add(gMapPolygon);
            polygonAttributes[gMapPolygon] = attributes;

            if (overlay == gridOverlay)
            {
                entire_grid.Add(polygon);
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
                Stroke = new Pen(overlayColors[layer_index].BorderColor, 3),
                Fill = new SolidBrush(overlayColors[layer_index].FillColor)
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
                Stroke = new Pen(overlayColors[layer_index].BorderColor, 3)
            };
            overlay.Routes.Add(route);
        }


        public MapWinGIS.Shapefile ConvertKmlToShapefile(GMapOverlay overlay)
        {
            var shapefile = new MapWinGIS.Shapefile();
            shapefile.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);

            // Add fields from the first polygon's attributes (if any)
            if (polygonAttributes.Count > 0)
            {
                var firstPolygon = polygonAttributes.Keys.First();
                var firstRow = polygonAttributes[firstPolygon];
                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 10, 10);
                }
            }

            // Add polygons to shapefile
            foreach (var polygon in overlay.Polygons)
            {
                var shape = new MapWinGIS.Shape();
                shape.Create(ShpfileType.SHP_POLYGON);

                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    var point = new MapWinGIS.Point
                    {
                        x = polygon.Points[i].Lng,
                        y = polygon.Points[i].Lat
                    };
                    shape.InsertPoint(point, ref i);
                }

                int shapeIndex = shapefile.NumShapes;
                shapefile.EditInsertShape(shape, ref shapeIndex);

                if (polygonAttributes.TryGetValue(polygon, out DataRow row))
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        shapefile.EditCellValue(i, shapeIndex, row[i].ToString());
                    }
                }
            }

            // Add fields from the first route's attributes (if any)
            if (routeAttributes_kml.Count > 0 && shapefile.NumFields == 0)
            {
                var firstRoute = routeAttributes_kml.Keys.First();
                var firstRow = routeAttributes_kml[firstRoute];
                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 10, 10);
                }
            }

            // Add routes to shapefile
            foreach (var route in overlay.Routes)
            {
                var shape = new MapWinGIS.Shape();
                shape.Create(ShpfileType.SHP_POLYLINE);

                for (int i = 0; i < route.Points.Count; i++)
                {
                    var point = new MapWinGIS.Point
                    {
                        x = route.Points[i].Lng,
                        y = route.Points[i].Lat
                    };
                    shape.InsertPoint(point, ref i);
                }

                int shapeIndex = shapefile.NumShapes;
                shapefile.EditInsertShape(shape, ref shapeIndex);

                if (routeAttributes_kml.TryGetValue(route, out DataRow row))
                {
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        shapefile.EditCellValue(i, shapeIndex, row[i].ToString());
                    }
                }
            }

            return shapefile;
        }

        public void ExportOverlayToKml(GMapOverlay overlay, string filePath)
        {
            var kmlDocument = new Document();
            var kml = new Kml { Feature = kmlDocument };

            if (overlay.Polygons != null)
            {
                foreach (var polygon in overlay.Polygons)
                {
                    var kmlPolygon = CreateKmlPolygon(polygon);
                    kmlDocument.AddFeature(kmlPolygon);
                }
            }

            if (overlay.Routes != null)
            {
                foreach (var route in overlay.Routes)
                {
                    var kmlLineString = CreateKmlLineString(route);
                    kmlDocument.AddFeature(kmlLineString);
                }
            }

            using (var stream = File.OpenWrite(filePath))
            {
                var serializer = new Serializer();
                serializer.Serialize(kml, stream);
            }
        }

        private SharpKml.Dom.Placemark CreateKmlPolygon(GMapPolygon gMapPolygon)
        {
            var kmlPolygon = new SharpKml.Dom.Polygon();
            var outerBoundary = new SharpKml.Dom.OuterBoundary();
            var linearRing = new SharpKml.Dom.LinearRing();

            if (linearRing.Coordinates == null)
            {
                linearRing.Coordinates = new CoordinateCollection();
            }

            if (gMapPolygon.Points != null && gMapPolygon.Points.Count > 0)
            {
                foreach (var point in gMapPolygon.Points)
                {
                    linearRing.Coordinates.Add(new SharpKml.Base.Vector(point.Lat, point.Lng));
                }
            }

            outerBoundary.LinearRing = linearRing;
            kmlPolygon.OuterBoundary = outerBoundary;

            var placemark = new SharpKml.Dom.Placemark
            {
                Geometry = kmlPolygon,
                Name = gMapPolygon.Name
            };

            return placemark;
        }

        private SharpKml.Dom.Placemark CreateKmlLineString(GMapRoute gMapRoute)
        {
            var kmlLineString = new SharpKml.Dom.LineString();

            foreach (var point in gMapRoute.Points)
            {
                kmlLineString.Coordinates.Add(new SharpKml.Base.Vector(point.Lat, point.Lng));
            }

            var placemark = new SharpKml.Dom.Placemark
            {
                Geometry = kmlLineString,
                Name = gMapRoute.Name
            };

            return placemark;
        }


        // stokastik dosya seçimi butonu
        private async void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
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

                    tüm_katmanlar_array[layer_index] = kmlOverlay;
                    tüm_katmanlar_array_names[layer_index] = filename;
                    tüm_katmanlar_datatable[layer_index] = kml_datatable;

                    // convert .kml overlay into a MapWinGIS.Shapefile object
                    polygonAttributes_kml = new Dictionary<GMapPolygon, DataRow>();
                    routeAttributes_kml = new Dictionary<GMapRoute, DataRow>();
                    MapWinGIS.Shapefile shapefile = ConvertKmlToShapefile(kmlOverlay);
                    shapeFileArray_MapWinGIS[layer_index] = shapefile;

                    this.Cursor = Cursors.Default;

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
                case 10: return checkBox19;
                case 11: return checkBox20;
                case 12: return checkBox21;
                default: return null;
            }
        }

        // stokastik haritasına ait checkboxların initializationları
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
            checkBox19.Tag = 11;
            checkBox20.Tag = 12;
            checkBox21.Tag = 13;

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
            checkBox19.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox20.CheckedChanged += stokastik_checkBox_CheckedChanged;
            checkBox21.CheckedChanged += stokastik_checkBox_CheckedChanged;

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
            checkBox19.MouseDown += stokastik_checkBox_MouseDown;
            checkBox20.MouseDown += stokastik_checkBox_MouseDown;
            checkBox21.MouseDown += stokastik_checkBox_MouseDown;

            checkBox9.ForeColor = overlayColors[0].BorderColor;
            checkBox10.ForeColor = overlayColors[1].BorderColor;
            checkBox11.ForeColor = overlayColors[2].BorderColor;
            checkBox12.ForeColor = overlayColors[3].BorderColor;
            checkBox13.ForeColor = overlayColors[4].BorderColor;
            checkBox14.ForeColor = overlayColors[5].BorderColor;
            checkBox15.ForeColor = overlayColors[6].BorderColor;
            checkBox16.ForeColor = overlayColors[7].BorderColor;
            checkBox17.ForeColor = overlayColors[8].BorderColor;
            checkBox18.ForeColor = overlayColors[9].BorderColor;
            checkBox19.ForeColor = overlayColors[10].BorderColor;
            checkBox20.ForeColor = overlayColors[11].BorderColor;
            checkBox21.ForeColor = overlayColors[12].BorderColor;

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

        // define default colors for each overlay object
        private (Color BorderColor, Color FillColor)[] overlayColors = new (Color, Color)[]
        {
            (Color.Red, Color.FromArgb(50, Color.Red)),
            (Color.Blue, Color.FromArgb(50, Color.Blue)),
            (Color.Green, Color.FromArgb(50, Color.Green)),
            (Color.DarkGoldenrod, Color.FromArgb(50, Color.DarkGoldenrod)),
            (Color.Purple, Color.FromArgb(50, Color.Purple)),
            (Color.Orange, Color.FromArgb(50, Color.Orange)),
            (Color.Pink, Color.FromArgb(50, Color.Pink)),
            (Color.Brown, Color.FromArgb(50, Color.Brown)),
            (Color.Gray, Color.FromArgb(50, Color.Gray)),
            (Color.Cyan, Color.FromArgb(50, Color.Cyan)),
            (Color.DarkTurquoise, Color.FromArgb(50, Color.DarkTurquoise)),
            (Color.Black, Color.FromArgb(50, Color.Black)),
            (Color.Violet, Color.FromArgb(50, Color.Violet))
        };

        private void rengiDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem rengini_degistir_menu_item = sender as ToolStripMenuItem;

            if (rengini_degistir_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = rengini_degistir_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (checkbox_index < 0 || checkbox_index >= tüm_katmanlar_array.Length)
                {
                    MessageBox.Show("Yanlış katman endeksi!", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // Combine them into a single uint in the order expected by the Color class
                    uint abgr = (uint)(a << 24 | b << 16 | g << 8 | r);

                    // Update the polygons in the overlay
                    foreach (var polygon in overlay.Polygons)
                    {
                        polygon.Stroke = new Pen(Color.FromArgb(a, r, g, b), 3); // Set border color
                        polygon.Fill = new SolidBrush(Color.FromArgb(50, selectedColor)); // Set fill color with transparency
                    }

                    checkBox.ForeColor = Color.FromArgb(a, r, g, b);

                    gMapControl_stokastik.Refresh(); // Redraw the map to reflect the changes
                }
            }
        }

        // check whether the shapefile being exported is a grid shapefile or another shapefile
        public bool HasField(MapWinGIS.Shapefile shapefile, string fieldName)
        {
            for (int i = 0; i < shapefile.NumFields; i++)
            {
                var field = shapefile.get_Field(i);
                if (field.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
                    string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                    string extension = filename.Substring(filename.Length - 3);

                    if (extension == "shp")
                    {
                        MapWinGIS.Shapefile shapefile = shapeFileArray_MapWinGIS[checkbox_index];

                        /*if (!HasField(shapefile, "xMin") && !HasField(shapefile, "yMax"))
                        {
                            shapefile.EditDeleteField(0);
                        }*/

                        int fieldIndex;

                        /*fieldIndex = shapefile.get_FieldIndexByName("Cell_No");

                        if(fieldIndex  != -1)
                        {
                            MessageBox.Show("a");
                            shapefile.EditDeleteField(fieldIndex);
                        }      */

                        fieldIndex = shapefile.get_FieldIndexByName("MWShapeID");

                        if (fieldIndex != -1)
                        {
                            shapefile.EditDeleteField(fieldIndex);
                        }

                        shapefile.SaveAsEx(filepath, false, false);
                        shapefile.Close();
                        shapeFileArray_MapWinGIS[checkbox_index] = null;
                        MessageBox.Show("Dosya başarıyla kaydedildi.");
                    }
                    else if (extension == "kml")
                    {
                        GMapOverlay kmlOverlay = new GMapOverlay();
                        kmlOverlay = tüm_katmanlar_array[checkbox_index];

                        ExportOverlayToKml(kmlOverlay, filepath);
                        MessageBox.Show("Dosya başarıyla kaydedildi.");
                    }

                }
            }
        }

        private void temizleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem delete_menu_item = sender as ToolStripMenuItem;

            if (delete_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = delete_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (tüm_katmanlar_array[checkbox_index] != null)
                {
                    string katman_ismi = tüm_katmanlar_array_names[checkbox_index];

                    DialogResult temizle_result = MessageBox.Show(katman_ismi + " isimli katman " +
                        "silinecektir. Emin misiniz?", "", MessageBoxButtons.YesNo);

                    if (temizle_result == DialogResult.Yes)
                    {

                        if (Modül_Tabları.SelectedTab == tab_ea)
                        {
                            gMapControl_EA.Overlays.Remove(tüm_katmanlar_array[checkbox_index]);
                            gMapControl_EA.Refresh();
                        }

                        if (Modül_Tabları.SelectedTab == tab_stokastik)
                        {
                            gMapControl_stokastik.Overlays.Remove(tüm_katmanlar_array[checkbox_index]);
                            gMapControl_stokastik.Refresh();
                        }

                        tüm_katmanlar_array[checkbox_index].Dispose();
                        tüm_katmanlar_array[checkbox_index] = null;
                        checkBox.Checked = false;
                        checkBox.Visible = false;

                    }
                }
            }
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
                //textBox1.CausesValidation = true; //this line can be removed textbox1 not relevant to checkbox6
            }
            else
            {
                panel1.Visible = false;
            }
        }
        private void PredictionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PredictionCheckBox.Checked)
            {
                ELFTablePanel.Visible = true;
            }
            else
            {
                ELFTablePanel.Visible = false;
            }
        }




        /* ------------------------------------------------------------------------------------*/

        //////////////// --------------- BUTTON EVENTS  ------------------------////////////////

        // Yük haritası modülündeki Google Earth butonu
        private void button3_Click_1(object sender, EventArgs e)
        {
            string url = "https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu";
            webView21.CoreWebView2.Navigate(url);
        }

        // Yük haritası modülündeki OSM butonu
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

        // haritalardaki arazi katmanı
        private void Arazi_Click(object sender, EventArgs e) // Harita katmanları seçimi - Arazi
        {
            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.MapProvider = GMapProviders.GoogleTerrainMap;
            }

            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.MapProvider = GMapProviders.GoogleTerrainMap;
            }
        }

        // haritalardaki harita katmanı
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

        // haritalardaki uydu katmanı
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

        // haritalardaki OSM katmanı
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

        // haritalardaki Google Earth katmanı
        private void Google_Earth_Click(object sender, EventArgs e) // Harita katmanları seçimi - Google Earth
        {
            Google_Earth google_earth_form = new Google_Earth();
            google_earth_form.Owner = this;
            google_earth_form.Show();
            google_earth_form.BringToFront();
            google_earth_form.Focus();
        }

        private void CreateKMLFile(string latitude, string longitude)
        {
            string kmlContent = $@"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns='http://www.opengis.net/kml/2.2'>
    <Placemark>
        <name>Center Location</name>
        <LookAt>
            <longitude>{longitude}</longitude>
            <latitude>{latitude}</latitude>
            <altitude>0</altitude>
            <heading>0</heading>
            <tilt>0</tilt>
            <range>2000</range>
            <altitudeMode>relativeToGround</altitudeMode>
        </LookAt>
    </Placemark>
</kml>";

            string kmlFilePath = Path.Combine(Path.GetTempPath(), "center_location.kml");

            File.WriteAllText(kmlFilePath, kmlContent);
        }

        private void Google_Earth_Desktop_Click(object sender, EventArgs e)
        {
            string google_earth_path = @"C:\Program Files\Google\Google Earth Pro\client\googleearth.exe";

            try
            {
                // Ensure centerX and centerY are not null or empty
                if (!string.IsNullOrEmpty(centerX) && !string.IsNullOrEmpty(centerY))
                {
                    // Create the KML file with the current coordinates
                    CreateKMLFile(centerY, centerX); // Note: Latitude (Y) first, then Longitude (X)

                    // Path to the created KML file
                    string kmlFilePath = Path.Combine(Path.GetTempPath(), "center_location.kml");

                    // Start the process with the KML file as argument
                    Process.Start(google_earth_path, kmlFilePath);
                }
                else
                {
                    MessageBox.Show("Bir sorun oluştu. Lütfen haritada başka bir yeri seçiniz.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Google Earth Desktop uygulaması açılamadı. Lütfen ilgili yüklemenin bilgi" +
                    "sayarınızda halihazırda yüklü olduğunu teyit ediniz!   >" +
                    $"An error occurred: {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
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

        // when clicked on "Tabloyu Gör" toolStripMenuItem applied onto the layers added
        // onto the maps, open up their attribute table
        private void tabloyuGörToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tablo_formu.Show();
            tablo_formu.Activate();
        }

        // show the list of the available functions when clicked on the function button
        private void Stokastik_Fonksiyonlar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Fonksiyon.Show(Cursor.Position);
            }
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
            isSelecting_polygon = false;
            gMapControl_stokastik.Cursor = Cursors.Hand;
            mesafe_metre_stokastik.Visible = false;
            mesafe_metre_stokastik.Text = "";
            Mesafe_stokastik.Visible = false;
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
            isSelecting_polygon = true;
            gMapControl_stokastik.Cursor = Cursors.Arrow;
            mesafe_metre_stokastik.Visible = false;
            mesafe_metre_stokastik.Text = "";
            Mesafe_stokastik.Visible = false;
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
            Grid_Seçenekler grid_formu = new Grid_Seçenekler();
            grid_formu.Tag = this;
            grid_formu.Owner = this;
            grid_formu.Show();
            grid_formu.Activate();
            grid_formu.StartPosition = FormStartPosition.CenterParent;
        }


        /* -------------------------------------------------------------------------------------------*/


        //////////////// HARİTA EVENTLERİ - MouseDown, MouseUp, MouseMove, OnMapClick  ////////////////

        private void EA_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show the ContextMenuStrip at the mouse position
                ContextMenuStrip_Poligon.Show(Cursor.Position);
                Console.WriteLine("buradayım");
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
            if (e.Button == MouseButtons.Left)
            {
                // İşaretleyici seçimi kontrolü
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

                // Poligon seçimi kontrolü
                if (isSelecting_polygon)
                {
                    polygonPoints_ea.Add(pointClick);
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                    markerOverlay_stokastik.Markers.Add(marker);

                    if (polygonOverlay_ea != null)
                    {
                        gMapControl_stokastik.Overlays.Remove(polygonOverlay_ea);
                    }

                    layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

                    // Dizide boş yer olup olmadığını kontrol et
                    if (layer_index == -1)
                    {
                        MessageBox.Show("En fazla katman sayısına ulaşıldı. Daha fazla katman ekleyemezsiniz.");
                        return;
                    }

                    polygonOverlay_ea = new GMapOverlay("polygonOverlay_" + layer_index.ToString());
                    gMapControl_EA.Overlays.Add(polygonOverlay_ea);
                    gMapControl_EA.Refresh();

                    // Eğer 3 veya daha fazla nokta varsa, poligon çiz
                    if (polygonPoints_ea.Count >= 3)
                    {
                        Draw_Polygon(polygonPoints_ea, polygonOverlay_ea, gMapControl_EA);

                        double area = CalculatePolygonArea(polygonPoints_ea);

                        mesafe_metre_ea.Visible = true;
                        Mesafe_ea.Visible = true;
                        Mesafe_ea.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                    }
                }
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
            if (e.Button == MouseButtons.Left && isSelecting_grid == true)
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

            if (e.Button == MouseButtons.Right && isSelecting_polygon)
            {
                if (markerOverlay_stokastik.Markers != null)
                {
                    markerOverlay_stokastik.Markers.Clear();
                }

                if (polygonPoints_stokastik != null)
                {
                    polygonPoints_stokastik.Clear();
                }

                if (polygonOverlay_stokastik != null)
                {
                    polygonOverlay_stokastik.Clear();
                }

                Mesafe_stokastik.Visible = false;
                mesafe_metre_stokastik.Visible = false;

                gMapControl_stokastik.Refresh();
            }
        }

        private void gMapControl_stokastik_MouseMove(object sender, MouseEventArgs e)
        {

            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl_stokastik.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();

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

                // Clear the selection polygon and refresh the map
                gMapControl_stokastik.Overlays.Remove(bounding_box_overlay);
                AddGridToMap();
                gMapControl_stokastik.Refresh();
            }
        }

        // open up the Fonksiyonlar formu and populate its comboboxes with the specified array values
        private void katman_birleştir_Click(object sender, EventArgs e)
        {
            if (tüm_katmanlar_array_names[0] != null && tüm_katmanlar_array_names[1] != null)
            {
                fonksiyonFormu = new Fonksiyon_Oluştur()
                {
                    Tag = this,
                    Owner = this
                };

                foreach (string layers in tüm_katmanlar_array_names)
                {
                    if (layers != null)
                    {
                        fonksiyonFormu.comboBox_fonksiyonlar_1.Items.Add(layers);
                        fonksiyonFormu.comboBox_fonksiyonlar_2.Items.Add(layers);
                    }
                }
                fonksiyonFormu.comboBox_fonksiyonlar_1.Text = tüm_katmanlar_array_names[0];
                fonksiyonFormu.comboBox_fonksiyonlar_2.Text = tüm_katmanlar_array_names[1];

                // create an example row so that the columns of the second table could be displayed
                // in the list box
                DataRow example_row = tüm_katmanlar_datatable[1].NewRow();

                foreach (var columns in example_row.Table.Columns)
                {
                    SuspendLayout();
                    fonksiyonFormu.tum_sutunlar.Items.Add(columns.ToString());
                    ResumeLayout();
                }

                fonksiyonFormu.Show();
                fonksiyonFormu.BringToFront();
                fonksiyonFormu.Focus();
            }
            else
            {
                MessageBox.Show("Bu işlemi yapabilmek için en az 2 adet katman seçmelisiniz.",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gMapControl_stokastik_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
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
                if (isSelecting_polygon)
                {
                    polygonPoints_stokastik.Add(pointClick);
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                    markerOverlay_stokastik.Markers.Add(marker);

                    if (polygonOverlay_stokastik != null)
                    {
                        gMapControl_stokastik.Overlays.Remove(polygonOverlay_stokastik);
                    }

                    layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);
                    polygonOverlay_stokastik = new GMapOverlay("polygonOverlay_" + layer_index.ToString());
                    gMapControl_stokastik.Overlays.Add(polygonOverlay_stokastik);
                    gMapControl_stokastik.Refresh();

                    // eğer gmapControl_OnMapClick event'i ile 2 den fazla nokta seçilirse,
                    // bu noktalar arasında bir poligon çiz
                    if (polygonPoints_stokastik.Count >= 3)
                    {

                        Draw_Polygon(polygonPoints_stokastik, polygonOverlay_stokastik, gMapControl_stokastik);

                        double area = CalculatePolygonArea(polygonPoints_stokastik);

                        mesafe_metre_stokastik.Visible = true;
                        Mesafe_stokastik.Visible = true;
                        Mesafe_stokastik.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                    }
                }
            }
        }

        private void Draw_Polygon(List<PointLatLng> polygonPoints, GMapOverlay polygonOverlay, GMapControl gmap)
        {
            // bu noktalar arasında poligon çiz, mavi ile işaretle, ve de 
            // polygonOverlay katmanına ekle.
            string poligonIsim = $"Poligon_{polygonOverlay.Polygons.Count + 1}";
            GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim)
            {
                Stroke = new Pen(Color.DarkBlue, 3)
            };

            polygonOverlay.Polygons.Clear();
            polygonOverlay.Polygons.Add(polygon);
            gmap.Refresh();
        }

        private void Poligon_Kaydet_Click(object sender, EventArgs e)
        {

            if (polygonOverlay_stokastik != null && polygonOverlay_stokastik.Polygons.Count != 0)
            {
                markerOverlay_stokastik.Markers.Clear();

                layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);
                GMapOverlay overlay_to_be_saved = polygonOverlay_stokastik;
                tüm_katmanlar_array[layer_index] = overlay_to_be_saved;
                tüm_katmanlar_array_names[layer_index] = "Polygon_" + "_" + (layer_index + 1).ToString();

                // Convert gridOverlay to MapWinGIS.Shapefile so that it could be exported by the MapWinGIS
                // built-in function SaveAsEx
                MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(overlay_to_be_saved);
                shapeFileArray_MapWinGIS[layer_index] = myShapefile;

                // Create DataTable and store it
                DataTable polygonDataTable = CreatePolygonDataTable(polygonPoints_stokastik, layer_index);
                tüm_katmanlar_datatable[layer_index] = polygonDataTable;

                System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
                if (associatedCheckBox != null)
                {
                    associatedCheckBox.Checked = true;
                    associatedCheckBox.Visible = true;
                    associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
                }

                MessageBox.Show("Poligon kaydedildi.");
                Mesafe_stokastik.Visible = false;
                mesafe_metre_stokastik.Visible = false;
                isSelecting_polygon = false;

                // Prepare a new overlay for future use
                polygonOverlay_stokastik = null;
                polygonPoints_stokastik.Clear();
            }
            else
            {
                MessageBox.Show("Herhangi bir poligon çizilmemiştir. Lütfen öncelikle bir poligon çiziniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private DataTable CreatePolygonDataTable(List<PointLatLng> polygonPoints, int polygonId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PolygonID", typeof(int));
            dt.Columns.Add("Coordinates", typeof(string));
            dt.Columns.Add("Area_Size(m2)", typeof(string));

            // Create a string representation of the coordinates
            string coordinates = string.Join(", ", polygonPoints.Select(p => $"({p.Lat}, {p.Lng})"));

            double area = CalculatePolygonArea(polygonPoints_stokastik);

            // Create a new row
            DataRow row = dt.NewRow();
            row["PolygonID"] = polygonId;
            row["Coordinates"] = coordinates;
            row["Area_Size(m2)"] = Math.Round(area, 0).ToString();
            dt.Rows.Add(row);

            return dt;
        }

        private double CalculatePolygonArea(List<PointLatLng> points)
        {
            double area = 0;

            for (int i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[(i + 1) % points.Count];

                area += Deg2Rad(p2.Lng - p1.Lng) *
                        (2 + Math.Sin(Deg2Rad(p1.Lat)) + Math.Sin(Deg2Rad(p2.Lat)));
            }

            area = area * 6378137 * 6378137 / 2.0;

            return Math.Abs(area); // In square meters
        }

        private double Deg2Rad(double degrees)
        {
            return degrees * Math.PI / 180.0;
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
        public void AddGridToMap()
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
            tüm_katmanlar_array_names[layer_index] = "Grid_" + grid_size + "_" + (layer_index + 1).ToString();
            tüm_katmanlar_datatable[layer_index] = gridTable;

            // Convert gridOverlay to MapWinGIS.Shapefile so that it could be exported by the MapWinGIS
            // built-in function SaveAsEx
            MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(gridOverlay);
            shapeFileArray_MapWinGIS[layer_index] = myShapefile;

            System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
            if (associatedCheckBox != null)
            {
                associatedCheckBox.Checked = true;
                associatedCheckBox.Visible = true;
                associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
            }
        }

        // methods to convert meters info to longitude/latitude info to be used to create grids of mxm size
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

        /*private void UpdateListBox()
        {
            EA_list_box.Items.Clear();
            foreach (var file in loadedFiles)
            {
                EA_list_box.Items.Add(file.file_name);
            }
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

//THESE METHODS ARE ADDED FOR ELF METHOD SELECTION FROM METHODS FORM

        private string selectedMethod;  // Store the method
        public List<TabPage> hiddenTabs = new List<TabPage>();

        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {
            InitializeComponent();
            this.selectedMethod = selectedMethod;  // Store the method
            if (!string.IsNullOrEmpty(tabToSelect))
            {
                InitializeTabs(tabToSelect);
            }
            else
            {
                InitializeFormBasedOnMethod();
            }
        }

        private void InitializeTabs(string tabToSelect)
        {
            // Select the specific tab and hide others
            if (tabToSelect == "tab_girdi")
            {
                Modül_Tabları.SelectedTab = Modül_Tabları.TabPages["tab_girdi"];
                veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
                veri_listesi_seçimi.Enabled = false;

                HideOtherTabs("tab_girdi");
            }
        }

        private void HideOtherTabs(string tabToKeep)
        {
            // Hide all tabs except the specified one
            foreach (TabPage tabPage in Modül_Tabları.TabPages.Cast<TabPage>().ToList())
            {
                if (tabPage.Name != tabToKeep)
                {
                    hiddenTabs.Add(tabPage);  // Add to hiddenTabs list
                    Modül_Tabları.TabPages.Remove(tabPage);  // Remove tab
                }
            }
        }

        private void InitializeFormBasedOnMethod()
        {
            if (selectedMethod == "ELF (Ekonometrik)")
            {
                // Show only the tab_girdi tab and hide others
                Modül_Tabları.SelectedTab = Modül_Tabları.TabPages["tab_girdi"];
                veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
                veri_listesi_seçimi.Enabled = false;

                // Hide all other tabs
                HideOtherTabs("tab_girdi");
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // For SLF, do not hide any tabs
                // Optionally, add other logic if needed
            }
        }

        public void RestoreHiddenTabs()
        {
            foreach (TabPage tabPage in hiddenTabs)
            {
                Modül_Tabları.TabPages.Add(tabPage);  // Add back the hidden tabs
            }

            hiddenTabs.Clear();  // Clear the list after restoring
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            // Handle file loading logic for the "Girdi" module
            if (slfStartYear == 0 || slfEndYear == 0)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if an item is selected in the ComboBox before accessing it
            if (veri_listesi_seçimi.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir veri tipi seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit if no valid data type is selected
            }

            // Perform file selection based on the selected data type
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

            // Ensure girdiModülü is properly initialized
            if (!girdiModülleri.ContainsKey(seçilenVeriTipi))
            {
                MessageBox.Show("Geçersiz veri tipi seçildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit if the selected data type is not valid
            }

            // Use the selectedMethod here
            if (selectedMethod == "ELF (Ekonometrik)")
            {
                // Logic for ELF selection
                MessageBox.Show("ELF method selected, skipping prerequisites.");
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Logic for SLF selection
                MessageBox.Show("SLF method selected, prerequisites are required.");
            }
            else
            {
                // Handle other cases or invalid selection
                MessageBox.Show("No valid method selected.");
            }

            girdiModülü = girdiModülleri[seçilenVeriTipi];
            girdiModülü.SlfStartYear = slfStartYear;
            girdiModülü.SlfEndYear = slfEndYear;

            // Check if "ELF" is selected to skip prerequisites
            bool skipPrerequisites = (selectedMethod == "ELF (Ekonometrik)");

            // Call VEERProcess with skipPrerequisites flag
            var isImported = girdiModülü.VEERProcess(seçilenVeriTipi, skipPrerequisites);
            if (isImported)
            {
                veri_listesi_seçimi.Refresh();
                dataGridView1.DataSource = girdiModülü.CurrentDataTable;
            }
        }


        /*        private void button1_Click(object sender, EventArgs e)
                {
                    // Girdi modülündeki dosya yükleme butonuna tıklandığında çalışacak kodlar
                    if (slfStartYear == 0 || slfEndYear == 0)
                    {
                        MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Veri listesinde seçilen veri tipine göre dosya seçme işlemi yapılacak
                    string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
                    girdiModülü = girdiModülleri[seçilenVeriTipi];
                    girdiModülü.SlfStartYear = slfStartYear;
                    girdiModülü.SlfEndYear = slfEndYear;
                    var isImported = girdiModülü.VEERProcess(seçilenVeriTipi);
                    if (isImported)
                    {
                        veri_listesi_seçimi.Refresh();
                        dataGridView1.DataSource = girdiModülü.CurrentDataTable;
                    }
                }*/

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
        private void HighlightPolygon(GMapPolygon polygon, int index)
        {
            if (gridOverlay.Polygons.Contains(polygon))
            {
                // Reset previous selected polygon
                foreach (var poly in gridOverlay.Polygons)
                {
                    poly.Stroke = new Pen(overlayColors[index].BorderColor, 3);
                    poly.Fill = new SolidBrush(overlayColors[index].FillColor);
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
                    selectedPolygon.Stroke = new Pen(overlayColors[index].BorderColor, 3);
                    selectedPolygon.Fill = new SolidBrush(overlayColors[index].FillColor);
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
                        HighlightPolygon(polygon, layer_index);

                        if (polygonAttributes.TryGetValue(polygon, out DataRow row))
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
                        HighlightPolygon(polygon, layer_index);

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


        // ------------------------------------------- FONKSİYONLAR --------------------------------------- //
        private List<(GMapPolygon Polygon, DataRow Attributes)> ExtractPolygonsAndAttributes(GMapOverlay overlay, DataTable dataTable)
        {
            List<(GMapPolygon Polygon, DataRow Attributes)> polygonData = new List<(GMapPolygon, DataRow)>();

            foreach (GMapPolygon polygon in overlay.Polygons)
            {
                if (polygonAttributes.TryGetValue(polygon, out DataRow attributes))
                {
                    polygonData.Add((polygon, attributes));
                }
            }

            return polygonData;
        }

        // check whether two polygons intersect
        private bool PolygonsIntersect(GMapPolygon polygon1, GMapPolygon polygon2)
        {
            // Convert GMapPolygon to NTS Polygon
            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();

            var coordinates1 = polygon1.Points.Select(p => new NetTopologySuite.Geometries.Coordinate(p.Lng, p.Lat)).ToArray();
            var coordinates2 = polygon2.Points.Select(p => new NetTopologySuite.Geometries.Coordinate(p.Lng, p.Lat)).ToArray();

            var ntsPolygon1 = geometryFactory.CreatePolygon(coordinates1);
            var ntsPolygon2 = geometryFactory.CreatePolygon(coordinates2);

            return ntsPolygon1.Intersects(ntsPolygon2);
        }

        // Method to extract consumption value from DataRow
        private double ExtractConsumptionValue(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && double.TryParse(row[columnName].ToString(), out double value)
                ? value
                : 0.0;
        }

        // combine the attributes of the polygons which intersect one another
        private DataRow CombineAttributes(DataRow leftRow, DataRow rightRow)
        {
            // initialize the combined data table
            DataTable combinedTable = new DataTable();

            // Add columns from leftRow
            foreach (DataColumn column in leftRow.Table.Columns)
            {
                combinedTable.Columns.Add(column.ColumnName, column.DataType);
            }

            // Add columns from rightRow, avoiding duplicates
            foreach (DataColumn column in rightRow.Table.Columns)
            {
                if (!combinedTable.Columns.Contains(column.ColumnName))
                {
                    combinedTable.Columns.Add(column.ColumnName, column.DataType);
                }
            }

            // create a new row of the combinedTable so it will have the same columns and structure
            DataRow combinedRow = combinedTable.NewRow();

            // Fill combinedRow with values from leftRow
            foreach (DataColumn column in leftRow.Table.Columns)
            {
                combinedRow[column.ColumnName] = leftRow[column];
            }

            // Fill combinedRow with values from rightRow
            foreach (DataColumn column in rightRow.Table.Columns)
            {
                combinedRow[column.ColumnName] = rightRow[column];
            }

            return combinedRow;
        }

        private DataRow CombineAttributesWithAggregations(
            DataRow leftRow, DataRow rightRow,
            Dictionary<string, double> counts, Dictionary<string, double> sums,
            Dictionary<string, double> mins, Dictionary<string, double> maxs,
            List<string> selectedColumns)
        {
            // initialize the data table that will hold the combination of the two tables
            DataTable combinedTable = new DataTable();

            // Add columns from leftRow to the combinedTable
            foreach (DataColumn column in leftRow.Table.Columns)
            {
                combinedTable.Columns.Add(column.ColumnName, column.DataType);
            }

            // create a new row of the combinedTable so it will have the same columns and structure
            DataRow combinedRow = combinedTable.NewRow();

            // Fill combinedRow with values from leftRow
            foreach (DataColumn column in leftRow.Table.Columns)
            {
                combinedRow[column.ColumnName] = leftRow[column];
            }

            // Calculate summary measures for each selected column
            foreach (string column in selectedColumns)
            {
                double consumptionValue = ExtractConsumptionValue(rightRow, column);
                counts[column]++;
                sums[column] += consumptionValue;
                if (consumptionValue < mins[column]) mins[column] = consumptionValue;
                if (consumptionValue > maxs[column]) maxs[column] = consumptionValue;
            }

            return combinedRow;
        }


        // take two polygons with <GMapPolygon, DataRow> dictionary structure and combine them into
        // a new combined polygon with the same structure
        private List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> PerformSpatialJoin(
            List<(GMapPolygon Polygon, DataRow Attributes)> layer_1,
            List<(GMapPolygon Polygon, DataRow Attributes)> layer_2)
        {
            // initialize a list named "joinedData" that will hold the info about the polygon that is created
            // due to the intersection operation, and the corresponding data
            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> joinedData = new List<(GMapPolygon, DataRow)>();

            // polygon-wise spatial join of the two polygons
            foreach (var (gridPolygon, gridAttributes) in layer_1)
            {
                foreach (var (shapePolygon, shapeAttributes) in layer_2)
                {
                    if (PolygonsIntersect(gridPolygon, shapePolygon))
                    {
                        DataRow combinedAttributes = CombineAttributes(gridAttributes, shapeAttributes);
                        joinedData.Add((gridPolygon, combinedAttributes));
                    }
                }
            }
            return joinedData;
        }

        // take two polygons with <GMapPolygon, DataRow> dictionary structure and combine them into
        // a new combined polygon with the same structure
        private List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes,
            Dictionary<string, double> Counts,
            Dictionary<string, double> Sums,
            Dictionary<string, double> Mins,
            Dictionary<string, double> Maxs)> PerformSpatialJoinWithAggregations(
            List<(GMapPolygon Polygon, DataRow Attributes)> layer_1,
            List<(GMapPolygon Polygon, DataRow Attributes)> layer_2,
            List<string> selectedColumns)
        {
            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes,
                Dictionary<string, double> Counts,
                Dictionary<string, double> Sums,
                Dictionary<string, double> Mins,
                Dictionary<string, double> Maxs)> joinedData = new List<(GMapPolygon, DataRow,
                            Dictionary<string, double>,
                            Dictionary<string, double>,
                            Dictionary<string, double>,
                            Dictionary<string, double>)>();

            foreach (var (gridPolygon, gridAttributes) in layer_1)
            {
                // initialize the aggregate arrays as dictionaries
                var counts = selectedColumns.ToDictionary(column => column, column => 0.0);
                var sums = selectedColumns.ToDictionary(column => column, column => 0.0);
                var mins = selectedColumns.ToDictionary(column => column, column => double.MinValue);
                var maxs = selectedColumns.ToDictionary(column => column, column => double.MaxValue);

                // add all of the columns from the first layer, and only the aggregate columns
                // from the second layer
                DataRow combinedAttributes = gridAttributes.Table.NewRow();

                foreach (DataColumn columns_original in gridAttributes.Table.Columns)
                {
                    combinedAttributes[columns_original] = gridAttributes[columns_original];
                }

                foreach (string column in selectedColumns)
                {
                    if (!combinedAttributes.Table.Columns.Contains($"{column}_Count"))
                    {
                        combinedAttributes.Table.Columns.Add($"{column}_Count", typeof(double));
                        combinedAttributes.Table.Columns.Add($"{column}_Sum", typeof(double));
                        combinedAttributes.Table.Columns.Add($"{column}_Min", typeof(double));
                        combinedAttributes.Table.Columns.Add($"{column}_Max", typeof(double));
                    }
                }

                foreach (var (shapePolygon, shapeAttributes) in layer_2)
                {
                    if (PolygonsIntersect(gridPolygon, shapePolygon))
                    {
                        combinedAttributes = CombineAttributesWithAggregations(combinedAttributes, shapeAttributes,
                            counts, sums, mins, maxs, selectedColumns);
                    }
                }

                joinedData.Add((gridPolygon, combinedAttributes, counts, sums, mins, maxs));
            }

            return joinedData;
        }

        // after performing spatial join, create the resulting GMapOverlay object and add the resulting
        // polygon and attributes to the specified objects
        private GMapOverlay CreateResultingOverlay(
        List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> joinedData)
        {
            GMapOverlay resultingOverlay = new GMapOverlay("ResultingOverlay");

            foreach (var (resultingPolygon, resultingAttributes) in joinedData)
            {
                resultingOverlay.Polygons.Add(resultingPolygon);
                polygonAttributes[resultingPolygon] = resultingAttributes;

                resultingPolygon.Stroke = new Pen(Color.LightSeaGreen, 3);
                resultingPolygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Transparent));
            }

            return resultingOverlay;
        }

        // create the polygons and the affiliated data to the specified objects
        // that are the results of the jabl-summary functionality
        private GMapOverlay CreateResultingOverlayWithSummaries(
            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes,
                Dictionary<string, double> Counts,
                Dictionary<string, double> Sums,
                Dictionary<string, double> Mins,
                Dictionary<string, double> Maxs)> joinedData,
            List<string> selectedColumns)
        {
            GMapOverlay resultingOverlay = new GMapOverlay("ResultingOverlay");

            foreach (var (resultingPolygon, resultingAttributes, counts, sums, mins, maxs) in joinedData)
            {
                foreach (string column in selectedColumns)
                {

                    if (resultingAttributes != null)
                    {
                        resultingAttributes[$"{column}_Count"] = counts[column];
                        resultingAttributes[$"{column}_Sum"] = sums[column];

                        if (maxs[column] == double.MaxValue)
                        {
                            resultingAttributes[$"{column}_Max"] = double.PositiveInfinity;
                        }
                        else
                        {
                            resultingAttributes[$"{column}_Max"] = maxs[column];
                        }

                        if (mins[column] == double.MinValue)
                        {
                            resultingAttributes[$"{column}_Min"] = double.NegativeInfinity;
                        }
                        else
                        {
                            resultingAttributes[$"{column}_Min"] = mins[column];
                        }
                    }
                    else
                    {
                        MessageBox.Show("null");
                    }

                }
                resultingOverlay.Polygons.Add(resultingPolygon);
                polygonAttributes[resultingPolygon] = resultingAttributes;

                resultingPolygon.Stroke = new Pen(Color.LightSeaGreen, 5);
                resultingPolygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Transparent));

            }

            return resultingOverlay;
        }

        private void ModülFormu_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                    "Programı kapatmak istediğinize emin misiniz? Kaydedilmeyen veriler kaybolacaktır!",
                    "Çıkış",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancel the closing event
            }
        }

        private void Poligon_Sil_Click(object sender, EventArgs e)
        {
            if (polygonOverlay_stokastik == null || polygonOverlay_stokastik.Polygons.Count == 0)
            {
                MessageBox.Show("Herhangi bir poligon çizilmemiştir. Lütfen öncelikle bir poligon çiziniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void veri_listesi_seçimi_SelectedIndexChanged(object sender, EventArgs e)
        {
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
            girdiModülü = girdiModülleri[seçilenVeriTipi];
            dataGridView1.DataSource = girdiModülü.importedDataTable;
        }

        private void SortTabPagesAlphabetically(TabControl tabControl, bool ascending = true)
        {
            // Get the list of TabPages
            List<TabPage> tabPages = new List<TabPage>();
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                tabPages.Add(tabPage);
            }

            // Sort the list of TabPages based on the Text property
            tabPages.Sort((x, y) =>
            {
                return ascending ? string.Compare(x.Text, y.Text) : -string.Compare(x.Text, y.Text);
            });

            // Clear the current TabPages and add the sorted TabPages
            tabControl.TabPages.Clear();
            tabControl.TabPages.AddRange(tabPages.ToArray());
        }

        private void veri_listesi_seçimi_DrawItem(object sender, DrawItemEventArgs e)
        {

            // Check if the index is valid
            if (e.Index < 0)
                return;

            // Get the current item to be drawn
            string text = veri_listesi_seçimi.Items[e.Index].ToString();

            // Determine the color based on some condition
            Color textColor = Color.Red;
            var girdiModülü = girdiModülleri[text];
            if (girdiModülü.importedDataTable.Rows.Count > 0)
            {
                textColor = Color.Green;
            }

            e.DrawBackground();
            // Draw the text with the determined color
            using (Brush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            // Draw the focus rectangle if the item is selected
            e.DrawFocusRectangle();
        }

        private void raporGoruntuleButonu_Click(object sender, EventArgs e)
        {
            // Girdi modülündeki dosya yükleme butonuna tıklandığında çalışacak kodlar
            if (slfStartYear == 0 || slfEndYear == 0)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Veri listesinde seçilen veri tipine göre dosya seçme işlemi yapılacak
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
            girdiModülü = girdiModülleri[seçilenVeriTipi];
            girdiModülü.VEERReport(seçilenVeriTipi);
        }



        // join the two layers by their indexes within the tüm_katmanlar_array GMapOverlay array
        public async Task JoinAttributesByLocation()
        {
            // Assume selectedColumns is populated from the ComboBox selections
            List<string> selectedColumns = fonksiyonFormu.agrege_olacak_sutunlar;

            // find the indices of the layers that are selected in the "jabl" functionality/interface
            // in the "tüm_katmanlar_array_names"
            firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == firstLayerName);
            secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == secondLayerName);

            // extract the first and second overlay layers according to their specified indices
            GMapOverlay firstOverlay = tüm_katmanlar_array[firstLayerToJoin];
            GMapOverlay secondOverlay = tüm_katmanlar_array[secondLayerToJoin];

            // extract the data of the first layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstOverlay, tüm_katmanlar_datatable[firstLayerToJoin]);

            // extract the data of the second layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondOverlay, tüm_katmanlar_datatable[secondLayerToJoin]);

            // spatially join the two layers and store the results in the "joinedData" List object
            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> joinedData = PerformSpatialJoin(firstLayerData, secondLayerData);

            // create the resulting overlay with respect to the "joinedData" object
            GMapOverlay resultingOverlay = CreateResultingOverlay(joinedData);

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, i => i == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            // add the resulting layer and its name to the specified arrays
            tüm_katmanlar_array[layer_index] = resultingOverlay;
            tüm_katmanlar_array_names[layer_index] = "Birleştirilmiş_Katman_" + layer_index.ToString();

            // create a data table object and fill it with the information from the joinedData object
            DataTable joined_data_table = new DataTable();

            if (joinedData.Count > 0)
            {

                // Use the first DataRow to define the columns of the DataTable
                DataRow firstRow = joinedData[0].ResultingAttributes;

                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    joined_data_table.Columns.Add(column.ColumnName, column.DataType);
                }

                // Add each DataRow within the resulting "joinedData" object to the "joined_data_table" object
                foreach (var (_, dataRow) in joinedData)
                {
                    DataRow newRow = joined_data_table.NewRow();
                    foreach (DataColumn column in joined_data_table.Columns)
                    {
                        newRow[column.ColumnName] = dataRow[column.ColumnName];
                    }
                    joined_data_table.Rows.Add(newRow);
                }

            }

            // add the datatable to the array so that it can be summoned later
            tüm_katmanlar_datatable[layer_index] = joined_data_table;

            // checkbox on/off control
            System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
            if (associatedCheckBox != null)
            {
                associatedCheckBox.Checked = true;
                associatedCheckBox.Visible = true;
                associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
            }

            // add the layer to the specified map
            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Overlays.Add(resultingOverlay);
                gMapControl_stokastik.Refresh();
            }
            else if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Overlays.Add(resultingOverlay);
                gMapControl_EA.Refresh();
            }
        }

        private void ExcelDownloadButton_Click(object sender, EventArgs e)
        {
            if (slfStartYear == 0 || slfEndYear == 0)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
            const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
            var dataTable = girdiModülleri[seçilenVeriTipi].importedDataTable;
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show($"{seçilenVeriTipi} henüz içeri aktarılmadığından Excel dosyası kaydedilemiyor.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (dataTable.Rows.Count > 50000)
            {
                MessageBox.Show($"{seçilenVeriTipi} için veri boyutu çok büyük. CSV olarak dışa aktarmayı deneyebilirsiniz.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var fileDialog = new SaveFileDialog
            {
                Title = "Kaydedeceğiniz dosyanın adını giriniz.",
                Filter = $"{FilterExcelFiles}|{FilterAllFiles}"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = fileDialog.FileName;
                var excelExporter = new ExcelExporter();
                excelExporter.ExportExcelFile(
                    filePath,
                    dataTable,
                    seçilenVeriTipi
                );
            }
            else
            {
                MessageBox.Show("Dosya seçilmedi.");
            }

        }

        private void csvExportButton_Click(object sender, EventArgs e)
        {
            if (slfStartYear == 0 || slfEndYear == 0)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            const string FilterCsvFiles = "Csv dosyaları (*.csv)|*.csv";
            const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
            var dataTable = girdiModülleri[seçilenVeriTipi].importedDataTable;
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show($"{seçilenVeriTipi} henüz içeri aktarılmadığından Csv dosyası kaydedilemiyor.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var fileDialog = new SaveFileDialog
            {
                Title = "Kaydedeceğiniz dosyanın adını giriniz.",
                Filter = $"{FilterCsvFiles}|{FilterAllFiles}"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = fileDialog.FileName;
                var csvHandler = new CsvHandler();
                csvHandler.ExportCsvFile(
                    filePath,
                    dataTable
                );
            }
            else
            {
                MessageBox.Show("Dosya seçilmedi.");
            }
        }
        private void veri_listesi_seçimi_MouseDown(object sender, MouseEventArgs e)
        {

        }

        /* -------------------------------------------------------------------------------------------*/


        //////////////// YEAR SELECTION EVENTS ////////////////

        private void ResetYearSelectionProcessGirdiModulu()
        {
            slfStartYear = slfEndYear = 0;
            int currentYear = DateTime.Now.Year;
            int lastYear = currentYear - 1;

            startYearComboBox.SelectedIndex = -1;
            startYearComboBox.Text = "Yıl seçiniz";
            endYearComboBox.SelectedIndex = -1;
            endYearComboBox.Text = "Yıl seçiniz";
            yearApproveButton.Text = "Onayla";

            //// Clear any existing items in the ComboBox
            startYearComboBox.Items.Clear();

            // Add the years to the ComboBox
            startYearComboBox.Items.Add(lastYear);
            startYearComboBox.Items.Add(currentYear);

            // Disable the endYearComboBox initially
            startYearComboBox.Enabled = true;
            endYearComboBox.Enabled = false;
            yearApproveButton.Enabled = false;
           // veri_listesi_seçimi.Enabled = false;
        }
        private void ModülFormu_Load(object sender, EventArgs e)
        {
            // Modül formunu yüklerken reset year selection sürecini başlat
            ResetYearSelectionProcessGirdiModulu();
        }

        private void startYearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startYearComboBox.SelectedIndex == -1)
            {
                return;
            }
            // Get the selected year
            int selectedYear = (int)startYearComboBox.SelectedItem;


            // Enable the endYearComboBox
            endYearComboBox.Enabled = true;

            // Clear any existing items in the ComboBox
            endYearComboBox.Items.Clear();

            // Add years from selectedYear + 4 to selectedYear + 14
            for (int year = selectedYear + 4; year <= selectedYear + 14; year++)
            {
                endYearComboBox.Items.Add(year);
            }

            // Optionally, set the first year as the selected item
            //endYearComboBox.SelectedIndex = 0;
        }

        private void endYearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            yearApproveButton.Enabled = true;
        }
        private void calculateChargeStation(int greenAc, int redDc)
        {
            if (this.InvokeRequired)
            {
                // Eğer bu metod arka plandan çağrıldıysa, UI güncellemesini UI thread'ine taşı.
                this.Invoke(new Action(() => calculateChargeStation(greenAc, redDc)));
                return;
            }

            // Önce mevcut label'ı bulup, varsa kaldırıyoruz
            var existingLabel = gMapControl_EA.Controls.Find("istasyonAdetLabel", true).FirstOrDefault();
            if (existingLabel != null)
            {
                gMapControl_EA.Controls.Remove(existingLabel);  // gMapControl_EA'den kaldır
                Console.WriteLine("Label kaldırıldı");
            }


            // Yeni bir label oluşturuyoruz
            System.Windows.Forms.Label istasyonAdetLabel = new System.Windows.Forms.Label();

            // İstasyon sayılarını eksiltmeden gösteriyoruz
            istasyonAdetLabel.Text = $"AC istasyonlar: {greenAc-1}, DC istasyonlar: {redDc-1}";

            // Debug için konsola yazdır (log)
            Console.WriteLine($"AC Sayısı: {greenAc}, DC Sayısı: {redDc}");

            // Haritanın sağ üst köşesine etiketi yerleştiriyoruz
            istasyonAdetLabel.Location = new System.Drawing.Point(gMapControl_EA.Width - 400, 10);
            istasyonAdetLabel.AutoSize = true;  // Otomatik boyutlandırma

            // Yazı tipi ve stil ayarları
            istasyonAdetLabel.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
            istasyonAdetLabel.ForeColor = System.Drawing.Color.White;  // Yazı rengini beyaz yapıyoruz
            istasyonAdetLabel.BackColor = System.Drawing.Color.Transparent;  // Arka planı şeffaf yapıyoruz

            // Etiketi sağ üst köşeye sabitliyoruz
            istasyonAdetLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            istasyonAdetLabel.Name = "istasyonAdetLabel";  // İleride bulabilmek için ad veriyoruz

            // Label'i gMapControl_EA'ye ekliyoruz
            gMapControl_EA.Controls.Add(istasyonAdetLabel);

            // Haritayı yeniden çiziyoruz
            gMapControl_EA.Refresh();
        }


        private async Task eaHaritayaVeriYukleAsync()
        {
            int redDc = 0;
            int greenAc = 0;

            try
            {
                GMapOverlay eaOverlay = new GMapOverlay("EA Layer");

                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                gMapControl_EA.Overlays.Clear();

                DataTable eaData = await Task.Run(() => DataGridViewToDataTable(dataGridView1));

                if (eaData != null && eaData.Rows.Count > 0)
                {
                    greenAc = 0;  // Ensure counters are reset
                    redDc = 0;

                    Invoke(new Action(() =>
                    {
                        foreach (DataRow row in eaData.Rows)
                        {
                            if (!girdiModülü.IsNullLike(row["EA_X_KOORDINAT"]) && !girdiModülü.IsNullLike(row["EA_Y_KOORDINAT"]))
                            {
                                double x = Convert.ToDouble(row["EA_X_KOORDINAT"]);
                                double y = Convert.ToDouble(row["EA_Y_KOORDINAT"]);

                                if (int.TryParse(row["ISTASYON_GUCU"].ToString(), out int istasyonGucu))
                                {
                                    GMarkerGoogle marker;

                                    if (istasyonGucu <= 22)
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.green);
                                        greenAc++;
                                    }
                                    else
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.red);
                                        redDc++;
                                    }

                                    eaOverlay.Markers.Add(marker);
                                }
                            }
                        }

                        // Log counters for debugging purposes
                        
                        
                        // Call the function and catch any potential errors
                        try
                        {
                            Console.WriteLine($"Green AC: {greenAc}, Red DC: {redDc}");
                            calculateChargeStation(greenAc, redDc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error in calculateChargeStation: {ex.Message}");
                        }

                        gMapControl_EA.Overlays.Add(eaOverlay);
                        gMapControl_EA.Refresh();
                    }));
                }
                else
                {
                    MessageBox.Show("Lütfen Ea şarj noktalarını görebilmek için verilerinizi yükleyiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }


        public DataTable DataGridViewToDataTable(DataGridView dataGridView) // datagridview verilerinin datatable donusumu 
        {
            DataTable dataTable = new DataTable();

            // Sütunları ekleyin
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                // DataTable'e sütunları ekleyin
                dataTable.Columns.Add(column.Name, column.ValueType);
            }

            // Satırları ekleyin
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // Eğer satır doluysa veri ekleyin (son satır boş olabilir)
                if (!row.IsNewRow)
                {
                    DataRow dataRow = dataTable.NewRow();

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dataRow[cell.ColumnIndex] = cell.Value ?? DBNull.Value; // Hücre dolu değilse DBNull olarak ayarlayın
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private async void Modül_Tabları_SelectedIndexChanged(object sender, EventArgs e) // ea sarj modulu butonu tıklandgında baslayan event fonksiyonu
        {
            // Sadece "EA Şarj Modülü" tabına tıklandığında işlem yapalım
            if (Modül_Tabları.SelectedTab.Text == "EA Şarj Modülü")
            {
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("Lütfen önce verileri yükleyin.");
                    return;
                }

                // Harita işlemini başlat
                await eaHaritayaVeriYukleAsync();
            }
        }

        private void ea_Grid_Oluştur_Click(object sender, EventArgs e)
        {
            Grid_Seçenekler grid_formu = new Grid_Seçenekler();
            grid_formu.Tag = this;
            grid_formu.Owner = this;
            grid_formu.Show();
            grid_formu.Activate();
            grid_formu.StartPosition = FormStartPosition.CenterParent;
            
        }







        private void EA_list_box_SelectedIndexChanged(object sender, EventArgs e) // ea katman kısmı 
        {

        }

        

        private void yearApproveButton_Click(object sender, EventArgs e)
        {
            if (endYearComboBox.SelectedIndex == -1)
            {
                // if the end year is not chosen, it means we are still in selection process
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (startYearComboBox.Enabled == false && endYearComboBox.Enabled == false)
            {
                // but if both combobox are disabled, it means the selection process is already done
                // Check if any DataTable in girdiModülleri has rows
                bool anyTableHasRows = girdiModülleri.Values.Any(girdiModülü =>
                    girdiModülü.importedDataTable != null && girdiModülü.importedDataTable.Rows.Count > 0);
                if (anyTableHasRows)
                {
                    var dialogResult = MessageBox.Show("Yılları değiştirirseniz verileri tekrardan içeri aktarmanız gerekecek, devam etmek istiyor musunuz?", "Uyarı!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return;
                    }
                }
                ResetYearSelectionProcessGirdiModulu();
                foreach (var girdiModülü in girdiModülleri.Values)
                {
                    //girdiModülü.importedDataTable?.Clear(); // Clear the DataTable if it is not null
                    girdiModülü.importedDataTable = new DataTable();
                }
                dataGridView1.DataSource = null;
            }
            else
            {
                // selections are completed
                startYearComboBox.Enabled = false;
                endYearComboBox.Enabled = false;
                //veri_listesi_seçimi.Enabled = true;
                slfStartYear = (int)startYearComboBox.SelectedItem;
                slfEndYear = (int)endYearComboBox.SelectedItem;
                MessageBox.Show($"Başlangıç yılı: {slfStartYear}, Bitiş yılı: {slfEndYear}", "Yıllar belirlendi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //yearApproveButton.Enabled = false;
                yearApproveButton.Text = "Sıfırla";
            }
        }
        //VISUAL CHANGES
        private void ModuleTabPanel_Paint(object sender, PaintEventArgs e)
        {
            // Get the Graphics object from the PaintEventArgs
            Graphics graphics = e.Graphics;

            // Create a rectangle the same size as the panel
            Rectangle gradient_rectangle = new Rectangle(0, 0, ModuleTabPanel.Width, ModuleTabPanel.Height);

            // Define the gradient's properties
            Brush brush = new LinearGradientBrush(gradient_rectangle, Color.FromArgb(100, 252, 179, 38), Color.FromArgb(100, 0, 253, 147), 85f);

            // Apply the gradient by filling the rectangle with the brush
            graphics.FillRectangle(brush, gradient_rectangle);

            // Optionally, set the panel's background color to be fully transparent
            ModuleTabPanel.BackColor = Color.Transparent;
        }

        /*        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
                {
                    // Get the Graphics object from the PaintEventArgs
                    Graphics graphics = e.Graphics;

                    // Create a rectangle the same size as the panel
                    Rectangle gradient_rectangle = new Rectangle(0, 0, HeaderPanel.Width, HeaderPanel.Height);

                    // Define the gradient's properties
                    Brush brush = new LinearGradientBrush(gradient_rectangle, Color.FromArgb(100, 252, 179, 38), Color.FromArgb(100, 0, 253, 147), 65f);

                    // Apply the gradient by filling the rectangle with the brush
                    graphics.FillRectangle(brush, gradient_rectangle);

                    // Optionally, set the panel's background color to be fully transparent
                    HeaderPanel.BackColor = Color.Transparent;
                }*/


        /// <summary>
        /// ELF METHOD RELATED CHANGES&UPDATES
        /// </summary>
        // Assuming you have a class like this
        public class ScriptProcessor
        {
            public void ReScript(string scriptName, string parameters)
            {
                // Your script processing logic here
                Console.WriteLine($"Processing script: {scriptName} with parameters: {parameters}");
            }
        }

        // Inside your form or class, you would create an instance of ScriptProcessor
        private ScriptProcessor wdC = new ScriptProcessor();

        private void ELFPredictionButton_Click(object sender, EventArgs e)
        {
            // Check if the ComboBox has at least two items
            if (comboBox1.Items.Count >= 2)
            {
                // Get the first and second selected items from the ComboBox
                string parametre1 = comboBox1.Items[0].ToString();
                string parametre2 = comboBox1.Items[1].ToString();

                // Concatenate the parameters (adjust for your R script's needs)
                string combinedParameters = $"{parametre1} {parametre2}"; // Space-separated parameters

                // Assuming wdC is an instance of ScriptProcessor
                wdC.ReScript("vanilin kods.R", combinedParameters);

                // Full path to your R script
                string rScriptPath = @"C:\path\to\your\script.R";

                // Pass the combined parameters to ExecuteCommand to run the R script
                ExecuteCommand(rScriptPath, combinedParameters);
            }
            else
            {
                MessageBox.Show("ComboBox does not have enough items!");
            }
        }

        public void ExecuteCommand(string scriptPath, string arguments)
        {
            // Path to the Rscript executable
            string rScriptExecutable = @"C:\Program Files\R\R-X.X.X\bin\Rscript.exe"; // Adjust for your R installation path

            // Prepare the full command with the script path and parameters
            string command = $"\"{scriptPath}\" {arguments}";

            ProcessStartInfo processInfo = new ProcessStartInfo(rScriptExecutable, command)
            {
                CreateNoWindow = true,
                UseShellExecute = false, // Allows output redirection
                RedirectStandardOutput = true, // To capture output from R script
                RedirectStandardError = true
            };

            using (Process process = Process.Start(processInfo))
            {
                // Capture and log the output if needed
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Log or handle the output
                Console.WriteLine("Output: " + output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error: " + error);
                }
            }
        }
/*        // Assuming you have a class like this
        public class ScriptProcessor
        {
            public void ReScript(string scriptName, string parameters)
            {
                // Your script processing logic here
                Console.WriteLine($"Processing script: {scriptName} with parameters: {parameters}");
            }
        }

        // Inside your form or class, you would create an instance of ScriptProcessor
        private ScriptProcessor wdC = new ScriptProcessor();

        private void ELFPredictionButton_Click(object sender, EventArgs e)
        {
            // Check if the ComboBox has at least one item
            if (comboBox1.Items.Count >= 1)
            {
                // Create a list to hold up to 5 parameters
                List<string> parameters = new List<string>();

                // Loop through the ComboBox items (up to 5 items)
                for (int i = 0; i < comboBox1.Items.Count && i < 5; i++)
                {
                    parameters.Add(comboBox1.Items[i].ToString());
                }

                // Join the parameters with space separation
                string combinedParameters = string.Join(" ", parameters);

                // Assuming wdC is an instance of ScriptProcessor
                wdC.ReScript("vanilin kods.R", combinedParameters);

                // Full path to your R script
                string rScriptPath = @"C:\path\to\your\script.R";

                // Pass the combined parameters to ExecuteCommand to run the R script
                ExecuteCommand(rScriptPath, combinedParameters);
            }
            else
            {
                MessageBox.Show("ComboBox does not have enough items!");
            }
        }

        public void ExecuteCommand(string scriptPath, string arguments)
        {
            // Path to the Rscript executable
            string rScriptExecutable = @"C:\Program Files\R\R-X.X.X\bin\Rscript.exe"; // Adjust for your R installation path

            // Prepare the full command with the script path and parameters
            string command = $"\"{scriptPath}\" {arguments}";

            ProcessStartInfo processInfo = new ProcessStartInfo(rScriptExecutable, command)
            {
                CreateNoWindow = true,
                UseShellExecute = false, // Allows output redirection
                RedirectStandardOutput = true, // To capture output from R script
                RedirectStandardError = true
            };

            using (Process process = Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error: " + error);
                }
                else
                {
                    // Load table data into DataGridView as before
                    DataTable table = ReadCsvToDataTable(@"C:\path\to\output.csv");
                    dataGridView1.DataSource = table;

                    // Load graphical output (PNG) into PictureBox controls
                    LoadImagesIntoPictureBoxes();
                }
            }
        }
        private void LoadImagesIntoPictureBoxes()
        {
            // Path to the folder where the images are saved
            string imageFolderPath = @"C:\path\to\output\";

            // Load the first image into pictureBox1
            string imagePath1 = Path.Combine(imageFolderPath, "plot1.png");
            if (File.Exists(imagePath1))
            {
                pictureBox1.Image = Image.FromFile(imagePath1);
            }

            // Load the second image into pictureBox2
            string imagePath2 = Path.Combine(imageFolderPath, "plot2.png");
            if (File.Exists(imagePath2))
            {
                pictureBox2.Image = Image.FromFile(imagePath2);
            }

            // Add more PictureBox assignments as needed
        }

        public DataTable ReadCsvToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(','); // Adjust delimiter for TSV if needed
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header); // Add columns
                }

                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(',');
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            return dataTable;
        }*/


        public async Task JoinAttributesByLocation_summary()
        {
            // Assume selectedColumns is populated from the ComboBox selections
            List<string> selectedColumns = fonksiyonFormu.agrege_olacak_sutunlar;

            // find the indices of the layers that are selected in the "jabl-summary" functionality/interface
            // in the "tüm_katmanlar_array_names"
            firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == firstLayerName);
            secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == secondLayerName);

            // extract the first and second overlay layers according to their specified indices
            GMapOverlay firstOverlay = tüm_katmanlar_array[firstLayerToJoin];
            GMapOverlay secondOverlay = tüm_katmanlar_array[secondLayerToJoin];

            // extract the data of the first layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstOverlay, tüm_katmanlar_datatable[firstLayerToJoin]);

            // extract the data of the second layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondOverlay, tüm_katmanlar_datatable[secondLayerToJoin]);

            // spatially join the two layers and store the results in the "joinedData" List object
            var joinedData = PerformSpatialJoinWithAggregations(firstLayerData, secondLayerData, selectedColumns);

            // create the resulting overlay with respect to the "joinedData" object
            GMapOverlay resultingOverlay = CreateResultingOverlayWithSummaries(joinedData, selectedColumns);

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, i => i == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            // add the resulting layer and its name to the specified arrays
            tüm_katmanlar_array[layer_index] = resultingOverlay;
            tüm_katmanlar_array_names[layer_index] = "Birleştirilmiş_Katman_" + layer_index.ToString();

            // create a data table object and fill it with the information from the joinedData object
            DataTable joined_data_table = new DataTable();

            if (joinedData.Count > 0)
            {

                // Use the first DataRow to define the columns of the DataTable
                DataRow firstRow = joinedData[0].ResultingAttributes;

                // Add columns from firstRow except those ending with _Count, _Sum, _Min, _Max
                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    if (!column.ColumnName.EndsWith("_Count") &&
                        !column.ColumnName.EndsWith("_Sum") &&
                        !column.ColumnName.EndsWith("_Min") &&
                        !column.ColumnName.EndsWith("_Max"))
                    {
                        joined_data_table.Columns.Add(column.ColumnName, column.DataType);
                    }
                }


                // Add the selected aggregate columns based on checkboxes
                foreach (string column in selectedColumns)
                {
                    if (fonksiyonFormu.checkBoxCount.Checked)
                        joined_data_table.Columns.Add($"{column}_Count", typeof(double));
                    if (fonksiyonFormu.checkBoxSum.Checked)
                        joined_data_table.Columns.Add($"{column}_Sum", typeof(double));
                    if (fonksiyonFormu.checkBoxMin.Checked)
                        joined_data_table.Columns.Add($"{column}_Min", typeof(double));
                    if (fonksiyonFormu.checkBoxMaks.Checked)
                        joined_data_table.Columns.Add($"{column}_Max", typeof(double));
                }

                foreach (var (_, dataRow, counts, sums, mins, maxs) in joinedData)
                {
                    DataRow newRow = joined_data_table.NewRow();

                    // Add original columns
                    foreach (DataColumn column in firstRow.Table.Columns)
                    {
                        if (!column.ColumnName.EndsWith("_Count") &&
                            !column.ColumnName.EndsWith("_Sum") &&
                            !column.ColumnName.EndsWith("_Min") &&
                            !column.ColumnName.EndsWith("_Max"))
                        {
                            newRow[column.ColumnName] = dataRow[column.ColumnName];
                        }
                    }

                    // Add selected aggregate values
                    foreach (DataColumn column in joined_data_table.Columns)
                    {
                        string baseColumnName = column.ColumnName.Replace("_Count", "")
                                                                .Replace("_Sum", "")
                                                                .Replace("_Min", "")
                                                                .Replace("_Max", "");

                        if (column.ColumnName.EndsWith("_Count") && fonksiyonFormu.checkBoxCount.Checked)
                            newRow[column.ColumnName] = counts[baseColumnName];
                        if (column.ColumnName.EndsWith("_Sum") && fonksiyonFormu.checkBoxSum.Checked)
                            newRow[column.ColumnName] = sums[baseColumnName];
                        if (column.ColumnName.EndsWith("_Min") && fonksiyonFormu.checkBoxMin.Checked)
                            newRow[column.ColumnName] = mins[baseColumnName];
                        if (column.ColumnName.EndsWith("_Max") && fonksiyonFormu.checkBoxMaks.Checked)
                            newRow[column.ColumnName] = maxs[baseColumnName];
                    }

                    joined_data_table.Rows.Add(newRow);
                }

            }

            // add the datatable to the array so that it can be summoned later
            tüm_katmanlar_datatable[layer_index] = joined_data_table;

            // checkbox on/off control
            System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxByIndex(layer_index);
            if (associatedCheckBox != null)
            {
                associatedCheckBox.Checked = true;
                associatedCheckBox.Visible = true;
                associatedCheckBox.Text = tüm_katmanlar_array_names[layer_index];
            }

            // add the resulting overlay to the specified gMapControl object
            if (Modül_Tabları.SelectedTab == tab_stokastik)
            {
                gMapControl_stokastik.Overlays.Add(resultingOverlay);
                gMapControl_stokastik.Refresh();
            }
            else if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Overlays.Add(resultingOverlay);
                gMapControl_EA.Refresh();
            }
        }

        // -------------------------------------------------------------------------------------------------- //

    }
}


