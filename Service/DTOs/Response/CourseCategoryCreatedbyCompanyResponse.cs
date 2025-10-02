using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class CourseCategoryCreatedbyCompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
