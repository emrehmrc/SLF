using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using GMap.NET;
using System.Globalization;
using OfficeOpenXml;

namespace SLF
{
    public partial class Poligon_Özellik_Tanımlama : Form
    {
        public ModülFormu modül_formu;
        public CBS cbsFormu;
        public Nokta_Yuk_Bilgi_Formu yük_bilgi_formu_objesi;
        private DataTable dataTable;
        private string excelFilePath;
        private int layerIndex;
        private Dictionary<string, HashSet<string>> columnValues; // For dropdown options
        private List<Dictionary<string, string>> excelDataRows; // Store full Excel table data

        HashSet<string> manualColumns;

        private bool isSelecting_YGA;
        private bool isSelecting_YUK;
        private bool isSelecting_KentselDonusum;

        public bool is_poligon_saved = true;
        private bool isKaydetClicked = false;

        // Add a public property to access the DataTable
        public DataTable PolygonDataTable => dataTable;

        // Field to store the current KeyPress lambda delegate
        private KeyPressEventHandler _currentKeyPressHandler;


        public Poligon_Özellik_Tanımlama(bool isSelectingYUK, bool isSelectingYGA, bool isSelectingKentselDonusum,
            List<PointLatLng> polygonPoints, ModülFormu mainform)
        {

            InitializeComponent();
            modül_formu = new ModülFormu();
            cbsFormu = new CBS(modül_formu);
            this.modül_formu = mainform;

            // Set the flags before calling SetupDataGridView
            isSelecting_YUK = isSelectingYUK;
            isSelecting_YGA = isSelectingYGA;
            isSelecting_KentselDonusum = isSelectingKentselDonusum;

            excelFilePath = System.IO.Path.Combine(modül_formu.ana_menu_form_objesi.userRootPath,
                (string)modül_formu.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                (string)modül_formu.ana_menu_form_objesi.config.İl,
                (string)modül_formu.ana_menu_form_objesi.config.İlçe,
                (string)modül_formu.ana_menu_form_objesi.config.Point_Load_Musaade).Replace('/', '\\');


            // Determine layer index (already the polygon is added on to the overlay, so this will return 1)
            layerIndex = Array.FindIndex(modül_formu.cbs.tüm_katmanlar_array_imar, s => s == null);

            if (layerIndex < 0)
            {
                MessageBox.Show("Çizilen poligon geçersiz bir katmana ait!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                layerIndex = 0; // Default to 0 if not found
            }

            // Load Excel data and set up DataGridView
            SetupDataGridView(polygonPoints);

        }


        private void SetupDataGridView(List<PointLatLng> polygonPoints)
        {
            dataTable = new DataTable();

            // Create a string representation of the coordinates in WKT format
            string coordinates = $"Polygon (({string.Join(", ", polygonPoints.Select(p => $"{p.Lat} {p.Lng}"))}))";
            string area = Math.Round(cbsFormu.CalculatePolygonArea(polygonPoints), 1).ToString() + " m2";


            // construct the parameters of the point load addition 
            if (isSelecting_YUK == true)
            {
                // Add columns
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("Koordinatlar", typeof(string));
                dataTable.Columns.Add("Çizilen Alan (m2)", typeof(string));
                dataTable.Columns.Add("Ortalama Kapladığı Alan (m2)", typeof(string));
                dataTable.Columns.Add("Tüketim Sınıfı", typeof(string));
                dataTable.Columns.Add("ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + (layerIndex).ToString();
                dataTable.Rows[0]["Koordinatlar"] = coordinates;
                dataTable.Rows[0]["Çizilen Alan (m2)"] = area;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

                // Set tooltips for column headers
                PoligonDataGridView.Columns["Polygon ID"].ToolTipText = "Poligona ait özgün ID numarası";
                PoligonDataGridView.Columns["Tipi"].ToolTipText = "Poligonun tipi veya kategorisi - AVM, Restoran vb.";
                PoligonDataGridView.Columns["Koordinatlar"].ToolTipText = "Poligonun WKT formatındaki koordinatları";
                PoligonDataGridView.Columns["Çizilen Alan (m2)"].ToolTipText = "Poligonun hesaplanan alanı (metrekare)";
                PoligonDataGridView.Columns["Ortalama Kapladığı Alan (m2)"].ToolTipText = "Poligonun ortalama kapladığı alan (metrekare)";
                PoligonDataGridView.Columns["Tüketim Sınıfı"].ToolTipText = "Poligonun enerji tüketim sınıfı - Ticarethane, Sanayi, vb.";
                PoligonDataGridView.Columns["ENERJILENDIRME_YILI"].ToolTipText = "Poligonun enerjilendirmeye başladığı yıl";
                PoligonDataGridView.Columns["Kurulu Güç (kW)"].ToolTipText = "Poligonun kurulu güç kapasitesi (kilowatt)";
                PoligonDataGridView.Columns["Pik Yüklenme (%)"].ToolTipText = "Poligonun pik yüklenme oranı (yüzde)";
                PoligonDataGridView.Columns["Pik Demant (kW)"].ToolTipText = "Poligonun pik güç talebi (kilowatt)";

                // Populate dropdowns and store Excel data
                LoadExcelData();
                if (columnValues == null)
                {
                    return;
                }
                SetupDropdownColumns(columnValues);

            } else if (isSelecting_YGA == true)
            {
                // Add "Polygon ID" column
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Koordinatlar", typeof(string));
                dataTable.Columns.Add("Çizilen Alan (m2)", typeof(string));
                dataTable.Columns.Add("1-2 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("3-4 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("5-7 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("8 USTU KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("VILLA MESKEN", typeof(string));
                dataTable.Columns.Add("AYDINLATMA", typeof(string));
                dataTable.Columns.Add("KUCUK_SANAYI", typeof(string));
                dataTable.Columns.Add("KUCUK_TICARETHANE", typeof(string));
                dataTable.Columns.Add("ORTA_SANAYI", typeof(string));
                dataTable.Columns.Add("ORTA_TICARETHANE", typeof(string));
                dataTable.Columns.Add("TARIMSAL_SULAMA", typeof(string));
                dataTable.Columns.Add("Başlangıç Yılı", typeof(string));
                dataTable.Columns.Add("Satürasyon Hızı", typeof(string));
                dataTable.Columns.Add("TAKS", typeof(string));
                dataTable.Columns.Add("Park, Yol, Kaldırım Oranı (%)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + (layerIndex).ToString();
                dataTable.Rows[0]["Koordinatlar"] = coordinates;
                dataTable.Rows[0]["Çizilen Alan (m2)"] = area;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

                // Set tooltips for column headers
                PoligonDataGridView.Columns["Polygon ID"].ToolTipText = "Poligona ait özgün ID numarası";
                PoligonDataGridView.Columns["Koordinatlar"].ToolTipText = "Poligonun WKT formatındaki koordinatları";
                PoligonDataGridView.Columns["Çizilen Alan (m2)"].ToolTipText = "Poligonun hesaplanan alanı (metrekare)";
                PoligonDataGridView.Columns["1-2 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 1-2 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["3-4 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 3-4 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["5-7 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 5-7 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["8 USTU KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 8 ve üzeri katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["VILLA MESKEN"].ToolTipText = "Poligon içindeki tahmini Villa mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["AYDINLATMA"].ToolTipText = "Poligon içindeki tahmini Aydınlatma amaçlı oluşacak kullanım oranı (0-100)";
                PoligonDataGridView.Columns["KUCUK_SANAYI"].ToolTipText = "Poligon içindeki tahmini Küçük sanayi tesislerinin oranı (0-100)";
                PoligonDataGridView.Columns["KUCUK_TICARETHANE"].ToolTipText = "Poligon içindeki tahmini Küçük ticari işletmelerin oranı (0-100)";
                PoligonDataGridView.Columns["ORTA_SANAYI"].ToolTipText = "Poligon içindeki tahmini Orta ölçekli sanayi tesislerinin oranı (0-100)";
                PoligonDataGridView.Columns["ORTA_TICARETHANE"].ToolTipText = "Poligon içindeki tahmini Orta ölçekli ticari işletmelerin oranı (0-100)";
                PoligonDataGridView.Columns["TARIMSAL_SULAMA"].ToolTipText = "Poligon içindeki tahmini Tarımsal sulama amaçlı kullanım oranı (0-100)";
                PoligonDataGridView.Columns["Başlangıç Yılı"].ToolTipText = "Poligon için planlanan başlangıç yılı";
                PoligonDataGridView.Columns["Satürasyon Hızı"].ToolTipText = "Poligonun tahmini doygunluğa ulaşma hızı (1-5)";
                PoligonDataGridView.Columns["TAKS"].ToolTipText = "Poligona ait yaklaşık TAKS bilgisi";
                PoligonDataGridView.Columns["Park, Yol, Kaldırım Oranı (%)"].ToolTipText = "Poligon içindeki tahmini Park, yol ve kaldırım alanlarının oranı (0-100)";

            } else if (isSelecting_KentselDonusum == true)
            {
                // Add "Polygon ID" column
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Koordinatlar", typeof(string));
                dataTable.Columns.Add("Çizilen Alan (m2)", typeof(string));
                dataTable.Columns.Add("1-2 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("3-4 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("5-7 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("8 USTU KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("VILLA MESKEN", typeof(string));
                dataTable.Columns.Add("AYDINLATMA", typeof(string));
                dataTable.Columns.Add("KUCUK_SANAYI", typeof(string));
                dataTable.Columns.Add("KUCUK_TICARETHANE", typeof(string));
                dataTable.Columns.Add("ORTA_SANAYI", typeof(string));
                dataTable.Columns.Add("ORTA_TICARETHANE", typeof(string));
                dataTable.Columns.Add("TARIMSAL_SULAMA", typeof(string));
                dataTable.Columns.Add("Başlangıç Yılı", typeof(string));
                dataTable.Columns.Add("Satürasyon Hızı", typeof(string));
                dataTable.Columns.Add("Park, Yol, Kaldırım Oranı (%)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + (layerIndex).ToString();
                dataTable.Rows[0]["Koordinatlar"] = coordinates;
                dataTable.Rows[0]["Çizilen Alan (m2)"] = area;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

                // Set tooltips for column headers
                PoligonDataGridView.Columns["Polygon ID"].ToolTipText = "Poligona ait özgün ID numarası";
                PoligonDataGridView.Columns["Koordinatlar"].ToolTipText = "Poligonun WKT formatındaki koordinatları";
                PoligonDataGridView.Columns["Çizilen Alan (m2)"].ToolTipText = "Poligonun hesaplanan alanı (metrekare)";
                PoligonDataGridView.Columns["1-2 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 1-2 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["3-4 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 3-4 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["5-7 KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 5-7 katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["8 USTU KATLI MESKEN"].ToolTipText = "Poligon içindeki tahmini 8 ve üzeri katlı mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["VILLA MESKEN"].ToolTipText = "Poligon içindeki tahmini Villa mesken yapılarının oranı (0-100)";
                PoligonDataGridView.Columns["AYDINLATMA"].ToolTipText = "Poligon içindeki tahmini Aydınlatma amaçlı oluşacak kullanım oranı (0-100)";
                PoligonDataGridView.Columns["KUCUK_SANAYI"].ToolTipText = "Poligon içindeki tahmini Küçük sanayi tesislerinin oranı (0-100)";
                PoligonDataGridView.Columns["KUCUK_TICARETHANE"].ToolTipText = "Poligon içindeki tahmini Küçük ticari işletmelerin oranı (0-100)";
                PoligonDataGridView.Columns["ORTA_SANAYI"].ToolTipText = "Poligon içindeki tahmini Orta ölçekli sanayi tesislerinin oranı (0-100)";
                PoligonDataGridView.Columns["ORTA_TICARETHANE"].ToolTipText = "Poligon içindeki tahmini Orta ölçekli ticari işletmelerin oranı (0-100)";
                PoligonDataGridView.Columns["TARIMSAL_SULAMA"].ToolTipText = "Poligon içindeki tahmini Tarımsal sulama amaçlı kullanım oranı (0-100)";
                PoligonDataGridView.Columns["Başlangıç Yılı"].ToolTipText = "Poligon için planlanan başlangıç yılı";
                PoligonDataGridView.Columns["Satürasyon Hızı"].ToolTipText = "Poligonun tahmini doygunluğa ulaşma hızı (1-5)";
                PoligonDataGridView.Columns["Park, Yol, Kaldırım Oranı (%)"].ToolTipText = "Poligon içindeki tahmini Park, yol ve kaldırım alanlarının oranı (0-100)";

            }
        }

        private void LoadExcelData()
        {

            // Initialize columnValues based on selection
            if (isSelecting_YUK)
            {
                columnValues = new Dictionary<string, HashSet<string>>
                {
                    { "Tipi", new HashSet<string>() },
                    { "Ortalama Kapladığı Alan (m2)", new HashSet<string>() },
                    { "Tüketim Sınıfı", new HashSet<string>() },
                    { "ENERJILENDIRME_YILI", new HashSet<string>() },
                    { "Kurulu Güç (kW)", new HashSet<string>() },
                    { "Pik Yüklenme (%)", new HashSet<string>() },
                    { "Pik Demant (kW)", new HashSet<string>() }
                };
            }
            else
            {
                MessageBox.Show("Geçersiz seçim: Herhangi bir point load seçilmedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            excelDataRows = new List<Dictionary<string, string>>(); // Initialize the list to store full rows

            try
            {
                using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        // Select the appropriate sheet based on the condition
                        DataTable excelTable;
                        if (isSelecting_YUK)
                        {
                            if (result.Tables.Count < 1)
                            {
                                throw new Exception("Excel dosyasında 'YUK' için gerekli olan ilk sayfa bulunamadı.");
                            }
                            excelTable = result.Tables[0];
                        }
                        else
                        {
                            throw new Exception("Geçersiz seçim: Herhangi bir point load seçilmedi.");
                        }

                        // Log column names for debugging
                        string columnNames = string.Join(", ", excelTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                        Console.WriteLine($"Excel Sheet Columns: {columnNames}");

                        // Validate that all expected columns exist
                        foreach (var expectedColumn in columnValues.Keys)
                        {
                            if (!excelTable.Columns.Contains(expectedColumn))
                            {
                                MessageBox.Show($"Excel dosyasında beklenen sütun bulunamadı: {expectedColumn}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // Process the selected sheet
                        foreach (DataRow row in excelTable.Rows)
                        {
                            var rowData = new Dictionary<string, string>();
                            foreach (DataColumn column in excelTable.Columns)
                            {
                                string columnName = column.ColumnName;
                                string value = row[columnName]?.ToString() ?? string.Empty;
                                rowData[columnName] = value;

                                // Only add to columnValues if the column is defined in columnValues
                                if (columnValues.ContainsKey(columnName) && !string.IsNullOrWhiteSpace(value))
                                {
                                    columnValues[columnName].Add(value);
                                }
                            }
                            excelDataRows.Add(rowData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel dosyasını okurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> dropdownColumns = new List<string>();

        private void SetupDropdownColumns(Dictionary<string, HashSet<string>> columnValues)
        {
            if (isSelecting_YUK)
            {
                manualColumns = new HashSet<string>
                {
                    "Polygon ID",
                    "Koordinatlar",
                    "Çizilen Alan (m2)"
                };
            }

            var currentValues = new Dictionary<string, object>();
            foreach (DataGridViewRow row in PoligonDataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewColumn column in PoligonDataGridView.Columns)
                    {
                        if (!manualColumns.Contains(column.Name))
                        {
                            currentValues[column.Name] = row.Cells[column.Name].Value;
                        }
                    }
                    break;
                }
            }

            var columnsToReplace = new List<(int Index, string Name)>();
            foreach (DataGridViewColumn column in PoligonDataGridView.Columns)
            {
                if (!manualColumns.Contains(column.Name))
                {
                    columnsToReplace.Add((column.Index, column.Name));
                }
            }

            dropdownColumns.Clear();

            foreach (var (index, name) in columnsToReplace)
            {
                if (!columnValues.ContainsKey(name))
                    continue;

                var comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = name,
                    HeaderText = name,
                    DataPropertyName = name,
                    DataSource = columnValues[name].OrderBy(x => x).ToList(),
                    ValueType = typeof(string),
                    FlatStyle = FlatStyle.Standard
                };

                PoligonDataGridView.Columns.Remove(name);
                PoligonDataGridView.Columns.Insert(index, comboBoxColumn);

                dropdownColumns.Add(name);
            }

            // Reapply stored values, allowing custom values to persist
            foreach (DataGridViewRow row in PoligonDataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (var kvp in currentValues)
                    {
                        var cell = row.Cells[kvp.Key];
                        if (cell is DataGridViewComboBoxCell comboBoxCell && kvp.Value != null)
                        {
                            cell.Value = kvp.Value; // Restore original value, including custom ones
                        }
                    }
                    break;
                }
            }
        }

        // Dictionary to store adjusted horizontal values (Year -> (Category -> Adjusted Value))
        // e.g., { 2027: { "TİCARETHANE": 3597070.999 - difference, "SANAYİ": ... } }
        private static readonly Dictionary<int, Dictionary<string, double>> AdjustedHorizontalValues = new Dictionary<int, Dictionary<string, double>>();

        private bool ValidatePolygonData()
        {
            try
            {
                // Validation for isSelecting_YGA or isSelecting_KentselDonusum
                if (isSelecting_YGA || isSelecting_KentselDonusum)
                {
                    // Columns to validate for percentage sum
                    string[] percentageColumns = new string[]
                    {
                        "1-2 KATLI MESKEN",
                        "3-4 KATLI MESKEN",
                        "5-7 KATLI MESKEN",
                        "8 USTU KATLI MESKEN",
                        "VILLA MESKEN",
                        "AYDINLATMA",
                        "KUCUK_SANAYI",
                        "KUCUK_TICARETHANE",
                        "ORTA_SANAYI",
                        "ORTA_TICARETHANE",
                        "TARIMSAL_SULAMA",
                        "Park, Yol, Kaldırım Oranı (%)"
                    };

                    // Additional columns to validate
                    string[] additionalColumns = { "Başlangıç Yılı", "Satürasyon Hızı" };
                    if (isSelecting_YGA)
                    {
                        additionalColumns = additionalColumns.Concat(new[] { "TAKS" }).ToArray();
                    }

                    // Check if all required columns exist
                    foreach (string column in percentageColumns.Concat(additionalColumns))
                    {
                        if (dataTable.Columns.Contains(column)) continue;
                        MessageBox.Show($"Hata: '{column}' sütunu bulunamadı.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    double total = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Validate percentage sum
                        foreach (string column in percentageColumns)
                        {
                            // Check if the column exists and the value is not null or empty
                            if (!string.IsNullOrEmpty(row[column]?.ToString()))
                            {
                                // Try to parse the value as a double
                                if (double.TryParse(row[column].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                                {
                                    total += value;
                                }
                                else
                                {
                                    MessageBox.Show($"Hata: '{column}' sütununda geçersiz bir değer var.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                            }
                        }

                        // Validate Başlangıç Yılı
                        string startYearStr = row["Başlangıç Yılı"]?.ToString();
                        if (string.IsNullOrWhiteSpace(startYearStr))
                        {
                            MessageBox.Show("Hata: 'Başlangıç Yılı' sütunu boş olamaz.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (!int.TryParse(startYearStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int startYear) || startYear < 2025 || startYear > 2075)
                        {
                            MessageBox.Show($"Hata: 'Başlangıç Yılı' 2025 ile 2075 arasında olmalıdır. Girilen: {startYearStr}", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Validate Satürasyon Hızı
                        string saturationSpeedStr = row["Satürasyon Hızı"]?.ToString();
                        if (string.IsNullOrWhiteSpace(saturationSpeedStr))
                        {
                            MessageBox.Show("Hata: 'Satürasyon Hızı' sütunu boş olamaz.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (!double.TryParse(saturationSpeedStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double saturationSpeed) || saturationSpeed < 1 || saturationSpeed > 5)
                        {
                            MessageBox.Show($"Hata: 'Satürasyon Hızı' 1 ile 5 arasında olmalıdır. Girilen: {saturationSpeedStr}", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Validate TAKS when isSelecting_YGA is true
                        if (isSelecting_YGA)
                        {
                            string taksStr = row["TAKS"]?.ToString();
                            if (string.IsNullOrWhiteSpace(taksStr))
                            {
                                MessageBox.Show("Hata: 'TAKS' sütunu boş olamaz.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                    }

                    // Check if total is approximately 100 (allowing for small floating-point errors)
                    if (Math.Abs(total - 100) >= 0.1)
                    {
                        return false; // Error message handled in button click
                    }
                }

                // Validation for isSelecting_YUK
                if (isSelecting_YUK)
                {
                    // Required columns for YUK
                    string[] columnsToValidate = { "Kurulu Güç (kW)", "Pik Yüklenme (%)", "Pik Demant (kW)", "ENERJILENDIRME_YILI" };
                    string categoryColumn = isSelecting_YUK ? "Tüketim Sınıfı" : "ABONE_GRUBU";
                    columnsToValidate = columnsToValidate.Concat(new[] { categoryColumn }).ToArray();

                    // Check if all required columns exist
                    foreach (string column in columnsToValidate)
                    {
                        if (!dataTable.Columns.Contains(column))
                        {
                            MessageBox.Show($"Hata: '{column}' sütunu bulunamadı.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }


                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Validate Başlangıç Yılı
                        string startYearStr = row["ENERJILENDIRME_YILI"]?.ToString();

                        if (string.IsNullOrWhiteSpace(startYearStr))
                        {
                            MessageBox.Show("Hata: 'ENERJILENDIRME_YILI' sütunu boş olamaz.", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (!int.TryParse(startYearStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int startYear) || startYear < 2025 || startYear > 2075)
                        {
                            MessageBox.Show($"Hata: 'ENERJILENDIRME_YILI' 2025 ile 2075 arasında olmalıdır. Girilen: {startYearStr}", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        string kuruluGucStr = row["Kurulu Güç (kW)"]?.ToString();


                        if (!int.TryParse(kuruluGucStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int kuruluGuc) || kuruluGuc > 10000)
                        {
                            MessageBox.Show($"Hata: 'Kurulu Güç (kW)' değeri en fazla 10,000 kW olabilir. Girilen: {kuruluGucStr}", 
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }


                    }


                    // Iterate over each row in the DataTable
                    for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                    {
                        DataRow row = dataTable.Rows[rowIndex];

                        // Get values for the required columns
                        string powerValueStr = row["Kurulu Güç (kW)"]?.ToString();
                        string peakLoadValueStr = row["Pik Yüklenme (%)"]?.ToString();
                        string peakDemandValueStr = row["Pik Demant (kW)"]?.ToString();
                        string energizationYearStr = row["ENERJILENDIRME_YILI"]?.ToString();
                        string categoryValue = row[categoryColumn]?.ToString();

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(powerValueStr) ||
                            string.IsNullOrWhiteSpace(peakLoadValueStr) ||
                            string.IsNullOrWhiteSpace(peakDemandValueStr) ||
                            string.IsNullOrWhiteSpace(energizationYearStr) ||
                            string.IsNullOrWhiteSpace(categoryValue))
                        {
                            MessageBox.Show(
                                $"Satır {rowIndex + 1}: Tüm alanlar doldurulmalıdır: " +
                                $"Kurulu Güç='{powerValueStr}', Pik Yüklenme='{peakLoadValueStr}', Pik Demant='{peakDemandValueStr}', " +
                                $"ENERJILENDIRME_YILI='{energizationYearStr}', {categoryColumn}='{categoryValue}'",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Parse values
                        if (!double.TryParse(powerValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double powerValue) ||
                            !double.TryParse(peakLoadValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double peakLoadValue) ||
                            !double.TryParse(peakDemandValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double peakDemandValue) ||
                            !int.TryParse(energizationYearStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int energizationYear))
                        {
                            MessageBox.Show(
                                $"Satır {rowIndex + 1}: Geçersiz değerler: " +
                                $"Kurulu Güç='{powerValueStr}', Pik Yüklenme='{peakLoadValueStr}', Pik Demant='{peakDemandValueStr}', " +
                                $"ENERJILENDIRME_YILI='{energizationYearStr}'",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Validate the power equation
                        double expectedPeakDemand = powerValue * peakLoadValue;
                        const double tolerance = 1;
                        if (Math.Abs(expectedPeakDemand - peakDemandValue) > tolerance)
                        {
                            MessageBox.Show(
                                $"Satır {rowIndex + 1}: 'Kurulu Güç (kW)' * 'Pik Yüklenme (%)' = 'Pik Demant (kW)' eşitliği sağlanmıyor.\n" +
                                $"Hesaplanan: {expectedPeakDemand:F2} kW, Girilen: {peakDemandValue:F2} kW",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Determine the category column in Excel
                        string excelColumn;
                        string categoryUpper = categoryValue.ToUpper();
                        if (categoryUpper == "TICARETHANE")
                        {
                            excelColumn = "TICARETHANE_HORIZONTAL";
                        }
                        else if (categoryUpper == "SANAYI")
                        {
                            excelColumn = "SANAYI_HORIZONTAL";
                        }
                        else if (categoryUpper == "TARIMSAL_SULAMA")
                        {
                            excelColumn = "TARIMSAL_SULAMA_HORIZONTAL";
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Satır {rowIndex + 1}: {categoryColumn} yalnızca 'TICARETHANE', 'SANAYI' veya 'TARIMSAL_SULAMA'" +
                                $" olabilir. Girilen: {categoryValue}",
                                "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        if(modül_formu.point_load_konsolidasyonu == true)
                        {
                            // Construct the path to the Excel file
                            string hor_ver_path = System.IO.Path.Combine(modül_formu.ana_menu_form_objesi.userRootPath,
                                (string)modül_formu.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                                (string)modül_formu.ana_menu_form_objesi.config.İl,
                                (string)modül_formu.ana_menu_form_objesi.config.İlçe,
                                (string)modül_formu.ana_menu_form_objesi.config.proje_ismi,
                                (string)modül_formu.ana_menu_form_objesi.config.ELF.hor_ver_dosyası).Replace('/', '\\');

                            if (!File.Exists(hor_ver_path))
                            {
                                MessageBox.Show($"Hata: Excel dosyası bulunamadı: {hor_ver_path}", "Dosya Hatası", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }


                            // Read the Excel file
                            using (var package = new ExcelPackage(new FileInfo(hor_ver_path)))
                            {
                                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                                int rowCount = worksheet.Dimension.Rows;
                                int yearCol = 2; // "YIL" is the second column
                                int horizontalCol = -1;

                                // Find the column index for TICARETHANE_HORIZONTAL, SANAYI_HORIZONTAL or TARIMSAL_SULAMA_HORIZONTAL
                                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                                {
                                    if (worksheet.Cells[1, col].Text == excelColumn)
                                    {
                                        horizontalCol = col;
                                        break;
                                    }
                                }

                                if (horizontalCol == -1)
                                {
                                    MessageBox.Show($"Hata: '{excelColumn}' sütunu Excel dosyasında bulunamadı.", "Doğrulama Hatası", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                // Validate for years t0, t+1, and t+2
                                for (int yearOffset = 0; yearOffset <= 2; yearOffset++)
                                {
                                    int targetYear = energizationYear + yearOffset;
                                    double multiplier = (yearOffset == 0) ? 0.5 : 0.25;
                                    double calculatedValue = peakDemandValue / 2.5 * 8760 * multiplier;
                                    double thresholdMultiplier = 0.9; // 90%

                                    // Find the row for the target year
                                    double horizontalValue = 0;
                                    bool yearFound = false;
                                    for (int excelRow = 2; excelRow <= rowCount; excelRow++) // Renamed 'row' to 'excelRow'
                                    {
                                        string yearStr = worksheet.Cells[excelRow, yearCol].Text?.Replace(",", "");
                                        if (int.TryParse(yearStr, out int excelYear) && excelYear == targetYear)
                                        {
                                            string horizontalStr = worksheet.Cells[excelRow, horizontalCol].Text?.Replace(",", "");
                                            if (double.TryParse(horizontalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out horizontalValue))
                                            {
                                                yearFound = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!yearFound)
                                    {
                                        MessageBox.Show($"Hata: {targetYear} yılı Excel dosyasında bulunamadı.", "Doğrulama Hatası", 
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }

                                    // Adjust horizontal value if previous polygons have modified it
                                    double adjustedHorizontalValue = horizontalValue;
                                    if (AdjustedHorizontalValues.ContainsKey(targetYear) && AdjustedHorizontalValues[targetYear].ContainsKey(categoryUpper))
                                    {
                                        adjustedHorizontalValue = AdjustedHorizontalValues[targetYear][categoryUpper];
                                    }

                                    double threshold = adjustedHorizontalValue * thresholdMultiplier;
                                    if (calculatedValue > threshold)
                                    {
                                        MessageBox.Show(
                                            $"Satır {rowIndex + 1}: {targetYear} yılı için {excelColumn} ELF tahmininden gelen " +
                                            $"kWh tüketim sınırını aşıyor.\n" +
                                            $"Hesaplanan Değer: {calculatedValue:F0} kWh," +
                                            $" İzin Verilen Maksimum ({targetYear} verisinin ({adjustedHorizontalValue:F0})" +
                                            $" %90'ı) = {threshold:F0} kWh",
                                            "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                }

                                // If all validations pass, calculate and store the differences
                                for (int yearOffset = 0; yearOffset <= 2; yearOffset++)
                                {
                                    int targetYear = energizationYear + yearOffset;
                                    double multiplier = (yearOffset == 0) ? 0.5 : 0.25;
                                    double calculatedValue = peakDemandValue / 2.5 * 8760 * multiplier;

                                    // Find the original horizontal value again for difference calculation
                                    double horizontalValue = 0;
                                    for (int excelRow = 2; excelRow <= rowCount; excelRow++) // Renamed 'row' to 'excelRow'
                                    {
                                        string yearStr = worksheet.Cells[excelRow, yearCol].Text?.Replace(",", "");
                                        if (int.TryParse(yearStr, out int excelYear) && excelYear == targetYear)
                                        {
                                            string horizontalStr = worksheet.Cells[excelRow, horizontalCol].Text?.Replace(",", "");
                                            if (double.TryParse(horizontalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out horizontalValue))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    // Adjust the value (subtract the calculated value)
                                    double adjustedValue = horizontalValue;
                                    if (AdjustedHorizontalValues.ContainsKey(targetYear) && AdjustedHorizontalValues[targetYear].ContainsKey(categoryUpper))
                                    {
                                        adjustedValue = AdjustedHorizontalValues[targetYear][categoryUpper];
                                    }
                                    adjustedValue -= calculatedValue;

                                    // Store the adjusted value in the dictionary
                                    if (!AdjustedHorizontalValues.ContainsKey(targetYear))
                                    {
                                        AdjustedHorizontalValues[targetYear] = new Dictionary<string, double>();
                                    }
                                    AdjustedHorizontalValues[targetYear][categoryUpper] = adjustedValue;
                                }
                            }

                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Doğrulama sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void buton_poligon_ozellik_Click(object sender, EventArgs e)
        {

            if (ValidatePolygonData())
            {
                isKaydetClicked = true;
                this.Close();
            }
            else
            {
                if (isSelecting_YGA || isSelecting_KentselDonusum)
                {
                    MessageBox.Show("Hata: İmar tiplerinin toplamı 100(%) olmalıdır!\n\n" +
                        "İlgili imar tipleri şunlardır:\n\n" +
                        "1-2 KATLI MESKEN\n" +
                        "3-4 KATLI MESKEN\n" +
                        "5-7 KATLI MESKEN\n" +
                        "8 USTU KATLI MESKEN\n" +
                        "VILLA MESKEN\n" +
                        "AYDINLATMA\n" +
                        "KUCUK_SANAYI\n" +
                        "KUCUK_TICARETHANE\n" +
                        "ORTA_SANAYI\n" +
                        "ORTA_TICARETHANE\n" +
                        "TARIMSAL_SULAMA\n" +
                        "Park, Yol, Kaldırım Oranı (%)", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buton_yük_tipleri_Click(object sender, EventArgs e)
        {

            if (isSelecting_YUK)
            {
                yük_bilgi_formu_objesi = new Nokta_Yuk_Bilgi_Formu(excelFilePath,true);
            }


            yük_bilgi_formu_objesi.Owner = this;
            yük_bilgi_formu_objesi.ShowDialog();
            yük_bilgi_formu_objesi.BringToFront();
            yük_bilgi_formu_objesi.Focus();

             // Reload Excel data and update dropdowns
             LoadExcelData();
             if (columnValues == null)
             {
                 MessageBox.Show("Excel dosyası okunamadı, dropdown listeleri doldurulamıyor. Lütfen Excel dosyasını kapatıp tekrar deneyin.",
                     "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }

             SetupDropdownColumns(columnValues);

             // Check if yukler or musaade changed after the dialog closes
             if (yük_bilgi_formu_objesi.is_yukler_changed)
             {
                 yük_bilgi_formu_objesi.is_yukler_changed = false;
                 yük_bilgi_formu_objesi.yuk_select = true;
             }

        }

        private void PoligonDataGridView_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {

            if (isSelecting_YUK == true)
            {
                // Check if the changed cell is in the "Tipi" column
                if (e.ColumnIndex == PoligonDataGridView.Columns["Tipi"].Index && e.RowIndex >= 0)
                {
                    string selectedTipi = PoligonDataGridView.Rows[e.RowIndex].Cells["Tipi"].Value?.ToString();
                    if (!string.IsNullOrEmpty(selectedTipi))
                    {
                        // Find the row in excelDataRows that matches the selected Tipi
                        var matchingRow = excelDataRows.FirstOrDefault(row => row["Tipi"] == selectedTipi);
                        if (matchingRow != null)
                        {
                            // Update the other columns with the corresponding values
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Ortalama Kapladığı Alan (m2)"].Value = matchingRow["Ortalama Kapladığı Alan (m2)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Tüketim Sınıfı"].Value = matchingRow["Tüketim Sınıfı"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["ENERJILENDIRME_YILI"].Value = matchingRow["ENERJILENDIRME_YILI"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Kurulu Güç (kW)"].Value = matchingRow["Kurulu Güç (kW)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Yüklenme (%)"].Value = matchingRow["Pik Yüklenme (%)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Demant (kW)"].Value = matchingRow["Pik Demant (kW)"];
                        }
                    }
                }

            } 

        }

        private void Poligon_Özellik_Tanımlama_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Only update is_poligon_saved if the form wasn't closed via the Kaydet button.
            if (!isKaydetClicked)
            {
                is_poligon_saved = false;
            }
            else
            {
                isSelecting_YUK = false;
                isSelecting_YGA = false;
                isSelecting_KentselDonusum = false;
            }
        }

        private void PoligonDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

                comboBox.Leave -= ComboBox_Leave;
                comboBox.TextChanged -= ComboBox_TextChanged;

                comboBox.Leave += ComboBox_Leave;
                comboBox.TextChanged += ComboBox_TextChanged;
            }

            if (e.Control is System.Windows.Forms.TextBox textBox)
            {
                string columnName = PoligonDataGridView.Columns[PoligonDataGridView.CurrentCell.ColumnIndex].Name;
                Console.WriteLine($"EditingControlShowing: ColumnName={columnName}, isSelecting_YGA={isSelecting_YGA}");

                // Remove the previous KeyPress handler if it exists
                if (_currentKeyPressHandler != null)
                {
                    textBox.KeyPress -= _currentKeyPressHandler;
                }

                // Create a new lambda delegate with the current columnName
                _currentKeyPressHandler = (s, ev) => TextBox_KeyPress_NumericWithDecimalForTAKS(s, ev, columnName);

                // Add the new KeyPress handler
                textBox.KeyPress += _currentKeyPressHandler;
            }
        }

        private string currentComboBoxValue = null;

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                currentComboBoxValue = comboBox.Text;
                Console.WriteLine($"ComboBox_TextChanged: Value={currentComboBoxValue}");
            }
        }

        private void ComboBox_Leave(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                try
                {
                    currentComboBoxValue = comboBox.Text;
                    Console.WriteLine($"ComboBox_Leave: Value={currentComboBoxValue}");

                    var cell = PoligonDataGridView.CurrentCell;
                    if (cell != null && !string.IsNullOrEmpty(currentComboBoxValue))
                    {
                        int rowIndex = PoligonDataGridView.CurrentCell.RowIndex;
                        string columnName = PoligonDataGridView.Columns[cell.ColumnIndex].Name;

                        // Add the custom value to the column's DataSource to make it valid
                        if (dropdownColumns.Contains(columnName) && columnValues.ContainsKey(columnName))
                        {
                            if (!columnValues[columnName].Contains(currentComboBoxValue))
                            {
                                columnValues[columnName].Add(currentComboBoxValue);
                                Console.WriteLine($"ComboBox_Leave: Added {currentComboBoxValue} to DataSource for {columnName}");

                                // Update the DataGridViewComboBoxColumn's DataSource
                                if (PoligonDataGridView.Columns[columnName] is DataGridViewComboBoxColumn comboBoxColumn)
                                {
                                    comboBoxColumn.DataSource = null; // Temporarily clear to avoid binding issues
                                    comboBoxColumn.DataSource = columnValues[columnName].OrderBy(x => x).ToList();
                                    Console.WriteLine($"ComboBox_Leave: Updated DataSource for {columnName}");
                                }
                            }
                        }

                        // Force the cell to accept the value
                        cell.Value = currentComboBoxValue;
                        Console.WriteLine($"ComboBox_Leave: Forced cell value to {currentComboBoxValue}");

                        // Update the DataTable
                        dataTable.Rows[rowIndex][columnName] = currentComboBoxValue;
                        Console.WriteLine($"ComboBox_Leave: Updated DataTable with {currentComboBoxValue}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ComboBox_Leave Error: {ex.Message}");
                    MessageBox.Show($"Hata: {ex.Message}", "Düzenleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TextBox_KeyPress_NumericWithDecimalForTAKS(object sender, KeyPressEventArgs e, string columnName)
        {
            // Get the culture-specific decimal separator
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            Console.WriteLine($"KeyPress: ColumnName={columnName}, KeyChar={e.KeyChar}, DecimalSeparator={decimalSeparator}, isSelecting_YGA={isSelecting_YGA}");

            // Allow backspace and control characters (e.g., Enter, Tab)
            if (e.KeyChar == (char)Keys.Back || char.IsControl(e.KeyChar))
            {
                Console.WriteLine("KeyPress: Allowed backspace or control character.");
                return;
            }

            // Allow digits (0-9)
            if (char.IsDigit(e.KeyChar))
            {
                Console.WriteLine("KeyPress: Allowed digit.");
                return;
            }

            // Allow both '.' and the culture-specific decimal separator for the "TAKS" column
            if (columnName == "TAKS" && isSelecting_YGA && (e.KeyChar == '.' || e.KeyChar.ToString() == decimalSeparator))
            {
                // Check if a decimal separator already exists in the TextBox
                System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
                if (textBox != null && !textBox.Text.Contains(decimalSeparator) && !textBox.Text.Contains("."))
                {
                    Console.WriteLine("KeyPress: Decimal separator allowed.");
                    return; // Allow the decimal separator
                }
                else
                {
                    Console.WriteLine("KeyPress: Decimal separator blocked: Already exists or TextBox is null.");
                }
            }

            // Block all other characters (including A-Z and other symbols)
            e.Handled = true;
            Console.WriteLine("KeyPress: Character blocked.");
        }

        private void PoligonDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                // Existing TAKS validation and Başlangıç Yılı validation
                if (isSelecting_YGA || isSelecting_KentselDonusum)
                {
                    // TAKS validation
                    if (e.ColumnIndex == PoligonDataGridView.Columns["TAKS"]?.Index)
                    {
                        string input = e.FormattedValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                            input = input.Replace(decimalSeparator, ".");

                            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double taksValue) || taksValue < 0.1 || taksValue > 1)
                            {
                                e.Cancel = true;
                                MessageBox.Show("Hata: TAKS değeri 0.1 ile 1 arasında olmalıdır!", "Geçersiz Değer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // Satürasyon Hızı validation
                    if (e.ColumnIndex == PoligonDataGridView.Columns["Satürasyon Hızı"]?.Index)
                    {
                        string input = e.FormattedValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                            input = input.Replace(decimalSeparator, ".");

                            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double saturationSpeed) || saturationSpeed < 1 || saturationSpeed > 5)
                            {
                                e.Cancel = true;
                                MessageBox.Show("Hata: Satürasyon Hızı 1 ile 5 arasında olmalıdır!", "Geçersiz Değer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // Başlangıç Yılı validation
                    if (e.ColumnIndex == PoligonDataGridView.Columns["Başlangıç Yılı"]?.Index)
                    {
                        string input = e.FormattedValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int yearValue) || yearValue < 2025 || yearValue > 2075)
                            {
                                e.Cancel = true;
                                MessageBox.Show("Hata: Başlangıç Yılı 2025 ile 2075 arasında olmalıdır!", "Geçersiz Değer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                // Existing dropdown column validation
                string columnName = PoligonDataGridView.Columns[e.ColumnIndex].Name;
                if (dropdownColumns.Contains(columnName))
                {
                    string input = e.FormattedValue?.ToString();
                    Console.WriteLine($"CellValidating: Column={columnName}, Input={input}");
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        e.Cancel = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CellValidating Error: {ex.Message}");
                e.Cancel = true;
                MessageBox.Show($"Hata: {ex.Message}", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PoligonDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                string columnName = PoligonDataGridView.Columns[e.ColumnIndex].Name;
                var cell = PoligonDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                string editedValue = currentComboBoxValue ?? cell.Value?.ToString();

                if (!string.IsNullOrEmpty(editedValue))
                {
                    Console.WriteLine($"CellEndEdit: Column={columnName}, EditedValue={editedValue}, CellValueBefore={cell.Value}");

                    // Save to the DataTable
                    dataTable.Rows[e.RowIndex][columnName] = editedValue;

                    // Force the DataGridView cell to retain the value
                    if (dropdownColumns.Contains(columnName))
                    {
                        cell.Value = editedValue;
                        Console.WriteLine($"CellEndEdit: Forced DataGridView value to {editedValue}, CellValueAfter={cell.Value}");
                    }
                }

                currentComboBoxValue = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CellEndEdit Error: {ex.Message}");
                MessageBox.Show($"Hata: {ex.Message}", "Düzenleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PoligonDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                string columnName = PoligonDataGridView.Columns[e.ColumnIndex].Name;
                var cell = PoligonDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (dropdownColumns.Contains(columnName))
                {
                    string dataTableValue = dataTable.Rows[e.RowIndex][columnName]?.ToString();
                    Console.WriteLine($"CellEnter: Column={columnName}, DataTableValue={dataTableValue}, CellValue={cell.Value}");
                    if (!string.IsNullOrEmpty(dataTableValue))
                    {
                        cell.Value = dataTableValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CellEnter Error: {ex.Message}");
                MessageBox.Show($"Hata: {ex.Message}", "Hücre Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}