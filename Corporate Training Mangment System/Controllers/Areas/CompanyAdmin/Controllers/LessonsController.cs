using DataAccess.IRepository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly IRepository<Lesson> _lessonRepository;

        public LessonsController(IRepository<Lesson> lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonResponse>>> GetAll()
        {
            var lessons = await _lessonRepository.GetAsync(includes: [l => l.Chapter]);
            return Ok(lessons.Adapt<IEnumerable<LessonResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var lesson = _lessonRepository.GetOne(
                l => l.Id == id,
                includes: [l => l.Chapter]);

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

    }

    
}
