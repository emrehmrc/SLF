using System;
using System.Windows.Forms;

namespace SLF
{
    public partial class Tablo_Formu : Form
    {
        public DataGridView attribute_table;
        private ModülFormu parentForm; // Reference to the parent ModülFormu

        public Tablo_Formu(ModülFormu parent)
        {
            InitializeComponent();
            attribute_table = this.vektörel_attribute_table;
            parentForm = parent; // Store the reference to ModülFormu

        }


        private void Tablo_Formu_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void toolStripMenuItem_tablo_Click(object sender, EventArgs e)
        {
            if (attribute_table.SelectedRows.Count == 0) return;

            // Get the selected row
            DataGridViewRow selectedRow = attribute_table.SelectedRows[0];
            int rowIndex = Convert.ToInt32(selectedRow.Cells["RowIndex"].Value);

            // Call the parent form's method to zoom to the feature
            parentForm.ZoomToFeature(rowIndex);
        }
    }
}