﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }

        public int ExamId { get; set; }
    }
}
