using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class LienKetNguoiDungDuAnViewModel
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public int DuAnId { get; set; }
        public int? QuyenId { get; set; }
        public bool? LaQuanLy { get; set; }
        public string Avatar { get; set; }
        public DateTime NgayThamGia { get; set; }
    }
}
