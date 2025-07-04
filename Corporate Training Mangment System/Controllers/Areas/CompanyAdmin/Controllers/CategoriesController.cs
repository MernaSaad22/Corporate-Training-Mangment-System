using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;

namespace Corporate_Training_Mangment_System.Controllers.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICourseCategoryRepository _coursecategoryRepository;
        public CategoriesController(ICourseCategoryRepository coursecategoryRepository)
        {
             _coursecategoryRepository = coursecategoryRepository;
        }
        //CourseCategory==>Name is not unique

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<CourseCategoruResponse>>> GetAll()
        {
            var categories = await _coursecategoryRepository.GetAsync();


            var config = new TypeAdapterConfig();
            return Ok(categories.Adapt<IEnumerable<CourseCategoruResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var category = _coursecategoryRepository.GetOne(e => e.Id == id);
            if (category is not null)

                return Ok(category.Adapt<CourseCategoruResponse>());

            return NotFound();
        }
        [HttpPost("")]

        public async Task<IActionResult> Create([FromBody] CourseCategoryRequest coursecategoryRequest)
        {


            var newCategory = await _coursecategoryRepository.CreateAsync(coursecategoryRequest.Adapt<CourseCategory>());
            if (newCategory is not null)
            {
                return Created($"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/Categories/{newCategory.Id}", newCategory);
            }
            return BadRequest();

        }


        [HttpPut("{id}")]

        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CourseCategoryRequest coursecategoryRequest)
        {

            var categoryInDB = _coursecategoryRepository.GetOne(e => e.Id == id, tracked: false);
            if (categoryInDB is not null)
            {
                var newCourseCategoryRequest = coursecategoryRequest.Adapt<CourseCategory>();
               newCourseCategoryRequest.Id = categoryInDB.Id;
                var newCategory = await _coursecategoryRepository.EditAsync(newCourseCategoryRequest);

                if (newCategory is not null)
                {
                    return NoContent();
                }

                return BadRequest();

            }
            return NotFound();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var category = _coursecategoryRepository.GetOne(e => e.Id == id);
            if (category is not null)
            {
                var deletedCategory = await _coursecategoryRepository.DeleteAsync(category);
                if (deletedCategory is not null)
                {
                    return NoContent();
                }
                return BadRequest();
            }

            return NotFound();
        }

    }
}
