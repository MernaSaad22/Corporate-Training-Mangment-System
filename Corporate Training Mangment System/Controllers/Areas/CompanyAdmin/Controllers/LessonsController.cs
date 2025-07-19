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

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]

    public class LessonsController : ControllerBase
    {
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICompanyRepository _companyRepository;

        public LessonsController(IRepository<Lesson> lessonRepository,ICourseRepository courseRepository,ICompanyRepository companyRepository)
        {
            _lessonRepository = lessonRepository;
            this._courseRepository = courseRepository;
            this._companyRepository = companyRepository;
        }

       
        [HttpGet("by-course/{courseId}")]
        public async Task<ActionResult<IEnumerable<LessonResponse>>> GetLessonsByCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            // Course ownership check
            var course = (await _courseRepository.GetAsync(c => c.Id == courseId && c.CompanyId == company.Id)).FirstOrDefault();
            if (course is null) return NotFound("Course not found or not owned by you.");

            var lessons = await _lessonRepository.GetAsync(
                expression: l => l.Chapter.CourseId == courseId,
                includes: [l => l.Chapter]);

            return Ok(lessons.Adapt<IEnumerable<LessonResponse>>());
        }



        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lesson = _lessonRepository.GetOne(
                l => l.Id == id &&
                     l.Chapter.Course.Company.ApplicationUserId == userId,
                includes: [l => l.Chapter, l => l.Chapter.Course, l => l.Chapter.Course.Company, l => l.Chapter.Course.Company.ApplicationUser]);

            if (lesson is null) return NotFound();

            return Ok(lesson.Adapt<LessonResponse>());
        }


        [HttpGet("chapter/{chapterId}")]
            public async Task<ActionResult<IEnumerable<LessonResponse>>> GetByChapter(int chapterId)
            {
                var lessons = await _lessonRepository.GetAsync(
                    expression: l => l.ChapterId == chapterId,
                    includes: [l => l.Chapter]);

                return Ok(lessons.Adapt<IEnumerable<LessonResponse>>());
            }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LessonRequest request)
        {
            var lesson = request.Adapt<Lesson>();

            var created = await _lessonRepository.CreateAsync(lesson);
            if (created is null)
                return BadRequest("Failed to create lesson.");

            return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Lessons/{created.Id}",
                created.Adapt<LessonResponse>());
        }

       

            [HttpPut("{id}")]
            public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] LessonRequest request)
            {
                var existing = _lessonRepository.GetOne(l => l.Id == id);
                if (existing is null)
                    return NotFound();

                request.Adapt(existing);

                var updated = await _lessonRepository.EditAsync(existing);
                if (updated is null)
                    return BadRequest("Failed to update lesson.");

                return Ok(updated.Adapt<LessonResponse>());
            }


            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete([FromRoute] int id)
            {
                var lesson = _lessonRepository.GetOne(l => l.Id == id);
                if (lesson is null)
                    return NotFound();

                var deleted = await _lessonRepository.DeleteAsync(lesson);
                if (deleted is null)
                    return BadRequest("Failed to delete lesson.");

                return NoContent();
            }

        //[HttpPost("{lessonId}/upload-video")]
        //public async Task<IActionResult> UploadVideo(int lessonId, IFormFile video)
        //{
        //    var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        //    if (lesson is null)
        //        return NotFound("Lesson not found.");

        //    if (video is null || video.Length == 0)
        //        return BadRequest("Invalid video file.");

        //    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "lessons");
        //    Directory.CreateDirectory(uploadsFolder);

        //    var fileName = $"{Guid.NewGuid()}_{video.FileName}";
        //    var filePath = Path.Combine(uploadsFolder, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await video.CopyToAsync(stream);
        //    }

        //    lesson.VideoFileName = video.FileName;
        //    lesson.VideoPath = $"/uploads/lessons/{fileName}";
        //    lesson.VideoSizeInBytes = video.Length;
        //    await _lessonRepository.UpdateAsync(lesson);

        //    return Ok(new { videoUrl = lesson.VideoPath });
        //}



    }


}
