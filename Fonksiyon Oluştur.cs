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
        }

        private async void buton_jabl_Click(object sender, EventArgs e)
        {
            mod1 = (ModülFormu)Tag;
            mod1.firstLayerName = comboBox_fonksiyonlar_1.Text;
            mod1.secondLayerName = comboBox_fonksiyonlar_2.Text;
            this.Cursor = Cursors.WaitCursor;
            await mod1.JoinAttributesByLocation();
            this.Cursor = Cursors.Default;
        }
    }
}
