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
        public async Task<IActionResult> CreateTimesheet(Timesheet timesheet)
        {
            return Ok();
        }
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTimesheet(/*ActivityRequirements[] requirements*/)
        {
            var testPresenterAvail = new bool[2][]
            {
                new bool[16],
                new bool[]
                {
                    false, false, true, true, true, false, false, false, false, true, true, false, false, false, true, true
                }
            };
            var generator = new TimesheetGenerator.TimesheetGenerator(16,
                new bool[2][] {new bool[16], new bool[16] },
                testPresenterAvail
                );
            generator.InitActivities(
                    new int[] {1, 2, 3 },
                    new int[] { 0, 0, 1},
                    new int[] { 0, 1, 1}
                );

            generator.Generate();

            return Ok(generator.generated);
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
