using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberGroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public MemberGroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var group = await _groupService.GetGroupByIdAsync(groupId);

            if (group != null) return Ok(group);
            else return BadRequest();
        }
        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetOrganizationById(int organizationId)
        {
            var organization = await _groupService.GetOrganizationByIdAsync(organizationId);

            if (organization != null) return Ok(organization);
            else return BadRequest();
        }
        [HttpGet("organization/{organizationId}/all")]
        public async Task<IActionResult> GetGroupsByOrganizationId(int organizationId)
        {
            var groups = await _groupService.GetGroupsByOrganizationIdAsync(organizationId);
            
            if (groups != null) return Ok(groups);
            else return BadRequest();
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
