using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;

namespace StockViewApplication
{
    [Serializable]
    public class StockItem
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public string CurrentPrice { get; set; }
        public string PercentageOfChangeFromLastDay { get; set; }
        public string FiftyTwoWeekHigh { get; set; }
        public string FiftyTwoWeekLow { get; set; }

        public string id { get; set; }
        public string t { get; set; }
        public string e { get; set; }

        public string l { get; set; }

        public string l_fix { get; set; }

        public string l_cur { get; set; }

        public string s { get; set; }

        public string ltt { get; set; }

        public string lt { get; set; }

        public string lt_dts { get; set; }

        public string c { get; set; }

        public string c_fix { get; set; }

        public string cp { get; set; }
        public string cp_fix { get; set; }

        public string ccol { get; set; }

        public string pcls_fix { get; set; }


        public static List<StockItem> FetchAllStocks(string symbols)
        {
            if (string.IsNullOrEmpty(symbols))
                symbols = ConfigurationManager.AppSettings["DefinedStocks"];

            string url = "http://finance.yahoo.com/d/quotes.csv?s="+symbols+"&f=snl1p2kj";
            return GetAllStockDetails(GetData(url));
            //WebRequest webRequest = WebRequest.Create("http://finance.google.com/finance/info?client=ig&q=" + symbols);
            //var response = new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd().Split(new string[] { "//" }, StringSplitOptions.None);
            //return DeserializeFromJson<List<StockItem>>(response[1]);
        }

        /// <summary>
        /// pull data based on a passed url text
        /// </summary>
        /// <param name="webpageUriString"></param>
        /// <returns></returns>
        public static string GetData(string webpageUriString)
        {
            string tempStorageString = "";

            try
            {
                if (webpageUriString != "")
                {
                    //create a new instance of the class
                    //using should properly close and dispose so we don't have to bother
                    using (var webClient = new WebClient())
                    {
                        using (Stream responseStream = webClient.OpenRead(webpageUriString))
                        {
                            using (StreamReader responseStreamReader = new StreamReader(responseStream))
                            {
                                //extract the response we got from the internet server
                                tempStorageString = responseStreamReader.ReadToEnd();

                                //change the unix style line endings so they work here
                                tempStorageString = tempStorageString.Replace("\n", Environment.NewLine);
                            }
                        }
                    }
                }
            }
            catch { }

            return tempStorageString;
        }

        /// <summary>
        /// take csv data and push it into a List
        /// </summary>
        /// <param name="csvData">data from Google or Yahoo in CSV format</param>
        /// <returns></returns>
        public static List<StockItem> GetAllStockDetails(string csvData)
        {
            List<StockItem> parsedStockData = new List<StockItem>();

            using (StringReader reader = new StringReader(csvData))
            {
                string csvLine;

                //drop the first row because it is a header we don't need
                //reader.ReadLine();

                while ((csvLine = reader.ReadLine()) != null)
                {
                    string[] splitLine = csvLine.Split(',');

                    if (splitLine.Length >= 6)
                    {
                        StockItem newItem = new StockItem();

                        newItem.Symbol = splitLine[0].Trim(new char[] { '"' });
                        newItem.CompanyName = splitLine[1].Trim(new char[] { '"' });
                        newItem.CurrentPrice = splitLine[2].Trim(new char[] { '"' });
                        newItem.PercentageOfChangeFromLastDay = splitLine[3].Trim(new char[] { '"' });
                        newItem.FiftyTwoWeekHigh = splitLine[4].Trim(new char[] { '"' });
                        newItem.FiftyTwoWeekLow = splitLine[5].Trim(new char[] { '"' });

                        parsedStockData.Add(newItem);

                        ////parse all values from the downloaded csv file
                        //double tempOpen;
                        //if (Double.TryParse(splitLine[1], out tempOpen))
                        //{
                        //    newItem.Symbol = tempOpen;
                        //}
                        //double tempHigh;
                        //if (Double.TryParse(splitLine[2], out tempHigh))
                        //{
                        //    newItem.high = tempHigh;
                        //}
                        //double tempLow;
                        //if (Double.TryParse(splitLine[3], out tempLow))
                        //{
                        //    newItem.low = tempLow;
                        //}
                        //double tempClose;
                        //if (Double.TryParse(splitLine[4], out tempClose))
                        //{
                        //    newItem.close = tempClose;
                        //}
                        //double tempVolume;
                        //if (Double.TryParse(splitLine[5], out tempVolume))
                        //{
                        //    newItem.volume = tempVolume;
                        //}

                        ////if we parse the date successfully, we add the
                        ////new StockDataItem to our result set
                        //DateTime tempDate;
                        //if (DateTime.TryParse(splitLine[0], out tempDate))
                        //{
                        //    parsedStockData.Add(tempDate, newItem);
                        //}
                    }
                }
            }

            return parsedStockData;
        }
        /// <summary>
        /// deserialize to object type from JSON string
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="jsonString">JSON string to be deserialized</param>
        /// <returns>returns object</returns>
        public static T DeserializeFromJson<T>(string jsonString)
        {
            var js = new JavaScriptSerializer();
            T results = default(T);
            if (string.IsNullOrEmpty(jsonString))
            {
                return results;
            }

            results = js.Deserialize<T>(jsonString);
            return results;
        }

    }
}
