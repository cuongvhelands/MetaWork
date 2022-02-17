using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class MauGoiChuyenGiaoProvider
    {
        TimerDataContext db = null;
        public MauGoiChuyenGiaoProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<MauGoiChuyenGiaoViewModel> GetAll()
        {
            try
            {
                var str = "select * from MauGoiChuyenGiao";
                return db.ExecuteQuery<MauGoiChuyenGiaoViewModel>(str).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public MauGoiChuyenGiaoViewModel GetById(int id)
        {
            try
            {
                return db.ExecuteQuery<MauGoiChuyenGiaoViewModel>("Select * from MauGoiChuyenGiao where MauGoiChuyenGiaoId=" + id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public int Insert(string tenMauGoi, string TenShip,string noiDung, string value,Guid userId)
        {
            try
            {
                var check = db.MauGoiChuyenGiaos.Count(t => t.TenMau == tenMauGoi);
                if (check > 0) return 0;
                MauGoiChuyenGiao entity = new MauGoiChuyenGiao();
                entity.TenMau = tenMauGoi;
                entity.LoaiMau = 1;
                entity.NgayCapNhat = DateTime.Now;
                entity.NguoiCapNhat = userId;
                entity.StrTask = value;
                entity.NoiDung = noiDung;
                entity.TenShip = TenShip;
                db.MauGoiChuyenGiaos.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.MauGoiChuyenGiaoId;
            }
            catch
            {
                return 0;
            }
        }
    }
}
