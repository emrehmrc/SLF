using Avalonia.Controls;
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
    public partial class MethodForm : Form
    {
        public ModülFormu mod1;
        public string selectedMethod { get; private set; }

        public MethodForm()
        {
            InitializeComponent();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (MethodComboBox.SelectedItem != null)
            {
                selectedMethod = MethodComboBox.SelectedItem.ToString();
                OpenModülFormuBasedOnSelection(selectedMethod);
            }
            else
            {
                MessageBox.Show("Please select an option from the ComboBox.");
            }
        }

        private void OpenModülFormuBasedOnSelection(string method)
        {
            mod1 = new ModülFormu(method);  // Pass selectedMethod to ModülFormu
            this.Hide();  // Hide current form
            mod1.ShowDialog();  // Show the new form as a dialog
            this.Show();  // Show current form again after new form is closed
        }


        private void MethodPanel_Paint(object sender, PaintEventArgs e)
        {
            MethodPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
        }
    }
}



/*namespace SLF
{
    public partial class MethodForm : Form
    {
        public ModülFormu mod1;
        //public string selectedItem;
        public string selectedMethod { get; private set; }


        public MethodForm()
        {
            InitializeComponent();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            // Check if the ComboBox has a selected item
            if (MethodComboBox.SelectedItem != null)
            {
                selectedMethod = MethodComboBox.SelectedItem.ToString();

                // Handle different actions based on the selected ComboBox item
                switch (selectedMethod)
                {
                    case "SLF":
                        // Open the ModülFormu with no specific tab selected (open whole tabs)
                        mod1 = new ModülFormu();
                        this.Hide();  // Hide current form
                        mod1.ShowDialog();  // Show the new form as a dialog
                        this.Show();  // Show current form again after new form is closed
                        break;

                    case "ELF":
                        // Open the ModülFormu and select a specific tab
                        mod1 = new ModülFormu("tab_girdi");
                        this.Hide();
                        mod1.ShowDialog();
                        this.Show();
                        break;

                    default:
                        // Handle unexpected selections or show an error message
                        MessageBox.Show("Please select a valid option from the ComboBox.");
                        break;
                }
            }
            else
            {
                // If no item is selected, show a message
                MessageBox.Show("Please select an option from the ComboBox.");
            }
        }

        private void MethodPanel_Paint(object sender, PaintEventArgs e)
        {
            MethodPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
        }
    }
}*/

/*
Modül_Tabları.SelectTab(tab_girdi);
veri_listesi_seçimi.Text = "Ekonometrik Yük Tahmini Verileri";
veri_listesi_seçimi.Enabled = false;
*/


/*namespace SLF
{
    public partial class OptionForm : Form
    {
        public ModülFormu mod1;
        public OptionForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Initialize the class-level mod1 variable
            mod1 = new ModülFormu();

            // Hide the current form (GirişFormu)
            this.Hide();

            // Show the new form
            mod1.ShowDialog();

            // Once the new form is closed, show the current form (GirişFormu) again
            this.Show();
        }
    }
}*/
