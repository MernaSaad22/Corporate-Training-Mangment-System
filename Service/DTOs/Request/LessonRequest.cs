using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class LessonRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int ChapterId { get; set; }
        public IFormFile? VideoFile { get; set; }

    }
}
