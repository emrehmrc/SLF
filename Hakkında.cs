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
    public partial class Hakkında : Form
    {

        public HomePageForm gir2;
        public Hakkında()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gir2 = (HomePageForm)Tag;
            gir2.Show();
            this.Hide();
        }

        private void Hakkında_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            HomePageForm hakkında_to_giris = new HomePageForm();
            hakkında_to_giris.Show();
        }
    }
}
