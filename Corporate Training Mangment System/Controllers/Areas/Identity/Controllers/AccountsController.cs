using Entities;
using Scalar;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mapster;
using Service.DTOs.Request;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginRequest loginRequest)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginRequest.EmialOrUserName);
            ModelStateDictionary keyValuePairs = new();

            if (applicationUser is null)
                applicationUser = await _userManager.FindByNameAsync(loginRequest.EmialOrUserName);
            if(applicationUser is not null)
            {
                if (applicationUser.LockoutEnabled)
                {
                    var result = await _userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);
                    if (result)
                    {
                        await _signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);
                        return NoContent();

                    }
                    else
                    {
                        keyValuePairs.AddModelError("EmailOrUserName","Invalid Email Or UserName");
                        keyValuePairs.AddModelError("Password", "Invalid Password");
                        return BadRequest(keyValuePairs);
                    }
                }
                else
                {
                    keyValuePairs.AddModelError(string.Empty, $"Tou Have Block Untill{applicationUser.LockoutEnd}");
                }
            }
            else
            {
                keyValuePairs.AddModelError("EmailOrUserName","Invalid Email Or UserName");
                keyValuePairs.AddModelError("Password","Invalid Password");
            }
            return BadRequest(keyValuePairs);

        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
    }
