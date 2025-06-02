using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLF
{
    public partial class Tablo_olustur : Form
    {
        public Tablo_olustur()
        {
            InitializeComponent();
         
        }



        // BUTON 1 - Yeni script için hazır
        private void dtrVeriTabloOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Yeni script için işlevler buraya eklenecek

                // Örnek yapı:
                // 1. Seçilen dosyaları kontrol et
                // 2. Script yolunu belirle
                // 3. Argümanları hazırla
                // 4. Python scriptini çalıştır

                MessageBox.Show("Bu buton yeni script için yapılandırılacak!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BUTON 2 - Yeni script için hazır
        private void aboneVeriOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Yeni script için işlevler buraya eklenecek

                // Örnek yapı:
                // 1. Seçilen dosyaları kontrol et
                // 2. Script yolunu belirle
                // 3. Argümanları hazırla
                // 4. Python scriptini çalıştır

                MessageBox.Show("Bu buton yeni script için yapılandırılacak!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}