using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MetaWork.Project.Models;
using MetaWork.Data.ViewModel;
using MetaWork.Data.Provider;
using System.Globalization;
using System.Web.Security;
using RestSharp;

namespace MetaWork.Project.Controllers
{
    public class TodoController : Controller
    {
        string docId;
        string doc;
        string codaApi;
        // GET: Todo
        CongViecModel _congViecM = new CongViecModel();
        NguoiDungModel _nguoiDungM = new NguoiDungModel();
        LoaiCongViecProvider _loaiCongViecProvider = new LoaiCongViecProvider();
        LoaiCongViecModel _loaiCongViecModel = new LoaiCongViecModel();
        PhongBanModel _phongBanModel = new PhongBanModel();
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
        public ActionResult ToDoWork(string strDuAnId, string strPhongBanIds, bool? quanTam, int? trangThaiCongViecId, string tenShipAble, byte? type, string strNguoiDungId, string strStartDate, string strEndDate)
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

            ToDoWorkViewModel vm = new ToDoWorkViewModel() { Type = type ?? 11, TrangThaiCongViecId = trangThaiCongViecId ?? 0, DuAnIds = duAnIds ?? null, QuanTam = quanTam ?? false, TenShipable = tenShipAble, PhongBanIds = phongBanIds ?? null, StrNguoiDungId = strNguoiDungId, strDuAnIds = strDuAnId, StrPhongBanIds = strPhongBanIds };
            DateTime startDate;
            DateTime endDate;
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

            vm.DuAns = _congViecM.GetToDoWorksBy(userId, vm.DuAnIds, vm.PhongBanIds, nguoiDungIds, vm.TrangThaiCongViecId, vm.QuanTam, tenShipAble, startDate, endDate);
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
            var userId = GetUserID();
            var shipId = _congViecM.InsertShipableToDoWork(vm, userId);
            if (shipId > 0)
            {
                CauHinhProvider ch = new CauHinhProvider();
                var linkCoda = ch.GetValueByTen("Project/Edit/" + vm.DuAnId);
                if (string.IsNullOrEmpty(linkCoda))
                {
                    linkCoda = System.Configuration.ConfigurationManager.AppSettings.Get("LinkProjectEdit");
                }
                var str = GetShipablesFromDuAn(vm.DuAnId, linkCoda);
            }

            return shipId;

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
        public string GetShipablesFromDuAn(int duAnId, string linkCoda)
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
                    foreach (var item in obj.items)
                    {
                        if (item.name.ToUpper() == "PROJECT " + DateTime.Now.Year)
                        {
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
        public string GetRowByTableNameToShipable(string tableName, int duAnId)
        {
            DuAnModel duanM = new DuAnModel();
            var duAn = duanM.GetShortInfoById(duAnId);
            var client = new RestClient(codaApi + "/tables/" + tableName + "/rows");
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
                                            break;
                                        case "Tháng":
                                            vm.Note = str;
                                            break;
                                        case "Mã":
                                            vm.Ma = str;
                                            break;


                                    }
                                }
                                catch
                                {

                                }
                            }
                            return vm.Document;
                        }
                        catch
                        {

                        }

                    }


                }

            }
            return "";
        }
    }
}