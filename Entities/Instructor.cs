using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Instructor
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Specialization { get; set; }

        public string? CompanyId { get; set; } 
        public Company ?Company { get; set; } 
    }
}
