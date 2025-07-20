using DataAccess.IRepository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Response;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area ("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class PlansController : ControllerBase
    {
        private readonly IRepository<Company> _companyRepository;

        public PlansController(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public IActionResult GetMyPlan()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var company = _companyRepository.GetOne(
                c => c.ApplicationUserId == userId,
                includes: new Expression<Func<Company, object>>[] { c => c.Plan });

            if (company is null || company.Plan is null)
                return NotFound("Company or plan not found.");

            var response = company.Plan.Adapt<PlanResponse>();
            return Ok(response);
        }


        [HttpGet("PlanDetails")]
        public IActionResult GetDetailsMyPlan()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var company = _companyRepository.GetOne(
                c => c.ApplicationUserId == userId,
                includes: new Expression<Func<Company, object>>[] { c => c.Plan });

            if (company is null || company.Plan is null)
                return NotFound("Company or plan not found.");

            var response = company.Plan.Adapt<PlanCompanyDetailsResponse>();
            response.StartDate = company.StartDate;
            response.EndDate = company.EndDate;

            if (company.EndDate.HasValue)
            {
                var remaining = (company.EndDate.Value - DateTime.Now).Days;

                response.DaysRemaining = remaining > 0 ? remaining : 0;

                if (remaining <= 0)
                {
                    response.IsExpired = true;
                    response.StatusMessage = "Your plan has expired.";
                }
                else if (remaining <= 3)
                {
                    response.IsExpired = false;
                    response.StatusMessage = $"Your plan is expiring in {remaining} day(s).";
                }
                else
                {
                    response.IsExpired = false;
                    response.StatusMessage = $"Your plan is active. {remaining} day(s) remaining.";
                }
            }
            else
            {
                response.IsExpired = true;
                response.StatusMessage = "Plan dates are missing.";
            }

            return Ok(response);
        }





    }
}
