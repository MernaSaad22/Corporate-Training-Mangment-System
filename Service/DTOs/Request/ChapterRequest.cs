using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ChapterRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public int CourseId { get; set; }
        [Range(1, int.MaxValue)]
        public int Order { get; set; }
    }
}
