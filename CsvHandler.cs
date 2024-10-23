using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    internal class CsvHandler
    {
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

