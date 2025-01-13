using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using NetTopologySuite.IO;
using NetTopologySuite.IO.ShapeFile.Extended;
using Npgsql;
using SLF.services;

namespace SLF
{
    public partial class DatabaseListForm : Form

    {
        private GirdiModülü girdiModülü;
        //public Dictionary<string, GirdiModülü> girdiModülleri = new Dictionary<string, GirdiModülü> {
        //    {"Abone Verileri", new AboneVerileri()},
        //    {"DEK Verileri", new DEKModulu()},
        //    {"DTR Verileri", new DTRModulu()},
        //    {"EA Şarj Verileri", new EASarjModulu()},
        //    {"Ekonometrik Yük Tahmini Verileri", new EkonometrikYukTahminiModulu()},
        //    {"Fider Verileri", new FiderVerileri()},
        //    {"İmar Verileri", new GirdiModülü()},
        //    {"Enerji Müsaadeleri Verileri", new EnerjiMusaadeleri()},
        //    {"Yeni Projelendirilmiş DTR Verileri", new YeniProjelendirilmisDTR()},
        //};
        private readonly Dictionary<string, List<string>> requiredColumns = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)


{
    { "OGAGTRF.TAB", new List<string> {
        "ID",
        "KODU",
        "X_KOORDINAT",
        "Y_KOORDINAT",
        "PRIMER_GERILIM",
        "GUCU",

    }},
    { "dtr_aril_verileri", new List<string> {
        "Tanım Numarası",
        "CBS KODU",
        "Çarpan",
        "Aktif Çekiş",
        "Demand Çekiş",
        "Demand Çekiş Tarihi"
    }},
    { "TRAFOBINATIP.TAB", new List<string> {
        "TM_ID",
        "TM_FIDER_ID",
        "ADR_ILCE_ID"
    }},
     { "EA Şarj Verileri", new List<string> { "istasyon_adi", "istasyon_tipi", "istasyon_gucu", "ea_trafo_kodu", "ea_x_koordinat", "ea_y_koordinat" } },
     { "DTR Verileri", new List<string> { "TRAFO_ID", "TRAFO_KODU", "TRAFO_ILCE_ADI", "TRAFO_KAPASITESI" } }
};


