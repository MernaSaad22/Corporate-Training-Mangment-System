using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class PlanRequest
    {
        public string Name { get; set; }
        public int MaxCourses { get; set; }
        public int MaxEmployees { get; set; }
    }
}
