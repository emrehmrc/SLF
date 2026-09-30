using GMap.NET;
using GMap.NET.WindowsForms;
using MapWinGIS;
using NetTopologySuite.IO;
using SharpKml.Base;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using System.Globalization;
using System.Xml.Linq;


namespace SLF
{
    public class CBS
    {
        // the main index to use within the arrays and the associated checkboxes
        public int layer_index;

        // center points of the polygons drawn - point load poligonları icin kullanılacak.
        public (double Latitude, double Longitude)[] polygonCenterPoints { get; set; } = new (double, double)[50];

        private Dictionary<string, System.Drawing.Color> currentImarTipiColorMap; // Stores the color mapping for the current KML file

        // GMapOverlay arrays, one per map:
        public GMapOverlay[] tüm_katmanlar_array_imar = new GMapOverlay[50];
        public GMapOverlay[] tüm_katmanlar_array_yuk = new GMapOverlay[50];
        public string[] tüm_katmanlar_array_polygon_tags = new string[50];

        public string[] tüm_katmanlar_array_names = new string[50];
        public System.Data.DataTable[] tüm_katmanlar_datatable = new DataTable[50];

        public MapWinGIS.Shapefile[] shapeFileArray_MapWinGIS = new MapWinGIS.Shapefile[50];


        // see the attributes of a polygon when clicked on it on the map 
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_imar;
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_yuk;

        // variables that are to be used to export .kml files
        public Dictionary<GMapPolygon, DataRow> polygonAttributes_kml;
        public Dictionary<GMapRoute, DataRow> routeAttributes_kml;

        public string imported_filename;

        // the opacity of the Overlay leyers
        private const int opacity = 50;

        // ---------- GRID VARIABLES  --------- //
        public GMapOverlay bounding_box_overlay;
        public GMapOverlay gridOverlay = new GMapOverlay("grid");
        public GMapPolygon bounding_box_polygon;
        public int grid_size = 250;
        public bool isSelecting_grid = false;
        public PointLatLng starting_point;
        public PointLatLng ending_point;

        // ------------------------------------//
        private GMapPolygon selectedPolygon;

        public Dictionary<NetTopologySuite.Geometries.Polygon, DataRow> polygonAttributes_grid; // for polygons of grids
        public List<NetTopologySuite.Geometries.Polygon> entire_grid;
        public GMapPolygon combinedPolygon;
        public NetTopologySuite.Geometries.MultiPolygon multiPolygon;

        private readonly ModülFormu modülFormu;

        // Get the user's profile path
        public string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string targetDirectory;

        // Haversine formula to calculate distance between two points (in meters) - POINT LOAD yük dagıtma mantıgında kullanılacak
        public double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth's radius in meters
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        public (double Latitude, double Longitude)? ParseWktCentroid(string wkt)
        {
            if (string.IsNullOrEmpty(wkt))
                return null;

            wkt = wkt.Trim().ToUpper(); // Normalize case
            try
            {
                if (wkt.StartsWith("POLYGON"))
                {
                    var pointStrings = wkt.Replace("POLYGON ((", "").Replace("))", "").Split(',');
                    if (pointStrings.Length >= 3)
                    {
                        double minLat = double.MaxValue, maxLat = double.MinValue;
                        double minLon = double.MaxValue, maxLon = double.MinValue;

                        foreach (var point in pointStrings)
                        {
                            var coords = point.Trim().Split(' ');
                            if (coords.Length == 2 && double.TryParse(coords[0], out double lat) && double.TryParse(coords[1], out double lon))
                            {
                                minLat = Math.Min(minLat, lat); // Latitude (Y)
                                maxLat = Math.Max(maxLat, lat);
                                minLon = Math.Min(minLon, lon); // Longitude (X)
                                maxLon = Math.Max(maxLon, lon);
                            }
                            else
                            {
                                Console.WriteLine($"Invalid coordinate pair in WKT: {point}");
                            }
                        }
                        if (minLat != double.MaxValue && maxLat != double.MinValue && minLon != double.MaxValue && maxLon != double.MinValue)
                        {
                            var centroid = ((minLat + maxLat) / 2, (minLon + maxLon) / 2);
                            return centroid;
                        }
                    }
                }
                Console.WriteLine($"Failed to parse WKT: {wkt}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParseWktCentroid error: {ex.Message} for WKT: {wkt}");
                return null;
            }
        }

        public CBS(ModülFormu mainform)
        {
            this.modülFormu = mainform;

            // define a directory to be opened first in the file dialog screens
            targetDirectory = System.IO.Path.Combine(userProfilePath, "Desktop");

            polygonAttributes_imar = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_yuk = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();
        }

        public CBS()
        {
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
        // helper method to find the first available slot in the arrays

        private int FindFirstFreeLayerIndex()
        {
            for (int i = 0; i < 50; i++)
            {
                // If all four overlays at index i are null, that means it’s free
                if (tüm_katmanlar_array_imar[i] == null && tüm_katmanlar_array_yuk[i] == null)
                {
                    return i;
                }
            }
            return -1; // none free
        }


        public async Task cbs_dosya_secimi(GMapControl callingMap, Form callingForm, DataGridView dataGridView)
        {
            layer_index = FindFirstFreeLayerIndex();

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 50 adet katman seçilebilmektedir.");
                return;
            }

            // Use the safe OpenFileDialog wrapper to reduce sporadic blank/white dialog issues
            // Default initial directory requested by user:
            string targetDirectory = @"C:\Users\Emre Hangul\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı";
            string filter = "Shapefile|*.shp|Google Earth File|*.kml";
            string filepath = SLF.Utilities.DialogHelpers.ShowOpenFileDialogSafe("Seçilecek vektörel dosya", filter, targetDirectory);

            if (!string.IsNullOrEmpty(filepath))
            {
                imported_filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                string extension = imported_filename.Substring(imported_filename.Length - 3);

                GMapOverlay overlay_imar = new GMapOverlay($"overlay_{layer_index + 1}_imar");
                GMapOverlay overlay_yuk = new GMapOverlay($"overlay_{layer_index + 1}_yuk");

                DataTable dt = new DataTable();
                callingForm.Cursor = Cursors.WaitCursor;

                try
                {
                    if (extension == "shp")
                    {
                        await LoadShapefile(filepath, overlay_imar, dt, dataGridView);
                        callingForm.Cursor = Cursors.Default;

                        CopyOverlayContents(overlay_imar, overlay_yuk, polygonAttributes_imar, polygonAttributes_yuk);
                    }
                    else if (extension == "kml")
                    {
                        await LoadKmlFile(filepath, overlay_imar, dt, callingMap);

                        CopyOverlayContents(overlay_imar, overlay_yuk, polygonAttributes_imar, polygonAttributes_yuk);
                    }

                    // Build both overlays off-map. Attach them only after all geometry is ready
                    // so adding thousands of objects cannot trigger a redraw for each object.
                    modülFormu.gMapControl_imar.Overlays.Add(overlay_imar);
                    modülFormu.gMapControl_yuk.Overlays.Add(overlay_yuk);

                    tüm_katmanlar_array_imar[layer_index] = overlay_imar;
                    tüm_katmanlar_array_yuk[layer_index] = overlay_yuk;

                    tüm_katmanlar_datatable[layer_index] = dt;
                    tüm_katmanlar_array_names[layer_index] = imported_filename;
                    tüm_katmanlar_array_polygon_tags[layer_index] = "IMPORTED"; // Default tag for imported layers

                    List<CheckBox> associatedChecks = modülFormu.GetCheckBoxesByIndex(layer_index);
                    foreach (var chk in associatedChecks)
                    {
                        chk.Text = imported_filename;
                        chk.Visible = true;
                        chk.Checked = true;
                        chk.ForeColor = overlayColors[layer_index].BorderColor;
                        chk.Tag = (layer_index + 1).ToString(); // Set Tag to 1-based layer index
                    }

                    // Mark all categories for update
                    modülFormu.pendingUpdates["imar"] = true;
                    modülFormu.pendingUpdates["yuk"] = true;

                    // Update only the active tab immediately
                    modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_imar, "imar");
                    modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_yuk, "yuk");

                    // Zoom to the center of the layer
                    ZoomToLayerCenter(overlay_imar, modülFormu.gMapControl_imar, modülFormu.gMapControl_yuk);
                }
                finally
                {
                    callingForm.Cursor = Cursors.Default;
                }

