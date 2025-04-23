using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace SLF
{
    public partial class HomePageForm : Form
    {
        public ModülFormu mod1;
        public Hakkında mod2;

        public string json_file;
        public dynamic config;
        public string userRootPath;
        public string config_path;

        public HomePageForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Set the Enter key to trigger the StartButton click event
            this.AcceptButton = StartButton;

            // Ensure StartButton has focus when the form is shown
            this.Shown += HomePageForm_Shown;

            userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            config_path = Path.Combine(userRootPath,
                "MRC\\MRC - 1.1.3_T&SI\\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\\il_ilce_kırılımları\\İzmir\\Program Dosyaları\\config.json").Replace("/", "\\");

            // read the json file and create the "config" variable.
            json_file = File.ReadAllText(config_path);
            
            config = JsonConvert.DeserializeObject(json_file);

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

        private void buton_yardım_Click(object sender, EventArgs e)
        {
            Yardım yardım = new Yardım();
            yardım.Show();
        }

        private void buton_hakkında_Click(object sender, EventArgs e)
        {
            Hakkında hakkında = new Hakkında();
            hakkında.Show();
        }
    }

}