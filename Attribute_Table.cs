using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.Linq;

namespace SLF
{
    public partial class Tablo_Formu : Form
    {
        public DataGridView attribute_table;
        private ModülFormu parentForm; // Reference to the parent ModülFormu

        public Tablo_Formu(ModülFormu parent)
        {
            InitializeComponent();
            attribute_table = this.vektörel_attribute_table;
            parentForm = parent; // Store the reference to ModülFormu
            this.toolStripMenuItem_tablo.Click += new System.EventHandler(this.toolStripMenuItem_tablo_Click);
            this.toolStripMenuItem_S.Click += new System.EventHandler(this.toolStripMenuItem_S_Click); // Subscribe S click event

            UpdateMenuItemVisibility();

            attribute_table.DataBindingComplete += (s, args) => UpdateMenuItemVisibility();
        }

        public Tablo_Formu()
        {
            InitializeComponent();
            attribute_table = this.vektörel_attribute_table;
            this.toolStripMenuItem_tablo.Click -= new System.EventHandler(this.toolStripMenuItem_tablo_Click);

            UpdateMenuItemVisibility();

            attribute_table.DataBindingComplete += (s, args) => UpdateMenuItemVisibility();
        }

        private void UpdateMenuItemVisibility()
        {
            // Check if attribute_table is initialized and contains the "id" column
            if (attribute_table != null && attribute_table.Columns.Contains("id"))
            {
                toolStripMenuItem_S.Visible = true;
            }
            else
            {
                toolStripMenuItem_S.Visible = false;
            }
        }

