using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
   public class ThoiGianLamViecTrongNgayViewModel
    {
       public DateTime NgayLamViec { get; set; }
        public string StrNgayLamViec { get; set; }
        public List<TenCongViecViewModel> TenCongViecs { get; set; }
        public Guid NguoiDungId { get; set; }
      
        public string HoTen { get; set; }
        public List<ThoiGianLamViecViewModel> ThoiGianLamViecs { get; set; }
    }
}
