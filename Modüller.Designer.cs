using System;
using System.Windows.Forms;
using SLF;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModülFormu));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Modül_Tabları = new System.Windows.Forms.TabControl();
            this.tab_girdi = new System.Windows.Forms.TabPage();
            this.OpenModuleButton = new SLF.CustomButton();
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
            this.DEKPointsLayerCheckBox = new System.Windows.Forms.CheckBox();
            this.DEKProgressBar = new System.Windows.Forms.ProgressBar();
            this.DEKStatusLabel = new System.Windows.Forms.Label();
            this.gMapControl_DEK = new GMap.NET.WindowsForms.GMapControl();
            this.panel_DEK = new System.Windows.Forms.Panel();
            this.DEKCenterAddButton = new SLF.CustomButton();
            this.comboBox_DEK_il = new System.Windows.Forms.ComboBox();
            this.DEKSimulasyonSonucGoruntule = new SLF.CustomButton();
            this.DEKRunSimulationButton = new SLF.CustomButton();
            this.comboBox_dek_ilce_secimi = new System.Windows.Forms.ComboBox();
            this.dekSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.dekSimDefBtn = new System.Windows.Forms.RadioButton();
            this.dekSimMinBtn = new System.Windows.Forms.RadioButton();
            this.DEKSimButton = new SLF.CustomButton();
            this.label_DEK_Gelecek = new System.Windows.Forms.Label();
            this.comboBox_DEK_Yıl = new System.Windows.Forms.ComboBox();
            this.tab_ea = new System.Windows.Forms.TabPage();
            this.EAStationsLegendPanel = new System.Windows.Forms.Panel();
            this.SimulasyonSonucGoruntule = new SLF.CustomButton();
            this.EANewSimulationResultsButton = new SLF.CustomButton();
            this.comboBox_ea_ilce_secimi = new System.Windows.Forms.ComboBox();
            this.EASimButton = new SLF.CustomButton();
            this.EaSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.FutureSimLabel = new System.Windows.Forms.Label();
            this.comboBox_ea_il_secimi = new System.Windows.Forms.ComboBox();
            this.EaSimDefBtn = new System.Windows.Forms.RadioButton();
            this.comboBox_ea_yıl_secimi = new System.Windows.Forms.ComboBox();
            this.EaSimMinBtn = new System.Windows.Forms.RadioButton();
            this.panel_ea = new System.Windows.Forms.Panel();
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
            this.EAStationAddButton = new SLF.CustomButton();
            this.ACHomeLegendLabel = new System.Windows.Forms.Label();
            this.ACWorkLegendLabel = new System.Windows.Forms.Label();
            this.ACHomeLegendValueLabel = new System.Windows.Forms.Label();
            this.gMapControl_EA = new GMap.NET.WindowsForms.GMapControl();
            this.tab_ekonometrik = new System.Windows.Forms.TabPage();
            this.ELFResultsTabControls = new System.Windows.Forms.TabControl();
            this.ELFMinResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFMinResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFLowResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFLowResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFBaseResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFBaseResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFHighResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFHighResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFMaxResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFMaxResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFGraphicOutputsTabPage = new System.Windows.Forms.TabPage();
            this.panel_ELF_Grafikler = new System.Windows.Forms.Panel();
            this.pictureBox_ELF_5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_3 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_1 = new System.Windows.Forms.PictureBox();
            this.ELFTablePanel = new System.Windows.Forms.Panel();
            this.ELFGraphicsPanel = new System.Windows.Forms.Panel();
            this.ELFShowGraphsButton = new SLF.CustomButton();
            this.SenaryoNewSelectionButton = new SLF.CustomButton();
            this.SenaryoResultsLabel = new System.Windows.Forms.Label();
            this.tab_imar = new System.Windows.Forms.TabPage();
            this.checkBox_imar_15 = new System.Windows.Forms.CheckBox();
            this.katmanlar_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabloyuGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rengiDeğiştirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temizleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yenidenAdlandırToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox_imar_14 = new System.Windows.Forms.CheckBox();
            this.panel_imar = new System.Windows.Forms.Panel();
            this.buton_imar_katmanlar = new System.Windows.Forms.Button();
            this.harita_katmanları_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Arazi = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth_Desktop = new System.Windows.Forms.ToolStripMenuItem();
            this.Harita = new System.Windows.Forms.ToolStripMenuItem();
            this.OSM = new System.Windows.Forms.ToolStripMenuItem();
            this.Sokak_Görünümü = new System.Windows.Forms.ToolStripMenuItem();
            this.Uydu = new System.Windows.Forms.ToolStripMenuItem();
            this.webView_imar = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gMapControl_imar = new GMap.NET.WindowsForms.GMapControl();
            this.mesafe_metre_imar = new System.Windows.Forms.Label();
            this.Mesafe_imar = new System.Windows.Forms.Label();
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
            this.İmar_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Grid_Oluştur = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.tab_optDTR = new System.Windows.Forms.TabPage();
            this.gMapControl_optimalDTR = new GMap.NET.WindowsForms.GMapControl();
            this.webView_optimalDTR = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.checkBox_optDTR_Eskişehir = new System.Windows.Forms.CheckBox();
            this.checkBox_optDTR_İzmir = new System.Windows.Forms.CheckBox();
            this.tab_senaryo = new System.Windows.Forms.TabPage();
            this.SenaryoModulePanel = new System.Windows.Forms.Panel();
            this.EkonometrikSenaryoElementsPanel = new System.Windows.Forms.Panel();
            this.RModelProgressBar = new System.Windows.Forms.ProgressBar();
            this.ShowResultsButton = new SLF.CustomButton();
            this.RModelStatusLabel = new System.Windows.Forms.Label();
            this.ELFPredictionShowResultsButton = new SLF.CustomButton();
            this.ELFScenerioSaveButton = new SLF.CustomButton();
            this.richTextBox_senaryolar_ELF = new System.Windows.Forms.RichTextBox();
            this.SenaryoModuleTabControl = new Guna.UI2.WinForms.Guna2TabControl();
            this.EkonometrikSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.EkonometrikSenaryoOutputsPanel = new System.Windows.Forms.Panel();
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
            this.YeniGenislemeSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.tab_stokastik = new System.Windows.Forms.TabPage();
            this.checkBox_stokastik_15 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_14 = new System.Windows.Forms.CheckBox();
            this.panel_stokastik = new System.Windows.Forms.Panel();
            this.webView_stokastik = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gMapControl_stokastik = new GMap.NET.WindowsForms.GMapControl();
            this.buton_stokastik_harita_katmanlar = new System.Windows.Forms.Button();
            this.checkBox_stokastik_13 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_12 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_11 = new System.Windows.Forms.CheckBox();
            this.mesafe_metre_stokastik = new System.Windows.Forms.Label();
            this.Mesafe_stokastik = new System.Windows.Forms.Label();
            this.checkBox_stokastik_10 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_9 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_8 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_7 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_6 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_5 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_4 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_3 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_2 = new System.Windows.Forms.CheckBox();
            this.checkBox_stokastik_1 = new System.Windows.Forms.CheckBox();
            this.label_stokastik_katmanlar = new System.Windows.Forms.Label();
            this.stokastik_dosya_seçimi = new System.Windows.Forms.Button();
            this.toolStrip_stokastik = new System.Windows.Forms.ToolStrip();
            this.Stokastik_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Grid_Oluştur = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.tab_yükHaritası = new System.Windows.Forms.TabPage();
            this.legendPanel = new System.Windows.Forms.Panel();
            this.yuk_yıl_deger = new System.Windows.Forms.Label();
            this.yuk_yıl_text = new System.Windows.Forms.Label();
            this.trackBar_Yıllar = new System.Windows.Forms.TrackBar();
            this.Mesafe_yuk = new System.Windows.Forms.Label();
            this.mesafe_metre_yuk = new System.Windows.Forms.Label();
            this.toolStrip_yuk = new System.Windows.Forms.ToolStrip();
            this.Yuk_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Yuk_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.Yuk_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.gMapControl_yuk = new GMap.NET.WindowsForms.GMapControl();
            this.buton_yuk_haritası_katmanlar = new System.Windows.Forms.Button();
            this.tab_rapor = new System.Windows.Forms.TabPage();
            this.tab_validasyon = new System.Windows.Forms.TabPage();
            this.tab_yga = new System.Windows.Forms.TabPage();
            this.buton_dosya_yga = new System.Windows.Forms.Button();
            this.checkBox_yga_15 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_14 = new System.Windows.Forms.CheckBox();
            this.panel_yga = new System.Windows.Forms.Panel();
            this.webView_yga = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gMapControl_yga = new GMap.NET.WindowsForms.GMapControl();
            this.buton_yga_harita_katmanlar = new System.Windows.Forms.Button();
            this.Mesafe_yga = new System.Windows.Forms.Label();
            this.mesafe_metre_yga = new System.Windows.Forms.Label();
            this.label_yga_katmanlar = new System.Windows.Forms.Label();
            this.FinishPolygonButton = new System.Windows.Forms.Button();
            this.checkBox_yga_13 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_12 = new System.Windows.Forms.CheckBox();
            this.toolStrip_yga = new System.Windows.Forms.ToolStrip();
            this.toolStrip_yga_seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_yga_kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_yga_mesafe = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_yga_poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_yga_nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_yga_fonksiyon = new System.Windows.Forms.ToolStripButton();
            this.checkBox_yga_11 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_10 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_9 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_8 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_5 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_7 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_1 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_6 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_2 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_3 = new System.Windows.Forms.CheckBox();
            this.checkBox_yga_4 = new System.Windows.Forms.CheckBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.Toolbox_EA = new System.Windows.Forms.ToolStrip();
            this.EA_Seç = new System.Windows.Forms.ToolStripButton();
            this.EA_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.EA_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.EA_Poligon = new System.Windows.Forms.ToolStripButton();
            this.EA_Nokta = new System.Windows.Forms.ToolStripButton();
            this.ButtonKml = new System.Windows.Forms.Button();
            this.oznitelikAc = new System.Windows.Forms.Button();
            this.EA_list_box = new System.Windows.Forms.ListBox();
            this.ELFRadioButtonsPanel = new System.Windows.Forms.Panel();
            this.ELFPredictionButton = new System.Windows.Forms.Button();
            this.SenaryoSelectionButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip_Nokta = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Nokta_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Nokta_Sil = new System.Windows.Forms.ToolStripMenuItem();
            this.Kaydet = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip_Poligon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Poligon_Çiz = new System.Windows.Forms.ToolStripMenuItem();
            this.Poligon_Sil = new System.Windows.Forms.ToolStripMenuItem();
            this.Poligon_Kaydet = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip_Fonksiyon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.katman_birleştir = new System.Windows.Forms.ToolStripMenuItem();
            this.overlap_analizi = new System.Windows.Forms.ToolStripMenuItem();
            this.ModuleTabPanel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.gMapControl_optimal_dtr = new GMap.NET.WindowsForms.GMapControl();
            this.Seç_Stokastik = new System.Windows.Forms.ToolStrip();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.HomePageButton = new SLF.CustomButton();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.buton_ea_harita_katmanlar = new System.Windows.Forms.Button();
            this.ELFMinSenaryoGraphPicBox = new System.Windows.Forms.PictureBox();
            this.Modül_Tabları.SuspendLayout();
            this.tab_girdi.SuspendLayout();
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
            this.tab_ekonometrik.SuspendLayout();
            this.ELFResultsTabControls.SuspendLayout();
            this.ELFMinResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinResultsTable)).BeginInit();
            this.ELFLowResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowResultsTable)).BeginInit();
            this.ELFBaseResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseResultsTable)).BeginInit();
            this.ELFHighResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighResultsTable)).BeginInit();
            this.ELFMaxResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxResultsTable)).BeginInit();
            this.ELFGraphicOutputsTabPage.SuspendLayout();
            this.panel_ELF_Grafikler.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_1)).BeginInit();
            this.ELFGraphicsPanel.SuspendLayout();
            this.tab_imar.SuspendLayout();
            this.katmanlar_right_click.SuspendLayout();
            this.panel_imar.SuspendLayout();
            this.harita_katmanları_right_click.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).BeginInit();
            this.toolStrip_imar.SuspendLayout();
            this.tab_optDTR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_optimalDTR)).BeginInit();
            this.tab_senaryo.SuspendLayout();
            this.SenaryoModulePanel.SuspendLayout();
            this.EkonometrikSenaryoElementsPanel.SuspendLayout();
            this.SenaryoModuleTabControl.SuspendLayout();
            this.EkonometrikSenaryoTabPage.SuspendLayout();
            this.EkonometrikSenaryoOutputsPanel.SuspendLayout();
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
            this.tab_stokastik.SuspendLayout();
            this.panel_stokastik.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_stokastik)).BeginInit();
            this.toolStrip_stokastik.SuspendLayout();
            this.tab_yükHaritası.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).BeginInit();
            this.toolStrip_yuk.SuspendLayout();
            this.tab_yga.SuspendLayout();
            this.panel_yga.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yga)).BeginInit();
            this.toolStrip_yga.SuspendLayout();
            this.Toolbox_EA.SuspendLayout();
            this.ContextMenuStrip_Nokta.SuspendLayout();
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
            this.Modül_Tabları.Controls.Add(this.tab_ekonometrik);
            this.Modül_Tabları.Controls.Add(this.tab_imar);
            this.Modül_Tabları.Controls.Add(this.tab_optDTR);
            this.Modül_Tabları.Controls.Add(this.tab_senaryo);
            this.Modül_Tabları.Controls.Add(this.tab_stokastik);
            this.Modül_Tabları.Controls.Add(this.tab_yükHaritası);
            this.Modül_Tabları.Controls.Add(this.tab_rapor);
            this.Modül_Tabları.Controls.Add(this.tab_validasyon);
            this.Modül_Tabları.Controls.Add(this.tab_yga);
            this.Modül_Tabları.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Modül_Tabları.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Modül_Tabları.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Modül_Tabları.HotTrack = true;
            this.Modül_Tabları.ImageList = this.imageList;
            this.Modül_Tabları.Location = new System.Drawing.Point(0, 0);
            this.Modül_Tabları.Margin = new System.Windows.Forms.Padding(2);
            this.Modül_Tabları.Multiline = true;
            this.Modül_Tabları.Name = "Modül_Tabları";
            this.Modül_Tabları.Padding = new System.Drawing.Point(20, 3);
            this.Modül_Tabları.SelectedIndex = 0;
            this.Modül_Tabları.Size = new System.Drawing.Size(1368, 598);
            this.Modül_Tabları.TabIndex = 2;
            this.Modül_Tabları.SelectedIndexChanged += new System.EventHandler(this.Modül_Tabları_SelectedIndexChanged);
            // 
            // tab_girdi
            // 
            this.tab_girdi.AutoScroll = true;
            this.tab_girdi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tab_girdi.Controls.Add(this.OpenModuleButton);
            this.tab_girdi.Controls.Add(this.label_girdi_veri_onizleme);
            this.tab_girdi.Controls.Add(this.panel_girdi_rapor_olustur);
            this.tab_girdi.Controls.Add(this.dataGridView_girdi);
            this.tab_girdi.Controls.Add(this.panel_girdi_dısa_aktar);
            this.tab_girdi.Controls.Add(this.panel_girdi_yıl_secimi);
            this.tab_girdi.Controls.Add(this.panel_girdi_dosya_secimi);
            this.tab_girdi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_girdi.ImageIndex = 6;
            this.tab_girdi.Location = new System.Drawing.Point(4, 48);
            this.tab_girdi.Margin = new System.Windows.Forms.Padding(2);
            this.tab_girdi.Name = "tab_girdi";
            this.tab_girdi.Padding = new System.Windows.Forms.Padding(2);
            this.tab_girdi.Size = new System.Drawing.Size(1360, 546);
            this.tab_girdi.TabIndex = 0;
            this.tab_girdi.Text = "Girdi Modülü";
            this.tab_girdi.UseVisualStyleBackColor = true;
            // 
            // OpenModuleButton
            // 
            this.OpenModuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenModuleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OpenModuleButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.OpenModuleButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.OpenModuleButton.BorderRadius = 0;
            this.OpenModuleButton.BorderSize = 0;
            this.OpenModuleButton.FlatAppearance.BorderSize = 0;
            this.OpenModuleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenModuleButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.OpenModuleButton.ForeColor = System.Drawing.Color.White;
            this.OpenModuleButton.Location = new System.Drawing.Point(1191, 498);
            this.OpenModuleButton.Name = "OpenModuleButton";
            this.OpenModuleButton.Size = new System.Drawing.Size(150, 40);
            this.OpenModuleButton.TabIndex = 19;
            this.OpenModuleButton.Text = "Modüle Git";
            this.OpenModuleButton.TextColor = System.Drawing.Color.White;
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
            this.label_girdi_veri_onizleme.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_girdi_veri_onizleme.Location = new System.Drawing.Point(2, 63);
            this.label_girdi_veri_onizleme.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_girdi_veri_onizleme.Name = "label_girdi_veri_onizleme";
            this.label_girdi_veri_onizleme.Size = new System.Drawing.Size(91, 16);
            this.label_girdi_veri_onizleme.TabIndex = 5;
            this.label_girdi_veri_onizleme.Text = "Veri Önizleme:";
            // 
            // panel_girdi_rapor_olustur
            // 
            this.panel_girdi_rapor_olustur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_girdi_rapor_olustur.Controls.Add(this.label_girdi_rapor);
            this.panel_girdi_rapor_olustur.Controls.Add(this.raporGoruntuleButonu);
            this.panel_girdi_rapor_olustur.Location = new System.Drawing.Point(1191, 7);
            this.panel_girdi_rapor_olustur.Margin = new System.Windows.Forms.Padding(2);
            this.panel_girdi_rapor_olustur.Name = "panel_girdi_rapor_olustur";
            this.panel_girdi_rapor_olustur.Size = new System.Drawing.Size(144, 53);
            this.panel_girdi_rapor_olustur.TabIndex = 15;
            // 
            // label_girdi_rapor
            // 
            this.label_girdi_rapor.AutoSize = true;
            this.label_girdi_rapor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_rapor.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_girdi_rapor.Location = new System.Drawing.Point(2, 4);
            this.label_girdi_rapor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_girdi_rapor.Name = "label_girdi_rapor";
            this.label_girdi_rapor.Size = new System.Drawing.Size(93, 17);
            this.label_girdi_rapor.TabIndex = 7;
            this.label_girdi_rapor.Text = "Rapor Oluştur:";
            // 
            // raporGoruntuleButonu
            // 
            this.raporGoruntuleButonu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.raporGoruntuleButonu.BackColor = System.Drawing.Color.White;
            this.raporGoruntuleButonu.BackgroundImage = global::SLF.Properties.Resources.Health_Graph;
            this.raporGoruntuleButonu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.raporGoruntuleButonu.ForeColor = System.Drawing.Color.Transparent;
            this.raporGoruntuleButonu.Location = new System.Drawing.Point(98, 11);
            this.raporGoruntuleButonu.Margin = new System.Windows.Forms.Padding(2);
            this.raporGoruntuleButonu.Name = "raporGoruntuleButonu";
            this.raporGoruntuleButonu.Size = new System.Drawing.Size(33, 34);
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
            this.dataGridView_girdi.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_girdi.BackgroundColor = System.Drawing.Color.Snow;
            this.dataGridView_girdi.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_girdi.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_girdi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_girdi.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_girdi.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dataGridView_girdi.Location = new System.Drawing.Point(2, 92);
            this.dataGridView_girdi.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView_girdi.Name = "dataGridView_girdi";
            this.dataGridView_girdi.ReadOnly = true;
            this.dataGridView_girdi.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_girdi.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_girdi.RowHeadersWidth = 18;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.dataGridView_girdi.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_girdi.RowTemplate.Height = 24;
            this.dataGridView_girdi.Size = new System.Drawing.Size(1356, 400);
            this.dataGridView_girdi.TabIndex = 4;
            // 
            // panel_girdi_dısa_aktar
            // 
            this.panel_girdi_dısa_aktar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_girdi_dısa_aktar.Controls.Add(this.label_dısa_aktar);
            this.panel_girdi_dısa_aktar.Controls.Add(this.ExcelDownloadButton);
            this.panel_girdi_dısa_aktar.Controls.Add(this.csvExportButton);
            this.panel_girdi_dısa_aktar.Location = new System.Drawing.Point(1031, 7);
            this.panel_girdi_dısa_aktar.Margin = new System.Windows.Forms.Padding(2);
            this.panel_girdi_dısa_aktar.Name = "panel_girdi_dısa_aktar";
            this.panel_girdi_dısa_aktar.Size = new System.Drawing.Size(158, 53);
            this.panel_girdi_dısa_aktar.TabIndex = 14;
            // 
            // label_dısa_aktar
            // 
            this.label_dısa_aktar.AutoSize = true;
            this.label_dısa_aktar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_dısa_aktar.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_dısa_aktar.Location = new System.Drawing.Point(2, 2);
            this.label_dısa_aktar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_dısa_aktar.Name = "label_dısa_aktar";
            this.label_dısa_aktar.Size = new System.Drawing.Size(70, 17);
            this.label_dısa_aktar.TabIndex = 16;
            this.label_dısa_aktar.Text = "Dışa Aktar:";
            // 
            // ExcelDownloadButton
            // 
            this.ExcelDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExcelDownloadButton.BackColor = System.Drawing.Color.White;
            this.ExcelDownloadButton.BackgroundImage = global::SLF.Properties.Resources.Spreadsheet_File;
            this.ExcelDownloadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExcelDownloadButton.ForeColor = System.Drawing.Color.Transparent;
            this.ExcelDownloadButton.Location = new System.Drawing.Point(79, 11);
            this.ExcelDownloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.ExcelDownloadButton.Name = "ExcelDownloadButton";
            this.ExcelDownloadButton.Size = new System.Drawing.Size(32, 34);
            this.ExcelDownloadButton.TabIndex = 7;
            this.ExcelDownloadButton.UseVisualStyleBackColor = false;
            this.ExcelDownloadButton.Click += new System.EventHandler(this.ExcelDownloadButton_Click);
            // 
            // csvExportButton
            // 
            this.csvExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.csvExportButton.BackColor = System.Drawing.Color.White;
            this.csvExportButton.BackgroundImage = global::SLF.Properties.Resources.CSV21;
            this.csvExportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.csvExportButton.ForeColor = System.Drawing.Color.Transparent;
            this.csvExportButton.Location = new System.Drawing.Point(115, 11);
            this.csvExportButton.Margin = new System.Windows.Forms.Padding(2);
            this.csvExportButton.Name = "csvExportButton";
            this.csvExportButton.Size = new System.Drawing.Size(31, 34);
            this.csvExportButton.TabIndex = 8;
            this.csvExportButton.UseVisualStyleBackColor = true;
            this.csvExportButton.Click += new System.EventHandler(this.csvExportButton_Click);
            // 
            // panel_girdi_yıl_secimi
            // 
            this.panel_girdi_yıl_secimi.Controls.Add(this.startYearComboBox);
            this.panel_girdi_yıl_secimi.Controls.Add(this.yearApproveButton);
            this.panel_girdi_yıl_secimi.Controls.Add(this.endYearComboBox);
            this.panel_girdi_yıl_secimi.Location = new System.Drawing.Point(4, 2);
            this.panel_girdi_yıl_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.panel_girdi_yıl_secimi.Name = "panel_girdi_yıl_secimi";
            this.panel_girdi_yıl_secimi.Size = new System.Drawing.Size(209, 58);
            this.panel_girdi_yıl_secimi.TabIndex = 12;
            // 
            // startYearComboBox
            // 
            this.startYearComboBox.ForeColor = System.Drawing.Color.DarkBlue;
            this.startYearComboBox.FormattingEnabled = true;
            this.startYearComboBox.Location = new System.Drawing.Point(3, 3);
            this.startYearComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.startYearComboBox.Name = "startYearComboBox";
            this.startYearComboBox.Size = new System.Drawing.Size(77, 25);
            this.startYearComboBox.TabIndex = 9;
            this.startYearComboBox.Text = "Yıl seçiniz";
            this.startYearComboBox.SelectedIndexChanged += new System.EventHandler(this.startYearComboBox_SelectedIndexChanged);
            // 
            // yearApproveButton
            // 
            this.yearApproveButton.BackColor = System.Drawing.Color.Transparent;
            this.yearApproveButton.FlatAppearance.BorderSize = 0;
            this.yearApproveButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.yearApproveButton.ForeColor = System.Drawing.Color.DarkBlue;
            this.yearApproveButton.Location = new System.Drawing.Point(64, 30);
            this.yearApproveButton.Margin = new System.Windows.Forms.Padding(2);
            this.yearApproveButton.Name = "yearApproveButton";
            this.yearApproveButton.Size = new System.Drawing.Size(75, 26);
            this.yearApproveButton.TabIndex = 11;
            this.yearApproveButton.Text = "ONAYLA";
            this.yearApproveButton.UseVisualStyleBackColor = false;
            this.yearApproveButton.Click += new System.EventHandler(this.yearApproveButton_Click);
            // 
            // endYearComboBox
            // 
            this.endYearComboBox.ForeColor = System.Drawing.Color.DarkBlue;
            this.endYearComboBox.FormattingEnabled = true;
            this.endYearComboBox.Location = new System.Drawing.Point(126, 2);
            this.endYearComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.endYearComboBox.Name = "endYearComboBox";
            this.endYearComboBox.Size = new System.Drawing.Size(77, 25);
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
            this.panel_girdi_dosya_secimi.Location = new System.Drawing.Point(473, 4);
            this.panel_girdi_dosya_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.panel_girdi_dosya_secimi.Name = "panel_girdi_dosya_secimi";
            this.panel_girdi_dosya_secimi.Size = new System.Drawing.Size(334, 56);
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
            this.veri_listesi_seçimi.Location = new System.Drawing.Point(39, 22);
            this.veri_listesi_seçimi.Margin = new System.Windows.Forms.Padding(2);
            this.veri_listesi_seçimi.Name = "veri_listesi_seçimi";
            this.veri_listesi_seçimi.Size = new System.Drawing.Size(161, 26);
            this.veri_listesi_seçimi.TabIndex = 0;
            this.veri_listesi_seçimi.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.veri_listesi_seçimi_DrawItem);
            this.veri_listesi_seçimi.SelectedIndexChanged += new System.EventHandler(this.veri_listesi_seçimi_SelectedIndexChanged);
            // 
            // label_girdi_veri_tipi_secimi
            // 
            this.label_girdi_veri_tipi_secimi.AutoSize = true;
            this.label_girdi_veri_tipi_secimi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_veri_tipi_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_girdi_veri_tipi_secimi.Location = new System.Drawing.Point(2, 1);
            this.label_girdi_veri_tipi_secimi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_girdi_veri_tipi_secimi.Name = "label_girdi_veri_tipi_secimi";
            this.label_girdi_veri_tipi_secimi.Size = new System.Drawing.Size(139, 17);
            this.label_girdi_veri_tipi_secimi.TabIndex = 1;
            this.label_girdi_veri_tipi_secimi.Text = "Dosya Veri Tipi Seçimi:";
            // 
            // label_girdi_dosya_secimi
            // 
            this.label_girdi_dosya_secimi.AutoSize = true;
            this.label_girdi_dosya_secimi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_girdi_dosya_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.label_girdi_dosya_secimi.Location = new System.Drawing.Point(197, 4);
            this.label_girdi_dosya_secimi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_girdi_dosya_secimi.Name = "label_girdi_dosya_secimi";
            this.label_girdi_dosya_secimi.Size = new System.Drawing.Size(88, 17);
            this.label_girdi_dosya_secimi.TabIndex = 3;
            this.label_girdi_dosya_secimi.Text = "Dosya Seçimi:";
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.BackColor = System.Drawing.Color.Transparent;
            this.SelectFolderButton.BackgroundImage = global::SLF.Properties.Resources.Folder3;
            this.SelectFolderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectFolderButton.ForeColor = System.Drawing.Color.Transparent;
            this.SelectFolderButton.Location = new System.Drawing.Point(289, 14);
            this.SelectFolderButton.Margin = new System.Windows.Forms.Padding(2);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(43, 36);
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
            this.tab_dek.Location = new System.Drawing.Point(4, 48);
            this.tab_dek.Margin = new System.Windows.Forms.Padding(2);
            this.tab_dek.Name = "tab_dek";
            this.tab_dek.Size = new System.Drawing.Size(1360, 546);
            this.tab_dek.TabIndex = 6;
            this.tab_dek.Text = "DEK Modülü";
            this.tab_dek.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel1.Controls.Add(this.DEKPointsLayerCheckBox);
            this.panel1.Controls.Add(this.DEKProgressBar);
            this.panel1.Controls.Add(this.DEKStatusLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1174, 43);
            this.panel1.TabIndex = 45;
            // 
            // DEKPointsLayerCheckBox
            // 
            this.DEKPointsLayerCheckBox.AutoSize = true;
            this.DEKPointsLayerCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DEKPointsLayerCheckBox.Checked = true;
            this.DEKPointsLayerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DEKPointsLayerCheckBox.Location = new System.Drawing.Point(7, 11);
            this.DEKPointsLayerCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.DEKPointsLayerCheckBox.Name = "DEKPointsLayerCheckBox";
            this.DEKPointsLayerCheckBox.Size = new System.Drawing.Size(110, 21);
            this.DEKPointsLayerCheckBox.TabIndex = 61;
            this.DEKPointsLayerCheckBox.Text = "DEK Noktaları";
            this.DEKPointsLayerCheckBox.UseVisualStyleBackColor = true;
            this.DEKPointsLayerCheckBox.CheckedChanged += new System.EventHandler(this.DEKPointsLayerCheckBox_CheckedChanged);
            // 
            // DEKProgressBar
            // 
            this.DEKProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKProgressBar.Location = new System.Drawing.Point(998, 15);
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
            this.DEKStatusLabel.Location = new System.Drawing.Point(619, 15);
            this.DEKStatusLabel.Name = "DEKStatusLabel";
            this.DEKStatusLabel.Size = new System.Drawing.Size(42, 13);
            this.DEKStatusLabel.TabIndex = 59;
            this.DEKStatusLabel.Text = "Status:";
            this.DEKStatusLabel.Visible = false;
            // 
            // gMapControl_DEK
            // 
            this.gMapControl_DEK.AllowDrop = true;
            this.gMapControl_DEK.Bearing = 0F;
            this.gMapControl_DEK.CanDragMap = true;
            this.gMapControl_DEK.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_DEK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_DEK.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_DEK.GrayScaleMode = false;
            this.gMapControl_DEK.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_DEK.LevelsKeepInMemory = 5;
            this.gMapControl_DEK.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_DEK.Margin = new System.Windows.Forms.Padding(2);
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
            this.gMapControl_DEK.Size = new System.Drawing.Size(1174, 546);
            this.gMapControl_DEK.TabIndex = 38;
            this.gMapControl_DEK.Zoom = 0D;
            this.gMapControl_DEK.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_DEK_OnMapClick);
            this.gMapControl_DEK.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_Dek_OnMarkerClick);
            // 
            // panel_DEK
            // 
            this.panel_DEK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_DEK.Controls.Add(this.DEKCenterAddButton);
            this.panel_DEK.Controls.Add(this.comboBox_DEK_il);
            this.panel_DEK.Controls.Add(this.DEKSimulasyonSonucGoruntule);
            this.panel_DEK.Controls.Add(this.DEKRunSimulationButton);
            this.panel_DEK.Controls.Add(this.comboBox_dek_ilce_secimi);
            this.panel_DEK.Controls.Add(this.dekSimMaxBtn);
            this.panel_DEK.Controls.Add(this.dekSimDefBtn);
            this.panel_DEK.Controls.Add(this.dekSimMinBtn);
            this.panel_DEK.Controls.Add(this.DEKSimButton);
            this.panel_DEK.Controls.Add(this.label_DEK_Gelecek);
            this.panel_DEK.Controls.Add(this.comboBox_DEK_Yıl);
            this.panel_DEK.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_DEK.Location = new System.Drawing.Point(1174, 0);
            this.panel_DEK.Margin = new System.Windows.Forms.Padding(2);
            this.panel_DEK.Name = "panel_DEK";
            this.panel_DEK.Size = new System.Drawing.Size(186, 546);
            this.panel_DEK.TabIndex = 37;
            // 
            // DEKCenterAddButton
            // 
            this.DEKCenterAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKCenterAddButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKCenterAddButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKCenterAddButton.BorderRadius = 0;
            this.DEKCenterAddButton.BorderSize = 0;
            this.DEKCenterAddButton.Enabled = false;
            this.DEKCenterAddButton.FlatAppearance.BorderSize = 0;
            this.DEKCenterAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKCenterAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCenterAddButton.ForeColor = System.Drawing.Color.White;
            this.DEKCenterAddButton.Location = new System.Drawing.Point(26, 444);
            this.DEKCenterAddButton.Name = "DEKCenterAddButton";
            this.DEKCenterAddButton.Size = new System.Drawing.Size(144, 40);
            this.DEKCenterAddButton.TabIndex = 55;
            this.DEKCenterAddButton.Text = "Dagıtık Üretim Merkezi Ekle ";
            this.DEKCenterAddButton.TextColor = System.Drawing.Color.White;
            this.DEKCenterAddButton.UseVisualStyleBackColor = false;
            this.DEKCenterAddButton.Click += new System.EventHandler(this.DEKCenterAddButton_Click);
            // 
            // comboBox_DEK_il
            // 
            this.comboBox_DEK_il.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.comboBox_DEK_il.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_DEK_il.FormattingEnabled = true;
            this.comboBox_DEK_il.Location = new System.Drawing.Point(10, 223);
            this.comboBox_DEK_il.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_DEK_il.Name = "comboBox_DEK_il";
            this.comboBox_DEK_il.Size = new System.Drawing.Size(68, 25);
            this.comboBox_DEK_il.TabIndex = 1;
            this.comboBox_DEK_il.Text = "İL";
            this.comboBox_DEK_il.SelectedIndexChanged += new System.EventHandler(this.dek_city_SelectedIndexChanged);
            // 
            // DEKSimulasyonSonucGoruntule
            // 
            this.DEKSimulasyonSonucGoruntule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DEKSimulasyonSonucGoruntule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimulasyonSonucGoruntule.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimulasyonSonucGoruntule.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKSimulasyonSonucGoruntule.BorderRadius = 0;
            this.DEKSimulasyonSonucGoruntule.BorderSize = 0;
            this.DEKSimulasyonSonucGoruntule.Enabled = false;
            this.DEKSimulasyonSonucGoruntule.FlatAppearance.BorderSize = 0;
            this.DEKSimulasyonSonucGoruntule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKSimulasyonSonucGoruntule.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKSimulasyonSonucGoruntule.ForeColor = System.Drawing.Color.White;
            this.DEKSimulasyonSonucGoruntule.Location = new System.Drawing.Point(30, 142);
            this.DEKSimulasyonSonucGoruntule.Name = "DEKSimulasyonSonucGoruntule";
            this.DEKSimulasyonSonucGoruntule.Size = new System.Drawing.Size(140, 42);
            this.DEKSimulasyonSonucGoruntule.TabIndex = 62;
            this.DEKSimulasyonSonucGoruntule.Text = "Sonuçları Getir";
            this.DEKSimulasyonSonucGoruntule.TextColor = System.Drawing.Color.White;
            this.DEKSimulasyonSonucGoruntule.UseVisualStyleBackColor = false;
            this.DEKSimulasyonSonucGoruntule.Click += new System.EventHandler(this.DEKSimulasyonSonucGoruntule_Click);
            // 
            // DEKRunSimulationButton
            // 
            this.DEKRunSimulationButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKRunSimulationButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKRunSimulationButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKRunSimulationButton.BorderRadius = 0;
            this.DEKRunSimulationButton.BorderSize = 0;
            this.DEKRunSimulationButton.FlatAppearance.BorderSize = 0;
            this.DEKRunSimulationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKRunSimulationButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKRunSimulationButton.ForeColor = System.Drawing.Color.White;
            this.DEKRunSimulationButton.Location = new System.Drawing.Point(26, 41);
            this.DEKRunSimulationButton.Name = "DEKRunSimulationButton";
            this.DEKRunSimulationButton.Size = new System.Drawing.Size(144, 40);
            this.DEKRunSimulationButton.TabIndex = 61;
            this.DEKRunSimulationButton.Text = "DEK Gelecek Simülasyonu Oluştur";
            this.DEKRunSimulationButton.TextColor = System.Drawing.Color.White;
            this.DEKRunSimulationButton.UseVisualStyleBackColor = false;
            this.DEKRunSimulationButton.Click += new System.EventHandler(this.DEKRunSimulationButton_Click);
            // 
            // comboBox_dek_ilce_secimi
            // 
            this.comboBox_dek_ilce_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_dek_ilce_secimi.FormattingEnabled = true;
            this.comboBox_dek_ilce_secimi.Location = new System.Drawing.Point(100, 223);
            this.comboBox_dek_ilce_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_dek_ilce_secimi.Name = "comboBox_dek_ilce_secimi";
            this.comboBox_dek_ilce_secimi.Size = new System.Drawing.Size(70, 25);
            this.comboBox_dek_ilce_secimi.TabIndex = 60;
            this.comboBox_dek_ilce_secimi.Text = "İLÇE";
            this.comboBox_dek_ilce_secimi.SelectedIndexChanged += new System.EventHandler(this.ilceSecimiDEK);
            // 
            // dekSimMaxBtn
            // 
            this.dekSimMaxBtn.AutoSize = true;
            this.dekSimMaxBtn.Location = new System.Drawing.Point(41, 317);
            this.dekSimMaxBtn.Name = "dekSimMaxBtn";
            this.dekSimMaxBtn.Size = new System.Drawing.Size(105, 21);
            this.dekSimMaxBtn.TabIndex = 59;
            this.dekSimMaxBtn.TabStop = true;
            this.dekSimMaxBtn.Text = "Hızlı Senaryo";
            this.dekSimMaxBtn.UseVisualStyleBackColor = true;
            this.dekSimMaxBtn.CheckedChanged += new System.EventHandler(this.dekSimMaxBtn_CheckedChanged);
            // 
            // dekSimDefBtn
            // 
            this.dekSimDefBtn.AutoSize = true;
            this.dekSimDefBtn.Location = new System.Drawing.Point(41, 290);
            this.dekSimDefBtn.Name = "dekSimDefBtn";
            this.dekSimDefBtn.Size = new System.Drawing.Size(140, 21);
            this.dekSimDefBtn.TabIndex = 58;
            this.dekSimDefBtn.TabStop = true;
            this.dekSimDefBtn.Text = "Varsayılan Senaryo";
            this.dekSimDefBtn.UseVisualStyleBackColor = true;
            this.dekSimDefBtn.CheckedChanged += new System.EventHandler(this.dekSimDefBtn_CheckedChanged);
            // 
            // dekSimMinBtn
            // 
            this.dekSimMinBtn.AutoSize = true;
            this.dekSimMinBtn.Location = new System.Drawing.Point(41, 263);
            this.dekSimMinBtn.Name = "dekSimMinBtn";
            this.dekSimMinBtn.Size = new System.Drawing.Size(114, 21);
            this.dekSimMinBtn.TabIndex = 57;
            this.dekSimMinBtn.TabStop = true;
            this.dekSimMinBtn.Text = "Yavaş Senaryo";
            this.dekSimMinBtn.UseVisualStyleBackColor = true;
            this.dekSimMinBtn.CheckedChanged += new System.EventHandler(this.dekSimMinBtn_CheckedChanged);
            // 
            // DEKSimButton
            // 
            this.DEKSimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKSimButton.BorderRadius = 0;
            this.DEKSimButton.BorderSize = 0;
            this.DEKSimButton.Enabled = false;
            this.DEKSimButton.FlatAppearance.BorderSize = 0;
            this.DEKSimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKSimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKSimButton.ForeColor = System.Drawing.Color.White;
            this.DEKSimButton.Location = new System.Drawing.Point(26, 365);
            this.DEKSimButton.Name = "DEKSimButton";
            this.DEKSimButton.Size = new System.Drawing.Size(144, 40);
            this.DEKSimButton.TabIndex = 54;
            this.DEKSimButton.Text = "DEK Senaryo Sonuçları Görüntüle";
            this.DEKSimButton.TextColor = System.Drawing.Color.White;
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
            this.label_DEK_Gelecek.Location = new System.Drawing.Point(7, 9);
            this.label_DEK_Gelecek.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_DEK_Gelecek.Name = "label_DEK_Gelecek";
            this.label_DEK_Gelecek.Size = new System.Drawing.Size(163, 17);
            this.label_DEK_Gelecek.TabIndex = 3;
            this.label_DEK_Gelecek.Text = "DEK Gelecek Simülasyonu";
            // 
            // comboBox_DEK_Yıl
            // 
            this.comboBox_DEK_Yıl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.comboBox_DEK_Yıl.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_DEK_Yıl.FormattingEnabled = true;
            this.comboBox_DEK_Yıl.Location = new System.Drawing.Point(63, 96);
            this.comboBox_DEK_Yıl.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_DEK_Yıl.Name = "comboBox_DEK_Yıl";
            this.comboBox_DEK_Yıl.Size = new System.Drawing.Size(69, 25);
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
            this.tab_ea.Location = new System.Drawing.Point(4, 48);
            this.tab_ea.Margin = new System.Windows.Forms.Padding(2);
            this.tab_ea.Name = "tab_ea";
            this.tab_ea.Size = new System.Drawing.Size(1360, 546);
            this.tab_ea.TabIndex = 5;
            this.tab_ea.Text = "EA Şarj Modülü";
            this.tab_ea.UseVisualStyleBackColor = true;
            // 
            // EAStationsLegendPanel
            // 
            this.EAStationsLegendPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EAStationsLegendPanel.Controls.Add(this.SimulasyonSonucGoruntule);
            this.EAStationsLegendPanel.Controls.Add(this.EANewSimulationResultsButton);
            this.EAStationsLegendPanel.Controls.Add(this.comboBox_ea_ilce_secimi);
            this.EAStationsLegendPanel.Controls.Add(this.EASimButton);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimMaxBtn);
            this.EAStationsLegendPanel.Controls.Add(this.FutureSimLabel);
            this.EAStationsLegendPanel.Controls.Add(this.comboBox_ea_il_secimi);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimDefBtn);
            this.EAStationsLegendPanel.Controls.Add(this.comboBox_ea_yıl_secimi);
            this.EAStationsLegendPanel.Controls.Add(this.EaSimMinBtn);
            this.EAStationsLegendPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EAStationsLegendPanel.Location = new System.Drawing.Point(952, 43);
            this.EAStationsLegendPanel.Name = "EAStationsLegendPanel";
            this.EAStationsLegendPanel.Size = new System.Drawing.Size(206, 503);
            this.EAStationsLegendPanel.TabIndex = 51;
            // 
            // SimulasyonSonucGoruntule
            // 
            this.SimulasyonSonucGoruntule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SimulasyonSonucGoruntule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SimulasyonSonucGoruntule.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SimulasyonSonucGoruntule.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.SimulasyonSonucGoruntule.BorderRadius = 0;
            this.SimulasyonSonucGoruntule.BorderSize = 0;
            this.SimulasyonSonucGoruntule.Enabled = false;
            this.SimulasyonSonucGoruntule.FlatAppearance.BorderSize = 0;
            this.SimulasyonSonucGoruntule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SimulasyonSonucGoruntule.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SimulasyonSonucGoruntule.ForeColor = System.Drawing.Color.White;
            this.SimulasyonSonucGoruntule.Location = new System.Drawing.Point(30, 147);
            this.SimulasyonSonucGoruntule.Name = "SimulasyonSonucGoruntule";
            this.SimulasyonSonucGoruntule.Size = new System.Drawing.Size(140, 42);
            this.SimulasyonSonucGoruntule.TabIndex = 60;
            this.SimulasyonSonucGoruntule.Text = "Sonuçları Getir";
            this.SimulasyonSonucGoruntule.TextColor = System.Drawing.Color.White;
            this.SimulasyonSonucGoruntule.UseVisualStyleBackColor = false;
            this.SimulasyonSonucGoruntule.Click += new System.EventHandler(this.SimulasyonSonucGoruntule_Click);
            // 
            // EANewSimulationResultsButton
            // 
            this.EANewSimulationResultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EANewSimulationResultsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EANewSimulationResultsButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EANewSimulationResultsButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.EANewSimulationResultsButton.BorderRadius = 0;
            this.EANewSimulationResultsButton.BorderSize = 0;
            this.EANewSimulationResultsButton.FlatAppearance.BorderSize = 0;
            this.EANewSimulationResultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EANewSimulationResultsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EANewSimulationResultsButton.ForeColor = System.Drawing.Color.White;
            this.EANewSimulationResultsButton.Location = new System.Drawing.Point(30, 34);
            this.EANewSimulationResultsButton.Name = "EANewSimulationResultsButton";
            this.EANewSimulationResultsButton.Size = new System.Drawing.Size(140, 42);
            this.EANewSimulationResultsButton.TabIndex = 58;
            this.EANewSimulationResultsButton.Text = "Gelecek Simülasyonu Oluştur";
            this.EANewSimulationResultsButton.TextColor = System.Drawing.Color.White;
            this.EANewSimulationResultsButton.UseVisualStyleBackColor = false;
            this.EANewSimulationResultsButton.Click += new System.EventHandler(this.EANewSimulationResultsButton_Click);
            // 
            // comboBox_ea_ilce_secimi
            // 
            this.comboBox_ea_ilce_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_ea_ilce_secimi.FormattingEnabled = true;
            this.comboBox_ea_ilce_secimi.Location = new System.Drawing.Point(117, 269);
            this.comboBox_ea_ilce_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_ea_ilce_secimi.Name = "comboBox_ea_ilce_secimi";
            this.comboBox_ea_ilce_secimi.Size = new System.Drawing.Size(70, 25);
            this.comboBox_ea_ilce_secimi.TabIndex = 59;
            this.comboBox_ea_ilce_secimi.Text = "İLÇE";
            this.comboBox_ea_ilce_secimi.SelectedIndexChanged += new System.EventHandler(this.ilceSecimiMonteCarlo);
            // 
            // EASimButton
            // 
            this.EASimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EASimButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EASimButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.EASimButton.BorderRadius = 0;
            this.EASimButton.BorderSize = 0;
            this.EASimButton.Enabled = false;
            this.EASimButton.FlatAppearance.BorderSize = 0;
            this.EASimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EASimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EASimButton.ForeColor = System.Drawing.Color.White;
            this.EASimButton.Location = new System.Drawing.Point(38, 425);
            this.EASimButton.Name = "EASimButton";
            this.EASimButton.Size = new System.Drawing.Size(140, 42);
            this.EASimButton.TabIndex = 50;
            this.EASimButton.Text = "Gelecek Senaryo Görüntüle";
            this.EASimButton.TextColor = System.Drawing.Color.White;
            this.EASimButton.UseVisualStyleBackColor = false;
            this.EASimButton.Click += new System.EventHandler(this.gelecekSimilasyonGoruntule);
            // 
            // EaSimMaxBtn
            // 
            this.EaSimMaxBtn.AutoSize = true;
            this.EaSimMaxBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimMaxBtn.Location = new System.Drawing.Point(39, 381);
            this.EaSimMaxBtn.Name = "EaSimMaxBtn";
            this.EaSimMaxBtn.Size = new System.Drawing.Size(105, 21);
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
            this.FutureSimLabel.Location = new System.Drawing.Point(32, 12);
            this.FutureSimLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FutureSimLabel.Name = "FutureSimLabel";
            this.FutureSimLabel.Size = new System.Drawing.Size(138, 17);
            this.FutureSimLabel.TabIndex = 2;
            this.FutureSimLabel.Text = "Gelecek Simülasyonu:";
            // 
            // comboBox_ea_il_secimi
            // 
            this.comboBox_ea_il_secimi.ForeColor = System.Drawing.Color.DarkBlue;
            this.comboBox_ea_il_secimi.FormattingEnabled = true;
            this.comboBox_ea_il_secimi.Items.AddRange(new object[] {
            "İzmir",
            "Eskişehir"});
            this.comboBox_ea_il_secimi.Location = new System.Drawing.Point(16, 269);
            this.comboBox_ea_il_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_ea_il_secimi.Name = "comboBox_ea_il_secimi";
            this.comboBox_ea_il_secimi.Size = new System.Drawing.Size(70, 25);
            this.comboBox_ea_il_secimi.TabIndex = 1;
            this.comboBox_ea_il_secimi.Text = "İL";
            this.comboBox_ea_il_secimi.SelectedIndexChanged += new System.EventHandler(this.ilSecimiMonteCarlo);
            // 
            // EaSimDefBtn
            // 
            this.EaSimDefBtn.AutoSize = true;
            this.EaSimDefBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimDefBtn.Location = new System.Drawing.Point(38, 354);
            this.EaSimDefBtn.Name = "EaSimDefBtn";
            this.EaSimDefBtn.Size = new System.Drawing.Size(140, 21);
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
            this.comboBox_ea_yıl_secimi.Location = new System.Drawing.Point(61, 104);
            this.comboBox_ea_yıl_secimi.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_ea_yıl_secimi.Name = "comboBox_ea_yıl_secimi";
            this.comboBox_ea_yıl_secimi.Size = new System.Drawing.Size(70, 25);
            this.comboBox_ea_yıl_secimi.TabIndex = 0;
            this.comboBox_ea_yıl_secimi.Text = "YIL";
            this.comboBox_ea_yıl_secimi.SelectedIndexChanged += new System.EventHandler(this.yilSecimiMonteCarlo);
            // 
            // EaSimMinBtn
            // 
            this.EaSimMinBtn.AutoSize = true;
            this.EaSimMinBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.EaSimMinBtn.Location = new System.Drawing.Point(38, 327);
            this.EaSimMinBtn.Name = "EaSimMinBtn";
            this.EaSimMinBtn.Size = new System.Drawing.Size(114, 21);
            this.EaSimMinBtn.TabIndex = 51;
            this.EaSimMinBtn.TabStop = true;
            this.EaSimMinBtn.Text = "Yavaş Senaryo";
            this.EaSimMinBtn.UseVisualStyleBackColor = true;
            this.EaSimMinBtn.CheckedChanged += new System.EventHandler(this.EaSimMinBtn_CheckedChanged);
            // 
            // panel_ea
            // 
            this.panel_ea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_ea.Controls.Add(this.EAPointsLayerCheckBox);
            this.panel_ea.Controls.Add(this.progressBar);
            this.panel_ea.Controls.Add(this.statusLabel);
            this.panel_ea.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_ea.Location = new System.Drawing.Point(0, 0);
            this.panel_ea.Margin = new System.Windows.Forms.Padding(2);
            this.panel_ea.Name = "panel_ea";
            this.panel_ea.Size = new System.Drawing.Size(1158, 43);
            this.panel_ea.TabIndex = 44;
            // 
            // EAPointsLayerCheckBox
            // 
            this.EAPointsLayerCheckBox.AutoSize = true;
            this.EAPointsLayerCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.EAPointsLayerCheckBox.Checked = true;
            this.EAPointsLayerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EAPointsLayerCheckBox.Location = new System.Drawing.Point(7, 11);
            this.EAPointsLayerCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.EAPointsLayerCheckBox.Name = "EAPointsLayerCheckBox";
            this.EAPointsLayerCheckBox.Size = new System.Drawing.Size(102, 21);
            this.EAPointsLayerCheckBox.TabIndex = 61;
            this.EAPointsLayerCheckBox.Text = "EA Noktaları";
            this.EAPointsLayerCheckBox.UseVisualStyleBackColor = true;
            this.EAPointsLayerCheckBox.CheckedChanged += new System.EventHandler(this.EAPointsLayerCheckBox_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(982, 15);
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
            this.statusLabel.Location = new System.Drawing.Point(619, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 13);
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
            this.GelecekSimPanel.Location = new System.Drawing.Point(1158, 0);
            this.GelecekSimPanel.Margin = new System.Windows.Forms.Padding(2);
            this.GelecekSimPanel.Name = "GelecekSimPanel";
            this.GelecekSimPanel.Size = new System.Drawing.Size(202, 546);
            this.GelecekSimPanel.TabIndex = 43;
            // 
            // DCFastLegendValueLabel
            // 
            this.DCFastLegendValueLabel.AutoSize = true;
            this.DCFastLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DCFastLegendValueLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.DCFastLegendValueLabel.Location = new System.Drawing.Point(113, 190);
            this.DCFastLegendValueLabel.Name = "DCFastLegendValueLabel";
            this.DCFastLegendValueLabel.Size = new System.Drawing.Size(45, 13);
            this.DCFastLegendValueLabel.TabIndex = 57;
            this.DCFastLegendValueLabel.Text = "150 kW";
            // 
            // DCFastLegendLabel
            // 
            this.DCFastLegendLabel.AutoSize = true;
            this.DCFastLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.DCFastLegendLabel.Location = new System.Drawing.Point(19, 190);
            this.DCFastLegendLabel.Name = "DCFastLegendLabel";
            this.DCFastLegendLabel.Size = new System.Drawing.Size(55, 17);
            this.DCFastLegendLabel.TabIndex = 56;
            this.DCFastLegendLabel.Text = "DC-Fast";
            // 
            // checkBox_DC_Fast
            // 
            this.checkBox_DC_Fast.AutoSize = true;
            this.checkBox_DC_Fast.Checked = true;
            this.checkBox_DC_Fast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_DC_Fast.Location = new System.Drawing.Point(116, 371);
            this.checkBox_DC_Fast.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_DC_Fast.Name = "checkBox_DC_Fast";
            this.checkBox_DC_Fast.Size = new System.Drawing.Size(78, 21);
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
            this.checkBox_AC_Public.Location = new System.Drawing.Point(8, 371);
            this.checkBox_AC_Public.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_AC_Public.Name = "checkBox_AC_Public";
            this.checkBox_AC_Public.Size = new System.Drawing.Size(92, 21);
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
            this.ACPublicLegendValueLabel.Size = new System.Drawing.Size(42, 13);
            this.ACPublicLegendValueLabel.TabIndex = 55;
            this.ACPublicLegendValueLabel.Text = " 22 kW";
            // 
            // ACPublicLegendLabel
            // 
            this.ACPublicLegendLabel.AutoSize = true;
            this.ACPublicLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACPublicLegendLabel.Location = new System.Drawing.Point(19, 156);
            this.ACPublicLegendLabel.Name = "ACPublicLegendLabel";
            this.ACPublicLegendLabel.Size = new System.Drawing.Size(66, 17);
            this.ACPublicLegendLabel.TabIndex = 54;
            this.ACPublicLegendLabel.Text = "AC-Public";
            // 
            // checkBox_AC_Home
            // 
            this.checkBox_AC_Home.AutoSize = true;
            this.checkBox_AC_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_AC_Home.Checked = true;
            this.checkBox_AC_Home.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Home.Location = new System.Drawing.Point(6, 339);
            this.checkBox_AC_Home.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_AC_Home.Name = "checkBox_AC_Home";
            this.checkBox_AC_Home.Size = new System.Drawing.Size(88, 21);
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
            this.AddStationLabel.Location = new System.Drawing.Point(16, 55);
            this.AddStationLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.AddStationLabel.Name = "AddStationLabel";
            this.AddStationLabel.Size = new System.Drawing.Size(102, 17);
            this.AddStationLabel.TabIndex = 45;
            this.AddStationLabel.Text = "İstasyon Tipleri:";
            // 
            // checkBox_AC_Work
            // 
            this.checkBox_AC_Work.AutoSize = true;
            this.checkBox_AC_Work.Checked = true;
            this.checkBox_AC_Work.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Work.Location = new System.Drawing.Point(107, 339);
            this.checkBox_AC_Work.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_AC_Work.Name = "checkBox_AC_Work";
            this.checkBox_AC_Work.Size = new System.Drawing.Size(88, 21);
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
            this.ACWorkLegendValueLabel.Size = new System.Drawing.Size(42, 13);
            this.ACWorkLegendValueLabel.TabIndex = 53;
            this.ACWorkLegendValueLabel.Text = " 11 kW";
            // 
            // EAStationAddButton
            // 
            this.EAStationAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EAStationAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EAStationAddButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EAStationAddButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.EAStationAddButton.BorderRadius = 0;
            this.EAStationAddButton.BorderSize = 0;
            this.EAStationAddButton.Enabled = false;
            this.EAStationAddButton.FlatAppearance.BorderSize = 0;
            this.EAStationAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EAStationAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EAStationAddButton.ForeColor = System.Drawing.Color.White;
            this.EAStationAddButton.Location = new System.Drawing.Point(37, 248);
            this.EAStationAddButton.Name = "EAStationAddButton";
            this.EAStationAddButton.Size = new System.Drawing.Size(140, 42);
            this.EAStationAddButton.TabIndex = 49;
            this.EAStationAddButton.Text = "EA Şarj İstasyonu Ekle";
            this.EAStationAddButton.TextColor = System.Drawing.Color.White;
            this.EAStationAddButton.UseVisualStyleBackColor = false;
            this.EAStationAddButton.Click += new System.EventHandler(this.EAStationAddButton_Click);
            // 
            // ACHomeLegendLabel
            // 
            this.ACHomeLegendLabel.AutoSize = true;
            this.ACHomeLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACHomeLegendLabel.Location = new System.Drawing.Point(17, 88);
            this.ACHomeLegendLabel.Name = "ACHomeLegendLabel";
            this.ACHomeLegendLabel.Size = new System.Drawing.Size(67, 17);
            this.ACHomeLegendLabel.TabIndex = 50;
            this.ACHomeLegendLabel.Text = "AC-Home";
            // 
            // ACWorkLegendLabel
            // 
            this.ACWorkLegendLabel.AutoSize = true;
            this.ACWorkLegendLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.ACWorkLegendLabel.Location = new System.Drawing.Point(19, 124);
            this.ACWorkLegendLabel.Name = "ACWorkLegendLabel";
            this.ACWorkLegendLabel.Size = new System.Drawing.Size(63, 17);
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
            this.ACHomeLegendValueLabel.Size = new System.Drawing.Size(39, 13);
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
            this.gMapControl_EA.Cursor = System.Windows.Forms.Cursors.Default;
            this.gMapControl_EA.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_EA.GrayScaleMode = false;
            this.gMapControl_EA.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_EA.LevelsKeepInMemory = 5;
            this.gMapControl_EA.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_EA.Margin = new System.Windows.Forms.Padding(2);
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
            this.gMapControl_EA.Size = new System.Drawing.Size(1358, 3077);
            this.gMapControl_EA.TabIndex = 18;
            this.gMapControl_EA.Zoom = 0D;
            // 
            // tab_ekonometrik
            // 
            this.tab_ekonometrik.AutoScroll = true;
            this.tab_ekonometrik.Controls.Add(this.ELFResultsTabControls);
            this.tab_ekonometrik.Controls.Add(this.ELFTablePanel);
            this.tab_ekonometrik.Controls.Add(this.ELFGraphicsPanel);
            this.tab_ekonometrik.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tab_ekonometrik.ImageIndex = 4;
            this.tab_ekonometrik.Location = new System.Drawing.Point(4, 48);
            this.tab_ekonometrik.Margin = new System.Windows.Forms.Padding(2);
            this.tab_ekonometrik.Name = "tab_ekonometrik";
            this.tab_ekonometrik.Padding = new System.Windows.Forms.Padding(2);
            this.tab_ekonometrik.Size = new System.Drawing.Size(1360, 546);
            this.tab_ekonometrik.TabIndex = 1;
            this.tab_ekonometrik.Text = "Ekonometrik Talep Tahmini Modülü";
            this.tab_ekonometrik.UseVisualStyleBackColor = true;
            // 
            // ELFResultsTabControls
            // 
            this.ELFResultsTabControls.Controls.Add(this.ELFMinResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFLowResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFBaseResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFHighResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFMaxResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFGraphicOutputsTabPage);
            this.ELFResultsTabControls.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ELFResultsTabControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFResultsTabControls.Location = new System.Drawing.Point(189, 2);
            this.ELFResultsTabControls.Margin = new System.Windows.Forms.Padding(2);
            this.ELFResultsTabControls.Name = "ELFResultsTabControls";
            this.ELFResultsTabControls.SelectedIndex = 0;
            this.ELFResultsTabControls.Size = new System.Drawing.Size(1169, 542);
            this.ELFResultsTabControls.TabIndex = 3;
            // 
            // ELFMinResultsTabPage
            // 
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinResultsTable);
            this.ELFMinResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFMinResultsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMinResultsTabPage.Name = "ELFMinResultsTabPage";
            this.ELFMinResultsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFMinResultsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFMinResultsTabPage.TabIndex = 0;
            this.ELFMinResultsTabPage.Text = "Minimum Sonuçlar";
            this.ELFMinResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFMinResultsTable
            // 
            this.ELFMinResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFMinResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMinResultsTable.DefaultCellStyle = dataGridViewCellStyle4;
            this.ELFMinResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMinResultsTable.Location = new System.Drawing.Point(2, 2);
            this.ELFMinResultsTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMinResultsTable.Name = "ELFMinResultsTable";
            this.ELFMinResultsTable.RowHeadersWidth = 51;
            this.ELFMinResultsTable.Size = new System.Drawing.Size(1157, 508);
            this.ELFMinResultsTable.TabIndex = 0;
            // 
            // ELFLowResultsTabPage
            // 
            this.ELFLowResultsTabPage.Controls.Add(this.ELFLowResultsTable);
            this.ELFLowResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFLowResultsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFLowResultsTabPage.Name = "ELFLowResultsTabPage";
            this.ELFLowResultsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFLowResultsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFLowResultsTabPage.TabIndex = 1;
            this.ELFLowResultsTabPage.Text = "Düşük Sonuçlar";
            this.ELFLowResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFLowResultsTable
            // 
            this.ELFLowResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFLowResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFLowResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFLowResultsTable.Location = new System.Drawing.Point(2, 2);
            this.ELFLowResultsTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFLowResultsTable.Name = "ELFLowResultsTable";
            this.ELFLowResultsTable.RowHeadersWidth = 51;
            this.ELFLowResultsTable.Size = new System.Drawing.Size(1157, 508);
            this.ELFLowResultsTable.TabIndex = 1;
            // 
            // ELFBaseResultsTabPage
            // 
            this.ELFBaseResultsTabPage.Controls.Add(this.ELFBaseResultsTable);
            this.ELFBaseResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFBaseResultsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFBaseResultsTabPage.Name = "ELFBaseResultsTabPage";
            this.ELFBaseResultsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFBaseResultsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFBaseResultsTabPage.TabIndex = 2;
            this.ELFBaseResultsTabPage.Text = "Baz Sonuçlar";
            this.ELFBaseResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFBaseResultsTable
            // 
            this.ELFBaseResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFBaseResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBaseResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBaseResultsTable.Location = new System.Drawing.Point(2, 2);
            this.ELFBaseResultsTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFBaseResultsTable.Name = "ELFBaseResultsTable";
            this.ELFBaseResultsTable.RowHeadersWidth = 51;
            this.ELFBaseResultsTable.Size = new System.Drawing.Size(1157, 508);
            this.ELFBaseResultsTable.TabIndex = 1;
            // 
            // ELFHighResultsTabPage
            // 
            this.ELFHighResultsTabPage.Controls.Add(this.ELFHighResultsTable);
            this.ELFHighResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFHighResultsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFHighResultsTabPage.Name = "ELFHighResultsTabPage";
            this.ELFHighResultsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFHighResultsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFHighResultsTabPage.TabIndex = 3;
            this.ELFHighResultsTabPage.Text = "Yüksek Sonuçlar";
            this.ELFHighResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFHighResultsTable
            // 
            this.ELFHighResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFHighResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFHighResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFHighResultsTable.Location = new System.Drawing.Point(2, 2);
            this.ELFHighResultsTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFHighResultsTable.Name = "ELFHighResultsTable";
            this.ELFHighResultsTable.RowHeadersWidth = 51;
            this.ELFHighResultsTable.Size = new System.Drawing.Size(1157, 508);
            this.ELFHighResultsTable.TabIndex = 1;
            // 
            // ELFMaxResultsTabPage
            // 
            this.ELFMaxResultsTabPage.Controls.Add(this.ELFMaxResultsTable);
            this.ELFMaxResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFMaxResultsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMaxResultsTabPage.Name = "ELFMaxResultsTabPage";
            this.ELFMaxResultsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFMaxResultsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFMaxResultsTabPage.TabIndex = 4;
            this.ELFMaxResultsTabPage.Text = "Maksimum Sonuçlar";
            this.ELFMaxResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFMaxResultsTable
            // 
            this.ELFMaxResultsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFMaxResultsTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFMaxResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMaxResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxResultsTable.Cursor = System.Windows.Forms.Cursors.Default;
            this.ELFMaxResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMaxResultsTable.Location = new System.Drawing.Point(0, 2);
            this.ELFMaxResultsTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMaxResultsTable.Name = "ELFMaxResultsTable";
            this.ELFMaxResultsTable.RowHeadersWidth = 51;
            this.ELFMaxResultsTable.Size = new System.Drawing.Size(1082, 165);
            this.ELFMaxResultsTable.TabIndex = 1;
            // 
            // ELFGraphicOutputsTabPage
            // 
            this.ELFGraphicOutputsTabPage.Controls.Add(this.panel_ELF_Grafikler);
            this.ELFGraphicOutputsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFGraphicOutputsTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ELFGraphicOutputsTabPage.Name = "ELFGraphicOutputsTabPage";
            this.ELFGraphicOutputsTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ELFGraphicOutputsTabPage.Size = new System.Drawing.Size(1161, 512);
            this.ELFGraphicOutputsTabPage.TabIndex = 5;
            this.ELFGraphicOutputsTabPage.Text = "Projeksiyon Grafik Sonuçları";
            this.ELFGraphicOutputsTabPage.UseVisualStyleBackColor = true;
            // 
            // panel_ELF_Grafikler
            // 
            this.panel_ELF_Grafikler.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_5);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_4);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_3);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_2);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_1);
            this.panel_ELF_Grafikler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_ELF_Grafikler.Location = new System.Drawing.Point(2, 2);
            this.panel_ELF_Grafikler.Name = "panel_ELF_Grafikler";
            this.panel_ELF_Grafikler.Size = new System.Drawing.Size(1157, 508);
            this.panel_ELF_Grafikler.TabIndex = 6;
            // 
            // pictureBox_ELF_5
            // 
            this.pictureBox_ELF_5.Location = new System.Drawing.Point(524, 110);
            this.pictureBox_ELF_5.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ELF_5.Name = "pictureBox_ELF_5";
            this.pictureBox_ELF_5.Size = new System.Drawing.Size(256, 229);
            this.pictureBox_ELF_5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_5.TabIndex = 4;
            this.pictureBox_ELF_5.TabStop = false;
            // 
            // pictureBox_ELF_4
            // 
            this.pictureBox_ELF_4.Location = new System.Drawing.Point(263, 236);
            this.pictureBox_ELF_4.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ELF_4.Name = "pictureBox_ELF_4";
            this.pictureBox_ELF_4.Size = new System.Drawing.Size(256, 229);
            this.pictureBox_ELF_4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_4.TabIndex = 3;
            this.pictureBox_ELF_4.TabStop = false;
            // 
            // pictureBox_ELF_3
            // 
            this.pictureBox_ELF_3.Location = new System.Drawing.Point(263, 2);
            this.pictureBox_ELF_3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ELF_3.Name = "pictureBox_ELF_3";
            this.pictureBox_ELF_3.Size = new System.Drawing.Size(256, 229);
            this.pictureBox_ELF_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_3.TabIndex = 2;
            this.pictureBox_ELF_3.TabStop = false;
            // 
            // pictureBox_ELF_2
            // 
            this.pictureBox_ELF_2.Location = new System.Drawing.Point(2, 236);
            this.pictureBox_ELF_2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ELF_2.Name = "pictureBox_ELF_2";
            this.pictureBox_ELF_2.Size = new System.Drawing.Size(256, 229);
            this.pictureBox_ELF_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_2.TabIndex = 1;
            this.pictureBox_ELF_2.TabStop = false;
            // 
            // pictureBox_ELF_1
            // 
            this.pictureBox_ELF_1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox_ELF_1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ELF_1.Name = "pictureBox_ELF_1";
            this.pictureBox_ELF_1.Size = new System.Drawing.Size(256, 229);
            this.pictureBox_ELF_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_1.TabIndex = 0;
            this.pictureBox_ELF_1.TabStop = false;
            // 
            // ELFTablePanel
            // 
            this.ELFTablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFTablePanel.Location = new System.Drawing.Point(230, 2);
            this.ELFTablePanel.Margin = new System.Windows.Forms.Padding(2);
            this.ELFTablePanel.Name = "ELFTablePanel";
            this.ELFTablePanel.Size = new System.Drawing.Size(724, 7494);
            this.ELFTablePanel.TabIndex = 21;
            // 
            // ELFGraphicsPanel
            // 
            this.ELFGraphicsPanel.AutoScroll = true;
            this.ELFGraphicsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.ELFGraphicsPanel.Controls.Add(this.ELFShowGraphsButton);
            this.ELFGraphicsPanel.Controls.Add(this.SenaryoNewSelectionButton);
            this.ELFGraphicsPanel.Controls.Add(this.SenaryoResultsLabel);
            this.ELFGraphicsPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ELFGraphicsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ELFGraphicsPanel.Location = new System.Drawing.Point(2, 2);
            this.ELFGraphicsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.ELFGraphicsPanel.Name = "ELFGraphicsPanel";
            this.ELFGraphicsPanel.Size = new System.Drawing.Size(187, 542);
            this.ELFGraphicsPanel.TabIndex = 22;
            // 
            // ELFShowGraphsButton
            // 
            this.ELFShowGraphsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFShowGraphsButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFShowGraphsButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.ELFShowGraphsButton.BorderRadius = 0;
            this.ELFShowGraphsButton.BorderSize = 0;
            this.ELFShowGraphsButton.FlatAppearance.BorderSize = 0;
            this.ELFShowGraphsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ELFShowGraphsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ELFShowGraphsButton.ForeColor = System.Drawing.Color.White;
            this.ELFShowGraphsButton.Location = new System.Drawing.Point(18, 251);
            this.ELFShowGraphsButton.Name = "ELFShowGraphsButton";
            this.ELFShowGraphsButton.Size = new System.Drawing.Size(150, 40);
            this.ELFShowGraphsButton.TabIndex = 34;
            this.ELFShowGraphsButton.Text = "Grafik Sonuçlarını Göster";
            this.ELFShowGraphsButton.TextColor = System.Drawing.Color.White;
            this.ELFShowGraphsButton.UseVisualStyleBackColor = false;
            this.ELFShowGraphsButton.Click += new System.EventHandler(this.ELFShowGraphsButton_Click);
            // 
            // SenaryoNewSelectionButton
            // 
            this.SenaryoNewSelectionButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SenaryoNewSelectionButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SenaryoNewSelectionButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.SenaryoNewSelectionButton.BorderRadius = 0;
            this.SenaryoNewSelectionButton.BorderSize = 0;
            this.SenaryoNewSelectionButton.FlatAppearance.BorderSize = 0;
            this.SenaryoNewSelectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SenaryoNewSelectionButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SenaryoNewSelectionButton.ForeColor = System.Drawing.Color.White;
            this.SenaryoNewSelectionButton.Location = new System.Drawing.Point(18, 186);
            this.SenaryoNewSelectionButton.Name = "SenaryoNewSelectionButton";
            this.SenaryoNewSelectionButton.Size = new System.Drawing.Size(150, 40);
            this.SenaryoNewSelectionButton.TabIndex = 33;
            this.SenaryoNewSelectionButton.Text = "Yeniden Senaryo Oluştur";
            this.SenaryoNewSelectionButton.TextColor = System.Drawing.Color.White;
            this.SenaryoNewSelectionButton.UseVisualStyleBackColor = false;
            this.SenaryoNewSelectionButton.Click += new System.EventHandler(this.SenaryoNewSelectionButton_Click);
            // 
            // SenaryoResultsLabel
            // 
            this.SenaryoResultsLabel.AutoSize = true;
            this.SenaryoResultsLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SenaryoResultsLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.SenaryoResultsLabel.Location = new System.Drawing.Point(4, 24);
            this.SenaryoResultsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SenaryoResultsLabel.Name = "SenaryoResultsLabel";
            this.SenaryoResultsLabel.Size = new System.Drawing.Size(182, 21);
            this.SenaryoResultsLabel.TabIndex = 7;
            this.SenaryoResultsLabel.Text = "ELF Senaryo Sonuçları:";
            // 
            // tab_imar
            // 
            this.tab_imar.Controls.Add(this.checkBox_imar_15);
            this.tab_imar.Controls.Add(this.checkBox_imar_14);
            this.tab_imar.Controls.Add(this.panel_imar);
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
            this.tab_imar.Location = new System.Drawing.Point(4, 48);
            this.tab_imar.Margin = new System.Windows.Forms.Padding(2);
            this.tab_imar.Name = "tab_imar";
            this.tab_imar.Size = new System.Drawing.Size(1360, 546);
            this.tab_imar.TabIndex = 4;
            this.tab_imar.Text = "İmar Analizleri";
            this.tab_imar.UseVisualStyleBackColor = true;
            // 
            // checkBox_imar_15
            // 
            this.checkBox_imar_15.AutoSize = true;
            this.checkBox_imar_15.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_imar_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_15.Location = new System.Drawing.Point(4, 487);
            this.checkBox_imar_15.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_15.Name = "checkBox_imar_15";
            this.checkBox_imar_15.Size = new System.Drawing.Size(133, 21);
            this.checkBox_imar_15.TabIndex = 60;
            this.checkBox_imar_15.Text = "checkBox_imar_15";
            this.checkBox_imar_15.UseVisualStyleBackColor = false;
            this.checkBox_imar_15.Visible = false;
            // 
            // katmanlar_right_click
            // 
            this.katmanlar_right_click.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.katmanlar_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabloyuGörToolStripMenuItem,
            this.rengiDeğiştirToolStripMenuItem,
            this.temizleToolStripMenuItem,
            this.yenidenAdlandırToolStripMenuItem,
            this.kaydetToolStripMenuItem});
            this.katmanlar_right_click.Name = "katmanlar_right_click";
            this.katmanlar_right_click.Size = new System.Drawing.Size(169, 134);
            this.katmanlar_right_click.Opening += new System.ComponentModel.CancelEventHandler(this.katmanlar_right_click_Opening);
            // 
            // tabloyuGörToolStripMenuItem
            // 
            this.tabloyuGörToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabloyuGörToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tabloyuGörToolStripMenuItem.Image")));
            this.tabloyuGörToolStripMenuItem.Name = "tabloyuGörToolStripMenuItem";
            this.tabloyuGörToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.tabloyuGörToolStripMenuItem.Text = "Tabloyu Gör";
            this.tabloyuGörToolStripMenuItem.Click += new System.EventHandler(this.tabloyuGörToolStripMenuItem_Click);
            // 
            // rengiDeğiştirToolStripMenuItem
            // 
            this.rengiDeğiştirToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rengiDeğiştirToolStripMenuItem.Image")));
            this.rengiDeğiştirToolStripMenuItem.Name = "rengiDeğiştirToolStripMenuItem";
            this.rengiDeğiştirToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.rengiDeğiştirToolStripMenuItem.Text = "Rengi Değiştir";
            this.rengiDeğiştirToolStripMenuItem.Click += new System.EventHandler(this.rengiDeğiştirToolStripMenuItem_Click);
            // 
            // temizleToolStripMenuItem
            // 
            this.temizleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("temizleToolStripMenuItem.Image")));
            this.temizleToolStripMenuItem.Name = "temizleToolStripMenuItem";
            this.temizleToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.temizleToolStripMenuItem.Text = "Temizle";
            this.temizleToolStripMenuItem.Click += new System.EventHandler(this.temizleToolStripMenuItem_Click);
            // 
            // yenidenAdlandırToolStripMenuItem
            // 
            this.yenidenAdlandırToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("yenidenAdlandırToolStripMenuItem.Image")));
            this.yenidenAdlandırToolStripMenuItem.Name = "yenidenAdlandırToolStripMenuItem";
            this.yenidenAdlandırToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.yenidenAdlandırToolStripMenuItem.Text = "Yeniden Adlandır";
            this.yenidenAdlandırToolStripMenuItem.Click += new System.EventHandler(this.yenidenAdlandırToolStripMenuItem_Click);
            // 
            // kaydetToolStripMenuItem
            // 
            this.kaydetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("kaydetToolStripMenuItem.Image")));
            this.kaydetToolStripMenuItem.Name = "kaydetToolStripMenuItem";
            this.kaydetToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.kaydetToolStripMenuItem.Text = "Kaydet";
            this.kaydetToolStripMenuItem.Click += new System.EventHandler(this.kaydetToolStripMenuItem_Click);
            // 
            // checkBox_imar_14
            // 
            this.checkBox_imar_14.AutoSize = true;
            this.checkBox_imar_14.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_imar_14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_14.Location = new System.Drawing.Point(4, 462);
            this.checkBox_imar_14.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_14.Name = "checkBox_imar_14";
            this.checkBox_imar_14.Size = new System.Drawing.Size(133, 21);
            this.checkBox_imar_14.TabIndex = 59;
            this.checkBox_imar_14.Text = "checkBox_imar_14";
            this.checkBox_imar_14.UseVisualStyleBackColor = false;
            this.checkBox_imar_14.Visible = false;
            // 
            // panel_imar
            // 
            this.panel_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_imar.AutoSize = true;
            this.panel_imar.Controls.Add(this.buton_imar_katmanlar);
            this.panel_imar.Controls.Add(this.webView_imar);
            this.panel_imar.Controls.Add(this.gMapControl_imar);
            this.panel_imar.Controls.Add(this.mesafe_metre_imar);
            this.panel_imar.Controls.Add(this.Mesafe_imar);
            this.panel_imar.Location = new System.Drawing.Point(187, 56);
            this.panel_imar.Margin = new System.Windows.Forms.Padding(2);
            this.panel_imar.Name = "panel_imar";
            this.panel_imar.Size = new System.Drawing.Size(951, 4152);
            this.panel_imar.TabIndex = 58;
            // 
            // buton_imar_katmanlar
            // 
            this.buton_imar_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_imar_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_imar_katmanlar.BackgroundImage")));
            this.buton_imar_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_imar_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_imar_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_imar_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_imar_katmanlar.Location = new System.Drawing.Point(2, 3980);
            this.buton_imar_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_imar_katmanlar.Name = "buton_imar_katmanlar";
            this.buton_imar_katmanlar.Size = new System.Drawing.Size(46, 43);
            this.buton_imar_katmanlar.TabIndex = 59;
            this.buton_imar_katmanlar.UseVisualStyleBackColor = true;
            // 
            // harita_katmanları_right_click
            // 
            this.harita_katmanları_right_click.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.harita_katmanları_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Arazi,
            this.Google_Earth,
            this.Google_Earth_Desktop,
            this.Harita,
            this.OSM,
            this.Sokak_Görünümü,
            this.Uydu});
            this.harita_katmanları_right_click.Name = "harita_katmanları_right_click";
            this.harita_katmanları_right_click.Size = new System.Drawing.Size(171, 186);
            // 
            // Arazi
            // 
            this.Arazi.Image = ((System.Drawing.Image)(resources.GetObject("Arazi.Image")));
            this.Arazi.Name = "Arazi";
            this.Arazi.Size = new System.Drawing.Size(170, 26);
            this.Arazi.Text = "Arazi";
            this.Arazi.Click += new System.EventHandler(this.Arazi_Click);
            // 
            // Google_Earth
            // 
            this.Google_Earth.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth.Image")));
            this.Google_Earth.Name = "Google_Earth";
            this.Google_Earth.Size = new System.Drawing.Size(170, 26);
            this.Google_Earth.Text = "GE Online";
            this.Google_Earth.Click += new System.EventHandler(this.Google_Earth_Click);
            // 
            // Google_Earth_Desktop
            // 
            this.Google_Earth_Desktop.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth_Desktop.Image")));
            this.Google_Earth_Desktop.Name = "Google_Earth_Desktop";
            this.Google_Earth_Desktop.Size = new System.Drawing.Size(170, 26);
            this.Google_Earth_Desktop.Text = "GE Pro Desktop";
            this.Google_Earth_Desktop.Click += new System.EventHandler(this.Google_Earth_Desktop_Click);
            // 
            // Harita
            // 
            this.Harita.Image = ((System.Drawing.Image)(resources.GetObject("Harita.Image")));
            this.Harita.Name = "Harita";
            this.Harita.Size = new System.Drawing.Size(170, 26);
            this.Harita.Text = "Harita";
            this.Harita.Click += new System.EventHandler(this.Harita_Click);
            // 
            // OSM
            // 
            this.OSM.Image = ((System.Drawing.Image)(resources.GetObject("OSM.Image")));
            this.OSM.Name = "OSM";
            this.OSM.Size = new System.Drawing.Size(170, 26);
            this.OSM.Text = "Open Street Map";
            this.OSM.Click += new System.EventHandler(this.OSM_Click);
            // 
            // Sokak_Görünümü
            // 
            this.Sokak_Görünümü.Image = ((System.Drawing.Image)(resources.GetObject("Sokak_Görünümü.Image")));
            this.Sokak_Görünümü.Name = "Sokak_Görünümü";
            this.Sokak_Görünümü.Size = new System.Drawing.Size(170, 26);
            this.Sokak_Görünümü.Text = "Sokak Görünümü";
            this.Sokak_Görünümü.Click += new System.EventHandler(this.Sokak_Görünümü_Click);
            // 
            // Uydu
            // 
            this.Uydu.Image = ((System.Drawing.Image)(resources.GetObject("Uydu.Image")));
            this.Uydu.Name = "Uydu";
            this.Uydu.Size = new System.Drawing.Size(170, 26);
            this.Uydu.Text = "Uydu";
            this.Uydu.Click += new System.EventHandler(this.Uydu_Click);
            // 
            // webView_imar
            // 
            this.webView_imar.AllowExternalDrop = true;
            this.webView_imar.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_imar.CreationProperties = null;
            this.webView_imar.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_imar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_imar.Location = new System.Drawing.Point(0, 0);
            this.webView_imar.Margin = new System.Windows.Forms.Padding(2);
            this.webView_imar.Name = "webView_imar";
            this.webView_imar.Size = new System.Drawing.Size(951, 4152);
            this.webView_imar.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_imar.TabIndex = 38;
            this.webView_imar.Visible = false;
            this.webView_imar.ZoomFactor = 1D;
            // 
            // gMapControl_imar
            // 
            this.gMapControl_imar.AllowDrop = true;
            this.gMapControl_imar.Bearing = 0F;
            this.gMapControl_imar.CanDragMap = true;
            this.gMapControl_imar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_imar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_imar.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_imar.GrayScaleMode = false;
            this.gMapControl_imar.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_imar.LevelsKeepInMemory = 5;
            this.gMapControl_imar.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_imar.Margin = new System.Windows.Forms.Padding(2);
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
            this.gMapControl_imar.Size = new System.Drawing.Size(951, 4152);
            this.gMapControl_imar.TabIndex = 35;
            this.gMapControl_imar.Zoom = 0D;
            this.gMapControl_imar.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_imar_OnMapClick);
            this.gMapControl_imar.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_imar_OnMapDoubleClick);
            this.gMapControl_imar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseDown);
            this.gMapControl_imar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseMove);
            this.gMapControl_imar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseUp);
            // 
            // mesafe_metre_imar
            // 
            this.mesafe_metre_imar.AutoSize = true;
            this.mesafe_metre_imar.Location = new System.Drawing.Point(93, 8);
            this.mesafe_metre_imar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_imar.Name = "mesafe_metre_imar";
            this.mesafe_metre_imar.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_imar.TabIndex = 56;
            this.mesafe_metre_imar.Visible = false;
            // 
            // Mesafe_imar
            // 
            this.Mesafe_imar.AutoSize = true;
            this.Mesafe_imar.Location = new System.Drawing.Point(2, 0);
            this.Mesafe_imar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_imar.Name = "Mesafe_imar";
            this.Mesafe_imar.Size = new System.Drawing.Size(54, 17);
            this.Mesafe_imar.TabIndex = 57;
            this.Mesafe_imar.Text = "Mesafe:";
            this.Mesafe_imar.Visible = false;
            // 
            // checkBox_imar_1
            // 
            this.checkBox_imar_1.AutoSize = true;
            this.checkBox_imar_1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_1.Location = new System.Drawing.Point(4, 135);
            this.checkBox_imar_1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_1.Name = "checkBox_imar_1";
            this.checkBox_imar_1.Size = new System.Drawing.Size(126, 21);
            this.checkBox_imar_1.TabIndex = 55;
            this.checkBox_imar_1.Text = "checkBox_imar_1";
            this.checkBox_imar_1.UseVisualStyleBackColor = true;
            this.checkBox_imar_1.Visible = false;
            // 
            // checkBox_imar_2
            // 
            this.checkBox_imar_2.AutoSize = true;
            this.checkBox_imar_2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_2.Location = new System.Drawing.Point(4, 160);
            this.checkBox_imar_2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_2.Name = "checkBox_imar_2";
            this.checkBox_imar_2.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_2.TabIndex = 54;
            this.checkBox_imar_2.Text = "checkBox_imar_2";
            this.checkBox_imar_2.UseVisualStyleBackColor = true;
            this.checkBox_imar_2.Visible = false;
            // 
            // checkBox_imar_3
            // 
            this.checkBox_imar_3.AutoSize = true;
            this.checkBox_imar_3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_3.Location = new System.Drawing.Point(4, 185);
            this.checkBox_imar_3.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_3.Name = "checkBox_imar_3";
            this.checkBox_imar_3.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_3.TabIndex = 53;
            this.checkBox_imar_3.Text = "checkBox_imar_3";
            this.checkBox_imar_3.UseVisualStyleBackColor = true;
            this.checkBox_imar_3.Visible = false;
            // 
            // checkBox_imar_4
            // 
            this.checkBox_imar_4.AutoSize = true;
            this.checkBox_imar_4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_4.Location = new System.Drawing.Point(4, 210);
            this.checkBox_imar_4.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_4.Name = "checkBox_imar_4";
            this.checkBox_imar_4.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_4.TabIndex = 52;
            this.checkBox_imar_4.Text = "checkBox_imar_4";
            this.checkBox_imar_4.UseVisualStyleBackColor = true;
            this.checkBox_imar_4.Visible = false;
            // 
            // checkBox_imar_5
            // 
            this.checkBox_imar_5.AutoSize = true;
            this.checkBox_imar_5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_5.Location = new System.Drawing.Point(4, 236);
            this.checkBox_imar_5.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_5.Name = "checkBox_imar_5";
            this.checkBox_imar_5.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_5.TabIndex = 51;
            this.checkBox_imar_5.Text = "checkBox_imar_5";
            this.checkBox_imar_5.UseVisualStyleBackColor = true;
            this.checkBox_imar_5.Visible = false;
            // 
            // checkBox_imar_6
            // 
            this.checkBox_imar_6.AutoSize = true;
            this.checkBox_imar_6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_6.Location = new System.Drawing.Point(4, 261);
            this.checkBox_imar_6.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_6.Name = "checkBox_imar_6";
            this.checkBox_imar_6.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_6.TabIndex = 50;
            this.checkBox_imar_6.Text = "checkBox_imar_6";
            this.checkBox_imar_6.UseVisualStyleBackColor = true;
            this.checkBox_imar_6.Visible = false;
            // 
            // checkBox_imar_7
            // 
            this.checkBox_imar_7.AutoSize = true;
            this.checkBox_imar_7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_7.Location = new System.Drawing.Point(4, 286);
            this.checkBox_imar_7.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_7.Name = "checkBox_imar_7";
            this.checkBox_imar_7.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_7.TabIndex = 49;
            this.checkBox_imar_7.Text = "checkBox_imar_7";
            this.checkBox_imar_7.UseVisualStyleBackColor = true;
            this.checkBox_imar_7.Visible = false;
            // 
            // checkBox_imar_8
            // 
            this.checkBox_imar_8.AutoSize = true;
            this.checkBox_imar_8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_8.Location = new System.Drawing.Point(4, 311);
            this.checkBox_imar_8.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_8.Name = "checkBox_imar_8";
            this.checkBox_imar_8.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_8.TabIndex = 48;
            this.checkBox_imar_8.Text = "checkBox_imar_8";
            this.checkBox_imar_8.UseVisualStyleBackColor = true;
            this.checkBox_imar_8.Visible = false;
            // 
            // checkBox_imar_9
            // 
            this.checkBox_imar_9.AutoSize = true;
            this.checkBox_imar_9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_9.Location = new System.Drawing.Point(4, 336);
            this.checkBox_imar_9.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_9.Name = "checkBox_imar_9";
            this.checkBox_imar_9.Size = new System.Drawing.Size(128, 21);
            this.checkBox_imar_9.TabIndex = 47;
            this.checkBox_imar_9.Text = "checkBox_imar_9";
            this.checkBox_imar_9.UseVisualStyleBackColor = true;
            this.checkBox_imar_9.Visible = false;
            // 
            // checkBox_imar_10
            // 
            this.checkBox_imar_10.AutoSize = true;
            this.checkBox_imar_10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_10.Location = new System.Drawing.Point(4, 362);
            this.checkBox_imar_10.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_10.Name = "checkBox_imar_10";
            this.checkBox_imar_10.Size = new System.Drawing.Size(133, 21);
            this.checkBox_imar_10.TabIndex = 46;
            this.checkBox_imar_10.Text = "checkBox_imar_10";
            this.checkBox_imar_10.UseVisualStyleBackColor = true;
            this.checkBox_imar_10.Visible = false;
            // 
            // checkBox_imar_11
            // 
            this.checkBox_imar_11.AutoSize = true;
            this.checkBox_imar_11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_11.Location = new System.Drawing.Point(4, 387);
            this.checkBox_imar_11.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_11.Name = "checkBox_imar_11";
            this.checkBox_imar_11.Size = new System.Drawing.Size(131, 21);
            this.checkBox_imar_11.TabIndex = 45;
            this.checkBox_imar_11.Text = "checkBox_imar_11";
            this.checkBox_imar_11.UseVisualStyleBackColor = true;
            this.checkBox_imar_11.Visible = false;
            // 
            // checkBox_imar_12
            // 
            this.checkBox_imar_12.AutoSize = true;
            this.checkBox_imar_12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_imar_12.Location = new System.Drawing.Point(4, 412);
            this.checkBox_imar_12.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_12.Name = "checkBox_imar_12";
            this.checkBox_imar_12.Size = new System.Drawing.Size(133, 21);
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
            this.checkBox_imar_13.Location = new System.Drawing.Point(4, 437);
            this.checkBox_imar_13.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_imar_13.Name = "checkBox_imar_13";
            this.checkBox_imar_13.Size = new System.Drawing.Size(133, 21);
            this.checkBox_imar_13.TabIndex = 43;
            this.checkBox_imar_13.Text = "checkBox_imar_13";
            this.checkBox_imar_13.UseVisualStyleBackColor = false;
            this.checkBox_imar_13.Visible = false;
            // 
            // label_imar_katmanlar
            // 
            this.label_imar_katmanlar.AutoSize = true;
            this.label_imar_katmanlar.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_imar_katmanlar.Location = new System.Drawing.Point(16, 106);
            this.label_imar_katmanlar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_imar_katmanlar.Name = "label_imar_katmanlar";
            this.label_imar_katmanlar.Size = new System.Drawing.Size(81, 19);
            this.label_imar_katmanlar.TabIndex = 42;
            this.label_imar_katmanlar.Text = "Katmanlar";
            // 
            // imar_dosya_seçimi
            // 
            this.imar_dosya_seçimi.Location = new System.Drawing.Point(2, 35);
            this.imar_dosya_seçimi.Margin = new System.Windows.Forms.Padding(2);
            this.imar_dosya_seçimi.Name = "imar_dosya_seçimi";
            this.imar_dosya_seçimi.Size = new System.Drawing.Size(130, 36);
            this.imar_dosya_seçimi.TabIndex = 41;
            this.imar_dosya_seçimi.Text = "Dosya Seç";
            this.imar_dosya_seçimi.UseVisualStyleBackColor = true;
            this.imar_dosya_seçimi.Click += new System.EventHandler(this.imar_dosya_seçimi_Click);
            // 
            // toolStrip_imar
            // 
            this.toolStrip_imar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip_imar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.İmar_Seç,
            this.toolStripSeparator9,
            this.İmar_Kaydır,
            this.toolStripSeparator10,
            this.İmar_Mesafe_Ölç,
            this.toolStripSeparator11,
            this.İmar_Poligon,
            this.toolStripSeparator12,
            this.İmar_Nokta,
            this.toolStripSeparator13,
            this.İmar_Grid_Oluştur,
            this.toolStripSeparator14,
            this.İmar_Fonksiyonlar});
            this.toolStrip_imar.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_imar.Name = "toolStrip_imar";
            this.toolStrip_imar.Size = new System.Drawing.Size(1360, 27);
            this.toolStrip_imar.TabIndex = 40;
            this.toolStrip_imar.Text = "toolStrip1";
            // 
            // İmar_Seç
            // 
            this.İmar_Seç.AccessibleDescription = "";
            this.İmar_Seç.AccessibleName = "";
            this.İmar_Seç.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(250)))), ((int)(((byte)(249)))));
            this.İmar_Seç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Seç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Seç.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Seç.Image")));
            this.İmar_Seç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Seç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Seç.Margin = new System.Windows.Forms.Padding(300, 1, 0, 2);
            this.İmar_Seç.Name = "İmar_Seç";
            this.İmar_Seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Seç.Size = new System.Drawing.Size(66, 24);
            this.İmar_Seç.Tag = "";
            this.İmar_Seç.Text = "Seç";
            this.İmar_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.İmar_Seç.Click += new System.EventHandler(this.İmar_Seç_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 27);
            // 
            // İmar_Kaydır
            // 
            this.İmar_Kaydır.AccessibleDescription = "";
            this.İmar_Kaydır.AccessibleName = "";
            this.İmar_Kaydır.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.İmar_Kaydır.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Kaydır.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Kaydır.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Kaydır.Image")));
            this.İmar_Kaydır.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Kaydır.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Kaydır.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Kaydır.Name = "İmar_Kaydır";
            this.İmar_Kaydır.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Kaydır.Size = new System.Drawing.Size(85, 24);
            this.İmar_Kaydır.Tag = "";
            this.İmar_Kaydır.Text = "Kaydır";
            this.İmar_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            this.İmar_Kaydır.Click += new System.EventHandler(this.İmar_Kaydır_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 27);
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
            this.İmar_Mesafe_Ölç.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Mesafe_Ölç.Name = "İmar_Mesafe_Ölç";
            this.İmar_Mesafe_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Mesafe_Ölç.Size = new System.Drawing.Size(117, 24);
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
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 27);
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
            this.İmar_Poligon.Size = new System.Drawing.Size(93, 24);
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
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 27);
            // 
            // İmar_Nokta
            // 
            this.İmar_Nokta.AccessibleDescription = "";
            this.İmar_Nokta.AccessibleName = "";
            this.İmar_Nokta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.İmar_Nokta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Nokta.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Nokta.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Nokta.Image")));
            this.İmar_Nokta.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Nokta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Nokta.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Nokta.Name = "İmar_Nokta";
            this.İmar_Nokta.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Nokta.Size = new System.Drawing.Size(83, 24);
            this.İmar_Nokta.Tag = "";
            this.İmar_Nokta.Text = "Nokta";
            this.İmar_Nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 27);
            // 
            // İmar_Grid_Oluştur
            // 
            this.İmar_Grid_Oluştur.AccessibleDescription = "";
            this.İmar_Grid_Oluştur.AccessibleName = "";
            this.İmar_Grid_Oluştur.BackColor = System.Drawing.Color.Beige;
            this.İmar_Grid_Oluştur.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.İmar_Grid_Oluştur.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.İmar_Grid_Oluştur.Image = ((System.Drawing.Image)(resources.GetObject("İmar_Grid_Oluştur.Image")));
            this.İmar_Grid_Oluştur.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.İmar_Grid_Oluştur.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.İmar_Grid_Oluştur.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.İmar_Grid_Oluştur.Name = "İmar_Grid_Oluştur";
            this.İmar_Grid_Oluştur.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.İmar_Grid_Oluştur.Size = new System.Drawing.Size(122, 24);
            this.İmar_Grid_Oluştur.Tag = "";
            this.İmar_Grid_Oluştur.Text = "Grid Oluştur";
            this.İmar_Grid_Oluştur.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Grid_Oluştur.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Grid_Oluştur.ToolTipText = "Belirli bir alan seçilip bu alanda mxn şeklinde bir grid (ızgara) tanımlar.";
            this.İmar_Grid_Oluştur.Click += new System.EventHandler(this.İmar_Grid_Oluştur_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 27);
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
            this.İmar_Fonksiyonlar.Size = new System.Drawing.Size(125, 24);
            this.İmar_Fonksiyonlar.Tag = "";
            this.İmar_Fonksiyonlar.Text = "Fonksiyonlar";
            this.İmar_Fonksiyonlar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Fonksiyonlar.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Fonksiyonlar.ToolTipText = "Çeşitli vektörel veya tabular algoritmaları içerir.";
            this.İmar_Fonksiyonlar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.İmar_Fonksiyonlar_MouseDown);
            // 
            // tab_optDTR
            // 
            this.tab_optDTR.Controls.Add(this.gMapControl_optimalDTR);
            this.tab_optDTR.Controls.Add(this.webView_optimalDTR);
            this.tab_optDTR.Controls.Add(this.checkBox_optDTR_Eskişehir);
            this.tab_optDTR.Controls.Add(this.checkBox_optDTR_İzmir);
            this.tab_optDTR.Location = new System.Drawing.Point(4, 48);
            this.tab_optDTR.Margin = new System.Windows.Forms.Padding(2);
            this.tab_optDTR.Name = "tab_optDTR";
            this.tab_optDTR.Size = new System.Drawing.Size(1360, 546);
            this.tab_optDTR.TabIndex = 7;
            this.tab_optDTR.Text = "Optimal DTR Konumlandırma";
            this.tab_optDTR.UseVisualStyleBackColor = true;
            // 
            // gMapControl_optimalDTR
            // 
            this.gMapControl_optimalDTR.AllowDrop = true;
            this.gMapControl_optimalDTR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_optimalDTR.Bearing = 0F;
            this.gMapControl_optimalDTR.CanDragMap = true;
            this.gMapControl_optimalDTR.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_optimalDTR.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_optimalDTR.GrayScaleMode = false;
            this.gMapControl_optimalDTR.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_optimalDTR.LevelsKeepInMemory = 5;
            this.gMapControl_optimalDTR.Location = new System.Drawing.Point(2, 0);
            this.gMapControl_optimalDTR.Margin = new System.Windows.Forms.Padding(2);
            this.gMapControl_optimalDTR.MarkersEnabled = true;
            this.gMapControl_optimalDTR.MaxZoom = 2;
            this.gMapControl_optimalDTR.MinZoom = 2;
            this.gMapControl_optimalDTR.MouseWheelZoomEnabled = true;
            this.gMapControl_optimalDTR.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_optimalDTR.Name = "gMapControl_optimalDTR";
            this.gMapControl_optimalDTR.NegativeMode = false;
            this.gMapControl_optimalDTR.PolygonsEnabled = true;
            this.gMapControl_optimalDTR.RetryLoadTile = 0;
            this.gMapControl_optimalDTR.RoutesEnabled = true;
            this.gMapControl_optimalDTR.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_optimalDTR.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_optimalDTR.ShowTileGridLines = false;
            this.gMapControl_optimalDTR.Size = new System.Drawing.Size(802, 3112);
            this.gMapControl_optimalDTR.TabIndex = 42;
            this.gMapControl_optimalDTR.Zoom = 0D;
            // 
            // webView_optimalDTR
            // 
            this.webView_optimalDTR.AllowExternalDrop = true;
            this.webView_optimalDTR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView_optimalDTR.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_optimalDTR.CreationProperties = null;
            this.webView_optimalDTR.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_optimalDTR.Location = new System.Drawing.Point(2, 2);
            this.webView_optimalDTR.Margin = new System.Windows.Forms.Padding(2);
            this.webView_optimalDTR.Name = "webView_optimalDTR";
            this.webView_optimalDTR.Size = new System.Drawing.Size(802, 3112);
            this.webView_optimalDTR.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_optimalDTR.TabIndex = 40;
            this.webView_optimalDTR.Visible = false;
            this.webView_optimalDTR.ZoomFactor = 1D;
            // 
            // checkBox_optDTR_Eskişehir
            // 
            this.checkBox_optDTR_Eskişehir.AutoSize = true;
            this.checkBox_optDTR_Eskişehir.Location = new System.Drawing.Point(859, 247);
            this.checkBox_optDTR_Eskişehir.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_optDTR_Eskişehir.Name = "checkBox_optDTR_Eskişehir";
            this.checkBox_optDTR_Eskişehir.Size = new System.Drawing.Size(79, 21);
            this.checkBox_optDTR_Eskişehir.TabIndex = 22;
            this.checkBox_optDTR_Eskişehir.Text = "Eskişehir";
            this.checkBox_optDTR_Eskişehir.UseVisualStyleBackColor = true;
            // 
            // checkBox_optDTR_İzmir
            // 
            this.checkBox_optDTR_İzmir.AutoSize = true;
            this.checkBox_optDTR_İzmir.Location = new System.Drawing.Point(859, 225);
            this.checkBox_optDTR_İzmir.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_optDTR_İzmir.Name = "checkBox_optDTR_İzmir";
            this.checkBox_optDTR_İzmir.Size = new System.Drawing.Size(57, 21);
            this.checkBox_optDTR_İzmir.TabIndex = 21;
            this.checkBox_optDTR_İzmir.Text = "İzmir";
            this.checkBox_optDTR_İzmir.UseVisualStyleBackColor = true;
            // 
            // tab_senaryo
            // 
            this.tab_senaryo.Controls.Add(this.SenaryoModulePanel);
            this.tab_senaryo.Location = new System.Drawing.Point(4, 48);
            this.tab_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tab_senaryo.Name = "tab_senaryo";
            this.tab_senaryo.Size = new System.Drawing.Size(1360, 546);
            this.tab_senaryo.TabIndex = 3;
            this.tab_senaryo.Text = "Senaryo Oluşturma Modülü";
            this.tab_senaryo.UseVisualStyleBackColor = true;
            // 
            // SenaryoModulePanel
            // 
            this.SenaryoModulePanel.Controls.Add(this.EkonometrikSenaryoElementsPanel);
            this.SenaryoModulePanel.Controls.Add(this.SenaryoModuleTabControl);
            this.SenaryoModulePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModulePanel.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModulePanel.Margin = new System.Windows.Forms.Padding(2);
            this.SenaryoModulePanel.Name = "SenaryoModulePanel";
            this.SenaryoModulePanel.Size = new System.Drawing.Size(1360, 546);
            this.SenaryoModulePanel.TabIndex = 0;
            // 
            // EkonometrikSenaryoElementsPanel
            // 
            this.EkonometrikSenaryoElementsPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.RModelProgressBar);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ShowResultsButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.RModelStatusLabel);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFPredictionShowResultsButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFScenerioSaveButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.richTextBox_senaryolar_ELF);
            this.EkonometrikSenaryoElementsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EkonometrikSenaryoElementsPanel.Location = new System.Drawing.Point(1121, 0);
            this.EkonometrikSenaryoElementsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.EkonometrikSenaryoElementsPanel.Name = "EkonometrikSenaryoElementsPanel";
            this.EkonometrikSenaryoElementsPanel.Size = new System.Drawing.Size(239, 546);
            this.EkonometrikSenaryoElementsPanel.TabIndex = 1;
            // 
            // RModelProgressBar
            // 
            this.RModelProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.RModelProgressBar.Location = new System.Drawing.Point(42, 295);
            this.RModelProgressBar.MarqueeAnimationSpeed = 200;
            this.RModelProgressBar.Name = "RModelProgressBar";
            this.RModelProgressBar.Size = new System.Drawing.Size(170, 22);
            this.RModelProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.RModelProgressBar.TabIndex = 62;
            this.RModelProgressBar.Visible = false;
            // 
            // ShowResultsButton
            // 
            this.ShowResultsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ShowResultsButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ShowResultsButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.ShowResultsButton.BorderRadius = 0;
            this.ShowResultsButton.BorderSize = 0;
            this.ShowResultsButton.FlatAppearance.BorderSize = 0;
            this.ShowResultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowResultsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ShowResultsButton.ForeColor = System.Drawing.Color.White;
            this.ShowResultsButton.Location = new System.Drawing.Point(44, 380);
            this.ShowResultsButton.Name = "ShowResultsButton";
            this.ShowResultsButton.Size = new System.Drawing.Size(150, 40);
            this.ShowResultsButton.TabIndex = 14;
            this.ShowResultsButton.Text = " Sonuçları Göster";
            this.ShowResultsButton.TextColor = System.Drawing.Color.White;
            this.ShowResultsButton.UseVisualStyleBackColor = false;
            this.ShowResultsButton.Visible = false;
            this.ShowResultsButton.Click += new System.EventHandler(this.ShowResultsButton_Click);
            // 
            // RModelStatusLabel
            // 
            this.RModelStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RModelStatusLabel.AutoSize = true;
            this.RModelStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.RModelStatusLabel.Location = new System.Drawing.Point(5, 208);
            this.RModelStatusLabel.Name = "RModelStatusLabel";
            this.RModelStatusLabel.Size = new System.Drawing.Size(42, 13);
            this.RModelStatusLabel.TabIndex = 61;
            this.RModelStatusLabel.Text = "Status:";
            this.RModelStatusLabel.Visible = false;
            // 
            // ELFPredictionShowResultsButton
            // 
            this.ELFPredictionShowResultsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFPredictionShowResultsButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFPredictionShowResultsButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.ELFPredictionShowResultsButton.BorderRadius = 0;
            this.ELFPredictionShowResultsButton.BorderSize = 0;
            this.ELFPredictionShowResultsButton.FlatAppearance.BorderSize = 0;
            this.ELFPredictionShowResultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ELFPredictionShowResultsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ELFPredictionShowResultsButton.ForeColor = System.Drawing.Color.White;
            this.ELFPredictionShowResultsButton.Location = new System.Drawing.Point(42, 305);
            this.ELFPredictionShowResultsButton.Name = "ELFPredictionShowResultsButton";
            this.ELFPredictionShowResultsButton.Size = new System.Drawing.Size(150, 40);
            this.ELFPredictionShowResultsButton.TabIndex = 13;
            this.ELFPredictionShowResultsButton.Text = "Tahmin Yap/ Sonuçlarını Göster";
            this.ELFPredictionShowResultsButton.TextColor = System.Drawing.Color.White;
            this.ELFPredictionShowResultsButton.UseVisualStyleBackColor = false;
            this.ELFPredictionShowResultsButton.Click += new System.EventHandler(this.ELFPredictionShowResultsButton_Click);
            // 
            // ELFScenerioSaveButton
            // 
            this.ELFScenerioSaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFScenerioSaveButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFScenerioSaveButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.ELFScenerioSaveButton.BorderRadius = 0;
            this.ELFScenerioSaveButton.BorderSize = 0;
            this.ELFScenerioSaveButton.FlatAppearance.BorderSize = 0;
            this.ELFScenerioSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ELFScenerioSaveButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ELFScenerioSaveButton.ForeColor = System.Drawing.Color.White;
            this.ELFScenerioSaveButton.Location = new System.Drawing.Point(42, 146);
            this.ELFScenerioSaveButton.Name = "ELFScenerioSaveButton";
            this.ELFScenerioSaveButton.Size = new System.Drawing.Size(150, 40);
            this.ELFScenerioSaveButton.TabIndex = 12;
            this.ELFScenerioSaveButton.Text = "Senaryo Değişikliklerini Kaydet";
            this.ELFScenerioSaveButton.TextColor = System.Drawing.Color.White;
            this.ELFScenerioSaveButton.UseVisualStyleBackColor = false;
            this.ELFScenerioSaveButton.Click += new System.EventHandler(this.ELFScenerioSaveButton_Click);
            // 
            // richTextBox_senaryolar_ELF
            // 
            this.richTextBox_senaryolar_ELF.BackColor = System.Drawing.Color.NavajoWhite;
            this.richTextBox_senaryolar_ELF.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_senaryolar_ELF.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox_senaryolar_ELF.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.richTextBox_senaryolar_ELF.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_senaryolar_ELF.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox_senaryolar_ELF.Name = "richTextBox_senaryolar_ELF";
            this.richTextBox_senaryolar_ELF.Size = new System.Drawing.Size(239, 87);
            this.richTextBox_senaryolar_ELF.TabIndex = 10;
            this.richTextBox_senaryolar_ELF.Text = "Tablolar üzerinde değişiklik yaparak senaryo üretebilirsiniz. Yeni senaryo tahmin" +
    " sonuçlarını görüntülemek için lütfen önce değişiklikleri kaydedin.";
            // 
            // SenaryoModuleTabControl
            // 
            this.SenaryoModuleTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.SenaryoModuleTabControl.Controls.Add(this.EkonometrikSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.YeniGenislemeSenaryoTabPage);
            this.SenaryoModuleTabControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.SenaryoModuleTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModuleTabControl.ItemSize = new System.Drawing.Size(220, 40);
            this.SenaryoModuleTabControl.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModuleTabControl.Margin = new System.Windows.Forms.Padding(2);
            this.SenaryoModuleTabControl.Name = "SenaryoModuleTabControl";
            this.SenaryoModuleTabControl.SelectedIndex = 0;
            this.SenaryoModuleTabControl.Size = new System.Drawing.Size(1360, 546);
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
            // 
            // EkonometrikSenaryoTabPage
            // 
            this.EkonometrikSenaryoTabPage.Controls.Add(this.EkonometrikSenaryoOutputsPanel);
            this.EkonometrikSenaryoTabPage.Location = new System.Drawing.Point(224, 4);
            this.EkonometrikSenaryoTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.EkonometrikSenaryoTabPage.Name = "EkonometrikSenaryoTabPage";
            this.EkonometrikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.EkonometrikSenaryoTabPage.Size = new System.Drawing.Size(1132, 538);
            this.EkonometrikSenaryoTabPage.TabIndex = 0;
            this.EkonometrikSenaryoTabPage.Text = "Ekonometrik Senaryolar";
            this.EkonometrikSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // EkonometrikSenaryoOutputsPanel
            // 
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.ELFSenaryoTabControls);
            this.EkonometrikSenaryoOutputsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EkonometrikSenaryoOutputsPanel.Location = new System.Drawing.Point(2, 2);
            this.EkonometrikSenaryoOutputsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.EkonometrikSenaryoOutputsPanel.Name = "EkonometrikSenaryoOutputsPanel";
            this.EkonometrikSenaryoOutputsPanel.Size = new System.Drawing.Size(1128, 534);
            this.EkonometrikSenaryoOutputsPanel.TabIndex = 0;
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
            this.ELFSenaryoTabControls.Location = new System.Drawing.Point(6, 2);
            this.ELFSenaryoTabControls.Margin = new System.Windows.Forms.Padding(2);
            this.ELFSenaryoTabControls.Name = "ELFSenaryoTabControls";
            this.ELFSenaryoTabControls.SelectedIndex = 0;
            this.ELFSenaryoTabControls.Size = new System.Drawing.Size(936, 860);
            this.ELFSenaryoTabControls.TabIndex = 2;
            // 
            // tabPage_min_senaryo
            // 
            this.tabPage_min_senaryo.Controls.Add(this.ELFMinSenaryoTable);
            this.tabPage_min_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_min_senaryo.Location = new System.Drawing.Point(4, 26);
            this.tabPage_min_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_min_senaryo.Name = "tabPage_min_senaryo";
            this.tabPage_min_senaryo.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_min_senaryo.Size = new System.Drawing.Size(928, 830);
            this.tabPage_min_senaryo.TabIndex = 0;
            this.tabPage_min_senaryo.Text = "Minimum Senaryo";
            this.tabPage_min_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFMinSenaryoTable
            // 
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFMinSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.ELFMinSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFMinSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMinSenaryoTable.DefaultCellStyle = dataGridViewCellStyle6;
            this.ELFMinSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMinSenaryoTable.Location = new System.Drawing.Point(2, 2);
            this.ELFMinSenaryoTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMinSenaryoTable.Name = "ELFMinSenaryoTable";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFMinSenaryoTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.ELFMinSenaryoTable.RowHeadersWidth = 18;
            this.ELFMinSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMinSenaryoTable.Size = new System.Drawing.Size(924, 826);
            this.ELFMinSenaryoTable.TabIndex = 0;
            // 
            // tabPage_dusuk_senaryo
            // 
            this.tabPage_dusuk_senaryo.Controls.Add(this.ELFLowSenaryoTable);
            this.tabPage_dusuk_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_dusuk_senaryo.Location = new System.Drawing.Point(4, 26);
            this.tabPage_dusuk_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_dusuk_senaryo.Name = "tabPage_dusuk_senaryo";
            this.tabPage_dusuk_senaryo.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_dusuk_senaryo.Size = new System.Drawing.Size(928, 830);
            this.tabPage_dusuk_senaryo.TabIndex = 1;
            this.tabPage_dusuk_senaryo.Text = "Düşük Senaryo";
            this.tabPage_dusuk_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFLowSenaryoTable
            // 
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFLowSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.ELFLowSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFLowSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFLowSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFLowSenaryoTable.Location = new System.Drawing.Point(2, 2);
            this.ELFLowSenaryoTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFLowSenaryoTable.Name = "ELFLowSenaryoTable";
            this.ELFLowSenaryoTable.RowHeadersWidth = 18;
            this.ELFLowSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFLowSenaryoTable.Size = new System.Drawing.Size(924, 826);
            this.ELFLowSenaryoTable.TabIndex = 1;
            // 
            // tabPage_baz_senaryo
            // 
            this.tabPage_baz_senaryo.Controls.Add(this.ELFBaseSenaryoTable);
            this.tabPage_baz_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_baz_senaryo.Location = new System.Drawing.Point(4, 26);
            this.tabPage_baz_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_baz_senaryo.Name = "tabPage_baz_senaryo";
            this.tabPage_baz_senaryo.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_baz_senaryo.Size = new System.Drawing.Size(928, 830);
            this.tabPage_baz_senaryo.TabIndex = 2;
            this.tabPage_baz_senaryo.Text = "Baz Senaryo";
            this.tabPage_baz_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFBaseSenaryoTable
            // 
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFBaseSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.ELFBaseSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFBaseSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBaseSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFBaseSenaryoTable.DefaultCellStyle = dataGridViewCellStyle10;
            this.ELFBaseSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBaseSenaryoTable.Location = new System.Drawing.Point(2, 2);
            this.ELFBaseSenaryoTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFBaseSenaryoTable.Name = "ELFBaseSenaryoTable";
            this.ELFBaseSenaryoTable.RowHeadersWidth = 18;
            this.ELFBaseSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFBaseSenaryoTable.Size = new System.Drawing.Size(924, 826);
            this.ELFBaseSenaryoTable.TabIndex = 1;
            // 
            // tabPage_yuksek_senaryo
            // 
            this.tabPage_yuksek_senaryo.Controls.Add(this.ELFHighSenaryoTable);
            this.tabPage_yuksek_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_yuksek_senaryo.Location = new System.Drawing.Point(4, 26);
            this.tabPage_yuksek_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_yuksek_senaryo.Name = "tabPage_yuksek_senaryo";
            this.tabPage_yuksek_senaryo.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_yuksek_senaryo.Size = new System.Drawing.Size(928, 830);
            this.tabPage_yuksek_senaryo.TabIndex = 3;
            this.tabPage_yuksek_senaryo.Text = "Yüksek Senaryo";
            this.tabPage_yuksek_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFHighSenaryoTable
            // 
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFHighSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            this.ELFHighSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFHighSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFHighSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFHighSenaryoTable.DefaultCellStyle = dataGridViewCellStyle12;
            this.ELFHighSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFHighSenaryoTable.Location = new System.Drawing.Point(2, 2);
            this.ELFHighSenaryoTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFHighSenaryoTable.Name = "ELFHighSenaryoTable";
            this.ELFHighSenaryoTable.RowHeadersWidth = 51;
            this.ELFHighSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFHighSenaryoTable.Size = new System.Drawing.Size(924, 826);
            this.ELFHighSenaryoTable.TabIndex = 1;
            // 
            // tabPage_maks_senaryo
            // 
            this.tabPage_maks_senaryo.Controls.Add(this.ELFMaxSenaryoTable);
            this.tabPage_maks_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_maks_senaryo.Location = new System.Drawing.Point(4, 26);
            this.tabPage_maks_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage_maks_senaryo.Name = "tabPage_maks_senaryo";
            this.tabPage_maks_senaryo.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage_maks_senaryo.Size = new System.Drawing.Size(928, 830);
            this.tabPage_maks_senaryo.TabIndex = 4;
            this.tabPage_maks_senaryo.Text = "Maksimum Senaryo";
            this.tabPage_maks_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFMaxSenaryoTable
            // 
            this.ELFMaxSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFMaxSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMaxSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMaxSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMaxSenaryoTable.Location = new System.Drawing.Point(2, 2);
            this.ELFMaxSenaryoTable.Margin = new System.Windows.Forms.Padding(2);
            this.ELFMaxSenaryoTable.Name = "ELFMaxSenaryoTable";
            this.ELFMaxSenaryoTable.RowHeadersWidth = 18;
            this.ELFMaxSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMaxSenaryoTable.Size = new System.Drawing.Size(924, 826);
            this.ELFMaxSenaryoTable.TabIndex = 1;
            // 
            // YeniGenislemeSenaryoTabPage
            // 
            this.YeniGenislemeSenaryoTabPage.Location = new System.Drawing.Point(224, 4);
            this.YeniGenislemeSenaryoTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.YeniGenislemeSenaryoTabPage.Name = "YeniGenislemeSenaryoTabPage";
            this.YeniGenislemeSenaryoTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.YeniGenislemeSenaryoTabPage.Size = new System.Drawing.Size(1132, 538);
            this.YeniGenislemeSenaryoTabPage.TabIndex = 4;
            this.YeniGenislemeSenaryoTabPage.Text = "Yeni Genişleme Alanları ";
            this.YeniGenislemeSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // tab_stokastik
            // 
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_15);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_14);
            this.tab_stokastik.Controls.Add(this.panel_stokastik);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_13);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_12);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_11);
            this.tab_stokastik.Controls.Add(this.mesafe_metre_stokastik);
            this.tab_stokastik.Controls.Add(this.Mesafe_stokastik);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_10);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_9);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_8);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_7);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_6);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_5);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_4);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_3);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_2);
            this.tab_stokastik.Controls.Add(this.checkBox_stokastik_1);
            this.tab_stokastik.Controls.Add(this.label_stokastik_katmanlar);
            this.tab_stokastik.Controls.Add(this.stokastik_dosya_seçimi);
            this.tab_stokastik.Controls.Add(this.toolStrip_stokastik);
            this.tab_stokastik.ImageIndex = 15;
            this.tab_stokastik.Location = new System.Drawing.Point(4, 48);
            this.tab_stokastik.Margin = new System.Windows.Forms.Padding(2);
            this.tab_stokastik.Name = "tab_stokastik";
            this.tab_stokastik.Size = new System.Drawing.Size(1360, 546);
            this.tab_stokastik.TabIndex = 2;
            this.tab_stokastik.Text = "Stokastik Yük Tahmini Modülü";
            this.tab_stokastik.UseVisualStyleBackColor = true;
            // 
            // checkBox_stokastik_15
            // 
            this.checkBox_stokastik_15.AutoSize = true;
            this.checkBox_stokastik_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_15.Location = new System.Drawing.Point(5, 498);
            this.checkBox_stokastik_15.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_15.Name = "checkBox_stokastik_15";
            this.checkBox_stokastik_15.Size = new System.Drawing.Size(160, 21);
            this.checkBox_stokastik_15.TabIndex = 40;
            this.checkBox_stokastik_15.Text = "checkBox_stokastik_15";
            this.checkBox_stokastik_15.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_15.Visible = false;
            // 
            // checkBox_stokastik_14
            // 
            this.checkBox_stokastik_14.AutoSize = true;
            this.checkBox_stokastik_14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_14.Location = new System.Drawing.Point(5, 473);
            this.checkBox_stokastik_14.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_14.Name = "checkBox_stokastik_14";
            this.checkBox_stokastik_14.Size = new System.Drawing.Size(160, 21);
            this.checkBox_stokastik_14.TabIndex = 39;
            this.checkBox_stokastik_14.Text = "checkBox_stokastik_14";
            this.checkBox_stokastik_14.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_14.Visible = false;
            // 
            // panel_stokastik
            // 
            this.panel_stokastik.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_stokastik.Controls.Add(this.webView_stokastik);
            this.panel_stokastik.Controls.Add(this.gMapControl_stokastik);
            this.panel_stokastik.Controls.Add(this.buton_stokastik_harita_katmanlar);
            this.panel_stokastik.Location = new System.Drawing.Point(173, 28);
            this.panel_stokastik.Margin = new System.Windows.Forms.Padding(2);
            this.panel_stokastik.Name = "panel_stokastik";
            this.panel_stokastik.Size = new System.Drawing.Size(949, 500);
            this.panel_stokastik.TabIndex = 38;
            // 
            // webView_stokastik
            // 
            this.webView_stokastik.AllowExternalDrop = true;
            this.webView_stokastik.CreationProperties = null;
            this.webView_stokastik.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_stokastik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_stokastik.Location = new System.Drawing.Point(0, 0);
            this.webView_stokastik.Margin = new System.Windows.Forms.Padding(2);
            this.webView_stokastik.Name = "webView_stokastik";
            this.webView_stokastik.Size = new System.Drawing.Size(949, 500);
            this.webView_stokastik.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_stokastik.TabIndex = 36;
            this.webView_stokastik.Visible = false;
            this.webView_stokastik.ZoomFactor = 1D;
            // 
            // gMapControl_stokastik
            // 
            this.gMapControl_stokastik.AllowDrop = true;
            this.gMapControl_stokastik.Bearing = 0F;
            this.gMapControl_stokastik.CanDragMap = true;
            this.gMapControl_stokastik.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_stokastik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_stokastik.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_stokastik.GrayScaleMode = false;
            this.gMapControl_stokastik.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_stokastik.LevelsKeepInMemory = 5;
            this.gMapControl_stokastik.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_stokastik.Margin = new System.Windows.Forms.Padding(2);
            this.gMapControl_stokastik.MarkersEnabled = true;
            this.gMapControl_stokastik.MaxZoom = 2;
            this.gMapControl_stokastik.MinZoom = 2;
            this.gMapControl_stokastik.MouseWheelZoomEnabled = true;
            this.gMapControl_stokastik.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_stokastik.Name = "gMapControl_stokastik";
            this.gMapControl_stokastik.NegativeMode = false;
            this.gMapControl_stokastik.PolygonsEnabled = true;
            this.gMapControl_stokastik.RetryLoadTile = 0;
            this.gMapControl_stokastik.RoutesEnabled = true;
            this.gMapControl_stokastik.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_stokastik.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_stokastik.ShowTileGridLines = false;
            this.gMapControl_stokastik.Size = new System.Drawing.Size(949, 500);
            this.gMapControl_stokastik.TabIndex = 30;
            this.gMapControl_stokastik.Zoom = 0D;
            this.gMapControl_stokastik.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_stokastik_OnMapClick);
            this.gMapControl_stokastik.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_stokastik_OnMapDoubleClick);
            this.gMapControl_stokastik.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_stokastik_OnMarkerClick);
            this.gMapControl_stokastik.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_stokastik_MouseDown);
            this.gMapControl_stokastik.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_stokastik_MouseMove);
            this.gMapControl_stokastik.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_stokastik_MouseUp);
            // 
            // buton_stokastik_harita_katmanlar
            // 
            this.buton_stokastik_harita_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_stokastik_harita_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_stokastik_harita_katmanlar.BackgroundImage")));
            this.buton_stokastik_harita_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_stokastik_harita_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_stokastik_harita_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_stokastik_harita_katmanlar.Location = new System.Drawing.Point(0, 456);
            this.buton_stokastik_harita_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_stokastik_harita_katmanlar.Name = "buton_stokastik_harita_katmanlar";
            this.buton_stokastik_harita_katmanlar.Size = new System.Drawing.Size(44, 42);
            this.buton_stokastik_harita_katmanlar.TabIndex = 29;
            this.buton_stokastik_harita_katmanlar.UseVisualStyleBackColor = true;
            this.buton_stokastik_harita_katmanlar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buton_stokastik_harita_katmanlar_MouseClick);
            // 
            // checkBox_stokastik_13
            // 
            this.checkBox_stokastik_13.AutoSize = true;
            this.checkBox_stokastik_13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_13.Location = new System.Drawing.Point(5, 448);
            this.checkBox_stokastik_13.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_13.Name = "checkBox_stokastik_13";
            this.checkBox_stokastik_13.Size = new System.Drawing.Size(160, 21);
            this.checkBox_stokastik_13.TabIndex = 35;
            this.checkBox_stokastik_13.Text = "checkBox_stokastik_13";
            this.checkBox_stokastik_13.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_13.Visible = false;
            // 
            // checkBox_stokastik_12
            // 
            this.checkBox_stokastik_12.AutoSize = true;
            this.checkBox_stokastik_12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_12.Location = new System.Drawing.Point(5, 420);
            this.checkBox_stokastik_12.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_12.Name = "checkBox_stokastik_12";
            this.checkBox_stokastik_12.Size = new System.Drawing.Size(160, 21);
            this.checkBox_stokastik_12.TabIndex = 34;
            this.checkBox_stokastik_12.Text = "checkBox_stokastik_12";
            this.checkBox_stokastik_12.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_12.Visible = false;
            // 
            // checkBox_stokastik_11
            // 
            this.checkBox_stokastik_11.AutoSize = true;
            this.checkBox_stokastik_11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_11.Location = new System.Drawing.Point(6, 392);
            this.checkBox_stokastik_11.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_11.Name = "checkBox_stokastik_11";
            this.checkBox_stokastik_11.Size = new System.Drawing.Size(158, 21);
            this.checkBox_stokastik_11.TabIndex = 33;
            this.checkBox_stokastik_11.Text = "checkBox_stokastik_11";
            this.checkBox_stokastik_11.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_11.Visible = false;
            // 
            // mesafe_metre_stokastik
            // 
            this.mesafe_metre_stokastik.AutoSize = true;
            this.mesafe_metre_stokastik.Location = new System.Drawing.Point(262, 37);
            this.mesafe_metre_stokastik.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_stokastik.Name = "mesafe_metre_stokastik";
            this.mesafe_metre_stokastik.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_stokastik.TabIndex = 32;
            this.mesafe_metre_stokastik.Visible = false;
            // 
            // Mesafe_stokastik
            // 
            this.Mesafe_stokastik.AutoSize = true;
            this.Mesafe_stokastik.Location = new System.Drawing.Point(188, 37);
            this.Mesafe_stokastik.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_stokastik.Name = "Mesafe_stokastik";
            this.Mesafe_stokastik.Size = new System.Drawing.Size(54, 17);
            this.Mesafe_stokastik.TabIndex = 31;
            this.Mesafe_stokastik.Text = "Mesafe:";
            this.Mesafe_stokastik.Visible = false;
            // 
            // checkBox_stokastik_10
            // 
            this.checkBox_stokastik_10.AutoSize = true;
            this.checkBox_stokastik_10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_10.Location = new System.Drawing.Point(5, 365);
            this.checkBox_stokastik_10.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_10.Name = "checkBox_stokastik_10";
            this.checkBox_stokastik_10.Size = new System.Drawing.Size(160, 21);
            this.checkBox_stokastik_10.TabIndex = 28;
            this.checkBox_stokastik_10.Text = "checkBox_stokastik_10";
            this.checkBox_stokastik_10.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_10.Visible = false;
            // 
            // checkBox_stokastik_9
            // 
            this.checkBox_stokastik_9.AutoSize = true;
            this.checkBox_stokastik_9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_9.Location = new System.Drawing.Point(5, 337);
            this.checkBox_stokastik_9.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_9.Name = "checkBox_stokastik_9";
            this.checkBox_stokastik_9.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_9.TabIndex = 27;
            this.checkBox_stokastik_9.Text = "checkBox_stokastik_9";
            this.checkBox_stokastik_9.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_9.Visible = false;
            // 
            // checkBox_stokastik_8
            // 
            this.checkBox_stokastik_8.AutoSize = true;
            this.checkBox_stokastik_8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_8.Location = new System.Drawing.Point(5, 310);
            this.checkBox_stokastik_8.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_8.Name = "checkBox_stokastik_8";
            this.checkBox_stokastik_8.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_8.TabIndex = 26;
            this.checkBox_stokastik_8.Text = "checkBox_stokastik_8";
            this.checkBox_stokastik_8.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_8.Visible = false;
            // 
            // checkBox_stokastik_7
            // 
            this.checkBox_stokastik_7.AutoSize = true;
            this.checkBox_stokastik_7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_7.Location = new System.Drawing.Point(5, 282);
            this.checkBox_stokastik_7.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_7.Name = "checkBox_stokastik_7";
            this.checkBox_stokastik_7.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_7.TabIndex = 25;
            this.checkBox_stokastik_7.Text = "checkBox_stokastik_7";
            this.checkBox_stokastik_7.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_7.Visible = false;
            // 
            // checkBox_stokastik_6
            // 
            this.checkBox_stokastik_6.AutoSize = true;
            this.checkBox_stokastik_6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_6.Location = new System.Drawing.Point(5, 254);
            this.checkBox_stokastik_6.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_6.Name = "checkBox_stokastik_6";
            this.checkBox_stokastik_6.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_6.TabIndex = 24;
            this.checkBox_stokastik_6.Text = "checkBox_stokastik_6";
            this.checkBox_stokastik_6.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_6.Visible = false;
            // 
            // checkBox_stokastik_5
            // 
            this.checkBox_stokastik_5.AutoSize = true;
            this.checkBox_stokastik_5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_5.Location = new System.Drawing.Point(5, 227);
            this.checkBox_stokastik_5.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_5.Name = "checkBox_stokastik_5";
            this.checkBox_stokastik_5.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_5.TabIndex = 23;
            this.checkBox_stokastik_5.Text = "checkBox_stokastik_5";
            this.checkBox_stokastik_5.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_5.Visible = false;
            // 
            // checkBox_stokastik_4
            // 
            this.checkBox_stokastik_4.AutoSize = true;
            this.checkBox_stokastik_4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_4.Location = new System.Drawing.Point(5, 199);
            this.checkBox_stokastik_4.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_4.Name = "checkBox_stokastik_4";
            this.checkBox_stokastik_4.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_4.TabIndex = 22;
            this.checkBox_stokastik_4.Text = "checkBox_stokastik_4";
            this.checkBox_stokastik_4.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_4.Visible = false;
            // 
            // checkBox_stokastik_3
            // 
            this.checkBox_stokastik_3.AutoSize = true;
            this.checkBox_stokastik_3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_3.Location = new System.Drawing.Point(5, 171);
            this.checkBox_stokastik_3.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_3.Name = "checkBox_stokastik_3";
            this.checkBox_stokastik_3.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_3.TabIndex = 21;
            this.checkBox_stokastik_3.Text = "checkBox_stokastik_3";
            this.checkBox_stokastik_3.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_3.Visible = false;
            // 
            // checkBox_stokastik_2
            // 
            this.checkBox_stokastik_2.AutoSize = true;
            this.checkBox_stokastik_2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_2.Location = new System.Drawing.Point(5, 144);
            this.checkBox_stokastik_2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_2.Name = "checkBox_stokastik_2";
            this.checkBox_stokastik_2.Size = new System.Drawing.Size(155, 21);
            this.checkBox_stokastik_2.TabIndex = 20;
            this.checkBox_stokastik_2.Text = "checkBox_stokastik_2";
            this.checkBox_stokastik_2.UseVisualStyleBackColor = true;
            this.checkBox_stokastik_2.Visible = false;
            // 
            // checkBox_stokastik_1
            // 
            this.checkBox_stokastik_1.AutoSize = true;
            this.checkBox_stokastik_1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_stokastik_1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_stokastik_1.Location = new System.Drawing.Point(5, 116);
            this.checkBox_stokastik_1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_stokastik_1.Name = "checkBox_stokastik_1";
            this.checkBox_stokastik_1.Size = new System.Drawing.Size(153, 21);
            this.checkBox_stokastik_1.TabIndex = 19;
            this.checkBox_stokastik_1.Text = "checkBox_stokastik_1";
            this.checkBox_stokastik_1.UseVisualStyleBackColor = false;
            this.checkBox_stokastik_1.Visible = false;
            // 
            // label_stokastik_katmanlar
            // 
            this.label_stokastik_katmanlar.AutoSize = true;
            this.label_stokastik_katmanlar.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_stokastik_katmanlar.Location = new System.Drawing.Point(17, 83);
            this.label_stokastik_katmanlar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_stokastik_katmanlar.Name = "label_stokastik_katmanlar";
            this.label_stokastik_katmanlar.Size = new System.Drawing.Size(81, 19);
            this.label_stokastik_katmanlar.TabIndex = 18;
            this.label_stokastik_katmanlar.Text = "Katmanlar";
            // 
            // stokastik_dosya_seçimi
            // 
            this.stokastik_dosya_seçimi.Location = new System.Drawing.Point(6, 37);
            this.stokastik_dosya_seçimi.Margin = new System.Windows.Forms.Padding(2);
            this.stokastik_dosya_seçimi.Name = "stokastik_dosya_seçimi";
            this.stokastik_dosya_seçimi.Size = new System.Drawing.Size(130, 36);
            this.stokastik_dosya_seçimi.TabIndex = 17;
            this.stokastik_dosya_seçimi.Text = "Dosya Seç";
            this.stokastik_dosya_seçimi.UseVisualStyleBackColor = true;
            this.stokastik_dosya_seçimi.Click += new System.EventHandler(this.stokastik_dosya_seçimi_Click);
            // 
            // toolStrip_stokastik
            // 
            this.toolStrip_stokastik.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip_stokastik.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Stokastik_Seç,
            this.toolStripSeparator1,
            this.Stokastik_Kaydır,
            this.toolStripSeparator2,
            this.Stokastik_Mesafe_Ölç,
            this.toolStripSeparator3,
            this.Stokastik_Poligon,
            this.toolStripSeparator4,
            this.Stokastik_Nokta,
            this.toolStripSeparator5,
            this.Stokastik_Grid_Oluştur,
            this.toolStripSeparator6,
            this.Stokastik_Fonksiyonlar});
            this.toolStrip_stokastik.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_stokastik.Name = "toolStrip_stokastik";
            this.toolStrip_stokastik.Size = new System.Drawing.Size(1360, 27);
            this.toolStrip_stokastik.Stretch = true;
            this.toolStrip_stokastik.TabIndex = 1;
            this.toolStrip_stokastik.Text = "toolStrip1";
            // 
            // Stokastik_Seç
            // 
            this.Stokastik_Seç.AccessibleDescription = "";
            this.Stokastik_Seç.AccessibleName = "";
            this.Stokastik_Seç.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(250)))), ((int)(((byte)(249)))));
            this.Stokastik_Seç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Seç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Seç.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Seç.Image")));
            this.Stokastik_Seç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Seç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Seç.Margin = new System.Windows.Forms.Padding(175, 1, 0, 2);
            this.Stokastik_Seç.Name = "Stokastik_Seç";
            this.Stokastik_Seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Seç.Size = new System.Drawing.Size(66, 24);
            this.Stokastik_Seç.Tag = "";
            this.Stokastik_Seç.Text = "Seç";
            this.Stokastik_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.Stokastik_Seç.Click += new System.EventHandler(this.Stokastik_Seç_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Kaydır
            // 
            this.Stokastik_Kaydır.AccessibleDescription = "";
            this.Stokastik_Kaydır.AccessibleName = "";
            this.Stokastik_Kaydır.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Stokastik_Kaydır.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Kaydır.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Kaydır.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Kaydır.Image")));
            this.Stokastik_Kaydır.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Kaydır.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Kaydır.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Kaydır.Name = "Stokastik_Kaydır";
            this.Stokastik_Kaydır.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Kaydır.Size = new System.Drawing.Size(85, 24);
            this.Stokastik_Kaydır.Tag = "";
            this.Stokastik_Kaydır.Text = "Kaydır";
            this.Stokastik_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            this.Stokastik_Kaydır.Click += new System.EventHandler(this.Stokastik_Kaydır_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Mesafe_Ölç
            // 
            this.Stokastik_Mesafe_Ölç.AccessibleDescription = "";
            this.Stokastik_Mesafe_Ölç.AccessibleName = "";
            this.Stokastik_Mesafe_Ölç.BackColor = System.Drawing.Color.Honeydew;
            this.Stokastik_Mesafe_Ölç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Mesafe_Ölç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Mesafe_Ölç.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Mesafe_Ölç.Image")));
            this.Stokastik_Mesafe_Ölç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Mesafe_Ölç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Mesafe_Ölç.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Mesafe_Ölç.Name = "Stokastik_Mesafe_Ölç";
            this.Stokastik_Mesafe_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Mesafe_Ölç.Size = new System.Drawing.Size(117, 24);
            this.Stokastik_Mesafe_Ölç.Tag = "";
            this.Stokastik_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.Stokastik_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            this.Stokastik_Mesafe_Ölç.Click += new System.EventHandler(this.Stokastik_Mesafe_Ölç_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Poligon
            // 
            this.Stokastik_Poligon.AccessibleDescription = "";
            this.Stokastik_Poligon.AccessibleName = "";
            this.Stokastik_Poligon.BackColor = System.Drawing.Color.Thistle;
            this.Stokastik_Poligon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Poligon.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Poligon.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Poligon.Image")));
            this.Stokastik_Poligon.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Poligon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Poligon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Poligon.Name = "Stokastik_Poligon";
            this.Stokastik_Poligon.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Poligon.Size = new System.Drawing.Size(93, 24);
            this.Stokastik_Poligon.Tag = "";
            this.Stokastik_Poligon.Text = "Poligon";
            this.Stokastik_Poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            this.Stokastik_Poligon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Stokastik_Poligon_MouseDown);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Nokta
            // 
            this.Stokastik_Nokta.AccessibleDescription = "";
            this.Stokastik_Nokta.AccessibleName = "";
            this.Stokastik_Nokta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Stokastik_Nokta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Nokta.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Nokta.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Nokta.Image")));
            this.Stokastik_Nokta.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Nokta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Nokta.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Nokta.Name = "Stokastik_Nokta";
            this.Stokastik_Nokta.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Nokta.Size = new System.Drawing.Size(83, 24);
            this.Stokastik_Nokta.Tag = "";
            this.Stokastik_Nokta.Text = "Nokta";
            this.Stokastik_Nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            this.Stokastik_Nokta.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Stokastik_Nokta_MouseDown);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Grid_Oluştur
            // 
            this.Stokastik_Grid_Oluştur.AccessibleDescription = "";
            this.Stokastik_Grid_Oluştur.AccessibleName = "";
            this.Stokastik_Grid_Oluştur.BackColor = System.Drawing.Color.Beige;
            this.Stokastik_Grid_Oluştur.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Grid_Oluştur.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Grid_Oluştur.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Grid_Oluştur.Image")));
            this.Stokastik_Grid_Oluştur.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Grid_Oluştur.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Grid_Oluştur.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Grid_Oluştur.Name = "Stokastik_Grid_Oluştur";
            this.Stokastik_Grid_Oluştur.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Grid_Oluştur.Size = new System.Drawing.Size(122, 24);
            this.Stokastik_Grid_Oluştur.Tag = "";
            this.Stokastik_Grid_Oluştur.Text = "Grid Oluştur";
            this.Stokastik_Grid_Oluştur.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Grid_Oluştur.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Grid_Oluştur.ToolTipText = "Belirli bir alan seçilip bu alanda mxn şeklinde bir grid (ızgara) tanımlar.";
            this.Stokastik_Grid_Oluştur.Click += new System.EventHandler(this.Stokastik_Grid_Oluştur_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // Stokastik_Fonksiyonlar
            // 
            this.Stokastik_Fonksiyonlar.AccessibleDescription = "";
            this.Stokastik_Fonksiyonlar.AccessibleName = "";
            this.Stokastik_Fonksiyonlar.BackColor = System.Drawing.Color.LightBlue;
            this.Stokastik_Fonksiyonlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Stokastik_Fonksiyonlar.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Stokastik_Fonksiyonlar.Image = ((System.Drawing.Image)(resources.GetObject("Stokastik_Fonksiyonlar.Image")));
            this.Stokastik_Fonksiyonlar.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Stokastik_Fonksiyonlar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stokastik_Fonksiyonlar.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Stokastik_Fonksiyonlar.Name = "Stokastik_Fonksiyonlar";
            this.Stokastik_Fonksiyonlar.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Fonksiyonlar.Size = new System.Drawing.Size(125, 24);
            this.Stokastik_Fonksiyonlar.Tag = "";
            this.Stokastik_Fonksiyonlar.Text = "Fonksiyonlar";
            this.Stokastik_Fonksiyonlar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Fonksiyonlar.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Fonksiyonlar.ToolTipText = "Çeşitli vektörel veya tabular algoritmaları içerir.";
            this.Stokastik_Fonksiyonlar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Stokastik_Fonksiyonlar_MouseDown);
            // 
            // tab_yükHaritası
            // 
            this.tab_yükHaritası.Controls.Add(this.legendPanel);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_deger);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_text);
            this.tab_yükHaritası.Controls.Add(this.trackBar_Yıllar);
            this.tab_yükHaritası.Controls.Add(this.Mesafe_yuk);
            this.tab_yükHaritası.Controls.Add(this.mesafe_metre_yuk);
            this.tab_yükHaritası.Controls.Add(this.toolStrip_yuk);
            this.tab_yükHaritası.Controls.Add(this.gMapControl_yuk);
            this.tab_yükHaritası.Controls.Add(this.buton_yuk_haritası_katmanlar);
            this.tab_yükHaritası.Location = new System.Drawing.Point(4, 48);
            this.tab_yükHaritası.Margin = new System.Windows.Forms.Padding(2);
            this.tab_yükHaritası.Name = "tab_yükHaritası";
            this.tab_yükHaritası.Size = new System.Drawing.Size(1360, 546);
            this.tab_yükHaritası.TabIndex = 9;
            this.tab_yükHaritası.Text = "Yük Haritası Modülü";
            this.tab_yükHaritası.UseVisualStyleBackColor = true;
            // 
            // legendPanel
            // 
            this.legendPanel.AutoSize = true;
            this.legendPanel.Location = new System.Drawing.Point(6, 80);
            this.legendPanel.Margin = new System.Windows.Forms.Padding(2);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(161, 328);
            this.legendPanel.TabIndex = 44;
            // 
            // yuk_yıl_deger
            // 
            this.yuk_yıl_deger.AutoSize = true;
            this.yuk_yıl_deger.Location = new System.Drawing.Point(24, 9);
            this.yuk_yıl_deger.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.yuk_yıl_deger.Name = "yuk_yıl_deger";
            this.yuk_yıl_deger.Size = new System.Drawing.Size(36, 17);
            this.yuk_yıl_deger.TabIndex = 41;
            this.yuk_yıl_deger.Text = "2024";
            // 
            // yuk_yıl_text
            // 
            this.yuk_yıl_text.AutoSize = true;
            this.yuk_yıl_text.Location = new System.Drawing.Point(4, 9);
            this.yuk_yıl_text.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.yuk_yıl_text.Name = "yuk_yıl_text";
            this.yuk_yıl_text.Size = new System.Drawing.Size(25, 17);
            this.yuk_yıl_text.TabIndex = 40;
            this.yuk_yıl_text.Text = "Yıl:";
            // 
            // trackBar_Yıllar
            // 
            this.trackBar_Yıllar.Location = new System.Drawing.Point(4, 30);
            this.trackBar_Yıllar.Margin = new System.Windows.Forms.Padding(2);
            this.trackBar_Yıllar.Maximum = 2030;
            this.trackBar_Yıllar.Minimum = 2024;
            this.trackBar_Yıllar.Name = "trackBar_Yıllar";
            this.trackBar_Yıllar.Size = new System.Drawing.Size(163, 45);
            this.trackBar_Yıllar.TabIndex = 39;
            this.trackBar_Yıllar.Value = 2024;
            this.trackBar_Yıllar.ValueChanged += new System.EventHandler(this.trackBar_Yıllar_ValueChanged);
            // 
            // Mesafe_yuk
            // 
            this.Mesafe_yuk.AutoSize = true;
            this.Mesafe_yuk.Location = new System.Drawing.Point(194, 29);
            this.Mesafe_yuk.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_yuk.Name = "Mesafe_yuk";
            this.Mesafe_yuk.Size = new System.Drawing.Size(54, 17);
            this.Mesafe_yuk.TabIndex = 38;
            this.Mesafe_yuk.Text = "Mesafe:";
            this.Mesafe_yuk.Visible = false;
            // 
            // mesafe_metre_yuk
            // 
            this.mesafe_metre_yuk.AutoSize = true;
            this.mesafe_metre_yuk.Location = new System.Drawing.Point(268, 28);
            this.mesafe_metre_yuk.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_yuk.Name = "mesafe_metre_yuk";
            this.mesafe_metre_yuk.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_yuk.TabIndex = 37;
            this.mesafe_metre_yuk.Visible = false;
            // 
            // toolStrip_yuk
            // 
            this.toolStrip_yuk.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip_yuk.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Yuk_Seç,
            this.toolStripSeparator7,
            this.Yuk_Kaydır,
            this.toolStripSeparator8,
            this.Yuk_Mesafe_Ölç});
            this.toolStrip_yuk.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_yuk.Name = "toolStrip_yuk";
            this.toolStrip_yuk.Size = new System.Drawing.Size(1360, 27);
            this.toolStrip_yuk.TabIndex = 36;
            this.toolStrip_yuk.Text = "toolStrip1";
            // 
            // Yuk_Seç
            // 
            this.Yuk_Seç.AccessibleDescription = "";
            this.Yuk_Seç.AccessibleName = "";
            this.Yuk_Seç.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(250)))), ((int)(((byte)(249)))));
            this.Yuk_Seç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Yuk_Seç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Yuk_Seç.Image = ((System.Drawing.Image)(resources.GetObject("Yuk_Seç.Image")));
            this.Yuk_Seç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Yuk_Seç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Yuk_Seç.Margin = new System.Windows.Forms.Padding(300, 1, 0, 2);
            this.Yuk_Seç.Name = "Yuk_Seç";
            this.Yuk_Seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Yuk_Seç.Size = new System.Drawing.Size(66, 24);
            this.Yuk_Seç.Tag = "";
            this.Yuk_Seç.Text = "Seç";
            this.Yuk_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.Yuk_Seç.Click += new System.EventHandler(this.Yuk_Seç_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // Yuk_Kaydır
            // 
            this.Yuk_Kaydır.AccessibleDescription = "";
            this.Yuk_Kaydır.AccessibleName = "";
            this.Yuk_Kaydır.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Yuk_Kaydır.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Yuk_Kaydır.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Yuk_Kaydır.Image = ((System.Drawing.Image)(resources.GetObject("Yuk_Kaydır.Image")));
            this.Yuk_Kaydır.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Yuk_Kaydır.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Yuk_Kaydır.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Yuk_Kaydır.Name = "Yuk_Kaydır";
            this.Yuk_Kaydır.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Yuk_Kaydır.Size = new System.Drawing.Size(85, 24);
            this.Yuk_Kaydır.Tag = "";
            this.Yuk_Kaydır.Text = "Kaydır";
            this.Yuk_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 27);
            // 
            // Yuk_Mesafe_Ölç
            // 
            this.Yuk_Mesafe_Ölç.AccessibleDescription = "";
            this.Yuk_Mesafe_Ölç.AccessibleName = "";
            this.Yuk_Mesafe_Ölç.BackColor = System.Drawing.Color.Honeydew;
            this.Yuk_Mesafe_Ölç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Yuk_Mesafe_Ölç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Yuk_Mesafe_Ölç.Image = ((System.Drawing.Image)(resources.GetObject("Yuk_Mesafe_Ölç.Image")));
            this.Yuk_Mesafe_Ölç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Yuk_Mesafe_Ölç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Yuk_Mesafe_Ölç.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.Yuk_Mesafe_Ölç.Name = "Yuk_Mesafe_Ölç";
            this.Yuk_Mesafe_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Yuk_Mesafe_Ölç.Size = new System.Drawing.Size(117, 24);
            this.Yuk_Mesafe_Ölç.Tag = "";
            this.Yuk_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.Yuk_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            // 
            // gMapControl_yuk
            // 
            this.gMapControl_yuk.AllowDrop = true;
            this.gMapControl_yuk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_yuk.Bearing = 0F;
            this.gMapControl_yuk.CanDragMap = true;
            this.gMapControl_yuk.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_yuk.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_yuk.GrayScaleMode = false;
            this.gMapControl_yuk.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_yuk.LevelsKeepInMemory = 5;
            this.gMapControl_yuk.Location = new System.Drawing.Point(197, 48);
            this.gMapControl_yuk.Margin = new System.Windows.Forms.Padding(2);
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
            this.gMapControl_yuk.Size = new System.Drawing.Size(950, 518);
            this.gMapControl_yuk.TabIndex = 34;
            this.gMapControl_yuk.Zoom = 0D;
            this.gMapControl_yuk.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_yuk_OnMapClick);
            this.gMapControl_yuk.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseDown);
            this.gMapControl_yuk.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseMove);
            this.gMapControl_yuk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseUp);
            // 
            // buton_yuk_haritası_katmanlar
            // 
            this.buton_yuk_haritası_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_yuk_haritası_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_yuk_haritası_katmanlar.BackgroundImage")));
            this.buton_yuk_haritası_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_yuk_haritası_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_yuk_haritası_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_yuk_haritası_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_yuk_haritası_katmanlar.Location = new System.Drawing.Point(197, 523);
            this.buton_yuk_haritası_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_yuk_haritası_katmanlar.Name = "buton_yuk_haritası_katmanlar";
            this.buton_yuk_haritası_katmanlar.Size = new System.Drawing.Size(44, 42);
            this.buton_yuk_haritası_katmanlar.TabIndex = 35;
            this.buton_yuk_haritası_katmanlar.UseVisualStyleBackColor = true;
            // 
            // tab_rapor
            // 
            this.tab_rapor.ImageIndex = 16;
            this.tab_rapor.Location = new System.Drawing.Point(4, 48);
            this.tab_rapor.Margin = new System.Windows.Forms.Padding(2);
            this.tab_rapor.Name = "tab_rapor";
            this.tab_rapor.Size = new System.Drawing.Size(1360, 546);
            this.tab_rapor.TabIndex = 8;
            this.tab_rapor.Text = "Raporlama";
            this.tab_rapor.UseVisualStyleBackColor = true;
            // 
            // tab_validasyon
            // 
            this.tab_validasyon.Location = new System.Drawing.Point(4, 48);
            this.tab_validasyon.Margin = new System.Windows.Forms.Padding(2);
            this.tab_validasyon.Name = "tab_validasyon";
            this.tab_validasyon.Size = new System.Drawing.Size(1360, 546);
            this.tab_validasyon.TabIndex = 10;
            this.tab_validasyon.Text = "Validasyon Modülü";
            this.tab_validasyon.UseVisualStyleBackColor = true;
            // 
            // tab_yga
            // 
            this.tab_yga.Controls.Add(this.buton_dosya_yga);
            this.tab_yga.Controls.Add(this.checkBox_yga_15);
            this.tab_yga.Controls.Add(this.checkBox_yga_14);
            this.tab_yga.Controls.Add(this.panel_yga);
            this.tab_yga.Controls.Add(this.label_yga_katmanlar);
            this.tab_yga.Controls.Add(this.FinishPolygonButton);
            this.tab_yga.Controls.Add(this.checkBox_yga_13);
            this.tab_yga.Controls.Add(this.checkBox_yga_12);
            this.tab_yga.Controls.Add(this.toolStrip_yga);
            this.tab_yga.Controls.Add(this.checkBox_yga_11);
            this.tab_yga.Controls.Add(this.checkBox_yga_10);
            this.tab_yga.Controls.Add(this.checkBox_yga_9);
            this.tab_yga.Controls.Add(this.checkBox_yga_8);
            this.tab_yga.Controls.Add(this.checkBox_yga_5);
            this.tab_yga.Controls.Add(this.checkBox_yga_7);
            this.tab_yga.Controls.Add(this.checkBox_yga_1);
            this.tab_yga.Controls.Add(this.checkBox_yga_6);
            this.tab_yga.Controls.Add(this.checkBox_yga_2);
            this.tab_yga.Controls.Add(this.checkBox_yga_3);
            this.tab_yga.Controls.Add(this.checkBox_yga_4);
            this.tab_yga.ImageIndex = 13;
            this.tab_yga.Location = new System.Drawing.Point(4, 48);
            this.tab_yga.Name = "tab_yga";
            this.tab_yga.Padding = new System.Windows.Forms.Padding(3);
            this.tab_yga.Size = new System.Drawing.Size(1360, 546);
            this.tab_yga.TabIndex = 11;
            this.tab_yga.Text = "Yeni Genişleme Alanları";
            this.tab_yga.UseVisualStyleBackColor = true;
            // 
            // buton_dosya_yga
            // 
            this.buton_dosya_yga.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_dosya_yga.BackColor = System.Drawing.Color.White;
            this.buton_dosya_yga.Location = new System.Drawing.Point(5, 476);
            this.buton_dosya_yga.Margin = new System.Windows.Forms.Padding(2);
            this.buton_dosya_yga.Name = "buton_dosya_yga";
            this.buton_dosya_yga.Size = new System.Drawing.Size(118, 34);
            this.buton_dosya_yga.TabIndex = 62;
            this.buton_dosya_yga.Text = "Dosya Seç";
            this.buton_dosya_yga.UseVisualStyleBackColor = false;
            this.buton_dosya_yga.Click += new System.EventHandler(this.buton_dosya_yga_Click);
            // 
            // checkBox_yga_15
            // 
            this.checkBox_yga_15.AutoSize = true;
            this.checkBox_yga_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_15.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_15.Location = new System.Drawing.Point(8, 440);
            this.checkBox_yga_15.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_15.Name = "checkBox_yga_15";
            this.checkBox_yga_15.Size = new System.Drawing.Size(128, 21);
            this.checkBox_yga_15.TabIndex = 61;
            this.checkBox_yga_15.Text = "checkBox_yga_15";
            this.checkBox_yga_15.UseVisualStyleBackColor = true;
            this.checkBox_yga_15.Visible = false;
            // 
            // checkBox_yga_14
            // 
            this.checkBox_yga_14.AutoSize = true;
            this.checkBox_yga_14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_14.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_14.Location = new System.Drawing.Point(7, 415);
            this.checkBox_yga_14.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_14.Name = "checkBox_yga_14";
            this.checkBox_yga_14.Size = new System.Drawing.Size(128, 21);
            this.checkBox_yga_14.TabIndex = 60;
            this.checkBox_yga_14.Text = "checkBox_yga_14";
            this.checkBox_yga_14.UseVisualStyleBackColor = true;
            this.checkBox_yga_14.Visible = false;
            // 
            // panel_yga
            // 
            this.panel_yga.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_yga.Controls.Add(this.webView_yga);
            this.panel_yga.Controls.Add(this.gMapControl_yga);
            this.panel_yga.Controls.Add(this.buton_yga_harita_katmanlar);
            this.panel_yga.Controls.Add(this.Mesafe_yga);
            this.panel_yga.Controls.Add(this.mesafe_metre_yga);
            this.panel_yga.Location = new System.Drawing.Point(183, 31);
            this.panel_yga.Margin = new System.Windows.Forms.Padding(2);
            this.panel_yga.Name = "panel_yga";
            this.panel_yga.Size = new System.Drawing.Size(957, 488);
            this.panel_yga.TabIndex = 59;
            // 
            // webView_yga
            // 
            this.webView_yga.AllowExternalDrop = true;
            this.webView_yga.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_yga.CreationProperties = null;
            this.webView_yga.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_yga.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_yga.Location = new System.Drawing.Point(0, 0);
            this.webView_yga.Margin = new System.Windows.Forms.Padding(2);
            this.webView_yga.Name = "webView_yga";
            this.webView_yga.Size = new System.Drawing.Size(957, 488);
            this.webView_yga.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_yga.TabIndex = 40;
            this.webView_yga.Visible = false;
            this.webView_yga.ZoomFactor = 1D;
            // 
            // gMapControl_yga
            // 
            this.gMapControl_yga.AllowDrop = true;
            this.gMapControl_yga.Bearing = 0F;
            this.gMapControl_yga.CanDragMap = true;
            this.gMapControl_yga.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_yga.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl_yga.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_yga.GrayScaleMode = false;
            this.gMapControl_yga.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_yga.LevelsKeepInMemory = 5;
            this.gMapControl_yga.Location = new System.Drawing.Point(0, 0);
            this.gMapControl_yga.Margin = new System.Windows.Forms.Padding(2);
            this.gMapControl_yga.MarkersEnabled = true;
            this.gMapControl_yga.MaxZoom = 2;
            this.gMapControl_yga.MinZoom = 2;
            this.gMapControl_yga.MouseWheelZoomEnabled = true;
            this.gMapControl_yga.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_yga.Name = "gMapControl_yga";
            this.gMapControl_yga.NegativeMode = false;
            this.gMapControl_yga.PolygonsEnabled = true;
            this.gMapControl_yga.RetryLoadTile = 0;
            this.gMapControl_yga.RoutesEnabled = true;
            this.gMapControl_yga.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_yga.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_yga.ShowTileGridLines = false;
            this.gMapControl_yga.Size = new System.Drawing.Size(957, 488);
            this.gMapControl_yga.TabIndex = 57;
            this.gMapControl_yga.Zoom = 0D;
            this.gMapControl_yga.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_yga_OnMapClick);
            this.gMapControl_yga.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_yga_OnMapDoubleClick);
            this.gMapControl_yga.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_yga_OnMarkerClick);
            this.gMapControl_yga.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yga_MouseDown);
            this.gMapControl_yga.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yga_MouseMove);
            // 
            // buton_yga_harita_katmanlar
            // 
            this.buton_yga_harita_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_yga_harita_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_yga_harita_katmanlar.BackgroundImage")));
            this.buton_yga_harita_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_yga_harita_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_yga_harita_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_yga_harita_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_yga_harita_katmanlar.Location = new System.Drawing.Point(2, 441);
            this.buton_yga_harita_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_yga_harita_katmanlar.Name = "buton_yga_harita_katmanlar";
            this.buton_yga_harita_katmanlar.Size = new System.Drawing.Size(46, 43);
            this.buton_yga_harita_katmanlar.TabIndex = 56;
            this.buton_yga_harita_katmanlar.UseVisualStyleBackColor = true;
            // 
            // Mesafe_yga
            // 
            this.Mesafe_yga.AutoSize = true;
            this.Mesafe_yga.Location = new System.Drawing.Point(11, 9);
            this.Mesafe_yga.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_yga.Name = "Mesafe_yga";
            this.Mesafe_yga.Size = new System.Drawing.Size(54, 17);
            this.Mesafe_yga.TabIndex = 56;
            this.Mesafe_yga.Text = "Mesafe:";
            this.Mesafe_yga.Visible = false;
            // 
            // mesafe_metre_yga
            // 
            this.mesafe_metre_yga.AutoSize = true;
            this.mesafe_metre_yga.Location = new System.Drawing.Point(84, 9);
            this.mesafe_metre_yga.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_yga.Name = "mesafe_metre_yga";
            this.mesafe_metre_yga.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_yga.TabIndex = 56;
            this.mesafe_metre_yga.Visible = false;
            // 
            // label_yga_katmanlar
            // 
            this.label_yga_katmanlar.AutoSize = true;
            this.label_yga_katmanlar.BackColor = System.Drawing.Color.Transparent;
            this.label_yga_katmanlar.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_yga_katmanlar.ForeColor = System.Drawing.Color.DarkOrange;
            this.label_yga_katmanlar.Location = new System.Drawing.Point(4, 31);
            this.label_yga_katmanlar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_yga_katmanlar.Name = "label_yga_katmanlar";
            this.label_yga_katmanlar.Size = new System.Drawing.Size(81, 19);
            this.label_yga_katmanlar.TabIndex = 42;
            this.label_yga_katmanlar.Text = "Katmanlar";
            // 
            // FinishPolygonButton
            // 
            this.FinishPolygonButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FinishPolygonButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.FinishPolygonButton.FlatAppearance.BorderSize = 0;
            this.FinishPolygonButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FinishPolygonButton.ForeColor = System.Drawing.Color.Snow;
            this.FinishPolygonButton.Location = new System.Drawing.Point(8, 515);
            this.FinishPolygonButton.Name = "FinishPolygonButton";
            this.FinishPolygonButton.Size = new System.Drawing.Size(116, 32);
            this.FinishPolygonButton.TabIndex = 37;
            this.FinishPolygonButton.Text = "YGA KAYDET";
            this.FinishPolygonButton.UseVisualStyleBackColor = false;
            this.FinishPolygonButton.Click += new System.EventHandler(this.YGASaveButton_Click);
            // 
            // checkBox_yga_13
            // 
            this.checkBox_yga_13.AutoSize = true;
            this.checkBox_yga_13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_13.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_13.Location = new System.Drawing.Point(6, 390);
            this.checkBox_yga_13.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_13.Name = "checkBox_yga_13";
            this.checkBox_yga_13.Size = new System.Drawing.Size(128, 21);
            this.checkBox_yga_13.TabIndex = 55;
            this.checkBox_yga_13.Text = "checkBox_yga_13";
            this.checkBox_yga_13.UseVisualStyleBackColor = true;
            this.checkBox_yga_13.Visible = false;
            // 
            // checkBox_yga_12
            // 
            this.checkBox_yga_12.AutoSize = true;
            this.checkBox_yga_12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_12.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_12.Location = new System.Drawing.Point(6, 362);
            this.checkBox_yga_12.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_12.Name = "checkBox_yga_12";
            this.checkBox_yga_12.Size = new System.Drawing.Size(128, 21);
            this.checkBox_yga_12.TabIndex = 54;
            this.checkBox_yga_12.Text = "checkBox_yga_12";
            this.checkBox_yga_12.UseVisualStyleBackColor = true;
            this.checkBox_yga_12.Visible = false;
            // 
            // toolStrip_yga
            // 
            this.toolStrip_yga.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip_yga.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_yga_seç,
            this.toolStripSeparator15,
            this.toolStrip_yga_kaydır,
            this.toolStripSeparator16,
            this.toolStrip_yga_mesafe,
            this.toolStripSeparator17,
            this.toolStrip_yga_poligon,
            this.toolStripSeparator18,
            this.toolStrip_yga_nokta,
            this.toolStripSeparator19,
            this.toolStrip_yga_fonksiyon});
            this.toolStrip_yga.Location = new System.Drawing.Point(3, 3);
            this.toolStrip_yga.Name = "toolStrip_yga";
            this.toolStrip_yga.Size = new System.Drawing.Size(1354, 27);
            this.toolStrip_yga.Stretch = true;
            this.toolStrip_yga.TabIndex = 58;
            this.toolStrip_yga.Text = "toolStrip1";
            // 
            // toolStrip_yga_seç
            // 
            this.toolStrip_yga_seç.AccessibleDescription = "";
            this.toolStrip_yga_seç.AccessibleName = "";
            this.toolStrip_yga_seç.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(250)))), ((int)(((byte)(249)))));
            this.toolStrip_yga_seç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_seç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_seç.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_seç.Image")));
            this.toolStrip_yga_seç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_seç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_seç.Margin = new System.Windows.Forms.Padding(175, 1, 0, 2);
            this.toolStrip_yga_seç.Name = "toolStrip_yga_seç";
            this.toolStrip_yga_seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_seç.Size = new System.Drawing.Size(66, 24);
            this.toolStrip_yga_seç.Tag = "";
            this.toolStrip_yga_seç.Text = "Seç";
            this.toolStrip_yga_seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_seç.ToolTipText = "Harita üzerinde seçim yapar.";
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStrip_yga_kaydır
            // 
            this.toolStrip_yga_kaydır.AccessibleDescription = "";
            this.toolStrip_yga_kaydır.AccessibleName = "";
            this.toolStrip_yga_kaydır.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.toolStrip_yga_kaydır.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_kaydır.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_kaydır.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_kaydır.Image")));
            this.toolStrip_yga_kaydır.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_kaydır.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_kaydır.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStrip_yga_kaydır.Name = "toolStrip_yga_kaydır";
            this.toolStrip_yga_kaydır.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_kaydır.Size = new System.Drawing.Size(85, 24);
            this.toolStrip_yga_kaydır.Tag = "";
            this.toolStrip_yga_kaydır.Text = "Kaydır";
            this.toolStrip_yga_kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStrip_yga_mesafe
            // 
            this.toolStrip_yga_mesafe.AccessibleDescription = "";
            this.toolStrip_yga_mesafe.AccessibleName = "";
            this.toolStrip_yga_mesafe.BackColor = System.Drawing.Color.Honeydew;
            this.toolStrip_yga_mesafe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_mesafe.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_mesafe.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_mesafe.Image")));
            this.toolStrip_yga_mesafe.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_mesafe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_mesafe.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStrip_yga_mesafe.Name = "toolStrip_yga_mesafe";
            this.toolStrip_yga_mesafe.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_mesafe.Size = new System.Drawing.Size(117, 24);
            this.toolStrip_yga_mesafe.Tag = "";
            this.toolStrip_yga_mesafe.Text = "Mesafe Ölç";
            this.toolStrip_yga_mesafe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_mesafe.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_mesafe.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStrip_yga_poligon
            // 
            this.toolStrip_yga_poligon.AccessibleDescription = "";
            this.toolStrip_yga_poligon.AccessibleName = "";
            this.toolStrip_yga_poligon.BackColor = System.Drawing.Color.Thistle;
            this.toolStrip_yga_poligon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_poligon.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_poligon.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_poligon.Image")));
            this.toolStrip_yga_poligon.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_poligon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_poligon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStrip_yga_poligon.Name = "toolStrip_yga_poligon";
            this.toolStrip_yga_poligon.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_poligon.Size = new System.Drawing.Size(93, 24);
            this.toolStrip_yga_poligon.Tag = "";
            this.toolStrip_yga_poligon.Text = "Poligon";
            this.toolStrip_yga_poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            this.toolStrip_yga_poligon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip_yga_poligon_MouseDown);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStrip_yga_nokta
            // 
            this.toolStrip_yga_nokta.AccessibleDescription = "";
            this.toolStrip_yga_nokta.AccessibleName = "";
            this.toolStrip_yga_nokta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.toolStrip_yga_nokta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_nokta.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_nokta.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_nokta.Image")));
            this.toolStrip_yga_nokta.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_nokta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_nokta.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStrip_yga_nokta.Name = "toolStrip_yga_nokta";
            this.toolStrip_yga_nokta.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_nokta.Size = new System.Drawing.Size(83, 24);
            this.toolStrip_yga_nokta.Tag = "";
            this.toolStrip_yga_nokta.Text = "Nokta";
            this.toolStrip_yga_nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStrip_yga_fonksiyon
            // 
            this.toolStrip_yga_fonksiyon.AccessibleDescription = "";
            this.toolStrip_yga_fonksiyon.AccessibleName = "";
            this.toolStrip_yga_fonksiyon.BackColor = System.Drawing.Color.LightBlue;
            this.toolStrip_yga_fonksiyon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStrip_yga_fonksiyon.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStrip_yga_fonksiyon.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_yga_fonksiyon.Image")));
            this.toolStrip_yga_fonksiyon.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStrip_yga_fonksiyon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_yga_fonksiyon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStrip_yga_fonksiyon.Name = "toolStrip_yga_fonksiyon";
            this.toolStrip_yga_fonksiyon.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStrip_yga_fonksiyon.Size = new System.Drawing.Size(125, 24);
            this.toolStrip_yga_fonksiyon.Tag = "";
            this.toolStrip_yga_fonksiyon.Text = "Fonksiyonlar";
            this.toolStrip_yga_fonksiyon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStrip_yga_fonksiyon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStrip_yga_fonksiyon.ToolTipText = "Çeşitli vektörel veya tabular algoritmaları içerir.";
            this.toolStrip_yga_fonksiyon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip_yga_fonksiyon_MouseDown);
            // 
            // checkBox_yga_11
            // 
            this.checkBox_yga_11.AutoSize = true;
            this.checkBox_yga_11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_11.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_11.Location = new System.Drawing.Point(7, 334);
            this.checkBox_yga_11.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_11.Name = "checkBox_yga_11";
            this.checkBox_yga_11.Size = new System.Drawing.Size(126, 21);
            this.checkBox_yga_11.TabIndex = 53;
            this.checkBox_yga_11.Text = "checkBox_yga_11";
            this.checkBox_yga_11.UseVisualStyleBackColor = true;
            this.checkBox_yga_11.Visible = false;
            // 
            // checkBox_yga_10
            // 
            this.checkBox_yga_10.AutoSize = true;
            this.checkBox_yga_10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_10.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_10.Location = new System.Drawing.Point(6, 307);
            this.checkBox_yga_10.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_10.Name = "checkBox_yga_10";
            this.checkBox_yga_10.Size = new System.Drawing.Size(128, 21);
            this.checkBox_yga_10.TabIndex = 52;
            this.checkBox_yga_10.Text = "checkBox_yga_10";
            this.checkBox_yga_10.UseVisualStyleBackColor = true;
            this.checkBox_yga_10.Visible = false;
            // 
            // checkBox_yga_9
            // 
            this.checkBox_yga_9.AutoSize = true;
            this.checkBox_yga_9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_9.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_9.Location = new System.Drawing.Point(6, 282);
            this.checkBox_yga_9.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_9.Name = "checkBox_yga_9";
            this.checkBox_yga_9.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_9.TabIndex = 51;
            this.checkBox_yga_9.Text = "checkBox_yga_9";
            this.checkBox_yga_9.UseVisualStyleBackColor = true;
            this.checkBox_yga_9.Visible = false;
            // 
            // checkBox_yga_8
            // 
            this.checkBox_yga_8.AutoSize = true;
            this.checkBox_yga_8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_8.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_8.Location = new System.Drawing.Point(6, 255);
            this.checkBox_yga_8.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_8.Name = "checkBox_yga_8";
            this.checkBox_yga_8.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_8.TabIndex = 50;
            this.checkBox_yga_8.Text = "checkBox_yga_8";
            this.checkBox_yga_8.UseVisualStyleBackColor = true;
            this.checkBox_yga_8.Visible = false;
            // 
            // checkBox_yga_5
            // 
            this.checkBox_yga_5.AutoSize = true;
            this.checkBox_yga_5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_5.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_5.Location = new System.Drawing.Point(6, 172);
            this.checkBox_yga_5.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_5.Name = "checkBox_yga_5";
            this.checkBox_yga_5.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_5.TabIndex = 47;
            this.checkBox_yga_5.Text = "checkBox_yga_5";
            this.checkBox_yga_5.UseVisualStyleBackColor = true;
            this.checkBox_yga_5.Visible = false;
            // 
            // checkBox_yga_7
            // 
            this.checkBox_yga_7.AutoSize = true;
            this.checkBox_yga_7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_7.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_7.Location = new System.Drawing.Point(6, 228);
            this.checkBox_yga_7.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_7.Name = "checkBox_yga_7";
            this.checkBox_yga_7.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_7.TabIndex = 49;
            this.checkBox_yga_7.Text = "checkBox_yga_7";
            this.checkBox_yga_7.UseVisualStyleBackColor = true;
            this.checkBox_yga_7.Visible = false;
            // 
            // checkBox_yga_1
            // 
            this.checkBox_yga_1.AutoSize = true;
            this.checkBox_yga_1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_yga_1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_1.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_1.Location = new System.Drawing.Point(6, 61);
            this.checkBox_yga_1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_1.Name = "checkBox_yga_1";
            this.checkBox_yga_1.Size = new System.Drawing.Size(121, 21);
            this.checkBox_yga_1.TabIndex = 43;
            this.checkBox_yga_1.Text = "checkBox_yga_1";
            this.checkBox_yga_1.UseVisualStyleBackColor = false;
            this.checkBox_yga_1.Visible = false;
            // 
            // checkBox_yga_6
            // 
            this.checkBox_yga_6.AutoSize = true;
            this.checkBox_yga_6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_6.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_6.Location = new System.Drawing.Point(6, 199);
            this.checkBox_yga_6.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_6.Name = "checkBox_yga_6";
            this.checkBox_yga_6.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_6.TabIndex = 48;
            this.checkBox_yga_6.Text = "checkBox_yga_6";
            this.checkBox_yga_6.UseVisualStyleBackColor = true;
            this.checkBox_yga_6.Visible = false;
            // 
            // checkBox_yga_2
            // 
            this.checkBox_yga_2.AutoSize = true;
            this.checkBox_yga_2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_2.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_2.Location = new System.Drawing.Point(6, 89);
            this.checkBox_yga_2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_2.Name = "checkBox_yga_2";
            this.checkBox_yga_2.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_2.TabIndex = 44;
            this.checkBox_yga_2.Text = "checkBox_yga_2";
            this.checkBox_yga_2.UseVisualStyleBackColor = true;
            this.checkBox_yga_2.Visible = false;
            // 
            // checkBox_yga_3
            // 
            this.checkBox_yga_3.AutoSize = true;
            this.checkBox_yga_3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_3.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_3.Location = new System.Drawing.Point(6, 116);
            this.checkBox_yga_3.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_3.Name = "checkBox_yga_3";
            this.checkBox_yga_3.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_3.TabIndex = 45;
            this.checkBox_yga_3.Text = "checkBox_yga_3";
            this.checkBox_yga_3.UseVisualStyleBackColor = true;
            this.checkBox_yga_3.Visible = false;
            // 
            // checkBox_yga_4
            // 
            this.checkBox_yga_4.AutoSize = true;
            this.checkBox_yga_4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yga_4.ForeColor = System.Drawing.Color.DarkOrange;
            this.checkBox_yga_4.Location = new System.Drawing.Point(6, 145);
            this.checkBox_yga_4.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_yga_4.Name = "checkBox_yga_4";
            this.checkBox_yga_4.Size = new System.Drawing.Size(123, 21);
            this.checkBox_yga_4.TabIndex = 46;
            this.checkBox_yga_4.Text = "checkBox_yga_4";
            this.checkBox_yga_4.UseVisualStyleBackColor = true;
            this.checkBox_yga_4.Visible = false;
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
            this.imageList.Images.SetKeyName(13, "");
            this.imageList.Images.SetKeyName(14, "Health Graph.ico");
            this.imageList.Images.SetKeyName(15, "Deviation.ico");
            this.imageList.Images.SetKeyName(16, "Graph Report2.ico");
            // 
            // Toolbox_EA
            // 
            this.Toolbox_EA.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbox_EA.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EA_Seç,
            this.EA_Kaydır,
            this.EA_Mesafe_Ölç,
            this.EA_Poligon,
            this.EA_Nokta});
            this.Toolbox_EA.Location = new System.Drawing.Point(0, 0);
            this.Toolbox_EA.Name = "Toolbox_EA";
            this.Toolbox_EA.Size = new System.Drawing.Size(1312, 32);
            this.Toolbox_EA.TabIndex = 32;
            this.Toolbox_EA.Text = "toolStrip2";
            // 
            // EA_Seç
            // 
            this.EA_Seç.AccessibleDescription = "";
            this.EA_Seç.AccessibleName = "";
            this.EA_Seç.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(250)))), ((int)(((byte)(249)))));
            this.EA_Seç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Seç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Seç.Image = ((System.Drawing.Image)(resources.GetObject("EA_Seç.Image")));
            this.EA_Seç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Seç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Seç.Margin = new System.Windows.Forms.Padding(204, 1, 0, 2);
            this.EA_Seç.Name = "EA_Seç";
            this.EA_Seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Seç.Size = new System.Drawing.Size(66, 29);
            this.EA_Seç.Tag = "";
            this.EA_Seç.Text = "Seç";
            this.EA_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            // 
            // EA_Kaydır
            // 
            this.EA_Kaydır.AccessibleDescription = "";
            this.EA_Kaydır.AccessibleName = "";
            this.EA_Kaydır.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.EA_Kaydır.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Kaydır.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Kaydır.Image = ((System.Drawing.Image)(resources.GetObject("EA_Kaydır.Image")));
            this.EA_Kaydır.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Kaydır.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Kaydır.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.EA_Kaydır.Name = "EA_Kaydır";
            this.EA_Kaydır.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Kaydır.Size = new System.Drawing.Size(85, 29);
            this.EA_Kaydır.Tag = "";
            this.EA_Kaydır.Text = "Kaydır";
            this.EA_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            // 
            // EA_Mesafe_Ölç
            // 
            this.EA_Mesafe_Ölç.AccessibleDescription = "";
            this.EA_Mesafe_Ölç.AccessibleName = "";
            this.EA_Mesafe_Ölç.BackColor = System.Drawing.Color.Honeydew;
            this.EA_Mesafe_Ölç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Mesafe_Ölç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Mesafe_Ölç.Image = ((System.Drawing.Image)(resources.GetObject("EA_Mesafe_Ölç.Image")));
            this.EA_Mesafe_Ölç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Mesafe_Ölç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Mesafe_Ölç.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.EA_Mesafe_Ölç.Name = "EA_Mesafe_Ölç";
            this.EA_Mesafe_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Mesafe_Ölç.Size = new System.Drawing.Size(117, 29);
            this.EA_Mesafe_Ölç.Tag = "";
            this.EA_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.EA_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            // 
            // EA_Poligon
            // 
            this.EA_Poligon.AccessibleDescription = "";
            this.EA_Poligon.AccessibleName = "";
            this.EA_Poligon.BackColor = System.Drawing.Color.Thistle;
            this.EA_Poligon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Poligon.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Poligon.Image = ((System.Drawing.Image)(resources.GetObject("EA_Poligon.Image")));
            this.EA_Poligon.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Poligon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Poligon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.EA_Poligon.Name = "EA_Poligon";
            this.EA_Poligon.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Poligon.Size = new System.Drawing.Size(93, 29);
            this.EA_Poligon.Tag = "";
            this.EA_Poligon.Text = "Poligon";
            this.EA_Poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            // 
            // EA_Nokta
            // 
            this.EA_Nokta.AccessibleDescription = "";
            this.EA_Nokta.AccessibleName = "";
            this.EA_Nokta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.EA_Nokta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Nokta.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Nokta.Image = ((System.Drawing.Image)(resources.GetObject("EA_Nokta.Image")));
            this.EA_Nokta.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Nokta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Nokta.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.EA_Nokta.Name = "EA_Nokta";
            this.EA_Nokta.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Nokta.Size = new System.Drawing.Size(83, 29);
            this.EA_Nokta.Tag = "";
            this.EA_Nokta.Text = "Nokta";
            this.EA_Nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            this.EA_Nokta.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EA_Nokta_MouseDown);
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
            this.EA_list_box.Location = new System.Drawing.Point(17, 271);
            this.EA_list_box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EA_list_box.Name = "EA_list_box";
            this.EA_list_box.Size = new System.Drawing.Size(244, 82);
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
            // ContextMenuStrip_Nokta
            // 
            this.ContextMenuStrip_Nokta.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Nokta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Nokta_Ekle,
            this.Nokta_Sil,
            this.Kaydet});
            this.ContextMenuStrip_Nokta.Name = "EA_ContextMenuStrip_Nokta";
            this.ContextMenuStrip_Nokta.Size = new System.Drawing.Size(135, 82);
            // 
            // Nokta_Ekle
            // 
            this.Nokta_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("Nokta_Ekle.Image")));
            this.Nokta_Ekle.Name = "Nokta_Ekle";
            this.Nokta_Ekle.Size = new System.Drawing.Size(134, 26);
            this.Nokta_Ekle.Text = "Nokta Ekle";
            this.Nokta_Ekle.Click += new System.EventHandler(this.Nokta_Ekle_Click);
            // 
            // Nokta_Sil
            // 
            this.Nokta_Sil.Image = ((System.Drawing.Image)(resources.GetObject("Nokta_Sil.Image")));
            this.Nokta_Sil.Name = "Nokta_Sil";
            this.Nokta_Sil.Size = new System.Drawing.Size(134, 26);
            this.Nokta_Sil.Text = "Nokta Sil";
            // 
            // Kaydet
            // 
            this.Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Kaydet.Image")));
            this.Kaydet.Name = "Kaydet";
            this.Kaydet.Size = new System.Drawing.Size(134, 26);
            this.Kaydet.Text = "Kaydet";
            // 
            // ContextMenuStrip_Poligon
            // 
            this.ContextMenuStrip_Poligon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Poligon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Poligon_Çiz,
            this.Poligon_Sil,
            this.Poligon_Kaydet});
            this.ContextMenuStrip_Poligon.Name = "ContextMenuStrip_Poligon";
            this.ContextMenuStrip_Poligon.Size = new System.Drawing.Size(159, 82);
            // 
            // Poligon_Çiz
            // 
            this.Poligon_Çiz.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Çiz.Image")));
            this.Poligon_Çiz.Name = "Poligon_Çiz";
            this.Poligon_Çiz.Size = new System.Drawing.Size(158, 26);
            this.Poligon_Çiz.Text = "Poligon Çiz";
            this.Poligon_Çiz.Click += new System.EventHandler(this.Poligon_Çiz_Click);
            // 
            // Poligon_Sil
            // 
            this.Poligon_Sil.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Sil.Image")));
            this.Poligon_Sil.Name = "Poligon_Sil";
            this.Poligon_Sil.Size = new System.Drawing.Size(158, 26);
            this.Poligon_Sil.Text = "Poligon Sil";
            this.Poligon_Sil.Click += new System.EventHandler(this.Poligon_Sil_Click);
            // 
            // Poligon_Kaydet
            // 
            this.Poligon_Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Kaydet.Image")));
            this.Poligon_Kaydet.Name = "Poligon_Kaydet";
            this.Poligon_Kaydet.Size = new System.Drawing.Size(158, 26);
            this.Poligon_Kaydet.Text = "Poligon Kaydet";
            this.Poligon_Kaydet.Click += new System.EventHandler(this.Poligon_Kaydet_Click);
            // 
            // ContextMenuStrip_Fonksiyon
            // 
            this.ContextMenuStrip_Fonksiyon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Fonksiyon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.katman_birleştir,
            this.overlap_analizi});
            this.ContextMenuStrip_Fonksiyon.Name = "ContextMenuStrip_Fonksiyon";
            this.ContextMenuStrip_Fonksiyon.Size = new System.Drawing.Size(162, 56);
            // 
            // katman_birleştir
            // 
            this.katman_birleştir.Image = ((System.Drawing.Image)(resources.GetObject("katman_birleştir.Image")));
            this.katman_birleştir.Name = "katman_birleştir";
            this.katman_birleştir.Size = new System.Drawing.Size(161, 26);
            this.katman_birleştir.Text = "Katman Birleştir";
            this.katman_birleştir.Click += new System.EventHandler(this.katman_birleştir_Click);
            // 
            // overlap_analizi
            // 
            this.overlap_analizi.Image = ((System.Drawing.Image)(resources.GetObject("overlap_analizi.Image")));
            this.overlap_analizi.Name = "overlap_analizi";
            this.overlap_analizi.Size = new System.Drawing.Size(161, 26);
            this.overlap_analizi.Text = "Overlap Analizi";
            // 
            // ModuleTabPanel
            // 
            this.ModuleTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModuleTabPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.ModuleTabPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ModuleTabPanel.Controls.Add(this.Modül_Tabları);
            this.ModuleTabPanel.Location = new System.Drawing.Point(0, 31);
            this.ModuleTabPanel.Name = "ModuleTabPanel";
            this.ModuleTabPanel.Size = new System.Drawing.Size(1368, 598);
            this.ModuleTabPanel.TabIndex = 5;
            // 
            // gMapControl_optimal_dtr
            // 
            this.gMapControl_optimal_dtr.AllowDrop = true;
            this.gMapControl_optimal_dtr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_optimal_dtr.Bearing = 0F;
            this.gMapControl_optimal_dtr.CanDragMap = true;
            this.gMapControl_optimal_dtr.Cursor = System.Windows.Forms.Cursors.Default;
            this.gMapControl_optimal_dtr.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_optimal_dtr.GrayScaleMode = false;
            this.gMapControl_optimal_dtr.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_optimal_dtr.LevelsKeepInMemory = 5;
            this.gMapControl_optimal_dtr.Location = new System.Drawing.Point(294, 38);
            this.gMapControl_optimal_dtr.Margin = new System.Windows.Forms.Padding(2);
            this.gMapControl_optimal_dtr.MarkersEnabled = true;
            this.gMapControl_optimal_dtr.MaxZoom = 2;
            this.gMapControl_optimal_dtr.MinZoom = 2;
            this.gMapControl_optimal_dtr.MouseWheelZoomEnabled = true;
            this.gMapControl_optimal_dtr.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl_optimal_dtr.Name = "gMapControl_optimal_dtr";
            this.gMapControl_optimal_dtr.NegativeMode = false;
            this.gMapControl_optimal_dtr.PolygonsEnabled = true;
            this.gMapControl_optimal_dtr.RetryLoadTile = 0;
            this.gMapControl_optimal_dtr.RoutesEnabled = true;
            this.gMapControl_optimal_dtr.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl_optimal_dtr.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl_optimal_dtr.ShowTileGridLines = false;
            this.gMapControl_optimal_dtr.Size = new System.Drawing.Size(736, 519);
            this.gMapControl_optimal_dtr.TabIndex = 31;
            this.gMapControl_optimal_dtr.Zoom = 0D;
            // 
            // Seç_Stokastik
            // 
            this.Seç_Stokastik.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Seç_Stokastik.Location = new System.Drawing.Point(0, 0);
            this.Seç_Stokastik.Name = "Seç_Stokastik";
            this.Seç_Stokastik.Size = new System.Drawing.Size(100, 25);
            this.Seç_Stokastik.TabIndex = 0;
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.HeaderPanel.Controls.Add(this.HomePageButton);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1368, 31);
            this.HeaderPanel.TabIndex = 6;
            // 
            // HomePageButton
            // 
            this.HomePageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HomePageButton.BackColor = System.Drawing.Color.Transparent;
            this.HomePageButton.BackgroundColor = System.Drawing.Color.Transparent;
            this.HomePageButton.BackgroundImage = global::SLF.Properties.Resources.Homen;
            this.HomePageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HomePageButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.HomePageButton.BorderRadius = 0;
            this.HomePageButton.BorderSize = 0;
            this.HomePageButton.FlatAppearance.BorderSize = 0;
            this.HomePageButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGreen;
            this.HomePageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HomePageButton.ForeColor = System.Drawing.Color.White;
            this.HomePageButton.Location = new System.Drawing.Point(1329, 3);
            this.HomePageButton.Name = "HomePageButton";
            this.HomePageButton.Size = new System.Drawing.Size(36, 25);
            this.HomePageButton.TabIndex = 5;
            this.HomePageButton.TextColor = System.Drawing.Color.White;
            this.HomePageButton.UseVisualStyleBackColor = false;
            this.HomePageButton.Click += new System.EventHandler(this.HomePageButton_Click);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1368, 629);
            this.Controls.Add(this.ModuleTabPanel);
            this.Controls.Add(this.HeaderPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(1154, 666);
            this.Name = "ModülFormu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jeo-Uzamsal Talep Tahmini Yazılımı           ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModülFormu_FormClosing);
            this.Load += new System.EventHandler(this.ModülFormu_Load);
            this.Modül_Tabları.ResumeLayout(false);
            this.tab_girdi.ResumeLayout(false);
            this.tab_girdi.PerformLayout();
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
            this.tab_ekonometrik.ResumeLayout(false);
            this.ELFResultsTabControls.ResumeLayout(false);
            this.ELFMinResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinResultsTable)).EndInit();
            this.ELFLowResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowResultsTable)).EndInit();
            this.ELFBaseResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseResultsTable)).EndInit();
            this.ELFHighResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighResultsTable)).EndInit();
            this.ELFMaxResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxResultsTable)).EndInit();
            this.ELFGraphicOutputsTabPage.ResumeLayout(false);
            this.panel_ELF_Grafikler.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_1)).EndInit();
            this.ELFGraphicsPanel.ResumeLayout(false);
            this.ELFGraphicsPanel.PerformLayout();
            this.tab_imar.ResumeLayout(false);
            this.tab_imar.PerformLayout();
            this.katmanlar_right_click.ResumeLayout(false);
            this.panel_imar.ResumeLayout(false);
            this.panel_imar.PerformLayout();
            this.harita_katmanları_right_click.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).EndInit();
            this.toolStrip_imar.ResumeLayout(false);
            this.toolStrip_imar.PerformLayout();
            this.tab_optDTR.ResumeLayout(false);
            this.tab_optDTR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_optimalDTR)).EndInit();
            this.tab_senaryo.ResumeLayout(false);
            this.SenaryoModulePanel.ResumeLayout(false);
            this.EkonometrikSenaryoElementsPanel.ResumeLayout(false);
            this.EkonometrikSenaryoElementsPanel.PerformLayout();
            this.SenaryoModuleTabControl.ResumeLayout(false);
            this.EkonometrikSenaryoTabPage.ResumeLayout(false);
            this.EkonometrikSenaryoOutputsPanel.ResumeLayout(false);
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
            this.tab_stokastik.ResumeLayout(false);
            this.tab_stokastik.PerformLayout();
            this.panel_stokastik.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView_stokastik)).EndInit();
            this.toolStrip_stokastik.ResumeLayout(false);
            this.toolStrip_stokastik.PerformLayout();
            this.tab_yükHaritası.ResumeLayout(false);
            this.tab_yükHaritası.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).EndInit();
            this.toolStrip_yuk.ResumeLayout(false);
            this.toolStrip_yuk.PerformLayout();
            this.tab_yga.ResumeLayout(false);
            this.tab_yga.PerformLayout();
            this.panel_yga.ResumeLayout(false);
            this.panel_yga.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yga)).EndInit();
            this.toolStrip_yga.ResumeLayout(false);
            this.toolStrip_yga.PerformLayout();
            this.Toolbox_EA.ResumeLayout(false);
            this.Toolbox_EA.PerformLayout();
            this.ContextMenuStrip_Nokta.ResumeLayout(false);
            this.ContextMenuStrip_Poligon.ResumeLayout(false);
            this.ContextMenuStrip_Fonksiyon.ResumeLayout(false);
            this.ModuleTabPanel.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoGraphPicBox)).EndInit();
            this.ResumeLayout(false);

        }

        private void btnTamamla_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnplgn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        public System.Windows.Forms.TabControl Modül_Tabları;
        public System.Windows.Forms.TabPage tab_stokastik;
        private System.Windows.Forms.TabPage tab_girdi;
        private System.Windows.Forms.TabPage tab_ekonometrik;
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
        private System.Windows.Forms.TabPage tab_validasyon;
        private System.Windows.Forms.Panel ELFTablePanel;
        private System.Windows.Forms.Button stokastik_dosya_seçimi;
        private System.Windows.Forms.Label label_stokastik_katmanlar;
        private System.Windows.Forms.CheckBox checkBox_stokastik_10;
        private System.Windows.Forms.CheckBox checkBox_stokastik_9;
        private System.Windows.Forms.CheckBox checkBox_stokastik_8;
        private System.Windows.Forms.CheckBox checkBox_stokastik_7;
        private System.Windows.Forms.CheckBox checkBox_stokastik_6;
        private System.Windows.Forms.CheckBox checkBox_stokastik_5;
        private System.Windows.Forms.CheckBox checkBox_stokastik_4;
        private System.Windows.Forms.CheckBox checkBox_stokastik_3;
        private System.Windows.Forms.CheckBox checkBox_stokastik_2;
        private System.Windows.Forms.CheckBox checkBox_stokastik_1;
        private System.Windows.Forms.ContextMenuStrip katmanlar_right_click;
        private System.Windows.Forms.ToolStripMenuItem tabloyuGörToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rengiDeğiştirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temizleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kaydetToolStripMenuItem;
        private System.Windows.Forms.Button buton_stokastik_harita_katmanlar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_stokastik;
        private System.Windows.Forms.ContextMenuStrip harita_katmanları_right_click;
        private System.Windows.Forms.ToolStripMenuItem Arazi;
        private System.Windows.Forms.ToolStripMenuItem Harita;
        private System.Windows.Forms.ToolStripMenuItem Uydu;
        private System.Windows.Forms.ToolStripMenuItem Google_Earth;
        private System.Windows.Forms.ToolStripMenuItem OSM;
        public System.Windows.Forms.Label mesafe_metre_stokastik;
        private System.Windows.Forms.Label Mesafe_stokastik;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Nokta;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Ekle;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Sil;
        private System.Windows.Forms.ToolStripMenuItem Kaydet;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Poligon;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Çiz;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Sil;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Kaydet;
        private ContextMenuStrip ContextMenuStrip_Fonksiyon;
        private ToolStripMenuItem katman_birleştir;
        private ToolStripMenuItem overlap_analizi;
        private CheckBox checkBox_stokastik_13;
        private CheckBox checkBox_stokastik_12;
        private CheckBox checkBox_stokastik_11;
        private ToolStripMenuItem Google_Earth_Desktop;
        private ToolStrip toolStrip_stokastik;
        private ToolStripButton Stokastik_Seç;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton Stokastik_Kaydır;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton Stokastik_Mesafe_Ölç;
        private ToolStripButton Stokastik_Poligon;
        private ToolStripButton Stokastik_Nokta;
        private ToolStripButton Stokastik_Grid_Oluştur;
        private ToolStripButton Stokastik_Fonksiyonlar;
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
        private TabPage tab_senaryo;
        private Button SenaryoSelectionButton;
        private Label label_girdi_rapor;
        private Label label_dısa_aktar;
        private Label label_girdi_veri_onizleme;
        private Panel ELFGraphicsPanel;
        private Guna.UI2.WinForms.Guna2TabControl SenaryoModuleTabControl;
        private TabPage EkonometrikSenaryoTabPage;
        private Panel SenaryoModulePanel;
        private Panel EkonometrikSenaryoOutputsPanel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel EkonometrikSenaryoElementsPanel;
        private TabControl ELFSenaryoTabControls;
        private TabPage tabPage_min_senaryo;
        private DataGridView ELFMinSenaryoTable;
        private TabPage tabPage_dusuk_senaryo;
        private TabPage tabPage_baz_senaryo;
        private TabPage tabPage_yuksek_senaryo;
        private TabPage tabPage_maks_senaryo;
        private DataGridView ELFLowSenaryoTable;
        private DataGridView ELFBaseSenaryoTable;
        private DataGridView ELFHighSenaryoTable;
        private DataGridView ELFMaxSenaryoTable;
        private Button ELFPredictionButton;
        private PictureBox ELFMinSenaryoGraphPicBox;
        private Panel ELFRadioButtonsPanel;
        private TabControl ELFResultsTabControls;
        private TabPage ELFMinResultsTabPage;
        private DataGridView ELFMinResultsTable;
        private TabPage ELFLowResultsTabPage;
        private DataGridView ELFLowResultsTable;
        private TabPage ELFBaseResultsTabPage;
        private DataGridView ELFBaseResultsTable;
        private TabPage ELFHighResultsTabPage;
        private DataGridView ELFHighResultsTable;
        private TabPage ELFMaxResultsTabPage;
        private DataGridView ELFMaxResultsTable;
        private RichTextBox richTextBox_senaryolar_ELF;
        private ToolStripMenuItem Sokak_Görünümü;
        private Button buton_yuk_haritası_katmanlar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_yuk;
        public GMap.NET.WindowsForms.GMapControl gMapControl_imar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_imar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_optimalDTR;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_optimalDTR;
        private ToolStrip toolStrip_imar;
        private ToolStripButton İmar_Seç;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton İmar_Kaydır;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripButton İmar_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton İmar_Poligon;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripButton İmar_Nokta;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripButton İmar_Grid_Oluştur;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripButton İmar_Fonksiyonlar;
        private Button buton_ea_harita_katmanlar;
        private ToolStrip Toolbox_EA;
        private Button ButtonKml;
        private Button oznitelikAc;
        private ListBox EA_list_box;
        public GMap.NET.WindowsForms.GMapControl gMapControl_EA;
        private ToolStrip toolStrip_yuk;
        private ToolStripButton Yuk_Seç;
        private ToolStripButton Yuk_Kaydır;
        private ToolStripButton Yuk_Mesafe_Ölç;
        private Label mesafe_metre_yuk;
        private Label Mesafe_yuk;
        private CheckBox checkBox_imar_8;
        private CheckBox checkBox_imar_9;
        private CheckBox checkBox_imar_10;
        private CheckBox checkBox_imar_11;
        private CheckBox checkBox_imar_12;
        private CheckBox checkBox_AC_Home;
        private CheckBox checkBox_AC_Work;
        private CheckBox checkBox_AC_Public;
        private CheckBox checkBox_DC_Fast;
        private CheckBox checkBox_imar_13;
        private Button imar_dosya_seçimi;
        private Label Mesafe_imar;
        private Label mesafe_metre_imar;
        private TrackBar trackBar_Yıllar;
        private Label yuk_yıl_deger;
        private Label yuk_yıl_text;
        public Panel legendPanel;
        public System.Windows.Forms.TabPage tab_ea;
        private System.Windows.Forms.Label label_imar_katmanlar;
        private System.Windows.Forms.CheckBox checkBox_imar_5;
        private System.Windows.Forms.CheckBox checkBox_imar_4;
        private System.Windows.Forms.CheckBox checkBox_imar_3;
        private System.Windows.Forms.CheckBox checkBox_imar_2;
        private System.Windows.Forms.CheckBox checkBox_imar_1;
        private System.Windows.Forms.CheckBox checkBox_imar_7;
        private System.Windows.Forms.CheckBox checkBox_imar_6;
        private System.Windows.Forms.Label SenaryoResultsLabel;
        private System.Windows.Forms.ToolStripButton EA_Seç;
        private System.Windows.Forms.ToolStripButton EA_Kaydır;
        private System.Windows.Forms.ToolStripButton EA_Mesafe_Ölç;
        private System.Windows.Forms.ToolStripButton EA_Poligon;
        private System.Windows.Forms.ToolStripButton EA_Nokta;
        private System.Windows.Forms.Label mesafe_metre_DeK;
        private System.Windows.Forms.Label Mesafe_Dek;
        private ToolStrip Seç_Stokastik;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Label FutureSimLabel;
        private ComboBox comboBox_ea_il_secimi;
        private ComboBox comboBox_ea_yıl_secimi;
        private Panel GelecekSimPanel;
        private Panel panel_ea;
        private TabPage ELFGraphicOutputsTabPage;
        private Panel panel_DEK;
        private ComboBox comboBox_DEK_il;
        private ComboBox comboBox_DEK_Yıl;
        private Label label_DEK_Gelecek;
        private CheckBox checkBox27;
        public GMap.NET.WindowsForms.GMapControl gMapControl_optimal_dtr;
        private ImageList imageList;
        private CheckBox checkBox_optDTR_Eskişehir;
        private CheckBox checkBox_optDTR_İzmir;
        public GMap.NET.WindowsForms.GMapControl gMapControl_DEK;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripSeparator toolStripSeparator8;
        private Panel panel_ELF_Grafikler;
        private PictureBox pictureBox_ELF_5;
        private PictureBox pictureBox_ELF_4;
        private PictureBox pictureBox_ELF_3;
        private PictureBox pictureBox_ELF_2;
        private PictureBox pictureBox_ELF_1;
        private Panel HeaderPanel;
        private TabPage YeniGenislemeSenaryoTabPage;
        private CustomButton OpenModuleButton;
        private TabPage tab_yga;
        private CustomButton ELFScenerioSaveButton;
        private CustomButton ELFPredictionShowResultsButton;
        private CustomButton ShowResultsButton;
        private CustomButton SenaryoNewSelectionButton;
        private CustomButton ELFShowGraphsButton;
        private CustomButton EASimButton;
        private CustomButton DEKSimButton;
        private CustomButton DEKCenterAddButton;
        private Panel EAStationsLegendPanel;
        private RadioButton dekSimMaxBtn;
        private RadioButton dekSimDefBtn;
        private RadioButton dekSimMinBtn;
        private RadioButton EaSimMaxBtn;
        private RadioButton EaSimDefBtn;
        private RadioButton EaSimMinBtn;
        private CustomButton HomePageButton;
        private Button FinishPolygonButton;
        private CheckBox checkBox_yga_13;
        private CheckBox checkBox_yga_12;
        private CheckBox checkBox_yga_11;
        private CheckBox checkBox_yga_10;
        private CheckBox checkBox_yga_9;
        private CheckBox checkBox_yga_8;
        private CheckBox checkBox_yga_7;
        private CheckBox checkBox_yga_6;
        private CheckBox checkBox_yga_5;
        private CheckBox checkBox_yga_4;
        private CheckBox checkBox_yga_3;
        private CheckBox checkBox_yga_2;
        private CheckBox checkBox_yga_1;
        private Label mesafe_metre_yga;
        private Label Mesafe_yga;
        private ToolStrip miniToolStrip;
        private ToolStrip toolStrip_yga;
        private ToolStripButton toolStrip_yga_seç;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripButton toolStrip_yga_kaydır;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripButton toolStrip_yga_mesafe;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripButton toolStrip_yga_poligon;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripButton toolStrip_yga_nokta;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripButton toolStrip_yga_fonksiyon;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_yga;
        public GMap.NET.WindowsForms.GMapControl gMapControl_yga;
        private Button buton_yga_harita_katmanlar;
        private Label label_yga_katmanlar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_stokastik;
        private Panel panel_stokastik;
        private Panel panel_yga;
        private Panel panel_imar;
        private Button buton_imar_katmanlar;
        private CheckBox checkBox_stokastik_15;
        private CheckBox checkBox_stokastik_14;
        private CheckBox checkBox_imar_15;
        private CheckBox checkBox_imar_14;
        private CheckBox checkBox_yga_15;
        private CheckBox checkBox_yga_14;
        private Button buton_dosya_yga;
        private CustomButton EANewSimulationResultsButton;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Label RModelStatusLabel;
        private ProgressBar RModelProgressBar;
        private ComboBox comboBox_ea_ilce_secimi;
        private Label DCFastLegendValueLabel;
        private Label DCFastLegendLabel;
        private Label ACPublicLegendValueLabel;
        private Label ACPublicLegendLabel;
        private Label AddStationLabel;
        private Label ACWorkLegendValueLabel;
        private CustomButton EAStationAddButton;
        private Label ACHomeLegendLabel;
        private Label ACWorkLegendLabel;
        private Label ACHomeLegendValueLabel;
        private CustomButton SimulasyonSonucGoruntule;
        private CheckBox EAPointsLayerCheckBox;
        private ComboBox comboBox_dek_ilce_secimi;
        private Panel panel1;
        private CheckBox DEKPointsLayerCheckBox;
        private ProgressBar DEKProgressBar;
        private Label DEKStatusLabel;
        private CustomButton DEKRunSimulationButton;
        private CustomButton DEKSimulasyonSonucGoruntule;
    }
}
