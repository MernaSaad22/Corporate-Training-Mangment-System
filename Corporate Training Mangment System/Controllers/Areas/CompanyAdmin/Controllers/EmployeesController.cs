using Azure;
using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]

    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<EmployeeCourseProgress> _courseProgressRepository;
        private readonly IRepository<Course> _courseRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Company> companyRepository, UserManager<ApplicationUser> userManager
            , IRepository<EmployeeCourseProgress> courseProgressRepository,IRepository<Course>courseRepository)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            this._userManager = userManager;
            _courseProgressRepository = courseProgressRepository;
            _courseRepository = courseRepository;
        }

        
        [HttpGet("my-employees")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetByCompany()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized("Company not found for this user.");

            var employees = await _employeeRepository.GetAsync(
                e => e.CompanyId == company.Id,
                includes: [e => e.ApplicationUser, e => e.Company]);

            TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.ApplicationUser.UserName ?? "")
                .Map(dest => dest.CompanyId, src => src.CompanyId);

            return Ok(employees.Adapt<IEnumerable<EmployeeResponse>>());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>> GetOne([FromRoute] string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized("Company not found for this user.");

            var employee = _employeeRepository.GetOne(
                e => e.Id == id && e.CompanyId == company.Id,
                includes: [e => e.ApplicationUser, e => e.Company]);

            if (employee is null)
                return NotFound();

            TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.ApplicationUser.UserName ?? "")
                .Map(dest => dest.CompanyId, src => src.CompanyId);

            return Ok(employee.Adapt<EmployeeResponse>());
        }


        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User ID not found in token.");

        //    var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
        //    if (company is null)
        //        return Unauthorized("Company not found for this user.");

        //    var user = await _userManager.FindByIdAsync(request.ApplicationUserId);
        //    if (user is null)
        //        return BadRequest("User not found.");

        //    var employee = new Employee
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        ApplicationUserId = user.Id,
        //        ApplicationUser = user,
        //        CompanyId = company.Id,
        //        Company = company,
        //        JobTitle = request.JobTitle
        //    };

        //    var newEmployee = await _employeeRepository.CreateAsync(employee);
        //    if (newEmployee is null)
        //        return BadRequest();

        //    var response = new EmployeeResponse
        //    {
        //        Id = newEmployee.Id,
        //        JobTitle = newEmployee.JobTitle,
        //        CompanyId = newEmployee.CompanyId,
        //        ApplicationUserId = newEmployee.ApplicationUserId,
        //        UserName = user.UserName,
        //    };

        //    return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Employees/{response.Id}", response);
        //}

        // i want when a company Add an employee make sure <=Plan.MaxEmployees
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            // Include Plan when fetching company
            var company = _companyRepository.GetOne(
                c => c.ApplicationUserId == userId,
                includes: [c => c.Plan]);

            if (company is null)
                return Unauthorized("Company not found for this user.");

            // Check current employee count
            var existingEmployees = await _employeeRepository.GetAsync(e => e.CompanyId == company.Id);
            if (existingEmployees.Count() >= company.Plan.MaxEmployees)
                return BadRequest($"Employee limit reached for your plan. Max allowed: {company.Plan.MaxEmployees}");

            var user = await _userManager.FindByIdAsync(request.ApplicationUserId);
            if (user is null)
                return BadRequest("User not found.");

            var employee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                ApplicationUser = user,
                CompanyId = company.Id,
                Company = company,
                JobTitle = request.JobTitle
            };

            var newEmployee = await _employeeRepository.CreateAsync(employee);
            if (newEmployee is null)
                return BadRequest();

            var response = new EmployeeResponse
            {
                Id = newEmployee.Id,
                JobTitle = newEmployee.JobTitle,
                CompanyId = newEmployee.CompanyId,
                ApplicationUserId = newEmployee.ApplicationUserId,
                UserName = user.UserName,
            };

            return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Employees/{response.Id}", response);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] EmployeeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized("Company not found for this user.");

            var employeeInDb = _employeeRepository.GetOne(
                e => e.Id == id && e.CompanyId == company.Id,
                includes: [e => e.ApplicationUser, e => e.Company]);

            if (employeeInDb is null)
                return NotFound();

            employeeInDb.JobTitle = request.JobTitle;
            employeeInDb.ApplicationUserId = request.ApplicationUserId;

            var updated = await _employeeRepository.EditAsync(employeeInDb);
            if (updated is null)
                return BadRequest();

            var response = new EmployeeResponse
            {
                Id = updated.Id,
                JobTitle = updated.JobTitle,
                CompanyId = updated.CompanyId,
                ApplicationUserId = updated.ApplicationUserId,
                UserName = updated.ApplicationUser?.UserName ?? ""
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null)
                return Unauthorized("Company not found for this user.");

            var employee = _employeeRepository.GetOne(e => e.Id == id && e.CompanyId == company.Id);
            if (employee is null) return NotFound("Employee not found or does not belong to your company.");

            var deleted = await _employeeRepository.DeleteAsync(employee);
            if (deleted is null) return BadRequest("Failed to delete employee.");

            return NoContent();
        }




        [HttpGet("course/{courseId}/progress")]
        public async Task<ActionResult<IEnumerable<EmployeeProgressResponse>>> GetEmployeesProgressByCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized("Company not found for this user.");

            var employeesInCompany = await _employeeRepository.GetAsync(e => e.CompanyId == company.Id);
            var employeeIds = employeesInCompany.Select(e => e.Id).ToList();

            var progressList = await _courseProgressRepository.GetAsync(
                p => p.CourseId == courseId && employeeIds.Contains(p.EmployeeId),
                includes: [p => p.Employee, p => p.Employee.ApplicationUser]
            );

            var result = progressList.Select(p => new EmployeeProgressResponse
            {
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee?.ApplicationUser?.UserName ?? "Unknown",
                LessonProgress = (double)p.LessonProgress,
                AssignmentProgress = (double)p.AssignmentProgress,
                ExamProgress = (double)p.ExamProgress,
                TotalProgress = Math.Round((double)(p.LessonProgress + p.AssignmentProgress + p.ExamProgress), 2),
                LastUpdated = p.LastUpdated
            });

            return Ok(result);
        }

        [HttpGet("{employeeId}/course/{courseId}")]
        public IActionResult GetEmployeeCourseProgress([FromRoute]string employeeId, [FromRoute]int courseId)
        {
            var companyUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(companyUserId))
                return Unauthorized("User ID not found in token.");

           
            var company = _companyRepository.GetOne(c => c.ApplicationUserId == companyUserId);
            if (company == null)
                return Unauthorized("Company not found for this user.");

 
            var employee = _employeeRepository.GetOne(e => e.Id == employeeId && e.CompanyId == company.Id);
            if (employee == null)
                return Unauthorized("Employee not found or does not belong to your company.");

     
            var course = _courseRepository.GetOne(c => c.Id == courseId);
            if (course == null)
                return NotFound("Course not found.");

    
            var progress = _courseProgressRepository.GetOne(
                p => p.EmployeeId == employeeId && p.CourseId == courseId,
                includes: [p => p.Employee, p => p.Employee.ApplicationUser]);

            if (progress == null)
                return NotFound("No progress found for this employee in this course.");

            var response = new EmployeeProgressResponse
            {
                EmployeeId =employeeId,
                EmployeeName = progress.Employee.ApplicationUser.UserName,
                LessonProgress = (double)progress.LessonProgress,
                AssignmentProgress = (double)progress.AssignmentProgress,
                ExamProgress = (double)progress.ExamProgress,
                TotalProgress = (double)Math.Round(progress.LessonProgress + progress.AssignmentProgress + progress.ExamProgress, 2),
                LastUpdated = progress.LastUpdated
            };

            return Ok(response);
        }


    }
}