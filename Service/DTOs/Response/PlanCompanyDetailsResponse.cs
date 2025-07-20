using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class PlanCompanyDetailsResponse
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxCourses { get; set; }
        public int MaxEmployees { get; set; }
        public decimal Cost { get; set; }
        public int DurationInDays { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DaysRemaining { get; set; }

        public bool IsExpired { get; set; }         
        public string StatusMessage { get; set; } = ""; 
    }
}
