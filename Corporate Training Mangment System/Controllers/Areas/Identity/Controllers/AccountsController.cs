using Entities;
using Scalar;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mapster;
using Service.DTOs.Request;
namespace Corporate_Training_Mangment_System.Controllers
{
    
    [Area("Identity")]
    
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {



            ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(registerRequest.Email, "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");


               // await _userManager.AddToRoleAsync(applicationUser, SD.Customer);

                return Created();
            }
            else
            {

                return BadRequest(result.Errors);



            }
        }
    }
    }
