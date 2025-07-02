using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class PlanResponse
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxCourses { get; set; }
        public int MaxEmployees { get; set; }
    }
}
