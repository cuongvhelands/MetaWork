using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaWork.WorkTime.Models;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Controllers
{
    [RoutePrefix("api/Time")]
    public class TimeApiController : ApiController
    {
        [HttpPost]
        [Route("GetTimeBy")]
        public List<NguoiDungViewModel> GetsBy(ReportUserViewModel vm)
        {   
            // test push
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            return model.GetTimeOfUserBys(vm.Emails,vm.TimeFrom,vm.TimeTo);
        }
        [HttpGet]
        [Route("GetNguoiDungs")]
        public List<NguoiDungViewModel> GetsBy(string email)
        {
            NguoiDungModel model = new NguoiDungModel();
            return model.GetNguoiDungsBy( email);
        }
        [HttpGet]
        [Route("GetDanhSachChoDuyet")]
        public List<DanhSachChoDuyetViewModel> GetDanhSachChoDuyet(List<string> emails)
        {
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            return model.GetDanhSachChoDuyetBy(emails);        
        }

        [HttpGet]
        [Route("GetDanhSachTenShipableBy")]
        public List<CongViecViewModel> GetDanhSachTenShipableBy(string soDienThoai, int tuan, int nam)
        {
            CongViecModel model = new CongViecModel();
            return model.GetShipablesBy(soDienThoai, tuan, nam);
        }

        [HttpGet]
        [Route("GetDanhSachTenToDoBy")]
        public List<CongViecViewModel> GetDanhSachTenToDoBy(string soDienThoai, int shipableId, int tuan, int nam)
        {
            CongViecModel model = new CongViecModel();
            return model.GetToDoBy(shipableId,soDienThoai, tuan, nam);
        }

        [HttpGet]
        [Route("GetDanhSachToDoTrongTuanBy/{userName}/{passWord}/{startTime}/{endTime}")]
        public List<ThoiGianLamViecTrongNgayViewModel> GetDanhSachToDoTrongTuanBy(string userName,string passWord,int startTime, int endTime)
        {
            DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(startTime);
            DateTime endDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(endTime);
            CongViecModel model = new CongViecModel();
            return model.GetToDosInTimeBy(startDate, endDate, userName, EndCode.Encrypt(passWord));
        }
        [HttpGet]
        [Route("AddTimeFromXml/{filePath}")]
        public bool AddTimeFromXml(string filePath)
        {

            CongViecModel model = new CongViecModel();
            model.testAdd(filePath);
            return true;
        }


    }
}