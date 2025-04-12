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
    public partial class DEK : Form
    {
        public DataTable FiltrelenmisSonuc { get; private set; }

        public List<string> secilenYillar { get; set; }

        private DataTable _orijinalTablo;

        public event EventHandler<FiltreEventArgs> FiltrelemeYapildi;
        public DEK(DataTable dt)
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

        private void button1_Click(object sender, EventArgs e)
        {
            // 2. Yıl Listesi
            secilenYillar = new List<string>();
            for (int i = 1; i < checkedListBox1.Items.Count; i++) // 0. index = Hepsi
            {
                if (checkedListBox1.GetItemChecked(i))
                    secilenYillar.Add(checkedListBox1.Items[i].ToString());
            }

            

            var filtreli = _orijinalTablo.AsEnumerable().Where(row =>
                (secilenYillar.Count == 0 || secilenYillar.Contains(row.Field<string>("year")))
                );

            FiltrelenmisSonuc = filtreli.Any() ? filtreli.CopyToDataTable() : _orijinalTablo.Clone();

            FiltrelemeYapildi?.Invoke(this, new FiltreEventArgs
            {
                dt = FiltrelenmisSonuc
            });
        }

        public void DrawMap(DataTable dekTable, GMapOverlay overlay, GMapControl gMapControl1)
        {
            overlay.Markers.Clear();
            overlay.Polygons.Clear();

            foreach (DataRow dek in dekTable.Rows)
            {


                // Hücre sınırları
                double top = Convert.ToDouble(dek["Top"]);
                double bottom = Convert.ToDouble(dek["Bottom"]);
                double left = Convert.ToDouble(dek["Left"]);
                double right = Convert.ToDouble(dek["Right"]);

                // Trafo konumlarını tutacak liste
                List<PointLatLng> markerKonumlari = new List<PointLatLng>();

                PointLatLng konum = new PointLatLng((top + bottom) / 2, (left + right) / 2);


                
                GMarkerGoogleType markerType = GMarkerGoogleType.red_dot;
                
                

                string tooltip = $"Durumu: {dek["durum"]}";

                var marker = new GMarkerGoogle(konum, markerType)
                {
                    ToolTipText = tooltip,
                    Tag = dek["id"]
                };  

                overlay.Markers.Add(marker);
            }                

                gMapControl1.Overlays.Add(overlay);
            
        }
        }
    }
