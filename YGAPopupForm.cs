using System;
using System.Collections.Generic;
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
            InitializeDataTable();
            InitializeDataGridView();


            // Register event handlers directly in the constructor
            this.FormClosing += YGAPopupForm_FormClosing;  // Handle form closing event
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;  // Handle DataGridView cell value changes
        }
        private void InitializeDataGridView()
        {


            // Set combo box for 'imar_tipi' column
            if (dataGridView1.Columns["imar_tipi"] is DataGridViewComboBoxColumn comboBoxColumn)
            {
                comboBoxColumn.DataSource = new List<string> { "Residential", "Commercial", "Industrial", "Mixed" };
            }
            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;

        }
        /*        // Initialize the DataGridView with the data
        private void InitializeDataGridView()
        {
            dataGridView1.DataSource = dataTable;
        }
*/

        private void InitializeDataTable()
        {
            dataGridView1.DataSource = dataTable;


            // New columns
            dataTable.Columns.Add("yasakli_alan_percentage", typeof(double));
            dataTable.Columns.Add("id", typeof(int));
            dataTable.Columns.Add("left", typeof(double));
            dataTable.Columns.Add("top", typeof(double));
            dataTable.Columns.Add("right", typeof(double));
            dataTable.Columns.Add("bottom", typeof(double));
            dataTable.Columns.Add("IsDevelopmentArea", typeof(bool));
            dataTable.Columns.Add("lat", typeof(double));
            dataTable.Columns.Add("lon", typeof(double));
            dataTable.Columns.Add("imar_tipi", typeof(string));           // ComboBox values
            dataTable.Columns.Add("ilce", typeof(string));
            dataTable.Columns.Add("taks", typeof(double));
            dataTable.Columns.Add("agirlik/hiz", typeof(double));
            dataTable.Columns.Add("baslangic_yili", typeof(int));

            // Optional: Add initial rows if necessary
            // dataTable.Rows.Add(...);
        }



        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return dataTable;
        }

        /*        // Handle Save button click
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
                }*/
        private void YGASaveButton_Click(object sender, EventArgs e)
        {
            // Update the DataTable with user input
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                // Update the DataTable with new values from the DataGridView
                dataTable.Rows[row.Index]["PolygonID"] = row.Cells["PolygonID"].Value;
                dataTable.Rows[row.Index]["Coordinates"] = row.Cells["Coordinates"].Value;
                dataTable.Rows[row.Index]["Area_Size(m2)"] = row.Cells["Area_Size(m2)"].Value;

                // New columns added by user
                dataTable.Rows[row.Index]["yasakli_alan_percentage"] = row.Cells["yasakli_alan_percentage"].Value;
                dataTable.Rows[row.Index]["id"] = row.Cells["id"].Value;
                dataTable.Rows[row.Index]["left"] = row.Cells["left"].Value;
                dataTable.Rows[row.Index]["top"] = row.Cells["top"].Value;
                dataTable.Rows[row.Index]["right"] = row.Cells["right"].Value;
                dataTable.Rows[row.Index]["bottom"] = row.Cells["bottom"].Value;
                dataTable.Rows[row.Index]["IsDevelopmentArea"] = row.Cells["IsDevelopmentArea"].Value;
                dataTable.Rows[row.Index]["lat"] = row.Cells["lat"].Value;
                dataTable.Rows[row.Index]["lon"] = row.Cells["lon"].Value;
                dataTable.Rows[row.Index]["imar_tipi"] = row.Cells["imar_tipi"].Value;
                dataTable.Rows[row.Index]["ilce"] = row.Cells["ilce"].Value;
                dataTable.Rows[row.Index]["taks"] = row.Cells["taks"].Value;
                dataTable.Rows[row.Index]["agirlik/hiz"] = row.Cells["agirlik/hiz"].Value;
                dataTable.Rows[row.Index]["baslangic_yili"] = row.Cells["baslangic_yili"].Value;
            }
/*            // Example validation before saving
            foreach (DataGridViewCell cell in dataGridView1.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }
            }
*/
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
