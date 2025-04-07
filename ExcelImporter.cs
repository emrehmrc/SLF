using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using SLF.Services;

namespace SLF
{
    public class InvalidColumnHeadersException : Exception
    {
        public InvalidColumnHeadersException(string message) : base(message)
        {
        }
    }

    internal class ExcelImporter
    {
        private YearService _yearService;
        private Dictionary<string, List<string>> _expectedHeadersMap;

        // Dictionary'yi dinamik oluşturmak için temel başlıklar
        private readonly Dictionary<string, List<string>> _baseHeadersMap = new Dictionary<string, List<string>>
        {
            {
                "Abone Verileri",
                new List<string> {
                    "TESISAT_NO",
                    "ABONE_X_KOORDINAT",
                    "ABONE_Y_KOORDINAT",
                    "BINA_ID",
                    "BINA_TURU",
                    "ABONE_ILCE_ID",
                    "BAGLANDIGI_TRAFO_KODU",
                    "BAGLANTI_GUCU",
                    "SOZLESME_DURUMU",
                    "ABONE_GRUBU",
                    "GERILIM_SEVIYESI",
                    "ABONE_BASLANGIC_TARIHI",
                    "ABONE_BITIS_TARIHI",
                    // Dinamik yıl alanları oluşturulacak
                }
            },
            {
                "EA Şarj Verileri",
                new List<string> {
                    "ISTASYON_ADI",
                    "ISTASYON_TIPI",
                    "ISTASYON_GUCU",
                    "EA_TRAFO_KODU",
                    "EA_X_KOORDINAT",
                    "EA_Y_KOORDINAT"
                }
            },
            {
                "DTR Verileri",
                new List<string> {
                    "TRAFO_ID",
                    "TRAFO_KODU",
                    "TRAFO_ILCE_ADI",
                    "TRAFO_MAHALLE_ADI",
                    "TRAFO_MULKIYET",
                    "FIDER_ADI",
                    "TRAFO_KAPASITESI",
                    "TM_ID",
                    "TM_FIDER_ID",
                    "TRAFO_X_KOORDINAT",
                    "TRAFO_Y_KOORDINAT",
                    "TRAFO_ADI",
                    "TRAFO_KURULUM_TARIHI",
                    "PRIMER_GERILIM",
                    "SEKONDER_GERILIM",
                    // Dinamik yıl alanları oluşturulacak
                }
            },
            {
                "DEK Verileri",
                new List<string> {
                    "ILCE_ADI",
                    "KAYNAK_TIPI",
                    "DEK_KURULU_GUCU",
                    "DEK_X_KOORDINAT",
                    "DEK_Y_KOORDINAT",
                    "DEK_TM_ADI",
                    "DEK_KURULUM_YERI",
                    "DEK_BAGLANDIGI_TRAFO_KODU",
                }
            },
            {
                "Ekonometrik Yük Tahmini Verileri",
                new List<string> {
                    "YIL",
                    "GDP_GROWTH",
                    "ULKE_NUFUS",
                    "BOLGE_NUFUS",
                    "KKO",
                    "KKM",
                    "MESKEN_DAGITILAN",
                    "SANAYI_DAGITILAN",
                    "TICARETHANE_DAGITILAN",
                    "TARIMSAL_SULAMA_DAGITILAN",
                    "AYDINLATMA_DAGITILAN",
                    "TOPLAM_DAGITILAN",
                    "MESKEN_FATURALANAN",
                    "SANAYI_FATURALANAN",
                    "TICARETHANE_FATURALANAN",
                    "TARIMSAL_SULAMA_FATURALANAN",
                    "AYDINLATMA_FATURALANAN",
                    "TOPLAM_FATURALANAN",
                    "MESKEN_ABONE_SAYISI",
                    "SANAYI_ABONE_SAYISI",
                    "TICARETHANE_ABONE_SAYISI",
                    "TARIMSAL_SULAMA_ABONE_SAYISI",
                    "AYDINLATMA_ABONE_SAYISI",
                    "TOPLAM_ABONE_SAYISI",
                    "BOLGE_YAZ_PUANT",
                    "BOLGE_KIS_PUANT",
                    "GRP",
                    "GRP_TARIMSAL_URETIM",
                    "GRP_SANAYI_URETIM",
                    "GRP_HIZMET_URETIM",
                    "GRP_INSAAT_URETIM",
                    "GRP_TARIMSAL_URETIM_%",
                    "GRP_SANAYI_URETIM_%",
                    "GRP_HIZMET_URETIM_%",
                    "GRP_INSAAT_URETIM_%",
                    "GDP",
                    "GDP_TARIMSAL_URETIM",
                    "GDP_SANAYI_URETIM",
                    "GDP_HIZMET_URETIM",
                    "GDP_INSAAT_URETIM",
                    "GDP_TARIMSAL_URETIM_%",
                    "GDP_SANAYI_URETIM_%",
                    "GDP_HIZMET_URETIM_%",
                    "GDP_INSAAT_URETIM_%",
                    "CDD",
                    "HDD",
                    "ULKE_NUFUS_%",
                    "BOLGE_NUFUS_%",
                    "EA_Talep",
                    "DEK_Uretim",
                    "Other"
                }
            },
            {
                "Fider Verileri",
                new List<string> {
                    "FIDER_TM_ADI",
                    "FIDER_ADI",
                    "FIDER_ID",
                    "FIDER_TARIH",
                    "FIDER_SAAT",
                    "FIDER_DEMANT",
                }
            },
            {
                "TM Verileri",
                new List<string> {
                    "EDW_TM_ID",
                    "EDW_TRAFO_ID",
                    "EDW_TARIH",
                    "EDW_TM_TUKETIM",
                    "EDW_TM_URETIM",
                }
            },
            {
                "Enerji Müsaadeleri Verileri",
                new List<string> {
                    "ENERJI_MUSAADE_NO",
                    "ENERJI_MUSAADE_ABONE_GRUBU",
                    "ENERJI_MUSAADE_ABONE_FAALIYET_KATEGORI",
                    "ENERJI_MUSAADE_TALEP_DURUMU",
                    "ENERJI_MUSAADE_GERILIM_SEVIYESI",
                    "ENERJI_MUSAADE_MUSTAKIL_TRAFO_BOOL",
                    "ENERJI_MUSAADE_BAGLANACAGI_TRAFO_ID",
                    "ENERJI_MUSAADE_BAGLANTI_GUCU",
                    "ENERJI_MUSAADE_IL",
                    "ENERJI_MUSAADE_ILCE",
                    "ENERJI_MUSAADE_MAHALLE",
                    "ENERJI_MUSAADE_ENERJILENDIRME_YILI",
                    "ENERJI_MUSAADE_BASVURU_TARIHI",
                    "ENERJI_MUSAADE_X_KOORDINAT",
                    "ENERJI_MUSAADE_Y_KOORDINAT",
                }
            },
            {
                "Yeni Projelendirilmiş DTR Verileri",
                new List<string> {
                    "PROJELENDIRILMIS_TRAFO_ID",
                    "PROJELENDIRILMIS_TRAFO_PROJE_KODU",
                    "PROJELENDIRILMIS_TRAFO_PROJE_ADI",
                    "PROJELENDIRILMIS_TRAFO_YATIRIM_SINIFI",
                    "PROJELENDIRILMIS_TRAFO_KAPASITE",
                    "PROJELENDIRILMIS_TRAFO_YENI_KAPASITE",
                    "PROJELENDIRILMIS_TRAFO_YATIRIM_YILI",
                    "PROJELENDIRILMIS_TRAFO_X_KOORDINAT",
                    "PROJELENDIRILMIS_TRAFO_Y_KOORDINAT",
                }
            }
        };

