using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class Tablo_olustur : Form
    {
        public Tablo_olustur()
        {
            InitializeComponent();
            LoadCbsFiles();
            LoadCsvFiles();
            listBoxCbsFiles.SelectionMode = SelectionMode.MultiExtended; // Çoklu seçim aktif
        }
        private readonly Dictionary<string, List<string>> requiredColumns = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
    {
    { "OGAGTRF", new List<string> { "ID", "KODU", "X_KOORDINAT", "Y_KOORDINAT", "PRIMER_GERILIM", "GUCU" } },
    { "dtr_aril_verileri_son", new List<string> { "Tanım Numarası", "SBS - CBS Kodu", "Çarpan", "Aktif Çekiş", "Demand Çekiş", "Demand Çekiş Tarihi" } },
    { "TRAFOBINATIP", new List<string> { "TM_ID", "TM_FIDER_ID", "ADR_ILCE_ID" } }
    };
        private void listBoxCsvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxCsvFiles.SelectedItem == null) return;

                // Seçilen dosya adı
                string selectedFile = listBoxCsvFiles.SelectedItem.ToString();

                // Tam dosya yolu oluştur
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string csvFolderPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "csv_tablolari", "girdiler");
                string filePath = Path.Combine(csvFolderPath, selectedFile);

                // CSV dosyasını yükle ve bir DataTable oluştur
                DataTable dataTable = LoadCsvFile(filePath);

                // Eğer başarılıysa tabloyu bir DataGridView içinde göster
                if (dataTable != null)
                {
                    //ShowDataTableInGrid(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCsvFiles()
        {
            try
            {
                // Üç seviye üst klasöre çık (bin/Debug'dan SLF klasörüne)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string csvFolderPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "csv_tablolari", "girdiler");

                // CSV klasörünü kontrol et
                if (!Directory.Exists(csvFolderPath))
                {
                    MessageBox.Show($"CSV klasörü bulunamadı! Beklenen yol: {csvFolderPath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // CSV dosyalarını listele
                string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");
                listBoxCsvFiles.Items.Clear();

                foreach (string file in csvFiles)
                {
                    listBoxCsvFiles.Items.Add(Path.GetFileName(file));
                }

                // Eğer klasör boşsa bilgi mesajı göster
                if (csvFiles.Length == 0)
                {
                    MessageBox.Show("CSV klasöründe hiçbir .csv dosyası bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable LoadCsvFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // CSV dosyasını satır satır oku
                var lines = File.ReadAllLines(filePath);

                if (lines.Length > 0)
                {
                    // İlk satır kolon isimleri
                    var columns = lines[0].Split(',');
                    foreach (var column in columns)
                    {
                        dataTable.Columns.Add(column.Trim());
                    }

                    // Diğer satırları veri olarak ekle
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var row = lines[i].Split(',');
                        dataTable.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSV dosyası yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }
        //private void ShowDataTableInGrid(DataTable dataTable)
        //{
        //    try
        //    {
        //        // DataGridView oluştur veya varsa temizle
        //        DataGridView dataGridView = Controls.OfType<DataGridView>().FirstOrDefault();
        //        if (dataGridView == null)
        //        {
        //            dataGridView = new DataGridView { Dock = DockStyle.Fill };
        //            Controls.Add(dataGridView);
        //        }

        //        dataGridView.DataSource = dataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Tabloyu gösterirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void listBoxCbsFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxCbsFiles.SelectedItem == null) return;

                // Seçilen dosya adı
                string selectedFile = listBoxCbsFiles.SelectedItem.ToString();

                // Tam dosya yolu oluştur
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string cbsFolderPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "SLF");
                string filePath = Path.Combine(cbsFolderPath, selectedFile);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadCbsFiles()
        {
            try
            {
                // Üç seviye üst klasöre çık (bin/Debug'dan SLF klasörüne)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string slfDir = Directory.GetParent(baseDir).Parent.Parent.FullName;
                string cbsFolderPath = Path.Combine(slfDir, "CBS", "SLF");

                // Debug için yolu yazdır
                //MessageBox.Show($"Aranan yol: {cbsFolderPath}");

                if (!Directory.Exists(cbsFolderPath))
                {
                    MessageBox.Show($"CBS klasörü bulunamadı! Beklenen yol: {cbsFolderPath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] tabFiles = Directory.GetFiles(cbsFolderPath, "*.tab");
                listBoxCbsFiles.Items.Clear();

                foreach (string file in tabFiles)
                {
                    listBoxCbsFiles.Items.Add(Path.GetFileName(file));
                }

                if (tabFiles.Length == 0)
                {
                    MessageBox.Show("CBS klasöründe hiçbir .tab dosyası bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RunPythonScript(string scriptPath, List<string> arguments)
        {
            try
            {
                string pythonPath = "python";  // veya "python.exe"
                string joinedArguments = string.Join(" ", arguments.Select(arg => $"\"{arg}\""));

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{scriptPath}\" {joinedArguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath),
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (Process process = Process.Start(startInfo))
                {
                    // Çıktıları toplamak için StringBuilder kullan
                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();

                    // Standart çıktıyı oku
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            output.AppendLine(e.Data);
                    };

                    // Hata çıktısını oku
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            error.AppendLine(e.Data);
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Hata kontrolü
                    if (process.ExitCode != 0)
                    {
                        string errorMessage = error.ToString();
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            errorMessage = output.ToString();  // Bazen hata mesajı standart çıktıda olabilir
                        }

                        string detailedError = $"Python script hatası (Çıkış Kodu: {process.ExitCode})\n\n" +
                                             $"Çalıştırılan Script: {scriptPath}\n" +
                                             $"Argümanlar: {joinedArguments}\n\n" +
                                             $"Hata Detayı:\n{errorMessage}";

                        MessageBox.Show(detailedError, "Script Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Başarılı çalışma durumu
                    string successOutput = output.ToString();
                    if (!string.IsNullOrEmpty(successOutput))
                    {
                        // Log başarılı çıktıyı
                        string logPath = Path.Combine(
                            Path.GetDirectoryName(scriptPath),
                            $"script_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                        );
                        File.WriteAllText(logPath, successOutput);

                        MessageBox.Show(
                            $"İşlem başarıyla tamamlandı!\n\nDetaylı log dosyası oluşturuldu:\n{logPath}",
                            "Başarılı",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Script çalıştırma hatası:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Sistem Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private DataTable LoadTabFile(string filePath)
        {
            DataTable dataTable = new DataTable();
            bool isDataSection = false;

            try
            {
                // Önce kolonları al
                var columns = ExtractColumnsFromTabFile(filePath);
                foreach (var col in columns)
                {
                    dataTable.Columns.Add(col.ColumnName);
                }

                // Veriyi oku
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Data Section başlangıcını bul
                    if (trimmedLine.Equals("Data Section", StringComparison.OrdinalIgnoreCase))
                    {
                        isDataSection = true;
                        continue;
                    }

                    // Veri bölümünde ve geçerli bir satır ise
                    if (isDataSection && !string.IsNullOrWhiteSpace(trimmedLine) &&
                        !trimmedLine.StartsWith("\"") && !trimmedLine.StartsWith("!"))
                    {
                        // Virgülle ayrılmış değerleri al
                        var values = trimmedLine.Split(',')
                            .Select(v => v.Trim())
                            .ToArray();

                        if (values.Length >= dataTable.Columns.Count)
                        {
                            var row = dataTable.NewRow();
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                row[i] = values[i];
                            }
                            dataTable.Rows.Add(row);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tab dosyası yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }
        private List<(string ColumnName, string ColumnType)> ExtractColumnsFromTabFile(string filePath)
        {
            var columns = new List<(string ColumnName, string ColumnType)>();
            bool isDefinitionTable = false;
            bool isFields = false;

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Definition Table başlangıcını bul
                    if (trimmedLine.Equals("Definition Table", StringComparison.OrdinalIgnoreCase))
                    {
                        isDefinitionTable = true;
                        continue;
                    }

                    // Fields satırını bul
                    if (isDefinitionTable && trimmedLine.StartsWith("Fields", StringComparison.OrdinalIgnoreCase))
                    {
                        isFields = true;
                        continue;
                    }


                    // begin_metadata'ya ulaştıysak bitir
                    if (trimmedLine.StartsWith("begin_metadata", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    // Kolon tanımlarını işle
                    if (isFields && !string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        if (!trimmedLine.StartsWith("Type") && trimmedLine.Contains(" "))
                        {
                            string[] parts = trimmedLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 1)
                            {
                                string columnName = parts[0].Trim();
                                string columnType = parts.Length > 1 ? parts[1].Trim(';', ' ') : "";
                                columns.Add((columnName, columnType));
                            }
                        }
                    }
                }

                // Debug için bulunan kolonları göster
                if (columns.Count > 0)
                {
                    string columnList = string.Join("\n", columns.Select(c => c.ColumnName));
                    //MessageBox.Show($"Bulunan kolonlar:\n{columnList}", "Kolon Listesi");
                }
                else
                {
                    MessageBox.Show("Hiç kolon bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tab dosyası kolonları okunurken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return columns;
        }
        private bool ValidateFilesAndColumns(Dictionary<string, List<string>> requiredColumns, List<string> selectedFiles)
        {
            try
            {
                foreach (var file in selectedFiles)
                {
                    // Dosya adını al (uzantısı olmadan)
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                    // Gerekli sütunlar sözlüğünde bu dosya var mı?
                    if (requiredColumns.TryGetValue(fileNameWithoutExtension, out List<string> requiredCols))
                    {
                        // Dosyanın tam yolunu oluştur
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "SLF", file);

                        // Tab veya CSV dosyasını yükle
                        DataTable dataTable = file.EndsWith(".tab", StringComparison.OrdinalIgnoreCase)
                            ? LoadTabFile(filePath)
                            : LoadCsvFile(filePath);

                        if (dataTable == null || dataTable.Columns.Count == 0)
                        {
                            MessageBox.Show($"Dosya boş veya okunamadı: {file}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // Gerekli sütunlar dosyada mevcut mu?
                        foreach (var requiredColumn in requiredCols)
                        {
                            if (!dataTable.Columns.Contains(requiredColumn))
                            {
                                MessageBox.Show($"'{file}' dosyasında gerekli sütun eksik: {requiredColumn}",
                                                "Eksik Sütun Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"'{file}' dosyası kontrol listesinde değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                return true; // Tüm kontroller başarılıysa
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kontrol sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void dtrVeriTabloOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Listeden seçilen dosyaları al
                List<string> selectedFiles = listBoxCbsFiles.SelectedItems.Cast<string>().ToList();

                if (selectedFiles.Count < 2)
                {
                    MessageBox.Show("Lütfen OGAGTRF.TAB ve TRAFOBINATIP.TAB dosyalarını seçin!",
                                    "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. CSV ListBox'ından aril_verisi.csv dosyasını seç
                if (listBoxCsvFiles.SelectedItem == null || listBoxCsvFiles.SelectedItem.ToString() != "aril_verisi.csv")
                {
                    MessageBox.Show("Lütfen 'aril_verisi.csv' dosyasını seçin!",
                                    "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string arilCsvFile = listBoxCsvFiles.SelectedItem.ToString();

                // 3. Python script yolunu ve çıktı klasörünü hazırla
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string rootDir = Directory.GetParent(baseDir).Parent.Parent.FullName;
                string scriptPath = Path.Combine(rootDir, "cbs", "PythonScripts", "kod", "dtr_v5_tab_V2.py");
                string outputFolder = Path.Combine(rootDir, "CBS", "csv_tablolari", "ciktilar");

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // 4. Python script argümanlarını hazırla
                List<string> arguments = new List<string>
        {
            Path.Combine(rootDir, "CBS", "SLF", selectedFiles.First(f => f.Equals("OGAGTRF.TAB", StringComparison.OrdinalIgnoreCase))),
            Path.Combine(rootDir, "CBS", "SLF", selectedFiles.First(f => f.Equals("TRAFOBINATIP.TAB", StringComparison.OrdinalIgnoreCase))),
            Path.Combine(rootDir, "CBS", "csv_tablolari", "girdiler", arilCsvFile),
            outputFolder
        };

                // 5. Python scriptini çalıştır
                RunPythonScript(scriptPath, arguments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DTR verisi işlenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void aboneVeriOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Listeden ABONE.TAB dosyasının seçili olduğunu kontrol et
                List<string> selectedFiles = listBoxCbsFiles.SelectedItems.Cast<string>().ToList();

                if (!selectedFiles.Any(f => f.Equals("ABONE.TAB", StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Lütfen ABONE.TAB dosyasını seçin!",
                                    "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Gerekli CSV dosyalarının seçili olduğunu kontrol et
                var requiredCsvFiles = new[] { "ABONE_BILGILER.csv", "DWH_MRC_TUKETIM_SAP.csv" };
                var selectedCsvFiles = listBoxCsvFiles.SelectedItems.Cast<string>().ToList();

                var missingCsvFiles = requiredCsvFiles.Where(f => !selectedCsvFiles.Contains(f)).ToList();
                if (missingCsvFiles.Any())
                {
                    MessageBox.Show($"Lütfen şu CSV dosyalarını seçin:\n{string.Join("\n", missingCsvFiles)}",
                                    "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Dosya yollarını hazırla
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string rootDir = Directory.GetParent(baseDir).Parent.Parent.FullName;

                // Python script yolu
                string scriptPath = Path.Combine(rootDir, "cbs", "PythonScripts", "kod", "abone_verileri_argv_son.py");

                // TAB dosyası yolu - listeden seçilen dosyayı kullan
                string tabFilePath = Path.Combine(rootDir, "CBS", "SLF",
                    selectedFiles.First(f => f.Equals("ABONE.TAB", StringComparison.OrdinalIgnoreCase)));

                // CSV dosyaları yolları - listeden seçilen dosyaları kullan
                string csvInputPath = Path.Combine(rootDir, "CBS", "csv_tablolari", "girdiler");
                string aboneBilgilerPath = Path.Combine(csvInputPath,
                    selectedCsvFiles.First(f => f.Equals("ABONE_BILGILER.csv", StringComparison.OrdinalIgnoreCase)));
                string tuketimPath = Path.Combine(csvInputPath,
                    selectedCsvFiles.First(f => f.Equals("DWH_MRC_TUKETIM_SAP.csv", StringComparison.OrdinalIgnoreCase)));

                // Output klasörü
                string outputFolder = Path.Combine(rootDir, "CBS", "csv_tablolari", "ciktilar");

                // Output klasörünü kontrol et ve gerekirse oluştur
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // 4. Python script argümanlarını hazırla
                List<string> arguments = new List<string>
        {
            tabFilePath,
            aboneBilgilerPath,
            tuketimPath,
            outputFolder
        };

                // Dosyaların varlığını kontrol et
                foreach (string path in new[] { tabFilePath, aboneBilgilerPath, tuketimPath })
                {
                    if (!File.Exists(path))
                    {
                        MessageBox.Show($"Dosya bulunamadı:\n{path}",
                                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // 5. Python scriptini çalıştır
                RunPythonScript(scriptPath, arguments);

                MessageBox.Show("İşlem tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Abone verisi işlenirken hata oluştu:\n{ex.Message}",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
    
