using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ReportPointViewModel
    {
        public List<string> HoTenNguoiDungs { get; set; }
        public List<decimal> Value { get; set; }
        public decimal Max { get; set; }
        public string Title { get; set; }
    }
}
