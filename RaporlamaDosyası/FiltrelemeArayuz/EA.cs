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
using GMap.NET.WindowsForms;
using GMap.NET;

namespace SLF.RaporlamaDosyası.FiltrelemeArayuz
{
    public partial class EA : Form
    {
        public DataTable FiltrelenmisSonuc { get; private set; }

        public List<string> secilenYillar { get; set; }

        public List<string> secilenDurumlar { get; set; }

        private DataTable _orijinalTablo;

        public event EventHandler<FiltreEventArgs> FiltrelemeYapildi;

        public EA(DataTable dt)
        {
            InitializeComponent();
            _orijinalTablo = dt;
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

        private void button1__Click(object sender, EventArgs e)
        {
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
                (secilenYillar.Count == 0 || secilenYillar.Contains(row.Field<string>("year"))) &&
                (secilenDurumlar.Count == 0 || secilenDurumlar.Contains(row.Field<string>("Durum")))

                );

            FiltrelenmisSonuc = filtreli.Any() ? filtreli.CopyToDataTable() : _orijinalTablo.Clone();

            FiltrelemeYapildi?.Invoke(this, new FiltreEventArgs
            {
                dt = FiltrelenmisSonuc
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Seçili yılları listeye al
            List<int> secilenYillar = new List<int>();
            List<string> secilenDurumlar = new List<string>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                if (item.ToString() != "Hepsi")
                {
                    secilenYillar.Add(int.Parse(item.ToString()));
                }

            }

            var filtrelenmisData = _orijinalTablo.AsEnumerable()
                .Where(row =>
                    (secilenYillar.Count == 0 ||
                    (int.TryParse(row.Field<string>("year"), out int year) && secilenYillar.Contains(year))) // Yıla göre filtreleme
                                )
            .ToList();

            FiltrelenmisSonuc = filtrelenmisData.Any() ? filtrelenmisData.CopyToDataTable() : _orijinalTablo.Clone();

            MessageBox.Show(FiltrelenmisSonuc.Rows.Count.ToString());
            
            FiltrelemeYapildi?.Invoke(this, new FiltreEventArgs
            {
                dt = FiltrelenmisSonuc
            });

        }
        public void DrawMap(DataTable eaTable, GMapOverlay overlay, GMapControl gMapControl1)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            foreach (DataRow ea in eaTable.Rows)
            {


                // Hücre sınırları
                double top = Convert.ToDouble(ea["Top"]);
                double bottom = Convert.ToDouble(ea["Bottom"]);
                double left = Convert.ToDouble(ea["Left"]);
                double right = Convert.ToDouble(ea["Right"]);

                // Trafo konumlarını tutacak liste
                List<PointLatLng> markerKonumlari = new List<PointLatLng>();

                PointLatLng konum = new PointLatLng((top + bottom) / 2, (left + right) / 2);

                string durum = ea["durum"].ToString();

                if (durum == "AC-HOME")
                {
                    GMarkerGoogleType markerType = GMarkerGoogleType.red_dot;
                }
                else if (durum == "AC-WORK")
                {
                    GMarkerGoogleType markerType = GMarkerGoogleType.blue_dot;
                }
                else if (durum == "AC-PUBLIC")
                {
                    GMarkerGoogleType markerType = GMarkerGoogleType.green_dot;
                }
                else if (durum == "DC-PUBLIC")
                {
                    GMarkerGoogleType markerType = GMarkerGoogleType.yellow_dot;



                    string tooltip = $"Durumu: {ea["durum"]}";

                    var marker = new GMarkerGoogle(konum, markerType)
                    {
                        ToolTipText = tooltip,
                        Tag = ea["id"]
                    };

                    overlay.Markers.Add(marker);
                }              

                gMapControl1.Overlays.Add(overlay);
            }
        }
    }
}
