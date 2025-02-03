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

            if (!YGADataGridView.Columns.Contains("density"))
                return;

            // Remove the existing "imar_tipi" column (if already present as a text column)
            YGADataGridView.Columns.Remove("density");

            if (!YGADataGridView.Columns.Contains("saturation_speed"))
                return;

            // Remove the existing "imar_tipi" column (if already present as a text column)
            YGADataGridView.Columns.Remove("saturation_speed");
            // Create and add a new ComboBox column for "imar_tipi"
            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "density", // Name must match the original column
                HeaderText = "Density",
                DataSource = new List<string> { "1", "2", "3" },
                DataPropertyName = "density", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };
            var comboBoxColumn2 = new DataGridViewComboBoxColumn
            {
                Name = "saturation_speed", // Name must match the original column
                HeaderText = "Saturation Speed",
                DataSource = new List<string> { "1", "2", "3", "4","5" },
                DataPropertyName = "saturation_speed", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };

            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn);
            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn2);
        }

        // Save button logic
        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("İşlem iptal edildi.");
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

                MessageBox.Show($"Shapefile başarıyla kaydedildi: {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving shapefile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

/*        private void SaveToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                string filePath = defaultFilePath;

                // Prompt the user to select a save location
                using (var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel File (*.xlsx)|*.xlsx",
                    Title = "Save Attributes to Excel",
                    FileName = Path.GetFileName(defaultFilePath) // Use the default name if provided
                })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = saveFileDialog.FileName;
                    }
                    else
                    {
                        // User canceled, return without saving
                        return;
                    }
                }

                // Save the Excel file
                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    workbook.Worksheets.Add(attributesTable, "Attributes");
                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"Attributes saved to {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
*/

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