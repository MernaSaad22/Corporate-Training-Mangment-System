using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class InstructorRequest
    {
        public string ApplicationUserId { get; set; } = null!;
        public string Specialization { get; set; } = null!;
    }
}
