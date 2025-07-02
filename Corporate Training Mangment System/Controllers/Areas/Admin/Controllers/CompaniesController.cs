using Azure.Core;
using DataAccess.IRepository;
using Entities;
using Mapster;
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
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<CompanyResponse>>> GetAll()
        {
            var companies = await _companyRepository.GetAsync();


            var config = new TypeAdapterConfig();
            return Ok(companies.Adapt<IEnumerable<CompanyResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string id)
        {
            var company = _companyRepository.GetOne(e => e.Id == id);
            if (company is not null)
               
                return Ok(company.Adapt<CompanyResponse>());

            return NotFound();
        }
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
                //var company = companyRequest.Adapt<Company>();

                var company = new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = companyRequest.Name,
                    PlanId = companyRequest.PlanId,
                    ApplicationUserId = companyRequest.ApplicationUserId,
                    Logo = fileName
                };
                //company.Logo = fileName;
                var newCompany = await _companyRepository.CreateAsync(company);
                if (newCompany is not null)
                {
                    return Created($"{Request.Scheme}://{Request.Host}/api/Admin/Companies/{newCompany.Id}", newCompany.Adapt<CompanyResponse>());
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

            var company = new Company
            {
                Id = companyInDB.Id,
                Name = companyRequest.Name,
                PlanId = companyRequest.PlanId,
                ApplicationUserId = companyRequest.ApplicationUserId,
                Logo = companyInDB.Logo
            };

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

                var oldPath = Path.Combine(imagesPath, companyInDB.Logo ?? string.Empty);
                if (!string.IsNullOrEmpty(companyInDB.Logo) && System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                company.Logo = fileName;
            }

            var newCompany = await _companyRepository.EditAsync(company);

            if (newCompany is not null)
                return NoContent();

            return BadRequest();
        }





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
