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
    public class ExamSubmissionRepository : Repository<ExamSubmission>, IExamSubmissionRepository
    {
        private readonly ApplicationDbContext _context;
        public ExamSubmissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
