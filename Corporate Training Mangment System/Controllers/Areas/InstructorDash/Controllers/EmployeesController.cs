using DataAccess.IRepository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Response;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<EmployeeCourse> _employeeCourseRepo;
        private readonly IRepository<Instructor> _instructorRepo;

        public EmployeesController(
            IRepository<EmployeeCourse> employeeCourseRepo,
            IRepository<Instructor> instructorRepo)
        {
            _employeeCourseRepo = employeeCourseRepo;
            _instructorRepo = instructorRepo;
        }

        [HttpGet("enrolled")]
        public async Task<IActionResult> GetEnrolledEmployees()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var employeeCourses = await _employeeCourseRepo.GetAsync(
                ec => ec.Course.InstructorId == instructor.Id,
                includes: new Expression<Func<EmployeeCourse, object>>[]
                {
            ec => ec.Employee,
            ec => ec.Employee.ApplicationUser,
            ec => ec.Course
                });

            var employees = employeeCourses
                .Select(ec => ec.Employee)
                .Distinct()
                .Select(e => new EmployeeInstructorResponse
                {
                    Id = e.Id,
                    FullName = e.ApplicationUser.UserName,
                    Email = e.ApplicationUser.Email
                });

            return Ok(employees);
        }

        [HttpGet("enrolled/{id}")]
        public async Task<IActionResult> GetEnrolledEmployee(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            // Make sure the employee is enrolled in a course that belongs to the instructor
            var employeeCourse = (await _employeeCourseRepo.GetAsync(
                ec => ec.EmployeeId == id && ec.Course.InstructorId == instructor.Id,
                includes: new Expression<Func<EmployeeCourse, object>>[]
                {
            ec => ec.Employee,
            ec => ec.Employee.ApplicationUser
                }
            )).FirstOrDefault();

            if (employeeCourse == null)
                return NotFound("Employee not found or not enrolled in your courses.");

            var employee = new EmployeeInstructorResponse
            {
                Id = employeeCourse.Employee.Id,
                FullName = employeeCourse.Employee.ApplicationUser.UserName,
                Email = employeeCourse.Employee.ApplicationUser.Email
            };

            return Ok(employee);
        }


    }

}

