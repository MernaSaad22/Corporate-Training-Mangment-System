using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeCourseProgress
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int CourseId { get; set; }

        public decimal LessonProgress { get; set; }    // 0 - 50
        public decimal AssignmentProgress { get; set; } // 0 - 25
        public decimal ExamProgress { get; set; }       // 0 - 25
        public decimal TotalProgress => LessonProgress + AssignmentProgress + ExamProgress;

        public DateTime LastUpdated { get; set; }

        public Employee Employee { get; set; }
        public Course Course { get; set; }
    }
}
