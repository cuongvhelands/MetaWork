using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ThoiGianLamViecViewModel
    {
        public int ThoiGianLamViecId{ get; set; }
      public byte LoaiThoiGian{ get; set; }
      public Guid? NguoiDungId{ get; set; }
      public int? CongViecId{ get; set; }
        public int? KhoaChaId { get; set; }
        public string TenShipAble { get; set; }
        public string TenCongViec { get; set; }
        public int? DuAnId { get; set; }
        public string TenDuAn { get; set; }
        public int? DuAnChaId { get; set; }
        public string TenDuAnCha { get; set; }
        public byte? LoaiTimer { get; set; }

      public DateTime? ThoiGianBatDau{ get; set; }
        public string StrThoiGianBatDau { get; set; }
      public DateTime? ThoiGianKetThuc { get; set; }
        public string StrThoiGianKetThuc { get; set; }
        public int TongThoiGian { get; set; }
        public string StrTongThoiGian { get; set; }
        public DateTime NgayLamViec { get; set; }
      public bool? PheDuyet{ get; set; }
      public string NoiDungLamViec{ get; set; }
      public byte? TrangThaiBaoViec{ get; set; }
        public string TokenId { get; set; }
        public byte? LoaiNgayLamViec { get; set; }

    }
}
