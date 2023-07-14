using NodaTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL
{
    public enum PriceInterval
    {
        Daily = 1,
        Intraday1min = 2,
        Intraday5min = 3,
        Intraday15min = 4,
        Intraday30min = 5,
        Intraday60min = 6
    }

    public enum ItemType
    {
        Stock = 1,
        Commodity = 2,
        ETF = 3
    }

    public enum ItemSubType
    {
        Unknown = 0,
        ConsumerStaples = 1,
        ConsumerDiscretionary = 2,
        Technology = 3,
        Materials = 4,
        Communication = 5,
        Energy = 6,
        Financial = 7,
        HealthCare = 8,
        Industrials = 9,
        Utilities = 10
    }
    public class PricesByIntervalData
    {
        public List<PriceTimeZoned> PricesTimeZoned = null;
        public double yMax;
        public double yMin;
    }
    public class Price
    {
        public int StartDateTimeUnix;
        public int EndDateTimeUnix;
        public double Open;
        public double Close;
        public double High;
        public double Low;
        public long Volume;
    }
    public class PriceTimeZoned : Price
    {
        public bool HasNoData = false;
        public DateTime StartDateTime;
        public DateTime EndDateTime;
        public string TimeZone;
    }

    public class Item
    {
        public string Symbol;
        public ItemType Type;
        public ItemSubType SubType;
        public Item()
        {

        }
    }

    public class DataLayer
    {
        public static string cs = "Data Source=.\\dbs\\stock\\{0};Version=3;Synchronous=NORMAL;Journal Mode=OFF;Page Size=16384;Temp Store=Memory";
        public static Dictionary<string, SQLiteConnection> cons = new Dictionary<string, SQLiteConnection>();
        public static string ExecutablePath = "";
        

        public static void Init(string executablePath)
        {
            ExecutablePath = executablePath;

            DirectoryInfo di = new DirectoryInfo(ExecutablePath + "dbs\\stock\\");

            FileInfo[] files = di.GetFiles();

            foreach (FileInfo file in files)
            {
                SQLiteConnection con = new SQLiteConnection(String.Format(cs, file.Name));
                con.Open();
                cons.Add(file.Name, con);
            }
        }

        public static List<string> GetAllSymbols(bool includeDisabled = false)
        {
            List<string> result = new List<string>();

            List<Item> items = GetAllItems(includeDisabled);

            for (int n = 0; n < items.Count; n++)
            {
                result.Add(items[n].Symbol);
            }

            return result;
        }

        public static List<Item> GetAllItems(bool includeDisabled = false)
        {
            List<Item> result = new List<Item>();

            string sql = "";

            if (includeDisabled)
            {
                sql = "SELECT * FROM Items ORDER BY Symbol ASC";
            }
            else
            {
                sql = "SELECT * FROM Items WHERE Enabled = 1 ORDER BY Symbol ASC";
            }
 
            using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["main.db"]))
            {

                SQLiteDataReader r = cmd1.ExecuteReader();
                while (r.Read())
                {
                    Item item = new Item();
                    item.Symbol = r["Symbol"].ToString();
                    item.Type = (ItemType)int.Parse(r["Type"].ToString());
                    item.SubType = (ItemSubType)int.Parse(r["SubType"].ToString());
                    result.Add(item);
                }
            }

            return result;
        }

        public static bool PriceDBExists(string symbol)
        {
            bool result = false;

            symbol = symbol.ToLower().Trim();

            result = File.Exists(ExecutablePath + "dbs\\stock\\p_" + symbol + ".db");

            return result;
        }

        public static bool ItemFullyExists(string symbol)
        {
            return PriceDBExists(symbol) && ItemExists(symbol);
        }

        public static bool ItemExists(string symbol)
        {
            bool result = false;

            symbol = symbol.ToLower().Trim();

            string sql = "SELECT * FROM Items WHERE Symbol = @Symbol";

            using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["main.db"]))
            {
                cmd1.Parameters.Add("Symbol", DbType.String).Value = symbol;
                object o = cmd1.ExecuteScalar();
                result = (o != null);
            }

            return result;
        }

        public static void CreatePriceDB(string symbol)
        {
            SQLiteConnection.CreateFile(ExecutablePath + "dbs\\stock\\p_" + symbol + ".db");

            SQLiteConnection con = new SQLiteConnection(String.Format(cs, "p_" + symbol + ".db"));
            con.Open();
            cons.Add("p_" + symbol + ".db", con);

            using (SQLiteTransaction t = cons["p_" + symbol + ".db"].BeginTransaction())
            {
                string sql = "CREATE TABLE \"Prices\" (\"Start\" INTEGER NOT NULL, \"End\" INTEGER NOT NULL, \"Open\" REAL NOT NULL, \"Close\" REAL NOT NULL, \"High\" REAL NOT NULL, \"Low\" REAL NOT NULL, \"Volume\" INTEGER NOT NULL, PRIMARY KEY(\"Start\",\"End\"));";

                using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["p_" + symbol + ".db"]))
                {
                    cmd1.ExecuteNonQuery();
                }
                t.Commit();
            }

        }

        // daily interval won't work
        public static PricesByIntervalData GetPricesByInterval(string symbol, PriceInterval interval, string timeZone, int amount)
        {
            PricesByIntervalData result = new PricesByIntervalData();

            List<PriceTimeZoned> resultList = new List<PriceTimeZoned>();

            List<Price> allPrices = GetAllPricesBySymbol(symbol, UC.PriceIntervalToSeconds(interval));

            List<Price> prices = null;

            int sliceIndex = (allPrices.Count - 1) - amount;

            if(sliceIndex >= 0)
            {
                prices = allPrices.GetRange(sliceIndex, amount);
            }
            else
            {
                prices = allPrices;
            }

            //allPrices.GetRange()

            DateTimeZone zone = DateTimeZoneProviders.Tzdb[timeZone];

            if (prices.Count > 0)
            {

                int secondsDiff = UC.PriceIntervalToSeconds(interval);

                double min = int.MaxValue;
                double max = int.MinValue;

                for (int n = prices.Count - 1; n >= 0; n--)
                {

                    Price price = prices[n];

                    if(price.Low < min)
                    {
                        min = price.Low;
                    }
                    if (price.High > max)
                    {
                        max = price.High;
                    }

                    PriceTimeZoned priceTimeZoned = new PriceTimeZoned();
                    priceTimeZoned.Close = price.Close;
                    priceTimeZoned.EndDateTimeUnix = price.EndDateTimeUnix;
                    priceTimeZoned.High = price.High;
                    priceTimeZoned.Low = price.Low;
                    priceTimeZoned.Open = price.Open;
                    priceTimeZoned.StartDateTimeUnix = price.StartDateTimeUnix;
                    priceTimeZoned.Volume = price.Volume;
                    priceTimeZoned.EndDateTime = UCDT.UnixTimeStampToZonedDateTime(price.EndDateTimeUnix, zone).ToDateTimeUnspecified();
                    priceTimeZoned.StartDateTime = UCDT.UnixTimeStampToZonedDateTime(price.StartDateTimeUnix, zone).ToDateTimeUnspecified();
                    priceTimeZoned.TimeZone = zone.Id;
                    resultList.Add(priceTimeZoned);
                    if (resultList.Count > amount)
                    {
                        break;
                    }
                    if(n - 1 >= 0)
                    {
                        Price prevPrice = prices[n - 1];
                        int secondsGap = price.StartDateTimeUnix - prevPrice.EndDateTimeUnix;

                        if(secondsGap > 0)
                        {
                            ZonedDateTime targetzdt = UCDT.UnixTimeStampToZonedDateTime(prevPrice.EndDateTimeUnix, zone);

                            ZonedDateTime zdt = UCDT.UnixTimeStampToZonedDateTime(price.StartDateTimeUnix, zone);
                            // start going back in time by the interval until we hit prevPriceEndDate
                            while (zdt != targetzdt)
                            {
                                PriceTimeZoned ptz = new PriceTimeZoned();
                                ptz.HasNoData = true;
                                ptz.EndDateTime = zdt.ToDateTimeUnspecified();
                                zdt = zdt.PlusSeconds(-secondsDiff);
                                ptz.StartDateTime = zdt.ToDateTimeUnspecified();
                                resultList.Add(ptz);
                                if (resultList.Count == amount)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (resultList.Count == amount)
                    {
                        break;
                    }

                }

               

                //resultList.Reverse();
                result.yMin = min;
                result.yMax = max;
                result.PricesTimeZoned = resultList;
            }

           

            return result;
        }

        public static List<Price> GetAllPricesBySymbol(string symbol, int periodSpanSeconds = -1)
        {
            List<Price> result = new List<Price>();

            string sqlPart = " ";
            if(periodSpanSeconds > 0)
            {
                sqlPart = " WHERE End - Start == " + periodSpanSeconds + " ";
            }

            string sql = "SELECT * FROM Prices" + sqlPart + "ORDER BY Start ASC";
            using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["p_" + symbol + ".db"]))
            {
                SQLiteDataReader r = cmd1.ExecuteReader();
                while (r.Read())
                {
                    Price p = new Price();
                    p.StartDateTimeUnix = int.Parse(r["Start"].ToString()); ;
                    p.EndDateTimeUnix = int.Parse(r["End"].ToString());
                    p.Open = double.Parse(r["Open"].ToString());
                    p.Close = double.Parse(r["Close"].ToString());
                    p.High = double.Parse(r["High"].ToString());
                    p.Low = double.Parse(r["Low"].ToString());
                    p.Volume = long.Parse(r["Volume"].ToString());
                    result.Add(p);
                }
            }

            return result;
        }
        public static void ItemSetEnabled(string symbol, bool enabled)
        {
            symbol = symbol.ToLower().Trim();

            using (SQLiteTransaction t = cons["main.db"].BeginTransaction())
            {
                string sql = "UPDATE Items SET Enabled = " + (enabled ? "1" : "0") + " WHERE Symbol = @Symbol";

                using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["main.db"]))
                {
                    cmd1.Parameters.Add("Symbol", DbType.String).Value = symbol;
                    cmd1.ExecuteNonQuery();
                }

                t.Commit();
            }
        }
        public static void InsertItem(string symbol, ItemType type, ItemSubType subType)
        {
            symbol = symbol.ToLower().Trim();

            using (SQLiteTransaction t = cons["main.db"].BeginTransaction())
            {
                string sql = "INSERT INTO Items (Symbol, Type, SubType) VALUES (@Symbol, @Type, @SubType)";

                using (SQLiteCommand cmd1 = new SQLiteCommand(sql, cons["main.db"]))
                {
                    cmd1.Parameters.Add("Symbol", DbType.String).Value = symbol;
                    cmd1.Parameters.Add("Type", DbType.Int32).Value = (int)type;
                    cmd1.Parameters.Add("SubType", DbType.Int32).Value = (int)subType;
                    cmd1.ExecuteNonQuery();
                }

                t.Commit();
            }
        }

        public static int BulkInsertPrice(List<Price> prices, string symbol, string conflictAction = "IGNORE")
        {
            int result = 0;

            string sql = "INSERT OR " + conflictAction + " INTO Prices (Start, End, Open, Close, High, Low, Volume) VALUES (@Start, @End, @Open, @Close, @High, @Low, @Volume)";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, cons["p_" + symbol + ".db"]))
            {
                cmd.Parameters.Add("Start", DbType.Int32).Value = 0;
                cmd.Parameters.Add("End", DbType.Int32).Value = 0;
                cmd.Parameters.Add("Open", DbType.Decimal).Value = 0;
                cmd.Parameters.Add("Close", DbType.Decimal).Value = 0;
                cmd.Parameters.Add("High", DbType.Decimal).Value = 0;
                cmd.Parameters.Add("Low", DbType.Decimal).Value = 0;
                cmd.Parameters.Add("Volume", DbType.Int64).Value = 0;

                for (int n = 0; n < prices.Count; n++)
                {
                    cmd.Parameters["Start"].Value = prices[n].StartDateTimeUnix;
                    cmd.Parameters["End"].Value = prices[n].EndDateTimeUnix;
                    cmd.Parameters["Open"].Value = prices[n].Open;
                    cmd.Parameters["Close"].Value = prices[n].Close;
                    cmd.Parameters["High"].Value = prices[n].High;
                    cmd.Parameters["Low"].Value = prices[n].Low;
                    cmd.Parameters["Volume"].Value = prices[n].Volume;

                    result += cmd.ExecuteNonQuery();
                }
            }

            return result;
        }
    }
}
