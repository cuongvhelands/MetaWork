using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class CongViecOutLineModel
    {
        CongViecProvider _manager;
        public CongViecOutLineModel()
        {
            _manager = new CongViecProvider();
        }
        #region Get
        #endregion
        #region InsertOrUpdate
        public int InsertCongViecOutLine(int duAnId,Guid nguoiDungId,int? khoaChaId,int tuan, int nam)
        {
            return _manager.InsertCongViecOutline(duAnId, khoaChaId,nguoiDungId, tuan, nam);
        }
        public DuAnViewModel GetAllOfDuAn(int duAnId)
        {
            DuAnProvider model = new DuAnProvider();
            var vm = model.GetByIdO1(duAnId);
            vm.CongViecs = GetAllCongViecOfProject(duAnId);
            return vm;

        }
        public List<CongViecViewModel> GetAllCongViecOfProject(int duAnId)
        {
            List<CongViecViewModel> lstToReturn = new List<CongViecViewModel>();
            var lst = _manager.GetAllTaskInDuAn(duAnId);
            if (lst != null && lst.Count > 0)
            {
                lstToReturn = GetDeQuys(lst, null);
            }
            return lstToReturn;
        }
        private List<CongViecViewModel> GetDeQuys(List<CongViecViewModel> data,int? khoaChaId)
        {
            List<CongViecViewModel> lstToReturn = new List<CongViecViewModel>();
            lstToReturn.AddRange(data.Where(t => t.KhoaChaId == khoaChaId).ToList().OrderBy(t => t.ThuTuSapXep).ThenByDescending(t => t.NgayTao).ToList());
            foreach(var item in lstToReturn)
            {
                if (data.Count(t => t.KhoaChaId == item.CongViecId) > 0)
                {
                    item.CongViecs = GetDeQuys(data, item.CongViecId);
                }
            }
            return lstToReturn;
        }


        #endregion
        #region Delete
        #endregion
        #region Check
        #endregion
        #region method
        #endregion
    }
}