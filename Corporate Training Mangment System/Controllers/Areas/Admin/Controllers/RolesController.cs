using DataAccess.IRepository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        public RolesController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _roleRepository.GetAsync();

            return Ok(users.Adapt<IEnumerable<RoleResponse>>());
        }
    }
}
