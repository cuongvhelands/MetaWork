using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class TrangThaiDuAnProvider
    {
        TimerDataContext db = null;
        public TrangThaiDuAnProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<TrangThaiDuAnViewModel> Gets()
        {
            try
            {
                return db.ExecuteQuery<TrangThaiDuAnViewModel>("Select * from TrangThaiDuAn").ToList();
            }
            catch
            {
                return null;
            }
        }
        public TrangThaiDuAnViewModel GetByName(string name)
        {
            try
            {
                return (from a in db.TrangThaiDuAns where a.TenTrangThaiDuAn.Equals(name)  select new TrangThaiDuAnViewModel { TrangThaiDuAnId = a.TrangThaiDuAnId, TenTrangThaiDuAn = a.TenTrangThaiDuAn }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public int InsertTrangThaiDuAn(TrangThaiDuAnViewModel vm)
        {
            try
            {
                TrangThaiDuAn entity = new TrangThaiDuAn();
              
                entity.TenTrangThaiDuAn = vm.TenTrangThaiDuAn;
                entity.MaMau = vm.MaMau; 
                db.TrangThaiDuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.TrangThaiDuAnId;
            }
            catch
            {
                return 0;
            }
        }
    }
}
