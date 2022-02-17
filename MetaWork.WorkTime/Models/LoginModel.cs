using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string password { get; set; }
        public bool rememberMe { get; set; }
    }
}