        public ExcelImporter()
        {
            _yearService = YearService.GetInstance();
            InitializeExpectedHeaders();

            // YearService'deki değişiklikleri dinleyerek başlıkları güncelleme
            _yearService.OnYearChanged += (sender, args) => {
                InitializeExpectedHeaders();
            };
        }

        // Dinamik başlıkları yeniden oluşturma metodu
        private void InitializeExpectedHeaders()
        {
            _expectedHeadersMap = new Dictionary<string, List<string>>();

            foreach (var entry in _baseHeadersMap)
            {
                string dataType = entry.Key;
                List<string> headers = new List<string>(entry.Value);

                // Veri tipine göre yıl alanlarını ekle
                if (dataType == "Abone Verileri")
                {
                    AddYearColumnsToAboneVerileri(headers);
                }
                else if (dataType == "DTR Verileri")
                {
                    AddYearColumnsToDTRVerileri(headers);
                }

                _expectedHeadersMap[dataType] = headers;
            }
        }

        // Abone verileri için yıl kolonlarını ekleme
        private void AddYearColumnsToAboneVerileri(List<string> headers)
        {
            // Son 5 yıl için tüketim ve talep kolonları ekle
            for (int year = _yearService.PenultimateYear - 3; year <= _yearService.lastYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }

            for (int year = _yearService.PenultimateYear - 3; year <= _yearService.lastYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_DEMANT_{year}");
                }
            }
        }

