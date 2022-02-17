using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MetaWork.WorkTime.Models;
using MetaWork.Data.Provider;
using MetaWork.Data;

namespace MetaWork.WorkTime.Controllers
{
    public class UserController : Controller
    {
        NguoiDungProvider nguoiDungProvider = new NguoiDungProvider();
        TimerDataContext db = new TimerDataContext();
        // GET: User
        public ActionResult Login()
        {
            return View();
        }
        public bool Active()
        {
            var user = GetUser();
            var date = DateTime.Now;
            var check = true;
            var active = nguoiDungProvider.GetActiveBy(user.NguoiDungId, new DateTime(date.Year, date.Month, date.Day, 5, 0, 0));
            if(active!=null)
            {
                if (active.Active) check = false;
                else check = true;
            }
            bool result;
            result = nguoiDungProvider.InsertOrUpdateActivity("Active", check, user.NguoiDungId, null);
            if (result)
            {
                ThoiGianLamViecModel ttM = new ThoiGianLamViecModel();
                if(check)
                ttM.SendMessageToSlack(user.HoTen + " clock in " );
                else
                    ttM.SendMessageToSlack(user.HoTen + " clock out ");
            }
            return result;
        }


        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var password = EndCode.Encrypt(model.password);
                var result = nguoiDungProvider.Login(model.userName, password);
                if (result == 1)
                {
                    var user = nguoiDungProvider.GetUserByUsernameAndPassword(model.userName, password);
                    FormsAuthentication.SetAuthCookie(model.userName, model.rememberMe);
                    return RedirectToAction("ToDoWork", "Todo");
                }
                else if (result == 2)
                {
                    ModelState.AddModelError("", "Mật khẩu không chính xác !");
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại !");
                }
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        public NguoiDung GetUser()
        {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return nguoiDungProvider.GetUserByUsername(userName);
        }
        public ActionResult ChangePassword()
        {
            var user = GetUser();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(Guid id, string newPassword)
        {
            nguoiDungProvider.ChangePassword(id, EndCode.Encrypt(newPassword));
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
    }
}