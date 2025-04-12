using GMap.NET;
using GMap.NET.WindowsForms;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class Fonksiyon_Oluştur : Form
    {
        public ModülFormu mod1;
        public List<string> agrege_olacak_sutunlar = new List<string>();
        public List<object> tum_sutunlar_listesi = new List<object>();
        public List<object> secilen_sutunlar_listesi = new List<object>();

        // acquires the active gmapcontrol object that the join operation will be done on.
        public GMap.NET.WindowsForms.GMapControl activeGMapControl;

        public Fonksiyon_Oluştur()
        {
            InitializeComponent();
        }

        private async void buton_jabl_Click(object sender, EventArgs e)
        {
            foreach(string cols in secilen_sutunlar_fonksiyonForm.Items)
            {
                agrege_olacak_sutunlar.Add(cols);
            }

            mod1 = Tag as ModülFormu;
            mod1.firstLayerName = comboBox_fonksiyonlar_1.Text;
            mod1.secondLayerName = comboBox_fonksiyonlar_2.Text;
            Cursor = Cursors.WaitCursor;

            // either do a "jabl" or "jabl-summary"
            if(checkBox_cell_statistics.Checked == false)
            {
                await mod1.cbs.JoinAttributesByLocation(activeGMapControl);
            }
            else
            {
                await mod1.cbs.JoinAttributesByLocation_summary(activeGMapControl);
            }

            Cursor = Cursors.Default;
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_cell_statistics.Checked == true)
            {
                Height = 700;
            }
            else
            {
                Height = 240;
            }
        }

        // tum sutunlar listesi
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (tum_sutunlar_fonksiyonForm.SelectedItems != null && tum_sutunlar_fonksiyonForm.Items.Count != 0)
            {
                foreach (var selectedItem in tum_sutunlar_fonksiyonForm.SelectedItems)
                {
                    if(!tum_sutunlar_listesi.Contains(selectedItem))
                    {
                        tum_sutunlar_listesi.Add(selectedItem);
                    }   
                }

                foreach (var item in tum_sutunlar_listesi)
                {
                    if (!secilen_sutunlar_fonksiyonForm.Items.Contains(item))
                    {
                        secilen_sutunlar_fonksiyonForm.Items.Add(item);
                        tum_sutunlar_fonksiyonForm.Items.Remove(item);
                    }             
                    
                }
                secilen_sutunlar_fonksiyonForm.ClearSelected();
                tum_sutunlar_fonksiyonForm.ClearSelected();
                tum_sutunlar_listesi.Clear();
                secilen_sutunlar_listesi.Clear();
            }
        }

        // secilen sutun listesi
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (secilen_sutunlar_fonksiyonForm.SelectedItems != null && secilen_sutunlar_fonksiyonForm.Items.Count != 0)
            {
                foreach (var selectedItem in secilen_sutunlar_fonksiyonForm.SelectedItems)
                {
                    if (!secilen_sutunlar_listesi.Contains(selectedItem))
                    {
                        secilen_sutunlar_listesi.Add(selectedItem);
                    }
   
                }

                foreach (var item in secilen_sutunlar_listesi)
                {
                    secilen_sutunlar_fonksiyonForm.Items.Remove(item);

                    if (!tum_sutunlar_fonksiyonForm.Items.Contains(item))
                    {
                        tum_sutunlar_fonksiyonForm.Items.Add(item);
                    }
                  
                }
                secilen_sutunlar_fonksiyonForm.ClearSelected();
                tum_sutunlar_fonksiyonForm.ClearSelected();
                tum_sutunlar_listesi.Clear();
                secilen_sutunlar_listesi.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void comboBox_fonksiyonlar_2_TextChanged(object sender, EventArgs e)
        {
            mod1 = (ModülFormu)Tag;

            // Find the first item that matches the search text
            string selected_table_name = mod1.cbs.tüm_katmanlar_array_names
                .FirstOrDefault(name => name.Contains(comboBox_fonksiyonlar_2.Text));

            int index = Array.IndexOf(mod1.cbs.tüm_katmanlar_array_names, selected_table_name);

            // create an example row so that the columns of the second table could be displayed
            // in the list box
            DataRow example_row = mod1.cbs.tüm_katmanlar_datatable[index].NewRow();

            secilen_sutunlar_fonksiyonForm.ClearSelected();
            tum_sutunlar_fonksiyonForm.ClearSelected();
            tum_sutunlar_listesi.Clear();
            secilen_sutunlar_listesi.Clear();
            tum_sutunlar_fonksiyonForm.Items.Clear();
            secilen_sutunlar_fonksiyonForm.Items.Clear();

            foreach (var columns in example_row.Table.Columns)
            {
                tum_sutunlar_fonksiyonForm.Items.Add(columns.ToString());
            }
        }
    }
}
