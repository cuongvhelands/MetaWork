using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
namespace MetaWork.Data.ViewModel
{
    public class DuAnViewModel
    {
        public int DuAnId { get; set; }
        public int? LoaiDuAnId { get; set; }
        public string TenLoaiDuAn { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenKhoaCha { get; set; }
        public string TenDuAn { get; set; }
        public string TenVietTat { get; set; }
        public string MaDuAn { get; set; }
        [AllowHtml]
        public string MoTa { get; set; }
        public string GhiChu { get; set; }
        public string TagLine { get; set; }
        public string ThongTinChiTiet { get; set; }
        public Guid NguoiQuanLyId { get; set; }
        public string HoTenNguoiQuanLy { get; set; }
        public string AvatarNguoiQuanLy { get; set; }
        public int? KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public int TrangThaiDuAnId { get; set; }
        public string MaMauTrangThaiDuAn { get; set; }
        public string TenTrangThaiDuAn { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int LoaiNganSachId { get; set; }
        public string TenLoaiNganSach { get; set; }
        public string TenLoaiNganSachVietTat { get; set; }
        public bool LuuTru { get; set; }    
        public bool QuanTam { get; set; }
        public int? TongNganSach { get; set; }
        public bool? ChoPhepVuotQua { get; set; }
        public int? MucCanhBao { get; set; }

        public DateTime? NgayBatDau { get; set; }
        public string StrNgayBatDau { get; set; }
        public string StrNgayBatDau2 { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string StrNgayKetThuc { get; set; }
        public List<GiaiDoanDuAnViewModel> GiaiDoanDuAns { get; set; }
        /// <summary>
        /// Mã màu giai đoạn active
        /// </summary>
        public string MaMau { get; set; }
        public string TenGiaiDoanActive { get; set; }
        public List<LoaiCongViecViewModel> LoaiCongViecs { get; set; }
        public List<LienKetNguoiDungDuAnViewModel> LienKetNguoiDungDuAn { get; set; }
        public List<NguoiDungViewModel> NguoiQuanLys { get; set; }
        public List<int> LoaiCongViecIds { get; set; }
        
        public string strLoaiCongViecIds { get; set; }
        public int Spent { get; set; }
        public string StrTimeSpent { get; set; }
        public int TotalShipable { get; set; }
        public int TotalShipableDone { get; set; }

        public List<TuanViewModel> Tuans { get; set; }
        public int GiaiDoanDuAnId { get; set; }
        public int TotalWeek { get; set; }
        public List<ThoiGianSuDungViewModel> ThoiGianSuDungs { get; set; }
        public List<CongViecViewModel> CongViecs { get; set; }
        public string BackGroundColor { get; set; }
        public int? CostH { get; set; }
        public string StrCostH { get; set; }
        public int? CostTien { get; set; }
        public string StrCostTien { get; set; }
        public string TenHoatDong { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        public string LinkCoda { get; set; }
        public string StrPhongBanIds { get; set; }
        public int? Cost { get; set; }
        public string StrCost { get; set; }
        public string StrTongNganSach { get; set; }
        public int Rowspan { get; set; }
    }
    public class TuanViewModel
    {
        public int week { get; set; }
        public int year { get; set; }
    }
    public class DuAnIndexViewModel
    {       
        public List<DuAnViewModel> DuAns { get; set; }     
        public int Type { get; set; }
        public int CountFavorite { get; set; }
        public int CountAll { get; set; }
        public int CountArchive { get; set; }
        public string StrNguoiDung { get; set; }
        public string StrStatus { get; set; }
        public int TypeGroup { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public List<LoaiDuAnViewModel> LoaiDuAns { get; set; }
    }

    public class ReportProjectViewModel
    {
        public int DuAnId { get; set; }
        public Guid NguoiDungId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }    
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public string JsonData { get; set; }
    }
    public class ReportProjectDetailsViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public int DuAnId { get; set; }
        public string TenDuAn { get; set; }
        public DateTime StartDate { get; set; }     
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public string KhoangThoiGian { get; set; }
        public List<double> HourSpents { get; set; }
        public List<string> LoaiCongViecs { get; set; }
        public List<string> TenDuAns { get; set; }
        public List<PieTimeViewModel> PieTime { get; set; }
        public string TotalHour { get; set; }

    }

    public class PieTimeViewModel
    {
        public string category { get; set; }
        public double value { get; set; }
    }
    public enum EnumLoaiNganSachType
    {
        MacDinh=3     

    }
    public enum EnumTrangThaiDuAnType
    {
        MacDinh = 1,
        Active =2,
        Pending=3

    }

    public class LienKetDuAnPhongBanViewModel
    {
        public int DuAnId { get; set; }
        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
    }
    public class LoaiDuAnViewModel
    {
        public int LoaiDuAnId { get; set; }
        public string TenLoaiDuAn { get; set; }
    }
    public enum EnumLoaiDuAnType
    {
        project = 1,
        process = 2,
        outline = 1003

    }
}
