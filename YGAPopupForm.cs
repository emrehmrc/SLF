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
            
            //this.FormClosing += YGAPopupForm_FormClosing;  // Handle form closing event
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
        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
            // Populate the userEnteredTable with values from the DataGridView
            /*            foreach (DataGridViewRow row in YGADataGridView.Rows)
                        {
                           // if (row.IsNewRow) continue;

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
                            newRow["IsDevelopmentArea"] = Convert.ToString(row.Cells["IsDevelopmentArea"].Value);
                            //newRow["lat"] = Convert.ToDouble(row.Cells["lat"].Value);
                            //newRow["lon"] = Convert.ToDouble(row.Cells["lon"].Value);
                            newRow["imar_tipi"] = row.Cells["imar_tipi"].Value.ToString();

                            dataTable.Rows.Add(newRow);
                        }*/

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

                // Add fields for the attributes with dynamic field types based on DataColumn data type
                foreach (DataColumn column in dataTable.Columns)
                {
                    FieldType fieldType = column.DataType == typeof(int) || column.DataType == typeof(long) ? FieldType.INTEGER_FIELD :
                                          column.DataType == typeof(double) || column.DataType == typeof(float) ? FieldType.DOUBLE_FIELD :
                                          FieldType.STRING_FIELD;

                    Field field = new Field
                    {
                        Name = column.ColumnName,
                        Type = fieldType,
                        Width = 50
                    };

                    shapefile.EditInsertField(field, shapefile.NumFields);
                }

                // Add polygons and attribute data
                foreach (DataRow row in dataTable.Rows)
                {
                    var shape = new MapWinGIS.Shape();
                    shape.Create(ShpfileType.SHP_POLYGON);

                    // Ensure coordinates are correctly formatted as WKT (Well-Known Text)
                    string coordinatesString = row["Coordinates"].ToString();
                    coordinatesString = coordinatesString.Replace("Polygon ((", "").Replace("))", ""); // Remove POLYGON (()) part
                    string[] coordinateStrings = coordinatesString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var coordinateString in coordinateStrings)
                    {
                        string[] coordinates = coordinateString.Trim().Split(' '); // Split by space to get lat, lon

                        if (coordinates.Length == 2 && double.TryParse(coordinates[0], out double lat) && double.TryParse(coordinates[1], out double lon))
                        {
                            shape.InsertPoint(new MapWinGIS.Point { x = lon, y = lat }, shape.numPoints);
                        }
                        else
                        {
                            MessageBox.Show($"Invalid coordinates format: {coordinateString}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue; // Skip invalid coordinates
                        }
                    }

                    // Insert the shape into the shapefile
                    bool shapeInserted = shapefile.EditInsertShape(shape, shapefile.NumShapes);
                    if (!shapeInserted)
                    {
                        MessageBox.Show($"Failed to insert shape: {shapefile.ErrorMsg[shapefile.LastErrorCode]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    // Insert attribute data into the shapefile
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


        // Save as shapefile method
        /*        private void SaveAsShapefile(DataTable dataTable, string filePath)
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

                            // Ensure coordinates are correctly formatted as lat, lon pairs
                            string coordinatesString = row["Coordinates"].ToString();
                            string[] coordinateStrings = coordinatesString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < coordinateStrings.Length; i += 2)  // Step by 2 since lat/lon are pairs
                            {
                                string latString = coordinateStrings[i];
                                string lonString = coordinateStrings[i + 1];

                                if (double.TryParse(latString, out double lat) && double.TryParse(lonString, out double lon))
                                {
                                    shape.InsertPoint(new MapWinGIS.Point { x = lon, y = lat }, shape.numPoints);
                                }
                                else
                                {
                                    MessageBox.Show($"Invalid coordinates format: {coordinateStrings[i]} {coordinateStrings[i + 1]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Skip invalid coordinates
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
                }*/
        // Get the updated DataTable from the form
        public DataTable GetUpdatedData()
        {
            return dataTable;
        }
        private void YGATableCancelButton_Click(object sender, EventArgs e)
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