using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using SLF.services;

namespace SLF
{
    public partial class DatabaseListForm : Form
    {
        public DatabaseListForm()
        {
            InitializeComponent();
        }

        private void DatabaseListForm_Load(object sender, EventArgs e)
        {
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
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                // Dosya satırlarını oku
                var lines = File.ReadAllLines(filePath);

                if (lines.Length > 0)
                {
                    // İlk satır başlıklar
                    var headers = lines[0].Split('\t');
                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // Diğer satırlar veri
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var row = lines[i].Split('\t');
                        dataTable.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tab dosyası yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
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

                // Dosyanın varlığını kontrol et
                if (File.Exists(filePath))
                {
                    // Dosyayı DataTable'a yükle
                    DataTable dataTable = LoadTabFile(filePath);

                    // Veriyi DataGridView'de göster
                    dataGridViewTableData.DataSource = dataTable;
                }
                else
                {
                    MessageBox.Show($"Seçilen dosya bulunamadı: {filePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        private void dtrVerileriTabloOlustur(object sender, EventArgs e)
        {

        }

        private void EaSarjTablosuOlustur_Click(object sender, EventArgs e)
        {

        }

        private void dekTablosuOlustur_Click(object sender, EventArgs e)
        {

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
