using DataAccess.IRepository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    //[Area("CompanyAdmin")]
    //[Route("api/[area]/[controller]")]
    //[ApiController]
    //public class ExamsController : ControllerBase
    //{
    //    private readonly IRepository<Exam> _examRepository;

    //    public ExamsController(IRepository<Exam> examRepository)
    //    {
    //        _examRepository = examRepository;
    //    }

    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<ExamResponse>>> GetAll()
    //    {
    //        var exams = await _examRepository.GetAsync(includes: [e => e.Chapter, e => e.Questions]);

    //        var response = exams.Select(e => new ExamResponse
    //        {
    //            Id = e.Id,
    //            ChapterId = e.ChapterId,
    //            ChapterTitle = e.Chapter.Title,
    //            TotalQuestions = e.Questions?.Count ?? 0
    //        });

    //        return Ok(response);
    //    }

    //    [HttpGet("{id}")]
    //    public IActionResult GetOne([FromRoute] int id)
    //    {
    //        var exam = _examRepository.GetOne(
    //            e => e.Id == id,
    //            includes: [e => e.Chapter, e => e.Questions]);

    //        if (exam is null) return NotFound();

    //        var response = new ExamResponse
    //        {
    //            Id = exam.Id,
    //            ChapterId = exam.ChapterId,
    //            ChapterTitle = exam.Chapter?.Title ?? "",
    //            TotalQuestions = exam.Questions?.Count ?? 0
    //        };

    //        return Ok(response);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] ExamRequest request)
    //    {
    //        var exam = request.Adapt<Exam>();

    //        var created = await _examRepository.CreateAsync(exam);
    //        if (created is null)
    //            return BadRequest("Failed to create exam.");

    //        return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Exams/{created.Id}",
    //            new ExamResponse
    //            {
    //                Id = created.Id,
    //                ChapterId = created.ChapterId,
    //                ChapterTitle = "", 
    //                TotalQuestions = 0
    //            });
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete([FromRoute] int id)
    //    {
    //        var exam = _examRepository.GetOne(e => e.Id == id);
    //        if (exam is null)
    //            return NotFound();

    //        var deleted = await _examRepository.DeleteAsync(exam);
    //        if (deleted is null)
    //            return BadRequest("Failed to delete exam.");

    //        return NoContent();
    //    }




    //}
}
