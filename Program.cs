using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using SLF.Optimal_DTR;

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
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DTR_Arayuz());
            //Application.Run(new HomePageForm());
            //Application.Run(new ChargingStationPopupForm());
        }
    }
}


/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using static SLF.ModülFormu;

namespace SLF
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize a NoktaVeri object (make sure to set properties as needed)
            NoktaVeri veri = new NoktaVeri
            {
                Enlem = 13.454, // Set the latitude
                Boylam = -16.731, // Set the longitude
                Bina_Demandi = 1000, // Example value
                Abone_Sayısı = 10 // Example value
            };

            // Pass the veri object to the ChargingStationPopupForm
            Application.Run(new ChargingStationPopupForm(veri));
        }
    }
}*/
