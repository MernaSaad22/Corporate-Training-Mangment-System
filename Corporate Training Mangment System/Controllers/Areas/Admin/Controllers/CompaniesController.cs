using Azure.Core;
using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles ="SuperAdmin")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPlanRepository _planRepository;
        public CompaniesController(ICompanyRepository companyRepository, IPlanRepository planRepository)
        {
            _companyRepository = companyRepository;
            _planRepository = planRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<CompanyResponse>>> GetAll()
        {
            var companies = await _companyRepository.GetAsync();


            var config = new TypeAdapterConfig();
            return Ok(companies.Adapt<IEnumerable<CompanyResponse>>());
        }
        //company without start and end date 
        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] string id)
        //{
        //    var company = _companyRepository.GetOne(e => e.Id == id);
        //    if (company is not null)

        //        return Ok(company.Adapt<CompanyResponse>());

        //    return NotFound();
        //}

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string id)
        {
            var company = _companyRepository.GetOne(e => e.Id == id);
            if (company is not null)

                return Ok(company.Adapt<CompanyDetailsResponse>());

            return NotFound();
        }
        //without start and end date
        //[HttpPost("")]

        //public async Task<IActionResult> Create([FromForm] CompanyRequest companyRequest)
        //{
        //    if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);

        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
        //        using (var stream = System.IO.File.Create(path))
        //        {
        //            await companyRequest.Logo.CopyToAsync(stream);
        //        }
        //        //var company = companyRequest.Adapt<Company>();

        //        var company = new Company
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Name = companyRequest.Name,
        //            PlanId = companyRequest.PlanId,
        //            ApplicationUserId = companyRequest.ApplicationUserId,
        //            Logo = fileName
        //        };
        //        //company.Logo = fileName;
        //        var newCompany = await _companyRepository.CreateAsync(company);
        //        if (newCompany is not null)
        //        {
        //            return Created($"{Request.Scheme}://{Request.Host}/api/Admin/Companies/{newCompany.Id}", newCompany.Adapt<CompanyResponse>());
        //        }

        //    }

        //    return BadRequest();

        //}


        //the best one 
        //[HttpPost("")]

        //public async Task<IActionResult> Create([FromForm] CompanyRequest companyRequest)
        //{
        //    if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);

        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);
        //        using (var stream = System.IO.File.Create(path))
        //        {
        //            await companyRequest.Logo.CopyToAsync(stream);
        //        }
        //        var plan = _planRepository.GetOne(p => p.Id == companyRequest.PlanId);
        //        if (plan == null)
        //        {
        //            return BadRequest("Invalid PlanId.");
        //        }

        //        var startDate = DateTime.Now;
        //        var endDate = startDate.AddDays(plan.DurationInDays);

        //        var company = new Company
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Name = companyRequest.Name,
        //            PlanId = companyRequest.PlanId,
        //            ApplicationUserId = companyRequest.ApplicationUserId,
        //            Logo = fileName,
        //            StartDate = startDate,
        //            EndDate = endDate
        //        };

        //        var newCompany = await _companyRepository.CreateAsync(company);
        //        if (newCompany is not null)
        //        {
        //            return Created($"{Request.Scheme}://{Request.Host}/api/Admin/Companies/{newCompany.Id}", newCompany.Adapt<CompanyDetailsResponse>());
        //        }
        //    }

        //    return BadRequest();
        //}

        // add startdate manual not .now because when i want to edit start date in the next transaction of the same company

        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] CompanyRequest companyRequest)
        {
            if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                using (var stream = System.IO.File.Create(path))
                {
                    await companyRequest.Logo.CopyToAsync(stream);
                }

                var plan = _planRepository.GetOne(p => p.Id == companyRequest.PlanId);
                if (plan == null)
                    return BadRequest("Invalid PlanId.");

                var startDate = companyRequest.TransactionDate;
                var endDate = startDate.AddDays(plan.DurationInDays);

                var company = new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = companyRequest.Name,
                    PlanId = companyRequest.PlanId,
                    ApplicationUserId = companyRequest.ApplicationUserId,
                    Logo = fileName,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var newCompany = await _companyRepository.CreateAsync(company);
                if (newCompany != null)
                {
                    return Created($"{Request.Scheme}://{Request.Host}/api/Admin/Companies/{newCompany.Id}",
                                   newCompany.Adapt<CompanyResponse>());
                }
            }

            return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] CompanyRequest companyRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var companyInDB = _companyRepository.GetOne(e => e.Id == id, tracked: false);
            if (companyInDB == null)
                return NotFound();

            var plan = _planRepository.GetOne(p => p.Id == companyRequest.PlanId);
            if (plan == null)
                return BadRequest("Invalid PlanId.");

            var startDate = companyRequest.TransactionDate;
            var endDate = startDate.AddDays(plan.DurationInDays);

            var company = new Company
            {
                Id = companyInDB.Id,
                Name = companyRequest.Name,
                PlanId = companyRequest.PlanId,
                ApplicationUserId = companyRequest.ApplicationUserId,
                Logo = companyInDB.Logo,
                StartDate = startDate,
                EndDate = endDate
            };

            // Optional logo update
            if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);
                var path = Path.Combine(imagesPath, fileName);

                using (var stream = System.IO.File.Create(path))
                {
                    await companyRequest.Logo.CopyToAsync(stream);
                }

                // Delete old file
                var oldPath = Path.Combine(imagesPath, companyInDB.Logo ?? string.Empty);
                if (!string.IsNullOrEmpty(companyInDB.Logo) && System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                company.Logo = fileName;
            }

            var updatedCompany = await _companyRepository.EditAsync(company);
            return updatedCompany != null ? NoContent() : BadRequest("Update failed.");
        }




        //without start and end date 
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] CompanyRequest companyRequest)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var companyInDB = _companyRepository.GetOne(e => e.Id == id, tracked: false);
        //    if (companyInDB == null)
        //        return NotFound();

        //    var company = new Company
        //    {
        //        Id = companyInDB.Id,
        //        Name = companyRequest.Name,
        //        PlanId = companyRequest.PlanId,
        //        ApplicationUserId = companyRequest.ApplicationUserId,
        //        Logo = companyInDB.Logo
        //    };

        //    if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
        //    {
        //        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        //        if (!Directory.Exists(imagesPath))
        //            Directory.CreateDirectory(imagesPath);

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);
        //        var path = Path.Combine(imagesPath, fileName);

        //        using (var stream = System.IO.File.Create(path))
        //        {
        //            await companyRequest.Logo.CopyToAsync(stream);
        //        }

        //        var oldPath = Path.Combine(imagesPath, companyInDB.Logo ?? string.Empty);
        //        if (!string.IsNullOrEmpty(companyInDB.Logo) && System.IO.File.Exists(oldPath))
        //        {
        //            System.IO.File.Delete(oldPath);
        //        }

        //        company.Logo = fileName;
        //    }

        //    var newCompany = await _companyRepository.EditAsync(company);

        //    if (newCompany is not null)
        //        return NoContent();

        //    return BadRequest();
        //}

        //problem in start date if this is the same plan like previous month
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] CompanyRequest companyRequest)
        //{


        //    var companyInDB = _companyRepository.GetOne(e => e.Id == id, tracked: false);
        //    if (companyInDB == null)
        //        return NotFound();

        //    // Check if the plan has changed
        //    bool planChanged = companyRequest.PlanId != companyInDB.PlanId;

        //    DateTime? startDate = companyInDB.StartDate;
        //    DateTime? endDate = companyInDB.EndDate;

        //    if (planChanged)
        //    {
        //        var plan = _planRepository.GetOne(p => p.Id == companyRequest.PlanId);
        //        if (plan == null)
        //            return BadRequest("Invalid PlanId.");

        //        startDate = DateTime.Now;
        //        endDate = startDate?.AddDays(plan.DurationInDays);
        //    }

        //    var company = new Company
        //    {
        //        Id = companyInDB.Id,
        //        Name = companyRequest.Name,
        //        PlanId = companyRequest.PlanId,
        //        ApplicationUserId = companyRequest.ApplicationUserId,
        //        Logo = companyInDB.Logo,
        //        StartDate = startDate,
        //        EndDate = endDate
        //    };

        //    // Handle new logo upload
        //    if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
        //    {
        //        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        //        if (!Directory.Exists(imagesPath))
        //            Directory.CreateDirectory(imagesPath);

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);
        //        var path = Path.Combine(imagesPath, fileName);

        //        using (var stream = System.IO.File.Create(path))
        //        {
        //            await companyRequest.Logo.CopyToAsync(stream);
        //        }

        //        // Delete old logo
        //        var oldPath = Path.Combine(imagesPath, companyInDB.Logo ?? string.Empty);
        //        if (!string.IsNullOrEmpty(companyInDB.Logo) && System.IO.File.Exists(oldPath))
        //        {
        //            System.IO.File.Delete(oldPath);
        //        }

        //        company.Logo = fileName;
        //    }

        //    var updatedCompany = await _companyRepository.EditAsync(company);
        //    if (updatedCompany is not null)
        //        return NoContent();

        //    return BadRequest("Failed to update company.");
        //}



        //[HttpPut("{id}")]

        //public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] CompanyRequest companyRequest)
        //{

        //    var companyInDB = _companyRepository.GetOne(e => e.Id == id, tracked: false);
        //    //var company = companyRequest.Adapt<Company>();
        //    var company = new Company
        //    {
        //        Id = companyInDB.Id,
        //        Name = companyRequest.Name,
        //        PlanId = companyRequest.PlanId,
        //        ApplicationUserId = companyRequest.ApplicationUserId,
        //        Logo = companyInDB.Logo // default, will be replaced if new logo is uploaded
        //    };

        //    if (companyInDB is not null)
        //    {

        //        if (companyRequest.Logo is not null && companyRequest.Logo.Length > 0)
        //        {
        //            // Add file to Images
        //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companyRequest.Logo.FileName);

        //            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

        //            using (var stream = System.IO.File.Create(path))
        //            {
        //                await companyRequest.Logo.CopyToAsync(stream);
        //            }

        //            // Delete old Images
        //            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", companyInDB.Logo);

        //            if (System.IO.File.Exists(oldPath))
        //            {
        //                System.IO.File.Delete(oldPath);
        //            }

        //            // Add file Name to company in DB

        //            company.Logo = fileName;

        //        }
        //        else
        //        {
        //            company.Logo = companyInDB.Logo;
        //        }

        //        company.Id = companyInDB.Id;
        //        var newCompany = await _companyRepository.EditAsync(company);




        //        if (newCompany is not null)
        //        {
        //            return NoContent();
        //        }

        //        return BadRequest();

        //    }
        //    return NotFound();

        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var company = _companyRepository.GetOne(e => e.Id == id);
            if (company is not null)
            {
                // Delete old img from Images
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", company.Logo);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                var deletedCompany = await _companyRepository.DeleteAsync(company);
                if (deletedCompany is not null)
                {

                    return NoContent();
                }
                return BadRequest();
            }

            return NotFound();
        }

       


    }
}
