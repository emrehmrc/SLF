//using Excel = Microsoft.Office.Interop.Excel; // Alias for the Excel namespace
using OfficeOpenXml; // Import the EPPlus library
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

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
        Dictionary<string, List<string>> expectedHeadersMap = new Dictionary<string, List<string>>
        {
            {
                "Abone Verileri",
                    new List<string> {
                        "TESISAT_NO",
                        "X_KOORDINAT",
                        "Y_KOORDINAT",
                        "ADR_BINA_ID",
                        "bina_turu",
                        "ADR_ILCE_ID",
                        "ENERJI_TABLO_KAYIT_KODU",
                        "BAGLANTI_GUCU",
                        "SOZ_DURUM",
                        "ABONE_GRUBU",
                        "GERILIM_SEVIYESI",
                        "SOZ_BAS_TARIH",
                        "SOZ_BIT_TARIH",
                        "2019_Tuketim",
                        "2020_Tuketim",
                        "2021_Tuketim",
                        "2022_Tuketim",
                        "2023_Tuketim",
                        "2019_Demant",
                        "2020_Demant",
                        "2021_Demant",
                        "2022_Demant",
                        "2023_Demant",
                    }
            },
            { "EA Şarj Verileri", new List<string> { 
                "ISTASYON_ADI", "ISTASYON_TIPI", "ISTASYON_GUCU", "EA_TRAFO_KODU","EA_X_KOORDINAT","EA_Y_KOORDINAT"
            } },
            {
                "DTR Verileri", new List<string> {
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
                    "YIL_DEMANT_2021",
                    "YIL_TUKETIM_2021",
                    "YIL_DEMANT_2022",
                    "YIL_TUKETIM_2022",
                    "YIL_DEMANT_2023",
                    "YIL_TUKETIM_2023"
            }
            },

            {
                "DEK Verileri", new List<string> {
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
            // Add more data types and their expected headers as needed
        };

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
            int colCount = worksheet.Dimension.Columns;
            List<string> expectedHeaders = expectedHeadersMap[seçilenVeriTipi];
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

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
