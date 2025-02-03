/*using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel; // Add this for Excel functionality

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

            // Create and add a new ComboBox column for "density"
            var comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = "density", // Name must match the original column
                HeaderText = "Density",
                DataSource = new List<string> { "1", "2", "3" },
                DataPropertyName = "density", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };

            // Create and add a new ComboBox column for "saturation_speed"
            var comboBoxColumn2 = new DataGridViewComboBoxColumn
            {
                Name = "saturation_speed", // Name must match the original column
                HeaderText = "Saturation Speed",
                DataSource = new List<string> { "1", "2", "3", "4", "5" },
                DataPropertyName = "saturation_speed", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };

            // Add the combobox columns to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn);
            YGADataGridView.Columns.Add(comboBoxColumn2);
        }

        // Save button logic
        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
            // Prompt the user to choose between saving as Shapefile or Excel
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Shapefile (*.shp)|*.shp|Excel File (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save YGA Parameters";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveDialog.FileName;
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".shp")
                    {
                        SaveAsShapefile(dataTable, filePath);
                        MessageBox.Show("YGA Parameters saved as Shapefile successfully!");
                    }
                    else if (extension == ".xlsx")
                    {
                        SaveToExcel(dataTable, filePath);
                        MessageBox.Show("YGA Parameters saved as Excel successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Unsupported file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        // Save DataTable to Excel
        private void SaveToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(dataTable, "YGA Parameters");
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Save DataTable as Shapefile
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

                MessageBox.Show($"Shapefile saved successfully: {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Cancel button logic
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
*/


using ClosedXML.Excel;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                DataSource = new List<string> { "1", "2", "3", "4", "5" },
                DataPropertyName = "saturation_speed", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };

            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn);
            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn2);
        }
        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
            // Show a SaveFileDialog to let the user choose a base filename
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                // Configure the dialog
                saveDialog.Filter = "All Files (*.*)|*.*"; // Allow any filename
                saveDialog.Title = "Save YGA Parameters (SHP and Excel)";
                //saveDialog.FileName = "YGA_Parameters"; // Default base name (no extension)

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Extract the directory and base filename (without extension)
                    string directory = Path.GetDirectoryName(saveDialog.FileName);
                    string baseName = Path.GetFileNameWithoutExtension(saveDialog.FileName);

                    // Generate paths for both SHP and Excel files
                    string shpPath = Path.Combine(directory, $"{baseName}.shp");
                    string xlsxPath = Path.Combine(directory, $"{baseName}.xlsx");

                    // Save both files
                    SaveAsShapefile(dataTable, shpPath);
                    SaveToExcel(dataTable, xlsxPath);

                    // Show success message with paths
                    MessageBox.Show(
                        $"Files saved successfully:\n\nShapefile: {shpPath}\nExcel: {xlsxPath}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Close the form
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
        // Save button logic
        /*        private void YGATableSaveButton_Click(object sender, EventArgs e)
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
        */
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

        private void SaveToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(dataTable, "YGA Parameters");
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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