using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class GiaiDoanDuAnProvider
    {
        TimerDataContext db = null;
        public GiaiDoanDuAnProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<GiaiDoanDuAnViewModel> GetsByDuAnId(int duAnId)
        {
            try
            {
                var str = "Select * from GiaiDoanDuAn where TrangThai=1 and DuAnId=" + duAnId;
                return db.ExecuteQuery<GiaiDoanDuAnViewModel>(str).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public GiaiDoanDuAnViewModel GetById(int id)
        {
            try
            {
                return db.ExecuteQuery<GiaiDoanDuAnViewModel>("select * from GiaiDoanDuAn where GiaiDoanDuAnId=" + id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public int InsetGiaiDoanDuAn(GiaiDoanDuAnViewModel vm,Guid userId)
        {
            try
            {
                var check = false;
                if (vm.TrangThaiHienTai)
                {
                    check = true;
                    try
                    {
                        var lst = db.GiaiDoanDuAns.Where(t => t.DuAnId == vm.DuAnId).ToList();
                        foreach(var item in lst)
                        {
                            item.TrangThaiHienTai = false;
                        }
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
                        var lst = db.GiaiDoanDuAns.Where(t => t.DuAnId == vm.DuAnId).ToList();
                        foreach (var item in lst)
                        {
                            if (item.TrangThaiHienTai == true) check = false;
                          
                        }
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                }

                GiaiDoanDuAn entity = new GiaiDoanDuAn();
                entity.TenGiaiDoan = vm.TenGiaiDoan;
                entity.MoTa = vm.MoTa;
                entity.NgayTao = DateTime.Now;
                entity.TrangThai = true;
                entity.DuAnId = vm.DuAnId;
                entity.ThoiGianBatDau = vm.ThoiGianBatDau;
                entity.ThoiGianKetThuc = vm.ThoiGianKetThuc;
                entity.LoaiGiaiDoanId = vm.LoaiGiaiDoanId;
                if (check)
                    entity.TrangThaiHienTai = vm.TrangThaiHienTai;
                else entity.TrangThaiHienTai = true;
                db.GiaiDoanDuAns.InsertOnSubmit(entity);
                db.SubmitChanges();
                if (vm.HangMucCongViecs != null && vm.HangMucCongViecs.Count > 0)
                {
                    foreach(var item in vm.HangMucCongViecs)
                    {
                        HangMucCongViec entity2 = new HangMucCongViec();
                        entity2.GiaiDoanDuAnId = entity.GiaiDoanDuAnId;
                        entity2.TenHangMuc = item.TenHangMuc;
                        entity2.TrangThai = item.TrangThai;
                        entity2.NgayCapNhat = DateTime.Now;
                        entity2.NguoiCapNhatId = userId;
                        db.HangMucCongViecs.InsertOnSubmit(entity2);
                    }
                }
                db.SubmitChanges();
                return entity.GiaiDoanDuAnId;
            }
            catch
            {
                return 0;
            }
        }

        public bool UpdateGiaiDoanDuAn(GiaiDoanDuAnViewModel vm, Guid userId)
        {
            try
            {
                var check = false;
                if (vm.TrangThaiHienTai)
                {
                    check = true;
                    try
                    {                      
                        var lst = db.GiaiDoanDuAns.Where(t => t.DuAnId == vm.DuAnId).ToList();
                        foreach (var item in lst)
                        {
                            item.TrangThaiHienTai = false;
                        }
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
                        var lst = db.GiaiDoanDuAns.Where(t => t.DuAnId == vm.DuAnId).ToList();
                        foreach (var item in lst)
                        {
                            if (item.TrangThaiHienTai == true && item.GiaiDoanDuAnId != vm.GiaiDoanDuAnId) check = true;
                        }
                        db.SubmitChanges();
                    }
                    catch
                    {

                    }
                }
                var entity = db.GiaiDoanDuAns.Where(t => t.GiaiDoanDuAnId == vm.GiaiDoanDuAnId).FirstOrDefault();               
                entity.TenGiaiDoan = vm.TenGiaiDoan;
                entity.MoTa = vm.MoTa;  
                entity.DuAnId = vm.DuAnId;
                entity.ThoiGianBatDau = vm.ThoiGianBatDau;
                entity.ThoiGianKetThuc = vm.ThoiGianKetThuc;
                entity.LoaiGiaiDoanId = vm.LoaiGiaiDoanId;
                if (check)
                    entity.TrangThaiHienTai = vm.TrangThaiHienTai;
                else entity.TrangThaiHienTai = true;
                db.SubmitChanges();
                List<HangMucCongViec> hangMucs = new List<HangMucCongViec>();
                try
                {
                    hangMucs = db.HangMucCongViecs.Where(t => t.GiaiDoanDuAnId == entity.GiaiDoanDuAnId).ToList();
                }
                catch
                {
                    hangMucs = new List<HangMucCongViec>();
                }
                if (hangMucs != null && hangMucs.Count > 0)
                {
                    foreach(var item in hangMucs)
                    {
                        var checkdelete = true;                     
                        if (vm.HangMucCongViecs != null && vm.HangMucCongViecs.Count > 0)
                        {
                            foreach(var item2 in vm.HangMucCongViecs)
                            {
                                if (item.HangMucCongViecId == item2.HangMucCongViecId)
                                {
                                    checkdelete = false;
                                    if (item.TenHangMuc != item2.TenHangMuc || item.TrangThai != item2.TrangThai)
                                    {                                      
                                        item.TenHangMuc = item2.TenHangMuc;
                                        item.TrangThai = item2.TrangThai;
                                        item.NgayCapNhat = DateTime.Now;
                                        item.NguoiCapNhatId = userId;
                                    }
                                }
                            }
                        }
                        if (checkdelete) db.HangMucCongViecs.DeleteOnSubmit(item);                       
                    }
                }
               
                if (vm.HangMucCongViecs != null && vm.HangMucCongViecs.Count > 0)
                {
                    foreach (var item in vm.HangMucCongViecs)
                    {
                        if (item.HangMucCongViecId < 0)
                        {
                            HangMucCongViec entity2 = new HangMucCongViec();
                            entity2.GiaiDoanDuAnId = entity.GiaiDoanDuAnId;
                            entity2.TenHangMuc = item.TenHangMuc;
                            entity2.TrangThai = item.TrangThai;
                            entity2.NgayCapNhat = DateTime.Now;
                            entity2.NguoiCapNhatId = userId;
                            db.HangMucCongViecs.InsertOnSubmit(entity2);
                        }                        
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

        public bool Delete(int giaiDoanDuAnId)
        {
            try
            {
                var entity = db.GiaiDoanDuAns.Where(t => t.GiaiDoanDuAnId == giaiDoanDuAnId).FirstOrDefault();
                db.GiaiDoanDuAns.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public GiaiDoanDuAnViewModel GetActiveBy(int duAnId)
        {
            try
            {
                var str = "Select * from GiaiDoanDuAn where TrangThai=1 and TrangThaiHienTai =1 and DuAnId=" + duAnId;
                return db.ExecuteQuery<GiaiDoanDuAnViewModel>(str).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
