using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MapWinGIS;
using NetTopologySuite.IO;
using OfficeOpenXml;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static SLF.ModülFormu;
using DrawingImage = System.Drawing.Image;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        TextBox logTextBox; // Declare logTextBox here
        private double startX = 0, startY = 0;
        public int slfStartYear = 0, slfEndYear = 0;
        private ExcelService _excelService;
        private ExcelService excelService = new ExcelService();
        // form objeleri
        public HomePageForm gir1;
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
        private List<PointLatLng> rulerPoints_Dek = new List<PointLatLng>();
        private GMapOverlay rulerOverlay_stokastik = new GMapOverlay("rulerOverlay_stokastik");
        private GMapOverlay rulerOverlay_ea = new GMapOverlay("rulerOverlay_ea");
        private GMapOverlay rulerOverlay_Dek = new GMapOverlay("rulerOverlay_Dek");
        private GMapRoute rulerRoute_stokastik;
        private GMapRoute rulerRoute_ea;
        private GMapRoute rulerRoute_Dek;
        private bool isRulerEnabled = false; // enable the drawing of a ruler while pushing mouse down   
        private bool isRulerActive = false; // enable the drawing of a ruler
        private GMapOverlay markerOverlay_stokastik = new GMapOverlay("markerOverlay_stokastik");
        private GMapOverlay markerOverlay_ea = new GMapOverlay("markerOverlay_ea");
        private GMapOverlay markerOverlay_Dek = new GMapOverlay("markerOverlay_Dek");
        // variables to be used to create polygonspolygonOverlay_stokastik
        private GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        public GMapOverlay polygonOverlay_stokastik;
        private GMapOverlay polygonOverlay_Dek = new GMapOverlay("polygonOverlay_Dek");
        private List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        //private List<PoligonVeri> poligonlar_ea = new List<PoligonVeri>();
        private List<PointLatLng> polygonPoints_stokastik = new List<PointLatLng>();
        //private List<PoligonVeri> poligonlar_stokastik = new List<PoligonVeri>();
        private List<PointLatLng> polygonPoints_Dek = new List<PointLatLng>();
        public bool isAddingChargingStation = false; // Sadece şarj istasyonu eklenirken true olacak.
        private bool isAddingDekPoint = false; // Sadece dek noktası eklenirken  true olacak.
        // variables to be used to create a grid
        public GMapOverlay bounding_box_overlay;
        private GMapOverlay gridOverlay = new GMapOverlay("grid");
        private GMapPolygon bounding_box_polygon;
        public int grid_size = 250;
        public bool isSelecting_grid = false;
        private PointLatLng starting_point;
        private PointLatLng ending_point;
        int selectedYear;
        // Get the user's profile path
        public string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string targetDirectory;
        private int _selectedYear = -1;
        private string _selectedCity = null;
        private Form popupForm; // easim ekran popup 
        // boolean variable to control the polygon selection by mouse down event
        private bool isSelecting_polygon = false;

        // boolean variable to control the marker/point selection by mouse down event
        private bool isSelecting_marker = false;

        // X and Y coordinates of the center location of the gMapControl object
        public string centerX;
        public string centerY;
        private DataTable veriMonteCarlo;
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
        private bool isDtrLoaded = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            // Başlangıçta butonu devre dışı bırak
            GelecekSimButton.Enabled = false;

            // Checkbox'ları başlangıçta görünmez yap
            checkBox22.Visible = false;
            checkBox23.Visible = false;
            checkBox24.Visible = false;
            checkBox25.Visible = false;

           

            // Checkbox'ları başlangıçta işaretli yap
            checkBox22.Checked = true;
            checkBox23.Checked = true;
            checkBox24.Checked = true;
            checkBox25.Checked = true;

            // Checkbox olaylarını bağla
            checkBox22.CheckedChanged += checkBox_Ac_Home;
            checkBox23.CheckedChanged += checkBox_Ac_Work;
            checkBox24.CheckedChanged += checkBox_Ac_Public;
            checkBox25.CheckedChanged += checkBox_Dc_Fast;

            // ComboBox olaylarını bağla
            comboBox1.SelectedIndexChanged += yilSecimiMonteCarlo;
            comboBox2.SelectedIndexChanged += ilSecimiMonteCarlo;

            // İlk durumda tüm marker'ları göster
            ToggleMarkers("AC-HOME", checkBox22.Checked);
            ToggleMarkers("AC-WORK", checkBox23.Checked);
            ToggleMarkers("AC-PUBLIC", checkBox24.Checked);
            ToggleMarkers("Fast-DC", checkBox25.Checked);
        }
        // Nokta veri yapısı
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                CheckSelections();
            }
        }

        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                CheckSelections();
            }
        }
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

        private string selectedMethod;  // Store the method
        public List<TabPage> hiddenTabs = new List<TabPage>();  // To store hidden tabs
        public ModülFormu() : this("", "")
        {
        }
        // Main constructor with parameters for selectedMethod and tabToSelect
        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {
            InitializeComponent();
            _excelService = new ExcelService();
            InitializeLogTextBox(); // Initialize logTextBox
            this.DoubleBuffered = true;
            this.selectedMethod = selectedMethod;  // Store the method
            // Initialize the maps and other UI components
            InitializeFormComponents();
            InitializeComboBoxes();


            if (!string.IsNullOrEmpty(tabToSelect))
            {
                InitializeTabs(tabToSelect);  // Select the specific tab and hide others
            }
            else
            {
                InitializeFormBasedOnMethod();  // Initialize based on the selected method
            }
        }

        // Initialize all form components (called in the constructors)
        private void InitializeComboBoxes()
        {
            
            // Yıl aralığını ComboBox1'e ekleyin
            var yearList = new List<int>();
            for (int year = slfStartYear; year <= slfEndYear; year++)
            {
                yearList.Add(year);
            }
            comboBox1.DataSource = yearList; // Yıl seçimi için ComboBox1

            // Şehir isimlerini ComboBox2'ye ekleyin
            comboBox2.Items.Clear();
            comboBox2.Items.Add("İzmir");
            comboBox2.Items.Add("Eskişehir");
        }
        private void InitializeFormComponents()
        {
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);
            InitializeGMap(gMapControl_Dek);

            SortTabPagesAlphabetically(Modül_Tabları, true);
            // Enable double buffering for the form to reduce flickering
            this.DoubleBuffered = true;

            ModuleTabPanel.Paint += new PaintEventHandler(ModuleTabPanel_Paint);

            // Default selected tab
            Modül_Tabları.SelectedTab = tab_girdi;

            // Initialize the arrays and other components
            tüm_katmanlar_array_names = new string[13];
            tüm_katmanlar_array = new GMapOverlay[13];
            shapeFileArray_MapWinGIS = new MapWinGIS.Shapefile[13];
            tüm_katmanlar_datatable = new DataTable[13];

            targetDirectory = System.IO.Path.Combine(userProfilePath, "Desktop");

            buton_stokastik_harita_katmanlar.BringToFront();
            //buton_ea_harita_katmanlar.BringToFront();

            polygonAttributes = new Dictionary<GMapPolygon, DataRow>();
            polygonAttributes_grid = new Dictionary<NetTopologySuite.Geometries.Polygon, DataRow>();

            // Add overlays to the maps
            gMapControl_stokastik.Overlays.Add(rulerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(markerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(gridOverlay);
            stokastik_haritası_checkboxes_init();

            gMapControl_EA.Overlays.Add(rulerOverlay_ea);
            gMapControl_EA.Overlays.Add(markerOverlay_ea);
            gMapControl_EA.Overlays.Add(polygonOverlay_ea);

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            gMapControl_Dek.Overlays.Add(rulerOverlay_Dek);
            gMapControl_Dek.Overlays.Add(markerOverlay_Dek);
            gMapControl_Dek.Overlays.Add(polygonOverlay_Dek);
            gMapControl_Dek.Overlays.Add(gridOverlay); // Eğer bu katman uygun ise
            veri_listesi_seçimi.SelectedIndex = 2;

            // Initialize the tablo_formu instance
            tablo_formu = new Tablo_Formu();
        }
        //VISUAL CHANGES
        private void ModuleTabPanel_Paint(object sender, PaintEventArgs e)
        {
        }
        // Initialize specific tabs and hide others
        // Method to hide a specific item from the ComboBox
        private void HideComboBoxItem(string itemToHide)
        {
            // Create a new list excluding the item you want to hide
            var filteredItems = new List<string>();
            foreach (var item in veri_listesi_seçimi.Items)
            {
                if (item.ToString() != itemToHide) // Check if the item is not the one to hide
                {
                    filteredItems.Add(item.ToString());
                }
            }

            // Clear existing items and add the filtered list
            veri_listesi_seçimi.Items.Clear();
            veri_listesi_seçimi.Items.AddRange(filteredItems.ToArray());

            // Set the default selection, if applicable
            if (filteredItems.Count > 0)
            {
                veri_listesi_seçimi.SelectedIndex = 2; // Select the first item or another index as needed
            }
        }
        private void InitializeTabs(params string[] tabsToSelect)
        {
            foreach (string tabToSelect in tabsToSelect)
            {
                if (Modül_Tabları.TabPages.ContainsKey(tabToSelect))
                {
                    
                    // Make sure the tab is selected in Modül_Tabları
                    Modül_Tabları.SelectedTab = Modül_Tabları.TabPages[tabToSelect];
                    // Custom logic for specific tabs in Modül_Tabları
                    if (tabToSelect == "tab_girdi")
                    {
                        veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
                        veri_listesi_seçimi.Enabled = false;
                    }
                }
                else if (SenaryoModuleTabControl.TabPages.ContainsKey(tabToSelect))
                {
                    // Make sure the tab is selected in SenaryoModuleTabControl
                    SenaryoModuleTabControl.SelectedTab = SenaryoModuleTabControl.TabPages[tabToSelect];
                    // Custom logic for specific tabs in SenaryoModuleTabControl
                }
                else
                {
                    throw new ArgumentException($"Tab '{tabToSelect}' does not exist in either TabControl.");
                }
            }

            // Hide all other tabs except the specified ones
            HideOtherTabs(tabsToSelect);
        }

        // Hide all tabs except the ones specified
        private void HideOtherTabs(params string[] tabsToKeep)
        {
            // Handle Modül_Tabları
            foreach (TabPage tabPage in Modül_Tabları.TabPages.Cast<TabPage>().ToList())
            {
                if (!tabsToKeep.Contains(tabPage.Name) && !hiddenTabs.Contains(tabPage))  // Ensure tab isn't already hidden
                {
                    hiddenTabs.Add(tabPage);
                    Modül_Tabları.TabPages.Remove(tabPage);
                }
            }

            // Handle SenaryoModuleTabControl
            foreach (TabPage tabPage in SenaryoModuleTabControl.TabPages.Cast<TabPage>().ToList())
            {
                if (!tabsToKeep.Contains(tabPage.Name) && !hiddenTabs.Contains(tabPage))  // Ensure tab isn't already hidden
                {
                    hiddenTabs.Add(tabPage);
                    SenaryoModuleTabControl.TabPages.Remove(tabPage);
                }
            }
        }

        // Initialize form based on the selected method
        private void InitializeFormBasedOnMethod()
        {
            if (selectedMethod == "ELF (Ekonometrik)")
            {
                // Show both the "tab_girdi" and "tab_ekonometrik" tabs and hide others
                InitializeTabs("tab_girdi", "tab_ekonometrik", "tab_senaryo", "EkonometrikSenaryoTabPage");
                Modül_Tabları.SelectedTab = tab_girdi;
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Hide the specific item you want to remove
                HideComboBoxItem("Ekonometrik Yük Tahmini Verileri"); // Replace with the actual item you want to hide
                // For SLF, do not hide any tabs. Add logic here if needed.
                // List of tab names to hide
                string[] tabsToHide = {"EkonometrikSenaryoTabPage", "tab_ekonometrik"};

                // Loop through each tab name and remove it if it exists
                foreach (string tabName in tabsToHide)
                {
                    if (SenaryoModuleTabControl.TabPages.ContainsKey(tabName))
                    {
                        SenaryoModuleTabControl.TabPages.RemoveByKey(tabName);
                    }
                    if (Modül_Tabları.TabPages.ContainsKey(tabName))
                    {
                        Modül_Tabları.TabPages.RemoveByKey(tabName);
                    }
                }
            }
        }

        // Restore hidden tabs
        public void RestoreHiddenTabs()
        {
            // Restore tabs for Modül_Tabları
            foreach (TabPage tabPage in hiddenTabs.ToList())
            {
                if (!Modül_Tabları.TabPages.Contains(tabPage))
                {
                    Modül_Tabları.TabPages.Add(tabPage);
                }
            }

            // Restore tabs for SenaryoModuleTabControl
            foreach (TabPage tabPage in hiddenTabs.ToList())
            {
                if (!SenaryoModuleTabControl.TabPages.Contains(tabPage))
                {
                    SenaryoModuleTabControl.TabPages.Add(tabPage);
                }
            }

            hiddenTabs.Clear();  // Clear the list after restoring
        }


        /// <summary>
        /// ELF METHOD Model RScript Run RELATED CHANGES & UPDATES
        /// </summary>


        public class ExcelService
        {
            // Load the worksheet into a DataTable for displaying in DataGridView
            public DataTable LoadWorksheetIntoDataTable(ExcelWorksheet worksheet)
            {
                DataTable dt = new DataTable();

                // Load header
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    dt.Columns.Add(worksheet.Cells[1, col].Text);
                }

                // Load data
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var newRow = dt.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        newRow[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dt.Rows.Add(newRow);
                }

                return dt;
            }

            // Update the worksheet from DataGridView based on user's changes
            public void UpdateWorksheetFromDataGridView(ExcelWorksheet worksheet, DataGridView dgv)
            {
                for (int row = 0; row < dgv.Rows.Count; row++)
                {
                    for (int col = 0; col < dgv.Columns.Count; col++)
                    {
                        var cell = worksheet.Cells[row + 2, col + 1]; // Start at row 2 in Excel (headers are in row 1)
                        var cellDisplayedValue = dgv.Rows[row].Cells[col].Value; // Get displayed value from DataGridView

                        // Update the cell value in Excel
                        if (cellDisplayedValue != null)
                        {
                            // If the displayed value is numeric (including percentages), update the Excel cell
                            if (double.TryParse(cellDisplayedValue.ToString().Replace("%", ""), out double updatedValue))
                            {
                                if (cellDisplayedValue.ToString().Contains("%"))
                                {
                                    // Convert percentage back to a decimal before updating (e.g., 5.2% -> 0.052)
                                    cell.Value = updatedValue / 100;
                                }
                                else
                                {
                                    // Otherwise, use the updated value directly
                                    cell.Value = updatedValue;
                                }
                            }
                            else
                            {
                                // If not numeric, update as text
                                cell.Value = cellDisplayedValue;
                            }
                        }
                    }
                }
            }
        }
        private async void OpenModuleButton_Click(object sender, EventArgs e)
        {
            // Disable the button initially
            OpenModuleButton.Enabled = false;

            string filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

            // Load the data table for the selected type
            var dataTable = girdiModülleri[seçilenVeriTipi].importedDataTable;

            // Check if the data table has any rows
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show($"{seçilenVeriTipi} henüz içeri aktarılmadığından modüle gidilemiyor.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OpenModuleButton.Enabled = true; // Re-enable the button before returning
                return;
            }

            // Asynchronous task to load the Excel package
            await Task.Run(() =>
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    Invoke(new Action(() =>
                    {
                        // Clear previous data
                        ELFMinSenaryoTable.DataSource = null;
                        ELFLowSenaryoTable.DataSource = null;
                        ELFBaseSenaryoTable.DataSource = null;
                        ELFHighSenaryoTable.DataSource = null;
                        ELFMaxSenaryoTable.DataSource = null;
                    }));

                    // Load sheets 2 to 6 into respective DataGridViews
                    for (int i = 1; i <= 5; i++)
                    {
                        var worksheet = package.Workbook.Worksheets[i];
                        DataTable dt = _excelService.LoadWorksheetIntoDataTable(worksheet);

                        Invoke(new Action(() =>
                        {
                            var dataGrids = new[] { ELFMinSenaryoTable, ELFLowSenaryoTable, ELFBaseSenaryoTable, ELFHighSenaryoTable, ELFMaxSenaryoTable };
                            dataGrids[i - 1].DataSource = dt;
                        }));
                    }
                }
            });

            // After loading the data, enable the button
            OpenModuleButton.Enabled = true;

            if (veri_listesi_seçimi.Text == "Ekonometrik Yük Tahmini Verileri")
            {
                Modül_Tabları.SelectedTab = tab_senaryo; // Move this line here to ensure it only executes after loading data
            }
            if (veri_listesi_seçimi.Text == "EA Şarj Verileri")
            {
                Modül_Tabları.SelectedTab = tab_ea;
            }
            if (veri_listesi_seçimi.Text == "DEK Verileri")
            {
                Modül_Tabları.SelectedTab = tab_dek;
            }
            /*            if (veri_listesi_seçimi.Text == "DTR Verileri")
                        {
                            Modül_Tabları.SelectedTab = tab_optDTR;
                        }*/
        }

        /// <summary>
        /// ELF METHOD Model RScript Run RELATED CHANGES & UPDATES
        /// </summary>
        // Helper method for logging

        /// <summary>
        /// ELF METHOD Model RScript Run RELATED CHANGES & UPDATES
        /// </summary>

        // Save button logic to update Excel file with changes from DataGridViews
        private async void ELFScenerioSaveGunaButton_Click(object sender, EventArgs e)
        {
            string originalFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";
            string modifiedFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

            try
            {
                await Task.Run(() =>
                {
                    using (var package = new ExcelPackage(new FileInfo(originalFilePath)))
                    {
                        // Update worksheets with data from DataGridViews
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[0], ELFMinSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[1], ELFLowSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[2], ELFBaseSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[3], ELFHighSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[4], ELFMaxSenaryoTable);

                        // Save the modified Excel file
                        package.SaveAs(new FileInfo(modifiedFilePath));
                    }
                });

                MessageBox.Show("User changes saved to the modified Excel file.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating Excel file: {ex.Message}");
            }
        }
        private async void ELFPredictionShowResultsGunaButton_Click(object sender, EventArgs e)
        {
            // Set cursor to wait
            Cursor.Current = Cursors.WaitCursor;

            // Define paths for the R script and modified input file
            string rScriptPath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Model\begum_model_deneme.R";
            string modifiedInputFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";
            string logFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\SONUÇLAR\script_output_log2.txt";

            try
            {
                // Check if the modified input file exists
                if (!File.Exists(modifiedInputFilePath))
                {
                    LogOutput("The specified modified input file does not exist.");
                    return;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "Rscript.exe", // Ensure Rscript.exe is accessible in your PATH
                        Arguments = $"\"{rScriptPath}\" \"{modifiedInputFilePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                // Log the output and error
                File.AppendAllText(logFilePath, $"Output:\n{output}\nError:\n{error}\n\n");

                if (process.ExitCode != 0)
                {
                    LogOutput($"R script encountered an error. Check the log file for details: {logFilePath}");
                }
                else
                {
                    LogOutput("R script executed successfully.");
                }
            }
            catch (Exception ex)
            {
                LogOutput($"An error occurred while running the R script: {ex.Message}");
            }
            finally
            {
                // Restore cursor to default
                Cursor.Current = Cursors.Default;
            }
        }

        // Helper method for logging output to logTextBox
        private void LogOutput(string message)
        {
            if (logTextBox != null)
            {
                if (logTextBox.InvokeRequired)
                {
                    logTextBox.Invoke(new Action<string>(LogOutput), message);
                }
                else
                {
                    logTextBox.AppendText(message + Environment.NewLine);
                }
            }
        }

        private void InitializeLogTextBox()
        {
            logTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Bottom, // Dock it at the bottom of the form
                Height = 100, // Adjust height as necessary
                ScrollBars = ScrollBars.Vertical // Enable vertical scroll
            };
            this.Controls.Add(logTextBox); // Add to the form controls
        }

        private async void ShowResults_Click(object sender, EventArgs e)
        {
            // Path to the Excel file
            string filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\SONUÇLAR\ELF_Tahmin_Sonuçları_2024-10-21 16_59_43.xlsx";

            // Asynchronous task to load the Excel package
            await Task.Run(() =>
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Clear previous data in the DataGridViews
                    Invoke(new Action(() =>
                    {
                        // Set DataSources to null to clear previous data
                        ELFMinResultsTable.DataSource = null;
                        ELFLowResultsTable.DataSource = null;
                        ELFBaseResultsTable.DataSource = null;
                        ELFHighResultsTable.DataSource = null;
                        ELFMaxResultsTable.DataSource = null;
                    }));

                    // Load sheets into their respective DataGridViews
                    var worksheets = new[] { "Bagımlı_Degisken_Tahminleri_1", "Bagımlı_Degisken_Tahminleri_2", "Bagımlı_Degisken_Tahminleri_3", "Bagımlı_Degisken_Tahminleri_4", "Bagımlı_Degisken_Tahminleri_5" }; // Replace with actual sheet names if needed
                    var dataGrids = new[] { ELFMinResultsTable, ELFLowResultsTable, ELFBaseResultsTable, ELFHighResultsTable, ELFMaxResultsTable };

                    for (int i = 0; i < worksheets.Length; i++)
                    {
                        var worksheet = package.Workbook.Worksheets[worksheets[i]];
                        if (worksheet != null)
                        {
                            DataTable dt = _excelService.LoadWorksheetIntoDataTable(worksheet);

                            Invoke(new Action(() =>
                            {
                                dataGrids[i].DataSource = dt; // Set DataGridView's DataSource
                            }));
                        }
                    }
                }
            });

            // Load images into PictureBox controls after loading the results
            LoadImagesIntoPictureBoxes();

            // Optionally, switch to the results tab
            Modül_Tabları.SelectedTab = tab_ekonometrik;
        }





        /*
                private void ELFPredictionShowResultsGunaButton_Click(object sender, EventArgs e)
                {
                    try
                    {
                        // Set cursor to wait while running the operations
                        Cursor.Current = Cursors.WaitCursor;

                        string modifiedFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

                        // Check if the modified file exists
                        if (!File.Exists(modifiedFilePath))
                        {
                            MessageBox.Show("The modified Excel file does not exist. Please save the scena" +
                                "rio first.");
                            return;
                        }

                        // Run the R script
                        string resultsFilePath = RunModelRScript(modifiedFilePath);

                        if (resultsFilePath == null)
                        {
                            // If R script failed or no results path was returned, stop further execution
                            return;
                        }

                        // Load results into tab_ekonometrik
                        LoadResultsToTabEkonometrik(resultsFilePath);
                    }
                    finally
                    {
                        // Restore cursor to default
                        Cursor.Current = Cursors.Default;
                    }
                }

                // Method to run the R script
                private string RunModelRScript(string modifiedFilePath)
                {
                    string rScriptPath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Model\begum_model_deneme.R";
                    string resultsFilePath = "";

                    // Set up process info
                    var processInfo = new ProcessStartInfo()
                    {
                        FileName = "Rscript.exe",
                        Arguments = $"\"{rScriptPath}\" \"{modifiedFilePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Start the process
                    using (var process = Process.Start(processInfo))
                    {
                        process.OutputDataReceived += (sender, args) =>
                        {
                            if (!string.IsNullOrEmpty(args.Data))
                            {
                                Console.WriteLine(args.Data);
                                resultsFilePath = args.Data;  // Capture the file path
                            }
                        };

                        process.ErrorDataReceived += (sender, args) => Console.WriteLine("ERROR: " + args.Data);

                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }

                    if (string.IsNullOrEmpty(resultsFilePath))
                    {
                        MessageBox.Show("Error: No results file path was generated by the R script.");
                        return null;
                    }

                    MessageBox.Show("R script executed successfully. Results saved in: " + resultsFilePath);
                    return resultsFilePath;  // Return the results file path
                }

                // Method to load results into tab_ekonometrik
                private void LoadResultsToTabEkonometrik(string resultsFilePath)
                {
                    if (!File.Exists(resultsFilePath))
                    {
                        MessageBox.Show("The results file does not exist.");
                        return;
                    }

                    using (var package = new ExcelPackage(new FileInfo(resultsFilePath)))
                    {
                        // Load the corresponding results into each DataGridView
                        LoadWorksheetToDataGridView(package.Workbook.Worksheets[1], ELFMinResultsTable);
                        LoadWorksheetToDataGridView(package.Workbook.Worksheets[2], ELFLowResultsTable);
                        LoadWorksheetToDataGridView(package.Workbook.Worksheets[3], ELFBaseResultsTable);
                        LoadWorksheetToDataGridView(package.Workbook.Worksheets[4], ELFHighResultsTable);
                        LoadWorksheetToDataGridView(package.Workbook.Worksheets[5], ELFMaxResultsTable);
                    }

                    // Switch to the results tab after loading all the data
                    Modül_Tabları.SelectedTab = tab_ekonometrik;
                }

                // Helper method to load data from an Excel worksheet into a DataGridView
                private void LoadWorksheetToDataGridView(ExcelWorksheet worksheet, DataGridView dataGridView)
                {
                    DataTable dt = new DataTable();

                    // Load headers from the first row
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dt.Columns.Add(worksheet.Cells[1, col].Text);
                    }

                    // Load data from the worksheet into the DataTable (starting from row 2)
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var newRow = dt.NewRow();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            newRow[col - 1] = worksheet.Cells[row, col].Text;
                        }
                        dt.Rows.Add(newRow);
                    }

                    // Assign the DataTable as the DataSource of the DataGridView
                    dataGridView.DataSource = dt;
                }
        */



        /// <summary>
        /// ELF METHOD MODEL GRAPH RELATED CHANGES & UPDATES
        /// </summary>
