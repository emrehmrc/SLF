using System.Windows.Forms;

namespace SLF
{
    partial class ModülFormu
    {

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModülFormu));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Modül_Tabları = new System.Windows.Forms.TabControl();
            this.tab_girdi = new System.Windows.Forms.TabPage();
            this.panel_proje_ekle = new System.Windows.Forms.Panel();
            this.ProjeEkleButton = new System.Windows.Forms.Button();
            this.label_proje_ekle = new System.Windows.Forms.Label();
            this.OpenModuleButton = new System.Windows.Forms.Button();
            this.label_girdi_veri_onizleme = new System.Windows.Forms.Label();
            this.panel_girdi_rapor_olustur = new System.Windows.Forms.Panel();
            this.label_girdi_rapor = new System.Windows.Forms.Label();
            this.raporGoruntuleButonu = new System.Windows.Forms.Button();
            this.dataGridView_girdi = new System.Windows.Forms.DataGridView();
            this.panel_girdi_dısa_aktar = new System.Windows.Forms.Panel();
            this.label_dısa_aktar = new System.Windows.Forms.Label();
            this.ExcelDownloadButton = new System.Windows.Forms.Button();
            this.csvExportButton = new System.Windows.Forms.Button();
            this.panel_girdi_yıl_secimi = new System.Windows.Forms.Panel();
            this.startYearComboBox = new System.Windows.Forms.ComboBox();
            this.yearApproveButton = new System.Windows.Forms.Button();
            this.endYearComboBox = new System.Windows.Forms.ComboBox();
            this.panel_girdi_dosya_secimi = new System.Windows.Forms.Panel();
            this.veri_listesi_seçimi = new System.Windows.Forms.ComboBox();
            this.label_girdi_veri_tipi_secimi = new System.Windows.Forms.Label();
            this.label_girdi_dosya_secimi = new System.Windows.Forms.Label();
            this.SelectFolderButton = new System.Windows.Forms.Button();
            this.tab_dek = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CreateReportButton = new System.Windows.Forms.Button();
            this.DEKPointsLayerCheckBox = new System.Windows.Forms.CheckBox();
            this.DEKProgressBar = new System.Windows.Forms.ProgressBar();
            this.DEKStatusLabel = new System.Windows.Forms.Label();
            this.gMapControl_DEK = new GMap.NET.WindowsForms.GMapControl();
            this.panel_DEK = new System.Windows.Forms.Panel();
            this.DEKCenterAddButton = new System.Windows.Forms.Button();
            this.DEKSimulasyonSonucGoruntule = new System.Windows.Forms.Button();
            this.DEKRunSimulationButton = new System.Windows.Forms.Button();
            this.dekSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.dekSimDefBtn = new System.Windows.Forms.RadioButton();
            this.dekSimMinBtn = new System.Windows.Forms.RadioButton();
            this.DEKSimButton = new System.Windows.Forms.Button();
            this.label_DEK_Gelecek = new System.Windows.Forms.Label();
            this.comboBox_DEK_Yıl = new System.Windows.Forms.ComboBox();
            this.tab_ea = new System.Windows.Forms.TabPage();
            this.EAStationsLegendPanel = new System.Windows.Forms.Panel();
            this.SimulasyonSonucGoruntule = new System.Windows.Forms.Button();
            this.EANewSimulationResultsButton = new System.Windows.Forms.Button();
            this.EASimButton = new System.Windows.Forms.Button();
            this.EaSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.FutureSimLabel = new System.Windows.Forms.Label();
            this.EaSimDefBtn = new System.Windows.Forms.RadioButton();
            this.comboBox_ea_yıl_secimi = new System.Windows.Forms.ComboBox();
            this.EaSimMinBtn = new System.Windows.Forms.RadioButton();
            this.panel_ea = new System.Windows.Forms.Panel();
            this.CreateReportButton2 = new System.Windows.Forms.Button();
            this.EAPointsLayerCheckBox = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.GelecekSimPanel = new System.Windows.Forms.Panel();
            this.DCFastLegendValueLabel = new System.Windows.Forms.Label();
            this.DCFastLegendLabel = new System.Windows.Forms.Label();
            this.checkBox_DC_Fast = new System.Windows.Forms.CheckBox();
            this.checkBox_AC_Public = new System.Windows.Forms.CheckBox();
            this.ACPublicLegendValueLabel = new System.Windows.Forms.Label();
            this.ACPublicLegendLabel = new System.Windows.Forms.Label();
            this.checkBox_AC_Home = new System.Windows.Forms.CheckBox();
            this.AddStationLabel = new System.Windows.Forms.Label();
            this.checkBox_AC_Work = new System.Windows.Forms.CheckBox();
            this.ACWorkLegendValueLabel = new System.Windows.Forms.Label();
            this.EAStationAddButton = new System.Windows.Forms.Button();
            this.ACHomeLegendLabel = new System.Windows.Forms.Label();
            this.ACWorkLegendLabel = new System.Windows.Forms.Label();
            this.ACHomeLegendValueLabel = new System.Windows.Forms.Label();
            this.gMapControl_EA = new GMap.NET.WindowsForms.GMapControl();
            this.tab_imar = new System.Windows.Forms.TabPage();
            this.buton_SLF_tahmini = new System.Windows.Forms.Button();
            this.buton_abone_sayısı_tahmini = new System.Windows.Forms.Button();
            this.buton_imar_tahmini = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel_imar = new System.Windows.Forms.Panel();
            this.Mesafe_imar = new System.Windows.Forms.Label();
            this.webView_imar = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.buton_imar_katmanlar = new System.Windows.Forms.Button();
            this.harita_katmanları_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Arazi = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth_Desktop = new System.Windows.Forms.ToolStripMenuItem();
            this.Harita = new System.Windows.Forms.ToolStripMenuItem();
            this.OSM = new System.Windows.Forms.ToolStripMenuItem();
            this.Sokak_Görünümü = new System.Windows.Forms.ToolStripMenuItem();
            this.Uydu = new System.Windows.Forms.ToolStripMenuItem();
            this.mesafe_metre_imar = new System.Windows.Forms.Label();
            this.gMapControl_imar = new GMap.NET.WindowsForms.GMapControl();
            this.buton_DL_calıstır = new System.Windows.Forms.Button();
            this.checkBox_imar_15 = new System.Windows.Forms.CheckBox();
            this.katmanlar_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabloyuGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rengiDeğiştirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temizleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yenidenAdlandırToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox_imar_14 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_1 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_2 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_3 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_4 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_5 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_6 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_7 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_8 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_9 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_10 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_11 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_12 = new System.Windows.Forms.CheckBox();
            this.checkBox_imar_13 = new System.Windows.Forms.CheckBox();
            this.label_imar_katmanlar = new System.Windows.Forms.Label();
            this.imar_dosya_seçimi = new System.Windows.Forms.Button();
            this.toolStrip_imar = new System.Windows.Forms.ToolStrip();
            this.İmar_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.tab_optDTR = new System.Windows.Forms.TabPage();
            this.tab_ekonometrik = new System.Windows.Forms.TabPage();
            this.SenaryoModulePanel = new System.Windows.Forms.Panel();
            this.EkonometrikSenaryoElementsPanel = new System.Windows.Forms.Panel();
            this.buton_ELF_tablo_sec = new System.Windows.Forms.Button();
            this.textBox_sonuc_ELF = new System.Windows.Forms.TextBox();
            this.label_s_ELF = new System.Windows.Forms.Label();
            this.label_graphics = new System.Windows.Forms.Label();
            this.comboBox_ekonometrik = new System.Windows.Forms.ComboBox();
            this.ELFTahminButonu = new System.Windows.Forms.Button();
            this.ELFScenerioSaveButton = new System.Windows.Forms.Button();
            this.SenaryoModuleTabControl = new Guna.UI2.WinForms.Guna2TabControl();
            this.EkonometrikSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.ELFSenaryoTabControls = new System.Windows.Forms.TabControl();
            this.tabPage_min_senaryo = new System.Windows.Forms.TabPage();
            this.ELFMinSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage_dusuk_senaryo = new System.Windows.Forms.TabPage();
            this.ELFLowSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage_baz_senaryo = new System.Windows.Forms.TabPage();
            this.ELFBaseSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage_yuksek_senaryo = new System.Windows.Forms.TabPage();
            this.ELFHighSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage_maks_senaryo = new System.Windows.Forms.TabPage();
            this.ELFMaxSenaryoTable = new System.Windows.Forms.DataGridView();
            this.EkonometrikSonuclarTabPage = new System.Windows.Forms.TabPage();
            this.ELFSonuçlarTabControls = new System.Windows.Forms.TabControl();
            this.tabPage_min_sonuclar = new System.Windows.Forms.TabPage();
            this.ELFMinimumResultsTable = new System.Windows.Forms.DataGridView();
            this.tabPage_dusuk_sonuclar = new System.Windows.Forms.TabPage();
            this.ELFDüşükResultsTable = new System.Windows.Forms.DataGridView();
            this.tabPage_baz_sonuclar = new System.Windows.Forms.TabPage();
            this.ELFBazResultsTable = new System.Windows.Forms.DataGridView();
            this.tabPage_yuksek_sonuclar = new System.Windows.Forms.TabPage();
            this.ELFYüksekResultsTable = new System.Windows.Forms.DataGridView();
            this.tabPage_maks_sonuclar = new System.Windows.Forms.TabPage();
            this.ELFMaksimumResultsTable = new System.Windows.Forms.DataGridView();
            this.EkonometrikGrafiklerTabPage = new System.Windows.Forms.TabPage();
            this.pictureBox_ekonometrik = new System.Windows.Forms.PictureBox();
            this.tab_yükHaritası = new System.Windows.Forms.TabPage();
            this.buton_HTML = new System.Windows.Forms.PictureBox();
            this.checkBox_yuk_main = new System.Windows.Forms.CheckBox();
            this.panel_yuk = new System.Windows.Forms.Panel();
            this.webView_yuk = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gMapControl_yuk = new GMap.NET.WindowsForms.GMapControl();
            this.buton_yuk_haritası_katmanlar = new System.Windows.Forms.Button();
            this.checkBox_yuk_15 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_8 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_6 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_11 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_14 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_13 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_12 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_10 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_9 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_7 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_4 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_5 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_3 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_2 = new System.Windows.Forms.CheckBox();
            this.checkBox_yuk_1 = new System.Windows.Forms.CheckBox();
            this.legendPanel = new System.Windows.Forms.Panel();
            this.yuk_yıl_deger = new System.Windows.Forms.Label();
            this.yuk_yıl_text = new System.Windows.Forms.Label();
            this.trackBar_Yıllar = new System.Windows.Forms.TrackBar();
            this.tab_rapor = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.RModelProgressBar = new System.Windows.Forms.ProgressBar();
            this.RModelStatusLabel = new System.Windows.Forms.Label();
            this.GelecekSimButton = new System.Windows.Forms.Button();
            this.DeepLearningModelButton = new System.Windows.Forms.Button();
            this.YeniGenislemeSidePanel = new System.Windows.Forms.Panel();
            this.label_yga_katmanlar = new System.Windows.Forms.Label();
            this.FinishPolygonButton = new System.Windows.Forms.Button();
            this.YeniGenislemeMapPanel = new System.Windows.Forms.Panel();
            this.ButtonKml = new System.Windows.Forms.Button();
            this.oznitelikAc = new System.Windows.Forms.Button();
            this.EA_list_box = new System.Windows.Forms.ListBox();
            this.ELFRadioButtonsPanel = new System.Windows.Forms.Panel();
            this.ELFPredictionButton = new System.Windows.Forms.Button();
            this.SenaryoSelectionButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip_Poligon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.YGA_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Point_Load_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Kentsel_Donusum_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Enerji_Müsaadesi_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Poligon_Kaydet = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip_Fonksiyon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.katman_birleştir = new System.Windows.Forms.ToolStripMenuItem();
            this.ModuleTabPanel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.buton_tablo_olustur = new System.Windows.Forms.Button();
            this.buton_database_giris = new System.Windows.Forms.Button();
            this.HomePageButton = new System.Windows.Forms.Button();
            this.buton_proje_sec = new System.Windows.Forms.Button();
            this.Point_Load_Çiz = new System.Windows.Forms.ToolStripMenuItem();
            this.YGA_Çiz = new System.Windows.Forms.ToolStripMenuItem();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.buton_ea_harita_katmanlar = new System.Windows.Forms.Button();
            this.ELFMinSenaryoGraphPicBox = new System.Windows.Forms.PictureBox();
            this.Modül_Tabları.SuspendLayout();
            this.tab_girdi.SuspendLayout();
            this.panel_proje_ekle.SuspendLayout();
            this.panel_girdi_rapor_olustur.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_girdi)).BeginInit();
            this.panel_girdi_dısa_aktar.SuspendLayout();
            this.panel_girdi_yıl_secimi.SuspendLayout();
            this.panel_girdi_dosya_secimi.SuspendLayout();
            this.tab_dek.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_DEK.SuspendLayout();
            this.tab_ea.SuspendLayout();
            this.EAStationsLegendPanel.SuspendLayout();
            this.panel_ea.SuspendLayout();
            this.GelecekSimPanel.SuspendLayout();
            this.tab_imar.SuspendLayout();
            this.panel_imar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).BeginInit();
            this.harita_katmanları_right_click.SuspendLayout();
            this.katmanlar_right_click.SuspendLayout();
            this.toolStrip_imar.SuspendLayout();
            this.tab_ekonometrik.SuspendLayout();
            this.SenaryoModulePanel.SuspendLayout();
            this.EkonometrikSenaryoElementsPanel.SuspendLayout();
            this.SenaryoModuleTabControl.SuspendLayout();
            this.EkonometrikSenaryoTabPage.SuspendLayout();
            this.ELFSenaryoTabControls.SuspendLayout();
            this.tabPage_min_senaryo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoTable)).BeginInit();
            this.tabPage_dusuk_senaryo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowSenaryoTable)).BeginInit();
            this.tabPage_baz_senaryo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseSenaryoTable)).BeginInit();
            this.tabPage_yuksek_senaryo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighSenaryoTable)).BeginInit();
            this.tabPage_maks_senaryo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxSenaryoTable)).BeginInit();
            this.EkonometrikSonuclarTabPage.SuspendLayout();
            this.ELFSonuçlarTabControls.SuspendLayout();
            this.tabPage_min_sonuclar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinimumResultsTable)).BeginInit();
            this.tabPage_dusuk_sonuclar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFDüşükResultsTable)).BeginInit();
            this.tabPage_baz_sonuclar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFBazResultsTable)).BeginInit();
            this.tabPage_yuksek_sonuclar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFYüksekResultsTable)).BeginInit();
            this.tabPage_maks_sonuclar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaksimumResultsTable)).BeginInit();
            this.EkonometrikGrafiklerTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ekonometrik)).BeginInit();
            this.tab_yükHaritası.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buton_HTML)).BeginInit();
            this.panel_yuk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).BeginInit();
            this.ContextMenuStrip_Poligon.SuspendLayout();
            this.ContextMenuStrip_Fonksiyon.SuspendLayout();
            this.ModuleTabPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoGraphPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Modül_Tabları
            // 
            this.Modül_Tabları.Controls.Add(this.tab_girdi);
            this.Modül_Tabları.Controls.Add(this.tab_dek);
            this.Modül_Tabları.Controls.Add(this.tab_ea);
            this.Modül_Tabları.Controls.Add(this.tab_imar);
            this.Modül_Tabları.Controls.Add(this.tab_optDTR);
            this.Modül_Tabları.Controls.Add(this.tab_ekonometrik);
            this.Modül_Tabları.Controls.Add(this.tab_yükHaritası);
            this.Modül_Tabları.Controls.Add(this.tab_rapor);
            this.Modül_Tabları.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Modül_Tabları.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Modül_Tabları.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Modül_Tabları.HotTrack = true;
            this.Modül_Tabları.ImageList = this.imageList;
            this.Modül_Tabları.Location = new System.Drawing.Point(0, 0);
            this.Modül_Tabları.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Modül_Tabları.Multiline = true;
            this.Modül_Tabları.Name = "Modül_Tabları";
            this.Modül_Tabları.Padding = new System.Drawing.Point(20, 3);
            this.Modül_Tabları.SelectedIndex = 0;
            this.Modül_Tabları.Size = new System.Drawing.Size(1320, 727);
            this.Modül_Tabları.TabIndex = 2;
            this.Modül_Tabları.SelectedIndexChanged += new System.EventHandler(this.Modül_Tabları_SelectedIndexChanged);
            this.Modül_Tabları.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.Modül_Tabları_Selecting);
            // 
            // tab_girdi
            // 
            this.tab_girdi.AutoScroll = true;
            this.tab_girdi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tab_girdi.Controls.Add(this.panel_proje_ekle);
            this.tab_girdi.Controls.Add(this.OpenModuleButton);
            this.tab_girdi.Controls.Add(this.label_girdi_veri_onizleme);
            this.tab_girdi.Controls.Add(this.panel_girdi_rapor_olustur);
            this.tab_girdi.Controls.Add(this.dataGridView_girdi);
            this.tab_girdi.Controls.Add(this.panel_girdi_dısa_aktar);
            this.tab_girdi.Controls.Add(this.panel_girdi_yıl_secimi);
            this.tab_girdi.Controls.Add(this.panel_girdi_dosya_secimi);
            this.tab_girdi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_girdi.ImageIndex = 12;
            this.tab_girdi.Location = new System.Drawing.Point(4, 56);
            this.tab_girdi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_girdi.Name = "tab_girdi";
            this.tab_girdi.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_girdi.Size = new System.Drawing.Size(1312, 667);
            this.tab_girdi.TabIndex = 0;
            this.tab_girdi.Text = "Girdi Modülü";
            this.tab_girdi.UseVisualStyleBackColor = true;
            // 
            // panel_proje_ekle
            // 
            this.panel_proje_ekle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_proje_ekle.Controls.Add(this.ProjeEkleButton);
            this.panel_proje_ekle.Controls.Add(this.label_proje_ekle);
            this.panel_proje_ekle.Location = new System.Drawing.Point(809, 2);
            this.panel_proje_ekle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_proje_ekle.Name = "panel_proje_ekle";
            this.panel_proje_ekle.Size = new System.Drawing.Size(192, 69);
            this.panel_proje_ekle.TabIndex = 20;
            // 
            // ProjeEkleButton
            // 
            this.ProjeEkleButton.BackColor = System.Drawing.Color.White;
            this.ProjeEkleButton.BackgroundImage = global::SLF.Properties.Resources.download_folder_file_icon_219533;
            this.ProjeEkleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ProjeEkleButton.ForeColor = System.Drawing.Color.Transparent;
            this.ProjeEkleButton.Location = new System.Drawing.Point(129, 15);
            this.ProjeEkleButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ProjeEkleButton.Name = "ProjeEkleButton";
            this.ProjeEkleButton.Size = new System.Drawing.Size(47, 44);
            this.ProjeEkleButton.TabIndex = 8;
            this.ProjeEkleButton.UseVisualStyleBackColor = false;
            this.ProjeEkleButton.Click += new System.EventHandler(this.ProjeEkleButton_Click);
            // 
            // label_proje_ekle
            // 
            this.label_proje_ekle.AutoSize = true;
            this.label_proje_ekle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_proje_ekle.Location = new System.Drawing.Point(3, 5);
            this.label_proje_ekle.Name = "label_proje_ekle";
            this.label_proje_ekle.Size = new System.Drawing.Size(88, 23);
            this.label_proje_ekle.TabIndex = 7;
            this.label_proje_ekle.Text = "Proje Ekle:";
            // 
            // OpenModuleButton
            // 
            this.OpenModuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenModuleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OpenModuleButton.FlatAppearance.BorderSize = 0;
            this.OpenModuleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenModuleButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.OpenModuleButton.ForeColor = System.Drawing.Color.White;
            this.OpenModuleButton.Location = new System.Drawing.Point(1196, 608);
            this.OpenModuleButton.Margin = new System.Windows.Forms.Padding(4);
            this.OpenModuleButton.Name = "OpenModuleButton";
            this.OpenModuleButton.Size = new System.Drawing.Size(107, 39);
            this.OpenModuleButton.TabIndex = 19;
            this.OpenModuleButton.Text = "Modüle Git";
            this.OpenModuleButton.UseVisualStyleBackColor = false;
            this.OpenModuleButton.Click += new System.EventHandler(this.OpenModuleButton_Click);
            // 
            // label_girdi_veri_onizleme
            // 
            this.label_girdi_veri_onizleme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_girdi_veri_onizleme.AutoSize = true;
            this.label_girdi_veri_onizleme.Font = new System.Drawing.Font("Maiandra GD", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_veri_onizleme.Location = new System.Drawing.Point(3, 78);
            this.label_girdi_veri_onizleme.Name = "label_girdi_veri_onizleme";
            this.label_girdi_veri_onizleme.Size = new System.Drawing.Size(118, 20);
            this.label_girdi_veri_onizleme.TabIndex = 5;
            this.label_girdi_veri_onizleme.Text = "Veri Önizleme:";
            // 
            // panel_girdi_rapor_olustur
            // 
            this.panel_girdi_rapor_olustur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_girdi_rapor_olustur.Controls.Add(this.label_girdi_rapor);
            this.panel_girdi_rapor_olustur.Controls.Add(this.raporGoruntuleButonu);
            this.panel_girdi_rapor_olustur.Location = new System.Drawing.Point(1388, 4);
            this.panel_girdi_rapor_olustur.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_girdi_rapor_olustur.Name = "panel_girdi_rapor_olustur";
            this.panel_girdi_rapor_olustur.Size = new System.Drawing.Size(192, 65);
            this.panel_girdi_rapor_olustur.TabIndex = 15;
            // 
            // label_girdi_rapor
            // 
            this.label_girdi_rapor.AutoSize = true;
            this.label_girdi_rapor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_rapor.Location = new System.Drawing.Point(3, 5);
            this.label_girdi_rapor.Name = "label_girdi_rapor";
            this.label_girdi_rapor.Size = new System.Drawing.Size(120, 23);
            this.label_girdi_rapor.TabIndex = 7;
            this.label_girdi_rapor.Text = "Rapor Oluştur:";
            // 
            // raporGoruntuleButonu
            // 
            this.raporGoruntuleButonu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.raporGoruntuleButonu.BackColor = System.Drawing.Color.White;
            this.raporGoruntuleButonu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("raporGoruntuleButonu.BackgroundImage")));
            this.raporGoruntuleButonu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.raporGoruntuleButonu.ForeColor = System.Drawing.Color.Transparent;
            this.raporGoruntuleButonu.Location = new System.Drawing.Point(131, 14);
            this.raporGoruntuleButonu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.raporGoruntuleButonu.Name = "raporGoruntuleButonu";
            this.raporGoruntuleButonu.Size = new System.Drawing.Size(43, 36);
            this.raporGoruntuleButonu.TabIndex = 6;
            this.raporGoruntuleButonu.UseVisualStyleBackColor = false;
            this.raporGoruntuleButonu.Click += new System.EventHandler(this.raporGoruntuleButonu_Click);
            // 
            // dataGridView_girdi
            // 
            this.dataGridView_girdi.AllowUserToAddRows = false;
            this.dataGridView_girdi.AllowUserToDeleteRows = false;
            this.dataGridView_girdi.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_girdi.BackgroundColor = System.Drawing.Color.Snow;
            this.dataGridView_girdi.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_girdi.ColumnHeadersHeight = 29;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_girdi.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_girdi.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dataGridView_girdi.Location = new System.Drawing.Point(3, 113);
            this.dataGridView_girdi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView_girdi.Name = "dataGridView_girdi";
            this.dataGridView_girdi.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_girdi.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_girdi.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.dataGridView_girdi.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_girdi.RowTemplate.Height = 24;
            this.dataGridView_girdi.Size = new System.Drawing.Size(1282, 489);
            this.dataGridView_girdi.TabIndex = 4;
            // 
            // panel_girdi_dısa_aktar
            // 
            this.panel_girdi_dısa_aktar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_girdi_dısa_aktar.Controls.Add(this.label_dısa_aktar);
            this.panel_girdi_dısa_aktar.Controls.Add(this.ExcelDownloadButton);
            this.panel_girdi_dısa_aktar.Controls.Add(this.csvExportButton);
            this.panel_girdi_dısa_aktar.Location = new System.Drawing.Point(1074, 4);
            this.panel_girdi_dısa_aktar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_girdi_dısa_aktar.Name = "panel_girdi_dısa_aktar";
            this.panel_girdi_dısa_aktar.Size = new System.Drawing.Size(211, 65);
            this.panel_girdi_dısa_aktar.TabIndex = 14;
            // 
            // label_dısa_aktar
            // 
            this.label_dısa_aktar.AutoSize = true;
            this.label_dısa_aktar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_dısa_aktar.Location = new System.Drawing.Point(3, 2);
            this.label_dısa_aktar.Name = "label_dısa_aktar";
            this.label_dısa_aktar.Size = new System.Drawing.Size(91, 23);
            this.label_dısa_aktar.TabIndex = 16;
            this.label_dısa_aktar.Text = "Dışa Aktar:";
            // 
            // ExcelDownloadButton
            // 
            this.ExcelDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExcelDownloadButton.BackColor = System.Drawing.Color.White;
            this.ExcelDownloadButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ExcelDownloadButton.BackgroundImage")));
            this.ExcelDownloadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExcelDownloadButton.ForeColor = System.Drawing.Color.Transparent;
            this.ExcelDownloadButton.Location = new System.Drawing.Point(105, 14);
            this.ExcelDownloadButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExcelDownloadButton.Name = "ExcelDownloadButton";
            this.ExcelDownloadButton.Size = new System.Drawing.Size(43, 36);
            this.ExcelDownloadButton.TabIndex = 7;
            this.ExcelDownloadButton.UseVisualStyleBackColor = false;
            this.ExcelDownloadButton.Click += new System.EventHandler(this.ExcelDownloadButton_Click);
            // 
            // csvExportButton
            // 
            this.csvExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.csvExportButton.BackColor = System.Drawing.Color.White;
            this.csvExportButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("csvExportButton.BackgroundImage")));
            this.csvExportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.csvExportButton.ForeColor = System.Drawing.Color.Transparent;
            this.csvExportButton.Location = new System.Drawing.Point(153, 14);
            this.csvExportButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.csvExportButton.Name = "csvExportButton";
            this.csvExportButton.Size = new System.Drawing.Size(43, 36);
            this.csvExportButton.TabIndex = 8;
            this.csvExportButton.UseVisualStyleBackColor = true;
            this.csvExportButton.Click += new System.EventHandler(this.csvExportButton_Click);
            // 
            // panel_girdi_yıl_secimi
            // 
            this.panel_girdi_yıl_secimi.Controls.Add(this.startYearComboBox);
            this.panel_girdi_yıl_secimi.Controls.Add(this.yearApproveButton);
            this.panel_girdi_yıl_secimi.Controls.Add(this.endYearComboBox);
            this.panel_girdi_yıl_secimi.Location = new System.Drawing.Point(5, 2);
            this.panel_girdi_yıl_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_girdi_yıl_secimi.Name = "panel_girdi_yıl_secimi";
            this.panel_girdi_yıl_secimi.Size = new System.Drawing.Size(279, 71);
            this.panel_girdi_yıl_secimi.TabIndex = 12;
            // 
            // startYearComboBox
            // 
            this.startYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.startYearComboBox.FormattingEnabled = true;
            this.startYearComboBox.Location = new System.Drawing.Point(4, 4);
            this.startYearComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.startYearComboBox.Name = "startYearComboBox";
            this.startYearComboBox.Size = new System.Drawing.Size(101, 29);
            this.startYearComboBox.TabIndex = 9;
            this.startYearComboBox.Text = "Yıl seçiniz";
            this.startYearComboBox.SelectedIndexChanged += new System.EventHandler(this.startYearComboBox_SelectedIndexChanged);
            // 
            // yearApproveButton
            // 
            this.yearApproveButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.yearApproveButton.Location = new System.Drawing.Point(85, 37);
            this.yearApproveButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.yearApproveButton.Name = "yearApproveButton";
            this.yearApproveButton.Size = new System.Drawing.Size(100, 32);
            this.yearApproveButton.TabIndex = 11;
            this.yearApproveButton.Text = "Onayla";
            this.yearApproveButton.UseVisualStyleBackColor = true;
            this.yearApproveButton.Click += new System.EventHandler(this.yearApproveButton_Click);
            // 
            // endYearComboBox
            // 
            this.endYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.endYearComboBox.FormattingEnabled = true;
            this.endYearComboBox.Location = new System.Drawing.Point(168, 2);
            this.endYearComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.endYearComboBox.Name = "endYearComboBox";
            this.endYearComboBox.Size = new System.Drawing.Size(101, 29);
            this.endYearComboBox.TabIndex = 10;
            this.endYearComboBox.Text = "Yıl seçiniz";
            this.endYearComboBox.SelectedIndexChanged += new System.EventHandler(this.endYearComboBox_SelectedIndexChanged);
            // 
            // panel_girdi_dosya_secimi
            // 
            this.panel_girdi_dosya_secimi.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel_girdi_dosya_secimi.Controls.Add(this.veri_listesi_seçimi);
            this.panel_girdi_dosya_secimi.Controls.Add(this.label_girdi_veri_tipi_secimi);
            this.panel_girdi_dosya_secimi.Controls.Add(this.label_girdi_dosya_secimi);
            this.panel_girdi_dosya_secimi.Controls.Add(this.SelectFolderButton);
            this.panel_girdi_dosya_secimi.Location = new System.Drawing.Point(320, 4);
            this.panel_girdi_dosya_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_girdi_dosya_secimi.Name = "panel_girdi_dosya_secimi";
            this.panel_girdi_dosya_secimi.Size = new System.Drawing.Size(445, 69);
            this.panel_girdi_dosya_secimi.TabIndex = 13;
            // 
            // veri_listesi_seçimi
            // 
            this.veri_listesi_seçimi.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.veri_listesi_seçimi.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.veri_listesi_seçimi.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.veri_listesi_seçimi.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.veri_listesi_seçimi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.veri_listesi_seçimi.Font = new System.Drawing.Font("Maiandra GD", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.veri_listesi_seçimi.FormattingEnabled = true;
            this.veri_listesi_seçimi.Items.AddRange(new object[] {
            "Abone Verileri",
            "DEK Verileri",
            "DTR Verileri",
            "EA Şarj Verileri",
            "Ekonometrik Yük Tahmini Verileri",
            "Fider Verileri",
            "İmar Verileri",
            "Enerji Müsaadeleri Verileri",
            "Yeni Projelendirilmiş DTR Verileri"});
            this.veri_listesi_seçimi.Location = new System.Drawing.Point(52, 27);
            this.veri_listesi_seçimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.veri_listesi_seçimi.Name = "veri_listesi_seçimi";
            this.veri_listesi_seçimi.Size = new System.Drawing.Size(213, 31);
            this.veri_listesi_seçimi.TabIndex = 0;
            this.veri_listesi_seçimi.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.veri_listesi_seçimi_DrawItem);
            this.veri_listesi_seçimi.SelectedIndexChanged += new System.EventHandler(this.veri_listesi_seçimi_SelectedIndexChanged);
            // 
            // label_girdi_veri_tipi_secimi
            // 
            this.label_girdi_veri_tipi_secimi.AutoSize = true;
            this.label_girdi_veri_tipi_secimi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_veri_tipi_secimi.Location = new System.Drawing.Point(3, 1);
            this.label_girdi_veri_tipi_secimi.Name = "label_girdi_veri_tipi_secimi";
            this.label_girdi_veri_tipi_secimi.Size = new System.Drawing.Size(180, 23);
            this.label_girdi_veri_tipi_secimi.TabIndex = 1;
            this.label_girdi_veri_tipi_secimi.Text = "Dosya Veri Tipi Seçimi:";
            // 
            // label_girdi_dosya_secimi
            // 
            this.label_girdi_dosya_secimi.AutoSize = true;
            this.label_girdi_dosya_secimi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_dosya_secimi.Location = new System.Drawing.Point(263, 5);
            this.label_girdi_dosya_secimi.Name = "label_girdi_dosya_secimi";
            this.label_girdi_dosya_secimi.Size = new System.Drawing.Size(114, 23);
            this.label_girdi_dosya_secimi.TabIndex = 3;
            this.label_girdi_dosya_secimi.Text = "Dosya Seçimi:";
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.BackColor = System.Drawing.Color.White;
            this.SelectFolderButton.BackgroundImage = global::SLF.Properties.Resources.download_folder_file_icon_219533;
            this.SelectFolderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectFolderButton.ForeColor = System.Drawing.Color.Transparent;
            this.SelectFolderButton.Location = new System.Drawing.Point(385, 15);
            this.SelectFolderButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(47, 44);
            this.SelectFolderButton.TabIndex = 2;
            this.SelectFolderButton.UseVisualStyleBackColor = false;
            this.SelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // tab_dek
            // 
            this.tab_dek.Controls.Add(this.panel1);
            this.tab_dek.Controls.Add(this.gMapControl_DEK);
            this.tab_dek.Controls.Add(this.panel_DEK);
            this.tab_dek.ImageIndex = 0;
            this.tab_dek.Location = new System.Drawing.Point(4, 30);
            this.tab_dek.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_dek.Name = "tab_dek";
            this.tab_dek.Size = new System.Drawing.Size(1312, 693);
            this.tab_dek.TabIndex = 6;
            this.tab_dek.Text = "DEK Modülü";
            this.tab_dek.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel1.Controls.Add(this.CreateReportButton);
            this.panel1.Controls.Add(this.DEKPointsLayerCheckBox);
            this.panel1.Controls.Add(this.DEKProgressBar);
            this.panel1.Controls.Add(this.DEKStatusLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1031, 43);
            this.panel1.TabIndex = 45;
            // 
            // CreateReportButton
            // 
            this.CreateReportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.CreateReportButton.FlatAppearance.BorderSize = 0;
            this.CreateReportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CreateReportButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.CreateReportButton.ForeColor = System.Drawing.Color.Snow;
            this.CreateReportButton.Location = new System.Drawing.Point(600, 7);
            this.CreateReportButton.Name = "CreateReportButton";
            this.CreateReportButton.Size = new System.Drawing.Size(129, 31);
            this.CreateReportButton.TabIndex = 63;
            this.CreateReportButton.Text = "Rapor Oluştur";
            this.CreateReportButton.UseVisualStyleBackColor = false;
            this.CreateReportButton.Click += new System.EventHandler(this.CreateReportButton_Click);
            // 
            // DEKPointsLayerCheckBox
            // 
            this.DEKPointsLayerCheckBox.AutoSize = true;
            this.DEKPointsLayerCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DEKPointsLayerCheckBox.Checked = true;
            this.DEKPointsLayerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DEKPointsLayerCheckBox.Location = new System.Drawing.Point(31, 9);
            this.DEKPointsLayerCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DEKPointsLayerCheckBox.Name = "DEKPointsLayerCheckBox";
            this.DEKPointsLayerCheckBox.Size = new System.Drawing.Size(138, 27);
            this.DEKPointsLayerCheckBox.TabIndex = 61;
            this.DEKPointsLayerCheckBox.Text = "DEK Noktaları";
            this.DEKPointsLayerCheckBox.UseVisualStyleBackColor = true;
            this.DEKPointsLayerCheckBox.CheckedChanged += new System.EventHandler(this.DEKPointsLayerCheckBox_CheckedChanged);
            // 
            // DEKProgressBar
            // 
            this.DEKProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKProgressBar.Location = new System.Drawing.Point(872, 11);
            this.DEKProgressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DEKProgressBar.MarqueeAnimationSpeed = 200;
            this.DEKProgressBar.Name = "DEKProgressBar";
            this.DEKProgressBar.Size = new System.Drawing.Size(148, 22);
            this.DEKProgressBar.TabIndex = 60;
            this.DEKProgressBar.Visible = false;
            // 
            // DEKStatusLabel
            // 
            this.DEKStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKStatusLabel.AutoSize = true;
            this.DEKStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKStatusLabel.Location = new System.Drawing.Point(772, 14);
            this.DEKStatusLabel.Name = "DEKStatusLabel";
            this.DEKStatusLabel.Size = new System.Drawing.Size(50, 19);
            this.DEKStatusLabel.TabIndex = 59;
            this.DEKStatusLabel.Text = "Status:";
            this.DEKStatusLabel.Visible = false;
            // 
            // gMapControl_DEK
            // 
            this.gMapControl_DEK.AllowDrop = true;
            this.gMapControl_DEK.Bearing = 0F;
            this.gMapControl_DEK.CanDragMap = true;
            this.gMapControl_DEK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_DEK.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_DEK.GrayScaleMode = false;
            this.gMapControl_DEK.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_DEK.LevelsKeepInMemory = 5;
            this.gMapControl_DEK.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_DEK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gMapControl_DEK.MarkersEnabled = true;
            this.gMapControl_DEK.MaxZoom = 2;
            this.gMapControl_DEK.MinZoom = 2;
            this.gMapControl_DEK.MouseWheelZoomEnabled = true;
            this.gMapControl_DEK.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_DEK.Name = "gMapControl_DEK";
            this.gMapControl_DEK.NegativeMode = false;
            this.gMapControl_DEK.PolygonsEnabled = true;
            this.gMapControl_DEK.RetryLoadTile = 0;
            this.gMapControl_DEK.RoutesEnabled = true;
            this.gMapControl_DEK.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_DEK.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_DEK.ShowTileGridLines = false;
            this.gMapControl_DEK.Size = new System.Drawing.Size(1031, 693);
            this.gMapControl_DEK.TabIndex = 38;
            this.gMapControl_DEK.Zoom = 0D;
            this.gMapControl_DEK.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_DEK_OnMapClick);
            this.gMapControl_DEK.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_Dek_OnMarkerClick);
            // 
            // panel_DEK
            // 
            this.panel_DEK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_DEK.Controls.Add(this.DEKCenterAddButton);
            this.panel_DEK.Controls.Add(this.DEKSimulasyonSonucGoruntule);
            this.panel_DEK.Controls.Add(this.DEKRunSimulationButton);
            this.panel_DEK.Controls.Add(this.dekSimMaxBtn);
            this.panel_DEK.Controls.Add(this.dekSimDefBtn);
            this.panel_DEK.Controls.Add(this.dekSimMinBtn);
            this.panel_DEK.Controls.Add(this.DEKSimButton);
            this.panel_DEK.Controls.Add(this.label_DEK_Gelecek);
            this.panel_DEK.Controls.Add(this.comboBox_DEK_Yıl);
            this.panel_DEK.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_DEK.Location = new System.Drawing.Point(1031, 0);
            this.panel_DEK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_DEK.Name = "panel_DEK";
            this.panel_DEK.Size = new System.Drawing.Size(281, 693);
            this.panel_DEK.TabIndex = 37;
            // 
            // DEKCenterAddButton
            // 
            this.DEKCenterAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKCenterAddButton.Enabled = false;
            this.DEKCenterAddButton.FlatAppearance.BorderSize = 0;
            this.DEKCenterAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKCenterAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCenterAddButton.ForeColor = System.Drawing.Color.White;
            this.DEKCenterAddButton.Location = new System.Drawing.Point(75, 564);
            this.DEKCenterAddButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DEKCenterAddButton.Name = "DEKCenterAddButton";
            this.DEKCenterAddButton.Size = new System.Drawing.Size(144, 64);
            this.DEKCenterAddButton.TabIndex = 55;
            this.DEKCenterAddButton.Text = "Dagıtık Üretim Merkezi Ekle ";
            this.DEKCenterAddButton.UseVisualStyleBackColor = false;
            this.DEKCenterAddButton.Click += new System.EventHandler(this.DEKCenterAddButton_Click);
            // 
            // DEKSimulasyonSonucGoruntule
            // 
            this.DEKSimulasyonSonucGoruntule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimulasyonSonucGoruntule.FlatAppearance.BorderSize = 0;
            this.DEKSimulasyonSonucGoruntule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKSimulasyonSonucGoruntule.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKSimulasyonSonucGoruntule.ForeColor = System.Drawing.Color.White;
            this.DEKSimulasyonSonucGoruntule.Location = new System.Drawing.Point(75, 200);
            this.DEKSimulasyonSonucGoruntule.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DEKSimulasyonSonucGoruntule.Name = "DEKSimulasyonSonucGoruntule";
            this.DEKSimulasyonSonucGoruntule.Size = new System.Drawing.Size(140, 66);
            this.DEKSimulasyonSonucGoruntule.TabIndex = 62;
            this.DEKSimulasyonSonucGoruntule.Text = "Sonuçları Getir";
            this.DEKSimulasyonSonucGoruntule.UseVisualStyleBackColor = false;
            this.DEKSimulasyonSonucGoruntule.Click += new System.EventHandler(this.DEKSimulasyonSonucGoruntule_Click);
            // 
            // DEKRunSimulationButton
            // 
            this.DEKRunSimulationButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKRunSimulationButton.FlatAppearance.BorderSize = 0;
            this.DEKRunSimulationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKRunSimulationButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKRunSimulationButton.ForeColor = System.Drawing.Color.White;
            this.DEKRunSimulationButton.Location = new System.Drawing.Point(56, 58);
            this.DEKRunSimulationButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DEKRunSimulationButton.Name = "DEKRunSimulationButton";
            this.DEKRunSimulationButton.Size = new System.Drawing.Size(176, 64);
            this.DEKRunSimulationButton.TabIndex = 61;
            this.DEKRunSimulationButton.Text = "DEK Gelecek Simülasyonu Oluştur";
            this.DEKRunSimulationButton.UseVisualStyleBackColor = false;
            this.DEKRunSimulationButton.Click += new System.EventHandler(this.DEKRunSimulationButton_Click);
            // 
            // dekSimMaxBtn
            // 
            this.dekSimMaxBtn.AutoSize = true;
            this.dekSimMaxBtn.Location = new System.Drawing.Point(74, 386);
            this.dekSimMaxBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dekSimMaxBtn.Name = "dekSimMaxBtn";
            this.dekSimMaxBtn.Size = new System.Drawing.Size(132, 27);
            this.dekSimMaxBtn.TabIndex = 59;
            this.dekSimMaxBtn.TabStop = true;
            this.dekSimMaxBtn.Text = "Hızlı Senaryo";
            this.dekSimMaxBtn.UseVisualStyleBackColor = true;
            this.dekSimMaxBtn.CheckedChanged += new System.EventHandler(this.dekSimMaxBtn_CheckedChanged);
            // 
            // dekSimDefBtn
            // 
            this.dekSimDefBtn.AutoSize = true;
            this.dekSimDefBtn.Location = new System.Drawing.Point(74, 358);
            this.dekSimDefBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dekSimDefBtn.Name = "dekSimDefBtn";
            this.dekSimDefBtn.Size = new System.Drawing.Size(176, 27);
            this.dekSimDefBtn.TabIndex = 58;
            this.dekSimDefBtn.TabStop = true;
            this.dekSimDefBtn.Text = "Varsayılan Senaryo";
            this.dekSimDefBtn.UseVisualStyleBackColor = true;
            this.dekSimDefBtn.CheckedChanged += new System.EventHandler(this.dekSimDefBtn_CheckedChanged);
            // 
            // dekSimMinBtn
            // 
            this.dekSimMinBtn.AutoSize = true;
            this.dekSimMinBtn.Location = new System.Drawing.Point(74, 331);
            this.dekSimMinBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dekSimMinBtn.Name = "dekSimMinBtn";
            this.dekSimMinBtn.Size = new System.Drawing.Size(141, 27);
            this.dekSimMinBtn.TabIndex = 57;
            this.dekSimMinBtn.TabStop = true;
            this.dekSimMinBtn.Text = "Yavaş Senaryo";
            this.dekSimMinBtn.UseVisualStyleBackColor = true;
            this.dekSimMinBtn.CheckedChanged += new System.EventHandler(this.dekSimMinBtn_CheckedChanged);
            // 
            // DEKSimButton
            // 
            this.DEKSimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimButton.FlatAppearance.BorderSize = 0;
            this.DEKSimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKSimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKSimButton.ForeColor = System.Drawing.Color.White;
            this.DEKSimButton.Location = new System.Drawing.Point(53, 457);
            this.DEKSimButton.Margin = new System.Windows.Forms.Padding(4);
            this.DEKSimButton.Name = "DEKSimButton";
            this.DEKSimButton.Size = new System.Drawing.Size(187, 78);
            this.DEKSimButton.TabIndex = 54;
            this.DEKSimButton.Text = "DEK Senaryo Sonuçları Görüntüle";
            this.DEKSimButton.UseVisualStyleBackColor = false;
            this.DEKSimButton.Click += new System.EventHandler(this.dekSimulasyonGoruntule);
            // 
            // label_DEK_Gelecek
            // 
            this.label_DEK_Gelecek.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_DEK_Gelecek.AutoSize = true;
            this.label_DEK_Gelecek.BackColor = System.Drawing.Color.DarkOrange;
            this.label_DEK_Gelecek.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_DEK_Gelecek.Location = new System.Drawing.Point(36, 9);
            this.label_DEK_Gelecek.Name = "label_DEK_Gelecek";
            this.label_DEK_Gelecek.Size = new System.Drawing.Size(208, 23);
            this.label_DEK_Gelecek.TabIndex = 3;
            this.label_DEK_Gelecek.Text = "DEK Gelecek Simülasyonu";
            // 
            // comboBox_DEK_Yıl
            // 
            this.comboBox_DEK_Yıl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.comboBox_DEK_Yıl.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_DEK_Yıl.FormattingEnabled = true;
            this.comboBox_DEK_Yıl.Location = new System.Drawing.Point(113, 145);
            this.comboBox_DEK_Yıl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_DEK_Yıl.Name = "comboBox_DEK_Yıl";
            this.comboBox_DEK_Yıl.Size = new System.Drawing.Size(69, 29);
            this.comboBox_DEK_Yıl.TabIndex = 0;
            this.comboBox_DEK_Yıl.Text = "YIL";
            this.comboBox_DEK_Yıl.SelectedIndexChanged += new System.EventHandler(this.dek_list_years);
            // 
            // tab_ea
            // 
            this.tab_ea.Controls.Add(this.EAStationsLegendPanel);
            this.tab_ea.Controls.Add(this.panel_ea);
            this.tab_ea.Controls.Add(this.GelecekSimPanel);
            this.tab_ea.Controls.Add(this.gMapControl_EA);
            this.tab_ea.ImageIndex = 2;
            this.tab_ea.Location = new System.Drawing.Point(4, 30);
            this.tab_ea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ea.Name = "tab_ea";
            this.tab_ea.Size = new System.Drawing.Size(1312, 693);
            this.tab_ea.TabIndex = 5;
            this.tab_ea.Text = "EA Şarj Modülü";
            this.tab_ea.UseVisualStyleBackColor = true;
            // 
            // EAStationsLegendPanel
            // 
            this.EAStationsLegendPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EAStationsLegendPanel.Controls.Add(this.SimulasyonSonucGoruntule);
            this.EAStationsLegendPanel.Controls.Add(this.EANewSimulationResultsButton);
            this.EAStationsLegendPanel.Controls.Add(this.EASimButton);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimMaxBtn);
            this.EAStationsLegendPanel.Controls.Add(this.FutureSimLabel);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimDefBtn);
            this.EAStationsLegendPanel.Controls.Add(this.comboBox_ea_yıl_secimi);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimMinBtn);
            this.EAStationsLegendPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EAStationsLegendPanel.Location = new System.Drawing.Point(904, 43);
            this.EAStationsLegendPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EAStationsLegendPanel.Name = "EAStationsLegendPanel";
            this.EAStationsLegendPanel.Size = new System.Drawing.Size(205, 650);
            this.EAStationsLegendPanel.TabIndex = 51;
            // 
            // SimulasyonSonucGoruntule
            // 
            this.SimulasyonSonucGoruntule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SimulasyonSonucGoruntule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SimulasyonSonucGoruntule.Enabled = false;
            this.SimulasyonSonucGoruntule.FlatAppearance.BorderSize = 0;
            this.SimulasyonSonucGoruntule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SimulasyonSonucGoruntule.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SimulasyonSonucGoruntule.ForeColor = System.Drawing.Color.White;
            this.SimulasyonSonucGoruntule.Location = new System.Drawing.Point(21, 402);
            this.SimulasyonSonucGoruntule.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SimulasyonSonucGoruntule.Name = "SimulasyonSonucGoruntule";
            this.SimulasyonSonucGoruntule.Size = new System.Drawing.Size(157, 55);
            this.SimulasyonSonucGoruntule.TabIndex = 60;
            this.SimulasyonSonucGoruntule.Text = "Sonuçları Getir";
            this.SimulasyonSonucGoruntule.UseVisualStyleBackColor = false;
            this.SimulasyonSonucGoruntule.Click += new System.EventHandler(this.SimulasyonSonucGoruntule_Click);
            // 
            // EANewSimulationResultsButton
            // 
            this.EANewSimulationResultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EANewSimulationResultsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EANewSimulationResultsButton.FlatAppearance.BorderSize = 0;
            this.EANewSimulationResultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EANewSimulationResultsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EANewSimulationResultsButton.ForeColor = System.Drawing.Color.White;
            this.EANewSimulationResultsButton.Location = new System.Drawing.Point(21, 295);
            this.EANewSimulationResultsButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EANewSimulationResultsButton.Name = "EANewSimulationResultsButton";
            this.EANewSimulationResultsButton.Size = new System.Drawing.Size(157, 68);
            this.EANewSimulationResultsButton.TabIndex = 58;
            this.EANewSimulationResultsButton.Text = "Gelecek Simülasyonu Oluştur";
            this.EANewSimulationResultsButton.UseVisualStyleBackColor = false;
            this.EANewSimulationResultsButton.Click += new System.EventHandler(this.EANewSimulationResultsButton_Click);
            // 
            // EASimButton
            // 
            this.EASimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EASimButton.FlatAppearance.BorderSize = 0;
            this.EASimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EASimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EASimButton.ForeColor = System.Drawing.Color.White;
            this.EASimButton.Location = new System.Drawing.Point(21, 506);
            this.EASimButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EASimButton.Name = "EASimButton";
            this.EASimButton.Size = new System.Drawing.Size(157, 54);
            this.EASimButton.TabIndex = 50;
            this.EASimButton.Text = "Gelecek Senaryo Görüntüle";
            this.EASimButton.UseVisualStyleBackColor = false;
            this.EASimButton.Click += new System.EventHandler(this.gelecekSimilasyonGoruntule);
            // 
            // EaSimMaxBtn
            // 
            this.EaSimMaxBtn.AutoSize = true;
            this.EaSimMaxBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimMaxBtn.Location = new System.Drawing.Point(16, 116);
            this.EaSimMaxBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EaSimMaxBtn.Name = "EaSimMaxBtn";
            this.EaSimMaxBtn.Size = new System.Drawing.Size(132, 27);
            this.EaSimMaxBtn.TabIndex = 53;
            this.EaSimMaxBtn.TabStop = true;
            this.EaSimMaxBtn.Text = "Hızlı Senaryo";
            this.EaSimMaxBtn.UseVisualStyleBackColor = true;
            this.EaSimMaxBtn.CheckedChanged += new System.EventHandler(this.EaSimMaxBtn_CheckedChanged);
            // 
            // FutureSimLabel
            // 
            this.FutureSimLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FutureSimLabel.AutoSize = true;
            this.FutureSimLabel.BackColor = System.Drawing.Color.DarkOrange;
            this.FutureSimLabel.ForeColor = System.Drawing.Color.DarkBlue;
            this.FutureSimLabel.Location = new System.Drawing.Point(12, 12);
            this.FutureSimLabel.Name = "FutureSimLabel";
            this.FutureSimLabel.Size = new System.Drawing.Size(176, 23);
            this.FutureSimLabel.TabIndex = 2;
            this.FutureSimLabel.Text = "Gelecek Simülasyonu:";
            // 
            // EaSimDefBtn
            // 
            this.EaSimDefBtn.AutoSize = true;
            this.EaSimDefBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimDefBtn.Location = new System.Drawing.Point(16, 180);
            this.EaSimDefBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EaSimDefBtn.Name = "EaSimDefBtn";
            this.EaSimDefBtn.Size = new System.Drawing.Size(176, 27);
            this.EaSimDefBtn.TabIndex = 52;
            this.EaSimDefBtn.TabStop = true;
            this.EaSimDefBtn.Text = "Varsayılan Senaryo";
            this.EaSimDefBtn.UseVisualStyleBackColor = true;
            this.EaSimDefBtn.CheckedChanged += new System.EventHandler(this.EaSimDefBtn_CheckedChanged);
            // 
            // comboBox_ea_yıl_secimi
            // 
            this.comboBox_ea_yıl_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_ea_yıl_secimi.FormattingEnabled = true;
            this.comboBox_ea_yıl_secimi.Items.AddRange(new object[] {
            "2015",
            "2016"});
            this.comboBox_ea_yıl_secimi.Location = new System.Drawing.Point(68, 63);
            this.comboBox_ea_yıl_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_ea_yıl_secimi.Name = "comboBox_ea_yıl_secimi";
            this.comboBox_ea_yıl_secimi.Size = new System.Drawing.Size(71, 29);
            this.comboBox_ea_yıl_secimi.TabIndex = 0;
            this.comboBox_ea_yıl_secimi.Text = "YIL";
            this.comboBox_ea_yıl_secimi.SelectedIndexChanged += new System.EventHandler(this.yilSecimiMonteCarlo);
            // 
            // EaSimMinBtn
            // 
            this.EaSimMinBtn.AutoSize = true;
            this.EaSimMinBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimMinBtn.Location = new System.Drawing.Point(16, 150);
            this.EaSimMinBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EaSimMinBtn.Name = "EaSimMinBtn";
            this.EaSimMinBtn.Size = new System.Drawing.Size(141, 27);
            this.EaSimMinBtn.TabIndex = 51;
            this.EaSimMinBtn.TabStop = true;
            this.EaSimMinBtn.Text = "Yavaş Senaryo";
            this.EaSimMinBtn.UseVisualStyleBackColor = true;
            this.EaSimMinBtn.CheckedChanged += new System.EventHandler(this.EaSimMinBtn_CheckedChanged);
            // 
            // panel_ea
            // 
            this.panel_ea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_ea.Controls.Add(this.CreateReportButton2);
            this.panel_ea.Controls.Add(this.EAPointsLayerCheckBox);
            this.panel_ea.Controls.Add(this.progressBar);
            this.panel_ea.Controls.Add(this.statusLabel);
            this.panel_ea.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_ea.Location = new System.Drawing.Point(0, 0);
            this.panel_ea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_ea.Name = "panel_ea";
            this.panel_ea.Size = new System.Drawing.Size(1109, 43);
            this.panel_ea.TabIndex = 44;
            // 
            // CreateReportButton2
            // 
            this.CreateReportButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.CreateReportButton2.FlatAppearance.BorderSize = 0;
            this.CreateReportButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CreateReportButton2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.CreateReportButton2.ForeColor = System.Drawing.Color.Snow;
            this.CreateReportButton2.Location = new System.Drawing.Point(693, 7);
            this.CreateReportButton2.Name = "CreateReportButton2";
            this.CreateReportButton2.Size = new System.Drawing.Size(129, 31);
            this.CreateReportButton2.TabIndex = 64;
            this.CreateReportButton2.Text = "Rapor Oluştur";
            this.CreateReportButton2.UseVisualStyleBackColor = false;
            this.CreateReportButton2.Click += new System.EventHandler(this.CreateReportButton2_Click);
            // 
            // EAPointsLayerCheckBox
            // 
            this.EAPointsLayerCheckBox.AutoSize = true;
            this.EAPointsLayerCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.EAPointsLayerCheckBox.Checked = true;
            this.EAPointsLayerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EAPointsLayerCheckBox.Location = new System.Drawing.Point(7, 11);
            this.EAPointsLayerCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EAPointsLayerCheckBox.Name = "EAPointsLayerCheckBox";
            this.EAPointsLayerCheckBox.Size = new System.Drawing.Size(127, 27);
            this.EAPointsLayerCheckBox.TabIndex = 61;
            this.EAPointsLayerCheckBox.Text = "EA Noktaları";
            this.EAPointsLayerCheckBox.UseVisualStyleBackColor = true;
            this.EAPointsLayerCheckBox.CheckedChanged += new System.EventHandler(this.EAPointsLayerCheckBox_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(947, 12);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressBar.MarqueeAnimationSpeed = 200;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(148, 22);
            this.progressBar.TabIndex = 60;
            this.progressBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.statusLabel.Location = new System.Drawing.Point(863, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(50, 19);
            this.statusLabel.TabIndex = 59;
            this.statusLabel.Text = "Status:";
            this.statusLabel.Visible = false;
            // 
            // GelecekSimPanel
            // 
            this.GelecekSimPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.GelecekSimPanel.Controls.Add(this.DCFastLegendValueLabel);
            this.GelecekSimPanel.Controls.Add(this.DCFastLegendLabel);
            this.GelecekSimPanel.Controls.Add(this.checkBox_DC_Fast);
            this.GelecekSimPanel.Controls.Add(this.checkBox_AC_Public);
            this.GelecekSimPanel.Controls.Add(this.ACPublicLegendValueLabel);
            this.GelecekSimPanel.Controls.Add(this.ACPublicLegendLabel);
            this.GelecekSimPanel.Controls.Add(this.checkBox_AC_Home);
            this.GelecekSimPanel.Controls.Add(this.AddStationLabel);
            this.GelecekSimPanel.Controls.Add(this.checkBox_AC_Work);
            this.GelecekSimPanel.Controls.Add(this.ACWorkLegendValueLabel);
            this.GelecekSimPanel.Controls.Add(this.EAStationAddButton);
            this.GelecekSimPanel.Controls.Add(this.ACHomeLegendLabel);
            this.GelecekSimPanel.Controls.Add(this.ACWorkLegendLabel);
            this.GelecekSimPanel.Controls.Add(this.ACHomeLegendValueLabel);
            this.GelecekSimPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.GelecekSimPanel.Location = new System.Drawing.Point(1109, 0);
            this.GelecekSimPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.GelecekSimPanel.Name = "GelecekSimPanel";
            this.GelecekSimPanel.Size = new System.Drawing.Size(203, 693);
            this.GelecekSimPanel.TabIndex = 43;
            // 
            // DCFastLegendValueLabel
            // 
            this.DCFastLegendValueLabel.AutoSize = true;
            this.DCFastLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DCFastLegendValueLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.DCFastLegendValueLabel.Location = new System.Drawing.Point(113, 190);
            this.DCFastLegendValueLabel.Name = "DCFastLegendValueLabel";
            this.DCFastLegendValueLabel.Size = new System.Drawing.Size(57, 19);
            this.DCFastLegendValueLabel.TabIndex = 57;
            this.DCFastLegendValueLabel.Text = "150 kW";
            // 
            // DCFastLegendLabel
            // 
            this.DCFastLegendLabel.AutoSize = true;
            this.DCFastLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.DCFastLegendLabel.Location = new System.Drawing.Point(19, 190);
            this.DCFastLegendLabel.Name = "DCFastLegendLabel";
            this.DCFastLegendLabel.Size = new System.Drawing.Size(70, 23);
            this.DCFastLegendLabel.TabIndex = 56;
            this.DCFastLegendLabel.Text = "DC-Fast";
            // 
            // checkBox_DC_Fast
            // 
            this.checkBox_DC_Fast.AutoSize = true;
            this.checkBox_DC_Fast.Checked = true;
            this.checkBox_DC_Fast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_DC_Fast.Location = new System.Drawing.Point(37, 424);
            this.checkBox_DC_Fast.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_DC_Fast.Name = "checkBox_DC_Fast";
            this.checkBox_DC_Fast.Size = new System.Drawing.Size(99, 27);
            this.checkBox_DC_Fast.TabIndex = 42;
            this.checkBox_DC_Fast.Text = "DC-FAST";
            this.checkBox_DC_Fast.UseVisualStyleBackColor = true;
            this.checkBox_DC_Fast.Visible = false;
            this.checkBox_DC_Fast.CheckedChanged += new System.EventHandler(this.checkBox_Dc_Fast);
            // 
            // checkBox_AC_Public
            // 
            this.checkBox_AC_Public.AutoSize = true;
            this.checkBox_AC_Public.Checked = true;
            this.checkBox_AC_Public.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Public.Location = new System.Drawing.Point(36, 393);
            this.checkBox_AC_Public.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Public.Name = "checkBox_AC_Public";
            this.checkBox_AC_Public.Size = new System.Drawing.Size(117, 27);
            this.checkBox_AC_Public.TabIndex = 41;
            this.checkBox_AC_Public.Text = "AC-PUBLIC";
            this.checkBox_AC_Public.UseVisualStyleBackColor = true;
            this.checkBox_AC_Public.Visible = false;
            this.checkBox_AC_Public.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Public);
            // 
            // ACPublicLegendValueLabel
            // 
            this.ACPublicLegendValueLabel.AutoSize = true;
            this.ACPublicLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACPublicLegendValueLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACPublicLegendValueLabel.Location = new System.Drawing.Point(113, 159);
            this.ACPublicLegendValueLabel.Name = "ACPublicLegendValueLabel";
            this.ACPublicLegendValueLabel.Size = new System.Drawing.Size(53, 19);
            this.ACPublicLegendValueLabel.TabIndex = 55;
            this.ACPublicLegendValueLabel.Text = " 22 kW";
            // 
            // ACPublicLegendLabel
            // 
            this.ACPublicLegendLabel.AutoSize = true;
            this.ACPublicLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACPublicLegendLabel.Location = new System.Drawing.Point(19, 156);
            this.ACPublicLegendLabel.Name = "ACPublicLegendLabel";
            this.ACPublicLegendLabel.Size = new System.Drawing.Size(85, 23);
            this.ACPublicLegendLabel.TabIndex = 54;
            this.ACPublicLegendLabel.Text = "AC-Public";
            // 
            // checkBox_AC_Home
            // 
            this.checkBox_AC_Home.AutoSize = true;
            this.checkBox_AC_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_AC_Home.Checked = true;
            this.checkBox_AC_Home.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Home.Location = new System.Drawing.Point(37, 361);
            this.checkBox_AC_Home.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Home.Name = "checkBox_AC_Home";
            this.checkBox_AC_Home.Size = new System.Drawing.Size(112, 27);
            this.checkBox_AC_Home.TabIndex = 39;
            this.checkBox_AC_Home.Text = "AC-HOME";
            this.checkBox_AC_Home.UseVisualStyleBackColor = true;
            this.checkBox_AC_Home.Visible = false;
            this.checkBox_AC_Home.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Home);
            // 
            // AddStationLabel
            // 
            this.AddStationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddStationLabel.AutoSize = true;
            this.AddStationLabel.BackColor = System.Drawing.Color.DarkOrange;
            this.AddStationLabel.ForeColor = System.Drawing.Color.DarkBlue;
            this.AddStationLabel.Location = new System.Drawing.Point(43, 55);
            this.AddStationLabel.Name = "AddStationLabel";
            this.AddStationLabel.Size = new System.Drawing.Size(128, 23);
            this.AddStationLabel.TabIndex = 45;
            this.AddStationLabel.Text = "İstasyon Tipleri:";
            // 
            // checkBox_AC_Work
            // 
            this.checkBox_AC_Work.AutoSize = true;
            this.checkBox_AC_Work.Checked = true;
            this.checkBox_AC_Work.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Work.Location = new System.Drawing.Point(37, 455);
            this.checkBox_AC_Work.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Work.Name = "checkBox_AC_Work";
            this.checkBox_AC_Work.Size = new System.Drawing.Size(111, 27);
            this.checkBox_AC_Work.TabIndex = 40;
            this.checkBox_AC_Work.Text = "AC-WORK";
            this.checkBox_AC_Work.UseVisualStyleBackColor = true;
            this.checkBox_AC_Work.Visible = false;
            this.checkBox_AC_Work.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Work);
            // 
            // ACWorkLegendValueLabel
            // 
            this.ACWorkLegendValueLabel.AutoSize = true;
            this.ACWorkLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACWorkLegendValueLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACWorkLegendValueLabel.Location = new System.Drawing.Point(113, 127);
            this.ACWorkLegendValueLabel.Name = "ACWorkLegendValueLabel";
            this.ACWorkLegendValueLabel.Size = new System.Drawing.Size(53, 19);
            this.ACWorkLegendValueLabel.TabIndex = 53;
            this.ACWorkLegendValueLabel.Text = " 11 kW";
            // 
            // EAStationAddButton
            // 
            this.EAStationAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EAStationAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EAStationAddButton.Enabled = false;
            this.EAStationAddButton.FlatAppearance.BorderSize = 0;
            this.EAStationAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EAStationAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EAStationAddButton.ForeColor = System.Drawing.Color.White;
            this.EAStationAddButton.Location = new System.Drawing.Point(37, 249);
            this.EAStationAddButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EAStationAddButton.Name = "EAStationAddButton";
            this.EAStationAddButton.Size = new System.Drawing.Size(140, 53);
            this.EAStationAddButton.TabIndex = 49;
            this.EAStationAddButton.Text = "EA Şarj İstasyonu Ekle";
            this.EAStationAddButton.UseVisualStyleBackColor = false;
            this.EAStationAddButton.Click += new System.EventHandler(this.EAStationAddButton_Click);
            // 
            // ACHomeLegendLabel
            // 
            this.ACHomeLegendLabel.AutoSize = true;
            this.ACHomeLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACHomeLegendLabel.Location = new System.Drawing.Point(17, 89);
            this.ACHomeLegendLabel.Name = "ACHomeLegendLabel";
            this.ACHomeLegendLabel.Size = new System.Drawing.Size(86, 23);
            this.ACHomeLegendLabel.TabIndex = 50;
            this.ACHomeLegendLabel.Text = "AC-Home";
            // 
            // ACWorkLegendLabel
            // 
            this.ACWorkLegendLabel.AutoSize = true;
            this.ACWorkLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACWorkLegendLabel.Location = new System.Drawing.Point(19, 124);
            this.ACWorkLegendLabel.Name = "ACWorkLegendLabel";
            this.ACWorkLegendLabel.Size = new System.Drawing.Size(80, 23);
            this.ACWorkLegendLabel.TabIndex = 52;
            this.ACWorkLegendLabel.Text = "AC-Work";
            // 
            // ACHomeLegendValueLabel
            // 
            this.ACHomeLegendValueLabel.AutoSize = true;
            this.ACHomeLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACHomeLegendValueLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACHomeLegendValueLabel.Location = new System.Drawing.Point(116, 92);
            this.ACHomeLegendValueLabel.Name = "ACHomeLegendValueLabel";
            this.ACHomeLegendValueLabel.Size = new System.Drawing.Size(49, 19);
            this.ACHomeLegendValueLabel.TabIndex = 51;
            this.ACHomeLegendValueLabel.Text = "11 kW";
            // 
            // gMapControl_EA
            // 
            this.gMapControl_EA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_EA.Bearing = 0F;
            this.gMapControl_EA.CanDragMap = true;
            this.gMapControl_EA.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_EA.GrayScaleMode = false;
            this.gMapControl_EA.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_EA.LevelsKeepInMemory = 5;
            this.gMapControl_EA.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_EA.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gMapControl_EA.MarkersEnabled = true;
            this.gMapControl_EA.MaxZoom = 2;
            this.gMapControl_EA.MinZoom = 2;
            this.gMapControl_EA.MouseWheelZoomEnabled = true;
            this.gMapControl_EA.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_EA.Name = "gMapControl_EA";
            this.gMapControl_EA.NegativeMode = false;
            this.gMapControl_EA.PolygonsEnabled = true;
            this.gMapControl_EA.RetryLoadTile = 0;
            this.gMapControl_EA.RoutesEnabled = true;
            this.gMapControl_EA.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_EA.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_EA.ShowTileGridLines = false;
            this.gMapControl_EA.Size = new System.Drawing.Size(1206, 2508);
            this.gMapControl_EA.TabIndex = 18;
            this.gMapControl_EA.Zoom = 0D;
            this.gMapControl_EA.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_EA_OnMarkerClick);
            // 
            // tab_imar
            // 
            this.tab_imar.AutoScroll = true;
            this.tab_imar.Controls.Add(this.buton_SLF_tahmini);
            this.tab_imar.Controls.Add(this.buton_abone_sayısı_tahmini);
            this.tab_imar.Controls.Add(this.buton_imar_tahmini);
            this.tab_imar.Controls.Add(this.panel2);
            this.tab_imar.Controls.Add(this.panel_imar);
            this.tab_imar.Controls.Add(this.buton_DL_calıstır);
            this.tab_imar.Controls.Add(this.checkBox_imar_15);
            this.tab_imar.Controls.Add(this.checkBox_imar_14);
            this.tab_imar.Controls.Add(this.checkBox_imar_1);
            this.tab_imar.Controls.Add(this.checkBox_imar_2);
            this.tab_imar.Controls.Add(this.checkBox_imar_3);
            this.tab_imar.Controls.Add(this.checkBox_imar_4);
            this.tab_imar.Controls.Add(this.checkBox_imar_5);
            this.tab_imar.Controls.Add(this.checkBox_imar_6);
            this.tab_imar.Controls.Add(this.checkBox_imar_7);
            this.tab_imar.Controls.Add(this.checkBox_imar_8);
            this.tab_imar.Controls.Add(this.checkBox_imar_9);
            this.tab_imar.Controls.Add(this.checkBox_imar_10);
            this.tab_imar.Controls.Add(this.checkBox_imar_11);
            this.tab_imar.Controls.Add(this.checkBox_imar_12);
            this.tab_imar.Controls.Add(this.checkBox_imar_13);
            this.tab_imar.Controls.Add(this.label_imar_katmanlar);
            this.tab_imar.Controls.Add(this.imar_dosya_seçimi);
            this.tab_imar.Controls.Add(this.toolStrip_imar);
            this.tab_imar.ImageIndex = 11;
            this.tab_imar.Location = new System.Drawing.Point(4, 30);
            this.tab_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_imar.Name = "tab_imar";
            this.tab_imar.Size = new System.Drawing.Size(1312, 693);
            this.tab_imar.TabIndex = 4;
            this.tab_imar.Text = "İmar Analizleri";
            this.tab_imar.UseVisualStyleBackColor = true;
            // 
            // buton_SLF_tahmini
            // 
            this.buton_SLF_tahmini.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_SLF_tahmini.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buton_SLF_tahmini.ForeColor = System.Drawing.Color.SteelBlue;
            this.buton_SLF_tahmini.Location = new System.Drawing.Point(1101, 610);
            this.buton_SLF_tahmini.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_SLF_tahmini.Name = "buton_SLF_tahmini";
            this.buton_SLF_tahmini.Size = new System.Drawing.Size(187, 47);
            this.buton_SLF_tahmini.TabIndex = 77;
            this.buton_SLF_tahmini.Text = "SLF Metodunu Çalıştır";
            this.buton_SLF_tahmini.UseVisualStyleBackColor = true;
            this.buton_SLF_tahmini.Click += new System.EventHandler(this.buton_SLF_tahmini_Click);
            // 
            // buton_abone_sayısı_tahmini
            // 
            this.buton_abone_sayısı_tahmini.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_abone_sayısı_tahmini.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buton_abone_sayısı_tahmini.ForeColor = System.Drawing.Color.SteelBlue;
            this.buton_abone_sayısı_tahmini.Location = new System.Drawing.Point(885, 610);
            this.buton_abone_sayısı_tahmini.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_abone_sayısı_tahmini.Name = "buton_abone_sayısı_tahmini";
            this.buton_abone_sayısı_tahmini.Size = new System.Drawing.Size(192, 47);
            this.buton_abone_sayısı_tahmini.TabIndex = 76;
            this.buton_abone_sayısı_tahmini.Text = "Abone Sayısı Tahmini Yap";
            this.buton_abone_sayısı_tahmini.UseVisualStyleBackColor = true;
            this.buton_abone_sayısı_tahmini.Click += new System.EventHandler(this.buton_abone_sayısı_tahmini_Click);
            // 
            // buton_imar_tahmini
            // 
            this.buton_imar_tahmini.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_imar_tahmini.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buton_imar_tahmini.ForeColor = System.Drawing.Color.SteelBlue;
            this.buton_imar_tahmini.Location = new System.Drawing.Point(706, 610);
            this.buton_imar_tahmini.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_imar_tahmini.Name = "buton_imar_tahmini";
            this.buton_imar_tahmini.Size = new System.Drawing.Size(157, 47);
            this.buton_imar_tahmini.TabIndex = 75;
            this.buton_imar_tahmini.Text = "İmar Tahmini Yap";
            this.buton_imar_tahmini.UseVisualStyleBackColor = true;
            this.buton_imar_tahmini.Click += new System.EventHandler(this.buton_imar_tahmini_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.RosyBrown;
            this.panel2.Location = new System.Drawing.Point(286, 662);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1003, 10);
            this.panel2.TabIndex = 74;
            // 
            // panel_imar
            // 
            this.panel_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_imar.Controls.Add(this.Mesafe_imar);
            this.panel_imar.Controls.Add(this.webView_imar);
            this.panel_imar.Controls.Add(this.buton_imar_katmanlar);
            this.panel_imar.Controls.Add(this.mesafe_metre_imar);
            this.panel_imar.Controls.Add(this.gMapControl_imar);
            this.panel_imar.Location = new System.Drawing.Point(284, 43);
            this.panel_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_imar.Name = "panel_imar";
            this.panel_imar.Size = new System.Drawing.Size(1004, 560);
            this.panel_imar.TabIndex = 73;
            // 
            // Mesafe_imar
            // 
            this.Mesafe_imar.AutoSize = true;
            this.Mesafe_imar.Location = new System.Drawing.Point(27, 7);
            this.Mesafe_imar.Name = "Mesafe_imar";
            this.Mesafe_imar.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_imar.TabIndex = 57;
            this.Mesafe_imar.Text = "Mesafe:";
            this.Mesafe_imar.Visible = false;
            // 
            // webView_imar
            // 
            this.webView_imar.AllowExternalDrop = true;
            this.webView_imar.CreationProperties = null;
            this.webView_imar.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_imar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_imar.Location = new System.Drawing.Point(0, 0);
            this.webView_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_imar.Name = "webView_imar";
            this.webView_imar.Size = new System.Drawing.Size(1004, 560);
            this.webView_imar.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_imar.TabIndex = 72;
            this.webView_imar.Visible = false;
            this.webView_imar.ZoomFactor = 1D;
            // 
            // buton_imar_katmanlar
            // 
            this.buton_imar_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_imar_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_imar_katmanlar.BackgroundImage")));
            this.buton_imar_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_imar_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_imar_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_imar_katmanlar.Location = new System.Drawing.Point(6, 504);
            this.buton_imar_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_imar_katmanlar.Name = "buton_imar_katmanlar";
            this.buton_imar_katmanlar.Size = new System.Drawing.Size(57, 49);
            this.buton_imar_katmanlar.TabIndex = 62;
            this.buton_imar_katmanlar.UseVisualStyleBackColor = true;
            // 
            // harita_katmanları_right_click
            // 
            this.harita_katmanları_right_click.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.harita_katmanları_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Arazi,
            this.Google_Earth,
            this.Google_Earth_Desktop,
            this.Harita,
            this.OSM,
            this.Sokak_Görünümü,
            this.Uydu});
            this.harita_katmanları_right_click.Name = "harita_katmanları_right_click";
            this.harita_katmanları_right_click.Size = new System.Drawing.Size(201, 228);
            // 
            // Arazi
            // 
            this.Arazi.Image = ((System.Drawing.Image)(resources.GetObject("Arazi.Image")));
            this.Arazi.Name = "Arazi";
            this.Arazi.Size = new System.Drawing.Size(200, 32);
            this.Arazi.Text = "Arazi";
            this.Arazi.Click += new System.EventHandler(this.Arazi_Click);
            // 
            // Google_Earth
            // 
            this.Google_Earth.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth.Image")));
            this.Google_Earth.Name = "Google_Earth";
            this.Google_Earth.Size = new System.Drawing.Size(200, 32);
            this.Google_Earth.Text = "GE Online";
            this.Google_Earth.Click += new System.EventHandler(this.Google_Earth_Click);
            // 
            // Google_Earth_Desktop
            // 
            this.Google_Earth_Desktop.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth_Desktop.Image")));
            this.Google_Earth_Desktop.Name = "Google_Earth_Desktop";
            this.Google_Earth_Desktop.Size = new System.Drawing.Size(200, 32);
            this.Google_Earth_Desktop.Text = "GE Pro Desktop";
            this.Google_Earth_Desktop.Click += new System.EventHandler(this.Google_Earth_Desktop_Click);
            // 
            // Harita
            // 
            this.Harita.Image = ((System.Drawing.Image)(resources.GetObject("Harita.Image")));
            this.Harita.Name = "Harita";
            this.Harita.Size = new System.Drawing.Size(200, 32);
            this.Harita.Text = "Harita";
            this.Harita.Click += new System.EventHandler(this.Harita_Click);
            // 
            // OSM
            // 
            this.OSM.Image = ((System.Drawing.Image)(resources.GetObject("OSM.Image")));
            this.OSM.Name = "OSM";
            this.OSM.Size = new System.Drawing.Size(200, 32);
            this.OSM.Text = "Open Street Map";
            this.OSM.Click += new System.EventHandler(this.OSM_Click);
            // 
            // Sokak_Görünümü
            // 
            this.Sokak_Görünümü.Image = ((System.Drawing.Image)(resources.GetObject("Sokak_Görünümü.Image")));
            this.Sokak_Görünümü.Name = "Sokak_Görünümü";
            this.Sokak_Görünümü.Size = new System.Drawing.Size(200, 32);
            this.Sokak_Görünümü.Text = "Sokak Görünümü";
            this.Sokak_Görünümü.Click += new System.EventHandler(this.Sokak_Görünümü_Click);
            // 
            // Uydu
            // 
            this.Uydu.Image = ((System.Drawing.Image)(resources.GetObject("Uydu.Image")));
            this.Uydu.Name = "Uydu";
            this.Uydu.Size = new System.Drawing.Size(200, 32);
            this.Uydu.Text = "Uydu";
            this.Uydu.Click += new System.EventHandler(this.Uydu_Click);
            // 
            // mesafe_metre_imar
            // 
            this.mesafe_metre_imar.AutoSize = true;
            this.mesafe_metre_imar.Location = new System.Drawing.Point(128, 7);
            this.mesafe_metre_imar.Name = "mesafe_metre_imar";
            this.mesafe_metre_imar.Size = new System.Drawing.Size(0, 23);
            this.mesafe_metre_imar.TabIndex = 56;
            this.mesafe_metre_imar.Visible = false;
            // 
            // gMapControl_imar
            // 
            this.gMapControl_imar.Bearing = 0F;
            this.gMapControl_imar.CanDragMap = true;
            this.gMapControl_imar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_imar.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_imar.GrayScaleMode = false;
            this.gMapControl_imar.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_imar.LevelsKeepInMemory = 5;
            this.gMapControl_imar.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gMapControl_imar.MarkersEnabled = true;
            this.gMapControl_imar.MaxZoom = 2;
            this.gMapControl_imar.MinZoom = 2;
            this.gMapControl_imar.MouseWheelZoomEnabled = true;
            this.gMapControl_imar.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_imar.Name = "gMapControl_imar";
            this.gMapControl_imar.NegativeMode = false;
            this.gMapControl_imar.PolygonsEnabled = true;
            this.gMapControl_imar.RetryLoadTile = 0;
            this.gMapControl_imar.RoutesEnabled = true;
            this.gMapControl_imar.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_imar.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_imar.ShowTileGridLines = false;
            this.gMapControl_imar.Size = new System.Drawing.Size(1004, 560);
            this.gMapControl_imar.TabIndex = 60;
            this.gMapControl_imar.Zoom = 0D;
            this.gMapControl_imar.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_imar_OnMapClick);
            this.gMapControl_imar.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_imar_OnMapDoubleClick);
            this.gMapControl_imar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseDown);
            this.gMapControl_imar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseMove);
            this.gMapControl_imar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseUp);
            // 
            // buton_DL_calıstır
            // 
            this.buton_DL_calıstır.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buton_DL_calıstır.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buton_DL_calıstır.ForeColor = System.Drawing.Color.SteelBlue;
            this.buton_DL_calıstır.Location = new System.Drawing.Point(516, 610);
            this.buton_DL_calıstır.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_DL_calıstır.Name = "buton_DL_calıstır";
            this.buton_DL_calıstır.Size = new System.Drawing.Size(170, 47);
            this.buton_DL_calıstır.TabIndex = 64;
            this.buton_DL_calıstır.Text = "Bina Tiplerini Oluştur";
            this.buton_DL_calıstır.UseVisualStyleBackColor = true;
            this.buton_DL_calıstır.Click += new System.EventHandler(this.buton_DL_calıstır_Click);
            // 
            // checkBox_imar_15
            // 
            this.checkBox_imar_15.AutoSize = true;
            this.checkBox_imar_15.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_imar_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_15.Location = new System.Drawing.Point(8, 636);
            this.checkBox_imar_15.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_15.Name = "checkBox_imar_15";
            this.checkBox_imar_15.Size = new System.Drawing.Size(169, 27);
            this.checkBox_imar_15.TabIndex = 60;
            this.checkBox_imar_15.Text = "checkBox_imar_15";
            this.checkBox_imar_15.UseVisualStyleBackColor = false;
            this.checkBox_imar_15.Visible = false;
            // 
            // katmanlar_right_click
            // 
            this.katmanlar_right_click.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.katmanlar_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabloyuGörToolStripMenuItem,
            this.rengiDeğiştirToolStripMenuItem,
            this.temizleToolStripMenuItem,
            this.yenidenAdlandırToolStripMenuItem,
            this.kaydetToolStripMenuItem});
            this.katmanlar_right_click.Name = "katmanlar_right_click";
            this.katmanlar_right_click.Size = new System.Drawing.Size(206, 184);
            this.katmanlar_right_click.Opening += new System.ComponentModel.CancelEventHandler(this.katmanlar_right_click_Opening);
            // 
            // tabloyuGörToolStripMenuItem
            // 
            this.tabloyuGörToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabloyuGörToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tabloyuGörToolStripMenuItem.Image")));
            this.tabloyuGörToolStripMenuItem.Name = "tabloyuGörToolStripMenuItem";
            this.tabloyuGörToolStripMenuItem.Size = new System.Drawing.Size(205, 36);
            this.tabloyuGörToolStripMenuItem.Text = "Tabloyu Gör";
            this.tabloyuGörToolStripMenuItem.Click += new System.EventHandler(this.tabloyuGörToolStripMenuItem_Click);
            // 
            // rengiDeğiştirToolStripMenuItem
            // 
            this.rengiDeğiştirToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rengiDeğiştirToolStripMenuItem.Image")));
            this.rengiDeğiştirToolStripMenuItem.Name = "rengiDeğiştirToolStripMenuItem";
            this.rengiDeğiştirToolStripMenuItem.Size = new System.Drawing.Size(205, 36);
            this.rengiDeğiştirToolStripMenuItem.Text = "Rengi Değiştir";
            this.rengiDeğiştirToolStripMenuItem.Click += new System.EventHandler(this.rengiDeğiştirToolStripMenuItem_Click);
            // 
            // temizleToolStripMenuItem
            // 
            this.temizleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("temizleToolStripMenuItem.Image")));
            this.temizleToolStripMenuItem.Name = "temizleToolStripMenuItem";
            this.temizleToolStripMenuItem.Size = new System.Drawing.Size(205, 36);
            this.temizleToolStripMenuItem.Text = "Temizle";
            this.temizleToolStripMenuItem.Click += new System.EventHandler(this.temizleToolStripMenuItem_Click);
            // 
            // yenidenAdlandırToolStripMenuItem
            // 
            this.yenidenAdlandırToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("yenidenAdlandırToolStripMenuItem.Image")));
            this.yenidenAdlandırToolStripMenuItem.Name = "yenidenAdlandırToolStripMenuItem";
            this.yenidenAdlandırToolStripMenuItem.Size = new System.Drawing.Size(205, 36);
            this.yenidenAdlandırToolStripMenuItem.Text = "Yeniden Adlandır";
            this.yenidenAdlandırToolStripMenuItem.Click += new System.EventHandler(this.yenidenAdlandırToolStripMenuItem_Click);
            // 
            // kaydetToolStripMenuItem
            // 
            this.kaydetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("kaydetToolStripMenuItem.Image")));
            this.kaydetToolStripMenuItem.Name = "kaydetToolStripMenuItem";
            this.kaydetToolStripMenuItem.Size = new System.Drawing.Size(205, 36);
            this.kaydetToolStripMenuItem.Text = "Kaydet";
            this.kaydetToolStripMenuItem.Click += new System.EventHandler(this.kaydetToolStripMenuItem_Click);
            // 
            // checkBox_imar_14
            // 
            this.checkBox_imar_14.AutoSize = true;
            this.checkBox_imar_14.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_imar_14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_14.Location = new System.Drawing.Point(8, 601);
            this.checkBox_imar_14.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_14.Name = "checkBox_imar_14";
            this.checkBox_imar_14.Size = new System.Drawing.Size(170, 27);
            this.checkBox_imar_14.TabIndex = 59;
            this.checkBox_imar_14.Text = "checkBox_imar_14";
            this.checkBox_imar_14.UseVisualStyleBackColor = false;
            this.checkBox_imar_14.Visible = false;
            // 
            // checkBox_imar_1
            // 
            this.checkBox_imar_1.AutoSize = true;
            this.checkBox_imar_1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_1.Location = new System.Drawing.Point(8, 148);
            this.checkBox_imar_1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_1.Name = "checkBox_imar_1";
            this.checkBox_imar_1.Size = new System.Drawing.Size(160, 27);
            this.checkBox_imar_1.TabIndex = 55;
            this.checkBox_imar_1.Text = "checkBox_imar_1";
            this.checkBox_imar_1.UseVisualStyleBackColor = true;
            this.checkBox_imar_1.Visible = false;
            // 
            // checkBox_imar_2
            // 
            this.checkBox_imar_2.AutoSize = true;
            this.checkBox_imar_2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_2.Location = new System.Drawing.Point(8, 183);
            this.checkBox_imar_2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_2.Name = "checkBox_imar_2";
            this.checkBox_imar_2.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_2.TabIndex = 54;
            this.checkBox_imar_2.Text = "checkBox_imar_2";
            this.checkBox_imar_2.UseVisualStyleBackColor = true;
            this.checkBox_imar_2.Visible = false;
            // 
            // checkBox_imar_3
            // 
            this.checkBox_imar_3.AutoSize = true;
            this.checkBox_imar_3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_3.Location = new System.Drawing.Point(8, 218);
            this.checkBox_imar_3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_3.Name = "checkBox_imar_3";
            this.checkBox_imar_3.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_3.TabIndex = 53;
            this.checkBox_imar_3.Text = "checkBox_imar_3";
            this.checkBox_imar_3.UseVisualStyleBackColor = true;
            this.checkBox_imar_3.Visible = false;
            // 
            // checkBox_imar_4
            // 
            this.checkBox_imar_4.AutoSize = true;
            this.checkBox_imar_4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_4.Location = new System.Drawing.Point(8, 254);
            this.checkBox_imar_4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_4.Name = "checkBox_imar_4";
            this.checkBox_imar_4.Size = new System.Drawing.Size(163, 27);
            this.checkBox_imar_4.TabIndex = 52;
            this.checkBox_imar_4.Text = "checkBox_imar_4";
            this.checkBox_imar_4.UseVisualStyleBackColor = true;
            this.checkBox_imar_4.Visible = false;
            // 
            // checkBox_imar_5
            // 
            this.checkBox_imar_5.AutoSize = true;
            this.checkBox_imar_5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_5.Location = new System.Drawing.Point(8, 288);
            this.checkBox_imar_5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_5.Name = "checkBox_imar_5";
            this.checkBox_imar_5.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_5.TabIndex = 51;
            this.checkBox_imar_5.Text = "checkBox_imar_5";
            this.checkBox_imar_5.UseVisualStyleBackColor = true;
            this.checkBox_imar_5.Visible = false;
            // 
            // checkBox_imar_6
            // 
            this.checkBox_imar_6.AutoSize = true;
            this.checkBox_imar_6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_6.Location = new System.Drawing.Point(8, 322);
            this.checkBox_imar_6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_6.Name = "checkBox_imar_6";
            this.checkBox_imar_6.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_6.TabIndex = 50;
            this.checkBox_imar_6.Text = "checkBox_imar_6";
            this.checkBox_imar_6.UseVisualStyleBackColor = true;
            this.checkBox_imar_6.Visible = false;
            // 
            // checkBox_imar_7
            // 
            this.checkBox_imar_7.AutoSize = true;
            this.checkBox_imar_7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_7.Location = new System.Drawing.Point(8, 358);
            this.checkBox_imar_7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_7.Name = "checkBox_imar_7";
            this.checkBox_imar_7.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_7.TabIndex = 49;
            this.checkBox_imar_7.Text = "checkBox_imar_7";
            this.checkBox_imar_7.UseVisualStyleBackColor = true;
            this.checkBox_imar_7.Visible = false;
            // 
            // checkBox_imar_8
            // 
            this.checkBox_imar_8.AutoSize = true;
            this.checkBox_imar_8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_8.Location = new System.Drawing.Point(8, 393);
            this.checkBox_imar_8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_8.Name = "checkBox_imar_8";
            this.checkBox_imar_8.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_8.TabIndex = 48;
            this.checkBox_imar_8.Text = "checkBox_imar_8";
            this.checkBox_imar_8.UseVisualStyleBackColor = true;
            this.checkBox_imar_8.Visible = false;
            // 
            // checkBox_imar_9
            // 
            this.checkBox_imar_9.AutoSize = true;
            this.checkBox_imar_9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_9.Location = new System.Drawing.Point(8, 428);
            this.checkBox_imar_9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_9.Name = "checkBox_imar_9";
            this.checkBox_imar_9.Size = new System.Drawing.Size(162, 27);
            this.checkBox_imar_9.TabIndex = 47;
            this.checkBox_imar_9.Text = "checkBox_imar_9";
            this.checkBox_imar_9.UseVisualStyleBackColor = true;
            this.checkBox_imar_9.Visible = false;
            // 
            // checkBox_imar_10
            // 
            this.checkBox_imar_10.AutoSize = true;
            this.checkBox_imar_10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_10.Location = new System.Drawing.Point(8, 463);
            this.checkBox_imar_10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_10.Name = "checkBox_imar_10";
            this.checkBox_imar_10.Size = new System.Drawing.Size(169, 27);
            this.checkBox_imar_10.TabIndex = 46;
            this.checkBox_imar_10.Text = "checkBox_imar_10";
            this.checkBox_imar_10.UseVisualStyleBackColor = true;
            this.checkBox_imar_10.Visible = false;
            // 
            // checkBox_imar_11
            // 
            this.checkBox_imar_11.AutoSize = true;
            this.checkBox_imar_11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_11.Location = new System.Drawing.Point(8, 498);
            this.checkBox_imar_11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_11.Name = "checkBox_imar_11";
            this.checkBox_imar_11.Size = new System.Drawing.Size(167, 27);
            this.checkBox_imar_11.TabIndex = 45;
            this.checkBox_imar_11.Text = "checkBox_imar_11";
            this.checkBox_imar_11.UseVisualStyleBackColor = true;
            this.checkBox_imar_11.Visible = false;
            // 
            // checkBox_imar_12
            // 
            this.checkBox_imar_12.AutoSize = true;
            this.checkBox_imar_12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_12.Location = new System.Drawing.Point(8, 533);
            this.checkBox_imar_12.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_12.Name = "checkBox_imar_12";
            this.checkBox_imar_12.Size = new System.Drawing.Size(169, 27);
            this.checkBox_imar_12.TabIndex = 44;
            this.checkBox_imar_12.Text = "checkBox_imar_12";
            this.checkBox_imar_12.UseVisualStyleBackColor = true;
            this.checkBox_imar_12.Visible = false;
            // 
            // checkBox_imar_13
            // 
            this.checkBox_imar_13.AutoSize = true;
            this.checkBox_imar_13.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_imar_13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_13.Location = new System.Drawing.Point(8, 569);
            this.checkBox_imar_13.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_imar_13.Name = "checkBox_imar_13";
            this.checkBox_imar_13.Size = new System.Drawing.Size(169, 27);
            this.checkBox_imar_13.TabIndex = 43;
            this.checkBox_imar_13.Text = "checkBox_imar_13";
            this.checkBox_imar_13.UseVisualStyleBackColor = false;
            this.checkBox_imar_13.Visible = false;
            // 
            // label_imar_katmanlar
            // 
            this.label_imar_katmanlar.AutoSize = true;
            this.label_imar_katmanlar.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_imar_katmanlar.Location = new System.Drawing.Point(21, 110);
            this.label_imar_katmanlar.Name = "label_imar_katmanlar";
            this.label_imar_katmanlar.Size = new System.Drawing.Size(99, 24);
            this.label_imar_katmanlar.TabIndex = 42;
            this.label_imar_katmanlar.Text = "Katmanlar";
            // 
            // imar_dosya_seçimi
            // 
            this.imar_dosya_seçimi.Location = new System.Drawing.Point(3, 43);
            this.imar_dosya_seçimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.imar_dosya_seçimi.Name = "imar_dosya_seçimi";
            this.imar_dosya_seçimi.Size = new System.Drawing.Size(173, 44);
            this.imar_dosya_seçimi.TabIndex = 41;
            this.imar_dosya_seçimi.Text = "Dosya Seç";
            this.imar_dosya_seçimi.UseVisualStyleBackColor = true;
            this.imar_dosya_seçimi.Click += new System.EventHandler(this.imar_dosya_seçimi_Click);
            // 
            // toolStrip_imar
            // 
            this.toolStrip_imar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip_imar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.İmar_Mesafe_Ölç,
            this.toolStripSeparator11,
            this.İmar_Poligon,
            this.toolStripSeparator12,
            this.İmar_Fonksiyonlar});
            this.toolStrip_imar.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_imar.Name = "toolStrip_imar";
            this.toolStrip_imar.Size = new System.Drawing.Size(1312, 32);
            this.toolStrip_imar.TabIndex = 40;
            this.toolStrip_imar.Text = "toolStrip1";
            // 
            // İmar_Mesafe_Ölç
            // 
            this.İmar_Mesafe_Ölç.AccessibleDescription = "";
            this.İmar_Mesafe_Ölç.AccessibleName = "";
            this.İmar_Mesafe_Ölç.BackColor = System.Drawing.Color.Honeydew;
            this.İmar_Mesafe_Ölç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Mesafe_Ölç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Mesafe_Ölç.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Mesafe_Ölç.Image")));
            this.İmar_Mesafe_Ölç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Mesafe_Ölç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Mesafe_Ölç.Margin = new System.Windows.Forms.Padding(300, 1, 0, 2);
            this.İmar_Mesafe_Ölç.Name = "İmar_Mesafe_Ölç";
            this.İmar_Mesafe_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Mesafe_Ölç.Size = new System.Drawing.Size(134, 29);
            this.İmar_Mesafe_Ölç.Tag = "";
            this.İmar_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.İmar_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            this.İmar_Mesafe_Ölç.Click += new System.EventHandler(this.İmar_Mesafe_Ölç_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 32);
            // 
            // İmar_Poligon
            // 
            this.İmar_Poligon.AccessibleDescription = "";
            this.İmar_Poligon.AccessibleName = "";
            this.İmar_Poligon.BackColor = System.Drawing.Color.Thistle;
            this.İmar_Poligon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Poligon.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Poligon.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Poligon.Image")));
            this.İmar_Poligon.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Poligon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Poligon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Poligon.Name = "İmar_Poligon";
            this.İmar_Poligon.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Poligon.Size = new System.Drawing.Size(106, 29);
            this.İmar_Poligon.Tag = "";
            this.İmar_Poligon.Text = "Poligon";
            this.İmar_Poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            this.İmar_Poligon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.İmar_Poligon_MouseDown);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 32);
            // 
            // İmar_Fonksiyonlar
            // 
            this.İmar_Fonksiyonlar.AccessibleDescription = "";
            this.İmar_Fonksiyonlar.AccessibleName = "";
            this.İmar_Fonksiyonlar.BackColor = System.Drawing.Color.LightBlue;
            this.İmar_Fonksiyonlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Fonksiyonlar.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Fonksiyonlar.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Fonksiyonlar.Image")));
            this.İmar_Fonksiyonlar.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Fonksiyonlar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Fonksiyonlar.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Fonksiyonlar.Name = "İmar_Fonksiyonlar";
            this.İmar_Fonksiyonlar.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Fonksiyonlar.Size = new System.Drawing.Size(146, 29);
            this.İmar_Fonksiyonlar.Tag = "";
            this.İmar_Fonksiyonlar.Text = "Fonksiyonlar";
            this.İmar_Fonksiyonlar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Fonksiyonlar.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Fonksiyonlar.ToolTipText = "Çeşitli vektörel veya tabular algoritmaları içerir.";
            this.İmar_Fonksiyonlar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.İmar_Fonksiyonlar_MouseDown);
            // 
            // tab_optDTR
            // 
            this.tab_optDTR.Location = new System.Drawing.Point(4, 30);
            this.tab_optDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_optDTR.Name = "tab_optDTR";
            this.tab_optDTR.Size = new System.Drawing.Size(1312, 693);
            this.tab_optDTR.TabIndex = 7;
            this.tab_optDTR.Text = "Optimal DTR Konumlandırma";
            this.tab_optDTR.UseVisualStyleBackColor = true;
            // 
            // tab_ekonometrik
            // 
            this.tab_ekonometrik.Controls.Add(this.SenaryoModulePanel);
            this.tab_ekonometrik.Location = new System.Drawing.Point(4, 30);
            this.tab_ekonometrik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ekonometrik.Name = "tab_ekonometrik";
            this.tab_ekonometrik.Size = new System.Drawing.Size(1312, 693);
            this.tab_ekonometrik.TabIndex = 3;
            this.tab_ekonometrik.Text = "Ekonometrik Talep Tahmini Modülü";
            this.tab_ekonometrik.UseVisualStyleBackColor = true;
            // 
            // SenaryoModulePanel
            // 
            this.SenaryoModulePanel.Controls.Add(this.EkonometrikSenaryoElementsPanel);
            this.SenaryoModulePanel.Controls.Add(this.SenaryoModuleTabControl);
            this.SenaryoModulePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModulePanel.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModulePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SenaryoModulePanel.Name = "SenaryoModulePanel";
            this.SenaryoModulePanel.Size = new System.Drawing.Size(1312, 693);
            this.SenaryoModulePanel.TabIndex = 0;
            // 
            // EkonometrikSenaryoElementsPanel
            // 
            this.EkonometrikSenaryoElementsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EkonometrikSenaryoElementsPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.buton_ELF_tablo_sec);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.textBox_sonuc_ELF);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.label_s_ELF);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.label_graphics);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.comboBox_ekonometrik);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFTahminButonu);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFScenerioSaveButton);
            this.EkonometrikSenaryoElementsPanel.Location = new System.Drawing.Point(0, 0);
            this.EkonometrikSenaryoElementsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoElementsPanel.Name = "EkonometrikSenaryoElementsPanel";
            this.EkonometrikSenaryoElementsPanel.Size = new System.Drawing.Size(250, 693);
            this.EkonometrikSenaryoElementsPanel.TabIndex = 1;
            // 
            // buton_ELF_tablo_sec
            // 
            this.buton_ELF_tablo_sec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_ELF_tablo_sec.Location = new System.Drawing.Point(46, 519);
            this.buton_ELF_tablo_sec.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_ELF_tablo_sec.Name = "buton_ELF_tablo_sec";
            this.buton_ELF_tablo_sec.Size = new System.Drawing.Size(125, 33);
            this.buton_ELF_tablo_sec.TabIndex = 19;
            this.buton_ELF_tablo_sec.Text = "Tabloyu Seç";
            this.buton_ELF_tablo_sec.UseVisualStyleBackColor = true;
            this.buton_ELF_tablo_sec.Visible = false;
            this.buton_ELF_tablo_sec.Click += new System.EventHandler(this.buton_ELF_tablo_sec_Click);
            // 
            // textBox_sonuc_ELF
            // 
            this.textBox_sonuc_ELF.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_sonuc_ELF.Location = new System.Drawing.Point(15, 256);
            this.textBox_sonuc_ELF.Multiline = true;
            this.textBox_sonuc_ELF.Name = "textBox_sonuc_ELF";
            this.textBox_sonuc_ELF.Size = new System.Drawing.Size(223, 66);
            this.textBox_sonuc_ELF.TabIndex = 18;
            this.textBox_sonuc_ELF.Visible = false;
            // 
            // label_s_ELF
            // 
            this.label_s_ELF.AutoSize = true;
            this.label_s_ELF.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_s_ELF.Location = new System.Drawing.Point(25, 214);
            this.label_s_ELF.Name = "label_s_ELF";
            this.label_s_ELF.Size = new System.Drawing.Size(182, 23);
            this.label_s_ELF.TabIndex = 16;
            this.label_s_ELF.Text = "Güncel Sonuç Dosyası:";
            this.label_s_ELF.Visible = false;
            // 
            // label_graphics
            // 
            this.label_graphics.AutoSize = true;
            this.label_graphics.ForeColor = System.Drawing.Color.Navy;
            this.label_graphics.Location = new System.Drawing.Point(74, 362);
            this.label_graphics.Name = "label_graphics";
            this.label_graphics.Size = new System.Drawing.Size(91, 23);
            this.label_graphics.TabIndex = 15;
            this.label_graphics.Text = "Grafik Seç:";
            this.label_graphics.Visible = false;
            // 
            // comboBox_ekonometrik
            // 
            this.comboBox_ekonometrik.DropDownHeight = 150;
            this.comboBox_ekonometrik.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ekonometrik.DropDownWidth = 250;
            this.comboBox_ekonometrik.Font = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_ekonometrik.FormattingEnabled = true;
            this.comboBox_ekonometrik.IntegralHeight = false;
            this.comboBox_ekonometrik.ItemHeight = 20;
            this.comboBox_ekonometrik.Location = new System.Drawing.Point(28, 388);
            this.comboBox_ekonometrik.MinimumSize = new System.Drawing.Size(170, 0);
            this.comboBox_ekonometrik.Name = "comboBox_ekonometrik";
            this.comboBox_ekonometrik.Size = new System.Drawing.Size(199, 28);
            this.comboBox_ekonometrik.TabIndex = 14;
            this.comboBox_ekonometrik.Visible = false;
            this.comboBox_ekonometrik.DropDown += new System.EventHandler(this.comboBox_ekonometrik_DropDown);
            this.comboBox_ekonometrik.SelectedIndexChanged += new System.EventHandler(this.comboBox_ekonometrik_SelectedIndexChanged);
            // 
            // ELFTahminButonu
            // 
            this.ELFTahminButonu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ELFTahminButonu.FlatAppearance.BorderSize = 0;
            this.ELFTahminButonu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ELFTahminButonu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ELFTahminButonu.ForeColor = System.Drawing.Color.White;
            this.ELFTahminButonu.Location = new System.Drawing.Point(52, 114);
            this.ELFTahminButonu.Margin = new System.Windows.Forms.Padding(4);
            this.ELFTahminButonu.Name = "ELFTahminButonu";
            this.ELFTahminButonu.Size = new System.Drawing.Size(141, 60);
            this.ELFTahminButonu.TabIndex = 13;
            this.ELFTahminButonu.Text = "Tahmin Yap";
            this.ELFTahminButonu.UseVisualStyleBackColor = false;
            this.ELFTahminButonu.Click += new System.EventHandler(this.ELFTahminButonu_Click);
            // 
            // ELFScenerioSaveButton
            // 
            this.ELFScenerioSaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ELFScenerioSaveButton.FlatAppearance.BorderSize = 0;
            this.ELFScenerioSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ELFScenerioSaveButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ELFScenerioSaveButton.ForeColor = System.Drawing.Color.White;
            this.ELFScenerioSaveButton.Location = new System.Drawing.Point(52, 27);
            this.ELFScenerioSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFScenerioSaveButton.Name = "ELFScenerioSaveButton";
            this.ELFScenerioSaveButton.Size = new System.Drawing.Size(141, 53);
            this.ELFScenerioSaveButton.TabIndex = 12;
            this.ELFScenerioSaveButton.Text = "Değişiklikleri Kaydet";
            this.ELFScenerioSaveButton.UseVisualStyleBackColor = false;
            this.ELFScenerioSaveButton.Click += new System.EventHandler(this.ELFScenerioSaveButton_Click);
            // 
            // SenaryoModuleTabControl
            // 
            this.SenaryoModuleTabControl.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.SenaryoModuleTabControl.Controls.Add(this.EkonometrikSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.EkonometrikSonuclarTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.EkonometrikGrafiklerTabPage);
            this.SenaryoModuleTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModuleTabControl.ItemSize = new System.Drawing.Size(220, 40);
            this.SenaryoModuleTabControl.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModuleTabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SenaryoModuleTabControl.Name = "SenaryoModuleTabControl";
            this.SenaryoModuleTabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SenaryoModuleTabControl.SelectedIndex = 0;
            this.SenaryoModuleTabControl.Size = new System.Drawing.Size(1312, 693);
            this.SenaryoModuleTabControl.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.SenaryoModuleTabControl.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.SenaryoModuleTabControl.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.SenaryoModuleTabControl.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.SenaryoModuleTabControl.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.SenaryoModuleTabControl.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.SenaryoModuleTabControl.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.SenaryoModuleTabControl.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.SenaryoModuleTabControl.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.SenaryoModuleTabControl.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.SenaryoModuleTabControl.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.SenaryoModuleTabControl.TabButtonSelectedState.FillColor = System.Drawing.Color.LightSalmon;
            this.SenaryoModuleTabControl.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.SenaryoModuleTabControl.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.SenaryoModuleTabControl.TabButtonSelectedState.InnerColor = System.Drawing.Color.LightGreen;
            this.SenaryoModuleTabControl.TabButtonSize = new System.Drawing.Size(220, 40);
            this.SenaryoModuleTabControl.TabIndex = 0;
            this.SenaryoModuleTabControl.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.SenaryoModuleTabControl.TabMenuOrientation = Guna.UI2.WinForms.TabMenuOrientation.VerticalRight;
            this.SenaryoModuleTabControl.SelectedIndexChanged += new System.EventHandler(this.SenaryoModuleTabControl_SelectedIndexChanged);
            // 
            // EkonometrikSenaryoTabPage
            // 
            this.EkonometrikSenaryoTabPage.Controls.Add(this.ELFSenaryoTabControls);
            this.EkonometrikSenaryoTabPage.Location = new System.Drawing.Point(4, 4);
            this.EkonometrikSenaryoTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoTabPage.Name = "EkonometrikSenaryoTabPage";
            this.EkonometrikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoTabPage.Size = new System.Drawing.Size(1084, 685);
            this.EkonometrikSenaryoTabPage.TabIndex = 0;
            this.EkonometrikSenaryoTabPage.Text = "Senaryo Oluştur";
            this.EkonometrikSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFSenaryoTabControls
            // 
            this.ELFSenaryoTabControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage_min_senaryo);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage_dusuk_senaryo);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage_baz_senaryo);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage_yuksek_senaryo);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage_maks_senaryo);
            this.ELFSenaryoTabControls.Location = new System.Drawing.Point(252, 4);
            this.ELFSenaryoTabControls.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFSenaryoTabControls.Name = "ELFSenaryoTabControls";
            this.ELFSenaryoTabControls.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ELFSenaryoTabControls.SelectedIndex = 0;
            this.ELFSenaryoTabControls.Size = new System.Drawing.Size(832, 677);
            this.ELFSenaryoTabControls.TabIndex = 2;
            // 
            // tabPage_min_senaryo
            // 
            this.tabPage_min_senaryo.Controls.Add(this.ELFMinSenaryoTable);
            this.tabPage_min_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_min_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_min_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_min_senaryo.Name = "tabPage_min_senaryo";
            this.tabPage_min_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_min_senaryo.Size = new System.Drawing.Size(824, 643);
            this.tabPage_min_senaryo.TabIndex = 0;
            this.tabPage_min_senaryo.Text = "Minimum Senaryo";
            this.tabPage_min_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFMinSenaryoTable
            // 
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFMinSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.ELFMinSenaryoTable.BackgroundColor = System.Drawing.Color.White;
            this.ELFMinSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinSenaryoTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMinSenaryoTable.DefaultCellStyle = dataGridViewCellStyle5;
            this.ELFMinSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMinSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMinSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMinSenaryoTable.Name = "ELFMinSenaryoTable";
            this.ELFMinSenaryoTable.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFMinSenaryoTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.ELFMinSenaryoTable.RowHeadersWidth = 18;
            this.ELFMinSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMinSenaryoTable.Size = new System.Drawing.Size(818, 639);
            this.ELFMinSenaryoTable.TabIndex = 0;
            this.ELFMinSenaryoTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFMinSenaryoTable_EditingControlShowing);
            // 
            // tabPage_dusuk_senaryo
            // 
            this.tabPage_dusuk_senaryo.Controls.Add(this.ELFLowSenaryoTable);
            this.tabPage_dusuk_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_dusuk_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_dusuk_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_senaryo.Name = "tabPage_dusuk_senaryo";
            this.tabPage_dusuk_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_senaryo.Size = new System.Drawing.Size(824, 617);
            this.tabPage_dusuk_senaryo.TabIndex = 1;
            this.tabPage_dusuk_senaryo.Text = "Düşük Senaryo";
            this.tabPage_dusuk_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFLowSenaryoTable
            // 
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFLowSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.ELFLowSenaryoTable.BackgroundColor = System.Drawing.Color.White;
            this.ELFLowSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFLowSenaryoTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFLowSenaryoTable.DefaultCellStyle = dataGridViewCellStyle8;
            this.ELFLowSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFLowSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFLowSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFLowSenaryoTable.Name = "ELFLowSenaryoTable";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFLowSenaryoTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.ELFLowSenaryoTable.RowHeadersWidth = 18;
            this.ELFLowSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFLowSenaryoTable.Size = new System.Drawing.Size(818, 613);
            this.ELFLowSenaryoTable.TabIndex = 1;
            this.ELFLowSenaryoTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFLowSenaryoTable_EditingControlShowing);
            // 
            // tabPage_baz_senaryo
            // 
            this.tabPage_baz_senaryo.Controls.Add(this.ELFBaseSenaryoTable);
            this.tabPage_baz_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_baz_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_baz_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_senaryo.Name = "tabPage_baz_senaryo";
            this.tabPage_baz_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_senaryo.Size = new System.Drawing.Size(824, 617);
            this.tabPage_baz_senaryo.TabIndex = 2;
            this.tabPage_baz_senaryo.Text = "Baz Senaryo";
            this.tabPage_baz_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFBaseSenaryoTable
            // 
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFBaseSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.ELFBaseSenaryoTable.BackgroundColor = System.Drawing.Color.White;
            this.ELFBaseSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBaseSenaryoTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFBaseSenaryoTable.DefaultCellStyle = dataGridViewCellStyle11;
            this.ELFBaseSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBaseSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFBaseSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBaseSenaryoTable.Name = "ELFBaseSenaryoTable";
            this.ELFBaseSenaryoTable.RowHeadersWidth = 18;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ELFBaseSenaryoTable.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.ELFBaseSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFBaseSenaryoTable.Size = new System.Drawing.Size(818, 613);
            this.ELFBaseSenaryoTable.TabIndex = 1;
            this.ELFBaseSenaryoTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFBaseSenaryoTable_EditingControlShowing);
            // 
            // tabPage_yuksek_senaryo
            // 
            this.tabPage_yuksek_senaryo.Controls.Add(this.ELFHighSenaryoTable);
            this.tabPage_yuksek_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_yuksek_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_yuksek_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_senaryo.Name = "tabPage_yuksek_senaryo";
            this.tabPage_yuksek_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_senaryo.Size = new System.Drawing.Size(824, 617);
            this.tabPage_yuksek_senaryo.TabIndex = 3;
            this.tabPage_yuksek_senaryo.Text = "Yüksek Senaryo";
            this.tabPage_yuksek_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFHighSenaryoTable
            // 
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFHighSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.ELFHighSenaryoTable.BackgroundColor = System.Drawing.Color.White;
            this.ELFHighSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFHighSenaryoTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFHighSenaryoTable.DefaultCellStyle = dataGridViewCellStyle14;
            this.ELFHighSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFHighSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFHighSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFHighSenaryoTable.Name = "ELFHighSenaryoTable";
            this.ELFHighSenaryoTable.RowHeadersWidth = 51;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ELFHighSenaryoTable.RowsDefaultCellStyle = dataGridViewCellStyle15;
            this.ELFHighSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFHighSenaryoTable.Size = new System.Drawing.Size(818, 613);
            this.ELFHighSenaryoTable.TabIndex = 1;
            this.ELFHighSenaryoTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFHighSenaryoTable_EditingControlShowing);
            // 
            // tabPage_maks_senaryo
            // 
            this.tabPage_maks_senaryo.Controls.Add(this.ELFMaxSenaryoTable);
            this.tabPage_maks_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_maks_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_maks_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_senaryo.Name = "tabPage_maks_senaryo";
            this.tabPage_maks_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_senaryo.Size = new System.Drawing.Size(824, 617);
            this.tabPage_maks_senaryo.TabIndex = 4;
            this.tabPage_maks_senaryo.Text = "Maksimum Senaryo";
            this.tabPage_maks_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFMaxSenaryoTable
            // 
            this.ELFMaxSenaryoTable.BackgroundColor = System.Drawing.Color.White;
            this.ELFMaxSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMaxSenaryoTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMaxSenaryoTable.DefaultCellStyle = dataGridViewCellStyle16;
            this.ELFMaxSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMaxSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMaxSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMaxSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaxSenaryoTable.Name = "ELFMaxSenaryoTable";
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFMaxSenaryoTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.ELFMaxSenaryoTable.RowHeadersWidth = 18;
            this.ELFMaxSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMaxSenaryoTable.Size = new System.Drawing.Size(818, 613);
            this.ELFMaxSenaryoTable.TabIndex = 1;
            this.ELFMaxSenaryoTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFMaxSenaryoTable_EditingControlShowing);
            // 
            // EkonometrikSonuclarTabPage
            // 
            this.EkonometrikSonuclarTabPage.Controls.Add(this.ELFSonuçlarTabControls);
            this.EkonometrikSonuclarTabPage.Location = new System.Drawing.Point(4, 4);
            this.EkonometrikSonuclarTabPage.Name = "EkonometrikSonuclarTabPage";
            this.EkonometrikSonuclarTabPage.Size = new System.Drawing.Size(1084, 659);
            this.EkonometrikSonuclarTabPage.TabIndex = 1;
            this.EkonometrikSonuclarTabPage.Text = "Sonuçları Görüntüle";
            this.EkonometrikSonuclarTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFSonuçlarTabControls
            // 
            this.ELFSonuçlarTabControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFSonuçlarTabControls.Controls.Add(this.tabPage_min_sonuclar);
            this.ELFSonuçlarTabControls.Controls.Add(this.tabPage_dusuk_sonuclar);
            this.ELFSonuçlarTabControls.Controls.Add(this.tabPage_baz_sonuclar);
            this.ELFSonuçlarTabControls.Controls.Add(this.tabPage_yuksek_sonuclar);
            this.ELFSonuçlarTabControls.Controls.Add(this.tabPage_maks_sonuclar);
            this.ELFSonuçlarTabControls.Location = new System.Drawing.Point(252, 2);
            this.ELFSonuçlarTabControls.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFSonuçlarTabControls.Name = "ELFSonuçlarTabControls";
            this.ELFSonuçlarTabControls.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ELFSonuçlarTabControls.SelectedIndex = 0;
            this.ELFSonuçlarTabControls.Size = new System.Drawing.Size(829, 527);
            this.ELFSonuçlarTabControls.TabIndex = 4;
            // 
            // tabPage_min_sonuclar
            // 
            this.tabPage_min_sonuclar.Controls.Add(this.ELFMinimumResultsTable);
            this.tabPage_min_sonuclar.Location = new System.Drawing.Point(4, 30);
            this.tabPage_min_sonuclar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_min_sonuclar.Name = "tabPage_min_sonuclar";
            this.tabPage_min_sonuclar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_min_sonuclar.Size = new System.Drawing.Size(821, 493);
            this.tabPage_min_sonuclar.TabIndex = 0;
            this.tabPage_min_sonuclar.Text = "Minimum Sonuçlar";
            this.tabPage_min_sonuclar.UseVisualStyleBackColor = true;
            // 
            // ELFMinimumResultsTable
            // 
            this.ELFMinimumResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFMinimumResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinimumResultsTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMinimumResultsTable.DefaultCellStyle = dataGridViewCellStyle18;
            this.ELFMinimumResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinimumResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMinimumResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMinimumResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMinimumResultsTable.Name = "ELFMinimumResultsTable";
            this.ELFMinimumResultsTable.RowHeadersWidth = 51;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ELFMinimumResultsTable.RowsDefaultCellStyle = dataGridViewCellStyle19;
            this.ELFMinimumResultsTable.Size = new System.Drawing.Size(815, 489);
            this.ELFMinimumResultsTable.TabIndex = 0;
            this.ELFMinimumResultsTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFMinimumResultsTable_EditingControlShowing);
            // 
            // tabPage_dusuk_sonuclar
            // 
            this.tabPage_dusuk_sonuclar.Controls.Add(this.ELFDüşükResultsTable);
            this.tabPage_dusuk_sonuclar.Location = new System.Drawing.Point(4, 30);
            this.tabPage_dusuk_sonuclar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_sonuclar.Name = "tabPage_dusuk_sonuclar";
            this.tabPage_dusuk_sonuclar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_sonuclar.Size = new System.Drawing.Size(821, 493);
            this.tabPage_dusuk_sonuclar.TabIndex = 1;
            this.tabPage_dusuk_sonuclar.Text = "Düşük Sonuçlar";
            this.tabPage_dusuk_sonuclar.UseVisualStyleBackColor = true;
            // 
            // ELFDüşükResultsTable
            // 
            this.ELFDüşükResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFDüşükResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFDüşükResultsTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFDüşükResultsTable.DefaultCellStyle = dataGridViewCellStyle20;
            this.ELFDüşükResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFDüşükResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFDüşükResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFDüşükResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFDüşükResultsTable.Name = "ELFDüşükResultsTable";
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFDüşükResultsTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.ELFDüşükResultsTable.RowHeadersWidth = 51;
            this.ELFDüşükResultsTable.Size = new System.Drawing.Size(815, 489);
            this.ELFDüşükResultsTable.TabIndex = 1;
            this.ELFDüşükResultsTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFDüşükResultsTable_EditingControlShowing);
            // 
            // tabPage_baz_sonuclar
            // 
            this.tabPage_baz_sonuclar.Controls.Add(this.ELFBazResultsTable);
            this.tabPage_baz_sonuclar.Location = new System.Drawing.Point(4, 30);
            this.tabPage_baz_sonuclar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_sonuclar.Name = "tabPage_baz_sonuclar";
            this.tabPage_baz_sonuclar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_sonuclar.Size = new System.Drawing.Size(821, 493);
            this.tabPage_baz_sonuclar.TabIndex = 2;
            this.tabPage_baz_sonuclar.Text = "Baz Sonuçlar";
            this.tabPage_baz_sonuclar.UseVisualStyleBackColor = true;
            // 
            // ELFBazResultsTable
            // 
            this.ELFBazResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFBazResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBazResultsTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFBazResultsTable.DefaultCellStyle = dataGridViewCellStyle22;
            this.ELFBazResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBazResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBazResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFBazResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBazResultsTable.Name = "ELFBazResultsTable";
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle23.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFBazResultsTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle23;
            this.ELFBazResultsTable.RowHeadersWidth = 51;
            this.ELFBazResultsTable.Size = new System.Drawing.Size(815, 489);
            this.ELFBazResultsTable.TabIndex = 1;
            this.ELFBazResultsTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFBazResultsTable_EditingControlShowing);
            // 
            // tabPage_yuksek_sonuclar
            // 
            this.tabPage_yuksek_sonuclar.Controls.Add(this.ELFYüksekResultsTable);
            this.tabPage_yuksek_sonuclar.Location = new System.Drawing.Point(4, 30);
            this.tabPage_yuksek_sonuclar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_sonuclar.Name = "tabPage_yuksek_sonuclar";
            this.tabPage_yuksek_sonuclar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_sonuclar.Size = new System.Drawing.Size(821, 493);
            this.tabPage_yuksek_sonuclar.TabIndex = 3;
            this.tabPage_yuksek_sonuclar.Text = "Yüksek Sonuçlar";
            this.tabPage_yuksek_sonuclar.UseVisualStyleBackColor = true;
            // 
            // ELFYüksekResultsTable
            // 
            this.ELFYüksekResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFYüksekResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFYüksekResultsTable.ColumnHeadersHeight = 29;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFYüksekResultsTable.DefaultCellStyle = dataGridViewCellStyle24;
            this.ELFYüksekResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFYüksekResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFYüksekResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFYüksekResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFYüksekResultsTable.Name = "ELFYüksekResultsTable";
            this.ELFYüksekResultsTable.RowHeadersWidth = 51;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ELFYüksekResultsTable.RowsDefaultCellStyle = dataGridViewCellStyle25;
            this.ELFYüksekResultsTable.Size = new System.Drawing.Size(815, 489);
            this.ELFYüksekResultsTable.TabIndex = 1;
            this.ELFYüksekResultsTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFYüksekResultsTable_EditingControlShowing);
            // 
            // tabPage_maks_sonuclar
            // 
            this.tabPage_maks_sonuclar.Controls.Add(this.ELFMaksimumResultsTable);
            this.tabPage_maks_sonuclar.Location = new System.Drawing.Point(4, 30);
            this.tabPage_maks_sonuclar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_sonuclar.Name = "tabPage_maks_sonuclar";
            this.tabPage_maks_sonuclar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_sonuclar.Size = new System.Drawing.Size(821, 493);
            this.tabPage_maks_sonuclar.TabIndex = 4;
            this.tabPage_maks_sonuclar.Text = "Maksimum Sonuçlar";
            this.tabPage_maks_sonuclar.UseVisualStyleBackColor = true;
            // 
            // ELFMaksimumResultsTable
            // 
            this.ELFMaksimumResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFMaksimumResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMaksimumResultsTable.ColumnHeadersHeight = 29;
            this.ELFMaksimumResultsTable.Cursor = System.Windows.Forms.Cursors.Default;
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle26.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle26.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMaksimumResultsTable.DefaultCellStyle = dataGridViewCellStyle26;
            this.ELFMaksimumResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMaksimumResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMaksimumResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMaksimumResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaksimumResultsTable.Name = "ELFMaksimumResultsTable";
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFMaksimumResultsTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle27;
            this.ELFMaksimumResultsTable.RowHeadersWidth = 51;
            this.ELFMaksimumResultsTable.Size = new System.Drawing.Size(815, 489);
            this.ELFMaksimumResultsTable.TabIndex = 1;
            this.ELFMaksimumResultsTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.ELFMaksimumResultsTable_EditingControlShowing);
            // 
            // EkonometrikGrafiklerTabPage
            // 
            this.EkonometrikGrafiklerTabPage.Controls.Add(this.pictureBox_ekonometrik);
            this.EkonometrikGrafiklerTabPage.Location = new System.Drawing.Point(4, 4);
            this.EkonometrikGrafiklerTabPage.Name = "EkonometrikGrafiklerTabPage";
            this.EkonometrikGrafiklerTabPage.Size = new System.Drawing.Size(1084, 659);
            this.EkonometrikGrafiklerTabPage.TabIndex = 2;
            this.EkonometrikGrafiklerTabPage.Text = "Grafikler";
            this.EkonometrikGrafiklerTabPage.UseVisualStyleBackColor = true;
            // 
            // pictureBox_ekonometrik
            // 
            this.pictureBox_ekonometrik.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_ekonometrik.Location = new System.Drawing.Point(292, 84);
            this.pictureBox_ekonometrik.Name = "pictureBox_ekonometrik";
            this.pictureBox_ekonometrik.Size = new System.Drawing.Size(723, 462);
            this.pictureBox_ekonometrik.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ekonometrik.TabIndex = 0;
            this.pictureBox_ekonometrik.TabStop = false;
            // 
            // tab_yükHaritası
            // 
            this.tab_yükHaritası.Controls.Add(this.buton_HTML);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_main);
            this.tab_yükHaritası.Controls.Add(this.panel_yuk);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_15);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_8);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_6);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_11);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_14);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_13);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_12);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_10);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_9);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_7);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_4);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_5);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_3);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_2);
            this.tab_yükHaritası.Controls.Add(this.checkBox_yuk_1);
            this.tab_yükHaritası.Controls.Add(this.legendPanel);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_deger);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_text);
            this.tab_yükHaritası.Controls.Add(this.trackBar_Yıllar);
            this.tab_yükHaritası.ImageIndex = 13;
            this.tab_yükHaritası.Location = new System.Drawing.Point(4, 56);
            this.tab_yükHaritası.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_yükHaritası.Name = "tab_yükHaritası";
            this.tab_yükHaritası.Size = new System.Drawing.Size(1312, 667);
            this.tab_yükHaritası.TabIndex = 9;
            this.tab_yükHaritası.Text = "Yük Yoğunluğu Haritası";
            this.tab_yükHaritası.UseVisualStyleBackColor = true;
            // 
            // buton_HTML
            // 
            this.buton_HTML.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_HTML.BackgroundImage")));
            this.buton_HTML.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_HTML.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buton_HTML.Location = new System.Drawing.Point(239, 37);
            this.buton_HTML.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_HTML.Name = "buton_HTML";
            this.buton_HTML.Size = new System.Drawing.Size(44, 44);
            this.buton_HTML.TabIndex = 74;
            this.buton_HTML.TabStop = false;
            this.buton_HTML.Visible = false;
            this.buton_HTML.Click += new System.EventHandler(this.buton_HTML_Click);
            // 
            // checkBox_yuk_main
            // 
            this.checkBox_yuk_main.AutoSize = true;
            this.checkBox_yuk_main.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_main.Location = new System.Drawing.Point(299, 6);
            this.checkBox_yuk_main.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_main.Name = "checkBox_yuk_main";
            this.checkBox_yuk_main.Size = new System.Drawing.Size(185, 27);
            this.checkBox_yuk_main.TabIndex = 73;
            this.checkBox_yuk_main.Text = "checkBox_yuk_main";
            this.checkBox_yuk_main.UseVisualStyleBackColor = true;
            this.checkBox_yuk_main.Visible = false;
            this.checkBox_yuk_main.CheckedChanged += new System.EventHandler(this.checkBox_yuk_main_CheckedChanged);
            // 
            // panel_yuk
            // 
            this.panel_yuk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_yuk.Controls.Add(this.webView_yuk);
            this.panel_yuk.Controls.Add(this.gMapControl_yuk);
            this.panel_yuk.Controls.Add(this.buton_yuk_haritası_katmanlar);
            this.panel_yuk.Location = new System.Drawing.Point(299, 37);
            this.panel_yuk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_yuk.Name = "panel_yuk";
            this.panel_yuk.Size = new System.Drawing.Size(757, 596);
            this.panel_yuk.TabIndex = 72;
            // 
            // webView_yuk
            // 
            this.webView_yuk.AllowExternalDrop = true;
            this.webView_yuk.CreationProperties = null;
            this.webView_yuk.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_yuk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_yuk.Location = new System.Drawing.Point(0, 0);
            this.webView_yuk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_yuk.Name = "webView_yuk";
            this.webView_yuk.Size = new System.Drawing.Size(757, 596);
            this.webView_yuk.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_yuk.TabIndex = 71;
            this.webView_yuk.Visible = false;
            this.webView_yuk.ZoomFactor = 1D;
            // 
            // gMapControl_yuk
            // 
            this.gMapControl_yuk.AllowDrop = true;
            this.gMapControl_yuk.Bearing = 0F;
            this.gMapControl_yuk.CanDragMap = true;
            this.gMapControl_yuk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_yuk.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_yuk.GrayScaleMode = false;
            this.gMapControl_yuk.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_yuk.LevelsKeepInMemory = 5;
            this.gMapControl_yuk.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_yuk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gMapControl_yuk.MarkersEnabled = true;
            this.gMapControl_yuk.MaxZoom = 2;
            this.gMapControl_yuk.MinZoom = 2;
            this.gMapControl_yuk.MouseWheelZoomEnabled = true;
            this.gMapControl_yuk.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_yuk.Name = "gMapControl_yuk";
            this.gMapControl_yuk.NegativeMode = false;
            this.gMapControl_yuk.PolygonsEnabled = true;
            this.gMapControl_yuk.RetryLoadTile = 0;
            this.gMapControl_yuk.RoutesEnabled = true;
            this.gMapControl_yuk.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_yuk.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_yuk.ShowTileGridLines = false;
            this.gMapControl_yuk.Size = new System.Drawing.Size(757, 596);
            this.gMapControl_yuk.TabIndex = 34;
            this.gMapControl_yuk.Zoom = 0D;
            this.gMapControl_yuk.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_yuk_OnMapDoubleClick);
            this.gMapControl_yuk.MouseLeave += new System.EventHandler(this.gMapControl_yuk_MouseLeave);
            this.gMapControl_yuk.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseMove);
            // 
            // buton_yuk_haritası_katmanlar
            // 
            this.buton_yuk_haritası_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_yuk_haritası_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_yuk_haritası_katmanlar.BackgroundImage")));
            this.buton_yuk_haritası_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_yuk_haritası_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_yuk_haritası_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_yuk_haritası_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_yuk_haritası_katmanlar.Location = new System.Drawing.Point(3, 542);
            this.buton_yuk_haritası_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_yuk_haritası_katmanlar.Name = "buton_yuk_haritası_katmanlar";
            this.buton_yuk_haritası_katmanlar.Size = new System.Drawing.Size(59, 52);
            this.buton_yuk_haritası_katmanlar.TabIndex = 35;
            this.buton_yuk_haritası_katmanlar.UseVisualStyleBackColor = true;
            // 
            // checkBox_yuk_15
            // 
            this.checkBox_yuk_15.AutoSize = true;
            this.checkBox_yuk_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_15.Location = new System.Drawing.Point(8, 601);
            this.checkBox_yuk_15.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_15.Name = "checkBox_yuk_15";
            this.checkBox_yuk_15.Size = new System.Drawing.Size(163, 27);
            this.checkBox_yuk_15.TabIndex = 70;
            this.checkBox_yuk_15.Text = "checkBox_yuk_15";
            this.checkBox_yuk_15.UseVisualStyleBackColor = true;
            this.checkBox_yuk_15.Visible = false;
            // 
            // checkBox_yuk_8
            // 
            this.checkBox_yuk_8.AutoSize = true;
            this.checkBox_yuk_8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_8.Location = new System.Drawing.Point(8, 354);
            this.checkBox_yuk_8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_8.Name = "checkBox_yuk_8";
            this.checkBox_yuk_8.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_8.TabIndex = 69;
            this.checkBox_yuk_8.Text = "checkBox_yuk_8";
            this.checkBox_yuk_8.UseVisualStyleBackColor = true;
            this.checkBox_yuk_8.Visible = false;
            // 
            // checkBox_yuk_6
            // 
            this.checkBox_yuk_6.AutoSize = true;
            this.checkBox_yuk_6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_6.Location = new System.Drawing.Point(8, 286);
            this.checkBox_yuk_6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_6.Name = "checkBox_yuk_6";
            this.checkBox_yuk_6.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_6.TabIndex = 68;
            this.checkBox_yuk_6.Text = "checkBox_yuk_6";
            this.checkBox_yuk_6.UseVisualStyleBackColor = true;
            this.checkBox_yuk_6.Visible = false;
            // 
            // checkBox_yuk_11
            // 
            this.checkBox_yuk_11.AutoSize = true;
            this.checkBox_yuk_11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_11.Location = new System.Drawing.Point(8, 460);
            this.checkBox_yuk_11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_11.Name = "checkBox_yuk_11";
            this.checkBox_yuk_11.Size = new System.Drawing.Size(161, 27);
            this.checkBox_yuk_11.TabIndex = 67;
            this.checkBox_yuk_11.Text = "checkBox_yuk_11";
            this.checkBox_yuk_11.UseVisualStyleBackColor = true;
            this.checkBox_yuk_11.Visible = false;
            // 
            // checkBox_yuk_14
            // 
            this.checkBox_yuk_14.AutoSize = true;
            this.checkBox_yuk_14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_14.Location = new System.Drawing.Point(8, 565);
            this.checkBox_yuk_14.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_14.Name = "checkBox_yuk_14";
            this.checkBox_yuk_14.Size = new System.Drawing.Size(164, 27);
            this.checkBox_yuk_14.TabIndex = 66;
            this.checkBox_yuk_14.Text = "checkBox_yuk_14";
            this.checkBox_yuk_14.UseVisualStyleBackColor = true;
            this.checkBox_yuk_14.Visible = false;
            // 
            // checkBox_yuk_13
            // 
            this.checkBox_yuk_13.AutoSize = true;
            this.checkBox_yuk_13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_13.Location = new System.Drawing.Point(8, 530);
            this.checkBox_yuk_13.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_13.Name = "checkBox_yuk_13";
            this.checkBox_yuk_13.Size = new System.Drawing.Size(163, 27);
            this.checkBox_yuk_13.TabIndex = 65;
            this.checkBox_yuk_13.Text = "checkBox_yuk_13";
            this.checkBox_yuk_13.UseVisualStyleBackColor = true;
            this.checkBox_yuk_13.Visible = false;
            // 
            // checkBox_yuk_12
            // 
            this.checkBox_yuk_12.AutoSize = true;
            this.checkBox_yuk_12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_12.Location = new System.Drawing.Point(8, 495);
            this.checkBox_yuk_12.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_12.Name = "checkBox_yuk_12";
            this.checkBox_yuk_12.Size = new System.Drawing.Size(163, 27);
            this.checkBox_yuk_12.TabIndex = 64;
            this.checkBox_yuk_12.Text = "checkBox_yuk_12";
            this.checkBox_yuk_12.UseVisualStyleBackColor = true;
            this.checkBox_yuk_12.Visible = false;
            // 
            // checkBox_yuk_10
            // 
            this.checkBox_yuk_10.AutoSize = true;
            this.checkBox_yuk_10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_10.Location = new System.Drawing.Point(8, 425);
            this.checkBox_yuk_10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_10.Name = "checkBox_yuk_10";
            this.checkBox_yuk_10.Size = new System.Drawing.Size(163, 27);
            this.checkBox_yuk_10.TabIndex = 63;
            this.checkBox_yuk_10.Text = "checkBox_yuk_10";
            this.checkBox_yuk_10.UseVisualStyleBackColor = true;
            this.checkBox_yuk_10.Visible = false;
            // 
            // checkBox_yuk_9
            // 
            this.checkBox_yuk_9.AutoSize = true;
            this.checkBox_yuk_9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_9.Location = new System.Drawing.Point(8, 390);
            this.checkBox_yuk_9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_9.Name = "checkBox_yuk_9";
            this.checkBox_yuk_9.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_9.TabIndex = 62;
            this.checkBox_yuk_9.Text = "checkBox_yuk_9";
            this.checkBox_yuk_9.UseVisualStyleBackColor = true;
            this.checkBox_yuk_9.Visible = false;
            // 
            // checkBox_yuk_7
            // 
            this.checkBox_yuk_7.AutoSize = true;
            this.checkBox_yuk_7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_7.Location = new System.Drawing.Point(8, 320);
            this.checkBox_yuk_7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_7.Name = "checkBox_yuk_7";
            this.checkBox_yuk_7.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_7.TabIndex = 61;
            this.checkBox_yuk_7.Text = "checkBox_yuk_7";
            this.checkBox_yuk_7.UseVisualStyleBackColor = true;
            this.checkBox_yuk_7.Visible = false;
            // 
            // checkBox_yuk_4
            // 
            this.checkBox_yuk_4.AutoSize = true;
            this.checkBox_yuk_4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_4.Location = new System.Drawing.Point(8, 215);
            this.checkBox_yuk_4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_4.Name = "checkBox_yuk_4";
            this.checkBox_yuk_4.Size = new System.Drawing.Size(157, 27);
            this.checkBox_yuk_4.TabIndex = 60;
            this.checkBox_yuk_4.Text = "checkBox_yuk_4";
            this.checkBox_yuk_4.UseVisualStyleBackColor = true;
            this.checkBox_yuk_4.Visible = false;
            // 
            // checkBox_yuk_5
            // 
            this.checkBox_yuk_5.AutoSize = true;
            this.checkBox_yuk_5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_5.Location = new System.Drawing.Point(8, 250);
            this.checkBox_yuk_5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_5.Name = "checkBox_yuk_5";
            this.checkBox_yuk_5.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_5.TabIndex = 59;
            this.checkBox_yuk_5.Text = "checkBox_yuk_5";
            this.checkBox_yuk_5.UseVisualStyleBackColor = true;
            this.checkBox_yuk_5.Visible = false;
            // 
            // checkBox_yuk_3
            // 
            this.checkBox_yuk_3.AutoSize = true;
            this.checkBox_yuk_3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_3.Location = new System.Drawing.Point(8, 180);
            this.checkBox_yuk_3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_3.Name = "checkBox_yuk_3";
            this.checkBox_yuk_3.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_3.TabIndex = 58;
            this.checkBox_yuk_3.Text = "checkBox_yuk_3";
            this.checkBox_yuk_3.UseVisualStyleBackColor = true;
            this.checkBox_yuk_3.Visible = false;
            // 
            // checkBox_yuk_2
            // 
            this.checkBox_yuk_2.AutoSize = true;
            this.checkBox_yuk_2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_2.Location = new System.Drawing.Point(8, 145);
            this.checkBox_yuk_2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_2.Name = "checkBox_yuk_2";
            this.checkBox_yuk_2.Size = new System.Drawing.Size(156, 27);
            this.checkBox_yuk_2.TabIndex = 57;
            this.checkBox_yuk_2.Text = "checkBox_yuk_2";
            this.checkBox_yuk_2.UseVisualStyleBackColor = true;
            this.checkBox_yuk_2.Visible = false;
            // 
            // checkBox_yuk_1
            // 
            this.checkBox_yuk_1.AutoSize = true;
            this.checkBox_yuk_1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_1.Location = new System.Drawing.Point(8, 110);
            this.checkBox_yuk_1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_yuk_1.Name = "checkBox_yuk_1";
            this.checkBox_yuk_1.Size = new System.Drawing.Size(154, 27);
            this.checkBox_yuk_1.TabIndex = 56;
            this.checkBox_yuk_1.Text = "checkBox_yuk_1";
            this.checkBox_yuk_1.UseVisualStyleBackColor = true;
            this.checkBox_yuk_1.Visible = false;
            // 
            // legendPanel
            // 
            this.legendPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.legendPanel.AutoSize = true;
            this.legendPanel.ForeColor = System.Drawing.Color.MediumBlue;
            this.legendPanel.Location = new System.Drawing.Point(1061, 37);
            this.legendPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(245, 596);
            this.legendPanel.TabIndex = 44;
            // 
            // yuk_yıl_deger
            // 
            this.yuk_yıl_deger.AutoSize = true;
            this.yuk_yıl_deger.Location = new System.Drawing.Point(32, 11);
            this.yuk_yıl_deger.Name = "yuk_yıl_deger";
            this.yuk_yıl_deger.Size = new System.Drawing.Size(46, 23);
            this.yuk_yıl_deger.TabIndex = 41;
            this.yuk_yıl_deger.Text = "2025";
            // 
            // yuk_yıl_text
            // 
            this.yuk_yıl_text.AutoSize = true;
            this.yuk_yıl_text.Location = new System.Drawing.Point(5, 11);
            this.yuk_yıl_text.Name = "yuk_yıl_text";
            this.yuk_yıl_text.Size = new System.Drawing.Size(32, 23);
            this.yuk_yıl_text.TabIndex = 40;
            this.yuk_yıl_text.Text = "Yıl:";
            // 
            // trackBar_Yıllar
            // 
            this.trackBar_Yıllar.Location = new System.Drawing.Point(5, 37);
            this.trackBar_Yıllar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar_Yıllar.Maximum = 2050;
            this.trackBar_Yıllar.Minimum = 2025;
            this.trackBar_Yıllar.Name = "trackBar_Yıllar";
            this.trackBar_Yıllar.Size = new System.Drawing.Size(217, 56);
            this.trackBar_Yıllar.SmallChange = 2;
            this.trackBar_Yıllar.TabIndex = 39;
            this.trackBar_Yıllar.Value = 2025;
            this.trackBar_Yıllar.ValueChanged += new System.EventHandler(this.trackBar_Yıllar_ValueChanged);
            // 
            // tab_rapor
            // 
            this.tab_rapor.Location = new System.Drawing.Point(4, 56);
            this.tab_rapor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_rapor.Name = "tab_rapor";
            this.tab_rapor.Size = new System.Drawing.Size(1312, 667);
            this.tab_rapor.TabIndex = 8;
            this.tab_rapor.Text = "Raporlama";
            this.tab_rapor.UseVisualStyleBackColor = true;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Sun.ico");
            this.imageList.Images.SetKeyName(1, "Profit.ico");
            this.imageList.Images.SetKeyName(2, "Car Charger.ico");
            this.imageList.Images.SetKeyName(3, "Money Circulation.ico");
            this.imageList.Images.SetKeyName(4, "Money Circulation2.ico");
            this.imageList.Images.SetKeyName(5, "Input.ico");
            this.imageList.Images.SetKeyName(6, "Input2.ico");
            this.imageList.Images.SetKeyName(7, "World Map3.ico");
            this.imageList.Images.SetKeyName(8, "World Map2.ico");
            this.imageList.Images.SetKeyName(9, "Texas.ico");
            this.imageList.Images.SetKeyName(10, "Map.ico");
            this.imageList.Images.SetKeyName(11, "World Map.ico");
            this.imageList.Images.SetKeyName(12, "Input3.ico");
            this.imageList.Images.SetKeyName(13, "png-clipart-heat-map-google-search-visualization-google-infographic-orange-thumbn" +
        "ail.ico");
            this.imageList.Images.SetKeyName(14, "graph-5_icon-icons.com_58023.ico");
            // 
            // RModelProgressBar
            // 
            this.RModelProgressBar.Location = new System.Drawing.Point(0, 0);
            this.RModelProgressBar.Name = "RModelProgressBar";
            this.RModelProgressBar.Size = new System.Drawing.Size(100, 23);
            this.RModelProgressBar.TabIndex = 0;
            // 
            // RModelStatusLabel
            // 
            this.RModelStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.RModelStatusLabel.Name = "RModelStatusLabel";
            this.RModelStatusLabel.Size = new System.Drawing.Size(100, 23);
            this.RModelStatusLabel.TabIndex = 0;
            // 
            // GelecekSimButton
            // 
            this.GelecekSimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.GelecekSimButton.FlatAppearance.BorderSize = 0;
            this.GelecekSimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GelecekSimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.GelecekSimButton.ForeColor = System.Drawing.Color.White;
            this.GelecekSimButton.Location = new System.Drawing.Point(44, 250);
            this.GelecekSimButton.Margin = new System.Windows.Forms.Padding(4);
            this.GelecekSimButton.Name = "GelecekSimButton";
            this.GelecekSimButton.Size = new System.Drawing.Size(187, 52);
            this.GelecekSimButton.TabIndex = 50;
            this.GelecekSimButton.Text = "Gelecek Similasyonu Görüntüle";
            this.GelecekSimButton.UseVisualStyleBackColor = false;
            this.GelecekSimButton.Click += new System.EventHandler(this.gelecekSimilasyonGoruntule);
            // 
            // DeepLearningModelButton
            // 
            this.DeepLearningModelButton.Location = new System.Drawing.Point(0, 0);
            this.DeepLearningModelButton.Name = "DeepLearningModelButton";
            this.DeepLearningModelButton.Size = new System.Drawing.Size(75, 23);
            this.DeepLearningModelButton.TabIndex = 0;
            // 
            // YeniGenislemeSidePanel
            // 
            this.YeniGenislemeSidePanel.Location = new System.Drawing.Point(0, 0);
            this.YeniGenislemeSidePanel.Name = "YeniGenislemeSidePanel";
            this.YeniGenislemeSidePanel.Size = new System.Drawing.Size(200, 100);
            this.YeniGenislemeSidePanel.TabIndex = 0;
            // 
            // label_yga_katmanlar
            // 
            this.label_yga_katmanlar.Location = new System.Drawing.Point(0, 0);
            this.label_yga_katmanlar.Name = "label_yga_katmanlar";
            this.label_yga_katmanlar.Size = new System.Drawing.Size(100, 23);
            this.label_yga_katmanlar.TabIndex = 0;
            // 
            // FinishPolygonButton
            // 
            this.FinishPolygonButton.Location = new System.Drawing.Point(0, 0);
            this.FinishPolygonButton.Name = "FinishPolygonButton";
            this.FinishPolygonButton.Size = new System.Drawing.Size(75, 23);
            this.FinishPolygonButton.TabIndex = 0;
            // 
            // YeniGenislemeMapPanel
            // 
            this.YeniGenislemeMapPanel.Location = new System.Drawing.Point(0, 0);
            this.YeniGenislemeMapPanel.Name = "YeniGenislemeMapPanel";
            this.YeniGenislemeMapPanel.Size = new System.Drawing.Size(200, 100);
            this.YeniGenislemeMapPanel.TabIndex = 0;
            // 
            // ButtonKml
            // 
            this.ButtonKml.Location = new System.Drawing.Point(8, 126);
            this.ButtonKml.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonKml.Name = "ButtonKml";
            this.ButtonKml.Size = new System.Drawing.Size(143, 39);
            this.ButtonKml.TabIndex = 30;
            this.ButtonKml.Text = "KML Yükle";
            this.ButtonKml.UseVisualStyleBackColor = true;
            this.ButtonKml.Click += new System.EventHandler(this.ButtonKml_Click);
            // 
            // oznitelikAc
            // 
            this.oznitelikAc.Location = new System.Drawing.Point(17, 542);
            this.oznitelikAc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.oznitelikAc.Name = "oznitelikAc";
            this.oznitelikAc.Size = new System.Drawing.Size(195, 38);
            this.oznitelikAc.TabIndex = 29;
            this.oznitelikAc.Text = "Öznitelikleri Göster";
            this.oznitelikAc.UseVisualStyleBackColor = true;
            // 
            // EA_list_box
            // 
            this.EA_list_box.FormattingEnabled = true;
            this.EA_list_box.ItemHeight = 16;
            this.EA_list_box.Location = new System.Drawing.Point(17, 271);
            this.EA_list_box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EA_list_box.Name = "EA_list_box";
            this.EA_list_box.Size = new System.Drawing.Size(244, 84);
            this.EA_list_box.TabIndex = 20;
            // 
            // ELFRadioButtonsPanel
            // 
            this.ELFRadioButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this.ELFRadioButtonsPanel.Name = "ELFRadioButtonsPanel";
            this.ELFRadioButtonsPanel.Size = new System.Drawing.Size(200, 100);
            this.ELFRadioButtonsPanel.TabIndex = 0;
            // 
            // ELFPredictionButton
            // 
            this.ELFPredictionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ELFPredictionButton.Location = new System.Drawing.Point(95, 243);
            this.ELFPredictionButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFPredictionButton.Name = "ELFPredictionButton";
            this.ELFPredictionButton.Size = new System.Drawing.Size(160, 44);
            this.ELFPredictionButton.TabIndex = 18;
            this.ELFPredictionButton.Text = "Tahmin Yap";
            this.ELFPredictionButton.UseVisualStyleBackColor = true;
            // 
            // SenaryoSelectionButton
            // 
            this.SenaryoSelectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SenaryoSelectionButton.Location = new System.Drawing.Point(36, 553);
            this.SenaryoSelectionButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SenaryoSelectionButton.Name = "SenaryoSelectionButton";
            this.SenaryoSelectionButton.Size = new System.Drawing.Size(159, 44);
            this.SenaryoSelectionButton.TabIndex = 17;
            this.SenaryoSelectionButton.Text = "Senaryo Seç";
            this.SenaryoSelectionButton.UseVisualStyleBackColor = true;
            // 
            // ContextMenuStrip_Poligon
            // 
            this.ContextMenuStrip_Poligon.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.ContextMenuStrip_Poligon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.YGA_Ekle,
            this.Point_Load_Ekle,
            this.Kentsel_Donusum_Ekle,
            this.Enerji_Müsaadesi_Ekle,
            this.Poligon_Kaydet});
            this.ContextMenuStrip_Poligon.Name = "ContextMenuStrip_Poligon";
            this.ContextMenuStrip_Poligon.Size = new System.Drawing.Size(287, 234);
            // 
            // YGA_Ekle
            // 
            this.YGA_Ekle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.YGA_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("YGA_Ekle.Image")));
            this.YGA_Ekle.Name = "YGA_Ekle";
            this.YGA_Ekle.Size = new System.Drawing.Size(286, 46);
            this.YGA_Ekle.Text = "Yeni Genişleme Alanı Çiz";
            this.YGA_Ekle.Click += new System.EventHandler(this.YGA_Ekle_Click);
            // 
            // Point_Load_Ekle
            // 
            this.Point_Load_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("Point_Load_Ekle.Image")));
            this.Point_Load_Ekle.Name = "Point_Load_Ekle";
            this.Point_Load_Ekle.Size = new System.Drawing.Size(286, 46);
            this.Point_Load_Ekle.Text = "Noktasal Yük Ekle";
            this.Point_Load_Ekle.Click += new System.EventHandler(this.Point_Load_Ekle_Click);
            // 
            // Kentsel_Donusum_Ekle
            // 
            this.Kentsel_Donusum_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("Kentsel_Donusum_Ekle.Image")));
            this.Kentsel_Donusum_Ekle.Name = "Kentsel_Donusum_Ekle";
            this.Kentsel_Donusum_Ekle.Size = new System.Drawing.Size(286, 46);
            this.Kentsel_Donusum_Ekle.Text = "Kentsel Dönüşüm Alanı Ekle";
            this.Kentsel_Donusum_Ekle.Click += new System.EventHandler(this.Kentsel_Donusum_Ekle_Click);
            // 
            // Enerji_Müsaadesi_Ekle
            // 
            this.Enerji_Müsaadesi_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("Enerji_Müsaadesi_Ekle.Image")));
            this.Enerji_Müsaadesi_Ekle.Name = "Enerji_Müsaadesi_Ekle";
            this.Enerji_Müsaadesi_Ekle.Size = new System.Drawing.Size(286, 46);
            this.Enerji_Müsaadesi_Ekle.Text = "Enerji Müsaadesi Ekle";
            this.Enerji_Müsaadesi_Ekle.Click += new System.EventHandler(this.Enerji_Müsaadesi_Ekle_Click);
            // 
            // Poligon_Kaydet
            // 
            this.Poligon_Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Kaydet.Image")));
            this.Poligon_Kaydet.Name = "Poligon_Kaydet";
            this.Poligon_Kaydet.Size = new System.Drawing.Size(286, 46);
            this.Poligon_Kaydet.Text = "Poligon Kaydet";
            this.Poligon_Kaydet.Click += new System.EventHandler(this.Poligon_Kaydet_Click);
            // 
            // ContextMenuStrip_Fonksiyon
            // 
            this.ContextMenuStrip_Fonksiyon.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.ContextMenuStrip_Fonksiyon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.katman_birleştir});
            this.ContextMenuStrip_Fonksiyon.Name = "ContextMenuStrip_Fonksiyon";
            this.ContextMenuStrip_Fonksiyon.Size = new System.Drawing.Size(198, 40);
            // 
            // katman_birleştir
            // 
            this.katman_birleştir.Image = ((System.Drawing.Image)(resources.GetObject("katman_birleştir.Image")));
            this.katman_birleştir.Name = "katman_birleştir";
            this.katman_birleştir.Size = new System.Drawing.Size(197, 36);
            this.katman_birleştir.Text = "Katman Birleştir";
            this.katman_birleştir.Click += new System.EventHandler(this.katman_birleştir_Click);
            // 
            // ModuleTabPanel
            // 
            this.ModuleTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModuleTabPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.ModuleTabPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ModuleTabPanel.Controls.Add(this.Modül_Tabları);
            this.ModuleTabPanel.Location = new System.Drawing.Point(0, 38);
            this.ModuleTabPanel.Margin = new System.Windows.Forms.Padding(4);
            this.ModuleTabPanel.Name = "ModuleTabPanel";
            this.ModuleTabPanel.Size = new System.Drawing.Size(1320, 727);
            this.ModuleTabPanel.TabIndex = 5;
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.HeaderPanel.Controls.Add(this.buton_tablo_olustur);
            this.HeaderPanel.Controls.Add(this.buton_database_giris);
            this.HeaderPanel.Controls.Add(this.HomePageButton);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(4);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1322, 38);
            this.HeaderPanel.TabIndex = 6;
            // 
            // buton_tablo_olustur
            // 
            this.buton_tablo_olustur.Location = new System.Drawing.Point(177, 4);
            this.buton_tablo_olustur.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_tablo_olustur.Name = "buton_tablo_olustur";
            this.buton_tablo_olustur.Size = new System.Drawing.Size(153, 28);
            this.buton_tablo_olustur.TabIndex = 7;
            this.buton_tablo_olustur.Text = "Tablo Oluştur";
            this.buton_tablo_olustur.UseVisualStyleBackColor = true;
            this.buton_tablo_olustur.Click += new System.EventHandler(this.buton_tablo_olustur_Click);
            // 
            // buton_database_giris
            // 
            this.buton_database_giris.Location = new System.Drawing.Point(3, 4);
            this.buton_database_giris.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_database_giris.Name = "buton_database_giris";
            this.buton_database_giris.Size = new System.Drawing.Size(153, 28);
            this.buton_database_giris.TabIndex = 6;
            this.buton_database_giris.Text = "Database Girişi";
            this.buton_database_giris.UseVisualStyleBackColor = true;
            this.buton_database_giris.Click += new System.EventHandler(this.buton_database_giris_Click);
            // 
            // HomePageButton
            // 
            this.HomePageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HomePageButton.BackColor = System.Drawing.Color.NavajoWhite;
            this.HomePageButton.BackgroundImage = global::SLF.Properties.Resources.homepage__1_;
            this.HomePageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.HomePageButton.FlatAppearance.BorderSize = 0;
            this.HomePageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HomePageButton.ForeColor = System.Drawing.Color.White;
            this.HomePageButton.Location = new System.Drawing.Point(1268, 4);
            this.HomePageButton.Margin = new System.Windows.Forms.Padding(4);
            this.HomePageButton.Name = "HomePageButton";
            this.HomePageButton.Size = new System.Drawing.Size(45, 34);
            this.HomePageButton.TabIndex = 5;
            this.HomePageButton.UseVisualStyleBackColor = false;
            this.HomePageButton.Click += new System.EventHandler(this.HomePageButton_Click);
            // 
            // buton_proje_sec
            // 
            this.buton_proje_sec.Location = new System.Drawing.Point(0, 0);
            this.buton_proje_sec.Name = "buton_proje_sec";
            this.buton_proje_sec.Size = new System.Drawing.Size(75, 23);
            this.buton_proje_sec.TabIndex = 0;
            // 
            // Point_Load_Çiz
            // 
            this.Point_Load_Çiz.Name = "Point_Load_Çiz";
            this.Point_Load_Çiz.Size = new System.Drawing.Size(244, 26);
            this.Point_Load_Çiz.Text = "Nokta Yük Ekle";
            // 
            // YGA_Çiz
            // 
            this.YGA_Çiz.Name = "YGA_Çiz";
            this.YGA_Çiz.Size = new System.Drawing.Size(244, 26);
            this.YGA_Çiz.Text = "Yeni Genişleme Alanı Çiz";
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AccessibleName = "New item selection";
            this.miniToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDown;
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.BackColor = System.Drawing.Color.White;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.miniToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.miniToolStrip.Location = new System.Drawing.Point(788, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.miniToolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.miniToolStrip.Size = new System.Drawing.Size(1437, 32);
            this.miniToolStrip.TabIndex = 36;
            // 
            // buton_ea_harita_katmanlar
            // 
            this.buton_ea_harita_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_ea_harita_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_ea_harita_katmanlar.BackgroundImage")));
            this.buton_ea_harita_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_ea_harita_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_ea_harita_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_ea_harita_katmanlar.Location = new System.Drawing.Point(373, 654);
            this.buton_ea_harita_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_ea_harita_katmanlar.Name = "buton_ea_harita_katmanlar";
            this.buton_ea_harita_katmanlar.Size = new System.Drawing.Size(61, 52);
            this.buton_ea_harita_katmanlar.TabIndex = 31;
            this.buton_ea_harita_katmanlar.UseVisualStyleBackColor = true;
            // 
            // ELFMinSenaryoGraphPicBox
            // 
            this.ELFMinSenaryoGraphPicBox.Location = new System.Drawing.Point(809, 58);
            this.ELFMinSenaryoGraphPicBox.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMinSenaryoGraphPicBox.Name = "ELFMinSenaryoGraphPicBox";
            this.ELFMinSenaryoGraphPicBox.Size = new System.Drawing.Size(308, 210);
            this.ELFMinSenaryoGraphPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ELFMinSenaryoGraphPicBox.TabIndex = 22;
            this.ELFMinSenaryoGraphPicBox.TabStop = false;
            // 
            // ModülFormu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1322, 773);
            this.Controls.Add(this.ModuleTabPanel);
            this.Controls.Add(this.HeaderPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1333, 807);
            this.Name = "ModülFormu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "                          ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModülFormu_FormClosing);
            this.Load += new System.EventHandler(this.ModülFormu_Load);
            this.Modül_Tabları.ResumeLayout(false);
            this.tab_girdi.ResumeLayout(false);
            this.tab_girdi.PerformLayout();
            this.panel_proje_ekle.ResumeLayout(false);
            this.panel_proje_ekle.PerformLayout();
            this.panel_girdi_rapor_olustur.ResumeLayout(false);
            this.panel_girdi_rapor_olustur.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_girdi)).EndInit();
            this.panel_girdi_dısa_aktar.ResumeLayout(false);
            this.panel_girdi_dısa_aktar.PerformLayout();
            this.panel_girdi_yıl_secimi.ResumeLayout(false);
            this.panel_girdi_dosya_secimi.ResumeLayout(false);
            this.panel_girdi_dosya_secimi.PerformLayout();
            this.tab_dek.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel_DEK.ResumeLayout(false);
            this.panel_DEK.PerformLayout();
            this.tab_ea.ResumeLayout(false);
            this.EAStationsLegendPanel.ResumeLayout(false);
            this.EAStationsLegendPanel.PerformLayout();
            this.panel_ea.ResumeLayout(false);
            this.panel_ea.PerformLayout();
            this.GelecekSimPanel.ResumeLayout(false);
            this.GelecekSimPanel.PerformLayout();
            this.tab_imar.ResumeLayout(false);
            this.tab_imar.PerformLayout();
            this.panel_imar.ResumeLayout(false);
            this.panel_imar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).EndInit();
            this.harita_katmanları_right_click.ResumeLayout(false);
            this.katmanlar_right_click.ResumeLayout(false);
            this.toolStrip_imar.ResumeLayout(false);
            this.toolStrip_imar.PerformLayout();
            this.tab_ekonometrik.ResumeLayout(false);
            this.SenaryoModulePanel.ResumeLayout(false);
            this.EkonometrikSenaryoElementsPanel.ResumeLayout(false);
            this.EkonometrikSenaryoElementsPanel.PerformLayout();
            this.SenaryoModuleTabControl.ResumeLayout(false);
            this.EkonometrikSenaryoTabPage.ResumeLayout(false);
            this.ELFSenaryoTabControls.ResumeLayout(false);
            this.tabPage_min_senaryo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoTable)).EndInit();
            this.tabPage_dusuk_senaryo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowSenaryoTable)).EndInit();
            this.tabPage_baz_senaryo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseSenaryoTable)).EndInit();
            this.tabPage_yuksek_senaryo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighSenaryoTable)).EndInit();
            this.tabPage_maks_senaryo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxSenaryoTable)).EndInit();
            this.EkonometrikSonuclarTabPage.ResumeLayout(false);
            this.ELFSonuçlarTabControls.ResumeLayout(false);
            this.tabPage_min_sonuclar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinimumResultsTable)).EndInit();
            this.tabPage_dusuk_sonuclar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFDüşükResultsTable)).EndInit();
            this.tabPage_baz_sonuclar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFBazResultsTable)).EndInit();
            this.tabPage_yuksek_sonuclar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFYüksekResultsTable)).EndInit();
            this.tabPage_maks_sonuclar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaksimumResultsTable)).EndInit();
            this.EkonometrikGrafiklerTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ekonometrik)).EndInit();
            this.tab_yükHaritası.ResumeLayout(false);
            this.tab_yükHaritası.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buton_HTML)).EndInit();
            this.panel_yuk.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).EndInit();
            this.ContextMenuStrip_Poligon.ResumeLayout(false);
            this.ContextMenuStrip_Fonksiyon.ResumeLayout(false);
            this.ModuleTabPanel.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoGraphPicBox)).EndInit();
            this.ResumeLayout(false);

        }

        public System.Windows.Forms.TabControl Modül_Tabları;
        private System.Windows.Forms.TabPage tab_girdi;
        private System.Windows.Forms.TabPage tab_imar;
        private System.Windows.Forms.TabPage tab_dek;
        private System.Windows.Forms.TabPage tab_optDTR;
        private System.Windows.Forms.TabPage tab_rapor;
        private System.Windows.Forms.TabPage tab_yükHaritası;
        private System.Windows.Forms.Label label_girdi_veri_tipi_secimi;
        private System.Windows.Forms.ComboBox veri_listesi_seçimi;
        private System.Windows.Forms.DataGridView dataGridView_girdi;
        private System.Windows.Forms.Label label_girdi_dosya_secimi;
        private System.Windows.Forms.Button SelectFolderButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_yuk;
        private System.Windows.Forms.ContextMenuStrip katmanlar_right_click;
        private System.Windows.Forms.ToolStripMenuItem tabloyuGörToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rengiDeğiştirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temizleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kaydetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip harita_katmanları_right_click;
        private System.Windows.Forms.ToolStripMenuItem Arazi;
        private System.Windows.Forms.ToolStripMenuItem Harita;
        private System.Windows.Forms.ToolStripMenuItem Uydu;
        private System.Windows.Forms.ToolStripMenuItem Google_Earth;
        private System.Windows.Forms.ToolStripMenuItem OSM;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Poligon;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Kaydet;
        private ContextMenuStrip ContextMenuStrip_Fonksiyon;
        private ToolStripMenuItem katman_birleştir;
        private ToolStripMenuItem Google_Earth_Desktop;
        private Button raporGoruntuleButonu;
        private Button ExcelDownloadButton;
        private ToolStripMenuItem yenidenAdlandırToolStripMenuItem;
        private Button csvExportButton;
        private ComboBox endYearComboBox;
        private ComboBox startYearComboBox;
        private Button yearApproveButton;
        private Panel ModuleTabPanel;
        private Panel panel_girdi_yıl_secimi;
        private Panel panel_girdi_rapor_olustur;
        private Panel panel_girdi_dısa_aktar;
        private Panel panel_girdi_dosya_secimi;
        public TabPage tab_ekonometrik;
        private Button SenaryoSelectionButton;
        private Label label_girdi_rapor;
        private Label label_dısa_aktar;
        private Label label_girdi_veri_onizleme;
        private Guna.UI2.WinForms.Guna2TabControl SenaryoModuleTabControl;
        private TabPage EkonometrikSenaryoTabPage;
        private Panel SenaryoModulePanel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel EkonometrikSenaryoElementsPanel;
        private Button ELFPredictionButton;
        private PictureBox ELFMinSenaryoGraphPicBox;
        private Panel ELFRadioButtonsPanel;
        private ToolStripMenuItem Sokak_Görünümü;
        private Button buton_yuk_haritası_katmanlar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_yuk;
        private ToolStrip toolStrip_imar;
        private ToolStripButton İmar_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton İmar_Poligon;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripButton İmar_Fonksiyonlar;
        private Button buton_ea_harita_katmanlar;
        private ToolStrip Toolbox_EA;
        private Button ButtonKml;
        private Button oznitelikAc;
        private ListBox EA_list_box;
        public GMap.NET.WindowsForms.GMapControl gMapControl_EA;
        private CheckBox checkBox_AC_Home;
        private CheckBox checkBox_AC_Work;
        private CheckBox checkBox_AC_Public;
        private CheckBox checkBox_DC_Fast;
        private Button imar_dosya_seçimi;
        private Label yuk_yıl_deger;
        private Label yuk_yıl_text;
        public Panel legendPanel;
        public System.Windows.Forms.TabPage tab_ea;
        private System.Windows.Forms.Label label_imar_katmanlar;
        private System.Windows.Forms.CheckBox checkBox_imar_5;
        private System.Windows.Forms.CheckBox checkBox_imar_8;
        private System.Windows.Forms.CheckBox checkBox_imar_9;
        private System.Windows.Forms.CheckBox checkBox_imar_10;
        private System.Windows.Forms.CheckBox checkBox_imar_11;
        private System.Windows.Forms.CheckBox checkBox_imar_12;
        private System.Windows.Forms.CheckBox checkBox_imar_13;
        private System.Windows.Forms.CheckBox checkBox_imar_15;
        private System.Windows.Forms.CheckBox checkBox_imar_14;
        private System.Windows.Forms.CheckBox checkBox_imar_4;
        private System.Windows.Forms.CheckBox checkBox_imar_3;
        private System.Windows.Forms.CheckBox checkBox_imar_2;
        private System.Windows.Forms.CheckBox checkBox_imar_1;
        private System.Windows.Forms.CheckBox checkBox_imar_7;
        private System.Windows.Forms.CheckBox checkBox_imar_6;
        private System.Windows.Forms.Label mesafe_metre_DeK;
        private System.Windows.Forms.Label Mesafe_Dek;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.ToolStripMenuItem Point_Load_Çiz;
        private Label FutureSimLabel;
        private ComboBox comboBox_ea_yıl_secimi;
        private Panel GelecekSimPanel;
        private Panel panel_ea;
        private Label AddStationLabel;
        private Panel panel_DEK;
        private ComboBox comboBox_DEK_Yıl;
        private Label label_DEK_Gelecek;
        private CheckBox checkBox27;
        private ImageList imageList;
        public GMap.NET.WindowsForms.GMapControl gMapControl_DEK;
        private Panel HeaderPanel;
        public System.Windows.Forms.Button OpenModuleButton;
        private TabPage tab_yga;
        private Button ELFScenerioSaveButton;
        private Button ELFTahminButonu;
        private Button EAStationAddButton;
        private Button GelecekSimButton;
        private Panel EAStationsLegendPanel;
        private Label ACHomeLegendValueLabel;
        private Label ACHomeLegendLabel;
        private Label ACWorkLegendValueLabel;
        private Label ACWorkLegendLabel;
        private Label DCFastLegendValueLabel;
        private Label DCFastLegendLabel;
        private Label ACPublicLegendValueLabel;
        private Label ACPublicLegendLabel;
        private Button EASimButton;
        private RadioButton EaSimMaxBtn;
        private RadioButton EaSimDefBtn;
        private RadioButton EaSimMinBtn;
        private Button HomePageButton;
        private Panel YeniGenislemeSidePanel;
        private Panel YeniGenislemeMapPanel;
        private Button FinishPolygonButton;
        private Label label_yga_katmanlar;
        private Panel panel_proje_ekle;
        private Label label_proje_ekle;
        private Button ProjeEkleButton;
        private Button DeepLearningModelButton;


        private ToolStrip miniToolStrip;
        private Button buton_proje_sec;
        private CheckBox checkBox_yuk_12;
        private CheckBox checkBox_yuk_10;
        private CheckBox checkBox_yuk_9;
        private CheckBox checkBox_yuk_7;
        private CheckBox checkBox_yuk_4;
        private CheckBox checkBox_yuk_5;
        private CheckBox checkBox_yuk_3;
        private CheckBox checkBox_yuk_2;
        private CheckBox checkBox_yuk_1;
        private CheckBox checkBox_yuk_8;
        private CheckBox checkBox_yuk_6;
        private CheckBox checkBox_yuk_11;
        private CheckBox checkBox_yuk_14;
        private CheckBox checkBox_yuk_13;
        private CheckBox checkBox_yuk_15;

        private ToolStripMenuItem YGA_Çiz;
        private Button buton_database_giris;
        private Button buton_tablo_olustur;
        private ToolStripMenuItem YGA_Ekle;
        private ToolStripMenuItem Point_Load_Ekle;
        private Button buton_DL_calıstır;
        private Panel panel_imar;
        private Label Mesafe_imar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_imar;
        private Button buton_imar_katmanlar;
        private Label mesafe_metre_imar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_imar;
        private Panel panel_yuk;
        private TrackBar trackBar_Yıllar;
        private CheckBox checkBox_yuk_main;
        private Panel panel1;
        private CheckBox DEKPointsLayerCheckBox;
        private ProgressBar DEKProgressBar;
        private Label DEKStatusLabel;
        private System.Windows.Forms.Button DEKRunSimulationButton;
        private System.Windows.Forms.Button DEKSimulasyonSonucGoruntule;

        private System.Windows.Forms.Button EANewSimulationResultsButton;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Label RModelStatusLabel;
        private ProgressBar RModelProgressBar;
        private System.Windows.Forms.Button SimulasyonSonucGoruntule;
        private CheckBox EAPointsLayerCheckBox;
        private PictureBox buton_HTML;
        private TabPage EkonometrikSonuclarTabPage;
        private TabControl ELFSonuçlarTabControls;
        private TabPage tabPage_min_sonuclar;
        private DataGridView ELFMinimumResultsTable;
        private TabPage tabPage_dusuk_sonuclar;
        private DataGridView ELFDüşükResultsTable;
        private TabPage tabPage_baz_sonuclar;
        private DataGridView ELFBazResultsTable;
        private TabPage tabPage_yuksek_sonuclar;
        private DataGridView ELFYüksekResultsTable;
        private TabPage tabPage_maks_sonuclar;
        private DataGridView ELFMaksimumResultsTable;
        private TabPage EkonometrikGrafiklerTabPage;
        private PictureBox pictureBox_ekonometrik;
        private ComboBox comboBox_ekonometrik;
        private Label label_graphics;
        private TabControl ELFSenaryoTabControls;
        private TabPage tabPage_min_senaryo;
        private DataGridView ELFMinSenaryoTable;
        private TabPage tabPage_dusuk_senaryo;
        private DataGridView ELFLowSenaryoTable;
        private TabPage tabPage_baz_senaryo;
        private DataGridView ELFBaseSenaryoTable;
        private TabPage tabPage_yuksek_senaryo;
        private DataGridView ELFHighSenaryoTable;
        private TabPage tabPage_maks_senaryo;
        private DataGridView ELFMaxSenaryoTable;
        private Label label_s_ELF;
        private TextBox textBox_sonuc_ELF;
        private Panel panel2;
        private System.Windows.Forms.Button CreateReportButton;
        private Button CreateReportButton2;
        private ToolStripMenuItem Enerji_Müsaadesi_Ekle;
        private Button buton_SLF_tahmini;
        private Button buton_abone_sayısı_tahmini;
        private Button buton_imar_tahmini;
        private Button buton_ELF_tablo_sec;
        private ToolStripMenuItem Kentsel_Donusum_Ekle;
        private Button DEKCenterAddButton;
        private RadioButton dekSimMaxBtn;
        private RadioButton dekSimDefBtn;
        private RadioButton dekSimMinBtn;
        private Button DEKSimButton;
    }
}