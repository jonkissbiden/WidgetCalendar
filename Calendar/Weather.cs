using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public class Weather
    {
        public string name { get; set; }
        public IList<Info> weather { get; set; }
        public Main main { get; set; }
        public string visibility { get; set; }
        public Wind wind { get; set; }

        public static string Get()
        {
            try
            {
                string API_KEY = "3317482834e00f47218dee4c1ef70556";
                string lang = Properties.Settings.Default.Language.Remove(2);
                WebClient webClient = new WebClient();
                string ip = webClient.DownloadString("http://ipv4.icanhazip.com").Trim();
                string[] latlong = webClient.DownloadString("https://ipapi.co/" + ip + "/latlong/").Split(",");
                string res = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?lat=" + latlong[0] + "&lon=" + latlong[1] + "&lang=" + lang + "&units=metric" + "&appid=" + API_KEY);
                Weather result = JsonConvert.DeserializeObject<Weather>(res);
                string weatherString = result.name;
                foreach(Info i in result.weather)
                {
                    weatherString += ", " + i.description;
                }
                weatherString += ", " + result.main.temp + "℃";
                return weatherString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public class Info
        {
            public string id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }
        public class Main
        {
            public string temp { get; set; }
            public string feels_like { get; set; }
            public string temp_min { get; set; }
            public string temp_max { get; set; }
            public string pressure { get; set; }
            public string humidity { get; set; }
        }
        public class Wind
        {
            public string speed { get; set; }
            public string deg { get; set; }
        }
        public class Sys
        {
            public string country { get; set; }
            public string sunrise { get; set; }
            public string sunset { get; set; }
        }
    }
}
