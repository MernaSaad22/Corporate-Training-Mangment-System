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
    public class LessonRepository:Repository<Lesson>,ILessonRepository
    {
        public readonly ApplicationDbContext _context;
        public LessonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
