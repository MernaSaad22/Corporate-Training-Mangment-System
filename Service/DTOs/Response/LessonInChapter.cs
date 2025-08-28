using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class LessonInChapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

      
        public string ChapterTitle { get; set; }
        public string? VideoUrl { get; set; }

        public int Order { get; set; }
    }
}
