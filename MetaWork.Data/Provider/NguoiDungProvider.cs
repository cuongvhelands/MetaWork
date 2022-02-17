using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class NguoiDungProvider
    {
        TimerDataContext db = null;
        public NguoiDungProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }

        public List<HoTenNguoiDungViewModel> GetHoTenNguoiDung()
        {
            return (from a in db.NguoiDungs where a.TrangThai == true select new HoTenNguoiDungViewModel(){HoTen=a.HoTen,NguoiDungId=a.NguoiDungId  }).OrderBy(a=>a.HoTen).ToList();
        }
        public List<NguoiDungViewModel> GetNguoiDungByQuyen( byte quyen)
        {
            try
            {
                return (from n in db.NguoiDungs join l in db.LienKetNguoiDungPhongBans on n.NguoiDungId equals l.NguoiDungId join p in db.PhongBans on l.PhongBanId equals p.PhongBanId where n.Quyen == quyen && n.TrangThai==true select new NguoiDungViewModel { NguoiDungId = n.NguoiDungId, HoTen = n.HoTen, Email = n.Email, TenDangNhap = n.TenDangNhap, MatKhau = n.MatKhau, PhongBanId = p.PhongBanId, TenPhongBan = p.TenPhongBan }).ToList();
            }
            catch
            {
                return null;
            }
        }


        public List<NguoiDungViewModel> GetLeadersByPhongBanId(int phongBanId)
        {
            return (from n in db.NguoiDungs join l in db.LienKetNguoiDungPhongBans on n.NguoiDungId equals l.NguoiDungId join p in db.PhongBans on l.PhongBanId equals p.PhongBanId where l.PhongBanId == phongBanId && n.Quyen == 3 && n.TrangThai == true select new NguoiDungViewModel { NguoiDungId = n.NguoiDungId, HoTen = n.HoTen, Email = n.Email, TenPhongBan = p.TenPhongBan ,PhongBanId=p.PhongBanId}).ToList();
        }

        public NguoiDungActivityViewModel GetActiveBy(Guid nguoiDungId, DateTime date)
        {
            try
            {
                return (from l in db.LienKetNguoiDungActivities join a in db.Avtivities on l.ActivityId equals a.ActivityId where l.NguoiDungId == nguoiDungId && l.NgayCapNhat >= date select new NguoiDungActivityViewModel { ActivityId = a.ActivityId, NguoiDungId = l.NguoiDungId, LienKetNguoiDungActivityId = l.LienKetNguoiDungActivityId, TenActivity = a.TenActivity, Icon = a.icon, Active = l.Actice }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public bool InsertOrUpdateActivity(string tenActivity,bool Active, Guid nguoiDungId,string icon)
        {
            try
            {
                int activityId = 0;
                try
                {
                    var activity = db.Avtivities.Where(t => t.TenActivity.Equals(tenActivity)).FirstOrDefault();
                    activityId = activity.ActivityId;
                }
                catch
                {                   
                    var activity = new Avtivity() { icon = icon, TenActivity = tenActivity, NgayTao = DateTime.Now, NguoiTao = nguoiDungId } ;
                    db.Avtivities.InsertOnSubmit(activity);
                    db.SubmitChanges();
                    activityId = activity.ActivityId;
                }
                try
                {
                    var lk = db.LienKetNguoiDungActivities.Where(t => t.ActivityId == activityId && t.NguoiDungId == nguoiDungId).FirstOrDefault();
                    lk.NgayCapNhat = DateTime.Now;
                    lk.Actice = Active;
                    db.SubmitChanges();
                }
                catch
                {
                    var lk = new LienKetNguoiDungActivity() { ActivityId = activityId, NguoiDungId = nguoiDungId, NgayCapNhat = DateTime.Now, Actice = Active };
                    db.LienKetNguoiDungActivities.InsertOnSubmit(lk);
                    db.SubmitChanges();
                }
                return true;
               
            }
            catch
            {
                return false;
            }
        }
        public NguoiDungViewModel GetByHoTen (string name)
        {
            try
            {
                return (from a in db.NguoiDungs where a.HoTen.Equals(name) select new NguoiDungViewModel { NguoiDungId = a.NguoiDungId, HoTen = a.HoTen }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public Guid GetNguoiDungIdByPhoneNumber(string phoneNumber)
        {
            try
            {
                return db.NguoiDungs.Where(n => n.SoDienThoai.Equals(phoneNumber.Trim())).FirstOrDefault().NguoiDungId;
            }
            catch(Exception ex)
            {
                return Guid.Empty;
            }
        }

        public List<Guid> GetIdsBy(List<int> phongbanId, List<int> duAnIds)
        {
            try
            {
                var str = "select n.NguoiDungId from nguoiDung as n";               
                    str += " join LienKetNguoiDungPhongBan as lkP on n.NguoiDungId = lkP.NguoiDungId ";
                    
                if (duAnIds != null && duAnIds.Count > 0)
                {
                  
                    str += 
                        " join PhongBan as P on lkP.PhongBanId = P.PhongBanId join LienKetDuAnPhongBan as lkD on P.PhongBanId = lkD.PhongBanId";
                }
                var check = false;
                if (phongbanId != null && phongbanId.Count > 0)
                {
                    check = true;
                    str += " where lkP.PhongBanId in (";
                    foreach(var id in phongbanId)
                    {
                        str += id + ",";
                    }
                    str= str.Substring(0, str.Length - 1);
                    str += ")";
                }
                if (duAnIds != null && duAnIds.Count > 0)
                {
                    if (check)
                        str += " and ";
                    else str += " where";
                    str += " lkD.DuAnId in (";
                    foreach (var id in duAnIds)
                    {
                        str += id + ",";
                    }
                    str = str.Substring(0, str.Length - 1);
                    str += ")";
                }
                str += "  group by n.NguoiDungId";
                return db.ExecuteQuery<Guid>(str).ToList();
            }
            catch
            {
                try
                {
                    return db.NguoiDungs.Where(t => t.TrangThai == true).Select(t => t.NguoiDungId).ToList();
                }
                catch
                {
                    return null;
                }
               

            }
        }

        public List<NguoiDungViewModel> GetsByIds(List<Guid> ids)
        {
            try
            {
                return (from n in db.NguoiDungs where ids.Contains(n.NguoiDungId) select new NguoiDungViewModel { NguoiDungId = n.NguoiDungId, HoTen = n.HoTen ,Avatar=n.Avatar}).ToList();
            }
            catch
            {
                return null;
            }
           
        }
        public NguoiDungViewModel GetBy(string userName, string password)
        {
            try
            {
                return db.ExecuteQuery<NguoiDungViewModel>("select * from NguoiDung where TenDangNhap='" + userName + "' and MatKhau='" + password + "' and TrangThai =1").FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public NguoiDungViewModel GetById(Guid nguoiDungId)
        {
            try
            {
                return db.ExecuteQuery<NguoiDungViewModel>("Select * from NguoiDung where NguoiDungId = '" + nguoiDungId.ToString() + "'").FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public List<NguoiDungViewModel> GetsExcept(List<Guid> ids)
        {
            try
            {
                var str = "select * from NguoiDung ";
                if (ids != null && ids.Count > 0)
                {
                    str += "where NguoiDungId not in (";
                    var leng = ids.Count;
                    var i = 1;
                    foreach (var item in ids)
                    {
                        str += "'" + item.ToString() + "'";
                        if (i < leng)
                        {
                            str += ",";
                        }
                        i++;
                    }
                    str += ")";
                }
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<NguoiDungViewModel> GetsByDuAnId(int duAnId)
        {
            try
            {
                var str = "select * from NguoiDung as n inner join LienKetNguoiDungPhongBan as l on n.NguoiDungId=l.NguoiDungId inner join PhongBan as p on l.PhongBanId = p.PhongBanId inner join LienKetDuAnPhongBan as lk on p.PhongBanId = lk.PhongBanId inner join DuAn as dac on lk.DuAnId = dac.DuAnId inner join DuAn as d on dac.DuAnId = d.KhoaChaId";
                if(duAnId>0) str+=" where d.DuAnId=" + duAnId;
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<NguoiDungViewModel> GetsByDuAnId(int duAnId, Guid? nguoiXuLyId)
        {
            try
            {
                var str = "select * from NguoiDung as n inner join LienKetNguoiDungDuAn as l on n.NguoiDungId=l.NguoiDungId where l.DuAnId=" + duAnId;
                if (nguoiXuLyId != null && nguoiXuLyId != Guid.Empty) str += "and n.NguoiDungId != '" + nguoiXuLyId + "'";

                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<NguoiDungViewModel> GetAll()
        {
            try
            {
                var str = "select * from NguoiDung where TrangThai=1";
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<Guid> GetUserIdsBy(int? phongBanId)
        {
            try
            {
                var str = "select n.NguoiDungId from NguoiDung as n ";
                if (phongBanId > 0)
                {
                    str += "inner join LienKetNguoiDungPhongBan as l on n.NguoiDungId=l.NguoiDungId inner join PhongBan as p on l.PhongBanId=p.PhongBanId where p.KhoaChaId =" + phongBanId + " or p.PhongBanId=" + phongBanId + " group by n.NguoiDungId";
                }
                return db.ExecuteQuery<Guid>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<NguoiDungViewModel> GetNguoiDungsByPhongBanId(int id)
        {
            try
            {
                var str = "select * from NguoiDung ";
                if (id > 0) str += "as n inner join LienKetNguoiDungPhongBan as lk on n.NguoiDungId=lk.NguoiDungId where n.TrangThai=1 and lk.PhongBanId=" + id;
                else str += " where TrangThai=1";
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public NguoiDung GetUserByUsernameAndPassword(string username, string password)
        {
            try
            {
                return db.NguoiDungs.SingleOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int Login(string userName, string password)
        {
            var result = db.NguoiDungs.SingleOrDefault(u => u.TenDangNhap == userName);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.MatKhau == password)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        public List<NguoiDungViewModel> GetNguoiQuanLyBy(int duAnId)
        {
            try
            {
                var str = "select * from NguoiDung as n inner join LienKetNguoiDungDuAn as l on n.NguoiDungId=l.NguoiDungId where l.LaQuanLy=1 and l.DuAnId=" + duAnId;
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public NguoiDung GetUserByUsername(string username)
        {
            try
            {
                return db.NguoiDungs.SingleOrDefault(x => x.TenDangNhap == username);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool ChangePassword(Guid id,string new_password)
        {
            try
            {
                var model = db.NguoiDungs.SingleOrDefault(x => x.NguoiDungId == id);
                model.MatKhau = new_password;
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<NguoiDungViewModel> GetNguoiDungByLoai(int loaiNguoiDung)
        {
            try
            {
                return db.ExecuteQuery<NguoiDungViewModel>("Select * from NguoiDung where LoaiNguoiDungId = " + loaiNguoiDung).ToList();

            }
            catch
            {
                return null;
            }
        }

        public byte GetQuyenNguoiDungById(Guid nguoiDungId)
        {
            try
            {
                var str = "select Quyen from NguoiDung where NguoiDungId ='" + nguoiDungId.ToString() + "'";
                return db.ExecuteQuery<byte>(str).FirstOrDefault();
            }
            catch
            {
                return 1;
            }
        }

        public List<NguoiDungViewModel> GetNguoiDungByEmails(List<string> emails)
        {
            try
            {
                var str = "select * from NguoiDung";
                if (emails != null && emails.Count > 0)
                {
                    str += " where Email in(";
                    int i = 1;
                    foreach(var item in emails)
                    {
                        str += "'"+item+"'";
                        if (i < emails.Count) str += ",";
                        i++;
                    }
                    str+=")";
                }
                return db.ExecuteQuery<NguoiDungViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public NguoiDungViewModel GetNguoiDungByEmail(string email)
        {
            try
            {
                var str = "select * from NguoiDung where Email = '"+email+"' ";
               
                   
                return db.ExecuteQuery<NguoiDungViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public NguoiDungViewModel GetAvatarNguoiDungById(Guid nguoiDungId)
        {
            try{
                return (from n in db.NguoiDungs where n.NguoiDungId == nguoiDungId && n.TrangThai == true select new NguoiDungViewModel { NguoiDungId = n.NguoiDungId, HoTen = n.HoTen, Avatar = n.Avatar }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
            
        }
        public bool Update(Guid userId, string userNameSlack)
        {
            try
            {
                var user = db.NguoiDungs.Where(t => t.NguoiDungId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.UserNameSlack = userNameSlack;
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool IsExistUserIdSlack(string userIdSlack)
        {
            try
            {
                var user = db.NguoiDungs.Where(t => t.UserNameSlack == userIdSlack).FirstOrDefault();
                if (user != null&&user.NguoiDungId!=Guid.Empty)
                {                    
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public NguoiDung GetByUserIdSlack(string userIdSlack)
        {
            try
            {
                return db.NguoiDungs.Where(t => t.UserNameSlack.ToUpper() == userIdSlack.Trim().ToUpper()).FirstOrDefault();
               
            }
            catch
            {
                return null;
            }
        }
        public NguoiDungViewModel GetNguoiDungByUserName(string username)
        {
            try
            {
                return (from n in db.NguoiDungs where n.TenDangNhap == username select new NguoiDungViewModel { HoTen = n.HoTen, NguoiDungId = n.NguoiDungId, Email = n.Email ,Avatar=n.Avatar}).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

    }
}
