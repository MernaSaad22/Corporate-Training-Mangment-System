using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeCourseShortResponse
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
