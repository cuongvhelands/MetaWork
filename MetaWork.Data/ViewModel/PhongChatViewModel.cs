using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class PhongChatViewModel
    {
        public int PhongChatId { get; set; }
        public int? KhoaChaId { get; set; }
        /// <summary>
        /// 2: phongchat
        /// 1: User
        /// </summary>
        public byte Type { get; set; }

        public string TenPhongChat { get; set; }
        public string TenVietTat { get; set; }
        public string ItemId { get; set; }
        // 1: dự án worktime
        public byte ItemType { get; set; }
        public DateTime NgayTao { get; set; }
        public List<MessageViewModel> Messages { get; set; }
        public int Count { get; set; }
        public List<PhongChatViewModel> PhongChats { get; set; }
        public int CountNguoiDung { get; set; }
        public int CountFile { get; set; }
        public Guid User { get; set; }
        public DateTime LastSend { get; set; }
        public Guid NguoiTao { get; set; }
        public string TitleChannel { get; set; }
        public DateTime DateJoin { get; set; }
        public int CountUnRead { get; set; }
        public List<NguoiDungViewModel> NguoiDungs { get; set; }
        public string FileId { get; set; }
        public List<FileViewModel> Files { get; set; }
        public string StrHTMLTabFile { get; set; }
        public List<PhongChatInfoViewModel> PhongChatInfos {get;set;}
        public List<HoTenNguoiDungViewModel> HoTenNguoiDungs { get; set; }
    }
    public class LienKetNguoiDungPhongChatViewModel
    {
        public int PhongChatId { get; set; }
        public Guid NguoiDungId { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class RoomViewModel
    {
        public string TenRoom { get; set; }
        public string ItemId { get; set; }
        public byte ItemType { get; set; }
        public string PhongChatId { get; set; }
        public List<MessageViewModel> Messages { get; set; }
    }
    public class PhongChatInfoViewModel
    {
        public int PhongChatInfoId { get; set; }
        public int PhongChatId { get; set; }
        public byte Type { get; set; }
        public string TenPhongChatInfo { get; set; }
        public string NoiDung { get; set; }
        public string GhiChu { get; set; }       
    }
}
