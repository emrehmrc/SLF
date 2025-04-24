using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Reflection;

namespace SLF.Optimal_DTR
{


    internal class ConfigDosyasi
    {

        public string exeLocation;
        public string projectRoot;

        public string json_file;
        public dynamic config;

        // Resolve the Excel file path relative to SLF.exe
        

        public void SaveConfigToFile()
        {
            exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // e.g., C:\Users\ehan0\source\repos\emrehmrc\SLF\bin\Debug
            projectRoot = Directory.GetParent(exeLocation)?.Parent?.FullName; // Move up two levels to SLF root (C:\Users\ehan0\source\repos\emrehmrc\SLF)

            // read the json file and create the "config" variable.
            json_file = File.ReadAllText(Path.Combine(projectRoot, "config.json"));
            config = JsonConvert.DeserializeObject(json_file);




        }
    }

}
