using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class PhongBanProvider
    {
        TimerDataContext db = new TimerDataContext();

        public PhongBanProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<PhongBanViewModel> GetAll()
        {
            try
            {
                var str = "select * from PhongBan";
                return db.ExecuteQuery<PhongBanViewModel>(str).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<PhongBanViewModel> GetByKhoaChaId(int khoaChaId)
        {
            try
            {
                var str = "select * from PhongBan where KhoaChaId="+khoaChaId;
                return db.ExecuteQuery<PhongBanViewModel>(str).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public PhongBanViewModel GetByNguoiDungId(Guid nguoiDungId)
        {
            try
            {
                var str = "select * from PhongBan as p inner join LienKetNguoiDungPhongBan as lk on p.PhongBanId=lk.PhongBanId where lk.NguoiDungId='"+nguoiDungId.ToString()+"'";
                return db.ExecuteQuery<PhongBanViewModel>(str).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<int> GetIdsByNguoiDungId(Guid nguoiDungId)
        {
            try
            {
                var str = "select p.PhongBanId from PhongBan as p inner join LienKetNguoiDungPhongBan as lk on p.PhongBanId=lk.PhongBanId where lk.NguoiDungId='" + nguoiDungId.ToString() + "'";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
