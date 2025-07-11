using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeResponse
    {
        public string Id { get; set; }
        public string JobTitle { get; set; }
        public string CompanyId { get; set; }
        public string ApplicationUserId { get; set; }
        public string UserName { get; set; }
    }
}
