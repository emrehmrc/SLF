using System;
using System.Windows.Forms;
using SLF.Services; // DatabaseManager için namespace

namespace SLF
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Şifre alanını maskele
            textBoxPassword.PasswordChar = '*';
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string server = textBoxServer.Text.Trim();
            string port = textBoxPort.Text.Trim();
            string database = textBoxDatabase.Text.Trim();
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            // Gerekli alanların doldurulup doldurulmadığını kontrol et
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) ||
                string.IsNullOrEmpty(database) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connString = $"Server={server};Port={port};Database={database};Username={username};Password={password}";

            try
            {
                // DatabaseManager Singleton nesnesini başlat
                DatabaseManager.GetInstance(connString).GetConnection();

                MessageBox.Show("Bağlantı başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Giriş başarılı, formu kapat ve ana uygulamayı devam ettir
                this.DialogResult = DialogResult.OK; // LoginForm'dan başarıyla çıkmak için
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    }

