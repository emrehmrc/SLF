using System.Windows.Forms;

namespace SLF
{
    public partial class Tablo_Formu : Form
    {
        public DataGridView attribute_table;
        public Tablo_Formu()
        {
            InitializeComponent();
            attribute_table = this.vektörel_attribute_table;

        }

        private void Tablo_Formu_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
