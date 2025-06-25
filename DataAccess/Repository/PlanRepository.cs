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
    public class PlanRepository : Repository<Plan>, IPlanRepository
    {
        public readonly ApplicationDbContext _context;
        public PlanRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
