using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;


namespace StockViewApplication
{
    public partial class StockViewForm : Form
    {
        private Database database;
        

        public StockViewForm()
        {
            InitializeComponent();

           
            

            database = new Database("Stock Table", new string[] { "Symbol","CompanyName", "Current Price", "% change from last day", "52 Week High", "52 Week Low" });
            database.addStockSymbolToTheTable("", database.Data_table);

            dataGridView1.DataSource = database.Data_table;

            database.loadSavedSymbols();

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value!=null && e.Value != DBNull.Value)
            {
                //+ or - sign
                if (((string)e.Value).StartsWith("+"))
                {
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (((string)e.Value).StartsWith("-"))
                {
                    e.CellStyle.ForeColor = Color.Red;
                }

                if (((string)e.Value).EndsWith("%") == true && ((string)e.Value).StartsWith("-") == false)
                {
                    e.CellStyle.ForeColor = Color.Green;
                }

            }
        }

        private void StockViewForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                systemTrayNotifyIcon.Icon = new Icon("StockView.ico");
                systemTrayNotifyIcon.Visible = true;
                systemTrayNotifyIcon.BalloonTipText = "StockViewApp";
                systemTrayNotifyIcon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                systemTrayNotifyIcon.Visible = false;
            }
        
        }

        private void systemTrayNotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void StockViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        //static async Task RunAsync()
        //{
        //    //var stockList = DeserializeFromJson<ProductDefinitionDataResponse>(ExecuteWebRequestToStream(new Uri("http://finance.google.com/finance/info?client=ig&q=INTC,CSCO,MSFT")));

        //    using (var client = new HttpClient())
        //    {

        //        //client.BaseAddress = new Uri("http://finance.google.com/finance/info?client=ig&q=INTC,CSCO,MSFT");
        //        //client.DefaultRequestHeaders.Accept.Clear();
        //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var response = await client.GetAsync("http://finance.google.com/finance/info?client=ig&q=INTC,CSCO,MSFT");
        //        var stockList = DeserializeFromJson<StockItem>(response.Content.ReadAsStringAsync().Result);
        //    }
        //}

        //private static Task DeserializeFromJson<T>(Task<string> task)
        //{
        //    DeserializeFromJson<StockItem>(task.ToString);
        //    //throw new NotImplementedException();
        //}




    }
}
