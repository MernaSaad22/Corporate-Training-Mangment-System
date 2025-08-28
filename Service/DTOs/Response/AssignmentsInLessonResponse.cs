using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class AssignmentsInLessonResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? Deadline { get; set; }
        public string LessonTitle { get; set; }

    }
}
