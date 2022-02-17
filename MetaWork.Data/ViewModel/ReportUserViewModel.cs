using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ReportUserViewModel
    {
        public List<string> Emails { get; set; }
        public int TimeFrom { get; set; } 
        public int TimeTo { get; set; }
    }
}
