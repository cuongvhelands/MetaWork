using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Models
{
    public class NguoiDungModel
    {
        NguoiDungProvider _manager = new NguoiDungProvider();
        public List<NguoiDungViewModel> GetNguoiDungsByQuyen(byte quyen)
        {
            return _manager.GetNguoiDungByQuyen(quyen);
        }
        public NguoiDungViewModel GetBy(string userName, string passWord)
        {        
            return _manager.GetBy(userName, EndCode.Encrypt(passWord));
        }
        public NguoiDungViewModel GetNguoiDungByUserName(string userName)
        {

            return _manager.GetNguoiDungByUserName(userName);
        }
        public NguoiDungViewModel GetById(Guid nguoiDungId)
        {
            return _manager.GetById(nguoiDungId);
        }
        public List<NguoiDungViewModel> GetsExcept(List<Guid> userIds)
        {
         
            return _manager.GetsExcept(userIds);
        }
        public List<NguoiDungViewModel> GetsByCongViecId(int congViecId,Guid userId)
        {         
            CongViecProvider congViecM = new CongViecProvider();
            var congViecVm = congViecM.GetById(congViecId);
            if (congViecVm == null) return null;
            var duAnId = congViecVm.DuAnId;
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            var lst = _manager.GetsByDuAnId(duAnId);
            if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
            DuAnProvider duAnM = new DuAnProvider();
            var duAnVm = duAnM.GetById(duAnId, userId);
            if (duAnVm != null)
            {
                var check = true;
                foreach(var item in lst)
                {
                    if (item.NguoiDungId == userId)
                    {
                        check = false;
                    }
                }
                if (check) lstToReturn.Add(_manager.GetById(userId));
            }
            return lstToReturn;

        }
        public List<NguoiDungViewModel> GetByDuAnId(int DuAnId)
        {          
            return _manager.GetsByDuAnId(DuAnId);
        }
        public List<NguoiDungViewModel> GetByDuAnId2(int DuAnId,Guid nguoiXuLyId)
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>() { new NguoiDungViewModel() { NguoiDungId = Guid.Empty, HoTen = "Chọn người dùng" } };       
            var lst = _manager.GetsByDuAnId(DuAnId, nguoiXuLyId);
            if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
            return lstToReturn;
        }
        public List<NguoiDungViewModel> GetByDuAnId2(int duAnId)
        {         
            var lst= _manager.GetsByDuAnId(duAnId);
            return lst;
        }
        public List<NguoiDungViewModel> GetAll()
        {          
            return _manager.GetAll();
        }
        public List<NguoiDungViewModel> GetAll2()
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>() { new NguoiDungViewModel() {NguoiDungId=Guid.Empty,HoTen="Tất cả người dùng" } };
            var lst = _manager.GetAll();
            if (lst != null && lst.Count > 0)
            {
                lstToReturn.AddRange(lst);
            }
            return lstToReturn;
        }
        public List<NguoiDungViewModel> GetsByPhongBanId(int phongBanId)
        {          
            return _manager.GetNguoiDungsByPhongBanId(phongBanId);
        }
        public List<NguoiDungViewModel> GetNguoiDungByLoai(int loaiNguoiDung)
        {        
            return _manager.GetNguoiDungByLoai(loaiNguoiDung);
        }

        public List<NguoiDungViewModel> GetNguoiDungsBy(string email)
        {
            List<string> emails = new List<string>();
            if (!string.IsNullOrEmpty(email)) emails.Add(email);
            var lst= _manager.GetNguoiDungByEmails(emails);
            if (lst != null && lst.Count > 0)
            {
                foreach(var item in lst)
                {
                    PhongBanProvider pbP = new PhongBanProvider();
                    var phongBan = pbP.GetByNguoiDungId(item.NguoiDungId);
                    if (phongBan != null)
                    {
                        item.PhongBanId = phongBan.PhongBanId;
                        item.TenPhongBan = phongBan.TenPhongBan;
                    }
                    
                }
               
            }
            return lst;
        }

        public List<NguoiDungViewModel> GetTenNguoiDungBy(List<int> phongBanIds, List<int> duAnIds)
        {
            var ids = _manager.GetIdsBy(phongBanIds, duAnIds);
            if (ids != null && ids.Count > 0) return _manager.GetsByIds(ids);
            return null;
        }
    }
}