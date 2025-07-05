using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class ChapterResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }
}
