using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class QuestionAnswerRequest
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }
}
