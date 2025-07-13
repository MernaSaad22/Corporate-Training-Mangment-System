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
    public class CoursesController : ControllerBase
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Instructor> _instructorRepository;
        private readonly IRepository<CourseCategory> _courseCategoryRepository;
        private readonly IRepository<Company> _companyRepository;

        public CoursesController(IRepository<Course> courseRepository,IRepository<Instructor> instructorRepository,
            IRepository<CourseCategory> courseCategoryRepository,IRepository<Company> companyRepository)
        {
            _courseRepository = courseRepository;
            this._instructorRepository = instructorRepository;
            this._courseCategoryRepository = courseCategoryRepository;
            this._companyRepository = companyRepository;
        }

        //without authorize
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        //{
        //    var courses = await _courseRepository.GetAsync(
        //        includes: new Expression<Func<Course, object>>[] { c => c.Instructor, c => c.Company, c => c.Category });

        //    return Ok(courses.Adapt<IEnumerable<CourseResponse>>());
        //}

//with authorization
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            var courses = await _courseRepository.GetAsync(
    c => c.CompanyId == company.Id,
    includes: new Expression<Func<Course, object>>[] {
        c => c.Instructor,
        c => c.Instructor.ApplicationUser,
        c => c.Category,
        c => c.Company
    });

            return Ok(courses.Adapt<IEnumerable<CourseResponse>>());
        }



        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] int id)
        //{
        //    var course = _courseRepository.GetOne(
        //        c => c.Id == id,
        //        includes: new Expression<Func<Course, object>>[] { c => c.Instructor, c => c.Company, c => c.Category });

        //    if (course == null) return NotFound();

        //    return Ok(course.Adapt<CourseResponse>());
        //}

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            var course = _courseRepository.GetOne(
                c => c.Id == id,
                includes: new Expression<Func<Course, object>>[] { c => c.Instructor, c => c.Company, c => c.Category });

            if (course is null) return NotFound();
            if (course.CompanyId != company.Id) return Forbid();

            return Ok(course.Adapt<CourseResponse>());
        }



        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CourseRequest request)
        //{

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);


        //    var instructor = _instructorRepository.GetOne(i => i.Id == request.InstructorId);
        //    if (instructor == null)
        //        return BadRequest("Instructor not found.");

        //    var category = _courseCategoryRepository.GetOne(c => c.Id == request.CategoryId);
        //    if (category == null)
        //        return BadRequest("Category not found.");

        //    var company = _companyRepository.GetOne(c => c.Id == request.CompanyId);
        //    if (company == null)
        //        return BadRequest("Company not found.");

        //    var course = request.Adapt<Course>();
        //    var created = await _courseRepository.CreateAsync(course);
        //    if (created == null)
        //        return BadRequest("Failed to create course.");

        //    var response = created.Adapt<CourseResponse>();
        //    return CreatedAtAction(nameof(GetOne), new { id = created.Id }, response);
        //}


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            var instructor = _instructorRepository.GetOne(i => i.Id == request.InstructorId);
            if (instructor is null || instructor.CompanyId != company.Id)
                return BadRequest("Instructor not found or does not belong to your company.");

            var category = _courseCategoryRepository.GetOne(c => c.Id == request.CategoryId);
            if (category is null || category.CompanyId != company.Id)
                return BadRequest("Category not found or does not belong to your company.");

            var course = request.Adapt<Course>();
            course.CompanyId = company.Id;

            var created = await _courseRepository.CreateAsync(course);
            if (created is null) return BadRequest("Failed to create course.");

            return CreatedAtAction(nameof(GetOne), new { id = created.Id }, created.Adapt<CourseResponse>());
        }



        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var existingCourse = _courseRepository.GetOne(c => c.Id == id);
        //    if (existingCourse == null) return NotFound();


        //    existingCourse.Title = request.Title;
        //    existingCourse.CompanyId = request.CompanyId;
        //    existingCourse.InstructorId = request.InstructorId;
        //    existingCourse.CategoryId = request.CategoryId;

        //    var updated = await _courseRepository.EditAsync(existingCourse);
        //    if (updated == null)
        //        return BadRequest("Failed to update course.");

        //    return Ok(updated.Adapt<CourseResponse>());
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            var course = _courseRepository.GetOne(c => c.Id == id);
            if (course is null) return NotFound();
            if (course.CompanyId != company.Id) return Forbid();

            var instructor = _instructorRepository.GetOne(i => i.Id == request.InstructorId);
            if (instructor is null || instructor.CompanyId != company.Id)
                return BadRequest("Instructor not found or does not belong to your company.");

            var category = _courseCategoryRepository.GetOne(c => c.Id == request.CategoryId);
            if (category is null || category.CompanyId != company.Id)
                return BadRequest("Category not found or does not belong to your company.");

            course.Title = request.Title;
            course.InstructorId = request.InstructorId;
            course.CategoryId = request.CategoryId;

            var updated = await _courseRepository.EditAsync(course);
            if (updated is null) return BadRequest("Failed to update course.");

            return Ok(updated.Adapt<CourseResponse>());
        }



        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    var course = _courseRepository.GetOne(c => c.Id == id);
        //    if (course == null) return NotFound();

        //    var deleted = await _courseRepository.DeleteAsync(course);
        //    if (deleted == null)
        //        return BadRequest("Failed to delete course.");

        //    return NoContent();
        //}


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();

            var course = _courseRepository.GetOne(c => c.Id == id);
            if (course is null) return NotFound();
            if (course.CompanyId != company.Id) return Forbid();

            var deleted = await _courseRepository.DeleteAsync(course);
            if (deleted is null) return BadRequest("Failed to delete course.");

            return NoContent();
        }

    }
}
