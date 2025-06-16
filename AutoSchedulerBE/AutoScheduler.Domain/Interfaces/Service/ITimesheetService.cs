using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Service
{
	public interface ITimesheetService
	{
		public Task<Timesheet> GetTimesheetByIdAsync(int timesheetId);
		public Task<Timesheet> GetTimesheetByGroupIdAsync(int groupId);
		public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId);
		public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId);
		public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId);
		public Task<IList<TimeslotDTO[]>> GenerateTimesheetAsync(GeneratorRequirementsDTO generatorRequirementsDTO);
		public Task CreateTimesheetAsync(TimesheetDTO timesheetDto);
		public Task UpdateTimesheetAsync(Timesheet timesheet);
		public Task DeleteTimesheetAsync(int timesheetId);
	}
}
