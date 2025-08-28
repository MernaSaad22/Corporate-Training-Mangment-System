using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeAssignmentSubmission
    {
        public int Id { get; set; }

        public string FileUrl { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;


        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }   

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public decimal? Grade { get; set; }

    }
}
