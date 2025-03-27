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
using SLF.services;
using SLF.Services;
using System.Reflection;

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


        private bool isDtrLoaded = false;






        // ------------------------------------------------------------------------------------------------------------ //
        // ---------------------------------------------- GENEL DEĞİŞKENLER ---------------------------------------------- //

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

        private bool aboneVerisiYuklendi = false;
        private bool dtrVerisiYuklendi = false;
        private bool eaVerisiYuklendi = false;

        private DTRModulu dtrmod = new DTRModulu();


        // Main constructor of the Modüller Formu 
        public ModülFormu(string selectedMethod = "", string tabToSelect = "")
        {
            // initialize the Modul Formu
            InitializeComponent();
            SetupLayout();


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


            // Resolve the Excel file path relative to SLF.exe
            string exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // e.g., C:\Users\ehan0\source\repos\emrehmrc\SLF\bin\Debug
            string projectRoot = Directory.GetParent(exeLocation)?.Parent?.FullName; // Move up two levels to SLF root (C:\Users\ehan0\source\repos\emrehmrc\SLF)
            if (projectRoot != null)
            {
                polygonTypesExcelPath = Path.Combine(projectRoot, "Excel Files", "point_load.xlsx"); // e.g., C:\Users\ehan0\source\repos\emrehmrc\SLF\Excel Files\point_load.xlsx
            }
            else
            {
                // Fallback to a default path if resolution fails
                polygonTypesExcelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "point_load.xlsx");
                MessageBox.Show($"Excel dosya yolu çözülemedi. Varsayılan yol kullanılıyor: {polygonTypesExcelPath}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


        }

        public ModülFormu() : this("", "")
        {
        }

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
                Console.WriteLine($"isİmportedModule: {seçilenVeriTipi} işlemi başarısız.");
                return;
            }

            if (!GirdiModülü.dataTablesByType.ContainsKey(seçilenVeriTipi))
            {
                Console.WriteLine($"isImportedModule: {seçilenVeriTipi} için tablo bulunamadı.");
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

                // Diğer statik koleksiyonları veya değişkenleri de temizle
                // ModülFormu.modulescheck?.Clear();
                // CBS sınıfındaki global değişkenler varsa onları da temizleyebilirsiniz

                Console.WriteLine("Global veri yapıları temizlendi");
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
        private bool SaveAsProject()
        {
            try
            {
                // Proje klasörünün ana dizini
                string projectsBaseDir = Path.Combine(PathService.BaseDirectory, PathService.FullPath);

                // Proje Ekle dialogunu göster
                using (var folderBrowser = new ProjectFolderPicker(projectsBaseDir))
                {
                    // Sadece mevcut projeleri seçmeye izin ver - bu seçeneği istemiyorsanız kaldırabilirsiniz
                    folderBrowser.EnableCreateProject = true;

                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = folderBrowser.SelectedPath;
                        string projectName = folderBrowser.SelectedProjectName;

                        // Projeyi aç
                        PathService.OpenProject(projectName);

                        // Proje durumunu kaydet ve verileri ekle
                        return UpdateCurrentProject();
                    }
                }

                // Kullanıcı iptal etti
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje kaydedilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private bool UpdateCurrentProject()
        {
            try
            {
                // YearService'ten yılları al
                var yearService = YearService.GetInstance();

                // Proje yolunu al
                string projectPath = Path.Combine(
                    PathService.BaseDirectory,
                    PathService.FullPath,
                    PathService.CurrentWorkingFolder);

                // Proje durumunu hazırla
                var projectState = new Dictionary<string, object>();
                projectState["CompletedModules"] = GirdiModülü.dataTablesByType.Keys.ToList();
                projectState["SLFStartYear"] = yearService.slfStartYear;
                projectState["SLFEndYear"] = yearService.slfEndYear;
                projectState["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Durum dosyasını güncelle
                string statePath = Path.Combine(projectPath, "project_state.json");
                string json = System.Text.Json.JsonSerializer.Serialize(projectState,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(statePath, json);

                // Modül verilerini kaydet
                SaveAllModuleDataToCSV(projectPath);

                // Başarılı
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje güncellenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Read-Only özniteliğini kaldıran yardımcı metot
        private void RemoveReadOnlyAttributes(string path)
        {
            // Dosyaların özniteliklerini değiştir
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fileInfo.Attributes &= ~FileAttributes.ReadOnly;
                }
            }

            // Alt klasörlerin özniteliklerini değiştir
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                if ((dirInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    dirInfo.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
        }

        private bool DirectoryHasContent(string path)
        {
            if (!Directory.Exists(path))
                return false;

            int fileCount = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
            return fileCount > 0;
        }

        // Eski geçici klasörleri temizleme metodu
        private void CleanupOldTempFolders()
        {
            try
            {
                if (string.IsNullOrEmpty(PathService.SelectedCity) ||
                    string.IsNullOrEmpty(PathService.SelectedDistrict))
                    return;

                string districtPath = Path.Combine(PathService.BaseDirectory, PathService.FullPath);

                if (!Directory.Exists(districtPath))
                    return;

                // "temp_" ile başlayan tüm klasörleri bul
                string[] tempFolders = Directory.GetDirectories(districtPath, "temp_*");

                foreach (string folder in tempFolders)
                {
                    // Aktif klasör değilse sil
                    if (PathService.CurrentMode != PathService.WorkingMode.Temporary ||
                        !folder.EndsWith(PathService.CurrentWorkingFolder))
                    {
                        try
                        {
                            Directory.Delete(folder, true);
                            Debug.WriteLine($"Eski geçici klasör silindi: {folder}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Klasör silinirken hata: {ex.Message}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eski klasörler temizlenirken hata: {ex.Message}");
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
                    else if (selectedTabText == "İmar Analizleri" && (!GirdiModülü.dataTablesByType.ContainsKey("İmar Planı")))
                    {
                        // Sekme geçişini tamamen iptal et
                        MessageBox.Show("İmar planı verileri yüklenmeden bu sekmeye geçiş yapılamaz.");
                        Modül_Tabları.SelectedIndexChanged -= Modül_Tabları_SelectedIndexChanged;
                        Modül_Tabları.SelectedTab = tab_girdi;
                        Modül_Tabları.SelectedIndexChanged += Modül_Tabları_SelectedIndexChanged;
                        return;
                    }
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


        private void LogDataTableInfo(DataTable dt, string source)
        {
            if (dt != null)
            {
                LogOutput($"Veri Kaynağı: {source}");
                LogOutput($"Satır Sayısı: {dt.Rows.Count}");
                LogOutput($"Kolon Sayısı: {dt.Columns.Count}");
                LogOutput($"Kolonlar: {string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
            }
            else
            {
                LogOutput($"Veri Kaynağı {source}: DataTable null");
            }
        }

        // TABLO OLUSTUR BUTONU
        private void button2_Click(object sender, EventArgs e)
        {
            // Tablonun bulunduğu formu oluştur ve göster
            Tablo_olustur tabloForm = new Tablo_olustur();
            tabloForm.Show();
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
                                //Properties.Settings.Default.LastProjectName = projectName;
                                //Properties.Settings.Default.LastProjectCity = PathService.SelectedCity;
                                //Properties.Settings.Default.LastProjectDistrict = PathService.SelectedDistrict;
                                //Properties.Settings.Default.Save();

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
        private void ClearLoadedData()
        {
            // GirdiModülü'ndeki veri tablolarını temizle
            GirdiModülü.dataTablesByType.Clear();

            // DataGridView'daki verileri temizle
            if (dataGridView_girdi != null)
            {
                dataGridView_girdi.DataSource = null;
            }

            // UI durumunu sıfırla
            if (veri_listesi_seçimi != null)
            {
                veri_listesi_seçimi.SelectedIndex = -1;
            }

            // Diğer UI elemanlarını sıfırla
            UpdateModuleButtonsState();
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


        /// Mevcut verileri proje olarak kaydet
        private bool SaveCurrentProject()
        {
            try
            {
                // YearService'ten yılları al
                var yearService = YearService.GetInstance();

                // Proje durumunu hazırla
                var projectState = new Dictionary<string, object>();
                projectState["CompletedModules"] = GirdiModülü.dataTablesByType.Keys.ToList();
                projectState["SLFStartYear"] = yearService.slfStartYear;
                projectState["SLFEndYear"] = yearService.slfEndYear;
                projectState["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Mevcut çalışma moduna göre işlem yap
                if (PathService.CurrentMode == PathService.WorkingMode.Project)
                {
                    // Projenin tam yolunu al
                    string projectPath = Path.Combine(
                        PathService.BaseDirectory,
                        PathService.FullPath,
                        PathService.CurrentWorkingFolder);

                    // Proje durum dosyasını güncelle
                    string statePath = Path.Combine(projectPath, "project_state.json");
                    string json = System.Text.Json.JsonSerializer.Serialize(projectState,
                        new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText(statePath, json);

                    // Tüm modül verilerini CSV olarak kaydet
                    SaveAllModuleDataToCSV(projectPath);

                    // Projenin adını al (proje_ ön ekini çıkar)
                    string projectName = PathService.CurrentWorkingFolder.StartsWith("proje_")
                        ? PathService.CurrentWorkingFolder.Substring(6)
                        : PathService.CurrentWorkingFolder;

                    MessageBox.Show($"Proje '{projectName}' başarıyla güncellendi.",
                        "Proje Güncellendi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
                else // Temporary mode - yeni proje oluşturma
                {
                    // Bu kısmı yine var olan ProjeEkleButton_Click metoduna yönlendirebilirsiniz
                    MessageBox.Show("Kaydedilecek bir proje açılmamış. Lütfen önce 'Proje Ekle' butonu ile bir proje açın.",
                        "Proje Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje kaydedilirken hata oluştu: {ex.Message}",
                    "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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

        private void DeepLearningModelButton_Click(object sender, EventArgs e)
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
        // ------------------------------------------ INITIALIZATION & GENERAL METHODS ------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

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
            panel_imar.Size = new Size(targetWidth, targetHeight);

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
            panel_imar.Location = new Point(targetX, targetY);

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
            tablo_formu = new Tablo_Formu();
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
                    checkBoxes[i].Location = new Point(checkBoxes[i].Location.X, currentY);
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
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- GİRDİ MODÜLÜ --------------------------------------------------- //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

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
            //string seçilenVeriTipi = veri_listesi_seçimi.SelectedItem.ToString();


            girdiModülü.slfStartYear = slfStartYear;
            girdiModülü.slfEndYear = slfEndYear;


            InitializeComboBoxes(); // yılların guncellenmesi 
                                    // Check if "ELF" is selected to skip prerequisites
            bool skipPrerequisites = (selectedMethod == "ELF (Ekonometrik)");

            // Call VEERProcess with skipPrerequisites flag
            var isImported = girdiModülü.VEERProcess(seçilenVeriTipi, skipPrerequisites);
            //dataGridView_girdi.DataSource = GirdiModülü.dataTablesByType[seçilenVeriTipi];
            //dataGridView_girdi.Refresh();
            //Console.WriteLine(isImported.ToString());
            //isİmportedModule(isImported, seçilenVeriTipi);
            //if (isImported)
            //{
            //    modulescheck.Add(seçilenVeriTipi);
            //    veri_listesi_seçimi.Refresh();
            //    Console.WriteLine(modulescheck.Count);
            //    dataGridView_girdi.DataSource = girdiModülü.CurrentDataTable;


            //}

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
                    Enlem = Math.Round(pointClick.Lat, 5),
                    Boylam = Math.Round(pointClick.Lng, 5)
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


        private void gMapControl_EA_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null && item.Tag is NoktaVeri && Modül_Tabları.SelectedTab == tab_ea)
            {
                NoktaVeri seçili_nokta = item.Tag as NoktaVeri;
                NoktaBilgileriniGoster(seçili_nokta);
            }
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
                SelectedSpeed = "varsayılan";
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

            // Use dataGridView1.DataSource as the DataTable instead of eaDataTable
            DataTable dataTable = dataGridView_girdi.DataSource as DataTable;
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
            // Refresh the map to show the new marker
            gMapControl_EA.Refresh();

            // Reset the flag after adding the station
            isAddingChargingStation = false;
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
            else if (SelectedCity == "İzmir" && SelectedSpeed == "varsayılan")
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
            else if (SelectedCity == "Eskişehir" && SelectedSpeed == "varsayılan")
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
            popupForm.Show(); // Yeni pencereyi göster
        }

        private Task HaritaUzerindeSimulasyonGosterimi(DataTable veriTablosu)
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
                    Enlem = Math.Round(pointClick.Lat, 5),
                    Boylam = Math.Round(pointClick.Lng, 5)
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
            gMapControl_yuk.Overlays.Add(cbs.tüm_katmanlar_array_yuk[0]);

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
        // ------------------------------------------------------------------------------------------------------------ //
        // -------------------------------------------- ELF ------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //
        // ------------------------------------------------------------------------------------------------------------ //

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

                string modifiedFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Arşiv\Modified_INPUT_FILE.xlsx";

                // Check if the modified file exists
                if (!File.Exists(modifiedFilePath))
                {
                    MessageBox.Show("Lütfen önce senaryo dosyasını ekleyin.");
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
            string rScriptPath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\Model\begum_model_deneme.R";
            string resultsFilePath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\Ekonometrik Yük Tahmini Verileri\Program\SONUÇLAR\";

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

            MessageBox.Show("Modeller başarıyla çalıştırıldı. " + resultsFilePath);
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
            // Always update the DataGridView with the DataTable
            tablo_formu.attribute_table.DataSource = datatable;

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
            if (layerIndex < 0 || layerIndex >= 15)
            {
                MessageBox.Show("Geçersiz katman endeksi.");
                return;
            }

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

        private void İmar_Nokta_MouseDown(object sender, MouseEventArgs e)
        {
            /*if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip_Nokta.Show(Cursor.Position);
            }*/
        }

        private void Point_Load_Çiz_Click(object sender, EventArgs e)
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

        private void YGA_Çiz_Click(object sender, EventArgs e)
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


        // Show just the single row whenever a polygon is clicked on which corresponds to its row
        private void ShowAttributeRow(DataRow row)
        {
            // Clone the structure of the original table and import the specific row
            DataTable singleRowTable = row.Table.Clone();
            singleRowTable.ImportRow(row);

            // Show the attribute table with just one row
            ShowAttributeTable(singleRowTable);

        }

    }
}