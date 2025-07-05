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
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAll()
        {
            var employees = await _employeeRepository.GetAsync(
                includes: [e => e.ApplicationUser, e => e.Company]);

            return Ok(employees.Adapt<IEnumerable<EmployeeResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string id)
        {
            var employee = _employeeRepository.GetOne(
                e => e.Id == id,
                includes: [e => e.ApplicationUser, e => e.Company]);

            if (employee is null) return NotFound();

            return Ok(employee.Adapt<EmployeeResponse>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
        {
            var employee = request.Adapt<Employee>();
            employee.Id = Guid.NewGuid().ToString();

            var newEmployee = await _employeeRepository.CreateAsync(employee);
            if (newEmployee is null) return BadRequest();

            return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Employees/{newEmployee.Id}",
                newEmployee.Adapt<EmployeeResponse>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] EmployeeRequest request)
        {
            var employeeInDb = _employeeRepository.GetOne(e => e.Id == id);
            if (employeeInDb is null) return NotFound();

            request.Adapt(employeeInDb);

            var updated = await _employeeRepository.EditAsync(employeeInDb);
            if (updated is null) return BadRequest();

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
