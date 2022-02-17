using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class HangMucCongViecProvider
    {
        TimerDataContext db = null;
        public HangMucCongViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }

        public int Insert(string tenHangMucCongViec, bool trangThai,Guid userId)
        {
            try
            {
                HangMucCongViec entity = new HangMucCongViec();
                entity.TenHangMuc = tenHangMucCongViec;
                entity.TrangThai = trangThai;
                entity.NguoiCapNhatId = userId;
                entity.NgayCapNhat = DateTime.Now;
                db.HangMucCongViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.HangMucCongViecId;
            }
            catch
            {
                return 0;
            }
        }

        public List<HangMucCongViecViewModel> GetsByGiaiDoanDuAnId(int giaiDoanDuAnId)
        {
            try
            {
                var str = "select * from HangMucCongViec where GiaiDoanDuAnId =" + giaiDoanDuAnId;
                return db.ExecuteQuery<HangMucCongViecViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
