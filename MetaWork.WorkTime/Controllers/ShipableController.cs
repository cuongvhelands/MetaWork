using Newtonsoft.Json;
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
    public class ShipableController : Controller
    {
        DuAnModel _model = new DuAnModel();
        CongViecModel _congViecM = new CongViecModel();
        [Authorize]
        public ActionResult Index(int? duAnId,int? tuan,int? year,string trangThaiIds, Guid? nguoiDungId,int? teamId)
        {
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            if (tuan == null)
            {
                tuan = GetNumerWeekOfYears2(monday);
                year = monday.Year;
            }
            if (nguoiDungId == null) nguoiDungId = Guid.Empty;
            ViewBag.TuanHienTai = GetNumerWeekOfYears2(monday);
            List<int> ttIds = new List<int>();
            if (!string.IsNullOrEmpty(trangThaiIds))
            {
                var col = trangThaiIds.Split(',');
                if (col != null && col.Count() > 0)
                {
                    foreach(var item in col)
                    {
                        ttIds.Add(int.Parse(item));
                    }
                }
            }
            var results = _congViecM.GetBy(duAnId, tuan.Value, year.Value, ttIds, nguoiDungId.Value, teamId);
            ViewBag.Tuan = tuan;
            ViewBag.Year = year;        
            ViewBag.DuAnId = duAnId??0;
            ViewBag.NguoiDungIdMS = nguoiDungId;
            ViewBag.Team = teamId ?? 0;
            ViewBag.trangThaiCV = "";
            if (ttIds != null && ttIds.Count > 0)
            {
                ViewBag.trangThaiCV = JsonConvert.SerializeObject(ttIds);
            }
           
            ViewBag.Ma = _congViecM.GetMaShipable();
            ViewBag.Now = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            return View(results);
        }

        public ActionResult Report(List<Guid> nguoiDungIds,int? duAnId, List<int> trangThaiIds, string strStartTime, string strEndTime)
        {
            ReportShipAbleViewModel result = new ReportShipAbleViewModel();
            DateTime startTime;
            DateTime endTime;
            var now = DateTime.Now;
            if(!DateTime.TryParseExact(strStartTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime)){
                startTime = new DateTime(now.Year, now.Month, 1);
            }
            if (!DateTime.TryParseExact(strEndTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))
            {
                endTime = startTime.AddMonths(1).AddDays(-1);
            }
            result.DuAnId = duAnId ?? 0;
            result.NguoiDungIds = nguoiDungIds ;
            result.TrangThaiIds = trangThaiIds ;
            result.StrStartTime = startTime.ToString("dd/MM/yyyy");
            result.StrEndTime = endTime.ToString("dd/MM/yyyy");
            result.ShipableInWeeks = _congViecM.GetShipablesInWeekBy(duAnId, trangThaiIds, nguoiDungIds, startTime, endTime);
            return View(result);
        }



        [Authorize]
        public ActionResult AddNew()
        {
            ViewBag.Ma = _congViecM.GetMaShipable();
            return View();
        }
        [Authorize]
        public ActionResult Edit(int congViecId)
        {
            var vm = _congViecM.GetById(congViecId);
            if (vm == null) return RedirectToAction("Index");
            return View(vm);
        }
        [Authorize]

        #region method
        [Authorize]
        public string GetTeam()
        {
            List<PhongBanViewModel> lst = new List<PhongBanViewModel>() { new PhongBanViewModel() { TenPhongBan = "Tất cả các phòng ban", PhongBanId = 0 } };
            PhongBanModel model = new PhongBanModel();
            var lstVm = model.GetAll();
            if (lstVm != null) lst.AddRange(lstVm);
            return JsonConvert.SerializeObject(lst);
        }
        [Authorize]
        public string GetDuAns()
        {

            var userId = GetUserID();
            return JsonConvert.SerializeObject(_model.GetsBy("", null));
        }
        [Authorize]
        public string GetDuAns2()
        {

            var userId = GetUserID();
            List<DuAnViewModel> results = new List<DuAnViewModel>();
            results.Add(new DuAnViewModel() { DuAnId = 0, TenDuAn = "Chọn dự án" });
            var lst = _model.GetAll();
            if (lst != null && lst.Count > 0) results.AddRange(lst);
            return JsonConvert.SerializeObject(results);
        }
        [Authorize]
        public string GetGiaiDoanDuAns(int duAnId)
        {
            return JsonConvert.SerializeObject(_model.GetGiaiDoanDuAnsByDuAnId(duAnId));
        }
        [Authorize]
        public string GetThuTuUuTiens()
        {
            List<ThuTuUuTienViewModel> results = new List<ThuTuUuTienViewModel>();
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Thấp", ThuTuUuTien = 1 });
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Trung bình", ThuTuUuTien = 2 });
            results.Add(new ThuTuUuTienViewModel() { TenThuTuUuTien = "Cao", ThuTuUuTien = 3 });
            return JsonConvert.SerializeObject(results);
        }
        [Authorize]
        public string GetDoPhucTaps()
        {
            List<DoPhucTapViewModel> results = new List<DoPhucTapViewModel>();
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Dễ", DoPhucTap = 1 });
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Bình thường", DoPhucTap = 2 });
            results.Add(new DoPhucTapViewModel() { TenDoPhucTap = "Khó", DoPhucTap = 3 });
            return JsonConvert.SerializeObject(results);
        }
        [Authorize]
        public string GetTrangThaiCongViecs()
        {
            return JsonConvert.SerializeObject(_congViecM.GetTrangThaiCongViecs());
        }
        [Authorize]
        public string GetTrangThaiShipables()
        {
            return JsonConvert.SerializeObject(_congViecM.GetTrangThaiShipables((int)EnumTrangThaiCongViecType.shipable));
        }
        [Authorize]
        public string GetTrangThaiShipables2()
        {
            return JsonConvert.SerializeObject(_congViecM.GetTrangThaiShipables((int)EnumTrangThaiCongViecType.shipableCheck));
        }
        [Authorize]
        public string GetTrangThaiShipablesAll()
        {
          
            List<TrangThaiCongViecViewModel> results = new List<TrangThaiCongViecViewModel>();
            results.Add(new TrangThaiCongViecViewModel() { TrangThaiCongViecId = 0, TenTrangThai = "Chọn trạng thái" });
            var lst = _congViecM.GetTrangThaiShipablesAll(null);
            if (lst != null && lst.Count > 0) results.AddRange(lst);
            return JsonConvert.SerializeObject(results);        
        }
        [Authorize]
        public string GetTrangThaiShipablesByKhoaCha(int khoaChaId)
        {
            if(khoaChaId==0)  return JsonConvert.SerializeObject(_congViecM.GetTrangThaiShipables((int)EnumTrangThaiCongViecType.shipable));
            else
            {             
                var trangThaiVm = _congViecM.GetTTCVById(khoaChaId);
                List<TrangThaiCongViecViewModel> lstToReturn = new List<TrangThaiCongViecViewModel>();
                if (trangThaiVm != null) lstToReturn.Add(trangThaiVm);
                if(khoaChaId!= (int)EnumTrangThaiCongViecType.shipableCheck)
                {
                    var lst = _congViecM.GetTrangThaiShipables(khoaChaId);
                    if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
                }              
                return JsonConvert.SerializeObject(lstToReturn);
            }
        }
        [Authorize]
        public string GetTrangThaiShipablesByKhoaCha2(int khoaChaId,int shipId)
        {
            if (khoaChaId == 0) return JsonConvert.SerializeObject(_congViecM.GetTrangThaiShipables((int)EnumTrangThaiCongViecType.shipable));
            else
            {           
                List<TrangThaiCongViecViewModel> lstToReturn = new List<TrangThaiCongViecViewModel>();
                var ship = _congViecM.GetById(shipId);                
                    var lst = _congViecM.GetTrangThaiShipables(khoaChaId);
                    if (lst != null && lst.Count > 0) {
                        if (ship != null)
                        {
                            foreach(var item in lst)
                            {
                                if (ship.KhoaChaId != null)
                                {
                                    if (item.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.shipableContinue) lstToReturn.Add(item);
                                }else if (ship.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue)
                                {
                                    if (item.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.shipableDebit) lstToReturn.Add(item);
                            }
                            else
                            {
                                if (item.TrangThaiCongViecId == ship.TrangThaiCongViecId) lstToReturn.Add(item);
                            }
                              
                            }
                        }
                        else
                        {
                            lstToReturn.AddRange(lst);
                        }
                      

                    }

               
                return JsonConvert.SerializeObject(lstToReturn);
            }
        }

        [Authorize]
        public string GetNguoiDungs()
        {
            var userId = Guid.Parse("29208D00-EFA0-4F32-8C15-1BDA5006506D");
            NguoiDungModel model = new NguoiDungModel();
            return JsonConvert.SerializeObject(model.GetsExcept(new List<Guid>() { userId }));
        }
       
        [Authorize]
        public string GetNguoiDungsByDuAnId(int duAnId)
        {          
            NguoiDungModel model = new NguoiDungModel();
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            lstToReturn.Add(new NguoiDungViewModel() { NguoiDungId = Guid.Empty, HoTen = "Tất cả người dùng" });
            var lst = model.GetByDuAnId(duAnId);
            if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
            return JsonConvert.SerializeObject(lstToReturn);
        }

        [Authorize]
        public string GetNguoiDungsByPhongBan(int phongBanId)
        {
            NguoiDungModel model = new NguoiDungModel();
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            lstToReturn.Add(new NguoiDungViewModel() { NguoiDungId = Guid.Empty, HoTen = "Tất cả người dùng" });
            var lst = model.GetsByPhongBanId(phongBanId);
            if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
            return JsonConvert.SerializeObject(lstToReturn);
        }
        [Authorize]
        public string GetNguoiDungsShipable(int congViecId)
        {
            var userId = Guid.Parse("29208D00-EFA0-4F32-8C15-1BDA5006506D");
            NguoiDungModel model = new NguoiDungModel();
            return JsonConvert.SerializeObject(model.GetsByCongViecId(congViecId, userId));
        }
        [Authorize]
        public string GetWeeks()
        {
            var weekNow = GetNumerWeek(DateTime.Now);
            var maxWeek = GetNumberWeekOfYears(DateTime.Now.Year);
            List<WeekViewModel> results = new List<WeekViewModel>();
            var j = 1;
            var year = Helpers.GetMonDayBy(DateTime.Now).Year;
            for (int i = 0; i < 5; i++)
            {
                if (weekNow <= maxWeek)
                {
                    results.Add(new WeekViewModel() { Week = weekNow+"-"+year, TenWeek = "Tuần " + weekNow });
                    weekNow++;
                }
                else
                {
                    year++;
                    results.Add(new WeekViewModel() { Week = j + "-" + year, TenWeek = "Tuần " + j });
                    j++;
                }
            }
            return JsonConvert.SerializeObject(results);
        }
        [Authorize]
        public static int GetNumberWeekOfYears(int year)
        {
            CultureInfo myCI = new CultureInfo("en-US");
            System.Globalization.Calendar myCal = myCI.Calendar;
            DateTime LastDay = new System.DateTime(year, 12, 31);
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            return myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW);
        }
        [Authorize]
        public int GetNumerWeek(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }

        [Authorize]
        public int GetNumerWeekOfYears2(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, 1, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            var week = (date - firstMonthMonday).Days / 7 + 1;
            if (firstMonthMonday.Year == 2019) week++;
            return week;
        }

        [Authorize]
        public class WeekViewModel
        {
            public string Week { get; set; }
            public string TenWeek { get; set; }
        }
        [Authorize]
        [HttpPost]
        public int AddShipable(CongViecViewModel vm)
        {
            var userId = GetUserID();
            return _congViecM.InsertShipable(vm, userId);
        }
        [HttpPost]
        public bool DeleteShipable(int congViecId)
        {
            return _congViecM.DeleteShipable(congViecId);
        }
        [Authorize]
        [HttpPost]
        public bool UpdateShipable(CongViecViewModel vm)
        {
            var userId = GetUserID();
            return _congViecM.UpdateShipable(vm, userId);
        }
        [Authorize]
        public string GetById(int congViecId)
        {
            var vm = _congViecM.GetShipableById(congViecId);

            if (vm != null)
            {
                return JsonConvert.SerializeObject(vm);
            }
            return "";
        }
        [Authorize]
        public string GetWeeks2()
        {
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var week = GetNumerWeekOfYears2(monday);
            var year = monday.Year;
            var maxWeek = Helpers.GetNumberWeekOfYears(year);
            var nextWeek = week + 1;          
            List<WeekViewModel> results = new List<WeekViewModel>();
            results.Add(new WeekViewModel() { Week = week+"-"+year, TenWeek = "Tuần hiện tại " });
            if (week == maxWeek) {
                nextWeek = 1;
                year++;
            }
            results.Add(new WeekViewModel() { Week = nextWeek + "-" + year, TenWeek = "Tuần tiếp theo " });

            return JsonConvert.SerializeObject(results);
        }

        [Authorize]        
        public string GetWeeks3(int week, int year)
        {
            var maxWeek = Helpers.GetNumberWeekOfYears(year);

            List<WeekViewModel> results = new List<WeekViewModel>();
            for (int i = 0; i < 5; i++)
            {
                if (week > maxWeek)
                {
                    week = 1;
                    year++;
                }
                results.Add(new WeekViewModel() { Week = week + "-" + year, TenWeek = "Tuần " + week });
                week++;
            }
            return JsonConvert.SerializeObject(results);
        }
  
        [Authorize]
        public string GetMota(int congViecId)
        {
            var vm = _congViecM.GetById(congViecId);
            if (vm != null) return vm.MoTa;
            return "";
        }
        #endregion

        [Authorize]
        public ActionResult Insert()
        {
            return View();
        }
        [Authorize]
        public ActionResult ListShipable()
        {
            return View();
        }
        public Guid GetUserID()
        {
            NguoiDungProvider nguoiDungP = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }

      
    }
}