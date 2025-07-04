using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class CourseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyId { get; set; } = null!;
        public string InstructorId { get; set; } = null!;
        public int CategoryId { get; set; }
    }
}
