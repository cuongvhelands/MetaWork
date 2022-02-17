using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class PhongChatProvider
    {
        TimerDataContext db = null;
        public PhongChatProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        #region Get 
        public List<PhongChatInfoViewModel> GetPhongChatInfosBy(int phongChatId, string tenPhongChat)
        {
            return (from a in db.PhongChatInfos where a.PhongChatId == phongChatId && (a.TenPhongChatInfo.ToUpper() == tenPhongChat.ToUpper() || string.IsNullOrEmpty(tenPhongChat)) select new PhongChatInfoViewModel() { PhongChatInfoId = a.PhongChatInfoId, PhongChatId = a.PhongChatId, TenPhongChatInfo = a.TenPhongChatInfo, NoiDung = a.NoiDung, GhiChu = a.GhiChu }).ToList();
        }
        public int GetPhongChatIdBy(string tenPhongChat)
        {
            try
            {
                if (db.PhongChats.Where(t => t.TenPhongChat.ToUpper().Equals(tenPhongChat.ToUpper())).Count() == 0) return 0;
                return db.PhongChats.Where(t => t.TenPhongChat.ToUpper().Equals(tenPhongChat.ToUpper())).FirstOrDefault().PhongChatId;
            }
            catch
            {
                return 0;
            }
        }
        public List<Guid> GetNguoiDungIdsBy(int phongChatId)
        {
            try
            {
                var queryable = (from a in db.LienKetNguoiDungPhongChats where a.PhongChatId == phongChatId select a.NguoiDungId);

                return queryable.ToList();

            }
            catch
            {
                return null;
            }
        }
        public int CountNguoiDungBy(int phongChatId)
        {
            try
            {
                var queryable = (from a in db.LienKetNguoiDungPhongChats where a.PhongChatId == phongChatId select a.NguoiDungId);

                return queryable.Count();

            }
            catch
            {
                return 0;
            }
        }
        public PhongChatViewModel GetById(int PhongChatId)
        {
            try
            {
                var str = "Select  p.PhongChatId,p.TenPhongChat,p.KhoaChaId,p.Type,p.TenVietTat,p.ItemId,p.ItemType,p.NgayTao,p.NguoiTao,l.NgayTao as DateJoin from PhongChat as p inner join LienKetNguoiDungPhongChat as l on p.PhongChatId = l.PhongChatId where p.PhongChatId=" + PhongChatId;
                return db.ExecuteQuery<PhongChatViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public PhongChatViewModel GetById2(int PhongChatId)
        {
            try
            {
                var str = "Select  * from phongChat where PhongChatid = " + PhongChatId;
                return db.ExecuteQuery<PhongChatViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<PhongChatViewModel> GetByKhoaChaId(int phongChatChaId)
        {
            try
            {
                var str = "select c.PhongChatId,c.KhoaChaid,c.Type,c.TenPhongChat,c.TenVietTat,(select COUNT(m.MessageId) from Message as m where m.DiaChiNhan = CAST(c.PhongChatId as varchar(10) )) as Count from PhongChat as c where c.KhoaChaId =  " + phongChatChaId;
                return db.ExecuteQuery<PhongChatViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<MessageViewModel> GetMessagesByDiaChiNhan(string diaChiNhan, int pageNumber, int pageSize, Guid nguoiDungId)
        {
            try
            {
                var str = "Select m.MessageId,m.DiaChiNhan,m.NguoiGuiId,m.Type,m.NoiDung,m.NgayTao,m.Title,l.DaXem from Message as m  left join LienKetMessage as l on m.MessageId = l.MessageId where m.DiaChiNhan ='" + diaChiNhan + "' and l.NguoiNhanId='" + nguoiDungId.ToString() + "' order by m.NgayTao desc";
                if (pageNumber > 0 && pageSize > 0) str += " offset " + (pageNumber - 1) * pageSize + " rows fetch next " + pageSize + " rows only";
                return db.ExecuteQuery<MessageViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<PhongChatViewModel> GetsByNguoiDungId(Guid nguoiDungId)
        {
            try
            {
                var queryable = (from a in db.PhongChats join b in db.LienKetNguoiDungPhongChats on a.PhongChatId equals b.PhongChatId where b.NguoiDungId == nguoiDungId select new PhongChatViewModel() { PhongChatId = a.PhongChatId, TenPhongChat = a.TenPhongChat, TenVietTat = a.TenVietTat });
                if (queryable != null && queryable.Count() > 0) return queryable.ToList();
                return null;
            }
            catch
            {
                return null;
            }
        }

        public PhongChatInfoViewModel GetInfoCalendarBy(int phongChatId)
        {
            try
            {
                return (from a in db.PhongChatInfos where a.PhongChatId == phongChatId && a.Type == 1 && a.TenPhongChatInfo.ToUpper() == "LINKCALENDAR" select new PhongChatInfoViewModel { PhongChatInfoId = a.PhongChatInfoId, TenPhongChatInfo = a.TenPhongChatInfo, PhongChatId = a.PhongChatId, NoiDung = a.NoiDung, Type = a.Type, GhiChu = a.GhiChu }).FirstOrDefault();
            }
            catch
            {
                return null;
            }


        }
        public List<HoTenNguoiDungViewModel> GetHoTenNguoiDungsBy(int phongChatId)
        {
            try
            {
                return (from a in db.LienKetNguoiDungPhongChats join b in db.NguoiDungs on a.NguoiDungId equals b.NguoiDungId where a.PhongChatId == phongChatId select new HoTenNguoiDungViewModel() { HoTen = b.HoTen, NguoiDungId = b.NguoiDungId }).ToList();
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region InserOrUpdate
        public int InsertPhongChatInfo(int phongChatId, string TenPhongChat, string noiDung, string GhiChu, byte type, Guid nguoiDungId)
        {
            try
            {
                PhongChatInfo entity = new PhongChatInfo();
                entity.PhongChatId = phongChatId;
                entity.TenPhongChatInfo = TenPhongChat;
                entity.NoiDung = noiDung;
                entity.GhiChu = GhiChu;
                entity.Type = type;
                entity.NguoiCapNhat = nguoiDungId;
                entity.NgayCapNhat = DateTime.Now;
                db.PhongChatInfos.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.PhongChatInfoId;
            }
            catch
            {
                return 0;
            }
        }
        public int InsertPhongChat(string tenPhongChat, Guid nguoiTao, int? khoaChaId, byte type, string itemId, byte itemType, string tenVietTat)
        {
            try
            {
                if (db.PhongChats.Where(t => t.TenPhongChat.Equals(tenPhongChat)).Count() == 0 || string.IsNullOrEmpty(tenPhongChat))
                {
                    PhongChat entity = new PhongChat();
                    entity.TenPhongChat = tenPhongChat;
                    entity.ItemId = itemId;
                    entity.ItemType = itemType;
                    entity.TenVietTat = tenVietTat;
                    entity.KhoaChaId = khoaChaId;
                    entity.Type = type;
                    entity.NgayTao = DateTime.Now;
                    entity.NguoiTao = nguoiTao;
                    db.PhongChats.InsertOnSubmit(entity);
                    db.SubmitChanges();
                    return entity.PhongChatId;
                }
                else
                {
                    return db.PhongChats.Where(t => t.TenPhongChat.Equals(tenPhongChat)).FirstOrDefault().PhongChatId;
                }
            }
            catch
            {
                return 0;
            }
        }
        public bool UpdatePhongChat(int phongChatId, string tenPhongChat, string itemId, byte itemType, string tenVietTat)
        {
            try
            {
                var pc = db.PhongChats.Where(t => t.PhongChatId == phongChatId).FirstOrDefault();
                pc.TenPhongChat = tenPhongChat;
                pc.ItemId = itemId;
                pc.ItemType = itemType;
                pc.TenVietTat = tenPhongChat;
                db.SubmitChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool UpdatePhongChatInfo(int phongChatId, string tenPhongChat, string noiDung, string ghiChu)
        {
            try
            {
                var pc = db.PhongChatInfos.Where(t => t.PhongChatId == phongChatId && t.TenPhongChatInfo.ToUpper() == tenPhongChat.ToUpper()).FirstOrDefault();
                pc.NoiDung = noiDung;
                pc.GhiChu = ghiChu;
                db.SubmitChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool InsertLienKetPhongChat(int phongChatId, Guid nguoiDungId)
        {
            try
            {
                if (db.LienKetNguoiDungPhongChats.Where(t => t.PhongChatId == phongChatId && t.NguoiDungId == nguoiDungId).Count() == 0)
                {
                    LienKetNguoiDungPhongChat entity = new LienKetNguoiDungPhongChat();
                    entity.NguoiDungId = nguoiDungId;
                    entity.NgayTao = DateTime.Now;
                    entity.PhongChatId = phongChatId;
                    db.LienKetNguoiDungPhongChats.InsertOnSubmit(entity);
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertLienKetPhongChat(int phongChatId, List<Guid> nguoiDungId)
        {
            try
            {
                try
                {
                    var lst = db.LienKetNguoiDungPhongChats.Where(t => t.PhongChatId == phongChatId).ToList();
                    if (lst != null && lst.Count > 0) db.LienKetNguoiDungPhongChats.DeleteAllOnSubmit(lst);
                    db.SubmitChanges();
                }
                catch
                {

                }
                List<LienKetNguoiDungPhongChat> pcs = new List<LienKetNguoiDungPhongChat>();
                if (nguoiDungId != null && nguoiDungId.Count > 0)
                {
                    foreach (var item in nguoiDungId)
                    {
                        pcs.Add(new LienKetNguoiDungPhongChat() { PhongChatId = phongChatId, NguoiDungId = item, NgayTao = DateTime.Now });
                    }
                    db.LienKetNguoiDungPhongChats.InsertAllOnSubmit(pcs);
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Check
        public bool IsExistChannelName(string channelName, int channelId)
        {
            try
            {
                var count = (from a in db.PhongChats where a.TenPhongChat.ToUpper() == channelName.ToUpper().Trim() && (channelId == 0 || (channelId != 0 && a.PhongChatId != channelId)) select a.PhongChatId).Count();
                if (count > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Delete
        public bool DeleteLienKetNguoiDungPhongChat(int phongChatId,Guid nguoiDungId)
        {
            try
            {
                var lk = db.LienKetNguoiDungPhongChats.Where(t => t.NguoiDungId == nguoiDungId && t.PhongChatId == phongChatId).ToList();
                db.LienKetNguoiDungPhongChats.DeleteAllOnSubmit(lk);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion






    }
}