using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class EmployeeRequest
    {
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]

        public string JobTitle { get; set; }
        [Required]

        public string CompanyId { get; set; }
    }
}
