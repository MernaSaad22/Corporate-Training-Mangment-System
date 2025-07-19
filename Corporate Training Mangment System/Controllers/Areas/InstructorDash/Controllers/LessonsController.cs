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

    public class LessonsController : ControllerBase
    {
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IChapterRepository _chapterRepository;

        public LessonsController(IRepository<Lesson> lessonRepository,IRepository<Course> courseRepository,
            ICompanyRepository companyRepository,IChapterRepository chapterRepository)
        {
            _lessonRepository = lessonRepository;
            this._courseRepository = courseRepository;
            this._companyRepository = companyRepository;
            this._chapterRepository = chapterRepository;
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<ActionResult<IEnumerable<LessonResponse>>> GetLessonsByCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var course = (await _courseRepository.GetAsync(
                c => c.Id == courseId && c.Instructor.ApplicationUserId == userId,
                includes: [c => c.Instructor])).FirstOrDefault();

            if (course is null)
                return NotFound("Course not found or not owned by you.");

            var lessons = await _lessonRepository.GetAsync(
                l => l.Chapter.CourseId == courseId,
                includes: [l => l.Chapter]);

            return Ok(lessons.Adapt<IEnumerable<LessonResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lesson = _lessonRepository.GetOne(
                l => l.Id == id && l.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [l => l.Chapter, l => l.Chapter.Course, l => l.Chapter.Course.Instructor]);

            if (lesson is null) return NotFound();

            return Ok(lesson.Adapt<LessonResponse>());
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<ActionResult<IEnumerable<LessonResponse>>> GetByChapter(int chapterId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var chapter = _chapterRepository.GetOne(
                c => c.Id == chapterId && c.Course.Instructor.ApplicationUserId == userId,
                includes: [c => c.Course]);

            if (chapter is null)
                return NotFound("Chapter not found or not owned by you.");

            var lessons = await _lessonRepository.GetAsync(
                l => l.ChapterId == chapterId,
                includes: [l => l.Chapter]);

            return Ok(lessons.Adapt<IEnumerable<LessonResponse>>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LessonRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var chapter = _chapterRepository.GetOne(
                c => c.Id == request.ChapterId && c.Course.Instructor.ApplicationUserId == userId,
                includes: [c => c.Course]);

            if (chapter is null)
                return NotFound("Chapter not found or unauthorized.");

            var lesson = request.Adapt<Lesson>();
            var created = await _lessonRepository.CreateAsync(lesson);

            if (created is null)
                return BadRequest("Failed to create lesson.");

            return Created($"{Request.Scheme}://{Request.Host}/api/InstructorDash/Lessons/{created.Id}",
                created.Adapt<LessonResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] LessonRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lesson = _lessonRepository.GetOne(
                l => l.Id == id && l.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [l => l.Chapter.Course]);

            if (lesson is null)
                return NotFound("Lesson not found or unauthorized.");

            // Verify new Chapter belongs to the same instructor
            if (request.ChapterId != lesson.ChapterId)
            {
                var newChapter = _chapterRepository.GetOne(
                    c => c.Id == request.ChapterId && c.Course.Instructor.ApplicationUserId == userId,
                    includes: [c => c.Course]);

                if (newChapter is null)
                    return BadRequest("Invalid chapter.");
            }

            request.Adapt(lesson);
            lesson.Id = id;

            var updated = await _lessonRepository.EditAsync(lesson);
            if (updated is null)
                return BadRequest("Failed to update lesson.");

            return Ok(updated.Adapt<LessonResponse>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lesson = _lessonRepository.GetOne(
                l => l.Id == id && l.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [l => l.Chapter.Course]);

            if (lesson is null)
                return NotFound("Lesson not found or unauthorized.");

            var deleted = await _lessonRepository.DeleteAsync(lesson);
            if (deleted is null)
                return BadRequest("Failed to delete lesson.");

            return NoContent();
        }

        [HttpPost("{lessonId}/video")]
        public async Task<IActionResult> UploadLessonVideo(int lessonId, IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Get Instructor  from token
            var instructorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized();

            // check lesson if it belongs to that instructor
            var lesson = _lessonRepository.GetOne(
                l => l.Id == lessonId && l.Chapter.Course.Instructor.ApplicationUserId == instructorId,
                new Expression<Func<Lesson, object>>[]
                {
                     l => l.Chapter,
                     l => l.Chapter.Course
                }
            );

            if (lesson == null)
                return NotFound("Lesson not found or access denied.");

            // Prepare the directory to save the video
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "lessonVideos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(videoFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            var fullVideoUrl = $"{Request.Scheme}://{Request.Host}/lessonVideos/{fileName}";
            lesson.VideoUrl = fullVideoUrl;
            await _lessonRepository.EditAsync(lesson);

            return Ok(new { videoUrl = fullVideoUrl });
        }



    }


}
