using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class DiemPointProvider
    {
        TimerDataContext db = null;
        public DiemPointProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public bool InsertDiemPointByShipableId(int shipAbleId,bool done)
        {
            CongViecProvider congViecM = new CongViecProvider();
            try
            {
                var lst = db.CongViecs.Where(t => t.KhoaChaId == shipAbleId && t.LaShipAble == false).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach(var item in lst)
                    {
                        // Xóa điểm point cũ nếu có của công việc này
                        try
                        {
                            var lstDiem = db.DiemPoints.Where(t => t.CongViecId == item.CongViecId).ToList();
                            if (lstDiem != null && lstDiem.Count > 0)
                            {
                                foreach(var item2 in lstDiem)
                                {
                                    var lstDiemChiTiet = db.DiemPointChiTiets.Where(t => t.DiemPointId == item2.DiemPointId).ToList();
                                    db.DiemPointChiTiets.DeleteAllOnSubmit(lstDiemChiTiet);
                                    db.SubmitChanges();
                                }
                            }
                            db.DiemPoints.DeleteAllOnSubmit(lstDiem);
                            db.SubmitChanges();
                        }
                        catch
                        {

                        }
                        //Thêm điểm point mới cho shipableId này.
                        DiemPoint entity = new DiemPoint();
                        entity.CongViecId = item.CongViecId;
                        entity.NguoiDungId = item.NguoiXuLyId.Value;
                        entity.NgayCapNhat = DateTime.Now;
                        db.DiemPoints.InsertOnSubmit(entity);
                        db.SubmitChanges();
                        DiemPointChiTiet entity2 = new DiemPointChiTiet();
                        entity2.DiemPointId = entity.DiemPointId;
                        if (done&&item.TrangThaiCongViecId==(int)EnumTrangThaiCongViecType.congViecDone) entity2.ChamDiem = (int)item.DiemPoint.Value;
                        else entity2.ChamDiem= -(int)item.DiemPoint.Value;                     
                        entity2.LoaiPoint = item.LoaiPoint.Value;
                        entity2.NgayCapNhat = DateTime.Now;
                        db.DiemPointChiTiets.InsertOnSubmit(entity2);
                        db.SubmitChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetPointOfUser(Guid userId)
        {
            try
            {
                var str = "select sum(ct.ChamDiem) from DiemPoint as d inner join DiemPointChiTiet as ct on d.DiemPointId=ct.DiemPointId where d.NguoiDungId='" + userId.ToString() + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public int GetPointOfShipable(int shipable)
        {
            try
            {
                var str = "select sum(ct.ChamDiem) from DiemPoint as d inner join DiemPointChiTiet as ct on d.DiemPointId=ct.DiemPointId inner join CongViec as c on d.CongViecId=c.CongViecId where c.LaShipAble=0 and c.KhoaChaId=" + shipable;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
