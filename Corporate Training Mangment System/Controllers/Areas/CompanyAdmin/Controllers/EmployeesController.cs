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

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Company> companyRepository, UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            this._userManager = userManager;
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


    }
}