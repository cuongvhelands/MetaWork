using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class LogCongViecViewModel
    {
        public int LogCongViecId { get; set; }
        public int CongViecId { get; set; }
        public string TenLogCongViec { get; set; }
        public string NoiDungLog { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid NguoiTao { get; set; }
    }
}
