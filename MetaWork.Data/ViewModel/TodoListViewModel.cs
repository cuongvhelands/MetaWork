using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class TodoListViewModel
    {
        public string KeyWord { get; set; }
        public int Week {get;set;}
        public string StrWeek { get; set; }
        public int Year { get; set; }
        public Guid NguoiDungId { get; set; }
        public int DuAnId { get; set; }
        public int XacNhanHoanThanh { get; set; }
        public List<CongViecViewModel> ShipAbles { get; set; }
        public int Count { get; set; }
        public IPagedList<CongViecViewModel> PageList { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
    }
}
