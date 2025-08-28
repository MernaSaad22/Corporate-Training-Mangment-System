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
    public class EmployeeLessonProgressesRepository : Repository<EmployeeLessonProgress>, IEmployeeLessonProgressesRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeLessonProgressesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


    }
}
