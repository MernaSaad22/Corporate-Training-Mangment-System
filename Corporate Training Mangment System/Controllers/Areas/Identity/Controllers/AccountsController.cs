using Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Scalar;
using Service.DTOs.Request;
using Service.Utility;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = Service.DTOs.Request.LoginRequest;
using RegisterRequest = Service.DTOs.Request.RegisterRequest;
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

                //until we add a payment way
                await _userManager.AddToRoleAsync(applicationUser, SD.CompanyAdmin);

                return Created();
            }
            else
            {

                return BadRequest(result.Errors);



            }
        }


        [HttpPost("RegisterInstructor")]
        public async Task<IActionResult> RegisterInstructor(RegisterRequest registerRequest)
        {



            ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(registerRequest.Email, "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");

                //until we add a payment way
                await _userManager.AddToRoleAsync(applicationUser, SD.Instructor);

                return Created();
            }
            else
            {

                return BadRequest(result.Errors);



            }
        }


        [HttpPost("RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee(RegisterRequest registerRequest)
        {



            ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(registerRequest.Email, "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");

                //until we add a payment way
                await _userManager.AddToRoleAsync(applicationUser, SD.Employee);

                return Created();
            }
            else
            {

                return BadRequest(result.Errors);



            }
        }
        //chang httppost to httpGet ==>we use token
        //[HttpGet("Login")]
         [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginRequest loginRequest)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName);
            ModelStateDictionary keyValuePairs = new();

            if (applicationUser is null)
                applicationUser = await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);
            if(applicationUser is not null)
            {
                if (applicationUser.LockoutEnabled)
                {
                    var result = await _userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);
                    if (result)
                    {
                        await _signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);
                        var roles=await _userManager.GetRolesAsync(applicationUser);

                        var claims = new[]
                        {
                           
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                             new Claim(ClaimTypes.Name,applicationUser.UserName),
                             new Claim(ClaimTypes.NameIdentifier,applicationUser.Id),
                              new Claim(ClaimTypes.Role,String.Join(",",roles))
                          

                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Eraa##Team##project##5132025##Merna##Naira##Emad"));
                        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: "https://localhost:7046",
                            //front project but i use localhost to test
                            audience: "https://localhost:7046",
                            claims:claims,
                            //AddDays byt i use second for test now (:
                           expires:DateTime.UtcNow.AddSeconds(15),
                           //for final project 
                           // expires:DateTime.UtcNow.AddDays(15),
                            signingCredentials: creds


                            );
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            role = roles.FirstOrDefault(),
                            username = applicationUser.UserName
                        });

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

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string userId,[FromQuery]string token)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if(applicationUser is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(applicationUser, token);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("ResendEmail")]
        public async Task<IActionResult> ResendEmail(ResendEmailRequest resendEmailRequest)
        {
            var applicationUser = await _userManager.FindByEmailAsync(resendEmailRequest.EmailOrUserName);
            ModelStateDictionary keyValuePairs = new();
            if (applicationUser is null) 
                applicationUser = await _userManager.FindByNameAsync(resendEmailRequest.EmailOrUserName);
            if(applicationUser is not null)
            {
                if (!applicationUser.EmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", userId = applicationUser.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(applicationUser!.Email??"", "Confirmation Your Account", $"Please Confirm Your Account By Clicking <a href='{confirmationLink}'>Here</a>");
                    return NoContent();
                }
                else
                {
                    keyValuePairs.AddModelError(String.Empty, "Already confirmed!");
                }
            }
            keyValuePairs.AddModelError("EmailOrUserName","Invalid Email Or User Name");
            return BadRequest(keyValuePairs);

        }
    }
    }
