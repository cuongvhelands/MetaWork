using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Models
{
    public class LoaiCongViecModel
    {
        LoaiCongViecProvider _manager = new LoaiCongViecProvider();
        public List<LoaiCongViecViewModel> Gets()
        {

            return _manager.GetsByKhoaChaId(0);
        }
        public bool InsertLienKetLoaiCongViecDuAn(LienKetLoaiCongViecDuAnViewModel vm)
        {
            return _manager.InsertLienKetLoaiCongViecDuAn(vm.DuAnId, vm.LoaiCongViecId);
        }
        public bool DeleteLienKetLoaiCongViecDuAn(LienKetLoaiCongViecDuAnViewModel vm)
        {
            return _manager.DeleteLienKetLoaiCongViecDuAn(vm.DuAnId, vm.LoaiCongViecId);
        }
        public int InsertLoaiCongViec(LoaiCongViecViewModel vm)
        {
            return _manager.Insert(vm.TenLoaiCongViec, vm.MaLoaiCongViec);
        }

        public List<LoaiCongViecViewModel> GetByDuAn(int DuAnId)
        {
            return _manager.GetIdsByDuAn(DuAnId);
        }

        public List<LoaiCongViecViewModel> GetAll()
        {
            return _manager.GetAll();
        }
    }
}