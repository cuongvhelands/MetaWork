using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace MetaWork.WorkTime.Models
{
    public class MetaWorkModel
    {
       
        public string GetContentOfPage(string url)
        {
            WebBrowser web = new WebBrowser();
            web.Navigate(new Uri(url));
            return web.DocumentText;
        }
    }
}