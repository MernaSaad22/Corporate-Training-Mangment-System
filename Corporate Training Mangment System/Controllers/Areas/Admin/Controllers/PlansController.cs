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
    public class PlansController : ControllerBase
    {

        private readonly IPlanRepository _planRepository;
        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<PlanResponse>>> GetAll()
        {
            var plans = await _planRepository.GetAsync();


            var config = new TypeAdapterConfig();
            return Ok(plans.Adapt<IEnumerable<PlanResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var plan = _planRepository.GetOne(e => e.Id == id);
            if (plan is not null)

                return Ok(plan.Adapt<PlanResponse>());

            return NotFound();
        }
        [HttpPost("")]

        public async Task<IActionResult> Create([FromBody] PlanRequest planRequest)
        {


            var newPlan = await _planRepository.CreateAsync(planRequest.Adapt<Plan>());
            if (newPlan is not null)
            {
                return Created($"{Request.Scheme}://{Request.Host}/api/Admin/Plans/{newPlan.Id}", newPlan);
            }
            return BadRequest();

        }


        [HttpPut("{id}")]

        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] PlanRequest planRequest)
        {

            var planInDB = _planRepository.GetOne(e => e.Id == id, tracked: false);
            if (planInDB is not null)
            {
                var newPlanRequest = planRequest.Adapt<Plan>();
                newPlanRequest.Id = planInDB.Id;
                var newPlan = await _planRepository.EditAsync(newPlanRequest);

                if (newPlan is not null)
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
            var plan = _planRepository.GetOne(e => e.Id == id);
            if (plan is not null)
            {
                var deletedPlan = await _planRepository.DeleteAsync(plan);
                if (deletedPlan is not null)
                {
                    return NoContent();
                }
                return BadRequest();
            }

            return NotFound();
        }

    }
}
