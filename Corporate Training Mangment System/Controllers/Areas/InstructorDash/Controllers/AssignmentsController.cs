using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class AssignmentsController : ControllerBase
    {

        private readonly IRepository<Assignment> _assignmentRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Instructor> _instructorRepo;
        private readonly IRepository<Lesson> _lessonRepo;
        public AssignmentsController(
            IRepository<Assignment> assignmentRepo,
            IRepository<Course> courseRepo,
            IRepository<Instructor> instructorRepo,
            IRepository<Lesson>lessonRepo)
        {
            _assignmentRepo = assignmentRepo;
            _courseRepo = courseRepo;
            _instructorRepo = instructorRepo;
            _lessonRepo=lessonRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var assignments = await _assignmentRepo.GetAsync(
                a => a.InstructorId == instructor.Id,
                includes: [a => a.Course]);

            return Ok(assignments.Adapt<IEnumerable<AssignmentResponse>>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var assignment = (await _assignmentRepo.GetAsync(
                a => a.Id == id && a.InstructorId == instructor.Id)).FirstOrDefault();

            if (assignment == null)
                return NotFound("Assignment not found or not yours.");

            return Ok(assignment.Adapt<AssignmentResponse>());
        }
        //filter assignment by course id
        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            // Ensure the course belongs to the instructor
            var course = _courseRepo.GetOne(c => c.Id == courseId && c.InstructorId == instructor.Id);
            if (course is null) return Forbid("You do not have access to this course.");

            // Fetch assignments
            var assignments = await _assignmentRepo.GetAsync(
                a => a.CourseId == courseId && a.InstructorId == instructor.Id);

            return Ok(assignments.Adapt<IEnumerable<AssignmentResponse>>());
        }
        //filter assignment by lesson id
        [HttpGet("by-lesson/{lessonId}")]
        public async Task<IActionResult> GetByLesson(int lessonId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var lesson = (await _lessonRepo.GetAsync(
                l => l.Id == lessonId,
                includes: new Expression<Func<Lesson, object>>[]
                {
            l => l.Chapter,
            l => l.Chapter.Course
                }
            )).FirstOrDefault();

            if (lesson == null || lesson.Chapter.Course.InstructorId != instructor.Id)
                return Forbid("You do not have access to this lesson or it does not exist.");

            var assignments = await _assignmentRepo.GetAsync(
                a => a.LessonId == lessonId && a.InstructorId == instructor.Id);

            return Ok(assignments.Adapt<IEnumerable<AssignmentResponse>>());
        }




        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] AssignmentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var course = _courseRepo.GetOne(c => c.Id == request.CourseId && c.InstructorId == instructor.Id);
            if (course is null) return BadRequest("Course not found or not owned by you.");

            string? fileUrl = null;
            if (request.File is not null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var ext = Path.GetExtension(request.File.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Allowed: .pdf, .doc, .docx, .xls, .xlsx");

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assignments");
                Directory.CreateDirectory(folderPath);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(stream);

                fileUrl = fileName;
            }

            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                Deadline = request.Deadline,
                FileUrl = fileUrl,
                CourseId = request.CourseId,
                LessonId = request.LessonId, // NEW
                InstructorId = instructor.Id
            };

            var created = await _assignmentRepo.CreateAsync(assignment);
            return CreatedAtAction(nameof(GetOne), new { id = created.Id }, created.Adapt<AssignmentResponse>());
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(int id, [FromForm] AssignmentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var assignment = _assignmentRepo.GetOne(a => a.Id == id && a.InstructorId == instructor.Id);
            if (assignment is null) return NotFound();

            assignment.Title = request.Title;
            assignment.Description = request.Description;
            assignment.Deadline = request.Deadline;
            assignment.LessonId = request.LessonId; 

            if (request.File is not null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var ext = Path.GetExtension(request.File.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Invalid file type.");

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assignments");
                Directory.CreateDirectory(folderPath);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(stream);

                assignment.FileUrl = fileName;
            }

            var updated = await _assignmentRepo.EditAsync(assignment);
            return Ok(updated.Adapt<AssignmentResponse>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return Unauthorized();

            var assignment = _assignmentRepo.GetOne(a => a.Id == id && a.InstructorId == instructor.Id);
            if (assignment is null) return NotFound();

            await _assignmentRepo.DeleteAsync(assignment);
            return NoContent();
        }
    }
}
