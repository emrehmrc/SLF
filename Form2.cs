using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class formOznitelik : Form
    {
        public formOznitelik()
        {
            InitializeComponent();
        }

        // Öznitelikleri ayarlamak için bir metot
        public void SetOznitelikler(List<NoktaVeri> noktalar)
        {
            oznitelik.DataSource = noktalar;
        }

        private void oznitelik_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}