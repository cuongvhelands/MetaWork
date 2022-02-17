using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
   public class NguoiDungActivityViewModel
    {
        public int LienKetNguoiDungActivityId { get; set; }
        public Guid NguoiDungId { get; set; }
        public string HoTenNguoiDung { get; set; }
        public int ActivityId { get; set; }
        public string TenActivity { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool Active { get; set; }
        public string Icon { get; set; }

    }
    public enum EnumNguoiDungActivityId
    {
        Active = 6,
    }
}
