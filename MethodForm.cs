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
            this.DoubleBuffered = true;
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
           // this.Hide();  // Hide current form
            mod1.ShowDialog();  // Show the new form as a dialog
            this.Show();  // Show current form again after new form is closed
        }


        private void MethodPanel_Paint(object sender, PaintEventArgs e)
        {
            MethodPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
            this.DoubleBuffered = true;
        }
    }
}

