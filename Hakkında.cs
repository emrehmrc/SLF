using System.Windows.Forms;

namespace SLF
{
    public partial class Hakkında : Form
    {

        public HomePageForm gir2;

        public Hakkında()
        {
            InitializeComponent();
            PopulateRichTextBox();
        }


        private void Hakkında_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }

        private void PopulateRichTextBox()
        {
            // Set the initial text
            richTextBox1.Text = "Hakkında (About)\n" +
                               "Yazılım Adı: Jeo-Uzamsal Talep Tahmini (SLF)\n" +
                               "Sürüm: v1.0\n" +
                               "Geliştirici: MRC Türkiye\n\n" +
                               "Proje Yöneticileri: Osmangazi Elektrik Dağıtım A.Ş. (OEDAŞ), GDZ Elektrik Dağıtım A.Ş. (GDZ EDAŞ)\n\n" +
                               "Proje Yürütücüsü (Danışman Firma): MRC Türkiye\n\n" +
                               "Geliştirme Dönemi: Ocak 2023 Ar-Ge Dönemi\n\n" +
                               "Destekleyici Kurum: Enerji Piyasası Düzenleme Kurumu (EPDK)\n\n\n" +
                               "Amaç\n\n" +
                               "Jeo-Uzamsal Talep Tahmini (SLF) Yazılımı, elektrik dağıtım altyapılarının geleceğe yönelik gelişimini coğrafi ve talep odaklı analizlerle öngörebilmek amacıyla geliştirilmiş, akıllı bir karar destek sistemidir. Elektrik tüketicilerinin konumsal dağılımı, geçmiş ve projeksiyon verileri, abone türleri, imar planları, şarj altyapısı ve dağıtık üretim verilerini entegre ederek, dağıtım planlamasında yüksek doğruluklu ve mekansal tahminler sunar.\n\n" +
                               "Temel Özellikler\n\n" +
                               "• Jeo-Uzamsal Yük Tahmini: Hücre düzeyinde (70x90 m) talep modellemesi\n" +
                               "• İmar Analizleri: TAKS/KAKS, bina tipolojisi, arazi sınıflamaları\n" +
                               "• Ekonometrik Modelleme: Nüfus, GRP, meteorolojik veri ve tüketim tahminleri\n" +
                               "• Elektrikli Araç Şarj Altyapısı: Kategorilere göre lokasyon önerileri\n" +
                               "• Dağıtık Üretim Potansiyeli: GES ve diğer kaynaklara uygunluk değerlendirmesi\n" +
                               "• Optimal Trafo Yerleşimi: Yük yoğunluğu ve trafo kapasite eşleşmesi\n" +
                               "• Çoklu Senaryo ve Stokastik Hesaplama: Belirsizlik altında planlama\n" +
                               "• CBS Tabanlı Görselleştirme: Yük yoğunluğu haritaları ve altyapı katmanları\n" +
                               "• Dinamik Raporlama: Grafik, tablo ve rapor formatında çıktılar\n\n" +
                               "Teknik Altyapı\n\n" +
                               "• Veritabanı: PostgreSQL\n" +
                               "• CBS: GIS tabanlı analiz ve görselleştirme\n" +
                               "• Yazılım Dili: Python, R (back-end) ve C# (arayüz)\n" +
                               "• Girdi Kaynakları: İmar, OSOS, SAP, CBS, EA, DEK, Uydu, Overpass.turbo verileri\n\n" +
                               "Proje Ekibi\n\n" +
                               "• Proje Yöneticisi: Burak ÖZSOY (burak.ozsoy@mrc-tr.com)\n" +
                               "• Proje Sorumlusu: Gökhan TOSUN (Gokhan.Tosun@mrc-tr.com)\n" +
                               "• Yazılım Sorumlusu: Emre HANGÜL (emre.hangul@mrc-tr.com)\n" +
                               "• Kalite Sorumlusu: Benhür SATIR (benhur.satir@mrc-tr.com)\n\n" +
                               "Analitik Ekibi:\n" +
                               "Gökhan TOSUN, Burak ÖZSOY, Benhür SATIR, Emre HANGÜL, Arda SARI\n\n" +
                               "Yazılım Ekibi:\n" +
                               "Emre HANGÜL, Arda SARI, Vural BAYRAKLI, Begüm ORHAN, Batuhan YETİŞ\n";
        }
    }
}
