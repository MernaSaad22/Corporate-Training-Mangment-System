using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ExamSubmission
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime SubmittedAt { get; set; }

        public List<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

        public decimal? Grade { get; set; } 
    }
}
