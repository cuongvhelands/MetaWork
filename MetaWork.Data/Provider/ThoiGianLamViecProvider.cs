using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class ThoiGianLamViecProvider
    {
        TimerDataContext db = null;
        public ThoiGianLamViecProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public int GetTimeSpentOfNguoiDungInDuAnBy(int duAnId, Guid nguoiDungId,DateTime startTime,DateTime endTime)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId inner join DuAn as d on c.DuAnId = d.DuAnId where (d.KhoaChaId=" + duAnId + " or (d.KhoaChaId is null and d.DuAnId=" + duAnId + " ))and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet = 1 and t.NgayLamViec>='"+startTime.ToString("yyyy-MM-dd")+"' and t.NgayLamViec<='"+endTime.ToString("yyyy-MM-dd")+"'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }



        public int GetTimeSpentOfNguoiDungInDuAnThanhPhan(int duAnId,Guid nguoiDungId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where c.DuAnId=" + duAnId +" and t.NguoiDungId='"+nguoiDungId.ToString()+"' and t.PheDuyet = 1";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int CostTienOfDuAn(int duAnId)
        {
            try
            {
                var str = "select sum(di.CostTien) from DuAn as dtp inner join DuAn as d on dtp.KhoaChaId=d.DuAnId inner join DuAnInfo as di on dtp.DuAnId = di.DuAnId where d.DuAnId=" + duAnId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int TongNganSachOfDuAn(int duAnId)
        {
            try
            {
                var str = "select sum(di.TongNganSach) from DuAn as dtp inner join DuAn as d on dtp.KhoaChaId=d.DuAnId inner join DuAnInfo as di on dtp.DuAnId = di.DuAnId where d.DuAnId=" + duAnId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }


        public int GetTimeSpentOfNguoiDungInDuAn(int duAnId, Guid nguoiDungId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId inner join DuAn as d on c.DuAnId = d.DuAnId where d.KhoaChaId=" + duAnId + " and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet = 1";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public List<Guid> GetUserSpentTimeInDuAnThanhPhan(int duAnId)
        {
            try
            {
                var str = "select t.NguoiDungId from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId = c.CongViecId where t.PheDuyet=1 and c.DuAnId = "+duAnId +" group by t.NguoiDungId";
                return db.ExecuteQuery<Guid>(str).ToList();               
            }
            catch
            {
                return null;
            }
        }
        public List<Guid> GetUserSpentTimeInDuAn(int duAnId)
        {
            try
            {
                var str = "select t.NguoiDungId from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId = c.CongViecId inner join duAn as d on c.DuAnId = d.DuAnId where t.PheDuyet=1 and d.KhoaChaId = " + duAnId + " group by t.NguoiDungId";
                return db.ExecuteQuery<Guid>(str).ToList();
            }
            catch
            {
                return null;
            }
        }


        public bool OverWriteTime(Guid nguoiDungId,DateTime startTime, DateTime endTime,bool? PheDuyet,byte loaiThoiGian,int? congViecId,string tokenId)
        {
            try
            {
                // Xóa hết các khoảng thời gian trong chuỗi.
                try
                {
                    var lstE = db.ThoiGianLamViecs.Where(t => t.NguoiDungId == nguoiDungId && t.ThoiGianBatDau >= startTime && t.ThoiGianBatDau <= endTime && t.ThoiGianKetThuc >= startTime && t.ThoiGianKetThuc <= endTime).ToList();
                    if (lstE != null && lstE.Count > 0) {
                        db.ThoiGianLamViecs.DeleteAllOnSubmit(lstE);                        
                    }                   
                }
                catch
                {
                }
                // Cắt khoảng thời gian có 1 phần time trong chuỗi
                try
                {
                    var E = db.ThoiGianLamViecs.Where(t => t.NguoiDungId == nguoiDungId && t.ThoiGianBatDau <= startTime && t.ThoiGianKetThuc > startTime && t.ThoiGianKetThuc < endTime).FirstOrDefault();
                    if (E != null)
                    {
                        E.ThoiGianKetThuc = startTime;
                        E.TongThoiGian =(int)(E.ThoiGianKetThuc - E.ThoiGianBatDau).Value.TotalSeconds;
                        db.SubmitChanges();
                    }                   
                }
                catch
                {
                }
                try
                {
                    var E = db.ThoiGianLamViecs.Where(t => t.NguoiDungId == nguoiDungId && t.ThoiGianBatDau > startTime && t.ThoiGianBatDau < endTime && t.ThoiGianKetThuc >= endTime).FirstOrDefault();
                    if (E != null)
                    {
                        E.ThoiGianBatDau = endTime;
                        E.TongThoiGian = (int)(E.ThoiGianKetThuc - E.ThoiGianBatDau).Value.TotalSeconds;
                        db.SubmitChanges();
                    }                   
                }
                catch
                {

                }
                var entity = new ThoiGianLamViec();
                entity.PheDuyet = PheDuyet;
                entity.NgayLamViec = startTime;
                entity.LoaiThoiGian = loaiThoiGian;
                entity.CongViecId = congViecId;
                entity.TokenId = tokenId;
                entity.TongThoiGian = (int)(endTime - startTime).TotalSeconds;
                entity.ThoiGianBatDau = startTime;
                entity.ThoiGianKetThuc = endTime;
                entity.NguoiDungId = nguoiDungId;
                db.ThoiGianLamViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public CongViecViewModel GetInfoOfTakForTimeBy(Guid nguoiDungId, int taskId)
        {
            try
            {
                var str = "select min(t.ThoiGianBatDau) as ThoiGianBatDau,max(t.ThoiGianKetThuc) as ThoiGianKetThuc, sum(t.TongThoiGian) as TongThoiGian from ThoiGianLamViec as t inner join CongViec as todo on t.CongViecId=todo.CongViecId inner join CongViec as task on todo.KhoaChaId=task.CongViecId where t.NguoiDungId='" + nguoiDungId.ToString() + "' and task.CongViecId=" + taskId;
                return db.ExecuteQuery<CongViecViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
           

        }
        public List<Guid> GetNguoiDungIdsSpendedTimeInTask(int taskId)
        {
            try
            {
                var str = "select t.NguoiDungId from ThoiGianLamViec as t  inner join CongViec as todo on t.CongViecId = todo.CongViecId inner join NguoiDung as n on t.NguoiDungId=n.NguoiDungId inner join CongViec as task on todo.KhoaChaId = task.CongViecId where task.CongViecId=" + taskId + " and t.ThoiGianBatDau is not null and t.ThoiGianKetThuc is not null group by t.NguoiDungId";
                return db.ExecuteQuery<Guid>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Guid> GetNguoiDungIdsStartTimeInTask(int taskId)
        {
            try
            {                
                var str = "select t.NguoiDungId from ThoiGianLamViec as t  inner join CongViec as todo on t.CongViecId = todo.CongViecId inner join NguoiDung as n on t.NguoiDungId=n.NguoiDungId inner join CongViec as task on todo.KhoaChaId = task.CongViecId where task.CongViecId=" + taskId + " and t.ThoiGianBatDau is not null and t.ThoiGianKetThuc is null group by t.NguoiDungId";
                return db.ExecuteQuery<Guid>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public int GetToTalTimeSpendInShip(int shipId)
        {
            try
            {
                var str = "select sum(tt.tongThoiGian) from CongViec as ship inner join  CongViec as task on ship.CongViecId=task.KhoaChaId inner join CongViec as todo on task.CongViecId=todo.KhoaChaId inner join ThoiGianLamViec as tt on todo.CongViecId = tt.CongViecId where tt.PheDuyet=1 and ship.CongViecId=" + shipId + " group by ship.CongViecId";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
        public int GetToTalTimeSpendInTask(int taskId)
        {
            try
            {
                var str = " select sum(tt.tongThoiGian) from CongViec as todo inner join ThoiGianLamViec as tt on todo.CongViecId = tt.CongViecId where tt.PheDuyet=1 and todo.KhoaChaId="+taskId+" group by todo.KhoaChaId ";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

       



        public List<AddThoiGianLamViecWithXMLViewModel> GetThoiGianBy(Guid nguoiDungId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var str = "select c.congViecId ,c.TenCongViec,n.TenDangNhap,t.LoaiThoiGian,c.KhoaChaId,k.TenCongViec as TenKhoaCha, t.ThoiGianBatDau,t.ThoiGianKetThuc,c.LoaiTimer,c.TrangThaiCongViecId,c.Tuan,c.Nam,c.IsToDoAdd,t.TongThoiGian, t.TokenId,t.NgayLamViec,t.PheDuyet,c.DuAnId,c.NgayTao,c.NgayDuKienHoanThanh from ThoiGianLamViec as t inner join congViec as c on t.CongViecId = c.CongViecId inner join congViec as k on c.KhoaChaId = k.congViecId inner join NguoiDung as n on t.NguoiDungId = n.NguoiDungId where t.NguoiDungId = '" + nguoiDungId.ToString()+"' and t.ThoiGianBatDau>='"+startTime.ToString("yyyy-MM-dd")+ " 00:00:00.000' and t.ThoiGianBatDau<'" + endTime.ToString("yyyy-MM-dd") + " 00:00:00.000' and t.ThoiGianKetThuc is not null";
                return db.ExecuteQuery<AddThoiGianLamViecWithXMLViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
           
        }


        public int GetTimeSpentOfDuAn(int duAnId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) as tongThoiGian from CongViec as c inner join ThoiGianLamViec as t on c.CongViecId=t.CongViecId where c.DuAnId="+duAnId;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
         
        }
        public int Insert(int? congViecId, Guid userId, DateTime? thoiGianBatDau, DateTime? thoiGianKetThuc, byte loaiThoiGian, int tongThoiGian, bool? pheDuyet, string noiDungLamViec, byte? trangThaiBaoViec, string tokenId, DateTime ngayLamViec)
        {
            try
            {
                ThoiGianLamViec entity = new ThoiGianLamViec();
                entity.CongViecId = congViecId;
                entity.NguoiDungId = userId;
                entity.LoaiThoiGian = loaiThoiGian;
                entity.ThoiGianBatDau = thoiGianBatDau;
                entity.ThoiGianKetThuc = thoiGianKetThuc;
                entity.TongThoiGian = tongThoiGian;
                entity.NgayLamViec = ngayLamViec;
                entity.PheDuyet = pheDuyet;
                entity.TokenId = tokenId;
                entity.NoiDungLamViec = noiDungLamViec;
                entity.TrangThaiBaoViec = trangThaiBaoViec;
                db.ThoiGianLamViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.ThoiGianLamViecId;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public int Insert2(int? congViecId, Guid userId, DateTime? thoiGianBatDau, DateTime? thoiGianKetThuc, byte loaiThoiGian, int tongThoiGian, bool? pheDuyet, string noiDungLamViec, byte? trangThaiBaoViec, string tokenId, DateTime ngayLamViec,byte? loaiNgayLamViec)
        {
            try
            {
                ThoiGianLamViec entity = new ThoiGianLamViec();
                entity.CongViecId = congViecId;
                entity.NguoiDungId = userId;
                entity.LoaiThoiGian = loaiThoiGian;
                entity.ThoiGianBatDau = thoiGianBatDau;
                entity.ThoiGianKetThuc = thoiGianKetThuc;
                entity.TongThoiGian = tongThoiGian;
                entity.NgayLamViec = ngayLamViec;
                entity.PheDuyet = pheDuyet;
                entity.TokenId = tokenId;
                entity.NoiDungLamViec = noiDungLamViec;
                entity.TrangThaiBaoViec = trangThaiBaoViec;
                entity.LoaiNgayLamViec = loaiNgayLamViec;
                db.ThoiGianLamViecs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.ThoiGianLamViecId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public bool PheDuyet(int toDoId)
        {
            try
            {
                var entitys = db.ThoiGianLamViecs.Where(t => t.CongViecId == toDoId&&(t.PheDuyet==false||t.PheDuyet==null));
                if (entitys != null && entitys.Count() > 0)
                {
                    foreach(var item in entitys)
                    {
                        item.PheDuyet = true;
                  }
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool UpdateStopBy(int congViecId, Guid userId)
        {
            try
            {
                var entity = db.ThoiGianLamViecs.Where(t => t.CongViecId == congViecId&&t.ThoiGianKetThuc==null).FirstOrDefault();
                var congViecE = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                var user = db.NguoiDungs.Where(t => t.NguoiDungId == userId).FirstOrDefault();
                var timeFromDefault = getDateTime(entity.ThoiGianBatDau.Value, user.KhungThoiGianBatDau);
                var timeToDefault = getDateTime(entity.ThoiGianBatDau.Value, user.KhungThoiGianKetThuc);
                var timeNghiTruaF = getDateTime(entity.ThoiGianBatDau.Value, "12:00");
                var timeNghiTruaT = getDateTime(entity.ThoiGianBatDau.Value, "13:00");
                bool ot = true;
                if (entity.ThoiGianBatDau.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")) entity.ThoiGianKetThuc = DateTime.Now;             
                else
                {
                    entity.ThoiGianKetThuc = getDateTime(entity.ThoiGianBatDau.Value, "23:59");
                    ot = false;
                }
                if (user.Quyen == 1 && entity.ThoiGianKetThuc > timeToDefault)
                {
                    if (entity.ThoiGianKetThuc > timeToDefault.AddMinutes(20)) 
                    entity.ThoiGianKetThuc = timeToDefault;
                }
                    if (timeFromDefault > entity.ThoiGianBatDau)
                {
                    if (entity.ThoiGianKetThuc < timeFromDefault)
                    {
                       db.ThoiGianLamViecs.DeleteOnSubmit(entity);
                        db.SubmitChanges();
                        return true;
                    }
                    else
                    {
                        //var endtime = entity.ThoiGianKetThuc;
                        //Insert(congViecId, userId, entity.ThoiGianBatDau, timeFromDefault.AddMinutes(-1),2, (int)(timeFromDefault - entity.ThoiGianBatDau).Value.TotalSeconds, false, "", 2, entity.TokenId, entity.ThoiGianBatDau.Value);
                        //entity.ThoiGianBatDau = timeFromDefault;
                        //Update(entity.ThoiGianLamViecId, timeFromDefault, 1, null, true);
                        //entity.ThoiGianKetThuc = endtime;
                        entity.ThoiGianBatDau = timeFromDefault;
                    }
                }
                else
                {
                    if (entity.ThoiGianKetThuc > timeToDefault)
                    {
                        if (entity.ThoiGianBatDau > timeToDefault)
                        {
                            return Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value,2, entity.ThoiGianKetThuc.Value, null);
                        }
                        else
                        {
                            if (ot) {
                                if (entity.ThoiGianKetThuc <= timeToDefault.AddMinutes(20)){
                                    Insert(congViecId, userId, timeToDefault.AddTicks(1), entity.ThoiGianKetThuc,1, (int)(entity.ThoiGianKetThuc - timeToDefault.AddTicks(1)).Value.TotalSeconds,true, "",null, entity.TokenId, entity.ThoiGianKetThuc.Value);
                                }
                                else
                                {
                                    Insert(congViecId, userId, timeToDefault.AddTicks(1), entity.ThoiGianKetThuc, 2, (int)(entity.ThoiGianKetThuc - timeToDefault.AddTicks(1)).Value.TotalSeconds, null, "", null, entity.TokenId, entity.ThoiGianKetThuc.Value);
                                }
                                }                         
                            entity.ThoiGianKetThuc = timeToDefault;
                        }
                    }
                }
                if ((entity.ThoiGianBatDau < timeNghiTruaF && entity.ThoiGianKetThuc < timeNghiTruaF) || (entity.ThoiGianBatDau > timeNghiTruaT))
                {
                    Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value,1, entity.ThoiGianKetThuc.Value, true);
                }
                else
                {
                    if (entity.ThoiGianBatDau < timeNghiTruaF && entity.ThoiGianKetThuc >= timeNghiTruaF)
                    {
                        var timeEnd = entity.ThoiGianKetThuc;
                        Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value,1, timeNghiTruaF, true);
                        if (timeEnd > timeNghiTruaT)
                        {
                            Insert(congViecId, userId, timeNghiTruaT, timeEnd, 1, (int)(timeEnd - timeNghiTruaT).Value.TotalSeconds, true, "", 2, entity.TokenId, timeEnd.Value);
                        }
                    }
                    else if(entity.ThoiGianBatDau>=timeNghiTruaF&&entity.ThoiGianBatDau<=timeNghiTruaT)
                    {
                        if (entity.ThoiGianKetThuc <= timeNghiTruaT)
                        {
                            db.ThoiGianLamViecs.DeleteOnSubmit(entity);
                        }
                        else
                        {
                            Update(entity.ThoiGianLamViecId, timeNghiTruaT,1, entity.ThoiGianKetThuc.Value, true);
                        }
                    }
                }
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        public bool UpdateStopBy2(int congViecId, Guid userId)
        {
            try
            {
                var entity = db.ThoiGianLamViecs.Where(t => t.CongViecId == congViecId && t.ThoiGianKetThuc == null).FirstOrDefault();
                var tokenId = entity.TokenId;
                var congViecE = db.CongViecs.Where(t => t.CongViecId == congViecId).FirstOrDefault();
                var user = db.NguoiDungs.Where(t => t.NguoiDungId == userId).FirstOrDefault();
                var timeFromDefault = getDateTime(entity.ThoiGianBatDau.Value, user.KhungThoiGianBatDau);
                var timeToDefault = getDateTime(entity.ThoiGianBatDau.Value, user.KhungThoiGianKetThuc);
                var timeNghiTruaF = getDateTime(entity.ThoiGianBatDau.Value, "12:00");
                var timeNghiTruaT = getDateTime(entity.ThoiGianBatDau.Value, "13:00");
                bool ot = true;
                if (entity.ThoiGianBatDau.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")) entity.ThoiGianKetThuc = DateTime.Now;
                else
                {
                    entity.ThoiGianKetThuc = getDateTime(entity.ThoiGianBatDau.Value, "23:59");
                    ot = false;
                }
                if (user.Quyen == 1 && entity.ThoiGianKetThuc > timeToDefault)
                {
                    if (entity.ThoiGianKetThuc > timeToDefault.AddMinutes(20))
                        entity.ThoiGianKetThuc = timeToDefault;
                }
                if (timeFromDefault > entity.ThoiGianBatDau)
                {
                    if (entity.ThoiGianKetThuc < timeFromDefault)
                    {
                        db.ThoiGianLamViecs.DeleteOnSubmit(entity);
                        db.SubmitChanges();
                        return true;
                    }
                    else
                    {                        
                        entity.ThoiGianBatDau = timeFromDefault;
                    }
                }
                else
                {
                    if (entity.ThoiGianKetThuc > timeToDefault)
                    {
                        if (entity.ThoiGianBatDau > timeToDefault)
                        {
                            return OverWriteTime(userId, entity.ThoiGianBatDau.Value, entity.ThoiGianKetThuc.Value, null, 2, congViecId, tokenId);
                            //return Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value, 2, entity.ThoiGianKetThuc.Value, null);
                        }
                        else
                        {
                            if (ot)
                            {
                                if (entity.ThoiGianKetThuc <= timeToDefault.AddMinutes(20))
                                {
                                    //Insert(congViecId, userId, timeToDefault.AddTicks(1), entity.ThoiGianKetThuc, 1, (int)(entity.ThoiGianKetThuc - timeToDefault.AddTicks(1)).Value.TotalSeconds, true, "", null, entity.TokenId, entity.ThoiGianKetThuc.Value);
                                    OverWriteTime(userId, timeToDefault, entity.ThoiGianKetThuc.Value, true, 1, congViecId, tokenId);
                                }
                                else
                                {
                                    //Insert(congViecId, userId, timeToDefault.AddTicks(1), entity.ThoiGianKetThuc, 2, (int)(entity.ThoiGianKetThuc - timeToDefault.AddTicks(1)).Value.TotalSeconds, null, "", null, entity.TokenId, entity.ThoiGianKetThuc.Value);
                                    OverWriteTime(userId, timeToDefault, entity.ThoiGianKetThuc.Value, null, 2, congViecId, tokenId);
                                }
                            }
                            entity.ThoiGianKetThuc = timeToDefault;
                        }
                    }
                }
                if ((entity.ThoiGianBatDau < timeNghiTruaF && entity.ThoiGianKetThuc < timeNghiTruaF) || (entity.ThoiGianBatDau > timeNghiTruaT))
                {
                    //Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value, 1, entity.ThoiGianKetThuc.Value, true);
                    OverWriteTime(userId, entity.ThoiGianBatDau.Value, entity.ThoiGianKetThuc.Value, true, 1, congViecId, tokenId);
                }
                else
                {
                    if (entity.ThoiGianBatDau < timeNghiTruaF && entity.ThoiGianKetThuc >= timeNghiTruaF)
                    {
                        var timeEnd = entity.ThoiGianKetThuc;
                        //Update(entity.ThoiGianLamViecId, entity.ThoiGianBatDau.Value, 1, timeNghiTruaF, true);
                        OverWriteTime(userId, entity.ThoiGianBatDau.Value, timeNghiTruaF, true, 1, congViecId, tokenId);
                        if (timeEnd > timeNghiTruaT)
                        {
                            //Insert(congViecId, userId, timeNghiTruaT, timeEnd, 1, (int)(timeEnd - timeNghiTruaT).Value.TotalSeconds, true, "", 2, entity.TokenId, timeEnd.Value);
                            OverWriteTime(userId, timeNghiTruaT, timeEnd.Value, true, 2, congViecId, tokenId);
                        }
                    }
                    else if (entity.ThoiGianBatDau >= timeNghiTruaF && entity.ThoiGianBatDau <= timeNghiTruaT)
                    {
                        if (entity.ThoiGianKetThuc <= timeNghiTruaT)
                        {
                            db.ThoiGianLamViecs.DeleteOnSubmit(entity);
                        }
                        else
                        {
                            //Update(entity.ThoiGianLamViecId, timeNghiTruaT, 1, entity.ThoiGianKetThuc.Value, true);
                            OverWriteTime(userId, timeNghiTruaT, entity.ThoiGianKetThuc.Value, true, 2, congViecId, tokenId);
                        }
                    }
                }
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateStopByV2(int congViecId, Guid userId,string apiTokenKeyLink)
        {
            try
            {
              
                var entity = db.ThoiGianLamViecs.Where(t => t.CongViecId == congViecId && t.ThoiGianKetThuc == null).FirstOrDefault();
                var strngayLamViec = entity.NgayLamViec.ToString("dd/MM/yyyy");
                UpdateStopBy(congViecId, userId);
                if (strngayLamViec != DateTime.Now.ToString("dd/MM/yyyy")) {                  
                    var ngayLamViec = entity.NgayLamViec.AddDays(1);
                    var nguoiDung = db.NguoiDungs.Where(t => t.NguoiDungId == userId).FirstOrDefault();                
                    while (ngayLamViec> DateTime.Now)
                    {
                        var endOfDay = ngayLamViec.AddDays(1).AddTicks(-1);
                        var token = GetTokenKey(userId, apiTokenKeyLink, ngayLamViec);
                        if(token!=null&& string.IsNullOrEmpty(token.key_token)&&token.manufacturing_date<DateTime.Now)
                        {                           
                            var startTime = token.manufacturing_date;
                            var endTime = DateTime.Now;
                            if (endTime > endOfDay) endTime = endOfDay;
                            Insert(congViecId, userId, startTime, null, 1, 0, null, null, null, token.key_token, ngayLamViec);
                            UpdateStopBy(congViecId, userId);
                        }
                        ngayLamViec = ngayLamViec.AddDays(1);
                    }
                }
                db.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private TokenViewModel GetTokenKey(Guid userId,string link,DateTime datetime)
        {
            try
            {
                var epoch = (datetime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                var client = new RestClient(link + "/" + userId.ToString()+"/"+epoch);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<TokenViewModel>(response.Content);
            }
            catch
            {
                return null;
            }

        }

        public List<ThoiGianLamViecViewModel> GetByCongViecId(int congViecId)
        {
            try
            {
                return db.ExecuteQuery<ThoiGianLamViecViewModel>("Select * from ThoiGianLamViec where CongViecId=" + congViecId).ToList();
            }
            catch
            {
                return null;
            }
        }
        public bool Update(int thoiGianLamViecId, DateTime thoiGianBatDau,byte loaiThoiGian, DateTime? thoiGianKetThuc, bool? pheDuyet)
        {
            try
            {
                var entity = db.ThoiGianLamViecs.Where(t => t.ThoiGianLamViecId == thoiGianLamViecId).FirstOrDefault();
                entity.ThoiGianBatDau = thoiGianBatDau;
                entity.ThoiGianKetThuc = thoiGianKetThuc;
                entity.LoaiThoiGian = loaiThoiGian;
                if(thoiGianKetThuc!=null)
                entity.TongThoiGian = (int)((thoiGianKetThuc - thoiGianBatDau).Value.TotalSeconds);
                entity.NgayLamViec = thoiGianBatDau;
                entity.PheDuyet = pheDuyet;
                if (thoiGianBatDau > thoiGianKetThuc) return false;
                if (String.Format("{0:dd/MM}", thoiGianBatDau) != String.Format("{0:dd/MM}", thoiGianKetThuc)) return false;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Lấy tổng thời gian đã chạy của todo đã được duyệt
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public int GetAllTimeSpentOfToDo(int todoId,bool?pheDuyet)
        {
            try
            {
                var str = "select SUM(TongThoiGian) from ThoiGianLamViec where CongViecId=" + todoId;
                if (pheDuyet != null && pheDuyet.Value)
                {
                    str += " and PheDuyet = 1 ";
                }
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public int GetAllTimeSpentByKhoaCha(int khoaChaId, bool? pheDuyet)
        {
            try
            {
                var str = "select SUM(TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId = c.congViecId where c.KhoaChaId=" + khoaChaId;
                if (pheDuyet != null && pheDuyet.Value)
                {
                    str += " and PheDuyet = 1 ";
                }
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public int GetTotalTimeOfToDoInDay(int toDoId, DateTime day, bool? pheDuyet, int loaiThoiGian)
        {
            try
            {
                var str = "select sum(TongThoiGian) from ThoiGianLamViec where CongViecId=" + toDoId + " and NgayLamViec='" + String.Format("{0:yyyy-MM-dd}", day) + "' and LoaiThoiGian=" + loaiThoiGian;
                if (pheDuyet != null && pheDuyet.Value)
                {
                    str += " and PheDuyet =1";
                }
                else
                {
                    str += " and (PheDuyet =0 or PheDuyet IS NULL)";
                }
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
           
           
        }
        public List<DateTime> GetDatesStartTimeOfTaskId(int taskId)
        {
            try
            {
                var str = "Select NgayLamViec from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where c.KhoaChaId="+ taskId + " group by NgayLamViec  order by NgayLamViec desc";
                return db.ExecuteQuery<DateTime>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách các todo trong tuần chưa done(được quyền start time) của user
        /// </summary>
        /// <param name="week"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetAllToDoNotDoneInWeekOfUser(int week, Guid userId)
        {
            try
            {
                var str = "select * from CongViec where Tuan= " + week + " and NguoiXuLyId='" + userId.ToString() + "' and (XacNhanHoanThanh IS NULL or (XacNhanHoanThanh IS NOT NULL and XacNhanHoanThanh=1))";
                return db.ExecuteQuery<CongViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy todo đang tính giờ của người dùng.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CongViecViewModel GetToDoStartTimeOfUser(Guid userId)
        {
            try
            {
                var str = "select * from CongViec as c inner join ThoiGianLamViec as t on c.CongViecId=t.CongViecId where t.ThoiGianBatDau IS NOT NULL and t.ThoiGianKetThuc IS NULL and t.NguoiDungId='" + userId.ToString() + "'";
                return db.ExecuteQuery<CongViecViewModel>(str).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public CongViecViewModel GetTaskStartTimeOfUser(Guid userId)
        {
            try
            {
                var str = "select c.CongViecId,c.TenCongViec from CongViec as c inner join CongViec as todo on c.congViecId =todo.KhoachaId inner join  ThoiGianLamViec as t on todo.CongViecId=t.CongViecId where t.ThoiGianBatDau IS NOT NULL and t.ThoiGianKetThuc IS NULL and t.NguoiDungId='" + userId.ToString() + "'";
                return db.ExecuteQuery<CongViecViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int GetTotalTimeUsedByUserInCurrentTask(int taskId, Guid userId)
        {
            try
            {
                
                    var str = "select sum(t.TongThoiGian) from CongViec as c inner join CongViec as todo on c.congViecId = todo.KhoachaId inner join  ThoiGianLamViec as t on todo.CongViecId = t.CongViecId where c.CongViecId= "+taskId+" and t.ThoiGianBatDau IS NOT NULL and t.ThoiGianKetThuc IS NOT NULL and t.NguoiDungId = '" + userId.ToString() + "' group by c.CongViecId";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public CongViecViewModel GetToDoStartTimeOfUser2(Guid userId)
        {
            try
            {
                return  (from c in db.CongViecs join t in db.ThoiGianLamViecs on c.CongViecId equals t.CongViecId where t.ThoiGianBatDau.HasValue && !t.ThoiGianKetThuc.HasValue && t.NguoiDungId == userId && !c.IsToDoAdd select new CongViecViewModel { CongViecId = c.CongViecId, TenCongViec = c.TenCongViec, KhoaChaId = c.KhoaChaId, DuAnId = c.DuAnId, TrangThaiCongViecId = c.TrangThaiCongViecId, IsToDoAdd = c.IsToDoAdd }).FirstOrDefault(); ;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ThoiGianLamViecViewModel GetById(int id)
        {
            try
            {
                var str = "Select * from ThoiGianLamViec where ThoiGianLamViecId=" + id;
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public ThoiGianLamViecViewModel GetThoiGianIsStartOfUser(Guid userId)
        {
            try
            {
                var str = "select * from ThoiGianLamViec where ThoiGianBatDau IS NOT NULL and ThoiGianKetThuc IS NULL and NguoiDungId='" + userId.ToString() + "'";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public int CurrentToDoId(Guid userId)
        {
            try
            {
                var str = "select CongViecId from ThoiGianLamViec where ThoiGianBatDau IS NOT NULL and ThoiGianKetThuc IS NULL and NguoiDungId='" + userId.ToString() + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private DateTime getDateTime(DateTime date, string time)
        {
            //string timeString = "11/12/2009 13:30:00.000";
            IFormatProvider culture = new CultureInfo("en-US", true);
            //DateTime dateVal = DateTime.ParseExact(timeString, "dd/MM/yyyy HH:mm:ss.fff", culture);


            var str = "";
            var day = date.Day;
            if (day < 10) str += "0" + day;
            else str += day;
            var month = date.Month;
            if (month < 10) str += "/0" + month;
            else str += "/" + month;
            str += "/" + date.Year + " " + time;
            str += ":00.000";
            return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm:ss.fff", culture);
        }
        public bool UpdatePheDuyetBy(int congViecId,DateTime ngayLamViec, byte loaiThoiGian,bool check)
        {
            try
            {
                var lst = db.ThoiGianLamViecs.Where(t => t.CongViecId == congViecId && t.LoaiThoiGian == loaiThoiGian && t.NgayLamViec == ngayLamViec);
                foreach(var item in lst)
                {
                    item.PheDuyet = check;
                }
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdatePheDuyetBy(int thoiGianLamViecId, string tokenId)
        {
            try
            {
                var item = db.ThoiGianLamViecs.Where(t => t.ThoiGianLamViecId == thoiGianLamViecId).FirstOrDefault();
                item.PheDuyet = true;
                item.TokenId = tokenId;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool HuyDuyet(int thoiGianLamViecId)
        {
            try
            {
                var item = db.ThoiGianLamViecs.Where(t => t.ThoiGianLamViecId == thoiGianLamViecId).FirstOrDefault();
                item.PheDuyet = false;              
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int GetAllTimeOfUserInDay(Guid user, DateTime ngayLamViec, int loaiThoiGian)
        {
            try
            {
                var str = "select sum(TongThoiGian) from ThoiGianLamViec where NguoiDungId='" + user.ToString() + "' and NgayLamViec='" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' and PheDuyet =1 and LoaiThoiGian=" + loaiThoiGian;
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return 0;
            }
            
        }
        public int GetTimePheDuyetInRankTimeInDay(Guid userId,DateTime ngayLamViec)
        {
            try
            {
                int count = 0;
                var user = db.NguoiDungs.Where(t => t.NguoiDungId == userId).FirstOrDefault();
                var timeFromDefault = getDateTime(ngayLamViec, user.KhungThoiGianBatDau);
                var timeToDefault = getDateTime(ngayLamViec, user.KhungThoiGianKetThuc);
                var lst = (from t in db.ThoiGianLamViecs where t.NguoiDungId == userId && t.PheDuyet == true && ((t.ThoiGianBatDau >= timeFromDefault && t.ThoiGianBatDau <= t.ThoiGianBatDau) || (t.ThoiGianKetThuc >= timeFromDefault && t.ThoiGianKetThuc <= timeToDefault) || (t.ThoiGianBatDau < timeFromDefault && t.ThoiGianKetThuc > timeToDefault)) select new ThoiGianLamViecViewModel() { ThoiGianLamViecId=t.ThoiGianLamViecId,ThoiGianBatDau=t.ThoiGianBatDau,ThoiGianKetThuc=t.ThoiGianKetThuc,NgayLamViec=t.NgayLamViec,NguoiDungId=t.NguoiDungId,TongThoiGian=t.TongThoiGian,PheDuyet=t.PheDuyet}).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach(var item in lst)
                    {
                        var start = item.ThoiGianBatDau;
                        var end = item.ThoiGianKetThuc;
                        if (start < timeFromDefault) start = timeFromDefault;
                        if (end > timeToDefault) end = timeToDefault;
                        if (start < end) count += (int)(end.Value - start.Value).TotalSeconds;
                    }
                }
                return count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Lấy tổng thời gian(s) của người dùng (nguoiDungId) trong 1 dự án (duAnId) trong một khoảng thời gian nhất định(startDate-endDate)
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="DuAnId"></param>
        /// <param name="startDate"></param>
        /// <param name="dateTime"></param>
        /// CreateByCuongVh
        /// <returns>int</returns>
        public int GetTimeInProjectBy(Guid nguoiDungId, int DuAnId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where t.NguoiDungId='"+nguoiDungId.ToString()+"' and c.DuAnId="+DuAnId+" and t.PheDuyet=1 and t.NgayLamViec>='"+startDate.ToString("yyyy-MM-dd")+"' and t.NgayLamViec<='"+ endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public int GetAllTimeInProjectBy(int DuAnId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where c.DuAnId=" + DuAnId + " and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public int GetAllTimeOfUserBy(Guid nguoiDungId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }




        /// <summary>
        /// Lấy thời gian chạy giờ của các loại công việc trong các dự án theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetTimeOfLoaiCongViecInProjectBy(int duAnId, DateTime startDate, DateTime endDate, int loaiCongViecId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId inner join LienKetLoaiCongViec as l on c.CongViecId=l.CongViecId where l.LoaiCongViecId=" + loaiCongViecId+ " and c.DuAnId=" + duAnId + " and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// Lấy thời gian chạy giờ của các loại công việc của một người theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetTimeOfLoaiCongViecWithUserBy(Guid nguoiDungId, DateTime startDate, DateTime endDate, int loaiCongViecId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId inner join LienKetLoaiCongViec as l on c.CongViecId=l.CongViecId where l.LoaiCongViecId=" + loaiCongViecId + " and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// Lấy thời gian chạy giờ của các loại công việc của một người trong một dự án theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetTimeOfLoaiCongViecWithUserProjectBy(Guid nguoiDungId,int duAnId, DateTime startDate, DateTime endDate, int loaiCongViecId)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId inner join LienKetLoaiCongViec as l on c.CongViecId=l.CongViecId where l.LoaiCongViecId=" + loaiCongViecId + " and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1 and c.DuAnId="+duAnId+" and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// Lấy tổng thời gian chạy giờ trong một dự án theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetTimeUserInProjectBy(Guid nguoiDungId, int duAnId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where  t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1 and c.DuAnId=" + duAnId + " and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }
        /// <summary>
        /// Lấy thời gian chạy ngoài giờ của dự án theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetOutTimeOfProjectBy(int duAnId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where t.LoaiThoiGian=3 and c.DuAnId=" + duAnId + " and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }
        /// <summary>
        /// Lấy thời gian chạy ngoài giờ của người dùng theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetOutTimeOfUserBy(Guid nguoiDungId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where t.LoaiThoiGian=3 and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1 and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }
        /// <summary>
        /// Lấy thời gian chạy ngoài giờ của người trong project dùng theo khoảng time
        /// </summary>
        /// <param name="duAnId"></param>       
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="loaiCongViecId"></param>
        /// <returns></returns>
        public int GetOutTimeOfUserProjectBy(Guid nguoiDungId, int duAnId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var str = "select sum(t.TongThoiGian) from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId where t.LoaiThoiGian=3 and t.NguoiDungId='" + nguoiDungId.ToString() + "' and t.PheDuyet=1  and c.DuAnId="+duAnId+" and t.NgayLamViec>='" + startDate.ToString("yyyy-MM-dd") + "' and t.NgayLamViec<='" + endDate.ToString("yyyy-MM-dd") + "'";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }

        }

        public bool CheckStartTime(Guid userId,DateTime time)
        {
            try
            {
                var str = "select * from ThoiGianLamViec where ThoiGianBatDau<='" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", time) + "' and ThoiGianKetThuc>='" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", time) + "' and NguoiDungId='"+userId.ToString()+"'";
                var lst = db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
                if (lst != null && lst.Count > 0) return false;
                return true;
            }
            catch
            {
                return true;
            }
        }

        public bool CheckInsertTime(DateTime startTime, DateTime endTime, Guid userId)
        {
            try
            {
                
                var str = "select * from ThoiGianLamViec where ((ThoiGianBatDau > '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", startTime) + "' and ThoiGianKetThuc< '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", endTime) + "') or(ThoiGianBatDau > '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", startTime) + "' and ThoiGianBatDau<'" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", endTime) + "' and ThoiGianKetThuc > '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", endTime) + "') or(ThoiGianBatDau < '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", startTime) + "' and ThoiGianKetThuc < '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", endTime) + "' and ThoiGianKetThuc> '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", startTime) + "')or(ThoiGianBatDau < '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", startTime) + "' and  ThoiGianKetThuc > '" + String.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", endTime) + "'))and NguoiDungId = '"+userId.ToString()+"' and NgayLamViec = '" + String.Format("{0:yyyy-MM-dd}", startTime) + "'";
                var lst = db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
                if (lst != null && lst.Count > 0) return false;
                return true;
            }
            catch
            {
                return true;
            }
        }
        public bool CheckInsertTime2(DateTime startTime, DateTime endTime, Guid userId)
        {
            try
            {

                var count = (from t in db.ThoiGianLamViecs where (((startTime >= t.ThoiGianBatDau && startTime <= t.ThoiGianKetThuc) || (endTime >= t.ThoiGianBatDau && endTime <= t.ThoiGianKetThuc)) && t.NguoiDungId == userId) select t.ThoiGianLamViecId).Count();
                if (count > 0) return false;
                return true;
            }
            catch
            {
                return true;
            }
        }

        public string GetTokenKeyBy(Guid userId, DateTime date)
        {
            try
            {
                var str = "select TokenId from ThoiGianLamViec where NguoiDungId = '" + userId.ToString() + "' and NgayLamViec = '" + String.Format("{0:yyyy-MM-dd}", date) + "' group by TokenId";
                return db.ExecuteQuery<string>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<ThoiGianLamViecViewModel> GetDanhSachChoDuyetsBy(Guid userId) {
            try
            {
                var str = "select * from ThoiGianLamViec where NguoiDungId='"+userId.ToString()+"' and PheDuyet IS NULL and TongThoiGian >0";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<DateTime> GetNgayChoDuyetsBy(Guid userId)
        {
            try
            {
                var str = "select NgayLamViec from ThoiGianLamViec where NguoiDungId='" + userId.ToString() + "' and PheDuyet IS NULL and TongThoiGian >0 group by NgayLamViec order by NgayLamViec desc";
                return db.ExecuteQuery<DateTime>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public bool Delete(int thoiGianLamViecId)
        {
            try
            {
                var item = db.ThoiGianLamViecs.Where(t => t.ThoiGianLamViecId == thoiGianLamViecId).FirstOrDefault();
                db.ThoiGianLamViecs.DeleteOnSubmit(item);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckIsExitDayType(Guid userId, DateTime ngayLamViec, byte loaiNgayLamViec)
        {
            try
            {
                var str = "select * from ThoiGianLamViec where NguoiDungId='" + userId.ToString() + "' and NgayLamViec='" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' and LoaiNgayLamViec=" + loaiNgayLamViec;
                var lst = db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
                if (lst != null && lst.Count > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ThoiGianLamViecViewModel GetAddDayTypeOfUser(Guid userId, DateTime ngayLamViec)
        {
            try
            {
                var str = "select * from ThoiGianLamViec where NguoiDungId = '"+userId.ToString()+ "' and NgayLamViec = '" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' and LoaiNgayLamViec is not null and LoaiNgayLamViec!= 2 and LoaiNgayLamViec!= 1";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public List<ThoiGianLamViecViewModel> GetDanhSachLamViecOfUserBy(Guid userId,DateTime ngayLamViec)
        {
            try
            {
                var str = "select * from ThoiGianLamViec as t left join CongViec as c on t.CongViecId=c.CongViecId where t.NguoiDungId = '" + userId.ToString() + "' and t.NgayLamViec = '" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' and t.TongThoiGian>0";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public List<ThoiGianLamViecViewModel> GetDanhSachLamViecOfUserBy2(Guid userId, DateTime ngayLamViec)
        {
            try
            {
                var str = "select t.ThoiGianLamViecId,t.ThoiGianBatDau,t.ThoiGianKetThuc,todo.TenCongViec,ship.TenCongViec as TenShipAble,ship.DuAnId,t.PheDuyet from ThoiGianLamViec as t left join CongViec as c on t.CongViecId=c.CongViecId left join CongViec as todo on todo.CongViecId=c.KhoaChaId left join CongViec as ship on todo.KhoaChaId=ship.CongViecId  where t.NguoiDungId = '" + userId.ToString() + "' and t.NgayLamViec = '" + String.Format("{0:yyyy-MM-dd}", ngayLamViec) + "' and t.TongThoiGian>0";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<ThoiGianLamViecViewModel> GetThoiGianLamViecsBy(Guid nguoiDungId,DateTime startDate,DateTime endDate)
        {
            try
            {
                var str = "select c.KhoaChaId as CongViecId,sum(tongthoiGian) as TongThoiGian from ThoiGianLamViec as t inner join CongViec as c on t.CongViecId=c.CongViecId  where t.NgayLamViec>='"+startDate.ToString("yyyy-MM-dd")+"' and t.NgayLamViec<='"+endDate.ToString("yyyy-MM-dd")+"' and t.NguoiDungId='"+nguoiDungId.ToString()+"' and t.PheDuyet=1 and t.CongViecId>0  group by c.KhoaChaId";
                return db.ExecuteQuery<ThoiGianLamViecViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
