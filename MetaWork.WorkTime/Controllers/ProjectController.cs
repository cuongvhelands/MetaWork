using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MetaWork.WorkTime.Models;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        DuAnModel _model = new DuAnModel();        
        CongViecModel _congViecM = new CongViecModel();
        NguoiDungModel _nguoiDungM = new NguoiDungModel();
        NoiDungModel _noiDungM = new NoiDungModel();
        // GET: Project
        public ActionResult Index(int? type)
        {                     
            var userId = GetUserID();
            var user = _nguoiDungM.GetById(userId);
            if (user == null) return RedirectToAction("Login", "User");
            else
            {
                if (user.Quyen != 3) return RedirectToAction("Index", "Home");
            }
            DuAnIndexViewModel vm = new DuAnIndexViewModel();
            if (type == null) type = 1;
            List<DuAnViewModel> result = _model.GetsBy("", type);
            vm.CountAll = _model.Count(2);
            vm.CountArchive = _model.Count(3);
            vm.CountFavorite = _model.Count(1);
            vm.Type = type.Value;
            vm.DuAns = result;
            return View(vm);
        }
       
        public ActionResult AddNew()
        {
            var userId = GetUserID();
            var user = _nguoiDungM.GetById(userId);
            if (user == null) return RedirectToAction("Login", "User");
            else
            {
                if (user.Quyen != 3) return RedirectToAction("Index", "Home");
            }
            ViewBag.Now = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            return View();
        }
       
        public ActionResult Edit(int duAnId)
        {
            var userId = GetUserID();
            var user = _nguoiDungM.GetById(userId);
            if (user == null) return RedirectToAction("Login", "User");
            else
            {
                if (user.Quyen != 3) return RedirectToAction("Index", "Home");
            }
            ViewBag.Now = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
           
            DuAnModel model = new DuAnModel();
            var vm = model.GetById(duAnId, userId);
            if (vm == null) return RedirectToAction("index");
            return View(vm);
        }          
       
        public ActionResult ListProject()
        {
            return View();

        }

        public ActionResult ProjectChart(string strStartTime, string strEndTime,int duAnId,int? giaiDoanDuAnId)
        {
            var userId = GetUserID();            
            var vm = _model.GetById2(duAnId, userId,null,null,giaiDoanDuAnId);
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            var tuan2 = Helpers.GetNumerWeek(DateTime.Now);
            var tuan3 = Helpers.GetNumerWeek(new DateTime(2022, 12, 31));
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var year = monday.Year;
            ViewBag.TuanHienTai = tuan;
            ViewBag.Nam = year;
            ViewBag.Ma = _congViecM.GetMaShipable();
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            vm.Spent = model.GetTimeSpentOfDuAn(vm.DuAnId);
            vm.StrTimeSpent = GetStrTime(vm.Spent);
            return View(vm);
        }
        private string GetStrTime(int timeSpent)
        {
            var result = "";
            if (timeSpent > 0)
            {
                var hour = timeSpent / 3600;
                var minutes = (timeSpent % 3600) / 60;
                var second = (timeSpent % 3600) % 60;
                if (second > 0) minutes++;
                result += hour + "h";
                if (minutes < 10)
                    result += "0" + minutes + "p";
                else result += minutes + "p";
            }
            else
            {
                result = "0h00p";
            }
            return result;
        }
        private string getStrTime2(int timeSpent)
        {
            var result = "";
            if (timeSpent > 0)
            {
                var hour = timeSpent / 3600;
                var minutes = (timeSpent % 3600) / 60;
                var second = (timeSpent % 3600) % 60;
                if (hour < 10) result += "0" + hour + ":";
                else result += hour + ":";
                if (minutes < 10)
                    result += "0" + minutes + ":";
                else result += minutes + ":";
                if (second < 10)
                    result += "0" + second;
                else result += second;
            }
            else
            {
                result = "00:00:00";
            }
            return result;
        }
        public ActionResult ResouceUser(int? phongBanId,string strNguoiDungId, string strStartTime, string strEndTime)
        {
            ResourceUserViewModel vm = new ResourceUserViewModel();
            NguoiDungModel nguoiDungM = new NguoiDungModel();
            var nguoiDung = nguoiDungM.GetById(GetUserID());
            ViewBag.QuyenM = nguoiDung.Quyen;
            ViewBag.NguoiDungId = GetUserID();
            var now = DateTime.Now;
            DateTime startDate;
            DateTime endDate;
            //strStartTime = "01/12/2019";
            //strEndTime = "31/12/2019";
            if (string.IsNullOrEmpty(strStartTime))           
                startDate = new DateTime(now.Year, now.Month, 1);           
            else startDate = DateTime.ParseExact(strStartTime, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(strEndTime))
                endDate = (new DateTime(startDate.Year, startDate.Month+1, 1)).AddDays(-1);
            else endDate= DateTime.ParseExact(strEndTime, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            List<Guid> nguoiDungIds = new List<Guid>();
            if(!string.IsNullOrEmpty(strNguoiDungId))
            {
                var col = strNguoiDungId.Split(',');
                foreach(var item in col)
                {
                    nguoiDungIds.Add(Guid.Parse(item));
                }
            }
            vm = model.GetsBy(phongBanId, nguoiDungIds, startDate, endDate);
            vm.StrStartDate = startDate.ToString("dd/MM/yyyy");
            vm.StrEndDate = endDate.ToString("dd/MM/yyyy");
            vm.PhongBanId = phongBanId ?? 0;
           
            PhongBanModel phongBanM = new PhongBanModel();
            vm.PhongBans = phongBanM.GetAll();
            vm.StrNguoiDungId = strNguoiDungId;
            vm.NguoiDungAll = nguoiDungM.GetAll();
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var tuan = Helpers.GetNumerWeek(monday);
            vm.Tuan = tuan;
 
            return View(vm);
        }
        public ActionResult CommentShipable(int shipAbleId)
        {
            var nguoiDungId = GetUserID();
            var vm = _congViecM.GetById3(shipAbleId, nguoiDungId);
            return View(vm);
        }
        public ActionResult PartialViewComment(Guid noiDungId)
        {
            var userId = GetUserID();
            var vm = _noiDungM.GetById(noiDungId, userId);
            return View(vm);
        }

        #region method

        public string GetLoaiCongViecs()
        {
            LoaiCongViecModel model = new LoaiCongViecModel();
            return JsonConvert.SerializeObject(model.Gets());
        }
       
        public string GetKhachHangs()
        {
            KhachHangModel model = new KhachHangModel();
            List<KhachHangViewModel> khachHangs = new List<KhachHangViewModel>() { new KhachHangViewModel() { KhachHangId = 0, TenKhachHang = "Chọn khách hàng" } };
            var result = model.Gets();
            if (result != null && result.Count > 0) khachHangs.AddRange(result);
            return JsonConvert.SerializeObject(khachHangs);
        }
       
        public string GetNguoiDungs()
        {
            NguoiDungModel model = new NguoiDungModel();
            var userId = Guid.Parse("3b2e2a9b-8d98-4aae-b967-01e28470bb71");
            List<NguoiDungViewModel> nguoiDungs = new List<NguoiDungViewModel>() { new NguoiDungViewModel() { NguoiDungId = Guid.Empty, HoTen = "Chọn người dùng" } };
            var result = model.GetsExcept(new List<Guid>() { userId });
            if (result != null && result.Count > 0) nguoiDungs.AddRange(result);
            return JsonConvert.SerializeObject(nguoiDungs);
        }

        public string GetNguoiDungByPhongBanId(int phongBanId)
        {
            NguoiDungModel model = new NguoiDungModel();
            return JsonConvert.SerializeObject(model.GetsByPhongBanId(phongBanId));
        }

        [HttpPost]
        public int AddDuAn(DuAnViewModel vm)
        {
            var userId = GetUserID();
            return _model.InsertDuAnViewModel(vm, userId);
        }
       
        [HttpPost]
        public bool UpdateDuAn(DuAnViewModel vm)
        {
            var userId = GetUserID();
            return _model.UpdateDuAn(vm, userId);
        }
       
        [HttpDelete]
        public bool DeleteDuAn(int duAnId)
        {
            var userId = GetUserID();
            return _model.DeleteDuAn(duAnId, userId);
        }
       
        [HttpPost]
        public int InsertLoaiCongViec(LoaiCongViecViewModel vm)
        {
            LoaiCongViecModel model = new LoaiCongViecModel();
            return model.InsertLoaiCongViec(vm);
        }
       
        [HttpPost]
        public bool InsertLienKetLoaiCongViecDuAn(LienKetLoaiCongViecDuAnViewModel vm)
        {
            LoaiCongViecModel model = new LoaiCongViecModel();
            return model.InsertLienKetLoaiCongViecDuAn(vm);
        }
       
        [HttpDelete]
        public bool DeleteLienKetLoaiCongViecDuAn(LienKetLoaiCongViecDuAnViewModel vm)
        {
            LoaiCongViecModel model = new LoaiCongViecModel();
            return model.DeleteLienKetLoaiCongViecDuAn(vm);
        }
       
        [HttpPost]
        public int InsertGiaiDoanDuAn(GiaiDoanDuAnViewModel vm)
        {
            var userId = GetUserID();
            return _model.InsertGiaiDoanDuAn(vm, userId);
        }
       
        [HttpPost]
        public bool UpdateGiaiDoanDuAn(GiaiDoanDuAnViewModel vm)
        {
            var userId = GetUserID();
            return _model.UpdateGiaiDoanDuAn(vm, userId);
        }
       
        [HttpPost]
        public Guid InsertCommentShip(NoiDungViewModel vm)
        {
            var nguoiDungId = GetUserID();
            vm.NguoiDungId = nguoiDungId;        
            return _noiDungM.InsertCommentShip(vm);
        }

        [HttpPost]
        public bool EditCommentShip(NoiDungViewModel vm)
        {
            var nguoiDungId = GetUserID();
            vm.NguoiDungId = nguoiDungId;          
            return _noiDungM.Edit(vm);
        }

        [HttpPost]
        public bool DeleteComment(Guid noiDungId)
        {          
            return _noiDungM.Delete(noiDungId, GetUserID());
        }

        public string GetGiaiDoanDuAnById(int id)
        {
            return JsonConvert.SerializeObject(_model.GetGiaiDoanDuAnById(id));
        }
       
        [HttpDelete]
        public bool DeleteGiaiDoanDuAn(int giaiDoanDuAnId)
        {
            return _model.DeleteGiaiDoanDuAn(giaiDoanDuAnId);
        }
       
        public string GetTrangThaiDuAns()
        {
            return JsonConvert.SerializeObject(_model.GetTrangThaiDuAns());
        }
       
        public string GetLoaiNganSachs()
        {
            return JsonConvert.SerializeObject(_model.GetLoaiNganSachs());
        }
       
        public string  GetLoaiGiaiDoans()
        {
            return JsonConvert.SerializeObject(_model.GetLoaiGiaiDoans());
        }

        public Guid GetUserID()
        {
            NguoiDungProvider nguoiDungP = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }

        public bool ArchaveProject(int duAnId)
        {
            var userId = GetUserID();
            return _model.Archive(duAnId, userId);
        }

        #region LienKetNguoiDungDuAn
        [HttpPost]
        public bool UpdateLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _model.UpdateLienKetNguoiDungDuAn(vm);
        }

        [HttpPost]
        public bool InsertLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _model.InsertLienKetNguoiDungDuAn(vm);
        }

        [HttpDelete]
        public bool DeleteLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _model.DeleteLienKetNguoiDungDuAn(vm);
        }

        #endregion
        #endregion
    }
}