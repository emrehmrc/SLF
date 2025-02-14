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


namespace SLF
{
    public partial class ModülFormu : Form
    {

        // ---------------------------------------------- GENEL DEĞİŞKENLER ---------------------------------------------- //


        List<string> modulescheck = new List<string>();
        public readonly CBS cbs;
        public int slfStartYear = 0, slfEndYear = 0;

        TextBox logTextBox; // Declare logTextBox here --------------
        private ExcelService _excelService;
        private ExcelService excelService = new ExcelService();

        private Form popupForm;  
        public Fonksiyon_Oluştur fonksiyonFormu;
        public Tablo_Formu tablo_formu;
        private GirdiModülü girdiModülü;


        public List<PointLatLng> rulerPoints_yga = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_stokastik = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_ea = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_yuk = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_imar = new List<PointLatLng>();

        public GMapOverlay rulerOverlay_yga = new GMapOverlay("rulerOverlay_yga");
        public GMapOverlay rulerOverlay_stokastik = new GMapOverlay("rulerOverlay_stokastik");
        public GMapOverlay rulerOverlay_ea = new GMapOverlay("rulerOverlay_ea");
        public GMapOverlay rulerOverlay_yuk = new GMapOverlay("rulerOverlay_yuk");
        public GMapOverlay rulerOverlay_imar = new GMapOverlay("rulerOverlay_imar");

        public GMapRoute rulerRoute_yga;
        public GMapRoute rulerRoute_stokastik;
        public GMapRoute rulerRoute_ea;
        public GMapRoute rulerRoute_yuk;
        public GMapRoute rulerRoute_imar;

        public GMapOverlay markerOverlay_yga = new GMapOverlay("markerOverlay_yga");
        public GMapOverlay markerOverlay_stokastik = new GMapOverlay("markerOverlay_stokastik");
        public GMapOverlay markerOverlay_ea = new GMapOverlay("markerOverlay_ea");
        public GMapOverlay markerOverlay_yuk = new GMapOverlay("markerOverlay_yuk");
        public GMapOverlay markerOverlay_imar = new GMapOverlay("markerOverlay_imar");
        public GMapOverlay markerOverlay_DEK = new GMapOverlay("markerOverlay_DEK");

        private List<PointLatLng> polygonPoints_yga = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_stokastik = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_imar = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_yuk = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_DEK = new List<PointLatLng>();

        public GMapOverlay polygonOverlay_yga = new GMapOverlay("polygonOverlay_yga");
        private GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        public GMapOverlay polygonOverlay_stokastik = new GMapOverlay("polygonOverlay_stokastik");
        public GMapOverlay polygonOverlay_imar = new GMapOverlay("polygonOverlay_imar");
        public GMapOverlay polygonOverlay_yuk = new GMapOverlay("polygonOverlay_yuk");
        public GMapOverlay polygonOverlay_DEK = new GMapOverlay("polygonOverlay_DEK");

        public Dictionary<int, GMapOverlay> overlaysByLayerIndex = new Dictionary<int, GMapOverlay>();
        public Dictionary<int, CheckBox[]> checkboxesByLayerIndex = new Dictionary<int, CheckBox[]>();
        public Dictionary<string, GMapOverlay> overlaysByName = new Dictionary<string, GMapOverlay>();

        public bool isRulerEnabled = false;
        public bool isRulerActive = false;
        public bool isSelecting_polygon = false;
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
        public CheckBox[] checkBoxes_yga;
        public CheckBox[] checkBoxes_imar;
        public CheckBox[] checkBoxes_stokastik;


        // nokta ekleme/çıkarma gibi opsiyonların olduğu sağ tık menüsü
        public ContextMenuStrip nokta_menüsü;

        // sol tıkla nokta ekleyebilme kontrolü
        public bool adding_points = false;

        // initialize the count of overlays within each mao
        public int ygaOverlayCount = 0;
        public int imarOverlayCount = 0;
        public int stokastikOverlayCount = 0;

        // variable to control the simultaneous on/off operations for all the related checkboxes together
        private bool _isSynchronizingCheckboxes = false;


        // ------------------------------------------------------------------------------------------------------------ //


        // ------------------------------------------ GİRDİ MODÜLÜ DEĞİŞKENLER ---------------------------------------- //

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


        private string SelectedSpeed = "";
        public bool isAddingChargingStation = false; // Sadece şarj istasyonu eklenirken true olacak.
        private bool isAddingDekPoint = false; // Sadece dek noktası eklenirken  true olacak.
        private int _selectedYear = -1;
        private string _selectedCity = null;


        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- CUSTOM CLASSES ------------------------------------------------ //

        // Nokta veri yapısı
        public class NoktaVeri
        {
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public double Bina_Demandi { get; set; }
            public int Abone_Sayısı { get; set; }
            public string CellId { get; set; } // Cell ID (optional)

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



        // ------------------------------------------ INITIALIZATION METHODS ------------------------------------------ //



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


