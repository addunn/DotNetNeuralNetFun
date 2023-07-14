using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Text;

namespace CL
{
    

    public class StockPrice
    {
        public string Symbol = "";
        public int StartDateTimeUnix = 0;
        public int EndDateTimeUnix = 0;
        public double Open = 0;
        public double Close = 0;
        public double High = 0;
        public double Low = 0;
        public long Volume = 0;

        public StockPrice()
        {

        }
    }
    public class StockAPI
    {
        private static readonly DateTimeZone EasternTimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];
        //private static readonly TimeZoneInfo EST = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        //private static readonly DateTimeZone UTCTimeZone = DateTimeZoneProviders.Tzdb["Greenwich Mean Time"];

        // premium key
        public static string APIKey = "P1PNCUHW9BBH10FE";

        // public static string APIKey = "AZN3PLB2MNLM65KB";
        /*
            https://www.alphavantage.co
	            api key: AZN3PLB2MNLM65KB
        */
        public static List<StockPrice> GetPrices(string symbol, PriceInterval method)
        {

            List<StockPrice> result = null;

            if(method == PriceInterval.Daily)
            {
                result = GetPricesDaily(symbol);
            }
            else if(method == PriceInterval.Intraday1min)
            {
                result = GetPricesIntraday(symbol, 1);
            }
            else if (method == PriceInterval.Intraday5min)
            {
                result = GetPricesIntraday(symbol, 5);
            }
            else if (method == PriceInterval.Intraday15min)
            {
                result = GetPricesIntraday(symbol, 15);
            }
            else if (method == PriceInterval.Intraday30min)
            {
                result = GetPricesIntraday(symbol, 30);
            }
            else if (method == PriceInterval.Intraday60min)
            {
                result = GetPricesIntraday(symbol, 60);
            }

            return result;
        }

        // Method name changed to follow .NET conventions.


        // 1min, 5min, 15min, 30min, 60min
        public static List<StockPrice> GetPricesIntraday(string symbol, int span)
        {
            List<StockPrice> result = new List<StockPrice>();



            string url = String.Format("https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={0}&interval={1}min&apikey={2}&outputsize={3}&datatype=csv", symbol, span, APIKey, "full");

            string csvPayload = UC.DownloadStringAsync(url);

            /*
                timestamp,open,high,low,close,volume
                2019-09-25,137.5000,139.9580,136.0400,139.3600,20835063
            */

            if(!(csvPayload.Contains("Invalid API call") && csvPayload.Contains("TIME_SERIES_INTRADAY")))
            { 

                List<string> lines = UC.MultilineStringToList(csvPayload);

                for (int n = 1; n < lines.Count; n++)
                {
                    string line = lines[n];
                    
                    string[] cols = line.Split(',');
                    string date = cols[0];
                    string open = cols[1];
                    string close = cols[4];
                    string high = cols[2];
                    string low = cols[3];
                    string volume = cols[5];

                    DateTime startDateTime = DateTime.Parse(date); 
                    DateTime endDateTime = DateTime.Parse(date);

                    endDateTime = endDateTime.AddMinutes(span);

                    ZonedDateTime startZoned = new LocalDateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second).InZoneLeniently(EasternTimeZone);
                    DateTime startUTC = startZoned.ToDateTimeUtc();

                    ZonedDateTime endZoned = new LocalDateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second).InZoneLeniently(EasternTimeZone);
                    DateTime endUTC = endZoned.ToDateTimeUtc();

                    StockPrice sp = new StockPrice();
                    sp.StartDateTimeUnix = UCDT.DateTimeToUnixTimeStamp(startUTC);
                    sp.EndDateTimeUnix = UCDT.DateTimeToUnixTimeStamp(endUTC);
                    sp.Open = double.Parse(open);
                    sp.Close = double.Parse(close);
                    sp.High = double.Parse(high);
                    sp.Low = double.Parse(low);
                    sp.Volume = long.Parse(volume);
                    sp.Symbol = symbol;

                    result.Add(sp);
                }
            }

            return result;
        }

        public static List<StockPrice> GetPricesDaily(string symbol)
        {
            List<StockPrice> result = new List<StockPrice>();

            string url = String.Format("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={0}&outputsize={1}&apikey={2}&datatype=csv", symbol, "full", APIKey);


            string csvPayload = UC.DownloadStringAsync(url);

            /*
                timestamp,open,high,low,close,volume
                2019-09-25,137.5000,139.9580,136.0400,139.3600,20835063
            */
            if (!(csvPayload.Contains("Invalid API call") && csvPayload.Contains("TIME_SERIES_INTRADAY")))
            {
                List<string> lines = UC.MultilineStringToList(csvPayload);

                for (int n = 1; n < lines.Count; n++)
                {
                    string line = lines[n];

                    string[] cols = line.Split(',');
                    string date = cols[0];
                    string open = cols[1];
                    string close = cols[4];
                    string high = cols[2];
                    string low = cols[3];
                    string volume = cols[5];

                    // pretend we are working with east coast time
                    DateTime startDateTime = DateTime.Parse(date); // should be something like "12-5-2017 00:00:00"
                    DateTime endDateTime = DateTime.Parse(date); // should be something like "12-5-2017 00:00:00"

                    // east coast nasdaq opens at 9:30am and closes at 4:00pm

                    // add 9.5 hours to startDateTime because the datetimes have 00:00
                    startDateTime = startDateTime.AddHours(9.5);
                    // add 16 hours to startDateTime
                    endDateTime = endDateTime.AddHours(16);

                    ZonedDateTime startZoned = new LocalDateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second).InZoneLeniently(EasternTimeZone);
                    DateTime startUTC = startZoned.ToDateTimeUtc();

                    ZonedDateTime endZoned = new LocalDateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second).InZoneLeniently(EasternTimeZone);
                    DateTime endUTC = endZoned.ToDateTimeUtc();

                    StockPrice sp = new StockPrice();
                    sp.StartDateTimeUnix = UCDT.DateTimeToUnixTimeStamp(startUTC);
                    sp.EndDateTimeUnix = UCDT.DateTimeToUnixTimeStamp(endUTC);
                    sp.Open = double.Parse(open);
                    sp.Close = double.Parse(close);
                    sp.High = double.Parse(high);
                    sp.Low = double.Parse(low);
                    sp.Volume = long.Parse(volume);
                    sp.Symbol = symbol;

                    result.Add(sp);
                }
            }
            return result;
        }

        
    }
}
