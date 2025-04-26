using AutoScheduler.Domain.Entities.MemberGroups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberGroupController : ControllerBase
    {
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            return Ok();
        }
        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetOrganizationById(int organizationId)
        {
            return Ok();
        }
        [HttpGet("organization/{organizationId}/all")]
        public async Task<IActionResult> GetGroupsByOrganizationId(int organizationId)
        {
            return Ok();
        }
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetGroupsByMemberId(int memberId)
        {
            return Ok();
        }
        [HttpGet("{groupId}/activity/{activityId}")]
        public async Task<IActionResult> GetMemberForGroupActivity(int groupId, int activityId)
        {
            return Ok();
        }
        
        [HttpPost("new")]
        public async Task<IActionResult> CreateGroup(Group group)
        {
            return Ok();
        }
        [HttpPost("organization/new")]
        public async Task<IActionResult> CreateOrganization(Organization organization)
        {
            return Ok();
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup(Group group)
        {
            return Ok();
        }
        [HttpPut("organization/update")]
        public async Task<IActionResult> UpdateOrganization(Organization organization)
        {
            return Ok();
        }
        [HttpDelete("delete/{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            return Ok();
        }
        [HttpDelete("delete/{organizationId}")]
        public async Task<IActionResult> DeleteOrganization(int organizationId)
        {
            return Ok();
        }
    }
}
