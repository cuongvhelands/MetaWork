using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MetaWork.Project.Models;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.Project.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        // GET: Home
        public ActionResult Index()
        {
            //SetUpTimer(new TimeSpan(11, 00, 00));
            return View();
        }

        public ActionResult AddDB()
        {
            //ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            //var dateStart = new DateTime(2019, 11, 1);
            //var endDate = new DateTime(2019, 11, 30);
            //var dateStartInt = (int)(dateStart - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            //var endDateInt = (int)(endDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            //var b = model.GetTimeOfUserBys(null, dateStartInt, endDateInt);
            //if (b != null && b.Count > 0)
            //{
            //    foreach (var item in b)
            //    {
            //        var d = item;

            //    }
            //}
            //var c = b;
            return View();
        }
        private System.Threading.Timer timer;
        private object _nguoiDungP;

        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.SomeMethodRunsAt1600();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SomeMethodRunsAt1600()
        {
            //this runs at 16:00:00
        }
        [Authorize]
        public ActionResult HuyDuyet(int thoiGianLamViecId)
        {
            NguoiDungProvider nguoiDungM = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var nguoiDung = nguoiDungM.GetUserByUsername(userName);
            if (nguoiDung.Quyen != 3) return RedirectToAction("Index","Home");
            ThoiGianLamViecProvider thoiGianLamViecM = new ThoiGianLamViecProvider();
            var delete = thoiGianLamViecM.HuyDuyet(thoiGianLamViecId);
            if (delete) ViewBag.Message = "Bạn hủy duyệt thành công.";
            else ViewBag.Message = "Bạn hủy duyệt không thành công.";
            return View();
        }
        [Authorize]
        public ActionResult PheDuyet(int thoiGianLamViecId,string tokenId,string CallBackAction, string CallBackToken,string CallBackDescription)
        {
            NguoiDungProvider nguoiDungM = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var nguoiDung = nguoiDungM.GetUserByUsername(userName);
            if (nguoiDung.Quyen != 3) return RedirectToAction("Index", "Home");
            ThoiGianLamViecProvider thoiGianLamViecM = new ThoiGianLamViecProvider();
            var update = thoiGianLamViecM.UpdatePheDuyetBy(thoiGianLamViecId,tokenId);
            if (update) {
                var vm = thoiGianLamViecM.GetById(thoiGianLamViecId);
                if (vm.LoaiNgayLamViec == 4 || vm.LoaiNgayLamViec == 5 || vm.LoaiNgayLamViec == 6)
                {
                    thoiGianLamViecM.Delete(thoiGianLamViecId);
                }
                ViewBag.Message = "Bạn phê duyệt thành công.";
                if (CallBackAction == "ApproveStaffDayByToken")
                {
                    GetAddResult(CallBackToken, CallBackDescription);                    
                }
            }
            else ViewBag.Message = "Bạn phê duyệt không thành công.";
            return View();          
        }
        private AddResultViewModel GetAddResult(string CallBackToken, string CallBackDescription)
        {
            try
            {
               
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApproveStaffDay")+"?token="+ CallBackToken + "&description="+ CallBackDescription);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<AddResultViewModel>(response.Content);
                //return response.Content;
            }
            catch
            {
                return null;
            }

        }
        public ActionResult Test()
        {
            //var client = new RestClient("http://work.tecotec.io/api/Time/GetDanhSachChoDuyet");
            //var request = new RestRequest(Method.POST);
            //request.AddJsonBody(vm);
            //IRestResponse response = client.Execute(request);
            //return response.Content;
            ////SetUpTimer(new TimeSpan(11, 00, 00));
            return View();
        }
    }
}