                modülFormu.gMapControl_imar.Refresh();
                modülFormu.gMapControl_yuk.Refresh();
            }
        }

        public void ZoomToLayerCenter(GMapOverlay overlay, GMapControl gMapControlImar, GMapControl gMapControlYuk)
        {
            if (overlay == null || !overlay.Polygons.Any()) return;

            // Calculate the bounding box for all polygons
            double minLat = double.MaxValue, maxLat = double.MinValue;
            double minLng = double.MaxValue, maxLng = double.MinValue;

            foreach (var polygon in overlay.Polygons)
            {
                foreach (var point in polygon.Points)
                {
                    minLat = Math.Min(minLat, point.Lat);
                    maxLat = Math.Max(maxLat, point.Lat);
                    minLng = Math.Min(minLng, point.Lng);
                    maxLng = Math.Max(maxLng, point.Lng);
                }
            }

            if (minLat == double.MaxValue || maxLat == double.MinValue || minLng == double.MaxValue || maxLng == double.MinValue)
            {
                // No valid coordinates found
                return;
            }

            // Calculate the center point
            double centerLat = (minLat + maxLat) / 2.0;
            double centerLng = (minLng + maxLng) / 2.0;
            PointLatLng centerPoint = new PointLatLng(centerLat, centerLng);

            // Zoom to the center point with a zoom level of 10 (city level)
            gMapControlImar.Position = centerPoint;
            gMapControlImar.Zoom = 13;

            gMapControlYuk.Position = centerPoint;
            gMapControlYuk.Zoom = 13;

        }

        public void CopyOverlayContents(
            GMapOverlay sourceOverlay,
            GMapOverlay targetOverlay,
            Dictionary<GMapPolygon, DataRow> sourceDict,
            Dictionary<GMapPolygon, DataRow> targetDict)
        {
            // 1) Copy Polygons
            // To avoid OutOfMemory exceptions when copying very large overlays, we:
            //  - reuse existing Stroke/Fill objects instead of cloning them
            //  - reuse the Points list reference (no deep copy of point arrays)
            //  - process polygons in batches and yield to the UI thread and GC between batches
            if (sourceOverlay == null || targetOverlay == null) return;

            foreach (var srcPolygon in sourceOverlay.Polygons)
            {
                try
                {
                    // Reuse Stroke/Fill and Points references to minimize allocations
                    var newPolygon = new GMapPolygon(srcPolygon.Points, srcPolygon.Name)
                    {
                        Stroke = srcPolygon.Stroke, // reuse reference
                        Fill = srcPolygon.Fill      // reuse reference
                    };
                    // Preserve Tag (Row_No) so feature lookup by Row_No still works after copying
                    try { newPolygon.Tag = srcPolygon.Tag; } catch { /* ignore tagging failures */ }

                    // Add the new polygon to the target overlay
                    targetOverlay.Polygons.Add(newPolygon);

                    // Now copy the attribute row from the source dictionary (if present)
                    if (sourceDict != null && sourceDict.TryGetValue(srcPolygon, out DataRow row))
                    {
                        // Wrap assignment in try/catch for memory pressure cases
                        try
                        {
                            targetDict[newPolygon] = row;
                        }
                        catch (OutOfMemoryException)
                        {
                            // Try to recover: force a GC, wait, then attempt a lighter-weight association
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            System.Threading.Thread.Sleep(200);

                            try
                            {
                                // As a fallback, store only the raw item array (lighter than DataRow reference)
                                // We store it as an object[] boxed in the dictionary to preserve data without keeping heavy DataRow structures
                                var values = row.ItemArray;
                                // Use a cast dictionary if caller expects DataRow; store a wrapper DataRow is impractical under memory pressure
                                // So skip the association to avoid OOM if still failing
                                // (Better approach: store a separate lightweight map elsewhere if needed)
                                // For now, skip association to keep UI responsive
                            }
                            catch { /* swallow fallback errors */ }
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    // If we hit OOM while creating polygon objects, attempt to recover gracefully
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.Threading.Thread.Sleep(500);

                    // Try one more time for this polygon but if it still fails skip it to continue processing remaining ones
                    try
                    {
                        var retryPolygon = new GMapPolygon(srcPolygon.Points, srcPolygon.Name) { Stroke = srcPolygon.Stroke, Fill = srcPolygon.Fill };
                        targetOverlay.Polygons.Add(retryPolygon);
                        if (sourceDict != null && sourceDict.TryGetValue(srcPolygon, out DataRow retryRow))
                        {
                            try { targetDict[retryPolygon] = retryRow; } catch { /* ignore */ }
                        }
                    }
                    catch (OutOfMemoryException)
                    {
                        // Give up on this polygon but continue the loop
                        continue;
                    }
                }

            }

            // 2) Copy Routes (no dictionary logic shown—add if you have route attributes)
            foreach (var srcRoute in sourceOverlay.Routes)
            {
                var newRoute = new GMapRoute(srcRoute.Points, srcRoute.Name)
                {
                    // Same note about .Clone() for stroke if you want separate objects
                    Stroke = (Pen)srcRoute.Stroke.Clone()
                };
                targetOverlay.Routes.Add(newRoute);
            }

            // 3) Copy Markers (same idea—no dictionary logic unless you store marker attributes)
            foreach (var srcMarker in sourceOverlay.Markers)
            {
                GMapMarker newMarker;
                if (srcMarker is GMarkerGoogle googleMarker)
                {
                    newMarker = new GMarkerGoogle(srcMarker.Position, googleMarker.Type)
                    {
                        ToolTipText = srcMarker.ToolTipText
                    };
                }
                else
                {
                    newMarker = new GMarkerGoogle(srcMarker.Position, GMarkerGoogleType.red)
                    {
                        ToolTipText = srcMarker.ToolTipText
                    };
                }
                targetOverlay.Markers.Add(newMarker);

                // If you store marker attributes in a dictionary, do a similar lookup + assignment here
            }
        }


        // define default colors for each overlay object
        public (System.Drawing.Color BorderColor, System.Drawing.Color FillColor)[] overlayColors = new (System.Drawing.Color, System.Drawing.Color)[]
        {
            (System.Drawing.Color.Red, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Red)),
            (System.Drawing.Color.Blue, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Blue)),
            (System.Drawing.Color.Green, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Green)),
            (System.Drawing.Color.DarkGoldenrod, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkGoldenrod)),
            (System.Drawing.Color.Purple, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Purple)),
            (System.Drawing.Color.Orange, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Orange)),
            (System.Drawing.Color.Pink, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Pink)),
            (System.Drawing.Color.Brown, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Brown)),
            (System.Drawing.Color.Gray, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Gray)),
            (System.Drawing.Color.Cyan, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Cyan)),
            (System.Drawing.Color.DarkTurquoise, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkTurquoise)),
            (System.Drawing.Color.Black, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Black)),
            (System.Drawing.Color.Violet, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Violet)),
            (System.Drawing.Color.MistyRose, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.MistyRose)),
            (System.Drawing.Color.Navy, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Navy)),
            (System.Drawing.Color.Chocolate, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Chocolate)),
            (System.Drawing.Color.BurlyWood, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.BurlyWood)),
            (System.Drawing.Color.DarkGray, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.MintCream)),
            (System.Drawing.Color.DarkGreen, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Beige)),
            (System.Drawing.Color.Chartreuse, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Chartreuse)),
            (System.Drawing.Color.Yellow, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Yellow)),
            (System.Drawing.Color.Magenta, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Magenta)),
            (System.Drawing.Color.Lime, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Lime)),
            (System.Drawing.Color.Teal, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Teal)),
            (System.Drawing.Color.Maroon, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Maroon)),
            (System.Drawing.Color.Olive, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Olive)),
            (System.Drawing.Color.Silver, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Silver)),
            (System.Drawing.Color.Gold, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Gold)),
            (System.Drawing.Color.Indigo, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Indigo)),
            (System.Drawing.Color.Coral, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Coral)),
            (System.Drawing.Color.Crimson, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Crimson)),
            (System.Drawing.Color.DarkBlue, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkBlue)),
            (System.Drawing.Color.DarkCyan, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkCyan)),
            (System.Drawing.Color.DarkGreen, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkGreen)),
            (System.Drawing.Color.DarkMagenta, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkMagenta)),
            (System.Drawing.Color.DarkOrange, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkOrange)),
            (System.Drawing.Color.DarkRed, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkRed)),
            (System.Drawing.Color.DarkViolet, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DarkViolet)),
            (System.Drawing.Color.DeepPink, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DeepPink)),
            (System.Drawing.Color.DeepSkyBlue, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DeepSkyBlue)),
            (System.Drawing.Color.DodgerBlue, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.DodgerBlue)),
            (System.Drawing.Color.Firebrick, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Firebrick)),
            (System.Drawing.Color.ForestGreen, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.ForestGreen)),
            (System.Drawing.Color.Fuchsia, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Fuchsia)),
            (System.Drawing.Color.HotPink, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.HotPink)),
            (System.Drawing.Color.IndianRed, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.IndianRed)),
            (System.Drawing.Color.Khaki, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Khaki)),
            (System.Drawing.Color.Lavender, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.Lavender)),
            (System.Drawing.Color.LawnGreen, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.LawnGreen)),
            (System.Drawing.Color.LightBlue, System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.LightBlue))
        };

        private System.Data.DataTable LoadAttributeTable(DataRow row, DataGridView dataGridView,
        ShapefileDataReader shapefile_reader, System.Data.DataTable data_table, int row_cnt)
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

        public void AddPolygonToOverlay(
            NetTopologySuite.Geometries.Polygon polygon,
            GMapOverlay overlay,
            string gMapPolygonId,
            DataRow attributes)
        {
            // Build the point list
            List<PointLatLng> points_list = new List<PointLatLng>();
            foreach (var coord in polygon.Coordinates)
            {
                points_list.Add(new PointLatLng(coord.Y, coord.X));
            }

            // Get the base fill and border colors
            System.Drawing.Color baseFillColor = overlayColors[layer_index].FillColor;
            System.Drawing.Color baseBorderColor = overlayColors[layer_index].BorderColor;

            // Set the alpha value to 50 for 20% opacity for the fill
            System.Drawing.Color transparentFillColor = System.Drawing.Color.FromArgb(opacity, baseFillColor.R, baseFillColor.G, baseFillColor.B);

            // Create the GMapPolygon
            GMapPolygon gMapPolygon = new GMapPolygon(points_list, gMapPolygonId)
            {
                Stroke = new Pen(baseBorderColor, 3), // Use original border color or transparentBorderColor if uncommented
                Fill = new SolidBrush(transparentFillColor)
            };

            // Store the Row_No in the polygon's Tag property
            if (attributes.Table.Columns.Contains("Row_No") && attributes["Row_No"] != DBNull.Value)
            {
                gMapPolygon.Tag = attributes["Row_No"];
            }

            // Update the checkboxes for that layer in each 4 different map
            List<CheckBox> associatedChecks = modülFormu.GetCheckBoxesByIndex(layer_index);
            foreach (var chk in associatedChecks)
            {
                chk.ForeColor = overlayColors[layer_index].BorderColor;
            }

            // Add the polygon to the overlay
            overlay.Polygons.Add(gMapPolygon);

            // Now record the attribute row in the dictionary that corresponds to *this* overlay
            if (overlay == modülFormu.gMapControl_imar.Overlays.FirstOrDefault(o => o == overlay))
            {
                polygonAttributes_imar[gMapPolygon] = attributes;
            }
            else if (overlay == modülFormu.gMapControl_yuk.Overlays.FirstOrDefault(o => o == overlay))
            {
                polygonAttributes_yuk[gMapPolygon] = attributes;
            }

            if (overlay == gridOverlay)
            {
                entire_grid.Add(polygon);
            }
        }


        private void AddLineStringToOverlay_kml(string coordinatesString, GMapOverlay overlay)
        {
            var points = coordinatesString.Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(coord =>
                {
                    var parts = coord.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[0], out double lon) && double.TryParse(parts[1], out double lat))
                    {
                        return new PointLatLng(lat, lon);
                    }
                    return (PointLatLng?)null;
                })
                .Where(p => p.HasValue)
                .Select(p => p.Value)
                .ToList();


            // Get the base border color
            System.Drawing.Color borderColor = overlayColors[layer_index].BorderColor;

            // Set the alpha value to 50 for 20% opacity
            System.Drawing.Color transparentBorderColor = System.Drawing.Color.FromArgb(opacity, borderColor.R, borderColor.G, borderColor.B);

            var route = new GMapRoute(points, "KmlLineString")
            {
                Stroke = new Pen(transparentBorderColor, 3)
            };

            overlay.Routes.Add(route);
        }

        private static System.Drawing.Color[] GenerateDistinguishableColors(int count)
        {
            System.Drawing.Color[] colors = new System.Drawing.Color[count];
            for (int i = 0; i < count; i++)
            {
                double hue = i * (360.0 / count);
                double saturation = 0.7;
                double value = 0.9;

                int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
                double f = hue / 60 - Math.Floor(hue / 60);

                value *= 255;
                int v = Convert.ToInt32(value);
                int p = Convert.ToInt32(value * (1 - saturation));
                int q = Convert.ToInt32(value * (1 - f * saturation));
                int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

                int r, g, b;
                if (hi == 0) { r = v; g = t; b = p; }
                else if (hi == 1) { r = q; g = v; b = p; }
                else if (hi == 2) { r = p; g = v; b = t; }
                else if (hi == 3) { r = p; g = q; b = v; }
                else if (hi == 4) { r = t; g = p; b = v; }
                else { r = v; g = p; b = q; }

                colors[i] = System.Drawing.Color.FromArgb(255, r, g, b);
            }
            return colors;
        }

        private void AddPolygonToOverlay_kml(string coordinatesString, GMapOverlay overlay, DataRow attributes)
        {
            if (string.IsNullOrEmpty(coordinatesString))
                return;

            var points = coordinatesString.Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(coord =>
                {
                    var parts = coord.Split(',');
                    if (parts.Length >= 2 && double.TryParse(parts[0], out double lon) && double.TryParse(parts[1], out double lat))
                    {
                        return new PointLatLng(lat, lon);
                    }
                    return (PointLatLng?)null;
                })
                .Where(p => p.HasValue)
                .Select(p => p.Value)
                .ToList();

            // Varsayılan renk
            System.Drawing.Color fillColor = overlayColors[layer_index].FillColor;
            System.Drawing.Color borderColor = overlayColors[layer_index].BorderColor;

            // imported_filename ile kontrol et
            if (imported_filename == "İMAR_SONUÇLAR.kml" && currentImarTipiColorMap != null)
            {
                if (attributes.Table.Columns.Contains("İmar Tipi") && attributes["İmar Tipi"] != DBNull.Value)
                {
                    string imarTipi = attributes["İmar Tipi"].ToString();
                    if (currentImarTipiColorMap.TryGetValue(imarTipi, out System.Drawing.Color mappedColor))
                    {
                        fillColor = mappedColor;
                        borderColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        File.AppendAllText("color_map_log.txt", $"İmar Tipi '{imarTipi}' için renk bulunamadı.\n");
                    }
                }
                else
                {
                    File.AppendAllText("color_map_log.txt", "İmar Tipi sütunu bulunamadı veya değer null.\n");
                }
            }

            fillColor = System.Drawing.Color.FromArgb(opacity, fillColor.R, fillColor.G, fillColor.B);

            GMapPolygon polygon = new GMapPolygon(points, "KmlPolygon")
            {
                Stroke = new Pen(borderColor, 3),
                Fill = new SolidBrush(fillColor)
            };

            // Store the Row_No in the polygon's Tag property as an integer when possible
            if (attributes.Table.Columns.Contains("Row_No") && attributes["Row_No"] != DBNull.Value)
            {
                try
                {
                    polygon.Tag = Convert.ToInt32(attributes["Row_No"]);
                }
                catch
                {
                    // Fallback to raw value if conversion fails
                    polygon.Tag = attributes["Row_No"];
                }
            }
            else
            {
                // If no Row_No is present, set Tag to a fallback sequential value
                try { polygon.Tag = overlay.Polygons.Count + 1; } catch { polygon.Tag = null; }
            }

            overlay.Polygons.Add(polygon);
            polygonAttributes_imar[polygon] = attributes;
            polygonAttributes_yuk[polygon] = attributes;
        }


        public MapWinGIS.Shapefile ConvertOverlayToShapefile(GMapOverlay overlay)
        {
            var shapefile = new MapWinGIS.Shapefile();
            shapefile.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);

            // Ensure attributes are added as fields
            if (polygonAttributes_imar.Count > 0)
            {
                var firstPolygon = polygonAttributes_imar.Keys.First();
                var firstRow = polygonAttributes_imar[firstPolygon];
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
                if (polygonAttributes_imar.TryGetValue(gMapPolygon, out DataRow row))
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
            if (polygonAttributes_imar.Count > 0)
            {
                var firstPolygon = polygonAttributes_imar.Keys.First();
                var firstRow = polygonAttributes_imar[firstPolygon];
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

                if (polygonAttributes_imar.TryGetValue(polygon, out DataRow row))
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


        public async Task LoadKmlFile(string filepath, GMapOverlay kmlOverlay,
            System.Data.DataTable data_table, GMapControl gMapControl)
        {
            if (!File.Exists(filepath))
            {
                MessageBox.Show("KML dosyası bulunamadı!");
                return;
            }

            int row_cnt = 1;

            try
            {
                XDocument kmlDoc = XDocument.Load(filepath);
                // Handle namespace: use empty namespace if xmlns is missing, otherwise use specified
                XNamespace ns = kmlDoc.Root?.Attribute("xmlns")?.Value ?? "";
                XNamespace gx = "http://www.google.com/kml/ext/2.2"; // For 2nd structure's extended namespace

                if (kmlDoc.Root == null || kmlDoc.Root.Name.LocalName != "kml")
                {
                    MessageBox.Show("KML dosyasında kml elementi bulunamadı.");
                    return;
                }

                // Initialize DataTable with base columns
                if (!data_table.Columns.Contains("Row_No")) data_table.Columns.Add("Row_No");
                if (!data_table.Columns.Contains("coordinates")) data_table.Columns.Add("coordinates");

                // Pre-process Schema to add SimpleField columns (for 3rd structure)
                var schema = kmlDoc.Descendants(ns + "Schema").FirstOrDefault();
                if (schema != null)
                {
                    foreach (var simpleField in schema.Elements(ns + "SimpleField"))
                    {
                        string fieldName = simpleField.Attribute("name")?.Value;
                        if (!string.IsNullOrEmpty(fieldName) && !data_table.Columns.Contains(fieldName))
                        {
                            data_table.Columns.Add(fieldName);
                        }
                    }
                }

                // Collect all Placemarks recursively
                var placemarks = new List<XElement>();
                void CollectPlacemarks(XElement element)
                {
                    if (element == null) return;
                    placemarks.AddRange(element.Elements(ns + "Placemark"));
                    foreach (var child in element.Elements().Where(e => e.Name.LocalName == "Folder" || e.Name.LocalName == "Document"))
                    {
                        CollectPlacemarks(child);
                    }
                }

                CollectPlacemarks(kmlDoc.Root);

                if (!placemarks.Any())
                {
                    MessageBox.Show("KML dosyasında Placemark elementi bulunamadı.");
                    return;
                }

                foreach (var placemark in placemarks)
                {
                    var row = data_table.NewRow();
                    row["Row_No"] = row_cnt;

                    // Handle name
                    var nameElement = placemark.Element(ns + "name");
                    if (nameElement != null)
                    {
                        if (!data_table.Columns.Contains("name")) data_table.Columns.Add("name");
                        row["name"] = nameElement.Value;
                    }

                    // Handle description (for 3rd structure)
                    var descriptionElement = placemark.Element(ns + "description");
                    if (descriptionElement != null)
                    {
                        if (!data_table.Columns.Contains("description")) data_table.Columns.Add("description");
                        row["description"] = descriptionElement.Value;
                    }

                    // Handle styleUrl
                    var styleUrlElement = placemark.Element(ns + "styleUrl");
                    if (styleUrlElement != null)
                    {
                        if (!data_table.Columns.Contains("styleUrl")) data_table.Columns.Add("styleUrl");
                        row["styleUrl"] = styleUrlElement.Value;
                    }

                    // Handle ExtendedData (for 1st and 3rd structures)
                    var extendedData = placemark.Element(ns + "ExtendedData");
                    if (extendedData != null)
                    {
                        // Process <Data> elements (1st structure)
                        foreach (var data in extendedData.Elements(ns + "Data"))
                        {
                            string name = data.Attribute("name")?.Value;
                            string value = data.Element(ns + "value")?.Value;
                            if (!string.IsNullOrEmpty(name))
                            {
                                if (!data_table.Columns.Contains(name)) data_table.Columns.Add(name);
                                row[name] = value ?? "";
                            }
                        }

                        // Process <SchemaData> and <SimpleData> elements (3rd structure)
                        var schemaData = extendedData.Element(ns + "SchemaData");
                        if (schemaData != null)
                        {
                            foreach (var simpleData in schemaData.Elements(ns + "SimpleData"))
                            {
                                string name = simpleData.Attribute("name")?.Value;
                                string value = simpleData.Value;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    if (!data_table.Columns.Contains(name)) data_table.Columns.Add(name);
                                    row[name] = value ?? "";
                                }
                            }
                        }
                    }

                    // Handle Polygon
                    var polygon = placemark.Element(ns + "Polygon");
                    if (polygon != null)
                    {
                        var coordinatesElement = polygon.Element(ns + "outerBoundaryIs")?.Element(ns + "LinearRing")?.Element(ns + "coordinates");
                        if (coordinatesElement != null)
                        {
                            string coordinatesString = coordinatesElement.Value.Trim();
                            var coords = coordinatesString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(coord =>
                                {
                                    var parts = coord.Split(',');
                                    if (parts.Length >= 2 && double.TryParse(parts[0], out double lon) && double.TryParse(parts[1], out double lat))
                                        return $"{Math.Round(lon, 6)},{Math.Round(lat, 6)}";
                                    return null;
                                })
                                .Where(c => c != null);
                            row["coordinates"] = string.Join(" ; ", coords);
                        }
                    }

                    // Handle Point
                    var point = placemark.Element(ns + "Point");
                    if (point != null)
                    {
                        var coordinatesElement = point.Element(ns + "coordinates");
                        if (coordinatesElement != null)
                        {
                            var parts = coordinatesElement.Value.Trim().Split(',');
                            if (parts.Length >= 2 && double.TryParse(parts[0], out double lon) && double.TryParse(parts[1], out double lat))
                            {
                                string point_coordinates = $"{Math.Round(lon, 6)} ; {Math.Round(lat, 6)}";
                                row["coordinates"] = point_coordinates;
                            }
                        }
                    }

                    // Handle LineString
                    var lineString = placemark.Element(ns + "LineString");
                    if (lineString != null)
                    {
                        var coordinatesElement = lineString.Element(ns + "coordinates");
                        if (coordinatesElement != null)
                        {
                            string coordinatesString = coordinatesElement.Value.Trim();
                            var coords = coordinatesString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(coord =>
                                {
                                    var parts = coord.Split(',');
                                    if (parts.Length >= 2 && double.TryParse(parts[0], out double lon) && double.TryParse(parts[1], out double lat))
                                        return $"{Math.Round(lon, 6)},{Math.Round(lat, 6)}";
                                    return null;
                                })
                                .Where(c => c != null);
                            row["coordinates"] = string.Join(" ; ", coords);
                            AddLineStringToOverlay_kml(coordinatesString, kmlOverlay);
                        }
                    }

                    data_table.Rows.Add(row);
                    row_cnt++;
                }

                // Handle legend for İMAR_SONUÇLAR.kml (after all Placemarks are processed)
                if (Path.GetFileName(filepath) == "İMAR_SONUÇLAR.kml")
                    {
                        var imarTipiValues = data_table.AsEnumerable()
                            .Where(row => row["İmar Tipi"] != DBNull.Value)
                            .Select(row => row["İmar Tipi"].ToString())
                            .Distinct()
                            .ToList();

                        if (imarTipiValues.Any())
                        {
                            System.Drawing.Color[] colors = GenerateDistinguishableColors(imarTipiValues.Count);
                            currentImarTipiColorMap = new Dictionary<string, System.Drawing.Color>();
                            for (int i = 0; i < imarTipiValues.Count; i++)
                            {
                                currentImarTipiColorMap[imarTipiValues[i]] = colors[i];
                            }

                            if (modülFormu.imar_legendPanel.Region == null)
                            {
                                modülFormu.imar_legendPanel = new Panel
                                {
                                    BackColor = System.Drawing.Color.White,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Location = new System.Drawing.Point(gMapControl.Width - 200, 10),
                                    Size = new Size(190, imarTipiValues.Count * 20 + 30),
                                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                                };
                                gMapControl.Controls.Add(modülFormu.imar_legendPanel);
                                modülFormu.imar_legendPanel.BringToFront();
                            }
                            else
                            {
                                modülFormu.imar_legendPanel.Size = new Size(190, imarTipiValues.Count * 20 + 30);
                            }

                            modülFormu.imar_legendPanel.Controls.Clear();

                            System.Windows.Forms.Label titleLabel = new System.Windows.Forms.Label
                            {
                                Text = "İmar Tipleri",
                                Location = new System.Drawing.Point(10, 5),
                                AutoSize = true
                            };
                            modülFormu.imar_legendPanel.Controls.Add(titleLabel);

                            int yOffset = 25;
                            foreach (var kvp in currentImarTipiColorMap)
                            {
                                Panel colorBox = new Panel
                                {
                                    BackColor = kvp.Value,
                                    Location = new System.Drawing.Point(10, yOffset),
                                    Size = new Size(20, 15)
                                };
                                System.Windows.Forms.Label imarTipiLabel = new System.Windows.Forms.Label
                                {
                                    Text = kvp.Key,
                                    Location = new System.Drawing.Point(40, yOffset),
                                    AutoSize = true
                                };
                                modülFormu.imar_legendPanel.Controls.Add(colorBox);
                                modülFormu.imar_legendPanel.Controls.Add(imarTipiLabel);
                                yOffset += 20;
                            }

                            modülFormu.imar_legendPanel.Visible = true;
                        }
                        else
                        {
                            modülFormu.imar_legendPanel?.Hide();
                        }
                    }
                    else
                    {
                        currentImarTipiColorMap = null;
                        if (modülFormu.imar_legendPanel != null)
                            modülFormu.imar_legendPanel.Visible = false;
                    }

                    // Add polygons to overlay (after all Placemarks are processed)
                    for (int i = 0; i < data_table.Rows.Count; i++)
                    {
                        var row = data_table.Rows[i];
                        var coordinates = row["coordinates"]?.ToString();
                        if (!string.IsNullOrEmpty(coordinates))
                        {
                            try
                            {
                                AddPolygonToOverlay_kml(coordinates, kmlOverlay, row);
                            }
                            catch (OutOfMemoryException oom)
                            {
                                // Try to recover and skip this polygon if still failing
                                try { GC.Collect(); GC.WaitForPendingFinalizers(); System.Threading.Thread.Sleep(200); }
                                catch { }
                                // log and continue
                                File.AppendAllText("kml_load_errors.txt", $"OOM while adding polygon row {i}: {oom.Message}\n");
                                continue;
                            }
                            catch (Exception ex)
                            {
                                // Log and continue for malformed geometry or unexpected errors
                                File.AppendAllText("kml_load_errors.txt", $"Error adding polygon row {i}: {ex.Message}\n");
                                continue;
                            }
                        }
                    }
                
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public async Task LoadShapefile(string filepath, GMapOverlay shapeFileOverlay,
            System.Data.DataTable shapefile_datatable, DataGridView dataGridView)
        {
            if (!File.Exists(filepath))
            {
                MessageBox.Show("Herhangi bir dosya bulunamadı. Lütfen tekrardan kontrol ediniz.");
                return;
            }

            if (!shapefile_datatable.Columns.Contains("Row_No"))
            {
                shapefile_datatable.Columns.Add("Row_No");
            }

            var shpReader = new ShapefileDataReader(filepath, new NetTopologySuite.Geometries.GeometryFactory());

            // Initialize the DataTable columns based on the shapefile's attribute fields
            for (int i = 0; i < shpReader.DbaseHeader.NumFields; i++)
            {
                var sütunlar = shpReader.DbaseHeader.Fields[i];
                if (!shapefile_datatable.Columns.Contains(sütunlar.Name))
                {
                    shapefile_datatable.Columns.Add(sütunlar.Name, typeof(string));
                }
            }

            int row_cnt = 1;

            while (shpReader.Read())
            {
                var geometry = shpReader.Geometry;
                DataRow row = shapefile_datatable.NewRow();
                shapefile_datatable = LoadAttributeTable(row, dataGridView, shpReader, shapefile_datatable, row_cnt);

                // Ensure Row_No is set before passing to AddPolygonToOverlay
                row["Row_No"] = row_cnt;

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

                row_cnt++;
            }

            // Log the DataTable to verify Row_No values
            File.WriteAllText("shapefile_datatable.txt", string.Join("\n", 
                shapefile_datatable.Rows.Cast<DataRow>().Select(r => $"Row_No: {r["Row_No"]}")));

            layer_index = Array.FindIndex(tüm_katmanlar_array_imar, s => s == null);
            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 50 adet katman seçilebilmektedir.");
                return;
            }

            //MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(shapeFileOverlay);
            //shapeFileArray_MapWinGIS[layer_index] = myShapefile;
        }



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
            layer_index = Array.FindIndex(tüm_katmanlar_array_imar, s => s == null);
            tüm_katmanlar_array_imar[layer_index] = gridOverlay;
            tüm_katmanlar_array_yuk[layer_index] = gridOverlay;

            // add grid overlay to the specified gmapcontrol objects
            modülFormu.gMapControl_imar.Overlays.Add(gridOverlay);

            // copy the contents of the grid in the imar tab to the grid in the yuk tab
            GMapOverlay grid_overlay_yuk = new GMapOverlay($"grid_overlay_{layer_index + 1}_yuk");
            modülFormu.gMapControl_yuk.Overlays.Add(grid_overlay_yuk);
            CopyOverlayContents(gridOverlay, grid_overlay_yuk, polygonAttributes_imar, polygonAttributes_yuk);

            tüm_katmanlar_array_names[layer_index] = "Grid_" + grid_size + "_" + (layer_index + 1).ToString();
            tüm_katmanlar_datatable[layer_index] = gridTable;

            // Convert gridOverlay to MapWinGIS.Shapefile so that it could be exported by the MapWinGIS
            // built-in function SaveAsEx
            MapWinGIS.Shapefile myShapefile = ConvertOverlayToShapefile(gridOverlay);
            shapeFileArray_MapWinGIS[layer_index] = myShapefile;


            List<CheckBox> associatedChecks = modülFormu.GetCheckBoxesByIndex(layer_index);
            foreach (var chk in associatedChecks)
            {
                chk.Text = tüm_katmanlar_array_names[layer_index];
                chk.Visible = true;
                chk.Checked = true;
            }

            // Mark all categories for update
            //modülFormu.pendingUpdates["imar"] = true; ////////////////////////////////////////////////////////////////////////////////////////
            //modülFormu.pendingUpdates["yuk"] = true;////////////////////////////////////////////////////////////////////////////////////////

            // Update only the active tab immediately
            //modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_imar, "imar");////////////////////////////////////////////////////////////////////////////////////////
            //modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_yuk, "yuk");////////////////////////////////////////////////////////////////////////////////////////

            modülFormu.gMapControl_imar.Refresh();
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

        // Helper method to recursively search for GMapControl in nested containers
        private Microsoft.Web.WebView2.WinForms.WebView2 FindWebViewInContainer(System.Windows.Forms.Control container)
        {
            foreach (System.Windows.Forms.Control control in container.Controls)
            {
                if (control is Microsoft.Web.WebView2.WinForms.WebView2 webViewControl)
                {
                    return webViewControl;
                }

                // Recursively check if the control is a container (e.g., Panel)
                if (control is Panel panel)
                {
                    Microsoft.Web.WebView2.WinForms.WebView2 nestedControl = FindWebViewInContainer(panel);
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
                    // If the control is a GMapControl, return it
                    if (control is Microsoft.Web.WebView2.WinForms.WebView2 webViewObject)
                    {
                        return webViewObject;
                    }

                    // If it's a container, recursively search for a GMapControl inside it
                    if (control is Panel panel)
                    {
                        Microsoft.Web.WebView2.WinForms.WebView2 nestedControl = FindWebViewInContainer(panel);
                        if (nestedControl != null)
                        {
                            return nestedControl;
                        }
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

        public void HighlightPolygon(GMapPolygon polygon, int index, GMapControl gMapControl)
        {
            if (gridOverlay.Polygons.Contains(polygon))
            {
                // Reset previous selected polygon
                foreach (var poly in gridOverlay.Polygons)
                {
                    poly.Stroke = new Pen(overlayColors[index].BorderColor, 3);
                    poly.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[index].FillColor));
                }

                // Highlight new selected polygon
                polygon.Stroke = new Pen(System.Drawing.Color.LawnGreen, 3);
                polygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.LawnGreen));

                gMapControl.Refresh();
            }
            else
            {
                // Find the overlay index for the new polygon
                int newOverlayIndexImar = -1;
                int newOverlayIndexYuk = -1;
                for (int i = 0; i < tüm_katmanlar_array_imar.Length; i++)
                {
                    if (tüm_katmanlar_array_imar[i] != null && tüm_katmanlar_array_imar[i].Polygons.Contains(polygon))
                    {
                        newOverlayIndexImar = i;
                        break;
                    }
                }
                for (int i = 0; i < tüm_katmanlar_array_yuk.Length; i++)
                {
                    if (tüm_katmanlar_array_yuk[i] != null && tüm_katmanlar_array_yuk[i].Polygons.Contains(polygon))
                    {
                        newOverlayIndexYuk = i;
                        break;
                    }
                }

                // Reset previous selected polygon
                if (selectedPolygon != null)
                {
                    // Find the overlay that contains the selectedPolygon in both imar and yuk
                    int overlayIndexImar = -1;
                    int overlayIndexYuk = -1;
                    for (int i = 0; i < tüm_katmanlar_array_imar.Length; i++)
                    {
                        if (tüm_katmanlar_array_imar[i] != null && tüm_katmanlar_array_imar[i].Polygons.Contains(selectedPolygon))
                        {
                            overlayIndexImar = i;
                            break;
                        }
                    }
                    for (int i = 0; i < tüm_katmanlar_array_yuk.Length; i++)
                    {
                        if (tüm_katmanlar_array_yuk[i] != null && tüm_katmanlar_array_yuk[i].Polygons.Contains(selectedPolygon))
                        {
                            overlayIndexYuk = i;
                            break;
                        }
                    }

                    // Reset the selectedPolygon in both imar and yuk if found
                    if (overlayIndexImar != -1 || overlayIndexYuk != -1)
                    {
                        // Use the overlayIndex from either imar or yuk (they should be the same due to CopyOverlayContents)
                        int overlayIndex = overlayIndexImar != -1 ? overlayIndexImar : overlayIndexYuk;
                        string layerName = tüm_katmanlar_array_names[overlayIndex];

                        // Find the corresponding polygons in both imar and yuk
                        GMapPolygon selectedPolygonImar = null;
                        GMapPolygon selectedPolygonYuk = null;
                        if (overlayIndexImar != -1)
                        {
                            selectedPolygonImar = tüm_katmanlar_array_imar[overlayIndexImar].Polygons.FirstOrDefault(p => p.Equals(selectedPolygon));
                        }
                        if (overlayIndexYuk != -1)
                        {
                            selectedPolygonYuk = tüm_katmanlar_array_yuk[overlayIndexYuk].Polygons.FirstOrDefault(p => p.Equals(selectedPolygon));
                        }

                        if (layerName == "İMAR_SONUÇLAR.kml" && currentImarTipiColorMap != null)
                        {
                            // Restore color based on İmar Tipi
                            if (polygonAttributes_yuk.TryGetValue(selectedPolygon, out DataRow row) &&
                                row.Table.Columns.Contains("İmar Tipi") && row["İmar Tipi"] != DBNull.Value)
                            {
                                string imarTipi = row["İmar Tipi"].ToString();
                                if (currentImarTipiColorMap.TryGetValue(imarTipi, out System.Drawing.Color mappedColor))
                                {
                                    if (selectedPolygonImar != null)
                                    {
                                        selectedPolygonImar.Stroke = new Pen(System.Drawing.Color.Black, 3);
                                        selectedPolygonImar.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, mappedColor));
                                    }
                                    if (selectedPolygonYuk != null)
                                    {
                                        selectedPolygonYuk.Stroke = new Pen(System.Drawing.Color.Black, 3);
                                        selectedPolygonYuk.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, mappedColor));
                                    }
                                }
                                else
                                {
                                    // Fallback to default color if İmar Tipi not found
                                    if (selectedPolygonImar != null)
                                    {
                                        selectedPolygonImar.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                        selectedPolygonImar.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                                    }
                                    if (selectedPolygonYuk != null)
                                    {
                                        selectedPolygonYuk.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                        selectedPolygonYuk.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                                    }
                                }
                            }
                            else
                            {
                                // Fallback to default color if attributes not found
                                if (selectedPolygonImar != null)
                                {
                                    selectedPolygonImar.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                    selectedPolygonImar.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                                }
                                if (selectedPolygonYuk != null)
                                {
                                    selectedPolygonYuk.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                    selectedPolygonYuk.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                                }
                            }
                        }
                        else
                        {
                            // Default reset for non-İMAR_SONUÇLAR.kml layers
                            if (selectedPolygonImar != null)
                            {
                                selectedPolygonImar.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                selectedPolygonImar.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                            }
                            if (selectedPolygonYuk != null)
                            {
                                selectedPolygonYuk.Stroke = new Pen(overlayColors[overlayIndex].BorderColor, 3);
                                selectedPolygonYuk.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, overlayColors[overlayIndex].FillColor));
                            }
                        }
                    }
                }

                // Highlight new selected polygon in both imar and yuk
                selectedPolygon = polygon;
                if (newOverlayIndexImar != -1)
                {
                    GMapPolygon polygonImar = tüm_katmanlar_array_imar[newOverlayIndexImar].Polygons.FirstOrDefault(p => p.Equals(polygon));
                    if (polygonImar != null)
                    {
                        polygonImar.Stroke = new Pen(System.Drawing.Color.LawnGreen, 3);
                        polygonImar.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.LawnGreen));
                    }
                }
                if (newOverlayIndexYuk != -1)
                {
                    GMapPolygon polygonYuk = tüm_katmanlar_array_yuk[newOverlayIndexYuk].Polygons.FirstOrDefault(p => p.Equals(polygon));
                    if (polygonYuk != null)
                    {
                        polygonYuk.Stroke = new Pen(System.Drawing.Color.LawnGreen, 3);
                        polygonYuk.Fill = new SolidBrush(System.Drawing.Color.FromArgb(opacity, System.Drawing.Color.LawnGreen));
                    }
                }

                // Refresh both GMapControls if necessary
                if (gMapControl == modülFormu.gMapControl_imar)
                {
                    modülFormu.gMapControl_imar.Refresh();
                    modülFormu.gMapControl_yuk.Refresh();
                }
                else if (gMapControl == modülFormu.gMapControl_yuk)
                {
                    modülFormu.gMapControl_yuk.Refresh();
                    modülFormu.gMapControl_imar.Refresh();
                }
            }
        }

        public void Draw_Polygon(List<PointLatLng> polygonPoints, GMapOverlay polygonOverlay, GMapControl gmap)
        {
            string poligonIsim = $"Poligon_{polygonOverlay.Polygons.Count + 1}";
            GMapPolygon polygon = new GMapPolygon(polygonPoints, poligonIsim)
            {
                Stroke = new Pen(System.Drawing.Color.DarkBlue, 3)
            };

            polygonOverlay.Polygons.Clear();
            polygonOverlay.Polygons.Add(polygon);
            gmap.Refresh();

            if (polygonPoints != null && polygonPoints.Count > 0)
            {
                double minLat = double.MaxValue, maxLat = double.MinValue;
                double minLon = double.MaxValue, maxLon = double.MinValue;

                foreach (var point in polygonPoints)
                {
                    minLat = Math.Min(minLat, point.Lat);
                    maxLat = Math.Max(maxLat, point.Lat);
                    minLon = Math.Min(minLon, point.Lng);
                    maxLon = Math.Max(maxLon, point.Lng);
                }
                var center = ((minLat + maxLat) / 2, (minLon + maxLon) / 2); // (lat, lon) order
                int layerIndex = Array.FindIndex(tüm_katmanlar_array_imar, overlay => overlay == polygonOverlay) >= 0
                    ? Array.FindIndex(tüm_katmanlar_array_imar, overlay => overlay == polygonOverlay)
                    : Array.FindIndex(tüm_katmanlar_array_yuk, overlay => overlay == polygonOverlay);
                if (layerIndex >= 0 && layerIndex < polygonCenterPoints.Length)
                {
                    polygonCenterPoints[layerIndex] = center;
                }
            }
        }


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


        public void CreateHeatmap(GMapOverlay overlay, DataTable dataTable, string columnName,
            Dictionary<GMapPolygon, DataRow> polygonAttributes)
        {
            double[] brackets = { 0, 3, 5, 10, 20, 30, 40, 50, 75, 100, double.PositiveInfinity };
            int bracketCount = brackets.Length - 1; // 10 intervals

            // Now continue with your original logic:
            foreach (GMapPolygon polygon in overlay.Polygons)
            {
                if (polygonAttributes.TryGetValue(polygon, out DataRow attributes))
                {
                    // Check if the DataRow contains the specified column.
                    if (!attributes.Table.Columns.Contains(columnName))
                    {
                        MessageBox.Show("Seçilen yıla ait veri bulunamadı.");
                        break;
                    }

                    string rawValue = attributes[columnName].ToString();

                    // Attempt to parse the string using InvariantCulture
                    if (!string.IsNullOrWhiteSpace(rawValue) &&
                    double.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        // Determine which bracket the value falls into
                        int bracketIndex = -1;
                        for (int i = 0; i < bracketCount; i++)
                        {
                            if (value >= brackets[i] && value < brackets[i + 1])
                            {
                                bracketIndex = i;
                                break;
                            }
                        }

                        if (bracketIndex >= 0)
                        {
                            // Normalize the bracket index to a value between 0 and 1 for color mapping
                            double normalizedValue = (double)bracketIndex / (bracketCount - 1);
                            System.Drawing.Color heatColor = GetHeatmapColor(normalizedValue);
                            polygon.Stroke = new Pen(heatColor, 1);
                            polygon.Fill = new SolidBrush(heatColor);
                            continue;
                        }
                    }
                }

                // If parsing fails or no value is provided, color the polygon with a default gray.
                polygon.Stroke = new Pen(System.Drawing.Color.Gray, 1);
                polygon.Fill = new SolidBrush(System.Drawing.Color.Gray);
            }

            // Refresh the map control to show updated colors.
            modülFormu.gMapControl_yuk.Refresh();
        }

        // In cbs class
        public void UpdateHeatmapLegend()
        {
            // Define the fixed brackets
            double[] brackets = { 0, 3, 5, 10, 20, 30, 40, 50, 75, 100, double.PositiveInfinity };
            string[] bracketLabels = { "0-3", "3-5", "5-10", "10-20", "20-30", "30-40", "40-50", "50-75", "75-100", "100-Inf" };
            int bracketCount = bracketLabels.Length; // Should be 10

            // Ensure the arrays exist (they should have been created in InitializeHeatmapLegendControls)
            if (modülFormu.colorBoxes == null || modülFormu.rangeLabels == null || modülFormu.unitLabel == null)
            {
                MessageBox.Show("Heatmap legend controls not initialized.");
                return;
            }

            // Update the unit label (just ensure it's visible)
            modülFormu.unitLabel.Text = "Yük Yoğunluğu (MW/km²)";
            modülFormu.unitLabel.Visible = true;

            // Update color boxes and range labels for each bracket
            for (int i = 0; i < bracketCount; i++)
            {
                // Calculate color gradient from blue to yellow based on the bracket index
                double normalizedValue = (double)i / (bracketCount - 1);
                System.Drawing.Color color = GetHeatmapColor(normalizedValue);

                // Update the color box
                modülFormu.colorBoxes[i].BackColor = color;
                modülFormu.colorBoxes[i].Visible = true;

                // Update the range label
                modülFormu.rangeLabels[i].Text = bracketLabels[i];
                modülFormu.rangeLabels[i].Visible = true;
            }

            // Force layout update on the legend panel
            modülFormu.legendPanel.PerformLayout();
        }

        public System.Drawing.Color GetHeatmapColor(double normalized)
        {
            // Clamp normalized to the range [0,1]
            normalized = Math.Max(0, Math.Min(1, normalized));

            // Define an alpha value for transparency (0 = fully transparent, 255 = opaque)
            int alpha = 150; // Adjust as needed

            int r, g, b;

            // Split the gradient into two segments:
            // - 0 to 0.5: Blue to Yellow
            // - 0.5 to 1: Yellow to Red
            if (normalized <= 0.5)
            {
                // Segment 1: Blue (0, 0, 255) to Yellow (255, 255, 0)
                // Scale normalized from [0, 0.5] to [0, 1] for this segment
                double segmentValue = normalized / 0.5; // Maps 0->0, 0.5->1

                r = (int)(segmentValue * 255);      // Increases from 0 to 255
                g = (int)(segmentValue * 255);      // Increases from 0 to 255
                b = (int)((1 - segmentValue) * 255); // Decreases from 255 to 0
            }
            else
            {
                // Segment 2: Yellow (255, 255, 0) to Red (255, 0, 0)
                // Scale normalized from [0.5, 1] to [0, 1] for this segment
                double segmentValue = (normalized - 0.5) / 0.5; // Maps 0.5->0, 1->1

                r = 255;                            // Stays at 255
                g = (int)((1 - segmentValue) * 255); // Decreases from 255 to 0
                b = 0;                              // Stays at 0
            }

            return System.Drawing.Color.FromArgb(alpha, r, g, b);
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
                if (polygonAttributes_imar.TryGetValue(polygon, out DataRow attributes))
                {
                    polygonData.Add((polygon, attributes));
                }
            }

            return polygonData;
        }

        // check whether two polygons intersect
        private bool PolygonsIntersect(GMapPolygon polygon1, GMapPolygon polygon2)
        {
            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();

            // Convert polygon1
            var coords1 = polygon1.Points
                .Select(p => new NetTopologySuite.Geometries.Coordinate(p.Lng, p.Lat))
                .ToList();

            if (!AreCoordinatesClosed(coords1))
            {
                coords1.Add(coords1[0]);
            }

            var ntsPolygon1 = geometryFactory.CreatePolygon(coords1.ToArray());

            // Convert polygon2
            var coords2 = polygon2.Points
                .Select(p => new NetTopologySuite.Geometries.Coordinate(p.Lng, p.Lat))
                .ToList();

            if (!AreCoordinatesClosed(coords2))
            {
                coords2.Add(coords2[0]);
            }

            var ntsPolygon2 = geometryFactory.CreatePolygon(coords2.ToArray());

            return ntsPolygon1.Intersects(ntsPolygon2);
        }

        // Helper that checks if the last coordinate equals the first
        private bool AreCoordinatesClosed(List<NetTopologySuite.Geometries.Coordinate> coords)
        {
            if (coords.Count < 2) return false;

            var first = coords[0];
            var last = coords[coords.Count - 1];

            // The standard 'closed' test is that first == last
            // If your data is lat/long, you might do a tolerance-based comparison
            return first.Equals2D(last);
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
                polygonAttributes_imar[resultingPolygon] = resultingAttributes;

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
                polygonAttributes_imar[resultingPolygon] = resultingAttributes;

                resultingPolygon.Stroke = new Pen(System.Drawing.Color.LightSeaGreen, 5);
                resultingPolygon.Fill = new SolidBrush(System.Drawing.Color.FromArgb(50, System.Drawing.Color.Transparent));

            }

            return resultingOverlay;
        }


        public async Task JoinAttributesByLocation(GMapControl gMapControl)
        {
            List<string> selectedColumns = modülFormu.fonksiyonFormu.agrege_olacak_sutunlar;

            modülFormu.firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == modülFormu.firstLayerName);
            modülFormu.secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names,
                name => name == modülFormu.secondLayerName);

            if (modülFormu.firstLayerToJoin == -1 || modülFormu.secondLayerToJoin == -1)
            {
                MessageBox.Show("Seçilen katmanlar bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GMapOverlay firstLayer = tüm_katmanlar_array_imar[modülFormu.firstLayerToJoin];
            GMapOverlay secondLayer = tüm_katmanlar_array_imar[modülFormu.secondLayerToJoin];

            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstLayer, tüm_katmanlar_datatable[modülFormu.firstLayerToJoin]);
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondLayer, tüm_katmanlar_datatable[modülFormu.secondLayerToJoin]);

            List<(GMapPolygon ResultingPolygon, DataRow ResultingAttributes)> joinedData = PerformSpatialJoin(firstLayerData, secondLayerData);

            string firstLayerTag = tüm_katmanlar_array_polygon_tags[modülFormu.firstLayerToJoin];
            string secondLayerTag = tüm_katmanlar_array_polygon_tags[modülFormu.secondLayerToJoin];
            bool isYukJoin = firstLayerTag == "YUK" || secondLayerTag == "YUK";
            int yukLayerIndex = firstLayerTag == "YUK" ? modülFormu.firstLayerToJoin : modülFormu.secondLayerToJoin;
            List<(GMapPolygon Polygon, DataRow Attributes)> yukLayerData = firstLayerTag == "YUK" ? firstLayerData : secondLayerData;

            if (isYukJoin && joinedData.Count > 0)
            {
                var centerPoint = polygonCenterPoints[yukLayerIndex];
                if (centerPoint.Latitude == 0.0 && centerPoint.Longitude == 0.0)
                {
                    MessageBox.Show("YUK katmanının merkez noktası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double minDistance = double.MaxValue;
                int closestRowIndex = -1;

                Console.WriteLine($"YUK Center Point: {centerPoint.Latitude}, {centerPoint.Longitude}");

                for (int i = 0; i < joinedData.Count; i++)
                {
                    var row = joinedData[i].ResultingAttributes;
                    if (row.Table.Columns.Contains("left") && row.Table.Columns.Contains("top") &&
                        row.Table.Columns.Contains("right") && row.Table.Columns.Contains("bottom"))
                    {
                        if (double.TryParse(row["left"].ToString(), out double left) &&
                            double.TryParse(row["top"].ToString(), out double top) &&
                            double.TryParse(row["right"].ToString(), out double right) &&
                            double.TryParse(row["bottom"].ToString(), out double bottom))
                        {
                            double centroidLat = (top + bottom) / 2; // Latitude (Y)
                            double centroidLon = (left + right) / 2; // Longitude (X)
                            double distance = CalculateHaversineDistance(
                                centerPoint.Latitude, centerPoint.Longitude,
                                centroidLat, centroidLon);
                            Console.WriteLine($"Row {i}: Centroid ({centroidLat}, {centroidLon}), Distance = {distance} m, Row_No = {row["Row_No"]}");
                            if (distance < minDistance || closestRowIndex == -1)
                            {
                                minDistance = distance;
                                closestRowIndex = i;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Row {i}: Failed to parse left, top, right, bottom: {row["left"]}, {row["top"]}, {row["right"]}, {row["bottom"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Row {i}: Missing left, top, right, or bottom columns");
                    }
                }

                Console.WriteLine($"Closest Row Index: {closestRowIndex}, Min Distance: {minDistance} m, Row_No of Closest = {joinedData[closestRowIndex].ResultingAttributes["Row_No"]}");

                if (closestRowIndex >= 0 && joinedData[0].ResultingAttributes.Table.Columns.Contains("Pik Demant (kW)"))
                {
                    var pikDemantValue = yukLayerData[0].Attributes["Pik Demant (kW)"];
                    for (int i = 0; i < joinedData.Count; i++)
                    {
                        var row = joinedData[i].ResultingAttributes;
                        row["Pik Demant (kW)"] = i == closestRowIndex ? pikDemantValue : 0.0;
                    }
                    Console.WriteLine($"Assigned Pik Demant (kW) = {pikDemantValue} to Row {closestRowIndex} (Row_No = {joinedData[closestRowIndex].ResultingAttributes["Row_No"]})");
                }
            }

            GMapOverlay resultingOverlay = CreateResultingOverlay(joinedData);
            resultingOverlay.Id = $"PolygonLayer_{layer_index + 1}";

            layer_index = Array.FindIndex(tüm_katmanlar_array_imar, i => i == null);
            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            string overlayTag = "JOINED";
            string[] validBaseTags = { "YGA", "YUK", "KENTSEL_DONUSUM" };

            if (validBaseTags.Contains(firstLayerTag))
            {
                overlayTag = $"{firstLayerTag}_JOINED";
            }
            else if (validBaseTags.Contains(secondLayerTag))
            {
                overlayTag = $"{secondLayerTag}_JOINED";
            }
            else
            {
                overlayTag = "DEFAULT_JOINED";
            }

            tüm_katmanlar_array_polygon_tags[layer_index] = overlayTag;
            modülFormu.overlayTags[resultingOverlay] = overlayTag;

            tüm_katmanlar_array_imar[layer_index] = resultingOverlay;
            tüm_katmanlar_array_names[layer_index] = "Birleştirilmiş_Katman_" + (layer_index + 1).ToString();

            System.Data.DataTable joined_data_table = new System.Data.DataTable();
            if (joinedData.Count > 0)
            {
                DataRow firstRow = joinedData[0].ResultingAttributes;
                foreach (DataColumn column in firstRow.Table.Columns)
                {
                    joined_data_table.Columns.Add(column.ColumnName, column.DataType);
                }

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
            else
            {
                Console.WriteLine("JoinAttributesByLocation - Warning: No resulting polygons after spatial join.");
            }

            tüm_katmanlar_datatable[layer_index] = joined_data_table;

            List<System.Windows.Forms.CheckBox> associatedCheckBoxes = modülFormu.GetCheckBoxesByIndex(layer_index);
            if (associatedCheckBoxes != null)
            {
                foreach (var checkBox in associatedCheckBoxes)
                {
                    checkBox.Checked = true;
                    checkBox.Visible = true;
                    checkBox.Text = tüm_katmanlar_array_names[layer_index];
                    checkBox.Tag = (layer_index + 1).ToString();
                }
            }

            modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_imar, "imar");
            modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_yuk, "yuk");

            MessageBox.Show($"Katmanlar başarıyla birleştirildi! Tag: {overlayTag}");

            gMapControl.Overlays.Add(resultingOverlay);
            gMapControl.Refresh();
        }



        // method to find the aggregate summary measures for each cell within the grid specified
        public async Task JoinAttributesByLocation_summary(GMapControl gMapControl)
        {
            // Assume selectedColumns is populated from the ComboBox selections
            List<string> selectedColumns = modülFormu.fonksiyonFormu.agrege_olacak_sutunlar;

            // find the indices of the layers that are selected in the "jabl-summary" functionality/interface
            // in the "tüm_katmanlar_array_names"
            modülFormu.firstLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == modülFormu.firstLayerName);
            modülFormu.secondLayerToJoin = Array.FindIndex(tüm_katmanlar_array_names, name => name == modülFormu.secondLayerName);

            // extract the first and second overlay layers according to their specified indices
            GMapOverlay firstOverlay = tüm_katmanlar_array_imar[modülFormu.firstLayerToJoin];
            GMapOverlay secondOverlay = tüm_katmanlar_array_imar[modülFormu.secondLayerToJoin];

            // extract the data of the first layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> firstLayerData =
                ExtractPolygonsAndAttributes(firstOverlay, tüm_katmanlar_datatable[modülFormu.firstLayerToJoin]);

            // extract the data of the second layer from the "tüm_katmanlar_datatable" array
            List<(GMapPolygon Polygon, DataRow Attributes)> secondLayerData =
                ExtractPolygonsAndAttributes(secondOverlay, tüm_katmanlar_datatable[modülFormu.secondLayerToJoin]);

            // spatially join the two layers and store the results in the "joinedData" List object
            var joinedData = PerformSpatialJoinWithAggregations(firstLayerData, secondLayerData, selectedColumns);

            // create the resulting overlay with respect to the "joinedData" object
            GMapOverlay resultingOverlay = CreateResultingOverlayWithSummaries(joinedData, selectedColumns);

            // Find the first available slot in the array that holds shapefile overlay layers
            layer_index = Array.FindIndex(tüm_katmanlar_array_imar, i => i == null);

            if (layer_index == -1)
            {
                MessageBox.Show("En fazla 13 adet katman seçilebilmektedir.");
                return;
            }

            // add the resulting layer and its name to the specified arrays
            tüm_katmanlar_array_imar[layer_index] = resultingOverlay;
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

            modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_imar, "imar");
            modülFormu.UpdateCheckboxPositions(modülFormu.checkBoxes_yuk, "yük");

            MessageBox.Show("Katmanlar başarıyla birleştirildi.!");

            gMapControl.Overlays.Add(resultingOverlay);
            gMapControl.Refresh();

        }

        ///////////////////////////////////////////////////////////////////////////////////////

    }
}