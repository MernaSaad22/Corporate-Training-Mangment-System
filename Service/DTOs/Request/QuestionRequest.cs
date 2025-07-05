using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class QuestionRequest
    {
        public string Text { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public int ExamId { get; set; }
    }
}
