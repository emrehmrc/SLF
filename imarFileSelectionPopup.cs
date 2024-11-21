using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SLF
{
    public partial class imarFileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }
        public string SelectedCity { get; private set; }

        public imarFileSelectionPopup()
        {
            InitializeComponent();
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
                // Set cursor to wait
                Cursor.Current = Cursors.WaitCursor;

                // Check which method is selected
                if (imarMethodSelectionComboBox.SelectedIndex == 0) // First method selected
                {
                    if (!imarizmirRadioButton.Checked && !imarEskisehirRadioButton.Checked)
                    {
                        MessageBox.Show("Lütfen bir şehir seçin.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Get selected city
                    string selectedRegion = imarizmirRadioButton.Checked ? "İzmir" : "Eskişehir";

                    if (string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen bir KML dosyası seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RunPythonScript(selectedRegion: selectedRegion, kmlFilePath: KmlFilePath);
                }
                else if (imarMethodSelectionComboBox.SelectedIndex == 1) // Second method selected
                {
                    if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!File.Exists(CsvFilePath))
                    {
                        MessageBox.Show($"CSV dosyası bulunamadı: {CsvFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!File.Exists(KmlFilePath))
                    {
                        MessageBox.Show($"KML dosyası bulunamadı: {KmlFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    RunPythonScript(csvFilePath: CsvFilePath, kmlFilePath: KmlFilePath);
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir yöntem seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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

        private void CityRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (imarizmirRadioButton.Checked)
            {
                SelectedCity = "İzmir";
            }
            else if (imarEskisehirRadioButton.Checked)
            {
                SelectedCity = "Eskişehir";
            }
        }
    }
}





/*using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace SLF
{
    public partial class imarFileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }
        public string SelectedCity { get; private set; }

        public imarFileSelectionPopup()
        {
            InitializeComponent();
            ConfigureUI();
        }
        private void imarFileSelectionPanel_Paint(object sender, PaintEventArgs e)
        {
            //imarFileSelectionPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
            this.DoubleBuffered = true;
        }
        private void ConfigureUI()
        {
            // Initialize the UI state
            imarCitySelectionPanel.Enabled = false; // Disable city selection initially
            imarFileSelectionPanel.Enabled = false;   // Disable file upload buttons initially
        }

        private void imarMethodSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Adjust UI based on the selected method
            if (imarMethodSelectionComboBox.SelectedIndex == 0) // Assuming first option enables city selection
            {
                imarCitySelectionPanel.Enabled = true;
                imarFileSelectionPanel.Enabled = false;
            }
            else if (imarMethodSelectionComboBox.SelectedIndex == 1) // Assuming second option enables file upload
            {
                imarCitySelectionPanel.Enabled = false;
                imarFileSelectionPanel.Enabled = true;
            }
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
                // Set cursor to wait
                Cursor.Current = Cursors.WaitCursor;

                // Check which method is selected
                if (imarMethodSelectionComboBox.SelectedIndex == 0) // First method selected
                {
                    if (!imarizmirRadioButton.Checked && !imarEskisehirRadioButton.Checked)
                    {
                        // No city selected
                        MessageBox.Show("Lütfen bir şehir seçin.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Get selected city
                    string selectedRegion = imarizmirRadioButton.Checked ? "İzmir" : "Eskişehir";

                    // Run Python script with the selected region
                    RunPythonScript(selectedRegion: selectedRegion);
                }
                else if (imarMethodSelectionComboBox.SelectedIndex == 1) // Second method selected
                {
                    // Ensure both CSV and KML file paths are selected
                    if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
                    {
                        MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Validate file paths
                    if (!File.Exists(CsvFilePath))
                    {
                        MessageBox.Show($"CSV dosyası bulunamadı: {CsvFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!File.Exists(KmlFilePath))
                    {
                        MessageBox.Show($"KML dosyası bulunamadı: {KmlFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Run Python script with file paths
                    RunPythonScript(CsvFilePath, KmlFilePath);
                }
                else
                {
                    // No valid method selected
                    MessageBox.Show("Lütfen geçerli bir yöntem seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restore cursor to default
                Cursor.Current = Cursors.Default;
            }
        }


        private void RunPythonScript(string csvFilePath = null, string kmlFilePath = null, string selectedRegion = null)
        {
            try
            {
                const string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe";
                const string scriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\imar-dataları\imar_datalari_v4.py";

                // Validate Python executable and script
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

                // Build arguments
                string arguments;
                if (!string.IsNullOrEmpty(selectedRegion))
                {
                    arguments = $"\"{scriptPath}\" \"{selectedRegion}\"";
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

        private void CityRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Update selected city based on the checked radio button
            if (imarizmirRadioButton.Checked)
            {
                SelectedCity = "İzmir";
            }
            else if (imarEskisehirRadioButton.Checked)
            {
                SelectedCity = "Eskişehir";
            }
        }
    }
}
*/
























/*using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SLF
{
    public partial class imarFileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }

        public imarFileSelectionPopup()
        {
            InitializeComponent();
        }
        private void imarFileSelectionPanel_Paint(object sender, PaintEventArgs e)
        {
            //imarFileSelectionPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
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
                    //csvFilePathTextBox.Text = CsvFilePath;
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
                    //kmlFilePathTextBox.Text = KmlFilePath;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Set cursor to wait
                Cursor.Current = Cursors.WaitCursor;

                // Ensure both CSV and KML file paths are selected
                if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
                {
                    MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate file paths
                if (!File.Exists(CsvFilePath))
                {
                    MessageBox.Show($"CSV dosyası bulunamadı: {CsvFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(KmlFilePath))
                {
                    MessageBox.Show($"KML dosyası bulunamadı: {KmlFilePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Run the Python script with selected file paths
                RunPythonScript(CsvFilePath, KmlFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restore cursor to default
                Cursor.Current = Cursors.Default;
            }
        }

        // Method to run the Python script
        private void RunPythonScript(string csvFilePath, string kmlFilePath)
        {
            try
            {
                const string pythonExePath = @"C:\Users\begum.orhan\AppData\Local\Programs\Python\Python312\python.exe";
                const string scriptPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\imar-dataları\imar_datalari_v3.py";

                // Validate Python executable and script
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

                string arguments = $"\"{scriptPath}\" \"{csvFilePath}\" \"{kmlFilePath}\"";

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
*/