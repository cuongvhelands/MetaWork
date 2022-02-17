using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using MetaWork.WorkTime.Chat;
using MetaWork.WorkTime.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;

namespace MetaWork.WorkTime.Controllers
{
    [Authorize]
    public class MetaWorkController : Controller
    {
        NguoiDungProvider _nguoiDungP = new NguoiDungProvider();
        // GET: MetaWork
        public ActionResult Index()
        {
            return View();
        }       

        protected string _url;
        string html = "";
        public string GetWebpage(string url)
        {
            _url = url;
            // WebBrowser is anadgad ActiveX control that must be run in a
            // single-threaded apartment so create a thread to create the
            // control and generate the thumbnail
            Thread thread = new Thread(new ThreadStart(GetWebPageWorker));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            string s = html;
            return s;
        }

        protected void GetWebPageWorker()
        {
            using (WebBrowser browser = new WebBrowser())
            {
                //  browser.ClientSize = new Size(_width, _height);
                browser.ScrollBarsEnabled = false;
                browser.ScriptErrorsSuppressed = true;
                browser.Navigate(_url);

                // Wait for control to load page
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();

                html = browser.DocumentText;

            }
        }
        public ActionResult PartialViewChannelChat(int channelId)
        {        
            PhongChatModel model = new PhongChatModel();
            var user = GetUserID();
            var vm = model.GetInfoOfChannel(channelId);
            if(vm!=null)
            vm.User = user;
            return View(vm);
        }

        public ActionResult PartialViewPhongChat(int phongchat)
        {
            var user = GetUserID();
            PhongChatModel model = new PhongChatModel();
            var vm = model.GetBy(phongchat,user);
            return View(vm);
        }

