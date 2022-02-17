using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class DanhSachChoDuyetViewModel
    {
        public int ThoiGianLamViecId { get; set; }
        public DateTime NgayDangKy { get; set; }
        public int NgayDangKyInSeconds { get; set; }
        public byte DayTypeId { get; set; }
        public string DayType { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public int ThoiGianBatDauInSeconds { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public int ThoiGianKetThucInSeconds { get; set; }
        /// <summary>
        /// 3h:36m
        /// </summary>
        public string TongThoiGian { get; set; }       
        public string TenDuAn { get; set; }
        public string TenShipAble { get; set; }
        public string TenToDo { get; set; }
        public string TenTask { get; set; }
        public string LyDo { get; set; }
        public string LinkHuyDuyet { get; set; }
        public string LinkPheDuyet { get; set; }
        public string TokenId { get; set; }
    }
}
