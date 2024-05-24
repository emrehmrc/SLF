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
        public Hakkında mod2;

        public GirişFormu()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mod1 = new ModülFormu();
            mod1.Tag = this;
            mod1.Show();
            this.Hide();
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            Yardım yardım_formu = new Yardım();
            yardım_formu.Show();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            mod2 = new Hakkında();
            mod2.Tag = this;
            mod2.Show();
            this.Hide();
        }
    }
}
