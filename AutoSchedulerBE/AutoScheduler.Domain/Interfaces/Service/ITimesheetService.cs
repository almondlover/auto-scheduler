using AutoScheduler.Domain.DTOs;
using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Enums;
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
		public Task<IList<TimesheetDTO>> GetTimesheetByGroupIdAsync(int groupId, TimesheetState state);
		public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId);
		public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId);
		public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId);
		public Task<IList<TimesheetDTO>> GenerateTimesheetAsync(GeneratorRequirementsDTO generatorRequirementsDTO);
		public Task<TimesheetDTO> CreateTimesheetAsync(TimesheetDTO timesheetDto);
		public Task UpdateTimesheetAsync(TimesheetDTO timesheetDto);
		public Task DeleteTimesheetAsync(int timesheetId);
        Task<IList<WeekDayTimeRangeDTO>> GetAvailableSpaceForTimeslotAsync(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO);
        Task<IList<TimeslotDTO>> GetConflictingForTimeslot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO);
        Task<IList<TimesheetDTO>> RegenerateTimesheetAsync(TimeslotRearrangementDTO timeslotRearrangementDto);
		Task ActivateTimesheetAsync(int timesheetId);
		Task<IList<HallDTO>> GetPossibleHallsForSlot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO);
        Task DeactivateTimesheetAsync(int timesheetId);
    }
}
