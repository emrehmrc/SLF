using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using ExcelDataReader;

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

        public Poligon_Özellik_Tanımlama()
        {
            InitializeComponent();
            modül_formu = new ModülFormu();
            cbsFormu = new CBS(modül_formu);

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
            SetupDataGridView();

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

        private void SetupDataGridView()
        {
            dataTable = new DataTable();

            // Add "Polygon ID" column
            dataTable.Columns.Add("Polygon ID", typeof(string));

            // Add other columns
            dataTable.Columns.Add("Tipi", typeof(string));
            dataTable.Columns.Add("Kapladığı Alan (m2)", typeof(string));
            dataTable.Columns.Add("Tüketim Sınıfı", typeof(string));
            dataTable.Columns.Add("Kurulu Güç", typeof(string));
            dataTable.Columns.Add("Pik Yüklenme (%)", typeof(string));
            dataTable.Columns.Add("Pik Demant", typeof(string));

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

        private void LoadExcelData()
        {
            columnValues = new Dictionary<string, HashSet<string>>
            {
                { "Tipi", new HashSet<string>() },
                { "Kapladığı Alan (m2)", new HashSet<string>() },
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
            // Store current values before modifying columns
            var currentValues = new Dictionary<string, object>();
            foreach (DataGridViewRow row in PoligonDataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewColumn column in PoligonDataGridView.Columns)
                    {
                        if (column.Name != "Polygon ID")
                        {
                            currentValues[column.Name] = row.Cells[column.Name].Value;
                        }
                    }
                    break; // Only one row
                }
            }

            // Create a list of columns to replace
            var columnsToReplace = new List<(int Index, string Name)>();
            foreach (DataGridViewColumn column in PoligonDataGridView.Columns)
            {
                if (column.Name != "Polygon ID") // Skip "Polygon ID" column
                {
                    columnsToReplace.Add((column.Index, column.Name));
                }
            }

            // Replace columns with dropdowns
            foreach (var (index, name) in columnsToReplace)
            {
                var comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = name,
                    HeaderText = name,
                    DataPropertyName = name,
                    DataSource = columnValues[name].OrderBy(x => x).ToList(), // Sort for better UX
                    ValueType = typeof(string),
                    FlatStyle = FlatStyle.Standard
                };
                PoligonDataGridView.Columns.Remove(name);
                PoligonDataGridView.Columns.Insert(index, comboBoxColumn);
            }

            // Reapply current values if they are still valid
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
                                cell.Value = kvp.Value; // Restore original value if still valid
                            }
                            else
                            {
                                cell.Value = columnValues[kvp.Key].FirstOrDefault(); // Set to first value if invalid
                            }
                        }
                    }
                    break; // Only one row
                }
            }
        }

        private void buton_poligon_ozellik_Click(object sender, EventArgs e)
        {
            modül_formu.PoligonKaydetEventi(sender, e, modül_formu.polygonOverlay_imar, modül_formu.polygonPoints_imar);
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
                        PoligonDataGridView.Rows[e.RowIndex].Cells["Kapladığı Alan (m2)"].Value = matchingRow["Kapladığı Alan (m2)"];
                        PoligonDataGridView.Rows[e.RowIndex].Cells["Tüketim Sınıfı"].Value = matchingRow["Tüketim Sınıfı"];
                        PoligonDataGridView.Rows[e.RowIndex].Cells["Kurulu Güç"].Value = matchingRow["Kurulu Güç"];
                        PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Yüklenme (%)"].Value = matchingRow["Pik Yüklenme (%)"];
                        PoligonDataGridView.Rows[e.RowIndex].Cells["Pik Demant"].Value = matchingRow["Pik Demant"];
                    }
                }
            }
        }
    }
}