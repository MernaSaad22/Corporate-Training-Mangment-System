using System.Security.Claims;
using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Training_Mangment_System.Controllers.EmployeeCompany
{
    [Area("EmployeesCompany")]
    [Route("api/EmployeesCompany")]
    public class EmployeesController : Controller
    {
        private readonly IRepository<EmployeeCourse> _employeeCourseRepo;

        public EmployeesController(IRepository<EmployeeCourse> employeeCourseRepo)
        {
            _employeeCourseRepo = employeeCourseRepo;
        }

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var courses = await _employeeCourseRepo.GetAsync(
                includes: [e => e.Course],
                expression: e => e.Employee.ApplicationUserId == userId
            );

            var result = courses.Select(e => new
            {
                e.Course.Id,
                e.Course.Title,

            });

            return Ok(result);
        }
        [HttpGet("EmployeeStats/{employeeId}")]
        public async Task<IActionResult> GetEmployeeStats(string employeeId)
        {
            var employeeCourses = await _employeeCourseRepo.GetAsync(
                expression: ec => ec.EmployeeId == employeeId
            );

            var enrolled = employeeCourses.Count();
            var completed = employeeCourses.Count(ec => ec.IsCompleted);
            var points = completed * 10; 

            var result = new EmployeeCourseDto
            {
                CoursesEnrolled = enrolled,
                CoursesCompleted = completed,
                PointsEarned = points
            };

            return Ok(result);
        }
        ///
    }
        
    
}
