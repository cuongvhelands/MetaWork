using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MetaWork.Data.ViewModel
{
    public class GiaiDoanDuAnViewModel
    {
        public int GiaiDoanDuAnId { get; set; }
        public int DuAnId { get; set; }
        public int LoaiGiaiDoanId { get; set; }
        public string TenLoaiGiaiDoan { get; set; }
        public string MaMau { get; set; }
        public string TenGiaiDoan { get; set; }
        [AllowHtml]
        public string MoTa { get; set; }
        public string StrThoiGianBatDau { get; set; }
        public string StrThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public bool TrangThaiHienTai { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public string GhiChu { get; set; }
        public int CountHangMucChecked { get; set; }
        public List<HangMucCongViecViewModel> HangMucCongViecs { get; set; }
        public List<CongViecViewModel> Shipables { get; set; }
    }
    public class GiaiDoanDuAnCodaViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string Ma { get; set; }
        public string Parent { get; set; }
        public string ProjectName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int CostH { get; set; }
        public int CostTien { get; set; }
        public int Budget { get; set; }
        public string Risk { get; set; }
        [AllowHtml]
        public string Document { get; set; }
        public string Note { get; set; }
        public string Slack_Name { get; set; }
        public string TeamLead { get; set; }
        public bool isInsert { get; set; }
        public bool isUpdate { get; set; }
    }
    public class TableGiaiDoanDuAnCodaViewModel{
        public List<GiaiDoanDuAnCodaViewModel> lst { get; set; }
        public string message { get; set; }
        }
}
