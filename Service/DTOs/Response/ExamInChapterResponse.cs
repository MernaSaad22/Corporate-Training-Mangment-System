using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class ExamInChapterResponse
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public int TotalQuestions { get; set; }




    }
}
