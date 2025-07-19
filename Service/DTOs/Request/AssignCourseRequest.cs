using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class AssignCourseRequest
    {
        public int CourseId { get; set; }
        public string EmployeeId { get; set; }

        public int DeadlineDays { get; set; }
    }
}
