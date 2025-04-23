using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace SLF
{
    public partial class imarFileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }
        public string SelectedCity { get; private set; }

        private DataGridView _dataGridViewGirdi;

        public imarFileSelectionPopup(DataGridView dataGridViewGirdi)
        {
            InitializeComponent();
            _dataGridViewGirdi = dataGridViewGirdi;
        }

        private void imarFileSelectionPanel_Paint(object sender, PaintEventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CsvFilePath = openFileDialog.FileName;
                }
            }
        }

        private void SelectKmlButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "KML Files (*.kml)|*.kml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    KmlFilePath = openFileDialog.FileName;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (imarMethodSelectionComboBox.SelectedIndex == 0)
                {
                    if (!imarizmirRadioButton.Checked && !imarEskisehirRadioButton.Checked)
                    {
                        MessageBox.Show("Lütfen bir şehir seçin.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string selectedRegion = imarizmirRadioButton.Checked ? "İzmir" : "Eskişehir";

                    if (string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen bir KML dosyası seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RunPythonScript(selectedRegion: selectedRegion, kmlFilePath: KmlFilePath);
                }
                else if (imarMethodSelectionComboBox.SelectedIndex == 1)
                {
                    if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RunPythonScript(csvFilePath: CsvFilePath, kmlFilePath: KmlFilePath);
                }

                // Populate the DataGridView after successful script execution
                UploadOutputToGridView();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void UploadOutputToGridView()
        {
            try
            {
                string outputCsvPath = @"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\09_Alinan Veriler\GDZ\veriler deneme\imar-denemeleri\updated_results.xlsx";

                if (!File.Exists(outputCsvPath))
                {
                    MessageBox.Show("Çıktı CSV dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var dataTable = new DataTable();
                using (var reader = new StreamReader(outputCsvPath))
                {
                    bool isHeader = true;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;

                        var values = line.Split(',');

                        if (isHeader)
                        {
                            foreach (var header in values)
                            {
                                dataTable.Columns.Add(header);
                            }
                            isHeader = false;
                        }
                        else
                        {
                            dataTable.Rows.Add(values);
                        }
                    }
                }

                _dataGridViewGirdi.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSV verileri yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunPythonScript(string csvFilePath = null, string kmlFilePath = null, string selectedRegion = null)
        {
            try
            {
                const string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe";
                const string scriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\imar-dataları\imar_datalari_v4.py";

                if (!File.Exists(pythonExePath))
                {
                    MessageBox.Show($"Python çalıştırılabilir dosyası bulunamadı: {pythonExePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(scriptPath))
                {
                    MessageBox.Show($"Python betik dosyası bulunamadı: {scriptPath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string arguments;
                if (!string.IsNullOrEmpty(selectedRegion) && !string.IsNullOrEmpty(kmlFilePath))
                {
                    arguments = $"\"{scriptPath}\" \"{selectedRegion}\" \"{kmlFilePath}\"";
                }
                else if (!string.IsNullOrEmpty(csvFilePath) && !string.IsNullOrEmpty(kmlFilePath))
                {
                    arguments = $"\"{scriptPath}\" \"{csvFilePath}\" \"{kmlFilePath}\"";
                }
                else
                {
                    MessageBox.Show("Geçersiz parametreler. Python betiği çalıştırılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonExePath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
                {
                    MessageBox.Show($"Python betiği çalışırken bir hata oluştu:\n{error}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Python betiği başarıyla çalıştırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Python betiği çalıştırılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}


