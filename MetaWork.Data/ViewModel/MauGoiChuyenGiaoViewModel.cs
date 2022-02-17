using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
   public class MauGoiChuyenGiaoViewModel
    {
      public int MauGoiChuyenGiaoId { get; set; }
      public string TenMau { get; set; }
        public string TenShip { get; set; }
      public byte LoaiMau { get; set; }
        public string NoiDung { get; set; }
      public string StrTask { get; set; }
       
      public DateTime? NgayCapNhat { get; set; }
      public Guid? NguoiCapNhat { get; set; }
    }
}
