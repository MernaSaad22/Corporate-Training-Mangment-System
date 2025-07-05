using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ChapterRequest
    {
        public string Title { get; set; } = null!;
        public int CourseId { get; set; }
    }
}
