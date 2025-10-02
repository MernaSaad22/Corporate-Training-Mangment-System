using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class PlanPaymentsController : ControllerBase
    {
        private readonly IRepository<Plan> _planRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUserRepository _userRepository;

        public PlanPaymentsController(IRepository<Company> companyRepository, IRepository<Plan> planRepository, IUserRepository userRepository)
        {
           _companyRepository = companyRepository;
            _planRepository = planRepository;
            _userRepository = userRepository;
        }



        //[HttpPost("create-checkout-session/{planId}")]
        //public async Task<IActionResult> CreateCheckoutSession(int planId)
        //{
        //    var plan = await _planRepository.GetAsync(p => p.Id == planId);
        //    var selectedPlan = plan.FirstOrDefault();

        //    if (selectedPlan == null)
        //        return NotFound("Plan not found");

        //    var domain = "https://localhost:7046"; // 🔁 Replace with your frontend domain if needed

        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>
        //        {
        //            new()
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmountDecimal = selectedPlan.Cost * 100, // Stripe uses cents
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = selectedPlan.Name
        //                    }
        //                },
        //                Quantity = 1
        //            }
        //        },
        //        Mode = "payment",
        //        SuccessUrl = $"{domain}/api/CompanyAdmin/PlanPayments/success?planId={selectedPlan.Id}",
        //        CancelUrl = $"{domain}/api/CompanyAdmin/PlanPayments/cancel"
        //    };

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    return Ok(new { url = session.Url });
        //}

        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(int planId)
        //{
        //    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User not logged in");

        //    // Get company using ApplicationUserId
        //    var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null || plan == null)
        //        return BadRequest("Invalid company or plan");

        //    company.PlanId = plan.Id;
        //    company.StartDate = DateTime.UtcNow;
        //    company.EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays);

        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message = "Payment successful and plan assigned!" });
        //}


        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(int planId)
        //{
        //    //var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    //if (string.IsNullOrEmpty(userId))
        //    //    return Unauthorized("User not logged in");
        //    //i test it manually becausse of redirection of stripe page to success action need his token 
        //    var userId = "752c0e6f-fc52-4c07-99ba-eb500c8baab2";

        //    var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null || plan == null)
        //        return BadRequest("Invalid company or plan");

        //    // ✅ Set the transaction date and end date
        //    company.PlanId = plan.Id;
        //    company.StartDate = DateTime.UtcNow;
        //    company.EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays);

        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message = "Payment successful and plan activated!" });
        //}


        [HttpGet("cancel")]
        public IActionResult PaymentCancel()
        {
            return Ok(new { message = "Payment cancelled." });
        }


        // if i want to make option for renew subscription
        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(int planId)
        //{
        //    //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    //if (string.IsNullOrEmpty(userId))
        //    //    return Unauthorized("User not logged in");
        //    // var userId = "7749891b-61ab-42f7-9a22-dc6b2a764589";
        //    var userId = "e74be296-7ad7-4819-a396-6eecab75bb7a";

        //     var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null || plan == null)
        //        return BadRequest("Invalid company or plan");

        //    var now = DateTime.UtcNow;

        //    //// If subscription expired, start from now, else extend
        //    //if (company.EndDate == null || company.EndDate < now)
        //    //{
        //    //    company.StartDate = now;
        //    //    company.EndDate = now.AddDays(plan.DurationInDays);
        //    //}
        //    //else
        //    //{
        //    //    // Add to current EndDate
        //    //    company.EndDate = company.EndDate.Value.AddDays(plan.DurationInDays);
        //    //}


        //    string message;

        //    if (company.EndDate == null || company.EndDate < now)
        //    {
        //        // First-time subscription or expired renewal
        //        company.StartDate = now;
        //        company.EndDate = now.AddDays(plan.DurationInDays);
        //        message = "Payment successful and plan activated!";
        //    }
        //    else
        //    {
        //        // Extend current subscription
        //        company.EndDate = company.EndDate.Value.AddDays(plan.DurationInDays);
        //        message = "Payment successful and plan renewed!";
        //    }

        //    company.PlanId = plan.Id;

        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message = "Payment successful and plan renewed!" });
        //}

        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(int planId)
        //{
        //    // Normally you get this from the logged-in user
        //    var userId = "e74be296-7ad7-4819-a396-6eecab75bb7a"; // ✅ remove spaces, yours had invalid formatting

        //    var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null || plan == null)
        //        return BadRequest("Invalid company or plan");

        //    var now = DateTime.UtcNow;

        //    string message;

        //    if (company.EndDate == null || company.EndDate < now)
        //    {
        //        // First-time subscription or expired renewal
        //        company.StartDate = now;
        //        company.EndDate = now.AddDays(plan.DurationInDays);
        //        message = "Payment successful and plan activated!";
        //    }
        //    else
        //    {
        //        // Extend current subscription
        //        company.EndDate = company.EndDate.Value.AddDays(plan.DurationInDays);
        //        message = "Payment successful and plan renewed!";
        //    }

        //    company.PlanId = plan.Id;

        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message });
        //}

        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(int planId)
        //{
        //    var userId = "e74be296-7ad7-4819-a396-6eecab75bb7a"; 

        //    var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var selectedPlan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null)
        //        return BadRequest("Company not found for this user.");

        //    if (selectedPlan == null)
        //        return BadRequest("Plan ID is invalid.");

        //    var now = DateTime.UtcNow;

        //    string message;

        //    bool isFirstTime =  company.StartDate == null || company.EndDate == null;

        //    if (isFirstTime)
        //    {
        //        // ✅ First time subscribing
        //        company.StartDate = now;
        //        company.EndDate = now.AddDays(selectedPlan.DurationInDays);
        //        message = "Payment successful and plan activated!";
        //    }
        //    else if (company.EndDate < now)
        //    {
        //        // ✅ Subscription expired
        //        if (company.PlanId == selectedPlan.Id)
        //        {
        //            // ✅ Same plan → Renewal
        //            company.StartDate = now;
        //            company.EndDate = now.AddDays(selectedPlan.DurationInDays);
        //            message = "Payment successful and plan renewed!";
        //        }
        //        else
        //        {
        //            // ✅ Different plan → Upgrade or change
        //            company.StartDate = now;
        //            company.EndDate = now.AddDays(selectedPlan.DurationInDays);
        //            message = "Payment successful and plan upgraded!";
        //        }
        //    }
        //    else
        //    {
        //        // ✅ Still active → Extend the subscription
        //        company.EndDate = company.EndDate.Value.AddDays(selectedPlan.DurationInDays);
        //        message = "Payment successful and plan extended!";
        //    }

        //    company.PlanId = selectedPlan.Id;
        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message });
        //}

        // worked put enable company to renew if enddate>now it is not correct
        //[HttpPost("create-checkout-session/{planId}")]
        //public async Task<IActionResult> CreateCheckoutSession(int planId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();
        //    if (plan == null)
        //        return NotFound("Plan not found");

        //    var domain = "https://localhost:7046"; // your domain

        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>
        //{
        //    new()
        //    {
        //        PriceData = new SessionLineItemPriceDataOptions
        //        {
        //            UnitAmountDecimal = plan.Cost * 100,
        //            Currency = "usd",
        //            ProductData = new SessionLineItemPriceDataProductDataOptions
        //            {
        //                Name = plan.Name
        //            }
        //        },
        //        Quantity = 1
        //    }
        //},
        //        Mode = "payment",
        //        ClientReferenceId = userId,
        //        Metadata = new Dictionary<string, string>
        //{
        //    { "planId", planId.ToString() },
        //    { "userId", userId }
        //},
        //        SuccessUrl = $"{domain}/api/CompanyAdmin/PlanPayments/success?session_id={{CHECKOUT_SESSION_ID}}",
        //        CancelUrl = $"{domain}/api/CompanyAdmin/PlanPayments/cancel"
        //    };

        //    var service = new SessionService();
        //    var session = service.Create(options);

        //    return Ok(new { url = session.Url });
        //}


        //[HttpGet("success")]
        //public async Task<IActionResult> PaymentSuccess(string session_id)
        //{
        //    if (string.IsNullOrEmpty(session_id))
        //        return BadRequest("Session ID is missing");

        //    var service = new SessionService();
        //    var session = await service.GetAsync(session_id);

        //    // Get userId from ClientReferenceId or Metadata
        //    var userId = session.ClientReferenceId ?? (session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null);
        //    if (string.IsNullOrEmpty(userId))
        //        return BadRequest("User not found in session");

        //    // Get planId from Metadata
        //    if (!session.Metadata.TryGetValue("planId", out var planIdStr) || !int.TryParse(planIdStr, out var planId))
        //    {
        //        return BadRequest("Plan ID missing or invalid");
        //    }

        //    var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
        //    var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

        //    if (company == null || plan == null)
        //        return BadRequest("Invalid company or plan");

        //    var now = DateTime.UtcNow;

        //    if (company.EndDate == null || company.EndDate < now)
        //    {
        //        company.StartDate = now;
        //        company.EndDate = now.AddDays(plan.DurationInDays);
        //    }
        //    else
        //    {
        //        company.EndDate = company.EndDate.Value.AddDays(plan.DurationInDays);
        //    }

        //    company.PlanId = plan.Id;

        //    await _companyRepository.EditAsync(company);

        //    return Ok(new { message = "Payment successful and plan activated or renewed!" });
        //}




        [HttpPost("create-checkout-session/{planId}")]
        public async Task<IActionResult> CreateCheckoutSession(int planId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();
            if (plan == null)
                return NotFound("Plan not found");

            // 🔍 Get company by ApplicationUserId
            var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
            if (company == null)
                return BadRequest("Company not found");

            var now = DateTime.UtcNow;

            // ❌ Block if subscription is still active
            if (company.EndDate != null && company.EndDate > now)
            {
                return BadRequest($"You already have an active subscription. You can renew or switch plans after {company.EndDate.Value:yyyy-MM-dd}.");
            }

            // ✅ Continue to create Stripe Checkout session
            var domain = "https://localhost:7046"; // your domain

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = plan.Cost * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = plan.Name
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                ClientReferenceId = userId,
                Metadata = new Dictionary<string, string>
        {
            { "planId", planId.ToString() },
            { "userId", userId }
        },
                SuccessUrl = $"{domain}/api/CompanyAdmin/PlanPayments/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/api/CompanyAdmin/PlanPayments/cancel"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { url = session.Url });
        }



        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
                return BadRequest("Session ID is missing");

            var service = new SessionService();
            var session = await service.GetAsync(session_id);

            // Get userId from ClientReferenceId or Metadata
            var userId = session.ClientReferenceId ?? (session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User not found in session");

            // Get planId from Metadata
            if (!session.Metadata.TryGetValue("planId", out var planIdStr) || !int.TryParse(planIdStr, out var planId))
            {
                return BadRequest("Plan ID missing or invalid");
            }

            var company = (await _companyRepository.GetAsync(c => c.ApplicationUserId == userId)).FirstOrDefault();
            var plan = (await _planRepository.GetAsync(p => p.Id == planId)).FirstOrDefault();

            if (company == null || plan == null)
                return BadRequest("Invalid company or plan");

            var now = DateTime.UtcNow;

            // 🚫 If the current subscription is still active, block the purchase
            if (company.EndDate != null && company.EndDate > now)
            {
                return BadRequest($"You already have an active subscription. You can renew or switch plans after {company.EndDate.Value:yyyy-MM-dd}.");
            }

            // ✅ Subscription is expired or not set, allow renewal or new plan selection
            company.StartDate = now;
            company.EndDate = now.AddDays(plan.DurationInDays);
            company.PlanId = plan.Id;

            await _companyRepository.EditAsync(company);

            return Ok(new { message = "Payment successful and plan activated or renewed!" });
        }


    }
}
