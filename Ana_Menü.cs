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
    public partial class HomePageForm : Form
    {
        public ModülFormu mod1;
        public Hakkında mod2;

        public HomePageForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Set the Enter key to trigger the StartButton click event
            this.AcceptButton = StartButton;

            // Ensure StartButton has focus when the form is shown
            this.Shown += HomePageForm_Shown;
        }

        private void HomePageForm_Shown(object sender, EventArgs e)
        {
            StartButton.Focus();
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            MethodForm methodForm = new MethodForm(this);
            methodForm.ShowDialog();
            this.Show();
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            Yardım yardım_formu = new Yardım();
            yardım_formu.Show();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            mod2 = new Hakkında();
            mod2.Tag = this;
            mod2.Show();
            this.Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Only trigger the StartButton's click event if HomePageForm is the active form
            if (keyData == Keys.Enter && this == Form.ActiveForm)
            {
                // Trigger StartButton's Click event
                StartButton.PerformClick();
                return true; // Mark the key as handled
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

}