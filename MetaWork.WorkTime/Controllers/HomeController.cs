using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MetaWork.WorkTime.Models;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using BasicAuthentication;
using System.Timers;
using System.Net;
using System.IO;
using System.Security.AccessControl;
using Newtonsoft.Json.Linq;

namespace MetaWork.WorkTime.Controllers
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
        System.Timers.Timer timer;
        private object _nguoiDungP;

        //private void SetUpTimer(TimeSpan alertTime)
        //{
        //    DateTime current = DateTime.Now;
        //    TimeSpan timeToGo = alertTime - current.TimeOfDay;
        //    if (timeToGo < TimeSpan.Zero)
        //    {
        //        return;//time already passed
        //    }
        //    this.timer = new System.Threading.Timer(x =>
        //    {
        //        this.SomeMethodRunsAt1600();
        //    }, null, timeToGo, Timeout.InfiniteTimeSpan);
        //}

        private void SomeMethodRunsAt1600()
        {
            //this runs at 16:00:00
        }

        public ActionResult HuyDuyet(int thoiGianLamViecId)
        {
            ThoiGianLamViecProvider thoiGianLamViecM = new ThoiGianLamViecProvider();
            var delete = thoiGianLamViecM.HuyDuyet(thoiGianLamViecId);
            if (delete) ViewBag.Message = "Bạn hủy duyệt thành công.";
            else ViewBag.Message = "Bạn hủy duyệt không thành công.";
            return View();
        }

        public ActionResult PheDuyet(int thoiGianLamViecId, string tokenId, string CallBackAction, string CallBackToken, string CallBackDescription)
        {
            ThoiGianLamViecProvider thoiGianLamViecM = new ThoiGianLamViecProvider();
            var update = thoiGianLamViecM.UpdatePheDuyetBy(thoiGianLamViecId, tokenId);
            if (update)
            {
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

                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApproveStaffDay") + "?token=" + CallBackToken + "&description=" + CallBackDescription);
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
        public void ServiceAuto()
        {
            timer = new System.Timers.Timer();
            this.timer.Interval = 120000; // 60 seconds
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Tick);
            this.timer.Enabled = true;
        }
        private void timer_Tick(object sender, ElapsedEventArgs e)
        {

            CongViecModel model = new CongViecModel();
            model.AuToSendMailShipInActive();
            model.AuToSendMailDoneTask();
        }

        public void AddAuthenticationFolder()
        {
            //var pathProject = AppDomain.CurrentDomain.BaseDirectory;
            //string DirectoryName = pathProject+ "Uploads";
            //AddDirectorySecurity(DirectoryName, @"CuongVh", FileSystemRights.ReadData, AccessControlType.Allow);
        }
        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            //// Create a new DirectoryInfo object.
            //DirectoryInfo dInfo = new DirectoryInfo(FileName);

            //// Get a DirectorySecurity object that represents the
            //// current security settings.
            //DirectorySecurity dSecurity = dInfo.GetAccessControl();

            //// Add the FileSystemAccessRule to the security settings.
            //dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
            //                                                Rights,
            //                                                ControlType));

            //// Set the new access settings.
            //dInfo.SetAccessControl(dSecurity);
        }
        public string Test()
        {
            Settup s = new Settup();
            s.SettupChannel();
            //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            //myRequest.Method = "GET";
            //WebResponse myResponse = myRequest.GetResponse();
            //StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            //string result = sr.ReadToEnd();
            //sr.Close();
            //myResponse.Close();


            //ThoiGianLamViecModel model = new ThoiGianLamViecModel();
            //var c=  model.GetTimeOfUserBys(new List<string>() { "ngochai@tecostore.vn" }, 1622505600, 1625097500);
            return "";
        }
        public ActionResult testUpload()
        {
            return View();
        }

        public ActionResult UploadFiles()
        {
            string FileName = "";
            string result = "";
            Guid FileGuild = Guid.NewGuid();
            FileProvider manager = new FileProvider();
            while (manager.IsExist(FileGuild))
            {
                FileGuild = Guid.NewGuid();
            }
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";    
                //string filename = Path.GetFileName(Request.Files[i].FileName);    

                HttpPostedFileBase file = files[i];
                string fname;

                // Checking for Internet Explorer    
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;

                }
                var ext = fname.Substring(fname.LastIndexOf(".") + 1);
                var filename = file.FileName;
                FileName = FileGuild + "." + ext;
                // Get the complete folder path and store the file inside it.
                fname = Server.MapPath("~/Uploads/") + FileGuild + "." + ext;
                fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                file.SaveAs(fname);
                var host = string.Format("{0}://{1}{2}{3}",
                System.Web.HttpContext.Current.Request.Url.Scheme,
                System.Web.HttpContext.Current.Request.Url.Host,
                System.Web.HttpContext.Current.Request.Url.Port == 80 ? string.Empty : ":" + System.Web.HttpContext.Current.Request.Url.Port,
                System.Web.HttpContext.Current.Request.ApplicationPath);
                //File Anh
                List<string> fileAnh = new List<string>() { "BMP", "JPG", "PNG" };

                if (fileAnh.Contains(ext.ToUpper()))
                {
                    result = "<p name=\"" + filename + "\" class=\"ImportNewFile\" id=\"" + FileGuild + "_" + ext + "\"><img src=\"" + host + "Uploads/" + FileName + "\"  width=\"300\" height=\"300\" </p>";
                }
                else
                {
                    result = "<p name=\"" + filename + "\"   class=\" ImportNewFile\" id=\"" + FileGuild + "_" + ext + "\"><a href=\"" + host + "Uploads/" + FileName + " \" title=\"" + file.FileName + "\"  >" + file.FileName + "</a></p>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OauthRedirect()
        {
            //https://localhost:44303/
            //http://beta.tecotec.vn
            var credentialsFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\credentials.json";
            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            var client_id = credentials["client_id"].ToString();
            var redirectURL = "https://accounts.google.com/o/oauth2/v2/auth?" +
 "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events&" +
 "access_type=online&" +
 "include_granted_scopes=true&" +
 "response_type=code&" +
 "state=hellothere&" +
 "redirect_uri=http://beta.tecotec.vn/oauth/callback&" +
 "client_id=" + client_id;

            return Redirect(redirectURL);
        }
    }
}