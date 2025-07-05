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
    public class ChaptersController : ControllerBase
    {
        private readonly IRepository<Chapter> _chapterRepository;

        public ChaptersController(IRepository<Chapter> chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChapterResponse>>> GetAll()
        {
            var chapters = await _chapterRepository.GetAsync(
                includes: [c => c.Course]);

            return Ok(chapters.Adapt<IEnumerable<ChapterResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var chapter = _chapterRepository.GetOne(
                c => c.Id == id,
                includes: [c => c.Course]);

            if (chapter is null) return NotFound();

            return Ok(chapter.Adapt<ChapterResponse>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChapterRequest request)
        {
            var chapter = request.Adapt<Chapter>();

            var newChapter = await _chapterRepository.CreateAsync(chapter);
            if (newChapter is null)
                return BadRequest("Failed to create chapter.");

            return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Chapters/{newChapter.Id}",
                newChapter.Adapt<ChapterResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ChapterRequest request)
        {
            var existingChapter = _chapterRepository.GetOne(c => c.Id == id);
            if (existingChapter is null)
                return NotFound();

            request.Adapt(existingChapter);

            var updated = await _chapterRepository.EditAsync(existingChapter);
            if (updated is null)
                return BadRequest("Failed to update chapter.");

            return Ok(updated.Adapt<ChapterResponse>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var chapter = _chapterRepository.GetOne(c => c.Id == id);
            if (chapter is null) return NotFound();

            var deleted = await _chapterRepository.DeleteAsync(chapter);
            if (deleted is null)
                return BadRequest("Failed to delete chapter.");

            return NoContent();
        }


    }
}
