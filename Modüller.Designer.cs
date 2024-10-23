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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
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
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.EA_Seç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Kaydır = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Mesafe_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.EA_Alan_Ölç = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
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
            this.button6 = new System.Windows.Forms.Button();
            this.tab_optDTR = new System.Windows.Forms.TabPage();
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
            this.checkBox21 = new System.Windows.Forms.CheckBox();
            this.katmanlar_right_click = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabloyuGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rengiDeğiştirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temizleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yenidenAdlandırToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaydetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.Seç_Stokastik = new System.Windows.Forms.ToolStrip();
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
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
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
            this.menuStrip1.SuspendLayout();
            this.Modül_Tabları.SuspendLayout();
            this.tab_girdi.SuspendLayout();
            this.OpenModuleButtonPanel.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tab_dek.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tab_ea.SuspendLayout();
            this.toolStrip2.SuspendLayout();
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
            this.katmanlar_right_click.SuspendLayout();
            this.Seç_Stokastik.SuspendLayout();
            this.tab_yükHaritası.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.panel1.SuspendLayout();
            this.ContextMenuStrip_Nokta.SuspendLayout();
            this.ContextMenuStrip_Poligon.SuspendLayout();
            this.ContextMenuStrip_Fonksiyon.SuspendLayout();
            this.ModuleTabPanel.SuspendLayout();
            this.HeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightSalmon;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.yardımToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1332, 24);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.fileToolStripMenuItem.Text = "Dosya";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.importToolStripMenuItem.Text = "İçeri Aktar";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exportToolStripMenuItem.Text = "Dışarı Aktar";
            // 
            // saToolStripMenuItem
            // 
            this.saToolStripMenuItem.Name = "saToolStripMenuItem";
            this.saToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.saToolStripMenuItem.Text = "Kaydet";
            // 
            // yardımToolStripMenuItem
            // 
            this.yardımToolStripMenuItem.Name = "yardımToolStripMenuItem";
            this.yardımToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
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
            this.Modül_Tabları.Margin = new System.Windows.Forms.Padding(2);
            this.Modül_Tabları.Multiline = true;
            this.Modül_Tabları.Name = "Modül_Tabları";
            this.Modül_Tabları.Padding = new System.Drawing.Point(20, 3);
            this.Modül_Tabları.SelectedIndex = 0;
            this.Modül_Tabları.Size = new System.Drawing.Size(1332, 559);
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
            this.tab_girdi.Controls.Add(this.dataGridView1);
            this.tab_girdi.Controls.Add(this.panel8);
            this.tab_girdi.Controls.Add(this.panel6);
            this.tab_girdi.Controls.Add(this.panel7);
            this.tab_girdi.Font = new System.Drawing.Font("Maiandra GD", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_girdi.Location = new System.Drawing.Point(4, 48);
            this.tab_girdi.Margin = new System.Windows.Forms.Padding(2);
            this.tab_girdi.Name = "tab_girdi";
            this.tab_girdi.Padding = new System.Windows.Forms.Padding(2);
            this.tab_girdi.Size = new System.Drawing.Size(1324, 507);
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
            this.label3.Location = new System.Drawing.Point(2, 78);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Veri Önizleme:";
            this.label3.UseWaitCursor = true;
            // 
            // OpenModuleButtonPanel
            // 
            this.OpenModuleButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenModuleButtonPanel.Controls.Add(this.OpenModuleButton);
            this.OpenModuleButtonPanel.Location = new System.Drawing.Point(1106, 462);
            this.OpenModuleButtonPanel.Name = "OpenModuleButtonPanel";
            this.OpenModuleButtonPanel.Size = new System.Drawing.Size(111, 36);
            this.OpenModuleButtonPanel.TabIndex = 17;
            this.OpenModuleButtonPanel.UseWaitCursor = true;
            // 
            // OpenModuleButton
            // 
            this.OpenModuleButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OpenModuleButton.Location = new System.Drawing.Point(0, 0);
            this.OpenModuleButton.Name = "OpenModuleButton";
            this.OpenModuleButton.Size = new System.Drawing.Size(111, 36);
            this.OpenModuleButton.TabIndex = 16;
            this.OpenModuleButton.Text = "Modüle Git";
            this.OpenModuleButton.UseVisualStyleBackColor = true;
            this.OpenModuleButton.UseWaitCursor = true;
            this.OpenModuleButton.Click += new System.EventHandler(this.OpenModuleButton_Click);
            // 
            // panel9
            // 
            this.panel9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel9.Controls.Add(this.label17);
            this.panel9.Controls.Add(this.raporGoruntuleButonu);
            this.panel9.Location = new System.Drawing.Point(1012, 3);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(183, 48);
            this.panel9.TabIndex = 15;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(2, 5);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(93, 17);
            this.label17.TabIndex = 7;
            this.label17.Text = "Rapor Oluştur:";
            this.label17.UseWaitCursor = true;
            // 
            // raporGoruntuleButonu
            // 
            this.raporGoruntuleButonu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.raporGoruntuleButonu.BackColor = System.Drawing.Color.White;
            this.raporGoruntuleButonu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("raporGoruntuleButonu.BackgroundImage")));
            this.raporGoruntuleButonu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.raporGoruntuleButonu.ForeColor = System.Drawing.Color.Transparent;
            this.raporGoruntuleButonu.Location = new System.Drawing.Point(116, 7);
            this.raporGoruntuleButonu.Margin = new System.Windows.Forms.Padding(2);
            this.raporGoruntuleButonu.Name = "raporGoruntuleButonu";
            this.raporGoruntuleButonu.Size = new System.Drawing.Size(42, 36);
            this.raporGoruntuleButonu.TabIndex = 6;
            this.raporGoruntuleButonu.UseVisualStyleBackColor = false;
            this.raporGoruntuleButonu.Click += new System.EventHandler(this.raporGoruntuleButonu_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(7, 96);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1210, 350);
            this.dataGridView1.TabIndex = 4;
            // 
            // panel8
            // 
            this.panel8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel8.Controls.Add(this.label18);
            this.panel8.Controls.Add(this.ExcelDownloadButton);
            this.panel8.Controls.Add(this.csvExportButton);
            this.panel8.Location = new System.Drawing.Point(799, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(210, 48);
            this.panel8.TabIndex = 14;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(2, 3);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 17);
            this.label18.TabIndex = 16;
            this.label18.Text = "Dışa Aktar:";
            this.label18.UseWaitCursor = true;
            // 
            // ExcelDownloadButton
            // 
            this.ExcelDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExcelDownloadButton.BackColor = System.Drawing.Color.White;
            this.ExcelDownloadButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ExcelDownloadButton.BackgroundImage")));
            this.ExcelDownloadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExcelDownloadButton.ForeColor = System.Drawing.Color.Transparent;
            this.ExcelDownloadButton.Location = new System.Drawing.Point(107, 0);
            this.ExcelDownloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.ExcelDownloadButton.Name = "ExcelDownloadButton";
            this.ExcelDownloadButton.Size = new System.Drawing.Size(42, 36);
            this.ExcelDownloadButton.TabIndex = 7;
            this.ExcelDownloadButton.UseVisualStyleBackColor = false;
            this.ExcelDownloadButton.UseWaitCursor = true;
            this.ExcelDownloadButton.Click += new System.EventHandler(this.ExcelDownloadButton_Click);
            // 
            // csvExportButton
            // 
            this.csvExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.csvExportButton.BackColor = System.Drawing.Color.White;
            this.csvExportButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("csvExportButton.BackgroundImage")));
            this.csvExportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.csvExportButton.ForeColor = System.Drawing.Color.Transparent;
            this.csvExportButton.Location = new System.Drawing.Point(153, 2);
            this.csvExportButton.Margin = new System.Windows.Forms.Padding(2);
            this.csvExportButton.Name = "csvExportButton";
            this.csvExportButton.Size = new System.Drawing.Size(42, 36);
            this.csvExportButton.TabIndex = 8;
            this.csvExportButton.UseVisualStyleBackColor = false;
            this.csvExportButton.UseWaitCursor = true;
            this.csvExportButton.Click += new System.EventHandler(this.csvExportButton_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.startYearComboBox);
            this.panel6.Controls.Add(this.yearApproveButton);
            this.panel6.Controls.Add(this.endYearComboBox);
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(227, 61);
            this.panel6.TabIndex = 12;
            // 
            // startYearComboBox
            // 
            this.startYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.startYearComboBox.FormattingEnabled = true;
            this.startYearComboBox.Location = new System.Drawing.Point(2, 5);
            this.startYearComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.startYearComboBox.Name = "startYearComboBox";
            this.startYearComboBox.Size = new System.Drawing.Size(92, 23);
            this.startYearComboBox.TabIndex = 9;
            this.startYearComboBox.Text = "Yıl seçiniz";
            this.startYearComboBox.SelectedIndexChanged += new System.EventHandler(this.startYearComboBox_SelectedIndexChanged);
            // 
            // yearApproveButton
            // 
            this.yearApproveButton.Location = new System.Drawing.Point(61, 33);
            this.yearApproveButton.Name = "yearApproveButton";
            this.yearApproveButton.Size = new System.Drawing.Size(75, 28);
            this.yearApproveButton.TabIndex = 11;
            this.yearApproveButton.Text = "Onayla";
            this.yearApproveButton.UseVisualStyleBackColor = true;
            this.yearApproveButton.Click += new System.EventHandler(this.yearApproveButton_Click);
            // 
            // endYearComboBox
            // 
            this.endYearComboBox.ForeColor = System.Drawing.Color.DarkOrange;
            this.endYearComboBox.FormattingEnabled = true;
            this.endYearComboBox.Location = new System.Drawing.Point(121, 5);
            this.endYearComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.endYearComboBox.Name = "endYearComboBox";
            this.endYearComboBox.Size = new System.Drawing.Size(92, 23);
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
            this.panel7.Location = new System.Drawing.Point(357, 2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(399, 64);
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
            this.veri_listesi_seçimi.Location = new System.Drawing.Point(15, 32);
            this.veri_listesi_seçimi.Margin = new System.Windows.Forms.Padding(2);
            this.veri_listesi_seçimi.Name = "veri_listesi_seçimi";
            this.veri_listesi_seçimi.Size = new System.Drawing.Size(161, 28);
            this.veri_listesi_seçimi.TabIndex = 0;
            this.veri_listesi_seçimi.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.veri_listesi_seçimi_DrawItem);
            this.veri_listesi_seçimi.SelectedIndexChanged += new System.EventHandler(this.veri_listesi_seçimi_SelectedIndexChanged);
            this.veri_listesi_seçimi.MouseDown += new System.Windows.Forms.MouseEventHandler(this.veri_listesi_seçimi_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dosya Veri Tipi Seçimi:";
            this.label1.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(207, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Dosya Seçimi:";
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.BackColor = System.Drawing.Color.White;
            this.SelectFolderButton.BackgroundImage = global::SLF.Properties.Resources.download_folder_file_icon_219533;
            this.SelectFolderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SelectFolderButton.ForeColor = System.Drawing.Color.Transparent;
            this.SelectFolderButton.Location = new System.Drawing.Point(299, 24);
            this.SelectFolderButton.Margin = new System.Windows.Forms.Padding(2);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(42, 36);
            this.SelectFolderButton.TabIndex = 2;
            this.SelectFolderButton.UseVisualStyleBackColor = false;
            this.SelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // tab_dek
            // 
            this.tab_dek.Controls.Add(this.button8);
            this.tab_dek.Controls.Add(this.label16);
            this.tab_dek.Controls.Add(this.dataGridView4);
            this.tab_dek.Location = new System.Drawing.Point(4, 48);
            this.tab_dek.Margin = new System.Windows.Forms.Padding(2);
            this.tab_dek.Name = "tab_dek";
            this.tab_dek.Size = new System.Drawing.Size(1324, 507);
            this.tab_dek.TabIndex = 6;
            this.tab_dek.Text = "DEK Modülü";
            this.tab_dek.UseVisualStyleBackColor = true;
            this.tab_dek.UseWaitCursor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(14, 36);
            this.button8.Margin = new System.Windows.Forms.Padding(2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(119, 36);
            this.button8.TabIndex = 16;
            this.button8.Text = "Dosya Seç";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.UseWaitCursor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(273, 14);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(98, 17);
            this.label16.TabIndex = 15;
            this.label16.Text = "Veri Ön İzleme:";
            this.label16.UseWaitCursor = true;
            // 
            // dataGridView4
            // 
            this.dataGridView4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Location = new System.Drawing.Point(276, 36);
            this.dataGridView4.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.RowHeadersWidth = 51;
            this.dataGridView4.RowTemplate.Height = 24;
            this.dataGridView4.Size = new System.Drawing.Size(752, 497);
            this.dataGridView4.TabIndex = 14;
            this.dataGridView4.UseWaitCursor = true;
            // 
            // tab_ea
            // 
            this.tab_ea.Controls.Add(this.label14);
            this.tab_ea.Controls.Add(this.mesafe_metre_ea);
            this.tab_ea.Controls.Add(this.Mesafe_ea);
            this.tab_ea.Controls.Add(this.toolStrip2);
            this.tab_ea.Controls.Add(this.buton_ea_harita_katmanlar);
            this.tab_ea.Controls.Add(this.ButtonKml);
            this.tab_ea.Controls.Add(this.oznitelikAc);
            this.tab_ea.Controls.Add(this.EA_list_box);
            this.tab_ea.Controls.Add(this.gMapControl_EA);
            this.tab_ea.Controls.Add(this.button7);
            this.tab_ea.Location = new System.Drawing.Point(4, 48);
            this.tab_ea.Margin = new System.Windows.Forms.Padding(2);
            this.tab_ea.Name = "tab_ea";
            this.tab_ea.Size = new System.Drawing.Size(1324, 507);
            this.tab_ea.TabIndex = 5;
            this.tab_ea.Text = "EA Şarj Modülü";
            this.tab_ea.UseVisualStyleBackColor = true;
            this.tab_ea.UseWaitCursor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 198);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 17);
            this.label14.TabIndex = 35;
            this.label14.Text = "Katmanlar:";
            this.label14.UseWaitCursor = true;
            // 
            // mesafe_metre_ea
            // 
            this.mesafe_metre_ea.AutoSize = true;
            this.mesafe_metre_ea.Location = new System.Drawing.Point(350, 33);
            this.mesafe_metre_ea.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_ea.Name = "mesafe_metre_ea";
            this.mesafe_metre_ea.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_ea.TabIndex = 34;
            this.mesafe_metre_ea.UseWaitCursor = true;
            this.mesafe_metre_ea.Visible = false;
            // 
            // Mesafe_ea
            // 
            this.Mesafe_ea.AutoSize = true;
            this.Mesafe_ea.Location = new System.Drawing.Point(287, 33);
            this.Mesafe_ea.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_ea.Name = "Mesafe_ea";
            this.Mesafe_ea.Size = new System.Drawing.Size(54, 17);
            this.Mesafe_ea.TabIndex = 33;
            this.Mesafe_ea.Text = "Mesafe:";
            this.Mesafe_ea.UseWaitCursor = true;
            this.Mesafe_ea.Visible = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EA_Seç,
            this.toolStripSeparator9,
            this.EA_Kaydır,
            this.toolStripSeparator10,
            this.EA_Mesafe_Ölç,
            this.toolStripSeparator11,
            this.EA_Alan_Ölç,
            this.toolStripSeparator12,
            this.EA_Poligon,
            this.toolStripSeparator13,
            this.EA_Nokta,
            this.toolStripSeparator14,
            this.toolStripButton15,
            this.toolStripSeparator15,
            this.toolStripButton16,
            this.toolStripSeparator16});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1324, 27);
            this.toolStrip2.TabIndex = 32;
            this.toolStrip2.Text = "toolStrip2";
            this.toolStrip2.UseWaitCursor = true;
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
            this.EA_Seç.Size = new System.Drawing.Size(66, 24);
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
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 27);
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
            this.EA_Kaydır.Size = new System.Drawing.Size(85, 24);
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
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 27);
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
            this.EA_Mesafe_Ölç.Size = new System.Drawing.Size(117, 24);
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
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 27);
            // 
            // EA_Alan_Ölç
            // 
            this.EA_Alan_Ölç.AccessibleDescription = "";
            this.EA_Alan_Ölç.AccessibleName = "";
            this.EA_Alan_Ölç.BackColor = System.Drawing.Color.Azure;
            this.EA_Alan_Ölç.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.EA_Alan_Ölç.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.EA_Alan_Ölç.Image = ((System.Drawing.Image)(resources.GetObject("EA_Alan_Ölç.Image")));
            this.EA_Alan_Ölç.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EA_Alan_Ölç.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EA_Alan_Ölç.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.EA_Alan_Ölç.Name = "EA_Alan_Ölç";
            this.EA_Alan_Ölç.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.EA_Alan_Ölç.Size = new System.Drawing.Size(99, 24);
            this.EA_Alan_Ölç.Tag = "";
            this.EA_Alan_Ölç.Text = "Alan Ölç";
            this.EA_Alan_Ölç.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Alan_Ölç.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Alan_Ölç.ToolTipText = "Çizilen bir poligonun alansal büyüklüğünü hesaplar.";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 27);
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
            this.EA_Poligon.Size = new System.Drawing.Size(93, 24);
            this.EA_Poligon.Tag = "";
            this.EA_Poligon.Text = "Poligon";
            this.EA_Poligon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EA_Poligon.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.EA_Poligon.ToolTipText = "Poligon çizme, silme veya kaydetme fonksiyonlarını yerine getirir.";
            this.EA_Poligon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EA_Poligon_MouseDown);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 27);
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
            this.EA_Nokta.Size = new System.Drawing.Size(83, 24);
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
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 27);
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
            this.toolStripButton15.Size = new System.Drawing.Size(122, 24);
            this.toolStripButton15.Tag = "";
            this.toolStripButton15.Text = "Grid Oluştur";
            this.toolStripButton15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton15.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripButton15.ToolTipText = "Belirli bir alan seçilip bu alanda mxn şeklinde bir grid (ızgara) tanımlar.";
            this.toolStripButton15.Click += new System.EventHandler(this.ea_Grid_Oluştur_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 27);
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
            this.toolStripButton16.Size = new System.Drawing.Size(125, 24);
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
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 27);
            // 
            // buton_ea_harita_katmanlar
            // 
            this.buton_ea_harita_katmanlar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buton_ea_harita_katmanlar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buton_ea_harita_katmanlar.BackgroundImage")));
            this.buton_ea_harita_katmanlar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buton_ea_harita_katmanlar.ContextMenuStrip = this.harita_katmanları_right_click;
            this.buton_ea_harita_katmanlar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buton_ea_harita_katmanlar.Location = new System.Drawing.Point(280, 531);
            this.buton_ea_harita_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_ea_harita_katmanlar.Name = "buton_ea_harita_katmanlar";
            this.buton_ea_harita_katmanlar.Size = new System.Drawing.Size(46, 42);
            this.buton_ea_harita_katmanlar.TabIndex = 31;
            this.buton_ea_harita_katmanlar.UseVisualStyleBackColor = true;
            this.buton_ea_harita_katmanlar.UseWaitCursor = true;
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
            this.Uydu});
            this.harita_katmanları_right_click.Name = "harita_katmanları_right_click";
            this.harita_katmanları_right_click.Size = new System.Drawing.Size(168, 160);
            // 
            // Arazi
            // 
            this.Arazi.Image = ((System.Drawing.Image)(resources.GetObject("Arazi.Image")));
            this.Arazi.Name = "Arazi";
            this.Arazi.Size = new System.Drawing.Size(167, 26);
            this.Arazi.Text = "Arazi";
            this.Arazi.Click += new System.EventHandler(this.Arazi_Click);
            // 
            // Google_Earth
            // 
            this.Google_Earth.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth.Image")));
            this.Google_Earth.Name = "Google_Earth";
            this.Google_Earth.Size = new System.Drawing.Size(167, 26);
            this.Google_Earth.Text = "GE Online";
            this.Google_Earth.Click += new System.EventHandler(this.Google_Earth_Click);
            // 
            // Google_Earth_Desktop
            // 
            this.Google_Earth_Desktop.Image = ((System.Drawing.Image)(resources.GetObject("Google_Earth_Desktop.Image")));
            this.Google_Earth_Desktop.Name = "Google_Earth_Desktop";
            this.Google_Earth_Desktop.Size = new System.Drawing.Size(167, 26);
            this.Google_Earth_Desktop.Text = "GE Pro Desktop";
            this.Google_Earth_Desktop.Click += new System.EventHandler(this.Google_Earth_Desktop_Click);
            // 
            // Harita
            // 
            this.Harita.Image = ((System.Drawing.Image)(resources.GetObject("Harita.Image")));
            this.Harita.Name = "Harita";
            this.Harita.Size = new System.Drawing.Size(167, 26);
            this.Harita.Text = "Harita";
            this.Harita.Click += new System.EventHandler(this.Harita_Click);
            // 
            // OSM
            // 
            this.OSM.Image = ((System.Drawing.Image)(resources.GetObject("OSM.Image")));
            this.OSM.Name = "OSM";
            this.OSM.Size = new System.Drawing.Size(167, 26);
            this.OSM.Text = "Open Street Map";
            this.OSM.Click += new System.EventHandler(this.OSM_Click);
            // 
            // Uydu
            // 
            this.Uydu.Image = ((System.Drawing.Image)(resources.GetObject("Uydu.Image")));
            this.Uydu.Name = "Uydu";
            this.Uydu.Size = new System.Drawing.Size(167, 26);
            this.Uydu.Text = "Uydu";
            this.Uydu.Click += new System.EventHandler(this.Uydu_Click);
            // 
            // ButtonKml
            // 
            this.ButtonKml.Location = new System.Drawing.Point(6, 102);
            this.ButtonKml.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonKml.Name = "ButtonKml";
            this.ButtonKml.Size = new System.Drawing.Size(107, 32);
            this.ButtonKml.TabIndex = 30;
            this.ButtonKml.Text = "KML Yükle";
            this.ButtonKml.UseVisualStyleBackColor = true;
            this.ButtonKml.UseWaitCursor = true;
            this.ButtonKml.Click += new System.EventHandler(this.ButtonKml_Click);
            // 
            // oznitelikAc
            // 
            this.oznitelikAc.Location = new System.Drawing.Point(13, 440);
            this.oznitelikAc.Margin = new System.Windows.Forms.Padding(2);
            this.oznitelikAc.Name = "oznitelikAc";
            this.oznitelikAc.Size = new System.Drawing.Size(146, 31);
            this.oznitelikAc.TabIndex = 29;
            this.oznitelikAc.Text = "Öznitelikleri Göster";
            this.oznitelikAc.UseVisualStyleBackColor = true;
            this.oznitelikAc.UseWaitCursor = true;
            // 
            // EA_list_box
            // 
            this.EA_list_box.FormattingEnabled = true;
            this.EA_list_box.ItemHeight = 17;
            this.EA_list_box.Location = new System.Drawing.Point(13, 220);
            this.EA_list_box.Margin = new System.Windows.Forms.Padding(2);
            this.EA_list_box.Name = "EA_list_box";
            this.EA_list_box.Size = new System.Drawing.Size(184, 89);
            this.EA_list_box.TabIndex = 20;
            this.EA_list_box.UseWaitCursor = true;
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
            this.gMapControl_EA.Location = new System.Drawing.Point(280, 33);
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
            this.gMapControl_EA.Size = new System.Drawing.Size(746, 539);
            this.gMapControl_EA.TabIndex = 18;
            this.gMapControl_EA.UseWaitCursor = true;
            this.gMapControl_EA.Zoom = 0D;
            this.gMapControl_EA.OnMapClick += new GMap.NET.WindowsForms.MapClick(this.gMapControl_EA_OnMapClick);
            this.gMapControl_EA.OnMapDoubleClick += new GMap.NET.WindowsForms.MapDoubleClick(this.gMapControl_EA_OnMapDoubleClick);
            this.gMapControl_EA.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl_EA_OnMarkerClick);
            this.gMapControl_EA.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl_EA_MouseDown);
            this.gMapControl_EA.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_EA_MouseMove);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(6, 33);
            this.button7.Margin = new System.Windows.Forms.Padding(2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(107, 32);
            this.button7.TabIndex = 17;
            this.button7.Text = "CSV Yükle";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.UseWaitCursor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tab_ekonometrik
            // 
            this.tab_ekonometrik.AutoScroll = true;
            this.tab_ekonometrik.Controls.Add(this.ELFTablePanel);
            this.tab_ekonometrik.Controls.Add(this.ELFGraphicsPanel);
            this.tab_ekonometrik.Location = new System.Drawing.Point(4, 48);
            this.tab_ekonometrik.Margin = new System.Windows.Forms.Padding(2);
            this.tab_ekonometrik.Name = "tab_ekonometrik";
            this.tab_ekonometrik.Padding = new System.Windows.Forms.Padding(2);
            this.tab_ekonometrik.Size = new System.Drawing.Size(1324, 507);
            this.tab_ekonometrik.TabIndex = 1;
            this.tab_ekonometrik.Text = "Ekonometrik Talep Tahmini Modülü";
            this.tab_ekonometrik.UseVisualStyleBackColor = true;
            this.tab_ekonometrik.UseWaitCursor = true;
            // 
            // ELFTablePanel
            // 
            this.ELFTablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ELFTablePanel.Controls.Add(this.ELFResultsTabControls);
            this.ELFTablePanel.Location = new System.Drawing.Point(433, 2);
            this.ELFTablePanel.Margin = new System.Windows.Forms.Padding(2);
            this.ELFTablePanel.Name = "ELFTablePanel";
            this.ELFTablePanel.Size = new System.Drawing.Size(889, 503);
            this.ELFTablePanel.TabIndex = 21;
            this.ELFTablePanel.UseWaitCursor = true;
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
            this.ELFResultsTabControls.Name = "ELFResultsTabControls";
            this.ELFResultsTabControls.SelectedIndex = 0;
            this.ELFResultsTabControls.Size = new System.Drawing.Size(889, 503);
            this.ELFResultsTabControls.TabIndex = 3;
            this.ELFResultsTabControls.UseWaitCursor = true;
            // 
            // ELFMinResultsTabPage
            // 
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinResultsTable);
            this.ELFMinResultsTabPage.Controls.Add(this.ELFMinSenaryoGraphPicBox);
            this.ELFMinResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFMinResultsTabPage.Name = "ELFMinResultsTabPage";
            this.ELFMinResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ELFMinResultsTabPage.Size = new System.Drawing.Size(881, 473);
            this.ELFMinResultsTabPage.TabIndex = 0;
            this.ELFMinResultsTabPage.Text = "Minimum Sonuçlar";
            this.ELFMinResultsTabPage.UseVisualStyleBackColor = true;
            this.ELFMinResultsTabPage.UseWaitCursor = true;
            // 
            // ELFMinResultsTable
            // 
            this.ELFMinResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMinResultsTable.Location = new System.Drawing.Point(6, 6);
            this.ELFMinResultsTable.Name = "ELFMinResultsTable";
            this.ELFMinResultsTable.Size = new System.Drawing.Size(546, 453);
            this.ELFMinResultsTable.TabIndex = 0;
            this.ELFMinResultsTable.UseWaitCursor = true;
            // 
            // ELFMinSenaryoGraphPicBox
            // 
            this.ELFMinSenaryoGraphPicBox.Location = new System.Drawing.Point(607, 47);
            this.ELFMinSenaryoGraphPicBox.Name = "ELFMinSenaryoGraphPicBox";
            this.ELFMinSenaryoGraphPicBox.Size = new System.Drawing.Size(231, 171);
            this.ELFMinSenaryoGraphPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ELFMinSenaryoGraphPicBox.TabIndex = 22;
            this.ELFMinSenaryoGraphPicBox.TabStop = false;
            this.ELFMinSenaryoGraphPicBox.UseWaitCursor = true;
            // 
            // ELFLowResultsTabPage
            // 
            this.ELFLowResultsTabPage.Controls.Add(this.ELFLowResultsTable);
            this.ELFLowResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFLowResultsTabPage.Name = "ELFLowResultsTabPage";
            this.ELFLowResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ELFLowResultsTabPage.Size = new System.Drawing.Size(881, 473);
            this.ELFLowResultsTabPage.TabIndex = 1;
            this.ELFLowResultsTabPage.Text = "Düşük Sonuçlar";
            this.ELFLowResultsTabPage.UseVisualStyleBackColor = true;
            this.ELFLowResultsTabPage.UseWaitCursor = true;
            // 
            // ELFLowResultsTable
            // 
            this.ELFLowResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowResultsTable.Location = new System.Drawing.Point(3, 0);
            this.ELFLowResultsTable.Name = "ELFLowResultsTable";
            this.ELFLowResultsTable.Size = new System.Drawing.Size(508, 465);
            this.ELFLowResultsTable.TabIndex = 1;
            this.ELFLowResultsTable.UseWaitCursor = true;
            // 
            // ELFBaseResultsTabPage
            // 
            this.ELFBaseResultsTabPage.Controls.Add(this.ELFBaseResultsTable);
            this.ELFBaseResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFBaseResultsTabPage.Name = "ELFBaseResultsTabPage";
            this.ELFBaseResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ELFBaseResultsTabPage.Size = new System.Drawing.Size(881, 473);
            this.ELFBaseResultsTabPage.TabIndex = 2;
            this.ELFBaseResultsTabPage.Text = "Baz Sonuçlar";
            this.ELFBaseResultsTabPage.UseVisualStyleBackColor = true;
            this.ELFBaseResultsTabPage.UseWaitCursor = true;
            // 
            // ELFBaseResultsTable
            // 
            this.ELFBaseResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseResultsTable.Location = new System.Drawing.Point(6, 3);
            this.ELFBaseResultsTable.Name = "ELFBaseResultsTable";
            this.ELFBaseResultsTable.Size = new System.Drawing.Size(668, 461);
            this.ELFBaseResultsTable.TabIndex = 1;
            this.ELFBaseResultsTable.UseWaitCursor = true;
            // 
            // ELFHighResultsTabPage
            // 
            this.ELFHighResultsTabPage.Controls.Add(this.ELFHighResultsTable);
            this.ELFHighResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFHighResultsTabPage.Name = "ELFHighResultsTabPage";
            this.ELFHighResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ELFHighResultsTabPage.Size = new System.Drawing.Size(881, 473);
            this.ELFHighResultsTabPage.TabIndex = 3;
            this.ELFHighResultsTabPage.Text = "Yüksek Sonuçlar";
            this.ELFHighResultsTabPage.UseVisualStyleBackColor = true;
            this.ELFHighResultsTabPage.UseWaitCursor = true;
            // 
            // ELFHighResultsTable
            // 
            this.ELFHighResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighResultsTable.Location = new System.Drawing.Point(83, 0);
            this.ELFHighResultsTable.Name = "ELFHighResultsTable";
            this.ELFHighResultsTable.Size = new System.Drawing.Size(574, 461);
            this.ELFHighResultsTable.TabIndex = 1;
            this.ELFHighResultsTable.UseWaitCursor = true;
            // 
            // ELFMaxResultsTabPage
            // 
            this.ELFMaxResultsTabPage.Controls.Add(this.ELFMaxResultsTable);
            this.ELFMaxResultsTabPage.Location = new System.Drawing.Point(4, 26);
            this.ELFMaxResultsTabPage.Name = "ELFMaxResultsTabPage";
            this.ELFMaxResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ELFMaxResultsTabPage.Size = new System.Drawing.Size(881, 473);
            this.ELFMaxResultsTabPage.TabIndex = 4;
            this.ELFMaxResultsTabPage.Text = "Maksimum Sonuçlar";
            this.ELFMaxResultsTabPage.UseVisualStyleBackColor = true;
            this.ELFMaxResultsTabPage.UseWaitCursor = true;
            // 
            // ELFMaxResultsTable
            // 
            this.ELFMaxResultsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxResultsTable.Location = new System.Drawing.Point(34, 3);
            this.ELFMaxResultsTable.Name = "ELFMaxResultsTable";
            this.ELFMaxResultsTable.Size = new System.Drawing.Size(690, 461);
            this.ELFMaxResultsTable.TabIndex = 1;
            this.ELFMaxResultsTable.UseWaitCursor = true;
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
            this.ELFGraphicsPanel.Location = new System.Drawing.Point(2, 2);
            this.ELFGraphicsPanel.Name = "ELFGraphicsPanel";
            this.ELFGraphicsPanel.Size = new System.Drawing.Size(425, 503);
            this.ELFGraphicsPanel.TabIndex = 22;
            this.ELFGraphicsPanel.UseWaitCursor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(201, 58);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 17);
            this.label10.TabIndex = 30;
            this.label10.Text = "Senaryo Seçimi:";
            this.label10.UseWaitCursor = true;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.ForeColor = System.Drawing.Color.Transparent;
            this.button1.Location = new System.Drawing.Point(319, 58);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 36);
            this.button1.TabIndex = 29;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.UseWaitCursor = true;
            // 
            // ELFRadioButtonsPanel
            // 
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFLowSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFMaxSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFMinSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFHighSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Controls.Add(this.ELFBaseSenaryoRadioButton);
            this.ELFRadioButtonsPanel.Location = new System.Drawing.Point(6, 6);
            this.ELFRadioButtonsPanel.Name = "ELFRadioButtonsPanel";
            this.ELFRadioButtonsPanel.Size = new System.Drawing.Size(175, 168);
            this.ELFRadioButtonsPanel.TabIndex = 28;
            this.ELFRadioButtonsPanel.UseWaitCursor = true;
            // 
            // ELFLowSenaryoRadioButton
            // 
            this.ELFLowSenaryoRadioButton.AutoSize = true;
            this.ELFLowSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFLowSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFLowSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFLowSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFLowSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFLowSenaryoRadioButton.Location = new System.Drawing.Point(3, 40);
            this.ELFLowSenaryoRadioButton.Name = "ELFLowSenaryoRadioButton";
            this.ELFLowSenaryoRadioButton.Size = new System.Drawing.Size(118, 21);
            this.ELFLowSenaryoRadioButton.TabIndex = 24;
            this.ELFLowSenaryoRadioButton.Text = "Düşük Senaryo";
            this.ELFLowSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFLowSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFLowSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFLowSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.ELFLowSenaryoRadioButton.UseWaitCursor = true;
            // 
            // ELFMaxSenaryoRadioButton
            // 
            this.ELFMaxSenaryoRadioButton.AutoSize = true;
            this.ELFMaxSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMaxSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFMaxSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMaxSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFMaxSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFMaxSenaryoRadioButton.Location = new System.Drawing.Point(3, 127);
            this.ELFMaxSenaryoRadioButton.Name = "ELFMaxSenaryoRadioButton";
            this.ELFMaxSenaryoRadioButton.Size = new System.Drawing.Size(147, 21);
            this.ELFMaxSenaryoRadioButton.TabIndex = 27;
            this.ELFMaxSenaryoRadioButton.Text = "Maksimum Senaryo";
            this.ELFMaxSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFMaxSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFMaxSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFMaxSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.ELFMaxSenaryoRadioButton.UseWaitCursor = true;
            // 
            // ELFMinSenaryoRadioButton
            // 
            this.ELFMinSenaryoRadioButton.AutoSize = true;
            this.ELFMinSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMinSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFMinSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFMinSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFMinSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFMinSenaryoRadioButton.Location = new System.Drawing.Point(3, 8);
            this.ELFMinSenaryoRadioButton.Name = "ELFMinSenaryoRadioButton";
            this.ELFMinSenaryoRadioButton.Size = new System.Drawing.Size(138, 21);
            this.ELFMinSenaryoRadioButton.TabIndex = 23;
            this.ELFMinSenaryoRadioButton.Text = "Minimum Senaryo";
            this.ELFMinSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFMinSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFMinSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFMinSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.ELFMinSenaryoRadioButton.UseWaitCursor = true;
            // 
            // ELFHighSenaryoRadioButton
            // 
            this.ELFHighSenaryoRadioButton.AutoSize = true;
            this.ELFHighSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFHighSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFHighSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFHighSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFHighSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFHighSenaryoRadioButton.Location = new System.Drawing.Point(3, 94);
            this.ELFHighSenaryoRadioButton.Name = "ELFHighSenaryoRadioButton";
            this.ELFHighSenaryoRadioButton.Size = new System.Drawing.Size(122, 21);
            this.ELFHighSenaryoRadioButton.TabIndex = 26;
            this.ELFHighSenaryoRadioButton.Text = "Yüksek Senaryo";
            this.ELFHighSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFHighSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFHighSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFHighSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.ELFHighSenaryoRadioButton.UseWaitCursor = true;
            // 
            // ELFBaseSenaryoRadioButton
            // 
            this.ELFBaseSenaryoRadioButton.AutoSize = true;
            this.ELFBaseSenaryoRadioButton.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFBaseSenaryoRadioButton.CheckedState.BorderThickness = 0;
            this.ELFBaseSenaryoRadioButton.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ELFBaseSenaryoRadioButton.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ELFBaseSenaryoRadioButton.CheckedState.InnerOffset = -4;
            this.ELFBaseSenaryoRadioButton.Location = new System.Drawing.Point(3, 67);
            this.ELFBaseSenaryoRadioButton.Name = "ELFBaseSenaryoRadioButton";
            this.ELFBaseSenaryoRadioButton.Size = new System.Drawing.Size(101, 21);
            this.ELFBaseSenaryoRadioButton.TabIndex = 25;
            this.ELFBaseSenaryoRadioButton.Text = "Baz Senaryo";
            this.ELFBaseSenaryoRadioButton.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ELFBaseSenaryoRadioButton.UncheckedState.BorderThickness = 2;
            this.ELFBaseSenaryoRadioButton.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.ELFBaseSenaryoRadioButton.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.ELFBaseSenaryoRadioButton.UseWaitCursor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(204, 133);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 57);
            this.button2.TabIndex = 19;
            this.button2.Text = "Tüm Sonuçları Görüntüle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.UseWaitCursor = true;
            // 
            // ELFPredictionButton
            // 
            this.ELFPredictionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ELFPredictionButton.Location = new System.Drawing.Point(71, 197);
            this.ELFPredictionButton.Margin = new System.Windows.Forms.Padding(2);
            this.ELFPredictionButton.Name = "ELFPredictionButton";
            this.ELFPredictionButton.Size = new System.Drawing.Size(120, 36);
            this.ELFPredictionButton.TabIndex = 18;
            this.ELFPredictionButton.Text = "Tahmin Yap";
            this.ELFPredictionButton.UseVisualStyleBackColor = true;
            this.ELFPredictionButton.UseWaitCursor = true;
            // 
            // SenaryoSelectionButton
            // 
            this.SenaryoSelectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SenaryoSelectionButton.Location = new System.Drawing.Point(27, 449);
            this.SenaryoSelectionButton.Margin = new System.Windows.Forms.Padding(2);
            this.SenaryoSelectionButton.Name = "SenaryoSelectionButton";
            this.SenaryoSelectionButton.Size = new System.Drawing.Size(119, 36);
            this.SenaryoSelectionButton.TabIndex = 17;
            this.SenaryoSelectionButton.Text = "Senaryo Seç";
            this.SenaryoSelectionButton.UseVisualStyleBackColor = true;
            this.SenaryoSelectionButton.UseWaitCursor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(326, 6);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(98, 17);
            this.label12.TabIndex = 7;
            this.label12.Text = "Veri Ön İzleme:";
            this.label12.UseWaitCursor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 245);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(229, 171);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.UseWaitCursor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 225);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 17);
            this.label9.TabIndex = 8;
            this.label9.Text = "Grafikler:";
            this.label9.UseWaitCursor = true;
            // 
            // tab_imar
            // 
            this.tab_imar.Controls.Add(this.button6);
            this.tab_imar.Location = new System.Drawing.Point(4, 48);
            this.tab_imar.Margin = new System.Windows.Forms.Padding(2);
            this.tab_imar.Name = "tab_imar";
            this.tab_imar.Size = new System.Drawing.Size(1324, 507);
            this.tab_imar.TabIndex = 4;
            this.tab_imar.Text = "İmar Analizleri";
            this.tab_imar.UseVisualStyleBackColor = true;
            this.tab_imar.UseWaitCursor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(50, 123);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 32);
            this.button6.TabIndex = 19;
            this.button6.Text = "Temizle";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.UseWaitCursor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // tab_optDTR
            // 
            this.tab_optDTR.Location = new System.Drawing.Point(4, 48);
            this.tab_optDTR.Margin = new System.Windows.Forms.Padding(2);
            this.tab_optDTR.Name = "tab_optDTR";
            this.tab_optDTR.Size = new System.Drawing.Size(1324, 507);
            this.tab_optDTR.TabIndex = 7;
            this.tab_optDTR.Text = "Optimal DTR Konumlandırma";
            this.tab_optDTR.UseVisualStyleBackColor = true;
            this.tab_optDTR.UseWaitCursor = true;
            // 
            // tab_senaryo
            // 
            this.tab_senaryo.Controls.Add(this.SenaryoModulePanel);
            this.tab_senaryo.Location = new System.Drawing.Point(4, 48);
            this.tab_senaryo.Margin = new System.Windows.Forms.Padding(2);
            this.tab_senaryo.Name = "tab_senaryo";
            this.tab_senaryo.Size = new System.Drawing.Size(1324, 507);
            this.tab_senaryo.TabIndex = 3;
            this.tab_senaryo.Text = "Senaryo Oluşturma Modülü";
            this.tab_senaryo.UseVisualStyleBackColor = true;
            this.tab_senaryo.UseWaitCursor = true;
            // 
            // SenaryoModulePanel
            // 
            this.SenaryoModulePanel.Controls.Add(this.SenaryoModuleTabControl);
            this.SenaryoModulePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SenaryoModulePanel.Location = new System.Drawing.Point(0, 0);
            this.SenaryoModulePanel.Name = "SenaryoModulePanel";
            this.SenaryoModulePanel.Size = new System.Drawing.Size(1324, 507);
            this.SenaryoModulePanel.TabIndex = 0;
            this.SenaryoModulePanel.UseWaitCursor = true;
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
            this.SenaryoModuleTabControl.Name = "SenaryoModuleTabControl";
            this.SenaryoModuleTabControl.SelectedIndex = 0;
            this.SenaryoModuleTabControl.Size = new System.Drawing.Size(1324, 507);
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
            this.SenaryoModuleTabControl.UseWaitCursor = true;
            // 
            // EkonometrikSenaryoTabPage
            // 
            this.EkonometrikSenaryoTabPage.Controls.Add(this.EkonometrikSenaryoOutputsPanel);
            this.EkonometrikSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.EkonometrikSenaryoTabPage.Name = "EkonometrikSenaryoTabPage";
            this.EkonometrikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.EkonometrikSenaryoTabPage.Size = new System.Drawing.Size(1136, 499);
            this.EkonometrikSenaryoTabPage.TabIndex = 0;
            this.EkonometrikSenaryoTabPage.Text = "Ekonometrik Senaryolar";
            this.EkonometrikSenaryoTabPage.UseVisualStyleBackColor = true;
            this.EkonometrikSenaryoTabPage.UseWaitCursor = true;
            // 
            // EkonometrikSenaryoOutputsPanel
            // 
            this.EkonometrikSenaryoOutputsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.ELFSenaryoTabControls);
            this.EkonometrikSenaryoOutputsPanel.Controls.Add(this.EkonometrikSenaryoElementsPanel);
            this.EkonometrikSenaryoOutputsPanel.Location = new System.Drawing.Point(3, 3);
            this.EkonometrikSenaryoOutputsPanel.Name = "EkonometrikSenaryoOutputsPanel";
            this.EkonometrikSenaryoOutputsPanel.Size = new System.Drawing.Size(1130, 493);
            this.EkonometrikSenaryoOutputsPanel.TabIndex = 0;
            this.EkonometrikSenaryoOutputsPanel.UseWaitCursor = true;
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
            this.ELFSenaryoTabControls.Location = new System.Drawing.Point(3, 3);
            this.ELFSenaryoTabControls.Name = "ELFSenaryoTabControls";
            this.ELFSenaryoTabControls.SelectedIndex = 0;
            this.ELFSenaryoTabControls.Size = new System.Drawing.Size(922, 497);
            this.ELFSenaryoTabControls.TabIndex = 2;
            this.ELFSenaryoTabControls.UseWaitCursor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ELFMinSenaryoTable);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(914, 467);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Minimum Senaryo";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.UseWaitCursor = true;
            // 
            // ELFMinSenaryoTable
            // 
            this.ELFMinSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMinSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMinSenaryoTable.Location = new System.Drawing.Point(3, 3);
            this.ELFMinSenaryoTable.Name = "ELFMinSenaryoTable";
            this.ELFMinSenaryoTable.Size = new System.Drawing.Size(908, 461);
            this.ELFMinSenaryoTable.TabIndex = 0;
            this.ELFMinSenaryoTable.UseWaitCursor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ELFLowSenaryoTable);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(914, 467);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Düşük Senaryo";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.UseWaitCursor = true;
            // 
            // ELFLowSenaryoTable
            // 
            this.ELFLowSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFLowSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFLowSenaryoTable.Location = new System.Drawing.Point(3, 3);
            this.ELFLowSenaryoTable.Name = "ELFLowSenaryoTable";
            this.ELFLowSenaryoTable.Size = new System.Drawing.Size(908, 461);
            this.ELFLowSenaryoTable.TabIndex = 1;
            this.ELFLowSenaryoTable.UseWaitCursor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.ELFBaseSenaryoTable);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(914, 467);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Baz Senaryo";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.UseWaitCursor = true;
            // 
            // ELFBaseSenaryoTable
            // 
            this.ELFBaseSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFBaseSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFBaseSenaryoTable.Location = new System.Drawing.Point(3, 3);
            this.ELFBaseSenaryoTable.Name = "ELFBaseSenaryoTable";
            this.ELFBaseSenaryoTable.Size = new System.Drawing.Size(908, 461);
            this.ELFBaseSenaryoTable.TabIndex = 1;
            this.ELFBaseSenaryoTable.UseWaitCursor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ELFHighSenaryoTable);
            this.tabPage5.Location = new System.Drawing.Point(4, 26);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(914, 467);
            this.tabPage5.TabIndex = 3;
            this.tabPage5.Text = "Yüksek Senaryo";
            this.tabPage5.UseVisualStyleBackColor = true;
            this.tabPage5.UseWaitCursor = true;
            // 
            // ELFHighSenaryoTable
            // 
            this.ELFHighSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFHighSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFHighSenaryoTable.Location = new System.Drawing.Point(3, 3);
            this.ELFHighSenaryoTable.Name = "ELFHighSenaryoTable";
            this.ELFHighSenaryoTable.Size = new System.Drawing.Size(908, 461);
            this.ELFHighSenaryoTable.TabIndex = 1;
            this.ELFHighSenaryoTable.UseWaitCursor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.ELFMaxSenaryoTable);
            this.tabPage6.Location = new System.Drawing.Point(4, 26);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(914, 467);
            this.tabPage6.TabIndex = 4;
            this.tabPage6.Text = "Maksimum Senaryo";
            this.tabPage6.UseVisualStyleBackColor = true;
            this.tabPage6.UseWaitCursor = true;
            // 
            // ELFMaxSenaryoTable
            // 
            this.ELFMaxSenaryoTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ELFMaxSenaryoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ELFMaxSenaryoTable.Location = new System.Drawing.Point(3, 3);
            this.ELFMaxSenaryoTable.Name = "ELFMaxSenaryoTable";
            this.ELFMaxSenaryoTable.Size = new System.Drawing.Size(908, 461);
            this.ELFMaxSenaryoTable.TabIndex = 1;
            this.ELFMaxSenaryoTable.UseWaitCursor = true;
            // 
            // EkonometrikSenaryoElementsPanel
            // 
            this.EkonometrikSenaryoElementsPanel.BackColor = System.Drawing.Color.Snow;
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.richTextBox1);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFPredictionShowResultsGunaButton);
            this.EkonometrikSenaryoElementsPanel.Controls.Add(this.ELFScenerioSaveGunaButton);
            this.EkonometrikSenaryoElementsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.EkonometrikSenaryoElementsPanel.Location = new System.Drawing.Point(923, 0);
            this.EkonometrikSenaryoElementsPanel.Name = "EkonometrikSenaryoElementsPanel";
            this.EkonometrikSenaryoElementsPanel.Size = new System.Drawing.Size(207, 493);
            this.EkonometrikSenaryoElementsPanel.TabIndex = 1;
            this.EkonometrikSenaryoElementsPanel.UseWaitCursor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Snow;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.richTextBox1.Location = new System.Drawing.Point(7, 32);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(203, 99);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "Tablolar üzerinde değişiklik yaparak senaryo üretebilirsiniz. Yeni senaryo tahmin" +
    " sonuçlarını görüntülemek için lütfen önce değişiklikleri kaydedin.";
            this.richTextBox1.UseWaitCursor = true;
            // 
            // ELFPredictionShowResultsGunaButton
            // 
            this.ELFPredictionShowResultsGunaButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.ELFPredictionShowResultsGunaButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.ELFPredictionShowResultsGunaButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.ELFPredictionShowResultsGunaButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.ELFPredictionShowResultsGunaButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ELFPredictionShowResultsGunaButton.ForeColor = System.Drawing.Color.White;
            this.ELFPredictionShowResultsGunaButton.Location = new System.Drawing.Point(0, 233);
            this.ELFPredictionShowResultsGunaButton.Name = "ELFPredictionShowResultsGunaButton";
            this.ELFPredictionShowResultsGunaButton.Size = new System.Drawing.Size(204, 45);
            this.ELFPredictionShowResultsGunaButton.TabIndex = 9;
            this.ELFPredictionShowResultsGunaButton.Text = "Tahmin Sonuçlarını Göster";
            this.ELFPredictionShowResultsGunaButton.UseWaitCursor = true;
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
            this.ELFScenerioSaveGunaButton.Location = new System.Drawing.Point(0, 170);
            this.ELFScenerioSaveGunaButton.Name = "ELFScenerioSaveGunaButton";
            this.ELFScenerioSaveGunaButton.Size = new System.Drawing.Size(206, 45);
            this.ELFScenerioSaveGunaButton.TabIndex = 6;
            this.ELFScenerioSaveGunaButton.Text = "Senaryo Değişikliklerini Kaydet";
            this.ELFScenerioSaveGunaButton.UseWaitCursor = true;
            this.ELFScenerioSaveGunaButton.Click += new System.EventHandler(this.ELFScenerioSaveGunaButton_Click);
            // 
            // StokastikSenaryoTabPage
            // 
            this.StokastikSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.StokastikSenaryoTabPage.Name = "StokastikSenaryoTabPage";
            this.StokastikSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.StokastikSenaryoTabPage.Size = new System.Drawing.Size(1136, 499);
            this.StokastikSenaryoTabPage.TabIndex = 1;
            this.StokastikSenaryoTabPage.Text = "Stokastik Senaryolar";
            this.StokastikSenaryoTabPage.UseVisualStyleBackColor = true;
            this.StokastikSenaryoTabPage.UseWaitCursor = true;
            // 
            // EASarjSenaryoTabPage
            // 
            this.EASarjSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.EASarjSenaryoTabPage.Name = "EASarjSenaryoTabPage";
            this.EASarjSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.EASarjSenaryoTabPage.Size = new System.Drawing.Size(1136, 499);
            this.EASarjSenaryoTabPage.TabIndex = 2;
            this.EASarjSenaryoTabPage.Text = "EA Şarj Senaryoları";
            this.EASarjSenaryoTabPage.UseVisualStyleBackColor = true;
            this.EASarjSenaryoTabPage.UseWaitCursor = true;
            // 
            // DEKSenaryoTabPage
            // 
            this.DEKSenaryoTabPage.Location = new System.Drawing.Point(184, 4);
            this.DEKSenaryoTabPage.Name = "DEKSenaryoTabPage";
            this.DEKSenaryoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DEKSenaryoTabPage.Size = new System.Drawing.Size(1136, 499);
            this.DEKSenaryoTabPage.TabIndex = 3;
            this.DEKSenaryoTabPage.Text = "DEK Senaryoları";
            this.DEKSenaryoTabPage.UseVisualStyleBackColor = true;
            this.DEKSenaryoTabPage.UseWaitCursor = true;
            // 
            // tabPage8
            // 
            this.tabPage8.Location = new System.Drawing.Point(184, 4);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1136, 499);
            this.tabPage8.TabIndex = 4;
            this.tabPage8.Text = "Yeni Genişleme Alanları ";
            this.tabPage8.UseVisualStyleBackColor = true;
            this.tabPage8.UseWaitCursor = true;
            // 
            // tab_stokastik
            // 
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
            this.tab_stokastik.Controls.Add(this.Seç_Stokastik);
            this.tab_stokastik.Location = new System.Drawing.Point(4, 48);
            this.tab_stokastik.Margin = new System.Windows.Forms.Padding(2);
            this.tab_stokastik.Name = "tab_stokastik";
            this.tab_stokastik.Size = new System.Drawing.Size(1324, 507);
            this.tab_stokastik.TabIndex = 2;
            this.tab_stokastik.Text = "Stokastik Yük Tahmini Modülü";
            this.tab_stokastik.UseVisualStyleBackColor = true;
            // 
            // checkBox21
            // 
            this.checkBox21.AutoSize = true;
            this.checkBox21.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox21.Location = new System.Drawing.Point(6, 470);
            this.checkBox21.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox21.Name = "checkBox21";
            this.checkBox21.Size = new System.Drawing.Size(96, 21);
            this.checkBox21.TabIndex = 35;
            this.checkBox21.Text = "checkBox21";
            this.checkBox21.UseVisualStyleBackColor = true;
            this.checkBox21.Visible = false;
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
            this.katmanlar_right_click.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.katmanlar_right_click_Closing);
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
            // checkBox20
            // 
            this.checkBox20.AutoSize = true;
            this.checkBox20.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox20.Location = new System.Drawing.Point(6, 442);
            this.checkBox20.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox20.Name = "checkBox20";
            this.checkBox20.Size = new System.Drawing.Size(98, 21);
            this.checkBox20.TabIndex = 34;
            this.checkBox20.Text = "checkBox20";
            this.checkBox20.UseVisualStyleBackColor = true;
            this.checkBox20.Visible = false;
            // 
            // checkBox19
            // 
            this.checkBox19.AutoSize = true;
            this.checkBox19.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox19.Location = new System.Drawing.Point(7, 414);
            this.checkBox19.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox19.Name = "checkBox19";
            this.checkBox19.Size = new System.Drawing.Size(96, 21);
            this.checkBox19.TabIndex = 33;
            this.checkBox19.Text = "checkBox19";
            this.checkBox19.UseVisualStyleBackColor = true;
            this.checkBox19.Visible = false;
            // 
            // mesafe_metre_stokastik
            // 
            this.mesafe_metre_stokastik.AutoSize = true;
            this.mesafe_metre_stokastik.Location = new System.Drawing.Point(306, 46);
            this.mesafe_metre_stokastik.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mesafe_metre_stokastik.Name = "mesafe_metre_stokastik";
            this.mesafe_metre_stokastik.Size = new System.Drawing.Size(0, 17);
            this.mesafe_metre_stokastik.TabIndex = 32;
            this.mesafe_metre_stokastik.Visible = false;
            // 
            // Mesafe_stokastik
            // 
            this.Mesafe_stokastik.AutoSize = true;
            this.Mesafe_stokastik.Location = new System.Drawing.Point(236, 46);
            this.Mesafe_stokastik.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Mesafe_stokastik.Name = "Mesafe_stokastik";
            this.Mesafe_stokastik.Size = new System.Drawing.Size(54, 17);
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
            this.gMapControl_stokastik.Location = new System.Drawing.Point(230, 38);
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
            this.gMapControl_stokastik.Size = new System.Drawing.Size(797, 513);
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
            this.buton_stokastik_harita_katmanlar.Location = new System.Drawing.Point(230, 509);
            this.buton_stokastik_harita_katmanlar.Margin = new System.Windows.Forms.Padding(2);
            this.buton_stokastik_harita_katmanlar.Name = "buton_stokastik_harita_katmanlar";
            this.buton_stokastik_harita_katmanlar.Size = new System.Drawing.Size(46, 42);
            this.buton_stokastik_harita_katmanlar.TabIndex = 29;
            this.buton_stokastik_harita_katmanlar.UseVisualStyleBackColor = true;
            // 
            // checkBox18
            // 
            this.checkBox18.AutoSize = true;
            this.checkBox18.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox18.Location = new System.Drawing.Point(6, 387);
            this.checkBox18.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox18.Name = "checkBox18";
            this.checkBox18.Size = new System.Drawing.Size(96, 21);
            this.checkBox18.TabIndex = 28;
            this.checkBox18.Text = "checkBox18";
            this.checkBox18.UseVisualStyleBackColor = true;
            this.checkBox18.Visible = false;
            // 
            // checkBox17
            // 
            this.checkBox17.AutoSize = true;
            this.checkBox17.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox17.Location = new System.Drawing.Point(6, 359);
            this.checkBox17.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox17.Name = "checkBox17";
            this.checkBox17.Size = new System.Drawing.Size(96, 21);
            this.checkBox17.TabIndex = 27;
            this.checkBox17.Text = "checkBox17";
            this.checkBox17.UseVisualStyleBackColor = true;
            this.checkBox17.Visible = false;
            // 
            // checkBox16
            // 
            this.checkBox16.AutoSize = true;
            this.checkBox16.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox16.Location = new System.Drawing.Point(6, 332);
            this.checkBox16.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox16.Name = "checkBox16";
            this.checkBox16.Size = new System.Drawing.Size(96, 21);
            this.checkBox16.TabIndex = 26;
            this.checkBox16.Text = "checkBox16";
            this.checkBox16.UseVisualStyleBackColor = true;
            this.checkBox16.Visible = false;
            // 
            // checkBox15
            // 
            this.checkBox15.AutoSize = true;
            this.checkBox15.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox15.Location = new System.Drawing.Point(6, 304);
            this.checkBox15.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new System.Drawing.Size(96, 21);
            this.checkBox15.TabIndex = 25;
            this.checkBox15.Text = "checkBox15";
            this.checkBox15.UseVisualStyleBackColor = true;
            this.checkBox15.Visible = false;
            // 
            // checkBox14
            // 
            this.checkBox14.AutoSize = true;
            this.checkBox14.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox14.Location = new System.Drawing.Point(6, 276);
            this.checkBox14.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(96, 21);
            this.checkBox14.TabIndex = 24;
            this.checkBox14.Text = "checkBox14";
            this.checkBox14.UseVisualStyleBackColor = true;
            this.checkBox14.Visible = false;
            // 
            // checkBox13
            // 
            this.checkBox13.AutoSize = true;
            this.checkBox13.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox13.Location = new System.Drawing.Point(6, 249);
            this.checkBox13.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(96, 21);
            this.checkBox13.TabIndex = 23;
            this.checkBox13.Text = "checkBox13";
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox13.Visible = false;
            // 
            // checkBox12
            // 
            this.checkBox12.AutoSize = true;
            this.checkBox12.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox12.Location = new System.Drawing.Point(6, 221);
            this.checkBox12.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(96, 21);
            this.checkBox12.TabIndex = 22;
            this.checkBox12.Text = "checkBox12";
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox12.Visible = false;
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox11.Location = new System.Drawing.Point(6, 193);
            this.checkBox11.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(94, 21);
            this.checkBox11.TabIndex = 21;
            this.checkBox11.Text = "checkBox11";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.Visible = false;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.ContextMenuStrip = this.katmanlar_right_click;
            this.checkBox10.Location = new System.Drawing.Point(6, 166);
            this.checkBox10.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(96, 21);
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
            this.checkBox9.Location = new System.Drawing.Point(6, 138);
            this.checkBox9.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(91, 21);
            this.checkBox9.TabIndex = 19;
            this.checkBox9.Text = "checkBox9";
            this.checkBox9.UseVisualStyleBackColor = false;
            this.checkBox9.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(18, 105);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 19);
            this.label13.TabIndex = 18;
            this.label13.Text = "Katmanlar";
            // 
            // stokastik_dosya_seçimi
            // 
            this.stokastik_dosya_seçimi.Location = new System.Drawing.Point(6, 38);
            this.stokastik_dosya_seçimi.Margin = new System.Windows.Forms.Padding(2);
            this.stokastik_dosya_seçimi.Name = "stokastik_dosya_seçimi";
            this.stokastik_dosya_seçimi.Size = new System.Drawing.Size(130, 36);
            this.stokastik_dosya_seçimi.TabIndex = 17;
            this.stokastik_dosya_seçimi.Text = "Dosya Seç";
            this.stokastik_dosya_seçimi.UseVisualStyleBackColor = true;
            this.stokastik_dosya_seçimi.Click += new System.EventHandler(this.stokastik_dosya_seçimi_Click);
            // 
            // Seç_Stokastik
            // 
            this.Seç_Stokastik.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Seç_Stokastik.Dock = System.Windows.Forms.DockStyle.None;
            this.Seç_Stokastik.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Seç_Stokastik.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.Seç_Stokastik.Location = new System.Drawing.Point(7, 9);
            this.Seç_Stokastik.Name = "Seç_Stokastik";
            this.Seç_Stokastik.Size = new System.Drawing.Size(1159, 27);
            this.Seç_Stokastik.TabIndex = 1;
            this.Seç_Stokastik.Text = "toolStrip1";
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
            this.toolStripSeparator3.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
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
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
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
            this.toolStripSeparator5.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
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
            this.toolStripSeparator6.Padding = new System.Windows.Forms.Padding(22, 0, 22, 0);
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
            this.tab_yükHaritası.Controls.Add(this.checkBox7);
            this.tab_yükHaritası.Controls.Add(this.checkBox6);
            this.tab_yükHaritası.Controls.Add(this.checkBox5);
            this.tab_yükHaritası.Controls.Add(this.checkBox4);
            this.tab_yükHaritası.Controls.Add(this.checkBox3);
            this.tab_yükHaritası.Controls.Add(this.checkBox2);
            this.tab_yükHaritası.Controls.Add(this.checkBox1);
            this.tab_yükHaritası.Controls.Add(this.label4);
            this.tab_yükHaritası.Controls.Add(this.button4);
            this.tab_yükHaritası.Controls.Add(this.button3);
            this.tab_yükHaritası.Controls.Add(this.webView21);
            this.tab_yükHaritası.Controls.Add(this.panel1);
            this.tab_yükHaritası.Location = new System.Drawing.Point(4, 48);
            this.tab_yükHaritası.Margin = new System.Windows.Forms.Padding(2);
            this.tab_yükHaritası.Name = "tab_yükHaritası";
            this.tab_yükHaritası.Size = new System.Drawing.Size(1324, 507);
            this.tab_yükHaritası.TabIndex = 9;
            this.tab_yükHaritası.Text = "Yük Haritası Modülü";
            this.tab_yükHaritası.UseVisualStyleBackColor = true;
            this.tab_yükHaritası.UseWaitCursor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(6, 274);
            this.checkBox7.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(77, 21);
            this.checkBox7.TabIndex = 17;
            this.checkBox7.Text = "Grafikler";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.UseWaitCursor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(6, 301);
            this.checkBox6.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(93, 21);
            this.checkBox6.TabIndex = 16;
            this.checkBox6.Text = "İstatistikler";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.UseWaitCursor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(32, 198);
            this.checkBox5.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(96, 21);
            this.checkBox5.TabIndex = 15;
            this.checkBox5.Text = "Aydınlatma";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.UseWaitCursor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(32, 171);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(124, 21);
            this.checkBox4.TabIndex = 14;
            this.checkBox4.Text = "Tarımsal Sulama";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.UseWaitCursor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(32, 143);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(97, 21);
            this.checkBox3.TabIndex = 13;
            this.checkBox3.Text = "Ticarethane";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.UseWaitCursor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(32, 115);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(66, 21);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Sanayi";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.UseWaitCursor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(32, 88);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(74, 21);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Mesken";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.UseWaitCursor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Maiandra GD", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 19);
            this.label4.TabIndex = 10;
            this.label4.Text = "Abone Sınıfı:";
            this.label4.UseWaitCursor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(893, 7);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(129, 27);
            this.button4.TabIndex = 2;
            this.button4.Text = "OpenStreetMap";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.UseWaitCursor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(740, 7);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(129, 27);
            this.button3.TabIndex = 1;
            this.button3.Text = "Google Maps";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.UseWaitCursor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView21.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(190, 52);
            this.webView21.Margin = new System.Windows.Forms.Padding(2);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(845, 386);
            this.webView21.Source = new System.Uri("https://www.google.com/maps/@38.4420517,27.1028334,13.29z?entry=ttu", System.UriKind.Absolute);
            this.webView21.TabIndex = 0;
            this.webView21.UseWaitCursor = true;
            this.webView21.ZoomFactor = 1D;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox5);
            this.panel1.Controls.Add(this.textBox4);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(6, 329);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 150);
            this.panel1.TabIndex = 18;
            this.panel1.UseWaitCursor = true;
            this.panel1.Visible = false;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(91, 95);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(76, 25);
            this.textBox5.TabIndex = 7;
            this.textBox5.UseWaitCursor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(91, 67);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(76, 25);
            this.textBox4.TabIndex = 6;
            this.textBox4.UseWaitCursor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(91, 39);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(76, 25);
            this.textBox3.TabIndex = 5;
            this.textBox3.UseWaitCursor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(91, 9);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(76, 25);
            this.textBox2.TabIndex = 4;
            this.textBox2.UseWaitCursor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 98);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.MaximumSize = new System.Drawing.Size(62, 68);
            this.label8.MinimumSize = new System.Drawing.Size(62, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 68);
            this.label8.TabIndex = 3;
            this.label8.Text = "label8";
            this.label8.UseWaitCursor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 67);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "label7";
            this.label7.UseWaitCursor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 39);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "label6";
            this.label6.UseWaitCursor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 11);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "label5";
            this.label5.UseWaitCursor = true;
            // 
            // tab_rapor
            // 
            this.tab_rapor.Location = new System.Drawing.Point(4, 48);
            this.tab_rapor.Margin = new System.Windows.Forms.Padding(2);
            this.tab_rapor.Name = "tab_rapor";
            this.tab_rapor.Size = new System.Drawing.Size(1324, 507);
            this.tab_rapor.TabIndex = 8;
            this.tab_rapor.Text = "Raporlama";
            this.tab_rapor.UseVisualStyleBackColor = true;
            this.tab_rapor.UseWaitCursor = true;
            // 
            // tab_validasyon
            // 
            this.tab_validasyon.Location = new System.Drawing.Point(4, 48);
            this.tab_validasyon.Margin = new System.Windows.Forms.Padding(2);
            this.tab_validasyon.Name = "tab_validasyon";
            this.tab_validasyon.Size = new System.Drawing.Size(1324, 507);
            this.tab_validasyon.TabIndex = 10;
            this.tab_validasyon.Text = "Validasyon Modülü";
            this.tab_validasyon.UseVisualStyleBackColor = true;
            this.tab_validasyon.UseWaitCursor = true;
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
            this.HomePageButton.Location = new System.Drawing.Point(1242, 2);
            this.HomePageButton.Margin = new System.Windows.Forms.Padding(2);
            this.HomePageButton.Name = "HomePageButton";
            this.HomePageButton.Size = new System.Drawing.Size(79, 30);
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
            // yenidenAdlandırToolStripMenuItem
            // 
            this.yenidenAdlandırToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("yenidenAdlandırToolStripMenuItem.Image")));
            this.yenidenAdlandırToolStripMenuItem.Name = "yenidenAdlandırToolStripMenuItem";
            this.yenidenAdlandırToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            this.yenidenAdlandırToolStripMenuItem.Text = "Yeniden Adlandır";
            this.yenidenAdlandırToolStripMenuItem.Click += new System.EventHandler(this.yenidenAdlandırToolStripMenuItem_Click);
            // 
            // ModuleTabPanel
            // 
            this.ModuleTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModuleTabPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.ModuleTabPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ModuleTabPanel.Controls.Add(this.Modül_Tabları);
            this.ModuleTabPanel.Location = new System.Drawing.Point(0, 54);
            this.ModuleTabPanel.Name = "ModuleTabPanel";
            this.ModuleTabPanel.Size = new System.Drawing.Size(1332, 559);
            this.ModuleTabPanel.TabIndex = 5;
            this.ModuleTabPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ModuleTabPanel_Paint);
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.HeaderPanel.Controls.Add(this.HomePageButton);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 24);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1332, 34);
            this.HeaderPanel.TabIndex = 3;
            // 
            // ModülFormu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1332, 612);
            this.Controls.Add(this.HeaderPanel);
            this.Controls.Add(this.ModuleTabPanel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.DarkOrange;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
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
            this.katmanlar_right_click.ResumeLayout(false);
            this.Seç_Stokastik.ResumeLayout(false);
            this.Seç_Stokastik.PerformLayout();
            this.tab_yükHaritası.ResumeLayout(false);
            this.tab_yükHaritası.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        public System.Windows.Forms.TabPage tab_ea;
        private System.Windows.Forms.TabPage tab_girdi;
        private System.Windows.Forms.TabPage tab_ekonometrik;
        private System.Windows.Forms.TabPage tab_imar;
        private System.Windows.Forms.TabPage tab_dek;
        private System.Windows.Forms.TabPage tab_optDTR;
        private System.Windows.Forms.TabPage tab_rapor;
        private System.Windows.Forms.TabPage tab_yükHaritası;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox veri_listesi_seçimi;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectFolderButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TabPage tab_validasyon;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button6;
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
        //private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox EA_list_box;
        //private System.Windows.Forms.Button button6;
        public GMap.NET.WindowsForms.GMapControl gMapControl_EA;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button oznitelikAc;
        private System.Windows.Forms.Button ButtonKml;
        public GMap.NET.WindowsForms.GMapControl gMapControl_stokastik;
        private System.Windows.Forms.ContextMenuStrip harita_katmanları_right_click;
        private System.Windows.Forms.ToolStripMenuItem Arazi;
        private System.Windows.Forms.ToolStripMenuItem Harita;
        private System.Windows.Forms.ToolStripMenuItem Uydu;
        private System.Windows.Forms.ToolStripMenuItem Google_Earth;
        private System.Windows.Forms.ToolStripMenuItem OSM;
        private System.Windows.Forms.Label mesafe_metre_stokastik;
        private System.Windows.Forms.Label Mesafe_stokastik;
        private System.Windows.Forms.Button buton_ea_harita_katmanlar;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton EA_Seç;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton EA_Kaydır;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton EA_Mesafe_Ölç;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton EA_Alan_Ölç;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton EA_Poligon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripButton EA_Nokta;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripButton toolStripButton15;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton toolStripButton16;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.Label mesafe_metre_ea;
        private System.Windows.Forms.Label Mesafe_ea;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Nokta;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Ekle;
        private System.Windows.Forms.ToolStripMenuItem Nokta_Sil;
        private System.Windows.Forms.ToolStripMenuItem Kaydet;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Poligon;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Çiz;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Sil;
        private System.Windows.Forms.ToolStripMenuItem Poligon_Kaydet;
        private System.Windows.Forms.Label label14;
        private ContextMenuStrip ContextMenuStrip_Fonksiyon;
        private ToolStripMenuItem katman_birleştir;
        private ToolStripMenuItem overlap_analizi;
        private CheckBox checkBox21;
        private CheckBox checkBox20;
        private CheckBox checkBox19;
        private ToolStripMenuItem Google_Earth_Desktop;
        private ToolStrip Seç_Stokastik;
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
    }
}