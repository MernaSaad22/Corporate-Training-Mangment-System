using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class CourseProgressesController : ControllerBase
    {
        private readonly IRepository<EmployeeCourseProgress> _courseProgressRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Instructor> _instructorRepo;

        public CourseProgressesController(
            IRepository<EmployeeCourseProgress> courseProgressRepo,
            IRepository<Course> courseRepo,
            IRepository<Instructor> instructorRepo)
        {
            _courseProgressRepo = courseProgressRepo;
            _courseRepo = courseRepo;
            _instructorRepo = instructorRepo;
        }

       



        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseProgress([FromRoute] int courseId)
        {
            var userAppId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userAppId);
            if (instructor is null)
                return Unauthorized("Instructor not found.");

            
            var course = _courseRepo.GetOne(c => c.Id == courseId);

            if (course == null || course.InstructorId != instructor.Id)
                return Unauthorized("You do not have access to this course.");

            var progressList = await _courseProgressRepo.GetAsync(
    p => p.CourseId == courseId,
    includes: [p => p.Employee, p => p.Employee.ApplicationUser]
);

            var result = progressList.Select(p => new
            {
                EmployeeName = p.Employee.ApplicationUser.UserName ,
                LessonProgress = p.LessonProgress,
                AssignmentProgress = p.AssignmentProgress,
                ExamProgress = p.ExamProgress,
                TotalProgress = Math.Round(p.LessonProgress + p.AssignmentProgress + p.ExamProgress, 2),
                LastUpdated = p.LastUpdated
            });

            return Ok(result);
        }


        [HttpGet("{courseId}/employee_progress/{employeeId}")]
        public IActionResult GetEmployeeProgress(int courseId, string employeeId)
        {
          
            var instructorAppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == instructorAppUserId);
            if (instructor == null)
                return Unauthorized("Instructor not found.");

       
            var course = _courseRepo.GetOne(c => c.Id == courseId);
            if (course == null || course.InstructorId != instructor.Id)
                return Unauthorized("You do not have access to this course.");

            
            var progress =  _courseProgressRepo.GetOne(
                p => p.CourseId == courseId && p.EmployeeId == employeeId,
                includes: [p => p.Employee, p => p.Employee.ApplicationUser]
            );

            if (progress == null)
                return NotFound("No progress found for this employee in this course.");

            var result = new
            {
                EmployeeName = progress.Employee.ApplicationUser.UserName ,
                LessonProgress = progress.LessonProgress,
                AssignmentProgress = progress.AssignmentProgress,
                ExamProgress = progress.ExamProgress,
                TotalProgress = Math.Round(progress.LessonProgress + progress.AssignmentProgress + progress.ExamProgress, 2),
                LastUpdated = progress.LastUpdated
            };

            return Ok(result);
        }


    }
}
