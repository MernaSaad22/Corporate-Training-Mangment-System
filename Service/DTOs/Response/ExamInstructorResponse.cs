using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class ExamInstructorResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public int ChapterId { get; set; }
        public List<QuestionInstructorResponse> Questions { get; set; }
    }
}
