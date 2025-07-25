﻿using AutoScheduler.Application.Services;
using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var requirements = await _activityService.GetRequirementsByGroupIdAsync(groupId);

            if (requirements != null) return Ok(requirements);
            else return BadRequest();
        }
        [HttpGet("hallTypes/all")]
        public async Task<IActionResult> GetAllHallTypes()
        {
            var hallTypes = await _activityService.GetAllHallTypesAsync();

            if (hallTypes != null) return Ok(hallTypes);
            else return BadRequest();
        }
        [HttpPost("new")]
        public async Task<IActionResult> CreateActivity(ActivityDTO activityDto)
        {
            await _activityService.CreateActivityAsync(activityDto);

            if (activityDto != null) return Ok(activityDto);
            else return BadRequest();
        }
        [HttpPost("halls/new")]
        [Authorize(Roles = "ResourceManager")]
        public async Task<IActionResult> CreateHall(HallDTO hallDto)
        {
            await _activityService.CreateHallAsync(hallDto);

            if (hallDto != null) return Ok(hallDto);
            else return BadRequest();
        }
        [HttpPost("requirement/new")]
        public async Task<IActionResult> CreateActivityRequirements(ActivityRequirementsDTO requirementsDto)
        {
            await _activityService.CreateActivityRequirementsAsync(requirementsDto);

            if (requirementsDto != null) return Ok(requirementsDto);
            else return BadRequest();
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateActivity(ActivityDTO activityDto)
        {
            await _activityService.UpdateActivityAsync(activityDto);

            return Ok(activityDto);
        }
        [HttpPut("hall/update")]
        public async Task<IActionResult> UpdateHall(HallDTO hallDTO)
        {
            await _activityService.UpdateHallAsync(hallDTO);

            return Ok(hallDTO);
        }
        [HttpDelete("delete/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            await _activityService.DeleteActivityAsync(activityId);

            return Ok();
        }
        [HttpDelete("hall/delete/{hallId}")]
        public async Task<IActionResult> DeleteHall(int hallId)
        {
            await _activityService.DeleteHallAsync(hallId);

            return Ok();
        }
    }
}