        public DatabaseListForm()
        {
            InitializeComponent();
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

        public DataTable LoadCsvFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dataTable.Rows.Add(dr);
                }
            }

            return dataTable;
        }
        private bool ValidateColumns(DataTable table, List<string> requiredColumns)
        {
            if (table == null || table.Columns.Count == 0)
            {
                MessageBox.Show("Tablo boş veya geçersiz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Mevcut kolon isimlerini büyük harfe çevir
            var currentColumns = table.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName.ToUpperInvariant())
                .ToList();

            // Eksik kolonları bul
            var missingColumns = requiredColumns
                .Where(col => !currentColumns.Contains(col.ToUpperInvariant()))
                .ToList();

            // Eksik kolon varsa kullanıcıya bildir
            if (missingColumns.Any())
            {
                string missingMessage = $"Tabloda eksik olan sütunlar: {string.Join(", ", missingColumns)}";
                MessageBox.Show(missingMessage, "Eksik Kolonlar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Veritabanından tabloyu yükle
        public DataTable LoadDatabaseTable(string tableName)
        {
            DataTable dataTable = new DataTable();
            var connection = DatabaseManager.GetInstance().GetConnection();

            try
            {
                string query = $"SELECT * FROM \"{tableName}\"";
                using (var cmd = new NpgsqlCommand(query, connection))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                    
                    // Kolon isimlerini büyük harfe çevir
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        col.ColumnName = col.ColumnName.ToUpperInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı tablosu yüklenirken hata: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }
        private void DatabaseListForm_Load(object sender, EventArgs e)
        {
            listBoxCbsFiles.SelectionMode = SelectionMode.MultiExtended;
            listBoxTables.SelectionMode = SelectionMode.MultiExtended;

            try
            {
                // Veritabanı tablolarını yükle
                var connection = DatabaseManager.GetInstance("").GetConnection();
                string query = "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname = 'public'";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader.GetString(0);
                            listBoxTables.Items.Add(tableName);
                        }
                    }
                }

                // CBS klasöründeki dosyaları yükle
                LoadCbsFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void HandleData(string source, string dataType, bool isDatabaseTable = false)
        {
            try
            {
                DataTable dataTable;

                if (isDatabaseTable)
                {
                    // Veritabanı tablosunu yükle
                    dataTable = LoadDatabaseTable(source);
                    
                    // GirdiModülü'nü oluştur ve veriyi işle
                    GirdiModülü module = new GirdiModülü();
                    module.importedDataTable = dataTable;
                    
                    if (module.VEERProcess(dataType))
                    {
                        // Başarılı işlem sonrası ModülFormu'nu güncelle
                        ModülFormu modülFormu = new ModülFormu();
                        modülFormu.isİmportedModule(true, dataType);
                        
                        MessageBox.Show($"{dataType} başarıyla işlendi ve Girdi Modülü'ne aktarıldı.", 
                            "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Ana form DataGridView'ı güncelle
                        dataGridViewTableData.DataSource = module.importedDataTable;
                    }
                    else
                    {
                        MessageBox.Show($"{dataType} işleme sırasında hata oluştu.", 
                            "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // CSV dosyası işleme kodu...
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri işleme hatası: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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

            // Dosyanın varlığını kontrol et
            //    if (File.Exists(filePath))
            //    {
            //        MessageBox.Show($"Dosya bulundu: {filePath}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        // Dosyayı DataTable'a yükle
            //        DataTable dataTable = LoadTabFile(filePath);

            //        // DataTable kontrolü


            //        // Veriyi DataGridView'de göster
            //        dataGridViewTableData.DataSource = dataTable;
            //        MessageBox.Show("Tablo başarıyla yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //    else
            //    {
            //        MessageBox.Show($"Seçilen dosya bulunamadı: {filePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void listBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedItem == null) return;
            string selectedTable = listBoxTables.SelectedItem.ToString();
            LoadSelectedTableData(selectedTable);
        }

        private void LoadSelectedTableData(string tableName)
        {
            try
            {
                var connection = DatabaseManager.GetInstance("").GetConnection();

                // Tablo adını doğrula
                string tableNameQuery = @"
            SELECT tablename 
            FROM pg_catalog.pg_tables 
            WHERE schemaname = 'public' 
            AND LOWER(tablename) = LOWER(@tableName)";

                string actualTableName;
                using (var cmd = new NpgsqlCommand(tableNameQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    actualTableName = cmd.ExecuteScalar()?.ToString() ?? tableName;
                }

                // Sütun bilgilerini al
                string columnQuery = @"
            SELECT 
                column_name,
                data_type,
                udt_name
            FROM information_schema.columns 
            WHERE table_schema = 'public' 
            AND LOWER(table_name) = LOWER(@tableName)
            ORDER BY ordinal_position";

                var columns = new System.Collections.Generic.List<(string Name, string Type)>();
                using (var cmd = new NpgsqlCommand(columnQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@tableName", actualTableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add((reader.GetString(0), reader.GetString(2)));
                        }
                    }
                }

                if (columns.Count == 0)
                {
                    MessageBox.Show($"Tablo adı: {actualTableName}\nSütun bulunamadı.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Sütunları hazırla
                var selectColumns = columns.Select(col =>
                {
                    if (col.Type.ToLower() == "geometry")
                        return $"ST_AsText(\"{col.Name}\") AS \"{col.Name}\"";
                    else
                        return $"\"{col.Name}\"";
                });

                // Ana sorguyu oluştur
                string query = $"SELECT {string.Join(", ", selectColumns)} FROM \"{actualTableName}\" LIMIT 100"; // Performans için sınırla

                using (var cmd = new NpgsqlCommand(query, connection))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show($"{actualTableName} tablosu boş.",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        dataGridViewTableData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                        dataGridViewTableData.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo yüklenirken hata: {ex.Message}\nTablo adı: {tableName}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //private void buttonCreateTable_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Seçili tabloları al
        //        var selectedTables = listBoxTables.SelectedItems.Cast<string>().ToList();

        //        if (selectedTables.Count == 0)
        //        {
        //            MessageBox.Show("Lütfen en az bir tablo seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        // Python betiğini çalıştır ve kolon kontrolleri yap
        //        string scriptPath = @"path_to_your_script\data_processing.py";
        //        string arguments = string.Join(" ", selectedTables);
        //        string validationResult = RunPythonScript(scriptPath, arguments);

        //        // Sonuçları göster
        //        MessageBox.Show(validationResult, "Kolon Kontrolleri", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        // Tablo oluşturma işlemini başlat
        //        scriptPath = @"path_to_your_script\table_creation.py";
        //        string creationResult = RunPythonScript(scriptPath, arguments);

        //        MessageBox.Show("Tablo oluşturma işlemi tamamlandı: " + creationResult, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private bool ValidateColumns(string fileNameOrTableName, DataTable table)
        {
            if (!requiredColumns.TryGetValue(fileNameOrTableName, out var expectedColumns))
            {
                MessageBox.Show($"'{fileNameOrTableName}' için sütun kontrolü tanımlı değil.", "Eksik Tanım", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Mevcut kolonları göster
            string existingColumns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            //MessageBox.Show($"Mevcut kolonlar: {existingColumns}", "Mevcut Kolonlar");

            // Beklenen kolonları göster
            string expectedColumnsList = string.Join(", ", expectedColumns);
            //MessageBox.Show($"Beklenen kolonlar: {expectedColumnsList}", "Beklenen Kolonlar");

            var missingColumns = expectedColumns.Where(column =>
                !table.Columns.Cast<DataColumn>()
                    .Any(c => c.ColumnName.Equals(column, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (missingColumns.Any())
            {
                string missingMessage = $"'{fileNameOrTableName}' dosyasında/tabloda aşağıdaki sütunlar eksik:\n{string.Join("\n", missingColumns)}";
                MessageBox.Show(missingMessage, "Eksik Sütunlar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        public void ValidateRequiredColumns(DataTable dataTable, string dataType)
        {
            if (!requiredColumns.TryGetValue(dataType, out var expectedColumns))
            {
                throw new Exception($"'{dataType}' için gerekli kolonlar tanımlı değil.");
            }

            var missingColumns = expectedColumns.Except(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)).ToList();
            if (missingColumns.Any())
            {
                throw new Exception($"Eksik sütunlar: {string.Join(", ", missingColumns)}");
            }
        }

        // Tüm gerekli sütunlar mevcutsa
        private void RunPythonScript(string scriptPath, List<string> arguments)
        {
            try
            {
                string pythonPath = "python";

                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{scriptPath}\" {string.Join(" ", arguments)}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(start))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show($"Python script hatası:\n{error}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show($"Python script beklenmeyen bir hatayla çıktı (Çıkış Kodu: {process.ExitCode}).",
                                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    MessageBox.Show($"Python script çıktısı:\n{output}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Script çalıştırma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtrVerileriTabloOlustur(object sender, EventArgs e)
        {
            if (listBoxCbsFiles.SelectedItems.Count < 2 || listBoxTables.SelectedItems.Count < 1)
            {
                MessageBox.Show("Lütfen OGAGTRF.tab ve TRAFOBINATIP.tab dosyalarını, ayrıca DTR_ARIL_VERILERI tablosunu seçin.", "Eksik Veri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            //string scriptPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "PythonScript", "dtr_v4.py");
            // Dosya ve Tablo Seçimi
            string selectedTabFile1 = listBoxCbsFiles.SelectedItems.Cast<string>()
                .FirstOrDefault(file => file.Equals("OGAGTRF.tab", StringComparison.OrdinalIgnoreCase));
            string selectedTabFile2 = listBoxCbsFiles.SelectedItems.Cast<string>()
                .FirstOrDefault(file => file.Equals("TRAFOBINATIP.tab", StringComparison.OrdinalIgnoreCase));
            string selectedDatabaseTable = listBoxTables.SelectedItems.Cast<string>()
                .FirstOrDefault(table => table.Equals("dtr_aril_verileri", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(selectedTabFile1) || string.IsNullOrEmpty(selectedTabFile2) || string.IsNullOrEmpty(selectedDatabaseTable))
            {
                MessageBox.Show("Gerekli dosya veya tablo seçilmedi! Lütfen OGAGTRF.tab, TRAFOBINATIP.tab dosyalarını ve dtr_aril_verileri tablosunu seçin.",
                    "Eksik Veri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            // Dosya yolları
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            //string scriptPath = Path.Combine(baseDir,"cbs", "PythonScript", "dtr_v4.py");
            string scriptPath = Path.Combine(
            Directory.GetParent(baseDir).Parent.Parent.FullName,  // Proje kök dizinine git
                   "cbs", "PythonScripts", "dtr_v4.py"
                );

            //string scriptPath = @"C:\\Users\\batuhan.yetis\\source\\repos\\SLF\\cbs\\PythonScripts\\dtr_v4.py";
            string cbsFolderPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.FullName, "CBS", "SLF");

            string tabFilePath1 = Path.Combine(cbsFolderPath, selectedTabFile1);
            string tabFilePath2 = Path.Combine(cbsFolderPath, selectedTabFile2);

            if (!File.Exists(tabFilePath1) || !File.Exists(tabFilePath2))
            {
                MessageBox.Show("Gerekli dosyalardan biri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kolon Kontrolü
            DataTable tabFileTable1 = LoadTabFile(tabFilePath1);
            if (!ValidateColumns(selectedTabFile1, tabFileTable1)) return;

            DataTable tabFileTable2 = LoadTabFile(tabFilePath2);
            if (!ValidateColumns(selectedTabFile2, tabFileTable2)) return;

            // Veritabanı sütunlarını kontrol et
            DataTable databaseTable = new DataTable();
            try
            {
                var connection = DatabaseManager.GetInstance("").GetConnection();
                string query = $"SELECT * FROM \"{selectedDatabaseTable}\" LIMIT 1";
                using (var cmd = new NpgsqlCommand(query, connection))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(databaseTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı tablosu yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ValidateColumns(selectedDatabaseTable, databaseTable)) return;

            // Python Script Argümanları
            if (!File.Exists(scriptPath))
            {
                scriptPath = Path.Combine(
                    Directory.GetParent(baseDir).Parent.Parent.FullName,
                    "CBS",
                    "PythonScript",
                    "dtr_v4.py"
                );
            }

            List<string> arguments = new List<string>
            {
                tabFilePath1,   // OGAGTRF.tab dosyasının yolu
                tabFilePath2,   // TRAFOBINATIP.tab dosyasının yolu
                "gdz" // Veritabanı tablosu
            };

            // Python Script'i Çalıştır
            RunPythonScript(scriptPath, arguments);
        }

       
        private void EaSarjTablosuOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı seçimi al
                if (listBoxTables.SelectedItem == null)
                {
                    MessageBox.Show("Lütfen bir tablo seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedTable = listBoxTables.SelectedItem.ToString();

                // 1. Tabloyu Veritabanından Yükle
                DataTable rawDataTable = DatabaseHelper.LoadTable(selectedTable);

                // DEBUG: Yüklenen kolonları göster
                string originalColumns = string.Join(", ", rawDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                MessageBox.Show($"Orijinal Kolonlar: {originalColumns}", "Debug: Kolonlar");

                // 2. Kolon adlarını büyük harfe çevir
                //rawDataTable = ConvertColumnNamesToUpperCase(rawDataTable);

                // DEBUG: Büyük harfe çevrilen kolonları göster
                string updatedColumns = string.Join(", ", rawDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                MessageBox.Show($"Güncellenmiş Kolonlar: {updatedColumns}", "Debug: Kolonlar");
                

                // 3. Gerekli sütunları kontrol edin
                var requiredColumns = new List<string>
                {
                    "ISTASYON_ADI",
                    "ISTASYON_TIPI",
                    "ISTASYON_GUCU",
                    "EA_TRAFO_KODU",
                    "EA_X_KOORDINAT",
                    "EA_Y_KOORDINAT"
                };

                if (!ValidateColumns(rawDataTable, requiredColumns))
                {
                    MessageBox.Show("Tablo gerekli sütunlara sahip değil!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. EA Şarj Modülünü Çalıştır

                girdiModülü = ModülFormu.girdiModülleri["EA Şarj Verileri"];
                if (girdiModülü.VEERProcess("EA_Sarj_verileri"))
                {

                    
                    string secilen_veri_tipi = "EA Sarj Verileri";
                    //var isImported = girdiModülü.VEERProcess("EA_Sarj_verileri");
                    ModülFormu modülFormu = new ModülFormu();
                    modülFormu.isİmportedModule(true, secilen_veri_tipi);
                    


                    MessageBox.Show("Şarj istasyonu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"{selectedTable} işlenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dekTablosuOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı seçimi al
                if (listBoxTables.SelectedItem == null)
                {
                    MessageBox.Show("Lütfen bir tablo seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedTable = listBoxTables.SelectedItem.ToString();

                // 1. Tabloyu Veritabanından Yükle
                DataTable rawDataTable = DatabaseHelper.LoadTable(selectedTable);

                // DEBUG: Yüklenen kolonları göster
                string originalColumns = string.Join(", ", rawDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                MessageBox.Show($"Orijinal Kolonlar: {originalColumns}", "Debug: Kolonlar");

                // 2. Kolon adlarını büyük harfe çevir
                //rawDataTable = ConvertColumnNamesToUpperCase(rawDataTable);

                // DEBUG: Büyük harfe çevrilen kolonları göster
                string updatedColumns = string.Join(", ", rawDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                MessageBox.Show($"Güncellenmiş Kolonlar: {updatedColumns}", "Debug: Kolonlar");

                // 3. Gerekli sütunları kontrol edin
                var requiredColumns = new List<string>
                {
                    "ILCE_ADI",
                    "KAYNAK_TIPI",
                    "DEK_KURULU_GUCU",
                    "DEK_X_KOORDINAT",
                    "DEK_Y_KOORDINAT",
                    "DEK_TM_ADI",
                    "DEK_KURULUM_YERI",
                    "DEK_BAGLANDIGI_TRAFO_KODU"
                };

                if (!ValidateColumns(rawDataTable, requiredColumns))
                {
                    MessageBox.Show("Tablo gerekli sütunlara sahip değil!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. DEK Modülünü Çalıştır
                var dekModulu = new DEKModulu
                {
                    currentDataTable = rawDataTable // Modüle tabloyu gönder
                };

                if (dekModulu.VEERProcess(selectedTable))
                {
                    

                    // "DEK Verileri" olarak modu işaretle
                    string secilen_veri_tipi = "DEK Verileri";
                    ModülFormu modülFormu = new ModülFormu();
                    modülFormu.isİmportedModule(true, secilen_veri_tipi);
                    MessageBox.Show($"{selectedTable} başarıyla işlendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"{selectedTable} işlenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboneVeriTablosuOlustur_Click(object sender, EventArgs e)
        {

        }

        private void fiderVerileriTablosuOlustur_Click(object sender, EventArgs e)
        {

        }
        
        private void enerjiMüsaadeleriTablosuOlustur_Click(object sender, EventArgs e)
        {

        }

        private void YeniProjelendirilmisDTRVerileriTablosuOlustur_Click(object sender, EventArgs e)
        {

        }

        private void imarVerileriTablosuOlustur_Click(object sender, EventArgs e)
        {

        }
    }
    }
// Initialize the DataTable columns
// LoadShapefile metoduna eklenecek debug kodu
