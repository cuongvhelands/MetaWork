using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class FileViewModel
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid NguoiTao { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        /// <summary>
        /// 0: Folder
        /// 1: File Input.
        /// 2: File Document.
        /// 3: File Link
        /// </summary>
        public byte FileType { get; set; }
        public string ItemId { get; set; }
        /// <summary>
        /// 1: User
        /// 2: Channel
        /// 3: File
        /// </summary>
        public byte ItemType { get; set; }
        public List<LienKetFileViewModel> LienKetFiles { get; set; }
    }
    public class LienKetFileViewModel
    {
        public Guid FileId { get; set; }
        public string ItemId { get; set; }
        /// <summary>
        /// 1,PhongChat
        /// 
        /// </summary>
        public byte ItemType { get; set; }
    }
    public class FileReturnViewModel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string TextContent { get; set; }
    }
}
