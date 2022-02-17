using Newtonsoft.Json;
using RestSharp;
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
using static MetaWork.WorkTime.Controllers.ShipableController;
using MetaWork.Data.SQL;
using MetaWork.Data.Model;
using System.Data;

namespace MetaWork.WorkTime.Controllers
{
    
    public class TimeController : Controller
    {
        private IDataBase _dataBaseContext;
        public IDataBase DatabaseContext
        {
            get
            {
                if (_dataBaseContext == null)
                {
                    _dataBaseContext = new SQLDataBase();
                    var conn = System.Configuration.ConfigurationManager.ConnectionStrings["Data.Properties.Settings.TEERPV2ConnectionString"];
                    if (conn != null)
                    {
                        _dataBaseContext.ConnectionString = conn.ConnectionString;
                    }
                }
                return _dataBaseContext;

            }
        }
        private StaffDayModel _staffDay;
        public StaffDayModel StaffDay
        {
            get
            {
                if (_staffDay == null) _staffDay = new StaffDayModel(DatabaseContext);
                return _staffDay;
            }
        }

        // GET: Time
        CongViecModel _model = new CongViecModel();
        ThoiGianLamViecModel _thoiGianLamViecModel = new ThoiGianLamViecModel();
        NguoiDungProvider _nguoiDungP = new NguoiDungProvider();
        public void GetDataNguoiDung()
        {
            var nguoiDungs = _nguoiDungP.GetAll();
            DuAnProvider duAnP = new DuAnProvider();
            var duAns = duAnP.GetAllDuAn();
            var startDate = new DateTime(2021, 1, 1, 0, 0, 0);
            var endDate = new DateTime(2021, 7, 1, 0, 0, 0).AddSeconds(-1);
            ThoiGianLamViecProvider tgp = new ThoiGianLamViecProvider();
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var lstLuong = GetDataLuongThang(path + "LuongThang.xlsx", "LuongThang");
            List<DuAnViewModel> duAnHienThis = new List<DuAnViewModel>();
            foreach (var nguoiDung in nguoiDungs)
            {
                nguoiDung.DuAns = new List<DuAnViewModel>();
              foreach(var duAn in duAns)
                {
                    var time = tgp.GetTimeSpentOfNguoiDungInDuAnBy(duAn.DuAnId, nguoiDung.NguoiDungId, startDate, endDate);
                    if (time > 0 && lstLuong.Count(t => t.HoTen.ToUpper() == nguoiDung.HoTen.ToUpper()) > 0)
                    {
                        var luong = lstLuong.Where(t => t.HoTen.ToUpper() == nguoiDung.HoTen.ToUpper()).FirstOrDefault().LuongThang;
                        duAn.CostH = (int)(luong * time / (192 * 3600));
                        nguoiDung.DuAns.Add(duAn);
                        if (duAnHienThis.Count(t => t.DuAnId == duAn.DuAnId) == 0) duAnHienThis.Add(duAn);
                    }
                }
            }
            //
            var oldpath = AppDomain.CurrentDomain.BaseDirectory + "MauThongKeLuong.xlsx";
            var newdirect = AppDomain.CurrentDomain.BaseDirectory + "ThongKeLuong.xlsx";


        }
        public List<NguoiDungViewModel> GetDataLuongThang(string fileName, string sheetName)
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            ExcelOpenXml excel = new ExcelOpenXml(fileName, sheetName);
            var table = excel.ReadDataTable(2, false);
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    NguoiDungViewModel nd = new NguoiDungViewModel();
                    nd.HoTen = row[0].ToString();
                    nd.LuongThang = double.Parse(row[1].ToString());
                    lstToReturn.Add(nd);
                }
            }
            return lstToReturn;
        }



        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult ToDoList(int? duAnId, Guid? nguoiDungId, int? week, string keyWord, int? page, int? pageSize, int? year)
        {
            if (page == null || page == 0) page = 1;
            if (pageSize == null || pageSize == 0) pageSize = 5;
            if (duAnId == null) duAnId = 0;
            if (week == null)
            {
                var monday = Helpers.GetMonDayBy(DateTime.Now);
                week = Helpers.GetNumerWeek(monday);
                year = monday.Year;
            }
            if (nguoiDungId == null) nguoiDungId = GetUserID();
            var vm = _model.GetToDoListBy(duAnId.Value, nguoiDungId.Value, null, week.Value, keyWord, page.Value, pageSize.Value, year.Value);
            ViewBag.NguoiDungId = GetUserID();
            ViewBag.Quyen = _nguoiDungP.GetById(GetUserID()).Quyen;
            DateTime fromDate = Helpers.GetFirstMondayOfWeek(DateTime.Now.Year, week.Value);
            DateTime toDate = fromDate.AddDays(7).AddTicks(-1);
            vm.StrWeek = "Từ ngày " + String.Format("{0:dd/MM/yyyy}", fromDate).Substring(0, 5) + " đến ngày " + String.Format("{0:dd/MM/yyyy}", toDate).Substring(0, 5);
            vm.Year = year.Value;
            return View(vm);

        }
        [Authorize]
        public ActionResult TaskDetail(int taskId)
        {
            var vm = _model.GetTaskDetailById2(taskId);
            if (vm == null) return RedirectToAction("ToDoList");
            ViewBag.Quyen = _nguoiDungP.GetById(GetUserID()).Quyen;
            ViewBag.Now = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            ViewBag.NguoiDungId = GetUserID();
            var current_todo = _thoiGianLamViecModel.GetCurrentTodoCountTime(GetUserID());
            if (current_todo != null) vm.CongViecIsStartId = current_todo.CongViecId;
            return View(vm);
        }
        [Authorize]
        public ActionResult ReportUser(Guid? nguoiDungId, int? month)
        {
            var userId = GetUserID();
            var user = _nguoiDungP.GetById(userId);
            ViewBag.Quyen = user.Quyen;
            ViewBag.UserId = user.NguoiDungId;
            if (user.Quyen == 3 && nguoiDungId != null)
            {
                userId = nguoiDungId.Value;
            }
            var nguoiDung = _nguoiDungP.GetById(userId);
            var datetimeN = DateTime.Now;
            DateTime startDate;
            if (month == null || month == 1)
            {
                month = datetimeN.Month;
                nguoiDung.Month = 1;
                startDate = new DateTime(datetimeN.Year, datetimeN.Month, 1);
            }
            else
            {
                datetimeN = datetimeN.AddMonths(-1);
                startDate = new DateTime(datetimeN.Year, datetimeN.Month, 1);
                nguoiDung.Month = 0;
            }
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            var startTime = new DateTime(datetimeN.Year, datetimeN.Month, 1);
            var endTime = startTime.AddMonths(1).AddTicks(-1);
           
            var startDateToInt = (int)(startDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var endDateToInt = (int)(endDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var LichLamViecCaNhan = getLichLamViecCaNhan(nguoiDung.Email, startDateToInt, endDateToInt);
            string strTotalTime="0h";
            string strTotalTimeNeed="0h";
            string percel="0%";
            var dayTypes = getDanhSachLoaiNgayDangKy();
            nguoiDung.DayTypes = new List<DayType>();
            if (dayTypes != null && dayTypes.Count > 0)
            {
                foreach (var item in dayTypes)
                {
                    if (item.TimeRequireInSeconds == 0 && item.DayTypeId != 1)
                    {
                        nguoiDung.DayTypes.Add(item);
                    }
                }
                nguoiDung.LichLamViecCaNhans = _thoiGianLamViecModel.GetNgayLamViecsOfUser2(userId, startTime, endTime, LichLamViecCaNhan, out strTotalTime, out strTotalTimeNeed, out percel);
                ViewBag.CountStaffDay = CountNumberStaffDay(Guid.Parse("1ea0b760-36b2-4d2f-9e88-a16cc7f5ac68"), "13/3/2021");
            }
            else
            {
                nguoiDung.LichLamViecCaNhans = new List<LichLamViecCaNhanViewModel>();
                ViewBag.CountStaffDay = 0;
            }
            
            ViewBag.StrTotalTime = strTotalTime;
            ViewBag.StrTotalTimeNeed = strTotalTimeNeed;
            ViewBag.Percel = percel;
            ViewBag.HQ = String.Format("{0:MM/dd/yyyy}", DateTime.Now.AddDays(-1));
            DuAnModel duAnM = new DuAnModel();
            nguoiDung.DuAns = duAnM.GetsByNguoiDungId(nguoiDung.NguoiDungId);
           
            return View(nguoiDung);
        }
        [Authorize]
        public ActionResult PartalViewNgayLamViec(Guid nguoiDungId, string strNgayLamViec)
        {
            ThoiGianLamViecTrongNgayViewModel vm = new ThoiGianLamViecTrongNgayViewModel();
            DuAnModel duAnM = new DuAnModel();
            vm.NgayLamViec = DateTime.ParseExact(strNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            vm.ThoiGianLamViecs = _thoiGianLamViecModel.GetThoiGianLamViecOfUser2(nguoiDungId, vm.NgayLamViec);
            var nguoiDung = _nguoiDungP.GetById(nguoiDungId);
            vm.HoTen = nguoiDung.HoTen;
            if (vm.ThoiGianLamViecs != null && vm.ThoiGianLamViecs.Count > 0)
            {
                foreach (var item in vm.ThoiGianLamViecs)
                {
                    if (item.DuAnId != null)
                    {
                        var duAn = duAnM.GetShortInfoById(item.DuAnId.Value);
                        if (duAn != null)
                            item.TenDuAn = duAn.TenDuAn;
                        if (duAn.KhoaChaId != null)
                        {
                            var duAnCha = duAnM.GetShortInfoById(duAn.KhoaChaId.Value);
                            if (duAnCha != null)
                                item.TenDuAnCha = duAnCha.TenDuAn;
                        }
                    }
                    item.StrTongThoiGian = GetStrTime2(item.TongThoiGian);
                    if (item.ThoiGianBatDau != null)
                    {
                        item.StrThoiGianBatDau = item.ThoiGianBatDau.Value.ToString("hh:mm");
                    }
                    if (item.ThoiGianKetThuc != null)
                    {
                        item.StrThoiGianKetThuc = item.ThoiGianKetThuc.Value.ToString("hh:mm");
                    }
                }
            }
            return View(vm);
        }
        [Authorize]
        public ActionResult ChiTietNgayCong(string strNgayLamViec, Guid userId)
        {
            ThoiGianLamViecTrongNgayViewModel vm = new ThoiGianLamViecTrongNgayViewModel();
            DuAnModel duAnM = new DuAnModel();
            var nguoiDung = _nguoiDungP.GetById(userId);
            vm.NgayLamViec = DateTime.ParseExact(strNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var startDate = vm.NgayLamViec;
            var endDate = startDate.AddDays(1).AddSeconds(-1);
            var startDateToInt = (int)(startDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var endDateToInt = (int)(endDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var LichLamViecCaNhan = getLichLamViecCaNhan(nguoiDung.Email, startDateToInt, endDateToInt);
            vm.ThoiGianLamViecs = _thoiGianLamViecModel.GetThoiGianLamViecOfUser(userId, vm.NgayLamViec);

            vm.HoTen = nguoiDung.HoTen;
            ThoiGianLamViecProvider ttP = new ThoiGianLamViecProvider();
            var timeInRankTime = ttP.GetTimePheDuyetInRankTimeInDay(userId, startDate);

            if (vm.ThoiGianLamViecs != null && vm.ThoiGianLamViecs.Count > 0)
            {
                foreach (var item in vm.ThoiGianLamViecs)
                {
                    if (item.DuAnId != null)
                    {
                        var duAn = duAnM.GetById(item.DuAnId.Value, userId);
                        item.TenDuAn = duAn.TenDuAn;
                    }
                    item.StrTongThoiGian = GetStrTime2(item.TongThoiGian);
                    if (item.ThoiGianBatDau != null)
                    {
                        item.StrThoiGianBatDau = item.ThoiGianBatDau.Value.ToString("hh:mm tt");
                    }
                    if (item.ThoiGianKetThuc != null)
                    {
                        item.StrThoiGianKetThuc = item.ThoiGianKetThuc.Value.ToString("hh:mm tt");
                    }
                }
            }
            if (vm.NgayLamViec.DayOfWeek == DayOfWeek.Sunday)
            {
                vm.StrNgayLamViec = "Chủ nhật";
            }
            else
            {
                vm.StrNgayLamViec = "Thứ " + ((int)vm.NgayLamViec.DayOfWeek + 1);
            }
            vm.StrNgayLamViec += " | " + String.Format("{0:dd/MM/yyyy}", vm.NgayLamViec);
            PhongBanProvider pbP = new PhongBanProvider();
            var phongBanIds = pbP.GetIdsByNguoiDungId(userId);
            if (phongBanIds != null && phongBanIds.Count > 0 && phongBanIds.Contains((int)EnumPhongBanId.Sale))
            {
                var lst = _thoiGianLamViecModel.GetDanhSachActivity(nguoiDung.Email, startDateToInt, endDateToInt);
                if (LichLamViecCaNhan != null && LichLamViecCaNhan.Count > 0)
                {
                    var ngayLamViec = LichLamViecCaNhan[0];
                    //if (timeInRankTime < ngayLamViec.DayType.TimeRequireInSeconds)
                    //{
                    string tenCongViec = "";
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var item in lst)
                        {

                            if (!string.IsNullOrEmpty(item.NgayBatDau))
                            {
                                try
                                {
                                    item.DNgayBatDau = DateTime.ParseExact(item.NgayBatDau, "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
                                }
                                catch { }

                            }
                        }
                    }
                    var endTimeOfDay = ngayLamViec.Day.Value.AddDays(1).AddSeconds(-1);
                    var lstE = lst.Where(t => t.DNgayBatDau >= ngayLamViec.Day.Value && t.DNgayBatDau < endTimeOfDay).ToList();
                    if (lstE != null && lstE.Count > 0)
                    {
                        foreach (var item in lstE)
                        {
                            if (!string.IsNullOrEmpty(tenCongViec)) tenCongViec += "<br/>";
                            tenCongViec += item.TieuDe;
                        }
                    }
                    var count = lst.Count(t => t.DNgayBatDau >= ngayLamViec.Day.Value && t.DNgayBatDau < endTimeOfDay);
                    if (count > 0)
                    {
                        int timeall = timeInRankTime;
                        if (timeInRankTime < ngayLamViec.DayType.TimeRequireInSeconds)
                        {
                            timeall = timeInRankTime + count * 3600 * 4;
                            if (timeall > ngayLamViec.DayType.TimeRequireInSeconds) timeall = (int)ngayLamViec.DayType.TimeRequireInSeconds;
                        }


                        ThoiGianLamViecViewModel vmm = new ThoiGianLamViecViewModel()
                        {
                            ThoiGianLamViecId = 0,
                            LoaiTimer = 36,
                            TongThoiGian = timeall - timeInRankTime,
                            PheDuyet = true,
                            TenCongViec = tenCongViec,
                            StrTongThoiGian = GetStrTime2(timeall - timeInRankTime)
                        };
                        if (vm.ThoiGianLamViecs != null && vm.ThoiGianLamViecs.Count > 0) vm.ThoiGianLamViecs.Add(vmm);
                        else vm.ThoiGianLamViecs = new List<ThoiGianLamViecViewModel>() { vmm };
                    }

                    //}
                }
            }
            return View(vm);
        }

        [Authorize]
        public string GetCurrentTodoLogTime(Guid userId)
        {
            var result = _thoiGianLamViecModel.GetCurrentTodoCountTime(userId);
            if (result != null)
            {
                int newLog = (int)(DateTime.Now - result.ThoiGianBatDau).Value.TotalSeconds;
                int totalLog = newLog + GetTimeLogedOfTask(result.CongViecId);
                result.TongThoiGianLogChuaKetThuc = totalLog;
            }
            return JsonConvert.SerializeObject(result);
        }

        [Authorize]
        [HttpPost]
        public string StopTimer(int congViecId, Guid userId)
        {
            return JsonConvert.SerializeObject(_thoiGianLamViecModel.StopTodo(congViecId, userId));
        }

        [Authorize]
        [HttpPost]
        public string StopTimer2(int toDoId)
        {
            var result = _thoiGianLamViecModel.StopTodo(toDoId, GetUserID());
            if (result.Status)
            {
                _model.UpdateTrangThai(toDoId, (int)EnumTrangThaiCongViecType.todoBlock);
            }
            return JsonConvert.SerializeObject(result);
        }

        [Authorize]
        public int GetTimeLogedOfTask(int todoId)
        {
            var count = _thoiGianLamViecModel.GetTimeLogedOfTodo(todoId);
            return count;
        }

        public string AddOffTime(CongViecViewModel vm)
        {
            return JsonConvert.SerializeObject(_thoiGianLamViecModel.AddOffTime(vm, GetUserID()));
        }
        public string AddCongTac(CongViecViewModel vm)
        {
            return JsonConvert.SerializeObject(_thoiGianLamViecModel.AddCongTac(vm, GetUserID()));
        }
        public string AddQTA(CongViecViewModel vm)
        {
            if (!string.IsNullOrEmpty(vm.StrNgayLamViec))
            {
                vm.NgayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            }

            var token = GetTokenKey2(vm.NgayLamViec);
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                var nguoiDung = _nguoiDungP.GetById(GetUserID());
                RegisterToken(nguoiDung.Email, (int)(vm.NgayLamViec - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
                 token = GetTokenKey2(vm.NgayLamViec);
            }
            return JsonConvert.SerializeObject(_thoiGianLamViecModel.AddQTA2(vm, token, GetUserID()));
        }

        public string AddDayType(CongViecViewModel vm)
        {
            return JsonConvert.SerializeObject(_thoiGianLamViecModel.AddDayTime(vm, GetUserID()));
        }
        public string GetCongViecToLogTime(Guid nguoiDungId, string keyword)
        {
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var week = Helpers.GetNumerWeek(monday);

            var result = _model.GetListCongViecToLogTime(week, nguoiDungId, keyword);
            return JsonConvert.SerializeObject(result);
        }
        public string GetNguoiDungs()
        {
            NguoiDungModel model = new NguoiDungModel();
            return JsonConvert.SerializeObject(model.GetAll2());
        }
        public string GetTrangThaiToDos()
        {
            List<XacNhanHoanThanhViewModel> lstToReturn = new List<XacNhanHoanThanhViewModel>();
            lstToReturn.Add(new XacNhanHoanThanhViewModel() { XacNhanHoanThanh = 0, TenXacNhan = "Tất cả trạng thái" });
            lstToReturn.Add(new XacNhanHoanThanhViewModel() { XacNhanHoanThanh = 1, TenXacNhan = "Chưa hoàn thành" });
            lstToReturn.Add(new XacNhanHoanThanhViewModel() { XacNhanHoanThanh = 2, TenXacNhan = "Đã hoàn thành" });
            return JsonConvert.SerializeObject(lstToReturn);
        }

        [Authorize]
        public string GetWeeks3(int week, int year)
        {
            var maxWeek = Helpers.GetNumberWeekOfYears(year);
            List<WeekViewModel> results = new List<WeekViewModel>();
            for (int i = -1; i < 2; i++)
            {
                var week1 = week + i;
                if (week1 > maxWeek)
                {
                    week1 = 1;
                    year++;
                }
                if (week1 == 0)
                {
                    var lastYear = year - 1;
                    var lastWeek = Helpers.GetNumberWeekOfYears(lastYear);
                    results.Add(new WeekViewModel() { Week = lastWeek + "-" + lastYear, TenWeek = "Tuần " + lastWeek });
                }
                else
                    results.Add(new WeekViewModel() { Week = week1 + "-" + year, TenWeek = "Tuần " + week1 });
            }
            return JsonConvert.SerializeObject(results);
        }


        [Authorize]
        [HttpPost]
        public int StartTimer(int congViecId, Guid userId, string tokenId)
        {
            return _thoiGianLamViecModel.StartTimer(congViecId, userId, tokenId);
        }

        public string XacNhanHoanThanh(int toDoId)
        {
            return JsonConvert.SerializeObject(_model.XacNhanHoanThanhToDo(toDoId, GetUserID()));
        }

        public string XacNhanHoanThanhTask(int taskId)
        {
            return JsonConvert.SerializeObject(_model.XacNhanHoanThanhTask(taskId, GetUserID()));
        }

        public Guid GetUserID()
        {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return _nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }
        public string GetToDoById(int todoId)
        {
            var vm = _model.GetToDoDetailById(todoId);
            if (vm != null) return JsonConvert.SerializeObject(vm);
            return "";
        }

        public string GetTimeEntryById(int timeEntryId)
        {
            var vm = _model.GetTimeEntryDetailById(timeEntryId);
            if (vm != null) return JsonConvert.SerializeObject(vm);
            return "";
        }
        public int InsertOrUpdateToDo(CongViecViewModel vm)
        {
            vm.NguoiTaoId = GetUserID();
            return _model.InsertOrUpdateToDo(vm);
        }

        public string InsertOrUpdateTimeEntry(CongViecViewModel vm)
        {
            ResultViewModel result = new ResultViewModel();
            var userId = GetUserID();
            var nguoiDung = _nguoiDungP.GetById(userId);
            if (nguoiDung.Quyen == 1)
            {
                result.Status = false;
                result.Message = "Bạn không có quyền add time Entry";
            }
            else
            if (nguoiDung.Quyen == 2)
            {

                vm.XacNhanHoanThanh = null;
                if (DateTime.Now.Hour >= 8) vm.StrNgayLamViec = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                else vm.StrNgayLamViec = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(-1));
            }
            else if (nguoiDung.Quyen == 3)
            {
                vm.XacNhanHoanThanh = true;
            }
            vm.NguoiTaoId = userId;
            vm.NguoiXuLyId = userId;
            var token = GetTokenKey();
            var ngayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            if (token != null && !string.IsNullOrEmpty(token.key_token))
            {
                var current_todo = _thoiGianLamViecModel.GetCurrentTodoCountTime(userId);
                if (current_todo != null && current_todo.CongViecId > 0)
                {
                    result.Status = false;
                    result.Message = "Cần dừng công việc hiện tại để có thể đăng ký Công việc mới!";
                }
                else
                {
                    var startTime = getDateTime(ngayLamViec, vm.StrThoiGianBatDau);
                    var endTime = getDateTime(ngayLamViec, vm.StrThoiGianKetThuc);
                    if (endTime <= startTime)
                    {
                        result.Status = false;
                        result.Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.";
                    }
                    else
                    {
                        if (startTime < token.manufacturing_date)
                        {
                            result.Status = false;
                            result.Message = "Bạn không thể đăng ký trước giờ lấy token hôm nay của bạn là: " + token.manufacturing_date.ToString
                                ("hh:mm");
                        }
                        else
                        {
                            if (_thoiGianLamViecModel.CheckInsertTimeEntry(vm.StrThoiGianBatDau, vm.StrThoiGianKetThuc, ngayLamViec, userId))
                            {
                                var id = _model.InsertOrUpdateTimeEntry(vm, token.key_token, userId, ngayLamViec);
                                if (id > 0)
                                {
                                    result.Status = true;
                                    result.Message = "Đăng ký time Entry thành công.";
                                }
                                else
                                {
                                    result.Status = false;
                                    var khungBd = getDateTime(token.manufacturing_date, nguoiDung.KhungThoiGianBatDau);
                                    if (token.manufacturing_date >= khungBd)
                                        result.Message = "Khoảng thời gian đăng ký trong khoảng từ " + token.manufacturing_date.Hour + ":" + token.manufacturing_date.Minute + 1 + " đến " + nguoiDung.KhungThoiGianKetThuc;
                                    else result.Message = "Khoảng thời gian đăng ký trong khoảng từ " + khungBd.Hour + ":" + khungBd.Minute + " đến " + nguoiDung.KhungThoiGianKetThuc;
                                }
                            }
                            else
                            {
                                result.Status = false;
                                result.Message = "Thời gian đăng ký công việc không hợp lệ.";
                            }
                        }


                    }

                }


            }
            else
            {
                result.Status = false;
                result.Message
                    = "Cần đăng ký Token-Time để Add timeEntry";
            }
            //return _model.InsertOrUpdateTimeEntry(vm, GetTokenKey());
            return JsonConvert.SerializeObject(result);
        }

        public bool PheDuyetToDoOrTimeEntry(string id, bool check)
        {
            return _model.PheDuyetToDoOrTimeEntry(id, check);
        }

        public string GetTotalTime(int CongViecId)
        {
            return _model.GetAllTimeOfTask(CongViecId);
        }
        public bool CheckToken()
        {
            var str = GetTokenKey();
            if (str != null && string.IsNullOrEmpty(str.key_token)) return false;
            return true;
        }

        public bool DeleteTimeEntry(int timeEntryId)
        {
            return _model.DeleteTimeEntry(timeEntryId, GetUserID());
        }
        public bool DeleteToDo(string id)
        {
            return _model.DeleteToDo(id, GetUserID());
        }

        [Authorize]
        [HttpPost]
        public string StartTimer2(int toDoId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();


            var token = GetTokenKey();
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                result.Status = false;
                result.Message = "Cần đăng ký Token-Time để có thể bắt đầu công việc.!";
                result.ItemId = JsonConvert.SerializeObject(token);
            }
            else
            {
                var userId = GetUserID();
                var user = _nguoiDungP.GetById(userId);

                if (_thoiGianLamViecModel.CheckStartTime(userId))
                {
                    var id = _thoiGianLamViecModel.StartTimer(toDoId, userId, token.key_token);
                    if (id > 0)
                    {
                        result.Status = true;
                        result.ItemId = id.ToString();
                        result.Message = "Start time thành công.";
                        model.UpdateTrangThai(toDoId, (int)EnumTrangThaiCongViecType.todoDo);
                        Active();
                        var todo = model.GetById(toDoId);
                        _thoiGianLamViecModel.SendMessageToSlack(user.HoTen + " start công viêc: " + todo.TenCongViec);
                    }
                    else
                    {
                        result.Status = false;
                        result.ItemId = id.ToString();
                        result.Message = "Start time không thành công.";
                    }
                }
                else
                {
                    result.Status = false;
                    result.Message = "Bạn không thể start time trong thời gian đã đăng ký.";
                }


            }


            return JsonConvert.SerializeObject(result);
        }

        public string UpdateTrangThaiTask(int taskId, int trangThaiCongViecId)
        {
            var result = _model.UpdateTrangThai(taskId, trangThaiCongViecId);
            if (result)
            {
                TrangThaiCongViecProvider _manager = new TrangThaiCongViecProvider();
                var vm = _manager.GetById(trangThaiCongViecId);
                if (vm != null) return vm.MaMau + "-" + vm.TenTrangThai + "-" + vm.TrangThaiCongViecId;
            }
            return "";
        }
        [Authorize]
        [HttpPost]
        public string StartTimer3(int taskId)
        {
            var userId = GetUserID();
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var lstTodoIds = model.GetToDoIdsBy(taskId, userId);
            int toDoId = 0;
            if (lstTodoIds != null && lstTodoIds.Count > 0)
            {
                toDoId = lstTodoIds[0];
            }
            else
            {
                toDoId = model.InsertToDoByKhoaCha(taskId, userId);
            }
            var token = GetTokenKey();
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                RegisterToken(userId);
                token = GetTokenKey();
            }
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                result.Status = false;
                result.Message = "Cần đăng ký Token-Time để có thể bắt đầu công việc.!";
                result.ItemId = JsonConvert.SerializeObject(token);
            }
            else
            {
                if (_thoiGianLamViecModel.CheckStartTime(userId))
                {
                    var id = _thoiGianLamViecModel.StartTimer(toDoId, userId, token.key_token);
                    if (id > 0)
                    {
                        result.Status = true;
                        result.ItemId = id.ToString();
                        result.Message = "Start time thành công.";
                        model.UpdateTrangThai(toDoId, (int)EnumTrangThaiCongViecType.todoDo);
                        var task = model.GetById(taskId);
                        if (task.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.congviecDoing)
                            model.UpdateTrangThai(taskId, (int)EnumTrangThaiCongViecType.congviecDoing);
                        Active();
                        var nguoiDung = _nguoiDungP.GetById(userId);

                        _thoiGianLamViecModel.SendMessageToSlack(nguoiDung.HoTen + " start công viêc: " + task.TenCongViec);

                    }
                    else
                    {
                        result.Status = false;
                        result.ItemId = id.ToString();
                        result.Message = "Start time không thành công.";
                    }
                }
                else
                {
                    result.Status = false;
                    result.Message = "Bạn không thể start time trong thời gian đã đăng ký.";
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public bool Active()
        {
            var user = GetUserID();
            var date = DateTime.Now;
            var check = true;
            var active = _nguoiDungP.GetActiveBy(user, new DateTime(date.Year, date.Month, date.Day, 5, 0, 0));
            if (active != null)
            {
                if (active.Active) check = false;
                else check = true;
            }
            return _nguoiDungP.InsertOrUpdateActivity("Active", check, user, null);
        }
        public bool Active(Guid user)
        {          
            var date = DateTime.Now;
            var check = true;
            var active = _nguoiDungP.GetActiveBy(user, new DateTime(date.Year, date.Month, date.Day, 5, 0, 0));
            if (active != null)
            {
                if (active.Active) check = false;
                else check = true;
            }
            return _nguoiDungP.InsertOrUpdateActivity("Active", check, user, null);
        }
        public string CheckStartTime3(int taskId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var congViec = model.GetById(taskId);
            var userId = GetUserID();


            var token = GetTokenKey();
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                RegisterToken(userId);
                token = GetTokenKey();
            }
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                result.Message = "Bạn chưa lấy token key ngày hôm nay? Bạn hãy lấy token để start công việc. ";
                result.ItemId = "0";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result.Status = true;
            }

            if (token != null && !string.IsNullOrEmpty(token.key_token))
            {

            }
            else
            {

            }
            ThoiGianLamViecModel model2 = new ThoiGianLamViecModel();
            var currentToDo = model2.GetCurrentTodoCountTime(congViec.NguoiTaoId);
            if (currentToDo != null)
            {
                var task = model.GetTenCongViecByKhoaChaId(currentToDo.KhoaChaId.Value).FirstOrDefault();
                result.Message = "Bạn đang chạy thời gian trên công việc: " + task.TenCongViec;
                result.Status = false;
                result.ItemId = "1";
            }
            else
            {
                result.Status = true;
            }
            return JsonConvert.SerializeObject(result);
        }


        public ResultViewModel StartTimer4(int taskId,Guid userId)
        {         
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var lstTodoIds = model.GetToDoIdsBy(taskId, userId);
            int toDoId = 0;
            if (lstTodoIds != null && lstTodoIds.Count > 0)
            {
                toDoId = lstTodoIds[0];
            }
            else
            {
                toDoId = model.InsertToDoByKhoaCha(taskId, userId);
            }
            var token = GetTokenKey(userId);
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                RegisterToken(userId);
                token = GetTokenKey(userId);
            }
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                result.Status = false;
                result.Message = "Cần đăng ký Token-Time để có thể bắt đầu công việc.!";
                result.ItemId = JsonConvert.SerializeObject(token);
            }
            else
            {
                var id = _thoiGianLamViecModel.StartTimer(toDoId, userId, token.key_token);
                if (id > 0)
                {
                    result.Status = true;
                    result.ItemId = id.ToString();
                    result.Message = "Start time thành công.";
                    model.UpdateTrangThai(toDoId, (int)EnumTrangThaiCongViecType.todoDo);
                    var task = model.GetById(taskId);
                    if (task.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.congviecDoing)
                        model.UpdateTrangThai(taskId, (int)EnumTrangThaiCongViecType.congviecDoing);
                    Active(userId);
                    var nguoiDung = _nguoiDungP.GetById(userId);
                    _thoiGianLamViecModel.SendMessageToSlack(nguoiDung.HoTen + " start công viêc: " + task.TenCongViec);
                }
                else
                {
                    result.Status = false;
                    result.ItemId = id.ToString();
                    result.Message = "Start time không thành công.";
                }

            }
            return result;
        }

        public string GetShipAbleBy(int duAnId, Guid userId)
        {
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var week = Helpers.GetNumerWeek(monday);
            var year = monday.Year;
            return JsonConvert.SerializeObject(_model.GetShipablesBy3(duAnId, userId, week, year));
        }
        public string GetTaskBy(int shipId, int duAnId, Guid userId)
        {
            var week = Helpers.GetNumerWeek(DateTime.Now);
            return JsonConvert.SerializeObject(_model.GetTasksBy(duAnId, shipId, userId, week));
        }

        public string GetLoaiCongViecByTaskId(int taskId)
        {
            var lst = _model.GetLoaiCongViecsByTaskId(taskId);
            return JsonConvert.SerializeObject(lst);
        }

        private string GetTokenKey(DateTime date)
        {
            var userId = GetUserID();
            var time = (date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var client = new RestClient("http://113.190.243.226:8050/api/Token/GetMyToken/" + userId.ToString() + "/" + time);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<string>(response.Content);
        }
        private TokenViewModel GetTokenKey2(DateTime date)
        {
            try
            {
                var userId = GetUserID();
                var time = (date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                var client = new RestClient("http://token.tecotec.vn/api/Token/GetMyTokenV2/" + userId.ToString() + "/" + time);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<TokenViewModel>(response.Content);
            }
            catch
            {
                return new TokenViewModel();
            }

        }
        private TokenViewModel GetTokenKey()
        {
            try
            {
                var userId = GetUserID();
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiGetTokenKey") + "/" + userId.ToString());
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<TokenViewModel>(response.Content);
            }
            catch
            {
                return null;
            }

        }
        private TokenViewModel GetTokenKey(Guid userId)
        {
            try
            {              
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiGetTokenKey") + "/" + userId.ToString());
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<TokenViewModel>(response.Content);
            }
            catch
            {
                return null;
            }

        }
        private List<DayType> getDanhSachLoaiNgayDangKy()
        {
            try
            {
                var userId = GetUserID();
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiDanhMucLoaiNgayDangKy"));
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<List<DayType>>(response.Content);
                //return response.Content;
            }
            catch
            {
                return null;
            }

        }
        private List<LichLamViecCaNhanViewModel> getLichLamViecCaNhan(string email, int startDateToInt, int endDateToInt)
        {
            try
            {
                var userId = GetUserID();
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiLichLamViecCaNhan") + "?email=" + email + "&startDate=" + startDateToInt + "&endDate=" + endDateToInt);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<List<LichLamViecCaNhanViewModel>>(response.Content);
                //return response.Content;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private DateTime getDateTime(DateTime date, string time)
        {
            //string timeString = "11/12/2009 13:30:00.000";
            IFormatProvider culture = new CultureInfo("en-US", true);
            //DateTime dateVal = DateTime.ParseExact(timeString, "dd/MM/yyyy HH:mm:ss.fff", culture);


            var str = "";
            var day = date.Day;
            if (day < 10) str += "0" + day;
            else str += day;
            var month = date.Month;
            if (month < 10) str += "/0" + month;
            else str += "/" + month;
            str += "/" + date.Year + " " + time;
            str += ":00.000";
            return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm:ss.fff", culture);
        }
        private string GetStrTime2(int timeSpent)
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
        public ActionResult BangChamCong()
        {
            return View();
        }
        public bool DeleteThoiGian(int id)
        {
            return _thoiGianLamViecModel.DeleteThoiGianLamViec(id, GetUserID());
        }

        public int CountNumberStaffDay(Guid nguoiDungId, string time)
        {
            var nguoiDung = _nguoiDungP.GetById(nguoiDungId);
            DateTime day;
            DateTime.TryParseExact(time, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out day);
            if (day.Year < 2000) day = DateTime.Now;
            if (nguoiDung != null)
            {
                if (_staffDay == null)
                {
                    var a = StaffDay;
                }
                return _staffDay.CountStaffDayOfUser(nguoiDung.Email, day.Year, true);
            }
            return 0;

        }

        public ResultViewModel RegisterToken(Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            var nguoiDung = _nguoiDungP.GetById(userId);
            if (nguoiDung != null)
            {
                var link = System.Configuration.ConfigurationManager.AppSettings.Get("RegisterToken");
                var client = new RestClient(link);
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/json");
                RegisterMyTokenViewModel vm = new RegisterMyTokenViewModel();
                vm.UserName = nguoiDung.TenDangNhap;
                vm.Password = EndCode.Decrypt(nguoiDung.MatKhau);
                var json = JsonConvert.SerializeObject(vm);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
            }
            return result;
        }
        public bool RegisterToken(string email, int ngayLamViecToInt)
        {
            NguoiDungProvider ndP = new NguoiDungProvider();
            var link = System.Configuration.ConfigurationManager.AppSettings.Get("RegisterToken3");
            var client = new RestClient(link);
            var request = new RestRequest(Method.POST);
            var nguoiDung = ndP.GetNguoiDungByEmail(email);
            request.AddHeader("Content-Type", "application/json");
            RegisterMyTokenViewModel vm = new RegisterMyTokenViewModel();
            vm.UserName = nguoiDung.TenDangNhap;
            vm.Password = EndCode.Decrypt(nguoiDung.MatKhau);
            vm.DateTimeToInt = ngayLamViecToInt;
            var json = JsonConvert.SerializeObject(vm);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return true;
        }
        public class RegisterMyTokenViewModel
        {
            public string ClientReqCode { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int DateTimeToInt { get; set; }

        }

       
        public class SendMessageToSlackViewModel
        {
            public string channel { get; set; }
            public string text { get; set; }
        }
    }
}