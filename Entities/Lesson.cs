using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Order { get; set; }
        public string? VideoFileName { get; set; }
        public string? VideoUrl { get; set; }
        public TimeSpan? Duration { get; set; }


    }
}
