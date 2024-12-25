using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SLF
{
    public partial class YGAPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        private DataTable userEnteredTable;

        public YGAPopupForm(DataTable existingDataTable)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView();
            InitializeUserEnteredTable();

            this.FormClosing += YGAPopupForm_FormClosing;
            YGADataGridView.CellValueChanged += YGADataGridView_CellValueChanged;
        }

        // Initialize the DataGridView with existing data from the DataTable
        private void InitializeDataGridView()
        {
            // Bind the existing DataTable to the DataGridView
            YGADataGridView.DataSource = dataTable;

            // Ensure the 'imar_tipi' column exists and is a ComboBoxColumn
            if (YGADataGridView.Columns["imar_tipi"] is DataGridViewComboBoxColumn comboBoxColumn)
            {
                // Set the items for the ComboBox
                comboBoxColumn.DataSource = new List<string> { "Residential", "Commercial", "Industrial", "Mixed" };

                // Optionally, set the default selected item, if desired
                foreach (DataGridViewRow row in YGADataGridView.Rows)
                {
                    if (row.Cells["imar_tipi"].Value == null || string.IsNullOrEmpty(row.Cells["imar_tipi"].Value.ToString()))
                    {
                        row.Cells["imar_tipi"].Value = "Residential";  // Default selection
                    }
                }
            }
            else
            {
                // Log an error or show a message if the column is not found or not a ComboBox
                MessageBox.Show("The 'imar_tipi' column is not a ComboBox column or is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUserEnteredTable()
        {
            userEnteredTable = dataTable.Clone();  // Create a copy of the schema (columns) from the existing dataTable

            // Add any additional user-entered columns if necessary, only if they don't already exist
            if (!userEnteredTable.Columns.Contains("yasakli_alan_percentage"))
            {
                userEnteredTable.Columns.Add("yasakli_alan_percentage", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("id"))
            {
                userEnteredTable.Columns.Add("id", typeof(int));
            }
            if (!userEnteredTable.Columns.Contains("taks"))
            {
                userEnteredTable.Columns.Add("taks", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("baslangic_yili"))
            {
                userEnteredTable.Columns.Add("baslangic_yili", typeof(int));
            }
            if (!userEnteredTable.Columns.Contains("ilce"))
            {
                userEnteredTable.Columns.Add("ilce", typeof(string));
            }
            if (!userEnteredTable.Columns.Contains("left"))
            {
                userEnteredTable.Columns.Add("left", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("right"))
            {
                userEnteredTable.Columns.Add("right", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("IsDevelopmentArea"))
            {
                userEnteredTable.Columns.Add("IsDevelopmentArea", typeof(bool));
            }
            if (!userEnteredTable.Columns.Contains("lat"))
            {
                userEnteredTable.Columns.Add("lat", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("lon"))
            {
                userEnteredTable.Columns.Add("lon", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("imar_tipi"))
            {
                userEnteredTable.Columns.Add("imar_tipi", typeof(string));  // ComboBox values
            }
        }


            private void YGASaveButton_Click(object sender, EventArgs e)
            {
                // Populate the userEnteredTable with values from the DataGridView
                foreach (DataGridViewRow row in YGADataGridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    DataRow newRow = userEnteredTable.NewRow();
                    newRow["PolygonID"] = row.Cells["PolygonID"].Value.ToString();
                    newRow["Coordinates"] = row.Cells["Coordinates"].Value.ToString();
                    newRow["Area_Size(m2)"] = Convert.ToDouble(row.Cells["Area_Size(m2)"].Value);

                    // Add additional columns to the new row
                    newRow["yasakli_alan_percentage"] = row.Cells["yasakli_alan_percentage"].Value;
                    newRow["id"] = Convert.ToInt32(row.Cells["id"].Value);
                    newRow["taks"] = Convert.ToDouble(row.Cells["taks"].Value);
                    newRow["baslangic_yili"] = Convert.ToInt32(row.Cells["baslangic_yili"].Value);
                    newRow["ilce"] = row.Cells["ilce"].Value.ToString();
                    newRow["left"] = Convert.ToDouble(row.Cells["left"].Value);
                    newRow["right"] = Convert.ToDouble(row.Cells["right"].Value);
                    newRow["IsDevelopmentArea"] = Convert.ToBoolean(row.Cells["IsDevelopmentArea"].Value);
                    newRow["lat"] = Convert.ToDouble(row.Cells["lat"].Value);
                    newRow["lon"] = Convert.ToDouble(row.Cells["lon"].Value);
                    newRow["imar_tipi"] = row.Cells["imar_tipi"].Value.ToString();

                    userEnteredTable.Rows.Add(newRow);
                }

                // Prompt to save the data to a shapefile
                using (var saveFileDialog = new SaveFileDialog { Filter = "Shapefile (*.shp)|*.shp" })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveAsShapefile(userEnteredTable, saveFileDialog.FileName);
                    }
                }

                MessageBox.Show("YGA Parameters saved successfully!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            private void SaveAsShapefile(DataTable dataTable, string filePath)
            {
                try
                {
                    // Create a new Shapefile object
                    Shapefile shapefile = new Shapefile();
                    if (!shapefile.CreateNew(filePath, ShpfileType.SHP_POLYGON))
                    {
                        MessageBox.Show($"Failed to create shapefile: {shapefile.ErrorMsg[shapefile.LastErrorCode]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Add fields for the attributes
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Field field = new Field
                        {
                            Name = column.ColumnName,
                            Type = FieldType.STRING_FIELD,
                            Width = 50
                        };
                        shapefile.EditInsertField(field, shapefile.NumFields);
                    }

                    // Add polygons and attribute data
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var shape = new MapWinGIS.Shape();
                        shape.Create(ShpfileType.SHP_POLYGON);

                        // Here, assuming that the coordinates are stored as "Coordinates" column in WKT format
                        string[] coordinateStrings = row["Coordinates"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var coordinateString in coordinateStrings)
                        {
                            var coordinates = coordinateString.Trim('(', ')').Split(' ');
                            if (coordinates.Length == 2 && double.TryParse(coordinates[0], out double lat) && double.TryParse(coordinates[1], out double lon))
                            {
                                shape.InsertPoint(new MapWinGIS.Point { x = lon, y = lat }, shape.numPoints);
                            }
                        }

                        // Insert the shape into the shapefile
                        bool shapeInserted = shapefile.EditInsertShape(shape, shapefile.NumShapes);
                        if (!shapeInserted)
                        {
                            MessageBox.Show($"Failed to insert shape: {shapefile.ErrorMsg[shapefile.LastErrorCode]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Insert attributes
                        int shapeIndex = shapefile.NumShapes - 1;
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            shapefile.EditCellValue(i, shapeIndex, row[i]?.ToString());
                        }
                    }

                    // Save the shapefile
                    shapefile.SaveAs(filePath);
                    shapefile.Close();

                    MessageBox.Show($"Shapefile saved successfully at: {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving shapefile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void YGACancelButton_Click(object sender, EventArgs e)
            {
                this.Close();
            }

            private void YGAPopupForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (isOperationCancelled)
                {
                    MessageBox.Show("Operation cancelled.");
                }
            }
        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return userEnteredTable;
        }
        private void YGADataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
            {
                // Additional logic when cell value changes (if needed)
            }
        }
    }




/*using System;
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

        // New table to hold the user-entered data
        private DataTable userEnteredTable;

        public YGAPopupForm(DataTable existingDataTable)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView();

            // Initialize the new DataTable to store user-entered values
            InitializeUserEnteredTable();

            // Register event handlers directly in the constructor
            this.FormClosing += YGAPopupForm_FormClosing;  // Handle form closing event
            YGADataGridView.CellValueChanged += YGADataGridView_CellValueChanged;  // Handle DataGridView cell value changes
        }

        // Initialize the DataGridView with existing data from the DataTable
        private void InitializeDataGridView()
        {
            // Bind the existing DataTable to the DataGridView
            YGADataGridView.DataSource = dataTable;

            // Ensure the 'imar_tipi' column exists and is a ComboBoxColumn
            if (YGADataGridView.Columns["imar_tipi"] is DataGridViewComboBoxColumn comboBoxColumn)
            {
                // Set the items for the ComboBox
                comboBoxColumn.DataSource = new List<string> { "Residential", "Commercial", "Industrial", "Mixed" };

                // Optionally, set the default selected item, if desired
                foreach (DataGridViewRow row in YGADataGridView.Rows)
                {
                    if (row.Cells["imar_tipi"].Value == null || string.IsNullOrEmpty(row.Cells["imar_tipi"].Value.ToString()))
                    {
                        row.Cells["imar_tipi"].Value = "Residential";  // Default selection
                    }
                }
            }
            else
            {
                // Log an error or show a message if the column is not found or not a ComboBox
                MessageBox.Show("The 'imar_tipi' column is not a ComboBox column or is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Initialize the DataTable to store the user-entered data
        private void InitializeUserEnteredTable()
        {
            // Create a new DataTable for user-entered values
            userEnteredTable = dataTable.Clone();  // Create a copy of the schema (columns) from the existing dataTable
                                                   // Add any additional user-entered columns if necessary, only if they don't already exist
            if (!userEnteredTable.Columns.Contains("yasakli_alan_percentage"))
            {
                userEnteredTable.Columns.Add("yasakli_alan_percentage", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("id"))
            {
                userEnteredTable.Columns.Add("id", typeof(int));
            }
            if (!userEnteredTable.Columns.Contains("taks"))
            {
                userEnteredTable.Columns.Add("taks", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("baslangic_yili"))
            {
                userEnteredTable.Columns.Add("baslangic_yili", typeof(int));
            }
            if (!userEnteredTable.Columns.Contains("ilce"))
            {
                userEnteredTable.Columns.Add("ilce", typeof(string));
            }
            if (!userEnteredTable.Columns.Contains("left"))
            {
                userEnteredTable.Columns.Add("left", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("right"))
            {
                userEnteredTable.Columns.Add("right", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("IsDevelopmentArea"))
            {
                userEnteredTable.Columns.Add("IsDevelopmentArea", typeof(bool));
            }
            if (!userEnteredTable.Columns.Contains("lat"))
            {
                userEnteredTable.Columns.Add("lat", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("lon"))
            {
                userEnteredTable.Columns.Add("lon", typeof(double));
            }
            if (!userEnteredTable.Columns.Contains("imar_tipi"))
            {
                userEnteredTable.Columns.Add("imar_tipi", typeof(string));  // ComboBox values
            }
        }

        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return userEnteredTable;
        }

        // Handle Save button click
        private void YGASaveButton_Click(object sender, EventArgs e)
        {
            // Iterate through the rows in YGADataGridView
            foreach (DataGridViewRow row in YGADataGridView.Rows)
            {
                if (row.IsNewRow) continue;  // Skip the new row in the DataGridView

                // Create a new DataRow to add to the userEnteredTable
                DataRow newRow = userEnteredTable.NewRow();

                // Assign values from the existing DataTable (coordinates, area size)
                newRow["PolygonID"] = row.Cells["PolygonID"].Value.ToString();  // Ensure PolygonID is unique
                newRow["Coordinates"] = row.Cells["Coordinates"].Value.ToString();
                newRow["Area_Size(m2)"] = Convert.ToDouble(row.Cells["Area_Size(m2)"].Value);


                // Assign values from user-entered columns
                newRow["yasakli_alan_percentage"] = row.Cells["yasakli_alan_percentage"].Value;
                newRow["id"] = Convert.ToInt32(row.Cells["id"].Value);
                newRow["taks"] = Convert.ToDouble(row.Cells["taks"].Value);
                newRow["baslangic_yili"] = Convert.ToInt32(row.Cells["baslangic_yili"].Value);
                newRow["ilce"] = row.Cells["ilce"].Value.ToString();
                newRow["left"] = Convert.ToDouble(row.Cells["left"].Value);
                newRow["right"] = Convert.ToDouble(row.Cells["right"].Value);
                newRow["IsDevelopmentArea"] = Convert.ToBoolean(row.Cells["IsDevelopmentArea"].Value);
                newRow["lat"] = Convert.ToDouble(row.Cells["lat"].Value);
                newRow["lon"] = Convert.ToDouble(row.Cells["lon"].Value);
                newRow["imar_tipi"] = row.Cells["imar_tipi"].Value.ToString();  // ComboBox values

                // Add the new row to the userEnteredTable
                userEnteredTable.Rows.Add(newRow);
            }

            // Mark operation as successful
            isOperationCancelled = false;

            // Display success message
            MessageBox.Show("YGA Parameters saved successfully!");

            // Set the DialogResult and close the form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Handle Cancel button click
        private void YGACancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Handle form closing event
        private void YGAPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the operation was cancelled, show cancellation message
            if (isOperationCancelled)
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        // Handle DataGridView cell value changes (if needed)
        private void YGADataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // You can implement additional logic based on cell changes if needed
        }
    }
}
*/

/*using System;
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
            YGADataGridView.CellValueChanged += YGADataGridView_CellValueChanged;  // Handle DataGridView cell value changes
        }

        private void InitializeDataGridView()
        {
            // Bind the existing DataTable (attribute table) to the DataGridView
            YGADataGridView.DataSource = dataTable;
        }

        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return dataTable;
        }

        // Handle Save button click
private void YGASaveButton_Click(object sender, EventArgs e)
{
    // Iterate through the rows in YGADataGridView
    foreach (DataGridViewRow row in YGADataGridView.Rows)
    {
        if (row.IsNewRow) continue;  // Skip the new row in the DataGridView

        // Create a new DataRow to add to the DataTable
        DataRow newRow = dataTable.NewRow();

        // Assign the values from the DataGridView cells to the new DataRow
        newRow["PolygonID"] = row.Cells["PolygonID"].Value.ToString();  // Ensure PolygonID is unique
        newRow["Coordinates"] = row.Cells["Coordinates"].Value.ToString();
        newRow["Area_Size(m2)"] = Convert.ToDouble(row.Cells["Area_Size(m2)"].Value);

        // Add values from other columns in the DataGridView
        newRow["yasakli_alan_percentage"] = row.Cells["yasakli_alan_percentage"].Value;
        newRow["id"] = Convert.ToInt32(row.Cells["id"].Value);
        newRow["taks"] = Convert.ToDouble(row.Cells["taks"].Value);
        newRow["baslangic_yili"] = Convert.ToInt32(row.Cells["baslangic_yili"].Value);
        newRow["ilce"] = row.Cells["ilce"].Value.ToString();
        newRow["left"] = Convert.ToDouble(row.Cells["left"].Value);
        newRow["right"] = Convert.ToDouble(row.Cells["right"].Value);
        newRow["IsDevelopmentArea"] = Convert.ToBoolean(row.Cells["IsDevelopmentArea"].Value);
        newRow["lat"] = Convert.ToDouble(row.Cells["lat"].Value);
        newRow["lon"] = Convert.ToDouble(row.Cells["lon"].Value);
        newRow["imar_tipi"] = row.Cells["imar_tipi"].Value.ToString();  // ComboBox values

        // Add the new row to the DataTable
        dataTable.Rows.Add(newRow);
    }

    // Mark operation as successful
    isOperationCancelled = false;

    // Display success message
    MessageBox.Show("YGA Parameters saved successfully!");

    // Set the DialogResult and close the form
    this.DialogResult = DialogResult.OK;
    this.Close();
}


        private void YGACancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Handle form closing event
        private void YGAPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the operation was cancelled, show cancellation message
            if (isOperationCancelled)
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        // Handle DataGridView cell value changes (if needed)
        private void YGADataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // You can implement additional logic based on cell changes if needed
        }
    }
}



*/