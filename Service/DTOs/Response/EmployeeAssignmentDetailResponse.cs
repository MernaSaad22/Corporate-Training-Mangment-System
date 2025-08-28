using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeAssignmentDetailResponse
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? Deadline { get; set; }  
    }
}
