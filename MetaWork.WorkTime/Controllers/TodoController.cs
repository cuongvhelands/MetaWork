using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MetaWork.WorkTime.Models;
using MetaWork.Data.ViewModel;
using MetaWork.Data.Provider;
using System.Globalization;
using System.Web.Security;
using RestSharp;
using static MetaWork.WorkTime.Controllers.TimeController;
using System.Net.Mail;
using MetaWork.Data.SQL;
using MetaWork.Data.Model;

namespace MetaWork.WorkTime.Controllers
{
    public class TodoController : Controller
    {
        string docId;
        string doc;
        string cellShip;
        string codaApi, tableName;
        // GET: Todo
        CongViecModel _congViecM = new CongViecModel();
        NguoiDungModel _nguoiDungM = new NguoiDungModel();
        LoaiCongViecProvider _loaiCongViecProvider = new LoaiCongViecProvider();
        LoaiCongViecModel _loaiCongViecModel = new LoaiCongViecModel();
        PhongBanModel _phongBanModel = new PhongBanModel();
        DuAnModel _duAnM = new DuAnModel();

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




        [Authorize]
        public ActionResult Index()
        {
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            var results = _congViecM.GetBy(0, DateTime.Now.Year, tuan, 0);
            ViewBag.Tuan = tuan;
            return View(results);
        }
        [Authorize]
        public ActionResult Planning(string next_week, int? duAnId, int? trangThaiCongViecId)
        {
            var userId = GetUserID();
            var user = _nguoiDungM.GetById(userId);
            ViewBag.QuyenNguoiDung = user.Quyen;
            ViewBag.NguoiDungId = user.NguoiDungId;
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var tuan = Helpers.GetNumerWeek(monday);
            var year = monday.Year;
            var maxWeek = Helpers.GetNumberWeekOfYears(year);
            if (!string.IsNullOrEmpty(next_week))
            {
                if (tuan == maxWeek)
                {
                    tuan = 1;
                    year++;
                }
                else

                    tuan = tuan + 1;
            }
            if (duAnId == null) duAnId = 0;
            if (trangThaiCongViecId == null) trangThaiCongViecId = 0;
            var results = _congViecM.GetBy(duAnId.Value, year, tuan, trangThaiCongViecId.Value);

            var firstDayOfThisWeek = Helpers.GetFirstMondayOfWeek(year, tuan);
            var lastDayOfThisWeek = firstDayOfThisWeek.AddDays(5);

            var firstDayOfLastWeek = Helpers.GetFirstMondayOfWeek(year, tuan - 1);
            var lastDayOfLastWeek = firstDayOfLastWeek.AddDays(5);
            string lastWeekVal = "Từ " + firstDayOfLastWeek.Day + " tháng " + firstDayOfLastWeek.Month + " đến " + lastDayOfLastWeek.Day + " tháng " + lastDayOfLastWeek.Month;
            ViewBag.LastWeekVal = lastWeekVal;

            ViewBag.FirstDay = firstDayOfThisWeek.ToString("MM-dd-yyyy");
            ViewBag.LastDay = lastDayOfThisWeek.ToString("MM-dd-yyyy");
            string thisWeekVal = "Từ " + firstDayOfThisWeek.Day + " tháng " + firstDayOfThisWeek.Month + " đến " + lastDayOfThisWeek.Day + " tháng " + lastDayOfThisWeek.Month;
            ViewBag.ThisWeekVal = thisWeekVal;
            ViewBag.Tuan = tuan;
            if (!string.IsNullOrEmpty(next_week))
            {
                results.TuanTiepTheo = true;
            }
            return View(results);
        }

