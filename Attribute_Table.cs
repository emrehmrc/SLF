using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class Tablo_Formu : Form
    {
        public DataGridView dataGridView_objesi;
        public Tablo_Formu()
        {
            InitializeComponent();
            dataGridView_objesi = this.dataGridView1;
        }

        private void Tablo_Formu_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
