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
    public partial class ProjectFolderPicker : Form
    {
        private ListView projectListView;
        private Button selectButton;
        private Button cancelButton;
        private string baseDirectory;

        // Seçilen proje klasörünün tam yolu
        public string SelectedPath { get; private set; }

        // Seçilen proje adı
        public string SelectedProjectName { get; private set; }

        /// <summary>
        /// ProjectFolderPicker sınıfının yapıcı metodu
        /// </summary>
        /// <param name="baseDir">Projelerin bulunduğu temel dizin</param>
        public ProjectFolderPicker(string baseDir)
        {
            baseDirectory = baseDir;
            InitializeComponents();
            LoadProjectFolders();
        }
        /// <summary>
        /// Gelişmiş proje seçim formunu gösterir - Hem mevcut projeler arasından seçim hem de yeni proje oluşturma
        /// </summary>
        /// <param name="baseDir">Projelerin bulunduğu temel dizin</param>
        /// <param name="allowCreate">Yeni proje oluşturmaya izin verilsin mi</param>
        /// <returns>Seçilen veya oluşturulan proje adı, iptal edilirse null</returns>
        public static string ShowProjectSelectionDialog(string baseDir, bool allowCreate = true)
        {
            using (var projectPicker = new ProjectFolderPicker(baseDir))
            {
                projectPicker.EnableCreateProject = allowCreate;

                if (projectPicker.ShowDialog() == DialogResult.OK)
                {
                    return projectPicker.SelectedProjectName;
                }

                return null;
            }
        }
        // Yeni alanlar ekleyin
        private RadioButton selectExistingRadio;
        private RadioButton createNewRadio;
        private TextBox newProjectTextBox;
        private Panel createProjectPanel;
        private bool _enableCreateProject = false;

        /// <summary>
        /// Yeni proje oluşturmaya izin verilsin mi
        /// </summary>
        public bool EnableCreateProject
        {
            get { return _enableCreateProject; }
            set
            {
                _enableCreateProject = value;
                if (createProjectPanel != null)
                    createProjectPanel.Visible = value;
            }
        }

        // Mevcut InitializeComponents metodunu güncelleyin


        /// <summary>
        /// Form bileşenlerini oluşturur ve ayarlar
        /// </summary>
        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = "Proje Seçimi";
            this.Width = 500;
            this.Height = 480; // Formu biraz daha yüksek yapalım

            // Ana panel oluştur
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Başlık label'ı
            Label titleLabel = new Label
            {
                Text = "Proje Seçimi",
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mevcut projeleri seç radio butonu
            selectExistingRadio = new RadioButton
            {
                Text = "Mevcut bir projeyi seç",
                Checked = true,
                Dock = DockStyle.Top,
                Height = 30
            };

            // ListView oluştur
            projectListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                GridLines = true
            };

            // Sütunlar ekle
            projectListView.Columns.Add("Proje Adı", 150);
            projectListView.Columns.Add("Oluşturulma Tarihi", 150);
            projectListView.Columns.Add("Son Değişiklik", 150);

            // Yeni proje oluştur paneli
            createProjectPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120, // Yüksekliği artır
                Visible = true // Varsayılan olarak görünür yap
            };

            // Yeni proje oluştur radio butonu
            createNewRadio = new RadioButton
            {
                Text = "Yeni bir proje oluştur",
                Checked = false,
                Dock = DockStyle.Top,
                Height = 30
            };

            // Yeni proje adı için açıklama label'ı
            Label newProjectLabel = new Label
            {
                Text = "Proje Adı:",
                Dock = DockStyle.Top,
                Height = 20,
                Margin = new Padding(20, 5, 20, 0)
            };

            // Yeni proje adı için TextBox
            newProjectTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                Enabled = false,
                Margin = new Padding(20, 0, 20, 10),
                Text = "" // Boş olarak başlat
            };

            // Buton paneli
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            // Seç butonu
            selectButton = new Button
            {
                Text = "Seç/Oluştur",
                DialogResult = DialogResult.OK,
                Enabled = false,
                Width = 100,
                Height = 30,
                Location = new Point(this.Width - 230, 10)
            };

            // İptal butonu
            cancelButton = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Width = 100,
                Height = 30,
                Location = new Point(this.Width - 120, 10)
            };

            // Olaylar

            // ListView'a öğe seçildiğinde Seç butonunu etkinleştir
            projectListView.SelectedIndexChanged += (s, e) => {
                if (selectExistingRadio.Checked)
                {
                    bool validSelection = projectListView.SelectedItems.Count > 0 &&
                                         projectListView.SelectedItems[0].Tag != null &&
                                         projectListView.SelectedItems[0].Tag.ToString() != "empty";
                    selectButton.Enabled = validSelection;
                }
            };

            // DoubleClick ile seçim
            projectListView.DoubleClick += (s, e) => {
                if (projectListView.SelectedItems.Count > 0 &&
                    projectListView.SelectedItems[0].Tag != null &&
                    projectListView.SelectedItems[0].Tag.ToString() != "empty")
                {
                    selectExistingRadio.Checked = true;
                    this.SelectedPath = projectListView.SelectedItems[0].Tag.ToString();
                    this.SelectedProjectName = projectListView.SelectedItems[0].Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            // Radio buton değişim olayları
            selectExistingRadio.CheckedChanged += (s, e) => {
                if (selectExistingRadio.Checked)
                {
                    projectListView.Enabled = true;
                    newProjectTextBox.Enabled = false;

                    bool validSelection = projectListView.SelectedItems.Count > 0 &&
                                         projectListView.SelectedItems[0].Tag != null &&
                                         projectListView.SelectedItems[0].Tag.ToString() != "empty";
                    selectButton.Enabled = validSelection;

                    selectButton.Text = "Seç";
                }
            };

            createNewRadio.CheckedChanged += (s, e) => {
                if (createNewRadio.Checked)
                {
                    projectListView.Enabled = false;
                    newProjectTextBox.Enabled = true;
                    newProjectTextBox.Focus(); // TextBox'a odaklan
                    selectButton.Enabled = !string.IsNullOrWhiteSpace(newProjectTextBox.Text);
                    selectButton.Text = "Oluştur";
                }
            };
            newProjectTextBox.TextChanged += (s, e) => {
                if (createNewRadio.Checked)
                    selectButton.Enabled = !string.IsNullOrWhiteSpace(newProjectTextBox.Text);
            };

            // Seç butonu ile işlem
            selectButton.Click += (s, e) => {
                if (selectExistingRadio.Checked)
                {
                    // Mevcut projeyi seçme kodu
                    if (projectListView.SelectedItems.Count > 0 &&
                        projectListView.SelectedItems[0].Tag != null &&
                        projectListView.SelectedItems[0].Tag.ToString() != "empty")
                    {
                        this.SelectedPath = projectListView.SelectedItems[0].Tag.ToString();
                        this.SelectedProjectName = projectListView.SelectedItems[0].Text;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Lütfen bir proje seçin.",
                            "Seçim Yapılmadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else // Yeni proje oluştur
                {
                    string projectName = newProjectTextBox.Text.Trim();

                    Console.WriteLine($"Proje adı: '{projectName}'"); // Debug için

                    if (string.IsNullOrWhiteSpace(projectName))
                    {
                        MessageBox.Show("Lütfen geçerli bir proje adı girin.",
                            "Geçersiz İsim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Proje adında geçersiz karakter kontrolü
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        if (projectName.Contains(c))
                        {
                            MessageBox.Show($"Proje adı aşağıdaki karakterleri içeremez:\n{new string(Path.GetInvalidFileNameChars())}",
                                "Geçersiz Karakter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Klasör adı oluştur
                    string projectFolderName = $"proje_{projectName}";
                    string projectPath = Path.Combine(baseDirectory, projectFolderName);

                    // Eğer bu isimde bir proje zaten varsa
                    if (Directory.Exists(projectPath))
                    {
                        MessageBox.Show($"'{projectName}' adında bir proje zaten var. Lütfen farklı bir isim seçin.",
                            "Proje Zaten Var", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    this.SelectedProjectName = projectName;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };

            // Kontrolleri panellere ekle
            createProjectPanel.Controls.Add(newProjectTextBox);
            createProjectPanel.Controls.Add(newProjectLabel);
            createProjectPanel.Controls.Add(createNewRadio);

            buttonPanel.Controls.Add(selectButton);
            buttonPanel.Controls.Add(cancelButton);

            // Ana panele kontrolleri ekle
            mainPanel.Controls.Add(projectListView);
            mainPanel.Controls.Add(selectExistingRadio);
            mainPanel.Controls.Add(titleLabel);

            // Formu kontrollere ekle
            this.Controls.Add(createProjectPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(mainPanel);

            // Enter tuşuna basıldığında OK butonunu tetikle
            this.AcceptButton = selectButton;
            this.CancelButton = cancelButton;
        }

        /// <summary>
        /// Proje klasörlerini listeler
        /// </summary>
        private void LoadProjectFolders()
        {
            try
            {
                // Proje klasörlerini bul
                if (Directory.Exists(baseDirectory))
                {
                    string[] projectFolders = Directory.GetDirectories(baseDirectory, "proje_*");

                    if (projectFolders.Length == 0)
                    {
                        // Hiç proje yoksa, bir bilgi mesajı göster ve yeni proje oluşturma arayüzünü aktif et
                        var emptyItem = new ListViewItem("Henüz proje yok.");
                        emptyItem.SubItems.Add("-");
                        emptyItem.SubItems.Add("-");
                        emptyItem.ForeColor = Color.Gray;
                        emptyItem.Tag = "empty"; // Boş olduğunu belirtmek için tag ekle
                        projectListView.Items.Add(emptyItem);

                        // Yeni proje oluşturma arayüzünü aktif et
                        if (_enableCreateProject && createNewRadio != null)
                        {
                            createNewRadio.Checked = true;
                            // Proje listesinin tamamını devre dışı bırak
                            projectListView.Enabled = false;

                            // Bir bilgi mesajı ekle
                            Label noProjectLabel = new Label
                            {
                                Text = "Henüz proje bulunmuyor. Aşağıdan yeni bir proje oluşturabilirsiniz.",
                                AutoSize = true,
                                ForeColor = Color.DarkBlue,
                                Dock = DockStyle.Top,
                                Padding = new Padding(5)
                            };

                            // Label'ı forma ekle (varsa mevcut label'ı kaldır)
                            var existingLabel = this.Controls.Find("noProjectLabel", true).FirstOrDefault();
                            if (existingLabel != null)
                                this.Controls.Remove(existingLabel);

                            noProjectLabel.Name = "noProjectLabel";
                            this.Controls.Add(noProjectLabel);
                            noProjectLabel.BringToFront();
                        }

                        selectButton.Enabled = false;
                        return;
                    }

                    foreach (string folder in projectFolders)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(folder);
                        string projectName = dirInfo.Name.Substring(6); // "proje_" çıkar

                        // Proje state dosyasını kontrol et
                        string statePath = Path.Combine(folder, "project_state.json");
                        string lastSaved = "-";

                        if (File.Exists(statePath))
                        {
                            try
                            {
                                string json = File.ReadAllText(statePath);
                                var projectState = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                                if (projectState.TryGetValue("LastSaved", out object lastSavedObj))
                                {
                                    lastSaved = lastSavedObj.ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Proje durumu okunurken hata: {ex.Message}");
                            }
                        }

                        // ListView'a ekle
                        var item = new ListViewItem(projectName);
                        item.SubItems.Add(dirInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        item.SubItems.Add(lastSaved);
                        item.Tag = folder; // tam yolu sakla

                        projectListView.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show($"Proje dizini bulunamadı: {baseDirectory}",
                        "Dizin Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Proje klasörleri yüklenirken hata: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Yeni proje oluşturma formunu gösterir
        /// </summary>
        public static string ShowNewProjectDialog(string baseDir)
        {
            using (var inputDialog = new InputDialog("Yeni Proje", "Lütfen projenin adını girin:"))
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string projectName = inputDialog.InputText.Trim();

                    if (string.IsNullOrWhiteSpace(projectName))
                    {
                        MessageBox.Show("Geçerli bir proje adı girmelisiniz.",
                            "Geçersiz İsim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }

                    // Proje adında geçersiz karakter kontrolü
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        if (projectName.Contains(c))
                        {
                            MessageBox.Show($"Proje adı aşağıdaki karakterleri içeremez:\n{new string(Path.GetInvalidFileNameChars())}",
                                "Geçersiz Karakter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return null;
                        }
                    }

                    // Klasör adı oluştur
                    string projectFolderName = $"proje_{projectName}";
                    string projectPath = Path.Combine(baseDir, projectFolderName);

                    // Eğer bu isimde bir proje zaten varsa
                    if (Directory.Exists(projectPath))
                    {
                        MessageBox.Show($"'{projectName}' adında bir proje zaten var. Lütfen farklı bir isim seçin.",
                            "Proje Zaten Var", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }

                    return projectName;
                }
            }

            return null;
        }
    }
   

    /// <summary>
    /// Basit bir metin giriş dialog'u
    /// </summary>
    public class InputDialog : Form
    {
        private TextBox textBox;
        private Button buttonOK;
        private Button buttonCancel;
        private Label label;

        public string InputText => textBox.Text;

        public InputDialog(string title, string promptText)
        {
            this.Text = title;

            label = new Label
            {
                Text = promptText,
                AutoSize = true,
                Location = new Point(12, 9)
            };

            textBox = new TextBox
            {
                Location = new Point(12, 32),
                Size = new Size(260, 23)
            };

            buttonOK = new Button
            {
                Text = "Tamam",
                DialogResult = DialogResult.OK,
                Location = new Point(116, 70),
                Width = 75
            };

            buttonCancel = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Location = new Point(197, 70),
                Width = 75
            };


            this.Controls.Add(label);
            this.Controls.Add(textBox);
            this.Controls.Add(buttonOK);
            this.Controls.Add(buttonCancel);

            this.AcceptButton = buttonOK;
            this.CancelButton = buttonCancel;
            this.ClientSize = new Size(284, 107);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}
