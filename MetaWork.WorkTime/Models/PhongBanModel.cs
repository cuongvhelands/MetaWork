using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Models
{
    public class PhongBanModel
    {
        PhongBanProvider _phongBanProvider = new PhongBanProvider();

        public List<PhongBanViewModel> GetAll()
        {
            var vm = _phongBanProvider.GetAll();
            return vm;
        }

        public List<PhongBanViewModel> GetNguoiDungAll()
        {
            var vms = _phongBanProvider.GetAll();
            if (vms != null && vms.Count > 0)
            {
                NguoiDungProvider nguoiDungM = new NguoiDungProvider();
                foreach(var vm in vms)
                {
                    vm.NguoiDungs = nguoiDungM.GetNguoiDungsByPhongBanId(vm.PhongBanId);

                }
            }
            return vms;
        }

        public List<PhongBanViewModel> GetByKhoaChaId(int khoaChaId)
        {
            return _phongBanProvider.GetByKhoaChaId(khoaChaId);
        }
        public PhongBanViewModel GetByNguoiDungId (Guid nguoiDungId)
        {
            return _phongBanProvider.GetByNguoiDungId(nguoiDungId);
        }
    }
}