using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLF.Services;

namespace SLF
{
    public class InvalidCsvHeadersException : Exception
    {
        public InvalidCsvHeadersException(string message) : base(message)
        {
        }
    }

    internal class CsvHandler
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
                    "DEK_DTR_ADI",
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


        public CsvHandler()
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
            for (int year = _yearService.PenultimateYear - 3; year <= _yearService.LastYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }

            for (int year = _yearService.PenultimateYear - 3; year <= _yearService.LastYear; year++)
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
            for (int year = _yearService.PenultimateYear - 1; year <= _yearService.LastYear; year++)
            {
                if (year > 0) // Geçerli bir yıl ise
                {
                    headers.Add($"YIL_DEMANT_{year}");
                    headers.Add($"YIL_TUKETIM_{year}");
                }
            }
        }

        public DataTable ImportCsvFile(string filePath, string seçilenVeriTipi = null)
        {
            DataTable dt = new DataTable();
            
            try
            {
                // Performans ölçümü
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                // Dosyanın varlığını kontrol et
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Dosya bulunamadı: {filePath}", "Dosya Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                
                // Dosyayı oku
                string[] lines = File.ReadAllLines(filePath);
                
                if (lines.Length == 0)
                {
                    MessageBox.Show("Dosya boş.", "Boş Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return dt;
                }
                
                // Başlık satırını ayrıştır
                string[] headers = ParseCsvLine(lines[0]);
                
                // Eğer veri tipi belirtilmişse başlıkları doğrula
                if (!string.IsNullOrEmpty(seçilenVeriTipi) && _expectedHeadersMap.ContainsKey(seçilenVeriTipi))
                {
                    ValidateColumnHeaders(headers, seçilenVeriTipi);
                }
                
                // Sütunları ekle
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Trim());
                }
                
                // Veri satırlarını ekle
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;
                        
                    string[] data = ParseCsvLine(lines[i]);
                    DataRow row = dt.NewRow();
                    
                    for (int j = 0; j < data.Length && j < headers.Length; j++)
                    {
                        row[j] = data[j].Trim();
                    }
                    
                    dt.Rows.Add(row);
                }
                
                stopwatch.Stop();
                Debug.WriteLine($"CSV dosyası içe aktarımı {stopwatch.ElapsedMilliseconds} ms sürdü");
                
                return dt;
            }
            catch (InvalidCsvHeadersException ex)
            {
                MessageBox.Show(ex.Message, "Geçersiz CSV Formatı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Dosya okunurken bir izin hatası oluştu.", "Dosya Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Dosya okunurken bir girdi/çıktı hatası oluştu: {ex.Message}", "Dosya Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilinmeyen bir hata oluştu: {ex.Message}", "Dosya Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private void ValidateColumnHeaders(string[] actualHeaders, string seçilenVeriTipi)
        {
            List<string> expectedHeaders = _expectedHeadersMap[seçilenVeriTipi];
            
            if (actualHeaders.Length != expectedHeaders.Count)
            {
                throw new InvalidCsvHeadersException(
                    $"Sütun sayıları uyuşmuyor.\nBeklenen: {expectedHeaders.Count}\nMevcut: {actualHeaders.Length}"
                );
            }
            
            bool headerMismatch = false;
            var invalidColumnMessage = new StringBuilder("Sütun adları uyuşmuyor.\n");
            
            for (int i = 0; i < actualHeaders.Length; i++)
            {
                string columnHeader = actualHeaders[i];
                if (string.IsNullOrEmpty(columnHeader) || !expectedHeaders.Contains(columnHeader))
                {
                    headerMismatch = true;
                    string expectedHeader = expectedHeaders[i];
                    invalidColumnMessage.AppendLine($"{i+1}. sütun:\tBeklenen: {expectedHeader}\tMevcut: {columnHeader}");
                }
            }
            
            if (headerMismatch)
            {
                throw new InvalidCsvHeadersException(invalidColumnMessage.ToString());
            }
        }

        // CSV satırını doğru şekilde ayrıştırmak için yardımcı metod
        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder field = new StringBuilder();
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Ardışık çift tırnak, tek tırnak karakteri olarak değerlendirilir
                        field.Append('"');
                        i++;  // Bir sonraki karakteri atla
                    }
                    else
                    {
                        // Tırnak durumunu değiştir
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // Alan sınırı, yeni bir alana geç
                    result.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    // Normal karakter, alana ekle
                    field.Append(c);
                }
            }
            
            // Son alanı ekle
            result.Add(field.ToString());
            
            return result.ToArray();
        }
        
        public void ExportCsvFile(string filePath, DataTable dt)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header line
                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));
                    
                    // Write the data rows
                    foreach (DataRow row in dt.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field =>
                        {
                            string fieldString = field.ToString();
                            // Enclose the field in quotes if it contains a comma
                            return fieldString.Contains(",") ? $"\"{fieldString}\"" : fieldString;
                        });
                        writer.WriteLine(string.Join(",", fields));
                    }
                }
                
                MessageBox.Show("Dosya başarıyla kaydedildi.", "Dosya Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Dosya kaydedilirken bir izin hatası oluştu. Dosyanın kaydedileceği klasörün yazma iznine sahip olduğundan emin olun.", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Dosya kaydedilirken bir girdi/çıktı hatası oluştu: {ex.Message}", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilinmeyen bir hata oluştu: {ex.Message}", "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}