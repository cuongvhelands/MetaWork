using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class TagViewModel
    {
        public int TagId { get; set; }
        public Guid LienKetTagId { get; set; }
        public string TenTag { get; set; }
        public Guid NguoiTao { get; set; }
        public string ItemId { get; set; }
        // 1 : Task
        public byte ItemType { get; set; }
    }
    public enum EnumTagType
    {
        Task=1,     
    }
}
