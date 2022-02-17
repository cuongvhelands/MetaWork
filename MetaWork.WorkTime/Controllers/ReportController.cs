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
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        DuAnModel _model = new DuAnModel();
        
        public NguoiDungModel _nguoiDungM = new NguoiDungModel();
        public ActionResult Point(bool? phongBan,int? year, int? month, int? week)
        {
            PointViewModel vm = new PointViewModel(); 
            var monday = Helpers.GetMonDayBy(DateTime.Now);          
            vm.PhongBan = phongBan??false;
            vm.Year = year ?? monday.Year;
            vm.Month = month??monday.Month;
            vm.Week=week?? Helpers.GetNumberCurrentWeekOfMonth(monday);           
            vm.NumberWeek = Helpers.GetNumberWeekOfMonth(vm.Year, vm.Month);
            return View(vm);
        }
        public string GetPointBy(bool phongBan, int year, int month , int week)
        {
            CongViecModel model = new CongViecModel();          

            return JsonConvert.SerializeObject(model.GetReportPointBy(phongBan,year,month,week));
        }
        public int GetNumberWeekOfMonth(int year, int month)
        {
            return Helpers.GetNumberWeekOfMonth(year, month);
        }

        public ActionResult ReportProject(int? duAnId, Guid? nguoiDungId, string strStartDate, string strEndDate)
        {
            ReportProjectViewModel vm = new ReportProjectViewModel();
            DuAnModel model = new DuAnModel();
            vm.DuAnId = duAnId ?? 0;
            vm.NguoiDungId = nguoiDungId ?? Guid.Empty;         
            if (!string.IsNullOrEmpty(strStartDate)) vm.StartDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                vm.StartDate = new DateTime(year, month, 1);
            }
            if (!string.IsNullOrEmpty(strEndDate)) vm.EndDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                vm.EndDate = vm.StartDate.AddMonths(1).AddDays(-1);
            }          
            vm.DuAns = model.GetReportProjectBy(vm.DuAnId, vm.NguoiDungId, vm.StartDate, vm.EndDate);
            List<NguoiDungViewModel> nguoiDungs = new List<NguoiDungViewModel>();          
            if (nguoiDungId != Guid.Empty&&nguoiDungId!=null)
            {
                var nguoiDungVm = _nguoiDungM.GetById(vm.NguoiDungId);
                if (nguoiDungVm != null) nguoiDungs.Add(nguoiDungVm);
            }
            else
                nguoiDungs = _nguoiDungM.GetAll();
            vm.NguoiDungs = nguoiDungs;
            return View(vm);
        }

        public ActionResult PartialViewReportUserProject(Guid nguoiDungId,int duAnId, string strStartDate, string strEndDate)
        {
            DateTime startDate;
            DateTime endDate;
            if (!string.IsNullOrEmpty(strStartDate)) startDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                startDate = new DateTime(year, month, 1);
            }
            if (!string.IsNullOrEmpty(strEndDate)) endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else endDate = startDate.AddMonths(1).AddDays(-1);
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel() { NguoiDungId = nguoiDungId,DuAnId= duAnId, StartDate = startDate, EndDate = endDate };
            var userId = GetUserID();
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
            if (nguoiDung != null) vm.HoTen = nguoiDung.HoTen;
            var duAn = _model.GetById(duAnId, userId);
            if (duAn != null) vm.TenDuAn = duAn.TenDuAn;
            vm.KhoangThoiGian = getKhoangTime(vm.StartDate, vm.EndDate);
            return View(vm);
        }



        public ActionResult PartialViewReportDuAn(int duAnId, string strStartDate, string strEndDate)
        {            
            DateTime startDate;
            DateTime endDate;
            if (!string.IsNullOrEmpty(strStartDate)) startDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                startDate = new DateTime(year, month, 1);               
            }
            if (!string.IsNullOrEmpty(strEndDate)) endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else endDate=startDate.AddMonths(1).AddDays(-1);
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel() { DuAnId=duAnId,StartDate=startDate,EndDate=endDate};       
            var userId = GetUserID();            
            var duAn = _model.GetById(duAnId, userId);
            if (duAn != null) vm.TenDuAn = duAn.TenDuAn;
            vm.KhoangThoiGian = getKhoangTime(vm.StartDate, vm.EndDate);
            return View(vm);
        }

        public ActionResult PartialViewReportUser(Guid nguoiDungId, string strStartDate, string strEndDate)
        {
            DateTime startDate;
            DateTime endDate;
            if (!string.IsNullOrEmpty(strStartDate)) startDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                startDate = new DateTime(year, month, 1);
            }
            if (!string.IsNullOrEmpty(strEndDate)) endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else endDate = startDate.AddMonths(1).AddDays(-1);
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel() { NguoiDungId = nguoiDungId, StartDate = startDate, EndDate = endDate };
            NguoiDungModel model = new NguoiDungModel();           
            var nguoiDung = model.GetById(nguoiDungId);
            if (nguoiDung != null) vm.HoTen = nguoiDung.HoTen;
            vm.KhoangThoiGian = getKhoangTime(vm.StartDate, vm.EndDate);
            return View(vm);
        }     

        public string ExportExcel(int? duAnId, Guid? nguoiDungId, string strStartDate, string strEndDate)
        {
            ReportProjectViewModel vm = new ReportProjectViewModel();
            DuAnModel model = new DuAnModel();
            vm.DuAnId = duAnId ?? 0;
            vm.NguoiDungId = nguoiDungId ?? Guid.Empty;
            if (!string.IsNullOrEmpty(strStartDate)) vm.StartDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                vm.StartDate = new DateTime(year, month, 1);
            }
            if (!string.IsNullOrEmpty(strEndDate)) vm.EndDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
            {
                vm.EndDate = vm.StartDate.AddMonths(1).AddDays(-1);
            }
            vm.DuAns = model.GetReportProjectBy(vm.DuAnId, vm.NguoiDungId, vm.StartDate, vm.EndDate);
            List<NguoiDungViewModel> nguoiDungs = new List<NguoiDungViewModel>();
            if (nguoiDungId != Guid.Empty && nguoiDungId != null)
            {
                var nguoiDungVm = _nguoiDungM.GetById(vm.NguoiDungId);
                if (nguoiDungVm != null) nguoiDungs.Add(nguoiDungVm);
            }
            else
                nguoiDungs = _nguoiDungM.GetAll();
            vm.NguoiDungs = nguoiDungs;
            return "";
        }

        [HttpPost]
        public string GetDataReport(ReportProjectDetailsViewModel vm)
        {
            vm.StartDate = DateTime.ParseExact(vm.StrStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            vm.EndDate = DateTime.ParseExact(vm.StrEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);           
            return _model.GetDataReportProjectDetail(vm.DuAnId, vm.StartDate, vm.EndDate);           
        }
        [HttpPost]
        public string GetDataReportUser(ReportProjectDetailsViewModel vm)
        {
            vm.StartDate = DateTime.ParseExact(vm.StrStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            vm.EndDate = DateTime.ParseExact(vm.StrEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);          
            return _model.GetDataReportUserDetail(vm.NguoiDungId, vm.StartDate, vm.EndDate);
        }
        [HttpPost]
        public string GetDataReportUserProject(ReportProjectDetailsViewModel vm)
        {
            vm.StartDate = DateTime.ParseExact(vm.StrStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            vm.EndDate = DateTime.ParseExact(vm.StrEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return _model.GetDataReportUserProjectDetail(vm.NguoiDungId,vm.DuAnId, vm.StartDate, vm.EndDate);
        }
        [HttpPost]
        public string GetDataReportUserProject2(ReportProjectDetailsViewModel vm)
        {
            vm.StartDate = DateTime.ParseExact(vm.StrStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            vm.EndDate = DateTime.ParseExact(vm.StrEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return _model.GetDataReportUserProjectDetail2(vm.NguoiDungId, vm.StartDate, vm.EndDate);
        }
        public Guid GetUserID()
        {
            NguoiDungProvider nguoiDungP = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }

        private string getKhoangTime(DateTime startTime, DateTime endTime)
        {
            string result = "";
            if (startTime.Year != endTime.Year)
            {
                result = " từ ngày " + startTime.ToString("dd/MM/yyyy") + " đến ngày " + endTime.ToString("dd/MM/yyyy");
            }
            else
            {
                if (startTime.Month != endTime.Month)
                {
                    result = " từ ngày " + startTime.ToString("dd/MM") + " đến ngày " + endTime.ToString("dd/MM/yyyy");
                }
                else
                {
                    if (startTime.ToString("dd/MM/yyyy") == endTime.ToString("dd/MM/yyyy"))
                    {
                        result = " trong ngày " + startTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        result = " từ ngày " + startTime.Day + " đến ngày " + endTime.ToString("dd/MM/yyyy");
                    }
                }
            }
            return result;
        }
    }
}