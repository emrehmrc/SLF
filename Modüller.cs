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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading.Tasks;
using OfficeOpenXml;
using DrawingImage = System.Drawing.Image;
using System.Text;
using SLF.Services;
using System.Globalization;
using Newtonsoft.Json;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        private static ModülFormu instance;

        public static ModülFormu Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new ModülFormu();
                }
                return instance;
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ---------------------------------------------- GENEL DEĞİŞKENLER ---------------------------------------------- //

        public HomePageForm ana_menu_form_objesi;
        private MethodForm methodFormObjesi;

        public bool isImported;

        public string ELFrScriptModelPath;
        public string ELFrScriptSenaryolarPath;
        public string ELFResultsFilePath;
        public string ELFSenaryolarFilePath;

        List<string> modulescheck = new List<string>();
        public readonly CBS cbs;
        public int slfStartYear = 0, slfEndYear = 0;

        // point load degerlerini iceren Excel dosyası pathi.
        public string polygonTypesExcelPath;

        System.Windows.Forms.TextBox logTextBox; // Declare logTextBox here --------------
        private ExcelService _excelService;
        private ExcelService excelService = new ExcelService();

        private Form popupForm;
        public Fonksiyon_Oluştur fonksiyonFormu;
        public Tablo_Formu tablo_formu;
        private GirdiModülü girdiModülü;
        public Nokta_Yuk_Bilgi_Formu noktaYukBilgiFormuObjesi;
        public Poligon_Özellik_Tanımlama poligonOzellikFormu;

        public List<PointLatLng> rulerPoints_ea = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_yuk = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_imar = new List<PointLatLng>();

        public GMapOverlay rulerOverlay_ea = new GMapOverlay("rulerOverlay_ea");
        public GMapOverlay rulerOverlay_yuk = new GMapOverlay("rulerOverlay_yuk");
        public GMapOverlay rulerOverlay_imar = new GMapOverlay("rulerOverlay_imar");

        public GMapRoute rulerRoute_ea;
        public GMapRoute rulerRoute_yuk;
        public GMapRoute rulerRoute_imar;

        public GMapOverlay markerOverlay_ea = new GMapOverlay("markerOverlay_ea");
        public GMapOverlay markerOverlay_yuk = new GMapOverlay("markerOverlay_yuk");
        public GMapOverlay markerOverlay_imar = new GMapOverlay("markerOverlay_imar");
        public GMapOverlay markerOverlay_DEK = new GMapOverlay("markerOverlay_DEK");

        public List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        public List<PointLatLng> polygonPoints_imar = new List<PointLatLng>();
        public List<PointLatLng> polygonPoints_yuk = new List<PointLatLng>();
        public List<PointLatLng> polygonPoints_DEK = new List<PointLatLng>();

        public GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        public GMapOverlay polygonOverlay_imar = new GMapOverlay("polygonOverlay_imar");
        public GMapOverlay polygonOverlay_yuk = new GMapOverlay("polygonOverlay_yuk");
        public GMapOverlay polygonOverlay_DEK = new GMapOverlay("polygonOverlay_DEK");

        public Dictionary<int, GMapOverlay> overlaysByLayerIndex = new Dictionary<int, GMapOverlay>();
        public Dictionary<int, System.Windows.Forms.CheckBox[]> checkboxesByLayerIndex = new Dictionary<int, System.Windows.Forms.CheckBox[]>();
        public Dictionary<string, GMapOverlay> overlaysByName = new Dictionary<string, GMapOverlay>();

        public bool isRulerEnabled = false;
        public bool isRulerActive = false;
        public bool isSelecting_polygon = false;

        public bool isSelecting_YGA = false;
        public bool isSelecting_YUK = false;
        private bool isSelecting_marker = false;

        // X and Y coordinates of the center location of the gMapControl object to be used to create a sample
        // kml file to be opened in the Google Earth Desktop
        public string centerX;
        public string centerY;

        // Find the first available slot in the arrays that holds the shapefile overlay layers
        public int layer_index;

        // variables to be used in the "join attributes by location" functionality
        public int firstLayerToJoin;
        public int secondLayerToJoin;
        public string firstLayerName;
        public string secondLayerName;

        // variable to control whichever checkbox/its associated data is selected the latest
        public System.Windows.Forms.CheckBox lastClickedCheckbox;

        private string selectedMethod;  // Store the method
        public List<TabPage> hiddenTabs = new List<TabPage>();  // To store hidden tabs

        // Initialize all checkboxes
        // Class-level declaration of checkbox arrays
        public System.Windows.Forms.CheckBox[] checkBoxes_imar;
        public System.Windows.Forms.CheckBox[] checkBoxes_yuk;

        // nokta ekleme/çıkarma gibi opsiyonların olduğu sağ tık menüsü
        public ContextMenuStrip nokta_menüsü;

        // sol tıkla nokta ekleyebilme kontrolü
        public bool adding_points = false;

        // initialize the count of overlays within each mao
        public int imarOverlayCount = 0;

        // variable to control the simultaneous on/off operations for all the related checkboxes together
        private bool _isSynchronizingCheckboxes = false;

        private Dictionary<string, int> checkboxStartY = new Dictionary<string, int>();
        private Dictionary<string, TabPage> categoryTabPages = new Dictionary<string, TabPage>();
        private TabControl tabControlMain; // Reference to the TabControl

        // In modülFormu
        public Panel[] colorBoxes; // Array to hold color boxes for each bracket
        public System.Windows.Forms.Label[] rangeLabels; // Array to hold range labels for each bracket
        public System.Windows.Forms.Label unitLabel; // Single unit label

        public int load_density_cnt = 0;

        // In modülFormu class
        private int currentYear; // Store the current year from trackBar_Yıllar
        private ToolTip polygonToolTip; // Custom tooltip for displaying polygon data
        private GMapPolygon hoveredPolygon; // Track the currently hovered polygon

        Dictionary<GMapPolygon, DataRow> heatmapPolygonAttributes;
        private int overlayIndex = -1; // yük yoğunluğu sayfası için kullanılan final dosyanın tüm_katmanlar_array_names'teki indexi.


        private GMapPolygon highlightedPolygon; // Track the currently highlighted polygon
        private int lastSelectedCheckboxIndex = -1; // Track the last selected checkbox index

        private GMapOverlay eaOverlay; // Add this as a class-level variable

        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------ INITIALIZATION & GENERAL METHODS ------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        // Main constructor of the Modüller Formu 
        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {

            // initialize the Modul Formu
            InitializeComponent();
            SetupLayout();

            ana_menu_form_objesi = new HomePageForm();
            methodFormObjesi = new MethodForm(ana_menu_form_objesi);

            var yearService = YearService.GetInstance();
            if (this.slfStartYear > 0 && this.slfEndYear > 0)
            {
                // ModülFormu'na dışarıdan atanan değerleri YearService'e aktarma
                yearService.SetYears(this.slfStartYear, this.slfEndYear);
            }
            else
            {
                // YearService'ten değerleri alma
                this.slfStartYear = yearService.slfStartYear;
                this.slfEndYear = yearService.slfEndYear;
            }

            
            if (ana_menu_form_objesi.projectRoot != null)
            {
                polygonTypesExcelPath = Path.Combine(ana_menu_form_objesi.projectRoot, "Excel Files", 
                    "Point Load Karakteristikleri.xlsx", "point_load.xlsx"); 
                // e.g., C:\Users\ehan0\source\repos\emrehmrc\SLF\Excel Files\point_load.xlsx
            }
            else
            {
                // Fallback to a default path if resolution fails
                polygonTypesExcelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                    "point_load.xlsx");
                MessageBox.Show($"Excel dosya yolu çözülemedi. Varsayılan yol kullanılıyor: {polygonTypesExcelPath}", 
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _excelService = new ExcelService();
            InitializeLogTextBox(); // Initialize logTextBox

            this.selectedMethod = selectedMethod;  // Store the method
            InitializeComboBoxes();

            // initialize the instance of a CBS form
            cbs = new CBS(this);
            this.DoubleBuffered = true;

            this.selectedMethod = selectedMethod;  // Store the method
            // Initialize the maps and other UI components
            InitializeFormComponents();

            if (!string.IsNullOrEmpty(tabToSelect))
            {
                InitializeTabs(tabToSelect);  // Select the specific tab and hide others
            }
            else
            {
                InitializeFormBasedOnMethod();  // Initialize based on the selected method
            }

            InitializeCheckboxStartPositions();
            InitializeCategoryTabPages();

            CleanupTemporaryFolders();

            // Initialize the legend elements
            InitializeHeatmapLegendControls();

            // Initialize the tooltip
            polygonToolTip = new ToolTip
            {
                AutoPopDelay = 5000, // Tooltip stays visible for 5 seconds
                InitialDelay = 100,  // Delay before showing the tooltip
                ReshowDelay = 100,   // Delay before showing the tooltip again
                ShowAlways = false    // Show even if the form is not active
            };

            // Initialize currentYear
            currentYear = trackBar_Yıllar.Value;

            // Initialize tab_senaryo accessibility on form load
            UpdateTabSenaryoAccessibility();
        }

        public ModülFormu() : this("", "")
        {
        }

        private void InitializeGMap(GMap.NET.WindowsForms.GMapControl gmap)
        {
            gmap.MapProvider = GMapProviders.GoogleSatelliteMap;
            gmap.ShowCenter = false;
            gmap.MinZoom = 8;
            gmap.Manager.Mode = AccessMode.ServerAndCache;
            gmap.MaxZoom = 20;
            gmap.Zoom = 13;
            gmap.DragButton = MouseButtons.Left;
            gmap.Position = new PointLatLng(38.4644, 27.1114);
        }

        private const float PanelWidthRatio = 0.840f; // 1335/1585 ≈ 84.2% of tab_imar width
        private const float PanelHeightRatio = 0.850f; // 649/677 ≈ 95.9% of tab_imar height
        private const float PanelXOffsetRatio = 0.128f; // 203/1585 ≈ 12.8% (unchanged)
        private const float PanelYOffsetRatio = 0.0635f; // 43/677 ≈ 6.35% (unchanged)

        private void UpdatePanelSize()
        {
            // Get tab_imar client size (accounting for padding/margins if any)
            Rectangle tabClientArea = tab_imar.ClientRectangle;
            int tabWidth = tabClientArea.Width;
            int tabHeight = tabClientArea.Height;

            // Calculate panel size based on ratios
            int targetWidth = (int)(tabWidth * PanelWidthRatio);
            int targetHeight = (int)(tabHeight * PanelHeightRatio);

            // Calculate panel position based on ratios
            int targetX = (int)(tabWidth * PanelXOffsetRatio);
            int targetY = (int)(tabHeight * PanelYOffsetRatio);

            // Apply size to panel_imar
            //panel_imar.Size = new Size(targetWidth, targetHeight);

            // Boundary check: Ensure panel stays within tab_imar and form boundaries
            int margin = 10; // Small margin to prevent touching edges
            int maxX = tabWidth - targetWidth - margin; // Maximum X position to keep panel inside tab_imar
            int maxY = tabHeight - targetHeight - margin; // Maximum Y position to keep panel inside tab_imar

            // Further constrain by form's client size (to prevent overflow outside form)
            Rectangle formClientArea = this.ClientRectangle;
            int formMaxX = formClientArea.Width - targetWidth - margin - (this.Width - this.ClientSize.Width); // Account for form borders
            int formMaxY = formClientArea.Height - targetHeight - margin - (this.Height - this.ClientSize.Height);

            // Use the more restrictive boundary (tab_imar or form)
            maxX = Math.Min(maxX, formMaxX - tab_imar.Location.X); // Adjust for tab_imar's offset in form
            maxY = Math.Min(maxY, formMaxY - tab_imar.Location.Y);

            // Ensure position doesn't go negative
            targetX = Math.Max(0, Math.Min(targetX, maxX));
            targetY = Math.Max(0, Math.Min(targetY, maxY));

            // Apply the constrained position
            //panel_imar.Location = new Point(targetX, targetY);

            // Refresh GMapControl to handle rendering
            gMapControl_imar.Refresh();
        }
        private void SetupLayout()
        {
            // Set initial size based on form size
            UpdatePanelSize();

            // Handle form resize to update constraints
            this.Resize += Form1_Resize;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdatePanelSize();
            gMapControl_imar.Refresh(); // Refresh GMapControl to handle rendering
        }

        private void InitializeCheckboxStartPositions()
        {
            if (checkBoxes_imar[0] != null && checkBoxes_imar[0].Parent != null)
                checkboxStartY["imar"] = checkBoxes_imar[0].Location.Y;
            if (checkBoxes_yuk[0] != null && checkBoxes_yuk[0].Parent != null)
                checkboxStartY["yuk"] = checkBoxes_yuk[0].Location.Y;
        }


        // Initialize all form components (called in the constructors)
        private void InitializeFormComponents()
        {
            // Initialize GMap Controls
            InitializeGMap(gMapControl_EA);
            InitializeGMap(gMapControl_yuk);
            InitializeGMap(gMapControl_imar);
            InitializeGMap(gMapControl_optimalDTR);
            InitializeGMap(gMapControl_DEK);

            // Sort TabPages Alphabetically
            SortTabPagesAlphabetically(Modül_Tabları, true);

            // Enable double buffering to reduce flickering
            // this.DoubleBuffered = true;

            // Set Default Selected Tab
            Modül_Tabları.SelectedTab = tab_girdi;

            // Bring certain buttons to the front
            BringButtonsToFront();

            // Add overlays to the maps
            AddOverlaysToMaps();

            // Initialize checkboxes
            checkboxes_init();

            // Set process priority to High
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // Set default selection for veri_listesi_seçimi
            veri_listesi_seçimi.SelectedIndex = 2;

            // Initialize the tablo_formu instance
            tablo_formu = new Tablo_Formu(this);
        }

        // Helper method to add overlays to the maps
        private void AddOverlaysToMaps()
        {
            // Create a dictionary of overlays for each map control
            var overlays = new Dictionary<GMapControl, List<GMapOverlay>>()
            {
                { gMapControl_DEK, new List<GMapOverlay> { markerOverlay_DEK} },
                { gMapControl_EA, new List<GMapOverlay> { rulerOverlay_ea, markerOverlay_ea} },
                { gMapControl_yuk, new List<GMapOverlay> { polygonOverlay_yuk, rulerOverlay_yuk, markerOverlay_yuk } },
                { gMapControl_imar, new List<GMapOverlay> { polygonOverlay_imar, rulerOverlay_imar, markerOverlay_imar} }
            };

            // Iterate through each map control and add overlays
            foreach (var mapControl in overlays)
            {
                foreach (var overlay in mapControl.Value)
                {
                    mapControl.Key.Overlays.Add(overlay);
                }
            }
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

        // Initialize form based on the selected method
        private void InitializeFormBasedOnMethod()
        {
            if (selectedMethod == "ELF (Ekonometrik)")
            {
                // Show both the "tab_girdi" and "tab_ekonometrik" tabs and hide others
                InitializeTabs("tab_girdi", "tab_ekonometrik", "tab_senaryo", "EkonometrikSenaryoTabPage", 
                    "EkonometrikSonuclarTabPage");
                Modül_Tabları.SelectedTab = tab_girdi;
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Hide the specific item you want to remove
                HideComboBoxItem("Ekonometrik Yük Tahmini Verileri"); // Replace with the actual item you want to hide
                // For SLF, do not hide any tabs. Add logic here if needed.
                // List of tab names to hide
                string[] tabsToHide = { "tab_senaryo", "tab_ekonometrik","EkonometrikSenaryoTabPage",
                    "EkonometrikSonuclarTabPage"};

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


        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            // Prevent infinite recursion if we're programmatically changing checkboxes
            if (_isSynchronizingCheckboxes) return;

            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            // The layer index is stored in cb.Tag (1-based from your code)
            if (!int.TryParse(cb.Tag?.ToString(), out int oneBasedLayerIndex))
                return; // invalid Tag

            // Convert to 0-based
            int layerIndex = oneBasedLayerIndex - 1;
            if (layerIndex < 0) return;

            bool isVisible = cb.Checked;

            // toggle overlay visibility across all four arrays ---
            SetOverlayVisibility(cbs.tüm_katmanlar_array_imar[layerIndex], isVisible);
            SetOverlayVisibility(cbs.tüm_katmanlar_array_yuk[layerIndex], isVisible);

            // refresh all maps ---
            gMapControl_imar.Refresh();
            gMapControl_yuk.Refresh();

            // programmatically change the other two checkboxes in the same slot so that they match the newly toggled state.    
            _isSynchronizingCheckboxes = true;  // guard on

            try
            {
                // We want to find the "sibling" checkboxes at the same index across each map array:
                // e.g. checkBoxes_imar[layerIndex], checkBoxes_yga[layerIndex], etc.
                // But we only do it if they exist (i.e. within bounds).

                // If 'cb' is from the imar array, we set the yga and stokastik arrays' checkboxes.
                // If 'cb' is from the stokastik array, we set the imar and yga arrays' checkboxes, etc.
                // We can do it more generically by always syncing all three.

                if (layerIndex < checkBoxes_imar.Length)
                {
                    // Only set if it's a *different* reference to avoid re-triggering for the same box
                    if (!ReferenceEquals(cb, checkBoxes_imar[layerIndex]))
                    {
                        checkBoxes_imar[layerIndex].Checked = isVisible;
                    }
                }

                if (layerIndex < checkBoxes_yuk.Length)
                {
                    // Only set if it's a *different* reference to avoid re-triggering for the same box
                    if (!ReferenceEquals(cb, checkBoxes_yuk[layerIndex]))
                    {
                        checkBoxes_yuk[layerIndex].Checked = isVisible;
                    }
                }

            }
            finally
            {
                _isSynchronizingCheckboxes = false;  // guard off
            }
        }

        private void InitializeCategoryTabPages()
        {
            // Adjust these names based on your designer
            tabControlMain = Modül_Tabları; // The TabControl containing all tabs
            categoryTabPages["imar"] = tab_imar; // Tab page for "İmar Analizi"
            categoryTabPages["yuk"] = tab_yükHaritası;
        }

        public void checkboxes_init()
        {

            checkBoxes_imar = new System.Windows.Forms.CheckBox[] { checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4,
        checkBox_imar_5, checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9,
        checkBox_imar_10, checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15 };

            checkBoxes_yuk = new System.Windows.Forms.CheckBox[] { checkBox_yuk_1, checkBox_yuk_2, checkBox_yuk_3, checkBox_yuk_4,
        checkBox_yuk_5, checkBox_yuk_6, checkBox_yuk_7, checkBox_yuk_8, checkBox_yuk_9,
        checkBox_yuk_10, checkBox_yuk_11, checkBox_yuk_12, checkBox_yuk_13, checkBox_yuk_14, checkBox_yuk_15 };

            int[] tagValuesForCheckboxes = Enumerable.Range(1, 15).ToArray();

            void initializeCheckBoxes(System.Windows.Forms.CheckBox[] checkBoxes, int[] tagValues)
            {
                for (int i = 0; i < checkBoxes.Length; i++)
                {
                    checkBoxes[i].Tag = tagValues[i];
                    checkBoxes[i].CheckedChanged += checkBox_CheckedChanged;
                    checkBoxes[i].MouseDown += checkBox_MouseDown;
                    checkBoxes[i].ForeColor = cbs.overlayColors[i].BorderColor;
                    checkBoxes[i].Visible = false;
                }
            }

            initializeCheckBoxes(checkBoxes_imar, tagValuesForCheckboxes);
            initializeCheckBoxes(checkBoxes_yuk, tagValuesForCheckboxes);
        }

        private const int CheckboxHeight = 27; // Height of each checkbox (adjust as needed)
        private const int CheckboxSpacing = 8; // Space between checkboxes

        public Dictionary<string, bool> pendingUpdates = new Dictionary<string, bool>
        {
            { "imar", false },
            { "yuk", false }
        };

        public void UpdateCheckboxPositions(CheckBox[] checkBoxes, string mapCategory)
        {
            if (!checkboxStartY.ContainsKey(mapCategory) || !categoryTabPages.ContainsKey(mapCategory))
                return;

            TabPage tabPage = categoryTabPages[mapCategory];

            // If the tab is not currently visible, mark it for a pending update
            if (tabControlMain.SelectedTab != tabPage)
            {
                pendingUpdates[mapCategory] = true;
                return;
            }

            int currentY = checkboxStartY[mapCategory];

            tabControlMain.SuspendLayout();
            tabPage.SuspendLayout();

            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if (checkBoxes[i] != null && checkBoxes[i].Visible)
                {
                    checkBoxes[i].Location = new System.Drawing.Point(checkBoxes[i].Location.X, currentY);
                    currentY += CheckboxHeight + CheckboxSpacing;
                }
            }

            tabPage.ResumeLayout(false);
            tabControlMain.ResumeLayout(true);

            foreach (Control control in tabPage.Controls)
            {
                control.Invalidate();
                control.Update();
            }

            tabControlMain.Invalidate();
            tabControlMain.Update();
            this.Invalidate();
            this.Update();
        }

        private void ClearCheckboxesForAllMaps(int checkboxIndex)
        {
            var allCheckBoxes = new List<CheckBox>
            {
                checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4, checkBox_imar_5,
                checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9, checkBox_imar_10,
                checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15,

                checkBox_yuk_1, checkBox_yuk_2, checkBox_yuk_3, checkBox_yuk_4, checkBox_yuk_5,
                checkBox_yuk_6, checkBox_yuk_7, checkBox_yuk_8, checkBox_yuk_9, checkBox_yuk_10,
                checkBox_yuk_11, checkBox_yuk_12, checkBox_yuk_13, checkBox_yuk_14, checkBox_yuk_15,
            };

            var categoryCheckboxes = new Dictionary<string, CheckBox[]>
            {
                { "imar", checkBoxes_imar },
                { "yuk", checkBoxes_yuk }
            };

            CheckBox targetCheckbox = null;
            string targetCategory = null;
            foreach (var checkBox in allCheckBoxes)
            {
                if (checkBox.Tag != null && int.TryParse(checkBox.Tag.ToString(), out int tagIndex))
                {
                    if (tagIndex - 1 == checkboxIndex)
                    {
                        targetCheckbox = checkBox;
                        if (checkBox.Name.Contains("imar")) targetCategory = "imar";
                        else if (checkBox.Name.Contains("yuk")) targetCategory = "yuk";
                        break;
                    }
                }
            }

            if (targetCheckbox == null || targetCategory == null)
                return;

            foreach (var category in categoryCheckboxes.Keys)
            {
                if (categoryCheckboxes[category][checkboxIndex] != null)
                {
                    categoryCheckboxes[category][checkboxIndex].Checked = false;
                    categoryCheckboxes[category][checkboxIndex].Visible = false;
                    categoryCheckboxes[category][checkboxIndex].Refresh();
                }
            }

            foreach (var category in categoryCheckboxes.Keys)
            {
                UpdateCheckboxPositions(categoryCheckboxes[category], category);
            }
        }

        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------ GİRDİ MODÜLÜ DEĞİŞKENLER ---------------------------------------- //

        public static Dictionary<string, GirdiModülü> girdiModülleri = new Dictionary<string, GirdiModülü> {
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

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            // Handle file loading logic for the "Girdi" module
            if (slfStartYear == 0 || slfEndYear == 0)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if an item is selected in the ComboBox before accessing it
            if (veri_listesi_seçimi.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir veri tipi seçin.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit if no valid data type is selected
            }

            // Perform file selection based on the selected data type
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

            if (seçilenVeriTipi == "İmar Verileri")
            {
                using (imarFileSelectionPopup fileSelectionPopup = new imarFileSelectionPopup(dataGridView_girdi))
                {
                    if (fileSelectionPopup.ShowDialog() == DialogResult.OK)
                    {
                        string csvFilePath = fileSelectionPopup.CsvFilePath;
                        string kmlFilePath = fileSelectionPopup.KmlFilePath;

                        if (!File.Exists(csvFilePath) || !File.Exists(kmlFilePath))
                        {
                            MessageBox.Show("Geçerli dosyalar seçilmedi.", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            // Ensure girdiModülü is properly initialized
            if (!girdiModülleri.ContainsKey(seçilenVeriTipi))
            {
                MessageBox.Show("Geçersiz veri tipi seçildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Metodu sonlandır
            }

            girdiModülü = girdiModülleri[seçilenVeriTipi];
            if (girdiModülü == null)
            {
                MessageBox.Show($"{seçilenVeriTipi} için girdi modülü oluşturulamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Metodu sonlandır
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

            girdiModülü.slfStartYear = slfStartYear;
            girdiModülü.slfEndYear = slfEndYear;

            InitializeComboBoxes();

            bool skipPrerequisites = (selectedMethod == "ELF (Ekonometrik)");

            // Call VEERProcess with skipPrerequisites flag
            isImported = girdiModülü.VEERProcess(seçilenVeriTipi, skipPrerequisites);

            // Set the DataSource for dataGridView_girdi
            if (GirdiModülü.dataTablesByType.ContainsKey(seçilenVeriTipi))
            {
                dataGridView_girdi.DataSource = GirdiModülü.dataTablesByType[seçilenVeriTipi];
                // Apply formatting to dataGridView_girdi
                if (dataGridView_girdi.DataSource != null)
                {
                    girdiModülü.ApplyDataGridViewFormatting(GirdiModülü.dataTablesByType[seçilenVeriTipi], dataGridView_girdi);
                }
            }
            else
            {
                // Optionally, set DataSource to null or an empty DataTable to clear the grid
                dataGridView_girdi.DataSource = null;
            }

            dataGridView_girdi.Refresh();

            isİmportedModule(isImported, seçilenVeriTipi);

            if (isImported)
            {
                modulescheck.Add(seçilenVeriTipi);
                veri_listesi_seçimi.Refresh();
                Console.WriteLine(modulescheck.Count);

                // Update tab_senaryo accessibility after import
                UpdateTabSenaryoAccessibility();

            }
        }

        private void UpdateTabSenaryoAccessibility()
        {
            string requiredDataType = "Ekonometrik Yük Tahmini Verileri";
            tab_senaryo.Enabled = modulescheck.Contains(requiredDataType);
        }

        private async void OpenModuleButton_Click(object sender, EventArgs e)
        {
            // Disable the button initially
            OpenModuleButton.Enabled = false;

            string filePath = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                (string)ana_menu_form_objesi.config.İl,
                (string)ana_menu_form_objesi.config.İlçe,
                (string)ana_menu_form_objesi.config.ELF.INPUT_FILE);

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

            // Check if the selected data type is "Ekonometrik Yük Tahmini Verileri"
            if (seçilenVeriTipi == "Ekonometrik Yük Tahmini Verileri")
            {
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

                        // Ensure there are at least 6 worksheets
                        int totalSheets = package.Workbook.Worksheets.Count;
                        for (int i = 1; i <= 5; i++)
                        {
                            if (i < totalSheets)
                            {
                                var worksheet = package.Workbook.Worksheets[i];
                                DataTable dt = _excelService.LoadWorksheetIntoDataTable(worksheet);

                                Invoke(new Action(() =>
                                {
                                    var dataGrids = new[] { ELFMinSenaryoTable, ELFLowSenaryoTable, ELFBaseSenaryoTable, ELFHighSenaryoTable, ELFMaxSenaryoTable };
                                    dataGrids[i - 1].DataSource = dt;
                                }));
                            }
                            else
                            {
                                // If there are fewer than 6 sheets, show a message or handle as needed
                                MessageBox.Show("Eksik sayfalar bulundu. Lütfen dosyayı kontrol edin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                    }
                });
            }

            // Based on the selected data type, switch to the corresponding tab
            if (seçilenVeriTipi == "Ekonometrik Yük Tahmini Verileri")
            {
                Modül_Tabları.SelectedTab = tab_senaryo;
            }
            else if (seçilenVeriTipi == "EA Şarj Verileri")
            {
                Modül_Tabları.SelectedTab = tab_ea;
            }
            else if (seçilenVeriTipi == "DEK Verileri")
            {
                Modül_Tabları.SelectedTab = tab_dek;
            }

            // After loading the data, enable the button
            OpenModuleButton.Enabled = true;
        }

        private void veri_listesi_seçimi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (veri_listesi_seçimi.SelectedItem != null)
            {
                string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

                // dataTablesByType'ta bu veri var mı kontrol et
                if (GirdiModülü.dataTablesByType.ContainsKey(seçilenVeriTipi))
                {
                    // GirdiModülü'nü güncelle
                    if (girdiModülleri.ContainsKey(seçilenVeriTipi))
                    {
                        girdiModülleri[seçilenVeriTipi].importedDataTable = GirdiModülü.dataTablesByType[seçilenVeriTipi];
                    }

                    // DataGridView'ı güncelle
                    dataGridView_girdi.DataSource = GirdiModülü.dataTablesByType[seçilenVeriTipi];
                    dataGridView_girdi.Refresh();

                }
                else
                {
                    dataGridView_girdi.DataSource = null;
                }
            }
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


        private void ResetYearSelectionProcessGirdiModulu()
        {
            slfStartYear = slfEndYear = 0;
            int currentYear = DateTime.Now.Year;
            int lastYear = currentYear - 1;
            int lastYear2 = currentYear - 2;
            startYearComboBox.SelectedIndex = -1;
            startYearComboBox.Text = "Yıl seçiniz";
            endYearComboBox.SelectedIndex = -1;
            endYearComboBox.Text = "Yıl seçiniz";
            yearApproveButton.Text = "Onayla";

            //// Clear any existing items in the ComboBox
            startYearComboBox.Items.Clear();

            // Add the years to the ComboBox
            startYearComboBox.Items.Add(lastYear2);
            startYearComboBox.Items.Add(lastYear);
            startYearComboBox.Items.Add(currentYear);

            // Disable the endYearComboBox initially
            startYearComboBox.Enabled = true;
            endYearComboBox.Enabled = false;
            yearApproveButton.Enabled = false;
            // veri_listesi_seçimi.Enabled = false;
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
                // Unsubscribe from the FormClosing event only if the user clicks 'Yes'
                this.FormClosing -= ModülFormu_FormClosing;
                this.Hide(); // Hide the current form (ModülFormu)
            }
            // If the user clicks 'No', do nothing and stay on the current form
        }

        private void yearApproveButton_Click(object sender, EventArgs e)
        {
            ana_menu_form_objesi.config.ELF.ufuk_yılı = (int)endYearComboBox.SelectedItem - (int)startYearComboBox.SelectedItem;
            methodFormObjesi.SaveConfigToFile();
            
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
                dataGridView_girdi.DataSource = null;
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


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------ DEK MODÜLÜ DEĞİŞKENLER ------------------------------------------ //


        private DataTable veriMonteCarlo;

        private Dictionary<string, PointLatLng> cityCoordinates = new Dictionary<string, PointLatLng>
        {
            { "İzmir", new PointLatLng(38.4192, 27.1287) }, // Example coordinates for İzmir
            { "Eskişehir", new PointLatLng(39.7768, 30.5206) }, // Example coordinates for Eskişehir
            // Add more cities and their coordinates as needed
        };


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------ EA MODÜLÜ DEĞİŞKENLER ------------------------------------------- //

        private readonly Dictionary<string, List<string>> cityDistricts = new Dictionary<string, List<string>>
        {
            { "İzmir", new List<string> {/* "Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", */ "Çiğli", /*"Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karaburun", */"Karşıyaka",/* "Kemalpaşa", "Kınık", "Kiraz", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı"*/ } },
            { "Eskişehir", new List<string> { /*"Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "İnönü", "Mahmudiye", "Mihalgazi", "Mihalıççık", "Odunpazarı", "Sarıcakaya", "Seyitgazi", "Sivrihisar", */ "Tepebaşı" } }
        };
                private readonly Dictionary<string, string> districtIdMap = new Dictionary<string, string>
        {
            { "Çiğli", "1" },
            { "Karşıyaka", "2" },
            { "Tepebaşı", "1" }
        };


        private string SelectedSpeed = "";
        public bool isAddingChargingStation = false; // Sadece şarj istasyonu eklenirken true olacak.
        private bool isAddingDekPoint = false; // Sadece dek noktası eklenirken  true olacak.
        private int _selectedYear = -1;
        private string _selectedCity = null;
        private string _selectedDistrict;


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //


        // Initialize all form components (called in the constructors)
        private void InitializeComboBoxes()
        {
            // Yıl aralığını ComboBox1'e ekleyin


            Console.WriteLine("secilen_ilce_dizin: " + Path.Combine(PathService.BaseDirectory, PathService.FullPath));
            var yearList = new List<int>();
            for (int year = slfStartYear; year <= slfEndYear; year++)
            {
                yearList.Add(year);
            }
            comboBox_ea_yıl_secimi.DataSource = yearList; // Yıl seçimi için ComboBox1
            comboBox_DEK_Yıl.DataSource = yearList; // DEK yılı seçimi için ComboBox3
                                                    // Şehir isimlerini ComboBox2'ye ekleyin
            comboBox_DEK_il.Items.Clear(); // dek
            comboBox_ea_il_secimi.Items.Clear();   // ea 
            comboBox_ea_il_secimi.Items.Add("İzmir");
            comboBox_ea_il_secimi.Items.Add("Eskişehir");
            comboBox_DEK_il.Items.Add("İzmir");
            comboBox_DEK_il.Items.Add("Eskişehir");

        }

        public void isİmportedModule(bool isImported, string seçilenVeriTipi)
        {
            girdiModülü = girdiModülleri[seçilenVeriTipi];
            if (!GirdiModülü.dataTablesByType.ContainsKey(seçilenVeriTipi))
            {
                // Create a new DataTable and add it to the dictionary
                GirdiModülü.dataTablesByType[seçilenVeriTipi] = new DataTable();
            }
            //girdiModülü = girdiModülleri[seçilenVeriTipi];
            var importedDataTable = GirdiModülü.dataTablesByType[seçilenVeriTipi];


            if (!isImported)
            {
                return;
            }

            if (!GirdiModülü.dataTablesByType.ContainsKey(seçilenVeriTipi))
            {
                return;
            }


            // DataGridView temizleme ve bağlama
            dataGridView_girdi.DataSource = importedDataTable;

            // Görünürlük kontrolleri
            EnsureVisibility(dataGridView_girdi);

            // UI güncelleme
            dataGridView_girdi.Invoke((MethodInvoker)delegate
            {
                dataGridView_girdi.Refresh();
                dataGridView_girdi.BringToFront();
            });

        }

        private void EnsureVisibility(Control control)
        {
            // Parent kontrolü görünür değilse, görünür hale getir
            if (control.Parent != null && !control.Parent.Visible)
            {
                Console.WriteLine($"{control.Name} Parent kontrolü gizli. Görünür hale getiriliyor...");
                control.Parent.Visible = true;
            }

            // DataGridView görünür değilse, görünür hale getir
            if (!control.Visible)
            {
                Console.WriteLine($"{control.Name} gizli. Görünür hale getiriliyor...");
                control.Visible = true;
            }
        }


        private void ModülFormu_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Değişiklikler var mı kontrol et
            bool hasChanges = false;

            // Son kaydedilen modül listesi ile mevcut modül listesini karşılaştır
            if (PathService.CurrentMode == PathService.WorkingMode.Project)
            {
                // Proje zaten açık, değişiklik var mı kontrol et
                string statePath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    PathService.CurrentWorkingFolder,
                    "project_state.json");

                if (File.Exists(statePath))
                {
                    try
                    {
                        string json = File.ReadAllText(statePath);
                        var projectState = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                        if (projectState.TryGetValue("CompletedModules", out object modulesObj))
                        {
                            string modulesJson = modulesObj.ToString();
                            List<string> savedModules = System.Text.Json.JsonSerializer.Deserialize<List<string>>(modulesJson);

                            // Mevcut modüller
                            var currentModules = GirdiModülü.dataTablesByType.Keys.ToList();

                            // Değişiklik var mı?
                            if (currentModules.Count != savedModules.Count ||
                                !currentModules.All(m => savedModules.Contains(m)))
                            {
                                hasChanges = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Hata olduğunda değişiklikler olduğunu varsay
                        hasChanges = true;
                    }
                }
                else if (GirdiModülü.dataTablesByType.Count > 0)
                {
                    // Hiç kayıt yoksa ama veriler varsa değişiklikler var demektir
                    hasChanges = true;
                }
            }
            else if (PathService.CurrentMode == PathService.WorkingMode.Temporary && GirdiModülü.dataTablesByType.Count > 0)
            {
                // Geçici moddayız ve veri var, değişiklik var demektir
                hasChanges = true;
            }

            // Değişiklikler varsa kaydetme seçeneği sun
            if (hasChanges)
            {
                DialogResult result = MessageBox.Show(
                    "Kaydedilmemiş değişiklikler var. Çıkmadan önce kaydetmek ister misiniz?",
                    "Değişiklikler Kaydedilsin mi?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Çıkışı iptal et
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    // Projeyi kaydet
                    bool saveSuccess = false;

                    if (PathService.CurrentMode == PathService.WorkingMode.Project)
                    {
                        // Mevcut projeyi güncelle
                        saveSuccess = UpdateExistingProject();
                    }
                    else
                    {
                        //// Geçici moddayız, yeni proje adı sor
                        string projectName = ProjectFolderPicker.ShowNewProjectDialog(
                            Path.Combine(PathService.BaseDirectory, PathService.FullPath));

                        if (!string.IsNullOrEmpty(projectName))
                        {
                            // Projeyi oluştur
                            saveSuccess = CreateAndSaveProject(projectName);
                        }
                        else
                        {
                            // Kullanıcı iptal etti veya geçersiz isim
                            DialogResult continueResult = MessageBox.Show(
                                "Proje kaydedilmedi. Yine de çıkmak istiyor musunuz?",
                                "Kaydetme İptal Edildi",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (continueResult == DialogResult.No)
                            {
                                e.Cancel = true; // Çıkışı iptal et
                                return;
                            }
                        }
                    }

                    if (!saveSuccess)
                    {
                        // Kaydetme başarısız olduysa tekrar sor
                        DialogResult retryResult = MessageBox.Show(
                            "Proje kaydedilemedi. Yine de çıkmak istiyor musunuz?",
                            "Kaydetme Başarısız",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (retryResult == DialogResult.No)
                        {
                            e.Cancel = true; // Çıkışı iptal et
                            return;
                        }
                    }
                }
            }

            // Çıkış işlemine devam et
            try
            {
                // UI durumunu temizle - yeni eklenen metot
                ClearUserInterfaceState();

                // Global veri yapılarını temizle
                ClearGlobalData();

                // Geçici klasörleri temizle - tümünü temizle
                CleanupTemporaryFolders(true);

                // Veritabanı bağlantısını kapat
                try
                {
                    DatabaseManager.GetInstance("").CloseConnection();
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"Veritabanı kapatılırken hata: {dbEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kapanış sırasında hata: " + ex.Message);
            }
        }


        // ModülFormu sınıfına eklenecek yeni metot
        private void ClearUserInterfaceState()
        {
            try
            {
                // Panel, ComboBox ve diğer kontrolleri başlangıç durumuna geri getir

                // ComboBox'ları sıfırla
                if (veri_listesi_seçimi != null)
                {
                    veri_listesi_seçimi.SelectedIndex = -1;
                }

                if (comboBox_ea_il_secimi != null)
                {
                    comboBox_ea_il_secimi.SelectedIndex = -1;
                }

                if (comboBox_ea_yıl_secimi != null)
                {
                    comboBox_ea_yıl_secimi.SelectedIndex = -1;
                }

                if (comboBox_DEK_il != null)
                {
                    comboBox_DEK_il.SelectedIndex = -1;
                }

                if (comboBox_DEK_Yıl != null)
                {
                    comboBox_DEK_Yıl.SelectedIndex = -1;
                }

                // DataGridView'ları temizle
                if (dataGridView_girdi != null)
                {
                    dataGridView_girdi.DataSource = null;
                }

                // Tüm harita overlaylerini temizle
                ClearAllMapOverlays();

                // Haritaların renklerini sıfırla
                ResetMapColors();

                // CheckBox'ları sıfırla
                ResetAllCheckBoxes();

                // Form başlığını varsayılana çevir
                this.Text = "SLF Yazılımı";

                Console.WriteLine("UI durumu temizlendi");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UI durumu temizlenirken hata: {ex.Message}");
                // Hatayı yut ve devam et
            }
        }

        // Haritaların renklerini sıfırlayan yardımcı metot
        private void ResetMapColors()
        {
            try
            {

                // Haritaların renklerini varsayılana çevir
                if (gMapControl_EA != null)
                {
                    gMapControl_EA.MapProvider = GMapProviders.GoogleSatelliteMap;
                }

                if (gMapControl_DEK != null)
                {
                    gMapControl_DEK.MapProvider = GMapProviders.GoogleSatelliteMap;
                }

                if (gMapControl_yuk != null)
                {
                    gMapControl_yuk.MapProvider = GMapProviders.GoogleSatelliteMap;
                }

                if (gMapControl_imar != null)
                {
                    gMapControl_imar.MapProvider = GMapProviders.GoogleSatelliteMap;
                }


                if (gMapControl_optimalDTR != null)
                {
                    gMapControl_optimalDTR.MapProvider = GMapProviders.GoogleSatelliteMap;
                }

                Console.WriteLine("Harita renkleri sıfırlandı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Harita renkleri sıfırlanırken hata: {ex.Message}");
            }
        }


        // Tüm CheckBox'ları sıfırlayan yardımcı metot
        private void ResetAllCheckBoxes()
        {
            try
            {
                checkBoxes_imar = new System.Windows.Forms.CheckBox[] { checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4,
                    checkBox_imar_5, checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9,
                    checkBox_imar_10, checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15 };

                checkBoxes_yuk = new System.Windows.Forms.CheckBox[] { checkBox_yuk_1, checkBox_yuk_2, checkBox_yuk_3, checkBox_yuk_4,
                    checkBox_yuk_5, checkBox_yuk_6, checkBox_yuk_7, checkBox_yuk_8, checkBox_yuk_9,
                    checkBox_yuk_10, checkBox_yuk_11, checkBox_yuk_12, checkBox_yuk_13, checkBox_yuk_14, checkBox_yuk_15 };


                // EA modülü checkboxlarını sıfırla
                if (checkBox_AC_Home != null) checkBox_AC_Home.Checked = false;
                if (checkBox_AC_Public != null) checkBox_AC_Public.Checked = false;
                if (checkBox_AC_Work != null) checkBox_AC_Work.Checked = false;
                if (checkBox_DC_Fast != null) checkBox_DC_Fast.Checked = false;

                // EA modülü radio buttonlarını sıfırla
                if (EaSimMaxBtn != null) EaSimMaxBtn.Checked = false;
                if (EaSimMinBtn != null) EaSimMinBtn.Checked = false;
                if (EaSimDefBtn != null) EaSimDefBtn.Checked = false;

                // DEK modülü radio buttonlarını sıfırla
                if (dekSimMaxBtn != null) dekSimMaxBtn.Checked = false;
                if (dekSimMinBtn != null) dekSimMinBtn.Checked = false;
                if (dekSimDefBtn != null) dekSimDefBtn.Checked = false;

                // YGA, İmar ve Stokastik checkboxlarını sıfırla
                foreach (var cb in checkBoxes_yuk)
                {
                    if (cb != null)
                    {
                        cb.Checked = false;
                        cb.Visible = false;
                        cb.Text = "";
                        cb.ForeColor = SystemColors.ControlText; // Rengi varsayılana çevir
                    }
                }

                foreach (var cb in checkBoxes_imar)
                {
                    if (cb != null)
                    {
                        cb.Checked = false;
                        cb.Visible = false;
                        cb.Text = "";
                        cb.ForeColor = SystemColors.ControlText; // Rengi varsayılana çevir
                    }
                }

                // Last clicked checkbox'ı sıfırla
                lastClickedCheckbox = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CheckBox'lar sıfırlanırken hata: {ex.Message}");
            }
        }





        // Tüm harita overlay'lerini temizleyen yardımcı metot
        private void ClearAllMapOverlays()
        {
            try
            {
                // Tüm GMapControl'lerdeki overlay'leri temizle
                if (gMapControl_EA != null && gMapControl_EA.Overlays != null)
                {
                    gMapControl_EA.Overlays.Clear();
                    gMapControl_EA.Refresh();
                }

                if (gMapControl_DEK != null && gMapControl_DEK.Overlays != null)
                {
                    gMapControl_DEK.Overlays.Clear();
                    gMapControl_DEK.Refresh();
                }


                if (gMapControl_yuk != null && gMapControl_yuk.Overlays != null)
                {
                    gMapControl_yuk.Overlays.Clear();
                    gMapControl_yuk.Refresh();
                }

                if (gMapControl_imar != null && gMapControl_imar.Overlays != null)
                {
                    gMapControl_imar.Overlays.Clear();
                    gMapControl_imar.Refresh();
                }


                if (gMapControl_optimalDTR != null && gMapControl_optimalDTR.Overlays != null)
                {
                    gMapControl_optimalDTR.Overlays.Clear();
                    gMapControl_optimalDTR.Refresh();
                }

                // CBS sınıfındaki overlay dizisini de sıfırla
                if (cbs != null)
                {
                    for (int i = 0; i < cbs.tüm_katmanlar_array_imar.Length; i++)
                    {
                        cbs.tüm_katmanlar_array_imar[i] = null;
                        cbs.tüm_katmanlar_array_names[i] = null;

                        if (cbs.tüm_katmanlar_datatable[i] != null)
                        {
                            cbs.tüm_katmanlar_datatable[i].Dispose();
                            cbs.tüm_katmanlar_datatable[i] = null;
                        }

                        if (cbs.shapeFileArray_MapWinGIS[i] != null)
                        {
                            cbs.shapeFileArray_MapWinGIS[i].Close();
                            cbs.shapeFileArray_MapWinGIS[i] = null;
                        }
                    }
                }

                // Poligon noktalarını da temizle
                polygonPoints_imar?.Clear();
                polygonPoints_yuk?.Clear();
                polygonPoints_ea?.Clear();
                polygonPoints_DEK?.Clear();

                // Ruler noktalarını da temizle
                rulerPoints_imar?.Clear();
                rulerPoints_yuk?.Clear();
                rulerPoints_ea?.Clear();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Harita overlay'leri temizlenirken hata: {ex.Message}");
            }
        }


        private void ClearGlobalData()
        {
            try
            {
                // GirdiModülü veri tablolarını temizle
                GirdiModülü.dataTablesByType.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Global veri temizleme hatası: {ex.Message}");
            }
        }

        // Yeni proje oluştur ve verileri kaydet
        private bool CreateAndSaveProject(string projectName)
        {
            try
            {
                Console.WriteLine($"Proje oluşturma başladı: {projectName}");

                // Mevcut geçici klasörün yolunu al (kaydetmeden önce)
                string tempFolderPath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    PathService.CurrentWorkingFolder);

                // Proje klasör adını oluştur
                string projectFolderName = $"proje_{projectName}";
                string projectPath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    projectFolderName);

                // Eğer proje klasörü zaten varsa, kullanıcıya sor
                if (Directory.Exists(projectPath))
                {
                    var result = MessageBox.Show(
                        $"'{projectName}' adında bir proje zaten var. Üzerine yazmak istiyor musunuz?",
                        "Proje Zaten Var",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return false;

                    // Var olan klasörü tamamen silmek yerine, project_state.json'ı güncelle
                    try
                    {
                        // project_state.json dışındaki dosyaları güncelle
                        // Eğer aynı isimde bir dosya varsa, üzerine yazacak
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Var olan proje dosyası güncellenirken hata: {ex.Message}");
                        // Devam et
                    }
                }
                else
                {
                    // Proje klasörü yoksa oluştur
                    Directory.CreateDirectory(projectPath);
                }

                // Önemli: Klasör yapısını koru
                // Eğer geçici klasörde mevcut bir yapı varsa, onu doğrudan kopyala
                if (Directory.Exists(tempFolderPath))
                {
                    try
                    {
                        // Geçici klasördeki klasör yapısını kontrol et
                        bool hasGirdiler = Directory.Exists(Path.Combine(tempFolderPath, "Girdiler"));
                        bool hasSonuclar = Directory.Exists(Path.Combine(tempFolderPath, "Sonuçlar"));
                        bool hasImarAnalizi = Directory.Exists(Path.Combine(tempFolderPath, "imar_analizi_sonuclari"));

                        if (hasGirdiler || hasSonuclar || hasImarAnalizi)
                        {
                            // Mevcut klasör yapısını doğrudan kopyala
                            CopyDirectoryStructure(tempFolderPath, projectPath);
                            Console.WriteLine("Mevcut klasör yapısı korundu ve kopyalandı");
                        }
                        else
                        {
                            // Standart klasör yapısını oluştur
                            CreateStandardFolderStructure(projectPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Klasör yapısı kopyalanırken hata: {ex.Message}");
                        // Standart klasör yapısını oluştur
                        CreateStandardFolderStructure(projectPath);
                    }
                }
                else
                {
                    // Standart klasör yapısını oluştur
                    CreateStandardFolderStructure(projectPath);
                }

                // PathService'i güncelle
                PathService.OpenProject(projectName);

                // Proje durumunu kaydet
                SaveProjectState(projectPath);

                // Modül verilerini kaydet
                SaveAllModuleDataToCSV(projectPath);

                Console.WriteLine($"Proje başarıyla oluşturuldu ve kaydedildi: {projectName}");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje oluşturulurken hata oluştu: {ex.Message}",
                                "Oluşturma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Proje oluşturma hatası: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // Klasör yapısını doğrudan kopyala - sadece klasör yapısını korur
        private void CopyDirectoryStructure(string sourceDir, string targetDir)
        {
            try
            {
                // Target dizinini oluştur (yoksa)
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // Kaynak klasördeki tüm alt klasörleri kopyala
                foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(sourceSubDir);
                    string targetSubDir = Path.Combine(targetDir, dirInfo.Name);

                    // Alt klasörü oluştur
                    if (!Directory.Exists(targetSubDir))
                    {
                        Directory.CreateDirectory(targetSubDir);
                    }

                    // Rekürsif olarak alt klasörleri kopyala
                    CopyDirectoryStructure(sourceSubDir, targetSubDir);
                }

                // Dosyaları kopyalamak istemiyorsak bu kısmı yorum yapabiliriz
                // Burada sadece klasör yapısını koruyoruz, dosyaları SaveAllModuleDataToCSV ile kaydedeceğiz
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Klasör yapısı kopyalanırken hata: {ex.Message}");
                throw; // Üst metoda hatayı ilet
            }
        }

        // Standart klasör yapısını oluştur
        private void CreateStandardFolderStructure(string projectPath)
        {
            // Standart klasörleri oluştur
            string girdilerPath = Path.Combine(projectPath, "Girdiler");
            string sonuclarPath = Path.Combine(projectPath, "Sonuçlar");
            string imarAnalysiPath = Path.Combine(projectPath, "imar_analizi_sonuclari");

            // Klasörleri oluştur (yoksa)
            if (!Directory.Exists(girdilerPath))
                Directory.CreateDirectory(girdilerPath);

            if (!Directory.Exists(sonuclarPath))
                Directory.CreateDirectory(sonuclarPath);

            if (!Directory.Exists(imarAnalysiPath))
                Directory.CreateDirectory(imarAnalysiPath);

            Console.WriteLine("Standart klasör yapısı oluşturuldu");
        }

        // Proje durumunu kaydetme metodu ProjectState.cs dosyasında benzeri var ancak burada kendi versiyonumuzu kullanıyoruz
        private void SaveProjectState(string projectPath)
        {
            try
            {
                var yearService = YearService.GetInstance();

                // Proje durumunu hazırla
                var projectState = new Dictionary<string, object>
                {
                    ["CompletedModules"] = GirdiModülü.dataTablesByType.Keys.ToList(),
                    ["SLFStartYear"] = yearService.slfStartYear,
                    ["SLFEndYear"] = yearService.slfEndYear,
                    ["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["CreatedBy"] = Environment.UserName
                };

                // JSON olarak kaydet
                string json = System.Text.Json.JsonSerializer.Serialize(projectState,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                string statePath = Path.Combine(projectPath, "project_state.json");
                File.WriteAllText(statePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Proje durumu kaydedilirken hata: {ex.Message}");
                throw;
            }
        }
        // Modül verilerini CSV olarak kaydet


        // Mevcut projeyi güncelleme metodu
        private bool UpdateExistingProject()
        {
            try
            {
                // Proje yolunu al
                string projectPath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    PathService.CurrentWorkingFolder);

                // Proje durumunu kaydet
                SaveProjectState(projectPath);

                // Modül verilerini kaydet
                SaveAllModuleDataToCSV(projectPath);

                // Proje adını al (proje_ önekini çıkar)
                string projectName = PathService.CurrentWorkingFolder.StartsWith("proje_")
                    ? PathService.CurrentWorkingFolder.Substring(6)
                    : PathService.CurrentWorkingFolder;

                Console.WriteLine($"Mevcut proje güncellendi: {projectName}");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje güncellenirken hata oluştu: {ex.Message}", "Güncelleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        // Geçici klasörü proje olarak kaydet
        private bool CleanupTemporaryFolders(bool cleanupAll = false)
        {
            try
            {
                string districtPath = Path.Combine(PathService.BaseDirectory, PathService.FullPath);

                if (!Directory.Exists(districtPath))
                    return false;

                Console.WriteLine($"Geçici klasörler aranıyor: {districtPath}");

                // "temp_" ile başlayan tüm klasörleri bul
                string[] tempFolders = Directory.GetDirectories(districtPath, "temp_*");
                Console.WriteLine($"Bulunan geçici klasör sayısı: {tempFolders.Length}");

                foreach (string folder in tempFolders)
                {
                    try
                    {
                        // Mevcut klasör aktif çalışma klasörü mü ve tümünü temizleme modu aktif değil mi?
                        if (!cleanupAll &&
                            PathService.CurrentMode == PathService.WorkingMode.Temporary &&
                            !string.IsNullOrEmpty(PathService.CurrentWorkingFolder) &&
                            folder.EndsWith(PathService.CurrentWorkingFolder))
                        {
                            Console.WriteLine($"Aktif çalışma klasörü atlanıyor: {folder}");
                            continue;
                        }

                        Console.WriteLine($"Geçici klasör siliniyor: {folder}");

                        // Tüm salt okunur özniteliklerini kaldır
                        RemoveReadOnlyAttributesRecursive(folder);

                        // GC.Collect çağrısıyla açık dosya tanıtıcılarını temizle
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        // Silmeyi dene, olmazsa zorla sil
                        try
                        {
                            Directory.Delete(folder, true);
                            Console.WriteLine($"Geçici klasör başarıyla silindi: {folder}");
                        }
                        catch (IOException)
                        {
                            // Dosya işlemi hatası - zorla silmeyi dene
                            ForceDeleteDirectory(folder);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Yetki hatası - zorla silmeyi dene
                            ForceDeleteDirectory(folder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Geçici klasör silinirken hata: {ex.Message}");
                        // Hata olsa bile devam et
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Geçici klasörleri temizlerken genel hata: {ex.Message}");
                return false;
            }
        }

        // Zorla silme işlemi - Command line kullanarak sil
        private void ForceDeleteDirectory(string path)
        {
            try
            {
                Console.WriteLine($"Zorla silme deneniyor: {path}");

                // Command kullanarak silme
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C rd /S /Q \"{path}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Klasör başarıyla zorla silindi: {path}");
                }
                else
                {
                    Console.WriteLine($"Zorla silme başarısız: {path}, Çıkış kodu: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Zorla silme sırasında hata: {ex.Message}");
            }
        }
        private void RemoveReadOnlyAttributesRecursive(string path)
        {
            try
            {
                // Tüm dosyalar için salt okunur özniteliğini kaldır
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fileInfo.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }

                // Alt klasörler için de aynı işlemi yap
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    RemoveReadOnlyAttributesRecursive(directory);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Salt okunur özniteliği kaldırılırken hata: {ex.Message}");
            }
        }

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

        // BURASI SONRADAN AÇILACAK, SIMDILIK BOYLE KALSIN.
        private async void Modül_Tabları_SelectedIndexChanged(object sender, EventArgs e)
        {

            foreach (var category in categoryTabPages.Keys)
            {
                if (pendingUpdates[category] && tabControlMain.SelectedTab == categoryTabPages[category])
                {
                    if (category == "imar")
                        UpdateCheckboxPositions(checkBoxes_imar, "imar");
                    else if (category == "yuk")
                        UpdateCheckboxPositions(checkBoxes_yuk, "yuk");

                    // Add "yuk" if applicable
                    pendingUpdates[category] = false;
                }
            }


            // Gerekli kontrolleri yapmak için seçilen sekmeyi ve modülleri kontrol et
            string selectedTabText = Modül_Tabları.SelectedTab.Text;

            // Modüllerin yüklü olup olmadığını kontrol et
            if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                if ((selectedTabText == "EA Şarj Modülü" || selectedTabText == "DEK Modülü" || selectedTabText == "Yük Haritası Modülü") && !GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri"))
                {
                    // Sekme geçişini tamamen iptal et
                    Console.WriteLine(GirdiModülü.dataTablesByType.Count);
                    MessageBox.Show("DTR verileri yüklenmeden bu sekmeye geçiş yapılamaz.");
                    Modül_Tabları.SelectedIndexChanged -= Modül_Tabları_SelectedIndexChanged;
                    Modül_Tabları.SelectedTab = tab_girdi;
                    Modül_Tabları.SelectedIndexChanged += Modül_Tabları_SelectedIndexChanged;
                    return;
                }
                /*else if (selectedTabText == "İmar Analizleri" && (!GirdiModülü.dataTablesByType.ContainsKey("İmar Planı")))
                {
                    // Sekme geçişini tamamen iptal et
                    MessageBox.Show("İmar planı verileri yüklenmeden bu sekmeye geçiş yapılamaz.");
                    Modül_Tabları.SelectedIndexChanged -= Modül_Tabları_SelectedIndexChanged;
                    Modül_Tabları.SelectedTab = tab_girdi;
                    Modül_Tabları.SelectedIndexChanged += Modül_Tabları_SelectedIndexChanged;
                    return;
                }*/
                else if (selectedTabText == "Optimal DTR Konumlandırma"
                            && (!GirdiModülü.dataTablesByType.ContainsKey("İmar Planı")
                            && !GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri")))
                {
                    // Sekme geçişini tamamen iptal et
                    MessageBox.Show("DTR verileri ve İmar planı yüklenmeden bu sekmeye geçiş yapılamaz.");
                    Modül_Tabları.SelectedIndexChanged -= Modül_Tabları_SelectedIndexChanged;
                    Modül_Tabları.SelectedTab = tab_girdi;
                    Modül_Tabları.SelectedIndexChanged += Modül_Tabları_SelectedIndexChanged;
                    return;
                }
            }

            // EA Şarj Modülü tabına tıklanmışsa
            if (selectedTabText == "EA Şarj Modülü")
            {
                if ((!GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri")))
                {
                    MessageBox.Show("Lütfen EA Sarj modülü verilerinizi yükleyin.");
                    Modül_Tabları.SelectedTab = tab_girdi;
                    return;
                }
                else
                {
                    InitializeComboBoxes();
                    await eaHaritayaVeriYukleAsync();
                }


            }
            else if (selectedTabText == "DEK Modülü")
            {
                Console.WriteLine("DEK Modülü");
                if (!GirdiModülü.dataTablesByType.ContainsKey("DEK Verileri"))
                {
                    MessageBox.Show("Lütfen DEK modülü verilerinizi yükleyin.");
                    Modül_Tabları.SelectedTab = tab_girdi;
                    return;
                }
                else
                {
                    await dekHaritayaVeriYukleAsync();
                }

            }
            else if (Modül_Tabları.SelectedTab == tab_yükHaritası)
            {

                // Find the index of the overlay in tüm_katmanlar_array_imar_names that contains "xxx"
                string searchText = "SONUCLAR_Load_Density.kml"; // The text to search for
                overlayIndex = Array.FindIndex(cbs.tüm_katmanlar_array_names,
                    name => name != null && name.Contains(searchText));

                // if the SONUCLAR_load_density.kml file exists
                if (overlayIndex != -1)
                {
                    legendPanel.Visible = true;

                    load_density_cnt++;

                    if (load_density_cnt == 1)
                    {
                        trackBar_Yıllar.Value = trackBar_Yıllar.Minimum + 1;
                        trackBar_Yıllar.Value = trackBar_Yıllar.Minimum;
                    }

                    legendPanel.PerformLayout(); // Force layout update

                }
                else
                {
                    legendPanel.Visible = false;
                }

            } 
        }

        public void ProjeEkleButton_Click(object sender, EventArgs e)
        {
            try
            {
                // İl/ilçe bilgilerini kontrol et
                if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
                {
                    MessageBox.Show("Lütfen önce il ve ilçe seçimi yapın.",
                        "İl/İlçe Seçilmedi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Proje klasörünün ana dizini
                string projectsBaseDir = Path.Combine(PathService.BaseDirectory, PathService.FullPath);

                // Burada sadece mevcut projeleri listeleme ve seçme işlemi
                using (var folderBrowser = new ProjectFolderPicker(projectsBaseDir))
                {
                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string selectedPath = folderBrowser.SelectedPath;
                            string projectName = folderBrowser.SelectedProjectName;

                            // Geçici klasörden yüklü veri kontrolü
                            if (PathService.CurrentMode == PathService.WorkingMode.Temporary && GirdiModülü.dataTablesByType.Count > 0)
                            {
                                var result = MessageBox.Show(
                                    "Geçici çalışma klasöründeki veriler kaydedilmemiş. Proje açmak geçici verilerin kaybına neden olacaktır. Devam etmek istiyor musunuz?",
                                    "Veri Kaybı Uyarısı",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);
                                if (result != DialogResult.Yes)
                                    return;
                            }

                            // Projeyi aç
                            PathService.OpenProject(projectName);

                            // Varsayılan geçici klasörü temizle
                            CleanupDefaultTempFolder();

                            // Proje durumunu yükle
                            LoadProjectState();

                            // UI'ı güncelle
                            UpdateUIForLoadedProject();

                            // Form başlığını güncelle
                            this.Text = $"SLF Yazılımı - {PathService.SelectedCity}/{PathService.SelectedDistrict} - Proje: {projectName}";

                            MessageBox.Show($"Proje '{projectName}' başarıyla açıldı.",
                                "Proje Açıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Son çalışılan projeyi ayarlar dosyasına kaydet
                            try
                            {

                                Console.WriteLine($"Son proje bilgileri kaydedildi: {projectName}");
                            }
                            catch (Exception settingsEx)
                            {
                                Console.WriteLine($"Ayarlar kaydedilirken hata: {settingsEx.Message}");
                                // Ayarlar kaydedilemediğinde ana işlevi etkilememesi için hatayı yut
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Proje açılırken hata oluştu: {ex.Message}",
                                "Proje Açma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje işlemi sırasında hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Proje işlem hatası: {ex}");
            }
        }

        // Yeni metot: Varsayılan geçici klasörü temizle
        private bool CleanupDefaultTempFolder()
        {
            try
            {
                string districtPath = Path.Combine(PathService.BaseDirectory, PathService.FullPath);

                if (Directory.Exists(districtPath))
                {
                    // "temp_" ile başlayan tüm klasörleri bul
                    string[] tempFolders = Directory.GetDirectories(districtPath, "temp_*");

                    foreach (string folder in tempFolders)
                    {
                        try
                        {
                            // Salt okunur özniteliklerini kaldır
                            RemoveReadOnlyAttributesRecursive(folder);

                            // Klasörü sil
                            Directory.Delete(folder, true);
                            Console.WriteLine($"Geçici klasör silindi: {folder}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Geçici klasör silinirken hata: {ex.Message}");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Geçici klasörleri temizlerken hata: {ex.Message}");
                return false;
            }
        }

        private void LoadProjectState()
        {
            try
            {
                string statePath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    PathService.CurrentWorkingFolder,
                    "project_state.json");

                Console.WriteLine($"Proje durum dosyası: {statePath}");

                if (File.Exists(statePath))
                {
                    string json = File.ReadAllText(statePath);
                    Console.WriteLine($"Okunan JSON: {json}");

                    // JSON'ı deserialize et
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                        PropertyNameCaseInsensitive = true
                    };

                    var projectState = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);

                    // JSON deserialize edildi mi kontrol et
                    if (projectState == null)
                    {
                        Console.WriteLine("HATA: JSON deserialize edilemedi!");
                        return;
                    }

                    // JSON içeriğini yazdır
                    Console.WriteLine("JSON içeriği:");
                    foreach (var key in projectState.Keys)
                    {
                        Console.WriteLine($"- {key}: {projectState[key]}");
                    }

                    // Yıl bilgilerini yükle
                    if (projectState.TryGetValue("SLFStartYear", out object startYearObj) &&
                        projectState.TryGetValue("SLFEndYear", out object endYearObj))
                    {
                        try
                        {
                            // JSON.NET ile JsonElement tipindeki değeri int'e çevir
                            int startYear = Convert.ToInt32(startYearObj.ToString());
                            int endYear = Convert.ToInt32(endYearObj.ToString());

                            Console.WriteLine($"Yüklenecek yıllar: Başlangıç {startYear}, Bitiş {endYear}");

                            // YearService'i güncelle
                            var yearService = YearService.GetInstance();
                            yearService.SetYears(startYear, endYear);

                            // ModülFormu değişkenlerini güncelle
                            this.slfStartYear = startYear;
                            this.slfEndYear = endYear;

                            Console.WriteLine($"Yıllar başarıyla yüklendi ve setlendi");

                            // ComboBox'ları güncelle
                            UpdateYearComboBoxes();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Yılları setlerken hata: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("UYARI: JSON dosyasında yıl bilgileri bulunamadı!");
                    }

                    // Tamamlanan modülleri yükle
                    if (projectState.TryGetValue("CompletedModules", out object modulesObj))
                    {
                        try
                        {
                            // Modül listesini al
                            var modulesElement = (System.Text.Json.JsonElement)modulesObj;
                            List<string> completedModules = new List<string>();

                            // JsonElement dizisini List<string>'e dönüştür
                            if (modulesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                foreach (var element in modulesElement.EnumerateArray())
                                {
                                    if (element.ValueKind == System.Text.Json.JsonValueKind.String)
                                    {
                                        completedModules.Add(element.GetString());
                                    }
                                }
                            }

                            // Konsola kaydedilmiş modülleri yazdır
                            Console.WriteLine($"Kaydedilmiş {completedModules.Count} modül bulundu:");
                            foreach (var module in completedModules)
                            {
                                Console.WriteLine($"- {module}");
                            }

                            // Modül verilerini yükle
                            foreach (string module in completedModules)
                            {
                                LoadModuleData(module);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Modül listesi yüklenirken hata: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("UYARI: JSON dosyasında modül listesi bulunamadı!");
                    }
                }
                else
                {
                    Console.WriteLine($"UYARI: Proje durum dosyası bulunamadı: {statePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Proje durumu yüklenirken hata: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // ComboBox'ları yüklenen yıllara göre güncelle
        private void UpdateYearComboBoxes()
        {
            try
            {
                // ComboBox'ları bul ve yılları set et (buradaki isimler projenizdeki gerçek adlara göre değiştirilmeli)
                if (startYearComboBox != null && endYearComboBox != null)
                {
                    // YearService'ten yılları al
                    var yearService = YearService.GetInstance();
                    int startYear = yearService.slfStartYear;
                    int endYear = yearService.slfEndYear;

                    // ComboBox'lara yılları seç
                    for (int i = 0; i < startYearComboBox.Items.Count; i++)
                    {
                        if (startYearComboBox.Items[i].ToString() == startYear.ToString())
                        {
                            startYearComboBox.SelectedIndex = i;
                            break;
                        }
                    }

                    for (int i = 0; i < endYearComboBox.Items.Count; i++)
                    {
                        if (endYearComboBox.Items[i].ToString() == endYear.ToString())
                        {
                            endYearComboBox.SelectedIndex = i;
                            break;
                        }
                    }

                    // "Onayla" butonunun metnini güncelle (eğer bu butonu kullanıyorsanız)
                    if (yearApproveButton != null)
                    {
                        yearApproveButton.Text = "Sıfırla"; // Yıllar setlenmiş durumda
                    }

                    Console.WriteLine($"ComboBox'lar yüklenen yıllara göre güncellendi");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ComboBox'lar güncellenirken hata: {ex.Message}");
            }
        }

        /// Modül verilerini yükler
        private void LoadModuleData(string moduleName)
        {
            try
            {
                Console.WriteLine($"Modül verisi yükleniyor: {moduleName}");

                // Modül için girdiler klasörünü bul
                string modulePath = PathService.GetGirdilerPathForDataType(moduleName);
                Console.WriteLine($"Modül yolu: {modulePath}");

                if (!Directory.Exists(modulePath))
                {
                    Console.WriteLine($"Modül klasörü bulunamadı: {modulePath}");
                    return;
                }

                // Klasördeki tüm CSV dosyalarını göster
                var directory = new DirectoryInfo(modulePath);
                var csvFiles = directory.GetFiles("*.csv");
                Console.WriteLine($"{csvFiles.Length} adet CSV dosyası bulundu.");

                foreach (var file in csvFiles)
                {
                    Console.WriteLine($"- {file.Name} ({file.LastWriteTime})");
                }

                // Klasördeki en son dosyayı bul
                var latestFile = csvFiles.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

                if (latestFile != null)
                {
                    Console.WriteLine($"En son dosya: {latestFile.Name}");

                    // CSV'yi yükle
                    var csvHandler = new CsvHandler();
                    DataTable moduleData = csvHandler.ImportCsvFile(latestFile.FullName);

                    if (moduleData != null)
                    {
                        Console.WriteLine($"CSV başarıyla yüklendi: {moduleData.Rows.Count} satır, {moduleData.Columns.Count} sütun");

                        // GirdiModülü.dataTablesByType'a ekle
                        if (moduleData.Rows.Count > 0)
                        {
                            GirdiModülü.dataTablesByType[moduleName] = moduleData;

                            // GirdiModülleri sözlüğünü güncelle
                            if (girdiModülleri.ContainsKey(moduleName))
                            {
                                girdiModülleri[moduleName].importedDataTable = moduleData;
                                Console.WriteLine($"GirdiModülleri sözlüğü güncellendi: {moduleName}");
                            }
                            else
                            {
                                Console.WriteLine($"UYARI: {moduleName} girdiModülleri sözlüğünde bulunamadı!");
                            }

                            Console.WriteLine($"Modül verisi yüklendi: {moduleName}, Satır sayısı: {moduleData.Rows.Count}");
                        }
                        else
                        {
                            Console.WriteLine($"UYARI: {moduleName} için CSV dosyası boş!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"HATA: {latestFile.FullName} dosyası DataTable'a yüklenemedi!");
                    }
                }
                else
                {
                    Console.WriteLine($"UYARI: {modulePath} klasöründe CSV dosyası bulunamadı!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modül verisi yüklenirken hata: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
        private void UpdateUIForLoadedProject()
        {
            try
            {
                // veri_listesi_seçimi ComboBox'ını güncelle
                if (veri_listesi_seçimi != null)
                {
                    veri_listesi_seçimi.Refresh();
                }

                // Modül butonlarını etkinleştir/devre dışı bırak
                UpdateModuleButtonsState();

                // DataGridView'ı güncelle
                if (dataGridView_girdi != null && GirdiModülü.dataTablesByType.Count > 0)
                {
                    // İlk veri tipini göster
                    var firstModule = GirdiModülü.dataTablesByType.Keys.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstModule))
                    {
                        // GirdiModülü'nü güncelle
                        if (girdiModülleri.ContainsKey(firstModule))
                        {
                            girdiModülleri[firstModule].importedDataTable = GirdiModülü.dataTablesByType[firstModule];
                        }

                        // DataGridView'ı güncelle
                        dataGridView_girdi.DataSource = GirdiModülü.dataTablesByType[firstModule];
                        dataGridView_girdi.Refresh();

                        // ComboBox'ta da seç
                        if (veri_listesi_seçimi != null)
                        {
                            for (int i = 0; i < veri_listesi_seçimi.Items.Count; i++)
                            {
                                if (veri_listesi_seçimi.Items[i].ToString() == firstModule)
                                {
                                    veri_listesi_seçimi.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                // Form başlığını güncelle
                if (PathService.CurrentWorkingFolder != null && PathService.CurrentWorkingFolder.StartsWith("proje_"))
                {
                    string projectName = PathService.CurrentWorkingFolder.Substring(6); // "proje_" çıkar
                    this.Text = $"SLF Yazılımı - {PathService.SelectedCity}/{PathService.SelectedDistrict} - Proje: {projectName}";
                }

                Debug.WriteLine("UI yüklenen projeye göre güncellendi");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UI güncellenirken hata: {ex.Message}");
            }
        }

        private void UpdateModuleButtonsState()
        {
            // Burada gerçek buton adlarınıza göre butonların etkinliğini kontrol edin
            // Örnek: Bazı butonlar belirli modüllerin yüklenmiş olmasını gerektirebilir

            // DTR verisi yüklenmişse EA Şarj ve DEK modülleri etkinleştir
            bool dtrLoaded = GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri");

            // Bu kısımda gerçek buton adlarınızı kullanmalısınız
            // Örnek:
            if (dtrLoaded)
            {
                // EA Şarj ve DEK modülleri için butonları etkinleştir
                if (OpenModuleButton != null)
                    OpenModuleButton.Enabled = true;

                if (GelecekSimButton != null)
                    GelecekSimButton.Enabled = dtrLoaded;

                if (DEKSimButton != null)
                    DEKSimButton.Enabled = dtrLoaded;
            }

            // Diğer modüller için benzer kontroller eklenebilir
        }


        // Modül verilerini CSV olarak kaydet
        private void SaveAllModuleDataToCSV(string projectPath)
        {
            string girdilerPath = Path.Combine(projectPath, "Girdiler");

            // Klasör yoksa oluştur
            if (!Directory.Exists(girdilerPath))
            {
                Directory.CreateDirectory(girdilerPath);
            }

            foreach (var kvp in GirdiModülü.dataTablesByType)
            {
                string moduleKey = kvp.Key;
                DataTable moduleData = kvp.Value;

                string moduleFolderName = moduleKey.Replace(" ", "_");
                string moduleFolder = Path.Combine(girdilerPath, moduleFolderName);

                // Modül klasörü yoksa oluştur
                if (!Directory.Exists(moduleFolder))
                {
                    Directory.CreateDirectory(moduleFolder);
                }

                // Dosya adı oluştur
                string fileName = $"{moduleFolderName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = Path.Combine(moduleFolder, fileName);

                // CSV olarak kaydet
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Başlık satırı
                    int columnCount = moduleData.Columns.Count;
                    for (int i = 0; i < columnCount; i++)
                    {
                        sw.Write(moduleData.Columns[i].ColumnName);
                        if (i < columnCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine();

                    // Veri satırları
                    foreach (DataRow row in moduleData.Rows)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            // Null değerleri boş string olarak yaz
                            string value = row[i]?.ToString() ?? "";

                            // Virgül içeren değerleri çift tırnak içine al
                            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }

                            sw.Write(value);
                            if (i < columnCount - 1)
                            {
                                sw.Write(",");
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- CUSTOM CLASSES ------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        // Nokta veri yapısı
        public class NoktaVeri
        {
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public double Bina_Demandi { get; set; }
            public int Abone_Sayısı { get; set; }

            public string CellId { get; set; } // Cell ID (optional)

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
                System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 170, Height = 70 };
                System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Tamam", Left = 170, Width = 100, Top = 85, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- EA ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        private async void gMapControl_Ea_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {

            if (isAddingChargingStation)
            {
                // Yeni marker oluştur
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.yellow)
                {
                    ToolTipText = "Yeni Şarj İstasyonu"
                };
                markerOverlay_ea.Markers.Add(marker);

                // Nokta verisini oluştur
                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem = Math.Round(pointClick.Lat, 4),
                    Boylam = Math.Round(pointClick.Lng, 4)
                };

                // Popup formu göster
                using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                {
                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        // Başarılı olduğunda harita verilerini yükle
                        await eaHaritayaVeriYukleAsync();
                    }
                    else if (popupForm.OperationCancelled)
                    {
                        // İşlem iptal edilirse marker'ı kaldır
                        markerOverlay_ea.Markers.Remove(marker);
                    }
                }

                // İşaretleme işlemini sıfırla
                isAddingChargingStation = false;
                return;
            }

            OnMapClickEventi(pointClick, e, markerOverlay_ea, ref polygonPoints_ea,
                ref polygonOverlay_ea, Mesafe_Dek, mesafe_metre_DeK);

        }
        private async Task HandlePopupFormAsync(PointLatLng point, string cellId)
        {
            NoktaVeri noktaVeri_marker = new NoktaVeri
            {
                Enlem = Math.Round(point.Lat, 4),
                Boylam = Math.Round(point.Lng, 4),
                CellId = cellId
            };

            using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
            {
                if (popupForm.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("Popup form closed with OK. Updating data...");
                    // await eaHaritayaVeriYukleAsync();

                    DataTable dataTable = dataGridView_girdi.DataSource as DataTable;
                    DataRow updatedRow = dataTable.Rows.Cast<DataRow>().FirstOrDefault(r => r["id"].ToString() == cellId);
                    if (updatedRow != null)
                    {
                        Console.WriteLine($"Cell {cellId}: AC (Home): {updatedRow["AC (Home)_count"]}, " +
                                          $"AC (Work): {updatedRow["AC (Work)_count"]}, " +
                                          $"AC (Public): {updatedRow["AC (Public)_count"]}, " +
                                          $"Fast DC: {updatedRow["Fast DC_count"]}");
                    }
                    else
                    {
                        Console.WriteLine($"No row found for Cell {cellId} in DataTable.");
                    }

                    Console.WriteLine("Calling HaritaUzerindeSimulasyonGosterimi...");
                    await HaritaUzerindeSimulasyonGosterimi(dataTable);
                    Console.WriteLine("HaritaUzerindeSimulasyonGosterimi completed.");
                }
            }
        }
        private void RemoveMarkerFromOverlays(GMapMarker marker)
        {
            if (markerOverlay_ea.Markers.Contains(marker))
            {
                markerOverlay_ea.Markers.Remove(marker);
            }

            if (simulationOverlay.Markers.Contains(marker))
            {
                simulationOverlay.Markers.Remove(marker);
            }

            if (cellToolTipOverlay.Markers.Contains(marker))
            {
                cellToolTipOverlay.Markers.Remove(marker);
            }
        }
        private async void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check if the user is in "adding charging station" mode
                if (isAddingChargingStation)
                {
                    // Use the selected CellId from ModülFormu
                    string cellId = item.Tag?.ToString() ?? ModülFormu.SelectedCellId;

                    // Create a temporary marker for the charging station at the clicked location
                    GMapMarker marker = new GMarkerGoogle(item.Position, GMarkerGoogleType.yellow)
                    {
                        ToolTipText = "Yeni Şarj İstasyonu",
                        Tag = cellId // Store CellId in the marker's Tag temporarily
                    };

                    try
                    {
                        // Use the helper method to handle the popup form
                        await HandlePopupFormAsync(item.Position, cellId);
                    }
                    catch
                    {
                        RemoveMarkerFromOverlays(marker);
                    }

                    // Reset the flag after adding the station
                    isAddingChargingStation = false;

                    return;
                }
            }
        }
        private GMapMarker FindMarkerAtPosition(PointLatLng point)
        {
            foreach (var marker in cellToolTipOverlay.Markers)
            {
                if (marker.Position.Lat == point.Lat && marker.Position.Lng == point.Lng)
                {
                    return marker;
                }
            }
            return null;
        }
        private void AddMarkerToMap(NoktaVeri noktaVeri)
        {
            // Create a new marker for the charging station
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(noktaVeri.Enlem, noktaVeri.Boylam), GMarkerGoogleType.yellow)
            {
                ToolTipText = $"Şarj İstasyonu: {noktaVeri.CellId}",
                Tag = noktaVeri.CellId // Store CellId in the marker's Tag
            };

            // Add the marker to the appropriate overlay
            markerOverlay_ea.Markers.Add(marker);
            simulationOverlay.Markers.Add(marker);
            cellToolTipOverlay.Markers.Add(marker);

            // Refresh the map to display the new marker
            gMapControl_EA.Refresh();
        }
        private void EA_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
              //  ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void EaSimMaxBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (EaSimMaxBtn.Checked)
            {

                SelectedSpeed = "Hızlı";
            }
        }

        private void EaSimMinBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (EaSimMinBtn.Checked)
            {

                SelectedSpeed = "Yavaş";
            }
        }

        private void EaSimDefBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (EaSimMinBtn.Checked)
            {
                SelectedSpeed = "Varsayılan";
            }
        }
        private void ToggleMarkers(string markerType, bool isVisible)
        {
            // Iterate through all overlays and markers
            foreach (var overlay in gMapControl_EA.Overlays)
            {
                foreach (var marker in overlay.Markers)
                {
                    // Check if the marker is a GMarkerGoogle and has the specified type in its Tag
                    if (marker is GMarkerGoogle googleMarker && googleMarker.Tag?.ToString() == markerType)
                    {
                        // Update the marker's visibility
                        googleMarker.IsVisible = isVisible;
                    }
                }
            }

            // Refresh the map to reflect changes
            gMapControl_EA.Refresh();
        }

        private async void SimulasyonSonucGoruntule_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks while processing
            EAStationAddButton.Enabled = false;
            EASimButton.Enabled = false;

            try
            {
                string filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\çıktı\evcs_monte_carlo_distribution_kumulatif3 - Copy.xlsx";
                DataTable simulationData;
                try
                {
                    // Excel dosyasını aç
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        // Yıl seçimine göre sayfayı seç (SelectedYear değeri, sayfa indeksini temsil eder)
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[SelectedYear];

                        // Veriyi DataTable'a yükle
                        simulationData = excelService.LoadWorksheetIntoDataTable(worksheet);
                        // Filter DataTable based on SelectedDistrict and its ID
                        /*                        if (SelectedDistrict != null)
                                                {
                                                    if (districtIdMap.TryGetValue(SelectedDistrict, out string districtId))
                                                    {
                                                        var filteredRows = veriMonteCarlo.AsEnumerable()
                                                            .Where(row => row.Field<string>("ilce") == districtId)
                                                            .CopyToDataTable();
                                                        veriMonteCarlo = filteredRows; // Update with filtered data
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show($"No ID mapping found for district: {SelectedDistrict}. No data will be displayed.",
                                                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                        veriMonteCarlo.Clear(); // Clear data to prevent displaying all districts
                                                        return; // Exit the method
                                                    }
                                                }*/
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                    return; // Hata durumunda işlemi sonlandır
                }

                /*                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                                    {
                                        // Map SelectedYear index to actual year
                                        int baseYear = slfStartYear; // e.g., 2024
                                        string year = (SelectedYear != -1 && SelectedYear < (slfEndYear - slfStartYear + 1))
                                            ? (baseYear + SelectedYear).ToString()
                                            : "2025";

                                        ExcelWorksheet worksheet = package.Workbook.Worksheets[year];
                                        if (worksheet == null)
                                        {
                                            MessageBox.Show($"Worksheet for year {year} not found in output file.",
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        simulationData = excelService.LoadWorksheetIntoDataTable(worksheet);
                                }*/

                // Log column names for debugging
                Console.WriteLine("DataTable Columns: " + string.Join(", ", simulationData.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                gMapControl_EA.Overlays.Clear();
                gMapControl_EA.Refresh();
                // Merkezi Nokta Hesaplama ve Harita Üzerinde Gösterim
                HesaplaMerkezNoktaVeEkle(simulationData);
                await HaritaUzerindeSimulasyonGosterimi(simulationData);
                // Assuming you have a DataTable named 'veriTablosu' and a year (e.g., 2023)
                // await HaritaUzerindeSimulasyonGosterimiWithNewPoints(veriTablosu, 2023);


                MessageBox.Show("Veri başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
            }
            finally
            {
                EAStationAddButton.Enabled = true;
                EASimButton.Enabled = true;

            }
        }


        private async void EANewSimulationResultsButton_Click(object sender, EventArgs e)
        {
            // Disable buttons and TrackBar to prevent interaction while processing
            // EAStationAddButton.Enabled = false;
            EANewSimulationResultsButton.Enabled = false;
            SimulasyonSonucGoruntule.Enabled = false;

            try
            {
                Cursor = Cursors.WaitCursor;
                if (statusLabel != null)
                {
                    statusLabel.Text = "Python script started. This may take a while. Please wait...";
                    statusLabel.Visible = true;
                }
                if (progressBar != null)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.Visible = true;
                }

                string inputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\girdiler\new_buildings_2024_2035.xlsx";
                string outputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\çıktı\evcs_monte_carlo_distribution_kumulatif3 - Copy.xlsx";

                if (!File.Exists(inputFilePath))
                {
                    MessageBox.Show("Input file not found! Please ensure the file is saved correctly.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await RunPythonScriptAsync(inputFilePath);

                if (!File.Exists(outputFilePath))
                {
                    MessageBox.Show("Output file not generated! Please check the Python script.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Call SimilasyonSonucGoruntule to handle display
                //SimilasyonSonucGoruntule();

                // Add info message box to inform user of completion
                MessageBox.Show("Simulation process completed successfully!",
                    "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                if (progressBar != null)
                    progressBar.Visible = false;
                if (statusLabel != null)
                    statusLabel.Text = "Simulation process completed";

                EANewSimulationResultsButton.Enabled = true;
                // EAStationAddButton.Enabled = true;
                SimulasyonSonucGoruntule.Enabled = true;
            }
        }
        /*        private async void EANewSimulationResultsButton_Click(object sender, EventArgs e)
                {
                    // Disable buttons and TrackBar to prevent interaction while processing
                   // EAStationAddButton.Enabled = false;
                    EANewSimulationResultsButton.Enabled = false;
                    SimulasyonSonucGoruntule.Enabled = false;

                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        if (statusLabel != null)
                        {
                            statusLabel.Text = "Python script started. This may take a while. Please wait...";
                            statusLabel.Visible = true;
                        }
                        if (progressBar != null)
                        {
                            progressBar.Style = ProgressBarStyle.Marquee;
                            progressBar.Visible = true;
                        }

                        string inputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\girdiler\new_buildings_2024_2035.xlsx";
                        string outputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\çıktı\evcs_monte_carlo_distribution_kumulatif3 - Copy.xlsx";

                        if (!File.Exists(inputFilePath))
                        {
                            MessageBox.Show("Input file not found! Please ensure the file is saved correctly.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        await RunPythonScriptAsync(inputFilePath);

                        if (!File.Exists(outputFilePath))
                        {
                            MessageBox.Show("Output file not generated! Please check the Python script.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Call SimilasyonSonucGoruntule to handle display
                        //SimilasyonSonucGoruntule();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                        if (progressBar != null)
                            progressBar.Visible = false;
                        if (statusLabel != null)
                            statusLabel.Text = "Simulation process completed";

                        EANewSimulationResultsButton.Enabled = true;
                       // EAStationAddButton.Enabled = true;
                        SimulasyonSonucGoruntule.Enabled = true;
                    }
                }*/
        private async Task RunPythonScriptAsync(string inputFilePath)
        {
            try
            {
                string pythonScriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\ea\V2\Entegrasyon\EA_kumulativ.py";
                string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonExePath,
                    Arguments = $"\"{pythonScriptPath}\" \"{inputFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    await Task.Run(() => process.WaitForExit());

                    string output = await outputTask;
                    string error = await errorTask;

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python script failed with exit code {process.ExitCode}.\nError: {error}");
                    }
                    else if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine($"Python output: {output}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error running Python script: {ex.Message}");
            }
        }

        // Add this event handler for the checkbox
        private void EAPointsLayerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (eaOverlay == null) return;

            if (EAPointsLayerCheckBox.Checked)
            {
                if (!gMapControl_EA.Overlays.Contains(eaOverlay))
                {
                    gMapControl_EA.Overlays.Add(eaOverlay);
                }
            }
            else
            {
                if (gMapControl_EA.Overlays.Contains(eaOverlay))
                {
                    gMapControl_EA.Overlays.Remove(eaOverlay);
                }
            }
            gMapControl_EA.Refresh();
        }

        // Şehir seçimi yapıldığında çağrılan metot
        // Şehir seçimi yapıldığında çağrılan metot
        private void ilSecimiMonteCarlo(object sender, EventArgs e)
        {
            // Always clear the district combo box and reset SelectedDistrict
            comboBox_ea_ilce_secimi.Items.Clear();
            comboBox_ea_ilce_secimi.SelectedIndex = -1; // Ensure no selection
            comboBox_ea_ilce_secimi.Enabled = false; // Disable by default
            SelectedDistrict = null;
            SelectedCity = comboBox_ea_il_secimi.SelectedItem.ToString();
            if (cityDistricts.TryGetValue(SelectedCity, out var districts))
            {
                comboBox_ea_ilce_secimi.Invoke(new Action(() =>
                {
                    comboBox_ea_ilce_secimi.Items.Clear();
                    comboBox_ea_ilce_secimi.Items.AddRange(districts.ToArray());
                    comboBox_ea_ilce_secimi.SelectedIndex = -1;
                    comboBox_ea_ilce_secimi.Enabled = true;
                    comboBox_ea_ilce_secimi.Refresh();
                }));
            }
            /*            // Update SelectedCity if a valid selection exists
                        if (comboBox_ea_il_secimi.SelectedItem != null)
                        {
                            SelectedCity = comboBox_ea_il_secimi.SelectedItem.ToString();

                            // Populate district combo box based on selected city
                            if (cityDistricts.TryGetValue(SelectedCity, out var districts))
                            {
                                comboBox_ea_ilce_secimi.Items.AddRange(districts.ToArray());
                                comboBox_ea_ilce_secimi.Enabled = true;
                            }
                        }*/
            else
            {
                SelectedCity = null;
            }

            // Update button enablement and map position
            // CheckSelections();

            if (SelectedCity != null && cityCoordinates.TryGetValue(SelectedCity, out PointLatLng coordinates))
            {
                gMapControl_EA.Position = coordinates;
                gMapControl_EA.Zoom = 12;
            }
        }
        private void ilceSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox_ea_ilce_secimi.SelectedItem != null)
            {
                SelectedDistrict = comboBox_ea_ilce_secimi.SelectedItem.ToString();
                Console.WriteLine($"Selected District: {SelectedDistrict}");
            }
            else
            {
                SelectedDistrict = null;
                Console.WriteLine("District selection cleared.");
            }
            //   CheckSelections();
        }
        // Yıl seçimi yapıldığında çağrılan metot
        private void yilSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox_ea_yıl_secimi.SelectedIndex != -1)  // Geçerli bir seçim yapıldığında
            {
                SelectedYear = comboBox_ea_yıl_secimi.SelectedIndex;  // Yıl indeksini ayarla
                                                                      //  CheckSelections();  // Seçim durumunu kontrol et
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
            istasyonAdetLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
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

        private DataTable FormatEATableForDisplay(DataTable originalEATable)
        {
            // Yeni bir DataTable oluşturun
            DataTable formattedEATable = new DataTable();

            // İhtiyacınız olan sütunları ekleyin
            formattedEATable.Columns.Add("Hücre ID", typeof(string));
            formattedEATable.Columns.Add("Ilce", typeof(string)); // İlçe isimleri Çiğli ve Karşıyaka olarak ayarlanacak
            formattedEATable.Columns.Add("AC (Home)", typeof(int));
            formattedEATable.Columns.Add("AC (Work)", typeof(int));
            formattedEATable.Columns.Add("AC (Public)", typeof(int));
            formattedEATable.Columns.Add("Fast DC", typeof(int));
            formattedEATable.Columns.Add("İstasyon Gücü(kW)", typeof(int));
            formattedEATable.Columns.Add("Tüketim (kW/h)", typeof(int));

            // Orijinal tablodaki her bir satırı işleyin
            foreach (DataRow row in originalEATable.Rows)
            {
                // Yeni bir satır oluşturun
                DataRow newRow = formattedEATable.NewRow();

                // ID değerini alın
                newRow["Hücre ID"] = row["id"].ToString();  // Change "ID" to "Hücre ID" to match the formatted table column

                // İlçe değerini dönüştür (1 = Çiğli, 2 = Karşıyaka)
                int ilceValue = Convert.ToInt32(row["ilce"]);
                newRow["Ilce"] = ilceValue == 1 ? "Çiğli" : ilceValue == 2 ? "Karşıyaka" : "Eskişehir";

                // AC ve DC istasyon sayısını alın
                newRow["AC (Home)"] = Convert.ToInt32(row["AC (Home)_count"]);
                newRow["AC (Work)"] = Convert.ToInt32(row["AC (Work)_count"]);
                newRow["AC (Public)"] = Convert.ToInt32(row["AC (Public)_count"]);
                newRow["Fast DC"] = Convert.ToInt32(row["Fast DC_count"]);

                // Yeni satırı formatlanmış tabloya ekleyin
                formattedEATable.Rows.Add(newRow);
            }

            return formattedEATable;
        }
        private void EAStationAddButton_Click(object sender, EventArgs e)
        {

            try
            {
                // Show wait cursor
                Cursor = Cursors.WaitCursor;

                // Check if the "EA Şarj Verileri" key exists in the dataTablesByType dictionary
                if (!GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
                {
                    MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                    return;
                }

                gMapControl_EA.OnMarkerClick -= gMapControl_EA_OnMarkerClick;
                //   gMapControl_EA.OnMapClick -= gMapControl_Ea_OnMapClick;

                // Use dataGridView1.DataSource as the DataTable instead of eaDataTable
                DataTable dataTable = dataGridView_girdi.DataSource as DataTable;
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                    return;
                }

                /*                if (!gMapControl_EA.Overlays.Contains(simulationOverlay) || !gMapControl_EA.Overlays.Contains(cellToolTipOverlay))
                                {
                                    gMapControl_EA.OnMapClick += gMapControl_Ea_OnMapClick;
                                }
                */
                // Check if we are in the process of adding a charging station
                if (!isAddingChargingStation)
                {
                    MessageBox.Show("Lütfen harita üzerinde şarj istasyonu koordinatlarınızı belirleyiniz.");
                    isAddingChargingStation = true;
                    gMapControl_EA.OnMarkerClick += gMapControl_EA_OnMarkerClick;
                    return; // Exit to wait for the user to click on the map
                }

                // Get the clicked point on the map
                var pointClick = gMapControl_EA.FromLocalToLatLng(MousePosition.X, MousePosition.Y);

                // Refresh the map to show the new marker
                gMapControl_EA.Refresh();

                // Reset the flag after adding the station
                isAddingChargingStation = false;
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restore cursor to default
                Cursor = Cursors.Default;
            }
        }
        //  private GMapOverlay eaOverlay; // Add this as a class-level variable

        private async Task eaHaritayaVeriYukleAsync()
        {
            int redDc = 0;
            int greenAc = 0;

            try
            {
                // Initialize the overlay if not already created
                if (eaOverlay == null)
                {
                    eaOverlay = new GMapOverlay("EA Layer");
                }

                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                DataTable eaData = await Task.Run(() => GirdiModülü.dataTablesByType["EA Şarj Verileri"]);

                if (eaData != null && eaData.Rows.Count > 0)
                {
                    greenAc = 0;  // Reset counters
                    redDc = 0;

                    // Clear existing markers
                    eaOverlay.Markers.Clear();

                    Invoke(new Action(() =>
                    {
                        foreach (DataRow row in eaData.Rows)
                        {
                            if (!eaData.Columns.Contains("EA_X_KOORDINAT") ||
                                !eaData.Columns.Contains("EA_Y_KOORDINAT") ||
                                !eaData.Columns.Contains("ISTASYON_GUCU") &&
                                !GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
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

                        // Only add overlay if checkbox is checked and it's not already added
                        if (EAPointsLayerCheckBox.Checked && !gMapControl_EA.Overlays.Contains(eaOverlay))
                        {
                            gMapControl_EA.Overlays.Add(eaOverlay);
                        }

                        gMapControl_EA.Refresh();
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

        private async void gelecekSimilasyonGoruntule(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks while processing
            EAStationAddButton.Enabled = false;

            /*            // Checkbox'ları görünür hale getir
                        checkBox_AC_Home.Visible = true;
                        checkBox_AC_Public.Visible = true;
                        checkBox_AC_Work.Visible = true;
                        checkBox_DC_Fast.Visible = true;
                        checkBox_AC_Public.Checked = true;
                        checkBox_AC_Work.Checked = true;
                        checkBox_AC_Home.Checked = true;
                        checkBox_DC_Fast.Checked = true;*/

            gMapControl_EA.Overlays.Clear();
            gMapControl_EA.Refresh();

            // Şehir ve hız seçimine göre dosya yolunu ayarla
            string filePath = "";

            if (SelectedCity == "İzmir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_Yüksek.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_Düşük.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_baz.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_Esk_Yüksek.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_Esk_Düşük.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_esk_baz.xlsx";
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir şehir ve senaryo seçiniz.");
                return;
            }

            try
            {
                // Excel dosyasını aç
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Yıl seçimine göre sayfayı seç
                    int baseYear = slfStartYear; // e.g., 2024
                    string year = (SelectedYear != -1 && SelectedYear < (slfEndYear - slfStartYear + 1))
                        ? (baseYear + SelectedYear).ToString()
                        : "2025";

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[year];
                    if (worksheet == null)
                    {
                        MessageBox.Show($"Worksheet for year {year} not found in output file.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Load the DataTable
                    veriMonteCarlo = excelService.LoadWorksheetIntoDataTable(worksheet);

                    // Filter DataTable based on SelectedDistrict and its ID
                    if (SelectedDistrict != null)
                    {
                        if (districtIdMap.TryGetValue(SelectedDistrict, out string districtId))
                        {
                            var filteredRows = veriMonteCarlo.AsEnumerable()
                                .Where(row => row.Field<string>("ilce") == districtId)
                                .CopyToDataTable();
                            veriMonteCarlo = filteredRows; // Update with filtered data
                        }
                        else
                        {
                            MessageBox.Show($"No ID mapping found for district: {SelectedDistrict}. No data will be displayed.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            veriMonteCarlo.Clear(); // Clear data to prevent displaying all districts
                            return; // Exit the method
                        }
                    }
                }

                // Veri başarıyla yüklendiğinde bir bildirim gösterin
                MessageBox.Show("Veri başarıyla yüklendi.");
                EAStationAddButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                return;
            }

            DataTable cıktıPopup = FormatEATableForDisplay(veriMonteCarlo);
            DataGridView dataGridView = new DataGridView
            {
                DataSource = cıktıPopup,
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };

            // Merkezi Nokta Hesaplama ve Harita Üzerinde Gösterim
            HesaplaMerkezNoktaVeEkle(veriMonteCarlo);
            await HaritaUzerindeSimulasyonGosterimi(veriMonteCarlo);

            // Önceki popupForm varsa kapatın
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
                popupForm.Dispose();
            }

            popupForm = new Form
            {
                Text = "Hücre Analizi",
                Width = 730,
                Height = 600
            };

            popupForm.Controls.Add(dataGridView);
            // popupForm.Show();
        }
        GMapOverlay simulationOverlay = new GMapOverlay("Simulasyon_Layer");
        GMapOverlay cellToolTipOverlay = new GMapOverlay("CellToolTips");
        public static string SelectedCellId { get; set; }
        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
        {
            // Clear existing overlays and re-add them
            gMapControl_EA.Overlays.Clear();
            //   EAPointsLayerCheckBox.Checked = false;
            gMapControl_EA.Overlays.Add(simulationOverlay);
            gMapControl_EA.Overlays.Add(cellToolTipOverlay);
            // Uncheck the EAPointsLayerCheckBox since we're clearing all overlays
            Invoke(new Action(() =>
            {
                EAPointsLayerCheckBox.Checked = false;
            }));
            // Create a transparent bitmap for invisible markers
            Bitmap transparentBitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(transparentBitmap))
            {
                g.Clear(Color.Transparent);
            }

            foreach (DataRow row in veriTablosu.Rows)
            {
                if (row["Enlem"] == DBNull.Value || row["Boylam"] == DBNull.Value) continue;

                double enlem = Convert.ToDouble(row["Enlem"]);
                double boylam = Convert.ToDouble(row["Boylam"]);
                string cellId = row["id"] != DBNull.Value ? row["id"].ToString() : "N/A";

                // Get counts for each EV type, defaulting to 0 if null
                int acHomeCount = row["AC (Home)_count"] != DBNull.Value ? Convert.ToInt32(row["AC (Home)_count"]) : 0;
                int acWorkCount = row["AC (Work)_count"] != DBNull.Value ? Convert.ToInt32(row["AC (Work)_count"]) : 0;
                int acPublicCount = row["AC (Public)_count"] != DBNull.Value ? Convert.ToInt32(row["AC (Public)_count"]) : 0;
                int fastDcCount = row["Fast DC_count"] != DBNull.Value ? Convert.ToInt32(row["Fast DC_count"]) : 0;

                // Calculate total count
                int totalCount = acHomeCount + acWorkCount + acPublicCount + fastDcCount;

                // Build the detailed tooltip text for all cells
                string tooltipText = $"Cell: {cellId}\n" +
                                     $"AC (Home): {acHomeCount}\n" +
                                     $"AC (Work): {acWorkCount}\n" +
                                     $"AC (Public): {acPublicCount}\n" +
                                     $"Fast DC: {fastDcCount}";

                if (totalCount == 0)
                {
                    // Invisible marker for empty cells
                    var invisibleMarker = new GMarkerGoogle(new PointLatLng(enlem, boylam), transparentBitmap)
                    {
                        ToolTipText = tooltipText,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        Tag = cellId
                    };
                    cellToolTipOverlay.Markers.Add(invisibleMarker);
                }
                else
                {
                    // Visible marker for cells with EV stations
                    GMarkerGoogleType markerType = DetermineMarkerType(acHomeCount, acWorkCount, acPublicCount, fastDcCount);
                    var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), markerType)
                    {
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        Tag = cellId,
                        ToolTipText = tooltipText
                    };
                    simulationOverlay.Markers.Add(marker);
                }
            }

            // Refresh the map on the UI thread
            Invoke(new Action(() => gMapControl_EA.Refresh()));

            return Task.CompletedTask;
        }

        // Helper method to determine marker type (unchanged)
        private GMarkerGoogleType DetermineMarkerType(int acHomeCount, int acWorkCount, int acPublicCount, int fastDcCount)
        {
            int totalCount = acHomeCount + acWorkCount + acPublicCount + fastDcCount;
            if (totalCount == 0) return GMarkerGoogleType.gray_small; // Not used, but kept for consistency

            var counts = new[]
            {
        new { Type = "AC (Home)", Count = acHomeCount },
        new { Type = "AC (Work)", Count = acWorkCount },
        new { Type = "AC (Public)", Count = acPublicCount },
        new { Type = "Fast DC", Count = fastDcCount }
    };
            var dominantType = counts.OrderByDescending(c => c.Count).First().Type;

            if (dominantType == "AC (Home)")
            {
                return GMarkerGoogleType.green;
            }
            else if (dominantType == "AC (Work)")
            {
                return GMarkerGoogleType.blue;
            }
            else if (dominantType == "AC (Public)")
            {
                return GMarkerGoogleType.yellow;
            }
            else if (dominantType == "Fast DC")
            {
                return GMarkerGoogleType.red;
            }
            else
            {
                return GMarkerGoogleType.orange; // Fallback
            }
        }
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
            ToggleMarkers("AC-Home", checkBox_AC_Home.Checked);
        }

        private void checkBox_Ac_Work(object sender, EventArgs e)
        {
            ToggleMarkers("AC-Work", checkBox_AC_Work.Checked);
        }

        private void checkBox_Ac_Public(object sender, EventArgs e)
        {
            ToggleMarkers("AC-Public", checkBox_AC_Public.Checked);

        }

        private void checkBox_Dc_Fast(object sender, EventArgs e)
        {
            ToggleMarkers("DC-Fast", checkBox_DC_Fast.Checked);

        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- DEK ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        // Modül tabları veri importu mantıgında refer ediliyor. Silinmesin.
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
                return;
            }
            else
            {
                Console.WriteLine("Bilinmeyen tıklama türü");
            }
            var pointClick = gMapControl_DEK.FromLocalToLatLng(MousePosition.X, MousePosition.Y);

            //// Create or get the overlay for charging station markers
            //GMapOverlay chargingStationOverlay = gMapControl_EA.Overlays.FirstOrDefault(o => o.Id == "ChargingStationLayer");
            //if (chargingStationOverlay == null)
            //{
            //    chargingStationOverlay = new GMapOverlay("ChargingStationLayer");
            //    gMapControl_EA.Overlays.Add(chargingStationOverlay);
            //}

            // Refresh the map to show the new marker
            gMapControl_DEK.Refresh();

            // Reset the flag after adding the station
            isAddingDekPoint = false;
        }
        private async void gMapControl_Dek_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // Check if the user is in "adding charging station" mode
                if (isAddingDekPoint)
                {
                    // Use the selected CellId from ModülFormu
                    string cellId = item.Tag?.ToString() ?? ModülFormu.SelectedCellId;

                    // Create a temporary marker for the charging station at the clicked location
                    GMapMarker marker = new GMarkerGoogle(item.Position, GMarkerGoogleType.yellow)
                    {
                        ToolTipText = "Yeni DEK Noktası",
                        Tag = cellId // Store CellId in the marker's Tag temporarily
                    };

                    try
                    {
                        // Use the helper method to handle the popup form
                        await HandleDEKPopupFormAsync(item.Position, cellId);
                    }
                    catch
                    {
                        RemoveDEKMarkerFromOverlays(marker);
                    }

                    // Reset the flag after adding the station
                    isAddingDekPoint = false;

                    return;
                }
            }
        }
        private async void gMapControl_DEK_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            // DEK modülü için OnMapClickEventi çağrısı
            OnMapClickEventi(pointClick, e, markerOverlay_DEK, ref polygonPoints_DEK,
                ref polygonOverlay_DEK, Mesafe_Dek, mesafe_metre_DeK);

            if (isAddingDekPoint)
            {
                // Use the selected CellId from ModülFormu
                // Yeni marker oluştur
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green)
                {
                    ToolTipText = "Yeni DEK Noktası"
                };
                markerOverlay_DEK.Markers.Add(marker);

                // Nokta verisini oluştur
                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem = Math.Round(pointClick.Lat, 4),
                    Boylam = Math.Round(pointClick.Lng, 4)
                };

                // Popup formu göster
                using (DEKCenterPopupForm popupForm = new DEKCenterPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                {
                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        // Başarılı olduğunda harita verilerini yükle
                        await dekHaritayaVeriYukleAsync();
                    }
                    else if (popupForm.OperationCancelled)
                    {
                        // İşlem iptal edilirse marker'ı kaldır
                        markerOverlay_DEK.Markers.Remove(marker);
                    }
                }

                // İşaretleme işlemini sıfırla
                isAddingDekPoint = false;
                return;
            }
        }
        private async Task HandleDEKPopupFormAsync(PointLatLng point, string cellId)
        {
            NoktaVeri noktaVeri_marker = new NoktaVeri
            {
                Enlem = Math.Round(point.Lat, 4),
                Boylam = Math.Round(point.Lng, 4),
                CellId = cellId
            };

            using (DEKCenterPopupForm popupForm = new DEKCenterPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
            {
                if (popupForm.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("Popup form closed with OK. Updating data...");
                    // await eaHaritayaVeriYukleAsync();

                    DataTable dataTable = dataGridView_girdi.DataSource as DataTable;
                    DataRow updatedRow = dataTable.Rows.Cast<DataRow>().FirstOrDefault(r => r["id"].ToString() == cellId);
                    if (updatedRow != null)
                    {
                        Console.WriteLine($"Cell {cellId}: ");
                    }
                    else
                    {
                        Console.WriteLine($"No row found for Cell {cellId} in DataTable.");
                    }

                    Console.WriteLine("Calling HaritaUzerindeSimulasyonGosterimi...");
                    await HaritaUzerindeDEKSimulasyonGosterimi(dataTable);
                    Console.WriteLine("HaritaUzerindeSimulasyonGosterimi completed.");
                }
            }
        }
        private void RemoveDEKMarkerFromOverlays(GMapMarker marker)
        {
            /*            if (markerOverlay_ea.Markers.Contains(marker))
                        {
                            markerOverlay_ea.Markers.Remove(marker);
                        }*/

            if (DEKSimulationOverlay.Markers.Contains(marker))
            {
                DEKSimulationOverlay.Markers.Remove(marker);
            }

            if (DEKCellToolTipOverlay.Markers.Contains(marker))
            {
                DEKCellToolTipOverlay.Markers.Remove(marker);
            }
        }
        // DEK şehri seçildiğinde çağrılan metot
        private void dek_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_DEK_il.SelectedItem != null)  // Geçerli bir seçim yapıldığında
            {
                SelectedCity = comboBox_DEK_il.SelectedItem.ToString();  // Şehir adını ayarla
                SelectedDistrict = null;
                if (cityDistricts.TryGetValue(SelectedCity, out var districts))
                {
                    comboBox_dek_ilce_secimi.Invoke(new Action(() =>
                    {
                        comboBox_dek_ilce_secimi.Items.Clear();
                        comboBox_dek_ilce_secimi.Items.AddRange(districts.ToArray());
                        comboBox_dek_ilce_secimi.SelectedIndex = -1;
                        comboBox_dek_ilce_secimi.Enabled = true;
                        comboBox_dek_ilce_secimi.Refresh();
                    }));
                }
                else
                {
                    SelectedCity = null;
                }

                // Update button enablement and map position
                // CheckSelections();
                // Set map position based on selected city
                if (cityCoordinates.TryGetValue(SelectedCity, out PointLatLng coordinates))
                {
                    gMapControl_DEK.Position = coordinates; // Set the map's position
                    gMapControl_DEK.Zoom = 12; // Adjust the zoom level as needed
                }
            }
        }

        private async void dekSimulasyonGoruntule(object sender, EventArgs e)
        {
            DEKCenterAddButton.Enabled = false;
            gMapControl_DEK.Overlays.Clear();
            gMapControl_DEK.Refresh();

            // Şehir ve hız seçimine göre dosya yolunu ayarla
            string filePath = "";

            if (SelectedCity == "İzmir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\İzmir\dek_distribution_2024_2030_İzmir_yüksek.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\İzmir\dek_distribution_2024_2030_İzmir_düşük.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\İzmir\dek_distribution_2024_2030_3_İzmir_baz.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_yüksek.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_düşük.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_baz.xlsx";
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir şehir ve senaryo seçiniz.");
                return; // Geçerli bir şehir veya hız seçilmediyse işlemi sonlandır
            }

            DataTable dek_veri;

            /*            try
                        {
                            // Excel dosyasını aç
                            using (var package = new ExcelPackage(new FileInfo(filePath)))
                            {
                                // Yıl seçimine göre sayfayı seç (SelectedYear değeri, sayfa indeksini temsil eder)
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[SelectedYear];

                                // Veriyi DataTable'a yükle
                                dek_veri = excelService.LoadWorksheetIntoDataTable(worksheet);
                            }

                            // Veri başarıyla yüklendiğinde bir bildirim gösterin
                            MessageBox.Show("Veri başarıyla yüklendi.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                            return; // Hata durumunda işlemi sonlandır
                        }*/
            try
            {
                // Excel dosyasını aç
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Yıl seçimine göre sayfayı seç
                    int baseYear = slfStartYear; // e.g., 2024
                    string year = (SelectedYear != -1 && SelectedYear < (slfEndYear - slfStartYear + 1))
                        ? (baseYear + SelectedYear).ToString()
                        : "2025";

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[year];
                    if (worksheet == null)
                    {
                        MessageBox.Show($"Worksheet for year {year} not found in output file.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Load the DataTable
                    dek_veri = excelService.LoadWorksheetIntoDataTable(worksheet);

                    // Filter DataTable based on SelectedDistrict and its ID
                    if (SelectedDistrict != null)
                    {
                        if (districtIdMap.TryGetValue(SelectedDistrict, out string districtId))
                        {
                            var filteredRows = dek_veri.AsEnumerable()
                                .Where(row => row.Field<string>("ilce") == districtId)
                                .CopyToDataTable();
                            dek_veri = filteredRows; // Update with filtered data
                        }
                        else
                        {
                            MessageBox.Show($"No ID mapping found for district: {SelectedDistrict}. No data will be displayed.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dek_veri.Clear(); // Clear data to prevent displaying all districts
                            return; // Exit the method
                        }
                    }
                }

                // Veri başarıyla yüklendiğinde bir bildirim gösterin
                MessageBox.Show("Veri başarıyla yüklendi.");

                DEKCenterAddButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                return;
            }
            // Yeni bir DataGridView oluştur
            HesaplaMerkezNoktaVeEkle(dek_veri);
            DataTable dekResultPopup = FormatDEKTableForDisplay(dek_veri);

            DataGridView dataGridView = new DataGridView
            {
                DataSource = dekResultPopup,  // DataTable'ı bağla
                Dock = DockStyle.Fill,        // Formu doldurması için konumunu ayarla
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill // Sütunları otomatik boyutlandır
            };

            // Merkezi Nokta Hesaplama ve Harita Üzerinde Gösterim

            await HaritaUzerindeDEKSimulasyonGosterimi(dek_veri);
            //await HaritaUzerindeDekSimulasyonGosterimi(dek_veri);

            // Önceki popupForm varsa kapatın
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
                popupForm.Dispose();  // Eski formu serbest bırak
            }

            // Yeni popupForm'u oluşturun ve açın
            popupForm = new Form
            {
                Text = "DEK Hücre Analizi",
                Width = 800,
                Height = 600
            };

            popupForm.Controls.Add(dataGridView);
            // popupForm.Show(); // Yeni pencereyi göster
        }


        private void dekSimMinBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (dekSimMinBtn.Checked)
            {

                SelectedSpeed = "Yavaş";
            }
        }

        private void dekSimMaxBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (dekSimMaxBtn.Checked)
            {

                SelectedSpeed = "Hızlı";
            }
        }



        private void dekSimDefBtn_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (dekSimDefBtn.Checked)
                {

                    SelectedSpeed = "varsayılan";
                }
            }
        }

        // Nokta veri yapısı
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                // CheckSelections();
            }
        }

        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                // CheckSelections();
            }
        }

        public string SelectedDistrict
        {
            get => _selectedDistrict;
            set
            {
                _selectedDistrict = value;
                // CheckSelections();
            }
        }
        private void CheckSelections()
        {
            EASimButton.Enabled = SelectedYear != -1 && SelectedCity != null && SelectedDistrict != null;
            DEKSimButton.Enabled = SelectedYear != -1 && SelectedCity != null; //&& SelectedDistrict != null;
        }
      //  private GMapOverlay dekOverlay; // Add this as a class-level variable
        private async Task dekHaritayaVeriYukleAsync()
        {
            try
            {
                //  GMapOverlay dekOverlay = new GMapOverlay("Dek Layer");

                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                /*                if (gMapControl_DEK.Overlays.Contains(dekOverlay))
                                {
                                    gMapControl_DEK.Overlays.Remove(dekOverlay);
                                }*/

                // Initialize the overlay if not already created
                if (dekOverlay == null)
                {
                    dekOverlay = new GMapOverlay("DEK Layer");
                }

                DataTable dekData = await Task.Run(() => GirdiModülü.dataTablesByType["DEK Verileri"]);
                if (dekData != null && dekData.Rows.Count > 0)
                {
                    Invoke(new Action(() =>
                    {
                        foreach (DataRow row in dekData.Rows)
                        {
                            if (!dekData.Columns.Contains("DEK_X_KOORDINAT") ||
                                !dekData.Columns.Contains("DEK_Y_KOORDINAT") ||
                                !dekData.Columns.Contains("KAYNAK_TIPI") ||
                                !GirdiModülü.dataTablesByType.ContainsKey("DEK Verileri"))
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

                        //gMapControl_DEK.Overlays.Add(dekOverlay);
                        // Only add overlay if checkbox is checked and it's not already added
                        if (DEKPointsLayerCheckBox.Checked && !gMapControl_DEK.Overlays.Contains(dekOverlay))
                        {
                            gMapControl_DEK.Overlays.Add(dekOverlay);
                        }
                        gMapControl_DEK.Refresh();
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

        private DataTable FormatDEKTableForDisplay(DataTable originalDEKTable)
        {
            // Yeni bir DataTable oluşturun
            DataTable formattedDEKTable = new DataTable();

            // İhtiyacınız olan sütunları ekleyin
            formattedDEKTable.Columns.Add("ID", typeof(string));
            formattedDEKTable.Columns.Add("Ilce", typeof(string)); // İlçe isimleri Çiğli ve Karşıyaka olarak ayarlanacak
            formattedDEKTable.Columns.Add("DEK Değeri", typeof(double));


            // Orijinal tablodaki her bir satırı işleyin
            foreach (DataRow row in originalDEKTable.Rows)
            {
                // `DEK_distributed` değeri 0 olan satırları atla
                if (row["DEK_distributed"] != DBNull.Value && Convert.ToDouble(row["DEK_distributed"]) != 0)
                {
                    // Yeni bir satır oluşturun
                    DataRow newRow = formattedDEKTable.NewRow();

                    // ID, İlçe ve diğer sütunları doldurun
                    newRow["ID"] = row["id"].ToString();

                    // İlçe değerini dönüştür (1 = Çiğli, 2 = Karşıyaka, diğerleri "Bilinmeyen")
                    int ilceValue = Convert.ToInt32(row["ilce"]);
                    newRow["Ilce"] = ilceValue == 1 ? "Çiğli" : ilceValue == 2 ? "Karşıyaka" : "Bilinmeyen";

                    // Diğer değerler
                    newRow["DEK Değeri"] = Convert.ToDouble(row["DEK_distributed"]);
                    //newRow["Enlem"] = Convert.ToDouble(row["Enlem"]);
                    //newRow["Boylam"] = Convert.ToDouble(row["Boylam"]);

                    // Yeni satırı formatlanmış tabloya ekleyin
                    formattedDEKTable.Rows.Add(newRow);
                }
            }

            return formattedDEKTable;
        }

        GMapOverlay DEKSimulationOverlay = new GMapOverlay("Simulasyon_Layer");
        GMapOverlay DEKCellToolTipOverlay = new GMapOverlay("CellToolTips");
        public static string DEKSelectedCellId { get; set; }

        private Task HaritaUzerindeDEKSimulasyonGosterimi(DataTable veriTablosu)
        {
            // Clear existing overlays and re-add the global overlays
            gMapControl_DEK.Overlays.Clear();
            gMapControl_DEK.Overlays.Add(DEKSimulationOverlay);
            gMapControl_DEK.Overlays.Add(DEKCellToolTipOverlay);

            // Uncheck the DEKPointsLayerCheckBox since we're clearing all overlays
            Invoke(new Action(() =>
            {
                DEKPointsLayerCheckBox.Checked = false;
            }));

            // Create a transparent bitmap for invisible markers (size can be adjusted as needed)
            Bitmap transparentBitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(transparentBitmap))
            {
                g.Clear(Color.Transparent);
            }

            // Process each row in the DataTable
            foreach (DataRow row in veriTablosu.Rows)
            {
                // Extract basic data: latitude, longitude, and cell id.
                double enlem = Convert.ToDouble(row["Enlem"]);
                double boylam = Convert.ToDouble(row["Boylam"]);
                string cellId = row["id"] != DBNull.Value ? row["id"].ToString() : "N/A";

                // Get the DEK_distributed value and build the tooltip text
                double dekValue = row["DEK_distributed"] != DBNull.Value ? Convert.ToDouble(row["DEK_distributed"]) : 0;
                string tooltipText = $"ID: {cellId}\nDEK: {dekValue}";

                // If DEK_distributed is zero, add an invisible marker to the tooltip overlay.
                if (dekValue == 0)
                {
                    var invisibleMarker = new GMarkerGoogle(new PointLatLng(enlem, boylam), transparentBitmap)
                    {
                        ToolTipText = tooltipText,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        Tag = cellId
                    };
                    DEKCellToolTipOverlay.Markers.Add(invisibleMarker);
                }
                else
                {
                    // Otherwise, create a visible marker. Here we're using a blue marker type.
                    var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue)
                    {
                        ToolTipText = tooltipText,
                        ToolTipMode = MarkerTooltipMode.OnMouseOver,
                        Tag = cellId
                    };
                    DEKSimulationOverlay.Markers.Add(marker);
                }

                // Debug output per row (optional)
                Console.WriteLine($"Processed cell {cellId} at ({enlem}, {boylam}) with DEK: {dekValue}");
            }

            // Refresh the map control to display the new markers
            Invoke(new Action(() =>
            {
                gMapControl_DEK.Refresh();
                Console.WriteLine("Map refreshed.");
            }));

            return Task.CompletedTask;
        }


        private void dek_list_years(object sender, EventArgs e) // 
        {
            if (comboBox_DEK_Yıl.SelectedIndex != -1)  // Geçerli bir seçim yapıldığında
            {
                SelectedYear = comboBox_DEK_Yıl.SelectedIndex;  // Yıl indeksini ayarla
                //CheckSelections();  // Seçim durumunu kontrol et
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



        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- İMAR ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        private async void imar_dosya_seçimi_Click(object sender, EventArgs e)
        {
            await cbs.cbs_dosya_secimi(gMapControl_imar, this, tablo_formu.attribute_table);
        }

        private void İmar_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_imar?.Clear();
            polygonOverlay_imar?.Clear();

            cbs.CBS_ölç(mesafe_metre_imar, Mesafe_imar);
        }

        private void İmar_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_imar, rulerRoute_imar, gMapControl_imar,
                mesafe_metre_imar, Mesafe_imar);
        }

        private void İmar_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_imar, rulerRoute_imar, gMapControl_imar,
                    mesafe_metre_imar, Mesafe_imar);
        }

        private void gMapControl_imar_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_imar, Mesafe_imar, mesafe_metre_imar,
                rulerPoints_imar, markerOverlay_imar, rulerOverlay_imar, ref rulerRoute_imar,
                polygonPoints_imar, polygonOverlay_imar);
        }

        private void İmar_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void gMapControl_imar_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl_imar.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();

            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid == true)
            {
                cbs.ending_point = gMapControl_imar.FromLocalToLatLng(e.X, e.Y);
                cbs.UpdateSelectionPolygon(gMapControl_imar);
            }

            // eğer sadece 1 adet nokta seçilmişse, ve ikinci nokta dinamik olarak farklı yerlere
            // tıklanarak seçiliyorsa, mesafeyi de buna göre güncelle.
            if (isRulerActive && rulerPoints_imar.Count == 1 && isRulerEnabled == true)
            {

                var point = gMapControl_imar.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_imar != null && rulerRoute_imar.Distance != 0)
                {
                    rulerOverlay_imar.Routes.Remove(rulerRoute_imar);
                }
                rulerRoute_imar = new GMapRoute(new List<PointLatLng> { rulerPoints_imar[0], point },
                    "rulerRoute_imar");
                rulerRoute_imar.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_imar.Routes.Add(rulerRoute_imar);
                gMapControl_imar.Refresh();
            }
        }

        private void gMapControl_imar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                cbs.ending_point = gMapControl_imar.FromLocalToLatLng(e.X, e.Y);
                cbs.isSelecting_grid = false;
                gMapControl_imar.CanDragMap = true;

                // Clear the selection polygon and refresh the map
                gMapControl_imar.Overlays.Remove(cbs.bounding_box_overlay);
                cbs.AddGridToMap(gMapControl_imar);
                gMapControl_imar.Refresh();
            }
        }

        private void gMapControl_imar_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_imar, ref polygonPoints_imar,
                    ref polygonOverlay_imar, Mesafe_imar, mesafe_metre_imar);
        }


        private void İmar_Grid_Oluştur_Click(object sender, EventArgs e)
        {
            Grid_Seçenekler grid_formu = new Grid_Seçenekler();
            grid_formu.Tag = this;
            grid_formu.Owner = this;
            grid_formu.Show();
            grid_formu.Activate();
            grid_formu.StartPosition = FormStartPosition.CenterParent;
        }

        private void İmar_Fonksiyonlar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Fonksiyon.Show(Cursor.Position);
            }
        }

        private void gMapControl_yuk_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                try
                {
                    int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                    // Ensure checkbox_index is within valid range
                    if (checkbox_index < 0 || checkbox_index >= cbs.tüm_katmanlar_array_yuk.Length)
                    {
                        Console.WriteLine("Invalid checkbox index.");
                        return;
                    }

                    foreach (var polygon in cbs.tüm_katmanlar_array_yuk[checkbox_index].Polygons)
                    {
                        if (cbs.IsPointInPolygon(pointClick, polygon))
                        {
                            // Highlight the polygon and update the layer index
                            cbs.HighlightPolygon(polygon, layer_index, cbs.GetActiveGMapControl());
                            layer_index = checkbox_index;  // Update the current layer index to the clicked polygon's layer

                            // Try to get the attributes of the clicked polygon
                            if (cbs.polygonAttributes_yuk.TryGetValue(polygon, out DataRow row))
                            {
                                ShowAttributeRow(row);  // Show the row attributes
                                tablo_formu.Show();     // Display the table form
                            }
                            else
                            {
                                Console.WriteLine("Polygon attributes not found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions to prevent the app from crashing
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }


        private void gMapControl_imar_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                try
                {
                    int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                    // Ensure checkbox_index is within valid range
                    if (checkbox_index < 0 || checkbox_index >= cbs.tüm_katmanlar_array_imar.Length)
                    {
                        Console.WriteLine("Invalid checkbox index.");
                        return;
                    }

                    foreach (var polygon in cbs.tüm_katmanlar_array_imar[checkbox_index].Polygons)
                    {
                        if (cbs.IsPointInPolygon(pointClick, polygon))
                        {
                            // Highlight the polygon and update the layer index
                            cbs.HighlightPolygon(polygon, layer_index, cbs.GetActiveGMapControl());
                            layer_index = checkbox_index;  // Update the current layer index to the clicked polygon's layer

                            // Try to get the attributes of the clicked polygon
                            if (cbs.polygonAttributes_imar.TryGetValue(polygon, out DataRow row))
                            {
                                ShowAttributeRow(row);  // Show the row attributes
                                tablo_formu.Show();     // Display the table form
                            }
                            else
                            {
                                Console.WriteLine("Polygon attributes not found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions to prevent the app from crashing
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- YUK ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        private void trackBar_Yıllar_ValueChanged(object sender, EventArgs e)
        {
            int selectedYear = trackBar_Yıllar.Value;
            currentYear = selectedYear; // Update the current year
            yuk_yıl_deger.Text = $"{selectedYear}";

            // Construct the column name based on the selected year
            string columnName = $"Yük_Yoğunluğu_{selectedYear}";


            // Find the index of the overlay in tüm_katmanlar_array_imar_names that contains "xxx"
            string searchText = "SONUCLAR_Load_Density.kml"; // The text to search for
            overlayIndex = Array.FindIndex(cbs.tüm_katmanlar_array_names,
                name => name != null && name.Contains(searchText));

            // Check if the overlay was found
            if (overlayIndex == -1 || cbs.tüm_katmanlar_array_imar[overlayIndex] == null)
            {
                MessageBox.Show($"SONUCLAR_Load_Density.kml dosyası bulunamadı. Lütfen ilgili dosyanın SLF hesabı sonucu " +
                    $"oluşturulduğundan emin olunuz.");

            }
            else
            {
                legendPanel.Visible = true;

                // Call a method to update the heatmap using the selected year's data
                UpdateHeatmapForYear(columnName);

                // If a polygon is currently hovered, update the tooltip
                if (hoveredPolygon != null)
                {
                    UpdatePolygonToolTip(hoveredPolygon);
                }

            }

        }

        private void UpdateHeatmapForYear(string columnName)
        {
            // Use the active GMapControl (ensure you're consistent with one control)
            GMapControl mapControl = cbs.GetActiveGMapControl();

            // Instead of clearing all overlays, remove only the heatmapOverlay if it exists
            GMapOverlay existingHeatmapOverlay = mapControl.Overlays.FirstOrDefault(o => o.Id == "HeatmapOverlay");
            if (existingHeatmapOverlay != null)
            {
                mapControl.Overlays.Remove(existingHeatmapOverlay);
            }

            GMapOverlay originalOverlay = cbs.tüm_katmanlar_array_imar[overlayIndex];

            // Create a new overlay for the heatmap
            GMapOverlay heatmapOverlay = new GMapOverlay("HeatmapOverlay");

            // Create a new dictionary for the heatmap overlay's polygon attributes
            heatmapPolygonAttributes = new Dictionary<GMapPolygon, DataRow>();

            // Copy the contents of the original overlay to the heatmap overlay
            cbs.CopyOverlayContents(originalOverlay, heatmapOverlay, cbs.polygonAttributes_imar, heatmapPolygonAttributes);

            // Update the heatmap colors based on the data for the selected year
            cbs.CreateHeatmap(heatmapOverlay, cbs.tüm_katmanlar_datatable[overlayIndex], columnName, heatmapPolygonAttributes);

            // Update the legend (which should be independent)
            cbs.UpdateHeatmapLegend(); // Updated method to reuse existing controls

            // Add both the original overlay and the new heatmap overlay to the map
            //mapControl.Overlays.Add(originalOverlay);
            mapControl.Overlays.Add(heatmapOverlay);

            // Associate the heatmap overlay with checkBox_yuk_main
            checkBox_yuk_main.Tag = heatmapOverlay; // Store the overlay in the Tag property
            checkBox_yuk_main.Checked = true; // Make the heatmap visible by default
            checkBox_yuk_main.Text = "Yük Yoğunluğu Katmanı"; // Set a meaningful name
            checkBox_yuk_main.Visible = true; // Ensure the checkbox is visible

            buton_HTML.Visible = true;

            // Force a repaint by toggling the visibility of the heatmap overlay
            SetOverlayVisibility(heatmapOverlay, false); // Hide
            SetOverlayVisibility(heatmapOverlay, true);  // Show
                                                         //mapControl.Invalidate(); // Force a full repaint
            mapControl.Refresh(); // Refresh the map control
        }

        private void InitializeHeatmapLegendControls()
        {
            // Define the fixed brackets (same as in CreateHeatmapLegend)
            double[] brackets = { 0, 3, 5, 10, 20, 35, 50, 75, 100, 200, double.PositiveInfinity };
            string[] bracketLabels = { "0-3", "3-5", "5-10", "10-20", "20-35", "35-50", "50-75", "75-100", "100-200", "200-Inf" };
            int bracketCount = bracketLabels.Length; // Should be 10

            // Initialize arrays
            colorBoxes = new Panel[bracketCount];
            rangeLabels = new System.Windows.Forms.Label[bracketCount];

            // Create the unit label
            unitLabel = new System.Windows.Forms.Label
            {
                Text = "Yük Yoğunluğu (W/m²)",
                Font = new System.Drawing.Font("Verdana", 7, FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(5, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Name = "unitLabel"
            };
            legendPanel.Controls.Add(unitLabel);

            // Create color boxes and range labels for each bracket
            for (int i = 0; i < bracketCount; i++)
            {
                // Create a color box
                colorBoxes[i] = new Panel
                {
                    Size = new System.Drawing.Size(20, 20),
                    Location = new System.Drawing.Point(10, unitLabel.Bottom + 10 + (i * 25)),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Name = $"colorBox_{i}"
                };
                legendPanel.Controls.Add(colorBoxes[i]);

                // Create a range label
                rangeLabels[i] = new System.Windows.Forms.Label
                {
                    Text = bracketLabels[i],
                    Font = new System.Drawing.Font("Verdana", 8),
                    AutoSize = true,
                    Location = new System.Drawing.Point(colorBoxes[i].Right + 5, colorBoxes[i].Top),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Name = $"rangeLabel_{i}"
                };
                legendPanel.Controls.Add(rangeLabels[i]);
            }
        }

        private void checkBox_yuk_main_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            // Get the associated overlay from the Tag property
            GMapOverlay overlay = cb.Tag as GMapOverlay;
            if (overlay == null) return;

            // Toggle visibility of the overlay
            bool isVisible = cb.Checked;
            SetOverlayVisibility(overlay, isVisible);

            // Refresh the map control to show updates
            cbs.GetActiveGMapControl().ReloadMap();
            cbs.GetActiveGMapControl().Refresh();
        }

        // Helper to toggle polygons/routes/markers
        private void SetOverlayVisibility(GMapOverlay overlay, bool visible)
        {
            if (overlay == null) return;

            foreach (var poly in overlay.Polygons)
                poly.IsVisible = visible;

            foreach (var route in overlay.Routes)
                route.IsVisible = visible;

            foreach (var marker in overlay.Markers)
                marker.IsVisible = visible;
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- ELF ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //



        // Save button logic to update Excel file with changes from DataGridViews
        private async void ELFScenerioSaveButton_Click(object sender, EventArgs e)
        {

            string originalFilePath = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                (string)ana_menu_form_objesi.config.İl,
                (string)ana_menu_form_objesi.config.İlçe,
                (string)ana_menu_form_objesi.config.ELF.INPUT_FILE);

            try
            {
                await Task.Run(() =>
                {
                    using (var package = new ExcelPackage(new FileInfo(originalFilePath)))
                    {
                        // Update worksheets with data from DataGridViews
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[1], ELFMinSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[2], ELFLowSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[3], ELFBaseSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[4], ELFHighSenaryoTable);
                        _excelService.UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[5], ELFMaxSenaryoTable);

                        // Save the modified Excel file
                        package.SaveAs(originalFilePath);
                    }
                });

                MessageBox.Show("Kullanıcı değişiklikleri excel dosyasına kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya güncelleme hatası: {ex.Message}");
            }
        }

        private void LoadEkonometrikResults(string resultsFilePath)
        {
            if (!File.Exists(resultsFilePath))
            {
                MessageBox.Show("Sonuç dosyası bulunamadı.");
                return;
            }

            using (var package = new ExcelPackage(new FileInfo(resultsFilePath)))
            {
                // Load the corresponding results into each DataGridView
                LoadWorksheetToDataGridView(package.Workbook.Worksheets[0], ELFMinimumResultsTable);
                LoadWorksheetToDataGridView(package.Workbook.Worksheets[1], ELFDüşükResultsTable);
                LoadWorksheetToDataGridView(package.Workbook.Worksheets[2], ELFBazResultsTable);
                LoadWorksheetToDataGridView(package.Workbook.Worksheets[3], ELFYüksekResultsTable);
                LoadWorksheetToDataGridView(package.Workbook.Worksheets[4], ELFMaksimumResultsTable);
            }

            // Switch to the results tab after loading all the data
            SenaryoModuleTabControl.SelectedTab = EkonometrikSonuclarTabPage;
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

        private async void ShowResultsButton_Click(object sender, EventArgs e)
        {
            ELFResultsFilePath = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                (string)ana_menu_form_objesi.config.İl,
                (string)ana_menu_form_objesi.config.İlçe,
                (string)ana_menu_form_objesi.config.ELF.SONUÇLAR).Replace('/', '\\');

            // Asynchronous task to load the Excel package
            await Task.Run(() =>
            {
                using (var package = new ExcelPackage(new FileInfo(ELFResultsFilePath)))
                {
                    // Clear previous data in the DataGridViews
                    Invoke(new Action(() =>
                    {
                        // Set DataSources to null to clear previous data
                        ELFMinimumResultsTable.DataSource = null;
                        ELFDüşükResultsTable.DataSource = null;
                        ELFBazResultsTable.DataSource = null;
                        ELFYüksekResultsTable.DataSource = null;
                        ELFMaksimumResultsTable.DataSource = null;
                    }));

                    // Load sheets into their respective DataGridViews
                    var worksheets = new[] { "Bagımlı_Degisken_Tahminleri_1", "Bagımlı_Degisken_Tahminleri_2", "Bagımlı_Degisken_Tahminleri_3", "Bagımlı_Degisken_Tahminleri_4", "Bagımlı_Degisken_Tahminleri_5" };
                    var dataGrids = new[] { ELFMinimumResultsTable, ELFDüşükResultsTable, ELFBazResultsTable, ELFYüksekResultsTable, ELFMaksimumResultsTable };

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

        }


        private async Task<string> RunModelRScript()
        {
            ELFrScriptModelPath = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)ana_menu_form_objesi.config.İl,
                    (string)ana_menu_form_objesi.config.ELF.Rscript_Yolu_Model).Replace('/', '\\');

            string configPath = Path.Combine(((string)ana_menu_form_objesi.projectRoot).Replace('/', '\\'),
                "config.json");

            var processInfo = new ProcessStartInfo
            {
                FileName = "Rscript.exe", // Use .exe explicitly
                Arguments = $"--vanilla \"{ELFrScriptModelPath}\" \"{configPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        ELFResultsFilePath = args.Data;  // Capture the file path
                    }
                };

                process.ErrorDataReceived += (sender, args) => Console.WriteLine("HATA: " + args.Data);

                process.Start();
                process.BeginOutputReadLine();

                // Wait for the process to exit asynchronously
                await Task.Run(() => process.WaitForExit());

                // read the json file and create the "config" variable.
                ana_menu_form_objesi.json_file = File.ReadAllText(Path.Combine(ana_menu_form_objesi.projectRoot, "config.json"));
                ana_menu_form_objesi.config = JsonConvert.DeserializeObject(ana_menu_form_objesi.json_file);

                string results_path = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)ana_menu_form_objesi.config.İl,
                    (string)ana_menu_form_objesi.config.İlçe,
                    (string)ana_menu_form_objesi.config.ELF.SONUÇLAR_klasör,
                    (string)ana_menu_form_objesi.config.ELF.SONUÇLAR_name).Replace('/', '\\');


                if (string.IsNullOrEmpty(results_path))
                {
                    MessageBox.Show("RScript yolu hatası!.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }

                MessageBox.Show("Modeller başarıyla çalıştırıldı. ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return results_path;
            }
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


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------- GENEL HARITA VE TOOLBAR EVENTLERİ & METOTLAR -------------------------------//
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        // toolstrip'teki nokta butonu
        private void Nokta_Ekle_Click(object sender, EventArgs e)
        {
            isSelecting_marker = true;
            isSelecting_polygon = false;
        }

        // haritalardaki arazi katmanı
        private void Arazi_Click(object sender, EventArgs e) // Harita katmanları seçimi - Arazi
        {
            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;
            cbs.GetActiveGMapControl().MapProvider = GMapProviders.GoogleTerrainMap;
        }

        // haritalardaki harita katmanı
        private void Harita_Click(object sender, EventArgs e) // Harita katmanları seçimi - Harita
        {
            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;
            cbs.GetActiveGMapControl().MapProvider = GMapProviders.GoogleMap;
        }

        // haritalardaki uydu katmanı
        private void Uydu_Click(object sender, EventArgs e) // Harita katmanları seçimi - Uydu
        {
            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;
            cbs.GetActiveGMapControl().MapProvider = GMapProviders.GoogleSatelliteMap;
        }

        private void Sokak_Görünümü_Click(object sender, EventArgs e)
        {
            cbs.GetActiveGMapControl().Visible = false;
            cbs.GetActiveWebView().Visible = true;

            string url = "https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu";
            cbs.GetActiveWebView().CoreWebView2.Navigate(url);
        }

        // haritalardaki OSM katmanı
        private void OSM_Click(object sender, EventArgs e) // Harita katmanları seçimi - OpenStreetMap
        {
            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;
            cbs.GetActiveGMapControl().MapProvider = GMapProviders.OpenStreetMap;
        }

        // haritalardaki Google Earth katmanı
        private void Google_Earth_Click(object sender, EventArgs e) // Harita katmanları seçimi - Google Earth
        {

            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;

            Google_Earth google_earth_form = new Google_Earth();
            google_earth_form.Owner = this;
            google_earth_form.Show();
            google_earth_form.BringToFront();
            google_earth_form.Focus();
        }

        private void Google_Earth_Desktop_Click(object sender, EventArgs e)
        {

            cbs.GetActiveGMapControl().Visible = true;
            cbs.GetActiveWebView().Visible = false;

            string google_earth_path = @"C:\Program Files\Google\Google Earth Pro\client\googleearth.exe";

            try
            {
                // Ensure centerX and centerY are not null or empty
                if (!string.IsNullOrEmpty(centerX) && !string.IsNullOrEmpty(centerY))
                {
                    // Create the KML file with the current coordinates
                    cbs.CreateKMLFile(centerY, centerX); // Note: Latitude (Y) first, then Longitude (X)

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
                    $"Hata Mesajı: {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void Poligon_Sil_Click(object sender, EventArgs e)
        {
            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                if (polygonOverlay_imar == null || polygonOverlay_imar.Polygons.Count == 0)
                {
                    MessageBox.Show("Herhangi bir poligon çizilmemiştir. Lütfen öncelikle bir poligon çiziniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void Poligon_Kaydet_Click(object sender, EventArgs e)
        {
            if (isSelecting_YUK == true)
            {
                poligonOzellikFormu = new Poligon_Özellik_Tanımlama(true, false, polygonPoints_imar);

            }
            else if (isSelecting_YGA == true)
            {
                poligonOzellikFormu = new Poligon_Özellik_Tanımlama(false, true, polygonPoints_imar);
                poligonOzellikFormu.buton_yük_tipleri.Visible = false;
            }

            poligonOzellikFormu.Owner = this;
            poligonOzellikFormu.ShowDialog();
            poligonOzellikFormu.BringToFront();
            poligonOzellikFormu.Focus();

            if (poligonOzellikFormu.is_poligon_saved == true)
            {

                PoligonKaydetEventi(sender, e, polygonOverlay_imar, polygonPoints_imar);

                // Mark all categories for update
                pendingUpdates["imar"] = true;
                pendingUpdates["yuk"] = true;

                // Update only the active tab immediately
                UpdateCheckboxPositions(checkBoxes_imar, "imar");
                UpdateCheckboxPositions(checkBoxes_yuk, "yuk");

            }
        }

        // Helper method to bring buttons to the front
        private void BringButtonsToFront()
        {
            buton_ea_harita_katmanlar.BringToFront();
            buton_yuk_haritası_katmanlar.BringToFront();
            buton_imar_katmanlar.BringToFront();
        }

        public void OnMapClickEventi(
            PointLatLng pointClick,
            MouseEventArgs e,
            GMapOverlay markerOverlay,
            ref List<PointLatLng> polygonPoints,
            ref GMapOverlay polygonOverlay,
            System.Windows.Forms.Label mesafe,
            System.Windows.Forms.Label mesafe_metre)
        {
            if (e.Button == MouseButtons.Left)
            {
                // If user is placing markers (not polygons)
                if (isSelecting_marker)
                {
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green)
                    {
                        ToolTipText = $"Lat={Math.Round(pointClick.Lat, 5)}, Lng={Math.Round(pointClick.Lng, 5)}"
                    };
                    markerOverlay.Markers.Add(marker);
                    // Possibly store data in marker.Tag, etc.
                }

                // If user is drawing polygons
                if (isSelecting_polygon)
                {
                    // Add the newly clicked point
                    polygonPoints.Add(pointClick);

                    // Add a small marker on each click so the user sees the polygon corners
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue_small);
                    markerOverlay.Markers.Add(marker);

                    // find first free layer index
                    int layerIndex = FindFirstFreeLayerIndex();
                    if (layerIndex == -1)
                    {
                        MessageBox.Show("En fazla 15 adet katman!");
                        return;
                    }

                    // decide which map array to use, based on which gMapControl is currently active
                    GMapControl activeMap = cbs.GetActiveGMapControl();
                    GMapOverlay[] arrayForActiveMap = null;

                    if (activeMap == gMapControl_imar)
                        arrayForActiveMap = cbs.tüm_katmanlar_array_imar;
                    else if (activeMap == gMapControl_yuk)
                        arrayForActiveMap = cbs.tüm_katmanlar_array_yuk;
                    else
                    {
                        MessageBox.Show("Geçersiz harita kontrolü!");
                        return;
                    }

                    // 3) If we haven't created an overlay for this layerIndex yet, create & store it
                    if (arrayForActiveMap[layerIndex] == null)
                    {
                        // define the Id of the polygon "PolygonLayer_{layerIndex+1}"
                        string overlayName = $"PolygonLayer_{layerIndex + 1}";

                        GMapOverlay newOverlay = new GMapOverlay(overlayName);
                        activeMap.Overlays.Add(newOverlay);
                        arrayForActiveMap[layerIndex] = newOverlay;
                    }

                    // now set polygonOverlay = that array entry
                    polygonOverlay = arrayForActiveMap[layerIndex];

                    // if there are at least 3 points, let's draw or re-draw the polygon
                    if (polygonPoints.Count >= 3)
                    {
                        cbs.Draw_Polygon(polygonPoints, polygonOverlay, activeMap);

                        double area = cbs.CalculatePolygonArea(polygonPoints);
                        mesafe_metre.Visible = false;
                        mesafe.Visible = true;
                        mesafe.Text = "Seçili Alan: " + Math.Round(area, 0) + " m²";
                    }

                    activeMap.Refresh();
                }
            }
        }

        public void PoligonKaydetEventi(
            object sender,
            EventArgs e,
            GMapOverlay polygonOverlay,     // the overlay that user just drew the polygon(s) in
            List<PointLatLng> polygonPoints)
        {
            // make sure there's actually a polygon
            if (polygonOverlay == null || polygonOverlay.Polygons.Count == 0)
            {
                MessageBox.Show("Herhangi bir poligon çizilmemiştir. Önce poligon çiziniz.");
                return;
            }
            else
            {
                markerOverlay_ea.Markers?.Clear();
                markerOverlay_DEK.Markers?.Clear();
                markerOverlay_imar.Markers?.Clear();
                markerOverlay_yuk.Markers?.Clear();

                //Figure out which map array & layerIndex this overlay belongs to
                layer_index = FindLayerIndexFromOverlay(polygonOverlay);

                if (layer_index < 0)
                {
                    MessageBox.Show("Çizilen poligon geçersiz bir katmana ait!");
                    return;
                }

                // re-color the polygon
                foreach (var poly in polygonOverlay.Polygons)
                {
                    poly.Stroke = new Pen(cbs.overlayColors[layer_index].BorderColor, 3);
                    poly.Fill = new SolidBrush(cbs.overlayColors[layer_index].FillColor);
                }

                // identify the dictionary for that overlay
                Dictionary<GMapPolygon, DataRow> sourceDict = null;
                if (polygonOverlay == cbs.tüm_katmanlar_array_imar[layer_index])
                    sourceDict = cbs.polygonAttributes_imar;
                else if (polygonOverlay == cbs.tüm_katmanlar_array_yuk[layer_index])
                    sourceDict = cbs.polygonAttributes_yuk;

                else
                {
                    MessageBox.Show("Overlay dictionary eşleşmedi!");
                    return;
                }

                // create a DataTable for the layer
                DataTable polygonDataTable = poligonOzellikFormu.PolygonDataTable;
                cbs.tüm_katmanlar_datatable[layer_index] = polygonDataTable;

                // if you assume just one polygon => one row, store it in the dictionary
                DataRow singleRow = (polygonDataTable.Rows.Count > 0) ? polygonDataTable.Rows[0] : null;
                if (singleRow != null)
                {
                    foreach (var userPoly in polygonOverlay.Polygons)
                    {
                        sourceDict[userPoly] = singleRow;
                    }
                }

                //name for this layer
                string layerName = "Polygon_" + (layer_index + 1);
                cbs.tüm_katmanlar_array_names[layer_index] = layerName;

                // convert to shapefile
                MapWinGIS.Shapefile shp = cbs.ConvertOverlayToShapefile(polygonOverlay);
                cbs.shapeFileArray_MapWinGIS[layer_index] = shp;

                // copy to other overlays + dictionaries
                if (polygonOverlay != cbs.tüm_katmanlar_array_imar[layer_index])
                {
                    GMapOverlay newImar = new GMapOverlay($"PolygonLayer_{layer_index + 1}");
                    gMapControl_imar.Overlays.Add(newImar);
                    cbs.tüm_katmanlar_array_imar[layer_index] = newImar;

                    cbs.CopyOverlayContents(
                        polygonOverlay,
                        cbs.tüm_katmanlar_array_imar[layer_index],
                        sourceDict,
                        cbs.polygonAttributes_imar
                    );
                }

                if (polygonOverlay != cbs.tüm_katmanlar_array_yuk[layer_index])
                {

                    GMapOverlay newYuk = new GMapOverlay($"PolygonLayer_{layer_index + 1}");
                    gMapControl_yuk.Overlays.Add(newYuk);
                    cbs.tüm_katmanlar_array_yuk[layer_index] = newYuk;

                    cbs.CopyOverlayContents(
                        polygonOverlay,
                        cbs.tüm_katmanlar_array_yuk[layer_index],
                        sourceDict,
                        cbs.polygonAttributes_yuk
                    );
                }


                // Update the checkboxes for that layer in each 4 different map
                List<CheckBox> associatedChecks = GetCheckBoxesByIndex(layer_index);
                foreach (var chk in associatedChecks)
                {
                    chk.Text = polygonOverlay.Id;   // display layer name
                    chk.Visible = true;
                    chk.Checked = true;
                    chk.ForeColor = cbs.overlayColors[layer_index].BorderColor;
                }

                // Prepare a new overlay for future use
                polygonOverlay = null;
                polygonPoints.Clear();
                isSelecting_polygon = false;

                gMapControl_imar.Refresh();
                gMapControl_yuk.Refresh();

                mesafe_metre_imar.Text = "";
                Mesafe_imar.Text = "";

                MessageBox.Show("Poligon kaydedildi!");

                isSelecting_YGA = false;
                isSelecting_YUK = false;
            }
        }


        private void MouseDownEvent(object sender, MouseEventArgs e, GMapControl gMapControl,
            System.Windows.Forms.Label mesafe, System.Windows.Forms.Label mesafe_metre,
            List<PointLatLng> rulerPoints, GMapOverlay markerOverlay, GMapOverlay rulerOverlay,
            ref GMapRoute rulerRoute, List<PointLatLng> polygonPoints, GMapOverlay polygonOverlay)
        {
            // grid eventi
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid == true)
            {
                cbs.starting_point = gMapControl.FromLocalToLatLng(e.X, e.Y);
                cbs.ManuelGridSecimi(gMapControl);
            }

            // cetvel eventi ile mesafe çiz 
            if (e.Button == MouseButtons.Left && isRulerEnabled == true)
            {
                // seçilen piksel noktaları latitude ve longitude bilgisine dönüştür.
                var point = gMapControl.FromLocalToLatLng(e.X, e.Y);

                // bir marker objesi oluştur ve seçilen noktalara marker ata
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);
                markerOverlay.Markers.Add(marker);

                // seçilen noktaları bir listeye koy
                rulerPoints.Add(point);
                cbs.CetvelSecimi(gMapControl, mesafe_metre, rulerPoints, markerOverlay,
                    rulerOverlay, ref rulerRoute);
            }

            // sağ tıklayarak poligon çizmeyi bitir 
            if (e.Button == MouseButtons.Right && isSelecting_polygon)
            {
                markerOverlay.Markers?.Clear();
                polygonPoints?.Clear();
                polygonOverlay?.Clear();

                mesafe.Visible = false;
                mesafe_metre.Visible = false;

                cbs.GetActiveGMapControl().Refresh();
            }

            // sağ tıklayarak poligon çizmeyi bitir 
            if (e.Button == MouseButtons.Right && isRulerEnabled)
            {
                markerOverlay.Markers?.Clear();
                rulerPoints.Clear();

                mesafe.Visible = false;
                mesafe_metre.Visible = false;

                cbs.GetActiveGMapControl().Refresh();
            }

        }

        private void ModülFormu_Load(object sender, EventArgs e)
        {
            // Modül formunu yüklerken reset year selection sürecini başlat
            ResetYearSelectionProcessGirdiModulu();
        }


        public List<CheckBox> GetCheckBoxesByIndex(int index)
        {

            var imarCheckBoxes = new CheckBox[] { checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4,
                checkBox_imar_5, checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9, checkBox_imar_10,
                checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15 };

            var yukCheckBoxes = new CheckBox[] { checkBox_yuk_1, checkBox_yuk_2, checkBox_yuk_3, checkBox_yuk_4,
                checkBox_yuk_5, checkBox_yuk_6, checkBox_yuk_7, checkBox_yuk_8, checkBox_yuk_9, checkBox_yuk_10,
                checkBox_yuk_11, checkBox_yuk_12, checkBox_yuk_13, checkBox_yuk_14, checkBox_yuk_15 };

            // Ensure the index is valid before accessing arrays
            if (index >= 0 && index < 15)
            {
                // Return the checkboxes for the given index
                return new List<CheckBox> { imarCheckBoxes[index], yukCheckBoxes[index] };
            }

            // Return an empty list if index is out of range
            return new List<CheckBox>();  // Empty list instead of null
        }

        private void ShowAttributeTable(DataTable datatable)
        {
            // Create a copy of the DataTable to avoid modifying the original
            DataTable displayTable = datatable.Copy();

            // Add a RowIndex column to map back to the polygons
            if (!displayTable.Columns.Contains("RowIndex"))
            {
                displayTable.Columns.Add("RowIndex", typeof(int));
            }

            // Populate the RowIndex column
            for (int i = 0; i < displayTable.Rows.Count; i++)
            {
                displayTable.Rows[i]["RowIndex"] = i;
            }

            // Bind the DataTable to the DataGridView
            tablo_formu.attribute_table.DataSource = displayTable;

            // Hide the RowIndex column
            if (tablo_formu.attribute_table.Columns["RowIndex"] != null)
            {
                tablo_formu.attribute_table.Columns["RowIndex"].Visible = false;
            }

        }

        // temizle, rengini degistir vs. contextmenuitemları için associated checkbox taglerini ekleme
        // ilgili tabloyu getirme
        private void checkBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Cast the sender to a CheckBox
            System.Windows.Forms.CheckBox sender_checkbox = sender as System.Windows.Forms.CheckBox;

            // Safely parse the Tag property to an integer
            int checkbox_index;
            if (!Int32.TryParse(sender_checkbox.Tag?.ToString(), out checkbox_index))
            {
                return;
            }

            // Adjust the index since the tags are from 1 to 15 but the checkbox_indexes in the arrays are 0 to 14
            checkbox_index -= 1;
            lastSelectedCheckboxIndex = checkbox_index; // Store the index

            // Update the last clicked checkbox (style reset)
            if (lastClickedCheckbox != null)
            {
                // Reset the previous checkbox style to normal
                lastClickedCheckbox.Font = new Font(lastClickedCheckbox.Font, FontStyle.Regular);
            }

            // Update the last clicked checkbox reference
            lastClickedCheckbox = sender_checkbox;

            // Set the Tag property for context menu items
            if (temizleToolStripMenuItem != null)
            {
                temizleToolStripMenuItem.Tag = sender_checkbox;
            }
            if (rengiDeğiştirToolStripMenuItem != null)
            {
                rengiDeğiştirToolStripMenuItem.Tag = sender_checkbox;
            }
            if (kaydetToolStripMenuItem != null)
            {
                kaydetToolStripMenuItem.Tag = sender_checkbox;
            }
            if (yenidenAdlandırToolStripMenuItem != null)
            {
                yenidenAdlandırToolStripMenuItem.Tag = sender_checkbox;
            }

            // Check if the layer at the checkbox index exists
            if (checkbox_index >= 0 && checkbox_index < cbs.tüm_katmanlar_array_imar.Length && cbs.tüm_katmanlar_array_imar[checkbox_index] != null)
            {
                // Update the table form text
                var layerName = cbs.tüm_katmanlar_array_names[checkbox_index];
                var dataTable = cbs.tüm_katmanlar_datatable[checkbox_index];
                tablo_formu.Text = $"Veri Tablosu -- {layerName} -- {dataTable.Rows.Count} satır -- {dataTable.Columns.Count} sütun";

                // Show the attribute table
                ShowAttributeTable(dataTable);
            }
        }

        public void ZoomToFeature(int rowIndex)
        {
            if (lastSelectedCheckboxIndex < 0 ||
                lastSelectedCheckboxIndex >= cbs.tüm_katmanlar_array_imar.Length ||
                lastSelectedCheckboxIndex >= cbs.tüm_katmanlar_array_yuk.Length) return;

            // Get the overlays associated with the last selected checkbox
            GMapOverlay imarOverlay = cbs.tüm_katmanlar_array_imar[lastSelectedCheckboxIndex];
            GMapOverlay yukOverlay = cbs.tüm_katmanlar_array_yuk[lastSelectedCheckboxIndex];

            if (imarOverlay == null && yukOverlay == null) return; // If both overlays are null, exit

            // Get the polygons at the specified rowIndex from both overlays
            GMapPolygon imarPolygon = imarOverlay?.Polygons.ElementAtOrDefault(rowIndex);
            GMapPolygon yukPolygon = yukOverlay?.Polygons.ElementAtOrDefault(rowIndex);

            if (imarPolygon == null && yukPolygon == null) return; // If no polygons are found, exit


            // Calculate the bounding box (use either polygon, assuming they represent the same feature)
            GMapPolygon targetPolygon = imarPolygon ?? yukPolygon; // Use imarPolygon if available, otherwise yukPolygon
            if (targetPolygon == null) return;

            double minLat = double.MaxValue, maxLat = double.MinValue;
            double minLng = double.MaxValue, maxLng = double.MinValue;

            foreach (var point in targetPolygon.Points)
            {
                minLat = Math.Min(minLat, point.Lat);
                maxLat = Math.Max(maxLat, point.Lat);
                minLng = Math.Min(minLng, point.Lng);
                maxLng = Math.Max(maxLng, point.Lng);
            }

            // Add some padding to the bounding box
            double latPadding = (maxLat - minLat) * 0.1;
            double lngPadding = (maxLng - minLng) * 0.1;
            minLat -= latPadding;
            maxLat += latPadding;
            minLng -= lngPadding;
            maxLng += lngPadding;

            // Create the bounding box
            GMap.NET.RectLatLng bounds = new GMap.NET.RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);

            // Highlight and zoom in gMapControl_imar if the polygon exists
            if (imarPolygon != null)
            {
                highlightedPolygon = imarPolygon;
                imarPolygon.Stroke = new Pen(Color.Yellow, 3); // Highlight with a yellow border
                gMapControl_imar.SetZoomToFitRect(bounds);
                gMapControl_imar.Refresh();
            }

            // Highlight and zoom in gMapControl_yuk if the polygon exists
            if (yukPolygon != null)
            {
                highlightedPolygon = yukPolygon; // Update highlightedPolygon to the yukPolygon if it exists
                yukPolygon.Stroke = new Pen(Color.Yellow, 3); // Highlight with a yellow border
                gMapControl_yuk.SetZoomToFitRect(bounds);
                gMapControl_yuk.Refresh();
            }
        }

        // Helper method to reset map controls for a specific map
        private void ResetMapControls(GMapControl mapControl, System.Windows.Forms.Label distanceLabel, System.Windows.Forms.Label distanceMetreLabel,
            GMapOverlay markerOverlay, GMapOverlay rulerOverlay, GMapRoute rulerRoute, List<PointLatLng> rulerPoints)
        {
            // Clear the distance display and visibility
            distanceMetreLabel.Text = string.Empty;
            distanceLabel.Visible = false;

            // Clear overlays and ruler data
            markerOverlay?.Clear();
            rulerOverlay?.Clear();
            rulerRoute?.Clear();
            rulerPoints?.Clear();
        }

        private void yenidenAdlandırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem yeniden_adlandir = sender as ToolStripMenuItem;

            if (yeniden_adlandir != null)
            {
                System.Windows.Forms.CheckBox checkBox = yeniden_adlandir.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (cbs.tüm_katmanlar_array_imar[checkbox_index] != null)
                {
                    // Prompt the user to input a new name
                    string newName = Prompt.ShowDialog("Yeni isim:", "Katmanı Yeniden Adlandır");

                    if (!string.IsNullOrEmpty(newName))
                    {
                        // Rename the layer in your underlying data structure
                        checkBoxes_imar[checkbox_index].Text = newName;
                        checkBoxes_yuk[checkbox_index].Text = newName;
                        cbs.tüm_katmanlar_array_names[checkbox_index] = newName;

                        // Refresh the list/tree view
                        checkBox.Refresh();
                    }
                }
            }
        }

        private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem kaydet_menu_item)
            {
                System.Windows.Forms.CheckBox checkBox = kaydet_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                SaveFileDialog kaydet_file_dialog = new SaveFileDialog();

                kaydet_file_dialog.Filter = "Shapefile |*.shp|MapInfo File|*.tab|Google Earth File|*.kml";
                kaydet_file_dialog.InitialDirectory = cbs.targetDirectory;

                DialogResult kaydet_result = kaydet_file_dialog.ShowDialog();

                if (kaydet_result == DialogResult.OK)
                {
                    string filepath = kaydet_file_dialog.FileName;
                    string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                    string extension = filename.Substring(filename.Length - 3);

                    if (extension == "shp")
                    {
                        MapWinGIS.Shapefile shapefile = cbs.shapeFileArray_MapWinGIS[checkbox_index];

                        int fieldIndex;

                        fieldIndex = shapefile.get_FieldIndexByName("MWShapeID");

                        if (fieldIndex != -1)
                        {
                            shapefile.EditDeleteField(fieldIndex);
                        }

                        shapefile.SaveAsEx(filepath, false, false);
                        shapefile.Close();
                        cbs.shapeFileArray_MapWinGIS[checkbox_index] = null;
                        MessageBox.Show("Dosya başarıyla kaydedildi.");
                    }
                    else if (extension == "kml")
                    {
                        GMapOverlay kmlOverlay = new GMapOverlay();
                        kmlOverlay = cbs.tüm_katmanlar_array_imar[checkbox_index];

                        cbs.ExportOverlayToKml(kmlOverlay, filepath);
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

                // Ensure the Tag is set and is a valid number
                if (checkBox != null && checkBox.Tag != null)
                {
                    int checkbox_index;

                    if (int.TryParse(checkBox.Tag.ToString(), out checkbox_index))
                    {
                        checkbox_index -= 1;  // Adjust for 0-based indexing

                        // Ensure the index is within bounds of the array and the item exists
                        if (checkbox_index >= 0 && checkbox_index < cbs.tüm_katmanlar_array_imar.Length &&
                            cbs.tüm_katmanlar_array_imar[checkbox_index] != null)
                        {
                            string katman_ismi = cbs.tüm_katmanlar_array_names[checkbox_index];

                            DialogResult temizle_result = MessageBox.Show(katman_ismi + " isimli katman " +
                                "silinecektir. Emin misiniz?", "", MessageBoxButtons.YesNo);

                            if (temizle_result == DialogResult.Yes)
                            {
                                // Safe removal from overlays
                                if (cbs.tüm_katmanlar_array_imar[checkbox_index] != null)
                                {
                                    // Check if the overlay exists and remove it safely
                                    var overlay_imar = cbs.tüm_katmanlar_array_imar[checkbox_index];
                                    var overlay_yuk = cbs.tüm_katmanlar_array_yuk[checkbox_index];

                                    var activeMap = cbs.GetActiveGMapControl();

                                    if (activeMap.Overlays.Contains(overlay_imar) || activeMap.Overlays.Contains(overlay_yuk))
                                    {
                                        gMapControl_imar.Overlays.Remove(overlay_imar);
                                        gMapControl_yuk.Overlays.Remove(overlay_yuk);

                                        gMapControl_imar.Refresh();
                                        gMapControl_yuk.Refresh();
                                    }

                                    if (checkBox.Text == "SONUCLAR_Load_Density.kml")
                                    {
                                        checkBox_yuk_main.Checked = false;
                                        checkBox_yuk_main.Visible = false;
                                        checkBox_yuk_main.Tag = null;

                                        buton_HTML.Visible = false;

                                        load_density_cnt = 0;

                                        // Remove the heatmapOverlay by its ID
                                        GMapOverlay heatmapOverlayToRemove = gMapControl_yuk.Overlays.FirstOrDefault(o => o.Id == "HeatmapOverlay");
                                        if (heatmapOverlayToRemove != null)
                                        {
                                            gMapControl_yuk.Overlays.Remove(heatmapOverlayToRemove);
                                        }
                                    }
                                }

                                // Dispose and nullify references
                                cbs.tüm_katmanlar_array_imar[checkbox_index]?.Dispose();
                                cbs.tüm_katmanlar_array_yuk[checkbox_index]?.Dispose();

                                cbs.tüm_katmanlar_array_imar[checkbox_index] = null;
                                cbs.tüm_katmanlar_array_yuk[checkbox_index] = null;

                                cbs.tüm_katmanlar_datatable[checkbox_index] = null;
                                cbs.tüm_katmanlar_array_names[checkbox_index] = null;

                                // Clear checkboxes for all maps
                                ClearCheckboxesForAllMaps(checkbox_index);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Silinecek katman bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Silinecek katman sorunu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SetOverlayColor(GMapOverlay overlay, Color selectedColor)
        {
            if (overlay == null) return;

            // For polygons (area geometry)
            foreach (var polygon in overlay.Polygons)
            {
                // Set outline (stroke)
                polygon.Stroke = new Pen(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B), 3);
                polygon.Fill = new SolidBrush(Color.FromArgb(50, selectedColor));
            }

            // For routes (line geometry)
            foreach (var route in overlay.Routes)
            {
                route.Stroke = new Pen(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B), 3);
            }

        }

        private void rengiDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the "Change Color" menu item
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            // get the associated checkbox from the menu item’s Tag
            CheckBox checkBox = menuItem.Tag as CheckBox;
            if (checkBox == null) return;

            // Parse the layer index from the checkbox’s Tag to get the 0-based index
            if (!int.TryParse(checkBox.Tag?.ToString(), out int zeroBasedIndex))
            {
                MessageBox.Show("Katman endeksi çözümlenemedi.");
                return;
            }

            int layerIndex = zeroBasedIndex - 1;

            // retrieve the four overlay arrays 
            GMapOverlay overlayImar = cbs.tüm_katmanlar_array_imar[layerIndex];
            GMapOverlay overlayYuk = cbs.tüm_katmanlar_array_yuk[layerIndex];

            // prompt user for a color
            using (ColorDialog colorDialog = new ColorDialog
            {
                AnyColor = true,
                AllowFullOpen = true,
                FullOpen = true
            })
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;

                    // aApply color to each overlay
                    SetOverlayColor(overlayImar, selectedColor);
                    SetOverlayColor(overlayYuk, selectedColor);

                    // Recolor all three checkboxes associated with this layer index
                    checkBoxes_imar[layerIndex].ForeColor = selectedColor;
                    checkBoxes_yuk[layerIndex].ForeColor = selectedColor;

                    // refresh each map
                    gMapControl_imar.Refresh();
                    gMapControl_yuk.Refresh();
                }
            }
        }

        private void tabloyuGörToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tablo_formu.Show();
            tablo_formu.Activate();
        }


        // open up the Fonksiyonlar formu and populate its comboboxes with the specified array values
        private void katman_birleştir_Click(object sender, EventArgs e)
        {
            if (cbs.tüm_katmanlar_array_names[0] != null && cbs.tüm_katmanlar_array_names[1] != null)
            {
                fonksiyonFormu = new Fonksiyon_Oluştur()
                {
                    Tag = this,
                    Owner = this
                };

                foreach (string layers in cbs.tüm_katmanlar_array_names)
                {
                    if (layers != null)
                    {
                        fonksiyonFormu.comboBox_fonksiyonlar_1.Items.Add(layers);
                        fonksiyonFormu.comboBox_fonksiyonlar_2.Items.Add(layers);
                    }
                }
                fonksiyonFormu.comboBox_fonksiyonlar_1.Text = cbs.tüm_katmanlar_array_names[0];
                fonksiyonFormu.comboBox_fonksiyonlar_2.Text = cbs.tüm_katmanlar_array_names[1];

                // create an example row so that the columns of the second table could be displayed
                // in the list box
                DataRow example_row = cbs.tüm_katmanlar_datatable[1].NewRow();

                foreach (var columns in example_row.Table.Columns)
                {
                    SuspendLayout();
                    fonksiyonFormu.tum_sutunlar_fonksiyonForm.Items.Add(columns.ToString());
                    ResumeLayout();
                }

                fonksiyonFormu.activeGMapControl = cbs.GetActiveGMapControl();

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

        private void katmanlar_right_click_Opening(object sender, CancelEventArgs e)
        {
            // Get the context menu strip that is being opened
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;

            // Get the checkbox associated with the context menu strip and set its font to bold
            System.Windows.Forms.CheckBox clickedCheckBox = (System.Windows.Forms.CheckBox)contextMenuStrip.SourceControl;
            clickedCheckBox.Font = new Font(clickedCheckBox.Font, System.Drawing.FontStyle.Underline | FontStyle.Italic);

        }

        private int FindFirstFreeLayerIndex()
        {
            for (int i = 0; i < 13; i++)  // or whatever max size
            {
                if (cbs.tüm_katmanlar_datatable[i] == null)
                {
                    return i;
                }
            }
            return -1; // no free slot
        }

        // find the first open slot within the arrays
        public int FindLayerIndexFromOverlay(GMapOverlay overlay)
        {
            // Check each array for a match
            for (int i = 0; i < 15; i++)
            {
                if (cbs.tüm_katmanlar_array_imar[i] == overlay) return i;
                if (cbs.tüm_katmanlar_array_yuk[i] == overlay) return i;
            }
            return -1; // not found
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


        private void NoktaBilgileriniGoster(NoktaVeri nokta)
        {
            // Nokta bilgilerini göster
            MessageBox.Show($"Enlem: {nokta.Enlem}\nBoylam: {nokta.Boylam}\nBina Demandi: " +
                $"{nokta.Bina_Demandi}\nAbone Sayısı: {nokta.Abone_Sayısı}");

            // Noktayı silmek için enlem ve boylamdan PointLatLng oluşturuyoruz
            PointLatLng point = new PointLatLng(nokta.Enlem, nokta.Boylam);
        }

        private void buton_database_giris_Click(object sender, EventArgs e)
        {
            try
            {
                // Yıl kontrolü
                if (slfStartYear == 0 || slfEndYear == 0)
                {
                    MessageBox.Show("Lütfen başlangıç ve bitiş yıllarını belirleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // YearService'i başlangıçta güncelle - bu her zaman çalışacak
                var yearService = YearService.GetInstance();
                yearService.SetYears(slfStartYear, slfEndYear);

                // Veri tipi kontrolü
                if (veri_listesi_seçimi.SelectedItem == null)
                {
                    MessageBox.Show("Lütfen önce bir veri tipi seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();

                // girdiModülleri Dictionary'si kontrolü ve girdiModülü atama
                if (!girdiModülleri.ContainsKey(seçilenVeriTipi))
                {
                    MessageBox.Show($"'{seçilenVeriTipi}' için uygun bir modül bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Güvenli erişim ve girdiModülü yıl değerlerini atama
                girdiModülü = girdiModülleri[seçilenVeriTipi];

                // Veritabanı işlemleri
                if (DatabaseManager.GetInstance().IsConnected())
                {
                    try
                    {
                        using (var databaseListForm = new DatabaseListForm())
                        {
                            databaseListForm.Owner = this;
                            databaseListForm.FormClosed += (s, args) =>
                            {
                                // Form kapandığında gerekli güncellemeleri yap
                                if (dataGridView_girdi.DataSource != null)
                                {
                                    dataGridView_girdi.Refresh();
                                }
                            };
                            databaseListForm.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Veritabanı listesi gösterilirken hata oluştu: {ex.Message}",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            using (var databaseListForm = new DatabaseListForm())
                            {
                                databaseListForm.Owner = this;
                                databaseListForm.ShowDialog();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında beklenmeyen bir hata oluştu: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buton_tablo_olustur_Click(object sender, EventArgs e)
        {
            Tablo_olustur tablo_olustur_formu = new Tablo_olustur();
            tablo_olustur_formu.Show();
        }

        private void YGA_Ekle_Click(object sender, EventArgs e)
        {
            isSelecting_polygon = true;
            isSelecting_YGA = true;

            isRulerEnabled = false;
            isRulerActive = false;

            // Determine the active map control and reset accordingly
            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                ResetMapControls(gMapControl_imar, mesafe_metre_imar, Mesafe_imar, markerOverlay_imar, rulerOverlay_imar, rulerRoute_imar, rulerPoints_imar);
            }
        }

        private void Point_Load_Ekle_Click(object sender, EventArgs e)
        {
            isSelecting_polygon = true;
            isSelecting_YUK = true;

            isRulerEnabled = false;
            isRulerActive = false;

            // Determine the active map control and reset accordingly
            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                ResetMapControls(gMapControl_imar, mesafe_metre_imar, Mesafe_imar, markerOverlay_imar, rulerOverlay_imar, rulerRoute_imar, rulerPoints_imar);
            }
        }

        private void buton_DL_calıstır_Click(object sender, EventArgs e)
        {
            try
            {
                // İşlem sırasında imleç görünümünü değiştir
                Cursor.Current = Cursors.WaitCursor;

                // Gerekli kontroller (Abone verisi yüklü mü, il-ilçe seçilmiş mi)
                if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
                {
                    MessageBox.Show("Lütfen önce il ve ilçe seçimini yapın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!GirdiModülü.dataTablesByType.ContainsKey("Abone Verileri"))
                {
                    MessageBox.Show("Lütfen önce Abone Verileri'ni yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Deep Learning modelini çalıştır
                string result = PythonHelper.RunDeepLearningModel();

                // İşlem tamamlandığında başarı mesajı göster
                MessageBox.Show("İmar analizi başarıyla tamamlandı.\nSonuçlar 'imar_analizi_sonuclari/deep_learning_modeli' klasöründe kaydedildi.",
                                "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // İsteğe bağlı olarak sonuç klasörünü aç
                string imarAnaliziPath = PathService.GetImarAnaliziPathForType("deep_learning_modeli");
                System.Diagnostics.Process.Start("explorer.exe", imarAnaliziPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // İşlem bittiğinde imleci normal duruma getir
                Cursor.Current = Cursors.Default;
            }
        }

        private void gMapControl_yuk_MouseMove(object sender, MouseEventArgs e)
        {
            // Convert mouse coordinates to geographical coordinates
            PointLatLng mousePos = gMapControl_yuk.FromLocalToLatLng(e.X, e.Y);

            // Find the heatmap overlay
            GMapOverlay heatmapOverlay = gMapControl_yuk.Overlays.FirstOrDefault(o => o.Id == "HeatmapOverlay");
            if (heatmapOverlay == null) return;

            // Check if the mouse is inside any polygon in the heatmap overlay
            GMapPolygon newHoveredPolygon = null;
            foreach (GMapPolygon polygon in heatmapOverlay.Polygons)
            {
                if (cbs.IsPointInPolygon(mousePos, polygon))
                {
                    newHoveredPolygon = polygon;
                    break;
                }
            }

            // If the hovered polygon has changed, update the tooltip
            if (newHoveredPolygon != hoveredPolygon)
            {
                hoveredPolygon = newHoveredPolygon;
                if (hoveredPolygon != null && checkBox_yuk_main.Checked == true)
                {
                    UpdatePolygonToolTip(hoveredPolygon);
                    polygonToolTip.Show(GetToolTipText(hoveredPolygon), gMapControl_yuk, e.X + 15, e.Y + 15);
                }
                else
                {
                    polygonToolTip.Hide(gMapControl_yuk);
                }
            }
        }

        private string GetToolTipText(GMapPolygon polygon)
        {
            if (polygon == null || heatmapPolygonAttributes == null || !heatmapPolygonAttributes.TryGetValue(polygon, out DataRow attributes))
            {
                return "Herhangi bir veri bulunamadı.!";
            }

            // Define the columns to display
            string[] columns = new string[]
            {
                $"Mesken_{currentYear}",
                $"Sanayi_{currentYear}",
                $"Ticarethane_{currentYear}",
                $"Tarımsal Sulama_{currentYear}",
                $"Aydınlatma_{currentYear}",
                $"TOPLAM_YÜK_{currentYear}",
                $"Hücre İçi Yerleşim Alanı_{currentYear}",
                $"Yük_Yoğunluğu_{currentYear}"
            };

            // Build the tooltip text
            StringBuilder tooltipText = new StringBuilder();

            foreach (string column in columns)
            {
                if (attributes.Table.Columns.Contains(column))
                {
                    string value = attributes[column]?.ToString() ?? "N/A";
                    tooltipText.AppendLine($"{column}: {value}");
                }
                else
                {
                    tooltipText.AppendLine($"{column}: N/A");
                }
            }

            return tooltipText.ToString();
        }

        private void UpdatePolygonToolTip(GMapPolygon polygon)
        {
            if (polygon == null) return;
            string tooltipText = GetToolTipText(polygon);
            polygonToolTip.SetToolTip(gMapControl_yuk, tooltipText);
        }

        private void gMapControl_yuk_MouseLeave(object sender, EventArgs e)
        {
            // Hide the tooltip and clear the hovered polygon when the mouse leaves the map
            hoveredPolygon = null;
            polygonToolTip.Hide(gMapControl_yuk);
        }


        // Show just the single row whenever a polygon is clicked on which corresponds to its row
        private void ShowAttributeRow(DataRow row)
        {
            // Clone the structure of the original table and import the specific row
            DataTable singleRowTable = row.Table.Clone();
            singleRowTable.ImportRow(row);

            // Show the attribute table with just one row
            ShowAttributeTable(singleRowTable);

        }

        private GMapOverlay dekOverlay;

        // Add this event handler for the checkbox
        private void DEKPointsLayerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (dekOverlay == null) return;

            if (DEKPointsLayerCheckBox.Checked)
            {
                if (!gMapControl_DEK.Overlays.Contains(dekOverlay))
                {
                    gMapControl_DEK.Overlays.Add(dekOverlay);
                }
            }
            else
            {
                if (gMapControl_DEK.Overlays.Contains(dekOverlay))
                {
                    gMapControl_DEK.Overlays.Remove(dekOverlay);
                }
            }
            gMapControl_DEK.Refresh();
        }

        private void ilceSecimiDEK(object sender, EventArgs e)
        {
            if (comboBox_dek_ilce_secimi.SelectedItem != null)
            {
                SelectedDistrict = comboBox_dek_ilce_secimi.SelectedItem.ToString();
                Console.WriteLine($"Selected District: {SelectedDistrict}");
            }
            else
            {
                SelectedDistrict = null;
                Console.WriteLine("District selection cleared.");
            }
            // CheckSelections();
        }

        private async void DEKRunSimulationButton_Click(object sender, EventArgs e)
        {
            // Disable buttons and TrackBar to prevent interaction while processing
            DEKRunSimulationButton.Enabled = false;
            DEKSimulasyonSonucGoruntule.Enabled = false;
            //DEKSimButton.Enabled = false;

            try
            {
                Cursor = Cursors.WaitCursor;
                if (DEKStatusLabel != null)
                {
                    DEKStatusLabel.Text = "Python script started. This may take a while. Please wait...";
                    DEKStatusLabel.Visible = true;
                }
                if (DEKProgressBar != null)
                {
                    DEKProgressBar.Style = ProgressBarStyle.Marquee;
                    DEKProgressBar.Visible = true;
                }

                string inputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v1\DELTA_EA_DENEME_IMAR.xlsx";
                string outputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek_distribution_2024_2030_İzmir_düşük.xlsx";

                if (!File.Exists(inputFilePath))
                {
                    MessageBox.Show("Input file not found! Please ensure the file is saved correctly.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await RunPythonDEKScriptAsync(inputFilePath);

                if (!File.Exists(outputFilePath))
                {
                    MessageBox.Show("Output file not generated! Please check the Python script.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Call SimilasyonSonucGoruntule to handle display
                //SimilasyonSonucGoruntule();

                // Add info message box to inform user of completion
                MessageBox.Show("Simulation process completed successfully!",
                    "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                if (DEKProgressBar != null)
                    DEKProgressBar.Visible = false;
                if (DEKStatusLabel != null)
                    DEKStatusLabel.Text = "Simulation process completed";

                DEKRunSimulationButton.Enabled = true;
                DEKSimulasyonSonucGoruntule.Enabled = true;
                //DEKSimButton.Enabled = true;
            }
        }

        // Updated RunPythonScriptAsync to match your paths
        private async Task RunPythonDEKScriptAsync(string inputFilePath)
        {
            try
            {
                string pythonScriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek.py";
                string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonExePath,
                    Arguments = $"\"{pythonScriptPath}\" \"{inputFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    await Task.Run(() => process.WaitForExit());

                    string output = await outputTask;
                    string error = await errorTask;

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python script failed with exit code {process.ExitCode}.\nError: {error}");
                    }
                    else if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine($"Python output: {output}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error running Python script: {ex.Message}");
            }
        }

        private async void buton_HTML_Click(object sender, EventArgs e)
        {
            await ExportHeatmapToHtml();
        }


        private async Task ExportHeatmapToHtml()
        {
            // Find the heatmap overlay
            GMapOverlay heatmapOverlay = gMapControl_yuk.Overlays.FirstOrDefault(o => o.Id == "HeatmapOverlay");
            if (heatmapOverlay == null || heatmapOverlay.Polygons.Count == 0)
            {
                MessageBox.Show("Herhangi bir yük yoğunluğu haritası bulunamadı.");
                return;
            }

            // Define the columns to include in the tooltip
            string[] tooltipColumns = new string[]
            {
                $"Mesken_{currentYear}",
                $"Sanayi_{currentYear}",
                $"Ticarethane_{currentYear}",
                $"Tarımsal Sulama_{currentYear}",
                $"Aydınlatma_{currentYear}",
                $"TOPLAM_YÜK_{currentYear}",
                $"Hücre İçi Yerleşim Alanı_{currentYear}",
                $"Yük_Yoğunluğu_{currentYear}"
            };

            // Define the range of years to include (adjust as needed)
            int minYear = 2025; // Adjust based on your data
            int maxYear = 2050; // Adjust based on your data
            List<string> loadDensityColumns = new List<string>();
            for (int year = minYear; year <= maxYear; year++)
            {
                loadDensityColumns.Add($"Yük_Yoğunluğu_{year}");
            }

            // Open a SaveFileDialog to let the user choose where to save the HTML file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "HTML File|*.html",
                Title = "Haritayı HTML Olarak Kaydet",
                FileName = $"Yük_Yoğunluğu_Haritası_{currentYear}.html",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                this.Cursor = Cursors.Hand;
                return;
            }

            string filePath = saveFileDialog.FileName;

            // Generate GeoJSON for the polygons
            StringBuilder geoJson = new StringBuilder();
            geoJson.AppendLine("{");
            geoJson.AppendLine("  \"type\": \"FeatureCollection\",");
            geoJson.AppendLine("  \"features\": [");

            bool firstFeature = true;
            foreach (GMapPolygon polygon in heatmapOverlay.Polygons)
            {
                if (!firstFeature) geoJson.AppendLine(",");
                firstFeature = false;

                geoJson.AppendLine("    {");
                geoJson.AppendLine("      \"type\": \"Feature\",");
                geoJson.AppendLine("      \"geometry\": {");
                geoJson.AppendLine("        \"type\": \"Polygon\",");
                geoJson.AppendLine("        \"coordinates\": [");

                // Add the polygon coordinates
                geoJson.Append("          [");
                bool firstPoint = true;
                foreach (var point in polygon.Points)
                {
                    if (!firstPoint) geoJson.Append(",");
                    firstPoint = false;
                    geoJson.Append($"[{point.Lng},{point.Lat}]");
                }
                // Close the polygon by repeating the first point
                geoJson.Append($",[{polygon.Points[0].Lng},{polygon.Points[0].Lat}]");
                geoJson.AppendLine("]");

                geoJson.AppendLine("        ]");
                geoJson.AppendLine("      },");

                // Add properties for all columns
                geoJson.AppendLine("      \"properties\": {");
                bool firstProperty = true;

                // Add loadDensity for all years
                Dictionary<string, double> loadDensities = new Dictionary<string, double>();
                if (heatmapPolygonAttributes.TryGetValue(polygon, out DataRow attributes))
                {
                    foreach (string column in loadDensityColumns)
                    {
                        double loadDensity = 0.0;
                        if (attributes.Table.Columns.Contains(column))
                        {
                            string rawValue = attributes[column].ToString();
                            if (!string.IsNullOrWhiteSpace(rawValue) &&
                                double.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                            {
                                loadDensity = value;
                            }
                        }
                        loadDensities[column] = loadDensity;

                        if (!firstProperty) geoJson.AppendLine(",");
                        firstProperty = false;
                        geoJson.Append($"        \"{column}\": {loadDensity}");
                    }

                    // Add tooltip columns (for the current year)
                    foreach (string column in tooltipColumns)
                    {
                        geoJson.AppendLine(",");
                        string value = "N/A";
                        if (attributes.Table.Columns.Contains(column))
                        {
                            value = attributes[column]?.ToString() ?? "N/A";
                            // Escape quotes in the value to ensure valid JSON
                            value = value.Replace("\"", "\\\"");
                        }
                        geoJson.Append($"        \"{column}\": \"{value}\"");
                    }
                }
                else
                {
                    // If no attributes, set default values
                    foreach (string column in loadDensityColumns)
                    {
                        if (!firstProperty) geoJson.AppendLine(",");
                        firstProperty = false;
                        geoJson.Append($"        \"{column}\": 0.0");
                    }
                    foreach (string column in tooltipColumns)
                    {
                        geoJson.AppendLine(",");
                        geoJson.Append($"        \"{column}\": \"N/A\"");
                    }
                }

                geoJson.AppendLine();
                geoJson.AppendLine("      }");
                geoJson.Append("    }");
            }

            geoJson.AppendLine();
            geoJson.AppendLine("  ]");
            geoJson.AppendLine("}");

            // Calculate the center of the map (average of all polygon coordinates)
            double avgLat = 0, avgLng = 0;
            int pointCount = 0;
            foreach (GMapPolygon polygon in heatmapOverlay.Polygons)
            {
                foreach (var point in polygon.Points)
                {
                    avgLat += point.Lat;
                    avgLng += point.Lng;
                    pointCount++;
                }
            }
            if (pointCount > 0)
            {
                avgLat /= pointCount;
                avgLng /= pointCount;
            }

            // Generate the HTML content
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("  <title>Heatmap Dashboard</title>");
            htmlContent.AppendLine("  <link rel=\"stylesheet\" href=\"https://unpkg.com/leaflet@1.9.4/dist/leaflet.css\" />");
            htmlContent.AppendLine("  <script src=\"https://unpkg.com/leaflet@1.9.4/dist/leaflet.js\"></script>");
            htmlContent.AppendLine("  <style>");
            htmlContent.AppendLine("    body { margin: 0; font-family: Arial, sans-serif; }");
            // Position the dashboard at the bottom-left corner
            htmlContent.AppendLine("    #dashboard { position: absolute; bottom: 10px; left: 10px; z-index: 1000; background: white; padding: 10px; border-radius: 5px; box-shadow: 0 0 5px rgba(0,0,0,0.3); }");
            htmlContent.AppendLine("    #map { height: 100vh; width: 100%; }");
            htmlContent.AppendLine("    .leaflet-tooltip { white-space: pre-line; }");
            htmlContent.AppendLine("    #year-label { font-size: 16px; margin-bottom: 5px; }");
            htmlContent.AppendLine("    input[type=range] { width: 200px; }");
            htmlContent.AppendLine("  </style>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");
            htmlContent.AppendLine("  <div id=\"dashboard\">");
            htmlContent.AppendLine("    <div id=\"year-label\">Year: " + currentYear + "</div>");
            htmlContent.AppendLine($"    <input type=\"range\" id=\"year-slider\" min=\"{minYear}\" max=\"{maxYear}\" value=\"{currentYear}\" step=\"1\">");
            htmlContent.AppendLine("  </div>");
            htmlContent.AppendLine("  <div id=\"map\"></div>");
            htmlContent.AppendLine("  <script>");

            // Initialize the map
            htmlContent.AppendLine($"    var map = L.map('map').setView([{avgLat}, {avgLng}], 13);");
            htmlContent.AppendLine("    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {");
            htmlContent.AppendLine("      attribution: '© <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'");
            htmlContent.AppendLine("    }).addTo(map);");

            // Add the GeoJSON data
            htmlContent.AppendLine("    var geojsonData = " + geoJson.ToString() + ";");
            htmlContent.AppendLine("    var geojsonLayer;");

            // Function to get the color based on load density
            htmlContent.AppendLine("    function getColor(loadDensity) {");
            htmlContent.AppendLine("      var brackets = [0, 3, 5, 10, 25, 50, 75, 100, 200, 400, Infinity];");
            htmlContent.AppendLine("      var bracketIndex = -1;");
            htmlContent.AppendLine("      for (var i = 0; i < brackets.length - 1; i++) {");
            htmlContent.AppendLine("        if (loadDensity >= brackets[i] && loadDensity < brackets[i + 1]) {");
            htmlContent.AppendLine("          bracketIndex = i;");
            htmlContent.AppendLine("          break;");
            htmlContent.AppendLine("        }");
            htmlContent.AppendLine("      }");
            htmlContent.AppendLine("      var normalized = bracketIndex / (brackets.length - 2);");
            htmlContent.AppendLine("      if (normalized <= 0.5) {");
            htmlContent.AppendLine("        var t = normalized / 0.5;");
            htmlContent.AppendLine("        var r = Math.round(t * 255);");
            htmlContent.AppendLine("        var g = Math.round(t * 255);");
            htmlContent.AppendLine("        var b = Math.round((1 - t) * 255);");
            htmlContent.AppendLine("      } else {");
            htmlContent.AppendLine("        var t = (normalized - 0.5) / 0.5;");
            htmlContent.AppendLine("        var r = 255;");
            htmlContent.AppendLine("        var g = Math.round((1 - t) * 255);");
            htmlContent.AppendLine("        var b = 0;");
            htmlContent.AppendLine("      }");
            htmlContent.AppendLine("      return `rgb(${r}, ${g}, ${b})`;");
            htmlContent.AppendLine("    }");

            // Function to generate tooltip text
            htmlContent.AppendLine("    function getTooltipText(feature) {");
            htmlContent.AppendLine("      var props = feature.properties;");
            htmlContent.AppendLine("      var tooltipText = '';");

            foreach (string column in tooltipColumns)
            {
                htmlContent.AppendLine($"      tooltipText += '{column}: ' + (props['{column}'] || 'N/A') + '\\n';");
            }

            htmlContent.AppendLine("      return tooltipText;");
            htmlContent.AppendLine("    }");

            // Function to update the map for a given year
            htmlContent.AppendLine("    function updateMap(year) {");
            htmlContent.AppendLine("      if (geojsonLayer) {");
            htmlContent.AppendLine("        map.removeLayer(geojsonLayer);");
            htmlContent.AppendLine("      }");
            htmlContent.AppendLine("      geojsonLayer = L.geoJSON(geojsonData, {");
            htmlContent.AppendLine("        style: function(feature) {");
            htmlContent.AppendLine("          var loadDensity = feature.properties['Yük_Yoğunluğu_' + year] || 0;");
            htmlContent.AppendLine("          return {");
            htmlContent.AppendLine("            fillColor: getColor(loadDensity),");
            htmlContent.AppendLine("            fillOpacity: 0.6,");
            htmlContent.AppendLine("            color: getColor(loadDensity),");
            htmlContent.AppendLine("            weight: 1");
            htmlContent.AppendLine("          };");
            htmlContent.AppendLine("        },");
            htmlContent.AppendLine("        onEachFeature: function(feature, layer) {");
            htmlContent.AppendLine("          layer.bindTooltip(getTooltipText(feature), {");
            htmlContent.AppendLine("            sticky: true,");
            htmlContent.AppendLine("            direction: 'auto'");
            htmlContent.AppendLine("          });");
            htmlContent.AppendLine("        }");
            htmlContent.AppendLine("      }).addTo(map);");
            htmlContent.AppendLine("    }");

            // Initialize the map with the current year
            htmlContent.AppendLine($"    updateMap({currentYear});");

            // Add event listener for the slider
            htmlContent.AppendLine("    var slider = document.getElementById('year-slider');");
            htmlContent.AppendLine("    var yearLabel = document.getElementById('year-label');");
            htmlContent.AppendLine("    slider.addEventListener('input', function() {");
            htmlContent.AppendLine("      var year = parseInt(slider.value);");
            htmlContent.AppendLine("      yearLabel.textContent = 'Year: ' + year;");
            htmlContent.AppendLine("      updateMap(year);");
            htmlContent.AppendLine("    });");

            // Fit the map to the bounds of the GeoJSON layer
            htmlContent.AppendLine("    var bounds = L.geoJSON(geojsonData).getBounds();");
            htmlContent.AppendLine("    map.fitBounds(bounds);");

            htmlContent.AppendLine("  </script>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            // Write the HTML content to the file
            try
            {
                File.WriteAllText(filePath, htmlContent.ToString());
                MessageBox.Show($"Heatmap exported successfully to {filePath}");

                // Optionally open the HTML file in the default browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting heatmap: {ex.Message}");
            }
        }

        // Async click event handler
        private async void ELFTahminButonu_Click(object sender, EventArgs e)
        {
            try
            {

                ELFSenaryolarFilePath = Path.Combine((string)ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)ana_menu_form_objesi.config.İl,
                    (string)ana_menu_form_objesi.config.İlçe,
                    (string)ana_menu_form_objesi.config.ELF.INPUT_FILE);

                // Check if the modified file exists
                if (!File.Exists(ELFSenaryolarFilePath))
                {
                    MessageBox.Show("Lütfen önce senaryo dosyasını ekleyin.");
                    return;
                }

                // Run the R script asynchronously
                string resultsFilePath = await RunModelRScript();

                if (resultsFilePath == null)
                {
                    // If R script failed or no results path was returned, stop further execution
                    return;
                }


                // Load results into tab_ekonometrik
                LoadEkonometrikResults(resultsFilePath);
            }
            finally
            {
                // Restore cursor to default and force UI refresh
                this.Cursor = Cursors.Default;
                this.Refresh(); // Force the UI to update the cursor immediately
            }
        }

        private void SenaryoModuleTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(SenaryoModuleTabControl.SelectedTab == EkonometrikSonuclarTabPage)
            {
                EkonometrikSenaryoElementsPanel.Visible = false;
            }
            else
            {
                EkonometrikSenaryoElementsPanel.Visible = true;
            }
        }

        private void Modül_Tabları_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Check if the user is trying to access tab_senaryo
            if (e.TabPage == tab_senaryo && !tab_senaryo.Enabled)
            {
                // Prevent switching to the tab
                e.Cancel = true;
                // Show the warning message
                MessageBox.Show("Önce lütfen Ekonometrik Yük Tahmini verilerini yükleyiniz!",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            } else
            {
                SenaryoModuleTabControl.SelectedTab = EkonometrikSenaryoTabPage; 
            }
                
        }

        private async void DEKSimulasyonSonucGoruntule_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks while processing
            DEKCenterAddButton.Enabled = false;
            DEKSimButton.Enabled = false;
            try
            {
                string filePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek_distribution_2024_2030_İzmir_düşük.xlsx";
                DataTable simulationData;
                try
                {
                    // Excel dosyasını aç
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        // Yıl seçimine göre sayfayı seç (SelectedYear değeri, sayfa indeksini temsil eder)
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[SelectedYear];

                        // Veriyi DataTable'a yükle
                        simulationData = excelService.LoadWorksheetIntoDataTable(worksheet);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
                    return; // Hata durumunda işlemi sonlandır
                }

                gMapControl_DEK.Overlays.Clear();
                gMapControl_DEK.Refresh();
                // Merkezi Nokta Hesaplama ve Harita Üzerinde Gösterim
                HesaplaMerkezNoktaVeEkle(simulationData);
                await HaritaUzerindeDEKSimulasyonGosterimi(simulationData);

                MessageBox.Show("Veri başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken bir hata oluştu: {ex.Message}");
            }
            finally
            {
                DEKCenterAddButton.Enabled = true;
                DEKSimButton.Enabled = true;
            }
        }

    }
}