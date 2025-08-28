using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeAssignmentSubmissionResponse
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FileUrl { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
