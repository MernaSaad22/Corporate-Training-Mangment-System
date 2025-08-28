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
    //[Area("CompanyAdmin")]
    //[Route("api/[area]/[controller]")]
    //[ApiController]
    //[Authorize(Roles = "CompanyAdmin")]

    //public class ChaptersController : ControllerBase
    //{
    //    private readonly IRepository<Chapter> _chapterRepository;
    //    private readonly IRepository<Course> _courseRepository;
    //    private readonly IRepository<Company> _companyRepository;

    //    public ChaptersController(IRepository<Chapter> chapterRepository, IRepository<Course> courseRepository,IRepository<Company> companyRepository)
    //    {
    //        _chapterRepository = chapterRepository;
    //        this._courseRepository = courseRepository;
    //        this._companyRepository = companyRepository;
    //    }

    //    //[HttpGet]
    //    //public async Task<ActionResult<IEnumerable<ChapterResponse>>> GetAll()
    //    //{
    //    //    var chapters = await _chapterRepository.GetAsync(
    //    //        includes: [c => c.Course]);

    //    //    return Ok(chapters.Adapt<IEnumerable<ChapterResponse>>());
    //    [HttpGet("{courseId}/chapters")]
    //    public async Task<ActionResult<IEnumerable<ChapterResponse>>> GetChaptersForCourse(int courseId)
    //    {
    //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        //Console.WriteLine($"Token UserId: {userId}");

    //        if (userId is null) return Unauthorized();

    //        // check for company
    //        var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
    //        //Console.WriteLine($"userId: {userId}, company: {(company != null ? company.Name : "null")}");
    //        if (company is null) return NotFound();

    //        // Course check
    //        var course = (await _courseRepository.GetAsync(
    //            c => c.Id == courseId && c.CompanyId == company.Id
    //            )).FirstOrDefault();


    //        if (course is null)
    //            return NotFound("Course not found or does not belong to your company");

    //        var chapters = await _chapterRepository.GetAsync(
    //            c => c.CourseId == courseId
    //        );

    //        return Ok(chapters.Adapt<IEnumerable<ChapterResponse>>());
    //    }



    //    ////[HttpGet("{id}")]
    //    ////public IActionResult GetOne([FromRoute] int id)
    //    ////{
    //    ////    var chapter = _chapterRepository.GetOne(
    //    ////        c => c.Id == id,
    //    ////        includes: [c => c.Course]);

    //    ////    if (chapter is null) return NotFound();

    //    ////    return Ok(chapter.Adapt<ChapterResponse>());
    //    ////}
       
    //    [Authorize]
    //    [HttpGet("chapters/{id}")]
    //    public async Task<IActionResult> GetChapterById(int id)
    //    {
    //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        //Console.WriteLine($"Token UserId: {userId}");

    //        if (userId is null)
    //            return Unauthorized("User not authenticated");

    //        // Get company
    //        var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
    //        //Console.WriteLine($"userId: {userId}, company: {(company != null ? company.Name : "null")}");
    //        if (company is null)
    //            return NotFound("Company not found");

    //        // Get chapter with course
    //        var chapter = _chapterRepository.GetOne(
    //            expression: c => c.Id == id && c.Course.CompanyId == company.Id,
    //            includes: [c => c.Course]
    //        );

    //        if (chapter is null)
    //            return NotFound("Chapter not found or does not belong to your company");

    //        return Ok(chapter.Adapt<ChapterResponse>());
    //    }



    //    //[HttpPost]
    //    //public async Task<IActionResult> Create([FromBody] ChapterRequest request)
    //    //{
    //    //    var chapter = request.Adapt<Chapter>();

    //    //    var newChapter = await _chapterRepository.CreateAsync(chapter);
    //    //    if (newChapter is null)
    //    //        return BadRequest("Failed to create chapter.");

    //    //    return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Chapters/{newChapter.Id}",
    //    //        newChapter.Adapt<ChapterResponse>());
    //    //}

    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] ChapterRequest request)
    //    {
    //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (string.IsNullOrEmpty(userId))
    //            return Unauthorized("User ID not found in token.");

    //        var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
    //        if (company == null)
    //            return Unauthorized("Company not found for this user.");

    //        // Validate course belongs to current company
    //        var course = await _courseRepository.GetAsync(
    //            expression: c => c.Id == request.CourseId && c.CompanyId == company.Id);
    //        if (!course.Any())
    //            return BadRequest("Invalid course or unauthorized.");

    //        var chapter = request.Adapt<Chapter>();
    //        var newChapter = await _chapterRepository.CreateAsync(chapter);

    //        return Created(
    //            $"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Chapters/{newChapter.Id}",
    //            newChapter.Adapt<ChapterResponse>());
    //    }



    //    //[HttpPut("{id}")]
    //    //public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ChapterRequest request)
    //    //{
    //    //    var existingChapter = _chapterRepository.GetOne(c => c.Id == id);
    //    //    if (existingChapter is null)
    //    //        return NotFound();

    //    //    request.Adapt(existingChapter);

    //    //    var updated = await _chapterRepository.EditAsync(existingChapter);
    //    //    if (updated is null)
    //    //        return BadRequest("Failed to update chapter.");

    //    //    return Ok(updated.Adapt<ChapterResponse>());
    //    //}
    //    [HttpPut("{id}")]
    //    [Authorize(Roles = "CompanyAdmin")]
    //    public async Task<IActionResult> Edit(int id, [FromBody] ChapterRequest request)
    //    {
    //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (string.IsNullOrEmpty(userId))
    //            return Unauthorized("User ID not found in token.");

    //        var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
    //        if (company == null)
    //            return Unauthorized("Company not found for this user.");

    //        var chapter = (await _chapterRepository.GetAsync(
    //            expression: c => c.Id == id && c.Course.CompanyId == company.Id,
    //            includes: [c => c.Course])).FirstOrDefault();
    //        if (chapter == null)
    //            return NotFound("Chapter not found or not authorized.");

    //        // Verify that the updated course ID also belongs to the company
    //        if (request.CourseId != chapter.CourseId)
    //        {
    //            var course = await _courseRepository.GetAsync(
    //                expression: c => c.Id == request.CourseId && c.CompanyId == company.Id);
    //            if (!course.Any())
    //                return BadRequest("Invalid course update.");
    //        }

    //        request.Adapt(chapter);
    //        chapter.Id = id;

    //        await _chapterRepository.EditAsync(chapter);
    //        return Ok(chapter.Adapt<ChapterResponse>());
    //    }



    //    //[HttpDelete("{id}")]
    //    //public async Task<IActionResult> Delete([FromRoute] int id)
    //    //{
    //    //    var chapter = _chapterRepository.GetOne(c => c.Id == id);
    //    //    if (chapter is null) return NotFound();

    //    //    var deleted = await _chapterRepository.DeleteAsync(chapter);
    //    //    if (deleted is null)
    //    //        return BadRequest("Failed to delete chapter.");

    //    //    return NoContent();
    //    //}
    //    [HttpDelete("{id}")]
    //    [Authorize(Roles = "CompanyAdmin")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (string.IsNullOrEmpty(userId))
    //            return Unauthorized("User ID not found in token.");

    //        var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
    //        if (company == null)
    //            return Unauthorized("Company not found for this user.");

    //        var chapter = (await _chapterRepository.GetAsync(
    //            expression: c => c.Id == id && c.Course.CompanyId == company.Id,
    //            includes: [c => c.Course])).FirstOrDefault();

    //        if (chapter == null)
    //            return NotFound("Chapter not found or not authorized.");

    //        await _chapterRepository.DeleteAsync(chapter);
    //        return NoContent();
    //    }



   // }
}