/*        private void LoadImageIntoPictureBox(PictureBox pictureBox, string imagePath)
        {
            if (File.Exists(imagePath))
            {
                using (DrawingImage img = DrawingImage.FromFile(imagePath))
                {
                    pictureBox.Image = new Bitmap(img); // Create a new Bitmap to avoid file lock issues
                }
            }
            else
            {
                MessageBox.Show($"Image not found: {imagePath}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }*/
        public DataTable ReadExcelToDataTable(string filePath)
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Excel file not found: {filePath}");
                return null;
            }

            DataTable dataTable = new DataTable();

            try
            {
                // Load the Excel file
                using (var workbook = new XLWorkbook(filePath))
                {
                    // Get the first worksheet in the Excel file
                    var worksheet = workbook.Worksheet(1);

                    // Read the header (assuming the first row contains column names)
                    bool headerRow = true;
                    foreach (var row in worksheet.RowsUsed())
                    {
                        if (headerRow)
                        {
                            foreach (var cell in row.Cells())
                            {
                                dataTable.Columns.Add(cell.Value.ToString()); // Create columns based on the first row
                            }
                            headerRow = false; // Only process the header row once
                        }
                        else
                        {
                            // Create a new DataRow for each subsequent row in the Excel file
                            DataRow dataRow = dataTable.NewRow();
                            int columnIndex = 0;

                            foreach (var cell in row.Cells())
                            {
                                dataRow[columnIndex] = cell.Value.ToString(); // Assign cell values to the DataRow
                                columnIndex++;
                            }

                            dataTable.Rows.Add(dataRow); // Add DataRow to DataTable
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading Excel file: {ex.Message}");
                return null;
            }

            return dataTable;
        }
        /// <summary>
        /// ELF METHOD RScript SHOW RESULTS RELATED CHANGES & UPDATES END
        /// </summary>
        // Assuming you have a class like this


        private void LoadImagesIntoPictureBoxes()
        {
            // Path to the folder where the images are saved
            string imageFolderPath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Grafik Outputs\";

            // Load images into PictureBox controls with checks
            LoadImageIntoPictureBox(pictureBox1, Path.Combine(imageFolderPath, "bolge_aydınlatma_projections.png"));
            LoadImageIntoPictureBox(pictureBox2, Path.Combine(imageFolderPath, "bolge_mesken_projections.png"));
            LoadImageIntoPictureBox(pictureBox3, Path.Combine(imageFolderPath, "bolge_sanayi_projections.png"));
            LoadImageIntoPictureBox(pictureBox4, Path.Combine(imageFolderPath, "bolge_sulama_projections.png"));
            LoadImageIntoPictureBox(pictureBox5, Path.Combine(imageFolderPath, "bolge_ticarethane_projections.png"));
            // Add more PictureBox assignments as needed
        }

        private void LoadImageIntoPictureBox(PictureBox pictureBox, string imagePath)
        {
            if (File.Exists(imagePath))
            {
                using (DrawingImage img = DrawingImage.FromFile(imagePath))
                {
                    pictureBox.Image = new Bitmap(img); // Create a new Bitmap to avoid file lock issues
                }
            }
            else
            {
                MessageBox.Show($"Image not found: {imagePath}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*        public DataTable ReadCsvToDataTable(string filePath)
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



        /// <summary>
        /// ELF METHOD RScript SHOW RESULTS RELATED CHANGES & UPDATES END
        /// </summary>
        /// 
        private void HomePageButton_Click(object sender, EventArgs e)
        {   
            // Show the confirmation dialog for navigating to the home page
            DialogResult result = MessageBox.Show(
                "Ana sayfaya dönmek istediğinize emin misiniz? Kaydedilmeyen veriler kaybolacaktır!",
                "Ana Sayfaya Dön",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                // Unsubscribe from the FormClosing event to prevent the warning dialog
                this.FormClosing -= ModülFormu_FormClosing;

                // Proceed to open the home page
                HomePageForm homePageForm = new HomePageForm();
                homePageForm.Show();
                this.Hide(); // Hide the current form
            }
            // If the user clicks 'No', do nothing and stay on the current form
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
            if (Modül_Tabları.SelectedTab == tab_ea)
            {
                gMapControl_EA.Refresh();
            }
            if (Modül_Tabları.SelectedTab == tab_dek)
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
                    if (Modül_Tabları.SelectedTab == tab_ea)
                    {
                        gMapControl_EA.Overlays.Add(shapeFileOverlay);
                    }
                    if (Modül_Tabları.SelectedTab == tab_dek)
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
            yenidenAdlandırToolStripMenuItem.Tag = sender_checkbox;

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

        // A simple prompt dialog for renaming
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 320,
                    Height = 160,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                System.Windows.Forms.Label textLabel = new System.Windows.Forms.Label() { Left = 50, Top = 20, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 170 , Height = 70};
                Button confirmation = new Button() { Text = "Tamam", Left = 170, Width = 100, Top = 85, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        private void yenidenAdlandırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem yeniden_adlandir = sender as ToolStripMenuItem;

            if (yeniden_adlandir != null)
            {
                System.Windows.Forms.CheckBox checkBox = yeniden_adlandir.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (tüm_katmanlar_array[checkbox_index] != null)
                {
                    // Prompt the user to input a new name
                    string newName = Prompt.ShowDialog("Yeni isim:", "Katmanı Yeniden Adlandır");

                    if (!string.IsNullOrEmpty(newName))
                    {
                        // Rename the layer in your underlying data structure
                        checkBox.Text = newName; // Adjust this according to your layer data structure
                        tüm_katmanlar_array_names[checkbox_index] = newName;

                        // Refresh the list/tree view
                        checkBox.Refresh();
                    }
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
/*        private void button7_Click(object sender, EventArgs e) //EA EKRANINDAN KALDIRILAN CSV YÜKLE BUTONU EVENTİ 
        {
            Modül_Tabları.SelectTab(tab_girdi);
            veri_listesi_seçimi.Text = "EA Şarj Verileri";
            veri_listesi_seçimi.Enabled = false;
        }*/

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
        //private void Dek_Mesafe_Ölç_Click(object sender, EventArgs e)
        //{
        //    isRulerEnabled = true;
        //    Mesafe_ea.Visible = true;
        //    Mesafe_ea.BringToFront();
        //    mesafe_metre_Dek.Visible = true;
        //    mesafe_metre_Dek.BringToFront();
        //}

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
        private void Dek_Poligon_MouseDown(object sender, MouseEventArgs e)
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
        private void Dek_Nokta_MouseDown(object sender, MouseEventArgs e)
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
        private async void gMapControl_EA_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Priority check for adding a charging station
                if (isAddingChargingStation)
                {
                    // Add the red marker for charging station
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.yellow);
                    marker.ToolTipText = "Yeni Şarj İstasyonu";
                    markerOverlay_ea.Markers.Add(marker);

                    // Create the coordinate object for the popup form
                    NoktaVeri noktaVeri_marker = new NoktaVeri
                    {
                        Enlem = Math.Round(pointClick.Lat, 4),
                        Boylam = Math.Round(pointClick.Lng, 4)
                    };

                    using (ChargingStationPopupForm popupForm = new ChargingStationPopupForm(dataGridView1.DataSource as DataTable, noktaVeri_marker))
                    {
                        if (popupForm.ShowDialog() == DialogResult.OK)
                        {
                            // On success, update the map with the new station details
                            await eaHaritayaVeriYukleAsync();
                        }
                        else if (popupForm.OperationCancelled)
                        {
                            // If canceled, remove the added marker
                            markerOverlay_ea.Markers.Remove(marker);
                        }
                    }

                    // Reset the flag after handling the form
                    isAddingChargingStation = false;
                    return; // Exit to ensure other actions aren’t triggered
                }

                // Marker selection check (only if not adding a charging station)
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
                    NoktaVeri veri = (NoktaVeri)marker.Tag;
                    Console.WriteLine($"Enlem: {veri.Enlem}, Boylam: {veri.Boylam}");
                }

                // Polygon selection check (only if not adding a charging station)
                if (isSelecting_polygon)
                {
                    polygonPoints_ea.Add(pointClick);
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                    markerOverlay_ea.Markers.Add(marker);

                    if (polygonOverlay_ea != null)
                    {
                        gMapControl_EA.Overlays.Remove(polygonOverlay_ea);
                    }

                    layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

                    if (layer_index == -1)
                    {
                        MessageBox.Show("En fazla katman sayısına ulaşıldı. Daha fazla katman ekleyemezsiniz.");
                        return;
                    }

                    polygonOverlay_ea = new GMapOverlay("polygonOverlay_" + layer_index.ToString());
                    gMapControl_EA.Overlays.Add(polygonOverlay_ea);
                    gMapControl_EA.Refresh();

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


        /*        private void gMapControl_EA_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
                {

                    if (e.Button == MouseButtons.Left)
                    {
                        // İşaretleyici (marker) seçimi kontrolü
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

                            // marker.Tag'i NoktaVeri tipine dönüştürüp enlem ve boylamı yazdırabilirsiniz.
                            NoktaVeri veri = (NoktaVeri)marker.Tag;
                            Console.WriteLine($"Enlem: {veri.Enlem}, Boylam: {veri.Boylam}");


                        }
                        else
                        {
                            Console.WriteLine("Bilinmeyen tıklama türü");
                        }
                        // Poligon seçimi kontrolü
                        if (isSelecting_polygon)
                        {
                            polygonPoints_ea.Add(pointClick); // Poligon noktalarını listeye ekle
                            GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue); // Mavi marker ile göster
                            markerOverlay_ea.Markers.Add(marker); // Marker ekle

                            if (polygonOverlay_ea != null)
                            {
                                gMapControl_EA.Overlays.Remove(polygonOverlay_ea); // Eski poligon katmanını kaldır
                            }

                            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null); // Boş katman bul

                            // Dizide boş yer olup olmadığını kontrol et
                            if (layer_index == -1)
                            {
                                MessageBox.Show("En fazla katman sayısına ulaşıldı. Daha fazla katman ekleyemezsiniz.");
                                return;
                            }

                            // Yeni poligon katmanı ekle
                            polygonOverlay_ea = new GMapOverlay("polygonOverlay_" + layer_index.ToString());
                            gMapControl_EA.Overlays.Add(polygonOverlay_ea);
                            gMapControl_EA.Refresh();

                            // Eğer 3 veya daha fazla nokta varsa, poligon çiz
                            if (polygonPoints_ea.Count >= 3)
                            {
                                Draw_Polygon(polygonPoints_ea, polygonOverlay_ea, gMapControl_EA);

                                double area = CalculatePolygonArea(polygonPoints_ea); // Alan hesapla

                                mesafe_metre_ea.Visible = true;
                                Mesafe_ea.Visible = true;
                                Mesafe_ea.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                            }
                        }

                        // Şarj İstasyonu Ekleme kontrolü
                        if (isAddingChargingStation)
                        {
                            GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.red); // Şarj istasyonu için kırmızı marker
                            markerOverlay_ea.Markers.Add(marker);

                            NoktaVeri noktaVeri_marker = new NoktaVeri
                            {
                                Enlem = Math.Round(pointClick.Lat, 4),
                                Boylam = Math.Round(pointClick.Lng, 4)
                            };

                            marker.Tag = noktaVeri_marker;

                            // marker.Tag'i NoktaVeri tipine dönüştürüp enlem ve boylamı yazdırabilirsiniz.
                            NoktaVeri veri = (NoktaVeri)marker.Tag;
                            Console.WriteLine($"Şarj İstasyonu Eklendi - Enlem: {veri.Enlem}, Boylam: {veri.Boylam}");

                            // Bu aşamada şarj istasyonu popup formu veya veri girişi ekranı açılabilir
                            ShowChargingStationPopup(veri);

                            // Şarj istasyonu ekleme işlemi tamamlandığında flag'i kapat
                            isAddingChargingStation = false;
                        }
                    }
                }*/
        private void RemoveMarkerAtPosition(PointLatLng pointClick)
        {
            
            
            double tolerance = 0.0001;  // Tolerance for matching coordinates
            GMapMarker markerToRemove = markerOverlay_ea.Markers
                .FirstOrDefault(m =>
                    Math.Abs(m.Position.Lat - pointClick.Lat) < tolerance &&
                    Math.Abs(m.Position.Lng - pointClick.Lng) < tolerance);

            if (markerToRemove != null)
            {
                markerOverlay_ea.Markers.Remove(markerToRemove);  // Remove the marker
                gMapControl_EA.Refresh();  // Refresh the map to reflect changes
                Console.WriteLine("İşaretleyici kaldırıldı.");
            }
            else
            {
                Console.WriteLine("Kaldırılacak işaretleyici bulunamadı.");
            }
        }
        /*        private async Task ShowChargingStationPopup(NoktaVeri veri)
                {
                    // Yeni bir popup formu oluşturuyoruz
                    Form popupForm = new Form();
                    popupForm.Text = "Şarj İstasyonu Bilgileri";
                    popupForm.Size = new System.Drawing.Size(500, 300);

                    bool isOperationCancelled = true;  // İşlemin iptal edilip edilmediğini kontrol etmek için

                    // Yeni bir DataGridView oluşturuyoruz
                    DataGridView gridView = new DataGridView
                    {
                        Dock = DockStyle.Top,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        AllowUserToAddRows = false // Kullanıcı yeni satır ekleyemesin
                    };

                    // DTR Verileri tablosundan trafo kodlarını almak için
                    DataTable trafoDataTable = GirdiModülü.dataTablesByType["DTR Verileri"];

                    // Trafo kodlarını listeye ekliyoruz
                    List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                                                 .Select(row => row["TRAFO_KODU"].ToString())
                                                 .Distinct()
                                                 .ToList();

                    // DataGridView sütunlarını oluşturuyoruz
                    gridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "ISTASYON_ADI", HeaderText = "ISTASYON_ADI" });
                    gridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "ISTASYON_TIPI", HeaderText = "ISTASYON_TIPI" });
                    gridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "ISTASYON_GUCU", HeaderText = "ISTASYON_GUCU" });

                    // EA_TRAFO_KODU için ComboBox sütunu oluşturuyoruz
                    DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                    {
                        Name = "EA_TRAFO_KODU",
                        HeaderText = "Trafo Kodu",
                        DataSource = trafoKoduListesi,
                        DropDownWidth = 160,
                        FlatStyle = FlatStyle.Flat
                    };
                    gridView.Columns.Add(comboBoxColumn);

                    gridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "EA_X_KOORDINAT", HeaderText = "EA_X_KOORDINAT" });
                    gridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "EA_Y_KOORDINAT", HeaderText = "EA_Y_KOORDINAT" });

                    // Şarj İstasyonu koordinatlarını otomatik olarak dolduruyoruz
                    gridView.Rows.Add();
                    gridView.Rows[0].Cells["EA_X_KOORDINAT"].Value = veri.Enlem; // X Koordinatı
                    gridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value = veri.Boylam; // Y Koordinatı

                    // Butonlar için panel oluştur
                    FlowLayoutPanel buttonPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 40,
                        Padding = new Padding(5)
                    };

                    // Tamam butonu
                    Button btnTamam = new Button
                    {
                        Text = "Tamam",
                        Width = 75,
                        Height = 30,
                    };

                    // İptal butonu
                    Button btnIptal = new Button
                    {
                        Text = "İptal",
                        Width = 75,
                        Height = 30
                    };

                    // Butonları panele ekle
                    buttonPanel.Controls.Add(btnIptal);
                    buttonPanel.Controls.Add(btnTamam);

                    // İptal butonuna tıklanıldığında formu kapat
                    btnIptal.Click += (sender, e) =>
                    {
                        popupForm.Close();
                    };

                    // Eğer DataGridView1'in DataSource'u DataTable değilse, yeni bir DataTable oluştur
                    DataTable dataTable = dataGridView1.DataSource as DataTable;
                    if (dataTable == null)
                    {
                        // Yeni bir DataTable oluştur
                        dataTable = new DataTable();
                        dataTable.Columns.Add("ISTASYON_ADI", typeof(string));
                        dataTable.Columns.Add("ISTASYON_TIPI", typeof(string));
                        dataTable.Columns.Add("ISTASYON_GUCU", typeof(int));
                        dataTable.Columns.Add("EA_TRAFO_KODU", typeof(string));
                        dataTable.Columns.Add("EA_X_KOORDINAT", typeof(double));
                        dataTable.Columns.Add("EA_Y_KOORDINAT", typeof(double));
                        dataGridView1.DataSource = dataTable;
                    }

                    // Tamam butonuna tıklanıldığında veriyi DataGridView1'e ekleyelim
                    btnTamam.Click += (sender, e) =>
                    {
                        foreach (DataGridViewCell cell in gridView.Rows[0].Cells)
                        {
                            if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                            {
                                MessageBox.Show("Lütfen tüm alanları doldurun.");
                                return;
                            }
                        }

                        // Verilerin doğruluğunu kontrol edelim ve uygun tiplere çevirelim
                        if (double.TryParse(gridView.Rows[0].Cells["EA_X_KOORDINAT"].Value.ToString(), out double enlem) &&
                            double.TryParse(gridView.Rows[0].Cells["EA_Y_KOORDINAT"].Value.ToString(), out double boylam) &&
                            int.TryParse(gridView.Rows[0].Cells["ISTASYON_GUCU"].Value.ToString(), out int istasyonGucu))
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["ISTASYON_ADI"] = gridView.Rows[0].Cells["ISTASYON_ADI"].Value.ToString();
                            newRow["ISTASYON_TIPI"] = gridView.Rows[0].Cells["ISTASYON_TIPI"].Value.ToString();
                            newRow["ISTASYON_GUCU"] = istasyonGucu;
                            newRow["EA_TRAFO_KODU"] = gridView.Rows[0].Cells["EA_TRAFO_KODU"].Value.ToString();
                            newRow["EA_X_KOORDINAT"] = enlem;
                            newRow["EA_Y_KOORDINAT"] = boylam;

                            dataTable.Rows.Add(newRow);

                            isOperationCancelled = false;
                            popupForm.Close();
                        }
                        else
                        {
                            MessageBox.Show("Lütfen geçerli değerler girin.");
                        }
                    };

                    // Form kapanırken işlemin iptal edilip edilmediğini kontrol ediyoruz
                    popupForm.FormClosing += (s, e) =>
                    {
                        if (isOperationCancelled)
                        {
                            MessageBox.Show("İşlem iptal edildi.");
                        }
                    };

                    // Kontrolleri forma ekle
                    popupForm.Controls.Add(gridView);
                    popupForm.Controls.Add(buttonPanel);

                    popupForm.ShowDialog();
                    await eaHaritayaVeriYukleAsync();
                }*/
        private async void gMapControl_Dek_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check for adding a DEK point
                if (isAddingDekPoint)
                {
                    // Add the green marker for DEK point
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                    markerOverlay_Dek.Markers.Add(marker);

                    // Create the coordinate object for the popup form
                    NoktaVeri noktaVeri_marker = new NoktaVeri
                    {
                        Enlem = Math.Round(pointClick.Lat, 4),
                        Boylam = Math.Round(pointClick.Lng, 4)
                    };

                    using (DEKCenterPopupForm popupForm = new DEKCenterPopupForm(dataGridView1.DataSource as DataTable, noktaVeri_marker))
                    {
                        if (popupForm.ShowDialog() == DialogResult.OK)
                        {
                            // On success, update the map with the new DEK center details
                            await dekHaritayaVeriYukleAsync();
                        }
                        else if (popupForm.OperationCancelled)
                        {
                            // If canceled, remove the added marker
                            markerOverlay_Dek.Markers.Remove(marker);
                        }
                    }

                    // Reset the flag after handling the form
                    isAddingDekPoint = false;
                    return; // Exit to ensure other actions aren’t triggered
                }

                // Marker selection check (only if not adding a DEK point)
                if (isSelecting_marker)
                {
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                    markerOverlay_Dek.Markers.Add(marker);

                    NoktaVeri noktaVeri_marker = new NoktaVeri
                    {
                        Enlem = Math.Round(pointClick.Lat, 4),
                        Boylam = Math.Round(pointClick.Lng, 4)
                    };

                    marker.Tag = noktaVeri_marker;
                    Console.WriteLine($"Enlem: {noktaVeri_marker.Enlem}, Boylam: {noktaVeri_marker.Boylam}");
                }

                // Polygon selection check (only if not adding a DEK point)
                if (isSelecting_polygon)
                {
                    polygonPoints_Dek.Add(pointClick);
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                    markerOverlay_Dek.Markers.Add(marker);

                    if (polygonOverlay_Dek != null)
                    {
                        gMapControl_Dek.Overlays.Remove(polygonOverlay_Dek);
                    }

                    layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);

                    if (layer_index == -1)
                    {
                        MessageBox.Show("En fazla katman sayısına ulaşıldı. Daha fazla katman ekleyemezsiniz.");
                        return;
                    }

                    polygonOverlay_Dek = new GMapOverlay("polygonOverlay_" + layer_index.ToString());
                    gMapControl_Dek.Overlays.Add(polygonOverlay_Dek);
                    gMapControl_Dek.Refresh();

                    if (polygonPoints_Dek.Count >= 3)
                    {
                        Draw_Polygon(polygonPoints_Dek, polygonOverlay_Dek, gMapControl_Dek);

                        double area = CalculatePolygonArea(polygonPoints_Dek);
                        mesafe_metre_DeK.Visible = true;
                        Mesafe_Dek.Visible = true;
                        Mesafe_Dek.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                    }
                }
            }
        }


        /*        private void gMapControl_Dek_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
                {
                    // Sol tuşla tıklama yapılmazsa işlemi durdur
                    if (e.Button != MouseButtons.Left) return;

                    // Marker ekleme işlemi
                    if (isSelecting_marker && markerOverlay_Dek != null)
                    {
                        GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                        markerOverlay_Dek.Markers.Add(marker);

                        NoktaVeri noktaVeri_marker = new NoktaVeri
                        {
                            Enlem = Math.Round(pointClick.Lat, 4),
                            Boylam = Math.Round(pointClick.Lng, 4)
                        };
                        marker.Tag = noktaVeri_marker;

                        // Marker seçiliyken popup ekranı açmak için StartDekPointPopup fonksiyonunu çağırıyoruz
                        if (isAddingDekPoint)
                        {
                            StartDekPointPopup(noktaVeri_marker);  // Noktayı popup'a gönderiyoruz
                            isAddingDekPoint = false;
                        }
                    }

                    // Poligon çizme işlemi
                    if (isSelecting_polygon)
                    {
                        // İlk olarak poligon tamamlandığında yeni bir çizim için listeyi temizle
                        if (polygonPoints_stokastik == null || polygonPoints_stokastik.Count == 0)
                        {
                            polygonPoints_stokastik = new List<PointLatLng>();
                        }

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

                        // Poligon 3 nokta ve üzerindeyse çiz
                        if (polygonPoints_stokastik.Count >= 3)
                        {
                            Draw_Polygon(polygonPoints_stokastik, polygonOverlay_stokastik, gMapControl_stokastik);
                            double area = CalculatePolygonArea(polygonPoints_stokastik);

                            mesafe_metre_stokastik.Visible = true;
                            Mesafe_stokastik.Visible = true;
                            Mesafe_stokastik.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";

                            // Çizim tamamlandıktan sonra listeyi temizle
                            polygonPoints_stokastik.Clear();
                        }
                    }

                    if (isSelecting_polygon)
                    {
                        if (polygonPoints_Dek == null || polygonPoints_Dek.Count == 0)
                        {
                            polygonPoints_Dek = new List<PointLatLng>();
                        }

                        polygonPoints_Dek.Add(pointClick);

                        if (markerOverlay_Dek != null)
                        {
                            GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                            markerOverlay_Dek.Markers.Add(marker);
                        }

                        if (gMapControl_Dek != null && polygonOverlay_Dek != null)
                        {
                            gMapControl_Dek.Overlays.Remove(polygonOverlay_Dek);
                        }

                        if (tüm_katmanlar_array != null)
                        {
                            layer_index = Array.FindIndex(tüm_katmanlar_array, s => s == null);
                            if (layer_index == -1)
                            {
                                MessageBox.Show("En fazla katman sayısına ulaşıldı. Daha fazla katman ekleyemezsiniz.");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Katman dizisi başlatılmamış.");
                            return;
                        }

                        polygonOverlay_Dek = new GMapOverlay("polygonOverlay_" + layer_index.ToString());

                        if (gMapControl_Dek != null)
                        {
                            gMapControl_Dek.Overlays.Add(polygonOverlay_Dek);
                            gMapControl_Dek.Refresh();

                            if (polygonPoints_Dek.Count >= 3)
                            {
                                Draw_Polygon(polygonPoints_Dek, polygonOverlay_Dek, gMapControl_Dek);
                                double area = CalculatePolygonArea(polygonPoints_Dek);

                                if (mesafe_metre_DeK != null)
                                    mesafe_metre_DeK.Visible = true;

                                if (Mesafe_Dek != null)
                                {
                                    Mesafe_Dek.Visible = true;
                                    Mesafe_Dek.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                                }

                                // Çizim tamamlandıktan sonra listeyi temizle
                                polygonPoints_Dek.Clear();
                            }
                        }
                    }
                }*/
/*        private async Task StartDekPointPopup(NoktaVeri noktaVeri)
        {
            // Yeni bir popup formu oluşturuyoruz
            Form popupForm = new Form();
            popupForm.Text = "DEK Modülü Bilgileri";
            popupForm.Size = new System.Drawing.Size(600, 400);

            bool isOperationCancelled = true; // İşlem iptal durumunu kontrol etmek için

            // Yeni bir DataGridView oluşturuyoruz
            DataGridView gridView = new DataGridView
            {
                Dock = DockStyle.Top,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false, // Kullanıcı yeni satır ekleyemesin
                ColumnCount = 7 // 7 sütun (ComboBox sütunu hariç)
            };

            // Sütun başlıklarını ekliyoruz
            gridView.Columns[0].Name = "ILCE_ADI";
            gridView.Columns[0].HeaderText = "ILCE_ADI";
            gridView.Columns[1].Name = "KAYNAK_TIPI";
            gridView.Columns[1].HeaderText = "KAYNAK_TIPI";
            gridView.Columns[2].Name = "DEK_KURULU_GUCU";
            gridView.Columns[2].HeaderText = "DEK_KURULU_GUCU";
            gridView.Columns[3].Name = "DEK_X_KOORDINAT";
            gridView.Columns[3].HeaderText = "DEK_X_KOORDINAT";
            gridView.Columns[4].Name = "DEK_Y_KOORDINAT";
            gridView.Columns[4].HeaderText = "DEK_Y_KOORDINAT";
            gridView.Columns[5].Name = "DEK_TM_ADI";
            gridView.Columns[5].HeaderText = "DEK_TM_ADI";
            gridView.Columns[6].Name = "DEK_KURULUM_YERI";
            gridView.Columns[6].HeaderText = "DEK_KURULUM_YERI";

            // Trafo Kodu için ComboBox sütunu oluşturuyoruz
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "DEK_BAGLANDIGI_TRAFO_KODU",
                HeaderText = "Bağlandığı Trafo Kodu"
            };

            // DTR Verileri tablosundan trafo kodlarını almak için
            DataTable trafoDataTable = GirdiModülü.dataTablesByType["DTR Verileri"];

            // Geçerli trafo kodlarını bir listeye ekliyoruz
            List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                                         .Select(row => row["TRAFO_KODU"].ToString())
                                         .Distinct()
                                         .ToList();

            // ComboBox sütununa trafo kodlarını ekliyoruz
            comboBoxColumn.Items.AddRange(trafoKoduListesi.ToArray());

            // ComboBox sütununu DataGridView'e ekliyoruz
            gridView.Columns.Add(comboBoxColumn);

            // Sadece 1 satır ekliyoruz (ilk veri girişi için)
            gridView.Rows.Add();

            // Koordinatları direkt NoktaVeri'den dolduruyoruz
            gridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = noktaVeri.Enlem;
            gridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = noktaVeri.Boylam;

            // Tamam butonu oluştur
            Button btnTamam = new Button
            {
                Text = "Tamam",
                Width = 100,
                Height = 30
            };

            // İptal butonu oluştur
            Button btnIptal = new Button
            {
                Text = "İptal",
                Width = 100,
                Height = 30
            };

            // İptal butonuna tıklanıldığında popup formunu kapatalım
            btnIptal.Click += (s, eArgs) =>
            {
                isOperationCancelled = true;
                popupForm.Close();
            };

            // Tamam butonuna tıklanıldığında işlem tamamlansın
            btnTamam.Click += (s, eArgs) =>
            {
                // Tüm alanların doldurulmuş olduğunu kontrol ediyoruz
                foreach (DataGridViewCell cell in gridView.Rows[0].Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        MessageBox.Show("Lütfen tüm alanları doldurun.");
                        return;
                    }
                }

                // Yeni satır oluşturup DataGridView1'e ekleyeceğiz
                DataTable dataTable = dataGridView1.DataSource as DataTable;
                if (dataTable == null)
                {
                    dataTable = new DataTable();
                    dataTable.Columns.Add("ILCE_ADI", typeof(string));
                    dataTable.Columns.Add("KAYNAK_TIPI", typeof(string));
                    dataTable.Columns.Add("DEK_KURULU_GUCU", typeof(double));
                    dataTable.Columns.Add("DEK_X_KOORDINAT", typeof(double));
                    dataTable.Columns.Add("DEK_Y_KOORDINAT", typeof(double));
                    dataTable.Columns.Add("DEK_TM_ADI", typeof(string));
                    dataTable.Columns.Add("DEK_KURULUM_YERI", typeof(string));
                    dataTable.Columns.Add("DEK_BAGLANDIGI_TRAFO_KODU", typeof(string));
                    dataGridView1.DataSource = dataTable;
                }

                DataRow newRow = dataTable.NewRow();
                newRow["ILCE_ADI"] = gridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
                newRow["KAYNAK_TIPI"] = gridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
                newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(gridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
                newRow["DEK_X_KOORDINAT"] = noktaVeri.Boylam; // Boylam from NoktaVeri
                newRow["DEK_Y_KOORDINAT"] = noktaVeri.Enlem;  // Enlem from NoktaVeri
                newRow["DEK_TM_ADI"] = gridView.Rows[0].Cells["DEK_TM_ADI"].Value.ToString();
                newRow["DEK_KURULUM_YERI"] = gridView.Rows[0].Cells["DEK_KURULUM_YERI"].Value.ToString();
                newRow["DEK_BAGLANDIGI_TRAFO_KODU"] = gridView.Rows[0].Cells["DEK_BAGLANDIGI_TRAFO_KODU"].Value.ToString();

                // Yeni satırı DataTable'a ekliyoruz
                dataTable.Rows.Add(newRow);

                isOperationCancelled = false;
                popupForm.Close();
            };

            // Popup formu kapatılmaya çalışıldığında işlem iptal kontrolü yapalım
            popupForm.FormClosing += (s, eArgs) =>
            {
                if (isOperationCancelled)
                {
                    MessageBox.Show("İşlem iptal edildi.");
                }
            };

            // Popup formuna eklemek için GridView, Tamam ve İptal butonlarını ekliyoruz
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };
            panel.Controls.Add(btnTamam);
            panel.Controls.Add(btnIptal);

            popupForm.Controls.Add(panel);
            popupForm.Controls.Add(gridView);

            // Popup formunu gösteriyoruz
            popupForm.ShowDialog();
            await dekHaritayaVeriYukleAsync();
        }*/


        private void gMapControl_EA_MouseDown(object sender, MouseEventArgs e)
        {

            // cetvel eventi tanımlaması
            if (e.Button == MouseButtons.Left && isRulerEnabled == true)
            {

                // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
                isRulerActive = true;

                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);

                // seçilen noktaları bir listeye koy
                rulerPoints_stokastik.Add(point);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker_ea = new GMarkerGoogle(point, GMarkerGoogleType.orange_dot);
                markerOverlay_stokastik.Markers.Add(marker_ea);


                // 2 adet nokta seçildiği anda aralarındaki mesafeyi hesapla, göster, sonrasında
                // ise noktaların tutulduğu listeyi temizle
                if (rulerPoints_stokastik.Count == 2)
                {
                    markerOverlay_stokastik.Markers.Clear();

                    foreach (var rulerPoint in rulerPoints_ea)
                    {
                        GMapMarker marker_1 = new GMarkerGoogle(rulerPoint, GMarkerGoogleType.orange_dot);
                        markerOverlay_stokastik.Markers.Add(marker_1);
                    }

                    rulerRoute_stokastik.Dispose();
                    DrawRuler_ea(rulerOverlay_stokastik, rulerPoints_stokastik);
                    CalculateDistance(gMapControl_stokastik, mesafe_metre_stokastik, rulerPoints_stokastik);
                    rulerPoints_stokastik.Clear();
                    isRulerActive = false;
                }
            }
        }
        //private void HandleLeftClick(PointLatLng pointClick)
        //{
        //    // İşaretleyici (marker) seçimi kontrolü
        //    if (isSelecting_marker)
        //    {
        //        GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
        //        markerOverlay_ea.Markers.Add(marker);

        //        NoktaVeri noktaVeri_marker = new NoktaVeri
        //        {
        //            Enlem = Math.Round(pointClick.Lat, 4),
        //            Boylam = Math.Round(pointClick.Lng, 4)
        //        };

        //        marker.Tag = noktaVeri_marker;
        //        Console.WriteLine("Marker eklendi: Enlem: " + noktaVeri_marker.Enlem + ", Boylam: " + noktaVeri_marker.Boylam);
        //    }

        //    // Diğer işlemler (poligon vs.)
        //    // ...
        //}

        // Sağ tıklama ile marker silme işlemi
        // Genel işaretleyici kaldırma fonksiyonu
        
        
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
        private void gMapControl_Dek_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                ending_point = gMapControl_Dek.FromLocalToLatLng(e.X, e.Y);
                isSelecting_grid = false;
                gMapControl_stokastik.CanDragMap = true;

                // Clear the selection polygon and refresh the map
                gMapControl_Dek.Overlays.Remove(bounding_box_overlay);
                AddGridToMap();
                gMapControl_Dek.Refresh();
            }
        }
        private void gMapControl_ea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                ending_point = gMapControl_EA.FromLocalToLatLng(e.X, e.Y);
                isSelecting_grid = false;
                gMapControl_EA.CanDragMap = true;

                // Clear the selection polygon and refresh the map
                gMapControl_EA.Overlays.Remove(bounding_box_overlay);
                AddGridToMap();
                gMapControl_EA.Refresh();
            }
        }
        private void gMapControl_Dek_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isRulerEnabled)
            {
                // sol tuşa basıldığında nokta seçmeye başla ve cetveli aktif hale getir
                isRulerActive = true;

                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl_Dek.FromLocalToLatLng(e.X, e.Y);

                // seçilen noktaları bir listeye koy
                rulerPoints_Dek.Add(point);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.orange_dot);
                rulerOverlay_Dek.Markers.Add(marker);

                // 2 adet nokta seçildiğinde aralarındaki mesafeyi hesapla ve noktaların tutulduğu listeyi temizle
                if (rulerPoints_Dek.Count == 2)
                {
                    rulerRoute_Dek?.Dispose();
                    DrawRuler_Dek(rulerOverlay_Dek, rulerPoints_Dek);
                    //CalculateDistance(gMapControl_Dek, mesafe_metre_dek, rulerPoints_Dek);
                    rulerPoints_Dek.Clear();
                    isRulerActive = false;
                }
            }
        }

        private void gMapControl_Dek_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRulerActive && rulerPoints_Dek.Count == 1 && isRulerEnabled)
            {
                var point = gMapControl_Dek.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_Dek != null)
                {
                    rulerOverlay_Dek.Routes.Remove(rulerRoute_Dek);
                }
                rulerRoute_Dek = new GMapRoute(new List<PointLatLng> { rulerPoints_Dek[0], point }, "rulerRoute_Dek");
                rulerRoute_Dek.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_Dek.Routes.Add(rulerRoute_Dek);
                gMapControl_Dek.Refresh();
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

