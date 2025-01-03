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
    public class CBS2
    {
        // Dynamic collections instead of fixed-size arrays
        public List<GMapOverlay> tüm_katmanlar_overlays;
        public List<string> tüm_katmanlar_names;
        public List<MapWinGIS.Shapefile> shapeFileArray_MapWinGIS;
        public List<System.Data.DataTable> tüm_katmanlar_datatable;

        // See the attributes of a polygon when clicked on it on the map 
        public Dictionary<GMapPolygon, DataRow> polygonAttributes; // for polygons other than grids

        // Variables to be used to export .kml files
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_kml;
        public Dictionary<GMapRoute, DataRow> routeAttributes_kml;

        // ---------- GRID VARIABLES --------- //
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

        // Constructor

        public CBS2(ModülFormu mainform)
        {
            this.modülFormu = mainform;

            // Initialize the lists instead of arrays
            tüm_katmanlar_overlays = new List<GMapOverlay>();
            tüm_katmanlar_names = new List<string>();
            shapeFileArray_MapWinGIS = new List<MapWinGIS.Shapefile>();
            tüm_katmanlar_datatable = new List<System.Data.DataTable>();

            targetDirectory = Path.Combine(userProfilePath, "Desktop");

            polygonAttributes = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();

            polygonPoints_stokastik = new List<PointLatLng>();
        }

        // Example method to add an overlay dynamically
        public void AddOverlay(GMapOverlay overlay, string layerName)
        {
            tüm_katmanlar_overlays.Add(overlay);
            tüm_katmanlar_names.Add(layerName);
        }

        // Example method to add shapefile data dynamically
        public void AddShapefile(MapWinGIS.Shapefile shapefile, System.Data.DataTable dataTable)
        {
            shapeFileArray_MapWinGIS.Add(shapefile);
            tüm_katmanlar_datatable.Add(dataTable);
        }
        // Shared method to clear markers and dispose of routes
        private void ClearMarkersAndRoute(GMapOverlay markerOverlay, GMapRoute rulerRoute)
        {
            if (markerOverlay != null)
            {
                markerOverlay.Markers.Clear();
            }

            if (rulerRoute != null)
            {
                rulerRoute.Dispose();
            }
        }

        // Shared method to reset map state and labels visibility
        private void ResetMapState(GMapControl gMapControl, System.Windows.Forms.Label mesafe_calculated, System.Windows.Forms.Label mesafe_label)
        {
            gMapControl.CanDragMap = false;
            gMapControl.Cursor = Cursors.Arrow;
            mesafe_calculated.Visible = false;
            mesafe_calculated.Text = "";
            mesafe_label.Visible = false;
        }

        // Enable the map ruler (measurement tool)
        public void EnableRuler(System.Windows.Forms.Label mesafe_calculated, System.Windows.Forms.Label mesafe_label)
        {
            mesafe_label.Text = "Mesafe: ";
            modülFormu.isRulerEnabled = true;
            mesafe_label.Visible = true;
            mesafe_label.BringToFront();
            mesafe_calculated.Visible = true;
            mesafe_calculated.BringToFront();
        }

        // Disable the ruler and reset the map state
        public void DisableRuler(GMapOverlay markerOverlay, GMapRoute rulerRoute, GMapControl gMapControl, System.Windows.Forms.Label mesafe_calculated, System.Windows.Forms.Label mesafe_label)
        {
            ClearMarkersAndRoute(markerOverlay, rulerRoute);
            ResetMapState(gMapControl, mesafe_calculated, mesafe_label);

            modülFormu.isRulerEnabled = false;
            modülFormu.isSelecting_polygon = true;
        }

        // Enable map panning (dragging)
        public void EnableMapDragging(GMapOverlay markerOverlay, GMapRoute rulerRoute, GMapControl gMapControl, System.Windows.Forms.Label mesafe_calculated, System.Windows.Forms.Label mesafe_label)
        {
            ClearMarkersAndRoute(markerOverlay, rulerRoute);
            gMapControl.CanDragMap = true;
            gMapControl.Cursor = Cursors.Hand;

            modülFormu.isRulerEnabled = false;
            modülFormu.isSelecting_polygon = false;

            mesafe_calculated.Visible = false;
            mesafe_calculated.Text = "";
            mesafe_label.Visible = false;
        }
        public async Task cbs_dosya_secimi(GMapControl gmapcontrol, Form callingForm, DataGridView dataGridView)
        {
            // Create a dictionary to manage layers dynamically
            Dictionary<int, GMapOverlay> dynamicLayers = new Dictionary<int, GMapOverlay>();

            // File dialog to select a file to import
            OpenFileDialog vektorel_veri_seçimi = new OpenFileDialog
            {
                Filter = "Shapefile|*.shp|Google Earth File|*.kml|CSV File|*.csv",
                InitialDirectory = Path.Combine(userProfilePath, "Desktop")
            };

            DialogResult result = vektorel_veri_seçimi.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filepath = vektorel_veri_seçimi.FileName;
                string filename = Path.GetFileName(filepath);
                string extension = Path.GetExtension(filepath).ToLower();

                // Determine the index dynamically based on the current size of dynamicLayers
                int layer_index = dynamicLayers.Count;

                if (extension == ".shp")
                {
                    // Create new layers for the shapefile
                    GMapOverlay shapeFileOverlay_imar = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_imar");
                    GMapOverlay shapeFileOverlay_yuk = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_yuk");
                    GMapOverlay shapeFileOverlay_stokastik = new GMapOverlay($"shapeFileOverlay_{layer_index + 1}_stokastik");

                    // Add overlays to map controls
                    modülFormu.gMapControl_imar.Overlays.Add(shapeFileOverlay_imar);
                    modülFormu.gMapControl_yuk.Overlays.Add(shapeFileOverlay_yuk);
                    modülFormu.gMapControl_stokastik.Overlays.Add(shapeFileOverlay_stokastik);

                    // Store overlay layers dynamically in the dictionary
                    dynamicLayers.Add(layer_index, shapeFileOverlay_imar); // Correctly adding the overlay to dictionary
                    dynamicLayers.Add(layer_index + 1, shapeFileOverlay_yuk); // Adding additional overlays with unique keys
                    dynamicLayers.Add(layer_index + 2, shapeFileOverlay_stokastik);

                    // Create a new DataTable for the shapefile
                    System.Data.DataTable shapefile_datatable = new System.Data.DataTable();
                    callingForm.Cursor = Cursors.WaitCursor;
                    await LoadShapefile(filepath, shapeFileOverlay_imar, dynamicLayers, shapefile_datatable, dataGridView);
                    callingForm.Cursor = Cursors.Default;

                    // Associate the checkboxes for this layer
                    List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);
                    foreach (var checkBox in associatedCheckBoxes)
                    {
                        checkBox.Checked = true;
                        checkBox.Visible = true;
                        checkBox.Text = filename;

                        // Event to remove the layer when unchecked
                        checkBox.CheckedChanged += (sender, e) =>
                        {
                            if (!checkBox.Checked)
                            {
                                modülFormu.gMapControl_imar.Overlays.Remove(shapeFileOverlay_imar);
                                modülFormu.gMapControl_yuk.Overlays.Remove(shapeFileOverlay_yuk);
                                modülFormu.gMapControl_stokastik.Overlays.Remove(shapeFileOverlay_stokastik);
                            }
                            else
                            {
                                modülFormu.gMapControl_imar.Overlays.Add(shapeFileOverlay_imar);
                                modülFormu.gMapControl_yuk.Overlays.Add(shapeFileOverlay_yuk);
                                modülFormu.gMapControl_stokastik.Overlays.Add(shapeFileOverlay_stokastik);
                            }
                        };
                    }
                }
                else if (extension == ".kml")
                {
                    // KML processing logic follows similar to shapefile but for KML files
                    // Add dynamic layer management and checkbox functionality like above.
                }
            }

            // Refresh the map controls to reflect changes
            gmapcontrol.Refresh();
            gmapcontrol.ReloadMap();
        }



        public async Task LoadShapefile(string filepath, GMapOverlay shapeFileOverlay,
                                      Dictionary<int, GMapOverlay> dynamicLayers, System.Data.DataTable shapefile_datatable, DataGridView dataGridView)
        {
            // Check if the file exists
            if (!File.Exists(filepath))
            {
                MessageBox.Show("File not found. Please check the file path.");
                return;
            }

            // Initialize the 'Row_No' column if it doesn't exist
            if (!shapefile_datatable.Columns.Contains("Row_No"))
            {
                shapefile_datatable.Columns.Add("Row_No");
            }

            // Initialize Shapefile reader
            var shpReader = new ShapefileDataReader(filepath, new NetTopologySuite.Geometries.GeometryFactory());

            // Initialize DataTable columns based on shapefile attributes
            for (int i = 0; i < shpReader.DbaseHeader.NumFields; i++)
            {
                var field = shpReader.DbaseHeader.Fields[i];
                if (!shapefile_datatable.Columns.Contains(field.Name))
                {
                    shapefile_datatable.Columns.Add(field.Name, typeof(string)); // Assume all fields are strings
                }
            }

            int rowCount = 1;

            // Process the shapefile asynchronously
            await Task.Run(() =>
            {
                while (shpReader.Read())
                {
                    var geometry = shpReader.Geometry;

                    // Create a new DataRow for attributes
                    DataRow row = shapefile_datatable.NewRow();
                    shapefile_datatable = LoadAttributeTable(row, dataGridView, shpReader, shapefile_datatable, rowCount);
                    rowCount++;

                    // Process Geometry (Polygon or MultiPolygon)
                    if (geometry is NetTopologySuite.Geometries.Polygon polygon)
                    {
                        AddPolygonToOverlay(polygon, shapeFileOverlay, row);
                    }
                    else if (geometry is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                    {
                        foreach (var poly in multiPolygon.Geometries)
                        {
                            AddPolygonToOverlay((NetTopologySuite.Geometries.Polygon)poly, shapeFileOverlay, row);
                        }
                    }
                }
            });

            // Generate a unique key (layer index or layer count) for the dynamic layers
            int layerKey = dynamicLayers.Count;  // Get the count for a new unique index/key

            // Add the overlay to the dynamic layers dictionary with a unique key
            dynamicLayers.Add(layerKey, shapeFileOverlay);

            // Optionally, update a map overlay or layer for visualization (uncomment if needed)
            // gmapcontrol.Refresh();
            // gmapcontrol.ReloadMap();
        }


        // Helper method to add a polygon to the overlay
        private void AddPolygonToOverlay(NetTopologySuite.Geometries.Polygon polygon, GMapOverlay shapeFileOverlay, DataRow row)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            foreach (var coordinate in polygon.Coordinates)
            {
                points.Add(new PointLatLng(coordinate.Y, coordinate.X));
            }

            GMapPolygon gMapPolygon = new GMapPolygon(points, "shapeFilePolygon");
            shapeFileOverlay.Polygons.Add(gMapPolygon);

            // Optionally, store additional data in DataRow
            row["ShapeType"] = "Polygon";
        }

        // Helper method to load attribute data into the DataTable
        private System.Data.DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView,
                                                         ShapefileDataReader shpReader, System.Data.DataTable shapefile_datatable, int rowCount)
        {
            row["Row_No"] = rowCount;

            // Add attributes to the row from the shapefile's attribute fields
            for (int i = 0; i < shpReader.DbaseHeader.NumFields; i++)
            {
                var field = shpReader.DbaseHeader.Fields[i];
                row[field.Name] = shpReader.GetValue(i).ToString(); // Assuming string conversion
            }

            shapefile_datatable.Rows.Add(row);
            return shapefile_datatable;
        }


    }
}
