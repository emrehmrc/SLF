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
        public Önizleme()   // tum ekranların sekmeleri onizleme hata,bilgilendir vs..
        {
            InitializeComponent();
        }
        public DataGridView Onizleme_DataGrid1 { get { return Onizleme_dataGrid1; } }
        public DataGridView Onizleme_DataGrid2 { get { return Onizleme_dataGrid2; } }
        public DataGridView Onizleme_DataGrid3 { get { return Onizleme_dataGrid3; } }
        public DataGridView Onizleme_DataGrid4 { get { return Onizleme_dataGrid4; } }
        public DataGridView Onizleme_DataGrid5 { get { return Onizleme_dataGrid5; } }
        public Button Buton_YUKLE { get { return buton_YUKLE; } }
        public Button Buton_ÇIK { get { return buton_ÇIK; } }
        public Button Buton_İLERLE { get { return buton_İlerle; } }
        public TabPage Onizleme_Hata_Sekmesi { get { return Onizleme_Hata; } }
        public TabPage Onizleme_Warning_Sekmesi { get { return Onizleme_Warning; } }
        public TabPage Onizleme_Information_Sekmesi { get { return Onizleme_Information; } }
        public TabPage Onizleme_Statistics_Sekmesi { get { return Onizleme_Statistics; } }

        private void buton_İlerle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;       //dialog tum durumlar için tanımlı tekrar tanımlanabilir
        }

        private void buton_YUKLE_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buton_ÇIK_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("İşlemi iptal etmek istiyor musunuz? Bu veri tipi için yapılan işlemler kaybolacaktır.", "İşlem İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.DialogResult = DialogResult.Cancel;
        }

        private void Önizleme_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the close reason is the user clicking the close button
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Show a confirmation dialog
                DialogResult result = MessageBox.Show("İşlemi iptal etmek istiyor musunuz? Bu veri tipi için yapılan işlemler kaybolacaktır.", "İşlem İptali", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // If user chooses Yes, allow the form to close
                    e.Cancel = false;
                }
                else
                {
                    // If user chooses No, cancel the form closing
                    e.Cancel = true;
                }
            }
        }
    }
}
