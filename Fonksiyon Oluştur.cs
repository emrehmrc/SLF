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

        public Fonksiyon_Oluştur()
        {
            InitializeComponent();
            this.Height = 240;
            this.MaximumSize = new System.Drawing.Size(height: 700, width: this.Width);
        }

        private async void buton_jabl_Click(object sender, EventArgs e)
        {
            foreach(string cols in secilen_sutunlar.Items)
            {
                agrege_olacak_sutunlar.Add(cols);
            }

            mod1 = (ModülFormu)Tag;
            mod1.firstLayerName = comboBox_fonksiyonlar_1.Text;
            mod1.secondLayerName = comboBox_fonksiyonlar_2.Text;
            this.Cursor = Cursors.WaitCursor;

            // either do a "jabl" or "jabl-summary"
            if(checkBox_cell_statistics.Checked == false)
            {
                await mod1.JoinAttributesByLocation();
            }
            else
            {
                await mod1.JoinAttributesByLocation_summary();
            }

            this.Cursor = Cursors.Default;
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_cell_statistics.Checked == true)
            {
                this.Height = 700;
            }
            else
            {
                this.Height = 240;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(tum_sutunlar.SelectedItems != null)
            {
                foreach (var selected_items in tum_sutunlar.SelectedItems)
                {
                    secilen_sutunlar.Items.Add(selected_items);
                }
                tum_sutunlar.SelectedItems.Clear();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (secilen_sutunlar.SelectedItems != null)
            {
                foreach (var selected_items in secilen_sutunlar.SelectedItems)
                {
                    secilen_sutunlar.Items.Remove(selected_items);
                }
                secilen_sutunlar.SelectedItems.Clear();
            }
        }
    }
}
