using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
   public class DoPhucTapProvider
    {
        public List<DoPhucTapViewModel> Gets()
        {
            List<DoPhucTapViewModel> results = new List<DoPhucTapViewModel>();
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Dễ", DoPhucTap = 1 ,MaMauDoPhucTap = "<span style=\"font-size:12px;color:#ECECEC\"><i class=\"fas fa-flag\"></i></span>"
            });
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Trung bình", DoPhucTap = 2,MaMauDoPhucTap= "<span style=\"font-size:12px;color:#67BF7F\"><i class=\"fas fa-flag\"></i></span>" });
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Khó", DoPhucTap = 3, MaMauDoPhucTap = "<span style=\"font-size:12px;color:#F86B6B\"><i class=\"fas fa-flag\"></i></span>" });
            return results;
        }
        public DoPhucTapViewModel GetById(byte DoPhucTap)
        {
            DoPhucTapViewModel result = new DoPhucTapViewModel() { DoPhucTap = DoPhucTap };
            switch (DoPhucTap)
            {
                case 1:
                    result.TenDoPhucTap = "Dễ";
                    result.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#ECECEC\"><i class=\"fas fa-flag\"></i></span>";
                    break;
                case 2:
                    result.TenDoPhucTap = "Trung bình";
                    result.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#67BF7F\"><i class=\"fas fa-flag\"></i></span>";
                    break;
                case 3:
                    result.TenDoPhucTap = "Khó";
                    result.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#F86B6B\"><i class=\"fas fa-flag\"></i></span>";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
