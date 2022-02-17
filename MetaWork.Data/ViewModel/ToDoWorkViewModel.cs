using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ToDoWorkViewModel
    {
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public List<DuAnViewModel> DuAns { get; set; }
        public bool QuanTam { get; set; }
        public byte Type { get; set; }
        public List<TimeTypeViewModel> TimeTypes { get; set; }
        public List<TrangThaiCongViecViewModel> TrangThaiShipables { get; set; }
        public List<TrangThaiCongViecViewModel> TrangThaiTasks { get; set; }
        public int TrangThaiCongViecId { get; set; }
        public List<NguoiDungViewModel> NguoiDungAll { get; set; }
        public string StrNguoiDungId { get; set; }
        public List<DuAnViewModel> DuAnAll { get; set; }
        public List<int> DuAnIds { get; set; }
        public string strDuAnIds { get; set; }
      
        public string TenShipable { get; set; }
        public CongViecViewModel CurrentTask { get; set; }
        public int CurrentTodoId { get; set; }
        public List<int> PhongBanIds { get; set; }
        public string StrPhongBanIds { get; set; }
        public List<MauGoiChuyenGiaoViewModel> MauGoiChuyenGiaos { get; set; }
        public List<TrangThaiDuAnViewModel> TrangThaiDuAns { get; set; }
        public List<int> TrangThaiDuAnIds { get; set; }
        public string StrTrangThaiDuAnIds { get; set; }

    }
}
