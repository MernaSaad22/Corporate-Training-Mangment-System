using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class LessonReorderRequest
    {
        public int ChapterId { get; set; }
        public List<int> OrderedLessonIds { get; set; } = new();
    }
}
