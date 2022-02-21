using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MetaWork.WorkTime.Models;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.WorkTime.Controllers
{
    public class AddController : Controller
    {
        // GET: Add
        NguoiDungProvider _nguoiDungP = new NguoiDungProvider();
        public ActionResult Index1(int? phongBanId, byte? type, string strNguoiDungId, string strStartDate, string strEndDate)
        {

            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            NguoiDungModel nguoiDungM = new NguoiDungModel();
            var userId = GetUserID();
            var nguoiDung = nguoiDungM.GetById(userId);
            if (phongBanId == null && string.IsNullOrEmpty(strNguoiDungId))
                strNguoiDungId = userId.ToString();
            QuanLyCongViecViewModel vm = new QuanLyCongViecViewModel() { Type = type ?? 1, PhongBanId = phongBanId ?? 0 };
            // datetime
            DateTime startDate;
            DateTime endDate;
            if (vm.Type == 1)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                endDate = startDate.AddDays(7).AddTicks(-1);
                vm.TextHeader = "Tuần " + Helpers.GetNumerWeek(startDate);
            }
            else if (vm.Type == 2)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now).AddDays(-7);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                vm.TextHeader = "Tuần " + Helpers.GetNumerWeek(startDate);
                endDate = startDate.AddDays(7).AddTicks(-1);
            }
            else if (vm.Type == 11)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                vm.TextHeader = "Tháng " + startDate.Month;
            }
            else if (vm.Type == 12)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(-1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                vm.TextHeader = "Tháng " + startDate.Month;
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
                    endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                }
                vm.StrStartDate = startDate.ToString("dd/MM/yyyy");
                vm.StrEndDate = endDate.ToString("dd/MM/yyyy");
                vm.TextHeader = "Thời gian ";
            }
            // nguoiDung
            List<Guid> nguoiDungIds = new List<Guid>();
            if (!string.IsNullOrEmpty(strNguoiDungId))
            {
                var col = strNguoiDungId.Split(',');
                foreach (var item in col)
                {
                    nguoiDungIds.Add(Guid.Parse(item));
                }
            }
            // Danh sách ngày 
            vm.NgayLamCongViecs = model.GetToDosByV2(phongBanId, nguoiDungIds, startDate, endDate);
            // Khởi tạo
            PhongBanModel phongBanM = new PhongBanModel();
            vm.PhongBans = phongBanM.GetAll();
            DuAnModel duAnM = new DuAnModel();
            vm.DuAns = duAnM.GetsByNguoiDungId(GetUserID());
            vm.StrNguoiDungId = strNguoiDungId;
            vm.NguoiDungAll = nguoiDungM.GetAll();
            vm.TimeTypes = GetsTimes();
            vm.UserId = nguoiDung.NguoiDungId;
            vm.Quyen = nguoiDung.Quyen ?? 1;

            return View(vm);
        }
        public ActionResult Index(int? phongBanId, byte? type, string strNguoiDungId, string strStartDate, string strEndDate)
        {

            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            NguoiDungModel nguoiDungM = new NguoiDungModel();
            var userId = GetUserID();
            var nguoiDung = nguoiDungM.GetById(userId);
            if (phongBanId == null && string.IsNullOrEmpty(strNguoiDungId))
                strNguoiDungId = userId.ToString();
            QuanLyCongViecViewModel vm = new QuanLyCongViecViewModel() { Type = type ?? 1, PhongBanId = phongBanId ?? 0 };
            // datetime
            DateTime startDate;
            DateTime endDate;
            if (vm.Type == 1)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                endDate = startDate.AddDays(7).AddTicks(-1);
                vm.TextHeader = "Tuần " + Helpers.GetNumerWeek(startDate);
            }
            else if (vm.Type == 2)
            {
                var date = Helpers.GetMonDayBy(DateTime.Now).AddDays(-7);
                startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                vm.TextHeader = "Tuần " + Helpers.GetNumerWeek(startDate);
                endDate = startDate.AddDays(7).AddTicks(-1);
            }
            else if (vm.Type == 11)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                vm.TextHeader = "Tháng " + startDate.Month;
            }
            else if (vm.Type == 12)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(-1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                vm.TextHeader = "Tháng " + startDate.Month;
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
                    endDate = DateTime.ParseExact(strEndDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                }
                vm.StrStartDate = startDate.ToString("dd/MM/yyyy");
                vm.StrEndDate = endDate.ToString("dd/MM/yyyy");
                vm.TextHeader = "Thời gian ";
            }
            // nguoiDung
            List<Guid> nguoiDungIds = new List<Guid>();
            if (!string.IsNullOrEmpty(strNguoiDungId))
            {
                var col = strNguoiDungId.Split(',');
                foreach (var item in col)
                {
                    nguoiDungIds.Add(Guid.Parse(item));
                }
            }
            // Danh sách ngày 
            vm.NgayLamCongViecs = model.GetToDosByV2(phongBanId, nguoiDungIds, startDate, endDate);
            // Khởi tạo
            PhongBanModel phongBanM = new PhongBanModel();
            vm.PhongBans = phongBanM.GetAll();
            DuAnModel duAnM = new DuAnModel();
            vm.DuAns = duAnM.GetsByNguoiDungId(GetUserID());
            vm.StrNguoiDungId = strNguoiDungId;
            vm.NguoiDungAll = nguoiDungM.GetAll();
            vm.TimeTypes = GetsTimes();
            vm.UserId = nguoiDung.NguoiDungId;
            vm.Quyen = nguoiDung.Quyen ?? 1;

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
        public Guid GetUserID()
        {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return _nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }
        public string AddToDo(CongViecViewModel vm)
        {
            vm.NguoiTaoId = GetUserID();
            var nguoiDung = _nguoiDungP.GetById(vm.NguoiTaoId);
            if (nguoiDung.Quyen != 3)
            {
                vm.NguoiXuLyId = vm.NguoiTaoId;
            }
            else vm.NguoiTaoId = vm.NguoiXuLyId.Value;

            CongViecModel model = new CongViecModel();
            vm.NgayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd-MM-yyyy", CultureInfo.CurrentCulture);
            return JsonConvert.SerializeObject(model.InsertToDoV2(vm, GetTokenKey()));


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
        public string CheckUpdateTrangThaiCongViec(int congViecId, int trangThaiCongViec)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var congViec = model.GetById(congViecId);
            var nguoiDung = _nguoiDungP.GetById(GetUserID());
            var nguoitao = _nguoiDungP.GetById(congViec.NguoiTaoId);
            if (congViec.TrangThaiCongViecId == trangThaiCongViec)
            {
                result.Status = true;
            }
            else
            {
                if (nguoiDung.NguoiDungId == congViec.NguoiTaoId)
                {
                    var token = GetTokenKey();
                    if (token != null && !string.IsNullOrEmpty(token.key_token))
                    {
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Bạn chưa lấy token key ngày hôm nay. Bạn muốn tiếp tục chứ?(nếu tiếp tục, thời gian ngày hôm nay của bạn trong todo này sẽ bị xóa)";
                        result.ItemId = "1-" + congViec.TrangThaiCongViecId;
                        return JsonConvert.SerializeObject(result);
                    }

                }
                else
                {
                    if (trangThaiCongViec == (int)EnumTrangThaiCongViecType.todoDo)
                    {
                        result.Message = "Bạn không thể start công việc hộ người khác được.";
                        result.ItemId = "0-" + congViec.TrangThaiCongViecId;
                        return JsonConvert.SerializeObject(result);
                    }
                    else
                    {
                        var token = GetTokenKey(congViec.NguoiTaoId);
                        if (token != null && !string.IsNullOrEmpty(token.key_token))
                        {
                            result.Status = true;
                        }
                        else
                        {
                            if (congViec.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoDo)
                            {
                                result.Message = nguoitao.HoTen + " chưa lấy token key ngày hôm nay. Bạn muốn tiếp tục chứ?(nếu tiếp tục, thời gian ngày hôm nay của " + nguoitao.HoTen + " trong todo này sẽ bị xóa)";
                                result.ItemId = "1-" + congViec.TrangThaiCongViecId;
                                return JsonConvert.SerializeObject(result);
                            }


                        }
                    }



                }
                if (trangThaiCongViec == (int)EnumTrangThaiCongViecType.todoDo)
                {
                    ThoiGianLamViecModel model2 = new ThoiGianLamViecModel();

                    var currentToDo = model2.GetCurrentTodoCountTime(congViec.NguoiTaoId);
                    if (currentToDo != null)
                    {
                        if (nguoiDung.NguoiDungId != congViec.NguoiTaoId)
                            result.Message = nguoitao.HoTen + " đang chạy thời gian trên công việc: " + currentToDo.TenCongViec;
                        else result.Message = "Bạn đang chạy thời gian trên công việc: " + currentToDo.TenCongViec;
                        result.Status = false;
                        result.ItemId = "0-" + congViec.TrangThaiCongViecId;
                    }
                    else
                    {
                        result.Status = true;
                    }


                }
                else if (congViec.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoDo)
                {
                    result.Status = true;

                }

            }
            ThoiGianLamViecModel ttlvM = new ThoiGianLamViecModel();
            return JsonConvert.SerializeObject(result);
        }
        public bool UpdateTrangThaiCongViec(int congViecId, int trangThaiCongViec)
        {
            CongViecModel model = new CongViecModel();
            var congViec = model.GetById(congViecId);
            if (congViec.TrangThaiCongViecId == trangThaiCongViec)
                return true;
            return model.UpdateStatusV2(congViecId, trangThaiCongViec, GetTokenKey().key_token, System.Configuration.ConfigurationManager.AppSettings.Get("ApiGetTokenKeyV2"));
        }
        public ActionResult _PartialViewDetailToDo(int congViecId)
        {
            CongViecViewModel vm = new CongViecViewModel();
            CongViecModel model = new CongViecModel();
            vm = model.GetById(congViecId);
            ThoiGianLamViecProvider ttlv = new ThoiGianLamViecProvider();
            vm.ThoiGianLamViecs = ttlv.GetByCongViecId(congViecId);
            if (vm.ThoiGianLamViecs != null && vm.ThoiGianLamViecs.Count > 0)
            {
                foreach (var item in vm.ThoiGianLamViecs)
                {
                    if (item.TongThoiGian > 0) item.StrTongThoiGian = getStrTime(item.TongThoiGian);
                }
            }
            DuAnModel daM = new DuAnModel();
            var duAnC = daM.GetShortInfoById(vm.DuAnId);
            
            ViewBag.DuAns = daM.GetsByNguoiDungId(vm.NguoiTaoId);
            if (duAnC != null)
                ViewBag.DuAnThanhPhans = daM.GetDuAnThanhPhanByKhoaCha(duAnC.KhoaChaId.Value);
            //ViewBag.Shipables = model.GetTenShipAbleByDuAnId(vm.DuAnId);
            var task = model.GetById(vm.KhoaChaId.Value);
            //ViewBag.Task = model.GetTenCongViecByKhoaChaId(task.KhoaChaId.Value);
            ViewBag.shipId = task.KhoaChaId.Value;
            TrangThaiCongViecProvider ttcvM = new TrangThaiCongViecProvider();
            var lstTrangThai = ttcvM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.todoNew);
            vm.TrangThaiCongViecs = new List<TrangThaiCongViecViewModel>();
            if (vm.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoNew) vm.TrangThaiCongViecs.Add(new TrangThaiCongViecViewModel { TrangThaiCongViecId = vm.TrangThaiCongViecId, TenTrangThai = vm.TenTrangThai });
            vm.TrangThaiCongViecs.AddRange(lstTrangThai);
            return PartialView("_PartialViewDetailToDo", vm);
        }
        private string getStrTime(int timeSpent)
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
                    result += "0" + minutes + "min";
                else result += minutes + "min";
            }
            else
            {
                result = "0h00min";
            }
            return result;
        }
        public string GetTenShipByDuAnId(int duAnId)
        {
            CongViecModel model = new CongViecModel();
            List<CongViecViewModel> vms = new List<CongViecViewModel>();
            vms.Add(new CongViecViewModel { CongViecId = 0, TenCongViec = "Chọn shipable" });
            if (duAnId > 0)
            {
                var result = model.GetTenShipAbleByDuAnId(duAnId);
                if (result != null && result.Count > 0) vms.AddRange(result);
            }
            return JsonConvert.SerializeObject(vms);
        }
        public string GetTenTaskByshiableId(int shipableId)
        {
            CongViecModel model = new CongViecModel();
            List<CongViecViewModel> vms = new List<CongViecViewModel>();
            vms.Add(new CongViecViewModel { CongViecId = 0, TenCongViec = "Chọn task" });
            if (shipableId > 0)
            {
                var result = model.GetTenCongViecByKhoaChaId(shipableId);
                if (result != null && result.Count > 0) vms.AddRange(result);
            }
            return JsonConvert.SerializeObject(vms);

        }
        public string ChuyenToDo2Task(int toDoId, int taskId, int trangThai)
        {
            CongViecModel model = new CongViecModel();
            var result = CheckUpdateTrangThaiCongViec(toDoId, trangThai);
            ResultViewModel vm = JsonConvert.DeserializeObject<ResultViewModel>(result);
            if (vm.Status)
            {
                if (UpdateTrangThaiCongViec(toDoId, trangThai))
                {
                    if (model.UpdateToDo2Task(toDoId, taskId))
                    {
                        vm.Status = true;
                    }
                    else
                    {
                        vm.Status = false;
                        vm.Message = "Cập nhật không thành công";
                    }
                }
                else
                {
                    vm.Status = false;
                    vm.Message = "Update trạng thái không thành công";
                }
            }

            return JsonConvert.SerializeObject(vm);

        }
        public string Approvetime(int toDoId)
        {
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            return JsonConvert.SerializeObject(model.ApproveTime(toDoId));
        }
        public bool DeleteTime(int thoiGianId)
        {
            ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            return model.DeleteThoiGianLamViec(thoiGianId, GetUserID());
        }
        public bool DeleteToDo(int todo)
        {
            CongViecModel model = new CongViecModel();
            return model.DeleteToDoV2(todo, GetUserID());
        }
        public  ActionResult Test()
        {
            return View();
        }
        public async Task<bool> Test2()
        {           
            //var client = new RestClient("https://coda.io/apis/v1/docs/QLEj7bYtQA");
            //var request = new RestRequest();
            //request.Method = Method.GET;
            //request.AddHeader("Authorization","Bearer 7a053e92-81be-4c89-9313-927d02ba7d68");
            //var response = client.Execute(request);

            var client = new RestClient("https://coda.io/apis/v1/docs/QLEj7bYtQA");            
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 7a053e92-81be-4c89-9313-927d02ba7d68");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOnRydWUsInB1Ymxpc2hlckFuYWx5dGljc0FsbG93ZWQiOnRydWV9");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);



            return true;

           
        }
    }

}
