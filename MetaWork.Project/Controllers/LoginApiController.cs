using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaWork.Project.Models;
using MetaWork.Data.ViewModel;

namespace MetaWork.Project.Controllers
{
    
    [RoutePrefix("api/Login")]
    public class LoginApiController : ApiController
    {
        [HttpPost]
        [Route("GetUserBy2")]
        public NguoiDungViewModel GetBy2(LoginViewModel vm)
        {
            NguoiDungModel model = new NguoiDungModel();
            return model.GetBy(vm.UserName, vm.PassWord);
        }

        [Route("GetUserBy/{userName}/{passWord}")]
        public NguoiDungViewModel GetsBy(string userName, string passWord)
        {
            NguoiDungModel model = new NguoiDungModel();
            return model.GetBy(userName, passWord);
        }

    }
}