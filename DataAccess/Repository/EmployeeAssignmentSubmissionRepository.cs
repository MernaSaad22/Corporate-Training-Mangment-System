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
    public class EmployeeAssignmentSubmissionRepository:Repository<EmployeeAssignmentSubmission>, IEmployeeAssignmentSubmissionRepository
    {
        public readonly ApplicationDbContext _context;
        public EmployeeAssignmentSubmissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
