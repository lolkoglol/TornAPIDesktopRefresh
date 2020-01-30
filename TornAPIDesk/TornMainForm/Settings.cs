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
        public string UserInfoForeGround { get; set; }      
        public bool DarkMode { get; set; }

        public static AppSettings loadSettings()
        {
            if (File.Exists(MainForm1.Settings.SettingsFileName))
            {
                StreamReader sr = new StreamReader(MainForm1.Settings.SettingsFileName);
                AppSettings a = JsonConvert.DeserializeObject<AppSettings>(sr.ReadToEnd());
                MainForm1.Settings.APIKey = a.APIkey;
                MainForm1.Settings.UserInfoForeGround = a.UserInfoForeGround;              
                if (a.DarkMode == true)
                {
                    MainForm1.Settings.DarkMode = true;
                }

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
            StreamWriter sw = new StreamWriter(MainForm1.Settings.SettingsFileName);
            sw.Write(json);
            sw.Close();
        }


    }
}
