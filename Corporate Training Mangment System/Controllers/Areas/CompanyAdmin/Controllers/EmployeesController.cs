using Azure;
using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
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

        [HttpGet("by-company/{companyId}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetByCompany(string companyId)
        {
            var company = _companyRepository.GetOne(c => c.Id == companyId);
            if (company == null)
                return NotFound("Company not found.");

            var employees = await _employeeRepository.GetAsync(
                e => e.CompanyId == company.Id,
                includes: [e => e.ApplicationUser, e => e.Company]);

            // ✨ Mapster config علشان UserName و Name
            TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.ApplicationUser.UserName ?? "")
                .Map(dest => dest.CompanyId, src => src.CompanyId);

            return Ok(employees.Adapt<IEnumerable<EmployeeResponse>>());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>> GetOne([FromRoute] string id, [FromQuery] string companyId)
        {
            var employee = _employeeRepository.GetOne(
                e => e.Id == id && e.CompanyId == companyId,
                includes: [e => e.ApplicationUser, e => e.Company]);

            if (employee is null) return NotFound();

            TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.ApplicationUser.UserName??"")
                .Map(dest => dest.CompanyId, src => src.CompanyId);

            return Ok(employee.Adapt<EmployeeResponse>());
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.ApplicationUserId);
            if (user is null)
                return BadRequest("User not found.");

            var company = _companyRepository.GetOne(c => c.Id == request.CompanyId);
            if (company is null)
                return BadRequest("Company not found.");

            var employee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                ApplicationUser = user,
                CompanyId = request.CompanyId,
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
            var employeeInDb = _employeeRepository.GetOne(
                e => e.Id == id,
                includes: [e => e.ApplicationUser, e => e.Company]); 

            if (employeeInDb is null) return NotFound();

            request.Adapt(employeeInDb);
            employeeInDb.Id = id;

            var updated = await _employeeRepository.EditAsync(employeeInDb);
            if (updated is null) return BadRequest();

            TypeAdapterConfig<Employee, EmployeeResponse>.NewConfig()
               .Map(dest => dest.UserName, src => src.ApplicationUser.UserName ?? "")
               .Map(dest => dest.CompanyId, src => src.CompanyId);

            return Ok(updated.Adapt<EmployeeResponse>()); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var employee = _employeeRepository.GetOne(e => e.Id == id);
            if (employee is null) return NotFound();

            var deleted = await _employeeRepository.DeleteAsync(employee);
            if (deleted is null) return BadRequest();

            return NoContent();
        }

    }
}