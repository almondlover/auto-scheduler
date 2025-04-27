using AutoScheduler.Domain.Entities.Activities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        [HttpGet("{activityId}")]
        public async Task<IActionResult> GetActivityById(int activityId)
        {
            return Ok();
        }
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetActivitiesByMemberId(int memberId)
        {
            return Ok();
        }
        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetActivitiesByOrganizationId(int organizationId)
        {
            return Ok();
        }
        [HttpGet("requirements/group/{groupId}")]
        public async Task<IActionResult> GetRequirementsByGroupId(int groupId)
        {
            return Ok();
        }
        [HttpPost("new")]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return Ok();
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateActivity(Activity activity)
        {
            return Ok();
        }
        [HttpDelete("delete/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            return Ok();
        }
    }
}
