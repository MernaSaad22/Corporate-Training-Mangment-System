using DataAccess.Data;
using DataAccess.IRepository;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class EmployeeCourseRepository : Repository<EmployeeCourse>, IEmployeeCourseRepository
    {
        public readonly ApplicationDbContext _context;
        public EmployeeCourseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
