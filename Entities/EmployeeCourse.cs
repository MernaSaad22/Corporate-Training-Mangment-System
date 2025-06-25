using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeCourse
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; }  // Role=Employee
        public Employee Employee { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
