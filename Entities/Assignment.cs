using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string InstructorId { get; set; }  
        public Instructor Instructor { get; set; }

        public ICollection<EmployeeAssignment> EmployeeAssignments { get; set; }
    }
}
