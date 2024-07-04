using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLF
{
    public class NoFileSelectedException : Exception
    {
        public NoFileSelectedException(string message) : base(message)
        {
        }
    }

    public class GirdiModülü
    {
        protected Önizleme onizleme1 = new Önizleme();
        protected readonly List<string> veri_listesi_requires_xlsx = new List<string> {
            "EA Şarj Verileri",
            "DTR Verileri",
            "Abone Verileri"
        };
        protected readonly List<string> veri_listesi_requires_csv = new List<string> { };
        protected readonly List<string> veri_listesi_requires_tabular = new List<string> { };

        protected readonly List<string> nullLikeStrings = new List<string>
        {
            "",
            "null",
            "N/A",
            "#N/A"
        };

        protected string seçilenVeriTipi;

        protected const int HoursInYear = 8760;
        protected readonly int lastYear = DateTime.Now.Year - 1;
        protected readonly int penultimateYear = DateTime.Now.Year - 2;

        protected const string FileDialogTitle = "Bir veri dosyası seçiniz.";
        protected const string FilterExcelFiles = "Excel dosyaları (*.xlsx)|*.xlsx";
        protected const string FilterCsvFiles = "CSV dosyaları (*.csv)|*.csv";
        protected const string FilterTabularFiles = "KML dosyaları (*.kml)|*.kml";
        protected const string FilterAllFiles = "Tüm dosyalar (*.*)|*.*";

        protected readonly string combinedExcelFilter;
        protected readonly string combinedCsvFilter;
        protected readonly string combinedTabularFilter;

        protected DataTable currentDataTable = new DataTable();
        protected DataTable errorDataTable = new DataTable();
        protected DataTable warningDataTable = new DataTable();
        protected DataTable infoDataTable = new DataTable();

        protected Dictionary<string, List<int>> columnNullRowsMap = new Dictionary<string, List<int>>();
        protected Dictionary<string, List<int>> imputableRowsMap = new Dictionary<string, List<int>>();
        protected Dictionary<string, (double X, double Y)> binaIdToMostFrequentCoordinates = new Dictionary<string, (double X, double Y)>();
        protected Dictionary<string, string> aboneGrubuMostFrequent = new Dictionary<string, string>();
        //protected Dictionary<string, (double X, double Y)> binaIdToAverageCoordinates = new Dictionary<string, (double X, double Y)>();

        protected const int COORDINATE_ROUNDING_PRECISION = 3;
        protected const float MAX_THRESHOLD = float.MaxValue;
        protected const float MIN_THRESHOLD = float.MinValue;
        protected static (float Min, float Max) WARNING_ONLY = (MIN_THRESHOLD, MAX_THRESHOLD);
        protected static (float Min, float Max) INFO_ONLY = (MAX_THRESHOLD, MAX_THRESHOLD);
        protected static (float Min, float Max) ERROR_ONLY = (MIN_THRESHOLD, MIN_THRESHOLD);

        protected bool IsNullLike(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return true;
            }

            string stringValue = value.ToString();
            return nullLikeStrings.Contains(stringValue, StringComparer.OrdinalIgnoreCase);
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
            warningDataTable.Columns.Add("İmpütasyon", typeof(bool));
            onizleme1.Onizleme_DataGrid2.DataSource = errorDataTable;
            onizleme1.Onizleme_DataGrid3.DataSource = warningDataTable;
            onizleme1.Onizleme_DataGrid4.DataSource = infoDataTable;
            onizleme1.Onizleme_DataGrid1.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid2.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid3.AllowUserToAddRows = false;
            onizleme1.Onizleme_DataGrid4.AllowUserToAddRows = false;
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

        public virtual void Validate()
        {
            ClearRows();
        }


        public virtual void Remove()
        {
        
        }

        public virtual void Impute()
        {
        
        }

        protected void ClearRows()
        {
            errorDataTable.Rows.Clear();
            warningDataTable.Rows.Clear();
            infoDataTable.Rows.Clear();
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

