using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using GMap.NET;

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

        private bool isSelecting_YGA;
        private bool isSelecting_YUK;
        private bool isSelecting_Musaade;

        public bool is_poligon_saved = true;
        private bool isKaydetClicked = false;

        // Add a public property to access the DataTable
        public DataTable PolygonDataTable => dataTable;

        public Poligon_Özellik_Tanımlama(bool isSelectingYUK, bool isSelectingYGA, bool isSelectingMusaade,
            List<PointLatLng> polygonPoints)
        {
            InitializeComponent();
            modül_formu = new ModülFormu();
            cbsFormu = new CBS(modül_formu);

            // Set the flags before calling SetupDataGridView
            isSelecting_YUK = isSelectingYUK;
            isSelecting_YGA = isSelectingYGA;
            isSelecting_Musaade = isSelectingMusaade;

            excelFilePath = Path.Combine(modül_formu.ana_menu_form_objesi.userRootPath,
                (string)modül_formu.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                (string)modül_formu.ana_menu_form_objesi.config.İl,
                (string)modül_formu.ana_menu_form_objesi.config.İlçe,
                (string)modül_formu.ana_menu_form_objesi.config.Point_Load_Musaade).Replace('/', '\\');

            // Determine layer index
            layerIndex = FindFirstFreeLayerIndex();
            if (layerIndex < 0)
            {
                MessageBox.Show("Çizilen poligon geçersiz bir katmana ait!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                layerIndex = 0; // Default to 0 if not found
            }

            // Load Excel data and set up DataGridView
            SetupDataGridView(polygonPoints);

        }

        private int FindFirstFreeLayerIndex()
        {
            for (int i = 0; i < 15; i++)
            {
                // If all four overlays at index i are null, that means it’s free
                if (cbsFormu.tüm_katmanlar_array_imar[i] == null && cbsFormu.tüm_katmanlar_array_yuk[i] == null)
                {
                    return i;
                }
            }
            return -1; // none free
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
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + layerIndex;
                dataTable.Rows[0]["Koordinatlar"] = coordinates;
                dataTable.Rows[0]["Çizilen Alan (m2)"] = area;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

                // Populate dropdowns and store Excel data
                LoadExcelData();
                SetupDropdownColumns(columnValues);

            }
            else if (isSelecting_Musaade == true)
            {
                // Add columns
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Tipi", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ABONE_GRUBU", typeof(string));
                dataTable.Columns.Add("ENERJI_MUSAADE_ENERJILENDIRME_YILI", typeof(string));
                dataTable.Columns.Add("Kurulu Güç (kW)", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant (kW)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + layerIndex;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

                // Populate dropdowns and store Excel data
                LoadExcelData();
                SetupDropdownColumns(columnValues);

            }
            else if (isSelecting_YGA == true)
            {
                // Add "Polygon ID" column
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Koordinatlar", typeof(string));
                dataTable.Columns.Add("Çizilen Alan (m2)", typeof(string));
                dataTable.Columns.Add("1-2 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("3-4 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("5-7 KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("8 USTU KATLI MESKEN", typeof(string));
                dataTable.Columns.Add("AYDINLATMA", typeof(string));
                dataTable.Columns.Add("BUYUK SANAYI", typeof(string));
                dataTable.Columns.Add("BUYUK TICARETHANE", typeof(string));
                dataTable.Columns.Add("KUCUK SANAYI", typeof(string));
                dataTable.Columns.Add("KUCUK TICARETHANE", typeof(string));
                dataTable.Columns.Add("ORTA SANAYI", typeof(string));
                dataTable.Columns.Add("ORTA TICARETHANE", typeof(string));
                dataTable.Columns.Add("TARIMSAL SULAMA", typeof(string));
                dataTable.Columns.Add("Başlangıç Yılı", typeof(string));
                dataTable.Columns.Add("Satürasyon Hızı", typeof(string));
                dataTable.Columns.Add("Park, Yol, Kaldırım Oranı (%)", typeof(string));

                // Add a single row
                dataTable.Rows.Add(dataTable.NewRow());

                // Set "Polygon ID" value
                dataTable.Rows[0]["Polygon ID"] = "Polygon_" + layerIndex;
                dataTable.Rows[0]["Koordinatlar"] = coordinates;
                dataTable.Rows[0]["Çizilen Alan (m2)"] = area;

                // Bind DataTable to PoligonDataGridView
                PoligonDataGridView.DataSource = dataTable;
                PoligonDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                PoligonDataGridView.AllowUserToAddRows = false; // Prevent adding rows
                PoligonDataGridView.AllowUserToDeleteRows = false; // Prevent deleting rows
                PoligonDataGridView.ReadOnly = false; // Allow editing dropdowns
                PoligonDataGridView.AllowUserToOrderColumns = false; // Prevent column reordering

            }
        }

        private void LoadExcelData()
        {
            if (isSelecting_YUK)
            {
                columnValues = new Dictionary<string, HashSet<string>>
                {
                    { "Tipi", new HashSet<string>() },
                    { "Ortalama Kapladığı Alan (m2)", new HashSet<string>() },
                    { "Tüketim Sınıfı", new HashSet<string>() },
                    { "Kurulu Güç (kW)", new HashSet<string>() },
                    { "Pik Yüklenme (%)", new HashSet<string>() },
                    { "Pik Demant", new HashSet<string>() }
                };
                        }
                        else if (isSelecting_Musaade)
                        {
                            columnValues = new Dictionary<string, HashSet<string>>
                {
                    { "Tipi", new HashSet<string>() },
                    { "ENERJI_MUSAADE_ABONE_GRUBU", new HashSet<string>() },
                    { "ENERJI_MUSAADE_ENERJILENDIRME_YILI", new HashSet<string>() },
                    { "Kurulu Güç (kW)", new HashSet<string>() },
                    { "Pik Yüklenme (%)", new HashSet<string>() },
                    { "Pik Demant (kW)", new HashSet<string>() }
                };
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
                            // Read the first sheet (index 0)
                            if (result.Tables.Count < 1)
                            {
                                throw new Exception("Excel dosyasında 'YUK' için gerekli olan ilk sayfa bulunamadı.");
                            }
                            excelTable = result.Tables[0];
                        }
                        else if (isSelecting_Musaade)
                        {
                            // Read the second sheet (index 1)
                            if (result.Tables.Count < 2)
                            {
                                throw new Exception("Excel dosyasında 'Musaade' için gerekli olan ikinci sayfa bulunamadı.");
                            }
                            excelTable = result.Tables[1];
                        }
                        else
                        {
                            throw new Exception("Geçersiz seçim: Ne YUK ne de Musaade seçildi.");
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

                                // Also populate columnValues for dropdowns
                                if (!string.IsNullOrWhiteSpace(value))
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

        HashSet<string> manualColumns;

        private void SetupDropdownColumns(Dictionary<string, HashSet<string>> columnValues)
        {

            if (isSelecting_YUK)
            {
                // Define the columns that should remain manually defined.
                    manualColumns = new HashSet<string>
                {
                    "Polygon ID",
                    "Koordinatlar",
                    "Çizilen Alan (m2)"
                };

            } else if (isSelecting_Musaade)
            {
                // Define the columns that should remain manually defined.
                manualColumns = new HashSet<string>
                {
                    "Polygon ID"
                };
            }


            // Store current cell values for dropdown columns (from the first non-new row).
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
                    break; // Only capture values from one row.
                }
            }

            // Identify columns to be replaced with dropdowns.
            var columnsToReplace = new List<(int Index, string Name)>();
            foreach (DataGridViewColumn column in PoligonDataGridView.Columns)
            {
                if (!manualColumns.Contains(column.Name))
                {
                    columnsToReplace.Add((column.Index, column.Name));
                }
            }

            // Replace each target column with a DataGridViewComboBoxColumn.
            foreach (var (index, name) in columnsToReplace)
            {
                // Ensure there is a valid set of values for the current column.
                if (!columnValues.ContainsKey(name))
                    continue;

                var comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = name,
                    HeaderText = name,
                    DataPropertyName = name,
                    DataSource = columnValues[name].OrderBy(x => x).ToList(), // Sorted for better UX
                    ValueType = typeof(string),
                    FlatStyle = FlatStyle.Standard
                };

                // Remove the existing column and insert the new dropdown column.
                PoligonDataGridView.Columns.Remove(name);
                PoligonDataGridView.Columns.Insert(index, comboBoxColumn);
            }

            // Reapply stored values if they are still valid.
            foreach (DataGridViewRow row in PoligonDataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (var kvp in currentValues)
                    {
                        var cell = row.Cells[kvp.Key];
                        if (cell is DataGridViewComboBoxCell comboBoxCell && kvp.Value != null)
                        {
                            // Allow the original value even if it's not in the dropdown list
                            cell.Value = kvp.Value; // Restore original value
                        }
                    }
                    break; // Only process the first non-new row.
                }
            }
        }

        private void buton_poligon_ozellik_Click(object sender, EventArgs e)
        {
            isKaydetClicked = true;
            this.Close();
        }

        private void buton_yük_tipleri_Click(object sender, EventArgs e)
        {
            yük_bilgi_formu_objesi = new Nokta_Yuk_Bilgi_Formu(excelFilePath);
            yük_bilgi_formu_objesi.Owner = this;
            yük_bilgi_formu_objesi.ShowDialog();
            yük_bilgi_formu_objesi.BringToFront();
            yük_bilgi_formu_objesi.Focus();

            // Reload Excel data and update dropdowns
            LoadExcelData();
            SetupDropdownColumns(columnValues);

            // Check if yukler changed after the dialog closes
            if (yük_bilgi_formu_objesi.is_yukler_changed)
            {
                yük_bilgi_formu_objesi.is_yukler_changed = false;
                yük_bilgi_formu_objesi.yuk_select = true;

            } else if (yük_bilgi_formu_objesi.is_musaade_changed)
            {
                yük_bilgi_formu_objesi.is_musaade_changed = false;
                yük_bilgi_formu_objesi.musaade_select = true;
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
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Kurulu Güç (kW)"].Value = matchingRow["Kurulu Güç (kW)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Yüklenme (%)"].Value = matchingRow["Pik Yüklenme (%)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Demant (kW)"].Value = matchingRow["Pik Demant (kW)"];
                        }
                    }
                }

            } else if (isSelecting_Musaade == true)
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
                            PoligonDataGridView.Rows[e.RowIndex].Cells["ENERJI_MUSAADE_ABONE_GRUBU"].Value = matchingRow["ENERJI_MUSAADE_ABONE_GRUBU"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["ENERJI_MUSAADE_ENERJILENDIRME_YILI"].Value = matchingRow["ENERJI_MUSAADE_ENERJILENDIRME_YILI"];
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
                isSelecting_Musaade = false;
            }
        }

        private void PoligonDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox comboBox)
            {
                // Set the ComboBox to allow manual input
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                // Optional: Enable autocomplete for better UX
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }
    }
}