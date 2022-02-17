using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using MetaWork.WorkTime.Chat;
using MetaWork.WorkTime.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MetaWork.WorkTime.Controllers
{
    public class WorkController : Controller
    {
        // GET: Work
        public ActionResult Index()
        {           
            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult PartialViewListUserChat()
        {
            var userId = GetUserID();
            NguoiDungModel ndM = new NguoiDungModel();
            List<NguoiDungViewModel> nd = ndM.GetAll();
            return View(nd);
        }
        public ActionResult PartialViewListChannelChat()
        {
            var userId = GetUserID();
            DuAnModel duAnM = new DuAnModel();
            List<DuAnViewModel> nds = duAnM.GetDuAnTpsBy(userId);
            return View(nds);
        }
        public Guid GetUserID()
        {
            NguoiDungProvider nguoiDungP = new NguoiDungProvider();
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungP.GetUserByUsername(userName).NguoiDungId;
        }
        public string GetChatUser(Guid userId)
        {
            NguoiDungModel ndM = new NguoiDungModel();
            var nd = ndM.GetById(userId);
            return JsonConvert.SerializeObject(nd);
        }
        public string GetChatChannel(int duAnId)
        {
            DuAnModel daM = new DuAnModel();
            var da = daM.GetShortInfoById(duAnId);
            return JsonConvert.SerializeObject(da);
        }
        public ActionResult PartialViewRoomChat(string itemId)
        {
            var nguoiDungId = GetUserID();
            int id = 0;
            int.TryParse(itemId, out id);
            byte type = 1;
            if (id == 0) type = 2;
            RoomViewModel vm = new RoomViewModel() { ItemType=type,ItemId= itemId };    
            if (type == 1)
            {
                DuAnModel daM = new DuAnModel();
                var da = daM.GetShortInfoById(int.Parse(itemId));
                PhongChatProvider pc = new PhongChatProvider();
                var phongId = pc.GetPhongChatIdBy(da.TenDuAn);
                if (phongId == 0)
                {
                    //phongId= pc.InsertPhongChat(da.TenDuAn, da.DuAnId.ToString(), type, da.TenDuAn);
                }                
                vm.PhongChatId=phongId.ToString();
                pc.InsertLienKetPhongChat(phongId, nguoiDungId);                
            }
            else
            {
                var userId = Guid.Parse(itemId);
                NguoiDungModel ndM = new NguoiDungModel();
                var nd = ndM.GetById(userId);
                vm.TenRoom = nd.HoTen;
                vm.PhongChatId = itemId;
             
                
            }
            MessageModel model = new MessageModel();
            var lstMess = model.GetsBy(nguoiDungId.ToString(),vm.PhongChatId, type, 1, 20);
            if (lstMess != null && lstMess.Count > 0)
            {
                vm.Messages = lstMess.OrderBy(t => t.NgayTao).ToList();
            }
            return View(vm);
        }

        public string GetAllUser()
        {
            NguoiDungModel ndM = new NguoiDungModel();
            return JsonConvert.SerializeObject(ndM.GetAll());
        }
    }
}