using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class QuestionAnswer
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int ExamSubmissionId { get; set; }
        public ExamSubmission ExamSubmission { get; set; }

        public string Answer { get; set; }
    }
}
