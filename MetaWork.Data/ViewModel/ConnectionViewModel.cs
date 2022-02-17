using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class ConnectionViewModel
    {
        public string ConnectionId { get; set; }
        public Guid NguoiDungId { get; set; }
        public bool Connected { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
