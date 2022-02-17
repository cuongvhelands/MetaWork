using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class CongViecProvider
    {
        TimerDataContext db = null;
        public CongViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }


        #region TaskOutLine

        public List<CongViecViewModel> GetAllTaskInDuAn(int duAnId)
        {
            try
            {

                return (from a in db.CongViecs
                        join n in db.NguoiDungs on a.NguoiXuLyId equals n.NguoiDungId into an
                        from x in an.DefaultIfEmpty()
                        where a.DuAnId == duAnId
                        select new CongViecViewModel()
                        {
                            CongViecId = a.CongViecId,
                            KhoaChaId = a.KhoaChaId,
                            HoTen = x.HoTen ?? String.Empty,
                            NguoiXuLyId = a.NguoiXuLyId,
                            NgayDuKienHoanThanh = a.NgayDuKienHoanThanh,
                            QuanTam=a.Flag??false,
                            Tags = GetTagsBy(a.CongViecId.ToString(),(byte)EnumTagType.Task),
                            ThuTuSapXep=a.ThuTuSapXep
                        }).ToList();
            }
            catch
            {
                return null;
            }
        }

        private List<TagViewModel> GetTagsBy( string itemId, byte itemType) 
        {
            try
            {
                return (from b in db.Tags join l in db.LienKetTags on b.TagId equals l.TagId where  l.ItemId == itemId && l.itemType == itemType select new TagViewModel() { TagId = l.TagId, TenTag = b.TagName, ItemId = l.ItemId, ItemType = l.itemType ?? 0 }).ToList();
            }
            catch
            {
                return null;
            }            
        }


        public int InsertCongViecOutline(int duAnId, int? khoaChaId, Guid nguoiDungId, int tuan, int nam)
        {
            try
            {
                CongViec entity = new CongViec();
                entity.DuAnId = duAnId;
                entity.KhoaChaId = khoaChaId;
                entity.NgayTao = DateTime.Now;
                entity.NguoiTaoId = nguoiDungId;
                entity.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.congviecNew;
                entity.Tuan = tuan;
                entity.Nam = nam;
                entity.IsToDoAdd = false;
                entity.LaTaskOutline = true;
                db.CongViecs.InsertOnSubmit(entity);
                return entity.CongViecId;
            }
            catch
            {
                return 0;
            }
        }
        public bool UpdateTenCongViec(int congViecId, string tenCongViec)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.TenCongViec = tenCongViec;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateQuanTrong(int congViecId, bool isQuanTrong)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.Flag = isQuanTrong;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool UpdateThoiGianUocLuong(int congViecId, int time)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.ThoiGianUocLuong = time;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateKhoaCha(int congViecId, int? khoaChaId)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.KhoaChaId = khoaChaId;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateDuKienHoanThanh(int congViecId, DateTime ngayDuKienHoanThanh)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.NgayDuKienHoanThanh = ngayDuKienHoanThanh;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateNote(int congViecId, string note)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.MoTa = note;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        #region method Get

        public List<CongViecViewModel> GetCongViecWorkingInTimeBy(Guid nguoiDungId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var str = "select c.CongViecId, c.TenCongViec,c.KhoaChaId,s.TenCongViec as TenKhoaCha,s.DuAnId, d.TenDuAn from CongViec as c inner join CongViec as s on c.KhoaChaId = s.CongViecId inner join DuAn as d on s.DuAnId=d.DuAnId  where c.CongViecId in(select task.CongViecId from CongViec as task inner join CongViec as todo on task.CongViecId = todo.KhoaChaId inner join ThoiGianLamViec as t on todo.CongViecId = t.CongViecId where task.LaTask=1 and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.NgayLamViec>='" + startTime.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endTime.ToString("yyyy-MM-dd") + "' and task.TrangThaiCongViecId !=" + (int)EnumTrangThaiCongViecType.congViecDone + " group by task.CongViecId)";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<int> GetShipIdInActiveBy(DateTime startTime, DateTime endDate)
        {
            try
            {
                var str = " select s.CongViecId from DuAn as d inner join DuAn as dtp on d.DuAnId = dtp.KhoaChaId inner join LienKetDuAnPhongBan as lk on d.DuAnId = lk.DuAnId inner join CongViec as s on dtp.DuAnId=s.DuAnId where (s.NgayTao>='" + startTime.ToString("yyyy-MM-dd") + " 00:00:00.000' and s.NgayTao<='" + endDate.ToString("yyyy-MM-dd") + " 00:00:00.000' and s.NgayDuKienHoanThanh>='" + startTime.ToString("yyyy-MM-dd") + " 00:00:00.000') and s.TrangThaiCongViecId in(1,2) and s.LaShipAble=1 and s.CongViecId not in (select s.CongViecId from DuAn as d inner join DuAn as dtp on d.DuAnId = dtp.KhoaChaId inner join LienKetDuAnPhongBan as lk on d.DuAnId = lk.DuAnId inner join CongViec as s on dtp.DuAnId=s.DuAnId inner join CongViec as task on s.CongViecId=task.KhoaChaId inner join CongViec as todo on task.CongViecId = todo.KhoaChaId inner join ThoiGianLamViec as t on todo.CongViecId=t.CongViecId where (s.NgayTao>='" + startTime.ToString("yyyy-MM-dd") + " 00:00:00.000' and s.NgayTao<='" + endDate.ToString("yyyy-MM-dd") + " 00:00:00.000' and s.NgayDuKienHoanThanh>='" + startTime.ToString("yyyy-MM-dd") + " 00:00:00.000') and s.TrangThaiCongViecId in(1,2) and s.LaShipAble=1  group by s.CongViecId)   group by s.CongViecId";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch
            {
                return null;
            }


        }



        public int GetTongSoTaskOfShip(int shipId)
        {
            try
            {
                var str = "select count(task.CongViecId) from CongViec as ship inner join CongViec as task on ship.CongViecId = task.KhoaChaId where ship.LaShipAble=1 and task.LaTask=1 and ship.CongViecId=" + shipId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int GetTongSoTaskOfShipTheoTrangThai(int shipId, int trangThaiCongViecId)
        {
            try
            {
                var str = "select count(ship.CongViecId) from CongViec as ship inner join CongViec as task on ship.CongViecId = task.KhoaChaId where ship.LaShipAble=1 and task.LaTask=1 and ship.CongViecId=" + shipId + " and task.TrangThaiCongViecId=" + trangThaiCongViecId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public bool CheckIsExist(int CongViecId, string TenCongViec, int DuAnId)
        {
            try
            {
                var i = db.CongViecs.Where(t => t.CongViecId == CongViecId && t.TenCongViec.ToUpper() == TenCongViec.ToUpper() && t.DuAnId == DuAnId).Count();
                if (i > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }

        }

        public int GetShipAbleByTen(string tenCongViec, int duAnId)
        {
            try
            {
                return (from c in db.CongViecs where c.DuAnId == duAnId && c.LaShipAble == true && c.TenCongViec.Trim().ToUpper() == tenCongViec.Trim().ToUpper() select c.CongViecId).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public CongViecViewModel GetShipAbleByTen2(string tenCongViec, int duAnId)
        {
            try
            {
                return (from c in db.CongViecs where c.DuAnId == duAnId && c.LaShipAble == true && c.TenCongViec.Trim().ToUpper() == tenCongViec.Trim().ToUpper() select new CongViecViewModel() { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, KhoaChaId = c.KhoaChaId, DuAnId = c.DuAnId, NguoiXuLyId = c.NguoiXuLyId }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public int GetTaskByTen(string tenCongViec, int khoaChaId, int duAnId)
        {
            try
            {
                return (from c in db.CongViecs where c.DuAnId == duAnId && c.LaShipAble == false && c.LaTask == true && c.TenCongViec == tenCongViec && c.KhoaChaId == khoaChaId select c.CongViecId).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public bool UpdateNguoiXuLyId(int congViecId, Guid nguoiXuLyId)
        {
            try
            {
                var cv = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                if (cv != null && cv.CongViecId > 0)
                {
                    cv.NguoiXuLyId = nguoiXuLyId;
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

        public bool CheckIsExist(string TenCongViec, int duAnId)
        {
            try
            {
                var i = db.CongViecs.Where(t => t.TenCongViec.ToUpper() == TenCongViec.ToUpper() && t.LaShipAble == true && t.DuAnId == duAnId).Count();
                if (i > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Lấy danh sách id todo công việc theo khoảng time ( dùng cho lấy danh sách id)
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<int> GetCongViecIdsInTime(Guid nguoiDungId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select task.CongViecId from CongViec as task inner join CongViec as todo on task.CongViecId = todo.KhoaChaId inner join ThoiGianLamViec as t on todo.CongViecId = t.CongViecId where t.NguoiDungId='" + nguoiDungId.ToString() + "' and (t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + " 00:00:00.000' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + " 00:00:00.000') group by task.CongViecId";
                return db.ExecuteQuery<int>(str).ToList();
                //return (from task in db.CongViecs join todo in db.CongViecs on task.CongViecId equals todo.KhoaChaId join t in db.ThoiGianLamViecs on todo.CongViecId equals t.CongViecId into ct from t in ct.DefaultIfEmpty() where (((todo.NgayTao >= startDate && todo.NgayTao <= endDate) || (t.NgayLamViec >= startDate && t.NgayLamViec <= endDate)) && ( t.NguoiDungId == nguoiDungId)) select task.CongViecId).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }
        public List<CongViecViewModel> GetShipInTime(List<Guid> nguoiDungIds, Guid nguoiDungId, List<int> duAnIds, int trangThaiCongViecId, bool quanTam, string tenShipable, DateTime startDate, DateTime endDate)
        {
            try
            {

                if (!quanTam)
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId
                            where (duAnIds == null || duAnIds.Count == 0 || (duAnIds.Contains(c.DuAnId)))
                            && (trangThaiCongViecId == 0 || (trangThaiCongViecId > 0 && c.TrangThaiCongViecId == trangThaiCongViecId))
                            && (string.IsNullOrEmpty(tenShipable) || (!string.IsNullOrEmpty(tenShipable) && c.TenCongViec.Contains(tenShipable)))
                            && c.LaShipAble == true
                            && nguoiDungIds.Contains(c.NguoiXuLyId.Value)
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            select new CongViecViewModel { TenCongViec = c.TenCongViec, CongViecId = c.CongViecId, TrangThaiCongViecId = c.TrangThaiCongViecId, NgayTao = c.NgayTao, NgayDuKienHoanThanh = c.NgayDuKienHoanThanh, TenTrangThai = tt.TenTrangThai, MaMauTrangThaiCongViec = tt.MaMau, NguoiXuLyId = c.NguoiXuLyId, HoTenNguoiXuLy = n.HoTen, AvatarNguoiXuLy = n.Avatar, DuAnId = c.DuAnId }).ToList();
                }
                else
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId
                            join lk in db.LienKetNguoiDungCongViecs on c.CongViecId equals lk.CongViecId
                            where (duAnIds == null || duAnIds.Count == 0 || (duAnIds.Contains(c.DuAnId)))
                            && (trangThaiCongViecId == 0 || (trangThaiCongViecId > 0 && c.TrangThaiCongViecId == trangThaiCongViecId))
                            && (lk.QuanTam == quanTam && lk.NguoiDungId == nguoiDungId)
                            && (string.IsNullOrEmpty(tenShipable) || (!string.IsNullOrEmpty(tenShipable) && c.TenCongViec.Contains(tenShipable)))
                            && c.LaShipAble == true
                            && nguoiDungIds.Contains(c.NguoiXuLyId.Value)
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            select new CongViecViewModel { TenCongViec = c.TenCongViec, CongViecId = c.CongViecId, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = tt.TenTrangThai, MaMauTrangThaiCongViec = tt.MaMau, NgayDuKienHoanThanh = c.NgayDuKienHoanThanh, NgayTao = c.NgayTao, NguoiXuLyId = c.NguoiXuLyId, HoTenNguoiXuLy = n.HoTen, AvatarNguoiXuLy = n.Avatar, DuAnId = c.DuAnId }).ToList();
                }



            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<int> GetShipIdsBy(List<Guid> nguoiDungIds, Guid nguoiDungId, List<int> duAnIds, List<int> trangThaiDuAnIds, int trangThaiCongViecId, bool quanTam, string tenShipable, DateTime startDate, DateTime endDate)
        {
            try
            {

                if (!quanTam)
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join d in db.DuAns on c.DuAnId equals d.DuAnId
                            where (duAnIds == null || duAnIds.Count == 0 || duAnIds.Contains(d.KhoaChaId ?? 0))
                            && (trangThaiCongViecId == 0 || (trangThaiCongViecId > 0 && c.TrangThaiCongViecId == trangThaiCongViecId))
                            && (string.IsNullOrEmpty(tenShipable) || (!string.IsNullOrEmpty(tenShipable) && c.TenCongViec.Contains(tenShipable)))
                            && c.LaShipAble == true
                            && (nguoiDungIds.Contains(c.NguoiXuLyId.Value) || nguoiDungIds.Contains(c.NguoiTaoId))
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            select c.CongViecId).ToList();
                }
                else
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join lk in db.LienKetNguoiDungCongViecs on c.CongViecId equals lk.CongViecId
                            join d in db.DuAns on c.DuAnId equals d.DuAnId
                            where (duAnIds == null || duAnIds.Count == 0 || (duAnIds.Contains(d.KhoaChaId ?? 0)))
                            && (trangThaiCongViecId == 0 || (trangThaiCongViecId > 0 && c.TrangThaiCongViecId == trangThaiCongViecId))
                            && (lk.QuanTam == quanTam && lk.NguoiDungId == nguoiDungId)
                            && (string.IsNullOrEmpty(tenShipable) || (!string.IsNullOrEmpty(tenShipable) && c.TenCongViec.Contains(tenShipable)))
                            && c.LaShipAble == true
                            && nguoiDungIds.Contains(c.NguoiXuLyId.Value)
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            select c.CongViecId).ToList();
                }



            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CongViecViewModel GetInfoById(int congViecId)
        {
            return (from c in db.CongViecs
                    join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId into gj
                    from x in gj.DefaultIfEmpty()
                    join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId
                    where c.CongViecId == congViecId
                    select new CongViecViewModel
                    {
                        CongViecId = c.CongViecId,
                        TenCongViec = c.TenCongViec,
                        TrangThaiCongViecId = c.TrangThaiCongViecId,
                        TenTrangThai = t.TenTrangThai,
                        MaMau = t.MaMau,
                        MaMauTrangThaiCongViec = t.MaMau,
                        HoTenNguoiXuLy = x.HoTen ?? String.Empty
                        ,
                        NguoiXuLyId = c.NguoiXuLyId,
                        AvatarNguoiXuLy = x.Avatar ?? String.Empty,
                        ThuTuUuTien = c.ThuTuUuTien,
                        NgayCapNhat = c.NgayCapNhat,
                        NgayTao = c.NgayTao,
                        DuAnId = c.DuAnId,
                        KhoaChaId = c.KhoaChaId
                    }).FirstOrDefault();
        }

        public CongViecViewModel GetDetailShipById(int congViecId)
        {
            return (from c in db.CongViecs
                    join d in db.DuAns on c.DuAnId equals d.DuAnId
                    join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId
                    join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId into gj
                    from x in gj.DefaultIfEmpty()
                    where c.LaShipAble == true && c.CongViecId == congViecId
                    select new CongViecViewModel
                    {
                        CongViecId = c.CongViecId,
                        DuAnId = c.DuAnId,
                        TenDuAn = d.TenDuAn,
                        TenCongViec = c.TenCongViec,
                        GiaiDoanDuAnId = c.GiaiDoanDuAnId,
                        NgayDuKienHoanThanh = c.NgayDuKienHoanThanh,
                        Tuan = c.Tuan,
                        MoTa = c.MoTa,
                        TrangThaiCongViecId = c.TrangThaiCongViecId,
                        TenTrangThai = t.TenTrangThai,
                        MaMauTrangThaiCongViec = t.MaMau,
                        HoTenNguoiXuLy = x.HoTen ?? String.Empty
                        ,
                        NguoiXuLyId = c.NguoiXuLyId,
                        AvatarNguoiXuLy = x.Avatar ?? String.Empty,
                        NgayTao = c.NgayTao
                    }
                    ).FirstOrDefault();
        }

        public CongViecViewModel GetDetailTaskById(int congViecId)
        {
            return (from c in db.CongViecs
                    join d in db.DuAns on c.DuAnId equals d.DuAnId
                    join s in db.CongViecs on c.KhoaChaId equals s.CongViecId
                    where c.LaTask == true && c.CongViecId == congViecId
                    select new CongViecViewModel
                    {
                        CongViecId = c.CongViecId,
                        DuAnId = c.DuAnId,
                        TenDuAn = d.TenDuAn,
                        TenCongViec = c.TenCongViec,
                        GiaiDoanDuAnId = c.GiaiDoanDuAnId,
                        NgayDuKienHoanThanh = c.NgayDuKienHoanThanh,
                        Tuan = c.Tuan,
                        MoTa = c.MoTa,
                        KhoaChaId = c.KhoaChaId,
                        TenKhoaCha = s.TenCongViec,
                    }
                    ).FirstOrDefault();
        }

        public List<CongViecViewModel> GetTaskInTimeBy(List<int> duAnIds, List<int> trangThaiDuAnIds, List<Guid> nguoiDungIds, Guid nguoiDungId, bool quanTam, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (quanTam == true)
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join s in db.CongViecs on c.KhoaChaId equals s.CongViecId
                            join d in db.DuAns on s.DuAnId equals d.DuAnId
                            join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId
                            join lk in db.LienKetNguoiDungCongViecs on c.CongViecId equals lk.CongViecId
                            where (c.LaTask == true && c.LaShipAble == false)
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            && (duAnIds == null || duAnIds.Count == 0 || duAnIds.Contains(d.KhoaChaId ?? 0))
                            && (lk.QuanTam == quanTam && lk.NguoiDungId == nguoiDungId)
                            && (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(n.NguoiDungId))
                            && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            select new CongViecViewModel { TenCongViec = c.TenCongViec, CongViecId = c.CongViecId, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = tt.TenTrangThai, MaMauTrangThaiCongViec = tt.MaMau, NgayTao = c.NgayTao, NgayDuKienHoanThanh = c.NgayDuKienHoanThanh, MaMau = tt.MaMau, NguoiXuLyId = c.NguoiXuLyId, HoTenNguoiXuLy = n.HoTen, AvatarNguoiXuLy = n.Avatar, KhoaChaId = c.KhoaChaId, DuAnId = c.DuAnId }).ToList();
                }
                else
                {
                    return (from c in db.CongViecs
                            join tt in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals tt.TrangThaiCongViecId
                            join s in db.CongViecs on c.KhoaChaId equals s.CongViecId
                            join d in db.DuAns on s.DuAnId equals d.DuAnId
                            join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId
                            where (c.LaTask == true && c.LaShipAble == false)
                            && (c.NgayTao >= startDate && c.NgayTao <= endDate)
                            && (duAnIds == null || duAnIds.Count == 0 || duAnIds.Contains(d.KhoaChaId ?? 0))
                            && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            && (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(c.NguoiXuLyId.Value))
                            select new CongViecViewModel { TenCongViec = c.TenCongViec, CongViecId = c.CongViecId, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = tt.TenTrangThai, MaMauTrangThaiCongViec = tt.MaMau, NgayTao = c.NgayTao, NgayDuKienHoanThanh = c.NgayDuKienHoanThanh, MaMau = tt.MaMau, NguoiXuLyId = c.NguoiXuLyId, HoTenNguoiXuLy = n.HoTen, AvatarNguoiXuLy = n.Avatar, KhoaChaId = c.KhoaChaId, DuAnId = c.DuAnId }).ToList();
                }




            }
            catch
            {
                return null;
            }
        }

        public List<int> GetTaskIdRunTimeBy(List<int> duAnIds, List<int> trangThaiDuAnIds, List<Guid> nguoiDungIds, bool quanTam, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!quanTam)
                {
                    return (from task in db.CongViecs
                            join d in db.DuAns on task.DuAnId equals d.DuAnId
                            join todo in db.CongViecs on task.CongViecId equals todo.KhoaChaId
                            join time in db.ThoiGianLamViecs on todo.CongViecId equals time.CongViecId
                            where (duAnIds == null || duAnIds.Count == 0 || duAnIds.Contains(task.DuAnId))
                            && (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(time.NguoiDungId.Value))
                            && ((task.NgayTao >= startDate && task.NgayTao <= endDate) || (task.NgayDuKienHoanThanh >= startDate && task.NgayDuKienHoanThanh <= endDate))
                             && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            select task.CongViecId
                            ).GroupBy(t => t).Select(grp => grp.Key)
                           .ToList();
                }
                else
                {
                    return (from task in db.CongViecs
                            join d in db.DuAns on task.DuAnId equals d.DuAnId
                            join todo in db.CongViecs on task.CongViecId equals todo.KhoaChaId
                            join time in db.ThoiGianLamViecs on todo.CongViecId equals time.CongViecId
                            join lk in db.LienKetNguoiDungCongViecs on task.CongViecId equals lk.CongViecId
                            where (duAnIds == null || duAnIds.Count == 0 || duAnIds.Contains(task.DuAnId))
                            && (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(time.NguoiDungId.Value))
                            && (lk.QuanTam == quanTam && nguoiDungIds.Contains(lk.NguoiDungId))
                            && ((task.NgayTao >= startDate && task.NgayTao <= endDate) || (task.NgayDuKienHoanThanh >= startDate && task.NgayDuKienHoanThanh <= endDate))
                             && (trangThaiDuAnIds == null || trangThaiDuAnIds.Count == 0 || trangThaiDuAnIds.Contains(d.TrangThaiDuAnId))
                            select task.CongViecId
                           ).GroupBy(t => t).Select(grp => grp.Key)
                           .ToList();
                }
            }
            catch
            {
                return null;
            }
        }



        public TenCongViecViewModel GetTenCongViecById(int congViecId)
        {
            try
            {
                return (from c in db.CongViecs join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId where c.CongViecId == congViecId select new TenCongViecViewModel { TenCongViec = c.TenCongViec, TenTrangThai = t.TenTrangThai }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public bool getQuanTam(int congViecId, Guid nguoiDungId)
        {
            try
            {
                var entity = db.LienKetNguoiDungCongViecs.Where(t => t.CongViecId == congViecId && t.NguoiDungId == nguoiDungId).FirstOrDefault();
                if (entity != null) return entity.QuanTam;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public List<CongViecViewModel> GetTenShipableByDuAnId(int duAnId)
        {
            try
            {
                return (from c in db.CongViecs where c.DuAnId == duAnId select new CongViecViewModel { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, TrangThaiCongViecId = c.TrangThaiCongViecId }).ToList();

            }
            catch
            {
                return null;
            }

        }
        public List<CongViecViewModel> GetTenShipableByDuAnId2(int duAnId)
        {
            try
            {
                return (from c in db.CongViecs
                        join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId
                        where c.DuAnId == duAnId && c.LaShipAble == true
                        select new CongViecViewModel { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = t.TenTrangThai, MaMauTrangThaiCongViec = t.MaMau }).ToList();

            }
            catch
            {
                return null;
            }

        }

        public List<CongViecViewModel> GetTodoByV2(List<Guid> nguoidungIds, DateTime start, DateTime end)
        {
            try
            {
                return (from c in db.CongViecs join d in db.DuAns on c.DuAnId equals d.DuAnId join ts in db.CongViecs on c.KhoaChaId equals ts.CongViecId join s in db.CongViecs on ts.KhoaChaId equals s.CongViecId join n in db.NguoiDungs on c.NguoiXuLyId equals n.NguoiDungId join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId where c.LaShipAble == false && c.LaTask == false && (nguoidungIds == null || nguoidungIds.Count == 0 || nguoidungIds.Contains(c.NguoiXuLyId.Value)) && start <= c.NgayTao && end >= c.NgayTao select new CongViecViewModel { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, DuAnId = c.DuAnId, TenDuAn = d.TenDuAn, IsToDoAdd = c.IsToDoAdd, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = t.TenTrangThai, TenKhoaCha = ts.TenCongViec, TenShipable = s.TenCongViec, MaMauTrangThaiCongViec = t.MaMau, Tuan = c.Tuan, Nam = c.Nam, HoTenNguoiXuLy = n.HoTen, AvatarNguoiXuLy = n.Avatar, NguoiXuLyId = c.NguoiXuLyId, NgayTao = c.NgayTao, NgayDuKienHoanThanh = c.NgayDuKienHoanThanh }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public CongViecViewModel GetTimeFromCongViecId(int congviecId)
        {
            try
            {
                var vm = new CongViecViewModel();
                var str = "select sum(tongThoiGian) as TongThoiGian, MAX(ThoiGianKetThuc) as ThoiGianKetThuc, MIN(thoigianBatDau) as ThoiGianBatDau from ThoiGianLamViec  where CongViecId=" + congviecId;
                return db.ExecuteQuery<CongViecViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }


        #region Shipable
        public List<CongViecViewModel> GetShipablesBy(int duAnId, int giaiDoanDuAnId, int weekStart, int weekEnd, DateTime startTime, DateTime endTime)
        {
            try
            {
                //var str = "select * from CongViec where DuAnId="+duAnId+" and GiaiDoanDuAnId="+giaiDoanDuAnId+" and LaShipAble=1 and( (Tuan>="+ weekStart + " and Tuan<=" + weekEnd + ") or (TuanHoanThanh>=" + weekStart + " and TuanHoanThanh<=" + weekStart + ") or (NgayDuKienHoanThanh>='" + startTime.ToString("yyyy-MM-dd hh:mm:ss.fff")+ "'and NgayDuKienHoanThanh<='" + endTime.ToString("yyyy-MM-dd hh:mm:ss.fff") + "'))";
                var str = "select * from CongViec where DuAnId=" + duAnId + " and GiaiDoanDuAnId=" + giaiDoanDuAnId + " and LaShipAble=1";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetShipablesByGiaiDoan(int duAnId, int giaiDoanDuAnId)
        {
            try
            {

                var str = "select c.CongViecId,c.NguoiXuLyId,c.Nam,c.TenCongViec,c.TrangThaiCongViecId,c.NgayTao,c.Tuan,c.TuanHoanThanh,c.NgayDuKienHoanThanh,n.HoTen,n.Avatar as AvatarNguoiXuLy,t.TenTrangThai,t.MaMau from CongViec as c inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where c.DuAnId=" + duAnId + " and c.GiaiDoanDuAnId=" + giaiDoanDuAnId + " and c.LaShipAble=1";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách shipable ( tên shipable) mà có công việc được gắn với người dùng trong tuần( phục vụ cho api lấy danh sách shipable)
        /// </summary>
        /// <param name="week"></param>
        /// <param name="duAnId"></param>
        /// <param name="nguoiDungId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetShipables(Guid nguoiDungId, int week, int nam, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select TenCongViec, CongViecId from CongViec where CongViecId in (select KhoaChaId from CongViec where LaTask=1 and (NguoiXuLyId='" + nguoiDungId.ToString() + "' or NguoiHoTroId='" + nguoiDungId.ToString() + "') and ((Tuan = " + week + " and Nam =" + nam + ") or(NgayDuKienHoanThanh>= '" + startDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and NgayDuKienHoanThanh <='" + endDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')) group by KhoaChaId)";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int GetShipableIdByName(string tenShip, int duAnId)
        {
            try
            {
                return db.CongViecs.Where(t => t.TenCongViec.ToUpper() == tenShip.ToUpper() && t.LaShipAble == true && t.DuAnId == duAnId).Select(t => t.CongViecId).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int GetTaskIdByName(string tenTask, int duAnId)
        {
            try
            {
                return db.CongViecs.Where(t => t.TenCongViec.ToUpper() == tenTask.ToUpper() && t.LaShipAble == false && t.LaTask == true && t.DuAnId == duAnId).Select(t => t.CongViecId).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Lấy danh sách todo ( tên todo) theo ship và người dùng
        /// </summary>
        /// <param name="week"></param>
        /// <param name="duAnId"></param>
        /// <param name="nguoiDungId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetTodosBy(Guid nguoiDungId, int shipableId, int week, int nam, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select TenCongViec, CongViecId from CongViec where KhoaChaId = " + shipableId + " and LaTask=1 and (NguoiXuLyId='" + nguoiDungId.ToString() + "' or NguoiHoTroId='" + nguoiDungId.ToString() + "') and ((Tuan = " + week + " and Nam =" + nam + ") or(NgayDuKienHoanThanh>= '" + startDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and NgayDuKienHoanThanh <='" + endDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')) ";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Lấy danh sách todo ( tên todo) theo ship và người dùng
        /// </summary>
        /// <param name="week"></param>
        /// <param name="duAnId"></param>
        /// <param name="nguoiDungId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        //public List<CongViecViewModel> GetTodosByV2(List<Guid> nguoiDungIds, DateTime startDate, DateTime endDate)
        //{
        //    try
        //    {
        //        return (from c in db.QryToDos
        //                where (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(c.NguoiXuLyId.Value) && ((c.IsToDoAdd && c.NgayTao >= startDate && c.NgayTao <= endDate) || (c.ThoiGianLamViecId > 0 && !c.IsToDoAdd && c.NgayLamViec >= startDate && c.NgayLamViec <= endDate)))
        //                select new CongViecViewModel
        //                {
        //                    CongViecId = c.CongViecId,
        //                    TenCongViec = c.TenCongViec,
        //                    TenDuAn = c.TenDuAn,
        //                    NgayLamViec = c.NgayLamViec ?? c.NgayTao,
        //                    NgayTao = c.NgayTao,
        //                    NguoiXuLyId = c.NguoiXuLyId,
        //                    HoTenNguoiXuLy = c.HoTenNguoiXuLy,
        //                    AvatarNguoiXuLy = c.AvatarNguoiXuLy,
        //                    IsToDoAdd = c.IsToDoAdd
        //                }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public List<CongViecViewModel> GetTodosByV2(int congViecId,DateTime ngayLamViec)
        //{

        //}


        /// <summary>
        /// Lấy danh sách task theo shipable(khoaChaId) tuần làm việc (week) dự án(duAnId) người dùng (nguoiDungId) theo tên task(keyword)
        /// </summary>
        /// <param name="week"></param>
        /// <param name="duAnId"></param>
        /// <param name="nguoiDungId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetTasksBy(int khoaChaId, int week, int duAnId, Guid nguoiDungId, string keyword, int nam, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select c.CongViecId,c.ThoiGianUocLuong,c.ThuTuUuTien, c.TenCongViec, c.MaCongViec, nxl.HoTen as HoTenNguoiXuLy, nxl.Avatar as AvatarNguoiXuLy, nht.HoTen as HoTenNguoiHotro,c.NguoiXuLyId,c.NguoiHoTroId, nht.Avatar as AvatarNguoiHotro,tt.MaMau as MaMauTrangThaiCongViec,c.XacNhanHoanThanh,c.NgayDuKienHoanThanh, tt.TenTrangThai from CongViec as c inner join TrangThaiCongViec as tt on c.TrangThaiCongViecId=tt.TrangThaiCongViecId left join NguoiDung as nxl on c.NguoiXuLyId=nxl.NguoiDungId left join NguoiDung as nht on c.NguoiHoTroId=nht.NguoiDungId where c.LaShipAble=0 and c.LaTask=1";
                if (week > 0)
                {
                    str += " and ((c.Tuan =" + week + " and c.Nam =" + nam + ") or(c.NgayDuKienHoanThanh>= '" + startDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and c.NgayDuKienHoanThanh <='" + endDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";

                }
                str += " and c.KhoaChaId=" + khoaChaId;
                if (!string.IsNullOrEmpty(keyword)) str += " and c.TenCongViec like N'%" + keyword + "%'";
                if (duAnId > 0) str += " and c.duAnId=" + duAnId;
                if (nguoiDungId != Guid.Empty) str += " and(c.NguoiXuLyId='" + nguoiDungId.ToString() + "' or c.NguoiHotroId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetTasksBy2(int khoaChaId, int week, int duAnId, Guid nguoiDungId, string keyword)
        {
            try
            {
                var str = "select c.CongViecId,c.ThoiGianUocLuong,c.ThuTuUuTien, c.TenCongViec, c.MaCongViec, nxl.HoTen as HoTenNguoiXuLy, nxl.Avatar as AvatarNguoiXuLy, nht.HoTen as HoTenNguoiHotro,c.NguoiXuLyId,c.NguoiHoTroId, nht.Avatar as AvatarNguoiHotro,tt.MaMau as MaMauTrangThaiCongViec,c.XacNhanHoanThanh,c.TrangThaiCongViecId,c.NgayDuKienHoanThanh, tt.TenTrangThai from CongViec as c inner join TrangThaiCongViec as tt on c.TrangThaiCongViecId=tt.TrangThaiCongViecId left join NguoiDung as nxl on c.NguoiXuLyId=nxl.NguoiDungId left join NguoiDung as nht on c.NguoiHoTroId=nht.NguoiDungId where c.LaShipAble=0 and c.LaTask=1 and( c.XacNhanHoanThanh =0 or c.XacNhanHoanThanh IS NULL)";
                if (week > 0) str += " and c.Tuan =" + week;
                str += " and c.KhoaChaId=" + khoaChaId;
                if (!string.IsNullOrEmpty(keyword)) str += " and c.TenCongViec like N'%" + keyword + "%'";
                if (duAnId > 0) str += " and c.duAnId=" + duAnId;
                if (nguoiDungId != Guid.Empty) str += " and(c.NguoiXuLyId='" + nguoiDungId.ToString() + "' or c.NguoiHotroId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int CountTaskByShipId(int shipId)
        {
            try
            {
                var str = "Select Count(CongViecId) from CongViec where LaShipAble=0 and LaTask =1 and KhoaChaId=" + shipId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public List<CongViecViewModel> GetTasksBy3(int khoaChaId, int duAnId, Guid nguoiDungId, string keyword, int trangThaiCongViecId)
        {
            try
            {
                var str = "select c.CongViecId,c.ThoiGianUocLuong,c.ThuTuUuTien, c.TenCongViec, c.MaCongViec, nxl.HoTen as HoTenNguoiXuLy, nxl.Avatar as AvatarNguoiXuLy, nht.HoTen as HoTenNguoiHotro,c.NguoiXuLyId,c.NguoiHoTroId, nht.Avatar as AvatarNguoiHotro,tt.MaMau as MaMauTrangThaiCongViec,c.XacNhanHoanThanh,c.TrangThaiCongViecId,c.NgayDuKienHoanThanh, tt.TenTrangThai from CongViec as c inner join TrangThaiCongViec as tt on c.TrangThaiCongViecId=tt.TrangThaiCongViecId left join NguoiDung as nxl on c.NguoiXuLyId=nxl.NguoiDungId left join NguoiDung as nht on c.NguoiHoTroId=nht.NguoiDungId where c.LaShipAble=0 and c.LaTask=1 ";
                if (trangThaiCongViecId > 0) str += " and c.TrangThaiCongViecId!=" + trangThaiCongViecId;
                str += " and c.KhoaChaId=" + khoaChaId;
                if (!string.IsNullOrEmpty(keyword)) str += " and c.TenCongViec like N'%" + keyword + "%'";
                if (duAnId > 0) str += " and c.duAnId=" + duAnId;
                if (nguoiDungId != Guid.Empty) str += " and(c.NguoiXuLyId='" + nguoiDungId.ToString() + "' or c.NguoiHotroId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetTasksBy4(int khoaChaId, int duAnId, Guid nguoiDungId, string keyword, int trangThaiCongViecId)
        {
            try
            {
                var str = "select c.CongViecId,c.ThoiGianUocLuong,c.ThuTuUuTien, c.TenCongViec, c.MaCongViec, nxl.HoTen as HoTenNguoiXuLy, nxl.Avatar as AvatarNguoiXuLy, nht.HoTen as HoTenNguoiHotro,c.NguoiXuLyId,c.NguoiHoTroId, nht.Avatar as AvatarNguoiHotro,tt.MaMau as MaMauTrangThaiCongViec,c.XacNhanHoanThanh,c.TrangThaiCongViecId,c.NgayDuKienHoanThanh, tt.TenTrangThai from CongViec as c inner join TrangThaiCongViec as tt on c.TrangThaiCongViecId=tt.TrangThaiCongViecId left join NguoiDung as nxl on c.NguoiXuLyId=nxl.NguoiDungId left join NguoiDung as nht on c.NguoiHoTroId=nht.NguoiDungId where c.LaShipAble=0 and c.LaTask=1 ";
                if (trangThaiCongViecId > 0) str += " and c.TrangThaiCongViecId!=" + trangThaiCongViecId;
                str += " and c.KhoaChaId=" + khoaChaId;
                if (!string.IsNullOrEmpty(keyword)) str += " and c.TenCongViec like N'%" + keyword + "%'";
                if (duAnId > 0) str += " and c.duAnId=" + duAnId;
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        /// <summary>
        /// Lấy danh sách dự án id theo người
        /// </summary>
        /// <param name="khoaChaId"></param>
        /// <param name="duAnId"></param>
        /// <param name="nguoiDungId"></param>
        /// <param name="keyword"></param>
        /// <param name="trangThaiCongViecId"></param>
        /// <returns></returns>
        public List<int> GetShipsBy5(Guid nguoiDungId, string keyword, int trangThaiCongViecId)
        {
            try
            {
                var str = "select d.KhoaChaId from CongViec as ship inner join duAn as d on ship.DuAnId = d.DuAnId  inner join CongViec as c on ship.CongViecId=c.KhoaChaId inner join TrangThaiCongViec as tt on c.TrangThaiCongViecId=tt.TrangThaiCongViecId left join NguoiDung as nxl on c.NguoiXuLyId=nxl.NguoiDungId left join NguoiDung as nht on c.NguoiHoTroId=nht.NguoiDungId where c.LaShipAble=0 and c.LaTask=1 ";
                if (trangThaiCongViecId > 0) str += " and c.TrangThaiCongViecId!=" + trangThaiCongViecId;
                if (!string.IsNullOrEmpty(keyword)) str += " and c.TenCongViec like N'%" + keyword + "%'";
                if (nguoiDungId != Guid.Empty) str += " and(c.NguoiXuLyId='" + nguoiDungId.ToString() + "' or c.NguoiHotroId='" + nguoiDungId.ToString() + "'or ship.NguoiTaoId='" + nguoiDungId.ToString() + "' or ship.NguoiXuLyId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #region Task

        #endregion


        public List<CongViecViewModel> GetsByDuAnId(int duAnId, Guid userId)
        {
            try
            {
                var str = "select * from CongViec where DuAnId= " + duAnId + " and (NguoiXuLyId=N'" + userId.ToString() + "' or NguoiHoTroId=N'" + userId.ToString() + "' or NguoiTaoId=N'" + userId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetsBy(int? duAnId, int tuan, int nam, int? trangThaiCongViecId, List<Guid> nguoiDungIds)
        {
            try
            {
                var str = "select c.CongViecId,g.LoaiGiaiDoanId,c.ThuTuUuTien,c.KhoaChaId,c.DiemPoint,c.DuAnId,c.Tuan,c.TuanHoanThanh,c.GiaiDoanDuAnId,c.TenCongViec,d.MaDuAn,d.TenDuAn,g.TenGiaiDoan,c.MoTa,c.TrangThaiCongViecId,t.TenTrangThai,c.NgayDuKienHoanThanh,t.MaMau,c.NguoiXuLyId,c.MaCongViec,n.Avatar as AvatarNguoiXuLy,n.HoTen as HoTenNguoiXuLy from CongViec as c inner join DuAn as d on c.DuAnId=d.DuAnId left join GiaiDoanDuAn as g on c.GiaiDoanDuAnId=g.GiaiDoanDuAnId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId= n.NguoiDungId where c.LaShipAble=1 and ((c.TuanHoanThanh IS NULL and c.Tuan=" + tuan + " and c.Nam=" + nam + ")or( c.TuanHoanThanh=" + tuan + " and c.Nam=" + nam + " and c.TuanHoanThanh IS NOT NULL))";
                if (duAnId != null && duAnId > 0) str += " and c.DuAnId = " + duAnId;
                if (trangThaiCongViecId != null && trangThaiCongViecId > 0) str += " and c.TrangThaiCongViecId=" + trangThaiCongViecId;
                //if (nguoiDungIds != Guid.Empty) str += " and c.NguoiXuLyId='" + nguoiDungId.ToString() + "'";
                if (nguoiDungIds != null && nguoiDungIds.Count > 0)
                {
                    str += " and c.NguoiXuLyId in (";
                    var i = 1;
                    foreach (var id in nguoiDungIds)
                    {
                        str += "'" + id.ToString() + "'";
                        if (i < nguoiDungIds.Count) str += ",";
                        i++;
                    }
                    str += ")";
                }

                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách công việc theo dự án, trạng thái công việc, người dùng,tuần, năm
        /// </summary>
        /// <param name="duAnId"></param>
        /// <param name="tuan"></param>
        /// <param name="nam"></param>
        /// <param name="trangThaiCongViecIds"></param>
        /// <param name="nguoiDungIds"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetsBy(int? duAnId, int tuan, int nam, List<int> trangThaiCongViecIds, List<Guid> nguoiDungIds)
        {
            try
            {
                var str = "select c.CongViecId,g.LoaiGiaiDoanId,c.ThuTuUuTien,c.KhoaChaId,c.DiemPoint,c.DuAnId,c.Tuan,c.TuanHoanThanh,c.GiaiDoanDuAnId,c.TenCongViec,d.MaDuAn,d.TenDuAn,g.TenGiaiDoan,c.MoTa,c.TrangThaiCongViecId,t.TenTrangThai,c.NgayDuKienHoanThanh,t.MaMau,c.NguoiXuLyId,c.MaCongViec,n.Avatar as AvatarNguoiXuLy,n.HoTen as HoTenNguoiXuLy from CongViec as c inner join DuAn as d on c.DuAnId=d.DuAnId left join GiaiDoanDuAn as g on c.GiaiDoanDuAnId=g.GiaiDoanDuAnId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId= n.NguoiDungId where c.LaShipAble=1 and ((c.TuanHoanThanh IS NULL and c.Tuan=" + tuan + " and c.Nam=" + nam + ")or( c.TuanHoanThanh=" + tuan + " and c.Nam=" + nam + " and c.TuanHoanThanh IS NOT NULL))";
                if (duAnId != null && duAnId > 0) str += " and c.DuAnId = " + duAnId;
                if (trangThaiCongViecIds != null && trangThaiCongViecIds.Count > 0)
                {
                    str += " and c.TrangThaiCongViecId in(";
                    int i = 1;
                    foreach (var tt in trangThaiCongViecIds)
                    {
                        str += tt;
                        if (i < trangThaiCongViecIds.Count) str += ",";
                        i++;
                    }
                    str += ")";
                }
                if (nguoiDungIds != null && nguoiDungIds.Count > 0)
                {
                    str += " and c.NguoiXuLyId in (";
                    var i = 1;
                    foreach (var id in nguoiDungIds)
                    {
                        str += "'" + id.ToString() + "'";
                        if (i < nguoiDungIds.Count) str += ",";
                        i++;
                    }
                    str += ")";
                }

                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<CongViecViewModel> GetsBy(int? duAnId, int tuan)
        {
            try
            {
                var str = "select c.CongViecId,c.ThuTuUuTien,c.KhoaChaId,c.DiemPoint,c.DuAnId,c.Tuan,c.TuanHoanThanh,c.GiaiDoanDuAnId,c.TenCongViec,d.MaDuAn,d.TenDuAn,g.TenGiaiDoan,c.MoTa,c.TrangThaiCongViecId,t.TenTrangThai,c.NgayDuKienHoanThanh,t.MaMau,c.MaCongViec,n.Avatar as AvatarNguoiXuLy,n.HoTen as HoTenNguoiXuLy from CongViec as c inner join DuAn as d on c.DuAnId=d.DuAnId left join GiaiDoanDuAn as g on c.GiaiDoanDuAnId=g.GiaiDoanDuAnId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId= n.NguoiDungId where c.LaShipAble=1 and ((c.TrangThaiCongViecId!=12 and c.Tuan=" + tuan + ")or( c.TuanHoanThanh=" + tuan + " and c.TrangThaiCongViecId=12))";
                if (duAnId != null && duAnId > 0) str += " and c.DuAnId = " + duAnId;
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int CountCongViecOfUserBy(Guid userId, DateTime startDate, DateTime endDate, int trangThaiCongViecId)
        {
            try
            {
                var str = "select COUNT(congViecId) from CongViec where LaShipAble=0 and LaTask=1 and NguoiXuLyId='" + userId.ToString() + "' and NgayDuKienHoanThanh>= '" + startDate.ToString("yyyy-MM-dd hh:mm:ss") + "' and NgayDuKienHoanThanh<='" + endDate.ToString("yyyy-MM-dd hh:mm:ss") + "'";
                if (trangThaiCongViecId > 0)
                {
                    str += " and TrangThaiCongViecId=11";
                }
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public CongViecViewModel GetById(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<CongViecViewModel>("select * from CongViec where CongViecId=" + congViecId).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public CongViecViewModel GetById2(int congViecId)
        {
            try
            {
                var str = "select c.Nam,c.CongViecId,c.LoaiPoint,c.ThuTuUuTien,c.NgayTao,n.HoTen as HoTenNguoiXuLy,n.TenDangNhap as TenDangNhapNguoiXuLy,n.Avatar as AvatarNguoiXuLy, nht.Avatar as AvatarNguoiHoTro,c.NgayDuKienHoanThanh,c.DiemPoint,c.DuAnId, Nht.HoTen as HoTenNguoiHoTro,Nht.TenDangNhap as TenDangNhapNguoiHoTro,c.NguoiHoTroId, c.GiaiDoanDuAnId,c.KhoaChaId,c.ThoiGianUocLuong,c.GiaiDoanDuAnId,cv.TenCongViec as TenKhoaCha, c.TenCongViec, d.TenDuAn, g.TenGiaiDoan, c.MoTa, c.TrangThaiCongViecId, t.TenTrangThai, c.LaShipAble, t.MaMau, c.NguoiXuLyId, c.MaCongViec, c.Tuan,c.DoPhucTap,c.XacNhanHoanThanh from CongViec as c inner join DuAn as d on c.DuAnId=d.DuAnId left join GiaiDoanDuAn as g on c.GiaiDoanDuAnId=g.GiaiDoanDuAnId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId left join NguoiDung as Nht on c.NguoiHoTroId=Nht.NguoiDungId left join CongViec as cv on c.KhoaChaId=cv.CongViecId where c.CongViecId=" + congViecId;
                return db.ExecuteQuery<CongViecViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CongViecViewModel GetById3(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<CongViecViewModel>("select * from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where CongViecId =" + congViecId).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public CongViecViewModel GetById4(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<CongViecViewModel>("select c.CongViecId,c.DuAnId,c.TrangThaiCongViecId,c.LoaiTimer,c.NgayTao,n.HoTen,n.Avatar,n.Quyen,n.Email,c.NguoiXuLyId,c.NguoiHoTroId,c.TenCongViec from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where CongViecId =" + congViecId).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public int TotalShipableInDuAn(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<int>("select COUNT(CongViecId) from CongViec where DuAnId = " + duAnId + " and LaShipAble = 1").FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int TotalShipableDoneInDuAn(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<int>("select COUNT(CongViecId) from CongViec where DuAnId = " + duAnId + " and LaShipAble = 1 and TrangThaiCongViecId=4").FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public List<CongViecViewModel> GetShipablesNewOrDebitBy(int? duAnId, int tuan, int nam, List<int> trangThaiCongViecIds)
        {
            try
            {
                var str = "select c.CongViecId,c.ThuTuUuTien,c.NgayTao,n.HoTen as HoTenNguoiXuLy,n.TenDangNhap as TenDangNhapNguoiXuLy,n.Avatar as AvatarNguoiXuLy,c.NgayDuKienHoanThanh,c.DiemPoint,c.DuAnId,c.GiaiDoanDuAnId,d.MaDuAn,c.TenCongViec,d.TenDuAn,g.TenGiaiDoan,c.MoTa,c.TrangThaiCongViecId,t.TenTrangThai,t.MaMau,c.NguoiXuLyId,c.MaCongViec from CongViec as c inner join DuAn as d on c.DuAnId=d.DuAnId left join GiaiDoanDuAn as g on c.GiaiDoanDuAnId=g.GiaiDoanDuAnId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId=t.TrangThaiCongViecId left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where c.LaShipAble=1 and ((c.TuanHoanThanh IS NULL and c.Nam=" + nam + " and c.Tuan=" + tuan + ")or (c.TuanHoanThanh IS NOT NULL and c.TuanHoanThanh =" + tuan + " and c.Nam=" + nam + "))";
                if (duAnId != null && duAnId > 0) str += " and c.DuAnId = " + duAnId;
                if (trangThaiCongViecIds != null && trangThaiCongViecIds.Count > 0)
                {
                    str += " and c.TrangThaiCongViecId in (";
                    int i = 1;
                    foreach (var item in trangThaiCongViecIds)
                    {
                        str += item;
                        if (i < trangThaiCongViecIds.Count) str += ",";
                        i++;
                    }
                    str += ")";
                }
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetToDoBy(int khoaChaId)
        {
            try
            {
                var str = "select  c.CongViecId,c.XacNhanHoanThanh ,c.DiemPoint,c.LoaiTimer,c.LaShipAble,c.NgayDuKienHoanThanh,c.NgayTao,c.MaCongViec,c.DoPhucTap,c.ThuTuUuTien,c.TenCongViec,n.HoTen as HoTenNguoiXuLy,n.Avatar as AvatarNguoiXuLy, nht.Avatar as AvatarNguoiHoTro, nht.HoTen as HoTenNguoiHoTro, c.NguoiHoTroId, c.NguoiXuLyId, t.TenTrangThai, c.TrangThaiCongViecId, t.MaMau, t.TenTrangThai from CongViec as c left join NguoiDung as N on c.NguoiXuLyId = n.NguoiDungId left join NguoiDung as Nht on c.NguoiHoTroId=Nht.NguoiDungId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId = t.TrangThaiCongViecId where c.LaShipAble=0 and c.KhoaChaId=" + khoaChaId;
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public int GetToDoBy2(int khoaChaId)
        {
            try
            {
                var str = "select COUNT(congviecId) from CongViec where KhoaChaId=" + khoaChaId + " and LaShipAble=0 and LaTask=1";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public List<CongViecViewModel> GetToDoBy(DateTime startDate, DateTime endDate, List<int> khoaChaIds)
        {
            try
            {
                var str = "select  c.CongViecId,c.DiemPoint,c.NgayDuKienHoanThanh,c.MaCongViec,c.DoPhucTap,c.ThuTuUuTien,c.TenCongViec,n.HoTen as HoTenNguoiXuLy,n.TenDangNhap as TenDangNhapNguoiXuLy, Nht.HoTen as HoTenNguoiHoTro,Nht.TenDangNhap as TenDangNhapNguoiHotro,n.Avatar as AvatarNguoiXuLy,Nht.Avatar as AvatarNguoiHoTro,t.TenTrangThai,c.TrangThaiCongViecId,t.MaMau,t.TenTrangThai, c.NguoiHoTroId, c.NguoiXuLyId from CongViec as c left join NguoiDung as N on c.NguoiXuLyId = n.NguoiDungId left join NguoiDung as Nht on c.NguoiHoTroId=Nht.NguoiDungId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId = t.TrangThaiCongViecId where c.LaShipAble=0 and c.NgayDuKienHoanThanh >= '" + startDate.Year + "-" + startDate.Month + "-" + startDate.Day + " 00:00:00' and c.NgayDuKienHoanThanh <='" + endDate.Year + "-" + endDate.Month + "-" + endDate.Day + " " + endDate.Hour + ":" + endDate.Minute + ":" + endDate.Second + "'";
                if (khoaChaIds != null && khoaChaIds.Count > 0)
                {
                    str += " and c.KhoaChaId in(";
                    var i = 1;
                    foreach (var item in khoaChaIds)
                    {
                        str += item;
                        if (i < khoaChaIds.Count) str += ",";
                        i++;
                    }
                    str += ")";
                }
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetToDoBy(DateTime startDate, DateTime endDate, Guid userId, List<int> loaiCongViecIds)
        {
            try
            {
                var str = "select  c.CongViecId,c.DiemPoint,c.NgayDuKienHoanThanh,c.MaCongViec,c.DoPhucTap,c.ThuTuUuTien,c.TenCongViec,n.HoTen as HoTenNguoiXuLy, nht.HoTen as HoTenNguoiHoTro,n.TenDangNhap as TenDangNhapNguoiXuLy,n.Avatar as AvatarNguoiXuLy,Nht.Avatar as AvatarNguoiHoTro,Nht.TenDangNhap as TenDangNhapNguoiHotro,t.TenTrangThai,c.TrangThaiCongViecId,t.MaMau,t.TenTrangThai, c.NguoiHoTroId, c.NguoiXuLyId from CongViec as c left join NguoiDung as N on c.NguoiXuLyId = n.NguoiDungId left join NguoiDung as Nht on c.NguoiHoTroId=Nht.NguoiDungId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId = t.TrangThaiCongViecId where c.LaShipAble=0 and c.NguoiXuLyId= N'" + userId.ToString() + "' and c.NgayDuKienHoanThanh >= '" + startDate.Year + "-" + startDate.Month + "-" + startDate.Day + " 00:00:00' and c.NgayDuKienHoanThanh <='" + endDate.Year + "-" + endDate.Month + "-" + endDate.Day + " " + endDate.Hour + ":" + endDate.Minute + ":" + endDate.Second + "'";
                if (loaiCongViecIds != null && loaiCongViecIds.Count > 0)
                {
                    str = "select  c.CongViecId,c.DiemPoint,c.NgayDuKienHoanThanh,c.MaCongViec,c.DoPhucTap,c.ThuTuUuTien,c.TenCongViec,n.HoTen as HoTenNguoiXuLy, nht.HoTen as HoTenNguoiHoTro,n.TenDangNhap as TenDangNhapNguoiXuLy,n.Avatar as AvatarNguoiXuLy,Nht.Avatar as AvatarNguoiHoTro,Nht.TenDangNhap as TenDangNhapNguoiHotro,t.TenTrangThai,c.TrangThaiCongViecId,t.MaMau,t.TenTrangThai, c.NguoiHoTroId, c.NguoiXuLyId from CongViec as c left join NguoiDung as N on c.NguoiXuLyId = n.NguoiDungId left join NguoiDung as Nht on c.NguoiHoTroId=Nht.NguoiDungId inner join TrangThaiCongViec as t on c.TrangThaiCongViecId = t.TrangThaiCongViecId inner join LienKetLoaiCongViec as lkl on c.CongViecId=lkl.CongViecId where c.LaShipAble=0 and c.NguoiXuLyId= N'" + userId.ToString() + "' and c.NgayDuKienHoanThanh >= '" + startDate.Year + "-" + startDate.Month + "-" + startDate.Day + " 00:00:00' and c.NgayDuKienHoanThanh <='" + endDate.Year + "-" + endDate.Month + "-" + endDate.Day + " " + endDate.Hour + ":" + endDate.Minute + ":" + endDate.Second + "' and lkl.LoaiCongViecId in (";
                    int i = 1;
                    foreach (var item in loaiCongViecIds)
                    {
                        str += item;
                        if (i < loaiCongViecIds.Count)
                        {
                            str += ",";
                        }
                        i++;
                    }
                    str += ")";
                }
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public TrangThaiCongViecViewModel GetTrangThaiById(int id)
        {
            try
            {
                return db.ExecuteQuery<TrangThaiCongViecViewModel>("Select * from TrangThaiCongViec where TrangThaiCongViecId=" + id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy tất cả shipalbe với khóa cha id
        /// </summary>
        /// <param name="khoaChaId"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetsByKhoaChaId(int khoaChaId)
        {
            try
            {
                return db.ExecuteQuery<CongViecViewModel>("select * from CongViec where LaShipAble=1 and KhoaChaId=" + khoaChaId).ToList();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy tất cả shipalbe với khóa cha id
        /// </summary>
        /// <param name="khoaChaId"></param>
        /// <returns></returns>
        public List<int> GetToDoIdsBy(int taskId, Guid nguoiDungId)
        {
            try
            {
                return db.ExecuteQuery<int>("select congViecId from CongViec where LaShipAble=0 and LaTask = 0 and KhoaChaId=" + taskId + " and NguoiXuLyId = '" + nguoiDungId.ToString() + "'").ToList();
            }
            catch
            {
                return null;
            }
        }




        public List<CongViecViewModel> GetTenCongViecsByKhoaChaId(int khoaChaId)
        {
            try
            {
                return (from c in db.CongViecs join t in db.TrangThaiCongViecs on c.TrangThaiCongViecId equals t.TrangThaiCongViecId where c.KhoaChaId == khoaChaId select new CongViecViewModel { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, TrangThaiCongViecId = c.TrangThaiCongViecId, TenTrangThai = t.TenTrangThai, MaMauTrangThaiCongViec = t.MaMau }).ToList();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách shipable theo tuần, dự án
        /// </summary>
        /// <param name="week"></param>
        /// <param name="duAnId"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetShipablesBy(int week, int duAnId, Guid nguoiDungId)
        {
            try
            {
                var str = "Select * from CongViec Where LaShipable= 1";
                if (week > 0) str += " and ((Tuan = " + week + " and TuanHoanThanh IS NULL) or (TuanHoanThanh IS NOT NULL and Tuan <= " + week + " and TuanHoanThanh>=" + week + "))";
                else str += " and TrangThaiCongViecId not in (4,5,6)";
                if (duAnId > 0) str += " and DuAnId =" + duAnId;
                if (nguoiDungId != Guid.Empty) str += " and (NguoiXuLyId='" + nguoiDungId.ToString() + "' or NguoiHoTroId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<CongViecViewModel> GetShipablesBy2(int fromWeek, int fromYear, int toWeek, int toYear, int duAnId, Guid nguoiDungId)
        {
            try
            {
                var str = "Select * from CongViec Where LaShipable= 1";

                if (fromWeek > 0 && toWeek > 0)
                {
                    if (fromYear != toYear)
                    {
                        str += " and (((Tuan >= " + fromWeek + "and Nam = " + fromYear + ")or (Tuan<=" + toWeek + " and Nam =" + toYear + ")) and TuanHoanThanh IS NULL) or (TuanHoanThanh IS NOT NULL and ((Tuan >= " + fromWeek + "and Nam = " + fromYear + ")or (Tuan<=" + toWeek + " and Nam =" + toYear + ")) and (TuanHoanThanh>=" + fromWeek + ") or(TuanHoanThanh>=" + toWeek + "))";
                    }
                    else
                    {
                        str += " and (((Tuan >= " + fromWeek + "and Nam = " + fromYear + ")and (Tuan<=" + toWeek + " and Nam =" + toYear + ")) and TuanHoanThanh IS NULL) or (TuanHoanThanh IS NOT NULL and ((Tuan >= " + fromWeek + "and Nam = " + fromYear + ")and (Tuan<=" + toWeek + " and Nam =" + toYear + ")) and (TuanHoanThanh>=" + fromWeek + "))";
                    }
                }
                else str += " and TrangThaiCongViecId not in (4,5,6)";
                if (duAnId > 0) str += " and DuAnId =" + duAnId;
                if (nguoiDungId != Guid.Empty) str += " and (NguoiXuLyId='" + nguoiDungId.ToString() + "' or NguoiHoTroId='" + nguoiDungId.ToString() + "')";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }




        public List<CongViecViewModel> GetToDosBy(int khoaChaId, bool? makeAsDone)
        {
            try
            {
                var str = "select * from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where c.KhoaChaId=" + khoaChaId;
                if (makeAsDone != null)
                {
                    if (makeAsDone.Value) str += " and c.XacNhanHoanThanh=1";

                    else str += " and (c.XacNhanHoanThanh=0 or c.XacNhanHoanThanh IS NULL)";
                }
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<CongViecViewModel> GetToDosBy2(int khoaChaId, bool? makeAsDone, Guid nguoiDungId)
        {
            try
            {
                var str = "select * from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId where c.NguoiXuLyId='" + nguoiDungId.ToString() + "' and c.KhoaChaId=" + khoaChaId;
                if (makeAsDone != null)
                {
                    if (makeAsDone.Value) str += " and c.XacNhanHoanThanh=1";

                    else str += " and (c.XacNhanHoanThanh=0 or c.XacNhanHoanThanh IS NULL)";
                }
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách todo trong khoảng thời gian mà chưa done hoặc công việc đang chạy(congViecId)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="congViecId"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetToDosBy(Guid userId, DateTime startTime, DateTime endTime, int congViecId)
        {
            try
            {
                var str = "Select * from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId Where (c.NguoiXuLyId = '" + userId + "' and c.NgayTao <='" + endTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and c.NgayTao>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and c.LaShipAble=0 and c.LaTask =0 and c.LoaiTimer=1) or c.CongViecId= " + congViecId;
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Lay danh sach id ( todo, timeentry) theo khoa cha va ngay lam viec
        /// </summary>
        /// <param name="khoaChaId"></param>
        /// <param name="ngayLamViec"></param>
        /// <returns></returns>
        public List<int> GetIdsBy2(int khoaChaId, DateTime ngayLamViec)
        {
            try
            {
                var str = "select c.CongViecId from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId inner join ThoiGianLamViec as t on t.CongViecId=c.CongViecId where c.KhoaChaId=" + khoaChaId + " and t.NgayLamViec='" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' group by c.CongViecId order by c.congviecId";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách các công việc với khoảng thời gian đã chạy theo ngày và theo khóa cha
        /// </summary>
        /// <param name="khoaChaId"></param>
        /// <param name="ngayLamViec"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetsBy2(int khoaChaId, DateTime ngayLamViec)
        {
            try
            {
                var str = "select c.CongViecId,c.DuAnId,c.LoaiTimer,c.TrangThaiCongViecId,c.NgayTao,t.ThoiGianBatDau,t.ThoiGianKetThuc,t.TongThoiGian,t.LoaiThoiGian,t.NguoiDungId,t.NgayLamViec,t.PheDuyet,t.TokenId,t.LoaiNgayLamViec,t.ThoiGianLamViecId,n.HoTen,n.Avatar,n.Quyen,n.Email,c.NguoiXuLyId,c.NguoiHoTroId,c.TenCongViec from CongViec as c left join NguoiDung as n on c.NguoiXuLyId=n.NguoiDungId inner join ThoiGianLamViec as t on t.CongViecId=c.CongViecId where c.KhoaChaId=" + khoaChaId + " and t.ThoiGianKetThuc Is not null and t.NgayLamViec='" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' order by t.ThoiGianBatDau desc";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<int> GetCongViecIdsByKhoaCha(int khoaChaId)
        {
            try
            {
                var str = "select CongViecId from CongViec where KhoaChaId=" + khoaChaId;
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        #endregion   

        public CongViecViewModel GetTaskBy(string tenTask, int duAnId, int? khoaChaId)
        {
            try
            {
                if (khoaChaId == null || khoaChaId == 0)
                {
                    return (from c in db.CongViecs
                            join dtp in db.DuAns on c.DuAnId equals dtp.DuAnId
                            join d in db.DuAns on dtp.KhoaChaId equals d.DuAnId
                            where d.DuAnId == duAnId && c.LaShipAble == false && c.LaTask == true && c.TenCongViec.ToUpper().Trim().Equals(tenTask.Trim().ToUpper())
                            select new CongViecViewModel { CongViecId = c.CongViecId, DuAnId = c.DuAnId, TenCongViec = c.TenCongViec, NgayTao = c.NgayTao }
                            ).OrderByDescending(t => t.NgayTao).FirstOrDefault();
                }
                else
                {
                    return (from c in db.CongViecs
                            join d in db.DuAns on c.DuAnId equals d.DuAnId
                            where d.DuAnId == duAnId && c.LaShipAble == false && c.LaTask == true && c.TenCongViec.ToUpper().Trim().Equals(tenTask.Trim().ToUpper())
                            select new CongViecViewModel { CongViecId = c.CongViecId, DuAnId = c.DuAnId, TenCongViec = c.TenCongViec, NgayTao = c.NgayTao }
                           ).OrderByDescending(t => t.NgayTao).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }

        }



        public int Insert(CongViecViewModel vm)
        {
            try
            {
                CongViec entity = new CongViec();
                entity.DuAnId = vm.DuAnId;
                entity.NguoiTaoId = vm.NguoiTaoId;
                if (vm.NgayTao != null)
                    entity.NgayTao = vm.NgayTao.Value;
                else entity.NgayTao = DateTime.Now;
                entity.GiaiDoanDuAnId = vm.GiaiDoanDuAnId;
                entity.TenCongViec = vm.TenCongViec;
                entity.LaShipAble = vm.LaShipAble;
                entity.LaTask = vm.LaTask;
                entity.TrangThaiCongViecId = vm.TrangThaiCongViecId;
                entity.Tuan = vm.Tuan;
                if (vm.IsToDoAdd) entity.NgayDuKienHoanThanh = vm.NgayLamViec;
                else
                    entity.NgayDuKienHoanThanh = vm.NgayDuKienHoanThanh;
                entity.ThuTuUuTien = vm.ThuTuUuTien;
                entity.MoTa = vm.MoTa;
                entity.MaCongViec = vm.MaCongViec;
                entity.KhoaChaId = vm.KhoaChaId;
                entity.NguoiXuLyId = vm.NguoiXuLyId;
                entity.NguoiHoTroId = vm.NguoiHoTroId;
                entity.DoPhucTap = vm.DoPhucTap;
                entity.DiemPoint = vm.DiemPoint;
                entity.ThoiGianUocLuong = vm.ThoiGianUocLuong;
                entity.LoaiTimer = vm.LoaiTimer;
                entity.XacNhanHoanThanh = vm.XacNhanHoanThanh;
                entity.Nam = vm.Nam;
                entity.IsToDoAdd = vm.IsToDoAdd;
                db.CongViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.CongViecId;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public bool InsertOrUpdateQuanTam(int congViecId, Guid nguoiDungId)
        {
            try
            {
                var entity = db.LienKetNguoiDungCongViecs.Where(t => t.CongViecId == congViecId && t.NguoiDungId == nguoiDungId).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.QuanTam == true) entity.QuanTam = false;
                    else entity.QuanTam = true;
                }
                else
                {
                    var newEntity = new LienKetNguoiDungCongViec();
                    newEntity.CongViecId = congViecId;
                    newEntity.NguoiDungId = nguoiDungId;
                    newEntity.QuanTam = true;
                    db.LienKetNguoiDungCongViecs.InsertOnSubmit(newEntity);
                }
                db.SubmitChanges();
                return true;
            }
            catch
            {
                try
                {
                    var newEntity = new LienKetNguoiDungCongViec();
                    newEntity.CongViecId = congViecId;
                    newEntity.NguoiDungId = nguoiDungId;
                    newEntity.QuanTam = true;
                    db.LienKetNguoiDungCongViecs.InsertOnSubmit(newEntity);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }

        }

        public int InsertEntry(int duAnId, int khoaChaId, Guid nguoiTaoId, Guid nguoiXuLyId, string tenCongViec, string moTa, int tuan, int nam)
        {
            try
            {
                CongViec entity = new CongViec();
                entity.DuAnId = duAnId;
                entity.NguoiTaoId = nguoiTaoId;
                entity.NgayTao = DateTime.Now;
                entity.GiaiDoanDuAnId = null;
                entity.TenCongViec = tenCongViec;
                entity.LaShipAble = false;
                entity.LaTask = false;
                entity.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.todoNew;
                entity.Tuan = tuan;
                //entity.NgayDuKienHoanThanh = vm.NgayDuKienHoanThanh;
                //entity.ThuTuUuTien = vm.ThuTuUuTien;
                entity.MoTa = moTa;
                //entity.MaCongViec = vm.MaCongViec;
                entity.KhoaChaId = khoaChaId;
                entity.NguoiXuLyId = nguoiXuLyId;
                //entity.NguoiHoTroId = vm.NguoiHoTroId;
                //entity.DoPhucTap = vm.DoPhucTap;
                //entity.DiemPoint = vm.DiemPoint;
                //entity.ThoiGianUocLuong = vm.ThoiGianUocLuong;
                entity.LoaiTimer = 3;
                entity.Nam = nam;
                //entity.XacNhanHoanThanh = vm.XacNhanHoanThanh;
                db.CongViecs.InsertOnSubmit(entity);

                db.SubmitChanges();
                return entity.CongViecId;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public bool Update(CongViecViewModel vm)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == vm.CongViecId).FirstOrDefault();
                entity.DuAnId = vm.DuAnId;
                entity.NgayCapNhat = DateTime.Now;
                entity.GiaiDoanDuAnId = vm.GiaiDoanDuAnId;
                entity.TenCongViec = vm.TenCongViec;
                entity.TrangThaiCongViecId = vm.TrangThaiCongViecId;
                entity.Tuan = vm.Tuan;
                entity.NgayDuKienHoanThanh = vm.NgayDuKienHoanThanh;
                entity.ThuTuUuTien = vm.ThuTuUuTien;
                entity.MoTa = vm.MoTa;
                //entity.MaCongViec = vm.MaCongViec;
                if (entity.LaShipAble == false)
                    entity.KhoaChaId = vm.KhoaChaId;
                if (vm.NguoiXuLyId != null)
                    entity.NguoiXuLyId = vm.NguoiXuLyId;
                entity.NguoiHoTroId = vm.NguoiHoTroId;
                entity.DoPhucTap = vm.DoPhucTap;
                entity.DiemPoint = vm.DiemPoint;
                if (entity.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue)
                    entity.TuanHoanThanh = vm.TuanHoanThanh;
                entity.ThoiGianUocLuong = vm.ThoiGianUocLuong;
                entity.Nam = vm.Nam;
                db.SubmitChanges();
                InsertUpdateLKLoaiCongViec(entity.CongViecId, vm.LoaiCongViecIds);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateShipAble(int congViecId, string tenCongViec, DateTime? ngayDuKienHoanThanh, int duAnId, int? giaiDoanDuAnId, int trangThaiCongViecId, string moTa, Guid? nguoiXuLyId)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.DuAnId = duAnId;
                entity.NgayCapNhat = DateTime.Now;
                entity.GiaiDoanDuAnId = giaiDoanDuAnId;
                entity.TenCongViec = tenCongViec;
                entity.TrangThaiCongViecId = trangThaiCongViecId;
                entity.NgayDuKienHoanThanh = ngayDuKienHoanThanh;
                entity.MoTa = moTa;
                if (nguoiXuLyId != null && nguoiXuLyId != Guid.Empty)
                    entity.NguoiXuLyId = nguoiXuLyId;
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool updateKhoaCha(int congViecId, int khoaChaId)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.KhoaChaId = khoaChaId;
                db.SubmitChanges();
                return true;

            }
            catch
            {
                return false;
            }

        }
        public bool DoneAllCongViecInShipableBy(int shipableId, int TrangThaiCongViecDone)
        {
            try
            {
                var lst = db.CongViecs.Where(t => t.KhoaChaId == shipableId).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        item.TrangThaiCongViecId = TrangThaiCongViecDone;
                    }
                }
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool XacNhanHoanThanhToDo(int toDoId, bool xacNhanHoanThanh)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == toDoId).FirstOrDefault();
                entity.NgayCapNhat = DateTime.Now;
                entity.XacNhanHoanThanh = xacNhanHoanThanh;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool XacNhanHoanThanhTask(int taskId, bool xacNhanHoanthanh)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == taskId).FirstOrDefault();
                entity.NgayCapNhat = DateTime.Now;
                entity.XacNhanHoanThanh = xacNhanHoanthanh;
                db.SubmitChanges();
                try
                {
                    var todos = db.CongViecs.Where(t => t.KhoaChaId == taskId).ToList();
                    if (todos != null && todos.Count > 0)
                    {
                        foreach (var item in todos)
                        {
                            item.XacNhanHoanThanh = xacNhanHoanthanh;
                            item.NgayCapNhat = DateTime.Now;
                        }
                    }
                    db.SubmitChanges();
                }
                catch
                {

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int congViecId)
        {
            try
            {
                try
                {
                    var lst = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == congViecId).ToList();
                    db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lst);
                }
                catch
                {

                }
                try
                {
                    var lst = db.LienKetNguoiDungCongViecs.Where(t => t.CongViecId == congViecId).ToList();
                    db.LienKetNguoiDungCongViecs.DeleteAllOnSubmit(lst);
                }
                catch
                {

                }
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                db.CongViecs.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeletesByKhoaChaId(int khoaChaId)
        {
            try
            {
                var lst = db.CongViecs.Where(t => t.KhoaChaId == khoaChaId && t.LaShipAble == true).ToList();
                db.CongViecs.DeleteAllOnSubmit(lst);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTimeEntry(int timeEntryId)
        {
            try
            {
                //Xóa tất cả thời gian làm việc
                try
                {
                    var lst = db.ThoiGianLamViecs.Where(t => t.CongViecId == timeEntryId).ToList();
                    db.ThoiGianLamViecs.DeleteAllOnSubmit(lst);
                    db.SubmitChanges();
                }
                catch
                {

                }
                // Xóa liên kết loại công việc
                try
                {
                    var lst = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == timeEntryId).ToList();
                    db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lst);
                    db.SubmitChanges();
                }
                catch
                {

                }
                var entity = db.CongViecs.Where(t => t.CongViecId == timeEntryId).FirstOrDefault();
                db.CongViecs.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteToDo(int toDoId, int thoiGianLamViecId)
        {
            try
            {
                var checkdelete = true;
                if (thoiGianLamViecId > 0)
                {
                    try
                    {
                        var lst = db.ThoiGianLamViecs.Where(t => t.ThoiGianLamViecId == thoiGianLamViecId).ToList();
                        db.ThoiGianLamViecs.DeleteAllOnSubmit(lst);
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        var lst = db.ThoiGianLamViecs.Where(t => t.CongViecId == toDoId).ToList();
                        db.ThoiGianLamViecs.DeleteAllOnSubmit(lst);
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                }
                // Kiểm tra xem còn thời gianlamf việc không
                try
                {
                    var lst = db.ThoiGianLamViecs.Where(t => t.CongViecId == toDoId).ToList();
                    if (lst != null && lst.Count > 0) checkdelete = false;
                    else checkdelete = true;
                }
                catch
                {
                    checkdelete = true;
                }
                if (checkdelete)
                {
                    // Xóa liên kết loại công việc
                    try
                    {
                        var lst = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == toDoId).ToList();
                        db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lst);
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                    var entity = db.CongViecs.Where(t => t.CongViecId == toDoId).FirstOrDefault();
                    db.CongViecs.DeleteOnSubmit(entity);
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteToDoV2(int toDoId)
        {
            try
            {
                try
                {
                    // Xóa khoảng time
                    var lst = db.ThoiGianLamViecs.Where(t => t.CongViecId == toDoId).ToList();
                    db.ThoiGianLamViecs.DeleteAllOnSubmit(lst);
                    db.SubmitChanges();
                    // Xóa liên kết loại công việc
                    try
                    {
                        var lst2 = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == toDoId).ToList();
                        db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lst2);
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                    var entity = db.CongViecs.Where(t => t.CongViecId == toDoId).FirstOrDefault();
                    db.CongViecs.DeleteOnSubmit(entity);
                    db.SubmitChanges();
                }
                catch
                {

                }



                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertUpdateLKLoaiCongViec(int congViecId, List<int> loaiCongViecIds)
        {
            try
            {
                try
                {
                    var lst = db.LienKetLoaiCongViecs.Where(t => t.CongViecId == congViecId).ToList();
                    db.LienKetLoaiCongViecs.DeleteAllOnSubmit(lst);
                    db.SubmitChanges();
                }
                catch { }
                List<LienKetLoaiCongViec> lstE = new List<LienKetLoaiCongViec>();
                if (loaiCongViecIds != null && loaiCongViecIds.Count > 0)
                {
                    foreach (var item in loaiCongViecIds)
                    {
                        LienKetLoaiCongViec entity = new LienKetLoaiCongViec() { CongViecId = congViecId, LoaiCongViecId = item };
                        lstE.Add(entity);
                    }
                    db.LienKetLoaiCongViecs.InsertAllOnSubmit(lstE);
                    db.SubmitChanges();

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateTrangThai(int congViecId, int TrangThaiId)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.TrangThaiCongViecId = TrangThaiId;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateTrangThai2(int congViecId, int TrangThaiId, int tuanHoanThanh)
        {
            try
            {
                var entity = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                entity.TrangThaiCongViecId = TrangThaiId;
                entity.TuanHoanThanh = tuanHoanThanh;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public decimal GetPointUsedInWeek(int week, Guid userId)
        {
            try
            {
                var str = "select sum(DiemPoint) from CongViec where Tuan=" + week + " and NguoiXuLyId='" + userId + "'";
                return db.ExecuteQuery<decimal>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
