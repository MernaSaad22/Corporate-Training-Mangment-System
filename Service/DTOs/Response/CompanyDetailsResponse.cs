using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class CompanyDetailsResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Logo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
