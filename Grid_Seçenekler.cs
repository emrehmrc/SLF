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
    public partial class Grid_Seçenekler : Form
    {
        public ModülFormu modül;
        public TabControl Modül_Tabları;
        public TabPage tab_stokastik;
        public TabPage tab_ea;
        public Grid_Seçenekler()
        {
            InitializeComponent();
            
    }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                combobox_grid_sizes.Enabled = false;
            }
            else
            {
                combobox_grid_sizes.Enabled = true;
            }
        }

        private void grid_tamam_Click(object sender, EventArgs e)
        {
            // ModülFormu'nun aktif tabını kontrol ediyoruz
            modül = (ModülFormu)Tag;

            // Seçilen grid boyutunu ayarlıyoruz
            if (combobox_grid_sizes.SelectedIndex == 0)
            {
                modül.cbs.grid_size = 100;
            }
            else if (combobox_grid_sizes.SelectedIndex == 1)
            {
                modül.cbs.grid_size = 250;
            }
            else if (combobox_grid_sizes.SelectedIndex == 2)
            {
                modül.cbs.grid_size = 400;
            }
            else
            {
                modül.cbs.grid_size = 1000;
            }

            modül.Show();
            this.Close();

            modül.cbs.isSelecting_grid = true;
            modül.cbs.GetActiveGMapControl().Cursor = Cursors.Arrow;

        }

    }

}

