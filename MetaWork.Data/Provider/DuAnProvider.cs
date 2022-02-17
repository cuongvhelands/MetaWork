using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class DuAnProvider
    {
        TimerDataContext db = null;
        public DuAnProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }

        public DuAnViewModel GetByIdO1(int duAnId)
        {
            try
            {
                return (from a in db.DuAns where a.DuAnId == duAnId select new DuAnViewModel { DuAnId = duAnId, TenDuAn = a.TenDuAn }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }


        public List<DuAnViewModel> GetAllDuAn()
        {
                try
                {
                    return (from d in db.DuAns                        
                            where d.KhoaChaId == null
                            select new DuAnViewModel() { DuAnId = d.DuAnId, KhoaChaId = d.KhoaChaId, TenDuAn = d.TenDuAn }).ToList();
                }
                catch
                {
                    return null;
                }
          
        }


        public string GetMaDuAnForm(string ma)
        {
            
                int count = 1;
                string maDB = ma;
                while (IsExistDuAnByMa(maDB))
                {
                    maDB = ma+ count;
                    count++;
                }
                return maDB;
           
        }

        public bool IsExistDuAnByMa(string ma)
        {
            try
            {
                var count = (from d in db.DuAns where d.MaDuAn.Trim().ToUpper().Equals(ma.Trim().ToUpper()) select d.DuAnId).Count();
                if (count > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }


        public bool InsertLienKetDuAnPhongBan(int duAnId, int phongBanId)
        {
            try
            {
                LienKetDuAnPhongBan entity = new LienKetDuAnPhongBan();
                entity.DuAnId = duAnId;
                entity.PhongBanId = phongBanId;
                db.LienKetDuAnPhongBans.InsertOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteLienKetDuAnPhongBan(int duAnId, int phongBanId)
        {
            try
            {
                var entity = db.LienKetDuAnPhongBans.Where(t => t.DuAnId == duAnId && t.PhongBanId == phongBanId).FirstOrDefault();
                db.LienKetDuAnPhongBans.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<LienKetDuAnPhongBanViewModel> GetLKDuAnPhongBanByDuAnId(int duAnId)
        {
            try
            {
                return (from l in db.LienKetDuAnPhongBans
                        join d in db.DuAns on l.DuAnId equals d.DuAnId
                        join p in db.PhongBans on l.PhongBanId equals p.PhongBanId

                        where d.DuAnId == duAnId
                        select new LienKetDuAnPhongBanViewModel
                        {
                            DuAnId = d.DuAnId,
                            TenPhongBan = p.TenPhongBan,
                            PhongBanId = l.PhongBanId
                        }).ToList();
            }
            catch
            {
                return null;
            }
           
        }
        public List<int> GetPhongBanIdsByDuAnId(int duAnId)
        {
            try
            {
                return (from l in db.LienKetDuAnPhongBans
                        where l.DuAnId == duAnId
                        select l.PhongBanId).ToList();
            }
            catch
            {
                return null;
            }

        }
        public List<DuAnViewModel> GetDuAnThanhPhanByKhoaCha(int duAnId)
        {
            try
            {
                return (from d in db.DuAns
                        join c in db.DuAns on d.KhoaChaId equals c.DuAnId
                        join n in db.NguoiDungs on d.NguoiQuanLyId equals n.NguoiDungId into gj from x in gj.DefaultIfEmpty()
                        where d.KhoaChaId == duAnId
                        select new DuAnViewModel() { DuAnId = d.DuAnId, KhoaChaId = d.KhoaChaId, TenDuAn = d.TenDuAn, TenKhoaCha = c.TenDuAn,MaDuAn=d.MaDuAn, HoTenNguoiQuanLy = (x == null ? String.Empty : x.HoTen), NguoiQuanLyId = d.NguoiQuanLyId, AvatarNguoiQuanLy = (x == null ? String.Empty : x.Avatar), NgayTao = d.NgayTao }).ToList();
            }
            catch
            {
                return null;
            }
        }

        public DuAnViewModel GetDuAnThanhPhanBy(int khoaChaId,string tenDuAn)
        {
            try
            {
                return (from d in db.DuAns                     
                        where d.KhoaChaId == khoaChaId && d.TenDuAn.Trim().ToUpper().Equals(tenDuAn.Trim().ToUpper())
                        select new DuAnViewModel() { DuAnId = d.DuAnId, KhoaChaId = d.KhoaChaId, TenDuAn = d.TenDuAn, MaDuAn = d.MaDuAn, NgayTao = d.NgayTao }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public string GetMaDuAnById(int duAnId)
        {
            try
            {
                return (from d in db.DuAns where d.DuAnId == duAnId select d.MaDuAn).FirstOrDefault();
            }
            catch
            {
                return "";
            }
        }
        public List<DuAnViewModel> GetsBy(int type, List<Guid> nguoiDungIds, List<int> status)
        {
            try
            {
                return (from d in db.DuAns
                        join dc in db.DuAns on d.KhoaChaId equals dc.DuAnId
                        join di in db.DuAnInfos on d.DuAnId equals di.DuAnId
                        join l in db.LoaiDuAns on d.LoaiDuAnId equals l.LoaiDuAnId
                        join t in db.TrangThaiDuAns on d.TrangThaiDuAnId equals t.TrangThaiDuAnId
                        join n in db.NguoiDungs on d.NguoiQuanLyId equals n.NguoiDungId
                        where (type == 0 || d.LoaiDuAnId == type)
                        && (nguoiDungIds == null || nguoiDungIds.Count == 0 || nguoiDungIds.Contains(d.NguoiQuanLyId))
                        && (status == null || status.Count == 0 || status.Contains(d.TrangThaiDuAnId))
                        select new DuAnViewModel { DuAnId = d.DuAnId,TenDuAn=d.TenDuAn,TrangThaiDuAnId=d.TrangThaiDuAnId,TenTrangThaiDuAn=t.TenTrangThaiDuAn,MaMauTrangThaiDuAn=t.MaMau,TenLoaiDuAn=l.TenLoaiDuAn,LoaiDuAnId=l.LoaiDuAnId,MaMau=d.MaDuAn,TenKhoaCha=dc.TenDuAn,KhoaChaId=d.KhoaChaId,HoTenNguoiQuanLy=n.HoTen,AvatarNguoiQuanLy=n.Avatar,NguoiQuanLyId=d.NguoiQuanLyId, CostTien=di.CostTien??0,TongNganSach=di.TongNganSach,TenHoatDong=d.TenHoatDong,GhiChu=d.GhiChu,MoTa=d.MoTa ,TenVietTat=d.TenVietTat}).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public List<int> GetsBy(Guid userId)
        {
            try
            {
                var str = "select dac.DuAnId from DuAn as dac join DuAn as d on d.KhoaChaId = dac.DuAnId left join LienKetDuAnPhongBan as l on dac.DuAnId=l.DuAnId left join PhongBan as p on l.PhongBanId = p.PhongBanId left join LienKetNguoiDungPhongBan as lkP on lkP.PhongBanId=p.PhongBanId where d.NguoiQuanLyId=N'" + userId.ToString() + "' or lkP.NguoiDungId=N'" + userId.ToString() + "' group by dac.DuAnId ";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<int> GetDuAnTpsBy(Guid userId)
        {
            try
            {
                var str = "select d.DuAnId from DuAn as dac join DuAn as d on d.KhoaChaId = dac.DuAnId left join LienKetDuAnPhongBan as l on dac.DuAnId=l.DuAnId left join PhongBan as p on l.PhongBanId = p.PhongBanId left join LienKetNguoiDungPhongBan as lkP on lkP.PhongBanId=p.PhongBanId where d.NguoiQuanLyId=N'" + userId.ToString() + "' or lkP.NguoiDungId=N'" + userId.ToString() + "' group by d.DuAnId ";
                return db.ExecuteQuery<int>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DuAnViewModel> GetAll()
        {
            try
            {
                var str = "Select * from DuAn where LuuTru=0";
                return db.ExecuteQuery<DuAnViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public DuAnViewModel GetDuAnByName(string name)
        {
            try
            {
                return (from a in db.DuAns where a.TenDuAn.Trim().ToUpper().Equals(name.ToUpper()) && a.KhoaChaId == null select new DuAnViewModel { DuAnId = a.DuAnId, TenDuAn = a.TenDuAn }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public DuAnViewModel GetDuAnByName2(string name)
        {
            try
            {
                return (from a in db.DuAns where a.TenVietTat.Trim().ToUpper().Equals(name.Trim().ToUpper()) select new DuAnViewModel { DuAnId = a.DuAnId, TenDuAn = a.TenDuAn,KhoaChaId=a.KhoaChaId }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public List<DuAnViewModel> GetAll2()
        {
            try
            {
                var str = "Select * from DuAn order by TenDuAn";
                return db.ExecuteQuery<DuAnViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<DuAnViewModel> GetsBy()
        {
            try
            {
                var str = "select d.DuAnId,d.TenDuAn,d.MaDuAn,d.NgayBatDau,l.TenLoaiNganSachVietTat,di.TongNganSach,d.TrangThaiDuAnId,tt.TenTrangThaiDuAn,tt.MaMau,d.LoaiNganSachId from DuAn as d inner join DuAnInfo as di on d.DuAnId=di.DuAnId inner join LoaiNganSach as l on d.LoaiNganSachId=l.LoaiNganSachId inner join TrangThaiDuAn as tt on d.TrangThaiDuAnId=tt.TrangThaiDuAnId where d.TrangThai=1 ";
                return db.ExecuteQuery<DuAnViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DuAnViewModel> GetsBy(int? type, string keyWord)
        {
            try
            {
                var str = "select d.DuAnId,k.TenKhachHang,d.LuuTru,d.QuanTam,d.TenDuAn,d.MaDuAn,d.NgayBatDau,l.TenLoaiNganSachVietTat,di.TongNganSach,d.TrangThaiDuAnId,tt.TenTrangThaiDuAn,tt.MaMau as MaMauTrangThaiDuAn,d.LoaiNganSachId from DuAn as d inner join DuAnInfo as di on d.DuAnId=di.DuAnId inner join LoaiNganSach as l on d.LoaiNganSachId=l.LoaiNganSachId inner join TrangThaiDuAn as tt on d.TrangThaiDuAnId=tt.TrangThaiDuAnId left join KhachHang as k on d.KhachHangId=k.KhachHangId where d.TrangThai=1";
                if (!string.IsNullOrEmpty(keyWord))
                {
                    str += " and (d.TenDuAn like N'" + keyWord + "' or d.MaDuAn like N'" + keyWord + "')";
                }
                if (type == 1) str += " and d.QuanTam=1";
                if (type == 2) str += " and d.LuuTru=0";
                if (type == 3) str += " and d.LuuTru=1";
                return db.ExecuteQuery<DuAnViewModel>(str).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<DuAnViewModel> GetsByTrangThaiDuAn(int statusId)
        {
            try
            {
                var str = "select * from DuAn where KhoaChaId is not null and TrangThaiDuAnId = " + statusId;
                return db.ExecuteQuery<DuAnViewModel>(str).ToList();
            }
            catch
            {
                return null;
            }
        }
        public DuAnViewModel GetById(int duAnId, Guid userId)
        {
            try
            {
                var str = "select  d.TenDuAn,d.MoTa,d.KhachHangId, d.DuAnId,d.MaDuAn,di.TongNganSach,d.TrangThaiDuAnId,n.HoTen as HoTenNguoiQuanLy,n.Avatar as AvatarNguoiQuanLy,tt.TenTrangThaiDuAn,tt.MaMau as MaMauTrangThaiDuAn,d.QuanTam,d.NgayBatDau,d.NgayKetThuc from DuAn as d left join NguoiDung as n on d.NguoiQuanLyId=n.NguoiDungId inner join TrangThaiDuAn as tt on d.TrangThaiDuAnId=tt.TrangThaiDuAnId left join DuAnInfo as di on d.DuAnId=di.DuAnId left join LienKetNguoiDungDuAn as lk on d.DuAnId=lk.DuAnId where d.TrangThai=1 and d.DuAnId=" + duAnId;
                return db.ExecuteQuery<DuAnViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public DuAnViewModel GetById(int duAnId)
        {
            try
            {
                var str = "select d.LoaiNganSachId,d.QuanTam, d.TenDuAn,d.MoTa,d.GhiChu,d.KhachHangId,d.KhoaChaId, d.DuAnId,d.MaDuAn,di.CostTien,di.TongNganSach,d.TrangThaiDuAnId,d.NguoiQuanLyId,d.NgayBatDau,d.NgayKetThuc from DuAn as d inner join DuAnInfo as di on d.DuAnId=di.DuAnId inner join LoaiNganSach as l on d.LoaiNganSachId=l.LoaiNganSachId where d.DuAnId=" + duAnId;
                return db.ExecuteQuery<DuAnViewModel>(str).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public DuAnViewModel GetTenDuAnById(int duAnId)
        {
            try
            {
                var str = "select TenDuAn, DuAnId,KhoaChaId,TenVietTat from DuAn where DuAnId=" + duAnId;
                return db.ExecuteQuery<DuAnViewModel>(str).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DuAnViewModel GetDuAnThanhPhanById(int duAnId)
        {
            try
            {
                return (from d in db.DuAns join di in db.DuAnInfos on d.DuAnId equals di.DuAnId
                          join t in db.TrangThaiDuAns on d.TrangThaiDuAnId equals t.TrangThaiDuAnId
                          join c in db.DuAns on d.KhoaChaId equals c.DuAnId
                          join n in db.NguoiDungs on c.NguoiQuanLyId equals n.NguoiDungId
                          where d.DuAnId==duAnId
                          select new DuAnViewModel{ DuAnId=d.DuAnId,TenDuAn=d.TenDuAn,TrangThaiDuAnId=d.TrangThaiDuAnId,TenTrangThaiDuAn=t.TenTrangThaiDuAn,MaMauTrangThaiDuAn=t.MaMau,NguoiQuanLyId=d.NguoiQuanLyId,HoTenNguoiQuanLy=n.HoTen,AvatarNguoiQuanLy=n.Avatar,TenKhoaCha=c.TenDuAn,KhoaChaId=d.KhoaChaId,GhiChu=d.GhiChu,MoTa=d.MoTa,CostTien=di.CostTien??0,TongNganSach=di.TongNganSach,NgayTao=d.NgayTao}
                          ).FirstOrDefault();
                
            }
            catch
            {
                return null;
            }
        }

        public int Insert(DuAnViewModel vm, Guid userId)
        {
            try
            {
                DuAn entity = new DuAn();
                entity.KhoaChaId = vm.KhoaChaId;
                entity.KhachHangId = vm.KhachHangId;
                entity.LoaiDuAnId = vm.LoaiDuAnId;
                entity.TenDuAn = vm.TenDuAn;
                entity.MaDuAn = vm.MaDuAn;
                entity.TenVietTat = vm.TenVietTat;
                if(!string.IsNullOrEmpty(vm.MoTa))
                entity.MoTa = vm.MoTa;
                if (!string.IsNullOrEmpty(vm.GhiChu))
                    entity.GhiChu = vm.GhiChu;
                entity.TenHoatDong = vm.TenHoatDong;

                entity.TagLine = vm.TagLine;
                entity.ThongTinChiTiet = vm.ThongTinChiTiet;
                if (vm.NguoiQuanLyId != Guid.Empty)
                    entity.NguoiQuanLyId = vm.NguoiQuanLyId;
                else entity.NguoiQuanLyId = userId;
                entity.TrangThaiDuAnId = vm.TrangThaiDuAnId;
                entity.NgayTao = DateTime.Now;
                if (vm.LoaiNganSachId == 0) entity.LoaiNganSachId = (int)EnumLoaiNganSachType.MacDinh;
                else entity.LoaiNganSachId = vm.LoaiNganSachId;
                entity.LuuTru = vm.LuuTru;
                if (vm.TrangThaiDuAnId == 0) entity.TrangThaiDuAnId = (int)EnumTrangThaiDuAnType.MacDinh;
                else 
                entity.TrangThaiDuAnId = vm.TrangThaiDuAnId;
                entity.NgayBatDau = vm.NgayBatDau;
                entity.NgayKetThuc = vm.NgayKetThuc;
                entity.QuanTam = vm.QuanTam;
                entity.TrangThai = true;
                db.DuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                if (entity.DuAnId > 0)
                {
                    if (vm.GiaiDoanDuAns != null && vm.GiaiDoanDuAns.Count > 0)
                    {
                        foreach (var item in vm.GiaiDoanDuAns)
                        {
                            GiaiDoanDuAn entity2 = new GiaiDoanDuAn();
                            entity2.DuAnId = entity.DuAnId;
                            entity2.GhiChu = item.GhiChu;
                            entity2.MoTa = item.MoTa;
                            entity2.NgayTao = DateTime.Now;
                            entity2.TenGiaiDoan = item.TenGiaiDoan;
                            entity2.ThoiGianBatDau = item.ThoiGianBatDau;
                            entity2.ThoiGianKetThuc = item.ThoiGianKetThuc;
                            entity2.TrangThai = true;
                            entity2.LoaiGiaiDoanId = item.LoaiGiaiDoanId;
                            entity2.TrangThaiHienTai = item.TrangThaiHienTai;
                            db.GiaiDoanDuAns.InsertOnSubmit(entity2);
                            db.SubmitChanges();
                            if (item.HangMucCongViecs != null && item.HangMucCongViecs.Count > 0)
                            {
                                foreach (var item2 in item.HangMucCongViecs)
                                {
                                    HangMucCongViec entity3 = new HangMucCongViec();
                                    entity3.TenHangMuc = item2.TenHangMuc;
                                    entity3.TrangThai = item2.TrangThai;
                                    entity3.NgayCapNhat = DateTime.Now;
                                    entity3.NguoiCapNhatId = userId;
                                    entity3.GiaiDoanDuAnId = entity2.GiaiDoanDuAnId;
                                    db.HangMucCongViecs.InsertOnSubmit(entity3);
                                }
                                db.SubmitChanges();
                            }

                        }
                    }
                    DuAnInfo duAnInfoE = new DuAnInfo();
                    duAnInfoE.DuAnId = entity.DuAnId;
                    duAnInfoE.ChoPhepVuotQua = vm.ChoPhepVuotQua;
                    duAnInfoE.TongNganSach = vm.TongNganSach;
                    duAnInfoE.CostTien = vm.CostTien;
                    duAnInfoE.MucCanhBao = vm.MucCanhBao;
                    db.DuAnInfos.InsertOnSubmit(duAnInfoE);
                    if (vm.LoaiCongViecIds != null && vm.LoaiCongViecIds.Count > 0)
                    {
                        foreach (var item in vm.LoaiCongViecIds)
                        {
                            LienKetLoaiCongViecDuAn lkE = new LienKetLoaiCongViecDuAn();
                            lkE.DuAnId = entity.DuAnId;
                            lkE.LoaiCongViecId = item;
                            db.LienKetLoaiCongViecDuAns.InsertOnSubmit(lkE);
                        }
                        db.SubmitChanges();
                    }
                    if (vm.LienKetNguoiDungDuAn != null && vm.LienKetNguoiDungDuAn.Count > 0)
                    {
                        foreach (var item in vm.LienKetNguoiDungDuAn)
                        {
                            LienKetNguoiDungDuAn lkE = new LienKetNguoiDungDuAn();
                            lkE.DuAnId = entity.DuAnId;
                            lkE.NguoiDungId = item.NguoiDungId;
                            lkE.LaQuanLy = item.LaQuanLy;
                            lkE.NgayThamGia = DateTime.Now;
                            db.LienKetNguoiDungDuAns.InsertOnSubmit(lkE);
                        }
                        db.SubmitChanges();
                    }
                }
                db.SubmitChanges();
                return entity.DuAnId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertLoaiDuAn(string tenLoai)
        {
            try
            {
                LoaiDuAn entity = new LoaiDuAn();
                entity.TenLoaiDuAn = tenLoai;
                db.LoaiDuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.LoaiDuAnId;
            }
            catch
            {
                return 0;
            }
        }

        public LoaiDuAnViewModel GetLoaiDuAnByName(string name)
        {
            try
            {
                return (from a in db.LoaiDuAns where a.TenLoaiDuAn.Equals(name)  select new LoaiDuAnViewModel { LoaiDuAnId = a.LoaiDuAnId, TenLoaiDuAn = a.TenLoaiDuAn }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public List<LoaiDuAnViewModel> GetAllLoaiDuAn()
        {
            try
            {
                return (from a in db.LoaiDuAns  select new LoaiDuAnViewModel { LoaiDuAnId = a.LoaiDuAnId, TenLoaiDuAn = a.TenLoaiDuAn }).ToList();
            }
            catch
            {
                return null;
            }
        }

        #region LienKetNguoiDungDuAn
        public List<LienKetNguoiDungDuAnViewModel> GetlkNguoiDungDuAnByDuAnId(int duAnId)
        {
            try
            {
                return db.ExecuteQuery<LienKetNguoiDungDuAnViewModel>("Select * from LienKetNguoiDungDuAn as l inner join nguoiDung as n on l.NguoiDungId=n.NguoiDungId where DuAnId=" + duAnId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteLienKetNguoiDungDuAn(int duAnId, Guid nguoiDungId)
        {
            try
            {
                var entity = db.LienKetNguoiDungDuAns.Where(t => t.NguoiDungId == nguoiDungId && t.DuAnId == duAnId).FirstOrDefault();
                db.LienKetNguoiDungDuAns.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertLienKetNguoiDungDuAn(int duAnId, Guid nguoiDungId, bool LaQuanLy)
        {
            try
            {
                LienKetNguoiDungDuAn entity = new LienKetNguoiDungDuAn();
                entity.DuAnId = duAnId;
                entity.NguoiDungId = nguoiDungId;
                entity.LaQuanLy = LaQuanLy;
                entity.NgayThamGia = DateTime.Now;
                db.LienKetNguoiDungDuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateLienKetNguoiDungDuAn(int duAnId, Guid nguoiDungId, bool LaQuanLy)
        {
            try
            {
                var entity = db.LienKetNguoiDungDuAns.Where(t => t.NguoiDungId == nguoiDungId && t.DuAnId == duAnId).FirstOrDefault();
                entity.LaQuanLy = LaQuanLy;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public bool Update(DuAnViewModel vm, Guid userId)
        {
            try
            {
                var entity = db.DuAns.Where(t => t.DuAnId == vm.DuAnId).FirstOrDefault();
                entity.TenDuAn = vm.TenDuAn;
                entity.TenVietTat = vm.TenVietTat;
                entity.TrangThaiDuAnId = vm.TrangThaiDuAnId;
                entity.KhoaChaId = vm.KhoaChaId;
                //entity.MaDuAn = vm.MaDuAn;
                if(!string.IsNullOrEmpty(vm.MoTa))
                entity.MoTa = vm.MoTa;
                entity.NgayCapNhat = DateTime.Now;
                entity.NguoiCapNhatId = userId;
                
                    entity.GhiChu = vm.GhiChu;

                entity.TenHoatDong = vm.TenHoatDong;
                entity.NguoiQuanLyId = vm.NguoiQuanLyId;
                entity.LoaiDuAnId = vm.LoaiDuAnId;                
                //entity.KhachHangId = vm.KhachHangId;
                //entity.LoaiNganSachId = vm.LoaiNganSachId;
                //entity.LuuTru = vm.LuuTru;
                //entity.QuanTam = vm.QuanTam;
                //entity.NgayBatDau = vm.NgayBatDau;
                var duAnInfoE = db.DuAnInfos.Where(t => t.DuAnId == vm.DuAnId).FirstOrDefault();
                duAnInfoE.TongNganSach = vm.TongNganSach;
                duAnInfoE.CostTien = vm.CostTien;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int duAnId, Guid userId)
        {
            try
            {
                var entity = db.DuAns.Where(t => t.DuAnId == duAnId).FirstOrDefault();
                entity.TrangThai = false;
                entity.NgayCapNhat = DateTime.Now;
                entity.NguoiCapNhatId = userId;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Archive(int duAnId, Guid userId)
        {
            try
            {
                var entity = db.DuAns.Where(t => t.DuAnId == duAnId).FirstOrDefault();
                entity.LuuTru = true;
                entity.QuanTam = false;
                entity.NguoiCapNhatId = userId;
                entity.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int Count(int type)
        {
            try
            {
                var str = "select Count(DuAnId) from DuAn where";
                if (type == 1) str += " QuanTam =1";
                if (type == 2) str += " LuuTru =0";
                if (type == 3) str += " LuuTru=1";
                return db.ExecuteQuery<int>(str).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public DuAnViewModel GetByMa(string maDuAn)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) return null;
                var a= (from d in db.DuAns
                        join di in db.DuAnInfos on d.DuAnId equals di.DuAnId                                        
                        join l in db.LoaiDuAns on d.LoaiDuAnId equals l.LoaiDuAnId into gj from x in gj.DefaultIfEmpty() 
                        join t in db.TrangThaiDuAns on d.TrangThaiDuAnId equals t.TrangThaiDuAnId
                        join n in db.NguoiDungs on d.NguoiQuanLyId equals n.NguoiDungId
                        where d.MaDuAn==maDuAn
                        select new DuAnViewModel()
                        {
                            DuAnId = d.DuAnId,
                            TenDuAn=d.TenDuAn,
                            KhoaChaId = d.KhoaChaId,                           
                            TrangThaiDuAnId = d.TrangThaiDuAnId,
                            TenTrangThaiDuAn = t.TenTrangThaiDuAn,
                            MaMauTrangThaiDuAn = t.MaMau,
                            LoaiDuAnId = d.LoaiDuAnId,
                            TenLoaiDuAn = (x == null ? String.Empty : x.TenLoaiDuAn),
                            MaDuAn = d.MaDuAn,
                            HoTenNguoiQuanLy = n.HoTen,
                            AvatarNguoiQuanLy = n.Avatar,
                            NguoiQuanLyId = n.NguoiDungId,
                            CostTien = di.CostTien ?? 0,
                            TongNganSach = di.TongNganSach,
                            TenHoatDong = d.TenHoatDong,
                            MoTa = d.MoTa,
                            GhiChu = d.GhiChu,
                            TenVietTat=d.TenVietTat

                        }
                         ).FirstOrDefault();
                return a;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public List<DuAnViewModel> GetTenDuAnByIds(List<int> duAnIds)
        {
            try
            {
                return (from d in db.DuAns where duAnIds.Contains(d.DuAnId)  select new DuAnViewModel { DuAnId = d.DuAnId, TenDuAn = d.TenDuAn }).ToList();
            }
            catch
            {
                return null;
            }


        }
    }
}
