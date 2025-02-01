using System;
using System.Drawing;
using System.Windows.Forms;

namespace SLF
{
    public partial class MethodForm : Form
    {
        public ModülFormu mod1;
        public string selectedMethod { get; private set; }
        private HomePageForm homePageForm; // Reference to HomePageForm

        public MethodForm(HomePageForm homePageForm)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.homePageForm = homePageForm; // Store the reference

            // Set the Enter key to trigger the ForwardButton click event
            this.AcceptButton = ForwardButton;

            // Ensure ForwardButton has focus when the form is shown
            this.Shown += MethodForm_Shown;

        }

        private void MethodForm_Shown(object sender, EventArgs e)
        {
            ForwardButton.Focus();
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
                MessageBox.Show("İlerlemek için bir metot seçiniz");
            }
        }

        private void OpenModülFormuBasedOnSelection(string method)
        {
            mod1 = new ModülFormu(method);  // Pass selectedMethod to ModülFormu

            // Hide both forms
            this.Hide();  // Hide MethodForm
            homePageForm.Hide();  // Hide HomePageForm

            mod1.ShowDialog();  // Show the new form as a dialog

            // Optionally, you can show both forms again if needed

            homePageForm.Show(); // Show HomePageForm again if it needs to be visible
            this.Show();  // Show MethodForm again after ModülFormu is closed
        }

        private void MethodPanel_Paint(object sender, PaintEventArgs e)
        {
            MethodPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
            this.DoubleBuffered = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Only trigger the ForwardButton's click event if MethodForm is the active form
            if (keyData == Keys.Enter && this == Form.ActiveForm)
            {
                // Trigger ForwardButton's Click event
                ForwardButton.PerformClick();
                return true; // Mark the key as handled
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}