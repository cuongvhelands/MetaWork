using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Models
{
    public class KhachHangModel
    {
        KhachHangProvider _manager;
        public KhachHangModel()
        {
            _manager = new KhachHangProvider();
        }
        public List<KhachHangViewModel> Gets()
        {
            return _manager.Gets();
        }
    }
}