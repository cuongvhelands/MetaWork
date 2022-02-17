using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
   public  class AddThoiGianLamViecWithXMLViewModel
    {
        public string TenDangNhap { get; set; }
        public int CongViecId { get; set; }
        public byte? LoaiThoiGian { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenCongViec { get; set; }
        public string TenKhoaCha { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public byte? LoaiTimer { get; set; }
        public int TrangThaiCongViecId { get; set; }
        public int Tuan { get; set; }
        public int Nam { get; set; }
        public bool IsToDoAdd { get; set; }
        public int TongThoiGian { get; set; }
        public string TokenId { get; set; }
        public DateTime NgayLamViec { get; set; }
        public bool? PheDuyet { get; set; }
        public int DuAnId { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayDuKienHoanThanh { get; set; }
    }
}
