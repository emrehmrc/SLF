using GMap.NET;
using GMap.NET.WindowsForms;
using MapWinGIS;
using NetTopologySuite.IO;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using DocumentFormat.OpenXml.Wordprocessing;
using Font = System.Drawing.Font;

namespace SLF
{
    public class CBS
    {
        public int layer_index;
        public GMapOverlay[] tüm_katmanlar_array;
        public string[] tüm_katmanlar_array_names;
        public MapWinGIS.Shapefile[] shapeFileArray_MapWinGIS; // for conversions and built-in formulas only.
        public System.Data.DataTable[] tüm_katmanlar_datatable;

        // see the attributes of a polygon when clicked on it on the map 
        public Dictionary<GMapPolygon, DataRow> polygonAttributes; // for polygons other than grids

        // variables that are to be used to export .kml files
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_kml;
        public Dictionary<GMapRoute, DataRow> routeAttributes_kml;


        // ---------- GRID VARIABLES  --------- //
        public GMapOverlay bounding_box_overlay;
        public GMapOverlay gridOverlay = new GMapOverlay("grid");
        public GMapPolygon bounding_box_polygon;
        public int grid_size = 250;
        public bool isSelecting_grid = false;
        public PointLatLng starting_point;
        public PointLatLng ending_point;
        // ------------------------------------//

        public List<PointLatLng> polygonPoints_stokastik;
        private GMapPolygon selectedPolygon;

        public Dictionary<NetTopologySuite.Geometries.Polygon, DataRow> polygonAttributes_grid; // for polygons of grids
        public List<NetTopologySuite.Geometries.Polygon> entire_grid;
        public GMapPolygon combinedPolygon;
        public NetTopologySuite.Geometries.MultiPolygon multiPolygon;

        private readonly ModülFormu modülFormu;

        // Get the user's profile path
        public string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string targetDirectory;

        public CBS(ModülFormu mainform)
        {
            this.modülFormu = mainform;

            // Initialize the arrays and other components
            tüm_katmanlar_array_names = new string[20];
            tüm_katmanlar_array = new GMapOverlay[20];
            shapeFileArray_MapWinGIS = new MapWinGIS.Shapefile[13];
            tüm_katmanlar_datatable = new System.Data.DataTable[13];

            targetDirectory = System.IO.Path.Combine(userProfilePath, "Desktop");

            polygonAttributes = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();

            polygonPoints_stokastik = new List<PointLatLng>();
        }


        //---------------------------- CBS TOOLBOX METHODLARI ----------------------------------//

        public void CBS_sec(GMapOverlay markerOverlay, GMapRoute rulerRoute,
            GMapControl gMapControl,
            System.Windows.Forms.Label mesafe_calculated,
            System.Windows.Forms.Label mesafe_label)
        {

            // cetveli ve cetvele ait noktaları/markerları sil
            if (markerOverlay != null)
            {
                markerOverlay.Markers.Clear();
            }

            if (rulerRoute != null)
            {
                rulerRoute.Dispose();
            }

            gMapControl.CanDragMap = false;
            modülFormu.isRulerEnabled = false;
            modülFormu.isSelecting_polygon = true;
            gMapControl.Cursor = Cursors.Arrow;
            mesafe_calculated.Visible = false;
            mesafe_calculated.Text = "";
            mesafe_label.Visible = false;

        }

        public void CBS_kaydır(GMapOverlay markerOverlay, GMapRoute rulerRoute,
                GMapControl gMapControl,
                System.Windows.Forms.Label mesafe_calculated,
                System.Windows.Forms.Label mesafe_label)
        {

            if (markerOverlay != null)
            {
                markerOverlay.Markers.Clear();
            }

            if (rulerRoute != null)
            {
                rulerRoute.Dispose();
            }

            gMapControl.CanDragMap = true;
            modülFormu.isRulerEnabled = false;
            modülFormu.isSelecting_polygon = false;
            gMapControl.Cursor = Cursors.Hand;
            mesafe_calculated.Visible = false;
            mesafe_calculated.Text = "";
            mesafe_label.Visible = false;

        }

        public void CBS_ölç(System.Windows.Forms.Label mesafe_calculated,
            System.Windows.Forms.Label mesafe_label)
        {
            mesafe_label.Text = "Mesafe: ";
            modülFormu.isRulerEnabled = true;
            mesafe_label.Visible = true;
            mesafe_label.BringToFront();
            mesafe_calculated.Visible = true;
            mesafe_calculated.BringToFront();

        }


        //-----------------------------------------------------------------------------//
        /*        public async Task cbs_dosya_secimi(GMapControl gmapcontrol, Form callingForm, DataGridView dataGridView)
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
                            GMapOverlay shapeFileOverlay_imar = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_imar");
                            GMapOverlay shapeFileOverlay_yuk = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_yuk");
                            GMapOverlay shapeFileOverlay_stokastik = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_stokastik");

                            modülFormu.gMapControl_imar.Overlays.Add(shapeFileOverlay_imar);
                            modülFormu.gMapControl_yuk.Overlays.Add(shapeFileOverlay_yuk);
                            modülFormu.gMapControl_stokastik.Overlays.Add(shapeFileOverlay_stokastik);

                            // create a new datatable to be added to the tüm_katmanlar_datatable array
                            System.Data.DataTable shapefile_datatable = new System.Data.DataTable();

                            // run the import method
                            callingForm.Cursor = Cursors.WaitCursor;
                            await LoadShapefile(filepath, shapeFileOverlay_imar, shapefile_datatable, dataGridView);
                            callingForm.Cursor = Cursors.Default;

                            // Duplicate the polygons and routes from shapeFileOverlay_imar into each new overlay
                            CopyOverlayContents(shapeFileOverlay_imar, shapeFileOverlay_yuk);
                            CopyOverlayContents(shapeFileOverlay_imar, shapeFileOverlay_stokastik);


                            // add the layer and its name to the specified arrays
                            tüm_katmanlar_array[layer_index] = shapeFileOverlay_imar;
                            tüm_katmanlar_array_names[layer_index] = filename;
                            tüm_katmanlar_datatable[layer_index] = shapefile_datatable;

                            // Get the list of associated checkboxes for the given layer_index
                            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);

                            if (associatedCheckBoxes != null)
                            {
                                // Loop through each checkbox in the list and apply the required settings
                                foreach (var checkBox in associatedCheckBoxes)
                                {
                                    checkBox.Checked = true;
                                    checkBox.Visible = true;
                                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                                }
                            }
                        }
                        else if (extension == "kml")
                        {
                            GMapOverlay kmlOverlay_imar = new GMapOverlay($"kmlOverlay_{layer_index + 1}_imar");
                            GMapOverlay kmlOverlay_yuk = new GMapOverlay($"kmlOverlay_{layer_index + 1}_yuk");
                            GMapOverlay kmlOverlay_stokastik = new GMapOverlay($"kmlOverlay_{layer_index + 1}_stokastik");

                            modülFormu.gMapControl_imar.Overlays.Add(kmlOverlay_imar);
                            modülFormu.gMapControl_yuk.Overlays.Add(kmlOverlay_yuk);
                            modülFormu.gMapControl_stokastik.Overlays.Add(kmlOverlay_stokastik);

                            System.Data.DataTable kml_datatable = new System.Data.DataTable();
                            callingForm.Cursor = Cursors.WaitCursor;
                            await LoadKmlFile(filepath, kmlOverlay_imar, kml_datatable, gmapcontrol);

                            tüm_katmanlar_array[layer_index] = kmlOverlay_imar;
                            tüm_katmanlar_array_names[layer_index] = filename;
                            tüm_katmanlar_datatable[layer_index] = kml_datatable;

                            // convert .kml overlay into a MapWinGIS.Shapefile object
                            polygonAttributes_kml = new Dictionary<GMapPolygon, DataRow>();
                            routeAttributes_kml = new Dictionary<GMapRoute, DataRow>();
                            MapWinGIS.Shapefile shapefile = ConvertKmlToShapefile(kmlOverlay_imar);
                            shapeFileArray_MapWinGIS[layer_index] = shapefile;

                            // Duplicate the polygons and routes from shapeFileOverlay_imar into each new overlay
                            CopyOverlayContents(kmlOverlay_imar, kmlOverlay_yuk);
                            CopyOverlayContents(kmlOverlay_imar, kmlOverlay_stokastik);

                            callingForm.Cursor = Cursors.Default;

                            // Get the list of associated checkboxes for the given layer_index
                            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);

                            if (associatedCheckBoxes != null)
                            {
                                // Loop through each checkbox in the list and apply the required settings
                                foreach (var checkBox in associatedCheckBoxes)
                                {
                                    checkBox.Checked = true;
                                    checkBox.Visible = true;
                                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                                }
                            }
                        }
                    }
                    gmapcontrol.Refresh();
                    gmapcontrol.ReloadMap();

                }*/

