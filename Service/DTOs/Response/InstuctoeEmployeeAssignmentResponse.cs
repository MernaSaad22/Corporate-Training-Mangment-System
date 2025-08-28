using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class InstuctoeEmployeeAssignmentResponse
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime AssignedAt { get; set; }
       // public DateTime? CompletedAt { get; set; }
        public DateTime ?AssignmentDeadline { get; set; }
        public bool IsCompleted { get; set; }
    }
}
