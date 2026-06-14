using AutoScheduler.Application.Services;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimesheetGenerator;

namespace AutoScheduler.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize]
	public class TimesheetController : ControllerBase
	{
        private readonly ITimesheetService _timesheetService;
        public TimesheetController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }
        [HttpGet("{timesheetId}")]
		public async Task<IActionResult> GetTimesheetById(int timesheetId)
		{
            return Ok();
        }
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetTimesheetByGroupId(int groupId)
        {
            var timesheet = await _timesheetService.GetTimesheetByGroupIdAsync(groupId);

            if (timesheet != null) return Ok(timesheet);
            else return BadRequest();
        }
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetTimesheetsForMember(int memberId)
        {
            return Ok();
        }
        [HttpGet("timeslot/member/{memberId}")]
        public async Task<IActionResult> GetTimeslotsForMember(int memberId)
        {
            return Ok();
        }
        [HttpGet("{timesheetId}/optimize")]
        public async Task<IActionResult> GetOptimizedTimesheet(int timesheetId)
        {
            return Ok();
        }
        [HttpGet("history/group/{groupId}")]
        public async Task<IActionResult> GetTimesheetHistoryByGroupId(int groupId)
        {
            return Ok();
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateTimesheet(TimesheetDTO timesheetDto)
        {
            var result = await _timesheetService.CreateTimesheetAsync(timesheetDto);

            if (result != null) return Ok(result);
            else return BadRequest();
        }
        [HttpPost("{timesheetId}/activate")]
        public async Task<IActionResult> ActivateTimesheet(int timesheetId)
        {
            await _timesheetService.ActivateTimesheetAsync(timesheetId);
            return Ok();
        }
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTimesheet(GeneratorRequirementsDTO generatorRequirementsDTO)
        {
            var generated = await _timesheetService.GenerateTimesheetAsync(generatorRequirementsDTO);

            return Ok(generated);
        }
        [HttpPost("timeslot/available")]
        public async Task<IActionResult> GetAvailableSpaceForTimeslot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            var availableSlots = await _timesheetService.GetAvailableSpaceForTimeslotAsync(timeslotPlacementChangeDTO);

            return Ok(availableSlots);
        }
        [HttpPost("timeslot/conflicting")]
        public async Task<IActionResult> GetConflictingForTimeslot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            var conflictingSlots = await _timesheetService.GetConflictingForTimeslot(timeslotPlacementChangeDTO);

            return Ok(conflictingSlots);
        }
        [HttpPost("regenerate")]
        public async Task<IActionResult> RegenerateTimesheet(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            var generated = await _timesheetService.RegenerateTimesheetAsync(timeslotPlacementChangeDTO);

            return Ok(generated);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTimesheet(Timesheet timesheet)
        {
            return Ok();
        }
        [HttpPut("timeslot/update")]
        public async Task<IActionResult> UpdateTimeslot(Timeslot timeslot)
        {
            return Ok();
        }
        [HttpPost("timeslot/new")]
        public async Task<IActionResult> CreateTimeslot(Timeslot timeslot)
        {
            return Ok();
        }
        [HttpDelete("delete/{timesheetId}")]
        public async Task<IActionResult> DeleteTimesheet(int timesheetId)
        {
            await _timesheetService.DeleteTimesheetAsync(timesheetId);
            
            return Ok();
        }
        [HttpDelete("timeslot/delete/{timeslotId}")]
        public async Task<IActionResult> DeleteTimeslot(int timeslotId)
        {
            return Ok();
        }
    }
}
