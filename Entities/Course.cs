using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string CompanyId { get; set; }
        public Company Company { get; set; }

        public string InstructorId { get; set; }  // Role=>instructor
        public Instructor Instructor { get; set; }

        public int CategoryId { get; set; }
        public CourseCategory Category { get; set; }

        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<EmployeeCourse> EmployeeCourses { get; set; }
    }
}
