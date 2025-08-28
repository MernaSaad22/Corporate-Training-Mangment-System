using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeLessonProgress
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int LessonId { get; set; }
        public DateTime ViewedAt { get; set; }

        public Employee Employee { get; set; }
        public Lesson Lesson { get; set; }
    }
}
