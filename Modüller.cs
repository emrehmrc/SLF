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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading.Tasks;
using ClosedXML.Excel;
using OfficeOpenXml;
using DrawingImage = System.Drawing.Image;

namespace SLF
{
    public partial class ModülFormu : Form
    {
        public readonly CBS cbs;
        private readonly double startX = 0;
        private readonly double startY = 0;
        public int slfStartYear = 0, slfEndYear = 0;

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
        public List<PointLatLng> rulerPoints_stokastik = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_ea = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_yuk = new List<PointLatLng>();
        public List<PointLatLng> rulerPoints_imar = new List<PointLatLng>();

        public GMapOverlay rulerOverlay_stokastik = new GMapOverlay("rulerOverlay_stokastik");
        public GMapOverlay rulerOverlay_ea = new GMapOverlay("rulerOverlay_ea");
        public GMapOverlay rulerOverlay_yuk = new GMapOverlay("rulerOverlay_yuk");
        public GMapOverlay rulerOverlay_imar = new GMapOverlay("rulerOverlay_imar");

        public GMapRoute rulerRoute_stokastik;
        public GMapRoute rulerRoute_ea;
        public GMapRoute rulerRoute_yuk;
        public GMapRoute rulerRoute_imar;

        public bool isRulerEnabled = false;
        public bool isRulerActive = false; // enable the drawing of a ruler
        public bool isSelecting_polygon = false;

        public GMapOverlay markerOverlay_stokastik = new GMapOverlay("markerOverlay_stokastik");
        public GMapOverlay markerOverlay_ea = new GMapOverlay("markerOverlay_ea");
        public GMapOverlay markerOverlay_yuk = new GMapOverlay("markerOverlay_yuk");
        public GMapOverlay markerOverlay_imar = new GMapOverlay("markerOverlay_imar");

        private List<PointLatLng> polygonPoints_ea = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_stokastik = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_imar = new List<PointLatLng>();
        private List<PointLatLng> polygonPoints_yuk = new List<PointLatLng>();

        private GMapOverlay polygonOverlay_ea = new GMapOverlay("polygonOverlay_ea");
        public GMapOverlay polygonOverlay_stokastik = new GMapOverlay("polygonOverlay_stokastik");
        public GMapOverlay polygonOverlay_imar = new GMapOverlay("polygonOverlay_imar");
        public GMapOverlay polygonOverlay_yuk = new GMapOverlay("polygonOverlay_yuk");


        private DTRModulu dtrmod = new DTRModulu();


        // boolean variable to control the marker/point selection by mouse down event
        private bool isSelecting_marker = false;

        // X and Y coordinates of the center location of the gMapControl object
        public string centerX;
        public string centerY;


        private List<YüklenenDosya> loadedFiles = new List<YüklenenDosya>();


        // Find the first available slot in the array that holds shapefile overlay layers
        public int layer_index;

        // variables to be used in the "join attributes by location" functionality
        public int firstLayerToJoin;
        public int secondLayerToJoin;
        public string firstLayerName;
        public string secondLayerName;

        // variable to control whichever checkbox/its associated data is selected the latest
        public System.Windows.Forms.CheckBox lastClickedCheckbox;

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
        

        private string selectedMethod;  // Store the method
        public List<TabPage> hiddenTabs = new List<TabPage>();  // To store hidden tabs

        // Main constructor with parameters for selectedMethod and tabToSelect
        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {
            // initialize the Modul Formu
            InitializeComponent();
            
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


        }

