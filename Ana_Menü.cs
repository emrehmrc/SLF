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
    public partial class GirişFormu : Form
    {
        public ModülFormu mod1;

        public GirişFormu()
        {
            InitializeComponent();
            mod1 = new ModülFormu();
            mod1.Tag = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mod1.Show();
            this.Hide();
        }

        private void GirişFormu_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void roundButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
