using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ExamCreateRequest
    {
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public int ChapterId { get; set; }

        public List<QuestionCreateRequest> Questions { get; set; } = new();
    }
}