/*        private void ButtonKml_Click(object sender, EventArgs e) //EA EKRANINDAN KALDIRILAN KML YÜKLE BUTONU EVENTİ
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "KML Files (*.kml)|*.kml|All files (*.*)|*.*";
            openFileDialog.Title = "Select a KML File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string kmlFilePath = openFileDialog.FileName;
                KMLYukle(kmlFilePath);
            }
        }*/

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
            // Nokta bilgilerini göster
            MessageBox.Show($"Enlem: {nokta.Enlem}\nBoylam: {nokta.Boylam}\nBina Demandi: " +
                $"{nokta.Bina_Demandi}\nAbone Sayısı: {nokta.Abone_Sayısı}");

            // Noktayı silmek için enlem ve boylamdan PointLatLng oluşturuyoruz
            PointLatLng point = new PointLatLng(nokta.Enlem, nokta.Boylam);

            // Marker'ı verilen noktaya göre kaldırıyoruz
            //RemoveMarkerAtPosition(point);
        }

        private void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_ea)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
                
            }
        }
        private void gMapControl_Dek_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_dek)
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
                // MessageBox.Show("ELF method selected, skipping prerequisites.");
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Logic for SLF selection
                // MessageBox.Show("SLF method selected, prerequisites are required.");
            }
            else
            {
                // Handle other cases or invalid selection
                MessageBox.Show("No valid method selected.");
            }

            girdiModülü = girdiModülleri[seçilenVeriTipi];
            girdiModülü.SlfStartYear = slfStartYear;
            girdiModülü.SlfEndYear = slfEndYear;
            InitializeComboBoxes();
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
        private void DrawRuler_Dek(GMapOverlay rulerOverlay, List<PointLatLng> rulerPoints)
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
        private void gMapControl_Dek_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
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

            // Eğer "DTR Verileri" yüklüyse isDtrLoaded'ı true yap ve renk yeşil olsun
            if (text == "DTR Verileri" && girdiModülü.importedDataTable.Rows.Count > 0)
            {
                isDtrLoaded = true;
                textColor = Color.Green;
            }
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

        // Modül tabları geçişini kontrol etmek için Selecting olayını kullanıyoruz
       

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

            // İstasyon sayılarını doğru gösteriyoruz
            istasyonAdetLabel.Text = $"AC istasyonlar: {greenAc}, DC istasyonlar: {redDc}";

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
        private void calculateChargeStationCount()
        {
            int greenAcCount = 0;
            int redDcCount = 0;
            int blueAcWorkCount = 0;
            int yellowAcPublicCount = 0;

            // Harita üzerindeki tüm marker'ları dolaşarak türlerine göre sayım yap
            foreach (var overlay in gMapControl_EA.Overlays)
            {
                foreach (var marker in overlay.Markers)
                {
                    if (marker is GMarkerGoogle googleMarker)
                    {
                        switch (googleMarker.ToolTipText)
                        {
                            case "AC-HOME":
                                greenAcCount++;
                                break;
                            case "AC-WORK":
                                blueAcWorkCount++;
                                break;
                            case "AC-PUBLIC":
                                yellowAcPublicCount++;
                                break;
                            case "Fast-DC":
                                redDcCount++;
                                break;
                        }
                    }
                }
            }

            // Toplamları yazdırmak için calculateChargeStation metodunu çağırıyoruz
            calculateChargeStation(greenAcCount, redDcCount);

            // Ek sayımlar için ayrıca konsola yazdırıyoruz (isteğe bağlı)
            Console.WriteLine($"AC-HOME Sayısı: {greenAcCount}, AC-WORK Sayısı: {blueAcWorkCount}, AC-PUBLIC Sayısı: {yellowAcPublicCount}, Fast-DC Sayısı: {redDcCount}");
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

                if (gMapControl_EA.Overlays.Contains(eaOverlay))
                {
                    gMapControl_EA.Overlays.Remove(eaOverlay);
                }

                DataTable eaData = await Task.Run(() => DataGridViewToDataTable(dataGridView1));

                if (eaData != null && eaData.Rows.Count > 0)
                {
                    greenAc = 0;  // Sayaçları sıfırla
                    redDc = 0;

                    Invoke(new Action(() =>
                    {
                        foreach (DataRow row in eaData.Rows)
                        {
                            if (!eaData.Columns.Contains("EA_X_KOORDINAT") ||
                                !eaData.Columns.Contains("EA_Y_KOORDINAT") ||
                                !eaData.Columns.Contains("ISTASYON_GUCU"))
                            {
                                MessageBox.Show("Lütfen EA Sarj modülü verilerinizi yükleyin.");
                                return;
                            }

                            if (!girdiModülü.IsNullLike(row["EA_X_KOORDINAT"]) &&
                                !girdiModülü.IsNullLike(row["EA_Y_KOORDINAT"]))
                            {
                                if (double.TryParse(row["EA_X_KOORDINAT"].ToString(), out double x) &&
                                    double.TryParse(row["EA_Y_KOORDINAT"].ToString(), out double y))
                                {
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

                                        if (eaData.Columns.Contains("ISTASYON_ADI") &&
                                            !girdiModülü.IsNullLike(row["ISTASYON_ADI"]))
                                        {
                                            string istasyonAdi = row["ISTASYON_ADI"].ToString();
                                            marker.ToolTipText = istasyonAdi;
                                        }

                                        eaOverlay.Markers.Add(marker);
                                    }
                                }
                            }
                        }

                        gMapControl_EA.Overlays.Add(eaOverlay);
                        gMapControl_EA.Refresh();

                        // Sayaç değerlerini sağ üst köşede göster
                        calculateChargeStation(greenAc, redDc);
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
        private async Task dekHaritayaVeriYukleAsync()
        {
            try
            {
                GMapOverlay dekOverlay = new GMapOverlay("Dek Layer");

                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                if (gMapControl_Dek.Overlays.Contains(dekOverlay))
                {
                    gMapControl_Dek.Overlays.Remove(dekOverlay);
                }

                DataTable dekData = await Task.Run(() => DataGridViewToDataTable(dataGridView1));

                if (dekData != null && dekData.Rows.Count > 0)
                {
                    Invoke(new Action(() =>
                    {
                        foreach (DataRow row in dekData.Rows)
                        {
                            if (!dekData.Columns.Contains("DEK_X_KOORDINAT") ||
                                !dekData.Columns.Contains("DEK_Y_KOORDINAT") ||
                                !dekData.Columns.Contains("KAYNAK_TIPI"))
                            {
                                MessageBox.Show("Lütfen DEK modülü verilerinizi yükleyin.");
                                return;
                            }

                            if (!girdiModülü.IsNullLike(row["DEK_X_KOORDINAT"]) &&
                                !girdiModülü.IsNullLike(row["DEK_Y_KOORDINAT"]))
                            {
                                if (double.TryParse(row["DEK_X_KOORDINAT"].ToString(), out double x) &&
                                    double.TryParse(row["DEK_Y_KOORDINAT"].ToString(), out double y))
                                {
                                    string kaynakTipi = row["KAYNAK_TIPI"].ToString().ToLower();
                                    GMarkerGoogle marker;

                                    // Kaynak tipine göre marker rengini belirle
                                    if (kaynakTipi.Contains("güneş"))
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.yellow);
                                    }
                                    else if (kaynakTipi.Contains("rüzgar"))
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.blue);
                                    }
                                    else if (kaynakTipi.Contains("hidroelektrik"))
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.gray_small);
                                    }
                                    else if (kaynakTipi.Contains("biokütle"))
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.green);
                                    }
                                    else
                                    {
                                        marker = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.red);
                                    }

                                    dekOverlay.Markers.Add(marker);
                                }
                            }
                        }

                        gMapControl_Dek.Overlays.Add(dekOverlay);
                        gMapControl_Dek.Refresh();
                    }));
                }
                else
                {
                    Invoke(new Action(() => MessageBox.Show("Lütfen Dek noktalarını görebilmek için verilerinizi yükleyiniz.")));
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

        private async void Modül_Tabları_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer DTR Verileri henüz yüklenmediyse, kullanıcı sadece "Girdi Modülü" sekmesine erişebilir
            if (!isDtrLoaded && Modül_Tabları.SelectedTab != tab_girdi && selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Sekme geçişini tamamen iptal et
                MessageBox.Show("DTR verileri yüklenmeden diğer sekmelere geçiş yapılamaz.");
                Modül_Tabları.SelectedIndexChanged -= Modül_Tabları_SelectedIndexChanged;
                Modül_Tabları.SelectedTab = tab_girdi;
                Modül_Tabları.SelectedIndexChanged += Modül_Tabları_SelectedIndexChanged;
                return;
            }

            // EA Şarj Modülü tabına tıklanmışsa
            if (Modül_Tabları.SelectedTab.Text == "EA Şarj Modülü")
            {
                if (dataGridView1.DataSource == null)
                {
                    
                    MessageBox.Show("Lütfen önce verileri yükleyin.");
                    return;
                }

                // Harita işlemini başlat
                InitializeComboBoxes();
                await eaHaritayaVeriYukleAsync();
            }
            // DEK Modülü tabına tıklanmışsa
            else if (Modül_Tabları.SelectedTab.Text == "DEK Modülü")
            {
                Console.WriteLine("DEK Modülü");
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("Lütfen önce verileri yükleyin.");
                    return;
                }

                // Harita işlemini başlat
                await dekHaritayaVeriYukleAsync();
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

        private void EAStationAddButton_Click(object sender, EventArgs e)
        {
            // Check if the "EA Şarj Verileri" key exists in the dataTablesByType dictionary
            if (!GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
            {
                MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                return;
            }

            // Use dataGridView1.DataSource as the DataTable instead of eaDataTable
            DataTable dataTable = dataGridView1.DataSource as DataTable;
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                return;
            }

            // Check if we are in the process of adding a charging station
            if (!isAddingChargingStation)
            {
                MessageBox.Show("Lütfen harita üzerinde şarj istasyonu koordinatlarınızı belirleyiniz.");
                isAddingChargingStation = true;
                return; // Exit to wait for the user to click on the map
            }

            // Get the clicked point on the map
            var pointClick = gMapControl_EA.FromLocalToLatLng(MousePosition.X, MousePosition.Y);

            //// Create or get the overlay for charging station markers
            //GMapOverlay chargingStationOverlay = gMapControl_EA.Overlays.FirstOrDefault(o => o.Id == "ChargingStationLayer");
            //if (chargingStationOverlay == null)
            //{
            //    chargingStationOverlay = new GMapOverlay("ChargingStationLayer");
            //    gMapControl_EA.Overlays.Add(chargingStationOverlay);
            //}

            // Refresh the map to show the new marker
            gMapControl_EA.Refresh();

            // Reset the flag after adding the station
            isAddingChargingStation = false;
        }

        /*        private void EAStationAddButton_Click(object sender, EventArgs e)
                {
                    // Check if the "EA Şarj Verileri" key exists in the dataTablesByType dictionary
                    if (!GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
                    {
                        MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                        return;
                    }

                    // Use dataGridView1.DataSource as the DataTable instead of eaDataTable
                    DataTable dataTable = dataGridView1.DataSource as DataTable;
                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                        return;
                    }

                    // Assume a method to get the clicked point on the map
                    var pointClick = gMapControl_EA.FromLocalToLatLng(MousePosition.X, MousePosition.Y);
                    // Indicate that the process of adding a charging station has started
                    if (!isAddingChargingStation)
                    {
                        MessageBox.Show("Lütfen harita üzerinde şarj istasyonu koordinatlarınızı belirleyiniz.");
                        isAddingChargingStation = true;
                    }
                    else
                    {
                        Console.WriteLine("Bilinmeyen tıklama türü");
                    }
                }*/
        private void DEKCenterAddButton_Click(object sender, EventArgs e)
        {
            // Check if the "DTR Verileri" key exists in the dataTablesByType dictionary
            if (!GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri"))
            {
                MessageBox.Show("Lütfen Dağıtık Üretim verilerinizi ekleyin.");
                return;
            }

            // Check if "DTR Verileri" has data
            DataTable dtrDataTable = GirdiModülü.dataTablesByType["DTR Verileri"];
            if (dtrDataTable == null || dtrDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen Dağıtık Üretim verilerinizi ekleyin.");
                return;
            }

            // Check if "DEK Verileri" key exists in the dataTablesByType dictionary
            if (!GirdiModülü.dataTablesByType.ContainsKey("DEK Verileri"))
            {
                MessageBox.Show("Lütfen DEK verilerinizi ekleyin.");
                return;
            }

            // Check if "DEK Verileri" has data
            DataTable dekDataTable = GirdiModülü.dataTablesByType["DEK Verileri"];
            if (dekDataTable == null || dekDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen DEK verilerinizi ekleyin.");
                return;
            }

            // Indicate that the process of marking DEK points has started
            if (!isAddingDekPoint)
            {
                MessageBox.Show("Lütfen harita üzerinde DEK noktası koordinatlarınızı belirleyiniz.");
                isAddingDekPoint = true; // Set flag for DEK point marking
            }
            else
            {
                Console.WriteLine("Bilinmeyen tıklama türü");
            }
        }

        /*        private void DEKCenterAddButton_Click(object sender, EventArgs e)
                {
                    // First, check if "DTR Verileri" exists and has data
                    if (!GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri") ||
                        GirdiModülü.dataTablesByType["DTR Verileri"] == null ||
                        GirdiModülü.dataTablesByType["DTR Verileri"].Rows.Count == 0)
                    {
                        MessageBox.Show("Lütfen Dağıtık Üretim verilerinizi ekleyin.");
                    }
                    // Then check if "DEK Verileri" exists and has data
                    else if (!GirdiModülü.dataTablesByType.ContainsKey("DEK Verileri") ||
                             GirdiModülü.dataTablesByType["DEK Verileri"] == null ||
                             GirdiModülü.dataTablesByType["DEK Verileri"].Rows.Count == 0)
                    {
                        MessageBox.Show("Lütfen DEK verilerinizi ekleyin.");
                    }
                    else
                    {
                        // Start the DEK point marking process if both tables have data
                        MessageBox.Show("Lütfen harita üzerinde DEK noktası koordinatlarınızı belirleyiniz.");
                        isAddingDekPoint = true; // Set flag for DEK point marking
                    }
                }*/



        private async void gelecekSimilasyonGoruntule(object sender, EventArgs e)
        {
            // Checkbox'ları görünür hale getir
            checkBox22.Visible = true;
            checkBox23.Visible = true;
            checkBox24.Visible = true;
            checkBox25.Visible = true;

            // Şehir seçimine göre dosya yolunu ayarla
            string filePath = "";

            if (SelectedCity == "İzmir")
            {
                Console.WriteLine("path burda");
                filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\veriler deneme\arda-dek-ea\ea_evcs_monte_carlo_distribution_2025_2030_5.xlsx";
            }
            else if (SelectedCity == "Eskişehir")
            {
                filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\veriler deneme\arda-dek-ea\ea_montecarlo-deneme-Eskisehir.xlsx";
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir şehir seçiniz.");
                return; // Geçerli bir şehir seçilmediyse işlemi sonlandır
            }

            try
            {
                // Excel dosyasını aç
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Yıl seçimine göre sayfayı seç (SelectedYear değeri, sayfa indeksini temsil eder)
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[SelectedYear];

                    // Veriyi DataTable'a yükle
                    veriMonteCarlo = excelService.LoadWorksheetIntoDataTable(worksheet);
                }

                // Veri başarıyla yüklendiğinde bir bildirim gösterin
                MessageBox.Show("Veri başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                return; // Hata durumunda işlemi sonlandır
            }

            // Yeni bir DataGridView oluştur
            DataGridView dataGridView = new DataGridView
            {
                DataSource = veriMonteCarlo,  // DataTable'ı bağla
                Dock = DockStyle.Fill,        // Formu doldurması için konumunu ayarla
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill // Sütunları otomatik boyutlandır
            };

            // Merkezi Nokta Hesaplama ve Harita Üzerinde Gösterim
            HesaplaMerkezNoktaVeEkle(veriMonteCarlo);
            await HaritaUzerindeSimulasyonGosterimi(veriMonteCarlo);

            // Önceki popupForm varsa kapatın
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
                popupForm.Dispose();  // Eski formu serbest bırak
            }

            // Yeni popupForm'u oluşturun ve açın
            popupForm = new Form
            {
                Text = "Hücre Analizi",
                Width = 800,
                Height = 600
            };

            popupForm.Controls.Add(dataGridView);
            popupForm.Show(); // Yeni pencereyi göster
        }

        private void CheckSelections()
        {
            // Seçimlerin yapıldığını kontrol ederek butonu etkinleştir
            GelecekSimButton.Enabled = SelectedYear != -1 && SelectedCity != null;
        }

        // Yıl seçimi yapıldığında çağrılan metot
        private void yilSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)  // Geçerli bir seçim yapıldığında
            {
                SelectedYear = comboBox1.SelectedIndex;  // Yıl indeksini ayarla
                CheckSelections();  // Seçim durumunu kontrol et
            }
        }

        // Şehir seçimi yapıldığında çağrılan metot
        private void ilSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)  // Geçerli bir seçim yapıldığında
            {
                SelectedCity = comboBox2.SelectedItem.ToString();  // Şehir adını ayarla
                CheckSelections();  // Seçim durumunu kontrol et
            }
        }
        private void HesaplaMerkezNoktaVeEkle(DataTable dataTable)
        {
            // Eğer "MerkezEnlem" ve "MerkezBoylam" sütunları yoksa bu sütunları ekle
            if (!dataTable.Columns.Contains("Enlem"))
            {
                dataTable.Columns.Add("Enlem", typeof(double));
            }
            if (!dataTable.Columns.Contains("Boylam"))
            {
                dataTable.Columns.Add("Boylam", typeof(double));
            }

            // DataTable'daki verileri gezmek için
            foreach (DataRow row in dataTable.Rows)
            {
                // Koordinatları kontrol et ve null değilse işlemi yap
                if (row["left"] != DBNull.Value &&
                    row["top"] != DBNull.Value &&
                    row["right"] != DBNull.Value &&
                    row["bottom"] != DBNull.Value)
                {
                    // Sol, sağ, üst, alt koordinatları double olarak al
                    double left = Convert.ToDouble(row["left"]);
                    double top = Convert.ToDouble(row["top"]);
                    double right = Convert.ToDouble(row["right"]);
                    double bottom = Convert.ToDouble(row["bottom"]);

                    // Merkez koordinatları hesapla
                    double centerLat = (top + bottom) / 2;
                    double centerLng = (left + right) / 2;

                    // Hesaplanan merkez enlem ve boylam değerlerini ilgili satıra ekle
                    row["Enlem"] = centerLat;
                    row["Boylam"] = centerLng;
                }
            }
        }
        private void ToggleMarkers(string markerType, bool isVisible)
        {
            // gMapControl_EA üzerindeki tüm overlay'leri dolaşarak marker'ları kontrol ediyoruz
            foreach (var overlay in gMapControl_EA.Overlays)
            {
                foreach (var marker in overlay.Markers)
                {
                    // Marker, GMarkerGoogle türündeyse ve ToolTipText ile belirtilen türle eşleşiyorsa
                    if (marker is GMarkerGoogle googleMarker && googleMarker.ToolTipText == markerType)
                    {
                        // Marker'ın görünürlük durumunu güncelle
                        googleMarker.IsVisible = isVisible;
                    }
                }
            }

            // Harita güncellenmesi için refresh yapıyoruz
            gMapControl_EA.Refresh();
        }

        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
        {
            // Create or get the overlay for simulation markers
            GMapOverlay simulationOverlay = new GMapOverlay("Simulasyon_Layer");

            // Remove existing overlay if it exists
            if (gMapControl_EA.Overlays.Contains(simulationOverlay))
            {
                gMapControl_EA.Overlays.Remove(simulationOverlay);
            }

            // Add a new overlay for simulation markers
            gMapControl_EA.Overlays.Add(simulationOverlay);

            // Dictionary to hold markers based on their coordinates and types
            Dictionary<(double, double, string), GMarkerGoogle> markerDictionary = new Dictionary<(double, double, string), GMarkerGoogle>();

            // Process the rows in the DataTable
            foreach (DataRow row in veriTablosu.Rows)
            {
                if (row["Enlem"] != DBNull.Value && row["Boylam"] != DBNull.Value)
                {
                    double enlem = Convert.ToDouble(row["Enlem"]);
                    double boylam = Convert.ToDouble(row["Boylam"]);

                    // Check the counts and add markers accordingly
                    bool acHome = row["AC (Home)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Home)_count"]) != 0;
                    bool acWork = row["AC (Work)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Work)_count"]) != 0;
                    bool acPublic = row["AC (Public)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Public)_count"]) != 0;
                    bool fastDc = row["Fast DC_count"] != DBNull.Value && Convert.ToInt32(row["Fast DC_count"]) != 0;

                    // Create markers based on the conditions
                    if (acHome)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.green);
                        marker.ToolTipText = "AC-Home";
                        markerDictionary[(enlem, boylam, "AC-Home")] = marker;
                    }
                    if (acWork)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue);
                        marker.ToolTipText = "AC-Work";
                        markerDictionary[(enlem, boylam, "AC-Work")] = marker;
                    }
                    if (acPublic)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.yellow);
                        marker.ToolTipText = "AC-Public";
                        markerDictionary[(enlem, boylam, "AC-Public")] = marker;
                    }
                    if (fastDc)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.red);
                        marker.ToolTipText = "DC-Fast";
                        markerDictionary[(enlem, boylam, "DC-Fast")] = marker;
                    }
                }
            }

            // Add the created markers to the simulation overlay
            foreach (var marker in markerDictionary.Values)
            {
                simulationOverlay.Markers.Add(marker);
            }

            // Refresh the map control to show the new markers
            Invoke(new Action(() =>
            {
                gMapControl_EA.Refresh();
            }));

            return Task.CompletedTask;
        }



        /*        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
                {
                    // EA için ayrı bir GMap katmanı oluştur

                    GMapOverlay eaOverlay = new GMapOverlay("Simulasyon_Layer");
                    Dictionary<(double, double, string), GMarkerGoogle> markerDictionary = new Dictionary<(double, double, string), GMarkerGoogle>();
                    foreach (DataColumn column in veriTablosu.Columns)
                    {
                        Console.WriteLine(column.ColumnName);
                    }
                        foreach (DataRow row in veriTablosu.Rows)
                    {
                        if (row["Enlem"] != DBNull.Value && row["Boylam"] != DBNull.Value)
                        {
                            double enlem = Convert.ToDouble(row["Enlem"]);
                            double boylam = Convert.ToDouble(row["Boylam"]);

                            bool acHome = row["AC (Home)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Home)_count"]) != 0;
                            bool acWork = row["AC (Work)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Work)_count"]) != 0;
                            bool acPublic = row["AC (Public)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Public)_count"]) != 0;
                            bool fastDc = row["Fast DC_count"] != DBNull.Value && Convert.ToInt32(row["Fast DC_count"]) != 0;

                            // Her kategori için bağımsız olarak marker ekleme
                            if (acHome)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.green);
                                marker.ToolTipText = "AC-Home";

                                markerDictionary[(enlem, boylam, "AC-Home")] = marker;

                            }
                            if (acWork)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue);
                                marker.ToolTipText = "AC-Work";
                                markerDictionary[(enlem, boylam, "AC-Work")] = marker;

                            }
                            if (acPublic)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.yellow);
                                marker.ToolTipText = "AC-Public";
                                markerDictionary[(enlem, boylam, "AC-Public")] = marker;
                            }
                            if (fastDc)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.red);
                                marker.ToolTipText = "DC-Fast";
                                markerDictionary[(enlem, boylam, "DC-Fast")] = marker;
                            }
                        }
                    }

                    // Marker'ları overlay'e ekleyin
                    foreach (var marker in markerDictionary.Values)
                    {
                        eaOverlay.Markers.Add(marker);
                    }

                    // Haritayı güncelleyin
                    Invoke(new Action(() =>
                    {
                        gMapControl_EA.Overlays.Clear();
                        gMapControl_EA.Overlays.Add(eaOverlay);
                        gMapControl_EA.Refresh();
                    }));

                    return Task.CompletedTask;
                }*/



        private void calculateChargeStationWithFilter(int acHomeCount, int acWorkCount, int acPublicCount, int fastDcCount)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => calculateChargeStationWithFilter(acHomeCount, acWorkCount, acPublicCount, fastDcCount)));
                return;
            }

            // Mevcut paneli temizle
            var existingControls = this.Controls.Find("istasyonAdetLabelPanel", true);
            foreach (var control in existingControls)
            {
                this.Controls.Remove(control);
            }

            // Paneli oluştur ve ana formun üzerine ekle
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Location = new System.Drawing.Point(10, 10), // Sol üst köşeye yerleştir
                Size = new System.Drawing.Size(200, 150),    // Sabit boyut belirle
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.FromArgb(200, 255, 255, 255), // Yarı saydam beyaz arka plan
                Name = "istasyonAdetLabelPanel",
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle        // Çerçeve ekleyerek görünürlüğü artır
            };

            

            // Paneli ana forma ekleyin
            this.Controls.Add(panel);
            panel.BringToFront(); // Paneli öne getir
        }

        private void checkBox_Ac_Home(object sender, EventArgs e)
        {
            ToggleMarkers("AC-Home", checkBox22.Checked);
        }

        private void checkBox_Ac_Work(object sender, EventArgs e)
        {
            ToggleMarkers("AC-Work", checkBox23.Checked);
        }

        private void checkBox_Ac_Public(object sender, EventArgs e)
        {
            ToggleMarkers("AC-Public", checkBox24.Checked);

        }

        private void checkBox_Dc_Fast(object sender, EventArgs e)
        {
            ToggleMarkers("DC-Fast", checkBox25.Checked);

        }

        private void SenaryoNewSelectionButton_Click(object sender, EventArgs e)
        {
            Modül_Tabları.SelectedTab = tab_senaryo;
        }

        private void ELFShowGraphsButton_Click(object sender, EventArgs e)
        {
            ELFResultsTabControls.SelectedTab = ELFGraphicOutputsTabPage;
        }

        private void Dek_Grid_Oluştur_Click(object sender, EventArgs e)
        {
            Grid_Seçenekler grid_formu = new Grid_Seçenekler();
            grid_formu.Tag = this;
            grid_formu.Owner = this;
            grid_formu.Show();
            grid_formu.Activate();
            grid_formu.StartPosition = FormStartPosition.CenterParent;
        }
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


