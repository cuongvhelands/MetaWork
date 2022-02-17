using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class NoiDungProvider
    {
        TimerDataContext db = null;
        public NoiDungProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public Guid Insert(string itemId,byte itemType, byte loaiNoiDungId, string noiDungChiTiet,Guid nguoiDungId)
        {
            try
            {
                NoiDung entity = new NoiDung();
                entity.ItemId = itemId;
                entity.ItemType = itemType;
                entity.LoaiNoiDungId = loaiNoiDungId;
                entity.NgayCapNhat = DateTime.Now;
                entity.NgayTao = DateTime.Now;
                entity.NguoiDungId = nguoiDungId;
                entity.NoiDungChiTiet = noiDungChiTiet;
                entity.TrangThai = true;
                entity.NoiDungId = Guid.NewGuid();
                db.NoiDungs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.NoiDungId;
            }catch(Exception ex) {
                return Guid.Empty;
                 }
           
        }
        public bool Edit(Guid noiDungId,string noiDungChiTiet,Guid nguoiDungId)
        {
            try
            {
                var entity = db.NoiDungs.Where(t => t.NoiDungId == noiDungId && t.NguoiDungId == nguoiDungId).FirstOrDefault();
                entity.NoiDungChiTiet = noiDungChiTiet;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
     
        public List<NoiDungViewModel> GetsBy(string itemId, byte loaiNoiDungId,byte itemType)
        {
            try
            {
                var str = "Select nd.NoiDungId,nd.LoaiNoiDungId,nd.NgayCapNhat,nd.NgayTao,nd.NguoiDungId,nd.NoiDungChiTiet,nd.TrangThai,nd.ItemId,nd.ItemType,n.HoTen,n.Avatar from NoiDung as nd inner join nguoiDung as n on nd.NguoiDungId = n.NguoiDungId where nd.itemId='"+itemId+"' and nd.LoaiNoiDungId="+loaiNoiDungId+" and nd.ItemType="+itemType+" order by nd.NgayTao desc";
                return db.ExecuteQuery<NoiDungViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public NoiDungViewModel GetById(Guid noiDungId,Guid nguoiDungId)
        {
            try
            {
                var str = "Select nd.NoiDungId,nd.LoaiNoiDungId,nd.NgayCapNhat,nd.NgayTao,nd.NguoiDungId,nd.NoiDungChiTiet,nd.TrangThai,nd.ItemId,nd.ItemType,n.HoTen,n.Avatar from NoiDung as nd inner join nguoiDung as n on nd.NguoiDungId = n.NguoiDungId where nd.NoiDungId='" + noiDungId.ToString() + "' and nd.NguoiDungId='" + nguoiDungId.ToString() + "'";
                return db.ExecuteQuery<NoiDungViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public bool Delete(Guid noiDungId, Guid nguoiDungId)
        {
            try
            {
                var entity = db.NoiDungs.Where(t => t.NoiDungId == noiDungId && t.NguoiDungId == nguoiDungId).FirstOrDefault();
                db.NoiDungs.DeleteOnSubmit(entity);
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
