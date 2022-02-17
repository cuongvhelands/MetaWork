using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class HangMucCongViecViewModel
    {
        public int HangMucCongViecId { get; set; }
        public int GiaiDoanDuAnId { get; set; }
        public string TenHangMuc { get; set; }
        public string MoTa { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhatId { get; set; }
    }
}
