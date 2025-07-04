using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SLF.Services;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SLF.Optimal_DTR;

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
        public ModülFormu modülFormu;
        public HomePageForm homePageObjesi;
        protected Önizleme onizleme1 = new Önizleme();
        protected Raporlama raporlama1 = new Raporlama(); // excel sayfası için yapılmıs calısma excelexporter ve excel importer için bakılabilir ileri durumlarda 
        protected readonly List<string> veri_listesi_requires_xlsx = new List<string> {
            "Ekonometrik Yük Tahmini Verileri",
            "EA Şarj Verileri",
            "DTR Verileri",
            "DEK Verileri",
            "Abone Verileri",
            "İmar Verileri",
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
            { "dtr_verileri", "DTR Verileri" },
            { "imar_verileri", "İmar Verileri" },
            { "abone_final_tablosu","Abone Verileri" }
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

        public static Dictionary<string, DataTable> dataTablesByType = new Dictionary<string, DataTable>();
        protected static readonly object _dataTablesLock = new object();

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

        public bool IsNullLike(object value, bool isZero = false)
        {
            try
            {
                Console.WriteLine($"IsNullLike called with value: '{value}' (Type: {value?.GetType().Name})");

                if (value == null || value == DBNull.Value)
                {
                    Console.WriteLine("Value is null or DBNull, returning true");
                    return true;
                }

                if (isZero && value.ToString() == "0")
                {
                    Console.WriteLine("Value is '0' with isZero, returning true");
                    return true;
                }

                string stringValue = value?.ToString() ?? "";
                Console.WriteLine($"Converted to string: '{stringValue}'");

                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    Console.WriteLine("String is null or whitespace, returning true");
                    return true;
                }

                if (double.TryParse(stringValue, out _))
                {
                    Console.WriteLine("Value is numeric, returning false");
                    return false;
                }

                bool result = nullLikeStrings.Contains(stringValue, StringComparer.OrdinalIgnoreCase);
                Console.WriteLine($"Checked nullLikeStrings, result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsNullLike Exception: {ex.Message} - StackTrace: {ex.StackTrace}");
                return false; // Fallback to false on error
            }
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

        public async Task<bool> VEERProcess(string seçilenVeriTipi, bool skipPrerequisites = false)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Starting VEERProcess with seçilenVeriTipi: {seçilenVeriTipi}");
                if (!skipPrerequisites)
                {
                    CheckPrerequisites(seçilenVeriTipi);
                }

                await ProcessFileSelection(seçilenVeriTipi);
                System.Diagnostics.Debug.WriteLine("File selection processed.");

                DataTable dataTable = CurrentDataTable;

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"DataTable has {dataTable.Rows.Count} rows, {dataTable.Columns.Count} columns.");
                    currentDataTable = dataTable;

                    // Configure DataGridView
                    Onizleme1.Onizleme_DataGrid1.DataSource = null;
                    Onizleme1.Onizleme_DataGrid1.AllowUserToAddRows = false;
                    Onizleme1.Onizleme_DataGrid1.VirtualMode = false; // Ensure all rows are loaded
                    Onizleme1.Onizleme_DataGrid1.DataSource = currentDataTable;
                    ApplyDataGridViewFormatting(currentDataTable, Onizleme1.Onizleme_DataGrid1);
                    System.Diagnostics.Debug.WriteLine($"DataGridView bound with {Onizleme1.Onizleme_DataGrid1.Rows.Count} rows.");

                    Onizleme1.Buton_YUKLE.Enabled = false;
                    Onizleme1.Buton_İLERLE.Enabled = true;
                    ClearReportRows();

                    Preprocess();
                    System.Diagnostics.Debug.WriteLine("Preprocessing completed.");

                    while (true)
                    {
                        ClearRows();
                        Validate();
                        System.Diagnostics.Debug.WriteLine($"Validation completed. DataTable rows: {currentDataTable.Rows.Count}");
                        RenameTabCounts();
                        AppendAllToReportDataTables();
                        if (IsError())
                        {
                            System.Diagnostics.Debug.WriteLine("Errors detected, disabling buttons.");
                            Onizleme1.Buton_YUKLE.Enabled = false;
                            Onizleme1.Buton_İLERLE.Enabled = false;
                        }
                        if (!IsInfo() && !IsWarning())
                        {
                            System.Diagnostics.Debug.WriteLine("No info/warnings, enabling YUKLE.");
                            Onizleme1.Buton_YUKLE.Enabled = true;
                            Onizleme1.Buton_İLERLE.Enabled = false;
                        }

                        System.Diagnostics.Debug.WriteLine("Showing Onizleme1 dialog.");
                        var dialogResult = Onizleme1.ShowDialog();
                        System.Diagnostics.Debug.WriteLine($"Dialog result: {dialogResult}");

                        if (dialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                        else if (dialogResult == DialogResult.OK)
                        {
                            ImportProcessedData();
                            break;
                        }

                        using (new WaitCursor(modülFormu))
                        {
                            await Task.Yield(); // Allow UI to update

                            await Task.Run(() => Remove());
                            await Task.Run(() => ClearRows());
                            await Task.Run(() => Validate());
                            await Task.Run(() => Impute());

                        }
                        
                    }

                    Postprocess();
                    return true;
                }
                else
                {
                    if (seçilenVeriTipi != "İmar Verileri")
                    {
                        MessageBox.Show("Veri tablosu boş veya yüklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                Onizleme1.Onizleme_DataGrid1.ScrollBars = ScrollBars.Both;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu!!\n\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Update CheckPrerequisites
        public void CheckPrerequisites(string seçilenVeriTipi)
        {
            var missingPrerequisites = new List<string>();
            lock (_dataTablesLock)
            {
                foreach (var prerequisite in Prerequisites)
                {
                    if (!dataTablesByType.ContainsKey(prerequisite))
                    {
                        missingPrerequisites.Add(prerequisite);
                    }
                }
            }
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


        // Public read-only property
        public DataTable CurrentDataTable { get { return currentDataTable; } }
        public DataTable ErrorDataTable { get { return errorDataTable; } }
        public DataTable WarningDataTable { get { return warningDataTable; } }
        public DataTable InfoDataTable { get { return infoDataTable; } }
        public Önizleme Onizleme1 { get { return onizleme1; } }

        public GirdiModülü()
        {
            modülFormu = new ModülFormu();
            homePageObjesi = new HomePageForm();

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
            _yearService = YearService.GetInstance();
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

        // VEER prosesi tamamlanıp düzgün veriler elde edildikten sonra çağrılan metot
        public void ImportProcessedData()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (currentDataTable == null)
                {
                    throw new ArgumentNullException("currentDataTable", "CurrentDataTable null olamaz");
                }

                importedDataTable = currentDataTable.Copy();

                if (veri_listesi_requires_database.TryGetValue(seçilenVeriTipi, out string displayKey))
                {
                    seçilenVeriTipi = displayKey;
                }

                lock (_dataTablesLock)
                {
                    if (dataTablesByType == null)
                    {
                        dataTablesByType = new Dictionary<string, DataTable>();
                    }
                    dataTablesByType[seçilenVeriTipi] = importedDataTable;
                }

                SaveModuleDataToCSV();

                // Update project state for both temporary and imported projects
                string projectPath = Path.Combine(PathService.BaseDirectory, PathService.FullPath, PathService.CurrentWorkingFolder);
                UpdateProjectState(projectPath);

                if (seçilenVeriTipi == "Ekonometrik Yük Tahmini Verileri")
                {
                    try
                    {
                        var excelExporter = new ExcelExporter();
                        excelExporter.UpdateExcelFileFirstSheet(Path.Combine(modülFormu.ana_menu_form_objesi.userRootPath,
                            (string)modülFormu.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                                (string)modülFormu.ana_menu_form_objesi.config.İl,
                                (string)modülFormu.ana_menu_form_objesi.config.İlçe,
                                (string)modülFormu.ana_menu_form_objesi.config.ELF.INPUT_FILE).Replace('/', '\\'),
                            importedDataTable);
                        RunRScriptSenaryolar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error while saving the file: {ex.Message}");
                    }
                }

                ShowImportedMessage();
            }
            catch (Exception ex)
            {
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

            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV kaydetme hatası: {ex.Message}");
            }
        }

        private void UpdateProjectState(string projectPath)
        {
            try
            {
                var yearService = YearService.GetInstance();
                string statePath = Path.Combine(projectPath, "project_state.json");

                Dictionary<string, object> projectState = new Dictionary<string, object>();
                if (File.Exists(statePath))
                {
                    // Read existing project_state.json
                    string existingJson = File.ReadAllText(statePath);
                    projectState = JsonConvert.DeserializeObject<Dictionary<string, object>>(existingJson) ?? new Dictionary<string, object>();
                }

                // Update CompletedModules with current dataTablesByType keys
                var completedModules = dataTablesByType.Keys.ToList();
                projectState["CompletedModules"] = completedModules;

                // Preserve or add other fields
                projectState["SLFStartYear"] = projectState.ContainsKey("SLFStartYear") ? projectState["SLFStartYear"] : yearService.slfStartYear;
                projectState["SLFEndYear"] = projectState.ContainsKey("SLFEndYear") ? projectState["SLFEndYear"] : yearService.slfEndYear;
                projectState["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                projectState["CreatedBy"] = projectState.ContainsKey("CreatedBy") ? projectState["CreatedBy"] : Environment.UserName;

                // Serialize and save the updated state
                string updatedJson = JsonConvert.SerializeObject(projectState, Newtonsoft.Json.Formatting.Indented);
                Directory.CreateDirectory(projectPath); // Ensure directory exists
                File.WriteAllText(statePath, updatedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu proje durumu güncellenirken: {ex.Message}");
                throw;
            }
        }

        private DataTable ConvertColumnNamesToUpperCase(DataTable dataTable)
        {
            // Yeni bir DataTable oluşturup kolon adlarını büyük harfe çeviriyoruz
            DataTable updatedTable = new DataTable();

            foreach (DataColumn column in dataTable.Columns)
            {
                updatedTable.Columns.Add(column.ColumnName.ToUpperInvariant(), column.DataType);
            }

            // Orijinal verileri yeni tabloya taşı
            foreach (DataRow row in dataTable.Rows)
            {
                updatedTable.Rows.Add(row.ItemArray);
            }

            // DEBUG: Yeni tablonun kolonlarını yazdır
            string updatedColumns = string.Join(", ", updatedTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));

            return updatedTable;

        }

        private void RunRScriptSenaryolar()
        {
            try
            {
                // Construct the path to the R script
                string rScriptPath = Path.Combine(modülFormu.ana_menu_form_objesi.userRootPath,
                    (string)modülFormu.ana_menu_form_objesi.config.Ana_Klasör_Yolu,
                    (string)modülFormu.ana_menu_form_objesi.config.program_dosyaları_path,
                    (string)modülFormu.ana_menu_form_objesi.config.ELF.Rscript_Yolu_Senaryolar).Replace('/', '\\');

                // Run Rscript.exe directly with quoted paths
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "Rscript.exe",
                        Arguments = $"--vanilla \"{rScriptPath}\" \"{modülFormu.ana_menu_form_objesi.config_path}\"",
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

                // Show result
                if (process.ExitCode != 0)
                    MessageBox.Show($"R script çalışmasında bir hata meydana geldi.\nHata: {error}\nÇıktı: {output}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    MessageBox.Show($"R script çalıştırılarak 5 adet senaryo başarıyla oluşturuldu.!",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata meydana geldi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        public async Task ProcessFileSelection(string seçilenVeriTipi)
        {
            try
            {
                this.seçilenVeriTipi = seçilenVeriTipi;

                if (veri_listesi_requires_database.Keys.Contains(seçilenVeriTipi))
                {
                    ProcessDatabaseSelection(seçilenVeriTipi);
                    return;
                }

                using (var fileDialog1 = new OpenFileDialog { Title = FileDialogTitle })
                {

                    if (seçilenVeriTipi == "İmar Verileri")
                    {
                        return;
                    }

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
                        await ProcessSelectedFile(fileDialog1.FileName, seçilenVeriTipi);
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

        private async Task ProcessSelectedFile(string fileName, string seçilenVeriTipi)
        {
            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
            {

                
                    currentDataTable = await Task.Run(() =>
                    {
                        using (var stream = File.OpenRead(fileName))
                        {
                            return ProcessExcelFile(stream, seçilenVeriTipi);
                        }
                    });                                                               

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

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanından veri alınırken hata oluştu: {ex.Message}",
                    "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected DataTable ProcessExcelFile(Stream stream, string seçilenVeriTipi)
        {
            ExcelImporter importer = new ExcelImporter();
                       
            var table = importer.ImportExcelFileAsync(stream, seçilenVeriTipi);
            return NormalizeDataTableTypes(table);
            
            //DataTable dataTable = importer.ImportExcelFile(fileName, seçilenVeriTipi);
            //return NormalizeDataTableTypes(dataTable); // Return normalized table
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
                DataTable dataTable = DatabaseHelper.LoadTable(fileName);
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col == null)
                    {
                        continue;
                    }
                }
                if (dataTable.Columns.Contains("ID"))
                {
                    dataTable.Columns.Remove("ID");
                }
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col != null && col.ColumnName != null)
                    {
                        string oldName = col.ColumnName;
                        col.ColumnName = oldName.ToUpperInvariant();
                    }
                }
                return NormalizeDataTableTypes(dataTable); // Return normalized table
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected DataTable ProcessTabularFile(string fileName)
        {
            // TODO: Implement tabular file processing
            return new DataTable();
        }

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

        protected DataTable GetDataTableBasedOnThreshold(float currentPercentage, float warningThreshold, float errorThreshold)
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

        public void ApplyDataGridViewFormatting(DataTable dataTable, DataGridView dataGridView)
        {
            // Ensure the DataGridView is initialized
            if (dataGridView == null)
            {
                return;
            }

            // Get percentage columns from derived class (will be overridden in EkonometrikYukTahminiModulu)
            var percentageColumns = GetPercentageColumns();

            foreach (DataGridViewColumn gridColumn in dataGridView.Columns)
            {
                string columnName = gridColumn.DataPropertyName;

                // Check if the column is a percentage column
                if (percentageColumns.Contains(columnName))
                {
                    // Format as percentage: e.g., 0.034 -> 3.4%
                    gridColumn.DefaultCellStyle.Format = "P1"; // 1 decimal place, e.g., 3.4%
                    gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else
                {
                    // Check if the column is numeric (int, float, double, decimal)
                    var dataColumn = dataTable.Columns[columnName];
                    if (dataColumn != null && (dataColumn.DataType == typeof(int) ||
                                               dataColumn.DataType == typeof(float) ||
                                               dataColumn.DataType == typeof(double) ||
                                               dataColumn.DataType == typeof(decimal)))
                    {
                        // Format as numeric with thousand separators and max 1 decimal: e.g., 343565.4
                        gridColumn.DefaultCellStyle.Format = "N1"; // Thousand separators, 1 decimal place
                        gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
        }

        // Virtual method to get percentage columns, to be overridden by derived classes
        protected virtual HashSet<string> GetPercentageColumns()
        {
            return new HashSet<string>();
        }

        protected DataTable NormalizeDataTableTypes(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Columns.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("NormalizeDataTableTypes: Input DataTable is null or empty.");
                return new DataTable();
            }

            var percentageColumns = GetPercentageColumns(); // Assumes this returns columns like "Pik Yüklenme (%)"
            List<string> dateColumns = new List<string>();
            List<string> numericColumns = new List<string>();

            // Classify columns
            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName.ToLower();
                // Predefine known string columns to avoid misclassification
                if (columnName.Contains("id") || columnName.Contains("adi") || columnName.Contains("kod") ||
                    columnName.Contains("mulkiyet") || columnName.Contains("mahalle") || columnName.Contains("ilce"))
                {
                    continue; // Treat as string (e.g., TRAFO_ID, TRAFO_ADI)
                }

                int validDateCount = 0;
                int validNumberCount = 0;
                int nonEmptyCount = 0;
                int sampleSize = Math.Min(50, dataTable.Rows.Count);

                for (int i = 0; i < sampleSize; i++)
                {
                    object value = dataTable.Rows[i][column.ColumnName];
                    if (!IsNullLike(value))
                    {
                        string valueAsString = value.ToString().Trim();
                        if (valueAsString != "#N/A")
                        {
                            nonEmptyCount++;
                            if (DateTime.TryParseExact(valueAsString, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
                            {
                                validDateCount++;
                            }
                            else if (double.TryParse(valueAsString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedValue) &&
                                     !Regex.IsMatch(valueAsString, @"^\d+$")) // Exclude integer-like strings
                            {
                                validNumberCount++;
                            }
                        }
                    }
                }

                if (nonEmptyCount > 0)
                {
                    if (validDateCount >= nonEmptyCount * 0.9)
                    {
                        dateColumns.Add(column.ColumnName);
                    }
                    else if (validNumberCount >= nonEmptyCount * 0.9)
                    {
                        numericColumns.Add(column.ColumnName);
                    }
                }
            }

            // Create new table with adjusted types
            DataTable newTable = dataTable.Clone();
            foreach (DataColumn column in newTable.Columns)
            {
                bool isPercentage = percentageColumns.Contains(column.ColumnName);
                bool isNumeric = numericColumns.Contains(column.ColumnName);
                bool isDate = dateColumns.Contains(column.ColumnName);
                bool isKnownString = column.ColumnName.ToLower().Contains("id") ||
                                    column.ColumnName.ToLower().Contains("adi") ||
                                    column.ColumnName.ToLower().Contains("kod") ||
                                    column.ColumnName.ToLower().Contains("mulkiyet") ||
                                    column.ColumnName.ToLower().Contains("mahalle") ||
                                    column.ColumnName.ToLower().Contains("ilce");

                column.DataType = (isPercentage || isNumeric) ? typeof(double) : typeof(string);
                if (isDate || isKnownString)
                {
                    column.DataType = typeof(string); // Override for dates and known string columns
                }
                column.AllowDBNull = true;
            }

            // Copy all rows
            int rowsCopied = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                DataRow newRow = newTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    string columnName = column.ColumnName;
                    object value = row[columnName];

                    if (IsNullLike(value) || value.ToString().Trim() == "#N/A")
                    {
                        newRow[columnName] = DBNull.Value;
                    }
                    else if (newTable.Columns[columnName].DataType == typeof(double))
                    {
                        string valueAsString = value.ToString().Trim();
                        if (double.TryParse(valueAsString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedValue))
                        {
                            newRow[columnName] = parsedValue;
                        }
                        else
                        {
                            newRow[columnName] = DBNull.Value;
                        }
                    }
                    else
                    {
                        newRow[columnName] = value.ToString().Trim();
                    }
                }
                newTable.Rows.Add(newRow);
                rowsCopied++;
            }

            // Debug: Log the normalized table
            System.Diagnostics.Debug.WriteLine($"Normalized {newTable.Rows.Count} rows, {newTable.Columns.Count} columns (rows copied: {rowsCopied}).");
            foreach (DataColumn col in newTable.Columns)
            {
                System.Diagnostics.Debug.WriteLine($"Column: {col.ColumnName}, Type: {col.DataType}");
            }

            return newTable;
        }

        private bool IsNumericColumn(DataColumn column)
        {
            int validNumberCount = 0;
            int nonEmptyCount = 0;
            int sampleSize = Math.Min(50, column.Table.Rows.Count); // Increased sample size for accuracy

            for (int i = 0; i < sampleSize; i++)
            {
                object value = column.Table.Rows[i][column];
                if (!IsNullLike(value))
                {
                    string valueAsString = value.ToString().Trim();
                    if (valueAsString != "#N/A")
                    {
                        nonEmptyCount++;
                        // Strict numeric check: must parse as double and not look like an ID (e.g., integer-like strings)
                        if (double.TryParse(valueAsString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedValue) &&
                            !Regex.IsMatch(valueAsString, @"^\d+$")) // Exclude integer-like strings (e.g., "12345")
                        {
                            validNumberCount++;
                        }
                    }
                }
            }

            // Require 90% valid numbers to classify as numeric (stricter threshold)
            return nonEmptyCount > 0 && validNumberCount >= nonEmptyCount * 0.9;
        }


        protected void NormalizePercentageValues(DataTable dataTable)
        {
            var percentageColumns = GetPercentageColumns();

            foreach (DataColumn column in dataTable.Columns)
            {
                if (percentageColumns.Contains(column.ColumnName))
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!IsNullLike(row[column]))
                        {
                            if (double.TryParse(row[column].ToString(), out double value))
                            {
                                // Assume values > 1 are percentages (e.g., 3.4 -> 0.034)
                                if (value > 1)
                                {
                                    row[column] = value / 100.0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}