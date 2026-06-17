using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Enums;

namespace AutoScheduler.Domain.Interfaces.Repository
{
    public interface ITimesheetRepository
    {
        public Task<Timesheet> GetTimesheetByIdAsync(int timesheetId);
        public Task<IList<Timesheet>> GetTimesheetByGroupIdAsync(int groupId, TimesheetState state);
        public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId);
        public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId);
        public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId);
        public Task GenerateTimesheetAsync();
        public Task CreateTimesheetAsync(Timesheet timesheet);
        public Task CreateAvailabilityRangeAsync(IList<Availability> availabilityList);
        public Task UpdateTimesheetAsync(Timesheet timesheet);
        public Task DeleteTimesheetAsync(int timesheetId);
        public Task DeleteTimeslotsAvailability(IList<Timeslot> timeslots);
        public Task<IList<Hall[]>> GetHallsForRequirementsAsync (ActivityRequirements[] requirements);
        public Task<IList<Group>> GetGroupsForRequirementsAsync(ActivityRequirements[] requirements);
        Task<IList<ActivityRequirements>> GetRequirementsForTimesheetAsync(int timesheetId);
    }
}