        // Initialize all form components (called in the constructors)
        private void InitializeFormComponents()
        {
            InitializeGMap(gMapControl_stokastik);
            InitializeGMap(gMapControl_EA);
            InitializeGMap(gMapControl_yuk);
            InitializeGMap(gMapControl_imar);
            InitializeGMap(gMapControl_optimalDTR);

            SortTabPagesAlphabetically(Modül_Tabları, true);
            // Enable double buffering for the form to reduce flickering
            this.DoubleBuffered = true;

            // Default selected tab
            Modül_Tabları.SelectedTab = tab_girdi;

            buton_stokastik_harita_katmanlar.BringToFront();
            buton_ea_harita_katmanlar.BringToFront();
            buton_yuk_haritası_katmanlar.BringToFront();
            buton_imar_katmanlar.BringToFront();
            buton_optimalDTR_katmanlar.BringToFront();


            // Add overlays to the maps
            gMapControl_stokastik.Overlays.Add(rulerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(markerOverlay_stokastik);
            gMapControl_stokastik.Overlays.Add(cbs.gridOverlay);
            checkboxes_init();

            gMapControl_EA.Overlays.Add(rulerOverlay_ea);
            gMapControl_EA.Overlays.Add(markerOverlay_ea);
            gMapControl_EA.Overlays.Add(polygonOverlay_ea);

            gMapControl_yuk.Overlays.Add(rulerOverlay_yuk);
            gMapControl_yuk.Overlays.Add(markerOverlay_yuk);
            gMapControl_yuk.Overlays.Add(polygonOverlay_yuk);

            gMapControl_imar.Overlays.Add(rulerOverlay_imar);
            gMapControl_imar.Overlays.Add(markerOverlay_imar);
            gMapControl_imar.Overlays.Add(polygonOverlay_imar);

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // Default selection for veri_listesi_seçimi
            veri_listesi_seçimi.SelectedIndex = 2;

            // Initialize the tablo_formu instance
            tablo_formu = new Tablo_Formu();
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
                // For SLF, do not hide any tabs. Add logic here if needed.
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

        private void OpenModuleButton_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";

            // Load the Excel package
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Clear previous data
                ELFMinSenaryoTable.DataSource = null;
                ELFLowSenaryoTable.DataSource = null;
                ELFBaseSenaryoTable.DataSource = null;
                ELFHighSenaryoTable.DataSource = null;
                ELFMaxSenaryoTable.DataSource = null;

                // Load only sheets 2 to 6 (indices 1 to 5)
                for (int i = 0; i <= 5; i++) // i = 1 corresponds to sheet 2, i = 5 corresponds to sheet 6
                {
                    var worksheet = package.Workbook.Worksheets[i + 1]; // Worksheets are 1-indexed, so i + 1 is used here
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

                    // Set the data source for the corresponding DataGridView
                    if (i < 5) // Adjust to match the DataGridView indices
                    {
                        var dataGrids = new[] { ELFMinSenaryoTable, ELFLowSenaryoTable, ELFBaseSenaryoTable, ELFHighSenaryoTable, ELFMaxSenaryoTable };
                        dataGrids[i].DataSource = dt;
                    }
                }
            }

            // Optionally set the selected tab to tab_senaryo
            Modül_Tabları.SelectedTab = tab_senaryo;
        }


        /// <summary>
        /// ELF METHOD Model RScript Run RELATED CHANGES & UPDATES
        /// </summary>

        private void ELFScenerioSaveGunaButton_Click(object sender, EventArgs e)
        {
            string originalFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\INPUT_FILE-deneme.xlsx";
            string modifiedFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

            // Dictionary to store original formulas
            var originalFormulas = new Dictionary<string, string>();

            // Load the original Excel file and read formulas
            try
            {
                using (var package = new ExcelPackage(new FileInfo(originalFilePath)))
                {
                    // Loop through each worksheet and store formulas
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                        {
                            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                            {
                                var cell = worksheet.Cells[row, col];

                                // Store formulas in the dictionary
                                if (!string.IsNullOrEmpty(cell.Formula))
                                {
                                    // Generate a unique key for the cell based on its address
                                    originalFormulas[$"{worksheet.Name}!{cell.Address}"] = cell.Formula;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading original Excel file: {ex.Message}");
                return;
            }

            // Load the original file again to allow modifications
            try
            {
                using (var package = new ExcelPackage(new FileInfo(originalFilePath)))
                {
                    // Access the first worksheet for any required operations (preserving formulas)
                    ExcelWorksheet firstWorksheet = package.Workbook.Worksheets[0];

                    // Update worksheets with data from DataGridViews
                    UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[1], ELFMinSenaryoTable);
                    UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[2], ELFLowSenaryoTable);
                    UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[3], ELFBaseSenaryoTable);
                    UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[4], ELFHighSenaryoTable);
                    UpdateWorksheetFromDataGridView(package.Workbook.Worksheets[5], ELFMaxSenaryoTable);

                    // Restore original formulas
                    foreach (var kvp in originalFormulas)
                    {
                        var parts = kvp.Key.Split('!');
                        var sheetName = parts[0];
                        var cellAddress = parts[1];

                        var worksheet = package.Workbook.Worksheets[sheetName];
                        var cell = worksheet.Cells[cellAddress];

                        // Apply the original formula
                        cell.Formula = kvp.Value;
                    }

                    // Save the modified Excel file
                    package.SaveAs(new FileInfo(modifiedFilePath));
                }

                // Inform the user that the changes were saved successfully
                MessageBox.Show("User changes saved to the modified Excel file.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating Excel file: {ex.Message}");
            }
        }

