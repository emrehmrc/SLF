using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLF.Services
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    // Özel Türkçe diyalog sınıfı
    public class TurkceDiyalog : Form
    {
        private Label messageLabel;
        private Button evetButton;
        private Button hayirButton;
        private Button iptalButton;
        private DialogResult sonuc = DialogResult.None;

        public TurkceDiyalog(string mesaj, string baslik)
        {
            // Form ayarları
            this.Text = baslik;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 400;
            this.Height = 180;
            this.ShowInTaskbar = false;

            // Mesaj etiketi
            messageLabel = new Label();
            messageLabel.Text = mesaj;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(350, 0);
            messageLabel.Location = new Point(20, 20);
            messageLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(messageLabel);

            // Butonlar
            evetButton = new Button();
            evetButton.Text = "Evet";
            evetButton.DialogResult = DialogResult.Yes;
            evetButton.Location = new Point(75, 100);
            evetButton.Width = 80;
            evetButton.Click += (sender, e) => { sonuc = DialogResult.Yes; this.Close(); };
            this.Controls.Add(evetButton);

            hayirButton = new Button();
            hayirButton.Text = "Hayır";
            hayirButton.DialogResult = DialogResult.No;
            hayirButton.Location = new Point(165, 100);
            hayirButton.Width = 80;
            hayirButton.Click += (sender, e) => { sonuc = DialogResult.No; this.Close(); };
            this.Controls.Add(hayirButton);

            iptalButton = new Button();
            iptalButton.Text = "İptal";
            iptalButton.DialogResult = DialogResult.Cancel;
            iptalButton.Location = new Point(255, 100);
            iptalButton.Width = 80;
            iptalButton.Click += (sender, e) => { sonuc = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(iptalButton);

            // Enter ve Escape tuşları için
            this.AcceptButton = evetButton;
            this.CancelButton = iptalButton;
        }

        // MessageBox.Show benzeri statik metod
        public static DialogResult Goster(string mesaj, string baslik)
        {
            using (TurkceDiyalog form = new TurkceDiyalog(mesaj, baslik))
            {
                form.ShowDialog();
                return form.sonuc;
            }
        }
    }
}
