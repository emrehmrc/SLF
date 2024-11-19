using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CsvFilePath = openFileDialog.FileName;
                    csvFilePathTextBox.Text = CsvFilePath;
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
                    kmlFilePathTextBox.Text = KmlFilePath;
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












/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class FileSelectionPopup : Form
    {
        public string CsvFilePath { get; private set; }
        public string KmlFilePath { get; private set; }

        public FileSelectionPopup()
        {
            InitializeComponent();
        }

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CsvFilePath = openFileDialog.FileName;
                    csvFilePathTextBox.Text = CsvFilePath;
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
                    kmlFilePathTextBox.Text = KmlFilePath;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CsvFilePath) || string.IsNullOrEmpty(KmlFilePath))
            {
                MessageBox.Show("Lütfen hem CSV hem de KML dosyalarını seçin.", "Eksik Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

}
*/