        private void Tablo_Formu_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void toolStripMenuItem_tablo_Click(object sender, EventArgs e)
        {
            if (attribute_table == null || attribute_table.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir satır seçin veya veri tablosu yüklenmiş olsun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = attribute_table.SelectedRows[0];
            int rowIndex = Convert.ToInt32(selectedRow.Cells["RowIndex"].Value);

            if (parentForm == null)
            {
                MessageBox.Show("Parent form referansı geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            parentForm.ZoomToFeature(rowIndex);
        }

        private void toolStripMenuItem_S_Click(object sender, EventArgs e)
        {

            if (attribute_table == null || attribute_table.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir satır seçin veya veri tablosu yüklenmiş olsun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = attribute_table.SelectedRows[0];
            int rowIndex = Convert.ToInt32(selectedRow.Cells["RowIndex"].Value);

            // Check if "id" column exists and has a value
            if (selectedRow.Cells["id"] == null || selectedRow.Cells["id"].Value == DBNull.Value || string.IsNullOrEmpty(selectedRow.Cells["id"].Value.ToString()))
            {
                MessageBox.Show("Seçilen satırda 'id' sütunu bulunamadı veya geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(selectedRow.Cells["id"].Value);

            // Validate parentForm and its properties
            if (parentForm == null || parentForm.ana_menu_form_objesi == null || parentForm.ana_menu_form_objesi.config == null)
            {
                MessageBox.Show("Parent form, ana menu form objesi veya yapılandırma verileri geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Construct the file path
            string filePath = Path.Combine(parentForm.ana_menu_form_objesi.userRootPath,
                                          (string)parentForm.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                                          (string)parentForm.ana_menu_form_objesi.config.İl,
                                          (string)parentForm.ana_menu_form_objesi.config.İlçe,
                                          (string)parentForm.ana_menu_form_objesi.config.proje_ismi,
                                          "imar_analizi_sonuclari\\imar_planlari\\saturasyon\\saturasyon_sonuc\\merged_saturasyon.csv");

            bool useDataGridView = attribute_table.Columns.Contains("id") && attribute_table.Columns.Contains("Year_0");
            double[] values = null;
            string currentSaturation = null;

            if (useDataGridView)
            {
                // Use data from DataGridView if "id" and "Year_0" exist
                DataGridViewRow row = attribute_table.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => Convert.ToInt32(r.Cells["id"].Value) == id);
                if (row != null)
                {
                    int yearCount = 0;
                    for (int i = 0; i < attribute_table.Columns.Count; i++)
                    {
                        string columnName = attribute_table.Columns[i].Name.ToLower();
                        if (columnName.StartsWith("year_"))
                            yearCount++;
                    }
                    values = new double[yearCount];
                    int valueIndex = 0;
                    for (int i = 0; i < attribute_table.Columns.Count; i++)
                    {
                        string columnName = attribute_table.Columns[i].Name.ToLower();
                        if (columnName.StartsWith("year_"))
                        {
                            string valueStr = row.Cells[i].Value?.ToString().Trim() ?? "0";
                            if (double.TryParse(valueStr, out double parsedValue))
                                values[valueIndex++] = parsedValue;
                            else
                                values[valueIndex++] = 0.0;
                        }
                    }
                }
            }
            else
            {
                // Read and filter CSV if not using DataGridView
                bool fileAccessible = false;
                while (!fileAccessible)
                {
                    try
                    {
                        if (!File.Exists(filePath))
                        {
                            MessageBox.Show($"Dosya bulunamadı: {filePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string[] lines = File.ReadAllLines(filePath);
                        if (lines.Length == 0)
                        {
                            MessageBox.Show("CSV dosyası boş.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string header = lines[0];
                        string[] headers = ParseCsvHeader(header);


                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] parsedFields = ParseCsvLine(lines[i]);
                            string[] fields = new string[headers.Length];
                            Array.Copy(parsedFields, fields, Math.Min(parsedFields.Length, headers.Length));
                            if (parsedFields.Length < headers.Length)
                            {
                                string[] padding = Enumerable.Repeat("", headers.Length - parsedFields.Length).ToArray();
                                Array.Copy(padding, 0, fields, parsedFields.Length, padding.Length);
                            }

                            if (fields.Length > 0)
                            {
                                if (!int.TryParse(fields[0].Trim(), out int rowId))
                                {
                                    continue;
                                }

                                if (rowId == id)
                                {
                                    int yearCount = 0;
                                    List<int> yearIndices = new List<int>();
                                    for (int j = 0; j < headers.Length; j++)
                                    {
                                        string trimmedLower = headers[j].Trim().ToLower().Replace("\u00A0", " ").Replace("\uFEFF", "");
                                        if (trimmedLower.StartsWith("year_"))
                                        {
                                            yearCount++;
                                            yearIndices.Add(j);
                                        }
                                    }

                                    if (yearCount == 0)
                                    {
                                        MessageBox.Show("CSV dosyasında Year_ sütunları bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    values = new double[yearCount];
                                    int valueIndex = 0;
                                    foreach (int index in yearIndices)
                                    {
                                        if (index >= fields.Length)
                                        {
                                            MessageBox.Show($"Indeks {index} fields dizisinin uzunluğunu ({fields.Length}) aşıyor. Satır {i} veri eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        string value = fields[index].Trim();
                                        if (string.IsNullOrEmpty(value))
                                            values[valueIndex] = 0.0;
                                        else if (!double.TryParse(value, out double parsedValue))
                                            values[valueIndex] = 0.0;
                                        else
                                            values[valueIndex] = parsedValue;
                                        valueIndex++;
                                    }
                                    break;
                                }
                            }
                        }
                        fileAccessible = true; // Exit loop if file access succeeds
                    }
                    catch (IOException)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Dosya {Path.GetFileName(filePath)} şu anda kullanılıyor. Lütfen dosyayı kapatıp 'Tekrar Dene' seçeneğini tıklayın, veya 'İptal' ile işlemi sonlandırın.",
                            "Dosya Kullanımda",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Warning
                        );
                        if (result == DialogResult.Cancel)
                        {
                            return; // Exit the method if user cancels
                        }
                        // Retry loop will continue if user selects Retry
                    }
                }
            }

            // Attempt to read Saturation_updated_{year} from CSV and DataGridView
            string saturationColumn = $"Saturation_updated_{parentForm.slfStartYear}";
            if (File.Exists(filePath))
            {
                bool fileAccessible = false;
                while (!fileAccessible)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(filePath);
                        if (lines.Length > 0)
                        {
                            string[] headers = ParseCsvHeader(lines[0]);
                            int saturationIndex = Array.FindIndex(headers, h => h.Trim().ToLower() == saturationColumn.ToLower());
                            if (saturationIndex >= 0)
                            {
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    string[] fields = ParseCsvLine(lines[i]);
                                    if (fields.Length > 0 && int.TryParse(fields[0].Trim(), out int rowId) && rowId == id)
                                    {
                                        if (saturationIndex < fields.Length)
                                        {
                                            string value = fields[saturationIndex].Trim();
                                            if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double saturationValue))
                                            {
                                                currentSaturation = $"{saturationValue:F2}";
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("İndeks (CSV) fields uzunluğunu aşıyor.");
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Sütun (CSV) {saturationColumn} bulunamadı.");
                            }
                        }
                        fileAccessible = true; // Exit loop if file access succeeds
                    }
                    catch (IOException)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Dosya {Path.GetFileName(filePath)} şu anda kullanılıyor. Lütfen dosyayı kapatıp 'Tekrar Dene' seçeneğini tıklayın, veya 'İptal' ile işlemi sonlandırın.",
                            "Dosya Kullanımda",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Warning
                        );
                        if (result == DialogResult.Cancel)
                        {
                            return; // Exit the method if user cancels
                        }
                        // Retry loop will continue if user selects Retry
                    }
                }
            }

            // If not found in CSV, check DataGridView
            if (string.IsNullOrEmpty(currentSaturation) && attribute_table.Columns.Contains(saturationColumn))
            {
                DataGridViewRow row = attribute_table.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => Convert.ToInt32(r.Cells["id"].Value) == id);
                if (row != null)
                {
                    string valueStr = row.Cells[saturationColumn].Value?.ToString().Trim() ?? "";
                    Console.WriteLine($"Aranan sütun (DataGridView): {saturationColumn}, Değer: {valueStr}");
                    if (!string.IsNullOrEmpty(valueStr) && double.TryParse(valueStr, out double saturationValue))
                    {
                        currentSaturation = $"{saturationValue:F2}";
                        Console.WriteLine($"Güncel satürasyon (DataGridView) ayarlandı: {currentSaturation}");
                    }
                    else
                    {
                        Console.WriteLine("Değer (DataGridView) geçersiz veya boş.");
                    }
                }
            }

            if (values == null)
            {
                MessageBox.Show($"ID {id} için veri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowGraphForm(values, currentSaturation);
        }

        private void ShowGraphForm(double[] values, string currentSaturation)
        {
            Form graphForm = new Form
            {
                Text = "Beklenen Satürasyon Grafiği (S-Eğrisi)",
                Size = new Size(600, 400),
                TopMost = true
            };

            Chart chart = new Chart
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right // Anchor to all sides
            };
            chart.Size = new Size(graphForm.ClientSize.Width - 20, graphForm.ClientSize.Height - 60); // Adjust size to fit form
            chart.Location = new Point(10, 10);

            ChartArea chartArea = new ChartArea("ChartArea")
            {
                AxisX = { Title = "Yıl" },
                AxisY = { Title = "Satürasyon (%)", Minimum = 0, Maximum = 1 } // Fixed Y-axis from 0 to 1
            };
            chart.ChartAreas.Add(chartArea);

            Series series = new Series("Satürasyon")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = Color.Blue
            };

            // Find the index where the value first reaches or exceeds 1 (or the maximum if 1 isn't reached)
            int endIndex = 0;
            double maxValue = values.Max();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] >= 1.0 || i == values.Length - 1) // Stop at 1 or the last value
                {
                    endIndex = i;
                    break;
                }
            }
            if (endIndex == 0 && maxValue > 0) // If 1 isn't reached, use the last non-zero index
            {
                for (int i = values.Length - 1; i >= 0; i--)
                {
                    if (values[i] > 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            // Add points up to the dynamic end index
            for (int i = 0; i <= endIndex; i++)
            {
                series.Points.AddXY($"Yıl_{i}", values[i]);
            }

            // Set X-axis maximum to match the dynamic end index
            chartArea.AxisX.Maximum = endIndex;
            chartArea.AxisX.Minimum = 0;

            chart.Series.Add(series);

            // Add current saturation label if available
            if (!string.IsNullOrEmpty(currentSaturation))
            {
                Label saturationLabel = new Label
                {
                    Text = $"Güncel satürasyon: {Convert.ToDouble(currentSaturation)*100}%",
                    Location = new Point(10, chart.Bottom + 5), 
                    AutoSize = true,
                    Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular),
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };
                graphForm.Controls.Add(saturationLabel);
            }

            graphForm.Controls.Add(chart);
            graphForm.ShowDialog();
        }

        // Improved CSV header parsing with better delimiter and quote handling
        private string[] ParseCsvHeader(string line)
        {
            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            string currentField = "";
            char? previousChar = null;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"' && (previousChar == null || previousChar != '\\'))
                {
                    inQuotes = !inQuotes;
                }
                else if ((c == ',' || c == '\t' || c == ';') && !inQuotes)
                {
                    result.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
                previousChar = c;
            }

            if (!string.IsNullOrEmpty(currentField))
                result.Add(currentField.Trim());

            return result.ToArray();
        }

        // Parse CSV line with the same logic
        private string[] ParseCsvLine(string line)
        {
            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            string currentField = "";
            char? previousChar = null;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"' && (previousChar == null || previousChar != '\\'))
                {
                    inQuotes = !inQuotes;
                }
                else if ((c == ',' || c == '\t' || c == ';') && !inQuotes)
                {
                    result.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
                previousChar = c;
            }

            if (!string.IsNullOrEmpty(currentField))
                result.Add(currentField.Trim());

            return result.ToArray();
        }
    }
}