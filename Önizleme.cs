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
    public partial class Önizleme : Form
    {
        public Önizleme()
        {
            InitializeComponent();
        }
        public DataGridView Onizleme_DataGrid1 { get { return Onizleme_dataGrid1;} }
        public DataGridView Onizleme_DataGrid2 { get { return Onizleme_dataGrid2;} }
        public DataGridView Onizleme_DataGrid3 { get { return Onizleme_dataGrid3;} }
        public DataGridView Onizleme_DataGrid4 { get { return Onizleme_dataGrid4;} }
        public DataGridView Onizleme_DataGrid5 { get { return Onizleme_dataGrid5;} }
        public Button Buton_YUKLE { get { return buton_YUKLE; } }
        public Button Buton_ÇIK { get { return buton_ÇIK; } }
        public Button Buton_İLERLE { get { return buton_İlerle; } }

        private void buton_İlerle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
        }

        private void buton_YUKLE_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buton_ÇIK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
