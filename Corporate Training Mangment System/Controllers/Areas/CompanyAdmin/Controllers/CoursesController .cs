using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Linq.Expressions;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        {
            var courses = await _courseRepository.GetAsync(
                includes: new Expression<Func<Course, object>>[] { c => c.Instructor, c => c.Company, c => c.Category });

            return Ok(courses.Adapt<IEnumerable<CourseResponse>>());
        }

        // GET: api/CompanyAdmin/Courses/5
        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var course = _courseRepository.GetOne(
                c => c.Id == id,
                includes: new Expression<Func<Course, object>>[] { c => c.Instructor, c => c.Company, c => c.Category });

            if (course == null) return NotFound();

            return Ok(course.Adapt<CourseResponse>());
        }

        // POST: api/CompanyAdmin/Courses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check before creating 
            var instructor = _instructorRepository.GetOne(i => i.Id == request.InstructorId);
            if (instructor == null)
                return BadRequest("Instructor not found.");

            var category = _courseCategoryRepository.GetOne(c => c.Id == request.CategoryId);
            if (category == null)
                return BadRequest("Category not found.");

            var company = _companyRepository.GetOne(c => c.Id == request.CompanyId);
            if (company == null)
                return BadRequest("Company not found.");

            var course = request.Adapt<Course>();
            var created = await _courseRepository.CreateAsync(course);
            if (created == null)
                return BadRequest("Failed to create course.");

            var response = created.Adapt<CourseResponse>();
            return CreatedAtAction(nameof(GetOne), new { id = created.Id }, response);
        }


        // PUT: api/CompanyAdmin/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCourse = _courseRepository.GetOne(c => c.Id == id);
            if (existingCourse == null) return NotFound();

            // Update fields
            existingCourse.Title = request.Title;
            existingCourse.CompanyId = request.CompanyId;
            existingCourse.InstructorId = request.InstructorId;
            existingCourse.CategoryId = request.CategoryId;

            var updated = await _courseRepository.EditAsync(existingCourse);
            if (updated == null)
                return BadRequest("Failed to update course.");

            return Ok(updated.Adapt<CourseResponse>());
        }

        // DELETE: api/CompanyAdmin/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var course = _courseRepository.GetOne(c => c.Id == id);
            if (course == null) return NotFound();

            var deleted = await _courseRepository.DeleteAsync(course);
            if (deleted == null)
                return BadRequest("Failed to delete course.");

            return NoContent();
        }
    }
}
