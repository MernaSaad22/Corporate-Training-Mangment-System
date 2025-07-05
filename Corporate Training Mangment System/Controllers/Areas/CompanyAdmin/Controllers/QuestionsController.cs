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
    public class QuestionsController : ControllerBase
    {
        private readonly IRepository<Question> _questionRepository;

        public QuestionsController(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetAll()
        {
            var questions = await _questionRepository.GetAsync();
            return Ok(questions.Adapt<IEnumerable<QuestionResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var question = _questionRepository.GetOne(q => q.Id == id);
            if (question is null) return NotFound();

            return Ok(question.Adapt<QuestionResponse>());
        }

        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetByExam(int examId)
        {
            var questions = await _questionRepository.GetAsync(q => q.ExamId == examId);
            return Ok(questions.Adapt<IEnumerable<QuestionResponse>>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuestionRequest request)
        {
            var question = request.Adapt<Question>();

            var created = await _questionRepository.CreateAsync(question);
            if (created is null)
                return BadRequest("Failed to create question.");

            return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Questions/{created.Id}",
                created.Adapt<QuestionResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] QuestionRequest request)
        {
            var existing = _questionRepository.GetOne(q => q.Id == id);
            if (existing is null)
                return NotFound();

            request.Adapt(existing);

            var updated = await _questionRepository.EditAsync(existing);
            if (updated is null)
                return BadRequest("Failed to update question.");

            return Ok(updated.Adapt<QuestionResponse>());
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var question = _questionRepository.GetOne(q => q.Id == id);
            if (question is null) return NotFound();

            var deleted = await _questionRepository.DeleteAsync(question);
            if (deleted is null)
                return BadRequest("Failed to delete question.");

            return NoContent();
        }



    }
}
