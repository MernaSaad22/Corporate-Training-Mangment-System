using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Diagnostics;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICourseCategoryRepository _coursecategoryRepository;
        private readonly ICompanyRepository _companyRepository;


        public CategoriesController(ICourseCategoryRepository coursecategoryRepository,ICompanyRepository companyRepository)
        {
             _coursecategoryRepository = coursecategoryRepository;
            _companyRepository= companyRepository;
        }
        //CourseCategory==>Name is not unique


        //manually test
        //[HttpGet("")]
        //public async Task<ActionResult<IEnumerable<CourseCategoruResponse>>> GetAll()
        //{
        //    //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    //we dont have JWT and Authorize so i put userId manually
        //    var userId = "90d5f065-d5a5-41ec-a195-d1910d97d86e";

        //    var company =  _companyRepository.GetOne(c => c.ApplicationUserId == userId);
        //    if (company == null)
        //        return Unauthorized();
        //    // var categories = await _coursecategoryRepository.GetAsync();
        //    var categories = await _coursecategoryRepository.GetAsync(c => c.CompanyId == company.Id);



        //    var config = new TypeAdapterConfig();
        //    return Ok(categories.Adapt<IEnumerable<CourseCategoruResponse>>());
        //}

        //NOT WORK ):====>finally worked

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<CourseCategoruResponse>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Console.WriteLine("UserId: " + userId);
            

            if (userId is null)
                return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
           

            if (company == null)
                return Unauthorized();
            //when i test if enddate<datetime for a company so it's plan is expired 
            if (company.EndDate < DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");

            var categories = await _coursecategoryRepository.GetAsync(c => c.CompanyId == company.Id);
            return Ok(categories.Adapt<IEnumerable<CourseCategoruResponse>>());
        }


       


        // worked code to catch error message
        //when we register as company we need to add applicationuser from createcompany as SuperAdmin first

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CourseCategoruResponse>>> GetAll()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized("User ID not found in token");
        //    }

        //    var company =  _companyRepository.GetOne(c =>
        //        c.ApplicationUserId != null &&
        //        c.ApplicationUserId.Equals(userId));

        //    if (company == null)
        //    {
        //        return Unauthorized($"No company found for user ID: {userId}");
        //    }

        //    var categories = await _coursecategoryRepository.GetAsync(
        //        c => c.CompanyId == company.Id);

        //    return Ok(categories.Adapt<IEnumerable<CourseCategoruResponse>>());
        //}



        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] int id)
        //{
        //    var category = _coursecategoryRepository.GetOne(e => e.Id == id);
        //    if (category is not null)

        //        return Ok(category.Adapt<CourseCategoruResponse>());

        //    return NotFound();
        //}



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = "90d5f065-d5a5-41ec-a195-d1910d97d86e";

            if (userId is null)
                return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null)
                return Unauthorized();

            var category = _coursecategoryRepository.GetOne(e => e.Id == id);
            if (category is null)
                return NotFound();

            // 🔐 Ownership check
            if (category.CompanyId != company.Id)
                return Forbid();

            return Ok(category.Adapt<CourseCategoruResponse>());
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] CourseCategoryRequest coursecategoryRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = "1ba0cbf6-0596-4347-b001-fbd181019a38";
            //diffrentUsers
            //var userId = "90d5f065-d5a5-41ec-a195-d1910d97d86e";

            if (userId == null)
                return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized();

            var newCategory = coursecategoryRequest.Adapt<CourseCategory>();
            newCategory.CompanyId = company.Id;

            var createdCategory = await _coursecategoryRepository.CreateAsync(newCategory);

            if (createdCategory != null)
            {
                return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Categories/{createdCategory.Id}", createdCategory);
            }
            return BadRequest();
        }


        //[HttpPost("")]

        //public async Task<IActionResult> Create([FromBody] CourseCategoryRequest coursecategoryRequest)
        //{

        //   // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    //var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
        //   // if (company == null)
        //      //  return Unauthorized();
        //    var newCategory = await _coursecategoryRepository.CreateAsync(coursecategoryRequest.Adapt<CourseCategory>());
        //    if (newCategory is not null)
        //    {
        //        return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Categories/{newCategory.Id}", newCategory);
        //    }
        //    return BadRequest();

        //}


        //[HttpPut("{id}")]

        //public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseCategoryRequest coursecategoryRequest)
        //{

        //    var categoryInDB = _coursecategoryRepository.GetOne(e => e.Id == id, tracked: false);
        //    if (categoryInDB is not null)
        //    {
        //        var newCourseCategoryRequest = coursecategoryRequest.Adapt<CourseCategory>();
        //       newCourseCategoryRequest.Id = categoryInDB.Id;
        //        var newCategory = await _coursecategoryRepository.EditAsync(newCourseCategoryRequest);

        //        if (newCategory is not null)
        //        {
        //            return NoContent();
        //        }

        //        return BadRequest();

        //    }
        //    return NotFound();

        //}



        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseCategoryRequest coursecategoryRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = "90d5f065-d5a5-41ec-a195-d1910d97d86e";

            if (userId is null)
                return Unauthorized();

            var company =  _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null)
                return Unauthorized();

            var categoryInDB = _coursecategoryRepository.GetOne(e => e.Id == id, tracked: false);
            if (categoryInDB is null)
                return NotFound();

            // Check ownership
            if (categoryInDB.CompanyId != company.Id)
                return Forbid();

            var updatedCategory = coursecategoryRequest.Adapt<CourseCategory>();
            updatedCategory.Id = id;
            updatedCategory.CompanyId = company.Id; // Ensure company ID stays the same

            var result = await _coursecategoryRepository.EditAsync(updatedCategory);
            if (result is not null)
                return NoContent();

            return BadRequest();
        }


        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    var category = _coursecategoryRepository.GetOne(e => e.Id == id);
        //    if (category is not null)
        //    {
        //        var deletedCategory = await _coursecategoryRepository.DeleteAsync(category);
        //        if (deletedCategory is not null)
        //        {
        //            return NoContent();
        //        }
        //        return BadRequest();
        //    }

        //    return NotFound();
        //}



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = "90d5f065-d5a5-41ec-a195-d1910d97d86e";

            if (userId is null)
                return Unauthorized();

            var company =  _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company is null)
                return Unauthorized();

            var category = _coursecategoryRepository.GetOne(e => e.Id == id);
            if (category is null)
                return NotFound();

            // Check ownership
            if (category.CompanyId != company.Id)
                return Forbid();

            var deletedCategory = await _coursecategoryRepository.DeleteAsync(category);
            if (deletedCategory is not null)
                return NoContent();

            return BadRequest();
        }


    }
}
