using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class QuanLyCongViecViewModel
    {
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public int PhongBanId { get; set; }
        public string StrNguoiDungId { get; set; }
        public byte Type { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public List<PhongBanViewModel> PhongBans { get; set; }
        public List<NguoiDungViewModel> NguoiDungAll { get; set; }
        public List<NgayLamCongViecViewModel> NgayLamCongViecs { get; set; }
        public List<TimeTypeViewModel> TimeTypes { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        public string TextHeader { get; set; }
        public Guid UserId { get; set; }
        public byte Quyen { get; set; }
        public List<TrangThaiCongViecViewModel>  TrangThaiCongViecs { get; set; }
    }
    public class NgayLamCongViecViewModel
    {
        public DateTime NgayLamViec { get; set; }
        public DateTime End { get; set; }
        public string DisPlayNgayLamViec { get; set; }
        public IEnumerable<CongViecViewModel> ToDos { get; set; }
    }
    public class TimeTypeViewModel
    {
        public byte Type { get; set; }
        public string Name { get; set; }
    }
}
