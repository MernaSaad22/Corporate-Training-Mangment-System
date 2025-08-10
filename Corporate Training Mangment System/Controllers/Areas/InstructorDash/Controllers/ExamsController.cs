using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User ID not found in token.");

        //    var exam = _examRepository.GetOne(
        //        expression: e => e.Id == id && e.Chapter.Course.Instructor.ApplicationUserId == userId,
        //        includes: [e => e.Chapter, e => e.Questions, e => e.Chapter.Course]);

        //    if (exam is null)
        //        return NotFound();

        //    var response = new ExamResponse
        //    {
        //        Id = exam.Id,
        //        ChapterId = exam.ChapterId,
        //        ChapterTitle = exam.Chapter?.Title ?? "",
        //        TotalQuestions = exam.Questions?.Count ?? 0
        //    };

        //    return Ok(response);
        //}



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

            var response = new ExamInstructorResponse
            {
                Id = exam.Id,
                Title = exam.Title,
                Deadline = exam.Deadline,
                ChapterId = exam.ChapterId,
             
                Questions = exam.Questions.Select(q => new QuestionInstructorResponse
                {
                    Id = q.Id,
                    Text = q.Text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    Answer = q.Answer
                }).ToList()
            };

            return Ok(response);
        }



        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] ExamRequest request)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User ID not found in token.");

        //    var chapter = _chapterRepository.GetOne(
        //        expression: c => c.Id == request.ChapterId && c.Course.Instructor.ApplicationUserId == userId,
        //        includes: [c => c.Course]);

        //    if (chapter is null)
        //        return NotFound("Chapter not found or you're not authorized to create an exam in it.");

        //    var exam = request.Adapt<Exam>();
        //    var created = await _examRepository.CreateAsync(exam);
        //    if (created is null)
        //        return BadRequest("Failed to create exam.");

        //    return Created($"{Request.Scheme}://{Request.Host}/api/InstructorDash/Exams/{created.Id}",
        //        new ExamResponse
        //        {
        //            Id = created.Id,
        //            ChapterId = created.ChapterId,
        //            ChapterTitle = chapter.Title,
        //            TotalQuestions = 0
        //        });
        //}




        [HttpPost]
       
        public async Task<IActionResult> Create([FromBody] ExamCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var chapter = _chapterRepository.GetOne(
    c => c.Id == request.ChapterId
         && c.Course != null
         && c.Course.Instructor != null
         && c.Course.Instructor.ApplicationUserId == userId,
    includes: new Expression<Func<Chapter, object>>[] { c => c.Course, c => c.Course.Instructor });

            if (chapter == null)
                return Forbid(); // Avoid passing string to Forbid (not valid)

            if (chapter.Exam != null)
                return BadRequest("This chapter already has an exam.");

            var exam = new Exam
            {
                Title = request.Title,
                Deadline = request.Deadline,
                ChapterId = request.ChapterId,
                Questions = request.Questions?.Select(q => new Question
                {
                    Text = q.Text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    Answer = q.Answer
                }).ToList() ?? new List<Question>() // avoid null list
            };

            var created = await _examRepository.CreateAsync(exam);

            // Optional: Reload the created exam with Questions if not auto-included
            created = _examRepository.GetOne(e => e.Id == created.Id, includes: [e => e.Questions]);

            var response = new ExamInstructorResponse
            {
                Id = created.Id,
                Title = created.Title,
                Deadline = created.Deadline,
                ChapterId = created.ChapterId,
                Questions = created.Questions?.Select(q => new QuestionInstructorResponse
                {
                    Id = q.Id,
                    Text = q.Text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    Answer = q.Answer
                }).ToList() ?? new List<QuestionInstructorResponse>()
            };

            return CreatedAtAction(nameof(GetOne), new { id = response.Id }, response);
        }

// edit to write more questions not to delete questions and replace it
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ExamUpdateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var exam = _examRepository.GetOne(
                e => e.Id == id && e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Chapter, e => e.Chapter.Course]);

            if (exam == null)
                return NotFound("Exam not found or you don't have access.");

            // Update fields
            exam.Title = request.Title;
            exam.Deadline = request.Deadline;

            // You might want to delete old questions and replace them
            exam.Questions = request.Questions.Select(q => new Question
            {
                Text = q.Text,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                Answer = q.Answer
            }).ToList();

            await _examRepository.EditAsync(exam);


            return Ok("Exam updated successfully.");
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
        //edit specific question
        [HttpPatch("{examId}/questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int examId, int questionId, [FromBody] QuestionUpdateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var exam = _examRepository.GetOne(
                e => e.Id == examId && e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Questions, e => e.Chapter, e => e.Chapter.Course]
            );

            if (exam == null)
                return NotFound("Exam not found or unauthorized.");

            var question = exam.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                return NotFound("Question not found in this exam.");

          
            question.Text = request.Text;
            question.OptionA = request.OptionA;
            question.OptionB = request.OptionB;
            question.OptionC = request.OptionC;
            question.OptionD = request.OptionD;
            question.Answer = request.Answer;

            await _examRepository.EditAsync(exam); 

            return Ok("Question updated successfully.");
        }
        //delete specific question
        [HttpDelete("{examId}/questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int examId, int questionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var exam = _examRepository.GetOne(
                e => e.Id == examId && e.Chapter.Course.Instructor.ApplicationUserId == userId,
                includes: [e => e.Questions, e => e.Chapter, e => e.Chapter.Course]
            );

            if (exam == null)
                return NotFound("Exam not found or unauthorized.");

            var question = exam.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                return NotFound("Question not found in this exam.");

            exam.Questions.Remove(question); 

            await _examRepository.EditAsync(exam); 

            return Ok("Question deleted successfully.");
        }

    }
}
