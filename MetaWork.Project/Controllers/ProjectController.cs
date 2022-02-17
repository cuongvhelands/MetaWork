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
using MetaWork.Project.Models;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using RestSharp;
using System.Data;

namespace MetaWork.Project.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        string docId;
        string doc;
        string codaApi; 
        DuAnModel _model = new DuAnModel();        
        CongViecModel _congViecM = new CongViecModel();
        NguoiDungModel _nguoiDungM = new NguoiDungModel();
        NoiDungModel _noiDungM = new NoiDungModel();
        // GET: Project
        public ActionResult Index(int? type,string strNguoiDungId, string strStatus,int? typeGroup)
        {
            CauHinhProvider ch = new CauHinhProvider();
            ViewBag.DocIdDefault = ch.GetValueByTen("Project/Index");

            var userId = GetUserID();
            var user = _nguoiDungM.GetById(userId);
            
            if (user == null) return RedirectToAction("Login", "User");
            else
            {
                if (user.Quyen != 3) return RedirectToAction("Index", "Home");
            }
            DuAnIndexViewModel vm = new DuAnIndexViewModel() { Type=type??0,LoaiDuAns=_model.GetLoaiDuAns(),TypeGroup=typeGroup??1};
            List<int> trangThaiDuAnIds = new List<int>();
            List<Guid> nguoiDungIds = new List<Guid>();
            if (type==null && string.IsNullOrEmpty(strNguoiDungId) && string.IsNullOrEmpty(strStatus) && typeGroup == null)
            {               
                trangThaiDuAnIds = new List<int>() { (int)EnumTrangThaiDuAnType.Active, (int)EnumTrangThaiDuAnType.Pending };
                vm.StrStatus = (int)EnumTrangThaiDuAnType.Active+","+ (int)EnumTrangThaiDuAnType.Pending;
            }
            else
            {
                vm.StrStatus = strStatus;
                if(!string.IsNullOrEmpty(strStatus))
                {
                    var col = strStatus.Split(',');
                    foreach (var item in col)
                    {
                        trangThaiDuAnIds.Add(int.Parse(item));
                    }
                }
                vm.StrNguoiDung = strNguoiDungId;
                if (!string.IsNullOrEmpty(strNguoiDungId))
                {
                    var col = strNguoiDungId.Split(',');
                    foreach (var item in col)
                    {
                        nguoiDungIds.Add(Guid.Parse(item));
                    }
                }
                vm.TypeGroup = typeGroup??1;
            }   
            
            var duAns  = _model.GetsBy(vm.Type,nguoiDungIds,trangThaiDuAnIds);
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var bangluong = GetDataLuongThang(path + "LuongThang.xlsx", "LuongThang");          
            if (duAns != null && duAns.Count > 0)
            {
                if (vm.TypeGroup == 1)
                {
                    duAns = duAns.OrderBy(t => t.KhoaChaId).ThenBy(t => t.NguoiQuanLyId).ToList();
                    List<DuAnViewModel> lst = new List<DuAnViewModel>();
                    DuAnViewModel duAn = new DuAnViewModel();
                    foreach(var item in duAns)
                    {
                        item.CostH = _model.CostHCuaDuAnThanhPhan(item.DuAnId, bangluong);
                        item.Cost = 0;
                        if(item.CostH>0)
                        item.Cost +=item.CostH ;
                        if (item.CostTien > 0)
                            item.Cost += item.CostTien;
                        if (duAn.DuAnId == 0)
                        {
                            duAn.DuAnId = item.KhoaChaId.Value;
                            duAn.TenDuAn = item.TenKhoaCha;
                            duAn.DuAns = new List<DuAnViewModel>();
                        }
                        if (duAn.DuAnId == item.KhoaChaId)
                        {
                            duAn.DuAns.Add(item);
                        }
                        else
                        {
                            if (duAn.DuAns != null && duAn.DuAns.Count > 0)
                            {
                                duAn.DuAns = duAn.DuAns.OrderBy(t => t.TenHoatDong).ThenByDescending(t => t.TrangThaiDuAnId).ToList(); ;
                            }
                            lst.Add(duAn);
                            duAn = new DuAnViewModel() { DuAnId = item.KhoaChaId.Value, TenDuAn = item.TenKhoaCha, DuAns = new List<DuAnViewModel>() { item } };
                        }

                        
                    }
                    if (duAn.DuAns != null && duAn.DuAns.Count > 0)
                    {
                        duAn.DuAns = duAn.DuAns.OrderBy(t => t.TenHoatDong).ThenByDescending(t => t.TrangThaiDuAnId).ToList(); ;
                    }
                    lst.Add(duAn);
                    vm.DuAns = lst;
                }
                else
                {
                    duAns = duAns.OrderBy(t=>t.NguoiQuanLyId).ThenBy(t => t.KhoaChaId).ToList();
                    List<NguoiDungViewModel> lst = new List<NguoiDungViewModel>();
                    NguoiDungViewModel nd = new NguoiDungViewModel();
                    foreach (var item in duAns)
                    {
                        item.CostH = _model.CostHCuaDuAnThanhPhan(item.DuAnId, bangluong);
                        item.Cost = item.CostH + item.CostTien;
                        if (nd.NguoiDungId == Guid.Empty)
                        {
                            nd.NguoiDungId = item.NguoiQuanLyId;
                            nd.HoTen = item.HoTenNguoiQuanLy;
                            nd.DuAns = new List<DuAnViewModel>();
                        }
                        if (nd.NguoiDungId == item.NguoiQuanLyId)
                        {
                            nd.DuAns.Add(item);
                        }
                        else
                        {
                            if (nd.DuAns != null && nd.DuAns.Count > 0)
                            {
                                nd.DuAns = nd.DuAns.OrderBy(t => t.TenHoatDong).ThenByDescending(t => t.TrangThaiDuAnId).ToList(); ;
                            }
                            lst.Add(nd);
                            nd = new NguoiDungViewModel() { NguoiDungId = item.NguoiQuanLyId, HoTen = item.HoTenNguoiQuanLy, DuAns = new List<DuAnViewModel>() { item } };
                        }
                       
                    }
                    if (nd.DuAns != null && nd.DuAns.Count > 0)
                    {
                        nd.DuAns = nd.DuAns.OrderBy(t => t.TenHoatDong).ThenByDescending(t => t.TrangThaiDuAnId).ToList(); ;
                    }
                    lst.Add(nd);
                    vm.NguoiDungs = lst;
                }
            }

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
            DuAnModel model = new DuAnModel();
            var vm = model.GetById(duAnId, userId);
            if (vm == null) return RedirectToAction("index");
            CauHinhProvider ch = new CauHinhProvider();
            var ma = ch.GetValueByTen("Project/Edit/" + duAnId);
            if (string.IsNullOrEmpty(ma))
            {
                ma = System.Configuration.ConfigurationManager.AppSettings.Get("LinkProjectEdit");
            }           
            ViewBag.DocIdDefault = ma;
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var bangluong = GetDataLuongThang(path + "LuongThang.xlsx", "LuongThang");
            vm.CostH = model.CostHCuaDuAn(duAnId,bangluong);
            vm.CostTien = model.CostTienCuaDuAn(duAnId);
            vm.Cost = 0;
            if (vm.CostH > 0) vm.Cost += vm.CostH;
            if (vm.CostTien > 0) vm.Cost += vm.CostTien;

            vm.StrCost = vm.Cost.Value.ToString("C0").Substring(1);
            vm.TongNganSach = model.TongNhanSach(duAnId);
            if (vm.TongNganSach == null) vm.TongNganSach = 0;
            vm.StrTongNganSach =vm.TongNganSach.Value.ToString("C0").Substring(1);
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
            var tuan2 = Helpers.GetNumberWeekOfDay(DateTime.Now);
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

        public ActionResult PartialViewSynchrony(string link)
        {
            TableGiaiDoanDuAnCodaViewModel vm = new TableGiaiDoanDuAnCodaViewModel();
            docId = link;
            var doc = "";
            if (!string.IsNullOrEmpty(docId))
            {
                doc = docId.Substring(docId.IndexOf("_d")+2);
                if (doc.IndexOf("/") != -1)
                {
                    doc = doc.Substring(0, doc.IndexOf("/"));
                }
            }
            codaApi = System.Configuration.ConfigurationManager.AppSettings.Get("LinkCodaApi")+ doc;

            var result = CheckLinkCoda();
            if (result.Status)
            {
                CauHinhProvider cauHInhP = new CauHinhProvider();
                cauHInhP.InsertOrUpdte("Project/Index", link);
              vm.lst=  GetTable();
                if (vm.lst != null && vm.lst.Count > 0) vm.message = JsonConvert.SerializeObject(vm.lst);
            }
            else
            {
                vm.message = result.Message;
            }
            return View(vm);           
        }


        public ActionResult PartialViewDuAnThanhPhan(int duAnId, int? typeGroup)
        {
            DuAnIndexViewModel vm = new DuAnIndexViewModel();
            ThoiGianLamViecProvider tgM = new ThoiGianLamViecProvider();
            var duAns = _model.GetDuAnThanhPhanByKhoaCha(duAnId);
            if (duAns != null && duAns.Count > 0)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var table = GetDataLuongThang(path+"LuongThang.xlsx","LuongThang");
                foreach(var item in duAns)
                {
                    var tienLuong = 0;
                    var lstuser = tgM.GetUserSpentTimeInDuAn(item.DuAnId);
                    if (lstuser != null && lstuser.Count > 0)
                    {
                        foreach(var userId in lstuser)
                        {
                            var nguoiDung = _nguoiDungM.GetById(userId);
                            var timespent = tgM.GetTimeSpentOfNguoiDungInDuAnThanhPhan(item.DuAnId, userId);
                            if (timespent > 0)
                            {
                                try
                                {
                                    var u = table.Where(t => t.HoTen.ToUpper().Trim() == nguoiDung.HoTen.Trim().ToUpper()).FirstOrDefault();
                                   
                                }
                                catch { }
                            }
                        }
                        
                    }
                }
                if (typeGroup == null || typeGroup == 0)
                {
                    vm.DuAns = duAns;
                }
                else
                {
                    duAns = duAns.OrderBy(t => t.KhoaChaId).ThenBy(t => t.KhoaChaId).ToList();
                    List<NguoiDungViewModel> lst = new List<NguoiDungViewModel>();
                    NguoiDungViewModel nd = new NguoiDungViewModel();
                    foreach (var item in duAns)
                    {
                        if (nd.NguoiDungId == Guid.Empty)
                        {
                            nd.NguoiDungId = item.NguoiQuanLyId;
                            nd.HoTen = item.HoTenNguoiQuanLy;
                            nd.DuAns = new List<DuAnViewModel>();
                        }
                        if (nd.NguoiDungId == item.NguoiQuanLyId)
                        {
                            nd.DuAns.Add(item);
                        }
                        else
                        {
                            lst.Add(nd);
                            nd = new NguoiDungViewModel() { NguoiDungId = item.NguoiQuanLyId, HoTen = item.HoTenNguoiQuanLy, DuAns = new List<DuAnViewModel>() { item } };
                        }
                    }
                    lst.Add(nd);
                    vm.NguoiDungs = lst;
                }
            }
            vm.TypeGroup = typeGroup ?? 0;
                return View(vm);
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
        CodaColsTableProjectViewModel colAll;
        public List<GiaiDoanDuAnCodaViewModel> GetTable()
        {
        
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
                    foreach(var item in obj.items)
                    {
                        if(item.name.ToUpper()== "STATUS PROJECT")
                        {
                           GetColsByTableName(item.name);
                           return  GetRowsByTableName(item.name);


                            
                        }
                    }
                }
            }
           
            return null;
        }
        public List<GiaiDoanDuAnCodaViewModel> GetRowsByTableName(string tableName )
        {
            
            var client = new RestClient(codaApi + "/tables/"+ tableName+"/rows");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 845710e3-dcdf-4515-8e16-f3f14e80ac42");
            request.AddHeader("Cookie", "user_consent=eyJhbmFseXRpY3NBbGxvd2VkIjp0cnVlLCJhZHZlcnRpc2luZ0FsbG93ZWQiOmZhbHNlLCJwdWJsaXNoZXJBbmFseXRpY3NBbGxvd2VkIjp0cnVlfQ");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                
                List<GiaiDoanDuAnCodaViewModel> lstCoda = new List<GiaiDoanDuAnCodaViewModel>();
                var rows = JsonConvert.DeserializeObject< CodaRowsTableProjectViewModel>(response.Content);
                if (rows != null && rows.items != null && rows.items.Count > 0)
                {
                    foreach(var row in rows.items)
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
                                foreach(var item in b)
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
                               
                                var str= item.Value;
                             
                                try
                                {
                                    var col = colAll.items.Where(t => t.id == item.Key).FirstOrDefault();
                                    int tien;
                                    switch (col.name)
                                    {
                                        case "Project name":
                                            vm.ProjectName = str;
                                            break;
                                        case "Status":
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                vm.Status = str.Substring(str.IndexOf(".")+1).Trim();
                                            }
                                            
                                            break;
                                        case "Cost tiền?":
                                            int.TryParse(str.Replace(",","").Replace("₫",""), out tien);
                                            vm.CostTien = tien;
                                            break;
                                        case "Risk (delivery???)":
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                vm.Risk = str.Substring(str.IndexOf(".") + 1).Trim();
                                            }                                           
                                            break;
                                        case "Cost h":
                                            int.TryParse(str.Replace(",", "").Replace("₫", ""), out tien);
                                            vm.CostH = tien;
                                            break;
                                        case "Document HTML":
                                            if(!string.IsNullOrEmpty(str))
                                            vm.Document = str.Replace("”", "");
                                            break;
                                        case "Note":
                                            vm.Note = str;
                                            break;
                                        case "Slack_Name":
                                            vm.Slack_Name = str;
                                            break;
                                        case "Budget":
                                            int.TryParse(str.Replace(",", "").Replace("₫", ""), out tien);
                                            vm.Budget = tien;
                                            break;
                                        case "Tên Leader":
                                            vm.TeamLead = str;
                                            break;
                                        case "Type":
                                            vm.Type = str;
                                            break;
                                        case "Parent":
                                            vm.Parent = str;
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
                            if (!string.IsNullOrEmpty(vm.Ma) && !string.IsNullOrEmpty(vm.ProjectName) && !string.IsNullOrEmpty(vm.Parent) && !string.IsNullOrEmpty(vm.Type))
                                lstCoda.Add(vm);
                        }
                        catch
                        {

                        }
                      
                    }
                }
                if (lstCoda != null && lstCoda.Count > 0)
                {                 
                    foreach(var project in lstCoda)
                    {
                        var duAn = _model.GetByMa(project.Ma);
                        if (duAn != null && duAn.DuAnId > 0)
                        {
                            if (duAn.TenVietTat == null) duAn.TenVietTat = "";
                            var a = (duAn.MoTa+"").Replace("blank=", "blank").Replace("\"","").Replace("<br/>","<br>").Replace("&amp;","&");
                            var b = (project.Document + "").Replace("<br/>", "<br>");
                            var c = project.TeamLead;
                            if (c == "") c = duAn.HoTenNguoiQuanLy;
                            if (duAn.TenDuAn != project.ProjectName || project.Budget != duAn.TongNganSach || project.CostTien != duAn.CostTien || (b != a) || project.Note + "" != duAn.GhiChu + "" || project.Parent != duAn.TenKhoaCha || project.Risk + "" != duAn.TenHoatDong + "" || project.Status != duAn.TenTrangThaiDuAn || (c.ToLower() != duAn.HoTenNguoiQuanLy.ToLower()) || project.Type != duAn.TenLoaiDuAn+""||(project.Slack_Name.ToLower()!=duAn.TenVietTat.ToLower()&&(!string.IsNullOrEmpty(project.Slack_Name)||!string.IsNullOrEmpty(duAn.TenVietTat))))
                            {
                                project.isUpdate = true;                             
                            }
                        }else
                        {
                            project.isInsert = true;                        
                        }
                    }
                    lstCoda= lstCoda.OrderByDescending(t => t.isInsert).ThenByDescending(t => t.isUpdate).ToList();
                }

                return lstCoda;
            }
            else
            {
                return null;
            }
           
        }

        
        public string GetShipablesFromDuAnThanhPhan(int duAnId,string linkCoda)
        {
            var maDuAn = _model.GetMaDuAnById(duAnId);
            if (string.IsNullOrEmpty(maDuAn)) return null;

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
                CauHinhProvider ch = new CauHinhProvider();
                ch.InsertOrUpdte("Project/Edit/" + duAnId, linkCoda);
                var obj = JsonConvert.DeserializeObject<CodaTablesViewModel>(response.Content);
                if (obj != null && obj.items != null && obj.items.Count > 0)
                {
                    foreach (var item in obj.items)
                    {
                        if (item.name.ToUpper() == "PROJECT " + DateTime.Now.Year)
                        {
                            GetColsByTableName(item.name);
                            return JsonConvert.SerializeObject(GetRowByTableNameToShipable(item.name, duAnId));



                        }
                    }
                }
            }
            return null;
        }
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
                CauHinhProvider ch = new CauHinhProvider();
                ch.InsertOrUpdte("Project/Edit/" + duAnId, linkCoda);
                var obj = JsonConvert.DeserializeObject<CodaTablesViewModel>(response.Content);
                if (obj != null && obj.items != null && obj.items.Count > 0)
                {
                    foreach (var item in obj.items)
                    {
                        if (item.name.ToUpper() == "PROJECT " + DateTime.Now.Year)
                        {
                            GetColsByTableName(item.name);
                            return JsonConvert.SerializeObject(GetRowsToShipableBy(item.name, duAnId));



                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Lấy danh sách đầu mục công việc của dự án thành phần
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="duAnId"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetRowByTableNameToShipable(string tableName, int duAnThanhPhanId)
        {
            List<CongViecViewModel> lstToReturn = new List<CongViecViewModel>();
            var duAn = _model.GetShortInfoById(duAnThanhPhanId);
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
                   
                    if (row != null&&!string.IsNullOrEmpty(row.values.ToString()))
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
                                        case "ID":
                                            vm.Ma = str;
                                            break;


                                    }
                                }
                                catch
                                {

                                }
                            }

                            if (!string.IsNullOrEmpty(vm.Document))
                            {
                                var str = vm.Document;
                                while (str.IndexOf("\\n") != -1)
                                {
                                    var tenCongViec = str.Substring(0, str.IndexOf("\\n"));
                                    if (!string.IsNullOrEmpty(tenCongViec))
                                    {
                                        var col = tenCongViec.Split('_');
                                        if (col.Length > 1)
                                        {
                                            CongViecViewModel cv = new CongViecViewModel();                                            
                                                cv.TenCongViec = col[1];
                                                cv.HoTen = col[0];  
                                            if (!string.IsNullOrEmpty(cv.TenCongViec))
                                                lstToReturn.Add(cv);
                                        }
                                        
                                       
                                    }                                
                                    
                                    str = str.Substring(str.IndexOf("\\n") + 2);
                                }
                                if (!string.IsNullOrEmpty(str))
                                {
                                    
                                    var col = str.Split('_');
                                    if (col.Length > 1)
                                    {
                                        CongViecViewModel cv = new CongViecViewModel();
                                        cv.TenCongViec = col[1];
                                        cv.HoTen = col[0];
                                        if (!string.IsNullOrEmpty(cv.TenCongViec))
                                            lstToReturn.Add(cv);
                                    }   
                                }

                            }
                        }
                        catch
                        {

                        }

                    }


                }
                if(lstToReturn!=null&& lstToReturn.Count > 0)
                {
                  
                    foreach(var item in lstToReturn)
                    {
                        if (!_congViecM.CheckIsExitTenCongViec(item.TenCongViec.Trim(), duAnThanhPhanId)) item.IsAddNew = true;
                        else
                        {
                            var cv = _congViecM.GetShipByTen(item.TenCongViec, duAnThanhPhanId);
                            if (cv != null && cv.CongViecId > 0) {
                                if (cv.NguoiXuLyId == null) item.IsUpdate = true;
                                else
                                {
                                    var nguoiDung = _nguoiDungM.GetById(cv.NguoiXuLyId.Value);
                                    if (nguoiDung != null && nguoiDung.NguoiDungId != Guid.Empty && !nguoiDung.TenDangNhap.Trim().ToLower().Equals(item.HoTen.Trim().ToLower()))
                                    {
                                        item.IsUpdate = true;
                                    }
                                }
                               
                            }
                        }
                    }
                }
            }
                return lstToReturn;
        }
        /// <summary>
        /// Lấy danh sách dự án thành phần của dự án và các đầu mục trong dự án thành phần
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="duAnId"></param>
        /// <returns></returns>
        public List<DuAnViewModel> GetRowsToShipableBy(string tableName, int duAnId)
        {
            List<DuAnViewModel> lstToReturn = new List<DuAnViewModel>();
            lstToReturn = _model.GetDuAnThanhPhanByKhoaCha(duAnId);
            if (lstToReturn != null && lstToReturn.Count > 0)
            {
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
                        foreach(var duAn in lstToReturn)
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
                                List<CongViecViewModel> lstDauMucCV = new List<CongViecViewModel>();
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
                                    var strDauMuc = "";

                                    foreach (var item in result1)
                                    {

                                        var str = item.Value;

                                        try
                                        {
                                            var col = colAll.items.Where(t => t.id == item.Key).FirstOrDefault();
                                            switch (col.name)
                                            {                                                
                                                case "Shipable":
                                                    strDauMuc = str;
                                                    break;                                             
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    if (!string.IsNullOrEmpty(strDauMuc))
                                    {
                                        var str = strDauMuc;
                                        while (str.IndexOf("\\n") != -1)
                                        {
                                            var tenCongViec = str.Substring(0, str.IndexOf("\\n"));
                                            if (!string.IsNullOrEmpty(tenCongViec))
                                            {
                                                var col = tenCongViec.Split('_');
                                                if (col.Length > 1)
                                                {
                                                    CongViecViewModel cv = new CongViecViewModel();
                                                    cv.TenCongViec = col[1];
                                                    cv.HoTen = col[0];
                                                    if (!string.IsNullOrEmpty(cv.TenCongViec))
                                                        lstDauMucCV.Add(cv);
                                                }


                                            }

                                            str = str.Substring(str.IndexOf("\\n") + 2);
                                        }
                                        if (!string.IsNullOrEmpty(str))
                                        {

                                            var col = str.Split('_');
                                            if (col.Length > 1)
                                            {
                                                CongViecViewModel cv = new CongViecViewModel();
                                                cv.TenCongViec = col[1];
                                                cv.HoTen = col[0];
                                                if (!string.IsNullOrEmpty(cv.TenCongViec))
                                                    lstDauMucCV.Add(cv);
                                            }
                                        }

                                    }
                                }
                                catch
                                {

                                }
                                duAn.CongViecs = lstDauMucCV;
                            }
                        }
                       


                    }
                    if (lstToReturn != null && lstToReturn.Count > 0)
                    {

                        foreach (var duAn in lstToReturn)
                        {
                            if (duAn.CongViecs != null && duAn.CongViecs.Count > 0)
                            {
                                var rowspan = 0;
                                foreach(var congViec in duAn.CongViecs)
                                {
                                    if (!_congViecM.CheckIsExitTenCongViec(congViec.TenCongViec.Trim(), duAn.DuAnId)) { 
                                        congViec.IsAddNew = true;
                                        rowspan++;
                                    }
                                }
                                duAn.Rowspan = rowspan;
                            }
                        
                        }
                    }
                }
             
            }
            return lstToReturn;
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
       
     

        public string GetProjectInfoById(int duAnId)
        {
            return JsonConvert.SerializeObject(_model.GetById3(duAnId));
        }

        [HttpPost]
        public string AddProjectFromCoda (TableGiaiDoanDuAnCodaViewModel vm)
        {
            ResultViewModel result = new ResultViewModel();
            if (vm != null && vm.lst != null && vm.lst.Count > 0)
            {
                var lstString = GetChannelNames();
                result = _model.AddProjectFromCoda(vm.lst, GetUserID(), lstString);
            }
                return JsonConvert.SerializeObject(result);
        }

        public List<string> GetChannelNames()
        {

            var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiConversationsList");
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");     
            var client = new RestClient(link);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                try
                {
                    var lst = JsonConvert.DeserializeObject<ConversationsListViewModel>(response.Content);
                    return lst.channels.Select(t=>t.name).ToList();
                }
                catch
                {

                }

                return null;
            }
            return null;

        }


        public bool AddListShipableFromCoda(CongViecViewModel vm)
        {
            try
            {
                var userId = GetUserID();
                if (vm != null && vm.CongViecs != null && vm.CongViecs.Count > 0)
                {                    
                        foreach (var item in vm.CongViecs)
                        {
                        if (!string.IsNullOrEmpty(item.TenCongViec))
                        {
                            var duaN = _model.GetShortInfoById(item.DuAnId);
                            var nguoiDungN = _nguoiDungM.GetNguoiDungByUserName(item.HoTen);
                            if (!_congViecM.CheckIsExitTenCongViec(item.TenCongViec.Trim(), item.DuAnId))
                            {
                               
                                if(nguoiDungN != null&& nguoiDungN.NguoiDungId!=Guid.Empty)
                                _congViecM.InsertShipableToDoWork(item, nguoiDungN.NguoiDungId);
                            }
                                
                            else
                            {
                                var cv = _congViecM.GetShipByTen(item.TenCongViec.Trim(), item.DuAnId);
                                var update = false;
                                if (cv.NguoiXuLyId == null) update = true;
                                else
                                {
                                    var nguoiDung = _nguoiDungM.GetById(cv.NguoiXuLyId.Value);
                                    if (nguoiDung != null && nguoiDung.NguoiDungId != Guid.Empty && !nguoiDung.TenDangNhap.Trim().ToLower().Equals(item.HoTen.Trim().ToLower()))
                                    {
                                       
                                        update = true;
                                    }
                                }
                                if (update)
                                {
                                    _congViecM.UpdateNguoiXuLyOfShip(cv.CongViecId, nguoiDungN.NguoiDungId);
                                }
                            }
                        }
                            
                        }
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        [HttpGet]
        public string GetPhongBans()
        {
            PhongBanModel model = new PhongBanModel();
            return JsonConvert.SerializeObject(model.GetAll());
        }


        [HttpPost]
        public bool InsertLienKetDuAnPhongBan(LienKetDuAnPhongBanViewModel vm)
        {
           
            return _model.InsertLienKetDuAnPhongBan(vm.DuAnId,vm.PhongBanId);
        }

        [HttpDelete]
        public bool DeleteLienKetDuAnPhongBan(LienKetDuAnPhongBanViewModel vm)
        {
            return _model.DeleteLienKetDuAnPhongBan(vm.DuAnId, vm.PhongBanId);
        }


        public List<NguoiDungViewModel> GetDataLuongThang(string fileName, string sheetName)
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            ExcelOpenXml excel = new ExcelOpenXml(fileName, sheetName);
            var table= excel.ReadDataTable(2, false);
            if (table != null && table.Rows.Count > 0)
            {
                foreach(DataRow row in table.Rows)
                {
                    NguoiDungViewModel nd = new NguoiDungViewModel();
                    nd.HoTen = row[0].ToString();
                    nd.LuongThang = double.Parse(row[1].ToString());
                    lstToReturn.Add(nd);
                }
            }
            return lstToReturn;
        }
    }
}