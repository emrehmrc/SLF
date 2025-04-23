using System.Windows.Forms;

namespace SLF
{
    public partial class BekleForm : Form
    {
        public BekleForm()
        {
            InitializeComponent();

            // Set form properties to make it non-movable and centered
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

    }
}