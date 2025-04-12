using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace SLF.RaporlamaDosyası.FiltrelemeArayuz
{
    public partial class DTR : Form
    {
        public string mulkiyet { get; set; }

        public List<string> secilenYillar { get; set; }

        public List<string> secilenDurumlar { get; set; }

        public DataTable FiltrelenmisSonuc { get; private set; }

        private DataTable _orijinalTablo;

        public event EventHandler<FiltreEventArgs> FiltrelemeYapildi;


        public DTR(DataTable orijinalTable)
        {
            InitializeComponent();
            _orijinalTablo = orijinalTable;

        }

        public void DrawMap(DataTable trafoTable, GMapOverlay overlay, GMapControl gMapControl1)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            foreach (DataRow trafo in trafoTable.Rows)
            {
                int hucreId = Convert.ToInt32(trafo["merkez_hucre"]);

                // Bu hücreye ait trafoları filtrele
                var trafolar = trafoTable.AsEnumerable()
                    .Where(t => Convert.ToInt32(t["merkez_hucre"]) == hucreId)
                    .ToList();

                if (trafolar.Count == 0)
                    continue;

                // Hücre sınırları
                double top = Convert.ToDouble(trafo["Top"]);
                double bottom = Convert.ToDouble(trafo["Bottom"]);
                double left = Convert.ToDouble(trafo["Left"]);
                double right = Convert.ToDouble(trafo["Right"]);

                // Trafo konumlarını tutacak liste
                List<PointLatLng> markerKonumlari = new List<PointLatLng>();

                if (trafolar.Count == 1)
                {
                    markerKonumlari.Add(new PointLatLng((top + bottom) / 2, (left + right) / 2));
                }
                else if (trafolar.Count <= 4)
                {
                    markerKonumlari.Add(new PointLatLng(top, left));
                    markerKonumlari.Add(new PointLatLng(top, right));
                    markerKonumlari.Add(new PointLatLng(bottom, left));
                    markerKonumlari.Add(new PointLatLng(bottom, right));
                }
                else
                {
                    int satirSayisi = (int)Math.Ceiling(Math.Sqrt(trafolar.Count));
                    int sutunSayisi = (int)Math.Ceiling((double)trafolar.Count / satirSayisi);

                    double latStep = (top - bottom) / (satirSayisi + 1);
                    double lngStep = (right - left) / (sutunSayisi + 1);

                    for (int i = 1; i <= satirSayisi; i++)
                    {
                        for (int j = 1; j <= sutunSayisi; j++)
                        {
                            if (markerKonumlari.Count >= trafolar.Count)
                                break;

                            double lat = bottom + (i * latStep);
                            double lng = left + (j * lngStep);
                            markerKonumlari.Add(new PointLatLng(lat, lng));
                        }
                    }
                }

                for (int i = 0; i < trafolar.Count; i++)
                {
                    var t = trafolar[i];
                    PointLatLng konum = markerKonumlari[i % markerKonumlari.Count];

                    string owner = trafo["sahip"].ToString();
                    GMarkerGoogleType markerType = owner == "Özel" ? GMarkerGoogleType.red_dot : GMarkerGoogleType.blue_dot;

                    string tooltip = $"TrafoID: {t["trafo_id"]}\n" +
                                     $"Owner: {t["sahip"]}\n" +
                                     $"Year: {t["year"]}\n" +
                                     $"Trafo Durumu: {t["Durum"]}";

                    var marker = new GMarkerGoogle(konum, markerType)
                    {
                        ToolTipText = tooltip,
                        Tag = trafo["trafo_id"]
                    };

                    overlay.Markers.Add(marker);
                }
            }           

            gMapControl1.Overlays.Add(overlay);
        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Eğer "Hepsi" (index 0) işaretleniyorsa
            if (e.Index == 0)
            {
                // Hepsi işaretleniyorsa tümünü işaretle
                bool check = (e.NewValue == CheckState.Checked);

                // İşlemi event tamamlandıktan sonra yapmamız gerekiyor
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    for (int i = 1; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.SetItemChecked(i, check);
                    }
                }));
            }
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Eğer "Hepsi" (index 0) işaretleniyorsa
            if (e.Index == 0)
            {
                // Hepsi işaretleniyorsa tümünü işaretle
                bool check = (e.NewValue == CheckState.Checked);

                // İşlemi event tamamlandıktan sonra yapmamız gerekiyor
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    for (int i = 1; i < checkedListBox2.Items.Count; i++)
                    {
                        checkedListBox2.SetItemChecked(i, check);
                    }
                }));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // 1. Mülkiyet
            mulkiyet = "";
            if (radioButton2.Checked)
                mulkiyet = "Kurum";
            else if (radioButton3.Checked)
                mulkiyet = "Özel";
            // Hepsi seçiliyse boş bırak (filtreleme yapma)

            // 2. Yıl Listesi
            secilenYillar = new List<string>();
            for (int i = 1; i < checkedListBox1.Items.Count; i++) // 0. index = Hepsi
            {
                if (checkedListBox1.GetItemChecked(i))
                    secilenYillar.Add(checkedListBox1.Items[i].ToString());
            }

            // 3. Trafo Durumu
            secilenDurumlar = new List<string>();
            for (int i = 1; i < checkedListBox2.Items.Count; i++) // 0. index = Hepsi
            {
                if (checkedListBox2.GetItemChecked(i))
                    secilenDurumlar.Add(checkedListBox2.Items[i].ToString());
            }

            var filtreli = _orijinalTablo.AsEnumerable().Where(row =>
                (string.IsNullOrEmpty(mulkiyet) || row.Field<string>("sahip") == mulkiyet) &&
                (secilenYillar.Count == 0 || secilenYillar.Contains(row.Field<string>("year"))) &&
                (secilenDurumlar.Count == 0 || secilenDurumlar.Contains(row.Field<string>("Durum")))
            );

            FiltrelenmisSonuc = filtreli.Any() ? filtreli.CopyToDataTable() : _orijinalTablo.Clone();

            FiltrelemeYapildi?.Invoke(this, new FiltreEventArgs
            {
                dt = FiltrelenmisSonuc
            });
        }
    }
}
