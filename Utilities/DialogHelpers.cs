using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SLF.Utilities
{
    public static class DialogHelpers
    {
        /// <summary>
        /// Shows an OpenFileDialog with retries and safe defaults to avoid sporadic blank/white dialog issues.
        /// Returns the selected file path or null if cancelled or failed.
        /// </summary>
        public static string ShowOpenFileDialogSafe(string title, string filter = "All files (*.*)|*.*", string initialDirectory = null)
        {
            // Try a few times in case shell/Win32 initialization transiently fails
            const int maxAttempts = 3;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using (var dlg = new OpenFileDialog())
                    {
                        dlg.Title = string.IsNullOrWhiteSpace(title) ? "Open" : title;
                        dlg.Filter = string.IsNullOrWhiteSpace(filter) ? "All files (*.*)|*.*" : filter;
                        dlg.RestoreDirectory = true;
                        dlg.CheckFileExists = true;
                        dlg.CheckPathExists = true;
                        dlg.AutoUpgradeEnabled = false; // use legacy style on some OS to avoid rendering issues

                        string lastDirectory = SLF.Properties.Settings.Default.LastFileDialogDirectory;
                        string directoryToOpen = Directory.Exists(lastDirectory)
                            ? lastDirectory
                            : initialDirectory;

                        if (!string.IsNullOrWhiteSpace(directoryToOpen) && Directory.Exists(directoryToOpen))
                        {
                            try { dlg.InitialDirectory = directoryToOpen; } catch { }
                        }
                        else
                        {
                            // Desktop is a safe fallback
                            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        }

                        // Show dialog on UI thread
                        var result = dlg.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            string selectedDirectory = Path.GetDirectoryName(dlg.FileName);
                            if (!string.IsNullOrWhiteSpace(selectedDirectory) && Directory.Exists(selectedDirectory))
                            {
                                SLF.Properties.Settings.Default.LastFileDialogDirectory = selectedDirectory;
                                SLF.Properties.Settings.Default.Save();
                            }
                            return dlg.FileName;
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // transient error: try to recover by pumping messages and giving the shell time
                    try
                    {
                        Application.DoEvents();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(150 * attempt);
                    }
                    catch { }

                    if (attempt == maxAttempts)
                    {
                        // final failure: show user-friendly message and return null
                        MessageBox.Show("Dosya seçme penceresi açılırken bir hata oluştu. Lütfen uygulamayı yeniden başlatın veya Farklı bir klasör deneyin.\n\nHata: " + ex.Message,
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }

            return null;
        }
    }
}
