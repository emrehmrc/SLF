using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using static SLF.Optimal_DTR.Trafo;


namespace SLF.Optimal_DTR
{
    
    internal class ReadDB
    {
        public static List<Trafo> trafoData;

        public static List<Hucre> HucreData;


        public static void Maindatabase(string sqlitePath, string tableName)
        {

            //sqlitePath = @"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\veriler.db";   // SQLite veritabanı dosya yolu
            //tableName = "trafotest07031040";                      // SQLite tablo adı

            trafoData = new List<Trafo> ();

            trafoData = ReadDataFromSQLite(sqlitePath, tableName);


        }

        public static void Hucreverioku(string sqlitePath, string tableName)
        {

            //sqlitePath = @"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\Database.db";   // SQLite veritabanı dosya yolu
            //tableName = "Hucre";                      // SQLite tablo adı

            HucreData = ReadCellFromSQLite(sqlitePath, tableName);


        }

        public static List<Trafo> ReadDataFromSQLite(string sqlitePath, string tableName)
        {
            var dataList = new List<Trafo>();

            using (var connection = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
            {
                connection.Open();

                string selectQuery = $"SELECT * FROM {tableName};";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    int sayac = 0;
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("okunuyor");


                        while (reader.Read())
                        {
                            var trafo = new Trafo(
                                reader.IsDBNull(reader.GetOrdinal("trafo_id")) ? "Bilinmiyor" : reader.GetString(reader.GetOrdinal("trafo_id")),

                                reader.IsDBNull(reader.GetOrdinal("merkez_hucre")) ? 0 : reader.GetInt32(reader.GetOrdinal("merkez_hucre")),

                                /*reader.IsDBNull(reader.GetOrdinal("bos_kapasite_negatifler0")) ? 0.0 :
                                    (double.IsNaN(reader.GetDouble(reader.GetOrdinal("bos_kapasite_negatifler0"))) ? 0.0 : reader.GetDouble(reader.GetOrdinal("bos_kapasite_negatifler0"))),
                                */
                                reader.IsDBNull(reader.GetOrdinal("sahip")) ? "Bilinmiyor" : reader.GetString(reader.GetOrdinal("sahip")),

                                reader.IsDBNull(reader.GetOrdinal("year")) ? (short)0 : reader.GetInt16(reader.GetOrdinal("year")),

                                reader.IsDBNull(reader.GetOrdinal("Durum")) ? "Bilinmiyor" : reader.GetString(reader.GetOrdinal("Durum"))
                            );

                            dataList.Add(trafo);
                            sayac++;
                        }

                        
                    }
                }
            }

            return dataList;
        }

        public static List<Hucre> ReadCellFromSQLite(string sqlitePath, string tableName)
        {
            var dataList = new List<Hucre>();

            using (var connection = new SQLiteConnection($"Data Source={sqlitePath};Version=3;"))
            {
                connection.Open();

                string selectQuery = $"SELECT * FROM {tableName};";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    int sayac = 0;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = new Hucre(
                                reader.GetInt32(reader.GetOrdinal("id")),
                                reader.GetDouble(reader.GetOrdinal("left")),
                                reader.GetDouble(reader.GetOrdinal("top")),
                                reader.GetDouble(reader.GetOrdinal("right")),
                                reader.GetDouble(reader.GetOrdinal("bottom"))


                            );

                            dataList.Add(data);
                            sayac++;

                        }
                    }
                }
            }

            return dataList;
        }
    }
}
