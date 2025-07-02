using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SLF
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Inside your Program.cs -> Main()
            CultureInfo culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var logPath = Path.Combine(Application.StartupPath, "log.txt");
            var logWriter = new StreamWriter(logPath, append: true);
            logWriter.AutoFlush = true;

            Console.SetOut(logWriter);      // Console.WriteLine -> log.txt
            Console.SetError(logWriter);    // Hatalar da log.txt'ye gider

            Console.WriteLine("Uygulama başlatıldı: " + DateTime.Now);
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomePageForm());
            //Application.Run(new ModülFormu());                  

            Console.WriteLine("Uygulama kapatılıyor: " + DateTime.Now);
            logWriter.Close(); // Uygulama kapanırken dosyayı kapat


        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        // Non-UI Thread exceptions
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                HandleException(ex);
            }
            else
            {
                MessageBox.Show("Bilinmeyen Hata Gerçekleşti.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Your generic exception handler
        private static void HandleException(Exception ex)
        {
            // Example: show a message box and continue running
            MessageBox.Show($"Hata:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Optionally: log the exception to a file, telemetry, etc.
        }
    }
}
