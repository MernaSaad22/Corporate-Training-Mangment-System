using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxCourses { get; set; }
        public int MaxEmployees { get; set; }

        public ICollection<Company> Companies { get; set; }
    }
}
