using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Bibliography;
using SLF.Optimal_DTR;

namespace SLF.RaporlamaDosyası.FiltrelemeArayuz
{

    public partial class Alansal : Form
    {
        public DataTable FiltrelenmisSonuc { get; private set; }

        public List<string> secilenYillar { get; set; }

        private DataTable _orijinalTablo;

        public event EventHandler<FiltreEventArgs> FiltrelemeYapildi;
        public Alansal(DataTable dt)
        {
            InitializeComponent();
            YillariYerlestir();
            _orijinalTablo = dt;
        }

        private void YillariYerlestir()
        {
            this.checkedListBox1.Items.Clear();

            this.checkedListBox1.Items.Add("Hepsi");

            int ilkYilInt = 0;
            int sonYilInt = 0;

            try
            {
                if(FormManager.Form2Instance == null)
                {
                    ilkYilInt = DateTime.Now.Year; // Varsayılan olarak 5 yıl öncesi
                    sonYilInt = DateTime.Now.Year+10; // Şu anki yıl
                }
                else
                {

                    ilkYilInt = int.Parse(FormManager.Form2Instance.İlkYıl);
                    sonYilInt = int.Parse(FormManager.Form2Instance.SonYıl);
                                        
                }
                
            }

            catch
            {
                ilkYilInt = DateTime.Now.Year;
                sonYilInt = DateTime.Now.Year + 10;
            }


            for (int year = ilkYilInt-1; year <= sonYilInt; year++)
            {
                this.checkedListBox1.Items.Add(year.ToString());
            }

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Eğer "Hepsi" (index 0) işaretleniyorsa
            if (e.Index == 0)
            {
                // Hepsi işaretleniyorsa tümünü işaretle
                bool check = (e.NewValue == CheckState.Checked);

                // İşlemi event tamamlandıktan sonra yapmamız gerekiyor
                /*this.BeginInvoke((MethodInvoker)(() =>
                {
                    for (int i = 1; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.SetItemChecked(i, check);
                    }
                }));
                */
                for (int i = 1; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, check);
                }
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



            var filtreli = _orijinalTablo.AsEnumerable().Where(row =>
                (secilenYillar.Count == 0 || secilenYillar.Contains(row.Field<string>("year")))
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
                    secilenYillar.Count == 0 ||
                    secilenYillar.Contains(Convert.ToInt32(row.Field<long>("year")))
                )

            .ToList();

            FiltrelenmisSonuc = filtrelenmisData.Any() ? filtrelenmisData.CopyToDataTable() : _orijinalTablo.Clone();


            FiltrelemeYapildi?.Invoke(this, new FiltreEventArgs
            {
                dt = FiltrelenmisSonuc
            });

        }
    }
}
