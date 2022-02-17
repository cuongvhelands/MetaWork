using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class MessageViewModel
    {
        public Guid MessageId { get; set; }
        public Guid NguoiGuiId { get; set; }
        public string HoTen { get; set; }
        public string Avatar { get; set; }
        public string DiaChiNhan { get; set; }
        /// <summary>
        /// 1: user
        /// 2: phongChat
        /// </summary>
        public byte Type { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public bool? DaXem { get; set; }
        public List<MessageViewModel> Messages { get; set; }
    }
    public class LienKetMessageViewModel
    {
        public Guid MessageId { get; set; }
        public Guid NguoiDungId { get; set; }
        public bool DaXem { get; set; }
        public bool Thich { get; set; }

    }
}
