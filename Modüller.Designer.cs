using System;
using System.Windows.Forms;

namespace SLF
{
    partial class ModülFormu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModülFormu));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yardımToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Modül_Tabları = new System.Windows.Forms.TabControl();
            this.tab_girdi = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.OpenModuleButtonPanel = new System.Windows.Forms.Panel();
            this.OpenModuleButton = new System.Windows.Forms.Button();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.raporGoruntuleButonu = new System.Windows.Forms.Button();
            this.dataGridView_girdi = new System.Windows.Forms.DataGridView();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.ExcelDownloadButton = new System.Windows.Forms.Button();
            this.csvExportButton = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.startYearComboBox = new System.Windows.Forms.ComboBox();
            this.yearApproveButton = new System.Windows.Forms.Button();
            this.endYearComboBox = new System.Windows.Forms.ComboBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.veri_listesi_seçimi = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectFolderButton = new System.Windows.Forms.Button();
            this.tab_dek = new System.Windows.Forms.TabPage();
            this.button8 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.tab_ea = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.mesafe_metre_ea = new System.Windows.Forms.Label();
            this.Mesafe_ea = new System.Windows.Forms.Label();
            this.Toolbox_EA = new System.Windows.Forms.ToolStrip();
            this.EA_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton15 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton16 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.buton_ea_harita_katmanlar = new System.Windows.Forms.Button();
            this.harita_katmanları_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Arazi = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth = new System.Windows.Forms.ToolStripMenuItem();
            this.Google_Earth_Desktop = new System.Windows.Forms.ToolStripMenuItem();
            this.Harita = new System.Windows.Forms.ToolStripMenuItem();
            this.OSM = new System.Windows.Forms.ToolStripMenuItem();
            this.Sokak_Görünümü = new System.Windows.Forms.ToolStripMenuItem();
            this.Uydu = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonKml = new System.Windows.Forms.Button();
            this.oznitelikAc = new System.Windows.Forms.Button();
            this.EA_list_box = new System.Windows.Forms.ListBox();
            this.gMapControl_EA = new GMap.NET.WindowsForms.GMapControl();
            this.button7 = new System.Windows.Forms.Button();
            this.tab_ekonometrik = new System.Windows.Forms.TabPage();
            this.ELFTablePanel = new System.Windows.Forms.Panel();
            this.ELFResultsTabControls = new System.Windows.Forms.TabControl();
            this.ELFMinResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFMinResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFMinSenaryoGraphPicBox = new System.Windows.Forms.PictureBox();
            this.ELFLowResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFLowResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFBaseResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFBaseResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFHighResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFHighResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFMaxResultsTabPage = new System.Windows.Forms.TabPage();
            this.ELFMaxResultsTable = new System.Windows.Forms.DataGridView();
            this.ELFGraphicsPanel = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ELFRadioButtonsPanel = new System.Windows.Forms.Panel();
            this.ELFLowSenaryoRadioButton = new Guna.UI2.WinForms.Guna2RadioButton();
            this.ELFMaxSenaryoRadioButton = new Guna.UI2.WinForms.Guna2RadioButton();
            this.ELFMinSenaryoRadioButton = new Guna.UI2.WinForms.Guna2RadioButton();
            this.ELFHighSenaryoRadioButton = new Guna.UI2.WinForms.Guna2RadioButton();
            this.ELFBaseSenaryoRadioButton = new Guna.UI2.WinForms.Guna2RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.ELFPredictionButton = new System.Windows.Forms.Button();
            this.SenaryoSelectionButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tab_imar = new System.Windows.Forms.TabPage();
            this.Mesafe_imar = new System.Windows.Forms.Label();
            this.mesafe_metre_imar = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.katmanlar_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabloyuGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rengiDeğiştirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temizleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yenidenAdlandırToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox22 = new System.Windows.Forms.CheckBox();
            this.checkBox23 = new System.Windows.Forms.CheckBox();
            this.checkBox24 = new System.Windows.Forms.CheckBox();
            this.checkBox25 = new System.Windows.Forms.CheckBox();
            this.checkBox26 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.imar_dosya_seçimi = new System.Windows.Forms.Button();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.İmar_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Grid_Oluştur = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.İmar_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.buton_imar_katmanlar = new System.Windows.Forms.Button();
            this.webView_imar = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.gMapControl_imar = new GMap.NET.WindowsForms.GMapControl();
            this.tab_optDTR = new System.Windows.Forms.TabPage();
            this.gMapControl_optimalDTR = new GMap.NET.WindowsForms.GMapControl();
            this.buton_optimalDTR_katmanlar = new System.Windows.Forms.Button();
            this.webView_optimalDTR = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tab_senaryo = new System.Windows.Forms.TabPage();
            this.SenaryoModulePanel = new System.Windows.Forms.Panel();
            this.SenaryoModuleTabControl = new Guna.UI2.WinForms.Guna2TabControl();
            this.EkonometrikSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.EkonometrikSenaryoOutputsPanel = new System.Windows.Forms.Panel();
            this.ELFSenaryoTabControls = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ELFMinSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ELFLowSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ELFBaseSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ELFHighSenaryoTable = new System.Windows.Forms.DataGridView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.ELFMaxSenaryoTable = new System.Windows.Forms.DataGridView();
            this.EkonometrikSenaryoElementsPanel = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ELFPredictionShowResultsGunaButton = new Guna.UI2.WinForms.Guna2Button();
            this.ELFScenerioSaveGunaButton = new Guna.UI2.WinForms.Guna2Button();
            this.StokastikSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.EASarjSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.DEKSenaryoTabPage = new System.Windows.Forms.TabPage();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.tab_stokastik = new System.Windows.Forms.TabPage();
            this.webView_stokastik = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.checkBox21 = new System.Windows.Forms.CheckBox();
            this.checkBox20 = new System.Windows.Forms.CheckBox();
            this.checkBox19 = new System.Windows.Forms.CheckBox();
            this.mesafe_metre_stokastik = new System.Windows.Forms.Label();
            this.Mesafe_stokastik = new System.Windows.Forms.Label();
            this.gMapControl_stokastik = new GMap.NET.WindowsForms.GMapControl();
            this.buton_stokastik_harita_katmanlar = new System.Windows.Forms.Button();
            this.checkBox18 = new System.Windows.Forms.CheckBox();
            this.checkBox17 = new System.Windows.Forms.CheckBox();
            this.checkBox16 = new System.Windows.Forms.CheckBox();
            this.checkBox15 = new System.Windows.Forms.CheckBox();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.stokastik_dosya_seçimi = new System.Windows.Forms.Button();
            this.Toolbox_Stokastik = new System.Windows.Forms.ToolStrip();
            this.Stokastik_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Poligon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Nokta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Grid_Oluştur = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.Stokastik_Fonksiyonlar = new System.Windows.Forms.ToolStripButton();
            this.tab_yükHaritası = new System.Windows.Forms.TabPage();
            this.checkBox28 = new System.Windows.Forms.CheckBox();
            this.checkBox27 = new System.Windows.Forms.CheckBox();
            this.yuk_yıl_deger = new System.Windows.Forms.Label();
            this.yuk_yıl_text = new System.Windows.Forms.Label();
            this.trackBar_Yıllar = new System.Windows.Forms.TrackBar();
            this.Mesafe_yuk = new System.Windows.Forms.Label();
            this.mesafe_metre_yuk = new System.Windows.Forms.Label();
            this.Toolbox_Yuk = new System.Windows.Forms.ToolStrip();
            this.Yuk_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Yuk_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.Yuk_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.buton_yuk_haritası_katmanlar = new System.Windows.Forms.Button();
            this.gMapControl_yuk = new GMap.NET.WindowsForms.GMapControl();
            this.webView_yuk = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tab_rapor = new System.Windows.Forms.TabPage();
            this.tab_validasyon = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.HomePageButton = new System.Windows.Forms.Button();
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
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.legendPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.Modül_Tabları.SuspendLayout();
            this.tab_girdi.SuspendLayout();
            this.OpenModuleButtonPanel.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_girdi)).BeginInit();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tab_dek.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tab_ea.SuspendLayout();
            this.Toolbox_EA.SuspendLayout();
            this.harita_katmanları_right_click.SuspendLayout();
            this.tab_ekonometrik.SuspendLayout();
            this.ELFTablePanel.SuspendLayout();
            this.ELFResultsTabControls.SuspendLayout();
            this.ELFMinResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinResultsTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoGraphPicBox)).BeginInit();
            this.ELFLowResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowResultsTable)).BeginInit();
            this.ELFBaseResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseResultsTable)).BeginInit();
            this.ELFHighResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighResultsTable)).BeginInit();
            this.ELFMaxResultsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxResultsTable)).BeginInit();
            this.ELFGraphicsPanel.SuspendLayout();
            this.ELFRadioButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tab_imar.SuspendLayout();
            this.katmanlar_right_click.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).BeginInit();
            this.tab_optDTR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_optimalDTR)).BeginInit();
            this.tab_senaryo.SuspendLayout();
            this.SenaryoModulePanel.SuspendLayout();
            this.SenaryoModuleTabControl.SuspendLayout();
            this.EkonometrikSenaryoTabPage.SuspendLayout();
            this.EkonometrikSenaryoOutputsPanel.SuspendLayout();
            this.ELFSenaryoTabControls.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoTable)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowSenaryoTable)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseSenaryoTable)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighSenaryoTable)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxSenaryoTable)).BeginInit();
            this.EkonometrikSenaryoElementsPanel.SuspendLayout();
            this.tab_stokastik.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_stokastik)).BeginInit();
            this.Toolbox_Stokastik.SuspendLayout();
            this.tab_yükHaritası.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).BeginInit();
            this.Toolbox_Yuk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).BeginInit();
            this.ContextMenuStrip_Nokta.SuspendLayout();
            this.ContextMenuStrip_Poligon.SuspendLayout();
            this.ContextMenuStrip_Fonksiyon.SuspendLayout();
            this.ModuleTabPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.BackColor = System.Drawing.Color.LightSalmon;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.yardımToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(140, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.saToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.fileToolStripMenuItem.Text = "Dosya";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.importToolStripMenuItem.Text = "İçeri Aktar";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.exportToolStripMenuItem.Text = "Dışarı Aktar";
            // 
            // saToolStripMenuItem
            // 
            this.saToolStripMenuItem.Name = "saToolStripMenuItem";
            this.saToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.saToolStripMenuItem.Text = "Kaydet";
            // 
            // yardımToolStripMenuItem
            // 
            this.yardımToolStripMenuItem.Name = "yardımToolStripMenuItem";
            this.yardımToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.yardımToolStripMenuItem.Text = "Yardım";
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
            this.Modül_Tabları.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Modül_Tabları.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Modül_Tabları.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Modül_Tabları.HotTrack = true;
            this.Modül_Tabları.Location = new System.Drawing.Point(0, 0);
            this.Modül_Tabları.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Modül_Tabları.Multiline = true;
            this.Modül_Tabları.Name = "Modül_Tabları";
            this.Modül_Tabları.Padding = new System.Drawing.Point(20, 3);
            this.Modül_Tabları.SelectedIndex = 0;
            this.Modül_Tabları.Size = new System.Drawing.Size(1320, 688);
            this.Modül_Tabları.TabIndex = 2;
            this.Modül_Tabları.SelectedIndexChanged += new System.EventHandler(this.Modül_Tabları_SelectedIndexChanged);
            // 
            // tab_girdi
            // 
            this.tab_girdi.AutoScroll = true;
            this.tab_girdi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tab_girdi.Controls.Add(this.label3);
            this.tab_girdi.Controls.Add(this.OpenModuleButtonPanel);
            this.tab_girdi.Controls.Add(this.panel9);
            this.tab_girdi.Controls.Add(this.dataGridView_girdi);
            this.tab_girdi.Controls.Add(this.panel8);
            this.tab_girdi.Controls.Add(this.panel6);
            this.tab_girdi.Controls.Add(this.panel7);
            this.tab_girdi.Font = new System.Drawing.Font("Maiandra GD", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_girdi.Location = new System.Drawing.Point(4, 56);
            this.tab_girdi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_girdi.Name = "tab_girdi";
            this.tab_girdi.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_girdi.Size = new System.Drawing.Size(1312, 628);
            this.tab_girdi.TabIndex = 0;
            this.tab_girdi.Text = "Girdi Modülü";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Maiandra GD", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Veri Önizleme:";
            // 
            // OpenModuleButtonPanel
            // 
            this.OpenModuleButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenModuleButtonPanel.Controls.Add(this.OpenModuleButton);
            this.OpenModuleButtonPanel.Location = new System.Drawing.Point(1019, 569);
            this.OpenModuleButtonPanel.Margin = new System.Windows.Forms.Padding(4);
            this.OpenModuleButtonPanel.Name = "OpenModuleButtonPanel";
            this.OpenModuleButtonPanel.Size = new System.Drawing.Size(148, 44);
            this.OpenModuleButtonPanel.TabIndex = 17;
            // 
            // OpenModuleButton
            // 
            this.OpenModuleButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OpenModuleButton.Location = new System.Drawing.Point(0, 0);
            this.OpenModuleButton.Margin = new System.Windows.Forms.Padding(4);
            this.OpenModuleButton.Name = "OpenModuleButton";
            this.OpenModuleButton.Size = new System.Drawing.Size(148, 44);
            this.OpenModuleButton.TabIndex = 16;
            this.OpenModuleButton.Text = "Modüle Git";
            this.OpenModuleButton.UseVisualStyleBackColor = true;
            this.OpenModuleButton.Click += new System.EventHandler(this.OpenModuleButton_Click);
            // 
            // panel9
            // 
            this.panel9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel9.Controls.Add(this.label17);
            this.panel9.Controls.Add(this.raporGoruntuleButonu);
            this.panel9.Location = new System.Drawing.Point(1141, 11);
            this.panel9.Margin = new System.Windows.Forms.Padding(4);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(129, 81);
            this.panel9.TabIndex = 15;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 6);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(120, 23);
            this.label17.TabIndex = 7;
            this.label17.Text = "Rapor Oluştur:";
            // 
            // raporGoruntuleButonu
            // 
            this.raporGoruntuleButonu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.raporGoruntuleButonu.BackColor = System.Drawing.Color.White;
            this.raporGoruntuleButonu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("raporGoruntuleButonu.BackgroundImage")));
            this.raporGoruntuleButonu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.raporGoruntuleButonu.ForeColor = System.Drawing.Color.Transparent;
            this.raporGoruntuleButonu.Location = new System.Drawing.Point(43, 28);
            this.raporGoruntuleButonu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.raporGoruntuleButonu.Name = "raporGoruntuleButonu";
            this.raporGoruntuleButonu.Size = new System.Drawing.Size(56, 44);
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
            this.dataGridView_girdi.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView_girdi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_girdi.Location = new System.Drawing.Point(9, 118);
            this.dataGridView_girdi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView_girdi.Name = "dataGridView_girdi";
            this.dataGridView_girdi.ReadOnly = true;
            this.dataGridView_girdi.RowHeadersWidth = 51;
            this.dataGridView_girdi.RowTemplate.Height = 24;
            this.dataGridView_girdi.Size = new System.Drawing.Size(1261, 431);
            this.dataGridView_girdi.TabIndex = 4;
            // 
            // panel8
            // 
            this.panel8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel8.Controls.Add(this.label18);
            this.panel8.Controls.Add(this.ExcelDownloadButton);
            this.panel8.Controls.Add(this.csvExportButton);
            this.panel8.Location = new System.Drawing.Point(837, 33);
            this.panel8.Margin = new System.Windows.Forms.Padding(4);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(280, 59);
            this.panel8.TabIndex = 14;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(3, 4);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(91, 23);
            this.label18.TabIndex = 16;
            this.label18.Text = "Dışa Aktar:";
            // 
            // ExcelDownloadButton
            // 
            this.ExcelDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExcelDownloadButton.BackColor = System.Drawing.Color.White;
            this.ExcelDownloadButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ExcelDownloadButton.BackgroundImage")));
            this.ExcelDownloadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExcelDownloadButton.ForeColor = System.Drawing.Color.Transparent;
            this.ExcelDownloadButton.Location = new System.Drawing.Point(143, 0);
            this.ExcelDownloadButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExcelDownloadButton.Name = "ExcelDownloadButton";
            this.ExcelDownloadButton.Size = new System.Drawing.Size(56, 44);
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
            this.csvExportButton.Location = new System.Drawing.Point(204, 2);
            this.csvExportButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.csvExportButton.Name = "csvExportButton";
            this.csvExportButton.Size = new System.Drawing.Size(56, 44);
            this.csvExportButton.TabIndex = 8;
            this.csvExportButton.UseVisualStyleBackColor = false;
            this.csvExportButton.Click += new System.EventHandler(this.csvExportButton_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.startYearComboBox);
            this.panel6.Controls.Add(this.yearApproveButton);
            this.panel6.Controls.Add(this.endYearComboBox);
            this.panel6.Location = new System.Drawing.Point(4, 4);
            this.panel6.Margin = new System.Windows.Forms.Padding(4);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(306, 88);
            this.panel6.TabIndex = 12;
            // 
            // startYearComboBox
            // 
            this.startYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.startYearComboBox.FormattingEnabled = true;
            this.startYearComboBox.Location = new System.Drawing.Point(3, 6);
            this.startYearComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.startYearComboBox.Name = "startYearComboBox";
            this.startYearComboBox.Size = new System.Drawing.Size(121, 26);
            this.startYearComboBox.TabIndex = 9;
            this.startYearComboBox.Text = "Yıl seçiniz";
            this.startYearComboBox.SelectedIndexChanged += new System.EventHandler(this.startYearComboBox_SelectedIndexChanged);
            // 
            // yearApproveButton
            // 
            this.yearApproveButton.Location = new System.Drawing.Point(81, 41);
            this.yearApproveButton.Margin = new System.Windows.Forms.Padding(4);
            this.yearApproveButton.Name = "yearApproveButton";
            this.yearApproveButton.Size = new System.Drawing.Size(100, 34);
            this.yearApproveButton.TabIndex = 11;
            this.yearApproveButton.Text = "Onayla";
            this.yearApproveButton.UseVisualStyleBackColor = true;
            this.yearApproveButton.Click += new System.EventHandler(this.yearApproveButton_Click);
            // 
            // endYearComboBox
            // 
            this.endYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.endYearComboBox.FormattingEnabled = true;
            this.endYearComboBox.Location = new System.Drawing.Point(161, 6);
            this.endYearComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.endYearComboBox.Name = "endYearComboBox";
            this.endYearComboBox.Size = new System.Drawing.Size(121, 26);
            this.endYearComboBox.TabIndex = 10;
            this.endYearComboBox.Text = "Yıl seçiniz";
            this.endYearComboBox.SelectedIndexChanged += new System.EventHandler(this.endYearComboBox_SelectedIndexChanged);
            // 
            // panel7
            // 
            this.panel7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel7.Controls.Add(this.veri_listesi_seçimi);
            this.panel7.Controls.Add(this.label1);
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.SelectFolderButton);
            this.panel7.Location = new System.Drawing.Point(318, 4);
            this.panel7.Margin = new System.Windows.Forms.Padding(4);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(511, 88);
            this.panel7.TabIndex = 13;
            // 
            // veri_listesi_seçimi
            // 
            this.veri_listesi_seçimi.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.veri_listesi_seçimi.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.veri_listesi_seçimi.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.veri_listesi_seçimi.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.veri_listesi_seçimi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.veri_listesi_seçimi.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.veri_listesi_seçimi.Location = new System.Drawing.Point(20, 39);
            this.veri_listesi_seçimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.veri_listesi_seçimi.Name = "veri_listesi_seçimi";
            this.veri_listesi_seçimi.Size = new System.Drawing.Size(213, 32);
            this.veri_listesi_seçimi.TabIndex = 0;
            this.veri_listesi_seçimi.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.veri_listesi_seçimi_DrawItem);
            this.veri_listesi_seçimi.SelectedIndexChanged += new System.EventHandler(this.veri_listesi_seçimi_SelectedIndexChanged);
            this.veri_listesi_seçimi.MouseDown += new System.Windows.Forms.MouseEventHandler(this.veri_listesi_seçimi_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dosya Veri Tipi Seçimi:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(279, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Dosya Seçimi:";
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.BackColor = System.Drawing.Color.White;
            this.SelectFolderButton.BackgroundImage = global::SLF.Properties.Resources.download_folder_file_icon_219533;
            this.SelectFolderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectFolderButton.ForeColor = System.Drawing.Color.Transparent;
            this.SelectFolderButton.Location = new System.Drawing.Point(399, 30);
            this.SelectFolderButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(56, 44);
            this.SelectFolderButton.TabIndex = 2;
            this.SelectFolderButton.UseVisualStyleBackColor = false;
            this.SelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // tab_dek
            // 
            this.tab_dek.Controls.Add(this.button8);
            this.tab_dek.Controls.Add(this.label16);
            this.tab_dek.Controls.Add(this.dataGridView4);
            this.tab_dek.Location = new System.Drawing.Point(4, 56);
            this.tab_dek.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_dek.Name = "tab_dek";
            this.tab_dek.Size = new System.Drawing.Size(1312, 628);
            this.tab_dek.TabIndex = 6;
            this.tab_dek.Text = "DEK Modülü";
            this.tab_dek.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(19, 44);
            this.button8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(159, 44);
            this.button8.TabIndex = 16;
            this.button8.Text = "Dosya Seç";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(364, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(126, 23);
            this.label16.TabIndex = 15;
            this.label16.Text = "Veri Ön İzleme:";
            // 
            // dataGridView4
            // 
            this.dataGridView4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Location = new System.Drawing.Point(234, 2);
            this.dataGridView4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.RowHeadersWidth = 51;
            this.dataGridView4.RowTemplate.Height = 24;
            this.dataGridView4.Size = new System.Drawing.Size(1070, 644);
            this.dataGridView4.TabIndex = 14;
            // 
            // tab_ea
            // 
            this.tab_ea.Controls.Add(this.label14);
            this.tab_ea.Controls.Add(this.mesafe_metre_ea);
            this.tab_ea.Controls.Add(this.Mesafe_ea);
            this.tab_ea.Controls.Add(this.Toolbox_EA);
            this.tab_ea.Controls.Add(this.buton_ea_harita_katmanlar);
            this.tab_ea.Controls.Add(this.ButtonKml);
            this.tab_ea.Controls.Add(this.oznitelikAc);
            this.tab_ea.Controls.Add(this.EA_list_box);
            this.tab_ea.Controls.Add(this.gMapControl_EA);
            this.tab_ea.Controls.Add(this.button7);
            this.tab_ea.Location = new System.Drawing.Point(4, 56);
            this.tab_ea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ea.Name = "tab_ea";
            this.tab_ea.Size = new System.Drawing.Size(1312, 628);
            this.tab_ea.TabIndex = 5;
            this.tab_ea.Text = "EA Şarj Modülü";
            this.tab_ea.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 244);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 23);
            this.label14.TabIndex = 35;
            this.label14.Text = "Katmanlar:";
            // 
            // mesafe_metre_ea
            // 
            this.mesafe_metre_ea.AutoSize = true;
            this.mesafe_metre_ea.Location = new System.Drawing.Point(467, 41);
            this.mesafe_metre_ea.Name = "mesafe_metre_ea";
            this.mesafe_metre_ea.Size = new System.Drawing.Size(0, 23);
            this.mesafe_metre_ea.TabIndex = 34;
            this.mesafe_metre_ea.Visible = false;
            // 
            // Mesafe_ea
            // 
            this.Mesafe_ea.AutoSize = true;
            this.Mesafe_ea.Location = new System.Drawing.Point(383, 41);
            this.Mesafe_ea.Name = "Mesafe_ea";
            this.Mesafe_ea.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_ea.TabIndex = 33;
            this.Mesafe_ea.Text = "Mesafe:";
            this.Mesafe_ea.Visible = false;
            // 
            // Toolbox_EA
            // 
            this.Toolbox_EA.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbox_EA.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EA_Seç,
            this.toolStripSeparator9,
            this.EA_Kaydır,
            this.toolStripSeparator10,
            this.EA_Mesafe_Ölç,
            this.toolStripSeparator11,
            this.EA_Poligon,
            this.toolStripSeparator13,
            this.EA_Nokta,
            this.toolStripSeparator14,
            this.toolStripButton15,
            this.toolStripSeparator15,
            this.toolStripButton16,
            this.toolStripSeparator16});
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
            this.EA_Seç.Size = new System.Drawing.Size(73, 29);
            this.EA_Seç.Tag = "";
            this.EA_Seç.Text = "Seç";
            this.EA_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.EA_Seç.Click += new System.EventHandler(this.EA_Seç_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 32);
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
            this.EA_Kaydır.Click += new System.EventHandler(this.EA_Kaydır_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 32);
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
            this.EA_Mesafe_Ölç.Click += new System.EventHandler(this.EA_Mesafe_Ölç_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 32);
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
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 32);
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
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripButton15
            // 
            this.toolStripButton15.AccessibleDescription = "";
            this.toolStripButton15.AccessibleName = "";
            this.toolStripButton15.BackColor = System.Drawing.Color.Beige;
            this.toolStripButton15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStripButton15.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStripButton15.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton15.Image")));
            this.toolStripButton15.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripButton15.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton15.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStripButton15.Name = "toolStripButton15";
            this.toolStripButton15.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStripButton15.Size = new System.Drawing.Size(142, 29);
            this.toolStripButton15.Tag = "";
            this.toolStripButton15.Text = "Grid Oluştur";
            this.toolStripButton15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton15.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripButton15.ToolTipText = "Belirli bir alan seçilip bu alanda mxn şeklinde bir grid (ızgara) tanımlar.";
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripButton16
            // 
            this.toolStripButton16.AccessibleDescription = "";
            this.toolStripButton16.AccessibleName = "";
            this.toolStripButton16.BackColor = System.Drawing.Color.LightBlue;
            this.toolStripButton16.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStripButton16.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.toolStripButton16.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton16.Image")));
            this.toolStripButton16.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripButton16.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton16.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStripButton16.Name = "toolStripButton16";
            this.toolStripButton16.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.toolStripButton16.Size = new System.Drawing.Size(146, 29);
            this.toolStripButton16.Tag = "";
            this.toolStripButton16.Text = "Fonksiyonlar";
            this.toolStripButton16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton16.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripButton16.ToolTipText = "Çeşitli vektörel veya tabular algoritmaları içerir.";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 32);
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
            this.EA_list_box.ItemHeight = 21;
            this.EA_list_box.Location = new System.Drawing.Point(17, 271);
            this.EA_list_box.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EA_list_box.Name = "EA_list_box";
            this.EA_list_box.Size = new System.Drawing.Size(244, 109);
            this.EA_list_box.TabIndex = 20;
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
            this.gMapControl_EA.Location = new System.Drawing.Point(335, 34);
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
            this.gMapControl_EA.Size = new System.Drawing.Size(969, 586);
            this.gMapControl_EA.TabIndex = 18;
            this.gMapControl_EA.Zoom = 0D;
            this.gMapControl_EA.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_EA_OnMarkerClick);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(8, 41);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(143, 39);
            this.button7.TabIndex = 17;
            this.button7.Text = "CSV Yükle";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // tab_ekonometrik
            // 
            this.tab_ekonometrik.AutoScroll = true;
            this.tab_ekonometrik.Controls.Add(this.ELFTablePanel);
            this.tab_ekonometrik.Controls.Add(this.ELFGraphicsPanel);
            this.tab_ekonometrik.Location = new System.Drawing.Point(4, 56);
            this.tab_ekonometrik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ekonometrik.Name = "tab_ekonometrik";
            this.tab_ekonometrik.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_ekonometrik.Size = new System.Drawing.Size(1312, 628);
            this.tab_ekonometrik.TabIndex = 1;
            this.tab_ekonometrik.Text = "Ekonometrik Talep Tahmini Modülü";
            this.tab_ekonometrik.UseVisualStyleBackColor = true;
            // 
            // ELFTablePanel
            // 
            this.ELFTablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFTablePanel.Controls.Add(this.ELFResultsTabControls);
            this.ELFTablePanel.Location = new System.Drawing.Point(577, 2);
            this.ELFTablePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ELFTablePanel.Name = "ELFTablePanel";
            this.ELFTablePanel.Size = new System.Drawing.Size(729, 645);
            this.ELFTablePanel.TabIndex = 21;
            // 
            // ELFResultsTabControls
            // 
            this.ELFResultsTabControls.Controls.Add(this.ELFMinResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFLowResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFBaseResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFHighResultsTabPage);
            this.ELFResultsTabControls.Controls.Add(this.ELFMaxResultsTabPage);
            this.ELFResultsTabControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFResultsTabControls.Location = new System.Drawing.Point(0, 0);
            this.ELFResultsTabControls.Margin = new System.Windows.Forms.Padding(4);
            this.ELFResultsTabControls.Name = "ELFResultsTabControls";
            this.ELFResultsTabControls.SelectedIndex = 0;
            this.ELFResultsTabControls.Size = new System.Drawing.Size(729, 645);
            this.ELFResultsTabControls.TabIndex = 3;
            // 
            // ELFMinResultsTabPage
            // 
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinResultsTable);
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinSenaryoGraphPicBox);
            this.ELFMinResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFMinResultsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMinResultsTabPage.Name = "ELFMinResultsTabPage";
            this.ELFMinResultsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ELFMinResultsTabPage.Size = new System.Drawing.Size(721, 611);
            this.ELFMinResultsTabPage.TabIndex = 0;
            this.ELFMinResultsTabPage.Text = "Minimum Sonuçlar";
            this.ELFMinResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFMinResultsTable
            // 
            this.ELFMinResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMinResultsTable.Location = new System.Drawing.Point(8, 7);
            this.ELFMinResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMinResultsTable.Name = "ELFMinResultsTable";
            this.ELFMinResultsTable.RowHeadersWidth = 51;
            this.ELFMinResultsTable.Size = new System.Drawing.Size(728, 558);
            this.ELFMinResultsTable.TabIndex = 0;
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
            // ELFLowResultsTabPage
            // 
            this.ELFLowResultsTabPage.Controls.Add(this.ELFLowResultsTable);
            this.ELFLowResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFLowResultsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ELFLowResultsTabPage.Name = "ELFLowResultsTabPage";
            this.ELFLowResultsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ELFLowResultsTabPage.Size = new System.Drawing.Size(721, 611);
            this.ELFLowResultsTabPage.TabIndex = 1;
            this.ELFLowResultsTabPage.Text = "Düşük Sonuçlar";
            this.ELFLowResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFLowResultsTable
            // 
            this.ELFLowResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowResultsTable.Location = new System.Drawing.Point(4, 0);
            this.ELFLowResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFLowResultsTable.Name = "ELFLowResultsTable";
            this.ELFLowResultsTable.RowHeadersWidth = 51;
            this.ELFLowResultsTable.Size = new System.Drawing.Size(677, 572);
            this.ELFLowResultsTable.TabIndex = 1;
            // 
            // ELFBaseResultsTabPage
            // 
            this.ELFBaseResultsTabPage.Controls.Add(this.ELFBaseResultsTable);
            this.ELFBaseResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFBaseResultsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ELFBaseResultsTabPage.Name = "ELFBaseResultsTabPage";
            this.ELFBaseResultsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ELFBaseResultsTabPage.Size = new System.Drawing.Size(721, 611);
            this.ELFBaseResultsTabPage.TabIndex = 2;
            this.ELFBaseResultsTabPage.Text = "Baz Sonuçlar";
            this.ELFBaseResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFBaseResultsTable
            // 
            this.ELFBaseResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseResultsTable.Location = new System.Drawing.Point(8, 4);
            this.ELFBaseResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFBaseResultsTable.Name = "ELFBaseResultsTable";
            this.ELFBaseResultsTable.RowHeadersWidth = 51;
            this.ELFBaseResultsTable.Size = new System.Drawing.Size(891, 567);
            this.ELFBaseResultsTable.TabIndex = 1;
            // 
            // ELFHighResultsTabPage
            // 
            this.ELFHighResultsTabPage.Controls.Add(this.ELFHighResultsTable);
            this.ELFHighResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFHighResultsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ELFHighResultsTabPage.Name = "ELFHighResultsTabPage";
            this.ELFHighResultsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ELFHighResultsTabPage.Size = new System.Drawing.Size(721, 611);
            this.ELFHighResultsTabPage.TabIndex = 3;
            this.ELFHighResultsTabPage.Text = "Yüksek Sonuçlar";
            this.ELFHighResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFHighResultsTable
            // 
            this.ELFHighResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighResultsTable.Location = new System.Drawing.Point(111, 0);
            this.ELFHighResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFHighResultsTable.Name = "ELFHighResultsTable";
            this.ELFHighResultsTable.RowHeadersWidth = 51;
            this.ELFHighResultsTable.Size = new System.Drawing.Size(765, 567);
            this.ELFHighResultsTable.TabIndex = 1;
            // 
            // ELFMaxResultsTabPage
            // 
            this.ELFMaxResultsTabPage.Controls.Add(this.ELFMaxResultsTable);
            this.ELFMaxResultsTabPage.Location = new System.Drawing.Point(4, 30);
            this.ELFMaxResultsTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMaxResultsTabPage.Name = "ELFMaxResultsTabPage";
            this.ELFMaxResultsTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ELFMaxResultsTabPage.Size = new System.Drawing.Size(721, 611);
            this.ELFMaxResultsTabPage.TabIndex = 4;
            this.ELFMaxResultsTabPage.Text = "Maksimum Sonuçlar";
            this.ELFMaxResultsTabPage.UseVisualStyleBackColor = true;
            // 
            // ELFMaxResultsTable
            // 
            this.ELFMaxResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxResultsTable.Location = new System.Drawing.Point(45, 4);
            this.ELFMaxResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMaxResultsTable.Name = "ELFMaxResultsTable";
            this.ELFMaxResultsTable.RowHeadersWidth = 51;
            this.ELFMaxResultsTable.Size = new System.Drawing.Size(920, 567);
            this.ELFMaxResultsTable.TabIndex = 1;
            // 
            // ELFGraphicsPanel
            // 
            this.ELFGraphicsPanel.AutoScroll = true;
            this.ELFGraphicsPanel.BackColor = System.Drawing.Color.Transparent;
            this.ELFGraphicsPanel.Controls.Add(this.label10);
            this.ELFGraphicsPanel.Controls.Add(this.button1);
            this.ELFGraphicsPanel.Controls.Add(this.ELFRadioButtonsPanel);
            this.ELFGraphicsPanel.Controls.Add(this.button2);
            this.ELFGraphicsPanel.Controls.Add(this.ELFPredictionButton);
            this.ELFGraphicsPanel.Controls.Add(this.SenaryoSelectionButton);
            this.ELFGraphicsPanel.Controls.Add(this.label12);
            this.ELFGraphicsPanel.Controls.Add(this.pictureBox1);
            this.ELFGraphicsPanel.Controls.Add(this.label9);
            this.ELFGraphicsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ELFGraphicsPanel.Location = new System.Drawing.Point(3, 2);
            this.ELFGraphicsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.ELFGraphicsPanel.Name = "ELFGraphicsPanel";
            this.ELFGraphicsPanel.Size = new System.Drawing.Size(567, 624);
            this.ELFGraphicsPanel.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(268, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(129, 23);
            this.label10.TabIndex = 30;
            this.label10.Text = "Senaryo Seçimi:";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.ForeColor = System.Drawing.Color.Transparent;
            this.button1.Location = new System.Drawing.Point(425, 71);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 44);
            this.button1.TabIndex = 29;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // ELFRadioButtonsPanel
            // 
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFLowSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFMaxSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFMinSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFHighSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFBaseSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Location = new System.Drawing.Point(8, 7);
            this.ELFRadioButtonsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.ELFRadioButtonsPanel.Name = "ELFRadioButtonsPanel";
            this.ELFRadioButtonsPanel.Size = new System.Drawing.Size(233, 207);
            this.ELFRadioButtonsPanel.TabIndex = 28;
            // 
            // ELFLowSenaryoRadioButton
            // 
            this.ELFLowSenaryoRadioButton.AutoSize = true;
            this.ELFLowSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFLowSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFLowSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFLowSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFLowSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFLowSenaryoRadioButton.Location = new System.Drawing.Point(4, 49);
            this.ELFLowSenaryoRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFLowSenaryoRadioButton.Name = "ELFLowSenaryoRadioButton";
            this.ELFLowSenaryoRadioButton.Size = new System.Drawing.Size(147, 27);
            this.ELFLowSenaryoRadioButton.TabIndex = 24;
            this.ELFLowSenaryoRadioButton.Text = "Düşük Senaryo";
            this.ELFLowSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFLowSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFLowSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFLowSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            // 
            // ELFMaxSenaryoRadioButton
            // 
            this.ELFMaxSenaryoRadioButton.AutoSize = true;
            this.ELFMaxSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMaxSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFMaxSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMaxSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFMaxSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFMaxSenaryoRadioButton.Location = new System.Drawing.Point(4, 156);
            this.ELFMaxSenaryoRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMaxSenaryoRadioButton.Name = "ELFMaxSenaryoRadioButton";
            this.ELFMaxSenaryoRadioButton.Size = new System.Drawing.Size(184, 27);
            this.ELFMaxSenaryoRadioButton.TabIndex = 27;
            this.ELFMaxSenaryoRadioButton.Text = "Maksimum Senaryo";
            this.ELFMaxSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFMaxSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFMaxSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFMaxSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            // 
            // ELFMinSenaryoRadioButton
            // 
            this.ELFMinSenaryoRadioButton.AutoSize = true;
            this.ELFMinSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMinSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFMinSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMinSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFMinSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFMinSenaryoRadioButton.Location = new System.Drawing.Point(4, 10);
            this.ELFMinSenaryoRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMinSenaryoRadioButton.Name = "ELFMinSenaryoRadioButton";
            this.ELFMinSenaryoRadioButton.Size = new System.Drawing.Size(173, 27);
            this.ELFMinSenaryoRadioButton.TabIndex = 23;
            this.ELFMinSenaryoRadioButton.Text = "Minimum Senaryo";
            this.ELFMinSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFMinSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFMinSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFMinSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            // 
            // ELFHighSenaryoRadioButton
            // 
            this.ELFHighSenaryoRadioButton.AutoSize = true;
            this.ELFHighSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFHighSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFHighSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFHighSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFHighSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFHighSenaryoRadioButton.Location = new System.Drawing.Point(4, 116);
            this.ELFHighSenaryoRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFHighSenaryoRadioButton.Name = "ELFHighSenaryoRadioButton";
            this.ELFHighSenaryoRadioButton.Size = new System.Drawing.Size(152, 27);
            this.ELFHighSenaryoRadioButton.TabIndex = 26;
            this.ELFHighSenaryoRadioButton.Text = "Yüksek Senaryo";
            this.ELFHighSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFHighSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFHighSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFHighSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            // 
            // ELFBaseSenaryoRadioButton
            // 
            this.ELFBaseSenaryoRadioButton.AutoSize = true;
            this.ELFBaseSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFBaseSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFBaseSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFBaseSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFBaseSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFBaseSenaryoRadioButton.Location = new System.Drawing.Point(4, 82);
            this.ELFBaseSenaryoRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFBaseSenaryoRadioButton.Name = "ELFBaseSenaryoRadioButton";
            this.ELFBaseSenaryoRadioButton.Size = new System.Drawing.Size(126, 27);
            this.ELFBaseSenaryoRadioButton.TabIndex = 25;
            this.ELFBaseSenaryoRadioButton.Text = "Baz Senaryo";
            this.ELFBaseSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFBaseSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFBaseSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFBaseSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(272, 164);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 70);
            this.button2.TabIndex = 19;
            this.button2.Text = "Tüm Sonuçları Görüntüle";
            this.button2.UseVisualStyleBackColor = true;
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
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(435, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(126, 23);
            this.label12.TabIndex = 7;
            this.label12.Text = "Veri Ön İzleme:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 302);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(305, 210);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 23);
            this.label9.TabIndex = 8;
            this.label9.Text = "Grafikler:";
            // 
            // tab_imar
            // 
            this.tab_imar.Controls.Add(this.Mesafe_imar);
            this.tab_imar.Controls.Add(this.mesafe_metre_imar);
            this.tab_imar.Controls.Add(this.checkBox1);
            this.tab_imar.Controls.Add(this.checkBox2);
            this.tab_imar.Controls.Add(this.checkBox3);
            this.tab_imar.Controls.Add(this.checkBox4);
            this.tab_imar.Controls.Add(this.checkBox5);
            this.tab_imar.Controls.Add(this.checkBox6);
            this.tab_imar.Controls.Add(this.checkBox7);
            this.tab_imar.Controls.Add(this.checkBox8);
            this.tab_imar.Controls.Add(this.checkBox22);
            this.tab_imar.Controls.Add(this.checkBox23);
            this.tab_imar.Controls.Add(this.checkBox24);
            this.tab_imar.Controls.Add(this.checkBox25);
            this.tab_imar.Controls.Add(this.checkBox26);
            this.tab_imar.Controls.Add(this.label4);
            this.tab_imar.Controls.Add(this.imar_dosya_seçimi);
            this.tab_imar.Controls.Add(this.toolStrip3);
            this.tab_imar.Controls.Add(this.buton_imar_katmanlar);
            this.tab_imar.Controls.Add(this.webView_imar);
            this.tab_imar.Controls.Add(this.gMapControl_imar);
            this.tab_imar.Location = new System.Drawing.Point(4, 56);
            this.tab_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_imar.Name = "tab_imar";
            this.tab_imar.Size = new System.Drawing.Size(1312, 628);
            this.tab_imar.TabIndex = 4;
            this.tab_imar.Text = "İmar Analizleri";
            this.tab_imar.UseVisualStyleBackColor = true;
            // 
            // Mesafe_imar
            // 
            this.Mesafe_imar.AutoSize = true;
            this.Mesafe_imar.Location = new System.Drawing.Point(260, 43);
            this.Mesafe_imar.Name = "Mesafe_imar";
            this.Mesafe_imar.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_imar.TabIndex = 57;
            this.Mesafe_imar.Text = "Mesafe:";
            this.Mesafe_imar.Visible = false;
            // 
            // mesafe_metre_imar
            // 
            this.mesafe_metre_imar.AutoSize = true;
            this.mesafe_metre_imar.Location = new System.Drawing.Point(359, 41);
            this.mesafe_metre_imar.Name = "mesafe_metre_imar";
            this.mesafe_metre_imar.Size = new System.Drawing.Size(0, 23);
            this.mesafe_metre_imar.TabIndex = 56;
            this.mesafe_metre_imar.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox1.Location = new System.Drawing.Point(6, 166);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(112, 27);
            this.checkBox1.TabIndex = 55;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
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
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox2.Location = new System.Drawing.Point(6, 197);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(114, 27);
            this.checkBox2.TabIndex = 54;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox3.Location = new System.Drawing.Point(6, 228);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(114, 27);
            this.checkBox3.TabIndex = 53;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Visible = false;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox4.Location = new System.Drawing.Point(6, 259);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(115, 27);
            this.checkBox4.TabIndex = 52;
            this.checkBox4.Text = "checkBox4";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.Visible = false;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox5.Location = new System.Drawing.Point(6, 290);
            this.checkBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(114, 27);
            this.checkBox5.TabIndex = 51;
            this.checkBox5.Text = "checkBox5";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.Visible = false;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox6.Location = new System.Drawing.Point(6, 321);
            this.checkBox6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(114, 27);
            this.checkBox6.TabIndex = 50;
            this.checkBox6.Text = "checkBox6";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.Visible = false;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox7.Location = new System.Drawing.Point(6, 352);
            this.checkBox7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(114, 27);
            this.checkBox7.TabIndex = 49;
            this.checkBox7.Text = "checkBox7";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.Visible = false;
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox8.Location = new System.Drawing.Point(6, 383);
            this.checkBox8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(114, 27);
            this.checkBox8.TabIndex = 48;
            this.checkBox8.Text = "checkBox8";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.Visible = false;
            // 
            // checkBox22
            // 
            this.checkBox22.AutoSize = true;
            this.checkBox22.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox22.Location = new System.Drawing.Point(6, 414);
            this.checkBox22.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox22.Name = "checkBox22";
            this.checkBox22.Size = new System.Drawing.Size(123, 27);
            this.checkBox22.TabIndex = 47;
            this.checkBox22.Text = "checkBox22";
            this.checkBox22.UseVisualStyleBackColor = true;
            this.checkBox22.Visible = false;
            // 
            // checkBox23
            // 
            this.checkBox23.AutoSize = true;
            this.checkBox23.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox23.Location = new System.Drawing.Point(6, 445);
            this.checkBox23.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox23.Name = "checkBox23";
            this.checkBox23.Size = new System.Drawing.Size(123, 27);
            this.checkBox23.TabIndex = 46;
            this.checkBox23.Text = "checkBox23";
            this.checkBox23.UseVisualStyleBackColor = true;
            this.checkBox23.Visible = false;
            // 
            // checkBox24
            // 
            this.checkBox24.AutoSize = true;
            this.checkBox24.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox24.Location = new System.Drawing.Point(6, 476);
            this.checkBox24.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox24.Name = "checkBox24";
            this.checkBox24.Size = new System.Drawing.Size(124, 27);
            this.checkBox24.TabIndex = 45;
            this.checkBox24.Text = "checkBox24";
            this.checkBox24.UseVisualStyleBackColor = true;
            this.checkBox24.Visible = false;
            // 
            // checkBox25
            // 
            this.checkBox25.AutoSize = true;
            this.checkBox25.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox25.Location = new System.Drawing.Point(6, 507);
            this.checkBox25.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox25.Name = "checkBox25";
            this.checkBox25.Size = new System.Drawing.Size(123, 27);
            this.checkBox25.TabIndex = 44;
            this.checkBox25.Text = "checkBox25";
            this.checkBox25.UseVisualStyleBackColor = true;
            this.checkBox25.Visible = false;
            // 
            // checkBox26
            // 
            this.checkBox26.AutoSize = true;
            this.checkBox26.BackColor = System.Drawing.Color.Transparent;
            this.checkBox26.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox26.Location = new System.Drawing.Point(6, 538);
            this.checkBox26.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox26.Name = "checkBox26";
            this.checkBox26.Size = new System.Drawing.Size(123, 27);
            this.checkBox26.TabIndex = 43;
            this.checkBox26.Text = "checkBox26";
            this.checkBox26.UseVisualStyleBackColor = false;
            this.checkBox26.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(21, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 24);
            this.label4.TabIndex = 42;
            this.label4.Text = "Katmanlar";
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
            // toolStrip3
            // 
            this.toolStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip3.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.İmar_Seç,
            this.toolStripSeparator19,
            this.İmar_Kaydır,
            this.toolStripSeparator20,
            this.İmar_Mesafe_Ölç,
            this.toolStripSeparator21,
            this.İmar_Poligon,
            this.toolStripSeparator23,
            this.İmar_Nokta,
            this.toolStripSeparator24,
            this.İmar_Grid_Oluştur,
            this.toolStripSeparator25,
            this.İmar_Fonksiyonlar});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(1259, 32);
            this.toolStrip3.TabIndex = 40;
            this.toolStrip3.Text = "toolStrip1";
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
            this.İmar_Seç.Size = new System.Drawing.Size(73, 29);
            this.İmar_Seç.Tag = "";
            this.İmar_Seç.Text = "Seç";
            this.İmar_Seç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Seç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Seç.ToolTipText = "Harita üzerinde seçim yapar.";
            this.İmar_Seç.Click += new System.EventHandler(this.İmar_Seç_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator19.Size = new System.Drawing.Size(6, 32);
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
            this.İmar_Kaydır.Size = new System.Drawing.Size(95, 29);
            this.İmar_Kaydır.Tag = "";
            this.İmar_Kaydır.Text = "Kaydır";
            this.İmar_Kaydır.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Kaydır.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Kaydır.ToolTipText = "Harita üzerine basılı tutup farklı yönlerde hareketi sağlar.";
            this.İmar_Kaydır.Click += new System.EventHandler(this.İmar_Kaydır_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator20.Size = new System.Drawing.Size(6, 32);
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
            this.İmar_Mesafe_Ölç.Size = new System.Drawing.Size(134, 29);
            this.İmar_Mesafe_Ölç.Tag = "";
            this.İmar_Mesafe_Ölç.Text = "Mesafe Ölç";
            this.İmar_Mesafe_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.İmar_Mesafe_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.İmar_Mesafe_Ölç.ToolTipText = "Noktalar arası doğrusal uzaklığı hesaplar.";
            this.İmar_Mesafe_Ölç.Click += new System.EventHandler(this.İmar_Mesafe_Ölç_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator21.Size = new System.Drawing.Size(6, 32);
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
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator23.Size = new System.Drawing.Size(6, 32);
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
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator24.Size = new System.Drawing.Size(6, 32);
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
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator25.Size = new System.Drawing.Size(6, 32);
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
            // buton_imar_katmanlar
            // 
            this.buton_imar_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_imar_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_imar_katmanlar.BackgroundImage")));
            this.buton_imar_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_imar_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_imar_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_imar_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_imar_katmanlar.Location = new System.Drawing.Point(253, 574);
            this.buton_imar_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_imar_katmanlar.Name = "buton_imar_katmanlar";
            this.buton_imar_katmanlar.Size = new System.Drawing.Size(58, 52);
            this.buton_imar_katmanlar.TabIndex = 39;
            this.buton_imar_katmanlar.UseVisualStyleBackColor = true;
            // 
            // webView_imar
            // 
            this.webView_imar.AllowExternalDrop = true;
            this.webView_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView_imar.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_imar.CreationProperties = null;
            this.webView_imar.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_imar.Location = new System.Drawing.Point(253, 34);
            this.webView_imar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_imar.Name = "webView_imar";
            this.webView_imar.Size = new System.Drawing.Size(1051, 592);
            this.webView_imar.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_imar.TabIndex = 38;
            this.webView_imar.Visible = false;
            this.webView_imar.ZoomFactor = 1D;
            // 
            // gMapControl_imar
            // 
            this.gMapControl_imar.AllowDrop = true;
            this.gMapControl_imar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_imar.Bearing = 0F;
            this.gMapControl_imar.CanDragMap = true;
            this.gMapControl_imar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_imar.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_imar.GrayScaleMode = false;
            this.gMapControl_imar.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_imar.LevelsKeepInMemory = 5;
            this.gMapControl_imar.Location = new System.Drawing.Point(253, 34);
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
            this.gMapControl_imar.Size = new System.Drawing.Size(1051, 592);
            this.gMapControl_imar.TabIndex = 35;
            this.gMapControl_imar.Zoom = 0D;
            this.gMapControl_imar.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_imar_OnMapClick);
            this.gMapControl_imar.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_imar_OnMapDoubleClick);
            this.gMapControl_imar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseDown);
            this.gMapControl_imar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseMove);
            this.gMapControl_imar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_imar_MouseUp);
            // 
            // tab_optDTR
            // 
            this.tab_optDTR.Controls.Add(this.gMapControl_optimalDTR);
            this.tab_optDTR.Controls.Add(this.buton_optimalDTR_katmanlar);
            this.tab_optDTR.Controls.Add(this.webView_optimalDTR);
            this.tab_optDTR.Location = new System.Drawing.Point(4, 56);
            this.tab_optDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_optDTR.Name = "tab_optDTR";
            this.tab_optDTR.Size = new System.Drawing.Size(1312, 628);
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
            this.gMapControl_optimalDTR.Location = new System.Drawing.Point(151, 16);
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
            this.gMapControl_optimalDTR.Size = new System.Drawing.Size(1153, 630);
            this.gMapControl_optimalDTR.TabIndex = 42;
            this.gMapControl_optimalDTR.Zoom = 0D;
            // 
            // buton_optimalDTR_katmanlar
            // 
            this.buton_optimalDTR_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_optimalDTR_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_optimalDTR_katmanlar.BackgroundImage")));
            this.buton_optimalDTR_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_optimalDTR_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_optimalDTR_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_optimalDTR_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_optimalDTR_katmanlar.Location = new System.Drawing.Point(151, 594);
            this.buton_optimalDTR_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_optimalDTR_katmanlar.Name = "buton_optimalDTR_katmanlar";
            this.buton_optimalDTR_katmanlar.Size = new System.Drawing.Size(58, 52);
            this.buton_optimalDTR_katmanlar.TabIndex = 41;
            this.buton_optimalDTR_katmanlar.UseVisualStyleBackColor = true;
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
            this.webView_optimalDTR.Location = new System.Drawing.Point(151, 16);
            this.webView_optimalDTR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_optimalDTR.Name = "webView_optimalDTR";
            this.webView_optimalDTR.Size = new System.Drawing.Size(1153, 630);
            this.webView_optimalDTR.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_optimalDTR.TabIndex = 40;
            this.webView_optimalDTR.Visible = false;
            this.webView_optimalDTR.ZoomFactor = 1D;
            // 
            // tab_senaryo
            // 
            this.tab_senaryo.Controls.Add(this.SenaryoModulePanel);
            this.tab_senaryo.Location = new System.Drawing.Point(4, 56);
            this.tab_senaryo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_senaryo.Name = "tab_senaryo";
            this.tab_senaryo.Size = new System.Drawing.Size(1312, 628);
            this.tab_senaryo.TabIndex = 3;
            this.tab_senaryo.Text = "Senaryo Oluşturma Modülü";
            this.tab_senaryo.UseVisualStyleBackColor = true;
            // 
            // SenaryoModulePanel
            // 
            this.SenaryoModulePanel.Controls.Add(this.SenaryoModuleTabControl);
            this.SenaryoModulePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModulePanel.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModulePanel.Margin = new System.Windows.Forms.Padding(4);
            this.SenaryoModulePanel.Name = "SenaryoModulePanel";
            this.SenaryoModulePanel.Size = new System.Drawing.Size(1312, 628);
            this.SenaryoModulePanel.TabIndex = 0;
            // 
            // SenaryoModuleTabControl
            // 
            this.SenaryoModuleTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.SenaryoModuleTabControl.Controls.Add(this.EkonometrikSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.StokastikSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.EASarjSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.DEKSenaryoTabPage);
            this.SenaryoModuleTabControl.Controls.Add(this.tabPage8);
            this.SenaryoModuleTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModuleTabControl.ItemSize = new System.Drawing.Size(180, 40);
            this.SenaryoModuleTabControl.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModuleTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.SenaryoModuleTabControl.Name = "SenaryoModuleTabControl";
            this.SenaryoModuleTabControl.SelectedIndex = 0;
            this.SenaryoModuleTabControl.Size = new System.Drawing.Size(1312, 628);
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
            this.SenaryoModuleTabControl.TabButtonSize = new System.Drawing.Size(180, 40);
            this.SenaryoModuleTabControl.TabIndex = 0;
            this.SenaryoModuleTabControl.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            // 
            // EkonometrikSenaryoTabPage
            // 
            this.EkonometrikSenaryoTabPage.Controls.Add(this.EkonometrikSenaryoOutputsPanel);
            this.EkonometrikSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.EkonometrikSenaryoTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.EkonometrikSenaryoTabPage.Name = "EkonometrikSenaryoTabPage";
            this.EkonometrikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.EkonometrikSenaryoTabPage.Size = new System.Drawing.Size(1124, 620);
            this.EkonometrikSenaryoTabPage.TabIndex = 0;
            this.EkonometrikSenaryoTabPage.Text = "Ekonometrik Senaryolar";
            this.EkonometrikSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // EkonometrikSenaryoOutputsPanel
            // 
            this.EkonometrikSenaryoOutputsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.ELFSenaryoTabControls);
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.EkonometrikSenaryoElementsPanel);
            this.EkonometrikSenaryoOutputsPanel.Location = new System.Drawing.Point(4, 4);
            this.EkonometrikSenaryoOutputsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.EkonometrikSenaryoOutputsPanel.Name = "EkonometrikSenaryoOutputsPanel";
            this.EkonometrikSenaryoOutputsPanel.Size = new System.Drawing.Size(1054, 611);
            this.EkonometrikSenaryoOutputsPanel.TabIndex = 0;
            // 
            // ELFSenaryoTabControls
            // 
            this.ELFSenaryoTabControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage2);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage3);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage4);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage5);
            this.ELFSenaryoTabControls.Controls.Add(this.tabPage6);
            this.ELFSenaryoTabControls.Location = new System.Drawing.Point(4, 4);
            this.ELFSenaryoTabControls.Margin = new System.Windows.Forms.Padding(4);
            this.ELFSenaryoTabControls.Name = "ELFSenaryoTabControls";
            this.ELFSenaryoTabControls.SelectedIndex = 0;
            this.ELFSenaryoTabControls.Size = new System.Drawing.Size(776, 616);
            this.ELFSenaryoTabControls.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ELFMinSenaryoTable);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(768, 582);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Minimum Senaryo";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ELFMinSenaryoTable
            // 
            this.ELFMinSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMinSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinSenaryoTable.Location = new System.Drawing.Point(4, 4);
            this.ELFMinSenaryoTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMinSenaryoTable.Name = "ELFMinSenaryoTable";
            this.ELFMinSenaryoTable.RowHeadersWidth = 51;
            this.ELFMinSenaryoTable.Size = new System.Drawing.Size(760, 574);
            this.ELFMinSenaryoTable.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ELFLowSenaryoTable);
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage3.Size = new System.Drawing.Size(768, 582);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Düşük Senaryo";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ELFLowSenaryoTable
            // 
            this.ELFLowSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowSenaryoTable.Location = new System.Drawing.Point(4, 4);
            this.ELFLowSenaryoTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFLowSenaryoTable.Name = "ELFLowSenaryoTable";
            this.ELFLowSenaryoTable.RowHeadersWidth = 51;
            this.ELFLowSenaryoTable.Size = new System.Drawing.Size(760, 574);
            this.ELFLowSenaryoTable.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.ELFBaseSenaryoTable);
            this.tabPage4.Location = new System.Drawing.Point(4, 30);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage4.Size = new System.Drawing.Size(768, 582);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Baz Senaryo";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // ELFBaseSenaryoTable
            // 
            this.ELFBaseSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseSenaryoTable.Location = new System.Drawing.Point(4, 4);
            this.ELFBaseSenaryoTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFBaseSenaryoTable.Name = "ELFBaseSenaryoTable";
            this.ELFBaseSenaryoTable.RowHeadersWidth = 51;
            this.ELFBaseSenaryoTable.Size = new System.Drawing.Size(760, 574);
            this.ELFBaseSenaryoTable.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ELFHighSenaryoTable);
            this.tabPage5.Location = new System.Drawing.Point(4, 30);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage5.Size = new System.Drawing.Size(768, 582);
            this.tabPage5.TabIndex = 3;
            this.tabPage5.Text = "Yüksek Senaryo";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ELFHighSenaryoTable
            // 
            this.ELFHighSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighSenaryoTable.Location = new System.Drawing.Point(4, 4);
            this.ELFHighSenaryoTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFHighSenaryoTable.Name = "ELFHighSenaryoTable";
            this.ELFHighSenaryoTable.RowHeadersWidth = 51;
            this.ELFHighSenaryoTable.Size = new System.Drawing.Size(760, 574);
            this.ELFHighSenaryoTable.TabIndex = 1;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.ELFMaxSenaryoTable);
            this.tabPage6.Location = new System.Drawing.Point(4, 30);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage6.Size = new System.Drawing.Size(768, 582);
            this.tabPage6.TabIndex = 4;
            this.tabPage6.Text = "Maksimum Senaryo";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // ELFMaxSenaryoTable
            // 
            this.ELFMaxSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMaxSenaryoTable.Location = new System.Drawing.Point(4, 4);
            this.ELFMaxSenaryoTable.Margin = new System.Windows.Forms.Padding(4);
            this.ELFMaxSenaryoTable.Name = "ELFMaxSenaryoTable";
            this.ELFMaxSenaryoTable.RowHeadersWidth = 51;
            this.ELFMaxSenaryoTable.Size = new System.Drawing.Size(760, 574);
            this.ELFMaxSenaryoTable.TabIndex = 1;
            // 
            // EkonometrikSenaryoElementsPanel
            // 
            this.EkonometrikSenaryoElementsPanel.BackColor = System.Drawing.Color.Snow;
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.richTextBox1);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFPredictionShowResultsGunaButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFScenerioSaveGunaButton);
            this.EkonometrikSenaryoElementsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EkonometrikSenaryoElementsPanel.Location = new System.Drawing.Point(778, 0);
            this.EkonometrikSenaryoElementsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.EkonometrikSenaryoElementsPanel.Name = "EkonometrikSenaryoElementsPanel";
            this.EkonometrikSenaryoElementsPanel.Size = new System.Drawing.Size(276, 611);
            this.EkonometrikSenaryoElementsPanel.TabIndex = 1;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Snow;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.richTextBox1.Location = new System.Drawing.Point(9, 39);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(271, 122);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "Tablolar üzerinde değişiklik yaparak senaryo üretebilirsiniz. Yeni senaryo tahmin" +
    " sonuçlarını görüntülemek için lütfen önce değişiklikleri kaydedin.";
            // 
            // ELFPredictionShowResultsGunaButton
            // 
            this.ELFPredictionShowResultsGunaButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.ELFPredictionShowResultsGunaButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.ELFPredictionShowResultsGunaButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.ELFPredictionShowResultsGunaButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.ELFPredictionShowResultsGunaButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ELFPredictionShowResultsGunaButton.ForeColor = System.Drawing.Color.White;
            this.ELFPredictionShowResultsGunaButton.Location = new System.Drawing.Point(0, 287);
            this.ELFPredictionShowResultsGunaButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFPredictionShowResultsGunaButton.Name = "ELFPredictionShowResultsGunaButton";
            this.ELFPredictionShowResultsGunaButton.Size = new System.Drawing.Size(272, 55);
            this.ELFPredictionShowResultsGunaButton.TabIndex = 9;
            this.ELFPredictionShowResultsGunaButton.Text = "Tahmin Sonuçlarını Göster";
            this.ELFPredictionShowResultsGunaButton.Click += new System.EventHandler(this.ELFPredictionShowResultsGunaButton_Click);
            // 
            // ELFScenerioSaveGunaButton
            // 
            this.ELFScenerioSaveGunaButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.ELFScenerioSaveGunaButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.ELFScenerioSaveGunaButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.ELFScenerioSaveGunaButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.ELFScenerioSaveGunaButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ELFScenerioSaveGunaButton.ForeColor = System.Drawing.Color.White;
            this.ELFScenerioSaveGunaButton.Location = new System.Drawing.Point(0, 209);
            this.ELFScenerioSaveGunaButton.Margin = new System.Windows.Forms.Padding(4);
            this.ELFScenerioSaveGunaButton.Name = "ELFScenerioSaveGunaButton";
            this.ELFScenerioSaveGunaButton.Size = new System.Drawing.Size(275, 55);
            this.ELFScenerioSaveGunaButton.TabIndex = 6;
            this.ELFScenerioSaveGunaButton.Text = "Senaryo Değişikliklerini Kaydet";
            this.ELFScenerioSaveGunaButton.Click += new System.EventHandler(this.ELFScenerioSaveGunaButton_Click);
            // 
            // StokastikSenaryoTabPage
            // 
            this.StokastikSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.StokastikSenaryoTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.StokastikSenaryoTabPage.Name = "StokastikSenaryoTabPage";
            this.StokastikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.StokastikSenaryoTabPage.Size = new System.Drawing.Size(1124, 620);
            this.StokastikSenaryoTabPage.TabIndex = 1;
            this.StokastikSenaryoTabPage.Text = "Stokastik Senaryolar";
            this.StokastikSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // EASarjSenaryoTabPage
            // 
            this.EASarjSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.EASarjSenaryoTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.EASarjSenaryoTabPage.Name = "EASarjSenaryoTabPage";
            this.EASarjSenaryoTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.EASarjSenaryoTabPage.Size = new System.Drawing.Size(1124, 620);
            this.EASarjSenaryoTabPage.TabIndex = 2;
            this.EASarjSenaryoTabPage.Text = "EA Şarj Senaryoları";
            this.EASarjSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // DEKSenaryoTabPage
            // 
            this.DEKSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.DEKSenaryoTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.DEKSenaryoTabPage.Name = "DEKSenaryoTabPage";
            this.DEKSenaryoTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.DEKSenaryoTabPage.Size = new System.Drawing.Size(1124, 620);
            this.DEKSenaryoTabPage.TabIndex = 3;
            this.DEKSenaryoTabPage.Text = "DEK Senaryoları";
            this.DEKSenaryoTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage8
            // 
            this.tabPage8.Location = new System.Drawing.Point(184, 4);
            this.tabPage8.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage8.Size = new System.Drawing.Size(1124, 620);
            this.tabPage8.TabIndex = 4;
            this.tabPage8.Text = "Yeni Genişleme Alanları ";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // tab_stokastik
            // 
            this.tab_stokastik.Controls.Add(this.webView_stokastik);
            this.tab_stokastik.Controls.Add(this.checkBox21);
            this.tab_stokastik.Controls.Add(this.checkBox20);
            this.tab_stokastik.Controls.Add(this.checkBox19);
            this.tab_stokastik.Controls.Add(this.mesafe_metre_stokastik);
            this.tab_stokastik.Controls.Add(this.Mesafe_stokastik);
            this.tab_stokastik.Controls.Add(this.gMapControl_stokastik);
            this.tab_stokastik.Controls.Add(this.buton_stokastik_harita_katmanlar);
            this.tab_stokastik.Controls.Add(this.checkBox18);
            this.tab_stokastik.Controls.Add(this.checkBox17);
            this.tab_stokastik.Controls.Add(this.checkBox16);
            this.tab_stokastik.Controls.Add(this.checkBox15);
            this.tab_stokastik.Controls.Add(this.checkBox14);
            this.tab_stokastik.Controls.Add(this.checkBox13);
            this.tab_stokastik.Controls.Add(this.checkBox12);
            this.tab_stokastik.Controls.Add(this.checkBox11);
            this.tab_stokastik.Controls.Add(this.checkBox10);
            this.tab_stokastik.Controls.Add(this.checkBox9);
            this.tab_stokastik.Controls.Add(this.label13);
            this.tab_stokastik.Controls.Add(this.stokastik_dosya_seçimi);
            this.tab_stokastik.Controls.Add(this.Toolbox_Stokastik);
            this.tab_stokastik.Location = new System.Drawing.Point(4, 56);
            this.tab_stokastik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_stokastik.Name = "tab_stokastik";
            this.tab_stokastik.Size = new System.Drawing.Size(1312, 628);
            this.tab_stokastik.TabIndex = 2;
            this.tab_stokastik.Text = "Stokastik Yük Tahmini Modülü";
            this.tab_stokastik.UseVisualStyleBackColor = true;
            // 
            // webView_stokastik
            // 
            this.webView_stokastik.AllowExternalDrop = true;
            this.webView_stokastik.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView_stokastik.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_stokastik.CreationProperties = null;
            this.webView_stokastik.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_stokastik.Location = new System.Drawing.Point(307, 43);
            this.webView_stokastik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_stokastik.Name = "webView_stokastik";
            this.webView_stokastik.Size = new System.Drawing.Size(994, 577);
            this.webView_stokastik.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_stokastik.TabIndex = 37;
            this.webView_stokastik.Visible = false;
            this.webView_stokastik.ZoomFactor = 1D;
            // 
            // checkBox21
            // 
            this.checkBox21.AutoSize = true;
            this.checkBox21.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox21.Location = new System.Drawing.Point(8, 578);
            this.checkBox21.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox21.Name = "checkBox21";
            this.checkBox21.Size = new System.Drawing.Size(121, 27);
            this.checkBox21.TabIndex = 35;
            this.checkBox21.Text = "checkBox21";
            this.checkBox21.UseVisualStyleBackColor = true;
            this.checkBox21.Visible = false;
            // 
            // checkBox20
            // 
            this.checkBox20.AutoSize = true;
            this.checkBox20.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox20.Location = new System.Drawing.Point(8, 544);
            this.checkBox20.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox20.Name = "checkBox20";
            this.checkBox20.Size = new System.Drawing.Size(123, 27);
            this.checkBox20.TabIndex = 34;
            this.checkBox20.Text = "checkBox20";
            this.checkBox20.UseVisualStyleBackColor = true;
            this.checkBox20.Visible = false;
            // 
            // checkBox19
            // 
            this.checkBox19.AutoSize = true;
            this.checkBox19.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox19.Location = new System.Drawing.Point(9, 510);
            this.checkBox19.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox19.Name = "checkBox19";
            this.checkBox19.Size = new System.Drawing.Size(121, 27);
            this.checkBox19.TabIndex = 33;
            this.checkBox19.Text = "checkBox19";
            this.checkBox19.UseVisualStyleBackColor = true;
            this.checkBox19.Visible = false;
            // 
            // mesafe_metre_stokastik
            // 
            this.mesafe_metre_stokastik.AutoSize = true;
            this.mesafe_metre_stokastik.Location = new System.Drawing.Point(408, 57);
            this.mesafe_metre_stokastik.Name = "mesafe_metre_stokastik";
            this.mesafe_metre_stokastik.Size = new System.Drawing.Size(0, 23);
            this.mesafe_metre_stokastik.TabIndex = 32;
            this.mesafe_metre_stokastik.Visible = false;
            // 
            // Mesafe_stokastik
            // 
            this.Mesafe_stokastik.AutoSize = true;
            this.Mesafe_stokastik.Location = new System.Drawing.Point(315, 57);
            this.Mesafe_stokastik.Name = "Mesafe_stokastik";
            this.Mesafe_stokastik.Size = new System.Drawing.Size(70, 23);
            this.Mesafe_stokastik.TabIndex = 31;
            this.Mesafe_stokastik.Text = "Mesafe:";
            this.Mesafe_stokastik.Visible = false;
            // 
            // gMapControl_stokastik
            // 
            this.gMapControl_stokastik.AllowDrop = true;
            this.gMapControl_stokastik.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl_stokastik.Bearing = 0F;
            this.gMapControl_stokastik.CanDragMap = true;
            this.gMapControl_stokastik.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.gMapControl_stokastik.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl_stokastik.GrayScaleMode = false;
            this.gMapControl_stokastik.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl_stokastik.LevelsKeepInMemory = 5;
            this.gMapControl_stokastik.Location = new System.Drawing.Point(307, 43);
            this.gMapControl_stokastik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
            this.gMapControl_stokastik.Size = new System.Drawing.Size(994, 577);
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
            this.buton_stokastik_harita_katmanlar.Location = new System.Drawing.Point(307, 568);
            this.buton_stokastik_harita_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_stokastik_harita_katmanlar.Name = "buton_stokastik_harita_katmanlar";
            this.buton_stokastik_harita_katmanlar.Size = new System.Drawing.Size(58, 52);
            this.buton_stokastik_harita_katmanlar.TabIndex = 29;
            this.buton_stokastik_harita_katmanlar.UseVisualStyleBackColor = true;
            this.buton_stokastik_harita_katmanlar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buton_stokastik_harita_katmanlar_MouseClick);
            // 
            // checkBox18
            // 
            this.checkBox18.AutoSize = true;
            this.checkBox18.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox18.Location = new System.Drawing.Point(8, 476);
            this.checkBox18.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox18.Name = "checkBox18";
            this.checkBox18.Size = new System.Drawing.Size(121, 27);
            this.checkBox18.TabIndex = 28;
            this.checkBox18.Text = "checkBox18";
            this.checkBox18.UseVisualStyleBackColor = true;
            this.checkBox18.Visible = false;
            // 
            // checkBox17
            // 
            this.checkBox17.AutoSize = true;
            this.checkBox17.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox17.Location = new System.Drawing.Point(8, 442);
            this.checkBox17.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox17.Name = "checkBox17";
            this.checkBox17.Size = new System.Drawing.Size(121, 27);
            this.checkBox17.TabIndex = 27;
            this.checkBox17.Text = "checkBox17";
            this.checkBox17.UseVisualStyleBackColor = true;
            this.checkBox17.Visible = false;
            // 
            // checkBox16
            // 
            this.checkBox16.AutoSize = true;
            this.checkBox16.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox16.Location = new System.Drawing.Point(8, 409);
            this.checkBox16.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox16.Name = "checkBox16";
            this.checkBox16.Size = new System.Drawing.Size(121, 27);
            this.checkBox16.TabIndex = 26;
            this.checkBox16.Text = "checkBox16";
            this.checkBox16.UseVisualStyleBackColor = true;
            this.checkBox16.Visible = false;
            // 
            // checkBox15
            // 
            this.checkBox15.AutoSize = true;
            this.checkBox15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox15.Location = new System.Drawing.Point(8, 374);
            this.checkBox15.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new System.Drawing.Size(121, 27);
            this.checkBox15.TabIndex = 25;
            this.checkBox15.Text = "checkBox15";
            this.checkBox15.UseVisualStyleBackColor = true;
            this.checkBox15.Visible = false;
            // 
            // checkBox14
            // 
            this.checkBox14.AutoSize = true;
            this.checkBox14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox14.Location = new System.Drawing.Point(8, 340);
            this.checkBox14.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(122, 27);
            this.checkBox14.TabIndex = 24;
            this.checkBox14.Text = "checkBox14";
            this.checkBox14.UseVisualStyleBackColor = true;
            this.checkBox14.Visible = false;
            // 
            // checkBox13
            // 
            this.checkBox13.AutoSize = true;
            this.checkBox13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox13.Location = new System.Drawing.Point(8, 306);
            this.checkBox13.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(121, 27);
            this.checkBox13.TabIndex = 23;
            this.checkBox13.Text = "checkBox13";
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox13.Visible = false;
            // 
            // checkBox12
            // 
            this.checkBox12.AutoSize = true;
            this.checkBox12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox12.Location = new System.Drawing.Point(8, 272);
            this.checkBox12.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(121, 27);
            this.checkBox12.TabIndex = 22;
            this.checkBox12.Text = "checkBox12";
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox12.Visible = false;
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox11.Location = new System.Drawing.Point(8, 238);
            this.checkBox11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(119, 27);
            this.checkBox11.TabIndex = 21;
            this.checkBox11.Text = "checkBox11";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.Visible = false;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox10.Location = new System.Drawing.Point(8, 204);
            this.checkBox10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(121, 27);
            this.checkBox10.TabIndex = 20;
            this.checkBox10.Text = "checkBox10";
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox10.Visible = false;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.BackColor = System.Drawing.Color.Transparent;
            this.checkBox9.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox9.Location = new System.Drawing.Point(8, 170);
            this.checkBox9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(114, 27);
            this.checkBox9.TabIndex = 19;
            this.checkBox9.Text = "checkBox9";
            this.checkBox9.UseVisualStyleBackColor = false;
            this.checkBox9.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(24, 129);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 24);
            this.label13.TabIndex = 18;
            this.label13.Text = "Katmanlar";
            // 
            // stokastik_dosya_seçimi
            // 
            this.stokastik_dosya_seçimi.Location = new System.Drawing.Point(9, 66);
            this.stokastik_dosya_seçimi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.stokastik_dosya_seçimi.Name = "stokastik_dosya_seçimi";
            this.stokastik_dosya_seçimi.Size = new System.Drawing.Size(173, 44);
            this.stokastik_dosya_seçimi.TabIndex = 17;
            this.stokastik_dosya_seçimi.Text = "Dosya Seç";
            this.stokastik_dosya_seçimi.UseVisualStyleBackColor = true;
            this.stokastik_dosya_seçimi.Click += new System.EventHandler(this.stokastik_dosya_seçimi_Click);
            // 
            // Toolbox_Stokastik
            // 
            this.Toolbox_Stokastik.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Toolbox_Stokastik.Dock = System.Windows.Forms.DockStyle.None;
            this.Toolbox_Stokastik.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbox_Stokastik.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Stokastik_Seç,
            this.toolStripSeparator1,
            this.Stokastik_Kaydır,
            this.toolStripSeparator2,
            this.Stokastik_Mesafe_Ölç,
            this.toolStripSeparator3,
            this.Stokastik_Poligon,
            this.toolStripSeparator7,
            this.Stokastik_Nokta,
            this.toolStripSeparator5,
            this.Stokastik_Grid_Oluştur,
            this.toolStripSeparator6,
            this.Stokastik_Fonksiyonlar});
            this.Toolbox_Stokastik.Location = new System.Drawing.Point(5, 9);
            this.Toolbox_Stokastik.Name = "Toolbox_Stokastik";
            this.Toolbox_Stokastik.Size = new System.Drawing.Size(1259, 32);
            this.Toolbox_Stokastik.TabIndex = 1;
            this.Toolbox_Stokastik.Text = "toolStrip1";
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
            this.Stokastik_Seç.Margin = new System.Windows.Forms.Padding(300, 1, 0, 2);
            this.Stokastik_Seç.Name = "Stokastik_Seç";
            this.Stokastik_Seç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.Stokastik_Seç.Size = new System.Drawing.Size(73, 29);
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
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Kaydır.Size = new System.Drawing.Size(95, 29);
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
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Mesafe_Ölç.Size = new System.Drawing.Size(134, 29);
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
            this.toolStripSeparator3.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Poligon.Size = new System.Drawing.Size(106, 29);
            this.Stokastik_Poligon.Tag = "";
            this.Stokastik_Poligon.Text = "Poligon";
            this.Stokastik_Poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Stokastik_Poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.Stokastik_Poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            this.Stokastik_Poligon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Stokastik_Poligon_MouseDown);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Nokta.Size = new System.Drawing.Size(94, 29);
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
            this.toolStripSeparator5.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Grid_Oluştur.Size = new System.Drawing.Size(142, 29);
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
            this.toolStripSeparator6.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 32);
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
            this.Stokastik_Fonksiyonlar.Size = new System.Drawing.Size(146, 29);
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
            this.tab_yükHaritası.Controls.Add(this.checkBox28);
            this.tab_yükHaritası.Controls.Add(this.checkBox27);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_deger);
            this.tab_yükHaritası.Controls.Add(this.yuk_yıl_text);
            this.tab_yükHaritası.Controls.Add(this.trackBar_Yıllar);
            this.tab_yükHaritası.Controls.Add(this.Mesafe_yuk);
            this.tab_yükHaritası.Controls.Add(this.mesafe_metre_yuk);
            this.tab_yükHaritası.Controls.Add(this.Toolbox_Yuk);
            this.tab_yükHaritası.Controls.Add(this.buton_yuk_haritası_katmanlar);
            this.tab_yükHaritası.Controls.Add(this.gMapControl_yuk);
            this.tab_yükHaritası.Controls.Add(this.webView_yuk);
            this.tab_yükHaritası.Location = new System.Drawing.Point(4, 56);
            this.tab_yükHaritası.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_yükHaritası.Name = "tab_yükHaritası";
            this.tab_yükHaritası.Size = new System.Drawing.Size(1312, 628);
            this.tab_yükHaritası.TabIndex = 9;
            this.tab_yükHaritası.Text = "Yük Haritası Modülü";
            this.tab_yükHaritası.UseVisualStyleBackColor = true;
            // 
            // checkBox28
            // 
            this.checkBox28.AutoSize = true;
            this.checkBox28.Location = new System.Drawing.Point(12, 171);
            this.checkBox28.Name = "checkBox28";
            this.checkBox28.Size = new System.Drawing.Size(210, 27);
            this.checkBox28.TabIndex = 43;
            this.checkBox28.Text = "Yük Yoğunluğu Haritası";
            this.checkBox28.UseVisualStyleBackColor = true;
            // 
            // checkBox27
            // 
            this.checkBox27.AutoSize = true;
            this.checkBox27.Location = new System.Drawing.Point(12, 125);
            this.checkBox27.Name = "checkBox27";
            this.checkBox27.Size = new System.Drawing.Size(226, 27);
            this.checkBox27.TabIndex = 42;
            this.checkBox27.Text = "Yerleşik Alan Yoğunlukları";
            this.checkBox27.UseVisualStyleBackColor = true;
            // 
            // yuk_yıl_deger
            // 
            this.yuk_yıl_deger.AutoSize = true;
            this.yuk_yıl_deger.Location = new System.Drawing.Point(35, 23);
            this.yuk_yıl_deger.Name = "yuk_yıl_deger";
            this.yuk_yıl_deger.Size = new System.Drawing.Size(47, 23);
            this.yuk_yıl_deger.TabIndex = 41;
            this.yuk_yıl_deger.Text = "2024";
            // 
            // yuk_yıl_text
            // 
            this.yuk_yıl_text.AutoSize = true;
            this.yuk_yıl_text.Location = new System.Drawing.Point(8, 23);
            this.yuk_yıl_text.Name = "yuk_yıl_text";
            this.yuk_yıl_text.Size = new System.Drawing.Size(32, 23);
            this.yuk_yıl_text.TabIndex = 40;
            this.yuk_yıl_text.Text = "Yıl:";
            // 
            // trackBar_Yıllar
            // 
            this.trackBar_Yıllar.Location = new System.Drawing.Point(8, 49);
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
            this.Mesafe_yuk.Location = new System.Drawing.Point(258, 36);
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
            // Toolbox_Yuk
            // 
            this.Toolbox_Yuk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Toolbox_Yuk.Dock = System.Windows.Forms.DockStyle.None;
            this.Toolbox_Yuk.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbox_Yuk.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Yuk_Seç,
            this.toolStripSeparator4,
            this.Yuk_Kaydır,
            this.toolStripSeparator8,
            this.Yuk_Mesafe_Ölç,
            this.toolStripSeparator12});
            this.Toolbox_Yuk.Location = new System.Drawing.Point(253, 2);
            this.Toolbox_Yuk.Name = "Toolbox_Yuk";
            this.Toolbox_Yuk.Size = new System.Drawing.Size(683, 32);
            this.Toolbox_Yuk.TabIndex = 36;
            this.Toolbox_Yuk.Text = "toolStrip1";
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 32);
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
            this.Yuk_Kaydır.Click += new System.EventHandler(this.Yuk_Kaydır_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 32);
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
            this.Yuk_Mesafe_Ölç.Click += new System.EventHandler(this.Yuk_Mesafe_Ölç_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 32);
            // 
            // buton_yuk_haritası_katmanlar
            // 
            this.buton_yuk_haritası_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_yuk_haritası_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_yuk_haritası_katmanlar.BackgroundImage")));
            this.buton_yuk_haritası_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_yuk_haritası_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_yuk_haritası_katmanlar.Cursor = System.Windows.Forms.Cursors.Default;
            this.buton_yuk_haritası_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_yuk_haritası_katmanlar.Location = new System.Drawing.Point(253, 568);
            this.buton_yuk_haritası_katmanlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buton_yuk_haritası_katmanlar.Name = "buton_yuk_haritası_katmanlar";
            this.buton_yuk_haritası_katmanlar.Size = new System.Drawing.Size(58, 52);
            this.buton_yuk_haritası_katmanlar.TabIndex = 35;
            this.buton_yuk_haritası_katmanlar.UseVisualStyleBackColor = true;
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
            this.gMapControl_yuk.Location = new System.Drawing.Point(253, 34);
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
            this.gMapControl_yuk.Size = new System.Drawing.Size(1051, 584);
            this.gMapControl_yuk.TabIndex = 34;
            this.gMapControl_yuk.Zoom = 0D;
            this.gMapControl_yuk.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_yuk_OnMapClick);
            this.gMapControl_yuk.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseDown);
            this.gMapControl_yuk.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseMove);
            this.gMapControl_yuk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gMapControl_yuk_MouseUp);
            // 
            // webView_yuk
            // 
            this.webView_yuk.AllowExternalDrop = true;
            this.webView_yuk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView_yuk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView_yuk.CreationProperties = null;
            this.webView_yuk.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView_yuk.Location = new System.Drawing.Point(253, 32);
            this.webView_yuk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView_yuk.Name = "webView_yuk";
            this.webView_yuk.Size = new System.Drawing.Size(1051, 586);
            this.webView_yuk.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView_yuk.TabIndex = 0;
            this.webView_yuk.Visible = false;
            this.webView_yuk.ZoomFactor = 1D;
            // 
            // tab_rapor
            // 
            this.tab_rapor.Location = new System.Drawing.Point(4, 56);
            this.tab_rapor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_rapor.Name = "tab_rapor";
            this.tab_rapor.Size = new System.Drawing.Size(1312, 628);
            this.tab_rapor.TabIndex = 8;
            this.tab_rapor.Text = "Raporlama";
            this.tab_rapor.UseVisualStyleBackColor = true;
            // 
            // tab_validasyon
            // 
            this.tab_validasyon.Location = new System.Drawing.Point(4, 56);
            this.tab_validasyon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tab_validasyon.Name = "tab_validasyon";
            this.tab_validasyon.Size = new System.Drawing.Size(1312, 628);
            this.tab_validasyon.TabIndex = 10;
            this.tab_validasyon.Text = "Validasyon Modülü";
            this.tab_validasyon.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 62);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1471, 618);
            this.tabPage1.TabIndex = 10;
            this.tabPage1.Text = "Validasyon Modülü";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // HomePageButton
            // 
            this.HomePageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HomePageButton.BackColor = System.Drawing.Color.White;
            this.HomePageButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.HomePageButton.Location = new System.Drawing.Point(1656, 2);
            this.HomePageButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.HomePageButton.Name = "HomePageButton";
            this.HomePageButton.Size = new System.Drawing.Size(105, 37);
            this.HomePageButton.TabIndex = 4;
            this.HomePageButton.Text = "Ana Sayfa";
            this.HomePageButton.UseVisualStyleBackColor = true;
            this.HomePageButton.Click += new System.EventHandler(this.HomePageButton_Click);
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
            // ContextMenuStrip_Poligon
            // 
            this.ContextMenuStrip_Poligon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip_Poligon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Poligon_Çiz,
            this.Poligon_Sil,
            this.Poligon_Kaydet});
            this.ContextMenuStrip_Poligon.Name = "ContextMenuStrip_Poligon";
            this.ContextMenuStrip_Poligon.Size = new System.Drawing.Size(183, 82);
            // 
            // Poligon_Çiz
            // 
            this.Poligon_Çiz.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Çiz.Image")));
            this.Poligon_Çiz.Name = "Poligon_Çiz";
            this.Poligon_Çiz.Size = new System.Drawing.Size(182, 26);
            this.Poligon_Çiz.Text = "Poligon Çiz";
            this.Poligon_Çiz.Click += new System.EventHandler(this.Poligon_Çiz_Click);
            // 
            // Poligon_Sil
            // 
            this.Poligon_Sil.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Sil.Image")));
            this.Poligon_Sil.Name = "Poligon_Sil";
            this.Poligon_Sil.Size = new System.Drawing.Size(182, 26);
            this.Poligon_Sil.Text = "Poligon Sil";
            this.Poligon_Sil.Click += new System.EventHandler(this.Poligon_Sil_Click);
            // 
            // Poligon_Kaydet
            // 
            this.Poligon_Kaydet.Image = ((System.Drawing.Image)(resources.GetObject("Poligon_Kaydet.Image")));
            this.Poligon_Kaydet.Name = "Poligon_Kaydet";
            this.Poligon_Kaydet.Size = new System.Drawing.Size(182, 26);
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
            this.ContextMenuStrip_Fonksiyon.Size = new System.Drawing.Size(188, 56);
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
            // ModuleTabPanel
            // 
            this.ModuleTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModuleTabPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.ModuleTabPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ModuleTabPanel.Controls.Add(this.Modül_Tabları);
            this.ModuleTabPanel.Location = new System.Drawing.Point(0, 66);
            this.ModuleTabPanel.Margin = new System.Windows.Forms.Padding(4);
            this.ModuleTabPanel.Name = "ModuleTabPanel";
            this.ModuleTabPanel.Size = new System.Drawing.Size(1320, 688);
            this.ModuleTabPanel.TabIndex = 5;
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HeaderPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.HeaderPanel.Controls.Add(this.HomePageButton);
            this.HeaderPanel.Location = new System.Drawing.Point(0, 30);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(4);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1776, 42);
            this.HeaderPanel.TabIndex = 3;
            // 
            // legendPanel
            // 
            this.legendPanel.AutoSize = true;
            this.legendPanel.Location = new System.Drawing.Point(15, 218);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(209, 362);
            this.legendPanel.TabIndex = 44;
            // 
            // ModülFormu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1320, 753);
            this.Controls.Add(this.HeaderPanel);
            this.Controls.Add(this.ModuleTabPanel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ModülFormu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modüller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModülFormu_FormClosing);
            this.Load += new System.EventHandler(this.ModülFormu_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.Modül_Tabları.ResumeLayout(false);
            this.tab_girdi.ResumeLayout(false);
            this.tab_girdi.PerformLayout();
            this.OpenModuleButtonPanel.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_girdi)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tab_dek.ResumeLayout(false);
            this.tab_dek.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.tab_ea.ResumeLayout(false);
            this.tab_ea.PerformLayout();
            this.Toolbox_EA.ResumeLayout(false);
            this.Toolbox_EA.PerformLayout();
            this.harita_katmanları_right_click.ResumeLayout(false);
            this.tab_ekonometrik.ResumeLayout(false);
            this.ELFTablePanel.ResumeLayout(false);
            this.ELFResultsTabControls.ResumeLayout(false);
            this.ELFMinResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinResultsTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoGraphPicBox)).EndInit();
            this.ELFLowResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowResultsTable)).EndInit();
            this.ELFBaseResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseResultsTable)).EndInit();
            this.ELFHighResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighResultsTable)).EndInit();
            this.ELFMaxResultsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxResultsTable)).EndInit();
            this.ELFGraphicsPanel.ResumeLayout(false);
            this.ELFGraphicsPanel.PerformLayout();
            this.ELFRadioButtonsPanel.ResumeLayout(false);
            this.ELFRadioButtonsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tab_imar.ResumeLayout(false);
            this.tab_imar.PerformLayout();
            this.katmanlar_right_click.ResumeLayout(false);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_imar)).EndInit();
            this.tab_optDTR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView_optimalDTR)).EndInit();
            this.tab_senaryo.ResumeLayout(false);
            this.SenaryoModulePanel.ResumeLayout(false);
            this.SenaryoModuleTabControl.ResumeLayout(false);
            this.EkonometrikSenaryoTabPage.ResumeLayout(false);
            this.EkonometrikSenaryoOutputsPanel.ResumeLayout(false);
            this.ELFSenaryoTabControls.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMinSenaryoTable)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFLowSenaryoTable)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFBaseSenaryoTable)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFHighSenaryoTable)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ELFMaxSenaryoTable)).EndInit();
            this.EkonometrikSenaryoElementsPanel.ResumeLayout(false);
            this.tab_stokastik.ResumeLayout(false);
            this.tab_stokastik.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_stokastik)).EndInit();
            this.Toolbox_Stokastik.ResumeLayout(false);
            this.Toolbox_Stokastik.PerformLayout();
            this.tab_yükHaritası.ResumeLayout(false);
            this.tab_yükHaritası.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Yıllar)).EndInit();
            this.Toolbox_Yuk.ResumeLayout(false);
            this.Toolbox_Yuk.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView_yuk)).EndInit();
            this.ContextMenuStrip_Nokta.ResumeLayout(false);
            this.ContextMenuStrip_Poligon.ResumeLayout(false);
            this.ContextMenuStrip_Fonksiyon.ResumeLayout(false);
            this.ModuleTabPanel.ResumeLayout(false);
            this.HeaderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnTamamla_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnplgn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yardımToolStripMenuItem;
        public System.Windows.Forms.TabControl Modül_Tabları;
        public System.Windows.Forms.TabPage tab_stokastik;
        private System.Windows.Forms.TabPage tab_girdi;
        private System.Windows.Forms.TabPage tab_ekonometrik;
        private System.Windows.Forms.TabPage tab_imar;
        private System.Windows.Forms.TabPage tab_dek;
        private System.Windows.Forms.TabPage tab_optDTR;
        private System.Windows.Forms.TabPage tab_rapor;
        private System.Windows.Forms.TabPage tab_yükHaritası;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox veri_listesi_seçimi;
        private System.Windows.Forms.DataGridView dataGridView_girdi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectFolderButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_yuk;
        private System.Windows.Forms.TabPage tab_validasyon;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel ELFTablePanel;
        private System.Windows.Forms.Button stokastik_dosya_seçimi;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox18;
        private System.Windows.Forms.CheckBox checkBox17;
        private System.Windows.Forms.CheckBox checkBox16;
        private System.Windows.Forms.CheckBox checkBox15;
        private System.Windows.Forms.CheckBox checkBox14;
        private System.Windows.Forms.CheckBox checkBox13;
        private System.Windows.Forms.CheckBox checkBox12;
        private System.Windows.Forms.CheckBox checkBox11;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Button HomePageButton;
        private System.Windows.Forms.ContextMenuStrip katmanlar_right_click;
        private System.Windows.Forms.ToolStripMenuItem tabloyuGörToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rengiDeğiştirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temizleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kaydetToolStripMenuItem;
        private System.Windows.Forms.Button buton_stokastik_harita_katmanlar;
        private System.Windows.Forms.TabPage tabPage1;
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
        private CheckBox checkBox21;
        private CheckBox checkBox20;
        private CheckBox checkBox19;
        private ToolStripMenuItem Google_Earth_Desktop;
        private ToolStrip Toolbox_Stokastik;
        private ToolStripButton Stokastik_Seç;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton Stokastik_Kaydır;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton Stokastik_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton Stokastik_Poligon;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton Stokastik_Nokta;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton Stokastik_Grid_Oluştur;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton Stokastik_Fonksiyonlar;
        private Button raporGoruntuleButonu;
        private Button ExcelDownloadButton;
        private ToolStripMenuItem yenidenAdlandırToolStripMenuItem;
        private Button csvExportButton;
        private ComboBox endYearComboBox;
        private ComboBox startYearComboBox;
        private Button yearApproveButton;
        private Panel ModuleTabPanel;
        private Panel HeaderPanel;
        private Panel panel6;
        private Panel panel9;
        private Panel panel8;
        private Panel panel7;
        private TabPage tab_senaryo;
        private Button SenaryoSelectionButton;
        private Label label17;
        private Label label18;
        private Label label3;
        private Panel ELFGraphicsPanel;
        private Button OpenModuleButton;
        private Panel OpenModuleButtonPanel;
        private Label label9;
        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2TabControl SenaryoModuleTabControl;
        private TabPage EkonometrikSenaryoTabPage;
        private TabPage StokastikSenaryoTabPage;
        private TabPage EASarjSenaryoTabPage;
        private Panel SenaryoModulePanel;
        private Panel EkonometrikSenaryoOutputsPanel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel EkonometrikSenaryoElementsPanel;
        private TabPage DEKSenaryoTabPage;
        private TabControl ELFSenaryoTabControls;
        private TabPage tabPage2;
        private DataGridView ELFMinSenaryoTable;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private TabPage tabPage6;
        private DataGridView ELFLowSenaryoTable;
        private DataGridView ELFBaseSenaryoTable;
        private DataGridView ELFHighSenaryoTable;
        private DataGridView ELFMaxSenaryoTable;
        private Guna.UI2.WinForms.Guna2Button ELFScenerioSaveGunaButton;
        private Guna.UI2.WinForms.Guna2Button ELFPredictionShowResultsGunaButton;
        private TabPage tabPage8;
        private Button ELFPredictionButton;
        private Button button2;
        private Guna.UI2.WinForms.Guna2RadioButton ELFMaxSenaryoRadioButton;
        private Guna.UI2.WinForms.Guna2RadioButton ELFHighSenaryoRadioButton;
        private Guna.UI2.WinForms.Guna2RadioButton ELFBaseSenaryoRadioButton;
        private Guna.UI2.WinForms.Guna2RadioButton ELFLowSenaryoRadioButton;
        private Guna.UI2.WinForms.Guna2RadioButton ELFMinSenaryoRadioButton;
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
        private Label label10;
        private Button button1;
        private RichTextBox richTextBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_stokastik;
        private ToolStripMenuItem Sokak_Görünümü;
        private Button buton_yuk_haritası_katmanlar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_yuk;
        public GMap.NET.WindowsForms.GMapControl gMapControl_imar;
        private Button buton_imar_katmanlar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_imar;
        public GMap.NET.WindowsForms.GMapControl gMapControl_optimalDTR;
        private Button buton_optimalDTR_katmanlar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView_optimalDTR;
        private ToolStrip toolStrip3;
        private ToolStripButton İmar_Seç;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripButton İmar_Kaydır;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripButton İmar_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripButton İmar_Poligon;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripButton İmar_Nokta;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripButton İmar_Grid_Oluştur;
        private ToolStripSeparator toolStripSeparator25;
        private ToolStripButton İmar_Fonksiyonlar;
        public TabPage tab_ea;
        private Button buton_ea_harita_katmanlar;
        private Button button8;
        private Label label16;
        private DataGridView dataGridView4;
        private Label label14;
        private Label mesafe_metre_ea;
        private Label Mesafe_ea;
        private ToolStrip Toolbox_EA;
        private ToolStripButton EA_Seç;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton EA_Kaydır;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripButton EA_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton EA_Poligon;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripButton EA_Nokta;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripButton toolStripButton15;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripButton toolStripButton16;
        private ToolStripSeparator toolStripSeparator16;
        private Button ButtonKml;
        private Button oznitelikAc;
        private ListBox EA_list_box;
        public GMap.NET.WindowsForms.GMapControl gMapControl_EA;
        private Button button7;
        private ToolStrip Toolbox_Yuk;
        private ToolStripButton Yuk_Seç;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton Yuk_Kaydır;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripButton Yuk_Mesafe_Ölç;
        private ToolStripSeparator toolStripSeparator12;
        private Label mesafe_metre_yuk;
        private Label Mesafe_yuk;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private CheckBox checkBox7;
        private CheckBox checkBox8;
        private CheckBox checkBox22;
        private CheckBox checkBox23;
        private CheckBox checkBox24;
        private CheckBox checkBox25;
        private CheckBox checkBox26;
        private Label label4;
        private Button imar_dosya_seçimi;
        private Label Mesafe_imar;
        private Label mesafe_metre_imar;
        private TrackBar trackBar_Yıllar;
        private Label yuk_yıl_deger;
        private Label yuk_yıl_text;
        private CheckBox checkBox28;
        private CheckBox checkBox27;
        public Panel legendPanel;
    }
}