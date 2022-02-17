using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    

public class ActivityViewModel 
    {
        public long Id { get; set; }
        public string MaDinhDanh { get; set; }
        public byte? ActivityType { get; set; } = 2;
        public byte? NotificationType { get; set; }
        public long ChildId { get; set; }
        public long? LeadId { get; set; }
        public long? PartnerId { get; set; }
        public long? ContactId { get; set; }
        public long? QuotationId { get; set; }
        public long? OpportunityId { get; set; }
        public string TieuDe { get; set; }
        public byte TinhTrang { get; set; } = 1;
        public string NguoiXuLyId { get; set; }
        public string NguoiTaoId { get; set; }
      
        public string NgayTao { get; set; } 
        
        public string NgayBatDau { get; set; }
       
        public string NgayKetThuc { get; set; }

        public DateTime? DNgayBatDau { get; set; }
    }

}
