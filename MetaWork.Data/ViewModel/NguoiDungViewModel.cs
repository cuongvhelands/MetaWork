using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class NguoiDungViewModel
    {
        public Guid NguoiDungId { get; set; }
        public int LoaiNguoiDungId { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string KhungThoiGianBatDau { get; set; }
        public string KhungThoiGianKetThuc { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public byte? Quyen { get; set; }
        public string Avatar { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhatId { get; set; }
        public bool TrangThai { get; set; }
        public string GhiChu { get; set; }
        public int DiemPoint { get; set; }
        public decimal Point { get; set; }
        public List<NgayLamViecViewModel> NgayLamViecs { get; set; }
        public int Month { get; set; }
        public string Email { get; set; }
        public List<LichLamViecCaNhanViewModel> LichLamViecCaNhans { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        public List<DayType> DayTypes { get; set; }
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public int TotalTime { get; set; }
        public string StrTotalTime { get; set; }
        public int TotalTimeNeed { get; set; }
        public string StrToTalTimeNeed { get; set; }
        public double LuongThang { get; set; }
        public string UserNameSlack { get; set; }
    }
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
    public class ThoiGianSuDungViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public int ThoiGianSuDung { get; set; }
        public string StrThoiGianSuDung { get; set; }
    }
    public class FilterNguoiDungViewModel
    {
        public List<int> DuAnIds { get; set; }
        public List<int> PhongBanIds { get; set; }
    }
    public class ReportNguoiDungViewModel
    {
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public Guid nguoiDungId { get; set; }
        public string HoTen { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        
    }

    public class EnumNguoiDungIdViewModel
    {

        public Guid NguoiDungId { get; set; } = Guid.Parse("07c9bf6e-75e5-4349-97ab-ce87f662e4ac");
    }
    public class HoTenNguoiDungViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
    }
}
