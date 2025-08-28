using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ExamSubmissionRequest
    {

        public List<QuestionAnswerRequest> Answers { get; set; }
    }
}
