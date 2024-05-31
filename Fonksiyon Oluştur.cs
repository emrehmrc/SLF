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

        public Fonksiyon_Oluştur()
        {
            InitializeComponent();
            comboBox_fonksiyonlar_1.Items.Add("dklsjglsd");
        }

        private void button1_Click(object sender, EventArgs e)
        {
           mod1.firstLayerName = comboBox_fonksiyonlar_1.Text;
           mod1.secondLayerName = comboBox_fonksiyonlar_2.Text;
           mod1.JoinAttributesByLocation();
        }

        private void fonksiyon_listesi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(fonksiyon_listesi.SelectedIndex == 0)
            {
                mod1 = (ModülFormu)Tag;

                if (mod1.tüm_katmanlar_array_names != null)
                {
                    foreach (var layers in mod1.tüm_katmanlar_array_names)
                    {
                        if (layers != null)
                        {
                            comboBox_fonksiyonlar_1.Items.Add(layers);
                            comboBox_fonksiyonlar_2.Items.Add(layers);
                        }
                    }
                }
            }
        }
    }
}
