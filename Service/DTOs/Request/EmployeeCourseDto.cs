using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    internal class EmployeeCourseDto
    {
        public int CoursesEnrolled { get; set; }
        public int CoursesCompleted { get; set; }
        public int PointsEarned { get; set; }
    }
}
