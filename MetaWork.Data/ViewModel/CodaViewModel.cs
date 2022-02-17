using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MetaWork.Data.ViewModel
{
  public  class CodaViewModel
    {
    }
    public class CodaTablesViewModel
    {
        public List<CodaTableProjectViewModel> items { get; set; }
    }
    public class CodaTableProjectViewModel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string tableType { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public CodaTableProjectViewModel parent { get; set; }
    }
    public class CodaRowsTableProjectViewModel
    {
        public List<CodaRowTableProjectViewModel> items { get; set; }
    }
    public class CodaRowTableProjectViewModel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public object values { get; set; }
    }
    public class CodaColsTableProjectViewModel
    {
        public List<CodaColTableProjectViewModel> items { get; set; }
    }
    public class CodaColTableProjectViewModel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string href { get; set; }
        public string name { get; set; }
    }
    public class RowCodaViewModel
    {
        public RowCoda row { get; set; }
    }
    public class RowCoda
    {
        public List<CellCoda> cells { get; set; }
    }
    public class CellCoda
    {
        public string column { get; set; }
        [AllowHtml]
        public string value { get; set; }
    }
    public class dictionViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class MeetingCodaViewModel
    {
        public string id { get; set; }
        public string name { get; set; }

        public string TenDuAn { get; set; }
        public string MaDuAn { get; set; }
        public string TenMeeting { get; set; }
        public string LoaiMeeting { get; set; }
        public string StrNguoiThamGia { get; set; }
        public List<string> HoTenNguoiThamGias { get; set; }
        public string StrStartTime { get; set; }
        public string StrEndTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string NoiDung { get; set; }
        public string StrShipAble { get; set; }
        public List<string> TenShipAbles { get; set; }
        public string QuyetDinh { get; set; }
        public bool InsertOrUpdate { get; set; }
        public List<MeetingCodaViewModel> MeetingCodas { get; set; }

    }
}
