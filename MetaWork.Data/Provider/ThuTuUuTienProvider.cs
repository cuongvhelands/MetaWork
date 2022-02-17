using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
   public class ThuTuUuTienProvider
    {
        public List<ThuTuUuTienViewModel> Gets()
        {
            List<ThuTuUuTienViewModel> results = new List<ThuTuUuTienViewModel>();
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Thấp", ThuTuUuTien = 1 ,MaMauThuTuUuTien = "fa fa-arrow-circle-down text-secondary"
        });
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Trung bình", ThuTuUuTien = 2,MaMauThuTuUuTien= "fa fa-arrow-circle-right text-info"});
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Cao", ThuTuUuTien = 3, MaMauThuTuUuTien = "fa fa-arrow-circle-up text-danger" });
            return results;
        }
        public ThuTuUuTienViewModel GetById(byte thuTuUuTien)
        {
            ThuTuUuTienViewModel result = new ThuTuUuTienViewModel() { ThuTuUuTien = thuTuUuTien };
            switch (thuTuUuTien)
            {
                case 1:
                    result.TenThuTuUuTien = "Thấp";
                    result.MaMauThuTuUuTien = "fa fa-arrow-circle-down text-secondary";
                    break;
                case 2:
                    result.TenThuTuUuTien = "Trung bình";
                    result.MaMauThuTuUuTien = "fa fa-arrow-circle-right text-info";
                    break;
                case 3:
                    result.TenThuTuUuTien = "Cao";
                    result.MaMauThuTuUuTien = "fa fa-arrow-circle-up text-danger";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
