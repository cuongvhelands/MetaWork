using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class PhongBanViewModel
    {
        public int PhongBanId { set; get; }
        public int? KhoaChaId { set; get; }
        public string MaPhongBan { set; get; }
        public string TenPhongBan { set; get; }
        public decimal Point { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
    }
    public enum EnumPhongBanId
    {

        Sale = 6,
    }
}
