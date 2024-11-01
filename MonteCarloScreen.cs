using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class MonteCarloScreen : Form
    {
        private GirdiModülü girdiModülü = new GirdiModülü();  // GirdiModülü nesnesini başlatıyoruz
        public int slfStartYear, slfEndYear;
        private ExcelService excelService = new ExcelService();
        private DataTable veriMonteCarlo = new DataTable();

        public MonteCarloScreen()
        {
            InitializeComponent();

            // GirdiModülü nesnesinden yıl değerlerini al
            slfStartYear = girdiModülü.SlfStartYear;
            slfEndYear = girdiModülü.SlfEndYear;

            Console.WriteLine("Başlangıç Yılı: " + slfStartYear);  // Kontrol için başlangıç yılını yazdır
            Console.WriteLine("Bitiş Yılı: " + slfEndYear);        // Kontrol için bitiş yılını yazdır

            InitializeComboBoxes();

            // comboBoxStartYear'ın SelectedIndexChanged olayını bağlayın
            comboBoxStartYear.SelectedIndexChanged += başlangic_SelectedIndexChanged;
        }

        private void InitializeComboBoxes()
        {
            // Yıl aralığını oluştur ve ComboBox'lara ekle
            var startYearList = new List<int>();
            for (int year = slfStartYear; year <= slfEndYear; year++)
            {
                startYearList.Add(year);
            }

            comboBoxStartYear.DataSource = new List<int>(startYearList);
            comboBoxEndYear.DataSource = new List<int>(startYearList);
        }

        private void LoadExcelData()
        {
            try
            {
                string filePath = @"C:\Users\batuhan.yetis\MRC\MRC - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\EA Şarj\Arşiv\montecarlo-deneme.xlsx";
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    veriMonteCarlo = excelService.LoadWorksheetIntoDataTable(worksheet);
                }
                MessageBox.Show("Veri başarıyla yüklendi ve görüntüleniyor.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void başlangic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStartYear.SelectedItem != null)
            {
                int selectedYear = (int)comboBoxStartYear.SelectedItem;
                MessageBox.Show("Seçilen Başlangıç Yılı: " + selectedYear);
            }
        }
    }
}
