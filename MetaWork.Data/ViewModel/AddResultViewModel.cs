using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel{
    
    /// <summary>
    /// Kết quả sau khi thêm mới một đối tượng
    /// </summary>
    public class AddResultViewModel 
    {

        /// <summary>
        /// Trạng thái thêm mới thành công hay không
        /// </summary>
        /// <value>
        ///   <c>true</c> thành công; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; }
        /// <summary>
        /// Mô tả lỗi nếu có
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Id sau khi thêm mới
        /// </summary>
        /// <value>
        /// int, uniqueidentifier
        /// </value>
        public string NewId { get; set; }

    }
}
