using System;
using System.Data;
using System.Windows.Forms;

namespace SLF
{
    public partial class YGAPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        public YGAPopupForm(DataTable existingDataTable)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView();

            // Register event handlers directly in the constructor
            this.FormClosing += YGAPopupForm_FormClosing;  // Handle form closing event
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;  // Handle DataGridView cell value changes
        }

        // Initialize the DataGridView with the data
        private void InitializeDataGridView()
        {
            dataGridView1.DataSource = dataTable;
        }

        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return dataTable;
        }

        // Handle Save button click
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Update the DataTable with user input
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                // Update the DataTable with new values from the DataGridView
                dataTable.Rows[row.Index]["PolygonID"] = row.Cells["PolygonID"].Value;
                dataTable.Rows[row.Index]["Coordinates"] = row.Cells["Coordinates"].Value;
                dataTable.Rows[row.Index]["Area_Size(m2)"] = row.Cells["Area_Size(m2)"].Value;
            }

            isOperationCancelled = false;
            MessageBox.Show("YGA Parameters saved successfully!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Handle form closing event
        private void YGAPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOperationCancelled)
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        // Handle DataGridView cell value changes (if needed)
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Perform any additional actions based on cell changes if needed
        }
    }
}
