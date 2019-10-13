﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TornMainForm
{
    public partial class MainForm : Form
    {
        public static string APIKey = null; // api key 
        public MainForm()
        {
            InitializeComponent();
        }

        private void TornAPIKey_TextChanged(object sender, EventArgs e)
        {
            MainForm.APIKey = TornAPIKey.Text; // api key assgined
        }

        private void ApiKeyLockcbx_CheckedChanged(object sender, EventArgs e)
        {
            if (ApiKeyLockcbx.Checked == true)
            {
                TornAPIKey.ReadOnly = true;
            }
            if (ApiKeyLockcbx.Checked == false)
            {
                TornAPIKey.ReadOnly = false;
            }
        }
        public class ItemsAndId
        {
            public JToken ItemName = null;
            public string ItemId = null;
        }

        public static class MyFunctions
        {
            /// <summary>
            /// switch options: 1 = user , 2 = property , 3 = faction , 4 = company , 5 = market , 6 = torn. fields = The api options (children) from the switch description option.
            /// </summary>
            /// <param name="switchOption"></param>
            /// <param name="feilds"></param>
            /// <returns></returns>
            public static string FetchUserData(int switchOption, string fields,string VarToPlaceData) // function to request and receive API data.
            {
                string test = null;
                switch (switchOption)
                {
                    case 1:
                        test = string.Format("https://api.torn.com/user/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                    case 2:
                        test = string.Format("https://api.torn.com/property/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                    case 3:
                        test = test = string.Format("https://api.torn.com/faction/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                    case 4:
                        test = string.Format("https://api.torn.com/company/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                    case 5:
                        test = string.Format("https://api.torn.com/market/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                    case 6:
                        test = test = string.Format("https://api.torn.com/torn/?selections=" + fields + "&key=" + MainForm.APIKey);
                        break;
                }

                WebRequest RequestBasic = WebRequest.Create(test);
                RequestBasic.Method = "GET";
                HttpWebResponse ResponseBasic = null;
                ResponseBasic = (HttpWebResponse)RequestBasic.GetResponse();

                string strresulttest = null;
                using (Stream stream = ResponseBasic.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresulttest = sr.ReadToEnd();
                    VarToPlaceData = strresulttest;
                    sr.Close();

                    return strresulttest;
                }
            }

            public static DialogResult APIErrorChecks() //Function to check if api returns errors
            {


                WebRequest RequestBasic = WebRequest.Create("https://api.torn.com/user/?selections=basic&" + MainForm.APIKey);
                RequestBasic.Method = "GET";
                HttpWebResponse ResponseBasic = null;
                ResponseBasic = (HttpWebResponse)RequestBasic.GetResponse();

                string strresulttest = null;
                using (Stream stream = ResponseBasic.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                     strresulttest = sr.ReadToEnd();                   
                    sr.Close();

                    string feedback = strresulttest;
                    var JJJ = JObject.Parse(feedback);
                    var sss = JObject.Parse(Convert.ToString(JJJ));
                    var aaa = JObject.Parse(Convert.ToString(sss["error"]));
                  //  var Error = JObject.Parse(Convert.ToString(aaa["error"]));

                    return MessageBox.Show(Convert.ToString(aaa));
                    

                }


            }

            public static void TimerCountdownWithTicks(JToken JsonFrom, Label YourLabal, string JsonStringdataname)
            {
                if (Convert.ToInt32(JsonFrom[JsonStringdataname]) > 1)
                {
                    TimeSpan TimeTick = new TimeSpan();
                    JsonFrom[JsonStringdataname] = Convert.ToInt32(JsonFrom[JsonStringdataname]) - 1;
                    string TickDown = Convert.ToString(Convert.ToInt32(JsonFrom[JsonStringdataname]) - 1);
                    TimeTick = TimeSpan.FromSeconds(Convert.ToInt32(TickDown));
                    YourLabal.Text = String.Format(Convert.ToString(TimeTick), "MM:ss");
                }
            }
            /// <summary>
            /// Function limited to one parent hierarchy. FromwhatParent should equal the top hierarchy name inside the API called. The child can only be one step away/down from parent.  
            /// </summary>
            /// <param name="DictToStore"></param>
            /// <param name="FromWhatParent"></param>
            /// <param name="TheChildYouWant"></param>            
            /// <param name="VartoPullStoredJsonDataFrom"></param>
            /// <param name="knownIdUpperInt"></param>
            public static void AddJsonDataToDictionary(Dictionary<string, string> DictToStore, string FromWhatParent, string TheChildYouWant, string VartoPullStoredJsonDataFrom, int knownIdUpperInt) // get values from parent and add them to dictionary.
            {

                string datacollected = VartoPullStoredJsonDataFrom;  // fetching api url data
                var readabledata = JObject.Parse(datacollected);
                var Id = JObject.Parse(Convert.ToString(readabledata));
                var f = JObject.Parse(Convert.ToString(Id[FromWhatParent]));

                for (int i = 0; i < knownIdUpperInt; i++)
                {
                    try
                    {
                        var j = JObject.Parse(Convert.ToString(f[Convert.ToString(i)]));
                        DictToStore.Add(Convert.ToString(i), (Convert.ToString(j[TheChildYouWant])));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            /// <summary>
            /// a1 = Name a2 = Available Shares a3 = Current Price a4 = ForeCast a5 = Demand.
            /// </summary>
            /// <param name="Name"></param>
            /// <param name="AvailableShare"></param>
            /// <param name="CurrentPrice"></param>
            /// <param name="ForeCast"></param>
            /// <param name="Demand"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public static string StockInfoAsString(List<string> Name, List<string> AvailableShare, List<string> CurrentPrice, List<string> ForeCast, List<string> Demand, int index)
            {
                
                string Output = " Name: " + Name[index] + Environment.NewLine + " Available Shares: " +  AvailableShare[index] + Environment.NewLine
                       + " Current Price: " + String.Format("{0:c}", CurrentPrice[index]) + Environment.NewLine
                       + " forecast: " + ForeCast[index] + Environment.NewLine +  " Demand: " + Demand[index];
                return Output;
            }
        }

        public class FileReadWriteLocations
        {
            public static string FileToSaveItemList = null;
        }
        public class Settings
        {
            public static string APIKey = null;
            public static JToken ParsedSettingsData = null;
            public static string SettingsFileName = Directory.GetCurrentDirectory() + "\\Settings.json";
            public static string ItemFileName = Directory.GetCurrentDirectory() + "\\Items.json";
            public static int StockButtonTimerLimit = 12;
            public static int ItemRefreshLimit = 12;

        }

        public class TornData //variables to store Data obtained from Torn API
        {
            public static Dictionary<string, string> StocksIDandNames = new Dictionary<string, string>(); // value to store stock ID and Names for ID value.
            public static Dictionary<string, string> StockIDandCurrentPrice = new Dictionary<string, string>(); // value to store stock ID and CurrentPrice.
            public static Dictionary<string, string> StockIDandAvailableshares = new Dictionary<string, string>();
            public static Dictionary<string, string> StockIdandForecast = new Dictionary<string, string>();
            public static Dictionary<string, string> StockIdandDemand = new Dictionary<string, string>();
            public static Dictionary<string, string> StockIdandacronym = new Dictionary<string, string>();
            public static Dictionary<string, string> ItemsIdAndName = new Dictionary<string, string>();
            public static List<string> ItemNamesList = new List<string>(); //store list values  to add to combo box items            
            public static List<string> ItemIdList = new List<string>(); // store list values  to add to combo box items
            public static bool ItemLoaded = false; // value to determine if items and ids were loaded already
            public static bool ReadytoFetchitems = false;
            public static string TornStockInfo = null;
            public static string TornTime = null;
            public static JToken TornItemNames = null;
            public static string TornJsonFetchedInfo = null;
            public static string NameThenIDofItems = null;
            public static List<string> name = null;
            public static List<string> acronym = null;
            public static bool StockTimerActive = false;
            public static string Stock0 = null;     public static string Stock1 = null;          public static string Stock2 = null;
            public static string Stock3 = null;     public static string Stock4 = null;          public static string Stock5 = null;
            public static string Stock6 = null;     public static string Stock7 = null;          public static string Stock8 = null;
            public static string Stock9 = null;     public static string Stock10 = null;
            public static string Stock11 = null;    public static string Stock12 = null;          public static string Stock13 = null;
            public static string Stock14 = null;    public static string Stock15 = null;          public static string Stock16 = null;
            public static string Stock17 = null;    public static string Stock18 = null;          public static string Stock19 = null;
            public static string Stock20 = null;    public static string Stock21 = null;          public static string Stock22 = null;
            public static string Stock23 = null;    public static string Stock24 = null;          public static string Stock25 = null;
            public static string Stock26 = null;    public static string Stock27 = null;          public static string Stock28 = null;
            public static string Stock29 = null;    public static string Stock30 = null;

        }

        public class UserData // variables to store Data obtained from user API
        {
            public static JToken Notifications = null;
            public static string Basic = null; // value to become json string
            public static JObject User = null; // contain feteched user data from json string
            public static string level = null;
            public static string gender = null;
            public static string name = null;
            public static JToken Bank = null; //  value for bank information such as money in bank and time left for investment to end.
            public static string player_id = null;
            public static string status = null; // Value will be based on the status from the profile API 
            public static JToken Chainjson = null; // will contain all details about the Chain bar
            public static JToken Lifejson = null; // will contain all details about the life bar
            public static JToken Nervejson = null; // will contain all details about the nerve bar
            public static JToken Energyjson = null; // will contain all details about the energy bar
            public static JToken Happyjson = null; // will contain all details about the happy bar
            public static JToken factionjson = null; // will contain details of the users faction name. Value is fetched from profile API
            public static JToken companyJson = null;
            public static JToken money = null;
            public static JToken DBMCooldowns = null;
            public static string ChainCooldowns = null;
            public static JToken travel = null;
            public static JToken Events = null;

            public static long TimerAble = 0; // when timerable is > 0 the refreshtimer will automate itself. when an exception occurs value is put to 0 which turns timer off.
            public static string StatusLink = null;
            public static TimeSpan ts = TimeSpan.FromSeconds(0);
            public static string SetValue(string jsonString, string StoreVar, string FetchedValue) // fetch value for data you want and name to store it as. level/gender ect..

            {
                var Js = JObject.Parse(jsonString); // make json an var array?
                StoreVar = Convert.ToString(Js[FetchedValue]); // setvalue becomes the json feteched value
                return StoreVar; // return value so we can also set textbox value as the function. quicker assign.
            }
        }

        private void button1_Click(object sender, EventArgs e) // fetching Tab 1 Data
        {
              try
              {
            if (UserData.TimerAble == 0) // stops timer if exception occured. although if api is fixed within its interval will continue as normal or untill clicked again.
            {
                Refreshtimer.Stop();
            }
            if (APIKey.Length == 16)
            {
                APILengthWarning.Visible = false; // turn off API warning label 

                ButtonLimittimer.Start();
                GetDatabtn.Enabled = false;

                OneSecondtimer.Start();
                GetDatabtn.Text = Convert.ToString(ButtonLimittimer.Interval / 1000);

                UserData.Basic = MyFunctions.FetchUserData(1, "basic,profile,bars,money,cooldowns,notifications,travel,events", UserData.Basic); // Fields to fetch

                UserData.User = JObject.Parse(UserData.Basic); // parse to fetchable jtoken data.
                var details = JObject.Parse(UserData.Basic); // makes json string data callable. 

                lvlValuelbl.Text = UserData.SetValue(UserData.Basic, UserData.level, "level");
                GenderValuelbl.Text = UserData.SetValue(UserData.Basic, UserData.gender, "gender");
                NameValuelbl.Text = Convert.ToString(details["name"]);
                IDValuelbl.Text = Convert.ToString(details["player_id"]);
                TornData.TornTime = Convert.ToString(details["server_time"]);
                UserData.ChainCooldowns = Convert.ToString(details["cooldowns"]); 

                string status = Convert.ToString(details["status"]).Trim(new char[] { '[', ']', ' ', ',', '"', '.' }).Replace("\"", string.Empty).Replace(",", string.Empty);

                Statuslbl.Text = "Status: " + status;
                UserData.Lifejson = details["life"]; LifeValue.Text = Convert.ToString(UserData.Lifejson["current"] + " / " + UserData.Lifejson["maximum"]);
                UserData.Energyjson = details["energy"]; EnergyValuelbl.Text = Convert.ToString(UserData.Energyjson["current"] + " / " + UserData.Energyjson["maximum"]);
                UserData.Nervejson = details["nerve"]; NerveValuelbl.Text = Convert.ToString(UserData.Nervejson["current"] + " / " + UserData.Nervejson["maximum"]);
                UserData.Happyjson = details["happy"]; HappyValuelbl.Text = Convert.ToString(UserData.Happyjson["current"] + " / " + UserData.Happyjson["maximum"]);
                UserData.Chainjson = details["chain"]; ChainValuelbl.Text = Convert.ToString(UserData.Chainjson["current"]);
                UserData.factionjson = details["faction"];
                UserData.companyJson = details["job"];
                UserData.travel = details["travel"];

                // Points, money values
                PointsValuelbl.Text = "Points " + Convert.ToString(String.Format("{0:n0}", UserData.User["points"]));
                MoneyOnHandlbl.Text = "Money on hand: " + Convert.ToString("$" + String.Format("{0:n0}", UserData.User["money_onhand"]));
                MoneyInVaultlbl.Text = "Money in Vault: " + Convert.ToString("$" + String.Format("{0:n0}", UserData.User["vault_amount"]));
                CaymanbankValuelbl.Text = "Money in Cayman's: " + Convert.ToString("$" + String.Format("{0:n0}", UserData.User["cayman_bank"]));
                UserData.Bank = UserData.User["city_bank"]; // bank values 
                CityBankValuelbl.Text = "Money in Bank: " + Convert.ToString("$" + String.Format("{0:n0}", UserData.Bank["amount"]));
                CoolDownValuelbl.Text = Convert.ToString(UserData.Chainjson["cooldown"]);
                UserData.DBMCooldowns = details["cooldowns"];
             
                UserData.Notifications = details["notifications"];
                NewEventValuelbl.Text = "New Events [" + Convert.ToString(UserData.Notifications["events"]) + "]";
                NewMessagesValuelbl.Text = "New Messages [" + Convert.ToString(UserData.Notifications["messages"]) + "]";
                if (Convert.ToInt32(UserData.travel["time_left"]) < 1)
                {
                    Traveltimelbl.Visible = false;
                    TravelTimeValuelbl.Visible = false;
                }
                ChainTimeOutValuelbl.Text = Convert.ToString(UserData.Chainjson["timeout"]);

                // throw new Exception();

                //Event page 
                List<string> EventId = new List<string>();
                List<string> EventEvent = new List<string>();
                UserData.Events = UserData.User["events"];
                var LS = JsonConvert.SerializeObject(UserData.Events);
                JObject tfe = JObject.Parse(LS);
                foreach (var item in tfe)
                {
                    EventId.Add(Convert.ToString(item));
                }
                // EventId.Reverse();
                foreach (var item in EventId)
                {
                        try
                        {
                            string.Concat(item, "evnt");
                            string resultString = Regex.Match(item, @"\d+").Value; // finds first occurance of number for event id

                            JToken Idcontent = UserData.Events[resultString];
                            string AcEvent = Convert.ToString(Idcontent["event"]);
                            // making events neater
                            AcEvent = AcEvent.Trim(new Char[] { ' ', '*', '<', '>', '[', ']', '^', '/', '\\', 'a', '=' });
                            AcEvent = AcEvent.Replace("href", "");                          
                            AcEvent = AcEvent.Replace("</a>", "");
                            AcEvent = AcEvent.Replace("<b>", "");
                            AcEvent = AcEvent.Replace("[<a", "");
                            AcEvent = AcEvent.Replace("<//b>", "");                                                     
                           
                            AcEvent = Regex.Replace(AcEvent, "XID", " ");
                            AcEvent = Regex.Replace(AcEvent, @"[^0-9a-zA-Z:,. ]+", "");

                            AcEvent = Regex.Replace(AcEvent, "http:www.torn.comprofiles.php?", " ");
                            AcEvent = Regex.Replace(AcEvent, "http:www.torn.comstockexchange.php", " ");
                            AcEvent = Regex.Replace(AcEvent, "http:www.torn.comloader.php", " ");
                            AcEvent = Regex.Replace(AcEvent, "http:www.torn.comorganisedcrimes.php", " ");
                            AcEvent = Regex.Replace(AcEvent, "sidracingtablograce", " ");
                            AcEvent = Regex.Replace(AcEvent, "http:www.torn.comjoblist.phppcorpinfoID", " ");
                            AcEvent = Regex.Replace(AcEvent, "http: www.torn.comjoblist.phppcorpinfoID", " ");
                            AcEvent = AcEvent.Replace("view"," ");                            

                            AcEvent = Regex.Replace(AcEvent," [\\w]{ 1, 2}", " ");
                           // AcEvent = Regex.Replace(AcEvent, " [\\w]{ 18, 26}", " ");
                            AcEvent = AcEvent.TrimStart(' ');
                         
                            EventEvent.Add(AcEvent);
                            richTextBox1.Text = richTextBox1.Text + AcEvent + Environment.NewLine + Environment.NewLine;
                        }
                        catch (Exception)
                        {

                            continue;
                        }
        }                              

                ApiKeyLockcbx.Checked = true;
                UserData.TimerAble += 1; // when timerable is > 0 the refreshtimer will automate itself. when an exception occurs value is put to 0 which turns timer off.

                int f = Refreshtimer.Interval / 1000; // value of refresh rate

                RefreshValuelbl.Text = Convert.ToString(f);
                OneSecondtimeTwo.Start(); // this timer should turn on last to prevent errors. 
                if (UserData.TimerAble > 0)
                {
                    Refreshtimer.Start();
                }
            }
            if (APIKey.Length != 16)
            {
                APILengthWarning.Visible = true;
            }

        }
            catch (Exception)
           {
                try
                {
                                   
                MyFunctions.APIErrorChecks();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: 400");               
                }
                OneSecondtimeTwo.Stop();
                UserData.TimerAble = 0; // when timerable is > 0 the refreshtimer will automate itself. when an exception occurs value is put to 0 which turns timer off. value is increased by button press
           }
        }

        private void VisitTornlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.torn.com");
        }

        private void Refreshtimer_Tick(object sender, EventArgs e)
        {
            GetDatabtn.PerformClick();
        }

        private void ButtonLimittimer_Tick(object sender, EventArgs e)
        {
            GetDatabtn.Enabled = true;
            ButtonLimittimer.Stop();
        }

        private void OneSecondtimer_Tick(object sender, EventArgs e) // this timer is for the GetData button
        {
            GetDatabtn.Text = Convert.ToString(Convert.ToInt16(GetDatabtn.Text) - 1); // changing datatext value to decrease by 1 per second
            if (Convert.ToInt32(GetDatabtn.Text) < 1)
            {
                OneSecondtimer.Stop();
                GetDatabtn.Text = "GetData"; // reset text to original which indicates purpose
            }         
          
        }

        private void OneSecondtimeTwo_Tick(object sender, EventArgs e) // timer for each second passing
        {
            MyFunctions.TimerCountdownWithTicks(UserData.Energyjson, EnergyIncrementlbl, "ticktime");
            MyFunctions.TimerCountdownWithTicks(UserData.Nervejson, NerveTimerValuelbl, "ticktime");
            MyFunctions.TimerCountdownWithTicks(UserData.Lifejson, LifeTickValuelbl, "ticktime");
            MyFunctions.TimerCountdownWithTicks(UserData.Happyjson, HappyTickValuelbl, "ticktime");
            MyFunctions.TimerCountdownWithTicks(UserData.DBMCooldowns, DrugCooldownValuelbl, "drug");
            MyFunctions.TimerCountdownWithTicks(UserData.DBMCooldowns, MedicalCooldownValue, "medical");
            MyFunctions.TimerCountdownWithTicks(UserData.DBMCooldowns, BoosterCdValuelbl, "booster");
            MyFunctions.TimerCountdownWithTicks(UserData.Chainjson, CoolDownValuelbl, "cooldown");
            MyFunctions.TimerCountdownWithTicks(UserData.Bank, BankTimeLeftValuelbl, "time_left");
            if (Convert.ToInt32(UserData.travel["time_left"]) > 1 )
            {
                MyFunctions.TimerCountdownWithTicks(UserData.travel, TravelTimeValuelbl, "time_left");
                TravelTimeValuelbl.Visible = true;
                Traveltimelbl.Visible = true;
            }

            if (Convert.ToInt32(ChainTimeOutValuelbl.Text) > 0)
            {
                ChainTimeOutValuelbl.Text = Convert.ToString(Convert.ToInt32(ChainTimeOutValuelbl.Text) - 1);
            }

            if (Statuslbl.Text.Contains('<')) // shortens status attacking info to link to attackers profile.
            {
                string statusBegin = Statuslbl.Text.Split('<')[0];
                string statusend = Statuslbl.Text.Split('\\')[1];
                Statuslbl.Text = statusBegin;

                UserData.StatusLink = "https://www.torn.com/" + statusend;
                StatusLinkProfileValuelbl.Text = statusend;
               
                StatusLinkProfileValuelbl.Visible = true;

            }

            RefreshValuelbl.Text = Convert.ToString(Convert.ToInt32(RefreshValuelbl.Text) - 1); // decrease refresh value by 1 per timer tick which should be 1 second.

            try
            {
                DateTime begginingoftime = new DateTime(1970, 01, 01);

                var details = JObject.Parse(Convert.ToString(UserData.User));
                TornData.TornTime = Convert.ToString(Convert.ToInt64(TornData.TornTime) + 1);
                TimeSpan torntime = TimeSpan.FromSeconds(Convert.ToUInt64(TornData.TornTime) + 1);
                begginingoftime = begginingoftime + torntime;
                TornCityTimelbl.Text = Convert.ToString("TCT: " + begginingoftime);

            }
            catch (Exception)
            {
                TornCityTimelbl.Text = "0";
            }          

        }

        private void StatusLinkProfileValuelbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(UserData.StatusLink);
        }
        ///////////////////////////////////////////////////Tab 2 Start////////////////////////////////////////////////////////////////
        
        private void StockGetDatabtn_Click(object sender, EventArgs e)
        {
            new Thread(() => // new thread is used for more cpu intense tasks. this will allow the user constant app use as its main thread will not be too busy.
          {
              try
              {
                  TornData.TornJsonFetchedInfo = MyFunctions.FetchUserData(6, "stocks,items", TornData.TornJsonFetchedInfo);                    
                  
             if (TornData.StocksIDandNames.ContainsKey("1") == false) // once information is feteched there is no need to update it as it stays constant.
          {
              MyFunctions.AddJsonDataToDictionary(TornData.StockIdandacronym, "stocks", "acronym", TornData.TornJsonFetchedInfo, 33);
                TornData.acronym = TornData.StockIdandacronym.Values.ToList(); 
              MyFunctions.AddJsonDataToDictionary(TornData.StocksIDandNames, "stocks", "name", TornData.TornJsonFetchedInfo, 33);
                TornData.name = TornData.StocksIDandNames.Values.ToList();  
          }
    MyFunctions.AddJsonDataToDictionary(TornData.StockIDandCurrentPrice, "stocks", "current_price", TornData.TornJsonFetchedInfo, 33);
                  List<string> CurrentPrices = TornData.StockIDandCurrentPrice.Values.ToList();
    MyFunctions.AddJsonDataToDictionary(TornData.StockIDandAvailableshares, "stocks", "available_shares", TornData.TornJsonFetchedInfo, 33);
              List<string> AvailableShares = TornData.StockIDandAvailableshares.Values.ToList();                          
    MyFunctions.AddJsonDataToDictionary(TornData.StockIdandForecast, "stocks", "forecast", TornData.TornJsonFetchedInfo, 33);
                  List<string> forecast = TornData.StockIdandForecast.Values.ToList();
    Task.Delay(50);
            MyFunctions.AddJsonDataToDictionary(TornData.StockIdandDemand, "stocks", "demand", TornData.TornJsonFetchedInfo, 33);
                  List<string> demand = TornData.StockIdandDemand.Values.ToList();
            //Values for globals which hold stock information
              TornData.Stock0 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 0);
              TornData.Stock1 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 1);
              TornData.Stock2 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand,2 );
              TornData.Stock3 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 3);
              TornData.Stock4 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand,4);
              TornData.Stock5 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 5);
              TornData.Stock6 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 6);
              TornData.Stock7 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 7);
              TornData.Stock8 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 8);
              TornData.Stock9 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 9);
              TornData.Stock10 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 10);
              TornData.Stock11 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 11);
              TornData.Stock12 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 12);
              TornData.Stock13 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 13);
              TornData.Stock14 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 14);
              TornData.Stock15 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 15);
              TornData.Stock16 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 16);
              TornData.Stock17 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 17);
              TornData.Stock18 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 18);
              TornData.Stock19 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 19);
              TornData.Stock20 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand,20);
              TornData.Stock21 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 21);
              TornData.Stock22 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 22);
              TornData.Stock23 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 23);
              TornData.Stock24 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 24);
              TornData.Stock25 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 25);
              TornData.Stock26 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 26);
              TornData.Stock27 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 27);
              TornData.Stock28 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 28);
              TornData.Stock29 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 29);
              TornData.Stock30 = MyFunctions.StockInfoAsString(TornData.name, AvailableShares, CurrentPrices, forecast, demand, 30);                         

              TornData.StockTimerActive = true; // this will active timer to update info when button stock is pressed. is turned off by itself when activated.
                 }
               catch (Exception)
               {
                   MessageBox.Show("Error Report:300");
               }


          }).Start();
            //  throw new Exception();    
            try
            {
                TornData.StockTimerActive = true;
                StockActivateTimer.Start();
                StockInfoRefreshLimit.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Error Report:100. message superduperpoor if this continues");
            }
               
        }

        //FileReadToFromBrowseButton
        private void BrowseFileToReadAndSavebtn_Click(object sender, EventArgs e)
        {
            using (var FileReadToFromBrowseButton = new FolderBrowserDialog())
            {
                DialogResult result = FileReadToFromBrowseButton.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FileReadToFromBrowseButton.SelectedPath))
                {                                     

                }
                FileToReadtoAndSaveTotxtbox.Text = FileReadToFromBrowseButton.SelectedPath;
                FileReadWriteLocations.FileToSaveItemList = FileToReadtoAndSaveTotxtbox.Text;
            }
            //FileToReadtoAndSaveTotxtbox.Text = FileReadToFromBrowseButton.SelectedPath;
          //  FileReadWriteLocations.FileToSaveItemList = FileToReadtoAndSaveTotxtbox.Text;
        }   
      

        private void GetItemNamesAndIdbtn_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {                
                TornData.TornJsonFetchedInfo = MyFunctions.FetchUserData(6,"items,stocks", TornData.TornJsonFetchedInfo);
                }
                catch (Exception)
                {
                    MessageBox.Show("API problem: 200");
                }
                
                //  This area was used to create a file containing all items and id's of them.
                //TODO Write feature to fetch item circulation and name by using the Files Name which fetchs ID then fetching circulation.
                try
               {
                
                if (File.Exists(Settings.ItemFileName) == false )// create file if it does not exsist
                    {
                        MyFunctions.AddJsonDataToDictionary(TornData.ItemsIdAndName, "items", "name", TornData.TornJsonFetchedInfo, 1003); //fetch items and add to dict
                        TornData.ItemIdList = TornData.ItemsIdAndName.Keys.ToList();
                        TornData.ItemNamesList = TornData.ItemsIdAndName.Values.ToList();                    
               

                   // make object for item name and id. add them to object. convert to json. append to json file.  

                   JObject acd = new JObject( new JProperty ( JsonConvert.SerializeObject("e")));
                    for (int i = 0; i < TornData.ItemsIdAndName.Keys.Count ; i++)
                    {                                         
                         JProperty f = new JProperty(JsonConvert.SerializeObject(TornData.ItemNamesList[i]), TornData.ItemIdList[i]); //added items and id to json file.
                        try
                        {
                            acd.Add(f);
                        }
                        catch (Exception)
                        {
                            continue;
                        }                                                             
                    }
                    
                    File.AppendAllText(Settings.ItemFileName, Convert.ToString(acd));
                }
                if (File.Exists(Settings.ItemFileName) == true & TornData.ItemLoaded == false)
                {
                    MyFunctions.AddJsonDataToDictionary(TornData.ItemsIdAndName, "items", "name", TornData.TornJsonFetchedInfo, 1003);
                    TornData.NameThenIDofItems = File.ReadAllText(Settings.ItemFileName);
                    TornData.TornItemNames = Convert.ToString(JObject.Parse(TornData.NameThenIDofItems));                 
                    TornData.ItemIdList = TornData.ItemsIdAndName.Keys.ToList();
                    TornData.ItemNamesList = TornData.ItemsIdAndName.Values.ToList();
                                                                           
                }
             }
               catch (Exception)
                {
                   MessageBox.Show("Is your executable in a file you have read and write permissions?");
                }
                TornData.ReadytoFetchitems = true;

              
                FetchItemsTimer.Start();
            }).Start();

            if (TornData.ReadytoFetchitems == false)
            {
                Thread.Sleep(2500);
            }

            if (TornData.ReadytoFetchitems == true & TornData.ItemLoaded == false)
            {
                foreach (var item in TornData.ItemNamesList)
                {
                    ItemCombobox.Items.Add(item);
                }
                ItemCombobox.Enabled = true;
                ItemSearchbtn.Enabled = true;
                TornData.ItemLoaded = true;
                LoadItemRefreshLimiter.Start();
            }
            LoadItemRefreshLimiter.Start();
        }

        private void SettingsAPILockchkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (SettingsAPILockchkbox.Checked == true)
            {
                SettingsAPIKeyValuetxtbox.ReadOnly = true;
            }
            if (SettingsAPILockchkbox.Checked == false)
            {
                SettingsAPIKeyValuetxtbox.ReadOnly = false;
            }
        }       
        
        private void SaveSettingsbtn_Click(object sender, EventArgs e) //Settings Read 
        {
            try
            {
                AppSettings f = new AppSettings();
                f.APIkey = SettingsAPIKeyValuetxtbox.Text;
                f.saveSettings();
            }
            catch (Exception)
            { 
                MessageBox.Show("APIKEY not saved go to settings to do so. If this message continues make sure you have read/write permissions in current directory");
            }
        }      

        private void MainForm_Load(object sender, EventArgs e) //TODO fix file load and json parsing of saved settings.
        {
           
            AppSettings.loadSettings();
            SettingsAPIKeyValuetxtbox.Text = Settings.APIKey.Trim(' ');
            TornAPIKey.Text = Settings.APIKey.Trim(' '); 
            MainForm.APIKey = SettingsAPIKeyValuetxtbox.Text.Trim(' ');
            ApiKeyLockcbx.Checked = true;
            
            string Creator = "Creator: ";
            int CreatorLength = Creator.Length;
            Creatorlinklabel.Text = Creator + "SuperDuperPoor  [1036235]    ";
            Creatorlinklabel.LinkArea = this.Creatorlinklabel.LinkArea = new LinkArea(CreatorLength,Creatorlinklabel.Text.Length);
                                    
            if (SettingsAPILockchkbox.Checked == true)
            {
                SettingsAPIKeyValuetxtbox.ReadOnly = true;
            }
            if (SettingsAPILockchkbox.Checked == false)
            {
                SettingsAPIKeyValuetxtbox.ReadOnly = false;
            }
        }
        
        private void Creatorlinklabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.torn.com/profiles.php?XID=1036235");
        }

        private void StockActivateTimer_Tick(object sender, EventArgs e) // stock values
        {
            TornCityStockExchangelbl.Text = TornData.Stock0;
            StockInfo2lbl.Text = TornData.Stock1;            StockInfo3lbl.Text = TornData.Stock2;            Stockinfo4lbl.Text = TornData.Stock3;
            StockInfo5lbl.Text = TornData.Stock4;            StockInfo6lbl.Text = TornData.Stock5;            StockInfo7lbl.Text = TornData.Stock6;
            StockInfo8lbl.Text = TornData.Stock7;            StockInfo9lbl.Text = TornData.Stock8;            StockInfo10lbl.Text = TornData.Stock9;
            StockInfo11lbl.Text = TornData.Stock10;            StockInfo12lbl.Text = TornData.Stock11;            StockInfo13lbl.Text = TornData.Stock12;
            StockInfo14lbl.Text = TornData.Stock13;            StockInfo15lbl.Text = TornData.Stock14;            StockInfo16lbl.Text = TornData.Stock15;
            StockInfo17lbl.Text = TornData.Stock16;            StockInfo18lbl.Text = TornData.Stock17;            StockInfo19lbl.Text = TornData.Stock18;
            StockInfo20lbl.Text = TornData.Stock19;            StockInfo21lbl.Text = TornData.Stock20;            StockInfo22lbl.Text = TornData.Stock21;
            StockInfo23lbl.Text = TornData.Stock22;            StockInfo24lbl.Text = TornData.Stock23;            StockInfo25lbl.Text = TornData.Stock24;
            StockInfo26lbl.Text = TornData.Stock25;            StockInfo27lbl.Text = TornData.Stock26;            StockInfo28lbl.Text = TornData.Stock27;
            StockInfo29lbl.Text = TornData.Stock28;            StockInfo30lbl.Text = TornData.Stock29;            StockInfo31lbl.Text = TornData.Stock30;

            TornData.StockTimerActive = false; // change value so timer does not re active.     
                        
        }

        private void FetchItemsTimer_Tick(object sender, EventArgs e)
        {
            foreach (var item in TornData.ItemNamesList)
            {
                ItemCombobox.Items.Add(item);
            }            

            FetchItemsTimer.Stop();
           
        }

        private void ItemSearchbtn_Click(object sender, EventArgs e)
        {
           try
          {
                string ItemToSearch = ItemCombobox.Text;
                ItemToSearch.TrimEnd(' ');
                string idmatch = null;
                int c = 0;
                foreach (var item in TornData.ItemNamesList)
                {
                    if (item == ItemToSearch)
                    {
                        idmatch = TornData.ItemIdList[c];
                        break;
                    }
                    c++;
                }
                // fetchs and sets item values.
            string mainData = TornData.TornJsonFetchedInfo;
            string loopgwer = mainData;
            var ofjf = JObject.Parse(loopgwer);
            var igrji = JObject.Parse(Convert.ToString(ofjf));
            var ITEMS = JObject.Parse(Convert.ToString(igrji["items"]));
            var ITEMID = JObject.Parse(Convert.ToString(ITEMS[idmatch]));
            var name = Convert.ToString(ITEMID["name"]);
            var Buyprice = Convert.ToString(ITEMID["buy_price"]);
            var sellprice = Convert.ToString(ITEMID["sell_price"]);
            var marketvalue = Convert.ToString(ITEMID["market_value"]);
            var Circulation = Convert.ToString(ITEMID["circulation"]);
            var Description = Convert.ToString(ITEMID["description"]);
            var Effect = Convert.ToString(ITEMID["effect"]);
                if (Effect == "")
                {
                    Effect = "N/A";
                }
                richTextBox2.Text = "Name: " + name + Environment.NewLine + "BuyPrice: " + Buyprice + Environment.NewLine +
                "SellPrice: " + String.Format("{0:n0}", sellprice) + Environment.NewLine + "Market Value: " + String.Format("{0:n0}", marketvalue) + Environment.NewLine + 
                "Circulation: " + String.Format("{0:n0}", Circulation) + Environment.NewLine + "Effect: "+ Effect + Environment.NewLine + "Description: " + Description ;         
            
            }
            catch (Exception)
              {
                 MessageBox.Show("Is the item spelled correctly? Did you add extra characters to the end of item string?");
             }           
            
        }

        private void StockInfoRefreshLimit_Tick(object sender, EventArgs e)
        {
            if (Settings.StockButtonTimerLimit >= 1)
            {
                StockGetDatabtn.Enabled = false;
                StockGetDatabtn.Text = Convert.ToString(Settings.StockButtonTimerLimit);
                Settings.StockButtonTimerLimit -= 1;
            }           

            if (Settings.StockButtonTimerLimit < 1)
            {
                StockGetDatabtn.Enabled = true;
                StockGetDatabtn.Text = "Get Stock Info";
                Settings.StockButtonTimerLimit = 12;
                StockInfoRefreshLimit.Stop();
            }
        }

        private void LoadItemRefreshLimiter_Tick(object sender, EventArgs e)
        {
            if (Settings.ItemRefreshLimit >= 1)
            {
                GetItemNamesAndIdbtn.Enabled = false;
                GetItemNamesAndIdbtn.Text = Convert.ToString(Settings.ItemRefreshLimit);
                Settings.ItemRefreshLimit -= 1;
            }

            if (Settings.ItemRefreshLimit < 1)
            {
                GetItemNamesAndIdbtn.Enabled = true;
                GetItemNamesAndIdbtn.Text = "Load Items and update API item data";
                Settings.ItemRefreshLimit = 12;
                LoadItemRefreshLimiter.Stop();
            }
        }
    }
    
}