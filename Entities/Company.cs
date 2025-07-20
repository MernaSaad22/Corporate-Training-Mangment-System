using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; }
        public string? Logo { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        public string ApplicationUserId { get; set; }//owner
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