        public ActionResult PartialViewTabFile(int phongchat,string fileCha)
        {
            PhongChatModel model = new PhongChatModel();
            PhongChatViewModel vm = new PhongChatViewModel();            
            vm.Files = model.GetFileOfChannel(phongchat,null,fileCha);
            vm.FileId = fileCha;
            if (string.IsNullOrEmpty(fileCha))
            {
                vm.StrHTMLTabFile = "";
            }
            return View(vm);
        }
        public ActionResult PartialViewTabDocument(int phongchat, string fileCha)
        {
            PhongChatModel model = new PhongChatModel();
            var vms = model.GetFileOfChannel(phongchat,2, fileCha);
            return View(vms);
        }
        public ActionResult PartialViewThread(int threadId)
        {
            PhongChatModel model = new PhongChatModel();
            var vm = model.GetByThreadId(threadId);
            var user = GetUserID();
            vm.User = user;
            return View(vm);
        }
        public ActionResult PartialViewAllOfThread(int threadId)
        {
            var start = DateTime.Now;
            PhongChatModel model = new PhongChatModel();
            var vm = model.GetByThreadId(threadId);
            var user = GetUserID();
            vm.User = user;
            var spent = (DateTime.Now - start).TotalSeconds;
            return View(vm);
        }
        public Guid GetUserID()
        {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return _nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }
        public bool CheckIsExistSpace(string spaceName)
        {
            PhongChatModel model = new PhongChatModel();
            return model.CheckIsExistChannel(spaceName, 0);
        }
        public class InportThread
        {
            [AllowHtml]
            public string content { get; set; }
            public int phongChatId { get; set; }
        }
        [HttpPost]
        public int InsertThread(InportThread thread)
        {
            var user = GetUserID();
            PhongChatModel model = new PhongChatModel();
            if (!string.IsNullOrEmpty(thread.content))
            {
                int type = 1;
              
                var path = System.Configuration.ConfigurationManager.AppSettings.Get("HostWeb") + "Uploads/";
                var newContent = model.ProcessingMessage(thread.content, thread.phongChatId, user,path);
                var newThreadId = model.InsertThread(user, thread.phongChatId, 1, newContent);
                // Save message:
                MessageModel messageM = new MessageModel();
                var messageId = messageM.Insert(user, newThreadId.ToString(), 2, newContent);
                if (messageId != Guid.Empty)
                {
                    PhongChatProvider pc = new PhongChatProvider();
                    List<Guid> nguoiDungIds = pc.GetNguoiDungIdsBy(thread.phongChatId);
                }
                return newThreadId;
            }
            return 0;

        }
        public string GetAllOfThread(int threadId)
        {
            PhongChatModel model = new PhongChatModel();
            return JsonConvert.SerializeObject(model.GetAllOfThread(threadId, GetUserID()));
        }
        public string InsertFile(string fileName, string fileGuild, byte fileType, int phongChatId)
        {
            PhongChatModel model = new PhongChatModel();
            model.InsertFile(GetUserID(), fileName, fileGuild, fileType, phongChatId.ToString(), 2);
            return "";
        }
        public int InsertThreadByUpdateFile(Guid fileId, int phongChatId)
        {
            PhongChatModel model = new PhongChatModel();            
            return model.InsertThreadByUpdateFile(fileId, phongChatId, GetUserID());
            
        }
        public ActionResult UploadTabFileLocal()
        {
            string result = "" ;
            var vm = new FileReturnViewModel();
             Guid FileGuild = Guid.NewGuid();
            FileProvider manager = new FileProvider();
            while (manager.IsExist(FileGuild))
            {
                FileGuild = Guid.NewGuid();
            }
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            { 
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
                var FileName = FileGuild + "." + ext;
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
                    result = "<p name=\"" + filename + "\" class=\"UploadFileLocal\" id=\"" + FileGuild + "_" + ext + "\"><img src=\"" + host + "Uploads/" + FileName + "\"  width=\"300\" height=\"300\" </p>";
                }
                else
                {
                    result = "<p name=\"" + filename + "\"  class=\"borderBlack text-center UploadFileLocal\" id=\"" + FileGuild + "_" + ext + "\"><a href=\"" + host + "Uploads/" + FileName + " \" title=\""+ file.FileName + "\"  >" + file.FileName + "</a></p>";
                }
                vm.TextContent = result;
                vm.FilePath = host + "Uploads/" + FileName;
                vm.FileName = file.FileName;

                result = JsonConvert.SerializeObject(vm);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public bool UploadFile(int phongChatId,string fileName,string filePath,byte fileType,string FileId)
        {
            var userId = GetUserID();
            FileModel model = new FileModel();
            if (string.IsNullOrEmpty(FileId))
            {
              
                if (model.InsertFileLocal(fileName, filePath, fileType, userId, phongChatId.ToString(), 2) != Guid.Empty)
                {
                    return true;
                }
            }
            else
            {
                if (model.UpdateFile(fileName, filePath, Guid.Parse(FileId))) return true;
            }
            
            
            return false;
        }
        public bool DeleteMessage(Guid messageId)
        {
            PhongChatModel model = new PhongChatModel();
            return model.DeleteMessage(messageId, GetUserID());
        }
        public bool DeleteFile(Guid fileId)
        {
            PhongChatModel model = new PhongChatModel();
            return model.DeleteFile(fileId, GetUserID());
        }
        public string GetFileInfo(Guid fileId)
        {
            FileModel model = new FileModel();
            return JsonConvert.SerializeObject(model.GetById(fileId));
        }
        public bool InsertFolder(int phongChatId,string fileName,string fileId)
        {
            FileModel model = new FileModel();
            var userId = GetUserID();
            if (string.IsNullOrEmpty(fileId))
            {
                if (model.InsertFileLocal(fileName, fileId, 0, userId, phongChatId.ToString(), 2) != Guid.Empty) return true;               
            }
            else
            {
                if (model.InsertFileLocal(fileName, fileId, 0, userId, fileId, 3) != Guid.Empty) return true;
            }
            return false;
        }
        [HttpPost]
        public string AddChannel(string title, List<Guid> userIds,string calendarId,string embedCalendar)
        {
            PhongChatModel model = new PhongChatModel();
            return JsonConvert.SerializeObject(model.AddChannel(title, userIds, calendarId, embedCalendar, GetUserID()));
        }
        [HttpPost]
        public string UpdateChannel(int phongChatId,string title, List<Guid> userIds, string calendarId, string embedCalendar)
        {
            PhongChatModel model = new PhongChatModel();
            return JsonConvert.SerializeObject(model.UpdateChannel(phongChatId,title, userIds, calendarId, embedCalendar, GetUserID()));
        }
        public string GetInfoChannel(int phongChatId)
        {
            PhongChatModel model = new PhongChatModel();
            return JsonConvert.SerializeObject(model.GetShortInfoOfChannel(phongChatId));
        }
        public string GetUserForSelect2()
        {
            var userId = GetUserID();
            NguoiDungModel model = new NguoiDungModel();
            var users = model.GetsExcept(new List<Guid>() { userId });
            List<ObjNguoiDungSelect2> lstTOReturn = new List<ObjNguoiDungSelect2>();
            if (users != null && users.Count > 0)
            {
                foreach(var item in users)
                {
                    lstTOReturn.Add(new ObjNguoiDungSelect2() { id = item.NguoiDungId, text = item.HoTen });
                }
                
            }
            return JsonConvert.SerializeObject(lstTOReturn);
        }
        public bool DeleteNguoiThamGia(int phongChatId, Guid nguoiDungId)
        {
            PhongChatModel model = new PhongChatModel();
            return model.DeleteNguoiThamGia(phongChatId, nguoiDungId);
        }
        public class ObjNguoiDungSelect2
        {
            public Guid id { get; set; }
            public string text { get; set; }
        }
        public ActionResult PartialViewTaskOutline(int phongChatId)
        {
            CongViecOutLineModel model = new CongViecOutLineModel();
            DuAnViewModel vm = model.GetAllOfDuAn(phongChatId);
            return View(vm);
        }
    }
}