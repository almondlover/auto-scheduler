using AutoScheduler.Application.Services;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimesheetGenerator;

namespace AutoScheduler.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
            return Ok();
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
            await _timesheetService.CreateTimesheetAsync(timesheetDto);

            if (timesheetDto != null) return Ok(timesheetDto);
            else return BadRequest();
        }
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTimesheet(GeneratorRequirementsDTO generatorRequirementsDTO)
        {
            var generated = await _timesheetService.GenerateTimesheetAsync(generatorRequirementsDTO);

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
            return Ok();
        }
        [HttpDelete("timeslot/delete/{timeslotId}")]
        public async Task<IActionResult> DeleteTimeslot(int timeslotId)
        {
            return Ok();
        }
    }
}
