/*using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class DEKCenterPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        public bool OperationCancelled => isOperationCancelled;

        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Fill initial coordinates
            DEKCenterDataGridView.Rows.Add();
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

            // Set ISTASYON_TIPI options for DEK types
            if (DEKCenterDataGridView.Columns["KAYNAK_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "GES (Güneş)", "RES (Rüzgar)", "BES (Biokütle)" }; // Adjust types as needed
            }

            // Populate transformer codes if available
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                                                              .Select(row => row["TRAFO_KODU"].ToString())
                                                              .Distinct()
                                                              .ToList();

                if (DEKCenterDataGridView.Columns["DEK_BAGLANDIGI_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupEventHandlers()
        {
            DEKTamamButton.Click += DEKTamamButton_Click;
            DEKCancelButton.Click += DEKCancelButton_Click;
            this.FormClosing += DEKCenterPopupForm_FormClosing;
        }

        private void DEKTamamButton_Click(object sender, EventArgs e)
        {
            // Validate the input
            foreach (DataGridViewCell cell in DEKCenterDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Add new row to the existing DataTable
            DataRow newRow = dataTable.NewRow();
            newRow["ILCE_ADI"] = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
            newRow["KAYNAK_TIPI"] = DEKCenterDataGridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
            newRow["DEK_X_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
            newRow["DEK_Y_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);
            newRow["DEK_TM_ADI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_TM_ADI"].Value.ToString();
            newRow["DEK_KURULUM_YERI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_KURULUM_YERI"].Value.ToString();
           // newRow["DEK_BAGLANDIGI_TRAFO_KODU"] = DEKCenterDataGridView.Rows[0].Cells["DEK_BAGLANDIGI_TRAFO_KODU"].Value.ToString();

            dataTable.Rows.Add(newRow);

            // Show success message
            MessageBox.Show("DEK merkezi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isOperationCancelled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DEKCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DEKCenterPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOperationCancelled)
            {
                MessageBox.Show("İşlem iptal edildi.");
            }
        }
    }
}
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using static SLF.ModülFormu;

namespace SLF
{
    public partial class DEKCenterPopupForm : Form
    {
        private readonly DataTable dataTable;
        private bool isOperationCancelled = true;
        private NoktaVeri veri; // Store the 'veri' object in the class field

        public bool OperationCancelled => isOperationCancelled;

        // Define a dictionary for cities and their coordinates
        private Dictionary<string, PointLatLng> cityCoordinates = new Dictionary<string, PointLatLng>
    {
        { "İzmir", new PointLatLng(38.4192, 27.1287) },
        { "Eskişehir", new PointLatLng(39.7768, 30.5206) },
        // Add more cities and their coordinates as needed
    };

        // Define districts for İzmir and Eskişehir
        private Dictionary<string, List<string>> cityDistricts = new Dictionary<string, List<string>>
    {
        { "İzmir", new List<string> { "Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", "Çiğli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karaburun", "Karşıyaka", "Kemalpaşa", "Kınık", "Kiraz", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı" } },
        { "Eskişehir", new List<string> { "Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "İnönü", "Mahmudiye", "Mihalgazi", "Mihalıççık", "Odunpazarı", "Sarıcakaya", "Seyitgazi", "Sivrihisar", "Tepebaşı" } }
    };
       
        public DEKCenterPopupForm(DataTable existingDataTable, NoktaVeri veri)
        {
            InitializeComponent();
            dataTable = existingDataTable;
            this.veri = veri; // Store the 'veri' object in the class field

            InitializeDataGridView(veri);
            SetupEventHandlers();
        }

        private void InitializeDataGridView(NoktaVeri veri)
        {
            // Fill initial coordinates from veri object
            DEKCenterDataGridView.Rows.Add();
            DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value = veri.Enlem;
            DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value = veri.Boylam;

            // Set ISTASYON_TIPI options for DEK types
            if (DEKCenterDataGridView.Columns["KAYNAK_TIPI"] is DataGridViewComboBoxColumn typeComboBoxColumn)
            {
                typeComboBoxColumn.DataSource = new List<string> { "GES (Güneş)", "RES (Rüzgar)", "BES (Biokütle)" };
            }

            // Populate transformer codes if available
            if (GirdiModülü.dataTablesByType.TryGetValue("DTR Verileri", out DataTable trafoDataTable))
            {
                List<string> trafoKoduListesi = trafoDataTable.AsEnumerable()
                                                              .Select(row => row["TRAFO_KODU"].ToString())
                                                              .Distinct()
                                                              .ToList();

                if (DEKCenterDataGridView.Columns["DEK_BAGLANDIGI_TRAFO_KODU"] is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    comboBoxColumn.DataSource = trafoKoduListesi;
                }
            }
            else
            {
                MessageBox.Show("DTR Verileri bulunamadı. Lütfen kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Initially populate the city names in the ILCE_ADI ComboBox
            PopulateCityComboBox();
        }

        private void PopulateCityComboBox()
        {
            var comboBoxColumn = DEKCenterDataGridView.Columns["ILCE_ADI"] as DataGridViewComboBoxColumn;

            if (comboBoxColumn != null)
            {
                // Clear the existing items in the ComboBox column
                comboBoxColumn.Items.Clear();

                // Add the districts for each city into the ComboBox column
                foreach (var city in cityCoordinates.Keys)
                {
                    if (cityDistricts.ContainsKey(city))
                    {
                        comboBoxColumn.Items.AddRange(cityDistricts[city].ToArray());
                    }
                }
            }
        }

        private void DEKCenterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Use veri.Enlem and veri.Boylam as the selected coordinates
            double selectedX = veri.Enlem; // Assuming veri.Enlem is the latitude
            double selectedY = veri.Boylam; // Assuming veri.Boylam is the longitude

            // Debugging to check the coordinates
            Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");

            // Call the method to filter districts based on these coordinates
            FilterCountiesBasedOnCoordinates(selectedX, selectedY);
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate the distance between two points on the Earth
            const double R = 6371; // Radius of the earth in km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c; // Distance in km
            return distance;
        }

        private void FilterCountiesBasedOnCoordinates(double selectedX, double selectedY)
        {
            // Get the closest city from the dictionary based on the coordinates
            var closestCity = cityCoordinates
                              .OrderBy(city => GetDistance(city.Value.Lat, city.Value.Lng, selectedX, selectedY))
                              .FirstOrDefault();

            // Debugging to check which city is selected
            Console.WriteLine($"Selected Coordinates: X={selectedX}, Y={selectedY}");
            Console.WriteLine($"Closest City: {closestCity.Key}");

            if (closestCity.Key != null)
            {
                // Access the ComboBox column in the DataGridView
                var comboBoxColumn = DEKCenterDataGridView.Columns["ILCE_ADI"] as DataGridViewComboBoxColumn;

                if (comboBoxColumn != null)
                {
                    // Clear the existing items
                    comboBoxColumn.Items.Clear();

                    // Ensure that the closest city has districts defined in the dictionary
                    if (cityDistricts.ContainsKey(closestCity.Key))
                    {
                        // Add the districts for the closest city to the ComboBox column
                        comboBoxColumn.Items.AddRange(cityDistricts[closestCity.Key].ToArray());
                    }

                    // Debugging to check the districts added
                    Console.WriteLine($"Added Districts: {string.Join(", ", cityDistricts[closestCity.Key])}");
                }

                // Optionally, select the first district in the ComboBox
                var comboBoxCell = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"] as DataGridViewComboBoxCell;
                if (comboBoxCell != null && comboBoxCell.Items.Count > 0)
                {
                    comboBoxCell.Value = comboBoxCell.Items[0];  // Set the first district as the default
                }
            }
        }


        private void SetupEventHandlers()
        {
            DEKTamamButton.Click += DEKTamamButton_Click;
            DEKCancelButton.Click += DEKCancelButton_Click;
            this.FormClosing += DEKCenterPopupForm_FormClosing;
            DEKCenterDataGridView.CellValueChanged += DEKCenterDataGridView_CellValueChanged;
        }

        private void DEKTamamButton_Click(object sender, EventArgs e)
        {
            // Validate the input
            foreach (DataGridViewCell cell in DEKCenterDataGridView.Rows[0].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }
            }

            // Add new row to the existing DataTable
            DataRow newRow = dataTable.NewRow();
            newRow["ILCE_ADI"] = DEKCenterDataGridView.Rows[0].Cells["ILCE_ADI"].Value.ToString();
            newRow["KAYNAK_TIPI"] = DEKCenterDataGridView.Rows[0].Cells["KAYNAK_TIPI"].Value.ToString();
            newRow["DEK_KURULU_GUCU"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_KURULU_GUCU"].Value);
            newRow["DEK_X_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_X_KOORDINAT"].Value);
            newRow["DEK_Y_KOORDINAT"] = Convert.ToDouble(DEKCenterDataGridView.Rows[0].Cells["DEK_Y_KOORDINAT"].Value);
            newRow["DEK_TM_ADI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_TM_ADI"].Value.ToString();
            newRow["DEK_KURULUM_YERI"] = DEKCenterDataGridView.Rows[0].Cells["DEK_KURULUM_YERI"].Value.ToString();
            //newRow["DEK_BAGLANDIGI_TRAFO_KODU"] = DEKCenterDataGridView.Rows[0].Cells["DEK_BAGLANDIGI_TRAFO_KODU"].Value.ToString();

            dataTable.Rows.Add(newRow);

            // Show success message
            MessageBox.Show("DEK merkezi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isOperationCancelled = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DEKCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DEKCenterPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOperationCancelled)
            {
                MessageBox.Show("İşlem iptal edildi.");
            }
        }
    }

}

