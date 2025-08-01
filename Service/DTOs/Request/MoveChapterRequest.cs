using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class MoveChapterRequest
    {
        public int ChapterId { get; set; }
        public int CourseId { get; set; }
        public int NewOrder { get; set; }
    }
}
