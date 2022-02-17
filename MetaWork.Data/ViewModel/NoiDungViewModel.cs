using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class NoiDungViewModel
    {
        public Guid NoiDungId { get; set; }
        public byte? LoaiNoiDungId { get; set; }
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Avatar { get; set; }

        public string ItemId { get; set; }
        public byte? ItemType { get; set; }
        public bool TrangThai { get; set; }
        public string NoiDungChiTiet { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public DateTime NgayTao { get; set; }
        public int ShipAbleId { get; set; }
        public bool Edit { get; set; }
    }
    public enum EnumLoaiNoiDungType
    {
        CommentDuAnAndShip=1      
    }
    public enum EnumItemTypeType
    {
        ShipAbleType = 1
    }

}
