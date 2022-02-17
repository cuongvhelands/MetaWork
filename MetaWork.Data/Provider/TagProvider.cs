using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class TagProvider
    {
        TimerDataContext db = null;
        public TagProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public int InsertTag(string tagName,bool isPublic,Guid nguoiTao)
        {
            try
            {
                Tag entity = new Tag();
                entity.TagName = tagName;
                entity.IsPublic = isPublic;
                entity.NgayTao = DateTime.Now;
                entity.NguoiTao = nguoiTao;
                db.Tags.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.TagId;
            }
            catch
            {
                return 0;
            }
        }
        public Guid InsertLienKetTag(int tagId, string itemId, byte itemType)
        {
            try
            {
                LienKetTag entity = new LienKetTag();
                entity.TagId = tagId;
                entity.ItemId = itemId;
                entity.itemType = itemType;
                db.LienKetTags.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.LienKetTagId;
            }
            catch
            {
                return Guid.Empty;
            }
        }
        public bool DeleteLienKetTag(int tagid,string itemId,byte itemType)
        {
            try
            {
                var qr = db.LienKetTags.Where(t => t.TagId == tagid && t.ItemId == itemId && t.itemType == itemType);
                if (qr.Count() > 0)
                {
                    db.LienKetTags.DeleteOnSubmit(qr.FirstOrDefault());
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
