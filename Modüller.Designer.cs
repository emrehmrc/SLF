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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModülFormu));
            this.Modül_Tabları = new System.Windows.Forms.TabControl();
            this.tab_girdi = new System.Windows.Forms.TabPage();
            this.buton_proje_sec = new System.Windows.Forms.Button();
            this.label_girdi_veri_onizleme = new System.Windows.Forms.Label();
            this.panel_girdi_rapor_olustur = new System.Windows.Forms.Panel();
            this.label_girdi_rapor = new System.Windows.Forms.Label();
            this.dataGridView_girdi = new System.Windows.Forms.DataGridView();
            this.panel_girdi_dısa_aktar = new System.Windows.Forms.Panel();
            this.label_dısa_aktar = new System.Windows.Forms.Label();
            this.panel_girdi_yıl_secimi = new System.Windows.Forms.Panel();
            this.startYearComboBox = new System.Windows.Forms.ComboBox();
            this.yearApproveButton = new System.Windows.Forms.Button();
            this.endYearComboBox = new System.Windows.Forms.ComboBox();
            this.panel_girdi_dosya_secimi = new System.Windows.Forms.Panel();
            this.veri_listesi_seçimi = new System.Windows.Forms.ComboBox();
            this.label_girdi_veri_tipi_secimi = new System.Windows.Forms.Label();
            this.label_girdi_dosya_secimi = new System.Windows.Forms.Label();
            this.tab_dek = new System.Windows.Forms.TabPage();
            this.gMapControl_DEK = new GMap.NET.WindowsForms.GMapControl();
            this.panel_DEK = new System.Windows.Forms.Panel();
            this.dekSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.dekSimDefBtn = new System.Windows.Forms.RadioButton();
            this.dekSimMinBtn = new System.Windows.Forms.RadioButton();
            this.label_DEK_Gelecek = new System.Windows.Forms.Label();
            this.comboBox_DEK_il = new System.Windows.Forms.ComboBox();
            this.comboBox_DEK_Yıl = new System.Windows.Forms.ComboBox();
            this.tab_ea = new System.Windows.Forms.TabPage();
            this.EAStationsLegendPanel = new System.Windows.Forms.Panel();
            this.DCFastLegendValueLabel = new System.Windows.Forms.Label();
            this.DCFastLegendLabel = new System.Windows.Forms.Label();
            this.ACPublicLegendValueLabel = new System.Windows.Forms.Label();
            this.ACPublicLegendLabel = new System.Windows.Forms.Label();
            this.ACWorkLegendValueLabel = new System.Windows.Forms.Label();
            this.ACWorkLegendLabel = new System.Windows.Forms.Label();
            this.ACHomeLegendValueLabel = new System.Windows.Forms.Label();
            this.ACHomeLegendLabel = new System.Windows.Forms.Label();
            this.AddStationLabel = new System.Windows.Forms.Label();
            this.panel_ea = new System.Windows.Forms.Panel();
            this.checkBox_DC_Fast = new System.Windows.Forms.CheckBox();
            this.checkBox_AC_Public = new System.Windows.Forms.CheckBox();
            this.checkBox_AC_Home = new System.Windows.Forms.CheckBox();
            this.checkBox_AC_Work = new System.Windows.Forms.CheckBox();
            this.GelecekSimPanel = new System.Windows.Forms.Panel();
            this.EaSimMaxBtn = new System.Windows.Forms.RadioButton();
            this.EaSimDefBtn = new System.Windows.Forms.RadioButton();
            this.EaSimMinBtn = new System.Windows.Forms.RadioButton();
            this.FutureSimLabel = new System.Windows.Forms.Label();
            this.comboBox_ea_il_secimi = new System.Windows.Forms.ComboBox();
            this.comboBox_ea_yıl_secimi = new System.Windows.Forms.ComboBox();
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
            this.ELFTablePanel = new System.Windows.Forms.Panel();
            this.ELFGraphicsPanel = new System.Windows.Forms.Panel();
            this.SenaryoResultsLabel = new System.Windows.Forms.Label();
            this.tab_imar = new System.Windows.Forms.TabPage();
            this.panel_imar = new System.Windows.Forms.Panel();
            this.Mesafe_imar = new System.Windows.Forms.Label();
            this.harita_katmanları_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mesafe_metre_imar = new System.Windows.Forms.Label();
            this.gMapControl_imar = new GMap.NET.WindowsForms.GMapControl();
            this.webView_imar = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.checkBox_imar_15 = new System.Windows.Forms.CheckBox();
            this.katmanlar_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.tab_optDTR = new System.Windows.Forms.TabPage();
            this.gMapControl_optimalDTR = new GMap.NET.WindowsForms.GMapControl();
            this.webView_optimalDTR = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.checkBox_optDTR_Eskişehir = new System.Windows.Forms.CheckBox();
            this.checkBox_optDTR_İzmir = new System.Windows.Forms.CheckBox();
            this.tab_senaryo = new System.Windows.Forms.TabPage();
            this.SenaryoModulePanel = new System.Windows.Forms.Panel();
            this.EkonometrikSenaryoElementsPanel = new System.Windows.Forms.Panel();
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
            this.tab_yükHaritası = new System.Windows.Forms.TabPage();
            this.webView_yuk = new Microsoft.Web.WebView2.WinForms.WebView2();
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
            this.label1 = new System.Windows.Forms.Label();
            this.yuk_yıl_deger = new System.Windows.Forms.Label();
            this.yuk_yıl_text = new System.Windows.Forms.Label();
            this.trackBar_Yıllar = new System.Windows.Forms.TrackBar();
            this.Mesafe_yuk = new System.Windows.Forms.Label();
            this.mesafe_metre_yuk = new System.Windows.Forms.Label();
            this.toolStrip_yuk = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.gMapControl_yuk = new GMap.NET.WindowsForms.GMapControl();
            this.tab_rapor = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.Toolbox_EA = new System.Windows.Forms.ToolStrip();
            this.ButtonKml = new System.Windows.Forms.Button();
            this.oznitelikAc = new System.Windows.Forms.Button();
            this.EA_list_box = new System.Windows.Forms.ListBox();
            this.ELFRadioButtonsPanel = new System.Windows.Forms.Panel();
            this.ELFPredictionButton = new System.Windows.Forms.Button();
            this.SenaryoSelectionButton = new System.Windows.Forms.Button();
            this.ContextMenuStrip_Nokta = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenuStrip_Poligon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenuStrip_Fonksiyon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ModuleTabPanel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.gMapControl_optimal_dtr = new GMap.NET.WindowsForms.GMapControl();
            this.Seç_Stokastik = new System.Windows.Forms.ToolStrip();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.OpenModuleButton = new SLF.CustomButton();
            this.DEKCenterAddButton = new SLF.CustomButton();
            this.DEKSimButton = new SLF.CustomButton();
            this.EAStationAddButton = new SLF.CustomButton();
            this.EASimButton = new SLF.CustomButton();
            this.ELFShowGraphsButton = new SLF.CustomButton();
            this.SenaryoNewSelectionButton = new SLF.CustomButton();
            this.ShowResultsButton = new SLF.CustomButton();
            this.ELFPredictionShowResultsButton = new SLF.CustomButton();
            this.ELFScenerioSaveButton = new SLF.CustomButton();
            this.raporGoruntuleButonu = new System.Windows.Forms.Button();
            this.ExcelDownloadButton = new System.Windows.Forms.Button();
            this.csvExportButton = new System.Windows.Forms.Button();
            this.SelectFolderButton = new System.Windows.Forms.Button();
            this.pictureBox_ELF_5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_3 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ELF_1 = new System.Windows.Forms.PictureBox();
            this.buton_imar_katmanlar = new System.Windows.Forms.Button();
            this.tabloyuGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rengiDeğiştirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temizleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yenidenAdlandırToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.İmar_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.İmar_Poligon = new System.Windows.Forms.ToolStripButton();
            this.İmar_Nokta = new System.Windows.Forms.ToolStripButton();
            this.İmar_Grid_Oluştur = new System.Windows.Forms.ToolStripButton();
            this.İmar_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.Yuk_Seç = new System.Windows.Forms.ToolStripButton();
            this.Yuk_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.Yuk_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.buton_yuk_haritası_katmanlar = new System.Windows.Forms.Button();
            this.HomePageButton = new SLF.CustomButton();
            this.Arazi = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth_Desktop = new System.Windows.Forms.ToolStripMenuItem();
            this.Harita = new System.Windows.Forms.ToolStripMenuItem();
            this.OSM = new System.Windows.Forms.ToolStripMenuItem();
            this.Sokak_Görünümü = new System.Windows.Forms.ToolStripMenuItem();
            this.Uydu = new System.Windows.Forms.ToolStripMenuItem();
            this.EA_Seç = new System.Windows.Forms.ToolStripButton();
            this.EA_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.EA_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.EA_Poligon = new System.Windows.Forms.ToolStripButton();
            this.EA_Nokta = new System.Windows.Forms.ToolStripButton();
            this.Nokta_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.Nokta_Sil = new System.Windows.Forms.ToolStripMenuItem();
            this.Kaydet = new System.Windows.Forms.ToolStripMenuItem();
            this.Point_Load_Çiz = new System.Windows.Forms.ToolStripMenuItem();
            this.YGA_Çiz = new System.Windows.Forms.ToolStripMenuItem();
            this.Poligon_Sil = new System.Windows.Forms.ToolStripMenuItem();
            this.Poligon_Kaydet = new System.Windows.Forms.ToolStripMenuItem();
            this.katman_birleştir = new System.Windows.Forms.ToolStripMenuItem();
            this.overlap_analizi = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ELFGraphicsPanel.SuspendLayout();
            this.tab_imar.SuspendLayout();
            this.panel_imar.SuspendLayout();
            this.harita_katmanları_right_click.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).BeginInit();
            this.katmanlar_right_click.SuspendLayout();
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
            this.tab_yükHaritası.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).BeginInit();
            this.legendPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).BeginInit();
            this.toolStrip_yuk.SuspendLayout();
            this.Toolbox_EA.SuspendLayout();
            this.ContextMenuStrip_Nokta.SuspendLayout();
            this.ContextMenuStrip_Poligon.SuspendLayout();
            this.ContextMenuStrip_Fonksiyon.SuspendLayout();
            this.ModuleTabPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_1)).BeginInit();
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
            this.Modül_Tabları.Size = new System.Drawing.Size(1593, 728);
            this.Modül_Tabları.TabIndex = 2;
            this.Modül_Tabları.SelectedIndexChanged += new System.EventHandler(this.Modül_Tabları_SelectedIndexChanged);
            // 
            // tab_girdi
            // 
            this.tab_girdi.AutoScroll = true;
            this.tab_girdi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tab_girdi.Controls.Add(this.buton_proje_sec);
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
            this.tab_girdi.Size = new System.Drawing.Size(1585, 668);
            this.tab_girdi.TabIndex = 0;
            this.tab_girdi.Text = "Girdi Modülü";
            this.tab_girdi.UseVisualStyleBackColor = true;
            // 
            // buton_proje_sec
            // 
            this.buton_proje_sec.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buton_proje_sec.Location = new System.Drawing.Point(924, 5);
            this.buton_proje_sec.Name = "buton_proje_sec";
            this.buton_proje_sec.Size = new System.Drawing.Size(105, 65);
            this.buton_proje_sec.TabIndex = 20;
            this.buton_proje_sec.Text = "Hazır Proje Seç";
            this.buton_proje_sec.UseVisualStyleBackColor = true;
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
            this.panel_girdi_rapor_olustur.Location = new System.Drawing.Point(1357, 9);
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
            this.dataGridView_girdi.Location = new System.Drawing.Point(3, 113);
            this.dataGridView_girdi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView_girdi.Name = "dataGridView_girdi";
            this.dataGridView_girdi.ReadOnly = true;
            this.dataGridView_girdi.RowHeadersWidth = 18;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.dataGridView_girdi.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_girdi.RowTemplate.Height = 24;
            this.dataGridView_girdi.Size = new System.Drawing.Size(1528, 485);
            this.dataGridView_girdi.TabIndex = 4;
            // 
            // panel_girdi_dısa_aktar
            // 
            this.panel_girdi_dısa_aktar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_girdi_dısa_aktar.Controls.Add(this.label_dısa_aktar);
            this.panel_girdi_dısa_aktar.Controls.Add(this.ExcelDownloadButton);
            this.panel_girdi_dısa_aktar.Controls.Add(this.csvExportButton);
            this.panel_girdi_dısa_aktar.Location = new System.Drawing.Point(1144, 9);
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
            this.panel_girdi_dosya_secimi.Location = new System.Drawing.Point(447, 5);
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
            // tab_dek
            // 
            this.tab_dek.Controls.Add(this.gMapControl_DEK);
            this.tab_dek.Controls.Add(this.panel_DEK);
            this.tab_dek.ImageIndex = 0;
            this.tab_dek.Location = new System.Drawing.Point(4, 56);
            this.tab_dek.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_dek.Name = "tab_dek";
            this.tab_dek.Size = new System.Drawing.Size(1585, 668);
            this.tab_dek.TabIndex = 6;
            this.tab_dek.Text = "DEK Modülü";
            this.tab_dek.UseVisualStyleBackColor = true;
            // 
            // gMapControl_DEK
            // 
            this.gMapControl_DEK.AllowDrop = true;
            this.gMapControl_DEK.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_DEK.Bearing = 0F;
            this.gMapControl_DEK.CanDragMap = true;
            this.gMapControl_DEK.Cursor = System.Windows.Forms.Cursors.Arrow;
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
            this.gMapControl_DEK.Size = new System.Drawing.Size(1337, 3069);
            this.gMapControl_DEK.TabIndex = 38;
            this.gMapControl_DEK.Zoom = 0D;
            this.gMapControl_DEK.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_DEK_OnMapClick);
            this.gMapControl_DEK.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_Dek_OnMarkerClick);
            // 
            // panel_DEK
            // 
            this.panel_DEK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_DEK.Controls.Add(this.dekSimMaxBtn);
            this.panel_DEK.Controls.Add(this.dekSimDefBtn);
            this.panel_DEK.Controls.Add(this.dekSimMinBtn);
            this.panel_DEK.Controls.Add(this.DEKCenterAddButton);
            this.panel_DEK.Controls.Add(this.DEKSimButton);
            this.panel_DEK.Controls.Add(this.label_DEK_Gelecek);
            this.panel_DEK.Controls.Add(this.comboBox_DEK_il);
            this.panel_DEK.Controls.Add(this.comboBox_DEK_Yıl);
            this.panel_DEK.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_DEK.Location = new System.Drawing.Point(1337, 0);
            this.panel_DEK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_DEK.Name = "panel_DEK";
            this.panel_DEK.Size = new System.Drawing.Size(248, 694);
            this.panel_DEK.TabIndex = 37;
            // 
            // dekSimMaxBtn
            // 
            this.dekSimMaxBtn.AutoSize = true;
            this.dekSimMaxBtn.Location = new System.Drawing.Point(32, 191);
            this.dekSimMaxBtn.Margin = new System.Windows.Forms.Padding(4);
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
            this.dekSimDefBtn.Location = new System.Drawing.Point(32, 158);
            this.dekSimDefBtn.Margin = new System.Windows.Forms.Padding(4);
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
            this.dekSimMinBtn.Location = new System.Drawing.Point(32, 124);
            this.dekSimMinBtn.Margin = new System.Windows.Forms.Padding(4);
            this.dekSimMinBtn.Name = "dekSimMinBtn";
            this.dekSimMinBtn.Size = new System.Drawing.Size(141, 27);
            this.dekSimMinBtn.TabIndex = 57;
            this.dekSimMinBtn.TabStop = true;
            this.dekSimMinBtn.Text = "Yavaş Senaryo";
            this.dekSimMinBtn.UseVisualStyleBackColor = true;
            this.dekSimMinBtn.CheckedChanged += new System.EventHandler(this.dekSimMinBtn_CheckedChanged);
            // 
            // label_DEK_Gelecek
            // 
            this.label_DEK_Gelecek.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_DEK_Gelecek.AutoSize = true;
            this.label_DEK_Gelecek.Location = new System.Drawing.Point(9, 11);
            this.label_DEK_Gelecek.Name = "label_DEK_Gelecek";
            this.label_DEK_Gelecek.Size = new System.Drawing.Size(208, 23);
            this.label_DEK_Gelecek.TabIndex = 3;
            this.label_DEK_Gelecek.Text = "DEK Gelecek Simülasyonu";
            // 
            // comboBox_DEK_il
            // 
            this.comboBox_DEK_il.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.comboBox_DEK_il.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.comboBox_DEK_il.FormattingEnabled = true;
            this.comboBox_DEK_il.Location = new System.Drawing.Point(27, 66);
            this.comboBox_DEK_il.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_DEK_il.Name = "comboBox_DEK_il";
            this.comboBox_DEK_il.Size = new System.Drawing.Size(89, 29);
            this.comboBox_DEK_il.TabIndex = 1;
            this.comboBox_DEK_il.Text = "İL";
            this.comboBox_DEK_il.SelectedIndexChanged += new System.EventHandler(this.dek_city_SelectedIndexChanged);
            // 
            // comboBox_DEK_Yıl
            // 
            this.comboBox_DEK_Yıl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.comboBox_DEK_Yıl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.comboBox_DEK_Yıl.FormattingEnabled = true;
            this.comboBox_DEK_Yıl.Location = new System.Drawing.Point(139, 66);
            this.comboBox_DEK_Yıl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_DEK_Yıl.Name = "comboBox_DEK_Yıl";
            this.comboBox_DEK_Yıl.Size = new System.Drawing.Size(91, 29);
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
            this.tab_ea.Location = new System.Drawing.Point(4, 56);
            this.tab_ea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ea.Name = "tab_ea";
            this.tab_ea.Size = new System.Drawing.Size(1585, 668);
            this.tab_ea.TabIndex = 5;
            this.tab_ea.Text = "EA Şarj Modülü";
            this.tab_ea.UseVisualStyleBackColor = true;
            // 
            // EAStationsLegendPanel
            // 
            this.EAStationsLegendPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EAStationsLegendPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EAStationsLegendPanel.Controls.Add(this.DCFastLegendValueLabel);
            this.EAStationsLegendPanel.Controls.Add(this.DCFastLegendLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACPublicLegendValueLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACPublicLegendLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACWorkLegendValueLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACWorkLegendLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACHomeLegendValueLabel);
            this.EAStationsLegendPanel.Controls.Add(this.ACHomeLegendLabel);
            this.EAStationsLegendPanel.Controls.Add(this.AddStationLabel);
            this.EAStationsLegendPanel.Controls.Add(this.EAStationAddButton);
            this.EAStationsLegendPanel.Location = new System.Drawing.Point(1313, 5567);
            this.EAStationsLegendPanel.Margin = new System.Windows.Forms.Padding(4);
            this.EAStationsLegendPanel.Name = "EAStationsLegendPanel";
            this.EAStationsLegendPanel.Size = new System.Drawing.Size(268, 326);
            this.EAStationsLegendPanel.TabIndex = 51;
            // 
            // DCFastLegendValueLabel
            // 
            this.DCFastLegendValueLabel.AutoSize = true;
            this.DCFastLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DCFastLegendValueLabel.Location = new System.Drawing.Point(132, 166);
            this.DCFastLegendValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DCFastLegendValueLabel.Name = "DCFastLegendValueLabel";
            this.DCFastLegendValueLabel.Size = new System.Drawing.Size(57, 19);
            this.DCFastLegendValueLabel.TabIndex = 57;
            this.DCFastLegendValueLabel.Text = "150 kW";
            // 
            // DCFastLegendLabel
            // 
            this.DCFastLegendLabel.AutoSize = true;
            this.DCFastLegendLabel.Location = new System.Drawing.Point(7, 166);
            this.DCFastLegendLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DCFastLegendLabel.Name = "DCFastLegendLabel";
            this.DCFastLegendLabel.Size = new System.Drawing.Size(70, 23);
            this.DCFastLegendLabel.TabIndex = 56;
            this.DCFastLegendLabel.Text = "DC-Fast";
            // 
            // ACPublicLegendValueLabel
            // 
            this.ACPublicLegendValueLabel.AutoSize = true;
            this.ACPublicLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACPublicLegendValueLabel.Location = new System.Drawing.Point(132, 128);
            this.ACPublicLegendValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACPublicLegendValueLabel.Name = "ACPublicLegendValueLabel";
            this.ACPublicLegendValueLabel.Size = new System.Drawing.Size(53, 19);
            this.ACPublicLegendValueLabel.TabIndex = 55;
            this.ACPublicLegendValueLabel.Text = " 22 kW";
            // 
            // ACPublicLegendLabel
            // 
            this.ACPublicLegendLabel.AutoSize = true;
            this.ACPublicLegendLabel.Location = new System.Drawing.Point(7, 124);
            this.ACPublicLegendLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACPublicLegendLabel.Name = "ACPublicLegendLabel";
            this.ACPublicLegendLabel.Size = new System.Drawing.Size(85, 23);
            this.ACPublicLegendLabel.TabIndex = 54;
            this.ACPublicLegendLabel.Text = "AC-Public";
            // 
            // ACWorkLegendValueLabel
            // 
            this.ACWorkLegendValueLabel.AutoSize = true;
            this.ACWorkLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACWorkLegendValueLabel.Location = new System.Drawing.Point(132, 89);
            this.ACWorkLegendValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACWorkLegendValueLabel.Name = "ACWorkLegendValueLabel";
            this.ACWorkLegendValueLabel.Size = new System.Drawing.Size(53, 19);
            this.ACWorkLegendValueLabel.TabIndex = 53;
            this.ACWorkLegendValueLabel.Text = " 11 kW";
            // 
            // ACWorkLegendLabel
            // 
            this.ACWorkLegendLabel.AutoSize = true;
            this.ACWorkLegendLabel.Location = new System.Drawing.Point(7, 85);
            this.ACWorkLegendLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACWorkLegendLabel.Name = "ACWorkLegendLabel";
            this.ACWorkLegendLabel.Size = new System.Drawing.Size(80, 23);
            this.ACWorkLegendLabel.TabIndex = 52;
            this.ACWorkLegendLabel.Text = "AC-Work";
            // 
            // ACHomeLegendValueLabel
            // 
            this.ACHomeLegendValueLabel.AutoSize = true;
            this.ACHomeLegendValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.ACHomeLegendValueLabel.Location = new System.Drawing.Point(136, 46);
            this.ACHomeLegendValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACHomeLegendValueLabel.Name = "ACHomeLegendValueLabel";
            this.ACHomeLegendValueLabel.Size = new System.Drawing.Size(49, 19);
            this.ACHomeLegendValueLabel.TabIndex = 51;
            this.ACHomeLegendValueLabel.Text = "11 kW";
            // 
            // ACHomeLegendLabel
            // 
            this.ACHomeLegendLabel.AutoSize = true;
            this.ACHomeLegendLabel.Location = new System.Drawing.Point(4, 41);
            this.ACHomeLegendLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACHomeLegendLabel.Name = "ACHomeLegendLabel";
            this.ACHomeLegendLabel.Size = new System.Drawing.Size(86, 23);
            this.ACHomeLegendLabel.TabIndex = 50;
            this.ACHomeLegendLabel.Text = "AC-Home";
            // 
            // AddStationLabel
            // 
            this.AddStationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddStationLabel.AutoSize = true;
            this.AddStationLabel.Location = new System.Drawing.Point(3, 0);
            this.AddStationLabel.Name = "AddStationLabel";
            this.AddStationLabel.Size = new System.Drawing.Size(128, 23);
            this.AddStationLabel.TabIndex = 45;
            this.AddStationLabel.Text = "İstasyon Tipleri:";
            // 
            // panel_ea
            // 
            this.panel_ea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.panel_ea.Controls.Add(this.checkBox_DC_Fast);
            this.panel_ea.Controls.Add(this.checkBox_AC_Public);
            this.panel_ea.Controls.Add(this.checkBox_AC_Home);
            this.panel_ea.Controls.Add(this.checkBox_AC_Work);
            this.panel_ea.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_ea.Location = new System.Drawing.Point(0, 0);
            this.panel_ea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_ea.Name = "panel_ea";
            this.panel_ea.Size = new System.Drawing.Size(1316, 32);
            this.panel_ea.TabIndex = 44;
            // 
            // checkBox_DC_Fast
            // 
            this.checkBox_DC_Fast.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBox_DC_Fast.AutoSize = true;
            this.checkBox_DC_Fast.Checked = true;
            this.checkBox_DC_Fast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_DC_Fast.Location = new System.Drawing.Point(856, 6);
            this.checkBox_DC_Fast.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_DC_Fast.Name = "checkBox_DC_Fast";
            this.checkBox_DC_Fast.Size = new System.Drawing.Size(99, 27);
            this.checkBox_DC_Fast.TabIndex = 42;
            this.checkBox_DC_Fast.Text = "DC-FAST";
            this.checkBox_DC_Fast.UseVisualStyleBackColor = true;
            this.checkBox_DC_Fast.CheckedChanged += new System.EventHandler(this.checkBox_Dc_Fast);
            // 
            // checkBox_AC_Public
            // 
            this.checkBox_AC_Public.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBox_AC_Public.AutoSize = true;
            this.checkBox_AC_Public.Checked = true;
            this.checkBox_AC_Public.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Public.Location = new System.Drawing.Point(712, 6);
            this.checkBox_AC_Public.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Public.Name = "checkBox_AC_Public";
            this.checkBox_AC_Public.Size = new System.Drawing.Size(117, 27);
            this.checkBox_AC_Public.TabIndex = 41;
            this.checkBox_AC_Public.Text = "AC-PUBLIC";
            this.checkBox_AC_Public.UseVisualStyleBackColor = true;
            this.checkBox_AC_Public.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Public);
            // 
            // checkBox_AC_Home
            // 
            this.checkBox_AC_Home.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBox_AC_Home.AutoSize = true;
            this.checkBox_AC_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_AC_Home.Checked = true;
            this.checkBox_AC_Home.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Home.Location = new System.Drawing.Point(440, 6);
            this.checkBox_AC_Home.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Home.Name = "checkBox_AC_Home";
            this.checkBox_AC_Home.Size = new System.Drawing.Size(112, 27);
            this.checkBox_AC_Home.TabIndex = 39;
            this.checkBox_AC_Home.Text = "AC-HOME";
            this.checkBox_AC_Home.UseVisualStyleBackColor = true;
            this.checkBox_AC_Home.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Home);
            // 
            // checkBox_AC_Work
            // 
            this.checkBox_AC_Work.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBox_AC_Work.AutoSize = true;
            this.checkBox_AC_Work.Checked = true;
            this.checkBox_AC_Work.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AC_Work.Location = new System.Drawing.Point(575, 6);
            this.checkBox_AC_Work.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_AC_Work.Name = "checkBox_AC_Work";
            this.checkBox_AC_Work.Size = new System.Drawing.Size(111, 27);
            this.checkBox_AC_Work.TabIndex = 40;
            this.checkBox_AC_Work.Text = "AC-WORK";
            this.checkBox_AC_Work.UseVisualStyleBackColor = true;
            this.checkBox_AC_Work.CheckedChanged += new System.EventHandler(this.checkBox_Ac_Work);
            // 
            // GelecekSimPanel
            // 
            this.GelecekSimPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.GelecekSimPanel.Controls.Add(this.EaSimMaxBtn);
            this.GelecekSimPanel.Controls.Add(this.EaSimDefBtn);
            this.GelecekSimPanel.Controls.Add(this.EaSimMinBtn);
            this.GelecekSimPanel.Controls.Add(this.EASimButton);
            this.GelecekSimPanel.Controls.Add(this.FutureSimLabel);
            this.GelecekSimPanel.Controls.Add(this.comboBox_ea_il_secimi);
            this.GelecekSimPanel.Controls.Add(this.comboBox_ea_yıl_secimi);
            this.GelecekSimPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.GelecekSimPanel.Location = new System.Drawing.Point(1316, 0);
            this.GelecekSimPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.GelecekSimPanel.Name = "GelecekSimPanel";
            this.GelecekSimPanel.Size = new System.Drawing.Size(269, 694);
            this.GelecekSimPanel.TabIndex = 43;
            // 
            // EaSimMaxBtn
            // 
            this.EaSimMaxBtn.AutoSize = true;
            this.EaSimMaxBtn.Location = new System.Drawing.Point(43, 188);
            this.EaSimMaxBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EaSimMaxBtn.Name = "EaSimMaxBtn";
            this.EaSimMaxBtn.Size = new System.Drawing.Size(132, 27);
            this.EaSimMaxBtn.TabIndex = 53;
            this.EaSimMaxBtn.TabStop = true;
            this.EaSimMaxBtn.Text = "Hızlı Senaryo";
            this.EaSimMaxBtn.UseVisualStyleBackColor = true;
            this.EaSimMaxBtn.CheckedChanged += new System.EventHandler(this.EaSimMaxBtn_CheckedChanged);
            // 
            // EaSimDefBtn
            // 
            this.EaSimDefBtn.AutoSize = true;
            this.EaSimDefBtn.Location = new System.Drawing.Point(41, 155);
            this.EaSimDefBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EaSimDefBtn.Name = "EaSimDefBtn";
            this.EaSimDefBtn.Size = new System.Drawing.Size(176, 27);
            this.EaSimDefBtn.TabIndex = 52;
            this.EaSimDefBtn.TabStop = true;
            this.EaSimDefBtn.Text = "Varsayılan Senaryo";
            this.EaSimDefBtn.UseVisualStyleBackColor = true;
            this.EaSimDefBtn.CheckedChanged += new System.EventHandler(this.EaSimDefBtn_CheckedChanged);
            // 
            // EaSimMinBtn
            // 
            this.EaSimMinBtn.AutoSize = true;
            this.EaSimMinBtn.Location = new System.Drawing.Point(41, 122);
            this.EaSimMinBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EaSimMinBtn.Name = "EaSimMinBtn";
            this.EaSimMinBtn.Size = new System.Drawing.Size(141, 27);
            this.EaSimMinBtn.TabIndex = 51;
            this.EaSimMinBtn.TabStop = true;
            this.EaSimMinBtn.Text = "Yavaş Senaryo";
            this.EaSimMinBtn.UseVisualStyleBackColor = true;
            this.EaSimMinBtn.CheckedChanged += new System.EventHandler(this.EaSimMinBtn_CheckedChanged);
            // 
            // FutureSimLabel
            // 
            this.FutureSimLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FutureSimLabel.AutoSize = true;
            this.FutureSimLabel.Location = new System.Drawing.Point(35, 32);
            this.FutureSimLabel.Name = "FutureSimLabel";
            this.FutureSimLabel.Size = new System.Drawing.Size(176, 23);
            this.FutureSimLabel.TabIndex = 2;
            this.FutureSimLabel.Text = "Gelecek Simülasyonu:";
            // 
            // comboBox_ea_il_secimi
            // 
            this.comboBox_ea_il_secimi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.comboBox_ea_il_secimi.FormattingEnabled = true;
            this.comboBox_ea_il_secimi.Items.AddRange(new object[] {
            "İzmir",
            "Eskişehir"});
            this.comboBox_ea_il_secimi.Location = new System.Drawing.Point(16, 78);
            this.comboBox_ea_il_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_ea_il_secimi.Name = "comboBox_ea_il_secimi";
            this.comboBox_ea_il_secimi.Size = new System.Drawing.Size(92, 29);
            this.comboBox_ea_il_secimi.TabIndex = 1;
            this.comboBox_ea_il_secimi.Text = "İL";
            this.comboBox_ea_il_secimi.SelectedIndexChanged += new System.EventHandler(this.ilSecimiMonteCarlo);
            // 
            // comboBox_ea_yıl_secimi
            // 
            this.comboBox_ea_yıl_secimi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.comboBox_ea_yıl_secimi.FormattingEnabled = true;
            this.comboBox_ea_yıl_secimi.Items.AddRange(new object[] {
            "2015",
            "2016"});
            this.comboBox_ea_yıl_secimi.Location = new System.Drawing.Point(152, 78);
            this.comboBox_ea_yıl_secimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_ea_yıl_secimi.Name = "comboBox_ea_yıl_secimi";
            this.comboBox_ea_yıl_secimi.Size = new System.Drawing.Size(92, 29);
            this.comboBox_ea_yıl_secimi.TabIndex = 0;
            this.comboBox_ea_yıl_secimi.Text = "YIL";
            this.comboBox_ea_yıl_secimi.SelectedIndexChanged += new System.EventHandler(this.yilSecimiMonteCarlo);
            // 
            // gMapControl_EA
            // 
            this.gMapControl_EA.Bearing = 0F;
            this.gMapControl_EA.CanDragMap = true;
            this.gMapControl_EA.Cursor = System.Windows.Forms.Cursors.Default;
            this.gMapControl_EA.Dock = System.Windows.Forms.DockStyle.Fill;
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
            this.gMapControl_EA.Size = new System.Drawing.Size(1585, 694);
            this.gMapControl_EA.TabIndex = 18;
            this.gMapControl_EA.Zoom = 0D;
            this.gMapControl_EA.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_Ea_OnMapClick);
            this.gMapControl_EA.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_EA_OnMarkerClick);
            // 
            // tab_ekonometrik
            // 
            this.tab_ekonometrik.AutoScroll = true;
            this.tab_ekonometrik.Controls.Add(this.ELFResultsTabControls);
            this.tab_ekonometrik.Controls.Add(this.ELFTablePanel);
            this.tab_ekonometrik.Controls.Add(this.ELFGraphicsPanel);
            this.tab_ekonometrik.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tab_ekonometrik.ImageIndex = 4;
            this.tab_ekonometrik.Location = new System.Drawing.Point(4, 56);
            this.tab_ekonometrik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ekonometrik.Name = "tab_ekonometrik";
            this.tab_ekonometrik.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ekonometrik.Size = new System.Drawing.Size(1585, 668);
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
            this.ELFResultsTabControls.Location = new System.Drawing.Point(252, 2);
            this.ELFResultsTabControls.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFResultsTabControls.Name = "ELFResultsTabControls";
            this.ELFResultsTabControls.SelectedIndex = 0;
            this.ELFResultsTabControls.Size = new System.Drawing.Size(1330, 690);
            this.ELFResultsTabControls.TabIndex = 3;
            // 
            // ELFMinResultsTabPage
            // 
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinResultsTable);
            this.ELFMinResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFMinResultsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMinResultsTabPage.Name = "ELFMinResultsTabPage";
            this.ELFMinResultsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMinResultsTabPage.Size = new System.Drawing.Size(1322, 656);
            this.ELFMinResultsTabPage.TabIndex = 0;
            this.ELFMinResultsTabPage.Text = "Minimum Sonuçlar";
            this.ELFMinResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFMinResultsTable
            // 
            this.ELFMinResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFMinResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFMinResultsTable.DefaultCellStyle = dataGridViewCellStyle3;
            this.ELFMinResultsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFMinResultsTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMinResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMinResultsTable.Name = "ELFMinResultsTable";
            this.ELFMinResultsTable.RowHeadersWidth = 51;
            this.ELFMinResultsTable.Size = new System.Drawing.Size(1316, 652);
            this.ELFMinResultsTable.TabIndex = 0;
            // 
            // ELFLowResultsTabPage
            // 
            this.ELFLowResultsTabPage.Controls.Add(this.ELFLowResultsTable);
            this.ELFLowResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFLowResultsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFLowResultsTabPage.Name = "ELFLowResultsTabPage";
            this.ELFLowResultsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFLowResultsTabPage.Size = new System.Drawing.Size(1322, 656);
            this.ELFLowResultsTabPage.TabIndex = 1;
            this.ELFLowResultsTabPage.Text = "Düşük Sonuçlar";
            this.ELFLowResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFLowResultsTable
            // 
            this.ELFLowResultsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFLowResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFLowResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFLowResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFLowResultsTable.Location = new System.Drawing.Point(3, 0);
            this.ELFLowResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFLowResultsTable.Name = "ELFLowResultsTable";
            this.ELFLowResultsTable.RowHeadersWidth = 51;
            this.ELFLowResultsTable.Size = new System.Drawing.Size(1441, 252);
            this.ELFLowResultsTable.TabIndex = 1;
            // 
            // ELFBaseResultsTabPage
            // 
            this.ELFBaseResultsTabPage.Controls.Add(this.ELFBaseResultsTable);
            this.ELFBaseResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFBaseResultsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBaseResultsTabPage.Name = "ELFBaseResultsTabPage";
            this.ELFBaseResultsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBaseResultsTabPage.Size = new System.Drawing.Size(1322, 656);
            this.ELFBaseResultsTabPage.TabIndex = 2;
            this.ELFBaseResultsTabPage.Text = "Baz Sonuçlar";
            this.ELFBaseResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFBaseResultsTable
            // 
            this.ELFBaseResultsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFBaseResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFBaseResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBaseResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBaseResultsTable.Location = new System.Drawing.Point(5, 2);
            this.ELFBaseResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBaseResultsTable.Name = "ELFBaseResultsTable";
            this.ELFBaseResultsTable.RowHeadersWidth = 51;
            this.ELFBaseResultsTable.Size = new System.Drawing.Size(1440, 252);
            this.ELFBaseResultsTable.TabIndex = 1;
            // 
            // ELFHighResultsTabPage
            // 
            this.ELFHighResultsTabPage.Controls.Add(this.ELFHighResultsTable);
            this.ELFHighResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFHighResultsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFHighResultsTabPage.Name = "ELFHighResultsTabPage";
            this.ELFHighResultsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFHighResultsTabPage.Size = new System.Drawing.Size(1322, 656);
            this.ELFHighResultsTabPage.TabIndex = 3;
            this.ELFHighResultsTabPage.Text = "Yüksek Sonuçlar";
            this.ELFHighResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFHighResultsTable
            // 
            this.ELFHighResultsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFHighResultsTable.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ELFHighResultsTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFHighResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighResultsTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFHighResultsTable.Location = new System.Drawing.Point(5, 0);
            this.ELFHighResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFHighResultsTable.Name = "ELFHighResultsTable";
            this.ELFHighResultsTable.RowHeadersWidth = 51;
            this.ELFHighResultsTable.Size = new System.Drawing.Size(1435, 252);
            this.ELFHighResultsTable.TabIndex = 1;
            // 
            // ELFMaxResultsTabPage
            // 
            this.ELFMaxResultsTabPage.Controls.Add(this.ELFMaxResultsTable);
            this.ELFMaxResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFMaxResultsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaxResultsTabPage.Name = "ELFMaxResultsTabPage";
            this.ELFMaxResultsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaxResultsTabPage.Size = new System.Drawing.Size(1322, 656);
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
            this.ELFMaxResultsTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaxResultsTable.Name = "ELFMaxResultsTable";
            this.ELFMaxResultsTable.RowHeadersWidth = 51;
            this.ELFMaxResultsTable.Size = new System.Drawing.Size(1445, 256);
            this.ELFMaxResultsTable.TabIndex = 1;
            // 
            // ELFGraphicOutputsTabPage
            // 
            this.ELFGraphicOutputsTabPage.Controls.Add(this.panel_ELF_Grafikler);
            this.ELFGraphicOutputsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFGraphicOutputsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFGraphicOutputsTabPage.Name = "ELFGraphicOutputsTabPage";
            this.ELFGraphicOutputsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFGraphicOutputsTabPage.Size = new System.Drawing.Size(1322, 656);
            this.ELFGraphicOutputsTabPage.TabIndex = 5;
            this.ELFGraphicOutputsTabPage.Text = "Projeksiyon Grafik Sonuçları";
            this.ELFGraphicOutputsTabPage.UseVisualStyleBackColor = true;
            // 
            // panel_ELF_Grafikler
            // 
            this.panel_ELF_Grafikler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_ELF_Grafikler.BackColor = System.Drawing.Color.NavajoWhite;
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_5);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_4);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_3);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_2);
            this.panel_ELF_Grafikler.Controls.Add(this.pictureBox_ELF_1);
            this.panel_ELF_Grafikler.Location = new System.Drawing.Point(3, 2);
            this.panel_ELF_Grafikler.Margin = new System.Windows.Forms.Padding(4);
            this.panel_ELF_Grafikler.Name = "panel_ELF_Grafikler";
            this.panel_ELF_Grafikler.Size = new System.Drawing.Size(1440, 176);
            this.panel_ELF_Grafikler.TabIndex = 6;
            // 
            // ELFTablePanel
            // 
            this.ELFTablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFTablePanel.Location = new System.Drawing.Point(307, 2);
            this.ELFTablePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFTablePanel.Name = "ELFTablePanel";
            this.ELFTablePanel.Size = new System.Drawing.Size(965, 7477);
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
            this.ELFGraphicsPanel.Location = new System.Drawing.Point(3, 2);
            this.ELFGraphicsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFGraphicsPanel.Name = "ELFGraphicsPanel";
            this.ELFGraphicsPanel.Size = new System.Drawing.Size(249, 690);
            this.ELFGraphicsPanel.TabIndex = 22;
            // 
            // SenaryoResultsLabel
            // 
            this.SenaryoResultsLabel.AutoSize = true;
            this.SenaryoResultsLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.SenaryoResultsLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.SenaryoResultsLabel.Location = new System.Drawing.Point(5, 30);
            this.SenaryoResultsLabel.Name = "SenaryoResultsLabel";
            this.SenaryoResultsLabel.Size = new System.Drawing.Size(225, 28);
            this.SenaryoResultsLabel.TabIndex = 7;
            this.SenaryoResultsLabel.Text = "ELF Senaryo Sonuçları:";
            // 
            // tab_imar
            // 
            this.tab_imar.AutoScroll = true;
            this.tab_imar.Controls.Add(this.panel_imar);
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
            this.tab_imar.Location = new System.Drawing.Point(4, 56);
            this.tab_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_imar.Name = "tab_imar";
            this.tab_imar.Size = new System.Drawing.Size(1585, 668);
            this.tab_imar.TabIndex = 4;
            this.tab_imar.Text = "İmar Analizleri";
            this.tab_imar.UseVisualStyleBackColor = true;
            // 
            // panel_imar
            // 
            this.panel_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_imar.Controls.Add(this.Mesafe_imar);
            this.panel_imar.Controls.Add(this.buton_imar_katmanlar);
            this.panel_imar.Controls.Add(this.mesafe_metre_imar);
            this.panel_imar.Controls.Add(this.gMapControl_imar);
            this.panel_imar.Controls.Add(this.webView_imar);
            this.panel_imar.Location = new System.Drawing.Point(302, 43);
            this.panel_imar.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
            this.panel_imar.Name = "panel_imar";
            this.panel_imar.Size = new System.Drawing.Size(1236, 949);
            this.panel_imar.TabIndex = 63;
            // 
            // Mesafe_imar
            // 
            this.Mesafe_imar.AutoSize = true;
            this.Mesafe_imar.Location = new System.Drawing.Point(18, 17);
            this.Mesafe_imar.Name = "Mesafe_imar";
            this.Mesafe_imar.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_imar.TabIndex = 57;
            this.Mesafe_imar.Text = "Mesafe:";
            this.Mesafe_imar.Visible = false;
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
            this.harita_katmanları_right_click.Size = new System.Drawing.Size(196, 186);
            // 
            // mesafe_metre_imar
            // 
            this.mesafe_metre_imar.AutoSize = true;
            this.mesafe_metre_imar.Location = new System.Drawing.Point(139, 27);
            this.mesafe_metre_imar.Name = "mesafe_metre_imar";
            this.mesafe_metre_imar.Size = new System.Drawing.Size(0, 23);
            this.mesafe_metre_imar.TabIndex = 56;
            this.mesafe_metre_imar.Visible = false;
            // 
            // gMapControl_imar
            // 
            this.gMapControl_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_imar.Bearing = 0F;
            this.gMapControl_imar.CanDragMap = true;
            this.gMapControl_imar.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_imar.GrayScaleMode = false;
            this.gMapControl_imar.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_imar.LevelsKeepInMemory = 5;
            this.gMapControl_imar.Location = new System.Drawing.Point(0, 0);
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
            this.gMapControl_imar.Size = new System.Drawing.Size(1236, 946);
            this.gMapControl_imar.TabIndex = 60;
            this.gMapControl_imar.Zoom = 0D;
            this.gMapControl_imar.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_imar_OnMapClick);
            this.gMapControl_imar.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_imar_OnMapDoubleClick);
            this.gMapControl_imar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseDown);
            this.gMapControl_imar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseMove);
            this.gMapControl_imar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseUp);
            // 
            // webView_imar
            // 
            this.webView_imar.AllowExternalDrop = true;
            this.webView_imar.CreationProperties = null;
            this.webView_imar.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_imar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView_imar.Location = new System.Drawing.Point(0, 0);
            this.webView_imar.Name = "webView_imar";
            this.webView_imar.Size = new System.Drawing.Size(1236, 949);
            this.webView_imar.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_imar.TabIndex = 61;
            this.webView_imar.Visible = false;
            this.webView_imar.ZoomFactor = 1D;
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
            this.katmanlar_right_click.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.katmanlar_right_click.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabloyuGörToolStripMenuItem,
            this.rengiDeğiştirToolStripMenuItem,
            this.temizleToolStripMenuItem,
            this.yenidenAdlandırToolStripMenuItem,
            this.kaydetToolStripMenuItem});
            this.katmanlar_right_click.Name = "katmanlar_right_click";
            this.katmanlar_right_click.Size = new System.Drawing.Size(196, 134);
            this.katmanlar_right_click.Opening += new System.ComponentModel.CancelEventHandler(this.katmanlar_right_click_Opening);
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
            this.checkBox_imar_4.Location = new System.Drawing.Point(8, 253);
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
            this.checkBox_imar_6.Location = new System.Drawing.Point(8, 323);
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
            this.checkBox_imar_13.Location = new System.Drawing.Point(8, 568);
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
            this.label_imar_katmanlar.Location = new System.Drawing.Point(22, 109);
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
            this.İmar_Nokta,
            this.toolStripSeparator13,
            this.İmar_Grid_Oluştur,
            this.toolStripSeparator14,
            this.İmar_Fonksiyonlar});
            this.toolStrip_imar.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_imar.Name = "toolStrip_imar";
            this.toolStrip_imar.Size = new System.Drawing.Size(1585, 32);
            this.toolStrip_imar.TabIndex = 40;
            this.toolStrip_imar.Text = "toolStrip1";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 32);
            // 
            // tab_optDTR
            // 
            this.tab_optDTR.Controls.Add(this.gMapControl_optimalDTR);
            this.tab_optDTR.Controls.Add(this.webView_optimalDTR);
            this.tab_optDTR.Controls.Add(this.checkBox_optDTR_Eskişehir);
            this.tab_optDTR.Controls.Add(this.checkBox_optDTR_İzmir);
            this.tab_optDTR.Location = new System.Drawing.Point(4, 56);
            this.tab_optDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_optDTR.Name = "tab_optDTR";
            this.tab_optDTR.Size = new System.Drawing.Size(1585, 668);
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
            this.gMapControl_optimalDTR.Location = new System.Drawing.Point(93, 186);
            this.gMapControl_optimalDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
            this.gMapControl_optimalDTR.Size = new System.Drawing.Size(1033, 3533);
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
            this.webView_optimalDTR.Location = new System.Drawing.Point(8, 2);
            this.webView_optimalDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_optimalDTR.Name = "webView_optimalDTR";
            this.webView_optimalDTR.Size = new System.Drawing.Size(1033, 3186);
            this.webView_optimalDTR.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_optimalDTR.TabIndex = 40;
            this.webView_optimalDTR.Visible = false;
            this.webView_optimalDTR.ZoomFactor = 1D;
            // 
            // checkBox_optDTR_Eskişehir
            // 
            this.checkBox_optDTR_Eskişehir.AutoSize = true;
            this.checkBox_optDTR_Eskişehir.Location = new System.Drawing.Point(1181, 110);
            this.checkBox_optDTR_Eskişehir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_optDTR_Eskişehir.Name = "checkBox_optDTR_Eskişehir";
            this.checkBox_optDTR_Eskişehir.Size = new System.Drawing.Size(97, 27);
            this.checkBox_optDTR_Eskişehir.TabIndex = 22;
            this.checkBox_optDTR_Eskişehir.Text = "Eskişehir";
            this.checkBox_optDTR_Eskişehir.UseVisualStyleBackColor = true;
            // 
            // checkBox_optDTR_İzmir
            // 
            this.checkBox_optDTR_İzmir.AutoSize = true;
            this.checkBox_optDTR_İzmir.Location = new System.Drawing.Point(1181, 82);
            this.checkBox_optDTR_İzmir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_optDTR_İzmir.Name = "checkBox_optDTR_İzmir";
            this.checkBox_optDTR_İzmir.Size = new System.Drawing.Size(70, 27);
            this.checkBox_optDTR_İzmir.TabIndex = 21;
            this.checkBox_optDTR_İzmir.Text = "İzmir";
            this.checkBox_optDTR_İzmir.UseVisualStyleBackColor = true;
            // 
            // tab_senaryo
            // 
            this.tab_senaryo.Controls.Add(this.SenaryoModulePanel);
            this.tab_senaryo.Location = new System.Drawing.Point(4, 56);
            this.tab_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_senaryo.Name = "tab_senaryo";
            this.tab_senaryo.Size = new System.Drawing.Size(1585, 668);
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
            this.SenaryoModulePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SenaryoModulePanel.Name = "SenaryoModulePanel";
            this.SenaryoModulePanel.Size = new System.Drawing.Size(1585, 694);
            this.SenaryoModulePanel.TabIndex = 0;
            // 
            // EkonometrikSenaryoElementsPanel
            // 
            this.EkonometrikSenaryoElementsPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ShowResultsButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFPredictionShowResultsButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFScenerioSaveButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.richTextBox_senaryolar_ELF);
            this.EkonometrikSenaryoElementsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EkonometrikSenaryoElementsPanel.Location = new System.Drawing.Point(1329, 0);
            this.EkonometrikSenaryoElementsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoElementsPanel.Name = "EkonometrikSenaryoElementsPanel";
            this.EkonometrikSenaryoElementsPanel.Size = new System.Drawing.Size(256, 694);
            this.EkonometrikSenaryoElementsPanel.TabIndex = 1;
            // 
            // richTextBox_senaryolar_ELF
            // 
            this.richTextBox_senaryolar_ELF.BackColor = System.Drawing.Color.NavajoWhite;
            this.richTextBox_senaryolar_ELF.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_senaryolar_ELF.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox_senaryolar_ELF.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.richTextBox_senaryolar_ELF.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_senaryolar_ELF.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox_senaryolar_ELF.Name = "richTextBox_senaryolar_ELF";
            this.richTextBox_senaryolar_ELF.Size = new System.Drawing.Size(256, 107);
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
            this.SenaryoModuleTabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SenaryoModuleTabControl.Name = "SenaryoModuleTabControl";
            this.SenaryoModuleTabControl.SelectedIndex = 0;
            this.SenaryoModuleTabControl.Size = new System.Drawing.Size(1585, 694);
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
            this.EkonometrikSenaryoTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoTabPage.Name = "EkonometrikSenaryoTabPage";
            this.EkonometrikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoTabPage.Size = new System.Drawing.Size(1357, 686);
            this.EkonometrikSenaryoTabPage.TabIndex = 0;
            this.EkonometrikSenaryoTabPage.Text = "Ekonometrik Senaryolar";
            this.EkonometrikSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // EkonometrikSenaryoOutputsPanel
            // 
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.ELFSenaryoTabControls);
            this.EkonometrikSenaryoOutputsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EkonometrikSenaryoOutputsPanel.Location = new System.Drawing.Point(3, 2);
            this.EkonometrikSenaryoOutputsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EkonometrikSenaryoOutputsPanel.Name = "EkonometrikSenaryoOutputsPanel";
            this.EkonometrikSenaryoOutputsPanel.Size = new System.Drawing.Size(1351, 682);
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
            this.ELFSenaryoTabControls.Location = new System.Drawing.Point(8, 2);
            this.ELFSenaryoTabControls.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFSenaryoTabControls.Name = "ELFSenaryoTabControls";
            this.ELFSenaryoTabControls.SelectedIndex = 0;
            this.ELFSenaryoTabControls.Size = new System.Drawing.Size(1095, 1235);
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
            this.tabPage_min_senaryo.Size = new System.Drawing.Size(1087, 1201);
            this.tabPage_min_senaryo.TabIndex = 0;
            this.tabPage_min_senaryo.Text = "Minimum Senaryo";
            this.tabPage_min_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFMinSenaryoTable
            // 
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFMinSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.ELFMinSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFMinSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFMinSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
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
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ELFMinSenaryoTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.ELFMinSenaryoTable.RowHeadersWidth = 18;
            this.ELFMinSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMinSenaryoTable.Size = new System.Drawing.Size(1081, 1197);
            this.ELFMinSenaryoTable.TabIndex = 0;
            // 
            // tabPage_dusuk_senaryo
            // 
            this.tabPage_dusuk_senaryo.Controls.Add(this.ELFLowSenaryoTable);
            this.tabPage_dusuk_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_dusuk_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_dusuk_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_senaryo.Name = "tabPage_dusuk_senaryo";
            this.tabPage_dusuk_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_dusuk_senaryo.Size = new System.Drawing.Size(1087, 1201);
            this.tabPage_dusuk_senaryo.TabIndex = 1;
            this.tabPage_dusuk_senaryo.Text = "Düşük Senaryo";
            this.tabPage_dusuk_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFLowSenaryoTable
            // 
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFLowSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.ELFLowSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFLowSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFLowSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFLowSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFLowSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFLowSenaryoTable.Name = "ELFLowSenaryoTable";
            this.ELFLowSenaryoTable.RowHeadersWidth = 18;
            this.ELFLowSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFLowSenaryoTable.Size = new System.Drawing.Size(1081, 1202);
            this.ELFLowSenaryoTable.TabIndex = 1;
            // 
            // tabPage_baz_senaryo
            // 
            this.tabPage_baz_senaryo.Controls.Add(this.ELFBaseSenaryoTable);
            this.tabPage_baz_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_baz_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_baz_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_senaryo.Name = "tabPage_baz_senaryo";
            this.tabPage_baz_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_baz_senaryo.Size = new System.Drawing.Size(1087, 1201);
            this.tabPage_baz_senaryo.TabIndex = 2;
            this.tabPage_baz_senaryo.Text = "Baz Senaryo";
            this.tabPage_baz_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFBaseSenaryoTable
            // 
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFBaseSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.ELFBaseSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFBaseSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFBaseSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFBaseSenaryoTable.DefaultCellStyle = dataGridViewCellStyle9;
            this.ELFBaseSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFBaseSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFBaseSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFBaseSenaryoTable.Name = "ELFBaseSenaryoTable";
            this.ELFBaseSenaryoTable.RowHeadersWidth = 18;
            this.ELFBaseSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFBaseSenaryoTable.Size = new System.Drawing.Size(1081, 1202);
            this.ELFBaseSenaryoTable.TabIndex = 1;
            // 
            // tabPage_yuksek_senaryo
            // 
            this.tabPage_yuksek_senaryo.Controls.Add(this.ELFHighSenaryoTable);
            this.tabPage_yuksek_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_yuksek_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_yuksek_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_senaryo.Name = "tabPage_yuksek_senaryo";
            this.tabPage_yuksek_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_yuksek_senaryo.Size = new System.Drawing.Size(1087, 1201);
            this.tabPage_yuksek_senaryo.TabIndex = 3;
            this.tabPage_yuksek_senaryo.Text = "Yüksek Senaryo";
            this.tabPage_yuksek_senaryo.UseVisualStyleBackColor = true;
            // 
            // ELFHighSenaryoTable
            // 
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Snow;
            this.ELFHighSenaryoTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.ELFHighSenaryoTable.BackgroundColor = System.Drawing.Color.Snow;
            this.ELFHighSenaryoTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ELFHighSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ELFHighSenaryoTable.DefaultCellStyle = dataGridViewCellStyle11;
            this.ELFHighSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighSenaryoTable.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ELFHighSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFHighSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFHighSenaryoTable.Name = "ELFHighSenaryoTable";
            this.ELFHighSenaryoTable.RowHeadersWidth = 51;
            this.ELFHighSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFHighSenaryoTable.Size = new System.Drawing.Size(1081, 1202);
            this.ELFHighSenaryoTable.TabIndex = 1;
            // 
            // tabPage_maks_senaryo
            // 
            this.tabPage_maks_senaryo.Controls.Add(this.ELFMaxSenaryoTable);
            this.tabPage_maks_senaryo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabPage_maks_senaryo.Location = new System.Drawing.Point(4, 30);
            this.tabPage_maks_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_senaryo.Name = "tabPage_maks_senaryo";
            this.tabPage_maks_senaryo.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_maks_senaryo.Size = new System.Drawing.Size(1087, 1201);
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
            this.ELFMaxSenaryoTable.Location = new System.Drawing.Point(3, 2);
            this.ELFMaxSenaryoTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFMaxSenaryoTable.Name = "ELFMaxSenaryoTable";
            this.ELFMaxSenaryoTable.RowHeadersWidth = 18;
            this.ELFMaxSenaryoTable.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ELFMaxSenaryoTable.Size = new System.Drawing.Size(1081, 1202);
            this.ELFMaxSenaryoTable.TabIndex = 1;
            // 
            // YeniGenislemeSenaryoTabPage
            // 
            this.YeniGenislemeSenaryoTabPage.Location = new System.Drawing.Point(224, 4);
            this.YeniGenislemeSenaryoTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.YeniGenislemeSenaryoTabPage.Name = "YeniGenislemeSenaryoTabPage";
            this.YeniGenislemeSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.YeniGenislemeSenaryoTabPage.Size = new System.Drawing.Size(1357, 686);
            this.YeniGenislemeSenaryoTabPage.TabIndex = 4;
            this.YeniGenislemeSenaryoTabPage.Text = "Yeni Genişleme Alanları ";
            this.YeniGenislemeSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // tab_yükHaritası
            // 
            this.tab_yükHaritası.Controls.Add(this.webView_yuk);
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
            this.tab_yükHaritası.Controls.Add(this.Mesafe_yuk);
            this.tab_yükHaritası.Controls.Add(this.mesafe_metre_yuk);
            this.tab_yükHaritası.Controls.Add(this.toolStrip_yuk);
            this.tab_yükHaritası.Controls.Add(this.gMapControl_yuk);
            this.tab_yükHaritası.Controls.Add(this.buton_yuk_haritası_katmanlar);
            this.tab_yükHaritası.Location = new System.Drawing.Point(4, 56);
            this.tab_yükHaritası.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_yükHaritası.Name = "tab_yükHaritası";
            this.tab_yükHaritası.Size = new System.Drawing.Size(1585, 668);
            this.tab_yükHaritası.TabIndex = 9;
            this.tab_yükHaritası.Text = "Yük Haritası Modülü";
            this.tab_yükHaritası.UseVisualStyleBackColor = true;
            // 
            // webView_yuk
            // 
            this.webView_yuk.AllowExternalDrop = true;
            this.webView_yuk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView_yuk.CreationProperties = null;
            this.webView_yuk.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_yuk.Location = new System.Drawing.Point(263, 62);
            this.webView_yuk.Name = "webView_yuk";
            this.webView_yuk.Size = new System.Drawing.Size(1074, 565);
            this.webView_yuk.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_yuk.TabIndex = 71;
            this.webView_yuk.Visible = false;
            this.webView_yuk.ZoomFactor = 1D;
            // 
            // checkBox_yuk_15
            // 
            this.checkBox_yuk_15.AutoSize = true;
            this.checkBox_yuk_15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox_yuk_15.Location = new System.Drawing.Point(8, 600);
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
            this.checkBox_yuk_8.Location = new System.Drawing.Point(8, 355);
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
            this.checkBox_yuk_6.Location = new System.Drawing.Point(8, 285);
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
            this.legendPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.legendPanel.AutoSize = true;
            this.legendPanel.Controls.Add(this.label1);
            this.legendPanel.Location = new System.Drawing.Point(1343, 59);
            this.legendPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(193, 571);
            this.legendPanel.TabIndex = 44;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 350);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "TSfd";
            // 
            // yuk_yıl_deger
            // 
            this.yuk_yıl_deger.AutoSize = true;
            this.yuk_yıl_deger.Location = new System.Drawing.Point(32, 11);
            this.yuk_yıl_deger.Name = "yuk_yıl_deger";
            this.yuk_yıl_deger.Size = new System.Drawing.Size(47, 23);
            this.yuk_yıl_deger.TabIndex = 41;
            this.yuk_yıl_deger.Text = "2024";
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
            this.trackBar_Yıllar.Maximum = 2030;
            this.trackBar_Yıllar.Minimum = 2024;
            this.trackBar_Yıllar.Name = "trackBar_Yıllar";
            this.trackBar_Yıllar.Size = new System.Drawing.Size(217, 56);
            this.trackBar_Yıllar.TabIndex = 39;
            this.trackBar_Yıllar.Value = 2024;
            this.trackBar_Yıllar.ValueChanged += new System.EventHandler(this.trackBar_Yıllar_ValueChanged);
            // 
            // Mesafe_yuk
            // 
            this.Mesafe_yuk.AutoSize = true;
            this.Mesafe_yuk.Location = new System.Drawing.Point(259, 36);
            this.Mesafe_yuk.Name = "Mesafe_yuk";
            this.Mesafe_yuk.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_yuk.TabIndex = 38;
            this.Mesafe_yuk.Text = "Mesafe:";
            this.Mesafe_yuk.Visible = false;
            // 
            // mesafe_metre_yuk
            // 
            this.mesafe_metre_yuk.AutoSize = true;
            this.mesafe_metre_yuk.Location = new System.Drawing.Point(357, 34);
            this.mesafe_metre_yuk.Name = "mesafe_metre_yuk";
            this.mesafe_metre_yuk.Size = new System.Drawing.Size(0, 23);
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
            this.toolStrip_yuk.Size = new System.Drawing.Size(1585, 32);
            this.toolStrip_yuk.TabIndex = 36;
            this.toolStrip_yuk.Text = "toolStrip1";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 32);
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
            this.gMapControl_yuk.Location = new System.Drawing.Point(263, 59);
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
            this.gMapControl_yuk.Size = new System.Drawing.Size(1074, 571);
            this.gMapControl_yuk.TabIndex = 34;
            this.gMapControl_yuk.Zoom = 0D;
            this.gMapControl_yuk.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_yuk_OnMapClick);
            this.gMapControl_yuk.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseDown);
            this.gMapControl_yuk.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseMove);
            this.gMapControl_yuk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseUp);
            // 
            // tab_rapor
            // 
            this.tab_rapor.Location = new System.Drawing.Point(4, 56);
            this.tab_rapor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_rapor.Name = "tab_rapor";
            this.tab_rapor.Size = new System.Drawing.Size(1585, 668);
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
            this.imageList.Images.SetKeyName(13, "");
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
            // ContextMenuStrip_Nokta
            // 
            this.ContextMenuStrip_Nokta.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Nokta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Nokta_Ekle,
            this.Nokta_Sil,
            this.Kaydet});
            this.ContextMenuStrip_Nokta.Name = "EA_ContextMenuStrip_Nokta";
            this.ContextMenuStrip_Nokta.Size = new System.Drawing.Size(154, 82);
            // 
            // ContextMenuStrip_Poligon
            // 
            this.ContextMenuStrip_Poligon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Poligon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Point_Load_Çiz,
            this.YGA_Çiz,
            this.Poligon_Sil,
            this.Poligon_Kaydet});
            this.ContextMenuStrip_Poligon.Name = "ContextMenuStrip_Poligon";
            this.ContextMenuStrip_Poligon.Size = new System.Drawing.Size(245, 136);
            // 
            // ContextMenuStrip_Fonksiyon
            // 
            this.ContextMenuStrip_Fonksiyon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Fonksiyon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.katman_birleştir,
            this.overlap_analizi});
            this.ContextMenuStrip_Fonksiyon.Name = "ContextMenuStrip_Fonksiyon";
            this.ContextMenuStrip_Fonksiyon.Size = new System.Drawing.Size(188, 56);
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
            this.ModuleTabPanel.Size = new System.Drawing.Size(1593, 728);
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
            this.HeaderPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.HeaderPanel.Controls.Add(this.HomePageButton);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(4);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1593, 38);
            this.HeaderPanel.TabIndex = 6;
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
            this.OpenModuleButton.Location = new System.Drawing.Point(1331, 610);
            this.OpenModuleButton.Margin = new System.Windows.Forms.Padding(4);
            this.OpenModuleButton.Name = "OpenModuleButton";
            this.OpenModuleButton.Size = new System.Drawing.Size(200, 49);
            this.OpenModuleButton.TabIndex = 19;
            this.OpenModuleButton.Text = "Modüle Git";
            this.OpenModuleButton.TextColor = System.Drawing.Color.White;
            this.OpenModuleButton.UseVisualStyleBackColor = false;
            this.OpenModuleButton.Click += new System.EventHandler(this.OpenModuleButton_Click);
            // 
            // DEKCenterAddButton
            // 
            this.DEKCenterAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKCenterAddButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKCenterAddButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKCenterAddButton.BorderRadius = 0;
            this.DEKCenterAddButton.BorderSize = 0;
            this.DEKCenterAddButton.FlatAppearance.BorderSize = 0;
            this.DEKCenterAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKCenterAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKCenterAddButton.ForeColor = System.Drawing.Color.White;
            this.DEKCenterAddButton.Location = new System.Drawing.Point(32, 450);
            this.DEKCenterAddButton.Margin = new System.Windows.Forms.Padding(4);
            this.DEKCenterAddButton.Name = "DEKCenterAddButton";
            this.DEKCenterAddButton.Size = new System.Drawing.Size(192, 49);
            this.DEKCenterAddButton.TabIndex = 55;
            this.DEKCenterAddButton.Text = "Dagıtık Üretim Merkezi Ekle ";
            this.DEKCenterAddButton.TextColor = System.Drawing.Color.White;
            this.DEKCenterAddButton.UseVisualStyleBackColor = false;
            this.DEKCenterAddButton.Click += new System.EventHandler(this.DEKCenterAddButton_Click);
            // 
            // DEKSimButton
            // 
            this.DEKSimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.DEKSimButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.DEKSimButton.BorderRadius = 0;
            this.DEKSimButton.BorderSize = 0;
            this.DEKSimButton.FlatAppearance.BorderSize = 0;
            this.DEKSimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEKSimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.DEKSimButton.ForeColor = System.Drawing.Color.White;
            this.DEKSimButton.Location = new System.Drawing.Point(32, 258);
            this.DEKSimButton.Margin = new System.Windows.Forms.Padding(4);
            this.DEKSimButton.Name = "DEKSimButton";
            this.DEKSimButton.Size = new System.Drawing.Size(192, 49);
            this.DEKSimButton.TabIndex = 54;
            this.DEKSimButton.Text = "DEK Gelecek Simülasyonu";
            this.DEKSimButton.TextColor = System.Drawing.Color.White;
            this.DEKSimButton.UseVisualStyleBackColor = false;
            this.DEKSimButton.Click += new System.EventHandler(this.dekSimulasyonGoruntule);
            // 
            // EAStationAddButton
            // 
            this.EAStationAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EAStationAddButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EAStationAddButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EAStationAddButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.EAStationAddButton.BorderRadius = 0;
            this.EAStationAddButton.BorderSize = 0;
            this.EAStationAddButton.FlatAppearance.BorderSize = 0;
            this.EAStationAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EAStationAddButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EAStationAddButton.ForeColor = System.Drawing.Color.White;
            this.EAStationAddButton.Location = new System.Drawing.Point(41, 246);
            this.EAStationAddButton.Margin = new System.Windows.Forms.Padding(4);
            this.EAStationAddButton.Name = "EAStationAddButton";
            this.EAStationAddButton.Size = new System.Drawing.Size(187, 52);
            this.EAStationAddButton.TabIndex = 49;
            this.EAStationAddButton.Text = "EA Şarj İstasyonu Ekle";
            this.EAStationAddButton.TextColor = System.Drawing.Color.White;
            this.EAStationAddButton.UseVisualStyleBackColor = false;
            this.EAStationAddButton.Click += new System.EventHandler(this.EAStationAddButton_Click);
            // 
            // EASimButton
            // 
            this.EASimButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EASimButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.EASimButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.EASimButton.BorderRadius = 0;
            this.EASimButton.BorderSize = 0;
            this.EASimButton.FlatAppearance.BorderSize = 0;
            this.EASimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EASimButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EASimButton.ForeColor = System.Drawing.Color.White;
            this.EASimButton.Location = new System.Drawing.Point(44, 250);
            this.EASimButton.Margin = new System.Windows.Forms.Padding(4);
            this.EASimButton.Name = "EASimButton";
            this.EASimButton.Size = new System.Drawing.Size(187, 52);
            this.EASimButton.TabIndex = 50;
            this.EASimButton.Text = "Gelecek Similasyonu Görüntüle";
            this.EASimButton.TextColor = System.Drawing.Color.White;
            this.EASimButton.UseVisualStyleBackColor = false;
            this.EASimButton.Click += new System.EventHandler(this.gelecekSimilasyonGoruntule);
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
            this.ELFShowGraphsButton.Location = new System.Drawing.Point(24, 309);
            this.ELFShowGraphsButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFShowGraphsButton.Name = "ELFShowGraphsButton";
            this.ELFShowGraphsButton.Size = new System.Drawing.Size(200, 49);
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
            this.SenaryoNewSelectionButton.Location = new System.Drawing.Point(24, 229);
            this.SenaryoNewSelectionButton.Margin = new System.Windows.Forms.Padding(4);
            this.SenaryoNewSelectionButton.Name = "SenaryoNewSelectionButton";
            this.SenaryoNewSelectionButton.Size = new System.Drawing.Size(200, 49);
            this.SenaryoNewSelectionButton.TabIndex = 33;
            this.SenaryoNewSelectionButton.Text = "Yeniden Senaryo Oluştur";
            this.SenaryoNewSelectionButton.TextColor = System.Drawing.Color.White;
            this.SenaryoNewSelectionButton.UseVisualStyleBackColor = false;
            this.SenaryoNewSelectionButton.Click += new System.EventHandler(this.SenaryoNewSelectionButton_Click);
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
            this.ShowResultsButton.Location = new System.Drawing.Point(21, 335);
            this.ShowResultsButton.Margin = new System.Windows.Forms.Padding(4);
            this.ShowResultsButton.Name = "ShowResultsButton";
            this.ShowResultsButton.Size = new System.Drawing.Size(200, 49);
            this.ShowResultsButton.TabIndex = 14;
            this.ShowResultsButton.Text = " Sonuçları Göster";
            this.ShowResultsButton.TextColor = System.Drawing.Color.White;
            this.ShowResultsButton.UseVisualStyleBackColor = false;
            this.ShowResultsButton.Click += new System.EventHandler(this.ShowResultsButton_Click);
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
            this.ELFPredictionShowResultsButton.Location = new System.Drawing.Point(21, 250);
            this.ELFPredictionShowResultsButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFPredictionShowResultsButton.Name = "ELFPredictionShowResultsButton";
            this.ELFPredictionShowResultsButton.Size = new System.Drawing.Size(200, 49);
            this.ELFPredictionShowResultsButton.TabIndex = 13;
            this.ELFPredictionShowResultsButton.Text = "Tahmin Yap/ Sonuçlarını Göster";
            this.ELFPredictionShowResultsButton.TextColor = System.Drawing.Color.White;
            this.ELFPredictionShowResultsButton.UseVisualStyleBackColor = false;
            this.ELFPredictionShowResultsButton.Visible = false;
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
            this.ELFScenerioSaveButton.Location = new System.Drawing.Point(21, 162);
            this.ELFScenerioSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFScenerioSaveButton.Name = "ELFScenerioSaveButton";
            this.ELFScenerioSaveButton.Size = new System.Drawing.Size(200, 49);
            this.ELFScenerioSaveButton.TabIndex = 12;
            this.ELFScenerioSaveButton.Text = "Senaryo Değişikliklerini Kaydet";
            this.ELFScenerioSaveButton.TextColor = System.Drawing.Color.White;
            this.ELFScenerioSaveButton.UseVisualStyleBackColor = false;
            this.ELFScenerioSaveButton.Click += new System.EventHandler(this.ELFScenerioSaveButton_Click);
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
            // pictureBox_ELF_5
            // 
            this.pictureBox_ELF_5.Location = new System.Drawing.Point(699, 135);
            this.pictureBox_ELF_5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox_ELF_5.Name = "pictureBox_ELF_5";
            this.pictureBox_ELF_5.Size = new System.Drawing.Size(341, 282);
            this.pictureBox_ELF_5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_5.TabIndex = 4;
            this.pictureBox_ELF_5.TabStop = false;
            // 
            // pictureBox_ELF_4
            // 
            this.pictureBox_ELF_4.Location = new System.Drawing.Point(351, 290);
            this.pictureBox_ELF_4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox_ELF_4.Name = "pictureBox_ELF_4";
            this.pictureBox_ELF_4.Size = new System.Drawing.Size(341, 282);
            this.pictureBox_ELF_4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_4.TabIndex = 3;
            this.pictureBox_ELF_4.TabStop = false;
            // 
            // pictureBox_ELF_3
            // 
            this.pictureBox_ELF_3.Location = new System.Drawing.Point(351, 2);
            this.pictureBox_ELF_3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox_ELF_3.Name = "pictureBox_ELF_3";
            this.pictureBox_ELF_3.Size = new System.Drawing.Size(341, 282);
            this.pictureBox_ELF_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_3.TabIndex = 2;
            this.pictureBox_ELF_3.TabStop = false;
            // 
            // pictureBox_ELF_2
            // 
            this.pictureBox_ELF_2.Location = new System.Drawing.Point(3, 290);
            this.pictureBox_ELF_2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox_ELF_2.Name = "pictureBox_ELF_2";
            this.pictureBox_ELF_2.Size = new System.Drawing.Size(341, 282);
            this.pictureBox_ELF_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_2.TabIndex = 1;
            this.pictureBox_ELF_2.TabStop = false;
            // 
            // pictureBox_ELF_1
            // 
            this.pictureBox_ELF_1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox_ELF_1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox_ELF_1.Name = "pictureBox_ELF_1";
            this.pictureBox_ELF_1.Size = new System.Drawing.Size(341, 282);
            this.pictureBox_ELF_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ELF_1.TabIndex = 0;
            this.pictureBox_ELF_1.TabStop = false;
            // 
            // buton_imar_katmanlar
            // 
            this.buton_imar_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_imar_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_imar_katmanlar.BackgroundImage")));
            this.buton_imar_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_imar_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_imar_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_imar_katmanlar.Location = new System.Drawing.Point(3, 889);
            this.buton_imar_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_imar_katmanlar.Name = "buton_imar_katmanlar";
            this.buton_imar_katmanlar.Size = new System.Drawing.Size(59, 52);
            this.buton_imar_katmanlar.TabIndex = 62;
            this.buton_imar_katmanlar.UseVisualStyleBackColor = true;
            // 
            // tabloyuGörToolStripMenuItem
            // 
            this.tabloyuGörToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabloyuGörToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tabloyuGörToolStripMenuItem.Image")));
            this.tabloyuGörToolStripMenuItem.Name = "tabloyuGörToolStripMenuItem";
            this.tabloyuGörToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.tabloyuGörToolStripMenuItem.Text = "Tabloyu Gör";
            this.tabloyuGörToolStripMenuItem.Click += new System.EventHandler(this.tabloyuGörToolStripMenuItem_Click);
            // 
            // rengiDeğiştirToolStripMenuItem
            // 
            this.rengiDeğiştirToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rengiDeğiştirToolStripMenuItem.Image")));
            this.rengiDeğiştirToolStripMenuItem.Name = "rengiDeğiştirToolStripMenuItem";
            this.rengiDeğiştirToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.rengiDeğiştirToolStripMenuItem.Text = "Rengi Değiştir";
            this.rengiDeğiştirToolStripMenuItem.Click += new System.EventHandler(this.rengiDeğiştirToolStripMenuItem_Click);
            // 
            // temizleToolStripMenuItem
            // 
            this.temizleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("temizleToolStripMenuItem.Image")));
            this.temizleToolStripMenuItem.Name = "temizleToolStripMenuItem";
            this.temizleToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.temizleToolStripMenuItem.Text = "Temizle";
            this.temizleToolStripMenuItem.Click += new System.EventHandler(this.temizleToolStripMenuItem_Click);
            // 
            // yenidenAdlandırToolStripMenuItem
            // 
            this.yenidenAdlandırToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("yenidenAdlandırToolStripMenuItem.Image")));
            this.yenidenAdlandırToolStripMenuItem.Name = "yenidenAdlandırToolStripMenuItem";
            this.yenidenAdlandırToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.yenidenAdlandırToolStripMenuItem.Text = "Yeniden Adlandır";
            this.yenidenAdlandırToolStripMenuItem.Click += new System.EventHandler(this.yenidenAdlandırToolStripMenuItem_Click);
            // 
            // kaydetToolStripMenuItem
            // 
            this.kaydetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("kaydetToolStripMenuItem.Image")));
            this.kaydetToolStripMenuItem.Name = "kaydetToolStripMenuItem";
            this.kaydetToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.kaydetToolStripMenuItem.Text = "Kaydet";
            this.kaydetToolStripMenuItem.Click += new System.EventHandler(this.kaydetToolStripMenuItem_Click);
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
            this.İmar_Nokta.Size = new System.Drawing.Size(94, 29);
            this.İmar_Nokta.Tag = "";
            this.İmar_Nokta.Text = "Nokta";
            this.İmar_Nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            this.İmar_Nokta.MouseDown += new System.Windows.Forms.MouseEventHandler(this.İmar_Nokta_MouseDown);
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
            this.İmar_Grid_Oluştur.Size = new System.Drawing.Size(142, 29);
            this.İmar_Grid_Oluştur.Tag = "";
            this.İmar_Grid_Oluştur.Text = "Grid Oluştur";
            this.İmar_Grid_Oluştur.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Grid_Oluştur.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Grid_Oluştur.ToolTipText = "Belirli bir alan seçilip bu alanda mxn şeklinde bir grid (ızgara) tanımlar.";
            this.İmar_Grid_Oluştur.Click += new System.EventHandler(this.İmar_Grid_Oluştur_Click);
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
            this.Yuk_Seç.Size = new System.Drawing.Size(73, 29);
            this.Yuk_Seç.Tag = "";
            this.Yuk_Seç.Text = "Seç";
            this.Yuk_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.Yuk_Seç.Click += new System.EventHandler(this.Yuk_Seç_Click);
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
            this.Yuk_Kaydır.Size = new System.Drawing.Size(95, 29);
            this.Yuk_Kaydır.Tag = "";
            this.Yuk_Kaydır.Text = "Kaydır";
            this.Yuk_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
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
            this.Yuk_Mesafe_Ölç.Size = new System.Drawing.Size(134, 29);
            this.Yuk_Mesafe_Ölç.Tag = "";
            this.Yuk_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.Yuk_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Yuk_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Yuk_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            // 
            // buton_yuk_haritası_katmanlar
            // 
            this.buton_yuk_haritası_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_yuk_haritası_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_yuk_haritası_katmanlar.BackgroundImage")));
            this.buton_yuk_haritası_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_yuk_haritası_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_yuk_haritası_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_yuk_haritası_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_yuk_haritası_katmanlar.Location = new System.Drawing.Point(263, 578);
            this.buton_yuk_haritası_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_yuk_haritası_katmanlar.Name = "buton_yuk_haritası_katmanlar";
            this.buton_yuk_haritası_katmanlar.Size = new System.Drawing.Size(59, 52);
            this.buton_yuk_haritası_katmanlar.TabIndex = 35;
            this.buton_yuk_haritası_katmanlar.UseVisualStyleBackColor = true;
            // 
            // HomePageButton
            // 
            this.HomePageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HomePageButton.BackColor = System.Drawing.Color.NavajoWhite;
            this.HomePageButton.BackgroundColor = System.Drawing.Color.NavajoWhite;
            this.HomePageButton.BackgroundImage = global::SLF.Properties.Resources.homepage__1_;
            this.HomePageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.HomePageButton.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.HomePageButton.BorderRadius = 0;
            this.HomePageButton.BorderSize = 0;
            this.HomePageButton.FlatAppearance.BorderSize = 0;
            this.HomePageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HomePageButton.ForeColor = System.Drawing.Color.White;
            this.HomePageButton.Location = new System.Drawing.Point(1538, 4);
            this.HomePageButton.Margin = new System.Windows.Forms.Padding(4);
            this.HomePageButton.Name = "HomePageButton";
            this.HomePageButton.Size = new System.Drawing.Size(45, 34);
            this.HomePageButton.TabIndex = 5;
            this.HomePageButton.TextColor = System.Drawing.Color.White;
            this.HomePageButton.UseVisualStyleBackColor = false;
            this.HomePageButton.Click += new System.EventHandler(this.HomePageButton_Click);
            // 
            // Arazi
            // 
            this.Arazi.Image = ((System.Drawing.Image)(resources.GetObject("Arazi.Image")));
            this.Arazi.Name = "Arazi";
            this.Arazi.Size = new System.Drawing.Size(195, 26);
            this.Arazi.Text = "Arazi";
            this.Arazi.Click += new System.EventHandler(this.Arazi_Click);
            // 
            // Google_Earth
            // 
            this.Google_Earth.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth.Image")));
            this.Google_Earth.Name = "Google_Earth";
            this.Google_Earth.Size = new System.Drawing.Size(195, 26);
            this.Google_Earth.Text = "GE Online";
            this.Google_Earth.Click += new System.EventHandler(this.Google_Earth_Click);
            // 
            // Google_Earth_Desktop
            // 
            this.Google_Earth_Desktop.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth_Desktop.Image")));
            this.Google_Earth_Desktop.Name = "Google_Earth_Desktop";
            this.Google_Earth_Desktop.Size = new System.Drawing.Size(195, 26);
            this.Google_Earth_Desktop.Text = "GE Pro Desktop";
            this.Google_Earth_Desktop.Click += new System.EventHandler(this.Google_Earth_Desktop_Click);
            // 
            // Harita
            // 
            this.Harita.Image = ((System.Drawing.Image)(resources.GetObject("Harita.Image")));
            this.Harita.Name = "Harita";
            this.Harita.Size = new System.Drawing.Size(195, 26);
            this.Harita.Text = "Harita";
            this.Harita.Click += new System.EventHandler(this.Harita_Click);
            // 
            // OSM
            // 
            this.OSM.Image = ((System.Drawing.Image)(resources.GetObject("OSM.Image")));
            this.OSM.Name = "OSM";
            this.OSM.Size = new System.Drawing.Size(195, 26);
            this.OSM.Text = "Open Street Map";
            this.OSM.Click += new System.EventHandler(this.OSM_Click);
            // 
            // Sokak_Görünümü
            // 
            this.Sokak_Görünümü.Image = ((System.Drawing.Image)(resources.GetObject("Sokak_Görünümü.Image")));
            this.Sokak_Görünümü.Name = "Sokak_Görünümü";
            this.Sokak_Görünümü.Size = new System.Drawing.Size(195, 26);
            this.Sokak_Görünümü.Text = "Sokak Görünümü";
            this.Sokak_Görünümü.Click += new System.EventHandler(this.Sokak_Görünümü_Click);
            // 
            // Uydu
            // 
            this.Uydu.Image = ((System.Drawing.Image)(resources.GetObject("Uydu.Image")));
            this.Uydu.Name = "Uydu";
            this.Uydu.Size = new System.Drawing.Size(195, 26);
            this.Uydu.Text = "Uydu";
            this.Uydu.Click += new System.EventHandler(this.Uydu_Click);
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
            this.EA_Seç.Size = new System.Drawing.Size(73, 29);
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
            this.EA_Kaydır.Size = new System.Drawing.Size(95, 29);
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
            this.EA_Mesafe_Ölç.Size = new System.Drawing.Size(134, 29);
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
            this.EA_Poligon.Size = new System.Drawing.Size(106, 29);
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
            this.EA_Nokta.Size = new System.Drawing.Size(94, 29);
            this.EA_Nokta.Tag = "";
            this.EA_Nokta.Text = "Nokta";
            this.EA_Nokta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Nokta.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Nokta.ToolTipText = "Haritaya tıklanarak nokta/marker eklemeye veya silmeye yarar.";
            this.EA_Nokta.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EA_Nokta_MouseDown);
            // 
            // Nokta_Ekle
            // 
            this.Nokta_Ekle.Image = ((System.Drawing.Image)(resources.GetObject("Nokta_Ekle.Image")));
            this.Nokta_Ekle.Name = "Nokta_Ekle";
            this.Nokta_Ekle.Size = new System.Drawing.Size(153, 26);
            this.Nokta_Ekle.Text = "Nokta Ekle";
            this.Nokta_Ekle.Click += new System.EventHandler(this.Nokta_Ekle_Click);
            // 
            // Nokta_Sil
            // 
            this.Nokta_Sil.Image = ((System.Drawing.Image)(resources.GetObject("Nokta_Sil.Image")));
            this.Nokta_Sil.Name = "Nokta_Sil";
            this.Nokta_Sil.Size = new System.Drawing.Size(153, 26);
            this.Nokta_Sil.Text = "Nokta Sil";
            // 
            // Kaydet
            // 
            this.Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Kaydet.Image")));
            this.Kaydet.Name = "Kaydet";
            this.Kaydet.Size = new System.Drawing.Size(153, 26);
            this.Kaydet.Text = "Kaydet";
            // 
            // Point_Load_Çiz
            // 
            this.Point_Load_Çiz.Image = ((System.Drawing.Image)(resources.GetObject("Point_Load_Çiz.Image")));
            this.Point_Load_Çiz.Name = "Point_Load_Çiz";
            this.Point_Load_Çiz.Size = new System.Drawing.Size(244, 26);
            this.Point_Load_Çiz.Text = "Nokta Yük Ekle";
            this.Point_Load_Çiz.Click += new System.EventHandler(this.Point_Load_Çiz_Click);
            // 
            // YGA_Çiz
            // 
            this.YGA_Çiz.Image = ((System.Drawing.Image)(resources.GetObject("YGA_Çiz.Image")));
            this.YGA_Çiz.Name = "YGA_Çiz";
            this.YGA_Çiz.Size = new System.Drawing.Size(244, 26);
            this.YGA_Çiz.Text = "Yeni Genişleme Alanı Çiz";
            this.YGA_Çiz.Click += new System.EventHandler(this.YGA_Çiz_Click);
            // 
            // Poligon_Sil
            // 
            this.Poligon_Sil.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Sil.Image")));
            this.Poligon_Sil.Name = "Poligon_Sil";
            this.Poligon_Sil.Size = new System.Drawing.Size(244, 26);
            this.Poligon_Sil.Text = "Poligon Sil";
            this.Poligon_Sil.Click += new System.EventHandler(this.Poligon_Sil_Click);
            // 
            // Poligon_Kaydet
            // 
            this.Poligon_Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Kaydet.Image")));
            this.Poligon_Kaydet.Name = "Poligon_Kaydet";
            this.Poligon_Kaydet.Size = new System.Drawing.Size(244, 26);
            this.Poligon_Kaydet.Text = "Poligon Kaydet";
            this.Poligon_Kaydet.Click += new System.EventHandler(this.Poligon_Kaydet_Click);
            // 
            // katman_birleştir
            // 
            this.katman_birleştir.Image = ((System.Drawing.Image)(resources.GetObject("katman_birleştir.Image")));
            this.katman_birleştir.Name = "katman_birleştir";
            this.katman_birleştir.Size = new System.Drawing.Size(187, 26);
            this.katman_birleştir.Text = "Katman Birleştir";
            this.katman_birleştir.Click += new System.EventHandler(this.katman_birleştir_Click);
            // 
            // overlap_analizi
            // 
            this.overlap_analizi.Image = ((System.Drawing.Image)(resources.GetObject("overlap_analizi.Image")));
            this.overlap_analizi.Name = "overlap_analizi";
            this.overlap_analizi.Size = new System.Drawing.Size(187, 26);
            this.overlap_analizi.Text = "Overlap Analizi";
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
            this.ClientSize = new System.Drawing.Size(1593, 766);
            this.Controls.Add(this.ModuleTabPanel);
            this.Controls.Add(this.HeaderPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1611, 813);
            this.Name = "ModülFormu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jeo-Uzamsal Talep Tahmini Yazılımı           ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
            this.ELFGraphicsPanel.ResumeLayout(false);
            this.ELFGraphicsPanel.PerformLayout();
            this.tab_imar.ResumeLayout(false);
            this.tab_imar.PerformLayout();
            this.panel_imar.ResumeLayout(false);
            this.panel_imar.PerformLayout();
            this.harita_katmanları_right_click.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).EndInit();
            this.katmanlar_right_click.ResumeLayout(false);
            this.toolStrip_imar.ResumeLayout(false);
            this.toolStrip_imar.PerformLayout();
            this.tab_optDTR.ResumeLayout(false);
            this.tab_optDTR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_optimalDTR)).EndInit();
            this.tab_senaryo.ResumeLayout(false);
            this.SenaryoModulePanel.ResumeLayout(false);
            this.EkonometrikSenaryoElementsPanel.ResumeLayout(false);
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
            this.tab_yükHaritası.ResumeLayout(false);
            this.tab_yükHaritası.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).EndInit();
            this.legendPanel.ResumeLayout(false);
            this.legendPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).EndInit();
            this.toolStrip_yuk.ResumeLayout(false);
            this.toolStrip_yuk.PerformLayout();
            this.Toolbox_EA.ResumeLayout(false);
            this.Toolbox_EA.PerformLayout();
            this.ContextMenuStrip_Nokta.ResumeLayout(false);
            this.ContextMenuStrip_Poligon.ResumeLayout(false);
            this.ContextMenuStrip_Fonksiyon.ResumeLayout(false);
            this.ModuleTabPanel.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ELF_1)).EndInit();
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
        private System.Windows.Forms.Panel ELFTablePanel;
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
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Nokta;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Ekle;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Sil;
        private System.Windows.Forms.ToolStripMenuItem Kaydet;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Poligon;
        private System.Windows.Forms.ToolStripMenuItem Point_Load_Çiz;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Sil;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Kaydet;
        private ContextMenuStrip ContextMenuStrip_Fonksiyon;
        private ToolStripMenuItem katman_birleştir;
        private ToolStripMenuItem overlap_analizi;
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
        public GMap.NET.WindowsForms.GMapControl gMapControl_optimalDTR;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_optimalDTR;
        private ToolStrip toolStrip_imar;
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
        private Label AddStationLabel;
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
        private CustomButton ELFScenerioSaveButton;
        private CustomButton ELFPredictionShowResultsButton;
        private CustomButton ShowResultsButton;
        private CustomButton SenaryoNewSelectionButton;
        private CustomButton ELFShowGraphsButton;
        private CustomButton EAStationAddButton;
        private CustomButton EASimButton;
        private CustomButton DEKSimButton;
        private CustomButton DEKCenterAddButton;
        private Panel EAStationsLegendPanel;
        private Label ACHomeLegendValueLabel;
        private Label ACHomeLegendLabel;
        private Label ACWorkLegendValueLabel;
        private Label ACWorkLegendLabel;
        private Label DCFastLegendValueLabel;
        private Label DCFastLegendLabel;
        private Label ACPublicLegendValueLabel;
        private Label ACPublicLegendLabel;
        private RadioButton dekSimMaxBtn;
        private RadioButton dekSimDefBtn;
        private RadioButton dekSimMinBtn;
        private RadioButton EaSimMaxBtn;
        private RadioButton EaSimDefBtn;
        private RadioButton EaSimMinBtn;
        private CustomButton HomePageButton;
        private ToolStrip miniToolStrip;
        private CheckBox checkBox_imar_15;
        private CheckBox checkBox_imar_14;
        public GMap.NET.WindowsForms.GMapControl gMapControl_imar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_imar;
        private Button buton_imar_katmanlar;
        private Panel panel_imar;
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
        private Label label1;
        private CheckBox checkBox_yuk_8;
        private CheckBox checkBox_yuk_6;
        private CheckBox checkBox_yuk_11;
        private CheckBox checkBox_yuk_14;
        private CheckBox checkBox_yuk_13;
        private CheckBox checkBox_yuk_15;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_yuk;
        private ToolStripMenuItem YGA_Çiz;
    }
}
