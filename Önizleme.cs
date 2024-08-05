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
    }
}
