using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
   public class MessageProvider
    {
        TimerDataContext db = null;
        public MessageProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public Guid InsertMessage(Guid nguoiGui,string diaChiNhan,byte type,string noiDung,string title)
        {
            try
            {                
                Message entity = new Message();
                entity.MessageId = Guid.NewGuid();
                entity.DiaChiNhan = diaChiNhan;
                entity.NgayTao = DateTime.Now;
                entity.NguoiGuiId = nguoiGui;
                entity.Type = type;
                entity.NoiDung = noiDung;
                entity.Title = title;
                db.Messages.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.MessageId;
            }
            catch(Exception ex)
            {
                return Guid.Empty;
            }
        }

        public List<MessageViewModel> GetsBy(string diaChiGui,string diaChiNhan,byte type,int pageNumber,int pageSize)
        {
            try
            {
                var str = "";
                if (type == 1)
                {
                    str = "select m.MessageId,m.DiaChiNhan,m.Type,m.NgayTao,m.NoiDung,m.NguoiGuiId,n.HoTen,n.Avatar from Message as m inner join NguoiDung as n on m.NguoiGuiId = n.NguoiDungId where  (m.DiaChiNhan='" + diaChiNhan + "' and m.Type=1)  order by m.NgayTao desc offset " + (pageNumber - 1) * pageSize + " rows fetch next " + pageSize + " rows only";
                }
                else
                {
                     str = "select m.MessageId,m.DiaChiNhan,m.Type,m.NgayTao,m.NoiDung,m.NguoiGuiId,n.HoTen,n.Avatar from Message as m inner join NguoiDung as n on m.NguoiGuiId = n.NguoiDungId where ( m.DiaChiNhan in('" + diaChiGui + "','" + diaChiNhan + "') and m.NguoiGuiId in('" + diaChiGui + "','" + diaChiNhan + "') and m.Type=2)  order by m.NgayTao desc offset " + (pageNumber - 1) * pageSize + " rows fetch next " + pageSize + " rows only";
                }
               
              return  db.ExecuteQuery<MessageViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public List<MessageViewModel> GetsBy(int thread, int pageNumber, int pageSize)
        {
            try
            {
                var str = "";
                str = "select m.MessageId,m.DiaChiNhan,m.Type,m.NgayTao,m.NoiDung,m.NguoiGuiId,n.HoTen,n.Avatar from Message as m inner join NguoiDung as n on m.NguoiGuiId = n.NguoiDungId where ( m.DiaChiNhan ='" + thread + "' and m.Type=2)  order by m.NgayTao"; 
                if(pageNumber>0&&pageSize>0)
                str+= " offset " + (pageNumber - 1) * pageSize + " rows fetch next " + pageSize + " rows only"; 
                return db.ExecuteQuery<MessageViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertLienKetMassage(Guid messageId, List<Guid> nguoiDungIds)
        {
            try
            {
                List<LienKetMessage> entitys = new List<LienKetMessage>();
                foreach(var id in nguoiDungIds)
                {
                    LienKetMessage entity = new LienKetMessage { MessageId = messageId, NguoiNhanId = id, DaXem = false, Thich = false };
                    entitys.Add(entity);
                }
                db.LienKetMessages.InsertAllOnSubmit(entitys);
                db.SubmitChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
        
        public int CountUnReadPhongChatThread(Guid nguoiDungId,int PhongChatId)
        {
            try
            {
               return  (from m in db.Messages join l in db.LienKetMessages on m.MessageId equals l.MessageId where (from b in db.PhongChats where b.KhoaChaId == PhongChatId select b.PhongChatId.ToString()).Contains(m.DiaChiNhan) && m.Type == 2 &&l.NguoiNhanId==nguoiDungId&&l.DaXem==false select m.MessageId).Count();
               
            }
            catch
            {
                return 0;
            }
        }
        public bool DeleteMessage(Guid messageId)
        {
            //
            try {
                var entitys = db.LienKetMessages.Where(t => t.MessageId == messageId).ToList();
                db.LienKetMessages.DeleteAllOnSubmit(entitys);
                db.SubmitChanges();
            }
            catch
            {
            }
            try
            {
                var entity = db.Messages.Where(t => t.MessageId == messageId).FirstOrDefault();
                db.Messages.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public MessageViewModel GetById(Guid messageId)
        {
            return db.ExecuteQuery<MessageViewModel>("Select * from Message where MessageId='" + messageId.ToString() + "'").FirstOrDefault();
        }
    }
}