        // Main constructor of the Modüller Formu 
        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {
            // initialize the Modul Formu
            InitializeComponent();
            gMapControl_EA.OnMarkerClick -= gMapControl_EA_OnMarkerClick;
            gMapControl_EA.OnMapClick -= gMapControl_Ea_OnMapClick;
            _excelService = new ExcelService();
            InitializeLogTextBox(); // Initialize logTextBox
            //this.DoubleBuffered = true;
            this.selectedMethod = selectedMethod;  // Store the method
            InitializeComboBoxes();

            // initialize the instance of a CBS form
            cbs = new CBS(this);

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

        // Initialize all form components (called in the constructors)
        private void InitializeFormComponents()
        {
            // Initialize GMap Controls
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);
            InitializeGMap(gMapControl_yuk);
            InitializeGMap(gMapControl_imar);
            InitializeGMap(gMapControl_optimalDTR);
            InitializeGMap(gMapControl_DEK);
            InitializeGMap(gMapControl_yga);


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
            tablo_formu = new Tablo_Formu();
        }

        
        // Helper method to add overlays to the maps
        private void AddOverlaysToMaps()
        {
            // Create a dictionary of overlays for each map control
            var overlays = new Dictionary<GMapControl, List<GMapOverlay>>()
    {
        { gMapControl_stokastik, new List<GMapOverlay> { polygonOverlay_stokastik, rulerOverlay_stokastik, markerOverlay_stokastik } },
        { gMapControl_DEK, new List<GMapOverlay> { markerOverlay_DEK} },
        { gMapControl_EA, new List<GMapOverlay> { rulerOverlay_ea, markerOverlay_ea} },
        { gMapControl_yga, new List<GMapOverlay> { polygonOverlay_yga, markerOverlay_yga, rulerOverlay_yga } },
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
                InitializeTabs("tab_girdi", "tab_ekonometrik", "tab_senaryo", "EkonometrikSenaryoTabPage");
                Modül_Tabları.SelectedTab = tab_girdi;
            }
            else if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                // Hide the specific item you want to remove
                HideComboBoxItem("Ekonometrik Yük Tahmini Verileri"); // Replace with the actual item you want to hide
                // For SLF, do not hide any tabs. Add logic here if needed.
                // List of tab names to hide
                string[] tabsToHide = { "EkonometrikSenaryoTabPage", "tab_ekonometrik" };

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


        // BURASI SONRADAN AÇILACAK, SIMDILIK BOYLE KALSIN.
        private async void Modül_Tabları_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Gerekli kontrolleri yapmak için seçilen sekmeyi ve modülleri kontrol et
            string selectedTabText = Modül_Tabları.SelectedTab.Text;

            // Modüllerin yüklü olup olmadığını kontrol et
            if (selectedMethod == "SLF (Jeo-Uzamsal)")
            {
                if ((selectedTabText == "EA Şarj Modülü" || selectedTabText == "DEK Modülü" || selectedTabText == "Yük Haritası Modülü") && !GirdiModülü.dataTablesByType.ContainsKey("DTR Verileri"))
                {
                    // Sekme geçişini tamamen iptal et
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

                // Harita işlemini başlat
            }
            // DEK Modülü tabına tıklanmışsa
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
        }

        private void SenaryoNewSelectionButton_Click(object sender, EventArgs e)
        {
            Modül_Tabları.SelectedTab = tab_senaryo;
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
            SetOverlayVisibility(cbs.tüm_katmanlar_array_yga[layerIndex], isVisible);
            SetOverlayVisibility(cbs.tüm_katmanlar_array_yuk[layerIndex], isVisible);
            SetOverlayVisibility(cbs.tüm_katmanlar_array_stokastik[layerIndex], isVisible);

            // refresh all maps ---
            gMapControl_imar.Refresh();
            gMapControl_yga.Refresh();
            gMapControl_yuk.Refresh();
            gMapControl_stokastik.Refresh();

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

                if (layerIndex < checkBoxes_yga.Length)
                {
                    if (!ReferenceEquals(cb, checkBoxes_yga[layerIndex]))
                    {
                        checkBoxes_yga[layerIndex].Checked = isVisible;
                    }
                }

                if (layerIndex < checkBoxes_stokastik.Length)
                {
                    if (!ReferenceEquals(cb, checkBoxes_stokastik[layerIndex]))
                    {
                        checkBoxes_stokastik[layerIndex].Checked = isVisible;
                    }
                }
            }
            finally
            {
                _isSynchronizingCheckboxes = false;  // guard off
            }
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


        public void checkboxes_init()
        {
            // Initialize the arrays with the checkboxes for each map category
            checkBoxes_yga = new CheckBox[] { checkBox_yga_1, checkBox_yga_2, checkBox_yga_3, checkBox_yga_4, 
                checkBox_yga_5, checkBox_yga_6, checkBox_yga_7, checkBox_yga_8, checkBox_yga_9, 
                checkBox_yga_10, checkBox_yga_11, checkBox_yga_12, 
                checkBox_yga_13, checkBox_yga_14, checkBox_yga_15 };
            checkBoxes_imar = new CheckBox[] { checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4, 
                checkBox_imar_5, checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9, 
                checkBox_imar_10, checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15 };
            checkBoxes_stokastik = new CheckBox[] { checkBox_stokastik_1, checkBox_stokastik_2, checkBox_stokastik_3, 
                checkBox_stokastik_4, checkBox_stokastik_5, checkBox_stokastik_6, checkBox_stokastik_7, 
                checkBox_stokastik_8, checkBox_stokastik_9, checkBox_stokastik_10, checkBox_stokastik_11, 
                checkBox_stokastik_12, checkBox_stokastik_13, checkBox_stokastik_14, checkBox_stokastik_15 };

            // generate an array of [1, 2, 3, ..., 15]
            int[] tagValuesForCheckboxes = Enumerable.Range(1, 15).ToArray();  

            // Function to initialize CheckBoxes with a tag, event handlers, and forecolor
            void initializeCheckBoxes(CheckBox[] checkBoxes, int[] tagValues)
            {
                for (int i = 0; i < checkBoxes.Length; i++)
                {
                    checkBoxes[i].Tag = tagValues[i];
                    checkBoxes[i].CheckedChanged += checkBox_CheckedChanged;
                    checkBoxes[i].MouseDown += checkBox_MouseDown;
                    checkBoxes[i].ForeColor = cbs.overlayColors[i].BorderColor;  // Set the color from the corresponding cbs.overlayColors
                }
            }

            // Initialize all checkboxes
            initializeCheckBoxes(checkBoxes_yga, tagValuesForCheckboxes);
            initializeCheckBoxes(checkBoxes_imar, tagValuesForCheckboxes);
            initializeCheckBoxes(checkBoxes_stokastik, tagValuesForCheckboxes);
        }


        // ------------------------------------------------------------------------------------------------------------ //

        // --------------------------------- SPESİFİK MODÜL EVENTLERİ & METHODLAR ------------------------------------- //


        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- GİRDİ MODÜLÜ --------------------------------------------------- //


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
                            MessageBox.Show("Geçerli dosyalar seçilmedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

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
                MessageBox.Show("Geçerli bir opsiyon seçilmedi.");
            }

            girdiModülü = girdiModülleri[seçilenVeriTipi];

            girdiModülü.SlfStartYear = slfStartYear;
            girdiModülü.SlfEndYear = slfEndYear;

            InitializeComboBoxes(); // yılların guncellenmesi 
                                    // Check if "ELF" is selected to skip prerequisites
            bool skipPrerequisites = (selectedMethod == "ELF (Ekonometrik)");

            // Call VEERProcess with skipPrerequisites flag
            var isImported = girdiModülü.VEERProcess(seçilenVeriTipi, skipPrerequisites);

            if (isImported)
            {
                modulescheck.Add(seçilenVeriTipi);
                veri_listesi_seçimi.Refresh();
                Console.WriteLine(modulescheck.Count);
                dataGridView_girdi.DataSource = girdiModülü.CurrentDataTable;


            }
        }

        private async void OpenModuleButton_Click(object sender, EventArgs e)
        {
            // Disable the button initially
            OpenModuleButton.Enabled = false;

            string filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";
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
            string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();
            girdiModülü = girdiModülleri[seçilenVeriTipi];
            dataGridView_girdi.DataSource = girdiModülü.importedDataTable;
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
            yearApproveButton.Text = "ONAYLA";

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
                yearApproveButton.Text = "SIFIRLA";
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //

        // -------------------------------------------- STOKASTİK ----------------------------------------------------- //


        private async void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            // Use 'await' so this method actually waits for cbs_dosya_secimi to finish
            await cbs.cbs_dosya_secimi(gMapControl_stokastik, this, tablo_formu.attribute_table);
            this.Cursor = Cursors.Default;

        }

        private void Stokastik_Fonksiyonlar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Fonksiyon.Show(Cursor.Position);
            }
        }
        private void Stokastik_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void Stokastik_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void Stokastik_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_stokastik?.Clear();
            polygonOverlay_stokastik?.Clear();

