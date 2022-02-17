using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Models
{
    public class NoiDungModel
    {
        NoiDungProvider _manager = new NoiDungProvider();
        public Guid InsertCommentShip(NoiDungViewModel vm)
        {
            return _manager.Insert(vm.ShipAbleId.ToString(), (byte)EnumItemTypeType.ShipAbleType, (byte)EnumLoaiNoiDungType.CommentDuAnAndShip, vm.NoiDungChiTiet, vm.NguoiDungId);
        }
        public NoiDungViewModel GetById(Guid noiDungId,Guid nguoiDungId)
        {
            return _manager.GetById(noiDungId, nguoiDungId);
        }
        public bool Edit(NoiDungViewModel vm)
        {
            return _manager.Edit(vm.NoiDungId, vm.NoiDungChiTiet, vm.NguoiDungId);
        }
        public bool Delete(Guid noiDungId, Guid nguoiDungId)
        {
            return _manager.Delete(noiDungId, nguoiDungId);
        }
    }
}