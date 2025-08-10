using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Request
{
    public class ReorderChaptersRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public List<int> OrderedChapterIds { get; set; } = [];
    }
}
