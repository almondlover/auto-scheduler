using AutoScheduler.Application.Services;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }
        [HttpGet("{activityId}")]
        public async Task<IActionResult> GetActivityById(int activityId)
        {
            var activity = await _activityService.GetActivityByIdAsync(activityId);

            if (activity != null) return Ok(activity);
            else return BadRequest();
        }
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetActivitiesByMemberId(int memberId)
        {
            return Ok();
        }
        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetActivitiesByOrganizationId(int organizationId)
        {
            var activities = await _activityService.GetActivitiesByOrganizationIdAsync(organizationId);

            if (activities != null) return Ok(activities);
            else return BadRequest();
        }
        [HttpGet("requirements/group/{groupId}")]
        public async Task<IActionResult> GetRequirementsByGroupId(int groupId)
        {
            return Ok();
        }
        [HttpPost("new")]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            await _activityService.CreateActivityAsync(activity);

            if (activity != null) return Ok(activity);
            else return BadRequest();
        }
        [HttpPost("requirement/new")]
        public async Task<IActionResult> CreateActivityRequirements(ActivityRequirements requirements)
        {
            await _activityService.CreateActivityRequirementsAsync(requirements);

            if (requirements != null) return Ok(requirements);
            else return BadRequest();
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateActivity(Activity activity)
        {
            await _activityService.UpdateActivityAsync(activity);

            return Ok(activity);
        }
        [HttpDelete("delete/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            await _activityService.DeleteActivityAsync(activityId);

            return Ok();
        }
    }
}
