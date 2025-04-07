using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SLF.services
{
    public static class HelperService
    {

        public static void isİmportedModule(bool isImported, string seçilenVeriTipi,
                                            List<string> modulesCheck, ComboBox veri_listesi_seçimi,
                                            DataGridView dataGridView, GirdiModülü girdiModülü)
        {
            modulesCheck.Add(seçilenVeriTipi);
            veri_listesi_seçimi.Refresh();
            Console.WriteLine(modulesCheck.Count);
            dataGridView.DataSource = girdiModülü.CurrentDataTable;
        }
    }
}
