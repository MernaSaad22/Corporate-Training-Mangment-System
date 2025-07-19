using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ExamRequest
    {
        public string Title { get; set; }
        public int ChapterId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
