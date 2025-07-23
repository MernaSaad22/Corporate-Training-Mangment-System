using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class AssignmentRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public DateTime? Deadline { get; set; }
        public IFormFile? File { get; set; }

        public int LessonId { get; set; }
    }
}
