using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class LoaiCongViecProvider
    {
        TimerDataContext db = null;
        public LoaiCongViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }

        public int Insert(string tenLoaiCongViec, string maLoaiCongViec)
        {
            try
            {
                LoaiCongViec entity = new LoaiCongViec();
                entity.TenLoaiCongViec = tenLoaiCongViec;
                entity.MaLoaiCongViec = maLoaiCongViec;
                entity.NgayTao = DateTime.Now;
                db.LoaiCongViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.LoaiCongViecId;
            }
            catch
            {
                return 0;
            }
        }
        public List<LoaiCongViecViewModel> Gets()
        {
            try
            {
                return db.ExecuteQuery<LoaiCongViecViewModel>("Select * from LoaiCongViec").ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<LoaiCongViecViewModel> GetsByKhoaChaId(int khoaChaId)
        {
            try
            {
                var str = "Select * from LoaiCongViec";
                if (khoaChaId > 0) str += " where KhoaChaId=" + khoaChaId;
                else str += " where KhoaChaId IS NULL";
                return db.ExecuteQuery<LoaiCongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<int> GetIdsByDuAnId(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<int>("Select LoaiCongViecId from LienKetLoaiCongViecDuAn where DuAnId=" + duAnId).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<LoaiCongViecViewModel> GetAllLoaiCongViecCon()
        {
            try
            {
                var str = "Select * from LoaiCongViec where KhoaChaId IS NOT NULL";
                return db.ExecuteQuery<LoaiCongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy loại công việc con theo khoa cha trong loai cong việc của dự án
        /// </summary>
        /// <param name="duAnId"></param>
        /// <returns></returns>
        public List<LoaiCongViecViewModel> GetLoaiCongViecConByDuAnId(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<LoaiCongViecViewModel>("Select * from LoaiCongViec where KhoaChaid in(Select LoaiCongViecId from LienKetLoaiCongViecDuAn where DuAnId=" + duAnId+")").ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<LoaiCongViecViewModel> GetIdsByDuAn(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<LoaiCongViecViewModel>("Select * from LienKetLoaiCongViecDuAn as lk inner join LoaiCongViec as l on l.LoaiCongViecId = lk.LoaiCongViecId where lk.DuAnId=" + duAnId).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<LoaiCongViecViewModel> GetAll()
        {
            try
            {
                return db.ExecuteQuery<LoaiCongViecViewModel>("Select * from LoaiCongViec").ToList();
            }
            catch
            {
                return null;
            }
        }
        public bool InsertLienKetLoaiCongViecDuAn(int duAnId, int loaiCongViecId)
        {
            try
            {
                LienKetLoaiCongViecDuAn entity = new LienKetLoaiCongViecDuAn() { DuAnId = duAnId, LoaiCongViecId = loaiCongViecId };
                db.LienKetLoaiCongViecDuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertLienKetLoaiCongViecs(int congViecId, List<int> loaiCongViecIds)
        {
            try
            {
                try
                {
                    var lstE = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == congViecId).ToList();
                    if (lstE!=null&& lstE.Count > 0)
                    {
                        db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lstE);
                        db.SubmitChanges();
                    }
                }
                catch
                {

                }

                if (loaiCongViecIds != null && loaiCongViecIds.Count > 0)
                {
                    List<LienKetLoaiCongViec> entitys = new List<LienKetLoaiCongViec>();
                    foreach(var item in loaiCongViecIds)
                    {
                        LienKetLoaiCongViec entity = new LienKetLoaiCongViec() { CongViecId = congViecId, LoaiCongViecId = item };
                        entitys.Add(entity);
                    }
                    db.LienKetLoaiCongViecs.InsertAllOnSubmit(entitys);
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<int> GetIdsByCongViecId(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<int>("Select LoaiCongViecId from LienKetLoaiCongViec where CongViecId=" + congViecId).ToList();
            }
            catch
            {
                return null;
            }
        }
      
        public List<LoaiCongViecViewModel> GetsByCongViecId(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<LoaiCongViecViewModel>("Select * from LoaiCongViec as l inner join  LienKetLoaiCongViec as lk on l.LoaiCongViecId=lk.LoaiCongViecId where lk.CongViecId=" + congViecId).ToList();
            }
            catch
            {
                return null;
            }
        }
        public bool DeleteLienKetLoaiCongViecDuAn(int duAnId, int loaiCongViecId)
        {
            try
            {
                var entity = db.LienKetLoaiCongViecDuAns.Where(t => t.DuAnId == duAnId && t.LoaiCongViecId == loaiCongViecId).FirstOrDefault();
                db.LienKetLoaiCongViecDuAns.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
