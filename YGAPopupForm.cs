/*using MapWinGIS;
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
}*/



using ClosedXML.Excel;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            // Remove the existing "saturation_speed" column (if already present as a text column)
            YGADataGridView.Columns.Remove("saturation_speed");

            if (!YGADataGridView.Columns.Contains("start_year"))
                return;

            // Remove the existing "saturation_speed" column (if already present as a text column)
            YGADataGridView.Columns.Remove("start_year");


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
            var comboBoxColumn3 = new DataGridViewComboBoxColumn
            {
                Name = "start_year", // Name must match the original column
                HeaderText = "Başlangıç Yılı",
                DataSource = new List<string> { "2024", "2025", "2026", "2027", "2028", "2029", "2030", "2031", "2032", "2033", "2034", "2035" },
                DataPropertyName = "start_year", // Map to the DataTable column
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                AutoComplete = true
            };
            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn);
            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn2);
            // Add the combobox column to the DataGridView
            YGADataGridView.Columns.Add(comboBoxColumn3);
        }
        private void YGATableSaveButton_Click(object sender, EventArgs e)
        {
            // Show a SaveFileDialog to let the user choose a base filename
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "All Files (*.*)|*.*";
                saveDialog.Title = "Save YGA Parameters (SHP and Excel)";
                // saveDialog.FileName = "YGA_Parameters"; // Default base name

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string directory = Path.GetDirectoryName(saveDialog.FileName);
                    string baseName = Path.GetFileNameWithoutExtension(saveDialog.FileName);

                    string shpPath = Path.Combine(directory, $"{baseName}.shp");
                    string xlsxPath = Path.Combine(directory, $"{baseName}.xlsx");

                    // Save new files
                    SaveAsShapefile(dataTable, shpPath);
                    SaveToExcel(dataTable, xlsxPath);

                    // Append data to the fixed Excel file
                    //AppendToFixedExcelFile(dataTable);

                    // Show success message
                    MessageBox.Show(
                        $"New files saved:\n\nShapefile: {shpPath}\nExcel: {xlsxPath}\n\n" +
                        $"Data also appended to fixed Excel file.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
        private readonly Dictionary<string, string> _columnMappings = new Dictionary<string, string>
{
    { "density", "density" },               // Maps DataTable's "density" to Excel's "Density"
    { "saturation_speed", "saturation_speed" },
    //{ "Coordinates", "Geometry" },
    // Add other columns as needed
        { "Mesken", "Mesken" },
            { "Sanayi", "Sanayi" },
                { "Ticarethane", "Ticarethane" },
                                { "start_year", "start_year" },
};
        private void AppendToFixedExcelFile(DataTable dataTable)
        {
            string fixedExcelPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda_slf\v2\kullanici_girdisi2.xlsx";

            try
            {
                using (var workbook = File.Exists(fixedExcelPath)
                        ? new XLWorkbook(fixedExcelPath)
                        : new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Count > 0
                        ? workbook.Worksheet(1)
                        : workbook.Worksheets.Add("User Inputs");

                    // Ensure headers exist for mapped columns
                    int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                    bool isNewFile = lastRow == 0;

                    // Add headers if the file is new or missing mapped columns
                    if (isNewFile)
                    {
                        int col = 1;
                        foreach (var mapping in _columnMappings.Values)
                        {
                            worksheet.Cell(1, col).Value = mapping;
                            col++;
                        }
                        lastRow = 1; // Start data from row 2
                    }

                    // Verify that all mapped columns exist in the Excel file
                    var headerCells = worksheet.Row(1).CellsUsed();
                    var excelHeaders = headerCells.Select(c => c.Value.ToString()).ToList();

                    // Create a dictionary to map Excel column names to their indices
                    var excelColumnIndices = new Dictionary<string, int>();
                    foreach (var header in excelHeaders)
                    {
                        excelColumnIndices[header] = worksheet.Row(1).CellsUsed()
                            .First(c => c.Value.ToString() == header).Address.ColumnNumber;
                    }

                    // Append data rows
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        lastRow++;
                        foreach (var mapping in _columnMappings)
                        {
                            string dataTableColumn = mapping.Key;
                            string excelColumn = mapping.Value;

                            // Skip if the DataTable column doesn't exist
                            if (!dataTable.Columns.Contains(dataTableColumn))
                            {
                                MessageBox.Show($"DataTable column '{dataTableColumn}' not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            // Skip if the Excel column doesn't exist
                            if (!excelColumnIndices.ContainsKey(excelColumn))
                            {
                                MessageBox.Show($"Excel column '{excelColumn}' not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            // Get the Excel column index
                            int excelColIndex = excelColumnIndices[excelColumn];

                            // Update the cell
                            worksheet.Cell(lastRow, excelColIndex).Value = dataRow[dataTableColumn].ToString();
                        }
                    }

                    // Save changes
                    workbook.SaveAs(fixedExcelPath);
                }

                MessageBox.Show($"Data appended to fixed Excel file:\n{fixedExcelPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating fixed Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /*        private void AppendToFixedExcelFile(DataTable dataTable)
                {
                    string fixedExcelPath = @"C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda_slf\v2\kullanici_girdisi2.xlsx";

                    try
                    {
                        using (var workbook = File.Exists(fixedExcelPath)
                                ? new XLWorkbook(fixedExcelPath)
                                : new XLWorkbook())
                        {
                            // Get the worksheet or create it if it doesn't exist
                            var worksheet = workbook.Worksheets.Count > 0
                                ? workbook.Worksheet(1)
                                : workbook.Worksheets.Add("User Inputs");

                            // Find the last used row (skip header if it exists)
                            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                            bool hasHeaders = lastRow > 0;

                            // Add headers if the file is new or empty
                            if (lastRow == 0)
                            {
                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    worksheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
                                }
                                lastRow = 1; // Start appending data from row 2
                            }

                            // Append data rows
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                lastRow++;
                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    worksheet.Cell(lastRow, i + 1).Value = dataRow[i].ToString();
                                }
                            }

                            // Save changes
                            workbook.SaveAs(fixedExcelPath);
                        }

                        MessageBox.Show($"Data appended to fixed Excel file:\n{fixedExcelPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating fixed Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }*/
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