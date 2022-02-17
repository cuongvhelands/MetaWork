using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ResourceUserViewModel
    {
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public int PhongBanId { get; set; }
        public string StrNguoiDungId { get; set; }
        public int MaxTotalTime { get; set; }
        public string StrMaxTotalTime { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public List<PhongBanViewModel> PhongBans { get; set; }
        public List<NguoiDungViewModel> NguoiDungAll { get; set; }
        public int Tuan { get; set; }
        public bool TuanTiepTheo { get; set; }
    }
}
