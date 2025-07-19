using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]

    public class ChaptersController : ControllerBase
    {
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IInstructorRepository _instructorRepository;

        public ChaptersController(IRepository<Chapter> chapterRepository, IRepository<Course> courseRepository,IRepository<Company> companyRepository,
            IInstructorRepository instructorRepository)
        {
            _chapterRepository = chapterRepository;
            this._courseRepository = courseRepository;
            this._companyRepository = companyRepository;
            this._instructorRepository = instructorRepository;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ChapterResponse>>> GetAll()
        //{
        //    var chapters = await _chapterRepository.GetAsync(
        //        includes: [c => c.Course]);

        //    return Ok(chapters.Adapt<IEnumerable<ChapterResponse>>());
        [HttpGet("{courseId}/chapters")]
        public async Task<ActionResult<IEnumerable<ChapterResponse>>> GetChaptersForCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            // check for instructor
            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == userId);
            if (instructor is null) return NotFound("Instructor not found");


            // Course check
            var course = (await _courseRepository.GetAsync(
                c => c.Id == courseId && c.InstructorId == instructor.Id
                )).FirstOrDefault();
            if (course is null)
                return NotFound("Course not found or does not belong to your courses.");

            var chapters = await _chapterRepository.GetAsync(
                c => c.CourseId == courseId
            );

            return Ok(chapters.Adapt<IEnumerable<ChapterResponse>>());
        }


        [HttpGet("chapters/{id}")]
        public async Task<IActionResult> GetChapterById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return Unauthorized("User not authenticated");

            // Get instructor
            var instructor = _instructorRepository.GetOne(c => c.ApplicationUserId == userId);
            if (instructor is null)
                return NotFound("Instructor not found");

            // Get chapter with course
            var chapter = _chapterRepository.GetOne(
                expression: c => c.Id == id && c.Course.InstructorId == instructor.Id,
                includes: [c => c.Course]
            );

            if (chapter is null)
                return NotFound("Chapter not found or does not belong to your courses.");

            return Ok(chapter.Adapt<ChapterResponse>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChapterRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var instructor = _instructorRepository.GetOne(c => c.ApplicationUserId == userId);
            if (instructor == null)
                return Unauthorized("Instructor not found for this user.");

            // Validate course belongs to instructor
            var course = await _courseRepository.GetAsync(
                expression: c => c.Id == request.CourseId && c.InstructorId == instructor.Id);
            if (!course.Any())
                return BadRequest("Invalid course or unauthorized.");

            var chapter = request.Adapt<Chapter>();
            var newChapter = await _chapterRepository.CreateAsync(chapter);

            return Created(
                $"{Request.Scheme}://{Request.Host}/api/InstructorDash/Chapters/{newChapter.Id}",
                newChapter.Adapt<ChapterResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ChapterRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var instructor = _instructorRepository.GetOne(c => c.ApplicationUserId == userId);
            if (instructor == null)
                return Unauthorized("Instructor not found for this user.");

            var chapter = (await _chapterRepository.GetAsync(
                expression: c => c.Id == id && c.Course.InstructorId == instructor.Id,
                includes: [c => c.Course])).FirstOrDefault();
            if (chapter == null)
                return NotFound("Chapter not found or not authorized.");

            // Verify that the updated course ID also belongs to the instructor
            if (request.CourseId != chapter.CourseId)
            {
                var course = await _courseRepository.GetAsync(
                    expression: c => c.Id == request.CourseId && c.InstructorId == instructor.Id);
                if (!course.Any())
                    return BadRequest("Invalid course update.");
            }

            request.Adapt(chapter);
            chapter.Id = id;

            await _chapterRepository.EditAsync(chapter);
            return Ok(chapter.Adapt<ChapterResponse>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var instructor = _instructorRepository.GetOne(c => c.ApplicationUserId == userId);
            if (instructor == null)
                return Unauthorized("Instructor not found for this user.");

            var chapter = (await _chapterRepository.GetAsync(
                expression: c => c.Id == id && c.Course.InstructorId == instructor.Id,
                includes: [c => c.Course])).FirstOrDefault();

            if (chapter == null)
                return NotFound("Chapter not found or not authorized.");

            await _chapterRepository.DeleteAsync(chapter);
            return NoContent();
        }



    }
}
