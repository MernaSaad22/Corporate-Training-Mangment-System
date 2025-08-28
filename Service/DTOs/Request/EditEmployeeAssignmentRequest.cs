using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class EditEmployeeAssignmentRequest
    {
        public string EmployeeId { get; set; }  
        public bool ?IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
