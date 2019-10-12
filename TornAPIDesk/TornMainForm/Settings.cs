using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TornMainForm
{
    public class AppSettings
    {
        public string APIkey { get; set; }

        public static AppSettings loadSettings()
        {
            if (File.Exists(MainForm.Settings.SettingsFileName))
            {
                StreamReader sr = new StreamReader(MainForm.Settings.SettingsFileName);
                AppSettings a = JsonConvert.DeserializeObject<AppSettings>(sr.ReadToEnd());
                MainForm.Settings.APIKey = a.APIkey;
                sr.Close();
                return a;
            }
            else
            {
                return new AppSettings();
            }            
        }

        public  void saveSettings()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            StreamWriter sw = new StreamWriter(MainForm.Settings.SettingsFileName);
            sw.Write(json);
            sw.Close();
        }


    }
}
