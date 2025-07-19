using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]

    public class ExamsController : ControllerBase
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly IChapterRepository _chapterRepository;

        public ExamsController(IRepository<Exam> examRepository,IChapterRepository chapterRepository)
        {
            _examRepository = examRepository;
            this._chapterRepository = chapterRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamResponse>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var exams = await _examRepository.GetAsync(
                expression: e => e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Chapter, e => e.Questions, e => e.Chapter.Course]);

            var response = exams.Select(e => new ExamResponse
            {
                Id = e.Id,
                ChapterId = e.ChapterId,
                ChapterTitle = e.Chapter.Title,
                TotalQuestions = e.Questions?.Count ?? 0
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var exam = _examRepository.GetOne(
                expression: e => e.Id == id && e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Chapter, e => e.Questions, e => e.Chapter.Course]);

            if (exam is null)
                return NotFound();

            var response = new ExamResponse
            {
                Id = exam.Id,
                ChapterId = exam.ChapterId,
                ChapterTitle = exam.Chapter?.Title ?? "",
                TotalQuestions = exam.Questions?.Count ?? 0
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var chapter = _chapterRepository.GetOne(
                expression: c => c.Id == request.ChapterId && c.Course.Instructor.ApplicationUserId == userId,
                includes: [c => c.Course]);

            if (chapter is null)
                return NotFound("Chapter not found or you're not authorized to create an exam in it.");

            var exam = request.Adapt<Exam>();
            var created = await _examRepository.CreateAsync(exam);
            if (created is null)
                return BadRequest("Failed to create exam.");

            return Created($"{Request.Scheme}://{Request.Host}/api/InstructorDash/Exams/{created.Id}",
                new ExamResponse
                {
                    Id = created.Id,
                    ChapterId = created.ChapterId,
                    ChapterTitle = chapter.Title,
                    TotalQuestions = 0
                });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var exam = _examRepository.GetOne(
                e => e.Id == id && e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Chapter.Course]);

            if (exam is null)
                return NotFound("Exam not found or you're not authorized to delete it.");

            var deleted = await _examRepository.DeleteAsync(exam);
            if (deleted is null)
                return BadRequest("Failed to delete exam.");

            return NoContent();
        }


    }
}
