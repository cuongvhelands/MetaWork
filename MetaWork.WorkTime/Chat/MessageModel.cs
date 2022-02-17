using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MetaWork.WorkTime.Chat
{
    public class MessageModel
    {
        public List<MessageViewModel> GetsBy(string diaChiGui, string diaChiNhan, byte type,int pageNumber, int pageSize)
        {
            MessageProvider manager = new MessageProvider();
            return manager.GetsBy( diaChiGui,diaChiNhan, type, pageNumber, pageSize);
        }
        public Guid Insert(Guid nguoiDungId,string diaChiNhan,byte type,string noiDung)
        {
            MessageProvider manager = new MessageProvider();
            return manager.InsertMessage(nguoiDungId, diaChiNhan, type, noiDung,"");
        }
        public bool AddLienKet(Guid messageId,List<Guid> nguoiDungIds)
        {
            MessageProvider manager = new MessageProvider();
            return manager.InsertLienKetMassage(messageId, nguoiDungIds);
        }

        
    }
}