        public async Task cbs_dosya_secimi(GMapControl gmapcontrol, Form callingForm, DataGridView dataGridView)
        {
            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            // File dialog to select a file to import
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                string targetDirectory = Path.Combine(userProfilePath, "Desktop");
                fileDialog.Filter = "Shapefile|*.shp|Google Earth File|*.kml|CSV File|*.csv";
                fileDialog.InitialDirectory = targetDirectory;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = fileDialog.FileName;
                    string filename = Path.GetFileName(filepath);
                    string extension = Path.GetExtension(filename)?.ToLower();

                    switch (extension)
                    {
                        case ".shp":
                            await ProcessShapefile(filepath, filename, callingForm, dataGridView);
                            break;

                        case ".kml":
                            await ProcessKml(filepath, filename, callingForm, gmapcontrol);
                            break;

                        default:
                            MessageBox.Show("Geçersiz dosya formatı.");
                            break;
                    }
                }
            }

            gmapcontrol.Refresh();
            gmapcontrol.ReloadMap();
        }
        private async Task ProcessShapefile(string filepath, string overlayName, Form callingForm, DataGridView dataGridView)
        {
            // Create overlays for the shapefile
            GMapOverlay[] overlays = CreateOverlays("shp");

            // Add the first overlay to each map control
            if (modülFormu.gMapControl_imar == null) 
            {
                modülFormu.gMapControl_imar.Overlays.Add(overlays[0]);
            }
            //modülFormu.gMapControl_imar.Overlays.Add(overlays[0]);
            modülFormu.gMapControl_yuk.Overlays.Add(overlays[1]);
            modülFormu.gMapControl_stokastik.Overlays.Add(overlays[2]);
            modülFormu.gMapControl_yga.Overlays.Add(overlays[3]);

            // Create a DataTable for the shapefile data
            System.Data.DataTable shapefileDatatable = new System.Data.DataTable();
            callingForm.Cursor = Cursors.WaitCursor;

            // Load the shapefile asynchronously
            await LoadShapefile(filepath, overlays[0], shapefileDatatable, dataGridView);
            callingForm.Cursor = Cursors.Default;

            // Add polygons to all maps (after copying them to overlays)
            modülFormu.AddShapefileLayerToAllMaps(overlays[0]);  // Assuming overlays[0] contains the shapes to be added to all maps
            AddLayerToArrays(overlays[0], overlayName, shapefileDatatable);
        }

        public GMapOverlay[] CreateOverlays(string type)
        {
            string baseName = $"{type}Overlay_{layer_index}";
            Console.WriteLine($"Created layer at index {layer_index}: {baseName}");
            return new[]
            {
                new GMapOverlay($"{baseName}_imar"),
                new GMapOverlay($"{baseName}_yuk"),
                new GMapOverlay($"{baseName}_stokastik"),
                new GMapOverlay($"{baseName}_yga")

            };
        }
        private void AddLayerToArrays(GMapOverlay overlay, string overlayName, System.Data.DataTable datatable)
        {
            // Check if the maximum number of layers has been reached
            if (layer_index >= tüm_katmanlar_array.Length)
            {
                Console.WriteLine("Error: Maximum number of layers (20) reached. Cannot add more layers.");
                MessageBox.Show("Maximum number of layers (20) reached. Cannot add more layers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add the overlay, name, and datatable to the arrays at the current layer_index
            tüm_katmanlar_array[layer_index] = overlay;
            tüm_katmanlar_array_names[layer_index] = overlayName;
            tüm_katmanlar_datatable[layer_index] = datatable;

            Console.WriteLine($"Added layer at index {layer_index}: {overlayName}");

            // Increment the layer_index for the next layer
            layer_index++;
        }


        public void CopyOverlayContents(GMapOverlay sourceOverlay, GMapOverlay targetOverlay)
        {
            // Copy Polygons
            foreach (var polygon in sourceOverlay.Polygons)
            {
                var newPolygon = new GMapPolygon(polygon.Points, polygon.Name)
                {
                    Stroke = polygon.Stroke,  // Copy stroke style
                    Fill = polygon.Fill       // Copy fill color
                };
                targetOverlay.Polygons.Add(newPolygon); // Add to target overlay
            }

            // Copy Routes
            foreach (var route in sourceOverlay.Routes)
            {
                var newRoute = new GMapRoute(route.Points, route.Name)
                {
                    Stroke = route.Stroke  // Copy stroke style
                };
                targetOverlay.Routes.Add(newRoute);  // Add to target overlay
            }

            // Copy Markers
            foreach (var marker in sourceOverlay.Markers)
            {
                GMapMarker newMarker;

                // Check if the marker is of type GMarkerGoogle and copy it accordingly
                if (marker is GMarkerGoogle googleMarker)
                {
                    newMarker = new GMarkerGoogle(marker.Position, googleMarker.Type)
                    {
                        ToolTipText = marker.ToolTipText  // Copy tooltip text
                    };
                }
                else
                {
                    // For non-GMarkerGoogle markers, create a default one
                    newMarker = new GMarkerGoogle(marker.Position, GMarkerGoogleType.red)
                    {
                        ToolTipText = marker.ToolTipText  // Copy tooltip text
                    };
                }

                targetOverlay.Markers.Add(newMarker);  // Add to target overlay
            }
        }
        private async Task ProcessKml(string filepath, string filename, Form callingForm, GMapControl gmapcontrol)
        {
            GMapOverlay[] overlays = CreateOverlays("kml");

            // Add the first overlay to each map
            modülFormu.gMapControl_imar.Overlays.Add(overlays[0]);
            modülFormu.gMapControl_yuk.Overlays.Add(overlays[1]);
            modülFormu.gMapControl_stokastik.Overlays.Add(overlays[2]);
            modülFormu.gMapControl_yga.Overlays.Add(overlays[3]);

            System.Data.DataTable kmlDatatable = new System.Data.DataTable();
            callingForm.Cursor = Cursors.WaitCursor;
            await LoadKmlFile(filepath, overlays[0], kmlDatatable, gmapcontrol);
            callingForm.Cursor = Cursors.Default;

            // Copy the contents from the first overlay to the others
            CopyOverlayContents(overlays[0], overlays[1]);
            CopyOverlayContents(overlays[0], overlays[2]);
            CopyOverlayContents(overlays[0], overlays[3]);

            //AddLayerToArrays(overlays[0], filename, kmlDatatable);
            //HandleCheckBoxes(layer_index, filename);
        }
        // define default colors for each overlay object
        public (System.Drawing.Color BorderColor, System.Drawing.Color FillColor)[] overlayColors = new (System.Drawing.Color, System.Drawing.Color)[]
        {
            (System.Drawing.Color.Red, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Red)),
            (System.Drawing.Color.Blue, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Blue)),
            (System.Drawing.Color.Green, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Green)),
            (System.Drawing.Color.DarkGoldenrod, System.Drawing.Color.FromArgb(50, System.Drawing.Color.DarkGoldenrod)),
           // (System.Drawing.Color.Yellow, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Yellow)),
            (System.Drawing.Color.Purple, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Purple)),
            (System.Drawing.Color.Orange, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Orange)),
            (System.Drawing.Color.Pink, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Pink)),
           // (System.Drawing.Color.Brown, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Brown)),
            (System.Drawing.Color.Magenta, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Magenta)),
            (System.Drawing.Color.Gray, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Gray)),
            (System.Drawing.Color.Cyan, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Cyan)),
            (System.Drawing.Color.DarkTurquoise, System.Drawing.Color.FromArgb(50, System.Drawing.Color.DarkTurquoise)),
            //(System.Drawing.Color.Black, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Black)),
            (System.Drawing.Color.Teal, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Teal)),
            (System.Drawing.Color.Violet, System.Drawing.Color.FromArgb(50, System.Drawing.Color.Violet))
        };

        private System.Data.DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView,
        ShapefileDataReader shapefile_reader, System.Data.DataTable data_table, int row_cnt)
        {
            // populate the new row by using the .GetValue method
            for (int i = 1; i < shapefile_reader.DbaseHeader.NumFields; i++)
            {
                row[i] = shapefile_reader.GetValue(i); // get the value of all columns for the i-th row
            }

            // Set the Row_No value after all fields have been populated
            row["Row_No"] = row_cnt;

            // Add the resulting row to the DataTable
            data_table.Rows.Add(row);

            // Check if Invoke is needed for cross-thread operations
            if (dataGridView.InvokeRequired)
            {
                // If we are on a background thread, invoke the operation on the UI thread
                dataGridView.Invoke(new Action(() =>
                {
                    dataGridView.DataSource = data_table;
                }));
            }
            else
            {
                // If we are on the UI thread, directly set the DataSource
                dataGridView.DataSource = data_table;
            }

            return data_table;
        }


        /*        private System.Data.DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView,
                ShapefileDataReader shapefile_reader, System.Data.DataTable data_table, int row_cnt)
                {

                    // populate the new row by using the .GetValue method 
                    for (int i = 0; i < shapefile_reader.DbaseHeader.NumFields; i++)
                    {
                        row["Row_No"] = row_cnt;
                        row[i] = shapefile_reader.GetValue(i); // get the value of all columns for the i-th row
                    }
                    // Check if Invoke is needed for cross-thread operations
                    if (dataGridView.InvokeRequired)
                    {
                        // If we are on a background thread, invoke the operation on the UI thread
                        dataGridView.Invoke(new Action(() =>
                        {
                            dataGridView.DataSource = data_table;
                        }));
                    }
                    else
                    {
                        // If we are on the UI thread, directly set the DataSource
                        dataGridView.DataSource = data_table;
                    }
                    // add the resulting row to the datatable
                    data_table.Rows.Add(row);
                    return data_table;
                }*/
        /*        private System.Data.DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView,
                    ShapefileDataReader shapefile_reader, System.Data.DataTable data_table, int row_cnt)
                {
                    // Ensure the DataTable has the same number of columns as the shapefile fields
                    if (data_table.Columns.Count == 0)  // Check if columns are not already added
                    {
                        // Add "Row_No" column to the DataTable
                        data_table.Columns.Add("Row_No", typeof(int));

                        // Add columns for each shapefile field dynamically based on the shapefile's header
                        for (int i = 0; i < shapefile_reader.DbaseHeader.NumFields; i++)
                        {
                            string fieldName = shapefile_reader.DbaseHeader.Fields[i].Name; // Correctly access the field name
                            data_table.Columns.Add(fieldName, typeof(string)); // Assuming string type for all fields
                        }
                    }

                    // Populate the new row using the .GetValue method
                    row["Row_No"] = row_cnt;  // Set the row number

                    for (int i = 0; i < shapefile_reader.DbaseHeader.NumFields; i++)  // Start from 0 for valid indexing
                    {
                        row[shapefile_reader.DbaseHeader.Fields[i].Name] = shapefile_reader.GetValue(i); // Use field names to insert values
                    }

                    // Add the resulting row to the DataTable
                    data_table.Rows.Add(row);

                    // Check if Invoke is needed for cross-thread operations
                    if (dataGridView.InvokeRequired)
                    {
                        // If we are on a background thread, invoke the operation on the UI thread
                        dataGridView.Invoke(new Action(() =>
                        {
                            dataGridView.DataSource = data_table;
                        }));
                    }
                    else
                    {
                        // If we are on the UI thread, directly set the DataSource
                        dataGridView.DataSource = data_table;
                    }

                    return data_table;
                }
        */

        public void AddPolygonToOverlay(NetTopologySuite.Geometries.Polygon polygon,
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
                Stroke = new Pen(overlayColors[layer_index].BorderColor, 2),
                Fill = new SolidBrush(overlayColors[layer_index].FillColor)
            };

            overlay.Polygons.Add(gMapPolygon);
            polygonAttributes[gMapPolygon] = attributes;

            if (overlay == gridOverlay)
            {
                entire_grid.Add(polygon);
            }
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

        /*        public MapWinGIS.Shapefile ConvertOverlayToShapefile(GMapOverlay overlay)
                {
                    var shapefile = new MapWinGIS.Shapefile();
                    shapefile.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);

                    // Ensure attributes are added as fields
                    if (polygonAttributes.Count > 0)
                    {
                        var firstPolygon = polygonAttributes.Keys.First();
                        var firstRow = polygonAttributes[firstPolygon];

                        // Dynamically handle different field types based on DataColumn data type
                        foreach (DataColumn column in firstRow.Table.Columns)
                        {
                            FieldType fieldType = column.DataType == typeof(int) || column.DataType == typeof(long) ? FieldType.INTEGER_FIELD :
                                                  column.DataType == typeof(double) || column.DataType == typeof(float) ? FieldType.DOUBLE_FIELD :
                                                  FieldType.STRING_FIELD;

                            shapefile.EditAddField(column.ColumnName, fieldType, 50, 10);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No attributes found for polygons.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    foreach (var gMapPolygon in overlay.Polygons)
                    {
                        var shape = new MapWinGIS.Shape();
                        shape.Create(ShpfileType.SHP_POLYGON);

                        // Add points to shape from polygon
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
                                // Add attribute value from the DataRow to the shapefile
                                shapefile.EditCellValue(i, shapeIndex, row[i]?.ToString());
                            }
                        }
                    }

                    return shapefile;
                }*/


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
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 5, 100);
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

        public void ExportOverlayToKml(GMapOverlay overlay, string filePath)
        {
            var kmlDocument = new SharpKml.Dom.Document();
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

        public void CreateKMLFile(string latitude, string longitude)
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
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 5, 100);
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
                    shapefile.EditAddField(column.ColumnName, MapWinGIS.FieldType.STRING_FIELD, 5, 100);
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


        public async Task LoadKmlFile(string filepath, GMapOverlay kmlOverlay,
            System.Data.DataTable data_table,
            GMapControl gMapControl)
        {
            // eğer dosya bulunamadıysa uyarı ver
            if (!File.Exists(filepath))
            {
                MessageBox.Show("KML dosyası bulunamadı.!");
                return;
            }

            // oluşturulacak data table'a eklenecek olan row_cnt variable'ının initialization'u
            int row_cnt = 1;

            // wrap the KML file processing into a task to avoid blocking the main thread
            await Task.Run(() =>
            {
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
                            var row = data_table.NewRow();

                            if (!columnsAdded)
                            {
                                // Add columns based on the Schema if available
                                foreach (var schema in document.Schemas)
                                {
                                    foreach (var field in schema.Fields)
                                    {
                                        if (!data_table.Columns.Contains(field.Name))
                                        {
                                            data_table.Columns.Add(field.Name);
                                        }
                                    }
                                }
                                columnsAdded = true;
                            }

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
                                string point_coordinates = Math.Round(points.Coordinate.Longitude, 6).ToString() +
                                    " ; " + Math.Round(points.Coordinate.Latitude, 6).ToString();
                                row["Row_No"] = row_cnt;
                                row["coordinates"] = point_coordinates;
                            }

                            // add linestring coordinates to "coordinates" column if any linestring exists
                            foreach (SharpKml.Dom.LineString lineString in placemark.Flatten().OfType<SharpKml.Dom.LineString>())
                            {
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
            });

            gMapControl.Refresh(); // Refresh the map control after processing the KML file
        }



        private void EnsureColumnExists(DataTable dataTable, string columnName)
        {
            if (!dataTable.Columns.Contains(columnName))
            {
                dataTable.Columns.Add(columnName);
            }
        }

        private void ProcessPlacemark(SharpKml.Dom.Feature document, DataTable dataTable, GMapOverlay kmlOverlay, ref int rowCnt)
        {
            foreach (var placemark in document.Flatten().OfType<SharpKml.Dom.Placemark>())
            {
                var row = dataTable.NewRow();

                ProcessPlacemarkData(placemark, dataTable, ref rowCnt); // Process placemark data

                dataTable.Rows.Add(row);
                rowCnt++;  // Increment row count

                AddGeometryToOverlay(placemark, kmlOverlay, row);
            }
        }

        private void ProcessPlacemarkData(SharpKml.Dom.Placemark placemark, DataTable dataTable, ref int rowCnt)
        {
            var row = dataTable.NewRow();

            // Handle ExtendedData
            if (placemark.ExtendedData != null)
            {
                foreach (var schemaData in placemark.ExtendedData.SchemaData)
                {
                    foreach (var simpleData in schemaData.SimpleData)
                    {
                        EnsureColumnExists(dataTable, simpleData.Name);
                        row[simpleData.Name] = simpleData.Text;
                    }
                }

                foreach (var data in placemark.ExtendedData.Data)
                {
                    EnsureColumnExists(dataTable, data.Name);
                    row[data.Name] = data.Value;
                }
            }

            // Handle Coordinates (polygon, point, line)
            HandleCoordinates(placemark, row, ref rowCnt);

            // Handle Direct Attributes
            var attributes = placemark.GetType().GetProperties();
            foreach (var attribute in attributes)
            {
                EnsureColumnExists(dataTable, attribute.Name);
                row[attribute.Name] = attribute.GetValue(placemark)?.ToString();
            }

            dataTable.Rows.Add(row);
        }

        private void HandleCoordinates(SharpKml.Dom.Placemark placemark, DataRow row, ref int rowCnt)
        {
            foreach (var polygon in placemark.Flatten().OfType<SharpKml.Dom.Polygon>())
            {
                var coordinatesString = string.Join(" ; ",
                    polygon.Flatten().OfType<SharpKml.Dom.OuterBoundary>()
                    .SelectMany(outerBoundary => outerBoundary.Flatten().OfType<SharpKml.Dom.LinearRing>())
                    .Select(coord => $"{Math.Round(coord.Coordinates.First().Longitude, 6)},{Math.Round(coord.Coordinates.First().Latitude, 6)}"));
                row["Row_No"] = rowCnt;
                row["coordinates"] = coordinatesString;
            }

            foreach (var point in placemark.Flatten().OfType<SharpKml.Dom.Point>())
            {
                string pointCoordinates = $"{Math.Round(point.Coordinate.Longitude, 6)} ; {Math.Round(point.Coordinate.Latitude, 6)}";
                row["Row_No"] = rowCnt;
                row["coordinates"] = pointCoordinates;
            }

            foreach (var lineString in placemark.Flatten().OfType<SharpKml.Dom.LineString>())
            {
                var coordinatesString = string.Join(" ; ",
                    lineString.Coordinates.Select(coord => $"{Math.Round(coord.Longitude, 6)},{Math.Round(coord.Latitude, 6)}"));
                row["Row_No"] = rowCnt;
                row["coordinates"] = coordinatesString;
            }
        }

        private void AddColumnsFromSchema(SharpKml.Dom.Document document, DataTable dataTable)
        {
            foreach (var schema in document.Schemas)
            {
                foreach (var field in schema.Fields)
                {
                    EnsureColumnExists(dataTable, field.Name);
                }
            }
        }

        private void AddGeometryToOverlay(SharpKml.Dom.Placemark placemark, GMapOverlay kmlOverlay, DataRow row)
        {
            var geometry = placemark.Geometry;

            if (geometry is SharpKml.Dom.Polygon polygon)
            {
                AddPolygonToOverlay_kml(polygon, kmlOverlay, row);
            }
            else if (geometry is SharpKml.Dom.LineString lineString)
            {
                AddLineStringToOverlay_kml(lineString, kmlOverlay);
            }
        }
        public async Task LoadShapefile(string filepath, GMapOverlay shapeFileOverlay,
                                       System.Data.DataTable shapefile_datatable, DataGridView dataGridView)
        {
            // eğer dosya bulunamadıysa uyarı ver
            if (!File.Exists(filepath))
            {
                MessageBox.Show("Herhangi bir dosya bulunamadı. Lütfen tekrardan kontrol ediniz.");
                return;
            }

            // Ensure "Row_No" column exists in DataTable and is of type Int32
            if (!shapefile_datatable.Columns.Contains("Row_No"))
            {
                shapefile_datatable.Columns.Add("Row_No", typeof(int)); // Ensure Row_No is of type int
            }

            // shpReader object to read from the shp file that is being imported
            var shpReader = new ShapefileDataReader(filepath, new NetTopologySuite.Geometries.GeometryFactory());

            // Initialize the DataTable columns based on the shapefile's attribute fields
            for (int i = 0; i < shpReader.DbaseHeader.NumFields; i++)
            {
                var column = shpReader.DbaseHeader.Fields[i];
                if (!shapefile_datatable.Columns.Contains(column.Name))
                {
                    shapefile_datatable.Columns.Add(column.Name, typeof(string)); // Simplified to string for all fields
                }
            }

            int row_cnt = 1;

            // Wrap the entire processing part in Task.Run to make it non-blocking
            await Task.Run(() =>
            {
                // Read the lines of the .shp file one by one until no more line/row is left
                while (shpReader.Read())
                {
                    // Create a new row for the DataTable and then populate it
                    DataRow row = shapefile_datatable.NewRow();
                    shapefile_datatable = LoadAttributeTable(row, dataGridView,
                               shpReader, shapefile_datatable, row_cnt);

                    row_cnt++;

                    // Check if the geometry of the shapefile includes one polygon or is a multipolygon
                    var geometry = shpReader.Geometry;

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
            });

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
        }


        /*        // method that loads a shapefile object to the specified GMapOverlay map object
                public async Task LoadShapefile(string filepath, GMapOverlay shapeFileOverlay,
                                       System.Data.DataTable shapefile_datatable, DataGridView dataGridView)
                {
                    // eğer dosya bulunamadıysa uyarı ver
                    if (!File.Exists(filepath))
                    {
                        MessageBox.Show("Herhangi bir dosya bulunamadı. Lütfen tekrardan kontrol ediniz.");
                        return;
                    }

                    if (!shapefile_datatable.Columns.Contains("Row_No"))
                    {
                        // datatable that will hold the attribute table of the .shp file
                        shapefile_datatable.Columns.Add("Row_No");
                    }

                    // shpReader object to read from the shp file that is being imported
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

                    // Wrap the entire processing part in Task.Run to make it non-blocking
                    await Task.Run(() =>
                    {
                        // read the lines of the .shp file one by one until no more line/row is left
                        while (shpReader.Read())
                        {
                            // extract the geometry information of each line in the .shp file
                            var geometry = shpReader.Geometry;

                            // create a new row for the datatable and then populate it by
                            // using the LoadAttributeTable() method
                            DataRow row = shapefile_datatable.NewRow();
                            shapefile_datatable = LoadAttributeTable(row, dataGridView,
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
                    });

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
                }
        */


        // ----------------------------------- GRID CREATION ---------------------------------//

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


        // grid oluşturmak için mouse'u basılı tutup çekerken aynı zamanda seçilen alanı
        // gösteren poligonu da güncelle
        public void UpdateSelectionPolygon(GMapControl gMapControl)
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
                gMapControl.Refresh(); // harita objesini güncelle
            }
        }

        // grid oluşturma metodu
        public List<NetTopologySuite.Geometries.Polygon> CreateGrid(double xMin, double yMin, double xMax,
                double yMax, double cellSizeLat, double cellSizeLon, out System.Data.DataTable gridTable,
                Dictionary<NetTopologySuite.Geometries.Polygon, DataRow> polygonAttributes_grid)
        {
            // NTS libraries to create polygons
            var polygons = new List<NetTopologySuite.Geometries.Polygon>();
            var geomFactory = new NetTopologySuite.Geometries.GeometryFactory(); // class that has CreatePolygon() method

            int cell_no = 1;

            // Create a DataTable to hold the grid coordinates
            gridTable = new System.Data.DataTable();
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
        public void AddGridToMap(GMapControl gMapControl)
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
            double cellSizeDegreesLon = MetersToDegreesLongitude(cellSizeMeters,
                gMapControl.Position.Lat);

            // Create grid and DataTable
            System.Data.DataTable gridTable;
            var polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();
            var grid = CreateGrid(xMin, yMin, xMax, yMax, cellSizeDegreesLat, cellSizeDegreesLon,
                out gridTable, polygonAttributes_grid);

            // Add grid polygons to the overlay
            foreach (var polygon in grid)
            {
                AddPolygonToOverlay(polygon, gridOverlay, "gridPolygon", polygonAttributes_grid[polygon]);
            }

            gMapControl.Refresh();

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);
            tüm_katmanlar_array[layer_index] = gridOverlay;
            tüm_katmanlar_array_names[layer_index] = "Grid_" + grid_size + "_" + (layer_index + 1).ToString();
            tüm_katmanlar_datatable[layer_index] = gridTable;

            // Convert gridOverlay to MapWinGIS.Shapefile so that it could be exported by the MapWinGIS
            // built-in function SaveAsEx
            MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(gridOverlay);
            shapeFileArray_MapWinGIS[layer_index] = myShapefile;

            // Get the list of associated checkboxes for the given layer_index
            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);

            if (associatedCheckBoxes != null)
            {
                // Loop through each checkbox in the list and apply the required settings
                foreach (var checkBox in associatedCheckBoxes)
                {
                    checkBox.Checked = true;
                    checkBox.Visible = true;
                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                }
            }
        }



        // ------------------------------------ EXTRAS --------------------------------------//
        public GMapControl GetActiveGMapControl()
        {
            TabPage selectedTab = modülFormu.Modül_Tabları.SelectedTab;

            if (selectedTab != null)
            {
                foreach (System.Windows.Forms.Control control in selectedTab.Controls)
                {
                    // If the control is a GMapControl, return it
                    if (control is GMapControl gmapControl)
                    {
                        return gmapControl;
                    }

                    // If it's a container, recursively search for a GMapControl inside it
                    if (control is Panel panel)
                    {
                        GMapControl nestedControl = FindGMapControlInContainer(panel);
                        if (nestedControl != null)
                        {
                            return nestedControl;
                        }
                    }
                }
            }

            return null;
        }

        // Helper method to recursively search for GMapControl in nested containers
        private GMapControl FindGMapControlInContainer(System.Windows.Forms.Control container)
        {
            foreach (System.Windows.Forms.Control control in container.Controls)
            {
                if (control is GMapControl gmapControl)
                {
                    return gmapControl;
                }

                // Recursively check if the control is a container (e.g., Panel)
                if (control is Panel panel)
                {
                    GMapControl nestedControl = FindGMapControlInContainer(panel);
                    if (nestedControl != null)
                    {
                        return nestedControl;
                    }
                }
            }
            return null;
        }


        public Microsoft.Web.WebView2.WinForms.WebView2 GetActiveWebView()
        {
            TabPage selectedTab = modülFormu.Modül_Tabları.SelectedTab;

            if (selectedTab != null)
            {
                foreach (System.Windows.Forms.Control control in selectedTab.Controls)
                {
                    if (control is Microsoft.Web.WebView2.WinForms.WebView2 webView)
                    {
                        return webView;
                    }
                }
            }
            return null;
        }


        //method to check whether the point that is double clicked on the map is in a polygon
        public bool IsPointInPolygon(PointLatLng point, GMapPolygon polygon)
        {
            int i, j = polygon.Points.Count - 1;
            bool oddNodes = false;

            for (i = 0; i < polygon.Points.Count; i++)
            {
                if (polygon.Points[i].Lat < point.Lat && polygon.Points[j].Lat >= point.Lat
                || polygon.Points[j].Lat < point.Lat && polygon.Points[i].Lat >= point.Lat)
                {
                    if (polygon.Points[i].Lng + (point.Lat - polygon.Points[i].Lat) /
                        (polygon.Points[j].Lat - polygon.Points[i].Lat) *
                        (polygon.Points[j].Lng - polygon.Points[i].Lng) < point.Lng)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        //highlight the polygon which is double clicked on
        public void HighlightPolygon(GMapPolygon polygon, int index, GMapControl gMapControl)
        {
            if (gridOverlay.Polygons.Contains(polygon))
            {
                // Reset previous selected polygon
                foreach (var poly in gridOverlay.Polygons)
                {
                    poly.Stroke = new Pen(overlayColors[index].BorderColor, 2);
                    poly.Fill = new SolidBrush(overlayColors[index].FillColor);
                }

                // Highlight new selected polygon
                polygon.Stroke = new Pen(System.Drawing.Color.LawnGreen, 2);
                polygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.LawnGreen));



                gMapControl.Refresh();
            }
            else
            {
                // Reset previous selected polygon to the previously defined default map colors
                if (selectedPolygon != null)
                {
                    selectedPolygon.Stroke = new Pen(overlayColors[index].BorderColor, 2);
                    selectedPolygon.Fill = new SolidBrush(overlayColors[index].FillColor);
                }

                // Highlight new selected polygon with a different border and fill color
                selectedPolygon = polygon;
                selectedPolygon.Stroke = new Pen(System.Drawing.Color.LawnGreen, 2);
                selectedPolygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.LawnGreen));

                gMapControl.Refresh();
            }

        }
        public void Draw_Polygon(List<PointLatLng> polygonPoints, GMapOverlay polygonOverlay, GMapControl gmap)
        {
            // bu noktalar arasında poligon çiz, mavi ile işaretle, ve de 
            // polygonOverlay katmanına ekle.
            string poligonIsim = $"Poligon_{polygonOverlay.Polygons.Count + 1}";
            GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim)
            {
                Stroke = new Pen(System.Drawing.Color.Purple, 2)
            };

            polygonOverlay.Polygons.Clear();
            polygonOverlay.Polygons.Add(polygon);
            gmap.Refresh();
        }
        public System.Data.DataTable CreatePolygonDataTable(List<PointLatLng> polygonPoints, int polygonId)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("PolygonID", typeof(int));
            dt.Columns.Add("Coordinates", typeof(string));
            dt.Columns.Add("Area_Size(m2)", typeof(string));
            dt.Columns.Add("baslangic_yili", typeof(string)); // Add custom fields
            dt.Columns.Add("ilce", typeof(string));          // Add custom fields
            dt.Columns.Add("imar_tipi", typeof(string));     // Add custom fields
            dt.Columns.Add("IsDevelopmentArea", typeof(string)); // Add custom fields
            dt.Columns.Add("yasakli_alan_percentage", typeof(string)); // Add custom fields
            dt.Columns.Add("agirlik_hiz", typeof(string));   // Add custom fields
            dt.Columns.Add("taks", typeof(string));          // Add custom fields

            // Create a string representation of the coordinates in WKT format
            string coordinates = $"Polygon (({string.Join(", ", polygonPoints.Select(p => $"{p.Lat} {p.Lng}"))}))";

            double area = CalculatePolygonArea(polygonPoints);

            // Create a new row
            DataRow row = dt.NewRow();
            row["PolygonID"] = polygonId;
            row["Coordinates"] = coordinates;  // Store as WKT format
            row["Area_Size(m2)"] = Math.Round(area, 0).ToString();
            row["baslangic_yili"] = ""; // Default empty value
            row["ilce"] = ""; // Default empty value
            row["imar_tipi"] = ""; // Default empty value
            row["IsDevelopmentArea"] = ""; // Default empty value
            row["yasakli_alan_percentage"] = ""; // Default empty value
            row["agirlik_hiz"] = ""; // Default empty value
            row["taks"] = ""; // Default empty value
            dt.Rows.Add(row);

            return dt;
        }

        /*        public System.Data.DataTable CreatePolygonDataTable(List<PointLatLng> polygonPoints, int polygonId)
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("PolygonID", typeof(int));
                    dt.Columns.Add("Coordinates", typeof(string));
                    dt.Columns.Add("Area_Size(m2)", typeof(string));

                    // Create a string representation of the coordinates
                    string coordinates = string.Join(", ", polygonPoints.Select(p => $"({p.Lat}, {p.Lng})"));

                    double area = CalculatePolygonArea(polygonPoints);

                    // Create a new row
                    DataRow row = dt.NewRow();
                    row["PolygonID"] = polygonId;
                    row["Coordinates"] = coordinates;
                    row["Area_Size(m2)"] = Math.Round(area, 0).ToString();
                    dt.Rows.Add(row);

                    return dt;
                }*/

        public double CalculatePolygonArea(List<PointLatLng> points)
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

        public double Deg2Rad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        public System.Drawing.Color GetHeatmapColor(double value, double min, double max)
        {
            double ratio = (value - min) / (max - min);
            int red = (int)(255 * ratio);
            int blue = (int)(255 * (1 - ratio));
            return System.Drawing.Color.FromArgb(100, red, 0, blue); // Semi-transparent color
        }


        public void CreateHeatmap(GMapOverlay overlay, System.Data.DataTable dataTable, string columnName)
        {
            // Step 1: Find the min and max values for normalization
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row[columnName] != DBNull.Value && double.TryParse(row[columnName].ToString(),
                    out double value))
                {
                    if (value < min) min = value;
                    if (value > max) max = value;
                }
            }

            // Step 2: Apply heatmap color to each polygon based on the column value
            foreach (GMapPolygon polygon in overlay.Polygons)
            {
                // Get the corresponding DataRow for the polygon
                if (polygonAttributes.TryGetValue(polygon, out DataRow attributes))
                {

                    if (attributes[columnName] != DBNull.Value && double.TryParse(attributes[columnName].ToString(),
                        out double value))
                    {

                        System.Drawing.Color heatColor = GetHeatmapColor(value, min, max);
                        polygon.Stroke = new Pen(heatColor, 1);
                        polygon.Fill = new SolidBrush(heatColor);
                    }
                }
            }

            // Refresh the map control to show updated colors
            modülFormu.gMapControl_yuk.Refresh();
        }

        public void CreateHeatmapLegend(double min, double max)
        {
            // Clear previous legend if it exists
            if (modülFormu.Controls.ContainsKey("heatmapLegend"))
            {
                modülFormu.Controls.RemoveByKey("heatmapLegend");
            }

            // Divide the range into 10 equal brackets
            double range = max - min;
            double bracketSize = range / 10;

            // Generate labels and color boxes for each bracket
            for (int i = 0; i < 10; i++)
            {
                double bracketMin = min + (i * bracketSize);
                double bracketMax = bracketMin + bracketSize;

                // Calculate color gradient from blue to red
                System.Drawing.Color color = GetHeatmapColor(i / 9.0); // Pass a normalized value (0 to 1)

                // Create a color box
                Panel colorBox = new Panel
                {
                    Size = new System.Drawing.Size(20, 20),
                    Location = new System.Drawing.Point(10, i * 20 + 10),
                    BackColor = color
                };

                modülFormu.legendPanel.Controls.Add(colorBox);

                // Create a label for the bracket range
                System.Windows.Forms.Label rangeLabel = new System.Windows.Forms.Label
                {
                    Text = $"{bracketMin:F2} - {bracketMax:F2}",
                    Location = new System.Drawing.Point(35, i * 20 + 10),
                    AutoSize = true,
                    Font = new Font("Arial", 8)
                };

                modülFormu.legendPanel.Controls.Add(rangeLabel);
            }
        }

        // Color gradient method for blue to red
        private System.Drawing.Color GetHeatmapColor(double ratio)
        {
            int red = (int)(255 * ratio);
            int blue = (int)(255 * (1 - ratio));
            return System.Drawing.Color.FromArgb(255, red, 0, blue); // Opaque colors
        }


        // ------------------------------- HARİTA EVENTLERİ ----------------------------------/////////////////////

        public void ManuelGridSecimi(GMapControl gMapControl)
        {
            gMapControl.CanDragMap = false;
            bounding_box_overlay = new GMapOverlay("bounding_box_overlay");

            // seçilen alanı kullanıcıya gösterecek olan poligonu oluşturmaya başla
            bounding_box_polygon = new GMapPolygon(new List<PointLatLng>(), "bounding_box_polygon")
            {
                Stroke = new Pen(System.Drawing.Color.White, 3),
                Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.White))
            };

            bounding_box_overlay.Polygons.Add(bounding_box_polygon);
            gMapControl.Overlays.Add(bounding_box_overlay);
        }

        // cetvel ile seçilen2 nokta arasındaki mesafeyi metre cinsinden göster
        public void CalculateDistance(GMapControl gmap, System.Windows.Forms.Label mesafe_metre,
            List<PointLatLng> rulerPoints)
        {
            if (rulerPoints.Count == 2)
            {
                double meter_distance = Math.Round(gmap.MapProvider.Projection.GetDistance(rulerPoints[0],
                    rulerPoints[1]) * 1000, 3);
                mesafe_metre.Text = meter_distance.ToString() + " metre";
            }
        }

        public void DrawRuler(GMapOverlay rulerOverlay,
        List<PointLatLng> rulerPoints,
        ref GMapRoute rulerRoute)
        {
            if (rulerRoute != null)
            {
                rulerOverlay.Routes.Remove(rulerRoute);
            }
            rulerRoute = new GMapRoute(rulerPoints, "ruler_Route");
            rulerRoute.Stroke = new Pen(System.Drawing.Color.Red, 3);
            rulerOverlay.Routes.Add(rulerRoute);

            GetActiveGMapControl().Refresh();
        }

        public void CetvelSecimi(GMapControl gMapControl,
            System.Windows.Forms.Label mesafe_metre,
            List<PointLatLng> rulerPoints,
            GMapOverlay markerOverlay,
            GMapOverlay rulerOverlay,
            ref GMapRoute rulerRoute)
        {
            // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
            modülFormu.isRulerActive = true;

            // 2 adet nokta seçildiği anda aralarındaki mesafeyi hesapla ve noktaların
            // tutulduğu listeyi temizle
            if (rulerPoints.Count == 2)
            {
                markerOverlay.Markers.Clear();

                foreach (var rulerPoint in rulerPoints)
                {
                    GMapMarker marker_1 = new GMarkerGoogle(rulerPoint, GMarkerGoogleType.orange_dot);
                    markerOverlay.Markers.Add(marker_1);
                }

                rulerRoute.Dispose();
                DrawRuler(rulerOverlay, rulerPoints, ref rulerRoute);
                CalculateDistance(gMapControl, mesafe_metre, rulerPoints);
                rulerPoints.Clear();
                modülFormu.isRulerActive = false;
            }
        }

        // --------------------------------------- FONKSİYONLAR ----------------------------------------//


        //  method to extract data from polygons
        private List<(GMapPolygon Polygon, DataRow Attributes)>
            ExtractPolygonsAndAttributes(GMapOverlay overlay, System.Data.DataTable dataTable)
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
            System.Data.DataTable combinedTable = new System.Data.DataTable();

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
            System.Data.DataTable combinedTable = new System.Data.DataTable();

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

                resultingPolygon.Stroke = new Pen(System.Drawing.Color.LightSeaGreen, 3);
                resultingPolygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.Transparent));
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

                resultingPolygon.Stroke = new Pen(System.Drawing.Color.LightSeaGreen, 5);
                resultingPolygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.Transparent));

            }

            return resultingOverlay;
        }
        // join the two layers by their indexes within the tüm_katmanlar_array GMapOverlay array
        public async Task JoinAttributesByLocation(GMapControl gMapControl)
        {
            // Assume selectedColumns is populated from the ComboBox selections
            List<string> selectedColumns = modülFormu.fonksiyonFormu.agrege_olacak_sutunlar;

            // find the indices of the layers that are selected in the "jabl" functionality/interface
            modülFormu.firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == modülFormu.firstLayerName);
            modülFormu.secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == modülFormu.secondLayerName);

            // extract the first and second overlay layers according to their specified indices
            GMapOverlay firstOverlay = tüm_katmanlar_array[modülFormu.firstLayerToJoin];
            GMapOverlay secondOverlay = tüm_katmanlar_array[modülFormu.secondLayerToJoin];

            // extract the data of the first layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstOverlay, tüm_katmanlar_datatable[modülFormu.firstLayerToJoin]);

            // extract the data of the second layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondOverlay, tüm_katmanlar_datatable[modülFormu.secondLayerToJoin]);

            // Perform the spatial join and aggregation in a background task
            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> joinedData = await Task.Run(() =>
            {
                return PerformSpatialJoin(firstLayerData, secondLayerData);
            });

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
            System.Data.DataTable joined_data_table = new System.Data.DataTable();

            if (joinedData.Count > 0)
            {
                // Use the first DataRow to define the columns of the DataTable
                DataRow firstRow = joinedData[0].ResultingAttributes;

                // Add columns from firstRow
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

            // Get the list of associated checkboxes for the given layer_index
            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);

            if (associatedCheckBoxes != null)
            {
                // Loop through each checkbox in the list and apply the required settings
                foreach (var checkBox in associatedCheckBoxes)
                {
                    checkBox.Checked = true;
                    checkBox.Visible = true;
                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                }
            }

            // Add the overlay to the GMapControl and refresh
            gMapControl.Overlays.Add(resultingOverlay);
            gMapControl.Refresh();
        }

        public async Task JoinAttributesByLocation_summary(GMapControl gMapControl)
        {
            // Assume selectedColumns is populated from the ComboBox selections
            List<string> selectedColumns = modülFormu.fonksiyonFormu.agrege_olacak_sutunlar;

            // find the indices of the layers that are selected in the "jabl-summary" functionality/interface
            modülFormu.firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == modülFormu.firstLayerName);
            modülFormu.secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == modülFormu.secondLayerName);

            // extract the first and second overlay layers according to their specified indices
            GMapOverlay firstOverlay = tüm_katmanlar_array[modülFormu.firstLayerToJoin];
            GMapOverlay secondOverlay = tüm_katmanlar_array[modülFormu.secondLayerToJoin];

            // extract the data of the first layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstOverlay, tüm_katmanlar_datatable[modülFormu.firstLayerToJoin]);

            // extract the data of the second layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondOverlay, tüm_katmanlar_datatable[modülFormu.secondLayerToJoin]);

            // Perform the spatial join and aggregation in a background task
            var joinedData = await Task.Run(() =>
            {
                return PerformSpatialJoinWithAggregations(firstLayerData, secondLayerData, selectedColumns);
            });

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
            System.Data.DataTable joined_data_table = new System.Data.DataTable();

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
                    if (modülFormu.fonksiyonFormu.checkBoxCount.Checked)
                        joined_data_table.Columns.Add($"{column}_Count", typeof(double));
                    if (modülFormu.fonksiyonFormu.checkBoxSum.Checked)
                        joined_data_table.Columns.Add($"{column}_Sum", typeof(double));
                    if (modülFormu.fonksiyonFormu.checkBoxMin.Checked)
                        joined_data_table.Columns.Add($"{column}_Min", typeof(double));
                    if (modülFormu.fonksiyonFormu.checkBoxMaks.Checked)
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

                        if (column.ColumnName.EndsWith("_Count") && modülFormu.fonksiyonFormu.checkBoxCount.Checked)
                            newRow[column.ColumnName] = counts[baseColumnName];
                        if (column.ColumnName.EndsWith("_Sum") && modülFormu.fonksiyonFormu.checkBoxSum.Checked)
                            newRow[column.ColumnName] = sums[baseColumnName];
                        if (column.ColumnName.EndsWith("_Min") && modülFormu.fonksiyonFormu.checkBoxMin.Checked)
                            newRow[column.ColumnName] = mins[baseColumnName];
                        if (column.ColumnName.EndsWith("_Max") && modülFormu.fonksiyonFormu.checkBoxMaks.Checked)
                            newRow[column.ColumnName] = maxs[baseColumnName];
                    }

                    joined_data_table.Rows.Add(newRow);
                }
            }

            // add the datatable to the array so that it can be summoned later
            tüm_katmanlar_datatable[layer_index] = joined_data_table;

            // Get the list of associated checkboxes for the given layer_index
            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);

            if (associatedCheckBoxes != null)
            {
                // Loop through each checkbox in the list and apply the required settings
                foreach (var checkBox in associatedCheckBoxes)
                {
                    checkBox.Checked = true;
                    checkBox.Visible = true;
                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                }
            }

            gMapControl.Overlays.Add(resultingOverlay);
            gMapControl.Refresh();
        }


        ///////////////////////////////////////////////////////////////////////////////////////

    }
}