        //public ActionResult WorkSheet()
        //{
        //    var result = _congViecM.GetToDoInWeekBy(null, null, null, null, null);
        //    return View(result);
        //}       
        [Authorize]
        public ActionResult WorkSheet(int? phongBanId, Guid? userId, string strLoaiCongViecId)
        {
            if (!phongBanId.HasValue)
            {
                phongBanId = 1;
            }
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            ViewBag.Tuan = tuan;
            ViewBag.PhongBanId = phongBanId;
            ViewBag.UserId = userId;
            List<int> LoaiCongViecIds = new List<int>();
            if (!string.IsNullOrEmpty(strLoaiCongViecId) && strLoaiCongViecId != "null")
            {
                var col = strLoaiCongViecId.Split(',');
                foreach (var item in col)
                {
                    LoaiCongViecIds.Add(int.Parse(item));
                }
            }
            var result = _congViecM.GetToDoInWeekBy(null, null, phongBanId, userId, LoaiCongViecIds);
            result.StrLoaiCongViec = strLoaiCongViecId;
            return View(result);
        }
        [Authorize]
        public ActionResult WorkSheetSummary(int? phongBanId, Guid? userId, string strLoaiCongViecId)
        {
            if (!phongBanId.HasValue)
            {
                phongBanId = 1;
            }
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            ViewBag.Tuan = tuan;
            ViewBag.PhongBanId = phongBanId;
            ViewBag.UserId = userId;
            List<int> LoaiCongViecIds = new List<int>();
            if (!string.IsNullOrEmpty(strLoaiCongViecId) && strLoaiCongViecId != "null")
            {
                var col = strLoaiCongViecId.Split(',');
                foreach (var item in col)
                {
                    LoaiCongViecIds.Add(int.Parse(item));
                }
            }
            var result = _congViecM.GetToDoInWeekBy(null, null, phongBanId, userId, LoaiCongViecIds);
            result.StrLoaiCongViec = strLoaiCongViecId;
            return View(result);
        }
        [Authorize]
        public Guid GetUserID()
        {
            NguoiDungProvider nguoiDungP = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }
        [Authorize]
        public ActionResult ToDoWork(string strDuAnId, string strTrangThaiDuAnId, string strPhongBanIds, bool? quanTam, int? trangThaiCongViecId, string tenShipAble, byte? type, string strNguoiDungId, string strStartDate, string strEndDate)
        {
            tenShipAble = string.Empty;
            var userId = GetUserID();
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            ViewBag.TuanHienTai = GetNumerWeekOfYears2(monday);
            ViewBag.Year = monday.Year;
            var nguoiDung = _nguoiDungM.GetById(userId);
            PhongBanModel pbM = new PhongBanModel();
            var phongban = pbM.GetByNguoiDungId(userId);
            if (string.IsNullOrEmpty(strStartDate) && string.IsNullOrEmpty(strEndDate) && type == null)
            {
                strNguoiDungId = userId.ToString();
                strTrangThaiDuAnId = (int)EnumTrangThaiDuAnType.Active + "";
            }
            List<int> duAnIds = new List<int>();
            if (!string.IsNullOrEmpty(strDuAnId))
            {
                try
                {
                    var col = strDuAnId.Split(',');
                    foreach (var item in col)
                    {
                        duAnIds.Add(int.Parse(item));
                    }
                }
                catch { }
            }
            List<int> trangThaiDuAnIds = new List<int>();
            if (!string.IsNullOrEmpty(strTrangThaiDuAnId))
            {
                try
                {
                    var col = strTrangThaiDuAnId.Split(',');
                    foreach (var item in col)
                    {
                        trangThaiDuAnIds.Add(int.Parse(item));
                    }
                }
                catch { }
            }
            List<int> phongBanIds = new List<int>();
            if (!string.IsNullOrEmpty(strPhongBanIds))
            {
                try
                {
                    var col = strPhongBanIds.Split(',');
                    foreach (var item in col)
                    {
                        phongBanIds.Add(int.Parse(item));
                    }
                }
                catch { }
            }
            else
            {
                if (string.IsNullOrEmpty(strStartDate) && string.IsNullOrEmpty(strEndDate) && type == null)
                {
                    phongBanIds.Add(phongban.PhongBanId);
                    strPhongBanIds = phongban.PhongBanId.ToString();
                }
            }

            ToDoWorkViewModel vm = new ToDoWorkViewModel() { Type = type ?? 0, TrangThaiCongViecId = trangThaiCongViecId ?? 0, DuAnIds = duAnIds ?? null, QuanTam = quanTam ?? false, TenShipable = tenShipAble, PhongBanIds = phongBanIds ?? null, StrNguoiDungId = strNguoiDungId, strDuAnIds = strDuAnId, StrPhongBanIds = strPhongBanIds, StrTrangThaiDuAnIds = strTrangThaiDuAnId };
            DateTime startDate;
            DateTime endDate;
            if (vm.Type == 0)
            {
                var day = DateTime.Now.AddDays(-30);
                startDate = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
                endDate = startDate.AddDays(31).AddTicks(-1);
            }
            else
            if (vm.Type == 1)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                endDate = startDate.AddDays(7).AddTicks(-1);
            }
            else if (vm.Type == 2)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now).AddDays(-7);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                endDate = startDate.AddDays(7).AddTicks(-1);
            }
            else if (vm.Type == 11)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            else if (vm.Type == 12)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(-1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            else
            {
                if (string.IsNullOrEmpty(strStartDate) || string.IsNullOrEmpty(strEndDate))
                {
                    endDate = DateTime.Now;
                    startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                }
                else
                {
                    startDate = DateTime.ParseExact(strStartDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                    endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.CurrentCulture).AddDays(1).AddTicks(-1);
                }
                vm.StrStartDate = startDate.ToString("dd/MM/yyyy");
                vm.StrEndDate = endDate.ToString("dd/MM/yyyy");
            }
            List<Guid> nguoiDungIds = new List<Guid>();
            if (!string.IsNullOrEmpty(strNguoiDungId))
            {
                var col = strNguoiDungId.Split(',');
                foreach (var item in col)
                {
                    nguoiDungIds.Add(Guid.Parse(item));
                }
            }

            vm.DuAns = _congViecM.GetToDoWorksBy(userId, vm.DuAnIds, trangThaiDuAnIds, vm.PhongBanIds, nguoiDungIds, vm.TrangThaiCongViecId, vm.QuanTam, tenShipAble, startDate, endDate);
            DuAnModel duAnM = new DuAnModel();
            vm.DuAnAll = duAnM.GetsByNguoiDungId(userId);
            var lstTTShip = GetByKhoaCha((int)EnumTrangThaiCongViecType.shipable);
            if (lstTTShip != null && lstTTShip.Count > 0) lstTTShip.RemoveAt(0);
            vm.TrangThaiShipables = lstTTShip;
            var lstTTTask = GetByKhoaCha((int)EnumTrangThaiCongViecType.congviec);
            if (lstTTTask != null && lstTTTask.Count > 0) lstTTTask.RemoveAt(0);
            vm.TrangThaiTasks = lstTTTask;
            var nguoidung = _nguoiDungM.GetById(GetUserID());
            vm.NguoiDungAll = _nguoiDungM.GetsByPhongBanId(nguoidung.PhongBanId);
            vm.TimeTypes = GetsTimes();
            var currentTask = _congViecM.GetcurrentTask(userId);
            vm.CurrentTask = currentTask;
            vm.CurrentTodoId = _congViecM.GetCurrentToDoId(userId);
            MauGoiChuyenGiaoProvider mg = new MauGoiChuyenGiaoProvider();
            vm.MauGoiChuyenGiaos = mg.GetAll();
            ViewBag.Ma = _congViecM.GetMaShipable();
            ViewBag.NguoiDungIdM = userId;
            return View(vm);
        }
        private List<TimeTypeViewModel> GetsTimes()
        {
            List<TimeTypeViewModel> lstToReturn = new List<TimeTypeViewModel>();
            lstToReturn.Add(new TimeTypeViewModel() { Type = 0, Name = "30 ngày gần nhất" });
            lstToReturn.Add(new TimeTypeViewModel() { Type = 1, Name = "Tuần này" });
            lstToReturn.Add(new TimeTypeViewModel() { Type = 2, Name = "Tuần trước" });
            lstToReturn.Add(new TimeTypeViewModel() { Type = 11, Name = "Tháng này" });
            lstToReturn.Add(new TimeTypeViewModel() { Type = 12, Name = "Tháng trước" });
            lstToReturn.Add(new TimeTypeViewModel() { Type = 21, Name = "Khoảng thời gian" });
            return lstToReturn;
        }
        private List<TrangThaiCongViecViewModel> GetByKhoaCha(int khoaChaId)
        {
            TrangThaiCongViecProvider ttM = new TrangThaiCongViecProvider();
            TrangThaiCongViecViewModel tt = ttM.GetById(khoaChaId);
            List<TrangThaiCongViecViewModel> lstToReturn = new List<TrangThaiCongViecViewModel>();
            lstToReturn.Add(tt);
            var ttcs = ttM.GetByKhoaChaId(khoaChaId);
            if (ttcs != null && ttcs.Count > 0)
            {
                foreach (var ttc in ttcs)
                {
                    lstToReturn.AddRange(GetByKhoaCha(ttc.TrangThaiCongViecId));
                }
            }
            return lstToReturn;
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
        [HttpPost]
        public int InsertTemplate(MauGoiChuyenGiaoViewModel vm)
        {
            MauGoiChuyenGiaoProvider manager = new MauGoiChuyenGiaoProvider();
            return manager.Insert(vm.TenMau, vm.TenShip, vm.NoiDung, vm.StrTask, GetUserID());
        }



        [Authorize]
        public string GetTeamplateById(int id)
        {
            MauGoiChuyenGiaoProvider manager = new MauGoiChuyenGiaoProvider();
            return JsonConvert.SerializeObject(manager.GetById(id));
        }

        [Authorize]
        public string GetAllTeamplate()
        {
            MauGoiChuyenGiaoProvider manager = new MauGoiChuyenGiaoProvider();
            return JsonConvert.SerializeObject(manager.GetAll());
        }


        #region method
        [Authorize]
        public string GetNguoiDung(int DuAnId)
        {
            var result = _nguoiDungM.GetByDuAnId(DuAnId);
            return JsonConvert.SerializeObject(result);
        }
        [Authorize]
        public string GetNguoiDung2(int duAnId, Guid nguoiXuLyId)
        {
            var result = _nguoiDungM.GetByDuAnId2(duAnId, nguoiXuLyId);
            return JsonConvert.SerializeObject(result);
        }

        [Authorize]
        public string GetNguoiDung3(int duAnId)
        {
            var result = _nguoiDungM.GetByDuAnId(duAnId);
            var nguoidung = _nguoiDungM.GetById(GetUserID());
            var result2 = _nguoiDungM.GetsByPhongBanId(nguoidung.PhongBanId);
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (lstToReturn.Where(t => t.NguoiDungId == item.NguoiDungId).Count() == 0) lstToReturn.Add(item);
                }
            }
            if (result2 != null && result2.Count > 0)
            {
                foreach (var item in result2)
                {
                    if (lstToReturn.Where(t => t.NguoiDungId == item.NguoiDungId).Count() == 0) lstToReturn.Add(item);
                }
            }
            return JsonConvert.SerializeObject(lstToReturn);
        }


        public string GetShipablesBy(int? duAnId, int tuan)
        {
            return JsonConvert.SerializeObject(_congViecM.GetShipablesBy(duAnId, tuan));
        }

        public string GetLoaiCongViec(int DuAnId)
        {
            var result = _loaiCongViecProvider.GetIdsByDuAn(DuAnId);
            return JsonConvert.SerializeObject(result);
        }

        public string GetDuAnThanhPhans(int duAnId)
        {
            var result = _duAnM.GetDuAnThanhPhanByKhoaCha(duAnId);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public int InsertTodo(CongViecViewModel model, string date)
        {
            var check = false;
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    model.NgayDuKienHoanThanh = dt;
                    check = true;
                }
            }
            if (!check)
            {
                model.NgayDuKienHoanThanh = monday.AddDays(4);
            }
            model.LaTask = true;
            model.Nam = monday.Year;
            return _congViecM.InserCongViec(model);
        }
        [Authorize]
        [HttpPost]
        public bool UpdateStatus(int CongViecId, int TrangThaiId)
        {
            return _congViecM.UpdateTrangThai(CongViecId, TrangThaiId);
        }
        [Authorize]
        [HttpGet]
        public bool UpdateQuanTam(int CongViecId)
        {
            return _congViecM.UpdateQuanTam(CongViecId, GetUserID());
        }
        [HttpPost]
        public bool UpdateStatusShipable(int CongViecId, int TrangThaiId)
        {
            return _congViecM.UpdateTrangThaiShipable(CongViecId, TrangThaiId, GetUserID());
        }
        [Authorize]
        [HttpPost]
        public string UpdateTodo(CongViecViewModel model, string date)
        {
            try
            {
                model.NgayDuKienHoanThanh = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {

            }

            model.LaTask = true;
            return JsonConvert.SerializeObject(_congViecM.UpdateTodo(model, GetUserID()));
        }
        [Authorize]
        [HttpPost]
        public bool DeleteTodo(int congViecId)
        {
            return _congViecM.DeleteCongViec(congViecId);
        }
        [Authorize]
        [HttpPost]
        public bool DeleteTodo2(int congViecId)
        {
            return _congViecM.DeleteToDoV2(congViecId, GetUserID());
        }
        [Authorize]
        [HttpPost]
        public bool DeleteTask(int taskId)
        {
            return _congViecM.DeleteTask(taskId, GetUserID());
        }
        [Authorize]
        public string GetAllNguoiDung()
        {
            var result = _nguoiDungM.GetAll();
            return JsonConvert.SerializeObject(result);
        }

        [Authorize]
        public string GetNguoiDungByPhongBanId(int phongBanId)
        {
            var result = _nguoiDungM.GetsByPhongBanId(phongBanId);
            return JsonConvert.SerializeObject(result);
        }
        [Authorize]
        public string GetAllLoaiCongViec()
        {
            var result = _loaiCongViecModel.GetAll();
            return JsonConvert.SerializeObject(result);
        }
        public string GetCongViecById(int CongViecId)
        {
            var result = _congViecM.GetById2(CongViecId);
            return JsonConvert.SerializeObject(result);
        }
        public decimal GetPointNotUsedInWeek(int week, Guid? userId)
        {
            if (userId == null) return 6;
            var n = _congViecM.GetPointUsedInWeek(week, userId.Value);
            return (6 - n);
        }

        public bool InsertShipableDebit(int shipableId)
        {
            return _congViecM.InsertShipableDebit(shipableId, GetUserID());
        }
        public bool ContinueShip(int shipId)
        {
            return _congViecM.UpdateTrangThaiShipable(shipId, (int)EnumTrangThaiCongViecType.shipableContinue, GetUserID());
        }
        public string GetPhongBan()
        {
            var result = _phongBanModel.GetAll();
            return JsonConvert.SerializeObject(result);
        }
        #endregion



        [Authorize]
        [HttpPost]
        public int AddShipable(CongViecViewModel vm)
        {
            tableName = "";
            var userId = GetUserID();
            var shipId = _congViecM.InsertShipableToDoWork(vm, userId);
            return shipId;
        }

        public bool DongBoShipableToCoda(int shipId)
        {
            if (shipId > 0)
            {
                var vm = _congViecM.GetInfoShip(shipId);
                if (vm != null)
                {
                    CauHinhProvider ch = new CauHinhProvider();
                    var linkCoda = ch.GetValueByTen("Project/Edit/" + vm.DuAnId);
                    if (string.IsNullOrEmpty(linkCoda))
                    {
                        linkCoda = System.Configuration.ConfigurationManager.AppSettings.Get("LinkProjectEdit");
                    }
                    var duan = GetShipablesFromDuAn(vm.DuAnId, linkCoda);
                    if (duan != null)
                    {
                        var strShip = "";
                        var ship = duan.Document;
                        if (!string.IsNullOrEmpty(duan.Document))
                        {
                            strShip = duan.Document;
                            var lstString = new List<string>();
                            var str = duan.Document;
                            while (str.IndexOf("\\n") != -1)
                            {
                                var tenCongViec = str.Substring(0, str.IndexOf("\\n"));
                                if(!string.IsNullOrEmpty(tenCongViec))
                                lstString.Add(tenCongViec.Trim());
                                str = str.Substring(str.IndexOf("\\n") + 2);
                            }
                            if (!string.IsNullOrEmpty(str))
                            {
                                lstString.Add(str);
                            }
                            if (lstString.Count(t => t.ToUpper() == vm.TenCongViec.Trim().ToUpper()) == 0)
                            {
                                strShip += "\\n " + vm.TenCongViec;
                            }
                        }
                        else
                        {
                            strShip = vm.TenCongViec;
                        }
                        UpdateShipable(duan.name, strShip);
                    }
                }
            }
            return true;
        }

        public bool UpdateShipable(string rowName, string value)
        {

            var link = codaApi + "/tables/" + tableName + "/rows/" + rowName;
            var client = new RestClient(link);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            request.AddHeader("Content-Type", "application/json");
            RowCodaViewModel row = new RowCodaViewModel();
            row.row = new RowCoda();
            row.row.cells = new List<CellCoda>();
            row.row.cells.Add(new CellCoda() { column = cellShip, value = value });
            var json = JsonConvert.SerializeObject(row).Replace("\\\\\\\\n", "\\n").Replace("\\\\n", "\\n");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var cccc = response.StatusCode;

            }
            else
            {
                var cccc = response.StatusCode;
            }

            return true;

        }

        [Authorize]
        public ResultViewModel CheckLinkCoda()
        {
            ResultViewModel result = new ResultViewModel();
            try
            {

                var client = new RestClient(codaApi);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
                request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    result.Status = true;

                }
                else
                {
                    result.Message = "Không kết nối được đến Coda với link =" + docId;
                }

            }
            catch
            {
                result.Message = "Không kết nối được đến  Coda với link =" + docId;
            }
            return result;
        }
        [Authorize]
        [HttpPost]
        public int UpdateShipable(CongViecViewModel vm)
        {
            return _congViecM.UpdateShipable(vm);
        }

        [Authorize]
        [HttpPost]
        public bool AddTask(CongViecViewModel vm)
        {
            var userId = GetUserID();
            return _congViecM.InsertTaskToDoWork(vm, userId);
        }

        [Authorize]
        [HttpGet]
        public string GetInfoShip(int shipId)
        {
            return JsonConvert.SerializeObject(_congViecM.GetInfoShip(shipId));
        }
        [Authorize]
        [HttpGet]
        public string GetInfoTask(int taskId)
        {
            return JsonConvert.SerializeObject(_congViecM.GetTaskInfo(taskId));
        }
        [Authorize]
        [HttpGet]
        public string GetPhongBans()
        {
            PhongBanModel model = new PhongBanModel();
            return JsonConvert.SerializeObject(model.GetAll());
        }
        [Authorize]
        [HttpGet]
        public string GetDuAns()
        {
            DuAnModel model = new DuAnModel();
            var userid = GetUserID();
            return JsonConvert.SerializeObject(model.GetsByNguoiDungId(userid));
        }
        [Authorize]
        [HttpGet]
        public string GetNguoiDungsBy(string strDuAnIds, string strPhongBanIds)
        {
            NguoiDungModel model = new NguoiDungModel();

            List<int> duAnIds = new List<int>();
            if (!string.IsNullOrEmpty(strDuAnIds))
            {
                try
                {
                    var col = strDuAnIds.Split(',');
                    foreach (var item in col)
                    {
                        duAnIds.Add(int.Parse(item));
                    }
                }
                catch { }
            }
            List<int> phongBanIds = new List<int>();
            if (!string.IsNullOrEmpty(strPhongBanIds))
            {
                try
                {
                    var col = strPhongBanIds.Split(',');
                    foreach (var item in col)
                    {
                        phongBanIds.Add(int.Parse(item));
                    }
                }
                catch { }
            }

            return JsonConvert.SerializeObject(model.GetTenNguoiDungBy(phongBanIds, duAnIds));
        }


        public string GetNguoiDungAll()
        {
            return JsonConvert.SerializeObject(_phongBanModel.GetNguoiDungAll());
        }
        public string GetTrangThaiDuAns()
        {
            return JsonConvert.SerializeObject(_duAnM.GetTrangThaiDuAns());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetNguoiDungAll2()
        {
            return JsonConvert.SerializeObject(_nguoiDungM.GetAll());
        }
        public string GetDetailShipById(int shipId)
        {
            return JsonConvert.SerializeObject(_congViecM.GetDetailShip(shipId));
        }

        [Authorize]
        public string GetTrangThaiShipablesBy(int ttId)
        {


            List<TrangThaiCongViecViewModel> lstToReturn = new List<TrangThaiCongViecViewModel>();
            if (ttId == (int)EnumTrangThaiCongViecType.shipableNew)
            {
                var trangThaiVm = _congViecM.GetTTCVById(ttId);
                if (trangThaiVm != null) lstToReturn.Add(trangThaiVm);
            }
            var lst = _congViecM.GetTrangThaiShipables((int)EnumTrangThaiCongViecType.shipableNew);
            if (lst != null && lst.Count > 0) lstToReturn.AddRange(lst);
            return JsonConvert.SerializeObject(lstToReturn);

        }

        CodaColsTableProjectViewModel colAll;
        public GiaiDoanDuAnCodaViewModel GetShipablesFromDuAn(int duAnId, string linkCoda)
        {

            docId = linkCoda;
            var doc = "";
            if (!string.IsNullOrEmpty(docId))
            {
                doc = docId.Substring(docId.IndexOf("_d") + 2);
                if (doc.IndexOf("/") != -1)
                {
                    doc = doc.Substring(0, doc.IndexOf("/"));
                }
            }
            codaApi = System.Configuration.ConfigurationManager.AppSettings.Get("LinkCodaApi") + doc;
            var client = new RestClient(codaApi + "/tables");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var obj = JsonConvert.DeserializeObject<CodaTablesViewModel>(response.Content);
                if (obj != null && obj.items != null && obj.items.Count > 0)
                {
                    tableName = "Project " + DateTime.Now.Year;
                    foreach (var item in obj.items)
                    {
                        if (item.name.ToUpper() == tableName.ToUpper())
                        {
                            tableName = item.name;
                            GetColsByTableName(item.name);
                            return GetRowByTableNameToShipable(item.name, duAnId);



                        }
                    }
                }
            }
            return null;
        }

        public string GetColsByTableName(string tableName)
        {
            ResultViewModel result = new ResultViewModel();
            var client = new RestClient(codaApi + "/tables/" + tableName + "/columns");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                colAll = JsonConvert.DeserializeObject<CodaColsTableProjectViewModel>(response.Content);
            }
            else
            {
                result.Message = "Không kết nối được đến status Project.";
            }
            return "";
        }
        public GiaiDoanDuAnCodaViewModel GetRowByTableNameToShipable(string tableName, int duAnId)
        {
            DuAnModel duanM = new DuAnModel();
            var duAn = duanM.GetShortInfoById(duAnId);
            var link = codaApi + "/tables/" + tableName + "/rows/";
            var client = new RestClient(link);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var rows = JsonConvert.DeserializeObject<CodaRowsTableProjectViewModel>(response.Content);
                if (rows != null && rows.items != null && rows.items.Count > 0)
                {
                    CodaRowTableProjectViewModel row;
                    try
                    {
                        row = rows.items.Where(t => t.values.ToString().IndexOf(duAn.MaDuAn) != -1).FirstOrDefault();
                        //row = rows.items.Where(t => t.values.ToString().IndexOf(duAn.MaDuAn) != -1).FirstOrDefault();
                    }
                    catch
                    {
                        row = new CodaRowTableProjectViewModel();
                    }

                    if (row != null && !string.IsNullOrEmpty(row.values.ToString()))
                    {
                        var objs = row.values;
                        var a = objs.ToString();
                        try
                        {
                            var result2 = a.Trim('{', '}')
              .Replace("\",", "#*&#@")
              .Replace(",", "")
                   .Replace("#*&#@", "\",");
                            var b = result2.Split(',');
                            List<dictionViewModel> result1 = new List<dictionViewModel>();
                            if (b != null & b.Length > 0)
                            {
                                foreach (var item in b)
                                {
                                    var dic = new dictionViewModel();
                                    dic.Key = item.Substring(0, item.IndexOf(":")).Replace("\"", "").Trim();
                                    var value = item.Substring(item.IndexOf(":") + 1).Trim();
                                    dic.Value = value.Substring(1, value.Length - 2).Trim();
                                    result1.Add(dic);
                                }
                            }
                            GiaiDoanDuAnCodaViewModel vm = new GiaiDoanDuAnCodaViewModel() { id = row.id, name = row.name };

                            foreach (var item in result1)
                            {

                                var str = item.Value;

                                try
                                {
                                    var col = colAll.items.Where(t => t.id == item.Key).FirstOrDefault();
                                    switch (col.name)
                                    {
                                        case "Dự án":
                                            vm.ProjectName = str;
                                            break;
                                        case "Status":
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                vm.Status = str.Substring(str.IndexOf(".") + 1).Trim();
                                            }

                                            break;
                                        case "Shipable":
                                            vm.Document = str;
                                            cellShip = item.Key;
                                            break;
                                        case "Tháng":
                                            vm.Note = str;
                                            break;
                                        case "ID":
                                            vm.Ma = str;
                                            break;


                                    }
                                }
                                catch
                                {

                                }
                            }
                            return vm;
                        }
                        catch
                        {

                        }

                    }


                }

            }
            return null;
        }
        
        public ActionResult PartialViewMeetingCoda(string linkCoda)
        {
            List<MeetingCodaViewModel> lstToreturn = new List<MeetingCodaViewModel>();
            docId = linkCoda;
            var doc = "";
            if (!string.IsNullOrEmpty(docId))
            {
                doc = docId.Substring(docId.IndexOf("_d") + 2);
                if (doc.IndexOf("/") != -1)
                {
                    doc = doc.Substring(0, doc.IndexOf("/"));
                }
            }
            codaApi = System.Configuration.ConfigurationManager.AppSettings.Get("LinkCodaApi") + doc;
            //var client = new RestClient(codaApi + "/tables");
            //var client = new RestClient(codaApi );
            var client = new RestClient(codaApi + "/tables");

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var obj = JsonConvert.DeserializeObject<CodaTablesViewModel>(response.Content);
                if (obj != null && obj.items != null && obj.items.Count > 0)
                {
                    tableName = "Meeting";
                    foreach (var item in obj.items)
                    {
                        if (item.name.ToUpper() == tableName.ToUpper())
                        {
                            tableName = item.name;
                            GetColsByTableName(item.name);
                            lstToreturn= GetRowByTableNameToMeetingCoda(item.name);
                        }
                    }
                }
            }
            return View(lstToreturn);
        }

        public List<MeetingCodaViewModel> GetRowByTableNameToMeetingCoda(string tableName)
        {
            List<MeetingCodaViewModel> lstToReturn = new List<MeetingCodaViewModel>();  
            var link = codaApi + "/tables/" + tableName + "/rows/";
            var client = new RestClient(link);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var rows = JsonConvert.DeserializeObject<CodaRowsTableProjectViewModel>(response.Content);
                if (rows != null && rows.items != null && rows.items.Count > 0)
                {
                    foreach (var row in rows.items)
                    {
                        if (row != null && !string.IsNullOrEmpty(row.values.ToString()))
                        {
                            var objs = row.values;
                            var a = objs.ToString();
                            try
                            {
                                var result2 = a.Trim('{', '}')
                  .Replace("\",", "#*&#@")
                  .Replace(",", "")
                       .Replace("#*&#@", "\",");
                                var b = result2.Split(',');
                                List<dictionViewModel> result1 = new List<dictionViewModel>();
                                if (b != null & b.Length > 0)
                                {
                                    foreach (var item in b)
                                    {
                                        var dic = new dictionViewModel();
                                        dic.Key = item.Substring(0, item.IndexOf(":")).Replace("\"", "").Trim();
                                        var value = item.Substring(item.IndexOf(":") + 1).Trim();
                                        dic.Value = value.Substring(1, value.Length - 2).Trim();
                                        result1.Add(dic);
                                    }
                                }
                                MeetingCodaViewModel vm = new MeetingCodaViewModel() { id = row.id, name = row.name };

                                foreach (var item in result1)
                                {

                                    var str = item.Value;

                                    try
                                    {
                                        var col = colAll.items.Where(t => t.id == item.Key).FirstOrDefault();
                                        switch (col.name)
                                        {
                                            case "ID":
                                                vm.MaDuAn = str;
                                                break;
                                            case "Tên":
                                                vm.TenMeeting = str;
                                                break;
                                            case "Loại":
                                                vm.LoaiMeeting = str;
                                                break;
                                            case "Người tham gia":
                                                if (!string.IsNullOrEmpty(str))
                                                {
                                                    List<string> lstString = new List<string>();
                                                    while (str.IndexOf("\\n") != -1)
                                                    {
                                                        var hoTen = str.Substring(0, str.IndexOf("\\n"));
                                                        if (!string.IsNullOrEmpty(hoTen))
                                                            lstString.Add(hoTen);
                                                        str = str.Substring(str.IndexOf("\\n") + 2);
                                                    }
                                                    if (!string.IsNullOrEmpty(str)) lstString.Add(str);
                                                    vm.HoTenNguoiThamGias = lstString;
                                                }

                                                break;
                                            case "Start":
                                                vm.StrStartTime = str;                                                
                                                break;
                                            case "Stop":
                                                vm.StrEndTime = str;
                                                break;
                                            case "Project 2021":
                                                vm.TenDuAn = str;
                                                break;
                                            case "Nội dung":
                                                if (!string.IsNullOrEmpty(str)) str = str.Replace("\\n", "<br/>");
                                                vm.NoiDung = str;
                                                break;
                                            case "Shipable":
                                                if (!string.IsNullOrEmpty(str))
                                                {
                                                    List<string> lstString = new List<string>();
                                                    while (str.IndexOf("\\n") != -1)
                                                    {
                                                        var tenCongViec = str.Substring(0, str.IndexOf("\\n"));
                                                        if (!string.IsNullOrEmpty(tenCongViec))
                                                            lstString.Add(tenCongViec);
                                                        str = str.Substring(str.IndexOf("\\n") + 2);
                                                    }
                                                    if (!string.IsNullOrEmpty(str))
                                                    {
                                                        lstString.Add(str);
                                                    }
                                                    vm.TenShipAbles = lstString;
                                                }
                                                break;
                                            case "Quyết định":
                                                if (!string.IsNullOrEmpty(str)) str = str.Replace("\\n", "<br/>");
                                                vm.QuyetDinh = str;
                                                break;
                                            case "InsertOrUpdate":
                                                if (string.IsNullOrEmpty(str)) vm.InsertOrUpdate = true;
                                                else vm.InsertOrUpdate = false;
                                                break;

                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                                
                                if (!string.IsNullOrEmpty(vm.MaDuAn)&&!string.IsNullOrEmpty(vm.StrStartTime)&&!string.IsNullOrEmpty(vm.StrEndTime)&&vm.HoTenNguoiThamGias!=null&&vm.HoTenNguoiThamGias.Count>0)
                                    {
                                    lstToReturn.Add(vm);
                                }
                                        }

                            catch
                            {

                            }

                        }

                    }
                }

            }
            return lstToReturn;
        }
        [HttpPost]
        public string AddMeetingToDB(MeetingCodaViewModel vm)
        {
            ResultViewModel result = new ResultViewModel() { Status = true };
            if (vm.MeetingCodas != null && vm.MeetingCodas.Count > 0)
            {
                DuAnProvider duAnP = new DuAnProvider();
                NguoiDungProvider nguoiDungP = new NguoiDungProvider();
                ThoiGianLamViecProvider thoiGianP = new ThoiGianLamViecProvider();
                CongViecProvider cvP = new CongViecProvider();
                foreach (var item in vm.MeetingCodas)
                {
                    var maDuAn = item.MaDuAn.Trim();
                    var strTime = item.StrStartTime.Trim();
                    var strNguoiDung = item.StrNguoiThamGia.Trim();
                    var strShip = item.StrShipAble.Trim();
                    var duAn = duAnP.GetByMa(maDuAn);
                    if (duAn != null)
                    {
                        List<string> hoTens = new List<string>();
                        if (!string.IsNullOrEmpty(strNguoiDung))
                        {                            
                            while (strNguoiDung.IndexOf("\n") != -1)
                            {
                                var tenCongViec = strNguoiDung.Substring(0, strNguoiDung.IndexOf("\n")).Trim();
                                if (!string.IsNullOrEmpty(tenCongViec))
                                    hoTens.Add(tenCongViec);
                                strNguoiDung = strNguoiDung.Substring(strNguoiDung.IndexOf("\n") + 2);
                            }
                            if (!string.IsNullOrEmpty(strNguoiDung))
                            {
                                hoTens.Add(strNguoiDung.Trim());
                            }
                            
                        }
                        if (hoTens.Count > 0)
                        {
                            var shipId  = cvP.GetShipAbleByTen("Meeting", duAn.DuAnId);
                            
                            if(shipId==0)                            
                                shipId = cvP.Insert(new CongViecViewModel() { LaShipAble = true, LaTask = false, DuAnId = duAn.DuAnId, TenCongViec = "Meeting", TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDoing, Nam = DateTime.Now.Year, Tuan = Helpers.GetNumerWeek(DateTime.Now), IsToDoAdd = false });
                           
                            var taskId = cvP.GetTaskByTen(vm.TenMeeting, shipId, duAn.DuAnId);
                            if (taskId == 0) taskId = cvP.Insert(new CongViecViewModel() { LaShipAble = false, LaTask = true, DuAnId = duAn.DuAnId, TenCongViec = vm.TenMeeting, TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDoing, Nam = DateTime.Now.Year, Tuan = Helpers.GetNumerWeek(DateTime.Now), IsToDoAdd = false, KhoaChaId = shipId });
                            var strStart = strTime.Substring(3, strTime.IndexOf("đến")-3);
                            var strEnd = strTime.Substring(strTime.IndexOf("đến")+3);
                            if (!string.IsNullOrEmpty(strStart) && !string.IsNullOrEmpty(strEnd))
                            {
                              
                                try
                                {
                                    DateTime startTime= DateTime.ParseExact(strStart.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
                                    DateTime endTime = DateTime.ParseExact(strEnd.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
                                    var nam = startTime.Year;
                                    var week = Helpers.GetNumerWeek(startTime);
                                    foreach (var hoTen in hoTens)
                                    {
                                        var nguoiDung = nguoiDungP.GetByHoTen(hoTen);
                                        if (nguoiDung != null)
                                        {
                                            var newId = cvP.InsertEntry(duAn.DuAnId, taskId, nguoiDung.NguoiDungId, nguoiDung.NguoiDungId,vm.TenMeeting,null, week, nam);
                                            if (newId > 0)
                                            {
                                                thoiGianP.OverWriteTime(nguoiDung.NguoiDungId, startTime, endTime, null, 1, newId, null);
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    return "error_ Định dạng time không đúng kiểu dd/MM/yyyy HH:mm";
                                }
                                
                            }
                        }
                        List<CongViecViewModel> ships = new List<CongViecViewModel>();
                        if (!string.IsNullOrEmpty(strShip))
                        {
                            while (strShip.IndexOf("\n") != -1)
                            {
                                var tenCongViec = strShip.Substring(0, strShip.IndexOf("\n"));
                                if (!string.IsNullOrEmpty(tenCongViec))
                                {
                                    var col = tenCongViec.Split('_');
                                    if (col.Length > 1)
                                    {
                                        CongViecViewModel cv = new CongViecViewModel();
                                        cv.TenCongViec = col[1];
                                        cv.HoTen = col[0];
                                        if (!string.IsNullOrEmpty(cv.TenCongViec))
                                            ships.Add(cv);
                                    }
                                }
                                strShip = strShip.Substring(strShip.IndexOf("\n") + 2);
                            }
                            if (!string.IsNullOrEmpty(strShip))
                            {
                                var col = strShip.Split('_');
                                if (col.Length > 1)
                                {
                                    CongViecViewModel cv = new CongViecViewModel();
                                    cv.TenCongViec = col[1];
                                    cv.HoTen = col[0];
                                    if (!string.IsNullOrEmpty(cv.TenCongViec))
                                        ships.Add(cv);
                                }
                            }
                        }
                        if (ships.Count > 0)
                        {
                            foreach(var ship in ships)
                            {
                                if (!string.IsNullOrEmpty(ship.TenCongViec))
                                {
                                    var nguoiDungN = _nguoiDungM.GetNguoiDungByUserName(ship.HoTen.Trim());
                                    if (nguoiDungN != null && nguoiDungN.NguoiDungId != Guid.Empty)
                                    {
                                        var shipId = cvP.GetShipAbleByTen(ship.TenCongViec, duAn.DuAnId);
                                        if (shipId == 0)
                                        {
                                            cvP.Insert(new CongViecViewModel() { LaShipAble = true, LaTask = false, DuAnId = duAn.DuAnId, TenCongViec = ship.TenCongViec, TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDoing, Nam = DateTime.Now.Year, Tuan = Helpers.GetNumerWeek(DateTime.Now), IsToDoAdd = false, NguoiXuLyId = nguoiDungN.NguoiDungId,NguoiTaoId= nguoiDungN.NguoiDungId });
                                        }
                                        else
                                        {
                                            cvP.UpdateNguoiXuLyId(shipId, nguoiDungN.NguoiDungId);
                                        }
                                    }
                                   
                                }
                            }
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string StartTime()
        {
            ThoiGianLamViecModel tt = new ThoiGianLamViecModel();
            var channel =Request["channel_name"].ToString();
            var userIdSlack = Request["user_id"].ToString();
            var str = "";
            var check = true;
            var tenTask = Request["text"].ToString();           
            if (!CheckIsExistDuAn(channel.Trim()))
            {
                check = false;
                str ="Tên dự án :"+channel +" không tồn tại.(Tên dự án là tên channel trong slack)";
            }
            if (!CheckIsExistUserIdSlack(userIdSlack.Trim()))
            {
                check = false;
                str = "Bạn chưa liên kết tài khoản slack vs WorkTime. Bạn hãy gõ cú pháp: configuser username để liên kết tài khoản( username là tên đăng nhập của bạn trong worktime)";
            }
            if (check)
            {
                NguoiDungProvider ndP = new NguoiDungProvider();
                var nd = ndP.GetByUserIdSlack(userIdSlack);
                DuAnProvider model = new DuAnProvider();
                var duAn = model.GetDuAnByName2(channel.Trim());
                var task = _congViecM.GetTaskToStartTime(tenTask, duAn.DuAnId, duAn.KhoaChaId,nd.NguoiDungId);
                TimeController ct = new TimeController();
                var result = ct.StartTimer4(task.CongViecId,nd.NguoiDungId);
                if (!result.Status)
                {
                    str = result.Message;
                    tt.SendMessageToSlackWithChannelId(str, userIdSlack, true);
                }
            }
            else
            {
                tt.SendMessageToSlackWithChannelId(str, userIdSlack, true);
            }
            return "";
            
        }
        [HttpPost]
        public string StopTime()
        {
            ThoiGianLamViecModel tt = new ThoiGianLamViecModel();           
            var userIdSlack = Request["user_id"].ToString();
            var str = "";
            var check = true; 
            if (!CheckIsExistUserIdSlack(userIdSlack.Trim()))
            {
                check = false;
                str = "Bạn chưa liên kết tài khoản slack vs WorkTime. Bạn hãy gõ cú pháp: /configuser username để liên kết tài khoản( username là tên đăng nhập của bạn trong worktime)";
            }
            if (check)
            {
                NguoiDungProvider ndP = new NguoiDungProvider();
                var nd = ndP.GetByUserIdSlack(userIdSlack);
                ThoiGianLamViecModel ttM = new ThoiGianLamViecModel();
                var result = ttM.GetCurrentTodoCountTime(nd.NguoiDungId);
                if (result != null && result.CongViecId > 0)
                {
                    var s = ttM.StopTodo(result.CongViecId, nd.NguoiDungId);
                    if (s.Status)
                    {
                        str = "Stop công việc thành công.";
                    }
                    else str = "Stop công việc không thành công.";
                }
                else str = "Bạn hiện không chạy giờ công việc nào.";
            }
           
                tt.SendMessageToSlackWithChannelId(str, userIdSlack, true);
            
            return "";

        }
        public bool CheckIsExistDuAn(string tenVietTat)
        {
            DuAnProvider model = new DuAnProvider();
            var duAn = model.GetDuAnByName2(tenVietTat.Trim());
            if (duAn!=null && duAn.DuAnId > 0) return true;
            return false;
        }
        public bool CheckIsExistUserIdSlack(string userIdSlack)
        {
            NguoiDungProvider model = new NguoiDungProvider();
            var nd = model.GetByUserIdSlack(userIdSlack);
            if (nd!=null&&nd.NguoiDungId!=Guid.Empty) return true;
            return false;
        }


        public string Test()
        {
           
            _congViecM.AuToSendMailDoneTask();
            _congViecM.AuToSendMailShipInActive();
            ////return "";
            //DateTime startDate = new DateTime(2021, 6, 7, 0, 0, 0);
            //DateTime endDate = new DateTime(2021, 6, 12, 0, 0, 0);
            //CongViecModel model = new CongViecModel();
            //var a= model.GetToDosInTimeBy(startDate, endDate, "cuongvh", EndCode.Encrypt("123456"));
            //


            return "xong";
        }

       

        [HttpPost]
        public string ConfigUserName()
        {
           var userName = Request["text"].ToString();
            NguoiDungProvider ndP = new NguoiDungProvider();
            var nguoiDung = ndP.GetUserByUsername(userName);
            var str = "";
            string userId = Request["user_id"].ToString();
            if (nguoiDung != null && nguoiDung.NguoiDungId != Guid.Empty)
            {
                if (CheckIsExistUserIdSlack(userId))
                {
                    ndP.Update(nguoiDung.NguoiDungId, "");
                }
                ndP.Update(nguoiDung.NguoiDungId, userId);
                str = "Bạn đã config thành công liên kết slack vs workTime.";
            }
            else
            {
                str = "UserName đăng nhập trong workTime bạn nhập là: " + userName + " không tồn tại.";
            }
            ThoiGianLamViecModel tt = new ThoiGianLamViecModel();
            tt.SendMessageToSlackWithChannelId(str, userId, true);
            return "";
        }

        [HttpPost]
        public bool UpdateTrangThaiTask(int CongViecId, int TrangThaiId)
        {
            var userId = GetUserID();
            var result = _congViecM.UpdateStatus(CongViecId, TrangThaiId);
            if (result&&TrangThaiId==(int)EnumTrangThaiCongViecType.congViecDone)
            {
                var cf = new CauHinhProvider();
                var ap = cf.GetValueByTen("METAWORK.SENDDONE");
                var nguoiDung = _nguoiDungM.GetById(userId);
                var cv = _congViecM.GetTaskInfo(CongViecId);
                ap += DateTime.Now.ToString("dd/MM/yyyy HH:mm")   + ": " + nguoiDung.HoTen + " done công việc: " + cv.TenCongViec + " \n";
                cf.InsertOrUpdte("METAWORK.SENDDONE", ap);

                //var cv = _congViecM.GetTaskInfo(CongViecId);
                //var nguoiDung = _nguoiDungM.GetById(userId);
                //ThoiGianLamViecModel ttM = new ThoiGianLamViecModel();
                //var channelId = ttM.GetChannelIdByChannelName("general");

                //ttM.SendMessageToSlackWithChannelId(nguoiDung.HoTen + " done công việc: " + cv.TenCongViec, channelId, false);
            }
            return result;           
        }
      





        [HttpPost]
        public void GetListTaskWorkingInWeek()
        {
            ThoiGianLamViecModel tt = new ThoiGianLamViecModel();
            var userIdSlack = Request["user_id"].ToString();
            var str = "";
            var check = true;
            if (!CheckIsExistUserIdSlack(userIdSlack.Trim()))
            {
                check = false;
                str = "Bạn chưa liên kết tài khoản slack vs WorkTime. Bạn hãy gõ cú pháp: configuser username để liên kết tài khoản( username là tên đăng nhập của bạn trong worktime)";
            }
            if (check)
            {
                tt.SendMessageToSlackWithChannelId("<a class=\"btn btn-default\"> hehehe</a>", userIdSlack, true);
            }
            else
            {
                tt.SendMessageToSlackWithChannelId(str, userIdSlack, true);
            }
        }

        public string UpdateTrangThaiTask(int congViecId,Guid nguoiDungId)
        {
            var cv = _congViecM.GetById(congViecId);
            var trangThaiId = cv.TrangThaiCongViecId;
            var result = _congViecM.UpdateTrangThai( congViecId, (int)EnumTrangThaiCongViecType.congViecDone);
            if (result && trangThaiId != (int)EnumTrangThaiCongViecType.congViecDone)
            {
                var cf = new CauHinhProvider();
                var ap = cf.GetValueByTen("METAWORK.SENDDONE");
                var nguoiDung = _nguoiDungM.GetById(nguoiDungId);              
                ap += DateTime.Now.ToString("dd/MM/yyyy HH:mm")+ " done công việc: " + cv.TenCongViec + " \n";
                cf.InsertOrUpdte("METAWORK.SENDDONE", ap);
                //var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
                //ThoiGianLamViecModel ttM = new ThoiGianLamViecModel();
                //var channelId = ttM.GetChannelIdByChannelName("general");
                //ttM.SendMessageToSlackWithChannelId(nguoiDung.HoTen + " done công việc: " + cv.TenCongViec, channelId, false);
            }
            return "Done công việc thành công!";

        }
        [HttpPost]
        public void GetTasksToSlack()
        {
            ThoiGianLamViecModel tt = new ThoiGianLamViecModel();
            var userIdSlack = Request["user_id"].ToString();
            //var userIdSlack = "U9ZMSV095";
            var str = "";
            var check = true;
            if (!CheckIsExistUserIdSlack(userIdSlack.Trim()))
            {
                check = false;
                str = "Bạn chưa liên kết tài khoản slack vs WorkTime. Bạn hãy gõ cú pháp: configuser username để liên kết tài khoản( username là tên đăng nhập của bạn trong worktime)";
            }
            if (check)
            {
                NguoiDungProvider ndP = new NguoiDungProvider();
                var nd = ndP.GetByUserIdSlack(userIdSlack);
                var now = DateTime.Now;
                var endDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 0);
                var startDate = endDate.AddDays(-7).AddMinutes(1);
                var lstCv = _congViecM.GetCongViecWorkingInTime(nd.NguoiDungId, startDate, endDate);
                if (lstCv != null && lstCv.Count > 0)
                {
                    lstCv = lstCv.OrderByDescending(t => t.DuAnId).ThenByDescending(t => t.KhoaChaId).ToList();
                    str = " Danh sách công việc bạn đã chạy giờ trong 7 ngày gần nhất mà chưa done: \n";
                    var block = "[        {                        \"type\": \"section\",			\"text\": {                            \"type\": \"plain_text\",				\"text\": \"Danh sách công việc bạn đã chạy giờ trong 7 ngày gần nhất mà chưa done: \n\",				\"emoji\": true            }                    }";
                    foreach (var item in lstCv)
                    {
                        block += ",{			\"type\": \"section\",			\"text\": {				\"type\": \"mrkdwn\",				\"text\": \">" + item.TenDuAn + ">" + item.TenKhoaCha + ">" + item.TenCongViec + ".\"			},			\"accessory\": {				\"type\": \"button\",				\"text\": {					\"type\": \"plain_text\",					\"text\": \"Done\",					\"emoji\": true				},				\"value\": \"Done\",				\"url\": \"http://beta.tecotec.vn/Todo/UpdateTrangThaiTask?congViecId=" + item.CongViecId + "&&nguoiDungId=" + nd.NguoiDungId.ToString() + "\",				\"action_id\": \"button-action\"			}		}";
                    }
                    block += "]";
                    tt.SendMessageToSlackWithChannelIdBlock(str, block, userIdSlack, true);
                }
                else
                {
                    tt.SendMessageToSlackWithChannelId("Bạn không có task nào đã chạy giờ trong 7 ngày gần nhất mà chưa done", userIdSlack, true);
                }

            }
            else
            {
                tt.SendMessageToSlackWithChannelId(str, userIdSlack, true);
            }
        }


        public void GetDanhSachChoDuyetG3()
        {
            if (_staffDay == null)
            {
                var a = StaffDay;
            }
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            var lstUser = _nguoiDungM.GetNguoiDungsByQuyen(3);
            if (lstUser != null && lstUser.Count > 0)
            {
                var lstEmail = lstUser.Select(t => t.Email).ToList();
                var now = DateTime.Now;
                var monthday = Helpers.GetMonDayBy(now);
                var startday = monthday.AddDays(-2);
                var endDay = monthday.AddDays(5);
                var week = Helpers.GetNumberCurrentWeekOfMonth(now);
                var title = "Danh sách chờ duyệt tuần " + week + " tháng " + monthday + " của G3.";
                var content= "<h1><span style=\"font-family:&quot; Helvetica Neue&quot;,serif; color:#34495e;letter-spacing:-.75pt;font-weight:normal\">DANH SÁCH NGÀY CHỜ DUYỆT<u></u><u></u></span></h1>";
                content += "<h3><span style=\"font - family:&quot; Helvetica Neue&quot;,serif\">(Từ ngày " + startday.ToString("dd/MM/yyyy") + " đến ngày " + endDay.ToString("dd/MM/yyyy") + " <u></u><u></u></span></h3>";
                content += "<table border=\"1\" cellspacing=\"0\" cellpadding=\"0\" width=\"100 % \" style=\"width:100.0%;border-collapse:collapse;border:none; border-radius:5px\"><thead><tr><td width=\"12 % \" style=\"width: 12.0 %; border:solid #dddddd 1.0pt;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Ngày<u></u><u></u></span></p></td><td width=\"15%\" style=\"width:15.0%;border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Người<u></u><u></u></span></p></td><td width=\"12%\" style=\"width:12.0%;border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Bắt đầu / Kết thúc<u></u><u></u></span></p></td><td width=\"10%\" style=\"width:10.0%;border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Loại ngày<u></u><u></u></span></p></td><td style=\"border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Mô tả<u></u><u></u></span></p></td><td style=\"border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"><p class=\"MsoNormal\"><span style=\"color:#9e9e9e\">Lý do<u></u><u></u></span></p></td><td width=\"10%\" style=\"width:10.0%;border:solid #dddddd 1.0pt;border-left:none;padding:6.0pt 6.0pt 6.0pt 6.0pt\"></td></tr></thead><tbody>";                
                var ds = model.GetDanhSachChoDuyetBy(lstEmail);
                ds = ds.Where(t => t.NgayDangKy >= startday && t.NgayDangKy <= endDay).OrderBy(t => t.NgayDangKy).ThenBy(t => t.HoTen).ToList();
                foreach(var item in ds)
                {
                    var daytoInt = (int)(item.NgayDangKy - new DateTime(1970, 1, 1)).TotalSeconds;
                    
                    if (string.IsNullOrEmpty(item.TokenId))
                    {
                        
                        var token = GetTokenKey2(item.NgayDangKy);
                        if (!string.IsNullOrEmpty(token.key_token)) item.TokenId = token.key_token;
                        else
                        {
                            if(RegisterToken(item.Email, daytoInt)){
                                 token = GetTokenKey2(item.NgayDangKy);
                                if (!string.IsNullOrEmpty(token.key_token)) item.TokenId = token.key_token;
                            }
                        }
                    }
                    content += "<tr><td style=\"border: solid #dddddd 1.0pt;border-top:none;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\">" + item.NgayDangKy.ToString("dd/MM/yyyy") + "<u></u><u></u></p></td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\"><span style=\"color:#5d9cec\">" + item.HoTen + "</span> <u></u><u></u></p></td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\">" + item.ThoiGianBatDau.ToString("HH:mm") + " - " + item.ThoiGianKetThuc.ToString("HH:mm") + " (" + item.TongThoiGian + ")<u></u><u></u></p></td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\">" + item.DayType + "<u></u><u></u></p></td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\"><span style=\"color:#5d9cec\">" + item.TenDuAn + " &gt;</span> <span style=\"color:#81c868\">" + item.TenShipAble + " &gt;</span> " + item.TenTask.Replace(">", "&gt;") + " <br><span style=\"color:#9e9e9e\">" + item.TenToDo.Replace(">", "&gt;") + "</span><u></u><u></u></p></td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\">" + item.LyDo + "</td><td style=\"border-top:none;border-left:none;border-bottom:solid #dddddd 1.0pt;border-right:solid #dddddd 1.0pt;padding:3.75pt 3.75pt 3.75pt 3.75pt\"><p class=\"MsoNormal\" align=\"center\" style=\"text-align:center\"><a href=\"" + HttpUtility.UrlEncode(item.LinkPheDuyet + "&&tokenId=" + item.TokenId);
                    if (item.NgayDangKy != null)
                    {
                    
                    }


                    content+= " \" target=\"_blank\"><span style=\"color:#81c868\">Phê Duyệt </span></a><a href=\""+item.LinkHuyDuyet+" target =\"_blank\"><span style=\"color:red\">Không </span></a><u></u><u></u></p></td></tr>";
                }
                content += "</tbody></table>";
                MailMessage email2Sent = new MailMessage("sourcing@tecotec.com.vn", "cuongvh.elands@gmail.com"/*leads[0].Email*/ , title, content);
                email2Sent.IsBodyHtml = true;
                //if (leads.Count > 1)
                //{
                //    for(int i = 1; i < leads.Count; i++)
                //    {
                //        email2Sent.CC.Add(new MailAddress(leads[i].Email));
                //    }
                //}
                //    email2Sent.CC.Add(new MailAddress("phuongnt@tecotec.com.vn"));
                //email2Sent.CC.Add(new MailAddress("vietanh@tecotec.com.vn"));
                SentMailSMTP sendEmail = new SentMailSMTP();
                sendEmail.SendMail(email2Sent);
            }
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
    }
}