using DataAccess.IRepository;
using DataAccess.Repository;
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
    public class CoursesController : ControllerBase
    {
        private readonly IRepository<Instructor> _instructorRepository;
        private readonly IRepository<Course> _courseRepository;

        public CoursesController(IRepository<Course> courseRepository, IRepository<Instructor> instructorRepository)
        {
            _courseRepository = courseRepository;
            this._instructorRepository = instructorRepository;
           
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepository.GetOne(c => c.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var courses = await _courseRepository.GetAsync(
       c => c.InstructorId == instructor.Id);


            return Ok(courses.Adapt<IEnumerable<CourseInstructorResponse>>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseInstructorResponse>> GetOne(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            
            var course = (await _courseRepository.GetAsync(
                c => c.Id == id && c.InstructorId == instructor.Id
            )).FirstOrDefault();

            if (course is null)
                return NotFound("Course not found or does not belong to this instructor.");

            return Ok(course.Adapt<CourseInstructorResponse>());
        }

    }
}
