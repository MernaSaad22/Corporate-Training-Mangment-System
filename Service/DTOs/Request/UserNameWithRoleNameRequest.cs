using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class UserNameWithRoleNameRequest
    {
        public string UserName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}
