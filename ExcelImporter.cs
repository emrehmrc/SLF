using OfficeOpenXml;
using SLF.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

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

        // Excel Importer'dan kopyalanacak temel başlıklar dizini
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
                    "TRAFO_KODU",
                    "TRAFO_MULKIYET",
                    "TRAFO_KAPASITESI",
                    "TRAFO_X_KOORDINAT",
                    "TRAFO_Y_KOORDINAT",
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
                    "DEK_KURULUM_YERI",
                    "DEK_BAGLANDIGI_TRAFO_KODU",
                }
            },
            {
                "Ekonometrik Yük Tahmini Verileri",
                new List<string> {
                    "YIL",
                    "GDP_BUYUME_ORANI",
                    "ILCE_NUFUS",
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
                    "HDD"
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
            // Son 2 yıl için tüketim kolonları ekle
            for (int year = _yearService.slfStartYear - 2; year <= _yearService.slfStartYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }

            // Son 2 yıl için talep kolonları ekle
            for (int year = _yearService.slfStartYear - 2; year <= _yearService.slfStartYear; year++)
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
            // Son 2 yıl için talep ve tüketim kolonları ekle
            for (int year = _yearService.slfStartYear - 2; year <= _yearService.slfStartYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_DEMANT_{year}");
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }
        }


        private void ValidateColumnHeaders(ExcelWorksheet worksheet, string seçilenVeriTipi)
        {
            int colCount = worksheet.Dimension.Columns;
            List<string> expectedHeaders = _expectedHeadersMap[seçilenVeriTipi];
            int expectedCount = expectedHeaders.Count;

            // Sütun sayılarını konsola yazdır (debug için)
            System.Diagnostics.Debug.WriteLine($"Beklenen sütun sayısı: {expectedCount}, Excel'deki mevcut: {colCount}");


            // Sadece mevcut sütunların kontrolü
            bool headerMismatch = false;
            var invalidColumnMessage = new StringBuilder("Sütun adları uyuşmuyor.\n");

            for (int col = 1; col <= Math.Min(colCount, expectedCount); col++)
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

        public DataTable ImportExcelFile(string filePath, string seçilenVeriTipi)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // First sheet, adjust if needed

                    // Validate headers
                    ValidateColumnHeaders(worksheet, seçilenVeriTipi);

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;
                    if (rowCount < 2 || colCount < 1)
                    {
                        throw new Exception("Excel dosyasında veri veya sütun başlığı bulunamadı.");
                    }

                    // Debug: Log worksheet dimensions
                    System.Diagnostics.Debug.WriteLine($"Worksheet dimensions: {rowCount} rows, {colCount} columns.");

                    // Create columns
                    for (int col = 1; col <= colCount; col++)
                    {
                        string columnName = worksheet.Cells[1, col].Text?.Trim() ?? $"Column{col}";
                        Type columnType = typeof(string); // Default to string
                        if (columnName == "ABONE_X_KOORDINAT" || columnName == "ABONE_Y_KOORDINAT")
                        {
                            columnType = typeof(double); // Enforce double for coordinate columns
                        }
                        dataTable.Columns.Add(new DataColumn
                        {
                            ColumnName = columnName,
                            DataType = columnType,
                            AllowDBNull = true
                        });
                    }

                    // Populate all rows
                    int rowsAdded = 0;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        bool rowHasData = false;

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cell = worksheet.Cells[row, col];
                            string valueAsString = cell.Value?.ToString()?.Trim();
                            string columnName = dataTable.Columns[col - 1].ColumnName;

                            if (string.IsNullOrEmpty(valueAsString) || valueAsString == "#N/A")
                            {
                                dataRow[col - 1] = DBNull.Value;
                            }
                            else if (columnName == "ABONE_X_KOORDINAT" || columnName == "ABONE_Y_KOORDINAT")
                            {
                                if (double.TryParse(valueAsString, out double value))
                                {
                                    dataRow[col - 1] = value;
                                    rowHasData = true;
                                }
                                else
                                {
                                    dataRow[col - 1] = DBNull.Value;
                                }
                            }
                            else
                            {
                                dataRow[col - 1] = valueAsString;
                                rowHasData = true;
                            }
                        }

                        if (rowHasData)
                        {
                            dataTable.Rows.Add(dataRow);
                            rowsAdded++;
                        }
                    }

                    // Debug: Log the imported table
                    System.Diagnostics.Debug.WriteLine($"Imported {dataTable.Rows.Count} rows, {dataTable.Columns.Count} columns (rows added: {rowsAdded}).");
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"Column: {col.ColumnName}, Type: {col.DataType}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel dosyasını okurken hata oluştu!!\n\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }

            return dataTable;
        }
    }
}