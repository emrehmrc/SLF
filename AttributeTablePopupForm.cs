using GMap.NET;
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
    public partial class AttributeTablePopupForm : Form
    {
        private List<PointLatLng> polygonPoints;

        public AttributeTablePopupForm(List<PointLatLng> polygonPoints)
        {
            InitializeComponent();
            this.polygonPoints = polygonPoints;
            PopulateTable();
        }

        // Populate the table with user input fields
        private void PopulateTable()
        {
            // Define the columns for the DataGridView
            if (dataGridView1.Columns.Count == 0)  // Avoid adding columns repeatedly
            {
                dataGridView1.Columns.Add("Latitude", "Latitude");
                dataGridView1.Columns.Add("Longitude", "Longitude");
                dataGridView1.Columns.Add("Parameter1", "Parameter1");
                dataGridView1.Columns.Add("Parameter2", "Parameter2");
            }

            // Add rows based on the polygon points
            foreach (var point in polygonPoints)
            {
                dataGridView1.Rows.Add(point.Lat, point.Lng, "", "");  // Example columns: Latitude, Longitude, Parameter1, Parameter2
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Capture user input and save or process as needed
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var latitude = row.Cells[0].Value;
                var longitude = row.Cells[1].Value;
                var parameter1 = row.Cells[2].Value;
                var parameter2 = row.Cells[3].Value;

                // Store or process data accordingly
            }

            MessageBox.Show("Parameters saved successfully!");
            this.Close();
        }
    }
}
