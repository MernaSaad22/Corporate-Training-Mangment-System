using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utility.Progress
{
    public interface IProgressService
    {
        Task UpdateEmployeeCourseProgress(string employeeId, int courseId);
    }
}
