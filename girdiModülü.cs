using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLF
{
    public class NoFileSelectedException : Exception // dosyanın yuklenme durumları mesajları
    {
        public NoFileSelectedException(string message) : base(message)
        {
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
        protected readonly List<string> veri_listesi_requires_xlsx = new List<string> {  // 2. ekran list kosullar tanımlı birbirine baglı olan moduller ekle olup olmadıgı kontrolu yapılıyor
            "EA Şarj Verileri",
            "Ekonometrik Yük Tahmini Verileri",
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

        protected readonly List<string> nullLikeStrings = new List<string> // doluluk bosluk check kısımları kontrolu yapılıyor
        {
            "",
            "null",
            "N/A",
            "#N/A"
        };

        protected string seçilenVeriTipi;
        protected int slfStartYear;
        protected int slfEndYear;
        public int SlfStartYear
        {
            get { return slfStartYear; }
            set { slfStartYear = value; }
        }

        public int SlfEndYear
        {
            get { return slfEndYear; }
            set { slfEndYear = value; }
        }
        protected virtual List<string> Prerequisites { get; } = new List<string>(); 

        protected const int HoursInYear = 8760;
        protected int lastYear
        {
            get { return slfStartYear - 1; }
        }
        protected int penultimateYear
        {
            get { return slfStartYear - 2; }
        }
        protected int horizonYear
        {
            get { return slfStartYear; }
        }
        //protected int lastYear; = DateTime.Now.Year - 1;
        //protected readonly int penultimateYear = DateTime.Now.Year - 2;

        protected const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        protected const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        protected const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        protected const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        protected const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        protected readonly string combinedExcelFilter;
        protected readonly string combinedCsvFilter;
        protected readonly string combinedTabularFilter;

        protected DataTable currentDataTable = new DataTable();
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

        protected bool IsNullLike(object value, bool isZero=false)
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

        public void VEERReport(string seçilenVeriTipi) {
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
        public bool VEERProcess(string seçilenVeriTipi)
        {
            try
            {
                CheckPrerequisites(seçilenVeriTipi); // Check the required datatables for the given module
                // ProcessFileSelection metodu ile dosya seçme işlemi yapılır ve seçilen dosya veri tablosuna yüklenir
                ProcessFileSelection(seçilenVeriTipi);
                DataTable dataTable = CurrentDataTable;
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

                        // Exit the loop if there are no info or warning messages
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
                            break;
                        }

                        Remove();
                        ClearRows();
                        Validate();
                        Impute();
                    }
                    Postprocess();
                    ImportProcessedData();
                    ShowImportedMessage();
                    return true;
                }
                else
                {
                    MessageBox.Show("Dosya seçimi gerçekleştirilemedi.", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (NoFileSelectedException ex)
            {
                MessageBox.Show(ex.Message, "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidColumnHeadersException ex)
            {
                MessageBox.Show("Geçersiz sütun biçimi: " + ex.Message, "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (PrerequisiteException ex)
            {
                MessageBox.Show(ex.Message, "Önkoşul hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        protected void ImportProcessedData()
        {
            importedDataTable = currentDataTable.Copy();
            dataTablesByType[seçilenVeriTipi] = importedDataTable;
        }

        public void ProcessFileSelection(string seçilenVeriTipi)
        {
            this.seçilenVeriTipi = seçilenVeriTipi;
            var fileDialog1 = new OpenFileDialog
            {
                Title = FileDialogTitle
            };

            if (veri_listesi_requires_xlsx.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedExcelFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessExcelFile(selectedFileName, seçilenVeriTipi);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else if (veri_listesi_requires_csv.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedCsvFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessCsvFile(selectedFileName);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else if (veri_listesi_requires_tabular.Contains(seçilenVeriTipi))
            {
                fileDialog1.Filter = combinedTabularFilter;
                if (fileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = fileDialog1.FileName;
                    currentDataTable = ProcessTabularFile(selectedFileName);
                }
                else throw new NoFileSelectedException("Dosya seçimi gerçekleştirilemedi.");
            }
            else throw new NoFileSelectedException("Bu veri tipi için atanmış bir dosya seçimi prosedürü henüz yok.");
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

