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
            
            if (combobox_grid_sizes.SelectedIndex == 0)
            {
                modül = (ModülFormu)Tag;
                modül.grid_size = 100;
                modül.Show();
                this.Close();
                modül.gMapControl_stokastik.Overlays.Remove(modül.bounding_box_overlay);
                modül.CreateAndAddGridToMap();
            }
            else if (combobox_grid_sizes.SelectedIndex == 1)
            {
                modül = (ModülFormu)Tag;
                modül.grid_size = 250;
                modül.Show();
                this.Close();
                modül.gMapControl_stokastik.Overlays.Remove(modül.bounding_box_overlay);
                modül.CreateAndAddGridToMap();
            }
            else if (combobox_grid_sizes.SelectedIndex == 2)
            {
                modül = (ModülFormu)Tag;
                modül.grid_size = 400;
                modül.Show();
                this.Close();
                modül.gMapControl_stokastik.Overlays.Remove(modül.bounding_box_overlay);
                modül.CreateAndAddGridToMap();
            }
            else
            {
                modül = (ModülFormu)Tag;
                modül.grid_size = 1000;
                modül.Show();
                this.Close();
                modül.gMapControl_stokastik.Overlays.Remove(modül.bounding_box_overlay);
                modül.CreateAndAddGridToMap();
            }
        }

    }
}
