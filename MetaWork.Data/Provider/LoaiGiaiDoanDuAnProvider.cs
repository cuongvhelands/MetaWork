using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class LoaiGiaiDoanDuAnProvider
    {/// <summary>
    /// Chuyển DB sau
    /// </summary>
    /// <returns></returns>
        public List<LoaiGiaiDoanDuAnViewModel> Gets()
        {
            List<LoaiGiaiDoanDuAnViewModel> lstToReturn = new List<LoaiGiaiDoanDuAnViewModel>();
            lstToReturn.Add(new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId = 1, TenLoaiGiaiDoan = "Khởi động",MaMau= "label-default" });
            lstToReturn.Add(new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId = 2, TenLoaiGiaiDoan = "Nghiên cứu",MaMau= "label-primary" });
            lstToReturn.Add(new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId = 3, TenLoaiGiaiDoan = "Thiết kế",MaMau= "label-warning" });
            lstToReturn.Add(new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId = 4, TenLoaiGiaiDoan = "Sản xuất",MaMau= "label-success" });
            lstToReturn.Add(new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId = 5, TenLoaiGiaiDoan = "Kết thúc", MaMau = "label-info" });

            return lstToReturn;
        }
        public LoaiGiaiDoanDuAnViewModel GetById(int loaiGiaiDoanDuAnId)
        {
            LoaiGiaiDoanDuAnViewModel result = new LoaiGiaiDoanDuAnViewModel() { LoaiGiaiDoanDuAnId=loaiGiaiDoanDuAnId };
            switch (loaiGiaiDoanDuAnId)
            {
                case 1:
                    result.TenLoaiGiaiDoan = "Khởi động";
                    result.MaMau = "label-default";
                    break;
                case 2:
                    result.TenLoaiGiaiDoan = "Nghiên cứu";
                    result.MaMau = "label-primary";
                    break;
                case 3:
                    result.TenLoaiGiaiDoan = "Thiết kế";
                    result.MaMau = "label-warning";
                    break;
                case 4:
                    result.TenLoaiGiaiDoan = "Sản xuất";
                    result.MaMau = "label-success";
                    break;
                case 5:
                    result.TenLoaiGiaiDoan = "Kết thúc";
                    result.MaMau = "label-info";
                    break;
              
                default:
                break;
            } 
            return result;
        }
    }
}
