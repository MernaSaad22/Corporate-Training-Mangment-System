using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class EmployeeRequest
    {
        public string ApplicationUserId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyId { get; set; }
    }
}
