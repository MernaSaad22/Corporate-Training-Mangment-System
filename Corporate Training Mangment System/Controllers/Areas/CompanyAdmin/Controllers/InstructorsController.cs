using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Linq.Expressions;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class InstructorsController : ControllerBase
    {
        //private readonly IInstructorRepository _instructorRepository;

        //public InstructorsController(IInstructorRepository instructorRepository)
        //{
        //    _instructorRepository = instructorRepository;
        //}



        private readonly IInstructorRepository _instructorRepository;
        private readonly ICompanyRepository _companyRepository;

        public InstructorsController(IInstructorRepository instructorRepository, ICompanyRepository companyRepository)
        {
            _instructorRepository = instructorRepository;
            _companyRepository = companyRepository;
        }


        //[HttpGet("")]
        //public async Task<ActionResult<IEnumerable<InstructorResponse>>> GetAll()
        //{
        //    var instructors = await _instructorRepository.GetAsync(includes: [e => e.ApplicationUser]);


        //    TypeAdapterConfig<Entities.Instructor, InstructorResponse>.NewConfig()
        // .Map(dest => dest.FullName, src => src.ApplicationUser.UserName);
        //    return Ok(instructors.Adapt<IEnumerable<InstructorResponse>>());
        //}

        //using authorization
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<InstructorResponse>>> GetAll()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null) return Unauthorized();

            var instructors = await _instructorRepository.GetAsync(
                i => i.CompanyId == company.Id,
                includes: [i => i.ApplicationUser]);

            TypeAdapterConfig<Instructor, InstructorResponse>.NewConfig()
                .Map(dest => dest.FullName, src => src.ApplicationUser.UserName);

            return Ok(instructors.Adapt<IEnumerable<InstructorResponse>>());
        }





        //fullname==>null?
        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] string id)
        //{
        //    var instructor = _instructorRepository.GetOne(e => e.Id == id);
        //    if (instructor is not null)

        //        return Ok(instructor.Adapt<InstructorResponse>());

        //    return NotFound();
        //}

        //i try to solve the fullname problem and it be solved):
        //[HttpGet("{id}")]
        //public IActionResult GetOne([FromRoute] string id)
        //{
        //    var instructor = _instructorRepository.GetOne(
        //        e => e.Id == id,
        //        includes: new Expression<Func<Instructor, object>>[] { e => e.ApplicationUser });

        //    if (instructor == null)
        //        return NotFound();
        //    TypeAdapterConfig<Entities.Instructor, InstructorResponse>.NewConfig()
        //        .Map(dest => dest.FullName, src => src.ApplicationUser != null ? src.ApplicationUser.UserName : string.Empty);


        //    return Ok(instructor.Adapt<InstructorResponse>());
        //}

        //add authorization
        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string id)
        {
            // ✅ Get logged-in user ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            // ✅ Get the company for the current user
            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null)
                return Unauthorized();

            // ✅ Load instructor including ApplicationUser
            var instructor = _instructorRepository.GetOne(
                e => e.Id == id,
                includes: new Expression<Func<Instructor, object>>[] { e => e.ApplicationUser });

            if (instructor == null)
                return NotFound();

            // ✅ Ownership check
            if (instructor.CompanyId != company.Id)
                return Forbid();

            // ✅ Manual mapping to avoid null-operator issue
            var response = new InstructorResponse
            {
                Id = instructor.Id,
                FullName = instructor.ApplicationUser?.UserName ?? string.Empty,
                ApplicationUserId = instructor.ApplicationUserId,
                Specialization = instructor.Specialization,
                CompanyId = instructor.CompanyId
            };

            return Ok(response);
        }





        //[HttpPost("")]
        //public async Task<IActionResult> Create([FromBody] InstructorRequest instructorRequest)
        //{

        //    var newInstructor = await _instructorRepository.CreateAsync(instructorRequest.Adapt<Instructor>());
        //    if (newInstructor is not null)
        //    {
        //        return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Instructors/{newInstructor.Id}", newInstructor);
        //    }
        //    return BadRequest();
        //}

        //correct without authorzation
        //[HttpPost("")]
        //public async Task<IActionResult> Create([FromBody] InstructorRequest instructorRequest)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var instructor = instructorRequest.Adapt<Instructor>();
        //    instructor.Id = Guid.NewGuid().ToString();

        //    var newInstructor = await _instructorRepository.CreateAsync(instructor);

        //    if (newInstructor != null)
        //    {
        //        var response = newInstructor.Adapt<InstructorResponse>();
        //        return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Instructors/{newInstructor.Id}", response);
        //    }

        //    return BadRequest("Failed to create instructor.");
        //}

        //add authorization

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] InstructorRequest instructorRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null) return Unauthorized();

            //renew subscription
            if (company.EndDate < DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");


            var instructor = instructorRequest.Adapt<Instructor>();
            instructor.Id = Guid.NewGuid().ToString();
            instructor.CompanyId = company.Id;

            var newInstructor = await _instructorRepository.CreateAsync(instructor);

            if (newInstructor != null)
            {
                var withUser = _instructorRepository.GetOne(
                    i => i.Id == newInstructor.Id,
                    includes: [i => i.ApplicationUser]);

                TypeAdapterConfig<Instructor, InstructorResponse>.NewConfig()
                    .Map(dest => dest.FullName, src => src.ApplicationUser.UserName);

                return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Instructors/{newInstructor.Id}",
                    withUser.Adapt<InstructorResponse>());
            }

            return BadRequest("Failed to create instructor.");
        }






        //i want to solve null fullname but not solver ):
        //[HttpPost("")]
        //public async Task<IActionResult> Create([FromBody] InstructorRequest instructorRequest)
        //{
        //    var newInstructorEntity = instructorRequest.Adapt<Instructor>();

        //    var createdInstructor = await _instructorRepository.CreateAsync(newInstructorEntity);
        //    if (createdInstructor is not null)
        //    {
        //        // Fetch the created instructor with ApplicationUser included
        //        var instructorWithUser = _instructorRepository.GetOne(
        //            e => e.Id == createdInstructor.Id,
        //            includes: new Expression<Func<Instructor, object>>[] { e => e.ApplicationUser });

        //        // Map with FullName from ApplicationUser.UserName
        //        TypeAdapterConfig<Entities.Instructor, InstructorResponse>.NewConfig()
        //            .Map(dest => dest.FullName, src => src.ApplicationUser.UserName);

        //        var response = instructorWithUser.Adapt<InstructorResponse>();

        //        return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Instructors/{response.Id}", response);
        //    }

        //    return BadRequest();
        //}


        //correct without authorization 
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] InstructorRequest instructorRequest)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Find existing instructor
        //    var existingInstructor = _instructorRepository.GetOne(e => e.Id == id);
        //    if (existingInstructor == null)
        //        return NotFound();

        //    // Map updated fields from request to entity
        //    instructorRequest.Adapt(existingInstructor);

        //    var updatedInstructor = await _instructorRepository.EditAsync(existingInstructor);

        //    if (updatedInstructor != null)
        //    {
        //        var response = updatedInstructor.Adapt<InstructorResponse>();
        //        return Ok(response);
        //    }

        //    return BadRequest("Failed to update instructor.");
        //}




        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] InstructorRequest instructorRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null) return Unauthorized();


            //renew subscription
            if (company.EndDate < DateTime.Now)
                return Ok("Your subscription has expired. Please renew your plan.");


            var existingInstructor = _instructorRepository.GetOne(i => i.Id == id);
            if (existingInstructor == null) return NotFound();
            if (existingInstructor.CompanyId != company.Id) return Forbid();

            instructorRequest.Adapt(existingInstructor);

            var updatedInstructor = await _instructorRepository.EditAsync(existingInstructor);
            if (updatedInstructor != null)
                return Ok(updatedInstructor.Adapt<InstructorResponse>());

            return BadRequest("Failed to update instructor.");
        }


        //correct without authorization
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] string id)
        //{
        //    var existingInstructor = _instructorRepository.GetOne(e => e.Id == id);
        //    if (existingInstructor == null)
        //        return NotFound();

        //    var deletedInstructor = await _instructorRepository.DeleteAsync(existingInstructor);
        //    if (deletedInstructor != null)
        //    {
        //        return NoContent(); // 204 No Content on successful deletion
        //    }

        //    return BadRequest("Failed to delete instructor.");
       // }

    //with authorization
    [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var company = _companyRepository.GetOne(c => c.ApplicationUserId == userId);
            if (company == null) return Unauthorized();

            var instructor = _instructorRepository.GetOne(i => i.Id == id);
            if (instructor == null) return NotFound();
            if (instructor.CompanyId != company.Id) return Forbid();

            var deleted = await _instructorRepository.DeleteAsync(instructor);
            if (deleted != null) return NoContent();

            return BadRequest("Failed to delete instructor.");
        }



        // if i want to delete instructor and userapplication from DB it willl be like this i will discuss with my team

        //private readonly IInstructorRepository _instructorRepository;
        //private readonly UserManager<ApplicationUser> _userManager;

        //public InstructorsController(IInstructorRepository instructorRepository, UserManager<ApplicationUser> userManager)
        //{
        //    _instructorRepository = instructorRepository;
        //    _userManager = userManager;
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] string id)
        //{
        //    var instructor = _instructorRepository.GetOne(e => e.Id == id, includes: new Expression<Func<Instructor, object>>[] { e => e.ApplicationUser });
        //    if (instructor == null)
        //        return NotFound();

        //    // Delete ApplicationUser first
        //    if (instructor.ApplicationUser != null)
        //    {
        //        var result = await _userManager.DeleteAsync(instructor.ApplicationUser);
        //        if (!result.Succeeded)
        //        {
        //            return BadRequest("Failed to delete linked user account.");
        //        }
        //    }

        //    // Delete Instructor entity
        //    var deleted = await _instructorRepository.DeleteAsync(instructor);
        //    if (deleted == null)
        //    {
        //        return BadRequest("Failed to delete instructor.");
        //    }

        //    return NoContent();
        //}



    }
}
