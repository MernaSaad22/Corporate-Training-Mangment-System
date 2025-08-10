using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class QuestionUpdateRequest
    {
        public string Text { get; set; } = null!;
        public string OptionA { get; set; } = null!;
        public string OptionB { get; set; } = null!;
        public string OptionC { get; set; } = null!;
        public string OptionD { get; set; } = null!;
        public string Answer { get; set; } = null!;
    }
}
