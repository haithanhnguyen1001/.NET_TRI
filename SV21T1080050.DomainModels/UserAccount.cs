using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080050.DomainModels
{
    /// <summary>
    /// Thông tin tài khoản trong CSDL
    /// </summary>
    public class UserAccount
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string RoleNames { get; set; } = "";

    }
}
