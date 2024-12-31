using MapWinGIS;
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

            InitializeDataGridView();

            // Register event handlers directly in the constructor
            this.FormClosing += YGAPopupForm_FormClosing;  // Handle form closing event
            YGADataGridView.CellValueChanged += YGADataGridView_CellValueChanged;  // Handle DataGridView cell value changes
        }

        // Initialize the DataGridView with existing data from the DataTable
        private void InitializeDataGridView()
        {
            // Bind the existing DataTable to the DataGridView
            YGADataGridView.DataSource = dataTable;

            if (!YGADataGridView.Columns.Contains("imar_tipi"))
                return;

            // Remove the existing "imar_tipi" column (if already present as a text column)
            YGADataGridView.Columns.Remove("imar_tipi");

            // Create and add a new ComboBox column for "imar_tipi"
            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "imar_tipi", // Name must match the original column
                HeaderText = "Imar Tipi",
                DataSource = new List<string> { "Residential", "Commercial", "Industrial", "Mixed" },
                DataPropertyName = "imar_tipi", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };

            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn);
        }

        // Save button logic
        private void YGASaveButton_Click(object sender, EventArgs e)
        {
            // Populate the userEnteredTable with values from the DataGridView
            foreach (DataGridViewRow row in YGADataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                DataRow newRow = dataTable.NewRow();
                newRow["PolygonID"] = row.Cells["PolygonID"].Value.ToString();
                newRow["Coordinates"] = row.Cells["Coordinates"].Value.ToString();
                newRow["Area_Size(m2)"] = Convert.ToDouble(row.Cells["Area_Size(m2)"].Value);
                // Add additional columns to the new row
                newRow["yasakli_alan_percentage"] = row.Cells["yasakli_alan_percentage"].Value;
                newRow["taks"] = Convert.ToDouble(row.Cells["taks"].Value);
                newRow["baslangic_yili"] = Convert.ToInt32(row.Cells["baslangic_yili"].Value);
                newRow["ilce"] = row.Cells["ilce"].Value.ToString();
                //newRow["left"] = Convert.ToDouble(row.Cells["left"].Value);
                //newRow["right"] = Convert.ToDouble(row.Cells["right"].Value);
                newRow["IsDevelopmentArea"] = Convert.ToBoolean(row.Cells["IsDevelopmentArea"].Value);
                //newRow["lat"] = Convert.ToDouble(row.Cells["lat"].Value);
                //newRow["lon"] = Convert.ToDouble(row.Cells["lon"].Value);
                newRow["imar_tipi"] = row.Cells["imar_tipi"].Value.ToString();

                dataTable.Rows.Add(newRow);
            }

            // Save the DataTable or prompt to save as .shp
            using (var saveFileDialog = new SaveFileDialog { Filter = "Shapefile (*.shp)|*.shp" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveAsShapefile(dataTable, saveFileDialog.FileName);
                }
            }

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

        // Save as shapefile method
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
        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return dataTable;
        }
        private void YGACancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Handle DataGridView cell value changes (if needed)
        private void YGADataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Additional logic when cell value changes if needed
        }
    }
}

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