            cbs.CBS_ölç(mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void Stokastik_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void Stokastik_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
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

        private void gMapControl_stokastik_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_stokastik, Mesafe_stokastik, mesafe_metre_stokastik,
                rulerPoints_stokastik, markerOverlay_stokastik, rulerOverlay_stokastik, ref rulerRoute_stokastik,
                polygonPoints_stokastik, polygonOverlay_stokastik);
        }

        private void gMapControl_stokastik_MouseMove(object sender, MouseEventArgs e)
        {

            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl_stokastik.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();

            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid == true)
            {
                cbs.ending_point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);
                cbs.UpdateSelectionPolygon(gMapControl_stokastik);
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
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                cbs.ending_point = gMapControl_stokastik.FromLocalToLatLng(e.X, e.Y);
                cbs.isSelecting_grid = false;
                gMapControl_stokastik.CanDragMap = true;

                // Clear the selection polygon and refresh the map
                gMapControl_stokastik.Overlays.Remove(cbs.bounding_box_overlay);
                //cbs.AddGridToMap(gMapControl_stokastik);

                gMapControl_stokastik.Refresh();
                gMapControl_imar.Refresh();
            }
        }

        private void gMapControl_stokastik_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_stokastik, ref polygonPoints_stokastik,
                ref polygonOverlay_stokastik, Mesafe_stokastik, mesafe_metre_stokastik);

        }


        private void gMapControl_stokastik_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_stokastik)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }


        // show information about polygons when double-clicking on the map
        private void gMapControl_stokastik_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                foreach (var polygon in cbs.tüm_katmanlar_array_stokastik[checkbox_index].Polygons)
                {
                    if (cbs.IsPointInPolygon(pointClick, polygon))
                    {
                        cbs.HighlightPolygon(polygon, checkbox_index, cbs.GetActiveGMapControl());

                        if (cbs.polygonAttributes_stokastik.TryGetValue(polygon, out DataRow row))
                        {
                            ShowAttributeRow(row);
                            tablo_formu.Show();
                            tablo_formu.BringToFront();
                           
                        }
                    }
                }
            }
        }


        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- EA ------------------------------------------------------------ //
        /*        private async void gMapControl_Ea_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (isAddingChargingStation)
            {
                // Create a new marker for the charging station
                GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.yellow)
                {
                    ToolTipText = "Yeni Şarj İstasyonu",
                    Tag = ModülFormu.SelectedCellId // Access the static property
                };
                markerOverlay_ea.Markers.Add(marker);
                simulationOverlay.Markers.Add(marker);
                cellToolTipOverlay.Markers.Add(marker);
                // Create the point data including the selected CellId (if any)
                NoktaVeri noktaVeri_marker = new NoktaVeri
                {
                    Enlem = Math.Round(pointClick.Lat, 4),
                    Boylam = Math.Round(pointClick.Lng, 4),
                    CellId = ModülFormu.SelectedCellId // Access the static property
                };

                // Show the popup form to add a new charging station
                using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                {
                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        // If successful, reload the map data asynchronously
                        await eaHaritayaVeriYukleAsync();
                    }
                    else if (popupForm.OperationCancelled)
                    {
                        // If cancelled, remove the marker
                        markerOverlay_ea.Markers.Remove(marker);
                        simulationOverlay.Markers.Remove(marker);
                        cellToolTipOverlay.Markers.Remove(marker);
                    }
                }

                // Reset the flag after adding the station
                isAddingChargingStation = false;
                return;
            }

            // Standard map click handling
            OnMapClickEventi(pointClick, e, markerOverlay_ea, ref polygonPoints_ea,
                ref polygonOverlay_ea, Mesafe_yuk, mesafe_metre_yuk);
        }

        private async void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check if the user is in "adding charging station" mode
                if (isAddingChargingStation)
                {
                    // Create a new marker for the charging station at the clicked location
                    GMapMarker marker = new GMarkerGoogle(item.Position, GMarkerGoogleType.yellow)
                    {
                        ToolTipText = "Yeni Şarj İstasyonu",
                        Tag = ModülFormu.SelectedCellId // Access the static property
                    };
                    markerOverlay_ea.Markers.Add(marker);
                    simulationOverlay.Markers.Add(marker);
                    cellToolTipOverlay.Markers.Add(marker);
                    // Create the point data including the selected CellId (if any)
                    NoktaVeri noktaVeri_marker = new NoktaVeri
                    {
                        Enlem = Math.Round(item.Position.Lat, 4),
                        Boylam = Math.Round(item.Position.Lng, 4),
                        CellId = ModülFormu.SelectedCellId // Access the static property
                    };

                    // Show the popup form to add a new charging station
                    using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                    {
                        if (popupForm.ShowDialog() == DialogResult.OK)
                        {
                            // If successful, reload the map data asynchronously
                          await eaHaritayaVeriYukleAsync();
                        }
                        else if (popupForm.OperationCancelled)
                        {
                            // If cancelled, remove the marker
                            markerOverlay_ea.Markers.Remove(marker);
                        }
                    }

                    // Reset the flag after adding the station
                    isAddingChargingStation = false;
                    return;
                }

                // If not in "adding charging station" mode, open the popup form to edit the clicked marker
                if (item.Tag != null)
                {
                    // Retrieve the CellId from the marker's Tag
                    string cellId = item.Tag.ToString();

                    // Create a NoktaVeri instance with the marker's data
                    NoktaVeri noktaVeri_marker = new NoktaVeri
                    {
                        Enlem = Math.Round(item.Position.Lat, 4),
                        Boylam = Math.Round(item.Position.Lng, 4),
                        CellId = cellId // Pass the CellId to the popup form
                    };

                    // Open the popup form to display/edit the marker's details
                    using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                    {
                        if (popupForm.ShowDialog() == DialogResult.OK)
                        {
                            // Retrieve the updated NoktaVeri from the popup form
                            NoktaVeri updatedNoktaVeri = popupForm.NoktaVeri;

                            // Update the marker's Tag with the new CellId
                            item.Tag = updatedNoktaVeri.CellId;
                        }
                    }
                }
            }
        }*/
        /*        private void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (item.Tag != null)
                        {
                            // Retrieve the CellId from the marker's Tag
                            string cellId = item.Tag.ToString();

                            // Create a NoktaVeri instance with the marker's data
                            NoktaVeri noktaVeri_marker = new NoktaVeri
                            {
                                Enlem = Math.Round(item.Position.Lat, 4),
                                Boylam = Math.Round(item.Position.Lng, 4),
                                CellId = cellId // Pass the CellId to the popup form
                            };

                            // Open the popup form to display/edit the marker's details
                            using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                            {
                                if (popupForm.ShowDialog() == DialogResult.OK)
                                {
                                    // Retrieve the updated NoktaVeri from the popup form
                                    NoktaVeri updatedNoktaVeri = popupForm.NoktaVeri;

                                    // Update the marker's Tag with the new CellId
                                    item.Tag = updatedNoktaVeri.CellId;
                                }
                            }
                        }
                    }
                }*/
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
                ref polygonOverlay_ea, Mesafe_yuk, mesafe_metre_yuk);

        }

        /*        private void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
                {
                    if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_ea)
                    {
                        NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                        NoktaBilgileriniGoster(seçili_nokta);
                    }
                }*/
        /*        private async Task HandlePopupFormAsync(PointLatLng point, string cellId)
        {
            // Create a NoktaVeri instance with the marker's data
            NoktaVeri noktaVeri_marker = new NoktaVeri
            {
                Enlem = Math.Round(point.Lat, 4),
                Boylam = Math.Round(point.Lng, 4),
                CellId = cellId // Pass the CellId to the popup form
            };

            // Create a temporary marker for the charging station
            GMapMarker tempMarker = new GMarkerGoogle(point, GMarkerGoogleType.yellow)
            {
                ToolTipText = "Yeni Şarj İstasyonu",
                Tag = cellId // Store CellId in the marker's Tag temporarily
            };

            // Add the temporary marker to the overlay
            markerOverlay_ea.Markers.Add(tempMarker);

            try
            {
                // Open the popup form to display/edit the marker's details
                using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
                {
                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        // Reload the map data asynchronously after the popup form saves the data
                        await eaHaritayaVeriYukleAsync();

                        // Add the marker to the map (if not already added)
                        AddMarkerToMap(noktaVeri_marker);
                    }
                    else if (popupForm.OperationCancelled)
                    {
                        // If cancelled, remove the temporary marker
                        RemoveMarkerFromOverlays(tempMarker);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the process
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Ensure the temporary marker is removed in case of an error
                RemoveMarkerFromOverlays(tempMarker);
            }
        }*/

        private async Task HandlePopupFormAsync(PointLatLng point, string cellId)
        {
            // Create a NoktaVeri instance with the marker's data
            NoktaVeri noktaVeri_marker = new NoktaVeri
            {
                Enlem = Math.Round(point.Lat, 4),
                Boylam = Math.Round(point.Lng, 4),
                CellId = cellId // Pass the CellId to the popup form
            };

            // Open the popup form to display/edit the marker's details
            using (EAStationPopupForm popupForm = new EAStationPopupForm(dataGridView_girdi.DataSource as DataTable, noktaVeri_marker))
            {
                if (popupForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload the map data asynchronously after the popup form saves the data
                    await eaHaritayaVeriYukleAsync();

                    // Add the marker to the map
                    AddMarkerToMap(noktaVeri_marker);
                }

            }
        }


        // Helper method to safely remove a marker from overlays
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
/*        private void gMapControl_Ea_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check if the user is in "adding charging station" mode
                if (isAddingChargingStation)
                {
                    // Simulate a new marker at the clicked location
                    GMapMarker simulatedMarker = new GMarkerGoogle(pointClick, GMarkerGoogleType.yellow)
                    {
                        ToolTipText = "Yeni Şarj İstasyonu",
                        Tag = ModülFormu.SelectedCellId // Use the selected CellId
                    };
                    // Reset the flag after adding the station
                    isAddingChargingStation = false;
                    // Call gMapControl_EA_OnMarkerClick with the simulated marker
                    gMapControl_EA_OnMarkerClick(simulatedMarker, e);


                    return;
                }

                // If not in "adding charging station" mode, check if the click is near an existing marker
                GMapMarker clickedMarker = FindMarkerAtPosition(pointClick);
                if (clickedMarker != null)
                {
                    // Call gMapControl_EA_OnMarkerClick with the clicked marker
                    gMapControl_EA_OnMarkerClick(clickedMarker, e);
                }
                else
                {
                    // Standard map click handling (if no marker is clicked)
                    OnMapClickEventi(pointClick, e, markerOverlay_ea, ref polygonPoints_ea,
                        ref polygonOverlay_ea, Mesafe_yuk, mesafe_metre_yuk);
                }
            }
        }*/

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
                ContextMenuStrip_Nokta.Show(Cursor.Position);
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

        /*        private void ToggleMarkers(string markerType, bool isVisible)
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
                }*/


        // Şehir seçimi yapıldığında çağrılan metot
        private void ilSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox_ea_il_secimi.SelectedItem != null)  // Geçerli bir seçim yapıldığında
            {
                SelectedCity = comboBox_ea_il_secimi.SelectedItem.ToString();  // Şehir adını ayarla
                CheckSelections();  // Seçim durumunu kontrol et

                // Set map position based on selected city
                if (cityCoordinates.TryGetValue(SelectedCity, out PointLatLng coordinates))
                {
                    gMapControl_EA.Position = coordinates; // Set the map's position
                    gMapControl_EA.Zoom = 12; // Adjust the zoom level as needed
                }
            }
        }

        // Yıl seçimi yapıldığında çağrılan metot
        private void yilSecimiMonteCarlo(object sender, EventArgs e)
        {
            if (comboBox_ea_yıl_secimi.SelectedIndex != -1)  // Geçerli bir seçim yapıldığında
            {
                SelectedYear = comboBox_ea_yıl_secimi.SelectedIndex;  // Yıl indeksini ayarla
                CheckSelections();  // Seçim durumunu kontrol et
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
            // Check if the "EA Şarj Verileri" key exists in the dataTablesByType dictionary
            if (!GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
            {
                MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                return;
            }
            gMapControl_EA.OnMarkerClick -= gMapControl_EA_OnMarkerClick;
            gMapControl_EA.OnMapClick -= gMapControl_Ea_OnMapClick;
            // Use dataGridView1.DataSource as the DataTable instead of eaDataTable
            DataTable dataTable = dataGridView_girdi.DataSource as DataTable;
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen EA ŞARJ verilerinizi ekleyin.");
                return;
            }
            if (!gMapControl_EA.Overlays.Contains(simulationOverlay) || !gMapControl_EA.Overlays.Contains(cellToolTipOverlay))
            {
                gMapControl_EA.OnMapClick += gMapControl_Ea_OnMapClick;
            }
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
        private void EANewSimulationResultsButton_Click(object sender, EventArgs e)
        {
            // Get the updated input file path (it was saved earlier in the popup form)
            string inputFilePath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\ea\DELTA_EA_DENEME_IMAR.xlsx"; // Modify this path accordingly

            // Check if the file exists
            if (File.Exists(inputFilePath))
            {
                // Call the Python script with the updated input file
                RunPythonScript(inputFilePath);
            }
            else
            {
                MessageBox.Show("Input file not found! Please ensure the file is saved correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunPythonScript(string inputFilePath)
        {
            try
            {
                string pythonScriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\EA-DEK\ea\ea.py"; // Modify this path accordingly
                string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe"; // Path to your Python executable

                // Build the process start information
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonExePath,
                    Arguments = $"\"{pythonScriptPath}\" \"{inputFilePath}\"",  // Pass the input file path to the script
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process and get the output
                Process process = new Process { StartInfo = startInfo };
                process.Start();

                // Optional: Read output from Python script
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Display result
                MessageBox.Show($"Python script executed successfully!\nOutput:\n{output}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running Python script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*        private void EANewSimulationResultsButton_Click(object sender, EventArgs e)
                {
                    // Get the updated input file path (it was saved earlier in the popup form)
                    string inputFilePath = @"C:\path\to\updated_input_file.csv"; // Modify this path accordingly

                    // Check if the file exists
                    if (File.Exists(inputFilePath))
                    {
                        // Call the Python script with the updated input file
                        RunPythonScript(inputFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Input file not found! Please ensure the file is saved correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                private void RunPythonScript(string inputFilePath)
                {
                    try
                    {
                        string pythonScriptPath = @"C:\path\to\your\python_script.py"; // Modify this path accordingly
                        string pythonExePath = @"C:\path\to\python.exe"; // Path to your Python executable

                        // Build the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = pythonExePath,
                            Arguments = $"\"{pythonScriptPath}\" \"{inputFilePath}\"",  // Pass the input file path to the script
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        // Start the process and get the output
                        Process process = new Process { StartInfo = startInfo };
                        process.Start();

                        // Optional: Read output from Python script
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        // Display result
                        MessageBox.Show($"Python script executed successfully!\nOutput:\n{output}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error running Python script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }*/

        private async Task eaHaritayaVeriYukleAsync()
        {
            int redDc = 0;
            int greenAc = 0;

            try
            {
                GMapOverlay eaOverlay = new GMapOverlay("EA Layer");

                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                if (gMapControl_EA.Overlays.Contains(eaOverlay))
                {
                    gMapControl_EA.Overlays.Remove(eaOverlay);
                }


                DataTable eaData = await Task.Run(() => GirdiModülü.dataTablesByType["EA Şarj Verileri"]);

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
                                !eaData.Columns.Contains("ISTASYON_GUCU") &&
                                !GirdiModülü.dataTablesByType.ContainsKey("EA Şarj Verileri"))
                            {
                                MessageBox.Show("Lütfen EA Sarj modülü verilerinizi yükleyin.");
                                return;
                            }
                            //Console.WriteLine(GirdiModülü.dataTablesByType);
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

        private async void gelecekSimilasyonGoruntule(object sender, EventArgs e)
        {
            // Checkbox'ları görünür hale getir
            checkBox_AC_Home.Visible = true;
            checkBox_AC_Public.Visible = true;
            checkBox_AC_Work.Visible = true;
            checkBox_DC_Fast.Visible = true;
            checkBox_AC_Public.Checked = true;
            checkBox_AC_Work.Checked = true;
            checkBox_AC_Home.Checked = true;
            checkBox_DC_Fast.Checked = true;

            gMapControl_EA.Overlays.Clear();
            gMapControl_EA.Refresh();

            // Şehir ve hız seçimine göre dosya yolunu ayarla
            string filePath = "";

            if (SelectedCity == "İzmir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_Yüksek.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_Düşük.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\İzmir\evcs_monte_carlo_distribution_2024_2030_İzmir_baz.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_Esk_Yüksek.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_Esk_Düşük.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\EV\Esk\evcs_monte_carlo_distribution_2024_2030_esk_baz.xlsx";
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir şehir ve senaryo seçiniz.");
                return; // Geçerli bir şehir veya hız seçilmediyse işlemi sonlandır
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
            DataTable cıktıPopup = FormatEATableForDisplay(veriMonteCarlo);
            // Yeni bir DataGridView oluştur
            DataGridView dataGridView = new DataGridView
            {
                DataSource = cıktıPopup,  // Bind the DataTable
                Dock = DockStyle.Fill,     // Make sure it's filling the container/form
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells // Resize columns based on content
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
                Width = 730,
                Height = 600
            };

            popupForm.Controls.Add(dataGridView);
          //  popupForm.Show(); // Yeni pencereyi göster
        }

        /*                private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
                        {
                            // Create a new overlay for simulation markers
                            GMapOverlay simulationOverlay = new GMapOverlay("Simulasyon_Layer");

                            // Add a new overlay for simulation markers (No need to remove it if it's new)
                            gMapControl_EA.Overlays.Clear(); // Optionally clear the previous overlays, if needed
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
                        }*/
        GMapOverlay simulationOverlay = new GMapOverlay("Simulasyon_Layer");
        GMapOverlay cellToolTipOverlay = new GMapOverlay("CellToolTips");
        public static string SelectedCellId { get; set; }
        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
        {
            // Create overlays:

            gMapControl_EA.Overlays.Clear();
            gMapControl_EA.Overlays.Add(simulationOverlay);
            gMapControl_EA.Overlays.Add(cellToolTipOverlay);

            // Create a transparent bitmap for invisible markers
            Bitmap transparentBitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(transparentBitmap))
            {
                g.Clear(Color.Transparent);
            }

            Dictionary<(double, double, string), GMarkerGoogle> markerDictionary = new Dictionary<(double, double, string), GMarkerGoogle>();

            foreach (DataRow row in veriTablosu.Rows)
            {
                if (row["Enlem"] != DBNull.Value && row["Boylam"] != DBNull.Value)
                {
                    double enlem = Convert.ToDouble(row["Enlem"]);
                    double boylam = Convert.ToDouble(row["Boylam"]);
                    string cellId = row["id"] != DBNull.Value ? row["id"].ToString() : "N/A";

                    // Add an invisible marker for cell tooltips and selection
                    var invisibleMarker = new GMarkerGoogle(new PointLatLng(enlem, boylam), transparentBitmap);
                    invisibleMarker.ToolTipText = $"Cell: {cellId}";
                    invisibleMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    invisibleMarker.Tag = cellId; // Store the cell ID in the marker's Tag
                    cellToolTipOverlay.Markers.Add(invisibleMarker);

                    // Create simulation markers for EV points
                    bool acHome = row["AC (Home)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Home)_count"]) != 0;
                    bool acWork = row["AC (Work)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Work)_count"]) != 0;
                    bool acPublic = row["AC (Public)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Public)_count"]) != 0;
                    bool fastDc = row["Fast DC_count"] != DBNull.Value && Convert.ToInt32(row["Fast DC_count"]) != 0;

                    if (acHome)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.green);
                        marker.ToolTipText = $"Grid: {cellId}\nAC-Home";
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.Tag = "AC-Home"; // Store marker type in Tag
                        markerDictionary[(enlem, boylam, "AC-Home")] = marker;
                    }
                    if (acWork)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue);
                        marker.ToolTipText = $"Grid: {cellId}\nAC-Work";
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.Tag = "AC-Work"; // Store marker type in Tag
                        markerDictionary[(enlem, boylam, "AC-Work")] = marker;
                    }
                    if (acPublic)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.yellow);
                        marker.ToolTipText = $"Grid: {cellId}\nAC-Public";
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.Tag = "AC-Public"; // Store marker type in Tag
                        markerDictionary[(enlem, boylam, "AC-Public")] = marker;
                    }
                    if (fastDc)
                    {
                        var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.red);
                        marker.ToolTipText = $"Grid: {cellId}\nDC-Fast";
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.Tag = "DC-Fast"; // Store marker type in Tag
                        markerDictionary[(enlem, boylam, "DC-Fast")] = marker;
                    }
                }
            }

            foreach (var marker in markerDictionary.Values)
            {
                simulationOverlay.Markers.Add(marker);
            }

            Invoke(new Action(() =>
            {
                gMapControl_EA.Refresh();
            }));

            return Task.CompletedTask;
        }
        /*        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
                {
                    // Create overlays:
                    GMapOverlay simulationOverlay = new GMapOverlay("Simulasyon_Layer");
                    GMapOverlay cellToolTipOverlay = new GMapOverlay("CellToolTips");

                    gMapControl_EA.Overlays.Clear();
                    gMapControl_EA.Overlays.Add(simulationOverlay);
                    gMapControl_EA.Overlays.Add(cellToolTipOverlay);

                    Bitmap transparentBitmap = new Bitmap(16, 16);
                    using (Graphics g = Graphics.FromImage(transparentBitmap))
                    {
                        g.Clear(Color.Transparent);
                    }

                    Dictionary<(double, double, string), GMarkerGoogle> markerDictionary = new Dictionary<(double, double, string), GMarkerGoogle>();

                    foreach (DataRow row in veriTablosu.Rows)
                    {
                        if (row["Enlem"] != DBNull.Value && row["Boylam"] != DBNull.Value)
                        {
                            double enlem = Convert.ToDouble(row["Enlem"]);
                            double boylam = Convert.ToDouble(row["Boylam"]);
                            string cellId = row["id"] != DBNull.Value ? row["id"].ToString() : "N/A";

                            // Add an invisible marker for cell tooltips and selection
                            var invisibleMarker = new GMarkerGoogle(new PointLatLng(enlem, boylam), transparentBitmap);
                            invisibleMarker.ToolTipText = $"Cell: {cellId}";
                            invisibleMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            // Store the cell ID in the marker's Tag for later reference
                            invisibleMarker.Tag = cellId;
                            cellToolTipOverlay.Markers.Add(invisibleMarker);

                            // Create simulation markers for EV points (as before)
                            bool acHome = row["AC (Home)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Home)_count"]) != 0;
                            bool acWork = row["AC (Work)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Work)_count"]) != 0;
                            bool acPublic = row["AC (Public)_count"] != DBNull.Value && Convert.ToInt32(row["AC (Public)_count"]) != 0;
                            bool fastDc = row["Fast DC_count"] != DBNull.Value && Convert.ToInt32(row["Fast DC_count"]) != 0;

                            if (acHome)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.green);
                                marker.ToolTipText = $"Grid: {cellId}\nAC-Home";
                                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                markerDictionary[(enlem, boylam, "AC-Home")] = marker;
                            }
                            if (acWork)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue);
                                marker.ToolTipText = $"Grid: {cellId}\nAC-Work";
                                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                markerDictionary[(enlem, boylam, "AC-Work")] = marker;
                            }
                            if (acPublic)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.yellow);
                                marker.ToolTipText = $"Grid: {cellId}\nAC-Public";
                                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                markerDictionary[(enlem, boylam, "AC-Public")] = marker;
                            }
                            if (fastDc)
                            {
                                var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.red);
                                marker.ToolTipText = $"Grid: {cellId}\nDC-Fast";
                                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                markerDictionary[(enlem, boylam, "DC-Fast")] = marker;
                            }
                        }
                    }

                    foreach (var marker in markerDictionary.Values)
                    {
                        simulationOverlay.Markers.Add(marker);
                    }

                    Invoke(new Action(() =>
                    {
                        gMapControl_EA.Refresh();
                    }));

                    return Task.CompletedTask;
                }
        */

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


        // -------------------------------------------- DEK ------------------------------------------------------------ //



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

        private void gMapControl_Dek_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_dek)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }


        private async void gMapControl_DEK_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            // DEK modülü için OnMapClickEventi çağrısı
            OnMapClickEventi(pointClick, e, markerOverlay_DEK, ref polygonPoints_DEK,
                ref polygonOverlay_DEK, Mesafe_Dek, mesafe_metre_DeK);

            if (isAddingDekPoint)
            {
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


        // DEK şehri seçildiğinde çağrılan metot
        private void dek_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_DEK_il.SelectedItem != null)  // Geçerli bir seçim yapıldığında
            {
                SelectedCity = comboBox_DEK_il.SelectedItem.ToString();  // Şehir adını ayarla
                CheckSelections();  // Seçim durumunu kontrol et

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
            gMapControl_DEK.Overlays.Clear();
            gMapControl_DEK.Refresh();

            // Şehir ve hız seçimine göre dosya yolunu ayarla
            string filePath = "";

            if (SelectedCity == "İzmir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\99_Free Work Area\ArdaS\senaryolar\DEK\İzmir\dek_distribution_2024_2030_İzmir_yüksek.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\99_Free Work Area\ArdaS\senaryolar\DEK\İzmir\dek_distribution_2024_2030_İzmir_düşük.xlsx";
            }
            else if (SelectedCity == "İzmir" && SelectedSpeed == "varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\DEK\İzmir\dek_distribution_2024_2030_3_İzmir_baz.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Hızlı")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_baz.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "Yavaş")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_düşük.xlsx";
            }
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "varsayılan")
            {
                filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\ArdaS\senaryolar\DEK\Esk\dek_distribution_2024_2030_esk_yüksek.xlsx";
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir şehir ve senaryo seçiniz.");
                return; // Geçerli bir şehir veya hız seçilmediyse işlemi sonlandır
            }

            DataTable dek_veri;

            try
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
            popupForm.Show(); // Yeni pencereyi göster
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

        private void CheckSelections()
        {
            // Seçimlerin yapıldığını kontrol ederek butonu etkinleştir
            EASimButton.Enabled = SelectedYear != -1 && SelectedCity != null; // ea modulu 
            DEKSimButton.Enabled = SelectedYear != -1 && SelectedCity != null; // dek modulu 
        }


        private async Task dekHaritayaVeriYukleAsync()
        {
            try
            {
                GMapOverlay dekOverlay = new GMapOverlay("Dek Layer");

                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                if (gMapControl_DEK.Overlays.Contains(dekOverlay))
                {
                    gMapControl_DEK.Overlays.Remove(dekOverlay);
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

                        gMapControl_DEK.Overlays.Add(dekOverlay);
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

        private Task HaritaUzerindeDEKSimulasyonGosterimi(DataTable veriTablosu)
        {
            // Create or get the overlay for DEK simulation markers
            GMapOverlay dekOverlay = new GMapOverlay("DEK_Simulasyon_Layer");

            // Remove existing overlay if it exists
            if (gMapControl_DEK.Overlays.Contains(dekOverlay))
            {
                gMapControl_DEK.Overlays.Remove(dekOverlay);
                Console.WriteLine("Existing overlay removed.");
            }

            // Add a new overlay for DEK simulation markers
            gMapControl_DEK.Overlays.Add(dekOverlay);

            // Dictionary to hold markers based on their coordinates and types
            Dictionary<(double, double, string), GMarkerGoogle> markerDictionary = new Dictionary<(double, double, string), GMarkerGoogle>();

            // Process the rows in the DataTable
            foreach (DataRow row in veriTablosu.Rows)
            {
                // Debug output for each row
                Console.WriteLine($"Processing row with DEK_distributed: {row["DEK_distributed"]}");

                // Only process rows where DEK_distributed value is greater than 0
                if (row["DEK_distributed"] != DBNull.Value && Convert.ToDouble(row["DEK_distributed"]) > 0)
                {
                    // Get latitude and longitude values
                    double enlem = Convert.ToDouble(row["Enlem"]);
                    double boylam = Convert.ToDouble(row["Boylam"]);

                    // Get ID and DEK_distributed values
                    string id = row["id"].ToString();
                    double dekValue = Convert.ToDouble(row["DEK_distributed"]);

                    // Create a new marker and display it on the map
                    var marker = new GMarkerGoogle(new PointLatLng(enlem, boylam), GMarkerGoogleType.blue);
                    marker.ToolTipText = $"ID: {id}\nDEK: {dekValue}";

                    // Add the marker to the overlay
                    dekOverlay.Markers.Add(marker);
                    Console.WriteLine($"Marker added at ({enlem}, {boylam}) with ID: {id}");
                }
            }

            // Refresh the map control to show the new markers
            Invoke(new Action(() =>
            {
                gMapControl_DEK.Refresh(); // Update the map
                Console.WriteLine("Map refreshed.");
            }));

            return Task.CompletedTask;
        }

        private void dek_list_years(object sender, EventArgs e) // 
        {
            if (comboBox_DEK_Yıl.SelectedIndex != -1)  // Geçerli bir seçim yapıldığında
            {
                SelectedYear = comboBox_DEK_Yıl.SelectedIndex;  // Yıl indeksini ayarla
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


        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- YGA ------------------------------------------------------------ //


        // yga dosya seçimi butonu
        private async void yga_dosya_secimi_Click(object sender, EventArgs e)
        {
            // Assume `cbs` is properly instantiated
            await cbs.cbs_dosya_secimi(gMapControl_yga, this, tablo_formu.attribute_table);
        }

        private void Yga_Sec_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_yga, Mesafe_yga);
        }

        private void Yga_Mesafe_Olc_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_yga?.Clear();
            polygonOverlay_yga?.Clear();

        }

        private void Yga_Polygon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void Yga_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void gMapControl_yga_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_yga, Mesafe_yga, mesafe_metre_yga,
                rulerPoints_yga, markerOverlay_yga, rulerOverlay_yga, ref rulerRoute_yga,
                polygonPoints_yga, polygonOverlay_yga);
        }


        private void gMapControl_yga_MouseMove(object sender, MouseEventArgs e)
        {

            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl_yga.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();

            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid == true)
            {
                cbs.ending_point = gMapControl_yga.FromLocalToLatLng(e.X, e.Y);
                cbs.UpdateSelectionPolygon(gMapControl_yga);
            }

            // eğer sadece 1 adet nokta seçilmişse, ve ikinci nokta dinamik olarak farklı yerlere
            // tıklanarak seçiliyorsa, mesafeyi de buna göre güncelle.
            if (isRulerActive && rulerPoints_yga.Count == 1 && isRulerEnabled == true)
            {

                var point = gMapControl_yga.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_yga != null)
                {
                    rulerOverlay_yga.Routes.Remove(rulerRoute_yga);
                }
                rulerRoute_yga = new GMapRoute(new List<PointLatLng> { rulerPoints_yga[0], point }, "rulerRoute_yga");
                rulerRoute_yga.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_yga.Routes.Add(rulerRoute_yga);
                gMapControl_yga.Refresh();
            }
        }

        private void gMapControl_yga_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_yga, ref polygonPoints_yga,
                ref polygonOverlay_yga, Mesafe_yga, mesafe_metre_yga);

        }

        private void gMapControl_yga_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                foreach (var polygon in cbs.tüm_katmanlar_array_yga[checkbox_index].Polygons)
                {
                    if (cbs.IsPointInPolygon(pointClick, polygon))
                    {
                        cbs.HighlightPolygon(polygon, checkbox_index, cbs.GetActiveGMapControl());

                        if (cbs.polygonAttributes_yga.TryGetValue(polygon, out DataRow row))
                        {
                            ShowAttributeRow(row);
                            tablo_formu.Show();
                            tablo_formu.BringToFront();

                        }
                    }
                }
            }
        }

        private void gMapControl_yga_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_yga)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
        }


        private void YGASaveButton_Click(object sender, EventArgs e)
        {
            if (polygonPoints_yga.Count >= 3)  // Ensure polygon is valid (at least 3 points)
            {
                // Create a DataTable with polygon data (latitude, longitude, and parameters)
                DataTable polygonDataTable = cbs.CreatePolygonDataTable(polygonPoints_yga, layer_index);


                // Clean up after saving the polygon
                polygonPoints_yga.Clear(); // Clear the list of points for the polygon

                // Show the YGA-specific attribute table popup
                // `true` indicates it's for YGA
                ShowAttributeTable(polygonDataTable, true);  
                
                // Call PoligonKaydetEventi to save the polygon and handle other operations
                PoligonKaydetEventi(sender, e, polygonOverlay_yga, polygonPoints_yga);
            }
            else
            {
                MessageBox.Show("Lütfen en az 3 nokta tıklayarak bir poligon oluşturunuz.");
            }
        }


        private void ShowYGAPopupForm(DataTable datatable)
        {
            // Create the YGA-specific popup and pass the DataTable for context
            YGAPopupForm popup = new YGAPopupForm(datatable);

            // Show the form as a dialog (blocking until it's closed)
            if (popup.ShowDialog() == DialogResult.OK)
            {
                // After user input, get the updated DataTable from the popup
                //DataTable updatedData = popup.GetUpdatedData();  // Get the updated table
                //ShowAttributeTable(updatedData, true);  // Update the UI with the new data for YGA
            }
        }

        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- İMAR ------------------------------------------------------------ //

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
                //cbs.AddGridToMap(gMapControl_imar);
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


        // -------------------------------------------- YUK ------------------------------------------------------------ //


        private void Yuk_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_yuk, rulerRoute_yuk, gMapControl_yuk,
                mesafe_metre_yuk, Mesafe_yuk);
        }


        private void gMapControl_yuk_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_yuk, Mesafe_yuk, mesafe_metre_yuk,
                rulerPoints_yuk, markerOverlay_yuk, rulerOverlay_yuk, ref rulerRoute_yuk,
                polygonPoints_yuk, polygonOverlay_yuk);
        }

        private void gMapControl_yuk_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current position of the center of the map
            PointLatLng centerPosition = gMapControl_yuk.Position;

            // Update the strings with the center position coordinates
            centerX = centerPosition.Lng.ToString();
            centerY = centerPosition.Lat.ToString();

            // boolean controlu ile grid oluşturulacak alan seçimine başlanması
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid == true)
            {
                cbs.ending_point = gMapControl_yuk.FromLocalToLatLng(e.X, e.Y);
                cbs.UpdateSelectionPolygon(gMapControl_yuk);
            }

            // eğer sadece 1 adet nokta seçilmişse, ve ikinci nokta dinamik olarak farklı yerlere
            // tıklanarak seçiliyorsa, mesafeyi de buna göre güncelle.
            if (isRulerActive && rulerPoints_yuk.Count == 1 && isRulerEnabled == true)
            {

                var point = gMapControl_yuk.FromLocalToLatLng(e.X, e.Y);
                if (rulerRoute_yuk != null)
                {
                    rulerOverlay_yuk.Routes.Remove(rulerRoute_yuk);
                }
                rulerRoute_yuk = new GMapRoute(new List<PointLatLng> { rulerPoints_yuk[0], point }, "rulerRoute_yuk");
                rulerRoute_yuk.Stroke = new Pen(Color.Red, 3);
                rulerOverlay_yuk.Routes.Add(rulerRoute_yuk);
                gMapControl_yuk.Refresh();
            }
        }


        private void gMapControl_yuk_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && cbs.isSelecting_grid)
            {
                // grid oluşturmak için seçilen alan (bounding box) ın son noktası
                cbs.ending_point = gMapControl_yuk.FromLocalToLatLng(e.X, e.Y);
                cbs.isSelecting_grid = false;
                gMapControl_yuk.CanDragMap = true;

                // Clear the selection polygon and refresh the map
                gMapControl_yuk.Overlays.Remove(cbs.bounding_box_overlay);
                gMapControl_yuk.Refresh();
            }
        }

        private void gMapControl_yuk_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_yuk, ref polygonPoints_yuk,
        ref polygonOverlay_yuk, Mesafe_yuk, mesafe_metre_yuk);
        }


        private void trackBar_Yıllar_ValueChanged(object sender, EventArgs e)
        {
            int selectedYear = trackBar_Yıllar.Value;
            yuk_yıl_deger.Text = $"{selectedYear}";

            // Construct the column name based on the selected year
            string columnName = $"{selectedYear}";

            // Call a method to update the heatmap using the selected year's data
            UpdateHeatmapForYear(columnName);
        }

        
        private void UpdateHeatmapForYear(string columnName)
        {
            // Clear the existing overlay for a fresh heatmap
            cbs.GetActiveGMapControl().Overlays.Clear();
            gMapControl_yuk.Overlays.Clear();
            gMapControl_yuk.Overlays.Add(cbs.tüm_katmanlar_array_yuk[0]); // ------------------------------ CHECK THIS FOR LAYER_INDEX

            // Create a new overlay for the heatmap
            GMapOverlay heatmapOverlay = new GMapOverlay("Heatmap");

            // Assuming your data is stored in a DataTable called yourDataTable
            DataTable dataTable = cbs.tüm_katmanlar_datatable[0];

            // Initialize min and max values
            double min = double.MaxValue;
            double max = double.MinValue;

            // Calculate min and max values for the specified column
            foreach (DataRow row in dataTable.Rows)
            {
                if (row[columnName] != DBNull.Value && int.TryParse(row[columnName].ToString(), out int value))
                {
                    if (value < min) min = value;
                    if (value > max) max = value;
                }
            }

            // Display heatmap based on the column data
            cbs.CreateHeatmap(cbs.tüm_katmanlar_array_yuk[0], cbs.tüm_katmanlar_datatable[0], columnName);
            cbs.CreateHeatmapLegend(min, max);

            // Add the overlay to the GMap control
            cbs.GetActiveGMapControl().Overlays.Add(heatmapOverlay);
            cbs.GetActiveGMapControl().Refresh();


        }



        // ------------------------------------------------------------------------------------------------------------ //


        // -------------------------------------------- ELF ------------------------------------------------------------ //

        private void LoadImagesIntoPictureBoxes()
        {
            // Path to the folder where the images are saved
            string imageFolderPath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\veriler deneme\Grafik Outputs\";

            // Load images into PictureBox controls with checks
            LoadImageIntoPictureBox(pictureBox_ELF_1, Path.Combine(imageFolderPath, "bolge_aydınlatma_projections.png"));
            LoadImageIntoPictureBox(pictureBox_ELF_2, Path.Combine(imageFolderPath, "bolge_mesken_projections.png"));
            LoadImageIntoPictureBox(pictureBox_ELF_3, Path.Combine(imageFolderPath, "bolge_sanayi_projections.png"));
            LoadImageIntoPictureBox(pictureBox_ELF_4, Path.Combine(imageFolderPath, "bolge_sulama_projections.png"));
            LoadImageIntoPictureBox(pictureBox_ELF_5, Path.Combine(imageFolderPath, "bolge_ticarethane_projections.png"));
            // Add more PictureBox assignments as needed
        }



        // Save button logic to update Excel file with changes from DataGridViews
        private async void ELFScenerioSaveButton_Click(object sender, EventArgs e)
        {
            string originalFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";
            string modifiedFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

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

                MessageBox.Show("Kullanıcı değişiklikleri excel dosyasına kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya güncelleme hatası: {ex.Message}");
            }
        }

        private void LoadResultsToTabEkonometrik(string resultsFilePath)
        {
            if (!File.Exists(resultsFilePath))
            {
                MessageBox.Show("Sonuç dosyası bulunamadı.");
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
            // Path to the Excel file
            string filePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\SONUÇLAR\ELF_Tahmin_Sonuçları_2024-11-08 22_35_57.xlsx";

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



        private void ELFPredictionShowResultsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Set cursor to wait while running the operations
                Cursor.Current = Cursors.WaitCursor;

                string modifiedFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\INPUT_FILE\INPUT_FILE.xlsx";

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
                // Load images into PictureBox controls after loading the results
                LoadImagesIntoPictureBoxes();
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
            string rScriptPath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Model\model.R";
            string resultsFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\SONUÇLAR\ELF_Tahmin_Sonuçları.xlsx";

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
                        //resultsFilePath = args.Data;  // Capture the file path
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
            Console.WriteLine("R script executed successfully. Results saved in: " + resultsFilePath);
            return resultsFilePath;  // Return the results file path
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

        private void ELFShowGraphsButton_Click(object sender, EventArgs e)
        {
            ELFResultsTabControls.SelectedTab = ELFGraphicOutputsTabPage;
        }


        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------- GENEL HARITA VE TOOLBAR EVENTLERİ & METOTLAR -------------------------------//



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
        private void Poligon_Çiz_Click(object sender, EventArgs e)
        {
            isSelecting_polygon = true;
            isRulerEnabled = false;
            isRulerActive = false;

            // Determine the active map control and reset accordingly
            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                ResetMapControls(gMapControl_imar, mesafe_metre_imar, Mesafe_imar, markerOverlay_imar, rulerOverlay_imar, rulerRoute_imar, rulerPoints_imar);
            }
            else if (cbs.GetActiveGMapControl() == gMapControl_yga)
            {
                ResetMapControls(gMapControl_yga, mesafe_metre_yga, Mesafe_yga, markerOverlay_yga, rulerOverlay_yga, rulerRoute_yga, rulerPoints_yga);
            }
            else if (cbs.GetActiveGMapControl() == gMapControl_stokastik)
            {
                ResetMapControls(gMapControl_stokastik, mesafe_metre_stokastik, Mesafe_stokastik, markerOverlay_stokastik, rulerOverlay_stokastik, rulerRoute_stokastik, rulerPoints_stokastik);
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
            if (cbs.GetActiveGMapControl() == gMapControl_yga)
            {
                if (polygonOverlay_yga == null || polygonOverlay_yga.Polygons.Count == 0)
                {
                    MessageBox.Show("Herhangi bir poligon çizilmemiştir. Lütfen öncelikle bir poligon çiziniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (cbs.GetActiveGMapControl() == gMapControl_stokastik)
            {
                if (polygonOverlay_stokastik == null || polygonOverlay_stokastik.Polygons.Count == 0)
                {
                    MessageBox.Show("Herhangi bir poligon çizilmemiştir. Lütfen öncelikle bir poligon çiziniz.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void Poligon_Kaydet_Click(object sender, EventArgs e)
        {
            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                PoligonKaydetEventi(sender, e, polygonOverlay_imar, polygonPoints_imar);

            }
            if (cbs.GetActiveGMapControl() == gMapControl_yga)
            {
                PoligonKaydetEventi(sender, e, polygonOverlay_yga, polygonPoints_yga);

            }
            else if (cbs.GetActiveGMapControl() == gMapControl_stokastik)

            {
                PoligonKaydetEventi(sender, e, polygonOverlay_stokastik, polygonPoints_stokastik);
            }

        }


        // Helper method to bring buttons to the front
        private void BringButtonsToFront()
        {
            buton_yga_harita_katmanlar.BringToFront();
            buton_stokastik_harita_katmanlar.BringToFront();
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
                        ToolTipText = $"Lat={pointClick.Lat}, Lng={pointClick.Lng}"
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

                    if (activeMap == gMapControl_stokastik)
                        arrayForActiveMap = cbs.tüm_katmanlar_array_stokastik;
                    else if (activeMap == gMapControl_imar)
                        arrayForActiveMap = cbs.tüm_katmanlar_array_imar;
                    else if (activeMap == gMapControl_yga)
                        arrayForActiveMap = cbs.tüm_katmanlar_array_yga;
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
                markerOverlay_stokastik?.Clear();
                markerOverlay_ea.Markers?.Clear();
                markerOverlay_DEK.Markers?.Clear();
                markerOverlay_imar.Markers?.Clear();
                markerOverlay_yga.Markers?.Clear();
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
                else if (polygonOverlay == cbs.tüm_katmanlar_array_yga[layer_index])
                    sourceDict = cbs.polygonAttributes_yga;
                else if (polygonOverlay == cbs.tüm_katmanlar_array_yuk[layer_index])
                    sourceDict = cbs.polygonAttributes_yuk;
                else if (polygonOverlay == cbs.tüm_katmanlar_array_stokastik[layer_index])
                    sourceDict = cbs.polygonAttributes_stokastik;
                else
                {
                    MessageBox.Show("Overlay dictionary eşleşmedi!");
                    return;
                }

                // create a DataTable for the layer
                DataTable polygonDataTable = cbs.CreatePolygonDataTable(polygonPoints, layer_index);
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
                if (polygonOverlay != cbs.tüm_katmanlar_array_yga[layer_index])
                {
                    GMapOverlay newYga = new GMapOverlay($"PolygonLayer_{layer_index + 1}");
                    gMapControl_yga.Overlays.Add(newYga);
                    cbs.tüm_katmanlar_array_yga[layer_index] = newYga;

                    cbs.CopyOverlayContents(
                        polygonOverlay,
                        cbs.tüm_katmanlar_array_yga[layer_index],
                        sourceDict,
                        cbs.polygonAttributes_yga
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
                if (polygonOverlay != cbs.tüm_katmanlar_array_stokastik[layer_index])
                {

                    GMapOverlay newStok = new GMapOverlay($"PolygonLayer_{layer_index + 1}");
                    gMapControl_stokastik.Overlays.Add(newStok);
                    cbs.tüm_katmanlar_array_stokastik[layer_index] = newStok;

                    cbs.CopyOverlayContents(
                        polygonOverlay,
                        cbs.tüm_katmanlar_array_stokastik[layer_index],
                        sourceDict,
                        cbs.polygonAttributes_stokastik
                    );
                }

                // Update the checkboxes for that layer in each 4 different map
                List<CheckBox> associatedChecks = GetCheckBoxesByIndex(layer_index);
                foreach (var chk in associatedChecks)
                {
                    chk.Text = polygonOverlay.Id;   // display layer name
                    chk.Visible = true;
                    chk.Checked = true;
                }

                // Prepare a new overlay for future use
                polygonOverlay = null;
                polygonPoints.Clear();
                isSelecting_polygon = false;

                gMapControl_DEK.Refresh();
                gMapControl_EA.Refresh();
                gMapControl_imar.Refresh();
                gMapControl_stokastik.Refresh();
                gMapControl_yga.Refresh();
                gMapControl_yuk.Refresh();

                MessageBox.Show("Poligon kaydedildi!");
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

        private void ModülFormu_Load(object sender, EventArgs e)
        {
            // Modül formunu yüklerken reset year selection sürecini başlat
            ResetYearSelectionProcessGirdiModulu();
        }


        public List<CheckBox> GetCheckBoxesByIndex(int index)
        {
            // Define arrays for checkboxes
            var stokastikCheckBoxes = new CheckBox[] { checkBox_stokastik_1, checkBox_stokastik_2, checkBox_stokastik_3, 
                checkBox_stokastik_4, checkBox_stokastik_5, checkBox_stokastik_6, checkBox_stokastik_7, checkBox_stokastik_8, 
                checkBox_stokastik_9, checkBox_stokastik_10, checkBox_stokastik_11, checkBox_stokastik_12, 
                checkBox_stokastik_13, checkBox_stokastik_14, checkBox_stokastik_15 };
            var imarCheckBoxes = new CheckBox[] { checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4, 
                checkBox_imar_5, checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9, checkBox_imar_10, 
                checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15 };
            var ygaCheckBoxes = new CheckBox[] { checkBox_yga_1, checkBox_yga_2, checkBox_yga_3, checkBox_yga_4, 
                checkBox_yga_5, checkBox_yga_6, checkBox_yga_7, checkBox_yga_8, checkBox_yga_9, 
                checkBox_yga_10, checkBox_yga_11, checkBox_yga_12, checkBox_yga_13, checkBox_yga_14, checkBox_yga_15 };

            // Ensure the index is valid before accessing arrays
            if (index >= 0 && index < 15)
            {
                // Return the checkboxes for the given index
                return new List<CheckBox> { stokastikCheckBoxes[index], imarCheckBoxes[index], ygaCheckBoxes[index] };
            }

            // Return an empty list if index is out of range
            return new List<CheckBox>();  // Empty list instead of null
        }

        private void ShowAttributeTable(DataTable datatable, bool isYga = false)
        {
            // Always update the DataGridView with the DataTable
            tablo_formu.attribute_table.DataSource = datatable;

            // If it's for YGA, trigger the popup for additional input
            if (isYga)
            {
                // Display the YGA popup form and pass the polygonDataTable
                ShowYGAPopupForm(datatable);
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


        private void ClearCheckboxesForAllMaps(int checkboxIndex)
        {
            // Define all checkboxes for the maps
            var allCheckBoxes = new List<CheckBox>
            {
                checkBox_imar_1, checkBox_imar_2, checkBox_imar_3, checkBox_imar_4, checkBox_imar_5,
                checkBox_imar_6, checkBox_imar_7, checkBox_imar_8, checkBox_imar_9, checkBox_imar_10,
                checkBox_imar_11, checkBox_imar_12, checkBox_imar_13, checkBox_imar_14, checkBox_imar_15,

                checkBox_yga_1, checkBox_yga_2, checkBox_yga_3, checkBox_yga_4, checkBox_yga_5,
                checkBox_yga_6, checkBox_yga_7, checkBox_yga_8, checkBox_yga_9, checkBox_yga_10,
                checkBox_yga_11, checkBox_yga_12, checkBox_yga_13, checkBox_yga_14, checkBox_yga_15,

                checkBox_stokastik_1, checkBox_stokastik_2, checkBox_stokastik_3, checkBox_stokastik_4,
                checkBox_stokastik_5, checkBox_stokastik_6, checkBox_stokastik_7, checkBox_stokastik_8,
                checkBox_stokastik_9, checkBox_stokastik_10, checkBox_stokastik_11, checkBox_stokastik_12, 
                checkBox_stokastik_13, checkBox_stokastik_14, checkBox_stokastik_15
            };

            // Loop through all checkboxes and uncheck/hide the selected one
            foreach (var checkBox in allCheckBoxes)
            {
                if (checkBox.Tag != null && int.TryParse(checkBox.Tag.ToString(), out int tagIndex))
                {
                    if (tagIndex - 1 == checkboxIndex)  // Match the checkbox index
                    {
                        checkBox.Checked = false;  // Uncheck the checkbox
                        checkBox.Visible = false;  // Hide the checkbox
                        //checkBox.Tag = null;       // Reset the Tag to allow reuse  -----------------??????????????
                    }
                }
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
                        checkBoxes_stokastik[checkbox_index].Text = newName;
                        checkBoxes_yga[checkbox_index].Text = newName;


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
                                    var overlay_yga = cbs.tüm_katmanlar_array_yga[checkbox_index];
                                    var overlay_yuk = cbs.tüm_katmanlar_array_yuk[checkbox_index];
                                    var overlay_stokastik = cbs.tüm_katmanlar_array_stokastik[checkbox_index];

                                    var activeMap = cbs.GetActiveGMapControl();

                                    if (activeMap.Overlays.Contains(overlay_imar) || activeMap.Overlays.Contains(overlay_yga) ||
                                        activeMap.Overlays.Contains(overlay_yuk) || activeMap.Overlays.Contains(overlay_stokastik))
                                    {
                                        gMapControl_imar.Overlays.Remove(overlay_imar);
                                        gMapControl_yga.Overlays.Remove(overlay_yga);
                                        gMapControl_yuk.Overlays.Remove(overlay_yuk);
                                        gMapControl_stokastik.Overlays.Remove(overlay_stokastik);

                                        gMapControl_imar.Refresh();
                                        gMapControl_yga.Refresh();
                                        gMapControl_yuk.Refresh();
                                        gMapControl_stokastik.Refresh();
                                    }
                                }

                                // Dispose and nullify references
                                cbs.tüm_katmanlar_array_imar[checkbox_index]?.Dispose();
                                cbs.tüm_katmanlar_array_yga[checkbox_index]?.Dispose();
                                cbs.tüm_katmanlar_array_yuk[checkbox_index]?.Dispose();
                                cbs.tüm_katmanlar_array_stokastik[checkbox_index]?.Dispose();

                                cbs.tüm_katmanlar_array_imar[checkbox_index] = null;
                                cbs.tüm_katmanlar_array_yga[checkbox_index] = null;
                                cbs.tüm_katmanlar_array_yuk[checkbox_index] = null;
                                cbs.tüm_katmanlar_array_stokastik[checkbox_index] = null;

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
            if (layerIndex < 0 || layerIndex >= 15)
            {
                MessageBox.Show("Geçersiz katman endeksi.");
                return;
            }

            // retrieve the four overlay arrays 
            GMapOverlay overlayImar = cbs.tüm_katmanlar_array_imar[layerIndex];
            GMapOverlay overlayYga = cbs.tüm_katmanlar_array_yga[layerIndex];
            GMapOverlay overlayYuk = cbs.tüm_katmanlar_array_yuk[layerIndex];
            GMapOverlay overlayStok = cbs.tüm_katmanlar_array_stokastik[layerIndex];

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
                    SetOverlayColor(overlayYga, selectedColor);
                    SetOverlayColor(overlayYuk, selectedColor);
                    SetOverlayColor(overlayStok, selectedColor);

                    // Recolor all three checkboxes associated with this layer index
                    checkBoxes_imar[layerIndex].ForeColor = selectedColor;
                    checkBoxes_yga[layerIndex].ForeColor = selectedColor;
                    checkBoxes_stokastik[layerIndex].ForeColor = selectedColor;

                    // refresh each map
                    gMapControl_imar.Refresh();
                    gMapControl_yga.Refresh();
                    gMapControl_yuk.Refresh();
                    gMapControl_stokastik.Refresh();
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
        private int FindLayerIndexFromOverlay(GMapOverlay overlay)
        {
            // Check each array for a match
            for (int i = 0; i < 15; i++)
            {
                if (cbs.tüm_katmanlar_array_imar[i] == overlay) return i;
                if (cbs.tüm_katmanlar_array_stokastik[i] == overlay) return i;
                if (cbs.tüm_katmanlar_array_yga[i] == overlay) return i;
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

        private void buton_stokastik_harita_katmanlar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                harita_katmanları_right_click.Show();
            }
            else
            {
                harita_katmanları_right_click.Hide();
            }
        }

        private void buton_yga_harita_katmanlar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                harita_katmanları_right_click.Show();
            }
            else
            {
                harita_katmanları_right_click.Hide();
            }
        }

        private void toolStrip_yga_poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private async void buton_dosya_yga_Click(object sender, EventArgs e)
        {
            await cbs.cbs_dosya_secimi(gMapControl_yga, this, tablo_formu.attribute_table);
        }

        private void toolStrip_yga_fonksiyon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Fonksiyon.Show(Cursor.Position);
            }
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

        // -------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------- //

    }
}