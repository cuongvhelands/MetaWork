using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MetaWork.Data.ViewModel
{
    public class CongViecViewModel
    {
        public int CongViecId { get; set; }
        public int DuAnId { get; set; }
        public int? DuAnChaId { get; set; }
        public string TenDuAn { get; set; }
        public string MaDuAn { get; set; }
        public int? GiaiDoanDuAnId { get; set; }
        public int? LoaiGiaiDoanId { get; set; }
        public byte? LoaiTimer { get; set; }
        public string TenGiaiDoan { get; set; }
        public string TenGiaiDoanActive { get; set; }
        public string MaMauLoaiGiaiDoan { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenKhoaCha { get; set; }
        public bool? LaShipAble { get; set; }
        public bool? LaTask { get; set; }
        public string TenCongViec { get; set; }
        public string TenCongViecMute { get; set; }
        public string MaCongViec { get; set; }
        public Guid NguoiTaoId { get; set; }
        public Guid? NguoiXuLyId { get; set; }
        public string HoTenNguoiXuLy { get; set; }
        public string TenDangNhapNguoiXuLy { get; set; }
        public string Avatar { get; set; }
        public string AvatarNguoiXuLy { get; set; }
        public string HoTen { get; set; }
        public Guid? NguoiHoTroId { get; set; }
        public string HoTenNguoiHoTro { get; set; }
        public string AvatarNguoiHoTro { get; set; }
        public string TenDangNhapNguoiHoTro { get; set; }
        public DateTime? NgayTao { get; set; }
        public string StrNgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public byte? ThuTuUuTien { get; set; }
        public string TenThuTuUuTien { get; set; }
        public string MauChuThuTuUuTien { get; set; }
        public string MaMauThuTuUuTien { get; set; }
        public byte? DoPhucTap { get; set; }
        public string TenDoPhucTap { get; set; }
        public string MaMauDoPhucTap { get; set; }
        public DateTime? NgayDuKienHoanThanh { get; set; }
        public string StrNgayDuKienHoanThanh { get; set; }
        public string StrNgayDuKienHoanThanhFull { get; set; }
        public decimal? ThoiGianUocLuong { get; set; }
        public int TrangThaiCongViecId { get; set; }
        public string TenTrangThai { get; set; }
        public string MaMauTrangThaiCongViec { get; set; }
        public bool? XacNhanHoanThanh { get; set; }
        public decimal? DiemPoint { get; set; }
        public byte? LoaiPoint { get; set; }
        public byte? NhanCongViec { get; set; }
        [AllowHtml]
        public string MoTa { get; set; }
        public int Tuan { get; set; }
        public int Nam { get; set; }
        public int NamHoanThanh { get; set; }
        public string MaMau { get; set; }
        public List<CongViecViewModel> CongViecs { get; set; }
        public List<LoaiCongViecViewModel> LoaiCongViecs { get; set; }
        public TrangThaiCongViecViewModel TrangThaiCongViec { get; set; }
        public List<TrangThaiCongViecViewModel> TrangThaiCongViecs { get; set; }
        public List<TimeViewModel> Times { get; set; }
        public int TimeValue { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public List<int> LoaiCongViecIds { get; set; }
        public bool? XuLyVaoTuanTiepTheo { get; set; }
        public int? TuanHoanThanh { get; set; }
        public string StrThoiGianLamViec { get; set; }
        public int CongViecIsStartId { get; set; }

        public int? TongThoiGianLog { get; set; }

        public DateTime NgayLamViec { get; set; }
        public string StrNgayLamViec { get; set; }
        public bool? PheDuyet { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }

        public int? TongThoiGianLogChuaKetThuc { get; set; }

        public string TenLoaiCongViec { get; set; }
        public List<ToDoInDayViewModel> ToDoInDays { get; set; }
        public int TongThoiGian { get; set; }
        public decimal PercentTime { get; set; }
        public string StrTotalTime { get; set; } 
        public string StrTimeDone { get; set; }
        public byte LoaiThoiGian { get; set; }

        public bool Visible { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string StrThoiGianBatDau { get; set; }
        public string StrThoiGianKetThuc { get; set; }
        public int ThoiGianLamViecId { get; set; }
        public byte DayType { get; set; }
        public bool IsStart { get; set; }

        public int TuanBatDau { get; set; }
        public int TuanDuKienHoanThanh { get; set; }
        public int SoNgayDuKien { get; set; }
        public int TuanDaChay { get; set; }
        public int SoNgayDaChay { get; set; }
        public int SoTuanDuKien { get; set; }
        public int CountCongViecCon { get; set; }
        public int CountCongViecConDoing { get; set; }
        public List<NoiDungViewModel> Comments { get; set; }
        public List<string> MarginLeftComments { get; set; }
        public List<string> ListTaskName { get; set; }
        public bool IsToDoAdd { get; set; }
        public List<ThoiGianLamViecViewModel> ThoiGianLamViecs { get; set; }
        public string TenShipable { get; set; }
        public bool QuanTam { get; set; }
        public bool IsAddNew { get; set; }
        public bool IsUpdate { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public int? ThuTuSapXep { get; set; }
    }
    public class ShipAbleViewModel
    {
        public int Tuan { get; set; }
        public int TuanTruoc { get; set; }
        public int ShipAbleHoanThanhTuanHienTai { get; set; }
        public int ShipAbleHoanThanhTuanTruoc { get; set; }
        public int ShipAbleHoanThanhTuanTruocNua { get; set; }
        public int ShipAbleHoanThanhTuanTruocNuaNua { get; set; }

        public List<CongViecViewModel> ShipableTuanHienTai { get; set; }
        public List<CongViecViewModel> ToDoTuanHienTai { get; set; }

        public List<CongViecViewModel> ShipableTuanTruoc { get; set; }
        public List<CongViecViewModel> ToDoTuanTruoc { get; set; }
        public List<CongViecViewModel> ShipableTuanTruocNua { get; set; }
        public List<CongViecViewModel> ToDoTuanTruocNua { get; set; }
        public List<CongViecViewModel> ShipableTuanTruocNuaNua { get; set; }
        public List<CongViecViewModel> ToDoTuanTruocNuaNua { get; set; }
        public string StrWeek { get; set; }
        public List<DayInWeekViewModel> DayInWeeks { get; set; }
        public string StrWeek2 { get; set; }
        public List<DayInWeekViewModel> DayInWeeks2 { get; set; }
        public List<ShipAble2IndexViewModel> ShipAble2Indexs { get; set; }
        public int DuAnId { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        public List<TrangThaiCongViecViewModel> TrangThaiCongViecs { get; set; }
        public int TrangThaiCongViecId { get; set; }
        public int Nam { get; set; }
        public bool TuanTiepTheo { get; set; }
    }
    public class ReportShipAbleViewModel
    {
        public string StrStartTime { get; set; }
        public string StrEndTime { get; set; }
        public List<int> TrangThaiIds { get; set; }
        public List<Guid> NguoiDungIds { get; set; }
        public int DuAnId { get; set; }
        public List<ShipAbleInWeekViewModel> ShipableInWeeks { get; set; }
    }
    public class ShipAbleInWeekViewModel
    {
        public int Tuan { get; set; }
        public int Nam { get; set; }
        public string DisplayTuan { get; set; }
        public List<CongViecViewModel> Shipables { get; set; }
    }
    public class ShipAble2IndexViewModel
    {
        public List<CongViecViewModel> Shipables { get; set; }
        public int Tuan { get; set; }
        public int Nam { get; set; }
        public string strTuan { get; set; }
    }
    //public class ShipAbleViewModel
    //{
    //    public int Tuan { get; set; }
    //    public int ShipAbleHoanThanhTuan { get; set; }    
    //    public int rowSpane { get; set; }
    //    public List<CongViecViewModel> ShipableTuan { get; set; }
    //    public List<CongViecViewModel> ToDoTuan { get; set; }
    public class ToDoOfUserInWeekViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Avatar { get; set; }
        public List<CongViecViewModel> CongViecs { get; set; }
        public int week { get; set; }
    }
    public class ToDoInWeekViewModel
    {
        public int? PhongBanId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public int Tuan { get; set; }
        public string StrLoaiCongViec { get; set; }
        public List<ToDoOfUserInWeekViewModel> ToDoOfUserInWeek { get; set; }
        public string StrWeek { get; set; }
        public List<DayInWeekViewModel> DayInWeeks { get; set; }
    }
    public class DayInWeekViewModel
    {
        public int DayOfWeek { get; set; }
        public string TenDayInWeek { get; set; }
        public string NgayThang { get; set; }
    }
    public class XacNhanHoanThanhViewModel
    {
        public int XacNhanHoanThanh { get; set; }
        public string TenXacNhan { get; set; }
    }
    public class TimeViewModel
    {
        public int Value { get; set; }
        public string Summary { get; set; }
    }
    public class ToDoInDayViewModel
    {
        public DateTime Day { get; set; }
        public List<CongViecViewModel> CongViecs { get; set; }
    }
    public class TenCongViecViewModel
    {
        public string TenCongViec { get; set; }
        public string TenTrangThai { get; set; }
        public List<TenCongViecViewModel> TenCongViecs { get; set; }
    }
}
