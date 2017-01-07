using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StockViewApplication
{
    /// <summary>
    /// Database class that holds data-table: data_table (used to hold individualt stocks data)
    /// </summary>
    class Database
    {
        private DataTable data_table;

        public DataTable Data_table
        {
            get { return data_table; }
            set { data_table = value; }
        }
        
        /// <summary>
        /// Timer that ticks every 5 seconds to pull the XML file
        /// </summary>
        private Timer updateTimer;

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="tableName">name of the table that holds individual stock data</param>
        /// <param name="colNames">Column names in both tables</param>
        public Database(string tableName, string[] colNames)
        {
            data_table = new DataTable(tableName);

            foreach (string s in colNames)
            {
                data_table.Columns.Add(s);
            }

            updateTimer = new Timer();
            updateTimer.Interval = 500000000; //Change the value here to increase/decrease the update time
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Enabled = true;
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            //Fetching all the stocks at once in XDocument file
            //XDocument doc = Stock_quotes.FetchQuote(this.getAllSymbolsFromTable(data_table) + ConfigurationManager.AppSettings["DefinedStocks"]);
            //This will update the data_table            
            this.addValuesToTheTable(data_table, StockItem.FetchAllStocks(""));
        }

        /// <summary>
        /// Adds a stock symbol to the table or throws an ArgumentException
        /// </summary>
        /// <param name="symbol">symbol(s) to the added. Multiple entries are allowed that are separated by " " or ","</param>
        /// <param name="table"></param>
        public void addStockSymbolToTheTable(string symbol, DataTable table)
        {
            //if (symbol != null && symbol.Length > 0)
            //{
                List<StockItem> list = StockItem.FetchAllStocks(symbol);
                foreach (StockItem stock in list)
                {
                    table.Rows.Add(stock.Symbol,stock.CompanyName, stock.CurrentPrice ,stock.PercentageOfChangeFromLastDay, stock.FiftyTwoWeekHigh,stock.FiftyTwoWeekLow);
                }
            //}
            //else
            //{
            //    throw new ArgumentException("Added symbol is not accepted as a valid input");
            //}
        }

        
        /// <summary>
        /// Gets all the symbols (in the symbol column) from the table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string getAllSymbolsFromTable(DataTable table)
        {
            StringBuilder result = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                result.Append(row["Symbol"] + " ");
            }
            return result.ToString();
        }

        /// <summary>
        /// Updates the table data
        /// </summary>
        /// <param name="table"></param>
        /// <param name="doc"></param>
        public void addValuesToTheTable(DataTable table, List<StockItem> stockItems)
        {
            foreach (DataRow row in table.Rows)
            {
                StockItem stock = stockItems.Single(x=>x.Symbol == (string)row["Symbol"]) as StockItem;
                row["Symbol"] = stock.Symbol;
                row["CompanyName"] = stock.CompanyName;
                row["Current Price"] = stock.CurrentPrice;
                row["% change from last day"] = stock.PercentageOfChangeFromLastDay;
                row["52 Week High"] = stock.FiftyTwoWeekHigh;
                row["52 Week Low"] = stock.FiftyTwoWeekLow;
                //row["Company"] = stock.Company;
                //row["Date"] = stock.Date;
                //row["Time"] = stock.Time;
                //row["Closed Yesterday"] = stock.Y_close;
                //row["Trade"] = stock.Trade;
                //row["Chg"] = stock.Chg;
                //row["%Chg"] = stock.Perc_chg;
                //row["Volume"] = stock.Volume;
                //row["High"] = stock.High;
                //row["Low"] = stock.Low;
                //row["Chart"] = stock.Chart_url;
                //row["Market Cap"] = stock.Market_cap;
                //row["Exchange"] = stock.Exchange;
                //row["Currency"] = stock.Currency;
            }
        }

        /// <summary>
        /// Saves the symbols that user has entered into the settings file 
        /// </summary>
        public void saveSymbols()
        {
            Properties.Settings.Default.symbols = new System.Collections.Specialized.StringCollection();
            foreach (DataRow row in data_table.Rows)
            {
                Properties.Settings.Default.symbols.Add((string)row["Symbol"]);
            }
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Loads symbols that user had entered previously from the settings file
        /// </summary>
        public void loadSavedSymbols()
        {
            var list = Properties.Settings.Default.symbols;
            
            if (list !=null && list.Count != 0)
            {
                StringBuilder symbols = new StringBuilder();
                foreach (string s in list)
                {
                    symbols.Append(s + " ");
                }
                try
                {
                    this.addStockSymbolToTheTable(symbols.ToString(), data_table);
                }
                catch (ArgumentException ar)
                {
                    MessageBox.Show(ar.Message);
                }
            }
            
        }




    }
}
