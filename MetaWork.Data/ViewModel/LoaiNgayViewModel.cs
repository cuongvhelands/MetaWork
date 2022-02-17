using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class LoaiNgayViewModel
    {
    }
    public class LichLamViecCaNhanViewModel
    {
        public DateTime? Day { get; set; }
        public int DayInSeconds { get; set; }
        public DayType DayType { get; set; }
        public int TongThoiGianInTime { get; set; }
        public int TongThoiGianOutTime { get; set; }
        public int TongThoiGianOffTime { get; set; }
        public string StrTongThoiGianInWork { get; set; }
        public string StrTongThoiGianOff { get; set; }
        public string StrTongThoiGian { get; set; }
        public string StrTongThoiGianNeed { get; set; }
    }
    public class DayType
    {
        public int DayTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }      
        public double TimeRequireInSeconds { get; set; }
        public double AddStaffLeaveInDays { get; set; }
    }
}
