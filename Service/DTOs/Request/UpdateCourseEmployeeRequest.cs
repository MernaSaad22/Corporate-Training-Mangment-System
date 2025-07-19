using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class UpdateCourseEmployeeRequest
    {
        public bool IsCompleted { get; set; }
        public int DeadlineDays { get; set; }
    }
}
