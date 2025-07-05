using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class ExamResponse
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }
        public string ChapterTitle { get; set; }

        public int TotalQuestions { get; set; }
    }
}
