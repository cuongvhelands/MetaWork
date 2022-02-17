using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class LoaiCongViecViewModel
    {
        public int LoaiCongViecId { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenLoaiCongViec { get; set; }
        public string MaLoaiCongViec { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public List<LoaiCongViecViewModel> LoaiCongViecs { get; set; }
    }
}
