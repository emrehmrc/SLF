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
    public partial class HomePageForm : Form
    {
        public ModülFormu mod1;
        public Hakkında mod2;

        public HomePageForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }


        private void StartButton_Click(object sender, EventArgs e)
        {
            MethodForm optionForm = new MethodForm();
            //this.Hide();
            optionForm.ShowDialog();
            this.Show();

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
