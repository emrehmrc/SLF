using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
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

        public bool is_poligon_saved = true;
        private bool isKaydetClicked = false;

        // Add a public property to access the DataTable
        public DataTable PolygonDataTable => dataTable;

        public Poligon_Özellik_Tanımlama(bool isSelectingYUK, bool isSelectingYGA,
            List<PointLatLng> polygonPoints)
        {
            InitializeComponent();
            modül_formu = new ModülFormu();
            cbsFormu = new CBS(modül_formu);

            // Set the flags before calling SetupDataGridView
            isSelecting_YUK = isSelectingYUK;
            isSelecting_YGA = isSelectingYGA;

            //PoligonDataGridView.EditingControlShowing += PoligonDataGridView_EditingControlShowing;


            // Resolve the Excel file path relative to SLF.exe
            string exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string projectRoot = Directory.GetParent(exeLocation)?.Parent?.FullName;
            if (projectRoot != null)
            {
                excelFilePath = Path.Combine(projectRoot, "Excel Files", "point_load.xlsx");
            }
            else
            {
                excelFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "point_load.xlsx");
                MessageBox.Show($"Excel dosya yolu çözülemedi. Varsayılan yol kullanılıyor: {excelFilePath}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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

        /*
        private void PoligonDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Check if the current cell is a ComboBox cell and get the underlying ComboBox control.
            if (PoligonDataGridView.CurrentCell is DataGridViewComboBoxCell && e.Control is ComboBox comboBox)
            {
                // Allow user to type custom text.
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                // Optionally, enable auto-complete for better UX.
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }*/


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
            string area = Math.Round(cbsFormu.CalculatePolygonArea(polygonPoints),1).ToString() + " m2";

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
                dataTable.Columns.Add("Kurulu Güç", typeof(string));
                dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
                dataTable.Columns.Add("Pik Demant", typeof(string));

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

            } else if (isSelecting_YGA == true)
            {
                // Add "Polygon ID" column
                dataTable.Columns.Add("Polygon ID", typeof(string));
                dataTable.Columns.Add("Koordinatlar", typeof(string));
                dataTable.Columns.Add("Çizilen Alan (m2)", typeof(string));
                dataTable.Columns.Add("Arazi Oranı - Mesken (%)", typeof(string));
                dataTable.Columns.Add("Arazi Oranı - Sanayi (%)", typeof(string));
                dataTable.Columns.Add("Arazi Oranı - Ticarethane (%)", typeof(string));
                dataTable.Columns.Add("Başlangıç Yılı", typeof(string));
                dataTable.Columns.Add("Satürasyon Hızı", typeof(string));
                dataTable.Columns.Add("Yoğunluk", typeof(string));
                dataTable.Columns.Add("Park, Yol, Kaldırım Oranı (%)", typeof(string));
                dataTable.Columns.Add("Sosyal Yapı Parsel Oranı (%)", typeof(string));

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
            columnValues = new Dictionary<string, HashSet<string>>
            {
                { "Tipi", new HashSet<string>() },
                { "Ortalama Kapladığı Alan (m2)", new HashSet<string>() },
                { "Tüketim Sınıfı", new HashSet<string>() },
                { "Kurulu Güç", new HashSet<string>() },
                { "Pik Yüklenme (%)", new HashSet<string>() },
                { "Pik Demant", new HashSet<string>() }
            };

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

                        var excelTable = result.Tables[0];
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

        private void SetupDropdownColumns(Dictionary<string, HashSet<string>> columnValues)
        {
            // Define the columns that should remain manually defined.
            var manualColumns = new HashSet<string>
            {
                "Polygon ID",
                "Koordinatlar",
                "Çizilen Alan (m2)"
            };

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
                            if (columnValues[kvp.Key].Contains(kvp.Value.ToString()))
                            {
                                cell.Value = kvp.Value; // Restore original value if still valid.
                            }
                            else
                            {
                                cell.Value = columnValues[kvp.Key].FirstOrDefault(); // Otherwise, set first available value.
                            }
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
            yük_bilgi_formu_objesi = new Nokta_Yuk_Bilgi_Formu(modül_formu.polygonTypesExcelPath);
            yük_bilgi_formu_objesi.Owner = this;
            yük_bilgi_formu_objesi.ShowDialog();
            yük_bilgi_formu_objesi.BringToFront();
            yük_bilgi_formu_objesi.Focus();

            // Check if yukler changed after the dialog closes
            if (yük_bilgi_formu_objesi.is_yukler_changed)
            {
                // Reload Excel data and update dropdowns
                LoadExcelData();
                SetupDropdownColumns(columnValues);
                yük_bilgi_formu_objesi.is_yukler_changed = false; // Reset the flag
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
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Kurulu Güç"].Value = matchingRow["Kurulu Güç"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Yüklenme (%)"].Value = matchingRow["Pik Yüklenme (%)"];
                            PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Demant"].Value = matchingRow["Pik Demant"];
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
        }
    }
}