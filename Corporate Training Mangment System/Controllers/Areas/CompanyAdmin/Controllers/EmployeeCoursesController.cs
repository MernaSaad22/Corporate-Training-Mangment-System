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

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]

    public class EmployeeCoursesController : ControllerBase
    {
        private readonly IRepository<EmployeeCourse> _employeeCourseRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Company> _companyRepository;

        public EmployeeCoursesController(IRepository<EmployeeCourse> employeeCourseRepository,IRepository<Employee> employeeRepository,
            IRepository<Course> courseRepository,IRepository<Company>companyRepository)
        {
            _employeeCourseRepository = employeeCourseRepository;
            this._employeeRepository = employeeRepository;
            this._courseRepository = courseRepository;
            this._companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null) return Unauthorized();
            var employeeCourses = await _employeeCourseRepository.GetAsync(
                includes: [ec => ec.Course, ec => ec.Employee],
                expression: ec => ec.Course.Company.ApplicationUserId == userId
            );

            return Ok(employeeCourses.Adapt<List<EmployeeCourseResponse>>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        

            var employeeCourse = (await _employeeCourseRepository.GetAsync(
                expression: ec => ec.Id == id,
                includes: [ec => ec.Course.Company]
            )).FirstOrDefault();

            if (employeeCourse == null)
                return NotFound("EmployeeCourse not found.");

            if (employeeCourse.Course.Company.ApplicationUserId != userId)
                return Forbid();

            return Ok(employeeCourse.Adapt<EmployeeCourseResponse>());
        }

        // company can not add mor than the max employee or mx coyrses in it's plan but it can assign the same employee to more than one course 
        //like we have plan contain of 2 course and 3 employess ==>assign 2 employees for the first cours
                                                              //==>assign 1 from the first for the second course else 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssignCourseRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var course = (await _courseRepository.GetAsync(
                expression: c => c.Id == request.CourseId,
                includes: [c => c.Company]
            )).FirstOrDefault();

            if (course is null)
                return NotFound("Course not found.");

            var employee = _employeeRepository.GetOne(e => e.Id == request.EmployeeId);
            if (employee is null)
                return NotFound("Employee not found.");

            if (course.Company?.ApplicationUserId != userId)
                return Forbid("Only the company admin can assign students to this course.");
            //for renew subscription
            if(course.Company.EndDate< DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");


            var alreadyAssigned = (await _employeeCourseRepository.GetAsync(
                ec => ec.CourseId == request.CourseId && ec.EmployeeId == request.EmployeeId
            )).Any();

            if (alreadyAssigned)
                return BadRequest("Employee already assigned to this course.");

            var employeeCourse = new EmployeeCourse
            {
                CourseId = request.CourseId,
                EmployeeId = request.EmployeeId,
                AssignedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow.AddDays(request.DeadlineDays),
                IsCompleted = false
            };

            var newEmployeeCourse = await _employeeCourseRepository.CreateAsync(employeeCourse);
            return Created(
                    $"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/EmployeeCourses/{newEmployeeCourse.Id}",
                    newEmployeeCourse.Adapt<EmployeeCourseResponse>()
                );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] UpdateCourseEmployeeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //
            

            var employeeCourse = (await _employeeCourseRepository.GetAsync(
                ec => ec.Id == id,
                includes: [ec => ec.Course.Company]
            )).FirstOrDefault();

            if (employeeCourse == null)
                return NotFound("Not found.");
            // if company is owner
            if (employeeCourse.Course.Company.ApplicationUserId != userId)
                return Forbid();
            //for renew subscription
            if (employeeCourse.Course.Company.EndDate < DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");


            employeeCourse.IsCompleted = request.IsCompleted;
            employeeCourse.CompletedAt = DateTime.UtcNow.AddDays(request.DeadlineDays);

            var updated = await _employeeCourseRepository.EditAsync(employeeCourse);
            return Ok(updated.Adapt<EmployeeCourseResponse>());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var employeeCourse = (await _employeeCourseRepository.GetAsync(
                ec => ec.Id == id,
                includes: [ec => ec.Course.Company]
            )).FirstOrDefault();

            if (employeeCourse == null)
                return NotFound("Not found.");
            //owner of company
            if (employeeCourse.Course.Company.ApplicationUserId != userId)
                return Forbid();

            //for renew subscription
            if (employeeCourse.Course.Company.EndDate < DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");


            await _employeeCourseRepository.DeleteAsync(employeeCourse);
            return NoContent();
        }
    }
}
