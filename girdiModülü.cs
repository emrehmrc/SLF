using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SLF.services;
using SLF.Services;


namespace SLF
{
    public class NoFileSelectedException : Exception
    {
        public NoFileSelectedException(string message) : base(message)
        {
            MessageBox.Show(message, "Dosya Seçim Hatası",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    
    public class PrerequisiteException : Exception
    {
        public PrerequisiteException(string message) : base(message)
        {
        }
    }
  
    public class GirdiModülü
    {
         
        protected Önizleme onizleme1 = new Önizleme();
        protected Raporlama raporlama1 = new Raporlama(); // excel sayfası için yapılmıs calısma excelexporter ve excel importer için bakılabilir ileri durumlarda 
        protected readonly List<string> veri_listesi_requires_xlsx = new List<string> {  
            "Ekonometrik Yük Tahmini Verileri",
            "EA Şarj Verileri",
            "Fider Verileri",
            "TM Verileri",
            "DTR Verileri",
            "DEK Verileri",
            "Abone Verileri",
            "Enerji Müsaadeleri Verileri",
            "Yeni Projelendirilmiş DTR Verileri"
        };
        
        protected static readonly List<int> TRAFO_KAPASITE_LISTESI = new List<int> // trafo yakınsama için kullanılan list
        {
            15, 25, 40, 50, 63, 100, 160, 200, 250, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500
        };
        protected readonly List<string> veri_listesi_requires_csv = new List<string> { }; 
        protected readonly List<string> veri_listesi_requires_tabular = new List<string> { };
        protected readonly Dictionary<string, string> veri_listesi_requires_database = new Dictionary<string, string>
        {
            { "EA_Sarj_verileri", "EA Şarj Verileri" },
            { "dek_verileri", "DEK Verileri" },
            { "projelendirilmis_trafolar", "Yeni Projelendirilmiş DTR Verileri" },
            { "enerji_musaade_verileri", "Enerji Müsaadeleri Verileri" },
            { "dtr_verileri", "DTR Verileri" },
            {"abone_final_tablosu","Abone Verileri" }
        };
        protected virtual List<string> Prerequisites { get; } = new List<string>();
        protected readonly List<string> nullLikeStrings = new List<string> // doluluk bosluk check kısımları kontrolu yapılıyor
        {
            "",
            "null",
            "N/A",
            "#N/A"
        };
        public string seçilenVeriTipi { get; set; }
        protected readonly YearService _yearService;

        public int slfStartYear
        {
            get { return _yearService.slfStartYear; }
            set { _yearService.slfStartYear = value; }
        }

        public int slfEndYear
        {
            get { return _yearService.slfEndYear; }
            set { _yearService.slfEndYear = value; }
        }

        // Diğer ilgili yıl property'leri - bunların YearService'den alınması önemli
        public int lastYear
        {
            get { return _yearService.LastYear; }
        }

        public int penultimateYear
        {
            get { return _yearService.PenultimateYear; }
        }

        public int horizonYear
        {
            get { return _yearService.HorizonYear; }
        }

        protected const int HoursInYear = 8760;

        protected const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        protected const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        protected const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        protected const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        protected const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        protected readonly string combinedExcelFilter;
        protected readonly string combinedCsvFilter;
        protected readonly string combinedTabularFilter;

        public DataTable currentDataTable = new DataTable();
        public DataTable importedDataTable = new DataTable();
        public static Dictionary<string, DataTable> dataTablesByType = new Dictionary<string, DataTable>();  // Static so that it can be accessed as the same instance from other subclasses
        protected DataTable errorDataTable = new DataTable();
        protected DataTable warningDataTable = new DataTable();
        protected DataTable infoDataTable = new DataTable();
        protected DataTable statDataTable = new DataTable();
        protected DataTable errorDataTableReport = new DataTable();
        protected DataTable warningDataTableReport = new DataTable();
        protected DataTable infoDataTableReport = new DataTable();
        protected DataTable statDataTableReport = new DataTable();
        protected DataTable reportDataTableReport = new DataTable();
        
        protected Dictionary<string, List<int>> columnNullRowsMap = new Dictionary<string, List<int>>();
        protected Dictionary<string, List<int>> imputableRowsMap = new Dictionary<string, List<int>>();
        protected Dictionary<string, (double X, double Y)> binaIdToMostFrequentCoordinates = new Dictionary<string, (double X, double Y)>();
        protected Dictionary<string, string> aboneGrubuMostFrequent = new Dictionary<string, string>();
        protected Dictionary<string, double> annualPeakDemand = new Dictionary<string, double>();
        //protected Dictionary<string, (double X, double Y)> binaIdToAverageCoordinates = new Dictionary<string, (double X, double Y)>();

        protected const int COORDINATE_ROUNDING_PRECISION = 3;
        protected const float MAX_THRESHOLD = float.MaxValue;
        protected const float MIN_THRESHOLD = float.MinValue;
        protected static (float Min, float Max) WARNING_ONLY = (MIN_THRESHOLD, MAX_THRESHOLD);
        protected static (float Min, float Max) INFO_ONLY = (MAX_THRESHOLD, MAX_THRESHOLD);
        protected static (float Min, float Max) ERROR_ONLY = (MIN_THRESHOLD, MIN_THRESHOLD);

        protected static bool aboneTrafoConnectivityPass = true;
        protected const string TO_BE_IMPUTED_STRING = "TO_BE_IMPUTED";

        protected readonly Dictionary<string, double> kFactorByAboneGrubu = new Dictionary<string, double>
        {
            { "AYDINLATMA", 2.5 },
            { "GENEL_AYDINLATMA", 2.5 },
            { "MESKEN", 2.5 },
            { "SANAYI", 2.5 },
            { "TARIMSAL SULAMA", 2.5 },
            { "TICARETHANE", 2.5 },
            { "URETICI", 2.5 },
        };
        protected double K_FACTOR = 2.5;

        protected int RoundUpTrafoKapasitesi(double yeniTrafoKapasitesi)
        {
            // Find the smallest value in the list that is greater than or equal to yeniTrafoKapasitesi
            int roundedKapasite = TRAFO_KAPASITE_LISTESI.FirstOrDefault(kapasite => kapasite >= yeniTrafoKapasitesi);

            // If no such value is found (meaning yeniTrafoKapasitesi is larger than any value in the list), 
            // return the maximum value in the list
            if (roundedKapasite == 0)
            {
                roundedKapasite = TRAFO_KAPASITE_LISTESI.Max();
            }

            return roundedKapasite;
        }

        public bool IsNullLike(object value, bool isZero=false) // 0 VE negatif kontrolu 
        {
            if (value == null || value == DBNull.Value)
            {
                return true;
            }
            if (isZero && value.ToString()=="0")
            {
                return true;
            }

            string stringValue = value.ToString();
            return nullLikeStrings.Contains(stringValue, StringComparer.OrdinalIgnoreCase);
        }

        public void VEERReport(string seçilenVeriTipi)
        {
            DataTable dataTable = importedDataTable;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                RenameTabCounts();
                raporlama1.ShowDialog();
            }
            else
            {
                MessageBox.Show($"{seçilenVeriTipi}'ni yüklemeden rapor tablosunu göremezsiniz.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // bool skipPrerequisites is added for direct access to ELF Method

        public bool VEERProcess(string seçilenVeriTipi, bool skipPrerequisites = false)
        {
            try
            {
                // Skip prerequisite check if the flag is true
                if (!skipPrerequisites)
                {
                    CheckPrerequisites(seçilenVeriTipi);
                }

                ProcessFileSelection(seçilenVeriTipi);

                DataTable dataTable = CurrentDataTable;
                Console.WriteLine(seçilenVeriTipi);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    Onizleme1.Onizleme_DataGrid1.DataSource = dataTable;
                    onizleme1.Buton_YUKLE.Enabled = false;
                    onizleme1.Buton_İLERLE.Enabled = true;
                    ClearReportRows();

                    Preprocess();
                    while (true)
                    {
                        ClearRows();
                        Validate();
                        RenameTabCounts();
                        AppendAllToReportDataTables();
                        if (IsError())
                        {
                            onizleme1.Buton_YUKLE.Enabled = false;
                            onizleme1.Buton_İLERLE.Enabled = false;
                        }

                        if (!IsInfo() && !IsWarning())
                        {
                            onizleme1.Buton_YUKLE.Enabled = true;
                            onizleme1.Buton_İLERLE.Enabled = false;
                        }

                        var dialogResult = Onizleme1.ShowDialog();
                        if (dialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                        else if (dialogResult == DialogResult.OK)
                        {
                            // ImportProcessedData buraya eklenmeli
                            Console.WriteLine("Dialog OK - ImportProcessedData çağrılıyor");
                            ImportProcessedData();  // Bu satır çalışıyor mu?
                            Console.WriteLine("ImportProcessedData tamamlandı");
                            break;
                        }

                        Remove();
                        ClearRows();
                        Validate();
                        Impute();
                    }

                    Postprocess();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VEERProcess Hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }

            return false;
        }
        public void ShowImportedMessage()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"{seçilenVeriTipi} başarıyla yüklendi.");
            sb.AppendLine($"Toplam satır sayısı: {importedDataTable.Rows.Count}");

            // Convert to string
            string result = sb.ToString();
            MessageBox.Show(result, "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public void CheckPrerequisites(string seçilenVeriTipi)
        {
            var missingPrerequisites = new List<string>();

            // Check each prerequisite
            foreach (var prerequisite in Prerequisites)
            {
                if (!dataTablesByType.ContainsKey(prerequisite))
                {
                    missingPrerequisites.Add(prerequisite);
                }
            }

            // If there are missing prerequisites, throw an exception with the list
            if (missingPrerequisites.Count > 0)
            {
                var missingMessage = string.Join(", ", missingPrerequisites);
                throw new PrerequisiteException($"{seçilenVeriTipi}nin yüklenmesi için öncelikle şu verilerin yüklenmesi gerekir: {missingMessage}");
            }
        }
        protected static (float Min, float Max) WarningErrorBoundary(float boundary)
        {
            // Bi verinin "boundary"ye kadar olan kısmı warning, "boundary"den sonrası error
            return (MIN_THRESHOLD, boundary);
        }

        protected static (float Min, float Max) InfoErrorBoundary(float boundary)
        {
            // Bi verinin "boundary"ye kadar olan kısmı info, "boundary"den sonrası error
            return (boundary, boundary);
        }

        protected static (float Min, float Max) InfoWarningBoundary(float boundary)
        { 
            // Bi verinin "boundary"ye kadar olan kısmı info, "boundary"den sonrası warning
            return (boundary, MAX_THRESHOLD);
        }

        // Public read-only property
        public DataTable CurrentDataTable { get { return currentDataTable; }}
        public DataTable ErrorDataTable { get { return errorDataTable; }}
        public DataTable WarningDataTable { get { return warningDataTable; }}
        public DataTable InfoDataTable { get { return infoDataTable; }}
        public Önizleme Onizleme1 { get { return onizleme1; }}
        
        public GirdiModülü()
        {
            combinedExcelFilter = $"{FilterExcelFiles}|{FilterAllFiles}";
            combinedCsvFilter = $"{FilterCsvFiles}|{FilterAllFiles}";
            combinedTabularFilter = $"{FilterTabularFiles}|{FilterAllFiles}";
            AddColumnsToDataTable(errorDataTable);
            AddColumnsToDataTable(warningDataTable);
            AddColumnsToDataTable(infoDataTable);
            AddColumnsToDataTable(statDataTable);
            AddColumnsToDataTable(errorDataTableReport);
            AddColumnsToDataTable(warningDataTableReport);
            AddColumnsToDataTable(infoDataTableReport);
            AddColumnsToDataTable(statDataTableReport);
            onizleme1.Onizleme_DataGrid2.DataSource = errorDataTable;
            onizleme1.Onizleme_DataGrid3.DataSource = warningDataTable;
            onizleme1.Onizleme_DataGrid4.DataSource = infoDataTable;
            onizleme1.Onizleme_DataGrid5.DataSource = statDataTable;
            raporlama1.Onizleme_DataGrid2.DataSource = errorDataTableReport;
            raporlama1.Onizleme_DataGrid3.DataSource = warningDataTableReport;
            raporlama1.Onizleme_DataGrid4.DataSource = infoDataTableReport;
            raporlama1.Onizleme_DataGrid5.DataSource = statDataTableReport;
            //raporlama1.Onizleme_DataGrid6.DataSource = reportDataTableReport;
            onizleme1.Onizleme_DataGrid1.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid2.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid3.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid4.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid5.AllowUserToAddRows = false;
            raporlama1.Onizleme_DataGrid2.AllowUserToAddRows = false;
            raporlama1.Onizleme_DataGrid3.AllowUserToAddRows = false;
            raporlama1.Onizleme_DataGrid4.AllowUserToAddRows = false;
            raporlama1.Onizleme_DataGrid5.AllowUserToAddRows = false;
            _yearService = YearService.GetInstance();
            //raporlama1.Onizleme_DataGrid6.AllowUserToAddRows = false;
        }

        public bool IsError()
        {
            return errorDataTable.Rows.Count > 0;
        }
        public bool IsWarning()
        {
            return warningDataTable.Rows.Count > 0;
        }

        public bool IsInfo()
        {
            return infoDataTable.Rows.Count > 0;
        }

        protected void AddColumnsToDataTable(DataTable table)
        {
            table.Columns.Add("Sütun Adı", typeof(string));
            table.Columns.Add("Validasyon Türü", typeof(string));
            table.Columns.Add("Validasyon Bilgisi", typeof(string));
            table.Columns.Add("Ek Açıklamalar", typeof(string));
        }

        private void AppendAllToReportDataTables()
        {
            AppendToReportDataTables(errorDataTable, errorDataTableReport);
            AppendToReportDataTables(warningDataTable, warningDataTableReport);
            AppendToReportDataTables(infoDataTable, infoDataTableReport);
            AppendToReportDataTables(statDataTable, statDataTableReport);
        }

        protected void AppendToReportDataTables(DataTable source, DataTable destination)
        {
            foreach (DataRow row in source.Rows)
            {
                // Check if an identical row already exists in the destination
                bool duplicateExists = destination.AsEnumerable().Any(r => r.ItemArray.SequenceEqual(row.ItemArray));

                // If no duplicate exists, import the row
                if (!duplicateExists)
                {
                    destination.ImportRow(row);
                }
            }
        }

        public void ImportProcessedData()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                Console.WriteLine("\n=== ImportProcessedData Başlıyor ===");

                // CurrentDataTable kontrolü
                if (currentDataTable == null)
                {
                    throw new ArgumentNullException("currentDataTable", "CurrentDataTable null olamaz");
                }

                // Veriyi kopyala
                importedDataTable = currentDataTable.Copy();
                Console.WriteLine($"Veri kopyalandı - Satır sayısı: {importedDataTable.Rows.Count}");
                Console.WriteLine($"Orijinal seçilenVeriTipi: {seçilenVeriTipi}");

                // Key dönüşümü
                if (veri_listesi_requires_database.TryGetValue(seçilenVeriTipi, out string displayKey))
                {
                    Console.WriteLine($"Key dönüşümü: {seçilenVeriTipi} -> {displayKey}");
                    seçilenVeriTipi = displayKey;
                }

                // dataTablesByType null kontrolü
                if (dataTablesByType == null)
                {
                    Console.WriteLine("dataTablesByType null, yeni instance oluşturuluyor");
                    dataTablesByType = new Dictionary<string, DataTable>();
                }

                // Dictionary'e ekle
                if (!dataTablesByType.ContainsKey(seçilenVeriTipi))
                {
                    Console.WriteLine($"Yeni veri ekleniyor: {seçilenVeriTipi}");
                }
                else
                {
                    Console.WriteLine($"Mevcut veri güncelleniyor: {seçilenVeriTipi}");
                }

                dataTablesByType[seçilenVeriTipi] = importedDataTable;
                Console.WriteLine($"Veri eklendi/güncellendi - Key: {seçilenVeriTipi}, Satır sayısı: {importedDataTable.Rows.Count}");

                // Veriyi klasöre CSV olarak kaydet
                SaveModuleDataToCSV();

                // Geçici klasörün proje durumunu güncelle 
                if (PathService.CurrentMode == PathService.WorkingMode.Temporary)
                {
                    string tempPath = Path.Combine(
                        PathService.BaseDirectory,
                        PathService.FullPath,
                        PathService.CurrentWorkingFolder);

                    SaveTempProjectState(tempPath);
                }

                Console.WriteLine("=== ImportProcessedData Tamamlandı ===\n");

                // Kullanıcıya bilgi göster
                ShowImportedMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n!!! ImportProcessedData HATA !!!");
                Console.WriteLine($"Hata Mesajı: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void SaveModuleDataToCSV()
        {
            try
            {
                // Veri tipi için doğru klasör yolunu al
                string folderPath = PathService.GetGirdilerPathForDataType(seçilenVeriTipi);

                if (string.IsNullOrEmpty(folderPath))
                {
                    Console.WriteLine("Geçerli bir klasör yolu alınamadı, veri kaydedilemedi.");
                    return;
                }

                // Klasör yoksa oluştur
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Dosya adını oluştur (veri tipi ve zaman damgası ile)
                string fileName = $"{seçilenVeriTipi.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string fullPath = Path.Combine(folderPath, fileName);

                // CSV olarak kaydet
                using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.UTF8))
                {
                    // Başlık satırı
                    int columnCount = importedDataTable.Columns.Count;
                    for (int i = 0; i < columnCount; i++)
                    {
                        sw.Write(importedDataTable.Columns[i].ColumnName);
                        if (i < columnCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine();

                    // Veri satırları
                    foreach (DataRow row in importedDataTable.Rows)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            // Null değerleri boş string olarak yaz
                            string value = row[i]?.ToString() ?? "";

                            // Virgül içeren değerleri çift tırnak içine al
                            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }

                            sw.Write(value);
                            if (i < columnCount - 1)
                            {
                                sw.Write(",");
                            }
                        }
                        sw.WriteLine();
                    }
                }

                Console.WriteLine($"Veri CSV olarak kaydedildi: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV kaydetme hatası: {ex.Message}");
            }
        }
        private void SaveTempProjectState(string tempPath)
        {
            try
            {
                // Tamamlanan modül listesini doğrudan dataTablesByType'dan al
                var completedModules = dataTablesByType.Keys.ToList();

                var projectState = new Dictionary<string, object>
                {
                    ["CompletedModules"] = completedModules,
                    ["LastUpdated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string json = System.Text.Json.JsonSerializer.Serialize(projectState,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                string statePath = Path.Combine(tempPath, "temp_state.json");
                File.WriteAllText(statePath, json);

                Console.WriteLine($"Geçici proje durumu kaydedildi: {statePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geçici proje durumu kaydedilirken hata: {ex.Message}");
            }
        }
        private void SaveDataToCorrectFolder()
        {
            try
            {
                // Veri tipi için doğru klasör yolunu al
                string folderPath = PathService.GetGirdilerPathForDataType(seçilenVeriTipi);

                if (string.IsNullOrEmpty(folderPath))
                {
                    Console.WriteLine("Geçerli bir klasör yolu alınamadı, veri kaydedilemedi.");
                    return;
                }

                // Dosya adını oluştur (veri tipi ve zaman damgası ile)
                string fileName = $"{seçilenVeriTipi.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string fullPath = Path.Combine(folderPath, fileName);

                // Veriyi CSV olarak kaydet
                SaveDataTableToCsv(importedDataTable, fullPath);

                Console.WriteLine($"Veri başarıyla kaydedildi: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri kaydetme hatası: {ex.Message}");
            }
        }
        private void SaveDataTableToCsv(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Sütun başlıklarını ekle
            List<string> columnNames = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                columnNames.Add(column.ColumnName);
            }
            sb.AppendLine(string.Join(",", columnNames));

            // Verileri ekle
            foreach (DataRow row in dt.Rows)
            {
                List<string> fields = new List<string>();
                foreach (var item in row.ItemArray)
                {
                    // Virgülleri ve tırnak işaretlerini düzgün biçimlendir
                    string field = item?.ToString() ?? "";
                    if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                    {
                        field = "\"" + field.Replace("\"", "\"\"") + "\"";
                    }
                    fields.Add(field);
                }
                sb.AppendLine(string.Join(",", fields));
            }

            // Dosyayı kaydet
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        private DataTable ConvertColumnNamesToUpperCase(DataTable dataTable)
        {
            // Yeni bir DataTable oluşturup kolon adlarını büyük harfe çeviriyoruz
            DataTable updatedTable = new DataTable();

            foreach (DataColumn column in dataTable.Columns)
            {
                // DEBUG: Her kolon adını göstermek
                Console.WriteLine($"Orijinal Kolon: {column.ColumnName}");

                updatedTable.Columns.Add(column.ColumnName.ToUpperInvariant(), column.DataType);
            }

            // Orijinal verileri yeni tabloya taşı
            foreach (DataRow row in dataTable.Rows)
            {
                updatedTable.Rows.Add(row.ItemArray);
            }

            // DEBUG: Yeni tablonun kolonlarını yazdır
            string updatedColumns = string.Join(", ", updatedTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Console.WriteLine($"Yeni Kolonlar: {updatedColumns}");

            return updatedTable;
        }

        public void ProcessFileSelection(string seçilenVeriTipi)
        {
            try
            {
                this.seçilenVeriTipi = seçilenVeriTipi;

                // Veritabanı işlemleri için ayrı kontrol
                if (veri_listesi_requires_database.Keys.Contains(seçilenVeriTipi))
                {
                    ProcessDatabaseSelection(seçilenVeriTipi);
                    return;
                }

                // Dosya seçim işlemleri
                using (var fileDialog1 = new OpenFileDialog { Title = FileDialogTitle })
                {
                    string filter = GetFileFilter(seçilenVeriTipi);
                    if (string.IsNullOrEmpty(filter))
                    {
                        MessageBox.Show("Bu veri tipi için atanmış bir dosya veya veritabanı seçimi prosedürü henüz yok.",
                            "Prosedür Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    fileDialog1.Filter = filter;
                    if (fileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        ProcessSelectedFile(fileDialog1.FileName, seçilenVeriTipi);
                    }
                    else
                    {
                        MessageBox.Show("Dosya seçimi gerçekleştirilemedi.",
                            "Dosya Seçim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFileFilter(string seçilenVeriTipi)
        {
            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
                return combinedExcelFilter;
            else if (veri_listesi_requires_csv.Contains(seçilenVeriTipi))
                return combinedCsvFilter;
            else if (veri_listesi_requires_tabular.Contains(seçilenVeriTipi))
                return combinedTabularFilter;
            return null;
        }

        private void ProcessSelectedFile(string fileName, string seçilenVeriTipi)
        {
            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
            {
                currentDataTable = ProcessExcelFile(fileName, seçilenVeriTipi);
            }
            else if (veri_listesi_requires_csv.Contains(seçilenVeriTipi))
            {
                currentDataTable = ProcessCsvFile(fileName);
            }
            else if (veri_listesi_requires_tabular.Contains(seçilenVeriTipi))
            {
                currentDataTable = ProcessTabularFile(fileName);
            }
        }

        private void ProcessDatabaseSelection(string seçilenVeriTipi)
        {
            try
            {
                currentDataTable = ProcesssqlFile(seçilenVeriTipi);

                // ID kolonunu kontrol et ve kaldır
                if (currentDataTable.Columns.Contains("ID"))
                {
                    currentDataTable.Columns.Remove("ID");
                    MessageBox.Show("ID kolonu kaldırıldı.",
                        "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Kolon isimlerini büyük harfe çevir
                currentDataTable = ConvertColumnNamesToUpperCase(currentDataTable);

                Console.WriteLine($"Veri tipi: {seçilenVeriTipi}");
                Console.WriteLine($"Satır sayısı: {currentDataTable.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanından veri alınırken hata oluştu: {ex.Message}",
                    "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected DataTable ProcessExcelFile(string fileName, string seçilenVeriTipi)
        {
            ExcelImporter importer = new ExcelImporter();
            DataTable dataTable = importer.ImportExcelFile(fileName, seçilenVeriTipi);



            return dataTable;
        }

        protected DataTable ProcessCsvFile(string fileName)
        {
            // TODO: Implement CSV file processing
            return new DataTable();
        }
        protected DataTable ProcesssqlFile(string fileName)
        {
            try
            {
                Console.WriteLine($"ProcesssqlFile başladı - fileName: {fileName}");
                DataTable dataTable = DatabaseHelper.LoadTable(fileName);

                // Gelen veriyi kontrol et
                Console.WriteLine("Kolonlar kontrol ediliyor...");
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col == null)
                    {
                        Console.WriteLine("NULL kolon bulundu!");
                        continue;
                    }
                    Console.WriteLine($"Kolon adı: {col.ColumnName}");
                }

                // ID kolonunu güvenli şekilde kaldır
                if (dataTable.Columns.Contains("ID"))
                {
                    Console.WriteLine("ID kolonu kaldırılıyor");
                    dataTable.Columns.Remove("ID");
                }

                // Kolon isimlerini güvenli şekilde büyük harfe çevir
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col != null && col.ColumnName != null)
                    {
                        string oldName = col.ColumnName;
                        col.ColumnName = oldName.ToUpperInvariant();
                        Console.WriteLine($"Kolon adı değiştirildi: {oldName} -> {col.ColumnName}");
                    }
                }

                Console.WriteLine($"Toplam satır sayısı: {dataTable.Rows.Count}");
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProcesssqlFile Hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        protected DataTable ProcessTabularFile(string fileName)
        {
            // TODO: Implement tabular file processing
            return new DataTable();
        }

        // Helper method to get the original tab text without the count
        private string GetOriginalTabText(TabPage tabPage)
        {
            string text = tabPage.Text;
            int index = text.LastIndexOf('(');
            if (index > 0)
            {
                return text.Substring(0, index).Trim();
            }
            return text;
        }

        public void RenameTabCounts()
        {
            int errorCount = errorDataTable.Rows.Count;
            int warningCount = warningDataTable.Rows.Count;
            int infoCount = infoDataTable.Rows.Count;
            int statCount = statDataTable.Rows.Count;

            int errorReportCount = errorDataTableReport.Rows.Count;
            int warningReportCount = warningDataTableReport.Rows.Count;
            int infoReportCount = infoDataTableReport.Rows.Count;
            int statReportCount = statDataTableReport.Rows.Count;

            // Append counts to the current text of each tab
            onizleme1.Onizleme_Hata_Sekmesi.Text = $"{GetOriginalTabText(onizleme1.Onizleme_Hata_Sekmesi)} ({errorCount})";
            onizleme1.Onizleme_Warning_Sekmesi.Text = $"{GetOriginalTabText(onizleme1.Onizleme_Warning_Sekmesi)} ({warningCount})";
            onizleme1.Onizleme_Information_Sekmesi.Text = $"{GetOriginalTabText(onizleme1.Onizleme_Information_Sekmesi)} ({infoCount})";
            onizleme1.Onizleme_Statistics_Sekmesi.Text = $"{GetOriginalTabText(onizleme1.Onizleme_Statistics_Sekmesi)} ({statCount})";

            raporlama1.Onizleme_Hata_Sekmesi.Text = $"{GetOriginalTabText(raporlama1.Onizleme_Hata_Sekmesi)} ({errorReportCount})";
            raporlama1.Onizleme_Warning_Sekmesi.Text = $"{GetOriginalTabText(raporlama1.Onizleme_Warning_Sekmesi)} ({warningReportCount})";
            raporlama1.Onizleme_Information_Sekmesi.Text = $"{GetOriginalTabText(raporlama1.Onizleme_Information_Sekmesi)} ({infoReportCount})";
            raporlama1.Onizleme_Statistics_Sekmesi.Text = $"{GetOriginalTabText(raporlama1.Onizleme_Statistics_Sekmesi)} ({statReportCount})";
        }

        public virtual void Validate()
        {
        }


        public virtual void Remove()
        {

        }

        public virtual void Impute()
        {

        }

        public virtual void Preprocess()
        {

        }

        public virtual void Postprocess()
        {

        }

        protected void ClearRows()
        {
            errorDataTable.Rows.Clear();
            warningDataTable.Rows.Clear();
            infoDataTable.Rows.Clear();
            statDataTable.Rows.Clear();
        }
        protected void ClearReportRows()
        {
            errorDataTableReport.Rows.Clear();
            warningDataTableReport.Rows.Clear();
            infoDataTableReport.Rows.Clear();
            statDataTableReport.Rows.Clear();
        }
        protected DataTable GetDataTableBasedOnThreshold(
            float currentPercentage,
            float warningThreshold,
            float errorThreshold
        )
        {
            if (currentPercentage >= errorThreshold)
            {
                return errorDataTable;
            }
            else if (currentPercentage >= warningThreshold)
            {
                return warningDataTable;
            }
            else
            {
                return infoDataTable;
            }
        }
    }
}

