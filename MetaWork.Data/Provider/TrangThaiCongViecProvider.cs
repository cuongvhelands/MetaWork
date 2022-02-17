using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class TrangThaiCongViecProvider
    {
        TimerDataContext db = null;
        public TrangThaiCongViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<TrangThaiCongViecViewModel> Gets()
        {
            try
            {
              return  db.ExecuteQuery<TrangThaiCongViecViewModel>("Select * from TrangThaiCongViec").ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<TrangThaiCongViecViewModel> GetByKhoaChaId(int khoaChaId)
        {
            try
            {
                return db.ExecuteQuery<TrangThaiCongViecViewModel>("Select * from TrangThaiCongViec where KhoaChaId="+khoaChaId).ToList();
            }
            catch
            {
                return null;
            }
        }

        public TrangThaiCongViecViewModel GetById(int id)
        {
            try
            {
                return db.ExecuteQuery<TrangThaiCongViecViewModel>("Select * from TrangThaiCongViec where TrangThaiCongViecId=" + id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
