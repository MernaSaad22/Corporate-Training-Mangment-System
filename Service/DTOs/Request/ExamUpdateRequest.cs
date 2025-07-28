using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ExamUpdateRequest
    {
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public List<QuestionCreateRequest>? Questions { get; set; }
    }
}
