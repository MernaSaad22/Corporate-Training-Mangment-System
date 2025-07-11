using DataAccess.IRepository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Service.DTOs.Request;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _userRepository.GetAsync();

            return Ok(roles.Adapt<IEnumerable<UserResponse>>());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {


                return Ok(user.Adapt<UserResponse>());
            }

            return NotFound();
        }

        [HttpPost("")]

        public async Task<IActionResult> ChangeRole(UserNameWithRoleNameRequest userNameWithRoleNameRequest)
        {

            ModelStateDictionary keyValuePairs = new ModelStateDictionary();
            var applicationUser = await _userManager.FindByNameAsync(userNameWithRoleNameRequest.UserName);

            if (applicationUser is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(applicationUser);
                await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

                var result = await _userManager.AddToRoleAsync(applicationUser, userNameWithRoleNameRequest.RoleName);

                if (result.Succeeded)
                {


                    return NoContent();
                }
                else
                {


                    return BadRequest(result.Errors);
                }
            }

            keyValuePairs.AddModelError("UserName", "Invalid UserName");
            return BadRequest(keyValuePairs);
        }
        [HttpGet("LockUnLock/{id}")]
        public async Task<IActionResult> LockUnLock([FromRoute] string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.Now.AddMonths(1);

                }
                else
                {
                    user.LockoutEnd = null;

                }

                user.LockoutEnabled = !user.LockoutEnabled;
                await _userManager.UpdateAsync(user);

                return Ok("Update user status successfully");
            }

            return NotFound();
        }
    }
}
