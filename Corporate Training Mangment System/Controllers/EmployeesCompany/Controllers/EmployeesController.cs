using System.Security.Claims;
using Corporate_Training_Mangment_System.Controllers.EmployeeCompany;
using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.EmployeesCompany.Controllers
{

    [Area("EmployeesCompany")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    // [Area("EmployeesCompany")]
    //[Route("api/EmployeesCompany")]
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

            var result = courses.Select(e => new EmployeeCourseShortResponse
            {
                CourseId = e.Course.Id,
                Title = e.Course.Title,
                IsCompleted = e.IsCompleted,
                AssignedAt = e.AssignedAt
            });

            return Ok(result);
        }


        //[HttpGet("my-courses")]
        //public async Task<IActionResult> GetMyCourses()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var courses = await _employeeCourseRepo.GetAsync(
        //        includes: [e => e.Course],
        //        expression: e => e.Employee.ApplicationUserId == userId
        //    );

        //    var result = courses.Select(e => new
        //    {
        //        e.Course.Id,
        //        e.Course.Title,

        //    });

        //    return Ok(result);
        //}

        //    [HttpGet("my-course/{id}")]
        //    public async Task<IActionResult> GetMyCourse([FromRoute] int id)
        //    {
        //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if (string.IsNullOrEmpty(userId))
        //            return Unauthorized();

        //        var course =  _employeeCourseRepo.GetOne(
        //    expression: e => e.Id == id && e.Employee.ApplicationUserId == userId,
        //    includes: [e => e.Course]
        //);

        //        if (course is null)
        //            return NotFound();

        //        var result = new
        //        {
        //            course.Course.Id,
        //            course.Course.Title,
        //            course.IsCompleted,

        //        };

        //        return Ok(result);

        //}


        [HttpGet("my-course/{id}")]
        public async Task<IActionResult> GetMyCourse([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var course =  _employeeCourseRepo.GetOne(
                expression: e => e.Course.Id == id && e.Employee.ApplicationUserId == userId,
                includes: [
                    e => e.Course,
            e => e.Course.Instructor.ApplicationUser,
            e => e.Course.Category,
            e => e.Course.Company,
            e => e.Course.Chapters
                ]
            );

            if (course is null)
                return NotFound();

            var result = new EmployeeCourseDetailsResponse
            {
                CourseId = course.Course.Id,
                Title = course.Course.Title,
                InstructorName = course.Course.Instructor?.ApplicationUser?.UserName ,
               
                CategoryName = course.Course.Category?.Name ,
                AssignedAt = course.AssignedAt,
                CompletedAt = course.CompletedAt,
                IsCompleted = course.IsCompleted,
                ChapterTitles = course.Course.Chapters?.Select(c => c.Title).ToList()
            };

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

            var result = new EmployeeCourseDtoResponse
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