        // DTR verileri için yıl kolonlarını ekleme
        private void AddYearColumnsToDTRVerileri(List<string> headers)
        {
            // Son 3 yıl için talep ve tüketim kolonları ekle
            for (int year = _yearService.PenultimateYear - 1; year <= _yearService.lastYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_DEMANT_{year}");
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }
        }

        public DataTable ImportExcelFile(string filePath, string seçilenVeriTipi)
        {
            DataTable dataTable = new DataTable();

            // Example of measuring import time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

                // Validate column headers. In case it fails, it throws an exception.
                ValidateColumnHeaders(worksheet, seçilenVeriTipi);

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Create columns in DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    DataColumn column = new DataColumn();
                    column.ColumnName = worksheet.Cells[1, col].Text;
                    dataTable.Columns.Add(column);
                }

                // Populate DataTable with Excel data
                // Row starts from 2 because 1st row is column headers
                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Excel file import took: {stopwatch.ElapsedMilliseconds} ms");

            return dataTable;
        }

        private void ValidateColumnHeaders(ExcelWorksheet worksheet, string seçilenVeriTipi)
        {
            if (!_expectedHeadersMap.ContainsKey(seçilenVeriTipi))
            {
                throw new InvalidColumnHeadersException(
                    $"Tanımlanmamış veri tipi: {seçilenVeriTipi}"
                );
            }

            int colCount = worksheet.Dimension.Columns;
            List<string> expectedHeaders = _expectedHeadersMap[seçilenVeriTipi];
            int expectedCount = expectedHeaders.Count;

            if (colCount != expectedCount)
            {
                throw new InvalidColumnHeadersException(
                    $"Sütun sayıları uyuşmuyor.\nBeklenen: {expectedCount}\nMevcut: {colCount}"
                );
            }

            bool headerMismatch = false;
            var invalidColumnMessage = new StringBuilder("Sütun adları uyuşmuyor.\n");

            for (int col = 1; col <= colCount; col++)
            {
                string columnHeader = worksheet.Cells[1, col].Text;
                if (string.IsNullOrEmpty(columnHeader) || !expectedHeaders.Contains(columnHeader))
                {
                    headerMismatch = true;
                    string expectedHeader = expectedHeaders[col - 1];
                    invalidColumnMessage.AppendLine($"{col}. sütun:\tBeklenen: {expectedHeader}\tMevcut: {columnHeader}");
                }
            }

            if (headerMismatch)
            {
                throw new InvalidColumnHeadersException(invalidColumnMessage.ToString());
            }
        }
    }
}