        // Load the original Excel file into DataGridView for display
        private void LoadDataIntoDataGridView(ExcelWorksheet worksheet, DataGridView dgv)
        {
            dgv.Rows.Clear(); // Clear existing rows

            for (int row = 1; row <= worksheet.Dimension.Rows; row++)
            {
                int rowIndex = dgv.Rows.Add(); // Add a new row to the DataGridView

                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    var cell = worksheet.Cells[row, col];

                    // Store the original value in the cell's tag for later use
                    dgv.Rows[rowIndex].Cells[col - 1].Value = Math.Round(cell.GetValue<double>(), 1); // Display as 4.9%
                    dgv.Rows[rowIndex].Cells[col - 1].Tag = cell.Value; // Preserve full value
                }
            }
        }

        // Helper method to update an Excel worksheet based on the DataGridView
        private void UpdateWorksheetFromDataGridView(ExcelWorksheet worksheet, DataGridView dgv)
        {
            for (int row = 0; row < dgv.Rows.Count; row++)
            {
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    var cell = worksheet.Cells[row + 2, col + 1]; // Start at row 2 in Excel
                    var cellValue = dgv.Rows[row].Cells[col].Tag; // Get the original full value

                    // Only update cell values if the original value is not null
                    if (cellValue != null)
                    {
                        // Set the numeric value directly
                        cell.Value = cellValue;

                        // Preserve the original format from the original file
                        cell.Style.Numberformat.Format = "0.000000000000000%"; // Preserve full precision
                    }
                }
            }
        }

        /// <summary>
        /// ELF METHOD Model RScript Run RELATED CHANGES & UPDATES
        /// </summary>
        private void ELFPredictionShowResultsGunaButton_Click(object sender, EventArgs e)
        {
            string modifiedFilePath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

            // Check if the modified file exists
            if (!File.Exists(modifiedFilePath))
            {
                MessageBox.Show("The modified Excel file does not exist. Please save the scenario first.");
                return;
            }

            // Run the R script
            string resultsFilePath = RunModelRScript(modifiedFilePath);

            // Load results into tab_ekonometrik
            LoadResultsToTabEkonometrik(resultsFilePath);
        }

        // Method to run the R script
        private string RunModelRScript(string modifiedFilePath)
        {
            string rScriptPath = @"C:\Users\begum.orhan\MRC\İletişim sitesi - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Model\model.R";
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
                // Capture output from the R script
                process.OutputDataReceived += (sender, args) => {
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

        // stokastik dosya seçimi butonu
        private async void stokastik_dosya_seçimi_Click(object sender, EventArgs e)
        {
            // Assume `cbs` is properly instantiated
            await cbs.cbs_dosya_secimi(gMapControl_stokastik, this, tablo_formu.attribute_table);

        }

        private async void imar_dosya_seçimi_Click(object sender, EventArgs e)
        {
            await cbs.cbs_dosya_secimi(gMapControl_imar, this, tablo_formu.attribute_table);
        }


        public List<CheckBox> GetCheckBoxesByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return new List<CheckBox> { checkBox9, checkBox1 };
                case 1:
                    return new List<CheckBox> { checkBox10, checkBox2 };
                case 2:
                    return new List<CheckBox> { checkBox11, checkBox3 };
                case 3:
                    return new List<CheckBox> { checkBox12, checkBox4 };
                case 4:
                    return new List<CheckBox> { checkBox13, checkBox5 };
                case 5:
                    return new List<CheckBox> { checkBox14, checkBox6 };
                case 6:
                    return new List<CheckBox> { checkBox15, checkBox7 };
                case 7:
                    return new List<CheckBox> { checkBox16, checkBox8 };
                case 8:
                    return new List<CheckBox> { checkBox17, checkBox22 };
                case 9:
                    return new List<CheckBox> { checkBox18, checkBox23 };
                case 10:
                    return new List<CheckBox> { checkBox19, checkBox24 };
                case 11:
                    return new List<CheckBox> { checkBox20, checkBox25 };
                case 12:
                    return new List<CheckBox> { checkBox21, checkBox26 };

                // Add more cases as needed
                default:
                    return null; // or return an empty list if preferred
            }
        }

        // stokastik haritasına ait checkboxların initializationları
        private void checkboxes_init()
        {

            checkBox1.Tag = 1;
            checkBox2.Tag = 2;
            checkBox3.Tag = 3;
            checkBox4.Tag = 4;
            checkBox5.Tag = 5;
            checkBox6.Tag = 6;
            checkBox7.Tag = 7;
            checkBox8.Tag = 8;
            checkBox22.Tag = 9;
            checkBox23.Tag = 10;
            checkBox24.Tag = 11;
            checkBox25.Tag = 12;
            checkBox26.Tag = 13;

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

            // checkboxlara event atama
            checkBox1.CheckedChanged += checkBox_CheckedChanged;
            checkBox2.CheckedChanged += checkBox_CheckedChanged;
            checkBox3.CheckedChanged += checkBox_CheckedChanged;
            checkBox4.CheckedChanged += checkBox_CheckedChanged;
            checkBox5.CheckedChanged += checkBox_CheckedChanged;
            checkBox6.CheckedChanged += checkBox_CheckedChanged;
            checkBox7.CheckedChanged += checkBox_CheckedChanged;
            checkBox8.CheckedChanged += checkBox_CheckedChanged;
            checkBox9.CheckedChanged += checkBox_CheckedChanged;
            checkBox10.CheckedChanged += checkBox_CheckedChanged;
            checkBox11.CheckedChanged += checkBox_CheckedChanged;
            checkBox12.CheckedChanged += checkBox_CheckedChanged;
            checkBox13.CheckedChanged += checkBox_CheckedChanged;
            checkBox14.CheckedChanged += checkBox_CheckedChanged;
            checkBox15.CheckedChanged += checkBox_CheckedChanged;
            checkBox16.CheckedChanged += checkBox_CheckedChanged;
            checkBox17.CheckedChanged += checkBox_CheckedChanged;
            checkBox18.CheckedChanged += checkBox_CheckedChanged;
            checkBox19.CheckedChanged += checkBox_CheckedChanged;
            checkBox20.CheckedChanged += checkBox_CheckedChanged;
            checkBox21.CheckedChanged += checkBox_CheckedChanged;
            checkBox22.CheckedChanged += checkBox_CheckedChanged;
            checkBox23.CheckedChanged += checkBox_CheckedChanged;
            checkBox24.CheckedChanged += checkBox_CheckedChanged;
            checkBox25.CheckedChanged += checkBox_CheckedChanged;
            checkBox26.CheckedChanged += checkBox_CheckedChanged;

            checkBox1.MouseDown += checkBox_MouseDown;
            checkBox2.MouseDown += checkBox_MouseDown;
            checkBox3.MouseDown += checkBox_MouseDown;
            checkBox4.MouseDown += checkBox_MouseDown;
            checkBox5.MouseDown += checkBox_MouseDown;
            checkBox6.MouseDown += checkBox_MouseDown;
            checkBox7.MouseDown += checkBox_MouseDown;
            checkBox8.MouseDown += checkBox_MouseDown;
            checkBox9.MouseDown += checkBox_MouseDown;
            checkBox10.MouseDown += checkBox_MouseDown;
            checkBox11.MouseDown += checkBox_MouseDown;
            checkBox12.MouseDown += checkBox_MouseDown;
            checkBox13.MouseDown += checkBox_MouseDown;
            checkBox14.MouseDown += checkBox_MouseDown;
            checkBox15.MouseDown += checkBox_MouseDown;
            checkBox16.MouseDown += checkBox_MouseDown;
            checkBox17.MouseDown += checkBox_MouseDown;
            checkBox18.MouseDown += checkBox_MouseDown;
            checkBox19.MouseDown += checkBox_MouseDown;
            checkBox20.MouseDown += checkBox_MouseDown;
            checkBox21.MouseDown += checkBox_MouseDown;
            checkBox22.MouseDown += checkBox_MouseDown;
            checkBox23.MouseDown += checkBox_MouseDown;
            checkBox24.MouseDown += checkBox_MouseDown;
            checkBox25.MouseDown += checkBox_MouseDown;
            checkBox26.MouseDown += checkBox_MouseDown;

            checkBox1.ForeColor = cbs.overlayColors[0].BorderColor;
            checkBox2.ForeColor = cbs.overlayColors[1].BorderColor;
            checkBox3.ForeColor = cbs.overlayColors[2].BorderColor;
            checkBox4.ForeColor = cbs.overlayColors[3].BorderColor;
            checkBox5.ForeColor = cbs.overlayColors[4].BorderColor;
            checkBox6.ForeColor = cbs.overlayColors[5].BorderColor;
            checkBox7.ForeColor = cbs.overlayColors[6].BorderColor;
            checkBox8.ForeColor = cbs.overlayColors[7].BorderColor;
            checkBox9.ForeColor = cbs.overlayColors[0].BorderColor;
            checkBox10.ForeColor = cbs.overlayColors[1].BorderColor;
            checkBox11.ForeColor = cbs.overlayColors[2].BorderColor;
            checkBox12.ForeColor = cbs.overlayColors[3].BorderColor;
            checkBox13.ForeColor = cbs.overlayColors[4].BorderColor;
            checkBox14.ForeColor = cbs.overlayColors[5].BorderColor;
            checkBox15.ForeColor = cbs.overlayColors[6].BorderColor;
            checkBox16.ForeColor = cbs.overlayColors[7].BorderColor;
            checkBox17.ForeColor = cbs.overlayColors[8].BorderColor;
            checkBox18.ForeColor = cbs.overlayColors[9].BorderColor;
            checkBox19.ForeColor = cbs.overlayColors[10].BorderColor;
            checkBox20.ForeColor = cbs.overlayColors[11].BorderColor;
            checkBox21.ForeColor = cbs.overlayColors[12].BorderColor;
            checkBox22.ForeColor = cbs.overlayColors[8].BorderColor;
            checkBox23.ForeColor = cbs.overlayColors[9].BorderColor;
            checkBox24.ForeColor = cbs.overlayColors[10].BorderColor;
            checkBox25.ForeColor = cbs.overlayColors[11].BorderColor;
            checkBox26.ForeColor = cbs.overlayColors[12].BorderColor;

        }

        private void ShowAttributeTable(DataTable datatable)
        {
            tablo_formu.attribute_table.DataSource = datatable;
        }


        // mouse down event of the checkboxes which displays the related data table with the corresponding
        // checkbox/layer
        private void checkBox_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.CheckBox sender_checkbox = sender as System.Windows.Forms.CheckBox;
            int checkbox_index = int.Parse(sender_checkbox.Tag.ToString()) - 1;

            // Update the last clicked checkbox
            if (lastClickedCheckbox != null)
            {
                // Reset previous checkbox style to normal
                lastClickedCheckbox.Font = new Font(lastClickedCheckbox.Font, FontStyle.Regular);
            }

            //sender_checkbox.Font = new Font(sender_checkbox.Font, FontStyle.Italic | FontStyle.Underline);
            lastClickedCheckbox = sender_checkbox;

            temizleToolStripMenuItem.Tag = sender_checkbox;
            rengiDeğiştirToolStripMenuItem.Tag = sender_checkbox;
            kaydetToolStripMenuItem.Tag = sender_checkbox;
            yenidenAdlandırToolStripMenuItem.Tag = sender_checkbox;

            if (cbs.tüm_katmanlar_array[checkbox_index] != null)
            {
                tablo_formu.Text = "Veri Tablosu -- " + cbs.tüm_katmanlar_array_names[checkbox_index] +
                   " -- " + cbs.tüm_katmanlar_datatable[checkbox_index].Rows.Count + " satır -- " +
                   cbs.tüm_katmanlar_datatable[checkbox_index].Columns.Count + " sütun";
                ShowAttributeTable(cbs.tüm_katmanlar_datatable[checkbox_index]);
            }
        }

        // display or hide the layers by checkboxes of the stokastik_yuk_tahmini form
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox)sender;
            int index = int.Parse(checkBox.Tag.ToString()) - 1;

            if (cbs.tüm_katmanlar_array[index] != null)
            {
                cbs.tüm_katmanlar_array[index].IsVisibile = checkBox.Checked;
                cbs.GetActiveGMapControl().Refresh();
            }
        }


        //------------------------------------Tool Strip Menu Items --------------------------//
        private void yenidenAdlandırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem yeniden_adlandir = sender as ToolStripMenuItem;

            if (yeniden_adlandir != null)
            {
                System.Windows.Forms.CheckBox checkBox = yeniden_adlandir.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (cbs.tüm_katmanlar_array[checkbox_index] != null)
                {
                    // Prompt the user to input a new name
                    string newName = Prompt.ShowDialog("Yeni isim:", "Katmanı Yeniden Adlandır");

                    if (!string.IsNullOrEmpty(newName))
                    {
                        // Rename the layer in your underlying data structure
                        checkBox.Text = newName; // Adjust this according to your layer data structure
                        cbs.tüm_katmanlar_array_names[checkbox_index] = newName;

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
                        kmlOverlay = cbs.tüm_katmanlar_array[checkbox_index];

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
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (cbs.tüm_katmanlar_array[checkbox_index] != null)
                {
                    string katman_ismi = cbs.tüm_katmanlar_array_names[checkbox_index];

                    DialogResult temizle_result = MessageBox.Show(katman_ismi + " isimli katman " +
                        "silinecektir. Emin misiniz?", "", MessageBoxButtons.YesNo);

                    if (temizle_result == DialogResult.Yes)
                    {
                        cbs.GetActiveGMapControl().Overlays.Remove(cbs.tüm_katmanlar_array[checkbox_index]);
                        cbs.GetActiveGMapControl().Refresh();

                        cbs.tüm_katmanlar_array[checkbox_index].Dispose();
                        cbs.tüm_katmanlar_array[checkbox_index] = null;
                        cbs.tüm_katmanlar_array_names[checkbox_index] = null;
                        cbs.tüm_katmanlar_datatable[checkbox_index] = null;
                        checkBox.Checked = false;
                        checkBox.Visible = false;

                    }
                }
            }
        }

        private void rengiDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem rengini_degistir_menu_item = sender as ToolStripMenuItem;

            if (rengini_degistir_menu_item != null)
            {
                System.Windows.Forms.CheckBox checkBox = rengini_degistir_menu_item.Tag as System.Windows.Forms.CheckBox;
                int checkbox_index = int.Parse(checkBox.Tag.ToString()) - 1;

                if (checkbox_index < 0 || checkbox_index >= cbs.tüm_katmanlar_array.Length)
                {
                    MessageBox.Show("Yanlış katman endeksi!", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                GMapOverlay overlay = cbs.tüm_katmanlar_array[checkbox_index];
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

                    cbs.GetActiveGMapControl().Refresh(); // Redraw the map to reflect the changes
                }
            }
        }

        //---------------------------------------------------------------------------///



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




        // nokta ekleme/çıkarma gibi opsiyonların olduğu menü
        public ContextMenuStrip nokta_menüsü;

        // noktaların eklenip çıkarılacağı liste
        //private List<Shape> pointsList = new List<Shape>();

        // sol tıkla nokta ekleyebilme kontrolü
        public bool adding_points = false;
        int point_index = 0;


        /* ------------------------------------------------------------------------------------*/

        //////////////// --------------- BUTTON EVENTS  ------------------------////////////////


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
                    $"An error occurred: {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // eğer EA tabı seçilmişe, Stokastik tabındaki polygonları sil, ya da vice versa.
        private void Poligon_Çiz_Click(object sender, EventArgs e)
        {
            isSelecting_polygon = true;
            isRulerEnabled = false;
            isRulerActive = false;

            if (cbs.GetActiveGMapControl() == gMapControl_imar)
            {
                polygonPoints_imar.Clear();
                mesafe_metre_imar.Text = string.Empty;
                Mesafe_imar.Visible = false;
                markerOverlay_imar?.Clear();
                rulerOverlay_imar?.Clear();
                rulerRoute_imar?.Clear();
                rulerPoints_imar?.Clear();

            }
            else if (cbs.GetActiveGMapControl() == gMapControl_stokastik)
            {
                polygonPoints_stokastik.Clear();
                mesafe_metre_stokastik.Text = string.Empty;
                Mesafe_stokastik.Visible = false;
                markerOverlay_stokastik?.Clear();
                rulerOverlay_stokastik?.Clear();
                rulerRoute_stokastik?.Clear();
                rulerPoints_stokastik?.Clear();
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



        //---------------------------- CBS TOOLBOX EVENTLERİ  ----------------------------------//

        private void Stokastik_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void İmar_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_imar, rulerRoute_imar, gMapControl_imar,
                mesafe_metre_imar, Mesafe_imar);
        }

        private void EA_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void Yuk_Kaydır_Click(object sender, EventArgs e)
        {
            cbs.CBS_kaydır(markerOverlay_yuk, rulerRoute_yuk, gMapControl_yuk,
                mesafe_metre_yuk, Mesafe_yuk);
        }

        private void Stokastik_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_stokastik, rulerRoute_stokastik, gMapControl_stokastik,
                mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void EA_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_ea, rulerRoute_ea, gMapControl_EA,
                mesafe_metre_ea, Mesafe_ea);
        }

        private void Yuk_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_yuk, rulerRoute_yuk, gMapControl_yuk,
                mesafe_metre_yuk, Mesafe_yuk);
        }

        private void İmar_Seç_Click(object sender, EventArgs e)
        {
            cbs.CBS_sec(markerOverlay_imar, rulerRoute_imar, gMapControl_imar,
                    mesafe_metre_imar, Mesafe_imar);
        }

        private void Stokastik_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_stokastik?.Clear();
            polygonOverlay_stokastik?.Clear();

            cbs.CBS_ölç(mesafe_metre_stokastik, Mesafe_stokastik);
        }

        private void EA_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_ea?.Clear();
            polygonOverlay_ea?.Clear();

            cbs.CBS_ölç(mesafe_metre_ea, Mesafe_ea);
        }

        private void Yuk_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_yuk?.Clear();
            polygonOverlay_yuk?.Clear();

            cbs.CBS_ölç(mesafe_metre_yuk, Mesafe_yuk);
        }

        private void İmar_Mesafe_Ölç_Click(object sender, EventArgs e)
        {
            isRulerActive = true;
            isRulerEnabled = true;
            polygonPoints_imar?.Clear();
            polygonOverlay_imar?.Clear();
            
            cbs.CBS_ölç(mesafe_metre_imar, Mesafe_imar);
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

        private void Stokastik_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        private void EA_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void Stokastik_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }
        }

        private void MouseDownEvent(object sender, MouseEventArgs e, GMapControl gMapControl,
            Label mesafe, Label mesafe_metre, List<PointLatLng> rulerPoints, 
            GMapOverlay markerOverlay, GMapOverlay rulerOverlay,
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
        
        private void gMapControl_stokastik_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_stokastik, Mesafe_stokastik, mesafe_metre_stokastik,
                rulerPoints_stokastik,markerOverlay_stokastik,rulerOverlay_stokastik,ref rulerRoute_stokastik,
                polygonPoints_stokastik, polygonOverlay_stokastik);
        }

        private void gMapControl_imar_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownEvent(sender, e, gMapControl_imar, Mesafe_imar, mesafe_metre_imar,
                rulerPoints_imar, markerOverlay_imar, rulerOverlay_imar, ref rulerRoute_imar,
                polygonPoints_imar, polygonOverlay_imar);
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
                cbs.AddGridToMap(gMapControl_stokastik);
                gMapControl_stokastik.Refresh();
            }
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
                    fonksiyonFormu.tum_sutunlar.Items.Add(columns.ToString());
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


        public void OnMapClickEventi(PointLatLng pointClick, MouseEventArgs e, GMapOverlay markerOverlay,
    List<PointLatLng> polygonPoints, ref GMapOverlay polygonOverlay,
    Label mesafe, Label mesafe_metre)
        {
            if (e.Button == MouseButtons.Left)
            {
                // boolean control for marker selection when clicking on the map
                if (isSelecting_marker)
                {
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.green);
                    markerOverlay.Markers.Add(marker);

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
                    polygonPoints.Add(pointClick);
                    GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue);
                    markerOverlay.Markers.Add(marker);

                    if (polygonOverlay != null)
                    {
                        cbs.GetActiveGMapControl().Overlays.Remove(polygonOverlay);
                    }

                    layer_index = Array.FindIndex(cbs.tüm_katmanlar_array, s => s == null);

                    polygonOverlay = new GMapOverlay("polygonOverlay_" + layer_index.ToString());

                    cbs.GetActiveGMapControl().Overlays.Add(polygonOverlay);
                    cbs.GetActiveGMapControl().Refresh();

                    // eğer gmapControl_OnMapClick event'i ile 2 den fazla nokta seçilirse,
                    // bu noktalar arasında bir poligon çiz
                    if (polygonPoints.Count >= 3)
                    {
                        cbs.Draw_Polygon(polygonPoints, polygonOverlay, cbs.GetActiveGMapControl());
                        double area = cbs.CalculatePolygonArea(polygonPoints);

                        mesafe_metre.Visible = false;
                        mesafe.Visible = true;
                        mesafe.Text = "Seçili Alan: " + Math.Round(area, 0).ToString() + " m²";
                    }
                }
            }


        }

        private void gMapControl_imar_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_imar, polygonPoints_imar, ref polygonOverlay_imar,
                Mesafe_imar, mesafe_metre_imar);
        }

        private void gMapControl_stokastik_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            OnMapClickEventi(pointClick, e, markerOverlay_stokastik, polygonPoints_stokastik, 
                ref polygonOverlay_stokastik, Mesafe_stokastik, mesafe_metre_stokastik);
        }


        private void Poligon_Kaydet_Click(object sender, EventArgs e)
        {

            if (polygonOverlay_stokastik != null && polygonOverlay_stokastik.Polygons.Count != 0)
            {
                markerOverlay_stokastik.Markers.Clear();

                layer_index = Array.FindIndex(cbs.tüm_katmanlar_array, s => s == null);
                GMapOverlay overlay_to_be_saved = polygonOverlay_stokastik;
                cbs.tüm_katmanlar_array[layer_index] = overlay_to_be_saved;
                cbs.tüm_katmanlar_array_names[layer_index] = "Polygon_" + "_" + (layer_index + 1).ToString();

                // Convert gridOverlay to MapWinGIS.Shapefile so that it could be exported by the MapWinGIS
                // built-in function SaveAsEx
                MapWinGIS.Shapefile myShapefile = cbs.ConvertOverlayToShapefile(overlay_to_be_saved);
                cbs.shapeFileArray_MapWinGIS[layer_index] = myShapefile;

                // Create DataTable and store it
                DataTable polygonDataTable = cbs.CreatePolygonDataTable(polygonPoints_stokastik, layer_index);
                cbs.tüm_katmanlar_datatable[layer_index] = polygonDataTable;

                // Get the list of associated checkboxes for the given layer_index
                List<System.Windows.Forms.CheckBox> associatedCheckBoxes = GetCheckBoxesByIndex(layer_index);

                if (associatedCheckBoxes != null)
                {
                    // Loop through each checkbox in the list and apply the required settings
                    foreach (var checkBox in associatedCheckBoxes)
                    {
                        checkBox.Checked = true;
                        checkBox.Visible = true;
                        checkBox.Text = cbs.tüm_katmanlar_array_names[layer_index];
                    }
                }

                /*System.Windows.Forms.CheckBox associatedCheckBox = GetCheckBoxesByIndex(layer_index);
                if (associatedCheckBox != null)
                {
                    associatedCheckBox.Checked = true;
                    associatedCheckBox.Visible = true;
                    associatedCheckBox.Text = cbs.tüm_katmanlar_array_names[layer_index];
                }*/

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


        /////////////////////////////// ---------------------- /////////////////////////////////

        private void katmanlar_right_click_Opening(object sender, CancelEventArgs e)
        {
            // Get the context menu strip that is being opened
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;

            // Get the checkbox associated with the context menu strip and set its font to bold
            System.Windows.Forms.CheckBox clickedCheckBox = (System.Windows.Forms.CheckBox)contextMenuStrip.SourceControl;
            clickedCheckBox.Font = new Font(clickedCheckBox.Font, System.Drawing.FontStyle.Underline | FontStyle.Italic);

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

            // Check if "ELF" is selected to skip prerequisites
            bool skipPrerequisites = (selectedMethod == "ELF (Ekonometrik)");

            // Call VEERProcess with skipPrerequisites flag
            var isImported = girdiModülü.VEERProcess(seçilenVeriTipi, skipPrerequisites);
            if (isImported)
            {
                veri_listesi_seçimi.Refresh();
                dataGridView_girdi.DataSource = girdiModülü.CurrentDataTable;
            }
        }


        // show information about polygons when double-clicking on the map
        private void gMapControl_stokastik_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                foreach (var polygon in cbs.tüm_katmanlar_array[checkbox_index].Polygons)
                {
                    if (cbs.IsPointInPolygon(pointClick, polygon))
                    {
                        cbs.HighlightPolygon(polygon, checkbox_index, cbs.GetActiveGMapControl());

                        if (cbs.polygonAttributes.TryGetValue(polygon, out DataRow row))
                        {
                            ShowAttributeRow(row);
                            tablo_formu.Show();
                        }
                    }
                }
            }
        }

        private void gMapControl_imar_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lastClickedCheckbox != null)
            {
                int checkbox_index = int.Parse(lastClickedCheckbox.Tag.ToString()) - 1;

                foreach (var polygon in cbs.tüm_katmanlar_array[checkbox_index].Polygons)
                {
                    if (cbs.IsPointInPolygon(pointClick, polygon))
                    {
                        cbs.HighlightPolygon(polygon, layer_index, cbs.GetActiveGMapControl());
                        layer_index = checkbox_index;
                        if (cbs.polygonAttributes.TryGetValue(polygon, out DataRow row))
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


        // ------------------------------------------------------------------------------------- //

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
            dataGridView_girdi.DataSource = girdiModülü.importedDataTable;
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

        private void HomePageButton_Click(object sender, EventArgs e)
        {
            HomePageForm homePageForm = new HomePageForm();
            homePageForm.Show();
            this.Hide();
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
                yearApproveButton.Text = "Sıfırla";
            }
        }


        /* -------------------------------------------------------------------------------------------*/


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
            istasyonAdetLabel.Text = $"AC istasyonlar: {greenAc - 1}, DC istasyonlar: {redDc - 1}";

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

                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Veri kaynağı bulunamadı. Lütfen verileri kontrol edin.");
                    return;
                }

                gMapControl_EA.Overlays.Clear();

                DataTable eaData = await Task.Run(() => DataGridViewToDataTable(dataGridView_girdi));

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
                            calculateChargeStation(greenAc, redDc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Bir hata ile karşılaştı: {ex.Message}");
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
            /*// Sadece "EA Şarj Modülü" tabına tıklandığında işlem yapalım
            if (Modül_Tabları.SelectedTab.Text == "EA Şarj Modülü")
            {
                if (dataGridView_girdi.DataSource == null)
                {
                    MessageBox.Show("Lütfen önce verileri yükleyin.");
                    return;
                }

                // Harita işlemini başlat
                await eaHaritayaVeriYukleAsync();
            }*/
            if (cbs.GetActiveGMapControl() != null)
            {
                cbs.GetActiveGMapControl().Refresh();
            }

        }

        private void buton_stokastik_harita_katmanlar_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                harita_katmanları_right_click.Show();
            } else
            {
                harita_katmanları_right_click.Hide();
            }
        }

        private void Sokak_Görünümü_Click(object sender, EventArgs e)
        {
            cbs.GetActiveGMapControl().Visible = false;
            cbs.GetActiveWebView().Visible = true;

            string url = "https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu";
            cbs.GetActiveWebView().CoreWebView2.Navigate(url);
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

        private void İmar_Poligon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Poligon.Show(Cursor.Position);
            }
        }

        // -------------------------------------------------------------------------------------------------- //

    }
}


