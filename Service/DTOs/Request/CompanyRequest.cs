using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class CompanyRequest
    {


        public string Name { get; set; } = null!;

        public int PlanId { get; set; }
      
        public IFormFile? Logo { get; set; }

        public string ApplicationUserId { get; set; }//owner

        public DateTime TransactionDate { get; set; }

    }
}
