using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Xml;

namespace ConsoleApplicationRatp
{
    class Program
    {
        static void Main(string[] args)
        {

            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                //client.Headers["User-Agent"] =
                //"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                //"(compatible; MSIE 6.0; Windows NT 5.1; " +
                //".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                string vrandom = ((new Random(DateTime.Now.Millisecond).Next()) * 10000.0d).ToString();
                string vamount = "15";
                string varticle = "2";
                string url = "https://mailing.canalce.com/r/" + vrandom + "?tagid=Commandes&amount=" + vamount + "&article=" + varticle;
                client.DownloadData(url);
            }

        }
    }
}
