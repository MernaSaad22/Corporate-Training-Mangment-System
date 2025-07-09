using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CourseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //To make each company can edit its contant==>for add migration because the previous CourseCategory without this coloum
        public string? CompanyId { get; set; }
        public Company? Company { get; set; }
        //public string CompanyId { get; set; } = null!;
        //public Company Company { get; set; }=null!;
        public ICollection<Course> Courses { get; set; }
    }
}
