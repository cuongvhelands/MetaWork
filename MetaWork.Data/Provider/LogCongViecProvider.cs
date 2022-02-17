using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
   public class LogCongViecProvider
    {
        TimerDataContext db = null;
        public LogCongViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<LogCongViecViewModel> GetsByCongViecId(int congViecId,string tenLog)
        {
            try
            {
                return (from a in db.LogCongViecs.Where(t => t.CongViecId == congViecId && tenLog.ToUpper() == tenLog.ToUpper()) select new LogCongViecViewModel() {CongViecId=congViecId,LogCongViecId= a.LogCongViecId,NgayTao=a.NgayTao,}).ToList();
            }
            catch
            {
                return null;
            }
        }
        public int Insert(int congViecId, string tenLog, string noiDung,Guid nguoiDungId)
        {
            try
            {
                LogCongViec entity = new LogCongViec();
                entity.CongViecId = congViecId;
                entity.TenLogCongViec = tenLog;
                entity.NoiDungLog = noiDung;
                entity.NgayTao = DateTime.Now;
                entity.NguoiTao = nguoiDungId;
                db.LogCongViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.LogCongViecId;
            }
            catch
            {
                return 0;
            }
        }
    }
}
