using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeProgressResponse
    {
        public string EmployeeId { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public double LessonProgress { get; set; }
        public double AssignmentProgress { get; set; }
        public double ExamProgress { get; set; }
        public double TotalProgress { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
