using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class ConnectionProvider
    {
        TimerDataContext db = null;
        public ConnectionProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public bool InsertOrUpdate (string connectiongId,Guid nguoiDungId,bool connected)
        {
            try
            {
                var connect = db.Connections.Where(t => t.ConnectionId == connectiongId && t.NguoiDungId == nguoiDungId);
                if (connect != null && connect.Count() > 0) {
                    var connec = connect.FirstOrDefault();              
                    connec.NgayCapNhat = DateTime.Now;
                    connec.Connected = connected;
                }
                else
                {
                    Connection connec = new Connection();
                    connec.ConnectionId = connectiongId;
                    connec.NguoiDungId = nguoiDungId;
                    connec.NgayCapNhat = DateTime.Now;
                    connec.Connected = connected;
                    db.Connections.InsertOnSubmit(connec);
                }
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<ConnectionViewModel> GetConnectionByTenDangNhap(string tenDangNhap)
        {
            try
            {
                return (from a in db.Connections join b in db.NguoiDungs on a.NguoiDungId equals b.NguoiDungId where b.TenDangNhap.ToLower() == tenDangNhap.ToLower() select new ConnectionViewModel { ConnectionId=a.ConnectionId,NguoiDungId=a.NguoiDungId,Connected=a.Connected,NgayCapNhat=a.NgayCapNhat}).OrderByDescending(t=>t.NgayCapNhat).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<ConnectionViewModel> GetConnectionByUserId(Guid userId)
        {
            try
            {
                return (from a in db.Connections join b in db.NguoiDungs on a.NguoiDungId equals b.NguoiDungId where b.NguoiDungId == userId &&a.Connected==true select new ConnectionViewModel { ConnectionId = a.ConnectionId, NguoiDungId = a.NguoiDungId, Connected = a.Connected, NgayCapNhat = a.NgayCapNhat }).OrderByDescending(t => t.NgayCapNhat).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
