using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ResendEmailRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
