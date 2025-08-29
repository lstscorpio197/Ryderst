using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopAdmin.Dto.User
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}